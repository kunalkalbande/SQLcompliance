package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import com.idera.sqlcm.entities.instances.ConnectionStatus;
import com.idera.sqlcm.ui.dashboard.AlertLabelConverter;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class Instance extends CMEntity {

    @JsonProperty("InstanceStatus")
    protected boolean instanceOK;

    @JsonProperty("name")
    protected String instanceName;

    @JsonProperty("StatusText")
    protected String statusText;

    @JsonProperty("databaseCount")
    protected int numberOfAuditedDatabases;

    @JsonProperty("sqlVersionString")
    protected String sqlServerVersionEdition;

    @JsonProperty("AuditStatus")
    protected String auditStatus;

    @JsonProperty("lastTimeAgentContacted")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    protected Date lastAgentContact;

    @JsonProperty("severeAlerts")
    protected int severeAlerts;

    @JsonProperty("highAlerts")
    protected int highAlerts;

    @JsonProperty("mediumAlert")
    protected int mediumAlert;

    @JsonProperty("lowAlerts")
    protected int lowAlerts;

    @JsonProperty("recentAlertCount")
    protected int recentAlertCount;

    private boolean auditSettingsUpdateEnabled;

    private boolean auditEnabled;
    private boolean primary;

    @JsonProperty("isDeployed")
    private boolean deployed;

    public boolean isInstanceOK() {
        return instanceOK;
    }

    public void setInstanceOK(boolean instanceOK) {
        this.instanceOK = instanceOK;
    }

    public String getInstanceName() {
        return instanceName;
    }

    public void setInstanceName(String instanceName) {
        this.instanceName = instanceName;
    }

    public String getStatusText() {
        return statusText;
    }

    public void setStatusText(String statusText) {
        this.statusText = statusText;
    }

    public int getNumberOfAuditedDatabases() {
        return numberOfAuditedDatabases;
    }

    public void setNumberOfAuditedDatabases(int numberOfAuditedDatabases) {
        this.numberOfAuditedDatabases = numberOfAuditedDatabases;
    }

    public String getSqlServerVersionEdition() {
        return sqlServerVersionEdition;
    }

    public void setSqlServerVersionEdition(String sqlServerVersionEdition) {
        this.sqlServerVersionEdition = sqlServerVersionEdition;
    }

    public String getAuditStatus() {
        return auditStatus;
    }

    public void setAuditStatus(String auditStatus) {
        this.auditStatus = auditStatus;
    }

    public Date getLastAgentContact() {
        return lastAgentContact;
    }

    public void setLastAgentContact(Date lastAgentContact) {
        this.lastAgentContact = lastAgentContact;
    }

    public ConnectionStatus getStatus() {
        ConnectionStatus status = ConnectionStatus.getConnectionStatus(instanceOK);
        return status;
    }

    public int getSevereAlerts() {
        return severeAlerts;
    }

    public void setSevereAlerts(int severeAlerts) {
        this.severeAlerts = severeAlerts;
    }

    public int getHighAlerts() {
        return highAlerts;
    }

    public void setHighAlerts(int highAlerts) {
        this.highAlerts = highAlerts;
    }

    public int getMediumAlert() {
        return mediumAlert;
    }

    public void setMediumAlert(int mediumAlert) {
        this.mediumAlert = mediumAlert;
    }

    public int getLowAlerts() {
        return lowAlerts;
    }

    public void setLowAlerts(int lowAlerts) {
        this.lowAlerts = lowAlerts;
    }

    public int getRecentAlertCount() {
        return recentAlertCount;
    }

    public void setRecentAlertCount(int recentAlertCount) {
        this.recentAlertCount = recentAlertCount;
    }

    public boolean isAuditSettingsUpdateEnabled() {
        return auditSettingsUpdateEnabled;
    }

    public void setAuditSettingsUpdateEnabled(boolean auditSettingsUpdateEnabled) {
        this.auditSettingsUpdateEnabled = auditSettingsUpdateEnabled;
    }

    public boolean isAuditEnabled() {
        return auditEnabled;
    }

    public void setAuditEnabled(boolean auditEnabled) {
        this.auditEnabled = auditEnabled;
    }

    public boolean isPrimary() {
        return primary;
    }

    public void setPrimary(boolean primary) {
        this.primary = primary;
    }

    public boolean isDeployed() {
        return deployed;
    }

    public void setDeployed(boolean deployed) {
        this.deployed = deployed;
    }

    @JsonIgnore
    public String getAlertInfo() {
        return AlertLabelConverter.doConvert(this);
    }
}
