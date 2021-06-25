using System;


namespace Idera.SQLcompliance.Core
{
   /// <summary>
   /// Summary description for CoreConstants.
   /// </summary>
   public class CoreConstants
   {
	   
      #region Private constructor (so class does not get instantiated)

      private CoreConstants() {}

      #endregion

      #region License keys (external products)

      // Xceed component license keys
      public const string Xceed_LicenseKey_Zip         = "ZIN21GKKWZN4T1N44AA";
      public const string Xceed_LicenseKey_Compression = "SCN10DKKPZTUTWAU4AA";
      //public const string Xceed_LicenseKey_Grid        = "GRD23GTKPZX4T1AU4AA";  2.3 license key
      public const string Xceed_LicenseKey_Grid        = "GRD25-XK8BR-77GF5-U4AA";

      #endregion

      #region Version Definitions
      public const string  AgentInterface                 = "1.0.0.0";
      public const string  AgentVersion                   = "1.00.00";
      public const string  AgentBuild                     = "1234";
      public const string  ServerInterface                = "1.0.0.0";
      public const string  ServerVersion                  = "1.00.00";
      public const string  ServerBuild                    = "1234";
      public const string  GUIInterface                   = "0.0.0.0";
      public const string  GUIVersion                     = "0.00.00";
      public const string  GUIBuild                       = "2";
      #endregion

      #region Install stuff

      // TODO:  Needs to be configurable	
      public const string AgentServiceMSIInstalledName          = "Idera SQLcompliance Agent";
      public const string AgentServicexMSIInstalledName_x64 = "Idera SQLcompliance Agent (x64)";
      public const string AgentServicexMSIInstalledName_ia64 = "Idera SQLcompliance Agent (ia64)";//TODO: need to be verified

      public const string AgentServiceInstallerFilename_Win32         = "SQLcomplianceAgent.msi";
      public const string AgentServiceInstallerFilename_x64 = "SQLcomplianceAgent-x64.msi";
      public const string AgentServiceInstallerFilename_ia64 = "SQLcomplianceAgent-ia64.msi";
      public const string AgentServiceInstallerRemoteShare = @"\ADMIN$\";
      public const string AgentServicex64PreInstallerFilename = "ISRuntime.exe";

      public const string AgentServiceInstallerMsiexecInstall   = "msiexec /qn /i ";		
      public const string AgentServiceInstallerMsiexecUninstall = "msiexec /qn /x ";
      public const string AgentServicex64PreInstallerCommand = "/S /v/qn";

      public const string msiInstall = "msiexec /i {0} " +
         "COLLECT_SERVER={1} " +
         "INSTANCE={2} " +
         "TRACE_DIRECTORY={3} " +
         "SERVICEACCOUNT={4} " +
         "SERVICEPASSWORD={5} " +
         "/qb-";
      public const string msiUninstall = "msiexec /x {0} /qb-";
      
      public const string AgentServiceInstallerMsiexecUpgradePrefix = "msiexec /qn /i ";	
      public const string AgentServiceInstallerMsiexecUpgradeSuffix = " REINSTALLMODE=voums REINSTALL=ALL";
      public const string AGENT_SERVICE_WINDOWS_INSTALLER_REINSTALL_PROPERTIES = "REINSTALLMODE=vamus REINSTALL=ALL";
		
      public const string DefaultInstallDirectory = @"C:\Program Files\Idera\SQLcompliance";
      public const string DefaultAgentTraceDirectory = DefaultInstallDirectory + @"\AgentTraceFiles";
      public const string DefaultLMUtilDirectory = DefaultInstallDirectory + @"\LMUtil";
      
      
      #endregion

      #region CLI

      public const string CLI_Name = "SQLcmCmd" ;

      #endregion
      
      #region SQLcompliance Agent service

      public const string ClusterAgentUpgradeFilename = "UpgradeInfo.cmp";

      // Remoting Defaults
      public const int      AgentServerTcpPort           = 5200;
      public const string   AgentServerName              = "SQLcomplianceAgentServer";
      public const string   AgentClientName              = "SQLcomplianceAgentClient";
		
      // Service Defaults
      public const string   AgentServiceName             = "SQLcomplianceAgent";
      public const string   AgentServiceDisplayName      = "SQLcompliance Agent";

      // Event Log		
      public const string   EventLogSource_AgentService  = AgentServiceDisplayName;

      public const string   AgentCoreWellKnownName       = "SQLcomplianceAgentCore";
		
      // Registry Settings
      public const string   Agent_RegKey                     = @"Software\Idera\SQLcompliance\SQLcomplianceAgent";
      public const string   Agent_RegVal_Server              = "Server";
      public const string   Agent_RegVal_ServerPort          = "ServerPort";
      public const string   Agent_RegVal_AgentPort           = "AgentPort";
      public const string   Agent_RegVal_AuditingEnabled     = "IsAuditEnabled";
      public const string   Agent_RegVal_LogLevel            = "LogLevel";
      public const string   Agent_RegVal_Instances           = "Instances";
      public const string   Agent_RegVal_HeartbeatInterval   = "HeartbeatInterval";
      public const string   Agent_RegVal_CollectionInterval  = "CollectionInterval";
      public const string   Agent_RegVal_DetectionInterval   = "DetectionInterval";
      public const string   Agent_RegVal_MaxTraceSize        = "MaxTraceSize";
      public const string   Agent_RegVal_TraceOptions        = "TraceOptions";
      public const string   Agent_RegVal_TraceDirectory      = "TraceDirectory";
      public const string   Agent_RegVal_LastCollectionTime  = "LastCollectionTime";
      public const string   Agent_RegVal_StartupSPTraces     = Agent_StartupSPTraces;
      public const string   Agent_RegVal_AgentTraces         = Agent_AgentStartedTraces;

      public const string   Agent_RegVal_StoppedTraces       = Agent_StoppedTraces;
      public const string   Agent_RegVal_ForceCollectionInterval = "ForceCollectionInterval";
      public const string   Agent_RegVal_SQLVersion          = "SQLVersion";
      public const string   Agent_RegVal_MaxUnattendedTime   = "MaxUnattendedTime";
      public const string   Agent_RegVal_MaxFolderSize       = "MaxFolderSize";
      public const string   Agent_RegVal_StopTracesOnShutDown = "StopTracesOnShutdown";
      public const string   Agent_RegVal_EnumerateUsersInterval = "EnumerateUsersInterval";
      public const string   Agent_RegVal_SkipAdminCheck         = "SkipAdminCheck";
      public const string   Agent_RegVal_FileTransferPageSize = "PageSize";
      public const string   Agent_RegVal_UseClientActivatedFileTransfer = "UseClientActivatedFileTransfer";
      public const string   Agent_RegVal_IPAddressBindType = "IPAddressBindType";
      public const string   Agent_RegVal_AssemblyRootDirectory = "AssemblyRootDirectory";

      // Agent Status Type Descriptions
      public const string  AgentStatus_Unknown   = "Undefined status record";
      public const string  AgentStatus_Heartbeat = "Heartbeat received";
      public const string  AgentStatus_Startup   = "Agent service started";
      public const string  AgentStatus_Shutdown  = "Agent service stopped";
      public const string  AgentStatus_Error     = "Agent error occurred";
      public const string  AgentStatus_Warning   = "Agent warning";
	
      // General Agent Configuration		
      public const string   Agent_Default_Server	                 = "(local)";
      public const string   Agent_Default_AuditingEnabled          = "True";
      public const int      Agent_Default_LogLevel                 = 1;
      public const int      Agent_Default_MaxTraceSize             = 5;
      public const int      Agent_Default_TraceOptions             = 2;   // Rollover
      
      public const int      Agent_Default_HeartbeatInterval        = 5;   // minutes
      public const int      Agent_Default_CollectInterval          = 2;   // minutes
      public const int      Agent_Default_ForceCollectionInterval  = 6;   // minutes
      public const int      Agent_Default_MaxUnattendedTime        = 7;  // in days
      public const int      Agent_Default_MaxFolderSize            = 2;  // in GB
      public const int      Agent_Default_EnumerateUsersInterval   = 360; // minutes
      public const int      Agent_Default_TamperingDetectionInterval = 60; // seconds
      public const int      Agent_Default_TraceStartTimeout        = 30; // seconds
      public const int      Agent_Default_FileTransferPageSize     = 5242880; // bytes

      // Types of trace information used by storing and retrieving TraceInfo
      public const string   Agent_StartupSPTraces     = "StartupSPTraces";  // SQL Server startup SP started traces
      public const string   Agent_AgentStartedTraces  = "AgentTraces";      // SQLcompliance agent started traces
      public const string   Agent_StoppedTraces       = "StoppedTraces";    // Stopped traces
      public const string   Agent_SQLcomplianceTraces = "SQLcomplianceTraces";  // SQLcompliance internal traces
      public const string   Agent_SavedTraces         = "SavedTraces";      // Traces saved for further processing

      // Agent Trace constants
      public const string   Agent_Default_TraceDirectory     = "AgentTraceFiles";
      public const string   Agent_ServerTracePrefix          = "SQLcomplianceS";
      public const string   Agent_DBTracePrefix              = "SQLcomplianceD";
      public const string   Agent_ObjectTracePrefix          = "SQLcomplianceO";
      public const string   Agent_TableTracePrefix           = "SQLcomplianceT";
      public const string   Agent_UserTracePrefix            = "SQLcomplianceU";
      public const string   Agent_DDLInfix                   = "D";
      public const string   Agent_DMLInfix                   = "M";
      public const string   Agent_SELECTInfix                = "S";
      public const string   Agent_TextDataInfix              = "T";

      // Stored procedure constatns
      public const string   Agent_AuditSPName         = "sp_SQLcompliance_Audit";
        //5.4_4.1.1 Extended Event
      public const string Agent_AuditSPNameXE = "sp_SQLcompliance_AuditXE";
      public const string Agent_Full_AuditSPNameXE = "master.dbo." + Agent_AuditSPNameXE;
      public const string   Agent_Full_AuditSPName = "master.dbo." + Agent_AuditSPName;
      public const string Agent_StartUpSPName = "sp_SQLcompliance_StartUp";
      public const string   Agent_ProcOptionSP        = "sp_procoption";
      public const string   Agent_ProcOption_Startup  = "startup";

      // Agent internal version number 
      public const int      Agent_InternalVersion            =  100;


      // Agent status cache
      public const string   Agent_StatusCache_FileName = "SQLcmStatus.cache";
      
      // Before/After data change
      public const string Agent_BeforeAfter_SchemaName = "SQLcompliance_Data_Change";
      public const string Agent_BeforeAfter_TableName = "SQLcompliance_Changed_Data_Table";
      
      #endregion
		
      #region Log Record Descriptions
		
      public const string  Log_ServerStarting   = "Collection Server started";
      public const string  Log_ServerStopping   = "Collection Server stopped";
      public const string  Log_ServerNew        = "Added server registration";
      public const string  Log_ServerDelete     = "Delete server registration";
      public const string  Log_ServerEnabled    = "Enabled auditing of server";
      public const string  Log_ServerDisable    = "Disabled auditing of server";
      public const string  Log_DatabaseNew      = "Added audited database";
      public const string  Log_DatabaseDelete   = "Delete audited database";
      public const string  Log_DatabaseEnabled  = "Enabled auditing of database";
      public const string  Log_DatabaseDisable  = "Disabled auditing of database";
      
      #endregion
		
      #region SQLcompliance Collection service

      // TCP port that the SQLcompliance Collection Server listens to
      public const int      CollectionServerTcpPort           = 5201;
      public const string   CollectionClientName              = "CollectionServiceClient";
      public const string   CollectionServerName              = "CollectionServiceServer";
      public const string   TraceCollectorName                = "TraceCollector";
		
      public const string   CollectionServiceName             = "SQLcomplianceCollectionService";
      public const string   CollectionServiceDisplayName      = "SQLcompliance Collection Service";
		
      public const string   EventLogSource_CollectionService  = CollectionServiceDisplayName;
      public const string   EventLogSource_Alerting  = "SQLcompliance Alerting" ;
      public const string   CollectionServiceMSIInstalledName = "Idera SQLcompliance Collection Service";
		
      public const string   CollectionServiceWellKnownName    = "CollectionService";
      public const string   RemoteStatusLoggerWellKnownName   = "RemoteStatusLogger";
      
      // TODO: Check out intervals for correctness
      public const int      CollectionServicePollInterval          = 30000; // milliseconds
      public const int      CollectionService_SqlReconnectInterval = 300000; // 5 minutes (5*60*1000)
      public const int      CollectionService_AlertJobPoolRetryInterval = 900000; // 15 minutes (15*60*1000)
      public const int      CollectionService_DefaultAlertingMaxEventsToProcess = 5000;
      public const int      CollectionService_DefaultSqlCommandTimeout = 300; // seconds
      
      //public const string   CollectionService_TraceDirectory  = "RepositoryTraceFiles";
      
      // Registry Settings
      public const string SQLcompliance_RegKey = @"Software\Idera\SQLcompliance";
      public const string SQLcompliance_RegKey_Path = "Path";
      public const string SQLcompliance_RegKey_Version = "Version";

      public const string   CollectionService_RegKey                   = @"Software\Idera\SQLcompliance\CollectionService";
      public const string   CollectionService_RegVal_ServerPort        = "ServerPort";
      public const string   CollectionService_RegVal_AgentPort         = "AgentPort";
      public const string   CollectionService_RegVal_ServerInstance    = "ServerInstance";
      public const string   CollectionService_RegVal_LogLevel          = "LogLevel";
      public const string   CollectionService_RegVal_TraceDir          = "TraceDirectory";
      public const string   CollectionService_RegVal_JobPoolThreads    = "JobPoolThreads";
      public const string   CollectionService_RegVal_ActivityLogLevel  = "ActivityLogLevel";
      public const string   CollectionService_RegVal_SqlCommandTimeout = "SqlCommandTimeout";
      public const string   CollectionService_RegVal_AllowCaptureSql   = "AllowCaptureSql" ;
      public const string   CollectionService_RegVal_JobsLogLevel      = "JobsLogLevel";
      public const string   CollectionService_RegVal_LogSQLErrors      = "LogParsingErrors";

      public const string   CollectionService_RegVal_FilterAgentEvents  = "FilterAgentEvents";
      public const string   CollectionService_RegVal_FilterGUIEvents    = "FilterGUIEvents";
      public const string   CollectionService_RegVal_FilterServerEvents = "FilterServerEvents";

      public const string   CollectionService_RegVal_AlertingMaxEventsToProcess = "AlertingMaxEventsToProcess";

      public const string   CollectionService_RegVal_DontDeleteNonDMLTraces = "DontDeleteNonDMLTraces";
      public const string   CollectionService_RegVal_DontDeleteDMLTraces    = "DontDeleteDMLTraces";

      public const string   CollectionService_RegVal_LogFilteredOutEvents = "LogFilteredOutEvents";
      public const string   CollectionService_RegVal_SkipAdminCheck       = "SkipAdminCheck";

      public const string   CollectionService_RegVal_OptimizeRules       = "OptimizeRules";
      
      public const string   CollectionService_RegVal_ArchiveBatchSize    = "ArchiveBatchSize";
      public const string   CollectionService_RegVal_GroomBatchSize      = "GroomBatchSize";
      
      public const string   CollectionService_RegVal_DaysStatsCached     = "DaysStatsCached";
      public const string   CollectionService_RegVal_ParseForUpdateStats = "ParseForUpdateStats";

      public const string   CollectionService_RegVal_LinkDataChange      = "LinkDataChange";

      public const string   CollectionService_RegVal_CheckEventIndexes = "CheckEventDBIndexes";

       public const string CollectionService_RegVal_IsRepositoryPreserved = "IsRepositoryPreserved";
		
      // Configuration Defaults
      public const int      DefaultMaxSqlLength                       = 128;
      public const int      DefaultDaysStatsCached                    = 1;
      
      // Constants
      public static readonly DateTime InvalidDateTimeValue = new DateTime(1900, 1, 1);
		
      #endregion
      
      #region Management Console
      public const string   ManagementConsoleName              = "SQLcompliance Management Console";
      public const string   EventLogSource_ManagementConsole  = ManagementConsoleName;
      
      public const string GUIClientName = "GUIClient";
      #endregion
		
      #region Licensing

      // SQL compliance manager License
      public const string LicenseCaption = "SQL compliance manager License";
      public const string LicenseInvalid = "License {0} is invalid";
      public const string LicenseCantCoverAllInstances = "The number of registered servers is more than the available licenses";
      public const string CantAddTrialToPermamentLicense = "Can't add a trial license to a production license";
      public const string LicenseInvalidRepository = "This license is invalid for current repository {0}";
      public const string LicenseInvalidProductID = "This license is invalid for SQL compliance manager";
      public const string LicenseInvalidProductVersion = "The license key is for an older version of SQL Compliance manager. Please visit the customer portal to acquire a new license key at http://www.idera.com/licensing.";
      public const string LicenseExpired = "This license has expired";
      public const string LicenseInvalidDuplicate = "This license has already been registered";
      public const string LicenseExpiringDays = "The license {0} will expire in {1} days";
      public const string LicenseExpiringDay = "The license {0} will expire in 1 day";
      public const string LicenseExpiringToday = "The license {0} will expire today";
      public const string LicenseConvertExpiring = "The trial license {0} will expire in {1} days.\n\nPlease contact Idera to obtain an updated license for SQL compliance manager.";
      public const string LicenseNoValidLicense = "SQL compliance manager is not licensed. You must have a valid license to run SQL compliance manager."
                                                + "\n\nPlease contact Idera to obtain a valid license for SQL compliance manager.";
      public const string LicenseTooManyRegisteredServers = "The number of registered servers is more than the available licenses."
                                                    + "\nNo more audited data will be collected until more licenses are added or a registered server is deleted."
                                                    + "\n\nPlease contact Idera to obtain more license for SQL compliance manager or remove a registered server.";
      public const string LicenseInterestText = "Thank you for your interest in SQL compliance manager.";
      public const string LicenseEnterProductionText = "Please enter your production license";
      public const string LicenseTrialExpiredText = "SQL compliance manager requires a valid license. The trial license has expired, please enter your production license";
      public const string LicenseUnsupportedConfigText = "This configuration requires a production license. To use a Trial license you must run the Console on the same server as the repository.";
      public const string DeleteLicenseCaption = "Delete SQL compliance manager License";
      public const string DeleteConfirmMsg = "Do you wish to delete the SQL compliance manager license?";
      
      internal const string LicenseInstancePrefix     = "serverinstance=";
      internal const string LicenseSeparator         =  "&";
      internal const string ProductSeparator         = ".";
      public const string ProductName                = "SQLcompliance";
      public const string ProductName_Enterprise     = "SQLcompliance_Enterprise";
      internal const string RegistryHiveLicensing    = "IDEBT362";
      internal const string RegistryKeyLicensingData = "bdata" ;
      internal const int RegistryHiveLicensingStart  = 801;
      internal const int RegistryHiveLicensingConversion = 802;
      internal const int ProductExpirationDaysLimit  = 14;
      internal const int LicenseConversionDaysLimit = 45;
      internal const int ProductTrialNumberSqlServerInstances = 10;
      public const int ProductID = 1400 ;
      public const string LmProductVersion                = "5.4";
		
      // Trial Status Strings
      public static string TrialExpired                 = "Trial expired.  Contact your Idera sales representative for assistance.";
      public static string TrialExpiredInGrace          = "Trial expired - Product will continue to work for {0} days. Contact your Idera sales representative for assistance.";
      public static string TrialValid                   = "Days left in trial: {0}";
      

      #endregion	

      #region SQLDMO

      public const string SQLDMO_Property_DatabaseSize = "Size";
      public const int VersionBuild_SQL70SP3 = 961;
      public const int VersionBuild_SQL2000SP3 = 760;
      public const string VersionString_MSDE70 = "MSDE";
      public const string VersionString_MSDE2000 = "Desktop Engine";
      public const string OSVersionNT = " NT ";
      public const string OSVersionNonNT = " 8.1 Pro ";

      public const int SqlServerVersionMajor_2000 = 8;
      public const int SqlServerVersionMajor_7 = 7;

      #endregion

      #region Event Ids

      // Use event ids in the range of 29000-29999 for SQLcompliance
      // Event ID 29000 is the "generic" id for all SQLcompliance events if an id is not specified.
      internal const int EventId_SQLcompliance = 29000;

      #endregion

      #region Informational messages and exception strings

      // Startup Problems
      internal const string Exception_ServerStartup = "The SQL compliance Manager Collection Server service is shutting down because of problems found during service startup.\n";
      internal const string Exception_SQLServerStartup = "The SQL compliance Manager Collection Server service cannot connect to the SQL Server during service startup.\n"
                                                       + "  Trace collection and processing will resume after the connection is established.";
      internal const string Exception_MainDatabaseStartup = "The SQL compliance Manager Collection Server service cannot connect to the main repository database during service startup.\n"
                                                       + "  Trace collection and processing will resume after the database is available.";
      internal const string Exception_AgentStartup = "The Sqlcompliance Agent service is shutting down because of problems found during service startup.\n";
      public const string Exception_ServerAlreadyRunning = "Another instance of the Collection Server is already running. Only one instance may be run at a time. Aborting service startup.";
      public const string Exception_AgentAlreadyRunning  = "Another instance of the SQLcompliance Agent is already running. Only one instance may be run at a time. Aborting service startup.";

      public const string Exception_x64PreInstallerExitCode = "x64 PreInstaller exited with return code of {0}.";
      
      // server delete
      internal const string Exception_ServerDeleted = "The SQL Server {0} is no longer registered for auditing with SQL compliance manager. The requested operation cannot be performed.";
      
      // trace job - security
      internal const string Exception_CantReadTraceFile = 
         "The Collection Server is unable to read the trace file to process its events. This is usually a result of the SQL Server service account having insufficient privileges to read the files in the Collection Server trace directory. Trace file: {0}";

      // Informational messages
      internal const string Info_InstallingBackupService = "Backup service not installed on remote server; installing backup service...";	

      internal const string Info_TrialLicenseWillExpire = "This trial license is about to expire.\nIt will expire in {0} days on {1}.";
      internal const string Info_TrialLicenseHasExpired = "This trial license has expired.  Please contact Idera for assistance.";
		
      internal const string Info_DisabledByLicenseCheck = "Server auditing disabled because of expired license.";


      // Warnings
      internal const string Warning_DisablingSecurityChannels = "In demo mode; disabling remoting security between client and backup service";
      internal const string Warning_TraceStopped = "SQLcompliance Manager trace is stopped unexpectedly.  {0}.";
      internal const string Warning_TraceClosed = "SQLcompliance Manager trace is closed unexpectedly.  {0}.";
      internal const string Warning_TraceAltered = "SQLcompliance Manager trace is altered unexpectedly.  {0}.";

      // For error logs
      public const string ErrorPrefix     = "Error: ";
      public const string ExceptionPrefix = "Exception: ";
      public const string DetailsPrefix   = "Details: ";

      // Exception strings
      public const string Exception_MissingSQLcomplianceDatabase = 
         "\n\nSQL compliance manager is missing one or both of its internal databases: SQLcompliance and SQLcomplianceProcessing. "+
         "The Collection Server service startup cannot continue and has been aborted.\n\n";
      
      public const string Exception_InvalidUsername = "Username is invalid";
      public const string Exception_InvalidPassword = "Password is invalid";
      public const string Exception_CollectionServiceNotAvailable = "Collection service not available on {0}; caching status events locally until the service is available\n\nError:\n\n{1}";
      public const string Exception_CollectionServiceSecurityChannel = "Cannot establish secure client connection to collection service on {0}.  Management service should be restarted to reinitialize security channels.";
      public const string Exception_InvalidCurrentSimultaneousLoggerCount = "Critical error in ManagementServer; Number of simultaneous status loggers is less than zero. The Collection Service should be restarted.";
      public const string Exception_SqlCommand = "SQL command was not successful";
      public const string Exception_CouldntReadServerRecord = "An error occurred trying to read information for a registered server.\n\nError:\n\n{0}";
      public const string Exception_CouldntUpdateServerRecord = "An error occurred trying to update the information for a registered server.\n\nError:\n\n{0}";
      public const string Exception_CouldNotImpersonateUser = "Could not impersonate user; confirm that the user account running the SQLcompliance Agent has the 'Impersonate a client after authentication' privilege, and, if on Windows 2000 or earlier, the 'Act as part of the operating system' privilege.";
      public const string Exception_ErrorCollectingTrace = "An error occurred collecting SQL Server traces";
      public const string Exception_InvalidTraceFileNameFormat = "The trace file name format is Invalid : {0}";
      public const string Exception_ServerMissingEventsDatabase = "Registered SQL Server is missing an entry for its events database in the repository";
      public const string Exception_CantAccessJobsTable = "An error occurred trying to read processing jobs from the repository.";
      public const string Exception_CantLoadTraceFile = "An error occurred loading the events in trace file from '{0}'. SQL compliance manager will automatically retry later.";
      public const string Exception_ErrorUpdatingServerTable = "An error occurred updating the servers table.";
      public const string Exception_CantCreateEventsDatabase = "An error occurred creating an events database.";
      public const string Exception_CantCreateEventsTable    = "An error occurred creating the events table.";
      public const string Exception_CantCreateEventsSQLTable = "An error occurred creating the events SQL table.";
      public const string Exception_CantDeleteEventsDatabase = "An error occurred deleting an events database.";
      public const string Exception_TraceJobHung = "An error occured requiring the event processing job pool to be shut down. A trace job has hung for over 2 hours. This is preventing the event processing engine from restarting. Shutting down the Collection Server.";
      public const string Exception_ErrorWritingLogRecord = "Unable to write activity log record.";
      public const string Exception_ErrorWritingAgentLogRecord = "Unable to write SQLcompliance Agent activity log record.";
      public const string Exception_ErrorLoadingPreferences = "An error occurred reading the Collection Server settings from the registry.";
      public const string Exception_ErrorReceivingTrace = "An error occurred receiving new trace files.";
      public const string Exception_CantReadRepository           = "Unable to read the repository database. This may be due to insufficient privileges of the service account or an unsupported version of the repository database format.";
      public const string Exception_UnsupportedRepositoryVersion = "This version of the Collection Server is incompatible with the repository database format. Contact support for upgrade information.";
      public const string Exception_IncompatibleAgentRepositoryVersion = "This version of the SQLcompliance Agent is incompatible with the repository database format. Contact support for upgrade information.";
      public const string Exception_ErrorGettingTraceEventsAndFilters = "An error occurred retrieving trace events and filters from audit configuration file {0}.  Event filtering is turned off for this trace.";
      public const string Exception_NoInstanceData= "No repository data found for requested instance";
      public const string Exception_ServerShuttingDown = "Request refused - Collection Server service is in the process of shutting down";
      
      public const string Exception_InstanceNotRegisteredAtAgent  = "An error occurred executing the requested operation. The SQL Server instance '{0}' is not registered with the SQLcompliance Agent.";
      public const string Exception_AgentCannotConnect = "The SQLcompliance Agent cannot connect to the audited SQL Server instance {0}. The Agent will be unable to gather audit data until the problem is corrected.";
      public const string Exception_TraceTamperingDetectionError = "An error occurred during trace tampering detection.";
      
      
      public const string Exception_ServiceAccountNotSysadmin =
         "\n\nThe service account does not have sufficient privileges "+
         "on the SQL Server hosting the Repository. The service will not start "+
         "without sufficient permissions and has aborted the service startup.\n\n"+
         "Required SQL Server Permissions:\n"+
         "The service account must be a a member of Systems Administrators.\n\n";
         
      public const string Exception_ServiceAccountNotLocalAdmin =         
         "\n\nThe service account does not have sufficient privileges "+
         "on this computer. The service will not start "+
         "without sufficient permissions and has aborted the service startup.\n\n"+
         "Required Permissions:\n"+
         "The service account must be a member of BUILTIN\\Administrators.\n\n";

      public const string Exception_UserDoesNotHavePermissionOnThisSP = "User does not have permissions to execute this stored procedure.";
      
      public const string Exception_RejectedAgentRequest = "SQLcompliance Agent request rejected - Instance {0} not registered with SQL compliance manager.";

      public const string Exception_CantConnectToRepository = "An error occurred trying to connect to the repository.";
      public const string Exception_CantCloseConnectionToRepository = "An error occurred trying to close the connection to the repository.";
      public const string Exception_CantWriteFinalTrace     = "An error occurred copying events in the final events tables.";
      public const string Exception_ErrorWritingEventRecord     = "Event write error - Server: '{0}' File: '{1}' Time: '{2}' Checksum: {3}";
      public const string Exception_ErrorWritingEventRecordSQL     = "Event write error - Server: '{0}' File: '{1}' Time: '{2}' Checksum: {3}\nSQL:{4}";
      public const string Exception_UnrecoverableProcessingError = "An unrecoverable error occurred processing the trace file: '{0}'. SQL compliance manager cannot process this file to load any auditing activity in the file.";
      public const string Exception_ProcessingError              = "An error occurred processing the trace file: '{0}'. SQL compliance manager cannot process this file to load any auditing activity in the file. It will retry later.";
      public const string Exception_ErrorPersistingAuditConfiguration = "An error occurred persisting the audit configurations.";
      public const string Exception_ErrorReadingAuditConfiguratioin = "An error occurred reading the audit configurations.";
      public const string Exception_NoConfiguredSQLServerInstance = "No registered SQL Server or the instance configuration is missing.  Agent will start auditing after audit settings is updated.";
      public const string Exception_ErrorReadingAgentRegistry = "Error reading the agent registry settings.";
      public const string Exception_ErrorReadingTraceInfoFromRegistry = "Unable to read the trace information from registry";
      public const string Exception_ErrorRecreatingStoredProcedures = "Unable to recreate SQLcompliance stored procedures.";
      
      public const string Exception_ErrorJobPoolThreadDown = "Error: Trace processing job thread is down - attempting to restart";
      
      // Exception messages need formating
      //TODO: Change the name of these constants so we know they need formating
      public const string Exception_Format_UnrecoverableProcessingError = "An unrecoverable error occurred processing the trace file: {0}. SQL compliance manager cannot process this file to load any auditing activity in the file.";
      public const string Exception_Format_ErrorCreatingTraceCollector = "An error occurred creating the trace collector for {0}.";
      public const string Exception_ErrorProcessingTraceFile = "An error occurred processing the trace file: {0}. This will be automatically retried later.";
      public const string Exception_CantReadAgentConfiguration = "Error reading agent configuration from registry - agent startup aborting\n\nError:\n\n{0}";
      public const string Exception_ErrorSendingHeartbeat = "An error occurred trying to send the heartbeat message to the collection server.";
      public const string Exception_RepositoryNotAvailable = "The repository is not available or connection cannot be made.\n\nError:\n\n{0}";
      public const string Exception_CouldntCreateServerRecord = "An error occurred adding an unregistered server to the repository.\n\nError:\n\n{0}";
      public const string Exception_Format_CouldntReadServerRecord = "An error occurred reading information for registered server {0}.\n\nError:\n\n{1}";
      public const string Exception_Format_CouldntUpdateServerRecord = "An error occurred updating the information for registered server {0}.\n\nError:\n\n{1}";
      public const string Exception_Format_ErrorWritingStreamToDisk = "An error occurred writing to the file : {0}";
      public const string Exception_Format_CouldntReadServerConfigVersion = "An error occurred reading information for registered server configuration version {0}.";
      public const string Exception_Format_InvalidAuditCategory = "The specified audit category value ({0}) is invalid.";
      public const string Exception_Format_InvalidTypeForTheOperation = "Type {0} is not valid for the operation operation. {0} type is expected.";
      public const string Exception_Format_InvalidTraceEventID = "Invalid trace event ID: {0}";
      public const string Exception_Format_InvalidTraceCategory = "Invalid trace category: {0}";
      public const string Exception_ErrorCreatingAuditStoredProcedure = "An error occurred creating audit stored procedure.";
      public const string Exception_ErrorCreatingAuditStoredProcedureCopySaved = @"An error occurred creating audit stored procedure.  A copy of the script is saved in {0}.";
      public const string Exception_ErrorCreatingStartupStoredProcedureCopySaved = @"An error occurred creating startup stored procedure.  A copy of the script is saved in {0}.";
      public const string Exception_ErrorCreatingStartupStoredProcedure = "An error occurred creating startup stored procedure.";
      public const string Exception_ErrorSettingSPPermissions = "An error occured setting stored procedure permissions.  Name = {0}.";
      public const string Exception_TraceDirectoryNameTooLong = "The path is too long after being fully qualified. Make sure path is less than 180 characters.";
      public const string Exception_FailToInitializeRemotingChannel = "Fail to initialize remoting channel. {0}";

      // Collection Service Exceptions
      public const string Exception_CannotReadCollectorTraceDirectory = "An error occurred reading the Collection Service trace directory";
      public const string Exception_RemoteFileAlreadyExists = "The file already exists on the remote server";
      public const string Exception_Format_ErrorSendingFileToServer = "An error occurred sending {0} to the collection server";
      public const string Exception_Format_NetworkErrorSendingFileToServer = "An network error occurred sending files to the collection server.  Error: {0}";
      public const string Exception_ErrorLoadingConfigurationForInstance = "An error occurred reading the audit configuration from the repository server: ";
      public const string Exception_Format_ErrorLoadingDBAuditConfigurationsFromDatabase = "An error occurred reading database audit configurations for {0}";
      public const string Exception_MissingAuditConfigurationFile = "The matching audit configuration file for the trace file is missing.  Filename: ";
      public const string Exception_FileAlreadyExistsOnTheServer = "The file already exists on the server.  Filename: {0}.";
      public const string Exception_ErrorGettingSQLcomplianceDBIds = "An error occurred retrieving SQLcompliance database IDs for {0}.";
      public const string Exception_ErrorLoadingUserConfigurationForInstance = "An error occurred reading the privileged user audit configuration for {0} from the repository server. ";
      public const string Exception_Format_ErrorLoadingUserAuditConfigurationsFromDatabase = "An error occurred reading user audit configurations from database for {0}";
      public const string Exception_Format_ErrorLoadingUserAuditConfigurationsFromDataReader = "An error occurred reading user audit configurations record for {0}";
      public const string Exception_CannotReadSystemDatabaseTable = "An error occurred when reading the SystemDatabases table. ";
      public const string Exception_ErrorReadingAuditedUserTables = "An error occurred when reading audited user tables from {0}.  Database ID: {1}";
      
      public const string Exception_AgentServiceNotInstalled = "SQLcompliance Agent not installed on server";
      public const string Exception_AgentServiceInstalled = "SQLcompliance Agent is already installed on server";
      public const string Exception_ErrorWrongMsiVersion = "Windows Installer 2.0 or higher is required to install the SQLcompliance Agent.";
      public const string Exception_ErrorRebootRequired = "The target machine needs a reboot to complete the SQLcompliance Agent installattion.";
      public const string Exception_ErrorInstallingAgentService = "SQLcompliance Agent could not be installed on remote server.  Check the Event Log on the remote server for more information.";
      public const string Exception_ErrorUpgradingAgentService = "SQLcompliance Agent could not be upgraded on remote server";
      public const string Exception_ErrorUninstallingAgentService = "SQLcompliance Agent could not be uninstalled on remote server.  The SQLcompliance agent is currently in use or the upgrade could not obtain exclusive access to the agent.";
      public const string Exception_WMIAccessDeniedLocal = "Administrative access on local server is required";
      public const string Exception_WMIAccessDeniedRemote = "Administrative access on remote server is required";
      public const string Exception_ServerNotResponding = "Server not responding";
      public const string Exception_CantUpgradeIfNotInstalled = "Unable to upgrade SQLcompliance Agent - The SQLcompliance Agent service is not installed on the target computer. Use the Deploy Agent option to install the service.";
      public const string Exception_IncompatibleRepositoryAndAgentSQLServerVersion = "The version of the SQL Server hosting the repository server cannot support the version of the audited SQL Server instance.";
      public const string Exception_RepositoryDoesNotSupportSQL2005 = "SQL Server hosting the repository cannot support SQL Server 2005.  Audited instance: {0}.";

      // Alerting Exceptions
      public const string Exception_AlertingStartup = "An error occurred starting the Alerts processor." ;
      public const string Exception_ErrorAlertingJobPoolThreadDown = "Error: Alert processing job thread is down - attempting to restart";
      public const string Exception_AlertingJobHung = "An error occured requiring the alert processing job pool to be shut down. An alerts job has hung for over 2 hours. This is preventing the event processing engine from restarting.";
      public const string Exception_AlertingJobError_0 = "Alert Job Error:  Unable to retrieve events from {0}";
      public const string Exception_AlertingJobError_1 = "Alert Job Error:  Unable to generate alerts for {0}";
      public const string Exception_AlertingJobError_2 = "Alert Job Error:  Unable to store alerts - ConnectionString: {0}";
      public const string Exception_AlertingJobError_3 = "Alert Job Error:  Unable to prepare actions for {0}";
      public const string Exception_AlertingJobError_4 = "Alert Job Error:  Unable to update alerts watermark for {0}";
      public const string Exception_AlertingJobError_5 = "Alert Job Error:  Unable to perform actions for {0}";
      public const string Exception_InvalidAlertRule = "This alert rule contains incomplete criteria.  Alerts will not be generated from this rule until these criteria are removed or properly specified." ;

      public const string Exception_DataAlertingSCJobError_0 = "SC Data Alert Job Error:  Unable to retrieve Sensitive Column events from {0}";
      public const string Exception_DataAlertingSCJobError_1 = "SC Data Alert Job Error:  Unable to generate data alerts for {0}";
      public const string Exception_DataAlertingSCJobError_2 = "SC Data Alert Job Error:  Unable to store Sensitive Column alerts - ConnectionString: {0}";
      public const string Exception_DataAlertingSCJobError_3 = "SC Data Alert Job Error:  Unable to prepare actions for {0}";
      public const string Exception_DataAlertingSCJobError_4 = "SC Data Alert Job Error:  Unable to perform actions for {0}";

      public const string Exception_DataAlertingBADJobError_0 = "BAD Data Alert Job Error:  Unable to retrieve Before After Data events from {0}";
      public const string Exception_DataAlertingBADJobError_1 = "BAD Data Alert Job Error:  Unable to generate data alerts for {0}";
      public const string Exception_DataAlertingBADJobError_2 = "BAD Data Alert Job Error:  Unable to store Before After Data alerts - ConnectionString: {0}";
      public const string Exception_DataAlertingBADJobError_3 = "BAD Data Alert Job Error:  Unable to prepare actions for {0}";
      public const string Exception_DataAlertingBADJobError_4 = "BAD Data Alert Job Error:  Unable to perform actions for {0}";

      public const string Exception_StatusAlertingJobError_0 = "Status Alert Job Error:  Unable to retrieve status alert rules.";
      public const string Exception_StatusAlertingJobError_1 = "Status Alert Job Error:  Unable to generate the collection server alerts.";
      public const string Exception_StatusAlertingJobError_2 = "Status Alert Job Error:  Unable to store the collection server alerts.";
      public const string Exception_StatusAlertingJobError_3 = "Status Alert Job Error:  Unable to prepare actions for the collection server alerts.";
      public const string Exception_StatusAlertingJobError_4 = "Status Alert Job Error:  Unable to perform actions for the collection server alerts.";
      
      // Event Filters Exceptions
      public const string Exception_IncompleteEventFilter = "This event filter contains incomplete criteria.  Events will not be filtered with this rule until these criteria are removed or properly specified." ;
      public const string Exception_InvalidEventFilter = "This event filter filters all events for a server.  Disable auditing on the target server instead.  Events will not be filtered with this rule until more criteria are added." ;

      // Feature Availability
      public const string Feature_BeforeAfterNotAvailableUserTables = "User tables must be audited for Before-After data auditing to be available.";
      public const string Feature_BeforeAfterNotAvailableAgent = "Before-After data auditing is not supported by the agent.  Please upgrade the agent to version 3.1 or later.";
      public const string Feature_BeforeAfterNotAvailable = "Before-After data auditing is not available for SQL Server 2000.";
      public const string Feature_BeforeAfterNotAvailableVersionUnknown = "Before-After data auditing is not available until an agent is deployed to audit the server and a heartbeat has been received.";
      public const string Feature_BeforeAfterNotAvailableCompatibility = "Before-After data auditing is not available for databases with compatibility level less than 90.";
      public const string Feature_SensitiveColumnNotAvailableAgent = "Sensitive Column auditing is not supported by the agent.  Please upgrade the agent to version 3.5 or later.";
      public const string Feature_SensitiveColumnNotAvailableVersionUnknown = "Sensitive Column auditing is not available until an agent is deployed to audit the server and a heartbeat has been received.";
      public const string Feature_TrustedUserNotAvailableAgent = "Trusted Users are not supported by the agent.  Please upgrade the agent to version 3.0 or later.";

      // SQL Server Versioning exceptions
      public const string Exception_CannotParseSQLServerVersion = "Cannot parse SQL Server version string";
      public const string Exception_InvalidSQLServerOSCombo = "This is an unsupported SQL Server/OS combination; aborting operation";	
		
      public static string Exception_AgentNotAvailable
         = "The SQLcompliance Agent on {0} cannot be reached. The SQLcompliance Agent service may be down or a network error is preventing contact.";
      public static string Exception_ServerNotAvailable
         = "The Collection Server on {0} cannot be reached. The Collection Server service may be down or a network error is preventing contact.";
      
      // Custom serialization and deserialization exception messages
      public const string Exception_SerializationError = "An error occurred when serializing {0}.  Exception: {1}.";
      public const string Exception_DeserializationError = "An error occurred when deserializing {1}.  Exception: {1}.";

      // Various Display strings 
      public const string Msg_EventJob = "SQL compliance manager event file processing job";
            
      // Connection related exceptions
      public const string Exception_ErrorConnectingToSQLServer = "The SQLcompliance Agent cannot connect to the audited SQL Server instance {0}. The Agent will be unable to gather audit data until the problem is corrected.  Exception: {1}";
      public const string Exception_InvalidConnection = "The connection is invalid";
      public const string Exception_ConnectionNotOpen = "The connection is not in open state";

      // Stored procedure related exceptions
      public const string Exception_ErrorRegisterStartupStoredProcedure = "An error occurred when registering startup stored procedure.  Error code = {0}";
      public const string Info_AuditSPCreated = "SQLcompliance audit stored procedure {0} created for {1}.\n" +
         "Configuration Version: {2}\n" +
         "Agent Version: {3}\n" +
         "Last Modified Time (UTC): {4}\n";

      public const string Integrity_NewRecordsFound      = "New events inserted";
      public const string Integrity_MissingRecords       = "Records have been deleted";
      public const string Integrity_ModifiedRecordsFound = "Event record modified";
      public const string Integrity_DummyRecord          = "Dummy record inserted for fixing database integrity.";


      // Get/save trace information related exceptions
      public const string Exception_ErrorCreatingNewStoppedTraceList = "An error occurred when creating a new stopped trace list";

      public const string Debug_InvalidTraceRecord          = "Invalid Trace Record: {0} {1} Error: {2}";
      public const string Debug_MissingServerDatabaseColumn = "No events database established for {0}.";
      public const string Debug_ConfigUpdateNeeded          = "Config update needed for {0}: Old: {1} New: {2}";
      public const string Debug_CantDeleteJob               = "An error occured trying to delete the processing job.";
      public const string Debug_NewEventsCommitted          = "Events written: Server: {0}\nFile: {1}\nProcessed Events: {2} \nEvents written: {3}\nLast EventID:{4}\nHigh Watermark:{5}";
      public const string Debug_CantDeleteTempTable         = "Couldnt delete table from temp processing database";
      public const string Debug_CantDeleteFile              = "An error occurred deleting an uncompressed file.";
      public const string Debug_EventCounts                 = "TraceFile '{0}' - Events Processed:\n"+
         "    Total: {1} Updated: {2} Inserted: {3}\n" +
         "    SQLcm: {4}  Overlapped: {5}  Filtered: {6}  Deleted: {7}";
      public const string Debug_InvalidEventClass           = "TraceFile '{0}' - Date: {1}  EventClass: {2}";

      public const string Info_EnterAlertingJobPool         = "Entering Alert Processing Job Pool Thread";
      public const string Info_LeaveAlertingJobPool         = "Leaving Alert Processing Job Pool Thread";
      public const string Info_EnterJobPool                 = "Entering Event Processing Job Pool Thread";
      public const string Info_LeaveJobPool                 = "Leaving Event Processing Job Pool Thread";
      public static string Info_BeginJobProcess             = "Begin job process {0} ({1} [{2}])";
      public static string Info_EndJobProcess               = "End job process {0} ({1} [{2}]) {3}, elapsed time {4} ms";

      public const string Info_ServerSettings               = "Idera Collection Server Settings\n"+
         "--------------------------------\n" +
         "{0}, {1}\n" + 
         "{2}, {3}\n" + 
         "SQL Server Instance: {4}\n"+
         "Trace Directory: {5}\n"+
         "Collection Server Port: {6}\n"+
         "SQLcompliance Agent Port: {7}\n"+
         "Log level: {8}\n"; 


      public const string Info_AgentSettings                = "Idera SQLcompliance Agent Settings\n" +
         "-------------------------------------\n" +
         "{0}, {1}\n" + 
         "Trace Directory: {2}\n" +
         "Collection Server: {3}\n" +
         "Collection Server Port: {4}\n" +
         "Agent Port: {5}\n" +
         "Log level: {6}\n"; 

      public const string Info_InstanceSettings             = "-------------------------------------\n" +
         "SQL Server Instance: {0}\n" +
         "Current Configuration Version: {1}\n";
                                                              
      public const string Info_UpdatingConfiguration        = "Updating audit configurations for {0}.  Old version: {1}.  New version: {2}.";

      // Alert Strings
      public const string Alert_CorruptedTraceFile          = "Trace file '{0}' for SQL server '{1}' can not be read.";
      public const string Alert_CantProcessTraceFile        = "Trace file '{0}' for SQL server '{1}' has been altered or deleted since receiving it from the SQLcompliance Agent. The audit data in this file cannot be loaded into the repository.";
      public const string Alert_InvalidTraceDirectory       = "The trace file directory location specified in the registry for the Collection Service is invalid. Server startup is being aborted. Specified location is '{0}'";
      public const string Alert_DeletedTraceDirectory       = "The trace file directory was deleted outside of normal event processing. This may have resulted in a loss of auditing data. SQL Secure server will recreate the directory so that auditing may continue.";
      public const string Alert_UnmatechedClusteredAgentPort = "The agent port ({0}) for clustered instance '{1}' does not match the port{2} configured on the agent.";

      public const string Error_NoEventDataAvailable = "No data available for the selected event. This could occur if the event has been groomed or archived from the event database for this SQL Server or if the database is extremely busy and unable to respond to the request in a timely fashion.";

      public const string Error_CantReceiveDefaultTrace = "An error occurred during receiving last default trace";
#endregion

      #region Miscellaneous
		
      // windows permissions stuff
      public const string BuiltInAdministrators    = @"BUILTIN\Administrators";
      public const string BuiltInAdministratorsSID = "S-1-5-32-544";
      public const int    BuiltInAdministratorsRID = 544;

      public const string EventLogSource = "SQLcompliance";
		
		
      public const long OneGigabyte = 1073741824;   // 1024.5 * 1024 * 1024
		
      // User used for system job change log events
      public const string Log_SystemUser = "Collection Server";

      // Registry keys needed to find OSQL on backup server machine
      public const string RegistryKeySql2000ClientSetup = @"SOFTWARE\Microsoft\Microsoft SQL Server\80\Tools\ClientSetup";
      public const string RegistryKeySql70ClientSetup   = @"SOFTWARE\Microsoft\MSSQLServer\Setup";
      public const string RegistryKeySqlPath            = "SQLPath";
      public const string RegistryKeyDemoMode           = "DemoMode";
      public const string OsqlExeName                   = @"\BINN\osql.exe";
      public const int    OsqlQueryMaxLength            = 1000;
		
      // Message Handling Constants
      public const int    MaxSimultaneousStatusLoggingCount     = 5;

      // Window Services and SQL Server Messages used by the agent
      public const string ServiceControlManager        = "Service Control Manager";
      
      // Seed for checksum - so it is a little more random
      public const int     ChecksumSeed                 = 3662;
      
      // Duplicate entry on insert error code
      public const int     SqlErrorCode_DuplicateEntry = 2627;
      
      // Default Application Name for SQL Connections - note: dont change
      // special first character or management console will get filtered out
      // this char is just to reduce likelihood of apps with prefix SQLcompliance
      // from being filtered out accidentally
      public const string   DefaultSqlApplicationName = "SQLcompliânce";
      
      // Time Zone
      public static string TimeZone_UTC                 = "(UTC) Universal Coordinated Time";
      
      // Archive stuff
      public const int ArchiveBatchSize = 25000;
      public const int GroomBatchSize   = 10000;
      
      public const string Error_ArchiveInProgress = "An existing archive operation is already in progress. A new operation cannot be started until the current archive operation is finished.";
      
      // Integrity Check
      public static int BadEventType_EventGap      = 0;
      public static int BadEventType_AddedEvent    = 1;
      public static int BadEventType_ModifiedEvent = 2;

      // Serialization Versioins
      // For each new version, increment the version number and add a new constant for the new
      // version.  Assign the new version number to SerializationVersion as the current version
      // number.
      internal const int SerializationVersion_20 = 200;
      internal const int SerializationVersion_21 = 211;
      internal const int SerializationVersion_30 = 300;
      internal const int SerializationVersion_31 = 310;
      internal const int SerializationVersion_32 = 320;
      internal const int SerializationVersion_33 = 330;
      internal const int SerializationVersion_35 = 350;
      internal const int SerializationVersion_36 = 360;
      internal const int SerializationVersion_37 = 370;
      internal const int SerializationVersion_40 = 400;
      internal const int SerializationVersion_42 = 420;
      internal const int SerializationVersion_43 = 430;
      internal const int SerializationVersion_44 = 440;
       internal const int SerializationVersion_45 = 450;
       internal const int SerializationVersion_50 = 500;
       internal const int SerializationVersion_51 = 510;
       internal const int SerializationVersion_53 = 530;
       internal const int SerializationVersion_54 = 540;
       internal const int SerializationVersion = SerializationVersion_54;

      // SQL Server 2005 system database name and ID
      internal const string SQL2005SystemDatabase   = "mssqlsystemresource";
      internal const int    SQL2005SystemDatabaseId = 32767;


      #endregion
		
      #region Integrity Check and Archive Results

      public const int IntegrityCheck_Passed            = 0;
      public const int IntegrityCheck_InProgress        = 1;
      public const int IntegrityCheck_Failed            = 2;
      public const int IntegrityCheck_FailedAndRepaired = 3;
      public const int IntegrityCheck_Incomplete        = 4;
      
      public const string IntegrityCheckString_Passed            = "Passed";
      public const string IntegrityCheckString_InProgress        = "In progress";
      public const string IntegrityCheckString_Failed            = "Problems found but no action taken";
      public const string IntegrityCheckString_FailedAndRepaired = "Problems found and marked in audit data";
      public const string IntegrityCheckString_Incomplete        = "Incomplete";
      
      public const int Archive_Completed         = 0;
      public const int Archive_InProgress        = 1;
      public const int Archive_FailedIntegrity   = 2;
      public const int Archive_FailedWithErrors  = 3;
      public const int Archive_Incomplete        = 4;
      
      public const string ArchiveString_Passed           = "Completed successfully";
      public const string ArchiveString_InProgress       = "In progress";
      public const string ArchiveString_FailedIntegrity  = "Failed integrity check; archive operation aborted";
      public const string ArchiveString_FailedWithErrors = "Archive failed; see Collection Server event log for details";
      public const string ArchiveString_Incomplete       = "Incomplete";
      
      public const string Exception_ArchiveFailedIntegrity  = "The archive operation could not be run for instance {0}. The integrity check of the events database failed. Archiving cannot be performed until the problems are acknowledged.";
      
      public const string Info_GroomSuccessLog = "Audit data older than {0} days groomed for SQL Server instance {1}." ;
      public const string Error_GroomFailedLog = "An error occurred grooming the audit data for SQL Server {0}.\r\n\r\nError:\r\n\r\n{1}";
      public const string Exception_GroomFailedIntegrity    = "The grooming operation could not be run for instance {0}. The integrity check of the events database failed. Grooming of event data cannot be performed until the problems are acknowledged.";

      // Integrity Check logs and errors
      public const string Error_IntegrityCheckSchemaError = "The schema of the selected database is incompatible with this version of SQL compliance manager. The integrity of this database cannot be checked.\n\nDatabase: {0}"; 
      public const string Error_IntegrityCheckError    = "An error occurred performing the manual integrity check operation."; 
      public const string Error_IntegrityCheckErrorLog = "An error occurred performing the integrity check for SQL Server {0}\r\n\r\nError:\r\n\r\n{1}"; 
      public const string Error_IntegrityCheckRepairErrorLog = "An error occurred marking integrity errors for SQL Server {0}\r\n\r\nError:\r\n\r\n{1}"; 

      public const string Info_IntegrityCheckPassed    = "The integrity check completed and found no problems in database {0}.";
      public const string Info_IntegrityCheckPassedLog =
         "Integrity check passed.\r\n" +
         "\tInstance: {0}\r\n" +
         "\tDatabase: {1}\r\n";
      public const string Info_IntegrityCheckPassedPeriodLog =
         "\tPeriod:   '{0}' to '{1}'";
      public const string Info_IntegrityCheckPassedNoEventsLog =
         "\tPeriod:   No events in database";
         
      public const string Info_IntegrityCheckFailedLog =
         "Failed Integrity Check\r\n" +
         "\tInstance: {0}\r\n" +
         "\tDatabase: {1}\r\n" +
         "\tPeriod:   '{2}' to '{3}'\r\n\r\n" +
         "Summary of problems:\r\n" +
         "\tEvents deleted:  {4}\r\n" +
         "\tEvents modified: {5}\r\n" +
         "\tEvents inserted: {6}\r\n\r\n" +
         "The problems found during the integrity check were ";
         
      public const string Info_IntegrityCheckFailedLog_Fixed    = "marked."; 
      public const string Info_IntegrityCheckFailedLog_NotFixed = "not marked.";
         
      public const string Info_IntegrityRepaired = "The integrity problems in this database have been marked. Events with category 'Integrity Check' have been placed in the database to mark the spot where the problem occurred.";
		
      #endregion
		
      #region Repository Database
		
      //-------------------------------------------------------------------------
      // Databases making up the SQLcompliance world
      //
      // Scheme - commponents check for compatibility by checking 100s digit
      //          so if you are making a change that breaks old components roll
      //          the 100s digit; otehrwie just bump the ones
      //-------------------------------------------------------------------------
      public const int    RepositorySqlComplianceDbSchemaVersion    = 1702;
      public const int    RepositoryEventsDbSchemaVersion           = 703;
		
      public const string RepositoryServerDefault               = "(LOCAL)";
		
      // SQLServer
      public const string RepositoryMasterDatabase                = "master";
      public const string RepositoryDatabase                      = "SQLcompliance";
      public const string RepositoryServerTable                   = "Servers";
      public const string RepositoryDatabaseTable                 = "Databases";
      public const string RepositoryDatabaseObjectsTable          = "DatabaseObjects";
      public const string RepositoryDataChangeTablesTable         = "DataChangeTables";
      public const string RepositoryDataChangeColumnsTable        = "DataChangeColumns";
      public const string RepositorySensitiveColumnTablesTable    = "SensitiveColumnTables";
      public const string RepositorySensitiveColumnColumnsTable   = "SensitiveColumnColumns";

      public const string RepositoryReglationTable       = "Regulation";
      public const string RepositoryRegulationMapTable   = "RegulationMap";

      public const string RepositoryAgentEventTable            = "AgentEvents";
      public const string RepositoryAgentEventTypeTable	      = "AgentEventTypes";
      public const string RepositoryChangeLogEventTable        = "ChangeLog";
      public const string RepositoryChangeLogEventTypeTable	   = "ChangeLogEventTypes";
      public const string RepositoryJobsTable		            = "Jobs";
      public const string RepositorySystemDatabaseTable        = "SystemDatabases";
		
      public const string RepositoryConfigurationTable   = "Configuration";
      public const string RepositoryReportsTable         = "Reports";

      public const string RepositoryGroupsTable          = "DatabaseGroups";
      public const string RepositoryMembersTable         = "DatabaseGroupMembers";

      public const string RepositoryEventTypesTable		= "EventTypes";
      public const string RepositoryEventCategoriesTable = "EventCategories";

      // Events and Archive Databases		
      public const string DatabaseType_System            = "System";
      public const string DatabaseType_Archive           = "Archive";
      public const string DatabaseType_Events            = "Event";
      
      public const string RepositoryEventsTable          = "Events";
      public const string RepositoryEventSqlTable        = "EventSQL";
      public const string RepositoryMetaTable            = "Description";
      public const string RepositoryStatsTable           = "Stats";
      public const string RepositoryApplicationsTable    = "Applications";
      public const string RepositoryHostsTable           = "Hosts";
      public const string RepositoryLoginsTable          = "Logins";
      public const string RepositoryDataChangesTable     = "DataChanges";
      public const string RepositoryColumnChangesTable   = "ColumnChanges";
      public const string RepositorySensitiveColumnsTable = "SensitiveColumns";
      public const string RepositoryTablesTable = "Tables";
      public const string RepositoryColumnsTable = "Columns";
		
      public const string RepositoryArchiveMetaTable     = "Description";
      public const string RepositoryArchiveEventsTable   = RepositoryEventsTable;
      public const string RepositoryArchiveEventSQLTable = RepositoryEventSqlTable;
      public const string RepositoryArchiveLogTable      = RepositoryChangeLogEventTable;

      public const string Repository_EventDBPrefix       = "SQLcompliance_";
      public const string Repository_EventsSuffix        = "Events";
      public const string Repository_SqlSuffix           = "Sql";
		
      // Processing Database
      public const string RepositoryTempDatabase         = "SQLcomplianceProcessing";
      public const string RepositoryTemp_TimesTable      = "TraceTimes";
      public const string RepositoryTemp_StatesTable     = "TraceStates";
      public const string RepositoryTemp_DupTable        = "TraceCounts";
      public const string RepositoryTemp_LoginsTable     = "TraceLogins";

      // Alerting Tables
      public const string RepositoryAlertsTable = "Alerts" ;
      public const string RepositoryAlertRulesTable = "AlertRules" ;
      public const string StatusRuleTypesTable = "StatusRuleTypes";
      public const string DataRuleTypesTable = "DataRuleTypes";
      public const string RepositoryAlertRuleConditionsTable = "AlertRuleConditions" ;
      public const string RepositoryAlertRuleActionsTable = "AlertRuleActions" ;
      public const string RepositoryActionResultsTable = "ActionResults" ;
      //public const string RepositoryEventFieldMapTable = "EventFieldMap" ;
      public const string RepositoryAlertTypesTable = "AlertTypes" ;
      public const string RepositoryAlertRuleActionTypesTable = "AlertRuleActionTypes" ;
      public const string RepositoryActionResultStatusTypesTable = "ActionResultStatusTypes" ;

      // Event Filters Tables
      public const string RepositoryEventFiltersTable = "EventFilters" ;
      public const string RepositoryEventFilterConditionsTable = "EventFilterConditions" ;

      // Repository Stored Procedures
      public const string RepositoryStoredProcUpgradeEventDatabase = "sp_sqlcm_UpgradeEventDatabase";

      #endregion		

      #region SQL Statements

      // Commonly used SQL statements
      public const string QueryStoredProcedureExistence = "SELECT count(name) FROM sysobjects WHERE name = '{0}' AND type = 'P'";
      public const string DropStoredProcedure = "DROP PROC {0}";
      
      #endregion
      
      #region Public Properties

      public static int   alertingMaxEventsToProcess = 5000;

      public static bool  filterAgentEvents  = true;
      public static bool  filterGUIEvents    = true;
      public static bool  filterServerEvents = true;
      
      public static bool  DontDeleteNonDMLTraces = false;
      public static bool  DontDeleteDMLTraces    = false;

      public static bool  AllowCaptureSql = true ;

      public static bool  LogFilteredOutEvents = false;
      
      static public int   sqlcommandTimeout = 300;  // timeout in seconds for SQLcommands - used in various places throughout code
      static public bool   optimizeRules = true ;
      
      static public int   archiveBatchSize;
      static public int   groomBatchSize;
      static public bool  ParseForUpdateStats = true;

      static public bool LinkDataChangeRecords = true;
      // must be explictly used 

      static public int   FileTransferPageSize;
      static public bool  UseClientActivatedFileTransfer = false;
      
      static public int   DaysStatsCached;

      static public bool LogSQLParsingErrors = true;
      #endregion

      #region Permission Checks

       public const string PermissionCheck1 = @"Collection Service has rights to the repository databases.";
       public const string PermissionCheck2 = @"Collection Service has rights to read registry HKLM\Software\Idera\SQLcompliance.";
       public const string PermissionCheck3 = @"Collection Service has permissions to collection trace directory.";
       public const string PermissionCheck4 = @"Agent Service has permissions to agent trace directory.";
       public const string PermissionCheck5 = @"Agent Service has rights to read registry HKLM\Software\Idera\SQLcompliance.";
       public const string PermissionCheck6 = @"Agent Service has rights to the instance.";
       public const string PermissionCheck7 = @"SQL Server has permissions to the agent trace directory.";
       public const string PermissionCheck8 = @"SQL Server has permissions to the collection trace directory.";

       #endregion

       public const string CustomerPortalLink = "http://www.idera.com/licensing";

       #region LM 2.0 Integration-Adding
       //[START] SQLcm : LM 2.0 Integration-Adding a link to License Manager Interface
       public const string LICENSE_MANAGER_PATH = @"\License Manager Utility.exe";
       public const string PRODUCT_SHORT_NAME = "SQLcm";
       public const string REGISTRY_PATH_LM = @"SOFTWARE\Idera\LM\SQLcm";
       //[END] SQLcm  : LM 2.0 Integration-Adding a link to License Manager Interface

       #endregion

       public const string ConnectionStringApplicationNamePrefix = "SQLcompliance Manager";
       public const string DesktopClientConnectionStringApplicationName = ConnectionStringApplicationNamePrefix + " Desktop Client";

       public const int DefaultCommandTimeout = 300;
   }
}
