using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.AddDatabase;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Archives;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.DatabaseProperties;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.RegulationSettings;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1.Stats;
using Idera.SQLcompliance.Core.Stats;

namespace SQLcomplianceCwfAddin.RestService.ServiceContracts.v1.Interfaces
{
    [ServiceContract]
    public interface IDatabasesService
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditedDatabasesForInstance?parentId={parentId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get instance databases.")]
        List<AuditedDatabaseInfo> GetAuditedDatabasesForInstance(int parentId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetEventsDatabasesForInstance?parentId={parentId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get databases from Events table.")]
        List<string> GetEventsDatabasesForInstance(int parentId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetNotRegisteredDatabasesForInstance?parentId={parentId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get instance databases.")]
        List<AuditedDatabaseInfo> GetNotRegisteredDatabasesForInstance(int parentId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetDatabasesForArchiveAttachment?parentId={parentId}&showAll={showAll}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get instance databases.")]
        List<AuditedDatabaseInfo> GetDatabasesForArchiveAttachment(int parentId, bool showAll);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetDatabaseAvailabilityGroups", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get availability groups for database list.")]
        List<AvailabilityGroup> GetDatabaseAvailabilityGroups(List<AuditedDatabaseInfo> databaseList);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetServerRoleUsersForInstance?serverId={serverId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get instance server roles and logins.")]
        ServerRolesAndUsers GetServerRoleUsersForInstance(int serverId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/VerifyPermissions?serverId={serverId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Run permission checks for server and return their status info.")]
        PermissionChecksStatus VerifyPermissions(int serverId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetDatabaseTableList", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        [Description("Get table list by filtering by server id, database name and table name.")]
        List<DatabaseObject> GetDatabaseTableList(DatabaseTableFilter filter);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/AddDatabases", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Register all databases from list.")]
        void AddDatabases(AuditDatabaseSettings auditSeetings);

        //Regulation Guidelines Template settings
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRegulationSettingsForDatabase", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get all databases templates regulation settings from list.")]
        AuditActivity GetRegulationSettingsForDatabase(AuditRegulationSettings auditSettings);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/RemoveDatabase", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Remove database by id.")]
        bool RemoveDatabase(RemoveDatabaseRequest removeDatabaseRequest);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/EnableAuditingForDatabases", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable or disable auditing for databases.")]
        void EnableAuditingForDatabases(EnableAuditForDatabases databases);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAuditedActivityForDatabase?instanceId={serverId}&databaseId={databaseId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get audited activity for database in instance.")]
        AuditedDatabaseActivityResult GetAuditedActivityForDatabase(int serverId, int databaseId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetRecentDatabaseActivity?instanceId={serverId}&databaseId={databaseId}&days={days}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get recent activity for database in instance.")]
        List<KeyValuePair<RestStatsCategory, List<RestStatsData>>> GetRecentDatabaseActivity(int serverId, int databaseId, int days);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAuditDatabaseProperties?databaseId={databaseId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get instance databases.")]
        AuditDatabaseProperties GetAuditDatabaseProperties(int databaseId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/UpdateAuditDatabaseProperties", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Update audit database properties.")]
        string UpdateAuditDatabaseProperties(AuditDatabaseProperties databaseProperties);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetServerClrStatus?serverId={serverId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Server Clr status.")]
        ClrStatus GetServerClrStatus(int serverId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/EnableClr", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Enable Clr for server.")]
        ClrStatus EnableClr(ClrStatus enableStatus);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetColumnList?databaseId={databaseId}&tableName={tableName}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Server Clr status.")]
        List<string> GetColumnList(int databaseId, string tableName);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetNewDatabaseColumnList?tableName={tableName}&instance={instanceName}&databaseName={databaseName}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get New Database Column List")]
        List<string> GetNewDatabaseColumnList(string tableName, string instanceName, string databaseName);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetRegulationTypeList", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Regulation Type List.")]
        List<RestRegulation> GetRegulationTypeList();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetRegulationSectionDictionary", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Regulation Section Dictionary.")]
        Dictionary<string, List<RestRegulationSection>> GetRegulationSectionDictionary();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/IsUpgradeRequiredForEventDatabase", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Check if update is required for event database.")]
        bool IsUpgradeRequiredForEventDatabase(ArchivePropertiesRequest archive);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/UpgradeEventDatabaseSchema", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Update event database schema.")]
        void UpgradeEventDatabaseSchema(string archive);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateIndexesForEventDatabase", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Updates indexes for event database.")]
        void UpdateIndexesForEventDatabase(string databaseName);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetAllDatabasesForInstance?parentId={parentId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get instance databases.")]
        List<AuditedDatabaseInfo> GetAllDatabasesForInstance(int parentId);
        //SQLCM 5.4 start
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetTableDetailsForAll", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        [Description("Get table list by filtering by server id and database name.")]
        List<DatabaseTableSummary> GetTableDetailsForAll(DatabaseTableDetailsForAllFilter filter);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetColumnDetailsForAll", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        [Description("Get table list by filtering by server id and database name.")]
        List<ColumnTableSummary> GetColumnDetailsForAll(DatabaseTableDetailsForAllFilter filter);
		
		[WebInvoke(Method = "GET", UriTemplate = "/GetProfileDetails", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get column search profile.")]
        List<ProfilerObject> GetProfileDetails();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/GetActiveProfile", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get active column search profile.")]
        string GetActiveProfile();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteString", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Delete string from all profiles")]
        void DeleteString(ProfilerObject selectedList);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/InsertString", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Insert string from all profiles")]
        void InsertString(ProfilerObject newString);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/UpdateString", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Update edited string")]
        void UpdateString(List<ProfilerObject> updateStrings);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/ActivateProfile", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Activates a profile")]
        void ActivateProfile(string profileName);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/ResetData", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Restore default settings")]
        void ResetData();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/InsertNewProfile", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Insert new profile")]
        void InsertNewProfile(List<ProfilerObject> newProfileDetails);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/UpdateCurrentProfile", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Update current profile")]
        void UpdateCurrentProfile(List<ProfilerObject> updatedProfileDetails);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/DeleteProfile", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Delete profile")]
        void DeleteProfile(string profileName);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/UpdateIsUpdated", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Is Updated")]
        void UpdateIsUpdated(string value);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(UriTemplate = "/GetIsUpdated", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Is Updated")]
        string GetIsUpdated();
		
		[WebInvoke(Method = "POST", UriTemplate = "/ExportDatabaseAuditSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Export Database Audit Settings")]
        string ExportDatabaseAuditSettings(int request);

        [WebInvoke(Method = "POST", UriTemplate = "/ExportDatabasesRegulationAuditSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Export Database Regulation Audit Settings")]
        List<string> ExportDatabasesRegulationAuditSettings(List<AuditedDatabaseInfo> request);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/ImportDatabaseAuditSetting", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("import Database Audit Settings")]
        void ImportDatabaseAuditSetting(AllImportSettingDetails request);
        //SQLCM 5.4 end

        //Regulation Guidelines Template settings
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRegulationSettingsServerLevel", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get all databases templates regulation settings from list.")]
        RegulationCustomDetail GetRegulationSettingsForServer(AuditRegulationSettings auditSettings);
    }
}
