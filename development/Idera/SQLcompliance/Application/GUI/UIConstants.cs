using System.Drawing ;
using System.Windows.Forms ;

namespace Idera.SQLcompliance.Application.GUI
{
	/// <summary>
	/// Summary description for UIConstants.
	/// </summary>
	public class UIConstants
	{
   	private UIConstants()
   	{
   	}
   	
      //---------------------
      // SQLcompliance Constants
      //---------------------
      public static string AppTitle = @"IDERA SQL Compliance Manager";
      
      public static string CollectionServiceName  = "Collection Service";
      public static string AgentServiceName       = "SQLcompliance Agent";
      
      public static string ServerStatus_OK             = "OK";
      public static string ServerStatus_AwaitingManual = "Awaiting manual deployment";
      public static string ServerStatus_Inactive       = "Inactive";
      public static string ServerStatus_NotDeployed    = "Agent not deployed";
      public static string ServerStatus_NotRunning     = "Down";
      public static string ServerStatus_Crippled       = "Error";
      public static string ServerStatus_Disabled       = "Disabled";
      public static string ServerStatus_Enabled        = "Enabled";
      public static string ServerStatus_NoAuditData    = "No Audit Data Collected";
      public static string ServerStatus_Unknown        = "Unknown";
      public static string ServerStatus_NotRegistered  = "Not registered";
      public static string ServerStatus_LicenseExpired = "License Expired";
      public static string ServerStatus_NotAudited     = "Archive server";
      public static string ServerStatus_Pending        = "; Update pending";
      public static string ServerStatus_Requested      = "; Update requested";
      public static string ServerStatus_Stale          = "No recent heartbeat";
      public static string ServerStatus_VeryStale      = "No contact in over a day";
      public static string ServerStatus_NoEventsDatabase = "Error creating database";
      public static string ServerStatus_Down           = "Non-operational";
      public static string ServerStatus_ReviewNeeded   = "Corrupted events; review needed";
      public static string ServerStatus_Initializing   = "Initializing...";
      public static string ServerStatus_2005NotSupported = "SQL Server 2005 Unsupported";
      public static string ServerStatus_2008NotSupported = "SQL Server 2008 Unsupported";
      public static string ServerStatus_2012NotSupported = "SQL Server 2012 Unsupported";
      public static string ServerStatus_2014NotSupported = "SQL Server 2014 Unsupported";
      public static string ServerStatus_2016NotSupported = "SQL Server 2016 Unsupported";	  
      public static string ServerStatus_2017NotSupported = "SQL Server 2017 Unsupported";
      public static string ServerStatus_AgentUpgradeRequired = "Agent Upgrade Required";

      public static string AuditStatus_ViewActivityLog = "View Activity Log";
      
      public static string ServerStatus_UnknownVerbose  = "Collection Server status is unknown. The service is running but has not sent its regular heartbeat message in over an hour.";
      public static string ServerStatus_StartingVerbose = "Collection Server is starting. Status will show as non-operational until initialization complete.";
      public static string ServerStatus_OKVerbose       = "Collection Server is up.";
      public static string ServerStatus_DownVerbose     = "Collection Server is not operational or is inaccessible. When the server is non-operational, no events can be collected from audited SQL Servers.";
      
      public static string RepositoryDownCantContinue =
         "Error: The Management Console has lost its connection to the SQL Compliance Manager Repository " +
         "and cannot reestablish the connection. This may be due to network problems or a problem at the " +
         "SQL Server hosting the Repository. The Management Console cannot continue and is shutting down.";

      public static string Info_DatabaseIsClean        = "OK";
      public static string Info_DatabaseHasErrors      = "Database contains integrity check problems";
      
      public static string LogLevel_Silent             = "Silent";
      public static string LogLevel_Normal             = "Normal";
      public static string LogLevel_Verbose            = "Verbose";
      public static string LogLevel_Debug              = "Debug";

      public static string AgentLogLevel_Normal        = "Normal";
      public static string AgentLogLevel_Verbose       = "Verbose";
      
      public static string OverallStatus_OK            = "All servers are OK";
      public static string OverallStatus_Warning       = "One or more servers need attention";
      public static string OverallStatus_Alert         = "One or more servers are  in critical condition";
      public static string OverallStatus_NoConnection  = "Could not connect to SQL Compliance Manager Repository";
      public static string OverallStatus_ServerDown    = "Collection Server down or not responding";
      public static string OverallStatus_NoServers     = "No registered SQL Servers";
      
      public const int    ServerImage_OK       = 0;
      public const int    ServerImage_Warning  = 1;
      public const int    ServerImage_Alert    = 2;

      public static string ConnectingSplash             = "Connecting to {0}...";
      public static string NoServersLoaded              = "No servers loaded";
      public static string NoLogsLoaded                 = "No logs loaded";
      public static string NoStatusRecordsLoaded        = "No records loaded";

      public enum Table_Column_Usage
      {
         Filter = 0,            
         BADTables = 1,         //3.1
         BADColumns = 2,        //3.2 and up
         SensitiveColumns = 3 
      }
  
      public static string BAD_AllColumns               = "All Columns";
      public static string BAD_NoColumns                = "Not configured";
      public static string SC_AllColumns                = "All Columns";
      public static string SC_Individual                = "Individual";
      public static string SC_Dataset                   = "Dataset";

      public const string ADDTableTitle                 = "Add User Tables for {0} Auditing";
      public const string ADDTableTitle_DML             = "DML/Select";
      public const string ADDTableTitle_BAD             = "Before-After Data";
      public const string ADDTableTilte_SC              = "Sensitive Columns";
      public const string AddTableHeader                = "Select tables to audit";
      public const string AddTableDescription           = "Specific user tables can be selected for DML/Select auditing.";
      public const string AddTableDescription_SC        = "Specific user tables can be selected for Sensitive Column auditing";
      public const string AddTableDescription_BAD       = "Available tables are tables that do not contain BLOB columns such as binary or text.";
      public const string AddTableDescription_BAD_Cols  = "Specific user tables can be selected for Before-After Data auditing and specific columns must be selected for any tables containing BLOB columns.";
      public const string BAD_TABLE_Missing_Title       = ADDTableTitle_BAD;
      public const string Warning_BAD_Table_Missing     = "Audited table {0} is no longer available for auditing. It may have been removed or renamed.";
      public const string Warning_BAD_Tables_Missing    = "Audited tables {0} are no longer available for auditing. They may have been removed or renamed.";
      public const string Warning_BAD_Tables_Removed    = "A table selected for Before-After data auditing no longer exists and will be removed from auditing.\n\nDo you want to continue?";
      public const string Error_Cant_Edit_BAD_Table     = "Table {0} is no longer available for auditing. Remove the table from auditing or add a new table for auditing and all unavailable tables will be removed.";
      public const string Warning_SC_Table_Missing      = "Audited table {0} is no longer available for auditing. It may have been removed or renamed.";
      public const string Warning_SC_Tables_Missing     = "Audited tables {0} are no longer available for auditing. They may have been removed or renamed.";

      // GUI Error Messages      
      public static string Warning_RemoveAuditedDatabases  = "Warning: This will stop auditing of the selected databases. Activity data for future operations performed on these databases will not be available for audit reports once they are removed.";
      public static string Title_RemoveAuditedDatabases    = "Remove Audited Databases";
      public static string Warning_DisableAuditedDatabases = "Warning: This will disable auditing of the selected databases. Activity data for operations performed on disabled databases is not collected and is not available for audit reports.";
      public static string Title_DisableAuditedDatabases   = "Disabled Database Auditing";
      public static string Warning_DisableAuditedServers = "Warning: This will disable auditing of the selected server and its databases. Activity data for operations performed on disabled servers and databases is not collected and is not available for audit reports.";
      public static string Title_DisableAuditedServers   = "Disable Server Auditing";
      
      public static string Title_CantChangeDatabases     = "Cannot change database auditing status";      
      public static string Warning_CantChangeDatabases   = "The auditing status of a database cannot be changed while auditing of the SQL Server hosting the database is disabled.";
      
      public static string Title_RemoveServer            = "Remove SQL Server";
      public static string Warning_RemoveServer          = "Warning: When you remove a registered SQL Server, you stop auditing of " +
                                                           "activities performed on the SQL Server. To temporarily " +
                                                           "pause auditing of a SQL Server, use the Disable Auditing feature instead.\n\n" +
                                                           "Do you wish to remove the SQL Server now?";

      public static string Title_ServerAlreadyAdded = "Server is added";
      public static string Warning_ServerAlreadyAdded = "At this step server is added to repository.\n\nDo you wish to perform delete SQL Server?";
                                                           
      public static string Warning_RemoveClusteredServer = "Warning: When you remove a registered SQL Server, you stop auditing of " +
                                                           "activities performed on the SQL Server. Note that " +
                                                           "this SQL Server is a virtual SQL Server. You " + 
                                                           "will need to manually remove the SQLcompliance Agent from each node in the cluster " +
                                                           "hosting the virtual SQL Server to complete this operation.\n\n" +
                                                           "Do you wish to remove the SQL Server now?";
      public static string Title_RemoveEventsDatabase    = "Keep SQL Server Audit Data";
      public static string Warning_RemoveEventsDatabase
         = "Deleting the audit data collected for an audited SQL Server instance " +
           "may violate your company's auditing practices. We recommend leaving the database " +
           "containing the collected audit data until you are sure it is properly backed up.\n\n" +
           "Do you wish to keep the database containing the audit data for this SQL Server instance?";
      public static string Error_RemoveEventsDatabase = "An error occurred removing the events database for this SQL Server instance. The SQL Server instance will still be removed but you will need to delete the events database at a later time.";
           

      public static string Title_RemoveArchive           = "Remove Archive Database";
      public static string Warning_RemoveArchive
               = "Warning: Removing an archive database from the Repository will make the " +
                 "archive data unavailable for viewing and reporting within SQL compliance " +
                 "manager. The database containing the archived data will not be deleted.\n\n" +
                 "Do you wish to remove the archive database from the Repository now?";

      public static string Info_ManualUninstallRequired   = "The SQLcompliance Agent service for {0} is no longer capturing audit data. " +
                                                            "However, because this agent was installed manually you will need to " +
                                                            "manually run the uninstall to complete this task.";

      public static string Error_CantStartWithoutConnection = "The Management Console cannot be run without making a connection to a SQL Compliance Manager Repository. Try again later when a connection can be established.";
      
      public static string Info_CouldntStopAgent
           = "An error occurred trying to contact the SQLcompliance Agent to stop auditing of {0}. If you continue with " +
             "the removal process, the agent will be left unaffected. If this is the last instance on the computer, you " +
             "will need to manually remove the agent later to complete the process." +
             "\n\nError:\n\n{1}\n\n\n" + 
             "Do you wish to continue with the removal of this SQL Server instance? ";
      public static string Error_DeleteServerProblem      = "An error occurred trying to remove the registered SQL Server: {0}.\n\n" + 
                                                            "Error:\n\n{1}";
      public static string Error_UninstallAgent          = "An error occurred uninstalling the SQLcompliance Agent for {0}.\n\n" + 
                                                            "Error:\n\n{1}";

      public static string Error_CouldntContactCollectionService = "An error occurred connecting to the Collection Service. If you think the service is running and that this error is due to a problem accessing the service we suggest fixing the connection problem before continuing.\n\n" + 
                                                                  "Error:\n\n{0}\n\n\n" + 
                                                                  "Do you wish to continue with the removal of this SQL Server instance?";
      public static string Error_RemoveEventsServerProblem  = "An error occurred trying to remove the SQL Server instance: {0} and its associated audit data.\n\n" + 
                                                            "Error:\n\n{1}";
      
      // Agent Activation Errors
      public static string Title_DeployAgent           = "Deploy SQLcompliance Agent";
      public static string Error_DeployUpdateNowFailed = "Activation of the SQLcompliance Agent for this instance failed. The SQLcompliance Agent for the computer hosting the instance is already installed " +
                                                           "but is not responding to the activation attempt. Check to make sure that the SQLcompliance Agent " +
                                                           "is installed and running.";
      public static string Error_DeployFailed             = "An error occurred installing the SQLcompliance Agent for this instance. The instance will not be audited until the problem preventing the install has been fixed.";
      public static string Info_DeployComplete            = "The SQLcompliance Agent for this instance has been deployed."; 
      
      public static string Title_UpgradeAgent             = "Upgrade SQLcompliance Agent";
      public static string Error_UpgradeFailed            = "An error occurred upgrading the SQLcompliance Agent for the selected SQL Server. Please fix the following problem and then reattempt the upgrade.";
      public static string Error_UpgradeFailedNoInfo      = "An error occurred during verifying a version of the upgraded SQLcompliance Agent for the selected SQL Server. Please check the application event logs for more information.";
      public static string Info_UpgradeComplete           = "The SQLcompliance Agent for this SQL Server has been successfully upgraded."; 
      public static string Info_CantUpgradeLocal          = "The local SQLcompliance Agent may not be upgraded from the management console. You must upgrade the SQLcompliance Agent using the full setup program used to install the SQLcompliance Agent and Management Console.";
      public static string Info_CantUpgradeManual         = "The SQLcompliance Agent may not be upgraded from the management console. You must upgrade the SQLcompliance Agent using the full setup program used to install the manually deployed SQLcompliance Agent.";
      public static string Info_CantUpgradeAgentNewer     = "The SQLcompliance Agent cannot be upgraded from this computer. The remote SQLcompliance Agent is newer then the local version of the Management Console.";
      public static string Info_AlreadyUpgraded           = "The SQLcompliance Agent is already current. No upgrade is necessary";

      public static string Title_ForceCollection = "Collect Audit Data Now";
      public static string Warning_ForceCollection = "The 'Collect Audit Data Now' feature causes the SQLcompliance Agent to forward any collected data to the Collection Server for processing. Depending on the workload at the Collection Server, it may take several minutes for the audit data to be available for viewing and reporting.";
      public static string Error_ForceCollectionFailed = "An error occurred requesting the SQLcompliance Agent to collect and send any gathered audit data.";
      public static string Info_DeployManualInstallOverride = "The SQLcompliance Agent is marked as requiring manual installation. You may continue the attempt to deploy but "+
                                                              "auditing of the instance will not begin until the SQLcompliance Agent is installed." +
                                                              "\n\nDo you want to try to deploy the SQLcompliance Agent now?";


      public static string Error_AuditAfterManualInstall = "This instance has been registered. Auditing will begin " +
                                                              "once the SQLcompliance Agent is manually installed on the " +
                                                              "computer hosting the instance.";
      // Agent Service Control      
      public static string Title_StartAgent   = "Start SQLcompliance Agent";
      public static string Title_StopAgent    = "Stop SQLcompliance Agent";
      public static string Warning_StopAgent  = "Warning: Stopping a SQLcompliance Agent will prevent new trace information from being sent " +
                                                "to the Repository.\n\nDo you want to continue and stop the SQLcompliance Agent service?";
                                               
      public static string Error_AgentNotInstalled = "SQLcompliance Agent is not installed.";
      public static string Error_CantStopAgent     = "The SQLcompliance Agent on {0} cannot be stopped.\n\nError: {1}";
      public static string Info_AgentStopped       = "The SQLcompliance Agent on computer '{0}' was successfully stopped.";
      public static string Error_CantStartAgent    = "The SQLcompliance Agent on {0} cannot be started. This operation will not work if you do not have permissions to start and stop services on the remote machine or if the SQLcompliance Agent is located across a firewall blocking the attempt to start the service.\n\nError: {1}";
      public static string Info_AgentStarted       = "The SQLcompliance Agent on computer '{0}' was successfully started.";
      
      // Collection Server Service Control      
      public static string Title_StartServer   = "Start Collection Server";
      public static string Title_StopServer    = "Stop Collection Server";
      public static string Warning_StopServer  = "Warning: Stopping the Collection Server will prevent new trace information from being received " +
                                                 "in the Repository.\n\nDo you want to continue and stop the Collection Server service?";
                                               
      public static string Error_ServerNotInstalled = "Collection Server is not installed.";
      public static string Error_CantStopServer     = "An error occurred stopping the Collection Server on computer '{0}'.\n\nError: {1}";
      public static string Info_ServerStopped       = "The Collection Server on computer '{0}' was successfully stopped.";
      public static string Error_CantStartServer     = "An error occurred starting the Collection Server on computer '{0}'.\n\nError: {1}";
      public static string Info_ServerStarted       = "The Collection Server on computer '{0}' was successfully started.";
      public static string Info_ServerRestarted     = "The Collection Server on computer '{0}' was successfully restarted.";
      
      public static string Title_Activate     = "Register SQL Server";
      public static string Error_CantActivate = "The SQL Server cannot be registered with a SQLcompliance Agent at this time because a connection could not be made to the Collection Server.";
      
      
      public static string Title_UninstallAgent     = "Uninstall SQLcompliance Agent";
      public static string Error_CantUninstallAgent = "The SQLcompliance Agent cannot be uninstalled.";
                                                           
     // Miscellaneous                                                      
      public static string ErrorLabel                      = "\n\nError:\n\n";
      
      public static string Error_CantLoadHelpFile          = "Unable to display the SQLCompliance Manager help file.";
      
      public static string Title_NoLicense                 = "SQL Compliance Manager Trial License";
      public static string Info_NoLicense                  = "A 14 day trial license will be automatically " +
                                                             "generated the first time the Collection Service is started. Until either the trial license " +
                                                             "is generated or you load a purchased licensed key all auditing activities are disabled. \n\n"+
                                                             "Please contact IDERA to obtain a valid license for SQL Compliance Manager so that you may began auditing your SQL Servers.";
      public static string Title_LicenseExpired            = "SQL Compliance Manager License Expired";
      public static string Info_LicenseExpired             = "The trial license for this copy of SQL Compliance Manager has expired. You may still continue "+
                                                              "to use the product to view and report on collected audit data. However, all auditing "+
                                                              "of registered SQL Servers and their databases has been disabled. \n\n"+
                                                              "Please contact IDERA to obtain a valid license for SQL Compliance Manager so that you may resume auditing your SQL Servers.";
      public static string Title_LicenseAboutToExpire      = "SQL Compliance Manager License About to Expire";
      public static string Info_LicenseAboutToExpire       = "The trial license for this copy of SQL Compliance Manager will expire in {0} days on {1}. When the license expires, "+
                                                              "all auditing will be disabled but you will be able to continue to use the product to view and report on collected audit data.\n\n"+
                                                              "Please contact IDERA to obtain a new license for SQL Compliance Manager to prevent an interruption in the auditing of your SQL Servers and their databases.";
      public static string Info_NeverExpires = "Never";                                                              
      
      // Database Error Messages
      public static string Error_DMOLoadServers            = "An error occurred trying to load the list of SQL Servers available on your network.";
      public static string Error_CantLoadDatabase          
         = "The databases for the selected server could not be loaded. The SQLcompliance agent"  +
           " for the SQL Server is down or inaccessible and you do not have access to directly" +
           " retrieve the list of databases from the SQL Server.";

      public static string Error_CantLoadServers           = "Could not load registered servers from the Repository. Databases may not be added for this server until the problem is corrected.";
      public static string Error_CantConnectToInstance     = "Unable to connect to the SQL Server instance to load data.\n\nError:\n\n{0}";
      public static string Error_CantLoadTables            = "The tables for this database could not be loaded. The SQLcompliance agent for the SQL Server is down or inaccessible and you do not have access to directly retrieve the list of tables from the SQL Server. You may not select specific tables for auditing until the problem is resolved.";
      
      public static string Error_CantLoadRoles             = "The server roles and logins for this server could not be loaded. The SQLcompliance agent for the SQL Server is down or inaccessible and you do not have access to directly retrieve the list of logins and roles from the SQL Server. You may not specify privileged users for auditing until the problem is resolved.";
      public static string Error_ServerNotRegistered       = "The SQL Server you specified is not registered with SQL Compliance Manager. Please select a different server or exit this wizard and register the server if you wish to start auditing of databases on a new SQL server.";
      public static string Error_ServerAlreadyRegistered   = "This SQL Server instance is already registered for auditing.";
      public static string Error_DatabaseNotFound          = "The database cannot be found on the selected SQL server or is already being audited. Please select a different database.";
      public static string Error_ServerRequired            = "You must select a registered SQL Server.";
      public static string Error_DatabaseRequired          = "You must select a database to audit.";
      public static string Error_DatabaseAlreadyAudited    = "This database is already being audited. Please select another database.";
      public static string Error_NoDatabasesSelected       = "You must select at least one database to be audited.";
      public static string Error_TooManyDatabasesSelected  = "Only 100 databases may be added at a time.";
      public static string Error_MustSelectOneAuditOption   = "You must select at least one type of activity to be audited.";
      public static string Error_MustSelectOneAuditObject   = "You must select at least one type of object to be audited.";
      public static string Warning_MustSelectOneAuditOption  = "You have not select any activity types to be audited. This will limit the events gathered to privileged user auditing or database level auditing\n\nDo you wish to continue without selecting any of these activities to be audited?";
      public static string Error_MustSelectOneAuditUserOption  = "You must select at least one type of activity to be audited for privileged users.";
      public static string Error_AllDatabasesAudited       = "All databases for the selected server are already being audited.";
      public static string Warning_CaptureAll              = "Warning: Selecting to capture operation low level details can increase the amount of information gathered for this database significantly. It is recommended that this option be left off unless absolutely needed.";
      public static string Error_ErrorCreatingDatabase     = "An error occurred trying to set the database up for auditing. The database may be added after the problem is resolved.";
      public static string Error_ErrorSavingDatabase       = "An error occurred trying to save the changes to the database. The database may be modified after the problem is resolved.";
      public static string Error_CantDeleteDatabase        = "An error occurred trying to delete the database. The database may be deleted after the problem is resolved.";
      public static string Error_CantDeleteServer          = "An error occurred trying to delete the server or its associated tables in the Repository. The server may be deleted after the problem is resolved.";
      public static string Error_ErrorSavingServer         = "An error occurred trying to save the changes to the registered server. The server may be modified after the problem is resolved.";
      public static string Error_CantChangeAuditingStatus  = "An error occurred trying to changed the database auditing status. Try the change again after the problem is resolved.";
      public static string Error_NoUserTables              = "At least one user table must be selected when you choose to specify which user tables to audit. Select at least one user tables or change the option to 'Don't audit user tables' or 'Audit all user tables'";
      public static string Error_NoColumns                 = "At least one column must be selected when you choose 'Audit Selected Columns'. Select at least one column to continue.";
      public static string Error_BlobColumnsNotSupported   = "Columns containing BLOB data cannot be audited. Remove the BLOB column from the selected list.";
      public static string Error_BlobTablesNotConfigured   = "At least one table containing BLOB data has been selected for Before-After auditing, but has not been configured with a list of columns to audit. Configure all tables properly for Before-After auditing or remove the table from the list.";
      public static string Error_BADTableNotAudited        = "The table [{0}] has been selected for Before-After auditing but is not currently being audited for DML. Either add the table for DML auditing or remove it from Before-After data auditing.";
      public static string Error_DMLAuditingNotEnabled     = "Before-After auditing has been configured, but DML auditing has been disabled. Enable DML auditing to continue.";
      public static string Error_UserTableAuditingNotEnabled = "Before-After auditing has been configured, but DML auditing for user tables has been disabled. Enable DML auditing for the selected user tables to continue.";
      public static string Error_LoadingDatabaseProperties = "An error occurred trying to load the properties for the selected database.\n\nError:\n\n{0}";
      public static string Error_LoadingServerProperties   = "An error occurred trying to load the properties for the selected SQL server.\n\nError:\n\n{0}";
      public static string Error_LoadingArchiveProperties  = "An error occurred trying to load the properties for the selected archive.\n\nError:\n\n{0}";
      public static string Error_ReadingSQLcomplianceDatabases = "An error occurred reading the system database table in the Repository.";
      public static string Error_CantLoadLocalInstance     = "An error occurred connecting to the local SQL Server. Try again when the problem is resolved.";
      public static string Error_InvalidServerName         = "The specified SQL Server instance name is not in a legal format. Enter '(local)'or 'instance_name' for a local instance or 'computer\\instance' for a remote instance.";
      public static string Error_InvalidServiceAccountName = "Enter a service account name in the form of 'domain\\user'.";
      public static string Error_MismatchedPasswords       = "Password fields don't match.";
 	   public static string Error_InvalidDomainCredentials  = "The domain account credentials supplied could not be verified." ;
 	   public static string Error_NoInstallUtilLib          = "Could not verify service account password.\n\nError: {0}";
      public static string Error_InvalidTraceDirectory     = "The trace directory must be a valid local directory path on the SQLcompliance Agent Computer, may not include relative pathing, and must be 180 characters or less.";
      public static string Error_UpdateNowFailed           = "An error occurred issuing the request to update the audit settings for the selected SQLcompliance Agent."; 
      public static string Error_UpdateTraceDirectoryFailed = "An error occurred trying to change the SQLcompliance Agent trace directory. The trace directory at the agent is unchanged.";
      public static string Error_NoEventDataAvailable      = "No data available for the selected event.";
      public static string Error_CantCreateEventsDatabase  = "An error occurred creating the events database for this instance. The registration of this server cannot be done at this time.";
      public static string Error_CantReadSqlVersionNameForServer = "An error occurred during reading sql version name for this instance.";
      public static string Error_CantReadSqlPropertiesForServer = "An error occurred during reading SQL Server properties.";
      public static string Error_CantImportArchive         = "An error occurred attaching the archive database to the Repository.";
      public static string Error_CantUpdateArchive         = "An error occurred updating the archive database properties.";
      public static string Error_ArchiveDeleted            = "Could not open archive properties for this archive's underlying database: '{0}'. The usual cause for this is that the archive database has been removed from SQL Server outside of SQL Compliance Manager.";

	  public static string Warning_ScheduledArchiveRunning = "Scheduled archive is already running. Please wait for it to complete and try again after some time.";

      public static string Error_InvalidReportsServer      = "The Reporting Services computer may not be blank.";
      public static string Error_InvalidReportsFolder      = "The Reporting Services folder may not be blank.";
      
      public static string Error_SavingLogin     = "An error occurred saving the SQL Server login.";
      public static string Title_DeletingLogin   = "Delete Login";
      public static string Title_RegisteredSQLServerProperties = "Registered SQL Server Properties";
      public static string Warning_DeletingLogin = "Warning - This will delete the SQL Server login and all associated database users (if any) and their access within SQL Server. \r\n\r\nAre you sure you want to remove this login?";
      public static string Error_DeletingLogin   = "An error occurred deleting the SQL Server login.";
      public static string Error_AddRoleMember   = "An error occurred adding the SQL Server login to the server role {0}.";
      public static string Error_DropRoleMember  = "An error occurred dropping the SQL Server login from the server role {0}.";
      public static string Error_AddingLogin     = "An error occurred adding the new SQL Server login.";
      public static string Error_GrantAccessLogin = "An error occurred granting access to the new SQL Server login.";
      public static string Warning_LoginAccess   = "• Will have access to the SQL Server instance that hosts the Repository\r\n";
      public static string Warning_NoLoginAccess = "• Will not have access to the SQL Server instance that hosts the Repository\r\n";
      public static string Warning_Sysadmin      = "• Will belong to the sysadmin fixed server role\r\n";
      public static string Warning_NoSysadmin    = "• Will not belong to the sysadmin fixed server role\r\n";
      public static string Warning_WebAppAccess = "• Will have access to web application\r\n";
        public static string Warning_NoWebAppAccess = "• Will not have access to web application\r\n";
      public static string Title_ModifyLogin     = "Update Login";
      public static string Question_Continue     = "Do you want to continue?";
      public static string Message_Permissions   = "Warning - You are changing the following permission settings for {0} login:\r\n\r\n";
      public static string Message_AlreadyExists = "If a login named {0} already exists, SQLCompliance Manager will change the permissions for that login.\r\n\r\n";
      
      public static string Error_NoDatabasePermsSpecified = "You must choose at least one permission option for database {0}.";
      
      public static string Info_ArchiveServerDescription   = "Archive server - This instance is not audited by this installation of SQL Compliance Manager.";
      
      public static string Info_ArchiveUpgradeNeeded =
         "This archive database was created by an older version of " +
         "SQL Compliance Manager. The database schema must be upgraded to the " +
         "current version before it can be attached to the repository. This " +
         "upgrade process will not affect any existing event data. " +
         "\n\nDo you want to upgrade the archive database?";      
      public static string Info_ArchiveIndexUpgradeNeeded = "This archive database lacks indexes that " +
         "significantly improve the performance of the Management Console while viewing events" + 
         " in this database.  These indexes can be applied now or later through the console or " +
         "the command-line interface.  Applying indexes can be a lengthy and process-intensive " +
         "process, depending upon the size of the database.  \n\nWould you like to apply the indexes now?" ;

      public static string Info_EventsIndexUpgradeNeeded = "This database lacks indexes that " +
         "significantly improve the performance of the Management Console while viewing events" + 
         " in this database.  These indexes can be applied now or later through the console or " +
         "the command-line interface.  Applying indexes can be a lengthy and process-intensive " +
         "process, depending upon the size of the database.  \n\nWould you like to apply the indexes now?" ;

      public static string Title_ConnectToServer              = "Connect to Repository";      
      public static string Error_CantConnectToServer          = "Could not connect to the Repository on '{0}'";
      public static string Error_CantReadRepository           = "Unable to read the Repository database on the selected server. This could be due to insufficient privileges or an unsupported version of the Repository database.";
      public static string Error_UnsupportedRepositoryVersion = "This version of the Management Console is incompatible with the selected Repository database format. Contact support for upgrade information.";
      
      // Server Error Messages
      public static string Error_ErrorCreatingServer       = "An error occurred trying to register the SQL Server with SQL Compliance Manager. The server may be registered after the problem is resolved.";
      public static string Error_ErrorConvertingServer     = "An error occurred trying to start auditing of the SQL Server. The server may be registered after the problem is resolved.";
      public static string Error_ErrorWritingLicense       = "An error occurred trying to save the new license key.";
      public static string Error_InvalidLicenseKey         = "The license key entered is not a valid IDERA license key. Please enter a valid license key. If you continue to have problems, contact your IDERA sales representative.";
      public static string Error_InvalidLicenseKey_TooManyServers = "This license key may not be installed. A new license key must allow for at least the number of instances already registered with SQL Compliance Manager.  Please remove some registered servers before loading the new license key.";
      public static string Error_MissingLicenseKey         = "Current license key is invalid.";
      public static string Error_MismatchedLicenseInstance = "License key does not match the target SQL Server instance.";
      public static string Error_NoLicenseKey              = "No license key installed";
      public static string Warning_InstallTrialLicenseOverPermanent = "Warning: You are replacing a permanent license key with a trial license key. Do you want to continue?";
      
      public static string Info_NewUnexpiredLicense = "You have successfully upgraded an expired license. To resume auditing, you will need to ensure that the Collection Server service is running and reenable auditing on any registered SQL Servers.";
      
      // Server options

      public static string Error_BadTraceFileRollover        = "'Trace File Rollover Size' must be between 2 Mb and 50 Mb.";
      public static string Error_BadCollectionInterval       = "'Collection Interval' must be at least 1 min";
      public static string Error_BadForcedCollectionInterval = "'Forced Collection Interval' must be at least 1 min";
      public static string Error_BadTraceStartTimeout        = "'Trace Start Timeout' must be at least 1 second and less than the 'Collection Interval'";
      public static string Error_BadTamperDetectionInterval = "'Tamper Detection Interval' must be at least 1 second";
      public static string Error_BadHeartbeatFrequency       = "'Heartbeat Interval' must be at least 2 minutes and less than 9,999 minutes.";
      public static string Error_BadMaxFolderSize            = "'Trace Directory Size' must be at least 1 Gb";
      public static string Error_BadMaxUnattendedTime        = "'Maximum Unattended Time' must be at least 1 day";
      
      public static string Info_RolloverSizeWarning = 
         "Warning: Trace file rollover size affects the size of the trace files produced by SQL Server. This size has an impact on the amount of memory used by the Collection Server. Use care in changing this value as it may impact the performance of the Collection Server.";

      public static string Error_IllegalSnapshotPeriod = "The snapshot interval must be a value between 1 and 999.";

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        public static string Caption_AuditingViaExtendedEvents = "Enable Auditing via Extended Events";
        public static string Error_AuditingViaExtendedEvents = "Databases on this Server have Database Properties set under the \"DML/SELECT filters\" tab with user tables specified for auditing. Extended Events do not support this functionality. Either continue auditing via Traces or change your database settings to Audit all database objects or all user tables.";
        public static string Error_CantChangeSelectedObjects = "Cannot change the auditing from all objects to selected objects for 'Auditing Via Extended Events'";

        // Filter GUI strings
        public static string Error_IllegalFilterValue_Days     = "'Number of days' must be a value between 1 and 999.";
      public static string Error_IllegalFilterValue_DateRange = "The filter start date must be less then the end date.";
      public static string Filter_PrivilegedUsers            = "privileged user ";
      public static string Filter_DaysString                 = "Show {2}events from last {0:#,#} days (maximum {1:#,#} events)";
      public static string Filter_Unlimited                  = "Show up to {0:#,#} {1}events";
      public static string Filter_DateRangeString            = "Show {3}events from {0} to {1} (maximum {2:#,#} events)";

      //Event view preference strings      
      public static string Error_IllegalValue_PageSize = "'Event View Page size' must be  value between 1 and 99,999.";
      public static string Error_IllegalValue_AlertPageSize = "'Alert View Page size' must be value between 1 and 99,999.";
      
      public static string Error_LimitSQLLength = "Truncate SQL length must be greater than 0.";
      public static string Warning_ReportLimitSQLLength = "Microsoft Excel does not support more than 32767 characters in a cell. If there is a SQL Statement that contains more than 32767 characters, the report will be truncated.";
      public static string Error_InvalidThreshold = "Thresholds must be an integer value." ;
      public static string Error_ThresholdLessThanZero = "The {0} thresholds must have values greater than zero.";
      public static string Error_ThresholdErrorLessThanWarn = "The {0} error threshold cannot be less than the warning threshold.";
      public static string Error_ThresholdOverflow = "The {0} threshold is out of range.  Please specify a value between 1 and 2000000000.";
      
      // Archive options
      public static string Error_IllegalArchiveInterval = "Archive frequency must be between 1 and 999 days. To disable AutoArchive, select 'Do not AutoArchive'.";  
      public static string Error_IllegalArchiveAge      = "Age of event data to archive must be between 1 and 999";  
      public static string Error_IllegalPrefix          = @"The archive database prefix may not be empty and may not contain any of the following characters '*/:<>?\|'.";
      public static string Title_InvalidArchiveDatabase = "Invalid Archive Database";
      public static string Error_InvalidArchiveDatabase = "The selected database is not a SQL Compliance Manager archive database. Only SQL Compliance Manager archive databases may be attached to the Repository.";
      public static string Error_IncompatibleArchiveDatabase = "The selected database's schema is not supported by this version of SQL Compliance Manager and cannot be attached to the Repository.";
      public static string Info_ArchiveRequested        = "The archive process has been successfully requested."; 
      public static string Error_ArchiveNowFailed       = "An error occurred performing the archive operation."; 
      public static string Error_GroomFailed            = "An error occurred grooming the audit data for SQL Server {0}.\n\nError:\n\n{1}";
      public static string Error_GroomFailedContinue    = "\n\n\nDo you want to continue grooming audit data for other SQL Server instances?";
      public static string Error_GroomFailedAlerts         = "An error occurred grooming alerts.\r\n\r\nError:\r\n\r\n{0}";
      public static string Error_RemovingArchiveDatabase = "An error occurred trying to drop this archive database from the Repository.";
      public static string Error_IllegalGroomingAge      = "Age of event data to groom must be between 1 and 999";  
      public static string Error_IllegalAlertGroomingAge      = "Age of alert data to groom must be between 1 and 999";  
      public static string Error_ServerNotAvailable
            = "The {0} on {1} cannot be reached. The {0} service may be down or a network error is preventing the Management Console from contacting the {0}. Your request may not be processed at this time.";
            
      public static string Error_GroomAbortedAfterIntegrityCheck   = "The audit data for SQL Server {0} was not groomed. Grooming cannot be performed until you mark the problem events found during the integrity check.";
      public static string Error_ArchiveAbortedAfterIntegrityCheck = "The audit data for SQL Server {0} was not archived. Archiving cannot be performed until you mark the problem events found during the integrity check.";

	    public static string Error_ArchiveWeekdayNotSelected        = "Please select atleast one weekday for archiving.";
        public static string Error_ArchiveDBDirectoryNotProvided    = "Archive database files location not provided.";
        public static string Error_ArchiveDBDirectoryNotExists      = "Archive database files location '{0}' doesnot exists or your account does not have rights to access it.";
      
      // Report Errors
      public static string Error_ReportParameterValueTooLong = "Error: The value for parameter {0} exceeds maximum allowed length.";

	    public static string Error_InMemoryTablesNotSupported = @"In memory tables are not supported for Before-After Data auditing.";


      // Status Descriptions
      public static string Status_Enabled             = "Enabled";
      public static string Status_Disabled            = "Disabled";
      public static string Status_Pending             = "Update pending";
      public static string Status_Requested           = "Update requested";
      public static string Status_Never               = "Never";
      public static string Status_Current             = "Current";
      public static string Status_ReportingOnly       = "Not Audited";
      public static string Status_Unavailable         = "Unavailable";
      
      // MainForm Tree Labels
      public static string Tree_MgmtServer        = "SQL Compliance Manager";
      public static string Tree_Servers           = "Registered SQL Servers";
      public static string Tree_Logs              = "Activities";
      public static string Tree_Roles             = "Roles";
      public static string Tree_Alerts            = "Alerts";
      public static string Tree_AlertRules        = "Alert Rules";
      public static string Tree_EventFilters        = "Audit Event Filters" ;
      public static string Tree_Reports           = "Reports";
      public static string Tree_Activity          = "Activities";
      public static string Tree_ActivityByServer  = "Audited SQL Servers";
      public static string Tree_ActivityByGroup   = "Audited Activities (by Group)";
      //public static string Tree_PrivilegedUsers   = "Privileged User Activities";
      public static string Tree_Archives          = "Archives";
      public static string Tree_Logins            = "Logins";

      /*
      // MainForm Tree Image List
      public static int    Icon_MgmtServer      = 0;
      public static int    Icon_Alerts          = 1;
      public static int    Icon_Reports         = 2;
      public static int    Icon_Activity        = 3;
      public static int    Icon_Server          = 4;
      public static int    Icon_ServerDisabled  = 5;
      public static int    Icon_DBEnabled       = 6;
      public static int    Icon_DBDisabled      = 7;
      public static int    Icon_Roles           = 8;
      public static int    Icon_User            = 9;
      public static int    Icon_ArchiveServer   = 10;
      public static int    Icon_AlertRules      = 11;
      public static int    Icon_EventFilters    = 12;
      public static int    Icon_Archive         = 13;

      public static int    Icon_Logs            = 3;
      public static int    Icon_Logins          = 8;*/

      
      
      // Login lables
      public static string  Login_Role          = "Role";
      public static string  Login_WindowsUser   = "Windows User";
      public static string  Login_WindowsGroup  = "Windows Group";
      public static string  Login_Standard      = "Standard";
      public static string  Login_Administrator = "SQL Compliance Manager Administrator";
      public static string  Login_Auditor       = "SQL Compliance Manager Auditor";
      public static string  Login_Deny          = "Deny";
      public static string  Login_Permit        = "Permit";
      public static string  Login_ViaGroup      = "Via group membership";
      public static string  Login_CanConfigure  = "Can configure settings and view audit data";
      public static string  Login_CanView       = "Can view and report on audit data";
      public static string  Login_None          = "None";
      
      // Generic List Labels
      public static string  List_Database      = "Database";
      public static string  List_Server        = "SQL Server";
      public static string  List_DatabaseGroup = "Database Group";
      public static string  List_Archive       = "Archive";
      
      // alerts caption
      public static string  Grid_NoAlerts        = "No matching Event alerts";
      public static string Grid_NoStatusAlerts = "No matching Status alerts";
      public static string Grid_NoDataAlerts = "No matching Data alerts";

      // events caption
      public static string  Grid_ServerCaption   = "Server: {0}";
      public static string  Grid_ServerUsersCaption = "Server: {0}  Privileged user events";
      public static string  Grid_DatabaseCaption = "Server: {0}  Database: {1}";
      public static string  Grid_ArchiveCaption = "Server: {0}  Archive: {1}";
      public static string  Grid_Unloaded        = "";
      public static string  Grid_Loading         = "Loading...";
      public static string  Grid_LoadingEvents   = "  Loading Events, Please wait...";
      public static string  Grid_Loaded          = "Showing {0:#,#} of {1:#,#} events";
      public static string  Grid_Error           = "ERROR LOADING LIST";
      public static string  Grid_ErrorLabelFmt   = "Error loading list: {0}";
      public static string  Grid_NoEvents        = "No matching events";
      public static string Grid_EventsDatabaseMissing = "The events database has been deleted from SQL Server,";
      public static string Grid_UserDoesntHavePermission = "You do not have permission to view the events for this SQL Server.";
      public static string Grid_DatabaseNeedsUpdate = "This events database needs index updates.  Click here to learn more." ;
      
      public static string  List_NoArchives      = "No available archives";
      
      public static string  Prop_ErrorLoading    = "An error occurred loading the event\n\nError:\n\n{0}"; 
      public static string  Prop_NotDataRow      = "Current row is not an event. Use Next and Previous to navigate to a different row with an event."; 
      
      public static string   EventDatabaseNotCreated = "Event database not created yet";
      
      public static string   Title_AddDatabasesNow  = "Add Audited Databases";
      public static string   Prompt_AddDatabasesNow = "Do you want to add audited databases for this SQL Server now?";
      
		public const string PleaseEnterCredentials = "Please enter credentials for accessing:\r\n";
		public const string PleaseEnterWindowsCredentialsFor = "Please enter windows credentials for accessing:\r\n";
		public const string PleaseEnterAdministrativeCredentials = "Please enter your administrative credentials.";
		public const string PleaseEnterAdministrativeCredentialsFor = "Please enter administrative credentials for\r\n";
		
      

      #region GUI Preference Registry Definition

      public static string RegKeyGUI       = @"Software\Idera\SQLCM\Console";
      public static string RegKeyIderaProducts = @"Software\Idera\RegisteredProducts" ;
      public static string RegKeySQLcmTools = @"Software\Idera\SQLCM\Tools" ;
      
      public static string RegVal_RU        = "Server";
      
      public static string RegVal_ViewGroupByColumn = "ViewGroupByColumn";
      public static string RegVal_ViewCommonTasks   = "ViewCommonTasks";
      public static string RegVal_ViewConsoleTree   = "ViewConsoleTree";
      public static string RegVal_ViewToolbar       = "ViewToolbar";
      public static string RegVal_ViewBanners       = "ViewBanners";
      public static string RegVal_ViewJobsMenu      = "ViewJobsMenu";

      public static string RegKey_ChangeLogViewFilter = "ChangeLogViewFilter";
      public static string RegKey_ActivityLogViewFilter = "ActivityLogViewFilter";
      public static string RegKey_AlertViewFilter = "AlertViewFilter";
      public static string RegKey_EventViewFilter = "EventViewFilter";
      public static string RegKey_ArchiveViewFilter = "ArchiveViewFilter";
      public static string RegVal_LimitType       = "LimitType";
      public static string RegVal_nDays           = "nDays";
      public static string RegVal_StartDate       = "StartDate";
      public static string RegVal_EndDate         = "EndDate";
      public static string RegVal_PrivUsers       = "PrivUsers";
      public static string RegVal_CategoryId      = "CategoryId";
      public static string RegVal_TypeId          = "TypeId";
      public static string RegVal_AlertLevel       = "Level";
      
      public static string RegVal_LogLevel        = "LogLevel";
      public static string RegVal_EventPageSize        = "PageSize";
      public static string RegVal_ShowLocalTime   = "ShowLocalTime";

      public static string RegVal_AlertPageSize        = "AlertPageSize";
      public static string RegVal_MaxRecentAlerts       = "MaxRecentAlerts";
      
#endregion

      #region SQL Strings
      
      public static string    SQL_dbo = "dbo";
      
      #endregion

      
      public static string DatabasePhysicalNameProperty = "PhysicalName";

      public static string IderaHomePage            = @"http://www.idera.com";
      public static string SQLcomplianceHomePage    = @"http://www.idera.com/productssolutions/sqlserver/sqlcompliancemanager";
      public static string SupportHomePage          = @"https://www.idera.com/support/productsupport";
      public static string KnowledgeBaseHomePage    = @"http://www.idera.com/support/sqlsharepoint";
      public static string SQLcomplianceUpdatePage  = @"http://www.idera.com/webscripts/versioncheck?v=59000&productid=sqlcompliance";

      public static string PercentSign = "%";
      public static string Ellipsis = "...";
      public static string BlankPassword = "        ";
      public static string Yes = "Yes";
      public static string No  = "No";

      public static string SizeInBytesDisplayFormat = "###,###,###,###,###,###,##0";
      public static string DropDownAll = @"<ALL>";
      public static string UpperLocal = "(LOCAL)";

		// the min/max refresh rates for AutoRefresh of Status panel and Statistics pane
		// if you want to go greater than 60 then you need to add code around the timer intervals, because they 
		// only support 64.8 seconds top. If you go less than 5 then you slow the GUI down for no real gain
		// since Backup Service only send status every 5 seconds for scalability.
		public static int MinRefreshRate = 5;
		public static int MaxRefreshRate = 60;

		// new
		public static string PrefFileName = @"\sqlcompliance.prefs";

		// SQLcompliance default extension
		public static string SQLcomplianceExtension = ".compliance";
		
		public static string Cdrive = @"C:\";

        public static char[] SlashDelim = @"\".ToCharArray();
		public static char[] PeriodDelim = @".".ToCharArray();

      public static string ConsoleWindowName = @"SQL Compliance Manager";
		public static string GUIExecutableName = @"SQLcompliance.exe";

        public enum RestoreSubmitType
        {
            Database, 
            File
        }

		public enum TreeNodeType 
		{
			NotValid = -1,
			Root = 0, 
			Group = 1,
			Instance = 2,
			Database = 3
		}

		// button and label constants misc
		public static string StatusCanceled = "Canceled";
		public static string CancelBackup = "Cancel Backup";
		public static string CancelRestore = "Cancel Restore";

		// various messages
		public static string CancelBackupWarning = "WARNING! Before canceling, please note the following:\r\n\r\n1) If this is an append to an existing backup archive file, canceling the\r\nbackup operation may result in some wasted space in the archive file\r\nas it contains the contents of the incomplete backup.\r\n\r\n2) If this is the first backup set for that backup archive the backup file(s)\r\nare deleted.";
		public static string CancelRestoreWarning = "WARNING! Before canceling, please note the following:\r\n\r\nCanceling a restore operation will result in leaving the database\r\nin the 'restore loading' state. You will likely need to restart the\r\ndatabase recovery with a full restore.";

		//used by preference form, wizard and backup panel
		public static string EncryptNoneDescription = "Description: The backup file is not encrypted. This file can be restored without a password.";
		public static string EncryptNoneUse = "Use: Select this option if speed is your primary concern, and encryption is not required.";
		public static string EncryptDESDescription = "Description: The backup file is encrypted using Data Encryption Standard (DES). DES encrypts data in 64-bit blocks using a 64-bit key.";
		public static string EncryptDESUse = "Use: Select this option if speed is not a concern, DES encryption is required, and data security is moderate.";
		public static string Encrypt3DESDescription = "Description: The backup file is encrypted using the Triple Data Encryption Standard (3DES). The 3DES algorithm uses three successive iterations of the DES algorithm with three 64-bit keys.\r\n\r\nNote: Triple DES is slow and requires three times more processing than standard DES, but is much more secure.";
		public static string Encrypt3DESUse = "Use: Select this option if speed is not a concern, 3DES encryption is required, and data security is high.";
		public static string EncryptRC2Description = "Description: The backup file is encrypted using Rivest's Cipher 2 (RC2). RC2 is a variable key-size block cipher. The RC2 generated key is 128 bits and uses a block size of 64 bits.\r\n\r\nNote: RC2 is a quicker, more secure than DES; it requires two and three times less processing than DES, but is usually a little slower than Rijndael.";
		public static string EncryptRC2Use = "Use: Select this option if speed is a concern and strong encryption is required.";
		public static string EncryptRijDescription = "Description: The backup file is encrypted using the AES Rijndael algorithm. Rijndael is a symmetric 128-bit block data encryption. The key is 256 bits in length.\r\n\r\nNote: AES is quicker and more secure than DES and generally performs better than RC2.";
		public static string EncryptRijUse = "Use: Select this option if speed is a concern, and strong encryption is required.";



        //SQLCM 5.6 Start
        public static string BeforeAfterAuditing_DuplicateErrorMessage = "Table {0} is duplicated in the database, please remove the duplicate table from the" +
                               " database in order to proceed with auditing.";
        //SQLCM 5.6 End


		// Big hairy label follows in rtf format for preference wizard and dialog
		// it is ugly this way but temporary so just live with it for a while.
		// Mike Frank usability marketing requirement.
		// thanks, Earl
		public static string CompressionHype = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fswiss\fprq2\fcharset0 Microsoft Sans Serif;}{\f1\froman\fcharset0 Times New Roman;}{\f2\fswiss\fcharset0 Arial;}}
{\colortbl ;\red0\green0\blue0;}
{\*\generator Msftedit 5.41.15.1503;}\viewkind4\uc1\pard\sb100\sa100\cf1\f0\fs16 1. Very High Speed / Good Compression (Recommended)\cf0\par
\cf1 This option provides the fastest backup times. If maximizing execution speed and minimizing system overhead are your objectives, this is the best choice.\cf0\par
\pard\sb100\sa100\cf1 2. High Speed / Moderate Compression\cf0\par
\cf1 This option provides a higher compression rate but at the expense of execution speed. If output file size is more important than speed and you do not mind a small amount of overhead on your system, this is the best choice.\cf0\par
\pard\sb100\sa100\cf1 3. Moderate Speed / High Compression\cf0\par
\cf1 This option provides an even higher compression rate but with a further reduction in execution speed. If output file size is more important than speed and you don't mind a moderate amount of overhead on your system, this is the best choice.\cf0\par
\pard\sb100\sa100\cf1 4. Slower Speed / Very High Compression\cf0\par
\cf1 This option provides the highest compression rate but with a significantly reduced execution speed. If reduced output file size is your primary objective, this is the best choice.\cf0\par
\pard\sb100\sa100\cf1 Note: The compression rate will vary somewhat depending on the type of data you are backing up (e.g., text compresses better than binary). Execution speed can vary based on your system setup (e.g., the number of CPUs, disk configuration, network speed, overall system load).\cf0\f1\fs24\par
\pard\f2\fs20\par
}";

	    public static string GreyingLogicNotes =
	        "Note: Selected items that are disabled have been enabled at the server level. Des"
	        + "elected items that are disabled are waiting for other settings to be applied bef" + "ore you can use them.";

        public static string Error_NonCompressedFileTransfer = "Agent deployed on this server is older than version 5.8. Non-compressed events file transfer is supported with agent version 5.8 and above. Either continue auditing with compressed events files or upgrade the agent to latest version to audit with non-compressed events files.";
        public static string Caption_NonCompressedFileTransfer = "Enable Non-Compressed File Transfer";
	}
}
