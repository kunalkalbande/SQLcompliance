using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Idera.SQLcompliance.Core.Event;
using Idera.SQLcompliance.Core.Rules.Filters;
using SQLcomplianceCwfAddin.Helpers.Agent;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Alerts;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ActivityLogs;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.ManagedInstances.Credentials;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.UserSettings;
using SQLcomplianceCwfAddin.Helpers.SQL;
using Idera.SQLcompliance.Core.Agent;
using Idera.SQLcompliance.Core;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using Idera.SQLcompliance.Core.Stats;
using SQLcomplianceCwfAddin.Helpers.Stats;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AuditReports;
using System.Text.RegularExpressions;
using Idera.SQLcompliance.Core.Templates;
using Idera.SQLcompliance.Core.Rules;
using System.Collections;
using System.IO;
using System.Security.Principal;
using Idera.SQLcompliance.Core.Collector;
using Idera.SQLcompliance.Core.Templates.IOHandlers;
using Idera.SQLcompliance.Core.Templates.AuditSettings;
using Idera.SQLcompliance.Core.Status;
using Idera.SQLcompliance.Core.Remoting;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using System.Xml;
using System.Xml.Serialization;


namespace SQLcomplianceCwfAddin.Helpers
{
    internal class QueryExecutor
    {
        // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
        // Bypass the sysadmin permissions by getting registered dll information during server startup
        private const string SqlCmStartupDllChecker = 
@"USE [master]; SELECT [DllName], [DllVersion] FROM SqlCmStartupDllChecker 
WHERE [DllName] IN ('Microsoft.SqlServer.XEvent.Linq.dll', 'Microsoft.SqlServer.XE.Core.dll')";
        private const string XeLinqDll = "Microsoft.SqlServer.XEvent.Linq.dll";
        private const string XeCoreDll = "Microsoft.SqlServer.XE.Core.dll";

        #region members

        AllImportSettingDetails allImportSettingDetails;
        private static QueryExecutor _instance;
        ICollection _tablelist;

        #endregion

        #region properties

        public static QueryExecutor Instance
        {
            get { return _instance ?? (_instance = new QueryExecutor()); }
        }

        #endregion

        #region internal\private methods

        private int GetSqlServerVersion(string versionString)
        {
            if (string.IsNullOrEmpty(versionString))
                return 0;

            var versionParts = versionString.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (versionParts.Length == 0)
                return 0;

            return Convert.ToInt32(versionParts[0]);
        }

        internal int GetProcessedEventCount(string connectionString, string eventDatabase)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (var command = new SqlCommand(QueryBuilder.Instance.GetProcessedEventCount(eventDatabase), connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        internal int GetAlertCount(string connectionString, string server)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (var command = new SqlCommand(QueryBuilder.Instance.GetAlertCount(server), connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        internal int GetAuditedDatabaseCount(string connectionString, int serverId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(QueryBuilder.Instance.GetAuditedDatabaseCount(serverId), connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        internal List<string> GetEnabledEventFiltersForInstance(string connectionString, string instance)
        {
            var result = new List<string>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = QueryBuilder.Instance.GetEnabledEventFiltersForInstance(instance);
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(reader["name"].ToString());

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal string GetEventDatabaseName(string connectionString, int serverId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = QueryBuilder.Instance.GetInstanceInfo(serverId);
                using (var command = new SqlCommand(query, connection))
                {
                    var instanceName = string.Empty;
                    var eventDatabase = string.Empty;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            instanceName = Convert.ToString(reader["Instance"]);
                            eventDatabase = Convert.ToString(reader["EventDatabase"]);
                        }
                    }

                    return string.IsNullOrEmpty(instanceName) ? null : eventDatabase;
                }
            }
        }

        internal int GetDatabaseCount(string instanceName)
        {
            var connectionString = string.Format("Data Source={0};Initial Catalog=master;Integrated Security=SSPI;", instanceName);
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(QueryBuilder.Instance.GetDatabaseCount(), connection))
                    {
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (SqlException ex)
            {
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal string GetAlertRuleName(string connectionString, AlertRuleType type, long ruleTypeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (var command = new SqlCommand(QueryBuilder.Instance.GetAlertRuleName(type, ruleTypeId), connection))
                {
                    return Convert.ToString(command.ExecuteScalar());
                }
            }
        }

        private static object GetValidSqlValue(string value)
        {
            return value ?? SqlString.Null;
        }

        private DateTime RemoveMillisecounds(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }

        private static DateTime RemoveTimePart(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        #endregion

        public IList<EventFilter> GetEventFiltersForDatabase(SqlConnection connection, string query, int serverId, int databaseId)
        {
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ServerId", SqlDbType.Int).Value = serverId;

                        using (var reader = command.ExecuteReader())
                        {
                            return Transformer.Instance.TransformEventFilterData(reader, serverId, databaseId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ServerRecord> GetAllServerRecords(SqlConnection connection)
        {
            List<ServerRecord> serverList = new List<ServerRecord>();

            using (var newConnection = new SqlConnection(connection.ConnectionString))
            {
                try
                {
                    serverList = ServerRecord.GetServers(connection, false);
                }
                catch
                {
                    //TODO: exception handling
                }
            }

            return serverList;
        }

        public void UpdateOtherProperties(List<AuditedServerStatus> result, FilteredRegisteredInstancesStatusRequest request, SqlConnection connection)
        {
            var allServerRecords = GetAllServerRecords(connection);
            UpdateAllwaysOnStutus(result, allServerRecords, connection);

            foreach (var serverStatus in result)
            {
                SetLocalDateTimes(serverStatus);

                if (request.UpdateStatisticProperties)
                {
                    var serverRecord = SqlCmRecordReader.FindServerRecord(serverStatus.Instance, allServerRecords);
                    SetStatisticCounts(serverRecord, serverStatus, request.Days.Value, connection);
                    SetServerAuditConfigurationList(serverRecord, serverStatus);
                    SetPrivilegedUsersAuditConfigurationList(serverRecord, serverStatus);
                    SetEventFiltersAuditConfigurationList(serverRecord, serverStatus, connection);
                }
            }
        }

        public void UpdateAllwaysOnStutus(List<AuditedServerStatus> result, List<ServerRecord> allServerRecords, SqlConnection connection)
        {
            using (var newConnection = new SqlConnection(connection.ConnectionString))
            {
                var updateStatisticsActions = result.Select(status => Task.Factory.StartNew(() => UpdatePrimaryStatusForInstances(allServerRecords, status, newConnection))).ToArray();
                Task.WaitAll(updateStatisticsActions);
            }
        }

        public void UpdatePrimaryStatusForInstances(List<ServerRecord> allServerRecords, AuditedServerStatus serverStatus, SqlConnection connection)
        {
            try
            {
                if (!PrimaryStatusForInstanceManager.Instance.IsStatusCanBeUpdated(serverStatus.Instance)) return;
                var serverRecord = SqlCmRecordReader.FindServerRecord(serverStatus.Instance, allServerRecords);
                var readOnlySecondaryReplicaList = AgentManagerHelper.Instance.GetAllReplicaNodeInfoList(serverRecord, connection);

                foreach (var replicaInfo in readOnlySecondaryReplicaList)
                {
                    if (replicaInfo.ReplicaServerName.ToLower() == serverRecord.Instance.ToLower() &&
                        replicaInfo.IsPrimary)
                    {
                        serverStatus.IsPrimary = true;
                    }
                }
                PrimaryStatusForInstanceManager.Instance.NoteServerIsOnline(serverStatus.Instance);
            }
            catch
            {
                PrimaryStatusForInstanceManager.Instance.NoteServerIsOffline(serverStatus.Instance);
                //We skip it because it is for to know is server is involved into always on group and is primary
            }
        }

        public void SetLocalDateTimes(AuditedServerStatus serverStatus)
        {
            serverStatus.LastAgentContactTime = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(serverStatus.LastAgentContactTime);
            serverStatus.LastHeartbeat = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(serverStatus.LastHeartbeat);
            serverStatus.LastArchived = DateTimeHelper.GetLocalTimeOfCurrentTimeZone(serverStatus.LastArchived);
        }

        public void SetStatisticCounts(ServerRecord server, AuditedServerStatus serverStatus, int daysToShow, SqlConnection connection)
        {
            serverStatus.CollectedEventCount = StatsHelper.CountTotalSince(server.SrvId, StatsCategory.EventProcessed, daysToShow, connection);
            serverStatus.RecentAlertCount = StatsHelper.CountTotalSince(server.SrvId, StatsCategory.Alerts, daysToShow, connection);
            var configuration = SqlCmConfigurationHelper.GetConfiguration(connection);
            var dbList = AgentHelper.LoadAllDatabases(configuration, server);
            serverStatus.TotalDatabaseCount = dbList == null ? 0 : dbList.Count;
        }

        public void SetServerAuditConfigurationList(ServerRecord server, AuditedServerStatus serverStatus)
        {
            var stringList = new List<string>();

            if (server.AuditLogins)
                stringList.Add("Logins");

            // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
            if (server.AuditLogouts)
                stringList.Add("Logouts");

            if (server.AuditFailedLogins)
                stringList.Add("Failed Logins");

            if (server.AuditSecurity)
                stringList.Add("Security");

            if (server.AuditDDL)
                stringList.Add("DDL");

            if (server.AuditAdmin)
                stringList.Add("Admin");

            if (server.AuditUDE)
                stringList.Add("UDE");

            serverStatus.AuditedServerActivities = stringList;
        }

        public void SetPrivilegedUsersAuditConfigurationList(ServerRecord server, AuditedServerStatus serverStatus)
        {
            var stringList = new List<string>();

            int userCount = 0;

            if (server.AuditUsersList != null && server.AuditUsersList.Length > 0)
            {
                UserList users = new UserList(server.AuditUsersList);
                userCount = users.Logins.Length + users.ServerRoles.Length;

                if (server.AuditUserLogins || server.AuditUserAll)
                    stringList.Add("Logins");

                // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                if (server.AuditUserLogouts || server.AuditUserAll)
                    stringList.Add("Logouts");

                if (server.AuditUserFailedLogins || server.AuditUserAll)
                    stringList.Add("Failed Logins");

                if (server.AuditUserSecurity || server.AuditUserAll)
                    stringList.Add("Security");

                if (server.AuditUserDDL || server.AuditUserAll)
                    stringList.Add("DDL");

                if (server.AuditUserAdmin || server.AuditUserAll)
                    stringList.Add("Admin");

                if (server.AuditUserUDE || server.AuditUserAll)
                    stringList.Add("UDE");

                if (server.AuditUserDML || server.AuditUserAll)
                    stringList.Add("DML");

                if (server.AuditUserSELECT || server.AuditUserAll)
                    stringList.Add("Select");

                if (((server.AuditUserDML || server.AuditUserSELECT) && server.AuditUserCaptureSQL) || server.AuditUserAll)
                    stringList.Add("Capture SQL");

                if (server.AuditUserCaptureDDL || server.AuditUserAll)
                {
                    stringList.Add("Capture DDL");
                }

                if ((server.AuditUserDML && server.AuditUserCaptureTrans) || server.AuditUserAll)
                    stringList.Add("Capture Transactions");
            }

            serverStatus.AuditedPrivilegedUsersActivities = stringList;
            serverStatus.PrivilegedUsersCount = userCount;
        }

        public void SetEventFiltersAuditConfigurationList(ServerRecord server, AuditedServerStatus serverStatus, SqlConnection connection)
        {
            var filters = FiltersDal.SelectEventFiltersForServer(connection, server.Instance);
            var conditions = new Dictionary<int, int>();
            var conditionNames = new List<string>();
            if (filters.Count > 0)
            {
                foreach (var filter in filters)
                {
                    foreach (var condition in filter.Conditions)
                    {
                        if (!conditions.ContainsKey(condition.FieldId) &&
                            condition.FieldId != (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.serverName)
                        {
                            conditions.Add(condition.FieldId, condition.FieldId);
                            conditionNames.Add(condition.TargetEventField.DisplayName);
                        }
                    }
                }
                conditionNames.Sort();
            }

            serverStatus.EventFilters = conditionNames;
        }

        private List<bool> GetFilteringStatusList(string filteringStatusString)
        {
            var statusToFilter = new List<bool>();

            if (!string.IsNullOrEmpty(filteringStatusString))
            {
                var statusStringArray = filteringStatusString.Split(',');
                foreach (var statusString in statusStringArray)
                {
                    int convertedIntStatus;
                    if (Int32.TryParse(statusString, out convertedIntStatus))
                    {
                        bool boolean = Convert.ToBoolean(convertedIntStatus);
                        if (!statusToFilter.Contains(boolean))
                        {
                            statusToFilter.Add(boolean);
                        }
                    }
                }
            }

            return statusToFilter;
        }

        private List<string> GetFilteringStatusTextList(string filteringStatusTextString)
        {
            var statusToFilter = new List<string>();

            if (!string.IsNullOrEmpty(filteringStatusTextString))
            {
                var statusStringArray = filteringStatusTextString.Split(',');
                foreach (var statusString in statusStringArray)
                {
                    if (!statusToFilter.Contains(statusString))
                    {
                        statusToFilter.Add(statusString);
                    }
                }
            }

            return statusToFilter;
        }


        public List<AuditedServerStatus> GetFilteredInstancesStatuses(SqlConnection connection, FilteredRegisteredInstancesStatusRequest request)
        {
            var result = new List<AuditedServerStatus>();
            var statusToBeFiltered = GetFilteringStatusList(request.Status);
            var statusTextToBeFiltered = GetFilteringStatusTextList(request.StatusText);
            string temp = "";
            if (request.StatusText != null)
            {
                temp = statusTextToBeFiltered.First<string>();
                temp = WildCardToRegular(temp.Replace('%', '*'));
            }


            bool isArchieved = false;
            try
            {
                using (connection)
                {
                    var serverVersion = GetSqlServerVersion(connection.ServerVersion);
                    using (var command = new SqlCommand("[SQLcompliance]..[sp_sqlcm_GetFilteredInstancesStatuses]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@InstanceName", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.InstanceName) ?
                            SqlString.Null : request.InstanceName;

                        command.Parameters.Add("@VersionName", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.SqlVersion) ?
                            SqlString.Null : request.SqlVersion;

                        command.Parameters.Add("@IsAudited", SqlDbType.TinyInt).Value = !request.IsAudited.HasValue ? SqlByte.Null : request.IsAudited.Value;
                        command.Parameters.Add("@IsEnabled", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.IsEnabled)
                            ? SqlString.Null : request.IsEnabled;

                        command.Parameters.Add("@LastAgentContactFrom", SqlDbType.DateTime).Value =
                            !request.LastAgentContactFrom.HasValue
                                ? SqlDateTime.Null
                                : request.LastAgentContactFrom.Value;
                        command.Parameters.Add("@LastAgentContactTo", SqlDbType.DateTime).Value = !request.LastAgentContactTo.HasValue ? SqlDateTime.Null : request.LastAgentContactTo.Value;
                        command.Parameters.Add("@AuditedDatabaseCountFrom", SqlDbType.Int).Value = !request.NumberOfAuditedDatabasesFrom.HasValue ? SqlInt32.Null : request.NumberOfAuditedDatabasesFrom.Value;
                        command.Parameters.Add("@AuditedDatabaseCountTo", SqlDbType.Int).Value = !request.NumberOfAuditedDatabasesTo.HasValue ? SqlInt32.Null : request.NumberOfAuditedDatabasesTo.Value;
                        command.Parameters.Add("@InstanceId", SqlDbType.Int).Value = !request.ServerId.HasValue ? SqlInt32.Null : request.ServerId.Value;
                        command.Parameters.Add("@EventsDateFrom", SqlDbType.DateTime).Value = !request.Days.HasValue ? SqlDateTime.Null : DateTime.Now.AddDays(-request.Days.Value);


                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var status = Transformer.Instance.TransformAuditedServerAlertData(reader, serverVersion, connection.ConnectionString);

                                if (status.IsAudited == true)
                                {
                                    bool valid = true;

                                    if (valid &&
                                        statusToBeFiltered.Count > 0 &&
                                        !statusToBeFiltered.Contains(status.ServerStatus))
                                    {
                                        valid = false;
                                    }

                                    if (valid &&
                                        statusTextToBeFiltered.Count > 0 &&
                                        !Regex.IsMatch(status.Message, temp, RegexOptions.IgnoreCase))
                                    {
                                        valid = false;
                                    }
                                    if (valid)
                                        result.Add(status);
                                    isArchieved = false;
                                }
                                else
                                {
                                    if (status.Message == "Archive server")
                                    {
                                        result.Add(status);
                                        isArchieved = true;
                                    }
                                }
                            }
                        }
                    }
                    if (isArchieved == false)
                    {
                        UpdateOtherProperties(result, request, connection);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        private static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }

        public EventDistributionForDatabaseResult GetEventDistributionForDatabase(SqlConnection connection, string query)
        {
            var result = new EventDistributionForDatabaseResult();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            result = Transformer.Instance.TransformEventDistributionForDatabase(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public List<EnvironmentObject> GetEnvironmentObjectHierarchy(SqlConnection connection, string query, int parentId)
        {
            var result = new List<EnvironmentObject>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformEnvironmentObjectData(reader, parentId));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public CmCombinedLicense GetCmLicenses(SqlConnection connection, string query, string licenseScope)
        {
            try
            {
                using (connection)
                {

                    var config = SqlCmConfigurationHelper.GetConfiguration(connection);
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = command.ExecuteReader())
                        {
                            var result = Transformer.Instance.TransformCmLicenseData(reader, licenseScope, config.LicenseObject);
                            result.RepositoryServer = connection.DataSource;
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void AddFilterEventParametersToCommand(SqlCommand command, FilteredEventRequest request)
        {
            command.Parameters.Add("@DatabaseId", SqlDbType.Int).Value = request.DatabaseId.HasValue ? request.DatabaseId.Value : SqlInt32.Null;
            command.Parameters.Add("@DatabaseName", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.DatabaseName) ? SqlString.Null : request.DatabaseName;

            command.Parameters.Add("@Category", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Category) ? SqlString.Null : request.Category;
            command.Parameters.Add("@EventCategoryId", SqlDbType.Int).Value = request.StatCategory.HasValue
                ? (int)(request.StatCategory.Value)
                : SqlInt32.Null;
            command.Parameters.Add("@EventType", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.EventType) ? SqlString.Null : request.EventType;

            command.Parameters.Add("@LoginName", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.LoginName) ? SqlString.Null : request.LoginName;
            command.Parameters.Add("@TargetObject", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.TargetObject) ? SqlString.Null : request.TargetObject;

            command.Parameters.Add("@SpidFrom", SqlDbType.Int).Value = !request.SpidFrom.HasValue ? SqlInt32.Null : request.SpidFrom.Value;
            command.Parameters.Add("@SpidTo", SqlDbType.Int).Value = !request.SpidTo.HasValue ? SqlInt32.Null : request.SpidTo.Value;

            command.Parameters.Add("@Application", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Application) ? SqlString.Null : request.Application;
            command.Parameters.Add("@Host", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Host) ? SqlString.Null : request.Host;
            command.Parameters.Add("@Server", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Server) ? SqlString.Null : request.Server;
            command.Parameters.Add("@AccessCheck", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.AccessCheck) ? SqlString.Null : request.AccessCheck;
            command.Parameters.Add("@DatabaseUser", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.DatabaseUser) ? SqlString.Null : request.DatabaseUser;
            command.Parameters.Add("@Object", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Object) ? SqlString.Null : request.Object;
            command.Parameters.Add("@TargetLogin", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.TargetLogin) ? SqlString.Null : request.TargetLogin;
            command.Parameters.Add("@TargetUser", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.TargetUser) ? SqlString.Null : request.TargetUser;
            command.Parameters.Add("@Role", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Role) ? SqlString.Null : request.Role;
            command.Parameters.Add("@Owner", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Owner) ? SqlString.Null : request.Owner;
            command.Parameters.Add("@PrivilegedUser", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.PrivilegedUser) ? SqlString.Null : request.PrivilegedUser;
            command.Parameters.Add("@SessionLogin", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.SessionLogin) ? SqlString.Null : request.SessionLogin;

            command.Parameters.Add("@AuditedUpdatesFrom", SqlDbType.Int).Value = !request.AuditedUpdatesFrom.HasValue ? SqlInt32.Null : request.AuditedUpdatesFrom.Value;
            command.Parameters.Add("@AuditedUpdatesTo", SqlDbType.Int).Value = !request.AuditedUpdatesTo.HasValue ? SqlInt32.Null : request.AuditedUpdatesTo.Value;

            command.Parameters.Add("@PrimaryKey", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.PrimaryKey) ? SqlString.Null : request.PrimaryKey;
            command.Parameters.Add("@TableId", SqlDbType.Int).Value = !request.TableId.HasValue ? SqlInt32.Null : request.TableId.Value;
            command.Parameters.Add("@Table", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Table) ? SqlString.Null : request.Table;
            command.Parameters.Add("@ColumnId", SqlDbType.Int).Value = !request.ColumnId.HasValue ? SqlInt32.Null : request.ColumnId.Value;
            command.Parameters.Add("@Column", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Column) ? SqlString.Null : request.Column;
            command.Parameters.Add("@BeforeValue", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.BeforeValue) ? SqlString.Null : request.BeforeValue;
            command.Parameters.Add("@AfterValue", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.AfterValue) ? SqlString.Null : request.AfterValue;

            command.Parameters.Add("@ColumnsUpdatedFrom", SqlDbType.Int).Value = !request.ColumnsUpdatedFrom.HasValue ? SqlInt32.Null : request.ColumnsUpdatedFrom.Value;
            command.Parameters.Add("@ColumnsUpdatedTo", SqlDbType.Int).Value = !request.ColumnsUpdatedTo.HasValue ? SqlInt32.Null : request.ColumnsUpdatedTo.Value;

            command.Parameters.Add("@Schema", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Schema) ? SqlString.Null : request.Schema;

            var timeFrom = DateTimeHelper.GetUtcTime(request.TimeFrom);
            command.Parameters.Add("@TimeFrom", SqlDbType.DateTime).Value =
    !timeFrom.HasValue ? SqlDateTime.Null : timeFrom.Value;

            var timeTo = DateTimeHelper.GetUtcTime(request.TimeTo);
            command.Parameters.Add("@TimeTo", SqlDbType.DateTime).Value =
                !timeTo.HasValue ? SqlDateTime.Null : RemoveMillisecounds(timeTo.Value);

            command.Parameters.Add("@DateFrom", SqlDbType.DateTime).Value =
    !request.DateFrom.HasValue ? SqlDateTime.Null : request.DateFrom.Value;

            command.Parameters.Add("@DateTo", SqlDbType.DateTime).Value =
                !request.DateTo.HasValue ? SqlDateTime.Null : RemoveTimePart(request.DateTo.Value);

            command.Parameters.Add("@Details", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Details) ? SqlString.Null : request.Details;
            command.Parameters.Add("@Page", SqlDbType.Int).Value = request.Page.HasValue ? request.Page.Value : SqlInt32.Null;
            command.Parameters.Add("@PageSize", SqlDbType.Int).Value = request.PageSize.HasValue ? request.PageSize.Value : SqlInt32.Null;
            command.Parameters.Add("@SortColumn", SqlDbType.NVarChar).Value = !string.IsNullOrWhiteSpace(request.SortColumn) ? request.SortColumn : SqlString.Null;
            command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = request.SortDirection.HasValue ? request.SortDirection.Value : SqlInt32.Null;
        }

        private List<DetaliedAuditEvent> GetDetaliedAuditEventList(SqlCommand command, DetaliedEventsResponse result)
        {
            var eventList = new List<DetaliedAuditEvent>();

            using (var reader = command.ExecuteReader())
            {
                int recordsCount = 0;
                if (reader.Read())
                    recordsCount = reader.GetInt32(0);

                reader.NextResult();

                while (reader.Read())
                    eventList.Add(Transformer.Instance.TransformDetaliedAuditEvent(reader));

                result.RecordCount = recordsCount;

                reader.NextResult();

                while (reader.Read())
                {
                    result.EventType.Add(Transformer.Instance.TransformEventFilterData(reader));
                }
            }

            return eventList;
        }

        private void ProcessFilterEventsCommandResult(SqlCommand command, string eventDatabase, FilteredEventRequest request, DetaliedEventsResponse result)
        {
            AddFilterEventParametersToCommand(command, request);
            var eventList = GetDetaliedAuditEventList(command, result);
            LoadBeforeAfterDataForEvent(command.Connection, eventDatabase, request, eventList, result);
        }

        internal DetaliedEventsResponse GetAuditedEvents(SqlConnection connection, string query, string eventDatabase, FilteredEventRequest request)
        {
            var result = new DetaliedEventsResponse();

            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = 0; //Fix for SQLCM-4134
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@EventDatabaseName", SqlDbType.NVarChar).Value = eventDatabase;
                        ProcessFilterEventsCommandResult(command, eventDatabase, request, result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        internal DetaliedEventsResponse GetArchivedEvents(SqlConnection connection, string query, string eventDatabase, FilteredEventRequest request)
        {
            var result = new DetaliedEventsResponse();

            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ServerId", SqlDbType.Int).Value = request.ServerId;
                        command.Parameters.Add("@Archive", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Archive) ? SqlString.Null : request.Archive;
                        ProcessFilterEventsCommandResult(command, eventDatabase, request, result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        private static List<Tuple<int, string>> GetLookupItems(string databaseName, string tableName, SqlConnection connection)
        {
            var retVal = new List<Tuple<int, string>>();
            bool isTables = tableName == CoreConstants.RepositoryTablesTable;
            string baseQuery = isTables ? "SELECT TOP (250)schemaName, name,id FROM {0}..{1} ORDER BY name ASC"
                                        : "SELECT TOP (250)name,id FROM {0}..{1} ORDER BY name ASC";

            try
            {
                var query = String.Format(baseQuery,
                                      SQLHelpers.CreateSafeDatabaseName(databaseName), tableName);
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (isTables)
                        {
                            while (reader.Read())
                            {

                                string schema = SQLHelpers.GetString(reader, 0);
                                string table = SQLHelpers.GetString(reader, 1);

                                var id = SQLHelpers.GetInt32(reader, 2);
                                var name = CoreHelpers.GetTableNameKey(schema, table);
                                var item = new Tuple<int, string>(id, name);
                                retVal.Add(item);
                            }
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                var id = SQLHelpers.GetInt32(reader, 1);
                                var name = SQLHelpers.GetString(reader, 0);
                                var item = new Tuple<int, string>(id, name);
                                retVal.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Instance.Write(String.Format("Unable to lookup ids from {0}..{1}", databaseName, tableName), e);
            }
            return retVal;
        }

        private static void AddItemsToDictionary(Dictionary<int, string> itemDictionary, List<Tuple<int, string>> lookupItemList)
        {
            foreach (var tuple in lookupItemList)
            {
                var tableId = tuple.Item1;
                if (!itemDictionary.ContainsKey(tableId))
                {
                    var tableName = tuple.Item2;
                    itemDictionary.Add(tableId, tableName);
                }
            }
        }

        private void LoadBeforeAfterDataForEvent(SqlConnection connection, string eventDatabase, FilteredEventRequest request, IEnumerable<DetaliedAuditEvent> eventList, DetaliedEventsResponse result)
        {
            Dictionary<int, string> tableDictionary = new Dictionary<int, string>();
            tableDictionary.Add(int.MinValue, "All");
            tableDictionary.Add(int.MinValue + 1, "Empty");
            Dictionary<int, string> columnDictionary = new Dictionary<int, string>();
            columnDictionary.Add(int.MinValue, "All");
            columnDictionary.Add(int.MinValue + 1, "Empty");

            var tablesToFilter = GetLookupItems(eventDatabase, CoreConstants.RepositoryTablesTable, connection);
            AddItemsToDictionary(tableDictionary, tablesToFilter);
            result.Tables = tableDictionary.ToList();

            var columnToFilter = GetLookupItems(eventDatabase, CoreConstants.RepositoryColumnsTable, connection);
            AddItemsToDictionary(columnDictionary, columnToFilter);
            result.Columns = columnDictionary.ToList();

            result.Events.AddRange(eventList);
        }

        internal List<AuditEvent> GetDataForRecentAuditEventsWidget(SqlConnection connection, string query)
        {
            var result = new List<AuditEvent>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformAuditEventData(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal int ExecuteNonQuery(SqlConnection connection, string query)
        {
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Need to return all 5 categories in chart
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private IList<RestStatsData> AddCateroriesToResult(List<RestStatsData> result, IEnumerable<RestStatsCategory> categories, IDictionary<RestStatsCategory, string> categoryMappings)
        {
            var resultCategories = result.Select(x => x.Category);
            var _categorires = categories.Except(resultCategories);

            foreach (var category in _categorires)
            {
                string categoryName;
                categoryMappings.TryGetValue(category, out categoryName);

                result.Add(new RestStatsData() { Category = category, CategoryName = categoryName });
            }

            return result;
        }

        private static void GenerateStatDataForEmptyResultSet(IEnumerable<RestStatsCategory> categories, List<RestStatsData> result, IEnumerable<RestStatsCategory> statsCategories,
            IDictionary<RestStatsCategory, string> categoryMappings)
        {

            if (categories != null)
                foreach (var statsCategory in statsCategories)
                    result.Add(new RestStatsData { Category = statsCategory, CategoryName = categoryMappings[statsCategory] });
            else
                foreach (RestStatsCategory categoryValue in Enum.GetValues(typeof(RestStatsCategory)))
                    if (categoryValue != RestStatsCategory.Unknown)
                        result.Add(new RestStatsData { Category = categoryValue, CategoryName = categoryMappings[categoryValue] });

        }

        internal List<RestStatsData> GetDataForActivityReportCardWidget(SqlConnection connection, string query)
        {
            var result = new List<RestStatsData>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformStatsData(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal IEnumerable<EventProperties> GetRecentDatabaseEvents(SqlConnection connection, string query)
        {
            IList<EventProperties> result = new List<EventProperties>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformEventProperties(reader, true));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        internal EventProperties GetEventProperties(SqlConnection connection, string query, string eventDatabase)
        {
            EventProperties result = null;
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = Transformer.Instance.TransformEventProperties(reader, false, true);
                            result.SqlVersion = GetSqlServerVersion(connection.ServerVersion);

                            List<DataChangeRecord> dcRecords = DataChangeRecord.GetDataChangeRecords(connection, eventDatabase, result.EventId);
                            if (dcRecords != null)
                            {
                                result.RowsAffected = dcRecords.Count;

                                foreach (DataChangeRecord dataChangeRecord in dcRecords)
                                {
                                    List<ColumnChangeRecord> ccRecords = ColumnChangeRecord.GetColumnChangeRecords(connection, eventDatabase, dataChangeRecord);
                                    HashSet<string> columnNames = new HashSet<string>();
                                    foreach (ColumnChangeRecord ccRec in ccRecords)
                                        columnNames.Add(ccRec.columnName);

                                    result.ColumnsAffected = string.Join(",", columnNames.ToArray());
                                }
                            }

                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public AuditedDatabase GetAuditedDatabase(SqlConnection connection, string query)
        {
            var result = new AuditedDatabase();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                result = Transformer.Instance.TransformAuditedDatabaseData(reader);

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<string> GetRCCData(SqlConnection connection, string query, string serverName = "")
        {
            List<string> result = new List<string>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (!string.IsNullOrEmpty(serverName))
                        {
                            command.Parameters.Add("@serverName", SqlDbType.NVarChar).Value = serverName;
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!string.IsNullOrEmpty(serverName))
                                {
                                    result.Add(SQLHelpers.GetString(reader, 0));
                                }
                                else
                                {
                                    result.Add(SQLHelpers.GetString(reader, 1));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<int> GetStandardRegulationSettings(SqlConnection connection, int regulationId)
        {
            List<int> result = new List<int>();
            try
            {
                string query = "sp_cmreport_GetStandardRegulationSettings";
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@regulationId", SqlDbType.Int).Value = regulationId;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.Add(SQLHelpers.GetInt32(reader, 0));
                            result.Add(SQLHelpers.GetInt32(reader, 1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            return result;
        }

        public List<T> GetItemList<T>(SqlConnection connection, string query) where T : new()
        {
            var result = new List<T>();

            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                T item = ReadItem<T>(reader);
                                result.Add(item);
                            }
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        internal T ReadItem<T>(IDataReader reader) where T : new()
        {
            T item = new T();

            foreach (PropertyInfo property in item.GetType().GetProperties())
            {
                object value = ReadValueForProperty(reader, property);
                property.SetValue(item, value, null);
            }

            return item;
        }

        private object ReadValueForProperty(IDataReader reader, PropertyInfo property)
        {
            object value = reader[property.Name];

            if (DBNull.Value.Equals(value))
            {
                return property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;
            }

            return Convert.ChangeType(value, property.PropertyType);
        }

        internal List<ServerAlert> GetAlertsData(SqlConnection connection, string query)
        {
            var result = new List<ServerAlert>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformInstancesAlertData(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal FilteredAlertsResponse GetFilteredAlerts(SqlConnection connection, FilteredAlertRequest request)
        {
            var result = new FilteredAlertsResponse();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand("[SQLcompliance]..[sp_sqlcm_SelectFilteredAlerts]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@AlertLevels", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Levels) ? SqlString.Null : request.Levels;
                        command.Parameters.Add("@Instance", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.InstanceName) ? SqlString.Null : request.InstanceName;

                        command.Parameters.Add("@DateFrom", SqlDbType.DateTime).Value =
                            !request.DateFrom.HasValue ? SqlDateTime.Null : request.DateFrom.Value;

                        command.Parameters.Add("@DateTo", SqlDbType.DateTime).Value =
                            !request.DateTo.HasValue ? SqlDateTime.Null : RemoveTimePart(request.DateTo.Value);

                        var timeFrom = DateTimeHelper.GetUtcTime(request.TimeFrom);
                        command.Parameters.Add("@TimeFrom", SqlDbType.DateTime).Value =
                            !timeFrom.HasValue ? SqlDateTime.Null : timeFrom.Value;

                        var timeTo = DateTimeHelper.GetUtcTime(request.TimeTo);
                        command.Parameters.Add("@TimeTo", SqlDbType.DateTime).Value =
                            !timeTo.HasValue ? SqlDateTime.Null : RemoveMillisecounds(timeTo.Value);

                        command.Parameters.Add("@AlertType", SqlDbType.Int).Value = request.AlertType.HasValue ? (int)request.AlertType.Value : SqlInt32.Null;
                        command.Parameters.Add("@SourceRule", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.SourceRule) ? SqlString.Null : request.SourceRule;
                        command.Parameters.Add("@EventTypeName", SqlDbType.NVarChar).Value = !string.IsNullOrWhiteSpace(request.Event) ? request.Event : SqlString.Null;
                        command.Parameters.Add("@Details", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Detail) ? SqlString.Null : request.Detail; ;
                        command.Parameters.Add("@Page", SqlDbType.Int).Value = request.Page.HasValue ? request.Page.Value : SqlInt32.Null;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = request.PageSize.HasValue ? request.PageSize.Value : SqlInt32.Null;
                        command.Parameters.Add("@SortColumn", SqlDbType.NVarChar).Value = !string.IsNullOrWhiteSpace(request.SortColumn) ? request.SortColumn : SqlString.Null;
                        command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = request.SortDirection.HasValue ? request.SortDirection.Value : SqlInt32.Null;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                result.RecordCount = reader.GetInt32(0);

                            reader.NextResult();

                            while (reader.Read())
                            {
                                AlertLevel alertLevel =
                                    (AlertLevel)Transformer.Instance.ReadData<int>(reader, "alertLevel");
                                int alertCount = Transformer.Instance.ReadData<int>(reader, "alertCount");

                                switch (alertLevel)
                                {
                                    case AlertLevel.Low:
                                        result.TotalLowAlerts = alertCount;
                                        break;
                                    case AlertLevel.Medium:
                                        result.TotalMediumAlerts = alertCount;
                                        break;
                                    case AlertLevel.High:
                                        result.TotalHighAlerts = alertCount;
                                        break;
                                    case AlertLevel.Severe:
                                        result.TotalSevereAlerts = alertCount;
                                        break;
                                }
                            }

                            reader.NextResult();

                            if (reader.Read())
                            {
                                result.CountOfInstancesWithLowAlerts = Transformer.Instance.ReadData<int>(reader, "low");
                                result.CountOfInstancesWithMediumAlerts = Transformer.Instance.ReadData<int>(reader,
                                    "med");
                                result.CountOfInstancesWithHighAlerts = Transformer.Instance.ReadData<int>(reader,
                                    "high");
                                result.CountOfInstancesWithSevereAlerts = Transformer.Instance.ReadData<int>(reader,
                                    "severe");
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformFilteredAlertData(reader));
                            }

                            reader.NextResult();
                            while (reader.Read())
                            {
                                result.EventType.Add(Transformer.Instance.TransformEventFilterData(reader));
                            }

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<ArchiveRecord> GetArchiveList(SqlConnection connection, string query, string instance)
        {
            var result = new List<ArchiveRecord>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add("@instance", SqlDbType.NVarChar).Value = instance;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformArchiveData(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ArchiveProperties GetArchiveProperties(SqlConnection connection, string query, string archive)
        {
            ArchiveProperties result = new ArchiveProperties();
            result.DisplayName = "Invalid Archive Database";
            result.Description =
                "The selected database is not a SQL Compliance Manager archive database. Only SQL Compliance Manager archive databases may be attached to the Repository.";

            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                result = Transformer.Instance.TransformArchiveProperties(reader, archive);

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Cant get archived properties for archive {0} due to the following error: ", archive);
                ErrorLog.Instance.Write(errorMessage, ex, true);
            }

            return result;
        }

        public void DetachArchive(SqlConnection connection, string query, DetachArchiveRequest request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Instance", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Instance) ? SqlString.Null : request.Instance;
                    command.Parameters.Add("@DatabaseName", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.DatabaseName) ? SqlString.Null : request.DatabaseName;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SetViewSettings(SqlConnection connection, string query, string connectionUser, ViewSettings settings)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(connectionUser) ? SqlString.Null : connectionUser;
                    command.Parameters.Add("@ViewId", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(settings.ViewId) ? SqlString.Null : settings.ViewId;
                    command.Parameters.Add("@Timeout", SqlDbType.Int).Value = !settings.Timeout.HasValue ? SqlInt32.Null : settings.Timeout.Value;
                    command.Parameters.Add("@Filter", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(settings.Filter) ? SqlString.Null : settings.Filter;
                    command.Parameters.Add("@ViewName", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(settings.ViewName) ? SqlString.Null : settings.ViewName;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ViewSettings GetViewSettings(SqlConnection connection, string query)
        {
            ViewSettings result = new ViewSettings();

            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                result = Transformer.Instance.TransformViewSettings(reader);

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public void AttachArchive(SqlConnection connection, string query, string archive)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Archive", SqlDbType.NVarChar).Value = archive;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public AgentConnectionDetails GetAgentConnectionDetails(string query, SqlConnection connection)
        {
            AgentConnectionDetails result = null;

            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                result = Transformer.Instance.TransformAgentConnectionDetails(reader);

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<ServerAlert> GetAlertsByDatabases(SqlConnection connection, string query, string days)
        {
            var result = new List<ServerAlert>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        int tempDays;
                        command.Parameters.Add("@EventDateFrom", SqlDbType.DateTime).Value = int.TryParse(days, out tempDays) ? DateTime.Now.AddDays(-tempDays) : SqlDateTime.Null;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformDatabaseAlerts(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public UserSettingsModel GetUserSettings(SqlConnection connection, string query, int dashboardUserId)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@DashboardUserId", SqlDbType.Int).Value = dashboardUserId;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return Transformer.Instance.TransformUserSettings(reader);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<UserSettingsModel> GetUserSettings(SqlConnection connection, string query)
        {
            var result = new List<UserSettingsModel>();
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            result.Add(Transformer.Instance.TransformUserSettings(reader));

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void CreateUpdateUserSettings(SqlConnection connection, string query, UserSettingsModel userSettings)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Account", SqlDbType.NVarChar, 255).Value = userSettings.Account;
                    command.Parameters.Add("@DashboardUserId", SqlDbType.Int).Value = userSettings.DashboardUserId;
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 230).Value = userSettings.Email;
                    command.Parameters.Add("@SessionTimout", SqlDbType.Int).Value = (object)userSettings.SessionTimout ?? DBNull.Value;
                    command.Parameters.Add("@Subscribed", SqlDbType.Bit).Value = userSettings.Subscribed;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void DeleteUserSettings(SqlConnection connection, string query, DeleteUserSettingsRequest dashboardUserIds)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@DashboardUserIds", SqlDbType.NVarChar).Value = string.Join(",", dashboardUserIds.DashbloardUserIds);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<AlertsGroup> GetAlertsGroups(SqlConnection connection, string query, int instanceId)
        {
            var result = new List<AlertsGroup>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (instanceId != 0)
                            command.Parameters.Add("@InstanceId", SqlDbType.Int).Value = instanceId;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformAlertsGroup(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void DismissAlertsGroupForInstance(SqlConnection connection, DismissAlertsGroupRequest request)
        {
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand("[SQLcompliance]..[sp_sqlcm_DismissAlertsGroupForInstance]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@InstanceId", SqlDbType.Int).Value = request.InstanceId;
                        command.Parameters.Add("@AlertType", SqlDbType.Int).Value = Convert.ToInt32(request.AlertType);
                        command.Parameters.Add("@AlertLevel", SqlDbType.Int).Value = Convert.ToInt32(request.AlertLevel);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<ServerAlert> GetAlerts(SqlConnection connection, string query, int instanceId, AlertType alertType, AlertLevel alertLevel, int pageSize, int page)
        {
            var result = new List<ServerAlert>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (instanceId != 0)
                            command.Parameters.Add("@InstanceId", SqlDbType.Int).Value = instanceId;

                        command.Parameters.Add("@AlertType", SqlDbType.Int).Value = alertType;
                        command.Parameters.Add("@AlertLevel", SqlDbType.Int).Value = alertLevel;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
                        command.Parameters.Add("@Page", SqlDbType.Int).Value = page;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformFilteredAlertData(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ManagedInstanceResponce GetManagedInstances(SqlConnection connection, string query, PaginationRequest request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (request.Page.HasValue)
                    {
                        command.Parameters.Add("@Page", SqlDbType.Int).Value = request.Page.Value;
                    }

                    if (request.PageSize.HasValue)
                    {
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = request.PageSize.Value;
                    }

                    if (!string.IsNullOrWhiteSpace(request.SortColumn))
                    {
                        command.Parameters.Add("@SortColumn", SqlDbType.NVarChar).Value = request.SortColumn;
                    }

                    if (request.SortDirection.HasValue)
                    {
                        command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = request.SortDirection.Value;
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        return Transformer.Instance.TransformManagedInstanceResponce(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ManagedInstanceProperties GetManagedInstance(SqlConnection connection, string query, int id)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Transformer.Instance.TransformInstanceInfo(reader);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ManagedInstanceForEditResponce GetOwnersAndLocations(SqlConnection connection, string query)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        return Transformer.Instance.TransformOwnersAndLocations(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateManagedInstance(SqlConnection connection, string query, ManagedInstanceProperties request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@AccountType", SqlDbType.Int).Value = request.Credentials.AccountType;
                    command.Parameters.Add("@CollectionInterval", SqlDbType.Int).Value = request.DataCollectionSettings.CollectionInterval;
                    command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = GetValidSqlValue(request.Comments);
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = request.Id;
                    command.Parameters.Add("@KeepDataFor", SqlDbType.Int).Value = request.DataCollectionSettings.KeepDataFor;
                    command.Parameters.Add("@Location", SqlDbType.NVarChar).Value = GetValidSqlValue(request.Location);
                    command.Parameters.Add("@Owner", SqlDbType.NVarChar).Value = GetValidSqlValue(request.Owner);

                    var encriptedPassword = EncryptionHelper.QuickEncrypt(request.Credentials.Password);
                    command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = GetValidSqlValue(encriptedPassword);
                    command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = GetValidSqlValue(request.Credentials.Account);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<ManagedServerInstance> GetManagedServerInstances(SqlConnection connection, string query, IEnumerable<int> instancesIds)
        {
            try
            {
                var result = new List<ManagedServerInstance>();

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@InstanceIds", SqlDbType.NVarChar).Value = string.Join(",", instancesIds);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            result.Add(Transformer.Instance.TransformManagedServerInstance(reader));

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateManagedInstancesCredentials(SqlConnection connection, string query, BatchInstancesCredentialsRequest request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@InstanceIds", SqlDbType.NVarChar).Value = string.Join(",", request.InstancesIds);

                    var encriptedPassword = EncryptionHelper.QuickEncrypt(request.Credentials.Password);
                    command.Parameters.Add("@AccountType", SqlDbType.Int).Value = request.Credentials.AccountType;
                    command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = GetValidSqlValue(encriptedPassword);
                    command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = GetValidSqlValue(request.Credentials.Account);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<InstanceForSynchronization> GetInstancesForSynchronization(SqlConnection connection, string query)
        {
            var result = new List<InstanceForSynchronization>();
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            result.Add(Transformer.Instance.TransformInstanceForSynchronization(reader));

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void MarkInstancesAsSynchronized(SqlConnection connection, string query, IEnumerable<int> ids)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@InstancesToMarkIds", SqlDbType.NVarChar).Value = string.Join(",", ids);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IEnumerable<ActivityLogsInfo> GetActivityLogs(SqlConnection connection, string query, int instanceId, ActivityLogsType alertType, ActivityLogsLevel alertLevel, int pageSize, int page)
        {
            var result = new List<ActivityLogsInfo>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (instanceId != 0)
                            command.Parameters.Add("@InstanceId", SqlDbType.Int).Value = instanceId;

                        command.Parameters.Add("@EventType", SqlDbType.Int).Value = alertType;
                        command.Parameters.Add("@AlertLevel", SqlDbType.Int).Value = alertLevel;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
                        command.Parameters.Add("@Page", SqlDbType.Int).Value = page;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformActivityLogsInfo(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal FilteredActivityLogsViewResponce GetFilteredActivityLogs(SqlConnection connection, string query, FilteredActivityLogsViewRequest request)
        {
            var result = new FilteredActivityLogsViewResponce();
            try
            {
                Console.Out.WriteLine("inside Responce");
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Instance", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.InstanceName) ? SqlString.Null : request.InstanceName;
                        command.Parameters.Add("@Event", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Event) ? SqlString.Null : request.Event;
                        command.Parameters.Add("@DateFrom", SqlDbType.DateTime).Value = !request.DateFrom.HasValue ? SqlDateTime.Null : request.DateFrom.Value;
                        command.Parameters.Add("@DateTo", SqlDbType.DateTime).Value = !request.DateTo.HasValue ? SqlDateTime.Null : RemoveTimePart(request.DateTo.Value);
                        var timeFrom = DateTimeHelper.GetUtcTime(request.TimeFrom);
                        command.Parameters.Add("@TimeFrom", SqlDbType.DateTime).Value = !timeFrom.HasValue ? SqlDateTime.Null : request.TimeFrom.Value;
                        var timeTo = DateTimeHelper.GetUtcTime(request.TimeTo);
                        command.Parameters.Add("@TimeTo", SqlDbType.DateTime).Value = !timeTo.HasValue ? SqlDateTime.Null : RemoveMillisecounds(request.TimeTo.Value);
                        command.Parameters.Add("@EventType", SqlDbType.Int).Value = request.EventType.HasValue ? (int)request.EventType.Value : SqlInt32.Null;
                        command.Parameters.Add("@Details", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Detail) ? SqlString.Null : request.Detail; ;
                        command.Parameters.Add("@Page", SqlDbType.Int).Value = request.Page.HasValue ? request.Page.Value : SqlInt32.Null;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = request.PageSize.HasValue ? request.PageSize.Value : SqlInt32.Null;
                        command.Parameters.Add("@SortColumn", SqlDbType.NVarChar).Value = !string.IsNullOrWhiteSpace(request.SortColumn) ? request.SortColumn : SqlString.Null;
                        command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = request.SortDirection.HasValue ? request.SortDirection.Value : SqlInt32.Null;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                result.RecordCount = reader.GetInt32(0);

                            reader.NextResult();
                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformFilteredActivityLogsData(reader));
                            }

                            reader.NextResult();
                            while (reader.Read())
                            {
                                result.EventType.Add(Transformer.Instance.TransformEventFilterData(reader));
                            }
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal List<ServerActivityLogs> GetActivityLogsData(SqlConnection connection, string query)
        {
            var result = new List<ServerActivityLogs>();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformInstancesActivityLogsData(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal FilteredChangeLogsViewResponce GetFilteredChangeLogs(SqlConnection connection, string query, FilteredChangeLogsViewRequest request)
        {
            var result = new FilteredChangeLogsViewResponce();
            try
            {
                Console.Out.WriteLine("inside Responce");
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Instance", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.InstanceName) ? SqlString.Null : request.InstanceName;
                        command.Parameters.Add("@Event", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Event) ? SqlString.Null : request.Event;
                        command.Parameters.Add("@DateFrom", SqlDbType.DateTime).Value = !request.DateFrom.HasValue ? SqlDateTime.Null : request.DateFrom.Value;
                        command.Parameters.Add("@DateTo", SqlDbType.DateTime).Value = !request.DateTo.HasValue ? SqlDateTime.Null : RemoveTimePart(request.DateTo.Value);
                        var timeFrom = DateTimeHelper.GetUtcTime(request.TimeFrom);
                        command.Parameters.Add("@TimeFrom", SqlDbType.DateTime).Value = !timeFrom.HasValue ? SqlDateTime.Null : request.TimeFrom.Value;
                        var timeTo = DateTimeHelper.GetUtcTime(request.TimeTo);
                        command.Parameters.Add("@TimeTo", SqlDbType.DateTime).Value = !timeTo.HasValue ? SqlDateTime.Null : RemoveMillisecounds(request.TimeTo.Value);
                        command.Parameters.Add("@LogType", SqlDbType.Int).Value = request.EventType.HasValue ? (int)request.EventType.Value : SqlInt32.Null;
                        command.Parameters.Add("@User", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.User) ? SqlString.Null : request.User;
                        command.Parameters.Add("@Detail", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Detail) ? SqlString.Null : request.Detail;
                        command.Parameters.Add("@Page", SqlDbType.Int).Value = request.Page.HasValue ? request.Page.Value : SqlInt32.Null;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = request.PageSize.HasValue ? request.PageSize.Value : SqlInt32.Null;
                        command.Parameters.Add("@SortColumn", SqlDbType.NVarChar).Value = !string.IsNullOrWhiteSpace(request.SortColumn) ? request.SortColumn : SqlString.Null;
                        command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = request.SortDirection.HasValue ? request.SortDirection.Value : SqlInt32.Null;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                result.RecordCount = reader.GetInt32(0);

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformFilteredChangeLogsData(reader));
                            }

                            reader.NextResult();
                            while (reader.Read())
                            {
                                result.EventType.Add(Transformer.Instance.TransformEventFilterData(reader));
                            }


                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal AlertRulesResponse GetAlertRules(SqlConnection connection, string query, AlertRulesRequest request)
        {
            var result = new AlertRulesResponse();
            try
            {
                Console.Out.WriteLine("inside Responce");
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@AlertLevels", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Levels) ? SqlString.Null : request.Levels;
                        command.Parameters.Add("@TargetInstance", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.InstanceName) ? SqlString.Null : request.InstanceName;
                        command.Parameters.Add("@AlertType", SqlDbType.Int).Value = request.AlertType.HasValue ? (int)request.AlertType.Value : SqlInt32.Null;
                        command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Name) ? SqlString.Null : request.Name;
                        command.Parameters.Add("@LogMessage", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.LogMessage) ? SqlString.Null : request.LogMessage;
                        command.Parameters.Add("@EmailMessage", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.EmailMessage) ? SqlString.Null : request.EmailMessage;
                        command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Email) ? SqlString.Null : request.Email;
                        command.Parameters.Add("@SnmpTrap", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.SnmpTrap) ? SqlString.Null : request.SnmpTrap;
                        command.Parameters.Add("@EventLog", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.EventLog) ? SqlString.Null : request.EventLog;
                        command.Parameters.Add("@Rule", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Rule) ? SqlString.Null : request.Rule;
                        command.Parameters.Add("@RuleType", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.RuleType) ? SqlString.Null : request.RuleType;
                        command.Parameters.Add("@Server", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Server) ? SqlString.Null : request.Server;
                        command.Parameters.Add("@Page", SqlDbType.Int).Value = request.Page.HasValue ? request.Page.Value : SqlInt32.Null;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = request.PageSize.HasValue ? request.PageSize.Value : SqlInt32.Null;
                        command.Parameters.Add("@SortColumn", SqlDbType.NVarChar).Value = !string.IsNullOrWhiteSpace(request.SortColumn) ? request.SortColumn : SqlString.Null;
                        command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = request.SortDirection.HasValue ? request.SortDirection.Value : SqlInt32.Null;
                        using (var reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                                result.RecordCount = reader.GetInt32(0);
                            reader.NextResult();

                            while (reader.Read())
                            {
                                AlertType alertType =
                                    (AlertType)Transformer.Instance.ReadData<int>(reader, "alertType");
                                int alertCount = Transformer.Instance.ReadData<int>(reader, "alertCount");

                                switch (alertType)
                                {
                                    case AlertType.Event:
                                        result.TotalEventAlertRules = alertCount;
                                        break;
                                    case AlertType.Data:
                                        result.TotalDataAlertRules = alertCount;
                                        break;
                                    case AlertType.Status:
                                        result.TotalStatusAlertRules = alertCount;
                                        break;
                                }
                            }

                            reader.NextResult();

                            if (reader.Read())
                            {
                                result.CountOfInstancesWithEventAlerts = Transformer.Instance.ReadData<int>(reader, "event");
                                result.CountOfInstancesWithDataAlerts = Transformer.Instance.ReadData<int>(reader, "data");
                                result.CountOfInstancesWithStatusAlerts = Transformer.Instance.ReadData<int>(reader, "status");
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformAlertRules(reader));
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.EventType.Add(Transformer.Instance.TransformEventFilterData(reader));
                            }

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void GetEnableAlertRules(SqlConnection connection, string query, EnableAlertRulesRequest request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Enable", SqlDbType.Bit).Value = request.IsEnabled;
                    command.Parameters.Add("@RuleId", SqlDbType.Int).Value = request.RuleId;

                    command.ExecuteNonQuery();
                }
                int serverPort = getServerPort(connection);
                string server = getSever(connection);
                ServerManager srvManager = RemoteObjectProviderBase.GetObject<ServerManager>(server, serverPort);
                srvManager.UpdateAlertRules();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void GetDeleteAlertRules(SqlConnection connection, string query, EnableAlertRulesRequest request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@RuleId", SqlDbType.Int).Value = request.RuleId;
                    command.ExecuteNonQuery();
                }
                int serverPort = getServerPort(connection);
                string server = getSever(connection);
                ServerManager srvManager = RemoteObjectProviderBase.GetObject<ServerManager>(server, serverPort);
                srvManager.UpdateAlertRules();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void GetUpdateSnmpConfiguration(SqlConnection connection, string query, UpdateSnmpConfigurationRequest request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SnmpServerAddress", SqlDbType.NVarChar).Value = request.SnmpServerAddress;
                    command.Parameters.Add("@SnmpPort", SqlDbType.Int).Value = request.Port;
                    command.Parameters.Add("@Community", SqlDbType.NVarChar).Value = request.Community;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateSNMPThresholdConfiguration(SqlConnection connection, string query, UpdateSNMPThresholdConfiguration request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@InstanceName", SqlDbType.NVarChar).Value = request.InstanceName;
                    command.Parameters.Add("@SenderEmail", SqlDbType.NVarChar).Value = request.SenderEmail;
                    command.Parameters.Add("@SendMailPermission", SqlDbType.TinyInt).Value = request.SendMailPermission;
                    command.Parameters.Add("@SnmpPermission", SqlDbType.TinyInt).Value = request.SnmpPermission;
                    command.Parameters.Add("@LogsPermission", SqlDbType.TinyInt).Value = request.LogsPermission;
                    command.Parameters.Add("@SnmpServerAddress", SqlDbType.NVarChar).Value = request.SnmpServerAddress;
                    command.Parameters.Add("@Port", SqlDbType.Int).Value = request.Port;
                    command.Parameters.Add("@Community", SqlDbType.NVarChar).Value = request.Community;
                    command.Parameters.Add("@Severity", SqlDbType.TinyInt).Value = request.Severity;
                    command.Parameters.Add("@SrvId", SqlDbType.Int).Value = request.SrvId;
                    command.Parameters.Add("@MessageData", SqlDbType.NVarChar).Value = request.MessageData;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void DeleteThresholdConfiguration(SqlConnection connection, string query, string request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@InstanceName", SqlDbType.NVarChar).Value = request;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //SQLCM_125_ _ Rohit_ Start
        public GetSNMPConfigResponse GetSNMPThresholdConfiguration(SqlConnection connection, string query, GetSNMPThresholdConfiguration request)
        {
            var result = new GetSNMPConfigResponse();
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@InstanceName", SqlDbType.NVarChar).Value = request.InstanceName;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(Transformer.Instance.TransformSNMPConfigData(reader));
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //SQLCM_125_ _ Rohit_ End

        public void GetUpdateSmtpConfiguration(SqlConnection connection, string query, UpdateSmtpConfigurationRequest request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@SmtpServer", SqlDbType.NVarChar).Value = request.SmtpServer;
                    command.Parameters.Add("@SmtpPort", SqlDbType.Int).Value = request.SmtpPort;
                    command.Parameters.Add("@SmtpAuthType", SqlDbType.Int).Value = request.SmtpAuthType;
                    command.Parameters.Add("@SmtpSsl", SqlDbType.Int).Value = request.SmtpSsl;
                    command.Parameters.Add("@SmtpUsername", SqlDbType.NVarChar).Value = request.SmtpUsername;
                    command.Parameters.Add("@SmtpPassword", SqlDbType.NVarChar).Value = request.SmtpPassword;
                    command.Parameters.Add("@SmtpSenderAddress", SqlDbType.NVarChar).Value = request.SmtpSenderAddress;
                    command.Parameters.Add("@SmtpSenderName", SqlDbType.NVarChar).Value = request.SmtpSenderName;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void GetUpdateWindowsLogEntry(SqlConnection connection, string query, UpdateWindowsLogEntryRequest request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@LogType", SqlDbType.Int).Value = request.LogType;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void InsertStatusAlertRules(SqlConnection connection, string query, InsertStatusAlertRulesRequest request)
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            string   user = id.Name;
            int serverPort = getServerPort(connection);
            string server = getSever(connection);
            request.DataQuery = String.Format(request.DataQuery, CoreConstants.RepositoryChangeLogEventTable,
                                            SQLHelpers.CreateSafeString(user));
            try
            {                
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@StrQuery", SqlDbType.NVarChar).Value = request.DataQuery;
                    command.ExecuteNonQuery();
                }

                ServerManager srvManager = RemoteObjectProviderBase.GetObject<ServerManager>(server, serverPort);
                srvManager.UpdateAlertRules();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal AuditEventFilterResponse GetAuditEventFilter(SqlConnection connection, string query, AuditEventFilterRequest request)
        {
            var result = new AuditEventFilterResponse();
            try
            {
                Console.Out.WriteLine("inside Response");
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@isEnabled", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.IsEnabled) ? SqlString.Null : request.IsEnabled;
                        command.Parameters.Add("@Instance", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Instance) ? SqlString.Null : request.Instance;
                        command.Parameters.Add("@Filter", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Filter) ? SqlString.Null : request.Filter;
                        command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.DescriptionFilter) ? SqlString.Null : request.DescriptionFilter;

                        command.Parameters.Add("@TargetInstance", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.TargetInstances) ? SqlString.Null : request.TargetInstances;
                        command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Name) ? SqlString.Null : request.Name;
                        command.Parameters.Add("@EventType", SqlDbType.Int).Value = request.EventType;
                        command.Parameters.Add("@Page", SqlDbType.Int).Value = request.Page.HasValue ? request.Page.Value : SqlInt32.Null;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = request.PageSize.HasValue ? request.PageSize.Value : SqlInt32.Null;
                        command.Parameters.Add("@SortColumn", SqlDbType.NVarChar).Value = !string.IsNullOrWhiteSpace(request.SortColumn) ? request.SortColumn : SqlString.Null;
                        command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = request.SortDirection.HasValue ? request.SortDirection.Value : SqlInt32.Null;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformAuditEventFilterData(reader));
                            }

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void GetEnableAuditEventFilter(SqlConnection connection, string query, EnableAuditEventFilter request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@Enable", SqlDbType.Bit).Value = request.IsEnabled;
                    command.Parameters.Add("@EventId", SqlDbType.Int).Value = request.EventId;
                    command.ExecuteNonQuery();
                }

                int serverPort = getServerPort(connection);
                string server = getSever(connection);
                ServerManager srvManager = RemoteObjectProviderBase.GetObject<ServerManager>(server, serverPort);
                srvManager.UpdateEventFilters();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void GetAuditEventFilterDelete(SqlConnection connection, string query, EnableAuditEventFilter request)
        {
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@filterid", SqlDbType.Int).Value = request.EventId;
                    command.ExecuteNonQuery();
                }

                int serverPort = getServerPort(connection);
                string server = getSever(connection);
                ServerManager srvManager = RemoteObjectProviderBase.GetObject<ServerManager>(server, serverPort);
                srvManager.UpdateEventFilters();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public AuditEventExportResponse GetDataByFilterId(SqlConnection connection, string query, AuditEventExportRequest request)
        {
            var result = new AuditEventExportResponse();
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@filterId", SqlDbType.Int).Value = request.FilterId;

                    command.ExecuteNonQuery();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal AuditEventExportResponse GetAuditEventExportList(SqlConnection connection, string query, AuditEventExportRequest request)
        {
            var result = new AuditEventExportResponse();
            try
            {
                Console.Out.WriteLine("inside Responce");
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@filterId", SqlDbType.Int).Value = request.FilterId;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformAuditEventExport(reader));
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformAuditEventExportCondition(reader));
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformAuditEventExportEventType(reader));
                            }

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void InsertStatusEventFilter(SqlConnection connection, string query, InsertStatusEventFilterRequest request)
        {

            WindowsIdentity id = WindowsIdentity.GetCurrent();
            string user = id.Name;
            int serverPort = getServerPort(connection);
            string server = getSever(connection);
            request.DataQuery = String.Format(request.DataQuery, CoreConstants.RepositoryChangeLogEventTable,
                                            SQLHelpers.CreateSafeString(user));
            try
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@StrQuery", SqlDbType.NVarChar).Value = request.DataQuery;
                    command.ExecuteNonQuery();
                }

                ServerManager srvManager = RemoteObjectProviderBase.GetObject<ServerManager>(server, serverPort);
                srvManager.UpdateEventFilters();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal AlertRulesExportResponse GetAlertRulesExportList(SqlConnection connection, string query, AlertRulesExportRequest request)
        {
            var result = new AlertRulesExportResponse();
            try
            {
                Console.Out.WriteLine("inside Responce");
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@ruleId", SqlDbType.Int).Value = request.RuleId;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformAlertRulesExport(reader));
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformAlertRulesExportCondition(reader));
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformAlertRulesExportCategory(reader));
                            }

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal string ExportAlertRules(SqlConnection connection, String request)
        {
            string result = "failed";
            try
            {
                setEventFieldValue();
                int count = 1;
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fileNameOnly = "AlertRules";
                string extension = ".xml";
                String exportPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\AlertRules.xml";
                while (File.Exists(exportPath))
                {
                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                    exportPath = Path.Combine(path, tempFileName + extension);
                }
                //Dump the data
                AlertRuleTemplate tmpl = new AlertRuleTemplate();
                //Given line commented for Web File export. In current scenario no use of repositry server to export.
                //tmpl.RepositoryServer = "WIN-2FHEDOOVQT6";                    
                tmpl.ImportCWF(connection.ConnectionString, request);
                tmpl.Save(exportPath);
                result = exportPath;
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        internal string ImportAlertRules(SqlConnection connection, String request)
        {
            string result = "failed";// = new AlertRulesExportResponse();
            try
            {
                AlertRuleTemplate ruleTemplate = new AlertRuleTemplate();
                if (ruleTemplate.LoadCWF(request) == true)
                {
                    foreach (Idera.SQLcompliance.Core.Rules.Alerts.AlertRule rule in ruleTemplate.AlertRules)
                    {
                        rule.Enabled = false;
                        Idera.SQLcompliance.Core.Rules.Alerts.AlertingDal.InsertAlertRule(rule, connection);
                    }

                    foreach (Idera.SQLcompliance.Core.Rules.Alerts.StatusAlertRule statusRule in ruleTemplate.StatusAlertRules)
                    {
                        statusRule.Enabled = false;
                        Idera.SQLcompliance.Core.Rules.Alerts.AlertingDal.InsertAlertRule(statusRule, connection);
                    }

                    foreach (Idera.SQLcompliance.Core.Rules.Alerts.DataAlertRule dataRule in ruleTemplate.DataAlertRules)
                    {
                        dataRule.Enabled = false;
                        Idera.SQLcompliance.Core.Rules.Alerts.AlertingDal.InsertAlertRule(dataRule, connection);
                    }
                    result = "success";
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        public void setEventFieldValue()
        {
            Hashtable _sqlServerFields = new Hashtable();
            EventField field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.eventType;
            field.DisplayName = "Type";
            field.ColumnName = "eventType";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.Integer;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.eventCategory;
            field.DisplayName = "Category";
            field.ColumnName = "eventCategory";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.Integer;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.applicationName;
            field.DisplayName = "Application Name";
            field.ColumnName = "applicationName";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.String;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.privilegedUsers;
            field.DisplayName = "Privileged Users";
            field.ColumnName = "privilegedUsers";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.String;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.rowCount;
            field.DisplayName = "Row Count";
            field.ColumnName = "rowcount";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.String;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.loginName;
            field.DisplayName = "Login Name";
            field.ColumnName = "loginName";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.String;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.success;
            field.DisplayName = "Access Check Passed";
            field.ColumnName = "success";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.Bool;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.databaseName;
            field.DisplayName = "Database";
            field.ColumnName = "databaseName";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.String;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.objectName;
            field.DisplayName = "Object Name";
            field.ColumnName = "objectName";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.String;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.hostName;
            field.DisplayName = "Hostname";
            field.ColumnName = "hostName";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.String;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.objectType;
            field.DisplayName = "Object Type";
            field.ColumnName = "objectType";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.Integer;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.privilegedUser;
            field.DisplayName = "Privileged User";
            field.ColumnName = "privilegedUser";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.Bool;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.serverName;
            field.DisplayName = "SQL Server";
            field.ColumnName = "serverName";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.String;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.dataRuleApplicationName;
            field.DisplayName = "Application Name";
            field.ColumnName = "applicationName";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.String;
            _sqlServerFields[field.Id] = field;

            field = new EventField();
            field.Id = (int)Idera.SQLcompliance.Core.Rules.Alerts.AlertableEventFields.dataRuleLoginName;
            field.DisplayName = "Login Name";
            field.ColumnName = "loginName";
            field.RuleType = EventType.SqlServer;
            field.DataFormat = MatchType.String;
            _sqlServerFields[field.Id] = field;

            Idera.SQLcompliance.Core.Rules.Alerts.AlertingDal.SqlServerAlertableFields = _sqlServerFields;
        }

        public ApplicationActivityResponse GetApplicationActivity(SqlConnection connection, string query, ApplicationActivityRequest request)
        {
            var result = new ApplicationActivityResponse();
            try
            {
                using (connection)
                {
                    var serverVersion = GetSqlServerVersion(connection.ServerVersion);
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@eventDatabase", SqlDbType.NVarChar).Value = request.Instance.Replace(@"\", @"_");
                        command.Parameters.Add("@databaseName", SqlDbType.NVarChar).Value = request.Database;
                        command.Parameters.Add("@applicationName", SqlDbType.NVarChar).Value = request.Application;
                        command.Parameters.Add("@eventCategory", SqlDbType.Int).Value = request.Category;
                        command.Parameters.Add("@startDate", SqlDbType.DateTime).Value = request.From;
                        command.Parameters.Add("@endDate", SqlDbType.DateTime).Value = request.To;
                        command.Parameters.Add("@privilegedUserOnly", SqlDbType.Bit).Value = request.User;
                        command.Parameters.Add("@showSqlText", SqlDbType.Bit).Value = request.Sql;
                        command.Parameters.Add("@sortColumn", SqlDbType.NVarChar).Value = request.SortColumn;
                        command.Parameters.Add("@rowCount", SqlDbType.Int).Value = request.RowCount;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformApplicationActivity(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DMLActivityResponse GetDMLActivityData(SqlConnection connection, string query, DMLActivityRequest request)
        {
            var result = new DMLActivityResponse();
            try
            {
                using (connection)
                {
                    var serverVersion = GetSqlServerVersion(connection.ServerVersion);
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@eventDatabase", SqlDbType.NVarChar).Value = request.Instance.Replace(@"\", @"_");
                        command.Parameters.Add("@databaseName", SqlDbType.NVarChar).Value = request.Database;
                        command.Parameters.Add("@loginName", SqlDbType.NVarChar).Value = request.LoginName;
                        command.Parameters.Add("@objectName", SqlDbType.NVarChar).Value = request.ObjectName;
                        command.Parameters.Add("@schemaName", SqlDbType.NVarChar).Value = request.SchemaName;
                        command.Parameters.Add("@startDate", SqlDbType.DateTime).Value = request.From;
                        command.Parameters.Add("@endDate", SqlDbType.DateTime).Value = request.To;
                        command.Parameters.Add("@sortColumn", SqlDbType.NVarChar).Value = request.SortColumn;
                        command.Parameters.Add("@rowCount", SqlDbType.Int).Value = request.RowCount;
                        command.Parameters.Add("@primaryKey", SqlDbType.NVarChar).Value = request.Key;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformDMLActivity(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public LoginCreationHistoryResponse GetLoginCreationHistoryData(SqlConnection connection, string query, LoginCreationHistoryRequest request)
        {
            var result = new LoginCreationHistoryResponse();
            try
            {
                using (connection)
                {
                    var serverVersion = GetSqlServerVersion(connection.ServerVersion);
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@eventDatabase", SqlDbType.NVarChar).Value = request.Instance.Replace(@"\", @"_");
                        command.Parameters.Add("@loginName", SqlDbType.NVarChar).Value = request.Login;
                        command.Parameters.Add("@startDate", SqlDbType.DateTime).Value = request.From;
                        command.Parameters.Add("@endDate", SqlDbType.DateTime).Value = request.To;
                        command.Parameters.Add("@sortColumn", SqlDbType.NVarChar).Value = request.SortColumn;
                        command.Parameters.Add("@rowCount", SqlDbType.Int).Value = request.RowCount;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformLoginCreationHistory(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public LoginDeletionHistoryResponse GetLoginDeletionHistoryData(SqlConnection connection, string query, LoginDeletionHistoryRequest request)
        {
            var result = new LoginDeletionHistoryResponse();
            try
            {
                using (connection)
                {
                    var serverVersion = GetSqlServerVersion(connection.ServerVersion);
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@eventDatabase", SqlDbType.NVarChar).Value = request.Instance.Replace(@"\", @"_");
                        command.Parameters.Add("@loginName", SqlDbType.NVarChar).Value = request.Login;
                        command.Parameters.Add("@startDate", SqlDbType.DateTime).Value = request.From;
                        command.Parameters.Add("@endDate", SqlDbType.DateTime).Value = request.To;
                        command.Parameters.Add("@sortColumn", SqlDbType.NVarChar).Value = request.SortColumn;
                        command.Parameters.Add("@rowCount", SqlDbType.Int).Value = request.RowCount;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformLoginDeletionHistory(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ObjectActivityResponse GetObjectActivityData(SqlConnection connection, string query, ObjectActivityRequest request)
        {
            var result = new ObjectActivityResponse();
            try
            {
                using (connection)
                {
                    var serverVersion = GetSqlServerVersion(connection.ServerVersion);
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@eventDatabaseAll", SqlDbType.NVarChar).Value = request.instance.Replace(@"\", @"_");
                        command.Parameters.Add("@databaseName", SqlDbType.NVarChar).Value = request.Database;
                        command.Parameters.Add("@loginName", SqlDbType.NVarChar).Value = request.LoginName;
                        command.Parameters.Add("@objectName", SqlDbType.NVarChar).Value = request.ObjectName;
                        command.Parameters.Add("@eventCategory", SqlDbType.Int).Value = request.category;
                        command.Parameters.Add("@startDate", SqlDbType.DateTime).Value = request.from;
                        command.Parameters.Add("@endDate", SqlDbType.DateTime).Value = request.to;
                        command.Parameters.Add("@privilegedUserOnly", SqlDbType.Bit).Value = request.user;
                        command.Parameters.Add("@showSqlText", SqlDbType.Bit).Value = request.sql;
                        command.Parameters.Add("@sortColumn", SqlDbType.NVarChar).Value = request.SortColumn;
                        command.Parameters.Add("@rowCount", SqlDbType.Int).Value = request.RowCount;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformObjectActivity(reader));
                            }
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PermissionDeniedActivityResponse GetPermissionDeniedActivityData(SqlConnection connection, string query, PermissionDeniedActivityRequest request)
        {
            var result = new PermissionDeniedActivityResponse();
            try
            {
                using (connection)
                {
                    var serverVersion = GetSqlServerVersion(connection.ServerVersion);
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@eventDatabase", SqlDbType.NVarChar).Value = request.instance.Replace(@"\", @"_");
                        command.Parameters.Add("@databaseName", SqlDbType.NVarChar).Value = request.Database;
                        command.Parameters.Add("@loginName", SqlDbType.NVarChar).Value = request.LoginName;
                        command.Parameters.Add("@eventCategory", SqlDbType.Int).Value = request.category;
                        command.Parameters.Add("@startDate", SqlDbType.DateTime).Value = request.from;
                        command.Parameters.Add("@endDate", SqlDbType.DateTime).Value = request.to;
                        command.Parameters.Add("@privilegedUserOnly", SqlDbType.Bit).Value = request.user;
                        command.Parameters.Add("@showSqlText", SqlDbType.Bit).Value = request.sql;
                        command.Parameters.Add("@sortColumn", SqlDbType.NVarChar).Value = request.SortColumn;
                        command.Parameters.Add("@rowCount", SqlDbType.Int).Value = request.RowCount;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformPermissionDeniedActivity(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public UserActivityResponse GetUserActivityData(SqlConnection connection, string query, UserActivityRequest request)
        {
            var result = new UserActivityResponse();
            try
            {
                using (connection)
                {
                    var serverVersion = GetSqlServerVersion(connection.ServerVersion);
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@eventDatabase", SqlDbType.NVarChar).Value = request.instance.Replace(@"\", @"_");
                        command.Parameters.Add("@databaseName", SqlDbType.NVarChar).Value = request.Database;
                        command.Parameters.Add("@loginName", SqlDbType.NVarChar).Value = request.LoginName;
                        command.Parameters.Add("@eventCategory", SqlDbType.Int).Value = request.category;
                        command.Parameters.Add("@startDate", SqlDbType.DateTime).Value = request.from;
                        command.Parameters.Add("@endDate", SqlDbType.DateTime).Value = request.to;
                        command.Parameters.Add("@privilegedUserOnly", SqlDbType.Bit).Value = request.user;
                        command.Parameters.Add("@showSqlText", SqlDbType.Bit).Value = request.sql;
                        command.Parameters.Add("@sortColumn", SqlDbType.NVarChar).Value = request.SortColumn;
                        command.Parameters.Add("@rowCount", SqlDbType.Int).Value = request.RowCount;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformUserActivity(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //start sqlcm 5.6 -5469(Configuration Check Report)
        public ConfigurationSettingDefaultResponse GetConfigurationCheckDefaultDatabase(SqlConnection connection,string query)
        {
            try
            {
                ConfigurationSettingDefaultResponse result = new ConfigurationSettingDefaultResponse();
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {

                           while(reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    ConfigurationSettingKeyValue value = new ConfigurationSettingKeyValue();
                                    value.key = reader.GetName(i);
                                    if (value.key.Equals("auditFailures") || value.key.Equals("auditUserFailures"))
                                        value.value = reader.GetByte(i).ToString();
                                    else if (value.key.Equals("auditUsersList") || value.key.Equals("auditPrivUsersList"))
                                        value.value = SQLHelpers.GetString(reader, i);
                                    else
                                        value.value = reader.GetBoolean(i).ToString();
                                    result.Add(value);
                                }
                            }
                            return result;
                        }
                    }
                }
                
            }
            catch(Exception e)
            {
                throw;
            }
           
        }
        public ConfigurationSettingDefaultResponse GetConfigurationCheckDefaultServer(SqlConnection connection,String query)
        {
            try
            {
                ConfigurationSettingDefaultResponse result = new ConfigurationSettingDefaultResponse();
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    ConfigurationSettingKeyValue value = new ConfigurationSettingKeyValue();
                                    value.key = reader.GetName(i);
                                    if (value.key.Equals("auditFailures") || value.key.Equals("auditUserFailures") || value.key.Equals("defaultAccess"))
                                        value.value = reader.GetByte(i).ToString();
                                    else if (value.key.Equals("auditUsersList") || value.key.Equals("auditTrustedUsersList"))
                                        value.value = SQLHelpers.GetString(reader, i);
                                    else if (value.key.Equals("maxSqlLength"))
                                        value.value = reader.GetInt32(i).ToString();
                                    else
                                        value.value = reader.GetBoolean(i).ToString();
                                    result.Add(value);
                                }
                            }
                            return result;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw;
            }
        }
        public ConfigurationCheckSettingResponse GetConfigurationCheckSetting(SqlConnection connection,string query)
        {
            var result = new ConfigurationCheckSettingResponse();
            try
            {
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            result.Add(reader.GetString(1));
                        }
                        return result;
                    }
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }
        public ConfigurationCheckResponse GetConfigurationCheckData(SqlConnection connection, string query, ConfigurationCheckRequest request)
        {
            var result = new ConfigurationCheckProcedureResponse();
            try
            {
                string serverName = request.Instance;
                string databaseName = request.Database;
                int serverID = 0, databaseId = 0;
                
                
                using (connection)
                {
                    if (serverName.Equals("All"))
                        serverID = 0;
                    else
                    {
                        using (var command = new SqlCommand(String.Format("select srvId from {0}..{1} where instance='{2}'", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryServerTable, serverName), connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                reader.Read();
                                serverID = reader.GetInt32(reader.GetOrdinal("srvId"));
                            }
                        }
                    }

                    if (databaseName.Equals("All"))
                        databaseId = 0;
                    else
                    {
                        using (var command = new SqlCommand(String.Format("select sqlDatabaseId from {0}..{1} where name='{2}'", CoreConstants.RepositoryDatabase, CoreConstants.RepositoryDatabaseTable, databaseName), connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                reader.Read();
                                databaseId = reader.GetInt16(reader.GetOrdinal("sqlDatabaseId"));
                            }
                        }
                    }

                    var serverVersion = GetSqlServerVersion(connection.ServerVersion);
                    using (var command = new SqlCommand(query, connection))
                    {

                        
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@flag", SqlDbType.Bit).Value = 0;
                        command.Parameters.Add("@serverName", SqlDbType.Int).Value = serverID;
                        command.Parameters.Add("@dbName", SqlDbType.Int).Value = databaseId;
                        command.Parameters.Add("@status", SqlDbType.Int).Value = request.DefaultStatus;
                        command.Parameters.Add("@auditSetting", SqlDbType.Int).Value = request.AuditSetting;


                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ConfigurationCheckProcedureData data = new ConfigurationCheckProcedureData();
                                Transformer.Instance.ConfigurationCheckServerConfiguration(data, reader);
                                result.Add(data);
                            }

                        }
                    }

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@flag", SqlDbType.Bit).Value = 1;
                        command.Parameters.Add("@serverName", SqlDbType.Int).Value = serverID;
                        command.Parameters.Add("@dbName", SqlDbType.Int).Value = databaseId;
                        command.Parameters.Add("@status", SqlDbType.Int).Value = request.DefaultStatus;
                        command.Parameters.Add("@auditSetting", SqlDbType.Int).Value = request.AuditSetting;


                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ConfigurationCheckProcedureData data = new ConfigurationCheckProcedureData();
                                Transformer.Instance.ConfigurationCheckDatabaseConfiguration(data, reader);
                                result.Add(data);
                            }


                        }
                    }


                    ConfigurationCheckResponse response = new ConfigurationCheckResponse();
                    for (int i = 0; i < result.ConfigurationCheck.Count; i++)
                    {
                        if (result.ConfigurationCheck[i].flag == 0)
                        {
                            ConfigurationCheckJSONServer configurationCheckJSONServer = new ConfigurationCheckJSONServer();
                            int serverId = result.ConfigurationCheck[i].srvId;
                            configurationCheckJSONServer.srvId = result.ConfigurationCheck[i].srvId;
                            configurationCheckJSONServer.instance = result.ConfigurationCheck[i].instance;
                            configurationCheckJSONServer.isDeployed = result.ConfigurationCheck[i].isDeployed;
                            configurationCheckJSONServer.eventDatabase = result.ConfigurationCheck[i].eventDatabase;
                            configurationCheckJSONServer.agentVersion = result.ConfigurationCheck[i].agentVersion;
                            configurationCheckJSONServer.auditAdmin = result.ConfigurationCheck[i].auditAdmin;
                            configurationCheckJSONServer.auditCaptureSql = result.ConfigurationCheck[i].auditCaptureSQL;
                            configurationCheckJSONServer.auditCaptureSqlXE = result.ConfigurationCheck[i].auditCaptureSQLXE;
                            configurationCheckJSONServer.auditDBCC = result.ConfigurationCheck[i].auditDBCC;
                            configurationCheckJSONServer.auditDDL = result.ConfigurationCheck[i].auditDDL;
                            configurationCheckJSONServer.auditDML = result.ConfigurationCheck[i].auditDML;
                            configurationCheckJSONServer.auditExceptions = result.ConfigurationCheck[i].auditExceptions;
                            configurationCheckJSONServer.auditFailedLogins = result.ConfigurationCheck[i].auditFailedLogins;
                            configurationCheckJSONServer.auditFailures = result.ConfigurationCheck[i].auditFailures;
                            configurationCheckJSONServer.auditLogins = result.ConfigurationCheck[i].auditLogins;
                            configurationCheckJSONServer.auditLogouts = result.ConfigurationCheck[i].auditLogouts;
                            configurationCheckJSONServer.auditSecurity = result.ConfigurationCheck[i].auditSecurity;
                            configurationCheckJSONServer.auditSELECT = result.ConfigurationCheck[i].auditSELECT;
                            configurationCheckJSONServer.auditSystemEvents = result.ConfigurationCheck[i].auditSystemEvents;
                            configurationCheckJSONServer.auditTrace = result.ConfigurationCheck[i].auditTrace;
                            configurationCheckJSONServer.auditUDE = result.ConfigurationCheck[i].auditUDE;
                            configurationCheckJSONServer.auditUserAdmin = result.ConfigurationCheck[i].auditUserAdmin;
                            configurationCheckJSONServer.auditUserAll = result.ConfigurationCheck[i].auditUserAll;
                            configurationCheckJSONServer.auditUserCaptureDDL = result.ConfigurationCheck[i].auditUserCaptureDDL;
                            configurationCheckJSONServer.auditUserCaptureSQL = result.ConfigurationCheck[i].auditUserCaptureSQL;
                            configurationCheckJSONServer.auditUserCaptureTrans = result.ConfigurationCheck[i].auditUserCaptureTrans;
                            configurationCheckJSONServer.auditUserDDL = result.ConfigurationCheck[i].auditUserDDL;
                            configurationCheckJSONServer.auditUserDML = result.ConfigurationCheck[i].auditUserDML;
                            configurationCheckJSONServer.auditUserExceptions = result.ConfigurationCheck[i].auditUserExceptions;
                            configurationCheckJSONServer.auditUserExtendedEvents = result.ConfigurationCheck[i].auditUserExtendedEvents;
                            configurationCheckJSONServer.auditUserFailedLogins = result.ConfigurationCheck[i].auditUserFailedLogins;
                            configurationCheckJSONServer.auditUserFailures = result.ConfigurationCheck[i].auditUserFailures;
                            configurationCheckJSONServer.auditUserLogins = result.ConfigurationCheck[i].auditUserLogins;
                            configurationCheckJSONServer.auditUserLogouts = result.ConfigurationCheck[i].auditUserLogouts;
                            configurationCheckJSONServer.auditUsers = result.ConfigurationCheck[i].auditUsers;
                            configurationCheckJSONServer.auditUserSecurity = result.ConfigurationCheck[i].auditUserSecurity;
                            configurationCheckJSONServer.auditUserSELECT = result.ConfigurationCheck[i].auditUserSELECT;
                            configurationCheckJSONServer.auditUsersList = result.ConfigurationCheck[i].auditUsersList;
                            configurationCheckJSONServer.auditUserUDE = result.ConfigurationCheck[i].auditUserUDE;
                            // SQLCM-5867: Web console Configuration Check report is different from the management console report generated
                            configurationCheckJSONServer.isAuditLogEnabled = result.ConfigurationCheck[i].isAuditLogEnabled;
                            configurationCheckJSONServer.auditTrustedUsersList = result.ConfigurationCheck[i].auditTrustedUsersList;

                            for (int j = 0; j < result.ConfigurationCheck.Count; j++)
                            {
                                if (i != j)
                                {
                                    if (result.ConfigurationCheck[j].flag == 1 && serverId == result.ConfigurationCheck[j].srvId)
                                    {
                                        configurationCheckJSONServer.databasesConfigurationList.Add(result.ConfigurationCheck[j]);
                                    }
                                }
                            }
                            response.Add(configurationCheckJSONServer);

                        }
                    }



                    return response;



                }
                
            }
            catch (Exception ex)
            {
                throw;
            }

           

        }
        //end sqlcm 5.6 - 5469(Configuration Check Report)
        public RowCountResponse GetRowCountData(SqlConnection connection, string query, RowCountRequest request)
        {
            var result = new RowCountResponse();
            try
            {
                using (connection)
                {
                    var serverVersion = GetSqlServerVersion(connection.ServerVersion);
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@eventDatabaseAbove2005", SqlDbType.NVarChar).Value = request.instance.Replace(@"\", @"_");
                        command.Parameters.Add("@databaseNameAll", SqlDbType.NVarChar).Value = request.Database;
                        command.Parameters.Add("@loginNameAbove2005", SqlDbType.NVarChar).Value = request.LoginName;
                        command.Parameters.Add("@tableName", SqlDbType.NVarChar).Value = request.objectName;
                        command.Parameters.Add("@columnName", SqlDbType.NVarChar).Value = request.columnName;
                        command.Parameters.Add("@startDate", SqlDbType.DateTime).Value = request.from;
                        command.Parameters.Add("@endDate", SqlDbType.DateTime).Value = request.to;
                        command.Parameters.Add("@privilegedUsers", SqlDbType.NVarChar).Value = request.privilegedUsers;
                        command.Parameters.Add("@showSqlText", SqlDbType.Bit).Value = request.sql;
                        command.Parameters.Add("@rowCountThreshold", SqlDbType.Int).Value = request.rowCountThreshold;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformRowCount(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public RegulatoryComplianceResponse GetRegulatoryComplianceData(SqlConnection connection, string query, RegulatoryComplianceRequest request)
        {
            var result = new RegulatoryComplianceResponse();
            try
            {
                using (connection)
                {
                    Transformer.Instance.IntializeRegulationSettings(connection);
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@serverName", SqlDbType.NVarChar).Value = request.ServerName;
                        command.Parameters.Add("@dbName", SqlDbType.NVarChar).Value = request.DatabaseName;
                        command.Parameters.Add("@getServerSettings", SqlDbType.Bit).Value = 1;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformRegulatoryComplianceServer(reader, request));
                            }
                        }
                    }

                    if (request.Values != (int)Transformer.Values.Varies)
                    {
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("@serverName", SqlDbType.NVarChar).Value = request.ServerName;
                            command.Parameters.Add("@dbName", SqlDbType.NVarChar).Value = request.DatabaseName;
                            command.Parameters.Add("@getServerSettings", SqlDbType.Bit).Value = 0;

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    result.Add(Transformer.Instance.TransformRegulatoryComplianceDatabase(reader, request));
                                }   
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        internal DataAlertRulesInfo GetDataAlertRulesInfo(SqlConnection connection, string query, DataAlertRulesServerId request)
        {
            var result = new DataAlertRulesInfo();
            try
            {
                Console.Out.WriteLine("inside Responce");
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                  //      command.Parameters.Add("@ServerId", SqlDbType.Int).Value = request.SrvId;
                        command.Parameters.Add("@ConditionId", SqlDbType.Int).Value = request.ConditionId;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformDataAlertRulesDBDetail(reader));
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformDataAlertRulesTableDetail(reader));
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.Add(Transformer.Instance.TransformDataAlertRulesColumnDetail(reader));
                            }

                            //DataAlertRulesColumnDetail columnDetails = new DataAlertRulesColumnDetail();

                            //result.Add(columnDetails);

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal ServerActivityLogs GetActivityProperties(SqlConnection connection, string query, FilteredActivityLogsViewRequest request)
        {
            var result = new ServerActivityLogs();
            try
            {
                Console.Out.WriteLine("inside Responce");
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@EventId", SqlDbType.Int).Value = request.EventId.HasValue ? (int)request.EventId.Value : SqlInt32.Null;
                        command.Parameters.Add("@Instance", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.InstanceName) ? SqlString.Null : request.InstanceName;
                        command.Parameters.Add("@DateFrom", SqlDbType.DateTime).Value = !request.DateFrom.HasValue ? SqlDateTime.Null : request.DateFrom.Value;
                        command.Parameters.Add("@DateTo", SqlDbType.DateTime).Value = !request.DateTo.HasValue ? SqlDateTime.Null : request.DateTo.Value;
                        command.Parameters.Add("@TimeFrom", SqlDbType.DateTime).Value = !request.TimeFrom.HasValue ? SqlDateTime.Null : request.TimeFrom.Value;
                        command.Parameters.Add("@TimeTo", SqlDbType.DateTime).Value = !request.TimeTo.HasValue ? SqlDateTime.Null : request.TimeTo.Value;
                        command.Parameters.Add("@EventType", SqlDbType.Int).Value = request.EventType.HasValue ? (int)request.EventType.Value : SqlInt32.Null;
                        command.Parameters.Add("@Details", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Detail) ? SqlString.Null : request.Detail; ;
                        command.Parameters.Add("@Page", SqlDbType.Int).Value = request.Page.HasValue ? request.Page.Value : SqlInt32.Null;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = request.PageSize.HasValue ? request.PageSize.Value : SqlInt32.Null;
                        command.Parameters.Add("@SortColumn", SqlDbType.NVarChar).Value = !string.IsNullOrWhiteSpace(request.SortColumn) ? request.SortColumn : SqlString.Null;
                        command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = request.SortDirection.HasValue ? request.SortDirection.Value : SqlInt32.Null;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                            reader.NextResult();
                            while (reader.Read())
                            {
                                result = Transformer.Instance.TransformActivityLogsPropertyData(reader);
                            }
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        internal ServerChangeLogs GetChangeProperties(SqlConnection connection, string query, FilteredChangeLogsViewRequest request)
        {
            var result = new ServerChangeLogs();
            try
            {
                Console.Out.WriteLine("inside Responce");
                using (connection)
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@EventId", SqlDbType.Int).Value = request.EventId.HasValue ? (int)request.EventId.Value : SqlInt32.Null;
                        command.Parameters.Add("@Instance", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.InstanceName) ? SqlString.Null : request.InstanceName;
                        command.Parameters.Add("@DateFrom", SqlDbType.DateTime).Value = !request.DateFrom.HasValue ? SqlDateTime.Null : request.DateFrom.Value;
                        command.Parameters.Add("@DateTo", SqlDbType.DateTime).Value = !request.DateTo.HasValue ? SqlDateTime.Null : request.DateTo.Value;
                        command.Parameters.Add("@TimeFrom", SqlDbType.DateTime).Value = !request.TimeFrom.HasValue ? SqlDateTime.Null : request.TimeFrom.Value;
                        command.Parameters.Add("@TimeTo", SqlDbType.DateTime).Value = !request.TimeTo.HasValue ? SqlDateTime.Null : request.TimeTo.Value;
                        command.Parameters.Add("@LogType", SqlDbType.Int).Value = request.EventType.HasValue ? (int)request.EventType.Value : SqlInt32.Null;
                        command.Parameters.Add("@Details", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Detail) ? SqlString.Null : request.Detail; ;
                        command.Parameters.Add("@Page", SqlDbType.Int).Value = request.Page.HasValue ? request.Page.Value : SqlInt32.Null;
                        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = request.PageSize.HasValue ? request.PageSize.Value : SqlInt32.Null;
                        command.Parameters.Add("@SortColumn", SqlDbType.NVarChar).Value = !string.IsNullOrWhiteSpace(request.SortColumn) ? request.SortColumn : SqlString.Null;
                        command.Parameters.Add("@SortOrder", SqlDbType.Int).Value = request.SortDirection.HasValue ? request.SortDirection.Value : SqlInt32.Null;

                        using (var reader = command.ExecuteReader())
                        {
                            reader.NextResult();

                            while (reader.Read())
                            {
                                result = Transformer.Instance.TransformChangeLogsPropertyData(reader);
                            }

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public CategoryResponse GetCateGoryInfo(SqlConnection connection, CategoryRequest request)
        {
            var result = new CategoryResponse();
            try
            {
                Console.Out.WriteLine("inside Responce");
                using (connection)
                {
                    using (var command = new SqlCommand("[sp_sqlcm_GetEventCategory]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@category", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(request.Category) ? SqlString.Null : request.Category;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformCategoryData(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ViewNameResponse GetViewName(SqlConnection connection, String viewId)
        {
            var result = new ViewNameResponse();
            try
            {
                Console.Out.WriteLine("inside Responce");
                using (connection)
                {
                    using (var command = new SqlCommand("[sp_sqlcm_GetViewName]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ViewId", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(viewId) ? SqlString.Null : viewId;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                result.Add(Transformer.Instance.TransformViewNameData(reader));

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        internal string ImportAuditEventFilters(SqlConnection connection, string request)
        {
            string result = "failed";
            try
            {
                FilterTemplate filterTemplate = new FilterTemplate();
                if (filterTemplate.LoadCWF(request) == true)
                {
                    foreach (EventFilter filter in filterTemplate.EventFilters)
                    {
                        filter.Enabled = false;
                        FiltersDal.InsertEventFilter(filter, connection);
                    }
                    result = "success";
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }
        internal string ImportCwfEventFilter(SqlConnection connection, String request)
        {
            string result = "failed";
            try
            {
                setEventFieldValue();
                int count = 1;
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fileNameOnly = "EventFilters";
                string extension = ".xml";
                String exportPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\EventFilters.xml";
                while (File.Exists(exportPath))
                {
                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                    exportPath = Path.Combine(path, tempFileName + extension);
                }
                FilterTemplate tmpl = new FilterTemplate();
                tmpl.ImportCWF(connection.ConnectionString, request);
                tmpl.Save(exportPath);
                result = exportPath;
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        public int getServerPort(SqlConnection connection)
        {
            int serverPort = 0;
            var query = "SELECT serverPort from [SQLcompliance].[dbo].[Configuration]";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        serverPort = Transformer.Instance.ReadData<int>(reader, "serverPort");
                    }
                }
            }
            return serverPort;
        }

        public string getSever(SqlConnection connection)
        {
            string serverRepositoryName = "";
            var query = "SELECT serverPort,server from [SQLcompliance].[dbo].[Configuration]";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        serverRepositoryName = Transformer.Instance.ReadData<string>(reader, "server");
                    }
                }
            }
            return serverRepositoryName;
        }
		public SNMPConfigurationData UpdateSnmpConfigData(SqlConnection connection)
        {
            SNMPConfigurationData serverRepositoryData = new SNMPConfigurationData();
            var query = "SELECT snmpServerAddress,snmpPort,snmpCommunity from [SQLcompliance].[dbo].[Configuration]";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        serverRepositoryData.SnmpAddress = Transformer.Instance.ReadData<string>(reader, "snmpServerAddress");
                        serverRepositoryData.Port = Transformer.Instance.ReadData<int>(reader, "snmpPort");
                        serverRepositoryData.Community = Transformer.Instance.ReadData<string>(reader, "snmpCommunity");
                    }
                }
            }
            return serverRepositoryData;
        }

        public bool CheckSnmpAddress(SNMPConfigurationData request)
        {
            String errorString;
            Idera.SQLcompliance.Core.Rules.Alerts.SNMPConfiguration snmpConfiguration = 
                new Idera.SQLcompliance.Core.Rules.Alerts.SNMPConfiguration();
            snmpConfiguration.ReceiverAddress = request.SnmpAddress;
            snmpConfiguration.ReceiverPort = request.Port;
            snmpConfiguration.Community = request.Community;
            return Idera.SQLcompliance.Core.Rules.Alerts.ActionProcessor.PerformSnmpTrapTest(snmpConfiguration, out errorString);
        }
		
		       //SQLCM 5.4 SCM-9 Start
    public string CreateRefreshDuration(SqlConnection connection, string query, String refreshDuration)
    {
        var result = new int();
        try
        {
            using (var command = new SqlCommand(query, connection))
            {
               result= (int)command.ExecuteNonQuery();
               if (result > 0)
               {
                   return "true";
               }
               else
               {
                   return "false";
               }
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public string GetRefreshDuration(SqlConnection connection, string query)
    {
        var result = new int();
        try
        {
            using (var command = new SqlCommand(query, connection))
            {

                result = (int)command.ExecuteScalar();

                return result.ToString();

            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
	//Start SQLCm-5.4 
     //Requirement - 4.1.3.1
    public SensitiveColumnInfo validateSensitiveColumns(SqlConnection connection, String csvData)
    {
        var result = new SensitiveColumnInfo();
        result.validFile = false;
        int tableCount = 0, columnCount = 0, tableIndex = 0;
        String instance = csvData.Substring(0, csvData.IndexOf('*'));
        csvData = csvData.Replace(instance + "*", "\n");
        csvData = csvData.Replace("\r\n", "\n");
        csvData = csvData.TrimStart('\n');
        csvData = csvData.TrimEnd('\n');
        //csvData = csvData.TrimStart(' ');
        //csvData = csvData.TrimEnd(' ');
        csvData = csvData.Trim(); // SCM-228
        String[] sensitiveColumnLines = csvData.Split('\n');
        String[] temp;

        if (csvData.Length == 0)
        {
            result.validFile = false;
        }
            
        //for removing duplicate entries from csv
        for (int i = 0; i < sensitiveColumnLines.Length; i++)
        {
            for (int j = i + 1; j < sensitiveColumnLines.Length; j++)
            {
                if (sensitiveColumnLines[i].Equals(sensitiveColumnLines[j], StringComparison.InvariantCultureIgnoreCase))
                {
                    sensitiveColumnLines[j] = "";
                }
            }
        }

        //for removing duplicate database names and table names from return string
        for (int i = 0; i < sensitiveColumnLines.Length; i++)
        {
            temp = new String[sensitiveColumnLines.Length - 1];
            if (sensitiveColumnLines[i] == "")
                continue;
            String[] lineDetails = sensitiveColumnLines[i].Split(',');
            if (lineDetails.Length >= 2)
            {
                String dbName = lineDetails[0];
                String tblName = lineDetails[1];
                for (int j = i; j < sensitiveColumnLines.Length; j++)
                {
                    if (sensitiveColumnLines[j] == "")
                    {
                        continue;
                    }
                    String[] lineDetails1 = sensitiveColumnLines[j].Split(',');
                    if (lineDetails1.Length > 2)
                    {
                        String dbName1 = lineDetails1[0];
                        String tblName1 = lineDetails1[1];
                        if (j != i)
                        {
                            if (dbName.Equals(dbName1, StringComparison.InvariantCultureIgnoreCase) && tblName.Equals(tblName1, StringComparison.InvariantCultureIgnoreCase))
                            {
                                for (int k = 2; k < lineDetails1.Length; k++)
                                {
                                    sensitiveColumnLines[i] = sensitiveColumnLines[i] + "," + lineDetails1[k];
                                }
                                sensitiveColumnLines[j] = "";
                            }
                        }
                    }
                }
            }
        }

        //for removing duplicate column names from return string
        for (int i = 0; i < sensitiveColumnLines.Length; i++)
        {
            if (sensitiveColumnLines[i] == "")
                continue;
            String[] lineDetails = sensitiveColumnLines[i].Split(',');
            String tempLineDetails="";
            if (lineDetails.Length >= 2)
            {
                tempLineDetails = lineDetails[0] + "," + lineDetails[1];
                for (int j = 2; j < lineDetails.Length - 1; j++)
                {
                    String colName = lineDetails[j];
                    for (int k = j + 1; k < lineDetails.Length; k++)
                    {
                        if (colName.Equals(lineDetails[k], StringComparison.InvariantCultureIgnoreCase))
                        {
                            lineDetails[k] = "";
                        }
                    }
                    tempLineDetails =  tempLineDetails + "," + lineDetails[j];
                    if(j==lineDetails.Length-2)
                        tempLineDetails = tempLineDetails + "," + lineDetails[j+1];
                }
            }
            if (lineDetails.Length == 3)
                tempLineDetails = tempLineDetails + "," + lineDetails[2];
            sensitiveColumnLines[i] = tempLineDetails;
        }


        Array.Sort(sensitiveColumnLines, StringComparer.InvariantCulture);
        for (var index = 0; index < sensitiveColumnLines.Length; index += 1)
        {
            var sensitiveColumnDetail = sensitiveColumnLines[index].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (sensitiveColumnDetail.Length < 2)
                continue;

            var databaseName = sensitiveColumnDetail[0].Trim();
            var scTableName = sensitiveColumnDetail[1].Trim();
            var allColumns = sensitiveColumnDetail.Length == 2;

            try
            {
                AgentManager manager = new AgentManager();

                // get list of all databses on server, leaving out 'tempdb'
                var dbList = manager.GetRawUserDatabasesCWF(connection, instance);
                var sysList = manager.GetRawSystemDatabasesCWF(connection, instance);
                if (dbList != null && sysList != null)
                {
                    foreach (object o in sysList)
                        if (o.ToString() != "tempdb")
                            dbList.Add(o);
                }
                foreach (RawDatabaseObject rdo in dbList)
                {
                    if (!rdo.name.Equals(databaseName, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    _tablelist = manager.GetRawTablesCWF(connection, instance, rdo.name);
                    result.Add(Transformer.Instance.TransformSensitiveColumnDBDetail(rdo.name, rdo.dbid,true));
                    result.validFile = true;

                    foreach (RawTableObject table in _tablelist)
                    {
                        if (!table.FullTableName.Equals(scTableName, StringComparison.InvariantCultureIgnoreCase) &&
                            !table.FullTableName.Equals(String.Format("DBO.{0}", scTableName), StringComparison.InvariantCultureIgnoreCase))
                            continue;
                        tableCount++;
                        ICollection columnList = manager.GetRawColumnsCWF(connection, instance, rdo.name, table.FullTableName);
                        result.Add(Transformer.Instance.TransformSensitiveColumnTableDetail(table.FullTableName, table.id, rdo.dbid,true));
                        tableIndex++;

                        if (allColumns)
                        {
                            foreach (RawColumnObject rco in columnList)
                            {
                                columnCount++;
                                result.Add(Transformer.Instance.TransformSensitiveColumnColumnDetail(rco.ColumnName, rco.Id, table.id, rdo.dbid,true));
                            }
                        }

                        else
                        {
                            for (int p = 2; p < sensitiveColumnDetail.Length; p++)
                            {
                                String colName = sensitiveColumnDetail[p];
                                // SCM-228
                                colName = colName.Trim();
                                foreach (RawColumnObject rco in columnList)
                                {
                                    
                                    if (rco.ColumnName.Equals(colName, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        columnCount++;
                                        result.Add(Transformer.Instance.TransformSensitiveColumnColumnDetail(rco.ColumnName, rco.Id, table.id, rdo.dbid,true));
                                        break;
                                    }
                                }
                            }
                        }
                        if (tableCount > 0 && columnCount == 0)
                            result.Remove(tableIndex-1);
                        columnCount = 0;
                        tableCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                result.validFile = false;
                ex.ToString();
            }
        }
        return result;
    }
 
    //End Requirement - 4.1.3.1

      //Start - Requirement 4.1.4.1
      public AllImportSettingDetails ParseXml(SqlConnection sqlConnection, String xmlData)
      {
          InstanceTemplate.dbDetails.Clear();
          List<String> serverNames = new List<String>();
          var result = new AllImportSettingDetails();
          try
          {
              List<ServerRecord> servers = ServerRecord.GetServers(sqlConnection, false);
              foreach (ServerRecord server in servers)
              {
                  result.Add(Transformer.Instance.TransformServerDetails(server.ToString(), server.SrvId));                  
                  List<DatabaseRecord> dbs = DatabaseRecord.GetDatabases(sqlConnection, server.SrvId);
                  foreach(DatabaseRecord db in dbs)
                  {
                        result.Add(Transformer.Instance.TransformDBDetails(db.Name, db.DbId, server.SrvId)); 
                  }
              }
              InstanceTemplate tmp;
              XMLHandler<InstanceTemplate> reader = new XMLHandler<InstanceTemplate>();
              tmp = reader.ReadCWF(xmlData);
              result.validFile = true;

              result.allDBNames = InstanceTemplate.dbDetails;
              if (tmp.AuditTemplate.ServerLevelConfig == null)
              {
                  result.ServerLevelConfig = "false";
              }
              else
              {
                  result.ServerLevelConfig = "true";
              }

              if (tmp.AuditTemplate.DbLevelConfigs != null &&
                   tmp.AuditTemplate.DbLevelConfigs.Length > 0)
              {
                  DBAuditConfig[] config = tmp.AuditTemplate.DbLevelConfigs;
                  if (config != null && config.Length > 0)
                  {
                      UpdateDBRecord(result.auditedActivities, config[0]);
                  }
                  foreach (DBAuditConfig item in config)
                  {
                      if (item.SensitiveColumnTables != null && item.SensitiveColumnTables.Length > 0)
                      {
                          result.auditedActivities.AuditSensitiveColumns = true;
                      }
                      if (item.DataChangeTables != null && item.DataChangeTables.Length > 0)
                      {
                          result.auditedActivities.AuditBeforeAfter = true;
                      }
                      if (item.PrivUserConfig != null)
                      {
                          result.auditedActivities.AuditPrivilegedUsers = true;
                      }
                  }              
              }

              if (tmp.AuditTemplate.ServerLevelConfig != null)
              {
                    ServerAuditConfig _serverConfig = tmp.AuditTemplate.ServerLevelConfig;
                    if (_serverConfig.Categories != null && _serverConfig.Categories.Length > 0)
                    {
                        foreach (AuditCategory cat in _serverConfig.Categories)
                            UpdateServerRecord(cat, result.serverAuditedActivities);                        
                    }
                    result.serverAuditedActivities.AuditAccessCheck = (AccessCheckFilter)_serverConfig.AccessCheckFilter;
                    if (tmp.AuditTemplate.PrivUserConfig != null)
                    {
                        result.serverAuditedActivities.AuditPrivilegedUsers = true;
                    }
              }              

              if (tmp.AuditTemplate.PrivUserConfig == null)
              {
                  result.privUserConfig = "false";
              }
              else
              {
                  result.privUserConfig = "true";
              }

              result.DatabasePrivUser = "false";

              if (tmp.AuditTemplate.DbLevelConfigs == null ||
                    tmp.AuditTemplate.DbLevelConfigs.Length == 0)
              {
                  result.Database = "false";
                  result.MatchDBNames = "false";
              }
              else
              {
                  result.Database = "true";
                  result.MatchDBNames = "true";
                  foreach (DBAuditConfig config in tmp.AuditTemplate.DbLevelConfigs)
                  {
                      if (config.PrivUserConfig != null)
                      {
                          result.DatabasePrivUser = "true";
                      }
                  }
              }
          }
          catch (Exception ex)
          {
              result.validFile = false;
              ex.ToString();
          }

          return result;
      }


      private void UpdateDBRecord(AuditActivity tempDb, DBAuditConfig config)
      {
          // Basic DB Settings
          foreach (AuditCategory cat in config.Categories)
          {
              switch (cat)
              {
                  case AuditCategory.DDL:
                      tempDb.AuditDDL = true;
                      break;
                  case AuditCategory.Security:
                      tempDb.AuditSecurity = true;
                      break;
                  case AuditCategory.Admin:
                      tempDb.AuditAdmin = true;
                      break;
                  case AuditCategory.SELECT:
                      tempDb.AuditSELECT = true;
                      break;
                  case AuditCategory.DML:
                      tempDb.AuditDML = true;
                      break;
              }
          }
          tempDb.AuditCaptureSQL = config.KeepSQL;
          tempDb.AuditCaptureTrans = config.AuditTrans;
          tempDb.AuditCaptureDDL = config.AuditDDL;
          tempDb.AuditAccessCheck = (AccessCheckFilter)config.AccessCheckFilter;
      }

      private void UpdateServerRecord(AuditCategory cat, AuditActivity tempServer)
      {
          switch (cat)
          {
              case AuditCategory.Logins:
                  tempServer.AuditLogins = true;
                  break;
                // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
                case AuditCategory.Logouts:
                  tempServer.AuditLogouts = true;
                  break;
              case AuditCategory.DDL:
                  tempServer.AuditDDL = true;
                  break;
              case AuditCategory.Security:
                  tempServer.AuditSecurity = true;
                  break;
              case AuditCategory.Admin:
                  tempServer.AuditAdmin = true;
                  break;
              case AuditCategory.FailedLogins:
                  tempServer.AuditFailedLogins = true;
                  break;
              case AuditCategory.UDC: // User defined
                  tempServer.AuditDefinedEvents = true;
                  break;
          }
      }
      //End - Requirement 4.1.4.1


      internal void GetSaveSensitiveColumnData(SqlConnection sqlConnection, SensitiveColumnInfo sensitiveColumnInfo)
      {
         // throw new NotImplementedException();
          var retVal = new List<SensitiveColumnTableRecord>();
          var updateSqlBuilder = new StringBuilder();
          foreach (SensitiveColumnDatabaseDetails scDatabase in sensitiveColumnInfo.sensitiveDatabase)
          {
              retVal.Clear();
              int serverId = scDatabase.SrvId;
              var dbId = DatabaseRecord.GetDatabaseId(sqlConnection, serverId, scDatabase.Name);
              foreach (SensitiveColumnTableDetails scTable in sensitiveColumnInfo.sensitiveTable)
              {
                  SensitiveColumnTableRecord sensitiveColumnTableRecord = new SensitiveColumnTableRecord();
                  if (scTable.DbId == scDatabase.DbId)
                  {
                      var oldSCLists = SensitiveColumnTableRecord.GetAuditedTables(sqlConnection, serverId, dbId);
                      
                      List<SensitiveColumnTableRecord> tableRecords = null;
                      if (oldSCLists != null)
                      {
                          tableRecords = new List<SensitiveColumnTableRecord>();
                          for (int i = 0; i < oldSCLists.Count; i++)
                          {
                              if (oldSCLists[i].Type != CoreConstants.IndividualColumnType)
                              {
                                  foreach (string table in oldSCLists[i].FullTableName.Split(','))
                                  {
                                      SensitiveColumnTableRecord newRecord = new SensitiveColumnTableRecord();
                                      newRecord.TableName = table.Split('.')[1];
                                      if (oldSCLists[i].tableIdMap.ContainsKey(table))
                                          newRecord.ObjectId = oldSCLists[i].tableIdMap[table];
                                      else
                                      newRecord.ObjectId = oldSCLists[i].ObjectId;
                                      newRecord.SchemaName = table.Split('.')[0]; ;
                                      newRecord.SelectedColumns = oldSCLists[i].SelectedColumns;
                                      newRecord.SrvId = oldSCLists[i].SrvId;
                                      newRecord.Columns = Array.FindAll(oldSCLists[i].Columns, x => x.Contains(table + "."));
                                      newRecord.DbId = oldSCLists[i].DbId;
                                      newRecord.ColumnId = oldSCLists[i].ColumnId;
                                      newRecord.Type = oldSCLists[i].Type;
                                      tableRecords.Add(newRecord);
                                  }
                                  oldSCLists.Remove(oldSCLists[i]);
                                  i--;
                              }
                          }
                      }

                      if (tableRecords != null && tableRecords.Count > 0)
                      {
                          oldSCLists.AddRange(tableRecords);
                      }                    
         
                      sensitiveColumnTableRecord.DbId = dbId;
                      sensitiveColumnTableRecord.SrvId = serverId;
                      sensitiveColumnTableRecord.ObjectId = scTable.TblId;
                      sensitiveColumnTableRecord.TableName = scTable.Name.Substring(scTable.Name.IndexOf('.')+1);
                      sensitiveColumnTableRecord.SchemaName = scTable.Name.Substring(0, scTable.Name.IndexOf('.'));
                      sensitiveColumnTableRecord.Type = CoreConstants.IndividualColumnType;
                      foreach (SensitiveColumnColumnDetail scColumn in sensitiveColumnInfo.sensitiveColumn)
                      {
                          if (scColumn.TblId == scTable.TblId)
                          {
                              sensitiveColumnTableRecord.AddColumn(scColumn.Name);
                          }
                      }
                      sensitiveColumnTableRecord.SelectedColumns = true;
                      retVal.Add(sensitiveColumnTableRecord);

                      
                      
                      
                      var isFound = false;

                      if (oldSCLists != null)
                      {
                          foreach (var oldSCList in oldSCLists)
                          {
                              foreach (var newSCList in retVal)
                              {
                                  if (oldSCList.Type == CoreConstants.IndividualColumnType && oldSCList.FullTableName.Equals(newSCList.FullTableName, StringComparison.InvariantCultureIgnoreCase))
                                  {
                                      isFound = true;
                                      break;
                                  }
                              }

                              if (!isFound)
                                  retVal.Add(oldSCList);

                              isFound = false;
                          }
                      }

                  }
                  SensitiveColumnTableRecord.CreateUserTables(sqlConnection, retVal, serverId, dbId, null);
              }

              updateSqlBuilder.AppendFormat("UPDATE {0} SET auditSensitiveColumns = 1 WHERE dbId={1} AND srvId={2} AND name='{3}';{4}", CoreConstants.RepositoryDatabaseTable, dbId, serverId, scDatabase.Name, Environment.NewLine);
              var command = new SqlCommand(updateSqlBuilder.ToString(), sqlConnection);
              command.ExecuteNonQuery();
          }
      }
	  
	  ///<Export Instance Audit XML File>
        /// Rest Services Method to export the instance Audit Setting xml file.
        ///</ExportAudit>
      internal string ExportServerAuditSettingsAction(SqlConnection connection, string server)
      {
          string result = "failed";
          if (server != null)
          {
              int count = 1;
              string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
              string fileNameOnly = String.Format("{0}_AuditSettings", server).Replace('\\', '_');
              string extension = ".xml";
              String exportPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\" + fileNameOnly + extension;
              while (File.Exists(exportPath))
              {
                  string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                  exportPath = Path.Combine(path, tempFileName + extension);
              }
              InstanceTemplate tmpl = new InstanceTemplate();
              tmpl.RepositoryServer = server;
              tmpl.ImportAuditSettingsCwf(connection, server);
              tmpl.Save(exportPath);
              result = exportPath;
          }
          return result;
      }

        ///<Export Database Audit XML File>
        /// Rest Services Method to export the Database Audit Setting xml file.
        ///</ExportAudit>
        public string ExportDatabaseAuditSettingsAction(SqlConnection connection,int databaseId)
        {
            string result = "failed";
            var db = SqlCmRecordReader.GetDatabaseRecord(databaseId, connection);
            if (db != null)
            {
                int count = 1;
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fileNameOnly = String.Format("{0}_{1}_AuditSettings", db.SrvInstance, db.Name).Replace('\\', '_');
                string extension = ".xml";
                String exportPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\" + fileNameOnly + extension;
                while (File.Exists(exportPath))
                {
                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                    exportPath = Path.Combine(path, tempFileName + extension);
                }
                InstanceTemplate tmpl = new InstanceTemplate();
                tmpl.ExportDatabaseAuditSettingsCwf(connection, db);
                tmpl.Save(exportPath);
                result = exportPath;
            }
          return result;
        }

        /***Start SQLCm 5.4***/
        /*Requirement 4.1.4.1*/

        public void ImportDatabaseAuditSetting(SqlConnection connection, AllImportSettingDetails request)
        {
            try
            { 
                int serverPort=0;
                string serverRepositoryName="";
                var query = "SELECT serverPort,server from [SQLcompliance].[dbo].[Configuration]";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            serverPort = Transformer.Instance.ReadData<int>(reader, "serverPort");
                            serverRepositoryName = Transformer.Instance.ReadData<string>(reader, "server");
                        }
                    }
                }
                InstanceTemplate _template = new InstanceTemplate();
                _template.LoadCWF(request.xmlData);
                List<ServerRecord> servers = ServerRecord.GetServers(connection, false);
                if (request.userCheckServer || request.userCheckServerPrivilage)
                {
                    foreach (ServerRecord item in servers)
                    {
                        foreach (ServerDetails serverSelection in request.ServerDetails)
                        {
                            if(serverSelection.serverId == item.SrvId)
                            {
                                ServerRecord server = item;
                                ServerRecord originalServer = server.Clone();

                                // Apply Server Settings
                                if (request.userCheckServer)
                                {
                                    _template.AuditTemplate.ApplyServerSettings(server, request.overwriteSelection);
                                }

                                if (request.userCheckServerPrivilage)
                                {
                                    _template.AuditTemplate.ApplyServerUserSettings(server, request.overwriteSelection);
                                }

                                // Only update the record if they don't match.
                                if (!ServerRecord.Match(originalServer, server))
                                {
                                    server.Connection = connection;
                                    if (server.Write(originalServer))
                                    {
                                        string changeLog = Snapshot.ServerChangeLog(originalServer, server);
                                        // Register change to server and perform audit log				      
                                        RegisterChange(server.SrvId, LogType.ModifyServer, server.Instance, changeLog,connection);
                                        server.ConfigVersion++;
                                    }
                                    else
                                    {
                                
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                if (request.userCheckDatabase || request.usercheckDatabasePrivilage)
                {
                    foreach (ServerDetails serverData in request.ServerDetails)
                    {
                        List<DatabaseRecord> dbs = DatabaseRecord.GetDatabases(connection, serverData.serverId);

                        foreach (DatabaseRecord item in dbs)
                        {
                            foreach (TargetDatabaseDetail selectedDatabase in request.userdbServerComboSelection)
                            {
                                {
                                    if (selectedDatabase.dbName == item.Name)
                                    {
                                        bool dbSuccess = false;
                                        DatabaseRecord database = item;
                                        int index = servers.IndexOf(servers.Single(i => i.SrvId == selectedDatabase.serverId));
                                        ServerRecord server = (ServerRecord)servers[index];
                                        DatabaseRecord originalDb = database.Clone();
                                        Dictionary<string, DBO> originalTables, tables = new Dictionary<string, DBO>();
                                        Dictionary<string, DataChangeTableRecord> originalDataChangeTables, dataChangeTables;
                                        List<SensitiveColumnTableRecord> originalSensitiveColumnTables, sensitiveColumnTables;
                                        var databaseTemplateName = request.usermatchdbNameSelection ? database.Name : request.userdbSelection.First().dbName;

                                        // We must preserve the audited tables to determine if we need to update them
                                        //  after the template is applied
                                        bool auditUserDML = _template.AuditTemplate.CheckAuditUserDML(databaseTemplateName.ToString());

                                        if (auditUserDML && !database.AuditDML)
                                        {
                                            database.AuditUserTables = 2;
                                            database.AuditDmlAll = _template.AuditTemplate.CheckDMLSelectFilter(databaseTemplateName.ToString()); 
                                        }
                                        if (database.AuditDML && database.AuditDmlAll)
                                        {
                                            database.AuditUserTables = 1;
                                        }
                                        

                                        string oldTablesSnapshot = Snapshot.GetDatabaseTables(connection, database.DbId, "\t\t");
                                        if (database.AuditUserTables == 2)
                                        {
                                            originalTables = DBO.GetAuditedTables(connection.ConnectionString, database.DbId);
                                            foreach (string s in originalTables.Keys)
                                                tables.Add(s, originalTables[s]);
                                        }
                                        else
                                            originalTables = new Dictionary<string, DBO>();

                                        // preserve the previously audited BA tables if we need to update them
                                        string oldDcTablesSnapshot = Snapshot.GetDataChangeTables(connection, database.DbId, "\t");
                                        dataChangeTables = new Dictionary<string, DataChangeTableRecord>();
                                        originalDataChangeTables = new Dictionary<string, DataChangeTableRecord>();
                                        if (database.AuditDataChanges)
                                        {
                                            List<DataChangeTableRecord> tableList = DataChangeTableRecord.GetAuditedTables(connection, database.SrvId, database.DbId);
                                            foreach (DataChangeTableRecord dcTable in tableList)
                                            {
                                                dataChangeTables.Add(dcTable.TableName, dcTable);
                                                originalDataChangeTables.Add(dcTable.TableName, dcTable);
                                            }
                                        }

                                        // preserve the previously audited SC tables if we need to update them
                                        string oldSCTablesSnapshot = Snapshot.GetSensitiveColumnTables(connection, database.DbId, "\t");
                                        sensitiveColumnTables = new List<SensitiveColumnTableRecord>();
                                        originalSensitiveColumnTables = new List<SensitiveColumnTableRecord>();
                                        if (database.AuditSensitiveColumns)
                                        {
                                            List<SensitiveColumnTableRecord> tableList = SensitiveColumnTableRecord.GetAuditedTables(connection, database.SrvId, database.DbId);
                                            if (tableList != null)
                                            {
                                                foreach (SensitiveColumnTableRecord scTable in tableList)
                                                {
                                                    sensitiveColumnTables.Add(scTable);
                                                    originalSensitiveColumnTables.Add(scTable);
                                                }
                                            }
                                        }

                                        // apply the template

                                        if (request.userCheckDatabase)
                                        {
                                            _template.AuditTemplate.ApplyDatabaseSettings(databaseTemplateName,
                                                                                          server,
                                                                                          database,
                                                                                          tables,
                                                                                      dataChangeTables,
                                                                                      sensitiveColumnTables,
                                                                                      request.overwriteSelection,
                                                                                      serverRepositoryName,
                                                                                      serverPort);
                                        }

                                        if (request.usercheckDatabasePrivilage)
                                        {
                                            _template.AuditTemplate.ApplyDatabasePrivilegedUserSettings(databaseTemplateName, database, request.overwriteSelection);
                                        }

                                        database.AuditDataChanges = (dataChangeTables.Count > 0);
                                        database.AuditSensitiveColumns = (sensitiveColumnTables.Count > 0);

                                        // Write updates as needed
                                        bool tableUpdateNeeded = false;
                                        bool dcTableUpdateNeeded = false;
                                        bool scTableUpdateNeeded = false;
                                        bool dbChangesMade = false;
                                        database.Connection = connection;
                                        using (SqlTransaction t = connection.BeginTransaction())
                                        {
                                            try
                                            {
                                                if (!DatabaseRecord.Match(originalDb, database))
                                                {
                                                    if (!database.Write(originalDb, t))
                                                        throw new Exception(DatabaseRecord.GetLastError());
                                                    dbChangesMade = true;
                                                }

                                                if (tables.Count == originalTables.Count)
                                                {
                                                    foreach (string s in tables.Keys)
                                                        if (!originalTables.ContainsKey(s))
                                                            tableUpdateNeeded = true;
                                                }
                                                else
                                                    tableUpdateNeeded = true;
                                                // We need to update audited tables
                                                //  Lazy way - drop then readd
                                                List<DBO> newAuditTables = new List<DBO>();
                                                if (tables.Count > 0 && originalTables.Count > 0)
                                                {
                                                    List<DBO> newTables = new List<DBO>(tables.Values);
                                                    List<DBO> originalTable = new List<DBO>(originalTables.Values);
                                                    foreach (DBO s in tables.Values)
                                                    {
                                                        if (!originalTables.ContainsKey(s.FullName))
                                                        {
                                                            newAuditTables.Add(s);
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    newAuditTables = new List<DBO>(tables.Values);
                                                }
                                                if (tableUpdateNeeded)
                                                {
                                                    DBO.UpdateUserTables(database.Connection,
                                                                         newAuditTables, originalTables.Count, database.DbId, t, request.overwriteSelection);
                                                }

                                                if (dataChangeTables.Count == originalDataChangeTables.Count)
                                                {
                                                    foreach (string s in dataChangeTables.Keys)
                                                        if (!originalDataChangeTables.ContainsKey(s))
                                                            dcTableUpdateNeeded = true;
                                                }
                                                else
                                                    dcTableUpdateNeeded = true;
                                                if (dcTableUpdateNeeded)
                                                {
                                                    List<DataChangeTableRecord> tableList = new List<DataChangeTableRecord>();
                                                    tableList.AddRange(dataChangeTables.Values);
                                                    DataChangeTableRecord.UpdateUserTables(connection, tableList, database.SrvId, database.DbId, t);
                                                }

                                                if (sensitiveColumnTables.Count == originalSensitiveColumnTables.Count)
                                                {
                                                    foreach (SensitiveColumnTableRecord record in sensitiveColumnTables)
                                                        if (!originalSensitiveColumnTables.Contains(record))
                                                            scTableUpdateNeeded = true;
                                                }
                                                else
                                                    scTableUpdateNeeded = true;

                                                if (scTableUpdateNeeded)
                                                {
                                                    List<SensitiveColumnTableRecord> sensitiveColumns = new List<SensitiveColumnTableRecord>();
                                                    sensitiveColumns.AddRange(sensitiveColumnTables);
                                                    SqlDirect sqlDirect = SqlDirect.OpenDirectConnection(database.SrvInstance);
                                                    SensitiveColumnTableRecord.UpdateUserTables(connection, sensitiveColumns, database.SrvId, database.DbId, t, true, database.SrvInstance, database.Name, sqlDirect.Connection);
                                                }

                                                t.Commit();
                                                dbSuccess = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                t.Rollback();
                                                //ErrorMessage.Show(this.Text, UIConstants.Error_ErrorSavingDatabase, ex.Message);
                                            }
                                            finally
                                            {
                                                // Only write changelog if changes were made and successfully written
                                                if ((dbChangesMade || tableUpdateNeeded) && dbSuccess)
                                                {
                                                    string changeLog = Snapshot.DatabaseChangeLog(connection,
                                                                                                  originalDb,
                                                                                                  database,
                                                                                                  oldTablesSnapshot,
                                                                                                  oldDcTablesSnapshot,
                                                                                                  oldSCTablesSnapshot);

                                                    // Register change to server and perform audit log				      
                                                    RegisterChange(database.SrvId, LogType.ModifyDatabase, database.SrvInstance, changeLog, connection);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
              
            }
            catch (Exception e)
            {               
     
            }
        }


         public void  RegisterChange(
		      int               serverId,
		      LogType           logType,
		      string            serverName,
		      string            info,
              SqlConnection connection
		   )
		{
            // bump server version number so that agent will synch up	      
	         ServerRecord.IncrementServerConfigVersion( connection,
	                                                    serverId );
      	   
	         // Log update
	         LogRecord.WriteLog( connection,
                                logType,
                                serverName,
                                info );
                                 
           // inform agent of change?                                 
      }
        //SQLCM 5.4 SCM-9 END

         public bool IsLinqDllLoaded(SqlConnection connection)
        {
            try
            {
                string instanceName = connection.DataSource;
                instanceName = instanceName.ToUpper().Replace(".", Environment.MachineName).Replace("(LOCAL)", Environment.MachineName);
                if (instanceName.StartsWith(Environment.MachineName))
                {
                    ActiveQueryCollector activeQueryCollector = new ActiveQueryCollector();
                    return activeQueryCollector.IsLinqAssemblyLoaded();
                }
                else
                {
                    int dllVersion = 0;
                    // SQLCM 5.6- 566/740/4620/5280 (Non-Admin and Non-Sysadmin role) Permissions
                    // Allow reading Linq Check from Cached tables for non sysadmins
                    var dllName = string.Empty;
                    using (connection)
                    {
                        var isSysAdmin = RawSQL.IsCurrentUserSysadmin(connection);
                        // Read from the table
                        if (!isSysAdmin)
                        {
                            var dllMap = new Dictionary<string, int>();
                            using (var command = new SqlCommand(SqlCmStartupDllChecker, connection))
                            {
                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.HasRows) {
                                        while (reader.Read()) {
                                            var dllNameIndex = reader.GetOrdinal("DllName");
                                            var dllVersionIndex = reader.GetOrdinal("DllVersion");
                                            if (dllNameIndex != -1 && dllVersionIndex != -1)
                                            {
                                                if (!reader.IsDBNull(dllNameIndex) && !reader.IsDBNull(dllVersionIndex))
                                                {
                                                    dllName = reader.GetString(dllNameIndex);
                                                    dllVersion = reader.GetInt32(dllVersionIndex);
                                                }

                                                // Add to Dll Map
                                                if (!dllMap.ContainsKey(dllName))
                                                {
                                                    dllMap.Add(dllName, dllVersion);
                                                }
                                                else
                                                {
                                                    dllMap[dllName] = dllVersion;
                                                }
                                            }
                                        }
                                    }
                                }
                                return (dllMap.ContainsKey(XeLinqDll) && dllMap[XeLinqDll] <= 110) 
                                        || (dllMap.ContainsKey(XeCoreDll) && dllMap[XeCoreDll] != 0);
                            }
                        }
                        else
                        {
                            using (var command = new SqlCommand("[dbo].[sp_sqlcm_LinkDllCheck]", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.Add("@dllFile", SqlDbType.NVarChar).Value = XeLinqDll;
                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        dllVersion = reader.GetInt32(0);
                                    }
                                }
                            }
                            if (dllVersion <= 0)
                                return false;
                            else if (dllVersion <= 110)
                                return true;
                            else
                            {
                                using (var command = new SqlCommand("[dbo].[sp_sqlcm_LinkDllCheck]", connection))
                                {
                                    dllVersion = 0;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.Add("@dllFile", SqlDbType.NVarChar).Value = XeCoreDll;
                                    using (var reader = command.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            dllVersion = reader.GetInt32(0);
                                        }
                                        if (dllVersion != 0)
                                            return true;
                                        else
                                            return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
         ///<Export Database Audit XML File>
         /// Rest Services Method to export the Database Audit Setting xml file.
         ///</ExportAudit>
         ///
         public List<string> ExportDatabaseRegulationAuditSettingsAction(SqlConnection connection, List<AuditedDatabaseInfo> databases)
         {
             List<string> fileNamelist = new List<string>();
             DatabaseRecord  dr =  new DatabaseRecord();
             foreach (AuditedDatabaseInfo item in databases)
             {
                 var db = SqlCmRecordReader.GetDatabaseRecordByName(item.ServerId, item.Name, connection);
                 string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                 string fileNameOnly = String.Format("{0}_{1}_AuditSettings", db.SrvInstance, db.Name).Replace('\\', '_');
                 string extension = ".xml";
                 String exportPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\" + fileNameOnly + extension;
                 if (File.Exists(exportPath))
                 {
                     File.Delete(exportPath);
                 }
                 InstanceTemplate tmpl = new InstanceTemplate();
                 tmpl.ExportDatabaseAuditSettingsCwf(connection, db);
                 tmpl.Save(exportPath);
                 fileNamelist.Add(exportPath);
             }
             return fileNamelist;
         }

         internal List<string> ExportServerRegulationAuditSettingsAction(SqlConnection connection, string server)
         {
             List<string> fileNamelist = new List<string>();
             if (server != null)
             {
                 string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                 string fileNameOnly = String.Format("{0}_AuditSettings", server).Replace('\\', '_');
                 string extension = ".xml";
                 String exportPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\" + fileNameOnly + extension;
                 while (File.Exists(exportPath))
                 {
                     File.Delete(exportPath);
                 }
                 InstanceTemplate tmpl = new InstanceTemplate();
                 tmpl.RepositoryServer = server;
                 tmpl.ImportAuditSettingsCwf(connection, server);
                 tmpl.Save(exportPath);
                 fileNamelist.Add(exportPath);
             }
             return fileNamelist;
         }
   }
 }