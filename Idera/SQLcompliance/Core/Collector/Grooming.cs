using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Scripting ;
using Idera.SQLcompliance.Core.Stats;

namespace Idera.SQLcompliance.Core.Collector
{
	/// <summary>
	/// Summary description for Grooming.
	/// </summary>
	internal class Grooming
	{
      #region Private Member Fields
      private Repository    _rep = null;
      private SqlConnection _conn = null;

      private DateTime      m_endTime;
	   private int           m_batchSize;
      private int _groomingAge ;
      
      #endregion
	   
      #region Private Constructor

      internal Grooming()
      {
         _rep = new Repository();
         _rep.OpenConnection();
         _conn = _rep.connection;
      }

      #endregion

      #region Public Methods

      //---------------------------------------------------------------
      // Groom - Groom all instance databases
      //
      //---------------------------------------------------------------
      internal CMCommandResult GroomAll(int groomingAge, IntegrityCheckAction icAction)
      {
         return GroomAll(groomingAge, icAction, CoreConstants.groomBatchSize);
      }
	   
      //---------------------------------------------------------------
      // Groom - Groom all instance databases
      //
      //---------------------------------------------------------------
      internal CMCommandResult GroomAll(int groomingAge, IntegrityCheckAction icAction, int batchSize)
      {
         CMCommandResult retVal = new CMCommandResult() ;
         // calculate last event time to keep
         // since all event databases are stored in UTC we get to 
         // calculate once and use for all            
         m_endTime        = DateTime.UtcNow;
         TimeSpan offset  = new TimeSpan( groomingAge, 0, 0, 0);
         m_endTime       -= offset; // Convert to local time
         _groomingAge = groomingAge ;

         m_batchSize = batchSize;

         // groom all instances - integrity check must be done here
         string [] instances = GetInstancesToGroom();
         
         if( instances == null )
         {
            CMCommandResult result = new CMCommandResult(ResultCode.Error) ;
            result.AddResultString("An error occurred getting instances to groom.") ;
            return result ;
         }

         for( int i = 0; i < instances.Length; i++ )
         {
            try
            {
               GroomOneInstance( instances[i], icAction);
               retVal.AddResultString(String.Format("Grooming operation completed for instance {0}", instances[i])) ;
            }
            catch(Exception e)
            {
               retVal.AddResultString(e.Message) ;
            }
         }
         return retVal ;
      }
	   
      internal CMCommandResult Groom(string instance, int groomingAge, IntegrityCheckAction icAction)
      {
         return Groom(instance, groomingAge, icAction, CoreConstants.groomBatchSize);
      }


      //---------------------------------------------------------------
      // Groom - Groom one instance database
      //
      //---------------------------------------------------------------
      internal CMCommandResult Groom(string instance, int groomingAge, IntegrityCheckAction icAction, int batchSize )
      {
         CMCommandResult retVal ;

         // calculate last event time to keep
         // since all event databases are stored in UTC we get to 
         // calculate once and use for all            
         m_endTime        = DateTime.UtcNow;
         TimeSpan offset  = new TimeSpan( groomingAge, 0, 0, 0);
         m_endTime       -= offset; // Convert to local time
         _groomingAge = groomingAge ;
         m_batchSize = batchSize;

         // groom specified instance - integrity check done by GUI
         GroomOneInstance( instance, icAction);
         retVal = new CMCommandResult() ;
         retVal.AddResultString(String.Format("Grooming operation completed for instance {0}", instance)) ;
         return retVal ;
      }

      //---------------------------------------------------------------
      // Archive - archive events and logs for a single instance.
      //---------------------------------------------------------------
      internal void GroomOneInstance(string instance, IntegrityCheckAction icAction)
      {
         Repository rep = new Repository();
         try
         {
            rep.OpenConnection();
            if ( ! ServerRecord.ServerIsAudited(instance,rep.connection ) )
            {
               throw new Exception( String.Format( CoreConstants.Exception_ServerDeleted, instance ));
            }
         }
         finally
         {
            rep.CloseConnection();
         }
      
         try
         {
            rep = new Repository() ;
            rep.OpenConnection() ;
            IntegrityChecker ic = new IntegrityChecker();
               
            // check database integrity before grooming
            if (icAction == IntegrityCheckAction.PerformCheck)
            {
               string msg = String.Format("Checking integrity of {0} before groom operation", instance);
               ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                  msg );
            
               CheckResult result = ic.CheckIntegrity(instance, false );
               if ( ! result.intact )
               {
                  ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                     String.Format( "Grooming aborted for instance {0} - Integrity check failure", instance) );

                  throw new Exception( String.Format( CoreConstants.Exception_GroomFailedIntegrity,
                     instance ) );
               }
            }
            else if(icAction == IntegrityCheckAction.SkipCheck)
            {
               LogRecord.WriteLog(rep.connection, LogType.ManualIntegrityCheck, instance,
                  "Skipped integrity check before groom operation" );
            }
         
            string instanceDbName = GetInstanceDbName( instance );
            
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
               String.Format( "Grooming operation started for instance {0}: \n" +
               "\tGroom age: events earlier than {1}\n" +
               "\tBatch size: {2}\n", 
               instance,
               m_endTime,
               m_batchSize ) );
                                       
            EventDatabase.SetDatabaseState(instanceDbName, EventsDatabaseState.Busy);
            //Status.PercentDone = 0;
            GroomEvents( instanceDbName );
            GroomEventSQL( instanceDbName );
            GroomSQLsecureLog( instance );
            GroomStats( instanceDbName );
            GroomDataChangeTables( instanceDbName );
            GroomSensitiveColumns(instanceDbName);

            if(icAction != IntegrityCheckAction.SkipCheck)
               ic.RebuildChain( instance, instanceDbName, false);
            else
               EventDatabase.SetDatabaseState(instanceDbName, EventsDatabaseState.NormalChainBroken);
            
            ErrorLog.Instance.Write( String.Format( "Grooming operation completed for instance {0}", instance) );

            string snapshot = String.Format(CoreConstants.Info_GroomSuccessLog, _groomingAge, instance) ;
            
            LogRecord.WriteLog( rep.connection, LogType.Groom, instance, snapshot);

         }
         catch( Exception e)
         {
            string msg = String.Format( "An error occurred grooming events for instance {0}. Error message: {1}.",
               instance,
               e.Message );
            ErrorLog.Instance.Write( msg, ErrorLog.Severity.Warning );

            string logMsg = String.Format( CoreConstants.Error_GroomFailedLog,
               instance,
               e.Message );            
                                        
            LogRecord.WriteLog( rep.connection, LogType.Groom, instance, logMsg);
            
            throw e;
         }
         finally
         {
            rep.CloseConnection() ;
         }
      }
      
      #endregion

      #region WorkUtility Functions

      //---------------------------------------------------------------
      // GetInstanceDbName
      //---------------------------------------------------------------
      private string
         GetInstanceDbName(
            string instance
         )
      {
         string dbName = "";

         try
         {
            string query = GetInstanceDbNameQuery( instance );

            using ( SqlCommand command = new SqlCommand( query, _conn ) )
            {
               using  ( SqlDataReader reader = command.ExecuteReader() )
               {
                  if( reader != null && reader.Read() )
                  {
                     if( !reader.IsDBNull(0) )
                        dbName = reader.GetString(0);
                  }
               }
            }
         }
         catch( Exception e)
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
               e,
               true );
         }

         return dbName;
      }
      
      //---------------------------------------------------------------
      // GetInstanceDbNameQuery
      //---------------------------------------------------------------
      private string
         GetInstanceDbNameQuery(
            string instance
         )
      {
         return String.Format( "SELECT eventDatabase FROM {0} WHERE instance = {1}",
                               CoreConstants.RepositoryServerTable,
                               SQLHelpers.CreateSafeString( instance ) );
                      
      }
      

      //---------------------------------------------------------------
      // Execute - Execute a non-query SQL statement
      //---------------------------------------------------------------
      private int
         Execute(
            string stmt
         )
      {
         int nRows ;
         SqlTransaction trans;
         
         try
         {
            using( SqlCommand command = new SqlCommand( stmt, _conn ) )
            {
               using( trans = _conn.BeginTransaction(IsolationLevel.ReadUncommitted))
               {
                  try
                  {
                     command.Transaction = trans;
                     command.CommandTimeout = CoreConstants.sqlcommandTimeout;
                     nRows = command.ExecuteNonQuery();
                     trans.Commit();
                  }
                  catch( Exception ex )
                  {
                     if( trans != null )
                        trans.Rollback();
                     throw ex;
                  }
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                     String.Format( "An error occurred when executing groom SQL statement.  Statement text: {0}.",
                                                    stmt),
                                     e,
                                     true );
            throw e;
         }
         
         return nRows;
      }

      //---------------------------------------------------------------
      // GetInstancesToGroom
      //---------------------------------------------------------------
      private string []
         GetInstancesToGroom()
      {
         ArrayList instanceList = new ArrayList();

         try
         {
            string query = GetGroomingInstancesStatement();
            using ( SqlCommand command = new SqlCommand( query, _conn ) )
            {
               using ( SqlDataReader reader = command.ExecuteReader() )
               {
                  if( reader != null )
                  {
                     while( reader.Read() )
                     {
                        if( !reader.IsDBNull(0) )
                           instanceList.Add( reader.GetString(0) );
                     }
                  }
               }
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Debug,
               "An error occurred getting instances to groom.",
               e,
               true );
         }

         return (string [])instanceList.ToArray( typeof(string) );
      }
      
      //---------------------------------------------------------------
      // GetGroomingInstancesStatement
      //---------------------------------------------------------------
      private string
         GetGroomingInstancesStatement()
      {
         return String.Format( "SELECT instance FROM {0}.dbo.{1} where isAuditedServer = 1",
                               SQLHelpers.CreateSafeDatabaseName( CoreConstants.RepositoryDatabase ),
                               CoreConstants.RepositoryServerTable );

      }
      
      #endregion
      
      #region Event and EventSql Grooming

      //---------------------------------------------------------------
      // GroomEvents
      //---------------------------------------------------------------
      private void
         GroomEvents(
            string    instanceDbName
         )
      {
         GroomRecords(instanceDbName, CoreConstants.RepositoryEventsTable);
      }

      //---------------------------------------------------------------
      // GroomEventSQL
      //---------------------------------------------------------------
      private void
         GroomEventSQL(
            string    instanceDbName 
         )
      {
         GroomRecords(instanceDbName, CoreConstants.RepositoryEventSqlTable);
      }

      //---------------------------------------------------------------
      // MoreToDelete
      //---------------------------------------------------------------
      private int
         MoreToDelete(
            string    databaseName,
            string    tableName 
         )
      {
         int count ;
         
         string stmt = GetMoreToDeleteStatement( databaseName, tableName );
         using ( SqlCommand cmd = new SqlCommand( stmt, _rep.connection ) )
         {
	         cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

				object obj = cmd.ExecuteScalar();
            if( obj is DBNull )
               count = 0;
            else
               count = (int)obj;
         }
         
         return count;
      }
      
      //---------------------------------------------------------------
      // GetMoreToDeleteStatement
      //---------------------------------------------------------------
      private string
         GetMoreToDeleteStatement( 
            string databaseName,
            string tableName
            )
      {
         return String.Format( "SELECT count(eventId) " +
                               "FROM {0}.dbo.{1} " +
                               "WHERE startTime < {2}",
                               SQLHelpers.CreateSafeDatabaseName( databaseName ),
                               tableName,
                               SQLHelpers.CreateSafeDateTime(m_endTime) );
      }

      //---------------------------------------------------------------
      // GetDeleteRecordStatement
      //---------------------------------------------------------------
      private string
         GetDeleteRecordStatement(
            string databaseName,
            string tableName )
      {
         if( _rep.SqlVersion >= 9 )
            return String.Format( "DELETE TOP ( {0} ) FROM {1}.dbo.{2} WHERE startTime < {3}",
                                 m_batchSize,
                                 SQLHelpers.CreateSafeDatabaseName( databaseName ),
                                 tableName,
                                 SQLHelpers.CreateSafeDateTime(m_endTime) );
         else
            return String.Format("DELETE {0}.dbo.{1} " +
                                 "FROM ( SELECT TOP {2} eventId " +
                                       "FROM {0}.dbo.{1} " +
                                       "WHERE startTime < {3} ) as e " +
                                       "WHERE e.eventId={0}.dbo.{1}.eventId " +
                                       "OPTION ( FAST {2} )",
                                 SQLHelpers.CreateSafeDatabaseName( databaseName ),
                                 tableName,
                                 m_batchSize,
                                 SQLHelpers.CreateSafeDateTime(m_endTime) );
      }
	   
	   
      //---------------------------------------------------------------
      // Groom records from a table
      //---------------------------------------------------------------
      private void
         GroomRecords(
            string    instanceDbName,
            string    tableName
         )
      {
         int nRows = -1;
         int rowsToDelete = MoreToDelete( instanceDbName, tableName );  
         ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                  String.Format( "Grooming {0}.dbo.{1} table: Total rows to delete: {2}", 
                                                 instanceDbName,
                                                 tableName,
                                                 rowsToDelete ) );
         
         int start = Environment.TickCount;
         
         int totalRows = 0;
         try
         {
            string stmt = GetDeleteRecordStatement( instanceDbName,
                                                    tableName );
            nRows = Execute( stmt );
            while ( nRows > 0 )
            {
               totalRows += nRows;
               nRows = Execute( stmt );
               if ( nRows <= 0 )
                  ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                           String.Format( "Last return value: {0}", nRows ) );
               else 
                  ErrorLog.Instance.Write( ErrorLog.Level.Debug,
                                           String.Format( "Groom: {0} rows deleted from {1}.dbo.{2}", nRows, instanceDbName, tableName ) );
            }
         }
         finally
         {
            int stop = Environment.TickCount;
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     String.Format( "Groom {0}.dbo.{1}: Total Rows: {2} Last: {3} Time: {4}",
                                                    instanceDbName,
                                                    tableName,
                                                    totalRows,
                                                    nRows,
                                                    stop - start ) );
         }
         
      }

      #endregion
      
      #region Change Log Grooming      
      
      //---------------------------------------------------------------
      // GroomSQLsecureLog
      //---------------------------------------------------------------
      private void
         GroomSQLsecureLog(
            string    instance
         )
      {
         string stmt = GetDeleteSQLsecureLogStatement( instance );
         
         int nRows = Execute( stmt );
         while ( nRows > 0 )                                                 
         {
            nRows = Execute( stmt );
         }
      }

      //---------------------------------------------------------------
      // GetDeleteSQLsecureLogStatement
      //---------------------------------------------------------------
      private string
         GetDeleteSQLsecureLogStatement(
            string   instance
         )
      {
         return String.Format( "DELETE {0}.dbo.{1} " +
                               "FROM (SELECT TOP {2} logId " +
                                     "FROM {0}.dbo.{1} " +
                                     "WHERE eventTime < {3} " +
                                     "AND logSqlServer = {4} ) as l " +
                               "WHERE l.logId={0}.dbo.{1}.logId",
                               SQLHelpers.CreateSafeDatabaseName( CoreConstants.RepositoryDatabase ),
                               CoreConstants.RepositoryChangeLogEventTable,
                               m_batchSize,
                               SQLHelpers.CreateSafeDateTime(m_endTime),
                               SQLHelpers.CreateSafeString( instance ) );
      }
      
      #endregion
      
      #region Stats Grooming

      //---------------------------------------------------------------
      // GroomStats
      //---------------------------------------------------------------
      private void
         GroomStats(
            string dbName
         )
      {
         string stmt = StatsDAL.CreateDeleteOldStatsStmt( dbName, m_endTime );
         Execute(stmt);
      }

      #endregion
      
      #region Before/after data grooming
      
      private void GroomDataChangeTables( string instanceDbName )
      {
          if (_rep.SqlVersion >= 9)
          {
              GroomDataChangeTable(instanceDbName, CoreConstants.RepositoryColumnChangesTable);
              GroomDataChangeTable(instanceDbName, CoreConstants.RepositoryDataChangesTable);
          }
      }
      

	   private void GroomDataChangeTable( string dbName, string table )
      {
         try
         {
            string stmt = GetGroomDataChangeTableStatement( dbName, table );

            int nRows = Execute( stmt );
            while ( nRows > 0 )
            {
               nRows = Execute( stmt );
            }
         }
         catch( Exception e )
         {
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     String.Format(
                                        "An error occurred grooming table {0} in database {1}.",
                                        table,
                                        dbName ),
                                     e,
                                     ErrorLog.Severity.Warning,
                                     true );
            throw;
         }
      }

      //---------------------------------------------------------------
      // GetDeleteSQLsecureLogStatement
      //---------------------------------------------------------------
      private string
         GetGroomDataChangeTableStatement( string dbName, string tableName)
      {
         // Before/after data capturing is supported on SQL2005 and later so we can use the new delete syntax.
         return String.Format( "DELETE TOP ( {0} ) FROM {1}.dbo.{2} WHERE startTime < {3}",
                               m_batchSize,
                               SQLHelpers.CreateSafeDatabaseName(dbName),
                               tableName,
                               SQLHelpers.CreateSafeDateTime(m_endTime));
      }

      //Groom sensitive columns
      private void GroomSensitiveColumns(string dbName)
      {
         string stmt = GetGroomSensitiveColumnTableStatement(dbName, CoreConstants.RepositorySensitiveColumnsTable);

         int nRows = Execute(stmt);

         while (nRows > 0)
         {
            nRows = Execute(stmt);
         }
      }

      private string GetGroomSensitiveColumnTableStatement(string dbName, string tableName)
      {
         if (_rep.SqlVersion >= 9)
            return String.Format("DELETE TOP ( {0} ) FROM {1}.dbo.{2} WHERE startTime < {3}",
                                 m_batchSize,
                                 SQLHelpers.CreateSafeDatabaseName(dbName),
                                 tableName,
                                 SQLHelpers.CreateSafeDateTime(m_endTime));
         else
            return String.Format("DELETE {0}.dbo.{1} " +
                                 "FROM ( SELECT TOP {2} eventId " +
                                       "FROM {0}.dbo.{1} " +
                                       "WHERE startTime < {3} ) as e " +
                                       "WHERE e.eventId={0}.dbo.{1}.eventId " +
                                       "OPTION ( FAST {2} )",
                                 SQLHelpers.CreateSafeDatabaseName(dbName),
                                 tableName,
                                 m_batchSize,
                                 SQLHelpers.CreateSafeDateTime(m_endTime));
      }
      #endregion
   }
}

