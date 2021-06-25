using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Idera.SQLcompliance.Core.Beta;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Remoting;
using Idera.SQLcompliance.Core.Security;
using Idera.SQLcompliance.Core.Status;
using Microsoft.Win32;
using System.Reflection;
using Xceed.Compression.Formats;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Security;
using System.Runtime.Serialization.Formatters;
using System.Configuration;

namespace Idera.SQLcompliance.Core.Agent
{
    /// <summary>
    /// Summary description for SQLcomplianceAgent.
    /// </summary>
    public class SQLcomplianceAgent : PermissionsCheckBase
    {
        #region Constants

        public const string DirectorySizeLimitReached = "agent trace directory size limit reached";

        #endregion
        #region Private Data Members

        private static SQLcomplianceAgent _instance;
        private static bool isClustered = false;
        private string virtualServerName = null;
        private static string agentRegistryKey = CoreConstants.Agent_RegKey;
        private static bool skipAdminCheck = false;
        private static string assemblyRootDirectory = null;

        private ServiceTimer heartbeat;              // heartbeat timer
        private AgentConfiguration configuration;
        //private bool                      monitorEventLog;
        private bool stopping = false;
        private bool maxFolderSizeReached = false;
        private TcpServerChannel tcpChannel;
        private Hashtable sqlInstances = new Hashtable();
        private object startStopLock = new Object();
        private bool serverAvailable = true;
        private Dictionary<string, int> directoryErrors = new Dictionary<string, int>();  // <errorString, count>
        private bool systemAlertError = false;
        private int _ipAddressBindType;
        private static string[] instancesWithPort;
        private static string[] instances;

        // For monitoring SQL Server activities
        //private static EventLog           eventLog;

        #endregion

        #region Properties

        public static SQLcomplianceAgent Instance
        {
            get { return _instance; }
        }

        public bool Stopping
        {
            get { return stopping; }
        }

        public string Server
        {
            get { return configuration.Server; }
        }

        public int ServerPort
        {
            get { return configuration.ServerPort; }
        }

        public string Name
        {
            get { return configuration.AgentName; }
        }

        public string AgentServer
        {
            get { return configuration.AgentServer; }
        }

        public int AgentPort
        {
            get { return configuration.AgentPort; }
        }

        public string AgentVersion
        {
            get { return configuration.AgentVersion; }
        }

        public int CollectionInterval
        {
            get { return configuration.CollectionInterval; }
        }

        public int ForceCollectionInterval
        {
            get { return configuration.ForceCollectionInterval; }
        }

        public int TraceStartTimeout
        {
            get { return configuration.TraceStartTimeout; }
        }

        public int DetectionInterval
        {
            get { return configuration.DetectionInterval; }
        }

        public int MaxTraceSize
        {
            get { return configuration.MaxTraceSize; }
        }

        public TraceOption TraceOptions
        {
            get { return configuration.TraceOptions; }
        }

        public string TraceDirectory
        {
            get { return configuration.TraceDirectory; }
        }

        public int MaxUnattendedTime
        {
            get { return configuration.MaxUnattendedTime; }
        }

        public bool IsCompressedFile
        {
            get { return configuration.IsCompressedFile; }
        }

        public AgentConfiguration Configuration
        {
            get { return configuration; }
        }

        public bool IsClustered
        {
            get { return isClustered; }
            set { isClustered = value; }
        }

        public string VirtualServerName
        {
            get { return virtualServerName; }
            set { virtualServerName = value; }
        }

        public string AssemblyRootDirectory
        {
            get { return assemblyRootDirectory; }
            set { assemblyRootDirectory = value; }
        }

        public static string AgentRegistryKey
        {
            get { return agentRegistryKey; }
        }

        public bool MaxFolderSizeReached
        {
            get { return maxFolderSizeReached; }
        }

        public bool ServerAvailable
        {
            get
            {
                return serverAvailable;
            }
            set
            {
                serverAvailable = value;
            }
        }

        #endregion

        #region Constructors


        static SQLcomplianceAgent()
        {
            _instance = new SQLcomplianceAgent();
        }

        private SQLcomplianceAgent()
        {

            configuration = new AgentConfiguration();

            // If we are monitoring the SQL Server via EventLog
            // initialized it as necessary.
            /* Not used
            if( monitorEventLog && eventLog == null  )
               eventLog = new EventLog("System");
            */

        }

        #endregion

        #region Agent Start/Stop/Pause/Resume
        //----------------------------------------------------------------------------
        // Called by SQLcomplianceAgentService OnStart or other applications: This is the
        // method to bootstrap the SQLcomplianceAgent.
        //----------------------------------------------------------------------------
        public void Start()
        {
            lock (startStopLock)  // dont run stop until start is complete
            {
                try
                {
                    stopping = false;

                    // Set logging to event log
                    ErrorLog.Instance.LogToEventLog = true;
                    if (isClustered)
                    {
                        ErrorLog.Instance.EventLogSource = String.Format("{0}${1}", CoreConstants.EventLogSource_AgentService, virtualServerName);
                        agentRegistryKey = String.Format("{0}${1}", CoreConstants.Agent_RegKey, virtualServerName);
                        configuration.AgentServer = virtualServerName;
                    }
                    else
                    {
                        ErrorLog.Instance.EventLogSource = CoreConstants.EventLogSource_AgentService;
                    }

                    // Check for local admin privileges
                    GetSkipAdminCheck(); // Check if user has asked us to skip the admin check
                    if (!skipAdminCheck)
                    {
                        if (!SecurityHelper.IsLocalAdmin())
                        {
                            // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
                            // Support SQLCM for Non Admin role
                            // throw new Exception(CoreConstants.Exception_ServiceAccountNotLocalAdmin);
                        }
                    }
                    if (stopping) return;

                    /* Commenting the Secure Implementation due to Agent Issues
                        // Create the composite channel that will encapsulate all of our client channels (enables hybrid secure/unsecure TCP)
                       RemotingCompositeChannel cc = new RemotingCompositeChannel(CoreConstants.AgentClientName, UriComparisonMode.WildcardMatch);

                       // Add the secure TCP channel
                       Hashtable tcpChannelProps = new Hashtable();
                       tcpChannelProps["name"] = CoreConstants.AgentClientName;
                       tcpChannelProps["secure"] = true;
                       tcpChannelProps["protectionLevel"] = ProtectionLevel.EncryptAndSign;
                       tcpChannelProps["tokenImpersonationLevel"] = TokenImpersonationLevel.Impersonation;
                       TcpClientChannel secureChannel = new TcpClientChannel(tcpChannelProps, new BinaryClientFormatterSinkProvider());
                       secureChannel.IsSecured = true;
                       cc.Add(@"^tcp:\/\/.*(?<!\+Open)$", secureChannel);

                       // Register the channel
                       ChannelServices.RegisterChannel(cc, false);
                     * */

                    Initialize();
                    if (stopping) return;

                    // Start accepting commands from server.
                    StartServerListener();
                    if (stopping) return;

                    SendStartupMessage();
                    if (stopping) return;

                    ValidateTraceDirectory(configuration.TraceDirectory);
                    CheckTraceFolderSize();

                    StartAuditing();
                    if (stopping) return;

                    StartHeartbeating();
                    if (stopping) return;

                    // Start monitoring SQL Server Start/Stop via EventLog.
                    //MonitorSQLServer( true );
                    //if ( stopping ) return;

                    // all initialization done successfully!
                    configuration.IsCrippled = false;
                    configuration.IsRunning = true;
                    ErrorLog.Instance.Write("Service started successfully.");
                    UpdateAuditConfiguration();
                    SetIsCompressedFile(true);                    
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write(CoreConstants.Exception_AgentStartup, e);
                    Process p = Process.GetCurrentProcess();
                    p.Kill();
                }
            }
        }

        //-----------------------------------------------------------------------
        // Stops all traces and terminate all worker threads.
        //-----------------------------------------------------------------------
        public void Stop()
        {
            stopping = true;       // signal that we are trying to stop

            lock (startStopLock) // dont run stop until start is complete
            {
                try
                {
                    StopHeartbeating();
                    StopCollectingTraces();

                    if (StopTracesOnShutdown())
                    {
                        DisableAuditing();
                        DeleteTraceDirectory();
                    }
                    // SQLCM-5846 - SQLCM traces getting collected on Passive nodes of an AG
                    else if (isClustered)
                    {
                        DisableAuditing();
                    }

                    SendShutdownMessage();
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, CoreConstants.AgentServiceName + " shutting down.");
                    sqlInstances.Clear();
                    ChannelServices.UnregisterChannel(tcpChannel);
                    ErrorLog.Instance.Write("Service stopped successfully.");
                }
                catch (Exception e)
                {
                    // Ignore all exceptions
                    ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug, e, true);
                }
            }
        }
        #endregion

        #region Initialization, Ping and Configurations

        //---------------------------------------------------------------
        // Initialize - Load agent configuration and initialize instances
        //---------------------------------------------------------------
        private void Initialize()
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            StringBuilder msg = new StringBuilder();

            try
            {
                rk = Registry.LocalMachine;
                rks = rk.OpenSubKey(agentRegistryKey, true);
                configuration.StartupTime = DateTime.UtcNow;

                // Agent level configurations
                configuration.Server = (string)rks.GetValue(CoreConstants.Agent_RegVal_Server,
                                                                         CoreConstants.Agent_Default_Server);
                configuration.ServerPort = (int)rks.GetValue(CoreConstants.Agent_RegVal_ServerPort,
                                                                         CoreConstants.CollectionServerTcpPort);
                configuration.AgentPort = (int)rks.GetValue(CoreConstants.Agent_RegVal_AgentPort,
                                                                      CoreConstants.AgentServerTcpPort);

                //We do not want a default.  If there is not a value in the registry, it will you the directory where the agent is currently running
                assemblyRootDirectory = (string)rks.GetValue(CoreConstants.Agent_RegVal_AssemblyRootDirectory);

                if (String.IsNullOrEmpty(assemblyRootDirectory))
                {
                    ClusterAgentUpgrade agentUpgrade = GetClusterAgentUpgradeInfo();

                    if (agentUpgrade != null && !String.IsNullOrEmpty(virtualServerName))
                    {
                        assemblyRootDirectory = GetAssemblyRootDirectory(agentUpgrade);

                        if (!String.IsNullOrEmpty(assemblyRootDirectory))
                        {
                            rks.SetValue(CoreConstants.Agent_RegVal_AssemblyRootDirectory, assemblyRootDirectory);
                        }
                    }
                }

                ErrorLog.Instance.ErrorLevel = (ErrorLog.Level)rks.GetValue(CoreConstants.Agent_RegVal_LogLevel,
                                                                                CoreConstants.Agent_Default_LogLevel);
                configuration.LogLevel = (int)ErrorLog.Instance.ErrorLevel;
                configuration.HeartbeatInterval = (int)rks.GetValue(CoreConstants.Agent_RegVal_HeartbeatInterval,
                                                                      CoreConstants.Agent_Default_HeartbeatInterval);
                if (configuration.HeartbeatInterval <= 0)
                    configuration.HeartbeatInterval = CoreConstants.Agent_Default_HeartbeatInterval;

                // 5.8 File Uncompress transfer mode
                configuration.IsCompressedFile = Convert.ToBoolean(rks.GetValue(CoreConstants.Agent_RegVal_IsCompressedFile, true));
                // Optional configurations for overriding SQL Server instance configurations
                configuration.CollectionInterval = (int)rks.GetValue(CoreConstants.Agent_RegVal_CollectionInterval,
                                                                      CoreConstants.Agent_Default_CollectInterval);
                if (configuration.CollectionInterval <= 0)
                    configuration.CollectionInterval = CoreConstants.Agent_Default_CollectInterval;
                configuration.ForceCollectionInterval = (int)rks.GetValue(CoreConstants.Agent_RegVal_ForceCollectionInterval,
                                                                            CoreConstants.Agent_Default_ForceCollectionInterval);
                configuration.DetectionInterval = (int)rks.GetValue(CoreConstants.Agent_RegVal_DetectionInterval,
                                                                     CoreConstants.Agent_Default_TamperingDetectionInterval);
                if (configuration.DetectionInterval <= 0)
                    configuration.DetectionInterval = CoreConstants.Agent_Default_TamperingDetectionInterval;

                if (configuration.TraceStartTimeout <= 0)
                    configuration.TraceStartTimeout = CoreConstants.Agent_Default_TraceStartTimeout;

                configuration.MaxTraceSize = (int)rks.GetValue(CoreConstants.Agent_RegVal_MaxTraceSize,
                                                                      CoreConstants.Agent_Default_MaxTraceSize);
                if (configuration.MaxTraceSize <= 0)
                    configuration.MaxTraceSize = CoreConstants.Agent_Default_MaxTraceSize;
                configuration.TraceOptions = (TraceOption)rks.GetValue(CoreConstants.Agent_RegVal_TraceOptions,
                                                                              CoreConstants.Agent_Default_TraceOptions);
                configuration.MaxFolderSize = (int)rks.GetValue(CoreConstants.Agent_RegVal_MaxFolderSize,
                                                                 CoreConstants.Agent_Default_MaxFolderSize);
                configuration.MaxUnattendedTime = (int)rks.GetValue(CoreConstants.Agent_RegVal_MaxUnattendedTime,
                                                                     CoreConstants.Agent_Default_MaxUnattendedTime);
                configuration.TraceDirectory = (string)rks.GetValue(CoreConstants.Agent_RegVal_TraceDirectory,
                                                                     configuration.TraceDirectory);
                int enumerateUsersInterval = (int)rks.GetValue(CoreConstants.Agent_RegVal_EnumerateUsersInterval,
                                                                CoreConstants.Agent_Default_EnumerateUsersInterval);
                // Trace file transfer
                CoreConstants.FileTransferPageSize = (int)rks.GetValue(CoreConstants.Agent_RegVal_FileTransferPageSize,
                                                                        CoreConstants.Agent_Default_FileTransferPageSize);
                // Either use server/client activated remoting objects for trace file transfers.
                // Use server activated objects by default.
                int ftMode = (int)rks.GetValue(CoreConstants.Agent_RegVal_UseClientActivatedFileTransfer, 0);
                CoreConstants.UseClientActivatedFileTransfer = ftMode > 0;

                // 0 - either, 4 - IPv4, 6 - IPv6
                _ipAddressBindType = (int)rks.GetValue(CoreConstants.Agent_RegVal_IPAddressBindType, 4);

                // Set a default for invalid values
                if (_ipAddressBindType != 0 && _ipAddressBindType != 4 && _ipAddressBindType != 6)
                {
                    _ipAddressBindType = 4;
                }

                // Get service account
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                configuration.ServiceAccount = identity.Name;

                // Initialize ConfigurationHelper properties
                ConfigurationHelper.Server = configuration.Server;
                ConfigurationHelper.ServerPort = configuration.ServerPort;
                if (enumerateUsersInterval <= 0)
                    enumerateUsersInterval = CoreConstants.Agent_Default_EnumerateUsersInterval;
                ConfigurationHelper.EnumerationInterval = new TimeSpan(enumerateUsersInterval / 60,
                                                                      enumerateUsersInterval % 60,
                                                                      0);
                msg.AppendFormat(CoreConstants.Info_AgentSettings,
                                  configuration.AgentName,
                                  configuration.AgentVersion,
                                  configuration.TraceDirectory,
                                  configuration.Server,
                                  configuration.ServerPort,
                                  configuration.AgentPort,
                                  configuration.LogLevel);

                // validate tracedir
                configuration.TraceDirectory = FixTraceDirectory(configuration.TraceDirectory);
                ValidateTraceDirectory(configuration.TraceDirectory);
                configuration.IsRunning = true;

                // Get SQL Server instances
                instances = (string[])rks.GetValue("Instances", null);
                instancesWithPort = (string[])rks.GetValue("InstancesWithPort", null);
                CoreHelpers.TurnOffCustomRemotingErrors();
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Agent Init - end");
            }
            catch (Exception ex)
            {
                // Error reading registry - log the error
                ErrorLog.Instance.Write(String.Format(CoreConstants.Exception_CantReadAgentConfiguration, ex.Message), ex);
                throw ex;
            }
            finally
            {
                if (rks != null)
                    rks.Close();

                if (rk != null)
                    rk.Close();
            }

            if (instances == null)
            {
                // No configured instances.
                ErrorLog.Instance.Write(ErrorLog.Level.Default, msg.ToString());
                return;
            }

            // Initializes instances
            SQLInstance sqlInstance = null;
            bool validated;
            bool isValid;

            foreach (string instanceName in instances)
            {
                validated = true;
                isValid = true;
                try
                {
                    isValid = ValidSqlServerOSCombo(instanceName);
                }
                catch (Exception e)
                {
                    validated = false;
                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                            String.Format("An error occurred validating {0} version number.", instanceName),
                                            e,
                                            ErrorLog.Severity.Error);
                }

                try
                {

                    if (validated && !isValid)
                    {
                        ErrorLog.Instance.Write(String.Format("{0}: {1}", instanceName, CoreConstants.Exception_InvalidSQLServerOSCombo), ErrorLog.Severity.Error);
                    }
                    else
                    {
                        ClearSystemAlertFlags(instanceName);
                        sqlInstance = new SQLInstance(instanceName);
                        sqlInstances.Add(sqlInstance.Name, sqlInstance);
                        sqlInstance.validated = validated;
                        sqlInstance.DetectionInterval = configuration.DetectionInterval;

                        // Create new instance status
                        CreateInstanceStatus(sqlInstance);
                        msg.AppendFormat(CoreConstants.Info_InstanceSettings, instanceName, sqlInstance.ConfigVersion);
                    }
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                             String.Format("An error occurred initializing {0}.", instanceName),
                                             e,
                                             ErrorLog.Severity.Error);
                    //we were not able to initialize the instance, go ahead an add it so we will catch it on the next heartbeat.
                    sqlInstances.Add(instanceName, sqlInstance);
                }
            }

            if (BetaHelper.IsBeta)
            {
                msg.AppendFormat("\n\n{0}", BetaHelper.Title_Beta);
            }

            ErrorLog.Instance.Write(ErrorLog.Level.Default, msg.ToString());
        }

        //Deserialize the Cluster agent upgrade information
        private ClusterAgentUpgrade GetClusterAgentUpgradeInfo()
        {
            ClusterAgentUpgrade agentUpgrade = null;

            try
            {
                FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                string path = Path.Combine(fileInfo.DirectoryName, CoreConstants.ClusterAgentUpgradeFilename);

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                agentUpgrade = (ClusterAgentUpgrade)formatter.Deserialize(stream);
                stream.Close();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Unable to get the cluster agent upgrade info.", e, ErrorLog.Severity.Error);
            }
            return agentUpgrade;
        }

        private string GetAssemblyRootDirectory(ClusterAgentUpgrade agentUpgrade)
        {
            string assemblyDirectory;

            try
            {
                if (agentUpgrade.AssemblyDirectories.TryGetValue(virtualServerName, out assemblyDirectory))
                {
                    return assemblyDirectory;
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "The assembly directory is not set.  Will use the default.", e, ErrorLog.Severity.Error);
            }
            return null;
        }


        //-----------------------------------------------------------------
        // SetAgentConfiguration: obsolete since 2.1.  Leave for backward
        //                        compatibility
        //-----------------------------------------------------------------
        public void SetAgentConfiguration(int logLevel,
                                          int heartbeatInterval,
                                          int collectionInterval,
                                          int forceCollectionInterval,
                                          int maxTraceSize,
                                          int maxFolderSize,
                                          int maxUnattendedTime
                                        )
        {
            SetLogLevel((ErrorLog.Level)logLevel);
            SetHeartbeatInterval(heartbeatInterval);
            SetCollectionInterval(collectionInterval);
            SetForceCollectionInterval(forceCollectionInterval);
            SetMaxFolderSize(maxFolderSize);
            SetMaxUnattendedTime(maxUnattendedTime);
            SetMaxTraceSize(maxTraceSize);
        }

        //-----------------------------------------------------------------
        // SetAgentConfiguration: obsolete since 3.2.  Leave for backward
        //                        compatibility
        //-----------------------------------------------------------------
        public void SetAgentConfiguration(int logLevel,
                                          int heartbeatInterval,
                                          int collectionInterval,
                                          int forceCollectionInterval,
                                          int maxTraceSize,
                                          int maxFolderSize,
                                          int maxUnattendedTime,
                                          int detectionInterval)
        {
            SetLogLevel((ErrorLog.Level)logLevel);
            SetHeartbeatInterval(heartbeatInterval);
            SetCollectionInterval(collectionInterval);
            SetForceCollectionInterval(forceCollectionInterval);
            SetMaxFolderSize(maxFolderSize);
            SetMaxUnattendedTime(maxUnattendedTime);
            SetMaxTraceSize(maxTraceSize);
            SetDectionInterval(detectionInterval);
        }

        //-----------------------------------------------------------------
        // SetAgentConfiguration
        //-----------------------------------------------------------------
        public void SetAgentConfiguration(int logLevel,
                                          int heartbeatInterval,
                                          int collectionInterval,
                                          int forceCollectionInterval,
                                          int maxTraceSize,
                                          int maxFolderSize,
                                          int maxUnattendedTime,
                                          int detectionInterval,
                                          int traceStartTimeout)
        {
            SetLogLevel((ErrorLog.Level)logLevel);
            SetHeartbeatInterval(heartbeatInterval);
            SetCollectionInterval(collectionInterval);
            SetForceCollectionInterval(forceCollectionInterval);
            SetMaxFolderSize(maxFolderSize);
            SetMaxUnattendedTime(maxUnattendedTime);
            SetMaxTraceSize(maxTraceSize);
            SetDectionInterval(detectionInterval);
            SetTraceStartTimeout(traceStartTimeout);
        }
        //-----------------------------------------------------------------
        // SetAgentConfiguration
        //-----------------------------------------------------------------
        public void SetAgentConfiguration(int logLevel,
                                          int heartbeatInterval,
                                          int collectionInterval,
                                          int forceCollectionInterval,
                                          int maxTraceSize,
                                          int maxFolderSize,
                                          int maxUnattendedTime,
                                          int detectionInterval,
                                          int traceStartTimeout,
                                          bool isCompressedFile)
        {
            SetLogLevel((ErrorLog.Level)logLevel);
            SetHeartbeatInterval(heartbeatInterval);
            SetCollectionInterval(collectionInterval);
            SetForceCollectionInterval(forceCollectionInterval);
            SetMaxFolderSize(maxFolderSize);
            SetMaxUnattendedTime(maxUnattendedTime);
            SetMaxTraceSize(maxTraceSize);
            SetDectionInterval(detectionInterval);
            SetTraceStartTimeout(traceStartTimeout);
            SetIsCompressedFile(isCompressedFile);
        }

        //-----------------------------------------------------------------
        // SetLogLevel
        //-----------------------------------------------------------------
        public void SetLogLevel(ErrorLog.Level logLevel)
        {
            if (logLevel == ErrorLog.Instance.ErrorLevel)
                return;

            try
            {
                ErrorLog.Instance.ErrorLevel = logLevel;
                SetRegistryValue(CoreConstants.Agent_RegVal_LogLevel, (int)logLevel);
                configuration.LogLevel = (int)logLevel;
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Agent log level changed.");
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetHeartbeatInterval
        //-----------------------------------------------------------------
        public void SetHeartbeatInterval(int newInterval)
        {
            try
            {
                if (newInterval <= 0)
                    newInterval = CoreConstants.Agent_Default_HeartbeatInterval;

                if (newInterval == configuration.HeartbeatInterval)
                    return;

                ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Setting new heartbeat interval to {0}", newInterval));
                SetRegistryValue(CoreConstants.Agent_RegVal_HeartbeatInterval, newInterval);
                configuration.HeartbeatInterval = newInterval;

                if (heartbeat != null)
                {
                    heartbeat.Interval = newInterval * 60 * 1000;
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred setting new heartbeat interval.", e, true);
            }
        }

        //-----------------------------------------------------------------
        // SetIsCompressedFile
        //-----------------------------------------------------------------
        public void SetIsCompressedFile(bool isCompressed)
        {
            try
            {                
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Setting new File type compression to {0}", isCompressed));
                if (configuration.IsCompressedFile == isCompressed)
                    return;

                configuration.IsCompressedFile = isCompressed;
                SetRegistryValue(CoreConstants.Agent_RegVal_IsCompressedFile,
                                  isCompressed);

                string[] instances = GetInstances();
                for (int i = 0; i < instances.Length; i++)
                    SetIsCompressedFile(instances[i], isCompressed);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred setting File type.", e, true);
            }
        }
        //-----------------------------------------------------------------
        public void SetIsCompressedFile(string instanceName, bool isCompressed)
        {
            try
            {
                SQLInstance sqlInstance = GetSQLInstance(instanceName);
                sqlInstance.SetIsCompressedFile(isCompressed);
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetForceCollectionInterval
        //-----------------------------------------------------------------
        public void SetForceCollectionInterval(int newInterval)
        {
            try
            {
                if (newInterval <= 0)
                    newInterval = CoreConstants.Agent_Default_ForceCollectionInterval;

                if (configuration.ForceCollectionInterval == newInterval)
                    return;

                configuration.ForceCollectionInterval = newInterval;
                SetRegistryValue(CoreConstants.Agent_RegVal_ForceCollectionInterval, newInterval);
                string[] instances = GetInstances();
                for (int i = 0; i < instances.Length; i++)
                    SetForceCollectionInterval(instances[i], newInterval);
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetForceCollectionInterval
        //-----------------------------------------------------------------
        public void SetForceCollectionInterval(string instanceName, int interval)
        {
            try
            {
                SQLInstance sqlInstance = GetSQLInstance(instanceName);
                sqlInstance.SetForceCollectionInterval(interval);
            }
            catch { }
        }

        public void SetTraceStartTimeout(string instanceName, int timeout)
        {
            try
            {
                SQLInstance sqlInstance = GetSQLInstance(instanceName);
                sqlInstance.SetTraceStartTimeout(timeout);
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetTraceStartTimeout
        //-----------------------------------------------------------------
        public void SetTraceStartTimeout(int timeout)
        {
            try
            {
                if (timeout <= 0)
                    timeout = CoreConstants.Agent_Default_TraceStartTimeout;

                if (configuration.TraceStartTimeout == timeout)
                    return;

                configuration.TraceStartTimeout = timeout;
                string[] instances = GetInstances();

                for (int i = 0; i < instances.Length; i++)
                    SetTraceStartTimeout(instances[i], timeout);
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetCollectionInterval
        //-----------------------------------------------------------------
        public void SetCollectionInterval(int newInterval)
        {
            try
            {
                if (newInterval <= 0)
                    newInterval = CoreConstants.Agent_Default_CollectInterval;

                if (configuration.CollectionInterval == newInterval)
                    return;

                configuration.CollectionInterval = newInterval;
                SetRegistryValue(CoreConstants.Agent_RegVal_CollectionInterval,
                                  newInterval);

                string[] instances = GetInstances();
                for (int i = 0; i < instances.Length; i++)
                    SetCollectionInterval(instances[i], newInterval);
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetCollectionInterval
        //-----------------------------------------------------------------
        public void SetCollectionInterval(string instanceName, int interval)
        {
            try
            {
                SQLInstance sqlInstance = GetSQLInstance(instanceName);
                sqlInstance.SetCollectionInterval(interval);
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetCollectionInterval
        //-----------------------------------------------------------------
        public void SetDectionInterval(int newInterval)
        {
            try
            {
                if (newInterval <= 0)
                    newInterval = CoreConstants.Agent_Default_TamperingDetectionInterval;

                if (configuration.DetectionInterval == newInterval)
                    return;

                ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Setting new detection interval to {0}", newInterval));
                configuration.DetectionInterval = newInterval;
                SetRegistryValue(CoreConstants.Agent_RegVal_DetectionInterval, newInterval);
                string[] instances = GetInstances();
                for (int i = 0; i < instances.Length; i++)
                    SetDetectionInterval(instances[i], newInterval);
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetCollectionInterval
        //-----------------------------------------------------------------
        public void SetDetectionInterval(string instanceName, int interval)
        {
            try
            {
                SQLInstance sqlInstance = GetSQLInstance(instanceName);
                sqlInstance.SetDetectionInterval(interval);
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetTraceDirectory - Set a new trace directory
        //-----------------------------------------------------------------
        public void SetTraceDirectory(string traceDirectory)
        {
            ArrayList aliasList = new ArrayList();
            try
            {
                if (traceDirectory == null)
                    return;

                if (traceDirectory == configuration.TraceDirectory)
                    return;

                string oldDirectory = configuration.TraceDirectory;

                ValidateTraceDirectory(traceDirectory);
                configuration.TraceDirectory = traceDirectory;
                SetRegistryValue(CoreConstants.Agent_RegVal_TraceDirectory, traceDirectory);

                // Restart the traces for all instances.
                SQLInstance sqlInstance = null;
                IDictionaryEnumerator enumerator = sqlInstances.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    try
                    {
                        sqlInstance = (SQLInstance)enumerator.Value;
                        sqlInstance.TraceDirectory = traceDirectory;
                        aliasList.Add(sqlInstance.Alias);
                        sqlInstance.RestartTraces(true);
                    }
                    catch (Exception e)
                    {
                        string msg = String.Format("An error occurred restarting the traces for {0}.", sqlInstance.Name);
                        ErrorLog.Instance.Write(msg, e, true);
                        throw new Exception(msg, e);
                    }
                }

                // Move the files in the old directory to the new one
                foreach (string alias in aliasList)
                {
                    string[] files = Directory.GetFiles(oldDirectory, alias + "*.trc");
                    FileInfo fileInfo;
                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            try
                            {
                                fileInfo = new FileInfo(files[i]);
                                fileInfo.MoveTo(Path.Combine(traceDirectory, fileInfo.Name));
                            }
                            catch { }
                        }

                    }

                    files = Directory.GetFiles(oldDirectory, "XE" + alias + "*.xel");
                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            try
                            {
                                fileInfo = new FileInfo(files[i]);
                                fileInfo.MoveTo(Path.Combine(traceDirectory, fileInfo.Name));
                            }
                            catch { }
                        }

                    }

                    //5.5 Audit Logs
                    files = Directory.GetFiles(oldDirectory, "AL" + alias + "*.sqlaudit");
                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            try
                            {
                                fileInfo = new FileInfo(files[i]);
                                fileInfo.MoveTo(Path.Combine(traceDirectory, fileInfo.Name));
                            }
                            catch { }
                        }

                    }

                    files = Directory.GetFiles(oldDirectory, alias + "*.bin");

                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            try
                            {
                                fileInfo = new FileInfo(files[i]);
                                fileInfo.MoveTo(Path.Combine(traceDirectory, fileInfo.Name));
                            }
                            catch { }
                        }
                    }
                }

                // delete directory if its empty now that we have moved all our fiels
                try
                {
                    Directory.Delete(oldDirectory);
                }
                catch { }
            }
            catch (Exception e)
            {
                string msg = String.Format("An error occurred setting new trace directory {0}. Error: {1}.", traceDirectory, e.Message.Replace("\\", "\\\\"));
                ErrorLog.Instance.Write(msg, e, true);
                throw new Exception(msg, e);
            }
        }

        //-----------------------------------------------------------------
        // DeleteTraceDirectory - Delete the trace directory
        //-----------------------------------------------------------------
        private void DeleteTraceDirectory()
        {
            ArrayList aliasList = new ArrayList();
            string traceDirectory = configuration.TraceDirectory;

            try
            {
                IDictionaryEnumerator enumerator = sqlInstances.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    try
                    {
                        aliasList.Add(((SQLInstance)enumerator.Value).Alias);
                    }
                    catch { }
                    {
                    }
                }

                // Delete trace files and .bin files for each instance
                foreach (string alias in aliasList)
                {
                    string[] files = Directory.GetFiles(traceDirectory, alias + "*.trc");
                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            try
                            {
                                File.Delete(files[i]);
                            }
                            catch { }
                        }
                    }

                    files = Directory.GetFiles(traceDirectory, "XE" + alias + "*.xel");
                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            try
                            {
                                File.Delete(files[i]);
                            }
                            catch { }
                        }
                    }

                    //5.5 Audit Logs
                    files = Directory.GetFiles(traceDirectory, "AL" + alias + "*.sqlaudit");
                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            try
                            {
                                File.Delete(files[i]);
                            }
                            catch { }
                        }
                    }

                    files = Directory.GetFiles(traceDirectory, alias + "*.bin");

                    if (files != null)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            try
                            {
                                File.Delete(files[i]);
                            }
                            catch { }
                        }

                    }
                }

                // delete directory if its empty now that we have moved all our fiels
                try
                {
                    Directory.Delete(traceDirectory);
                }
                catch { }
            }
            catch (Exception e)
            {
                string msg = String.Format("An error occurred deleting trace directory {0}. Error: {1}.", traceDirectory, e.Message);
                ErrorLog.Instance.Write(msg, e, true);
            }
        }

        //-----------------------------------------------------------------
        // SetMaxTraceSize - Sets new maximum trace size
        //-----------------------------------------------------------------
        public void SetMaxTraceSize(int newMaxTraceSize)
        {
            try
            {
                if (newMaxTraceSize == configuration.MaxTraceSize)
                    return;

                configuration.MaxTraceSize = newMaxTraceSize;
                SetRegistryValue(CoreConstants.Agent_RegVal_MaxTraceSize, newMaxTraceSize);

                SQLInstance sqlInstance = null;
                IDictionaryEnumerator enumerator = sqlInstances.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    try
                    {
                        sqlInstance = (SQLInstance)enumerator.Value;
                        sqlInstance.MaxTraceSize = newMaxTraceSize;
                        sqlInstance.RestartTraces(true);
                    }
                    catch (Exception e)
                    {
                        ErrorLog.Instance.Write("An error occurred restarting the traces for " + sqlInstance.Name + ".", e, true);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred setting new maximum trace file size.", e, true);
            }
        }

        //-----------------------------------------------------------------
        // SetMaxFolderSize
        //-----------------------------------------------------------------
        public void SetMaxFolderSize(int newMaxFolderSize)
        {
            if (configuration.MaxFolderSize == newMaxFolderSize)
                return;

            try
            {
                configuration.MaxFolderSize = newMaxFolderSize;
                SetRegistryValue(CoreConstants.Agent_RegVal_MaxFolderSize,
                                  newMaxFolderSize);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred setting new maximum trace file size.", e, true);
            }
        }

        //-----------------------------------------------------------------
        // SetMaxUnattendedTime
        //-----------------------------------------------------------------
        public void SetMaxUnattendedTime(int newMaxUnattendedTime)
        {
            if (configuration.MaxUnattendedTime == newMaxUnattendedTime)
                return;

            try
            {
                configuration.MaxUnattendedTime = newMaxUnattendedTime;
                SetRegistryValue(CoreConstants.Agent_RegVal_MaxUnattendedTime,
                                  newMaxUnattendedTime);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred setting new maximum trace file size.", e, true);
            }
        }

        //---------------------------------------------------------------------------
        // StopTracesOnShutdown 
        //---------------------------------------------------------------------------
        private bool StopTracesOnShutdown()
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            int stopTraces = 0;

            try
            {
                rk = Registry.LocalMachine;
                rks = rk.OpenSubKey(agentRegistryKey, true);
                stopTraces = (int)rks.GetValue(CoreConstants.Agent_RegVal_StopTracesOnShutDown, 0);

                // if stopTraceOnShutdown is set, delete the registry key since we are uninstalling
                // and uninstall wont kill it since install didnt create it
                if (stopTraces == 1)
                {
                    rks.DeleteValue(CoreConstants.Agent_RegVal_StopTracesOnShutDown);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(String.Format(CoreConstants.Exception_CantReadAgentConfiguration, ex.Message), ex, true);
            }
            finally
            {
                if (rks != null) rks.Close();
                if (rk != null) rk.Close();
            }
            return stopTraces == 1;
        }

        //----------------------------------------------------------
        // Validate whether SQL Server and OS versions are supported
        //----------------------------------------------------------
        public static bool ValidSqlServerOSCombo(string instance)
        {
            SqlServerConfiguration sqlServerConfiguration = GetSqlVersionString(instance);
            int sqlVersion;
            return IsValidSqlServerOSCombo(sqlServerConfiguration, out sqlVersion);
        }

        //-------------------------------------------------------------
        // Validate whether SQL Server and OS versions are supported,
        // and output the sqlVersion.
        //-------------------------------------------------------------
        public static bool ValidSqlServerOSCombo(string instance, out int sqlVersion)
        {
            SqlServerConfiguration sqlServerConfiguration = GetSqlVersionString(instance);

            return IsValidSqlServerOSCombo(sqlServerConfiguration, out sqlVersion);
        }

        //------------------------------------------------------------------
        // Retrieve instance's version string
        //------------------------------------------------------------------
        internal static SqlServerConfiguration GetSqlVersionString(string instance)
        {
            string versionString;
            string queryString = "";
            string selectString = "";
            Dictionary<string, string> instancesWithPortDictionary = new Dictionary<string, string>();

            instance = GetInstanceWithPorts(instance);
            //Find if this Instance contains port or not



            // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
            // Handle Basic Permissions Check for View Server State Permissions
            // Changes Start : 5.3.1 To remove @@Version for version check
            SqlServerConfiguration sqlServerConfiguration = new SqlServerConfiguration();
            queryString = "DECLARE @productVersion nvarchar(128)\n" +
                         "DECLARE @sqlVersion int\n" +
                         "DECLARE @sqlVersionBuild nvarchar(10)\n" +
                         "DECLARE @HasViewServerStatePermissions int\n" +
                         "SET @productVersion = CAST(SERVERPROPERTY('productversion') AS NVARCHAR)\n" +
                         "SET @sqlVersionBuild = SUBSTRING(@productVersion,CHARINDEX('.',@productVersion)+1,LEN(@productVersion))\n" +
                         "SET @sqlVersionBuild = SUBSTRING(@sqlVersionBuild,CHARINDEX('.',@sqlVersionBuild)+1,LEN(@sqlVersionBuild))\n" +
                         "SET @sqlVersion = SUBSTRING(@productVersion,1,CHARINDEX('.',@productVersion)-1)\n" +
                         "-- Set for permissions\n" + // Check for View Server State permissions required for sys.dm_os_windows_info
                         "SELECT @HasViewServerStatePermissions = COUNT(*) FROM sys.fn_my_permissions(NULL, NULL) where permission_name = 'VIEW SERVER STATE'\n";

            //To get OS Version (Major part)
            selectString += "SELECT LEFT(d.windows_release, CHARINDEX ('.' ,d.windows_release)-1) AS osVersionMajor, ";

            //To get OS Version (Minor part)
            selectString += "RIGHT(d.windows_release,  LEN(d.windows_release) - CHARINDEX ('.' ,d.windows_release)) AS osVersionMinor, ";

            //To get SQL Server Product Version (Major Part)
            selectString += "@sqlVersion AS sqlVersion, ";

            //To get SQL Server Product Version (Build Version)
            selectString += "@sqlVersionBuild AS sqlVersionBuild ";

            selectString += "FROM {0} d\n";

            // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
            // Handle Basic Permissions Check for View Server State Permissions
            queryString += "IF(@sqlVersion < 11 OR @HasViewServerStatePermissions <> 1)\n";//Lower versions do not support sys.dm_os_windows_info
            queryString += String.Format(selectString, "(SELECT RIGHT(SUBSTRING(@@VERSION," +
                           "CHARINDEX('Windows NT', @@VERSION), 14), 3) as windows_release)");
            queryString += "ELSE\n";

            queryString += String.Format(selectString, "sys.dm_os_windows_info");

            using (SqlCommand myCommand = new SqlCommand())
            {
                string strConn = String.Format("server={0};" +
                    "integrated security=SSPI;" +
                    "Connect Timeout=30;" +
                    "Application Name='{1}';",
                    instance,
                    CoreConstants.ManagementConsoleName);

                using (myCommand.Connection = new SqlConnection(strConn))
                {
                    myCommand.Connection.Open();
                    myCommand.CommandText = queryString;
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                sqlServerConfiguration.OsVersionMajor = int.Parse((string)reader["osVersionMajor"]);
                                sqlServerConfiguration.OsVersionMinor = int.Parse((string)reader["osVersionMinor"]);
                                sqlServerConfiguration.SqlVersion = (int)reader["sqlVersion"];
                                sqlServerConfiguration.SqlVersionBuild = (string)reader["sqlVersionBuild"];
                            }
                        }
                    }
                    myCommand.CommandText = "SELECT @@VERSION";
                    myCommand.CommandType = CommandType.Text;
                    versionString = (string)myCommand.ExecuteScalar();
                    if (versionString.IndexOf(CoreConstants.VersionString_MSDE70) != -1
                            || versionString.IndexOf(CoreConstants.VersionString_MSDE2000) != -1)
                    {
                        sqlServerConfiguration.IsMSDE = true;
                    }
                    int iStart = versionString.IndexOf("Service Pack ");
                    if (iStart != -1)
                        sqlServerConfiguration.SpNumber = int.Parse(versionString.Substring(iStart + 13, 1));
                }
            }
            return sqlServerConfiguration;
        }


        //-------------------------------------------------------------------
        // Validate whether SQL Server and OS versions are supported and 
        // output sqlVersion.
        //-------------------------------------------------------------------
        private static bool IsValidSqlServerOSCombo(SqlServerConfiguration sqlServerConfiguration, out int sqlVersion)
        {
            // Get the SQL server full version numbers
            /*string sqlVersionMajor;	
            string sqlVersionBuild;					
            int iStart, iEnd;
            try 
            {
             sqlVersion = 0;
                iStart = versionString.IndexOf("- ") ;
                if(iStart == -1)
                    return false ;
                iStart += 2 ;
                iEnd = versionString.IndexOf(".", iStart) ;
                if(iEnd == -1)
                    return false ;
                sqlVersionMajor = versionString.Substring(iStart, iEnd - iStart) ;
             sqlVersion = int.Parse( sqlVersionMajor );

                iStart = iEnd + 1 ;
                iEnd = versionString.IndexOf(".", iStart) ;
                if(iEnd == -1)
                    return false ;
                iStart = iEnd + 1 ;
                iEnd = versionString.IndexOf(" ", iStart) ;
                if(iEnd == -1)
                    return false ;
                sqlVersionBuild = versionString.Substring(iStart, iEnd - iStart) ;

                // We don't support MSDE
                if (versionString.IndexOf(CoreConstants.VersionString_MSDE70) != -1
                    || versionString.IndexOf(CoreConstants.VersionString_MSDE2000) != -1) 
                {
                    return false;
                }

                // We don't support < SQL Server 2000
                if (sqlVersion < 8) 
                {
                    return false;
                }

                // Get the OS version from the SQL version string
                int osVersionMajor;
                int osVersionMinor;
                string osVersion;
                int spNumber = 0 ;
                if (versionString.Contains(" NT "))
                {
                    osVersion = versionString.Substring(versionString.IndexOf(CoreConstants.OSVersionNT));
                    osVersion = osVersion.Substring(CoreConstants.OSVersionNT.Length);
                    osVersion = osVersion.Substring(0, osVersion.IndexOf(" "));

                }
                else
                {
                    if (versionString.Contains(" 8.1 Pro "))
                    {
                    osVersion = versionString.Substring(versionString.IndexOf(CoreConstants.OSVersionNonNT));
                    osVersion = osVersion.Substring(CoreConstants.OSVersionNonNT.Length);
                    osVersion = osVersion.Substring(0, osVersion.IndexOf(" "));
                    }
                    else
                    {
                        string[] arr = versionString.Split(new string[] { " on Windows " }, StringSplitOptions.None);
                        string[] version = arr[1].Split(new string[] { " <X" }, StringSplitOptions.None);
                        osVersion = version[0].Substring((version[0].Length - 3), (version[0].Length - (version[0].Length - 3)));
                    }
                }

                // Parse the major/minor OS version (X.Y)
                osVersionMajor = int.Parse(osVersion.Substring(0, 1));
                osVersionMinor = int.Parse(osVersion.Substring(2, 1));

                iStart = versionString.IndexOf("Service Pack ") ;
                if(iStart != -1)
                    spNumber = int.Parse(versionString.Substring(iStart + 13, 1)) ;


                // We don't support Windows 2003 Server and < SQL 2000 service pack 3a
                if (osVersionMajor >= 5 && osVersionMinor > 1  // Win2003 Server
                    && sqlVersion == 8 && int.Parse(sqlVersionBuild) < CoreConstants.VersionBuild_SQL2000SP3) 
                {
                    return false;
                }
                // We don't support Windows 2000 below service pack 4
                if(osVersionMajor == 5 && osVersionMinor == 0 && spNumber < 4)
                {
                    return false ;
                }*/
            try
            {
                sqlVersion = sqlServerConfiguration.SqlVersion;
                // We don't support MSDE
                if (sqlServerConfiguration.IsMSDE)
                {
                    return false;
                }

                // We don't support < SQL Server 2000
                if (sqlVersion < 8)
                {
                    return false;
                }

                // We don't support Windows 2003 Server and < SQL 2000 service pack 3a
                if (sqlServerConfiguration.OsVersionMajor >= 5 && sqlServerConfiguration.OsVersionMinor > 1  // Win2003 Server
                   && sqlVersion == 8 && int.Parse(sqlServerConfiguration.SqlVersionBuild) < CoreConstants.VersionBuild_SQL2000SP3)
                {
                    return false;
                }

                // We don't support Windows 2000 below service pack 4
                if (sqlServerConfiguration.OsVersionMajor == 5 && sqlServerConfiguration.OsVersionMinor == 0 && sqlServerConfiguration.SpNumber < 4)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new CoreException(CoreConstants.Exception_CannotParseSQLServerVersion, e);
            }
            // If we got here, we passed the tests
            return true;

            // Changes Ends : 5.3.1 To remove @@Version for version check
        }


        #endregion

        #region Audit Start/Stop/Restart Methods
        //---------------------------------------------------------------------------
        // StartAuditing : 
        //---------------------------------------------------------------------------
        private void StartAuditing()
        {
            try
            {
                IDictionaryEnumerator enumerator = sqlInstances.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    try
                    {
                        ((SQLInstance)enumerator.Value).StartAuditing();
                    }
                    catch { } // StartAuditing suppose to handle all the errors.  Keep the process going.
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Cannot enumerate audited instances for StartAuditing().", e, true);
            }
        }

        //---------------------------------------------------------------------------
        // StopCollectingTraces : 
        //---------------------------------------------------------------------------
        private void StopCollectingTraces()
        {
            try
            {
                IDictionaryEnumerator enumerator = sqlInstances.GetEnumerator();
                while (enumerator.MoveNext())
                    ((SQLInstance)enumerator.Value).StopCollectingTraces();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Cannot enumerate audited instances for StopCollectingTraces().", e, true);
            }
        }

        //---------------------------------------------------------------------------
        // DisableAuditing 
        //---------------------------------------------------------------------------
        public void DisableAuditing()
        {
            try
            {
                IDictionaryEnumerator enumerator = sqlInstances.GetEnumerator();
                while (enumerator.MoveNext())
                    ((SQLInstance)enumerator.Value).DisableAuditing(false);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Cannot enumerate audited instances for DisableAuditing().", e, true);
            }
        }

        //---------------------------------------------------------------------------
        // CollectTraceNow - Force trace collection now. 
        //---------------------------------------------------------------------------
        public void CollectTracesNow(string instance)
        {
            SQLInstance sqlInstance = GetSQLInstance(instance);
            if (sqlInstance == null) return;
            if (!sqlInstance.IsEnabled)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                          string.Format("CollectTracesNow: : auditing is disabled so collection will not be processed for {0}.", instance),
                          ErrorLog.Severity.Warning);
                return;
            }

            sqlInstance.CollectTracesNow();
        }

        //---------------------------------------------------------------------------
        // DisableAuditing - Disable event tracing and stop collecting traces. 
        //---------------------------------------------------------------------------
        public void DisableAuditing(string instanceName)
        {
            try
            {
                SQLInstance sqlInstance = GetSQLInstance(instanceName);
                if (sqlInstance == null)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             "DiableAuditing: Unable to find configuration for " + instanceName,
                                             ErrorLog.Severity.Warning);
                    return;
                }
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format("Disabling auditing for {0}.",
                                                        instanceName));
                sqlInstance.DisableAuditing(false);

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default, "Error disabling auditing for " + instanceName, e, true);
            }
        }

        //---------------------------------------------------------------------------
        // EnableAuditing - Starts event tracing and stop collecting traces. 
        //---------------------------------------------------------------------------
        public void EnableAuditing(string instanceName)
        {
            try
            {
                SQLInstance sqlInstance = GetSQLInstance(instanceName);
                if (sqlInstance == null)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             "EnableAuditing: Unable to find configuration for " + instanceName,
                                             ErrorLog.Severity.Warning);
                    return;
                }
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format("Enabling auditing for {0}.",
                                         instanceName));
                sqlInstance.EnableAuditing();

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                         "Error enabling auditing for " + instanceName,
                                         e,
                                         true);
            }
        }
        #endregion

        #region Methods for Sending Agent Status Messages
        //---------------------------------------------------------------------------
        /// SendStartupMessage - send message indicating agent has started successfully
        //---------------------------------------------------------------------------
        private void SendStartupMessage()
        {
            AgentStatusMsg msg = new AgentStatusMsg(configuration, AgentStatusMsg.MsgType.Startup);

            // Check if instances have been validated during start up.  This call is needed when system is
            // rebooted and SQL Server takes a long time to start.
            InstanceValidationCheck();

            bool[,] updateNeeded = StatusLogger.Instance.SendStatusEx(msg);

            if (updateNeeded != null)
            {
                for (int i = 0; i < updateNeeded.Length / 2; i++)
                {
                    try
                    {
                        SQLInstance sqlInstance;
                        sqlInstance = GetSQLInstance(msg.Config.InstanceStatusList[i].Instance);

                        if (updateNeeded[i, 0] || sqlInstance.ConfigurationFileErrorFound)
                        {
                            sqlInstance.UpdateAuditConfiguration();
                        }

                        if (!updateNeeded[i, 1]) // Repository's SQL Server version is lower than the instance's
                        {
                            sqlInstance.TraceCollectionStopped = true;
                            ErrorLog.Instance.Write(String.Format(CoreConstants.Exception_RepositoryDoesNotSupportSQL2005, sqlInstance.Name),
                                                     ErrorLog.Severity.Error);
                        }

                    }
                    catch (Exception e)
                    {
                        ErrorLog.Instance.Write("An error occurred updating audit configuration for "
                                                 + msg.Config.InstanceStatusList[i].Instance + ".",
                                                 e,
                                                 true);
                    }

                }
            }
        }

        //---------------------------------------------------------------------------
        /// SendShutdownMessage - send message indicating agent si shutting down
        //---------------------------------------------------------------------------
        private void SendShutdownMessage()
        {
            AgentStatusMsg msg = new AgentStatusMsg(configuration, AgentStatusMsg.MsgType.Shutdown);
            StatusLogger.Instance.SendStatus(msg);
        }

        //---------------------------------------------------------------------------
        /// SendAuditUpdatedMessage
        //---------------------------------------------------------------------------
        public void SendAuditUpdatedMessage(string instance)
        {
            InstanceStatus status = GetInstanceStatus(instance);

            if (status != null)
            {
                AgentStatusMsg msg = new AgentStatusMsg(status,
                                                       AgentStatusMsg.MsgType.Update,
                                                       Server,
                                                       ServerPort);
                StatusLogger.Instance.SendStatus(msg);
            }
        }

        //---------------------------------------------------------------------------
        /// SendAuditUpdatedMessage
        //---------------------------------------------------------------------------
        public bool SendTraceCollectedMessage(string instance)
        {
            InstanceStatus status = configuration.GetInstanceStatus(instance);
            if (status != null)
            {
                AgentStatusMsg msg = new AgentStatusMsg(status,
                                                       AgentStatusMsg.MsgType.TraceReceived,
                                                       Server,
                                                       ServerPort);
                StatusLogger.Instance.SendStatus(msg);
            }
            return true;
        }

        //---------------------------------------------------------------------------
        /// SendTraceTamperedMessage
        //---------------------------------------------------------------------------
        public bool SendTraceTamperedMessage(string instance, AgentStatusMsg.MsgType type)
        {
            InstanceStatus status = configuration.GetInstanceStatus(instance);
            if (status != null)
            {
                AgentStatusMsg msg = new AgentStatusMsg(status, type, Server, ServerPort);
                StatusLogger.Instance.SendStatus(msg);
            }
            return true;
        }

        //---------------------------------------------------------------------------
        /// SendErrorMessage : Not being used anywhere.  Need custom serialization
        ///                    when it is used.
        //---------------------------------------------------------------------------
        /*
        private static void
           SendErrorMessage (
              string       agentServer,
              string       instance, 
              int          configVersion, 
              DateTime     lastUpdateTime,
              string       server,
              int          port,
              int          errorNumber,
              string       errorMessage )
        {
           InstanceStatus status = new InstanceStatus( instance );
           status.ConfigVersion = configVersion;
           status.LastUpdateTime = lastUpdateTime;
           status.AgentServer = agentServer;
           ErrorLog.Instance.Write( ErrorLog.Level.Always,
                                    typeof(AgentErrorMsg).ToString()+ " need custom serializatioin.",
                                    ErrorLog.Severity.Warning );

           AgentStatusMsg msg = new AgentErrorMsg( status,
                                                     errorNumber,
                                                     server,
                                                     port );
           StatusLogger.Instance.SendStatus( msg );
        }
        */

        #endregion

        #region Heartbeat

        //---------------------------------------------------------------------------
        /// StartHeartbeatThread - Kick off heartbeat thread
        //---------------------------------------------------------------------------
        private void StartHeartbeating()
        {
            StartHeartbeating(configuration.HeartbeatInterval);
        }

        //---------------------------------------------------------------------------
        /// StartHeartbeatThread - Kick off heartbeat thread
        //---------------------------------------------------------------------------
        /// <summary>
        /// StartHeartbeating
        /// </summary>
        private void StartHeartbeating(int interval)
        {
            try
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Starting heartbeat timer.");
                if (heartbeat != null)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Stop existing heartbeat timer.");
                    heartbeat.Stop();
                }
                heartbeat = new ServiceTimer(new TimerCallback(SendHeartbeat), interval * 60 * 1000);
                heartbeat.Start();
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "heartbeat timer started.");
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred starting heartbeat timer.", e, true);
            }
        }

        //---------------------------------------------------------------------------
        /// StopHeartbeating
        //---------------------------------------------------------------------------
        private void StopHeartbeating()
        {
            if (heartbeat == null)
                return;

            heartbeat.Stop();
        }

        public void SendHeartbeat()
        {
            SendHeartbeat(null);
        }

        //---------------------------------------------------------------------------
        /// SendHeartbeat
        //---------------------------------------------------------------------------
        /// <summary>
        /// SendHeartbeat: Delegate that handles elapsed event for heartbeatTimer
        /// </summary>
        private void SendHeartbeat(object obj)
        {
            bool[] updateNeeded;
            AgentStatusMsg msg;
            try
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Sending heartbeat to the server.", ErrorLog.Severity.Informational);
                InstanceValidationCheck();

                // Check if MaxFolderSize limit is reached
                CheckTraceFolderSize();

                msg = new AgentStatusMsg(configuration, AgentStatusMsg.MsgType.Heartbeat);
                // Need to update audit configurations
                updateNeeded = StatusLogger.Instance.SendStatus(msg);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                         "An error occurred sending heartbeat to ther server.",
                                         e.Message,
                                         ErrorLog.Severity.Warning);
                return;

            }
            SQLInstance sqlInstance;

            try
            {
                for (int i = 0; i < updateNeeded.Length; i++)
                {
                    try
                    {
                        sqlInstance = GetSQLInstance(msg.Config.InstanceStatusList[i].Instance);

                        if (updateNeeded[i] || sqlInstance.ConfigurationFileErrorFound)
                        {
                            sqlInstance.UpdateAuditConfiguration();
                        }
                        else
                        {
                            sqlInstance.CheckDataChangeSetup();
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorLog.Instance.Write("An error occurred updating audit configuration for "
                                                 + msg.Config.InstanceStatusList[i].Instance + ".",
                                                 e,
                                                 true);
                    }
                }
                //Sync the Database IDs in the repository with the monitored Instance.
                SynchDBIds();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Error occurred when updating configurations during heartbeat.", e, ErrorLog.Severity.Error);
            }
            configuration.DbSchemaVersionChecked = true;
            ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Heartbeat sent to the server.", ErrorLog.Severity.Informational);
        }

        private void SynchDBIds()
        {
            try
            {
                IDictionaryEnumerator enumerator = sqlInstances.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    try
                    {
                        ((SQLInstance)enumerator.Value).SyncDBIds();
                    }
                    catch { } // SyncDBIds suppose to handle all the errors.  Keep the process going.
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Cannot enumerate audited instances for SyncDBIds().", e, true);
            }
        }

        public bool UpdateDBIds(Hashtable auditedServerDbIds, string instance)
        {
            RemoteAuditManager auditManager;

            try
            {
                auditManager = ConfigurationHelper.GetRemoteAuditManager();
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("Unable to get a connection to the collection server Error: {0}", e.ToString()), ErrorLog.Severity.Error);
                return false;
            }
            return auditManager.UpdateDBIds(auditedServerDbIds, instance);
        }
        #endregion

        #region System Alerts

        #endregion

        #region Methods for Remote Objects and Channels

        //---------------------------------------------------------------------------
        /// <summary>
        /// StartServerListener() registers and starts a SQLsecure Server listener
        /// to accept messages and commands.
        /// </summary>
        /// <returns>bool</returns>
        //---------------------------------------------------------------------------
        private bool StartServerListener()
        {

            try
            {

                /* Commenting the Secure Implementation due to Agent Issues
                 // Register the secure TCP channel
                 BinaryServerFormatterSinkProvider formatterProvider = new BinaryServerFormatterSinkProvider();
                 formatterProvider.TypeFilterLevel = TypeFilterLevel.Full;

                 // Register the unsecure TCP channel
                 IDictionary props = new Hashtable();
                 props["machineName"] = Environment.MachineName;
                 props["name"] = typeof(SQLcomplianceAgent).Name;
                 props["port"] = configuration.AgentPort;
                 props["secure"] = true;
                 props["protectionLevel"] = ProtectionLevel.EncryptAndSign;
                 props["impersonate"] = false; // This must be false, because we'll impersonate manually 
                 * */

                if (isClustered)
                {
                    // We must always bind to IP because we can have a local instance agent running on the 
                    // same machine containing agents to support virtual instances and to support Failover 
                    // in a active/active cluster
                    IPHostEntry hostInfo = Dns.GetHostEntry(configuration.AgentServer);

                    IDictionary props = new Hashtable();
                    props["name"] = typeof(SQLcomplianceAgent).Name;
                    props["port"] = configuration.AgentPort;

                    // We are using the IP Address for the TCP Channel so that in the cluster environment the agent
                    // is listening on the virtual server address instead of its own address.
                    // Note that this works only if this agent is on the active node.
                    bool bound = false;
                    foreach (IPAddress address in hostInfo.AddressList)
                    {
                        if (_ipAddressBindType == 0 && (address.AddressFamily == AddressFamily.InterNetwork ||
                           address.AddressFamily == AddressFamily.InterNetworkV6))
                        {
                            props["bindTo"] = address.ToString();
                            bound = true;
                        }
                        else if (_ipAddressBindType == 4 && address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            props["bindTo"] = address.ToString();
                            bound = true;
                        }
                        else if (_ipAddressBindType == 6 && address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            props["bindTo"] = address.ToString();
                            bound = true;
                        }
                        if (bound)
                            break;
                    }
                    if (!bound)
                    {
                        throw new Exception(String.Format("Unable to find an IP address to bind to for {0}", configuration.AgentServer));
                    }
                    tcpChannel = ChannelBuilder.GetRegisteredServerChannel(props);
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, String.Format("Opening TcpServerChannel for {0} on port {1}.",
                                                                                hostInfo.AddressList[0].ToString(),
                                                                                configuration.AgentPort));
                }
                else
                {
                    tcpChannel = ChannelBuilder.GetRegisteredServerChannel(typeof(SQLcomplianceAgent).Name, configuration.AgentPort);
                }

                /* Commenting the Secure Implementation due to Agent Issues
                tcpChannel = new TcpServerChannel(props, formatterProvider);
                ChannelServices.RegisterChannel(tcpChannel, true);
                 * */

                // Register the remotable object
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(AgentCommand),
                                                                    typeof(AgentCommand).Name,
                                                                    WellKnownObjectMode.SingleCall);

                string[] urls = tcpChannel.GetUrlsForUri(typeof(AgentCommand).Name);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format("{0} is registered as a remotable object.  URL: {1}",
                                                        typeof(AgentCommand).Name, urls[0]));

                // management console requests
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(ManagementConsoleRequest),
                                                                   typeof(ManagementConsoleRequest).Name,
                                                                   WellKnownObjectMode.SingleCall);
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                       String.Format("{0} is registered as a remotable object.  URL: {1}",
                                       typeof(ManagementConsoleRequest),
                                       tcpChannel.GetUrlsForUri(typeof(ManagementConsoleRequest).Name)[0]));
            }
            catch (Exception e)
            {
                // TODO: need to retry or report the problem depends on what kind of exception it is
                // For now, we die
                ErrorLog.Instance.Write(e, true);
                throw e;
            }

            return true;
        }

        #endregion

        #region Activate/Deactivate instances
        //-----------------------------------------------------------
        // ActivateInstance -- Activate a new SQL Server instance.
        //-----------------------------------------------------------
        public void ActivateInstance(string instanceName)
        {
            try
            {
                ErrorLog.Instance.Write("Activating SQL Server instance " + instanceName + ".");

                // Check to see if the instance is already activated
                //  If so, force a heartbeat.
                if (sqlInstances.ContainsKey(instanceName))
                {
                    SendHeartbeat(null);
                    return;
                }
                SQLInstance sqlInstance = new SQLInstance(instanceName);
                sqlInstances.Add(sqlInstance.Name, sqlInstance);

                // Create new instance status
                CreateInstanceStatus(sqlInstance);
                sqlInstance.UpdateAuditConfiguration();
                ErrorLog.Instance.Write("SQL Server instance " + instanceName + " activated.");
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred activating instance {0}.", instanceName), e, true);
                throw e;
            }
        }

        //-----------------------------------------------------------
        // DeactivateInstance -- Deactivates a SQL Server instance.
        //-----------------------------------------------------------
        public void DeactivateInstance(string instanceName, bool stopCollectingTraces)
        {
            try
            {
                ErrorLog.Instance.Write("Deactivating SQL Server instance " + instanceName + ".");

                SQLInstance sqlInstance = GetSQLInstance(instanceName);
                sqlInstance.DisableAuditing(stopCollectingTraces);
                sqlInstances.Remove(instanceName);
                configuration.RemoveInstanceStatus(instanceName);
                SetLastSentDefaultTraceNumber(instanceName, -1);

                ErrorLog.Instance.Write("SQL Server instance " + instanceName + " deactivated.");
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred deactivating instance {0}.", instanceName), e, true);
            }
        }
        #endregion

        #region Methods for Handling SQL Server Instances

        //-----------------------------------------------------------
        // GetSQLInstance
        //-----------------------------------------------------------
        /// <summary>
        /// Returns a ServerRecord for the specified instance.
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        internal SQLInstance GetSQLInstance(string instanceName)
        {
            SQLInstance instance = (SQLInstance)sqlInstances[instanceName.ToUpper()];

            if (instance == null)
            {
                throw new Exception(String.Format(CoreConstants.Exception_InstanceNotRegisteredAtAgent, instanceName));
            }

            return instance;
        }

        //-----------------------------------------------------------
        // GetInstanceStatus
        //-----------------------------------------------------------
        internal InstanceStatus GetInstanceStatus(string instanceName)
        {
            return configuration.GetInstanceStatus(instanceName);
        }

        //-----------------------------------------------------------
        // UpdateAuditConfiguration
        //-----------------------------------------------------------
        public void UpdateAuditConfiguration(string instanceName)
        {
            GetSQLInstance(instanceName).UpdateAuditConfiguration();
        }

        //-----------------------------------------------------------
        // UpdateAuditConfiguration: Updates all instances registered in the Agent service
        //-----------------------------------------------------------
        public void UpdateAuditConfiguration()
        {
            try
            { 
                foreach (string instanceName in instances)
                {
                    UpdateAuditConfiguration(instanceName);
                }
            } 
            catch(Exception ex)
            {
                ErrorLog.Instance.Write("An error occurred while updating audit configurations", ex, true);
            }
        }

        public void UpdateAuditUsers(string instanceName)
        {
            GetSQLInstance(instanceName).UpdateAuditUsers();
        }


        //-----------------------------------------------------------
        // InstanceValidationCheck
        //-----------------------------------------------------------
        private void InstanceValidationCheck()
        {
            IDictionaryEnumerator enumerator = sqlInstances.GetEnumerator();
            SQLInstance instance;

            while (enumerator.MoveNext())
            {
                instance = (SQLInstance)enumerator.Value;
                // If somehow the validation failed during start up, retry here and take appropriate actions after
                // validatation.
                if (!instance.validated && instance.IsEnabled)
                {
                    try
                    {
                        instance.validated = true;
                        if (ValidSqlServerOSCombo(instance.Name))
                        {
                            instance.StartAuditing();
                        }
                    }
                    catch
                    {
                        instance.validated = false;
                    }
                }
            }
        }

        #endregion

        #region EventLog Related Methods

        /* Not used
        /// <summary>
        /// NewLogEntryHandler: New EventLog entry written event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewLogEntryHandler(object sender, EntryWrittenEventArgs e)
        {
           if( e.Entry.EventID == 7036 &&
               e.Entry.Source.Equals( CoreConstants.ServiceControlManager ))
           {

              if( e.Entry.Message.StartsWith( "The MSSQLSERVER service entered the stopped state." ) )
              {
                 // TODO: SQL Server stopped.  Report it.  Collect traces.
                 ErrorLog.Instance.Write( "SQL Server stopped" );
              }
              else if( e.Entry.Message.StartsWith( "The MSSQLSERVER service entered the running state." ) )
              {
                 // TODO: SQL Server started.  Check tracing status.
                 ErrorLog.Instance.Write( "SQL Server started");
              }
           }

        }

        public void 
           MonitorSQLServer (
           bool enable
           )
        {
           if( enable )
           {
              if( eventLog == null )
                 eventLog = new EventLog("System");

              monitorEventLog = true;
              eventLog.EnableRaisingEvents = true;
              eventLog.SynchronizingObject = null; // Use system thread pool
              eventLog.EntryWritten += new EntryWrittenEventHandler( NewLogEntryHandler );
           }
           else if( eventLog != null )
           {
              monitorEventLog = false;
              eventLog.EnableRaisingEvents = false;
              // TODO: remove the previously added event handler
           }
        }

        */
        #endregion

        #region Connection
        //---------------------------------------------------------------
        // CreateConnectionString
        //---------------------------------------------------------------
        /*
        public static string
           CreateConnectionString(
              string instanceName )
        {
           if( instanceName == null )
              return null;
           return "server=" + instanceName
                    + ";integrated security=SSPI"
                    + ";Connect Timeout=30"  
                    + ";Application Name='" 
                    + CoreConstants.DefaultSqlApplicationName 
                    + "'";
        }
         */

        //---------------------------------------------------------------
        // CreateConnectionString
        //---------------------------------------------------------------
        public static string CreateConnectionString(string instanceName, string database)
        {
            if (instanceName == null)
                return null;

            instanceName = GetInstanceWithPorts(instanceName);
            string connStr =
               String.Format(
                  "server={0};integrated security=SSPI{1}{2};Connect Timeout=30;Application Name='{3}'",
                  instanceName,
                  database == null || database.Length == 0 ? "" : ";database=",
                  database == null || database.Length == 0 ? "" : database,
                  CoreConstants.DefaultSqlApplicationName);
            return connStr;
        }

        //---------------------------------------------------------------
        // GetConnection
        //---------------------------------------------------------------
        public static SqlConnection GetConnection(string instanceName)
        {
            SQLInstance instance;
            try
            {
                instance = Instance.GetSQLInstance(instanceName);
            }
            catch
            {
                // exception is thrown if the instance is not registered or haven't been added to the list
                SqlConnection conn = new SqlConnection(CreateConnectionString(instanceName, null));
                conn.Open();
                return conn;
            }
            return instance.GetConnection();

        }

        //---------------------------------------------------------------
        // CreateInstanceNameWithPorts
        //---------------------------------------------------------------
        public static string GetInstanceWithPorts(string instanceName)
        {
            Dictionary<string, string> instancesWithPortDictionary = new Dictionary<string, string>();

            //Find if this Instance contains port or not
            //If contains, then append the Port number to Instance Name
            if (instancesWithPort != null && instancesWithPort.Length > 0)
            {
                foreach (string instancePortCombo in instancesWithPort)
                {
                    instancesWithPortDictionary[instancePortCombo.Split(',')[0].ToString()] = instancePortCombo.Split(',')[1].ToString();
                }

                if (instancesWithPortDictionary.ContainsKey(instanceName))
                {
                    string port = instancesWithPortDictionary[instanceName];
                    instanceName = string.Join(",", instanceName, port);
                }
            }

            return instanceName;
        }

        #endregion

        #region Registry

        //---------------------------------------------------------------
        // GetInstances
        //---------------------------------------------------------------
        public static string[] GetInstances()
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            string[] instances = null;
            try
            {
                rk = Registry.LocalMachine;
                rks = rk.OpenSubKey(agentRegistryKey);
                instances = (string[])rks.GetValue("Instances");

            }
            catch { }
            finally
            {
                if (rks != null)
                    rks.Close();
                if (rk != null)
                    rk.Close();
            }
            return instances;

        }

        public static int GetLastSentDefaultTraceNumber(string instance)
        {
            var lastSentTraces = GetLastSentDefaultTraces();
            if (lastSentTraces.ContainsKey(instance))
                return lastSentTraces[instance];

            return -1;
        }

        private static Dictionary<string, int> GetLastSentDefaultTraces()
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            var lastSentDefaultTrace = new Dictionary<string, int>();
            try
            {
                rk = Registry.LocalMachine;
                rks = rk.OpenSubKey(agentRegistryKey);
                var instances = (string[])rks.GetValue("DefaultTrace");
                foreach (var instance in instances)
                {
                    var instanceEntry = instance.Split(new[] { ';' }, StringSplitOptions.None);
                    lastSentDefaultTrace.Add(instanceEntry[0], Convert.ToInt32(instanceEntry[1]));
                }
            }
            catch { }
            finally
            {
                if (rks != null)
                    rks.Close();
                if (rk != null)
                    rk.Close();
            }

            return lastSentDefaultTrace;
        }

        public static void SetLastSentDefaultTraceNumber(string instanceName, int traceNumber)
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            try
            {
                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(agentRegistryKey);

                var oldValues = GetLastSentDefaultTraces();
                if (oldValues.ContainsKey(instanceName))
                    oldValues[instanceName] = traceNumber;
                else
                    oldValues.Add(instanceName, traceNumber);

                var valueToWrite = new string[oldValues.Count];
                var counter = 0;
                foreach (var key in oldValues.Keys)
                    valueToWrite[counter++] = string.Format("{0};{1}", key, oldValues[key]);

                rks.SetValue("DefaultTrace", valueToWrite);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("An error occurred setting registry value LastSentDefaultTrace.", e, true);
            }
            finally
            {
                if (rks != null)
                    rks.Close();

                if (rk != null)
                    rk.Close();
            }
        }

        //---------------------------------------------------------------
        // GetTraceDirectory
        //---------------------------------------------------------------
        public static string GetTraceDirectory()
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            string traceDirectory = null;
            try
            {
                rk = Registry.LocalMachine;
                rks = rk.OpenSubKey(agentRegistryKey);
                traceDirectory = (string)rks.GetValue(CoreConstants.Agent_RegVal_TraceDirectory,
                                                       CoreConstants.Agent_Default_TraceDirectory);
                traceDirectory = FixTraceDirectory(traceDirectory);
            }
            catch { }
            finally
            {
                if (rks != null)
                    rks.Close();
                if (rk != null)
                    rk.Close();
            }
            return traceDirectory;
        }

        //---------------------------------------------------------------
        // GetSkipAdminCheck
        //---------------------------------------------------------------
        public static void GetSkipAdminCheck()
        {
            RegistryKey rk = null;
            RegistryKey rks = null;

            try
            {
                rk = Registry.LocalMachine;
                rks = rk.OpenSubKey(agentRegistryKey);

                int val = (int)rks.GetValue(CoreConstants.Agent_RegVal_SkipAdminCheck, 0);

                skipAdminCheck = (val == 1) ? true : false;
            }
            catch
            {
                ErrorLog.Instance.Write("Error accessing SQLcompliance Agent registry - Check that the service account has appropriate permissions for reading the registry.",
                                         ErrorLog.Severity.Error);
            }
            finally
            {
                if (rks != null) rks.Close();
                if (rk != null) rk.Close();
            }
        }

        //-----------------------------------------------------------
        // UpdateisEnabledValue
        //-----------------------------------------------------------
        public void UpdateisEnabledValue(string instanceName, bool isEnabled)
        {
            RegistryKey regKey = null;
            RegistryKey regSubkey = null;

            try
            {
                SQLInstance sqlInstance = GetSQLInstance(instanceName);
                sqlInstance.IsEnabled = isEnabled;
                // Update registry
                regKey = Registry.LocalMachine;
                regSubkey = regKey.OpenSubKey(agentRegistryKey + @"\" + sqlInstance.Alias, true);
                regSubkey.SetValue(CoreConstants.Agent_RegVal_AuditingEnabled, isEnabled ? "True" : "False");

            }
            catch (SqlComplianceSecurityException se)
            {
                string msg = String.Format("{0} does not have the permission to access the registry: {1}",
                   se.Source, se.Message);
                ErrorLog.Instance.Write(msg, ErrorLog.Severity.Error);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Error setting registry", e, true);
            }
            finally
            {

                if (regSubkey != null)
                    regSubkey.Close();

                if (regKey != null)
                    regKey.Close();
            }
        }

        internal void SetClientActivatedFileTransfer(bool value)
        {
            SetRegistryValue(CoreConstants.Agent_RegVal_UseClientActivatedFileTransfer, value ? 1 : 0);
        }

        private void SetRegistryValue(string valueName, object newValue)
        {
            RegistryKey rk = null;
            RegistryKey rks = null;
            try
            {
                rk = Registry.LocalMachine;
                rks = rk.CreateSubKey(agentRegistryKey);

                rks.SetValue(valueName, newValue);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("An error occurred setting registry value {0}.", valueName), e, true);
            }
            finally
            {
                if (rks != null)
                    rks.Close();

                if (rk != null)
                    rk.Close();
            }
        }
        #endregion

        #region Utilities

        //---------------------------------------------------------------------------
        // CreateInstanceStatus - creates a new InstanceStatus and update its values. 
        //---------------------------------------------------------------------------
        private void CreateInstanceStatus(SQLInstance sqlInstance)
        {
            try
            {
                // Used for heartbeating messages
                InstanceStatus instanceStatus = new InstanceStatus();
                if (isClustered)
                    instanceStatus.SqlVersion = sqlInstance.SqlVersion + 1000;
                else
                    instanceStatus.SqlVersion = sqlInstance.SqlVersion;
                instanceStatus.Instance = sqlInstance.Name;
                instanceStatus.AgentServer = configuration.AgentServer;
                configuration.AddInstanceStatus(instanceStatus);
                sqlInstance.UpdateStatus(instanceStatus);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Failed to create an InstanceStuts for " + sqlInstance.Name + ".",
                                         e,
                                         true);
            }
        }

        //---------------------------------------------------------------------------
        // CheckMaxFolderSize - Check if trace directory reached its max size. 
        //---------------------------------------------------------------------------
        private void CheckTraceFolderSize()
        {
            long folderSize = 0;

            folderSize = GetDirectorySize(configuration.TraceDirectory);
            GenerateAgentTraceDirectoryAlert(folderSize);

            if (IsMaxFolderSizeReached(folderSize))
            {
                if (!maxFolderSizeReached)
                {
                    maxFolderSizeReached = true;
                    ReportAgentTraceDirectoryError(DirectorySizeLimitReached);

                    // Stop tracing
                    try
                    {
                        IDictionaryEnumerator enumerator = sqlInstances.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            try
                            {
                                ((SQLInstance)enumerator.Value).StopTracing();
                                ErrorLog.Instance.Write("MaxFolderSize limit reached.  Traces for " + ((SQLInstance)enumerator.Value).Name + " are stopped.",
                                                          ErrorLog.Severity.Warning);
                            }
                            catch { }
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorLog.Instance.Write("Cannot enumerate audited instances to stop tracing.", e, true);
                    }
                }
            }
            else if (maxFolderSizeReached) // reset the maxFolderSizeReached variable and restart the traces
            {
                maxFolderSizeReached = false;
                ReportAgentTraceDirectoryErrorResolution(DirectorySizeLimitReached);

                // restart tracing
                try
                {
                    IDictionaryEnumerator enumerator = sqlInstances.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        try
                        {
                            ((SQLInstance)enumerator.Value).RestartTraces();
                        }
                        catch { }
                    }
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write("Cannot enumerate audited instances to restart tracing.", e, true);
                }
            }
        }

        //---------------------------------------------------------------------------
        // IsMaxFolderSizeReached - Check if trace directory reached its max size. 
        //---------------------------------------------------------------------------
        private bool IsMaxFolderSizeReached(long folderSize)
        {
            bool reached = false;
            try
            {
                if (configuration.MaxFolderSize <= 0)
                    return false;

                reached = ((long)configuration.MaxFolderSize * 1073741824) < folderSize;

            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "An error occurred calculating trace directory size.", e, true);
            }
            return reached;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private long GetDirectorySize(string dir)
        {
            long size = 0;
            string fileName = "";

            DirectoryInfo directory = new DirectoryInfo(dir);
            FileInfo[] files = directory.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    fileName = files[i].FullName;
                    if ((files[i].Attributes & FileAttributes.Directory) > 0)
                        size += GetDirectorySize(fileName);
                    else
                        size += files[i].Length;
                }
                catch (Exception e)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, "An error occurred getting file size for " + fileName, e, true);
                }
            }
            return size;
        }

        private void GenerateAgentTraceDirectoryAlert(long folderSize)
        {
            RemoteStatusLogger remoteLogger;

            // Get the singleton remote status logger
            remoteLogger = StatusLogger.GetRemoteLogger(Server, ServerPort);

            try
            {
                remoteLogger.GenerateAgentTraceDirectoryAlert(folderSize, (long)configuration.MaxFolderSize * 1073741824, configuration.AgentServer);
            }
            catch { }
        }

        //-------------------------------------------------------------------------
        // FixTraceDirectory
        //-------------------------------------------------------------------------
        static public string FixTraceDirectory(string traceDirectory)
        {
            // truncate trailing backslash - setup sometimes puts one on and it causes problems!
            if (traceDirectory != null && traceDirectory != "")
            {
                int len = traceDirectory.Length;
                if (traceDirectory[len - 1] == '\\' ||
                   traceDirectory[len - 1] == '/')
                {
                    traceDirectory = traceDirectory.Substring(0, len - 1);
                }
            }
            return traceDirectory;
        }


        //-------------------------------------------------------------------------
        // ValidateTraceDirectory
        //-------------------------------------------------------------------------
        private void ValidateTraceDirectory(string traceDirectory)
        {
            try
            {
                // handles errors here
                CoreHelpers.ValidateTraceDirectory(traceDirectory);
            }
            catch (Exception e)
            {
                ReportAgentTraceDirectoryError(e.Message);
            }
        }


        /// <summary>
        /// Check to see if the supplied directory exists.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>True if the directory is valid and exists, otherwise false</returns>
        public bool DirectoryExists(string directory)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(directory);

                if (dir.Exists)
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        #endregion

        #region System Alerts

        //--------------------------------------------------------------------------
        internal void ReportAgentTraceDirectoryValidationError(string errorMessage)
        {
            string details =
               String.Format("An agent trace directory error occurred.  Error: {0}.",
                              errorMessage);
            ErrorLog.Level level = ErrorLog.Level.Default;

            if (!directoryErrors.ContainsKey(errorMessage))
            {
                directoryErrors.Add(errorMessage, 1);
                SystemAlert alert =
                   new SystemAlert(SystemAlertType.TraceDirectoryError,
                                    DateTime.UtcNow,
                                    AgentServer,
                                    details);
                SubmitSystemAlert(null, alert);
            }
            else  // send the alert only at the first time
            {
                level = ErrorLog.Level.Debug;
                directoryErrors[errorMessage] += 1;
            }
            ErrorLog.Instance.Write(level, details, ErrorLog.Severity.Warning);
        }

        //--------------------------------------------------------------------------
        internal void ReportAgentTraceDirectoryValidationResolution()
        {
            string details = "trace directory validation errors";
            directoryErrors.Clear();
            ReportAgentTraceDirectoryErrorResolution(details);
        }

        //--------------------------------------------------------------------------
        internal void ReportAgentTraceDirectoryError(string errorMessage)
        {
            string details = String.Format("An agent trace directory error occurred.  Error: {0}.", errorMessage);
            SystemAlert alert = new SystemAlert(SystemAlertType.TraceDirectoryError,
                                                DateTime.UtcNow,
                                                AgentServer,
                                                details);
            SubmitSystemAlert(null, alert);
            ErrorLog.Instance.Write(ErrorLog.Level.Default, details, ErrorLog.Severity.Warning);
        }

        //--------------------------------------------------------------------------
        internal void ReportAgentTraceDirectoryErrorResolution(string errorMessage)
        {
            string details = String.Format("Agent trace directory error \"{0}\" resolved.", errorMessage);

            if (directoryErrors.ContainsKey(errorMessage))
            {
                directoryErrors.Remove(errorMessage);
            }
            ErrorLog.Instance.Write(ErrorLog.Level.Default, details);
            SystemAlert alert = new SystemAlert(SystemAlertType.TraceDirectoryResolution,
                                                DateTime.UtcNow,
                                                AgentServer,
                                                details);
            SubmitSystemAlert(null, alert, directoryErrors.Count == 0 && !maxFolderSizeReached);
        }

        //-----------------------------------------------------------------------
        internal void ReportCollectionServerConnectionError(Exception e)
        {
            ErrorLog.Level level = ErrorLog.Level.Debug;

            if (serverAvailable)
            {
                SystemAlert alert = new SystemAlert(SystemAlertType.CollectionServiceConnectionError,
                                                    DateTime.UtcNow,
                                                    AgentServer,
                                                    e.Message.Replace("\\", "\\\\"));
                serverAvailable = false;
                level = ErrorLog.Level.Default;
                SubmitSystemAlert(null, alert);
            }
            ErrorLog.Instance.Write(level, String.Format(CoreConstants.Exception_CollectionServiceNotAvailable,
                                                         AgentServer,
                                                         e.Message.Replace("\\", "\\\\")),
                                     ErrorLog.Severity.Warning);
        }

        //-----------------------------------------------------------------------
        internal void ReportCollectionServerConnectionResolution()
        {
            if (serverAvailable)
                return;

            string msg = String.Format("Connections to collection server {0} reestablished.", Server);

            serverAvailable = true;
            SystemAlert alert = new SystemAlert(SystemAlertType.CollectionServiceConnectionResolution,
                                                DateTime.UtcNow,
                                                AgentServer,
                                                msg);
            SubmitSystemAlert(null, alert);
            ErrorLog.Instance.Write(ErrorLog.Level.Default, "Connection to collection server reestablished.");
        }

        //-----------------------------------------------------------------------
        // SubmitSystemAlert - submit system alert to the collection service
        //-----------------------------------------------------------------------
        public void SubmitSystemAlert(object syncObj, SystemAlert alert)
        {
            SubmitSystemAlert(syncObj, alert, true);
        }

        //-----------------------------------------------------------------------
        // SubmitSystemAlert - submit system alert to the collection service
        //-----------------------------------------------------------------------
        public void SubmitSystemAlert(object syncObj, SystemAlert alert, bool allErrorsResolved)
        {
            if (syncObj == null)
                syncObj = startStopLock;

            try
            {
                // Lock for the entire status logging operation
                lock (syncObj)
                {
                    RemoteStatusLogger remoteLogger;

                    // Get the singleton remote status logger
                    remoteLogger = StatusLogger.GetRemoteLogger(Server, ServerPort);

                    try
                    {
                        remoteLogger.SubmitSystemAlert(alert, allErrorsResolved);
                        systemAlertError = false;
                        ReportCollectionServerConnectionResolution();
                    }
                    catch (Exception e)
                    {
                        systemAlertError = true;
                        ReportCollectionServerConnectionError(e);
                    }
                }
            }
            catch (Exception e)
            {
                if (!systemAlertError)
                {
                    systemAlertError = true;
                    ErrorLog.Instance.Write("An error occurred submitting system alerts.", e, ErrorLog.Severity.Warning);
                }
                else
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             "An error occurred submitting system alerts.",
                                             e,
                                             ErrorLog.Severity.Warning);
                }
            }
        }

        public void SubmitServerDownStatusAlert(object syncObj, string instance)
        {
            if (syncObj == null)
                syncObj = startStopLock;

            try
            {
                // Lock for the entire status logging operation
                lock (syncObj)
                {
                    RemoteStatusLogger remoteLogger;

                    // Get the singleton remote status logger
                    remoteLogger = StatusLogger.GetRemoteLogger(Server, ServerPort);
                    remoteLogger.GenerateServerDownStatusAlert(instance);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "An error occurred submitting server down status alert.", e, ErrorLog.Severity.Warning);
            }
        }

        public bool ClearSystemAlertFlags(string instance)
        {
            bool success = false;
            try
            {
                RemoteStatusLogger remoteLogger;

                // Get the singleton remote status logger
                remoteLogger = StatusLogger.GetRemoteLogger(Server, ServerPort);

                try
                {
                    success = remoteLogger.ResetSystemAlertFlags(instance);
                    systemAlertError = false;
                    ReportCollectionServerConnectionResolution();
                }
                catch (Exception e)
                {
                    systemAlertError = true;
                    ReportCollectionServerConnectionError(e);
                }
            }
            catch (Exception e)
            {
                if (!systemAlertError)
                {
                    systemAlertError = true;
                    ErrorLog.Instance.Write("An error occurred clearing system alerts for " + instance + ".",
                                            e,
                                            ErrorLog.Severity.Warning);
                }
                else
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                             "An error occurred clearing system alerts for " +
                                             instance + ".",
                                             e,
                                             ErrorLog.Severity.Warning);
                }
            }
            return success;
        }

        #endregion

        #region Permissions Check

        internal string HasRightToReadRegistry()
        {
            if (stopping)
                return "SQLcompliance Agent service is stopping. Can't check registry key read rights.";

            string serviceAccount = CoreHelpers.GetServiceAccount(CoreConstants.AgentServiceName).Replace("\\", "\\\\");

            StringBuilder resolutionStepsBuilder = new StringBuilder();
            resolutionStepsBuilder.AppendFormat("SQLcompliance Agent Service is running under service account '{0}'. \\line ", serviceAccount);
            resolutionStepsBuilder.Append("An error occured while requesting read access to registry key 'HKEY_LOCAL_MACHINE\\\\Software\\\\Idera\\\\SQLCM'. \\line ");
            resolutionStepsBuilder.Append("Please visit the following link for instructions to manually view/edit the registry: http://wiki.idera.com/x/QIRi \\line ");

            // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
            // Handles the new registry permissions for non admin users
            var exceptionOccured = false;
            Exception ex = CoreHelpers.IsRegistryKeyReadable(@"HKEY_LOCAL_MACHINE\Software\Idera\SQLCM");
            if (ex != null)
            {
                resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck5, "An error occured while requesting read access to registry key'HKEY_LOCAL_MACHINE\\Software\\Idera\\SQLCM'.", ErrorLog.Severity.Informational);
                exceptionOccured = true;
            }

            if (!SecurityHelper.IsLocalAdmin())
            {
                ex = CoreHelpers.IsRegistryKeyReadable(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\Eventlog");
                if (ex != null)
                {
                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message.Replace("\\", "\\\\"));
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck2, "An error occured while requesting read access to registry key'HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Services\\Eventlog'.", ErrorLog.Severity.Informational);
                    exceptionOccured = true;
                }

                ex = CoreHelpers.IsRegistryKeyReadable(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\Eventlog");
                if (ex != null)
                {
                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message.Replace("\\", "\\\\"));
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck2, "An error occured while requesting read access to registry key'HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\EventLog\\Security'.", ErrorLog.Severity.Informational);
                    exceptionOccured = true;
                }
            }
            return exceptionOccured ? resolutionStepsBuilder.ToString() : string.Empty;
        }

        internal string HasPermissionToTraceDirectory()
        {
            if (stopping)
                return "SQLcompliance Agent service is stopping. Can't check trace directory permissions.";

            string serviceAccount = CoreHelpers.GetServiceAccount(CoreConstants.AgentServiceName).Replace("\\", "\\\\");

            StringBuilder resolutionStepsBuilder = new StringBuilder();
            resolutionStepsBuilder.AppendFormat("SQLcompliance Agent Service is running under service account '{0}' ", serviceAccount);
            resolutionStepsBuilder.AppendFormat("and trace directory path is '{0}'. \\line Please verify that trace directory allows read - write access for account '{1}'. \\line ", Instance.TraceDirectory.Replace("\\", "\\\\"), serviceAccount);

            Exception ex = CoreHelpers.IsDirectoryWritable(Instance.TraceDirectory);
            if (ex == null)
                return string.Empty;

            resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message);
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck4, string.Format("No permissions to agent trace directory: {1}{0}Service account: {2}", Environment.NewLine, Instance.TraceDirectory, serviceAccount), ErrorLog.Severity.Informational);
            return resolutionStepsBuilder.ToString();
        }

        internal bool isRowCountEnabled()
        {
            bool isEnabledRowCountEnabled = false;
            try
            {
                isEnabledRowCountEnabled = bool.Parse(ConfigurationManager.AppSettings[CoreConstants.Agent_EnableRowcount_configName]);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                "An error ocurred while getting EnableRowCountForDatabaseAuditing date from config file", ex);
            }
            return isEnabledRowCountEnabled;
        }


        internal string HasRightsToSqlServer(string instanceName)
        {
            if (string.IsNullOrEmpty(instanceName))
                return "SQL server instance name not provided.";
            if (stopping)
                return string.Format("SQLcompliance Agent service is stopping. Can't check permission to access SQL server instance '{0}'.", instanceName.Replace("\\", "\\\\"));

            string serviceAccount = CoreHelpers.GetServiceAccount(CoreConstants.AgentServiceName).Replace("\\", "\\\\");

            StringBuilder resolutionStepsBuilder = new StringBuilder();
            resolutionStepsBuilder.AppendFormat("SQLcompliance Agent Service is running under service account '{0}' ", serviceAccount);
            resolutionStepsBuilder.AppendFormat("and monitered SQL server instance name is '{0}'. \\line ", instanceName.Replace("\\", "\\\\"));

            bool checkPassed;
            SqlConnection connection = new SqlConnection(CreateConnectionString(instanceName, "master"));
            try
            {
                connection.Open();
                // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
                // Check sufficient permissions for non sysadmin users
                checkPassed = RawSQL.IsCurrentUserSysadmin(connection) || RawSQL.HasSufficientPermissions(connection);
                if (!checkPassed)
                {
                    resolutionStepsBuilder.AppendFormat("Please check that service account '{0}' has 'sa' permission to SQL server instance '{1}'. \\line ", serviceAccount, instanceName.Replace("\\", "\\\\"));
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck6, string.Format("Please check that service account '{0}' has 'sa' permission to SQL server instance '{1}'.", serviceAccount, instanceName), ErrorLog.Severity.Informational);
                }
            }
            catch (SqlException ex)
            {
                checkPassed = false;
                resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line ", ex.Message.Replace("\\", "\\\\"));
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck6, ex.Message, ErrorLog.Severity.Informational);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return checkPassed ? string.Empty : resolutionStepsBuilder.ToString();
        }

        internal string SqlServerHasPermissionToTraceDirectory(string instanceName)
        {
            if (string.IsNullOrEmpty(instanceName))
                return "SQL server instance name not provided.";
            if (stopping)
                return string.Format("SQLcompliance Agent service is stopping. Can't check SQL server instance '{0}' permissions to read-write agent trace directory '{1}'.", instanceName.Replace("\\", "\\\\"), Instance.TraceDirectory.Replace("\\", "\\\\"));

            StringBuilder resolutionStepsBuilder = new StringBuilder();
            resolutionStepsBuilder.AppendFormat("Path of SQLcompliance Agent trace directory is '{0}' ", Instance.TraceDirectory.Replace("\\", "\\\\"));
            resolutionStepsBuilder.AppendFormat("and SQL server instance name is '{0}'. \\line ", instanceName.Replace("\\", "\\\\"));

            //By Hemant
            var actualLength = Instance.TraceDirectory.Length;
            string dateTimeSuffix = DateTime.Now.ToString("yyyyMMdd-HHmmssfff").Replace("-", "");
            actualLength += @"ECHO >".Length; //adding length of commands
            actualLength += @"\SQLCM.tmp".Length; //adding length of name of tmp file
            actualLength += dateTimeSuffix.Length;
            //By Hemant
            //--------

            if (actualLength > 128)
            {
                resolutionStepsBuilder.Append("ERROR: Length of trace directory should be less than or equal to 128 characters. Select different trace directory. \\line ");
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck7, string.Format("Length of trace directory '{0}' should be less than or equal to 128 characters.", Instance.TraceDirectory), ErrorLog.Severity.Informational);
                return resolutionStepsBuilder.ToString();
            }

            // find SQL server service name
            string serviceName = GetWindowsServiceNameOfSqlInstance(instanceName);

            if (string.IsNullOrEmpty(serviceName))
            {
                resolutionStepsBuilder.Append("ERROR: Failed to determine SQL server service name. \\line ");
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck7, "Failed to determine SQL server service name.", ErrorLog.Severity.Informational);
                return resolutionStepsBuilder.ToString();
            }

            string serviceLogonAccount = CoreHelpers.GetServiceAccount(serviceName);
            bool checkPassed = true;

            resolutionStepsBuilder.AppendFormat("SQL server service name is '{0}' and service account is '{1}'. \\line ", serviceName, serviceLogonAccount.Replace("\\", "\\\\"));
            resolutionStepsBuilder.Append("Please manually verify that SQL server's service account has read-write access to trace directory. \\line ");
            resolutionStepsBuilder.Append("Due to security restrictions, the security on this folder will not allow us to process the files properly. Specifying \"modify\" access on the directory for \"everyone\" should fix this issue. \\line ");

            #region SQL server file write & delete check

            Exception exception;
            bool result = CoreHelpers.IsDirectoryWritableBySql(Instance.TraceDirectory, CreateConnectionString(instanceName, null), out exception);
            if (!result)
            {
                checkPassed = false;
                if (exception != null)
                {
                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line", exception.Message.Replace("\\", "\\\\"));
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck7, exception.Message, ErrorLog.Severity.Informational);
                }
                else
                {
                    resolutionStepsBuilder.Append("ERROR: SQL server service account is not having permissions to read and write trace directory. \\line ");
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck7, "SQL server service account is not having permissions to read and write trace directory.", ErrorLog.Severity.Informational);
                }
            }

            // don't check further
            if (!checkPassed)
                return resolutionStepsBuilder.ToString();

            #endregion

            #region service logon account ACL entry check 

            result = CoreHelpers.CheckAndGrantDirectoryAccessPermissions(Instance.TraceDirectory, serviceLogonAccount, out exception);
            if (!result)
            {
                checkPassed = false;
                if (exception != null)
                {
                    resolutionStepsBuilder.AppendFormat("ERROR: {0} \\line", exception.Message.Replace("\\", "\\\\"));
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck7, exception.Message, ErrorLog.Severity.Informational);
                }
                else
                {
                    resolutionStepsBuilder.Append("ERROR: SQL server service account is not having permissions to read and write trace directory. \\line ");
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug, CoreConstants.PermissionCheck7, "SQL server service account is not having permissions to read and write trace directory.", ErrorLog.Severity.Informational);
                }

            }

            #endregion

            return checkPassed ? string.Empty : resolutionStepsBuilder.ToString();
        }











        #endregion

    }
}
