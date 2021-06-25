using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Security.Principal;


namespace Idera.SQLcompliance.Core
{
	public enum LogType : int
	{
	   Unknown               = 0,
	   ServerStartup         = 1,
	   ServerStop            = 2,
	   UpdateLicense         = 3,
	   NewServer             = 11,
	   RemoveServer          = 12,
	   ModifyServer          = 13,
	   DisableServer         = 14,
	   EnableServer          = 15,
	   DeployAgent           = 16,
	   ManualDeployAgent     = 17,
	   NewDatabase           = 20,
	   RemoveDatabase        = 21,
	   ModifyDatabase        = 22,
	   DisableDatabase       = 23,
	   EnableDatabase        = 24,
	   NewLogin              = 31,
	   DeleteLogin           = 32,
	   ModifyLogin           = 33,
	   Snapshot              = 36,
      Groom                 = 37,
      Archive               = 38,
      ConfigureAutoArchive  = 39,
      AuditSnapshotSchedule = 40,
      IntegrityBroken       = 41,
      ChangeAgentProperties = 42,
      ManualIntegrityCheck  = 43,
      RemoveEventsDatabase  = 44,
      AutoArchiveJob        = 45,
      CaptureSnapshotJob    = 46,
      IntegrityCheckJob     = 47,
      ArchiveChanged        = 48,
      ConfigureRepository   = 49,
      AttachArchive         = 50,
      DetachArchive         = 51,
      LoginFilteringChanged = 52,
      AlertRuleAdded        = 53,
      AlertRuleRemoved      = 54,
      AlertRuleModified     = 55,
      AlertRuleDisabled     = 56,
      AlertRuleEnabled      = 57,
      GroomAlerts           = 58,
      EventFilterAdded      = 59,
      EventFilterRemoved    = 60,
      EventFilterModified   = 61,
      EventFilterDisabled   = 62,
      EventFilterEnabled    = 63,
      ReIndexStarted        = 64,
      ReIndexEnded          = 65,
      EventDatabaseFull     = 66
}	 
   
   
	/// <summary>
	/// Summary description for LogRecord.
	/// </summary>
	public class LogRecord
	{
      #region Properties
      
      public int                 logId;
      public LogType             logType;
      public string              logUser;
      public DateTime            eventTime;
      public string              logSqlServer;
      public string              logInfo;
      
      public string              logTypeString;   // the other table
      
      #endregion
	
	   #region Constructor
	   
	   public LogRecord()
	   {
		   eventTime        = DateTime.UtcNow;
         logUser          = "";
         logType          = LogType.Unknown;
         logSqlServer     = "";
         logInfo          = "";
         
         logTypeString    = "Unknown";
	   }
	   
	   #endregion
	   
	   #region Read methods
	   
	   //----------------------------------------------------------------
	   // ReadLog
	   //----------------------------------------------------------------
		public void
		   Read(
		      SqlConnection conn,
		      int           id
	      )
	   {
	      try
	      {
	         string sqlfmt = "SELECT  l.logId,l.eventTime,l.logType,t.Name,l.logUser,l.logSqlServer,l.logInfo " +
	                         "FROM {0}..{1} as l,{0}..{2} as t " +
	                         "WHERE l.logId={3} AND l.logType=t.eventId;";
	         string sql    = String.Format( sqlfmt,
	                                       CoreConstants.RepositoryDatabase,
	                                       CoreConstants.RepositoryChangeLogEventTable,
	                                       CoreConstants.RepositoryChangeLogEventTypeTable,
	                                       id );
            using ( SqlCommand cmd = new SqlCommand( sql, conn ) )
            {
               using ( SqlDataReader reader = cmd.ExecuteReader() )
               {
                  if ( reader.Read() )
                  {
                     int col=0;
                     
                     logId         = SQLHelpers.GetInt32( reader, col++ );
                     eventTime     = SQLHelpers.GetDateTime( reader, col++ );
                     logType       = (LogType)SQLHelpers.GetInt32( reader, col++ );
                     logTypeString = SQLHelpers.GetString( reader, col++ );
                     logUser       = SQLHelpers.GetString( reader, col++ );
                     logSqlServer  = SQLHelpers.GetString( reader, col++ );
                     logInfo       = SQLHelpers.GetString( reader, col++ );
                  }
               }
            }
         }            
         catch (Exception ex)         
         {
            ErrorLog.Instance.Write( "Error reading log record",
                                     ex.Message );
            throw ex;                                     
         }
         	                                     
	                                     
	   }
	   
	   
	   #endregion
	
	   #region Static WriteLog Variants
	   
	   //----------------------------------------------------------------
	   // WriteLog - Generic logging
	   //----------------------------------------------------------------
		static public void
		   WriteLog(
		      SqlConnection conn,
		      LogType       type
	      )
	   {
		   WriteLog( conn,
		             type,
		             "",
		             "" );
	   }
	   
	   //----------------------------------------------------------------
	   // WriteLog - Generic log with extra info
	   //----------------------------------------------------------------
		static public void
		   WriteLog(
		      SqlConnection conn,
		      LogType       type,
		      string        info
	      )
	   {
		   WriteLog( conn,
		             type,
		             "",
		             info );
	   }
	   
	   //----------------------------------------------------------------
	   // WriteLog - Update to something related to a SQL Server
	   //            and routine that all other overloads call 
	   //            to actually perform write
	   //----------------------------------------------------------------
		static public void
		   WriteLog(
		      SqlConnection conn,
		      LogType       type,
		      string        sqlServer,
		      string        info
        )
	   {
         WindowsIdentity id = WindowsIdentity.GetCurrent();
         
         string   user = "";
		   if ( type != LogType.ServerStartup || type != LogType.ServerStop )
		   {
	         user = id.Name;
		   }
		   
		   WriteLog( conn,
		             type,
		             sqlServer,
		             info,
		             user );
		}


	   //----------------------------------------------------------------
	   // WriteLog - Update to something related to a SQL Server
	   //            and routine that all other overloads call 
	   //            to actually perform write
	   //----------------------------------------------------------------
		static public void
		   WriteLog(
		      SqlConnection conn,
		      LogType       type,
		      string        sqlServer,
		      string        info,
		      string        user
        )
	   {
		   if ( type == LogType.Unknown )
		   {
		      Debug.Write( "ERROR: Log record not initialized properly - not writing log record." );
		   }
		   else
		   {
		      try
		      {
               string cmdStr = String.Format( "INSERT INTO {0} (eventTime, logType, logUser, logSqlServer, logInfo) " + 
                                              "VALUES (GETUTCDATE(),{1},{2},{3},{4})",
                                              CoreConstants.RepositoryChangeLogEventTable,
                                              (int)type,
                                              SQLHelpers.CreateSafeString(user),
                                              SQLHelpers.CreateSafeString(sqlServer),
                                              SQLHelpers.CreateSafeString(info) );
               using ( SqlCommand cmd = new SqlCommand( cmdStr, conn ) )
               {
                  cmd.ExecuteNonQuery();
               }
            }
            catch ( Exception ex )
            {
               // TODO: If we cant write status log, we need to shut down since we cant audit??
		         ErrorLog.Instance.Write( ErrorLog.Level.Always,
		                                  CoreConstants.Exception_ErrorWritingLogRecord,
		                                  ex.Message,
		                                  ErrorLog.Severity.Error );
            }
         }
		}
         
      #endregion
	}
}
