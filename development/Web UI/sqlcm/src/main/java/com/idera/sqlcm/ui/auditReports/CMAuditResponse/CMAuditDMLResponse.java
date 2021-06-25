package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditDMLResponse  {
    @JsonProperty("EventType")
    String EventType;

    @JsonProperty("StartTime")
    String StartTime;
    
    @JsonProperty("LoginName")
    String LoginName;

    @JsonProperty("DatabaseName")
    String DatabaseName;

    @JsonProperty("columnName")
    String columnName;
    
    @JsonProperty("beforeValue")
    String beforeValue;
    
    @JsonProperty("table")
    String table;
        
    @JsonProperty("afterValue")
    String afterValue;

    @JsonProperty("primaryKeys")
    String primaryKeys;
    
    public CMAuditDMLResponse() {
    }

	public String getEventType() {
		return EventType;
	}

	public void setEventType(String eventType) {
		EventType = eventType;
	}

	public String getStartTime() {
		return StartTime;
	}

	public void setStartTime(String startTime) {
		StartTime = startTime;
	}

	public String getLoginName() {
		return LoginName;
	}

	public void setLoginName(String loginName) {
		LoginName = loginName;
	}

	public String getDatabaseName() {
		return DatabaseName;
	}

	public void setDatabaseName(String databaseName) {
		DatabaseName = databaseName;
	}

	public String getColumnName() {
		return columnName;
	}

	public void setColumnName(String columnName) {
		this.columnName = columnName;
	}

	public String getBeforeValue() {
		return beforeValue;
	}

	public void setBeforeValue(String beforeValue) {
		this.beforeValue = beforeValue;
	}

	public String getAfterValue() {
		return afterValue;
	}

	public void setAfterValue(String afterValue) {
		this.afterValue = afterValue;
	}

	public String getPrimaryKeys() {
		return primaryKeys;
	}

	public void setPrimaryKeys(String primaryKeys) {
		this.primaryKeys = primaryKeys;
	}

	public String getTable() {
		return table;
	}

	public void setTable(String table) {
		this.table = table;
	}

	@Override
    public String toString() {
        return "CMAuditDMLResponse{" +
        	", EventType=" + EventType +
            ", StartTime=" + StartTime +
            ", LoginName=" + LoginName +
            ", DatabaseName=" + DatabaseName +
            ", columnName=" + columnName +
            ", table=" + table +
            ", beforeValue=" + beforeValue +
            ", afterValue=" + afterValue +
            ", primaryKeys=" + primaryKeys +
            '}';
    }
}
