using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Licensing;
using Idera.SQLcompliance.Core.Stats;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.Templates.AuditTemplates;
using SQLcomplianceCwfAddin.Helpers.Agent;
using SQLcomplianceCwfAddin.Helpers.Regulations;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddServer;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.RemoveServers;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ServerProperties;
using ServerRole = SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase.ServerRole;
using SqlServerSecurityModel = SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances.SqlServerSecurityModel;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using System.Collections;

namespace SQLcomplianceCwfAddin.Helpers
{
    public class AuditServerHelper : Singleton<AuditServerHelper>
    {
        #region Private Members
        private static int MAX_THRESHOLD_VALUE = 2000000000;
        private static int DEFAULT_WATERMARK = -2100000000;
        private static string PROBLEM_LOADING_PRIVILEGED_USERS_ERROR = "Problem loading Privileged User auditing information.";
        private static string Error_LimitSQLLength = "Truncate SQL length must be greater than 0.";
        private static string Error_InvalidThreshold = "Thresholds must be an integer value.";
        private static string Error_ThresholdLessThanZero = "The {0} thresholds must have values greater than zero.";
        private static string Error_ThresholdErrorLessThanWarn = "The {0} error threshold cannot be less than the warning threshold.";
        private static string Error_ThresholdOverflow = "The {0} threshold is out of range for Statistic: {1}.  Please specify a value between 1 and 2000000000.";
        private static string EventDatabaseNotCreated = "Event database not created yet";
        private static string ServerStatus_NotAudited = "Archive server";
        private static string Error_MustSelectOneAuditUserOption = "You must select at least one type of activity to be audited for privileged users.";
        private static string Error_ErrorSavingServer = "An error occurred trying to save the changes to the registered server. The server may be modified after the problem is resolved.";
        private static string Error_DMOLoadServers = "An error occurred trying to load the list of SQL Servers available on your network.";
        private static string Warning_AlwaysOnRemoveServer = "Cannot remove the Server when the server has always on availability group databases, first remove the always on database from primary nodes.";
        private static string Error_RemoveEventsDatabase = "An error occurred removing the events database for this SQL Server instance. The SQL Server instance will still be removed but you will need to delete the events database at a later time.";
        private static string Error_RemoveEventsServerProblem = "An error occurred trying to remove the SQL Server instance: {0} and its associated audit data.\n\nError:\n\n{1}";
        private static string Error_NoInstallUtilLib = "Could not verify service account password.\n\nError: {0}";
        private static string Error_ErrorCreatingServer = "An error occurred trying to register the SQL Server with SQL Compliance Manager. The server may be registered after the problem is resolved.";
        private static string Error_ErrorConvertingServer = "An error occurred trying to start auditing of the SQL Server. The server may be registered after the problem is resolved.";
        private static string Error_CantCreateEventsDatabase = "An error occurred creating the events database for this instance. The registration of this server cannot be done at this time.";
        private static string Error_ServerAlreadyRegistered = "This SQL Server instance is already registered for auditing.";

        #endregion

        #region Private Members

        private string GetServerInstanceVersion(ServerRecord server)
        {
            string version = string.Empty;

            if (server.SqlVersion < 8 || server.SqlVersion > 14)
            {
                version = "Unknown";
            }
            else
            {
                version = GetInstanceVersion(server);

                if (server.isClustered)
                {
                    version += " (Virtual)";
                }

            }

            return version;
        }

        private void SetGeneralProperties(AuditServerProperties serverProperties, ServerRecord server, SqlConnection connection)
        {
            serverProperties.GeneralProperties = new ServerGeneralProperties();
            var generalProperties = serverProperties.GeneralProperties;
            generalProperties.Instance = server.Instance;
            generalProperties.InstanceVersion = GetServerInstanceVersion(server);
            generalProperties.CreatedDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(server.TimeCreated);
            generalProperties.LastModifiedDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(server.TimeLastModified);
            generalProperties.IsAuditedServer = server.IsAuditedServer;
            generalProperties.Description = server.Description;

            if (server.IsAuditedServer)
            {
                string opStatus;
                string auditStatus;
                SQLRepository.GetStatus(server, connection, out opStatus, out auditStatus);
                generalProperties.StatusMessage = opStatus;
                generalProperties.LastHeartbeatDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(server.TimeLastHeartbeat);
                generalProperties.EventsReceivedDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(server.TimeLastCollection);
                generalProperties.IsAuditEnabled = server.IsEnabled;
                generalProperties.LastAgentUpdateDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(server.LastConfigUpdate);
                generalProperties.AuditSettingsUpdateEnabled = server.ConfigVersion > server.LastKnownConfigVersion;
                generalProperties.EventsDatabaseName = string.IsNullOrEmpty(server.EventDatabase)
                    ? EventDatabaseNotCreated
                    : server.EventDatabase;
                generalProperties.IsDatabaseIntegrityOk = !server.ContainsBadEvents;

                generalProperties.LastIntegrityCheckDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(server.TimeLastIntegrityCheck);
                generalProperties.LastIntegrityCheckResultsStatus = generalProperties.LastIntegrityCheckDateTime.HasValue 
                        ?(IntegrityCheckStatus)Enum.ToObject(typeof(IntegrityCheckStatus), server.LastIntegrityCheckResult)
                        : IntegrityCheckStatus.None;

                generalProperties.LastArchiveCheckDateTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(server.TimeLastArchive);
                generalProperties.LastArchiveCheckResultsStatus = generalProperties.LastArchiveCheckDateTime.HasValue
                    ? (ArchiveCheckStatus)Enum.ToObject(typeof(ArchiveCheckStatus), server.LastArchiveResult)
                    : ArchiveCheckStatus.None;
            }
            else
            {
                generalProperties.StatusMessage = ServerStatus_NotAudited;
                generalProperties.LastHeartbeatDateTime = null;
                generalProperties.LastAgentUpdateDateTime = null;
                generalProperties.AuditSettingsUpdateEnabled = false;
                generalProperties.EventsReceivedDateTime = null;
                generalProperties.EventsDatabaseName = string.Empty;
                generalProperties.LastIntegrityCheckDateTime = null;
                generalProperties.LastIntegrityCheckResultsStatus = IntegrityCheckStatus.None;
                generalProperties.LastArchiveCheckDateTime = null;
                generalProperties.LastArchiveCheckResultsStatus = ArchiveCheckStatus.None;
            }
        }

        private void SetAuditActivities(AuditServerProperties serverProperties, ServerRecord server)
        {
            serverProperties.AuditedActivities = new AuditActivity();
            var auditActivity = serverProperties.AuditedActivities;
            auditActivity.AuditLogins = server.AuditLogins;
            auditActivity.AuditLogouts = server.AuditLogouts;
            auditActivity.AuditFailedLogins = server.AuditFailedLogins;
            auditActivity.AuditDDL = server.AuditDDL;
            auditActivity.AuditAdmin = server.AuditAdmin;
            auditActivity.AuditSecurity = server.AuditSecurity;
            auditActivity.AuditDefinedEvents = server.AuditUDE;
            auditActivity.AuditAccessCheck = server.AuditAccessCheck;
            auditActivity.AuditCaptureSQLXE = server.AuditCaptureSQLXE;
            auditActivity.IsAuditLogEnabled = server.IsAuditLogEnabled;
        }

        private void SetPrivilegedUserAuditing(AuditServerProperties serverProperties, ServerRecord server)
        {
            serverProperties.AuditedPrivilegedUserActivities = new AuditActivity();
            var auditUserActivities = serverProperties.AuditedPrivilegedUserActivities;
            serverProperties.PrivilegedRolesAndUsers = GetServerRolesAndUsersFromString(server.AuditUsersList, PROBLEM_LOADING_PRIVILEGED_USERS_ERROR);
            auditUserActivities.AuditAllUserActivities = server.AuditUserAll;
            auditUserActivities.AuditLogins = server.AuditUserLogins;
            auditUserActivities.AuditLogouts = server.AuditUserLogouts;
            auditUserActivities.AuditFailedLogins = server.AuditUserFailedLogins;
            auditUserActivities.AuditDDL = server.AuditUserDDL;
            auditUserActivities.AuditSecurity = server.AuditUserSecurity;
            auditUserActivities.AuditAdmin = server.AuditUserAdmin;
            auditUserActivities.AuditDML = server.AuditUserDML;
            auditUserActivities.AuditSELECT = server.AuditUserSELECT;
            auditUserActivities.AuditDefinedEvents = server.AuditUserUDE;
            auditUserActivities.AuditAccessCheck = server.AuditUserAccessCheck;
            auditUserActivities.AuditCaptureSQL = server.AuditUserCaptureSQL;
            auditUserActivities.AuditCaptureDDL = server.AuditUserCaptureDDL;
            auditUserActivities.AuditCaptureTrans = server.AuditUserCaptureTrans;
            auditUserActivities.AllowCaptureSql = CoreConstants.AllowCaptureSql;
            auditUserActivities.IsAgentVersionSupported = ServerRecord.CompareVersions(server.AgentVersion, "3.5") >= 0;
            auditUserActivities.AuditUserExtendedEvents = server.AuditExtendedEvents; //SQLCm 5.4_4.1.1_Extended Events
                                    
        }

        private ReportCard CreateDafaultReportCard(RestStatsCategory category, int serverId)
        {
            return new ReportCard
            {
                CriticalThreshold = 150,
                Enabled = false,
                Period = 4,
                ServerId = serverId,
                StatisticCategory = category,
                WarningThreshold = 100,  
            };
        }

        private List<ReportCard> GetDefaultThresholdList(int serverId)
        {
            return new List<ReportCard>
            {
                CreateDafaultReportCard(RestStatsCategory.Alerts, serverId),
                CreateDafaultReportCard(RestStatsCategory.FailedLogin, serverId),
                CreateDafaultReportCard(RestStatsCategory.Security, serverId),
                CreateDafaultReportCard(RestStatsCategory.Ddl, serverId),
                CreateDafaultReportCard(RestStatsCategory.PrivUserEvents, serverId),
                CreateDafaultReportCard(RestStatsCategory.EventProcessed, serverId),
                //start sqlcm 5.6 - 5363
                CreateDafaultReportCard(RestStatsCategory.Logins, serverId),
                CreateDafaultReportCard(RestStatsCategory.Logout, serverId),
                //end sqlcm 5.6 - 5363
            };
        }

        private void SetAuditingThresholds(AuditServerProperties serverProperties, ServerRecord server, SqlConnection connection)
        {
            serverProperties.AuditThresholdsData = new ThresholdsData();
            serverProperties.AuditThresholdsData.ThresholdList = GetDefaultThresholdList(server.SrvId);
            var reportCards = ReportCardRecord.GetServerReportCardEntries(connection, server.SrvId);
            var thresholdListToMerge = Converter.ConvertReferenceList(reportCards,
                    delegate(ReportCardRecord card)
                    {
                        return new ReportCard
                        {
                            ServerId = card.SrvId,
                            StatisticCategory = (RestStatsCategory)Enum.ToObject(typeof(RestStatsCategory), card.StatisticId),
                            WarningThreshold = card.WarningThreshold,
                            CriticalThreshold = card.CriticalThreshold,
                            Period = card.Period,
                            Enabled = card.Enabled,
                        };
                    });

            if (thresholdListToMerge != null &&
                thresholdListToMerge.Count > 0)
            {
                foreach(var reportCard in thresholdListToMerge)
                {
                    var reportCardToUpdate = serverProperties.AuditThresholdsData.ThresholdList.Find(t => t.StatisticCategory == reportCard.StatisticCategory);

                    if (reportCardToUpdate != null)
                    {
                        reportCardToUpdate.CriticalThreshold = reportCard.CriticalThreshold;
                        reportCardToUpdate.Enabled = reportCard.Enabled;
                        reportCardToUpdate.Period = reportCard.Period;
                        reportCardToUpdate.WarningThreshold = reportCard.WarningThreshold;
                    }
                }
            }
        }

        private void SetAdvancedSettings(AuditServerProperties serverProperties, ServerRecord server)
        {
            serverProperties.ServerAdvancedProperties = new ServerAdvancedProperties();
            serverProperties.ServerAdvancedProperties.DefaultDatabasePermissions =
                (DatabaseReadAccessLevel)Enum.ToObject(typeof(DatabaseReadAccessLevel), server.DefaultAccess);

            serverProperties.ServerAdvancedProperties.SQLStatementLimit = server.MaxSqlLength;
        }

        private void ValidateThresholdData(ThresholdsData auditThresholdsData)
        {
            if (auditThresholdsData != null &&
                auditThresholdsData.ThresholdList != null)
            {
                foreach (var card in auditThresholdsData.ThresholdList)
                {
                    if (card.WarningThreshold > MAX_THRESHOLD_VALUE)
                    {
                        LogAndThrowException(string.Format(Error_ThresholdOverflow, "Warning", card.StatisticCategory));
                    }

                    if (card.CriticalThreshold > MAX_THRESHOLD_VALUE)
                    {
                        LogAndThrowException(string.Format(Error_ThresholdOverflow, "Critical", card.StatisticCategory));
                    }

                    if (card.WarningThreshold <= 0 ||
                        card.CriticalThreshold <= 0)
                    {
                        LogAndThrowException(Error_ThresholdLessThanZero);
                    }

                    if (card.WarningThreshold > card.CriticalThreshold)
                    {
                        LogAndThrowException(Error_ThresholdErrorLessThanWarn);
                    }
                }
            }
        }

        private void ValidateProperties(AuditServerProperties serverProperties, ServerRecord server)
        {
            if (server.IsAuditedServer)
            {
                ValidateAuditedActivitiesForRolesAndUsers(serverProperties.PrivilegedRolesAndUsers, serverProperties.AuditedPrivilegedUserActivities);
                ValidateThresholdData(serverProperties.AuditThresholdsData);
            }
        }

        private void UpdateServerRecordWithAuditActivities(AuditActivity auditActivity, ServerRecord server)
        {
            server.AuditLogins = auditActivity.AuditLogins;
            server.AuditLogouts = auditActivity.AuditLogouts;
            server.AuditFailedLogins = auditActivity.AuditFailedLogins;
            server.AuditDDL = auditActivity.AuditDDL;
            server.AuditAdmin = auditActivity.AuditAdmin;
            server.AuditSecurity = auditActivity.AuditSecurity;
            server.AuditUDE = auditActivity.AuditDefinedEvents;
            server.AuditAccessCheck = auditActivity.AuditAccessCheck;
            server.AuditCaptureSQLXE = auditActivity.AuditCaptureSQLXE;
            server.IsAuditLogEnabled = auditActivity.IsAuditLogEnabled;
        }

        private void UpdateCustomServerRecordWithAuditActivities(AuditActivity auditActivity, ServerRecord server)
        {
            server.AuditLogins = auditActivity.AuditLogins;
            server.AuditLogouts = auditActivity.AuditLogouts;
            server.AuditFailedLogins = auditActivity.AuditFailedLogins;
            server.AuditDDL = auditActivity.AuditDDL;
            server.AuditAdmin = auditActivity.AuditAdmin;
            server.AuditSecurity = auditActivity.AuditSecurity;
            server.AuditUDE = auditActivity.AuditDefinedEvents;
            server.AuditAccessCheck = auditActivity.AuditAccessCheck;
        }

        private void UpdateServerRecordWithAuditActivities(AuditServerProperties serverProperties, ServerRecord server)
        {
            var auditActivity = serverProperties.AuditedActivities;
            UpdateServerRecordWithAuditActivities(auditActivity, server);
        }

        private void UpdateUserAuditActivities(AuditActivity auditUserActivities, ServerRecord server)
        {
            server.AuditUserAll = auditUserActivities.AuditAllUserActivities;
            server.AuditUserLogins = auditUserActivities.AuditLogins;
            server.AuditUserLogouts = auditUserActivities.AuditLogouts;
            server.AuditUserFailedLogins = auditUserActivities.AuditFailedLogins;
            server.AuditUserDDL = auditUserActivities.AuditDDL;
            server.AuditUserSecurity = auditUserActivities.AuditSecurity;
            server.AuditUserAdmin = auditUserActivities.AuditAdmin;
            server.AuditUserDML = auditUserActivities.AuditDML;
            server.AuditUserSELECT = auditUserActivities.AuditSELECT;
            server.AuditExtendedEvents = auditUserActivities.AuditUserExtendedEvents;  // SQLCm 5.4_4.1.1_Extended Events
            server.AuditUserUDE = auditUserActivities.AuditDefinedEvents;
            server.AuditUserAccessCheck = auditUserActivities.AuditAccessCheck;
            auditUserActivities.AllowCaptureSql = CoreConstants.AllowCaptureSql;
            auditUserActivities.IsAgentVersionSupported = ServerRecord.CompareVersions(server.AgentVersion, "3.5") >= 0;
            auditUserActivities.AuditCaptureSQL = auditUserActivities.AllowCaptureSql && auditUserActivities.AuditCaptureSQL;
            auditUserActivities.AuditCaptureDDL = auditUserActivities.AllowCaptureSql && auditUserActivities.AuditCaptureDDL;
            auditUserActivities.AuditCaptureTrans = auditUserActivities.IsAgentVersionSupported && auditUserActivities.AuditCaptureTrans;

            server.AuditUserCaptureSQL = auditUserActivities.AuditCaptureSQL;
            server.AuditUserCaptureTrans = auditUserActivities.AuditCaptureTrans;
            server.AuditUserCaptureDDL = auditUserActivities.AuditCaptureDDL;
        }

        private void UpdateServerRecordWithUserAuditingActivitiesAndRoleUsers(AuditActivity auditUserActivities, ServerRolesAndUsers rolesAndUsers ,ServerRecord server)
        {
            server.AuditUsersList = GetRolesAndUsersString(rolesAndUsers);
            UpdateUserAuditActivities(auditUserActivities, server);            
        }

        private void UpdateServerRecordWithPrivilegedUserAuditing(AuditServerProperties serverProperties, ServerRecord server)
        {
            var auditUserActivities = serverProperties.AuditedPrivilegedUserActivities;
            UpdateServerRecordWithUserAuditingActivitiesAndRoleUsers(auditUserActivities, serverProperties.PrivilegedRolesAndUsers, server);
        }

        private ServerRecord CreateServerRecord(AuditServerProperties serverProperties, ServerRecord server)
        {
            var newServer = server.Clone();
            newServer.Description = serverProperties.GeneralProperties.Description;
            newServer.DefaultAccess = Convert.ToInt32(serverProperties.ServerAdvancedProperties.DefaultDatabasePermissions);

            if (newServer.IsAuditedServer)
            {
                UpdateServerRecordWithAuditActivities(serverProperties, newServer);
                UpdateServerRecordWithPrivilegedUserAuditing(serverProperties, newServer);
                newServer.MaxSqlLength = serverProperties.ServerAdvancedProperties.SQLStatementLimit;
            }

            return newServer;
        }

        private string GetLocalHostName()
        {
            return Dns.GetHostName().ToUpper();            
        }

        private string GetInstanceServer(string instance)
        {
            string host = string.Empty;

            int pos = instance.IndexOf(@"\");
            if (pos > 0)
            {
                host = instance.Substring(0, pos);
            }
            else
            {
                host = instance;
            }

            // expand local reference
            if (host == "" || host == ".")
            {
                host = GetLocalHostName();
            }

            return host;
        }

        private bool IsRepositoryHost(string host, SQLcomplianceConfiguration config)
        {
            string repositoryHost = GetInstanceServer(config.Server.ToUpper());
            if (host == "" || host == ".")
            {
                host = GetLocalHostName();
            }

            if (host.ToUpper() == repositoryHost)
            {
                return true;
            }

            return false;
        }

        private ServerRecord FindRegisteredAudedtedServerRecord(string instance, SqlConnection connection)
        {
            string instanceServer = GetInstanceServer(instance);
            return FindRegisteredAudedtedServerRecord(instanceServer, instance, connection);
        }

        private ServerRecord FindRegisteredAudedtedServerRecord(string sqlInstanceServer, string sqlInstance, SqlConnection connection)
        {
            string instance = sqlInstance.ToUpper();
            string instanceServer = sqlInstanceServer.ToUpper();

            var serverList = ServerRecord.GetServers(connection, false);

            if (serverList == null ||
                serverList.Count == 0)
            {
                return null;
            }

            foreach (ServerRecord foundServer in serverList)
            {

                if (foundServer.IsAuditedServer)
                {
                    if (foundServer.Instance.ToUpper() == instance.ToUpper())
                    {
                        //ErrorMessage.Show(this.Text, UIConstants.Error_ServerAlreadyRegistered);
                        return null;
                    }

                    // some possible states depend on state of already
                    // audited instances on same computer				      
                    if (foundServer.InstanceServer.ToUpper() == instanceServer.ToUpper())
                    {
                        return foundServer;
                    }
                }
            }

            return null;
        }

        private ServerRecord FindRegisteredAudedtedServerRecord(ServerRecord serverToBeRegistered, SqlConnection connection)
        {
            return FindRegisteredAudedtedServerRecord(serverToBeRegistered.InstanceServer, serverToBeRegistered.Instance, connection);
        }

        private ServerRecord FindNotAudedtedServerRecord(ServerRecord notAuditedServerRecord, SqlConnection connection)
        {
            string instance = notAuditedServerRecord.Instance.ToUpper();
            var serverList = ServerRecord.GetServers(connection, false);

            if (serverList == null ||
                serverList.Count == 0)
            {
                return null;
            }

            foreach (ServerRecord foundServer in serverList)
            {

                if (!foundServer.IsAuditedServer &&
                    foundServer.Instance.ToUpper() == instance)
                {
                    return foundServer;
                }
            }

            return null;
        }

        private void SetAgentPropertiesForServerRecord(ServerRecord server, AgentDeploymentProperties properties)
        {
            server.IsDeployed = properties.IsDeployed;
            server.IsDeployedManually = properties.IsDeployedManually;
            server.AgentTraceDirectory = properties.AgentTraceDirectory;

            if (properties.AgentServiceCredentials != null)
            {
                server.AgentServiceAccount = properties.AgentServiceCredentials.Account;                
            }
        }

        private ServerRecord CreateNewServerRecord(AuditServerProperties serverProperties, AgentDeploymentProperties agentProperties, SQLcomplianceConfiguration config, SqlConnection connection)
        {
            var server = new ServerRecord();
            server.Connection = connection;

            // General
            server.Instance = serverProperties.GeneralProperties.Instance;
            server.InstancePort = serverProperties.GeneralProperties.InstancePort;
            server.isClustered = serverProperties.GeneralProperties.IsClustered;

            server.InstanceServer = string.IsNullOrEmpty(serverProperties.GeneralProperties.InstanceServer)
                ? GetInstanceServer(serverProperties.GeneralProperties.Instance) :
                serverProperties.GeneralProperties.InstanceServer;

            server.AgentServer = server.InstanceServer;
            server.Description = serverProperties.GeneralProperties.Description;
            server.IsEnabled = true;

            server.DefaultAccess = Convert.ToInt32(serverProperties.ServerAdvancedProperties.DefaultDatabasePermissions);
            server.ConfigVersion = 1;
            server.LastKnownConfigVersion = 0;
            server.LastConfigUpdate = DateTime.MinValue;
            server.IsAuditedServer = true;
            server.IsOnRepositoryHost = IsRepositoryHost(server.InstanceServer, config);

            var registeredServer = FindRegisteredAudedtedServerRecord(server, connection);
            // Agent Settings
            var alreadyDeployedAgentProperties = GetAgentPropertiesOfRegisteredServer(server.Instance, registeredServer, config);

            if (alreadyDeployedAgentProperties.IsDeployed)
            {
                SetAgentPropertiesForServerRecord(server, alreadyDeployedAgentProperties);
            }
            else
            {
                SetAgentPropertiesForServerRecord(server, agentProperties);
            }

            //// Audit Settings		
            UpdateServerRecordWithAuditActivities(serverProperties, server);
            UpdateServerRecordWithPrivilegedUserAuditing(serverProperties, server);

            server.AuditExceptions = false;
            server.AuditUserExceptions = false;

            
            server.LowWatermark = DEFAULT_WATERMARK;
            server.HighWatermark = DEFAULT_WATERMARK;

            //// copy agent properties from existing audited instances   
            if (registeredServer != null)
            {
                server.IsRunning = registeredServer.IsRunning;
                server.IsCrippled = registeredServer.IsCrippled;
                server.InsertAgentProperties = true;
                server.AgentServer = registeredServer.AgentServer;
                server.AgentPort = registeredServer.AgentPort;
                server.AgentServiceAccount = registeredServer.AgentServiceAccount;
                server.AgentTraceDirectory = registeredServer.AgentTraceDirectory;
                server.AgentCollectionInterval = registeredServer.AgentCollectionInterval;
                server.AgentForceCollectionInterval = registeredServer.AgentForceCollectionInterval;
                server.AgentHeartbeatInterval = registeredServer.AgentHeartbeatInterval;
                server.AgentLogLevel = registeredServer.AgentLogLevel;
                server.AgentMaxFolderSize = registeredServer.AgentMaxFolderSize;
                server.AgentMaxTraceSize = registeredServer.AgentMaxTraceSize;
                server.AgentMaxUnattendedTime = registeredServer.AgentMaxUnattendedTime;
                server.AgentTraceOptions = registeredServer.AgentTraceOptions;
                server.AgentVersion = registeredServer.AgentVersion;
                server.TimeLastHeartbeat = registeredServer.TimeLastHeartbeat;
            }
            else
            {
                var notAudtiedFindNotAudedtedServerRecord = FindNotAudedtedServerRecord(server, connection);

                if (notAudtiedFindNotAudedtedServerRecord != null)
                {
                    server.SrvId = ServerRecord.GetServerId(connection, server.Instance);  
                }
            }

            return server;
        }

        private bool SaveThreshold(ReportCardRecord oldValue, ReportCardRecord newValue, SqlConnection connection)
        {
            if (oldValue == null &&
                newValue.Enabled)
            {
                // We have a new threshold to write
                newValue.Write(connection);
                return true;
            }
            else if (oldValue != null &&
                (oldValue.Enabled != newValue.Enabled ||
                 oldValue.CriticalThreshold != newValue.CriticalThreshold ||
                 oldValue.WarningThreshold != newValue.WarningThreshold ||
                 oldValue.Period != newValue.Period))
            {
                // We need to update an existing record
                newValue.Update(connection);
                return true;
            }

            return false;
        }

        private bool SaveThresholds(AuditServerProperties serverProperties, SqlConnection connection)
        {
            bool isSaved = false;

            if (serverProperties.AuditThresholdsData != null &&
                serverProperties.AuditThresholdsData.ThresholdList != null)
            {
                List<ReportCardRecord> newCardRecordList = Converter.ConvertReferenceList(serverProperties.AuditThresholdsData.ThresholdList,
                    delegate(ReportCard card)
                    {
                        var cardRecord = new ReportCardRecord(card.ServerId, StatsCategory.Unknown);
                        cardRecord.StatisticId = Convert.ToInt32(card.StatisticCategory);
                        cardRecord.WarningThreshold = card.WarningThreshold;
                        cardRecord.CriticalThreshold = card.CriticalThreshold;
                        cardRecord.Period = card.Period;
                        cardRecord.Enabled = card.Enabled;
                        return cardRecord;
                    });

                var oldCardList = ReportCardRecord.GetServerReportCardEntries(connection, serverProperties.ServerId);
                foreach (var newCard in newCardRecordList)
                {
                    var oldCard = oldCardList.Find(card => card.StatisticId == newCard.StatisticId);
                    if (SaveThreshold(oldCard, newCard, connection))
                    {
                        isSaved = true;
                    }
                }
            }

            return isSaved;
        }

        private bool CheckIfServerHasAlwaysOnDatabases(int serverId, SqlConnection connection)
        {
            foreach (DatabaseRecord dbRecord in DatabaseRecord.GetDatabases(connection, serverId))
            {
                if (dbRecord.IsAlwaysOn)
                {
                    return true;
                }
            }

            return false;
        }

        private bool RemoveServerRecord(ServerRecord server, bool deleteEventsDatabase, SqlConnection connection)
        {
            bool wasRemoved = false;

            try
            {
                // remove server out of configuration tables (if no archives loaded!)
                if (server.CountLoadedArchives(connection) == 0)
                {
                    // no archives; delete it and all dbs
                    server.Delete();
                }
                else
                {
                    // mark as non-audited db since it is still hosting archives
                    ServerRecord oldSrv = server.Clone();
                    server.IsAuditedServer = false;
                    server.Write(oldSrv);

                    // delete all audit dbs
                    server.DeleteAuditData(null);
                }

                // delete events database
                if (deleteEventsDatabase)
                {
                    try
                    {
                        server.DeleteEventsDatabase();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.Write(ErrorLog.Level.Always,
                                                string.Format(Error_RemoveEventsDatabase, ex.Message),
                                                ex,
                                                ErrorLog.Severity.Error);
                    }
                }

                ServerRecord old = server.Clone();
                server.EventDatabase = string.Empty;
                server.Write(old);

                wasRemoved = true;

            }
            catch (Exception ex)
            {

                ErrorLog.Instance.Write(
                    ErrorLog.Level.Default,
                    string.Format(Error_RemoveEventsServerProblem, server.Instance, ex.Message),
                    ex,
                    ErrorLog.Severity.Error);
            }
            finally
            {
                if (wasRemoved)
                {
                    // Register change to server and perform audit log				      
                    ServerUpdate.LogChange(LogType.RemoveServer, server.Instance);

                    if (deleteEventsDatabase)
                    {
                        ServerUpdate.LogChange(LogType.RemoveEventsDatabase, server.Instance);
                    }
                }
            }
            return wasRemoved;
        }

        //AddServers helper methods
        private bool ValidateAccountName(string accountName)
        {
            string domain;
            string account;

            string tmp = accountName.Trim();

            int pos = tmp.IndexOf(@"\");
            if (pos <= 0)
            {
                return false;
            }
            else
            {
                domain = tmp.Substring(0, pos);
                account = tmp.Substring(pos + 1);

                if ((domain == "") || (account == ""))
                    return false;
            }
            return true;
        }

        private AuditServerProperties CreateDefaultServerProperties(AuditServerSettings serverSettings)
        {
            return new AuditServerProperties
            {
                GeneralProperties = new ServerGeneralProperties
                {
                    Instance = serverSettings.Instance,
                    IsClustered = serverSettings.IsVirtualServer,
                    Description = null,
                },

                AuditedActivities = new AuditActivity
                {
                    AuditLogins = false,
                    AuditLogouts = false,
                    AuditFailedLogins = true,
                    AuditSecurity = true,
                    AuditDDL = true,
                    AuditAdmin = false,
                    AuditDefinedEvents = false,
                    AuditAccessCheck = AccessCheckFilter.SuccessOnly,
                },

                PrivilegedRolesAndUsers = null,

                AuditedPrivilegedUserActivities = new AuditActivity
                {
                    AuditAllUserActivities = false,
                    AuditLogins = true,
                    AuditLogouts = true,
                    AuditFailedLogins = true,
                    AuditSecurity = true,
                    AuditAdmin = true,
                    AuditDDL = true,
                    AuditDML = false,
                    AuditSELECT = false,
                    AuditDefinedEvents = false,
                    AuditAccessCheck = AccessCheckFilter.SuccessOnly,
                    AuditCaptureSQL = false,
                    AuditCaptureTrans = false,
                    AuditCaptureDDL = false
                },

                ServerAdvancedProperties = new ServerAdvancedProperties
                {
                    DefaultDatabasePermissions = DatabaseReadAccessLevel.All,
                    SQLStatementLimit = 512,
                },
            };
        }

        private void WriteServerRecord(ServerRecord server, SqlConnection connection)
        {
            bool done;
            string errorMessage;

            var notAudtiedFindNotAudedtedServerRecord = FindNotAudedtedServerRecord(server, connection);

            if (notAudtiedFindNotAudedtedServerRecord != null)
            {
                done = server.Write(notAudtiedFindNotAudedtedServerRecord);
                errorMessage = Error_ErrorConvertingServer;
            }
            else
            {
                done = server.Create(null);
                errorMessage = Error_ErrorCreatingServer;
            }

            if (!done)
            {
                throw new Exception(string.Format("The WriteServerRecord method failed due to the following error: {0}. {1}", errorMessage, ServerRecord.GetLastError()));
            }
        }

        private AddServerStatusData ProcessEventDatabaseForServerRecord(ServerRecord server, AuditServerSettings serverSettings, SqlConnection connection)
        {
            var addServerData = new AddServerStatusData
            {
                ShouldIndexesToBeUpdated = false,
            };

            string eventsDatabase = EventDatabase.GetDatabaseName(server.Instance);

            if (!EventDatabase.DatabaseExists(eventsDatabase, connection))
            {
				// Strat SQLcm - 5.3.1 Changes for Agent deploy from Web console
                Idera.SQLcompliance.Core.Repository.ServerInstance = connection.DataSource;
                // Ends SQLcm - 5.3.1 Changes for Agent deploy from Web console

                // database doesnt already exist
                EventDatabase.Create(server.Instance,
                    eventsDatabase,
                    server.DefaultAccess,
                    connection);
            }
            else
            {
                // Existing events database case
                //if (radioDeleteDatabase.Checked || radioIncompatibleOverwrite.Checked)
                if (serverSettings.ExistingAuditData == ExistingAuditData.Delete)
                {
                    EventDatabase.InitializeExistingEventDatabase(server.Instance,
                        eventsDatabase,
                        server.DefaultAccess,
                        connection);
                    // reset watermarks
                    server.LowWatermark = DEFAULT_WATERMARK;
                    server.HighWatermark = DEFAULT_WATERMARK;
                }
                else
                {
                    // Upgrade existing database to latest version if needed
                    if (!EventDatabase.IsCompatibleSchema(eventsDatabase, connection))
                        EventDatabase.UpgradeEventDatabase(connection, eventsDatabase);

                    int schemaVersion = EventDatabase.GetDatabaseSchemaVersion(connection, eventsDatabase);

                    if (schemaVersion != CoreConstants.RepositoryEventsDbSchemaVersion)
                    {

                        addServerData.ShouldIndexesToBeUpdated = true;
                        //Form_CreateIndex frm = new Form_CreateIndex(true);
                        //if (frm.ShowDialog(this) == DialogResult.OK)
                        //{
                        //EventDatabase.UpdateIndexes(connection, eventsDatabase);
                        //}
                        //else
                        //{
                        //SQLcomplianceConfiguration config = new SQLcomplianceConfiguration();
                        //config.Read(Globals.Repository.Connection);

                        //if (config.IndexStartTime == DateTime.MinValue)
                        //{
                        //    Form_IndexSchedule schedule = new Form_IndexSchedule();
                        //    schedule.ShowDialog(this);
                        //    config.IndexStartTime = schedule.IndexStartTime;
                        //    config.IndexDuration = schedule.IndexDuration;
                        //    config.Write(connection);
                        //}
                        //SetReindexFlag(true);
                    }
                }

                // set watermarks to first and last record in existing database
                int lowWatermark;
                int highWatermark;

                EventDatabase.GetWatermarks(eventsDatabase,
                    out lowWatermark,
                    out highWatermark,
                    connection);

                server.LowWatermark = lowWatermark;

                if (server.LowWatermark != DEFAULT_WATERMARK)
                    server.LowWatermark--;
                server.HighWatermark = highWatermark;
            }

            // Update SystemDatabase Table
            EventDatabase.AddSystemDatabase(server.Instance, eventsDatabase, connection);

            server.EventDatabase = eventsDatabase;

            return addServerData;
        }

        private AddServerStatusData CreateServer(AuditServerSettings serverSettings, SQLcomplianceConfiguration config, SqlConnection connection)
        {
            AddServerStatusData statusData = null;
            // Create events database
            ServerRecord server = null;

            try
            {
                var serverProperties = CreateDefaultServerProperties(serverSettings);
                serverProperties.GeneralProperties.Description = serverSettings.Description;
                server = CreateNewServerRecord(serverProperties, serverSettings.AgentDeploymentProperties, config, connection);
                SetSqlServerPropertites(server);
                statusData = ProcessEventDatabaseForServerRecord(server, serverSettings, connection);
                WriteServerRecord(server, connection);

                string snapshot = Snapshot.ServerSnapshot(connection, server, false);

                ServerUpdate.RegisterChange(server.SrvId,
                                                LogType.NewServer,
                                                server.Instance,
                                                snapshot,
                                                connection);

                AgentStatusMsg.LogStatus(server.AgentServer,
                                            server.Instance,
                                            AgentStatusMsg.MsgType.Registered,
                                            connection);

                statusData.Server = server;
            }
            catch (Exception ex)
            {
                LogError(string.Format("{0}{1}", Error_CantCreateEventsDatabase, ex));
                statusData = null;
            }

            return statusData;
        }

        private void SetSqlServerPropertites(ServerRecord server)
        {
            var sqlServer = new SqlDirect();
            if (sqlServer.OpenConnection(server.Instance))
            try
            {
                if (sqlServer.OpenConnection(server.Instance))
                {
                    ArrayList values = SQLHelpers.GetServerProperties(sqlServer.Connection, "isClustered", "isHadrEnabled");

                    if (values != null)
                    {
                        //server.isClustered = Convert.ToBoolean(values[0]);
                        server.IsHadrEnabled = Convert.ToBoolean(values[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format("IsClustered Enabled Error :", ex.Message));
            }
            finally
            {
                sqlServer.CloseConnection();
            }
        }

        private void ValidateServerSettings(AuditServerSettings serverSettings, SqlConnection connection, SQLcomplianceConfiguration config)
        {
            var serverToAdd = SqlCmRecordReader.FindRegisteredAudedtedServerRecord(serverSettings.Instance, connection);

            if (serverToAdd != null)
            {
                LogAndThrowException(Error_ServerAlreadyRegistered);
            }

            // Agent Settings
            var registeredServer = SqlCmRecordReader.FindRegisteredAudedtedServerRecord(serverSettings.Instance.Split('\\')[0], connection);
            var alreadyDeployedAgentProperties = GetAgentPropertiesOfRegisteredServer(serverSettings.Instance, registeredServer, config);

            if (alreadyDeployedAgentProperties != null)
            {
                switch (serverSettings.AgentDeployStatus)
                {
                    case AgentDeployStatus.Now:
                        if (alreadyDeployedAgentProperties.IsDeployed ||
                            alreadyDeployedAgentProperties.IsDeployedManually)
                        {
                            LogAndThrowException("Not valid Agent Properties for deploying agent now because agent was deployed.");
                        }

                        if (serverSettings.AgentDeploymentProperties.AgentServiceCredentials == null ||
                            !ValidateCredentials(serverSettings.AgentDeploymentProperties.AgentServiceCredentials))
                        {
                            LogAndThrowException("Not valid agent service credentials for deploying agent now.");
                        }

                        if (!string.IsNullOrEmpty(serverSettings.AgentDeploymentProperties.AgentTraceDirectory))
                        {
                        PathHelper.ValidateTraceDirectoryPath(serverSettings.AgentDeploymentProperties.AgentTraceDirectory);
                        }
                        break;

                    case AgentDeployStatus.Manually:
                        if (alreadyDeployedAgentProperties.IsDeployed ||
                            alreadyDeployedAgentProperties.IsDeployedManually)
                        {
                            LogAndThrowException("Not valid Agent Properties for deploying agent manually because agent was deployed.");
                        }
                        break;

                    case AgentDeployStatus.Later:
                        if (alreadyDeployedAgentProperties.IsDeployed ||
                            alreadyDeployedAgentProperties.IsDeployedManually)
                        {
                            LogAndThrowException("Not valid Agent Properties for deploying agent later because agent was deployed.");
                        }
                        break;

                    case AgentDeployStatus.AlreadyDeployed:
                        if (!alreadyDeployedAgentProperties.IsDeployed )
                        {
                            LogAndThrowException("Not valid Agent Properties for deploying agent now because agent was deployed.");
                        }
                        break;
                }
            }
        }

        private void ProcessAgentDeploying(AuditServerSettings serverSettings, ServerRecord server, SQLcomplianceConfiguration config, SqlConnection connection)
        {
            switch (serverSettings.AgentDeployStatus)
            {
                case AgentDeployStatus.AlreadyDeployed:
                    {
                        if (server.IsDeployed)
                        {

                            server.IsRunning = AgentManagerHelper.Instance.ActivateAgentViaFirewall(server.Instance, config);
                            server.IsDeployed = server.IsRunning;

                            ServerRecord.SetIsFlags(server.Instance,
                                server.IsDeployed,
                                server.IsDeployedManually,
                                server.IsRunning,
                                server.IsCrippled,
                                connection);
                        }
                        break;
                    }

                case AgentDeployStatus.Now:
                    {
                        if (!server.IsDeployed)
                        {
                            server.IsDeployed = true;

                            ServerRecord.SetIsFlags(server.Instance,
                                server.IsDeployed,
                                server.IsDeployedManually,
                                server.IsRunning,
                                server.IsCrippled,
                                connection);

                            if (AgentManagerHelper.Instance.InstallAgent(server, serverSettings.AgentDeploymentProperties, config))
                            {
                                // agent deployed - update all instances on that computer
                                var servers = ServerRecord.GetServers(connection, true);

                                foreach (ServerRecord srvrec in servers)
                                {
                                    if (srvrec.InstanceServer.ToUpper() == server.InstanceServer.ToUpper())
                                    {
                                        ServerRecord oldServerState = srvrec.Clone();
                                        if (oldServerState.IsDeployed)
                                        {
                                            srvrec.IsRunning = true;
                                            srvrec.IsDeployed = true;
                                            srvrec.AgentTraceDirectory = server.AgentTraceDirectory;
                                            srvrec.AgentServiceAccount = server.AgentServiceAccount;
                                            srvrec.Write(oldServerState);

                                            LogRecord.WriteLog(connection,
                                                LogType.DeployAgent,
                                                srvrec.Instance,
                                                "SQLcompliance Agent deployed");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                server.IsDeployed = false;
                                //_deploymentSuccessful = false;

                                ServerRecord.SetIsFlags(server.Instance,
                                    server.IsDeployed,
                                    server.IsDeployedManually,
                                    server.IsRunning,
                                    server.IsCrippled,
                                    connection);
                            }
                        }

                        break;
                    }
            }

            AgentManagerHelper.Instance.PingAgent(server);
        }

        private AddServerStatusData AddServer(AuditServerSettings serverSettings, SQLcomplianceConfiguration config, SqlConnection connection)
        {
            ValidateServerSettings(serverSettings, connection, config);

            if (!LicenseHelper.LicenseAllowsMoreInstances(connection, config))
            {
                LogAndThrowException("AddServer: License doesn't allow to add more instances.");
            }

            var addServerStatusData = CreateServer(serverSettings, config, connection);

            if (addServerStatusData == null ||
                addServerStatusData.Server == null)
            {
                LogAndThrowException(string.Format("AddServer: Failed to create server record due to the following error {0}", ServerRecord.GetLastError()));
            }

            return addServerStatusData;
        }

        private AddServerStatus AddServerAndProcessAgentDeploy(AuditServerSettings serverSettings, SQLcomplianceConfiguration config, SqlConnection connection)
        {
            serverSettings.Instance = GetTranslatedInstanceName(serverSettings.Instance);

            var addStatus = new AddServerStatus
            {
                Instance = serverSettings.Instance,
                ShouldIndexesToBeUpdated = false,
                ErrorMessage = string.Empty,
            };

            try
            {
                var addStatusData = AddServer(serverSettings, config, connection);
                addStatus.ShouldIndexesToBeUpdated = addStatusData.ShouldIndexesToBeUpdated;
                addStatus.ServerId = addStatusData.Server.SrvId;
                ProcessAgentDeploying(serverSettings, addStatusData.Server, config, connection);
                addStatus.WasAgentDeployedAutomatically = addStatusData.Server.IsDeployed;
                addStatus.WasSuccessfullyAdded = true;
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                LogError(error);
                addStatus.ErrorMessage = error;
                addStatus.WasSuccessfullyAdded = false;
            }

            return addStatus;
        }

        private AgentDeployStatus GetAgentDeployStatus(AgentDeploymentProperties agentProperties)
        {
            var agentDeployStatus = AgentDeployStatus.Later;

            if (agentProperties.IsDeployed)
            {
                agentDeployStatus = AgentDeployStatus.AlreadyDeployed;
            }
            else if (agentProperties.IsDeployedManually)
            {
                agentDeployStatus = AgentDeployStatus.Manually;
            }

            return agentDeployStatus;
        }

        private AgentDeploymentProperties GetAgentPropertiesOfRegisteredServer(string instanceName, ServerRecord registeredServer, SQLcomplianceConfiguration config)
        {
            var agentProperties = new AgentDeploymentProperties();

            if (registeredServer != null &&
                registeredServer.IsDeployed)
            {
                agentProperties.IsDeployed = registeredServer.IsDeployed;
                agentProperties.IsDeployedManually = registeredServer.IsDeployedManually;
                agentProperties.AgentTraceDirectory = registeredServer.AgentTraceDirectory;
                agentProperties.AgentServiceCredentials = new Credentials
                {
                    Account = registeredServer.AgentServiceAccount,
                };
            }
            else
            {
                var instanceServer = GetInstanceServer(instanceName);
                agentProperties.IsDeployed = IsRepositoryHost(instanceServer, config);
                agentProperties.IsDeployedManually = agentProperties.IsDeployed;
            }

            return agentProperties;
        }
        private AuditActivity GetDefaultAuditUserActivitySettings()
        {
            return new AuditActivity
            {
                AuditAllUserActivities = false,
                AuditLogins = true,
                AuditLogouts = true,
                AuditFailedLogins = true,
                AuditSecurity = true,
                AuditAdmin = true,
                AuditDDL = true,
                AuditAccessCheck = AccessCheckFilter.SuccessOnly,
            };
        }
        private AuditActivity GetDefaultAuditUserActivitySettings(AuditDatabaseSettings auditedDatabaseSettings)
        {
            return new AuditActivity
            {
                AuditAllUserActivities = auditedDatabaseSettings.UserAuditedActivities.AuditAllUserActivities,
                AuditLogins = auditedDatabaseSettings.UserAuditedActivities.AuditLogins,
                AuditLogouts = auditedDatabaseSettings.UserAuditedActivities.AuditLogouts,
                AuditFailedLogins = auditedDatabaseSettings.UserAuditedActivities.AuditFailedLogins,
                AuditSecurity = auditedDatabaseSettings.UserAuditedActivities.AuditSecurity,
                AuditAdmin = auditedDatabaseSettings.UserAuditedActivities.AuditAdmin,
                AuditDDL = auditedDatabaseSettings.UserAuditedActivities.AuditDDL,
                AuditAccessCheck = AccessCheckFilter.SuccessOnly,
                AllowCaptureSql = auditedDatabaseSettings.UserAuditedActivities.AllowCaptureSql,
                AuditBeforeAfter=auditedDatabaseSettings.UserAuditedActivities.AuditBeforeAfter,
                AuditCaptureDDL=auditedDatabaseSettings.UserAuditedActivities.AuditCaptureDDL,
                AuditCaptureSQL=auditedDatabaseSettings.UserAuditedActivities.AuditCaptureSQL,
                AuditCaptureSQLXE=auditedDatabaseSettings.UserAuditedActivities.AuditCaptureSQLXE,
                AuditCaptureTrans=auditedDatabaseSettings.UserAuditedActivities.AuditCaptureTrans,
                AuditDefinedEvents=auditedDatabaseSettings.UserAuditedActivities.AuditDefinedEvents,
                AuditDML=auditedDatabaseSettings.UserAuditedActivities.AuditDML,
                AuditPrivilegedUsers=auditedDatabaseSettings.UserAuditedActivities.AuditPrivilegedUsers,
                AuditSELECT=auditedDatabaseSettings.UserAuditedActivities.AuditSELECT,
                AuditSensitiveColumns=auditedDatabaseSettings.UserAuditedActivities.AuditSensitiveColumns,
                AuditUserExtendedEvents=auditedDatabaseSettings.UserAuditedActivities.AuditUserExtendedEvents,
                CustomEnabled=auditedDatabaseSettings.UserAuditedActivities.CustomEnabled,
                IsAgentVersionSupported=auditedDatabaseSettings.UserAuditedActivities.IsAgentVersionSupported,
                IsAuditLogEnabled = auditedDatabaseSettings.UserAuditedActivities.IsAuditLogEnabled,
            };
        }

        private void ApplyRegulationSettingsForServerRecord(ServerRecord srv, AuditRegulationSettings regulationSettings, SqlConnection connection)
        {
            RegulationSettings settings;
            var regulationCategoryDictionary = RegulationSettingsHelper.Instance.GetCategoryDictionary(connection);
            var tempServer = new ServerRecord();


            // apply PCI
            if (regulationSettings.PCI)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins);
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts);
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins);
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition);
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity);
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges);
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined);

                    if ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }

            //// apply HIPAA
            //// the OR against the server settings is done because the user can select more than one template.  When more than one template is 
            //// selected, the options are combined together
            if (regulationSettings.HIPAA)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = false;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.DISA)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.DISA, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.NERC)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.NERC, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE; 

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.CIS)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.CIS, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE; 

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.SOX)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.SOX, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE; 

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.FERPA)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.FERPA, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.GDPR)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditUDE = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditUDE;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            //return the settings
            srv.AuditLogins = tempServer.AuditLogins;
            srv.AuditLogouts = tempServer.AuditLogouts;
            srv.AuditFailedLogins = tempServer.AuditFailedLogins;
            srv.AuditDDL = tempServer.AuditDDL;
            srv.AuditAdmin = tempServer.AuditAdmin;
            srv.AuditSecurity = tempServer.AuditSecurity;
            srv.AuditUDE = tempServer.AuditUDE;
            srv.AuditAccessCheck = tempServer.AuditAccessCheck;
        }

        private ServerRecord CreateModifiedServerRecord(ServerRecord oldServer, AuditDatabaseSettings auditDatabaseSeetings, SqlConnection connection)
        {
            var serverSettings = auditDatabaseSeetings.ServerSettingsToBeUpdated;
            var newServer = SqlCmRecordReader.CloneRecord(oldServer, connection);
            if (serverSettings != null)
                newServer.DefaultAccess = Convert.ToInt32(serverSettings.DatabasePermissions);

            if (newServer.IsAuditedServer)
            {
                if (auditDatabaseSeetings.CollectionLevel == AuditCollectionLevel.Regulation)
                {
                    if (auditDatabaseSeetings.AuditedActivities != null && auditDatabaseSeetings.AuditedServerActivities != null) 
                    {
                        UpdateCustomServerRecordWithAuditActivities(auditDatabaseSeetings.AuditedServerActivities, newServer);
                    }
                    else
                        ApplyRegulationSettingsForServerRecord(newServer, auditDatabaseSeetings.RegulationSettings, connection);
                }
                else
                    UpdateServerRecordWithAuditActivities(serverSettings.ServerAuditedActivities, newServer);

                // Privileged users
                if (auditDatabaseSeetings.PrivilegedRolesAndUsers != null
                    && ((auditDatabaseSeetings.PrivilegedRolesAndUsers.RoleList != null && auditDatabaseSeetings.PrivilegedRolesAndUsers.RoleList.Count > 0)
                    || (auditDatabaseSeetings.PrivilegedRolesAndUsers.UserList != null && auditDatabaseSeetings.PrivilegedRolesAndUsers.UserList.Count > 0)
                    ))
                {

                    var auditUserActivities = GetDefaultAuditUserActivitySettings(auditDatabaseSeetings);
                    UpdateServerRecordWithUserAuditingActivitiesAndRoleUsers(auditUserActivities, auditDatabaseSeetings.PrivilegedRolesAndUsers, newServer);
                }
                else
                {
                    var auditUserActivities = GetDefaultAuditUserActivitySettings();
                    UpdateServerRecordWithUserAuditingActivitiesAndRoleUsers(auditUserActivities, auditDatabaseSeetings.PrivilegedRolesAndUsers, newServer);
                }
            }

            return newServer;
        }


        //Apply Custom Rregulation Audit Setting

        private void UpdatedCustomRegulationServerAuditActivities(ServerRecord record, AuditActivity activity)
        {
            record.AuditLogins = activity.AuditLogins;
            record.AuditLogouts = activity.AuditLogouts;
            record.AuditFailedLogins = activity.AuditFailedLogins;
            record.AuditAdmin = activity.AuditAdmin;
            record.AuditDDL = activity.AuditDDL;
            record.AuditSecurity = activity.AuditSecurity;
            record.AuditUDE = activity.AuditDefinedEvents;
            record.AuditAccessCheck = activity.AuditAccessCheck;
        }

        private bool WriteNewServerRecord(ServerRecord newServer, ServerRecord oldServer, SqlConnection connection)
        {
            string errorMsg = "";
            bool retval = false;
            bool isDirty = false;

            try
            {
                //---------------------------------------
                // Write Server Properties if necessary
                //---------------------------------------
                if (!ServerRecord.Match(newServer, oldServer))
                {
                    if (!newServer.Write(oldServer))
                    {
                        errorMsg = ServerRecord.GetLastError();
                        throw (new Exception(errorMsg));
                    }
                    else
                    {
                        // update default security
                        if (newServer.DefaultAccess != oldServer.DefaultAccess)
                        {
                            EventDatabase.SetDefaultSecurity(oldServer.EventDatabase,
                                newServer.DefaultAccess,
                                oldServer.DefaultAccess,
                                false,
                                connection);
                        }

                        isDirty = true;
                    }
                }
                retval = true;
            }
            catch (Exception ex)
            {
                LogError(string.Format("WriteNewServerRecord method failed: {0}", ex));
            }
            finally
            {
                //-----------------------------------------------------------
                // Cleanup - Close transaction, update server, display error
                //-----------------------------------------------------------
                if (retval && isDirty && oldServer.IsAuditedServer)
                {
                    string changeLog = Snapshot.ServerChangeLog(oldServer, newServer);

                    // Register change to server and perform audit log				      
                    ServerUpdate.RegisterChange(newServer.SrvId,
                        LogType.ModifyServer,
                        newServer.Instance,
                        changeLog, connection);

                    // in case agent properties were updated, update all instances
                    //newServer.CopyAgentSettingsToAll(oldServer);

                    newServer.ConfigVersion++;
                }
                //if (!retval)
                //{
                //   ErrorMessage.Show(this.Text,
                //                     UIConstants.Error_ErrorSavingServer,
                //                     errorMsg);
                //}
            }

            return retval;
        }

        #endregion

        #region Public Methods
        public string GetInstanceVersion(ServerRecord server)
        {
            switch (server.SqlVersion)
            {
                case 8:
                    return "2000";
                case 9:
                    return "2005";
                case 10:
                    return "2008";
                case 11:
                    return "2012";
                case 12:
                    return "2014";
                case 13:
                    return "2016"; // Date 7/15/2016 4.1.7. Support SQL Server 2016
				case 14:
                    return "2017"; //Support SQL Server 2017	
            }

            return "Unknown";
        }

        public static ServerRolesAndUsers GetServerRolesAndUsersFromString(string serverRolesAndUsersString, string exceptionMessage)
        {
            var rolesAndUsers = new ServerRolesAndUsers();

            try
            {
                var userList = new UserList(serverRolesAndUsersString);

                rolesAndUsers.UserList =
                    Converter.ConvertList<Login, ServerLogin>(userList.Logins, login => new ServerLogin()
                    {
                        Sid = Convert.ToBase64String(login.Sid),
                        Name = login.Name,
                    });

                rolesAndUsers.RoleList =
                    Converter.ConvertList<Idera.SQLcompliance.Core.ServerRole, ServerRole>(userList.ServerRoles, role => new ServerRole()
                    {
                        Id = role.Id,
                        Name = role.Name,
                        FullName = role.FullName,
                    });

            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always,
                                        string.Format("{0}. Error: {1} ", exceptionMessage, ex),
                                        ErrorLog.Severity.Error);
            }

            return rolesAndUsers;
        }

        public static string GetRolesAndUsersString(ServerRolesAndUsers serverRolesAndUsers)
        {
            if (serverRolesAndUsers == null ||
               ((serverRolesAndUsers.RoleList == null || serverRolesAndUsers.RoleList.Count == 0) &&
                (serverRolesAndUsers.UserList == null || serverRolesAndUsers.UserList.Count == 0)))
            {
                return string.Empty;
            }

            var userList = new UserList();

            foreach (var role in serverRolesAndUsers.RoleList)
            {
                userList.AddServerRole(role.Name, role.FullName, role.Id);
            }

            foreach (var user in serverRolesAndUsers.UserList)
            {
                userList.AddLogin(user.Name, Convert.FromBase64String(user.Sid));
            }

            return userList.ToString();
        }

        public AuditServerProperties GetAuditServerProperties(int serverId, SqlConnection connection)
        {
            var serverProperties = new AuditServerProperties();
            var server = SqlCmRecordReader.GetServerRecord(serverId, connection);

            try
            {
                serverProperties.ServerId = server.SrvId;
                SetGeneralProperties(serverProperties, server, connection);
                SetAuditActivities(serverProperties, server);
                SetPrivilegedUserAuditing(serverProperties, server);
                SetAuditingThresholds(serverProperties, server, connection);
                SetAdvancedSettings(serverProperties, server);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always,
                                        string.Format("GetAuditDatabaseProperties REST mehtod failed due to the following error: {1} ", ex),
                                        ErrorLog.Severity.Error);
            }

            return serverProperties;
        }

        public bool UpdateAuditServerProperties(AuditServerProperties serverProperties, SqlConnection connection)
        {
            bool isDirty = false;
            bool retval = false;
            var oldServer = SqlCmRecordReader.GetServerRecord(serverProperties.ServerId, connection);
            ValidateProperties(serverProperties, oldServer);
            var server = CreateServerRecord(serverProperties, oldServer);
            server.Connection = connection;
            Exception internalException = null;

            try
            {
                SaveThresholds(serverProperties, connection);// SaveThresholds();
                //---------------------------------------
                // Write Server Properties if necessary
                //---------------------------------------
                if (!ServerRecord.Match(server, oldServer))
                {
                    if (!server.Write(oldServer))
                    {
                        LogAndThrowException(ServerRecord.GetLastError());
                    }
                    else
                    {
                        // update default security
                        if (server.DefaultAccess != oldServer.DefaultAccess)
                        {
                            EventDatabase.SetDefaultSecurity(oldServer.EventDatabase,
                                                             server.DefaultAccess,
                                                             oldServer.DefaultAccess,
                                                             false,
                                                             connection);
                        }

                        isDirty = true;
                    }
                }
                retval = true;
            }
            catch (Exception ex)
            {
                internalException = ex;
                retval = false;
            }
            finally
            {
                //-----------------------------------------------------------
                // Cleanup - Close transaction, update server, display error
                //-----------------------------------------------------------
                if (retval && isDirty && oldServer.IsAuditedServer)
                {
                    string changeLog = Snapshot.ServerChangeLog(oldServer, server);

                    // Register change to server and perform audit log				      
                    ServerUpdate.RegisterChange(server.SrvId, LogType.ModifyServer, server.Instance, changeLog, connection);

                    // in case agent properties were updated, update all instances
                    server.ConfigVersion++;
                }
            }

            if (!retval)
            {
                LogAndThrowException(string.Format("{0}. Failed to update audit server properties for server id {0} due to error: {1}", Error_ErrorSavingServer, serverProperties.ServerId), internalException);
            }

            return retval;
        }

        public List<AuditedServerInfo> GetAuditedInstances(bool auditServersOnly, SqlConnection connection)
        {
            try
            {
                var serverRecordList = ServerRecord.GetServers(connection, auditServersOnly, true);
                var serverInfoList = Converter.ConvertReferenceList(serverRecordList,
                                    delegate(ServerRecord server)
                                    {
                                        return new AuditedServerInfo
                                        {
                                            ServerId = server.SrvId,
                                            Name = server.Instance,
                                            IsAuditedServer = server.IsAuditedServer,
                                        };

                                    });

                return serverInfoList;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            "Loading audited servers.",
                                            ex,
                                            ErrorLog.Severity.Warning);
                return null;
            }
        }

        public List<string> GetAllAuditedInstances(SqlConnection connection)
        {
            try
            {
                var serverList = ServerRecord.GetAllServers(connection);

                return serverList;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            "Loading audited servers.",
                                            ex,
                                            ErrorLog.Severity.Warning);
                return null;
            }
        }

        public List<string> GetAllNotRegisteredInstanceNameList(SqlConnection connection)
        {
            var serverList = new List<string>();

            // loading logic
            try
            {
                SqlDataSourceEnumerator enumerator = SqlDataSourceEnumerator.Instance;
                using (DataTable tbl = enumerator.GetDataSources())
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        string server = row["ServerName"].ToString();
                        string instance = row["InstanceName"].ToString();
                        string fullName = (instance == null || instance.Length == 0) 
                            ? server : string.Format("{0}\\{1}", server, instance);
                        fullName = fullName.ToUpper();

                        if (fullName == "(LOCAL)")
                        {
                            fullName = GetLocalHostName();
                        }

                        bool audited = ServerRecord.ServerIsAudited(fullName.ToUpper(), connection);

                        if (!audited)
                        {
                            serverList.Add(fullName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always,
                                        string.Format("{0}: {1} ",Error_DMOLoadServers, ex),
                                        ErrorLog.Severity.Error);
            }

            serverList.Sort();
            return serverList;
        }

        public List<AddServerStatus> AddServers(List<AuditServerSettings> serverSettingsList, SqlConnection connection)
        {
            var addStatusList = new List<AddServerStatus>();
            var config = SqlCmConfigurationHelper.GetConfiguration(connection);

            foreach (var serverSettings in serverSettingsList)
            {
                if (!LicenseHelper.LicenseAllowsMoreInstances(connection)) throw new Exception("You have already reached the maximum number of registered SQL Server allowed by the current license.");

                var addStatus = AddServerAndProcessAgentDeploy(serverSettings, config, connection);
                addStatusList.Add(addStatus);
            }

            return addStatusList;
        }

        public List<AddServerStatus> ImportServerInstances(List<string> instanceList, SqlConnection connection)
        {
            instanceList = GetTranslatedInstancesNames(instanceList);

            var addStatusList = new List<AddServerStatus>();
            var config = SqlCmConfigurationHelper.GetConfiguration(connection);

            foreach (var instanceName in instanceList)
            {
                var registeredServer = SqlCmRecordReader.FindServerRecord(instanceName, connection);

                var serverSettings = new AuditServerSettings
                {
                    Instance = instanceName,
                    ExistingAuditData = ExistingAuditData.Keep,
                    AgentDeploymentProperties = GetAgentPropertiesOfRegisteredServer(instanceName, registeredServer, config),
                };

                serverSettings.AgentDeployStatus = GetAgentDeployStatus(serverSettings.AgentDeploymentProperties);

                if (registeredServer != null)
                {
                    serverSettings.IsVirtualServer = registeredServer.isClustered;
                }

                var addStatus = AddServerAndProcessAgentDeploy(serverSettings, config, connection);
                addStatusList.Add(addStatus);
            }

            return addStatusList;
        }

        public List<RemoveServerStatus> RemoveServers(RemoveServersRequest removeServersRequest, SqlConnection connection)
        {
            if (removeServersRequest.ServerIdList == null ||
                removeServersRequest.ServerIdList.Count == 0)
            {
                throw new Exception("Please specify ids of servers to be removed.");
            }

            var removeStatusList = new List<RemoveServerStatus>(removeServersRequest.ServerIdList.Count);

            foreach (var serverId in removeServersRequest.ServerIdList)
            {
                var removeStatus = new RemoveServerStatus
                {
                    ServerId = serverId,
                    WasRemoved = false,
                    ErrorMessage = string.Empty,
                };

                try
                {
                    var server = SqlCmRecordReader.GetServerRecord(serverId, connection);

                    removeStatus.WasAgentDeactivated = AgentManagerHelper.Instance.DeactivateAgent(server, removeServersRequest.DeleteEventsDatabase, connection);
                    if (!removeStatus.WasAgentDeactivated)
                    {
                        var message = string.Format("Couldn't deactivate agent for server instance {0}", server.Instance);
                        LogWarning(message);
                    }

                    removeStatus.WasRemoved = RemoveServerRecord(server, removeServersRequest.DeleteEventsDatabase, connection);

                    if (removeStatus.WasRemoved)
                    {
                        // Register change to server and perform audit log				      
                        AgentStatusMsg.LogStatus(server.AgentServer, server.Instance, AgentStatusMsg.MsgType.Unregistered, connection);
                        AgentManagerHelper.Instance.UninstallAgent(server, connection);
                    }
                }
                catch (Exception ex)
                {
                    if (!removeStatus.WasRemoved)
                    {
                        removeStatus.ErrorMessage = string.Format("Failed to remove successfully server with id {0} due to the following error {1}", serverId, ex.Message);
                        LogError((new Exception(removeStatus.ErrorMessage, ex)).ToString());                        
                    }
                    else
                    {
                        LogWarning(ex.ToString());    
                    }
                }

                removeStatusList.Add(removeStatus);
            }

            return removeStatusList;
        }

        public void EnableAuditingForServers(EnableAuditForServers servers, SqlConnection connection)
        {
            LogType logType = servers.Enable ? LogType.EnableServer : LogType.DisableServer;
            string actionString = servers.Enable ? "enable" : "disable";

            foreach (var serverId in servers.ServerIdList)
            {
                var server = SqlCmRecordReader.GetServerRecord(serverId, connection);

                if (server == null)
                {
                    LogError(string.Format("Failed get server record for server id {0}", serverId));   
                }

                if (server.EnableServer(servers.Enable))
                {
                    ServerUpdate.RegisterChange(server.SrvId, logType, server.Instance);
                }
                else
                {
                    LogError(string.Format("Failed to {0} server with id = {1}. Error: {2} ", actionString, serverId, ServerRecord.GetLastError()));
                }
            }
        }

        public AgentDeploymentProperties GetAgentDeploymentPropertiesForInstance(string instance, SqlConnection connection)
        {
            instance = GetTranslatedInstanceName(instance);
            var server = instance.Split('\\')[0];

            var config = SqlCmConfigurationHelper.GetConfiguration(connection);
            var registeredServer = SqlCmRecordReader.FindServerRecord(server, connection);
            return GetAgentPropertiesOfRegisteredServer(server, registeredServer, config);
        }

        public InstanceAvailableResponse IsInstanceAvailable(string instance, SqlConnection connection)
        {
            instance = GetTranslatedInstanceName(instance);

            var response = new InstanceAvailableResponse
            {
                IsAvailable = false,
                ErrorMessage = string.Empty,
            };

            try
            {
                var agentManager = AgentManagerHelper.Instance.GetAgentManagerProxy(connection);
                response.IsAvailable = agentManager.Ping(instance);
            }
            catch (Exception ex)
            {

                ErrorLog.Instance.Write(ErrorLog.Level.Verbose,
                                            String.Format("IsInstanceAvailable method: Pinging agent for instance '{0}'", instance),
                                            ex,
                                            ErrorLog.Severity.Warning);
                response.ErrorMessage = ex.ToString();
            }

            // straight connection to SQL Server
            if (!response.IsAvailable)
            {
                var sqlServer = new SqlDirect();
                response.IsAvailable = sqlServer.OpenConnection(instance);
                if (response.IsAvailable)
                {
                    response.ErrorMessage = string.Empty;
                    sqlServer.CloseConnection();    
                }
                else
                {
                    response.ErrorMessage += string.Format(". Also failed to connect directly to {0} due to the error: {1}", instance, sqlServer.GetLastError());
                }
            }

            return response;
        }

        public void ValidateAuditedActivitiesForRolesAndUsers(ServerRolesAndUsers rolesAndUsers, AuditActivity activity)
        {
            if ((rolesAndUsers.RoleList.Count > 0 ||
                rolesAndUsers.UserList.Count > 0) &&
                !activity.AuditAllUserActivities &&
                !activity.AuditLogins &&
                !activity.AuditLogouts &&
                !activity.AuditFailedLogins &&
                !activity.AuditSecurity &&
                !activity.AuditAdmin &&
                !activity.AuditDDL &&
                !activity.AuditDML &&
                !activity.AuditSELECT &&
                !activity.AuditDefinedEvents)
            {
                LogAndThrowException(Error_MustSelectOneAuditUserOption);
            }
        }

        public bool ValidateCredentials(Credentials credentials)
        {
            try
            {
                return ValidateAccountName(credentials.Account) &&
                        InstallUtil.VerifyPassword(credentials.Account, credentials.Password) == 0;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.Write(ErrorLog.Level.Always,
                        string.Format(Error_NoInstallUtilLib, ex),
                        ErrorLog.Severity.Error);
                return false;
            }
        }

        public IEnumerable<CredentialValidationResult> ValidateCredentials(ManagedCredentials credentials, IEnumerable<ManagedServerInstance> serverInstances)
        {
            var result = new List<CredentialValidationResult>();
            Parallel.ForEach(serverInstances, instance => result.Add(ValidateCredentials(credentials, instance)));

            return result;
        }

        private CredentialValidationResult ValidateCredentials(ManagedCredentials credentials, ManagedServerInstance instance)
            {
            var status = new CredentialValidationResult {Instance = instance.Instance};

            switch (credentials.AccountType)
                {
                case SqlServerSecurityModel.Integrated:
                    status.IsValid = true;
                    break;
                case SqlServerSecurityModel.IntegratedWindowsImpersonation:
                case SqlServerSecurityModel.User:
                    ValidateCredentials(credentials, instance, status);
                    break;
            }
            return status;
            }

        private static void ValidateCredentials(ManagedCredentials credentials, ManagedServerInstance instance, CredentialValidationResult status)
        {
            try
            {
                status.IsValid = instance.IsCredentialsValid(credentials);
            }
            catch (Exception e)
            {
                status.ErrorMessage = e.Message;
            }
        }

        public InstanceRegisteredStatus CheckInstanceRegisteredStatus(CheckInstanceRegisteredRequest request, SqlConnection connection)
        {
            try
            {
                request.Instance = GetTranslatedInstanceName(request.Instance);

                string eventsDatabase = EventDatabase.GetDatabaseName(request.Instance);
                bool doesEventDatabaseExist = EventDatabase.DatabaseExists(eventsDatabase, connection);

                var registeredStatus = new InstanceRegisteredStatus
                {
                    RegisteredStatus = RegisteredStatus.NotRegistered,
                    EventDatabaseName = doesEventDatabaseExist ? eventsDatabase : string.Empty,
                };

                var foundRegisteredServer = SqlCmRecordReader.FindRegisteredAudedtedServerRecord(request.Instance, connection);

                if (foundRegisteredServer != null)
                {
                    registeredStatus.RegisteredStatus = RegisteredStatus.IsRegistered;
                }
                else if (doesEventDatabaseExist)
                {
                    registeredStatus.RegisteredStatus = RegisteredStatus.WasRegistered;
                }

                return registeredStatus;
            }
            catch (Exception ex)
            {
                LogAndThrowException("CheckInstanceRegisteredStatus REST method failed.", ex);
                throw;
            }
        }

        public bool SaveServerAuditSettings(AuditDatabaseSettings auditDatabaseSeetings, SqlConnection connection)
        {
            var serverSettings = auditDatabaseSeetings.ServerSettingsToBeUpdated;
            if (auditDatabaseSeetings.UpdateServerSettings &&
                serverSettings == null)
            {
                LogAndThrowException("The ServerSettingsToBeUpdated property can't be null when UpdateServerSettings = true.");
            }

            var privilegedRolesAndUsers = auditDatabaseSeetings.PrivilegedRolesAndUsers;
            var serverAuditActivites = auditDatabaseSeetings.ServerSettingsToBeUpdated.ServerAuditedActivities;
            ValidateAuditedActivitiesForRolesAndUsers( privilegedRolesAndUsers, serverAuditActivites);
            var oldServer = SqlCmRecordReader.GetServerRecord(serverSettings.ServerId, connection);
            var newServer = CreateModifiedServerRecord(oldServer, auditDatabaseSeetings, connection);
            return WriteNewServerRecord(newServer, oldServer, connection);
        }
        #endregion

        public bool SaveServerRegulationAuditSetting(AuditDatabaseSettings auditDatabaseSeetings, SqlConnection connection)
        {
            var server = SqlCmRecordReader.GetServerRecord(auditDatabaseSeetings.DatabaseList[0].ServerId, connection);
            var privilegedRolesAndUsers = auditDatabaseSeetings.PrivilegedRolesAndUsers;
            var serverAuditActivites = auditDatabaseSeetings.UserAuditedActivities;
            if (privilegedRolesAndUsers != null && serverAuditActivites != null)
                ValidateAuditedActivitiesForRolesAndUsers(privilegedRolesAndUsers, serverAuditActivites);
            var oldServer = SqlCmRecordReader.GetServerRecord(server.SrvId, connection);
            var newServer = CreateModifiedServerRecord(oldServer, auditDatabaseSeetings, connection);
            return WriteNewServerRecord(newServer, oldServer, connection);
        }

        public AuditActivity GetRegulationTemplatesForServer(AuditRegulationSettings auditSettings, SqlConnection connection)
        {
            AuditActivity serverAuditActivity = null;
            if (auditSettings == null)
            {
                LogAndThrowException("Failed to add databases because audit settings are not provided. it can't be null.");
            }
            if (auditSettings == null)
            {
                return null;
            }
            serverAuditActivity = GetRegulationSettingsForServerRecord(auditSettings,connection);
            return serverAuditActivity;
        }

        private AuditActivity GetRegulationSettingsForServerRecord(AuditRegulationSettings regulationSettings, SqlConnection connection)
        {
            RegulationSettings settings;
            var regulationCategoryDictionary = RegulationSettingsHelper.Instance.GetCategoryDictionary(connection);
            var tempServer = new AuditActivity();
            
            // apply PCI
            if (regulationSettings.PCI)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.PCI, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditDefinedEvents = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditDefinedEvents;
                    tempServer.AuditPrivilegedUsers = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || tempServer.AuditPrivilegedUsers;

                    if ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }

            //// apply HIPAA
            //// the OR against the server settings is done because the user can select more than one template.  When more than one template is 
            //// selected, the options are combined together
            if (regulationSettings.HIPAA)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.HIPAA, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditDefinedEvents = false;
                    tempServer.AuditPrivilegedUsers = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || tempServer.AuditPrivilegedUsers;
                    
                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.DISA)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.DISA, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditDefinedEvents = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditDefinedEvents;
                    tempServer.AuditPrivilegedUsers = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || tempServer.AuditPrivilegedUsers;
                   
                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.NERC)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.NERC, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditDefinedEvents = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditDefinedEvents;
                    tempServer.AuditPrivilegedUsers = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || tempServer.AuditPrivilegedUsers;
                   
                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.CIS)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.CIS, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditDefinedEvents = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditDefinedEvents;
                    tempServer.AuditPrivilegedUsers = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || tempServer.AuditPrivilegedUsers;
                    
                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.SOX)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.SOX, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditDefinedEvents = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditDefinedEvents;
                    tempServer.AuditPrivilegedUsers = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || tempServer.AuditPrivilegedUsers;
                    
                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.FERPA)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.FERPA, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditDefinedEvents = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditDefinedEvents;
                    tempServer.AuditPrivilegedUsers = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || tempServer.AuditPrivilegedUsers;
                    
                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            if (regulationSettings.GDPR)
            {
                if (regulationCategoryDictionary.TryGetValue((int)Regulation.RegulationType.GDPR, out settings))
                {
                    tempServer.AuditLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins) || tempServer.AuditLogins;
                    tempServer.AuditLogouts = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts) || tempServer.AuditLogouts;
                    tempServer.AuditFailedLogins = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins) || tempServer.AuditFailedLogins;
                    tempServer.AuditDDL = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) || tempServer.AuditDDL;
                    tempServer.AuditAdmin = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity) || tempServer.AuditAdmin;
                    tempServer.AuditSecurity = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges) || tempServer.AuditSecurity;
                    tempServer.AuditDefinedEvents = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined) || tempServer.AuditDefinedEvents;
                    tempServer.AuditPrivilegedUsers = ((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) == (int)RegulationSettings.RegulationServerCategory.PrivelegedUsers) || tempServer.AuditPrivilegedUsers;

                    if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.SuccessOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.SuccessOnly;
                    else if (((settings.ServerCategories & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) || (tempServer.AuditAccessCheck == AccessCheckFilter.FailureOnly))
                        tempServer.AuditAccessCheck = AccessCheckFilter.FailureOnly;
                    else
                        tempServer.AuditAccessCheck = AccessCheckFilter.NoFilter;
                }
            }
            //return the settings
            return tempServer;
        }
    }
}
