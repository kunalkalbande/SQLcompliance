package com.idera.sqlcm.rest;

import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

import org.apache.log4j.Logger;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.InstanceStatus;
import com.idera.common.Utility;
import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.common.rest.RestResponse;
import com.idera.cwf.model.Product;
import com.idera.sqlcm.beEntities.AuditedInstanceBE;
import com.idera.sqlcm.entities.CMActivityLogs;
import com.idera.sqlcm.entities.CMAlert;
import com.idera.sqlcm.entities.CMAlertRulesEnableRequest;
import com.idera.sqlcm.entities.CMAlertRulesExportResponse;
import com.idera.sqlcm.entities.CMAlertsSummary;
import com.idera.sqlcm.entities.CMApplyReindexForArchiveRequest;
import com.idera.sqlcm.entities.CMArchive;
import com.idera.sqlcm.entities.CMArchiveProperties;
import com.idera.sqlcm.entities.CMArchivedDatabase;
import com.idera.sqlcm.entities.CMAttachArchive;
import com.idera.sqlcm.entities.CMAuditEventFilterExportResponse;
import com.idera.sqlcm.entities.CMAuditedDatabase;
import com.idera.sqlcm.entities.CMAuditedEventFilterEnable;
import com.idera.sqlcm.entities.CMAvailabilityInfo;
import com.idera.sqlcm.entities.CMBeforeAfterDataEventsResponse;
import com.idera.sqlcm.entities.CMChangeLogs;
import com.idera.sqlcm.entities.CMCheckAgentStatusResult;
import com.idera.sqlcm.entities.CMColumnDetails;
import com.idera.sqlcm.entities.CMDataAlertRuleInfoRequest;
import com.idera.sqlcm.entities.CMDataAlertRulesInfo;
import com.idera.sqlcm.entities.CMDataByFilterId;
import com.idera.sqlcm.entities.CMDataByRuleId;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMDatabaseAuditedActivity;
import com.idera.sqlcm.entities.CMDatabaseAuditingRequest;
import com.idera.sqlcm.entities.CMDeleteEntity;
import com.idera.sqlcm.entities.CMEntity.EntityType;
import com.idera.sqlcm.entities.CMEntity.NodeType;
import com.idera.sqlcm.entities.CMEntity.RuleType;
import com.idera.sqlcm.entities.CMEnvironment;
import com.idera.sqlcm.entities.CMEnvironmentDetails;
import com.idera.sqlcm.entities.CMEvent;
import com.idera.sqlcm.entities.CMEventProperties;
import com.idera.sqlcm.entities.CMFilterEventFiltersResponse;
import com.idera.sqlcm.entities.CMFilteredActivityLogsResponse;
import com.idera.sqlcm.entities.CMFilteredAlertRulesResponse;
import com.idera.sqlcm.entities.CMFilteredAlertsResponse;
import com.idera.sqlcm.entities.CMFilteredChangeLogsResponse;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;
import com.idera.sqlcm.entities.CMManageInstanceCredentials;
import com.idera.sqlcm.entities.CMManagedInstance;
import com.idera.sqlcm.entities.CMManagedInstanceDetails;
import com.idera.sqlcm.entities.CMManagedInstances;
import com.idera.sqlcm.entities.CMPageSortRequest;
import com.idera.sqlcm.entities.CMPermissionInfo;
import com.idera.sqlcm.entities.CMResponse;
import com.idera.sqlcm.entities.CMServerAuditingRequest;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.CMTableDetails;
import com.idera.sqlcm.entities.CMTreeNode;
import com.idera.sqlcm.entities.CMUpdatedArchiveProperties;
import com.idera.sqlcm.entities.CMUpgradeAgentResponse;
import com.idera.sqlcm.entities.CMUserSettings;
import com.idera.sqlcm.entities.CMValidateInstanceCredentialResult;
import com.idera.sqlcm.entities.CMViewSettings;
import com.idera.sqlcm.entities.CategoryRequest;
import com.idera.sqlcm.entities.CategoryResponse;
import com.idera.sqlcm.entities.EnvironmentAlerts;
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.entities.InstanceAlert;
import com.idera.sqlcm.entities.License;
import com.idera.sqlcm.entities.LicenseDetails;
import com.idera.sqlcm.entities.ProfilerObject;
import com.idera.sqlcm.entities.RegulationSection;
import com.idera.sqlcm.entities.RegulationType;
import com.idera.sqlcm.entities.StatisticDataResponse;
import com.idera.sqlcm.entities.ViewNameResponse;
import com.idera.sqlcm.entities.addserverwizard.AddInstanceResult;
import com.idera.sqlcm.entities.addserverwizard.AddServerEntity;
import com.idera.sqlcm.entities.addserverwizard.AgentDeploymentProperties;
import com.idera.sqlcm.entities.addserverwizard.AgentServiceAccount;
import com.idera.sqlcm.entities.addserverwizard.InstanceAvailableResult;
import com.idera.sqlcm.entities.addserverwizard.RegulationCustomDetail;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;
import com.idera.sqlcm.entities.addserverwizard.ServerRegisteredStatusInfo;
import com.idera.sqlcm.entities.addserverwizard.ServerRegisteredStatusInfoRequestArg;
import com.idera.sqlcm.entities.database.properties.CMCLRStatus;
import com.idera.sqlcm.entities.database.properties.CMDatabaseProperties;
import com.idera.sqlcm.entities.instances.CMAgentProperties;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.entities.instances.CMInstanceProperties;
import com.idera.sqlcm.entities.instances.CMRemoveInstanceResponse;
import com.idera.sqlcm.facade.SNMPConfigData;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.service.rest.SQLCMRestMethods;
import com.idera.sqlcm.smtpConfiguration.GetSNMPConfigResponse;
import com.idera.sqlcm.smtpConfiguration.GetSNMPConfiguration;
import com.idera.sqlcm.snmpTrap.UpdateSNMPConfiguration;
import com.idera.sqlcm.ui.auditReports.CMAuditApplication;
import com.idera.sqlcm.ui.auditReports.CMAuditConfiguration;
import com.idera.sqlcm.ui.auditReports.CMAuditDML;
import com.idera.sqlcm.ui.auditReports.CMAuditLoginCreation;
import com.idera.sqlcm.ui.auditReports.CMAuditLoginDeletion;
import com.idera.sqlcm.ui.auditReports.CMAuditObjectActivity;
import com.idera.sqlcm.ui.auditReports.CMAuditPermissionDenied;
import com.idera.sqlcm.ui.auditReports.CMAuditRegulatoryCompliance;
import com.idera.sqlcm.ui.auditReports.CMAuditRowCount;
import com.idera.sqlcm.ui.auditReports.CMAuditUserActivity;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditConfigurationDatabaseResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditConfigurationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditIderaDefaultValuesResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListApplicationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListConfigurationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListDMLRespose;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListLoginCreationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListLoginDeletionResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListObjectActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListPermissionDeniedActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListRegulatoryComplianceResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListRowCountResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListUserActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditSettingResponse;
import com.idera.sqlcm.ui.components.alerts.AlertGroupDTO;
import com.idera.sqlcm.ui.dialogs.UpdateSNMPThresholdConfiguration;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.AddDataAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.RegulationSettings;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesSaveEntity;
import com.idera.sqlcm.ui.importAuditSetting.AddWizardImportEntity;
import com.idera.sqlcm.ui.instancedetails.AllSensitiveDetails;
import com.idera.sqlcm.ui.instancedetails.STABS__;

public class SQLCMRestClient {

	private static final Logger log = Logger.getLogger(SQLCMRestClient.class);

	private static final String ALERT_TYPE = "alertType=";
	private static final String ALERT_LEVEL = "alertLevel=";
	private static final String AMP = "&";
	private static final String DATABSE_ID = "databaseId=";
	private static final String DATABASE_ID = "databaseId=";
	private static final String DATABASE_NAME = "databaseName=";
	private static final String TABLE_NAME = "tableName=";
	private static final String CATEGORY = "category=";
	private static final String ID = "id=";
	private static final String INSTANCE_ID = "instanceId=";
	private static final String INSTANCE = "instance=";
	private static final String ENTITY_TYPE = "type=";
	private static final String INTERVAL = "days=";
	private static final String OBJECT_ID = "objectId=";
	private static final String QSTN = "?";
	private static final String SERVER_ID = "serverId=";
	private static final String SERVER_NAME = "serverName=";

	private static final String EVENT_ID = "eventId=";
	private static final String PARENT_ID = "parentId=";
	private static final String FILTER_TEXT = "filterText=";
	private static final String VIEW_ID = "viewId=";
	private static final String VIEW_NAME = "viewName=";
	private static final String ARCHIVE = "archive=";
	private static final String SHOW_ALL = "showAll=";
	private static final String PAGE = "page=";
	private static final String PAGE_SIZE = "pageSize=";
	private static final String SORT_COLUMN = "sortColumn=";
	private static final String SORT_DIRECTION = "sortDirection=";
	private static CoreRestClient restClient = CoreRestClient.getInstanceWithoutTimeout();
	private static SQLCMRestClient instance;

	public SQLCMRestClient() {
	}

	public static SQLCMRestClient getInstance() {
		if (instance == null) {
			instance = new SQLCMRestClient();
		}
		return instance;
	}

	/**
	 * Enum to hold all the Rest method calls
	 */
	private enum RestMethods {

		GET_COLUMN_DETAILS_FOR_ALL("/GetColumnDetailsForAll"),
		GET_TABLES_DETAILS_FOR_ALL("/GetTableDetailsForAll"),
		EXPORT_EVENT_FILTERS("/ExportEventFilter"),
		Get_AUDITED_INSTANCES_WIDGET_DATA("/GetAuditedInstancesWidgetData"),
		GET_ALL_DATABASES_FOR_INSTANCE("/GetAllDatabasesForInstance"),
		SET_REFRESH_DURATION("/SetRefreshDuration"),
		GET_REFRESH_DURATION("/GetRefreshDuration"),
		GET_VIEW_NAMES("/GetViewName"),
		GET_ALERTS("/GetAlerts"), 
		GET_FILTERED_ACTIVITY_LOGS(	"/GetFilteredActivityLogs"),
		GET_UPDATE_SNMP_CONFIGURATION("/GetUpdateSnmpConfiguration"),
		GET_UPDATE_SNMP_THRESHOLD_CONFIGURATION("/UpdateSNMPThresholdConfiguration"),
		DELETE_THRESHOLD_CONFIGURATION("/DeleteThresholdConfiguration"),
		GET_SNMP_THRESHOLD_CONFIGURATION("/GetSNMPThresholdConfiguration"),
		GET_FILTERED_CHANGE_LOGS("/GetFilteredChangeLogs"),
		GET_ENABLE_ALERT_RULES("/GetEnableAlertRules"),
		INSERT_STATUS_ALERT_RULES("/InsertStatusAlertRules"),
		GET_DELETE_ALERT_RULES("/DeleteAlertRules"),
		GET_ALERTS_RULES("/GetAlertRules"),
		GET_ALERTS_GROUPS("/GetAlertsGroups"), 
		GET_ALL_INSTANCES_ALERTS("/GetAllInstancesAlerts"),
		GET_FILTERED_ALERTS("/GetFilteredAlerts"), 
		GET_ALL_INSTANCES_STATUS("/GetAllInstancesStatus"),
		GET_AUDIT_EVENTS("/GetEvents"),
		GET_AUDITED_EVENTS("/GetAuditedEvents"),
		GET_ARCHIVED_EVENTS("/GetArchivedEvents"),
		GET_EVENT_PROPERTIES("/GetEventProperties"),
		GET_AUDITED_INSTANCES_ALERTS("/GetAuditedInstancesAlert"),
		GET_CM_ENVIRONMENT("/GetMyCmEnvironment"),
		GET_DATABASE_AUDIT_EVENTS("/GetDatabaseEvents"),
		GET_DATABASE_DETAILS("/GetAuditedDatabase"),
		GET_DATABASE_STATICS("/GetDatabaseStatsData"),
		GET_ENVIRONMENT_ALERTS("/GetEnvironmentAlertStatus"),
		GET_INSTANCE_DETAILS("/GetAuditedSqlServer"),
		GET_INSTANCE_STATICS("/GetStatsData"),
		GET_RECENT_DATABASE_ACTIVITY("/GetRecentDatabaseActivity"),
		GET_LICENSE_DETAILS("/GetCmLicenses"),
		ADD_LICENSE("/AddLicense"),
		GET_TREE_NODES("/GetEnvironmentObjects"),
		GET_List_Data("/GetEnvironmentObjects"),
		GET_ENVIRONMENT_DETAILS("/GetEnvironmentDetails"), 
		GET_AUDITED_DATABASES_FOR_INSTANCE("/GetAuditedDatabasesForInstance"),
		GET_EVENTS_DATABASES_FOR_INSTANCE("/GetEventsDatabasesForInstance"),
		GET_AUDITED_ACTIVITY_FOR_DATABASE("/GetAuditedActivityForDatabase"), 
		GET_INSTANCES("GetAuditedSqlServers"), 
		GET_REGULATION_SECTION_DICTIONARY("/GetRegulationSectionDictionary"),
		GET_REGULATION_TYPE_LIST("/GetRegulationTypeList"), 
		GET_NOT_REGISTERED_DATABASES_FOR_INSTANCE("/GetNotRegisteredDatabasesForInstance"),
		VERIFY_PERMISSIONS("/VerifyPermissions"),
		ADD_DATABASES("/AddDatabases"),
		GET_REGULATION_SETTINGS_FOR_DATABASE("/GetRegulationSettingsForDatabase"),
		GET_DATABASE_TABLE_LIST("/GetDatabaseTableList"),
		GET_COLUMN_SEARCH_PROFILE("/GetProfileDetails"),
		GET_COLUMN_SEARCH_ACTIVE_PROFILE("/GetActiveProfile"),
		GET_IS_UPDATED("/GetIsUpdated"),
		DELETE_SELECTED_STRING("/DeleteString"),
		INSERT_NEW_STRING("/InsertString"),
		UPDATE_STRING("/UpdateString"),
		DELETE_PROFILE("/DeleteProfile"),
		OPEN_PROFILE("/ActivateProfile"),
		NEW_PROFILE("/InsertNewProfile"),
		UPDATE_CURRENT_PROFILE("/UpdateCurrentProfile"),
		UPDATE_IS_UPDATED("/UpdateIsUpdated"),
		RESET_DATA("/ResetData"),
		GET_COLUMN_LIST("/GetColumnList"),
		GET_SERVER_ROLE_USERS_FOR_INSTANCE("/GetServerRoleUsersForInstance"),
		GET_AUDIT_INSTANCE_PROPERTIES("/GetAuditServerProperties"),
		UPDATE_AUDIT_SERVER_PROPERTIES("/UpdateAuditServerProperties"),
		UPDATE_AUDIT_CONFIGURATION_FOR_SERVER("/UpdateAuditConfigurationForServer"),
		GET_AGENT_PROPERTIES("/GetAgentProperties"),
		UPDATE_AGENT_PROPERTIES("/UpdateAgentProperties"),
		UPGRADE_AGENT("/UpgradeAgent"),
		CHECK_AGENT_STATUS(	"/CheckAgentStatus"),
		ENABLE_AUDITING_FOR_DATABASES("/EnableAuditingForDatabases"),
		ENABLE_AUDITING_FOR_SERVERS("/EnableAuditingForServers"),
		REMOVE_AUDIT_SERVERS("/RemoveServers"),
		REMOVE_DATABASE("/RemoveDatabase"),
		GET_EVENTS_BY_INTERVAL_FOR_DATABASE("/GetEventsByIntervalForDatabase"),
		GET_EVENTS_BY_INTERVAL_FOR_INSTANCE("/GetEventsByIntervalForInstance"),
		GET_DATABASE_AVAILABILITY_GROUPS("/GetDatabaseAvailabilityGroups"),
		IS_INSTANCE_AVAILABLE("/IsInstanceAvailable"),
		GET_DATABASE_PROPERTIES("/GetAuditDatabaseProperties"),
		UPDATE_DATABASE_PROPERTIES("/UpdateAuditDatabaseProperties"),
		ENABLE_CLR("/EnableClr"),
		GET_ARCHIVES_LIST("/GetArchivesList"),
		GET_ARCHIVE_PROPERTIES("/GetArchiveProperties"),
		GET_DATABASES_FOR_ARCHIVE_ATTACHMENT("/GetDatabasesForArchiveAttachment"),
		CHECK_NEED_INDEX_UPDATES_FOR_ARCHIVE("/CheckNeedIndexUpdatesForArchive"),
		IS_INDEX_START_TIME_FOR_ARCHIVE_DATABASE_IS_VALID("/IsIndexStartTimeForArchiveDatabaseIsValid"),
		UPDATE_ARCHIVE_PROPERTIES("/UpdateArchiveProperties"),
		DETACH_ARCHIVE("/DetachArchive"),
		ATTACH_ARCHIVE("/AttachArchive"),
		UPGRADE_EVENT_DATABASE_SCHEMA("/UpgradeEventDatabaseSchema"),
		UPDATE_INDEXES_FOR_EVENT_DATABASE("/UpdateIndexesForEventDatabase"),
		APPLY_REINDEX_FOR_ARCHIVE("/ApplyReindexForArchive"),
		GET_VIEW_SETTINGS("/GetViewSettings"),
		SET_VIEW_SETTINGS("/SetViewSettings"),
		GET_FILTERED_AUDITED_INSTANCES_STATUS("/GetFilteredAuditedInstancesStatus"),
		DISMISS_ALERT("/DismissAlert"),
		DISMISS_ALERTS("/DismissAlerts"),
		DISMISS_ALERTS_BY_GROUP_AND_LEVEL("/DismissAlertsGroupForInstance"),
		CAN_ADD_ONE_MORE_INSTANCE("/CanAddOneMoreInstance"),
		GET_ALL_NOT_REGISTERED_INSTANCE_NAME_LIST("/GetAllNotRegisteredInstanceNameList"), 
		ADD_SERVERS("/AddServers"),
		IMPORT_SERVER_INSTANCES("/ImportServerInstances"),
		CREATE_UPDATE_USER("/CreateUpdateUserSettings"),
		GET_ALL_CM_USERS("/GetAllUserSettings"),
		GET_CM_USER_SETTINGS_BY_ID("/GetUserSettings"),
		DELETE_USER_SETTINGS("/DeleteUserSettings"),
		GET_MANAGED_INSTANCES("/GetManagedInstances"),
		GET_MANAGED_INSTANCE("/GetManagedInstance"),
		UPDATE_MANAGE_INSTANCE("/UpdateManagedInstance"),
		UPDATE_MANGED_INSTANCES_CREDENTIALS("/UpdateManagedInstancesCredentials"),
		VALIDATE_INSTANCE_CREDENTIALS("/ValidateInstanceCredentials"),
		GET_AGENT_DEPLOYMENT_PROPERTIES_FOR_INSTANCE("/GetAgentDeploymentPropertiesForInstance"),
		VALIDATE_DOMAIN_CREDENTIALS("/ValidateCredentials"),
		CHECK_SERVER_REGISTERED_STATUS_BY_NAME("/CheckInstanceRegisteredStatus"),
		GET_EVENTS_BY_CATEGORY_AND_DATE_FOR_INSTANCE("/GetEventsByCategoryAndDateForInstance"),
		INSERT_EVENT_FILTER_DATA("/InsertEventFilterData"),
		GET_AUDITED_EVENT_FILTER("/GetAuditedEventFilter"),
		GET_AUDITED_EVENT_FILTER_ENABLED("/GetEnableAuditEventFilter"),
		GET_AUDITED_EVENT_FILTER_DELETE("/GetAuditEventFilterDelete"),
		GET_DATA_BY_FILTER_ID("/GetAuditEventFilterExport"),
		ALERT_RULES_EXPORT_DATA("/GetAlertRulesExport"),
		EXPORT_ALERT_RULES("/ExportAlertRules"),
		IMPORT_ALERT_RULES("/ImportAlertRules"),
		GET_AUDIT_REPORT_APPLICATION_ACTIVITY("/GetAuditedReportApplicationActivity"),
		GET_AUDIT_REPORT_DML_ACTIVITY("/GetAuditedReportDMLActivity"),
		GET_AUDIT_REPORT_LOGIN_CREATION_HISTORY("/GetAuditedReportLoginCreationHistory"),
		GET_AUDIT_REPORT_LOGIN_DELETION_HISTORY("/GetAuditedReportLoginDeletionHistory"),
		GET_AUDIT_REPORT_OBJECT_ACTIVITY("/GetAuditedReportObjectActivity"),
		GET_AUDIT_REPORT_PERMISSION_DENIED_ACTIVITY("/GetAuditedReportPermissionDeniedActivity"),
		GET_AUDIT_REPORT_USER_ACTIVITY("/GetAuditedReportUserActivity"),
		GET_AUDIT_REPORT_ROW_COUNT("/GetAuditedReportRowCount"),
		DATA_ALERT_RULES_INFO("/GetDataAlertRulesInfo"),
		GET_ACTIVITY_PROPERTIES("/GetActivityProperties"),
		GET_CHANGE_PROPERTIES("/GetChangeProperties"),
		CATEGORY_INFO("/GetCateGoryInfo"),
		IMPORT_AUDIT_EVENT_FILTERS("/ImportAuditEventFilters"),
		UPDATE_SNMP_CONFIG_DATA("/UpdateSnmpConfigData"),
		CHECK_SNMP_ADDRESS("/CheckSnmpAddress"),
		GET_LOCAL_TIME("/GetLocalTime"),
		VALIDATE_SENSITIVE_COLUMNS("/GetValidateSensitiveColumns"),
		VALIDATE_AUDIT_SETTINGS("/validateAuditSettings"),
		SAVE_SENSITIVE_COLUMN_DATA("/GetSaveSensitiveColumnData"),
		EXPORT_SERVER_AUDIT_SETTING("/ExportServerAuditSettings"),
		EXPORT_DATABASE_AUDIT_SETTING("/ExportDatabaseAuditSettings"), 		
		IMPORT_DATABASE_AUDIT_SETTING("/ImportDatabaseAuditSetting"),
		IS_LINQ_DLL_LOADED("/IsLinqDllLoaded"),
		GET_REGULATION_SETTINGS_SERVER_LEVEL("/GetRegulationSettingsServerLevel"),
		GET_NEW_DATABASE_COLUMN_LIST("/GetNewDatabaseColumnList"),
		GET_SERVER_CLR_STATUS("/GetServerClrStatus"),
		EXPORT_SERVER_REGULATION_AUDIT_SETTING("/ExportServerRegulationAuditSettings"),
		EXPORT_DATABASES_REGULATION_AUDIT_SETTING("/ExportDatabasesRegulationAuditSettings"),
		GET_ALL_AUDITED_INSTANCES("/GetAllAuditedInstances"),
		GET_DATABASE_BY_SERVER_NAME("/GetDatabaseByServerName"),
		GET_ALL_AUDIT_SETTINGS("/GetAllAuditSettings"),
		GET_ALL_REGULATION_GUIDELINES("/GetAllRegulationGuidelines"),
		GET_AUDIT_REPORT_REGULATORY_COMPLIANCE("/GetAuditedReportRegulatoryCompliance"),
		GET_AUDIT_SETTINGS("/GetConfigurationCheckSettings"),
		GET_AUDIT_REPORT_CONFIGURATION_ACTIVITY("/GetConfigurationCheckData"),
		GET_CONFIGURATION_CHECK_DEFAULT_SERVER("/GetConfigurationCheckDefaultServer"),
		GET_CONFIGURATION_CHECK_DEFAULT_DATABASE("/GetConfigurationCheckDefaultDatabase");

		;

		String methodName;

		RestMethods(String methodName) {
			this.methodName = methodName;
		}

		public String getMethodName() {
			return methodName;
		}
	}

	/**
	 * Method to create request url
	 * @param methodCallUrl
	 * @return
	 * @throws RestException
	 */
	private String buildUrl(String methodCallUrl) throws RestException {
		Product currentProduct = WebUtil.getCurrentProduct();
		if (currentProduct == null) {
			log.error(String.format("ERROR: Rest Call to %s failed as Current Product is null", methodCallUrl));
			throw new RestException(SQLCMI18NStrings.ERR_CURRENT_PRODUCT_IS_NULL);
		}
		return currentProduct.getRestUrl() + methodCallUrl;

	}

	private RestException getRestException(String exceptionMessage, Exception restException, String callUrl) {
		log.error(Utility.getMessage(exceptionMessage, callUrl));
		Throwable cause = restException.getCause();
		if (cause != null) {
			log.error(cause.getMessage());
			log.error(Utility.getStackTrace(cause));
		} else {
			log.error(restException.getMessage());
			log.error(Utility.getStackTrace(restException));
		}
		return new RestException(restException, exceptionMessage, callUrl);
	}

	/**
	 * Method to executed GET request to fetch RestResponse.
	 * @param methodName
	 * @param typeReference
	 * @return RestResponse
	 * @throws RestException
	 */
	private <T> RestResponse<T> getRestResponse(String methodName, TypeReference<T> typeReference) throws RestException {
		String restUrl = buildUrl(methodName);
		log.info("Rest call GET: " + restUrl);
		return restClient.getProductData(restUrl, typeReference);
	}

	/**
	 * Method to executed POST request to fetch RestResponse.
	 * @param methodName
	 * @param requestObject
	 * @param typeReference
	 * @return RestResponse
	 * @throws RestException
	 */
	private <T> RestResponse<T> getPostRestResponse(String methodName, Object requestObject, TypeReference<T> typeReference) throws RestException {
		String restUrl = buildUrl(methodName);
		log.info("Rest call POST: " + restUrl);
		return restClient.postProductData(restUrl, requestObject, typeReference);
	}

	/**
	 * Method to get product license details.
	 * @return
	 * @throws RestException
	 */
	public LicenseDetails getLicenseDetails() throws RestException {
		String url = RestMethods.GET_LICENSE_DETAILS.getMethodName();
		try {
			RestResponse<LicenseDetails> restResponse = getRestResponse(url, new TypeReference<LicenseDetails>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_LICENSE_DETAILS, x, url);
		}
	}

	public License getLicense() throws RestException {
		String url = RestMethods.GET_LICENSE_DETAILS.getMethodName();
		try {
			RestResponse<License> restResponse = getRestResponse(url, new TypeReference<License>() {});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_LICENSE_DETAILS, x, url);
		}
	}

	public CMResponse addLicense(String licenseString) throws RestException {

		String url = RestMethods.ADD_LICENSE.getMethodName();
		try {
			RestResponse<CMResponse> restResponse = getPostRestResponse(
					url,
					licenseString,
					new TypeReference<CMResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	/**
	 * Method to fetch cm environment details
	 * @return
	 * @throws RestException
	 */
	public CMEnvironment getCMEnvironment() throws RestException {
		String url = RestMethods.GET_CM_ENVIRONMENT.getMethodName();
		try {
			RestResponse<CMEnvironment> restResponse = getRestResponse(url, new TypeReference<CMEnvironment>() {});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	/**
	 * Method to fetch environment Alert details
	 * @return
	 * @throws RestException
	 */
	public CMAlertsSummary getEnvironmentAlerts(int days) throws RestException {
		String url = RestMethods.GET_ENVIRONMENT_ALERTS.getMethodName() + QSTN + INTERVAL + days;
		try {
			RestResponse<CMAlertsSummary> restResponse = getRestResponse(
					url,
					new TypeReference<CMAlertsSummary>() {});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMInstance getInstanceDetails(long instId) throws RestException {
		String url = RestMethods.GET_INSTANCE_DETAILS.getMethodName() +
				QSTN + ID + instId;
		try {
			RestResponse<CMInstance> restResponse = getRestResponse(
					url,
					new TypeReference<CMInstance>() {});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}


	public List<AuditedInstanceBE> getAuditedInstancesWidgetData()
			throws RestException {
		String url = RestMethods.Get_AUDITED_INSTANCES_WIDGET_DATA.getMethodName();
		try {
			RestResponse<List<AuditedInstanceBE>> restResponse = getRestResponse(
					url,new TypeReference<List<AuditedInstanceBE>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMInstance getInstanceDetails(long instId, int days) throws RestException {
		String url = RestMethods.GET_INSTANCE_DETAILS.getMethodName() +
				QSTN + ID + instId + AMP + INTERVAL + days;
		try {
			RestResponse<CMInstance> restResponse = getRestResponse(
					url,
					new TypeReference<CMInstance>() {});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<StatisticDataResponse> getInstanceStatsData(long instId, int days,
			int categoryId) throws RestException {
		String url = RestMethods.GET_INSTANCE_STATICS.getMethodName() +
				QSTN + INSTANCE_ID + instId + AMP + INTERVAL +
				days + AMP + CATEGORY + categoryId;
		return getStatsData(url);
	}

	public List<StatisticDataResponse> getEnvironmentStatsData(int days, int categoryId) throws RestException {
		String url = RestMethods.GET_INSTANCE_STATICS.getMethodName() +
				QSTN + INTERVAL + days + AMP + CATEGORY + categoryId;
		return getStatsData(url);
	}

	public List<StatisticDataResponse> getRecentDatabaseActivity(Long instanceId, int days, Long databaseId) throws RestException {
		String url = RestMethods.GET_RECENT_DATABASE_ACTIVITY.getMethodName() +
				QSTN + INSTANCE_ID + instanceId  + AMP + DATABASE_ID + databaseId + AMP +INTERVAL + days;
		return getStatsData(url);

	}

	private List<StatisticDataResponse> getStatsData(String url) throws RestException {
		try {
			RestResponse<List<StatisticDataResponse>> restResponse = getRestResponse(
					url,
					new TypeReference<List<StatisticDataResponse>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMBeforeAfterDataEventsResponse getInstanceAuditEvents(long instId, int days, int category) throws RestException {
		String url = RestMethods.GET_EVENTS_BY_CATEGORY_AND_DATE_FOR_INSTANCE.getMethodName() + QSTN +
				INSTANCE_ID + instId + AMP + INTERVAL + days + AMP + CATEGORY + category;
		try {
			RestResponse<CMBeforeAfterDataEventsResponse> restResponse = getRestResponse(
					url,
					new TypeReference<CMBeforeAfterDataEventsResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMBeforeAfterDataEventsResponse getFullInstanceAuditEvents(Map<String, Object> filterRequest)
			throws RestException {
		String url = RestMethods.GET_AUDITED_EVENTS.getMethodName();
		try {
			RestResponse<CMBeforeAfterDataEventsResponse> restResponse = getPostRestResponse(
					url, filterRequest,
					new TypeReference<CMBeforeAfterDataEventsResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMBeforeAfterDataEventsResponse getArchivedEvents(Map<String, Object> filterRequest)
			throws RestException {
		String url = RestMethods.GET_ARCHIVED_EVENTS.getMethodName();
		try {
			RestResponse<CMBeforeAfterDataEventsResponse> restResponse = getPostRestResponse(
					url, filterRequest,
					new TypeReference<CMBeforeAfterDataEventsResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMAuditedDatabase getDatabaseDetails(String instanceId, String databaseId)
			throws RestException {
		String url = RestMethods.GET_DATABASE_DETAILS.getMethodName() + QSTN
				+ ID + databaseId + AMP + SERVER_ID + instanceId;
		try {
			RestResponse<CMAuditedDatabase> restResponse = getRestResponse(
					url,
					new TypeReference<CMAuditedDatabase>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<CMEvent> getDatabaseAuditEvents(String instanceId,
			String databaseId, int days) throws RestException {
		String url = RestMethods.GET_DATABASE_AUDIT_EVENTS.getMethodName() + QSTN +
				INSTANCE_ID + instanceId + AMP +
				DATABASE_ID + databaseId + AMP + INTERVAL + days;
		try {
			RestResponse<List<CMEvent>> restResponse = getRestResponse(
					url,
					new TypeReference<List<CMEvent>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<CMInstance> getInstances() throws RestException {
		String url = RestMethods.GET_FILTERED_AUDITED_INSTANCES_STATUS.getMethodName();
		try {
			RestResponse<List<CMInstance>> restResponse = getPostRestResponse(
					url,
					new HashMap<>(), // Return all instances
					new TypeReference<List<CMInstance>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<AuditedInstanceBE> getAllAuditedInstances(Map<String, Object> filterRequest)
			throws RestException {
		String url = RestMethods.GET_FILTERED_AUDITED_INSTANCES_STATUS.getMethodName();
		try {
			RestResponse<List<AuditedInstanceBE>> restResponse = getPostRestResponse(
					url, filterRequest,
					new TypeReference<List<AuditedInstanceBE>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMEventProperties getEventProperties(String instanceId, String eventId, String eventDatabase) throws RestException {
		String url = RestMethods.GET_EVENT_PROPERTIES.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("serverId", instanceId);
		args.put("eventId", eventId);
		args.put("eventDatabase", eventDatabase);
		try {
			RestResponse<CMEventProperties> restResponse = getPostRestResponse(
					url,
					args,
					new TypeReference<CMEventProperties>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMFilteredAlertsResponse getFilteredAlerts(Map<String, Object> filterRequest) throws RestException {
		String url = RestMethods.GET_FILTERED_ALERTS.getMethodName();
		try {
			RestResponse<CMFilteredAlertsResponse> restResponse = getPostRestResponse(url, filterRequest,
					new TypeReference<CMFilteredAlertsResponse>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<CMAlert> getAlerts(long instanceId, long alertType, long alertLevel, long pageSize, long page) throws RestException {
		StringBuilder sb = new StringBuilder(RestMethods.GET_ALERTS.getMethodName());
		boolean firstParameter = true;
		sb.append(QSTN).append(INSTANCE_ID).append(instanceId);
		firstParameter = false;
		sb.append(firstParameter ? QSTN : AMP).append(ALERT_TYPE).append(alertType);
		sb.append(AMP).append(ALERT_LEVEL).append(alertLevel);
		sb.append(AMP).append(PAGE_SIZE).append(pageSize);
		sb.append(AMP).append(PAGE).append(page);
		String url = sb.toString();

		try {
			RestResponse<List<CMAlert>> restResponse = getRestResponse(url, new TypeReference<List<CMAlert>>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<AlertGroupDTO> getAlertsGroups(long instanceId) throws RestException {
		String url = RestMethods.GET_ALERTS_GROUPS.getMethodName();
		if (instanceId != Long.MIN_VALUE) { // if instanceId == Long.MIN_VALUE means return all alerts
			url = url.concat(QSTN).concat(INSTANCE_ID + instanceId);
		}
		try {
			RestResponse<List<AlertGroupDTO>> restResponse = getRestResponse(url, new TypeReference<List<AlertGroupDTO>>() {});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void dismissAlerts(List<Long> alerts) throws RestException {
		String url = RestMethods.DISMISS_ALERTS.getMethodName();
		try {
			getPostRestResponse(url, alerts, new TypeReference<Object>() {});
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void dismissAlertsByGroupAndLevel(Map<String, Object> alertsRequest) throws RestException {
		String url = RestMethods.DISMISS_ALERTS_BY_GROUP_AND_LEVEL.getMethodName();
		try {
			getPostRestResponse(url, alertsRequest, new TypeReference<Object>() {});
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void dismissAlert(Long alert) throws RestException {
		String url = RestMethods.DISMISS_ALERT.getMethodName();
		try {
			getPostRestResponse(url, alert, new TypeReference<Object>() {});
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMEnvironmentDetails getCMEnvironmentDetails() throws RestException {
		String url = RestMethods.GET_ENVIRONMENT_DETAILS.getMethodName();
		try {
			RestResponse<CMEnvironmentDetails> restResponse = getRestResponse(
					url,
					new TypeReference<CMEnvironmentDetails>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public EnvironmentAlerts getAlertsSummary() throws RestException {
		String url = RestMethods.GET_ENVIRONMENT_ALERTS.getMethodName();
		try {
			RestResponse<EnvironmentAlerts> restResponse = getRestResponse(
					url,
					new TypeReference<EnvironmentAlerts>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<CMDatabase> getAuditedDatabasesForInstance(String instanceId) throws RestException {
		String url = RestMethods.GET_AUDITED_DATABASES_FOR_INSTANCE.getMethodName() + QSTN +
				PARENT_ID + instanceId;
		try {
			RestResponse<List<CMDatabase>> restResponse = getRestResponse(
					url,
					new TypeReference<List<CMDatabase>>() { });
			return restResponse.getResultObject();
		} catch (RestException x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<String> getEventsDatabasesForInstance(String instanceId) throws RestException {
		String url = RestMethods.GET_EVENTS_DATABASES_FOR_INSTANCE.getMethodName() + QSTN +
				PARENT_ID + instanceId;
		try {
			log.info("(DEBUG)URL : " + url + " server id : " + instanceId);
			RestResponse<List<String>> restResponse = getRestResponse(
					url,
					new TypeReference<List<String>>() { });
			return restResponse.getResultObject();
		} catch (RestException x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMDatabaseAuditedActivity getAuditedActivityForDatabase(String instanceId,
			String databaseId) throws RestException {
		String url = RestMethods.GET_AUDITED_ACTIVITY_FOR_DATABASE.getMethodName() + QSTN +
				INSTANCE_ID + instanceId + AMP + DATABASE_ID + databaseId;
		try {
			RestResponse<CMDatabaseAuditedActivity> restResponse = getRestResponse(
					url,
					new TypeReference<CMDatabaseAuditedActivity>() {
					});
			return restResponse.getResultObject();
		} catch (RestException x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}
	public CMAuditedActivities getRegulationSettingsForDatabase(RegulationSettings regulationSettings
			) throws RestException {
		String url = RestMethods.GET_REGULATION_SETTINGS_FOR_DATABASE.getMethodName();		
		try {
			RestResponse<CMAuditedActivities> restResponse = getPostRestResponse(
					url,
					regulationSettings,
					new TypeReference<CMAuditedActivities>() {
					});
			return restResponse.getResultObject();
		} catch (RestException x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<RegulationSection> getRegulationSectionDictionary()
			throws RestException {
		String url = RestMethods.GET_REGULATION_SECTION_DICTIONARY.getMethodName();
		try {
			RestResponse<List<RegulationSection>> restResponse = getRestResponse(
					url,
					new TypeReference<List<RegulationSection>>() {});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<RegulationType> getRegulationTypeList()
			throws RestException {
		String url = RestMethods.GET_REGULATION_TYPE_LIST.getMethodName();
		try {
			RestResponse<List<RegulationType>> restResponse = getRestResponse(
					url,
					new TypeReference<List<RegulationType>>() {});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<CMDatabase> getDatabaseList(long instanceId) throws RestException {
		String url = RestMethods.GET_NOT_REGISTERED_DATABASES_FOR_INSTANCE.getMethodName() + QSTN + PARENT_ID + instanceId;
		try {
			RestResponse<List<CMDatabase>> restResponse = getRestResponse(
					url,
					new TypeReference<List<CMDatabase>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMPermissionInfo getPermissionInfo(long instanceId) throws RestException {
		String url = RestMethods.VERIFY_PERMISSIONS.getMethodName() + QSTN + SERVER_ID + instanceId;
		try {
			RestResponse<CMPermissionInfo> restResponse = getRestResponse(
					url,
					new TypeReference<CMPermissionInfo>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public String addInstanceConfig(ServerConfigEntity addDatabasesSaveEntity) throws RestException {
		String url = RestMethods.ADD_DATABASES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(
					url,
					addDatabasesSaveEntity,
					new TypeReference<String>() {
					}
					);
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public static class TableListRequestObject {

		@JsonProperty("serverId")
		private long serverId;

		@JsonProperty("databaseName")
		private String databaseName;

		@JsonProperty("tableNameSearchText")
		private String tableNameSearchText;

		public TableListRequestObject() {
		}

		public TableListRequestObject(long serverId, String databaseName, String tableNameSearchText) {
			this.serverId = serverId;
			this.databaseName = databaseName;
			this.tableNameSearchText = tableNameSearchText;
		}

		public long getServerId() {
			return serverId;
		}

		public void setServerId(long serverId) {
			this.serverId = serverId;
		}

		public String getDatabaseName() {
			return databaseName;
		}

		public void setDatabaseName(String databaseName) {
			this.databaseName = databaseName;
		}

		public String getTableNameSearchText() {
			return tableNameSearchText;
		}

		public void setTableNameSearchText(String tableNameSearchText) {
			this.tableNameSearchText = tableNameSearchText;
		}
	}
	//SQLCM 5.4 Start

	public static class SelectedStringListRequestObject {

		@JsonProperty("selectedList")
		private ProfilerObject selectedList;

		public ProfilerObject getSelectedList() {
			return selectedList;
		}

		public void setSelectedList(ProfilerObject selectedList) {
			this.selectedList = selectedList;
		}

		public SelectedStringListRequestObject() {
		}

		public SelectedStringListRequestObject(ProfilerObject selectedList) {
			this.selectedList = selectedList;
		}
	}

	public static class TableDetailsRequestObject {
		@JsonProperty("databaseName")
		private String databaseName;


		@JsonProperty("tableName")
		private String tableName;

		@JsonProperty("serverId")
		private long serverId;



		public long getServerId() {
			return serverId;
		}

		public void setServerId(long serverId) {
			this.serverId = serverId;
		}

		public String getTableName() {
			return tableName;
		}

		public void setTableName(String tableName) {
			this.tableName = tableName;
		}

		public TableDetailsRequestObject() {
		}

		public TableDetailsRequestObject(String databaseName, String tableName, long serverId) {

			this.databaseName = databaseName;
			this.tableName = tableName;
			this.serverId = serverId;
		}
		public String getDatabaseName() {
			return databaseName;
		}

		public void setDatabaseName(String databaseName) {
			this.databaseName = databaseName;
		}

	}

	public static class TableDetailsRequestObjectForAll {
		@JsonProperty("databaseList")
		private List databaseList;

		@JsonProperty("serverId")
		private long serverId;

		@JsonProperty("profileName")
		private String profileName;


		public String getProfileName() {
			return profileName;
		}

		public void setProfileName(String profileName) {
			this.profileName = profileName;
		}

		public long getServerId() {
			return serverId;
		}

		public void setServerId(long serverId) {
			this.serverId = serverId;
		}


		public TableDetailsRequestObjectForAll() {
		}

		public TableDetailsRequestObjectForAll(List databaseList, long serverId, String profileName) {

			this.databaseList = databaseList;
			this.serverId = serverId;
			this.profileName = profileName;
		}

		public List getDatabaseList() {
			return databaseList;
		}

		public void setDatabaseList(List databaseList) {
			this.databaseList = databaseList;
		}

	}


	//SQLCM 5.4 END

	public List<CMTable> getTables(long instanceId, String databaseName, String filterText) throws RestException {
		String url = RestMethods.GET_DATABASE_TABLE_LIST.getMethodName();
		try {
			RestResponse<List<CMTable>> restResponse = getPostRestResponse(
					url,
					new TableListRequestObject(instanceId, databaseName, filterText),
					new TypeReference<List<CMTable>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<String> getColumns(long databaseId, String tableName) throws RestException {
		String url = null;
		try {
			url = URLEncoder.encode(RestMethods.GET_COLUMN_LIST.getMethodName()
					+ QSTN + DATABASE_ID + databaseId + AMP + TABLE_NAME
					+ tableName, "UTF-8");
			RestResponse<List<String>> restResponse = getRestResponse(
					url,
					new TypeReference<List<String>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMInstanceUsersAndRoles getInstanceUsersAndRoles(long instId) throws RestException {
		String url = RestMethods.GET_SERVER_ROLE_USERS_FOR_INSTANCE.getMethodName() + QSTN + SERVER_ID + instId;
		try {
			RestResponse<CMInstanceUsersAndRoles> restResponse = getRestResponse(
					url,
					new TypeReference<CMInstanceUsersAndRoles>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void changeAuditingForDatabase(CMDatabaseAuditingRequest cmDatabaseAuditingRequest) throws RestException {
		String url = RestMethods.ENABLE_AUDITING_FOR_DATABASES.getMethodName();
		try {
			getPostRestResponse(url, cmDatabaseAuditingRequest,	null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void removeDatabase(Map databaseMap) throws RestException {
		String url = RestMethods.REMOVE_DATABASE.getMethodName();
		try {
			getPostRestResponse(url, databaseMap, null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMDatabaseProperties getDatabaseProperties(Long databaseId) throws RestException {
		String url = RestMethods.GET_DATABASE_PROPERTIES.getMethodName()+ QSTN + DATABASE_ID + databaseId;
		try {
			RestResponse<CMDatabaseProperties> restResponse = getRestResponse(url, new TypeReference<CMDatabaseProperties>() {});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public String updateDatabaseProperties(CMDatabaseProperties databaseProperties) throws RestException {
		String url = RestMethods.UPDATE_DATABASE_PROPERTIES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url, databaseProperties, new TypeReference<String>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void changeAuditingForServers(CMServerAuditingRequest cmServerAuditingRequest) throws RestException {
		String url = RestMethods.ENABLE_AUDITING_FOR_SERVERS.getMethodName();
		try {
			getPostRestResponse(url, cmServerAuditingRequest, null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMInstanceProperties getAuditedInstanceProperties(Long instanceId) throws RestException {
		String url = RestMethods.GET_AUDIT_INSTANCE_PROPERTIES.getMethodName() + QSTN + SERVER_ID + instanceId;
		try {
			RestResponse<CMInstanceProperties> restResponse = getRestResponse(
					url,
					new TypeReference<CMInstanceProperties>() {
					});
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public String updateAuditConfigurationForServer(long instanceId) throws RestException {
		String url = RestMethods.UPDATE_AUDIT_CONFIGURATION_FOR_SERVER.getMethodName();
		Map<String, Long> requestMap = new HashMap<>();
		requestMap.put("serverId", instanceId);
		try {
			RestResponse<String> restResponse = getPostRestResponse(
					url,
					requestMap,
					new TypeReference<String>() {
					}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public void saveViewSettings(CMViewSettings settings) throws RestException {
		String url = RestMethods.SET_VIEW_SETTINGS.getMethodName();
		try {
			getPostRestResponse(url, settings, null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMViewSettings getViewSettings(String viewName) throws RestException {
		String url = RestMethods.GET_VIEW_SETTINGS.getMethodName() + QSTN + VIEW_NAME + viewName;
		try {
			RestResponse<CMViewSettings> restResponse = getRestResponse(
					url,
					new TypeReference<CMViewSettings>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMCLRStatus enableCLR(CMCLRStatus cmclrStatus) throws RestException {
		String url = RestMethods.ENABLE_CLR.getMethodName();
		try {
			RestResponse<CMCLRStatus> restResponse = getPostRestResponse(url, cmclrStatus, new TypeReference<CMCLRStatus>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public String updateAuditServerProperties(CMInstanceProperties properties) throws RestException {
		String url = RestMethods.UPDATE_AUDIT_SERVER_PROPERTIES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(
					url,
					properties,
					new TypeReference<String>() {
					}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public CMAgentProperties getAgentProperties(Long instanceId) throws RestException {
		String url = RestMethods.GET_AGENT_PROPERTIES.getMethodName() + QSTN + SERVER_ID + instanceId;
		try {
			RestResponse<CMAgentProperties> restResponse = getRestResponse(
					url,
					new TypeReference<CMAgentProperties>() {
					});
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public String updateAgentProperties(CMAgentProperties properties) throws RestException {
		String url = RestMethods.UPDATE_AGENT_PROPERTIES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(
					url,
					properties,
					new TypeReference<String>() {
					}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public CMUpgradeAgentResponse upgradeAgent(Long instanceId, String account, String password) throws RestException {
		String url = RestMethods.UPGRADE_AGENT.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("serverId", instanceId);
		args.put("account", account);
		args.put("password", password);
		try {
			RestResponse<CMUpgradeAgentResponse> restResponse = getPostRestResponse(
					url,
					args,
					new TypeReference<CMUpgradeAgentResponse>() {
					}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public CMCheckAgentStatusResult checkAgentStatus(String instanceName) throws RestException {
		String url = RestMethods.CHECK_AGENT_STATUS.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("instance", instanceName);
		try {
			RestResponse<CMCheckAgentStatusResult> restResponse = getPostRestResponse(
					url,
					args,
					new TypeReference<CMCheckAgentStatusResult>() {
					}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public List<CMArchivedDatabase> getArchivesList(String id) throws RestException {
		String url = RestMethods.GET_ARCHIVES_LIST.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("instance", id);
		try {
			RestResponse<List<CMArchivedDatabase>> restResponse = getPostRestResponse(
					url,
					args,
					new TypeReference<List<CMArchivedDatabase>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMArchiveProperties getArchiveProperties(String archiveName) throws RestException {
		String url = RestMethods.GET_ARCHIVE_PROPERTIES.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("archive", archiveName);
		try {
			RestResponse<CMArchiveProperties> restResponse = getPostRestResponse(
					url,
					args,
					new TypeReference<CMArchiveProperties>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void updateArchiveProperties(CMUpdatedArchiveProperties archiveProperties) throws RestException {
		String url = RestMethods.UPDATE_ARCHIVE_PROPERTIES.getMethodName();
		try {
			getPostRestResponse(url, archiveProperties, null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void detachArchive(CMArchive archive) throws RestException {
		String url = RestMethods.DETACH_ARCHIVE.getMethodName();
		try {
			getPostRestResponse(url, archive, null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<CMAttachArchive> getDatabasesForArchiveAttachment(long parentId, boolean showAll) throws RestException {
		String url = RestMethods.GET_DATABASES_FOR_ARCHIVE_ATTACHMENT.getMethodName() + QSTN + PARENT_ID + parentId + AMP + SHOW_ALL + showAll;
		try {
			RestResponse<List<CMAttachArchive>> restResponse = getRestResponse(
					url,
					new TypeReference<List<CMAttachArchive>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void attachArchive(String archiveName) throws RestException {
		String url = RestMethods.ATTACH_ARCHIVE.getMethodName();
		try {
			getPostRestResponse(url, archiveName, null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void upgradeEventDatabaseSchema(String archiveName) throws RestException {
		String url = RestMethods.UPGRADE_EVENT_DATABASE_SCHEMA.getMethodName();
		try {
			getPostRestResponse(url, archiveName, null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public Boolean checkNeedIndexUpdatesForArchive(String archiveName) throws RestException {
		String url = RestMethods.CHECK_NEED_INDEX_UPDATES_FOR_ARCHIVE.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("archive", archiveName);
		try {
			RestResponse<Boolean> restResponse = getPostRestResponse(
					url,
					args,
					new TypeReference<Boolean>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void updateIndexesForEventDatabase(String archiveName) throws RestException {
		String url = RestMethods.UPDATE_INDEXES_FOR_EVENT_DATABASE.getMethodName();
		try {
			getPostRestResponse(url, archiveName, null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public Boolean isIndexStartTimeForArchiveDatabaseIsValid() throws RestException {
		String url = RestMethods.IS_INDEX_START_TIME_FOR_ARCHIVE_DATABASE_IS_VALID.getMethodName();
		try {
			RestResponse<Boolean> restResponse = getRestResponse(
					url,
					new TypeReference<Boolean>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void applyReindexForAttach(CMApplyReindexForArchiveRequest archive) throws RestException {
		String url = RestMethods.APPLY_REINDEX_FOR_ARCHIVE.getMethodName();
		try {
			getPostRestResponse(url, archive, null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<CMRemoveInstanceResponse> removeAuditServers(List serverIdList, boolean deleteEventsDatabase) throws RestException {
		String url = RestMethods.REMOVE_AUDIT_SERVERS.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("serverIdList", serverIdList);
		args.put("deleteEventsDatabase", deleteEventsDatabase);
		try {
			RestResponse<List<CMRemoveInstanceResponse>> restResponse = getPostRestResponse(
					url,
					args,
					new TypeReference<List<CMRemoveInstanceResponse>>() {}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public List<String> getAllNotRegisteredInstanceNameList() throws RestException {
		String url = RestMethods.GET_ALL_NOT_REGISTERED_INSTANCE_NAME_LIST.getMethodName();
		try {
			RestResponse<List<String>> restResponse = getRestResponse(
					url,
					new TypeReference<List<String>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public boolean validateDomainCredentials(AgentServiceAccount agentServiceAccount) throws RestException  {
		String url = RestMethods.VALIDATE_DOMAIN_CREDENTIALS.getMethodName();
		try {
			RestResponse<Boolean> restResponse = getPostRestResponse(
					url,
					agentServiceAccount,
					new TypeReference<Boolean>() {}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public ServerRegisteredStatusInfo checkServerRegisteredStatusByName(String name) throws RestException  {
		String url = RestMethods.CHECK_SERVER_REGISTERED_STATUS_BY_NAME.getMethodName();
		ServerRegisteredStatusInfoRequestArg arg = new ServerRegisteredStatusInfoRequestArg(name);
		try {
			RestResponse<ServerRegisteredStatusInfo> restResponse = getPostRestResponse(
					url,
					arg,
					new TypeReference<ServerRegisteredStatusInfo>() {}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public List<AddInstanceResult> addServers(List<AddServerEntity> serverEntityList) throws RestException  {
		String url = RestMethods.ADD_SERVERS.getMethodName();
		try {
			RestResponse<List<AddInstanceResult>> restResponse = getPostRestResponse(
					url,
					serverEntityList,
					new TypeReference<List<AddInstanceResult>>() {}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public boolean canAddOneMoreInstance() throws RestException {
		String url = RestMethods.CAN_ADD_ONE_MORE_INSTANCE.getMethodName();
		try {
			RestResponse<Boolean> restResponse = getRestResponse(
					url,
					new TypeReference<Boolean>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public AgentDeploymentProperties getAgentDeploymentPropertiesForInstance(String instanceName) throws RestException  {
		String url = RestMethods.GET_AGENT_DEPLOYMENT_PROPERTIES_FOR_INSTANCE.getMethodName();
		HashMap<String, String> arg = new HashMap();
		arg.put("instance", instanceName);
		try {
			RestResponse<AgentDeploymentProperties> restResponse = getPostRestResponse(
					url,
					arg,
					new TypeReference<AgentDeploymentProperties>() {}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public InstanceAvailableResult isInstanceAvailable(String instanceName) throws RestException {
		String url = RestMethods.IS_INSTANCE_AVAILABLE.getMethodName();
		HashMap<String, String> arg = new HashMap();
		arg.put("instance", instanceName);
		try {
			RestResponse<InstanceAvailableResult> restResponse = getPostRestResponse(
					url,
					arg,
					new TypeReference<InstanceAvailableResult>() {}
					);
			return restResponse.getResultObject();
		} catch (Exception ex) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, ex, url);
		}
	}

	public CMBeforeAfterDataEventsResponse getEventsByIntervalForDatabase(long instanceId, long databaseId, int days, int activePage,
			Integer pageSize, int sortDirection, String sortColumn)
					throws RestException {
		String url = RestMethods.GET_EVENTS_BY_INTERVAL_FOR_DATABASE.getMethodName() + QSTN + SERVER_ID + instanceId + AMP
				+ DATABASE_ID + databaseId + AMP + INTERVAL + days + AMP + PAGE + activePage + AMP + PAGE_SIZE + pageSize + AMP
				+ SORT_COLUMN + sortColumn + AMP + SORT_DIRECTION + sortDirection;
		try {
			RestResponse<CMBeforeAfterDataEventsResponse> restResponse = getRestResponse(
					url,
					new TypeReference<CMBeforeAfterDataEventsResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMBeforeAfterDataEventsResponse getEventsByIntervalForInstance(long instanceId, int days, int activePage,
			Integer pageSize, int sortDirection, String sortColumn)
					throws RestException {
		String url = RestMethods.GET_EVENTS_BY_INTERVAL_FOR_INSTANCE.getMethodName() + QSTN + INSTANCE_ID + instanceId
				+ AMP + INTERVAL + days + AMP + PAGE + activePage + AMP + PAGE_SIZE + pageSize + AMP
				+ SORT_COLUMN + sortColumn + AMP + SORT_DIRECTION + sortDirection;
		try {
			RestResponse<CMBeforeAfterDataEventsResponse> restResponse = getRestResponse(
					url,
					new TypeReference<CMBeforeAfterDataEventsResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void createUpdateUser(CMUserSettings user) throws RestException {
		String url = RestMethods.CREATE_UPDATE_USER.getMethodName();
		try {
			getPostRestResponse(url, user, null);
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<CMUserSettings> getAllCMUserSettings() throws RestException {
		String url = RestMethods.GET_ALL_CM_USERS.getMethodName();
		try {
			RestResponse<List<CMUserSettings>> restResponse = getRestResponse(
					url,
					new TypeReference<List<CMUserSettings>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMUserSettings getCMUserSettingsByID(long dashboardUserId) throws RestException {
		String url = RestMethods.GET_CM_USER_SETTINGS_BY_ID.getMethodName() + QSTN + "dashboardUserId=" + dashboardUserId;
		try {
			RestResponse<CMUserSettings> restResponse = getRestResponse(
					url,
					new TypeReference<CMUserSettings>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void deleteCMUserSettingsByIDs(Set<Long> dashboardUserIdList) throws RestException {
		String url = RestMethods.DELETE_USER_SETTINGS.getMethodName();
		Map<String, Object> args = new HashMap<>();
		//TODO KM: fix typo
		args.put("dashbloardUserIds", dashboardUserIdList);
		try {
			getPostRestResponse(url, args, null);
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<AddInstanceResult> importInstances(Set<String> instanceNameList) throws RestException {
		String url = RestMethods.IMPORT_SERVER_INSTANCES.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("InstanceList", instanceNameList);
		RestResponse<List<AddInstanceResult>> restResponse = getPostRestResponse(
				url,
				args,
				new TypeReference<List<AddInstanceResult>>() {
				}
				);
		return restResponse.getResultObject();
	}

	public CMManagedInstances getManagedInstances(CMPageSortRequest sortRequest) throws RestException {
		String url = RestMethods.GET_MANAGED_INSTANCES.getMethodName();
		RestResponse<CMManagedInstances> restResponse = getPostRestResponse(
				url,
				sortRequest,
				new TypeReference<CMManagedInstances>() {
				});
		return restResponse.getResultObject();
	}

	public CMManagedInstanceDetails getManagedInstance(long id) throws RestException {
		String url = RestMethods.GET_MANAGED_INSTANCE.getMethodName() + QSTN + ID + id;
		RestResponse<CMManagedInstanceDetails> restResponse = getRestResponse(
				url,
				new TypeReference<CMManagedInstanceDetails>() {
				});
		return restResponse.getResultObject();
	}

	public List<CMValidateInstanceCredentialResult> validateInstanceCredentials(CMManageInstanceCredentials credentials, List<Long> instanceIdList) throws RestException {
		String url = RestMethods.VALIDATE_INSTANCE_CREDENTIALS.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("credentials", credentials);
		args.put("instancesIds", instanceIdList);
		RestResponse<List<CMValidateInstanceCredentialResult>> restResponse = getPostRestResponse(
				url,
				args,
				new TypeReference<List<CMValidateInstanceCredentialResult>>() {
				});
		return restResponse.getResultObject();
	}

	public void updateManagedInstancesCredentials(CMManageInstanceCredentials credentials, List<Long> instanceIdList) throws RestException {
		String url = RestMethods.UPDATE_MANGED_INSTANCES_CREDENTIALS.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("credentials", credentials);
		args.put("instancesIds", instanceIdList);
		getPostRestResponse(url, args, null);
	}

	public void updateManagedInstance(CMManagedInstance managedInstance) throws RestException {
		String url = RestMethods.UPDATE_MANAGE_INSTANCE.getMethodName();
		getPostRestResponse(url, managedInstance, null);
	}

	public List<CMAvailabilityInfo> getAvailabilityInfoList(List<CMDatabase> databaseList) throws RestException {
		String url = RestMethods.GET_DATABASE_AVAILABILITY_GROUPS.getMethodName();
		try {
			RestResponse<List<CMAvailabilityInfo>> restResponse = getPostRestResponse(
					url,
					databaseList,
					new TypeReference<List<CMAvailabilityInfo>>() {}
					);
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public String addAlertRules(AddAlertRulesSaveEntity addAlertRulesSaveEntity)
			throws RestException {
		String url = RestMethods.ADD_DATABASES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,
					addAlertRulesSaveEntity, new TypeReference<String>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public String addDataAlertRules(
			AddDataAlertRulesSaveEntity addDataAlertRulesSaveEntity)
					throws RestException {
		String url = RestMethods.ADD_DATABASES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,
					addDataAlertRulesSaveEntity, new TypeReference<String>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public String addStatusAlertRules(
			AddStatusAlertRulesSaveEntity addStatusAlertRulesSaveEntity)
					throws RestException {
		String url = RestMethods.ADD_DATABASES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,
					addStatusAlertRulesSaveEntity, new TypeReference<String>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public String addDatabases(AddDatabasesSaveEntity addDatabasesSaveEntity)
			throws RestException {
		String url = RestMethods.ADD_DATABASES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,
					addDatabasesSaveEntity, new TypeReference<String>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public List<CMTreeNode> getTreeNodes(long id, NodeType type)
			throws RestException {
		return STABS__.getDatabaseList();
	}

	public CMFilteredActivityLogsResponse getFilteredActivityLogs(
			Map<String, Object> filterRequest) throws RestException {
		String url = RestMethods.GET_FILTERED_ACTIVITY_LOGS.getMethodName();
		try {
			RestResponse<CMFilteredActivityLogsResponse> restResponse = getPostRestResponse(
					url, filterRequest,
					new TypeReference<CMFilteredActivityLogsResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public CMActivityLogs getActivityProperties(String eventId) throws RestException {
		String url = RestMethods.GET_ACTIVITY_PROPERTIES.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("eventId", eventId);
		try {
			RestResponse<CMActivityLogs> restResponse = getPostRestResponse(
					url,
					args,
					new TypeReference<CMActivityLogs>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMFilteredChangeLogsResponse getFilteredChangeLogs(
			Map<String, Object> filterRequest) throws RestException {
		String url = RestMethods.GET_FILTERED_CHANGE_LOGS.getMethodName();
		try {
			RestResponse<CMFilteredChangeLogsResponse> restResponse = getPostRestResponse(
					url, filterRequest,
					new TypeReference<CMFilteredChangeLogsResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public CMChangeLogs getChangeProperties(String eventId) throws RestException {
		String url = RestMethods.GET_CHANGE_PROPERTIES.getMethodName();
		Map<String, Object> args = new HashMap<>();
		args.put("eventId", eventId);
		try {
			RestResponse<CMChangeLogs> restResponse = getPostRestResponse(
					url,
					args,
					new TypeReference<CMChangeLogs>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMFilteredAlertRulesResponse getAlertRulesFilters(
			Map<String, Object> filterRequest) throws RestException {
		String url = RestMethods.GET_ALERTS_RULES.getMethodName();
		try {
			RestResponse<CMFilteredAlertRulesResponse> restResponse = getPostRestResponse(
					url, filterRequest,
					new TypeReference<CMFilteredAlertRulesResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public CMFilterEventFiltersResponse getFilteredEventFilters(
			Map<String, Object> filterRequest) throws RestException {
		String url = RestMethods.GET_FILTERED_ALERTS.getMethodName();
		try {
			RestResponse<CMFilterEventFiltersResponse> restResponse = getPostRestResponse(
					url, filterRequest,
					new TypeReference<CMFilterEventFiltersResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public Collection<InstanceAlert> getInstancesAlerts(RuleType alertType)
			throws RestException {
		String url = "";
		try {
			if (alertType == RuleType.All) {
				url = RestMethods.GET_ALL_INSTANCES_ALERTS.getMethodName();
			} else {
				url = RestMethods.GET_AUDITED_INSTANCES_ALERTS.getMethodName()
						+ QSTN + ALERT_TYPE + alertType.ordinal();
			}
			RestResponse<ArrayList<InstanceAlert>> restResponse = getRestResponse(
					url, new TypeReference<ArrayList<InstanceAlert>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public List<InstanceStatus> getInstancesStatus(EntityType type)
			throws RestException {
		String url = RestMethods.GET_ALL_INSTANCES_STATUS.getMethodName();
		try {
			RestResponse<ArrayList<InstanceStatus>> restResponse = getRestResponse(
					url, new TypeReference<ArrayList<InstanceStatus>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public String deleteInstances(Object[] instances) throws RestException {
		String url = SQLCMRestMethods.DELETE_INSTANCES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,
					instances, new TypeReference<String>() {
			});

			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public void changeAlertRulesStatus(CMAlertRulesEnableRequest cmAlertRulesEnableRequest)
			throws RestException {
		String url = RestMethods.GET_ENABLE_ALERT_RULES.getMethodName();
		try {
			getPostRestResponse(url, cmAlertRulesEnableRequest, null);
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public void delteAlertRules(CMAlertRulesEnableRequest cmAlertRulesEnableRequest)
			throws RestException {
		String url = RestMethods.GET_DELETE_ALERT_RULES.getMethodName();
		try {
			getPostRestResponse(url, cmAlertRulesEnableRequest, null);
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public void getUpdateSNMPConfiguration(UpdateSNMPConfiguration updateSNMPConfiguration)
			throws RestException {
		String url = RestMethods.GET_UPDATE_SNMP_CONFIGURATION.getMethodName();
		try {
			getPostRestResponse(url, updateSNMPConfiguration, null);
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public void insertStatusAlertRules(InsertQueryData insertQueryData)
			throws RestException {
		String url = RestMethods.INSERT_STATUS_ALERT_RULES.getMethodName();
		try {
			getPostRestResponse(url, insertQueryData, null);
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}
	public void InsertEventFilterData (InsertQueryData insertQueryData) throws RestException{
		String url = RestMethods.INSERT_EVENT_FILTER_DATA.getMethodName();
		try {
			getPostRestResponse(url, insertQueryData, null);
		} catch (Exception e) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, e, url);
		}
	}

	public void changeEventFilterStatus (CMAuditedEventFilterEnable cmAuditedEventFilterEnable) throws RestException{
		String url = RestMethods.GET_AUDITED_EVENT_FILTER_ENABLED.getMethodName();
		try {
			getPostRestResponse(url, cmAuditedEventFilterEnable, null);
		} catch (Exception e) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, e, url);
		}
	}

	public void deleteAlertRules (CMDeleteEntity cmDeleteEntity) throws RestException{
		String url = RestMethods.GET_DELETE_ALERT_RULES.getMethodName();
		try {
			getPostRestResponse(url, cmDeleteEntity, null);
		} catch (Exception e) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, e, url);
		}
	}


	public void deleteEventFilter (CMDeleteEntity cmDeleteEntity) throws RestException{
		String url = RestMethods.GET_AUDITED_EVENT_FILTER_DELETE.getMethodName();
		try {
			getPostRestResponse(url, cmDeleteEntity, null);
		} catch (Exception e) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, e, url);
		}
	}

	public CMAuditEventFilterExportResponse exportEventFilter(CMDataByFilterId cmDataByFilterId)
			throws RestException {
		String url = RestMethods.GET_DATA_BY_FILTER_ID.getMethodName();
		try {
			RestResponse<CMAuditEventFilterExportResponse> restResponse = getPostRestResponse(
					url, cmDataByFilterId,
					new TypeReference<CMAuditEventFilterExportResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMFilterEventFiltersResponse getFilterEventFilters(Map<String, Object> filterRequest)
			throws RestException {
		String url = RestMethods.GET_AUDITED_EVENT_FILTER.getMethodName();
		try {
			RestResponse<CMFilterEventFiltersResponse> restResponse = getPostRestResponse(
					url, filterRequest,
					new TypeReference<CMFilterEventFiltersResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMAlertRulesExportResponse exportAlertRules(CMDataByRuleId cmDataByRuleId)
			throws RestException {
		String url = RestMethods.ALERT_RULES_EXPORT_DATA.getMethodName();
		try {
			RestResponse<CMAlertRulesExportResponse> restResponse = getPostRestResponse(
					url, cmDataByRuleId,
					new TypeReference<CMAlertRulesExportResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public String exportAlertRulesById(String ruleIdString)
			throws RestException {
		String url = RestMethods.EXPORT_ALERT_RULES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,ruleIdString,new TypeReference<String>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public String importAlertRules(String xmlPath)
			throws RestException {
		String url = RestMethods.IMPORT_ALERT_RULES.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,xmlPath,new TypeReference<String>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}


	public CMDataAlertRulesInfo getCMDataAlertRulesInfo(CMDataAlertRuleInfoRequest cmDataAlertRuleInfoRequest)
			throws RestException {
		String url = RestMethods.DATA_ALERT_RULES_INFO.getMethodName();
		try {
			RestResponse<CMDataAlertRulesInfo> restResponse = getPostRestResponse(
					url, cmDataAlertRuleInfoRequest,
					new TypeReference<CMDataAlertRulesInfo>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	//requirement 4.1.1.8_start

	public CMAuditListApplicationResponse getAuditReport(CMAuditApplication cmAuditApplication) throws RestException {
		String url = null;
		try {
			url = RestMethods.GET_AUDIT_REPORT_APPLICATION_ACTIVITY.getMethodName();
			RestResponse<CMAuditListApplicationResponse> restResponse = getPostRestResponse(
					url, cmAuditApplication,
					new TypeReference<CMAuditListApplicationResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}


	public CMAuditListConfigurationResponse getAuditConfigurationReport(CMAuditConfiguration cmAuditConfiguration) throws RestException {
		String url = null;
		try {
				url = RestMethods.GET_AUDIT_REPORT_CONFIGURATION_ACTIVITY.getMethodName();
				RestResponse<CMAuditListConfigurationResponse> restResponse = getPostRestResponse(
						url, cmAuditConfiguration,
						new TypeReference<CMAuditListConfigurationResponse>() {
						});
				return restResponse.getResultObject();
			} catch (Exception x) {
				throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
			}
	}

	public  CMAuditIderaDefaultValuesResponse getIderaDatabaseDefaultValues() throws RestException {
		String url = null;
		url = RestMethods.GET_CONFIGURATION_CHECK_DEFAULT_DATABASE.getMethodName();
		try {
			RestResponse<CMAuditIderaDefaultValuesResponse> restResponse = getPostRestResponse(url, null, new TypeReference<CMAuditIderaDefaultValuesResponse>() {
			});
			return restResponse.getResultObject();
		}
		catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}

	}

	public  CMAuditIderaDefaultValuesResponse getIderaDefaultValues() throws RestException {
		String url = null;
		url = RestMethods.GET_CONFIGURATION_CHECK_DEFAULT_SERVER.getMethodName();
		try {
			RestResponse<CMAuditIderaDefaultValuesResponse> restResponse = getPostRestResponse(url, null, new TypeReference<CMAuditIderaDefaultValuesResponse>() {
			});
			return restResponse.getResultObject();
		}
		catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}

	}

	public CMAuditListDMLRespose getAuditDMLActivityReport(CMAuditDML cmAuditDML) throws RestException {
		String url = null;
		try {
			url = RestMethods.GET_AUDIT_REPORT_DML_ACTIVITY.getMethodName();
			RestResponse<CMAuditListDMLRespose> restResponse = getPostRestResponse(
					url, cmAuditDML,
					new TypeReference<CMAuditListDMLRespose>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMAuditListLoginCreationResponse getAuditLoginCreationReport(CMAuditLoginCreation cmAuditLoginCreation) throws RestException {
		String url = null;
		try {
			url = RestMethods.GET_AUDIT_REPORT_LOGIN_CREATION_HISTORY.getMethodName();
			RestResponse<CMAuditListLoginCreationResponse> restResponse = getPostRestResponse(
					url, cmAuditLoginCreation,
					new TypeReference<CMAuditListLoginCreationResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMAuditListLoginDeletionResponse getAuditLoginDeletionReport(CMAuditLoginDeletion cmAuditLoginDeletion) throws RestException {
		String url = null;
		try {
			url = RestMethods.GET_AUDIT_REPORT_LOGIN_DELETION_HISTORY.getMethodName();
			RestResponse<CMAuditListLoginDeletionResponse> restResponse = getPostRestResponse(
					url, cmAuditLoginDeletion,
					new TypeReference<CMAuditListLoginDeletionResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMAuditListPermissionDeniedActivityResponse getAuditPermissionDeniedReport(CMAuditPermissionDenied cmAuditPermissionDenied) throws RestException {
		String url = null;
		try {
			url = RestMethods.GET_AUDIT_REPORT_PERMISSION_DENIED_ACTIVITY.getMethodName();
			RestResponse<CMAuditListPermissionDeniedActivityResponse> restResponse = getPostRestResponse(
					url, cmAuditPermissionDenied,
					new TypeReference<CMAuditListPermissionDeniedActivityResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMAuditListObjectActivityResponse getAuditObjectActivityReport(CMAuditObjectActivity cmAuditObjectActivity)throws RestException {
		String url = null;
		try {
			url = RestMethods.GET_AUDIT_REPORT_OBJECT_ACTIVITY.getMethodName();
			RestResponse<CMAuditListObjectActivityResponse> restResponse = getPostRestResponse(
					url, cmAuditObjectActivity,
					new TypeReference<CMAuditListObjectActivityResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMAuditListUserActivityResponse getAuditUserActivityReport(CMAuditUserActivity cmAuditUserActivity) throws RestException {
		String url = null;
		try {
			url = RestMethods.GET_AUDIT_REPORT_USER_ACTIVITY.getMethodName();
			RestResponse<CMAuditListUserActivityResponse> restResponse = getPostRestResponse(
					url, cmAuditUserActivity,
					new TypeReference<CMAuditListUserActivityResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMAuditListRowCountResponse getAuditRowCountReport(CMAuditRowCount cmAuditRowCount) throws RestException {
		String url = null;
		try {
			url = RestMethods.GET_AUDIT_REPORT_ROW_COUNT.getMethodName();
			RestResponse<CMAuditListRowCountResponse> restResponse = getPostRestResponse(
					url, cmAuditRowCount,
					new TypeReference<CMAuditListRowCountResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMAuditListRegulatoryComplianceResponse getAuditRegulatoryComplianceReport(CMAuditRegulatoryCompliance cmAuditRegulatoryCompliance) throws RestException{
		String url = null;
		try {
			url = RestMethods.GET_AUDIT_REPORT_REGULATORY_COMPLIANCE.getMethodName();
			RestResponse<CMAuditListRegulatoryComplianceResponse> restResponse = getPostRestResponse(
					url, cmAuditRegulatoryCompliance,
					new TypeReference<CMAuditListRegulatoryComplianceResponse>() {
					});
			return restResponse.getResultObject();
		} catch(Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	//requirement 4.1.1.8_end


	public CategoryResponse getCategoryInfo(CategoryRequest categoryRequest)
			throws RestException {
		String url = RestMethods.CATEGORY_INFO.getMethodName();
		try {
			RestResponse<CategoryResponse> restResponse = getPostRestResponse(
					url, categoryRequest,
					new TypeReference<CategoryResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}


	public void getUpdateSNMPThresholdConfiguration(
			UpdateSNMPThresholdConfiguration updateSNMPThresholdConfiguration) {
		String url = RestMethods.GET_UPDATE_SNMP_THRESHOLD_CONFIGURATION.getMethodName();
		try {
			getPostRestResponse(url, updateSNMPThresholdConfiguration, null);
		} catch (Exception x) {
			try {
				throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
						url);
			} catch (RestException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}	

	public void deleteThresholdConfiguration(
			String instanceName) throws RestException{
		String url = RestMethods.DELETE_THRESHOLD_CONFIGURATION.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,instanceName,new TypeReference<String>(){});
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public GetSNMPConfigResponse getSNMPThresholdConfiguration(
			GetSNMPConfiguration getSNMPThresholdConfiguration) 
					throws RestException{
		String url = RestMethods.GET_SNMP_THRESHOLD_CONFIGURATION.getMethodName();
		try {
			RestResponse<GetSNMPConfigResponse> restResponse = getPostRestResponse(
					url, getSNMPThresholdConfiguration,
					new TypeReference<GetSNMPConfigResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(			
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public ViewNameResponse getViewName(String viewId)
			throws RestException {
		String url = RestMethods.GET_VIEW_NAMES.getMethodName() + QSTN + VIEW_ID + viewId;
		try {
			RestResponse<ViewNameResponse> restResponse = getPostRestResponse(
					url, null,
					new TypeReference<ViewNameResponse>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public String importAuditEventFilters(String xmlContent)
			throws RestException {
		String url = RestMethods.IMPORT_AUDIT_EVENT_FILTERS.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,xmlContent,new TypeReference<String>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public String exportEventFiltrById(String ruleIdString)
			throws RestException {
		String url = RestMethods.EXPORT_EVENT_FILTERS.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,ruleIdString,new TypeReference<String>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public SNMPConfigData updateSnmpConfigData() throws RestException {
		String url = RestMethods.UPDATE_SNMP_CONFIG_DATA.getMethodName();
		try {
			RestResponse<SNMPConfigData> restResponse = getPostRestResponse(url,null,new TypeReference<SNMPConfigData>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public boolean checkSnmpAddress(SNMPConfigData snmpConfigData) throws RestException {
		String url = RestMethods.CHECK_SNMP_ADDRESS.getMethodName();
		try {
			RestResponse<Boolean> restResponse = getPostRestResponse(url,snmpConfigData,new TypeReference<Boolean>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	//SQLCM 5.4 start
	public List<CMDatabase> getAllDatabaseList(long instanceId) throws RestException {
		String url = RestMethods.GET_ALL_DATABASES_FOR_INSTANCE.getMethodName() + QSTN + PARENT_ID + instanceId;
		try {
			RestResponse<List<CMDatabase>> restResponse = getRestResponse(
					url,
					new TypeReference<List<CMDatabase>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}


	public List<CMTableDetails> getTablesDetailsForAll( List databaseName, long instanceId, String profileName) throws RestException {
		String url = RestMethods.GET_TABLES_DETAILS_FOR_ALL.getMethodName();
		try {
			RestResponse<List<CMTableDetails>> restResponse = getPostRestResponse(
					url,
					new TableDetailsRequestObjectForAll(databaseName,  instanceId, profileName),
					new TypeReference<List<CMTableDetails>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}	


	public List<CMColumnDetails> getColumnDetailsForAll( List databaseName, long instanceId, String profileName) throws RestException {
		String url = RestMethods.GET_COLUMN_DETAILS_FOR_ALL.getMethodName();
		try {
			RestResponse<List<CMColumnDetails>> restResponse = getPostRestResponse(
					url,
					new TableDetailsRequestObjectForAll(databaseName,  instanceId, profileName ),
					new TypeReference<List<CMColumnDetails>>() {
					});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}	

	public List<ProfilerObject> getProfiles() throws RestException {
		String url = RestMethods.GET_COLUMN_SEARCH_PROFILE.getMethodName();
		try {
			RestResponse<List<ProfilerObject>> restResponse = getRestResponse(
					url,new TypeReference<List<ProfilerObject>>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public String getActiveProfile() throws RestException {
		String url = RestMethods.GET_COLUMN_SEARCH_ACTIVE_PROFILE.getMethodName();
		try {
			RestResponse<String> restResponse = getRestResponse(
					url,new TypeReference<String>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void deleteStringSeleted(ProfilerObject deletionList) throws RestException {
		String url = RestMethods.DELETE_SELECTED_STRING.getMethodName();
		try {
			getPostRestResponse(
					url,deletionList,
					null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void insertNewString(ProfilerObject insertionList) throws RestException {
		String url = RestMethods.INSERT_NEW_STRING.getMethodName();
		try {
			getPostRestResponse(
					url,insertionList,
					null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void updateString(List<ProfilerObject> updateStrings) throws RestException {
		String url = RestMethods.UPDATE_STRING.getMethodName();
		try {
			getPostRestResponse(
					url,updateStrings,
					null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void activateProfile(String profileName) throws RestException {
		String url = RestMethods.OPEN_PROFILE.getMethodName();
		try {
			getPostRestResponse(
					url,profileName,
					null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void resetData() throws RestException {
		String url = RestMethods.RESET_DATA.getMethodName();
		try {
			getPostRestResponse(
					url,null,
					null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void createProfile(List<ProfilerObject> newProfileDetails) throws RestException {
		String url = RestMethods.NEW_PROFILE.getMethodName();
		try {
			getPostRestResponse(
					url,newProfileDetails,
					null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void updateCurrentProfile(List<ProfilerObject> currentProfileDetails) throws RestException {
		String url = RestMethods.UPDATE_CURRENT_PROFILE.getMethodName();
		try {
			getPostRestResponse(
					url,currentProfileDetails,
					null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void updateIsUpdated(String value) throws RestException {
		String url = RestMethods.UPDATE_IS_UPDATED.getMethodName();
		try {
			getPostRestResponse(
					url,value,
					null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public void deleteProfile(String profileName) throws RestException {
		String url = RestMethods.DELETE_PROFILE.getMethodName();
		try {
			getPostRestResponse(
					url,profileName,
					null);
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}


	public String getIsUpdated() throws RestException {
		String url = RestMethods.GET_IS_UPDATED.getMethodName();
		try {
			RestResponse<String> restResponse = getRestResponse(
					url,new TypeReference<String>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}
	// Start SQLCM - 5.4 

	//Start SQLCM 4.1.3.1 - Sensitive Column Import


	public AllSensitiveDetails validateSensitiveColumns(String csvData)throws RestException
	{
		String url = RestMethods.VALIDATE_SENSITIVE_COLUMNS.getMethodName();
		try {
			RestResponse<AllSensitiveDetails> restResponse = getPostRestResponse(url,csvData,new TypeReference<AllSensitiveDetails>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}


	public void saveSensitiveColumnData(AllSensitiveDetails retVal)  throws RestException  {
		// TODO Auto-generated method stub
		String url = RestMethods.SAVE_SENSITIVE_COLUMN_DATA.getMethodName();
		try {
			RestResponse<AllSensitiveDetails> restResponse= getPostRestResponse(url,retVal,new TypeReference<AllSensitiveDetails>(){});

			//return restResponse.getResultObject();
		} catch(Exception e)
		{
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, e, url);
		}
	}


	//End SQLCM 4.1.3.1- Sensitive Column Import

	//Start SQLCM 4.1.4.1
	public AddWizardImportEntity validateAuditSettings(String xmlData)throws RestException
	{
		String url = RestMethods.VALIDATE_AUDIT_SETTINGS.getMethodName();
		try {
			RestResponse<AddWizardImportEntity> restResponse = getPostRestResponse(url,xmlData,new TypeReference<AddWizardImportEntity>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public String ExportServerAuditSettings(String currentInstanceName)throws RestException{
		String url = RestMethods.EXPORT_SERVER_AUDIT_SETTING.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,currentInstanceName,new TypeReference<String>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}		
	}

	public String ExportDatabaseAuditSettings(int database)
			throws RestException {
		String url = RestMethods.EXPORT_DATABASE_AUDIT_SETTING.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,
					database, new TypeReference<String>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public void importDatabaseAuditSetting(
			AddWizardImportEntity wizardSaveEntity) throws RestException{
		String url = RestMethods.IMPORT_DATABASE_AUDIT_SETTING.getMethodName();
		try {
			getPostRestResponse(url,wizardSaveEntity,new TypeReference<AddWizardImportEntity>(){});
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	//End SQLCM 4.1.4.1

	// SQLCM-2172 5.4 Version start
	public String setRefreshDuration(int refreshDuration)
			throws RestException {
		String url = RestMethods.SET_REFRESH_DURATION.getMethodName();
		try {
			RestResponse<CMResponse> restResponse =getPostRestResponse(url, refreshDuration, null);
			return restResponse.getMessage();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public String getRefreshDuration()
			throws RestException {
		String url = RestMethods.GET_REFRESH_DURATION.getMethodName();
		try {
			RestResponse<String> restResponse= getPostRestResponse(url, null, new TypeReference<String>() {
			});			
			System.out.println(restResponse.toString());
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}
	// SQLCM-2172 5.4 Version end
	public String getLocalTime() throws RestException{
		String url = RestMethods.GET_LOCAL_TIME.getMethodName();
		try {
			RestResponse<String> restResponse = getPostRestResponse(url,null,new TypeReference<String>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public boolean isLinqDllLoaded() throws RestException{
		String url = RestMethods.IS_LINQ_DLL_LOADED.getMethodName();
		try {
			RestResponse<Boolean> restResponse = getPostRestResponse(url,null,new TypeReference<Boolean>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<String> ExportDatabaseRegulationAuditSettings(List<CMDatabase> dblist)
			throws RestException {
		String url = RestMethods.EXPORT_DATABASES_REGULATION_AUDIT_SETTING.getMethodName();
		try {
			RestResponse<List<String>> restResponse = getPostRestResponse(url,
					dblist, new TypeReference<List<String>>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x,
					url);
		}
	}

	public RegulationCustomDetail getRegulationSettingsServerLevel(com.idera.sqlcm.entities.RegulationSettings regulationSettings) throws RestException {
		String url = RestMethods.GET_REGULATION_SETTINGS_SERVER_LEVEL.getMethodName();		
		try {
			RestResponse<RegulationCustomDetail> restResponse = getPostRestResponse(
					url,
					regulationSettings,
					new TypeReference<RegulationCustomDetail>() {
					});
			return restResponse.getResultObject();
		} catch (RestException x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<String> getNewDatabaseColumns(String tableName, String instanceName, String databaseName)
			throws RestException {
		String url = null;
		try {
			url = URLEncoder.encode(
					RestMethods.GET_NEW_DATABASE_COLUMN_LIST.getMethodName()
					+ QSTN + TABLE_NAME + tableName + AMP + INSTANCE
					+ instanceName.replace("\\", "%5C") + AMP
					+ DATABASE_NAME + databaseName, "UTF-8");
			RestResponse<List<String>> restResponse = getRestResponse(url, new TypeReference<List<String>>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public CMCLRStatus getServerCLRStatus(long serverId) throws RestException {
		String url = RestMethods.GET_SERVER_CLR_STATUS.getMethodName() + QSTN + SERVER_ID + serverId;
		try {
			RestResponse<CMCLRStatus> restResponse = getRestResponse(url, new TypeReference<CMCLRStatus>() {
			});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<String> ExportServerRegulationAuditSettings(String currentInstanceName)throws RestException{
		String url = RestMethods.EXPORT_SERVER_REGULATION_AUDIT_SETTING.getMethodName();
		try {
			RestResponse<List<String>> restResponse = getPostRestResponse(url,currentInstanceName,new TypeReference<List<String>>(){});
			return restResponse.getResultObject();
		} catch (Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}		
	}

	public CMAuditSettingResponse getConfigAuditSettingsList() throws RestException {
		String url = RestMethods.GET_AUDIT_SETTINGS.getMethodName();	
		try {

			log.info("(DEBUG)URL : " + url + " calling");
			
				RestResponse<CMAuditSettingResponse> restResponse = getPostRestResponse(url,
						null,new TypeReference<CMAuditSettingResponse>(){});
				return restResponse.getResultObject();
			 
			// String[] responseObject = new ObjectMapper().readValue(response, String[].class);
			
		/*	List<String> listObj =  new ArrayList<>();
			listObj.add("All");
			listObj.add("Logins");
			listObj.add("Failed Logins");
			listObj.add("Security Changes (e.g.  GRANT, REVOKE, LOGIN CHANGE PWD)");
			listObj.add("Database Definition (DDL) (e.g.  CREATE or DROP DATABASE)");
			CMAuditSettingResponse configResp = new CMAuditSettingResponse();
			configResp.setAuditSettingsList(listObj);
			return configResp;*/
		} catch (Exception e) {
			log.info("(DEBUG)URL : " + url + " Exception occured");
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, e,url);
		}
	}

	public List<String> GetAllAuditedInstances() throws RestException{
		String url = RestMethods.GET_ALL_AUDITED_INSTANCES.getMethodName();
		try {
			log.info("(DEBUG)URL : " + url);
			RestResponse<List<String>> restResponse = getRestResponse(url, new TypeReference<List<String>>() {});
			return restResponse.getResultObject();
		} catch(Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<String> GetDatabaseByServerName(String serverName) throws RestException{
		String url = RestMethods.GET_DATABASE_BY_SERVER_NAME.getMethodName() + QSTN + SERVER_NAME + serverName;
		try {
			log.info("(DEBUG)URL : " + url + " Server Name : " + serverName);
			RestResponse<List<String>> restResponse = getRestResponse(url, new TypeReference<List<String>>() {});
			return restResponse.getResultObject();
		} catch(Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<String> GetAllAuditSettings() throws RestException{
		String url = RestMethods.GET_ALL_AUDIT_SETTINGS.getMethodName();
		try {
			RestResponse<List<String>> restResponse = getRestResponse(url, new TypeReference<List<String>>() {});
			return restResponse.getResultObject();
		} catch(Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}

	public List<String> GetAllRegulationGuidelines() throws RestException{
		String url = RestMethods.GET_ALL_REGULATION_GUIDELINES.getMethodName();
		try {
			RestResponse<List<String>> restResponse = getRestResponse(url, new TypeReference<List<String>>() {});
			return restResponse.getResultObject();
		} catch(Exception x) {
			throw getRestException(
					SQLCMI18NStrings.ERR_GET_REST_EXCEPTION, x, url);
		}
	}
}



