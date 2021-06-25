package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import java.util.Date;

public class CMArchiveProperties{

    @JsonProperty("instance")
    private String instance;

    @JsonProperty("displayName")
    private String displayName;

    @JsonProperty("description")
    private String description;

    @JsonProperty("databaseName")
    private String databaseName;

    @JsonProperty("eventTimeSpanFrom")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date eventTimeSpanFrom;

    @JsonProperty("eventTimeSpanTo")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date eventTimeSpanTo;

    @JsonProperty("databaseIntegrity")
    private int databaseIntegrity;

    @JsonProperty("lastIntegrityCheck")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastIntegrityCheck;

    @JsonProperty("lastIntegrityCheckResult")
    private long lastIntegrityCheckResult;

    @JsonProperty("defaultAccess")
    private long defaultAccess;

    @JsonProperty("isCompatibleSchema")
    private boolean isCompatibleSchema;

    @JsonProperty("isValidArchive")
    private boolean isValidArchive;

    public String getInstance() {
        return instance;
    }

    public void setInstance(String instance) {
        this.instance = instance;
    }

    public String getDisplayName() {
        return displayName;
    }

    public void setDisplayName(String displayName) {
        this.displayName = displayName;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getDatabaseName() {
        return databaseName;
    }

    public void setDatabaseName(String databaseName) {
        this.databaseName = databaseName;
    }

    public Date getEventTimeSpanFrom() {
        return eventTimeSpanFrom;
    }

    public void setEventTimeSpanFrom(Date eventTimeSpanFrom) {
        this.eventTimeSpanFrom = eventTimeSpanFrom;
    }

    public Date getEventTimeSpanTo() {
        return eventTimeSpanTo;
    }

    public void setEventTimeSpanTo(Date eventTimeSpanTo) {
        this.eventTimeSpanTo = eventTimeSpanTo;
    }

    public int getDatabaseIntegrity() {
        return databaseIntegrity;
    }

    public void setDatabaseIntegrity(int databaseIntegrity) {
        this.databaseIntegrity = databaseIntegrity;
    }

    public Date getLastIntegrityCheck() {
        return lastIntegrityCheck;
    }

    public void setLastIntegrityCheck(Date lastIntegrityCheck) {
        this.lastIntegrityCheck = lastIntegrityCheck;
    }

    public long getLastIntegrityCheckResult() {
        return lastIntegrityCheckResult;
    }

    public void setLastIntegrityCheckResult(long lastIntegrityCheckResult) {
        this.lastIntegrityCheckResult = lastIntegrityCheckResult;
    }

    public long getDefaultAccess() {
        return defaultAccess;
    }

    public void setDefaultAccess(long defaultAccess) {
        this.defaultAccess = defaultAccess;
    }

    public boolean isCompatibleSchema() {
        return isCompatibleSchema;
    }

    public void setIsCompatibleSchema(boolean isCompatibleSchema) {
        this.isCompatibleSchema = isCompatibleSchema;
    }

    public boolean isValidArchive() {
        return isValidArchive;
    }

    public void setIsValidArchive(boolean isValidArchive) {
        this.isValidArchive = isValidArchive;
    }

    @Override
    public String toString() {
        return "CMArchiveProperties{" +
            "instance='" + instance + '\'' +
            ", displayName='" + displayName + '\'' +
            ", description='" + description + '\'' +
            ", databaseName='" + databaseName + '\'' +
            ", eventTimeSpanFrom=" + eventTimeSpanFrom +
            ", eventTimeSpanTo=" + eventTimeSpanTo +
            ", databaseIntegrity=" + databaseIntegrity +
            ", lastIntegrityCheck=" + lastIntegrityCheck +
            ", lastIntegrityCheckResult=" + lastIntegrityCheckResult +
            ", defaultAccess=" + defaultAccess +
            ", isCompatibleSchema=" + isCompatibleSchema +
            '}';
    }
}