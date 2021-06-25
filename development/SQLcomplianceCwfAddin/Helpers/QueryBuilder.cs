using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLcompliance.Core.Event;
using SQLcomplianceCwfAddin.RestService.DataContracts.v1;

namespace SQLcomplianceCwfAddin.Helpers
{
    internal class QueryBuilder
    {
        #region members

        private Dictionary<string,string> _queries;
        private static QueryBuilder _instance;

        #endregion

        #region constructor\destructor

        private QueryBuilder()
        {
            _queries = new Dictionary<string, string>();

            var sqlBuilder = new StringBuilder();
            var tempSqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine(" SELECT [srvId] AS 'Id'");
            sqlBuilder.AppendLine(" 	  ,-1 AS 'SystemId'");
            sqlBuilder.AppendLine("       ,[instance] AS 'Name'");
            sqlBuilder.AppendLine(" 	  , 1 AS 'Type'");
            sqlBuilder.AppendLine("       ,[description] AS 'Description'");
            sqlBuilder.AppendLine("       ,[isEnabled] AS 'IsEnabled'");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[Servers]");
            _queries.Add("GetEnvironmentObjectHierarchy_Root", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT  [dbId] AS 'Id'");
            sqlBuilder.AppendLine(" 		,[sqlDatabaseId] AS 'SystemId'");
            sqlBuilder.AppendLine(" 		,[name] AS 'Name'");
            sqlBuilder.AppendLine(" 		,2 AS 'Type'");
            sqlBuilder.AppendLine(" 		,[description] AS 'Description'");
            sqlBuilder.AppendLine("        ,[isEnabled] AS 'IsEnabled'");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[Databases] ");
            sqlBuilder.AppendLine(" WHERE ");
            sqlBuilder.AppendLine(" 	[srvId] = {0}");
            _queries.Add("GetEnvironmentObjectHierarchy_Server", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetCmLicenses]");
            _queries.Add("GetCmLicenses", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT [Timeout],[Filter], [ViewId], [UserId]");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[ViewSettings] {0}");
            _queries.Add("GetViewSettings", sqlBuilder.ToString());
            sqlBuilder.Clear();

            
            sqlBuilder.AppendLine(" SELECT [agentServer],[agentPort]");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[Servers] WHERE instance = '{0}'");
            _queries.Add("GetAgentConnectionDetails", sqlBuilder.ToString());
            sqlBuilder.Clear();
            
            sqlBuilder.AppendLine(" UPDATE [SQLcompliance]..[Alerts]");
            sqlBuilder.AppendLine("       SET isDismissed = 1");
            sqlBuilder.AppendLine(" WHERE [alertId] = {0}");
            _queries.Add("DismissAlert", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" UPDATE [SQLcompliance]..[Alerts]");
            sqlBuilder.AppendLine("       SET isDismissed = 1");
            sqlBuilder.AppendLine(" WHERE [alertId] IN ({0}) ");
            _queries.Add("DismissAlertList", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT e.eventId,e.eventType,e.eventCategory,e.targetObject,e.details,e.hash,");
            sqlBuilder.AppendLine("       e.eventClass,e.eventSubclass,e.startTime,e.spid,e.applicationName,e.hostName,");
            sqlBuilder.AppendLine("       e.serverName,e.loginName,e.success,e.databaseName,e.databaseId,e.dbUserName,");
            sqlBuilder.AppendLine("       e.objectType,e.objectName,e.objectId,e.permissions,e.columnPermissions,e.targetLoginName,");
            sqlBuilder.AppendLine("       e.targetUserName,e.roleName,e.ownerName,e.alertLevel,e.checksum,e.privilegedUser,t.name,");
            sqlBuilder.AppendLine("       t.category,e.fileName,e.linkedServerName,e.parentName,e.isSystem,e.sessionLoginName,");
            sqlBuilder.AppendLine("       e.providerName,e.appNameId,e.hostId,e.loginId,e.endTime,e.startSequence,e.endSequence, e.rowCounts, esql.sqlText");
            sqlBuilder.AppendLine("       FROM [{0}]..Events AS e");
            sqlBuilder.AppendLine("       LEFT OUTER JOIN SQLcompliance..EventTypes t ON e.eventType=t.evtypeid");
            sqlBuilder.AppendLine("       LEFT OUTER JOIN [{0}]..[EventSQL] esql ON esql.eventId=e.eventId");
            sqlBuilder.AppendLine("       WHERE e.eventId={1};");
            _queries.Add("GetEventProperties", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT TOP 100 e.eventId,e.eventType,e.eventCategory,e.targetObject,e.details,e.hash,");
            sqlBuilder.AppendLine("       e.eventClass,e.eventSubclass,e.startTime,e.spid,e.applicationName,e.hostName,");
            sqlBuilder.AppendLine("       e.serverName,e.loginName,e.success,e.databaseName,e.databaseId,e.dbUserName,");
            sqlBuilder.AppendLine("       e.objectType,e.objectName,e.objectId,e.permissions,e.columnPermissions,e.targetLoginName,");
            sqlBuilder.AppendLine("       e.targetUserName,e.roleName,e.ownerName,e.alertLevel,e.checksum,e.privilegedUser,t.name,");
            sqlBuilder.AppendLine("       t.category,e.fileName,e.linkedServerName,e.parentName,e.isSystem,e.sessionLoginName,");
            sqlBuilder.AppendLine("       e.providerName,e.appNameId,e.hostId,e.loginId,e.endTime,e.startSequence,e.endSequence, ");
            sqlBuilder.AppendLine(
                "  hasSensitiveColumns = (case when e.eventCategory = {0} then (select count(sc.columnName) from {1}..[SensitiveColumns] sc where e.eventId = sc.eventId) else (select 0) END)");
            sqlBuilder.AppendLine("FROM {1}..Events AS e LEFT OUTER JOIN SQLcompliance..EventTypes t ON e.eventType=t.evtypeid WHERE startTime >= {2} AND databaseId = {3};");
            _queries.Add("GetRecentDatabaseEvents", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_DetachArchive]");
            _queries.Add("DetachArchive", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetAuditedEvents]");
            _queries.Add("GetAuditedEvents", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_SetViewSetting]");
            _queries.Add("SetViewSetting", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetFilteredInstancesStatuses]");
            _queries.Add("GetFilteredInstancesStatuses", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetArchivedEvents]");
            _queries.Add("GetArchivedEvents", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_AttachArchive]");
            _queries.Add("AttachArchive", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetEventsFilters]");
            _queries.Add("GetEventFiltersForDatabase", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_SelectFilteredAlerts]");
            _queries.Add("GetFilteredAlerts", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_SelectFilteredActivityLogs]");
            _queries.Add("GetFilteredActivityLogs", sqlBuilder.ToString());
            sqlBuilder.Clear();


            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_SelectFilteredChangeLogs]");
            _queries.Add("GetFilteredChangeLogs", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetAlertRules]");
            _queries.Add("GetAlertRules", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetEnableAlertRules]");
            _queries.Add("GetEnableAlertRules", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetAlertsByDatabases]");
            _queries.Add("GetAlertsByDatabases", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetManagedInstances]");
            _queries.Add("GetManagedInstances", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetOwnersAndLocations]");
            _queries.Add("GetOwnersAndLocations", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_UpdateManagedInstance]");
            _queries.Add("UpdateManagedInstance", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetManagedServerInstances]");
            _queries.Add("GetManagedServerInstances", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_UpdateManagedInstancesCredentials]");
            _queries.Add("UpdateManagedInstancesCredentials", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetManagedInstance]");
            _queries.Add("GetManagedInstance", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetUserSettings]");
            _queries.Add("GetUserSettings", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_CreateUpdateUserSettings]");
            _queries.Add("CreateUpdateUserSettings", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_DeleteUserSettings]");
            _queries.Add("DeleteUserSettings", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetAlertsGroups]");
            _queries.Add("GetAlertsGroups", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetAlerts]");
            _queries.Add("GetAlerts", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetInstancesForSynchronization]");
            _queries.Add("GetInstancesForSynchronization", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_MarkInstancesAsSynchronized]");
            _queries.Add("MarkInstancesAsSynchronized", sqlBuilder.ToString());
            sqlBuilder.Clear();

            //4.1.1.8_start
            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetApplicationActivity]");
            _queries.Add("GetApplicationActivity", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetBeforeAfterData]");
            _queries.Add("GetDMLActivity", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetLoginCreationHistory]");
            _queries.Add("GetLoginCreationHistory", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetLoginDeletionHistory]");
            _queries.Add("GetLoginDeletionHistory", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetObjectActivityReport]");
            _queries.Add("GetObjectActivity", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetPermissionDenied]");
            _queries.Add("GetPermissionDeniedActivity", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetUserActivity]");
            _queries.Add("GetUserActivity", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetRowCount]");
            _queries.Add("GetRowCount", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetConfigurationSettings]");
            _queries.Add("GetRegulatoryCompliance", sqlBuilder.ToString());
            sqlBuilder.Clear();
            //4.1.1.8_end
            //start sqlcm 5.6 - 5469(Configuration Check Report)
            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetServerInstances]");
            _queries.Add("ConfigurationCheck_GetServerInstances", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetDatabases]");
            _queries.Add("ConfigurationCheck_GetDatabases", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetAuditSettings]");
            _queries.Add("ConfigurationCheck_GetAuditSettings", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetConfigurationSetting]");
            _queries.Add("GetConfigurationCheck", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_cmreport_GetParameterName]");
            _queries.Add("ConfigurationCheck_GetParameterName", sqlBuilder.ToString());
            sqlBuilder.Clear();

            //end sqlcm 5.6 - 5467(Configuration Check Report)
            sqlBuilder.AppendLine(" SELECT 	COUNT(*) AS 'ProcessedEventsCount' FROM [{0}]..[Events]");
            _queries.Add("GetProcessedEventsCountForServer", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(
                " SELECT instance,displayName,description,databaseName FROM [SQLcompliance]..[SystemDatabases] WHERE instance = @instance AND databaseType = 'Archive'");
            _queries.Add("GetArchivesList", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(
                " SELECT instance,displayName,description,startDate,endDate, bias,standardBias,standardDate,daylightBias,daylightDate, defaultAccess,containsBadEvents,timeLastIntegrityCheck,lastIntegrityCheckResult, sqlComplianceDbSchemaVersion,eventDbSchemaVersion FROM [{0}]..[Description]");
            _queries.Add("GetArchiveProperties", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT COUNT(*) AS 'DatabaseCount' FROM [SQLcompliance]..[Databases] WHERE [srvId] = {0} AND [isEnabled] = 1");
            _queries.Add("GetAuditedDatabaseCount", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT COUNT(*) AS 'DatabaseCount' FROM sys.databases");
            _queries.Add("GetDatabaseCount", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetEventEnable]");
            _queries.Add("GetEnableAuditEventFilter", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_DeleteAuditEventFilter]");
            _queries.Add("GetAuditEventFilterDelete", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetEventFilterExport]");
            _queries.Add("GetDataByFilterId", sqlBuilder.ToString());
            sqlBuilder.Clear();
			
			sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_InsertEventFilter]");
            _queries.Add("InsertStatusEventFilter", sqlBuilder.ToString());
            sqlBuilder.Clear();
			
			sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetAuditedEvents]");
            _queries.Add("GetAuditEventFilter", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT ");
            sqlBuilder.AppendLine(" 	alertId 'Id', ");
            sqlBuilder.AppendLine(" 	'[' + s.eventDatabase + ']..[Events]' 'EventTable'");
            sqlBuilder.AppendLine(" FROM Alerts a");
            sqlBuilder.AppendLine(" INNER JOIN [Servers] s ON s.instance = a.instance");
            _queries.Add("GetAlertIdsAndEventTableNames", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT [RuleName] AS 'RuleName' FROM [SQLcompliance]..[DataRuleTypes] WHERE [DataRuleId] = {0}");
            _queries.Add("GetDataAlertRuleName", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT [RuleName] AS 'RuleName' FROM [SQLcompliance]..[StatusRuleTypes] WHERE [StatusRuleId] = {0}");
            _queries.Add("GetStatusAlertRuleName", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT [name] AS 'RuleName' FROM [SQLcompliance]..[EventTypes] WHERE [evtypeid] = {0}");
            _queries.Add("GetEventAlertRuleName", sqlBuilder.ToString());
            sqlBuilder.Clear();

            tempSqlBuilder.AppendLine(" SELECT");
            tempSqlBuilder.AppendLine(" 	 a.alertId AS 'Id'");
            tempSqlBuilder.AppendLine(" 	,e.databaseName AS 'DatabaseName'");
            tempSqlBuilder.AppendLine(" 	,a.alertLevel AS 'AlertStatus'");
            tempSqlBuilder.AppendLine(" 	,a.alertType AS 'RuleType'");
            tempSqlBuilder.AppendLine(" 	,a.eventType  AS 'EventType'");
            tempSqlBuilder.AppendLine(" 	,e.serverName AS 'SqlServer'");
            tempSqlBuilder.AppendLine(" 	,a.created AS 'Time'");
            tempSqlBuilder.AppendLine(" FROM [SQLcompliance]..[Alerts] a");
            tempSqlBuilder.AppendLine(" INNER JOIN [SQLcompliance]..[AlertTypes] at ON at.alertTypeId = a.alertType ");
            tempSqlBuilder.AppendLine(" INNER JOIN {0} e ON e.eventId = a.alertEventId");
            tempSqlBuilder.AppendLine(" INNER JOIN [SQLcompliance]..[Databases] d ON d.sqlDatabaseId = e.databaseId");

            sqlBuilder.Append(tempSqlBuilder);
            sqlBuilder.AppendLine(" WHERE a.alertId = {1} AND d.isEnabled = {2}");
            _queries.Add("GetDatabaseAlert", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append(tempSqlBuilder);
            sqlBuilder.AppendLine(" WHERE a.alertId = {1}");
            _queries.Add("GetAllDatabaseAlert", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT [name]");
            sqlBuilder.AppendLine(" FROM [SQLcompliance].[dbo].[EventFilters]");
            sqlBuilder.AppendLine(" WHERE ");
            sqlBuilder.AppendLine("     [targetInstances] LIKE '%{0}%' AND ");
            sqlBuilder.AppendLine("     [enabled] = 1");
            _queries.Add("GetEnabledEventFiltersForInstance", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT TOP 100 [eventId] AS 'Id'");
            sqlBuilder.AppendLine(" 	          ,et.category AS 'Category'");
            sqlBuilder.AppendLine(" 	          ,et.name AS 'EventType'");
            sqlBuilder.AppendLine(" 	          ,e.startTime AS 'Time'");
            sqlBuilder.AppendLine(" 	          ,e.details AS 'Details'");
            sqlBuilder.AppendLine(" FROM [{0}]..[Events] e");
            sqlBuilder.AppendLine(" LEFT JOIN [SQLcompliance]..[EventTypes] et ON et.evcatid = e.eventCategory AND et.evtypeid = e.eventType");
            sqlBuilder.AppendLine(" WHERE e.[startTime] >= {1}"); // 2015-2-15 12:13:47.204
            sqlBuilder.AppendLine(" ORDER BY e.startTime DESC, e.eventId DESC");
            _queries.Add("GetEvents", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT TOP 100 [eventId] AS 'Id'");
            sqlBuilder.AppendLine(" 	          ,et.category AS 'Category'");
            sqlBuilder.AppendLine(" 	          ,et.name AS 'EventType'");
            sqlBuilder.AppendLine(" 	          ,e.startTime AS 'Time'");
            sqlBuilder.AppendLine(" 	          ,e.details AS 'Details'");
            sqlBuilder.AppendLine(" FROM [{0}]..[Events] e");
            sqlBuilder.AppendLine(" LEFT JOIN [SQLcompliance]..[EventTypes] et ON et.evcatid = e.eventCategory AND et.evtypeid = e.eventType");
            sqlBuilder.AppendLine(" WHERE e.[startTime] >= {1}"); // 2015-2-15 12:13:47.204
            sqlBuilder.AppendLine(" AND e.[databaseId] = {2}");
            sqlBuilder.AppendLine(" ORDER BY e.startTime DESC, e.eventId DESC");
            _queries.Add("GetDatabaseEvents", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT category,SUM(count) as categoryValue FROM {0}..Stats WHERE dbId = {1} AND date >= {2} AND date <= {3} AND category IN (8,9,10,11,15) GROUP BY category");
            _queries.Add("GetEventDistributionForDatabase", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT [instance] AS 'Instance'");
            sqlBuilder.AppendLine("       ,[eventDatabase] AS 'EventDatabase'");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[Servers]");
            sqlBuilder.AppendLine("    WHERE [srvId] = {0}");
            _queries.Add("GetInstanceInfo", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" DECLARE @DataChangeTableCount INT");
            sqlBuilder.AppendLine(" DECLARE @SensitiveColumnTableCount INT");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine(" SELECT @DataChangeTableCount = COUNT(*) FROM [SQLcompliance]..[DataChangeTables] WHERE [dbId] = {0} AND [srvId] = {1}");
            sqlBuilder.AppendLine(" SELECT @SensitiveColumnTableCount = COUNT(*) FROM [SQLcompliance].[dbo].[SensitiveColumnTables] WHERE [dbId] = {0} AND [srvId] = {1}");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine(" SELECT [dbId]");
            sqlBuilder.AppendLine("       ,@DataChangeTableCount AS 'BeforeAfterTableCount'");
            sqlBuilder.AppendLine(" 	 ,@SensitiveColumnTableCount AS 'SensitiveColumnTableCount'");
            sqlBuilder.AppendLine("       ,[srvId]");
            sqlBuilder.AppendLine("       ,[srvInstance]");
            sqlBuilder.AppendLine("       ,[sqlDatabaseId]");
            sqlBuilder.AppendLine("       ,[name]");
            sqlBuilder.AppendLine("       ,[description]");
            sqlBuilder.AppendLine("       ,[isSqlSecureDb]");
            sqlBuilder.AppendLine("       ,[isEnabled]");
            sqlBuilder.AppendLine("       ,[timeCreated]");
            sqlBuilder.AppendLine("       ,[timeLastModified]");
            sqlBuilder.AppendLine("       ,[timeEnabledModified]");
            sqlBuilder.AppendLine("       ,[auditDDL]");
            sqlBuilder.AppendLine("       ,[auditSecurity]");
            sqlBuilder.AppendLine("       ,[auditAdmin]");
            sqlBuilder.AppendLine("       ,[auditDML]");
            sqlBuilder.AppendLine("       ,[auditSELECT]");
            sqlBuilder.AppendLine("       ,[auditFailures]");
            sqlBuilder.AppendLine("       ,[auditCaptureSQL]");
            sqlBuilder.AppendLine("       ,[auditExceptions]");
            sqlBuilder.AppendLine("       ,[auditDmlAll]");
            sqlBuilder.AppendLine("       ,[auditUserTables]");
            sqlBuilder.AppendLine("       ,[auditSystemTables]");
            sqlBuilder.AppendLine("       ,[auditStoredProcedures]");
            sqlBuilder.AppendLine("       ,[auditDmlOther]");
            sqlBuilder.AppendLine("       ,[auditBroker]");
            sqlBuilder.AppendLine("       ,[auditLogins]");
            sqlBuilder.AppendLine("       ,[auditUsersList]");
            sqlBuilder.AppendLine("       ,[auditDataChanges]");
            sqlBuilder.AppendLine("       ,[auditSensitiveColumns]");
            sqlBuilder.AppendLine("       ,[auditCaptureTrans]");
            sqlBuilder.AppendLine("       ,[pci]");
            sqlBuilder.AppendLine("       ,[hipaa]");
            sqlBuilder.AppendLine("       ,[disa]");
            sqlBuilder.AppendLine("       ,[nerc]");
            sqlBuilder.AppendLine("       ,[cis]");
            sqlBuilder.AppendLine("       ,[sox]");
            sqlBuilder.AppendLine("       ,[ferpa]");
            sqlBuilder.AppendLine("       ,[auditLogouts]");
            sqlBuilder.AppendLine("       ,[auditUserLogouts]");  // SQLCM-5375 Release 5.6 - 6.1.4.3 - Capture Logout Events
            sqlBuilder.AppendLine(" FROM [SQLcompliance].[dbo].[Databases]");
            sqlBuilder.AppendLine(" WHERE [dbId] = {0} AND [srvId] = {1}");
            _queries.Add("GetDatabase", sqlBuilder.ToString());
            sqlBuilder.Clear();

            _queries.Add("GetAuditedDatabasesForInstace",
                @"select 
                      DB.dbId as Id, 
                      DB.srvId as ServerId,
                      DB.name as Name 
                  from 
                      dbo.[Databases] as DB 
                  where 
                      DB.srvId = {0} 
                  order 
                      by DB.name ASC;");

            sqlBuilder.AppendLine(" SELECT ");
            sqlBuilder.AppendLine(" 	 a.[alertId] AS 'Id'");
            sqlBuilder.AppendLine(" 	,s.[srvId] AS 'SqlServerId'");
            sqlBuilder.AppendLine(" 	,a.[alertEventId] AS 'alertEventId'");
            sqlBuilder.AppendLine(" 	,a.[created] AS 'AlertTime'");
            sqlBuilder.AppendLine(" 	,a.[instance] AS 'SqlServer'");
            sqlBuilder.AppendLine(" 	,a.[alertLevel] AS 'AlertStatus'");
            sqlBuilder.AppendLine(" 	,a.[alertType] AS 'AlertType'");
            sqlBuilder.AppendLine(" 	,r.[name] AS 'AlertRule'");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[Alerts] AS a");
            sqlBuilder.AppendLine(" LEFT JOIN [SQLcompliance]..[AlertRules] AS r ON r.ruleId = a.alertRuleId");
            sqlBuilder.AppendLine(" LEFT JOIN [SQLcompliance]..[Servers] AS s ON s.instance = a.instance");
            sqlBuilder.AppendLine(" WHERE (a.isDismissed = 0 OR a.isDismissed IS NULL) AND s.[srvId] = {0}");
            _queries.Add("GetAlertsForInstance", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT ");
            sqlBuilder.AppendLine(" 	 a.[alertId] AS 'Id'");
            sqlBuilder.AppendLine(" 	,s.[srvId] AS 'SqlServerId'");
            sqlBuilder.AppendLine(" 	,a.[alertEventId] AS 'alertEventId'");
            sqlBuilder.AppendLine(" 	,a.[created] AS 'AlertTime'");
            sqlBuilder.AppendLine(" 	,a.[instance] AS 'SqlServer'");
            sqlBuilder.AppendLine(" 	,a.[alertLevel] AS 'AlertStatus'");
            sqlBuilder.AppendLine(" 	,a.[alertType] AS 'AlertType'");
            sqlBuilder.AppendLine(" 	,r.[name] AS 'AlertRule'");
            sqlBuilder.AppendLine(" 	,ae.details");
            sqlBuilder.AppendLine(" 	,ae.eventType");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[Alerts] AS a");
            sqlBuilder.AppendLine(" LEFT JOIN [SQLcompliance]..[AlertRules] AS r ON r.ruleId = a.alertRuleId");
            sqlBuilder.AppendLine(" LEFT JOIN [SQLcompliance]..[Servers] AS s ON s.instance = a.instance");
            sqlBuilder.AppendLine(" LEFT JOIN [{0}]..[Events] e on e.eventId = a.alertEventId");
            sqlBuilder.AppendLine(" LEFT JOIN [SQLcompliance]..[AgentEvents] ae on ae.eventId = e.eventId");
            sqlBuilder.AppendLine(" WHERE (a.isDismissed = 0 OR a.isDismissed IS NULL)");
            sqlBuilder.AppendLine("     AND a.[alertType] = {1}");
            _queries.Add("GetInstancesAlerts", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT");
            sqlBuilder.AppendLine("     COUNT(*) AS 'AlertCount'");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[Alerts]");
            sqlBuilder.AppendLine(" WHERE");
            sqlBuilder.AppendLine("     [instance] = '{0}' AND [isDismissed] = 0 OR  [isDismissed] IS NULL");
            _queries.Add("GetAlertCount", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_DeleteAlertRules]");
            _queries.Add("GetDeleteAlertRules", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetUpdateSnmpConfiguration]");
            _queries.Add("GetUpdateSnmpConfiguration", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_UpdateSNMPThresholdConfiguration]");
            _queries.Add("UpdateSNMPThresholdConfiguration", sqlBuilder.ToString());
            sqlBuilder.Clear();

            //SQLCM_125_ _ Rohit_ Start
            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetSNMPThresholdConfiguration]");
            _queries.Add("GetSNMPThresholdConfiguration", sqlBuilder.ToString());
            sqlBuilder.Clear();
            //SQLCM_125_ _ Rohit_ End

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_DeleteThresholdConfiguration]");
            _queries.Add("DeleteThresholdConfiguration", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetUpdateSmtpConfiguration]");
            _queries.Add("GetUpdateSmtpConfiguration", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetUpdateWindowsLogEntry]");
            _queries.Add("GetUpdateWindowsLogEntry", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_InsertStatusAlertRules]");
            _queries.Add("InsertStatusAlertRules", sqlBuilder.ToString());
            sqlBuilder.Clear();


            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetAuditedEventsFilter]");
            _queries.Add("GetAuditEventFilterData", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetAlertRulesExport]");
            _queries.Add("GetDataByRuleId", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetDataAlertRulesInfo]");
            _queries.Add("GetDataByServerId", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetActivityProperties]");
            _queries.Add("GetActivityProperties", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.Append("[SQLcompliance]..[sp_sqlcm_GetChangeProperties]");
            _queries.Add("GetChangeProperties", sqlBuilder.ToString());
            sqlBuilder.Clear();

            //SQLCM 5.4 SCM-9 Start
            sqlBuilder.AppendLine(" UPDATE [SQLcompliance]..[WebRefreshDuration]");
            sqlBuilder.AppendLine("       SET RefreshDuration = {0}");
            _queries.Add("CreateRefreshDuration", sqlBuilder.ToString());
            sqlBuilder.Clear();

            sqlBuilder.AppendLine(" SELECT");
            sqlBuilder.AppendLine("     [RefreshDuration]");
            sqlBuilder.AppendLine(" FROM [SQLcompliance]..[WebRefreshDuration]"); ; // need to update the query
            _queries.Add("GetRefreshDuration", sqlBuilder.ToString());
            sqlBuilder.Clear();
            //SQLCM 5.4 SCM-9 END
            sqlBuilder.AppendLine(" SELECT d.[dbId]");
            sqlBuilder.AppendLine("       ,d.[srvId]");
            sqlBuilder.AppendLine("       ,s.[instance]");
            sqlBuilder.AppendLine("       ,d.[name]");
            sqlBuilder.AppendLine("       ,d.[description]");
            sqlBuilder.AppendLine("       ,d.[isEnabled]");
            sqlBuilder.AppendLine("       ,d.[timeCreated]");
            sqlBuilder.AppendLine("       ,d.[timeLastModified]");
            sqlBuilder.AppendLine("       ,d.[timeEnabledModified]");
            sqlBuilder.AppendLine("       ,d.[isSqlSecureDb]");
            sqlBuilder.AppendLine("       ,d.[isAlwaysOn]");
            sqlBuilder.AppendLine("       ,d.[isPrimary]");
            sqlBuilder.AppendLine("       ,d.[replicaServers]");
            sqlBuilder.AppendLine("       ,d.[availGroupName]");
            sqlBuilder.AppendLine("       ,d.[auditDDL]");
            sqlBuilder.AppendLine("       ,d.[auditSecurity]");
            sqlBuilder.AppendLine("       ,d.[auditAdmin]");
            sqlBuilder.AppendLine("       ,d.[auditBroker]");
            sqlBuilder.AppendLine("       ,d.[auditDML]");
            sqlBuilder.AppendLine("       ,d.[auditSELECT]");
            sqlBuilder.AppendLine("       ,d.[auditFailures]");
            sqlBuilder.AppendLine("       ,d.[auditCaptureSQL]");
            sqlBuilder.AppendLine("       ,d.[auditExceptions]");
            sqlBuilder.AppendLine("       ,d.[auditUsersList]");
            sqlBuilder.AppendLine("       ,d.[pci]");
            sqlBuilder.AppendLine("       ,d.[hipaa]");
            sqlBuilder.AppendLine("       ,d.[auditDmlAll]");
            sqlBuilder.AppendLine("       ,d.[auditUserTables]");
            sqlBuilder.AppendLine("       ,d.[auditSystemTables]");
            sqlBuilder.AppendLine("       ,d.[auditStoredProcedures]");
            sqlBuilder.AppendLine("       ,d.[auditDmlOther]");
            sqlBuilder.AppendLine("       ,d.[auditDataChanges]");
            sqlBuilder.AppendLine("       ,d.[auditSensitiveColumns]");
            sqlBuilder.AppendLine("       ,d.[auditCaptureTrans]");
            sqlBuilder.AppendLine("       ,d.[sqlDatabaseId]");
            sqlBuilder.AppendLine("       ,d.[auditPrivUsersList]");
            sqlBuilder.AppendLine("       ,d.[auditUserAll]");
            sqlBuilder.AppendLine("       ,d.[auditUserLogins]");
            sqlBuilder.AppendLine("       ,d.[auditUserFailedLogins]");
            sqlBuilder.AppendLine("       ,d.[auditUserDDL]");
            sqlBuilder.AppendLine("       ,d.[auditUserSecurity]");
            sqlBuilder.AppendLine("       ,d.[auditUserAdmin]");
            sqlBuilder.AppendLine("       ,d.[auditUserDML]");
            sqlBuilder.AppendLine("       ,d.[auditUserSELECT]");
            sqlBuilder.AppendLine("       ,d.[auditUserFailures]");
            sqlBuilder.AppendLine("       ,d.[auditUserCaptureSQL]");
            sqlBuilder.AppendLine("       ,d.[auditUserCaptureTrans]");
            sqlBuilder.AppendLine("       ,d.[auditUserExceptions]");
            sqlBuilder.AppendLine("       ,d.[auditUserUDE]");
            sqlBuilder.AppendLine("       ,d.[auditUserCaptureDDL]");
            sqlBuilder.AppendLine("       ,d.[sqlDatabaseId]");
            sqlBuilder.AppendLine("       ,d.[auditPrivUsersList]");
            sqlBuilder.AppendLine("       ,d.[auditUserAll]");
            sqlBuilder.AppendLine("       ,d.[disa]");
            sqlBuilder.AppendLine("       ,d.[nerc]");
            sqlBuilder.AppendLine("       ,d.[cis]");
            sqlBuilder.AppendLine("       ,d.[sox]");
            sqlBuilder.AppendLine("       ,d.[ferpa]");
            sqlBuilder.AppendLine(" FROM [SQLcompliance].[dbo].[Databases] d,[SQLcompliance].[dbo].[Servers]");
            sqlBuilder.AppendLine(" WHERE d.[srvId] = {0} AND s.[srvId] = {0} ORDER by s.[instance] ASC,d.[name] ASC");
            _queries.Add("GetDataAlertCheckData", sqlBuilder.ToString());
            sqlBuilder.Clear();
        }

        ~QueryBuilder()
        {
            _queries.Clear();
            _queries = null;
        }

        #endregion

        #region prperties

        public static QueryBuilder Instance
        {
            get { return _instance ?? (_instance = new QueryBuilder()); }
        }

        #endregion

        public string GetDatabase(int databaseId, int serverId)
        {
            return string.Format(_queries["GetDatabase"], databaseId, serverId);
        }

        public string GetEvents(string eventDatabase, int days)
        {
            var timeStamp = DateTime.Now.ToLocalTime().AddDays(-days);
            return string.Format(_queries["GetEvents"], eventDatabase, Transformer.Instance.CreateSafeDateTime(timeStamp));
        }

        public string GetAuditedEvents()
        {
            return _queries["GetAuditedEvents"];
        }

        public string GetArchivedEvents()
        {
            return _queries["GetArchivedEvents"];
        }

        public string GetDatabaseEvents(string eventDatabase, int days, int databaseId)
        {
            var timeStamp = DateTime.Now.ToLocalTime().AddDays(-days);
            return string.Format(_queries["GetDatabaseEvents"], eventDatabase, Transformer.Instance.CreateSafeDateTime(timeStamp), databaseId);
        }

        public string GetEventDistributionForDatabase(string eventDatabase, int days, int databaseId)
        {
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-days);

            return string.Format(_queries["GetEventDistributionForDatabase"], eventDatabase, databaseId, Transformer.Instance.CreateSafeDateTime(startDate), Transformer.Instance.CreateSafeDateTime(endDate));
        }

        public string GetRecentDatabaseEvents(string eventDatabase, int days, int databaseId)
        {
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-days);

            return string.Format(_queries["GetRecentDatabaseEvents"], (int)TraceEventCategory.SELECT, eventDatabase, Transformer.Instance.CreateSafeDateTime(startDate), databaseId);
        }

        public string GetEventFiltersForDatabase()
        {
            return string.Format(_queries["GetEventFiltersForDatabase"]);
        }

        public string GetEnabledEventFiltersForInstance(string instance)
        {
            var query = string.Format(_queries["GetEnabledEventFiltersForInstance"], instance);
            return query;
        }

        public string DismissAlert(int alertId)
        {
            var query = string.Format(_queries["DismissAlert"], alertId);
            return query;
        }

        public string DismissAlertList(IEnumerable<int> idList)
        {
            string alertIdsWithCommas = string.Join(",", idList);
            var query = string.Format(_queries["DismissAlertList"], alertIdsWithCommas);
            return query;
        }

        public string GetAlertIdsAndEventTableNames()
        {
            return _queries["GetAlertIdsAndEventTableNames"];
        }

        public string GetDatabaseCount()
        {
            return string.Format(_queries["GetDatabaseCount"]);
        }

        public string GetAuditedDatabaseCount(int instanceId)
        {
            return string.Format(_queries["GetAuditedDatabaseCount"], instanceId);
        }

        public string GetAlertRuleName(AlertRuleType type, long ruleTypeId)
        {
            switch (type)
            {
                case AlertRuleType.Data:
                    return string.Format(_queries["GetDataAlertRuleName"], ruleTypeId);

                case AlertRuleType.Event:
                    return string.Format(_queries["GetEventAlertRuleName"], ruleTypeId);

                case AlertRuleType.Status:
                    return string.Format(_queries["GetStatusAlertRuleName"], ruleTypeId);
            }

            return null;
        }

        public string GetProcessedEventCount(string eventDatabase)
        {
            return string.Format(_queries["GetProcessedEventsCountForServer"], eventDatabase);
        }

        public string GetAlertsForInstance(int serverId)
        {
            return string.Format(_queries["GetAlertsForInstance"], serverId);
        }

        public string GetEnvironmentObjectHierarchy(string objectId, EnvironmentObjectType type)
        {
            switch (type)
            {
                case EnvironmentObjectType.Root:
                    return _queries["GetEnvironmentObjectHierarchy_Root"];

                case EnvironmentObjectType.Server:
                    return string.Format(_queries["GetEnvironmentObjectHierarchy_Server"], objectId);

                default:
                    return null;
            }
        }

        public string GetCurrentAuditedSqlDatabaseAlerts()
        {
            return _queries["GetCurrentAuditedSqlDatabaseAlerts"];
        }

        internal string GetCmLicenses()
        {
            return _queries["GetCmLicenses"];
        }

        internal string GetInstanceInfo(int serverId)
        {
            return string.Format(_queries["GetInstanceInfo"], serverId);
        }

        public string GetEventProperties(string eventDatabase, int eventId)
        {
            return string.Format(_queries["GetEventProperties"], eventDatabase, eventId);
        }

        public string GetFilteredAlerts()
        {
            return _queries["GetFilteredAlerts"];
        }

        public string GetFilteredActivityLogs()
        {
            return _queries["GetFilteredActivityLogs"];
        }

        public string GetAlertRules()
        {
            return _queries["GetAlertRules"];
        }

        public string GetEnableAlertRules()
        {
            return _queries["GetEnableAlertRules"];
        }

        public string GetFilteredChangeLogs()
        {
            return _queries["GetFilteredChangeLogs"];
        }


        public string GetArchivesList()
        {
            return _queries["GetArchivesList"];
        }

        public string GetInstancesAlerts(AlertType type)
        {
            switch (type)
            {
                case AlertType.Event:
                case AlertType.Data:
                case AlertType.Status:
                    return string.Format(_queries["GetInstancesAlerts"], 
                                        (byte)type);

                default:
                    throw new Exception();
            }
        }

        internal string GetAlertCount(string server)
        {
            return string.Format(_queries["GetAlertCount"], server);
        }

        public string GetArchiveProperties(string databaseName)
        {
            return string.Format(_queries["GetArchiveProperties"], databaseName);
        }

        public string DetachArchive()
        {
            return _queries["DetachArchive"];
        }

        public string SetViewSetting()
        {
            return _queries["SetViewSetting"];
        }

        public string GetViewSettings(string connectionUser, string viewName)
        {
            string whereCondition = string.Format("WHERE UserId = '{0}'", connectionUser);
            if (!string.IsNullOrWhiteSpace(viewName))
                whereCondition = string.Format("{0} AND ViewName = '{1}'", whereCondition, viewName);

            return string.Format(_queries["GetViewSettings"], whereCondition);
        }

        public string AttachArchive()
        {
            return _queries["AttachArchive"];
        }

        public string GetAgentConnectionDetails(string instance)
        {
            return string.Format(_queries["GetAgentConnectionDetails"], instance);
        }

        public string GetAlertsByDatabases()
        {
            return _queries["GetAlertsByDatabases"];
        }

        public string GetUserSettings()
        {
            return _queries["GetUserSettings"];
        }

        public string CreateUpdateUserSettings()
        {
            return _queries["CreateUpdateUserSettings"];
        }

        public string DeleteUserSettings()
        {
            return _queries["DeleteUserSettings"];
        }

        public string GetAlertsGroups()
        {
            return _queries["GetAlertsGroups"];
        }

        public string GetAlerts()
        {
            return _queries["GetAlerts"];
        }
        //4.1.1.8_start
        public string GetApplicationActivity()
        {
            return _queries["GetApplicationActivity"];
        }

        public string GetDMLActivity()
        {
            return _queries["GetDMLActivity"];
        }

        public string GetLoginCreationHistory()
        {
            return _queries["GetLoginCreationHistory"];
        }

        public string GetLoginDeletionHistory()
        {
            return _queries["GetLoginDeletionHistory"];
        }

        public string GetObjectActivity()
        {
            return _queries["GetObjectActivity"];
        }

        public string GetPermissionDeniedActivity()
        {
            return _queries["GetPermissionDeniedActivity"];
        }

        public string GetUserActivity()
        {
            return _queries["GetUserActivity"];
        }

        public string GetRowCount()
        {
            return _queries["GetRowCount"];
        }

        public string GetRegulatoryCompliance()
        {
            return _queries["GetRegulatoryCompliance"];
        }
        //4.1.1.8_End
        //start sqlcm 5.6 - 5469(configuration check report)
        public string GetConfigurationCheck()
        {
            return _queries["GetConfigurationCheck"];
        }
        public string GetConfigurationCheckSetting()
        {
            return _queries["ConfigurationCheck_GetAuditSettings"];
        }
        //end sqlcm 5.6 - 5469(configuration check report)
        public string GetManagedInstances()
        {
            return _queries["GetManagedInstances"];
        }

        public string GetManagedInstance()
        {
            return _queries["GetManagedInstance"];
        }

        public string GetOwnersAndLocations()
        {
            return _queries["GetOwnersAndLocations"];
        }

        public string UpdateManagedInstance()
        {
            return _queries["UpdateManagedInstance"];
        }

        public string GetManagedServerInstances()
        {
            return _queries["GetManagedServerInstances"];
        }

        public string UpdateManagedInstancesCredentials()
        {
            return _queries["UpdateManagedInstancesCredentials"];
        }

        public string GetInstancesForSynchronization()
        {
            return _queries["GetInstancesForSynchronization"];
        }

        public string MarkInstancesAsSynchronized()
        {
            return _queries["MarkInstancesAsSynchronized"];
        }

        public string GetDeleteAlertRules()
        {
            return _queries["GetDeleteAlertRules"];
        }
        
        public string GetUpdateSnmpConfiguration()
        {
            return _queries["GetUpdateSnmpConfiguration"];
        }

        public string UpdateSNMPThresholdConfiguration()
        {
            return _queries["UpdateSNMPThresholdConfiguration"];
        }

        public string DeleteThresholdConfiguration()
        {
            return _queries["DeleteThresholdConfiguration"];
        }
        
        //SQLCM_125_ _ Rohit_ Start
        public string GetSNMPThresholdConfiguration()
        {
            return _queries["GetSNMPThresholdConfiguration"];
        }
        //SQLCM_125_ _ Rohit_ End

        public string GetUpdateSmtpConfiguration()
        {
            return _queries["GetUpdateSmtpConfiguration"];
        }

        public string GetUpdateWindowsLogEntry()
        {
            return _queries["GetUpdateWindowsLogEntry"];
        }

        public string InsertStatusAlertRules()
        {
            return _queries["InsertStatusAlertRules"];
        }
		
	    public string GetEnableAuditEventFilter()
        {
            return _queries["GetEnableAuditEventFilter"];
        }

        public string GetAuditEventFilterDelete()
        {
            return _queries["GetAuditEventFilterDelete"];
        }

        public string GetDataByFilterId()
        {
            return _queries["GetDataByFilterId"];
        }

        public string InsertStatusEventFilter()
        {
            return _queries["InsertStatusEventFilter"];
        }

        public string GetAuditEventFilter()
        {
            return _queries["GetAuditEventFilterData"];
        }

        public string GetDataByRuleId()
        {
            return _queries["GetDataByRuleId"];
        }

        public string GetDataAlertCheckData(int databaseId, int serverId)
        {
            return string.Format(_queries["GetDataAlertCheckData"], databaseId, serverId);
        }

        public string GetDataByServerId()
        {
            return _queries["GetDataByServerId"];
        }
        public string GetActivityProperties()
        {
            return _queries["GetActivityProperties"];
        }

        public string GetChangeProperties()
        {
            return _queries["GetChangeProperties"];
        }
        //SQLCM 5.4 SCM-9 Start
        public string CreateRefreshDuration(string refreshDuration)
        {
            return string.Format(_queries["CreateRefreshDuration"], refreshDuration);
        }

        public string GetRefreshDuration()
        {
            return _queries["GetRefreshDuration"];
        } //SQLCM 5.4 SCM-9 End
    }
}
