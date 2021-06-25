using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using System.Collections;
using System.Text;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Agent;
using TraceFilter = Idera.SQLcompliance.Core.Event.TraceFilter;

namespace Idera.SQLcompliance.Core.TraceProcessing
{
	/// <summary>
	/// Summary description for TraceJobPoolWorkItem.
	/// </summary>
	internal class TraceJobPoolWorkItem : BlackHen.Threading.WorkItem
	{
	   #region Properties
	   
	   TraceJobInfo jobInfo = null;
	   
		private static Object traceJobLock = new Object();
		
      static Hashtable instanceLocks = new Hashtable();
	   
	   #endregion
	   
	   #region Constructor
	   
		public TraceJobPoolWorkItem(
		   TraceJobInfo   inJobInfo
		)
		{
		   jobInfo = inJobInfo;
		}
		
		#endregion
		
	   #region Worker routines
	
		//---------------------------------------------------------------------------
		// Perform - BlackHen Threading Library Interface WorkItem entrypoint
		//---------------------------------------------------------------------------
      public override void Perform()
		{
           // ErrorLog.Instance.ErrorLevel = ErrorLog.Level.UltraDebug;
		   bool aborting = false;
		   TraceJobInfo.State newTraceJobState = TraceJobInfo.State.Running;
           string fileName = "";
		   
         // check if job aborted
		   if ( jobInfo.GetAborting() )
		   {
		      aborting = true;
		      AbortJob(jobInfo);
		      return;
		   }
		   
		   jobInfo.ResetCounts();
   		
		   try
		   {
		      ErrorLog.Instance.Write( ErrorLog.Level.Debug,
		                                 String.Format( "Starting TraceJob: Instance '{0} Trace '{1}' Audit '{2}", jobInfo.instance, jobInfo.compressedTraceFile, jobInfo.compressedAuditFile )) ;
		                                 
            // Get unique temp file name
            string tmp = Path.GetFileNameWithoutExtension(jobInfo.compressedTraceFile);
            jobInfo.tempTablePrefix = NativeMethods.GetHashCode(tmp).ToString();
            
            //  Signal Job Started
            jobInfo.state = TraceJobInfo.State.Running;
            jobInfo.Update();
         }
         catch (Exception ex)
         {
            jobInfo.UpdateState( TraceJobInfo.State.Failed );

            ErrorLog.Instance.Write( "TraceJobWorkItem:Phase 1",
                                       String.Format( CoreConstants.Exception_ErrorProcessingTraceFile,
                                                      jobInfo.compressedTraceFile ),
                                       ex,
                                       true );
            throw ex;
         }
         
         try
         {
             fileName = Path.GetFileName(jobInfo.compressedTraceFile);

            // Calculate uncompressed trace file name and temp table prefix
             if (fileName.StartsWith("XE"))
             {
                 jobInfo.traceFile = Path.ChangeExtension(jobInfo.compressedTraceFile,
                                                           ".xel");
             }
             else if (fileName.StartsWith("AL"))
             {
                 jobInfo.traceFile = Path.ChangeExtension(jobInfo.compressedTraceFile,
                                                           ".sqlaudit");
             }
             else
             {
                 jobInfo.traceFile = Path.ChangeExtension(jobInfo.compressedTraceFile,
                                                           ".trc");
             }
            jobInfo.auditFile = Path.ChangeExtension( jobInfo.compressedAuditFile,
                                                      ".bin");
            ValidateJobInput();
            
            // validate trace file checksum - we do this here because it is a different error case
            // then during prodessing where we retry later - here we alert and give up! 
            ValidateJobFiles( jobInfo );
         }
         catch (Exception ex )
         {
            ErrorLog.Instance.Write( "TraceJobWorkItem:Phase 2",
                                       String.Format( CoreConstants.Exception_Format_UnrecoverableProcessingError,
                                                      jobInfo.compressedTraceFile),
                                    ex,
                                    true );

            // any errors in processing files are unrecoverable - the only one we might want a special check for would be
            // out of disk space
            jobInfo.UpdateState( TraceJobInfo.State.UnrecoverableError );
            
            throw ex;
         }
         
		   try
		   {
            try
            {
               // Uncompress trace file
               int start = Environment.TickCount;
               
               TraceFile.UncompressToDisk( jobInfo.compressedTraceFile,
                                          jobInfo.traceFile);
               
               // Uncompress audit settings file
               TraceFile.UncompressToDisk( jobInfo.compressedAuditFile,
                                          jobInfo.auditFile);
                                           
               jobInfo.uncompressTime = Environment.TickCount - start;                                          
            }
            catch (FileNotFoundException fex )
            {
               ErrorLog.Instance.Write( "TraceJob::UncompressFiles",
                                        String.Format( CoreConstants.Exception_UnrecoverableProcessingError,
                                                       jobInfo.compressedTraceFile),
                                       fex,
                                       true );

               newTraceJobState = TraceJobInfo.State.UnrecoverableError;
               throw fex;
            }
            catch (Exception ex )
            {
               ErrorLog.Instance.Write( "TraceJob::UncompressFiles",
                                          String.Format( CoreConstants.Exception_ProcessingError,
                                                         jobInfo.compressedTraceFile),
                                       ex,
                                       true );

               if ( ex.Message.IndexOf( "altered or deleted" ) != -1 )
               {
                  newTraceJobState = TraceJobInfo.State.UnrecoverableError;
               }
               else
               {
                  newTraceJobState = TraceJobInfo.State.Failed;
               }
               
               throw ex;
            }
         
            // read dynamic parts of server config that could have changed recently
            // If this call fails, we have registered jobs for an instance that is no longer
            //  auidted.  Dump the data.  
            bool audited = GetServerConfiguration();

            // Load audit configuration from XML file
            if(audited)
            {
                if (fileName.StartsWith("XE"))
                    audited = GetTraceConfigurationXE();
                else
                {
                    audited = GetTraceConfiguration();
                }
            }


            // Skip trace processing if the trace is not part of the audit.
            if (audited)
            {
                // check if job aborted
                if (jobInfo.GetAborting())
                {
                    aborting = true;
                    AbortJob(jobInfo);
                    return;
                }

                //------------------------------------------------------------
                // Start job - need lock around this because we rely on
                //             processing event files in chronological order
                //
                // by putting lock at this level - we can go ahead and 
                // uncompress waiting trace files
                //
                // Ajeet - Moving the lock to repository write method to allow trace processing in parallel.
                //------------------------------------------------------------
                //lock (AcquireInstanceLock(jobInfo.instance))
                {
                    //process Audit Log file
                    if (fileName.StartsWith("AL"))
                    {
                        AuditLogJob processingJob = new AuditLogJob(jobInfo);
                        processingJob.Start();
                    }
                    else
                    {
                        TraceJob processingJob = new TraceJob(jobInfo);
                        if (fileName.StartsWith("XE"))
                            processingJob.StartXE();
                        else
                            processingJob.Start();
                    }
                }

                // check if job aborted
                if (jobInfo.GetAborting())
                {
                    aborting = true;
                    AbortJob(jobInfo);
                    return;
                }
            }

            // ran to completion - clean up            
            newTraceJobState = TraceJobInfo.State.Done;
            jobInfo.UpdateState( TraceJobInfo.State.Done );
            jobInfo.Delete();
            jobInfo.DeleteCompressedFiles();
		   }
         catch (SqlException sqEx)
         {
            // check if job aborted
		      if ( jobInfo.GetAborting() )
		      {
   		      aborting = true;
		         AbortJob(jobInfo);
		         return;
		      }
         
		      // log error
            ErrorLog.Instance.Write( "TraceJobWorkItem:Phase 3",
                                      String.Format( CoreConstants.Exception_ErrorProcessingTraceFile,
                                                     jobInfo.compressedTraceFile ),
                                      sqEx,
                                      true );
            if ( sqEx.Class <14 || sqEx.Class > 17 )
            {
               newTraceJobState = TraceJobInfo.State.Failed;
            }
            else
            {
               newTraceJobState = TraceJobInfo.State.UnrecoverableError;
            }
         }
         catch (Exception ex)
         {
            // check if job aborted
		      if ( jobInfo.GetAborting() )
		      {
   		      aborting = true;
		         AbortJob(jobInfo);
		         return;
		      }
   		   
		      // log error
            if ( newTraceJobState == TraceJobInfo.State.Running ) // if not set in some throw above
            {
               newTraceJobState = TraceJobInfo.State.Failed;
            }

            ErrorLog.Instance.Write( "TraceJobWorkItem:Phase 3.2",
                                       String.Format( CoreConstants.Exception_ErrorProcessingTraceFile,
                                                      jobInfo.compressedTraceFile ),
                                       ex,
                                       true );
         }
         finally
         {
            if ( ! aborting )
            {
               bool deleteTraceFiles = true;
               
               int traceLevel = jobInfo.traceType % 10;
               
               // Check if keep DML traces
               if (     traceLevel == 2 && (jobInfo.traceCategory ==  (int) TraceCategory.DML ||
                           jobInfo.traceCategory == (int)TraceCategory.DMLwithDetails ||
                           jobInfo.traceCategory == (int)TraceCategory.DMLwithSELECT ) ) 
               {
                  if (CoreConstants.DontDeleteDMLTraces) deleteTraceFiles = false;
               }
               else // Keep none-DML traces?
               {
                  if (CoreConstants.DontDeleteNonDMLTraces) deleteTraceFiles = false;
               }

               if (deleteTraceFiles && !(jobInfo.traceFile.EndsWith(".xel") && newTraceJobState != TraceJobInfo.State.Done))
               {
                  jobInfo.DeleteUncompressedFiles();
               }
               
               if ( newTraceJobState != TraceJobInfo.State.Done )
               {
                  jobInfo.UpdateState( newTraceJobState );
               }
            }
           // ErrorLog.Instance.ErrorLevel = ErrorLog.Level.Default; 
         }
		}

      //-----------------------------------------------------------------------
      // AcquireInstanceLock
      //-----------------------------------------------------------------------
	   private static Object acquireInstanceLock = new Object();

      static Object
         AcquireInstanceLock(
            string      instance
         )
      {
         string upperInstance = instance.ToUpper();
         
         lock (acquireInstanceLock )
         {
            if ( instanceLocks.Contains( upperInstance ) )
            {
               return instanceLocks[upperInstance];
            }
            else
            {
               Object syncObj = new Object();
               try
               {
                  instanceLocks.Add( upperInstance, syncObj );
               }
               catch ( ArgumentException )
               {
                  // ignore duplicate key error
                  syncObj = instanceLocks[upperInstance];
               }
               
               return syncObj;
            }
         }
      }
		
      //-----------------------------------------------------------------------
      // AbortJob - Kill job and clean up residue
      //-----------------------------------------------------------------------
		public void
		   AbortJob(
		      TraceJobInfo   jobInfo
		   )
		{
         //jobInfo.Delete();
         //jobInfo.DeleteCompressedFiles();
         jobInfo.DeleteUncompressedFiles();
		}
		
      //-----------------------------------------------------------------------
      // ValidateJobFiles - compare checksum in job record against
      //                    actual file - both trace file and xml file
      //-----------------------------------------------------------------------
      static private void
         ValidateJobFiles(
            TraceJobInfo jobInfo
         )
      {
         int traceChecksum = -1;
         int auditChecksum = -1;

         try
         {
            traceChecksum = TraceFile.CalculateChecksum( jobInfo.compressedTraceFile );
            auditChecksum = TraceFile.CalculateChecksum( jobInfo.compressedAuditFile );
         }
         catch( Exception ex )
         {
            ErrorLog.Instance.Write( String.Format( CoreConstants.Alert_CorruptedTraceFile,
                                                    jobInfo.compressedTraceFile,
                                                    jobInfo.instance ),
                                     ex, true );
         }
         
         if (( traceChecksum != jobInfo.traceChecksum ) ||
             ( auditChecksum != jobInfo.auditChecksum ))
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     String.Format( "Trace job: {0}\nChecksums(Table,Calc):\n\tTRZ {1} - {2}\n\tBIZ {3} - {4}",
                                                    jobInfo.compressedTraceFile,
                                                    jobInfo.traceChecksum, traceChecksum,
                                                    jobInfo.auditChecksum, auditChecksum ) );


            string msg = String.Format( CoreConstants.Alert_CantProcessTraceFile,
                                        jobInfo.compressedTraceFile,
                                        jobInfo.instance );
                                        
            InternalAlert.Raise( jobInfo.instance,
                                 msg );

            throw new Exception( msg );
         }
      }

      //-----------------------------------------------------------------------
      // ValidateJobInput - Make sure everything passed into the job is fine
      //                    and that all initial assumptions are valid
      //
      //                    Note: Throws exception on error
      //-----------------------------------------------------------------------
      private void
         ValidateJobInput()
      {
         if ( jobInfo.instance == null || jobInfo.instance == "" )
         {
            throw( new Exception("Invalid instance name passed to job"));
         }
         
         if ( jobInfo.traceFile == null || jobInfo.traceFile == "" )
         {
            throw( new Exception("Invalid trace filename passed to job"));
         }

         if ( jobInfo.tempTablePrefix == null || jobInfo.tempTablePrefix == "" )
         {
            throw( new Exception("Invalid temporary table prefix passed to job"));
         }
      }         
      
      //------------------------------------------------------------------------
      // GetServerConfiguration
      //------------------------------------------------------------------------
      private bool GetServerConfiguration()
      {
         bool retVal ;

         // Global configuration values
         Repository rep = new Repository();
         rep.OpenConnection();
         
         // server specific value
         retVal = ServerRecord.ReadTraceJobInfo( jobInfo.instance,
                                        rep.connection,
                                        out jobInfo.timeZoneInfo,
                                        out jobInfo.eventDatabase,
                                        out jobInfo.maxSql,
                                        out jobInfo.sqlVersion);
         rep.CloseConnection();
         return retVal ;
      }
      

      #endregion

       #region Audit Configuration Routines

      //---------------------------------------------------------------------------------
      // GetTraceConfiguration - Retrieve audited events and filters from the audit XML file
      //---------------------------------------------------------------------------------
      private bool
         GetTraceConfiguration()
      {
         bool audited = false;  // indicate whether the trace is valid

         try
         {
            TraceConfiguration traceConfig = ConfigurationHelper.GetTraceConfiguration( jobInfo.auditFile,
                                                                                        jobInfo.auditFile, 
                                                                                        true,
                                                                                        jobInfo.instance,
                                                                                        jobInfo.sqlVersion,
                                                                                        out jobInfo.dcTableLists,
                                                                                        out jobInfo.scTableLists,
                                                                                        out jobInfo.dbconfigs,
                                                                                        out jobInfo.userConfigs
                                                                                        );
             // this is exceptional case for Server Start Stop event in default Trace
             // not getting into usual config mess, just want to monitor server start stop
            if (jobInfo.auditFile.Contains("_1_998_") && traceConfig == null)
            {
                audited = true;
                jobInfo.keepingSql = true;
                jobInfo.keepingAdminSql = true;
                jobInfo.traceEvents.Clear();
                jobInfo.traceEvents.Add((int)TraceEventId.ServiceControl, (int)TraceEventId.ServiceControl);
            }

            else if (traceConfig != null)
            {
                jobInfo.keepingSql = traceConfig.KeepSQL;
                jobInfo.keepingAdminSql = traceConfig.KeepAdminSQL; // By Hemant
                int[] events = traceConfig.GetTraceEventsAsIntArray();
                if (events != null)
                {
                    audited = true;
                    for (int i = 0; i < events.Length; i++)
                    {
                        if (events[i] != (int)TraceEventId.SqlStmtStarting &&
                            events[i] != (int)TraceEventId.SpStarting &&
                            events[i] != (int)TraceEventId.Exception &&
                            events[i] != (int)TraceEventId.ObjectCreated &&
                            events[i] != (int)TraceEventId.ObjectDeleted &&
                            !jobInfo.traceEvents.ContainsKey(events[i]))
                            jobInfo.traceEvents.Add(events[i], events[i]);
                    }
                }

                string[] names = traceConfig.Databases;
                if (names != null &&
                    names.Length > 0)
                {
                    foreach (string name in names)
                    {
                        if (!jobInfo.databaseNames.ContainsKey(name))
                            jobInfo.databaseNames.Add(name, name);
                    }
                }

                TraceFilter[] filters = traceConfig.GetTraceFilters();
                ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug,
                                         String.Format("{0} events and {1} filters loaded from {2}.",
                                                        events == null ? 0 : events.Length,
                                                        filters == null ? 0 : filters.Length,
                                        jobInfo.auditFile));
                CreateFilterSets(filters);
                if (traceConfig.privUsers != null)
                {
                    foreach (string user in traceConfig.privUsers)
                    {
                        if (!jobInfo.privUsers.ContainsKey(user.ToUpper()))
                        {
                            jobInfo.privUsers.Add(user.ToUpper(), user.ToUpper());
                        }
                    }
                }

                if (traceConfig.privEvents != null)
                {
                    foreach (int pEvent in traceConfig.privEvents)
                    {
                        if (!jobInfo.privEvents.ContainsKey(pEvent))
                        {
                            jobInfo.privEvents.Add(pEvent, pEvent);
                        }
                    }

                    jobInfo.privSELECT = traceConfig.privSELECT;
                    jobInfo.privDML = traceConfig.privDML;
                }
            }
            
            if( !audited ) // not part of the auditing.  write a warning event.
            {
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                        String.Format( "{0} is not a valid trace file for instance {1}",
                                                      jobInfo.traceFile , jobInfo.instance ),
                                        ErrorLog.Severity.Warning );
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
               String.Format( CoreConstants.Exception_ErrorGettingTraceEventsAndFilters,
               jobInfo.auditFile ),
               e,
               true );
         }
         return audited;
      }

        //5.4 XE
      private bool
         GetTraceConfigurationXE()
      {
          bool audited = false;  // indicate whether the trace is valid

          try
          {
              XeTraceConfiguration traceConfigXE = ConfigurationHelper.GetTraceConfigurationXE(jobInfo.auditFile,
                                                                                          jobInfo.auditFile,
                                                                                          true,
                                                                                          jobInfo.instance,
                                                                                          jobInfo.sqlVersion,
                                                                                          out jobInfo.dcTableLists,
                                                                                          out jobInfo.scTableLists,
                                                                                          out jobInfo.dbconfigs,
                                                                                          out jobInfo.userConfigs);
              // this is exceptional case for Server Start Stop event in default Trace
              // not getting into usual config mess, just want to monitor server start stop
              if (jobInfo.auditFile.Contains("_1_998_") && traceConfigXE == null)
              {
                  audited = true;
                  jobInfo.keepingSql = true;
                  jobInfo.keepingAdminSql = true;
                  jobInfo.traceEvents.Clear();
                  jobInfo.traceEvents.Add((int)TraceEventId.ServiceControl, (int)TraceEventId.ServiceControl);
              }

              else if (traceConfigXE != null)
              {
                  jobInfo.keepingSql = traceConfigXE.KeepSQLXE;
                  audited = true;                  
                  int[] events = new int[4];
                  events[0] = (int)TraceEventId.SqlStmtStarting;
                  events[1] = (int)TraceEventId.SpStarting;
                  events[2] = (int)TraceEventId.SqlStmtCompleted;
                  events[3] = (int)TraceEventId.SpCompleted;
                  jobInfo.traceEvents.Add((int)TraceEventId.SqlStmtStarting, (int)TraceEventId.SqlStmtStarting);
                  jobInfo.traceEvents.Add((int)TraceEventId.SpStarting, (int)TraceEventId.SpStarting);
                  jobInfo.traceEvents.Add((int)TraceEventId.SqlStmtCompleted, (int)TraceEventId.SqlStmtCompleted);
                  jobInfo.traceEvents.Add((int)TraceEventId.SpCompleted, (int)TraceEventId.SpCompleted);

                  string[] names = traceConfigXE.Databases;
                  if (names != null &&
                      names.Length > 0)
                  {
                      foreach (string name in names)
                      {
                          if (!jobInfo.databaseNames.ContainsKey(name))
                              jobInfo.databaseNames.Add(name, name);
                      }
                  }

                  TraceFilter[] filters = traceConfigXE.GetTraceFilters();
                  ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug,
                                           String.Format("{0} events and {1} filters loaded from {2}.",
                                                          events == null ? 0 : events.Length,
                                                          filters == null ? 0 : filters.Length,
                                          jobInfo.auditFile));
                  CreateFilterSets(filters);
                  if (traceConfigXE.privUsers != null)
                  {
                      foreach (string user in traceConfigXE.privUsers)
                      {
                          if (!jobInfo.privUsers.ContainsKey(user.ToUpper()))
                          {
                              jobInfo.privUsers.Add(user.ToUpper(), user.ToUpper());
                          }
                      }
                  }

                  if (traceConfigXE.privEvents != null)
                  {
                      foreach (int pEvent in traceConfigXE.privEvents)
                      {
                          if (!jobInfo.privEvents.ContainsKey(pEvent))
                          {
                              jobInfo.privEvents.Add(pEvent, pEvent);
                          }
                      }

                      jobInfo.privSELECT = traceConfigXE.privSELECT;
                      jobInfo.privDML = traceConfigXE.privDML;
                  }
              }

              if (!audited) // not part of the auditing.  write a warning event.
              {
                  ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                           String.Format("{0} is not a valid trace file for instance {1}",
                                                         jobInfo.traceFile, jobInfo.instance),
                                           ErrorLog.Severity.Warning);
              }
          }
          catch (Exception e)
          {
              ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                 String.Format(CoreConstants.Exception_ErrorGettingTraceEventsAndFilters,
                 jobInfo.auditFile),
                 e,
                 true);
          }
          return audited;
          //   jobInfo.keepingSqlXE = true;
          //   jobInfo.traceEvents.Add((int)TraceEventId.SqlStmtStarting,(int)TraceEventId.SqlStmtStarting);
          //   return true;
      }



      //---------------------------------------------------------------------------------
      // CreateFilterSets - Group same type of filters into filter sets
      //---------------------------------------------------------------------------------
      private void
         CreateFilterSets (
            TraceFilter [] filters )
      {
         TraceFilter [] newFilters = ReplaceObjectIdFilters( filters );

         jobInfo.filterSets = TraceFilter.CreateFilterSets( newFilters );
      }

      //---------------------------------------------------------------------------------
      // ReplaceObjectIdFilters - Replace ObjectID filters with ObjectName filters
      //---------------------------------------------------------------------------------
      private TraceFilter []
         ReplaceObjectIdFilters(
            TraceFilter [] filters )
      {
         ArrayList finalFilterList = new ArrayList();
         try
         {
            int [] ids = new int[filters.Length];
            int count = 0;
            ArrayList sqlDbIds = new ArrayList();

            for( int i = 0; i < filters.Length; i++ )
            {
               if( filters[i].ColumnId == TraceColumnId.ObjectID )
               {
                  ids[count++] = filters[i].GetIntValue();
               }
               else if( filters[i].ColumnId == TraceColumnId.DatabaseID )
               {
                  sqlDbIds.Add( filters[i].GetIntValue() );
                  finalFilterList.Add( filters[i] );
               }
               else
                  finalFilterList.Add( filters[i] );
            }

            if( count == 0 )
               return filters;

            int [] dbIds = null;
            if( sqlDbIds.Count > 0 && count > 0 )
               dbIds = GetDbIds( sqlDbIds );

            List<string> objNames = GetObjectNames( ids, count, dbIds );  
            // replace object IDs with their full names
            for( int i = 0; i < objNames.Count; i++ )
            {
               try
               {
                  finalFilterList.Add( new TraceFilter( TraceColumnId.SQLcmTableName, 
                                                      
                                                     TraceFilterComparisonOp.Like ,
                                                     objNames[i],
                                                     TraceFilterLogicalOp.OR ));
               }
               catch{}
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     "Error replacing object IDs with object names.",
                                     e,
                                     true );
         }

         return (TraceFilter [])finalFilterList.ToArray( typeof(TraceFilter) );
      }

      //---------------------------------------------------------------------------------
      // GetObjectNames
      //---------------------------------------------------------------------------------
      private List<string>
         GetObjectNames (
            int [] ids,
            int    count,
            int [] dbIds 
         )
      {
         List<string> objNames = new List<string>( );
         string stmt = GetGetObjectNameSQL( ids, count, dbIds );
         Repository rep = new Repository();
         rep.OpenConnection();
         
         using ( SqlCommand cmd = new SqlCommand( stmt, rep.connection ) )
         {
	         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
            using ( SqlDataReader reader = cmd.ExecuteReader() )
            {
               if( reader != null &&
                  reader.HasRows )
               {
                  while( reader.Read() )
                  {
                     string fullName = CoreHelpers.GetTableNameKey( reader.GetString( 0 ),
                                                                    reader.GetString( 1 ) );
                     if( !objNames.Contains(fullName))
                     {
                        objNames.Add( fullName );
                     }
                  }
               }
            }
         }         
         rep.CloseConnection();
         rep = null;

         return objNames;
      }
      //---------------------------------------------------------------------------------
      // GetDbIds
      //---------------------------------------------------------------------------------
      private int []
         GetDbIds (
            ArrayList sqlDbIdList
         )
      {
         ArrayList dbIdList = new ArrayList();
         string stmt = GetQueryDbIdSQL( sqlDbIdList );
         Repository rep = new Repository();
         
         try
         {
            rep.OpenConnection();
            using ( SqlCommand cmd = new SqlCommand( stmt, rep.connection ) )
            {
	            cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  if( reader != null &&
                     reader.HasRows )
                  {
                     while( reader.Read() )
                     {
                        try
                        {
                           dbIdList.Add( reader.GetInt32(0));
                        }
                        catch{}
                     }
                  }
               }
            }  
         }
         catch( Exception e )
         {
            string msg = String.Format( "An error occurred retrieving database IDs for instance {0}",
                                        SQLHelpers.CreateSafeString( jobInfo.instance ) );
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     msg,
                                     e,
                                     true );
            throw e;
         }
         finally
         {
            rep.CloseConnection();
            rep = null;
         }

         return (int [])dbIdList.ToArray( typeof(int));
      }

      //------------------------------------------------------------------
      // GetQueryDbIdSQL
      //------------------------------------------------------------------
      private string
         GetQueryDbIdSQL(
            ArrayList sqlDbIdList
         )
      {
         StringBuilder query = new StringBuilder( "SELECT dbId FROM " 
                                                  + CoreConstants.RepositoryDatabaseTable 
                                                  + " WHERE sqlDatabaseId in ( ");

         for( int i = 0; i < sqlDbIdList.Count - 1; i++ )
            query.AppendFormat( "{0} , ", sqlDbIdList[i] );
         query.AppendFormat( "{0} ) AND srvInstance = {1}", 
            sqlDbIdList[sqlDbIdList.Count-1],
            SQLHelpers.CreateSafeString(jobInfo.instance) );

         return query.ToString();
      }



       //------------------------------------------------------------------
      // GetGetObjectNameSQL - SQL to query audited object names
      //------------------------------------------------------------------
      private string
         GetGetObjectNameSQL(
            int [] ids,
            int    count,
            int [] dbIds
         )
      {
         StringBuilder query = new StringBuilder( "SELECT schemaName, name From " 
                                                  + CoreConstants.RepositoryDatabaseObjectsTable 
                                                  + " WHERE id in ( ");

         for( int i = 0; i < count - 1; i++ )
            query.AppendFormat( "{0} , ", ids[i] );
         query.AppendFormat( "{0} )", ids[count-1] );
         if( dbIds != null && dbIds.Length > 0 )
         {
            query.Append( " and dbId in ( " );
            for( int i = 0; i < dbIds.Length - 1; i++ )
               query.AppendFormat( "{0}, ", dbIds[i] );
            query.AppendFormat( "{0} )", dbIds[dbIds.Length -1] );
         }

         return query.ToString();
      }

      #endregion
	}
}
