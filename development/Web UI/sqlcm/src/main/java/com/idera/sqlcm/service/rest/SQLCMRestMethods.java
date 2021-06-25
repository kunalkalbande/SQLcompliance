package com.idera.sqlcm.service.rest;

//TODO KM: clean
@Deprecated
public enum SQLCMRestMethods {

    //Instances Page
    GET_ALL_AUDITED_INSTANCES("/GetFilteredAuditedInstancesStatus"),
    DELETE_INSTANCES("/IInstanceService/DeleteInstance"),

    GET_AVAILABLE_INSTANCES("/IInstanceService/GetAvailableInstances"),
    CHECK_SQL_SERVER_CREDENTIALS("/IInstanceService/CheckSqlServerCredentials"),
    ADD_SERVERS("/IInstanceService/AddServers"),
    GET_INSTANCE_BY_ID("/IInstanceService/GetInstanceById?instanceId="),
    GET_INSTANCES("/IInstanceService/GetFilteredInstances"),;


   /* REGISTER_WIDGETS("/RegisterWidgets"),

    //Dashboard Page
    GET_ALERTS("/IAlertService/Alerts/GetAlerts"),
    DISMISS_ALERT("/IAlertService/Alerts/Dismiss?alertId="),
    DISMISS_ALERTS_GROUP("/IAlertService/Alerts/DismissGroup?alertType="),
    START_ALERTS_REFRESH("/IAlertService/Alerts/StartRefresh"),
    IS_ALERTS_REFRESHING("/IAlertService/Alerts/IsRefreshInProgress"),
    GET_TOP_X_DATABASE_BY_SIZE("/IDatabasesService/GetBigestDatabasesBySize"),
    GET_TOP_X_DATABASE_BY_ALERTS("/IDatabasesService/GetDatabasesByAlertCount"),
    GET_LONGEST_RUNNING_BACKUPS_BY_SIZE("/IBackupJobService/GetLongestRunningBackupJobs"),
    GET_MANAGED_INSTANCES("/IInstanceService/GetManagedInstances"),
    GET_ENVIRONMENT_OVERVIEW("/IEnvironmentService/GetEnvironmentOverview"),
    GET_STATUS_DETAILS("/IEnvironmentService/GetServerStatusToday"),
    GET_DISK_SPACE_SAVINGS("/IEnvironmentService/GetRoiStat"),

    //Restore Wizard
    POST_RESTORE_AGAIN("/IRestoreActions/RestoreAgain"),
    POST_START_RESTORE_WITH_DIFFERENT_OPTIONS("/IRestoreActions/StartRestoreWithDifferentOptions"),
    GET_DEFAULT_RESTORE_MODEL("/IRestoreActions/GetDefaultRestoreModel"),
    POST_START_RESTORE("/IRestoreActions/StartRestore"),
    POST_GENERATE_SCRIPTS("/IRestoreActions/GenerateScripts"),
    GET_BACKUP_FILES_INFO("/IRestoreActions/GetBackupFilesInfo"),
    GET_BACKUPS_FOR_RESTORE("/IRestoreActions/getBackups"),
    GET_RESTORABLE_DATABASES("/IRestoreActions/GetRestorableDatabases"),
    RESTORE_AGAIN("/IRestoreActions/RestoreAgain"),

    //Restore OLR Wizard
    START_RESTORE_OLR("/IObjectLevelRecoveryService/OLR/StartRecovery"),
    VIRTUAL_RESTORE("/IObjectLevelRecoveryService/OLR/VirtualRestoreDatabase"),
    GET_RECOVERY_STATUS("/IObjectLevelRecoveryService/OLR/GetRecoveryStatus"),
    GET_EXTRACTED_DATABASE_MODEL_FROM_VDB("/IObjectLevelRecoveryService/OLR/GetExtractedDatabaseModelFromVdb"),
    CANCEL_RESTORE("/IObjectLevelRecoveryService/OLR/CancelRestore"),

    //Backup Wizard
    GET_DEFAULT_BACKUP_MODEL("/IBackupActions/GetDefaultBackupModel"),
    GET_INSTANCE_DATABASES("/IDatabasesService/GetInstanceDatabases"),
    POST_CHECK_FOLDER_EXIST("/IBackupActions/CheckFolderExists"),
    GET_ENCRYPTION_LIST("/IBackupActions/GetEncryptionList"),
    POST_GET_SCRIPTS("/IBackupActions/GetScript"),
    POST_START_BACKUP("/IBackupActions/StartBackup"),
    BACKUP_AGAIN("/IBackupActions/BackupAgain"),
    BACKUP_WITH_DIFFERENT_OPTIONS("/IBackupActions/StartBackupWithDifferentOptions"),

    //Policies Page
    GET_POLICIES("/IPolicyService/GetFilteredPolicyInformationForPolicesPage"),

    //History Page
    GET_HISTORY("/IActionService/GetActions"),
    GET_OPERATION_DETAILS("/IActionService/GetActionDetails?actionIdInt="),
    SET_OPERATION_STATUS("/IBackupActions/SetProgressTo"),
    CANCEL_OPERATION("/IBackupActions/CancelOperation"),


    //Databases Page
    GET_DATABASES("/IDatabasesService/GetDatabases"),

    //Agents Page
    GET_AGENTS("/IBackupAgentService/GetBackupInfo"),
    REFRESH_AGENTS("/IBackupAgentService/RefreshInfo"),

    //Administration Page
    GET_USER_TIMEOUT("/IUsersTimeoutService/GetTimeout?userId="),
    UPDATE_USER_TIMEOUT("/IUsersTimeoutService/AddOrUpdate"),

    GET_ADMINISTRATION_GENERAL_BASIC_PREFERENCES("/ISettingsService/GetBasicApplicationSettings"),
    UPDATE_ADMINISTRATION_GENERAL_BASIC_PREFERENCES("/ISettingsService/UpdateBasicApplicationSettings"),

    GET_ADMINISTRATION_MANAGEMENT_SERVICE_COMPUTER_NAME("/ISettingsService/GetManagementServiceComputerName"),
    UPDATE_MANAGEMENT_SERVICE_COMPUTER_NAME("/ISettingsService/UpdateManagementServiceComputerName"),
    GET_ADMINISTRATION_REPOSITORY_MANAGEMENT_SERVICE_SETTINGS("/ISettingsService/GetRepositoryDatabaseSettings"),
    UPDATE_ADMINISTRATION_REPOSITORY_MANAGEMENT_SERVICE_SETTINGS("/ISettingsService/UpdateRepositoryDatabaseSettings"),
    GET_TEST_CONNECTION_RESULT("/ISettingsService/TestRepositoryConnection"),

    GET_ADMINISTRATION_GENERAL_BACKUP_PREFERENCES("/ISettingsService/GetBackupApplicationSettings"),
    UPDATE_ADMINISTRATION_GENERAL_BACKUP_PREFERENCES("/ISettingsService/UpdateBackupApplicationSettings"),

    GET_ADMINISTRATION_EMAIL_SETTING("/ISettingsService/GetEmailNotificationApplicationSettings"),
    POST_ADMINISTRATION_EMAIL_SETTING("/ISettingsService/UpdateEmailNotificationApplicationSettings"),
    POST_ADMINISTRATION_EMAIL_SETTING_TEST("/ISettingsService/TestEmailNotificationSettings"),

    GET_LICENSE_MODE("/ILicensingService/GetLicenseMode"),
    GET_MULTI_INSTANCE_LICENSE_INFO("/ILicensingService/GetMultiInstanceLicenseInfo"),
    GET_CENTRALIZED_LICENSE_INFO("/ILicensingService/GetCentralizedLicenseInfo"),
    VALIDATE_CENTRALIZED_LICENSE_KEY("/ILicensingService/ValidateCentralizedLicenseKey"),
    VALIDATE_MULTI_INSTANCE_LICENSE_KEY("/ILicensingService/ValidateMultiInstanceLicenseKey"),
    ADD_CENTRALIZED_LICENSE_KEY("/ILicensingService/AddCentralizedLicenseKey"),
    ADD_MULTI_INSTANCE_LICENSE_KEY("/ILicensingService/AddMultiInstanceLicenseKey"),
    DELETE_CENTRALIZED_LICENSE_KEY("/ILicensingService/DeleteCentralizedLicenseKey"),
    DELETE_MULTI_INSTANCE_LICENSE_KEY("/ILicensingService/DeleteMultiInstanceLicenseKey"),
    UPDATE_CENTRALIZED_LICENSE_INSTANCE("/ILicensingService/UpdateCentralizedLicenseInstance"),
    GET_LICENSE_TOKEN("/ILicensingService/GetLicenseToken?licenseKey="),;*/

    String methodName;

    SQLCMRestMethods(String methodName) {
        this.methodName = methodName;
    }

    public String getMethodName() {
        return methodName;
    }
}
