package com.idera.sqlcm.beEntities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class AuditedInstanceBE {
    private long id;
    private String instance;
    private int eventCategories;
    private int collectedEventCount;
    private int agentStatus;
    private boolean checkBool;

    public boolean isCheckBool() {
		return checkBool;
	}

	public void setCheckBool(boolean checkBool) {
		this.checkBool = checkBool;
	}

    @JsonProperty("isDeployed")
    private boolean deployed;

    public boolean isDeployed() {
        return deployed;
    }

    public void setDeployed(boolean isDeployed) {
        this.deployed = isDeployed;
    }

    @JsonProperty("totalDatabaseCount")

    private int databaseCount;

    private String statusText;

    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastHeartbeat;

    @JsonProperty("IsAudited")
    private boolean isAudited;

    @JsonProperty("isEnabled")
    private boolean enabled;

    private String sqlVersionString;
    private boolean serverStatus;
    private int lowAlerts;
    private int mediumAlert;
    private int highAlerts;
    private int severeAlerts;
    private int recentAlertCount;

    @JsonProperty("lastTimeAgentContacted")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastTimeAgentContacted;
    private boolean auditSettingsUpdateEnabled;
    private int auditedDatabaseCount;
    @JsonProperty("isPrimary")
    private boolean primary;

    public long getId() {
        return id;
    }

    public void setId(long id) {
        this.id = id;
    }

    public String getInstance() {
        return instance;
    }

    public void setInstance(String instance) {
        this.instance = instance;
    }

    public int getEventCategories() {
        return eventCategories;
    }

    public void setEventCategories(int eventCategories) {
        this.eventCategories = eventCategories;
    }

    public int getCollectedEventCount() {
        return collectedEventCount;
    }

    public void setCollectedEventCount(int collectedEventCount) {
        this.collectedEventCount = collectedEventCount;
    }

    public int getAgentStatus() {
        return agentStatus;
    }

    public void setAgentStatus(int agentStatus) {
        this.agentStatus = agentStatus;
    }

    public int getDatabaseCount() {
        return databaseCount;
    }

    public void setDatabaseCount(int databaseCount) {
        this.databaseCount = databaseCount;
    }

    public String getStatusText() {
        return statusText;
    }

    public void setStatusText(String statusText) {
        this.statusText = statusText;
    }

    public Date getLastHeartbeat() {
        return lastHeartbeat;
    }

    public void setLastHeartbeat(Date lastHeartbeat) {
        this.lastHeartbeat = lastHeartbeat;
    }

    public boolean isAudited() {
        return isAudited;
    }

    public void setAudited(boolean isAudited) {
        this.isAudited = isAudited;
    }

    public String getSqlVersionString() {
        return sqlVersionString;
    }

    public void setSqlVersionString(String sqlVersionString) {
        this.sqlVersionString = sqlVersionString;
    }

    public boolean isServerStatus() {
        return serverStatus;
    }

    public void setServerStatus(boolean serverStatus) {
        this.serverStatus = serverStatus;
    }

    public int getLowAlerts() {
        return lowAlerts;
    }

    public void setLowAlerts(int lowAlerts) {
        this.lowAlerts = lowAlerts;
    }

    public int getMediumAlert() {
        return mediumAlert;
    }

    public void setMediumAlert(int mediumAlert) {
        this.mediumAlert = mediumAlert;
    }

    public int getHighAlerts() {
        return highAlerts;
    }

    public void setHighAlerts(int highAlerts) {
        this.highAlerts = highAlerts;
    }

    public int getSevereAlerts() {
        return severeAlerts;
    }

    public void setSevereAlerts(int severeAlerts) {
        this.severeAlerts = severeAlerts;
    }

    public int getRecentAlertCount() {
        return recentAlertCount;
    }

    public void setRecentAlertCount(int recentAlertCount) {
        this.recentAlertCount = recentAlertCount;
    }

    public Date getLastTimeAgentContacted() {
        return lastTimeAgentContacted;
    }

    public void setLastTimeAgentContacted(Date lastTimeAgentContacted) {
        this.lastTimeAgentContacted = lastTimeAgentContacted;
    }

    public boolean isEnabled() {
        return enabled;
    }

    public void setEnabled(boolean enabled) {
        this.enabled = enabled;
    }

    public boolean isAuditSettingsUpdateEnabled() {
        return auditSettingsUpdateEnabled;
    }

    public void setAuditSettingsUpdateEnabled(boolean auditSettingsUpdateEnabled) {
        this.auditSettingsUpdateEnabled = auditSettingsUpdateEnabled;
    }

    public int getAuditedDatabaseCount() {
        return auditedDatabaseCount;
    }

    public void setAuditedDatabaseCount(int auditedDatabaseCount) {
        this.auditedDatabaseCount = auditedDatabaseCount;
    }

    public boolean isPrimary() {
        return primary;
    }

    public void setPrimary(boolean primary) {
        this.primary = primary;
    }
}
