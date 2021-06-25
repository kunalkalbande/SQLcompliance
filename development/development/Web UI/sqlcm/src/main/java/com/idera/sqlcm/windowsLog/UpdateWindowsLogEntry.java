package com.idera.sqlcm.windowsLog;


import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class UpdateWindowsLogEntry {


    @JsonProperty("logTYpe")
    String logType;
    
    public String getLogType() {
		return logType;
	}

	public void setLogType(String logType) {
		this.logType = logType;
	}

	@Override
    public String toString() {
        return "UpdateSMTPConfiguration{" +
            "logType=" + logType +
            '}';
    }
}
