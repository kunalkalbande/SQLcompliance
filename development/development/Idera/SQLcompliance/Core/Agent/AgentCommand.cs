using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Threading;
using Idera.SQLcompliance.Core.Status;
using Microsoft.Win32;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.AlwaysOn;
using System.Configuration;

namespace Idera.SQLcompliance.Core.Agent
{
    /// <summary>
    /// Summary description for AgentCommand.
    /// </summary>
    public class AgentCommand : MarshalByRefObject
    {
        #region Constructors

        public AgentCommand() { }

        #endregion

        #region Activate/Deactivate

        //---------------------------------------------------------------------------
        // Activate - Registers a new instance with the agent
        //---------------------------------------------------------------------------
        public void
           Activate(
              string instanceName
           )
        {
            Activate(instanceName, null, null);
        }

        //---------------------------------------------------------------------------
        // Activate - Registers a new instance with the agent
        //---------------------------------------------------------------------------
        public void
           Activate(
              string instanceName,
              string server,
              string traceDirectory
           )
        {
            if (instanceName == null)
                return;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     String.Format("Activating auditing for {0}.", instanceName));

            try
            {
                bool alreadyRegistered = false;  // prevents double registration - can happen if they 
                                                 // remove at GUI while agent is down
                                                 // Add registry entry
                RegistryKey rk = null;
                RegistryKey rks = null;
                try
                {
                    rk = Registry.LocalMachine;
                    rks = rk.CreateSubKey(SQLcomplianceAgent.AgentRegistryKey);

                    ArrayList newInstanceList = new ArrayList();
                    try
                    {
                        string[] instances = (string[])rks.GetValue(CoreConstants.Agent_RegVal_Instances);

                        // Create a new instance list
                        for (int i = 0; i < instances.Length; i++)
                        {
                            if (instances[i].ToUpper() == instanceName) alreadyRegistered = true;

                            newInstanceList.Add(instances[i]);
                        }
                    }
                    catch
                    {
                        // if we get here its because there is no INSTANCEs value; thats fine
                    }

                    if (!alreadyRegistered)
                    {
                        if (!SQLcomplianceAgent.ValidSqlServerOSCombo(instanceName))
                        {
                            string s = String.Format("{0}: {1}", instanceName, CoreConstants.Exception_InvalidSQLServerOSCombo);
                            ErrorLog.Instance.Write(s, ErrorLog.Severity.Error);
                            throw new CoreException(s);
                        }
                        newInstanceList.Add(instanceName);

                        // Delete the instance's subkey
                        rks.SetValue(CoreConstants.Agent_RegVal_Instances,
                                    (string[])newInstanceList.ToArray(typeof(string)));
                    }

                    if (server != null)
                        rks.SetValue(CoreConstants.Agent_RegVal_Server, server);

                    if (traceDirectory != null)
                        rks.SetValue(CoreConstants.Agent_RegVal_TraceDirectory, traceDirectory);

                }
                catch (Exception ex)
                {
                    // Failed to add to the registry, cannot continue;
                    throw ex;
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

                if (SQLcomplianceAgent.Instance != null)
                    SQLcomplianceAgent.Instance.ActivateInstance(instanceName);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format("Auditing for {0} activated.", instanceName));

            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write("AgentCommand::Activate",
                                         ex);
                throw ex;
            }
        }

        //---------------------------------------------------------------------------
        // Deactivate - Stops traces, drops stored procedures, updates
        //              registry for specified instance
        //
        //              if last instance - remove main registry key and trace dir
        //---------------------------------------------------------------------------
        public void Deactivate(
              string instanceName,
              bool removeEventsDatabase
           )
        {
            try
            {
                if (instanceName == null)
                    return;

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        String.Format("Deactivating auditing for {0}.", instanceName));

                instanceName = instanceName.ToUpper();

                int activeInstances = 1;
                string instanceAlias = NativeMethods.GetHashCode(instanceName).ToString();

                // Stop tracing, trace collection, drop stored procedures and free SQLInstance if exists
                if (SQLcomplianceAgent.Instance != null)
                {
                    SQLcomplianceAgent.Instance.DeactivateInstance(instanceName, removeEventsDatabase);
                    ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                            String.Format("Deactivating auditing for {0}.", instanceName));

                }
                else
                {
                    try
                    {
                        using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                        {
                            // Stop all traces
                            SPHelper.StopAllTraces(conn);
                            if (SPHelper.DoesSPExist(conn, CoreConstants.Agent_AuditSPNameXE))
                                SPHelper.StopAllTracesXE(conn);
                            if (SPHelper.DoesSPExist(conn, CoreConstants.Agent_AuditLogSPName))
                                SPHelper.StopAllAuditLogs(conn);
                            // Drop all stored procedures
                            SPHelper.DropStartupSP(conn);
                            SPHelper.DropAuditSP(conn);
                            SPHelper.DropAuditSPXE(conn);
                            SPHelper.DropAuditLogSP(conn);
                            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                                    String.Format("Deactivate auditing: auditing deactivated for {0}.", instanceName));

                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write("AgentCommand::Deactivate - Drop stored procedures",
                                                ex);
                    }
                }

                // Delete instance and its subkey from registry
                try
                {
                    RegistryKey rk = null;
                    RegistryKey rks = null;
                    try
                    {
                        rk = Registry.LocalMachine;
                        rks = rk.OpenSubKey(SQLcomplianceAgent.AgentRegistryKey, true);
                        string[] instances = null;

                        try
                        {
                            instances = (string[])rks.GetValue(CoreConstants.Agent_RegVal_Instances);
                        }
                        catch { }
                        ArrayList newInstanceList = new ArrayList();

                        if (instances != null)
                        {
                            // Create a new instance list
                            for (int i = 0; i < instances.Length; i++)
                            {
                                if (instances[i].ToUpper() != instanceName)
                                    newInstanceList.Add(instances[i]);
                            }

                            activeInstances = newInstanceList.Count;

                            if (activeInstances > 0)
                                rks.SetValue(CoreConstants.Agent_RegVal_Instances,
                                   (string[])newInstanceList.ToArray(typeof(string)));
                            else
                                rks.DeleteValue(CoreConstants.Agent_RegVal_Instances, false);
                        }

                        // Delete the instance's subkey
                        rks.DeleteSubKey(instanceAlias, false);

                    }
                    catch (Exception e)
                    {
                        throw e;
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
                catch { }

                // wipe out trace files.
                try
                {
                    string[] files = Directory.GetFiles(SQLcomplianceAgent.GetTraceDirectory(),
                                                           instanceAlias + "*");
                    for (int i = 0; i < files.Length; i++)
                    {
                        try
                        {
                            File.Delete(files[i]);
                        }
                        catch { }
                    }
                }
                catch { }

                //if ( activeInstances <= 0 )
                //{
                //If last instance, wipe out trace directory and registry key
                //   RemoveTraceDirectoryAndRegistry()
                //}
            }
            catch { }
        }

        //---------------------------------------------------------------------------
        // DeactivateAll - Stops all traces, drops stored procedures, deletes trace
        //                 directory, remove registry key and stops the agent.
        //---------------------------------------------------------------------------
        public void DeactivateAll()
        {
            string[] instances = null;
            try
            {
                // Get all instance names
                instances = SQLcomplianceAgent.GetInstances();

                if (instances != null)
                {
                    for (int i = 0; i < instances.Length; i++)
                    {
                        Deactivate(instances[i], true);
                    }
                }

            }
            catch { }
        }

        private void
           RemoveTraceDirectoryAndRegistry()
        {
            // Remove agent trace directory (if its empty)
            try
            {
                Directory.Delete(SQLcomplianceAgent.GetTraceDirectory());
            }
            catch { }

            // Remove agent registry
            try
            {
                RegistryKey rk = null;
                try
                {
                    rk = Registry.LocalMachine;
                    rk.DeleteSubKeyTree(SQLcomplianceAgent.AgentRegistryKey);
                }
                catch { }
                finally
                {
                    if (rk != null)
                        rk.Close();
                }
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // Ping - just used to test from afar
        //-----------------------------------------------------------------
        public bool Ping()
        {
            try
            {
                // We trigger a heartbeat during pings
                ThreadStart starter = SQLcomplianceAgent.Instance.SendHeartbeat;
                Thread t = new Thread(starter);
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception)
            {
            }
            return true;
        }

#if NEWAPIIMPLEMENTED
      //-----------------------------------------------------------------
      // GetVersionInfo
      //-----------------------------------------------------------------
      public void
         GetVersionInfo(
            out int    configVersion,
            out string collectionServer,
            out string agentVersion
         )
      {
         configVersion    = SQLcomplianceAgent.Instance.Configuration.ConfigVersion;
         collectionServer = SQLcomplianceAgent.Instance.Server;
         agentVersion     = SQLcomplianceAgent.Instance.AgentVersion;
      }
#endif

        #endregion

        #region Auditing

        //---------------------------------------------------------------------------
        // UpdateAuditConfiguration
        //---------------------------------------------------------------------------


        public void UpdateAuditUsers(string instanceName)
        {
            try
            {
                SQLcomplianceAgent.Instance.UpdateAuditUsers(instanceName);

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   String.Format("Audit configuration for audit users for {0} updated.", instanceName));
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);
                // Throw again to notify the caller something is wrong
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        public void
           UpdateAuditConfiguration(
           string instanceName)
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               String.Format("UpdateNow command received for {0}.", instanceName));

            try
            {
                ConfigurationHelper.SetConfigurationUpdatedFlag(instanceName);
                SQLcomplianceAgent.Instance.UpdateAuditConfiguration(instanceName);

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   String.Format("Audit configuration for {0} updated.", instanceName));
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);
                // Throw again to notify the caller something is wrong
                throw e;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        public bool
           GetIsRowCountEnabledFlag(
           string instanceName)
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               String.Format("UpdateNow command received for {0}.", instanceName));

            try
            {
                // SQLCM 5.9 Enable/DisableRowCount Feature for  XE
                bool isRowCountEnabled = GetEnableRowCountForDatabaseAuditingValueFromAppConfig();
                return isRowCountEnabled;
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);
                // Throw again to notify the caller something is wrong
                throw e;
            }
        }

        private bool GetEnableRowCountForDatabaseAuditingValueFromAppConfig()
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


        #endregion

        #region Configurations


        public string GetAgentSettings()
        {
            AgentConfiguration configuration = SQLcomplianceAgent.Instance.Configuration;
            string msg;
            msg = String.Format(CoreConstants.Info_AgentSettings,
               configuration.AgentName,
               configuration.AgentVersion,
               configuration.TraceDirectory,
               configuration.Server,
               configuration.ServerPort,
               configuration.AgentPort,
               configuration.LogLevel);

            foreach (InstanceStatus status in configuration.InstanceStatusList)
            {
                msg += String.Format(CoreConstants.Info_InstanceSettings, status.Instance, status.ConfigVersion);
            }
            return msg;
        }

        public string GetServerName()
        {
            try
            {
                return SQLcomplianceAgent.Instance.Configuration.Server;
            }
            catch (Exception)
            {
                return "";
            }
        }



        //-----------------------------------------------------------------
        // SetAgentConfiguration : obsolete since 2.1.  Leave for 
        //                         backward compatibility
        //-----------------------------------------------------------------
        public void
           SetAgentConfiguration(
              int logLevel,
              int heartbeatInterval,
              int collectionInterval,
              int forceCollectionInterval,
              int maxTraceSize,
              int maxFolderSize,
              int maxUnattendedTime
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Setting new agent configuration.");

            try
            {
                SQLcomplianceAgent.Instance.SetAgentConfiguration(logLevel,
                                                                heartbeatInterval,
                                                                collectionInterval,
                                                                forceCollectionInterval,
                                                                maxTraceSize,
                                                                maxFolderSize,
                                                                maxUnattendedTime);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Agent new configuration set.");
            }
            catch { }

        }

        //-----------------------------------------------------------------
        // SetAgentConfiguration : obsolete since 3.2.  Leave for 
        //                         backward compatibility
        //-----------------------------------------------------------------
        public void
           SetAgentConfiguration(
              int logLevel,
              int heartbeatInterval,
              int collectionInterval,
              int forceCollectionInterval,
              int maxTraceSize,
              int maxFolderSize,
              int maxUnattendedTime,
              int detectionInterval
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Setting new agent configuration.");

            try
            {
                SQLcomplianceAgent.Instance.SetAgentConfiguration(logLevel,
                                                                heartbeatInterval,
                                                                collectionInterval,
                                                                forceCollectionInterval,
                                                                maxTraceSize,
                                                                maxFolderSize,
                                                                maxUnattendedTime,
                                                                detectionInterval);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Agent new configuration set.");
            }
            catch { }

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
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Setting new agent configuration.");

            try
            {
                SQLcomplianceAgent.Instance.SetAgentConfiguration(logLevel,
                                                                heartbeatInterval,
                                                                collectionInterval,
                                                                forceCollectionInterval,
                                                                maxTraceSize,
                                                                maxFolderSize,
                                                                maxUnattendedTime,
                                                                detectionInterval,
                                                                traceStartTimeout);                                                               
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Agent new configuration set.");
            }
            catch { }
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
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Setting new agent configuration.");

            try
            {
                SQLcomplianceAgent.Instance.SetAgentConfiguration(logLevel,
                                                                heartbeatInterval,
                                                                collectionInterval,
                                                                forceCollectionInterval,
                                                                maxTraceSize,
                                                                maxFolderSize,
                                                                maxUnattendedTime,
                                                                detectionInterval,
                                                                traceStartTimeout,
                                                                isCompressedFile);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Agent new configuration set.");
            }
            catch { }
        }


        //-----------------------------------------------------------------
        // SetLogLevel
        //-----------------------------------------------------------------
        public void
           SetLogLevel(
              int logLevel
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Setting agent log level.");

            try
            {
                SQLcomplianceAgent.Instance.SetLogLevel((ErrorLog.Level)logLevel);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   "Agent log level set.");
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetHeartbeatInterval
        //-----------------------------------------------------------------
        public void
           SetHeartbeatInterval(
              int interval
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Setting agent heartbeat interval.");
            try
            {
                SQLcomplianceAgent.Instance.SetHeartbeatInterval(interval);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Agent heartbeat interval set.");
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetForceCollectionInterval -- for all instances
        //-----------------------------------------------------------------
        public void
           SetForceCollectionInterval(
              int interval
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Setting agent force collection interval.");
            try
            {
                SQLcomplianceAgent.Instance.SetForceCollectionInterval(interval);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Agent force collection interval set.");
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetForceCollectionInterval -- for a single instance
        //-----------------------------------------------------------------
        public void
           SetForceCollectionInterval(
              string instanceName,
              int interval
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Setting agent force collection interval.");
            try
            {
                SQLcomplianceAgent.Instance.SetForceCollectionInterval(instanceName, interval);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Agent force collection interval set.");
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetCollectionInterval -- for all instances
        //-----------------------------------------------------------------
        public void
           SetCollectionInterval(
              int interval
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Setting agent collection interval.");
            try
            {
                SQLcomplianceAgent.Instance.SetCollectionInterval(interval);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Agent collection interval set.");
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetCollectionInterval -- for single instance
        //-----------------------------------------------------------------
        public void
           SetCollectionInterval(
              string instanceName,
              int interval
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Setting agent collection interval.");
            try
            {
                SQLcomplianceAgent.Instance.SetCollectionInterval(instanceName, interval);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Agent collection interval set.");
            }
            catch { }
        }

        //-----------------------------------------------------------------
        // SetTraceDirectory
        //-----------------------------------------------------------------
        public void
           SetTraceDirectory(
              string traceDirectory
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Setting agent trace directory.");
            SQLcomplianceAgent.Instance.SetTraceDirectory(traceDirectory);
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Agent trace directory set.");
        }

        /// <summary>
        /// Check to see if the supplied directory exists.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>True if the directory is valid and exists, otherwise false</returns>
        public bool DirectoryExists(string directory)
        {
            return SQLcomplianceAgent.Instance.DirectoryExists(directory);
        }


        //-----------------------------------------------------------------
        // SetMaxTraceSize
        //-----------------------------------------------------------------
        public void
           SetMaxTraceSize(
              int maxTraceSize
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Setting agent maximum trace size.");
            try
            {
                SQLcomplianceAgent.Instance.SetCollectionInterval(maxTraceSize);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "Agent maximum trace size set.");
            }
            catch { }
        }


        #endregion

        #region AlwaysOn Availability Group

        //-----------------------------------------------------------------------
        // GetAVGRoles
        //-----------------------------------------------------------------------
        public IList
           GetAVGRoles(
           string instanceName)
        {
            IList avgList = null;

            ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug,
               "Retreiving the AlwaysOn Availability Group details for Primary and Secondary Roles" + instanceName);

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                {
                    avgList = GetAVGRoles(conn);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred retrieving AlwaysOn Availability Group Details.",
                                         e,
                                         true);
            }

            return avgList;
        }

        #endregion

        #region User List

        //-----------------------------------------------------------------------
        // GetUserList
        //-----------------------------------------------------------------------
        public RemoteUserList
           GetUserList(
           string instanceName)
        {
            RemoteUserList list = new RemoteUserList();

            ErrorLog.Instance.Write(ErrorLog.Level.UltraDebug,
               "Retrieving user name list for instance " + instanceName);

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                {
                    list.ServerRoles = GetServerRoles(conn);
                    list.Logins = GetLogins(conn);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         "An error occurred retrieving user list.",
                                         e,
                                         true);
            }

            return list;
        }

        #endregion

        #region Trace Collection
        public void
           CollectTracesNow(
              string instance
           )
        {
            if (instance == null)
                return;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               String.Format("CollectTraceNow command received for {0}.", instance));

            try
            {
                SQLcomplianceAgent.Instance.CollectTracesNow(instance);

                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   String.Format("Forced trace collection for {0} finished.", instance));
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);
                // Throw again to notify the caller something is wrong
                throw e;
            }
        }
        #endregion


        //SQLCM -541/4876/5259 v5.6
        public IList GetNewDatabasesList(string instanceName, DateTime lastheartbeatTime)
        {
            IList dbList = null;
            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                {
                    dbList = RawSQL.GetNewDatabasesList(conn, instanceName, lastheartbeatTime);
                }
            }
            catch (Exception)
            {

            }

            return dbList;
        }
        //SQLCM -541/4876/5259 v5.6 - END
        #region RawSQL

        //----------------------------------------------------------------------
        // GetRawUserDatabases
        //----------------------------------------------------------------------
        public IList
           GetRawUserDatabases(
              string instanceName
           )
        {
            IList dbList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                {
                    dbList = RawSQL.GetUserDatabases(conn);
                }
            }
            catch (Exception)
            {
                dbList = null;  // return null to indicate an error
            }

            return dbList;
        }

        //----------------------------------------------------------------------
        // GetRawUserDatabases
        //----------------------------------------------------------------------
        public IList
           GetRawUserDatabasesForAlwaysOn(
              string instanceName,
            string dbName
           )
        {
            IList dbList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                {
                    dbList = RawSQL.GetUserDatabasesForAlwaysOn(conn, instanceName);
                }
            }
            catch (Exception)
            {
                dbList = null;  // return null to indicate an error
            }

            return dbList;
        }

        //----------------------------------------------------------------------
        // GetRawAVGDetails
        //----------------------------------------------------------------------
        public IList
           GetRawAVGDetails(
              string instanceName,
              List<string> dbNames
           )
        {
            IList dbList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                {
                    dbList = RawSQL.GetAvailabilityGroupDetails(conn, dbNames);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);
                dbList = null;  // return null to indicate an error
            }

            return dbList;
        }


        //----------------------------------------------------------------------
        // GetSecondaryRoleAllowConnections
        //----------------------------------------------------------------------
        public List<SecondaryRoleDetails>
           GetSecondaryRoleAllowConnections(
              string instanceName
           )
        {
            List<SecondaryRoleDetails> secondaryDetails;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                {
                    secondaryDetails = RawSQL.GetSecondaryRoleAllowConnections(conn, instanceName);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);
                secondaryDetails = null;  // return null to indicate an error
            }

            return secondaryDetails;
        }


        //----------------------------------------------------------------------
        // Get Read Only Secondary Replica Server List
        //----------------------------------------------------------------------
        public List<string> GetReadOnlySecondaryReplicaServerList(string instanceName)
        {
            List<string> dbList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                {
                    dbList = RawSQL.GetReadOnlySecondaryReplicaServerList(conn);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);
                dbList = null;  // return null to indicate an error
            }

            return dbList;
        }

        //----------------------------------------------------------------------
        // Get Read Only Secondary Replica Server List
        //----------------------------------------------------------------------
        public List<ReplicaNodeInfo> GetAllReplicaNodeInfoList(string instanceName)
        {
            List<ReplicaNodeInfo> infoList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                {
                    infoList = RawSQL.GetAllReplicaNodeInfoList(conn);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);
                infoList = null;  // return null to indicate an error
            }

            return infoList;
        }

        //----------------------------------------------------------------------
        // GetRawSystemDatabases
        //----------------------------------------------------------------------
        public IList
           GetRawSystemDatabases(
             string instanceName
           )
        {
            IList dbList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    dbList = RawSQL.GetSystemDatabases(conn);
            }
            catch (Exception)
            {
                dbList = new ArrayList();  // return an empty array
            }

            return dbList;
        }

        //----------------------------------------------------------------------
        // GetRawTables
        //----------------------------------------------------------------------
        public IList
           GetRawTables(
             string instanceName,
              string dbName
           )
        {
            IList tblList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    tblList = RawSQL.GetTables(conn,
                                                   dbName);
            }
            catch (Exception)
            {
                tblList = new ArrayList();  // return an empty array
            }

            return tblList;
        }

        public IList GetRawTables(string instanceName, string dbName, string tableNameSearchText)
        {
            IList tblList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    tblList = RawSQL.GetTables(conn, dbName, tableNameSearchText);
            }
            catch (Exception)
            {
                tblList = new ArrayList();  // return an empty array
            }

            return tblList;
        }
        //SQLCM 5.4 Start

        //----------------------------------------------------------------------
        // GetRawTables
        //----------------------------------------------------------------------
        //public IList
        //   GetRawTableDetails(
        //     string instanceName,
        //      string dbName
        //   )
        //{
        //    IList tblList;

        //    try
        //    {
        //        using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
        //            tblList = RawSQL.GetTableDetails(conn, dbName);
        //    }
        //    catch (Exception)
        //    {
        //        tblList = new ArrayList();  // return an empty array
        //    }

        //    return tblList;
        //}



        //public IList GetRawTableDetail(string instanceName, string dbName, string tableName)
        //{
        //    IList tblList;

        //    try
        //    {
        //        using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
        //            tblList = RawSQL.GetTableDetails(conn, dbName, tableName);
        //    }
        //    catch (Exception)
        //    {
        //        tblList = new ArrayList();  // return an empty array
        //    }

        //    return tblList;
        //}

        public IList GetRawTableDetailForAll(string instanceName, IList dbName, string profileName)
        {
            IList tblList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    tblList = RawSQL.GetTableDetailsForAll(conn, conn, dbName, profileName);
            }
            catch (Exception)
            {
                tblList = new ArrayList();  // return an empty array
            }

            return tblList;
        }

        public IList GetRawColumnDetailForAll(string instanceName, IList dbName, string profileName)
        {
            IList tblList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    tblList = RawSQL.GetColumnDetailsForAll(conn, conn, dbName, profileName);
            }
            catch (Exception)
            {
                tblList = new ArrayList();  // return an empty array
            }

            return tblList;
        }

        //SQLCM 5.4 ENd


        public List<string> GetBlobTables(string instanceName, string dbName)
        {
            List<string> tblList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    tblList = RawSQL.GetBlobTables(conn, dbName);
            }
            catch (Exception)
            {
                tblList = new List<string>();  // return an empty array
            }

            return tblList;
        }

        //----------------------------------------------------------------------
        // GetRawColumns
        //----------------------------------------------------------------------
        public IList
           GetRawColumns(
              string instanceName,
              string dbName,
              string tableName
           )
        {
            IList colList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    colList = RawSQL.GetColumns(conn,
                                            dbName,
                                            tableName);
            }
            catch (Exception)
            {
                colList = new ArrayList();  // return an empty array
            }

            return colList;
        }

        //----------------------------------------------------------------------
        // GetRawServerRoles
        //----------------------------------------------------------------------
        public IList
           GetRawServerRoles(
             string instanceName
           )
        {
            IList dbList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    dbList = RawSQL.GetServerRoles(conn);
            }
            catch (Exception)
            {
                dbList = new ArrayList();  // return an empty array
            }

            return dbList;
        }

        //----------------------------------------------------------------------
        // GetRawServerLogins
        //----------------------------------------------------------------------
        public IList
           GetRawServerLogins(
             string instanceName
           )
        {
            IList dbList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    dbList = RawSQL.GetServerLogins(conn);
            }
            catch (Exception)
            {
                dbList = new ArrayList();  // return an empty array
            }

            return dbList;
        }

        //----------------------------------------------------------------------
        // GetRawLogins - get sql objects via agent to get through firewalls
        //----------------------------------------------------------------------
        public IList
           GetRawLogins(
             string instanceName,
              string databaseName
           )
        {
            IList dbList;

            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    dbList = RawSQL.GetLogins(conn,
                                             databaseName);
            }
            catch (Exception)
            {
                dbList = new ArrayList();  // return an empty array
            }

            return dbList;
        }

        #endregion

        #region CLR Enablement

        public void
           GetCLRStatus(
              string instanceName,
           out bool configured,
           out bool running
           )
        {
            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    RawSQL.GetCLRStatus(conn, out configured, out running);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        e,
                                        true);
                throw;

            }

        }

        public bool
           EnableCLR(
              string instanceName
           )
        {
            return EnableCLR(instanceName, true);
        }

        public bool
          EnableCLR(
             string instanceName,
             bool enable
          )
        {
            bool isEnabled = false;
            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                    isEnabled = RawSQL.EnableCLR(conn, enable);
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        e,
                                        true);
                throw;

            }

            return isEnabled;
        }

        public int GetCompatibilityLevel(string instanceName, string dbName)
        {
            try
            {
                using (SqlConnection conn = SQLcomplianceAgent.GetConnection(instanceName))
                {
                    return RawSQL.GetCompatibilityLevel(conn, dbName);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        e,
                                        true);
                throw;

            }
        }

        #endregion


        #region Versions
        /*
        //-----------------------------------------------------------------------
        // GetAgentVersions
        //-----------------------------------------------------------------------
        public AgentVersion
           GetAgentVersions()
        {
           return new AgentVersion( SQLcomplianceAgent.Instance.AgentVersion,
                                    CoreConstants.Agent_InternalVersion );
        }
        */

        #endregion

        #region Private Methods

        //-----------------------------------------------------------------------
        // GetServerRoles
        //-----------------------------------------------------------------------
        private IList
           GetAVGRoles(
              SqlConnection conn)
        {
            List<AlwaysOnAVG> AVGList = new List<AlwaysOnAVG>();
            AlwaysOnAVG alwaysOnAVG;

            using (SqlDataReader reader = GetAVGRoleReader(conn))
            {
                while (reader.Read())
                {
                    alwaysOnAVG = new AlwaysOnAVG();
                    alwaysOnAVG.AvailGroupId = reader.GetString(0);
                    alwaysOnAVG.AvailGroupName = reader.GetString(1);
                    alwaysOnAVG.NodesCount = reader.GetInt32(2);
                    AVGList.Add(alwaysOnAVG);
                }
            }

            return AVGList;
        }

        #region User List
        //-----------------------------------------------------------------------
        // GetServerRoles
        //-----------------------------------------------------------------------
        private ServerRole[]
           GetServerRoles(
              SqlConnection conn)
        {
            ArrayList roleList = new ArrayList();
            ServerRole newRole;

            using (SqlDataReader reader = GetServerRoleReader(conn))
            {
                while (reader.Read())
                {
                    newRole = new ServerRole();
                    newRole.Name = reader.GetString(0);
                    newRole.FullName = reader.GetString(1);
                    newRole.Id = reader.GetInt32(2);
                    roleList.Add(newRole);
                }
            }

            return (ServerRole[])roleList.ToArray(typeof(ServerRole));
        }

        //-----------------------------------------------------------------------
        // GetLogins - Retrieve login names
        //-----------------------------------------------------------------------
        public static Login[]
           GetLogins(
              SqlConnection conn)
        {
            ArrayList loginArray = new ArrayList();
            Login newLogin;
            byte[] newSid = null;

            using (SqlDataReader reader = GetLoginReader(conn))
            {
                while (reader.Read())
                {
                    newLogin = new Login();
                    newLogin.Name = reader.GetString(0);
                    newSid = new byte[85];
                    reader.GetBytes(1, 0, newSid, 0, 85);
                    newLogin.Sid = newSid;
                    loginArray.Add(newLogin);
                }
            }

            return (Login[])loginArray.ToArray(typeof(Login));
        }


        #endregion

        #region Data Helpers

        //-----------------------------------------------------------------------
        // GetServerRoleReader
        //-----------------------------------------------------------------------
        private SqlDataReader
           GetServerRoleReader(
              SqlConnection conn)
        {
            string query = BuildServerRolesQuery();

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                SqlDataReader reader = command.ExecuteReader();
                return reader;
            }
        }

        //-----------------------------------------------------------------------
        // GetLoginReader
        //-----------------------------------------------------------------------
        private static SqlDataReader
           GetLoginReader(
              SqlConnection conn)
        {
            string query = BuildLoginsQuery();

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                SqlDataReader reader = command.ExecuteReader();
                return reader;
            }
        }

        //-----------------------------------------------------------------------
        // GetAVGRoleReader
        //-----------------------------------------------------------------------
        private SqlDataReader
           GetAVGRoleReader(
              SqlConnection conn)
        {
            string query = BuildAVGRoleQuery();

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                SqlDataReader reader = command.ExecuteReader();
                return reader;
            }
        }

        #endregion

        #region SQL Query Builders

        //-----------------------------------------------------------------------
        // BuildLoginsQuery
        //-----------------------------------------------------------------------
        private static string
           BuildLoginsQuery()
        {
            return "SELECT lo.loginname, usu.sid "
               + " from sysusers	usu left outer join "
               + " (sysmembers mem inner join sysusers usg on mem.groupuid = usg.uid) on usu.uid = mem.memberuid "
               + " left outer join master.dbo.syslogins  lo on usu.sid = lo.sid "
               + " where (usu.islogin = 1 and usu.isaliased = 0 and usu.hasdbaccess = 1) and "
               + " (usg.uid is null and loginname is not null) ";

        }

        //-----------------------------------------------------------------------
        // BuildServerRolesQuery
        //-----------------------------------------------------------------------
        private string
           BuildServerRolesQuery()
        {
            return "select 'ServerRole' = v1.name, 'FullName' = v2.name , 'Id' = v1.number "
                     + "from master.dbo.spt_values v1, master.dbo.spt_values v2 "
                     + " where v1.low = 0 and "
                      + " v1.type = 'SRV' and "
                      + " v2.low = -1 and "
                      + " v2.type = 'SRV' and "
                      + " v1.number = v2.number ";

        }

        //-----------------------------------------------------------------------
        // BuildAVGRoleQuery
        //-----------------------------------------------------------------------
        private string
            BuildAVGRoleQuery()
        {
            return "select ag.group_id as AVGId, ag.name as AVGName, count(*) "
                        + "from master.sys.availability_groups as ag "
                        + "join master.sys.dm_hadr_availability_replica_states hars on ag.group_id = hars.group_id "
                        + "group by ag.group_id , ag.name "
                        + "having count(*) > 1 ";
        }
        #endregion

        #endregion

    }

    [Serializable]
    public struct AgentVersion : ISerializable
    {
        internal int structVersion;
        public string ExternalVersion;
        public int InternalVersion;

        public AgentVersion(
           string inExternalVersion,
           int inInternalVersion
           )
        {
            structVersion = CoreConstants.SerializationVersion;
            ExternalVersion = inExternalVersion;
            InternalVersion = inInternalVersion;
        }

        // Deserialization constructor
        public AgentVersion(
           SerializationInfo info,
           StreamingContext context
           )
        {
            try
            {
                structVersion = info.GetInt32("structVersion");
            }
            catch
            {
                // There is no version number prior to V 2.0.
                structVersion = 0;
            }
            ExternalVersion = info.GetString("ExternalVersion");
            InternalVersion = info.GetInt32("InternalVersion");
        }

        // Required custom serialization method
        public void
           GetObjectData(
              SerializationInfo info,
              StreamingContext context
           )
        {
            try
            {
                info.AddValue("structVersion", structVersion);
                info.AddValue("ExternalVersion", ExternalVersion);
                info.AddValue("InternalVersion", InternalVersion);
            }
            catch (Exception e)
            {
                SerializationHelper.ThrowSerializationException(e, this.GetType());
            }

        }

    }
}
