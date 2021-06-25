package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditRowCountResponse  {
    @JsonProperty("ApplicationName")
    String ApplicationName;
    
    @JsonProperty("ServerName")
    String ServerName;

    @JsonProperty("DatabaseName")
    String DatabaseName;

    @JsonProperty("EventType")
    String EventType;
    
    @JsonProperty("LoginName")
    String LoginName;
    
    @JsonProperty("TargetObject")
    String TargetObject;

    @JsonProperty("StartTime")
    String StartTime;

    @JsonProperty("SqlText")
    String SqlText;

    @JsonProperty("RoleName")
    String RoleName;

    @JsonProperty("Spid")
    String Spid;

    @JsonProperty("RowCounts")
    String RowCounts;

    @JsonProperty("ColumnName")
    String ColumnName;
        
    public CMAuditRowCountResponse() {
    }
	
	public String getApplicationName() {
		return ApplicationName;
	}

	public void setApplicationName(String applicationName) {
		ApplicationName = applicationName;
	}

	public String getDatabaseName() {
		return DatabaseName;
	}

	public void setDatabaseName(String databaseName) {
		DatabaseName = databaseName;
	}

	public String getEventType() {
		return EventType;
	}

	public void setEventType(String eventType) {
		EventType = eventType;
	}

	public String getLoginName() {
		return LoginName;
	}

	public void setLoginName(String loginName) {
		LoginName = loginName;
	}

	public String getTargetObject() {
		return TargetObject;
	}

	public void setTargetObject(String targetObject) {
		TargetObject = targetObject;
	}

	public String getStartTime() {
		return StartTime;
	}

	public void setStartTime(String startTime) {
		StartTime = startTime;
	}

	public String getSqlText() {
		return SqlText;
	}

	public void setSqlText(String sqlText) {
		SqlText = sqlText;
	}

	public String getRoleName() {
		return RoleName;
	}

	public void setRoleName(String roleName) {
		RoleName = roleName;
	}

	public String getSpid() {
		return Spid;
	}

	public void setSpid(String spid) {
		Spid = spid;
	}

	public String getRowCounts() {
		return RowCounts;
	}

	public void setRowCounts(String rowCounts) {
		RowCounts = rowCounts;
	}

	public String getColumnName() {
		return ColumnName;
	}

	public void setColumnName(String columnName) {
		ColumnName = columnName;
	}

	public String getServerName() {
		return ServerName;
	}

	public void setServerName(String serverName) {
		ServerName = serverName;
	}

	@Override
    public String toString() {
        return "CMAuditDMLResponse{" +
        	", ServerName=" + ServerName +
        	", ApplicationName=" + ApplicationName +
        	",DatabaseName=" + DatabaseName +
        	",EventType=" + EventType +
            ", Spid=" + Spid +
        	",RoleName="+ RoleName +
        	",ColumnName="+ ColumnName +            
            ", LoginName=" + LoginName +	
        	", TargetObject=" + TargetObject +
            ", StartTime=" + StartTime +
            ", RowCounts=" + RowCounts +
			", SqlText=" + SqlText +	
            '}';
    }
}
