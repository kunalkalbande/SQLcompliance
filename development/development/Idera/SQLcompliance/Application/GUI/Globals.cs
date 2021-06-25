using System ;
using System.Data.SqlClient ;
using System.Diagnostics ;
using Idera.SQLcompliance.Application.GUI.Helper;
using Idera.SQLcompliance.Application.GUI.SQL ;
using Idera.SQLcompliance.Core ;
using Idera.SQLcompliance.Core.Collector ;
using Microsoft.Win32 ;

namespace Idera.SQLcompliance.Application.GUI
{
	/// <summary>
	/// Summary description for Globals.
	/// </summary>
	public class Globals
	{
	   static public string                     RepositoryServer;
	   static public SQLRepository              Repository      = new SQLRepository();
	   static public SQLcomplianceConfiguration SQLcomplianceConfig = new SQLcomplianceConfiguration();
	   
	   static public bool   isAdmin            = true;   // SQLcompliance admin
	   static public bool   isAuditor          = true;
	   static public bool   isSysAdmin         = true;   // SQL Server admin
       static public bool isServerNodeSelected = false;
//	   //-------------------------------------
//	   // Preferences (persisted in registry)
//	   //-------------------------------------
//	   static public bool         ViewGroupByColumn = true;
//	   static public bool         ViewCommonTasks   = true;
//	   static public bool         ViewToolbar       = true;
//	   static public bool         ViewBanners       = true;
//
//      static public EventViewFilter ActivityLogViewFilter = new EventViewFilter();
//      static public EventViewFilter ChangeLogViewFilter = new EventViewFilter();
//      static public AlertViewFilter  AlertsViewFilter   = new AlertViewFilter();
//      static public EventViewFilter  EventsViewFilter   = new EventViewFilter();
//      static public EventViewFilter  ArchiveViewFilter = new EventViewFilter( true );
//      
      static public int          EventPageSizeDefault   = 1000;
//      static public int          EventPageSize          = EventPageSizeDefault;
//
      static public int AlertPageSizeDefault = 1000 ;
//      static public int MaxRecentAlertsDefault = 100 ;
//      static public int AlertPageSize = AlertPageSizeDefault ;
//      
//      static public bool         ShowLocalTime     = false;
      
      static public int          repositorySqlVersion = 0;
      
      
      
//		static private ErrorLog.Level  logLevel   = ErrorLog.Level.Default;
	   
	   //--------------------------------------------------------
	   // Constructor
	   //--------------------------------------------------------
		private Globals()
		{
		}
		
		//--------------------------------------------------------
		// ReadConfiguration - Load preferences from local registry
		//--------------------------------------------------------
		static public bool
		   ReadConfiguration()
		{
         bool retVal ;
		   // get repository sql server version
		   repositorySqlVersion = SQLHelpers.GetSqlVersion(Repository.Connection);
		   
		   // read rest of configuration table

         retVal =  SQLcomplianceConfig.Read(Repository.Connection);
         CoreConstants.AllowCaptureSql = ReadAllowCaptureSql() ;
         if (Settings.Default.LogLevel < 0) Settings.Default.LogLevel = 1;
         if (Settings.Default.LogLevel > 3) Settings.Default.LogLevel = 4;
         ErrorLog.Instance.ErrorLevel = (ErrorLog.Level)Settings.Default.LogLevel;
         return retVal;
		}

      private static bool ReadAllowCaptureSql()
      {
         try
         {
             RemoteCollector srv = GUIRemoteObjectsProvider.RemoteCollector();
            return srv.AllowCaptureSql() ;
         }
         catch (Exception)
         {
            try
            {
               string sql = String.Format( "SELECT count(dbId) FROM {0}..{1} WHERE auditCaptureSQL=2",
                  CoreConstants.RepositoryDatabase,
                  CoreConstants.RepositoryDatabaseTable) ;
               using ( SqlCommand cmd = new SqlCommand( sql, Repository.Connection ) )
               {
                  int count = (int)cmd.ExecuteScalar();
                  if(count > 0)
                     return false ;
               }
               sql = String.Format( "SELECT count(srvId) FROM {0}..{1} WHERE auditUserCaptureSQL=2",
                  CoreConstants.RepositoryDatabase,
                  CoreConstants.RepositoryServerTable) ;
               using ( SqlCommand cmd = new SqlCommand( sql, Repository.Connection ) )
               {
                  int count = (int)cmd.ExecuteScalar();
                  if(count > 0)
                     return false ;
               }
            }
            catch(Exception ex)
            {
               ErrorLog.Instance.Write(ErrorLog.Level.Default, "AllowCaptureSql", ex) ;
            }
         }
         return true ;
      }

		//--------------------------------------------------------
		// ReadPreferences - Load preferences from local registry
		//--------------------------------------------------------
		static public void ImportPreferences()
		{
         RegistryKey rk  = null;
         RegistryKey rks = null;
         
		   try
		   {
            rk  = Registry.CurrentUser;
            rks = rk.OpenSubKey(UIConstants.RegKeyGUI);
            CoreConstants.sqlcommandTimeout = Settings.Default.SqlCommandTimeout;

            // Nothing to import in this case
            if(rks == null)
               return ;
            
	         Settings.Default.ViewToolbar = (1 == (int)rks.GetValue( UIConstants.RegVal_ViewToolbar,       1 ));

	         Settings.Default.EventPageSize   = (int)rks.GetValue( UIConstants.RegVal_EventPageSize, 1000 );
            if (Settings.Default.EventPageSize < 1 || Settings.Default.EventPageSize > 99999)
               Settings.Default.EventPageSize = EventPageSizeDefault;
	         
            Settings.Default.AlertPageSize   = (int)rks.GetValue( UIConstants.RegVal_AlertPageSize, AlertPageSizeDefault );
            if (Settings.Default.AlertPageSize < 1 || Settings.Default.AlertPageSize > 99999)
               Settings.Default.AlertPageSize = AlertPageSizeDefault;
	         
	         Settings.Default.ShowLocalTime = (1 == (int)rks.GetValue( UIConstants.RegVal_ShowLocalTime, 1 ));
            
	         int lvl = (int)rks.GetValue( UIConstants.RegVal_LogLevel, 1 ); 
            if ( lvl < 0 ) lvl = 1;
            if ( lvl > 3 ) lvl = 4;
            Settings.Default.LogLevel = lvl ;
            ErrorLog.Instance.ErrorLevel = (ErrorLog.Level)Settings.Default.LogLevel;
            
            ErrorLog.Instance.Write( ErrorLog.Level.Verbose,
                                     "ErrorLog.Level = " + Settings.Default.LogLevel);
                                     
            // timeout value for SqlCommand (note that is not a default - it has to be
            // explicitly set on your SqlCommand objects before execution
	         Settings.Default.SqlCommandTimeout = (int)rks.GetValue( CoreConstants.CollectionService_RegVal_SqlCommandTimeout, 0 );
            if (Settings.Default.SqlCommandTimeout < 30) 
               CoreConstants.sqlcommandTimeout = CoreConstants.CollectionService_DefaultSqlCommandTimeout;
            else
               CoreConstants.sqlcommandTimeout = Settings.Default.SqlCommandTimeout ;

            // Now that we have imported the settings, we store them and nuke the registry
            Settings.Default.Save() ;
            rks.Close() ;
            rks = null ;
            rk.DeleteSubKeyTree(UIConstants.RegKeyGUI) ;
         }
         catch (Exception ex)
         {
            Debug.WriteLine( ex.Message );
         }
         finally
         {
            if ( rks != null )rks.Close();
            if ( rk != null )rk.Close();
         }
		}
	}
}
