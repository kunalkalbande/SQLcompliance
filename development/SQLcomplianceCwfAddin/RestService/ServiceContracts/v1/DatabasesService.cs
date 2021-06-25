using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Idera.SQLcompliance.Core;
using Idera.SQLcompliance.Core.Rules;
using Idera.SQLcompliance.Core.Rules.Alerts;
using Idera.SQLcompliance.Core.Rules.Filters;
using Idera.SQLcompliance.Core.Stats;
using SQLcomplianceCwfAddin.Helpers;
using SQLcomplianceCwfAddin.Helpers.Agent;
using SQLcomplianceCwfAddin.Helpers.Regulations;
using SQLcomplianceCwfAddin.Helpers.SQL;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.RegulationSettings;
using System;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using SQLcomplianceCwfAddin.Helpers.Stats;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1
{
    public partial class RestService
    {
        public List<AuditedDatabaseInfo> GetAuditedDatabasesForInstance(int parentId)
        {
            using (_logger.InfoCall("GetAuditedDatabasesForInstace"))
            {
                using (var connection = GetConnection())
                {
                var result = AuditDabaseHelper.Instance.GetAllDatabasesForInstance(parentId, connection);
                return result;
                }
            }
        }

        public List<String> GetEventsDatabasesForInstance(int parentId)
        {
            using (_logger.InfoCall("GetEventsDatabasesForInstance"))
            {
                using (var connection = GetConnection())
                {
                    var result = AuditDabaseHelper.Instance.GetEventsDatabasesForInstance(parentId, connection);
                    return result;
                }
            }
        }

        public List<AuditedDatabaseInfo> GetNotRegisteredDatabasesForInstance(int parentId)
        {
            using (_logger.InfoCall("GetNotRegisteredDatabasesForInstance"))
            {
                using (var connection = GetConnection())
                {
                var result = AuditDabaseHelper.Instance.GetNotRegisteredDatabasesForInstance(parentId, connection);
                return result;
            }
        }
        }

        public List<AuditedDatabaseInfo> GetDatabasesForArchiveAttachment(int parentId, bool showAll)
        {
            List<AuditedDatabaseInfo> databases = new List<AuditedDatabaseInfo>();

            using (_logger.InfoCall("GetNotRegisteredDatabasesForInstance"))
            {
                using (var connection = GetConnection())
                {
                    databases = AuditDabaseHelper.Instance.GetDatabasesForRepositoryInstace(connection);
                   
                    if (!showAll)
                    {
                        SQLcomplianceConfiguration configuration = SqlCmConfigurationHelper.GetConfiguration(connection);
                        databases =
                            databases.Where(db => db.Name.ToUpper().StartsWith(configuration.ArchivePrefix.ToUpper()))
                                .ToList();
                    }

                return databases;
            }
        }
        }

        public List<AvailabilityGroup> GetDatabaseAvailabilityGroups(List<AuditedDatabaseInfo> databaseList)
        {
            List<AvailabilityGroup> availabilityGroupList = new List<AvailabilityGroup>();
            using (_logger.InfoCall("GetAvailabilityGroupsForDatabases"))
            {
                using (var connection = GetConnection())
                {
                    try
                    {
                        availabilityGroupList = AgentManagerHelper.Instance.GetAvailabilityGroupList(databaseList, connection);
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(string.Format("Failed to get availability groups for databases due to error {0}", ex));
                }
            }

                return availabilityGroupList;
            }
        }

        public ServerRolesAndUsers GetServerRoleUsersForInstance(int serverId)
        {
            using (_logger.InfoCall("GetTrustedServerRoleUsersForInstance"))
            {
                using (var connection = GetConnection())
                {
                    ServerRolesAndUsers rolesAndUsers = AgentManagerHelper.Instance.GetServerRolesAndUsers(connection, serverId);
                    return rolesAndUsers;
                }
            }
        }

        public PermissionChecksStatus VerifyPermissions(int serverId)
        {
            using (_logger.InfoCall("VerifyPermissions"))
            {
                using (var connection = GetConnection())
                {
                    var checksStatus = PermissionCheckHelper.Instance.VerifyPermissions(serverId, connection);
                    return checksStatus;
                }
            }
        }

        public List<DatabaseObject> GetDatabaseTableList(DatabaseTableFilter filter)
        {
            using (_logger.InfoCall("GetDatabaseTableList"))
            {
                using (var connection = GetConnection())
                {
                    var tableList = AgentManagerHelper.Instance.GetDatabaseTableList(filter, connection);
                    return tableList;
                }
            }
        }

        public void AddDatabases(AuditDatabaseSettings auditSeetings)
        {
            using (_logger.InfoCall("AddDatabases"))
            {
                using (var connection = GetConnection())
                {
                    AuditDabaseHelper.Instance.AddDatabases(auditSeetings, connection);
                }
            }
        }

        //Regulation Guidelines Template Settings
        public AuditActivity GetRegulationSettingsForDatabase(AuditRegulationSettings auditSettings)
        {
            using (_logger.InfoCall("GetRegulationSettingsForDatabase"))
            {
                using (var connection = GetConnection())
                {
                    var result = AuditDabaseHelper.Instance.GetRegulationTemplatesForDatabase(auditSettings, connection);
                    return result;
                }
            }
        }

        public bool RemoveDatabase(RemoveDatabaseRequest removeDatabaseRequest)
        {
            using (_logger.InfoCall("RemoveDatabase"))
            {
                using (var connection = GetConnection())
                {
                    var wasRemoved = AuditDabaseHelper.Instance.RemoveDatabase(removeDatabaseRequest, connection);
                    return wasRemoved;
                }
            }
        }

        public void EnableAuditingForDatabases(EnableAuditForDatabases databases)
        {
            using (_logger.InfoCall("EnableAuditingForDatabase"))
            {
                using (var connection = GetConnection())
                {
                    AuditDabaseHelper.Instance.EnableAuditingForDatabases(databases, connection);
                }
            }
        }

        public List<KeyValuePair<RestStatsCategory, List<RestStatsData>>> GetRecentDatabaseActivity(int serverId, int databaseId, int days)
        {
            using (_logger.InfoCall("GetRecentDatabaseActivity"))
            {
                var categories = new List<RestStatsCategory>();
                categories.Add(RestStatsCategory.Admin);
                categories.Add(RestStatsCategory.Ddl);
                categories.Add(RestStatsCategory.Security);
                categories.Add(RestStatsCategory.Dml);
                categories.Add(RestStatsCategory.Select);

                using (var connection = GetConnection())
                {
                    return StatsAggregator.GetDatabaseActivityStatsDataInternal(serverId, databaseId, days, categories, connection);
                }
            }
        }

        public AuditedDatabaseActivityResult GetAuditedActivityForDatabase(int serverId, int databaseId)
        {
            using (_logger.InfoCall("GetAuditedActivityForDatabase"))
            {
                using (var connection = GetConnection())
                {
                    var result = AuditDabaseHelper.Instance.GetAuditedActivityForDatabase(databaseId, serverId, connection);
                    return result;
                }
            }
        }

        public AuditDatabaseProperties GetAuditDatabaseProperties(int databaseId)
        {
            using (_logger.InfoCall("GetAuditDatabaseProperties"))
            {
                using (var connection = GetConnection())
                {
                var databaseProperties = AuditDabaseHelper.Instance.GetAuditDatabaseProperties(databaseId, connection);
                return databaseProperties;
            }
        }
        }

        public string UpdateAuditDatabaseProperties(AuditDatabaseProperties databaseProperties)
        {
            using (_logger.InfoCall("UpdateAuditDatabaseProperties"))
            {
                using (var connection = GetConnection())
                {
                var errorMessage = AuditDabaseHelper.Instance.UpdateAuditDatabaseProperties(databaseProperties, connection);
                return errorMessage;
            }   
        }
        }

        public ClrStatus GetServerClrStatus(int serverId)
        {
            using (_logger.InfoCall("GetServerClrStatus"))
            {
                using (var connection = GetConnection())
                {
                var clrStatus = AgentManagerHelper.Instance.GetServerClrStatus(serverId, connection);
                return clrStatus;
            }
        }
        }

        public ClrStatus EnableClr(ClrStatus enableStatus)
        {
            using (_logger.InfoCall("EnableClr"))
            {
                using (var connection = GetConnection())
                {
                var errorMessage = AgentManagerHelper.Instance.EnableClr(enableStatus, connection);
                return errorMessage;
            }
        }
        }

        public List<string> GetColumnList(int databaseId, string tableName)
        {
            using (_logger.InfoCall("GetColumnList"))
            {
                using (var connection = GetConnection())
                {
                    var columnList = AgentManagerHelper.Instance.GetColumnList(databaseId, tableName, connection);
                    return columnList;
                }
            }            
        }

        public List<string> GetNewDatabaseColumnList(string tableName, string instanceName, string databaseName)
        {
            using (_logger.InfoCall("GetNewDatabaseColumnList"))
            {
                using (var connection = GetConnection())
                {
                    var columnList = AgentManagerHelper.Instance.GetColumnList(-1, tableName,connection, instanceName, databaseName);
                    return columnList;
                }
            }
        }

        public List<RestRegulation> GetRegulationTypeList()
        {
            using (_logger.InfoCall("GetRegulationTypeList"))
            {
                using (var connection = GetConnection())
                {
                    var regulationTypeList = RegulationSettingsHelper.Instance.GetRegulationTypeList(connection);
                    return regulationTypeList;
                }
            }
        }

        public Dictionary<string, List<RestRegulationSection>> GetRegulationSectionDictionary()
        {
            using (_logger.InfoCall("GetRegulationSectionDictionary"))
            {
                using (var connection = GetConnection())
                {
                    var dictionary = RegulationSettingsHelper.Instance.GetRegulationSectionDictionary(connection);
                    return dictionary;
                }
            }
        }

        public bool IsUpgradeRequiredForEventDatabase(ArchivePropertiesRequest archive)
        {
            using (_logger.InfoCall("IsUpgradeRequiredForEventDatabase"))
            {
                var properties = GetArchiveProperties(archive);

                return properties.IsCompatibleSchema;
            }
        }

        public void UpgradeEventDatabaseSchema(string archive)
        {
            using (_logger.InfoCall("IsUpgradeRequiredForEventDatabase"))
            {
                using (var connection = GetConnection())
                {
                    EventDatabase.UpgradeEventDatabase(connection, archive);
                }
            }
        }

        public void UpdateIndexesForEventDatabase(string databaseName)
        {
            using (_logger.InfoCall("UpdateIndexesForEventDatabase"))
            {
                using (var connection = GetConnection())
                {
                    EventDatabase.UpdateIndexes(connection, databaseName);
                }
            }
        }

        //SQLCM 5.4 Start
        public List<AuditedDatabaseInfo> GetAllDatabasesForInstance(int parentId)
        {
            using (_logger.InfoCall("GetAllDatabasesForInstance"))
            {
                using (var connection = GetConnection())
                {
                    var result = AgentManagerHelper.Instance.GetDatabaseDetailsForAll(connection, parentId);
                    return result;
                }
            }
        }

        public List<DatabaseTableSummary> GetTableDetailsForAll(DatabaseTableDetailsForAllFilter filter)
        {
            using (_logger.InfoCall("GetTableDetailsForAll"))
            {
                using (var connection = GetConnection())
                {
                    var tableList = AgentManagerHelper.Instance.GetTableDetailsForAll(filter, connection);

                    //AgentManagerHelper.Instance.GetDatabaseTableList(filter, connection);
                    return tableList;
                }
            }
        }

        public List<ColumnTableSummary> GetColumnDetailsForAll(DatabaseTableDetailsForAllFilter filter)
        {
            using (_logger.InfoCall("GetColumnDetailsForAll"))
            {
                using (var connection = GetConnection())
                {
                    var tableList = AgentManagerHelper.Instance.GetColumnDetailsForAll(filter, connection);

                    //AgentManagerHelper.Instance.GetDatabaseTableList(filter, connection);
                    return tableList;
                }
            }
        }
		
		public List<ProfilerObject> GetProfileDetails()
        {
            using (_logger.InfoCall("GetProfileDetails"))
            {
                using (var connection = GetConnection())
                {
                    var tableList = ProfilerObject.GetProfileDetails(connection);
                    return tableList;
                }
            }
        }

        public string GetActiveProfile()
        {
            using (_logger.InfoCall("GetActiveProfile"))
            {
                using (var connection = GetConnection())
                {
                    var tableList = ProfilerObject.GetActiveProfile(connection);
                    return tableList;
                }
            }
        }

        public void DeleteString(ProfilerObject selectedList)
        {
            using (_logger.InfoCall("DeleteString"))
            {
                using (var connection = GetConnection())
                {
                    ProfilerObject profiler = new ProfilerObject();
                    profiler.DeleteString(connection, selectedList);
                }
            }
        }

        public void InsertString(ProfilerObject newString)
        {
            using (_logger.InfoCall("InsertString"))
            {
                using (var connection = GetConnection())
                {
                    ProfilerObject profiler = new ProfilerObject();
                    profiler.InsertString(connection, newString);
                }
            }
        }

        public void UpdateString(List<ProfilerObject> updateStrings)
        {
            using (_logger.InfoCall("UpdateString"))
            {
                using (var connection = GetConnection())
                {
                    ProfilerObject profiler = new ProfilerObject();
                    profiler.UpdateString(connection, updateStrings);
                }
            }
        }

        public void ActivateProfile(string profileName)
        {
            using (_logger.InfoCall("ActivateProfile"))
            {
                using (var connection = GetConnection())
                {
                    ProfilerObject profiler = new ProfilerObject();
                    profiler.ActivateProfile(connection, profileName);
                }
            }
        }

        public void ResetData()
        {
            using (_logger.InfoCall("ResetData"))
            {
                using (var connection = GetConnection())
                {
                    ProfilerObject profiler = new ProfilerObject();
                    profiler.ResetData(connection);
                }
            }
        }

        public void InsertNewProfile(List<ProfilerObject> newProfileDetails)
        {
            using (_logger.InfoCall("InsertNewProfile"))
            {
                using (var connection = GetConnection())
                {
                    ProfilerObject profiler = new ProfilerObject();
                    profiler.InsertNewProfile(connection, newProfileDetails);
                }
            }
        }

        public void DeleteProfile(string profileName)
        {
            using (_logger.InfoCall("DeleteProfile"))
            {
                using (var connection = GetConnection())
                {
                    ProfilerObject profiler = new ProfilerObject();
                    profiler.DeleteProfile(connection, profileName);
                }
            }
        }

        public void UpdateCurrentProfile(List<ProfilerObject> updatedProfileDetails)
        {
            using (_logger.InfoCall("UpdateCurrentProfile"))
            {
                using (var connection = GetConnection())
                {
                    ProfilerObject profiler = new ProfilerObject();
                    profiler.UpdateCurrentProfile(connection, updatedProfileDetails);
                }
            }
        }

        public void UpdateIsUpdated(string value)
        {
            using (_logger.InfoCall("UpdateIsUpdated"))
            {
                using (var connection = GetConnection())
                {
                    ProfilerObject profiler = new ProfilerObject();
                    profiler.UpdateIsUpdated(connection, value);
                }
            }
        }

        public string GetIsUpdated()
        {
            using (_logger.InfoCall("GetIsUpdated"))
            {
                using (var connection = GetConnection())
                {
                    ProfilerObject profiler = new ProfilerObject();
                    var isUpdated = profiler.GetIsUpdated(connection);
                    return isUpdated;
                }
            }
        }
		
		public string ExportDatabaseAuditSettings(int request)
        {
            using (_logger.InfoCall("Export Database Audit Settings"))
            {
                var result = QueryExecutor.Instance.ExportDatabaseAuditSettingsAction(GetConnection(), request);
                return result;
            }
        }

        public List<string> ExportDatabasesRegulationAuditSettings(List<AuditedDatabaseInfo> request)
        {
            using (_logger.InfoCall("Export Database Regulation Audit Settings"))
            {
                var result = QueryExecutor.Instance.ExportDatabaseRegulationAuditSettingsAction(GetConnection(), request);
                return result;
            }
        }

        public void ImportDatabaseAuditSetting(AllImportSettingDetails request)
        {
            using (_logger.InfoCall("Export Database Audit Settings"))
            {
                QueryExecutor.Instance.ImportDatabaseAuditSetting(GetConnection(), request);
            }
        }

        //SQLCM 5.4 End
        public RegulationCustomDetail GetRegulationSettingsForServer(AuditRegulationSettings auditSettings)
        {
            using (_logger.InfoCall("GetRegulationSettingsForDatabase"))
            {
                using (var connection = GetConnection())
                {
                    RegulationCustomDetail regulationCustomDetail = new RegulationCustomDetail();
                    regulationCustomDetail.AuditedDatabaseActivities = AuditDabaseHelper.Instance.GetRegulationTemplatesForDatabase(auditSettings, connection);
                    regulationCustomDetail.AuditedServerActivities = AuditServerHelper.Instance.GetRegulationTemplatesForServer(auditSettings, connection);
                    return regulationCustomDetail;
                }
            }
        }
    }
}
