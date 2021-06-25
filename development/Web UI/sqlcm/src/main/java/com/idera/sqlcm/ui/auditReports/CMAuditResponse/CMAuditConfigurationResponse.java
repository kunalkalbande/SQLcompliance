package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditConfigurationResponse  {
	@JsonProperty("srvId")
	int srvId;
	/*int flag = 0;

	public int getFlag() {
		return flag;
	}
	public void setFlag(int flag) {
		this.flag = flag;
	}*/
	
	@JsonProperty("instance")
	String instance;
	
	@JsonProperty("isDeployed")
	int isDeployed;

	@JsonProperty("eventDatabase")
	String eventDatabase;
	
	@JsonProperty("agentVersion")
	String agentVersion;

	@JsonProperty("auditAdmin")
	int auditAdmin;
	
	@JsonProperty("auditCaptureSql")
	int auditCaptureSql;

	@JsonProperty("auditCaptureSqlXE")
	int auditCaptureSqlXE;
	
	@JsonProperty("auditDBCC")
	int auditDBCC;

	@JsonProperty("auditDDL")
	int auditDDL;
	
	@JsonProperty("auditDML")
	int auditDML;

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

	@JsonProperty("auditUserUDE")
	int auditUserUDE;
	
	@JsonProperty("isAuditLogEnabled")
	int isAuditLogEnabled;
	
	@JsonProperty("auditTrustedUsersList")
	String auditTrustedUsersList;

	@JsonProperty("DatabasesList")
	List<CMAuditConfigurationDatabaseResponse> DatabaseConfigList;
	
/*
	public int getServerEvents() {
		return ServerEvents;
	}

	public void setServerEvents(int serverEvents) {
		ServerEvents = serverEvents;
	}

	public int getStatus() {
		return Status;
	}

	public void setStatus(int status) {
		Status = status;
	}
*/
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
	public String getEventDatabase() {
		return eventDatabase;
	}
	public void setEventDatabase(String eventDatabase) {
		this.eventDatabase = eventDatabase;
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
	public String getAuditTrustedUsersList() {
		return auditTrustedUsersList;
	}
	public void setAuditTrustedUsersList(String auditTrustedUsersList) {
		this.auditTrustedUsersList = auditTrustedUsersList;
	}
	public List<CMAuditConfigurationDatabaseResponse> getDatabaseConfigList() {
		return DatabaseConfigList;
	}
	public void setDatabaseConfigList(List<CMAuditConfigurationDatabaseResponse> databaseConfigList) {
		DatabaseConfigList = databaseConfigList;
	}

}


