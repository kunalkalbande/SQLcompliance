package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMActivityLogs extends CMEntity {

	@JsonProperty("logId")
	private long logId;

	public long getLogId() {
		return logId;
	}

	public void setLogId(long logId) {
		this.logId = logId;
	}

	@JsonProperty("eventId")
	private long eventId;

	@JsonProperty("instanceName")
	private String instanceName;
	
	@JsonProperty("instanceId")
	private int instanceId;

	@JsonProperty("eventTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date eventTime;

	@JsonProperty("eventType")
	private String eventType;

	@JsonProperty("detail")
	private String detail;

	public CMActivityLogs() {
	}

	public long getEventId() {
		return eventId;
	}

	public void setEventId(long eventId) {
		this.eventId = eventId;
	}

	public String getInstanceName() {
		return instanceName;
	}

	public void setInstanceName(String instanceName) {
		this.instanceName = instanceName;
	}

	public int getInstanceId() {
		return instanceId;
	}

	public void setInstanceId(int instanceId) {
		this.instanceId = instanceId;
	}
	public String getEventType() {
		return eventType;
	}

	public void setEventType(String eventType) {
		this.eventType = eventType;
	}

	public String getDetail() {
		return detail;
	}

	public void setDetail(String detail) {
		this.detail = detail;
	}

	@Override
	public String toString() {
		return "CMActivityLogs{" + "logId=" + logId + ", eventId=" + eventId
				+ ", instanceName='" + instanceName + '\'' + ",instanceId='" + instanceId + '\'' + ", eventTime="
				+ eventTime + ", eventType='" + eventType + '\'' + ", detail='"
				+ detail + '\'' + '}';
	}

	public Date getEventTime() {
		return eventTime;
	}

	public void setEventTime(Date eventTime) {
		this.eventTime = eventTime;
	}
}
