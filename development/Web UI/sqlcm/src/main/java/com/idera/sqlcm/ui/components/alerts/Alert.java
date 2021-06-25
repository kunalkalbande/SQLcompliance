package com.idera.sqlcm.ui.components.alerts;

import com.fasterxml.jackson.annotation.JsonProperty;

public class Alert {

    @JsonProperty("alertId")
    private long alertId;

    @JsonProperty("alertType")
    private long alertType;

    @JsonProperty("instance")
    private String instance;

    @JsonProperty("created")
    private String created;

    @JsonProperty("alertLevel")
    private long alertLevel;

    @JsonProperty("alertEventId")
    private long alertEventId;

    @JsonProperty("ruleName")
    private String ruleName;

    @JsonProperty("eventType")
    private long eventType;

    @JsonProperty("alertRuleId")
    private long alertRuleId;

    @JsonProperty("emailStatus")
    private long emailStatus;

    @JsonProperty("logStatus")
    private long logStatus;

    @JsonProperty("message")
    private String message;

    @JsonProperty("computerName")
    private Object computerName;

    @JsonProperty("snmpTrapStatus")
    private long snmpTrapStatus;

    public long getAlertId() {
        return alertId;
    }

    public void setAlertId(long alertId) {
        this.alertId = alertId;
    }

    public long getAlertType() {
        return alertType;
    }

    public void setAlertType(long alertType) {
        this.alertType = alertType;
    }

    public String getInstance() {
        return instance;
    }

    public void setInstance(String instance) {
        this.instance = instance;
    }

    public String getCreated() {
        return created;
    }

    public void setCreated(String created) {
        this.created = created;
    }

    public long getAlertLevel() {
        return alertLevel;
    }

    public void setAlertLevel(long alertLevel) {
        this.alertLevel = alertLevel;
    }

    public long getAlertEventId() {
        return alertEventId;
    }

    public void setAlertEventId(long alertEventId) {
        this.alertEventId = alertEventId;
    }

    public String getRuleName() {
        return ruleName;
    }

    public void setRuleName(String ruleName) {
        this.ruleName = ruleName;
    }

    public long getEventType() {
        return eventType;
    }

    public void setEventType(long eventType) {
        this.eventType = eventType;
    }

    public long getAlertRuleId() {
        return alertRuleId;
    }

    public void setAlertRuleId(long alertRuleId) {
        this.alertRuleId = alertRuleId;
    }

    public long getEmailStatus() {
        return emailStatus;
    }

    public void setEmailStatus(long emailStatus) {
        this.emailStatus = emailStatus;
    }

    public long getLogStatus() {
        return logStatus;
    }

    public void setLogStatus(long logStatus) {
        this.logStatus = logStatus;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public Object getComputerName() {
        return computerName;
    }

    public void setComputerName(Object computerName) {
        this.computerName = computerName;
    }

    public long getSnmpTrapStatus() {
        return snmpTrapStatus;
    }

    public void setSnmpTrapStatus(long snmpTrapStatus) {
        this.snmpTrapStatus = snmpTrapStatus;
    }
}
