package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditApplicationResponse  {
    @JsonProperty("ApplicationName")
    String ApplicationName;

    @JsonProperty("Details")
    String Details;

    @JsonProperty("DatabaseName")
    String DatabaseName;

    @JsonProperty("eventType")
    String eventType;

    @JsonProperty("SqlText")
    String SqlText;
    
    @JsonProperty("HostName")
    String HostName;
    
    @JsonProperty("LoginName")
    String LoginName;
    
    @JsonProperty("TargetObject")
    String TargetObject;
    
    @JsonProperty("StartTime")
    String StartTime;

    public CMAuditApplicationResponse() {
    }

	public String getApplicationName() {
		return ApplicationName;
	}

	public void setApplicationName(String applicationName) {
		ApplicationName = applicationName;
	}

	public String getDetails() {
		return Details;
	}

	public void setDetails(String details) {
		Details = details;
	}

	public String getDatabaseName() {
		return DatabaseName;
	}

	public void setDatabaseName(String databaseName) {
		DatabaseName = databaseName;
	}

	public String getEventType() {
		return eventType;
	}

	public void setEventType(String eventType) {
		this.eventType = eventType;
	}

	public String getSqlText() {
		return SqlText;
	}

	public void setSqlText(String sqlText) {
		SqlText = sqlText;
	}

	public String getHostName() {
		return HostName;
	}

	public void setHostName(String hostName) {
		HostName = hostName;
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

	@Override
    public String toString() {
        return "CMAuditApplicationResponse{" +
            "ApplicationName=" + ApplicationName +
            ", Details=" + Details +
            ", DatabaseName=" + DatabaseName +
            ", eventType=" + eventType +
            ", SqlText=" + SqlText +
            ", HostName=" + HostName +
            ", LoginName=" + LoginName +
            ", TargetObject=" + TargetObject +
            ", StartTime=" + StartTime +
            '}';
    }
}
