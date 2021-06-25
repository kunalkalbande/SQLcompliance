package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMChangeLogs extends CMEntity {

    @JsonProperty("logId")
    private long logId;

    @JsonProperty("eventId")
    private long eventId;

    @JsonProperty("eventTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date eventTime;

    @JsonProperty("logType")
    private String logType;
    
    @JsonProperty("instanceId")
	private int instanceId;

    @JsonProperty("logUser")
    private String logUser;

    @JsonProperty("logSqlServer")
	private String logSqlServer;

    @JsonProperty("logInfo")
    private String logInfo;

    public long getLogId() {
		return logId;
	}

	public void setLogId(long logId) {
		this.logId = logId;
	}

	public long getEventId() {
		return eventId;
	}

	public void setEventId(long eventId) {
		this.eventId = eventId;
	}

	public Date getEventTime() {
		return eventTime;
	}

	public void setEventTime(Date eventTime) {
		this.eventTime = eventTime;
	}
	
	public int getInstanceId() {
		return instanceId;
	}

	public void setInstanceId(int instanceId) {
		this.instanceId = instanceId;
	}

	public String getLogType() {
		return logType;
	}

	public void setLogType(String logType) {
		this.logType = logType;
	}

	public String getLogUser() {
		return logUser;
	}

	public void setLogUser(String logUser) {
		this.logUser = logUser;
	}

	public String getLogSqlServer() {
		return logSqlServer;
	}

	public void setLogSqlServer(String logSqlServer) {
		this.logSqlServer = logSqlServer;
	}

	public String getLogInfo() {
		return logInfo;
	}

	public void setLogInfo(String logInfo) {
		this.logInfo = logInfo;
	}

	public CMChangeLogs() {
    }

    @Override
    public String toString() {
        return "CMActivityLogs{" +
            "logId=" + logId +
            ", eventId=" + eventId + 
            ", eventTime=" + eventTime +
            ",instanceId='" + instanceId + '\'' + 
            ", logType='" + logType +   '\'' +
            ", logUser='" + logUser + '\'' +
            ", logSqlServer='" + logSqlServer + '\'' +
            ", logInfo='" + logInfo + '\'' +
            '}';
    }
}
