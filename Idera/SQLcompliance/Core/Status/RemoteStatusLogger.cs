using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Collector;

namespace Idera.SQLcompliance.Core.Status
{
    /// <summary>
    /// Singleton remote object to handle status events received from agent
    /// Runs in SQLsecure collector service process space.
    /// </summary>
    public class RemoteStatusLogger : System.MarshalByRefObject, IStatusLogger
    {
        #region Constructor

        /// <summary>
        /// Static constructor for singleton object
        /// </summary>
        static RemoteStatusLogger()
        {
        }

        private static Object logStatusLock = new Object();

        #endregion

        #region Instance variables	

        private int sqlVersion;

        #endregion

        #region Delegates

        /*
          /// <summary>
          /// The delegate for handling status messages
          /// </summary>
          private delegate bool StatusLoggerDelegate( AgentStatusMsg statusMsg );
          private delegate bool GetAuditSettingsDelegate( AgentStatusMsg statusMsg );
        */

        #endregion


        #region Public methods

        //------------------------------------------------------------		
        // PingCollectionService
        //
        // Tests whether collection service is available and ready to receive actions.
        // Callers should wrap this call in a try/catch block; an exception will be 
        // thrown if the remotable object cannot be reached.
        //
        // Retval: Always returns true; if an exception is thrown,
        //         the collection service is not available</returns>
        //------------------------------------------------------------		
        public bool PingCollectionService()
        {
            // Test the repository database connection
            OpenStatusDatabase(true);

            // If we got here, then we're ready for action
            return true;
        }

        //------------------------------------------------------------------------
        // Send Status 
        //
        // Handle status event from agent
        //  retval - Does agent need to update its audit settings
        //------------------------------------------------------------------------
        public bool[] SendStatus(AgentStatusMsg statusMsg)
        {
            bool[,] instanceStatus = SendStatusEx(statusMsg);
            bool[] configUpdateNeeded;

            if (instanceStatus != null && instanceStatus.Length > 0)
                configUpdateNeeded = new bool[statusMsg.Config.InstanceStatusList.Length];
            else
                configUpdateNeeded = new bool[0];

            for (int i = 0; i < statusMsg.Config.InstanceStatusList.Length; i++)
            {
                configUpdateNeeded[i] = instanceStatus[i, 0];
            }
            return configUpdateNeeded;
        }

        //------------------------------------------------------------------------
        // Send Status 
        //
        // Handle status event from agent
        //
        //  retval - Does agent need to update its audit settings
        //------------------------------------------------------------------------
        public bool[,] SendStatusEx(AgentStatusMsg statusMsg)
        {
            bool[,] configUpdateNeeded = null;

            if (statusMsg.Config != null && statusMsg.Config.InstanceStatusList != null)
            {
                configUpdateNeeded = new bool[statusMsg.Config.InstanceStatusList.Length, 2];
            }
            SqlConnection dbConn = null;

            try
            {
                // Do we need to queue this up?  (i.e. are too many simultaneous operations running?)
                lock (logStatusLock)
                {
                    // Open the database connection
                    dbConn = OpenStatusDatabase(false);
                }

                // Log the status into the database
                switch (statusMsg.Type)
                {
                    case AgentStatusMsg.MsgType.Heartbeat:
                        HandleHeartbeatStatus(statusMsg, dbConn);
                        break;

                    case AgentStatusMsg.MsgType.Startup:
                        HandleStartupStatus(statusMsg, dbConn);
                        break;

                    case AgentStatusMsg.MsgType.Shutdown:
                        HandleShutdownStatus(statusMsg, dbConn);
                        break;

                    /*
                    case AgentStatusMsg.MsgType.Error:
                       HandleErrorStatus( (AgentErrorMsg)statusMsg, dbConn );
                       break;

                    case AgentStatusMsg.MsgType.Warning:
                       HandleWarningStatus( (AgentErrorMsg)statusMsg, dbConn );
                       break;
                    */

                    case AgentStatusMsg.MsgType.Update:
                        HandleConfigurationUpdate(statusMsg, dbConn);
                        break;

                    case AgentStatusMsg.MsgType.TraceReceived:
                        HandleTraceReceivedStatus(statusMsg, dbConn);
                        break;

                    case AgentStatusMsg.MsgType.TraceAltered:
                    case AgentStatusMsg.MsgType.TraceStopped:
                    case AgentStatusMsg.MsgType.TraceClosed:
                        HandleTraceTamperedMessage(statusMsg, dbConn);
                        break;

                    // TODO: Message types not yet handled
                    // Some of these might be needed
                    case AgentStatusMsg.MsgType.Alert:
                    case AgentStatusMsg.MsgType.Registered:
                    case AgentStatusMsg.MsgType.FirstTimeUpdate:
                    case AgentStatusMsg.MsgType.Undeployed:
                    case AgentStatusMsg.MsgType.Unregistered:
                    case AgentStatusMsg.MsgType.Resume:
                    case AgentStatusMsg.MsgType.Suspend:
                        break;
                }
            }
            catch (Exception e)
            {
                // Need to log this on the collection service side
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e);

                // Throw it back to the agent, so that it can deal with unavailable collection service
                throw e;
            }
            finally
            {
                if (configUpdateNeeded != null && configUpdateNeeded.Length != 0)
                {
                    // check agent config version to see if update needed
                    int currentConfig;
                    int lastKnownVersion;
                    bool updateNeeded = false;
                    string lastInstanceName = null;
                    DateTime lastModifiedTime = DateTime.MinValue;
                    InstanceStatus[] instances = statusMsg.Config.InstanceStatusList;

                    for (int i = 0; i < instances.Length; i++)
                    {
                        configUpdateNeeded[i, 0] = false;
                        // For cluster, you must consider that InstanceStatus.SqlVersion will be +1000
                        configUpdateNeeded[i, 1] = ((instances[i].SqlVersion % 1000) <= sqlVersion);

                        if (!configUpdateNeeded[i, 1]) // audited instance has higher version number than the repository server's
                        {
                            if (statusMsg.Type == AgentStatusMsg.MsgType.Startup)
                            {
                                // log the agent version problem.
                                AgentStatusMsg.LogStatus(instances[i], AgentStatusMsg.MsgType.UnsupportedSQLVerion, dbConn);
                            }
                            //continue; // no need to update configurations
                        }
                        currentConfig = GetConfigVersion(instances[i].Instance,
                                                         out lastKnownVersion,
                                                         out lastModifiedTime,
                                                         dbConn);

                        if (currentConfig != instances[i].ConfigVersion || lastKnownVersion != instances[i].ConfigVersion)
                        {
                            // Update the last known version for the instances
                            UpdateLastKnownConfigVersion(statusMsg, dbConn, instances[i].ConfigVersion, instances[i].Instance);
                        }

                        if (currentConfig > instances[i].ConfigVersion ||
                           lastModifiedTime > instances[i].LastModifiedTime)
                        {
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                    String.Format(CoreConstants.Debug_ConfigUpdateNeeded,
                                                    instances[i].Instance,
                                                    instances[i].ConfigVersion,
                                                    currentConfig));

                            // dont udpate lastKnownConfigVersion until agent comes back and gets audit settings
                            configUpdateNeeded[i, 0] = true;
                            updateNeeded = true;
                            lastInstanceName = instances[i].Instance;
                        }
                    }

                    if (updateNeeded && lastInstanceName != null)
                        CheckAgentSettings(statusMsg.Config, lastInstanceName, dbConn);

                    // Close the database connection if we opened it
                    if (dbConn != null)
                    {
                        dbConn.Dispose();
                    }
                }
            }
            return configUpdateNeeded;
        }

        //------------------------------------------------------------------------
        // GetAuditSettings - call from agent to get updated copy of audit settings
        //
        //------------------------------------------------------------------------
        public bool GetAuditSettings(AgentStatusMsg statusMsg)
        {
            bool retval = true;

            try
            {
                SqlConnection dbConn = null;
                dbConn = OpenStatusDatabase(false);
                int currentConfig;
                int lastKnownVersion;
                DateTime lastModifiedTime;
                InstanceStatus[] instances = statusMsg.Config.InstanceStatusList;

                foreach (InstanceStatus instance in instances)
                {
                    currentConfig = GetConfigVersion(instance.Instance,
                                                       out lastKnownVersion,
                                                       out lastModifiedTime,
                                                       dbConn);
                    if (currentConfig > 0)
                    {

                        // Update agent version now
                        UpdateLastKnownConfigVersion(statusMsg,
                                                      dbConn,
                                                      currentConfig,
                                                      instance.Instance);
                    }
                }
            }
            catch (Exception ex)
            {
                // Need to log this on the collection service side
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, ex);

                // Throw it back to the agent, so that it can deal with unavailable collection service
                throw ex;
            }
            return retval;
        }

        #endregion

        #region Private methods

        //------------------------------------------------------------		
        // OpenStatusDatabase 
        //------------------------------------------------------------		
        private SqlConnection OpenStatusDatabase(bool test)
        {
            SqlConnection dbConn = null;

            try
            {
                string strConn = String.Format("server={0};database={1};integrated security=SSPI;Connect Timeout=30;Application Name='{2}';",
                                                CollectionServer.ServerInstance,
                                                CoreConstants.RepositoryDatabase,
                                                CoreConstants.DefaultSqlApplicationName);
                dbConn = new SqlConnection(strConn);
                dbConn.Open();
                string version = dbConn.ServerVersion;
                sqlVersion = int.Parse(version.Substring(0, version.IndexOf('.')));

            }
            catch (Exception e)
            {
                // Database is not available
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format(CoreConstants.Exception_RepositoryNotAvailable,
                                                        e.Message),
                                         ErrorLog.Severity.Error);
                throw e;
            }
            finally
            {
                if (test)
                {
                    dbConn.Dispose();
                }
            }
            return dbConn;
        }

        //------------------------------------------------------------		
        // HandleHeartbeatStatus 
        //------------------------------------------------------------		
        private void HandleHeartbeatStatus(AgentStatusMsg statusMsg,
                                          SqlConnection dbConn)
        {
            // TODO: Validate heartbeat flags
            // TODO: Handle first heartbeat case
            // TODO: Hearbeat: Check following versions: audit settings, agent settings, agent binary

            InstanceStatus[] instances = statusMsg.Config.InstanceStatusList;
            bool isEnabled = true;
            bool isDeployed = false;
            int agentPort = -1;
            int idx = 0;

            foreach (InstanceStatus instance in instances)
            {
                if (InstanceExists(instance.Instance, out isEnabled, out isDeployed, dbConn, out agentPort))
                {
                    // log startup of instance
                    if (CollectionServer.activityLogLevel != 0)
                    {
                        AgentStatusMsg.LogStatus(statusMsg.Config.Server,
                                                  instance.Instance,
                                                  AgentStatusMsg.MsgType.Heartbeat,
                                                  dbConn);
                    }

                    // Handle manual deployment
                    if (!isDeployed)
                    {
                        HandleManualDeploymentUpdate(instance, dbConn);
                    }

                    // Update the status of the agent
                    string cmdStr = "not initialized";
                    try
                    {
                        /*
                        if( instance.IsClustered )
                        {
                           cmdStr = CreateClusteredHeartbeatUpdateCmd( statusMsg, idx );
                           if( agentPort != -1 && agentPort != statusMsg.Config.AgentPort )
                           {
                              // Warning message
                              string msg = String.Format( CoreConstants.Alert_UnmatechedClusteredAgentPort,
                                                         agentPort,
                                                         SQLHelpers.CreateSafeDatabaseName( instance.Instance ),
                                                         statusMsg.Config.AgentPort );
                                 ErrorLog.Instance.Write( msg,
                                                         ErrorLog.Severity.Warning );
                           }
                        }
                        else*/
                        cmdStr = CreateHeartbeatUpdateCmd(statusMsg, idx);

                        using (SqlCommand cmd = new SqlCommand(cmdStr, dbConn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(CoreConstants.Exception_ErrorWritingAgentLogRecord,
                                                 cmdStr,
                                                 ex,
                                                 ErrorLog.Severity.Error);
                    }
                }
                else
                {
                    if (CollectionServer.activityLogLevel != 0)
                    {
                        AgentStatusMsg.LogStatus(statusMsg.Config.Server,
                                                instance.Instance,
                                                AgentStatusMsg.MsgType.UnknownInstance,
                                                dbConn);
                    }
                }
                idx++;
            }
        }

        //------------------------------------------------------------		
        // HandleStartupStatus 
        //------------------------------------------------------------		
        private void HandleStartupStatus(AgentStatusMsg statusMsg,
                                         SqlConnection dbConn)
        {
            // Case - Fully registered agent that has been restarted
            // Case - Agent first time startup and so it needs rules
            // Case - Manually installed agent that has ever been registered

            InstanceStatus[] instances = statusMsg.Config.InstanceStatusList;
            bool isEnabled = true;
            bool isDeployed = false;
            int agentPort = -1;
            int idx = 0;
            foreach (InstanceStatus instance in instances)
            {
                if (InstanceExists(instance.Instance, out isEnabled, out isDeployed, dbConn, out agentPort))
                {

                    // log startup of instance
                    AgentStatusMsg.LogStatus(statusMsg.Config.Server,
                                              instance.Instance,
                                              AgentStatusMsg.MsgType.Startup,
                                              dbConn,
                                              statusMsg.StatusTime);

                    // Handle manual deployment
                    if (!isDeployed)
                    {
                        HandleManualDeploymentUpdate(instance, dbConn);
                    }

                    if (statusMsg.cached) // don't update status for cached messages
                        continue;
                    // Update the status of the agent
                    string cmdStr = "not initialized";
                    try
                    {
                        /*
                        if( instance.IsClustered )
                        {
                           cmdStr = CreateClusteredStartupUpdateCmd( statusMsg, idx );
                           if( agentPort != -1 && agentPort != statusMsg.Config.AgentPort )
                           {
                              // Warning message
                              string msg = String.Format( CoreConstants.Alert_UnmatechedClusteredAgentPort,
                                                         agentPort,
                                                         SQLHelpers.CreateSafeDatabaseName( instance.Instance ),
                                                         statusMsg.Config.AgentPort );
                                 ErrorLog.Instance.Write( msg,
                                                         ErrorLog.Severity.Warning );
                           }
                        }
                        else*/
                        cmdStr = CreateStartupUpdateCmd(statusMsg, idx);
                        using (SqlCommand cmd = new SqlCommand(cmdStr, dbConn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(CoreConstants.Exception_ErrorWritingAgentLogRecord,
                                                 cmdStr,
                                                 ex,
                                                 ErrorLog.Severity.Error);
                    }
                }
                else
                {
                    AgentStatusMsg.LogStatus(statusMsg.Config.Server,
                                            instance.Instance,
                                            AgentStatusMsg.MsgType.UnknownInstance,
                                            dbConn,
                                            statusMsg.StatusTime);
                }
                idx++;
            }
        }

        //------------------------------------------------------------		
        // HandleShutdownStatus 
        //------------------------------------------------------------		
        private void HandleShutdownStatus(AgentStatusMsg statusMsg,
                                          SqlConnection dbConn)
        {
            InstanceStatus[] instances = statusMsg.Config.InstanceStatusList;
            bool isEnabled = true;
            bool isDeployed = false;
            int idx = 0;
            int agentPort = -1;

            foreach (InstanceStatus instance in instances)
            {
                if (InstanceExists(instance.Instance, out isEnabled, out isDeployed, dbConn, out agentPort))
                {
                    // log startup of instance
                    AgentStatusMsg.LogStatus(statusMsg.Config.Server,
                                              instance.Instance,
                                              AgentStatusMsg.MsgType.Shutdown,
                                              dbConn,
                                              statusMsg.StatusTime);

                    if (statusMsg.cached)
                        continue;

                    // Update the status of the agent
                    try
                    {
                        string cmdStr = CreateShutdownUpdateCmd(statusMsg, idx);
                        using (SqlCommand cmd = new SqlCommand(cmdStr, dbConn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(CoreConstants.Exception_ErrorWritingAgentLogRecord,
                                                 ex,
                                                 ErrorLog.Severity.Error);
                    }
                }
                else
                {
                    AgentStatusMsg.LogStatus(statusMsg.Config.Server,
                                            instance.Instance,
                                            AgentStatusMsg.MsgType.UnknownInstance,
                                            dbConn);
                }
                idx++;
            }
        }

        /*
        //------------------------------------------------------------		
        // HandleErrorStatus 
        //------------------------------------------------------------		
        private void
           HandleErrorStatus(
              AgentErrorMsg     statusMsg,
              SqlConnection     dbConn
           )
        {
           // Add Agent (if necessary)

           AgentStatusMsg.LogStatus( statusMsg, dbConn );

           // Update Status

           // Alerting logic - send email; flag alert for alert views etc
        }

        //------------------------------------------------------------		
        // HandleWarningStatus 
        //------------------------------------------------------------		
        private void
           HandleWarningStatus(
              AgentErrorMsg     statusMsg,
              SqlConnection     dbConn
           )
        {
           AgentStatusMsg.LogStatus(  statusMsg,
                                      dbConn );
        }
        */

        private void HandleConfigurationUpdate(AgentStatusMsg statusMsg,
                                               SqlConnection conn)
        {
            AgentStatusMsg.LogStatus(statusMsg.Config.AgentServer,
                                       statusMsg.Status.Instance,
                                       AgentStatusMsg.MsgType.Update,
                                       conn,
                                       statusMsg.Status.LastUpdateTime);

            if (statusMsg.cached)
            {
                if (!NeedConfigUpdate(statusMsg.Status.Instance,
                                        statusMsg.Status.ConfigVersion,
                                        statusMsg.Status.LastUpdateTime,
                                        conn))
                    return;
            }
            SetLastConfigUpdate(statusMsg.Status.Instance,
                                 statusMsg.Status.ConfigVersion,
                                 statusMsg.Status.LastUpdateTime,
                                 conn);
        }

        private bool NeedConfigUpdate(string instance, int version, DateTime updateTime, SqlConnection conn)
        {
            int lastKnownVersion = -1;
            DateTime lastTime = DateTime.MinValue;

            GetConfigVersion(instance, out lastKnownVersion, out lastTime, conn);
            return (lastKnownVersion < version || lastTime < updateTime);
        }

        private void HandleTraceReceivedStatus(AgentStatusMsg statusMsg,
                                                SqlConnection conn)
        {
            if (CollectionServer.activityLogLevel != 0)
            {
                AgentStatusMsg.LogStatus(statusMsg.Status, AgentStatusMsg.MsgType.TraceReceived, conn);
            }
            SetLastTraceCollected(statusMsg.Status.Instance, conn);
        }

        private void HandleManualDeploymentUpdate(InstanceStatus status, SqlConnection conn)
        {
            string cmd = CreateDeploymentUpdateCmd(status.Instance);
            using (SqlCommand command = new SqlCommand(cmd, conn))
            {
                command.ExecuteNonQuery();
            }
            AgentStatusMsg.LogStatus(status,
                                      AgentStatusMsg.MsgType.ManuallyDeployed,
                                      conn);
        }

        private void HandleTraceTamperedMessage(AgentStatusMsg statusMsg,
                                                SqlConnection conn)
        {
            AgentStatusMsg.LogStatus(statusMsg.Config.AgentServer,
                                       statusMsg.Status.Instance,
                                       statusMsg.Type,
                                       conn,
                                       statusMsg.StatusTime);

        }
        #endregion

        #region System Alerts

        public bool SubmitSystemAlert(SystemAlert alert)
        {
            return SubmitSystemAlert(alert, true);
        }

        public bool SubmitSystemAlert(SystemAlert alert, bool allErrorsResolved)
        {
            if (alert == null)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default, "Unable to store a NULL SystemAlert.", ErrorLog.Severity.Warning);
                return true;
            }
            Repository rep = new Repository();
            rep.OpenConnection();

            try
            {
                return alert.StoreAlert(rep.connection, allErrorsResolved);
            }
            finally
            {
                rep.CloseConnection();
            }
        }

        public void GenerateServerDownStatusAlert(string instance)
        {
            Repository rep = new Repository();
            rep.OpenConnection();

            try
            {

                GenerateAgentAlerts(SystemAlertType.ServerConnectionError, rep.connection, 0, 0, instance);
            }
            finally
            {
                rep.CloseConnection();
            }
        }

        public void GenerateAgentTraceDirectoryAlert(long folderSize, long maxFolderSize, string instance)
        {
            Repository rep = new Repository();
            rep.OpenConnection();

            try
            {
                GenerateAgentAlerts(SystemAlertType.TraceDirectoryError, rep.connection, folderSize, maxFolderSize, instance);
            }
            finally
            {
                rep.CloseConnection();
            }
        }

        private void GenerateAgentAlerts(SystemAlertType alertType,
                                         SqlConnection connection,
                                         long folderSize,
                                         long maxFolderSize,
                                         string instance)
        {
            // If this is not an alert about the trace directory or 
            // connections to the monitored instance, return.  There
            // is no work to do.
            if (alertType != SystemAlertType.ServerConnectionError &&
                alertType != SystemAlertType.TraceDirectoryError)
                return;

            StatusRuleProcessor processor = new StatusRuleProcessor();
            List<StatusAlertRule> rules = null;
            List<StatusAlert> alerts = new List<StatusAlert>();
            int state = 0;
            AlertingConfiguration config = new AlertingConfiguration();

            try
            {
                config.Initialize(CollectionServer.ServerInstance);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write("Unable to Generate the collection server status alerts", e);
                return;
            }
            ActionProcessor actionProcessor = new ActionProcessor(config);

            try
            {
                rules = AlertingDal.SelectStatusAlertRules(connection);

                // No point in proceeding if there are no status alert rules.
                if (rules.Count <= 0)
                    return;

                state = 1;
                alerts = processor.GenerateAlerts(connection, rules, true, ConvertToStatusRuleType(alertType), folderSize, maxFolderSize, instance);
                state = 2;

                if (alerts != null && alerts.Count > 0)
                {
                    ErrorLog.Instance.Write(ErrorLog.Level.Verbose, String.Format("Alert: {0} collection server status alerts generated.", alerts.Count));

                    // Store the alert
                    foreach (StatusAlert alert in alerts)
                    {
                        string s = processor.ExpandMacros(alert.MessageTitle, alert);
                        alert.MessageTitle = s;
                        s = processor.ExpandMacros(alert.MessageBody, alert);
                        alert.MessageBody = s;
                        AlertingDal.InsertAlert(alert, connection);
                    }
                    state = 3;

                    // Prepare and store the actions
                    alerts.Sort(new StatusAlertLevelDescending());
                    state = 4;

                    // Perform the actions
                    actionProcessor.PerformActions(alerts, connection);
                }
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose, "Alert: Agent alert processing finished.");
            }
            catch (Exception e)
            {
                string errorString = "Error";

                switch (state)
                {
                    case 0:
                        errorString = CoreConstants.Exception_StatusAlertingJobError_0;
                        break;
                    case 1:
                        errorString = CoreConstants.Exception_StatusAlertingJobError_1;
                        break;
                    case 2:
                        errorString = CoreConstants.Exception_StatusAlertingJobError_2;
                        break;
                    case 3:
                        errorString = CoreConstants.Exception_StatusAlertingJobError_3;
                        break;
                    case 4:
                        errorString = CoreConstants.Exception_StatusAlertingJobError_4;
                        break;
                }
                ErrorLog.Instance.Write(errorString, e);
            }
        }

        private StatusRuleType ConvertToStatusRuleType(SystemAlertType systemType)
        {
            StatusRuleType statusType;

            switch (systemType)
            {
                case SystemAlertType.TraceDirectoryError:
                    statusType = StatusRuleType.TraceDirFullAgent;
                    break;
                case SystemAlertType.ServerConnectionError:
                    statusType = StatusRuleType.SqlServerDown;
                    break;
                default:
                    statusType = StatusRuleType.TraceDirFullAgent;
                    break;
            }
            return statusType;
        }

        public bool ResetSystemAlertFlags(string instance)
        {
            if (instance == null || instance.Length == 0)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Default, "Unable to reset SystemAlerts: null or empty instance name.", ErrorLog.Severity.Warning);
                return true;
            }

            Repository rep = new Repository();
            rep.OpenConnection();

            try
            {
                return SystemAlert.ResetAgentHealthFlag(rep.connection, instance);
            }
            finally
            {
                rep.CloseConnection();
            }
        }

        #endregion

        #region Config Version get/set routines

        //-------------------------------------------------------------------------------------------------
        // GetConfigVersion
        //-------------------------------------------------------------------------------------------------
        private int GetConfigVersion(string instance,
                                      out int lastKnownVersion,
                                      out DateTime lastModifiedTime,
                                      SqlConnection dbConn)
        {
            int configVersion = 0;
            lastKnownVersion = -1;
            lastModifiedTime = DateTime.MinValue;

            try
            {
                string cmdStr = String.Format("SELECT configVersion, lastKnownConfigVersion, s.timeLastModified, c.timeLastModified FROM {0} s, {2} c where instance={1}",
                                              CoreConstants.RepositoryServerTable,
                                              SQLHelpers.CreateSafeString(instance),
                                              CoreConstants.RepositoryConfigurationTable);
                using (SqlCommand cmd = new SqlCommand(cmdStr, dbConn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                configVersion = reader.GetInt32(0);
                            else
                                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                        String.Format(CoreConstants.Exception_Format_CouldntReadServerConfigVersion,
                                                                       instance),
                                                        ErrorLog.Severity.Error);
                            if (!reader.IsDBNull(1))
                                lastKnownVersion = reader.GetInt32(1);

                            // Get the time from the servers table
                            if (!reader.IsDBNull(2))
                                lastModifiedTime = reader.GetDateTime(2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(String.Format(CoreConstants.Exception_Format_CouldntReadServerRecord, instance, ex),
                                         ErrorLog.Severity.Error);
            }
            return configVersion;
        }

        //-------------------------------------------------------------------------------------------------
        // UpdateLastKnownConfigVersion
        //-------------------------------------------------------------------------------------------------
        private void UpdateLastKnownConfigVersion(AgentStatusMsg statusMsg,
                                                  SqlConnection dbConn,
                                                  int currentConfig,
                                                  string instanceName)
        {
            try
            {
                string cmdStr = String.Format("UPDATE {0} SET lastKnownConfigVersion={1} where instance='{2}'",
                                              CoreConstants.RepositoryServerTable,
                                              currentConfig,
                                              instanceName);
                using (SqlCommand cmd = new SqlCommand(cmdStr, dbConn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(String.Format(CoreConstants.Exception_Format_CouldntUpdateServerRecord,
                                                    instanceName,
                                                        ex.Message),
                                         ErrorLog.Severity.Error);
            }
        }

        //-------------------------------------------------------------------------------------------------
        // SetLastConfigUpdate
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="conn"></param>
        private void SetLastConfigUpdate(string instance,
                                         int updatedVersion,
                                         DateTime lastUpdateTime,
                                         SqlConnection conn)
        {
            try
            {
                string cmdStr = String.Format("UPDATE {0} SET lastConfigUpdate=GETUTCDATE(), lastKnownConfigVersion={1}, isUpdateRequested = 0 where instance={2}",
                                              CoreConstants.RepositoryServerTable,
                                              updatedVersion,
                                              SQLHelpers.CreateSafeString(instance));
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(String.Format(CoreConstants.Exception_CouldntUpdateServerRecord, ex.Message),
                                        instance,
                                        ErrorLog.Severity.Error);
            }
        }

        //-------------------------------------------------------------------------------------------------
        // SetLastTraceCollected
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="conn"></param>
        private void SetLastTraceCollected(string instance, SqlConnection conn)
        {
            try
            {
                string cmdStr = String.Format("UPDATE {0} SET timeLastCollection=GETUTCDATE() where instance={1}",
                   CoreConstants.RepositoryServerTable,
                   SQLHelpers.CreateSafeString(instance));
                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(String.Format(CoreConstants.Exception_CouldntUpdateServerRecord, ex.Message),
                   instance,
                   ErrorLog.Severity.Error);
            }
        }
        #endregion

        #region Database query builders

        //-------------------------------------------------------------------------------------------------
        // InstanceExists
        //-------------------------------------------------------------------------------------------------
        /*
        private bool
           InstanceExists(
              string               instance,
              out bool             isEnabled,
              SqlConnection        dbConn
           )
        {
           bool retval = false;
           isEnabled = true;

           string        selectStr = String.Format( "SELECT srvId, isEnabled from {0} where instance={1}",
                                                     CoreConstants.RepositoryServerTable,
                                                     SQLHelpers.CreateSafeString(instance) ); 
           try
           {
              using ( SqlCommand selectCmd = new SqlCommand( selectStr, dbConn ) )
              {
                 using ( SqlDataReader rdr = selectCmd.ExecuteReader() )
                 {
                    if ( rdr.HasRows )
                    {
                       retval = true;
                       rdr.Read();
                       if( !rdr.IsDBNull(1) )
                          isEnabled = rdr.GetByte(1) == 0 ? false : true;
                    }
                 }
              }
           }
           catch ( Exception ex )
           {
              ErrorLog.Instance.Write( "InstanceExists - read failed", 
                                       selectStr,
                                       ex );
           }
           return retval;
        }
        */

        //-------------------------------------------------------------------------------------------------
        // InstanceExists
        //-------------------------------------------------------------------------------------------------
        private bool InstanceExists(string instance,
                                   out bool isEnabled,
                                   out bool isDeployed,
                                   SqlConnection dbConn,
                                   out int agentPort)
        {
            bool retval = false;
            isDeployed = false;
            isEnabled = true;
            agentPort = -1;

            string selectStr = String.Format("SELECT srvId, isEnabled, isDeployed, agentPort from {0} where instance={1}",
                                                      CoreConstants.RepositoryServerTable,
                                                      SQLHelpers.CreateSafeString(instance));
            try
            {
                using (SqlCommand selectCmd = new SqlCommand(selectStr, dbConn))
                {
                    using (SqlDataReader rdr = selectCmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            retval = true;
                            rdr.Read();
                            if (!rdr.IsDBNull(1))
                                isEnabled = rdr.GetByte(1) == 0 ? false : true;
                            if (!rdr.IsDBNull(2))
                                isDeployed = rdr.GetByte(2) == 0 ? false : true;
                            if (!rdr.IsDBNull(3))
                                agentPort = rdr.GetInt32(3);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("InstanceExists - read failed", selectStr, ex);
            }
            return retval;
        }

        //-------------------------------------------------------------------------------------------------
        // CreateServerRecord - Makes sure a record exists for this server in the RegisteredServer
        //                      table - if it doesnt, one is added
        //-------------------------------------------------------------------------------------------------
        /*
        private void
           CreateServerRecord(
              string               instance,
              SqlConnection        dbConn
           )
        {
            bool bCreateRecord = false;

            try
            {
              // Does record for agent already exist?
              string        selectStr = String.Format( "SELECT srvId from {0} where instance='{1}'",
                                                       CoreConstants.RepositoryServerTable,
                                                       instance ); 
              using ( SqlCommand    selectCmd = new SqlCommand( selectStr, dbConn ) )
              {
                 using ( SqlDataReader rdr = selectCmd.ExecuteReader() )
                 {
                    if ( ! rdr.HasRows ) bCreateRecord = true;
                 }
              }

              if ( bCreateRecord )
              {
                 // Insert new record
                    string insertStr = String.Format( "INSERT INTO {0} (instance) VALUES ({1})",
                                                      CoreConstants.RepositoryServerTable,
                                                      SQLHelpers.CreateSafeString(instance));
                 using ( SqlCommand    insertCmd = new SqlCommand( insertStr, dbConn ) )
                 {
                    insertCmd.ExecuteNonQuery();
                 }
              }
           }
           catch ( Exception e )
           {
                  ErrorLog.Instance.Write( String.Format( CoreConstants.Exception_CouldntCreateServerRecord,
                                                          e.Message ),
                                           ErrorLog.Severity.Error);
           }
        }
        */

        //-------------------------------------------------------------------------------------------------
        // CreateHeartbeatUpdateCmd - Create SQL for updating status record for a registered server
        //-------------------------------------------------------------------------------------------------
        private string CreateHeartbeatUpdateCmd(AgentStatusMsg statusMsg, int instanceIndex)
        {
            StringBuilder builder = new StringBuilder();
            InstanceStatus instance = statusMsg.Config.InstanceStatusList[instanceIndex];

            builder.Append("UPDATE {0} SET ");
            builder.Append("agentVersion = '{1}', ");
            builder.Append("isRunning = {2}, ");
            builder.Append("isCrippled = {3}, ");
            builder.Append("timeLastHeartbeat = GETUTCDATE(), ");
            builder.Append("timeLastAgentContact = GETUTCDATE(), ");
            builder.Append("timeShutdown = NULL, ");
            builder.Append("sqlVersion = '{4}', ");
            builder.Append("agentServiceAccount = {5}, ");
            builder.Append("agentTraceDirectory = {6}, ");
            builder.Append("bias = {7}, ");
            builder.Append("standardBias = {8}, ");
            builder.Append("standardDate = {9}, ");
            builder.Append("daylightBias = {10}, ");
            builder.Append("daylightDate = {11} ");
            builder.Append("where instance = {12}");

            string cmdStr = String.Format(builder.ToString(),
                                           CoreConstants.RepositoryServerTable,
                                           statusMsg.Config.AgentVersion,
                                        Convert.ToInt16(statusMsg.Config.IsRunning),
                                        Convert.ToInt16(statusMsg.Config.IsCrippled),
                                           instance.SqlVersion,
                                        SQLHelpers.CreateSafeString(statusMsg.Config.ServiceAccount),
                                        SQLHelpers.CreateSafeString(statusMsg.Config.TraceDirectory),
                                        statusMsg.Config.Bias,
                                        statusMsg.Config.StandardBias,
                                        SQLHelpers.CreateSafeDateTimeString(statusMsg.Config.StandardDate),
                                        statusMsg.Config.DaylightBias,
                                        SQLHelpers.CreateSafeDateTimeString(statusMsg.Config.DaylightDate),
                                           SQLHelpers.CreateSafeString(instance.Instance));

            return cmdStr;
        }

        //-------------------------------------------------------------------------------------------------
        // CreateStartupUpdateCmd
        //-------------------------------------------------------------------------------------------------
        private string CreateStartupUpdateCmd(AgentStatusMsg statusMsg, int instanceIndex)
        {
            StringBuilder builder = new StringBuilder();
            InstanceStatus instance = statusMsg.Config.InstanceStatusList[instanceIndex];

            builder.Append("UPDATE {0} SET ");
            builder.Append("agentVersion = '{1}', ");
            builder.Append("isRunning = {2}, ");
            builder.Append("isCrippled = {3}, ");
            builder.Append("timeLastHeartbeat = GETUTCDATE(), ");
            builder.Append("timeStartup = GETUTCDATE(), ");
            builder.Append("timeShutdown = NULL, ");
            builder.Append("timeLastAgentContact = GETUTCDATE(), ");
            builder.Append("sqlVersion = '{4}', ");
            builder.Append("agentServiceAccount = {5}, ");
            builder.Append("agentTraceDirectory = {6}, ");
            builder.Append("bias = {7}, ");
            builder.Append("standardBias = {8}, ");
            builder.Append("standardDate = {9}, ");
            builder.Append("daylightBias = {10}, ");
            builder.Append("daylightDate = {11} ");
            builder.Append("where instance = {12}");

            string cmdStr = String.Format(builder.ToString(),
                                           CoreConstants.RepositoryServerTable,
                                           statusMsg.Config.AgentVersion,
                                        Convert.ToInt16(statusMsg.Config.IsRunning),
                                        Convert.ToInt16(statusMsg.Config.IsCrippled),
                                           instance.SqlVersion,
                                        SQLHelpers.CreateSafeString(statusMsg.Config.ServiceAccount),
                                        SQLHelpers.CreateSafeString(statusMsg.Config.TraceDirectory),
                                        statusMsg.Config.Bias,
                                        statusMsg.Config.StandardBias,
                                        SQLHelpers.CreateSafeDateTimeString(statusMsg.Config.StandardDate),
                                        statusMsg.Config.DaylightBias,
                                        SQLHelpers.CreateSafeDateTimeString(statusMsg.Config.DaylightDate),
                                           SQLHelpers.CreateSafeString(instance.Instance));
            return cmdStr;
        }

        //-------------------------------------------------------------------------------------------------
        // CreateShutdownUpdateCmd
        //-------------------------------------------------------------------------------------------------
        private string CreateShutdownUpdateCmd(AgentStatusMsg statusMsg, int instanceIndex)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("UPDATE {0} SET ");
            builder.Append("isRunning = {1}, ");
            builder.Append("isCrippled = {2}, ");
            builder.Append("timeLastHeartbeat = GETUTCDATE(), ");
            builder.Append("timeStartup = NULL, ");
            builder.Append("timeShutdown = GETUTCDATE(), ");
            builder.Append("timeLastAgentContact = GETUTCDATE() ");
            builder.Append("where instance = {3}");

            string cmdStr = String.Format(builder.ToString(),
                                           CoreConstants.RepositoryServerTable,
                                           0,              // isRunning
                                           0,              // isCrippled
                                           SQLHelpers.CreateSafeString(statusMsg.Config.InstanceStatusList[instanceIndex].Instance));
            return cmdStr;
        }

        //-------------------------------------------------------------------------------------------------
        // CreateDeploymentUpdateCmd
        //-------------------------------------------------------------------------------------------------
        private string CreateDeploymentUpdateCmd(string instanceName)
        {
            return String.Format("UPDATE {0} SET isDeployed = 1, isDeployedManually = 1 WHERE instance = {1}",
                                  CoreConstants.RepositoryServerTable,
                                  SQLHelpers.CreateSafeString(instanceName));
        }

        //-------------------------------------------------------------------------------------------------
        // CreateCheckTraceSettingQuery
        //-------------------------------------------------------------------------------------------------
        private static string CreateCheckTraceSettingQuery(string instanceName)
        {
            return String.Format("SELECT agentHeartbeatInterval"
                                    + ", agentCollectionInterval"
                                    + ", agentForceCollectionInterval"
                                    + ", agentMaxTraceSize"
                                    + ", agentMaxFolderSize"
                                    + ", agentMaxUnattendedTime"
                                    + ", agentLogLevel"
                                    + ", agentVersion"
                                    + ", agentDetectionInterval"
                                    + ", agentTraceStartTimeout"
                                    + ", isCompressedFile"
                                    + " from {0} where instance={1}",
                                    CoreConstants.RepositoryServerTable,
                                    SQLHelpers.CreateSafeString(instanceName));
        }


        #endregion

        #region Utilities

        //---------------------------------------------------------------
        // CheckTraceSettings
        //---------------------------------------------------------------
        internal static void CheckAgentSettings(AgentConfiguration config,
                                                  string instanceName,
                                                  SqlConnection conn)

        {
            if (config.classVersion < CoreConstants.SerializationVersion_21)
            {
                CheckAgentSettings(config.HeartbeatInterval,
                                     config.CollectionInterval,
                                     config.ForceCollectionInterval,
                                     config.MaxFolderSize,
                                     config.MaxTraceSize,
                                     config.MaxUnattendedTime,
                                     config.LogLevel,
                                     instanceName,
                                     conn);
            }
            else if (config.classVersion < CoreConstants.SerializationVersion_33)
            {
                CheckAgentSettings(config.HeartbeatInterval,
                                     config.CollectionInterval,
                                     config.ForceCollectionInterval,
                                     config.MaxFolderSize,
                                     config.MaxTraceSize,
                                     config.MaxUnattendedTime,
                                     config.LogLevel,
                                     instanceName,
                                     config.DetectionInterval,
                                     conn);
            }
            else
            {
                CheckAgentSettings(config.HeartbeatInterval,
                                     config.CollectionInterval,
                                     config.ForceCollectionInterval,
                                     config.MaxFolderSize,
                                     config.MaxTraceSize,
                                     config.MaxUnattendedTime,
                                     config.LogLevel,
                                     instanceName,
                                     config.DetectionInterval,
                                     config.TraceStartTimeout,
                                     conn);
            }
        }

        //---------------------------------------------------------------
        // CheckTraceSettings
        //---------------------------------------------------------------
        internal static void CheckAgentSettings(int inHeartbeatInterval,
                                                int inCollectionInterval,
                                                int inForceCollectionInterval,
                                                int inMaxFolderSize,
                                                int inMaxTraceSize,
                                                int inMaxUnattendedTime,
                                                int inLogLevel,
                                                string instanceName,
                                                SqlConnection conn)
        {
            CheckAgentSettings(inHeartbeatInterval, inCollectionInterval, inForceCollectionInterval, inMaxFolderSize,
               inMaxTraceSize, inMaxUnattendedTime, inLogLevel, instanceName, -1, conn);
        }

        //---------------------------------------------------------------
        // CheckAgentSettings
        //---------------------------------------------------------------
        internal static void CheckAgentSettings(int inHeartbeatInterval,
                                                int inCollectionInterval,
                                                int inForceCollectionInterval,
                                                int inMaxFolderSize,
                                                int inMaxTraceSize,
                                                int inMaxUnattendedTime,
                                                int inLogLevel,
                                                string instanceName,
                                                int inDetectionInterval,
                                                SqlConnection conn)
        {
            CheckAgentSettings(inHeartbeatInterval, inCollectionInterval, inForceCollectionInterval, inMaxFolderSize,
               inMaxTraceSize, inMaxUnattendedTime, inLogLevel, instanceName, -1, CoreConstants.Agent_Default_TraceStartTimeout, conn);
        }

        //---------------------------------------------------------------
        // CheckAgentSettings
        //---------------------------------------------------------------
        internal static void CheckAgentSettings(int inHeartbeatInterval,
                                                int inCollectionInterval,
                                                int inForceCollectionInterval,
                                                int inMaxFolderSize,
                                                int inMaxTraceSize,
                                                int inMaxUnattendedTime,
                                                int inLogLevel,
                                                string instanceName,
                                                int inDetectionInterval,
                                                int inTraceStartTimeout,
                                                SqlConnection conn)
        {
            try
            {
                int heartbeatInterval = inHeartbeatInterval;
                int collectionInterval = inCollectionInterval;
                int forceCollectionInterval = inForceCollectionInterval;
                int maxTraceSize = inMaxTraceSize;
                int maxFolderSize = inMaxFolderSize;
                int maxUnattendedTime = inMaxUnattendedTime;
                int logLevel = inLogLevel;
                int detectionInterval = inDetectionInterval;
                int traceStartTimeout = inTraceStartTimeout;
                bool updateNeeded = false;
                bool isCompressedFile = true;
                bool unCompressedFileSupport = false;
                string query = CreateCheckTraceSettingQuery(instanceName);
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {

                            if (!reader.IsDBNull(0))
                                heartbeatInterval = reader.GetInt32(0);
                            if (heartbeatInterval != (inHeartbeatInterval))
                                updateNeeded = true;

                            if (!reader.IsDBNull(1))
                                collectionInterval = reader.GetInt32(1);
                            if (collectionInterval != (inCollectionInterval))
                                updateNeeded = true;

                            if (!reader.IsDBNull(2))
                                forceCollectionInterval = reader.GetInt32(2);
                            if (forceCollectionInterval != (inForceCollectionInterval))
                                updateNeeded = true;

                            if (!reader.IsDBNull(3))
                                maxTraceSize = reader.GetInt32(3);
                            if (maxTraceSize != inMaxTraceSize)
                                updateNeeded = true;

                            if (!reader.IsDBNull(4))
                                maxFolderSize = reader.GetInt32(4);
                            if (maxFolderSize != inMaxFolderSize)
                                updateNeeded = true;

                            if (!reader.IsDBNull(5))
                                maxUnattendedTime = reader.GetInt32(5);
                            if (maxUnattendedTime != inMaxUnattendedTime)
                                updateNeeded = true;

                            if (!reader.IsDBNull(6))
                                logLevel = reader.GetInt32(6);
                            if (logLevel != inLogLevel)
                                updateNeeded = true;

                            if (!reader.IsDBNull(7))
                            {
                                string agentVersion = reader.GetString(7);
                                if (String.Compare(agentVersion, "2.1.0.0") >= 0 && !reader.IsDBNull(8))
                                {
                                    detectionInterval = reader.GetInt32(8);
                                    if (detectionInterval != inDetectionInterval)
                                    {
                                        updateNeeded = true;
                                    }
                                }

                                if (String.Compare(agentVersion, "3.3.0.0") >= 0 && !reader.IsDBNull(9))
                                {
                                    traceStartTimeout = reader.GetInt32(9);
                                    if (traceStartTimeout != inTraceStartTimeout)
                                        updateNeeded = true;
                                }
                            }
                            if (!reader.IsDBNull(7))
                            {
                                string agentVersion = reader.GetString(7);
                                if (String.Compare(agentVersion, "5.8.1.0") >= 0 && !reader.IsDBNull(7))
                                {
                                    isCompressedFile = SQLHelpers.ByteToBool(reader, 10);
                                    updateNeeded = true;
                                    unCompressedFileSupport = true;
                                }
                                else
                                    unCompressedFileSupport = false;
                            }
                        }

                        if (updateNeeded)
                        {
                            if(unCompressedFileSupport)
                            {
                                UpdateAgentSettings(instanceName,
                                                     heartbeatInterval,
                                                     collectionInterval,
                                                     forceCollectionInterval,
                                                     maxTraceSize,
                                                     maxFolderSize,
                                                     maxUnattendedTime,
                                                     logLevel,
                                                     detectionInterval,
                                                     traceStartTimeout,
                                                     isCompressedFile);
                            }
                            // detection interval is added V 2.1
                            else if (traceStartTimeout > 0)
                            {
                                UpdateAgentSettings(instanceName,
                                                     heartbeatInterval,
                                                     collectionInterval,
                                                     forceCollectionInterval,
                                                     maxTraceSize,
                                                     maxFolderSize,
                                                     maxUnattendedTime,
                                                     logLevel,
                                                     detectionInterval,
                                                     traceStartTimeout);
                            }
                            else if (detectionInterval > 0)
                            {
                                UpdateAgentSettings(instanceName,
                                                     heartbeatInterval,
                                                     collectionInterval,
                                                     forceCollectionInterval,
                                                     maxTraceSize,
                                                     maxFolderSize,
                                                     maxUnattendedTime,
                                                     logLevel,
                                                     detectionInterval);
                            }
                            else
                            {
                                UpdateAgentSettings(instanceName,
                                                     heartbeatInterval,
                                                     collectionInterval,
                                                     forceCollectionInterval,
                                                     maxTraceSize,
                                                     maxFolderSize,
                                                     maxUnattendedTime,
                                                     logLevel);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                           "An error occurred checking agent configuration for " + instanceName + ".",
                                           e,
                                           true);
            }
        }

        //---------------------------------------------------------------
        // UpdateAgentSettings : for version prior to 2.1
        //---------------------------------------------------------------
        internal static void UpdateAgentSettings(string instanceName,
                                                  int heartbeatInterval,
                                                  int collectionInterval,
                                                  int forceCollectionInterval,
                                                  int maxTraceSize,
                                                  int maxFolderSize,
                                                  int maxUnattendedTime,
                                                  int logLevel)
        {
            try
            {
                AgentCommand command = AgentManager.GetAgentCommand(instanceName);
                command.SetAgentConfiguration(logLevel,
                                                 heartbeatInterval,
                                                 collectionInterval,
                                                 forceCollectionInterval,
                                                 maxTraceSize,
                                                 maxFolderSize,
                                                 maxUnattendedTime);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred updating agent configuration for " + instanceName + ".",
                                         e,
                                         true);
            }
        }

        //---------------------------------------------------------------
        // UpdateAgentSettings : for version prior to 3.2
        //---------------------------------------------------------------
        internal static void UpdateAgentSettings(string instanceName,
                                                  int heartbeatInterval,
                                                  int collectionInterval,
                                                  int forceCollectionInterval,
                                                  int maxTraceSize,
                                                  int maxFolderSize,
                                                  int maxUnattendedTime,
                                                  int logLevel,
                                                  int detectionInterval
           )
        {
            try
            {
                AgentCommand command = AgentManager.GetAgentCommand(instanceName);
                command.SetAgentConfiguration(logLevel,
                                                 heartbeatInterval,
                                                 collectionInterval,
                                                 forceCollectionInterval,
                                                 maxTraceSize,
                                                 maxFolderSize,
                                                 maxUnattendedTime,
                                                 detectionInterval);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred updating agent configuration for " + instanceName + ".",
                                         e,
                                         true);
            }
        }
		
        internal static void UpdateAgentSettings(string instanceName,
                                                  int heartbeatInterval,
                                                  int collectionInterval,
                                                  int forceCollectionInterval,
                                                  int maxTraceSize,
                                                  int maxFolderSize,
                                                  int maxUnattendedTime,
                                                  int logLevel,
                                                  int detectionInterval,
                                                  int traceStartTimeout)
        {
            try
            {
                AgentCommand command = AgentManager.GetAgentCommand(instanceName);
                command.SetAgentConfiguration(logLevel,
                                                 heartbeatInterval,
                                                 collectionInterval,
                                                 forceCollectionInterval,
                                                 maxTraceSize,
                                                 maxFolderSize,
                                                 maxUnattendedTime,
                                                 detectionInterval,
                                                 traceStartTimeout);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "An error occurred updating agent configuration for " + instanceName + ".", e, true);
            }
        }

        //---------------------------------------------------------------
        // UpdateAgentSettings 
        //---------------------------------------------------------------
        internal static void UpdateAgentSettings(string instanceName,
                                                  int heartbeatInterval,
                                                  int collectionInterval,
                                                  int forceCollectionInterval,
                                                  int maxTraceSize,
                                                  int maxFolderSize,
                                                  int maxUnattendedTime,
                                                  int logLevel,
                                                  int detectionInterval,
                                                  int traceStartTimeout,
                                                  bool isCompressedFile )
        {
            try
            {
                AgentCommand command = AgentManager.GetAgentCommand(instanceName);
                command.SetAgentConfiguration(logLevel,
                                                 heartbeatInterval,
                                                 collectionInterval,
                                                 forceCollectionInterval,
                                                 maxTraceSize,
                                                 maxFolderSize,
                                                 maxUnattendedTime,
                                                 detectionInterval,
                                                 traceStartTimeout,
                                                 isCompressedFile);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "An error occurred updating agent configuration for " + instanceName + ".", e, true);
            }
        }

        #endregion
    }
}
