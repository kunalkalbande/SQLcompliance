package com.idera.sqlcm.entities;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMInstance extends CMEntity {

	private int databaseCount;
	private String statusMessage;
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date lastArchivedString;

    private String instance;

    @JsonProperty("eventCategories")
    private long eventCategories;

    @JsonProperty("collectedEventCount")
    private long collectedEventCount;

    @JsonProperty("agentStatus")
    private long agentStatus;

    @JsonProperty("auditedDatabaseCount")
    private long auditedDatabaseCount;

    @JsonProperty("statusText")
    private String statusText;

	public String getInstance() {
		return instance;
	}

	public void setInstance(String instance) {
		this.instance = instance;
	}
    @JsonProperty("lastHeartbeat")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastHeartbeat;

    @JsonProperty("IsAudited")
    private boolean isAudited;

    @JsonProperty("sqlVersionString")
    private String sqlVersionString;

    @JsonProperty("serverStatus")
    private boolean serverStatus;

    @JsonProperty("lowAlerts")
    private long lowAlerts;

    @JsonProperty("mediumAlert")
    private long mediumAlert;

    @JsonProperty("highAlerts")
    private long highAlerts;

    @JsonProperty("severeAlerts")
    private long severeAlerts;

    @JsonProperty("lastTimeAgentContacted")
    private String lastTimeAgentContacted;

    @JsonProperty("isEnabled")
    private boolean isEnabled;

    @JsonProperty("totalDatabaseCount")
    private long totalDatabaseCount;

    @JsonProperty("status")
    private long status;

    @JsonProperty("lastArchived")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastArchived;

    @JsonProperty("recentAlertCount")
    private long recentAlertCount;

    @JsonProperty("eventFilters")
    private Object eventFilters;

    @JsonProperty("auditedServerActivities")
    private List<String> auditedServerActivities = new ArrayList<String>();

    @JsonProperty("auditedPrivilegedUsersActivities")
    private List<String> auditedPrivilegedUsersActivities = new ArrayList<String>();

    @JsonProperty("privilegedUsersCount")
    private long privilegedUsersCount;

    @JsonProperty("detailedServerStatus")
    private long detailedServerStatus;

    @JsonProperty("instance")
    public String getInstanceName() {
        return instance;
    }

    @JsonProperty("instance")
    public void setInstanceName(String instance) {
        this.instance = instance;
        setName(instance);
    }

    public long getEventCategories() {
        return eventCategories;
    }

    public void setEventCategories(long eventCategories) {
        this.eventCategories = eventCategories;
    }

    public long getCollectedEventCount() {
        return collectedEventCount;
    }

    public void setCollectedEventCount(long collectedEventCount) {
        this.collectedEventCount = collectedEventCount;
    }

    public long getAgentStatus() {
        return agentStatus;
    }

    public void setAgentStatus(long agentStatus) {
        this.agentStatus = agentStatus;
    }

    public long getAuditedDatabaseCount() {
        return auditedDatabaseCount;
    }

    public void setAuditedDatabaseCount(long auditedDatabaseCount) {
        this.auditedDatabaseCount = auditedDatabaseCount;
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

    public long getLowAlerts() {
        return lowAlerts;
    }

    public void setLowAlerts(long lowAlerts) {
        this.lowAlerts = lowAlerts;
    }

    public long getMediumAlert() {
        return mediumAlert;
    }

    public void setMediumAlert(long mediumAlert) {
        this.mediumAlert = mediumAlert;
    }

    public long getHighAlerts() {
        return highAlerts;
    }

    public void setHighAlerts(long highAlerts) {
        this.highAlerts = highAlerts;
    }

    public long getSevereAlerts() {
        return severeAlerts;
    }

    public void setSevereAlerts(long severeAlerts) {
        this.severeAlerts = severeAlerts;
    }

    public String getLastTimeAgentContacted() {
        return lastTimeAgentContacted;
    }

    public void setLastTimeAgentContacted(String lastTimeAgentContacted) {
        this.lastTimeAgentContacted = lastTimeAgentContacted;
    }

    public boolean isEnabled() {
        return isEnabled;
    }

    public void setEnabled(boolean isEnabled) {
        this.isEnabled = isEnabled;
    }

    public long getTotalDatabaseCount() {
        return totalDatabaseCount;
    }

    public void setTotalDatabaseCount(long totalDatabaseCount) {
        this.totalDatabaseCount = totalDatabaseCount;
    }

    public long getStatus() {
        return status;
    }

    public void setStatus(long status) {
        this.status = status;
    }

    public Date getLastArchived() {
        return lastArchived;
    }

    public void setLastArchived(Date lastArchived) {
        this.lastArchived = lastArchived;
    }

    public long getRecentAlertCount() {
        return recentAlertCount;
    }

    public void setRecentAlertCount(long recentAlertCount) {
        this.recentAlertCount = recentAlertCount;
    }

    public Object getEventFilters() {
        return eventFilters;
    }

    public void setEventFilters(Object eventFilters) {
        this.eventFilters = eventFilters;
    }

    public List<String> getAuditedServerActivities() {
        return auditedServerActivities;
    }

    public void setAuditedServerActivities(List<String> auditedServerActivities) {
        this.auditedServerActivities = auditedServerActivities;
    }

    public List<String> getAuditedPrivilegedUsersActivities() {
        return auditedPrivilegedUsersActivities;
    }

    public void setAuditedPrivilegedUsersActivities(List<String> auditedPrivilegedUsersActivities) {
        this.auditedPrivilegedUsersActivities = auditedPrivilegedUsersActivities;
    }

    public long getDetailedServerStatus() {
        return detailedServerStatus;
    }

    public void setDetailedServerStatus(long detailedServerStatus) {
        this.detailedServerStatus = detailedServerStatus;
    }

    public long getPrivilegedUsersCount() {
        return privilegedUsersCount;
    }

    public void setPrivilegedUsersCount(long privilegedUsersCount) {
        this.privilegedUsersCount = privilegedUsersCount;
    }
}
