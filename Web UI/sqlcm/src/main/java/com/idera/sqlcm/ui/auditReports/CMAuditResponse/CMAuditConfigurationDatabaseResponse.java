package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditConfigurationDatabaseResponse  {

	@JsonProperty("flag")
	int flag;
	
	@JsonProperty("srvId")
	int srvId;

	@JsonProperty("instance")
	String instance;
	
	@JsonProperty("isDeployed")
	int isDeployed;
	
	@JsonProperty("instanceServer")
	String instanceServer	;
	
	@JsonProperty("eventDatabase")
	String eventDatabase	;
	
	@JsonProperty("sqlDatabaseId")
	int sqlDatabaseId	;
	
	@JsonProperty("name")
	String name	;
	
	@JsonProperty("agentVersion")
	String agentVersion;

	@JsonProperty("auditAdmin")
	int auditAdmin;
	
	@JsonProperty("auditCaptureSQL")
	int auditCaptureSql;

	@JsonProperty("auditCaptureSQLXE")
	int auditCaptureSqlXE;
	
	@JsonProperty("auditDBCC")
	int auditDBCC;

	@JsonProperty("auditDDL")
	int auditDDL;
	
	@JsonProperty("auditDML")
	int auditDML;
	
	@JsonProperty("auditDMLAll")
	int auditDMLAll;
	
	@JsonProperty("auditDMLOther")
	int auditDMLOther;

	@JsonProperty("auditExceptions")
	int auditExceptions;
	
	@JsonProperty("auditFailedLogins")
	int auditFailedLogins;

	@JsonProperty("auditFailures")
	int auditFailures;
	
	@JsonProperty("auditLogins")
	int auditLogins;

	@JsonProperty("auditLogouts")
	int auditLogouts;
	
	@JsonProperty("auditPrivUsersList")
	String auditPrivUsersList;
	
	@JsonProperty("auditSecurity")
	int auditSecurity;

	@JsonProperty("auditSELECT")
	int auditSELECT;
	
	@JsonProperty("auditSystemEvents")
	int auditSystemEvents;

	@JsonProperty("auditTrace")
	int auditTrace;

	@JsonProperty("auditUDE")
	int auditUDE;
	
	@JsonProperty("auditUserAdmin")
	int auditUserAdmin;

	@JsonProperty("auditUserAll")
	int auditUserAll;
	
	@JsonProperty("auditUserCaptureDDL")
	int auditUserCaptureDDL;

	@JsonProperty("auditUserCaptureSQL")
	int auditUserCaptureSQL;
	
	@JsonProperty("auditUserCaptureTrans")
	int auditUserCaptureTrans;

	@JsonProperty("auditUserDDL")
	int auditUserDDL;
	
	@JsonProperty("auditUserDML")
	int auditUserDML;

	@JsonProperty("auditUserExceptions")
	int auditUserExceptions;
	
	@JsonProperty("auditUserExtendedEvents")
	int auditUserExtendedEvents;

	@JsonProperty("auditUserFailedLogins")
	int auditUserFailedLogins;
	
	@JsonProperty("auditUserFailures")
	int auditUserFailures;

	@JsonProperty("auditUserLogins")
	int auditUserLogins;
	
	@JsonProperty("auditUserLogouts")
	int auditUserLogouts;

	@JsonProperty("auditUsers")
	int auditUsers;
	
	@JsonProperty("auditUserSecurity")
	int auditUserSecurity;

	@JsonProperty("auditUserSELECT")
	int auditUserSELECT;
	
	@JsonProperty("auditUsersList")
	String auditUsersList;
	
	@JsonProperty("auditUserTables")
	int auditUserTables;
	
	@JsonProperty("auditUserUDE")
	int auditUserUDE;

	@JsonProperty("isAuditLogEnabled")
	int isAuditLogEnabled;
	
	@JsonProperty("auditCaptureTrans")
	int auditCaptureTrans;
	
	@JsonProperty("auditCaptureDDL")
	int auditCaptureDDL;
	
	@JsonProperty("auditSystemTables")
	int auditSystemTables;
	
	@JsonProperty("auditStoredProcedures")
	int auditStoredProcedures;
	
	@JsonProperty("auditBroker")
	int auditBroker;
	
	@JsonProperty("auditDataChanges")
	int auditDataChanges;
	
	@JsonProperty("auditSensitiveColumns")
	int auditSensitiveColumns;
	
	@JsonProperty("auditTrustedUsersList")
	String auditTrustedUsersList;
	
	public int getFlag() {
		return flag;
	}
	public void setFlag(int flag) {
		this.flag = flag;
	}
	public int getSrvId() {
		return srvId;
	}
	public void setSrvId(int srvId) {
		this.srvId = srvId;
	}
	public String getInstance() {
		return instance;
	}
	public void setInstance(String instance) {
		this.instance = instance;
	}
	public int getIsDeployed() {
		return isDeployed;
	}
	public void setIsDeployed(int isDeployed) {
		this.isDeployed = isDeployed;
	}
	public String getInstanceServer() {
		return instanceServer;
	}
	public void setInstanceServer(String instanceServer) {
		this.instanceServer = instanceServer;
	}
	public String getEventDatabase() {
		return eventDatabase;
	}
	public void setEventDatabase(String eventDatabase) {
		this.eventDatabase = eventDatabase;
	}
	public int getSqlDatabaseId() {
		return sqlDatabaseId;
	}
	public void setSqlDatabaseId(int sqlDatabaseId) {
		this.sqlDatabaseId = sqlDatabaseId;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public String getAgentVersion() {
		return agentVersion;
	}
	public void setAgentVersion(String agentVersion) {
		this.agentVersion = agentVersion;
	}
	public int getAuditAdmin() {
		return auditAdmin;
	}
	public void setAuditAdmin(int auditAdmin) {
		this.auditAdmin = auditAdmin;
	}
	public int getAuditCaptureSql() {
		return auditCaptureSql;
	}
	public void setAuditCaptureSql(int auditCaptureSql) {
		this.auditCaptureSql = auditCaptureSql;
	}
	public int getAuditCaptureSqlXE() {
		return auditCaptureSqlXE;
	}
	public void setAuditCaptureSqlXE(int auditCaptureSqlXE) {
		this.auditCaptureSqlXE = auditCaptureSqlXE;
	}
	public int getAuditDBCC() {
		return auditDBCC;
	}
	public void setAuditDBCC(int auditDBCC) {
		this.auditDBCC = auditDBCC;
	}
	public int getAuditDDL() {
		return auditDDL;
	}
	public void setAuditDDL(int auditDDL) {
		this.auditDDL = auditDDL;
	}
	public int getAuditDML() {
		return auditDML;
	}
	public void setAuditDML(int auditDML) {
		this.auditDML = auditDML;
	}
	public int getAuditDMLAll() {
		return auditDMLAll;
	}
	public void setAuditDMLAll(int auditDMLAll) {
		this.auditDMLAll = auditDMLAll;
	}
	public int getAuditDMLOther() {
		return auditDMLOther;
	}
	public void setAuditDMLOther(int auditDMLOther) {
		this.auditDMLOther = auditDMLOther;
	}
	public int getAuditExceptions() {
		return auditExceptions;
	}
	public void setAuditExceptions(int auditExceptions) {
		this.auditExceptions = auditExceptions;
	}
	public int getAuditFailedLogins() {
		return auditFailedLogins;
	}
	public void setAuditFailedLogins(int auditFailedLogins) {
		this.auditFailedLogins = auditFailedLogins;
	}
	public int getAuditFailures() {
		return auditFailures;
	}
	public void setAuditFailures(int auditFailures) {
		this.auditFailures = auditFailures;
	}
	public int getAuditLogins() {
		return auditLogins;
	}
	public void setAuditLogins(int auditLogins) {
		this.auditLogins = auditLogins;
	}
	public int getAuditLogouts() {
		return auditLogouts;
	}
	public void setAuditLogouts(int auditLogouts) {
		this.auditLogouts = auditLogouts;
	}
	public String getAuditPrivUsersList() {
		return auditPrivUsersList;
	}
	public void setAuditPrivUsersList(String auditPrivUsersList) {
		this.auditPrivUsersList = auditPrivUsersList;
	}
	public int getAuditSecurity() {
		return auditSecurity;
	}
	public void setAuditSecurity(int auditSecurity) {
		this.auditSecurity = auditSecurity;
	}
	public int getAuditSELECT() {
		return auditSELECT;
	}
	public void setAuditSELECT(int auditSELECT) {
		this.auditSELECT = auditSELECT;
	}
	public int getAuditSystemEvents() {
		return auditSystemEvents;
	}
	public void setAuditSystemEvents(int auditSystemEvents) {
		this.auditSystemEvents = auditSystemEvents;
	}
	public int getAuditTrace() {
		return auditTrace;
	}
	public void setAuditTrace(int auditTrace) {
		this.auditTrace = auditTrace;
	}
	public int getAuditUDE() {
		return auditUDE;
	}
	public void setAuditUDE(int auditUDE) {
		this.auditUDE = auditUDE;
	}
	public int getAuditUserAdmin() {
		return auditUserAdmin;
	}
	public void setAuditUserAdmin(int auditUserAdmin) {
		this.auditUserAdmin = auditUserAdmin;
	}
	public int getAuditUserAll() {
		return auditUserAll;
	}
	public void setAuditUserAll(int auditUserAll) {
		this.auditUserAll = auditUserAll;
	}
	public int getAuditUserCaptureDDL() {
		return auditUserCaptureDDL;
	}
	public void setAuditUserCaptureDDL(int auditUserCaptureDDL) {
		this.auditUserCaptureDDL = auditUserCaptureDDL;
	}
	public int getAuditUserCaptureSQL() {
		return auditUserCaptureSQL;
	}
	public void setAuditUserCaptureSQL(int auditUserCaptureSQL) {
		this.auditUserCaptureSQL = auditUserCaptureSQL;
	}
	public int getAuditUserCaptureTrans() {
		return auditUserCaptureTrans;
	}
	public void setAuditUserCaptureTrans(int auditUserCaptureTrans) {
		this.auditUserCaptureTrans = auditUserCaptureTrans;
	}
	public int getAuditUserDDL() {
		return auditUserDDL;
	}
	public void setAuditUserDDL(int auditUserDDL) {
		this.auditUserDDL = auditUserDDL;
	}
	public int getAuditUserDML() {
		return auditUserDML;
	}
	public void setAuditUserDML(int auditUserDML) {
		this.auditUserDML = auditUserDML;
	}
	public int getAuditUserExceptions() {
		return auditUserExceptions;
	}
	public void setAuditUserExceptions(int auditUserExceptions) {
		this.auditUserExceptions = auditUserExceptions;
	}
	public int getAuditUserExtendedEvents() {
		return auditUserExtendedEvents;
	}
	public void setAuditUserExtendedEvents(int auditUserExtendedEvents) {
		this.auditUserExtendedEvents = auditUserExtendedEvents;
	}
	public int getAuditUserFailedLogins() {
		return auditUserFailedLogins;
	}
	public void setAuditUserFailedLogins(int auditUserFailedLogins) {
		this.auditUserFailedLogins = auditUserFailedLogins;
	}
	public int getAuditUserFailures() {
		return auditUserFailures;
	}
	public void setAuditUserFailures(int auditUserFailures) {
		this.auditUserFailures = auditUserFailures;
	}
	public int getAuditUserLogins() {
		return auditUserLogins;
	}
	public void setAuditUserLogins(int auditUserLogins) {
		this.auditUserLogins = auditUserLogins;
	}
	public int getAuditUserLogouts() {
		return auditUserLogouts;
	}
	public void setAuditUserLogouts(int auditUserLogouts) {
		this.auditUserLogouts = auditUserLogouts;
	}
	public int getAuditUsers() {
		return auditUsers;
	}
	public void setAuditUsers(int auditUsers) {
		this.auditUsers = auditUsers;
	}
	public int getAuditUserSecurity() {
		return auditUserSecurity;
	}
	public void setAuditUserSecurity(int auditUserSecurity) {
		this.auditUserSecurity = auditUserSecurity;
	}
	public int getAuditUserSELECT() {
		return auditUserSELECT;
	}
	public void setAuditUserSELECT(int auditUserSELECT) {
		this.auditUserSELECT = auditUserSELECT;
	}
	public String getAuditUsersList() {
		return auditUsersList;
	}
	public void setAuditUsersList(String auditUsersList) {
		this.auditUsersList = auditUsersList;
	}
	public int getAuditUserTables() {
		return auditUserTables;
	}
	public void setAuditUserTables(int auditUserTables) {
		this.auditUserTables = auditUserTables;
	}
	public int getAuditUserUDE() {
		return auditUserUDE;
	}
	public void setAuditUserUDE(int auditUserUDE) {
		this.auditUserUDE = auditUserUDE;
	}
	public int getIsAuditLogEnabled() {
		return isAuditLogEnabled;
	}
	public void setIsAuditLogEnabled(int isAuditLogEnabled) {
		this.isAuditLogEnabled = isAuditLogEnabled;
	}
	public int getAuditCaptureTrans() {
		return auditCaptureTrans;
	}
	public void setAuditCaptureTrans(int auditCaptureTrans) {
		this.auditCaptureTrans = auditCaptureTrans;
	}
	public int getAuditCaptureDDL() {
		return auditCaptureDDL;
	}
	public void setAuditCaptureDDL(int auditCaptureDDL) {
		this.auditCaptureDDL = auditCaptureDDL;
	}
	public int getAuditSystemTables() {
		return auditSystemTables;
	}
	public void setAuditSystemTables(int auditSystemTables) {
		this.auditSystemTables = auditSystemTables;
	}
	public int getAuditStoredProcedures() {
		return auditStoredProcedures;
	}
	public void setAuditStoredProcedures(int auditStoredProcedures) {
		this.auditStoredProcedures = auditStoredProcedures;
	}
	public int getAuditBroker() {
		return auditBroker;
	}
	public void setAuditBroker(int auditBroker) {
		this.auditBroker = auditBroker;
	}
	public int getAuditDataChanges() {
		return auditDataChanges;
	}
	public void setAuditDataChanges(int auditDataChanges) {
		this.auditDataChanges = auditDataChanges;
	}
	public int getAuditSensitiveColumns() {
		return auditSensitiveColumns;
	}
	public void setAuditSensitiveColumns(int auditSensitiveColumns) {
		this.auditSensitiveColumns = auditSensitiveColumns;
	}
	public String getAuditTrustedUsersList() {
		return auditTrustedUsersList;
	}
	public void setAuditTrustedUsersList(String auditTrustedUsersList) {
		this.auditTrustedUsersList = auditTrustedUsersList;
	}

}
