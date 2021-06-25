package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

public class CMAuditInstanceGeneralProperties {
    @JsonProperty("instance")
    private String instance;

    @JsonProperty("instancePort")
    private Long instancePort;

    @JsonProperty("instanceServer")
    private String instanceServer;

    @JsonProperty("instanceVersion")
    private String instanceVersion;

    @JsonProperty("isClustered")
    private boolean isClustered;

    @JsonProperty("description")
    private String description;

    @JsonProperty("statusMessage")
    private String statusMessage;

    @JsonProperty("createdDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date createdDateTime;

    @JsonProperty("lastModifiedDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastModifiedDateTime;

    @JsonProperty("lastHeartbeatDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastHeartbeatDateTime;

    @JsonProperty("eventsReceivedDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date eventsReceivedDateTime;

    @JsonProperty("IsAuditEnabled")
    private boolean auditEnabled;

    @JsonProperty("IsAuditedServer")
    private boolean auditedServer;

    @JsonProperty("lastAgentUpdateDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastAgentUpdateDateTime;

    @JsonProperty("auditSettingsUpdateEnabled")
    private boolean auditSettingsUpdateEnabled;

    @JsonProperty("eventsDatabaseName")
    private String eventsDatabaseName;

    @JsonProperty("isDatabaseIntegrityOk")
    private boolean databaseIntegrityOk;

    @JsonProperty("lastIntegrityCheckDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastIntegrityCheckDateTime;

    @JsonProperty("lastIntegrityCheckResultsStatus")
    private int lastIntegrityCheckResultsStatus;

    @JsonProperty("lastArchiveCheckDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastArchiveCheckDateTime;

    @JsonProperty("lastArchiveCheckResultsStatus")
    private int lastArchiveCheckResultsStatus;

    public String getInstance() {
        return instance;
    }

    public void setInstance(String instance) {
        this.instance = instance;
    }

    public Long getInstancePort() {
        return instancePort;
    }

    public void setInstancePort(Long instancePort) {
        this.instancePort = instancePort;
    }

    public String getInstanceServer() {
        return instanceServer;
    }

    public void setInstanceServer(String instanceServer) {
        this.instanceServer = instanceServer;
    }

    public String getInstanceVersion() {
        return instanceVersion;
    }

    public void setInstanceVersion(String instanceVersion) {
        this.instanceVersion = instanceVersion;
    }

    public boolean isClustered() {
        return isClustered;
    }

    public void setClustered(boolean isClustered) {
        this.isClustered = isClustered;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getStatusMessage() {
        return statusMessage;
    }

    public void setStatusMessage(String statusMessage) {
        this.statusMessage = statusMessage;
    }

    public Date getCreatedDateTime() {
        return createdDateTime;
    }

    public void setCreatedDateTime(Date createdDateTime) {
        this.createdDateTime = createdDateTime;
    }

    public Date getLastModifiedDateTime() {
        return lastModifiedDateTime;
    }

    public void setLastModifiedDateTime(Date lastModifiedDateTime) {
        this.lastModifiedDateTime = lastModifiedDateTime;
    }

    public Date getLastHeartbeatDateTime() {
        return lastHeartbeatDateTime;
    }

    public void setLastHeartbeatDateTime(Date lastHeartbeatDateTime) {
        this.lastHeartbeatDateTime = lastHeartbeatDateTime;
    }

    public Date getEventsReceivedDateTime() {
        return eventsReceivedDateTime;
    }

    public void setEventsReceivedDateTime(Date eventsReceivedDateTime) {
        this.eventsReceivedDateTime = eventsReceivedDateTime;
    }

    public void setLastAgentUpdateDateTime(Date lastAgentUpdateDateTime) {
        this.lastAgentUpdateDateTime = lastAgentUpdateDateTime;
    }

    public void setLastIntegrityCheckDateTime(Date lastIntegrityCheckDateTime) {
        this.lastIntegrityCheckDateTime = lastIntegrityCheckDateTime;
    }

    public void setLastArchiveCheckDateTime(Date lastArchiveCheckDateTime) {
        this.lastArchiveCheckDateTime = lastArchiveCheckDateTime;
    }

    public boolean isAuditEnabled() {
        return auditEnabled;
    }

    public void setAuditEnabled(boolean auditEnabled) {
        this.auditEnabled = auditEnabled;
    }

    public boolean isAuditedServer() {
        return auditedServer;
    }

    public void setAuditedServer(boolean auditedServer) {
        this.auditedServer = auditedServer;
    }

    public boolean isAuditSettingsUpdateEnabled() {
        return auditSettingsUpdateEnabled;
    }

    public void setAuditSettingsUpdateEnabled(boolean auditSettingsUpdateEnabled) {
        this.auditSettingsUpdateEnabled = auditSettingsUpdateEnabled;
    }

    public String getEventsDatabaseName() {
        return eventsDatabaseName;
    }

    public void setEventsDatabaseName(String eventsDatabaseName) {
        this.eventsDatabaseName = eventsDatabaseName;
    }

    public boolean isDatabaseIntegrityOk() {
        return databaseIntegrityOk;
    }

    public void setDatabaseIntegrityOk(boolean databaseIntegrityOk) {
        this.databaseIntegrityOk = databaseIntegrityOk;
    }

    public int getLastIntegrityCheckResultsStatus() {
        return lastIntegrityCheckResultsStatus;
    }

    public void setLastIntegrityCheckResultsStatus(int lastIntegrityCheckResultsStatus) {
        this.lastIntegrityCheckResultsStatus = lastIntegrityCheckResultsStatus;
    }

    public Date getLastAgentUpdateDateTime() {
        return lastAgentUpdateDateTime;
    }

    public Date getLastIntegrityCheckDateTime() {
        return lastIntegrityCheckDateTime;
    }

    public Date getLastArchiveCheckDateTime() {
        return lastArchiveCheckDateTime;
    }

    public int getLastArchiveCheckResultsStatus() {
        return lastArchiveCheckResultsStatus;
    }

    public void setLastArchiveCheckResultsStatus(int lastArchiveCheckResultsStatus) {
        this.lastArchiveCheckResultsStatus = lastArchiveCheckResultsStatus;
    }
}
