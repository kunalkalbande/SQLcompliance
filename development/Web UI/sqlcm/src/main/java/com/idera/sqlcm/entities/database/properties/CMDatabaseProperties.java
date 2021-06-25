package com.idera.sqlcm.entities.database.properties;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.google.common.base.Objects;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;

import java.util.Date;
import java.util.List;


@JsonIgnoreProperties(ignoreUnknown = true)
public class CMDatabaseProperties {

    @JsonProperty("databaseId")
    private Long databaseId;

    @JsonProperty("serverInstance")
    private String serverInstance;

    @JsonProperty("databaseName")
    private String databaseName;

    @JsonProperty("description")
    private String description;

    @JsonProperty("auditingEnableStatus")
    private boolean auditingEnableStatus;

    @JsonProperty("createdDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date createdDateTime;

    @JsonProperty("lastModifiedDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastModifiedDateTime;

    @JsonProperty("lastChangedStatusDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastChangedStatusDateTime;

    @JsonProperty("auditedActivities")
    private CMAuditedActivity auditedActivities;

    @JsonProperty("auditedPrivilegedUserActivities")
    private CMAuditedActivity auditedPrivilegedUserActivities;

    @JsonProperty("dmlSelectFilters")
    private CMDMLSelectFilter dmlSelectFilters;

    @JsonProperty("trustedRolesAndUsers")
    private CMInstanceUsersAndRoles trustedRolesAndUsers;

    @JsonProperty("privilegedRolesAndUsers")
    private CMInstanceUsersAndRoles privilegedRolesAndUsers;

    @JsonProperty("auditBeforeAfterData")
    private CMAuditBeforeAfterData auditBeforeAfterData;

    @JsonProperty("sensitiveColumnTableData")
    private CMSensitiveColumnTableData sensitiveColumnTableData;

    public Long getDatabaseId() {
        return databaseId;
    }

    public void setDatabaseId(Long databaseId) {
        this.databaseId = databaseId;
    }

    public String getServerInstance() {
        return serverInstance;
    }

    public void setServerInstance(String serverInstance) {
        this.serverInstance = serverInstance;
    }

    public String getDatabaseName() {
        return databaseName;
    }

    public void setDatabaseName(String databaseName) {
        this.databaseName = databaseName;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public boolean isAuditingEnableStatus() {
        return auditingEnableStatus;
    }

    public void setAuditingEnableStatus(boolean auditingEnableStatus) {
        this.auditingEnableStatus = auditingEnableStatus;
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

    public Date getLastChangedStatusDateTime() {
        return lastChangedStatusDateTime;
    }

    public void setLastChangedStatusDateTime(Date lastChangedStatusDateTime) {
        this.lastChangedStatusDateTime = lastChangedStatusDateTime;
    }

    public CMAuditedActivity getAuditedActivities() {
        return auditedActivities;
    }

    public void setAuditedActivities(CMAuditedActivity auditedActivities) {
        this.auditedActivities = auditedActivities;
    }

    public CMAuditedActivity getAuditedPrivilegedUserActivities() {
        return auditedPrivilegedUserActivities;
    }

    public void setAuditedPrivilegedUserActivities(CMAuditedActivity auditedPrivilegedUserActivities) {
        this.auditedPrivilegedUserActivities = auditedPrivilegedUserActivities;
    }

    public CMDMLSelectFilter getDmlSelectFilters() {
        return dmlSelectFilters;
    }

    public void setDmlSelectFilters(CMDMLSelectFilter dmlSelectFilters) {
        this.dmlSelectFilters = dmlSelectFilters;
    }

    public CMInstanceUsersAndRoles getTrustedRolesAndUsers() {
        return trustedRolesAndUsers;
    }

    public void setTrustedRolesAndUsers(CMInstanceUsersAndRoles trustedRolesAndUsers) {
        this.trustedRolesAndUsers = trustedRolesAndUsers;
    }

    public CMInstanceUsersAndRoles getPrivilegedRolesAndUsers() {
        return privilegedRolesAndUsers;
    }

    public void setPrivilegedRolesAndUsers(CMInstanceUsersAndRoles privilegedRolesAndUsers) {
        this.privilegedRolesAndUsers = privilegedRolesAndUsers;
    }

    public CMAuditBeforeAfterData getAuditBeforeAfterData() {
        return auditBeforeAfterData;
    }

    public void setAuditBeforeAfterData(CMAuditBeforeAfterData auditBeforeAfterData) {
        this.auditBeforeAfterData = auditBeforeAfterData;
    }

    public CMSensitiveColumnTableData getSensitiveColumnTableData() {
        return sensitiveColumnTableData;
    }

    public void setSensitiveColumnTableData(CMSensitiveColumnTableData sensitiveColumnTableData) {
        this.sensitiveColumnTableData = sensitiveColumnTableData;
    }

    @Override
    public String toString() {
        return Objects.toStringHelper(this)
                      .add("databaseId", databaseId)
                      .add("serverInstance", serverInstance)
                      .add("databaseName", databaseName)
                      .add("description", description)
                      .add("auditingEnableStatus", auditingEnableStatus)
                      .add("createdDateTime", createdDateTime)
                      .add("lastModifiedDateTime", lastModifiedDateTime)
                      .add("lastChangedStatusDateTime", lastChangedStatusDateTime)
                      .add("auditedActivities", auditedActivities)
                      .add("auditedPrivilegedUserActivities", auditedPrivilegedUserActivities)
                      .add("dmlSelectFilters", dmlSelectFilters)
                      .add("trustedRolesAndUsers", trustedRolesAndUsers)
                      .add("privilegedRolesAndUsers", privilegedRolesAndUsers)
                      .add("auditBeforeAfterData", auditBeforeAfterData)
                      .add("sensitiveColumnTableData", sensitiveColumnTableData)
                      .toString();
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        CMDatabaseProperties that = (CMDatabaseProperties) o;
        return Objects.equal(this.databaseId, that.databaseId) &&
                Objects.equal(this.serverInstance, that.serverInstance) &&
                Objects.equal(this.databaseName, that.databaseName) &&
                Objects.equal(this.description, that.description) &&
                Objects.equal(this.auditingEnableStatus, that.auditingEnableStatus) &&
                Objects.equal(this.createdDateTime, that.createdDateTime) &&
                Objects.equal(this.lastModifiedDateTime, that.lastModifiedDateTime) &&
                Objects.equal(this.lastChangedStatusDateTime, that.lastChangedStatusDateTime) &&
                Objects.equal(this.auditedActivities, that.auditedActivities) &&
                Objects.equal(this.auditedPrivilegedUserActivities, that.auditedPrivilegedUserActivities) &&
                Objects.equal(this.dmlSelectFilters, that.dmlSelectFilters) &&
                Objects.equal(this.trustedRolesAndUsers, that.trustedRolesAndUsers) &&
                Objects.equal(this.privilegedRolesAndUsers, that.privilegedRolesAndUsers) &&
                Objects.equal(this.auditBeforeAfterData, that.auditBeforeAfterData) &&
                Objects.equal(this.sensitiveColumnTableData, that.sensitiveColumnTableData);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(databaseId, serverInstance, databaseName, description, auditingEnableStatus, createdDateTime,
                lastModifiedDateTime, lastChangedStatusDateTime, auditedActivities, auditedPrivilegedUserActivities, dmlSelectFilters,
                trustedRolesAndUsers, privilegedRolesAndUsers, auditBeforeAfterData, sensitiveColumnTableData);
    }
}
