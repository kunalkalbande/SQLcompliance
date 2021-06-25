package com.idera.sqlcm.i18n;

import mazz.i18n.annotation.I18NMessage;

public interface SQLCMI18NStrings {

	@I18NMessage("{0} currently has no threshold enabled")
	String ACTIVITY_CHART_HEADER = "SQLCM.Labels.activity-chart-header";

	@I18NMessage("Admin")
	String ADMIN = "SQLCM.Labels.admin";
	
	@I18NMessage(" Red box indicates Regulatory Guideline has been changed from recommended settings")
	String RCC_REPORT_NOTE = "SQLCM.Labels.rcc-report-note";

	@I18NMessage("Agent Status")
	String AGENT_STATUS = "SQLCM.Labels.agent-status";

	@I18NMessage("Environment Alert Status")
	String ALERT_DIALOG_HEADER = "SQLCM.Labels.alert-dialog-header";

	@I18NMessage("Alert Status")
	String ALERT_STATUS = "SQLCM.Labels.alert-status";

	@I18NMessage("Alert Type")
	String ALERT_TYPE = "SQLCM.Labels.alert-type";

	@I18NMessage("Audit Configuration")
	String AUDIT_CONFIGURATION = "SQLCM.Labels.audit-configuration";

	@I18NMessage("Audited Databases")
	String AUDITED_DATABASES = "SQLCM.Labels.audited-database";

	@I18NMessage("Apply Guideline(s)")
	String APPLY_GUIDELINES = "SQLCM.Labels.apply_guidelines";

	@I18NMessage("{0} of {1}")
	String AUDITED_DATABASES_VALUE = "SQLCM.Labels.audited-database-value";

	@I18NMessage("Instances")
	String AUDITED_INSTANCES = "SQLCM.Labels.audited-instance";

	@I18NMessage("Administration")
	String ADMINISTRATION = "SQLCM.Labels.administration";

	@I18NMessage("Audited Databases")
	String WIDGET_AUDITED_DATABASES = "SQLCM.Labels.widget.audited-databases";

	@I18NMessage("Audited Servers")
	String WIDGET_AUDITED_SERVERS = "SQLCM.Labels.widget.audited-servers";

	@I18NMessage("Audited SQL Databases")
	String AUDITED_SQL_DATABASES = "SQLCM.Labels.audited-sql-database";

	@I18NMessage("Audited SQL Servers")
	String AUDITED_SQL_SERVER = "SQLCM.Labels.audited-sql-server";

	@I18NMessage("AUDITED SQL SERVER ALERTS")
	String AUDITED_SQL_SERVER_ALETRS = "SQLCM.Labels.audited-sql-server-alerts";

	@I18NMessage("Before-After")
	String BEFORE_AFTER = "SQLCM.Labels.before-after";

	@I18NMessage("Category")
	String CATEGORY = "SQLCM.Labels.Category";

	@I18NMessage("Collected Events")
	String COLLECTED_EVENTS = "SQLCM.Labels.collected-events";

	@I18NMessage("CURRENT AUDITED SQL DATABASE ALERTS")
	String CURRENT_AUDITED_DB_ALETRS = "SQLCM.Labels.current-audited-db-alerts";

	@I18NMessage("Home")
	String DASHBOARD = "SQLCM.Labels.nav-bar-dashboard-label";

	@I18NMessage("Database")
	String DATABASE = "SQLCM.Labels.database";

	@I18NMessage("Database View")
	String DATABASE_VIEW = "SQLCM.Labels.nav-bar-database-view";

	@I18NMessage("DDL")
	String DDL = "SQLCM.Labels.ddl";

	@I18NMessage("DML")
	String DML = "SQLCM.Labels.dml";

	@I18NMessage("Select")
	String SELECT = "SQLCM.Labels.select";

	@I18NMessage("Details")
	String DETAILS = "SQLCM.Labels.details";

	@I18NMessage("Display")
	String DISPLAY = "SQLCM.Labels.display";

	@I18NMessage("Enabled Event Categories")
	String ENABLED_EVENT_CATEGORIES = "SQLCM.Labels.enabled-event-categories";

	@I18NMessage("Current Product is null")
	String ERR_CURRENT_PRODUCT_IS_NULL = "SQLCM.Errors.product-is-null";

	@I18NMessage("An exception occurred getting license details.")
	String ERR_GET_LICENSE_DETAILS = "SQLCM.Errors.get-license-details";

	@I18NMessage("An exception occurred in rest api call.")
	String ERR_GET_REST_EXCEPTION = "SQLCM.Errors.get-rest-exception";

	@I18NMessage("An exception occurred while loading dashboard side bar details")
	String ERR_LOADING_DASHBOARD_SIDE_BAR = "SQLCM.Errors.loading-dashboard-side-bar";

	@I18NMessage("Error")
	String ERROR = "SQLCM.Labels.error";

	@I18NMessage("Error:")
	String ERROR_WITH_COLON = "SQLCM.Labels.error-with-colon";

	@I18NMessage("Warning")
	String WARNING = "SQLCM.Labels.warning";

	@I18NMessage("Information")
	String INFORMATION = "SQLCM.Labels.information";

	@I18NMessage("Confirm")
	String CONFIRMATION = "SQLCM.Labels.confirmation";
	
	@I18NMessage("Refreshing")
	String REFRESHING = "SQLCM.Labels.refreshing";

	@I18NMessage("Event")
	String EVENT = "SQLCM.Labels.event";

	@I18NMessage("All Alerts")
	String ALL_ALERTS = "SQLCM.Labels.all-alert";

	@I18NMessage("Event Alerts")
	String EVENT_ALERTS = "SQLCM.Labels.event-alert";

	@I18NMessage("Event Distribution")
	String EVENT_DISTRIBUTION = "SQLCM.Labels.event-distribution";

	@I18NMessage("Failed Logins")
	String FAILED_LOGIN = "SQLCM.Labels.failed-login";

	@I18NMessage("High")
	String HIGH = "SQLCM.Labels.high";

	@I18NMessage("High Status")
	String HIGH_STATUS = "SQLCM.Labels.high-status";

	@I18NMessage("HISTORICAL ALERTS")
	String HISTORICAL_SQL_SERVER_ALETRS = "SQLCM.Labels.historical-sql-server-alerts";

	@I18NMessage("INSTANCE VIEW")
	String INSTANCE_VIEW = "SQLCM.Labels.instance-view";

	@I18NMessage("Last Archived")
	String LAST_ARCHIVED = "SQLCM.Labels.last-archive";

	@I18NMessage("Last Heartbeat")
	String LAST_HERATBEAT = "SQLCM.Labels.last-heartbeat";

	@I18NMessage("Launch CM Windows Console")
	String LAUNCH_CM_CONSOLE = "SQLCM.Labels.launch-CM-label";

	@I18NMessage("Expire Date: {0}")
	String LICENSE_EXPIRATION_DATE = "SQLCM.Labels.license-expiration-date";

	@I18NMessage("Your license for SQL CM is expired.")
	String LICENSE_EXPIRED_MESSAGE = "SQLCM.Labels.license-expired-message";

	@I18NMessage("License Type: {0}")
	String LICENSE_TYPE = "SQLCM.Labels.license-type";

	@I18NMessage("Low Status")
	String LOW_STATUS = "SQLCM.Labels.low-status";

	@I18NMessage("Low")
	String LOW = "SQLCM.Labels.low";

	@I18NMessage("Medium")
	String MEDIUM = "SQLCM.Labels.medium";

	@I18NMessage("Medium Status")
	String MEDIUM_STATUS = "SQLCM.Labels.medium-status";

	@I18NMessage("My CM Environment")
	String MY_CM_ENVIRONMENT = "SQLCM.Labels.dashboard-side-bar-my-environment-title";

	@I18NMessage("Audited Databases are not available")
	String NO_AUDITED_DATABASES = "SQLCM.Labels.no-audited-databases";

	@I18NMessage("Audited Instances are not available")
	String NO_AUDITED_INSTANCES = "SQLCM.Labels.no-audited-instances";

	@I18NMessage("No data available")
	String NO_DATA_AVAILABLE = "SQLCM.Labels.no-data-available";

	@I18NMessage("None")
	String NONE = "SQLCM.Labels.none";

	@I18NMessage("Never")
	String NEVER = "SQLCM.Labels.never";

	@I18NMessage("There is no historical alerts")
	String NO_HISTORICAL_ALERTS = "SQLCM.Labels.no-historical-alerts";

	@I18NMessage("Registered Instances are not available")
	String NO_INSTANCE_REGISTERED = "SQLCM.Labels.no-instances-registered";

	@I18NMessage("No tables")
	String NO_TABLES = "SQLCM.Labels.no-tables";

	@I18NMessage("Number of Databases")
	String NUMBER_OF_DATABASES = "SQLCM.Labels.number-of-databases";

	@I18NMessage("Ok")
	String OK = "SQLCM.Labels.ok";

	@I18NMessage("Cancel")
	String CANCEL = "SQLCM.Labels.cancel";

	@I18NMessage("Yes")
	String YES = "SQLCM.Labels.yes";

	@I18NMessage("No")
	String NO = "SQLCM.Labels.no";

	@I18NMessage("1 Day")
	String ONE_DAY = "SQLCM.Labels.1-day";

	@I18NMessage("Overall Activity")
	String OVERALL_ACTIVITY = "SQLCM.Labels.overall-activity";

	@I18NMessage("Privileged User (%s)")
	String PRIVILEGED_USER_TITLE = "SQLCM.Labels.privileged-user-title";

	@I18NMessage("Privileged User")
	String PRIVILEGED_USER = "SQLCM.Labels.privileged-user";

	@I18NMessage("Processed Events")
	String PROCESSED_EVENTS = "SQLCM.Labels.processed-event";

	@I18NMessage("IDERA SQL Compliance Manager")
	String PRODUCT_NAME = "SQLCM.Labels.UI.product-name";

	@I18NMessage("Recent Alerts")
	String RECENT_ALERTS = "SQLCM.Labels.recent-alerts";

	@I18NMessage("Recent Events")
	String RECENT_EVENTS = "SQLCM.Labels.recent-events";

	@I18NMessage("Recent Audit Event")
	String RECENT_AUDIT_EVENT = "SQLCM.Labels.recent-audit-event";

	@I18NMessage("Recent Database Activity")
	String RECENT_DATABASE_ACTIVITY = "SQLCM.Labels.recent-database-activity";

	@I18NMessage("Refresh")
	String REFRESH = "SQLCM.Labels.refresh";

	@I18NMessage("Registered SQL Servers")
	String REGISTERED_SQL_SERVERS = "SQLCM.Labels.registered-sql-server";

	@I18NMessage("Regulation Guideline(s)")
	String REGULATION_GUIDELINES = "SQLCM.Labels.regulation-guidelines";

	@I18NMessage("Rule")
	String RULE = "SQLCM.Labels.rule";

	@I18NMessage("Rule Type")
	String RULE_TYPE = "SQLCM.Labels.rule-type";

	@I18NMessage("Security")
	String SECURITY = "SQLCM.Labels.security";

	@I18NMessage("Sensitive Columns")
	String SENSITIVE_COLUMNS = "SQLCM.Labels.sensitive-columns";

	@I18NMessage("Server")
	String SERVER = "SQLCM.Labels.server";

	@I18NMessage("Server Activity Report Card")
	String SERVER_ACTIVITY = "SQLCM.Labels.server-activity";

	@I18NMessage("Enterprise Activity Report Card")
	String ENTERPRISE_ACTIVITY = "SQLCM.Labels.enterprise-activity";

	@I18NMessage("Server Status")
	String SERVER_STATUS = "SQLCM.Labels.server-status";

	@I18NMessage("System Status")
	String SYSTEM_STATUS = "SQLCM.Labels.system-status";

	@I18NMessage("Audited Activity")
	String AUDITED_ACTIVITY = "SQLCM.Labels.audited-activity";

	@I18NMessage("Audited SQL Servers")
	String AUDITED_SQL_SERVERS = "SQLCM.Labels.audited-sql-servers";

	@I18NMessage("7 Days")
	String SEVEN_DAYS = "SQLCM.Labels.7-days";

	@I18NMessage("Severe")
	String SEVERE = "SQLCM.Labels.severe";

	@I18NMessage("Severe Status")
	String SEVERE_STATUS = "SQLCM.Labels.severe-status";

	@I18NMessage("Source Rule")
	String SOURCE_RULE = "SQLCM.Labels.source-rule";

	@I18NMessage("SQL Database")
	String SQL_DATABASE = "SQLCM.Labels.sql-database";

	@I18NMessage("SQL Server")
	String SQL_SERVER = "SQLCM.Labels.sql-server";

	@I18NMessage("SQL Server Alerts")
	String SQL_SERVER_ALERTS = "SQLCM.Labels.sql-server-alerts";

	@I18NMessage("Status")
	String STATUS = "SQLCM.Labels.status";

	@I18NMessage("30 Days")
	String THIRTY_DAY = "SQLCM.Labels.30-day";

	@I18NMessage("Time")
	String TIME = "SQLCM.Labels.time";

	@I18NMessage("Time of Alert")
	String TIME_OF_ALERT = "SQLCM.Labels.time-of-alert";

	@I18NMessage("Time Of Alert Cleared")
	String TIME_OF_ALERT_CLEARED = "SQLCM.Labels.time-of-alert-cleared";

	@I18NMessage("Trusted Users")
	String TRUSTED_USERS = "SQLCM.Labels.trusted-users";

	@I18NMessage("Total")
	String TOTAL = "SQLCM.Labels.total";

	@I18NMessage("Last Updated at %s")
	String UPDATED_AT = "SQLCM.Labels.last-updated-at";

	@I18NMessage("http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+5.0+Home")
	String WELCOME_DIALOG_HELP_LINK = "SQLCM.Labels.welcome-dialog-help-link";

	@I18NMessage("SQL CM product documentation.")
	String WELCOME_DIALOG_HELP_MSG = "SQLCM.Labels.welcome-dialog-help-message";

	@I18NMessage("The IDERA SQL Compliance Manager windows console is required to manage audit settings, add a new SQL Server instance, or manage user permissions.")
	String WELCOME_DIALOG_MSG = "SQLCM.Labels.welcome-dialog-message";

	@I18NMessage("For more information, please see the")
	String WELCOME_DIALOG_SEE_MORE = "SQLCM.Labels.welcome-dialog-see-more";

	@I18NMessage("Audited Instances")
	String INSTANCES_TITLE = "SQLCM.Labels.instances.title";

	@I18NMessage("Filtering")
	String FILTER_TITLE = "SQLCM.Labels.filter-title";

	@I18NMessage("Clear")
	String CLEAR = "SQLCM.Message.clear";

	@I18NMessage("Filtered By")
	String FILTERED_BY = "SQLCM.Labels.filtered-by";

	@I18NMessage("Apply filter as it changes")
	String AUTO_UPDATE = "SQLCM.Labels.auto-update";

	@I18NMessage("Filter changed. Apply now")
	String APPLY_CHANGES_NOW = "SQLCM.Messages.apply-changes-now";

	@I18NMessage("Load View")
	String LOAD_VIEW = "SQLCM.Messages.load-view";

	@I18NMessage("Save View")
	String SAVE_VIEW = "SQLCM.SQLCM.Label.save-view";

	@I18NMessage("Before - After Data")
	String BEFORE_AFTER_DATA = "SQLCM.Messages.before-after-data";

	@I18NMessage("Archive")
	String ARCHIVE = "SQLCM.Labels.archive";

	@I18NMessage("Attach")
	String ATTACH = "SQLCM.Labels.attach";

	@I18NMessage("Detach")
	String DETACH = "SQLCM.Labels.detach";

	@I18NMessage("Views")
	String VIEWS = "SQLCM.Messages.views";

	@I18NMessage("Failed to load registered SQL Server instances.")
	String FAILED_TO_LOAD_MONITORED_INSTANCES = "SQLCM.Messages.failed-to-load-monitored-instances";

	@I18NMessage("An exception occurred reacting to a filter update.")
	String EXCEPTION_OCCURRED_REACTING_TO_FILTER_UPDATE = "Messages.exception-occurred-reacting-to-filter-update";

	/* Instances columns begin */
	@I18NMessage("Status")
	String STATUS_COLUMN = "SQLCM.Labels.instances.status-column";

	@I18NMessage("Instance name")
	String INSTANCE_NAME_COLUMN = "SQLCM.Labels.instances.instance-name-column";

	@I18NMessage("Failed to load data")
	String FAILED_TO_LOAD_DATA = "SQLCM.Labels.failed-to-load-data";

	@I18NMessage("Status Text")
	String STATUS_TEXT_COLUMN = "SQLCM.Labels.instances.status-text-column";

	@I18NMessage("Agent Status Text")
	String AGENT_STATUS_TEXT_COLUMN = "SQLCM.Labels.instances.agent-status-text-column";

	@I18NMessage("Number of Audited DBs")
	String NUMBER_OF_AUDITED_DB_COLUMN = "SQLCM.Labels.instances.num-of-audited-dbs-column";

	@I18NMessage("SQL Server version")
	String SQL_SERVER_VERSION_AND_EDITION_COLUMN = "SQLCM.Labels.instances.sql-server-version-edition-column";

	@I18NMessage("Audit Status")
	String AUDIT_STATUS_COLUMN = "SQLCM.Labels.instances.audit-status-column";

	@I18NMessage("Last agent contact")
	String LAST_AGENT_CONTACT_COLUMN = "SQLCM.Labels.instances.last-agent-contact-column";

	@I18NMessage("")
	String INSTANCES_CHECKED_COLUMN = "SQLCM.Labels.instances.checked-column";

	@I18NMessage("Options")
	String INSTANCES_OPTIONS_COLUMN = "SQLCM.Labels.instances.options-column";
	/* Instances columns end */

	@I18NMessage("Enable Auditing")
	String INSTANCES_OPTION_ENABLE_AUDITING = "SQLCM.Labels.instances.options.enable-auditing";

	@I18NMessage("Disable Auditing")
	String INSTANCES_OPTION_DISABLE_AUDITING = "SQLCM.Labels.instances.options.disable-auditing";

	@I18NMessage("Update Audit Settings")
	String INSTANCES_OPTION_UPDATE_AUDIT_SETTINGS = "SQLCM.Labels.instances.options.update-audit-settings";

	@I18NMessage("Remove")
	String INSTANCES_OPTION_REMOVE = "SQLCM.Labels.instances.options.remove";

	@I18NMessage("Remove/Delete")
	String INSTANCES_OPTION_REMOVE_DELETE = "Labels.sql-cm.instances.actions.remove-delete";

	@I18NMessage("Refresh")
	String INSTANCES_OPTION_REFRESH = "SQLCM.Labels.instances.options.refresh";

	@I18NMessage("Upgrade Agent")
	String INSTANCES_OPTION_UPGRADE_AGENT = "SQLCM.Labels.instances.options.upgrade-agent";

	@I18NMessage("Check Agent Status")
	String INSTANCES_OPTION_CHECK_AGENT_STATUS = "SQLCM.Labels.instances.options.check-agent-status";

	@I18NMessage("Agent Properties")
	String INSTANCES_OPTION_AGENT_PROPERTIES = "SQLCM.Labels.instances.options.agent-properties";

	@I18NMessage("Agent Actions")
	String INSTANCES_OPTION_AGENT_ACTIONS = "SQLCM.Labels.instances.options.agent-actions";

	@I18NMessage("Properties (instance)")
	String INSTANCES_OPTION_INSTANCE_PROPERTIES = "SQLCM.Labels.instances.options.instance-properties";
	
	/*@I18NMessage("Edit Properties")
	String INSTANCES_OPTION_INSTANCE_PROPERTIES1 = "SQLCM.Labels.instances.options.instance-properties";*/

	@I18NMessage("Remove instance name filters")
	String REMOVE_INSTANCE_NAME_FILTERS = "SQLCM.Labels.remove-instance-name-filters";

	@I18NMessage("Remove status text filters")
	String REMOVE_STATUS_TEXT_FILTERS = "SQLCM.Labels.remove-status-text-filters";

	@I18NMessage("Remove {0} Filter")
	String FILTER_REMOVE = "SQLCM.Labels.filter.remove";

	@I18NMessage("From:")
	String FILTER_FROM = "SQLCM.Labels.filter.from";

	@I18NMessage("To:")
	String FILTER_TO = "SQLCM.Labels.filter.to";

	@I18NMessage("Out of Range (5-50)")
	String PAGE_SIZE_ERROR_STATUS = "Labels.sql-cm-listbox.page-size-error_5_50";
	
	@I18NMessage("Out of Range (1-100)")
	String PAGE_SIZE_ERROR = "Labels.sql-cm-listbox.page-size-error";

	@I18NMessage("Database")
	String INSTANCE_DETAIL_EVENTS_DATABASE_COLUMN = "SQLCM.Labels.instance-detail.events.database-column";

	@I18NMessage("Items per page")
	String PAGINATION_ITEMS_PER_PAGE = "Labels.sql-cm-listbox.pagination-items-per-page";

	@I18NMessage("Category")
	String INSTANCE_DETAIL_EVENTS_CATEGORY_COLUMN = "SQLCM.Labels.instance-detail.events.category-column";

	@I18NMessage("Event")
	String INSTANCE_DETAIL_EVENTS_EVENT_COLUMN = "SQLCM.Labels.instance-detail.events.event-column";

	@I18NMessage("Add SQL Server Instance")
	String ACTIONS_ADD_SERVER = "Labels.sql-cm.actions.add-server";

	@I18NMessage("View")
	String VIEW = "SQLCM.Labels.event-view";

	@I18NMessage("Date")
	String INSTANCE_DETAIL_EVENTS_DATE_COLUMN = "SQLCM.Labels.instance-detail.events.date-column";

	@I18NMessage("Time")
	String INSTANCE_DETAIL_EVENTS_TIME_COLUMN = "SQLCM.Labels.instance-detail.events.time";
	
	@I18NMessage("SQL")
	String INSTANCE_DETAIL_EVENTS_SQL_COLUMN = "SQLCM.Labels.instance-detail.events.sql";

	@I18NMessage("Events")
	String INSTANCE_DETAIL_EVENTS = "SQLCM.Labels.instance-detail.events";

	@I18NMessage("Login")
	String INSTANCE_DETAIL_EVENTS_LOGIN_COLUMN = "SQLCM.Labels.instance-detail.events.login";

	@I18NMessage("Options")
	String INSTANCE_DETAIL_EVENTS_OPTIONS_COLUMN = "SQLCM.Labels.instance-detail.events.options-column";

	@I18NMessage("Add databases")
	String INSTANCE_DETAIL_ADD_DATABASE = "SQLCM.Labels.instance-detail.add-database";

	@I18NMessage("Date")
	String DATE = "SQLCM.Labels.date";

	@I18NMessage("Textbox cannot contain more than 16000 characters")
	String TEXTBOX_CANNOT_CONTAIN_MORE_THAN = "Labels.sql-cm.textbox-cannot-contain-more";

	@I18NMessage("Login")
	String LOGIN = "SQLCM.Labels.login";

	@I18NMessage("Spid")
	String SPID = "SQLCM.Labels.spid";

	@I18NMessage("Application")
	String APPLICATION = "SQLCM.Labels.application";

	@I18NMessage("Host")
	String HOST = "SQLCM.Labels.host";

	@I18NMessage("Icon")
	String ICON = "SQLCM.Labels.icon";

	@I18NMessage("Access Check")
	String ACCESS_CHECK = "SQLCM.Labels.access-check";

	@I18NMessage("Database User")
	String DATABASE_USER = "SQLCM.Labels.database-user";

	@I18NMessage("Object")
	String OBJECT = "SQLCM.Labels.object";

	@I18NMessage("Target Login")
	String TARGET_LOGIN = "SQLCM.Labels.target-login";
	
	@I18NMessage("Created Login")
	String CREATED_LOGIN = "SQLCM.Labels.created-login";
	
	@I18NMessage("Deleted Login")
	String DELETED_LOGIN = "SQLCM.Labels.deleted-login";

	@I18NMessage("Target User")
	String TARGET_USER = "SQLCM.Labels.target-user";

	@I18NMessage("Role")
	String ROLE = "SQLCM.Labels.role";

	@I18NMessage("Owner")
	String OWNER = "SQLCM.Labels.owner";

	@I18NMessage("Session Login")
	String SESSION_LOGIN = "SQLCM.Labels.session-login";

	@I18NMessage("Audited Updates")
	String AUDITED_UPDATES = "SQLCM.Labels.audited-updates";

	@I18NMessage("Key")
	String PRIMARY_KEY = "SQLCM.Labels.primary-key";

	@I18NMessage("Table")
	String TABLE = "SQLCM.Labels.table";

	@I18NMessage("Column")
	String COLUMN = "SQLCM.Labels.column";

	@I18NMessage("Before Value")
	String BEFORE_VALUE = "SQLCM.Labels.before-value";

	@I18NMessage("After Value")
	String AFTER_VALUE = "SQLCM.Labels.after-value";

	@I18NMessage("Before")
	String BEFORE = "SQLCM.Labels.before";

	@I18NMessage("After")
	String AFTER = "SQLCM.Labels.after";
	
	@I18NMessage("Schema")
	String SCHEMA = "SQLCM.Labels.schema";

	@I18NMessage("Columns Updated")
	String COLUMNS_UPDATED = "SQLCM.Labels.columns-updated";

	@I18NMessage("Alerts")
	String ALERTS = "SQLCM.Labels.alerts";

	@I18NMessage("Alerts Rules")
	String ALERTSRULES = "SQLCM.Labels.alerts-rules";

	@I18NMessage("Alert")
	String ALERT = "SQLCM.Labels.alert";

	@I18NMessage("Alerts ({0} Severe | {1} High | {2} Medium | {3} Low)")
	String INSTANCES_ALERTS_SUMMARY = "SQLCM.Labels.instances-alerts-summary";

	@I18NMessage("AlertRules ({0} Event | {1} Data | {2} Status)")
	String INSTANCES_ALERTS_RULES_SUMMARY = "SQLCM.Labels.instances-alerts-rules-summary";

	@I18NMessage("Instance name")
	String INSTANCE_NAME = "SQLCM.Labels.instance.name";

	@I18NMessage("Level")
	String LEVEL = "SQLCM.Labels.level";

	@I18NMessage("Details")
	String DETAIL = "SQLCM.Labels.detail";

	@I18NMessage("Only 100 instances can be added at a time")
	String INSTANCES_ALLOWED = "Labels.sql-cm.instances-allowed";

	@I18NMessage("Event properties")
	String INSTANCE_DETAIL_EVENTS_OPTIONS_PROPERTIES = "SQLCM.Labels.instance-detail.events.options-properties";

	@I18NMessage("Refresh")
	String INSTANCE_DETAIL_EVENTS_OPTIONS_REFRESH = "SQLCM.Labels.instance-detail.events.options-refresh";

	@I18NMessage("Target Object")
	String TARGET_OBJECT = "SQLCM.Labels.target-object";

	@I18NMessage("Error During Adding Instances")
	String INSTANCES_ERROR_DURING_ADDING = "Labels.sql-cm.instances.error-add-instances";

	@I18NMessage("Audit Events")
	String INSTANCE_DETAIL_AUDIT_EVENTS = "SQLCM.Labels.instance-detail.audit-events";

	@I18NMessage("All Instance Events")
	String INSTANCE_DETAIL_ALL_AUDIT_EVENTS = "SQLCM.Labels.instance-detail.all-audit-events";

	@I18NMessage(" Instance with id -> [ {0} ] not found. ")
	String INSTANCE_DETAIL_MSG_INSTANCE_WITH_ID_NOT_FOUND = "SQLCM.Labels.instance-detail.instance-with-id-not-found";

	@I18NMessage("Error parse instance id argument ")
	String INSTANCE_DETAIL_MSG_ERROR_PARSE_INSTANCE_ID = "SQLCM.Labels.instance-detail.error-parse-instance-id-argument";

	@I18NMessage("This will delete the last column of view. If you delete the column, all widgets in the column will also be deleted. Are you sure you wish to continue and delete the column?")
	String ADMIN_DELETE_COLUMN_CONFIRMATION_MESSAGE = "Messages.admin-delete-column-confirmation-message";

	@I18NMessage("MM/dd hh:mm a")
	String ACTIVITY_CHART_ONE_DAY_TICK_DATE_FORMAT = "SQLCM.Labels.chart-one-day-tick-date-format";

	@I18NMessage("MM/dd hh:mm a")
	String ACTIVITY_CHART_SEVEN_DAYS_TICK_DATE_FORMAT = "SQLCM.Labels.chart-seven-days-tick-date-format";

	@I18NMessage("MM/dd")
	String ACTIVITY_CHART_THIRTY_DAYS_TICK_DATE_FORMAT = "SQLCM.Labels.chart-thirty-days-tick-date-format";

	@I18NMessage("Event Properties")
	String EVENT_PROPERTIES = "SQLCM.Labels.event-properties";

	@I18NMessage("Specify credentials to be used to connect to the computers that host the SQL Server instance to collect performance and configuration data.")
	String INSTANCES_SPECIFY_CREDENTIALS_FOR_WMI = "Labels.sql-cm.instances.specify-credentials-for-wmi";

	@I18NMessage("Warning: This will stop auditing of the selected databases. Activity data is for future operations performed on these databases will not be available for audit reports once they are removed.")
	String DATABASE_REMOVE_MESSAGE = "SQLCM.Labels.sql-cm.database.remove-message";

	@I18NMessage("Remove Audited Database")
	String DATABASE_REMOVE_TITLE = "SQLCM.Labels.sql-cm.database.remove-title";

	@I18NMessage("Warning: Removing an archive database from the Repository will make the archive data unavailable for viewing and reporting within SQL Compliance Manager. "
			+ "The database containing the archived data will not be deleted. Do you wish to remove the archive database from the Repository now?")
	String DATABASE_ARCHIVE_REMOVE_MESSAGE = "SQLCM.Labels.sql-cm.archive.remove-message";

	@I18NMessage("Remove Archive Database")
	String DATABASE_ARCHIVE_REMOVE_TITLE = "SQLCM.Labels.sql-cm.archive.remove-title";

	@I18NMessage("Failed to delete database because an internal exception occurred!")
	String DATABASE_FAILED_DELETE = "SQLCM.Labels.database.failed-delete";

	@I18NMessage("Can't remove instance(instances)")
	String CANT_REMOVE_INSTANCE = "Labels.sql-cm.instances.cant-remove";

	@I18NMessage("|")
	String VERTICAL_SEPARATOR_LABEL = "Labels.sql-cm.vertical-separator";

	@I18NMessage("Options")
	String EVENTS_OPTIONS_COLUMN = "SQLCM.Labels.events.options-column";

	@I18NMessage("Registered SQL Server Properties")
	String INSTANCE_PROPERTIES_DIALOG_TITLE = "Labels.sql-cm.instance-properties-dialog-title";

	/*----ServerStatus enum. Begin.---------*/
	@I18NMessage("OK")
	String SERVER_STATUS_OK = "Labels.sql-cm.server-status-ok";

	@I18NMessage("Disabled")
	String SERVER_STATUS_DISABLED = "Labels.sql-cm.server-status-disabled";

	@I18NMessage("Down")
	String SERVER_STATUS_DOWN = "Labels.sql-cm.server-status-down";

	@I18NMessage("Error")
	String SERVER_STATUS_ERROR = "Labels.sql-cm.server-status-error";

	@I18NMessage("Slow")
	String SERVER_STATUS_SLOW = "Labels.sql-cm.server-status-slow";

	@I18NMessage("Up")
	String SERVER_STATUS_UP = "Labels.sql-cm.server-status-up";

	@I18NMessage("Unknown")
	String SERVER_STATUS_UNKNOWN = "Labels.sql-cm.server-status-unknown";
	/*----ServerStatus enum. End.---------*/

	/*----State enum. Begin.---------*/
	@I18NMessage("Enabled")
	String STATE_ENABLED = "Labels.sql-cm.state-enabled";

	@I18NMessage("Disabled")
	String STATE_DISABLED = "Labels.sql-cm.state-disabled";
	/*----State enum. End.---------*/

	/*----Period enum. Begin.---------*/
	@I18NMessage("per hour")
	String PERIOD_PER_HOUR = "Labels.sql-cm.period-per-hour";

	@I18NMessage("per day")
	String PERIOD_PER_DAY = "Labels.sql-cm.period-per-day";
	/*----Period enum. End.---------*/

	/*----Instance properties dialog tabs. Begin.---------*/
	@I18NMessage("General")
	String INSTANCE_PROPERTIES_DIALOG_GENERAL_TAB = "Labels.sql-cm.instance-properties-dialog-general-tab";

	@I18NMessage("Audited Activities")
	String INSTANCE_PROPERTIES_DIALOG_AUDITED_ACTIVITIES_TAB = "Labels.sql-cm.instance-properties-dialog-audited-activities-tab";

	@I18NMessage("Privileged User Auditing")
	String INSTANCE_PROPERTIES_DIALOG_PRIVILEGED_USER_AUDITING_TAB = "Labels.sql-cm.instance-properties-dialog-privileged-user-auditing-tab";

	@I18NMessage("Auditing Thresholds")
	String INSTANCE_PROPERTIES_DIALOG_AUDITING_THRESHOLDS_TAB = "Labels.sql-cm.instance-properties-dialog-auditing-thresholds-tab";

	@I18NMessage("Advanced")
	String INSTANCE_PROPERTIES_DIALOG_ADVANCED_TAB = "Labels.sql-cm.instance-properties-dialog-advanced-tab";

	@I18NMessage("Learn how to optimize performance with audit settings.")
	String INSTANCE_PROPERTIES_DIALOG_OPTIMIZE_PERFORMANCE = "Labels.sql-cm.instance-properties-dialog-optimize-performance-link";

	@I18NMessage("Ok")
	String INSTANCE_PROPERTIES_DIALOG_OK_BUTTON = "Labels.sql-cm.instance-properties-dialog-ok-button";

	@I18NMessage("Cancel")
	String INSTANCE_PROPERTIES_DIALOG_CANCEL_BUTTON = "Labels.sql-cm.instance-properties-dialog-cancel-button";
	/*----Instance properties dialog tabs. End.---------*/

	/*----Instance properties dialog general tab labels. Begin.---------*/
	@I18NMessage("SQL Server:")
	String INST_PROP_D_GEN_TAB_SQL_SERVER = "Labels.sql-cm.inst-prop-d-gen-tab-sql-server";

	@I18NMessage("Version:")
	String INST_PROP_D_GEN_TAB_VERSION = "Labels.sql-cm.inst-prop-d-gen-tab-version";

	@I18NMessage("Description:")
	String INST_PROP_D_GEN_TAB_DESCRIPTION = "Labels.sql-cm.inst-prop-d-gen-tab-description";

	@I18NMessage("Status:")
	String INST_PROP_D_GEN_TAB_STATUS = "Labels.sql-cm.inst-prop-d-gen-tab-status";

	@I18NMessage("Date created:")
	String INST_PROP_D_GEN_TAB_DATE_CREATED = "Labels.sql-cm.inst-prop-d-gen-tab-date-created";

	@I18NMessage("Last modified:")
	String INST_PROP_D_GEN_TAB_LAST_MODIFIED = "Labels.sql-cm.inst-prop-d-gen-tab-last-modified";

	@I18NMessage("Last heartbeat:")
	String INST_PROP_D_GEN_TAB_LAST_HEARTBEAT = "Labels.sql-cm.inst-prop-d-gen-tab-last-heartbeat";

	@I18NMessage("Events received:")
	String INST_PROP_D_GEN_TAB_EVENTS_RECEIVED = "Labels.sql-cm.inst-prop-d-gen-tab-events-received";

	@I18NMessage("Audit Settings")
	String INST_PROP_D_GEN_TAB_AUDIT_SETTINGS = "Labels.sql-cm.inst-prop-d-gen-tab-audit-settings";

	@I18NMessage("Audit status:")
	String INST_PROP_D_GEN_TAB_AUDIT_STATUS = "Labels.sql-cm.inst-prop-d-gen-tab-audit-status";

	@I18NMessage("Last agent update:")
	String INST_PROP_D_GEN_TAB_LAST_AGENT_UPDATE = "Labels.sql-cm.inst-prop-d-gen-tab-last-agent-update";

	@I18NMessage("Audit settings status:")
	String INST_PROP_D_GEN_TAB_AUDIT_SETTINGS_STATUS = "Labels.sql-cm.inst-prop-d-gen-tab-audit-settings-status";

	@I18NMessage("Update now")
	String INST_PROP_D_GEN_TAB_UPDATE_NOW = "Labels.sql-cm.inst-prop-d-gen-tab-update-now";

	@I18NMessage("Events Database Information")
	String INST_PROP_D_GEN_TAB_EVENTS_DB_INFO = "Labels.sql-cm.inst-prop-d-gen-tab-events-db-info";

	@I18NMessage("Events database:")
	String INST_PROP_D_GEN_TAB_EVENTS_DB = "Labels.sql-cm.inst-prop-d-gen-tab-events-db";

	@I18NMessage("Database integrity:")
	String INST_PROP_D_GEN_TAB_DB_INTEGRITY = "Labels.sql-cm.inst-prop-d-gen-tab-db-integrity";

	@I18NMessage("Last integrity check:")
	String INST_PROP_D_GEN_TAB_LAST_INTEGRITY_CHECK = "Labels.sql-cm.inst-prop-d-gen-tab-last-integrity-check";

	@I18NMessage("Last integrity check result:")
	String INST_PROP_D_GEN_TAB_LAST_INTEGRITY_CHECK_RESULT = "Labels.sql-cm.inst-prop-d-gen-tab-last-integrity-check-result";

	@I18NMessage("Archive Summary")
	String INST_PROP_D_GEN_TAB_ARCHIVE_SUMMARY = "Labels.sql-cm.inst-prop-d-gen-tab-archive-summary";

	@I18NMessage("Time of last archive:")
	String INST_PROP_D_GEN_TAB_LAST_ARCHIVE_TIME = "Labels.sql-cm.inst-prop-d-gen-tab-last-archive-time";

	@I18NMessage("Last archive results:")
	String INST_PROP_D_GEN_TAB_LAST_ARCHIVE_RESULTS = "Labels.sql-cm.inst-prop-d-gen-tab-last-archive-results";
	/*----Instance properties dialog general tab labels. End.---------*/

	@I18NMessage("Archived Events")
	String ARCHIVED_EVENTS = "SQLCM.Labels.archived-events";

	@I18NMessage("Audit Events")
	String AUDIT_EVENTS = "SQLCM.Labels.audit-events";

	@I18NMessage("Data Alerts")
	String DATA_ALERTS = "SQLCM.Labels.data-alerts";

	@I18NMessage("Status Alerts")
	String STATUS_ALERTS = "SQLCM.Labels.status-alerts";

	/*----Instance properties dialog audited activities tab labels. Begin.---------*/
	@I18NMessage("Audited Activity")
	String INST_PROP_D_AUD_ACTIVITY_TAB_AUD_ACTIVITY = "Labels.sql-cm.inst-prop-d-aud-activity-tab-audit-activity";

	@I18NMessage("Logins")
	String INST_PROP_D_AUD_ACTIVITY_TAB_LOGINS = "Labels.sql-cm.inst-prop-d-aud-activity-tab-logins";

	@I18NMessage("Failed logins")
	String INST_PROP_D_AUD_ACTIVITY_TAB_FAILED_LOGINS = "Labels.sql-cm.inst-prop-d-aud-activity-tab-failed-logins";

	@I18NMessage("Security changes (e.g. GRANT, REVOKE, LOGIN CHANGE PWD)")
	String INST_PROP_D_AUD_ACTIVITY_TAB_SECURITY_CHANGES = "Labels.sql-cm.inst-prop-d-aud-activity-tab-security-changes";

	@I18NMessage("Database definition (e.g. CREATE or DROP DATABASE)")
	String INST_PROP_D_AUD_ACTIVITY_TAB_DB_DEFINITION = "Labels.sql-cm.inst-prop-d-aud-activity-tab-db-definition";

	@I18NMessage("Administrative Actions (e.g. DBCC)")
	String INST_PROP_D_AUD_ACTIVITY_TAB_ADMINISTRATIVE_ACTIVITIES = "Labels.sql-cm.inst-prop-d-aud-activity-tab-administrative-activities";

	@I18NMessage("User defined events (custom SQL Server event type)")
	String INST_PROP_D_AUD_ACTIVITY_TAB_USER_DEFINED_EVENTS = "Labels.sql-cm.inst-prop-d-aud-activity-tab-user-defined-events";

	/*SQLCm 5.4_4.1.1_Extended Events B*/
	@I18NMessage("SQL statements for Before-After Data and SELECT activities")
	String INST_PROP_D_AUD_ACTIVITY_TAB_BEFORE_AFTER_DATA = "Labels.sql-cm.inst-prop-d-aud-activity-tab-before-after-data";
	
	@I18NMessage("Access Check Filter")
	String INST_PROP_D_AUD_ACTIVITY_TAB_ACCESS_CHECK_FILTER = "Labels.sql-cm.inst-prop-d-aud-activity-tab-access-check-filter";

	@I18NMessage("Filter events based on access check")
	String INST_PROP_D_AUD_ACTIVITY_TAB_FILTER_EVENTS_BASED_ON_ACCESS_CHECK = "Labels.sql-cm.inst-prop-d-aud-activity-tab-filter-events-based-on-access-check";

	@I18NMessage("Passed")
	String INST_PROP_D_AUD_ACTIVITY_TAB_AUDIT_ACTIONS_PASS_ACCESS_CHECK = "Labels.sql-cm.inst-prop-d-aud-activity-tab-audit-actions-pass-access-check";

	@I18NMessage("Failed")
	String INST_PROP_D_AUD_ACTIVITY_TAB_AUDIT_ACTIONS_FAILED_ACCESS_CHECK = "Labels.sql-cm.inst-prop-d-aud-activity-tab-audit-actions-failed-access-check";

	@I18NMessage("Note: This screen sets the level of server level auditing only. To audit database level activity "
			+ "such as INSERT, UPDATE or SELECT statements, you need to designate audited databases from this server "
			+ "and the level of auditing for the database.")
	String INST_PROP_D_AUD_ACTIVITY_TAB_NOTE = "Labels.sql-cm.inst-prop-d-aud-activity-tab-note";
	/*----Instance properties dialog audited activities tab labels. End.---------*/

	/*----Instance properties dialog auditing thresholds tab labels. Begin.---------*/
	@I18NMessage("Warning")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_WARNING = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-warning";

	@I18NMessage("Critical")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_CRITICAL = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-critical";

	@I18NMessage("Period")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_PERIOD = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-period";

	@I18NMessage("Enabled")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_ENABLED = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-enabled";

	@I18NMessage("Event alerts")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_EVENT_ALERTS = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-even-alerts";

	@I18NMessage("Failed logins")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_FAILED_LOGINS = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-failed-logins";

	@I18NMessage("Security")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_SECURITY = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-security";

	@I18NMessage("DDL")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_DDL = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-ddl";

	@I18NMessage("Privileged user")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_PRIVILEGED_USER = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-privileged-user";

	@I18NMessage("Overall activity")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_OVERALL_ACTIVITY = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-overall-activity";

	@I18NMessage("Auditing thresholds can indicate when event activity is unusually high. If a threshold is exceeded,"
			+ " its status displays on the Activity Report Card tab for the event category.")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_NOTE = "Labels.sql-cm.inst-prop-d-aud-thresholds-tab-note";
	
	@I18NMessage("Threshold Notification")
	String INST_PROP_D_AUD_THRESHOLDS_NOTIFICATION_LINK = "Labels.sql-cm.inst-prop-d-aud-thresholds-notification-link";
	
	@I18NMessage("Threshold Notification")
	String INST_PROP_D_AUD_THRESHOLDS_TAB_THRESHOLD_NOTIFICATION_TITLE = "SQLCM.Labels.auditing-thresholds-title";

	/*----Instance properties dialog auditing thresholds tab labels. End.---------*/

	/*----Instance properties dialog advanced tab labels. Begin.---------*/
	@I18NMessage("Default Database Permissions")
	String INST_PROP_D_ADVANCED_TAB_DEFAULT_DB_PERMISSIONS = "Labels.sql-cm.inst-prop-d-advanced-tab-default-db-permissions";

	@I18NMessage("Select the default level of access you want to grant users on the database containing audit data "
			+ "for this SQL Server instance.")
	String INST_PROP_D_ADVANCED_TAB_LABEL_1 = "Labels.sql-cm.inst-prop-d-advanced-tab-label-1";

	@I18NMessage("Grant right to read events and their associated SQL statements.")
	String INST_PROP_D_ADVANCED_TAB_GRANT_READ_EVENTS_WITH_ADDS = "Labels.sql-cm.inst-prop-d-advanced-tab-grant-read-events-with-adds";

	@I18NMessage("Grant right to read events only - to allow users to view the associated SQL statements, you will "
			+ "need to explicitly grant users read access to the database.")
	String INST_PROP_D_ADVANCED_TAB_GRANT_READ_EVENTS_ONLY = "Labels.sql-cm.inst-prop-d-advanced-tab-grant-read-events-only";

	@I18NMessage("Deny read access by default - to allow users to view events and the associated SQL , you will "
			+ "need to explicitly grant users read access to the database.")
	String INST_PROP_D_ADVANCED_TAB_GRANT_DENY_READ_ = "Labels.sql-cm.inst-prop-d-advanced-tab-grant-deny-read";

	@I18NMessage("SQL Statement Limit")
	String INST_PROP_D_ADVANCED_TAB_SQL_STATEMENT_LIMIT = "Labels.sql-cm.inst-prop-d-advanced-tab-sql-statement-limit";

	@I18NMessage("In most cases, the high level event information gathered is sufficient for meeting audit "
			+ "requirements. However, some users may find that they need the extra details afforded by the collection "
			+ "of the actual SQL statement associated with each audited event.")
	String INST_PROP_D_ADVANCED_TAB_LABEL_2 = "Labels.sql-cm.inst-prop-d-advanced-tab-label-2";

	@I18NMessage("Be aware that collecting SQL statement will significantly increase the amount of data gathered and "
			+ "should be used sparingly. Gathered SQL statements may also contain confidential information. The option "
			+ "to gather SQL statements is available on each audited database.")
	String INST_PROP_D_ADVANCED_TAB_LABEL_3 = "Labels.sql-cm.inst-prop-d-advanced-tab-label-3";

	@I18NMessage("Use the following option to specify the maximum size of stored SQL statements. Statements "
			+ "exceeding this maximum are truncated.")
	String INST_PROP_D_ADVANCED_TAB_LABEL_4 = "Labels.sql-cm.inst-prop-d-advanced-tab-label-4";

	@I18NMessage("Store entire text of SQL statements")
	String INST_PROP_D_ADVANCED_TAB_LABEL_ENTIRE_STORE = "Labels.sql-cm.inst-prop-d-advanced-tab-label-entire-store";

	@I18NMessage("Truncate stored SQL statements after ")
	String INST_PROP_D_ADVANCED_TAB_LABEL_TRUNCATE_STORE_PART_1 = "Labels.sql-cm.inst-prop-d-advanced-tab-label-truncate-store-part-1";

	@I18NMessage("characters")
	String INST_PROP_D_ADVANCED_TAB_LABEL_TRUNCATE_STORE_PART_2 = "Labels.sql-cm.inst-prop-d-advanced-tab-label-truncate-store-part-2";

	@I18NMessage("For Reports, SQL text will be truncated after ")
	String INST_PROP_D_ADVANCED_TAB_LABEL_TRUNCATE_SQL_REPORT = "Labels.sql-cm.inst-prop-d-advanced-tab-label-truncate-sql-report";

	@I18NMessage("characters.")
	String INST_PROP_D_ADVANCED_TAB_LABEL_TRUNCATE_SQL_REPORT_CHARACTERS = "Labels.sql-cm.inst-prop-d-advanced-tab-label-truncate-sql-report-characters";

	
	/*----Instance properties dialog advanced tab labels. End.---------*/

	/*----Instance properties dialog privileged user auditing tab labels. Begin.---------*/
	@I18NMessage("Privileged users and roles to be audited:")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_PRIVILEGED_USERS = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-privileged-users";

	@I18NMessage("Add")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_ADD_BUTTON = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-add-button";

	@I18NMessage("Remove")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_REMOVE_BUTTON = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-remove-button";

	@I18NMessage("Audited Activity")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_AUDITED_ACTIVITY = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-audited-activity";

	@I18NMessage("Audit all activities done by privileged users")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_AUDIT_ALL_ACTIVITIES = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-audit-all-activities";

	@I18NMessage("Audit selected activities done by privileged users")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_AUDIT_SELECTED_ACTIVITIES = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-audit-selected-activities";

	@I18NMessage("Logins")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_LOGINS = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-logins";

	@I18NMessage("Failed logins")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_FAILED_LOGINS = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-failed-logins";

	@I18NMessage("Administrative actions")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_ADMINISTRATIVE_ACTIONS = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-administrative-actions";

	@I18NMessage("Database SELECT operations")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_DB_SELECT_OPERATIONS = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-db-select-operations";

	@I18NMessage("Database definition (DDL)")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_DDL = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-ddl";

	@I18NMessage("Database modification (DML)")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_DML = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-dml";

	@I18NMessage("User defined events")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_USER_DEFINED_EVENTS = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-user-defined-events";

	@I18NMessage("Security changes")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_SECURITY_CHANGES = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-security-changes";

	@I18NMessage("Filter events based on access check: ")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_FILTER_EVENTS = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-filter-events";

	@I18NMessage("Passed ")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_PASSED = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-passed";

	@I18NMessage("Failed ")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_FAILED = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-failed";

	@I18NMessage("Capture SQL statements for DML and SELECT activities ")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_CAPTURE_SQL = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-capture-sql";

	//SQLCm 5.4_4.1.1_Extended Events Start
	@I18NMessage("Capture DML and SELECT activities using SQL Extended Events ")
	 String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_CAPTURE_SQL_DML_EXTENDED = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-capture-sql-dml-select-extended-events";
	//SQLCm 5.4_4.1.1_Extended Events End

	
	@I18NMessage("Capture transaction status for DML activity")
	String INST_PROP_D_PRIVILEGED_USER_AUD_TAB_CAPTURE_TRANSACTION = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-capture-transaction";

	@I18NMessage("Capture SQL statements for DDL and Security Changes")
	String PRIVILEGED_DDL_ACTIVITIES = "Labels.sql-cm.privileged-ddl-activities";
	/*----Instance properties dialog privileged user auditing tab labels. End.---------*/

	/* LoginsRolesLocation enum. Begin. */
	@I18NMessage("Server Roles")
	String LOGINS_ROLES_LOCATION_SERVER_ROLES = "Labels.sql-cm.logins-roles-location.server-roles";

	@I18NMessage("Server Logins")
	String LOGINS_ROLES_LOCATION_SERVER_LOGINS = "Labels.sql-cm.logins-roles-location.server-logins";
	/* LoginsRolesLocation enum. End. */

	/* Add privileged users dialog. Begin. */
	@I18NMessage("Add Users")
	String ADD_PRIVILEGED_USERS_D_TITLE = "Labels.sql-cm.add-privileged-users-d.title";

	@I18NMessage("Show Logins/Roles from:")
	String ADD_PRIVILEGED_USERS_D_SHOW_LR_FROM = "Labels.sql-cm.add-privileged-users-d.show-lr-from";

	@I18NMessage("Available Logins/Roles:")
	String ADD_PRIVILEGED_USERS_D_AVAILABLE_LR = "Labels.sql-cm.add-privileged-users-d.available-lr";

	@I18NMessage("Add")
	String ADD_PRIVILEGED_USERS_D_ADD_BUTTON = "Labels.sql-cm.add-privileged-users-d.add-button";

	@I18NMessage("Remove")
	String ADD_PRIVILEGED_USERS_D_REMOVE_BUTTON = "Labels.sql-cm.add-privileged-users-d.remove-button";

	@I18NMessage("Add Users list: ")
	String ADD_PRIVILEGED_USERS_D_ADD_PRIVILEGED_USERS_LIST = "Labels.sql-cm.add-privileged-users-d.add-privileged-users-list";

	@I18NMessage("Note: Specifying large numbers of users may have a performance impact on the audited SQL Server.")
	String ADD_PRIVILEGED_USERS_D_NOTE = "Labels.sql-cm.add-privileged-users-d.note";
	/* Add privileged users dialog. End. */

	/* Detailed Event properties dialog. Begin. */
	@I18NMessage("Event Filters")
	String EVENT_FILTERS = "SQLCM.Labels.event-filters";

	@I18NMessage("An error occurred loading the event")
	String EVENT_PROPERTIES_ERROR_TITLE = "Labels.sql-cm.event-properties-error-title";

	@I18NMessage("No data available for the selected event. This could occur if the event has been groomed or archived from the event database for this SQL Server or if the database is extremely busy and unable to respond to the request in a timely fashion.")
	String EVENT_PROPERTIES_ERROR_BODY = "Labels.sql-cm.event-properties-error-body";

	@I18NMessage("Sensitive Columns")
	String EVENT_PROPERTIES_DIALOG_SENSITIVE_COLUMNS_TAB = "SQLCM.sql-cm.event-properties-dialog-sensitive-columns-tab";

	@I18NMessage("Before-After Data")
	String EVENT_PROPERTIES_DIALOG_BEFORE_AFTER_DATA_TAB = "SQLCM.sql-cm.event-properties-dialog-before-after-data-tab";

	@I18NMessage("Row #")
	String EVENT_PROPERTIES_DIALOG_ROW = "Labels.sql-cm.event-properties-dialog-row";

	@I18NMessage("Column")
	String EVENT_PROPERTIES_DIALOG_COLUMN = "Labels.sql-cm.event-properties-dialog-column";

	@I18NMessage("Archive Properties")
	String ARCHIVE_PROPERTIES_DIALOG_TITLE = "Labels.sql-cm.archive-properties-dialog-title";

	@I18NMessage("General")
	String ARCHIVE_PROPERTIES_DIALOG_GENERAL_TAB = "Labels.sql-cm.archive-properties-dialog-general-tab";

	@I18NMessage("Default Permissions")
	String ARCHIVE_PROPERTIES_DIALOG_DEFAULT_PERMISSIONS = "Labels.sql-cm.archive-properties-dialog-title-default-permissions";

	@I18NMessage("OK")
	String ARCHIVE_PROPERTIES_DIALOG_OK_BUTTON = "Labels.sql-cm.archive-properties-dialog-ok-button";

	@I18NMessage("Close")
	String ARCHIVE_PROPERTIES_DIALOG_CLOSE_BUTTON = "Labels.sql-cm.archive-properties-dialog-close-button";

	@I18NMessage("Select the default level of access you want to grant users.")
	String ARCHIVE_PROPERTIES_DIALOG_LEVEL_ACCESS_TITLE = "Labels.sql-cm.archive-properties-dialog-level-access-title";

	@I18NMessage("Default Database Permissions")
	String ARCHIVE_PROPERTIES_DIALOG_DEFAULT_DATABASE_PERMISSIONS = "Labels.sql-cm.archive-properties-dialog-title-default-database-permissions";

	@I18NMessage("Grant right to read events and their associated SQL statements.")
	String ARCHIVE_PROPERTIES_DIALOG_GRANT_RIGHT_TO_READ = "Labels.sql-cm.archive-properties-dialog-grant-right-to-read";

	@I18NMessage("Grant right to read events and their associated SQL statements."
			+ "Grant right to read events only - To allow users to view the associated SQL "
			+ "statements, you will need to explicitly grant users read access to the database.")
	String ARCHIVE_PROPERTIES_DIALOG_GRANT_RIGHT_TO_READ_ONLY = "Labels.sql-cm.archive-properties-dialog-grant-right-to-read-only";

	@I18NMessage("Deny read access by default - To allow users to view events and the associated "
			+ "SQL. you will need to explicitly grant users read access to the database.")
	String ARCHIVE_PROPERTIES_DIALOG_DENY_READ_ACCESS = "Labels.sql-cm.archive-properties-dialog-deny-read-access";

	@I18NMessage("SQL Server:")
	String ARCHIVE_PROPERTIES_DIALOG_SQL_SERVER = "Labels.sql-cm.archive-properties-dialog-sql-server";

	@I18NMessage("Display Name:")
	String ARCHIVE_PROPERTIES_DIALOG_DISPLAY_NAME = "Labels.sql-cm.archive-properties-dialog-display-name";

	@I18NMessage("Description:")
	String ARCHIVE_PROPERTIES_DIALOG_DESCRIPTION = "Labels.sql-cm.archive-properties-dialog-description";

	@I18NMessage("Archive Database Summary")
	String ARCHIVE_PROPERTIES_DIALOG_ARCHIVE_DATABASE_SUMMARY = "Labels.sql-cm.archive-properties-dialog-archive-database-summary";

	@I18NMessage("Database Name:")
	String ARCHIVE_PROPERTIES_DIALOG_DATABASE_NAME = "Labels.sql-cm.archive-properties-dialog-database-name";

	@I18NMessage("Event Time Span:")
	String ARCHIVE_PROPERTIES_DIALOG_EVENT_TIME_SPAN = "Labels.sql-cm.archive-properties-dialog-event-time-span";

	@I18NMessage("Database Integrity:")
	String ARCHIVE_PROPERTIES_DIALOG_DATABASE_INTEGRITY = "Labels.sql-cm.archive-properties-dialog-database-integrity";

	@I18NMessage("Last Integrity Check:")
	String ARCHIVE_PROPERTIES_DIALOG_LAST_INTEGRITY_CHECK = "Labels.sql-cm.archive-properties-dialog-last-integrity-check";

	@I18NMessage("Last Integrity Check Result:")
	String ARCHIVE_PROPERTIES_DIALOG_LAST_INTEGRITY_CHECK_RESULT = "Labels.sql-cm.archive-properties-dialog-last-integrity-check-result";

	@I18NMessage("to")
	String ARCHIVE_PROPERTIES_DIALOG_TO = "Labels.sql-cm.archive-properties-dialog-to";

	@I18NMessage("OK")
	String ARCHIVE_PROPERTIES_DIALOG_OK = "Labels.sql-cm.archive-properties-dialog-ok";

	@I18NMessage("Bad")
	String ARCHIVE_PROPERTIES_DIALOG_BAD = "Labels.sql-cm.archive-properties-dialog-bad";

	@I18NMessage("Never")
	String ARCHIVE_PROPERTIES_DIALOG_NEVER = "Labels.sql-cm.archive-properties-dialog-never";

	@I18NMessage("Attach Archive Database")
	String ARCHIVE_ATTACH_HEADER_MESSAGE = "Labels.sql-cm.archive-attach-header-message";

	@I18NMessage("Set Maintenance Schedule")
	String APPLY_REINDEX_FOR_ARCHIVE_HEADER_MESSAGE = "Labels.sql-cm.apply-reindex-for-archive-header-message";

	@I18NMessage("Specify when index maintenance should be performed on the Repository database.")
	String APPLY_REINDEX_FOR_ARCHIVE_MESSAGE = "Labels.sql-cm.apply-reindex-for-archive-message";

	@I18NMessage("Tell me more...")
	String APPLY_REINDEX_FOR_ARCHIVE_TELL_ME_MORE = "Labels.sql-cm.apply-reindex-for-archive-tell-me-more";

	@I18NMessage("Specify Schedule")
	String APPLY_REINDEX_SPECIFY_SCHEDULE = "Labels.sql-cm.apply-reindex-specify-schedule";

	@I18NMessage("Start Time:")
	String APPLY_REINDEX_START_TIME = "Labels.sql-cm.apply-reindex-start-time";

	@I18NMessage("Duration(HH:MM):")
	String APPLY_REINDEX_DURATION = "Labels.sql-cm.apply-reindex-duration";

	@I18NMessage("Disable Schedule")
	String APPLY_REINDEX_DISABLE_SCHEDULE = "Labels.sql-cm.apply-reindex-disable-schedule";

	@I18NMessage("Select an existing archive database to attach to SQL Compliance Manager.")
	String ARCHIVE_ATTACH_TITLE_MESSAGE = "Labels.sql-cm.archive-attach-title-message";

	@I18NMessage("Show all databases")
	String ARCHIVE_ATTACH_SHOW_ALL_DATABASES = "Labels.sql-cm.archive-attach-show-all-databases";

	@I18NMessage("Archive Database:")
	String ARCHIVE_PROPERTIES_DIALOG_ARCHIVE_DATABASE = "Labels.sql-cm.archive-properties-dialog-archive-database";

	@I18NMessage("Archive Information")
	String ARCHIVE_PROPERTIES_DIALOG_ARCHIVE_INFORMATION = "Labels.sql-cm.archive-properties-dialog-archive-information";

	@I18NMessage("This archive database was created by an older version of SQL Compliance Manager. The database schema must be upgraded to the current version before it can be attached to the repository. This upgrade process will not affect any existing event data. Do you want to upgrade the archive database?")
	String ARCHIVE_PROPERTIES_DIALOG_UPGRADE_DATABASE_TO_EARLIER_MESSAGE = "Labels.sql-cm.archive-properties-dialog-upgrade-to-earlier-database-message";

	@I18NMessage("This database was created using an earlier version of SQL Compliance Manager, and does not contain optimized indexes. Optimized indexes improve Management Console performance when viewing audited events. You can add these indexes now or later. The update process requires free disk space and may take some time to complete. Do you want to update indexes now?")
	String ARCHIVE_PROPERTIES_DIALOG_UPGRADE_DATABASE_TO_OLDER_MESSAGE = "Labels.sql-cm.archive-properties-dialog-upgrade-to-older-database-message";

	@I18NMessage("Server configuration wizard - Add Server")
	String ADD_SERVER_WIZARD_AUDIT_TITLE = "SQLCM.Label.add-server-wizard.title";

	@I18NMessage("SQLcm configuration wizard - Add databases")
	String ADD_SERVER_DATABASES_WIZARD_AUDIT_TITLE = "SQLCM.Label.add-server-databases-wizard.title";

	@I18NMessage("Audit Databases")
	String ADD_SERVER_WIZARD_AUDIT_DATABASES = "SQLCM.Label.add-server-wizard.audit-databases";

	@I18NMessage("<h3>Select Databases</h3>"
			+ "<p>"
			+ " Select the databases you want to audit. SQL Compliance Manager will collect audit data for the selected databases. "
			+ "</p>")
	String ADD_SERVER_WIZARD_SELECT_DATABASES_TIPS = "SQLCM.Html.add-server-wizard.select-databases.tips";

	@I18NMessage("Select All")
	String ADD_SERVER_WIZARD_SELECT_DATABASES_SELECT_ALL = "SQLCM.Label.add-server-wizard.select-databases.select-all-button";

	@I18NMessage("Unselect All")
	String ADD_SERVER_WIZARD_SELECT_DATABASES_UNSELECT_ALL = "SQLCM.Label.add-server-wizard.select-databases.unselect-all-button";

	@I18NMessage("<h3>Audit Collection Level</h3>"
			+ "<p>"
			+ " Select the audit collection level you want to use for the newly audited database."
			+ " The collection level affects the amount of event data collected for database activities."
			+ "</p>")
	String ADD_SERVER_WIZARD_AUDIT_COLLECTION_LEVEL_TIPS = "SQLCM.Html.add-server-wizard.audit-collection-level.tips";

	@I18NMessage("Default")
	String ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_LABEL = "SQLCM.Label.add-server-wizard.audit-collection-level.radio.default-label";

	@I18NMessage("\\u0020- Audits events and activities most commonly required by auditors This collection level meets most auditing needs")
	String ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_DESC = "SQLCM.Label.add-server-wizard.audit-collection-level.radio.default-deac";

	@I18NMessage("Tell me more")
	String ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_LINK_LABEL = "SQLCM.Label.add-server-wizard.audit-collection-level.radio.default.link-label";

	@I18NMessage("http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Audit+Collection+Level+window")
	String ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_LINK_URL = "SQLCM.Label.add-server-wizard.audit-collection-level.radio.default.link-url";

	@I18NMessage("Custom")
	String ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_CUSTOM_LABEL = "SQLCM.Label.add-server-wizard.audit-collection-level.radio.custom-label";

	@I18NMessage("\\u0020- Allows you to specify specific audit settings."
			+ " This collection level is recommended for advanced users only."
			+ " Before selecting specific audit settings,"
			+ " review the events gathered by the Custom collection level and review the help to better understand your choices")
	String ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_CUSTOM_DESC = "SQLCM.Label.add-server-wizard.audit-collection-level.radio.custom-desc";

	@I18NMessage("Regulation")
	String ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_REGULATION_LABEL = "SQLCM.Html.add-server-wizard.audit-collection-level.radio.regulation-label";

	@I18NMessage("\\u0020- Configures your audit settings to collect the event data required by specific regulatory guidelines, such as PCI or HIPAA ")
	String ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_REGULATION_DESC = "SQLCM.Html.add-server-wizard.audit-collection-level.radio.regulation-desc";

	@I18NMessage("<h3>Permissions Check</h3>"
			+ "<p>"
			+ " Required permissions are checked for proper functioning of SQLcm processes on SQL."
			+ " Server instance to be audited."
			+ "</p>"
			+ "<p>"
			+ " Please visit <a target='_blank' style='color: #FE4210' href='http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Permissions+Check+window'>link</a> for additional help. "
			+ "</p>")
	String ADD_SERVER_WIZARD_PERMISSION_CHECK_TIPS = "SQLCM.Html.add-server-wizard.permissions-check.tips";

	@I18NMessage("Operation Complete Total {0}, Passed {1}, Failed {2} ")
	String ADD_SERVER_WIZARD_PERMISSION_CHECK_OPERATION_INFO = "SQLCM.Label.add-server-wizard.permissions-check.operation-info";

	@I18NMessage("Re-check")
	String ADD_SERVER_WIZARD_PERMISSION_CHECK_RECHECK = "SQLCM.Label.add-server-wizard.permissions-check.re-check-button";

	@I18NMessage("Passed")
	String ADD_SERVER_WIZARD_PERMISSION_PASSED = "SQLCM.Label.add-server-wizard.permissions-check.passed";

	@I18NMessage("Failed")
	String ADD_SERVER_WIZARD_PERMISSION_FAILED = "SQLCM.Label.add-server-wizard.permissions-check.failed";

	@I18NMessage("Unknown")
	String ADD_SERVER_WIZARD_PERMISSION_UNKNOWN = "SQLCM.Label.add-server-wizard.permissions-check.unknown";

	@I18NMessage("<h3>Summary</h3>"
			+ "<p>"
			+ " Review the summary of the audit setting you chose for this SQL Server instance and its hosted databases."
			+ "</p>")
	String ADD_SERVER_WIZARD_SUMMARY_TIPS = "SQLCM.Html.add-server-wizard.summary.tips";

	@I18NMessage("Audit Level:")
	String ADD_SERVER_WIZARD_SUMMARY_AUDIT_LEVEL = "SQLCM.Label.add-server-wizard.summary.audit-level";

	@I18NMessage("Server:")
	String ADD_SERVER_WIZARD_SUMMARY_SERVER = "SQLCM.Label.add-server-wizard.summary.server";

	@I18NMessage("View the Regulation Guideline Details")
	String ADD_SERVER_WIZARD_SUMMARY_REGULATION_DETAILS = "SQLCM.Label.add-server-wizard.summary.regulation-details";

	@I18NMessage("Databases:")
	String ADD_SERVER_WIZARD_SUMMARY_DATABASES = "SQLCM.Label.add-server-wizard.summary.databases";

	@I18NMessage("View the Regulation Guideline Details")
	String ADD_SERVER_WIZARD_SUMMARY_REG_GUIDE_DETAILS = "SQLCM.Label.add-server-wizard.summary.reg-guide-details";

	/* . End. */

	/* Add databases: flow 2 step database audit settings. Begin. */
	@I18NMessage("<h3>Database Audit Settings</h3>"
			+ "<p>"
			+ " Specify the type of audit data you want to collect on the selected databases "
			+ "</p>"
			+ "<p>"
			+ " <a target='_blank' style='color: #FE4210' href='http://wiki.idera.com/display/SQLCM/Reduce+audit+data+to+optimize+performance'>Learn how to optimize performance with audit settings</a> "
			+ "</p>")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_TIPS = "SQLCM.Html.add-server-wizard.audit-settings.tips";

	@I18NMessage("<h3>Server Audit Settings</h3>"
			+ "<p>"
			+ " Select the type of audit data you want to collect for this SQL Server instance. "
			+ "</p>"
			+ "<p>"
			+ " <a target='_blank' style='color: #FE4210' href='http://wiki.idera.com/display/SQLCM/Reduce+audit+data+to+optimize+performance'>Learn how to optimize performance with audit settings</a> "
			+ "</p>")
	String ADD_SERVER_WIZARD_SERVER_AUDIT_SETTINGS_TIPS = "SQLCM.Html.add-server-wizard.server-audit-settings.tips";

	@I18NMessage("Audited Activity ")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_ACTIVITY = "SQLCM.Label.add-server-wizard.audit-settings.audited-activity";

	@I18NMessage("Security Changes")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_SECURITY_CHANGES = "SQLCM.Label.add-server-wizard.audit-settings.security-changes";

	@I18NMessage("Security Changes (e.g. GRANT, REVOKE, LOGIN CHANGE PWD)")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_SECURITY_CHANGES_E_G = "SQLCM.Label.add-server-wizard.audit-settings.security-changes-e-g";

	@I18NMessage("Database Definition (DDL)")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_DDL = "SQLCM.Label.add-server-wizard.audit-settings.ddl";

	@I18NMessage("Database Definition (DDL) (e.g. CREATE or DROP DATABASE)")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_DDL_E_G = "SQLCM.Label.add-server-wizard.audit-settings.ddl-e-g";

	@I18NMessage("Administrative Actions")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_ADMIN_ACTIVITIES = "SQLCM.Label.add-server-wizard.audit-settings.admin-activities";

	@I18NMessage("Administrative Actions (e. g. DBCC)")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_ADMIN_ACTIVITIES_E_G = "SQLCM.Label.add-server-wizard.audit-settings.admin-activities-e-g";

	@I18NMessage("User Defined Events (custom SQL Server event type)")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_USER_DEFINED_EVENTS = "SQLCM.Label.add-server-wizard.audit-settings.user-defined-events";

	@I18NMessage("Database Modification (DML)")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_DML = "SQLCM.Label.add-server-wizard.audit-settings.dml";

	@I18NMessage("Database SELECTs")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_DATABASE_SELECTS = "SQLCM.Label.add-server-wizard.audit-settings.database-selects";

	@I18NMessage("Filter events based on access check:")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_DATABASE_FILTER_EVENTS = "SQLCM.Label.add-server-wizard.audit-settings.filter-events";

	@I18NMessage("Capture SQL statements for DML and SELECT activities")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_DATABASE_CAPTURE_SQL = "SQLCM.Label.add-server-wizard.audit-settings.capture-sql";

	@I18NMessage("Capture Transaction Status for DML Activity")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_DATABASE_CAPTURE_TRANSACTION = "SQLCM.Label.add-server-wizard.audit-settings.capture-transaction";

	@I18NMessage("Passed")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_DATABASE_ONLY_PASSED = "SQLCM.Label.add-server-wizard.audit-settings.only-passed";

	@I18NMessage("Failed")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_DATABASE_ONLY_FAILED = "SQLCM.Label.add-server-wizard.audit-settings.only-failed";

	@I18NMessage("Access Check Filter")
	String ADD_SERVER_WIZARD_AUDIT_SETTINGS_DATABASE_ACCESS_CHECK_FILTER = "SQLCM.Label.add-server-wizard.audit-settings.access-check-filter";
	/* Add databases: flow 2 step database audit settings. End. */

	@I18NMessage("<h3>Default Permissions</h3>"
			+ "<p>"
			+ " Select the default level of access to audit data for this SQL Server instance. "
			+ "</p>"
			+ "<p>"
			+ " SQL Compliance Manager creates a database for each registered SQL Server instance"
			+ " to hold collected audit data. Select the default level of access you want to grant users. "
			+ "</p>")
	String ADD_SERVER_WIZARD_DEFAULT_PERMISSIONS_TIPS = "SQLCM.Html.add-server-wizard.default-permissions.tips";

	@I18NMessage("Default Database Permissions")
	String ADD_SERVER_WIZARD_DEFAULT_PERMISSIONS_LABEL = "SQLCM.Label.add-server-wizard.default-permissions.label";

	@I18NMessage("Grant right to read events and their associated SQL statements.")
	String ADD_SERVER_WIZARD_DEFAULT_PERMISSIONS_READ_EVENTS_AND_SQL_STATEMENTS = "SQLCM.Label.add-server-wizard.default-permissions.read-events-and-sql-statements";

	@I18NMessage("Grant right to read events only - To allow users to view the associated SQL"
			+ " statements. You will need to explicitly grant users read access to the database.")
	String ADD_SERVER_WIZARD_DEFAULT_PERMISSIONS_READ_EVENTS_ONLY = "SQLCM.Label.add-server-wizard.default-permissions.read-events-only";

	@I18NMessage("Deny read access by default - To allow users to view events and the"
			+ " associated SQL. You will need to explicitly grant users read access to the"
			+ " database.")
	String ADD_SERVER_WIZARD_DEFAULT_PERMISSIONS_DENY_READ_ACCESS = "SQLCM.Label.add-server-wizard.default-permissions.deny-read-access";

	@I18NMessage("<h3>Specify SQL Server</h3>"
			+ "<p>"
			+ " Specify the SQL Server to register with SQL Compliance Manager."
			+ " Once a SQL Server is registered, you can begin auditing database activity on the server. "
			+ "</p>")
	String ADD_SERVER_WIZARD_SPECIFY_SERVER_TIPS = "SQLCM.Html.add-server-wizard.specify-server.tips";

	@I18NMessage("Enter server name")
	String ADD_SERVER_WIZARD_SPECIFY_SERVER_ENTER_SERVER_NAME = "SQLCM.Label.add-server-wizard.specify-server.enter-server-name";

	@I18NMessage("SQL Server:")
	String ADD_SERVER_WIZARD_SPECIFY_SERVER_SQL_SERVER = "SQLCM.Label.add-server-wizard.specify-server.sql-server";
	
	@I18NMessage("..")
	String ADD_SERVER_WIZARD_SPECIFY_SERVER_SQL_BROWSE = "SQLCM.Label.add-server-wizard.specify-server.sql-browse";

	@I18NMessage("Browse")
	String ADD_SERVER_WIZARD_SPECIFY_SERVER_SELECT_SERVER_BUTTON = "SQLCM.Label.add-server-wizard.specify-server.select-server-button";

	@I18NMessage("Description:")
	String ADD_SERVER_WIZARD_SPECIFY_SERVER_DESC = "SQLCM.Label.add-server-wizard.specify-server.desc";

	@I18NMessage("    This SQL Server instance is already registed for auditing.")
	String ADD_SERVER_WIZARD_SPECIFY_SERVER_ALREADY_REGISTED = "SQLCM.Label.add-server-wizard.specify-server.already-registed";
	// -----------------
	@I18NMessage("Select SQL Server")
	String SERVER_LIST_POPUP_TITLE = "SQLCM.Label.server-list-popup.title";

	@I18NMessage("Select SQL Server:")
	String SERVER_LIST_POPUP_LABEL = "SQLCM.Label.server-list-popup.label";

	@I18NMessage("Error loading Server's name list")
	String SERVER_LIST_POPUP_ERROR_LOAD_SERVERS_NAMES = "SQLCM.Label.server-list-popup.error-load-servers-names";
	// ----------------
	@I18NMessage("<h3>Existing Audit Data </h3>"
			+ "<p>"
			+ " Audit data for this SQL Server instance already exists. "
			+ "</p>"
			+ "<p>"
			+ " A database containing audit data for this SQL Server instance already exists."
			+ " To avoid losing any previously collected audit data, use the existing database and keep the existing audit data. "
			+ "</p>")
	String ADD_SERVER_WIZARD_EXISTING_AUDIT_DATA_TIPS = "SQLCM.Html.add-server-wizard.existing-audit-data.tips";

	@I18NMessage("Keep the previously collected audit data and use the existing database."
			+ " If the events database is from a previous version of SQL Compliance Manager, it will be automatically upgraded.")
	String ADD_SERVER_WIZARD_EXISTING_AUDIT_DATA_KEEP_PREVIOUS = "SQLCM.Label.add-server-wizard.existing-audit-data.keep-previous";

	@I18NMessage("Delete the previously collected audit data but use the existing database."
			+ " This option will reinitialize the existing database. All audit data in the database will be permanently deleted.")
	String ADD_SERVER_WIZARD_EXISTING_AUDIT_DATA_DELETE_PREVIOUS = "SQLCM.Label.add-server-wizard.existing-audit-data.delete-previous";

	@I18NMessage("Database:")
	String ADD_SERVER_WIZARD_EXISTING_AUDIT_DATA_DB_LABEL = "SQLCM.Label.add-server-wizard.existing-audit-data.db-label";
	// ----------------
	@I18NMessage("<h3> AlwaysOn Availability Group Details </h3>"
			+ "<p>"
			+ " Showing the Databases that are involved in AlwaysOn Availability Group configuration. "
			+ "</p>"
			+ "<p>"
			+ " Register/Add all the Replica SQL Servers for auditing and deploy agent on the"
			+ " replica servers, to make use of AlwaysOn feature for these databases."
			+ "</p>")
	String ADD_SERVER_WIZARD_ALWAYS_ON_TIPS = "SQLCM.Html.add-server-wizard.always-on.tips";

	@I18NMessage("Database Name")
	String ADD_SERVER_WIZARD_ALWAYS_ON_COLUMN_DATABASE_NAME = "SQLCM.Label.add-server-wizard.always-on.column.database-name";

	@I18NMessage("Availability Group")
	String ADD_SERVER_WIZARD_ALWAYS_ON_COLUMN_AVAILABILITY_GROUP = "SQLCM.Label.add-server-wizard.always-on.column.availability-group";

	@I18NMessage("Replica Server")
	String ADD_SERVER_WIZARD_ALWAYS_ON_COLUMN_REPLICA_SERVER = "SQLCM.Label.add-server-wizard.always-on.column.replica-server";
	// ----------------
	@I18NMessage("<h3>SQL Server Cluster</h3>"
			+ "<p>"
			+ " Specify whether this is a virtual SQL Server hosted on a cluster. "
			+ "</p><br />"
			+ "<p>"
			+ " Select whether this SQL Server instance is a virtual SQL Server hosted by Microsoft Cluster Services."
			+ " This choice affects the deployment options available for this SQL Server. "
			+ "</p>")
	String ADD_SERVER_WIZARD_SERVER_CLUSTER_TIPS = "SQLCM.Html.add-server-wizard.server-cluster.tips";

	@I18NMessage("This SQL Server instance is hosted by a Microsoft SQL Server Cluster virtual server.")
	String ADD_SERVER_WIZARD_SERVER_CLUSTER_SERVER_HOSTED_BY = "SQLCM.Label.add-server-wizard.server-cluster.server-hosted-by";
	// ----------------------
	@I18NMessage("<h3>SQLcompIiance Agent Deployment</h3>"
			+ "<p>"
			+ " Specify the deployment option for this instance's agent. "
			+ "</p><br />"
			+ "<p>"
			+ " A SQLcompIiance Agent must be deployed to the computer hosting each audited SQL Server."
			+ " Auditing cannot be enabled until the agent has been deployed. Select the deployment option to use for this agent: "
			+ "</p>")
	String ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_TIPS = "SQLCM.Html.add-server-wizard.agent-deployment.tips";

	@I18NMessage("Deploy Now")
	String ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_DEPLOY_NOW = "SQLCM.Label.add-server-wizard.agent-deployment.deploy-now";

	@I18NMessage("Deploy Later")
	String ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_DEPLOY_LATER = "SQLCM.Label.add-server-wizard.agent-deployment.deploy-later";

	@I18NMessage("Deploy Manually")
	String ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_MANUALLY_DEPLOY = "SQLCM.Label.add-server-wizard.agent-deployment.manually-deploy";

	@I18NMessage("Already Deployed")
	String ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_ALREADY_DEPLOYED = "SQLCM.Label.add-server-wizard.agent-deployment.already-deployed";
	
	@I18NMessage("Error during load agent status call")
	String ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_LOAD_AGENT_STATUS_ERROR = "SQLCM.Label.add-server-wizard.agent-deployment.load-agent-status-error";
	// ----------------------
	@I18NMessage("<h3>Specify directory for temporary storage of audit data</h3>"
			+ "<p>"
			+ " The SQLcompliance Agent temporarily stores collected audit data in a trace directory on the computer that hosts the SQL Server instance."
			+ " Specify whether you want to use the default trace directory path or specify a different path. "
			+ "</p>")
	String ADD_SERVER_WIZARD_AGENT_TRACE_DIR_TIPS = "SQLCM.Html.add-server-wizard.agent-trace-dir.tips";

	@I18NMessage("Note : Use default trace directory - By default, the SQLcompIiance Agent will store collected audit data in a protected subdirectory of the agent installation directory.")
	String ADD_SERVER_WIZARD_TRACE_DIR_USE_DEFAULT_NOTE = "SQLCM.Label.add-server-wizard.agent-trace-dir.use-default-note";

	@I18NMessage("Use default trace directory")
	String ADD_SERVER_WIZARD_TRACE_DIR_USE_DEFAULT = "SQLCM.Label.add-server-wizard.agent-trace-dir.use-default";

	@I18NMessage("Specify alternate trace directory")
	String ADD_SERVER_WIZARD_TRACE_DIR_SPECIFY = "SQLCM.Label.add-server-wizard.agent-trace-dir.specify";

	@I18NMessage("Note: This directory will be created by the agent installation.")
	String ADD_SERVER_WIZARD_TRACE_DIR_NOTE = "SQLCM.Label.add-server-wizard.agent-trace-dir.note";

	@I18NMessage("The trace directory must be a valid local directory path on the SQLcompIiance Agent Computer, "
			+ "may not include relative pathing, and must be 180 characters or less.")
	String ADD_SERVER_WIZARD_TRACE_DIR_INVALID_PATH = "SQLCM.Label.add-server-wizard.agent-trace-dir.invalid-path";
	// ----------------------
	@I18NMessage("<h3>Database Load Error</h3>" + "<p>"
			+ " An error occurred loading the list of databases. " + "</p>")
	String ADD_SERVER_WIZARD_DATABASE_LOAD_ERROR_TIPS = "SQLCM.Html.add-server-wizard.database-load-error.tips";

	@I18NMessage("A connection to the new SQL Server is required before deployment of the SQLcompIiance Agent can continue."
			+ " Make sure that the SQL Server is up and reachable. "
			+ " If you cannot connect to the SQL Server from the Management Console, you will need to manually deploy the SQLcompIiance Agent. ")
	String ADD_SERVER_WIZARD_DATABASE_LOAD_ERROR_MSG = "SQLCM.Label.add-server-wizard.database-load-error.msg";
	// ----------------------
	@I18NMessage("<h3>License Limit Reached</h3>" + "<p>"
			+ " Maximum number of licensed instances reached " + "</p>")
	String ADD_SERVER_WIZARD_LICENSE_LIMIT_REACHED_TIPS = "SQLCM.Html.add-server-wizard.license-limit-reached.tips";

	@I18NMessage("You have already reached the maximum number of registered SQL Server allowed by the current license."
			+ " Please contact IDERA to purchase additional licenses.")
	String ADD_SERVER_WIZARD_LICENSE_LIMIT_REACHED_MSG = "SQLCM.Label.add-server-wizard.license-limit-reached.msg";
	// ----------------------
	@I18NMessage("<h3>SQLcompIiance Agent Service Account</h3>"
			+ "<p>"
			+ " Specify the service options This account needs to be given SQL Server Administrator privileges on the registered SQL Server. "
			+ "</p>"
			+ "<p>"
			+ " Note: The login specified for the SQLcompIiance Agent service account needs to be a valid domain account with appropriate permissions for"
			+ " creating traces and stored procedures on the registered SQL Server. "
			+ "</p>")
	String ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_TIPS = "SQLCM.Html.add-server-wizard.agent-service-account.tips";

	@I18NMessage("SQLcompIiance Agent Service Account")
	String ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_GROUP_LABEL = "SQLCM.Label.add-server-wizard.agent-service-account.group-label";

	@I18NMessage("Login account")
	String ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_LOGIN_ACCOUNT = "SQLCM.Label.add-server-wizard.agent-service-account.login-account";

	@I18NMessage("Password")
	String ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_PASSWORD = "SQLCM.Label.add-server-wizard.agent-service-account.password";

	@I18NMessage("Confirm password")
	String ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_CONFIRM_PASSWORD = "SQLCM.Label.add-server-wizard.agent-service-account.confirm-password";

	@I18NMessage("Enter Login. Format <domain>\\\\<user>")
	String ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_ENTER_LOGIN_ACCOUNT = "SQLCM.Label.add-server-wizard.agent-service-account.enter-login-account";

	@I18NMessage("Password fields don't match")
	String ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_PASSWORD_DONT_MATCH = "SQLCM.Label.add-server-wizard.agent-service-account.password-dont-match";

	@I18NMessage("    The domain account credentials supplied could not be verified.")
	String ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_CREDENTIALS_COULD_NOT_BE_VERIFIED = "SQLCM.Label.add-server-wizard.agent-service-account.credentials-could-not-be-verified";

	@I18NMessage("Error during CanAddOneMoreInstance call")
	String ADD_SERVER_WIZARD_LICENSE_LIMIT_REACHED_CALL_ERROR = "SQLCM.Label.add-server-wizard.license-limit-reached.call-error";

	@I18NMessage("Error during validate credentials call")
	String ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_CREDENTIALS_ERROR_VALIDATE_CALL = "SQLCM.Label.add-server-wizard.agent-service-account.error-validate-call";

	@I18NMessage("SQLcompliance Agent could not be installed. Access is denied.")
	String ADD_SERVER_WIZARD_AGENT_SERVICE_ACCESS_IS_DENIED = "SQLCM.Label.add-server-wizard.agent-service-account.access-is-denied";
	// ----------------------
	@I18NMessage("OK")
	String SELECT_SERVER_DIALOG_OK_BUTTON = "Labels.sql-cm.select-server-dialog.ok-button";

	@I18NMessage("Cancel")
	String SELECT_SERVER_DIALOG_CANCEL_BUTTON = "Labels.sql-cm.select-server-dialog.cancel-button";

	@I18NMessage("<h3>DML and SELECT Audit Filters</h3>"
			+ "<p>"
			+ " Select the database objects you want to audit for DML and SELECT activities. "
			+ "</p>")
	String ADD_SERVER_WIZARD_AUDIT_FILTERS_TIPS = "SQLCM.Html.add-server-wizard.dml-and-select-audit-filters.tips";

	@I18NMessage("Audit all database objects")
	String ADD_SERVER_WIZARD_AUDIT_ALL_DB_OBJECTS = "SQLCM.Label.add-server-wizard.dml-and-select-audit-filters.audit-all-db-objects";

	@I18NMessage("Audit the following database objects")
	String ADD_SERVER_WIZARD_AUDIT_FILTERS_AUDIT_FOLLOWING_DB_OBJECTS = "SQLCM.Label.add-server-wizard.dml-and-select-audit-filters.audit-following-db-objects";

	@I18NMessage("Audit user tables")
	String ADD_SERVER_WIZARD_AUDIT_FILTERS_AUDIT_USER_TABLES = "SQLCM.Label.add-server-wizard.dml-and-select-audit-filters.audit-user-tables";

	@I18NMessage("Audit system tables")
	String ADD_SERVER_WIZARD_AUDIT_FILTERS_AUDIT_SYSTEM_TABLES = "SQLCM.Label.add-server-wizard.dml-and-select-audit-filters.audit-system-tables";

	@I18NMessage("Audit stored procedures")
	String ADD_SERVER_WIZARD_AUDIT_FILTERS_AUDIT_STORED_PROCEDURES = "SQLCM.Label.add-server-wizard.dml-and-select-audit-filters.audit-stored-procedures";

	@I18NMessage("Audit all other object types (views, indexes, etc.)")
	String ADD_SERVER_WIZARD_AUDIT_FILTERS_AUDIT_ALL_OTHER_OBJECT_TYPES = "SQLCM.Label.add-server-wizard.dml-and-select-audit-filters.audit-all-other-object-types";

	/* Add databases: flow 2 step trusted users. Begin. */
	@I18NMessage("<h3>Trusted Users</h3>"
			+ "<p>"
			+ " Select users whose activities you never want collected, regardless of other audit settings "
			+ "</p>"
			+ "<br /><br />"
			+ "<p>"
			+ " Note: Adding trusted users during server registration can only be done if you have permission to list server roles and logins."
			+ " If the SQL Server is in a non-trusted domain, you will not be able to add trusted users until"
			+ " the SQLcompliance agent is deployed and the server registration is complete."
			+ "</p>")
	String ADD_SERVER_WIZARD_TRUSTED_USERS_TIPS = "SQLCM.Html.add-server-wizard.trusted-users.tips";

	@I18NMessage("Trusted Users:")
	String ADD_SERVER_WIZARD_AUDIT_TRUSTED_USERS_TRUSTED_USERS = "SQLCM.Label.add-server-wizard.trusted-users.trusted-users";
	/* Add databases: flow 2 step trusted users. End. */

	/* Add databases: flow 3 step regulation guidelines. Begin. */
	@I18NMessage("<h3>Regulation Guidelines</h3>"
			+ "<p>"
			+ " Select the regulation(s) you want to apply to these audited databases. "
			+ "</p>"
			+ "<br /><br />"
			+ "<p>"
			+ " Each regulation configures your audit settings according to its specific guidelines."
			+ " You can apply one or more regulations, depending on the event data you need to collect."
			+ "</p>")
	String ADD_SERVER_WIZARD_REG_GUIDE_TIPS = "SQLCM.Html.add-server-wizard.reg-guide.tips";

	@I18NMessage("Guidelines")
	String ADD_SERVER_WIZARD_REG_GUIDE_GUIDELINES = "SQLCM.Label.add-server-wizard.reg-guide.guidelines";

	@I18NMessage("PCI DSS")
	String ADD_SERVER_WIZARD_REG_GUIDE_PCI_DSS = "SQLCM.Label.add-server-wizard.reg-guide.pci-dss";

	@I18NMessage("HIPPA")
	String ADD_SERVER_WIZARD_REG_GUIDE_HIPPA = "SQLCM.Label.add-server-wizard.reg-guide.hippa";

	@I18NMessage("The current audit settings for the selected database(s) will be overridden when the regulation is applied.")
	String ADD_SERVER_WIZARD_REG_GUIDE_CURRENT_AUDIT_SETT = "SQLCM.Label.add-server-wizard.reg-guide.current-audit-sett";

	/* Add databases: flow 3 step regulation guidelines. End. */

	/* Add databases: flow 3 step regulation guidelines APPLY. Begin. */
	@I18NMessage("<h3>Regulation Guidelines</h3>" + "<p>"
			+ " Apply selected regulation guidelines " + "</p>")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_TIPS = "SQLCM.Html.add-server-wizard.reg-guide-apply.tips";

	@I18NMessage("You selected {0} regulatory guidelines as your audit level.")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_SELECTED_GUIDELINES = "SQLCM.Label.add-server-wizard.reg-guide-apply.selected-guidelines";

	@I18NMessage("PCI DSS")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_PCI_DSS_GUIDELINE = "SQLCM.Label.add-server-wizard.reg-guide-apply.pci-dss-guidelines";

	@I18NMessage("HIPAA")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_HIPAA_GUIDELINE = "SQLCM.Label.add-server-wizard.reg-guide-apply.hipaa-guidelines";

	@I18NMessage("\\u0020and\\u0020")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_SELECTED_GUIDELINES_AND = "SQLCM.Label.add-server-wizard.reg-guide-apply.guidelines-and";

	@I18NMessage("The SQLcompliance Agent will automatically collect the following server and database events:")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_DB_EVENTS = "SQLCM.Label.add-server-wizard.reg-guide-apply.db-events";

	@I18NMessage("Failed Logins")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_FAILED_LOGINS = "SQLCM.Label.add-server-wizard.reg-guide-apply.failed-logins";

	@I18NMessage("Administrative Actions")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_ADMIN_ACTIVITIES = "SQLCM.Label.add-server-wizard.reg-guide-apply.admin-activities";

	@I18NMessage("Security Changes")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_SECURITY_CHANGES = "SQLCM.Label.add-server-wizard.reg-guide-apply.security-changes";

	@I18NMessage("Database Definition")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_DB_DEFINITION = "SQLCM.Label.add-server-wizard.reg-guide-apply.db-definition";

	@I18NMessage("Database Modification")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_DB_MODIFICATION = "SQLCM.Label.add-server-wizard.reg-guide-apply.db-modification";

	@I18NMessage("Privileged users")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_PRIV_USERS = "SQLCM.Label.add-server-wizard.reg-guide-apply.priv-users";

	@I18NMessage("Privileged user Events")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_PRIV_USER_EVENTS = "SQLCM.Label.add-server-wizard.reg-guide-apply.priv-user-events";

	@I18NMessage("Sensitive Column Auditing")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_SENS_COLUMN_AUDITING = "SQLCM.Label.add-server-wizard.reg-guide-apply.sens-column-auditing";

	@I18NMessage("To successfully comply with the selected regulation guideline,"
			+ " you will need to configure the following audit settings:")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_AUDIT_SETTINGS = "SQLCM.Label.add-server-wizard.reg-guide-apply.audit-settings";

	@I18NMessage("Sensitive Column Auditing")
	String ADD_SERVER_WIZARD_REG_GUIDE_APPLY_SENS_COL_AUDITING = "SQLCM.Label.add-server-wizard.reg-guide-apply.sens-col-auditing";
	/* Add databases: flow 3 step regulation guidelines APPLY. End. */

	/* Add databases: flow 3 step Privileged Users. Begin. */
	@I18NMessage("<h3>Privileged Users</h3>"
			+ "<p>"
			+ " Select users whose activities you never want collected, regardless of other audit settings "
			+ "</p>"
			+ "<br /><br />"
			+ "<p>"
			+ " Note: Adding privileged users can only be done if you have permission to list server roles and logins."
			+ " If the SQL Server is in a non-trusted domain, you will not be able to add privileged users until"
			+ " the SQLcompliance agent is deployed and the server registration is complete."
			+ "</p>")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_TIPS = "SQLCM.Html.add-server-wizard.privileged-users.tips";

	@I18NMessage("Audited Privileged Users:")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED = "SQLCM.Label.add-server-wizard.privileged-users.audited";
	/* Add databases: flow 3 step Privileged Users. End. */

	/* Add databases: flow 3 step Privileged User Audited Activity. Begin. */
	@I18NMessage("<h3>Privileged User Audited Activity</h3>" + "<p>"
			+ " Select which activities to audit for privileged users "
			+ "</p>")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_TIPS = "SQLCM.Html.add-server-wizard.privileged-users-audited-activity.tips";

	@I18NMessage("Audit all activities done by privileged users")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_AUDIT_ALL = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.audit-all";

	@I18NMessage("Audit selected activities done by privileged users ")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_AUDIT_SELECTED = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.audit-selected";

	@I18NMessage("Logins")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_LOGINS = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.logins";

	@I18NMessage("Failed logins")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_FAILED_LOGINS = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.failed-logins";

	@I18NMessage("Security changes ")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_SECURITY_CHANGES = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.security-changes";

	@I18NMessage("Administrative actions")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_ADMIN_ACTIONS = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.admin-actions";

	@I18NMessage("Database Definition (DDL)")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_DDL = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.ddl";

	@I18NMessage("Database Modification (DML)")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_DML = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.dml";

	@I18NMessage("Database SELECT s")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_DB_SELECTS = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.db-selects";

	@I18NMessage("User Defined Events")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_DEFINED_EVENTS = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.defined-events";

	@I18NMessage("Filter events based on access check:")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_FILTER_EVENTS = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.filter-events";

	@I18NMessage("Passed")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_PASSED = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.passed";

	@I18NMessage("Failed")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_FAILED = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.failed";

	@I18NMessage("Capture SQL statements for DML and SELECT activities")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_CAPTURE_SQL = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.capture-sql";

	@I18NMessage("Capture Transaction Status for DML Activity")
	String ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_CAPTURE_TRANSACTION = "SQLCM.Label.add-server-wizard.privileged-users.audited-activity.capture-transaction";

	@I18NMessage("<h3>Sensitive Columns</h3>"
			+ "<p>"
			+ " Select the tables you want to audit for sensitive column access. "
			+ "</p>"
			+ "<p>"
			+ " For each database, enter the tables that you would like audited for Sensitive Column Access."
			+ " For each table, all columns will be audited. " + "</p>")
	String ADD_SERVER_WIZARD_SENSITIVE_COLUMNS_TIPS = "SQLCM.Html.add-server-wizard.sensitive-columns.tips";

	@I18NMessage("Tables")
	String ADD_SERVER_WIZARD_SENSITIVE_COLUMNS_TABLES = "SQLCM.Label.add-server-wizard.sensitive-columns.tables";

	@I18NMessage("Sensitive Column auditing is not available until an agent is deployed to audit the server"
			+ " and a heartbeat has been received.")
	String ADD_SERVER_WIZARD_SENSITIVE_COLUMNS_NOT_AVAILABLE = "SQLCM.Label.add-server-wizard.sensitive-columns.not-available";
	/* Add databases: flow 3 step Privileged User Audited Activity. End. */

	@I18NMessage("Add")
	String ADD_SERVER_WIZARD_BUTTON_ADD = "SQLCM.Label.add-server-wizard.button.add";

	@I18NMessage("Remove")
	String ADD_SERVER_WIZARD_BUTTON_REMOVE = "SQLCM.Label.add-server-wizard.button.remove";

	@I18NMessage("You are already auditing all available user and system databases from the selected SQL Server.")
	String ADD_SERVER_WIZARD_ALL_ALREADY_AUDITED = "SQLCM.Label.add-server-wizard.all-already-audited";

	@I18NMessage("Regulation details")
	String REGULATION_DETAILS_DIALOG_TITLE = "Labels.sql-cm.regulation-details-dialog-title";

	@I18NMessage("<h3>Regulation Details</h3>"
			+ "<p>"
			+ " This is a summary of the Regulation Guidelines that you are applying to the selected databases. "
			+ "</p>")
	String REGULATION_DETAILS_DIALOG_TIPS = "Labels.sql-cm.regulation-details-dialog-tips";

	@I18NMessage("Server events")
	String REGULATION_DETAILS_DIALOG_SERVER_EVENTS = "Labels.sql-cm.regulation-details-dialog-server-events";

	@I18NMessage("Database events")
	String REGULATION_DETAILS_DIALOG_DATABASE_EVENTS = "Labels.sql-cm.regulation-details-dialog-database-events";

	@I18NMessage("Close")
	String REGULATION_DETAILS_DIALOG_CLOSE_BUTTON = "Labels.sql-cm.regulation-details-dialog-close-button";

	@I18NMessage("Add User Tables for DML/Select Auditing")
	String USER_TABLES_FOR_DML_SELECT_DIALOG_TITLE = "Labels.sql-cm.select-tables-to-audit-for-dml-select-dialog.title";

	@I18NMessage("Add User Tables for Before-After Data Auditing")
	String USER_TABLES_FOR_BEFORE_AFTER_DATA_DIALOG_TITLE = "Labels.sql-cm.select-tables-to-audit-for-before-after-data-dialog.title";

	@I18NMessage("Add User Tables for Sensitive Columns Auditing")
	String SENSITIVE_COLUMNS_DIALOG_TITLE = "Labels.sql-cm.select-tables-to-audit-dialog.title";

	@I18NMessage("Add User Tables for Sensitive Columns Auditing")
	String USER_TABLES_SENSITIVE_COLUMNS_DIALOG_TITLE = "Labels.sql-cm.select-tables-to-audit-dialog.title";

	@I18NMessage("Add ->")
	String ADD_SERVER_WIZARD_BUTTON_ADD_ARROW = "SQLCM.Label.add-server-wizard.button.add-arrow";

	@I18NMessage("<- Remove")
	String ADD_SERVER_WIZARD_BUTTON_REMOVE_ARROW = "SQLCM.Label.add-server-wizard.button.remove-arrow";

	@I18NMessage("Warning: Selecting to capture operation low level details can increase the amount of information gathered for this database significantly. It is recommended that this option be left off unless absolutely needed.")
	String ADD_SERVER_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE = "SQLCM.Label.add-server-wizard.low-level-details-can-increase";

	@I18NMessage("Failed to load column list.")
	String FAILED_TO_LOAD_COLUMN_LIST = "Labels.failed-to-load-column-list";

	@I18NMessage("Failed to load availability info list")
	String FAILED_TO_LOAD_AVAILABILITY_INFO_LIST = "Labels.failed-to-load-availability-info-list";

	@I18NMessage("Failed to update instance.")
	String FAILED_TO_UPDATE_INSTANCE = "Labels.failed-to-update-instance";

	@I18NMessage("Failed to validate manage instance credentials.")
	String FAILED_TO_VALIDATE_MANAGE_INSTANCE_CREDENTIALS = "Labels.failed-to-validate-manage-instance-credentials";

	@I18NMessage("Failed to load event list.")
	String FAILED_TO_LOAD_EVENT_LIST = "Labels.failed-to-load-event-list";

	@I18NMessage("Failed to load instance list.")
	String FAILED_TO_LOAD_INSTANCE_LIST = "Labels.failed-to-load-instance-list";

	@I18NMessage("Failed to load statistic data.")
	String FAILED_TO_LOAD_STATISTIC_DATA = "Labels.failed-to-load-statistic-data";

	@I18NMessage("Failed to load permissions info.")
	String FAILED_TO_LOAD_PERMISSIONS_INFO = "Labels.failed-to-load-permissions-info";

	@I18NMessage("Failed to load audit instance properties.")
	String FAILED_TO_LOAD_AUDIT_INSTANCE_PROPERTIES = "Labels.failed-to-load-audit-instance-properties";

	@I18NMessage("Failed to update audit configuration for server.")
	String FAILED_TO_UPDATE_AUDIT_CONFIGURATION_FOR_SERVER = "Labels.failed-to-update-audit-configuration-for-server";

	@I18NMessage("Failed to update audit server properties.")
	String FAILED_TO_UPDATE_AUDIT_SERVER_PROPERTIES = "Labels.failed-to-update-audit-server-properties";

	@I18NMessage("Failed to load agent properties.")
	String FAILED_TO_LOAD_AGENT_PROPERTIES = "Labels.failed-to-load-agent-properties";

	@I18NMessage("Failed to update agent properties.")
	String FAILED_TO_UPDATE_AGENT_PROPERTIES = "Labels.failed-to-update-agent-properties";

	@I18NMessage("Failed to load database properties.")
	String FAILED_TO_LOAD_DATABASE_PROPERTIES = "Labels.failed-to-load-database-properties";

	@I18NMessage("Failed to update database properties.")
	String FAILED_TO_UPDATE_DATABASE_PROPERTIES = "Labels.failed-to-update-database-properties";

	@I18NMessage("Failed to enable CLR status.")
	String FAILED_TO_ENABLE_CLR_STATUS = "Labels.failed-to-enable-clr-status";

	@I18NMessage("Failed to upgrade agent.")
	String FAILED_TO_UPGRADE_AGENT = "Labels.failed-to-upgrade-agent";

	@I18NMessage("Failed to check agent status.")
	String FAILED_TO_CHECK_AGENT_STATUS = "Labels.failed-to-check-agent-status";

	@I18NMessage("Failed to load CM License.")
	String FAILED_TO_LOAD_LICENSE = "Labels.failed-to-load-license";

	@I18NMessage("Failed to apply new CM License.")
	String FAILED_TO_APPLY_NEW_CM_LICENSE = "Labels.failed-to-apply-new-cm-license";

	@I18NMessage("Failed to import instances.")
	String FAILED_TO_IMPORT_INSTANCES = "Labels.failed-to-import-instances";

	@I18NMessage("Failed to disable/enable auditing.")
	String FAILED_TO_DISABLE_ENABLE_AUDITING = "Labels.failed-to-disable-enable-auditing";
	
	@I18NMessage("Failed to load report.")
	String FAILED_TO_LOAD_REPORT = "Labels.failed-to-load-report";

	@I18NMessage("Failed to remove registered SQL Server instance.")
	String FAILED_TO_REMOVE_AUDIT_SERVER = "Labels.failed-to-remove-audit-server";

	@I18NMessage("Can not remove registered SQL Server instance. {0}")
	String CAN_NOT_REMOVE_AUDIT_SERVER = "Labels.can-not-remove-audit-server";

	@I18NMessage("You do not have any saved view.")
	String YOU_DO_NOT_HAVE_A_FAVORITE_FILTER_SET = "Labels.you-do-not-have-a-favorite-filter-set";

	@I18NMessage("Failed to process loaded view state.")
	String FAILED_TO_PROCESS_LOADED_VIEW_STATE = "Labels.failed-to-process-loaded-view-state";

	@I18NMessage("Failed to process view state for saving.")
	String FAILED_TO_PROCESS_VIEW_STATE_FOR_SAVING = "Labels.failed-to-process-view-state-for-saving";

	@I18NMessage("Failed to prepare time ranges.")
	String FAILED_TO_PREPARE_TIME = "Labels.failed-to-prepare-time-ranges";

	@I18NMessage("Failed to apply reindex for archive.")
	String FAILED_TO_APPLY_REINDEX_FOR_ARCHIVE = "Labels.failed-to-apply-reindex-for-archive";

	@I18NMessage("Failed to load databases for attach archive.")
	String FAILED_TO_LOAD_DATABASES_FOR_ATTACH_ARCHIVE = "Labels.failed-to-load-databases-for-attach-archive";

	@I18NMessage("Failed to load archive properties.")
	String FAILED_TO_LOAD_ARCHIVE_PROPERTIES = "Labels.failed-to-load-archive-properties";

	@I18NMessage("Failed to attach archive.")
	String FAILED_TO_ATTACH_ARCHIVE = "Labels.failed-to-attach-archive";

	@I18NMessage("Failed to upgrade event database schema.")
	String FAILED_TO_UPGRADE_EVENT_DATABASE_SCHEMA = "Labels.failed-to-upgrade-event-database-schema";

	@I18NMessage("Failed to check if need index updates for archive.")
	String FAILED_TO_CHECK_IF_NEED_INDEX_UPDATES = "Labels.failed-to-check-if-need-index-updates";

	@I18NMessage("Failed to update indexes for event database.")
	String FAILED_TO_UPDATE_INDEXES_FOR_EVENT_DATABASE = "Labels.failed-to-update-indexes-for-event-database";

	@I18NMessage("Failed to check: is valid index start time for archive database.")
	String FAILED_TO_CHECK_IS_VALID_START_TIME_FOR_ARCHIVE_DATABASE = "Labels.failed-to-check-is-valid-start-time-for-archive-database";

	@I18NMessage("Failed to load database audited activity.")
	String FAILED_TO_LOAD_DATABASE_AUDITED_ACTIVITY = "Labels.failed-to-load-database-audited-activity";

	@I18NMessage("Failed to load audited databases.")
	String FAILED_TO_LOAD_AUDITED_DATABASES = "Labels.failed-to-load-audited-databases";

	@I18NMessage("View state saved successful")
	String VIEW_STATE_SAVED_SUCCESSFUL = "Labels.view-state-saved-successful";

	@I18NMessage("View state loaded successful")
	String VIEW_STATE_LOADED_SUCCESSFUL = "Labels.view-state-loaded-successful";

	@I18NMessage("Failed to load saved view state.")
	String FAILED_TO_LOAD_SAVED_VIEW_STATE = "Labels.failed-to-load-saved-view-state";

	@I18NMessage("Failed to save view state.")
	String FAILED_TO_SAVE_VIEW_STATE = "Labels.failed-to-save-view-state";

	@I18NMessage("No saved Data.")
	String NO_SAVED_DATA = "Labels.no-saved-data";

	@I18NMessage("Permissions Check for SQL Server Instance {0}:")
	String PERMISSIONS_CHECK_FOR_INSTANCE = "Labels.permissions-check-for-instance";

	@I18NMessage("Error save database")
	String ERROR_SAVE_DATABASE = "Labels.error-save-database";
	@I18NMessage("Error save instance config")
	String ERROR_SAVE_INSTANCE_CONFIG = "Labels.error-save-instance-config";

	@I18NMessage("Add instance error")
	String ERROR_SAVE_INSTANCE = "Labels.error-save-instance";

	@I18NMessage("    SQLcompIiance Agent could not be installed. Access is denied.")
	String ERROR_DEPLOY_AGENT = "Labels.error-deploy-agent";

	@I18NMessage("Error during check instance availability")
	String ERROR_CHECK_INSTANCE_AVAILABILITY = "Labels.error-check-instance-availability";

	@I18NMessage("All Servers are OK")
	String SYSTEM_STATUS_OK = "SQLCM.Label.system-status-ok";

	@I18NMessage("One or more servers need attention")
	String SYSTEM_STATUS_WARNING = "SQLCM.Label.system-status-warning";

	@I18NMessage("One or more servers are broken")
	String SYSTEM_STATUS_ERROR = "SQLCM.Label.system-status-error";

	@I18NMessage("No registered SQL Servers")
	String SYSTEM_STATUS_NO_REGISTERED_INSTANCES = "SQLCM.Label.system-status-no-registered-instances";

	@I18NMessage("critical ")
	String ALERT_CRITICAL = "SQLCM.Label.alert-critical";

	@I18NMessage("warning ")
	String ALERT_WARNING = "SQLCM.Label.alert-warning";

	@I18NMessage("information ")
	String ALERT_INFORMATION = "SQLCM.Label.alert-information";

	@I18NMessage("ok ")
	String ALERT_OK = "SQLCM.Label.alert-ok";

	@I18NMessage("Regulation Guideline(s)")
	String ENHANCED_DATABASE_REGULATION_GUIDELINES = "SQLCM.Label.enhanced.database.regulation.guidelines";

	@I18NMessage("Database")
	String ENHANCED_DATABASE = "SQLCM.Label.enhanced.database";

	@I18NMessage("Before-After")
	String ENHANCED_DATABASE_BEFORE_AFTER = "SQLCM.Label.enhanced.database.before-after";

	@I18NMessage("Sensitive Columns")
	String ENHANCED_DATABASE_SENSITIVE_COLUMNS = "SQLCM.Label.enhanced.database.sensitive-columns";

	@I18NMessage("Trusted Users")
	String ENHANCED_DATABASE_TRUSTED_USERS = "SQLCM.Label.enhanced.database.trusted-users";

	@I18NMessage("Event Filters")
	String ENHANCED_DATABASE_EVENT_FILTERS = "SQLCM.Label.enhanced.database.event-filters";

	@I18NMessage("Enable Auditing")
	String ENABLE_AUDITING = "SQLCM.Labels.enable-auditing";

	@I18NMessage("Disable Auditing")
	String DISABLE_AUDITING = "SQLCM.Labels.disable-auditing";

	// Import SQL server instance. Begin.
	@I18NMessage("Select a file containing instances to be imported into SQL Compliance Manager. The instances names can be separated by commas, semi-colons or line breaks.")
	String IMPORT_SQL_SERVERS_TIP_1 = "SQLCM.Labels.import-sql-servers-tip-1";

	@I18NMessage("Once imported, you can use the bulk operations available on the Instances View to set credentials for monitoring the instances as well as owners, locations and comments.")
	String IMPORT_SQL_SERVERS_TIP_2 = "SQLCM.Labels.import-sql-servers-tip-2";

	@I18NMessage("1. Choose a .csv file to upload")
	String UPLOAD_CSV_FILE_LABEL = "SQLCM.Labels.upload-csv-file";

	@I18NMessage("Upload")
	String UPLOAD_LABEL = "SQLCM.Labels.upload";

	@I18NMessage("2. Select instances to import")
	String SELECT_INSTANCES_TO_IMPORT_LABEL = "SQLCM.Labels.select-instances-to-import";

	@I18NMessage("No instance names available for import.")
	String NO_INSTANCE_NAMES_FOUND = "SQLCM.Labels.no-instance-names-available-to-import";

	@I18NMessage("SQL Compliance Manager is not currently managing any instances")
	String NO_MANAGING_INSTANCES = "SQLCM.Labels.no-managing-instances";

	@I18NMessage("No Alerts")
	String NO_ALERTS_EMPTY_MSG = "SQLCM.Labels.no-alerts-empty-msg";

	@I18NMessage("No Events")
	String NO_EVENTS_EMPTY_MSG = "SQLCM.Labels.no-events-empty-msg";
	
	// SQLCM 5.4 Start
	@I18NMessage("Export Instance Settings")
	String EXPORT_OPTION = "SQLCM.Labels.export_instance_settings";
	// SQLCM 5.4 END
	
	@I18NMessage("IMPORT")
	String IMPORT_BUTTON = "SQLCM.Labels.import";
	// Import SQL server instance. Begin.

	// Manage SQL Server Instances dialog. Begin.
	@I18NMessage("View all your registered SQL Server instances, their respective account type, and the user name used to access the instance.")
	String MANAGE_SQL_SERVERS_TIP_1 = "SQLCM.Labels.manage-sql-servers-tip-1";

	@I18NMessage("You can edit the properties of any selected instance or choose several instances at once and edit their credentials.")
	String MANAGE_SQL_SERVERS_TIP_2 = "SQLCM.Labels.manage-sql-servers-tip-2";

	@I18NMessage("Manage SQL Server Instances")
	String MANAGE_SQL_SERVERS_TITLE = "SQLCM.Labels.manage-sql-servers-title";

	@I18NMessage("Account Type")
	String MANAGE_SQL_SERVERS_ACCOUNT_TYPE = "SQLCM.Labels.manage-sql-servers.account-type";

	@I18NMessage("User Name")
	String MANAGE_SQL_SERVERS_USER_NAME = "SQLCM.Labels.manage-sql-servers.user-name";

	@I18NMessage("Actions")
	String MANAGE_SQL_SERVERS_ACTIONS = "SQLCM.Labels.manage-sql-servers.actions";

	@I18NMessage("Edit")
	String MANAGE_SQL_SERVERS_EDIT_PROPERTIES = "SQLCM.Labels.manage-sql-servers.edit-properties";

	@I18NMessage("Edit Credentials")
	String MANAGE_SQL_SERVERS_EDIT_CREDENTIALS = "SQLCM.Labels.manage-sql-servers.edit-credentials";
	// Manage SQL Server Instances dialog. End.

	// CONFIG REFRESH PAGE SQLCM-2172 5.4 VERSION Start
	@I18NMessage("Configure Web Console Refresh Rate") 
	String MANAGE_REFRESH_D_TITLE = "SQLCM.Labels.manage-refresh-d.title";
	
	@I18NMessage("The minimum refresh rate is 30 seconds and the maximum refresh rate is 3600 seconds.") // SCM-9 
	String MANAGE_REFRESH_D_HEADING = "SQLCM.Labels.manage-refresh-d.heading";
	
	@I18NMessage("Current Refresh Rate") // SQLCM-2172	
	String MANAGE_REFRESH_D_CURRENT_REFRESH_RATE = "SQLCM.Labels.manage-refresh-d.current-refresh-rate";
	
	@I18NMessage("New Refresh Rate:") // SQLCM-2172	
	String MANAGE_REFRESH_D_NEW_REFRESH_RATE = "SQLCM.Labels.manage-refresh-d.new-refresh-rate";
	
	@I18NMessage("sec") // SQLCM-2172	
	String MANAGE_REFRESH_D_SECOND_TEXT = "SQLCM.Labels.manage-refresh-d.second-text";
	
	@I18NMessage("Failed to apply new web console refresh value.") //SQLCM-2172
	String FAILED_TO_APPLY_NEW_REFRESH_VALUE = "Labels.failed-to-apply-new-refresh_value";
	// SQLCM-2172 5.4 VERSION end
	
	@I18NMessage("Tips")
	String WIZARD_TIPS = "SQLCM.Label.wizard.tips";

	@I18NMessage("You have {0} alerts")
	String YOU_HAVE_X_ALERTS = "SQLCM.labels.you-have-x-alerts";

	// StatsCategory Enum. Begin.

	@I18NMessage("Alerts ({0})")
	String ALERTS_COUNT = "SQLCM.labels.alerts-count";

	@I18NMessage("No Alerts")
	String NO_ALERTS = "SQLCM.labels.no-alerts";

	@I18NMessage("Show all alerts")
	String SHOW_ALL_ALERTS = "SQLCM.labels.show-all-alerts";

	@I18NMessage("Show fewer alerts")
	String SHOW_FEWER_ALERTS = "SQLCM.labels.show-fewer-alerts";

	@I18NMessage("Critical")
	String ALERTS_SEVERITY_CRITICAL = "SQLCM.labels.alerts-severity-critical";

	@I18NMessage("Warning")
	String ALERTS_SEVERITY_WARNING = "SQLCM.labels.alerts-severity-warning";

	@I18NMessage("Info")
	String ALERTS_SEVERITY_INFO = "SQLCM.labels.alerts-severity-info";

	@I18NMessage("OK")
	String ALERTS_SEVERITY_OK = "SQLCM.labels.alerts-severity-ok";

	@I18NMessage("Data Alert")
	String ALERTS_TYPE_DATA = "SQLCM.labels.alerts-type-data";

	@I18NMessage("Data Alerts")
	String ALERTS_TYPE_DATA_PLURAL = "SQLCM.labels.alerts-type-data-plural";

	@I18NMessage("Event Alert")
	String ALERTS_TYPE_EVENT = "SQLCM.labels.alerts-type-event";

	@I18NMessage("Event Alerts")
	String ALERTS_TYPE_EVENT_PLURAL = "SQLCM.labels.alerts-type-event-plural";

	@I18NMessage("Status Alert")
	String ALERTS_TYPE_STATUS = "SQLCM.labels.alerts-type-status";

	@I18NMessage("Status Alerts")
	String ALERTS_TYPE_STATUS_PLURAL = "SQLCM.labels.alerts-type-status";

	@I18NMessage("Show alerts")
	String SHOW_ALERTS = "SQLCM.labels.show-alerts";

	@I18NMessage("Hide alerts")
	String HIDE_ALERTS = "SQLCM.labels.hide-alerts";

	@I18NMessage("Properties")
	String ALERT_PROPERTIES = "SQLCM.labels.alert-properties";

	@I18NMessage("Click show details for more information.")
	String SHOW_ALERTS_DETAILS = "SQLCM.labels.show-alerts-details";

	@I18NMessage("Instance")
	String ALERT_INSTANCE = "SQLCM.labels.alert-instance";

	@I18NMessage("Rule")
	String ALERT_RULE = "SQLCM.labels.alert-rule";

	@I18NMessage("Rule Type")
	String ALERT_RULE_TYPE = "SQLCM.labels.alert-rule-type";

	@I18NMessage("Dismiss Alert")
	String DISMISS_ALERT = "SQLCM.messages.dismiss-alert";

	@I18NMessage("Dismissing an alert notification will cause SQL CM to ignore this alert temporarily. "
			+ "You will not receive additional alert notifications in the SQL CM Home page until the alert condition passes "
			+ "and then goes into alert status again.  Are you sure you want to dismiss this alert?")
	String DISMISS_ALERT_WARNING = "SQLCM.messages.dismiss-alert-warning";

	@I18NMessage("Export Alerts")
	String EXPORT_ALERTS = "SQLCM.labels.export-alerts";

	@I18NMessage("Summary of alert categories")
	String SUMMARY_OF_ALERT_CATEGORIES = "Labels.summary-of-alert-categories";

	@I18NMessage("Details for all alerts")
	String DETAILS_FOR_ALL_ALERTS = "Labels.details-for-all-alerts";

	// Agent properties dialog. Begin.
	@I18NMessage("SQL compliance Agent Properties")
	String AGENT_PROPERTIES_D_TITLE = "SQLCM.agent-properties-d.title";

	// tabs
	@I18NMessage("General")
	String AGENT_PROPERTIES_D_GENERAL_TAB = "SQLCM.agent-properties-d.general-tab";

	@I18NMessage("Deployment")
	String AGENT_PROPERTIES_D_DEPLOYMENT_TAB = "SQLCM.agent-properties-d.deployment-tab";

	@I18NMessage("SQL Servers")
	String AGENT_PROPERTIES_D_SQL_SERVERS_TAB = "SQLCM.agent-properties-d.sql-servers-tab";

	@I18NMessage("Trace Options")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB = "SQLCM.agent-properties-d.trace-options-tab";

	@I18NMessage("SQLcompliance Agent Computer: ")
	String AGENT_PROPERTIES_D_GENERAL_TAB_COMPUTER = "SQLCM.agent-properties-d.general-tab.computer";

	@I18NMessage("Agent Settings")
	String AGENT_PROPERTIES_D_GENERAL_TAB_AGENT_SETTINGS = "SQLCM.agent-properties-d.general-tab.agent-settings";

	@I18NMessage("Agent status:")
	String AGENT_PROPERTIES_D_GENERAL_TAB_AGENT_STATUS = "SQLCM.agent-properties-d.general-tab.agent-status";

	@I18NMessage("Agent version:")
	String AGENT_PROPERTIES_D_GENERAL_TAB_AGENT_VERSION = "SQLCM.agent-properties-d.general-tab.agent-version";

	@I18NMessage("Agent port:")
	String AGENT_PROPERTIES_D_GENERAL_TAB_AGENT_PORT = "SQLCM.agent-properties-d.general-tab.agent-port";

	@I18NMessage("Last heartbeat:")
	String AGENT_PROPERTIES_D_GENERAL_TAB_LAST_HEARTBEAT = "SQLCM.agent-properties-d.general-tab.last-heartbeat";

	@I18NMessage("Heartbeat interval (min):")
	String AGENT_PROPERTIES_D_GENERAL_TAB_HEARTBEAT_INTERVAL = "SQLCM.agent-properties-d.general-tab.heartbeat-interval";

	@I18NMessage("Logging level:")
	String AGENT_PROPERTIES_D_GENERAL_TAB_LOGGING_LEVEL = "SQLCM.agent-properties-d.general-tab.logging-level";

	@I18NMessage("Audit Settings")
	String AGENT_PROPERTIES_D_GENERAL_TAB_AUDIT_SETTINGS = "SQLCM.agent-properties-d.general-tab.audit-settings";

	@I18NMessage("Last agent update:")
	String AGENT_PROPERTIES_D_GENERAL_TAB_LAST_AGENT_UPDATE = "SQLCM.agent-properties-d.general-tab.last-agent-update";

	@I18NMessage("Audit settings status:")
	String AGENT_PROPERTIES_D_GENERAL_TAB_AUDIT_SETTINGS_STATUS = "SQLCM.agent-properties-d.general-tab.audit-settings-status";

	@I18NMessage("Audit settings level at agent:")
	String AGENT_PROPERTIES_D_GENERAL_TAB_AGENT_AUDIT_SETTINGS_LEVEL = "SQLCM.agent-properties-d.general-tab.agent-audit-settings-level";

	@I18NMessage("Current audit settings level:")
	String AGENT_PROPERTIES_D_GENERAL_TAB_CURRENT_AUDIT_SETTINGS_LEVEL = "SQLCM.agent-properties-d.general-tab.current-audit-settings-level";

	@I18NMessage("Update Now")
	String AGENT_PROPERTIES_D_GENERAL_TAB_UPDATE_NOW = "SQLCM.agent-properties-d.general-tab.update-now";

	@I18NMessage("SQLcompliance Agent Service")
	String AGENT_PROPERTIES_D_DEPLOYMENT_TAB_AGENT_SERVICE = "SQLCM.agent-properties-d.deployment-tab.agent-service";

	@I18NMessage("Service account: ")
	String AGENT_PROPERTIES_D_DEPLOYMENT_TAB_SERVICE_ACCOUNT = "SQLCM.agent-properties-d.deployment-tab.service-account";

	@I18NMessage("SQLcompliance Agent Deployment")
	String AGENT_PROPERTIES_D_DEPLOYMENT_TAB_AGENT_DEPLOYMENT = "SQLCM.agent-properties-d.deployment-tab.agent-deployment";

	@I18NMessage("The SQLcompliance Agent is responsible for gathering and sending collected audit data for the "
			+ "following registered SQL Servers:")
	String AGENT_PROPERTIES_D_SQL_SERVERS_TAB_NOTE = "SQLCM.agent-properties-d.sql-servers-tab.note";

	@I18NMessage("SQL Server")
	String AGENT_PROPERTIES_D_SQL_SERVERS_TAB_SQL_SERVER_COLUMN = "SQLCM.agent-properties-d.sql-servers-tab.sql-server-column";

	@I18NMessage("Description")
	String AGENT_PROPERTIES_D_SQL_SERVERS_TAB_DESCRIPTION_COLUMN = "SQLCM.agent-properties-d.sql-servers-tab.description-column";

	@I18NMessage("SQLcompliance Agent Trace Directory")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_DIRECTORY = "SQLCM.agent-properties-d.trace-options-tab.directory";

	@I18NMessage("Trace directory: ")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_TRACE_DIRECTORY = "SQLCM.agent-properties-d.trace-options-tab.trace-directory";

	@I18NMessage("Trace Collection Options")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_TRACE_COLLECTION_OPTIONS = "SQLCM.agent-properties-d.trace-options-tab.trace-collection-options";

	@I18NMessage("Trace file rollover size (MB): ")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_FILE_ROLLOVER_SIZE = "SQLCM.agent-properties-d.trace-options-tab.file-rollover-size";

	@I18NMessage("Collection interval (min): ")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_COLLECTION_INTERVAL = "SQLCM.agent-properties-d.trace-options-tab.collection-interval";

	@I18NMessage("Force collection interval (min): ")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_FORCE_COLLECTION_INTERVAL = "SQLCM.agent-properties-d.trace-options-tab.force-collection-interval";

	@I18NMessage("Trace start timeout (sec): ")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_TRACE_START_TIMEOUT = "SQLCM.agent-properties-d.trace-options-tab.trace-start-timeout";

	@I18NMessage("Trace Tamper Detection Options")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_TRACE_TAMPER_DETECTION_OPTIONS = "SQLCM.agent-properties-d.trace-options-tab.trace-tamper-detection-options";

	@I18NMessage("Tamper detection interval (sec): ")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_TRACE_TAMPER_DETECTION_INTERVAL = "SQLCM.agent-properties-d.trace-options-tab.trace-tamper-detection-interval";

	@I18NMessage("Trace Directory Size Limit")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_TRACE_DIRECTORY_SIZE_LIMIT = "SQLCM.agent-properties-d.trace-options-tab.trace-directory-size-limit";

	@I18NMessage("Unattended Auditing Time Limit")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_UNATTENDED_AUDITING_TIME_LIMIT = "SQLCM.agent-properties-d.trace-options-tab.unattended-auditing-time-limit";

	@I18NMessage("Limit trace directory to ")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_TRACE_LIMIT = "SQLCM.agent-properties-d.trace-options-tab.trace-limit";

	@I18NMessage("Limit unattended auditing to ")
	String AGENT_PROPERTIES_D_TRACE_OPTIONS_TAB_UNATTENDED_AUDITING_LIMIT = "SQLCM.agent-properties-d.trace-options-tab.unattended-auditing-limit";
	// Agent properties dialog. End.

	@I18NMessage("Silent")
	String LOGGING_LEVEL_SILENT = "SQLCM.logging-level.silent";

	@I18NMessage("Normal")
	String LOGGING_LEVEL_NORMAL = "SQLCM.logging-level.normal";

	@I18NMessage("Verbose")
	String LOGGING_LEVEL_VERBOSE = "SQLCM.logging-level.verbose";

	@I18NMessage("Debug")
	String LOGGING_LEVEL_DEBUG = "SQLCM.logging-level.debug";

	@I18NMessage("Automatic Deployment - The SQLcompliance Agent for this instance is installed/uninstalled from the SQL Compliance Manager Console.")
	String AGENT_DEPLOYMENT_AUTOMATIC = "SQLCM.agent-deployment.automatic";

	@I18NMessage("Manual Deployment - The SQLcompliance Agent for this instance requires manual installation and "
			+ "uninstallation at the computer hosting this SQL Server instance. This option is required when the agent "
			+ "is located on a virtual server or the computer is located across a trust boundary.")
	String AGENT_DEPLOYMENT_MANUAL = "SQLCM.agent-deployment.manual";

	@I18NMessage("Deployed")
	String AGENT_STATUS_LABEL_DEPLOYED = "SQLCM.agent-status-label.deployed";

	@I18NMessage("Not Deployed")
	String AGENT_STATUS_LABEL_NOT_DEPLOYED = "SQLCM.agent-status-label.not-deployed";

	@I18NMessage("Unlimited")
	String LABEL_UNLIMITED = "SQLCM.label.unlimited";

	@I18NMessage("GB")
	String LABEL_GB = "SQLCM.label.gb";

	@I18NMessage("days")
	String LABEL_DAYS = "SQLCM.label.days";

	@I18NMessage("Remove SQL Server(s)")
	String TITLE_REMOVE_SQL_SERVER = "SQLCM.title.remove-sql-server";

	@I18NMessage("Server is added")
	String TITLE_SQL_SERVER_IS_ADDED = "SQLCM.title.sql-server-is-added";

	@I18NMessage("Warning: when you remove a registered SQL Server, you stop auditing of activities performed on "
			+ "the SQL Server. To temporarily pause auditing of a SQL Server, use the Disable Auditing feature instead. "
			+ "Do you wish to remove the SQL Server now?")
	String MESSAGE_REMOVE_SQL_SERVER = "SQLCM.message.remove-sql-server";

	@I18NMessage("Keep SQL Server Audit Data")
	String TITLE_KEEP_SQL_SERVER_AUDIT_DATA = "SQLCM.title.keep-sql-server-audit-data";

	@I18NMessage("Deleting the audit data collected for an audited SQL Server instance may violate your company's "
			+ "auditing practices. We recommend leaving the database containing the collected audit data  until you are "
			+ "sure it is properly backed up.")
	String MESSAGE_KEEP_SQL_SERVER_AUDIT_DATA_1 = "SQLCM.message.keep-sql-server-audit-data-1";

	@I18NMessage("Do you wish to keep the database containing the audit data for this SQL Server instance?")
	String MESSAGE_KEEP_SQL_SERVER_AUDIT_DATA_2 = "SQLCM.message.keep-sql-server-audit-data-2";

	@I18NMessage("SQL Server(s) has been removed.")
	String MESSAGE_SERVER_REMOVED = "SQLCM.Message.server-removed";

	@I18NMessage("At this step server is added to repository. Do you wish to perform delete SQL Server? ")
	String MESSAGE_DO_YOU_WISH_DELETE_SERVER = "SQLCM.Message.do-you-wish-delete-server";

	@I18NMessage("The SQL compliance Agent service for {0} is no longer capturing audit data. However, because this "
			+ "agent was installed manually you will need to manually run the uninstall to complete this task.")
	String MESSAGE_CAN_NOT_REMOVE_AGENT = "SQLCM.Message.can-not-remove-agent";

	@I18NMessage("Agent has been upgraded.")
	String MESSAGE_UPGRADE_AGENT_SUCCESS = "SQLCM.Message.upgrade-agent-success";

	@I18NMessage("Can not upgrade agent. {0}")
	String MESSAGE_CAN_NOT_UPGRADE_AGENT = "SQLCM.Message.can-not-upgrade-agent";

	@I18NMessage("A SQL compliance Agent is active on computer {0}.")
	String MESSAGE_CHECK_AGENT_STATUS_SUCCESS = "SQLCM.Message.check-agent-status-success";

	@I18NMessage("The SQL compliance Agent on computer {0} failed to respond. The SQL compliance Agent service may not be running.")
	String MESSAGE_CHECK_AGENT_STATUS_FAILURE = "SQLCM.Message.check-agent-status-failure";

	@I18NMessage("New SQL compliance License has been successfully applied.")
	String MESSAGE_APPLY_LICENSE_SUCCESS = "SQLCM.Message.apply-license-success";

	@I18NMessage("Failed to apply new license key {0}. {1}")
	String MESSAGE_FAILED_TO_APPLY_LICENSE_KEY = "SQLCM.Message.failed-to-apply-license-key";
	
	@I18NMessage("New refresh duration has been successfully applied.") //SQLCM 5.4 SCM-9 start
	String MESSAGE_APPLY_REFRESH_DURATION_SUCCESS = "SQLCM.Message.apply-refresh-duration-success";
	
	@I18NMessage("Data not found.")
	String MESSAGE_GET_COLUMN_SEARCH_DATA="SQLCM.Message.get-column-search-data";
	
	@I18NMessage("Please select database.")
	String MESSAGE_SELECT_DATABASE="SQLCM.Message.select-database";
	
	@I18NMessage("This profile name already exist.")
	String MESSAGE_PROFILE_NAME="SQLCM.Message.profile-name";
	
	@I18NMessage("Failed to apply new refresh duration key {0}. {1}")
	String MESSAGE_FAILED_TO_APPLY_REFRESH_DURATION = "SQLCM.Message.failed-to-apply-refresh-duration"; // SQLCM 5.4 SCM-9 ENd

	@I18NMessage("SQL servers: {0} have been removed.")
	String MESSAGE_REMOVE_INSTANCES_RESULT = "SQLCM.Message.remove-instances-result";

	@I18NMessage("Welcome")
	String WECOME_TAB_WELCOME = "SQLCM.welcome.tab-welcome";

	@I18NMessage("Users")
	String WECOME_TAB_USERS = "SQLCM.welcome.tab-users";

	@I18NMessage("Finish")
	String WECOME_TAB_FINISH = "SQLCM.welcome.tab-finish";

	@I18NMessage("Welcome to SQL Compliance Manager")
	String WECOME_TITLE = "SQLCM.welcome.title";

	@I18NMessage("Designed in partnership with major auditing firms and leading security experts, "
			+ "SQL Compliance Manager provides a powerful auditing and compliance solution for Microsoft SQL Server users. "
			+ "SQL Compliance Manager is a secure, lightweight auditing and reporting solution for Microsoft SQL Server designed to meet the needs "
			+ "of enterprise-scale SQL Server implementations. SQL CM provides unparalleled auditing and reporting services that help you meet the "
			+ "stringent requirements of today’s internal and external security standards.")
	String WECOME_TEXT = "SQLCM.welcome.text";

	@I18NMessage("Add New Users")
	String WECOME_USERS_TITLE = "SQLCM.welcome.users-title";

	@I18NMessage("IDERA SQL Compliance Manager allows you to specify three types of user roles:")
	String WECOME_USERS_TEXT_1 = "SQLCM.welcome.users-text-1";

	@I18NMessage("Administrator, User, and Read Only.")
	String WECOME_USERS_TEXT_2 = "SQLCM.welcome.users-text-2";

	@I18NMessage("Each user role can perform different actions and can access to different sections in the interface.")
	String WECOME_USERS_TEXT_3 = "SQLCM.welcome.users-text-3";

	@I18NMessage("Administrators have complete access and control of the features.")
	String WECOME_USERS_TEXT_4 = "SQLCM.welcome.users-text-4";

	@I18NMessage("Users can also perform all actions but do not have access to the Administration settings.")
	String WECOME_USERS_TEXT_5 = "SQLCM.welcome.users-text-5";

	@I18NMessage("Read Only users have restricted permissions and can only access the information in a read-only mode.")
	String WECOME_USERS_TEXT_6 = "SQLCM.welcome.users-text-6";

	@I18NMessage("Manage Users")
	String WECOME_USERS_LINK = "SQLCM.welcome.users-link";

	@I18NMessage("Other features")
	String WECOME_FINISH_TITLE = "SQLCM.welcome.finish-title";

	@I18NMessage("Monitor, audit and alert on SQL Server user activity and data changes")
	String WECOME_FINISH_SUB_TITLE = "SQLCM.welcome.finish-sub-title";

	@I18NMessage("<ul><li><strong>Audit sensitive data</strong>. See who did what, when, where, and how"
			+ "</li><li><strong>Track and detect</strong>. Monitor and alert on suspicious activity</li>"
			+ "<li><strong>Satisfy audits</strong>. For PCI, HIPAA, FERPA, and SOX requirements</li>"
			+ "<li><strong>Generate reports</strong>. 25 built-in reports to validate SQL Server audit trails</li>"
			+ "<li><strong>Minimize overhead</strong>. Light data collection agent minimizes server impact</li></ul>")
	String WECOME_FINISH_TEXT = "SQLCM.welcome.finish-text";

	@I18NMessage("This account has already been granted access to the IDERA Dashboard.")
	String DUPLICATE_USER = "Messages.duplicate-user";

	@I18NMessage("Added user is not a valid domain user!")
	String NOT_A_VALID_DOMAIN_USER = "Messages.not-a-valid-domain-user";

	@I18NMessage("User Not Saved because an internal exception occurred!")
	String USER_NOT_SAVED = "Messages.user.not.saved";

	@I18NMessage("Failed to load core user.")
	String FAILED_TO_LOAD_CORE_USER = "Messages.failed-to-load-core-user";

	@I18NMessage("Failed to load CM user settings.")
	String FAILED_TO_LOAD_CM_USER_SETTINGS = "Messages.failed-to-load-cm-user-settings";

	@I18NMessage("The last one user cannot be removed because no one will be able to access dashboard.")
	String ERROR_ALL_USERS_REMOVE = "Messages.error-all-users-remove";

	@I18NMessage("Success")
	String SUCCESS = "Messages.success";

	@I18NMessage("Failure")
	String FAILURE = "Messages.failure";

	@I18NMessage("User Name can not be empty")
	String MESSAGE_EMPTY_USER_NAME = "Messages.cm.empty-user-name";

	@I18NMessage("Manage Users and Subscriptions")
	String MESSAGE_USERS_TITLE = "Messages.sql-cm.manage-users-title";

	@I18NMessage("Add users to grant them access to the SQL Compliance Manager user interface.")
	String MESSAGE_USERS_D_TIP_1 = "Messages.sql-cm.manage-users-d-tip-1";

	@I18NMessage("SQL Compliance Manager uses Windows authentication to validate Users so enter the user name in the form  \\\"domain\\\\username\\\".")
	String MESSAGE_USERS_D_TIP_2 = "Messages.sql-cm.manage-users-d-tip-2";

	@I18NMessage("Users log into SQL Compliance Manager using Windows credentials.")
	String MESSAGE_USERS_D_TIP_3 = "Messages.sql-cm.manage-users-d-tip-3";

	// Database Audit Dialog
	@I18NMessage("Audited Database Properties")
	String DB_PROPS_DIALOG_TITLE = "SQLCM.db-props.dialog-title";

	@I18NMessage("General")
	String DB_PROPS_DIALOG_TAB_GENERAL = "SQLCM.db-props.tab-general";

	@I18NMessage("Audited Activities")
	String DB_PROPS_DIALOG_TAB_AUDITED_ACTIVITIES = "SQLCM.db-props.tab-audited-activities";

	@I18NMessage("DML/SELECT Filters")
	String DB_PROPS_DIALOG_TAB_DML_SELECT_FILTERS = "SQLCM.db-props.tab-dml-select-filters";

	@I18NMessage("Before-After Data")
	String DB_PROPS_DIALOG_TAB_BEFORE_AFTER_DATA = "SQLCM.db-props.tab-before-after-data";

	@I18NMessage("Sensitive Columns")
	String DB_PROPS_DIALOG_TAB_SENSITIVE_COLUMNS = "SQLCM.db-props.tab-sensitive-columns";

	@I18NMessage("Trusted Users")
	String DB_PROPS_DIALOG_TAB_TRUSTED_USERS = "SQLCM.db-props.tab-trusted-users";

	@I18NMessage("Privileged User Auditing")
	String DB_PROPS_DIALOG_TAB_PRIVILEGED_USER_AUDITING = "SQLCM.db-props.tab-privileged-user-auditing";

	@I18NMessage("Learn how to optimize performance with audit settings.")
	String DB_PROPS_DIALOG_OPTIMIZE_PERFORMANCE = "SQLCM.db-props.optimize-performance-link";

	@I18NMessage("OK")
	String DB_PROPS_DIALOG_OK_BUTTON = "SQLCM.db-props.ok-button";

	@I18NMessage("Cancel")
	String DB_PROPS_DIALOG_CANCEL_BUTTON = "SQLCM.db-props.cancel-button";

	@I18NMessage("Database parameter not passed.")
	String DATABASE_PARAMETER_NOT_PASSED = "Labels.database-parameter-not-passed";

	@I18NMessage("Server instance:")
	String DB_PROPS_DIALOG_GENERAL_SERVER_INSTANCE = "SQLCM.db-props.general.server-instance";

	@I18NMessage("Database name:")
	String DB_PROPS_DIALOG_GENERAL_DATABASE_NAME = "SQLCM.db-props.general.database-name";

	@I18NMessage("Description:")
	String DB_PROPS_DIALOG_GENERAL_DESCRIPTION = "SQLCM.db-props.general.description";

	@I18NMessage("Auditing status:")
	String DB_PROPS_DIALOG_GENERAL_AUDITING_STATUS = "SQLCM.db-props.general.auditing-status";

	@I18NMessage("Date created:")
	String DB_PROPS_DIALOG_GENERAL_DATE_CREATED = "SQLCM.db-props.general.date-created";

	@I18NMessage("Last modified:")
	String DB_PROPS_DIALOG_GENERAL_LAST_MODIFIED = "SQLCM.db-props.general.last-modified";

	@I18NMessage("Last change in auditing status:")
	String DB_PROPS_DIALOG_GENERAL_LAST_CHANGE = "SQLCM.db-props.general.last-change";

	@I18NMessage("Audited Activities")
	String DB_PROPS_DIALOG_AA_AUDITED_ACTIVITIES = "SQLCM.db-props.aa.audited-activities";

	@I18NMessage("Database Definition (DDL)")
	String DB_PROPS_DIALOG_AA_DATABASE_DEFINITION = "SQLCM.db-props.aa.database-definition";

	@I18NMessage("Security changes")
	String DB_PROPS_DIALOG_AA_SECURITY_CHANGES = "SQLCM.db-props.aa.security-changes";

	@I18NMessage("Administrative Actions")
	String DB_PROPS_DIALOG_AA_ADMINISTRATIVE_ACTIVITIES = "SQLCM.db-props.aa.administrative-activities";

	@I18NMessage("Database Modification (DML)")
	String DB_PROPS_DIALOG_AA_DATABASE_MODIFICATION = "SQLCM.db-props.aa.database-modification";

	@I18NMessage("Database SELECT operations")
	String DB_PROPS_DIALOG_AA_DATABASE_SELECT_OPERATIONS = "SQLCM.db-props.aa.database-select-operations";

	@I18NMessage("Access Check Filter")
	String DB_PROPS_DIALOG_AA_ACCESS_CHECK_FILTER = "SQLCM.db-props.aa.access-check-filter";

	@I18NMessage("Filter events based on access check")
	String DB_PROPS_DIALOG_AA_FILTER_EVENTS = "SQLCM.db-props.aa.filter-events";

	@I18NMessage("Capture SQL statements for DML and SELECT activities")
	String DB_PROPS_DIALOG_AA_CAPTURE_SQL = "SQLCM.db-props.aa.capture-sql";

	@I18NMessage("Capture Transaction Status for DML activity")
	String DB_PROPS_DIALOG_AA_CAPTURE_TRANSACTION = "SQLCM.db-props.aa.capture-transaction";

	@I18NMessage("Audit all database objects")
	String DB_PROPS_DIALOG_DML_AUDIT_ALL_DATABASE_OBJECTS = "SQLCM.db-props.dml.audit-all-database-objects";

	@I18NMessage("Audit the following database objects")
	String DB_PROPS_DIALOG_DML_AUDIT_FOLLOWING_DATABASE_OBJECTS = "SQLCM.db-props.dml.audit-following-database-objects";

	@I18NMessage("All Columns")
	String DB_PROPS_DIALOG_ALL_COLUMNS = "SQLCM.db-props.all-columns";

	@I18NMessage("User Tables")
	String DB_PROPS_DIALOG_DML_USER_TABLES = "SQLCM.db-props.dml.user-tables";

	@I18NMessage("Audit all user tables")
	String DB_PROPS_DIALOG_DML_USER_TABLES_AUDIT_ALL = "SQLCM.db-props.dml.user-tables-audit-all";

	@I18NMessage("Audit the following user tables")
	String DB_PROPS_DIALOG_DML_USER_TABLES_AUDIT_FOLLOWING = "SQLCM.db-props.dml.user-tables-audit-following";

	@I18NMessage("Don't audit user tables")
	String DB_PROPS_DIALOG_DML_USER_TABLES_DONT_AUDIT = "SQLCM.db-props.dml.user-tables-dont-audit";

	@I18NMessage("Other Object Types:")
	String DB_PROPS_DIALOG_DML_OTHER_OBJECT_TYPES = "SQLCM.db-props.dml.other-object-types";

	@I18NMessage("Audit system tables")
	String DB_PROPS_DIALOG_DML_AUDIT_SYSTEM_TABLES = "SQLCM.db-props.dml.audit-system-tables";

	@I18NMessage("Audit stored procedures")
	String DB_PROPS_DIALOG_DML_AUDIT_STORED_PROCEDURES = "SQLCM.db-props.dml.audit-stored-procedures";

	@I18NMessage("Audit all other object types (views, indexes, etc.)")
	String DB_PROPS_DIALOG_DML_AUDIT_ALL_OTHER = "SQLCM.db-props.dml.audit-all-other";

	@I18NMessage("Note: The settings on this page only apply if you have selected to audit DML or SELECT activities for this database on the 'Audited Activities' tab.")
	String DB_PROPS_DIALOG_DML_NOTE = "SQLCM.db-props.dml.note";

	@I18NMessage("Tables audited for DML Before-After data:")
	String DB_PROPS_DIALOG_BEFORE_AFTER_DATA_TABLES_AUDITED = "SQLCM.db-props.before-after-data.tables-audited-before-after";

	@I18NMessage("Note: If you do not select any columns, all columns will be audited by default. Auditing before-after data can result in a significant amount of data being collected. "
			+ "You should audit before-after data for tables only when it is necessary to have the before and after data "
			+ "for DML activity within the table.")
	String DB_PROPS_DIALOG_BEFORE_AFTER_DATA_NOTE = "SQLCM.db-props.before-after-data.note";

	@I18NMessage("CLR Status")
	String DB_PROPS_DIALOG_BEFORE_AFTER_DATA_CLR_STATUS = "SQLCM.db-props.before-after-data.clr-status";

	@I18NMessage("Enable Now")
	String DB_PROPS_DIALOG_BEFORE_AFTER_DATA_ENABLE_CLR = "SQLCM.db-props.before-after-data.enable-clr";

	@I18NMessage("Tables audited for Sensitive Column Access:")
	String DB_PROPS_DIALOG_SENSITIVE_COLUMNS_TABLES_AUDITED = "SQLCM.db-props.before-after-data.tables-audited-sensitive-columns";

	@I18NMessage("Note: If you do not select any columns, all columns will be audited by default. Auditing sensitive columns can result in a significant amount of data being collected. You should consider auditing "
			+ "SELECT commands at the column level only when those columns contain highly sensitive data that should not be widely accessed or read.")
	String DB_PROPS_DIALOG_SENSITIVE_COLUMNS_NOTE = "SQLCM.db-props.sensitive-columns.note";

	@I18NMessage("Sensitive Columns can be an individual column or a group of columns that come together to form sensitive data.")
	String DB_PROPS_DIALOG_SENSITIVE_COLUMNS_NOTE_ADDITIONAL = "SQLCM.db-props.sensitive-columns.note-additional";
	
	@I18NMessage("\"Set Column\" should be used for all individual columns where you would like access reported (IE - \"SSN\").")
	String DB_PROPS_DIALOG_SENSITIVE_COLUMNS_NOTE_COLUMN = "SQLCM.db-props.sensitive-columns.note-column";
	
	@I18NMessage("\"Set Dataset\" should be used when a group of combination of columns come together (IE \"FirstName\" + \"LastName\"). Events will not be tracked unless ALL columns in the dataset are accessed via the same query.")
	String DB_PROPS_DIALOG_SENSITIVE_COLUMNS_NOTE_DATASET = "SQLCM.db-props.sensitive-columns.note-dataset";
	
	@I18NMessage("Trusted users and roles to be filtered:")
	String DB_PROPS_DIALOG_TRUSTED_USERS_USERS_FOR_FILTER = "SQLCM.db-props.trusted-users.users-for-filter";

	@I18NMessage("A trusted user is a SQL Server login or role whose activity you do not need to audit.")
	String DB_PROPS_DIALOG_TRUSTED_USERS_NOTE_1 = "SQLCM.db-props.trusted-users.note-1";

	@I18NMessage("Specify the logins or roles you trust on this database.")
	String DB_PROPS_DIALOG_TRUSTED_USERS_NOTE_2 = "SQLCM.db-props.trusted-users.note-2";

	@I18NMessage("Tell me more...")
	String DB_PROPS_DIALOG_TRUSTED_USERS_TELL_ME_MORE = "SQLCM.db-props.trusted-users.tell-me-more";

	@I18NMessage("Table Name")
	String DB_PROPS_DIALOG_TABLE_NAME = "SQLCM.db-props.before-after-data.table-name";

	@I18NMessage("Maximum Rows")
	String DB_PROPS_DIALOG_MAXIMUM_ROWS = "SQLCM.db-props.before-after-data.maximum-rows";

	@I18NMessage("Columns")
	String DB_PROPS_DIALOG_COLUMNS = "SQLCM.db-props.before-after-data.columns";
	@I18NMessage("Type")
	String DB_PROPS_DIALOG_TYPE = "SQLCM.db-props.before-after-data.type";

	@I18NMessage("Add")
	String DB_PROPS_DIALOG_ADD = "SQLCM.db-props.before-after-data.add";
    @I18NMessage("Add Column")
    String DB_PROPS_DIALOG_ADD_COLUMN = "SQLCM.db-props.before-after-data.add-column";
	@I18NMessage("Add Dataset")
	String DB_PROPS_DIALOG_ADD_DATASET = "SQLCM.db-props.before-after-data.add-dataset";

	@I18NMessage("Remove")
	String DB_PROPS_DIALOG_REMOVE = "SQLCM.db-props.before-after-data.remove";

	@I18NMessage("Edit")
	String DB_PROPS_DIALOG_EDIT = "SQLCM.db-props.before-after-data.edit";

	@I18NMessage("Configure Table Auditing")
	String USER_COLUMNS_DIALOG_TITLE = "SQLCM.user-columns-dialog.title";

	@I18NMessage("Configure Table Auditing for table ")
	String USER_COLUMNS_CONFIGURE_TABLE_AUDITING = "SQLCM.user-columns-dialog.configure-table-auditing";

	@I18NMessage("Select maximum rows and columns to audit")
	String USER_COLUMNS_DIALOG_BAD_TITLE = "SQLCM.user-columns-dialog.bad-title";

	@I18NMessage("Configure Sensitive Columns")
	String USER_COLUMNS_DIALOG_SC_TITLE = "SQLCM.user-columns-dialog.sc-title";

	@I18NMessage("All columns will be audited unless only specific columns are selected.")
	String USER_COLUMNS_DIALOG_SUB_TITLE = "SQLCM.user-columns-dialog.sub-title";

	@I18NMessage("Select how many rows of change data to capture per DML transaction:")
	String USER_COLUMNS_DIALOG_HOW_MANY_ROWS = "SQLCM.user-columns-dialog.how-many-rows";

	@I18NMessage("Audit All Columns")
	String USER_COLUMNS_DIALOG_AUDIT_ALL_COLUMNS = "SQLCM.user-columns-dialog.audit-all-columns";

	@I18NMessage("Audit Selected Columns")
	String USER_COLUMNS_DIALOG_AUDIT_SELECTED_COLUMNS = "SQLCM.user-columns-dialog.audit-selected-columns";

	@I18NMessage("Available Columns:")
	String USER_COLUMNS_DIALOG_AVAILABLE_COLUMNS = "SQLCM.user-columns-dialog.available-columns";

	@I18NMessage("Selected Columns:")
	String USER_COLUMNS_DIALOG_SELECTED_COLUMNS = "SQLCM.user-columns-dialog.selected-columns";

	@I18NMessage("At least one column must be selected when you choose 'Audit Selected Columns'."
			+ "Select at least one column to continue.")
	String USER_COLUMNS_DIALOG_SELECTED_COLUMNS_ERROR = "SQLCM.user-columns-dialog.selected-columns-error";

	@I18NMessage("Atleast one column of each table in a Multi-Table DataSet must be selected.")
	String USER_COLUMNS_DIALOG_SELECTED_COLUMNS_ERROR_DATASET = "SQLCM.user-columns-dialog.selected-columns-error-dataset";

	@I18NMessage("At least one user table must be selected when you choose to specify which user tables to audit."
			+ "Select at least one user table or change the option to \'Don\'t audit user tables\' or \'Audit all user tables\'.")
	String ERROR_ONE_TABLE_MUST_BE_SELECTED = "SQLCM.messages.selected-columns-error";

	@I18NMessage("You must select at least one type of object to be audited.")
	String ERROR_ONE_TYPE_OF_OBJECT_MUST_BE_AUDITED = "SQLCM.messages.error-one-type-of-object-must-be-audited";

	@I18NMessage("You must select at least one type of activity to be audited.")
	String ERROR_ONE_TYPE_OF_ACTIVITY_MUST_BE_AUDITED = "SQLCM.messages.error-one-type-of-activity-must-be-audited";
	
	@I18NMessage("DML must be enabled in order to gather Before-After Data. Please select DML or deselect Before-After Data.")
	String INFORMATION_ON_DML_NOT_SELECTED_AND_BAD_SELECTED_IN_REGULATION="SQLCM.message.information-on-dml-not-select-bad-select";
	
	@I18NMessage("User tables must be audited for Before-After data auditing to be available.")
	String USER_TABLES_MUST_BE_AUDITED = "SQLCM.messages.user-tables-must-be-audited";

	@I18NMessage("Valid")
	String LICENSE_STATE_VALID = "Labels.sql-cm.license-state.valid";

	@I18NMessage("OK")
	String LICENSE_STATE_OK = "Labels.sql-cm.license-state.ok";

	@I18NMessage("Invalid key")
	String LICENSE_STATE_INVALID_KEY = "Labels.sql-cm.license-state.invalid-key";

	@I18NMessage("Invalid product id")
	String LICENSE_STATE_INVALID_PRODUCT_ID = "Labels.sql-cm.license-state.invalid-product-id";

	@I18NMessage("Invalid scope")
	String LICENSE_STATE_INVALID_SCOPE = "Labels.sql-cm.license-state.invalid-scope";

	@I18NMessage("Invalid expired")
	String LICENSE_STATE_INVALID_EXPIRED = "Labels.sql-cm.license-state.invalid-expired";

	@I18NMessage("Invalid mixed types")
	String LICENSE_STATE_INVALID_MIXED_TYPES = "Labels.sql-cm.license-state.invalid-mixed-types";

	@I18NMessage("Invalid duplicate license")
	String LICENSE_STATE_INVALID_DUPLICATE_LICENSE = "Labels.sql-cm.license-state.invalid-duplicate-license";

	@I18NMessage("Invalid product version")
	String LICENSE_STATE_INVALID_PRODUCT_VERSION = "Labels.sql-cm.license-state.invalid-product-version";

	@I18NMessage("Manage Users and Subscriptions: Remove User")
	String USERS_DELETE_CONFIRMATION_TITLE = "Messages.sql-cm.users-delete-confirmation-title";

	@I18NMessage("Import Instances")
	String ADMIN_IMPORT_INSTANCES = "SQLCM.Labels.import-instances";

	@I18NMessage(" (Primary)")
	String PRIMARY_BRACKETS = "SQLCM.Labels.primary-brackets";

	@I18NMessage("Passed")
	String EVENT_ACCESS_CHECK_PASSED = "Labels.sql-cm.event-access-check-passed";

	@I18NMessage("Failed")
	String EVENT_ACCESS_CHECK_FAILED = "Labels.sql-cm.event-access-check-failed";

	@I18NMessage("No")
	String PRIVILEGED_USER_NO = "Labels.sql-cm.privileged-user-no";

	@I18NMessage("Yes")
	String PRIVILEGED_USER_YES = "Labels.sql-cm.privileged-user-yes";
	/* Detailed Event properties dialog. Begin. */
	@I18NMessage("Audit Event Filters")
	String AUDIT_EVENT_FILTERS = "SQLCM.Labels.event-filters";

	/* SQLCM Req4.1.1.6 New Event Filters View - Start */
	@I18NMessage("Event Filters Rules")
	String EVENT_FILTERS_RULES = "SQLCM.Labels.event-filters-rules";

	@I18NMessage("Filter")
	String FILTER = "SQLCM.Labels.filter";

	@I18NMessage("Instance")
	String INSTANCE = "SQLCM.Labels.instance";

	@I18NMessage("New")
	String NEW = "SQLCM.Labels.new";

	@I18NMessage("Import")
	String IMPORT = "SQLCM.Labels.import";

	@I18NMessage("Failed to load audit event filters.")
	String FAILED_TO_LOAD_AUDIT_EVENT_FILTERS = "Labels.failed-to-load-audit-event-filters";

	@I18NMessage("Enable Status")
	String ENABLE_STATUS = "SQLCM.Labels.enable-status";

	@I18NMessage("Disable Status")
	String DISABLE_STATUS = "SQLCM.Labels.disable-status";

	@I18NMessage("Enable")
	String ENABLE = "SQLCM.Labels.enable";

	@I18NMessage("Disable")
	String DISABLE = "SQLCM.Labels.disable";

	@I18NMessage("Enabled")
	String ENABLED = "SQLCM.Labels.enabled";

	@I18NMessage("Disabled")
	String DISABLED = "SQLCM.Labels.disabled";

	@I18NMessage("Edit Filter")
	String EDIT_FILTER = "SQLCM.Labels.edit-filter";

	@I18NMessage("Delete")
	String DELETE = "SQLCM.Labels.delete";

	@I18NMessage("Export")
	String EXPORT = "SQLCM.Labels.export";
	
	@I18NMessage("Export Filter")
	String EXPORT_FILTER = "SQLCM.Labels.export-filter";

	@I18NMessage("Properties")
	String PROPERTIES = "SQLCM.Labels.properties";

	@I18NMessage("New Event Filter")
	String NEW_EVENT_FILTER_TITLE = "SQLCM.Label.new-event-filter-title";

	@I18NMessage("Add")
	String ADD_NEW_FILTER = "SQLCM.Labels.add-new-filter";

	@I18NMessage("Filter these Events")
	String FILTER_THESE_EVENTS = "SQLCM.Label.filter-these-events";

	@I18NMessage("All Events")
	String ALL_EVENTS = "SQLCM.Label.all-events";

	@I18NMessage("User Defined")
	String USER_DEFINED = "SQLCM.Labels.user-defined";

	@I18NMessage("Specify a name for this Filter")
	String SPECIFY_NAME_FOR_FILTER = "SQLCM.Label.specify-name-for-this-filter";

	@I18NMessage("Event Filter Changes")
	String SECURITY_CHANGES_EVENT_FILTER_RULES = "SQLCM.Label.security-changes-event-filter-rules";

	@I18NMessage("4-Severe")
	String SEVERE_EVENT_FILTER_RULES = "SQLCM.Label.severe-event-filter-rules";

	@I18NMessage("3-High")
	String HIGH_EVENT_FILTER_RULES = "SQLCM.Label.high-event-filter-rules";

	@I18NMessage("2-Medium")
	String MEDIUM_EVENT_FILTER_RULES = "SQLCM.Label.medium-event-filter-rules";

	@I18NMessage("1-Low")
	String LOW_EVENT_FILTER_RULES = "SQLCM.Label.low-event-filter-rules";

	@I18NMessage("<h3>Finish Event Filter</h3>"
			+ "<p>"
			+ " This window allows you to specify the general properties for this filter."
			+ "</p>")
	String FINISH_EVENT_FILTER_TIPS = "SQLCM.Html.finish-event-filter.tips";

	@I18NMessage("Show resolution steps after permissions check complete")
	String SHOW_RESOLUTION_STEPS = "SQLCM.Labels.show-resolution-steps";

	@I18NMessage("Selected file is not a valid XML file")
	String NOT_XML_FILE = "SQLCM.Labels.not-XML-file";

	@I18NMessage("Invalid file format")
	String INVALID_FILE_FORMAT = "SQLCM.invalid-file-format";
	
	@I18NMessage("Unable to read event filters for import.")
	String INVALID_FILE_FORMAT_EVENT_FILTER_IMPORT = "SQLCM.invalid-file-format-event-filter-import";
	
	@I18NMessage("Unable to read Alert Rules for import.")
	String INVALID_FILE_FORMAT_ALERT_RULES_IMPORT = "SQLCM.invalid-file-format-alert-rules-import";

	/* SQLCM Req4.1.1.6 New Event Filters View - End */

	@I18NMessage("N/A")
	String N_A = "SQLCM.Labels.n-a";

	@I18NMessage("Event Properties")
	String EVENT_PROPERTIES_DIALOG_TITLE = "Labels.sql-cm.event-properties-dialog-title";

	@I18NMessage("General")
	String EVENT_PROPERTIES_DIALOG_GENERAL_TAB = "Labels.sql-cm.event-properties-dialog-general-tab";

	@I18NMessage("Details")
	String EVENT_PROPERTIES_DIALOG_DETAILS_TAB = "Labels.sql-cm.event-properties-dialog-details-tab";

	@I18NMessage("Event")
	String EVENT_PROPERTIES_DIALOG_EVENT = "Labels.sql-cm.event-properties-dialog-event";

	@I18NMessage("Time")
	String EVENT_PROPERTIES_DIALOG_TIME = "Labels.sql-cm.event-properties-dialog-time";

	@I18NMessage("Category")
	String EVENT_PROPERTIES_DIALOG_CATEGORY = "Labels.sql-cm.event-properties-dialog-category";

	@I18NMessage("Type")
	String EVENT_PROPERTIES_DIALOG_TYPE = "Labels.sql-cm.event-properties-dialog-type";

	@I18NMessage("Application")
	String EVENT_PROPERTIES_DIALOG_APPLICATION = "Labels.sql-cm.event-properties-dialog-application";

	@I18NMessage("Login")
	String EVENT_PROPERTIES_DIALOG_LOGIN = "Labels.sql-cm.event-properties-dialog-login";

	@I18NMessage("Database")
	String EVENT_PROPERTIES_DIALOG_DATABASE = "Labels.sql-cm.event-properties-dialog-database";

	@I18NMessage("Target")
	String EVENT_PROPERTIES_DIALOG_TARGET = "Labels.sql-cm.event-properties-dialog-target";

	@I18NMessage("Event Details")
	String EVENT_PROPERTIES_DIALOG_EVENT_DETAILS = "Labels.sql-cm.event-properties-dialog-event-details";

	@I18NMessage("Before-After Data Summary")
	String EVENT_PROPERTIES_DIALOG_BEFORE_AFTER_DATA_SUMMARY = "Labels.sql-cm.event-properties-dialog-before-after-data-summary";

	@I18NMessage("Rows Affected")
	String EVENT_PROPERTIES_DIALOG_ROWS_AFFECTED = "Labels.sql-cm.event-properties-dialog-rows-affected";

	@I18NMessage("Before-After data auditing is not available for SQL Server 2000")
	String EVENT_PROPERTIES_DIALOG_BEFORE_AFTER_DATA_IS_NOT_AVAILABLE = "Labels.sql-cm.event-properties-dialog-before-after-data-is-not-available";

	@I18NMessage("Not Applicable")
	String EVENT_PROPERTIES_DIALOG_NOT_APPLICABLE = "Labels.sql-cm.event-properties-dialog-not-applicable";

	@I18NMessage("Column Affected")
	String EVENT_PROPERTIES_DIALOG_COLUMN_AFFECTED = "Labels.sql-cm.event-properties-dialog-column-affected";

	@I18NMessage("SQL Statement")
	String EVENT_PROPERTIES_DIALOG_SQL_STATEMENT = "Labels.sql-cm.event-properties-dialog-sql-statement";

	@I18NMessage("Copy")
	String EVENT_PROPERTIES_DIALOG_COPY_BUTTON = "Labels.sql-cm.event-properties-dialog-copy-button";

	@I18NMessage("Close")
	String EVENT_PROPERTIES_DIALOG_CLOSE_BUTTON = "Labels.sql-cm.event-properties-dialog-close-button";

	@I18NMessage("Property")
	String EVENT_PROPERTIES_DIALOG_PROPERTY = "Labels.sql-cm.event-properties-dialog-property";

	@I18NMessage("Value")
	String EVENT_PROPERTIES_DIALOG_VALUE = "Labels.sql-cm.event-properties-dialog-value";

	@I18NMessage("Event ID")
	String EVENT_PROPERTIES_DIALOG_EVENT_ID = "Labels.sql-cm.event-properties-dialog-event-id";

	@I18NMessage("Event time")
	String EVENT_PROPERTIES_DIALOG_EVENT_TIME = "Labels.sql-cm.event-properties-dialog-event-time";

	@I18NMessage("Event type ID")
	String EVENT_PROPERTIES_DIALOG_EVENT_TYPE_ID = "Labels.sql-cm.event-properties-dialog-event-type-id";

	@I18NMessage("Event Type")
	String EVENT_PROPERTIES_DIALOG_EVENT_TYPE = "Labels.sql-cm.event-properties-dialog-event-type";

	@I18NMessage("Event category")
	String EVENT_PROPERTIES_DIALOG_EVENT_CATEGORY = "Labels.sql-cm.event-properties-dialog-event-category";

	@I18NMessage("Event category ID")
	String EVENT_PROPERTIES_DIALOG_EVENT_CATEGORY_ID = "Labels.sql-cm.event-properties-dialog-event-category-id";

	@I18NMessage("Application name")
	String EVENT_PROPERTIES_DIALOG_APPLICATION_NAME = "Labels.sql-cm.event-properties-dialog-application-name";

	@I18NMessage("Target object")
	String EVENT_PROPERTIES_DIALOG_TARGET_OBJECT = "Labels.sql-cm.event-properties-dialog-target-object";

	@I18NMessage("Details")
	String EVENT_PROPERTIES_DIALOG_PROP_DETAILS = "Labels.sql-cm.event-properties-dialog-prop-details";

	@I18NMessage("Event class")
	String EVENT_PROPERTIES_DIALOG_EVENT_CLASS = "Labels.sql-cm.event-properties-dialog-event-class";

	@I18NMessage("Event subclass")
	String EVENT_PROPERTIES_DIALOG_EVENT_SUBCLASS = "Labels.sql-cm.event-properties-dialog-event-subclass";

	@I18NMessage("SPID")
	String EVENT_PROPERTIES_DIALOG_SPID = "Labels.sql-cm.event-properties-dialog-spid";

	@I18NMessage("Host name")
	String EVENT_PROPERTIES_DIALOG_HOST_NAME = "Labels.sql-cm.event-properties-dialog-host-name";

	@I18NMessage("Login name")
	String EVENT_PROPERTIES_DIALOG_LOGIN_NAME = "Labels.sql-cm.event-properties-dialog-login-name";

	@I18NMessage("Database name")
	String EVENT_PROPERTIES_DIALOG_DATABASE_NAME = "Labels.sql-cm.event-properties-dialog-database-name";

	@I18NMessage("Database ID")
	String EVENT_PROPERTIES_DIALOG_DATABASE_ID = "Labels.sql-cm.event-properties-dialog-database-id";

	@I18NMessage("Database user name")
	String EVENT_PROPERTIES_DIALOG_DATABASE_USER_NAME = "Labels.sql-cm.event-properties-dialog-database-user-name";

	@I18NMessage("Object type")
	String EVENT_PROPERTIES_DIALOG_OBJECT_TYPE = "Labels.sql-cm.event-properties-dialog-object-type";

	@I18NMessage("Object name")
	String EVENT_PROPERTIES_DIALOG_OBJECT_NAME = "Labels.sql-cm.event-properties-dialog-object-name";

	@I18NMessage("Object ID")
	String EVENT_PROPERTIES_DIALOG_OBJECT_ID = "Labels.sql-cm.event-properties-dialog-object-id";

	@I18NMessage("Permissions")
	String EVENT_PROPERTIES_DIALOG_PERMISSIONS = "Labels.sql-cm.event-properties-dialog-permissions";

	@I18NMessage("Privileged User Event")
	String EVENT_PROPERTIES_DIALOG_PRIVILEGED_USER_EVENT = "Labels.sql-cm.event-properties-dialog-privileged-user-event";

	@I18NMessage("Column permissions")
	String EVENT_PROPERTIES_DIALOG_COLUMN_PERMISSIONS = "Labels.sql-cm.event-properties-dialog-column-permissions";

	@I18NMessage("Target login name")
	String EVENT_PROPERTIES_DIALOG_TARGET_LOGIN_NAME = "Labels.sql-cm.event-properties-dialog-target-login-name";

	@I18NMessage("Target user name")
	String EVENT_PROPERTIES_DIALOG_TARGET_USER_NAME = "Labels.sql-cm.event-properties-dialog-target-user-name";

	@I18NMessage("Server name")
	String EVENT_PROPERTIES_DIALOG_SERVER_NAME = "Labels.sql-cm.event-properties-dialog-server-name";

	@I18NMessage("Role name")
	String EVENT_PROPERTIES_DIALOG_ROLE_NAME = "Labels.sql-cm.event-properties-dialog-role-name";

	@I18NMessage("Owner name")
	String EVENT_PROPERTIES_DIALOG_OWNER_NAME = "Labels.sql-cm.event-properties-dialog-owner-name";

	@I18NMessage("Checksum")
	String EVENT_PROPERTIES_DIALOG_CHECKSUM = "Labels.sql-cm.event-properties-dialog-checksum";

	@I18NMessage("Hash")
	String EVENT_PROPERTIES_DIALOG_HASH = "Labels.sql-cm.event-properties-dialog-hash";

	@I18NMessage("File name")
	String EVENT_PROPERTIES_DIALOG_FILE_NAME = "Labels.sql-cm.event-properties-dialog-file-name";

	@I18NMessage("Linked server name")
	String EVENT_PROPERTIES_DIALOG_LINKED_SERVER_NAME = "Labels.sql-cm.event-properties-dialog-linked-server-name";

	@I18NMessage("Parent name")
	String EVENT_PROPERTIES_DIALOG_PARENT_NAME = "Labels.sql-cm.event-properties-dialog-parent-name";

	@I18NMessage("System event")
	String EVENT_PROPERTIES_DIALOG_SYSTEM_EVENT = "Labels.sql-cm.event-properties-dialog-system-event";

	@I18NMessage("Session login name")
	String EVENT_PROPERTIES_DIALOG_SESSION_LOGIN_NAME = "Labels.sql-cm.event-properties-dialog-session-login-name";

	@I18NMessage("Provider name")
	String EVENT_PROPERTIES_DIALOG_PROVIDER_NAME = "Labels.sql-cm.event-properties-dialog-provider-name";

	@I18NMessage("Copy")
	String EVENT_PROPERTIES_DIALOG_COPY_TO_BUTTON = "Labels.sql-cm.event-properties-dialog-copy-to-button";

	@I18NMessage("Export as TXT")
	String EXPORT_AS_TXT = "Labels.export-as-txt";

	/* Detailed Event properties dialog. Begin. */
	@I18NMessage("Access check")
	String EVENT_PROPERTIES_DIALOG_ACCESS_CHECK = "Labels.sql-cm.event-properties-dialog-access-check";

	@I18NMessage("Passed")
	String EVENT_PROPERTIES_DIALOG_ACCESS_CHECK_PASSED = "Labels.sql-cm.event-properties-dialog-access-check-passed";

	@I18NMessage("Failed")
	String EVENT_PROPERTIES_DIALOG_ACCESS_CHECK_FAILED = "Labels.sql-cm.event-properties-dialog-access-check-failed";

	@I18NMessage("Instance Events")
	String INSTANCE_EVENTS = "SQLCM.Labels.instance-events";

	@I18NMessage("Database Events")
	String DATABASE_EVENTS = "SQLCM.Labels.database-events";

	@I18NMessage("Database Details")
	String DATABASE_DETAILS = "SQLCM.Labels.database-details";

	@I18NMessage("Remove Database")
	String REMOVE_DATABASE = "SQLCM.Labels.remove-database";

	@I18NMessage("Edit Properties")
	String EDIT_PROPERTIES = "SQLCM.Labels.edit-properties";

	@I18NMessage("Enable/Disable Auditing")
	String ENABLE_DISABLE_AUDITING = "SQLCM.Labels.enable-disable-auditing";

	@I18NMessage("Column")
	String COLUMN_TITLE = "SQLCM.Labels.column-title";

	@I18NMessage("Table")
	String TABLE_TITLE = "SQLCM.Labels.table-title";

	@I18NMessage("Select Column")
	String SELECT_COLUMN_TITLE = "SQLCM.Labels.select-column-title";

	/* Administration page. Begin. */
	@I18NMessage("Users")
	String ADMIN_USERS = "SQLCM.Labels.administration-user-title";

	@I18NMessage("Give users permission to use SQL Compliance Manager. Create, edit and delete users and subscribe to alerts using the Manage Users action.")
	String ADMIN_USER_SUMMARY = "SQLCM.Labels.administration-user-summary";

	@I18NMessage("Manage Users")
	String ADMIN_USER_LINK = "SQLCM.Labels.administration-user-link";

	@I18NMessage("Licensing")
	String ADMIN_LICENSE = "SQLCM.Labels.administration-license-title";

	@I18NMessage("A license is required to access SQL Compliance Manager features. View license status and add a license key using the Manage License action.")
	String ADMIN_LICENSE_SUMMARY = "SQLCM.Labels.administration-license-summary";

	@I18NMessage("Manage License")
	String ADMIN_LICENSE_LINK = "SQLCM.Labels.administration-license-link";

	@I18NMessage("Instances")
	String ADMIN_INSTANCES = "SQLCM.Labels.administration-instances-title";
	
	@I18NMessage("Configuration")
	String ADMIN_REFRESH_CONFIG = "SQLCM.Labels.administration-refresh-title";

	@I18NMessage("SQL Compliance Manager monitors SQL Server instances and their host computers. Add instances to be monitored using the Add SQL Server Instance action.")
	String ADMIN_INSTANCES_SUMMARY = "SQLCM.Labels.administration-instances-summary";
	
	@I18NMessage("Allows configuration of Web Console.") // SQLCM-2172 
	String ADMIN_REFRESH_CONFIG_SUMMARY = "SQLCM.Labels.administration-refresh-summary";

	@I18NMessage("Add SQL Server Instance")
	String ADMIN_ADD_INSTANCE_LINK = "SQLCM.Labels.administration-add-instance-link";

	@I18NMessage("Import SQL Servers")
	String ADMIN_IMPORT_SQL_SERVER_LINK = "SQLCM.Labels.administration-import-sql-servers-link";

	@I18NMessage("Manage SQL Servers Instances")
	String ADMIN_MANAGE_SQL_SERVER_INSTANCES_LINK = "SQLCM.Labels.administration-manage-sql-server-instances-link";
	
	@I18NMessage("Web Console Refresh Rate")
	String ADMIN_MANAGE_CONFIG_REFRESH_LINK = "SQLCM.Labels.administration-manage-sql-server-refresh-link";

	@I18NMessage("LM Utility")
	String ADMIN_MANAGE_SQL_SERVER_LM_UTILITY_LINK = "SQLCM.Labels.administration-manage-sql-server-lmutility-link";
	/* Administration page. End. */

	/* Manage users dialog. Begin. */
	@I18NMessage("Add User")
	String MANAGE_USERS_D_ADD_USER_B = "SQLCM.Labels.manage-users-d.add-user-b";

	@I18NMessage("Edit User")
	String MANAGE_USERS_D_EDIT_USER_B = "SQLCM.Labels.manage-users-d.edit-user-b";

	@I18NMessage("Remove User")
	String MANAGE_USERS_D_REMOVE_USER_B = "SQLCM.Labels.manage-users-d.remove-user-b";

	@I18NMessage("Name")
	String MANAGE_USERS_D_NAME_COLUMN = "SQLCM.Labels.manage-users-d.name-column";

	@I18NMessage("Role")
	String MANAGE_USERS_D_ROLE_COLUMN = "SQLCM.Labels.manage-users-d.role-column";

	@I18NMessage("Email")
	String MANAGE_USERS_D_EMAIL_COLUMN = "SQLCM.Labels.manage-users-d.email-column";

	@I18NMessage("Session timeout")
	String MANAGE_USERS_D_TIMEOUT_COLUMN = "SQLCM.Labels.manage-users-d.timeout-column";

	@I18NMessage("Subscribed to All Critical Alerts")
	String MANAGE_USERS_D_SUBSCRIBE_TO_ALERTS_COLUMN = "SQLCM.Labels.manage-users-d.subscribe-to-alerts-column";
	/* Manage users dialog. End. */

	/* Role enum. Begin. */
	@I18NMessage("Administrator")
	String ROLE_ADMINISTRATOR = "Labels.sql-cm.role-administrator";

	@I18NMessage("Read Only")
	String ROLE_READ_ONLY = "Labels.sql-cm.role-read-only";

	@I18NMessage("User")
	String ROLE_USER = "Labels.sql-cm.role-user";
	/* Role enum. End. */

	/* Add user dialog. Begin. */
	@I18NMessage("Add User:")
	String ADD_USER_D_TITLE = "Labels.sql-cm.add-user-d.title";

	@I18NMessage("User name:")
	String ADD_USER_D_USER_NAME = "Labels.sql-cm.add-user-d.user-name";

	@I18NMessage("Role:")
	String ADD_USER_D_ROLE = "Labels.sql-cm.add-user-d.role";

	@I18NMessage("Email:")
	String ADD_USER_D_EMAIL = "Labels.sql-cm.add-user-d.email";

	@I18NMessage("Session time out:")
	String ADD_USER_D_SESSION_TIME_OUT = "Labels.sql-cm.add-user-d.session-time-out";

	@I18NMessage("Subscribed to All Critical Alerts")
	String ADD_USER_D_SUBSCRIBE_CRITICAL_EVENTS = "Labels.sql-cm.add-user-d.subscribe-critical-events";

	@I18NMessage("Note: Enter user's Windows account ")
	String ADD_USER_D_NOTE_PART_1 = "Labels.sql-cm.add-user-d.note-part-1";

	@I18NMessage("using the form \\\"domain\\\\username\\\"")
	String ADD_USER_D_NOTE_PART_2 = "Labels.sql-cm.add-user-d.note-part-2";

	@I18NMessage("Receive SQL Compliance Manager Alerts email for critical issues such as availability or failure problems")
	String ADD_USER_D_INFO = "Labels.sql-cm.add-user-d.info";

	@I18NMessage("OK")
	String ADD_USER_D_OK_BUTTON = "Labels.sql-cm.add-user-d.ok-button";

	@I18NMessage("Cancel")
	String ADD_USER_D_CANCEL_BUTTON = "Labels.sql-cm.add-user-d.cancel-button";
	/* Add user dialog. End. */

	/* Edit user dialog. Begin. */
	@I18NMessage("Edit User:")
	String EDIT_USER_D_TITLE = "Labels.sql-cm.edit-user-d.title";
	/* Edit user dialog. End. */

	@I18NMessage("Do you want to delete the selected user(s)?")
	String USERS_DELETE_MESSAGE = "Messages.users-delete-message";

	@I18NMessage("Delete user(s)")
	String USERS_DELETE_TITLE = "Messages.users-delete-title";

	/* Error messages. Begin. */
	@I18NMessage("User Name can not be empty")
	String USERNAME_CANNOT_BE_EMPTY = "Labels.user-name-cannot-be-empty";

	@I18NMessage("User name must be in format: domain\\\\username and It has to contain valid caracters")
	String USER_NAME_BAD_FORMAT = "Labels.user-name-bad-format";

	@I18NMessage("User name contains invalid characters")
	String USER_NAME_CONTAINS_NO_PERMIT_CHARACTERS = "Messages.user-contains-not-permit-characters";

	@I18NMessage("User email is required to send alerts")
	String USER_EMAIL_IS_REQUIRED_TO_ALERTS = "Message.user-email-is-required";

	@I18NMessage("User email must be in format: example@domain.com")
	String USER_EMAIL_BAD_FORMAT = "Labels.user-email-bad-format";

	@I18NMessage("Session time out can not be less than {0} minute")
	String SESSION_TIMEOUT_CANNOT_BE_LESS_THAN_DEFAULT = "Labels.session-timeout-cannot-be-less-than-default";
	/* Error messages. End. */

	/* Manage License dialog. Begin. */
	@I18NMessage("Manage License")
	String MANAGE_LICENSE_D_TITLE = "SQLCM.Labels.manage-license-d.title";

	@I18NMessage("Current License")
	String MANAGE_LICENSE_D_SUB_TITLE = "SQLCM.Labels.manage-license-d.sub-title";

	@I18NMessage("Type:")
	String MANAGE_LICENSE_D_TYPE = "SQLCM.Labels.manage-license-d.type";

	@I18NMessage("Status:")
	String MANAGE_LICENSE_D_STATUS = "SQLCM.Labels.manage-license-d.status";

	@I18NMessage("Expiration Date:")
	String MANAGE_LICENSE_D_EXPIRATION_DATE = "SQLCM.Labels.manage-license-d.expiration-date";

	@I18NMessage("Total Licensed Servers:")
	String MANAGE_LICENSE_D_LICENSED_SERVERS = "SQLCM.Labels.manage-license-d.licensed-servers";

	@I18NMessage("Monitored Servers:")
	String MANAGE_LICENSE_D_MONITORED_SERVERS = "SQLCM.Labels.manage-license-d.monitored-servers";

	@I18NMessage("Key:")
	String MANAGE_LICENSE_D_KEY = "SQLCM.Labels.manage-license-d.key";

	@I18NMessage("SQL Compliance Manager Repository:")
	String MANAGE_LICENSE_D_REPOSITORY = "SQLCM.Labels.manage-license-d.repository";

	@I18NMessage("New License")
	String MANAGE_LICENSE_D_NEW_LICENSE = "SQLCM.Labels.manage-license-d.new-license";

	@I18NMessage("Apply")
	String MANAGE_LICENSE_D_APPLY_B = "SQLCM.Labels.manage-license-d.apply-b";
	
	// SQLCM-2172 5.4 version
	@I18NMessage("SAVE")
	String MANAGE_CONFIG_REFRESH_RATE_SAVE = "SQLCM.Labels.manage-config-refresh-rate-d.save";

	@I18NMessage("Click here to buy more SQL Compliance Manager licenses")
	String MANAGE_LICENSE_D_BUY_LICENCE_LINK = "SQLCM.Labels.manage-license-d.buy-license-link";
	
	@I18NMessage("License Manager")
	String LICENSE_MANAGER_LINK = "SQLCM.Labels.license-manager-link";
	/* Manage License dialog. End. */

	@I18NMessage("Add databases")
	String ADD_DATABASE_WIZARD_AUDIT_TITLE = "SQLCM.Label.add-database-wizard.title";

	@I18NMessage("Regulation Guidelines")
	String ADD_REGULATION_GUIDELINE_WIZARD_AUDIT_TITLE = "SQLCM.Label.add-regulation-guideline-wizard.title";

	@I18NMessage("<h3>Select Databases</h3>"
			+ "<p>"
			+ " Select the databases you want to audit. SQL Compliance Manager will collect audit data for the selected databases. "
			+ "</p>")
	String ADD_DATABASE_WIZARD_SELECT_DATABASES_TIPS = "SQLCM.Html.add-database-wizard.select-databases.tips";

	@I18NMessage("Select All")
	String ADD_DATABASE_WIZARD_SELECT_DATABASES_SELECT_ALL = "SQLCM.Label.add-database-wizard.select-databases.select-all-button";

	@I18NMessage("Unselect All")
	String ADD_DATABASE_WIZARD_SELECT_DATABASES_UNSELECT_ALL = "SQLCM.Label.add-database-wizard.select-databases.unselect-all-button";

	@I18NMessage("<h3>Audit Collection Level</h3>"
			+ "<p>"
			+ " Select the audit collection level you want to use for the newly audited database."
			+ " The collection level affects the amount of event data collected for database activities."
			+ "</p>")
	String ADD_DATABASE_WIZARD_AUDIT_COLLECTION_LEVEL_TIPS = "SQLCM.Html.add-database-wizard.audit-collection-level.tips";

	@I18NMessage("Default")
	String ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_LABEL = "SQLCM.Label.add-database-wizard.audit-collection-level.radio.default-label";

	@I18NMessage("\\u0020- Audits events and activities most commonly required by auditors This collection level meets most auditing needs")
	String ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_DESC = "SQLCM.Label.add-database-wizard.audit-collection-level.radio.default-deac";

	@I18NMessage("Tell me more")
	String ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_LINK_LABEL = "SQLCM.Label.add-database-wizard.audit-collection-level.radio.default.link-label";

	@I18NMessage("http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Audit+Collection+Level+window")
	String ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_LINK_URL = "SQLCM.Label.add-database-wizard.audit-collection-level.radio.default.link-url";

	@I18NMessage("Custom")
	String ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_CUSTOM_LABEL = "SQLCM.Label.add-database-wizard.audit-collection-level.radio.custom-label";

	@I18NMessage("\\u0020- Allows you to specify specific audit settings."
			+ " This collection level is recommended for advanced users only."
			+ " Before selecting specific audit settings,"
			+ " review the events gathered by the Custom collection level and review the help to better understand your choices")
	String ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_CUSTOM_DESC = "SQLCM.Label.add-database-wizard.audit-collection-level.radio.custom-desc";

	@I18NMessage("Regulation")
	String ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_REGULATION_LABEL = "SQLCM.Html.add-database-wizard.audit-collection-level.radio.regulation-label";

	@I18NMessage("\\u0020- Configures your audit settings to collect the event data required by specific regulatory guidelines, such as PCI or HIPAA ")
	String ADD_DATABASE_WIZARD_AUDIT_COLLECTION_RADIO_REGULATION_DESC = "SQLCM.Html.add-database-wizard.audit-collection-level.radio.regulation-desc";

	@I18NMessage("<h3>Permissions Check</h3>"
			+ "<p>"
			+ " Required permissions are checked for proper functioning of SQLcm processes on SQL."
			+ " Server instance to be audited."
			+ "</p>"
			+ "<p>"
			+ " Please visit <a target='_blank' style='color: #FE4210' href='http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Permissions+Check+window'>link</a> for additional help. "
			+ "</p>")
	String ADD_DATABASE_WIZARD_PERMISSION_CHECK_TIPS = "SQLCM.Html.add-database-wizard.permissions-check.tips";

	@I18NMessage("Operation Complete Total {0}, Passed {1}, Failed {2} ")
	String ADD_DATABASE_WIZARD_PERMISSION_CHECK_OPERATION_INFO = "SQLCM.Label.add-database-wizard.permissions-check.operation-info";
	
	@I18NMessage("Total Servers: {0}, Passed Servers: {1}, Failed Servers: {2} ")
	String PERMISSION_CHECK_SERVER_OPERATION_INFO = "SQLCM.Label.permissions-check.server.operation-info";

	@I18NMessage("Re-check")
	String ADD_DATABASE_WIZARD_PERMISSION_CHECK_RECHECK = "SQLCM.Label.add-database-wizard.permissions-check.re-check-button";

	@I18NMessage("Passed")
	String ADD_DATABASE_WIZARD_PERMISSION_PASSED = "SQLCM.Label.add-database-wizard.permissions-check.passed";

	@I18NMessage("Failed")
	String ADD_DATABASE_WIZARD_PERMISSION_FAILED = "SQLCM.Label.add-database-wizard.permissions-check.failed";

	@I18NMessage("Unknown")
	String ADD_DATABASE_WIZARD_PERMISSION_UNKNOWN = "SQLCM.Label.add-database-wizard.permissions-check.unknown";

	@I18NMessage("Permissions Check Failed")
	String PERMISSION_FAIL_DIALOG_TITLE = "SQLCM.Label.permission-fail-dialog.title";

	@I18NMessage("{0} required permission checks for proper functioning of SQL Compliance Manager "
			+ "processes on SQL Server instance to be audited failed.")
	String PERMISSION_FAIL_DIALOG_REQ_PERMISSION = "SQLCM.Label.permission-fail-dialog.req-permission";

	@I18NMessage("Please follow below mentioned resolution steps to fix the issues:")
	String PERMISSION_FAIL_DIALOG_PLEASE_FOLLOW = "SQLCM.Label.permission-fail-dialog.please-follow";

	@I18NMessage("You can still ignore the checks and continue or stay on the wizard page and re-check permissions. "
			+ "Please visit <a target='_blank' style='color:#006089' href='http://wiki.idera.com/x/KINi'>this</a> link for additional help.")
	String PERMISSION_FAIL_DIALOG_CAN_STILL_CONTINUE_OR_STAY = "SQLCM.Html.permission-fail-dialog.can-still-continue-or-stay";

	@I18NMessage("Ignore and Continue")
	String PERMISSION_FAIL_DIALOG_IGNORE_BUTTON = "SQLCM.Label.permission-fail-dialog.ignore-button";

	@I18NMessage("Stay and Re-check")
	String PERMISSION_FAIL_DIALOG_STAY_BUTTON = "SQLCM.Label.permission-fail-dialog.stay-button";

	@I18NMessage("<h3>Summary</h3>"
			+ "<p>"
			+ " Review the summary of the audit setting you chose for this SQL Server instance and its hosted databases."
			+ "</p>")
	String ADD_DATABASE_WIZARD_SUMMARY_TIPS = "SQLCM.Html.add-database-wizard.summary.tips";

	@I18NMessage("Audit Level:")
	String ADD_DATABASE_WIZARD_SUMMARY_AUDIT_LEVEL = "SQLCM.Label.add-database-wizard.summary.audit-level";

	@I18NMessage("Server:")
	String ADD_DATABASE_WIZARD_SUMMARY_SERVER = "SQLCM.Label.add-database-wizard.summary.server";

	@I18NMessage("View the Regulation Guideline Details")
	String ADD_DATABASE_WIZARD_SUMMARY_REGULATION_DETAILS = "SQLCM.Label.add-database-wizard.summary.regulation-details";

	@I18NMessage("Databases:")
	String ADD_DATABASE_WIZARD_SUMMARY_DATABASES = "SQLCM.Label.add-database-wizard.summary.databases";

	@I18NMessage("View the Regulation Guideline Details")
	String ADD_DATABASE_WIZARD_SUMMARY_REG_GUIDE_DETAILS = "SQLCM.Label.add-database-wizard.summary.reg-guide-details";

	/* . End. */

	/* Add databases: flow 2 step database audit settings. Begin. */
	@I18NMessage("<h3>Database Audit Settings</h3>"
			+ "<p>"
			+ " Specify the type of audit data you want to collect on the selected databases "
			+ "</p>"
			+ "<p>"
			+ " <a target='_blank' style='color: #FE4210' href='http://wiki.idera.com/display/SQLCM/Reduce+audit+data+to+optimize+performance'>Learn how to optimize performance with audit settings</a> "
			+ "</p>")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_TIPS = "SQLCM.Html.add-database-wizard.audit-settings.tips";

	@I18NMessage("Audited Activity ")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_ACTIVITY = "SQLCM.Label.add-database-wizard.audit-settings.audited-activity";

	@I18NMessage("Security Changes")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_SECURITY_CHANGES = "SQLCM.Label.add-database-wizard.audit-settings.security-changes";

	@I18NMessage("Database Definition (DDL)")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DDL = "SQLCM.Label.add-database-wizard.audit-settings.ddl";

	@I18NMessage("Administrative Actions")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_ADMIN_ACTIVITIES = "SQLCM.Label.add-database-wizard.audit-settings.admin-activities";

	@I18NMessage("Database Modification (DML)")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DML = "SQLCM.Label.add-database-wizard.audit-settings.dml";

	@I18NMessage("Database SELECTs")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_SELECTS = "SQLCM.Label.add-database-wizard.audit-settings.database-selects";

	@I18NMessage("Filter events based on access check:")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_FILTER_EVENTS = "SQLCM.Label.add-database-wizard.audit-settings.filter-events";

	@I18NMessage("Capture SQL statements for DML and SELECT activities")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_CAPTURE_SQL = "SQLCM.Label.add-database-wizard.audit-settings.capture-sql";

	@I18NMessage("Capture Transaction Status for DML Activity")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_CAPTURE_TRANSACTION = "SQLCM.Label.add-database-wizard.audit-settings.capture-transaction";

	@I18NMessage("Passed")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_ONLY_PASSED = "SQLCM.Label.add-database-wizard.audit-settings.only-passed";

	@I18NMessage("Failed")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_ONLY_FAILED = "SQLCM.Label.add-database-wizard.audit-settings.only-failed";

	@I18NMessage("Access Check Filter")
	String ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_ACCESS_CHECK_FILTER = "SQLCM.Label.add-database-wizard.audit-settings.access-check-filter";
	/* Add databases: flow 2 step database audit settings. End. */

	@I18NMessage("<h3>DML and SELECT Audit Filters</h3>"
			+ "<p>"
			+ " Select the database objects you want to audit for DML and SELECT activities. "
			+ "</p>")
	String ADD_DATABASE_WIZARD_AUDIT_FILTERS_TIPS = "SQLCM.Html.add-database-wizard.dml-and-select-audit-filters.tips";

	@I18NMessage("Audit all database objects")
	String ADD_DATABASE_WIZARD_AUDIT_ALL_DB_OBJECTS = "SQLCM.Label.add-database-wizard.dml-and-select-audit-filters.audit-all-db-objects";

	@I18NMessage("Audit the following database objects")
	String ADD_DATABASE_WIZARD_AUDIT_FILTERS_AUDIT_FOLLOWING_DB_OBJECTS = "SQLCM.Label.add-database-wizard.dml-and-select-audit-filters.audit-following-db-objects";

	@I18NMessage("Audit user tables")
	String ADD_DATABASE_WIZARD_AUDIT_FILTERS_AUDIT_USER_TABLES = "SQLCM.Label.add-database-wizard.dml-and-select-audit-filters.audit-user-tables";

	@I18NMessage("Audit system tables")
	String ADD_DATABASE_WIZARD_AUDIT_FILTERS_AUDIT_SYSTEM_TABLES = "SQLCM.Label.add-database-wizard.dml-and-select-audit-filters.audit-system-tables";

	@I18NMessage("Audit stored procedures")
	String ADD_DATABASE_WIZARD_AUDIT_FILTERS_AUDIT_STORED_PROCEDURES = "SQLCM.Label.add-database-wizard.dml-and-select-audit-filters.audit-stored-procedures";

	@I18NMessage("Audit all other object types (views, indexes, etc.)")
	String ADD_DATABASE_WIZARD_AUDIT_FILTERS_AUDIT_ALL_OTHER_OBJECT_TYPES = "SQLCM.Label.add-database-wizard.dml-and-select-audit-filters.audit-all-other-object-types";

	/* Add databases: flow 2 step trusted users. Begin. */
	@I18NMessage("<h3>Trusted Users</h3>"
			+ "<p>"
			+ " Select users whose activities you never want collected, regardless of other audit settings "
			+ "</p>"
			+ "<br /><br />"
			+ "<p>"
			+ " Note: Adding trusted users during server registration can only be done if you have permission to list server roles and logins."
			+ " If the SQL Server is in a non-trusted domain, you will not be able to add trusted users until"
			+ " the SQLcompliance agent is deployed and the server registration is complete."
			+ "</p>")
	String ADD_DATABASE_WIZARD_TRUSTED_USERS_TIPS = "SQLCM.Html.add-database-wizard.trusted-users.tips";

	@I18NMessage("Trusted Users:")
	String ADD_DATABASE_WIZARD_AUDIT_TRUSTED_USERS_TRUSTED_USERS = "SQLCM.Label.add-database-wizard.trusted-users.trusted-users";
	/* Add databases: flow 2 step trusted users. End. */

	/* Add databases: flow 3 step regulation guidelines. Begin. */
	@I18NMessage("<h3>Regulation Guidelines</h3>"
			+ "<p>"
			+ " Select the regulation(s) you want to apply to these audited databases. "
			+ "</p>"
			+ "<br /><br />"
			+ "<p>"
			+ " Each regulation configures your audit settings according to its specific guidelines."
			+ " You can apply one or more regulations, depending on the event data you need to collect."
			+ "</p>")
	String ADD_DATABASE_WIZARD_REG_GUIDE_TIPS = "SQLCM.Html.add-database-wizard.reg-guide.tips";

	@I18NMessage("Guidelines")
	String ADD_DATABASE_WIZARD_REG_GUIDE_GUIDELINES = "SQLCM.Label.add-database-wizard.reg-guide.guidelines";

	@I18NMessage("PCI DSS")
	String ADD_DATABASE_WIZARD_REG_GUIDE_PCI_DSS = "SQLCM.Label.add-database-wizard.reg-guide.pci-dss";

	@I18NMessage("HIPPA")
	String ADD_DATABASE_WIZARD_REG_GUIDE_HIPPA = "SQLCM.Label.add-database-wizard.reg-guide.hippa";

	@I18NMessage("The current audit settings for the selected database(s) will be overridden when the regulation is applied.")
	String ADD_DATABASE_WIZARD_REG_GUIDE_CURRENT_AUDIT_SETT = "SQLCM.Label.add-database-wizard.reg-guide.current-audit-sett";

	/* Add databases: flow 3 step regulation guidelines. End. */

	/* Add databases: flow 3 step regulation guidelines APPLY. Begin. */
	@I18NMessage("<h3>Regulation Guidelines</h3>" + "<p>"
			+ " Apply selected regulation guidelines " + "</p>")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_TIPS = "SQLCM.Html.add-database-wizard.reg-guide-apply.tips";

	@I18NMessage("You selected {0} regulatory guidelines as your audit level.")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_SELECTED_GUIDELINES = "SQLCM.Label.add-database-wizard.reg-guide-apply.selected-guidelines";

	@I18NMessage("PCI DSS")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_PCI_DSS_GUIDELINE = "SQLCM.Label.add-database-wizard.reg-guide-apply.pci-dss-guidelines";

	@I18NMessage("HIPAA")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_HIPAA_GUIDELINE = "SQLCM.Label.add-database-wizard.reg-guide-apply.hipaa-guidelines";

	@I18NMessage("\\u0020and\\u0020")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_SELECTED_GUIDELINES_AND = "SQLCM.Label.add-database-wizard.reg-guide-apply.guidelines-and";

	@I18NMessage("The SQLcompliance Agent will automatically collect the following server and database events:")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_DB_EVENTS = "SQLCM.Label.add-database-wizard.reg-guide-apply.db-events";

	@I18NMessage("Failed Logins")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_FAILED_LOGINS = "SQLCM.Label.add-database-wizard.reg-guide-apply.failed-logins";

	@I18NMessage("Administrative Actions")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_ADMIN_ACTIVITIES = "SQLCM.Label.add-database-wizard.reg-guide-apply.admin-activities";

	@I18NMessage("Security Changes")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_SECURITY_CHANGES = "SQLCM.Label.add-database-wizard.reg-guide-apply.security-changes";

	@I18NMessage("Database Definition")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_DB_DEFINITION = "SQLCM.Label.add-database-wizard.reg-guide-apply.db-definition";

	@I18NMessage("Database Modification")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_DB_MODIFICATION = "SQLCM.Label.add-database-wizard.reg-guide-apply.db-modification";

	@I18NMessage("Privileged users")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_PRIV_USERS = "SQLCM.Label.add-database-wizard.reg-guide-apply.priv-users";

	@I18NMessage("Privileged user Events")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_PRIV_USER_EVENTS = "SQLCM.Label.add-database-wizard.reg-guide-apply.priv-user-events";

	@I18NMessage("Sensitive Column Auditing")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_SENS_COLUMN_AUDITING = "SQLCM.Label.add-database-wizard.reg-guide-apply.sens-column-auditing";

	@I18NMessage("What you need to configure your audit settings.")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_AUDIT_SETTINGS = "SQLCM.Label.add-database-wizard.reg-guide-apply.audit-settings";
	@I18NMessage("Permission to list Server Roles and Logins")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_PERMISSION_SERVER_ROLES_LOGINS = "SQLCM.Label.add-database-wizard.reg-guide-apply.permission-server-roles-logins";

	
	@I18NMessage("Sensitive Column Auditing")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_SENS_COL_AUDITING = "SQLCM.Label.add-database-wizard.reg-guide-apply.sens-col-auditing";
	
	@I18NMessage("Before-After Data Auditing")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_BEFORE_AFTER_DATA_AUDITING = "SQLCM.Label.add-database-wizard.reg-guide-apply.before-after-data-auditing";
	
	@I18NMessage("Passed")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_PASSED_ACCESS_CHECK = "SQLCM.Label.add-database-wizard.reg-guide-apply.passed-access-check";
	
	@I18NMessage("Failed")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_FAILED_ACCESS_CHECK = "SQLCM.Label.add-database-wizard.reg-guide-apply.failed-access-check";
	
	@I18NMessage("Capture SQL statements for DDL and Security Changes")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_SQL_STATEMENTS_DDL_SECURITY_CHANGES = "SQLCM.Label.add-database-wizard.reg-guide-apply.sql-statements-ddl-security-changes";
	
	@I18NMessage("Privileged User And User Events")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_PRIVILEGED_USER_USER_EVENTS = "SQLCM.Label.add-database-wizard.reg-guide-apply.privileged-user-user-events";
	/* Add databases: flow 3 step regulation guidelines APPLY. End. */

	@I18NMessage("NOTE:")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_NOTE = "SQLCM.Label.add-database-wizard.reg-guide-apply.note";
	
	@I18NMessage("Custom")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_CUSTOM = "SQLCM.Label.add-database-wizard.reg-guide-apply.custom";
	
	@I18NMessage("New Custom template")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_NEW_CUSTOM_TEMPLATE = "SQLCM.Label.add-database-wizard.reg-guide-apply.new_custom_template";
	
	@I18NMessage("You will need to deploy the agent to add privileged users.")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_DEPLOY_AGENT_ADD_PRIV_USERS = "SQLCM.Label.add-database-wizard.reg-guide-apply.deploy-agent-add-priv-users";
	
	@I18NMessage("Need Help? <a target='_blank' style='color: #0000EE' href='http://wiki.idera.com/x/xwI1'>Click here</a> on how to configure Compliance Manager audit settings.")
	String ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_NEED_HELP = "SQLCM.Label.add-database-wizard.reg-guide-apply.need-help";
	/* Add databases: flow 3 step Privileged Users. Begin. */
	
	@I18NMessage("<h3>Privileged Users</h3>"
			+ "<p>"
			+ " Select users whose activities you never want collected, regardless of other audit settings "
			+ "</p>"
			+ "<br /><br />"
			+ "<p>"
			+ " Note: Adding privileged users can only be done if you have permission to list server roles and logins."
			+ " If the SQL Server is in a non-trusted domain, you will not be able to add privileged users until"
			+ " the SQLcompliance agent is deployed and the server registration is complete."
			+ "</p>")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_TIPS = "SQLCM.Html.add-database-wizard.privileged-users.tips";

	@I18NMessage("Audited Privileged Users:")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED = "SQLCM.Label.add-database-wizard.privileged-users.audited";
	/* Add databases: flow 3 step Privileged Users. End. */

	/* Add databases: flow 3 step Privileged User Audited Activity. Begin. */
	@I18NMessage("<h3>Privileged User Audited Activity</h3>" + "<p>"
			+ " Select which activities to audit for privileged users "
			+ "</p>")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_TIPS = "SQLCM.Html.add-database-wizard.privileged-users-audited-activity.tips";

	@I18NMessage("Audit all activities done by privileged users")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_AUDIT_ALL = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.audit-all";

	@I18NMessage("Audit selected activities done by privileged users ")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_AUDIT_SELECTED = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.audit-selected";

	@I18NMessage("Logins")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_LOGINS = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.logins";

	@I18NMessage("Failed logins")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_FAILED_LOGINS = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.failed-logins";

	@I18NMessage("Security changes ")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_SECURITY_CHANGES = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.security-changes";

	@I18NMessage("Administrative actions")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_ADMIN_ACTIONS = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.admin-actions";

	@I18NMessage("Database Definition (DDL)")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_DDL = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.ddl";

	@I18NMessage("Database Modification (DML)")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_DML = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.dml";

	@I18NMessage("Database SELECT s")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_DB_SELECTS = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.db-selects";

	@I18NMessage("User Defined Events")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_DEFINED_EVENTS = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.defined-events";

	@I18NMessage("Filter events based on access check:")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_FILTER_EVENTS = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.filter-events";

	@I18NMessage("Passed")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_PASSED = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.passed";

	@I18NMessage("Failed")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_FAILED = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.failed";

	@I18NMessage("Capture SQL statements for DML and SELECT activities")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_CAPTURE_SQL = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.capture-sql";

	@I18NMessage("Capture Transaction Status for DML Activity")
	String ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_CAPTURE_TRANSACTION = "SQLCM.Label.add-database-wizard.privileged-users.audited-activity.capture-transaction";

	@I18NMessage("<h3>Sensitive Columns</h3>"
			+ "<p>"
			+ " Select the tables you want to audit for sensitive column access. "
			+ "</p>"
			+ "<p>"
			+ " For each database, enter the tables that you would like audited for Sensitive Column Access."
			+ " For each table, all columns will be audited. " + "</p>")
	String ADD_DATABASE_WIZARD_SENSITIVE_COLUMNS_TIPS = "SQLCM.Html.add-database-wizard.sensitive-columns.tips";

	@I18NMessage("Tables")
	String ADD_DATABASE_WIZARD_SENSITIVE_COLUMNS_TABLES = "SQLCM.Label.add-database-wizard.sensitive-columns.tables";
	/* Add databases: flow 3 step Privileged User Audited Activity. End. */

	@I18NMessage("Add")
	String ADD_DATABASE_WIZARD_BUTTON_ADD = "SQLCM.Label.add-database-wizard.button.add";

	@I18NMessage("Remove")
	String ADD_DATABASE_WIZARD_BUTTON_REMOVE = "SQLCM.Label.add-database-wizard.button.remove";

	@I18NMessage("You are already auditing all available user and system databases from the selected SQL Server.")
	String ADD_DATABASE_WIZARD_ALL_ALREADY_AUDITED = "SQLCM.Label.add-database-wizard.all-already-audited";

	@I18NMessage("Select tables to audit")
	String SENSITIVE_COLUMNS_DIALOG_TIPS = "SQLCM.Html.sql-cm.select-tables-to-audit-dialog.tips";

	@I18NMessage("Available Tables:")
	String SENSITIVE_COLUMNS_DIALOG_AVAILABLE_TABLES = "SQLCM.Label.select-tables-to-audit-dialog.available-tables";

	@I18NMessage("Records Per Page")
	String SENSITIVE_COLUMNS_DIALOG_RECORDS_PER_PAGE = "SQLCM.Label.select-tables-to-audit-dialog.records-per-page";

	@I18NMessage("Selected Tables:")
	String SENSITIVE_COLUMNS_DIALOG_SELECTED_TABLES = "SQLCM.Label.select-tables-to-audit-dialog.selected-tables";

	@I18NMessage("OK")
	String SENSITIVE_COLUMNS_DIALOG_OK_BUTTON = "Labels.sql-cm.select-tables-to-audit-dialog.ok-button";

	@I18NMessage("Cancel")
	String SENSITIVE_COLUMNS_DIALOG_CANCEL_BUTTON = "Labels.sql-cm.select-tables-to-audit-dialog.cancel-button";

	@I18NMessage("Add ->")
	String ADD_DATABASE_WIZARD_BUTTON_ADD_ARROW = "SQLCM.Label.add-database-wizard.button.add-arrow";

	@I18NMessage("<- Remove")
	String ADD_DATABASE_WIZARD_BUTTON_REMOVE_ARROW = "SQLCM.Label.add-database-wizard.button.remove-arrow";

	@I18NMessage("Warning: Selecting to capture operation low level details can increase the amount of information gathered for this database significantly. It is recommended that this option be left. off unless absolutely needed.")
	String ADD_DATABASE_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE = "SQLCM.Label.add-database-wizard.low-level-details-can-increase";

	//SQLCm 5.4_4.1.1_Extended Events Start
	 @I18NMessage("Extended events functionality is supported for SQL Server 2012 and above.")
	 String EXTENDED_EVENTS_ERROR_MESSAGE = "Labels.sql-cm.inst-prop-d-privileged-user-aud-tab-capture-sql-dml-select-extended";
	 
	@I18NMessage("Extended events could not be switched on because the agent is not reachable. Please check the agent status and try again.")
	String EXTENDED_EVENTS_AGENT_ERROR_MESSAGE = "Labels.sql-cm.agent-error-message";
	 
	 @I18NMessage("Extended events functionality is supported for 5.4 and later version of SQL Compliance Manager Agent.")
	 String EXTENDED_EVENTS_WARNING_MESSAGE = "Labels.sqlcm.capture-sql-dml-select-extended";
	//SQLCm 5.4_4.1.1_Extended Events End
	
	@I18NMessage("Failed to load database list.")
	public static final String FAILED_TO_LOAD_DATABASE_LIST = "Labels.failed-to-load-database-list";

	@I18NMessage("Failed to load permissions list.")
	public static final String FAILED_TO_LOAD_PERMISSIONS_LIST = "Labels.failed-to-load-permissions-list";

	@I18NMessage("Failed to load instance.")
	public static final String FAILED_TO_LOAD_INSTANCE = "Labels.failed-to-load-instance";

	@I18NMessage("Failed to load instance Users And Roles.")
	public static final String FAILED_TO_LOAD_INSTANCE_USERS_AND_ROLES = "Labels.failed-to-load-instance-users-and-roles";

	@I18NMessage("Failed to load regulation types.")
	public static final String FAILED_TO_LOAD_REGULATION_TYPES = "Labels.failed-to-load-regulation-types";

	@I18NMessage("Failed to load instance detail")
	String FAILED_TO_LOAD_INSTANCE_DETAIL = "Labels.failed-to-load-instance-detail";

	@I18NMessage("Failed to load audited database list")
	String FAILED_TO_LOAD_AUDITED_DATABASE_LIST = "Labels.failed-to-load-audited-database-list";

	@I18NMessage("Failed to load filtered alerts.")
	String FAILED_TO_LOAD_FILTERED_ALERTS = "Labels.failed-to-load-filtered-alerts";

	@I18NMessage("Failed to retrieve saved view state.")
	String FAILED_TO_RETRIEVE_SAVED_VIEW_STATE = "Labels.failed-to-retrieve-saved-view-state";

	// Edit Instance Details Properties dialog. Begin.
	@I18NMessage("Edit Instance Properties")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_TITLE = "SQLCM.edit-instance-details-properties.title";

	@I18NMessage("Instance: ")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_INSTANCE_LABEL = "SQLCM.edit-instance-details-properties.instance";

	@I18NMessage("Instance Details and Ownership")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_TITLE_2 = "SQLCM.edit-instance-details-properties.title-2";

	@I18NMessage("Owner")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_OWNER = "SQLCM.edit-instance-details-properties.owner";

	@I18NMessage("Location")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_LOCATION = "SQLCM.edit-instance-details-properties.location";

	@I18NMessage("Comments")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_COMMENTS = "SQLCM.edit-instance-details-properties.comments";

	@I18NMessage("Data Collection Settings")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_TITLE_3 = "SQLCM.edit-instance-details-properties.title-3";

	@I18NMessage("Collection Interval")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_INTERVAL = "SQLCM.edit-instance-details-properties.interval";

	@I18NMessage("Keep Data for")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_KEEP_DATA_FOR = "SQLCM.edit-instance-details-properties.keep-data-for";

	@I18NMessage("Credentials")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_CREDENTIALS = "SQLCM.edit-instance-details-properties.credentials";

	@I18NMessage("SQL Connection Credentials")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_SQL_CREDENTIALS = "SQLCM.edit-instance-details-properties.sql-credentials";

	@I18NMessage("Specify the account to be used to gather instance information.")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_NOTE = "SQLCM.edit-instance-details-properties.note";

	@I18NMessage("Test the credentials to make sure SQL Compliance Manager can gather data for instances")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_PRO_TIP = "SQLCM.edit-instance-details-properties.pro-tip";

	@I18NMessage("Test Credentials")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_TEST_CREDENTIALS = "SQLCM.edit-instance-details-properties.test-credentials";

	@I18NMessage("Login")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_LOGIN = "SQLCM.edit-instance-details-properties.login";

	@I18NMessage("User name")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_USER_NAME = "SQLCM.edit-instance-details-properties.user-name";

	@I18NMessage("Password")
	String EDIT_INSTANCE_DETAILS_PROPERTIES_D_PASSWORD = "SQLCM.edit-instance-details-properties.password";
	// Edit Instance Details Properties dialog. End.

	// TimeMeasurement enum. Begin.
	@I18NMessage("Minute(s)")
	String TIME_MEASUREMENT_MINUTE = "SQLCM.time-measurement.minute";

	@I18NMessage("Day(s)")
	String TIME_MEASUREMENT_DAY = "SQLCM.time-measurement.day";
	// TimeMeasurement enum. End.

	@I18NMessage("Test Credentials")
	String TEST_CREDENTIALS_D_TITLE = "SQLCM.test-credentials.title";

	@I18NMessage("Edit Credentials")
	String EDIT_CREDENTIALS_D_TITLE = "SQLCM.edit-credentials.title";

	// IntegrityCheckStatus enum. Begin.
	@I18NMessage("None")
	String INTEGRITY_CHECK_STATUS_NONE = "SQLCM.integrity-check-status.none";

	@I18NMessage("Passed")
	String INTEGRITY_CHECK_STATUS_PASSED = "SQLCM.integrity-check-status.passed";

	@I18NMessage("In progress")
	String INTEGRITY_CHECK_STATUS_IN_PROGRESS = "SQLCM.integrity-check-status.inProgress";

	@I18NMessage("Failed")
	String INTEGRITY_CHECK_STATUS_FAILED = "SQLCM.integrity-check-status.failed";

	@I18NMessage("Failed and repaired")
	String INTEGRITY_CHECK_STATUS_FAILED_REPAIRED = "SQLCM.integrity-check-status.failed-repaired";

	@I18NMessage("Incomplete")
	String INTEGRITY_CHECK_STATUS_INCOMPLETE = "SQLCM.integrity-check-status.incomplete";
	// IntegrityCheckStatus enum. End.

	@I18NMessage("Enable")
	String ENABLE_LABEL = "SQLCM.enable-label";

	@I18NMessage("Disable")
	String DISABLE_LABEL = "SQLCM.disable-label";

	// ArchiveCheckStatus enum. Begin.
	@I18NMessage("None")
	String ARCHIVE_CHECK_STATUS_NONE = "SQLCM.archive-check-status.none";

	@I18NMessage("Completed")
	String ARCHIVE_CHECK_STATUS_COMPLETED = "SQLCM.archive-check-status.completed";

	@I18NMessage("In progress")
	String ARCHIVE_CHECK_STATUS_IN_PROGRESS = "SQLCM.archive-check-status.inProgress";

	@I18NMessage("Failed integrity")
	String ARCHIVE_CHECK_STATUS_FAILED_INTEGRITY = "SQLCM.archive-check-status.failed-integrity";

	@I18NMessage("Failed with errors")
	String ARCHIVE_CHECK_STATUS_FAILED_WITH_ERRORS = "SQLCM.archive-check-status.failed-with-errors";

	@I18NMessage("Incomplete")
	String ARCHIVE_CHECK_STATUS_INCOMPLETE = "SQLCM.archive-check-status.incomplete";
	// ArchiveCheckStatus enum. End.

	@I18NMessage("Current")
	String STATUS_CURRENT = "SQLCM.status.current";

	@I18NMessage("Update pending")
	String STATUS_UPDATE_PENDING = "SQLCM.status.update-pending";

	@I18NMessage("Prev")
	String WIZARD_BUTTON_PREV = "SQLCM.Label.wizard.button.prev";

	@I18NMessage("Next")
	String WIZARD_BUTTON_NEXT = "SQLCM.Label.wizard.button.next";

	@I18NMessage("Finish")
	String WIZARD_BUTTON_FINISH = "SQLCM.Label.wizard.button.finish";

	@I18NMessage("Cancel")
	String WIZARD_BUTTON_CANCEL = "SQLCM.Label.wizard.button.cancel";

	// StatsCategory Enum. Begin.

	@I18NMessage("Event Alerts")
	String STATS_CATEGORY_ALERTS = "SQLCM.stats-category.alerts";

	@I18NMessage("Privileged User")
	String STATS_CATEGORY_PRIVILEGED_USER_EVENTS = "SQLCM.stats-category.privileged-user-events";

	@I18NMessage("Failed Logins")
	String STATS_CATEGORY_FAILED_LOGIN = "SQLCM.stats-category.failed-logins";

	@I18NMessage("DDL")
	String STATS_CATEGORY_DDL = "SQLCM.stats-category.ddl";

	@I18NMessage("Security")
	String STATS_CATEGORY_SECURITY = "SQLCM.stats-category.security";

	@I18NMessage("Overall Activity")
	String STATS_CATEGORY_EVENTS_PROCESSED = "SQLCM.stats-category.events-processed";
	// StatsCategory Enum. End.

	@I18NMessage("Refresh")
	String ACTIONS_ADD_SERVER_LOGSVIEW = "Labels.sql-cm.actions.logsView";

	@I18NMessage("Activity Log")
	String ACTIVITY_LOG = "SQLCM.Labels.logsView.Activitylog";

	@I18NMessage("Change Log")
	String CHANGE_LOG = "SQLCM.Labels.logsView.Changelog";

	@I18NMessage("Instance")
	String LOGVIEW_INSTANCE = "SQLCM.Labels.instance";

	@I18NMessage("Email")
	String ALERT_RULE_EMAIL = "SQLCM.Labels.alertsRule.email-column";

	@I18NMessage("Event Log")
	String EVENT_LOG = "SQLCM.alertsRule.event_log";

	@I18NMessage("Print")
	String PRINT = "SQLCM.Labels.print";

	@I18NMessage("Print Preview")
	String PRINT_PREVIEW = "SQLCM.Labels.print-preview";

	@I18NMessage("Snmp Trap")
	String SNMP_TRAP = "SQLCM.alertsRule.snmp_trap";

	@I18NMessage("Data")
	String DATA = "SQLCM.Labels.data";

	@I18NMessage("Edit Alert Rules")
	String EDIT_ALERT_RULES = "SQLCM.Labels.edit-alert-rules";

	// New Alert Rules Start

	@I18NMessage("New Event Alert Rule")
	String NEW_EVENT_ALERT_RULES = "SQLCM.Label.new-event-alert-rules";

	@I18NMessage("Specify a name for this Rule")
	String SPECIFY_EVENT_ALERT_RULES = "SQLCM.Label.specify-event-alert-rules";

	@I18NMessage("Specify SQL Servers")
	String SPECIFY_SQL_SERVER = "SQLCM.Label.specify-sql-server";

	@I18NMessage("Specify Databases")
	String SPECIFIED_DATABASE = "SQLCM.Label.specified-database";
	
	@I18NMessage("Specify Table")
	String SPECIFIED_TABLE = "SQLCM.Label.specified-table";
	
	@I18NMessage("Specify Column")
	String SPECIFIED_COLUMN = "SQLCM.Label.specified-column";

	@I18NMessage("Specify Objects")
	String SPECIFIED_OBJECTS = "SQLCM.Label.specified-objects";

	@I18NMessage("Specify Words")
	String SPECIFIED_WORDS = "SQLCM.Label.specified-words";

	@I18NMessage("Select Event Types")
	String SELECT_EVENT_TYPES = "SQLCM.Label.select-event-types";

	@I18NMessage("True")
	String TRUE = "SQLCM.Labels.true";

	@I18NMessage("False")
	String FALSE = "SQLCM.Labels.false";

	@I18NMessage("Specify alert level")
	String SPECIFY_ALERT_RULES_LEVEL = "SQLCM.Label.specify-alert-rules-level";

	@I18NMessage("Description")
	String DESCRIPTION_EVENT_ALERT_RULES = "SQLCM.Label.description-event-alert-rules";

	@I18NMessage("Select the Event Type")
	String EVENT_TYPE_ALERT_RULES = "SQLCM.Label.select_event_type";

	@I18NMessage("Security Changes")
	String SECURITY_CHANGES_ALERT_RULES = "SQLCM.Label.security-changes-add-alert-rules";

	@I18NMessage("Data Definition(DDL)")
	String DATA_DEFINITION_ALERT_RULES = "SQLCM.Label.data-definition-add-alert-rules";

	@I18NMessage("Specific Events")
	String SPECIFIC_EVENT_ALERT_RULES = "SQLCM.Label.specific-events-add-alert-rules";

	@I18NMessage("Administrative Activity")
	String ADMINISTRATIVE_ACTIVITY_ALERT_RULES = "SQLCM.Label.administrative-activity-add-alert-rules";

	@I18NMessage("Data Manipulation(DML)")
	String DATA_MANIPULATION_ALERT_RULES = "SQLCM.Label.data-manipulation-add-alert-rules";

	@I18NMessage("Login Activity")
	String LOGIN_ACTIVITY_ALERT_RULES = "SQLCM.Label.login-activity-add-alert-rules";

	@I18NMessage("User Defined Events")
	String USER_DEFINED_ACTIVITY_ALERT_RULES = "SQLCM.Label.user-defined-events-add-alert-rules";

	@I18NMessage("Select the SQL Server Objects")
	String SQL_SERVER_OBJECTS_ALERT_RULES = "SQLCM.Label.sql-server-objects-alert-rules";

	@I18NMessage("SQL Server")
	String SQL_SERVER_ALERT_RULES = "SQLCM.Label.sql-server-alert-rules";

	@I18NMessage("Database Name")
	String DATABASE_NAME_ALERT_RULES = "SQLCM.Label.database-name-alert-rules";

	@I18NMessage("Object Name (including Table)")
	String OBJECT_NAME_ALERT_RULES = "SQLCM.Label.object-name-alert-rules";

	@I18NMessage("Host Name")
	String HOST_NAME_ALERT_RULES = "SQLCM.Label.host-name-alert-rules";

	@I18NMessage("Host")
	String HOST_NAME = "SQLCM.Label.host-name";
	
	@I18NMessage("Application")
	String APPLICATION_NAME = "SQLCM.Label.application-name";
	
	@I18NMessage("Select additional event filters")
	String ADD_EVENT_FILTER_ALERT_RULES = "SQLCM.Label.add-event-filter-alert-rules";
	
	@I18NMessage("Select additional data filters")
	String ADD_DATA_FILTER_ALERT_RULES = "SQLCM.Label.add-data-filter-alert-rules";

	@I18NMessage("Row Count (with Time Interval)")
	String ROW_COUNT_WITH_TIME_INTERVAL = "SQLCM.Label.row-count-with-time-interval";

	@I18NMessage("Application Name")
	String APPLICATION_NAME_ALERT_RULES = "SQLCM.Label.application-name-alert-rules";

	@I18NMessage("Login Name")
	String LOGIN_NAME_ALERT_RULES = "SQLCM.Label.login-name-alert-rules";

	@I18NMessage("Specify Row Count Threshold")
	String SPECIFY_ROW_COUNT_THREHOLD = "SQLCM.Label.specify-row-count-threshold";
	
	@I18NMessage("Row Count")
	String SPECIFY_ROW_COUNT = "SQLCM.Label.specify-row-count";
	
	
	@I18NMessage("Specify time frame (optional)- You can enter the time frame you want to apply to the threshold for the alert. For example, if you enter row count >=100 records over 2 hours, then the alert will be triggered if the row count reaches 100 records or more in 2 hour(s)")
	String SPECIFY_TIME_FRAME = "SQLCM.Label.specify-time-frame";
	
	@I18NMessage("Time frame (hrs)")
	String TIME_FRAME_HOURS = "SQLCM.Label.time-frame-hours";
	
	
	@I18NMessage("Exclude Certain Event Type")
	String EX_CERTAIN_EVENT_TYPE_ALERT_RULES = "SQLCM.Label.ex-certain-event-type-alert-rules";
	
	@I18NMessage("Are you sure you want to delete this rule?")                       
	String CONFIRM_DELETE_RULE = "SQLCM.Label.confirm-delete-rule";    
	
	@I18NMessage("Are you sure you want to delete this filter?")                       
	String CONFIRM_DELETE_FILTER = "SQLCM.Label.confirm-delete-filter"; 
	
	@I18NMessage("Are you sure you want to delete this log?")                       
	String CONFIRM_DELETE_LOG = "SQLCM.Label.confirm-delete-log"; 
	
	@I18NMessage("Question")                       
	String QUESTION = "SQLCM.Label.question"; 
	
	@I18NMessage("Do not ask this again in future")                       
	String FUTURE_CONFIRM = "SQLCM.Label.future-confirm";                     
	
	@I18NMessage("Select Event Types")
	String SELECT_EVENT_TYPES_ALERT_RULES = "SQLCM.Label.select-event-types-alert-rules";
	
	@I18NMessage("Access Check Passed")
	String ACCESS_CHECK_PASSED_ALERT_RULES = "SQLCM.Label.access-check-alert-rules";
	
	@I18NMessage("Privileged Users")
	String ACCESS_CHECK_PRIVILEGED_USER_ALERT_RULES = "SQLCM.Label.access-check-privileged-user-rules";

	@I18NMessage("Row Counts")
	String ACCESS_CHECK_ROW_COUNTS_ALERT_RULES = "SQLCM.Label.access-check-row-counts-rules";

	
	@I18NMessage("Is Privileged User")
	String IS_PRIVILEGED_USER_ALERT_RULES = "SQLCM.Label.is-privileged-type-alert-rules";

	@I18NMessage("Select Alert Actions")
	String SEL_ALERT_ACTION_ALERT_RULES = "SQLCM.Label.sel-aletr-action-alert-rules";

	@I18NMessage("Email Notification")
	String EMAIL_NOTIFICATION_ALERT_RULES = "SQLCM.Label.email-notification-alert-rules";

	@I18NMessage("Alert Message")
	String ALERT_MESSAGE_ALERT_RULES = "SQLCM.Label.alert-message-alert-rules";

	@I18NMessage("Specify Addresses")
	String ALERT_SPECIFIED_ADDRESSES = "SQLCM.Label.specified-addresses";

	@I18NMessage("Specify email address")
	String SPEC_MAIL_ADDRESS_ALERT_RULES = "SQLCM.Label.spec-mail-address-alert-rules";

	@I18NMessage("Windows Event Log Entry")
	String WIN_LOG_ENTRY_ALERT_RULES = "SQLCM.Label.win-log-entry-alert-rules";

	@I18NMessage("SNMP Trap")
	String SNMP_TRAP_ALERT_RULES = "SQLCM.Label.snmp-trap-alert-rules";

	@I18NMessage("Address")
	String ADDRESS_ALETR_RULES = "SQLCM.Label.address-alert-rules";

	@I18NMessage("Port")
	String PORT_ALERT_RULES = "SQLCM.Label.port-alert-rules";

	@I18NMessage("Community")
	String COMMUNITY_ALERT_RULES = "SQLCM.Label.community-alert-rules";

	@I18NMessage("4-Severe")
	String SEVERE_ALERT_RULES = "SQLCM.Label.severe-alert-rules";

	@I18NMessage("3-High")
	String HIGH_ALERT_RULES = "SQLCM.Label.high-alert-rules";

	@I18NMessage("2-Medium")
	String MEDIUM_ALERT_RULES = "SQLCM.Label.medium-alert-rules";

	@I18NMessage("1-Low")
	String LOW_ALERT_RULES = "SQLCM.Label.low-alert-rules";

	@I18NMessage("New Data Alert Rule")
	String DATA_ALERT_RULES_TITLE = "SQLCM.Label.data-alert-rules-title";

	@I18NMessage("Sensitive Column Accessed")
	String SENSITIVE_COLUMN_ACCESSED = "SQLCM.Label.sensitive-column-accessed";
	
	@I18NMessage("Numeric Column Value Changed")
	String NUMRIC_COLUMN_VALUE_CHANGED = "SQLCM.Label.numeric-column-value-changed";

	@I18NMessage("Column Value Changed")
	String COLUMN_VALUE_CHANGED = "SQLCM.Label.column-value-accessed";

	@I18NMessage("This alert will be generated when a column has been read that has been setup for Sensitive Column Auditing.")
	String SENSITIVE_COLUMN_ACCESSED_DESC = "SQLCM.Label.column-value-accessed_desc";

	@I18NMessage("The alert will be generated when a numeric column that has been setup for Before-After Data Auditing has been changed.")
	String COLUMN_VALUE_CHANGED_DESC = "SQLCM.Label.column-value-accessed-desc";

	@I18NMessage("Table Name")
	String SQLCM_TABLE_NAME = "SQLCM.Labels.table-name";

	@I18NMessage("Column Name")
	String SQLCM_COLUMN_NAME = "SQLCM.Labels.column-name";

	@I18NMessage("Agent trace directory reached size limit")
	String TRACE_DIR_FULL_AGENT = "SQLCM.Labels.trace-dir-full-agent";

	@I18NMessage("Collection Server trace directory reached size limit")
	String TRACE_DIR_FULL_COLLECT = "SQLCM.Labels.trace-dir-full-collect";

	@I18NMessage("Agent heartbeat was not received")
	String NO_HEARTBEAT = "SQLCM.Labels.no-heartbeat";

	@I18NMessage("Event database is too large")
	String REPOSITORY_TOO_BIG = "SQLCM.Labels.repository-too-big";

	@I18NMessage("Agent cannot connect to audited instance")
	String SQL_SERVER_DOWN = "SQLCM.Labels.sql-server-down";

	@I18NMessage("The SQLcompliance Agent trace directory on the audited SQL Server computer reached the specified percentage of its size limit.")
	String TRACE_DIR_FULL_AGENT_DESC = "SQLCM.Labels.trace-dir-full-agent-desc";

	@I18NMessage("The Collection Server trace directory reached the specified size limit.")
	String TRACE_DIR_FULL_COLLECT_DESC = "SQLCM.Labels.trace-dir-full-collect-desc";

	@I18NMessage("The Collection Server has not received a heartbeat from one of the deployed SQLcompliance Agents.")
	String NO_HEARTBEAT_DESC = "SQLCM.Labels.no-heartbeat-desc";

	@I18NMessage("An event database for an audited SQL Server instance reached the specified maximum size.")
	String REPOSITORY_TOO_BIG_DESC = "SQLCM.Labels.repository-too-big-desc";

	@I18NMessage("The SQLcompliance Agent cannot connect to one of the audited SQL Server instances.")
	String SQL_SERVER_DOWN_DESC = "SQLCM.Labels.sql-server-down-desc";

	@I18NMessage("<h3>SQL Server Event Type</h3>" + "<p>"
			+ "Specify the name for this rule and select the categorization level for the alert. Also, Select the SQL Server event you want to monitor. " + "</p>")
	String SQL_SERVER_EVENT_TYPE_TIPS = "SQLCM.Html.sql-server-event-type.tips";

	@I18NMessage("<h3>SQL Server Object Type</h3>"
			+ "<p>"
			+ " Select the SQL Server, database, object, or host you want to monitor. "
			+ "</p>" + "<h3>Additional Event Filters</h3>" + "<p>"
			+ " Specify when the selected event should trigger this alert. "
			+ "</p>")
	String SQL_SERVER_OBJECT_TYPE_TIPS = "SQLCM.Html.sql-server-object-type.tips";

	@I18NMessage("<h3>Alert Actions</h3>"
			+ "<p>"
			+ " Select the action to be taken for events that match this rule. "
			+ "</p>")
	String SQL_ALERT_ACTIONS_TIPS = "SQLCM.Html.sql-alert-actions.tips";

	@I18NMessage("<h3>Finish Alert Rule</h3>"
			+ "<p>"
			+ " Specify a name for this rule and select the categorization level for the alert.  Also, select whether to enable this rule now. "
			+ "</p>")
	String SQL_FINISH_ALERT_RULES_TIPS = "SQLCM.Html.sql-finish-alert-rules.tips";

	@I18NMessage("<h3>Data Alert Type</h3>"
			+ "<p>"
			+ "Specify a name for this rule and select the categorization level for the alert.  Also, Define the rule that will alert on Sensitive Column access or Before After Data change.  Click Next to continue."
			+ "</p>")
	String SQL_DATA_ALERT_TYPE_TIPS = "SQLCM.Html.sql-data-alert-type.tips";

	@I18NMessage("<h3>Data Alert Type</h3>"
			+ "<p>"
			+ "Select the SQL Server, database, table, and column that you want to monitor."
			+ "</p>")
	String SQL_DATA_ALERT_TYPE2_TIPS = "SQLCM.Html.sql-data-alert-type2.tips";

	@I18NMessage("<h3>Alert Actions</h3>"
			+ "<p>"
			+ "Select the action to be taken when SQL Server data matches this rule."
			+ "</p>")
	String SQL_DATA_ALERT_ACTIONS_TIPS = "SQLCM.Html.sql-data-alert-actions.tips";

	@I18NMessage("<h3>Finish Data Alert Rule</h3>"
			+ "<p>"
			+ "Select whether to enable this rule now."
			+ "</p>")
	String SQL_FINISH_DATA_ALERT_RULES_TIPS = "SQLCM.Html.sql-finish-data-alert-rules.tips";

	@I18NMessage("<h3>Status Alert Type</h3>" + "<p>"
			+ "Specify the name for this rule and select the categorization level for the alert. Also, Select the status alert type that you want to monitor." + "</p>")
	String SQL_STATUS_ALERT_TYPE_TIPS = "SQLCM.Html.sql-status-alert-type.tips";

	@I18NMessage("<h3>Alert Actions</h3>"
			+ "<p>"
			+ "Select the action to be taken when a SQL Compliance Manager status matches this rule."
			+ "</p>")
	String SQL_STATUS_ALERT_ACTIONS_TIPS = "SQLCM.Html.sql-status-alert-actions.tips";

	@I18NMessage("<h3>Finish Alert Rule</h3>"
			+ "<p>"
			+ "Select whether to enable this rule now."
			+ "</p>")
	String SQL_FINISH_EVENT_ALERT_RULES_TIPS = "SQLCM.Html.sql-finish-event-alert-rules.tips";
	
	@I18NMessage("<h3>Finish Status Alert Rule</h3>"
			+ "<p>"
			+ "Select whether to enable this rule now."
			+ "</p>")
	String SQL_FINISH_STATUS_ALERT_RULES_TIPS = "SQLCM.Html.sql-finish-status-alert-rules.tips";

	@I18NMessage("Data Definition(DDL)")
	String DATA_DEFINITION_EVENT_FILTER_RULES = "SQLCM.Label.data-definition-add-event-filter-rules";

	@I18NMessage("Specific Events")
	String SPECIFIC_EVENT_EVENT_FILTER_RULES = "SQLCM.Label.specific-events-add-event-filter-rules";

	@I18NMessage("Administrative Activity")
	String ADMINISTRATIVE_ACTIVITY_EVENT_FILTER_RULES = "SQLCM.Label.administrative-activity-add-event-filter-rules";

	@I18NMessage("Data Manipulation(DML)")
	String DATA_MANIPULATION_EVENT_FILTER_RULES = "SQLCM.Label.data-manipulation-add-event-filter-rules";

	@I18NMessage("Login Activity")
	String LOGIN_ACTIVITY_EVENT_FILTER_RULES = "SQLCM.Label.login-activity-add-event-filter-rules";

	@I18NMessage("User Defined Events")
	String USER_DEFINED_ACTIVITY_EVENT_FILTER_RULES = "SQLCM.Label.user-defined-events-add-event-filter-rules";

	@I18NMessage("Specify SQL Servers")
	String SPECIFIED_SQL_SERVERS = "SQLCM.Labels.specified-sql-server";

	@I18NMessage("Specify Databases")
	String SPECIFIED_SQL_DATABASES = "SQLCM.Labels.specified-databases";

	@I18NMessage("Specify Objects")
	String SPECIFIED_SQL_OBJECTS = "SQLCM.Labels.specified-objects";

	@I18NMessage("Specify SQL Servers")
	String SPECIFY_SQL_SERVERS = "SQLCM.Labels.specify-sql-server";

	@I18NMessage("Select the SQL Servers monitored by this alert:")
	String SQL_SERVERS_MONITORED = "SQLCM.Labels.sql-server-monitored";

	@I18NMessage("Specify Database Objects")
	String SPECIFY_DATABASE_OBJECTS = "SQLCM.Labels.specify-database-objects";

	@I18NMessage("Match all database object names")
	String MATCH_DATABASE_OBJECT_NAMES = "SQLCM.Labels.match-database-object-names";

	@I18NMessage("Match all database names")
	String MATCH_DATABASE_NAMES = "SQLCM.Labels.match-database-names";
	
	@I18NMessage("Listed")
	String LISTED = "SQLCM.Labels.listed";

	@I18NMessage("Except those listed")
	String EXCEPT_LISTED = "SQLCM.Labels.except-those-listed";

	@I18NMessage("Specify database object names to match (wildcards allowed)")
	String DATABASE_OBJECT_NAME_MATCH = "SQLCM.Labels.database-object-name-match";

	@I18NMessage("Object names to match:")
	String OBJECT_NAME_MATCH = "SQLCM.Labels.object-name-match";

	@I18NMessage("Specify Names")
	String SPECIFY_NAMES = "SQLCM.Labels.specify-names";

	@I18NMessage("Specify Words")
	String SPECIFY_WORDS = "SQLCM.Labels.specify-words";

	@I18NMessage("True")
	String LABEL_TRUE = "SQLCM.Labels.true";

	@I18NMessage("False")
	String LABEL_FALSE = "SQLCM.Labels.false";

	@I18NMessage("Specify Application Names")
	String SPECIFY_APPLICATION_NAME = "SQLCM.Labels.specify-application-name";

	@I18NMessage("Match all application names")
	String MATCH_ALL_APPLICATION_NAMES = "SQLCM.Labels.match-all-application-names";
	
	@I18NMessage("Specify Privileged Users")
	String SPECIFY_PRIVILEGED_USERS = "SQLCM.Labels.specify-privileged-users";

	@I18NMessage("Specify Row Count Threshold")
	String SPECIFY_ROW_COUNT_THRESHOLD = "SQLCM.Labels.specify-row-count-threshold";
	
	@I18NMessage("Match all privileged users")
	String MATCH_ALL_PRIVILEGED_USERS = "SQLCM.Labels.match-all-privileged-users";
	
	@I18NMessage("Specify privileged users to match (wildcards allowed):")
	String PRIVILEGED_USERS_TO_MATCH = "SQLCM.Labels.privileged-users-to-match";
	
	@I18NMessage("Specify database object names to match (wildcards allowed):")
	String DATABASE_APPLICATION_NAME_MATCH = "SQLCM.Labels.database-object-name-match";

	@I18NMessage("Specify database names to match (wildcards allowed):")
	String DATABASE_DATABASE_NAME_MATCH = "SQLCM.Labels.database-name-match";
	
	@I18NMessage("Specify application names to match (wildcards allowed):")
	String DATABASE_APPLICATION_TO_NAME_MATCH = "SQLCM.Labels.database-application-name-match";
	
	@I18NMessage("Specify privileged user names to match (wildcards allowed):")
	String DATABASE_PRIVILEGED_USER_TO_NAME_MATCH = "SQLCM.Labels.privileged-user-name-match";
	
	@I18NMessage("Specify login names to match (wildcards allowed):")
	String DATABASE_LOGIN_TO_NAME_MATCH = "SQLCM.Labels.database-login-name-match";
	
	@I18NMessage("Object names to match:")
	String APPLICATION_NAME_MATCH = "SQLCM.Labels.application-name-match";
	
	@I18NMessage("Privileged users to match:")
	String PRIVILEGED_USERS_MATCH = "SQLCM.Labels.privileged-users-match";
	
	@I18NMessage("Login names to match:")
	String LOGIN_NAME_TO_MATCH = "SQLCM.Labels.login-name-match";
	
	@I18NMessage("Application names to match:")
	String APPLICATION_TO_NAME_MATCH = "SQLCM.Labels.appliaction-name-match";
	
	@I18NMessage("Hostnames to match:")
	String HOSTNAMES_NAME_MATCH = "SQLCM.Labels.hostnames-name-match";
	
	@I18NMessage("Databse names to match:")
	String DATABASE_NAME_MATCH = "SQLCM.Labels.database-match";

	@I18NMessage("Match Null Application Names")
	String MATCH_NULL_APPLICATION_NAME = "SQLCM.Labels.match_null_application-name";

	@I18NMessage("Match Empty or Blank Application Names")
	String MATCH_EMPTY_APPLICATION_NAME = "SQLCM.Labels.match_empty_application-name";

	@I18NMessage("Specify Login Names")
	String SPECIFY_LOGIN_NAME = "SQLCM.Labels.specify-login-name";

	@I18NMessage("Match all login names")
	String MATCH_ALL_LOGIN_NAMES = "SQLCM.Labels.match-all-login-names";

	@I18NMessage("Specify login names to match (wildcards allowed)")
	String SPECIFY_LOGIN_NAME_MATCH = "SQLCM.Labels.specify-login-name-match";

	@I18NMessage("Login names to match:")
	String LOGIN_NAME_MATCH = "SQLCM.Labels.login-name-match";

	@I18NMessage("Specify Hostnames")
	String SPECIFY_HOSTNAME = "SQLCM.Labels.specify-hostname";

	@I18NMessage("Match all hostnames")
	String MATCH_ALL_HOSTNAMES = "SQLCM.Labels.match-all-hostnames";

	@I18NMessage("Specify hostnames to match (wildcards allowed):")
	String DATABASE_HOSTNAME_MATCH = "SQLCM.Labels.database-hostname-match";

	@I18NMessage("Hostnames to match:")
	String HOSTNAME_MATCH = "SQLCM.Labels.hostname-match";

	@I18NMessage("Match Null Hostnames")
	String MATCH_NULL_HOSTNAME = "SQLCM.Labels.match_null_hostname";

	@I18NMessage("Match Empty or Blank Hostnames")
	String MATCH_EMPTY_HOSTNAME = "SQLCM.Labels.match_empty_hostname";

	@I18NMessage("Search Text")
	String SEARCH_TEXT = "SQLCM.Labels.search_text";

	@I18NMessage("Match all search text")
	String MATCH_ALL_SEARCH_TEXT = "SQLCM.Labels.match-all-search-text";

	@I18NMessage("Specify a word or phrase to search for in the sender's address")
	String SPECIFY_SENDER_ADDRESS = "SQLCM.Labels.specify-sender-address";

	@I18NMessage("List")
	String LABEL_LIST = "SQLCM.Labels.label-list";

	@I18NMessage("Page Setup")
	String PAGE_SETUP = "SQLCM.Labels.page-setup";

	@I18NMessage("Select audit report from icons on the right or dropdown menu to get started")
	String AUDIT_REPORT_SIDE_ACTION_DESCRIPTION = "SQLCM.Labels.audit.report.sideaction-description";

	@I18NMessage("To")
	String AUDIT_REPORT_SIDE_ACTION_TO = "SQLCM.Labels.auditReport.sideAction.to";

	@I18NMessage("From")
	String AUDIT_REPORT_SIDE_ACTION_FROM = "SQLCM.Labels.auditReport.sideAction.from";

	@I18NMessage("Server")
	String AUDIT_REPORT_SIDE_ACTION_SERVER = "SQLCM.Labels.auditReport.sideAction.server";
	
	@I18NMessage("Default Status")
	String AUDIT_REPORT_SIDE_ACTION_DEFAULT_STATUS = "SQLCM.Labels.auditReport.sideAction.default-status";

	@I18NMessage("Instances")
	String AUDIT_REPORT_SIDE_ACTION_INSTANCES = "SQLCM.Labels.auditReport.sideAction.instances";

	@I18NMessage("Privileged User")
	String AUDIT_REPORT_SIDE_ACTION_PRIVILEGED_USER = "SQLCM.Labels.auditReport.sideAction.privileged-user";

	@I18NMessage("Database")
	String AUDIT_REPORT_SIDE_ACTION_DATABASE = "SQLCM.Labels.auditReport.sideAction.database";
	
	@I18NMessage("Audit Settings")
	String AUDIT_REPORT_SIDE_ACTION_AUDIT_SETTINGS = "SQLCM.Labels.auditReport.sideAction.audit-settings";
	
	@I18NMessage("Regulatory Guidelines")
	String AUDIT_REPORT_SIDE_ACTION_REGULATORY_GUIDELINES = "SQLCM.Labels.auditReport.sideAction.regulatory-guidelines";
	
	@I18NMessage("Values")
	String AUDIT_REPORT_SIDE_ACTION_VALUES = "SQLCM.Labels.auditReport.sideAction.values";

	@I18NMessage("Application")
	String AUDIT_REPORT_SIDE_ACTION_APPLICATION = "SQLCM.Labels.auditReport.sideAction.application";

	@I18NMessage("Login")
	String AUDIT_REPORT_SIDE_ACTION_LOGIN = "SQLCM.Labels.auditReport.sideAction.login";

	@I18NMessage("Table Name")
	String AUDIT_REPORT_SIDE_ACTION_OBJECT = "SQLCM.Labels.auditReport.sideAction.object";

	@I18NMessage("Column Name")
	String AUDIT_REPORT_SIDE_ACTION_COLUMN = "SQLCM.Labels.auditReport.sideAction.column";

	@I18NMessage("Row Count Threshold")
	String AUDIT_REPORT_SIDE_ACTION_ROW_COUNT_THRESHOLD = "SQLCM.Labels.auditReport.sideAction.rowCountThreshold";

	@I18NMessage("Primary Key")
	String AUDIT_REPORT_SIDE_ACTION_PRIMARY_KEY = "SQLCM.Labels.auditReport.sideAction.primary-key";

	@I18NMessage("Target Object")
	String AUDIT_REPORT_SIDE_ACTION_TARGET_OBJECT = "SQLCM.Labels.auditReport.sideAction.target-object";

	@I18NMessage("Category")
	String AUDIT_REPORT_SIDE_ACTION_CATEGORY = "SQLCM.Labels.auditReport.sideAction.category";

	@I18NMessage("Show SQL")
	String AUDIT_REPORT_SIDE_ACTION_SHOW_SQL = "SQLCM.Labels.auditReport.sideAction.show-sql";

	
	@I18NMessage("Copy")
	String FROM_EXISTING = "SQLCM.Labels.copy";

	@I18NMessage("Specify an email address for each person to receive this alert:")
	String ALERT_RULES_EMAIL_TEXT = "SQLCM.Labels.email-header";

	@I18NMessage("Use the following email addresses:")
	String ALERT_RULES_EMAIL_LIST = "SQLCM.Labels.email-list";

	@I18NMessage("Title")
	String TITLE = "SQLCM.Labels.title";

	@I18NMessage("Message")
	String MEASSAGE = "SQLCM.Labels.message";

	@I18NMessage("Alert Message Template")
	String MEASSAGE_TEMPLATE = "SQLCM.Labels.message-template";

	@I18NMessage("Double-click a variable to add it to")
	String MEASSAGE_TEMPLATE_TAG = "SQLCM.Html.message-template-tag";
	
	@I18NMessage("the email subject or message.")
	String MEASSAGE_TEMPLATE_TAG_REM = "SQLCM.Html.message-template-tag-rem";

	@I18NMessage("Enable")
	String ENABLE_ALERT_RULES = "SQLCM.Labels.alertRules.enable";

	@I18NMessage("Disable")
	String DISABLE_ALERT_RULES = "SQLCM.Labels.alertRules.disable";

	@I18NMessage("Delete")
	String DELETE_ALERT_RULES = "SQLCM.Labels.alertRules.delete";

	@I18NMessage("Max percentage of trace directory used:")
	String MAX_PERCENTAGE_TARCE_USED = "SQLCM.Labels.alertRules.max-trace-used";

	@I18NMessage("Trace File Directory Size (GB):")
	String TARCE_DIRECTORY_SIZE = "SQLCM.Labels.alertRules.trace-dir-size";

	@I18NMessage("Database Size (GB):")
	String DATABASE_SIZE = "SQLCM.Labels.alertRules.database-size";

	// SQLCM Req4.1.1.2 - Enhanced Instances View - Start
	@I18NMessage("Permissions Check")
	String INSTANCES_OPTION_PERMISSIONS_CHECK = "SQLCM.Labels.instances.options.permissions-check";

	@I18NMessage("Please visit <a target='_blank' style='color: #FE4210' href='https://wiki.idera.com/x/KINi'>this</a> link for additional help.")
	String PERMISSIONS_CHECK_DIALOG_HELP_LINK = "SQLCM.Html.permissions-check-dialog.help-link";
	// SQLCM Req4.1.1.2 - Enhanced Instances View - End

	// SQLCM Req4.1.1.3 Enhanced Audited Instance View - Start
	@I18NMessage("Permissions Check")
	String PERMISSIONS_CHECK = "SQLCM.Labels.permissions-check";
	// SQLCM Req4.1.1.3 Enhanced Audited Instance View - End

	@I18NMessage("Regulation Guidelines")
	String REGULATION_INSTANCES_OPTION_INSTANCE_PROPERTIES = "SQLCM.Labels.instances.options.instance-properties-regulation";

	@I18NMessage("Permissions Check")
	String PERMISSIONS_CHECK_DIALOG_TITLE = "Labels.sql-cm.permissions-check-dialog-title";

	@I18NMessage("Check Permissions")
	String PERMISSIONS_CHECK_DIALOG_CHCK_PERMISSIONS_BUTTON = "Labels.sql-cm.permissions-check-dialog-check-permissions-button";

	@I18NMessage("User")
	String USER = "SQLCM.Labels.user";

	@I18NMessage("Description")
	String DESCRIPTION = "SQLCM.Labels.description";

	@I18NMessage("Failed to collect view state.")
	String FAILED_TO_COLLECT_VIEW_STATE = "Labels.failed-to-collect-view-state";

	@I18NMessage("Logs")
	String LOGS_VIEW = "SQLCM.Labels.logs-View";

	@I18NMessage("Audit Reports")
	String AUDIT_REPORTS = "SQLCM.Labels.audit-reports";

	@I18NMessage("Audit Reports")
	String AUDIT_REPORT_TITLE = "SQLCM.Labels.audit-reports.title";

	@I18NMessage("Application Activity")
	String AUDIT_REPORT_APPLICATION_ACTIVITY_TITLE = "SQLCM.Labels.audit-reports.application.title";

	@I18NMessage("DML Activity (Before-After)")
	String AUDIT_REPORT_DML_ACTIVITY_TITLE = "SQLCM.Labels.audit-reports.dmlActivity.title";

	@I18NMessage("Login Creation History")
	String AUDIT_REPORT_LOGIN_CREATION_HISTORY_TITLE = "SQLCM.Labels.audit-reports.loginCreationHistory.title";

	@I18NMessage("Login Deletion History")
	String AUDIT_REPORT_LOGIN_DELETION_HISTORY_TITLE = "SQLCM.Labels.audit-reports.loginDeletionHistory.title";

	@I18NMessage("Object Activity")
	String AUDIT_REPORT_OBJECT_ACTIVITY_TITLE = "SQLCM.Labels.audit-reports.objectActivity.title";

	@I18NMessage("Table-Data Access by Rowcount")
	String AUDIT_REPORT_ROW_COUNT_TITLE = "SQLCM.Labels.audit-reports.rowCount.title";

	@I18NMessage("Permission Denied Activity")
	String AUDIT_REPORT_PERMISSION_DENIED_ACTIVITY_TITLE = "SQLCM.Labels.audit-reports.permissionDeniedActivity.title";

	@I18NMessage("User Activity History")
	String AUDIT_REPORT_USER_ACTIVITY_HISTORY_TITLE = "SQLCM.Labels.audit-reports.userActivityHistory.title";

	@I18NMessage("Configuration Check")
	String AUDIT_REPORT_CONFIGURATION_CHECK_TITLE = "SQLCM.Labels.audit-reports.configuration.check";

	@I18NMessage("Regulatory Compliance Check")
	String AUDIT_REPORT_REGULATORY_COMPLIANCE_CHECK_TITLE = "SQLCM.Labels.audit-reports.regulatoryCompliance.title";

	@I18NMessage("SQL Compliance Manager service account")
	String ACCOUNT_TYPE_CM_ACCOUNT = "SQLCM.account-type.cm-account";

	@I18NMessage("SQL Server login account")
	String ACCOUNT_TYPE_SERVER_LOGIN_ACCOUNT = "SQLCM.account-type.server-login-account";

	@I18NMessage("Windows user account")
	String ACCOUNT_TYPE_WINDOWS_USER_ACCOUNT = "SQLCM.account-type.windows-user-account";

	@I18NMessage("Enable this filter now")
	String ENABLE_FILTER_NOW = "SQLCM.Labels.enable-filter-now";

	@I18NMessage("Enable this rule now.")
	String ENABLE_THIS_RULE_NOW = "SQLCM.Labels.enable-this-rule-now";

	@I18NMessage("Failed to load users.")
	String FAILED_TO_LOAD_USERS = "Labels.failed-to-load-users";

	@I18NMessage("Edit Alert Rules")
	String EDIT_RULES = "SQLCM.Labels.alertRule.edit";
	
	/*@I18NMessage("Sensitive Column Accessed")
	String SENSITIVE_COLUMN_ACCESSED = "SQLCM.Label.sensitive-column-accessed";*/
	 
	/*@I18NMessage("Numeric Column Value Changed")
	String NUMRIC_COLUMN_VALUE_CHANGED = "SQLCM.Label.numeric-column-value-changed";*/

	@I18NMessage("Select Notification Actions")
	String THRESHOLD_NOTIFICATION_SELECT_NOTIFICATION_ACTION = "SQLCM.Label.threshold-notification-select-notofication-action";

	@I18NMessage("Send notification when the following thresholds have been exceeded")
	String THRESHOLD_NOTIFICATION_SELECT_NOTIFICATION_ACTION_MESSAGE = "SQLCM.Label.threshold-notification-select-notofication-action-message";
	
	@I18NMessage("Choose the instance for which you want to alert on sensitive column access.")
	String DATA_ALERT_SENSITIVE_INSTANCE = "SQLCM.Labels.datarule.sensitive.instance";

	@I18NMessage("Choose the database for which you want to alert on sensitive column access.")
	String DATA_ALERT_SENSITIVE_DATABASE = "SQLCM.Labels.datarule.sensitive.databse";
	
	@I18NMessage("Choose the table for which you want to alert on sensitive column access.")
	String DATA_ALERT_SENSITIVE_TABLE = "SQLCM.Labels.datarule.sensitive.table";
	
	@I18NMessage("Choose the column for which you want to alert on sensitive column access.")
	String DATA_ALERT_SENSITIVE_COLUMN = "SQLCM.Labels.datarule.sensitive.column";
	
	@I18NMessage("Select An Instance")
	String DATA_ALERT_SELECT_SQL_server = "SQLCM.Labels.datarule.select.Instance";
	
	@I18NMessage("Select A Database")
	String DATA_ALERT_SELECT_DB = "SQLCM.Labels.datarule.select.db";
	
	@I18NMessage("Select A Table")
	String DATA_ALERT_SELECT_TABLE= "SQLCM.Labels.datarule.select.table";
	
	
	@I18NMessage("New Rule")
	String NEW_RULE= "SQLCM.Label.new-rule";
	
	@I18NMessage("Activity Log Properties")
	String ACTIVITY_LOG_PROP= "Labels.sql-cm.activity-logs-dialog-title";
	
	@I18NMessage("Change Log Properties")
	String CHANGE_LOG_PROP= "Labels.sql-cm.change-logs-dialog-title";
	
	@I18NMessage("SQL Server")
	String ACTIVITY_LOGS_SQL_SERVER= "Labels.sql-cm.event-properties-dialog-sql-server";
	
	@I18NMessage("Details")
	String ACTIVITY_LOGS_DETAILS= "Labels.sql-cm.event-properties-dialog-sql-details";
	
	@I18NMessage("User")
	String CHANGE_LOGS_USER= "Labels.sql-cm.event-properties-dialog-user";

	@I18NMessage("New Data Alert Rule")
	String NEW_DATA_ALERT_RULES= "Labels.sql-cm.new-data-alert-rules";
	
	@I18NMessage("New Status Alert Rule")
	String NEW_STATUS_ALERT_RULES= "Labels.sql-cm.new-status-alert-rules";
	
	@I18NMessage("Add New Rule")
	String ADD_NEW_RULE= "SQLCM.Labels.addNewAlertRule";	

//Start - SCM-216: Table Listing for current instance
	@I18NMessage("Select a database (blank for all)")
	String SELECT_ONE= "SQLCM.Labels.newSelect";
	
	@I18NMessage("Select a table (blank for all)")
	String SELECT_Table_All= "SQLCM.Labels.newTableSelect";
	
	@I18NMessage("Select Database")
	String SELECT_ARCHIVE= "SQLCM.Labels.newArchiveSelect";
	
	@I18NMessage("All")
	String ALL= "SQLCM.Labels.all";
	
	@I18NMessage("SQL Column Search Settings")
	String COLUMN_SEARCH_TITLE = "SQLCM.Labels.internal_search_title";
	
	@I18NMessage("Are you sure Export to CSV?")
	String COLUMN_SEARCH_EXPORT_CSV = "SQLCM.Labels.internal_search_export_csv";
	
	@I18NMessage("Selected string is also referenced in {0}. Deleting it from this profile will also delete it from those profiles. Do you want to continue?")
	String COLUMN_SEARCH_STRING_DELETE = "SQLCM.Labels.column-search-string-delete";
	
	@I18NMessage("Changes will be saved to {0}. Select Yes to confirm and return to SQL Column Search. Select No to remain in Configure Search")
	String COLUMN_SEARCH_SAVE_PROFILE = "SQLCM.Labels.column-search-save-profile";
	
	@I18NMessage("All user created profiles will be deleted. Are you sure?")
	String COLUMN_SEARCH_RESET_PROFILE = "SQLCM.Labels.column-search-reset-profile";
	
	@I18NMessage("{0} profile will be deleted. Are you sure?")
	String COLUMN_SEARCH_PROFILE_DELETE_CONFIRM = "SQLCM.Labels.column-search-profile-delete-confirm";
	
	@I18NMessage("Selected search string record will be removed. Are you sure?")
	String COLUMN_SEARCH_PROFILE_DELETE_NO_PROFILE_CONFIRM = "SQLCM.Labels.column-search-profile-delete-no-profile-confirm";

	@I18NMessage("Select Items")
	String SELECTITEM= "SQLCM.Labels.addNewitems";
	@I18NMessage("Add Database")
	String ADDDATABASE = "SQLCM.Labels.addDatabase";
	
	@I18NMessage("Sensitive Column Search")
	String COLUMN_SEARCH = "SQLCM.Labels.internal_search";
	
	@I18NMessage("Sensitive Column Search") 
	String SENSITIVE_COLUMN_D_TITLE = "SQLCM.Labels.sensitive_column-d.title";
	
	@I18NMessage("Configure Search")
	String CONFIGURE_SEARCH_LINK = "SQLCM.Labels.administration-manage-sql-server-config-search-link";
	
	//SQLCM_5.4 Search column end
	
	@I18NMessage("Save a View")
	String SAVE_VIEW_LABEL = "SQLCM.Labels.save.view";
	
	@I18NMessage("Enter a name to Save View")
	String SAVE_VIEW_DESC = "SQLCM.Labels.save.view.desc";
	
	@I18NMessage("Save")
	String SAVE = "SQLCM.Label.save";
	
	@I18NMessage("Select A Column")
	String DATA_ALERT_SELECT_COLUMN= "SQLCM.Labels.datarule.select.column";
	
	@I18NMessage("Enter Agent Credentials")
	String ENTER_AGENT_CREDENTIALS_TITLE = "SQLCM.enter-agent-credentials.title";
	
	@I18NMessage("UPDATE NOW")
	String UPDATE_NOW = "SQLCM.update-now";
	
	@I18NMessage("SQL Server version")
	String SQL_SERVER_VERSION_COLUMN = "SQLCM.Labels.instances.sql-server-version-column";
	
	@I18NMessage("Audited DBs")
	String AUDITED_DB_COLUMN = "SQLCM.Labels.instances.audited-dbs-column";
	
	@I18NMessage("Widget Audited Instance")
	String WIDGET_AUDITED_INSTANCES = "SQLCM.Labels.widget-audited-instance";
	
	@I18NMessage("Edit Event Alert Rule")
	String EDIT_EVENT_ALERT_RULES = "SQLCM.Labels.edit-event-alert-rules";
	
	@I18NMessage("Edit Status Alert Rule")
	String EDIT_STATUS_ALERT_RULES = "SQLCM.Labels.edit-status-alert-rules";
	
	@I18NMessage("Edit Data Alert Rule")
	String EDIT_DATA_ALERT_RULES = "SQLCM.Labels.edit-data-alert-rules";
	
	@I18NMessage("Before-After Data Auditing has not been enabled on any of the audited instances.  Go to the Audited Database Properties Dialog to enable this feature.")
	String MSG_DATA_ALERT_RULES_BEFORE_AFETR = "SQLCM.Labels.msg-data-alert-rules-before-after";

	@I18NMessage("Sensitive Column auditing has not been enabled on any of the audited instances.  Go to the Audited Database Properties Dialog to enable this feature.")
	String MSG_DATA_ALERT_RULES_SENSITIVE_COLUMN = "SQLCM.Labels.msg-data-alert-rules-sensitive-column";

	@I18NMessage("An event of " +'"'+ "$AlertTypeName$" +'"'+ " occurred on Computer:$ComputerName$ Instance:$ServerName$ at $AlertTime$")
	String DEFAULT_STATUS_ALERT_RULES_MSG = "SQLCM.Labels.default-status-alert-rules-msg";

	@I18NMessage("$AlertLevel$ Status Alert")
	String DEFAULT_STATUS_ALERT_RULES_TITLE = "SQLCM.Labels.default-status-alert-rules-title";

	@I18NMessage("An event of " +'"'+ "$AlertType$" +'"'+ " occurred on $ServerName$ $EventTime$")
	String DEFAULT_DATA_ALERT_RULES_MSG = "SQLCM.Labels.default-data-alert-rules-msg";

	@I18NMessage("$AlertLevel$ Data Alert")
	String DEFAULT_DATA_ALERT_RULES_TITLE = "SQLCM.Labels.default-data-alert-rules-title";

	@I18NMessage("$EventType$ event occurred on $ServerName$ instance at $EventTime$ by $Login$.")
	String DEFAULT_EVENT_ALERT_RULES_MSG = "SQLCM.Labels.default-event-alert-rules-msg";

	@I18NMessage("$AlertLevel$ Alert on $ServerName$ instance")
	String DEFAULT_EVENT_ALERT_RULES_TITLE = "SQLCM.Labels.default-event-alert-rules-title";
	
	@I18NMessage("Clear All")
	String CLEAR_ALL="SQLCM.Labels.clear-all";
	
	@I18NMessage("<h3>SQL Server Event Type</h3>" + "<p>"
	        + "This window allows you to specify the general properties for this filter."
			+ "This window allows you to select the type of events you want to filter." + "</p>")
	String SQL_SERVER_EVENT_TYPE_FILTER_TIPS = "SQLCM.Html.sql-server-event-type-filter.tips";
	
	@I18NMessage("<h3>SQL Server Object Type</h3>" + "<p>"
			+ "This window allows you to select the objects associated with filtered events." + "</p>"
			+"<h3>SQL Server Event Source</h3>" + "<p>"
			+ "This window allows you to select the users and applications that initiated the filtered events." + "</p>")
	String SQL_SERVER_ObJECT_TYPE_FILTER_TIPS = "SQLCM.Html.sql-server-object-type-filter.tips";
	
	@I18NMessage("<h3>Finish Event Filter</h3>" + "<p>"
			+ "Select whether to enable this filter now." + "</p>")
	String SQL_SERVER_FINAL_FILTER_TIPS = "SQLCM.Html.sql-server-final-filter.tips";
	
	@I18NMessage("Invalid Alert Rule")
	String INVALID_RULE_TITLE = "Labels.invalid.rule.title";
	
	@I18NMessage("This alert rule contains incomplete criteria.  Alerts will not be generated from this rule until these criteria are removed or properly specified.")
	String INVALID_ALERT_RULE = "Labels.invalid.alert.rule";
	
	@I18NMessage("Invalid Event Filter")
	String INVALID_FILTER_TITLE = "Labels.invalid.filter.title";
	
	@I18NMessage("This event filter filters all events for a server.  Disable auditing on the target server instead."+
				"Events will not be filtered with this rule until more criteria are added.")
	String INVALID_EVENT_FILTER_CONDITION = "Labels.invalid.filter.condition";
	
	@I18NMessage("This event filter contains incomplete criteria.  Events will not be filtered with this rule until these criteria are removed or properly specified.")
	String INVALID_EVENT_FILTER = "Labels.invalid.event.filter";
	
	@I18NMessage("$AlertLevel$ Threshold Alert")
	String DEFAULT_THRESHOLD_ALERT_RULES_TITLE = "SQLCM.Labels.default-threshold-alert-rules-title";
	
	@I18NMessage('"'+ "$AlertTypeName$" +'"'+ " threshold event occurred on $ServerName$ instance at $AlertTime$.")
	String DEFAULT_THRESHOLD_ALERT_RULES_MSG = "SQLCM.Labels.default-threshold-alert-rules-msg";

	@I18NMessage("Threshold Message")
	String ALERT_MESSAGE_THRESHOLD_RULES = "SQLCM.Label.alert-message-threshold-rules";
	
	@I18NMessage("Unable to create the new alert rule. "
			+ "Message: String or binary data would be truncated. "
			+ "The statement has been terminated.")
	String INVALID_EVENT_ALERT_MESSAGE = "Labels.invalid.event.alert.message";
	
	@I18NMessage("Unable to create the new data alert rule. "
			+ "Message: String or binary data would be truncated. "
			+ "The statement has been terminated.")
	String INVALID_DATA_ALERT_MESSAGE = "Labels.invalid.data.alert.message";
	
	@I18NMessage("Unable to create the new status alert rule. "
			+ "Message: String or binary data would be truncated. "
			+ "The statement has been terminated.")
	String INVALID_STATUS_ALERT_MESSAGE = "Labels.invalid.status.alert.message";
	
	@I18NMessage("Filter events generated by")
	String FILTER_EVENTS_GENERATED = "SQLCM.Label.filter.events.generated.by";

//START SQLCm-5.4

	@I18NMessage("Import Sensitive Columns from CSV")
	String IMPORT_SESITIVE_COLUMNS_FROM_CSV = "SQLCM.labels.import-sensitive-columns-from-csv";
	
	@I18NMessage("Import Sensitive Columns ")
	String IMPORT_SESITIVE_COLUMNS = "SQLCM.labels.import-sensitive-columns";
	
	@I18NMessage("CSV file must have a row for each database you want to add for sensitive column auditing. First row value has to be database name, second value has to be table name followed by values for table's column names. If a row has only two values, first for database name and second for table name, all the columns will be selected for sensitive columns. A row with only one value is invalid and will be ignored. See examples below:")
	String IMPORT_Sensitive_text = "SQLCM.labels.import-sensitive-text";
	
	@I18NMessage("Please check the table columns you want to add to sensitive columns and uncheck those which you want to ignore. Hierarchy of database names, table names and column names to be audited for sensitive columns access is given below.")
	String IMPORT_Sensitive_text1 = "SQLCM.labels.import-sensitive-text1";
	
	@I18NMessage("Browse")
	String BROWSE= "SQLCM.Labels.browse";
	
	@I18NMessage("Import Audit Settings")
	String IMPORT_SERVER_AUDIT_SETTING= "SQLCM.Labels.import-audit-setting";
	
	@I18NMessage("Export Audit Settings")
	String EXPORT_SERVER_AUDIT_SETTING= "SQLCM.Labels.export-audit-setting";
	
	@I18NMessage("Select File to Import")
	String SELECT_FILE_TO_IMPORT= "SQLCM.Labels.select-file-to-import";
	
	@I18NMessage("This window allows you to select an audit settings file to import.")
	String SELECT_AN_AUDIT_FILE_TO_IMPORT = "SQLCM.Labels.audit-file-to-import";
	
	@I18NMessage("Select an Audit Settings file to import:")
	String SELECT_AN_AUDIT_SETTING_FILE_TO_IMPORT = "SQLCM.Labels.audit-setting-file-to-import";
	
	@I18NMessage("Finish")
	String DB_PROPS_DIALOG_FINISH_BUTTON = "SQLCM.db-props.Finish-button";
	
	@I18NMessage("Next >")
	String DB_PROPS_DIALOG_NEXT_BUTTON = "SQLCM.db-props.Next-button";
	
	@I18NMessage("Back <")
	String DB_PROPS_DIALOG_BACK_BUTTON = "SQLCM.db-props.Back-button";

	@I18NMessage("Select the audit setting to import:")
	String SELECT_AUDIT_SETTING_TO_IMPORT="SQLCM.Labels.select-audit-setting-to-import";
	
	@I18NMessage("Server Audit Setting")
	String SELECT_SERVER_AUDIT_SETTING="SQLCM.Labels.select-server-audit-setting";
	
	@I18NMessage("Server privileged User Audit Setting")
	String SERVER_PRIVILEGED_AUDIT_SETTING="SQLCM.Labels.server-privileged-audit-setting";
	
	@I18NMessage("Database Audit Setting")
	String SELECT_DB_AUDIT_SETTING="SQLCM.Labels.select-db-audit-setting";
	
	@I18NMessage("Database privileged User Audit Setting")
	String DB_PRIVILEGED_AUDIT_SETTING="SQLCM.Labels.db-privileged-audit-setting";
	
	@I18NMessage("Only import for matching database names")
	String IMPORT_FOR_MATCHING_DATABASE="SQLCM.Labels.import-for-matching-database";
	
	@I18NMessage("Select Database Setting to Import:")
	String SELECT_DB_SETTING_TO_IMPORT="SQLCM.Labels.select-db-setting-to-import";

	@I18NMessage("The window allows you to select the servers to receive the audit settings." )
	String TARGET_SERVER_TEXT="SQLCM.Labels.target-server-text";
	
	@I18NMessage("Select Target Servers to Import:")
	String SELECT_TARGET_SERVERS_TO_IMPORT="SQLCM.Labels.select-target-servers-to-import";

	@I18NMessage("Target Databases")
	String TARGET_DB="SQLCM.Labels.target-db";

	@I18NMessage("The window allows you to select the databases to receive the audit settings.")
	String  TARGET_DB_TEXT="SQLCM.Labels.target-db-text";
	@I18NMessage("Select Target Databases to Import:")
	String SELECT_TARGET_DB_TO_IMPORT="SQLCM.Labels.select-target-db-to-import";

	@I18NMessage("Browse")
	String BROWSE_IN_DATABASE= "SQLCM.Labels.browse-database";
	@I18NMessage("This Window allows you to select an audit setting file to import.")
	String SELECT_AUDIT_FILE_TEXT="SQLCM.Labels.select-audit-file-text";
	
	@I18NMessage("This Window allows you to select the type of audit setting  to import.")
	String SELECT_AUDIT_FILE_TEXT_NEXT ="SQLCM.Labels.select-audit-file-text-next";
	
	@I18NMessage("Import Audit Setting")
	String IMPORT_DATABASE_AUDIT_SETTING="SQLCM.Labels.import-database-audit-setting";
	
	@I18NMessage("Summary")
	String SUMMARY_TITLE="SQLCM.Labels.summary-title";
	
	@I18NMessage("This window allows you to verify your choices.")
	String SUMMARY_CHOICE="SQLCM.Labels.summary-text";
	
	@I18NMessage("Add to current audit setting")
	String ADD_TO_CURRENT_AUDIT_SETTING="SQLCM.Labels.add-to-current-audit-setting";
	
	@I18NMessage("Overwrite current audit setting")
	String OVERWRITE_CURRENT_AUDIT_SETTING="SQLCM.Labels.override-current-audit-setting";
	
	@I18NMessage("You are done. Click Finish to perform the import.")
	String YOU_ARE_DONE_CLICK_FINISH_TO_IMPORT="SQLCM.Labels.database-summary-final-text";
	
	@I18NMessage("Target Servers")
	String TARGET_SERVERS ="SQLCM.Labels.target-server";
	
	@I18NMessage("Sensitive Column Search")
	String Sensitive_Column_Search ="SQLCM.Labels.sensitive-column-search";
	
	@I18NMessage("Select an audit setting to import")
	String SELECT_AN_AUDIT_SETTING_TO_IMPORT ="SQLCM.Labels.database-summary-final-option-text";
	
    @I18NMessage("Export Audit Settings")
    String INSTANCES_OPTION_EXPORT_AUDIT_SETTINGS = "SQLCM.Labels.instances.options.export-audit-settings";
    
    @I18NMessage("Filtered By:")
    String INSTANCES_ALERT_FILTERED_BY = "SQLCM.Labels.instances.instance.alert.Filtered-by";
    
    @I18NMessage("Add New Rule:")
    String INSTANCES_ALERT_RULE_NEW_ADD = "SQLCM.Labels.instances.instance.alert.rule-add";
	 
	@I18NMessage("Showing page")
	String FOOT_SHOWING_PAGE = "Labels.sql-cm-listbox.showing-page";
	
	@I18NMessage("Event Filters")
	String EVENT_FILTERS_HEADING = "SQLCM.Labels.event-filters-heading";
	
	@I18NMessage("<p>Specific user tables can be selected for Before-After Data auditing. To filter the available tables, enter text in text field and click \"Enter\" on your keyboard (or click the magnifier icon). "
			+ "To revert, clear text field and click \"Enter\".</p>")
	String BEFORE_AFTER_LABLE = "SQLCM.Lables.before-after-lable";
	
	@I18NMessage("Specific user tables can be selected for DML/Select auditing. To filter the available tables, enter text in text field and click \"Enter\" on your keyboard (or click the magnifier icon). "
			+ "To revert, clear text field and click \"Enter\".")
	String DML_OR_SELECT_LABLE = "SQLCM.Lables.dml-select-lable";
	
	@I18NMessage("<p>Specific user tables can be selected for Sensitive Column auditing. To filter the available tables, enter text in text field and click \"Enter\" on your keyboard (or click the magnifier icon). "
			+ "To revert, clear text field and click \"Enter\".</p>")
	String SENSITIVE_LABLE = "SQLCM.Lable.sensitive-lable";
	 
	@I18NMessage(" ")
	String SQLCM_LABEL_QUESTION = "SQLCM.Label.Question";
	
	@I18NMessage("Total items")
	String SQLCM_LABEL_TOTAL_ITEMS = "SQLCM.Label.Total_Items";
	
	@I18NMessage( "If you delete this user, they will no longer have access to the IDERA Dashboard and SQL Compliance Manager. Are you sure you wish to continue and delete the user?")
	 String USERS_DELETE_CONFIRMATION_MESSAGE = "SQLCM.Labels.users_delete_confirmation_message";
	
	@I18NMessage("Out of Range (30-3600)")
	String REFRESH_TIME_DURATION = "Labels.sql-cm-listbox.refresh-time-duration";
	
	@I18NMessage("The files Microsoft.SQLServer.XEvent.Linq.dll and Microsoft.SQLServer.XE.Core.dll are missing. Please download and install the Shared Management Objects and corresponding CLR Types from the SQL Server 2016 Feature Pack. Learn more:")
	 String EXTENDED_EVENTS_LINQ_LOAD_MESSAGE = "Labels.sqlcm.extended.event-linq-load-message";
	
	@I18NMessage("http://wiki.idera.com/x/NoBfB")
	 String EXTENDED_EVENTS_LINQ_LOAD_MESSAGE_LINK = "Labels.sqlcm.extended.event-linq-load-message-link";	
	
	@I18NMessage("Extended Events can not be turned on at this time. Please try again after some time. If problem persists, contact IDERA support.")
	 String EXTENDED_EVENTS_GENERIC_ERROR_MESSAGE = "Labels.sqlcm.capture-sql-dml-select-extended-error";
	
	 @I18NMessage("Row Count")
	  String EVENT_PROPERTIES_ROW_COUNT = "Labels.sql-cm.event-properties-row-count";

	@I18NMessage("Microsoft Excel does not support more than 32767 characters in a cell. If there is a SQL Statement that contains more than 32767 characters, the report will be truncated.")
	String INSTANCE_PROPERTIES_DIALOG_ADVANCED_TAB_REPORT_WARNING = "Labels.sql-cm.instance-properties-dialog-advanced-tab-report-warning";

	@I18NMessage("Registered SQL Server Properties")
	String REGISTERED_SQL_SERVER_PROPERTIES_TITLE = "SQLCM.Labels.registered-sql-server-properties-title";
	@I18NMessage("Configure")
	String REGULATION_GUIDELINE_CONFIGURE_BUTTON = "SQLCM.Labels.regulation-guideline-configure-button";

	@I18NMessage("Configure Before After Data Change")
	String REGULATION_GUIDELINE_BEFORE_AFTER_TITLE = "SQLCM.Label.regulation-guideline-dialog.title";
	
	@I18NMessage("Configure Sensitive Column")
	String REGULATION_GUIDELINE_SENSITIVE_COLUMN_TITLE = "SQLCM.Label.regulation-guideline-dialog.sensitive-column-title";
	
	@I18NMessage("Role")
	String REPORT_ROW_COUNT_ROLENAME_TITLE = "SQLCM.Labels.report-row-count-roleName-title";

	@I18NMessage("SPID")
	String REPORT_ROW_REPORT_COUNT_PROPERTIES_TITLE = "SQLCM.Labels.report-row-count-SPID-title";

	@I18NMessage("# of Rows")
	String REPORT_ROW_COUNT_NUMBER_ROWS_TITLE = "SQLCM.Labels.report-row-count-number-of-rows-title";

	@I18NMessage("Column")
	String REPORT_ROW_COUNT_COLUMN_NAME_TITLE = "SQLCM.Labels.report-row-count-column-name-title";
	
	@I18NMessage("Server Instance")
	String REPORT_ROW_COUNT_SERVER_NAME_TITLE = "SQLCM.Labels.report-row-count-server-instance-title";
	
	@I18NMessage("Table Name")
	String REPORT_ROW_COUNT_TABLE_NAME_TITLE = "SQLCM.Labels.report-row-count-table-name-title";
	
	@I18NMessage("Date/Time")
	String REPORT_ROW_COUNT_DATE_TIME_TITLE = "SQLCM.Labels.report-row-count-date-time-title";
	//5.5 Audit Logs Start
	
	@I18NMessage("Capture DML and SELECT activities using SQL Server Audit Specifications ")
	String SQL_SERVER_AUDIT_SPECIFICATIONS = "Labels.sql-server.audit.specifications";

	@I18NMessage("SQL Server Audit Specifications functionality is supported for SQL Server 2017 and above.")
	String AUDIT_LOGS_SQL_SERVER_VERSION_ERROR = "Labels.sqlcm.audit-logs-sql-server-version-error";

	@I18NMessage("SQL Server Audit Specifications could not be switched on because the agent is not reachable. Please check the agent status and try again.")
	String AUDIT_LOGS_AGENT_NOT_REACHABLE = "Labels.sqlcm.audit-logs-agent-not-reachable";

	@I18NMessage("SQL Server Audit Specifications functionality is supported for 5.5 and later version of SQL Compliance Manager Agent.")
	String AUDIT_LOGS_AGENT_VERSION_ERROR = "Labels.sqlcm.audit-logs-agent-version-error";
	
	@I18NMessage("SQL Server Audit Specifications can not be turned on at this time. Please try again after some time. If problem persists, contact IDERA support.")
	String AUDIT_LOGS_GENERIC_ERROR = "Labels.sqlcm.audit-logs-generic-error";
		
	
	@I18NMessage("Capture DML and Select Activities")
	String CAPTURE_DML_AND_SELECT_ACTIVITIES = "Labels.capture-dml-and-select-activities";
	
	@I18NMessage("Via Trace Events")
	String VIA_TRACE_EVENTS = "Labels.via-trace-events";
	
	@I18NMessage("Via SQL Server Audit Specifications")
	String VIA_SQL_SERVER_AUDIT_SPECIFICATIONS = "Labels.via-sql-server-audit-specifications";
	
	@I18NMessage("Via Extended Events")
	String VIA_EXTENDED_EVENTS = "Labels.via-extended-events";
	//5.5 Audit Logs End
	
	@I18NMessage("DDL")
	String REGULATION_CUSTOM_DB_EVENT_DDL = "SQLCM.Labels.regulation.custom.db.event.ddl";
	
	@I18NMessage("DML")
	String REGULATION_CUSTOM_DB_EVENT_DML = "SQLCM.Labels.regulation.custom.db.event.dml";
	
	@I18NMessage("Security changes")
	String REGULATION_CUSTOM_DB_EVENT_SECURITY = "SQLCM.Labels.regulation.custom.db.event.security";
	
	@I18NMessage("Select")
	String REGULATION_CUSTOM_DB_EVENT_SELECT = "SQLCM.Labels.regulation.custom.db.event.select";
	
	@I18NMessage("Administrative Actions")
	String REGULATION_CUSTOM_DB_EVENT_ADMINISTRATIVE_ACTIVITY = "SQLCM.Labels.regulation.custom.db.event.Administrative-activity";
	
	@I18NMessage("Sensitive Column")
	String REGULATION_CUSTOM_DB_EVENT_SENSITIVE_COLUMN = "SQLCM.Labels.regulation.custom.db.event.sensitive-cloumn";
	
	@I18NMessage("Before-After Data")
	String REGULATION_CUSTOM_DB_EVENT_BEFORE_AFTER_DATA = "SQLCM.Labels.regulation.custom.db.event.before-after-data";
	
	@I18NMessage("Privileged User")
	String REGULATION_CUSTOM_DB_EVENT_PRIVILEGED_USER = "SQLCM.Labels.regulation.custom.db.event.privileged-user";
	
	@I18NMessage("Save Dialog Option")
	String REGULATION_SAVE_DIALOG_OPTION = "SQLCM.Labels.regulation.save-dialog-option";
	
	@I18NMessage("Apply Regulation")
	String REGULATION_APPLY_REGULATION_LABEL = "SQLCM.Labels.regulation.apply-regulation-label";
	
	@I18NMessage("Apply Regulation and Save with Custom Name.")
	String REGULATION_APPLY_REGULATION_WITH_SAVE_LABEL = "SQLCM.Labels.regulation.apply-regulation-with-save-option";
	
	@I18NMessage("Regulation Guidelines Template Name:")
	String REGULATION_TEMPLATE_NAME = "SQLCM.Labels.regulation.template-name";
	
	@I18NMessage("An unexpected error occurred downloading the XML template file.")
	String REGULATION_FILE_EXPORT_ERROR = "SQLCM.Labels.regulation.file-error";
	
	@I18NMessage("<h3>Sensitive Tables and Columns Access</h3>"
			+ "<p>"
			+ "Select each database table(s) you want to audit for sensitive column access. All table columns will be audited."
			+ "</p>"
			+ "<p>"
			+ " For each database, enter the tables that you would like audited for Sensitive Column Access." 
			+ "</p>")
	String REGULATION_GUIDELINE_SENSITIVE_COLUMN_TIPS = "SQLCM.Html.regulation-guideline-sensitive-column.tips";
	
	@I18NMessage("<h3>Before After Data Change</h3>"
			+ "<p>"
			+ "Select the databases you want to audit for Before After Data change, and click configure."
			+ "</p>"
			+ "<p>"
			+ " For each database, enter the tables that you would like audited for Before After Data change." 
			+ "</p>")
	String REGULATION_GUIDELINE_BEFORE_AFTER_COLUMN_TIPS = "SQLCM.Html.regulation-guideline-before-after-column.tips";

	@I18NMessage("Since you have chosen to customize your regulation guidelines, we can not directly map your choices to our standard regulation guidelines.")
	String REGULATION_EMPTY_MESSAGE = "SQLCM.Label.regulation-empty-message";
	
	@I18NMessage("*Configuration can be set up via the SQL Compliance Manager Windows Console")
	String CONFIGURATION_EDIT_MESSAGE = "SQLCM.Label.configuration-edit-message";

}
