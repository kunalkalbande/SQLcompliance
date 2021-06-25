package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditObjectActivityResponse  {
    @JsonProperty("ApplicationName")
    String ApplicationName;

    @JsonProperty("TargetObject")
    String TargetObject;

    @JsonProperty("DatabaseName")
    String DatabaseName;

    @JsonProperty("EventType")
    String EventType;
    
    @JsonProperty("HostName")
    String HostName;
    
    @JsonProperty("LoginName")
    String LoginName;

    @JsonProperty("StartTime")
    String StartTime;
    
    @JsonProperty("SqlText")
    String SqlText;
    
    @JsonProperty("Detail")
    String Detail;
    
    public CMAuditObjectActivityResponse() {
    }

	public String getApplicationName() {
		return ApplicationName;
	}

	public void setApplicationName(String applicationName) {
		ApplicationName = applicationName;
	}

	public String getTargetObject() {
		return TargetObject;
	}

	public void setTargetObject(String targetObject) {
		TargetObject = targetObject;
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

	public String getLoginName() {
		return LoginName;
	}

	public void setLoginName(String loginName) {
		LoginName = loginName;
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

	public String getDetail() {
		return Detail;
	}

	public void setDetail(String detail) {
		Detail = detail;
	}

	@Override
    public String toString() {
        return "CMAuditDMLResponse{" +
        	", ApplicationName=" + ApplicationName +
        	", Detail=" + Detail +
        	", TargetObject=" + TargetObject +
        	",DatabaseName=" + DatabaseName +
            ", LoginName=" + HostName +
            ",EventType=" + EventType +
            ", LoginName=" + LoginName +
            ", StartTime=" + StartTime +
            ", SqlText=" + SqlText +
            '}';
    }
}
