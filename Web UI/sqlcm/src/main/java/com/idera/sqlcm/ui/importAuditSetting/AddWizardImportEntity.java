/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.ui.importAuditSetting;

import java.util.List;
import java.util.Set;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.wizard.IWizardEntity;

public class AddWizardImportEntity extends CMEntity implements IWizardEntity {
	
	@JsonProperty("validFile")
	boolean validFile;
	
	@JsonProperty("dbDetails")
	List<String> dbDetails;
	
	@JsonProperty("privUserConfig")
	String privUserConfig;
	
	@JsonProperty("ServerLevelConfig")
	String ServerLevelConfig;

	@JsonProperty("Database")
	String Database;

	@JsonProperty("MatchDBNames")
	String MatchDBNames;

	@JsonProperty("DatabasePrivUser")
	String DatabasePrivUser;
	
	@JsonProperty("ServerDetails")
	List<ServerDetails> ServerDetails;
	
	@JsonProperty("DatabaseDetails")
	List<DatabaseDetails> DatabaseDetails;
	
	@JsonProperty("xmlData")
	String xmlData;
	
	@JsonProperty("auditedActivities")
    private CMAuditedActivities auditedActivities;
	
	@JsonProperty("serverAuditedActivities")
    private CMAuditedActivities serverAuditedActivities;

	@JsonProperty("userCheckServer")
	boolean userCheckServer;
	
	@JsonProperty("userCheckServerPrivilage")
	boolean userCheckServerPrivilage;

	@JsonProperty("userCheckDatabase")
	boolean userCheckDatabase;
	
	@JsonProperty("usercheckDatabasePrivilage")
	boolean usercheckDatabasePrivilage;
	
	@JsonProperty("overwriteSelection")
	boolean overwriteSelection;
	
	@JsonProperty("usermatchdbNameSelection")
	boolean usermatchdbNameSelection;
    
	@JsonProperty("userdbSelection")
	Set<DatabaseDetails> userdbSelection;
    
	@JsonProperty("userServerSelection")
	Set<ServerDetails> userServerSelection;
    
	@JsonProperty("userdbServerComboSelection")
	Set<TargetDatabaseDetails> userdbServerComboSelection;
	
	public boolean isValidFile() {
		return validFile;
	}
	public void setValidFile(boolean validFile) {
		this.validFile = validFile;
	}
	public String getXmlData() {
		return xmlData;
	}

	public void setXmlData(String xmlData) {
		this.xmlData = xmlData;
	}
	public CMAuditedActivities getAuditedActivities() {
		return auditedActivities;
	}
	public void setAuditedActivities(CMAuditedActivities auditedActivities) {
		this.auditedActivities = auditedActivities;
	}

	public CMAuditedActivities getServerAuditedActivities() {
		return serverAuditedActivities;
	}
	public void setServerAuditedActivities(CMAuditedActivities serverAuditedActivities) {
		this.serverAuditedActivities = serverAuditedActivities;
	}
	
	public boolean getUserCheckServer() {
		return userCheckServer;
	}

	public void setUserCheckServer(boolean userCheckServer) {
		this.userCheckServer = userCheckServer;
	}

	public boolean getUserCheckServerPrivilage() {
		return userCheckServerPrivilage;
	}

	public void setUserCheckServerPrivilage(boolean userCheckServerPrivilage) {
		this.userCheckServerPrivilage = userCheckServerPrivilage;
	}

	public boolean getUserCheckDatabase() {
		return userCheckDatabase;
	}

	public void setUserCheckDatabase(boolean userCheckDatabase) {
		this.userCheckDatabase = userCheckDatabase;
	}

	public boolean getUsercheckDatabasePrivilage() {
		return usercheckDatabasePrivilage;
	}

	public void setUsercheckDatabasePrivilage(boolean usercheckDatabasePrivilage) {
		this.usercheckDatabasePrivilage = usercheckDatabasePrivilage;
	}
	
	public boolean getUsermatchdbNameSelection() {
		return usermatchdbNameSelection;
	}

	public void setUsermatchdbNameSelection(boolean usermatchdbNameSelection) {
		this.usermatchdbNameSelection = usermatchdbNameSelection;
	}

	public Set<DatabaseDetails> getUserdbSelection() {
		return userdbSelection;
	}

	public void setUserdbSelection(Set<DatabaseDetails> userdbSelection) {
		this.userdbSelection = userdbSelection;
	}

	public Set<ServerDetails> getUserServerSelection() {
		return userServerSelection;
	}

	public void setUserServerSelection(Set<ServerDetails> userServerSelection) {
		this.userServerSelection = userServerSelection;
	}

	public Set<TargetDatabaseDetails> getUserdbServerComboSelection() {
		return userdbServerComboSelection;
	}

	public void setUserdbServerComboSelection(
			Set<TargetDatabaseDetails> userdbServerComboSelection) {
		this.userdbServerComboSelection = userdbServerComboSelection;
	}

	public boolean isOverwriteSelection() {
		return overwriteSelection;
	}

	public void setOverwriteSelection(boolean overwriteSelection) {
		this.overwriteSelection = overwriteSelection;
	}
	
	public List<String> getDbDetails() {
		return dbDetails;
	}

	public void setDbDetails(List<String> dbDetails) {
		this.dbDetails = dbDetails;
	}

	public List<DatabaseDetails> getDatabaseDetails() {
		return DatabaseDetails;
	}

	public void setDatabaseDetails(List<DatabaseDetails> databaseDetails) {
		DatabaseDetails = databaseDetails;
	}
	
	public List<ServerDetails> getServerDetails() {
		return ServerDetails;
	}

	public void setServerDetails(List<ServerDetails> serverDetails) {
		ServerDetails = serverDetails;
	}

	public List<String> getDBDetails() {
		return dbDetails;
	}

	public void setDBDetails(List<String> dbDetails) {
		this.dbDetails = dbDetails;
	}
	
	public String getPrivUserConfig() {
		return privUserConfig;
	}

	public void setPrivUserConfig(String privUserConfig) {
		this.privUserConfig = privUserConfig;
	}
	
	public String getServerLevelConfig() {
		return ServerLevelConfig;
	}

	public void setServerLevelConfig(String ServerLevelConfig) {
		this.ServerLevelConfig = ServerLevelConfig;
	}
	
	public String getDatabase() {
		return Database;
	}

	public void setDatabase(String Database) {
		this.Database = Database;
	}
	
	public String getMatchDBNames() {
		return MatchDBNames;
	}

	public void setMatchDBNames(String MatchDBNames) {
		this.MatchDBNames = MatchDBNames;
	}
	
	public String getDatabasePrivUser() {
		return DatabasePrivUser;
	}

	public void setDatabasePrivUser(String DatabasePrivUser) {
		this.DatabasePrivUser = DatabasePrivUser;
	}
}

/***End SQLCm 5.4***/