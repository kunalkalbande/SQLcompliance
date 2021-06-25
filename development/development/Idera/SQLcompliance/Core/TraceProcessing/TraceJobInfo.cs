using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Collections;
using System.IO;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Rules ;
using Idera.SQLcompliance.Core.Stats;
using Idera.SQLcompliance.Core.TimeZoneHelper;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for TraceJobInfo.
	/// </summary>
   internal class TraceJobInfo
   {
      #region enums
      
      public enum State : int
      {
         New                = 0,
         WaitingToRun       = 1,
         Running            = 2,
         Failed             = 3,
         UnrecoverableError = 4,
         Done               = 5,
         Aborting           = 6
      }

      #endregion
      
      #region Constructor
      
      public TraceJobInfo()
      {
         traceEvents = new Hashtable();
      }
      
      #endregion
      
      #region Private Data Members

      private Dictionary<string, int> appIdTable = new Dictionary<string, int>();
      private Dictionary< string, int> hostIdTable = new Dictionary< string, int>( );
      private Dictionary< string, int> loginIdTable = new Dictionary< string, int>( );
      private Dictionary<string, int> tableIdTable = new Dictionary<string, int>();
      private Dictionary<string, int> columnIdTable = new Dictionary<string, int>();
      private static Queue<TraceJobInfo> jobQueue = new Queue<TraceJobInfo>();
      private static object queueLock = new object();
      internal static bool newJobsFirst = false;

      private string agentVersion;

      private static object logLock = new object();
      private static int logIndex = -1;
      private static Dictionary<int, System.Diagnostics.Stopwatch> logTimers = new Dictionary<int, System.Diagnostics.Stopwatch>();

      #endregion

      #region Properties

      // properties in Jobs Table
      public int              jobId;      
	   public string           instance;
	   public DateTime         dateReceived;
      public State            state;
      public bool             aborting = false;
      public string           tempTablePrefix;
      public bool             privilegedUserTrace;
      public bool             sqlSecureTrace;

      public int              sqlVersion = 9; // 8=2000; 9=2005; 10=2008; 11=2012, 12=2014
      public bool             isSqlServer2005
      {
         get{ return (sqlVersion>=9); }
      }

      // NOTE:  We have overloaded this field to contain file:lastFinalEventProcessed
      //  where lastRowProcessed correlates to the RowCount in WriteFinalEventsTable
      public string compressedTraceFile;
      public int lastFinalEventProcessed = 0;
      public int traceChecksum = 0;

      public string           compressedAuditFile;
      public int              auditChecksum = 0;
      
      // properties used during processing
      public string           traceFile  = "";
      public string           auditFile  = "";
      public string           eventDatabase = "";
      public int              traceType     = 0;
      public int              traceCategory = 0;
      public int              traceSequence = 0;
      public int              traceLevel    = 0;
      public bool             keepingSql = false;
      public bool             keepingSqlXE = false; // 5.4 XE
      public bool             keepingAdminSql = false; //Added By Hemant
      public int              maxSql     = CoreConstants.DefaultMaxSqlLength;
      
      public TimeZoneHelper.TimeZoneInfo     timeZoneInfo;
      
      public int              eventsTotal      = 0;
      public int              eventsInserted   = 0;
      public int              eventsUpdated    = 0;
      
      public int              eventsOverlap     = 0;
      public int              eventsDeleted     = 0;
      public int              eventsDuplicated  = 0;
      public int              eventsFilteredOut = 0;
      public int              eventsSQLcm       = 0;
      public int              uncompressTime;
      
      public Hashtable        traceEvents;
      public Hashtable        privEvents = new Hashtable();
      public Hashtable        privUsers = new Hashtable();
      public Hashtable        databaseNames = new Hashtable();
      public ArrayList        filterSets;   
      
      public DateTime         firstEventTime = DateTime.MinValue;
      public DateTime         lastEventTime  = DateTime.MinValue;

      public bool             privSELECT;
      public bool             privDML;
      public bool             privDMLwithDetailsXE;

      public RuleProcessor eventFilterProcessor ;
      public InstanceStats iStats;
      public InstanceStats newStats;
      
      public Dictionary<int, List<string>> dcTableLists;
      public Dictionary<int, Dictionary<string, TableConfiguration>> scTableLists;
      public Dictionary<int, DBAuditConfiguration> dbconfigs; // added for SQLCM-2216
      public AuditConfiguration userConfigs;
      #endregion
      
      #region Public Methods
      
      //-----------------------------------------------------------------------
      // GetNextJob - reads Jobs table to load next available job
      //-----------------------------------------------------------------------
      static public TraceJobInfo
         GetNextJob()
      {
		   TraceJobInfo jobInfo = null;
		   if( jobQueue.Count == 0 )
		   {
	         GetJobs();
		   }
		   
		   try
		   {
		      jobInfo = jobQueue.Dequeue();
		   }
		   catch( InvalidOperationException )
		   {
		      // the queue is empty
		   }
		   catch( Exception ex )
		   {
		      ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
		                               "An error occurred accessing the job queue.",
		                               ex,
		                               ErrorLog.Severity.Warning );
		      throw;
		   }
		   
		   return jobInfo;
		   
      }
		

	   //-----------------------------------------------------------------------
      // UpdateState - set state to new value and write to Jobs Table
      //-----------------------------------------------------------------------
      public void
         UpdateState(
            State             newState
         )
      {
		   Repository repository = new Repository();
		   
         state = newState;

         string logFunction = "UpdateState";
         int logIndex = -1;

         try
         {
            logFunction = String.Format("UpdateState job {0} to {1}", jobId, state);
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();
            
		      string sqlText = String.Format( "UPDATE {0}..{1} " +
		                                      " SET state={2} " +
		                                      " WHERE jobId={3}",
	                                 CoreConstants.RepositoryDatabase,
		                              CoreConstants.RepositoryJobsTable,
		                              (int)state,
		                              jobId );
		      using ( SqlCommand cmd = new SqlCommand( sqlText, repository.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
		         cmd.ExecuteNonQuery();
		      }
            LogJobTableProcessEnd(logFunction, logIndex, true);
		   }
		   catch (Exception ex)
		   {
		      ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
		                               CoreConstants.Exception_CantAccessJobsTable,
		                               ex,
		                               ErrorLog.Severity.Error );
            LogJobTableProcessEnd(logFunction, logIndex, false);
            throw;
		   }
		   finally
		   {
		      repository.CloseConnection();
		   }
      }

      //-----------------------------------------------------------------------
      // UpdateRowsProcessed - append the number of final event rows that have already been
      //  inserted into the audit trail
      //-----------------------------------------------------------------------
      public void UpdateRowsProcessed(int rowsProcessed, SqlConnection conn, SqlTransaction trans)
      {
         lastFinalEventProcessed = rowsProcessed;

         string logFunction = "UpdateRowsProcessed";
         int logIndex = -1;

         try
         {
            logFunction = String.Format("UpdateRowsProcessed job {0} file {1} last row {2}", jobId, compressedTraceFile, lastFinalEventProcessed);
            logIndex = LogJobTableProcessBegin(logFunction);

            string sqlText = String.Format("UPDATE {0}..{1} " +
               " SET compressedTraceFile={2} " +
               " WHERE jobId={3}",
               CoreConstants.RepositoryDatabase,
               CoreConstants.RepositoryJobsTable,
               SQLHelpers.CreateSafeString(String.Format("{0}*{1}", compressedTraceFile, lastFinalEventProcessed)),
               jobId);
            using (SqlCommand cmd = new SqlCommand(sqlText, conn))
            {
               cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
               if (trans != null)
                  cmd.Transaction = trans;

               cmd.ExecuteNonQuery();
            }
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
               CoreConstants.Exception_CantAccessJobsTable,
               ex,
               ErrorLog.Severity.Error);
            LogJobTableProcessEnd(logFunction, logIndex, false);
            throw ex;
         }
      }
      
      //-----------------------------------------------------------------------
      // Update - Writes writeable record values to jobs table
      //-----------------------------------------------------------------------
      public void
         Update()
      {
		   Repository repository = new Repository();

         string logFunction = "Update";
         int logIndex = -1;
		   
         try
         {
            logFunction = String.Format("Update job {0} state {1} tempTablePrefix {2}", jobId, state, tempTablePrefix);
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();
            
		      string sqlText = String.Format( "UPDATE {0}..{1} SET" +
		                                      " state={2},tempTablePrefix={3} " +
		                                      " WHERE jobId={4}",
	                                 CoreConstants.RepositoryDatabase,
		                              CoreConstants.RepositoryJobsTable,
		                              (int)state,
		                              SQLHelpers.CreateSafeString(tempTablePrefix),
		                              jobId );
		      using ( SqlCommand cmd = new SqlCommand( sqlText, repository.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
		         cmd.ExecuteNonQuery();
		      }
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
		   catch (Exception ex)
		   {
		      ErrorLog.Instance.Write( "TraceJobInfo::Update",
		                               CoreConstants.Exception_CantAccessJobsTable,
		                               ex,
		                               ErrorLog.Severity.Error );
            LogJobTableProcessEnd(logFunction, logIndex, false);
            throw ex;
		   }
		   finally
		   {
		      repository.CloseConnection();
		   }
      }
      
      //-----------------------------------------------------------------------
      // Delete - Delete jobInfo record
      //-----------------------------------------------------------------------
      public void
         Delete()
      {
		   Repository repository = new Repository();

         string logFunction = "Delete";
         int logIndex = -1;

         try
         {
            logFunction = String.Format("Delete job {0}", jobId);
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();

		      string sqlText = String.Format( "DELETE FROM {0} where jobId={1}",
		                                      CoreConstants.RepositoryJobsTable,
		                                      jobId );
		                                      
		      using ( SqlCommand cmd = new SqlCommand( sqlText, repository.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
		         cmd.ExecuteNonQuery();
		      }
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
		   catch (Exception ex)
		   {
		      ErrorLog.Instance.Write( "TraceJobInfo::Delete",
		                               CoreConstants.Debug_CantDeleteJob,
		                               ex,
		                               ErrorLog.Severity.Warning );
            LogJobTableProcessEnd(logFunction, logIndex, false);
            throw ex;
		   }
		   finally
		   {
		      repository.CloseConnection();
		   }
      }
      
      //-----------------------------------------------------------------------
      // Insert - Creates new job record - used by trace file receiving handler
      //-----------------------------------------------------------------------
      public void
         Insert()
      {
		   Repository               repository = new Repository();

         string logFunction = "Insert";
         int logIndex = -1;

         try
         {
            logFunction = String.Format("Insert job for {0} file {1}", instance, compressedTraceFile);
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();
            
            string sqlTmp = "INSERT INTO {0} "+
                              "(" +
			                        "instance" +
    		                        ",state" +
    		                        ",dateReceived" +
			                        ",compressedTraceFile" +
    		                        ",traceChecksum" +
			                        ",compressedAuditFile" +
    		                        ",auditChecksum" +
                                 ",privilegedUserTrace" +
                                 ",sqlSecureTrace" +
			                     ") VALUES ({1},{2},{3},{4},{5},{6},{7},{8},{9});";
			                     
			   string cmdStr = String.Format( sqlTmp,
			                                  CoreConstants.RepositoryJobsTable,
			                                  SQLHelpers.CreateSafeString(instance),
			                                  (int)State.New,
			                                  SQLHelpers.CreateSafeDateTime(DateTime.UtcNow ),
			                                  SQLHelpers.CreateSafeString(compressedTraceFile),
			                                  traceChecksum,
			                                  SQLHelpers.CreateSafeString(compressedAuditFile),
			                                  auditChecksum,
                                           privilegedUserTrace ? 1 : 0,
                                           sqlSecureTrace      ? 1 : 0 );
		                     
		      using ( SqlCommand cmd = new SqlCommand( cmdStr, repository.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
		         cmd.ExecuteNonQuery();
		      }
		      
		      // Get ID of last record inserted and that is us!
		      cmdStr = String.Format( "SELECT MAX(IDENTITYCOL) FROM {0}..{1}",
	                                 CoreConstants.RepositoryDatabase,
		                              CoreConstants.RepositoryJobsTable );
		      using ( SqlCommand cmd = new SqlCommand( cmdStr, repository.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
		         using ( SqlDataReader reader = cmd.ExecuteReader() )
		         {
		            if ( reader.Read())
		            {
		               jobId = (int)reader.GetSqlInt32(0);
		               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
		                                       String.Format( "A new job created. Job ID: {0}", jobId ) );
		            }
		         }
		      }
            logFunction = String.Format("{0} job {1}", logFunction, jobId);
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
		   catch (Exception ex )
		   {
		      ErrorLog.Instance.Write( "TraceJobInfo::Insert",
		                               CoreConstants.Exception_CantAccessJobsTable,
		                               ex,
		                               ErrorLog.Severity.Error );
            LogJobTableProcessEnd(logFunction, logIndex, false);
            throw ex;
		   }
		   finally
		   {
		      repository.CloseConnection();
		   }
      }
      
      //---------------------------------------------------------------------------
      // ResetCounts -reset the counts on how many rows were affected by trace job
      //---------------------------------------------------------------------------
      public void
         ResetCounts()
      {
         eventsTotal      = 0;
         eventsDeleted    = 0;
         eventsInserted   = 0;
         eventsUpdated    = 0;
         eventsDuplicated = 0;
      }
      
      //-----------------------------------------------------------------------
      // GetAborting - gets flag on whether this job has been aborted
      //-----------------------------------------------------------------------
      public bool
         GetAborting()
      {
		   if ( this.aborting ) return true;

		   Repository repository = new Repository();

         string sqlText = "";

         string logFunction = "GetAborting";
         int logIndex = -1;

         try
         {
            logFunction = String.Format("GetAborting job {0}", jobId);
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();

		      sqlText = String.Format( "SELECT state,aborting FROM {0} where jobId={1}",
		                               CoreConstants.RepositoryJobsTable,
		                               this.jobId );
		      using ( SqlCommand cmd = new SqlCommand( sqlText, repository.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
		         using ( SqlDataReader reader = cmd.ExecuteReader() )
		         {
		            if ( reader.Read() )
		            {
		               int  st;
		               st = SQLHelpers.GetInt32(   reader, 0 );
		               bool ab;
		               ab = SQLHelpers.ByteToBool( reader, 1 );
		               if ( (st==(int)State.Aborting) || (ab) )
		               {
		                  this.aborting = true;
		               }
		            }
		            else
		            {
		               // no matching record
		               this.aborting = true;
		            }
		         }
		      }
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
		   catch (Exception ex)
		   {
		      ErrorLog.Instance.Write( String.Format( "Aborting jobid: {0}", this.jobId ), sqlText, ex );
            LogJobTableProcessEnd(logFunction, logIndex, false);
		      
		      // couldnt read jobid - must be missing ( or SQL went down) so we should abort.
            this.aborting = true;
		   }
		   finally
		   {
		      repository.CloseConnection();
		   }
		   
		   return this.aborting;
      }
      
      //-----------------------------------------------------------------------
      // SetAborting - sets job flag to aborting so jobs will shut down
      //-----------------------------------------------------------------------
       public void
         SetAborting()
      {
         SetAborting(true);
      }
      
       public void
         SetAborting( bool flag)
      {
		   Repository    repository = new Repository();
		   
		   this.aborting = flag;

         string logFunction = "SetAborting";
         int logIndex = -1;

         try
         {
            logFunction = String.Format("SetAborting job {0}", jobId);
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();

		      string sqlText = String.Format( "UPDATE {0} SET state=6,aborting=1 WHERE jobId={1}",
		                                      CoreConstants.RepositoryJobsTable,
		                                      this.jobId );
		      using( SqlCommand cmd = new SqlCommand( sqlText, repository.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
		         cmd.ExecuteNonQuery();
		      }
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
		   catch (Exception ex)
		   {
		      ErrorLog.Instance.Write( "TraceJobInfo::SetAborting",
		                               ex,
		                               ErrorLog.Severity.Warning );
            LogJobTableProcessEnd(logFunction, logIndex, false);
         }
		   finally
		   {
		      repository.CloseConnection();
		   }
      }
      
      
      public static void
         SetAbortingAll()
      {
         Repository    repository = new Repository();

         string logFunction = "SetAbortingAll";
         int logIndex = -1;

         try
         {
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();

            string sqlText = String.Format( "UPDATE {0} SET aborting=1 WHERE state in (0,1,2)",
               CoreConstants.RepositoryJobsTable );
            using( SqlCommand cmd = new SqlCommand( sqlText, repository.connection ) )
            {
               cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
               cmd.ExecuteNonQuery();
            }
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write( "TraceJobInfo::SetAborting",
               ex,
               ErrorLog.Severity.Warning );
            LogJobTableProcessEnd(logFunction, logIndex, false);
         }
         finally
         {
            repository.CloseConnection();
         }
      }
      

      //-----------------------------------------------------------------------
      // KillFinishedJobs
      //-----------------------------------------------------------------------
      static public void
         KillFinishedJobs(
            string            instance
         )
      {
		   Repository    repository = new Repository();

         string logFunction = "KillFinishedJobs";
         int logIndex = -1;

         try
         {
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();

		      string sqlText = String.Format( "DELETE FROM {0} WHERE (instance={1}) AND (state=4 or state=5)",
		                                      CoreConstants.RepositoryJobsTable,
		                                      SQLHelpers.CreateSafeString(instance) );
		      using ( SqlCommand cmd = new SqlCommand( sqlText, repository.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
		         cmd.ExecuteNonQuery();
		      }
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
		   catch (Exception ex)
		   {
		      ErrorLog.Instance.Write( "TraceJobInfo::KillFinishedJobs",
		                               ex,
		                               ErrorLog.Severity.Warning );
            LogJobTableProcessEnd(logFunction, logIndex, false);
         }
		   finally
		   {
		      repository.CloseConnection();
		   }
      }
      
      //-----------------------------------------------------------------------
      // SetAbortingForInstanceJobs - sets job flag to aborting so jobs will shut down
      //                              turns on flag for all jobs related to an instance
      //-----------------------------------------------------------------------
      static public void
         SetAbortingForInstanceJobs(
            string            instance
         )
      {
		   Repository    repository = new Repository();

         string logFunction = "SetAbortingForInstanceJobs";
         int logIndex = -1;

         try
         {
            logFunction = string.Format("SetAbortingForInstanceJobs for {0}", instance);
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();

		      string sqlText = String.Format( "UPDATE {0} SET state=6,aborting=1 WHERE instance={1}",
		                                      CoreConstants.RepositoryJobsTable,
		                                      SQLHelpers.CreateSafeString(instance) );
		      using ( SqlCommand cmd = new SqlCommand( sqlText, repository.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
		         cmd.ExecuteNonQuery();
		      }
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
		   catch (Exception ex)
		   {
		      ErrorLog.Instance.Write( "TraceJobInfo::SetAbortingForInstanceJobs",
		                               ex,
		                               ErrorLog.Severity.Warning );
            LogJobTableProcessEnd(logFunction, logIndex, false);
         }
		   finally
		   {
		      repository.CloseConnection();
		   }
      }
      
      //-----------------------------------------------------------------------
      // DeleteUncompressedFiles
      //-----------------------------------------------------------------------
      public void
         DeleteUncompressedFiles()
      {
         if ( this.traceFile != "" )
         {
            try { File.Delete( this.traceFile ); }
            catch( Exception ex1)
            {
               ErrorLog.Instance.Write( "DeleteFile",
                                        this.traceFile,
                                        ex1,
                                        ErrorLog.Severity.Warning );
            }
         }
         if ( this.auditFile != "" )
         {
            try { File.Delete( this.auditFile ); }
            catch( Exception ex2)
            {
               ErrorLog.Instance.Write( "DeleteFile",
                                        this.auditFile,
                                        ex2,
                                        ErrorLog.Severity.Warning );
            }
         }
         
         if ( this.traceFile != "" )
         {
            string bcp = Path.ChangeExtension(this.traceFile,".bcp");
            try { File.Delete( bcp ); }
            catch {}
         }
      }
      
      //-----------------------------------------------------------------------
      // DeleteCompressedFiles
      //-----------------------------------------------------------------------
      public void
         DeleteCompressedFiles()
      {
         if ( this.compressedTraceFile != "" )
         {
            try { File.Delete( this.compressedTraceFile ); }
            catch( Exception ex)
            {
		         ErrorLog.Instance.Write( "Delete",
		                                  this.compressedTraceFile,
		                                  ex,
		                                  ErrorLog.Severity.Warning );
            }
         }
         if ( this.compressedAuditFile != "" )
         {
            try { File.Delete( this.compressedAuditFile ); }
            catch( Exception ex)
            {
		         ErrorLog.Instance.Write( "Delete",
		                                  this.compressedAuditFile,
		                                  ex,
		                                  ErrorLog.Severity.Warning );
            }
         }
      }
      
      //-----------------------------------------------------------------------
      // GetInstanceJobCount
      //-----------------------------------------------------------------------
      static public int
         GetInstanceJobCount(
            string            instanceName
         )
      {
         int count;
         
		   Repository repository = new Repository();

         string logFunction = "GetInstanceJobCount";
         int logIndex = -1;

         try
         {
            logFunction = string.Format("GetInstanceJobCount for {0}", instanceName);
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();

		      string sqlText = String.Format( "SELECT count(jobId) FROM {0} where instance={1}",
		                                      CoreConstants.RepositoryJobsTable,
		                                      SQLHelpers.CreateSafeString(instanceName) );
		      using ( SqlCommand cmd = new SqlCommand( sqlText, repository.connection ) )
		      {
			      cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
			      
				   object obj = cmd.ExecuteScalar();
               if( obj is DBNull )
                  count = 0;
               else
                  count = (int) obj;
		      }
            logFunction = string.Format("{0} count {1}", logFunction, count);
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
		   catch (Exception)
		   {
		      count = 0;
            LogJobTableProcessEnd(logFunction, logIndex, false);
         }
		   finally
		   {
		      repository.CloseConnection();
		   }
		   
		   return count;
      }
      
      #endregion
      
      #region Internal Methods
      
      internal void UpdateIdCache( EventRecord er )
      {
         try
         {
            if ( er.applicationName != "" && !appIdTable.ContainsKey( er.applicationName ) )
               appIdTable.Add( er.applicationName, er.appNameId );
         }
         catch
         {}

         try
         {
            if ( er.hostName != "" && !hostIdTable.ContainsKey( er.hostName ))
               hostIdTable.Add( er.hostName, er.hostId );
         }
         catch
         {}

         try
         {
            if ( er.loginName != "" && !loginIdTable.ContainsKey( er.loginName ))
               loginIdTable.Add( er.loginName, er.loginId );
         }
         catch
         {}
      }
      
      internal void UpdateTableIdCache( string schema, string table, int id )
      {
         try
         {
            string key = CoreHelpers.GetTableNameKey( schema, table );
            if (key != "" && !tableIdTable.ContainsKey(key))
               tableIdTable.Add( key, id );
         }
         catch
         {}
      }
      
	   internal void UpdateColumnIdCache( string column, int id )
      {
         try
         {
            if ( column != "" && !columnIdTable.ContainsKey( column ) )
               columnIdTable.Add( column, id );
         }
         catch
         {}
      }

	   internal  void UpdateIdTables( SqlConnection conn )
      {
         foreach( string appName in appIdTable.Keys )
         {
            EventDatabase.UpdateIdTable( conn,
                           conn.Database,
                           CoreConstants.RepositoryApplicationsTable,
                           appName,
                           appIdTable[appName] );
         }
         
         foreach( string hostName in hostIdTable.Keys )
         {
            EventDatabase.UpdateIdTable(conn,
                           conn.Database,
                           CoreConstants.RepositoryHostsTable,
                           hostName,
                           hostIdTable[hostName]);
         }

         foreach (string loginName in loginIdTable.Keys)
         {
            EventDatabase.UpdateIdTable(conn,
                           conn.Database,
                           CoreConstants.RepositoryLoginsTable,
                           loginName,
                           loginIdTable[loginName]);
         }
      }
      
      internal void UpdateTableAndColumnIdTables( SqlConnection conn )
      {
         foreach ( string key in tableIdTable.Keys )
         {
            string schema = "";
            string table = "";
            CoreHelpers.GetTableNameFromKey( key, out schema, out table );
            EventDatabase.UpdateIdTableEx( conn,
                                         conn.Database,
                                         CoreConstants.RepositoryTablesTable,
                                         schema,
                                         table,
                                         tableIdTable[key] );
         }
         foreach ( string columnName in columnIdTable.Keys )
         {
            EventDatabase.UpdateIdTable( conn,
                                         conn.Database,
                                         CoreConstants.RepositoryColumnsTable,
                                         columnName,
                                         columnIdTable[columnName] );
         }
      }

	   internal static void 
         ResetJobQueue()
      {
         lock ( queueLock )
         {
            jobQueue.Clear();
         }
      }
      
      // Returns true if the agent is able to support BeforeAfter data collection (3.1 and beyond)
      internal bool SupportsBeforeAfter()
      {
         if (agentVersion.StartsWith("1") ||
            agentVersion.StartsWith("2") ||
            agentVersion.StartsWith("3.0"))
            return false;
         else
            return true;
      }

      internal static int LogJobTableProcessBegin(string process)
      {
         if (CollectionServer.jobsLogLevel == 1)
         {
            lock (logLock)
            {
               try
               {
                  logIndex = logIndex == int.MaxValue ? 1 : logIndex + 1;
                  logTimers.Add(logIndex, new System.Diagnostics.Stopwatch());
                  ErrorLog.Instance.Write(ErrorLog.Level.Always,
                                           string.Format(CoreConstants.Info_BeginJobProcess,
                                                            process,
                                                            logIndex,
                                                            logTimers.Count),
                                           ErrorLog.Severity.Informational);
                  logTimers[logIndex].Start();
               }
               catch { }
            }

            return logIndex;
         }
         else
         {
            return 0;
         }
      }

      internal static void LogJobTableProcessEnd(string process, int timerIndex, bool success)
      {
         // Index will be 0 if logging was off when process started, but finish processing any valid ones to remove timers and log the end of processes
         if (timerIndex > 0)
         {
            lock (logLock)
            {
               try
               {
                  logTimers[timerIndex].Stop();
                  ErrorLog.Instance.Write(ErrorLog.Level.Always,
                                           string.Format(CoreConstants.Info_EndJobProcess,
                                                            process,
                                                            timerIndex,
                                                            logTimers.Count,
                                                            success ? "succeeded" : "failed",
                                                            logTimers[timerIndex].ElapsedMilliseconds),
                                           success ? ErrorLog.Severity.Informational : ErrorLog.Severity.Warning);
                  logTimers.Remove(timerIndex);
               }
               catch { }
            }
         }
      }


      // Returns true if the agent is able to support Row Count information (5.5 and above)
      internal bool SupportsRowCount()
      {
          if (!string.IsNullOrEmpty(agentVersion))
          {
              string majorVersion = agentVersion.Substring(0, 3);
              if (float.Parse(majorVersion) >= 5.5f)
                  return true;
              else
                  return false;
          }
          return false;
      }
	   #endregion
	   
	   #region Private methods

      //
      // GetJobs(): retrieves all the jobs in the jobs table and queue them in the jobsQueue.
      //            The jobs are enqueued in round robin by instance names.
      //
      private static void
         GetJobs()
      {
         Repository repository = new Repository();
         Dictionary<string, List<TraceJobInfo>> jobs =
            new Dictionary<string, List<TraceJobInfo>>();

         string logFunction = "GetJobs";
         int logIndex = -1;

         try
         {
            logIndex = LogJobTableProcessBegin(logFunction);

            repository.OpenConnection();

            string sqlText = String.Format("SELECT jobId,j.instance,dateReceived," +
                                            "compressedTraceFile,traceChecksum," +
                                            "compressedAuditFile,auditChecksum," +
                                            "privilegedUserTrace,sqlSecureTrace, s.agentVersion " +
                                            "  FROM {0}..{1} j, {0}..{2} s WHERE (state=0 or state=3) and (aborting=0) " +
                                            " and j.instance = s.instance " +
                                            " ORDER BY dateReceived {3}",
                                            CoreConstants.RepositoryDatabase,
                                            CoreConstants.RepositoryJobsTable,
                                            CoreConstants.RepositoryServerTable,
                                            newJobsFirst ? "DESC" : "ASC");
            List<TraceJobInfo> jobList;

            using (SqlCommand cmd = new SqlCommand(sqlText, repository.connection))
            {
               cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

               using (SqlDataReader reader = cmd.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     TraceJobInfo jobInfo = new TraceJobInfo();

                     jobInfo.jobId = SQLHelpers.GetInt32(reader, 0);
                     jobInfo.instance = SQLHelpers.GetString(reader, 1);
                     jobInfo.dateReceived = SQLHelpers.GetDateTime(reader, 2);
                     jobInfo.compressedTraceFile = SQLHelpers.GetString(reader, 3);
                     //
                     // we overloaded this field to contain the file : finalEventsProcessedCount
                     //  This allows us to recover from processing failures in the middle of
                     //  a trace file without generating duplicate events in the audit trail.
                     int i = jobInfo.compressedTraceFile.IndexOf('*');
                     if (i != -1)
                     {
                        string s = jobInfo.compressedTraceFile;
                        jobInfo.compressedTraceFile = s.Substring(0, i);
                        try
                        {
                           jobInfo.lastFinalEventProcessed = Int32.Parse(s.Substring(i + 1));
                        }
                        catch
                        {
                           // any parsing exceptions mean we process the entire file again
                           jobInfo.lastFinalEventProcessed = 0;
                        }
                     }
                     jobInfo.traceChecksum = SQLHelpers.GetInt32(reader, 4);
                     jobInfo.compressedAuditFile = SQLHelpers.GetString(reader, 5);
                     jobInfo.auditChecksum = SQLHelpers.GetInt32(reader, 6);
                     jobInfo.privilegedUserTrace = SQLHelpers.ByteToBool(reader, 7);
                     jobInfo.sqlSecureTrace = SQLHelpers.ByteToBool(reader, 8);
                     jobInfo.agentVersion = SQLHelpers.GetString( reader, 9 );
                     jobInfo.state = State.New;
                     jobInfo.iStats = Stats.Stats.Instance.GetStats(jobInfo.instance);
                     jobInfo.newStats = jobInfo.iStats.GetNewInstanceStats();

                     ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug,
                                              String.Format(
                                                 "TraceJobInfo:GetNextJob\n\nInstance {0}\nTracefile {1}",
                                                 jobInfo.instance,
                                                 jobInfo.compressedTraceFile));
                     // set trace level               
                     FileInfo fi = new FileInfo(jobInfo.compressedTraceFile);
                     TraceFileNameAttributes tc =
                        TraceFileNameAttributes.GetNameAttributes(fi);
                     jobInfo.traceType = tc.TraceType;
                     jobInfo.traceSequence = tc.Sequence;
                     jobInfo.traceLevel = tc.AuditLevel;
                     jobInfo.traceCategory = tc.AuditCategory;

                     if (!jobs.ContainsKey(jobInfo.instance))
                     {
                        jobList = new List<TraceJobInfo>();
                        jobs.Add(jobInfo.instance, jobList);
                     }
                     else
                     {
                        jobList = jobs[jobInfo.instance];
                     }

                     jobList.Add(jobInfo);
                  }
               }
            }

            while (jobs.Count > 0)
            {
               List<string> removeList = new List<string>();
               foreach (string instance in jobs.Keys)
               {
                  jobList = jobs[instance];
                  logFunction = string.Format("{0}, {1} ({2} jobs)", logFunction, instance, jobList.Count);

                  // Only lock on write (reader/writer).
                  // Note that there suppose to be just one thread from the TraceJobPool,
                  // so this lock just a safety net.
                  lock (queueLock)
                  {
                     jobQueue.Enqueue(jobList[0]);
                  }
                  jobList.RemoveAt(0);
                  if (jobList.Count == 0)
                  {
                     removeList.Add(instance);
                  }
               }

               foreach (string instance in removeList)
               {
                  jobs.Remove(instance);
               }
            }
            LogJobTableProcessEnd(logFunction, logIndex, true);
         }
         catch (Exception ex)
         {
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                     CoreConstants.Exception_CantAccessJobsTable,
                                     ex,
                                     ErrorLog.Severity.Error);
            LogJobTableProcessEnd(logFunction, logIndex, false);
            throw;
         }
         finally
         {
            repository.CloseConnection();
         }
      }

      #endregion
   }
}
