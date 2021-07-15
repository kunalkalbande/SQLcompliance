using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Filters;
using SQLcomplianceCwfAddin.Helpers.Agent;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Alerts;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AlertRules;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ActivityLogs;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ChangeLogs;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.UserSettings;
using SystemAlertType = SQLcomplianceCwfAddin.RestService.DataContracts.v1.SystemAlertType;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports;
using Idera.SQLcompliance.Core.Licensing;
using Idera.SQLcompliance.Core.CWFDataContracts;
using Idera.SQLcompliance.Core.Templates.AuditTemplates;
namespace SQLcomplianceCwfAddin.Helpers
{
    internal class Transformer
    {
        #region members

        private static Transformer _instance;
        private static int CISServerSettings, CISDatabaseSettings;
        private static int DISASTIGServerSettings, DISASTIGDatabaseSettings;
        private static int FERPAServerSettings, FERPADatabaseSettings;
        private static int GDPRServerSettings, GDPRDatabaseSettings;
        private static int HIPAAServerSettings, HIPAADatabaseSettings;
        private static int NERCServerSettings, NERCDatabaseSettings;
        private static int PCIDSSServerSettings, PCIDSSDatabaseSettings;
        private static int SOXServerSettings, SOXDatabaseSettings;

        internal enum RegulationId
        {
            All = 0,
            CIS = 5,
            DISASTIG = 3,
            FERPA = 7,
            GDPR = 8,
            HIPAA = 2,
            NERC = 4,
            PCIDSS = 1,
            SOX = 6
        }

        internal enum FieldType
        {
            Text = 0,
            Checkbox,
            RadioButton
        }

        internal enum Values
        {
            Selected = 0,
            Varies,
            Deselected,
            NA
        }

        internal enum AuditSettings
        {
            All = 0,
            Logins,
            Logouts,
            FailedLogins,
            SecurityChanges,
			AdministrativeActions,
			DatabaseDefinition,
			DatabaseModification,
			DatabaseSelect,
			UserDefinedEvents,
            AccessCheckFilter,
			CaptureDMLAndSelectActivities,
			CaptureSQL,
            CaptureTrans,
            CaptureDDL
        }

        #endregion

        #region constructor\destructor

        private Transformer()
        {
        }

        #endregion

        #region properties

        public static Transformer Instance
        {
            get { return _instance ?? (_instance = new Transformer()); }
        }

        #endregion

        internal void IntializeRegulationSettings(SqlConnection connection)
        {
            List<int> settings;

            settings = QueryExecutor.Instance.GetStandardRegulationSettings(connection, (int)RegulationId.CIS);
            CISServerSettings = settings[0];
            CISDatabaseSettings = settings[1];

            settings = QueryExecutor.Instance.GetStandardRegulationSettings(connection, (int)RegulationId.DISASTIG);
            DISASTIGServerSettings = settings[0];
            DISASTIGDatabaseSettings = settings[1];

            settings = QueryExecutor.Instance.GetStandardRegulationSettings(connection, (int)RegulationId.FERPA);
            FERPAServerSettings = settings[0];
            FERPADatabaseSettings = settings[1];

            settings = QueryExecutor.Instance.GetStandardRegulationSettings(connection, (int)RegulationId.GDPR);
            GDPRServerSettings = settings[0];
            GDPRDatabaseSettings = settings[1];

            settings = QueryExecutor.Instance.GetStandardRegulationSettings(connection, (int)RegulationId.HIPAA);
            HIPAAServerSettings = settings[0];
            HIPAADatabaseSettings = settings[1];

            settings = QueryExecutor.Instance.GetStandardRegulationSettings(connection, (int)RegulationId.NERC);
            NERCServerSettings = settings[0];
            NERCDatabaseSettings = settings[1];

            settings = QueryExecutor.Instance.GetStandardRegulationSettings(connection, (int)RegulationId.PCIDSS);
            PCIDSSServerSettings = settings[0];
            PCIDSSDatabaseSettings = settings[1];

            settings = QueryExecutor.Instance.GetStandardRegulationSettings(connection, (int)RegulationId.SOX);
            SOXServerSettings = settings[0];
            SOXDatabaseSettings = settings[1];

        }

        internal T ReadData<T>(IDataReader reader, string columnName)
        {
            if (DBNull.Value.Equals(reader[columnName]))
                return default(T);

            return (T)Convert.ChangeType(reader[columnName], typeof(T));
        }

        internal T ReadData<T>(IDataReader reader, int index)
        {
            if (DBNull.Value.Equals(reader[index]))
                return default(T);

            return (T)Convert.ChangeType(reader[index], typeof(T));
        }

        internal string TranslateServerName(string serverName)
        {
            // extract instance name part, we receive <instance_name>;<repository_database_name>
            var instanceName = serverName.Split(new[] { ';' }, StringSplitOptions.None);
            if (instanceName.Length >= 1)
                serverName = instanceName[0];

            if (serverName.StartsWith("(local)", StringComparison.InvariantCultureIgnoreCase))
                serverName = serverName.Replace("(local)", Environment.MachineName);
            else if (serverName.StartsWith(".", StringComparison.InvariantCultureIgnoreCase))
                serverName = serverName.Replace(".", Environment.MachineName);

            return serverName;
        }

        internal string CreateSafeDateTime(DateTime timestamp)
        {
            string newString;

            if (timestamp == DateTime.MinValue)
                newString = "null";
            else
                newString = String.Format("CONVERT(DATETIME, '{0}-{1}-{2} {3}:{4}:{5}.{6:000}',121)",
                                           timestamp.Year,
                                           timestamp.Month,
                                           timestamp.Day,
                                           timestamp.Hour,
                                           timestamp.Minute,
                                           timestamp.Second,
                                           timestamp.Millisecond);

            return newString;
        }

        internal string GetDateString(DateTime dateTime)
        {
            return dateTime.ToString("O", CultureInfo.InvariantCulture);
        }

        private List<SystemAlertType> GetAgentHealthDetails(UInt64 agentHealth)
        {
            var retVal = new List<SystemAlertType>();

            if ((agentHealth & 0x0000000000000001ul) != 0)
                retVal.Add(SystemAlertType.AgentWarning);
            if ((agentHealth & 0x0000000000000002ul) != 0)
                retVal.Add(SystemAlertType.AgentConfigurationError);
            if ((agentHealth & 0x0000000000000004ul) != 0)
                retVal.Add(SystemAlertType.TraceDirectoryError);
            if ((agentHealth & 0x0000000000000008ul) != 0)
                retVal.Add(SystemAlertType.SqlTraceError);
            if ((agentHealth & 0x0000000000000010ul) != 0)
                retVal.Add(SystemAlertType.ServerConnectionError);
            if ((agentHealth & 0x0000000000000020ul) != 0)
                retVal.Add(SystemAlertType.CollectionServiceConnectionError);
            if ((agentHealth & 0x0000000000000040ul) != 0)
                retVal.Add(SystemAlertType.ClrError);

            return retVal;
        }

        private int GetEnabledEventCategories(IDataReader reader,
                                              out List<string> enabledServerEventCategories,
                                              out List<string> enabledPrivilidgedUsersEventCategories)
        {
            var result = 0;

            enabledServerEventCategories = new List<string>();
            if (ReadData<bool>(reader, "auditLogins"))
            {
                result += 1;
                enabledServerEventCategories.Add("Logins");
            }
            // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
            if (ReadData<bool>(reader, "auditLogouts"))
            {
                result += 1;
                enabledServerEventCategories.Add("Logouts");
            }
            if (ReadData<bool>(reader, "auditFailedLogins"))
            {
                result += 1;
                enabledServerEventCategories.Add("Failed Logins");
            }
            if (ReadData<bool>(reader, "auditDDL"))
            {
                result += 1;
                enabledServerEventCategories.Add("DDL");
            }
            if (ReadData<bool>(reader, "auditSecurity"))
            {
                result += 1;
                enabledServerEventCategories.Add("Security");
            }
            if (ReadData<bool>(reader, "auditAdmin"))
            {
                result += 1;
                enabledServerEventCategories.Add("Admin");
            }
            if (ReadData<bool>(reader, "auditUDE"))
            {
                result += 1;
                enabledServerEventCategories.Add("UDE");
            }

            enabledPrivilidgedUsersEventCategories = new List<string>();
            if (!DBNull.Value.Equals(reader["auditUsersList"]))
            {
                var auditUserAll = ReadData<bool>(reader, "auditUserAll");

                if (auditUserAll && ReadData<bool>(reader, "auditUserLogins"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("Logins");
                }
                // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                if (auditUserAll && ReadData<bool>(reader, "auditUserLogouts"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("Logouts");
                }
                if (auditUserAll && ReadData<bool>(reader, "auditUserFailedLogins"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("Failed Logins");
                }
                if (auditUserAll && ReadData<bool>(reader, "auditUserDDL"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("DDL");
                }
                if (auditUserAll && ReadData<bool>(reader, "auditUserSecurity"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("Security");
                }
                if (auditUserAll && ReadData<bool>(reader, "auditUserAdmin"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("Admin");
                }
                if (auditUserAll && ReadData<bool>(reader, "auditUserDML"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("DML");
                }
                if (auditUserAll && ReadData<bool>(reader, "auditUserSELECT"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("Select");
                }

                if (auditUserAll && ReadData<bool>(reader, "auditUserCaptureSQL"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("Capture SQL");
                }

                if (auditUserAll && ReadData<bool>(reader, "auditUserCaptureDDL"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("Capture DDL");
                }

                if (auditUserAll && ReadData<bool>(reader, "auditUserExceptions"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("Capture Exceptions");
                }
                if (auditUserAll && ReadData<bool>(reader, "auditUserFailures"))
                {
                    result += 1;
                    enabledPrivilidgedUsersEventCategories.Add("Capture Failures");
                }
            }

            return result;
        }

        public List<PluginCommon.CreateUser> TransformToCwfUserList(List<User> users)
        {
            var responseUserList = new List<PluginCommon.CreateUser>();

            foreach (var user in users)
            {
                var newUser = new PluginCommon.CreateUser();
                newUser.Account = user.Account;
                newUser.SID = user.Sid;
                newUser.UserType = user.UserType;

                responseUserList.Add(newUser);
            }

            return responseUserList;
        }



        public ServerStatus GetStatus(IDataReader reader, int repositoryServerVersion, out ServerStatusFlag serverStatusFlag, out AlertStatusFlag agentStatus, out string statusMessage, out string auditStatus)
        {
            serverStatusFlag = ServerStatusFlag.Ok;
            ServerStatus statusImage = ServerStatus.OK;
            agentStatus = AlertStatusFlag.Ok;
            statusMessage = "OK";
            auditStatus = string.Empty;

            var isEnabled = ReadData<bool>(reader, "isEnabled");
            var isAudited = ReadData<bool>(reader, "isAuditedServer");
            var isAgentRunning = ReadData<bool>(reader, "isRunning");
            var isAgentCrippled = ReadData<bool>(reader, "isCrippled");
            var isAgentDeployed = ReadData<bool>(reader, "isDeployed");
            var isAgentDeployedManually = ReadData<bool>(reader, "isDeployedManually");
            var isAgentConfigUpdateRequested = ReadData<bool>(reader, "isUpdateRequested");
            var agentConfigVersion = ReadData<int>(reader, "configVersion");
            var agentLastKnownConfigVersion = ReadData<int>(reader, "lastKnownConfigVersion");
            var timeLastAgentContacted = ReadData<DateTime>(reader, "timeLastAgentContact");
            var auditedServerSqlVersion = ReadData<int>(reader, "sqlVersion") % 1000;//% 1000 for sure if instance is cluster for cluster servers the 1000 is added to SQL version
            var agentVersion = ReadData<string>(reader, "agentVersion");
            var agentHeartbeatInterval = ReadData<int>(reader, "agentHeartbeatInterval");
            var agentLastContacted = ReadData<DateTime>(reader, "timeLastHeartbeat");
            var agentHealth = ReadData<UInt64>(reader, "agentHealth");

            if (!isAudited)
            {
                //-----------------------
                // Archive host
                //-----------------------
                serverStatusFlag = ServerStatusFlag.Error;
                statusImage = ServerStatus.Archive;
                agentStatus = AlertStatusFlag.Ok;
                statusMessage = "Archive server";
            }
            else
            {
                //----------------
                // Audited Server
                //----------------

                if (!isAgentDeployed)
                {
                    statusImage = ServerStatus.Warning;
                    serverStatusFlag = ServerStatusFlag.Error;
                    //--------------
                    // Not deployed
                    //--------------
                    if (isAgentDeployedManually)
                    {
                        statusMessage = "Awaiting manual deployment";
                        agentStatus = AlertStatusFlag.Warning;
                    }
                    else
                    {
                        statusMessage = "Agent not deployed";
                        agentStatus = AlertStatusFlag.Critical;
                    }
                }
                else
                {
                    //-------------------------------------
                    // Deployed - so check for other stuff
                    //-------------------------------------
                    if (timeLastAgentContacted == DateTime.MinValue)
                    {
                        // Agent has never reported in
                        serverStatusFlag = ServerStatusFlag.Unknown;
                        statusImage = ServerStatus.Warning;
                        agentStatus = AlertStatusFlag.Warning;
                        statusMessage = "Unknown";
                    }
                    else
                    {
                        // check for supported SQL Server Version 
                        // we cant support auditing 2005 from a repository hosted by 2000
                        if (auditedServerSqlVersion > repositoryServerVersion)
                        {
                            serverStatusFlag = ServerStatusFlag.Error;
                            statusImage = ServerStatus.Alert;
                            if (auditedServerSqlVersion == 9)
                                statusMessage = ServerStatusMessages.ServerStatus_2005NotSupported;
                            else if (auditedServerSqlVersion == 10)
                                statusMessage = ServerStatusMessages.ServerStatus_2008NotSupported;
                            else if (auditedServerSqlVersion == 11)
                                statusMessage = ServerStatusMessages.ServerStatus_2012NotSupported;
                            else if (auditedServerSqlVersion == 12)
                                statusMessage = ServerStatusMessages.ServerStatus_2014NotSupported;
                            else if (auditedServerSqlVersion == 13)
                                statusMessage = ServerStatusMessages.ServerStatus_2016NotSupported;
							else if (auditedServerSqlVersion == 14)
                                statusMessage = ServerStatusMessages.ServerStatus_2017NotSupported;
                        }
                        else if (auditedServerSqlVersion == 9 && repositoryServerVersion == 9
                                && agentVersion.StartsWith("1."))
                        {
                            // We also can't support auditing 2005 with a 1.x agent
                            serverStatusFlag = ServerStatusFlag.Error;
                            statusImage = ServerStatus.Alert;
                            agentStatus = AlertStatusFlag.Critical;
                            statusMessage = "Agent Upgrade Required";
                        }
                        else if (auditedServerSqlVersion == 10 && repositoryServerVersion == 10
                           && (agentVersion.StartsWith("1.") || agentVersion.StartsWith("2.")
                           || agentVersion.StartsWith("3.0")))
                        {
                            // We also can't support auditing 2008 with a 1.x, 2.x, or 3.0 agent
                            serverStatusFlag = ServerStatusFlag.Error;
                            statusImage = ServerStatus.Alert;
                            agentStatus = AlertStatusFlag.Critical;
                            statusMessage = "Agent Upgrade Required";
                        }
                        else if (auditedServerSqlVersion == 11 &&
                            repositoryServerVersion == 11 &&
                            (agentVersion.StartsWith("1.") ||
                             agentVersion.StartsWith("2.") ||
                             agentVersion.StartsWith("3.0") ||
                             agentVersion.StartsWith("3.1") ||
                             agentVersion.StartsWith("3.2") ||
                             agentVersion.StartsWith("3.3") ||
                             agentVersion.StartsWith("3.5") ||
                             agentVersion.StartsWith("3.6")))
                        {
                            //we can't support 2012 with anything less than 3.7
                            serverStatusFlag = ServerStatusFlag.Error;
                            statusImage = ServerStatus.Alert;
                            agentStatus = AlertStatusFlag.Critical;
                            statusMessage = "Agent Upgrade Required";
                        }
                        else
                        {
                            if (!isAgentRunning)
                            {
                                // Agent has reported in
                                serverStatusFlag = ServerStatusFlag.Error;
                                statusImage = ServerStatus.Alert;
                                agentStatus = AlertStatusFlag.Critical;
                                statusMessage = "Down";
                            }
                            else if (isAgentCrippled)
                            {
                                serverStatusFlag = ServerStatusFlag.Error;
                                statusImage = ServerStatus.Alert;
                                agentStatus = AlertStatusFlag.Critical;
                                statusMessage = "Error";
                            }
                            else
                            {
                                // Running but possible stuff to report

                                // states
                                // Agent hasnt called home in a day - red
                                // Agent has missed two heartbeats  - yellow
                                // Disabled
                                // Audit Pending

                                DateTime noContactWarning = DateTime.UtcNow;
                                DateTime noContactError = DateTime.UtcNow;

                                noContactWarning = noContactWarning.AddMinutes(-2 * agentHeartbeatInterval);
                                noContactError = noContactError.AddDays(-1);

                                if (DateTime.Compare(agentLastContacted,
                                                    noContactError) < 0)
                                {
                                    statusMessage = "No contact in over a day";
                                    serverStatusFlag = ServerStatusFlag.Unknown;
                                    agentStatus = AlertStatusFlag.Warning;
                                    statusImage = ServerStatus.Alert;
                                }
                                else if (DateTime.Compare(agentLastContacted,
                                                         noContactWarning) < 0)
                                {
                                    statusMessage = "No contact in over a day";
                                    serverStatusFlag = ServerStatusFlag.Unknown;
                                    agentStatus = AlertStatusFlag.Warning;
                                    statusImage = ServerStatus.Warning;
                                }
                            }
                        }
                    }
                }

                // Audit Status
                if (isEnabled)
                    auditStatus = "Enabled";
                else
                {
                    statusImage = ServerStatus.Disabled;
                    auditStatus = "Disabled";
                }

                if (isAgentDeployed)
                {
                    if (isAgentConfigUpdateRequested)
                    {
                        if (agentConfigVersion == agentLastKnownConfigVersion)
                        {

                        }
                        else
                        {
                            auditStatus += "; Update requested";
                            agentStatus = AlertStatusFlag.Informational;
                        }
                    }
                    else if (agentConfigVersion != agentLastKnownConfigVersion)
                    {
                        auditStatus += "; Update pending";
                        agentStatus = AlertStatusFlag.Warning;
                    }
                }
                else
                {
                    auditStatus = "None until deployed";
                    agentStatus = AlertStatusFlag.Warning;
                }
            }
            // System Alerts
            if (agentHealth != 0)
            {
                var alerts = Instance.GetAgentHealthDetails(agentHealth);
                if (alerts.Count > 1)
                {
                    serverStatusFlag = ServerStatusFlag.Error;
                    statusMessage = String.Format("{0} Unresolved System Alerts", alerts.Count);
                }
                else if (alerts.Count == 1)
                {
                    switch (alerts[0])
                    {
                        case SystemAlertType.AgentWarning:
                            statusMessage = "Agent Warning";
                            agentStatus = AlertStatusFlag.Warning;
                            break;
                        case SystemAlertType.AgentConfigurationError:
                            statusMessage = "Agent Configuration Error";
                            agentStatus = AlertStatusFlag.Critical;
                            break;
                        case SystemAlertType.TraceDirectoryError:
                            statusMessage = "Trace Directory Error";
                            agentStatus = AlertStatusFlag.Critical;
                            serverStatusFlag = ServerStatusFlag.Error;
                            break;
                        case SystemAlertType.SqlTraceError:
                            statusMessage = "SQL Trace Error";
                            serverStatusFlag = ServerStatusFlag.Error;
                            break;
                        case SystemAlertType.ServerConnectionError:
                            statusMessage = "Server Connection Error";
                            serverStatusFlag = ServerStatusFlag.Error;
                            break;
                        case SystemAlertType.CollectionServiceConnectionError:
                            statusMessage = "Collection Service Connection Error";
                            serverStatusFlag = ServerStatusFlag.Error;
                            break;
                        case SystemAlertType.ClrError:
                            statusMessage = "CLR Error";
                            agentStatus = AlertStatusFlag.Critical;
                            serverStatusFlag = ServerStatusFlag.Error;
                            break;
                    }
                }
                else
                {
                    statusMessage = "Unresolved System Alert";
                    agentStatus = AlertStatusFlag.Informational;
                }


                auditStatus = "View Activity Log";
                statusImage = ServerStatus.Alert;
            }

            return statusImage;
        }

        public AuditedServerStatus TransformAuditedServerAlertData(IDataReader reader,
                                                                  int repositoryServerVersion,
                                                                  string connectionString)
        {
            AlertStatusFlag agentStatus;
            string message;
            string auditMessage;


            var result = new AuditedServerStatus();
            result.Id = ReadData<int>(reader, "srvId");
            result.CollectedEventCount = ReadData<int>(reader, "collectedEventCount");
            result.AuditedDatabaseCount = ReadData<int>(reader, "auditedDbCount");
            result.Instance = ReadData<string>(reader, "instance");
            result.IsAudited = ReadData<bool>(reader, "isAuditedServer");
            result.IsEnabled = ReadData<bool>(reader, "isEnabled");
            result.IsRunning = ReadData<bool>(reader, "isRunning");
            result.IsDeployed = ReadData<bool>(reader, "isDeployed");
            result.SqlVersionString = ReadData<string>(reader, "sqlVersionName");
            result.LowAlerts = ReadData<int>(reader, "LowAlerts");
            result.MediumAlerts = ReadData<int>(reader, "MediumAlerts");
            result.HighAlerts = ReadData<int>(reader, "HighAlerts");
            result.SevereAlerts = ReadData<int>(reader, "CriticalAlerts");
            result.RecentAlertCount = ReadData<int>(reader, "recentAlertCount");

            DateTime startTime = ReadData<DateTime>(reader, "timeLastAgentContact");
            if (startTime != DateTime.MinValue)
                result.LastAgentContactTime = startTime;

            startTime = ReadData<DateTime>(reader, "timeLastArchive");
            if (startTime != DateTime.MinValue)
                result.LastArchived = startTime;

            startTime = ReadData<DateTime>(reader, "timeLastHeartbeat");
            if (startTime != DateTime.MinValue)
                result.LastHeartbeat = startTime;

            string eventFilters = ReadData<string>(reader, "eventFilters");
            if (!string.IsNullOrWhiteSpace(eventFilters))
                result.EventFilters = eventFilters.Split(new[] { ',' }).ToList();

            var lastKnownConfigVersion = ReadData<int>(reader, "lastKnownConfigVersion");
            var configVersion = ReadData<int>(reader, "configVersion");

            ServerStatusFlag serverStatusFlag;
            var status = GetStatus(reader, repositoryServerVersion, out serverStatusFlag, out agentStatus, out message, out auditMessage);

            if (status == ServerStatus.Disabled)
            {
                message = auditMessage;
            }

            result.AgentStatus = agentStatus;
            result.Message = message;
            result.StatusFlag = serverStatusFlag;
            result.ServerStatus = status == ServerStatus.OK;
            result.ServerStatusDetailed = status;
            result.AuditSettingsUpdateEnabled = configVersion > lastKnownConfigVersion;

            return result;
        }

        public EnvironmentObject TransformEnvironmentObjectData(IDataReader reader, int parentId)
        {
            var result = new EnvironmentObject();
            result.Id = ReadData<int>(reader, "Id");
            result.SyetemId = ReadData<int>(reader, "SystemId");
            result.Type = (EnvironmentObjectType)Enum.Parse(typeof(EnvironmentObjectType), Convert.ToString(reader["Type"]));
            result.Name = ReadData<string>(reader, "Name");
            result.Description = ReadData<string>(reader, "Description");
            result.IsEnabled = ReadData<bool>(reader, "IsEnabled");
            result.ParentId = parentId;

            return result;
        }

        public EventDistributionForDatabaseResult TransformEventDistributionForDatabase(IDataReader reader)
        {
            var result = new EventDistributionForDatabaseResult();
            while (reader.Read())
            {
                int categoryId = ReadData<int>(reader, "category");
                switch (categoryId)
                {
                    case (int)RestStatsCategory.Security:
                        result.Security = ReadData<int>(reader, "categoryValue");
                        break;
                    case (int)RestStatsCategory.Ddl:
                        result.DDL = ReadData<int>(reader, "categoryValue");
                        break;
                    case (int)RestStatsCategory.Admin:
                        result.Admin = ReadData<int>(reader, "categoryValue");
                        break;
                    case (int)RestStatsCategory.Dml:
                        result.DML = ReadData<int>(reader, "categoryValue");
                        break;
                    case (int)RestStatsCategory.Select:
                        result.Select = ReadData<int>(reader, "categoryValue");
                        break;
                }
            }

            return result;
        }

        internal CmCombinedLicense TransformCmLicenseData(IDataReader reader, string licenseScope, BBSProductLicense LicenseObject)
        {
            var result = new CmCombinedLicense();
            while (reader.Read())
            {
                result.Licenses.Add(TransformCmLicenseData(reader, licenseScope, LicenseObject, result.Licenses));
            }

            result.Populate(LicenseObject);

            reader.NextResult();
            reader.Read();
            result.MonitoredServers = ReadData<int>(reader, "auditedServersCount");

            return result;
        }

        internal CmLicense TransformCmLicenseData(IDataReader reader, string licenseScope, BBSProductLicense LicenseObject, List<CmLicense> licensesAddedToCombinedLicense)
        {
            var result = new CmLicense(licenseScope);
            result.Id = ReadData<int>(reader, "Id");
            result.Key = ReadData<string>(reader, "Key");
            result.CreatedBy = ReadData<string>(reader, "CreatedBy");
            result.CreatedTime = ReadData<DateTime>(reader, "CreatedTime");
            result.Populate(LicenseObject, licensesAddedToCombinedLicense);

            return result;
        }

        internal AuditEvent TransformAuditEventData(IDataReader reader)
        {
            var result = new AuditEvent();
            result.Id = ReadData<int>(reader, "Id");
            result.Category = ReadData<string>(reader, "Category");
            result.EventType = ReadData<string>(reader, "EventType");
            result.Details = ReadData<string>(reader, "Details");
            var time = ReadData<DateTime>(reader, "Time");
            result.Time = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value; 
            return result;
        }

        internal AuditEvent TransformEventData(IDataReader reader)
        {
            var result = TransformAuditEventData(reader);

            result.EventDatabase = ReadData<string>(reader, "EventDatabaseName");
            result.DatabaseName = ReadData<string>(reader, "databaseName");
            result.DatabaseId = ReadData<int>(reader, "databaseId");
            result.CategoryId = ReadData<int>(reader, "CategoryId");
            result.EventTypeId = ReadData<int>(reader, "EventTypeId");
            result.LoginName = ReadData<string>(reader, "LoginName");
            result.TargetObject = ReadData<string>(reader, "targetObject");
            return result;
        }

        internal DetaliedAuditEvent TransformDetaliedAuditEvent(IDataReader reader)
        {
            var result = new DetaliedAuditEvent();
            result.Id = ReadData<int>(reader, "Id");
            result.Category = ReadData<string>(reader, "Category");
            result.EventType = ReadData<string>(reader, "EventType");
            result.Details = ReadData<string>(reader, "Details");
            var time = ReadData<DateTime>(reader, "Time");
            result.Time = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value; 
            result.EventDatabase = ReadData<string>(reader, "EventDatabaseName");
            result.DatabaseName = ReadData<string>(reader, "databaseName");
            result.DatabaseId = ReadData<int>(reader, "databaseId");
            result.CategoryId = ReadData<int>(reader, "CategoryId");
            result.EventTypeId = ReadData<int>(reader, "EventTypeId");
            result.LoginName = ReadData<string>(reader, "LoginName");
            result.TargetObject = ReadData<string>(reader, "targetObject");


            result.Spid = ReadData<int>(reader, "Spid");
            result.Application = ReadData<string>(reader, "Application");
            result.Host = ReadData<string>(reader, "Host");
            result.Server = ReadData<string>(reader, "Server");
            result.AccessCheck = ReadData<int>(reader, "AccessCheck");
            result.DatabaseUser = ReadData<string>(reader, "DatabaseUser");
            result.Object = ReadData<string>(reader, "Object");
            result.TargetLogin = ReadData<string>(reader, "TargetLogin");
            result.TargetUser = ReadData<string>(reader, "TargetUser");
            result.Role = ReadData<string>(reader, "Role");
            result.Owner = ReadData<string>(reader, "Owner");
            result.PrivilegedUser = ReadData<int>(reader, "PrivilegedUser");
            result.SessionLogin = ReadData<string>(reader, "SessionLogin");
            result.AuditedUpdates = ReadData<int>(reader, "AuditedUpdates");
            result.PrimaryKey = ReadData<string>(reader, "PrimaryKey");
            result.Table = ReadData<string>(reader, "Table");
            result.Column = ReadData<string>(reader, "Column");
            result.BeforeValue = ReadData<string>(reader, "BeforeValue");
            result.AfterValue = ReadData<string>(reader, "AfterValue");
            result.ColumnsUpdated = ReadData<int>(reader, "ColumnsUpdated");
            result.Schema = ReadData<string>(reader, "Schema");

            return result;
        }

        internal EventFilterListData TransformEventFilterData(IDataReader reader)
        {
            var result = new EventFilterListData();
            result.EventType =  ReadData<string>(reader, "EventType");
            return result;
        }
        internal RestStatsData TransformStatsData(IDataReader reader)
        {
            var result = new RestStatsData();
            result.DatabaseId = ReadData<int>(reader, "DatabaseId");
            result.Date = ReadData<DateTime>(reader, "Date");
            result.Category = (RestStatsCategory)Enum.Parse(typeof(RestStatsCategory), Convert.ToString(reader["CategoryId"]));
            result.CategoryName = ReadData<string>(reader, "Category");
            result.Count = ReadData<int>(reader, "Count");
            return result;
        }

        internal RestStatsData TransformEnterpriseStatsData(IDataReader reader)
        {
            var result = TransformStatsData(reader);
            result.ServerId = ReadData<int>(reader, "ServerId");

            return result;
        }

        internal AuditedDatabase TransformAuditedDatabaseData(IDataReader reader)
        {
            var result = new AuditedDatabase();
            result.Id = ReadData<int>(reader, "dbId");
            result.Name = ReadData<string>(reader, "name");
            result.Instance = ReadData<string>(reader, "srvInstance");
            result.IsEnabled = ReadData<bool>(reader, "isEnabled");

            result.RegulationGuidelines = new List<string>();
            if (ReadData<bool>(reader, "pci"))
                result.RegulationGuidelines.Add("PCI");
            if (ReadData<bool>(reader, "hipaa"))
                result.RegulationGuidelines.Add("HIPAA");
            if (ReadData<bool>(reader, "disa"))
                result.RegulationGuidelines.Add("DISA");
            if (ReadData<bool>(reader, "nerc"))
                result.RegulationGuidelines.Add("NERC");
            if (ReadData<bool>(reader, "cis"))
                result.RegulationGuidelines.Add("CIS");
            if (ReadData<bool>(reader, "sox"))
                result.RegulationGuidelines.Add("SOX");
            if (ReadData<bool>(reader, "ferpa"))
                result.RegulationGuidelines.Add("FERPA");

            result.AuditedActivities = new List<string>();
            if (ReadData<bool>(reader, "auditSecurity"))
                result.AuditedActivities.Add("Security");
            if (ReadData<bool>(reader, "auditDDL"))
                result.AuditedActivities.Add("DDL");
            if (ReadData<bool>(reader, "auditAdmin"))
                result.AuditedActivities.Add("Admin");
            if (ReadData<bool>(reader, "auditDML"))
                result.AuditedActivities.Add(ReadData<bool>(reader, "auditDmlAll") ? "DML" : "DML (filtered)");
            if (ReadData<bool>(reader, "auditSELECT"))
                result.AuditedActivities.Add("Select");
            if (ReadData<bool>(reader, "auditCaptureSQL"))
                result.AuditedActivities.Add("Capture SQL");
            if (ReadData<bool>(reader, "auditCaptureTrans"))
                result.AuditedActivities.Add("Capture Transactions");

            result.BeforeAfterTableCount = ReadData<int>(reader, "BeforeAfterTableCount");
            result.SensitiveColumnsTableCount = ReadData<int>(reader, "SensitiveColumnTableCount");
            result.TrustedUserCount = 0; //TODO: implement this

            return result;
        }

        internal IList<EventFilter> TransformEventFilterData(IDataReader reader, int serverId, int databaseId)
        {
            IDictionary<int, EventFilter> filterMap = new Dictionary<int, EventFilter>();

            while (reader.Read())
            {
                EventFilter eventFilter = null;

                int filterId = ReadData<int>(reader, "filterId");
                if (!filterMap.ContainsKey(filterId))
                {
                    eventFilter = new EventFilter();
                    eventFilter.Id = filterId;
                    eventFilter.Name = ReadData<string>(reader, "name");
                    eventFilter.Description = ReadData<string>(reader, "description");
                    eventFilter.RuleType = (EventType)ReadData<int>(reader, "eventType");
                    eventFilter.TargetInstanceList = ReadData<string>(reader, "targetInstances");
                    eventFilter.Enabled = ReadData<bool>(reader, "enabled");

                    if (!eventFilter.Enabled)
                        continue;

                    filterMap.Add(eventFilter.Id, eventFilter);
                }
                else
                {
                    eventFilter = filterMap[filterId];
                }

                EventCondition eventCondition = new EventCondition();
                eventCondition.Id = ReadData<int>(reader, "conditionId");
                eventCondition.TargetEventField = (EventField)(FiltersDal.SqlServerFilterFields[ReadData<int>(reader, "fieldId")]);
                eventCondition.MatchString = ReadData<string>(reader, "matchString");
                eventCondition.ParentRule = eventFilter;

                eventFilter.AddCondition(eventCondition);
            }

            return filterMap.Values.ToList();
        }



        internal ServerAlert TransformInstancesAlertData(IDataReader reader)
        {
            var result = new ServerAlert();
            result.AlertId = ReadData<int>(reader, "Id");
            result.InstanceId = ReadData<int>(reader, "SqlServerId");
            var time = ReadData<DateTime>(reader, "AlertTime");
            result.AlertTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value; 
            result.InstanceName = ReadData<string>(reader, "SqlServer");
            result.Level = (AlertLevel)ReadData<int>(reader, "AlertStatus");
            result.Type = (AlertRuleType)ReadData<int>(reader, "AlertType");
            result.RuleName = ReadData<string>(reader, "AlertRule");
            result.AlertEventId = ReadData<int>(reader, "alertEventId");

            return result;
        }

        internal ServerAlert TransformFilteredAlertData(IDataReader reader)
        {
            var result = new ServerAlert();
            result.AlertId = ReadData<int>(reader, "alertId");
            result.InstanceId = ReadData<int>(reader, "serverId");
            var time = ReadData<DateTime>(reader, "alertTime");
            result.AlertTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
            result.InstanceName = ReadData<string>(reader, "instance");
            result.Level = (AlertLevel)ReadData<int>(reader, "alertLevel");
            result.Type = (AlertRuleType)ReadData<int>(reader, "alertType");
            result.RuleName = ReadData<string>(reader, "alertRule");
            result.AlertEventId = ReadData<int>(reader, "alertEventId");
            result.EventType = ReadData<int>(reader, "alertEventTypeId");
            result.Detail = ReadData<string>(reader, "alertEventDetail");
            result.AlertEventTypeName = ReadData<string>(reader, "alertEventTypeName");

            return result;
        }

        internal ServerAlert TransformDatabaseAlerts(IDataReader reader)
        {
            var result = new ServerAlert();
            result.Type = (AlertRuleType)ReadData<int>(reader, "ruleType");
            result.RuleName = ReadData<string>(reader, "ruleTypeName");
            var time = ReadData<DateTime>(reader, "created");
            result.AlertTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value; 
            result.InstanceName = ReadData<string>(reader, "instance");
            result.Level = (AlertLevel)ReadData<int>(reader, "alertLevel");
            result.EventType = ReadData<int>(reader, "eventTypeId");
            result.AlertEventTypeName = ReadData<string>(reader, "category");

            return result;
        }


        internal EventProperties TransformEventProperties(IDataReader reader, bool loadSensitiveColumns = false, bool loadExtendedEventData = false)
        {
            EventProperties properties = new EventProperties();
            properties.EventId = ReadData<int>(reader, "eventId");
            properties.EventType = ReadData<int>(reader, "eventType");
            properties.EventCategory = ReadData<int>(reader, "eventCategory");
            properties.TargetObject = ReadData<string>(reader, "targetObject");
            properties.Details = ReadData<string>(reader, "details");
            properties.Hash = ReadData<string>(reader, "hash");
            properties.EventClass = ReadData<int>(reader, "eventClass");
            properties.EventSubclass = ReadData<int>(reader, "eventSubclass");

            DateTime startTime = ReadData<DateTime>(reader, "startTime");
            if (startTime != DateTime.MinValue)
            {
                properties.StartTime = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(startTime);
            }

            properties.SpId = ReadData<int>(reader, "spid");
            properties.AppName = ReadData<string>(reader, "applicationName");
            properties.HostName = ReadData<string>(reader, "hostName");
            properties.ServerName = ReadData<string>(reader, "serverName");
            properties.LoginName = ReadData<string>(reader, "loginName");
            properties.Success = ReadData<int>(reader, "success");
            properties.DatabaseName = ReadData<string>(reader, "databaseName");
            properties.DatabaseId = ReadData<int>(reader, "databaseId");
            properties.DatabaseUserName = ReadData<string>(reader, "dbUserName");
            properties.ObjectType = ReadData<int>(reader, "objectType");
            properties.ObjectName = ReadData<string>(reader, "objectName");
            properties.ObjectId = ReadData<int>(reader, "objectId");
            properties.Permissions = ReadData<int>(reader, "permissions");
            properties.ColumnPermissions = ReadData<int>(reader, "columnPermissions");
            properties.TargetLoginName = ReadData<string>(reader, "targetLoginName");
            properties.TargetUserName = ReadData<string>(reader, "targetUserName");
            properties.RoleName = ReadData<string>(reader, "roleName");
            properties.OwnerName = ReadData<string>(reader, "ownerName");
            properties.AlertLevel = ReadData<int>(reader, "alertLevel");
            properties.CheckSum = ReadData<string>(reader, "checkSum");
            properties.PrivilegedUser = ReadData<int>(reader, "privilegedUser");
            properties.Name = ReadData<string>(reader, "name");
            properties.Category = ReadData<string>(reader, "category");
            properties.FileName = ReadData<string>(reader, "fileName");
            properties.LinkedServerName = ReadData<string>(reader, "linkedServerName");
            properties.ParentName = ReadData<string>(reader, "parentName");
            properties.IsSystem = ReadData<int>(reader, "isSystem");
            properties.SessionLoginName = ReadData<string>(reader, "sessionLoginName");
            properties.ProviderName = ReadData<string>(reader, "providerName");
            properties.AppNameId = ReadData<string>(reader, "appNameId");
            properties.HostId = ReadData<string>(reader, "hostId");
            properties.LoginId = ReadData<string>(reader, "loginId");

            DateTime endTime = ReadData<DateTime>(reader, "endTime");
            if (endTime != DateTime.MinValue)
                properties.EndTime = endTime;

            properties.StartSequence = ReadData<int>(reader, "startSequence");
            properties.EndSequence = ReadData<int>(reader, "endSequence");
            if (DBNull.Value.Equals(reader["rowCounts"]))
            {
                properties.RowCounts = null;
            }
            else
            {
                properties.RowCounts = ReadData<long>(reader, "rowCounts");
            }
            if (loadSensitiveColumns)
                properties.SensitiveColumns = ReadData<int>(reader, "hasSensitiveColumns");

            if (loadExtendedEventData)
            {
                properties.SqlStatement = ReadData<string>(reader, "sqlText");
            }

            return properties;
        }

        public ArchiveRecord TransformArchiveData(SqlDataReader reader)
        {
            ArchiveRecord result = new ArchiveRecord();

            result.Instance = ReadData<string>(reader, "instance");
            result.DisplayName = ReadData<string>(reader, "displayName");
            result.Description = ReadData<string>(reader, "description");
            result.DatabaseName = ReadData<string>(reader, "databaseName");

            return result;
        }

        public ArchiveProperties TransformArchiveProperties(SqlDataReader reader, string archive)
        {
            ArchiveProperties result = new ArchiveProperties();

            result.Instance = ReadData<string>(reader, "instance");
            result.DisplayName = ReadData<string>(reader, "displayName");
            result.Description = ReadData<string>(reader, "description");
            result.DatabaseName = archive;
            result.IsValidArchive = true;

            DateTime tempTime;
            tempTime = ReadData<DateTime>(reader, "startDate");
            if (tempTime != DateTime.MinValue)
                result.EventTimeSpanFrom = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(tempTime);
            tempTime = ReadData<DateTime>(reader, "endDate");
            if (tempTime != DateTime.MinValue)
                result.EventTimeSpanTo = DateTimeHelper.GetNullableLocalTimeOfCurrentTimeZone(tempTime);

            int containsBadEvents = ReadData<int>(reader, "containsBadEvents");

            result.DatabaseIntegrity = containsBadEvents == 1
                ? DatabaseIntegrityStatus.BadEvents
                : DatabaseIntegrityStatus.Ok;

            tempTime = ReadData<DateTime>(reader, "timeLastIntegrityCheck");
            if (tempTime != DateTime.MinValue)
            {
                result.LastIntegrityCheck = tempTime;
                result.LastIntegrityCheckResult = ReadData<int>(reader, "lastIntegrityCheckResult");
            }

            result.DefaultAccess = ReadData<int>(reader, "defaultAccess");

            result.Schema = ReadData<int>(reader, "eventDbSchemaVersion");
            result.IsCompatibleSchema = result.Schema <= CoreConstants.RepositoryEventsDbSchemaVersion;
            if (!result.IsCompatibleSchema)
                result.Description =
                    "The selected database's schema is not supported by this version of SQL Compliance Manager and cannot be attached to the Repository.";

            return result;
        }

        public ViewSettings TransformViewSettings(SqlDataReader reader)
        {
            ViewSettings result = new ViewSettings();

            result.UserId = ReadData<string>(reader, "UserId");
            result.ViewId = ReadData<string>(reader, "ViewId");
            result.Timeout = ReadData<int>(reader, "Timeout");
            result.Filter = ReadData<string>(reader, "Filter");

            return result;
        }

        public AgentConnectionDetails TransformAgentConnectionDetails(SqlDataReader reader)
        {
            AgentConnectionDetails result = new AgentConnectionDetails();
            result.AgentServer = ReadData<string>(reader, "agentServer");
            result.AgentPort = ReadData<int>(reader, "agentPort");

            return result;
        }

        public UserSettingsModel TransformUserSettings(SqlDataReader reader)
        {
            var result = new UserSettingsModel();

            result.Account = ReadData<string>(reader, "Account");
            result.DashboardUserId = ReadData<int>(reader, "DashboardUserId");
            result.Email = ReadData<string>(reader, "Email");
            result.SessionTimout = ReadData<int>(reader, "SessionTimout");
            result.Subscribed = ReadData<bool>(reader, "Subscribed");

            return result;
        }

        public AlertsGroup TransformAlertsGroup(SqlDataReader reader)
        {
            var result = new AlertsGroup();

            result.AlertLevel = (AlertLevel)ReadData<byte>(reader, "alertLevel");
            result.AlertType = (AlertType)ReadData<byte>(reader, "alertType");
            result.AlertsCount = ReadData<int>(reader, "alertsCount");

            return result;
        }

        public ManagedInstanceResponce TransformManagedInstanceResponce(SqlDataReader reader)
        {
            var result = new ManagedInstanceResponce {ManagedInstances = new List<ManagedInstance>()};

            while (reader.Read())
            {
                result.ManagedInstances.Add(TransformManagedInstance(reader));
            }

            reader.NextResult();
            reader.Read();
            result.TotalCount = ReadData<int>(reader, "ManagedInstances");

            return result;
        }

        public ManagedInstance TransformManagedInstance(SqlDataReader reader)
        {
            var result = new ManagedInstance();
            result.Id = ReadData<int>(reader, "Id");
            result.Credentials.AccountType = (SqlServerSecurityModel)ReadData<byte>(reader, "AccountType");
            result.InstanceName = ReadData<string>(reader, "InstanceName");
            result.Credentials.Account = ReadData<string>(reader, "UserName");
            return result;
        }

        public ManagedInstanceProperties TransformInstanceInfo(SqlDataReader reader)
        {
            var result = new ManagedInstanceProperties();

            result.Id = ReadData<int>(reader, "Id");
            result.Credentials.AccountType = (SqlServerSecurityModel)ReadData<byte>(reader, "AccountType");
            result.InstanceName = ReadData<string>(reader, "InstanceName");
            result.Credentials.Account = ReadData<string>(reader, "UserName");
            var passwordEncrypted = ReadData<string>(reader, "Password");
            result.Credentials.Password = EncryptionHelper.QuickDecrypt(passwordEncrypted);

            result.DataCollectionSettings.CollectionInterval = ReadData<int>(reader, "CollectionInterval");
            result.Comments = ReadData<string>(reader, "Comments");
            result.DataCollectionSettings.KeepDataFor = ReadData<int>(reader, "KeepDataFor");
            result.Location = ReadData<string>(reader, "Location");
            result.Owner = ReadData<string>(reader, "Owner");

            return result;
        }

        public ManagedInstanceForEditResponce TransformOwnersAndLocations(SqlDataReader reader)
        {
            var result = new ManagedInstanceForEditResponce();

            result.Owners = new List<string>();
            while (reader.Read())
            {
                result.Owners.Add(ReadData<string>(reader, "Owner"));
            }

            reader.NextResult();

            result.Locations = new List<string>();
            while (reader.Read())
            {
                result.Locations.Add(ReadData<string>(reader, "Location"));
            }

            return result;
        }

        public ManagedServerInstance TransformManagedServerInstance(SqlDataReader reader)
        {
            var result = new ManagedServerInstance();

            result.Id = ReadData<int>(reader, "Id");
            result.Instance = ReadData<string>(reader, "Instance");
            result.ServerHost = ReadData<string>(reader, "ServerHost");

            return result;
        }

        public InstanceForSynchronization TransformInstanceForSynchronization(SqlDataReader reader)
        {
            var result = new InstanceForSynchronization
            {
                Instance = ReadData<string>(reader, "instance"),
                TimeLastCollection = ReadData<DateTime>(reader, "timeLastCollection"),
                VersionName = ReadData<string>(reader, "versionName"),
                Owner = ReadData<string>(reader, "owner"),
                Location = ReadData<string>(reader, "location"),
                Comments = ReadData<string>(reader, "comments"),
                TimeCreated = ReadData<DateTime>(reader, "timeCreated"),
                Description = ReadData<string>(reader, "description"),
                SqlVersion = ReadData<string>(reader, "sqlVersion"),
                SrvId = ReadData<int>(reader, "srvId")
                
            };

            return result;
        }
        public ActivityLogsInfo TransformActivityLogsInfo(SqlDataReader reader)
        {
            var result = new ActivityLogsInfo();

            result.AlertEventId = ReadData<int>(reader, "alertEventId");
            result.AlertId = ReadData<int>(reader, "alertId");
            result.AlertLevel = (ActivityLogsLevel)ReadData<byte>(reader, "alertLevel");
            result.AlertType = (ActivityLogsType)ReadData<byte>(reader, "alertType");
            var createdDate = ReadData<object>(reader, "created");
            if (createdDate != null)
            {
                result.Created = (DateTime)createdDate;
            }

            result.EventType = (EventType)ReadData<byte>(reader, "eventType");
            result.Instance = ReadData<string>(reader, "instance");
            result.RuleName = ReadData<string>(reader, "ruleName");

            result.AlertRuleId = ReadData<int>(reader, "alertRuleId");
            result.EmailStatus = ReadData<byte>(reader, "emailStatus");
            result.LogStatus = ReadData<byte>(reader, "logStatus");
            result.Message = ReadData<string>(reader, "message");
            result.ComputerName = ReadData<string>(reader, "computerName");
            result.SnmpTrapStatus = ReadData<byte>(reader, "snmpTrapStatus");
            result.RuleName = ReadData<string>(reader, "ruleName");

            return result;
        }

        internal ServerActivityLogs TransformInstancesActivityLogsData(IDataReader reader)
        {
            var result = new ServerActivityLogs();
            result.EventId = ReadData<int>(reader, "Id");
            result.EventTime = ReadData<DateTime>(reader, "AlertTime");
            result.InstanceName = ReadData<string>(reader, "SqlServer");

            return result;
        }

        internal ServerActivityLogs TransformFilteredActivityLogsData(IDataReader reader)
        {
            var result = new ServerActivityLogs();
            result.LogId = ReadData<int>(reader, "id");
            result.EventId = ReadData<int>(reader, "eventId");
            var time = ReadData<DateTime>(reader, "eventTime");
            result.EventTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
            result.InstanceName = ReadData<string>(reader, "instance");
            result.instanceId = ReadData<int>(reader, "instanceId");
            result.EventType = ReadData<string>(reader, "eventType");
            result.Detail = ReadData<string>(reader, "details");


            return result;
        }

        internal ServerActivityLogs TransformActivityLogsPropertyData(IDataReader reader)
        {
            var result = new ServerActivityLogs();
            result.LogId = ReadData<int>(reader, "id");
            result.EventId = ReadData<int>(reader, "eventId");
            var time = ReadData<DateTime>(reader, "eventTime");
            result.EventTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
            result.InstanceName = ReadData<string>(reader, "instance");
            result.EventType = ReadData<string>(reader, "eventType");
            result.Detail = ReadData<string>(reader, "details");
            return result;
        }

        internal ServerChangeLogs TransformFilteredChangeLogsData(IDataReader reader)
        {
            var result = new ServerChangeLogs();
            result.LogId = ReadData<int>(reader, "id");
            result.EventId = ReadData<int>(reader, "logId");
            var time = ReadData<DateTime>(reader, "eventTime");
            result.EventTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
            result.LogType = ReadData<string>(reader, "logType");
            result.LogUser = ReadData<string>(reader, "logUser");
            result.LogSQLServer = ReadData<string>(reader, "logSqlServer");
            result.instanceId = ReadData<int>(reader, "instanceId");
            result.LogInfo = ReadData<string>(reader, "logInfo");

            return result;
        }

        internal ServerChangeLogs TransformChangeLogsPropertyData(IDataReader reader)
        {
            var result = new ServerChangeLogs();
            result.LogId = ReadData<int>(reader, "id");
            result.EventId = ReadData<int>(reader, "logId");
            var time = ReadData<DateTime>(reader, "eventTime");
            result.EventTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
            result.LogType = ReadData<string>(reader, "logType");
            result.LogUser = ReadData<string>(reader, "logUser");
            result.LogSQLServer = ReadData<string>(reader, "logSqlServer");
            result.LogInfo = ReadData<string>(reader, "logInfo");

            return result;
        }

        internal ServerAlertRule TransformAlertRules(IDataReader reader)
        {
            var result = new ServerAlertRule();
            result.RuleId = ReadData<int>(reader, "ruleId");
            result.Name = ReadData<string>(reader, "name");
            result.TargetInstances = ReadData<string>(reader, "targetInstances");
            result.Level = (AlertLevel)ReadData<int>(reader, "alertLevel");
            result.Type = (AlertRuleType)ReadData<int>(reader, "alertType");
            result.LogMessage = ReadData<string>(reader, "logMessage");
            result.EmailMessage = ReadData<string>(reader, "emailMessage");
            result.SnmpTrap = ReadData<string>(reader, "snmpTrap");
            result.Enabled = ReadData<bool>(reader, "enabled");
            result.InstanceId = ReadData<int>(reader, "instanceId");
            result.RuleValidation = ReadData<int>(reader, "ruleValidation");

            return result;
        }

        internal AuditEventFilter TransformAuditEventFilterData(IDataReader reader)
        {
            var result = new AuditEventFilter();
            result.Filterid = ReadData<int>(reader, "filterId");
            result.Name = ReadData<string>(reader, "name");
            result.Description = ReadData<string>(reader, "description");
            result.EventType = ReadData<int>(reader, "eventType");
            result.TargetInstances = ReadData<string>(reader, "targetInstances");
            result.Enabled = ReadData<bool>(reader, "enabled");
            result.InstanceId = ReadData<int>(reader, "instanceId");
            result.ValidFilter = ReadData<int>(reader, "validFilter");
            return result;
        }

        internal AuditEventExportData TransformAuditEventExport(IDataReader reader)
        {
            var result = new AuditEventExportData();
            result.FilterId = ReadData<int>(reader, "filterId");
            result.Name = ReadData<string>(reader, "name");
            result.Description = ReadData<string>(reader, "description");
            result.EventType = ReadData<int>(reader, "eventType");
            result.TargetInstance = ReadData<string>(reader, "targetInstances");
            result.Enabled = ReadData<bool>(reader, "enabled");

            return result;
        }

        internal AuditEventExportConditionData TransformAuditEventExportCondition(IDataReader reader)
        {
            var conditionResult = new AuditEventExportConditionData();
            conditionResult.ConditionId = ReadData<int>(reader, "conditionId");
            conditionResult.FieldId = ReadData<int>(reader, "fieldId");
            conditionResult.MatchString = ReadData<string>(reader, "matchString");
            return conditionResult;
        }

        internal AuditEventExportEventType TransformAuditEventExportEventType(IDataReader reader)
        {
            var conditionResult = new AuditEventExportEventType();
            conditionResult.Name = ReadData<string>(reader, "name");
            conditionResult.Category = ReadData<string>(reader, "category");
            return conditionResult;
        }


        internal AlertRulesExportData TransformAlertRulesExport(IDataReader reader)
        {
            var result = new AlertRulesExportData();
            result.RuleId = ReadData<int>(reader, "ruleId");
            result.Name = ReadData<string>(reader, "name");
            result.Description = ReadData<string>(reader, "description");
            result.AlertType = ReadData<int>(reader, "alertType");
            result.AlertLevel = ReadData<int>(reader, "alertLevel");
            result.TargetInstances = ReadData<string>(reader, "targetInstances");
            result.Enabled = ReadData<bool>(reader, "enabled");
            result.Message = ReadData<string>(reader, "message");
            result.LogMessage = ReadData<int>(reader, "logMessage");
            result.EmailMessage = ReadData<int>(reader, "emailMessage");
            result.SnmpTrap = ReadData<int>(reader, "snmpTrap");
            result.SnmpServerAddress = ReadData<string>(reader, "snmpServerAddress");
            result.SnmpPort = ReadData<int>(reader, "snmpPort");
            result.SnmpCommunity = ReadData<string>(reader, "snmpCommunity");
            return result;
        }

        internal AlertRulesExportConditionData TransformAlertRulesExportCondition(IDataReader reader)
        {
            var conditionResult = new AlertRulesExportConditionData();
            conditionResult.ConditionId = ReadData<int>(reader, "conditionId");
            conditionResult.FieldId = ReadData<int>(reader, "fieldId");
            conditionResult.MatchString = ReadData<string>(reader, "matchString");
            return conditionResult;
        }

        internal AlertRulesExportCategoryData TransformAlertRulesExportCategory(IDataReader reader)
        {
            var categoryResult = new AlertRulesExportCategoryData();
            categoryResult.Category = ReadData<string>(reader, "category");
            categoryResult.Name = ReadData<string>(reader, "name");
            return categoryResult;
        }
        
        public ApplicationActivityData TransformApplicationActivity(SqlDataReader reader)
        {
            var result = new ApplicationActivityData();
            {
                result.ApplicationName = ReadData<string>(reader, "applicationName");
                result.Details = ReadData<string>(reader, "details");
                result.DatabaseName = ReadData<string>(reader, "databaseName");
                result.EventType = ReadData<string>(reader, "eventType");
                result.HostName = ReadData<string>(reader, "hostName");
                result.LoginName = ReadData<string>(reader, "loginName");
                result.TargetObject = ReadData<string>(reader, "targetObject");
                var time = ReadData<DateTime>(reader, "startTime");
                DateTime dateTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.StartTime = dateTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
               // var time = ReadData<DateTime>(reader, "startTime");
               // result.StartTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.SqlText = ReadData<string>(reader, "sqlText");
            };

            return result;
        }

        internal DMLActivityData TransformDMLActivity(SqlDataReader reader)
        {
            var result = new DMLActivityData();
            {
                result.EventType = ReadData<string>(reader, "eventTypeString");
                var time = ReadData<DateTime>(reader, "startTime");
                DateTime dateTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.StartTime = dateTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
              //  var time = ReadData<DateTime>(reader, "startTime");              
                //result.StartTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value.ToString();
                result.LoginName = ReadData<string>(reader, "loginName"); ;
                result.DatabaseName = ReadData<string>(reader, "databaseName");
                result.Table = ReadData<string>(reader, "targetObject");
                result.ColumnName = ReadData<string>(reader, "columnName");
                result.BeforeValue = ReadData<string>(reader, "beforeValue");
                result.AfterValue = ReadData<string>(reader, "afterValue");
                result.Keys = ReadData<string>(reader, "primaryKey");
            };

            return result;
        }

        internal LoginCreationHistoryData TransformLoginCreationHistory(SqlDataReader reader)
        {
            var result = new LoginCreationHistoryData();
            {
                result.TargetLoginName = ReadData<string>(reader, "targetLoginName");
                result.LoginName = ReadData<string>(reader, "loginName");
                result.HostName = ReadData<string>(reader, "hostName");
                result.ApplicationName = ReadData<string>(reader, "applicationName");
                var time = ReadData<DateTime>(reader, "startTime");
                DateTime dateTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.StartTime = dateTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
               // var time = ReadData<DateTime>(reader, "startTime");
                //result.StartTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
            };

            return result;
        }

        internal LoginDeletionHistoryData TransformLoginDeletionHistory(SqlDataReader reader)
        {
            var result = new LoginDeletionHistoryData();
            {
                result.TargetLoginName = ReadData<string>(reader, "targetLoginName");
                result.LoginName = ReadData<string>(reader, "loginName");
                result.HostName = ReadData<string>(reader, "hostName");
                result.ApplicationName = ReadData<string>(reader, "applicationName");
                var time = ReadData<DateTime>(reader, "startTime");
                DateTime dateTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.StartTime = dateTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                //var time = ReadData<DateTime>(reader, "startTime");
                //result.StartTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
            };

            return result;
        }

        internal ObjectActivityData TransformObjectActivity(SqlDataReader reader)
        {
            var result = new ObjectActivityData();
            {
                result.ApplicationName = ReadData<string>(reader, "applicationName");
                result.TargetObject = ReadData<string>(reader, "targetObject");
                result.Detail = ReadData<string>(reader, "details");
                result.DatabaseName = ReadData<string>(reader, "databaseName");
                result.EventType = ReadData<string>(reader, "eventType");
                result.HostName = ReadData<string>(reader, "hostName");
                result.LoginName = ReadData<string>(reader, "loginName");
                var time = ReadData<DateTime>(reader, "startTime");
                DateTime dateTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.StartTime = dateTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
              // var time = ReadData<DateTime>(reader, "startTime");
                //result.StartTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.SqlText = ReadData<string>(reader, "sqlText");
            };

            return result;
        }

        internal PermissionDeniedActivityData TransformPermissionDeniedActivity(SqlDataReader reader)
        {
            var result = new PermissionDeniedActivityData();
            {
                result.ApplicationName = ReadData<string>(reader, "applicationName");
                result.DatabaseName = ReadData<string>(reader, "databaseName");
                result.EventType = ReadData<string>(reader, "eventType");
                result.HostName = ReadData<string>(reader, "hostName");
                result.Details = ReadData<string>(reader, "details");
                result.LoginName = ReadData<string>(reader, "loginName");
                result.TargetObject = ReadData<string>(reader, "targetObject");
                var time = ReadData<DateTime>(reader, "startTime");
                DateTime dateTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.StartTime = dateTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
               // var time = ReadData<DateTime>(reader, "startTime");
                //result.StartTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.SqlText = ReadData<string>(reader, "sqlText");
            };

            return result;
        }

        internal UserActivityData TransformUserActivity(SqlDataReader reader)
        {
            var result = new UserActivityData();
            {
                result.ApplicationName = ReadData<string>(reader, "applicationName");
                result.DatabaseName = ReadData<string>(reader, "databaseName");
                result.EventType = ReadData<string>(reader, "eventType");
                result.HostName = ReadData<string>(reader, "hostName");
                result.Details = ReadData<string>(reader, "details");
                result.LoginName = ReadData<string>(reader, "loginName");
                result.TargetObject = ReadData<string>(reader, "targetObject");
                var time = ReadData<DateTime>(reader, "startTime");
                DateTime dateTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.StartTime = dateTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                
                //var time = ReadData<DateTime>(reader, "startTime");
               // result.StartTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.SqlText = ReadData<string>(reader, "sqlText");
            };

            return result;
        }
        //start sqlcm 5.6 -5469(Configuration Check Report)
        internal void ConfigurationCheckServerConfiguration(ConfigurationCheckProcedureData data,SqlDataReader reader)
        {
            data.flag = 0;
            data.srvId = ReadData<int>(reader, "srvId");
            data.instance = ReadData<string>(reader, "instance");
            data.isDeployed = ReadData<int>(reader, "isDeployed");
            data.eventDatabase = ReadData<string>(reader, "eventDatabase");
            data.agentVersion = ReadData<string>(reader, "agentVersion");
            data.auditAdmin = ReadData<int>(reader, "auditAdmin");
            data.auditCaptureSQL = ReadData<int>(reader, "auditCaptureSQL");
            data.auditCaptureSQLXE = ReadData<int>(reader, "auditCaptureSQLXE");
            data.auditDBCC = ReadData<int>(reader, "auditDBCC");
            data.auditDDL = ReadData<int>(reader, "auditDDL");
            data.auditDML = ReadData<int>(reader, "auditDML");
            data.auditExceptions = ReadData<int>(reader, "auditExceptions");
            data.auditFailedLogins = ReadData<int>(reader, "auditFailedLogins");
            data.auditFailures = ReadData<int>(reader, "auditFailures");
            data.auditLogins = ReadData<int>(reader, "auditLogins");
            data.auditLogouts = ReadData<int>(reader, "auditLogouts");
            data.auditSecurity = ReadData<int>(reader, "auditSecurity");
            data.auditSELECT = ReadData<int>(reader, "auditSELECT");
            data.auditSystemEvents = ReadData<int>(reader, "auditSystemEvents");
            data.auditTrace = ReadData<int>(reader, "auditTrace");
            data.auditUDE = ReadData<int>(reader, "auditUDE");
            data.auditUserAdmin = ReadData<int>(reader, "auditUserAdmin");
            data.auditUserAll = ReadData<int>(reader, "auditUserAll");
            data.auditUserCaptureDDL = ReadData<int>(reader, "auditUserCaptureDDL");
            data.auditUserCaptureSQL = ReadData<int>(reader, "auditUserCaptureSQL");
            data.auditUserCaptureTrans = ReadData<int>(reader, "auditUserCaptureTrans");
            data.auditUserDDL = ReadData<int>(reader, "auditUserDDL");
            data.auditUserDML = ReadData<int>(reader, "auditUserDML");
            data.auditUserExceptions = ReadData<int>(reader, "auditUserExceptions");
            data.auditUserExtendedEvents = ReadData<int>(reader, "auditUserExtendedEvents");
            data.auditUserFailedLogins = ReadData<int>(reader, "auditUserFailedLogins");
            data.auditUserFailures = ReadData<int>(reader, "auditUserFailures");
            data.auditUserLogins = ReadData<int>(reader, "auditUserLogins");
            data.auditUserLogouts = ReadData<int>(reader, "auditUserLogouts");
            data.auditUsers = ReadData<string>(reader, "auditUsers");
            data.auditUserSecurity = ReadData<int>(reader, "auditUserSecurity");
            data.auditUserSELECT = ReadData<int>(reader, "auditUserSELECT");
            data.auditUsersList = ReadData<string>(reader, "auditUsersList");
            data.auditUserUDE = ReadData<int>(reader, "auditUserUDE");
            data.isAuditLogEnabled = ReadData<int>(reader, "isAuditLogEnabled");
            data.auditTrustedUsersList = ReadData<string>(reader, "auditTrustedUsersList");
        }
        internal void ConfigurationCheckDatabaseConfiguration(ConfigurationCheckProcedureData data,SqlDataReader reader)
        {
            data.flag = 1;
            data.srvId = ReadData<int>(reader, "srvId");
            data.instance = ReadData<string>(reader, "instance");
            data.isDeployed = ReadData<int>(reader, "isDeployed");
            data.instanceServer = ReadData<string>(reader, "instanceServer");
            data.eventDatabase = ReadData<string>(reader, "eventDatabase");
            data.sqlDatabaseId = ReadData<int>(reader, "sqlDatabaseId");
            data.name = ReadData<string>(reader, "name");
            data.agentVersion = ReadData<string>(reader, "agentVersion");
            data.auditAdmin = ReadData<int>(reader, "auditAdmin");
            data.auditCaptureSQL = ReadData<int>(reader, "auditCaptureSQL");
            data.auditCaptureSQLXE = ReadData<int>(reader, "auditCaptureSQLXE");
            data.auditDBCC = ReadData<int>(reader, "auditDBCC");
            data.auditDDL = ReadData<int>(reader, "auditDDL");
            data.auditDML = ReadData<int>(reader, "auditDML");
            data.auditDMLAll = ReadData<int>(reader, "auditDmlAll");
            data.auditDMLOther = ReadData<int>(reader, "auditDmlOther");
            data.auditExceptions = ReadData<int>(reader, "auditExceptions");
            data.auditFailedLogins = ReadData<int>(reader, "auditFailedLogins");
            data.auditFailures = ReadData<int>(reader, "auditFailures");
            data.auditLogins = ReadData<int>(reader, "auditLogins");
            data.auditLogouts = ReadData<int>(reader, "auditLogouts");
            data.auditPrivUsersList = ReadData<string>(reader, "auditPrivUsersList");
            data.auditSecurity = ReadData<int>(reader, "auditSecurity");
            data.auditSELECT = ReadData<int>(reader, "auditSELECT");
            data.auditSystemEvents = ReadData<int>(reader, "auditSystemEvents");
            data.auditTrace = ReadData<int>(reader, "auditTrace");
            data.auditUDE = ReadData<int>(reader, "auditUDE");
            data.auditUserAdmin = ReadData<int>(reader, "auditUserAdmin");
            data.auditUserAll = ReadData<int>(reader, "auditUserAll");
            data.auditUserCaptureDDL = ReadData<int>(reader, "auditUserCaptureDDL");
            data.auditUserCaptureSQL = ReadData<int>(reader, "auditUserCaptureSQL");
            data.auditUserCaptureTrans = ReadData<int>(reader, "auditUserCaptureTrans");
            data.auditUserDDL = ReadData<int>(reader, "auditUserDDL");
            data.auditUserDML = ReadData<int>(reader, "auditUserDML");
            data.auditUserExceptions = ReadData<int>(reader, "auditUserExceptions");
            data.auditUserExtendedEvents = ReadData<int>(reader, "auditUserExtendedEvents");
            data.auditUserFailedLogins = ReadData<int>(reader, "auditUserFailedLogins");
            data.auditUserFailures = ReadData<int>(reader, "auditUserFailures");
            data.auditUserLogins = ReadData<int>(reader, "auditUserLogins");
            data.auditUserLogouts = ReadData<int>(reader, "auditUserLogouts");
            data.auditUsers = ReadData<string>(reader, "auditUsers");
            data.auditUserSecurity = ReadData<int>(reader, "auditUserSecurity");
            data.auditUserSELECT = ReadData<int>(reader, "auditUserSELECT");
            data.auditUsersList = ReadData<string>(reader, "auditUsersList");
            data.auditUserTables = ReadData<int>(reader, "auditUserTables");
            data.auditUserUDE = ReadData<int>(reader, "auditUserUDE");
            data.isAuditLogEnabled = ReadData<int>(reader, "isAuditLogEnabled");
            data.auditCaptureTrans = ReadData<int>(reader, "auditCaptureTrans");
            data.auditCaptureDDL = ReadData<int>(reader, "auditCaptureDDL");
            data.auditSystemTables = ReadData<int>(reader, "auditSystemTables");
            data.auditStoredProcedures = ReadData<int>(reader, "auditStoredProcedures");
            data.auditBroker = ReadData<int>(reader, "auditBroker");
            data.auditDataChanges = ReadData<int>(reader, "auditDataChanges");
            data.auditSensitiveColumns = ReadData<int>(reader, "auditSensitiveColumns");
        }
        //end sqlcm 5.6 - 5469(Configuration Check Report)

        internal RowCountData TransformRowCount(SqlDataReader reader)
        {
            var result = new RowCountData();
            {
                result.serverName = ReadData<string>(reader, "serverName");
                result.ApplicationName = ReadData<string>(reader, "applicationName");
                result.RoleName = ReadData<string>(reader, "roleName");
                result.Spid = ReadData<string>(reader, "spid");
                result.RowCounts = ReadData<string>(reader, "rowCounts");
                result.ColumnName = ReadData<string>(reader, "columnName");
                result.DatabaseName = ReadData<string>(reader, "databaseName");
                result.EventType = ReadData<string>(reader, "eventTypeString");
                result.LoginName = ReadData<string>(reader, "loginName");
                result.TargetObject = ReadData<string>(reader, "targetObject");
                var time = ReadData<DateTime>(reader, "startTime");
                DateTime dateTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(time).Value;
                result.StartTime = dateTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                result.SqlText = ReadData<string>(reader, "sqlText");
            };

            return result;
        }

        internal bool IsPartOfAtleastOne(int regulationId, int auditSettings, bool IsServerLevel)
        {
            if (IsServerLevel) {
                if ((regulationId == (int)RegulationId.All) || 
                    ((regulationId == (int)RegulationId.CIS) && ((CISServerSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.DISASTIG) && ((DISASTIGServerSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.FERPA) && ((FERPAServerSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.GDPR) && ((GDPRServerSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.HIPAA) && ((HIPAAServerSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.NERC) && ((NERCServerSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.PCIDSS) && ((PCIDSSServerSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.SOX) && ((SOXServerSettings & auditSettings) == auditSettings)))
                {
                    return true;
                }
            }
            else
            {
                if ((regulationId == (int)RegulationId.All) ||
                    ((regulationId == (int)RegulationId.CIS) && ((CISDatabaseSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.DISASTIG) && ((DISASTIGDatabaseSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.FERPA) && ((FERPADatabaseSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.GDPR) && ((GDPRDatabaseSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.HIPAA) && ((HIPAADatabaseSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.NERC) && ((NERCDatabaseSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.PCIDSS) && ((PCIDSSDatabaseSettings & auditSettings) == auditSettings)) ||
                    ((regulationId == (int)RegulationId.SOX) && ((SOXDatabaseSettings & auditSettings) == auditSettings)))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool IsPartOfNone(int regulationId, int auditSettings, bool IsServerLevel)
        {
            if (IsServerLevel)
            {
                if ((regulationId == (int)RegulationId.All) ||
                    ((regulationId == (int)RegulationId.CIS) && ((CISServerSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.DISASTIG) && ((DISASTIGServerSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.FERPA) && ((FERPAServerSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.GDPR) && ((GDPRServerSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.HIPAA) && ((HIPAAServerSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.NERC) && ((NERCServerSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.PCIDSS) && ((PCIDSSServerSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.SOX) && ((SOXServerSettings & auditSettings) == 0)))
                {
                    return true;
                }
            }
            else
            {
                if ((regulationId == (int)RegulationId.All) ||
                    ((regulationId == (int)RegulationId.CIS) && ((CISDatabaseSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.DISASTIG) && ((DISASTIGDatabaseSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.FERPA) && ((FERPADatabaseSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.GDPR) && ((GDPRDatabaseSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.HIPAA) && ((HIPAADatabaseSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.NERC) && ((NERCDatabaseSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.PCIDSS) && ((PCIDSSDatabaseSettings & auditSettings) == 0)) ||
                    ((regulationId == (int)RegulationId.SOX) && ((SOXDatabaseSettings & auditSettings) == 0)))
                {
                    return true;
                }
            }
            return false;
        }

        internal RegulatoryComplianceData TransformRegulatoryComplianceServer(SqlDataReader reader, RegulatoryComplianceRequest request)
        {
            int auditLogins, auditLogouts, auditFailedLogins, auditDDL, auditAdmin, auditSecurity, auditUDE, auditXE, auditLogs, auditAccessCheck,
                userAppliedServerSettings, variableLogins, variableLogouts, variableFailures, variableDDL, variableAdmin, variableSecurity, userAppliedRegulations;

            RegulatoryComplianceData result = new RegulatoryComplianceData();
            List<RegulatoryComplianceRowData> rowList = new List<RegulatoryComplianceRowData>();
            int count;

            result.ServerName = ReadData<string>(reader, "instance");
            result.DatabaseName = "";
            result.IsDatabase = false;
            result.ShowCIS = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.CIS);
            result.ShowDISASTIG = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.DISASTIG);
            result.ShowFERPA = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.FERPA);
            result.ShowGDPR = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.GDPR);
            result.ShowHIPAA = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.HIPAA);
            result.ShowNERC = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.NERC);
            result.ShowPCIDSS = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.PCIDSS);
            result.ShowSOX = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.SOX);

            auditLogins = ReadData<int>(reader, "auditLogins");
            auditLogouts = ReadData<int>(reader, "auditLogouts");
            auditFailedLogins = ReadData<int>(reader, "auditFailedLogins");
            auditDDL = ReadData<int>(reader, "auditDDL");
            auditAdmin = ReadData<int>(reader, "auditAdmin");
            auditSecurity = ReadData<int>(reader, "auditSecurity");
            auditUDE = ReadData<int>(reader, "auditUDE");
            auditXE = ReadData<int>(reader, "auditCaptureSQLXE");
            auditLogs = ReadData<int>(reader, "isAuditLogEnabled");
            auditAccessCheck = ReadData<int>(reader, "auditFailures");
            userAppliedServerSettings = ReadData<int>(reader, "userAppliedServerSettings");
            variableLogins = ReadData<int>(reader, "variableLogins");
            variableLogouts = ReadData<int>(reader, "variableLogouts");
            variableFailures = ReadData<int>(reader, "variableFailures");
            variableDDL = ReadData<int>(reader, "variableDDL");
            variableAdmin = ReadData<int>(reader, "variableAdmin");
            variableSecurity = ReadData<int>(reader, "variableSecurity");
            userAppliedRegulations = ReadData<int>(reader, "userAppliedRegulations");

            count = 0;
            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.CaptureDMLAndSelectActivities) &&
                (request.Values == (int)Values.Selected && auditLogs == 1) ||
                (request.Values == (int)Values.Deselected && auditLogs == 0))
            {
                RegulatoryComplianceRowData row5 = new RegulatoryComplianceRowData();
                row5.FieldName = "Via SQL Server Audit Specifications";
                row5.IsHeader = false;
                row5.FieldType = (int)FieldType.RadioButton;
                row5.IsCISChecked = (auditLogs == 1);
                row5.IsCISRed = false;
                row5.IsDISASTIGChecked = (auditLogs == 1);
                row5.IsDISASTIGRed = false;
                row5.IsFERPAChecked = (auditLogs == 1);
                row5.IsFERPARed = false;
                row5.IsGDPRChecked = (auditLogs == 1);
                row5.IsGDPRRed = false;
                row5.IsHIPAAChecked = (auditLogs == 1);
                row5.IsHIPAARed = false;
                row5.IsNERCChecked = (auditLogs == 1);
                row5.IsNERCRed = false;
                row5.IsPCIDSSChecked = (auditLogs == 1);
                row5.IsPCIDSSRed = false;
                row5.IsSOXChecked = (auditLogs == 1);
                row5.IsSOXRed = false;
                row5.IsFieldNameRed = false;
                rowList.Add(row5);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.CaptureDMLAndSelectActivities) &&
                (request.Values == (int)Values.Selected && auditXE == 1) ||
                (request.Values == (int)Values.Deselected && auditXE == 0))
            {
                RegulatoryComplianceRowData row6 = new RegulatoryComplianceRowData();
                row6.FieldName = "Via Extended Events";
                row6.IsHeader = false;
                row6.FieldType = (int)FieldType.RadioButton;
                row6.IsCISChecked = (auditXE == 1);
                row6.IsCISRed = false;
                row6.IsDISASTIGChecked = (auditXE == 1);
                row6.IsDISASTIGRed = false;
                row6.IsFERPAChecked = (auditXE == 1);
                row6.IsFERPARed = false;
                row6.IsGDPRChecked = (auditXE == 1);
                row6.IsGDPRRed = false;
                row6.IsHIPAAChecked = (auditXE == 1);
                row6.IsHIPAARed = false;
                row6.IsNERCChecked = (auditXE == 1);
                row6.IsNERCRed = false;
                row6.IsPCIDSSChecked = (auditXE == 1);
                row6.IsPCIDSSRed = false;
                row6.IsSOXChecked = (auditXE == 1);
                row6.IsSOXRed = false;
                row6.IsFieldNameRed = false;
                rowList.Add(row6);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.CaptureDMLAndSelectActivities) &&
                (request.Values == (int)Values.Selected && auditXE == 0 && auditLogs == 0) ||
                (request.Values == (int)Values.Deselected && !(auditXE == 0 && auditLogs == 0)))
            {
                RegulatoryComplianceRowData row7 = new RegulatoryComplianceRowData();
                row7.FieldName = "Via Trace Events";
                row7.IsHeader = false;
                row7.FieldType = (int)FieldType.RadioButton;
                row7.IsCISChecked = (auditXE == 0 && auditLogs == 0);
                row7.IsCISRed = false;
                row7.IsDISASTIGChecked = (auditXE == 0 && auditLogs == 0);
                row7.IsDISASTIGRed = false;
                row7.IsFERPAChecked = (auditXE == 0 && auditLogs == 0);
                row7.IsFERPARed = false;
                row7.IsGDPRChecked = (auditXE == 0 && auditLogs == 0);
                row7.IsGDPRRed = false;
                row7.IsHIPAAChecked = (auditXE == 0 && auditLogs == 0);
                row7.IsHIPAARed = false;
                row7.IsNERCChecked = (auditXE == 0 && auditLogs == 0);
                row7.IsNERCRed = false;
                row7.IsPCIDSSChecked = (auditXE == 0 && auditLogs == 0);
                row7.IsPCIDSSRed = false;
                row7.IsSOXChecked = (auditXE == 0 && auditLogs == 0);
                row7.IsSOXRed = false;
                row7.IsFieldNameRed = false;
                rowList.Add(row7);
                count++;
            }

            if(count > 0)
            {
                RegulatoryComplianceRowData row8 = new RegulatoryComplianceRowData();
                row8.FieldName = "Capture DML and Selected Activities";
                row8.IsHeader = true;
                rowList.Add(row8);
            }

			count = 0;
			if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.AccessCheckFilter) &&
				(request.Values == (int)Values.Selected && auditAccessCheck == 2 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess, true)) ||
				(request.Values == (int)Values.Deselected && auditAccessCheck != 2 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess, true)) ||
				(request.Values == (int)Values.Varies && variableFailures == 1) ||
				(request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess, true)))
			{
				RegulatoryComplianceRowData row1 = new RegulatoryComplianceRowData();
				row1.FieldName = "Failed";
				row1.IsHeader = false;
				row1.FieldType = (int)FieldType.RadioButton;
				row1.IsCISChecked = (auditAccessCheck == 2);
				row1.IsCISRed = (auditAccessCheck != 2 && (CISServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess);
				row1.IsDISASTIGChecked = (auditAccessCheck == 2);
				row1.IsDISASTIGRed = (auditAccessCheck != 2 && (DISASTIGServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess);
				row1.IsFERPAChecked = (auditAccessCheck == 2);
				row1.IsFERPARed = (auditAccessCheck != 2 && (FERPAServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess);
				row1.IsGDPRChecked = (auditAccessCheck == 2);
				row1.IsGDPRRed = (auditAccessCheck != 2 && (GDPRServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess);
				row1.IsHIPAAChecked = (auditAccessCheck == 2);
				row1.IsHIPAARed = (auditAccessCheck != 2 && (HIPAAServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess);
				row1.IsNERCChecked = (auditAccessCheck == 2);
				row1.IsNERCRed = (auditAccessCheck != 2 && (NERCServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess);
				row1.IsPCIDSSChecked = (auditAccessCheck == 2);
				row1.IsPCIDSSRed = (auditAccessCheck != 2 && (PCIDSSServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess);
				row1.IsSOXChecked = (auditAccessCheck == 2);
				row1.IsSOXRed = (auditAccessCheck != 2 && (SOXServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterFailedAccess);
				row1.IsFieldNameRed = (row1.IsCISRed || row1.IsDISASTIGRed || row1.IsFERPARed || row1.IsGDPRRed ||
									row1.IsHIPAARed || row1.IsNERCRed || row1.IsPCIDSSRed || row1.IsSOXRed);
				rowList.Add(row1);
				count++;
			}

			if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.AccessCheckFilter) &&
				(request.Values == (int)Values.Selected && auditAccessCheck == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess, true)) ||
				(request.Values == (int)Values.Deselected && auditAccessCheck != 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess, true)) ||
				(request.Values == (int)Values.Varies && variableFailures == 1) ||
				(request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess, true)))
			{
				RegulatoryComplianceRowData row2 = new RegulatoryComplianceRowData();
				row2.FieldName = "Passed";
				row2.IsHeader = false;
				row2.FieldType = (int)FieldType.RadioButton;
				row2.IsCISChecked = (auditAccessCheck == 0);
				row2.IsCISRed = (auditAccessCheck != 0 && (CISServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess);
				row2.IsDISASTIGChecked = (auditAccessCheck == 0);
				row2.IsDISASTIGRed = (auditAccessCheck != 0 && (DISASTIGServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess);
				row2.IsFERPAChecked = (auditAccessCheck == 0);
				row2.IsFERPARed = (auditAccessCheck != 0 && (FERPAServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess);
				row2.IsGDPRChecked = (auditAccessCheck == 0);
				row2.IsGDPRRed = (auditAccessCheck != 0 && (GDPRServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess);
				row2.IsHIPAAChecked = (auditAccessCheck == 0);
				row2.IsHIPAARed = (auditAccessCheck != 0 && (HIPAAServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess);
				row2.IsNERCChecked = (auditAccessCheck == 0);
				row2.IsNERCRed = (auditAccessCheck != 0 && (NERCServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess);
				row2.IsPCIDSSChecked = (auditAccessCheck == 0);
				row2.IsPCIDSSRed = (auditAccessCheck != 0 && (PCIDSSServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess);
				row2.IsSOXChecked = (auditAccessCheck == 0);
				row2.IsSOXRed = (auditAccessCheck != 0 && (SOXServerSettings & (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationServerCategory.FilterPassedAccess);
				row2.IsFieldNameRed = (row2.IsCISRed || row2.IsDISASTIGRed || row2.IsFERPARed || row2.IsGDPRRed ||
									row2.IsHIPAARed || row2.IsNERCRed || row2.IsPCIDSSRed || row2.IsSOXRed);
				rowList.Add(row2);
				count++;
			}

			if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.AccessCheckFilter) &&
				(request.Values == (int)Values.Selected && auditAccessCheck != 1) ||
				(request.Values == (int)Values.Deselected && auditAccessCheck == 1) ||
				(request.Values == (int)Values.Varies && variableFailures == 1))
			{
				RegulatoryComplianceRowData row3 = new RegulatoryComplianceRowData();
				row3.FieldName = "Filter Events based on access check";
				row3.IsHeader = false;
				row3.FieldType = (int)FieldType.Checkbox;
				row3.IsCISChecked = (auditAccessCheck != 1);
				row3.IsCISRed = false;
				row3.IsDISASTIGChecked = (auditAccessCheck != 1);
				row3.IsDISASTIGRed = false;
				row3.IsFERPAChecked = (auditAccessCheck != 1);
				row3.IsFERPARed = false;
				row3.IsGDPRChecked = (auditAccessCheck != 1);
				row3.IsGDPRRed = false;
				row3.IsHIPAAChecked = (auditAccessCheck != 1);
				row3.IsHIPAARed = false;
				row3.IsNERCChecked = (auditAccessCheck != 1);
				row3.IsNERCRed = false;
				row3.IsPCIDSSChecked = (auditAccessCheck != 1);
				row3.IsPCIDSSRed = false;
				row3.IsSOXChecked = (auditAccessCheck != 1);
				row3.IsSOXRed = false;
				row3.IsFieldNameRed = false;
				rowList.Add(row3);
				count++;
			}

			if (count > 0)
			{
				RegulatoryComplianceRowData row4 = new RegulatoryComplianceRowData();
				row4.FieldName = "Access Check Filter";
				row4.IsHeader = true;
				rowList.Add(row4);
			}

			count = 0;
            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.UserDefinedEvents) &&
                (request.Values == (int)Values.Selected && auditUDE == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.UserDefined, true)) ||
                (request.Values == (int)Values.Deselected && auditUDE == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.UserDefined, true)) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.UserDefined, true)))
            {
                RegulatoryComplianceRowData row9 = new RegulatoryComplianceRowData();
                row9.FieldName = "User Defined Events";
                row9.IsHeader = false;
                row9.FieldType = (int)FieldType.Checkbox;
                row9.IsCISChecked = (auditUDE == 1);
                row9.IsCISRed = (auditUDE == 0 && (CISServerSettings & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined);
                row9.IsDISASTIGChecked = (auditUDE == 1);
                row9.IsDISASTIGRed = (auditUDE == 0 && (DISASTIGServerSettings & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined);
                row9.IsFERPAChecked = (auditUDE == 1);
                row9.IsFERPARed = (auditUDE == 0 && (FERPAServerSettings & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined);
                row9.IsGDPRChecked = (auditUDE == 1);
                row9.IsGDPRRed = (auditUDE == 0 && (GDPRServerSettings & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined);
                row9.IsHIPAAChecked = (auditUDE == 1);
                row9.IsHIPAARed = (auditUDE == 0 && (HIPAAServerSettings & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined);
                row9.IsNERCChecked = (auditUDE == 1);
                row9.IsNERCRed = (auditUDE == 0 && (NERCServerSettings & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined);
                row9.IsPCIDSSChecked = (auditUDE == 1);
                row9.IsPCIDSSRed = (auditUDE == 0 && (PCIDSSServerSettings & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined);
                row9.IsSOXChecked = (auditUDE == 1);
                row9.IsSOXRed = (auditUDE == 0 && (SOXServerSettings & (int)RegulationSettings.RegulationServerCategory.UserDefined) == (int)RegulationSettings.RegulationServerCategory.UserDefined);
                row9.IsFieldNameRed = (row9.IsCISRed || row9.IsDISASTIGRed || row9.IsFERPARed || row9.IsGDPRRed ||
                                    row9.IsHIPAARed || row9.IsNERCRed || row9.IsPCIDSSRed || row9.IsSOXRed);
                rowList.Add(row9);
                count++;
            }

			if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.DatabaseDefinition) &&
				(request.Values == (int)Values.Selected && auditDDL == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition, true)) ||
				(request.Values == (int)Values.Deselected && auditDDL == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition, true)) ||
				(request.Values == (int)Values.Varies && variableDDL == 1) ||
				(request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition, true)))
			{
				RegulatoryComplianceRowData row11 = new RegulatoryComplianceRowData();
				row11.FieldName = "Database Definition (DDL)";
				row11.IsHeader = false;
				row11.FieldType = (int)FieldType.Checkbox;
				row11.IsCISChecked = (auditDDL == 1);
				row11.IsCISRed = (auditDDL == 0 && (CISServerSettings & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition);
				row11.IsDISASTIGChecked = (auditDDL == 1);
				row11.IsDISASTIGRed = (auditDDL == 0 && (DISASTIGServerSettings & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition);
				row11.IsFERPAChecked = (auditDDL == 1);
				row11.IsFERPARed = (auditDDL == 0 && (FERPAServerSettings & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition);
				row11.IsGDPRChecked = (auditDDL == 1);
				row11.IsGDPRRed = (auditDDL == 0 && (GDPRServerSettings & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition);
				row11.IsHIPAAChecked = (auditDDL == 1);
				row11.IsHIPAARed = (auditDDL == 0 && (HIPAAServerSettings & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition);
				row11.IsNERCChecked = (auditDDL == 1);
				row11.IsNERCRed = (auditDDL == 0 && (NERCServerSettings & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition);
				row11.IsPCIDSSChecked = (auditDDL == 1);
				row11.IsPCIDSSRed = (auditDDL == 0 && (PCIDSSServerSettings & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition);
				row11.IsSOXChecked = (auditDDL == 1);
				row11.IsSOXRed = (auditDDL == 0 && (SOXServerSettings & (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationServerCategory.DatabaseDefinition);
				row11.IsFieldNameRed = (row11.IsCISRed || row11.IsDISASTIGRed || row11.IsFERPARed || row11.IsGDPRRed ||
									row11.IsHIPAARed || row11.IsNERCRed || row11.IsPCIDSSRed || row11.IsSOXRed);
				rowList.Add(row11);
				count++;
			}

			if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.AdministrativeActions) &&
                (request.Values == (int)Values.Selected && auditAdmin == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.AdminActivity, true)) ||
                (request.Values == (int)Values.Deselected && auditAdmin == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.AdminActivity, true)) ||
                (request.Values == (int)Values.Varies && variableAdmin == 1) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.AdminActivity, true)))
            {
                RegulatoryComplianceRowData row10 = new RegulatoryComplianceRowData();
                row10.FieldName = "Administrative Actions";
                row10.IsHeader = false;
                row10.FieldType = (int)FieldType.Checkbox;
                row10.IsCISChecked = (auditAdmin == 1);
                row10.IsCISRed = (auditAdmin == 0 && (CISServerSettings & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity);
                row10.IsDISASTIGChecked = (auditAdmin == 1);
                row10.IsDISASTIGRed = (auditAdmin == 0 && (DISASTIGServerSettings & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity);
                row10.IsFERPAChecked = (auditAdmin == 1);
                row10.IsFERPARed = (auditAdmin == 0 && (FERPAServerSettings & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity);
                row10.IsGDPRChecked = (auditAdmin == 1);
                row10.IsGDPRRed = (auditAdmin == 0 && (GDPRServerSettings & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity);
                row10.IsHIPAAChecked = (auditAdmin == 1);
                row10.IsHIPAARed = (auditAdmin == 0 && (HIPAAServerSettings & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity);
                row10.IsNERCChecked = (auditAdmin == 1);
                row10.IsNERCRed = (auditAdmin == 0 && (NERCServerSettings & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity);
                row10.IsPCIDSSChecked = (auditAdmin == 1);
                row10.IsPCIDSSRed = (auditAdmin == 0 && (PCIDSSServerSettings & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity);
                row10.IsSOXChecked = (auditAdmin == 1);
                row10.IsSOXRed = (auditAdmin == 0 && (SOXServerSettings & (int)RegulationSettings.RegulationServerCategory.AdminActivity) == (int)RegulationSettings.RegulationServerCategory.AdminActivity);
                row10.IsFieldNameRed = (row10.IsCISRed || row10.IsDISASTIGRed || row10.IsFERPARed || row10.IsGDPRRed ||
                                    row10.IsHIPAARed || row10.IsNERCRed || row10.IsPCIDSSRed || row10.IsSOXRed);
                rowList.Add(row10);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.SecurityChanges) &&
                (request.Values == (int)Values.Selected && auditSecurity == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.SecurityChanges, true)) ||
                (request.Values == (int)Values.Deselected && auditSecurity == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.SecurityChanges, true)) ||
                (request.Values == (int)Values.Varies && variableSecurity == 1) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.SecurityChanges, true)))
            {
                RegulatoryComplianceRowData row12 = new RegulatoryComplianceRowData();
                row12.FieldName = "Security Changes";
                row12.IsHeader = false;
                row12.FieldType = (int)FieldType.Checkbox;
                row12.IsCISChecked = (auditSecurity == 1);
                row12.IsCISRed = (auditSecurity == 0 && (CISServerSettings & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges);
                row12.IsDISASTIGChecked = (auditSecurity == 1);
                row12.IsDISASTIGRed = (auditSecurity == 0 && (DISASTIGServerSettings & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges);
                row12.IsFERPAChecked = (auditSecurity == 1);
                row12.IsFERPARed = (auditSecurity == 0 && (FERPAServerSettings & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges);
                row12.IsGDPRChecked = (auditSecurity == 1);
                row12.IsGDPRRed = (auditSecurity == 0 && (GDPRServerSettings & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges);
                row12.IsHIPAAChecked = (auditSecurity == 1);
                row12.IsHIPAARed = (auditSecurity == 0 && (HIPAAServerSettings & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges);
                row12.IsNERCChecked = (auditSecurity == 1);
                row12.IsNERCRed = (auditSecurity == 0 && (NERCServerSettings & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges);
                row12.IsPCIDSSChecked = (auditSecurity == 1);
                row12.IsPCIDSSRed = (auditSecurity == 0 && (PCIDSSServerSettings & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges);
                row12.IsSOXChecked = (auditSecurity == 1);
                row12.IsSOXRed = (auditSecurity == 0 && (SOXServerSettings & (int)RegulationSettings.RegulationServerCategory.SecurityChanges) == (int)RegulationSettings.RegulationServerCategory.SecurityChanges);
                row12.IsFieldNameRed = (row12.IsCISRed || row12.IsDISASTIGRed || row12.IsFERPARed || row12.IsGDPRRed ||
                                    row12.IsHIPAARed || row12.IsNERCRed || row12.IsPCIDSSRed || row12.IsSOXRed);
                rowList.Add(row12);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.FailedLogins) &&
                (request.Values == (int)Values.Selected && auditFailedLogins == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.FailedLogins, true)) ||
                (request.Values == (int)Values.Deselected && auditFailedLogins == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.FailedLogins, true)) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.FailedLogins, true)))
            {
                RegulatoryComplianceRowData row13 = new RegulatoryComplianceRowData();
                row13.FieldName = "Failed Logins";
                row13.IsHeader = false;
                row13.FieldType = (int)FieldType.Checkbox;
                row13.IsCISChecked = (auditFailedLogins == 1);
                row13.IsCISRed = (auditFailedLogins == 0 && (CISServerSettings & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins);
                row13.IsDISASTIGChecked = (auditFailedLogins == 1);
                row13.IsDISASTIGRed = (auditFailedLogins == 0 && (DISASTIGServerSettings & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins);
                row13.IsFERPAChecked = (auditFailedLogins == 1);
                row13.IsFERPARed = (auditFailedLogins == 0 && (FERPAServerSettings & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins);
                row13.IsGDPRChecked = (auditFailedLogins == 1);
                row13.IsGDPRRed = (auditFailedLogins == 0 && (GDPRServerSettings & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins);
                row13.IsHIPAAChecked = (auditFailedLogins == 1);
                row13.IsHIPAARed = (auditFailedLogins == 0 && (HIPAAServerSettings & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins);
                row13.IsNERCChecked = (auditFailedLogins == 1);
                row13.IsNERCRed = (auditFailedLogins == 0 && (NERCServerSettings & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins);
                row13.IsPCIDSSChecked = (auditFailedLogins == 1);
                row13.IsPCIDSSRed = (auditFailedLogins == 0 && (PCIDSSServerSettings & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins);
                row13.IsSOXChecked = (auditFailedLogins == 1);
                row13.IsSOXRed = (auditFailedLogins == 0 && (SOXServerSettings & (int)RegulationSettings.RegulationServerCategory.FailedLogins) == (int)RegulationSettings.RegulationServerCategory.FailedLogins);
                row13.IsFieldNameRed = (row13.IsCISRed || row13.IsDISASTIGRed || row13.IsFERPARed || row13.IsGDPRRed ||
                                    row13.IsHIPAARed || row13.IsNERCRed || row13.IsPCIDSSRed || row13.IsSOXRed);
                rowList.Add(row13);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.Logouts) &&
                (request.Values == (int)Values.Selected && auditLogouts == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.Logouts, true)) ||
                (request.Values == (int)Values.Deselected && auditLogouts == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.Logouts, true)) ||
                (request.Values == (int)Values.Varies && variableLogouts == 1) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.Logouts, true)))
            {
                RegulatoryComplianceRowData row14 = new RegulatoryComplianceRowData();
                row14.FieldName = "Logouts";
                row14.IsHeader = false;
                row14.FieldType = (int)FieldType.Checkbox;
                row14.IsCISChecked = (auditLogouts == 1);
                row14.IsCISRed = (auditLogouts == 0 && (CISServerSettings & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts);
                row14.IsDISASTIGChecked = (auditLogouts == 1);
                row14.IsDISASTIGRed = (auditLogouts == 0 && (DISASTIGServerSettings & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts);
                row14.IsFERPAChecked = (auditLogouts == 1);
                row14.IsFERPARed = (auditLogouts == 0 && (FERPAServerSettings & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts);
                row14.IsGDPRChecked = (auditLogouts == 1);
                row14.IsGDPRRed = (auditLogouts == 0 && (GDPRServerSettings & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts);
                row14.IsHIPAAChecked = (auditLogouts == 1);
                row14.IsHIPAARed = (auditLogouts == 0 && (HIPAAServerSettings & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts);
                row14.IsNERCChecked = (auditLogouts == 1);
                row14.IsNERCRed = (auditLogouts == 0 && (NERCServerSettings & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts);
                row14.IsPCIDSSChecked = (auditLogouts == 1);
                row14.IsPCIDSSRed = (auditLogouts == 0 && (PCIDSSServerSettings & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts);
                row14.IsSOXChecked = (auditLogouts == 1);
                row14.IsSOXRed = (auditLogouts == 0 && (SOXServerSettings & (int)RegulationSettings.RegulationServerCategory.Logouts) == (int)RegulationSettings.RegulationServerCategory.Logouts);
                row14.IsFieldNameRed = (row14.IsCISRed || row14.IsDISASTIGRed || row14.IsFERPARed || row14.IsGDPRRed ||
                                    row14.IsHIPAARed || row14.IsNERCRed || row14.IsPCIDSSRed || row14.IsSOXRed);
                rowList.Add(row14);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.Logins) &&
                (request.Values == (int)Values.Selected && auditLogins == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.Logins, true)) ||
                (request.Values == (int)Values.Deselected && auditLogins == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.Logins, true)) ||
                (request.Values == (int)Values.Varies && variableLogins == 1) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationServerCategory.Logins, true)))
            {
                RegulatoryComplianceRowData row15 = new RegulatoryComplianceRowData();
                row15.FieldName = "Logins";
                row15.IsHeader = false;
                row15.FieldType = (int)FieldType.Checkbox;
                row15.IsCISChecked = (auditLogins == 1);
                row15.IsCISRed = (auditLogins == 0 && (CISServerSettings & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins);
                row15.IsDISASTIGChecked = (auditLogins == 1);
                row15.IsDISASTIGRed = (auditLogins == 0 && (DISASTIGServerSettings & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins);
                row15.IsFERPAChecked = (auditLogins == 1);
                row15.IsFERPARed = (auditLogins == 0 && (FERPAServerSettings & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins);
                row15.IsGDPRChecked = (auditLogins == 1);
                row15.IsGDPRRed = (auditLogins == 0 && (GDPRServerSettings & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins);
                row15.IsHIPAAChecked = (auditLogins == 1);
                row15.IsHIPAARed = (auditLogins == 0 && (HIPAAServerSettings & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins);
                row15.IsNERCChecked = (auditLogins == 1);
                row15.IsNERCRed = (auditLogins == 0 && (NERCServerSettings & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins);
                row15.IsPCIDSSChecked = (auditLogins == 1);
                row15.IsPCIDSSRed = (auditLogins == 0 && (PCIDSSServerSettings & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins);
                row15.IsSOXChecked = (auditLogins == 1);
                row15.IsSOXRed = (auditLogins == 0 && (SOXServerSettings & (int)RegulationSettings.RegulationServerCategory.Logins) == (int)RegulationSettings.RegulationServerCategory.Logins);
                row15.IsFieldNameRed = (row15.IsCISRed || row15.IsDISASTIGRed || row15.IsFERPARed || row15.IsGDPRRed ||
                                    row15.IsHIPAARed || row15.IsNERCRed || row15.IsPCIDSSRed || row15.IsSOXRed);
                rowList.Add(row15);
                count++;
            }

            if (count > 0)
            {
                RegulatoryComplianceRowData row16 = new RegulatoryComplianceRowData();
                row16.FieldName = "Audited Activities";
                row16.IsHeader = true;
                rowList.Add(row16);
            }

            RegulatoryComplianceRowData row17 = new RegulatoryComplianceRowData();
            row17.FieldName = "Current Settings are Same as the Regulatory Guidelines Applied";
            row17.IsHeader = false;
            row17.FieldType = (int)FieldType.Text;
            row17.IsFieldNameRed = false;
            row17.IsCISChecked = ((userAppliedServerSettings & CISServerSettings) == CISServerSettings);
            row17.IsCISRed = false;
            row17.IsDISASTIGChecked = ((userAppliedServerSettings & DISASTIGServerSettings) == DISASTIGServerSettings);
            row17.IsDISASTIGRed = false;
            row17.IsFERPAChecked = ((userAppliedServerSettings & FERPAServerSettings) == FERPAServerSettings);
            row17.IsFERPARed = false;
            row17.IsGDPRChecked = ((userAppliedServerSettings & GDPRServerSettings) == GDPRServerSettings);
            row17.IsGDPRRed = false;
            row17.IsHIPAAChecked = ((userAppliedServerSettings & HIPAAServerSettings) == HIPAAServerSettings);
            row17.IsHIPAARed = false;
            row17.IsNERCChecked = ((userAppliedServerSettings & NERCServerSettings) == NERCServerSettings);
            row17.IsNERCRed = false;
            row17.IsPCIDSSChecked = ((userAppliedServerSettings & PCIDSSServerSettings) == PCIDSSServerSettings);
            row17.IsPCIDSSRed = false;
            row17.IsSOXChecked = ((userAppliedServerSettings & SOXServerSettings) == SOXServerSettings);
            row17.IsSOXRed = false;
            rowList.Add(row17);

            RegulatoryComplianceRowData row18 = new RegulatoryComplianceRowData();
            row18.FieldName = "Regulatory Guidelines Applied";
            row18.IsHeader = false;
            row18.FieldType = (int)FieldType.Text;
            row18.IsFieldNameRed = false;
            row18.IsCISChecked = ((userAppliedRegulations & (int)Regulation.RegulationTypeUser.CIS) == (int)Regulation.RegulationTypeUser.CIS);
            row18.IsCISRed = false;
            row18.IsDISASTIGChecked = ((userAppliedRegulations & (int)Regulation.RegulationTypeUser.DISA) == (int)Regulation.RegulationTypeUser.DISA);
            row18.IsDISASTIGRed = false;
            row18.IsFERPAChecked = ((userAppliedRegulations & (int)Regulation.RegulationTypeUser.FERPA) == (int)Regulation.RegulationTypeUser.FERPA);
            row18.IsFERPARed = false;
            row18.IsGDPRChecked = ((userAppliedRegulations & (int)Regulation.RegulationTypeUser.GDPR) == (int)Regulation.RegulationTypeUser.GDPR);
            row18.IsGDPRRed = false;
            row18.IsHIPAAChecked = ((userAppliedRegulations & (int)Regulation.RegulationTypeUser.HIPAA) == (int)Regulation.RegulationTypeUser.HIPAA);
            row18.IsHIPAARed = false;
            row18.IsNERCChecked = ((userAppliedRegulations & (int)Regulation.RegulationTypeUser.NERC) == (int)Regulation.RegulationTypeUser.NERC);
            row18.IsNERCRed = false;
            row18.IsPCIDSSChecked = ((userAppliedRegulations & (int)Regulation.RegulationTypeUser.PCI) == (int)Regulation.RegulationTypeUser.PCI);
            row18.IsPCIDSSRed = false;
            row18.IsSOXChecked = ((userAppliedRegulations & (int)Regulation.RegulationTypeUser.SOX) == (int)Regulation.RegulationTypeUser.SOX);
            row18.IsSOXRed = false;
            rowList.Add(row18);

            rowList.Reverse();
            result.RowList = rowList;

            return result;
        }

        internal RegulatoryComplianceData TransformRegulatoryComplianceDatabase(SqlDataReader reader, RegulatoryComplianceRequest request)
        {
            int auditDDL, auditAdmin, auditSecurity, auditDML, auditSelect, auditAccessCheck, auditCaptureSQL, auditCaptureTrans,
                auditCaptureDDL, userAppliedDatabaseSettings, pci, hipaa, disa, nerc, cis, sox, ferpa, gdpr;

            RegulatoryComplianceData result = new RegulatoryComplianceData();
            List<RegulatoryComplianceRowData> rowList = new List<RegulatoryComplianceRowData>();
            int count;

            result.ServerName = ReadData<string>(reader, "srvInstance");
            result.DatabaseName = ReadData<string>(reader, "name");
            result.IsDatabase = true;
            result.ShowCIS = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.CIS);
            result.ShowDISASTIG = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.DISASTIG);
            result.ShowFERPA = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.FERPA);
            result.ShowGDPR = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.GDPR);
            result.ShowHIPAA = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.HIPAA);
            result.ShowNERC = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.NERC);
            result.ShowPCIDSS = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.PCIDSS);
            result.ShowSOX = (request.RegulationGuidelines == (int)RegulationId.All || request.RegulationGuidelines == (int)RegulationId.SOX);

            auditDDL = ReadData<int>(reader, "auditDDL");
            auditAdmin = ReadData<int>(reader, "auditAdmin");
            auditSecurity = ReadData<int>(reader, "auditSecurity");
            auditDML = ReadData<int>(reader, "auditDML");
            auditSelect = ReadData<int>(reader, "auditSELECT");
            auditAccessCheck = ReadData<int>(reader, "auditFailures");
            auditCaptureSQL = ReadData<int>(reader, "auditCaptureSQL");
            auditCaptureTrans = ReadData<int>(reader, "auditCaptureTrans");
            auditCaptureDDL = ReadData<int>(reader, "auditCaptureDDL");
            userAppliedDatabaseSettings = ReadData<int>(reader, "userAppliedDatabaseSettings");
            pci = ReadData<int>(reader, "pci");
            hipaa = ReadData<int>(reader, "hipaa");
            disa = ReadData<int>(reader, "disa");
            nerc = ReadData<int>(reader, "nerc");
            cis = ReadData<int>(reader, "cis");
            sox = ReadData<int>(reader, "sox");
            ferpa = ReadData<int>(reader, "ferpa");
            gdpr = ReadData<int>(reader, "gdpr");

            count = 0;
            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.CaptureDDL) &&
                (request.Values == (int)Values.Selected && auditCaptureDDL == 1) ||
                (request.Values == (int)Values.Deselected && auditCaptureDDL == 0))
            {
                RegulatoryComplianceRowData row1 = new RegulatoryComplianceRowData();
                row1.FieldName = "Capture SQL Statements for DDL and Security Changes";
                row1.IsHeader = false;
                row1.FieldType = (int)FieldType.Checkbox;
                row1.IsCISChecked = (auditCaptureDDL == 1);
                row1.IsCISRed = false;
                row1.IsDISASTIGChecked = (auditCaptureDDL == 1);
                row1.IsDISASTIGRed = false;
                row1.IsFERPAChecked = (auditCaptureDDL == 1);
                row1.IsFERPARed = false;
                row1.IsGDPRChecked = (auditCaptureDDL == 1);
                row1.IsGDPRRed = false;
                row1.IsHIPAAChecked = (auditCaptureDDL == 1);
                row1.IsHIPAARed = false;
                row1.IsNERCChecked = (auditCaptureDDL == 1);
                row1.IsNERCRed = false;
                row1.IsPCIDSSChecked = (auditCaptureDDL == 1);
                row1.IsPCIDSSRed = false;
                row1.IsSOXChecked = (auditCaptureDDL == 1);
                row1.IsSOXRed = false;
                row1.IsFieldNameRed = false;
                rowList.Add(row1);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.CaptureTrans) &&
                (request.Values == (int)Values.Selected && auditCaptureTrans == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.Transactions, false)) ||
                (request.Values == (int)Values.Deselected && auditCaptureTrans == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.Transactions, false)) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.Transactions, false)))
            {
                RegulatoryComplianceRowData row2 = new RegulatoryComplianceRowData();
                row2.FieldName = "Capture Transaction Status for DML Activity";
                row2.IsHeader = false;
                row2.FieldType = (int)FieldType.Checkbox;
                row2.IsCISChecked = (auditCaptureTrans == 1);
                row2.IsCISRed = (auditCaptureTrans == 0 && (CISDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions);
                row2.IsDISASTIGChecked = (auditCaptureTrans == 1);
                row2.IsDISASTIGRed = (auditCaptureTrans == 0 && (DISASTIGDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions);
                row2.IsFERPAChecked = (auditCaptureTrans == 1);
                row2.IsFERPARed = (auditCaptureTrans == 0 && (FERPADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions);
                row2.IsGDPRChecked = (auditCaptureTrans == 1);
                row2.IsGDPRRed = (auditCaptureTrans == 0 && (GDPRDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions);
                row2.IsHIPAAChecked = (auditCaptureTrans == 1);
                row2.IsHIPAARed = (auditCaptureTrans == 0 && (HIPAADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions);
                row2.IsNERCChecked = (auditCaptureTrans == 1);
                row2.IsNERCRed = (auditCaptureTrans == 0 && (NERCDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions);
                row2.IsPCIDSSChecked = (auditCaptureTrans == 1);
                row2.IsPCIDSSRed = (auditCaptureTrans == 0 && (PCIDSSDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions);
                row2.IsSOXChecked = (auditCaptureTrans == 1);
                row2.IsSOXRed = (auditCaptureTrans == 0 && (SOXDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Transactions) == (int)RegulationSettings.RegulationDatabaseCategory.Transactions);
                row2.IsFieldNameRed = (row2.IsCISRed || row2.IsDISASTIGRed || row2.IsFERPARed || row2.IsGDPRRed ||
                                    row2.IsHIPAARed || row2.IsNERCRed || row2.IsPCIDSSRed || row2.IsSOXRed);
                rowList.Add(row2);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.CaptureSQL) &&
                (request.Values == (int)Values.Selected && auditCaptureSQL == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.SQLText, false)) ||
                (request.Values == (int)Values.Deselected && auditCaptureSQL == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.SQLText, false)) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.SQLText, false)))
            {
                RegulatoryComplianceRowData row3 = new RegulatoryComplianceRowData();
                row3.FieldName = "Capture SQL Statements for DML and SELECT activities";
                row3.IsHeader = false;
                row3.FieldType = (int)FieldType.Checkbox;
                row3.IsCISChecked = (auditCaptureSQL == 1);
                row3.IsCISRed = (auditCaptureSQL == 0 && (CISDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText);
                row3.IsDISASTIGChecked = (auditCaptureSQL == 1);
                row3.IsDISASTIGRed = (auditCaptureSQL == 0 && (DISASTIGDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText);
                row3.IsFERPAChecked = (auditCaptureSQL == 1);
                row3.IsFERPARed = (auditCaptureSQL == 0 && (FERPADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText);
                row3.IsGDPRChecked = (auditCaptureSQL == 1);
                row3.IsGDPRRed = (auditCaptureSQL == 0 && (GDPRDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText);
                row3.IsHIPAAChecked = (auditCaptureSQL == 1);
                row3.IsHIPAARed = (auditCaptureSQL == 0 && (HIPAADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText);
                row3.IsNERCChecked = (auditCaptureSQL == 1);
                row3.IsNERCRed = (auditCaptureSQL == 0 && (NERCDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText);
                row3.IsPCIDSSChecked = (auditCaptureSQL == 1);
                row3.IsPCIDSSRed = (auditCaptureSQL == 0 && (PCIDSSDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText);
                row3.IsSOXChecked = (auditCaptureSQL == 1);
                row3.IsSOXRed = (auditCaptureSQL == 0 && (SOXDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SQLText) == (int)RegulationSettings.RegulationDatabaseCategory.SQLText);
                row3.IsFieldNameRed = (row3.IsCISRed || row3.IsDISASTIGRed || row3.IsFERPARed || row3.IsGDPRRed ||
                                    row3.IsHIPAARed || row3.IsNERCRed || row3.IsPCIDSSRed || row3.IsSOXRed);
                rowList.Add(row3);
                count++;
            }

            if (count > 0)
            {
                RegulatoryComplianceRowData row4 = new RegulatoryComplianceRowData();
                row4.FieldName = "";
                row4.IsHeader = true;
                rowList.Add(row4);
            }

            count = 0;
            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.AccessCheckFilter) &&
                (request.Values == (int)Values.Selected && auditAccessCheck == 2 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess, false)) ||
                (request.Values == (int)Values.Deselected && auditAccessCheck != 2 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess, false)) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess, false)))
            {
                RegulatoryComplianceRowData row5 = new RegulatoryComplianceRowData();
                row5.FieldName = "Failed";
                row5.IsHeader = false;
                row5.FieldType = (int)FieldType.RadioButton;
                row5.IsCISChecked = (auditAccessCheck == 2);
                row5.IsCISRed = (auditAccessCheck != 2 && (CISDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess);
                row5.IsDISASTIGChecked = (auditAccessCheck == 2);
                row5.IsDISASTIGRed = (auditAccessCheck != 2 && (DISASTIGDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess);
                row5.IsFERPAChecked = (auditAccessCheck == 2);
                row5.IsFERPARed = (auditAccessCheck != 2 && (FERPADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess);
                row5.IsGDPRChecked = (auditAccessCheck == 2);
                row5.IsGDPRRed = (auditAccessCheck != 2 && (GDPRDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess);
                row5.IsHIPAAChecked = (auditAccessCheck == 2);
                row5.IsHIPAARed = (auditAccessCheck != 2 && (HIPAADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess);
                row5.IsNERCChecked = (auditAccessCheck == 2);
                row5.IsNERCRed = (auditAccessCheck != 2 && (NERCDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess);
                row5.IsPCIDSSChecked = (auditAccessCheck == 2);
                row5.IsPCIDSSRed = (auditAccessCheck != 2 && (PCIDSSDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess);
                row5.IsSOXChecked = (auditAccessCheck == 2);
                row5.IsSOXRed = (auditAccessCheck != 2 && (SOXDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterFailedAccess);
                row5.IsFieldNameRed = (row5.IsCISRed || row5.IsDISASTIGRed || row5.IsFERPARed || row5.IsGDPRRed ||
                                    row5.IsHIPAARed || row5.IsNERCRed || row5.IsPCIDSSRed || row5.IsSOXRed);
                rowList.Add(row5);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.AccessCheckFilter) &&
                (request.Values == (int)Values.Selected && auditAccessCheck == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess, true)) ||
                (request.Values == (int)Values.Deselected && auditAccessCheck != 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess, true)) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess, true)))
            {
                RegulatoryComplianceRowData row6 = new RegulatoryComplianceRowData();
                row6.FieldName = "Passed";
                row6.IsHeader = false;
                row6.FieldType = (int)FieldType.RadioButton;
                row6.IsCISChecked = (auditAccessCheck == 0);
                row6.IsCISRed = (auditAccessCheck != 0 && (CISDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess);
                row6.IsDISASTIGChecked = (auditAccessCheck == 0);
                row6.IsDISASTIGRed = (auditAccessCheck != 0 && (DISASTIGDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess);
                row6.IsFERPAChecked = (auditAccessCheck == 0);
                row6.IsFERPARed = (auditAccessCheck != 0 && (FERPADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess);
                row6.IsGDPRChecked = (auditAccessCheck == 0);
                row6.IsGDPRRed = (auditAccessCheck != 0 && (GDPRDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess);
                row6.IsHIPAAChecked = (auditAccessCheck == 0);
                row6.IsHIPAARed = (auditAccessCheck != 0 && (HIPAADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess);
                row6.IsNERCChecked = (auditAccessCheck == 0);
                row6.IsNERCRed = (auditAccessCheck != 0 && (NERCDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess);
                row6.IsPCIDSSChecked = (auditAccessCheck == 0);
                row6.IsPCIDSSRed = (auditAccessCheck != 0 && (PCIDSSDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess);
                row6.IsSOXChecked = (auditAccessCheck == 0);
                row6.IsSOXRed = (auditAccessCheck != 0 && (SOXDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess) == (int)RegulationSettings.RegulationDatabaseCategory.FilterPassedAccess);
                row6.IsFieldNameRed = (row6.IsCISRed || row6.IsDISASTIGRed || row6.IsFERPARed || row6.IsGDPRRed ||
                                    row6.IsHIPAARed || row6.IsNERCRed || row6.IsPCIDSSRed || row6.IsSOXRed);
                rowList.Add(row6);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.AccessCheckFilter) &&
                (request.Values == (int)Values.Selected && auditAccessCheck != 1) ||
                (request.Values == (int)Values.Deselected && auditAccessCheck == 1))
            {
                RegulatoryComplianceRowData row7 = new RegulatoryComplianceRowData();
                row7.FieldName = "Filter Events based on access check";
                row7.IsHeader = false;
                row7.FieldType = (int)FieldType.Checkbox;
                row7.IsCISChecked = (auditAccessCheck != 1);
                row7.IsCISRed = false;
                row7.IsDISASTIGChecked = (auditAccessCheck != 1);
                row7.IsDISASTIGRed = false;
                row7.IsFERPAChecked = (auditAccessCheck != 1);
                row7.IsFERPARed = false;
                row7.IsGDPRChecked = (auditAccessCheck != 1);
                row7.IsGDPRRed = false;
                row7.IsHIPAAChecked = (auditAccessCheck != 1);
                row7.IsHIPAARed = false;
                row7.IsNERCChecked = (auditAccessCheck != 1);
                row7.IsNERCRed = false;
                row7.IsPCIDSSChecked = (auditAccessCheck != 1);
                row7.IsPCIDSSRed = false;
                row7.IsSOXChecked = (auditAccessCheck != 1);
                row7.IsSOXRed = false;
                row7.IsFieldNameRed = false;
                rowList.Add(row7);
                count++;
            }

            if (count > 0)
            {
                RegulatoryComplianceRowData row8 = new RegulatoryComplianceRowData();
                row8.FieldName = "Access Check Filter";
                row8.IsHeader = true;
                rowList.Add(row8);
            }
            
            count = 0;
            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.DatabaseSelect) &&
                (request.Values == (int)Values.Selected && auditSelect == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.Select, false)) ||
                (request.Values == (int)Values.Deselected && auditSelect == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.Select, false)) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.Select, false)))
            {
                RegulatoryComplianceRowData row9 = new RegulatoryComplianceRowData();
                row9.FieldName = "Database SELECT operations";
                row9.IsHeader = false;
                row9.FieldType = (int)FieldType.Checkbox;
                row9.IsCISChecked = (auditSelect == 1);
                row9.IsCISRed = (auditSelect == 0 && (CISDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select);
                row9.IsDISASTIGChecked = (auditSelect == 1);
                row9.IsDISASTIGRed = (auditSelect == 0 && (DISASTIGDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select);
                row9.IsFERPAChecked = (auditSelect == 1);
                row9.IsFERPARed = (auditSelect == 0 && (FERPADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select);
                row9.IsGDPRChecked = (auditSelect == 1);
                row9.IsGDPRRed = (auditSelect == 0 && (GDPRDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select);
                row9.IsHIPAAChecked = (auditSelect == 1);
                row9.IsHIPAARed = (auditSelect == 0 && (HIPAADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select);
                row9.IsNERCChecked = (auditSelect == 1);
                row9.IsNERCRed = (auditSelect == 0 && (NERCDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select);
                row9.IsPCIDSSChecked = (auditSelect == 1);
                row9.IsPCIDSSRed = (auditSelect == 0 && (PCIDSSDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select);
                row9.IsSOXChecked = (auditSelect == 1);
                row9.IsSOXRed = (auditSelect == 0 && (SOXDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.Select) == (int)RegulationSettings.RegulationDatabaseCategory.Select);
                row9.IsFieldNameRed = (row9.IsCISRed || row9.IsDISASTIGRed || row9.IsFERPARed || row9.IsGDPRRed ||
                                    row9.IsHIPAARed || row9.IsNERCRed || row9.IsPCIDSSRed || row9.IsSOXRed);
                rowList.Add(row9);
                count++;
            }

            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.DatabaseModification) &&
                (request.Values == (int)Values.Selected && auditDML == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification, false)) ||
                (request.Values == (int)Values.Deselected && auditDML == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification, false)) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification, false)))
            {
                RegulatoryComplianceRowData row10 = new RegulatoryComplianceRowData();
                row10.FieldName = "Database Modification (DML)";
                row10.IsHeader = false;
                row10.FieldType = (int)FieldType.Checkbox;
                row10.IsCISChecked = (auditDML == 1);
                row10.IsCISRed = (auditDML == 0 && (CISDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification);
                row10.IsDISASTIGChecked = (auditDML == 1);
                row10.IsDISASTIGRed = (auditDML == 0 && (DISASTIGDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification);
                row10.IsFERPAChecked = (auditDML == 1);
                row10.IsFERPARed = (auditDML == 0 && (FERPADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification);
                row10.IsGDPRChecked = (auditDML == 1);
                row10.IsGDPRRed = (auditDML == 0 && (GDPRDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification);
                row10.IsHIPAAChecked = (auditDML == 1);
                row10.IsHIPAARed = (auditDML == 0 && (HIPAADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification);
                row10.IsNERCChecked = (auditDML == 1);
                row10.IsNERCRed = (auditDML == 0 && (NERCDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification);
                row10.IsPCIDSSChecked = (auditDML == 1);
                row10.IsPCIDSSRed = (auditDML == 0 && (PCIDSSDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification);
                row10.IsSOXChecked = (auditDML == 1);
                row10.IsSOXRed = (auditDML == 0 && (SOXDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseModification);
                row10.IsFieldNameRed = (row10.IsCISRed || row10.IsDISASTIGRed || row10.IsFERPARed || row10.IsGDPRRed ||
                                    row10.IsHIPAARed || row10.IsNERCRed || row10.IsPCIDSSRed || row10.IsSOXRed);
                rowList.Add(row10);
                count++;
            }
			
            if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.DatabaseDefinition) &&
                (request.Values == (int)Values.Selected && auditDDL == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition, false)) ||
                (request.Values == (int)Values.Deselected && auditDDL == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition, false)) ||
                (request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition, false)))
            {
                RegulatoryComplianceRowData row13 = new RegulatoryComplianceRowData();
                row13.FieldName = "Database Definition (DDL)";
                row13.IsHeader = false;
                row13.FieldType = (int)FieldType.Checkbox;
                row13.IsCISChecked = (auditDDL == 1);
                row13.IsCISRed = (auditDDL == 0 && (CISDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition);
                row13.IsDISASTIGChecked = (auditDDL == 1);
                row13.IsDISASTIGRed = (auditDDL == 0 && (DISASTIGDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition);
                row13.IsFERPAChecked = (auditDDL == 1);
                row13.IsFERPARed = (auditDDL == 0 && (FERPADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition);
                row13.IsGDPRChecked = (auditDDL == 1);
                row13.IsGDPRRed = (auditDDL == 0 && (GDPRDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition);
                row13.IsHIPAAChecked = (auditDDL == 1);
                row13.IsHIPAARed = (auditDDL == 0 && (HIPAADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition);
                row13.IsNERCChecked = (auditDDL == 1);
                row13.IsNERCRed = (auditDDL == 0 && (NERCDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition);
                row13.IsPCIDSSChecked = (auditDDL == 1);
                row13.IsPCIDSSRed = (auditDDL == 0 && (PCIDSSDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition);
                row13.IsSOXChecked = (auditDDL == 1);
                row13.IsSOXRed = (auditDDL == 0 && (SOXDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition) == (int)RegulationSettings.RegulationDatabaseCategory.DatabaseDefinition);
                row13.IsFieldNameRed = (row13.IsCISRed || row13.IsDISASTIGRed || row13.IsFERPARed || row13.IsGDPRRed ||
                                    row13.IsHIPAARed || row13.IsNERCRed || row13.IsPCIDSSRed || row13.IsSOXRed);
                rowList.Add(row13);
                count++;
            }

			if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.AdministrativeActions) &&
				(request.Values == (int)Values.Selected && auditAdmin == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity, false)) ||
				(request.Values == (int)Values.Deselected && auditAdmin == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity, false)) ||
				(request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity, false)))
			{
				RegulatoryComplianceRowData row11 = new RegulatoryComplianceRowData();
				row11.FieldName = "Administrative Actions";
				row11.IsHeader = false;
				row11.FieldType = (int)FieldType.Checkbox;
				row11.IsCISChecked = (auditAdmin == 1);
				row11.IsCISRed = (auditAdmin == 0 && (CISDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity);
				row11.IsDISASTIGChecked = (auditAdmin == 1);
				row11.IsDISASTIGRed = (auditAdmin == 0 && (DISASTIGDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity);
				row11.IsFERPAChecked = (auditAdmin == 1);
				row11.IsFERPARed = (auditAdmin == 0 && (FERPADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity);
				row11.IsGDPRChecked = (auditAdmin == 1);
				row11.IsGDPRRed = (auditAdmin == 0 && (GDPRDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity);
				row11.IsHIPAAChecked = (auditAdmin == 1);
				row11.IsHIPAARed = (auditAdmin == 0 && (HIPAADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity);
				row11.IsNERCChecked = (auditAdmin == 1);
				row11.IsNERCRed = (auditAdmin == 0 && (NERCDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity);
				row11.IsPCIDSSChecked = (auditAdmin == 1);
				row11.IsPCIDSSRed = (auditAdmin == 0 && (PCIDSSDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity);
				row11.IsSOXChecked = (auditAdmin == 1);
				row11.IsSOXRed = (auditAdmin == 0 && (SOXDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity) == (int)RegulationSettings.RegulationDatabaseCategory.AdminActivity);
				row11.IsFieldNameRed = (row11.IsCISRed || row11.IsDISASTIGRed || row11.IsFERPARed || row11.IsGDPRRed ||
									row11.IsHIPAARed || row11.IsNERCRed || row11.IsPCIDSSRed || row11.IsSOXRed);
				rowList.Add(row11);
				count++;
			}

			if ((request.AuditSettings == (int)AuditSettings.All || request.AuditSettings == (int)AuditSettings.SecurityChanges) &&
				(request.Values == (int)Values.Selected && auditSecurity == 1 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges, false)) ||
				(request.Values == (int)Values.Deselected && auditSecurity == 0 && IsPartOfAtleastOne(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges, false)) ||
				(request.Values == (int)Values.NA && IsPartOfNone(request.RegulationGuidelines, (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges, false)))
			{
				RegulatoryComplianceRowData row12 = new RegulatoryComplianceRowData();
				row12.FieldName = "Security Changes";
				row12.IsHeader = false;
				row12.FieldType = (int)FieldType.Checkbox;
				row12.IsCISChecked = (auditSecurity == 1);
				row12.IsCISRed = (auditSecurity == 0 && (CISDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges);
				row12.IsDISASTIGChecked = (auditSecurity == 1);
				row12.IsDISASTIGRed = (auditSecurity == 0 && (DISASTIGDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges);
				row12.IsFERPAChecked = (auditSecurity == 1);
				row12.IsFERPARed = (auditSecurity == 0 && (FERPADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges);
				row12.IsGDPRChecked = (auditSecurity == 1);
				row12.IsGDPRRed = (auditSecurity == 0 && (GDPRDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges);
				row12.IsHIPAAChecked = (auditSecurity == 1);
				row12.IsHIPAARed = (auditSecurity == 0 && (HIPAADatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges);
				row12.IsNERCChecked = (auditSecurity == 1);
				row12.IsNERCRed = (auditSecurity == 0 && (NERCDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges);
				row12.IsPCIDSSChecked = (auditSecurity == 1);
				row12.IsPCIDSSRed = (auditSecurity == 0 && (PCIDSSDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges);
				row12.IsSOXChecked = (auditSecurity == 1);
				row12.IsSOXRed = (auditSecurity == 0 && (SOXDatabaseSettings & (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges) == (int)RegulationSettings.RegulationDatabaseCategory.SecurityChanges);
				row12.IsFieldNameRed = (row12.IsCISRed || row12.IsDISASTIGRed || row12.IsFERPARed || row12.IsGDPRRed ||
									row12.IsHIPAARed || row12.IsNERCRed || row12.IsPCIDSSRed || row12.IsSOXRed);
				rowList.Add(row12);
				count++;
			}

			if (count > 0)
            {
                RegulatoryComplianceRowData row14 = new RegulatoryComplianceRowData();
                row14.FieldName = "Audited Activities";
                row14.IsHeader = true;
                rowList.Add(row14);
            }

            RegulatoryComplianceRowData row15 = new RegulatoryComplianceRowData();
            row15.FieldName = "Current Settings are Same as the Regulatory Guidelines Applied";
            row15.IsHeader = false;
            row15.FieldType = (int)FieldType.Text;
            row15.IsFieldNameRed = false;
            row15.IsCISChecked = ((userAppliedDatabaseSettings & CISDatabaseSettings) == CISDatabaseSettings);
            row15.IsCISRed = false;
            row15.IsDISASTIGChecked = ((userAppliedDatabaseSettings & DISASTIGDatabaseSettings) == DISASTIGDatabaseSettings);
            row15.IsDISASTIGRed = false;
            row15.IsFERPAChecked = ((userAppliedDatabaseSettings & FERPADatabaseSettings) == FERPADatabaseSettings);
            row15.IsFERPARed = false;
            row15.IsGDPRChecked = ((userAppliedDatabaseSettings & GDPRDatabaseSettings) == GDPRDatabaseSettings);
            row15.IsGDPRRed = false;
            row15.IsHIPAAChecked = ((userAppliedDatabaseSettings & HIPAADatabaseSettings) == HIPAADatabaseSettings);
            row15.IsHIPAARed = false;
            row15.IsNERCChecked = ((userAppliedDatabaseSettings & NERCDatabaseSettings) == NERCDatabaseSettings);
            row15.IsNERCRed = false;
            row15.IsPCIDSSChecked = ((userAppliedDatabaseSettings & PCIDSSDatabaseSettings) == PCIDSSDatabaseSettings);
            row15.IsPCIDSSRed = false;
            row15.IsSOXChecked = ((userAppliedDatabaseSettings & SOXDatabaseSettings) == SOXDatabaseSettings);
            row15.IsSOXRed = false;
            rowList.Add(row15);

            RegulatoryComplianceRowData row16 = new RegulatoryComplianceRowData();
            row16.FieldName = "Regulatory Guidelines Applied";
            row16.IsHeader = false;
            row16.FieldType = (int)FieldType.Text;
            row16.IsFieldNameRed = false;
            row16.IsCISChecked = (cis == 1);
            row16.IsCISRed = false;
            row16.IsDISASTIGChecked = (disa == 1);
            row16.IsDISASTIGRed = false;
            row16.IsFERPAChecked = (ferpa == 1);
            row16.IsFERPARed = false;
            row16.IsGDPRChecked = (gdpr == 1);
            row16.IsGDPRRed = false;
            row16.IsHIPAAChecked = (hipaa == 1);
            row16.IsHIPAARed = false;
            row16.IsNERCChecked = (nerc == 1);
            row16.IsNERCRed = false;
            row16.IsPCIDSSChecked = (pci == 1);
            row16.IsPCIDSSRed = false;
            row16.IsSOXChecked = (sox == 1);
            row16.IsSOXRed = false;
            rowList.Add(row16);

            rowList.Reverse();
            result.RowList = rowList;
            return result;
        }

        internal DataAlertRulesTableDetail TransformDataAlertRulesTableDetail(IDataReader reader)
        {           
            var result = new DataAlertRulesTableDetail();
            result.SrvId = ReadData<int>(reader, "srvId");
            result.DbId = ReadData<int>(reader, "dbId");
            result.ObjectId = ReadData<int>(reader, "objectId");
            result.SchemaName = ReadData<string>(reader, "schemaName");
            result.Name = ReadData<string>(reader, "tableName");
            result.SelectedColumn = ReadData<int>(reader,"selectedColumns");
            return result;
        }


        internal DataAlertRulesDBDetail TransformDataAlertRulesDBDetail(IDataReader reader)
        {
            var result = new DataAlertRulesDBDetail();
            result.SrvId = ReadData<int>(reader, "srvId");
            result.Name = ReadData<string>(reader, "name");
            result.DbId = ReadData<int>(reader, "dbId");
            return result;
        }

        internal DataAlertRulesColumnDetail TransformDataAlertRulesColumnDetail(IDataReader reader)
        {
            var result = new DataAlertRulesColumnDetail();
            result.Name = ReadData<string>(reader, "name");
            result.DbId = ReadData<int>(reader, "dbId");
            result.ObjectId = ReadData<int>(reader, "objectId");
            result.SrvId = ReadData<int>(reader, "srvId");
            return result;
        }

        internal CategoryData TransformCategoryData(IDataReader reader)
        {
            var result = new CategoryData();
            result.Name = ReadData<string>(reader, "name");
            result.EvTypeId = ReadData<string>(reader, "evtypeid");
            return result;
        }

        internal ViewNameData TransformViewNameData(IDataReader reader)
        {
            var result = new ViewNameData();
            result.ViewName = ReadData<string>(reader, "ViewName");
            return result;
        }

        internal SNMPConfigData TransformSNMPConfigData(IDataReader reader)
        {
            var result = new SNMPConfigData();
            result.SenderEmail = ReadData<string>(reader, "sender_email");
            result.LogsPermission = ReadData<bool>(reader, "logs_permission");
            result.SendMailPermission = ReadData<bool>(reader, "send_mail_permission");
            result.SnmpCommunity = ReadData<string>(reader, "snmpCommunity");
            result.SnmpPermission = ReadData<bool>(reader, "snmp_permission");
            result.SnmpPort = ReadData<int>(reader, "snmpPort");
            result.SnmpServerAddress = ReadData<string>(reader, "snmpAddress");
            result.Severity = ReadData<int>(reader, "severity");
            result.MessageData = ReadData<string>(reader, "messageData");
            return result;
        } 
//Start SQLCm-5.4 
        //Start - Requirement - 4.1.3.1

        internal SensitiveColumnDatabaseDetails TransformSensitiveColumnDBDetail(String name, int dbId, bool isSelected)
        {
            var result = new SensitiveColumnDatabaseDetails();
            result.Name = name;
            result.DbId = dbId;
            result.Selected = isSelected;
            return result;
        }

        internal SensitiveColumnTableDetails TransformSensitiveColumnTableDetail(String name, int tblId, int dbId, bool isSelected)
        {
            var result = new SensitiveColumnTableDetails();
            result.DbId = dbId;
            result.TblId = tblId;
            result.Name = name;
            result.Selected = isSelected;
            return result;
        }

        internal SensitiveColumnColumnDetail TransformSensitiveColumnColumnDetail(String name, int colId, int tblId, int dbId, bool isSelected)
        {
            var result = new SensitiveColumnColumnDetail();
            result.Name = name;
            result.DbId = dbId;
            result.ColId = colId;
            result.TblId = tblId;
            result.Selected = isSelected;
            return result;
        }
        //End - Requirement - 4.1.3.1

        //Start - Requirement - 4.1.4.1
        internal DatabaseDetails TransformDBDetails(String name, int dbId, int srvId)
        {
            var result = new DatabaseDetails();
            result.dbName = name;
            result.dbId = dbId;
            result.srvId = srvId;
            return result;
        }

        internal ServerDetails TransformServerDetails(String name, int srvId)
        {
            var result = new ServerDetails();
            result.serverName = name;
            result.serverId = srvId;
            return result;
        }
    }
}
