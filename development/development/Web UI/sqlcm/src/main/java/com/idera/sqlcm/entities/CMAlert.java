package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAlert extends CMEntity {

    @JsonProperty("instanceId")
    private long instanceId;

    @JsonProperty("instanceName")
    private String instanceName;

    @JsonProperty("time")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date time;

    @JsonProperty("alertType")
    private int alertType;

    @JsonProperty("alertLevel")
    private int alertLevel;

    @JsonProperty("sourceRule")
	private String sourceRule;

    @JsonProperty("alertEventId")
    private long alertEventId;

    @JsonProperty("eventType")
    private long eventType;

    @JsonProperty("detail")
    private String detail;

    @JsonProperty("event")
    private String event;

    @JsonProperty("eventTypeName")
    private String eventTypeName;

    public CMAlert() {
    }

    public long getInstanceId() {
        return instanceId;
    }

    public void setInstanceId(long instanceId) {
        this.instanceId = instanceId;
    }

    public String getInstanceName() {
        return instanceName;
    }

    public void setInstanceName(String instanceName) {
        this.instanceName = instanceName;
    }

    public Date getTime() {
        return time;
    }

    public void setTime(Date time) {
        this.time = time;
    }

    public int getAlertType() {
        return alertType;
    }

    public void setAlertType(int alertType) {
        this.alertType = alertType;
    }

    public int getAlertLevel() {
        return alertLevel;
    }

    public void setAlertLevel(int alertLevel) {
        this.alertLevel = alertLevel;
    }

    public String getSourceRule() {
        return sourceRule;
    }

    public void setSourceRule(String sourceRule) {
        this.sourceRule = sourceRule;
    }

    public long getAlertEventId() {
        return alertEventId;
    }

    public void setAlertEventId(long alertEventId) {
        this.alertEventId = alertEventId;
    }

    public long getEventType() {
        return eventType;
    }

    public void setEventType(long eventType) {
        this.eventType = eventType;
    }

    public String getDetail() {
        return detail;
    }

    public void setDetail(String detail) {
        this.detail = detail;
    }

    public String getEvent() {
        return event;
    }

    public void setEvent(String event) {
        this.event = event;
    }

    public String getEventTypeName() {
        return eventTypeName;
    }

    public void setEventTypeName(String eventTypeName) {
        this.eventTypeName = eventTypeName;
    }

    @Override
    public String toString() {
        return "CMAlert{" +
            "instanceId=" + instanceId +
            ", instanceName='" + instanceName + '\'' +
            ", time=" + time +
            ", alertType=" + alertType +
            ", alertLevel=" + alertLevel +
            ", sourceRule='" + sourceRule + '\'' +
            ", alertEventId=" + alertEventId +
            ", eventType=" + eventType +
            ", detail='" + detail + '\'' +
            ", event='" + event + '\'' +
            ", eventTypeName='" + eventTypeName + '\'' +
            '}';
    }
}
