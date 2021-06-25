using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;
using System.Net.Sockets;
using Idera.SQLcompliance.Core.Remoting;
using Microsoft.Win32;

using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.TraceProcessing;
using Idera.SQLcompliance.Core.AlwaysOn;
using Idera.SQLcompliance.Core.Event;

namespace Idera.SQLcompliance.Core.Collector
{
    /// <summary>
    /// Summary description for AgentManager.
    /// </summary>
    public class AgentManager : MarshalByRefObject
    {
        #region Constructors and Properties

        //SQLCM -541/4876/5259 v5.6
        static object _lockObject = null;
		static private SQLcomplianceConfiguration SQLcomplianceConfig = new SQLcomplianceConfiguration();

		//------------------------------------------------------------		
		//------------------------------------------------------------		
		/// <summary>
		/// 
		/// </summary>
		public AgentManager()
        {
            //SQLCM -541/4876/5259 v5.6s
            _lockObject = new object();
        }

        #endregion

        //
        // This method just returns true.  It is used to validate
        //  that a remoting object has been successfully obtained.
        public bool Ping()
        {
            return true;
        }

        //
        // This method actually pings the agent and validates that the agent
        //  is responsible for the instanceName provided.
        //
        public bool Ping(string instanceName)
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                return agentCommand.Ping();
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        #region Auditing

        public void UpdateAuditUsers(string instance)
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instance);
                agentCommand.UpdateAuditUsers(instance);
                agentCommand = null;
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, string.Format("Update Audit Users command sent to {0}.", instance));
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Always,
                    e,
                    true);

                throw TranslateRemotingException(GetInstanceServer(instance), e);
            }
        }

        //------------------------------------------------------------		
        // UpdateAuditConfiguration
        //------------------------------------------------------------		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        public void
           UpdateAuditConfiguration(
              string instanceName,
              string agentVersion = null
           )
        {
            Repository rep = null;
            bool isRowCountEnabledFlag = false;

            Version _agentVersion = null;
            Version desiredVersion = null;

            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                     "Sending UpdateNow command to " + instanceName + ".");
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                agentCommand.UpdateAuditConfiguration(instanceName);

                if (agentVersion != null)
                {
                    _agentVersion = new Version(agentVersion);
                    desiredVersion = new Version("5.9");
                    if (_agentVersion.CompareTo(desiredVersion) >= 0)
                        isRowCountEnabledFlag = agentCommand.GetIsRowCountEnabledFlag(instanceName); // SQLCM5.9
                }

                agentCommand = null;
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                               "UpdateNow command sent to " + instanceName + ".");
                
                ServerRecord.UpdateLastAgentContact(instanceName);
                if (agentVersion != null)
                {
                    if (_agentVersion.CompareTo(desiredVersion) >= 0)
                        ServerRecord.UpdateIsRowCountEnabledFlag(instanceName, isRowCountEnabledFlag); // SQLCM5.9
                }
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }

            try
            {
                rep = new Repository();
                rep.OpenConnection();

                RemoteStatusLogger.CheckAgentSettings(-1,
                                                       -1,
                                                       -1,
                                                       CoreConstants.Agent_Default_MaxFolderSize,
                                                       CoreConstants.Agent_Default_MaxTraceSize,
                                                       CoreConstants.Agent_Default_MaxUnattendedTime,
                                                       -1,
                                                       instanceName,
                                                       rep.connection);
                ServerRecord server = ServerRecord.GetServer(rep.connection, instanceName);
                UpdateInstanceDatabasesTable(rep.connection, server);
                //SQLCM -541/4876/5259 v5.6
                if (server.AddNewDatabasesAutomatically)
                {
                    AutomaticallyAuditNewDatabases(rep.connection, server);
                }
                //SQLCM -541/4876/5259 v5.6 - END
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
                if (rep != null)
                    rep.CloseConnection();
                rep = null;
            }
        }

        //--------------------------------------------------------------------
        // Activate - registers an instance with an agent
        //
        // create registry entries, trace directory (as needed), call home
        // to start auditing etc.
        //--------------------------------------------------------------------		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        public void
           Activate(
              string instanceName
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Activating new instance at agent");
            try
            {
                GetAgentCommand(instanceName).Activate(instanceName);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //--------------------------------------------------------------------
        // Activate - registers an instance with an agent
        //
        // create registry entries, trace directory (as needed), call home
        // to start auditing etc.
        //--------------------------------------------------------------------		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="server"></param>
        /// <param name="traceDirecotry"></param>
        public void
           Activate(
              string instanceName,
              string server,
              string traceDirectory
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Activating new instance at agent");
            try
            {
                GetAgentCommand(instanceName).Activate(instanceName,
                                                          server,
                                                          traceDirectory);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);

                throw TranslateRemotingException(server, e);
            }
        }

        //--------------------------------------------------------------------
        // Deactivate - unregisters an instance with an agent
        //              causes agent to clean up traces, trace files,
        //              stored procedures
        //              if last activated instance
        //                delete tracedirectory, registry key
        //--------------------------------------------------------------------		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        public void
           Deactivate(
              string instanceName,
              bool removeEventsDatabase
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Deactivating instance at agent");
            try
            {
                GetAgentCommand(instanceName).Deactivate(instanceName, removeEventsDatabase);

                // if deleting events database; just abort all the trace jobs
                // for this instance
                if (removeEventsDatabase)
                {
                    TraceJobInfo.SetAbortingForInstanceJobs(instanceName);
                }

                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //--------------------------------------------------------------------
        // DeactivateAll - undregister all auditing at agent
        //              causes agent to clean up traces, trace files,
        //              stored procedures, delete tracedirectory
        //
        // Note: Have to pass in at least one instanceName so we can find
        //       the agent for the machine
        //--------------------------------------------------------------------		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agentServer"></param>
        public void
           DeactivateAll(
              string instanceName
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Deactivating all instance registered at agent");
            try
            {
                GetAgentCommand(instanceName).DeactivateAll();
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //--------------------------------------------------------------------
        // CollectTracesNow - Force the instance to recreate the audit stored
        //                    procedures, restart the traces and collect the
        //                    current traces.
        //--------------------------------------------------------------------		
        public void
           CollectTracesNow(
              string instanceName
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                    "Force trace collection now...");
            try
            {
                GetAgentCommand(instanceName).CollectTracesNow(instanceName);
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                        "Force trace collection succeeded.");
            }
            catch (Exception e)
            {
                // Log the error
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        #endregion

        #region Configuration

        //--------------------------------------------------------------------
        // SetTraceDirectory -- Sets a new trace directory for all instances. 
        //--------------------------------------------------------------------
        public void
           SetTraceDirectory(
              string instanceName,
              string traceDirectory
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Sending SetTraceDirectory command to " + instanceName + ".");
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                agentCommand.SetTraceDirectory(traceDirectory);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //--------------------------------------------------------------------
        // DirectoryExists() - Check to see if a directory exists on the agent machine
        //--------------------------------------------------------------------
        public bool DirectoryExists(string instanceName, string directory)
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                "Sending DirectoryExists command to " + instanceName + ".");
            bool retVal = false;
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                retVal = agentCommand.DirectoryExists(directory);
                ServerRecord.UpdateLastAgentContact(instanceName);
                return retVal;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                    e,
                    true);
                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }

        }


        //-----------------------------------------------------------------
        // SetMaxTraceSize -- sets new maximum trace file size.  
        //-----------------------------------------------------------------
        public void
           SetMaxTraceSize(
              string instanceName,
              int maxTraceSize
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Sending SetMaxTraceSize command to " + instanceName + ".");
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                agentCommand.SetCollectionInterval(maxTraceSize);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error 
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }


        //-----------------------------------------------------------------
        // SetAgentConfiguration : for versions prior to 2.1
        //-----------------------------------------------------------------
        public void
           SetAgentConfiguration(
              string instanceName,
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
               "Sending SetAgentConfiguration command to " + instanceName + ".");
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                agentCommand.SetAgentConfiguration(logLevel,
                                                    heartbeatInterval,
                                                    collectionInterval,
                                                    forceCollectionInterval,
                                                    maxTraceSize,
                                                    maxFolderSize,
                                                    maxUnattendedTime);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //-----------------------------------------------------------------
        // SetAgentConfiguration : for versions prior to 3.2
        //-----------------------------------------------------------------
        public void
           SetAgentConfiguration(
              string instanceName,
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
               "Sending SetAgentConfiguration command to " + instanceName + ".");
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                agentCommand.SetAgentConfiguration(logLevel,
                                                    heartbeatInterval,
                                                    collectionInterval,
                                                    forceCollectionInterval,
                                                    maxTraceSize,
                                                    maxFolderSize,
                                                    maxUnattendedTime,
                                                    detectionInterval);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //-----------------------------------------------------------------
        // SetAgentConfiguration
        //-----------------------------------------------------------------
        public void SetAgentConfiguration(string instanceName,
                                          int logLevel,
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
            ErrorLog.Instance.Write(ErrorLog.Level.Debug, "Sending SetAgentConfiguration command to " + instanceName + ".");

            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                agentCommand.SetAgentConfiguration(logLevel,
                                                    heartbeatInterval,
                                                    collectionInterval,
                                                    forceCollectionInterval,
                                                    maxTraceSize,
                                                    maxFolderSize,
                                                    maxUnattendedTime,
                                                    detectionInterval,
                                                    traceStartTimeout,
                                                    isCompressedFile);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);
                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //-----------------------------------------------------------------
        // SetLogLevel -- Sets agent's log level.
        //-----------------------------------------------------------------
        public void
           SetLogLevel(
              string instanceName,
              int logLevel
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Sending SetLogLevel command to " + instanceName + ".");
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                agentCommand.SetLogLevel(logLevel);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //-----------------------------------------------------------------
        // SetHeartbeatInterval -- Sets new agent heartbeat interval.
        //-----------------------------------------------------------------
        public void
           SetHeartbeatInterval(
              string instanceName,
              int newInterval
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Sending SetHeartbeatInterval command to " + instanceName + ".");
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                agentCommand.SetHeartbeatInterval(newInterval);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }

        }


        //-----------------------------------------------------------------
        // SetForceCollectionInterval -- sets new force collection interval.
        // Set foalAllInstances to true if it applies to all instances
        // running on the machine.
        //-----------------------------------------------------------------
        public void
           SetForceCollectionInterval(
              string instanceName,
              int interval,
              bool forAllInstances
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Sending SetForceCollectionInterval command to " + instanceName + ".");
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                if (forAllInstances)
                    agentCommand.SetForceCollectionInterval(interval);
                else
                    agentCommand.SetForceCollectionInterval(instanceName, interval);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error 
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //-----------------------------------------------------------------
        // SetCollectionInterval -- sets new trace collection interval.  
        // Set foalAllInstances to true if it applies to all instances
        // running on the machine.
        //-----------------------------------------------------------------
        public void
           SetCollectionInterval(
              string instanceName,
              int interval,
              bool forAllInstances

           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Sending SetCollectionInterval command to " + instanceName + ".");
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                if (forAllInstances)
                    agentCommand.SetCollectionInterval(interval);
                else
                    agentCommand.SetCollectionInterval(instanceName, interval);
                ServerRecord.UpdateLastAgentContact(instanceName);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                   e,
                   true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        #endregion

        #region User List

        //------------------------------------------------------------		
        // GetUserList
        //------------------------------------------------------------		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        public RemoteUserList
           GetUserList(
              string instanceName
           )
        {
            ErrorLog.Instance.Write(ErrorLog.Level.Debug,
               "Sending GetUserList command to " + instanceName + ".");
            RemoteUserList list = new RemoteUserList(); ;
            try
            {
                list = GetAgentCommand(instanceName).GetUserList(instanceName);
            }
            catch (Exception e)
            {
                // Log the error
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
            }
            return list;
        }

        #endregion

        #region RawSQL

        //----------------------------------------------------------------------
        // GetRawUserDatabases - get sql objects via agent to get through firewalls
        //----------------------------------------------------------------------
        public IList
           GetRawUserDatabases(
             string instanceName
           )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                IList retList = agentCommand.GetRawUserDatabases(instanceName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //----------------------------------------------------------------------
        // GetRawUserDatabasesForAlwaysOn - get sql objects via agent to get through firewalls
        //----------------------------------------------------------------------
        public IList
           GetRawUserDatabasesForAlwaysOn(
             string instanceName,
            string dbName
           )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                IList retList = agentCommand.GetRawUserDatabasesForAlwaysOn(instanceName, dbName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //----------------------------------------------------------------------
        // GetRawAVGetails - get sql objects via agent to get through firewalls
        //----------------------------------------------------------------------
        public IList
           GetRawAVGDetails(
             string instanceName,
             List<string> dbNames
           )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                IList retList = agentCommand.GetRawAVGDetails(instanceName, dbNames);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //----------------------------------------------------------------------
        // GetSecondaryRoleAllowConnections - get sql objects via agent to get through firewalls
        //----------------------------------------------------------------------

        public List<SecondaryRoleDetails>
           GetSecondaryRoleAllowConnections(
           string instanceName
           )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                var retList = agentCommand.GetSecondaryRoleAllowConnections(instanceName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //----------------------------------------------------------------------
        // GetReadOnlySecondaryReplicaServerList
        //----------------------------------------------------------------------
        public List<string> GetReadOnlySecondaryReplicaServerList(string instanceName)
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                var retList = agentCommand.GetReadOnlySecondaryReplicaServerList(instanceName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //----------------------------------------------------------------------
        // GetAllReplicaNodeInfoList
        //----------------------------------------------------------------------
        public List<ReplicaNodeInfo> GetAllReplicaNodeInfoList(string instanceName)
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                var retList = agentCommand.GetAllReplicaNodeInfoList(instanceName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //----------------------------------------------------------------------
        // GetRawSystemDatabases - get sql objects via agent to get through firewalls
        //----------------------------------------------------------------------
        public IList
           GetRawSystemDatabases(
             string instanceName
           )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                IList retList = agentCommand.GetRawSystemDatabases(instanceName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //----------------------------------------------------------------------
        // GetRawTables - get sql objects via agent to get through firewalls
        //----------------------------------------------------------------------
        public IList
           GetRawTables(
             string instanceName,
              string dbName
           )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                IList retList = agentCommand.GetRawTables(instanceName,
                                                  dbName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        public IList GetRawTables(string instanceName, string dbName, string filterTableName)
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                IList retList = agentCommand.GetRawTables(instanceName, dbName, filterTableName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //SQLCM 5.4 Start


        //public IList
        // GetRawTableDetails(
        //   string instanceName,
        //    string dbName
        // )
        //{
        //    try
        //    {
        //        AgentCommand agentCommand = GetAgentCommand(instanceName);
        //        IList retList = agentCommand.GetRawTableDetails(instanceName,
        //                                          dbName);
        //        return retList;
        //    }
        //    catch (Exception e)
        //    {
        //        // Log the error and let the caller handle alerting, messaging etc
        //        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
        //                                 e,
        //                                 true);

        //        throw TranslateRemotingException(GetInstanceServer(instanceName), e);
        //    }
        //}



        //public IList GetRawTableDetails(string instanceName, string dbName, string tableName)
        //{
        //    try
        //    {
        //        AgentCommand agentCommand = GetAgentCommand(instanceName);
        //        IList retList = agentCommand.GetRawTableDetail(instanceName, dbName, tableName);
        //        return retList;
        //    }
        //    catch (Exception e)
        //    {
        //        // Log the error and let the caller handle alerting, messaging etc
        //        ErrorLog.Instance.Write(ErrorLog.Level.Debug,
        //                                 e,
        //                                 true);

        //        throw TranslateRemotingException(GetInstanceServer(instanceName), e);
        //    }
        //}


        public IList GetRawTableDetailsForAll(string instanceName, IList dbName, string profileName)
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                IList retList = agentCommand.GetRawTableDetailForAll(instanceName, dbName, profileName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        public IList GetRawColumnDetailsForAll(string instanceName, IList dbName, string profileName)
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                IList retList = agentCommand.GetRawColumnDetailForAll(instanceName, dbName, profileName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }


        //SQLCM 5.4 END





        public List<string> GetBlobTables(string instanceName, string dbName)
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                return agentCommand.GetBlobTables(instanceName, dbName);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug, e, true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //----------------------------------------------------------------------
        // GetTableColumns - get sql objects via agent to get through firewalls
        //----------------------------------------------------------------------
        public IList
           GetRawColumns(
              string instanceName,
              string dbName,
              string tableName
           )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommand(instanceName);
                IList retList = agentCommand.GetRawColumns(instanceName,
                                                  dbName, tableName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //----------------------------------------------------------------------
        // GetRawServerRoles - get sql objects via agent to get through firewalls
        //----------------------------------------------------------------------
        public IList
           GetRawServerRoles(
             string instanceName
          )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommandEx(instanceName);
                IList retList = agentCommand.GetRawServerRoles(instanceName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        //----------------------------------------------------------------------
        // GetRawServerLogins - get sql objects via agent to get through firewalls
        //----------------------------------------------------------------------
        public IList
           GetRawServerLogins(
             string instanceName
           )
        {
            return GetRawLogins(instanceName, "");
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
            try
            {
                AgentCommand agentCommand = GetAgentCommandEx(instanceName);
                IList retList = agentCommand.GetRawLogins(instanceName,
                                                           databaseName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
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
                AgentCommand agentCommand = GetAgentCommandEx(instanceName);
                agentCommand.GetCLRStatus(instanceName, out configured, out running);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
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
            try
            {
                AgentCommand agentCommand = GetAgentCommandEx(instanceName);
                return agentCommand.EnableCLR(instanceName, enable);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        public int GetCompatibilityLevel(string instanceName, string dbName)
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommandEx(instanceName);
                return agentCommand.GetCompatibilityLevel(instanceName, dbName);
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);
                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        #endregion

        #region Remoting objects
        //------------------------------------------------------------		
        // GetInstanceServer
        //------------------------------------------------------------		
        public static string
           GetInstanceServer(
              string instanceName,
              out int agentPort
           )
        {
            string agentServer = null;
            string query = "";

            agentPort = CoreConstants.AgentServerTcpPort;

            using (SqlConnection conn = GetDatabaseConnection())
            {
                query = String.Format("SELECT agentServer, agentPort FROM {0} WHERE instance = {1}",
                                       CoreConstants.RepositoryServerTable,
                                       SQLHelpers.CreateSafeString(instanceName));

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                agentServer = reader.GetString(0);
                            if (!reader.IsDBNull(1))
                                agentPort = reader.GetInt32(1);
                        }
                    }
                }

                CloseDatabaseConnection(conn);
            }

            return agentServer;
        }

        //------------------------------------------------------------		
        // GetInstanceServer
        //------------------------------------------------------------		
        public static string
           GetInstanceServer(
              string instanceName
           )
        {
            string agentServer = null;
            string query = "";


            using (SqlConnection conn = GetDatabaseConnection())
            {
                query = String.Format("SELECT agentServer FROM {0} WHERE instance = {1}",
                                       CoreConstants.RepositoryServerTable,
                                       SQLHelpers.CreateSafeString(instanceName));

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                agentServer = reader.GetString(0);
                        }
                    }
                }

                CloseDatabaseConnection(conn);
            }

            return agentServer;
        }

        //------------------------------------------------------------		
        // GetAgentCommand
        //------------------------------------------------------------		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        public static AgentCommand
           GetAgentCommand(
              string instanceName
           )
        {
            int agentPort = CoreConstants.AgentServerTcpPort;
            string agentServer = GetInstanceServer(instanceName, out agentPort);

            // If we dont' return null from this, the url will be formed
            //  to return the localhost agent, causing difficult to track errors.
            if (agentServer == null)
                return null;

            return CoreRemoteObjectsProvider.AgentCommand(agentServer, agentPort);
        }

        //------------------------------------------------------------		
        // GetAgentCommandEx
        //  string name - the instance or machine name
        //
        //  This function will parse the incoming name (instance or machine) to a base
        //  machine name.  It will then return an AgentCommand for the first agent entry in the
        //  servers table with an instanceServer that matches the parsed machine name.
        //
        // NOTE:  This allows you to retrieve an agent for an instance that is not being
        //  monitored by SQLCompliance Manager.  This can be useful for some of the generic
        //  machine-based functions, but should not be used for instance-specifc operations.
        //------------------------------------------------------------		
        public static AgentCommand GetAgentCommandEx(string name)
        {
            int agentPort = CoreConstants.AgentServerTcpPort;
            string machineName;
            int index;

            index = name.IndexOf("\\");
            if (index != -1)
                machineName = name.Substring(0, index);
            else
                machineName = name;

            try
            {
                using (SqlConnection conn = GetDatabaseConnection())
                {
                    string query = String.Format("SELECT agentPort FROM {0} WHERE agentServer = {1}",
                        CoreConstants.RepositoryServerTable,
                        SQLHelpers.CreateSafeString(machineName));

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    agentPort = reader.GetInt32(0);
                            }
                            else
                                return null;
                        }
                    }
                    CloseDatabaseConnection(conn);
                }
            }
            catch
            {
                return null;
            }

            return CoreRemoteObjectsProvider.AgentCommand(machineName, agentPort);
        }

        //---------------------------------------------------------------------------
        // TranslateRemotingException
        //---------------------------------------------------------------------------
        public static Exception
           TranslateRemotingException(
              string server,
              Exception ex
           )
        {
            try
            {
                SocketException socketEx = (SocketException)ex;
                if (socketEx.ErrorCode == 10061)
                {
                    return new Exception(String.Format(CoreConstants.Exception_AgentNotAvailable,
                                                         server));
                }
            }
            catch { }

            return ex;
        }


        #endregion

        #region Connection
        //------------------------------------------------------------		
        // GetDatabaseConnection 
        //------------------------------------------------------------		
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static SqlConnection
           GetDatabaseConnection()
        {
            SqlConnection conn = null;

            try
            {
                string connString = String.Format("server={0};database={1};integrated security=SSPI;Application Name='{2}';",
                                                    CollectionServer.ServerInstance,
                                                    CoreConstants.RepositoryDatabase,
                                                    CoreConstants.DefaultSqlApplicationName);
                conn = new SqlConnection(connString);
                conn.Open();

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

            return conn;
        }

        //------------------------------------------------------------		
        // CloseDatbaseConnection
        //------------------------------------------------------------		
        private static void
           CloseDatabaseConnection(
              SqlConnection conn
           )
        {
            if (conn != null &&
               conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            conn = null;
        }
        #endregion

        //Start SQLCm- 5.4
        // Requirement -4.1.3.1

        public IList
                  GetRawSystemDatabasesCWF(SqlConnection connection,
                    string instanceName
                  )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommandCWF(connection, instanceName);
                IList retList = agentCommand.GetRawSystemDatabases(instanceName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        public IList
          GetRawUserDatabasesCWF(SqlConnection connection,
            string instanceName
          )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommandCWF(connection, instanceName);
                IList retList = agentCommand.GetRawUserDatabases(instanceName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        public static AgentCommand
         GetAgentCommandCWF(SqlConnection connection,
            string instanceName
         )
        {
            int agentPort = CoreConstants.AgentServerTcpPort;
            string agentServer = GetInstanceServerCWF(connection, instanceName, out agentPort);

            // If we dont' return null from this, the url will be formed
            //  to return the localhost agent, causing difficult to track errors.
            if (agentServer == null)
                return null;

            return CoreRemoteObjectsProvider.AgentCommand(agentServer, agentPort);
        }

        public static string
         GetInstanceServerCWF(SqlConnection connection,
            string instanceName,
            out int agentPort
         )
        {
            string agentServer = null;
            string query = "";

            agentPort = CoreConstants.AgentServerTcpPort;

            query = String.Format("SELECT agentServer, agentPort FROM {0} WHERE instance = {1}",
                                        CoreConstants.RepositoryServerTable,
                                        SQLHelpers.CreateSafeString(instanceName));

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null && reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            agentServer = reader.GetString(0);
                        if (!reader.IsDBNull(1))
                            agentPort = reader.GetInt32(1);
                    }
                }
            }

            // CloseDatabaseConnection(conn);


            return agentServer;
        }

        public IList
          GetRawTablesCWF(SqlConnection connection,
            string instanceName,
             string dbName
          )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommandCWF(connection, instanceName);
                IList retList = agentCommand.GetRawTables(instanceName,
                                                  dbName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }

        public IList
         GetRawColumnsCWF(SqlConnection connection,
            string instanceName,
            string dbName,
            string tableName
         )
        {
            try
            {
                AgentCommand agentCommand = GetAgentCommandCWF(connection, instanceName);
                IList retList = agentCommand.GetRawColumns(instanceName,
                                                  dbName, tableName);
                return retList;
            }
            catch (Exception e)
            {
                // Log the error and let the caller handle alerting, messaging etc
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         e,
                                         true);

                throw TranslateRemotingException(GetInstanceServer(instanceName), e);
            }
        }
        //End requirement - 4.1.3.1

        public bool IsLinqAssemblyLoaded()
        {
            ActiveQueryCollector activeQueryCollector = new ActiveQueryCollector();
            return activeQueryCollector.IsLinqAssemblyLoaded();
        }

        public static void AutomaticallyAuditNewDatabases(SqlConnection sqlConnection, ServerRecord server)
        {
            string connectionDb = sqlConnection.Database;
            if (server == null)
                return;
            
            try
            {

				AgentCommand agentCommand = GetAgentCommand(server.Instance);

				// Get the time when new databases were added last time
                DateTime lastheartbeatTime = server.LastNewDatabasesAddTime;

                IList dbList = agentCommand.GetNewDatabasesList(server.Instance, lastheartbeatTime.AddMinutes(-1));
                if (dbList != null && dbList.Count > 0)
                {
                    foreach (RawColumnObject columnObject in dbList)
                    {
                        int dbId = columnObject.Id;
                        string columnValue = columnObject.ColumnName;
                        DateTime createdDate = columnObject.CreatedDate;
                        // SQLCM-5913: Add code to compare UTC time of server's last heart beat with creation time on dbs
                        if (createdDate > lastheartbeatTime)
                        {
							if (_lockObject == null)
							{
								_lockObject = new object();
							}

                            lock (_lockObject)
                            {
                                SaveDatabaseRecord(sqlConnection, server, columnValue, dbId);
                            }

                        }
                    }
                }

				// Update the last new databases add time
				// SQLCM-5913: Add code to compare UTC time of server's last heart beat with creation time on dbs
				string query = string.Format(@"UPDATE {0}..{1} SET lastNewDatabasesAddTime = GETUTCDATE() where srvId = {2}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable, server.SrvId);
				using (SqlCommand command = new SqlCommand(query, sqlConnection))
				{
					command.ExecuteNonQuery();
				}
			}
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(CoreConstants.Exception_InstanceDatabaseTable,
                                         ex.Message),
                                         ErrorLog.Severity.Error);
            }
            finally
            {
                sqlConnection.ChangeDatabase(connectionDb);
            }
        }

        public static void SaveDatabaseRecord(SqlConnection sqlConnection, ServerRecord server, string dbName, int dbId)
        {
           CreateDatabaseRecord(sqlConnection, server, dbName, dbId);
        }
        public static bool CheckDatabaseAlreadyExists(SqlConnection connection, string dbName, int srvId)
        {
            bool bFound = false;
            try
            {
                string query = string.Format(@"select count(dbId) from {0}..{1} where name like '{2}' and srvId = {3}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable, dbName, srvId);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader != null && reader.Read())
                        {
                            if (reader.GetInt32(0) > 0)
                                bFound = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(CoreConstants.Exception_InstanceDatabaseTable,
                                         ex.Message),
                                         ErrorLog.Severity.Error);
            }
            return bFound;
        }

        public static void CreateDatabaseRecord(SqlConnection sqlConnection, ServerRecord server, string dbName, int dbId)
        {
            bool bFound = false;
            try
            {
                bFound = CheckDatabaseAlreadyExists(sqlConnection, dbName, server.SrvId);
				SQLcomplianceConfig.Read(sqlConnection);
				if (!bFound)
                {
                    DatabaseRecord db = new DatabaseRecord();
                    db.Connection = sqlConnection;
                    db.SrvId = server.SrvId;
                    db.SrvInstance = server.Instance;
                    db.Name = dbName;
                    db.SqlDatabaseId = dbId;
                    db.Description = "";
                    db.IsEnabled = true;
                    db.IsSqlSecureDb = false;
                    db.IsAlwaysOn = false;
                    db.ReplicaServers = "";
                    //SQLCM-5758 v5.6
                    db.AuditDmlAll = true;
					db.AuditUserTables = SQLcomplianceConfig.AuditUserTables;
					db.AuditSystemTables = SQLcomplianceConfig.AuditSystemTables;
					db.AuditStoredProcedures = SQLcomplianceConfig.AuditStoredProcedures;
					db.AuditDmlOther = SQLcomplianceConfig.AuditDmlOther;
					ApplyDefaultDatabaseSettings(sqlConnection, db, server.Instance);
					if (db.Create(null))
					{
						// Register change to server and perform audit log	      
						ServerRecord.IncrementServerConfigVersion(sqlConnection,
																   db.SrvId);
						LogRecord.WriteLog(sqlConnection,
										   LogType.NewDatabase,
										   db.SrvInstance,
										   db.Name);

						ServerRecord.IncrementServerConfigVersion(sqlConnection,
																   db.SrvId);
						LogRecord.WriteLog(sqlConnection,
										   LogType.NewDatabase,
										   db.SrvInstance,
										   Snapshot.DatabaseSnapshot(sqlConnection, db.DbId, db.Name, true));
	
					}
				}
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(CoreConstants.Exception_InstanceDatabaseTable,
                                         ex.Message),
                                         ErrorLog.Severity.Error);
            }
        }

        //SQLCM-5758 v5.6
        public static DatabaseRecord GetDBAuditSettings(SqlConnection connection, string serverName)
        {
            string queryDefaultSettings = String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDefaultDatabaseSettings);
            string serverDefaultSettings = String.Format("select * from {0}..{1}", CoreConstants.RepositoryDatabase, CoreConstants.DefaultServerPropertise);

            UserList srvDefaultTrustedUserList, srvDefaultPrivilegedUserList;
            UserList defaultTrustedUserList, defaultPrivilegedUserList;
            var databaseRecord = new DatabaseRecord();
            try
            {
                #region read server default settings
                using (SqlCommand srvCommand = new SqlCommand(serverDefaultSettings, connection))
                {
                    using (SqlDataReader srvReader = srvCommand.ExecuteReader())
                    {
                        srvReader.Read();
                        srvDefaultPrivilegedUserList = new UserList(SQLHelpers.GetString(srvReader, "auditUsersList"));
                        srvDefaultTrustedUserList = new UserList(SQLHelpers.GetString(srvReader, "auditTrustedUsersList"));

                        databaseRecord.AuditDDL = SQLHelpers.GetBool(srvReader, "auditDDL");
                        databaseRecord.AuditSecurity = SQLHelpers.GetBool(srvReader, "auditSecurity");
                        databaseRecord.AuditAdmin = SQLHelpers.GetBool(srvReader, "auditAdmin");
                        databaseRecord.AuditAccessCheck = (AccessCheckFilter)SQLHelpers.GetByteToInt(srvReader, "auditFailures");
                        databaseRecord.AuditUserAll = SQLHelpers.GetBool(srvReader, "auditUserAll");
                        databaseRecord.AuditUserLogins = SQLHelpers.GetBool(srvReader, "auditUserLogins");
                        databaseRecord.AuditUserLogouts = SQLHelpers.GetBool(srvReader, "auditUserLogouts");
                        databaseRecord.AuditUserFailedLogins = SQLHelpers.GetBool(srvReader, "auditUserFailedLogins");
                        databaseRecord.AuditUserDDL = SQLHelpers.GetBool(srvReader, "auditUserDDL");
                        databaseRecord.AuditUserSecurity = SQLHelpers.GetBool(srvReader, "auditUserSecurity");
                        databaseRecord.AuditUserAdmin = SQLHelpers.GetBool(srvReader, "auditUserAdmin");
                        databaseRecord.AuditUserDML = SQLHelpers.GetBool(srvReader, "auditUserDML");
                        databaseRecord.AuditUserSELECT = SQLHelpers.GetBool(srvReader, "auditUserSELECT");
                        databaseRecord.AuditUserUDE = SQLHelpers.GetBool(srvReader, "auditUserUDE");
                        databaseRecord.AuditUserAccessCheck = (AccessCheckFilter)SQLHelpers.GetByteToInt(srvReader, "auditUserFailures");
                        databaseRecord.AuditUserCaptureSQL = SQLHelpers.GetBool(srvReader, "auditUserCaptureSQL");
                        databaseRecord.AuditUserCaptureTrans = SQLHelpers.GetBool(srvReader, "auditUserCaptureTrans");
                        databaseRecord.AuditUserCaptureDDL = SQLHelpers.GetBool(srvReader, "auditUserCaptureDDL");
                        srvReader.Close();
                    }
                }
                #endregion

                #region read actual server settings
                ServerRecord actualServerSettings = new ServerRecord();
                actualServerSettings.Connection = connection;
                actualServerSettings.Read(serverName);
                #endregion

                #region read db default settings
                using (SqlCommand command = new SqlCommand(queryDefaultSettings, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();

                        defaultTrustedUserList = new UserList(SQLHelpers.GetString(reader, "auditUsersList"));
                        defaultPrivilegedUserList = new UserList(SQLHelpers.GetString(reader, "auditPrivUsersList"));

                        var finalAuditFailures = SQLHelpers.GetByteToInt(reader, "auditFailures");
                        if ((int)actualServerSettings.AuditAccessCheck != 1)
                            finalAuditFailures = (int)actualServerSettings.AuditAccessCheck;
                        else if ((int)databaseRecord.AuditAccessCheck != 1)
                            finalAuditFailures = (int)databaseRecord.AuditAccessCheck; //set from server default abve

                        var finalAuditUserFailures = SQLHelpers.GetByteToInt(reader, "auditUserFailures");
                        if ((int)actualServerSettings.AuditUserAccessCheck != 1)
                            finalAuditUserFailures = (int)actualServerSettings.AuditUserAccessCheck;
                        else if ((int)databaseRecord.AuditUserAccessCheck != 1)
                            finalAuditUserFailures = (int)databaseRecord.AuditUserAccessCheck; //set from server default above

                        databaseRecord.AuditDDL = databaseRecord.AuditDDL || actualServerSettings.AuditDDL || SQLHelpers.GetBool(reader, "auditDDL");
                        databaseRecord.AuditSecurity = databaseRecord.AuditSecurity || actualServerSettings.AuditSecurity || SQLHelpers.GetBool(reader, "auditSecurity");
                        databaseRecord.AuditAdmin = databaseRecord.AuditAdmin || actualServerSettings.AuditAdmin || SQLHelpers.GetBool(reader, "auditAdmin");
                        databaseRecord.AuditDML = SQLHelpers.GetBool(reader, "auditDML");
                        databaseRecord.AuditSELECT = SQLHelpers.GetBool(reader, "auditSELECT");
                        databaseRecord.AuditAccessCheck = (AccessCheckFilter)finalAuditFailures;
                        databaseRecord.AuditCaptureSQL = SQLHelpers.GetBool(reader, "auditCaptureSQL");
                        databaseRecord.AuditCaptureTrans = SQLHelpers.GetBool(reader, "auditCaptureTrans");
                        databaseRecord.AuditCaptureDDL = SQLHelpers.GetBool(reader, "auditCaptureDDL");
                        databaseRecord.AuditUserAll = databaseRecord.AuditUserAll || actualServerSettings.AuditUserAll || SQLHelpers.GetBool(reader, "auditUserAll"); ;
                        databaseRecord.AuditUserLogins = databaseRecord.AuditUserLogins || actualServerSettings.AuditUserLogins || SQLHelpers.GetBool(reader, "auditUserLogins");
                        databaseRecord.AuditUserLogouts = databaseRecord.AuditUserLogouts || actualServerSettings.AuditUserLogouts || SQLHelpers.GetBool(reader, "auditUserLogouts");
                        databaseRecord.AuditUserFailedLogins = databaseRecord.AuditUserFailedLogins || actualServerSettings.AuditUserFailedLogins || SQLHelpers.GetBool(reader, "auditUserFailedLogins");
                        databaseRecord.AuditUserDDL = databaseRecord.AuditUserDDL || actualServerSettings.AuditUserDDL || SQLHelpers.GetBool(reader, "auditUserDDL");
                        databaseRecord.AuditUserSecurity = databaseRecord.AuditUserSecurity || actualServerSettings.AuditUserSecurity || SQLHelpers.GetBool(reader, "auditUserSecurity");
                        databaseRecord.AuditUserAdmin = databaseRecord.AuditUserAdmin || actualServerSettings.AuditUserAdmin || SQLHelpers.GetBool(reader, "auditUserAdmin");
                        databaseRecord.AuditUserDML = databaseRecord.AuditUserDML || actualServerSettings.AuditUserDML || SQLHelpers.GetBool(reader, "auditUserDML");
                        databaseRecord.AuditUserSELECT = databaseRecord.AuditUserSELECT || actualServerSettings.AuditUserSELECT || SQLHelpers.GetBool(reader, "auditUserSELECT");
                        databaseRecord.AuditUserUDE = databaseRecord.AuditUserUDE || actualServerSettings.AuditUserUDE || SQLHelpers.GetBool(reader, "auditUserUDE");
                        databaseRecord.AuditUserAccessCheck = (AccessCheckFilter)finalAuditUserFailures;
                        databaseRecord.AuditUserCaptureSQL = databaseRecord.AuditUserCaptureSQL || actualServerSettings.AuditUserCaptureSQL || SQLHelpers.GetBool(reader, "auditUserCaptureSQL");
                        databaseRecord.AuditUserCaptureTrans = databaseRecord.AuditUserCaptureTrans || actualServerSettings.AuditUserCaptureTrans || SQLHelpers.GetBool(reader, "auditUserCaptureTrans");
                        databaseRecord.AuditUserCaptureDDL = databaseRecord.AuditUserCaptureDDL || actualServerSettings.AuditUserCaptureDDL || SQLHelpers.GetBool(reader, "auditUserCaptureDDL");
                    }
                }
                #endregion

                #region merge trusted and pri users
                //getting roles and logins for the server
                ICollection roleList = null, loginList = null;
                SQLDirect sqlServer = null;

                //read the available logins and roles from server
                if (actualServerSettings.IsDeployed && actualServerSettings.IsRunning)
                {
                    //string url =  EndPointUrlBuilder.GetUrl(typeof(AgentManager), Globals.SQLcomplianceConfig.Server, Globals.SQLcomplianceConfig.ServerPort);
                    try
                    {
                        AgentManager manager = new AgentManager();
                        roleList = manager.GetRawServerRoles(serverName);

                        loginList = manager.GetRawServerLogins(serverName);

                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                                 String.Format("LoadRoles or Logins: Instance {0}", serverName),
                                                 ex,
                                                 ErrorLog.Severity.Warning);
                    }
                }

                if (roleList == null && loginList == null)
                {
                    sqlServer = new SQLDirect();
                    if (sqlServer.OpenConnection(serverName))
                    {
                        roleList = RawSQL.GetServerRoles(sqlServer.Connection);
                        loginList = RawSQL.GetServerLogins(sqlServer.Connection);
                    }
                }

                if (sqlServer != null)
                {
                    sqlServer.CloseConnection();
                }

                //actul servers users
                var actualServerPrivUsers = new UserList(actualServerSettings.AuditUsersList);
                var actualServerTrustedUsers = new UserList(actualServerSettings.AuditTrustedUsersList);

                UserList trustedUserList = new UserList();
                UserList privilegedUserList = new UserList();
                if ((roleList != null) && (roleList.Count != 0))
                {
                    foreach (RawRoleObject role in roleList)
                    {
                    	// SQLCM-5868: Roles added to default server settings gets added twice at database level
                        foreach (ServerRole serverRole in defaultTrustedUserList.ServerRoles)
                        {
                            if (serverRole.CompareName(role))
                            {
                                trustedUserList.AddServerRole(role);
                                break;
                            }
                        }
                        foreach (ServerRole serverRole in defaultPrivilegedUserList.ServerRoles)
                        {
                            if (serverRole.CompareName(role))
                            {
                                privilegedUserList.AddServerRole(role);
                                break;
                            }
                        }

                        // SQLCM-5868: Roles added to default server settings gets added twice at database level
                        //default server
                        foreach (var prvServerRole in srvDefaultPrivilegedUserList.ServerRoles)
                        {
                            var rawRole = (RawRoleObject)role;
                            if (prvServerRole.CompareName(rawRole))
                            {
                                privilegedUserList.AddServerRole(rawRole); //duplication handled inside AddServerRole
                                break;
                            }
                        }
                        foreach (var trustedServerRole in srvDefaultTrustedUserList.ServerRoles)
                        {
                            var rawRole = (RawRoleObject)role;
                            if (trustedServerRole.CompareName(rawRole))
                            {
                                trustedUserList.AddServerRole(trustedServerRole);
                                break;
                            }
                        }

                        // SQLCM-5868: Roles added to default server settings gets added twice at database level
                        // actual server
                        foreach (var prvServerRole in actualServerPrivUsers.ServerRoles)
                        {
                            var rawRole = (RawRoleObject)role;
                            if (prvServerRole.CompareName(rawRole))
                            {
                                privilegedUserList.AddServerRole(rawRole);
                                break;
                            }
                        }

                        foreach (var trustedServerRole in actualServerTrustedUsers.ServerRoles)
                        {
                            var rawRole = (RawRoleObject)role;
                            if (trustedServerRole.CompareName(rawRole))
                            {
                                trustedUserList.AddServerRole(rawRole);
                                break;
                            }
                        }
                    }

                }

                if ((loginList != null) && (loginList.Count != 0))
                {
                    foreach (RawLoginObject login in loginList)
                    {
                        foreach (Login defaultLogin in defaultTrustedUserList.Logins)
                        {
                            if (login.name.Equals(defaultLogin.Name))
                            {
                                trustedUserList.AddLogin(login.name, login.sid);
                                break;
                            }
                        }
                        foreach (Login defaultLogin in defaultPrivilegedUserList.Logins)
                        {
                            if (login.name.Equals(defaultLogin.Name))
                            {
                                privilegedUserList.AddLogin(login.name, login.sid);
                                break;
                            }
                        }

                        //default server
                        foreach (var prvLogin in srvDefaultPrivilegedUserList.Logins)
                        {
                            var rawLogin = (RawLoginObject)login;
                            if (rawLogin.name.Equals(prvLogin.Name))
                            {
                                privilegedUserList.AddLogin(prvLogin);
                                break;
                            }
                        }

                        foreach (var trustedLogin in srvDefaultTrustedUserList.Logins)
                        {
                            var rawLogin = (RawLoginObject)login;
                            if (rawLogin.name.Equals(trustedLogin.Name))
                            {
                                trustedUserList.AddLogin(trustedLogin);
                                break;
                            }
                        }

                        //actual server
                        foreach (var prvLogin in actualServerPrivUsers.Logins)
                        {
                            var rawLogin = (RawLoginObject)login;
                            if (rawLogin.name.Equals(prvLogin.Name))
                            {
                                privilegedUserList.AddLogin(prvLogin);
                                break;
                            }
                        }

                        foreach (var trustedLogin in actualServerTrustedUsers.Logins)
                        {
                            var rawLogin = (RawLoginObject)login;
                            if (rawLogin.name.Equals(trustedLogin.Name))
                            {
                                trustedUserList.AddLogin(trustedLogin);
                                break;
                            }
                        }
                    }
                }

                databaseRecord.AuditUsersList = (trustedUserList.Logins.Length > 0 || trustedUserList.ServerRoles.Length > 0) ?
                    trustedUserList.ToString() : string.Empty;
                databaseRecord.AuditPrivUsersList = (privilegedUserList.Logins.Length > 0 || privilegedUserList.ServerRoles.Length > 0) ?
                    privilegedUserList.ToString() : string.Empty;
                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }
            return databaseRecord;
        }

        public static void ApplyDefaultDatabaseSettings(SqlConnection sqlConnection, DatabaseRecord db, string instanceName)
        {
            try
            {
                //SQLCM-5758 v5.6
                var dbSettings = GetDBAuditSettings(sqlConnection, instanceName);
                //this object can be assigned directly as in db object, there might be other propertise set so just assign the required ones

                db.AuditDDL = dbSettings.AuditDDL;
                db.AuditSecurity = dbSettings.AuditSecurity;
                db.AuditAdmin = dbSettings.AuditAdmin;
                db.AuditDML = dbSettings.AuditDML;
                db.AuditSELECT = dbSettings.AuditSELECT;
                db.AuditAccessCheck = dbSettings.AuditAccessCheck;
                db.AuditCaptureDDL = dbSettings.AuditCaptureDDL;
                db.AuditCaptureTrans = dbSettings.AuditCaptureTrans;
                db.AuditCaptureSQL = dbSettings.AuditCaptureSQL;
                db.AuditUsersList = dbSettings.AuditUsersList;
                db.AuditPrivUsersList = dbSettings.AuditPrivUsersList;
                db.AuditUserAll = dbSettings.AuditUserAll;
                db.AuditUserLogins = dbSettings.AuditUserLogins;
                db.AuditUserLogouts = dbSettings.AuditUserLogouts;
                db.AuditUserFailedLogins = dbSettings.AuditUserFailedLogins;
                db.AuditUserDDL = dbSettings.AuditUserDDL;
                db.AuditUserSecurity = dbSettings.AuditUserSecurity;
                db.AuditUserAdmin = dbSettings.AuditUserAdmin;
                db.AuditUserDML = dbSettings.AuditUserDML;
                db.AuditUserSELECT = dbSettings.AuditUserSELECT;
                db.AuditUserUDE = dbSettings.AuditUserUDE;
                db.AuditUserAccessCheck = dbSettings.AuditUserAccessCheck;
                db.AuditUserCaptureSQL = dbSettings.AuditUserCaptureSQL;
                db.AuditUserCaptureTrans = dbSettings.AuditUserCaptureTrans;
                db.AuditUserCaptureDDL = dbSettings.AuditUserCaptureDDL;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(CoreConstants.Exception_InstanceDatabaseTable,
                                         ex.Message),
                                         ErrorLog.Severity.Error);
            }
        }
        //SQLCM -541/4876/5259 v5.6 -END

        /// <summary>
        /// UpdateInstanceDatabasesTable: Update the InstanceDatabases table with databases list when audit settings are updated
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="instanceName"></param>
        public static void UpdateInstanceDatabasesTable(SqlConnection sqlConnection, ServerRecord server)
        {
            string connectionDb = sqlConnection.Database;
            if (server == null)
                return;
            try
            {
                AgentCommand agentCommand = GetAgentCommand(server.Instance);
                IList userDbList = agentCommand.GetRawUserDatabases(server.Instance);
                IList systemDbList = agentCommand.GetRawSystemDatabases(server.Instance);

                if (userDbList != null)
                {
                    sqlConnection.ChangeDatabase(server.EventDatabase);
                    DataTable table = new DataTable();
                    table.Columns.Add("srvId", Type.GetType("System.Int32"));
                    table.Columns.Add("databaseName", Type.GetType("System.String"));
                    table.Columns.Add("dbId", Type.GetType("System.Int16"));
                    foreach (RawDatabaseObject db in userDbList)
                    {
                        DataRow row = table.NewRow();
                        row["srvId"] = server.SrvId;
                        row["databaseName"] = db.name;
                        row["dbId"] = db.dbid;
                        table.Rows.Add(row);
                    }
                    if (systemDbList != null)
                    {
                        foreach (RawDatabaseObject db in systemDbList)
                        {
                            DataRow row = table.NewRow();
                            row["srvId"] = server.SrvId;
                            row["databaseName"] = db.name;
                            row["dbId"] = db.dbid;
                            table.Rows.Add(row);
                        }
                    }
                    using (SqlTransaction transaction = sqlConnection.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        try
                        {
                            string deleteSql = string.Format("DELETE FROM {0}", CoreConstants.InstanceDatabasesTable);
                            using (SqlCommand cmd = new SqlCommand(deleteSql, sqlConnection, transaction))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction))
                            {
                                bulkCopy.DestinationTableName = CoreConstants.InstanceDatabasesTable;
                                bulkCopy.BulkCopyTimeout = CoreConstants.sqlcommandTimeout;
                                bulkCopy.WriteToServer(table);
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Debug,
                                         String.Format(CoreConstants.Exception_InstanceDatabaseTable,
                                         ex.Message),
                                         ErrorLog.Severity.Error);
            }
            finally
            {
                sqlConnection.ChangeDatabase(connectionDb);
            }
        }
    }

    //SQLCM-5758 v5.6
    class SQLDirect
    {
        #region Properties

        private bool _ownConnection = false;
        private SqlConnection _conn = null;

        public SqlConnection Connection
        {
            get { return _conn; }
            set { _conn = value; _ownConnection = false; }
        }

        #endregion

        #region GetLastError

        static private string errMsg;
        static public string GetLastError()
        {
            return errMsg;
        }

        #endregion

        #region Connection Management

        //-----------------------------------------------------------------------------
        // OpenConnection - open a connection to a SQL server
        //-----------------------------------------------------------------------------
        public bool OpenConnection(string serverName)
        {
            bool retval;

            try
            {
                string strConn = String.Format("server={0};" +
                                                "integrated security=SSPI;" +
                                               "Connect Timeout=30;" +
                                                "Application Name='{1}';",
                                                serverName,
                                                CoreConstants.ManagementConsoleName);

                // Cleanup any previous connections if necessary
                CloseConnection();

                _conn = new SqlConnection(strConn);
                _conn.Open();

                _ownConnection = true;

                errMsg = "";
                retval = true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                retval = false;
            }

            return retval;
        }

        //-----------------------------------------------------------------------------
        // CloseConnection - close the connection to the SQLsecure configuration database
        //-----------------------------------------------------------------------------
        public void CloseConnection()
        {
            if (_ownConnection && _conn != null)
            {
                _conn.Close();
                _conn.Dispose();
                _conn = null;
            }
        }

        #endregion


    }
}
