using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Microsoft.Win32;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Triggers;


namespace Idera.SQLcompliance.Core.Agent
{
    /// <summary>
    /// Summary description for InstanceConfig.
    /// </summary>
    public class SQLInstance
    {
        #region Private Member Fields

        private string name;               // SQL Server instance monitored by this agent
        private string alias;               // Instance alias for filenames and registry

        private int collectionInterval = CoreConstants.Agent_Default_CollectInterval;
        private int forceCollectionInterval = CoreConstants.Agent_Default_ForceCollectionInterval;
        private int detectionInterval = CoreConstants.Agent_Default_TamperingDetectionInterval;
        private int traceStartTimeout = CoreConstants.Agent_Default_TraceStartTimeout;

        // Trace options
        private int maxTraceSize = CoreConstants.Agent_Default_MaxTraceSize;
        private TraceOption traceOptions = (TraceOption)CoreConstants.Agent_Default_TraceOptions;
        private string traceDirectory = "";
        private DateTime stopTime = DateTime.MinValue;

        // Status
        private DateTime lastCollectionTime = DateTime.Now;

        // Configurations
        private int configVersion;
        private DateTime lastModifiedTime;

        private int sqlVersion;             // SQL Server major version number
        private bool isConfigured;
        private bool isEnabled;
        private bool isSqlSecureDb;
        private bool isAuditedServer;
        internal bool validated = false;
        internal bool[] rollover;
        internal bool TraceCollectionStopped = false;
        internal bool TraceErrorFound = false;
        internal bool TamperingDetectionErrorFound = false;
        internal int TamperingDetectionExceptionFound = 0;
        internal int ExceptionReportInterval;
        internal bool ConfigurationFileErrorFound = false;
        internal bool ConnectionHealthy = true;
        private Dictionary<string, int> configErrors = new Dictionary<string, int>();


        // Audit and event trace configurations
        private ServerAuditConfiguration auditConfiguration = new ServerAuditConfiguration();
        TraceConfiguration[] traceConfigurations;
        XeTraceConfiguration[] xetraceConfigurations;   // SQLCm 5.4_4.1.1_Extended Events
        AuditLogConfiguration[] auditLogConfigurations;
        internal TraceCollector collector = null;
        internal TamperingDetector detector = null;

        internal object syncObj = new object();

        // DML before/after data trigger manager
        internal TriggerManager triggerManager;
        private DateTime lastDCSetupCheckTime = DateTime.MaxValue;
        private bool isCompressedFile;
        #endregion

        #region Constructor

        public SQLInstance(string instanceName)
        {
            Name = instanceName;
            Initialize();
        }

        #endregion

        #region Properties and Events

        public int SqlVersion
        {
            get { return sqlVersion; }
            set { sqlVersion = value; }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value != null)
                {
                    name = value.ToUpper();
                    alias = NativeMethods.GetHashCode(name).ToString();
                }
                else
                {
                    name = null;
                    alias = null;
                }
            }
        }

        public string Alias
        {
            get { return alias; }
        }

        public int ConfigVersion
        {
            get { return configVersion; }
            set { configVersion = value; }
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

        public bool IsAuditedServer
        {
            get { return isAuditedServer; }
            set { isAuditedServer = value; }
        }

        public DateTime LastCollectionTime
        {
            get { return lastCollectionTime; }
            set { lastCollectionTime = value; }
        }

        public DateTime LastModifiedTime
        {
            get { return lastModifiedTime; }
            set { lastModifiedTime = value; }
        }

        public int CollectionInterval
        {
            get { return collectionInterval; }
            set { collectionInterval = value; }
        }

        public int ForceCollectionInterval
        {
            get { return forceCollectionInterval; }
            set { forceCollectionInterval = value; }
        }

        public int TraceStartTimeout
        {
            get { return traceStartTimeout; }
            set { traceStartTimeout = value; }
        }

        public int MaxTraceSize
        {
            get { return maxTraceSize; }
            set { maxTraceSize = value; }
        }

        public TraceOption TraceOptions
        {
            get { return traceOptions; }
            set { traceOptions = value; }
        }

        public string TraceDirectory
        {
            get { return traceDirectory; }
            set
            {
                traceDirectory = value;
                if (auditConfiguration != null)
                    auditConfiguration.TraceDirectory = traceDirectory;
                if (collector != null)
                    collector.TraceDirectory = traceDirectory;
            }
        }

        public DateTime StopTime
        {
            get { return stopTime; }
            set { stopTime = value; }
        }

        internal ServerAuditConfiguration AuditConfig
        {
            get { return auditConfiguration; }
        }

        internal int DetectionInterval
        {
            get { return detectionInterval; }
            set
            {
                if (detectionInterval >= 7)
                    detectionInterval = value;
                else
                    detectionInterval = CoreConstants.Agent_Default_TamperingDetectionInterval;

                ExceptionReportInterval = 36000 / detectionInterval;
            }
        }

        internal DateTime LastDCSetupCheckTime
        {
            get
            {
                return lastDCSetupCheckTime;
            }
            set
            {
                lastDCSetupCheckTime = value;
            }
        }

        public bool IsCompressedFile
        {
            get { return isCompressedFile; }
            set { isCompressedFile = value; }
        }

        #endregion

        #region Initialization and Configuration

        //------------------------------------------------------------------
        // Initialize - initailizes the configurations for this instance
        //------------------------------------------------------------------
        private void
           Initialize()
        {
            LoadConfiguration();

            try
            {
                // Load local audit configuration
                auditConfiguration.Instance = name;
                auditConfiguration.InstanceAlias = alias;
                auditConfiguration.TraceDirectory = traceDirectory;
                isConfigured = ConfigurationHelper.LoadAuditConfigurationFromFile(auditConfiguration);
                auditConfiguration.sqlVersion = sqlVersion;
                ExceptionReportInterval = 36000 / detectionInterval;

                if (isConfigured)
                {
                    configVersion = auditConfiguration.Version;
                    isEnabled = auditConfiguration.IsEnabled;
                    lastModifiedTime = auditConfiguration.LastModifiedTime;
                    isSqlSecureDb = auditConfiguration.IsSQLsecure;
                    traceConfigurations = auditConfiguration.GenerateTraceConfigurations();
                    xetraceConfigurations = auditConfiguration.GenerateTraceConfigurationsXE();
                    auditLogConfigurations = auditConfiguration.GenerateAuditLogConfigurations();
                }
                triggerManager = new TriggerManager(this, auditConfiguration);
                lastDCSetupCheckTime = DateTime.Now;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred loading local audit configuration for " + name + ".",
                                         e,
                                         true);
                isConfigured = false;
            }
        }

        //------------------------------------------------------------------
        // LoadConfiguration - read the current configuration from registry
        //------------------------------------------------------------------
        private void
           LoadConfiguration()
        {
            RegistryKey rk = null;
            RegistryKey rks = null;

            try
            {
                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(GetRegistrySubKey());

                // All the values are optional
                string trueOrFalse = (string)rks.GetValue(CoreConstants.Agent_RegVal_AuditingEnabled, CoreConstants.Agent_Default_AuditingEnabled);
                isEnabled = trueOrFalse.ToUpper().StartsWith("T");

                // Collector configuration
                collectionInterval = (int)rks.GetValue(CoreConstants.Agent_RegVal_CollectionInterval, SQLcomplianceAgent.Instance.CollectionInterval);
                forceCollectionInterval = (int)rks.GetValue(CoreConstants.Agent_RegVal_ForceCollectionInterval, SQLcomplianceAgent.Instance.ForceCollectionInterval);
                traceStartTimeout = SQLcomplianceAgent.Instance.TraceStartTimeout;

                // Tracing configuration
                maxTraceSize = (int)rks.GetValue(CoreConstants.Agent_RegVal_MaxTraceSize, SQLcomplianceAgent.Instance.MaxTraceSize);
                traceOptions = (TraceOption)rks.GetValue(CoreConstants.Agent_RegVal_TraceOptions, SQLcomplianceAgent.Instance.TraceOptions);
                traceDirectory = (string)rks.GetValue(CoreConstants.Agent_RegVal_TraceDirectory, SQLcomplianceAgent.Instance.TraceDirectory);
                traceDirectory = SQLcomplianceAgent.FixTraceDirectory(traceDirectory);
                sqlVersion = (int)rks.GetValue(CoreConstants.Agent_RegVal_SQLVersion, 0);
                isCompressedFile = Convert.ToBoolean(rks.GetValue(CoreConstants.Agent_RegVal_IsCompressedFile, true));
            }
            catch (Exception e)
            {
                // TODO: add a new constant to CoreConstant
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                   String.Format("An error occurred reading configuration registry for SQL Server {0}.", name),
                                  e,
                                  ErrorLog.Severity.Error);
            }
            finally
            {
                if (rks != null)
                    rks.Close();
                rks = null;

                if (rk != null)
                    rk.Close();
                rk = null;
            }
            try
            {
                CheckSQLVersionChange();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                         String.Format("An error occurred checking the SQL Server version for {0}.", name),
                                         e,
                                         ErrorLog.Severity.Error);
            }
        }

        //-----------------------------------------------------------------------------------
        //  CheckSQLVersionChange(): check if the SQL instance is upgraded.
        //-----------------------------------------------------------------------------------
        private void
           CheckSQLVersionChange()
        {
            int currentVersion = GetSqlVersion();
            if (currentVersion > 0 && sqlVersion > 0 && currentVersion != sqlVersion)
            {
                if (currentVersion > sqlVersion)  // instance is upgraded
                {
                    sqlVersion = currentVersion;
                    UpdateAuditConfiguration();          // Reload configuration for the instance
                    SetSQLVersion(sqlVersion);
                }
                else if (currentVersion < sqlVersion) // Downgraded?
                {
                    throw new Exception(String.Format("Reverting to an older version SQL Server is not supported.  Delete and register the instance from the Management Console to audit instance {0}.", this.name));
                }
            }
            else if (sqlVersion == 0 && currentVersion > 0)
            {
                SetSQLVersion(currentVersion);
            }
        }

        //-------------------------------------------------------------------------
        // SetSQLVersionValue - Set SQL Server version in registry
        //-------------------------------------------------------------------------
        private void
           SetSQLVersion(int sqlVersion)
        {
            RegistryKey rk = null;
            RegistryKey rks = null;

            try
            {
                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(GetRegistrySubKey());
                rks.SetValue(CoreConstants.Agent_RegVal_SQLVersion, sqlVersion);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred writting SQL Server {0} version to the registry.", this.name),
                                         e.Message,
                                         ErrorLog.Severity.Warning);

            }
            finally
            {
                this.sqlVersion = sqlVersion;
                if (rks != null)
                    rks.Close();
                rks = null;

                if (rk != null)
                    rk.Close();
                rk = null;
            }
        }


        //-------------------------------------------------------------------------
        // GetRegistrySubKey - returns the registry subkey string for this instance
        //-------------------------------------------------------------------------
        private string
           GetRegistrySubKey()
        {
            return SQLcomplianceAgent.AgentRegistryKey + @"\" + alias;
        }

        #endregion

        #region Connections
        //-------------------------------------------------------------------------
        // GetConnection - open a connection to the instance of the SQL Server.
        //-------------------------------------------------------------------------
        public SqlConnection GetConnection()
        {

            return GetConnection(null);
        }

        //-------------------------------------------------------------------------
        // GetConnection - open a connection to the instance of the SQL Server.
        //-------------------------------------------------------------------------
        public SqlConnection GetConnection(string database)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(SQLcomplianceAgent.CreateConnectionString(name, database));
                conn.Open();
                ResetConnectionState();
            }
            catch (Exception e)
            {
                string msg = String.Format(CoreConstants.Exception_ErrorConnectingToSQLServer, name, e.Message);
                //ConnectionFailedException ce = new ConnectionFailedException(msg, e);
                //SetConnectionState( false, ce );
                //throw ce;
            }

            return conn;
        }

        private int GetSqlVersion()
        {

            using (SqlConnection conn = GetConnection())
            {
                return GetSqlVersion(conn.ServerVersion);
            }
        }

        static internal int GetSqlVersion(string versionString)
        {
            return int.Parse(versionString.Substring(0, versionString.IndexOf('.'))); ;
        }

        #endregion

        #region Status
        //-------------------------------------------------------------------------
        // UpdateStatus - Update the status of the current instance.
        //-------------------------------------------------------------------------
        public void UpdateStatus(InstanceStatus status)
        {
            if (status == null)
                return;
            status.ConfigVersion = configVersion;
            status.LastModifiedTime = lastModifiedTime;
            status.LastCollectionTime = lastCollectionTime;
            status.MaxTraceSize = maxTraceSize;
            if (SQLcomplianceAgent.Instance.IsClustered)
                status.SqlVersion = sqlVersion + 1000;
            else
                status.SqlVersion = sqlVersion;
            status.TraceDirectory = traceDirectory;
            status.CollectionInterval = collectionInterval;
            status.ForceCollectionInterval = forceCollectionInterval;
            status.TraceStartTimeout = traceStartTimeout;
        }

        //-------------------------------------------------------------------------
        // UpdateStatus - Update the status of the current instance.
        //-------------------------------------------------------------------------
        public void UpdateStatus()
        {
            try
            {
                InstanceStatus status = SQLcomplianceAgent.Instance.GetInstanceStatus(name);
                if (status != null)
                {
                    UpdateStatus(status);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred updating status for " + name + ".",
                                         e,
                                         true);
            }
        }
        #endregion

        #region SyncDBIds

        public void SyncDBIds()
        {
            if (auditConfiguration.AuditDBList == null)
                return;

            //only try to sync the dbs if there are dbs being audited.
            if (auditConfiguration.AuditDBList.Length > 0)
            {
                bool updatedDbIds = false;

                //get the list of audited dbids
                Hashtable auditedServerDbIds = ConfigurationHelper.GetDbIds(name, auditConfiguration.AuditDBList);
                try
                {
                    updatedDbIds = SQLcomplianceAgent.Instance.UpdateDBIds(auditedServerDbIds, name);
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write(String.Format("Unable to update the database IDs for {0} Error: {1}", this.name, e.ToString()), ErrorLog.Severity.Error);
                }

                //get the new settings from the repository and roll the traces.
                if (updatedDbIds)
                    UpdateAuditConfiguration();
            }
        }

        #endregion

        #region Audit Management Methods
        //---------------------------------------------------------------------------
        // StartAuditing : Check SPs, start tracing and collection
        //---------------------------------------------------------------------------
        /// <summary>
        /// This method starts all the auditing processes.  It is called during 
        /// agent startup and when the auditing is changed from disabled to enabled.
        /// </summary>
        public void StartAuditing()
        {
            lock (syncObj)
            {
                if (isEnabled && validated)
                {
                    if (!isConfigured)
                    {
                        UpdateAuditConfiguration();
                        if (isConfigured)
                        {
                            // Start trace collection.
                            StartCollectingTraces();
                        }
                    }
                    else
                    {
                        bool updateNeed = false;
                        using (SqlConnection conn = GetConnection())
                        {
                            //if (auditConfiguration.AuditedUserGroups != null)
                            //{
                            //    for (int i = 0; i < auditConfiguration.AuditedUserGroups.Length; i++)
                            //    {
                            //        if (auditConfiguration.AuditedUserGroups != null && auditConfiguration.AuditedUserGroups[i].Length != 0)
                            //        {
                            //            string[] TrustedUsers = UserList.GetUserLIst(auditConfiguration.Instance, auditConfiguration.AuditedUserGroups, null);
                            //            if (TrustedUsers.Length != auditConfiguration.AuditedUsers.Length)
                            //            {
                            //                ServerRecord.IncrementServerConfigVersion(conn,
                            //                                      auditConfiguration.Instance.Split('\\')[0]);
                            //                updateNeed = true;
                            //            }
                            //        }
                            //    }
                            //}
                            //UpdateAuditUsers();
                            UpdateAuditConfig();
                            if (auditConfiguration.AuditDBList != null)
                            {
                                for (int i = 0; i < auditConfiguration.AuditDBList.Length; i++)
                                {
                                    if (auditConfiguration.AuditDBList[i].AuditedUserGroups != null && auditConfiguration.AuditDBList[i].AuditedUserGroups.Length != 0)
                                    {
                                        //var TrustedUsers = ConfigurationHelper.GetAuditedPrivilegedUsers(auditConfiguration.Instance,
                                        //   auditConfiguration.AuditDBList[i].AuditedUserGroups,
                                        //   auditConfiguration.AuditDBList[i].AuditedServerRoles);
                                        UserCache userchache = new UserCache(auditConfiguration.Instance);
                                        string[] TrustedUsers = UserList.GetUserLIst(auditConfiguration.Instance, auditConfiguration.AuditDBList[i].AuditedUserGroups, userchache);
                                        if (TrustedUsers.Length != auditConfiguration.AuditDBList[i].AuditedUsers.Length)
                                        {
                                            ServerRecord.IncrementServerConfigVersion(conn,
                                                                  auditConfiguration.Instance.Split('\\')[0]);
                                            updateNeed = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!updateNeed)
                            {
                                // SQL Server start up SP and main audit SP health check
                                SPHelper.AuditStoredProceduresHealthCheck(alias,
                                                                             conn,
                                                                             SQLcomplianceAgent.Instance.AgentVersion,
                                                                             auditConfiguration.Version,
                                                                             traceConfigurations,
                                                                             auditConfiguration.LastModifiedTime,
                                                                             traceDirectory,
                                                                             maxTraceSize,
                                                                             SQLcomplianceAgent.Instance.MaxUnattendedTime,
                                                                             traceOptions,
                                                                             sqlVersion,
                                                                             xetraceConfigurations,   // 5.4_4.1.1_Extended Events
                                                                             auditLogConfigurations);
                            }
                        }
                        if (updateNeed)
                        {
                            UpdateAuditUsers();
                            RestartTraces(true);
                            //UpdateAuditConfiguration();
                        }
                        else
                        {
                            // Start SQL Server Tracing if traces are not already started
                            StartTracing(null);
                            // Start trace collection.
                            StartCollectingTraces();
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------
        // StopAuditing - Stop event tracing and collecting traces. 
        //---------------------------------------------------------------------------
        public void StopAuditing()
        {
            StopTracing();
            StopCollectingTraces();
        }

        public void UpdateAuditUsers()
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, string.Format("Updating audit configuration for audit users for {0}.", name));

            try
            {
                ConfigurationHelper.LoadNewConfigurationFromServerAuditUsers(auditConfiguration);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always, string.Format("Error while updating configuration for audit user for {0}.", name));
                throw e;
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, string.Format("Audit configuration for audit users for {0} updated.", name));
        }

        //---------------------------------------------------------------------------
        // UpdateAuditConfiguration 
        //---------------------------------------------------------------------------
        public void UpdateAuditConfiguration()
        {

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Updating audit configuration for " + name + ".");

            try
            {
                bool savedIsEnabled = isEnabled;
                lock (syncObj)
                {
                    // Get new version from the server
                    InstanceStatus status = SQLcomplianceAgent.Instance.GetInstanceStatus(name);
                    if (status != null)
                    {
                        ConfigurationHelper.LoadNewConfigurationFromServer(auditConfiguration);
                        triggerManager.UpdateConfiguration(auditConfiguration);
                        configVersion = auditConfiguration.Version;
                        lastModifiedTime = auditConfiguration.LastModifiedTime;
                        isEnabled = auditConfiguration.IsEnabled;
                        isConfigured = true;
                        UpdateStatus();

                        // Update the server
                        SQLcomplianceAgent.Instance.SendAuditUpdatedMessage(name);

                        if (isEnabled)
                        {
                            traceConfigurations = auditConfiguration.GenerateTraceConfigurations();
                            xetraceConfigurations = auditConfiguration.GenerateTraceConfigurationsXE();
                            auditLogConfigurations = auditConfiguration.GenerateAuditLogConfigurations();
                            RestartTraces(true);
                            if (collector == null)
                                StartCollectingTraces();
                        }
                        else
                        {
                            if (savedIsEnabled)
                                DisableAuditing(false);
                        }
                    }
                    ReportConfigurationResolution();
                }

            }
            catch (SocketException se)
            {
                SQLcomplianceAgent.Instance.ReportCollectionServerConnectionError(se);
            }
            catch (Exception e)
            {
                ReportConfigurationError(e);
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Audit configuration for " + name + " updated.");

        }

        public void UpdateAuditConfig()
        {

            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Updating audit configuration for " + name + ".");

            try
            {
                bool savedIsEnabled = isEnabled;
                lock (syncObj)
                {
                    // Get new version from the server
                    InstanceStatus status = SQLcomplianceAgent.Instance.GetInstanceStatus(name);
                    if (status != null)
                    {
                        ConfigurationHelper.LoadNewConfigurationFromServer(auditConfiguration);
                        triggerManager.UpdateConfiguration(auditConfiguration);
                        configVersion = auditConfiguration.Version;
                        lastModifiedTime = auditConfiguration.LastModifiedTime;
                        isEnabled = auditConfiguration.IsEnabled;
                        isConfigured = true;
                        UpdateStatus();

                        // Update the server
                        SQLcomplianceAgent.Instance.SendAuditUpdatedMessage(name);

                        if (isEnabled)
                        {
                            traceConfigurations = auditConfiguration.GenerateTraceConfigurations();
                            xetraceConfigurations = auditConfiguration.GenerateTraceConfigurationsXE();
                            auditLogConfigurations = auditConfiguration.GenerateAuditLogConfigurations();
                        }
                        else
                        {
                            if (savedIsEnabled)
                                DisableAuditing(false);
                        }
                    }
                    ReportConfigurationResolution();
                }

            }
            catch (SocketException se)
            {
                SQLcomplianceAgent.Instance.ReportCollectionServerConnectionError(se);
            }
            catch (Exception e)
            {
                ReportConfigurationError(e);
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Audit configuration for " + name + " updated.");

        }


        //---------------------------------------------------------------------------
        // DisableAuditing - Disable event tracing and stop collecting traces. 
        //---------------------------------------------------------------------------
        public void DisableAuditing(bool throwAwayExistingTraceFiles)
        {
            try
            {
                lock (syncObj)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             String.Format("Disabling auditing for {0}.", name));

                    isEnabled = false;
                    try
                    {
                        using (SqlConnection conn = GetConnection())
                        {
                            SPHelper.StopAllTraces(conn);
                            if (SPHelper.DoesSPExist(conn, CoreConstants.Agent_AuditSPNameXE))
                                SPHelper.StopAllTracesXE(conn);
                            if (SPHelper.DoesSPExist(conn, CoreConstants.Agent_AuditLogSPName))
                                SPHelper.StopAllAuditLogs(conn);
                            SPHelper.DropStartupSP(conn);
                            SPHelper.DropAuditSP(conn);
                            SPHelper.DropAuditSPXE(conn);
                            SPHelper.DropAuditLogSP(conn);
                            TraceInfo.SaveAgentStartedTraceInfo(alias, SQLcomplianceAgent.Instance.AgentVersion, new TraceInfo[0]);
                            TraceInfo.SaveAgentStartedTraceInfoXE(alias, SQLcomplianceAgent.Instance.AgentVersion, new TraceInfo[0]);  // 5.4_4.1.1_Extended Events
                            TraceInfo.SaveAgentStartedAuditLogsInfo(alias, SQLcomplianceAgent.Instance.AgentVersion, new TraceInfo[0]);
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Default, e, true);
                    }

                    StopCollectingTraces();

                    if (throwAwayExistingTraceFiles)
                    {
                        DeleteTraceFiles();
                    }
                    else
                    {
                        CollectOnce();
                    }
                    triggerManager.UpdateConfiguration(null);
                    UpdateStatus();
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Auditing for {0} disabled.", name));
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                         "Error disabling auditing for " + name + ".",
                                         e,
                                         true);
            }
        }

        //---------------------------------------------------------------------------
        // EnableAuditing - Starts event tracing and stop collecting traces. 
        //---------------------------------------------------------------------------
        public void EnableAuditing()
        {
            try
            {
                isEnabled = true;
                StartAuditing();
                UpdateStatus();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                        "Error enabling auditing for " + name,
                                        e,
                                        true);
            }
        }

        //---------------------------------------------------------------------------
        // UpdateConfigurationFile 
        //---------------------------------------------------------------------------
        private bool UpdateConfigurationFile()
        {
            IFormatter formatter = null;
            Stream stream = null;
            bool success = false;
            RemoteAuditConfiguration remoteConfig = new RemoteAuditConfiguration();

            string filename = Path.Combine(auditConfiguration.TraceDirectory, auditConfiguration.InstanceAlias + ".bin");
            try
            {
                formatter = new BinaryFormatter();
                using (stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    remoteConfig = (RemoteAuditConfiguration)formatter.Deserialize(stream);
                    success = true;
                }
                ReportConfigurationResolution();
            }
            catch (Exception e)
            {
                ReportConfigurationError(e);
            }

            if (!success)
                return false;

            try
            {
                remoteConfig.AuditedUsers = auditConfiguration.AuditedUsers;
                remoteConfig.ServerTrustedUsers = auditConfiguration.AuditedServerTrustedUsers;
                remoteConfig.PrivUsers = GetAuditedPrivUsers();
                remoteConfig.StructVersion = CoreConstants.SerializationVersion;

                if (auditConfiguration.AuditDBList != null)
                {
                    Dictionary<int, DBRemoteAuditConfiguration> dbList = new Dictionary<int, DBRemoteAuditConfiguration>(remoteConfig.DBConfigs.Length);
                    foreach (DBRemoteAuditConfiguration rconfig in remoteConfig.DBConfigs)
                        dbList.Add(rconfig.DbId, rconfig);

                    foreach (DBAuditConfiguration dbConfig in auditConfiguration.AuditDBList)
                    {
                        DBRemoteAuditConfiguration tmpconfig;
                        if (dbList.ContainsKey(dbConfig.DBId))
                            tmpconfig = dbList[dbConfig.DBId];
                        else
                            continue;

                        tmpconfig.ServerRoles = dbConfig.AuditedServerRoles;
                        tmpconfig.Users = dbConfig.AuditedUserGroups;
                        tmpconfig.TrustedUsers = dbConfig.AuditedUsers;
                    }
                }
                ConfigurationHelper.SaveAuditConfigurationToFile(auditConfiguration, remoteConfig);
            }
            catch (Exception e)
            {
                ReportConfigurationError(e);
                success = false;
            }

            return success;
        }
        #endregion

        #region Tracing Start/Stop/Restart Methods
        //---------------------------------------------------------------------------
        // StartTracing 
        //---------------------------------------------------------------------------
        /// <summary>
        /// StartTracing - Method that starts all the configured event traces
        /// </summary>
        private void
           StartTracing(ArrayList traces)
        {
            //ErrorLog.Instance.ErrorLevel = ErrorLog.Level.UltraDebug; 
            bool timeout = false;
            //SqlConnection conn = null;
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Start tracing for {0}.", name), ErrorLog.Severity.Informational);

            if (SQLcomplianceAgent.Instance.MaxFolderSizeReached)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Agent maximum folder size limit reached.  Aborting StartTraceing.");
                return;
            }
            TraceInfo[] newTraces;
            TraceInfo[] newTracesXE;
            TraceInfo[] newAuditLogs;
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    // Get a list of running traces to stop
                    int[] stoppingTraceIds = SPHelper.GetRunningTraces(conn);
                    int[] stoppingTraceIdsXE = SPHelper.GetRunningTracesXE(conn);
                    int[] stoppingAuditLogIds = SPHelper.GetRunningAuditLogs(conn);

                    if (traces == null || traces.Count == 0)
                    {
                        newTraces = SPHelper.StartAllTraces(conn);
                        newTracesXE = SPHelper.StartAllTracesXE(conn);
                        newAuditLogs = SPHelper.StartAllAuditLogs(conn);
                    }
                    else
                    {
                        ArrayList stoppingTraceList = new ArrayList();
                        ArrayList newTraceList = new ArrayList();
                        Hashtable newTraceNumberList = new Hashtable();
                        //5.4 XE
                        ArrayList stoppingTraceListXE = new ArrayList();
                        ArrayList newTraceListXE = new ArrayList();
                        Hashtable newTraceNumberListXE = new Hashtable();

                        //5.5 Audit Logs
                        ArrayList stoppingAuditLogList = new ArrayList();
                        ArrayList newAuditLogList = new ArrayList();
                        Hashtable newAuditLogNumberList = new Hashtable();

                        for (int i = 0; i < traces.Count; i++)
                        {
                            string tempFile = Path.GetFileNameWithoutExtension(((TraceInfo)traces[i]).FileName);
                            if (tempFile.StartsWith("XE"))
                            {
                                if (!newTraceNumberListXE.Contains(((TraceInfo)traces[i]).TraceNumber))
                                {
                                    newTraceNumberListXE.Add(((TraceInfo)traces[i]).TraceNumber, traces[i]);

                                    if (!((TraceInfo)traces[i]).Rollover)
                                    {
                                        // stop the old trace and start a new one
                                        newTraceListXE.Add(SPHelper.StartTraceXE(conn, ((TraceInfo)traces[i]).TraceNumber));
                                        stoppingTraceListXE.Add(((TraceInfo)traces[i]).TraceId);
                                    }
                                    else if (((TraceInfo)traces[i]).Status == TraceStatus.Close)
                                    {
                                        newTraceListXE.Add(SPHelper.StartTraceXE(conn, ((TraceInfo)traces[i]).TraceNumber));
                                    }
                                    else // keep the old trace
                                    {
                                        newTraceListXE.Add(traces[i]);
                                    }
                                }
                                else // stop the trace without creating a new one to eliminate duplicate traces
                                {
                                    stoppingTraceListXE.Add(((TraceInfo)traces[i]).TraceId);
                                }
                            }
                            else if (tempFile.StartsWith("AL"))
                            {
                                if (!newAuditLogNumberList.Contains(((TraceInfo)traces[i]).TraceNumber))
                                {
                                    newAuditLogNumberList.Add(((TraceInfo)traces[i]).TraceNumber, traces[i]);

                                    if (!((TraceInfo)traces[i]).Rollover)
                                    {
                                        // stop the old trace and start a new one
                                        newAuditLogList.Add(SPHelper.StartTrace(conn, ((TraceInfo)traces[i]).TraceNumber));
                                        stoppingAuditLogList.Add(((TraceInfo)traces[i]).TraceId);
                                    }
                                    else if (((TraceInfo)traces[i]).Status == TraceStatus.Close)
                                    {
                                        newAuditLogList.Add(SPHelper.StartTrace(conn, ((TraceInfo)traces[i]).TraceNumber));
                                    }
                                    else // keep the old trace
                                    {
                                        newAuditLogList.Add(traces[i]);
                                    }
                                }
                                else // stop the trace without creating a new one to eliminate duplicate traces
                                {
                                    stoppingAuditLogList.Add(((TraceInfo)traces[i]).TraceId);
                                }
                            }
                            else
                            {

                                if (!newTraceNumberList.Contains(((TraceInfo)traces[i]).TraceNumber))
                                {
                                    newTraceNumberList.Add(((TraceInfo)traces[i]).TraceNumber, traces[i]);

                                    if (!((TraceInfo)traces[i]).Rollover)
                                    {
                                        // stop the old trace and start a new one
                                        newTraceList.Add(SPHelper.StartTrace(conn, ((TraceInfo)traces[i]).TraceNumber));
                                        stoppingTraceList.Add(((TraceInfo)traces[i]).TraceId);
                                    }
                                    else if (((TraceInfo)traces[i]).Status == TraceStatus.Close)
                                    {
                                        newTraceList.Add(SPHelper.StartTrace(conn, ((TraceInfo)traces[i]).TraceNumber));
                                    }
                                    else // keep the old trace
                                    {
                                        newTraceList.Add(traces[i]);
                                    }
                                }
                                else // stop the trace without creating a new one to eliminate duplicate traces
                                {
                                    stoppingTraceList.Add(((TraceInfo)traces[i]).TraceId);
                                }
                            }
                        }
                        stoppingTraceIds = (int[])stoppingTraceList.ToArray(typeof(int));
                        stoppingTraceIdsXE = (int[])stoppingTraceListXE.ToArray(typeof(int));     // 5.4_4.1.1_Extended Events
                        newTraces = (TraceInfo[])newTraceList.ToArray(typeof(TraceInfo));
                        newTracesXE = (TraceInfo[])newTraceListXE.ToArray(typeof(TraceInfo));     // 5.4_4.1.1_Extended Events
                        stoppingAuditLogIds = (int[])stoppingAuditLogList.ToArray(typeof(int));     // 5.5 Audit Logs
                        newAuditLogs = (TraceInfo[])newAuditLogList.ToArray(typeof(TraceInfo));
                    }


                    TraceInfo.SaveAgentStartedTraceInfo(alias, SQLcomplianceAgent.Instance.AgentVersion, newTraces);
                    TraceInfo.SaveAgentStartedTraceInfoXE(alias, SQLcomplianceAgent.Instance.AgentVersion, newTracesXE);  // 5.4_4.1.1_Extended Events

                    //5.5 Audit Logs
                    TraceInfo.SaveAgentStartedAuditLogsInfo(alias, SQLcomplianceAgent.Instance.AgentVersion, newAuditLogs);

                    // reset the trace tampered and trace error flags since the traces are restarted.
                    // this.TamperingDetectionErrorFound = false;
                    ReportTraceResolution();
                    rollover = new bool[newTraces.Length];

                    if (!UpdateConfigurationFile())
                    {
                        // TODO: send an alert to the server
                    }
                    SPHelper.StopTraces(conn, stoppingTraceIds);
                    SPHelper.StopTracesXE(conn, stoppingTraceIdsXE);
                    SPHelper.StopAuditLogs(conn, stoppingAuditLogIds);
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Tracing for {0} started.", name), ErrorLog.Severity.Informational);
                }
            }
            catch (SqlException e)
            {
                if (e.Message.Contains("Timeout expired"))
                    timeout = true;
                ReportTraceError(e, "The trace start timeout value can be modified on the Trace Options tab of the Agent Properties dialog in the SQLcompliance Management Console.");
            }
            catch (Exception e)
            {
                ReportTraceError(e, "");

            }

            // If timeout is true, we timeed out starting the new traces. Just add 
            // the ones we were able to the list in the registry and let tamper detection
            // fix the rest of it
            if (timeout)
            {
                UpdateRunningTraceList();
                UpdateRunningTraceListXE();     // 5.4_4.1.1_Extended Events
            }
            //ErrorLog.Instance.ErrorLevel = ErrorLog.Level.Default;
        }

        //---------------------------------------------------------------------------
        // RestartTraces - Restart the traces without unconditional recreate the SPs.
        //---------------------------------------------------------------------------
        public void RestartTraces()
        {
            RestartTraces(false, null);
        }

        //---------------------------------------------------------------------------
        // RestartTraces - Restart the traces without unconditional recreate the SPs.
        //---------------------------------------------------------------------------
        public void RestartTraces(bool recreate)
        {
            RestartTraces(recreate, null);
        }

        //---------------------------------------------------------------------------
        // RestartTraces 
        //---------------------------------------------------------------------------
        public void
           RestartTraces(bool recreateStoredProcedures, ArrayList traces)
        {
            if (SQLcomplianceAgent.Instance.MaxFolderSizeReached)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Agent max folder size limit reached.  Traces are not restarted");
                return;
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Restarting the traces for " + name + ".");
            bool all = recreateStoredProcedures || traces == null || traces.Count == 0;

            try
            {
                lock (syncObj)
                {
                    using (SqlConnection conn = GetConnection())
                    {
                        if (recreateStoredProcedures)
                        {
                            // UpdateAuditUsers();
                            UpdateAuditConfig();
                            traceConfigurations = auditConfiguration.GenerateTraceConfigurations();
                            xetraceConfigurations = auditConfiguration.GenerateTraceConfigurationsXE();
                            auditLogConfigurations = auditConfiguration.GenerateAuditLogConfigurations();
                            SPHelper.RecreateStoredProcedures(conn,
                                                                alias,
                                                                SQLcomplianceAgent.Instance.AgentVersion,
                                                                configVersion,
                                                                traceConfigurations,
                                                                auditConfiguration.LastModifiedTime,
                                                                traceDirectory,
                                                                maxTraceSize,
                                                                SQLcomplianceAgent.Instance.MaxUnattendedTime,
                                                                traceOptions,
                                                                sqlVersion,
                                                                xetraceConfigurations, // 5.4_4.1.1_Extended Events
                                                                auditLogConfigurations);
                        }
                        else
                        {
                            bool updateNeed = false;

                            if (auditConfiguration.AuditDBList != null)
                            {
                                for (int i = 0; i < auditConfiguration.AuditDBList.Length; i++)
                                {
                                    if (auditConfiguration.AuditDBList[i].AuditedUserGroups != null && auditConfiguration.AuditDBList[i].AuditedUserGroups.Length != 0)
                                    {
                                        //var TrustedUsers = ConfigurationHelper.GetAuditedPrivilegedUsers(auditConfiguration.Instance,
                                        //   auditConfiguration.AuditDBList[i].AuditedUserGroups,
                                        //   auditConfiguration.AuditDBList[i].AuditedServerRoles);
                                        UserCache userchache = new UserCache(auditConfiguration.Instance);
                                        string[] TrustedUsers = UserList.GetUserLIst(auditConfiguration.Instance, auditConfiguration.AuditDBList[i].AuditedUserGroups, userchache);
                                        if (TrustedUsers.Length != auditConfiguration.AuditDBList[i].AuditedUsers.Length)
                                        {
                                            ServerRecord.IncrementServerConfigVersion(conn,
                                                                  auditConfiguration.Instance.Split('\\')[0]);
                                            updateNeed = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            //if (SPHelper.AuditStoredProceduresHealthCheck(alias,
                            //                                                conn,
                            //                                                SQLcomplianceAgent.Instance.AgentVersion,
                            //                                                configVersion,
                            //                                                traceConfigurations,
                            //                                                auditConfiguration.LastModifiedTime,
                            //                                                traceDirectory,
                            //                                                maxTraceSize,
                            //                                                SQLcomplianceAgent.Instance.MaxUnattendedTime,
                            //                                                traceOptions,
                            //                                                sqlVersion,
                            //                                                xetraceConfigurations,
                            //                                                auditLogConfigurations)) // 5.4_4.1.1_Extended Events
                            if (updateNeed)
                            {
                                UpdateAuditUsers();
                                RestartTraces(true);
                                //UpdateAuditConfiguration();
                            }
                            else
                            {
                                if (SPHelper.AuditStoredProceduresHealthCheck(alias,
                                                                            conn,
                                                                            SQLcomplianceAgent.Instance.AgentVersion,
                                                                            configVersion,
                                                                            traceConfigurations,
                                                                            auditConfiguration.LastModifiedTime,
                                                                            traceDirectory,
                                                                            maxTraceSize,
                                                                            SQLcomplianceAgent.Instance.MaxUnattendedTime,
                                                                            traceOptions,
                                                                            sqlVersion,
                                                                            xetraceConfigurations,
                                                                            auditLogConfigurations)) // 5.4_4.1.1_Extended Events
                                    all = true;
                            }
                        }
                    }

                    if (all)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Restarting all traces.");
                        StartTracing(null);
                    }
                    else
                    {
                        if (ErrorLog.Instance.ErrorLevel >= ErrorLog.Level.Debug)
                        {
                            StringBuilder msg = new StringBuilder();
                            msg.AppendFormat("Restarting {0} traces:\n", traces.Count);

                            foreach (TraceInfo trace in traces)
                                msg.AppendFormat("Trace ID {0}, trace number {1}\n", trace.TraceId, trace.TraceNumber);

                            ErrorLog.Instance.Write(ErrorLog.Level.Debug, msg.ToString());
                        }
                        StartTracing(traces);
                    }
                }

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default, e, true);
            }

            if (isEnabled)
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Traces restarted for " + name);

        }

        //---------------------------------------------------------------------------
        // StopTracing - Stops all the current traces 
        //---------------------------------------------------------------------------
        public void StopTracing()
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Stopping traces stopped for " + name);

            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    int[] runningTraceIds = SPHelper.GetRunningTraces(conn);
                    SPHelper.StopTraces(conn, runningTraceIds);
                    runningTraceIds = SPHelper.GetRunningTracesXE(conn);
                    if (runningTraceIds != null)
                        SPHelper.StopTracesXE(conn, runningTraceIds);
                    runningTraceIds = SPHelper.GetRunningAuditLogs(conn);
                    if (runningTraceIds != null)
                        SPHelper.StopAuditLogs(conn, runningTraceIds);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default, e, true);
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Traces stopped for " + name);
        }

        // The logic here is we getting a list of current traces on the agent. Running, stopped, etc.  
        // We then add the traceIds to the registry if it is not already there.
        private void UpdateRunningTraceList()
        {
            try
            {
                ArrayList newTraceList = new ArrayList();
                int[] runningTraces;
                int index;

                //Get the list of traces from the agent.
                using (SqlConnection conn = GetConnection())
                {
                    runningTraces = SPHelper.GetRunningTraces(conn);
                }
                ArrayList traceIds = new ArrayList(runningTraces);

                //Get list of traces from the registry
                TraceInfo[] agentTraces = TraceInfo.GetAgentStartedTraceInfo(Alias);

                //find each registry trace in the list of instance traces and remove it from the instance trace list.
                // What is left will be the newly created traces before the stored procedure timed out.
                foreach (TraceInfo trace in agentTraces)
                {
                    index = traceIds.BinarySearch(trace.TraceId);

                    if (index >= 0)
                    {
                        traceIds.RemoveAt(index);
                    }
                    newTraceList.Add(trace);
                }
                traceIds.TrimToSize();

                //convert the array of trace Ids to a single string
                StringBuilder idList = new StringBuilder();

                foreach (int traceId in traceIds)
                    idList.AppendFormat("{0},", traceId);

                TraceInfo[] currentTraces;
                using (SqlConnection conn = GetConnection())
                {
                    currentTraces = SPHelper.GetTraceInfo(conn, idList.ToString());
                }

                //The trace numbers are one based.
                int startTraceNumber = 1;

                //add what is left of the agent trace list to the registry
                for (int i = 0; i < currentTraces.Length; i++)
                {
                    TraceInfo traceInfo = currentTraces[i];
                    traceInfo.TraceNumber = startTraceNumber++;
                    traceInfo.StartTime = DateTime.Now; //make sure this is in the correct time format
                    newTraceList.Add(traceInfo);

                    //add what is left of the agent trace list to the registry
                    if (startTraceNumber > agentTraces.Length)
                        startTraceNumber = 1;
                }
                agentTraces = (TraceInfo[])newTraceList.ToArray(typeof(TraceInfo));
                TraceInfo.SaveAgentStartedTraceInfo(Alias, SQLcomplianceAgent.Instance.AgentVersion, agentTraces);
            }
            catch (Exception e)
            {
            }
        }

        // The logic here is we getting a list of current traces on the agent. Running, stopped, etc.  
        // We then add the extended events traceIds to the registry if it is not already there.
        private void UpdateRunningTraceListXE()
        {
            try
            {
                ArrayList newTraceList = new ArrayList();
                int[] runningTraces;
                int index;

                //Get the list of traces from the agent.
                using (SqlConnection conn = GetConnection())
                {
                    runningTraces = SPHelper.GetRunningTracesXE(conn);
                }
                ArrayList traceIds = new ArrayList(runningTraces);

                //Get list of traces from the registry
                TraceInfo[] agentTracesXE = TraceInfo.GetAgentStartedTraceInfoXE(Alias);

                //find each registry trace in the list of instance traces and remove it from the instance trace list.
                // What is left will be the newly created traces before the stored procedure timed out.
                foreach (TraceInfo trace in agentTracesXE)
                {
                    index = traceIds.BinarySearch(trace.TraceId);

                    if (index >= 0)
                    {
                        traceIds.RemoveAt(index);
                    }
                    newTraceList.Add(trace);
                }
                traceIds.TrimToSize();

                //convert the array of trace Ids to a single string
                StringBuilder idList = new StringBuilder();

                foreach (int traceId in traceIds)
                    idList.AppendFormat("{0},", traceId);

                TraceInfo[] currentTraces;
                using (SqlConnection conn = GetConnection())
                {
                    currentTraces = SPHelper.GetTraceInfoXE(conn, idList.ToString());
                }

                //The trace numbers are one based.
                int startTraceNumber = 1;

                //add what is left of the agent trace list to the registry
                for (int i = 0; i < currentTraces.Length; i++)
                {
                    TraceInfo traceInfo = currentTraces[i];
                    traceInfo.TraceNumber = startTraceNumber++;
                    traceInfo.StartTime = DateTime.Now; //make sure this is in the correct time format
                    newTraceList.Add(traceInfo);

                    //add what is left of the agent trace list to the registry
                    if (startTraceNumber > agentTracesXE.Length)
                        startTraceNumber = 1;
                }
                agentTracesXE = (TraceInfo[])newTraceList.ToArray(typeof(TraceInfo));
                TraceInfo.SaveAgentStartedTraceInfoXE(Alias, SQLcomplianceAgent.Instance.AgentVersion, agentTracesXE);
            }
            catch (Exception e)
            {
            }
        }


        #endregion

        #region Trace Collection Methods

        //-------------------------------------------------------------------------
        // StartCollectingTraces
        //-------------------------------------------------------------------------
        /// <summary>
        /// Starts the collector that collect traces and send them to
        /// the repository server periodically.
        /// </summary>
        private void StartCollectingTraces()
        {
            try
            {
                if (TraceCollectionStopped)
                    return;

                if (collector == null)
                    collector = new TraceCollector(this);

                // Start a thread and collect traces while agent was stopped
                // Note: after switching from TimerState to ServiceTimer, collect
                // automatically collect trace once on Start() call.
                //CollectOnce();

                // Starts the periodic trace collection
                collector.Start();

                if (detector == null)
                    detector = new TamperingDetector(this);
                detector.Start();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred starting trace collector for " + name + ".",
                                         e,
                                         true);
            }
        }

        //-------------------------------------------------------------------------
        // StopCollectingTraces
        //-------------------------------------------------------------------------
        /// <summary>
        /// Stops the collector from collecting traces.
        /// </summary>
        public void StopCollectingTraces()
        {
            if (collector != null)
                collector.Stop();
            collector = null;

            if (detector != null)
                detector.Stop();
            detector = null;
        }

        public void CollectTracesNow()
        {
            RestartTraces(true, null);
            CollectOnce();
        }
        //---------------------------------------------------------------------------
        /// CollectOnce - Start a thread to collect the traces once.
        //---------------------------------------------------------------------------
        private void CollectOnce()
        {
            ThreadStart collectorDelegate = new ThreadStart(CollectDelegate);
            Thread collectorThread = new Thread(collectorDelegate);
            collectorThread.Name = "CollectOnce";
            collectorThread.Start();
        }

        //---------------------------------------------------------------------------
        /// CollectDelegate - Delegate for the collector thread.
        //---------------------------------------------------------------------------
        private void CollectDelegate()
        {
            if (collector == null)
            {
                collector = new TraceCollector(this);
                collector.Interval = Timeout.Infinite;
                collector.Start();
                collector = null;
            }
            else
                collector.CollectTraces(null);
        }

        //---------------------------------------------------------------------------
        /// SetCollectionInterval
        //---------------------------------------------------------------------------
        public void SetCollectionInterval(int newInterval)
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            try
            {
                if (newInterval <= 0)
                    newInterval = CoreConstants.Agent_Default_CollectInterval;

                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(GetRegistrySubKey());
                rks.SetValue(CoreConstants.Agent_RegVal_CollectionInterval, newInterval);

                if (collector != null)
                {
                    collector.Interval = newInterval * 60 * 1000;
                }
                UpdateStatus();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred setting new collection interval for {0}.", name),
                                         e,
                                         true);
            }
            finally
            {
                if (rks != null)
                    rks.Close();
                rks = null;

                if (rk != null)
                    rk.Close();
                rk = null;
            }
        }
        //---------------------------------------------------------------------------
        /// SetForceCollectionInterval
        //---------------------------------------------------------------------------
        public void SetForceCollectionInterval(int newInterval)
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            try
            {
                if (newInterval <= 0)
                    newInterval = CoreConstants.Agent_Default_ForceCollectionInterval;

                if (newInterval == forceCollectionInterval)
                    return;

                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(GetRegistrySubKey());
                rks.SetValue(CoreConstants.Agent_RegVal_ForceCollectionInterval, newInterval);

                forceCollectionInterval = newInterval;
                collector.ForceCollectionInterval = newInterval;
                UpdateStatus();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred setting new force collection interval for {0}.", name),
                                         e,
                                         true);
            }
            finally
            {
                if (rks != null)
                    rks.Close();
                rks = null;

                if (rk != null)
                    rk.Close();
                rk = null;
            }
        }


        public void SetIsCompressedFile(bool isCompressedFile)
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            try
            {
                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(GetRegistrySubKey());
                rks.SetValue(CoreConstants.Agent_RegVal_IsCompressedFile, isCompressedFile);
                UpdateStatus();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred setting new file type for {0}.", name),
                                         e,
                                         true);
            }
            finally
            {
                if (rks != null)
                    rks.Close();
                rks = null;

                if (rk != null)
                    rk.Close();
                rk = null;
            }
        }
        public void SetTraceStartTimeout(int timeout)
        {
            try
            {
                if (timeout <= 0)
                    timeout = CoreConstants.Agent_Default_TraceStartTimeout;

                if (timeout == traceStartTimeout)
                    return;

                traceStartTimeout = timeout;
                UpdateStatus();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred setting new Trace Start timeout for {0}.", name), e, true);
            }
        }

        //---------------------------------------------------------------------------
        /// SetDetectionInterval
        //---------------------------------------------------------------------------
        public void SetDetectionInterval(int newInterval)
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            try
            {
                if (newInterval <= 0)
                    newInterval = CoreConstants.Agent_Default_TamperingDetectionInterval;

                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(GetRegistrySubKey());
                rks.SetValue(CoreConstants.Agent_RegVal_DetectionInterval, newInterval);

                if (detector != null)
                {
                    detector.Interval = newInterval * 1000;
                }
                UpdateStatus();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred setting new detection interval for {0}.", name),
                                         e,
                                         true);
            }
            finally
            {
                if (rks != null)
                    rks.Close();
                rks = null;

                if (rk != null)
                    rk.Close();
                rk = null;
            }
        }


        //-------------------------------------------------------------------------
        // DeleteTraceFiles: deletes the instance's trace files.
        //-------------------------------------------------------------------------
        private void DeleteTraceFiles()
        {
            DirectoryInfo di = new DirectoryInfo(traceDirectory);
            FileInfo[] files = di.GetFiles(alias + "*.trc");
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        files[i].Delete();
                    }
                    catch (Exception e)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                 "An error occurred deleting trace file " + files[i].Name + ".",
                                                 e,
                                                 true);
                    }
                }
            }

            files = di.GetFiles("XE" + alias + "*.xel");
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        files[i].Delete();
                    }
                    catch (Exception e)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                 "An error occurred deleting trace file " + files[i].Name + ".",
                                                 e,
                                                 true);
                    }
                }
            }

            //5.5 Audit Logs
            files = di.GetFiles("AL" + alias + "*.sqlaudit");
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        files[i].Delete();
                    }
                    catch (Exception e)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                 "An error occurred deleting trace file " + files[i].Name + ".",
                                                 e,
                                                 true);
                    }
                }
            }
        }

        public void CheckDataChangeSetup()
        {
            if (DateTime.Now.AddDays(-1) >= lastDCSetupCheckTime)
                triggerManager.UpdateConfiguration(auditConfiguration);
            else
                triggerManager.CheckSharedSetup();
        }

        #endregion

        #region Privileged Users

        //-------------------------------------------------------------------------
        // GetAuditedPrivUsers
        //-------------------------------------------------------------------------
        public string[]
           GetAuditedPrivUsers()
        {
            int[] roles = this.auditConfiguration.AuditedServerRoles;

            if (roles == null || roles.Length == 0)
                return null;

            return UserList.GetAuditedServerRoleUsers(this.name, roles, null);

        }

        #endregion

        #region Agent Error State Handling Methods

        //-----------------------------------------------------------------------
        private void ResetConnectionState()
        {
            SetConnectionState(true, null);
        }

        //-----------------------------------------------------------------------
        private void SetConnectionState(bool healthy, Exception e)
        {
            SystemAlert alert = null;
            string msg = null;
            ErrorLog.Level level = ErrorLog.Level.Default;
            ErrorLog.Severity severity = ErrorLog.Severity.Warning;

            try
            {
                if (healthy)
                {
                    severity = ErrorLog.Severity.Informational;
                    if (!ConnectionHealthy)
                    {
                        msg = String.Format("Connections to {0} reestablished.",
                                             name);
                        alert =
                           new SystemAlert(SystemAlertType.ServerConnectionResolution,
                                            DateTime.UtcNow,
                                            name,
                                            String.Format("Connections to {0} reestablished.",
                                                           name));
                        ConnectionHealthy = true;
                    }
                }
                else
                {
                    msg = String.Format("Connections to {0} Failed.",
                                         name);
                    if (ConnectionHealthy)
                    {
                        alert = new SystemAlert(SystemAlertType.ServerConnectionError,
                                                 DateTime.UtcNow,
                                                 name,
                                                 msg);
                        ConnectionHealthy = false;
                    }
                    else
                    {
                        level = ErrorLog.Level.Debug;
                    }
                    SubmitServerDownStatusAlert(name);
                }

                if (alert != null)
                    SubmitSystemAlert(alert);
                LogError(level, msg, e, severity);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred setting agent connection state.",
                                         ex,
                                         ErrorLog.Severity.Warning,
                                         true);
            }
        }

        //-----------------------------------------------------------------------
        internal void ReportConfigurationError(Exception e)
        {
            ErrorLog.Level level = ErrorLog.Level.Debug;

            string details = String.Format("Error loading configuration for {0}.  Error: {1}.",
                                           name, e.Message);
            if (!ConfigurationFileErrorFound || !configErrors.ContainsKey(e.Message))
            {
                ConfigurationFileErrorFound = true;
                level = ErrorLog.Level.Default;
                configErrors.Add(e.Message, 1);
                SystemAlert alert = new SystemAlert(SystemAlertType.AgentConfigurationError,
                                                     DateTime.UtcNow,
                                                     name,
                                                     details);
                SubmitSystemAlert(alert);

            }
            else
            {
                configErrors[e.Message] += 1;
            }

            LogError(level, details, e, ErrorLog.Severity.Error);

        }

        //-----------------------------------------------------------------------
        internal void ReportConfigurationResolution()
        {
            if (!ConfigurationFileErrorFound)
                return;

            string details =
               String.Format("Agent configuration errors for {0} resolved.", name);
            ConfigurationFileErrorFound = false;
            configErrors.Clear();

            SystemAlert alert =
               new SystemAlert(SystemAlertType.AgentConfigurationResolution,
                                DateTime.UtcNow,
                                name,
                                details);
            LogError(ErrorLog.Level.Default, details, null, ErrorLog.Severity.Informational);
            SubmitSystemAlert(alert);
        }

        //--------------------------------------------------------------------------
        private void
           ReportTraceError(Exception e, string message)
        {
            ErrorLog.Level level = ErrorLog.Level.Debug;
            string details;

            if (String.IsNullOrEmpty(message))
            {
                details = String.Format("An error occurred starting traces for instance {0}.  Error: {1}.", name, e.Message);
            }
            else
            {
                details = String.Format("An error occurred starting traces for instance {0}.  Error: {1}. {2}", name, e.Message, message);
            }

            if (!TraceErrorFound)
            {
                TraceErrorFound = true;
                level = ErrorLog.Level.Default;

                SystemAlert alert =
                   new SystemAlert(SystemAlertType.SqlTraceError,
                                    DateTime.UtcNow,
                                    name,
                                    details);
                SubmitSystemAlert(alert);
            }

            ErrorLog.Instance.Write(level,
               details,
               e,
               ErrorLog.Severity.Warning,
               true);

        }

        //--------------------------------------------------------------------------
        private void
           ReportTraceResolution()
        {
            if (!TraceErrorFound)
                return;

            TraceErrorFound = false;
            string details = String.Format("Trace exception conditions resolved for {0}.", name);
            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                     details);
            SystemAlert alert =
               new SystemAlert(SystemAlertType.SqlTraceResolution,
                                DateTime.UtcNow,
                                name,
                                details);
            SubmitSystemAlert(alert);
        }

        #endregion

        #region System Alerts and Error Logs

        //-----------------------------------------------------------------------
        public void LogError(ErrorLog.Level level, string msg, Exception e, ErrorLog.Severity severity)
        {
            if (e != null)
                ErrorLog.Instance.Write(level, msg, e, severity, true);
            else
                ErrorLog.Instance.Write(level, msg, severity);
        }

        //-----------------------------------------------------------------------
        // SubmitSystemAlert - submit system alert to the collection service
        //-----------------------------------------------------------------------
        public void
           SubmitSystemAlert(
              SystemAlert alert
           )
        {
            SubmitSystemAlert(alert, true);
        }


        //-----------------------------------------------------------------------
        // SubmitSystemAlert - submit system alert to the collection service
        //-----------------------------------------------------------------------
        public void
           SubmitSystemAlert(
              SystemAlert alert,
              bool noMoreErrors
           )
        {
            try
            {
                SQLcomplianceAgent.Instance.SubmitSystemAlert(syncObj, alert, noMoreErrors);
            }
            catch (Exception e)
            {
                // We were probably called asynchronously, so nothing is around to catch the exception
                // but we still need to log it
                ErrorLog.Instance.Write(e, ErrorLog.Severity.Warning);
            }
        }

        public void SubmitServerDownStatusAlert(string instance)
        {
            try
            {
                SQLcomplianceAgent.Instance.SubmitServerDownStatusAlert(syncObj, instance);
            }
            catch (Exception e)
            {
                // We were probably called asynchronously, so nothing is around to catch the exception
                // but we still need to log it
                ErrorLog.Instance.Write(e, ErrorLog.Severity.Warning);
            }
        }

        #endregion

        #region Audit Logs
        // The logic here is we getting a list of current traces on the agent. Running, stopped, etc.  
        // We then add the traceIds to the registry if it is not already there.
        private void UpdateRunningAuditLogList()
        {
            try
            {
                ArrayList newTraceList = new ArrayList();
                int[] runningTraces;
                int index;

                //Get the list of traces from the agent.
                using (SqlConnection conn = GetConnection())
                {
                    runningTraces = SPHelper.GetRunningAuditLogs(conn);
                }
                ArrayList traceIds = new ArrayList(runningTraces);

                //Get list of traces from the registry
                TraceInfo[] agentTraces = TraceInfo.GetAgentStartedTraceInfo(Alias);

                //find each registry trace in the list of instance traces and remove it from the instance trace list.
                // What is left will be the newly created traces before the stored procedure timed out.
                foreach (TraceInfo trace in agentTraces)
                {
                    index = traceIds.BinarySearch(trace.TraceId);

                    if (index >= 0)
                    {
                        traceIds.RemoveAt(index);
                    }
                    newTraceList.Add(trace);
                }
                traceIds.TrimToSize();

                //convert the array of trace Ids to a single string
                StringBuilder idList = new StringBuilder();

                foreach (int traceId in traceIds)
                    idList.AppendFormat("{0},", traceId);

                TraceInfo[] currentTraces;
                using (SqlConnection conn = GetConnection())
                {
                    currentTraces = SPHelper.GetTraceInfo(conn, idList.ToString());
                }

                //The trace numbers are one based.
                int startTraceNumber = 1;

                //add what is left of the agent trace list to the registry
                for (int i = 0; i < currentTraces.Length; i++)
                {
                    TraceInfo traceInfo = currentTraces[i];
                    traceInfo.TraceNumber = startTraceNumber++;
                    traceInfo.StartTime = DateTime.Now; //make sure this is in the correct time format
                    newTraceList.Add(traceInfo);

                    //add what is left of the agent trace list to the registry
                    if (startTraceNumber > agentTraces.Length)
                        startTraceNumber = 1;
                }
                agentTraces = (TraceInfo[])newTraceList.ToArray(typeof(TraceInfo));
                TraceInfo.SaveAgentStartedTraceInfo(Alias, SQLcomplianceAgent.Instance.AgentVersion, agentTraces);
            }
            catch (Exception e)
            {
            }
        }

        #endregion
    }
}
