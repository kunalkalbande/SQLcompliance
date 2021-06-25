using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.AlwaysOn;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Security;
using Idera.SQLcompliance.Core.Service;
using Idera.SQLcompliance.Core.Status;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AgentProperties;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.UpgradeAgent;
using ServerRole = SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase.ServerRole;
using Idera.SQLcompliance.Core.Remoting;
using SQLcomplianceCwfAddin.RestService;
using Microsoft.Win32;

namespace SQLcomplianceCwfAddin.Helpers.Agent
{
    public class AgentManagerHelper : Singleton<AgentManagerHelper>
    {
        #region Private Members

        private const int NO_ID = -1;
        private static int UNLIMITED_SIZE = -1;
        private static int MAX_TRACE_FILE_ROLLOVER_SIZE = 10;
        private static int MIN_INTERVAL_TIME = 1;
        private static int MIN_HEARTBEAT_INTERVAL_TIME = 2;
        private static int MAX_INTERVAL_TIME = 9999;
        private static int MAX_UNATTENDED_INTERVAL_TIME = 999;
        private static int SECONDS_IN_ONE_MINUTE = 60;
        private static int MIN_FOLDER_SIZE = 1;
        private static int MAX_FOLDER_SIZE = 9999;
        private static int MIN_TRACE_FILE_SIZE = 2;
        private static int MAX_TRACE_FILE_SIZE = 50;
        private static string CollectionServiceName = "Collection Service";
        private static string AgentServiceName = "SQLcompliance Agent";
        private static string Error_ServerNotAvailable = "The {0} on {1} cannot be reached. The {0} service may be down or a network error is preventing the Management Console from contacting the {0}. Your request may not be processed at this time.";
        private static string Info_CouldntStopAgent = "An error occurred trying to contact the SQLcompliance Agent to stop auditing of {0}. If you continue with the removal process, the agent will be left unaffected. If this is the last instance on the computer, you will need to manually remove the agent later to complete the process. \n\nError:\n\n{1}\n\n\n Do you wish to continue with the removal of this SQL Server instance?";
        private static string Error_DeleteServerProblem = "An error occurred trying to remove the registered SQL Server: {0}.\n\n Error:\n\n{1}";
        private static string Info_ManualUninstallRequired = "The SQLcompliance Agent service for {0} is no longer capturing audit data. However, because this agent was installed manually you will need to manually run the uninstall to complete this task.";
        private static string Error_UninstallAgent = "An error occurred uninstalling the SQLcompliance Agent for {0}.\n\nError:\n\n{1}";
        private static string Error_CantUninstallAgent = "The SQLcompliance Agent cannot be uninstalled.";
        private static string Status_Requested = "Update requested";
        private static string Error_UpdateNowFailed = "An error occurred issuing the request to update the audit settings for the selected SQLcompliance Agent.";
        private static string AgentConfigurationUpdatingWarning = "Agent audit settings couldn't be updated because last known server configuration version is {0} and actual server configuration version is {1}";
        private static string Info_RolloverSizeWarning = "Warning: Trace file rollover size affects the size of the trace files produced by SQL Server. This size has an impact on the amount of memory used by the Collection Server. Use care in changing this value as it may impact the performance of the Collection Server.";
        private static string Error_BadTraceFileRollover = "'Trace File Rollover Size' must be between 2 Mb and 50 Mb.";
        private static string Error_BadCollectionInterval = "'Collection Interval' must be at least 1 min";
        private static string Error_BadForcedCollectionInterval = "'Forced Collection Interval' must be at least 1 min";
        private static string Error_BadTraceStartTimeout = "'Trace Start Timeout' must be at least 1 second and less than the 'Collection Interval'";
        private static string Error_BadTamperDetectionInterval = "'Tamper Detection Interval' must be at least 1 second";
        private static string Error_BadHeartbeatFrequency = "'Heartbeat Interval' must be at least 2 minutes and less than 9,999 minutes.";
        private static string Error_BadMaxFolderSize = "'Trace Directory Size' must be at least 1 Gb";
        private static string Error_BadMaxUnattendedTime = "'Maximum Unattended Time' must be at least 1 day";
        private static string Error_ErrorSavingServer = "An error occurred trying to save the changes to the registered server. The server may be modified after the problem is resolved.";

        private static string Info_CantUpgradeLocal = "The local SQLcompliance Agent may not be upgraded from the management console. You must upgrade the SQLcompliance Agent using the full setup program used to install the SQLcompliance Agent and Management Console.";
        private static string Info_CantUpgradeManual = "The SQLcompliance Agent may not be upgraded from the management console. You must upgrade the SQLcompliance Agent using the full setup program used to install the manually deployed SQLcompliance Agent.";
        private static string Info_CantUpgradeAgentNewer = "The SQLcompliance Agent cannot be upgraded from this computer. The remote SQLcompliance Agent is newer then the local version of the Management Console.";
        private static string Info_AlreadyUpgraded = "The SQLcompliance Agent is already current. No upgrade is necessary";


        #endregion

        #region private methods

        private string TranslateRemotingException(string server, string component, Exception ex)
        {
            string msg = ex.Message;

            try
            {
                SocketException socketEx = (SocketException) ex;
                if (socketEx.ErrorCode == 10061)
                {
                    msg = String.Format(Error_ServerNotAvailable, component, server);
                }
            }
            catch
            {
                
            }

            return msg;
        }

        private string LogCollectionServerError(string message, Exception ex, SqlConnection connection)
        {
            var config = SqlCmConfigurationHelper.GetConfiguration(connection);
            string error = TranslateRemotingException(config.Server, CollectionServiceName, ex);
            string loggedMessage = string.Format("{0}. Error: {1}", message, error);
            LogError(loggedMessage);
            return loggedMessage;
        }

        private List<ServerLogin> ConvertRawLoginObjectList(IEnumerable rawLoginObjectList)
        {
            var loginList = new List<ServerLogin>();
            foreach (RawLoginObject login in rawLoginObjectList)
            {
                loginList.Add(
                    new ServerLogin
                    {
                        Sid = Convert.ToBase64String(login.sid),
                        Name = login.name,
                    });
            }

            var sortedList = loginList.OrderBy(item => item.Name).ToList();
            return sortedList;
        }

        private List<ServerRole> ConvertRawRoleObjectList(IEnumerable rawRoleObjectList)
        {
            var roleList = new List<ServerRole>();

            foreach (RawRoleObject role in rawRoleObjectList)
            {
                roleList.Add(
                    new ServerRole
                    {
                        Id = role.roleid,
                        Name = role.name,
                        FullName = role.fullName,
                    });
            }
            var sortedList = roleList.OrderBy(item => item.FullName).ToList();
            return sortedList;
        }

        private bool IsAvailAbilityGroupSupported(ServerRecord server)
        {
            var sqlServer = new SqlDirect();

            if (sqlServer.OpenConnection(server.Instance))
            {
                int dot = sqlServer.Connection.ServerVersion.IndexOf(".");
                int SQLver = Convert.ToInt32(sqlServer.Connection.ServerVersion.Substring(0, dot));
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                    String.Format("The SQL Server Version is {0} for SQL Server {1}", sqlServer.Connection.ServerVersion, server.Instance),
                    ErrorLog.Severity.Informational);

                if (SQLver < 11)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetClrStatusMessage(ClrStatus status, ServerRecord server)
        {
            if (status.IsConfigured != status.IsRunning)
            {
                // RECONFIGURE failed it appears
                if (status.IsConfigured)
                {
                    status.StatusMessage = "CLR cannot be enabled.  Verify that lightweight pooling is disabled.";
                }
                else
                {
                    status.StatusMessage = "CLR is running but not conifgured.";
                }
            }
            else if (status.IsRunning)
            {
                status.StatusMessage = string.Format("CLR is enabled for {0}.", server.Instance);
            }
            else
            {
                status.StatusMessage = string.Format("CLR is not enabled for {0}.", server.Instance);
            }
        }

        private bool IsServerUp(ServerManager serverManager, ServerRecord server)
        {
            try
            {
                if (server.IsAuditedServer &&
                    server.IsDeployed)
                {
                    serverManager.Ping();
                    return true;
                }
            }
            catch (Exception ex)
            {

                ErrorLog.Instance.Write(ErrorLog.Level.Default,
                               string.Format("IsServerUp method faild for Instance {0}.", server.Instance),
                               ex,
                               ErrorLog.Severity.Error);
            }

            return false;
        }

        private bool StartAgentService(string agentServer)
        {
            try
            {
                // This will fail across untrusted domains/workgroups.  However, it is
                //  not a fatal error, so we silently catch and move along.  The true
                //  point of failure will be in the following AgentManager.Activate().
                AgentServiceManager serviceManager = new AgentServiceManager(null, null, agentServer, null, null);
                serviceManager.Start();
                return true;
            }
            catch (Exception ex)
            {
                LogError(string.Format("Failed to start Agent Service due to the following error: {0}", ex));
                return false;
            }
        }

        private string GetServerNameFromInstance(string instance)
        {
            return instance.IndexOf("\\") == -1 ? instance : instance.Substring(0, instance.IndexOf("\\"));
        }

        private bool DeactivateAgentViaFirewall(ServerRecord server, bool removeEventsDatabase, SqlConnection connection)
        {
            try
            {
                // server up - get it to stop agent (gets us through firewalls)
                var manager = GetAgentManagerProxy(connection);
                manager.Deactivate(server.Instance, removeEventsDatabase);
                return true;
            }
            catch (Exception ex)
            {
                LogCollectionServerError(string.Format(Info_CouldntStopAgent, server.Instance), ex, connection);
                return false;
            }
        }

        private bool DeactivateAgentDirectly(ServerRecord server, bool removeEventsDatabase)
        {
            try
            {
                // server down - try direct to agent stop request
                var agentCmd = GetAgentCommandProxy(server.AgentServer, server.AgentPort);
                agentCmd.Deactivate(server.Instance, removeEventsDatabase);
                return true;
            }
            catch (Exception ex)
            {
                string error = TranslateRemotingException(server.AgentServer, AgentServiceName, ex);
                ErrorLog.Instance.Write(
                    ErrorLog.Level.Default,
                    string.Format(Info_CouldntStopAgent, server.Instance, error),
                    ex,
                    ErrorLog.Severity.Error);

                return false;
            }
        }

        private void MarkServerInActive(ServerRecord server)
        {
            var oldSrv = server.Clone();
            server.IsRunning = false;
            server.IsEnabled = false;
            server.Write(oldSrv);
        }

        private void RemoteUninstallAgentService(string localAdmin, string localPassword, string targetServer, string remoteAdmin, string remotePassword)
        {
            // Instantiate the SQLsecure service manager
            var agentServiceManager = new AgentServiceManager(localAdmin, localPassword, targetServer, remoteAdmin, remotePassword);

            // Uninstall the service (which will stop it)
            agentServiceManager.Uninstall();
        }

        private void SetAgentSettings(AgentGeneralProperties general, ServerRecord server)
        {
            general.AgentSettings = new AgentSettings();
            var agentSettings = general.AgentSettings;
            agentSettings.IsDeployed = server.IsDeployed;
            agentSettings.Version = server.AgentVersion;
            agentSettings.Port = server.AgentPort;
            agentSettings.LastHeartbeatDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(server.TimeLastHeartbeat);
            agentSettings.HeartbeatInterval = server.AgentHeartbeatInterval;
            agentSettings.LoggingLevel = (LoggingLevel)Enum.ToObject(typeof(LoggingLevel), server.AgentLogLevel);
        }

        private void SetAgentAuditSettings(AgentGeneralProperties general, ServerRecord server)
        {
            general.AuditSettings = new AgentAuditSettings();
            var auditSettings = general.AuditSettings;
            auditSettings.LastAgentUpdateDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(server.LastConfigUpdate);
            auditSettings.AuditSettingsUpdateEnabled = server.ConfigVersion > server.LastKnownConfigVersion;
            auditSettings.AgentAuditLevel = server.LastKnownConfigVersion;
            auditSettings.CurrentAuditLevel = server.ConfigVersion;
        }

        private void SetGeneralProperties(AgentProperties properties, ServerRecord server)
        {
            properties.GeneralProperties = new AgentGeneralProperties();
            var general = properties.GeneralProperties;
            general.AgentComputer = server.InstanceServer;
            SetAgentSettings(general, server);
            SetAgentAuditSettings(general, server);
        }

        private void SetAgentDeploymentSettings(AgentProperties properties, ServerRecord server)
        {
            properties.Deployment = new AgentDeployment();
            var deployment = properties.Deployment;
            deployment.ServiceAccount = server.AgentServiceAccount;
            deployment.WasManuallyDeployed = server.IsDeployedManually;
        }

        private List<ServerRecord> GetServerRecordListForServer(string instanceServer, SqlConnection connection)
        {
            var serverList = ServerRecord.GetServers(connection, false);
            var list = new List<ServerRecord>();
            foreach (var server in serverList)
            {
                if (server.InstanceServer.ToUpper() == instanceServer.ToUpper())
                {
                    list.Add(server);
                }
            }
            return list;
        }

        private void SetAgentSQLServerList(AgentProperties properties, ServerRecord server, SqlConnection connection)
        {
            var serverList = GetServerRecordListForServer(server.InstanceServer, connection);
            var sqlServerInfoList = Converter.ConvertList<ServerRecord, SQLServerInfo>(
                                                            serverList,
                                                            serverRecord => new SQLServerInfo
                                                            {
                                                                Instance = serverRecord.Instance,
                                                                Description = serverRecord.Description
                                                            });
            properties.SqlServerList = sqlServerInfoList;
        }

        private void SetTraceOptions(AgentProperties properties, ServerRecord server)
        {
            properties.TraceOptions = new AgentTraceOptions();
            var options = properties.TraceOptions;
            options.AgentTraceDirectory = server.AgentTraceDirectory;
            options.TraceFileRolloverSize = server.AgentMaxTraceSize;
            options.CollectionInterval = server.AgentCollectionInterval;
            options.ForceCollectionInterval = server.AgentForceCollectionInterval;
            options.TraceStartTimeoutEnabled = false;

            if (server.AgentVersion == null ||
               server.AgentVersion == "")
            {
                options.TemperDetectionInterval = CoreConstants.Agent_Default_TamperingDetectionInterval;
            }
            else if (server.AgentVersion.StartsWith("1.1") ||
                    server.AgentVersion.StartsWith("1.2") ||
                    server.AgentVersion.StartsWith("2.0"))
            {
                // Old agents don't support this option
                options.TemperDetectionIntervalEnabled = false;
                options.TemperDetectionInterval = CoreConstants.Agent_Default_TamperingDetectionInterval;
            }
            else
            {
                options.TemperDetectionInterval = server.DetectionInterval;
                options.TraceStartTimeoutEnabled = ServerRecord.CompareVersions(server.AgentVersion, "3.3") >= 0;
            }

            options.TraceStartTimeout = options.TraceStartTimeoutEnabled
                ? server.AgentTraceStartTimeout : CoreConstants.Agent_Default_TraceStartTimeout;

            options.TraceDirectorySizeLimit = server.AgentMaxFolderSize;
            options.UnattendedTimeLimit = server.AgentMaxUnattendedTime;
        }

        private void ValidateRange(long value, long minValue, long maxValue, string errorMessage)
        {
            if (value < minValue ||
                value > maxValue)
            {
                throw new Exception(errorMessage);
            }
        }

        private void ValidateRange(long newValue, long oldValue, long minValue, long maxValue, string errorMessage)
        {
            if (newValue == oldValue)
            {
                return;
            }

            ValidateRange(newValue, minValue, maxValue, errorMessage);
        }

        private void UpdateServerRecordWithGeneralProperties(ServerRecord server, AgentGeneralProperties general)
        {
            server.AgentHeartbeatInterval = general.AgentSettings.HeartbeatInterval;
            server.AgentLogLevel = Convert.ToInt32(general.AgentSettings.LoggingLevel);
        }

        private void UpdateServerRecordWithTraceOptions(ServerRecord server, AgentTraceOptions options)
        {
            server.AgentTraceDirectory = options.AgentTraceDirectory;
            server.AgentMaxTraceSize = options.TraceFileRolloverSize;
            server.AgentCollectionInterval = options.CollectionInterval;
            server.AgentForceCollectionInterval = options.ForceCollectionInterval;
            server.AgentTraceStartTimeout = options.TraceStartTimeout;
            server.DetectionInterval = options.TemperDetectionInterval;
            server.AgentMaxFolderSize = options.TraceDirectorySizeLimit;
            server.AgentMaxUnattendedTime = options.UnattendedTimeLimit;
        }

        private ServerRecord CreateServerRecord(AgentProperties agentProperties, ServerRecord server, SqlConnection connection)
        {
            ServerRecord newServer = server.Clone();
            newServer.Connection = connection;
            UpdateServerRecordWithGeneralProperties(newServer, agentProperties.GeneralProperties);
            UpdateServerRecordWithTraceOptions(newServer, agentProperties.TraceOptions);

            if (!server.IsDeployed)
            {
                newServer.IsDeployedManually = agentProperties.Deployment.WasManuallyDeployed;
            }

            return newServer;
        }

        private void ValidateRequieredAgentSettings(AgentProperties agentProperties)
        {
            if (agentProperties == null)
            {
                throw new Exception("Agent properties couldn't be null.");
            }
            else if (agentProperties.GeneralProperties == null)
            {
                throw new Exception("General properties couldn't be null.");
            }
            else if (agentProperties.GeneralProperties.AgentSettings == null)
            {
                throw new Exception("Agent settings couldn't be null.");
            }
            else if (agentProperties.Deployment == null)
            {
                throw new Exception("Deployment properties couldn't be null.");
            }
            else if (agentProperties.TraceOptions == null)
            {
                throw new Exception("Trace options couldn't be null.");
            }
        }

        private void ValidateAgentProperties(AgentProperties agentProperties, ServerRecord server, SqlConnection connection)
        {
            ValidateRequieredAgentSettings(agentProperties);

            if (server.AgentMaxTraceSize <= MAX_TRACE_FILE_ROLLOVER_SIZE &&
                agentProperties.TraceOptions.TraceFileRolloverSize >= MAX_TRACE_FILE_ROLLOVER_SIZE)
            {
                LogWarning(Info_RolloverSizeWarning);
            }

            if (server.AgentTraceDirectory != agentProperties.TraceOptions.AgentTraceDirectory)
            {
                PathHelper.ValidateTraceDirectoryPath(agentProperties.TraceOptions.AgentTraceDirectory);
            }

            ValidateRange(agentProperties.TraceOptions.TraceFileRolloverSize, server.AgentMaxTraceSize, MIN_TRACE_FILE_SIZE, MAX_TRACE_FILE_SIZE, Error_BadTraceFileRollover);
            ValidateRange(agentProperties.TraceOptions.CollectionInterval, server.AgentCollectionInterval, MIN_INTERVAL_TIME, MAX_INTERVAL_TIME, Error_BadCollectionInterval);
            ValidateRange(agentProperties.TraceOptions.ForceCollectionInterval, server.AgentForceCollectionInterval, MIN_INTERVAL_TIME, MAX_INTERVAL_TIME, Error_BadForcedCollectionInterval);
            //Always validate the timeout value since it is dependent on other values.
            //the collection interval is in minutes and the trace timeout is in seconds
            ValidateRange(agentProperties.TraceOptions.TraceStartTimeout, MIN_INTERVAL_TIME, agentProperties.TraceOptions.CollectionInterval * SECONDS_IN_ONE_MINUTE, Error_BadTraceStartTimeout);

            ValidateRange(agentProperties.TraceOptions.TemperDetectionInterval, server.DetectionInterval, MIN_INTERVAL_TIME, MAX_INTERVAL_TIME, Error_BadTamperDetectionInterval);
            ValidateRange(agentProperties.GeneralProperties.AgentSettings.HeartbeatInterval, server.AgentHeartbeatInterval, MIN_HEARTBEAT_INTERVAL_TIME, MAX_INTERVAL_TIME, Error_BadHeartbeatFrequency);

            if (agentProperties.TraceOptions.TraceDirectorySizeLimit != UNLIMITED_SIZE)
            {
                ValidateRange(agentProperties.TraceOptions.TraceDirectorySizeLimit, server.AgentMaxFolderSize, MIN_FOLDER_SIZE, MAX_FOLDER_SIZE, Error_BadMaxFolderSize);
            }

            if (agentProperties.TraceOptions.UnattendedTimeLimit != UNLIMITED_SIZE)
            {
                ValidateRange(agentProperties.TraceOptions.UnattendedTimeLimit, server.AgentMaxUnattendedTime, MIN_INTERVAL_TIME, MAX_UNATTENDED_INTERVAL_TIME, Error_BadMaxUnattendedTime);
            }
        }

        private string CreateSnapshotString(ServerRecord server)
        {
            StringBuilder snap = new StringBuilder(1024);

            snap.AppendFormat("\tHeartbeat interval:\t{0}\r\n", server.AgentHeartbeatInterval);
            string lvl;
            if (server.AgentLogLevel == 3)
                lvl = "Debug";
            else if (server.AgentLogLevel == 2)
                lvl = "Verbose";
            else if (server.AgentLogLevel == 1)
                lvl = "Normal";
            else
                lvl = "Silent";
            snap.AppendFormat("\tLogging level:\t{0}\r\n", lvl);


            snap.AppendFormat("\tTrace rollover size:\t{0}\r\n", server.AgentMaxTraceSize);
            snap.AppendFormat("\tCollection interval\t{0}\r\n", server.AgentCollectionInterval);
            snap.AppendFormat("\tForced collection interval\t{0}\r\n", server.AgentForceCollectionInterval);

            //add the try catch to hide an error if the property doesn't exists.
            try
            {
                snap.AppendFormat("\tTrace Start Timeout\t\t{0}\r\n", server.AgentTraceStartTimeout);
            }
            catch { }


            if (server.AgentVersion == null || server.AgentVersion == "" ||
               (!server.AgentVersion.StartsWith("1.1") &&
               !server.AgentVersion.StartsWith("1.2") &&
               !server.AgentVersion.StartsWith("2.0")))
            {
                snap.AppendFormat("\tTamper detection interval\t{0}\r\n", server.DetectionInterval);
            }

            string lim;
            if (server.AgentMaxFolderSize == -1)
                lim = "Unlimited";
            else
                lim = String.Format("{0} GB", server.AgentMaxFolderSize);
            snap.AppendFormat("\tTrace directory size limit\t{0}\r\n", lim);

            if (server.AgentMaxUnattendedTime == -1)
                lim = "Unlimited";
            else
                lim = String.Format("{0} days", server.AgentMaxUnattendedTime);
            snap.AppendFormat("\tUnattended auditing time limit\t{0}\r\n", lim);

            return snap.ToString();
        }

        private string GetSqlComplianceManagerVersion()
        {
            try
            {
                var sqlComplianceKey = Registry.LocalMachine.OpenSubKey(CoreConstants.SQLcompliance_RegKey);
                if (sqlComplianceKey != null)
                {
                    string version = (string)sqlComplianceKey.GetValue(CoreConstants.SQLcompliance_RegKey_Version);
                    return version;
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format("Failed read SQL Compliance Management version from registry due to the error: {0}", ex));
            }

            return RestServiceConstants.ProductVersion;
        }

        private UpgradeAgentResponse UpgradeAgentMinor(SqlConnection connection, ServerRecord server)
        {
            var upgradeAgentResponse = new UpgradeAgentResponse();
            var config = SqlCmConfigurationHelper.GetConfiguration(connection);
            try
            {
                AgentControl.RemoteMinorUpgradeAgentService(null, null, server.InstanceServer, null, null, config.Server);
                upgradeAgentResponse.Success = true;
            }
            catch (Exception ex)
            {
                var errorMessage = ex.ToString();
                LogError(errorMessage);
                upgradeAgentResponse.Success = false;
                upgradeAgentResponse.ErrorMessage = ex.Message;
                return upgradeAgentResponse;
            }

            IsUpgradeSuccesfull(server.Instance, upgradeAgentResponse, connection);

            return upgradeAgentResponse;
        }

        private UpgradeAgentResponse UpgradeAgentMajor(SqlConnection connection, ServerRecord server, string serviceAccount, string servicePassword)
        {
            var upgradeAgentResponse = new UpgradeAgentResponse();

            try
            {
                AgentControl.RemoteMajorUpgradeAgentService(null, null, server.InstanceServer, null, null, serviceAccount, servicePassword);
                upgradeAgentResponse.Success = true;
            }
            catch(CoreException ex)
            {
                if (ex.Message == CoreConstants.Exception_ErrorRebootRequired)
                {
                    LogError(ex);
                    upgradeAgentResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.ToString();
                LogError(errorMessage);
                upgradeAgentResponse.Success = false;
                upgradeAgentResponse.ErrorMessage = ex.Message;
                return upgradeAgentResponse;
            }

            IsUpgradeSuccesfull(server.Instance, upgradeAgentResponse, connection);

            return upgradeAgentResponse;
        }

        private void IsUpgradeSuccesfull(string instance, UpgradeAgentResponse upgradeAgentResponse, SqlConnection connection)
        {
            bool wasAgentVersionVerified = false;

            try
        {
            ServerRecord srv = new ServerRecord() { Connection = connection };

            if (srv.Read(instance))
            {
                if (!string.IsNullOrEmpty(srv.AgentVersion))
                {
                        string cmVersion = GetSqlComplianceManagerVersion();

                        if (ServerRecord.CompareVersions(srv.AgentVersion, cmVersion) == 0)
                    {
                            wasAgentVersionVerified = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format("Could verify if agent upgrade for instance {0} was successfull because of error: {1}", instance, ex));
            }

            upgradeAgentResponse.UpgradeStatusMessage = wasAgentVersionVerified ?
                RestServiceConstants.Info_UpgradeComplete :
                RestServiceConstants.Error_UpgradeFailedNoInfo;
        }
        
        #endregion

        #region public methods

        public ServerManager GetServerManagerProxy(string server, int port)
        {
            var manager = ProxyObjecHelper.CreateProxyObject<ServerManager>(server, port);
            return manager;
        }

        public ServerManager GetServerManagerProxy(SqlConnection sqlCmRepositoryConnection)
        {
            var config = SqlCmConfigurationHelper.GetConfiguration(sqlCmRepositoryConnection);
            return GetServerManagerProxy(config.Server, config.ServerPort);
        }

        public AgentCommand GetAgentCommandProxy(string server, int port)
        {
            var commandAgent = ProxyObjecHelper.CreateProxyObject<AgentCommand>(server, port);
            return commandAgent;
        }

        public AgentManager GetAgentManagerProxy(string server, int port)
        {
            var manager = ProxyObjecHelper.CreateProxyObject<AgentManager>(server, port);
            return manager;
        }

        public AgentManager GetAgentManagerProxy(SQLcomplianceConfiguration config)
        {
            return GetAgentManagerProxy(config.Server, config.ServerPort);
        }

        public AgentManager GetAgentManagerProxy(SqlConnection sqlCmRepositoryConnection)
        {
            var config = SqlCmConfigurationHelper.GetConfiguration(sqlCmRepositoryConnection);
            return GetAgentManagerProxy(config);
        }

        public ServerRolesAndUsers GetServerRolesAndUsers(SqlConnection connection, int serverId)
        {
            ServerRecord server = null;
            IList rawLoginList = null;
            IList rawRoleList = null;

            var rolesAndUsersData = new ServerRolesAndUsers();

            try
            {
                server = SqlCmRecordReader.GetServerRecord(serverId, connection);

                if (server.IsDeployed && server.IsRunning)
                {
                    var agentManager = GetAgentManagerProxy(connection);
                    rawLoginList = agentManager.GetRawServerLogins(server.Instance);
                    rawRoleList = agentManager.GetRawServerRoles(server.Instance);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading server roles or logins for Instance Id {0}", serverId),
                                            ex,
                                            ErrorLog.Severity.Warning);
                rawLoginList = null;
                rawRoleList = null;
            }

            if (server != null &&
                rawLoginList == null &&
                rawRoleList == null)
            {
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    rawLoginList = RawSQL.GetServerLogins(sqlServer.Connection);
                    rawRoleList = RawSQL.GetServerRoles(sqlServer.Connection);
                }                
            }

            rolesAndUsersData.UserList = ConvertRawLoginObjectList(rawLoginList);
            rolesAndUsersData.RoleList = ConvertRawRoleObjectList(rawRoleList);

            return rolesAndUsersData;
        }

        public List<AvailabilityGroup> GetAvailabilityGroupList(List<AuditedDatabaseInfo> databaseList, SqlConnection connection)
        {
            var availabilityGroupList = new List<AvailabilityGroup>();
            ServerRecord server = null;
            IEnumerable rawAvailabilityGroupList = null;

            if (databaseList == null ||
                databaseList.Count == 0)
            {
                return availabilityGroupList;
            }

            int serverId = databaseList[0].ServerId;
            var databaseNameList = Converter.ConvertReferenceList<AuditedDatabaseInfo, string>(databaseList, database => database.Name);

            try
            {
                server = SqlCmRecordReader.GetServerRecord(serverId, connection);
                if (!IsAvailAbilityGroupSupported(server))
                {
                    return availabilityGroupList;
                }

                if (server.IsDeployed && server.IsRunning)
                {
                    var agentManager = GetAgentManagerProxy(connection);
                    
                    rawAvailabilityGroupList = agentManager.GetRawAVGDetails(server.Instance, databaseNameList);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading availability groups for server id {0}", serverId),
                                            ex,
                                            ErrorLog.Severity.Warning);
                rawAvailabilityGroupList = null;
            }

            if (server != null &&
                rawAvailabilityGroupList == null)
            {
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    rawAvailabilityGroupList = RawSQL.GetAvailabilityGroupDetails(connection, databaseNameList);
                }
            }

            availabilityGroupList.AddRange(
                Converter.ConvertReferenceList<RawAVGroup, AvailabilityGroup>(
                rawAvailabilityGroupList, 
                rawGroup => new AvailabilityGroup
                {
                    Name = rawGroup.avgName, 
                    DatabaseName = rawGroup.dbName, 
                    ReplicaServerName = rawGroup.replicaServerName, 
                    ReplicaCount = rawGroup.replicaCount
                }));

            return availabilityGroupList;
        }

        public List<string> GetReadOnlySecondaryReplicaServerList(ServerRecord server, SqlConnection connection)
        {
            List<string> replicaServerList = null;

            try
            {
                if (!IsAvailAbilityGroupSupported(server))
                {
                    return new List<string>();
                }

                if (server.IsDeployed && server.IsRunning)
                {
                    var agentManager = GetAgentManagerProxy(connection);

                    replicaServerList = agentManager.GetReadOnlySecondaryReplicaServerList(server.Instance);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading read only secondary replica servers from listener server {0}", server.Instance),
                                            ex,
                                            ErrorLog.Severity.Warning);
            }

            if (server != null &&
                replicaServerList == null)
            {
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    replicaServerList = RawSQL.GetReadOnlySecondaryReplicaServerList(connection);
                }
            }

            return replicaServerList;
        }

        public List<ReplicaNodeInfo> GetAllReplicaNodeInfoList(ServerRecord server, SqlConnection connection)
        {
            List<ReplicaNodeInfo> replicaServerList = null;

            try
            {
                if (!IsAvailAbilityGroupSupported(server))
                {
                    return new List<ReplicaNodeInfo>();
                }

                if (server.IsDeployed && server.IsRunning)
                {
                    var agentManager = GetAgentManagerProxy(connection);

                    replicaServerList = agentManager.GetAllReplicaNodeInfoList(server.Instance);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading read only secondary replica servers from listener server {0}", server.Instance),
                                            ex,
                                            ErrorLog.Severity.Warning);
            }

            if (server != null &&
                replicaServerList == null)
            {
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    replicaServerList = RawSQL.GetAllReplicaNodeInfoList(connection);
                }
            }

            return replicaServerList;
        }

        public List<DatabaseObject> GetDatabaseTableList(DatabaseTableFilter filter, SqlConnection connection)
        {
            List<DatabaseObject> tableList = null;
            ServerRecord server = null;
            IEnumerable rawTableList = null;

            try
            {
                server = SqlCmRecordReader.GetServerRecord(filter.ServerId, connection);

                if (server.IsDeployed && server.IsRunning)
                {
                    var agentManager = GetAgentManagerProxy(connection);
                    rawTableList = agentManager.GetRawTables(server.Instance, filter.DatabaseName, filter.TableNameSearchText);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading table objects for database '{0}'", filter.DatabaseName),
                                            ex,
                                            ErrorLog.Severity.Warning);
                rawTableList = null;
            }


            // straight connection to SQL Server
            if (server != null && 
                rawTableList == null)
            {
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    rawTableList = RawSQL.GetTables(connection, filter.DatabaseName, filter.TableNameSearchText);
                }
            }
            
            if (rawTableList != null)
            {
                var databaseId = DatabaseRecord.GetDatabaseId(connection, server.SrvId, filter.DatabaseName);

                tableList = new List<DatabaseObject>();

                foreach (RawTableObject rawTableObject in rawTableList)
                {
                    var tableObject = new DatabaseObject()
                    {
                        Id = rawTableObject.id,
                        ServerId = server.SrvId,
                        ObjectId = rawTableObject.id,
                        DatabaseId = databaseId,
                        ObjectType = ObjectType.Table,
                        TableName = rawTableObject.TableName,
                        FullTableName = rawTableObject.FullTableName,
                        SchemaName = rawTableObject.SchemaName,
                    };

                    tableList.Add(tableObject);
                }
            }

            return tableList;
        }

        ////SQLCM 5.4 Start

        public List<DatabaseTableSummary> GetTableDetailsForAll(DatabaseTableDetailsForAllFilter filter, SqlConnection connection)
        {
            List<DatabaseTableSummary> tableList = null;
            ServerRecord server = null;
            IEnumerable rawTableDetails = null;
			//Commented for SCM-1938 RC2
            //try
            //{
            //    server = SqlCmRecordReader.GetServerRecord(filter.ServerId, connection);

            //    if (server.IsDeployed && server.IsRunning)
            //    {
            //        var agentManager = GetAgentManagerProxy(connection);
                  
            //            //rawTableDetails = agentManager.GetRawTableDetailsForAll(server.Instance, filter.DatabaseList, filter.ProfileName);
            //        rawTableDetails = RawSQL.GetTableDetailsForAll(connection, filter.DatabaseList, filter.ProfileName);
            //    }
                
            //}
            //catch (Exception ex)
            //{
            //    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
            //                                String.Format("Loading table objects for database '{0}'", filter.DatabaseList),
            //                                ex,
            //                                ErrorLog.Severity.Warning);
            //    rawTableDetails = null;
            //}


            // straight connection to SQL Server
            server = SqlCmRecordReader.GetServerRecord(filter.ServerId, connection);
            if (server != null &&
                rawTableDetails == null)
            {                
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    rawTableDetails = RawSQL.GetTableDetailsForAll(connection, sqlServer.Connection, filter.DatabaseList, filter.ProfileName);
                }
            }

            if (rawTableDetails != null)
            {
                //var databaseId = DatabaseRecord.GetDatabaseId(connection, server.SrvId, filter.DatabaseName);

                tableList = new List<DatabaseTableSummary>();

                foreach (RawTableDetails rawTableObject in rawTableDetails)
                {
                    var tableObject = new DatabaseTableSummary()
                    {
                        DatabaseName = rawTableObject.DatabaseName,
                        SchemaTableName = rawTableObject.TableName,
                        Size = rawTableObject.Size,
                        RowCount = rawTableObject.RowCount,
                        ColumnsIdentified = rawTableObject.ColumnsIdentified,

                    };

                    tableList.Add(tableObject);
                }
            }

            return tableList;
        }


        public List<AuditedDatabaseInfo> GetDatabaseDetailsForAll(SqlConnection connection, int servId)
        {
            List<AuditedDatabaseInfo> dbList = null;
            IEnumerable rawDatabaseDetails = null;
            ServerRecord server = null;

            try
            {
                server = SqlCmRecordReader.GetServerRecord(servId, connection);
                // straight connection to SQL Server
                if (server != null &&
                    rawDatabaseDetails == null)
                {
                    var sqlServer = new SqlDirect();
                    if (sqlServer.OpenConnection(server.Instance))
                    {
                        rawDatabaseDetails = RawSQL.GetAllUserDatabases(servId, sqlServer.Connection);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading table objects for database "),
                                            ex,
                                            ErrorLog.Severity.Warning);
                rawDatabaseDetails = null;
            }

            

            if (rawDatabaseDetails != null)
            {

                dbList = new List<AuditedDatabaseInfo>();

                foreach (RawDatabaseDetails rawTableObject in rawDatabaseDetails)
                {
                    var dbObject = new AuditedDatabaseInfo()
                    {
                        Name = rawTableObject.DatabaseName,
                        Id = rawTableObject.DbId,
                        ServerId = rawTableObject.ServerId 
                    };

                    dbList.Add(dbObject);
                }
            }

            return dbList;
        }




        public List<ColumnTableSummary> GetColumnDetailsForAll(DatabaseTableDetailsForAllFilter filter, SqlConnection connection)
        {
            List<ColumnTableSummary> tableList = null;
            ServerRecord server = null;
            IEnumerable rawTableDetails = null;			
			//Commented for SCM-1938 RC2
            //try 
            //{
            //    server = SqlCmRecordReader.GetServerRecord(filter.ServerId, connection);

            //    if (server.IsDeployed && server.IsRunning)
            //    {
            //        var agentManager = GetAgentManagerProxy(connection);

            //        //rawTableDetails = agentManager.GetRawColumnDetailsForAll(server.Instance, filter.DatabaseList, filter.ProfileName);
            //        rawTableDetails = RawSQL.GetColumnDetailsForAll(connection, filter.DatabaseList, filter.ProfileName);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
            //                                String.Format("Loading table objects for database '{0}'", filter.DatabaseList),
            //                                ex,
            //                                ErrorLog.Severity.Warning);
            //    rawTableDetails = null;
            //}


            // straight connection to SQL Server
            server = SqlCmRecordReader.GetServerRecord(filter.ServerId, connection);
            if (server != null &&
                rawTableDetails == null)
            {
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    rawTableDetails = RawSQL.GetColumnDetailsForAll(connection,sqlServer.Connection, filter.DatabaseList, filter.ProfileName);
                }
            }

            if (rawTableDetails != null)
            {
                //var databaseId = DatabaseRecord.GetDatabaseId(connection, server.SrvId, filter.DatabaseName);

                tableList = new List<ColumnTableSummary>();

                foreach (RawColumnDetails rawTableObject in rawTableDetails)
                {
                    var tableObject = new ColumnTableSummary()
                    {
                        DatabaseName=rawTableObject.DatabaseName,
                        TableName = rawTableObject.TableName,
                        FieldName = rawTableObject.FieldName,
                        DataType = rawTableObject.DataType,
                        MatchStr = rawTableObject.MatchingStr
                        //RowCount = rawTableObject.LengthSize,
                     };

                    tableList.Add(tableObject);
                }
            }

            return tableList;
        }

        //SQLCM 5.4 End
        public Dictionary<string, DatabaseObject> GetTableDictionary(DatabaseRecord database, ServerRecord server, SqlConnection connection)
        {
            Dictionary<string, DatabaseObject> tableDictionary = null;

            IEnumerable rawTableList = null;

            try
            {
                if (server.IsDeployed && server.IsRunning)
                {
                    var agentManager = GetAgentManagerProxy(connection);
                    rawTableList = agentManager.GetRawTables(server.Instance, database.Name);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("Loading table objects for database '{0}'", database.Name),
                                            ex,
                                            ErrorLog.Severity.Warning);
                rawTableList = null;
            }


            // straight connection to SQL Server
            if (server != null &&
                rawTableList == null)
            {
                var sqlServer = new SqlDirect();
                if (sqlServer.OpenConnection(server.Instance))
                {
                    rawTableList = RawSQL.GetTables(connection, database.Name);
                }
            }

            if (rawTableList != null)
            {
                tableDictionary = new Dictionary<string, DatabaseObject>();

                bool supportsSchemas = CheckSupportsHelper.SupportsSchemas(server);

                foreach (RawTableObject rto in rawTableList)
                {
                    var dbo = new DatabaseObject
                    {
                        Id = rto.id,
                        ObjectId = NO_ID,
                        DatabaseId = database.DbId,
                        ServerId = NO_ID,
                        TableName =  rto.TableName,
                        FullTableName = rto.FullTableName,
                        SchemaName = rto.SchemaName,
                        ObjectType = ObjectType.Table,
                    };

                    string tableNameKey = supportsSchemas
                        ? dbo.FullTableName
                        : dbo.TableName;

                    tableDictionary.Add(tableNameKey, dbo);
                }
            }

            return tableDictionary;
        }

        public int GetCompatibilityLevel(DatabaseRecord database, SqlConnection connection)
        {
            int retVal = NO_ID;
            ServerRecord server = null;

            try
            {
                server = SqlCmRecordReader.GetServerRecord(database.SrvId, connection);

                if (server.IsDeployed && server.IsRunning)
                {
                    var agentManager = GetAgentManagerProxy(connection);
                    retVal = agentManager.GetCompatibilityLevel(server.Instance, database.Name);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            string.Format("Failed to get compatibility level for server id '{0}'.", database.SrvId),
                                            ex,
                                            ErrorLog.Severity.Warning);
                retVal = NO_ID;
            }

            if (server != null &&
                retVal == NO_ID)
            {
                using (var sqlServer = SqlDirect.OpenDirectConnection(server.Instance))
                {
                    if (sqlServer.IsConnected)
                    {
                        try
                        {
                            retVal = RawSQL.GetCompatibilityLevel(connection, database.Name);
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                           string.Format("GetCompatibilityLevel Direct: Instance {0} Database {1}", server.Instance, database.Name),
                                           ex,
                                           ErrorLog.Severity.Error);
                            retVal = NO_ID;
                        }
                        
                    }   
                }

            }

            return retVal;
        }

        public ClrStatus GetClrStatus(ServerRecord server, SqlConnection connection)
        {
            var status = new ClrStatus();
            bool configured = false;
            bool running = false;
            bool failedToGetStatus = false;

            try
            {
                if (server.IsDeployed && server.IsRunning)
                {
                    var agentManager = GetAgentManagerProxy(connection);
                    agentManager.GetCLRStatus(server.Instance, out configured, out running);
                }
                else
                {
                    failedToGetStatus = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            string.Format("Failed to check if clr enabled for server id '{0}'.", server.SrvId),
                                            ex,
                                            ErrorLog.Severity.Warning);

                failedToGetStatus = true;
            }

            if (failedToGetStatus)
            {
                using (var sqlServer = SqlDirect.OpenDirectConnection(server.Instance))
                {
                    if (sqlServer.IsConnected)
                    {
                        try
                        {
                            RawSQL.GetCLRStatus(connection, out configured, out running);
                            failedToGetStatus = false;
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                           string.Format("Get CLR Status Directly:  for server instance '{0}'.", server.Instance),
                                           ex,
                                           ErrorLog.Severity.Error);

                            failedToGetStatus = true;
                        }
                    }
                }                
            }

            if (failedToGetStatus)
            {
                status.StatusMessage = string.Format("Failed to check if CLR enabled for server instance '{0}'.", server.Instance);
                status.Enable = false;
            }
            else
            {
                status.IsConfigured = configured;
                status.Enable = configured;
                status.IsRunning = running;
                SetClrStatusMessage(status, server);
            }

            status.ServerId = server.SrvId;

            return status;
        }

        public ClrStatus GetServerClrStatus(int serverId, SqlConnection connection)
        {
            ServerRecord server = null;

            try
            {
                server = SqlCmRecordReader.GetServerRecord(serverId, connection);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            string.Format("GetServerClrStatus. Failed to read info for server id '{0}'.", serverId),
                                            ex,
                                            ErrorLog.Severity.Warning);
            }

            return GetClrStatus(server, connection);
        }

        public ClrStatus EnableClr(ClrStatus status, SqlConnection connection)
        {
            ClrStatus newStatus = new ClrStatus();
            ServerRecord server = null;
            bool wasException = false;

            try
            {
                server = SqlCmRecordReader.GetServerRecord(status.ServerId, connection);

                if (server.IsDeployed && server.IsRunning)
                {
                    var agentManager = GetAgentManagerProxy(connection);
                    agentManager.EnableCLR(server.Instance, status.Enable);
                }
                else
                {
                    wasException = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(
                    ErrorLog.Level.Verbose, 
                    string.Format("EnableClr mehtod. Agent failed to enable clr  for server id '{0}'.", newStatus.ServerId), 
                    ex, 
                    ErrorLog.Severity.Warning);

                wasException = true;
            }

            if (wasException)
            {
                using (var sqlServer = SqlDirect.OpenDirectConnection(server.Instance))
                {
                    if (sqlServer.IsConnected)
                    {
                        try
                        {
                            RawSQL.EnableCLR(connection, status.Enable);
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                           string.Format("EnableClr mehtod: failed to enable clr  for server id  '{0}'.", newStatus.ServerId),
                                           ex,
                                           ErrorLog.Severity.Error);
                        }
                    }
                }       
            }

            return GetClrStatus(server, connection);
        }

        public List<string> GetColumnList(int databaseId, string tableName, SqlConnection connection, string instanceName = null, string databaseName = null)
        {
            IList columnList = null;
            DatabaseRecord database = null;
            ServerRecord server = null;

            try
            {
                if (instanceName == null)
                {
                database = SqlCmRecordReader.GetDatabaseRecord(databaseId, connection);
                server = SqlCmRecordReader.GetServerRecord(database.SrvId, connection);
                    instanceName = server.Instance;
                    databaseName = database.Name;
                }
                if (server != null && server.IsDeployed && server.IsRunning)
                {
                    var agentManager = GetAgentManagerProxy(connection);
                    columnList = agentManager.GetRawColumns(instanceName, databaseName, tableName);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            string.Format("GetColumnList method failed for table {0}.", tableName),
                                            ex,
                                            ErrorLog.Severity.Warning);
                columnList = null;
            }

            if (columnList == null &&
                databaseName != null &&
                instanceName != null)
            {
                using (var sqlServer = SqlDirect.OpenDirectConnection(instanceName))
                {
                    if (sqlServer.IsConnected)
                    {
                        try
                        {
                            columnList = RawSQL.GetColumns(sqlServer.Connection, databaseName, tableName);
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Default,
                                           string.Format("GetCompatibilityLevel Direct: Instance {0} Database {1}", server.Instance, database.Name),
                                           ex,
                                           ErrorLog.Severity.Error);
                            columnList = null;
                        }
                    }
                }

            }

            var columnStringList = Converter.ConvertReferenceList<RawColumnObject, string>(columnList, col => col.ColumnName);
            return columnStringList;
        }

        public bool IsServerUp(ServerRecord server, SqlConnection connection)
        {
            var serverManager = GetServerManagerProxy(connection);
            return IsServerUp(serverManager, server);
        }

        public bool PingAgent(string agentServer, int agentPort)
        {
            try
            {
                var agentCmd = GetAgentCommandProxy(agentServer, agentPort);
                // ping agent
                return agentCmd.Ping();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool PingAgent(ServerRecord server)
        {
            return PingAgent(server.AgentServer, server.AgentPort);
        }

        public bool ActivateAgentViaFirewall(string instance, SQLcomplianceConfiguration config)
        {
            try
            {
                // Make sure the service is started
                string agentServer = GetServerNameFromInstance(instance);
                StartAgentService(agentServer);

                var manager = GetAgentManagerProxy(config);
                manager.Activate(instance);
                return true;
            }
            catch (Exception ex)
            {
                LogError(string.Format(TranslateRemotingException(config.Server, CollectionServiceName, ex)));
                return false;
            }
        }

        public bool DeactivateAgent(ServerRecord server, bool removeEventsDatabase, SqlConnection connection)
        {
            bool bRemoved;

            try
            {
                var serverManager = GetServerManagerProxy(connection);
                var serverUp = IsServerUp(serverManager, server);

                if (serverUp)
                {
                    bRemoved = DeactivateAgentViaFirewall(server, removeEventsDatabase, connection);
                }
                else
                {
                    bRemoved = DeactivateAgentDirectly(server, removeEventsDatabase);
                }

                // stop trace jobs
                if (serverUp)
                {
                    serverManager.StopTraceJobs(server.Instance);
                }

                MarkServerInActive(server);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(
                    ErrorLog.Level.Default,
                    string.Format(Error_DeleteServerProblem, server.Instance, ex.Message),
                    ex,
                    ErrorLog.Severity.Error);
                bRemoved = false;
            }

            return bRemoved;
        }

        public bool InstallAgent(ServerRecord server, AgentDeploymentProperties agentProperties, SQLcomplianceConfiguration config)
        {
            try
            {
                AgentControl.RemoteInstallAgentService(
                    null, //m_localAdmin,
                    null, //m_localPassword,
                    server.InstanceServer,
                    null, //m_remoteAdmin,
                    null, //m_remotePassword,
                    agentProperties.AgentServiceCredentials.Account, //m_serviceAccount,
                    agentProperties.AgentServiceCredentials.Password, //m_servicePassword,
                    server.Instance,
                    server.AgentTraceDirectory,
                    config.Server);
                return true;
            }
            catch (Exception ex)
            {
                LogError(
                    string.Format(
                        "The SQLcompliance Agent on {0} could not be instaLLED for the following reason:\n\nError: {1}",
                        server.InstanceServer, ex));
                return false;
            }
        }

        public bool UninstallAgent(ServerRecord server, SqlConnection connection)
        {
            bool bRemoved = false;

            try
            {
                //-----------------------------------------
                // Check for cases where we skip uninstall
                //-----------------------------------------
                if (server.CountSharedInstances(connection) != 0)
                {
                    return true;
                }

                if (server.IsDeployedManually)
                {

                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                       string.Format("The SQL Server Version is {0} for SQL Server {1}", server.Connection.ServerVersion, server.Instance),
                                       ErrorLog.Severity.Informational);
                    return true;
                }

                if (!server.IsDeployed ||       // not deployed; dont undeploy
                    server.IsOnRepositoryHost)  // leave agent on repository at all times
                {
                    return true; 
                }

                //--------------
                // Do Uninstall
                //--------------
                try
                {
                    RemoteUninstallAgentService(null, null, server.AgentServer, null, null);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(
                        ErrorLog.Level.Always, 
                        Error_CantUninstallAgent, 
                        ex, 
                        ErrorLog.Severity.Informational);
                }

                MarkServerInActive(server);

                bRemoved = true;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(
                    ErrorLog.Level.Always,
                    String.Format(Error_UninstallAgent, server.Instance, ex.Message),
                    ex,
                    ErrorLog.Severity.Informational);
            }
            finally
            {
                if (bRemoved)
                {
                    // agent deployed - update all instances on that computer
                    ICollection servers = ServerRecord.GetServers(connection, true);
                    foreach (ServerRecord serverRecord in servers)
                    {
                        if (serverRecord.InstanceServer.ToUpper() == server.InstanceServer.ToUpper())
                        {
                            ServerRecord oldServerState = serverRecord.Clone();
                            if (oldServerState.IsDeployed)
                            {
                                serverRecord.IsRunning = false;
                                serverRecord.IsDeployed = false;
                                serverRecord.Write(oldServerState);

                                AgentStatusMsg.LogStatus(serverRecord.AgentServer, serverRecord.Instance, AgentStatusMsg.MsgType.Unregistered, connection);
                            }
                        }
                    }
                }
            }
            return bRemoved;
        }

        public string UpdateAuditConfiguration(int serverId, SqlConnection connection)
        {
            var server = SqlCmRecordReader.GetServerRecord(serverId, connection);
            bool canAudtitSettingsBeUpdated = server.ConfigVersion > server.LastKnownConfigVersion;

            if (!canAudtitSettingsBeUpdated)
            {
                var warningMessage = string.Format(AgentConfigurationUpdatingWarning, server.LastKnownConfigVersion, server.ConfigVersion);
                LogWarning(warningMessage);
                return Status_Requested;
            }

            try
            {
                var agentManager = GetAgentManagerProxy(connection);
                agentManager.UpdateAuditConfiguration(server.Instance);
                return Status_Requested;
            }
            catch (Exception ex)
            {
                var loggedMessage = LogCollectionServerError(string.Format(Error_UpdateNowFailed, server.Instance), ex, connection);
                throw new Exception(loggedMessage, ex);
            }
        }

        public void UpdateAuditSettingsForAuditUsers(SqlConnection connection)
        {
            using (connection)
            {
                SQLcomplianceConfiguration configuration = SqlCmConfigurationHelper.GetConfiguration(connection);

                try
                {
                    AgentManager manager = CoreRemoteObjectsProvider.AgentManager(configuration.Server, configuration.ServerPort);
                    manager.UpdateAuditUsers(connection.DataSource);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             String.Format(ex.ToString()),
                                             ex,
                                             ErrorLog.Severity.Warning);
                }
            }
        }

        public void UpdateAuditConfigurationAsync(int serverId, SqlConnection connection)
        {
            new Task(() =>
            {
                using (connection)
                {
                    try
                    {
                    UpdateAuditConfiguration(serverId, connection);
                }
                    catch (Exception e)
                    {
                        LogError(e.Message);
                    }
                }
            }).Start();
        }

        public AgentProperties GetAgentProperties(int serverId, SqlConnection connection)
        {
            var server = SqlCmRecordReader.GetServerRecord(serverId, connection);
            var properties = new AgentProperties();
            properties.ServerId = serverId;
            SetGeneralProperties(properties, server);
            SetAgentDeploymentSettings(properties, server);
            SetAgentSQLServerList(properties, server, connection);
            SetTraceOptions(properties, server);
            return properties; 
        }

        public UpgradeAgentResponse UpgradeAgent(UpgradeAgentRequest request, SqlConnection connection)
        {
            UpgradeAgentResponse response = null;
            
            try
            {
                var server = SqlCmRecordReader.GetServerRecord(request.ServerId, connection);
                var errorMessage = ValidateAgentServerToBeUpgraded(server);

                if (!string.IsNullOrEmpty(errorMessage))
            {
                    response = new UpgradeAgentResponse();
                    response.ErrorMessage = errorMessage;
                    response.Success = false;
            }
                else
                {
                    if (IsMajourUpgrade(server, request, connection))
                    {
                        response = UpgradeAgentMajor(connection, server, request.Account, request.Password);
                    }
                    else
                    {
                        response = UpgradeAgentMinor(connection, server);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format("Cant upgrade agent because of exception: {0}", ex.ToString()));
                response = new UpgradeAgentResponse();
                response.ErrorMessage = ex.ToString();
                response.Success = false;
            }
            
            return response;
        }

        private bool IsMajourUpgrade(ServerRecord server, UpgradeAgentRequest request, SqlConnection connection)
            {
            if (!string.IsNullOrEmpty(server.AgentVersion))
                {
                    string cmVersion = GetSqlComplianceManagerVersion();
            
                if (ServerRecord.CompareReleaseVersions(server.AgentVersion, cmVersion) != 0)
                    {
                    return true;
                    }
            }

            return false;
        }

        private string GetHostName()
        {
            var hostName = string.Empty;

            try
            {
                hostName = Dns.GetHostName().ToUpper();
            }
            catch (SocketException ex)
            {
                LogWarning(string.Format("Cant get DNS host name because of error: {0}", ex));
            }

            return hostName;
        }

        private string ValidateAgentServerToBeUpgraded(ServerRecord server)
        {
            var errorMessage = string.Empty;

            var instanceServer = string.IsNullOrEmpty(server.InstanceServer) ? string.Empty : server.InstanceServer.ToUpper();
            var hostName = GetHostName();

            if (instanceServer == hostName)
            {
                errorMessage = Info_CantUpgradeLocal;
            }

            else if (server.IsDeployedManually)
                    {
                errorMessage = Info_CantUpgradeManual;
                    }

            else if (!string.IsNullOrEmpty(server.AgentVersion))
            {
                string cmVersion = GetSqlComplianceManagerVersion();

                if (ServerRecord.CompareVersions(server.AgentVersion, cmVersion) > 0)
                    {
                    errorMessage = Info_CantUpgradeAgentNewer;
                    }
                else if (ServerRecord.CompareVersions(server.AgentVersion, cmVersion) == 0)
                {
                    errorMessage = Info_AlreadyUpgraded;
                }
            }
           
            return errorMessage;
        }


        public bool UpdateAgentProperties(AgentProperties agentProperties, SqlConnection connection)
        {
            try
            {
                var oldServer = SqlCmRecordReader.GetServerRecord(agentProperties.ServerId, connection);
                ValidateAgentProperties(agentProperties, oldServer, connection);
                var oldSnapshot = CreateSnapshotString(oldServer);
                var newServer = CreateServerRecord(agentProperties, oldServer, connection);

                //---------------------------------------
                // Write Server Properties if necessary
                //---------------------------------------
                if (!ServerRecord.Match(newServer, oldServer))
                {
                    if (!newServer.Write(oldServer))
                    {
                        throw (new Exception(ServerRecord.GetLastError()));
                    }
                    else
                    {
                        string newSnapshot = CreateSnapshotString(newServer);
                        StringBuilder log = new StringBuilder(1024);
                        log.AppendFormat("Agent Properties Change: Server {0}\r\n", newServer.AgentServer);
                        log.Append("New Settings\r\n");
                        log.Append(newSnapshot);
                        log.Append("\r\nOld Settings\r\n");
                        log.Append(oldSnapshot);
                        LogRecord.WriteLog(connection, LogType.ChangeAgentProperties, newServer.AgentServer, log.ToString());
                    }

                    newServer.CopyAgentSettingsToAll(oldServer);

                    // bump server version number so that agent will synch up	      
                    newServer.ConfigVersion++;
                    ServerRecord.IncrementServerConfigVersion(connection, newServer.InstanceServer);
                }

                return true;
            }
            catch (Exception ex)
            {
                LogAndThrowException(string.Format("{0}. Failed to update agent properties due to error: {1}", Error_ErrorSavingServer, ex.Message), ex);
            }
            return true;
        }

        public static int GetDatabaseCount(AuditedServerStatus status, SQLcomplianceConfiguration configuration, SqlConnection connection)
        {
            IList dbList = null;
            IList sysList = null;

            if (status.IsRunning && status.IsDeployed)
            {
                string url = "";
                try
                {
                    url = String.Format("tcp://{0}:{1}/{2}",
                                         configuration.Server,
                                         configuration.ServerPort,
                                         typeof(AgentManager).Name);
                    AgentManager manager = (AgentManager)Activator.GetObject(
                       typeof(AgentManager),
                       url);

                    dbList = manager.GetRawUserDatabases(status.Instance);
                    sysList = manager.GetRawSystemDatabases(status.Instance);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             String.Format("LoadDatabases: URL: {0} Instance {1}", url, status.Instance),
                                             ex,
                                             ErrorLog.Severity.Warning);
                    dbList = null;
                }
            }

            // try straight connection to SQL Server if agent connection failed
            if (dbList == null)
            {
                try
                {
                    using (SqlConnection conn = AgentHelper.GetConnection(status.Instance))
                    {
                        dbList = RawSQL.GetUserDatabases(conn);
                        sysList = RawSQL.GetSystemDatabases(conn);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                             String.Format("LoadDatabases: Direction to Instance {0}", status.Instance),
                                             ex,
                                             ErrorLog.Severity.Warning);
                    dbList = null;

                }
            }
            if (dbList != null && sysList != null)
            {
                foreach (object o in sysList)
                    dbList.Add(o);
            }
            if (dbList != null)
                return dbList.Count;
            return 0;

        }
        #endregion
    }
}
