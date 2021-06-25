package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditLoginCreationResponse  {
    @JsonProperty("TargetLoginName")
    String TargetLoginName;
    
    @JsonProperty("LoginName")
    String LoginName;

    @JsonProperty("HostName")
    String HostName;

    @JsonProperty("ApplicationName")
    String ApplicationName;

    @JsonProperty("StartTime")
    String StartTime;
        
    public CMAuditLoginCreationResponse() {
    }

	public String getTargetLoginName() {
		return TargetLoginName;
	}

	public void setTargetLoginName(String targetLoginName) {
		TargetLoginName = targetLoginName;
	}

	public String getLoginName() {
		return LoginName;
	}

	public void setLoginName(String loginName) {
		LoginName = loginName;
	}

	public String getHostName() {
		return HostName;
	}

	public void setHostName(String hostName) {
		HostName = hostName;
	}

	public String getApplicationName() {
		return ApplicationName;
	}

	public void setApplicationName(String applicationName) {
		ApplicationName = applicationName;
	}

	public String getStartTime() {
		return StartTime;
	}

	public void setStartTime(String startTime) {
		StartTime = startTime;
	}

	@Override
    public String toString() {
        return "CMAuditDMLResponse{" +
        	", EventType=" + TargetLoginName +
        	", LoginName=" + LoginName +
            ", StartTime=" + StartTime +            
            ", HostName=" + HostName +
            ", StartTime=" + StartTime +           
            '}';
    }
}
