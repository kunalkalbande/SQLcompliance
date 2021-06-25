package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import org.zkoss.zul.ListModelList;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyAppNameViewModel.App;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyLoginViewModel.Login;

public class RegulationSettings {

	@JsonProperty("sqlServer")
    private boolean sqlServer;
    
    public boolean isSqlServer() {
		return sqlServer;
	}

	public void setSqlServer(boolean sqlServer) {
		this.sqlServer = sqlServer;
	}

	public boolean isTableName() {
		return tableName;
	}

	public void setTableName(boolean tableName) {
		this.tableName = tableName;
	}

	public boolean isDatabaseName() {
		return databaseName;
	}

	public void setDatabaseName(boolean databaseName) {
		this.databaseName = databaseName;
	}

	public boolean isColumnName() {
		return columnName;
	}

	public void setColumnName(boolean columnName) {
		this.columnName = columnName;
	}

	public String getDatabase() {
		return database;
	}

	public void setDatabase(String database) {
		this.database = database;
	}

	public String getInstances() {
		return Instances;
	}

	public void setInstances(String instances) {
		Instances = instances;
	}

	public String getDbName() {
		return dbName;
	}

	public void setDbName(String dbName) {
		this.dbName = dbName;
	}

	public String getTableNameData() {
		return tableNameData;
	}

	public void setTableNameData(String tableNameData) {
		this.tableNameData = tableNameData;
	}

	public String getColumnNameData() {
		return columnNameData;
	}

	public void setColumnNameData(String columnNameData) {
		this.columnNameData = columnNameData;
	}

	@JsonProperty("tableName")
    private boolean tableName;
    
    @JsonProperty("databaseName")
    private boolean databaseName;
    
    @JsonProperty("columnName")
    private boolean columnName;
       
    @JsonProperty("database")
    private String database;
    
    @JsonProperty("instances")
	public String Instances;
    
    @JsonProperty("dbName")
   	public String dbName;
    
    @JsonProperty("tableNameData")
   	public String tableNameData;
   	
    @JsonProperty("columnNameData")
   	public String columnNameData;
    
    
    @JsonProperty("matchString")
    public String matchString;
    
    @JsonProperty("loginMatchString")
    private String loginMatchString;
    
    public String getLoginMatchString() {
		return loginMatchString;
	}

	public void setLoginMatchString(String loginMatchString) {
		this.loginMatchString = loginMatchString;
	}

	@JsonProperty("ruleValue")
    public int ruleValue;

	public int getRuleValue() {
		return ruleValue;
	}

	public void setRuleValue(int ruleValue) {
		this.ruleValue = ruleValue;
	}

	public String getMatchString() {
		return matchString;
	}

	public void setMatchString(String matchString) {
		this.matchString = matchString;
	}

	@JsonProperty("applicationName")
	private boolean applicationName;

	public boolean isApplicationName() {
		return applicationName;
	}

	public void setApplicationName(boolean applicationName) {
		this.applicationName = applicationName;
	}

	@JsonProperty("rowCountWithTimeInterval")
	private boolean rowCountWithTimeInterval;

	public boolean isRowCountWithTimeInterval() {
		return rowCountWithTimeInterval;
	}

	public void setRowCountWithTimeInterval(boolean rowCountWithTimeInterval) {
		this.rowCountWithTimeInterval = rowCountWithTimeInterval;
	}

	@JsonProperty("loginName")
	private boolean loginName;

	public boolean isLoginName() {
		return loginName;
	}

	public void setLoginName(boolean loginName) {
		this.loginName = loginName;
	}

	@JsonProperty("appMatchString")
	private String appMatchString;

	public String getAppMatchString() {
		return appMatchString;
	}

	public void setAppMatchString(String appMatchString) {
		this.appMatchString = appMatchString;
	}

	@JsonProperty("appFieldId")
	private int appFieldId;

	public int getAppFieldId() {
		return appFieldId;
	}

	public void setAppFieldId(int appFieldId) {
		this.appFieldId = appFieldId;
	}

	@JsonProperty("rowCountMatchString")
	public String rowCountMatchString;

	public String getRowCountMatchString() {
		return rowCountMatchString;
	}

	public void setRowCountMatchString(String rowCountMatchString) {
		this.rowCountMatchString = rowCountMatchString;
	}

	@JsonProperty("rowCountFieldId")
	public int rowCountFieldId;

	public int getRowCountFieldId() {
		return rowCountFieldId;
	}

	public void setRowCountFieldId(int rowCountFieldId) {
		this.rowCountFieldId = rowCountFieldId;
	}

	@JsonProperty("loginFieldId")
	private int loginFieldId;

	public int getLoginFieldId() {
		return loginFieldId;
	}

	public void setLoginFieldId(int loginFieldId) {
		this.loginFieldId = loginFieldId;
	}

	// TODO Auto-generated method stub
	@JsonProperty("appNameList")
	public ListModelList<App> appNameList = new ListModelList<>();

	public ListModelList<App> getAppNameList() {
		return appNameList;
	}

	public void setAppNameList(ListModelList<App> appNameList) {
		this.appNameList = appNameList;
	}

	@JsonProperty("loginNameList")
	public ListModelList<Login> loginNameList = new ListModelList<>();

	public ListModelList<Login> getLoginNameList() {
		return loginNameList;
	}

	public void setLoginNameList(ListModelList<Login> loginNameList) {
		this.loginNameList = loginNameList;
	}

}