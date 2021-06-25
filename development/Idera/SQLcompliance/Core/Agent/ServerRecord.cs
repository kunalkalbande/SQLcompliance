using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.TimeZoneHelper;
using TimeZoneInfo = Idera.SQLcompliance.Core.TimeZoneHelper.TimeZoneInfo;

namespace Idera.SQLcompliance.Core.Agent
{
    /// <summary>
    /// Summary description for ServerRecord.
    /// </summary>
    /// 

    public class ServerAgentDetails
    {
        public ServerAgentDetails() { }
        private string agentVersion;
        private int sqlVersion;

        public string AgentVersion
        {
            get { return agentVersion; }
            set { agentVersion = value; }
        }

        public int SqlVersion
        {
            get { return sqlVersion; }
            set { sqlVersion = value; }
        }
    }
    public class ServerRecord
    {
        #region Private Properties

        // Each private property has a public counterpart declared below - The publc counterpart
        // is capitalized - e.g. private agentServer = public AgentServer - These are separated to 
        // make the code easier to read since the public definition is little more then plain old
        // get and set declarations

        private const string COMMA_CHARACTER = ",";

        private int srvId;

        private string instance;               // SQL Server instance monitored by this agent
        private string description;

        private string instanceServer;         // Computer hosting this SQL Server instance
        private string instanceAlias;          // Internal instance alias for filenames and registry
        private int sqlVersion;             // SQL Server major version number
        private string eventDatabase;
        private int defaultAccess;          // default database access: 0=none,1=events only,2=events and sql
        private int maxSqlLength;           // max length of sql statements to store for this server

        private int bias;
        private int standardBias;
        private DateTime standardDate;
        private int daylightBias;
        private DateTime daylightDate;


        private string server;                 // Server on which the SQLsecure Collection service is running
        private int serverPort;             // TCP Port on which collection service listens

        private bool isDeployed;
        private bool isDeployedManually;
        private bool isUpdateRequested;
        private bool isAuditedServer;
        private bool isRunning;              // Did bootstrap routine run to completion?
        private bool isCrippled;             // An error occurred that shut everything down - needs kickstart msg from GUI
        private bool isEnabled;              // Is agent actively collecting audit data
        private bool isSqlSecureDb;
        private bool isOnRepositoryHost;
        private int deployedByCommand;

        private DateTime timeStartup;            // Time agent started
        private DateTime timeCreated;
        private DateTime timeLastModified;
        private DateTime timeEnabledModified;
        private DateTime timeLastHeartbeat = DateTime.MinValue;
        private DateTime timeLastAgentContact = DateTime.MinValue;
        private DateTime timeLastCollection = DateTime.MinValue;

        // Audit Settings		
        private bool auditLogins;
        private bool auditLogouts;
        private bool auditFailedLogins;
        private bool auditDDL;
        private bool auditSecurity;
        private bool auditAdmin;
        private bool auditDML;
        private bool auditSELECT;
        private bool auditTrace = false;

        private int auditAccessCheck;
        private bool auditSystemEvents;
        private bool auditCaptureSQL;
        private bool auditCaptureSQLXE = true; //5.4 XE
        private bool auditExceptions = false;
        private bool auditUDE;

        private string auditTrustedUsersList;//v5.6 SQLCM-5373
        private string auditUsersList;
        private bool auditUserAll;
        private bool auditUserLogins;
        private bool auditUserLogouts;
        private bool auditUserFailedLogins;
        private bool auditUserDDL;
        private bool auditUserSecurity;
        private bool auditUserAdmin;
        private bool auditUserDML;
        private bool auditUserSELECT;
        private int auditUserAccessCheck;
        private bool auditUserExtendedEvents; // SQLCm 5.4_4.1.1_Extended Events 
        private bool auditUserCaptureSQL;
        private bool auditUserCaptureTrans;
        private bool auditUserExceptions;
        private bool auditUserUDE;

        private DateTime lastConfigUpdate = DateTime.MinValue;
        private int configVersion;
        private int lastKnownConfigVersion;
        private bool configUpdateRequested;

        private string agentServer;            // Server on which agent is running
        private int agentPort;              // TCP Port on which agent listens
        private string agentVersion;           // Agent assembly version - used for upgrade logic
        private string agentServiceAccount;
        private long agentMaxTraceSize;           // Maximum event trace file size in MB
        private string agentTraceDirectory;         // Directory where the audit trace files are saved
        private int agentHeartbeatInterval;
        private int agentCollectionInterval;
        private int agentForceCollectionInterval;
        private int agentLogLevel;
        private TraceOption agentTraceOptions;
        private int agentMaxFolderSize;
        private int agentMaxUnattendedTime;
        private int agentDetectionInterval;
        private int agentTraceStartTimeout;

        private bool newVersionAvailable;
        private string connectionString;       // Connection string common to all connections
                                               //private TraceCollector collector;              // For periodically collecting trace files
                                               //private TimerState     housekeeper;            // For periodic housekeeping work

        private bool eventReviewNeeded;
        private bool containsBadEvents;
        private int highWatermark;
        private int lowWatermark;

        private DateTime timeLastIntegrityCheck;
        private int lastIntegrityCheckResult;
        private DateTime timeLastArchive;
        private int lastArchiveResult;

        private UInt64 agentHealth;

        //5.5 Audit Logs
        private bool isAuditLogEnabled;

        private bool addNewDatabasesAutomatically; //SQLCM -541/4876/5259 v5.6
        private DateTime lastNewDatabasesAddTime;


        // Audit and event trace configurations
        private ServerAuditConfiguration auditConfiguration;
        private TraceConfiguration[] traceConfigurations;

        bool insertAgentProperties = false;

        private bool auditUserCaptureDDL;

        private bool _isCluster;
        private bool _isHadrEnabled;

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        private int countDatabasesAuditingAllObjects;

        private bool isCompressedFile = true;

        private bool isRowCountEnabled = false;

        #endregion

        #region Public Properties (Get/Set definitions)

        public int SrvId
        {
            get { return srvId; }
            set { srvId = value; }
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        public int CountDatabasesAuditingAllObjects
        {
            get
            {
                return countDatabasesAuditingAllObjects;
            }
            set
            {
                countDatabasesAuditingAllObjects = value;
            }
        }

        public bool CaptureDataChanges
        {
            get { return auditConfiguration.CaptureDataChanges; }
        }

        public bool AuditSensitiveColumns
        {
            get { return auditConfiguration.AuditSensitiveColumns; }
        }

        public string Instance
        {
            get { return instance; }
            set
            {
                if (value != null)
                {
                    instance = value.ToUpper();
                    instanceAlias = NativeMethods.GetHashCode(instance).ToString();
                    if (auditConfiguration != null)
                    {
                        auditConfiguration.Instance = instance;
                        auditConfiguration.InstanceAlias = instanceAlias;
                    }
                }
                else
                {
                    instance = null;
                    instanceAlias = null;
                }
            }
        }

        private int? _instancePort;

        public int? InstancePort
        {
            get
            {
                return _instancePort;
            }

            set { _instancePort = value; }
        }

        private string _sqlVersionName = string.Empty;

        public string SqlVersionName
        {
            get { return _sqlVersionName; }
            set { _sqlVersionName = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string InstanceServer
        {
            get { return instanceServer; }
            set { instanceServer = value; }
        }

        public string InstanceAlias
        {
            get { return instanceAlias; }
        }

        public int Bias
        {
            get { return bias; }
            set { bias = value; }
        }

        public int StandardBias
        {
            get { return standardBias; }
            set { standardBias = value; }
        }

        public DateTime StandardDate
        {
            get { return standardDate; }
            set { standardDate = value; }
        }

        public DateTime DaylightDate
        {
            get { return daylightDate; }
            set { daylightDate = value; }
        }

        public int DaylightBias
        {
            get { return daylightBias; }
            set { daylightBias = value; }
        }

        public int SqlVersion
        {
            // For 1.2, we override the sqlVersion flag to avoid changing the db schema
            // if the instance was a virtual server we add 1000
            get { return (sqlVersion % 1000); }
            set
            {
                sqlVersion = value;
                if (this.auditConfiguration != null)
                    auditConfiguration.sqlVersion = sqlVersion % 1000;
            }
        }

        public bool isClustered
        {
            get
            {
                // For 1.2, we override the sqlVersion flag to avoid changing the db schema
                // if the instance was a virtual server we add 1000
                if (sqlVersion >= 1000)
                    return true;
                else
                    return false;
            }

            set
            {
                if (value)
                {
                    if (sqlVersion < 1000) sqlVersion += 1000;
                }
                else
                {
                    if (sqlVersion >= 1000) sqlVersion -= 1000;
                }
            }
        }

        public string EventDatabase
        {
            get { return eventDatabase; }
            set { eventDatabase = value; }
        }

        public int DefaultAccess
        {
            get { return defaultAccess; }
            set { defaultAccess = value; }
        }

        public int MaxSqlLength
        {
            get { return maxSqlLength; }
            set { maxSqlLength = value; }
        }

        public string Server
        {
            get { return server; }
            set { server = value; }
        }
        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }

        public bool IsDeployed
        {
            get { return isDeployed; }
            set { isDeployed = value; }
        }

        public bool IsDeployedManually
        {
            get { return isDeployedManually; }
            set { isDeployedManually = value; }
        }

        public bool IsUpdateRequested
        {
            get { return isUpdateRequested; }
            set { isUpdateRequested = value; }
        }

        public bool IsAuditedServer
        {
            get { return isAuditedServer; }
            set { isAuditedServer = value; }
        }

        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; }
        }

        public bool IsCrippled
        {
            get { return isCrippled; }
            set { isCrippled = value; }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public bool IsSqlSecureDb
        {
            get { return isSqlSecureDb; }
            set { isSqlSecureDb = value; }
        }

        public bool IsOnRepositoryHost
        {
            get { return isOnRepositoryHost; }
            set { isOnRepositoryHost = value; }
        }

        public DateTime TimeStartup
        {
            get { return timeStartup; }
            set { timeStartup = value; }
        }

        public DateTime TimeCreated
        {
            get { return timeCreated; }
            set { timeCreated = value; }
        }

        public DateTime TimeLastModified
        {
            get { return timeLastModified; }
            set { timeLastModified = value; }
        }

        public DateTime TimeEnabledModified
        {
            get { return timeEnabledModified; }
            set { timeEnabledModified = value; }
        }

        public DateTime TimeLastHeartbeat
        {
            get { return timeLastHeartbeat; }
            set { timeLastHeartbeat = value; }
        }

        public DateTime TimeLastAgentContact
        {
            get { return timeLastAgentContact; }
            set { timeLastAgentContact = value; }
        }

        public DateTime TimeLastCollection
        {
            get { return timeLastCollection; }
            set { timeLastCollection = value; }
        }

        public DateTime TimeLastIntegrityCheck
        {
            get { return timeLastIntegrityCheck; }
            set { timeLastIntegrityCheck = value; }
        }

        public int LastIntegrityCheckResult
        {
            get { return lastIntegrityCheckResult; }
            set { lastIntegrityCheckResult = value; }
        }

        public DateTime TimeLastArchive
        {
            get { return timeLastArchive; }
            set { timeLastArchive = value; }
        }

        public int LastArchiveResult
        {
            get { return lastArchiveResult; }
            set { lastArchiveResult = value; }
        }


        // Audit Settings	
        public bool AuditLogins
        {
            get { return auditLogins; }
            set { auditLogins = value; }
        }

        /// <summary>
        /// SQLCM-5375-6.1.4.3-Capture Logout Events at Server level
        /// </summary>
	    public bool AuditLogouts
        {
            get { return auditLogouts; }
            set { auditLogouts = value; }
        }

        public bool AuditFailedLogins
        {
            get { return auditFailedLogins; }
            set { auditFailedLogins = value; }
        }

        public bool AuditDDL
        {
            get { return auditDDL; }
            set { auditDDL = value; }
        }

        public bool AuditSecurity
        {
            get { return auditSecurity; }
            set { auditSecurity = value; }
        }

        public bool AuditAdmin
        {
            get { return auditAdmin; }
            set { auditAdmin = value; }
        }

        public bool AuditDML
        {
            get { return auditDML; }
            set { auditDML = value; }
        }

        public bool AuditSELECT
        {
            get { return auditSELECT; }
            set { auditSELECT = value; }
        }

        public bool AuditTrace
        {
            get { return auditTrace; }
            set { auditTrace = value; }
        }

        public bool AuditExceptions
        {
            get { return auditExceptions; }
            set { auditExceptions = value; }
        }

        public AccessCheckFilter AuditAccessCheck
        {
            get { return (AccessCheckFilter)auditAccessCheck; }
            set { auditAccessCheck = (int)value; }
        }

        public bool AuditSystemEvents
        {
            get { return auditSystemEvents; }
            set { auditSystemEvents = value; }
        }

        public bool AuditCaptureSQL
        {
            get { return auditCaptureSQL; }
            set { auditCaptureSQL = value; }
        }

        public bool AuditCaptureSQLXE
        {
            get { return auditCaptureSQLXE; }
            set { auditCaptureSQLXE = value; }
        }

        public bool AuditUDE
        {
            get { return auditUDE; }
            set { auditUDE = value; }
        }

        ////v5.6 SQLCM-5373
        public string AuditTrustedUsersList
        {
            get { return auditTrustedUsersList; }
            set { auditTrustedUsersList = value; }
        }

        // Users
        public string AuditUsersList
        {
            get { return auditUsersList; }
            set { auditUsersList = value; }
        }

        public bool AuditUserAll
        {
            get { return auditUserAll; }
            set { auditUserAll = value; }
        }

        public bool AuditUserLogins
        {
            get { return auditUserLogins; }
            set { auditUserLogins = value; }
        }

        /// <summary>
        /// SQLCM-5375-6.1.4.3-Capture Logout Events
        /// </summary>
	    public bool AuditUserLogouts
        {
            get { return auditUserLogouts; }
            set { auditUserLogouts = value; }
        }

        public bool AuditUserFailedLogins
        {
            get { return auditUserFailedLogins; }
            set { auditUserFailedLogins = value; }
        }
        public bool AuditUserDDL
        {
            get { return auditUserDDL; }
            set { auditUserDDL = value; }
        }
        public bool AuditUserSecurity
        {
            get { return auditUserSecurity; }
            set { auditUserSecurity = value; }
        }

        public bool AuditUserAdmin
        {
            get { return auditUserAdmin; }
            set { auditUserAdmin = value; }
        }

        public bool AuditUserDML
        {
            get { return auditUserDML; }
            set { auditUserDML = value; }
        }

        public bool AuditUserSELECT
        {
            get { return auditUserSELECT; }
            set { auditUserSELECT = value; }
        }

        // SQLCm 5.4_4.1.1_Extended Events 
        public bool AuditExtendedEvents
        {
            get { return auditUserExtendedEvents; }
            set { auditUserExtendedEvents = value; }
        }

        public AccessCheckFilter AuditUserAccessCheck
        {
            get { return (AccessCheckFilter)auditUserAccessCheck; }
            set { auditUserAccessCheck = (int)value; }
        }

        public bool AuditUserCaptureSQL
        {
            get { return auditUserCaptureSQL; }
            set { auditUserCaptureSQL = value; }
        }

        public bool AuditUserCaptureTrans
        {
            get { return auditUserCaptureTrans; }
            set { auditUserCaptureTrans = value; }
        }

        public bool AuditUserExceptions
        {
            get { return auditUserExceptions; }
            set { auditUserExceptions = value; }
        }

        public bool AuditUserUDE
        {
            get { return auditUserUDE; }
            set { auditUserUDE = value; }
        }

        public DateTime LastConfigUpdate
        {
            get { return lastConfigUpdate; }
            set { lastConfigUpdate = value; }
        }

        public int ConfigVersion
        {
            get { return configVersion; }
            set { configVersion = value; }
        }

        public int LastKnownConfigVersion
        {
            get { return lastKnownConfigVersion; }
            set { lastKnownConfigVersion = value; }
        }

        public bool ConfigUpdateRequested
        {
            get { return configUpdateRequested; }
            set { configUpdateRequested = value; }
        }

        public string AgentServer
        {
            get { return agentServer; }
            set { agentServer = value; }
        }

        public int AgentPort
        {
            get { return agentPort; }
            set { agentPort = value; }
        }

        public string AgentVersion
        {
            get { return agentVersion; }
            set { agentVersion = value; }
        }

        public string AgentServiceAccount
        {
            get { return agentServiceAccount; }
            set { agentServiceAccount = value; }
        }

        public long AgentMaxTraceSize
        {
            get { return agentMaxTraceSize; }
            set { agentMaxTraceSize = value; }
        }

        public string AgentTraceDirectory
        {
            get { return agentTraceDirectory; }
            set
            {
                agentTraceDirectory = value;
            }
        }

        public int AgentHeartbeatInterval
        {
            get { return agentHeartbeatInterval; }
            set { agentHeartbeatInterval = value; }
        }

        public int AgentCollectionInterval
        {
            get { return agentCollectionInterval; }
            set { agentCollectionInterval = value; }
        }

        public int AgentForceCollectionInterval
        {
            get { return agentForceCollectionInterval; }
            set { agentForceCollectionInterval = value; }
        }

        public int AgentTraceStartTimeout
        {
            get { return agentTraceStartTimeout; }
            set { agentTraceStartTimeout = value; }
        }

        public int DetectionInterval
        {
            get { return agentDetectionInterval; }
            set { agentDetectionInterval = value; }
        }

        public int AgentLogLevel
        {
            get { return agentLogLevel; }
            set { agentLogLevel = value; }
        }

        public TraceOption AgentTraceOptions
        {
            get { return agentTraceOptions; }
            set { agentTraceOptions = value; }
        }

        public int AgentMaxFolderSize
        {
            get { return agentMaxFolderSize; }
            set { agentMaxFolderSize = value; }
        }

        public int AgentMaxUnattendedTime
        {
            get { return agentMaxUnattendedTime; }
            set { agentMaxUnattendedTime = value; }
        }

        public bool NewVersionAvailable
        {
            get { return newVersionAvailable; }
            set { newVersionAvailable = value; }
        }

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /*
       public TraceCollector Collector
       {
          get { return collector; }
          set { collector = value; }
       }

       public TimerState Housekeeper
       {
          get { return housekeeper; }
          set { housekeeper = value; }
       }*/

        public ServerAuditConfiguration AuditSettings
        {
            get { return auditConfiguration; }
            set { auditConfiguration = value; }
        }

        public TraceConfiguration[] TraceConfigurations
        {
            get { return traceConfigurations; }
            set { traceConfigurations = value; }
        }

        public bool InsertAgentProperties
        {
            get { return insertAgentProperties; }
            set { insertAgentProperties = value; }
        }

        public bool EventReviewNeeded
        {
            get { return eventReviewNeeded; }
            set { eventReviewNeeded = value; }
        }

        public bool ContainsBadEvents
        {
            get { return containsBadEvents; }
            set { containsBadEvents = value; }
        }

        public int HighWatermark
        {
            get { return highWatermark; }
            set { highWatermark = value; }
        }

        public int LowWatermark
        {
            get { return lowWatermark; }
            set { lowWatermark = value; }
        }

        public UInt64 AgentHealth
        {
            get { return agentHealth; }
            set { agentHealth = value; }
        }

        public bool AuditUserCaptureDDL
        {
            get { return auditUserCaptureDDL; }
            set { auditUserCaptureDDL = value; }
        }

        public bool IsCluster
        {
            get { return _isCluster; }
            set { _isCluster = value; }
        }


        public bool IsHadrEnabled
        {
            get { return _isHadrEnabled; }
            set { _isHadrEnabled = value; }
        }

        public int DeployedByCommand
        {
            get { return deployedByCommand; }
            set { deployedByCommand = value; }
        }


        //5.5 Audit Log
        public bool IsAuditLogEnabled
        {
            get { return isAuditLogEnabled; }
            set { isAuditLogEnabled = value; }
        }
        //SQLCM -541/4876/5259 v5.6
        public bool AddNewDatabasesAutomatically
        {
            get { return addNewDatabasesAutomatically; }
            set { addNewDatabasesAutomatically = value; }
        }

        public DateTime LastNewDatabasesAddTime
        {
            get { return lastNewDatabasesAddTime; }
            set { lastNewDatabasesAddTime = value; }
        }

        public bool IsCompressedFile
        {
            get { return isCompressedFile; }
            set { isCompressedFile = value; }
        }
        #endregion

        #region LastError

        static string errMsg = "";
        static public string GetLastError() { return errMsg; }

        #endregion

        #region Constructor

        /// <summary>
        /// Base constructor for ServerRecord objects
        /// </summary>
        public ServerRecord()
        {
            timeStartup = DateTime.UtcNow;

            instance = "";
            instanceAlias = "";
            instanceServer = "";

            lowWatermark = -2100000000;
            highWatermark = -2100000000;

            sqlVersion = 0;

            agentServer = Dns.GetHostName();
            agentPort = CoreConstants.AgentServerTcpPort;
            server = CoreConstants.Agent_Default_Server;
            serverPort = CoreConstants.CollectionServerTcpPort;

            agentVersion = "DEVEL";

            agentLogLevel = 1;

            isEnabled = false;
            isRunning = false;
            isCrippled = false;

            srvId = -1;
            description = "";

            eventDatabase = "";
            defaultAccess = 2;
            maxSqlLength = 512;
            //         collector              = null

            // Defaults
            agentMaxTraceSize = CoreConstants.Agent_Default_MaxTraceSize;
            agentTraceOptions = (TraceOption)CoreConstants.Agent_Default_TraceOptions;

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                string location = entryAssembly == null ? string.Empty : Assembly.GetEntryAssembly().Location;
                FileInfo fileInfo = new FileInfo(location);
                agentTraceDirectory = Path.Combine(fileInfo.DirectoryName, CoreConstants.Agent_Default_TraceDirectory);
            }

            agentHeartbeatInterval = CoreConstants.Agent_Default_HeartbeatInterval;
            agentCollectionInterval = CoreConstants.Agent_Default_CollectInterval;
            agentDetectionInterval = CoreConstants.Agent_Default_TamperingDetectionInterval;
            agentTraceStartTimeout = CoreConstants.Agent_Default_TraceStartTimeout;

            agentMaxFolderSize = CoreConstants.Agent_Default_MaxFolderSize;
            agentMaxUnattendedTime = CoreConstants.Agent_Default_MaxUnattendedTime;

            auditConfiguration = new ServerAuditConfiguration();

            auditUsersList = "";

            // unused options - make sure they are off by default         
            auditDML = false;
            auditSELECT = false;
            auditCaptureSQL = false;

            auditExceptions = false;
            auditUserExceptions = false;

            auditSystemEvents = false;

            //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
            countDatabasesAuditingAllObjects = 0;
        }

        #endregion

        #region Connection
        private SqlConnection connection = null;
        public SqlConnection Connection
        {
            get { return connection; }
            set
            {
                connection = value;

                try
                {
                    SQLHelpers.CheckConnection(connection);
                }
                catch { }
            }
        }

        #endregion

        #region Static Public Routines

        //-------------------------------------------------------------
        // GetServerId
        //--------------------------------------------------------------
        public static int
           GetServerId(
              SqlConnection conn,
             string serverName
          )
        {
            int serverId = -1;
            string cmdstr = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                cmdstr = GetServerIdSQL(serverName);

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            serverId = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "GetServerId", cmdstr, ex);
                errMsg = ex.Message;
            }

            return serverId;
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Get count of all databases with auditing all objects enabled
        public static int GetCountDatabasesAuditingAllObjects(SqlConnection conn, int srvId)
        {
            int countDatabasesAuditingAllObjects = 0;
            string query = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                query = GetCountDatabasesAuditingAllObjectsSQL(srvId);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            countDatabasesAuditingAllObjects = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "GetCountDatabasesAuditingAllObjects", query, ex);
                errMsg = ex.Message;
            }

            return countDatabasesAuditingAllObjects;
        }

        //SQLcm 5.6 - Fix for 5820
        //Gets agent version data for the required server id
        public static IList GetAgentVersionDataForServerId(SqlConnection conn, int srvId)
        {
            string query = "";
            IList dbList = null;
            ServerAgentDetails sd = null;
            try
            {
                SQLHelpers.CheckConnection(conn);

                query = GetAgentVersionDataForServerIdSQL(srvId);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dbList = new ArrayList();
                        if (reader.Read())
                        {
                            sd = new ServerAgentDetails();
                            sd.AgentVersion = reader.IsDBNull(0) ? null : reader.GetString(0);
                            sd.SqlVersion = reader.GetInt32(1);
                            dbList.Add(sd);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "GetAgentVersionDataForServerId", query, ex);
                errMsg = ex.Message;
            }

            return dbList;
        }

        //Get count of all databases with auditing DML enabled
        public static int GetCountAuditingDMLEnabled(SqlConnection conn, int srvId)
        {
            int countDatabasesAuditingDMLEnabled = 0;
            string query = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                query = GetCountAuditingDMLEnabledSQL(srvId);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            countDatabasesAuditingDMLEnabled = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "GetCountAuditingDMLEnabled", query, ex);
                errMsg = ex.Message;
            }

            return countDatabasesAuditingDMLEnabled;
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Increment 'countDatabasesAuditingAllObjects'
        public static void IncrementCountDatabasesAuditingAllObjects(SqlConnection conn, int srvId)
        {
            string query = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                query = GetIncrementCountDatabasesAuditingAllObjectsSQL(srvId);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "IncrementCountDatabasesAuditingAllObjects", query, ex);
                errMsg = ex.Message;
            }
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Decrement 'countDatabasesAuditingAllObjects'
        public static void DecrementCountDatabasesAuditingAllObjects(SqlConnection conn, int srvId)
        {
            string query = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                query = GetDecrementCountDatabasesAuditingAllObjectsSQL(srvId);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "DecrementCountDatabasesAuditingAllObjects", query, ex);
                errMsg = ex.Message;
            }
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Update 'countDatabasesAuditingAllObjects' with 'newValue'
        public static void UpdateCountDatabasesAuditingAllObjects(SqlConnection conn, int srvId, int newValue)
        {
            string query = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                query = GetUpdateCountDatabasesAuditingAllObjectsSQL(srvId, newValue);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "UpdateCountDatabasesAuditingAllObjects", query, ex);
                errMsg = ex.Message;
            }
        }

        //----------------------------------------------------------------
        // CompareVersions
        //----------------------------------------------------------------
        public static int CompareVersions(string version1, string version2)
        {
            if (string.IsNullOrEmpty(version2))
            {
                return 1;
            }
            if (string.IsNullOrEmpty(version1))
            {
                return -1;
            }
            if (version1.Equals(version2))
            {
                return 0;
            }
            // a purely string compare of the versions returns incorrect results when the build number is 4 digits
            string[] ver1 = version1.Split('.');
            string[] ver2 = version2.Split('.');
            // if the parts aren't equal then punt and do a string compare
            if (ver1.Length != ver2.Length)
            {
                return version1.CompareTo(version2);
            }

            for (int idx = 0; idx < ver1.Length; idx++)
            {
                try
                {
                    int num1 = Convert.ToInt32(ver1[idx]);
                    int num2 = Convert.ToInt32(ver2[idx]);

                    int comp = num1.CompareTo(num2);
                    if (comp != 0)
                    {
                        return comp;
                    }
                }
                catch
                {
                    // if something is not numeric then just do a string compare
                    return version1.CompareTo(version2);
                }
            }

            return 0;
        }

        //----------------------------------------------------------------
        // CompareReleaseVersions
        //----------------------------------------------------------------
        public static int CompareReleaseVersions(string version1, string version2)
        {
            if (string.IsNullOrEmpty(version2))
            {
                return 1;
            }
            if (string.IsNullOrEmpty(version1))
            {
                return -1;
            }
            if (version1.Equals(version2))
            {
                return 0;
            }
            // a purely string compare of the versions returns incorrect results when the numbers are a different order of magnitude
            string[] ver1 = version1.Split('.');
            string[] ver2 = version2.Split('.');
            try
            {
                int num1 = Convert.ToInt32(ver1[0]);
                int num2 = Convert.ToInt32(ver2[0]);

                int comp = num1.CompareTo(num2);
                if (comp != 0)
                {
                    return comp;
                }
            }
            catch
            {
                // if something is not numeric then just do a full string compare
                return version1.CompareTo(version2);
            }
            return 0;
        }

        //----------------------------------------------------------------
        // IncrementServerConfigVersion
        //----------------------------------------------------------------
        static public bool
           IncrementServerConfigVersion(
              SqlConnection conn,
              int serverId
           )
        {
            string cmdstr = "";


            try
            {
                SQLHelpers.CheckConnection(conn);

                cmdstr = GetIncrementSQL(serverId);

                int nRows;
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    nRows = cmd.ExecuteNonQuery();
                }
                if (nRows <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "IncrementServerConfigVersion", cmdstr, ex);
                errMsg = ex.Message;
                return false;
            }
        }

        //----------------------------------------------------------------
        // IncrementServerConfigVersion
        //----------------------------------------------------------------
        static public bool IncrementServerConfigVersion(SqlConnection conn, string serverName)
        {
            string cmdstr = "";


            try
            {
                SQLHelpers.CheckConnection(conn);

                cmdstr = GetIncrementSQL(serverName);

                int nRows;
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    nRows = cmd.ExecuteNonQuery();
                }
                if (nRows <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "IncrementServerConfigVersion", cmdstr, ex);
                errMsg = ex.Message;
                return false;
            }
        }

        //----------------------------------------------------------------
        // IncrementServerConfigVersionAll
        //----------------------------------------------------------------
        static public bool
           IncrementServerConfigVersionAll(
              SqlConnection conn
           )
        {
            string cmdstr = "";


            try
            {
                SQLHelpers.CheckConnection(conn);

                cmdstr = GetIncrementAllSQL();

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "IncrementServerConfigVersionAll", cmdstr, ex);
                errMsg = ex.Message;
                return false;
            }
        }


        //----------------------------------------------------------------
        // UpdateLastAgentContact
        //----------------------------------------------------------------
        static public void
           UpdateLastAgentContact(
              string instance
           )
        {
            Repository rep = null;

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                UpdateLastAgentContact(rep.connection, instance);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw e;
            }
            finally
            {
                if (rep != null) rep.CloseConnection();
            }
        }

        //----------------------------------------------------------------
        // UpdateLastAgentContact
        //----------------------------------------------------------------
        static public void
           UpdateLastAgentContact(
              bool serverOnly,
              string instance
           )
        {
            Repository rep = null;

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                UpdateLastAgentContact(rep.connection, serverOnly, instance);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw e;
            }
            finally
            {
                if (rep != null) rep.CloseConnection();
            }
        }

        //----------------------------------------------------------------
        // UpdateLastAgentContact
        //
        // update all instances sharing same agent that we successfully
        // talked to the agent on that machine
        //----------------------------------------------------------------
        static public void
           UpdateLastAgentContact(
              SqlConnection conn,
              string instance
           )
        {
            string cmdstr = "";


            try
            {
                SQLHelpers.CheckConnection(conn);

                string agent;
                cmdstr = String.Format("SELECT agentServer FROM {0}..{1} WHERE instance={2}",
                                              CoreConstants.RepositoryDatabase,
                                              CoreConstants.RepositoryServerTable,
                                              SQLHelpers.CreateSafeString(instance));

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj is DBNull)
                        return;
                    else
                        agent = (string)obj;
                }

                if (agent != "")
                {
                    cmdstr = String.Format("UPDATE {0}..{1} " +
                                                     "SET timeLastAgentContact=GETUTCDATE() " +
                                                     "WHERE agentServer={2};",
                                                  CoreConstants.RepositoryDatabase,
                                                  CoreConstants.RepositoryServerTable,
                                                  SQLHelpers.CreateSafeString(agent));
                    using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "UpdateLastAgentContact", cmdstr, ex);
                errMsg = ex.Message;
            }
        }

        //----------------------------------------------------------------
        // UpdateLastAgentContact
        //
        // update all instances sharing same agent that we successfully
        // talked to the agent on that machine
        //----------------------------------------------------------------
        static public void
           UpdateLastAgentContact(
              SqlConnection conn,
              bool serverOnly,
              string instance
           )
        {
            string cmdstr = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                string agent;
                if (serverOnly)
                {
                    agent = instance;
                }
                else
                {
                    cmdstr = String.Format("SELECT agentServer FROM {0}..{1} WHERE instance={2}",
                                                  CoreConstants.RepositoryDatabase,
                                                  CoreConstants.RepositoryServerTable,
                                                  SQLHelpers.CreateSafeString(instance));

                    using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                    {
                        object obj = cmd.ExecuteScalar();
                        if (obj is DBNull)
                            return;
                        else
                            agent = (string)obj;
                    }
                }

                if (agent != "")
                {
                    cmdstr = String.Format("UPDATE {0}..{1} " +
                                               "SET lastAgentContactTime=GETUTCDATE() " +
                                               "WHERE agentServer={2};",
                                             CoreConstants.RepositoryDatabase,
                                             CoreConstants.RepositoryServerTable,
                                             SQLHelpers.CreateSafeString(agent));
                    using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "UpdateLastAgentContact:2", cmdstr, ex);
                errMsg = ex.Message;
            }
        }

        //----------------------------------------------------------------
        // SetIsFlags
        //----------------------------------------------------------------
        static public void
           SetIsFlags(
             string instance,
             bool deployed,
             bool manuallyDeployed,
             bool running,
             bool crippled,
             SqlConnection conn
           )
        {
            string sql = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                sql = GetIsFlagsSQL(instance,
                                     deployed,
                                     manuallyDeployed,
                                     running,
                                     crippled);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "SetIsFlags", sql, ex);
                errMsg = ex.Message;
            }
        }

        //----------------------------------------------------------------
        // UpdateLastArchiveTime
        //----------------------------------------------------------------
        static public void
          UpdateLastArchiveTime(
             string instance
          )
        {
            Repository rep = null;
            string sql;

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                sql = String.Format("UPDATE {0}..{1} SET timeLastArchive=GETUTCDATE(),lastArchiveResult=1 WHERE instance={2}",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryServerTable,
                                     SQLHelpers.CreateSafeString(instance));
                using (SqlCommand cmd = new SqlCommand(sql, rep.connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw e;
            }
            finally
            {
                if (rep != null) rep.CloseConnection();
            }
        }


        //----------------------------------------------------------------
        // UpdateLastArchiveResult
        //----------------------------------------------------------------
        static public void
           UpdateLastArchiveResult(
              string instance,
              int archiveResult
           )
        {
            Repository rep = null;
            string sql;

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                sql = String.Format("UPDATE {0}..{1} SET lastArchiveResult={2} WHERE instance={3}",
                                     CoreConstants.RepositoryDatabase,
                                     CoreConstants.RepositoryServerTable,
                                     archiveResult,
                                     SQLHelpers.CreateSafeString(instance));
                using (SqlCommand cmd = new SqlCommand(sql, rep.connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw e;
            }
            finally
            {
                if (rep != null) rep.CloseConnection();
            }
        }

        //-------------------------------------------------------------------
        // DeleteServerRecord
        //-------------------------------------------------------------------
        static public void
           DeleteServerRecord(
              SqlConnection conn,
              string instance
           )
        {
            string cmdstr = "";

            try
            {
                SQLHelpers.CheckConnection(conn);

                cmdstr = String.Format("DELETE FROM {0} where instance={1};",
                                        CoreConstants.RepositoryServerTable,
                                        SQLHelpers.CreateSafeString(instance));
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "DeleteServerRecord", cmdstr, ex);
                errMsg = ex.Message;
            }
        }

        public static ServerRecord GetServerRecord(int serverId, SqlConnection connection)
        {
            var server = new ServerRecord();
            server.Connection = connection;
            server.Read(serverId);
            return server;
        }

        public static ServerRecord GetServerRecord(int serverId, string connectionString)
        {
            ServerRecord server = null;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                server = ServerRecord.GetServerRecord(serverId, connection);
            }

            return server;
        }

        #endregion

        #region Public Routines

        //-------------------------------------------------------------------
        // Clone
        //-------------------------------------------------------------------
        public ServerRecord
           Clone()
        {
            ServerRecord s = (ServerRecord)this.MemberwiseClone();

            return s;
        }

        //-------------------------------------------------------------------
        // Read
        //-------------------------------------------------------------------
        public bool
           Read(
              int serverId
           )
        {
            string where = String.Format("WHERE srvId={0}",
                                           serverId);
            return InternalRead(where);
        }

        //-------------------------------------------------------------------
        // Read
        //-------------------------------------------------------------------
        public bool
           Read(
              string serverInstance, SqlTransaction transaction = null
           )
        {
            string where = String.Format("WHERE instance={0}",
                                           SQLHelpers.CreateSafeString(serverInstance));
            return InternalRead(where, transaction);
        }

        static public bool
           ServerIsAudited(
              string serverInstance,
              SqlConnection conn
           )
        {
            bool isAudited = false;

            try
            {
                SQLHelpers.CheckConnection(conn);

                string sql = String.Format("SELECT instance FROM {0} WHERE isAuditedServer=1 AND instance={1}",
                                           CoreConstants.RepositoryServerTable,
                                           SQLHelpers.CreateSafeString(serverInstance));
                using (SqlCommand selectCmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isAudited = true;
                        }
                    }
                }
            }
            catch
            {
            }

            return isAudited;
        }

        //-------------------------------------------------------------------
        // UpdateDbRecord - Updates existing record
        //-------------------------------------------------------------------
        public bool
            UpdateDbRecord(
            ServerRecord server,
            bool isRolePrimary
           )
        {
            bool isRoleUpdated = false;

            try
            {
                int nRows;
                SQLHelpers.CheckConnection(server.Connection);
                int isPrimary = isRolePrimary ? 1 : 0;

                string sql = String.Format("UPDATE {0} SET isPrimary={1}  WHERE isAlwaysOn=1 AND srvInstance={2}",
                                           CoreConstants.RepositoryDatabaseTable,
                                           isPrimary,
                                           SQLHelpers.CreateSafeString(server.Instance));
                using (SqlCommand cmd = new SqlCommand(sql, server.Connection))
                {
                    nRows = cmd.ExecuteNonQuery();
                }

                if (nRows > 0)
                {
                    isRoleUpdated = true;
                }
            }

            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                           "Error updating database records for database roles",
                           ex,
                           ErrorLog.Severity.Error);
                errMsg = ex.Message;

                throw ex;
            }

            return isRoleUpdated;
        }

        public bool UpdateXESetting(SqlTransaction transaction)
        {
            bool retval = false;
            StringBuilder sqlCmd = new StringBuilder("");
            this.timeLastModified = DateTime.UtcNow;
            sqlCmd.Append(getUpdateXESettingSQL());
            try
            {
                int nRows;
                using (SqlCommand cmd = new SqlCommand(sqlCmd.ToString(),
                                                          connection))
                {
                    if (transaction != null)
                    {
                        cmd.Transaction = transaction;
                    }

                    nRows = cmd.ExecuteNonQuery();
                }
                if (nRows > 0)
                {
                    retval = true;
                }
            }

            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return retval;

        }

        public string getUpdateXESettingSQL()
        {
            StringBuilder prop = new StringBuilder("");

            AddUpdateProp(ref prop, "auditTrace", false, (this.auditTrace) ? 1 : 0);
            AddUpdateProp(ref prop, "auditCaptureSQLXE", false, (this.auditCaptureSQLXE) ? 1 : 0);

            StringBuilder tmp = new StringBuilder("");
            prop.Append(",timeLastModified = GETUTCDATE()");
            tmp.AppendFormat("UPDATE {0} SET ",
                                CoreConstants.RepositoryServerTable);
            tmp.Append(prop.ToString());
            tmp.AppendFormat(" WHERE instance={0};",
                                SQLHelpers.CreateSafeString(this.instance));
            return tmp.ToString();
        }


        //-------------------------------------------------------------------
        // Write - Updates existing record
        //-------------------------------------------------------------------
        public bool
          Write()
        {
            this.timeLastModified = DateTime.UtcNow;

            return InternalWrite(false /* dont create */,
                                  null,
                                  null);
        }

        //-------------------------------------------------------------------
        // Write - Updates existing record
        //-------------------------------------------------------------------
        public bool
           Write(
              ServerRecord oldServerRec
           )
        {
            this.timeLastModified = DateTime.UtcNow;

            return InternalWrite(false /* dont create */,
                                  oldServerRec,
                                  null);
        }

        //-------------------------------------------------------------------
        // Write - Updates existing record
        //-------------------------------------------------------------------
        public bool
           Write(
              ServerRecord oldServerRec,
              SqlTransaction transaction
          )
        {
            this.timeLastModified = DateTime.UtcNow;

            return InternalWrite(false /* dont create */,
                                  oldServerRec,
                                  transaction);
        }

        //-------------------------------------------------------------------
        // Create
        //-------------------------------------------------------------------
        public bool
           Create(
              SqlTransaction transaction
          )
        {
            bool retval;

            this.timeCreated = DateTime.UtcNow;
            this.timeLastModified = this.timeCreated;
            this.timeEnabledModified = this.timeCreated;

            retval = InternalWrite(true /* create */,
                                    null,
                                    transaction);
            if (retval)
            {
                //after write - we need to read and get generated serverId!
                this.srvId = GetServerId(connection,
                                          this.instance);
            }

            return retval;
        }

        //-------------------------------------------------------------------
        // EnableServer
        //-------------------------------------------------------------------
        public bool
           EnableServer(
              bool enableAuditing
           )
        {
            bool retval = false;

            string sqlCmd = GetEnableSQL(enableAuditing);

            try
            {
                int nRows;
                using (SqlCommand cmd = new SqlCommand(sqlCmd, connection))
                {
                    nRows = cmd.ExecuteNonQuery();
                }
                if (nRows > 0)
                {
                    this.IsEnabled = enableAuditing;
                    this.timeEnabledModified = DateTime.UtcNow;

                    retval = true;
                }
                else
                {
                    throw new Exception("No matching instances in the Repository");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "EnableServer", sqlCmd, ex);
                errMsg = ex.Message;
            }

            return retval;
        }

        //-------------------------------------------------------------------
        // Delete
        //
        // Deletes all the following
        //    Servers         table entry
        //    DatabaseObjects table entry
        //    Databases       table entry
        //    SystemDatabases table entry
        //    events database for server
        //    All Archive Databases for Server
        //    ReportCard      table entry for Server
        //-------------------------------------------------------------------
        public void
           Delete()
        {
            string cmd = "";
            SqlTransaction transaction;

            using (transaction = connection.BeginTransaction())
            {
                try
                {
                    // delete server record
                    cmd = GetDeleteSQL();
                    ExecuteSQL(cmd, transaction);

                    // delete threshold settings
                    cmd = GetDeleteThresholdSQL();
                    ExecuteSQL(cmd, transaction);

                    // delete traces of server from audit tables
                    DeleteAuditData(transaction);

                    transaction.Commit();
                    transaction = null;
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                        "Error deleting registered SQL Server: " + this.instance,
                        cmd,
                        ex,
                                             ErrorLog.Severity.Error);
                    errMsg = ex.Message;

                    throw ex;
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        //-------------------------------------------------------------------
        // DeleteAuditData
        //
        // Deletes all the following
        //    Servers         table entry
        //    DatabaseObjects table entry
        //    Databases       table entry
        //    SystemDatabases table entry
        //-------------------------------------------------------------------
        public void
           DeleteAuditData(
              SqlTransaction inTransaction
           )
        {
            string cmd = "";
            SqlTransaction transaction = null;

            try
            {
                if (inTransaction == null)
                {
                    transaction = connection.BeginTransaction();
                }
                else
                {
                    transaction = inTransaction;
                }

                cmd = GetDeleteSystemDatabaseSQL();
                ExecuteSQL(cmd, transaction);

                cmd = GetDeleteDatabaseObjectsSQL();
                ExecuteSQL(cmd, transaction);

                cmd = GetDeleteDatabasesSQL();
                ExecuteSQL(cmd, transaction);

                if (inTransaction == null)
                {
                    transaction.Commit();
                    transaction = null;
                }
            }
            catch (Exception ex)
            {
                if (inTransaction == null)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             "Error deleting audit databases for registered SQL Server: " + this.instance,
                                             cmd,
                                             ex,
                                             ErrorLog.Severity.Error);
                }
                errMsg = ex.Message;

                throw ex;
            }
            finally
            {
                if (inTransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
            }
        }



        //-------------------------------------------------------------------
        // DeleteEventsDatabase
        //-------------------------------------------------------------------
        public void
           DeleteEventsDatabase()
        {
            string cmd = "";

            try
            {
                if (!string.IsNullOrEmpty(eventDatabase))
                {
                    string databaseToDelete = SQLHelpers.CreateSafeDatabaseName(eventDatabase);

                    SqlConnection explicitConnection = new SqlConnection();
                    explicitConnection.ConnectionString = connection.ConnectionString;
                    explicitConnection.Open();

                    explicitConnection.ChangeDatabase("master");

                    SqlCommand explicitCommand = new SqlCommand();
                    explicitCommand.Connection = explicitConnection;
                    explicitCommand.CommandTimeout = CoreConstants.sqlcommandTimeout;
                    explicitCommand.CommandType = CommandType.Text;

                    explicitCommand.CommandText = string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", databaseToDelete);
                    explicitCommand.ExecuteNonQuery();

                    explicitCommand.CommandText = string.Format("DROP DATABASE {0}", databaseToDelete);
                    explicitCommand.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number != 3701 /* Database not found */ )
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                               "Error deleting Events Database for SQL Server: " + this.instance,
                                               cmd,
                                               sqlEx,
                                             ErrorLog.Severity.Error);
                    errMsg = sqlEx.Message;

                    throw sqlEx;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "Error deleting Events Database for SQL Server: " + this.instance,
                                         cmd,
                                         ex,
                                         ErrorLog.Severity.Error);
                errMsg = ex.Message;

                throw ex;
            }
        }

        //-------------------------------------------------------------------
        // DeleteChangeLogs
        //-------------------------------------------------------------------
        public void
           DeleteChangeLogs()
        {
            string cmd = "";

            try
            {
                cmd = String.Format("DELETE FROM {0}..{1} WHERE logSqlServer={2}",
                                      CoreConstants.RepositoryDatabase,
                                      CoreConstants.RepositoryChangeLogEventTable,
                                      SQLHelpers.CreateSafeString(this.instance));
                ExecuteSQL(cmd, null);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "Error deleting Change Logs events for SQL Server: " + this.instance,
                                         cmd,
                                         ex,
                                         ErrorLog.Severity.Error);
                errMsg = ex.Message;

                throw ex;
            }
        }

        //-------------------------------------------------------------------
        // ExecuteSQL
        //-------------------------------------------------------------------
        private void
           ExecuteSQL(
              string sql,
              SqlTransaction transaction
           )
        {
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }
                cmd.ExecuteNonQuery();
            }
        }

        //-------------------------------------------------------------
        // GetServer
        //--------------------------------------------------------------
        static public ServerRecord
           GetServer(
              SqlConnection conn,
              string serverName
           )
        {
            ServerRecord config = null;

            try
            {
                SQLHelpers.CheckConnection(conn);

                string cmdstr = GetSelectSQL(String.Format("WHERE instance='{0}'", serverName), "");

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            config = new ServerRecord();
                            config.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(String.Format("Error loading server: {0}", ex.Message));
                errMsg = ex.Message;
                config = null;
            }

            return config;
        }

        //-------------------------------------------------------------
        // GetServer
        //--------------------------------------------------------------
        static public ServerRecord
           GetServer(
              SqlConnection conn,
              int serverId
           )
        {
            ServerRecord config = null;

            try
            {
                SQLHelpers.CheckConnection(conn);

                string cmdstr = GetSelectSQL(String.Format("WHERE srvId={0}", serverId), "");

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            config = new ServerRecord();
                            config.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(String.Format("Error loading server: {0}", ex.Message));
                errMsg = ex.Message;
                config = null;
            }

            return config;
        }

        //-----------------------------------------------------------------------------
        // GetServers - Return a collection of registered servers 
        //-----------------------------------------------------------------------------
        static public List<ServerRecord>
           GetServers(
             SqlConnection conn,
             bool auditServersOnly
           )
        {
            return GetServers(conn, auditServersOnly, false);
        }

        static public List<ServerRecord>
            GetServers(
              SqlConnection conn,
              bool auditServersOnly,
              bool throwException
            )
        {
            List<ServerRecord> serverList = new List<ServerRecord>();

            try
            {
                SQLHelpers.CheckConnection(conn);

                string cmdstr = GetSelectSQL((auditServersOnly) ? "WHERE isAuditedServer=1" : "",
                                                           "ORDER BY instance ASC");

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ServerRecord config = new ServerRecord();
                            config.Load(reader);

                            serverList.Add(config);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "GetServers",
                                         ex);

                Debug.Write(String.Format("Error loading list: {0}", ex.Message));
                errMsg = ex.Message;

                if (throwException) throw ex;
            }

            return serverList;
        }

        static public List<string> GetAllServers(SqlConnection conn)
        {
            List<string> servers = new List<string>();

            try
            {
                SQLHelpers.CheckConnection(conn);

                string cmdstr = "[SQLcompliance]..[sp_cmreport_GetInstances]";

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            servers.Add(SQLHelpers.GetString(reader, 0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return servers;
        }

        //-----------------------------------------------------------------------------
        // GetDatabaseAlwaysOnDetails - Gets AlwaysOn Configuration details from the repository DB
        //-----------------------------------------------------------------------------
        static public IList
           GetDatabaseAlwaysOnDetails(
              SqlConnection conn,
              string instance
           )
        {
            IList dbList = null;
            DatabaseAlwaysOnDetails raw = null;

            try
            {
                if (instance == null || instance.Length <= 0)
                    return dbList;

                // Load Availability Group Details for the databases from Repository..Database
                string cmdstr = String.Format("Select dbId, srvId, srvInstance, sqlDatabaseId, name, isAlwaysOn, replicaServers, " +
                                             " isPrimary, availGroupName FROM {0}..{1} " +
                                             " where isAlwaysOn = 1 and srvInstance = '{2}'",
                                             CoreConstants.RepositoryDatabase,
                                             CoreConstants.RepositoryDatabaseTable,
                                             instance);

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dbList = new ArrayList();
                        while (reader.Read())
                        {
                            raw = new DatabaseAlwaysOnDetails();
                            raw.dbId = reader.GetInt32(0);
                            raw.srvId = reader.GetInt32(1);
                            raw.srvInstance = reader.GetString(2);
                            raw.sqlDatabaseId = reader.GetInt16(3);
                            raw.dbName = reader.GetString(4);
                            raw.isAlwaysOn = reader.GetByte(5);
                            raw.replicaServers = reader.GetString(6);
                            raw.isPrimary = reader.GetByte(7);
                            raw.availGroupName = reader.GetString(8);
                            dbList.Add(raw);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dbList;
        }

        //-----------------------------------------------------------------------------
        // UpdateAlwaysOnRole - Update the AlwaysOn Role to Primary for Failover in the repository
        // Also update the role to secondary for others, only one node can be primary at a time.
        //-----------------------------------------------------------------------------
        static public void
           UpdateAlwaysOnRole(
              SqlConnection conn,
              List<DatabaseAlwaysOnDetails> dbList
           )
        {
            try
            {
                if (dbList == null || dbList.Count <= 0)
                    return;

                foreach (DatabaseAlwaysOnDetails dbAlwaysOnDetails in dbList)
                {
                    // Load Availability Group Details for the databases from Repository..Database
                    string cmdstr = String.Format("update {0}..{1} set isPrimary = 0 " +
                                                 " where isAlwaysOn = 1 and availGroupName = '{2}' " +
                                                 " and replicaServers = '{3}' and name = '{4}';",
                                                 CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable,
                                                 dbAlwaysOnDetails.availGroupName, dbAlwaysOnDetails.replicaServers,
                                                 dbAlwaysOnDetails.dbName);
                    using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        cmd.ExecuteNonQuery();
                    }

                    cmdstr = String.Format("update {0}..{1} set isPrimary = 1 " +
                                                 " where isAlwaysOn = 1 and availGroupName = '{2}' and replicaServers = '{3}' " +
                                                 " and sqlDatabaseId = {4} and srvId = {5}",
                                                  CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable,
                                                  dbAlwaysOnDetails.availGroupName, dbAlwaysOnDetails.replicaServers,
                                                  dbAlwaysOnDetails.sqlDatabaseId, dbAlwaysOnDetails.srvId);
                    using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                    {
                        cmd.CommandTimeout = CoreConstants.sqlcommandTimeout;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return;
        }

        //v5.6 SQLCM-5373
        static public List<DatabaseRecord> GetDatabasesinSQLInstance(int srvId, SqlConnection conn, SqlTransaction transaction = null)
        {
            List<DatabaseRecord> dbList = new List<DatabaseRecord>();
            try
            {
                string cmdStr = String.Format("select dbId, srvId, srvInstance, name, auditUsersList, auditUserAll, auditUserSELECT from {0} where  srvId = {1} and isEnabled = {2}",
                                                  CoreConstants.RepositoryDatabaseTable, srvId, 1);
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            DatabaseRecord db = new DatabaseRecord();
                            db.DbId = reader.GetInt32(0);
                            db.SrvId = reader.GetInt32(1);
                            db.SrvInstance = reader.GetString(2);
                            db.Name = reader.GetString(3);
                            db.AuditUsersList = reader.GetString(4);
                            db.AuditUserAll = SQLHelpers.ByteToBool(reader, 5);
                            db.AuditUserSELECT = SQLHelpers.ByteToBool(reader, 6);
                            dbList.Add(db);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "An error occurred retrieving databases of a SQL instance.",
                                         ex);
                throw ex;
            }
            return dbList;
        }

        //-----------------------------------------------------------------------------
        // GetDatabasesWithInAVG - Gets the list of databases involved in the current AVG from the repository DB
        //-----------------------------------------------------------------------------
        static public IList
           GetDatabasesWithInAVG(
              SqlConnection conn,
              string dbName,
              string replicaServers,
              string availGroupName
           )
        {
            IList dbList = null;
            DatabaseAlwaysOnDetails raw = null;

            try
            {
                // Load Availability Group Details for the databases from Repository..Database
                string cmdstr = String.Format("Select dbId, srvId, srvInstance, sqlDatabaseId, name, isAlwaysOn, replicaServers, " +
                                             " isPrimary, availGroupName FROM {0}..{1} " +
                                             " where isAlwaysOn = 1 and name = {2} and availGroupName = {3} and replicaServers = {4} ",
                                             CoreConstants.RepositoryDatabase,
                                             CoreConstants.RepositoryDatabaseTable,
                                             SQLHelpers.CreateSafeString(dbName), SQLHelpers.CreateSafeString(availGroupName),
                                             SQLHelpers.CreateSafeString(replicaServers));

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dbList = new ArrayList();
                        while (reader.Read())
                        {
                            raw = new DatabaseAlwaysOnDetails();
                            raw.dbId = reader.GetInt32(0);
                            raw.srvId = reader.GetInt32(1);
                            raw.srvInstance = reader.GetString(2);
                            raw.sqlDatabaseId = reader.GetInt16(3);
                            raw.dbName = reader.GetString(4);
                            raw.isAlwaysOn = reader.GetByte(5);
                            raw.replicaServers = reader.GetString(6);
                            raw.isPrimary = reader.GetByte(7);
                            raw.availGroupName = reader.GetString(8);
                            dbList.Add(raw);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "An error occurred retrieving CLR Enabled value.",
                                         ex);
                throw ex;
            }

            return dbList;
        }

        //-----------------------------------------------------------------------------
        // CountAuditedServers
        //-----------------------------------------------------------------------------
        static public int CountAuditedServers(SqlConnection connection)
        {
            int count = 0;

            try
            {
                SQLHelpers.CheckConnection(connection);

                string cmdstr = String.Format("SELECT count(*) from [{0}]..{1} WHERE isAuditedServer=1",
                                                CoreConstants.RepositoryDatabase,
                                                CoreConstants.RepositoryServerTable);
                using (SqlCommand cmd = new SqlCommand(cmdstr, connection))
                {
                    Object obj = cmd.ExecuteScalar();
                    if (obj is DBNull)
                    {
                        count = 0;
                    }
                    else
                    {
                        count = (int)obj;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "CountAuditedServersExceptVirtuals", ex);
            }

            return count;
        }

        static public Tuple<int, int> CountAlwaysOnReplicaServersForVirtualServers(SqlConnection connection)
        {
            var alwaysOnVirtualServerIdList = new HashSet<int>();
            var uniqueReplicaServerList = new HashSet<string>();

            foreach (DatabaseRecord alwaysOnDatabase in DatabaseRecord.GetDatabases(connection, "s.sqlVersion >= 1000 AND d.isAlwaysOn = 1 AND d.replicaServers IS NOT NULL"))
            {
                var replicas = new List<string>(alwaysOnDatabase.ReplicaServers.Split(new string[] { COMMA_CHARACTER }, StringSplitOptions.RemoveEmptyEntries));
                replicas.ForEach((replica) => uniqueReplicaServerList.Add(replica));
                alwaysOnVirtualServerIdList.Add(alwaysOnDatabase.SrvId);
            }

            return new Tuple<int, int>(alwaysOnVirtualServerIdList.Count, uniqueReplicaServerList.Count);
        }

        static public List<string> GetReplicaInstancesOfServer(ServerRecord server, SqlConnection connection)
        {
            var uniqueReplicaServerList = new HashSet<string>();

            foreach (DatabaseRecord alwaysOnDatabase in DatabaseRecord.GetDatabases(connection, string.Format("d.srvId = {0} AND d.isAlwaysOn = 1 AND d.replicaServers IS NOT NULL", server.SrvId)))
            {
                var replicas = new List<string>(alwaysOnDatabase.ReplicaServers.Split(new string[] { COMMA_CHARACTER }, StringSplitOptions.RemoveEmptyEntries));
                replicas.ForEach((replica) => uniqueReplicaServerList.Add(replica));
            }

            return new List<string>(uniqueReplicaServerList);
        }

        //-------------------------------------------------------------
        // Load - ServerRecord from SqlDataReader
        //--------------------------------------------------------------
        private void
           Load(
              SqlDataReader reader
           )
        {
            int col = 0;

            srvId = SQLHelpers.GetInt32(reader, col++);
            instance = SQLHelpers.GetString(reader, col++);
            sqlVersion = SQLHelpers.GetInt32(reader, col++);
            description = SQLHelpers.GetString(reader, col++);

            isDeployed = SQLHelpers.ByteToBool(reader, col++);
            deployedByCommand = SQLHelpers.ByteToInt(reader, col);
            isDeployedManually = SQLHelpers.ByteToBool(reader, col++);
            isUpdateRequested = SQLHelpers.ByteToBool(reader, col++);
            isAuditedServer = SQLHelpers.ByteToBool(reader, col++);

            isRunning = SQLHelpers.ByteToBool(reader, col++);
            isCrippled = SQLHelpers.ByteToBool(reader, col++);
            isEnabled = SQLHelpers.ByteToBool(reader, col++);
            isSqlSecureDb = SQLHelpers.ByteToBool(reader, col++);
            isOnRepositoryHost = SQLHelpers.ByteToBool(reader, col++);

            timeLastAgentContact = SQLHelpers.GetDateTime(reader, col++);
            timeLastHeartbeat = SQLHelpers.GetDateTime(reader, col++);
            timeLastCollection = SQLHelpers.GetDateTime(reader, col++);

            eventDatabase = SQLHelpers.GetString(reader, col++);
            defaultAccess = SQLHelpers.GetInt32(reader, col++);
            maxSqlLength = SQLHelpers.GetInt32(reader, col++);

            auditLogins = SQLHelpers.ByteToBool(reader, col++);
            auditFailedLogins = SQLHelpers.ByteToBool(reader, col++);

            auditDDL = SQLHelpers.ByteToBool(reader, col++);
            auditSecurity = SQLHelpers.ByteToBool(reader, col++);
            auditAdmin = SQLHelpers.ByteToBool(reader, col++);
            auditDML = SQLHelpers.ByteToBool(reader, col++);
            auditSELECT = SQLHelpers.ByteToBool(reader, col++);
            auditTrace = SQLHelpers.ByteToBool(reader, col++);
            auditExceptions = SQLHelpers.ByteToBool(reader, col++);

            auditAccessCheck = SQLHelpers.ByteToInt(reader, col++);
            auditSystemEvents = SQLHelpers.ByteToBool(reader, col++);
            auditCaptureSQL = SQLHelpers.ByteToBool(reader, col++);
            auditUDE = SQLHelpers.ByteToBool(reader, col++);

            timeCreated = SQLHelpers.GetDateTime(reader, col++);
            timeLastModified = SQLHelpers.GetDateTime(reader, col++);
            timeEnabledModified = SQLHelpers.GetDateTime(reader, col++);

            configVersion = SQLHelpers.GetInt32(reader, col++);
            lastKnownConfigVersion = SQLHelpers.GetInt32(reader, col++);
            lastConfigUpdate = SQLHelpers.GetDateTime(reader, col++);
            configUpdateRequested = SQLHelpers.ByteToBool(reader, col++);

            auditUsersList = SQLHelpers.GetString(reader, col++);
            auditUserAll = SQLHelpers.ByteToBool(reader, col++);
            auditUserLogins = SQLHelpers.ByteToBool(reader, col++);
            auditUserFailedLogins = SQLHelpers.ByteToBool(reader, col++);

            auditUserDDL = SQLHelpers.ByteToBool(reader, col++);
            auditUserSecurity = SQLHelpers.ByteToBool(reader, col++);
            auditUserAdmin = SQLHelpers.ByteToBool(reader, col++);
            auditUserDML = SQLHelpers.ByteToBool(reader, col++);
            auditUserSELECT = SQLHelpers.ByteToBool(reader, col++);

            auditUserAccessCheck = SQLHelpers.ByteToInt(reader, col++);
            auditUserCaptureSQL = SQLHelpers.ByteToBool(reader, col++);
            auditUserCaptureTrans = SQLHelpers.ByteToBool(reader, col++);
            auditUserExceptions = SQLHelpers.ByteToBool(reader, col++);
            auditUserUDE = SQLHelpers.ByteToBool(reader, col++);

            bias = SQLHelpers.GetInt32(reader, col++);
            standardBias = SQLHelpers.GetInt32(reader, col++);
            standardDate = SQLHelpers.GetDateTime(reader, col++);
            daylightBias = SQLHelpers.GetInt32(reader, col++);
            daylightDate = SQLHelpers.GetDateTime(reader, col++);

            instanceServer = SQLHelpers.GetString(reader, col++);

            agentServer = SQLHelpers.GetString(reader, col++);
            agentPort = SQLHelpers.GetInt32(reader, col++);
            agentVersion = SQLHelpers.GetString(reader, col++);
            agentServiceAccount = SQLHelpers.GetString(reader, col++);

            agentMaxTraceSize = SQLHelpers.GetInt32(reader, col++);
            agentTraceDirectory = SQLHelpers.GetString(reader, col++);
            agentHeartbeatInterval = SQLHelpers.GetInt32(reader, col++);
            agentCollectionInterval = SQLHelpers.GetInt32(reader, col++);
            agentDetectionInterval = SQLHelpers.GetInt32(reader, col++);
            if (agentDetectionInterval == 0)
                agentDetectionInterval = CoreConstants.Agent_Default_TamperingDetectionInterval;

            agentForceCollectionInterval = SQLHelpers.GetInt32(reader, col++);
            agentLogLevel = SQLHelpers.GetInt32(reader, col++);
            agentTraceOptions = (TraceOption)SQLHelpers.GetInt32(reader, col++);
            agentMaxFolderSize = SQLHelpers.GetInt32(reader, col++);
            agentMaxUnattendedTime = SQLHelpers.GetInt32(reader, col++);

            containsBadEvents = SQLHelpers.ByteToBool(reader, col++);
            eventReviewNeeded = SQLHelpers.ByteToBool(reader, col++);

            if (!reader.IsDBNull(col))
                highWatermark = reader.GetInt32(col);
            else
                highWatermark = -2100000000;
            col++;

            if (!reader.IsDBNull(col))
                lowWatermark = reader.GetInt32(col);
            else
                lowWatermark = -2100000000;
            col++;

            timeLastIntegrityCheck = SQLHelpers.GetDateTime(reader, col++);
            lastIntegrityCheckResult = SQLHelpers.GetInt32(reader, col++);
            timeLastArchive = SQLHelpers.GetDateTime(reader, col++);
            lastArchiveResult = SQLHelpers.GetInt32(reader, col++);
            agentHealth = SQLHelpers.GetULong(reader, col++);
            agentTraceStartTimeout = SQLHelpers.GetInt32(reader, col++);

            if (agentTraceStartTimeout == 0)
                agentTraceStartTimeout = CoreConstants.Agent_Default_TraceStartTimeout;

            auditUserCaptureDDL = SQLHelpers.ByteToBool(reader, col++);
            _sqlVersionName = SQLHelpers.GetString(reader, col++);
            _isCluster = SQLHelpers.ByteToBool(reader, col++);
            _isHadrEnabled = SQLHelpers.ByteToBool(reader, col++);
            auditUserExtendedEvents = SQLHelpers.ByteToBool(reader, col++); //SQLCm 5.4_4.1.1_Extended Events 
            auditCaptureSQLXE = SQLHelpers.ByteToBool(reader, col++); //SQLCm 5.4_4.1.1_Extended Events 
            isAuditLogEnabled = SQLHelpers.ByteToBool(reader, col++);
            auditLogouts = SQLHelpers.ByteToBool(reader, col++);
            auditUserLogouts = SQLHelpers.ByteToBool(reader, col++);  // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
            auditTrustedUsersList = SQLHelpers.GetString(reader, col++); //v5.6 SQLCM-5373

            //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
            countDatabasesAuditingAllObjects = SQLHelpers.GetInt32(reader, col++);
            addNewDatabasesAutomatically = SQLHelpers.ByteToBool(reader, col++);//SQLCM -541/4876/5259 v5.6
            lastNewDatabasesAddTime = SQLHelpers.GetDateTime(reader, col++);
            isCompressedFile = SQLHelpers.ByteToBool(reader, col++);

        }

        //-------------------------------------------------------------
        // Match - Decides if two DatabaseRecords match
        //         Used by properties dialog to decide if write is needed
        //--------------------------------------------------------------
        public static bool
         Match(
            ServerRecord s1,
            ServerRecord s2
         )
        {
            if (s1 == null || s2 == null)
                return false;

            // General Properties            
            if (s2.instance != s1.instance) return false;
            if (s2.description != s1.description) return false;

            if (s1.IsAuditedServer)
            {
                if (s2.isDeployed != s1.isDeployed) return false;
                if (s2.isDeployedManually != s1.isDeployedManually) return false;
                if (s2.isUpdateRequested != s1.isUpdateRequested) return false;
                if (s2.isEnabled != s1.isEnabled) return false;

                if (s2.maxSqlLength != s1.maxSqlLength) return false;
                if (s2.defaultAccess != s1.defaultAccess) return false;

                // Audit Settings
                if (s2.auditLogins != s1.auditLogins) return false;
                if (s2.auditLogouts != s1.auditLogouts) return false;
                if (s2.auditFailedLogins != s1.auditFailedLogins) return false;
                if (s2.auditDDL != s1.auditDDL) return false;
                if (s2.auditSecurity != s1.auditSecurity) return false;
                if (s2.auditAdmin != s1.auditAdmin) return false;
                if (s2.auditDML != s1.auditDML) return false;
                if (s2.auditSELECT != s1.auditSELECT) return false;
                if (s2.auditTrace != s1.auditTrace) return false;
                if (s2.auditExceptions != s1.auditExceptions) return false;
                if (s2.auditAccessCheck != s1.auditAccessCheck) return false;
                if (s2.auditSystemEvents != s1.auditSystemEvents) return false;
                if (s2.auditCaptureSQL != s1.auditCaptureSQL) return false;
                if (s2.auditUserCaptureDDL != s1.auditUserCaptureDDL) return false;
                if (s2.auditUDE != s1.auditUDE) return false;
                if (s2.auditCaptureSQLXE != s1.auditCaptureSQLXE) return false; //5.4 XE

                // Privileged Users
                // Compare this way to detect "" and serialized empty lists properly.
                if (!UserList.Match(s2.auditUsersList, s1.auditUsersList)) return false;
                if (!UserList.Match(s2.auditTrustedUsersList, s1.auditTrustedUsersList)) return false; //v5.6 SQLCM-5373
                                                                                                       //if ( s2.auditUsersList != s1.auditUsersList ) return false;
                if (s2.auditUserAll != s1.auditUserAll) return false;
                if (s2.auditUserLogins != s1.auditUserLogins) return false;
                if (s2.auditUserLogouts != s1.auditUserLogouts) return false;
                if (s2.auditUserFailedLogins != s1.auditUserFailedLogins) return false;
                if (s2.auditUserDDL != s1.auditUserDDL) return false;
                if (s2.auditUserSecurity != s1.auditUserSecurity) return false;
                if (s2.auditUserAdmin != s1.auditUserAdmin) return false;
                if (s2.auditUserDML != s1.auditUserDML) return false;
                if (s2.auditUserSELECT != s1.auditUserSELECT) return false;
                if (s2.auditUserExtendedEvents != s1.auditUserExtendedEvents) return false; //SQLCm 5.4_4.1.1_Extended Events 
                if (s2.auditUserAccessCheck != s1.auditUserAccessCheck) return false;
                if (s2.auditUserCaptureSQL != s1.auditUserCaptureSQL) return false;
                if (s2.auditUserCaptureTrans != s1.auditUserCaptureTrans) return false;
                if (s2.auditUserExceptions != s1.auditUserExceptions) return false;
                if (s2.auditUserUDE != s1.auditUserUDE) return false;

                // Agent Settings
                if (s2.agentPort != s1.agentPort) return false;
                if (s2.agentServiceAccount != s1.agentServiceAccount) return false;
                if (s2.agentMaxTraceSize != s1.agentMaxTraceSize) return false;
                if (s2.agentTraceDirectory != s1.agentTraceDirectory) return false;
                if (s2.agentHeartbeatInterval != s1.agentHeartbeatInterval) return false;
                if (s2.agentCollectionInterval != s1.agentCollectionInterval) return false;
                if (s2.agentDetectionInterval != s1.agentDetectionInterval) return false;
                if (s2.agentForceCollectionInterval != s1.agentForceCollectionInterval) return false;
                if (s2.agentLogLevel != s1.agentLogLevel) return false;
                if (s2.agentTraceOptions != s1.agentTraceOptions) return false;
                if (s2.agentMaxFolderSize != s1.agentMaxFolderSize) return false;
                if (s2.agentMaxUnattendedTime != s1.agentMaxUnattendedTime) return false;
                if (s2.agentTraceStartTimeout != s1.agentTraceStartTimeout) return false;
                if (s2.isAuditLogEnabled != s1.isAuditLogEnabled) return false; // 5.5 Audit Logs

                //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
                if (s2.countDatabasesAuditingAllObjects != s1.countDatabasesAuditingAllObjects) return false;
                //SQLCM -541/4876/5259 v5.6
                if (s2.addNewDatabasesAutomatically != s1.addNewDatabasesAutomatically) return false;

                if (s2.lastNewDatabasesAddTime != s1.lastNewDatabasesAddTime) return false;

                if (s2.IsCompressedFile != s1.IsCompressedFile) return false;
            }

            // if we got here nothing changed
            return true;
        }

        //------------------------------------------------------------------
        // ReadTraceJobInfo
        //------------------------------------------------------------------
        static public bool ReadTraceJobInfo(
              string instance,
              SqlConnection conn,
              out TimeZoneInfo outTimeZoneInfo,
              out string outEventDatabase,
              out int outMaxSql,
              out int outSqlVersion
           )
        {
            string cmdstr;
            int bias;
            int standardBias;
            DateTime standardDate;
            int daylightBias;
            DateTime daylightDate;

            try
            {
                SQLHelpers.CheckConnection(conn);

                cmdstr = String.Format("SELECT eventDatabase,bias,standardBias,standardDate,daylightBias,daylightDate,maxSqlLength, sqlVersion " +
                                        "FROM {0}..{1} WHERE instance={2}",
                                        CoreConstants.RepositoryDatabase,
                                        CoreConstants.RepositoryServerTable,
                                        SQLHelpers.CreateSafeString(instance));
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int col = 0;
                            outEventDatabase = SQLHelpers.GetString(reader, col++);
                            bias = SQLHelpers.GetInt32(reader, col++);
                            standardBias = SQLHelpers.GetInt32(reader, col++);
                            standardDate = SQLHelpers.GetDateTime(reader, col++);
                            daylightBias = SQLHelpers.GetInt32(reader, col++);
                            daylightDate = SQLHelpers.GetDateTime(reader, col++);
                            outMaxSql = SQLHelpers.GetInt32(reader, col++);
                            outSqlVersion = SQLHelpers.GetInt32(reader, col++) % 1000;

                            TimeZoneStruct tzs = new TimeZoneStruct();
                            tzs.Bias = bias;
                            tzs.StandardBias = standardBias;
                            tzs.StandardDate = SystemTime.FromTimeZoneDateTime(standardDate);
                            tzs.DaylightBias = daylightBias;
                            tzs.DaylightDate = SystemTime.FromTimeZoneDateTime(daylightDate);
                            outTimeZoneInfo = new TimeZoneInfo();
                            outTimeZoneInfo.TimeZoneStruct = tzs;
                            return true;
                        }
                        else
                        {
                            outTimeZoneInfo = null;
                            outEventDatabase = null;
                            outMaxSql = -1;
                            outSqlVersion = -1;
                            return false;
                            // We need to recognize the case where this data isn't available.
                            //throw new Exception (CoreConstants.Exception_NoInstanceData );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                throw ex;
            }
        }

        public TimeZoneInfo
           GetTimeZoneInfo()
        {
            TimeZoneStruct tzs = new TimeZoneStruct();
            tzs.Bias = bias;
            tzs.StandardBias = standardBias;
            tzs.StandardDate = SystemTime.FromTimeZoneDateTime(standardDate);
            tzs.DaylightBias = daylightBias;
            tzs.DaylightDate = SystemTime.FromTimeZoneDateTime(daylightDate);

            TimeZoneInfo outTimeZoneInfo = new TimeZoneInfo();
            outTimeZoneInfo.TimeZoneStruct = tzs;

            return outTimeZoneInfo;
        }

        //--------------------------------------------------------------------------------		
        // CountServerInstances - counts number of instances on instance Server
        //--------------------------------------------------------------------------------		
        public int
           CountSharedInstances(
               SqlConnection conn
           )
        {
            int count = 0;

            try
            {
                string cmdstr = String.Format("SELECT count(*) FROM {0}..{1} WHERE instanceServer={2} AND isAuditedServer=1",
                                                 CoreConstants.RepositoryDatabase,
                                                 CoreConstants.RepositoryServerTable,
                                               SQLHelpers.CreateSafeString(instanceServer));
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj is DBNull)
                        count = 0;
                    else
                        count = (int)obj;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return count;
        }

        //--------------------------------------------------------------------------------		
        // CountLoadedArchives - counts number of loaded archives for an instance
        //--------------------------------------------------------------------------------		
        public int
           CountLoadedArchives(
               SqlConnection conn
           )
        {
            int count = 0;

            try
            {
                string cmdstr = String.Format("SELECT count(*) FROM {0}..{1} WHERE instance={2} AND databaseType='Archive'",
                                                 CoreConstants.RepositoryDatabase,
                                                 CoreConstants.RepositorySystemDatabaseTable,
                                               SQLHelpers.CreateSafeString(this.instance));
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj is DBNull)
                        count = 0;
                    else
                        count = (int)obj;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return count;
        }

        //--------------------------------------------------------------------------------		
        // IsAgentDeployed - Does any instance on this server have the agent deployed?
        //--------------------------------------------------------------------------------		
        public bool
           IsAgentDeployed(
               SqlConnection conn
          )
        {
            int count = -1;

            try
            {
                string cmdstr = String.Format("SELECT COUNT(1) FROM {0}..{1} WHERE instanceServer={2} AND isDeployed=1",
                                                 CoreConstants.RepositoryDatabase,
                                                 CoreConstants.RepositoryServerTable,
                                               SQLHelpers.CreateSafeString(instanceServer));
                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj is DBNull)
                        count = 0;
                    else
                        count = (int)obj;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return (count > 0) ? true : false;
        }

        //--------------------------------------------------------------------------------		
        // CopyAgentSettingsToAll - Copy agent properties to all instances on same server
        //--------------------------------------------------------------------------------		
        public void
           CopyAgentSettingsToAll(
              ServerRecord oldServer
           )
        {
            string cmdstr = "";
            try
            {
                string whereClause = String.Format("WHERE isAuditedServer=1 and instanceServer={0}",
                                                    SQLHelpers.CreateSafeString(this.instanceServer));
                cmdstr = GetUpdateSQL(oldServer, whereClause);

                using (SqlCommand cmd = new SqlCommand(cmdstr, this.connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("CopyAgentSettingsToAll",
                                         cmdstr,
                                         ex);
            }
        }

        //--------------------------------------------------------------------------------		
        // CopyTraceDirectoryToAll
        //--------------------------------------------------------------------------------		
        public void
           CopyTraceDirectoryToAll(
              ServerRecord oldServer
           )
        {
            try
            {
                string cmdstr = GetSelectSQL(
                                  String.Format("WHERE isAuditedServer=1 and instanceServer={0}",
                                                 SQLHelpers.CreateSafeString(this.instanceServer)),
                                  "");
                ArrayList srvList = new ArrayList();
                using (SqlCommand cmd = new SqlCommand(cmdstr, this.connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ServerRecord srv = new ServerRecord();
                            srv.connection = this.connection;
                            srv.Load(reader);
                            srvList.Add(srv);
                        }
                    }
                }

                if (oldServer != null && (this.agentTraceDirectory == oldServer.agentTraceDirectory))
                    return;

                // Update server records for all instances
                foreach (ServerRecord srv in srvList)
                {
                    ServerRecord tmpsrv = srv.Clone();

                    srv.agentTraceDirectory = this.agentTraceDirectory;
                    // write record
                    srv.Write(tmpsrv);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "CopyTraceDirectoryToAll",
                                         ex);
            }
        }

        public void ResetAuditSettings()
        {
            ResetServerAuditSettings();
            ResetUserAuditSettings();
        }

        public void ResetServerAuditSettings()
        {
            auditLogins = false;
            auditLogouts = false;
            auditFailedLogins = false;
            auditDDL = false;
            auditSecurity = false;
            auditAdmin = false;
            auditDML = false;
            auditSELECT = false;
            auditTrace = false;

            auditAccessCheck = (int)AccessCheckFilter.NoFilter;
            auditSystemEvents = false;
            auditCaptureSQL = false;
            auditCaptureSQLXE = false; //5.4 XE
            auditExceptions = false;
            auditUDE = false;
            isAuditLogEnabled = false; //5.5 Audit logs
        }

        public void ResetUserAuditSettings()
        {
            auditUsersList = "";
            auditUserAll = false;
            auditUserLogins = false;
            auditUserLogouts = false;
            auditUserFailedLogins = false;
            auditUserDDL = false;
            auditUserSecurity = false;
            auditUserAdmin = false;
            auditUserDML = false;
            auditUserSELECT = false;
            auditUserExtendedEvents = false; //SQLCm 5.4_4.1.1_Extended Events 
            auditUserAccessCheck = (int)AccessCheckFilter.NoFilter;
            auditUserCaptureSQL = false;
            auditUserCaptureTrans = false;
            auditUserExceptions = false;
            auditUserUDE = false;
        }

        #endregion

        #region Private Routines

        //-----------------------------------------------------------------------------
        // InternalRead
        //-----------------------------------------------------------------------------
        private bool
          InternalRead(
             string where, SqlTransaction transaction = null
          )
        {
            bool retval = false;

            try
            {
                string cmdstr = GetSelectSQL(where, "");

                using (SqlCommand cmd = new SqlCommand(cmdstr,
                                                       connection))
                {
                    if (transaction != null)
                        cmd.Transaction = transaction;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Load(reader);
                            retval = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return retval;
        }

        //-------------------------------------------------------------------
        // InternalWrite
        //-------------------------------------------------------------------
        private bool
         InternalWrite(
            bool newServer,
            ServerRecord oldServerRecord,
            SqlTransaction transaction
         )
        {
            bool retval = false;

            //--------------------------------------------------
            // Builds set of SQL commands for write transaction
            // we do this in a transaction to guarantee some
            // consistency between database and tables
            //--------------------------------------------------

            StringBuilder sqlCmd = new StringBuilder("");

            // Create Server Record
            if (newServer)
            {
                sqlCmd.Append(GetInsertSQL());
            }
            else
            {
                sqlCmd.Append(GetUpdateSQL(oldServerRecord));
            }

            // If sqlCmd is empty, then there is nothing to do!
            if (sqlCmd.Length == 0)
                return true;

            //--------------------------------------------------
            // Execute SQL
            //--------------------------------------------------
            try
            {
                int nRows;
                using (SqlCommand cmd = new SqlCommand(sqlCmd.ToString(),
                                                          connection))
                {
                    if (transaction != null)
                    {
                        cmd.Transaction = transaction;
                    }

                    nRows = cmd.ExecuteNonQuery();
                }
                if (nRows > 0)
                {
                    if (newServer)
                    {
                        this.srvId = GetServerId(connection,
                                                   this.instance);
                    }

                    retval = true;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return retval;
        }

        public bool SaveTrustedandPrivUsers(StringBuilder querySQL)
        {
            bool retval = false;
            try
            {
                int nRows;
                using (SqlCommand cmd = new SqlCommand(querySQL.ToString(), connection))
                {
                    nRows = cmd.ExecuteNonQuery();
                }
                retval = true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return retval;
        }
        public bool SaveServerLevelUsersFromWizard(ServerRecord server, RemoteUserList trustedUserList, RemoteUserList privilegedUserList)
        {
            StringBuilder userSQL = new StringBuilder("");
            userSQL.Append(GetDeleteOldUserForCurrentServerSQL(server));
            if (server.AuditTrustedUsersList != null && server.AuditTrustedUsersList != "")
            {
                userSQL.Append(GetTrustedUsersSQLForServer(server, trustedUserList));
            }
            if (server.AuditUsersList != null && server.AuditUsersList != "")
            {
                userSQL.Append(GetPrivilegedUsersSQLForServer(server, privilegedUserList));
            }
            if (SaveTrustedandPrivUsers(userSQL))
            {
                return true;
            }
            return false;
        }
        private string GetDeleteOldUserForCurrentServerSQL(ServerRecord serverDetails)
        {
            string deleteSQL = "";
            deleteSQL += GetDeleteSQLForUser(serverDetails.SrvId);
            return deleteSQL;
        }
        private string GetTrustedUsersSQLForServer(ServerRecord serverDetails, RemoteUserList userList)
        {
            string trustedUsersSQL = "";
            foreach (ServerRole tRole in userList.ServerRoles)
            {
                trustedUsersSQL += GetInsertSQLForUser(serverDetails.SrvId, 0, 1, 0, tRole.Name, null);
            }
            foreach (Login tLogin in userList.Logins)
            {
                trustedUsersSQL += GetInsertSQLForUser(serverDetails.SrvId, 0, 1, 0, null, tLogin.Name);
            }
            return trustedUsersSQL;
        }
        private string GetPrivilegedUsersSQLForServer(ServerRecord serverDetails, RemoteUserList userList)
        {
            string privUsersSQL = "";
            foreach (ServerRole tRole in userList.ServerRoles)
            {
                privUsersSQL += GetInsertSQLForUser(serverDetails.SrvId, 0, 0, 1, tRole.Name, null);
            }
            foreach (Login tLogin in userList.Logins)
            {
                privUsersSQL += GetInsertSQLForUser(serverDetails.SrvId, 0, 0, 1, null, tLogin.Name);
            }
            return privUsersSQL;
        }
        public bool DeleteUser(string querySQL)
        {
            bool retval = false;
            try
            {
                int nRows;
                using (SqlCommand cmd = new SqlCommand(querySQL, connection))
                {
                    nRows = cmd.ExecuteNonQuery();
                }
                if (nRows > 0)
                {
                    retval = true;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return retval;
        }
        #endregion

        #region SQL Builders

        //-------------------------------------------------------------
        // GetSelectStmt
        //--------------------------------------------------------------
        public static string
           GetSelectSQL(
             string strWhere,
             string strOrder)
        {
            string tmp = "SELECT srvId, instance, sqlVersion, description" +                                
                                ",isDeployed,isDeployedManually,isUpdateRequested,isAuditedServer" +
                                ",isRunning,isCrippled,isEnabled,isSqlSecureDb,isOnRepositoryHost" +
                                ",timeLastAgentContact,timeLastHeartbeat,timeLastCollection" +
                                ",eventDatabase,defaultAccess,maxSqlLength" +
                                ",auditLogins,auditFailedLogins" +
                                ",auditDDL,auditSecurity,auditAdmin,auditDML,auditSELECT,auditTrace,auditExceptions" +
                                ",auditFailures,auditSystemEvents,auditCaptureSQL,auditUDE" +
                                ",timeCreated,timeLastModified,timeEnabledModified" +
                                ",configVersion,lastKnownConfigVersion,lastConfigUpdate,configUpdateRequested" +
                                ",auditUsersList,auditUserAll,auditUserLogins,auditUserFailedLogins" +
                                ",auditUserDDL,auditUserSecurity,auditUserAdmin,auditUserDML,auditUserSELECT" +
                                ",auditUserFailures,auditUserCaptureSQL,auditUserCaptureTrans,auditUserExceptions,auditUserUDE" +
                                ",bias,standardBias,standardDate,daylightBias,daylightDate,instanceServer" +
                                ",agentServer, agentPort, agentVersion,agentServiceAccount" +
                                ",agentMaxTraceSize, agentTraceDirectory, agentHeartbeatInterval,agentCollectionInterval, agentDetectionInterval" +
                                ",agentForceCollectionInterval, agentLogLevel,agentTraceOptions" +
                                ",agentMaxFolderSize,agentMaxUnattendedTime" +
                                ",containsBadEvents,eventReviewNeeded,highWatermark,lowWatermark" +
                                ",timeLastIntegrityCheck,lastIntegrityCheckResult,timeLastArchive,lastArchiveResult,agentHealth" +
                                ",agentTraceStartTimeout,auditUserCaptureDDL,versionName,isCluster,isHadrEnabled,auditUserExtendedEvents,auditCaptureSQLXE,isAuditLogEnabled" + //SQLCm 5.4_4.1.1_Extended Events 
                                ",auditLogouts,auditUserLogouts, " + // SQLCM-5375-6.1.4.3-Capture Logout Events
                                "auditTrustedUsersList, " + //v5.6 SQLCM-5373
                                "countDatabasesAuditingAllObjects," + //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
                                "addNewDatabasesAutomatically, " + // SQLCM -541/4876/5259 v5.6
                                "lastNewDatabasesAddTime, " +
                                "isCompressedFile" +
                                " FROM {0} {1} {2}";

            return string.Format(tmp,
                                  CoreConstants.RepositoryServerTable,
                                  strWhere,
                                  strOrder);
        }

        //-------------------------------------------------------------
        // GetUpdateSQL
        //-------------------------------------------------------------
        public string
           GetUpdateSQL(
              ServerRecord oldServer
           )
        {
            return GetUpdateSQL(oldServer, null);
        }

        public string
           GetUpdateSQL(
              ServerRecord oldServer,
              string whereClause
           )
        {
            StringBuilder prop = new StringBuilder("");

            // General Properties
            if (oldServer == null || this.instance != oldServer.Instance)
                AddUpdateProp(ref prop, "instance", true, this.instance);
            if (oldServer == null || this.description != oldServer.description)
                AddUpdateProp(ref prop, "description", true, this.description);
            if (oldServer == null || this.isAuditedServer != oldServer.isAuditedServer)
                AddUpdateProp(ref prop, "isAuditedServer", false, (this.isAuditedServer) ? 1 : 0);

            if (this.IsAuditedServer)
            {
                if (oldServer == null || this.instanceServer != oldServer.InstanceServer)
                    AddUpdateProp(ref prop, "instanceServer", true, this.instanceServer);
                if (oldServer == null || this.sqlVersion != oldServer.sqlVersion)
                    AddUpdateProp(ref prop, "sqlVersion", false, this.sqlVersion);

                if (oldServer == null || this.defaultAccess != oldServer.defaultAccess)
                    AddUpdateProp(ref prop, "defaultAccess", false, this.defaultAccess);
                if (oldServer == null || this.maxSqlLength != oldServer.maxSqlLength)
                    AddUpdateProp(ref prop, "maxSqlLength", false, this.maxSqlLength);

                // "Is" Flags
                if (oldServer == null || this.isRunning != oldServer.isRunning)
                    AddUpdateProp(ref prop, "isRunning", false, (this.isRunning) ? 1 : 0);
                if (oldServer == null || this.isDeployed != oldServer.isDeployed)
                    AddUpdateProp(ref prop, "isDeployed", false, (this.isDeployed) ? 1 : 0);
                if (oldServer == null || this.isDeployedManually != oldServer.isDeployedManually)
                    AddUpdateProp(ref prop, "isDeployedManually", false, (this.isDeployedManually) ? 1 : 0);
                if (oldServer == null || this.isUpdateRequested != oldServer.isUpdateRequested)
                    AddUpdateProp(ref prop, "isUpdateRequested", false, (this.isUpdateRequested) ? 1 : 0);
                if (oldServer == null || this.isEnabled != oldServer.isEnabled)
                    AddUpdateProp(ref prop, "isEnabled", false, this.isEnabled ? 1 : 0);
                if (oldServer == null || this.isSqlSecureDb != oldServer.isSqlSecureDb)
                    AddUpdateProp(ref prop, "isSqlSecureDb", false, (this.isSqlSecureDb) ? 1 : 0);
                if (oldServer == null || this.isOnRepositoryHost != oldServer.isOnRepositoryHost)
                    AddUpdateProp(ref prop, "isOnRepositoryHost", false, (this.isOnRepositoryHost) ? 1 : 0);

                if (oldServer == null || this.configUpdateRequested != oldServer.configUpdateRequested)
                    AddUpdateProp(ref prop, "configUpdateRequested", false, (this.configUpdateRequested) ? 1 : 0);

                // audit settings
                if (oldServer == null || this.AuditLogins != oldServer.AuditLogins)
                    AddUpdateProp(ref prop, "auditLogins", false, (this.AuditLogins) ? 1 : 0);
                // SQLCM-5375-6.1.4.3-Capture Logout Events	
                if (oldServer == null || this.AuditLogouts != oldServer.AuditLogouts)
                    AddUpdateProp(ref prop, "auditLogouts", false, (this.AuditLogouts) ? 1 : 0);
                if (oldServer == null || this.auditFailedLogins != oldServer.auditFailedLogins)
                    AddUpdateProp(ref prop, "auditFailedLogins", false, (this.auditFailedLogins) ? 1 : 0);
                if (oldServer == null || this.auditDDL != oldServer.auditDDL)
                    AddUpdateProp(ref prop, "auditDDL", false, (this.auditDDL) ? 1 : 0);
                if (oldServer == null || this.auditSecurity != oldServer.auditSecurity)
                    AddUpdateProp(ref prop, "auditSecurity", false, (this.auditSecurity) ? 1 : 0);
                if (oldServer == null || this.auditAdmin != oldServer.auditAdmin)
                    AddUpdateProp(ref prop, "auditAdmin", false, (this.auditAdmin) ? 1 : 0);
                if (oldServer == null || this.auditDML != oldServer.auditDML)
                    AddUpdateProp(ref prop, "auditDML", false, (this.auditDML) ? 1 : 0);
                if (oldServer == null || this.auditSELECT != oldServer.auditSELECT)
                    AddUpdateProp(ref prop, "auditSELECT", false, (this.auditSELECT) ? 1 : 0);
                if (oldServer == null || this.auditTrace != oldServer.auditTrace)
                    AddUpdateProp(ref prop, "auditTrace", false, (this.auditTrace) ? 1 : 0);
                if (oldServer == null || this.auditExceptions != oldServer.auditExceptions)
                    AddUpdateProp(ref prop, "auditExceptions", false, (this.auditExceptions) ? 1 : 0);
                if (oldServer == null || this.auditAccessCheck != oldServer.auditAccessCheck)
                    AddUpdateProp(ref prop, "auditFailures", false, this.auditAccessCheck);
                if (oldServer == null || this.auditSystemEvents != oldServer.auditSystemEvents)
                    AddUpdateProp(ref prop, "auditSystemEvents", false, (this.auditSystemEvents) ? 1 : 0);
                if (oldServer == null || this.auditCaptureSQL != oldServer.auditCaptureSQL)
                    AddUpdateProp(ref prop, "auditCaptureSQL", false, (this.auditCaptureSQL) ? 1 : 0);
                if (oldServer == null || this.auditUDE != oldServer.auditUDE)
                    AddUpdateProp(ref prop, "auditUDE", false, (this.auditUDE) ? 1 : 0);

                if (oldServer == null || this.AuditUsersList != oldServer.AuditUsersList)
                    AddUnicodeProp(ref prop, "auditUsersList", this.AuditUsersList);

                //Trusted Users//v5.6 SQLCM-5373
                if (oldServer == null || (this.AuditTrustedUsersList != oldServer.AuditTrustedUsersList && this.AuditTrustedUsersList != null))
                    AddUnicodeProp(ref prop, "auditTrustedUsersList", this.AuditTrustedUsersList);

                if (oldServer == null || this.AuditUserAll != oldServer.AuditUserAll)
                    AddUpdateProp(ref prop, "auditUserAll", false, (this.AuditUserAll) ? 1 : 0);
                if (oldServer == null || this.AuditUserLogins != oldServer.AuditUserLogins)
                    AddUpdateProp(ref prop, "auditUserLogins", false, (this.AuditUserLogins) ? 1 : 0);
                // SQLCM-5375-6.1.4.3-Capture Logout Events
                if (oldServer == null || this.AuditUserLogouts != oldServer.AuditUserLogouts)
                    AddUpdateProp(ref prop, "auditUserLogouts", false, (this.AuditUserLogouts) ? 1 : 0);
                if (oldServer == null || this.auditUserFailedLogins != oldServer.auditUserFailedLogins)
                    AddUpdateProp(ref prop, "auditUserFailedLogins", false, (this.auditUserFailedLogins) ? 1 : 0);
                if (oldServer == null || this.auditUserDDL != oldServer.auditUserDDL)
                    AddUpdateProp(ref prop, "auditUserDDL", false, (this.auditUserDDL) ? 1 : 0);
                if (oldServer == null || this.auditUserSecurity != oldServer.auditUserSecurity)
                    AddUpdateProp(ref prop, "auditUserSecurity", false, (this.auditUserSecurity) ? 1 : 0);
                if (oldServer == null || this.auditUserAdmin != oldServer.auditUserAdmin)
                    AddUpdateProp(ref prop, "auditUserAdmin", false, (this.auditUserAdmin) ? 1 : 0);
                if (oldServer == null || this.auditUserDML != oldServer.auditUserDML)
                    AddUpdateProp(ref prop, "auditUserDML", false, (this.auditUserDML) ? 1 : 0);
                //SQLCm 5.4_4.1.1_Extended Events 
                if (oldServer == null || this.auditUserExtendedEvents != oldServer.auditUserExtendedEvents)
                    AddUpdateProp(ref prop, "auditUserExtendedEvents", false, (this.auditUserExtendedEvents) ? 1 : 0);
                //SQLCm 5.4_4.1.1_Extended Events 
                if (oldServer == null || this.auditCaptureSQLXE != oldServer.auditCaptureSQLXE)
                    AddUpdateProp(ref prop, "auditCaptureSQLXE", false, (this.auditCaptureSQLXE) ? 1 : 0);

                if (oldServer == null || this.auditUserSELECT != oldServer.auditUserSELECT)
                    AddUpdateProp(ref prop, "auditUserSELECT", false, (this.auditUserSELECT) ? 1 : 0);
                if (oldServer == null || this.auditUserAccessCheck != oldServer.auditUserAccessCheck)
                    AddUpdateProp(ref prop, "auditUserFailures", false, this.auditUserAccessCheck);
                if (oldServer == null || this.auditUserCaptureSQL != oldServer.auditUserCaptureSQL || !CoreConstants.AllowCaptureSql)
                {
                    if (CoreConstants.AllowCaptureSql)
                        AddUpdateProp(ref prop, "auditUserCaptureSQL", false, (this.auditUserCaptureSQL) ? 1 : 0);
                    else
                        AddUpdateProp(ref prop, "auditUserCaptureSQL", false, 2);
                }
                if (oldServer == null || this.AuditUserCaptureDDL != oldServer.AuditUserCaptureDDL || !CoreConstants.AllowCaptureSql)
                {
                    if (CoreConstants.AllowCaptureSql)
                        AddUpdateProp(ref prop, "auditUserCaptureDDL", false, (this.AuditUserCaptureDDL) ? 1 : 0);
                    else
                        AddUpdateProp(ref prop, "auditUserCaptureDDL", false, 2);
                }
                if (oldServer == null || this.auditUserCaptureTrans != oldServer.auditUserCaptureTrans)
                    AddUpdateProp(ref prop, "auditUserCaptureTrans", false, (this.auditUserCaptureTrans) ? 1 : 0);
                if (oldServer == null || this.auditUserExceptions != oldServer.auditUserExceptions)
                    AddUpdateProp(ref prop, "auditUserExceptions", false, (this.auditUserExceptions) ? 1 : 0);
                if (oldServer == null || this.auditUserUDE != oldServer.auditUserUDE)
                    AddUpdateProp(ref prop, "auditUserUDE", false, (this.auditUserUDE) ? 1 : 0);

                // agent
                if (oldServer == null || this.agentServer != oldServer.agentServer)
                    AddUpdateProp(ref prop, "agentServer", true, this.agentServer);
                if (oldServer == null || this.agentPort != oldServer.agentPort)
                    AddUpdateProp(ref prop, "agentPort", true, this.agentPort);
                if (oldServer == null || this.agentServiceAccount != oldServer.agentServiceAccount)
                    AddUpdateProp(ref prop, "agentServiceAccount", true, this.agentServiceAccount);
                if (oldServer == null || this.agentMaxTraceSize != oldServer.agentMaxTraceSize)
                    AddUpdateProp(ref prop, "agentMaxTraceSize", false, (int)this.agentMaxTraceSize);
                if (oldServer == null || this.agentTraceDirectory != oldServer.agentTraceDirectory)
                    AddUpdateProp(ref prop, "agentTraceDirectory", true, this.agentTraceDirectory);
                if (oldServer == null || this.agentHeartbeatInterval != oldServer.agentHeartbeatInterval)
                    AddUpdateProp(ref prop, "agentHeartbeatInterval", false, this.agentHeartbeatInterval);
                if (oldServer == null || this.agentCollectionInterval != oldServer.agentCollectionInterval)
                    AddUpdateProp(ref prop, "agentCollectionInterval", false, this.agentCollectionInterval);
                if (oldServer == null || this.agentDetectionInterval != oldServer.agentDetectionInterval)
                    AddUpdateProp(ref prop, "agentDetectionInterval", false, this.agentDetectionInterval);
                if (oldServer == null || this.agentForceCollectionInterval != oldServer.agentForceCollectionInterval)
                    AddUpdateProp(ref prop, "agentForceCollectionInterval", false, this.agentForceCollectionInterval);
                if (oldServer == null || this.agentLogLevel != oldServer.agentLogLevel)
                    AddUpdateProp(ref prop, "agentLogLevel", false, this.agentLogLevel);
                if (oldServer == null || this.agentMaxFolderSize != oldServer.agentMaxFolderSize)
                    AddUpdateProp(ref prop, "agentMaxFolderSize", false, this.agentMaxFolderSize);
                if (oldServer == null || this.agentMaxUnattendedTime != oldServer.agentMaxUnattendedTime)
                    AddUpdateProp(ref prop, "agentMaxUnattendedTime", false, this.agentMaxUnattendedTime);
                if (oldServer == null || this.agentTraceStartTimeout != oldServer.agentTraceStartTimeout)
                    AddUpdateProp(ref prop, "agentTraceStartTimeout", false, this.agentTraceStartTimeout);

                // watermarks               
                if (oldServer == null || this.lowWatermark != oldServer.lowWatermark)
                    AddUpdateProp(ref prop, "lowWatermark", false, this.lowWatermark);
                if (oldServer == null || this.highWatermark != oldServer.highWatermark)
                    AddUpdateProp(ref prop, "highWatermark", false, this.highWatermark);
                if (oldServer == null || this.eventDatabase != oldServer.eventDatabase)
                    AddUpdateProp(ref prop, "eventDatabase", true, this.eventDatabase);

                //5.5 Audit Logs
                if (oldServer == null || this.isAuditLogEnabled != oldServer.isAuditLogEnabled)
                    AddUpdateProp(ref prop, "isAuditLogEnabled", false, (this.isAuditLogEnabled) ? 1 : 0);

                //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
                if (oldServer == null || this.countDatabasesAuditingAllObjects != oldServer.countDatabasesAuditingAllObjects)
                {
                    AddUpdateProp(ref prop, "countDatabasesAuditingAllObjects", false, this.countDatabasesAuditingAllObjects);
                }
            }
            //SQLCM -541/4876/5259 v5.6
            if (oldServer == null || this.addNewDatabasesAutomatically != oldServer.addNewDatabasesAutomatically)
            {
                AddUpdateProp(ref prop, "addNewDatabasesAutomatically", true, (this.addNewDatabasesAutomatically) ? 1 : 0);

                if (this.addNewDatabasesAutomatically)
                {
                    // SQLCM-5913: Add code to compare UTC time of server's last heart beat with creation time on dbs
                    AddUpdateProp(ref prop, "lastNewDatabasesAddTime", false, "GETUTCDATE()");
                }
            }
            // SQLCM 5.8
            if (oldServer == null || this.isCompressedFile != oldServer.isCompressedFile)
                AddUpdateProp(ref prop, "isCompressedFile", false, (this.isCompressedFile) ? 1 : 0);

            //----------------------------------------------------
            // Finish Building SQL if any properties have changed
            //-----------------------------------------------------
            StringBuilder tmp = new StringBuilder("");
            if (prop.Length > 0)
            {
                // update last modified
                prop.Append(",timeLastModified = GETUTCDATE()");

                tmp.AppendFormat("UPDATE {0} SET ",
                                  CoreConstants.RepositoryServerTable);
                tmp.Append(prop.ToString());

                if (whereClause == null || whereClause == "")
                {
                    tmp.AppendFormat(" WHERE instance={0};",
                                      SQLHelpers.CreateSafeString(this.instance));
                }
                else
                {
                    tmp.AppendFormat(" {0}", whereClause);
                }

                return tmp.ToString();
            }

            return prop.ToString();
        }

        private void
           AddUpdateProp(
              ref StringBuilder currStr,
              string propName,
              bool propString,
              object propValue
           )
        {
            currStr.AppendFormat("{0}{1} = {2}",
                                  (currStr.Length > 0) ? COMMA_CHARACTER : "",
                                  propName,
                                  (propString) ? SQLHelpers.CreateSafeString(propValue == null ? string.Empty : propValue.ToString())
                                               : propValue);
        }

        private void
           AddUnicodeProp(
              ref StringBuilder currStr,
              string propName,
              object propValue
           )
        {
            string s = SQLHelpers.CreateSafeString(propValue.ToString());

            currStr.AppendFormat("{0}{1} = ",
                                  (currStr.Length > 0) ? COMMA_CHARACTER : "",
                                  propName);

            if (s == "null")
                currStr.AppendFormat(s);
            else
                currStr.AppendFormat("N{0}", s);
        }


        public string GetInsertSQLForUser(int srvId, int dbID, int isTrusted, int isPrivileged, string roleName, string loginName)
        {
            props = new StringBuilder("");
            values = new StringBuilder("");
            AddInsertProp("serverId", false, srvId);
            AddInsertProp("databaseId", false, dbID);
            AddInsertProp("isTrusted", false, isTrusted);
            AddInsertProp("isPrivileged", false, isPrivileged);
            AddInsertProp("roleName", true, roleName);
            AddInsertProp("loginName", true, loginName);
            AddInsertProp("isServerLevel", false, 1);
            string insertSql = String.Format("INSERT INTO {0}({1}) VALUES ({2});",
                                           CoreConstants.RepositoryUsersTable,
                                           props.ToString(),
                                           values.ToString());
            return insertSql;
        }
        public string GetDeleteSQLForUser(int srvId)
        {
            string deleteSql = String.Format("DELETE FROM {0} where serverId = {1};", CoreConstants.RepositoryUsersTable, srvId);
            return deleteSql;
        }
        public string GetDeleteSQLForUserDatabases(int srvId)
        {
            string deleteSql = String.Format("DELETE FROM {0} where databaseId IN (SELECT dbId FROM Databases WHERE srvId = {1});", CoreConstants.RepositoryUsersTable, srvId);
            return deleteSql;
        }
        //----------------------------------------------------------------
        // GetInsertSql
        //----------------------------------------------------------------
        public string
           GetInsertSQL()
        {
            props = new StringBuilder("");
            values = new StringBuilder("");

            // general	      
            AddInsertProp("instance", true, this.instance);

            if (InstancePort.HasValue)
            {
                AddInsertProp("instancePort", false, _instancePort);
            }

            AddInsertProp("sqlVersion", false, this.sqlVersion);

            AddInsertProp("description", true, this.description);
            AddInsertProp("eventDatabase", true, this.eventDatabase);
            AddInsertProp("defaultAccess", false, this.defaultAccess);
            AddInsertProp("maxSqlLength", false, this.maxSqlLength);
            AddInsertProp("instanceServer", true, this.instanceServer);
            AddInsertProp("isAuditedServer", false, (this.isAuditedServer) ? 1 : 0);

            // time	      
            AddInsertProp("timeCreated", false, "GETUTCDATE()");
            AddInsertProp("timeLastModified", false, "GETUTCDATE()");
            AddInsertProp("timeEnabledModified", false, "GETUTCDATE()");

            if (this.IsAuditedServer)
            {

                // "Is" Flags
                AddInsertProp("isRunning", false, (this.isRunning) ? 1 : 0);
                AddInsertProp("isCrippled", false, (this.isCrippled) ? 1 : 0);
                AddInsertProp("isDeployed", false, (this.isDeployed) ? 1 : 0);
                AddInsertProp("isDeployedManually", false, (this.isDeployedManually) ? 1 : 0);
                AddInsertProp("isUpdateRequested", false, (this.isUpdateRequested) ? 1 : 0);
                AddInsertProp("isEnabled", false, 1);
                AddInsertProp("isSqlSecureDb", false, (this.isSqlSecureDb) ? 1 : 0);
                AddInsertProp("isOnRepositoryHost", false, (this.isOnRepositoryHost) ? 1 : 0);

                // audit settings
                AddInsertProp("auditLogins", false, (this.auditLogins) ? 1 : 0);
                AddInsertProp("auditLogouts", false, (this.auditLogouts) ? 1 : 0);
                AddInsertProp("auditFailedLogins", false, (this.auditFailedLogins) ? 1 : 0);
                AddInsertProp("auditDDL", false, (this.auditDDL) ? 1 : 0);
                AddInsertProp("auditSecurity", false, (this.auditSecurity) ? 1 : 0);
                AddInsertProp("auditAdmin", false, (this.auditAdmin) ? 1 : 0);
                AddInsertProp("auditDML", false, (this.auditDML) ? 1 : 0);
                AddInsertProp("auditSELECT", false, (this.auditSELECT) ? 1 : 0);
                AddInsertProp("auditTrace", false, (this.auditTrace) ? 1 : 0);
                AddInsertProp("auditExceptions", false, (this.auditExceptions) ? 1 : 0);

                AddInsertProp("auditFailures", false, this.auditAccessCheck);
                AddInsertProp("auditSystemEvents", false, (this.auditSystemEvents) ? 1 : 0);
                AddInsertProp("auditCaptureSQL", false, (this.auditCaptureSQL) ? 1 : 0);
                AddInsertProp("auditUDE", false, (this.auditUDE) ? 1 : 0);

                //Privileged Users
                AddInsertUnicodeProp("auditUsersList", this.auditUsersList);
                //Trusted users //v5.6 SQLCM-5373
                AddInsertUnicodeProp("auditTrustedUsersList", this.auditTrustedUsersList);
                AddInsertProp("auditUserAll", false, (this.auditUserAll) ? 1 : 0);
                AddInsertProp("auditUserLogins", false, (this.auditUserLogins) ? 1 : 0);
                AddInsertProp("auditUserLogouts", false, (this.auditUserLogouts) ? 1 : 0);
                AddInsertProp("auditUserFailedLogins", false, (this.auditUserFailedLogins) ? 1 : 0);
                AddInsertProp("auditUserDDL", false, (this.auditUserDDL) ? 1 : 0);
                AddInsertProp("auditUserSecurity", false, (this.auditUserSecurity) ? 1 : 0);
                AddInsertProp("auditUserAdmin", false, (this.auditUserAdmin) ? 1 : 0);
                AddInsertProp("auditUserDML", false, (this.auditUserDML) ? 1 : 0);
                AddInsertProp("auditUserExtendedEvents", false, (this.auditUserExtendedEvents) ? 1 : 0); //SQLCm 5.4_4.1.1_Extended Events 
                AddInsertProp("auditCaptureSQLXE", false, (this.auditCaptureSQLXE) ? 1 : 0); //SQLCm 5.4_4.1.1_Extended Events 
                AddInsertProp("auditUserSELECT", false, (this.auditUserSELECT) ? 1 : 0);
                AddInsertProp("auditUserFailures", false, this.auditUserAccessCheck);
                if (CoreConstants.AllowCaptureSql)
                {
                    AddInsertProp("auditUserCaptureSQL", false, (this.auditUserCaptureSQL) ? 1 : 0);
                    AddInsertProp("auditUserCaptureDDL", false, (this.auditUserCaptureDDL) ? 1 : 0);
                }
                else
                {
                    AddInsertProp("auditUserCaptureSQL", false, 2);
                    AddInsertProp("auditUserCaptureDDL", false, 2);
                }
                AddInsertProp("auditUserCaptureTrans", false, (this.auditUserCaptureTrans) ? 1 : 0);
                AddInsertProp("auditUserExceptions", false, (this.auditUserExceptions) ? 1 : 0);
                AddInsertProp("auditUserUDE", false, (this.auditUserUDE) ? 1 : 0);

                // Agent
                AddInsertProp("agentServiceAccount", true, this.agentServiceAccount);
                AddInsertProp("agentServer", true, this.agentServer);

                // Watermarks
                AddInsertProp("lowWatermark", false, this.lowWatermark);
                AddInsertProp("highWatermark", false, this.highWatermark);


                // inherit from another instance case
                if (insertAgentProperties)
                {
                    AddInsertProp("agentPort", false, this.agentPort);
                    AddInsertProp("agentTraceDirectory", true, this.agentTraceDirectory);
                    AddInsertProp("agentCollectionInterval", false, this.agentCollectionInterval);
                    AddInsertProp("agentForceCollectionInterval", false, this.agentForceCollectionInterval);
                    AddInsertProp("agentDetectionInterval", false, this.agentDetectionInterval);
                    AddInsertProp("agentHeartbeatInterval", false, this.agentHeartbeatInterval);
                    AddInsertProp("agentLogLevel", false, this.agentLogLevel);
                    AddInsertProp("agentMaxFolderSize", false, this.agentMaxFolderSize);
                    AddInsertProp("agentMaxTraceSize", false, this.agentMaxTraceSize);
                    AddInsertProp("agentMaxUnattendedTime", false, this.agentMaxUnattendedTime);
                    AddInsertProp("agentTraceOptions", false, (int)this.agentTraceOptions);
                    AddInsertProp("agentVersion", true, this.agentVersion);
                    AddInsertProp("agentTraceStartTimeout", false, this.agentTraceStartTimeout);
                    if (this.timeLastAgentContact != DateTime.MinValue)
                    {
                        AddInsertProp("timeLastAgentContact", false, SQLHelpers.CreateSafeDateTime(this.timeLastAgentContact));
                    }
                    if (this.timeLastHeartbeat != DateTime.MinValue)
                    {
                        AddInsertProp("timeLastHeartbeat", false, SQLHelpers.CreateSafeDateTime(this.timeLastHeartbeat));
                    }
                }
                else
                {
                    // just set agent properties to defaults
                    AddInsertProp("agentPort", false, CoreConstants.AgentServerTcpPort);
                    AddInsertProp("agentTraceDirectory", true, "");
                    AddInsertProp("agentCollectionInterval", false, CoreConstants.Agent_Default_CollectInterval);
                    AddInsertProp("agentForceCollectionInterval", false, CoreConstants.Agent_Default_ForceCollectionInterval);
                    AddInsertProp("agentHeartbeatInterval", false, CoreConstants.Agent_Default_HeartbeatInterval);
                    AddInsertProp("agentLogLevel", false, CoreConstants.Agent_Default_LogLevel);
                    AddInsertProp("agentMaxFolderSize", false, CoreConstants.Agent_Default_MaxFolderSize);
                    AddInsertProp("agentMaxTraceSize", false, CoreConstants.Agent_Default_MaxTraceSize);
                    AddInsertProp("agentMaxUnattendedTime", false, CoreConstants.Agent_Default_MaxUnattendedTime);
                    AddInsertProp("agentTraceOptions", false, CoreConstants.Agent_Default_TraceOptions);
                    AddInsertProp("agentTraceStartTimeout", false, CoreConstants.Agent_Default_TraceStartTimeout);
                }

                // version stuff         
                AddInsertProp("configVersion", false, this.configVersion);
                AddInsertProp("lastKnownConfigVersion", false, this.lastKnownConfigVersion);

                if (this.lastConfigUpdate != DateTime.MinValue)
                {
                    AddInsertProp("lastConfigUpdate", false, null);
                }
            }

            AddInsertProp("versionName", true, _sqlVersionName);

            AddInsertProp("isCluster", false, _isCluster ? 1 : 0);
            AddInsertProp("isHadrEnabled", false, _isHadrEnabled ? 1 : 0);
            AddInsertProp("isAuditLogEnabled", false, (this.isAuditLogEnabled) ? 1 : 0); //5.5 Audit Logs


            //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
            AddInsertProp("countDatabasesAuditingAllObjects", false, this.countDatabasesAuditingAllObjects);
            //SQLCM -541/4876/5259 v5.6
            AddInsertProp("addNewDatabasesAutomatically", false, (this.AddNewDatabasesAutomatically) ? 1 : 0); //SQLCM -541/4876/5259 v5.6

            // SQLCM-5913: Add code to compare UTC time of server's last heart beat with creation time on dbs
            AddInsertProp("lastNewDatabasesAddTime", false, "GETUTCDATE()");

            // SQLCM5.8 Requirement 5.3
            AddInsertProp("isCompressedFile", false, this.isCompressedFile ? 1 : 0);
            
            string insertSql = String.Format("INSERT INTO {0}({1}) VALUES ({2});",
                                           CoreConstants.RepositoryServerTable,
                                           props.ToString(),
                                           values.ToString());

            return insertSql;
        }

        StringBuilder props;
        StringBuilder values;

        private void
           AddInsertProp(
              string propName,
              bool strValue,
              object propValue
          )
        {
            if (propValue == null) return;

            if (props.Length > 0) props.Append(COMMA_CHARACTER);
            props.Append(propName);

            if (values.Length > 0) values.Append(COMMA_CHARACTER);
            if (strValue)
                values.Append(SQLHelpers.CreateSafeString(propValue.ToString()));
            else
                values.Append(propValue);
        }

        private void
           AddInsertUnicodeProp(
              string propName,
              object propValue
          )
        {
            if (propValue == null) return;

            // property name
            if (props.Length > 0) props.Append(COMMA_CHARACTER);
            props.Append(propName);


            // value
            if (values.Length > 0) values.Append(COMMA_CHARACTER);
            string s = SQLHelpers.CreateSafeString(propValue.ToString());
            if (s == "null")
                values.AppendFormat(s);
            else
                values.AppendFormat("N{0}", s);
        }

        //----------------------------------------------------------------
        // GetIncrementSQL
        //----------------------------------------------------------------
        static public string
           GetIncrementSQL(
              int serverId
           )
        {
            return String.Format("UPDATE {0} " +
                                  "SET configVersion = configVersion + 1 " +
                                  "WHERE srvId={1}",
                                 CoreConstants.RepositoryServerTable,
                                 serverId);
        }

        //----------------------------------------------------------------
        // GetIncrementSQL
        //----------------------------------------------------------------
        static public string GetIncrementSQL(string instanceServer)
        {
            return String.Format("UPDATE SQLcompliance.dbo.{0} " +
                                  "SET configVersion = configVersion + 1 " +
                                  "WHERE instanceServer={1}",
                                  CoreConstants.RepositoryServerTable,
                                  SQLHelpers.CreateSafeString(instanceServer));
        }

        //----------------------------------------------------------------
        // GetIncrementSQLAll
        //----------------------------------------------------------------
        static public string
           GetIncrementAllSQL()
        {
            return String.Format("UPDATE {0} " +
                                  "SET configVersion = configVersion + 1 " +
                                  "WHERE isAuditedServer=1",
                                 CoreConstants.RepositoryServerTable);
        }

        //-------------------------------------------------------------
        // GetServerIdSQL - searching with server id and database name
        //--------------------------------------------------------------
        private static string
           GetServerIdSQL(
             string serverName
          )
        {
            string tmp = "SELECT srvId" +
                               " FROM {0} " +
                               " WHERE instance={1}";

            return string.Format(tmp,
                                  CoreConstants.RepositoryServerTable,
                                  SQLHelpers.CreateSafeString(serverName));
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Get query for getting count of all databases with auditing all Objects enabled of a server with id 'srvId'
        private static string GetCountDatabasesAuditingAllObjectsSQL(int srvId)
        {
            string query = "SELECT countDatabasesAuditingAllObjects FROM {0}..{1} WHERE srvId = {2}";

            return string.Format(query, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable, srvId);
        }

        //SQLcm 5.6 - Fix for 5820
        //Gets agent version data for the required server id
        private static string GetAgentVersionDataForServerIdSQL(int srvId)
        {
            string query = "SELECT agentVersion, sqlVersion FROM {0}..{1} WHERE srvId = {2}";

            return string.Format(query, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable, srvId);
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Get query for incrementing 'countDatabasesAuditingAllObjects'
        private static string GetIncrementCountDatabasesAuditingAllObjectsSQL(int srvId)
        {
            string query = "UPDATE {0}..{1} SET countDatabasesAuditingAllObjects = countDatabasesAuditingAllObjects + 1 WHERE srvId = {2}";

            return string.Format(query, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable, srvId);
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Get query for decrementing 'countDatabasesAuditingAllObjects'
        private static string GetDecrementCountDatabasesAuditingAllObjectsSQL(int srvId)
        {
            string query = "UPDATE {0}..{1} SET countDatabasesAuditingAllObjects = countDatabasesAuditingAllObjects - 1 WHERE srvId = {2}";

            return string.Format(query, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable, srvId);
        }

        //SQLcm 5.6 (Aakash Prakash) - Fix for 4967
        //Get query for updating 'countDatabasesAuditingAllObjects'
        private static string GetUpdateCountDatabasesAuditingAllObjectsSQL(int srvId, int newValue)
        {
            string query = "UPDATE {0}..{1} SET countDatabasesAuditingAllObjects = {2} WHERE srvId = {3}";

            return string.Format(query, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable, newValue, srvId);
        }

        //Get query for getting count of all auditing DML enabled objects
        private static string GetCountAuditingDMLEnabledSQL(int srvId)
        {
            string query = "SELECT COUNT(*) FROM {0}..{1} WHERE srvId = {2} AND (auditDML = 1 OR auditSELECT = 1) AND (auditDmlAll = 1 OR (auditDmlAll = 0 AND auditUserTables <> 2))";

            return string.Format(query, CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable, srvId);
        }

        //-------------------------------------------------------------
        // GetEnableSQL - Create UPDATE SQL to enable/disable 
        //                database auditing
        //--------------------------------------------------------------
        private string
         GetEnableSQL(
            bool enable
         )
        {
            StringBuilder cmd = new StringBuilder("");

            cmd.AppendFormat("UPDATE {0} SET ",
                              CoreConstants.RepositoryServerTable);

            cmd.AppendFormat("isEnabled = {0}",
                               enable ? 1 : 0);

            cmd.AppendFormat(",timeEnabledModified=GETUTCDATE()");

            cmd.AppendFormat(" WHERE srvId={0};", this.srvId);

            return cmd.ToString();
        }

        //-------------------------------------------------------------
        // GetDeleteSQL - Create SQL to delete a server
        //--------------------------------------------------------------
        private string
           GetDeleteSQL()
        {
            string cmdStr = String.Format("DELETE FROM {0} where srvId={1}",
                                           CoreConstants.RepositoryServerTable,
                                           this.srvId);
            return cmdStr;
        }

        //------------------------------------------------------------------------------------------
        // GetDeleteThresholdSQL - Create SQL to delete threshold settings from report card table
        //------------------------------------------------------------------------------------------
        private string
           GetDeleteThresholdSQL()
        {
            string cmdStr = String.Format("DELETE FROM {0} where srvId={1}",
                                           CoreConstants.RepositoryReportCardTable,
                                           this.srvId);
            return cmdStr;
        }

        //-------------------------------------------------------------
        // GetDeleteSystemDatabaseSQL
        //--------------------------------------------------------------
        private string
           GetDeleteSystemDatabaseSQL()
        {
            string cmdStr = String.Format("DELETE FROM {0} where instance={1} and databaseType='Event'",
                                           CoreConstants.RepositorySystemDatabaseTable,
                                           SQLHelpers.CreateSafeString(this.instance));
            return cmdStr;
        }

        //-------------------------------------------------------------
        // GetDeleteDatabaseObjectsSQL
        //--------------------------------------------------------------
        private string
           GetDeleteDatabaseObjectsSQL()
        {
            string cmdStr = String.Format(
               "DELETE FROM {0} WHERE objectId IN" +
               " (SELECT objectId FROM {2}..DatabaseObjects AS o, {2}..Databases AS d" +
               " WHERE o.dbId=d.dbId AND d.srvId={1});",
               CoreConstants.RepositoryDatabaseObjectsTable,
               this.srvId,
               CoreConstants.RepositoryDatabase);

            return cmdStr;
        }

        //-------------------------------------------------------------
        // GetDeleteDatabasesSQL
        //--------------------------------------------------------------
        private string
           GetDeleteDatabasesSQL()
        {
            string cmdStr = String.Format("DELETE FROM {0} WHERE srvId={1};",
                                           CoreConstants.RepositoryDatabaseTable,
                                           this.srvId);
            return cmdStr;
        }

        //-------------------------------------------------------------
        // GetDeleteEventsDatabaseSQL
        //--------------------------------------------------------------
        private string
           GetDeleteEventsDatabaseSQL()
        {
            string cmdStr = String.Format("DROP DATABASE {0}",
                                           SQLHelpers.CreateSafeDatabaseName(this.eventDatabase));
            return cmdStr;
        }

        //-------------------------------------------------------------
        // GetIsFlagsSQL
        //--------------------------------------------------------------
        static private string
           GetIsFlagsSQL(
              string instance,
              bool deployed,
              bool manuallyDeployed,
              bool running,
              bool crippled
           )
        {
            StringBuilder cmd = new StringBuilder("");

            cmd.AppendFormat("UPDATE {0} SET ",
                              CoreConstants.RepositoryServerTable);

            cmd.AppendFormat("isDeployed = {0}", deployed ? 1 : 0);
            cmd.AppendFormat(",isDeployedManually = {0}", manuallyDeployed ? 1 : 0);
            cmd.AppendFormat(",isRunning = {0}", running ? 1 : 0);
            cmd.AppendFormat(",isCrippled = {0}", crippled ? 1 : 0);
            cmd.AppendFormat(",timeEnabledModified=GETUTCDATE()");

            cmd.AppendFormat(" WHERE instance={0};", SQLHelpers.CreateSafeString(instance));

            return cmd.ToString();
        }

        public string IsInstalledManually(string instanceName)
        {
            string result = null;
            string strConn = String.Format("server={0};" +
                                           "integrated security=SSPI;" +
                                           "Connect Timeout=30;" +
                                           "Application Name='{1}';",
                                           instanceName, CoreConstants.ManagementConsoleName);

            SqlConnection conn = new SqlConnection(strConn);
            SqlCommand command = new SqlCommand();

            string strGetRegKey = String.Format("DECLARE @datapath varchar(255) " +
                                           "EXEC master..xp_regread '{0}','{1}','{2}', @datapath OUTPUT Select @datapath",
                                           CoreConstants.LocalMachine, CoreConstants.RegistryPath, CoreConstants.RegistryKey);
            command.CommandText = strGetRegKey;

            try
            {
                conn.Open();
                command.Connection = conn;
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    result = reader.IsDBNull(0) ? null : reader.GetString(0);
                }
            }

            catch (Exception)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred while retrieving registry value"));
            }

            finally
            {
                if (conn.State == ConnectionState.Open)

                    conn.Close();
            }
            return result;
        }

        #endregion

        public override string ToString()
        {
            return Instance;
        }

        // Added method to update audit setting to make XE default auditing after upgrade 5.8 - SQLCM-6206
        static public List<String>
            GetServersForDefaultXE(
              SqlConnection conn
            )
        {
            List<String> serverList = new List<String>();
            
            try
            {
                SQLHelpers.CheckConnection(conn);

                string cmdstr = "Select instance from Servers where [auditSQLXEDefault] = 1";

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           serverList.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "GetServers",
                                         ex);
            }
            return serverList;
        }

        static public List<String>
            ResetServersForDefaultXE(
              SqlConnection conn,
              string instance
            )
        {
            List<String> serverList = new List<String>();

            try
            {
                SQLHelpers.CheckConnection(conn);

                string cmdstr = String.Format("Update Servers set [auditSQLXEDefault] = 0 where instance = {0}", SQLHelpers.CreateSafeString(instance));

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    cmd.ExecuteReader();                    
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "ResetServersForDefaultXE",
                                         ex);
            }
            return serverList;
        }

        //----------------------------------------------------------------
        // UpdateIsRowCountEnabledFlag
        //----------------------------------------------------------------
        static public void
           UpdateIsRowCountEnabledFlag(
              string instance,
              bool isRowCountEnabled
           )
        {
            Repository rep = null;

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                UpdateIsRowCountEnabledFlag(rep.connection, instance, isRowCountEnabled);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw e;
            }
            finally
            {
                if (rep != null) rep.CloseConnection();
            }
        }

        //----------------------------------------------------------------
        // UpdateIsRowCountEnabledFlag
        // SQLCM 5.9
        //----------------------------------------------------------------
        static public void
           UpdateIsRowCountEnabledFlag(
              SqlConnection conn,
              string instance,
              bool isRowCountEnabled
           )
        {
            string cmdstr = "";


            try
            {
                SQLHelpers.CheckConnection(conn);

                string agent;
                cmdstr = String.Format("SELECT agentServer FROM {0}..{1} WHERE instance={2}",
                                              CoreConstants.RepositoryDatabase,
                                              CoreConstants.RepositoryServerTable,
                                              SQLHelpers.CreateSafeString(instance));

                using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj is DBNull)
                        return;
                    else
                        agent = (string)obj;
                }

                if (agent != "")
                {
                    cmdstr = String.Format("UPDATE {0}..{1} " +
                                                     "SET isRowCountEnabled={2} " +
                                                     "WHERE agentServer={3};",
                                                  CoreConstants.RepositoryDatabase,
                                                  CoreConstants.RepositoryServerTable,
                                                  isRowCountEnabled ? 1 : 0,
                                                  SQLHelpers.CreateSafeString(agent));
                    using (SqlCommand cmd = new SqlCommand(cmdstr, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "UpdateIsRowCountEnabledFlag", cmdstr, ex);
                errMsg = ex.Message;
            }
        }
    }
}
