package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditPermissionDeniedActivityResponse  {
    @JsonProperty("ApplicationName")
    String ApplicationName;

    @JsonProperty("DatabaseName")
    String DatabaseName;

    @JsonProperty("EventType")
    String EventType;
    
    @JsonProperty("HostName")
    String HostName;

    @JsonProperty("Details")
    String Details;
    
    @JsonProperty("LoginName")
    String LoginName;
    
    @JsonProperty("TargetObject")
    String TargetObject;

    @JsonProperty("StartTime")
    String StartTime;
    
    @JsonProperty("SqlText")
    String SqlText;
        
    public CMAuditPermissionDeniedActivityResponse() {
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

	public String getHostName() {
		return HostName;
	}

	public void setHostName(String hostName) {
		HostName = hostName;
	}

	public String getDetails() {
		return Details;
	}

	public void setDetails(String details) {
		Details = details;
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

	@Override
    public String toString() {
        return "CMAuditDMLResponse{" +
        	", ApplicationName=" + ApplicationName +
        	",DatabaseName=" + DatabaseName +
        	",EventType=" + EventType +
            ", LoginName=" + HostName +
        	",Details="+ Details +            
            ", LoginName=" + LoginName +	
        	", TargetObject=" + TargetObject +
            ", StartTime=" + StartTime +	
            ", SqlText=" + SqlText +	
            '}';
    }
}
