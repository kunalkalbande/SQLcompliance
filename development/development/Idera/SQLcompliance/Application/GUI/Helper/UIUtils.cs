using System;
using System.IO;
using System.Net.Sockets;
using System.Data;
using System.Data.SqlClient;

using Idera.SQLcompliance.Core;

namespace Idera.SQLcompliance.Application.GUI.Helper
{
    /// <summary>
    /// Deselect Options for user
    /// </summary>
    /// <remarks>
    /// <see cref="Idera.SQLcompliance.Application.GUI.Forms.Form_SelectionLogicDialog"/>
    /// </remarks>
    public enum DeselectOptions
    {
        None,   // Comes where Selection Happened
        CurrentLevelOnly,    // Deselect with Current Level Changes only
        OtherLevels  // Deselect with Other applicable levels based on the hierarchy
    }

    public enum DeselectControls
    {
        None,

        ServerLogins,

        ServerLogouts,

        ServerFailedLogins,

        ServerSecurityChanges,

        ServerDatabaseDefinition,

        ServerAdministrativeActivities,

        ServerFilterEvents,

        ServerFilterEventsPassOnly,

        ServerFilterEventsFailedOnly,

        ServerUserDefined,

        ServerUserLogins,

        ServerUserLogouts,

        ServerUserFailedLogins,

        ServerUserSecurityChanges,

        ServerUserAdministrativeActivities,

        ServerUserDatabaseDefinition,

        ServerUserDatabaseModifications,

        ServerUserDatabaseSelect,

        ServerUserUde,

        ServerUserCaptureSqlDmlSelect,

        ServerUserCaptureSqlTransactionStatus,

        ServerUserCaptureSqlDdlSecurity,

        ServerUserFilterEvents,

        ServerUserFilterEventsPassOnly,

        ServerUserFilterEventsFailedOnly,

        DbDatabaseDefinition,

        DbSecurityChanges,

        DbAdministrativeActivities,

        DbDatabaseModifications,

        DbDatabaseSelect,

        DbFilterEvents,

        DbFilterEventsPassOnly,

        DbFilterEventsFailedOnly,

        DbCaptureSqlDmlSelect,

        DbCaptureSqlTransactionStatus,

        DbCaptureSqlDdlSecurity
    }

    /// <summary>
    /// Summary description for UIUtils.
    /// </summary>
    public class UIUtils
    {
        public const string SelectionLogicTitle = "Selection Logic";

        public const string DeselectCurrentLevel = "Deselect Current Level Only";

        public const string DeselectDatabaseLevel = "Deselect at Database Level Auditing Only";

        public const string DeselectServerAndAllDatabases = "Deselect Server and all Databases";
        public const string DeselectPrivilegeUsersDatabase = "Deselect at Database Level and Privileged Users Auditing";

        public const string DefaultSelectionLogicMessageFormat = "You have deselected \"{0}\". This action should:";

        private const string NoneText = "None";
        private const string LoginsText = "Logins";
        private const string LogoutsText = "Logouts";
        private const string FailedLoginsText = "Failed Logins";
        private const string SecurityChangesText = "Security Changes";
        private const string DatabaseDefinitionText = "Database Definition (DDL)";
        private const string AdministrativeActivitiesText = "Administrative Actions";
        private const string DatabaseModificationText = "Database Modification (DML)";
        private const string DatabaseSelectText = "Database SELECT operations";
        private const string FilterEventsText = "Filter events based on access check";
        private const string UserDefinedEventsText = "User Defined Events";
        private const string CaptureSqlDmlSelectText = "Capture SQL Statements for DML and SELECT activities";
        private const string CaptureTransactionDmlText = "Capture Transaction Status for DML Activity";
        private const string CaptureDdlSecurityText = "Capture SQL statements for DDL and Security Changes";
        public UIUtils()
		{
		}

	    public static string GetDeselectedControlText(DeselectControls deselectControls)
	    {
            switch (deselectControls)
            {
                case DeselectControls.None:
                    return NoneText;
                case DeselectControls.ServerLogins:
                case DeselectControls.ServerUserLogins:
                    return LoginsText;
                case DeselectControls.ServerFailedLogins:
                case DeselectControls.ServerUserFailedLogins:
                    return FailedLoginsText;
                case DeselectControls.ServerSecurityChanges:
                case DeselectControls.ServerUserSecurityChanges:
                case DeselectControls.DbSecurityChanges:
                    return SecurityChangesText;
                case DeselectControls.ServerDatabaseDefinition:
                case DeselectControls.ServerUserDatabaseDefinition:
                case DeselectControls.DbDatabaseDefinition:
                    return DatabaseDefinitionText;
                case DeselectControls.ServerAdministrativeActivities:
                case DeselectControls.ServerUserAdministrativeActivities:
                case DeselectControls.DbAdministrativeActivities:
                    return AdministrativeActivitiesText;
                case DeselectControls.ServerFilterEvents:
                case DeselectControls.ServerUserFilterEvents:
                case DeselectControls.DbFilterEvents:
                    return FilterEventsText;
                case DeselectControls.ServerUserDefined:
                case DeselectControls.ServerUserUde:
                    return UserDefinedEventsText;
                case DeselectControls.ServerUserLogouts:
                case DeselectControls.ServerLogouts:
                    return LogoutsText;
                case DeselectControls.ServerUserDatabaseModifications:
                case DeselectControls.DbDatabaseModifications:
                    return DatabaseModificationText;
                case DeselectControls.ServerUserDatabaseSelect:
                case DeselectControls.DbDatabaseSelect:
                    return DatabaseSelectText;
                case DeselectControls.ServerUserCaptureSqlDmlSelect:
                case DeselectControls.DbCaptureSqlDmlSelect:
                    return CaptureSqlDmlSelectText;
                case DeselectControls.ServerUserCaptureSqlTransactionStatus:
                case DeselectControls.DbCaptureSqlTransactionStatus:
                    return CaptureTransactionDmlText;
                case DeselectControls.ServerUserCaptureSqlDdlSecurity:
                case DeselectControls.DbCaptureSqlDdlSecurity:
                    return CaptureDdlSecurityText;
                default:
                    return deselectControls.ToString();
            }
        }

        //-----------------------------------------------------------------------
        // GetLocalTimeDateString
        //-----------------------------------------------------------------------
        static public string
		   GetLocalTimeDateString(
		      DateTime          time
         )
      {
         string retStr;
         
			if ( time == DateTime.MinValue )
			{
			   retStr = UIConstants.Status_Never;
			}
			else
			{
			   DateTime local = time.ToLocalTime();
			   retStr = String.Format( "{0} {1}",
			                           local.ToShortDateString(),
			                           local.ToShortTimeString() );
			}
			
			return retStr;
      }
		
		//-----------------------------------------------------------------------
		// GetLocalDateString
		//-----------------------------------------------------------------------
		static public string
		   GetLocalDateString(
		      DateTime          time
         )
      {
         string retStr;
         
			if ( time == DateTime.MinValue )
			{
			   retStr = UIConstants.Status_Never;
			}
			else
			{
			   DateTime local = time.ToLocalTime();
			   retStr         = local.ToShortDateString();
			}
			
			return retStr;
      }
      
      //--------------------------------------------------------------------
      // GetInstanceHost
      //--------------------------------------------------------------------
      static public string
         GetInstanceHost(
            string            instance
         )
      {
         string host = "";
         
         int pos = instance.IndexOf(@"\");
		  if (pos > 0)
		  {
			  host = instance.Substring(0,pos);
		  }
		  else
		  {
			  host = instance ;
		  }
         
         // expand local reference
         if ( host == "" || host == "." )
         {
            host = System.Net.Dns.GetHostName().ToUpper();
         }
         
         return host;
      }
      
      //--------------------------------------------------------------------
      // Validate Path
      //--------------------------------------------------------------------
		static public bool
		   ValidatePath(
		      string      filepath
		   )
		{
		   // make sure defined and a local path
		   if ( filepath.Length<3) return false;
			if(filepath.Length > 180) return false ;
		   if ( filepath[1] != ':' ) return false;
		   if ( filepath[2] != '\\' ) return false;
			if(filepath.IndexOf("..") != -1) return false ;
		   
         try
         {
            if ( ! Path.IsPathRooted(filepath) )
               return false;
         }
         catch (Exception)
         {
            return false;
         }
		   return true;
		}
		
      //---------------------------------------------------------------------------
      // IsHeartbeatStale
      //---------------------------------------------------------------------------
      static public bool
         IsHeartbeatStale(
            DateTime heartbeat
         )
      {
         DateTime now = DateTime.UtcNow;
         now = now.AddHours(-1);
         
         if ( now.CompareTo(heartbeat) > 0 )
         {
            return true;
         }
         else
         {
            return false;
         }
      }
      
      //---------------------------------------------------------------------------
      // GetLogLevelString
      //---------------------------------------------------------------------------
      static public string
         GetLogLevelString(
            int logLevel
         )
      {
         if ( logLevel<1 )
            return UIConstants.LogLevel_Silent;
         else if ( logLevel==1 )
            return UIConstants.LogLevel_Normal;
         else if ( logLevel==2 )
            return UIConstants.LogLevel_Verbose;
         else
            return UIConstants.LogLevel_Debug;
      }

      //---------------------------------------------------------------------------
      // GetAgentLogLevelString
      //---------------------------------------------------------------------------
      static public string
         GetAgentLogLevelString(
            int agentLogLevel
         )
      {
         if ( agentLogLevel<1 )
            return UIConstants.AgentLogLevel_Normal;
         else
            return UIConstants.AgentLogLevel_Verbose;
      }
      
      static public int
         TextToInt(
            string   text
         )
      {
         int retval = 0;

         try
         {       
            retval = System.Convert.ToInt32( text );  
         }
         catch
         {
         }
         
         return retval;
      }

      //---------------------------------------------------------------------------
      // TranslateRemotingException
      //---------------------------------------------------------------------------
      public static string
         TranslateRemotingException(
            string    server,
            string    component,
            Exception ex
         )      
      {
         string msg = ex.Message;
         
         try
         {
            SocketException socketEx = (SocketException)ex;
            if ( socketEx.ErrorCode == 10061 )
            {
                msg = String.Format( UIConstants.Error_ServerNotAvailable,
                                     component,
                                     server );
            }
         }
         catch {}
         
         return msg;
      }
      
      
      //---------------------------------------------------------------------------
      // CloseIfConnectionLost
      //---------------------------------------------------------------------------
      public static bool
         CloseIfConnectionLost()
      {
         return CloseIfConnectionLost( Globals.Repository.Connection );
      }
      
      public static bool
         CloseIfConnectionLost(
            SqlConnection  conn
         )
      {
         bool lost = false;
         if ( conn.State != ConnectionState.Open )
         {
            lost = true;
            ErrorMessage.Show( UIConstants.ConsoleWindowName,
                               UIConstants.RepositoryDownCantContinue);
            System.Windows.Forms.Application.Exit();                                 
         }
         return lost;
      }
      
      public static string
         GetIntegrityCheckResult(
            int   result
         )
      {
         if ( result == CoreConstants.IntegrityCheck_InProgress )
         {
            return CoreConstants.IntegrityCheckString_InProgress;
         }
         else if ( result == CoreConstants.IntegrityCheck_Failed )
         {
            return CoreConstants.IntegrityCheckString_Failed;
         }
         else if ( result == CoreConstants.IntegrityCheck_FailedAndRepaired )
         {
            return CoreConstants.IntegrityCheckString_FailedAndRepaired;
         }
         else if ( result == CoreConstants.IntegrityCheck_Incomplete )
         {
            return CoreConstants.IntegrityCheckString_Incomplete;
         }
         else
         {
            return CoreConstants.IntegrityCheckString_Passed;
         }
      }
      
      public static string
         GetArchiveResult(
            int   result
         )
      {
         if ( result == CoreConstants.Archive_InProgress )
         {
            return CoreConstants.ArchiveString_InProgress;
         }
         else if ( result == CoreConstants.Archive_FailedIntegrity )
         {
            return CoreConstants.ArchiveString_FailedIntegrity;
         }
         else if ( result == CoreConstants.Archive_FailedWithErrors )
         {
            return CoreConstants.ArchiveString_FailedWithErrors;
         }
         else if ( result == CoreConstants.Archive_Incomplete )
         {
            return CoreConstants.ArchiveString_Incomplete;
         }
         else
         {
            return CoreConstants.ArchiveString_Passed;
         }
      }

    }
}
