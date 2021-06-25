package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;

public class CMInstanceProperties {
    @JsonProperty("serverId")
    private long serverId;
    @JsonProperty("generalProperties")
    private CMAuditInstanceGeneralProperties generalProperties;
    @JsonProperty("auditedActivities")
    private CMAuditedActivities auditedActivities;
    @JsonProperty("privilegedRolesAndUsers")
    private CMInstanceUsersAndRoles privilegedRolesAndUsers;
    @JsonProperty("auditedPrivilegedUserActivities")
    private CMAuditedPrivilegedUserActivities auditedPrivilegedUserActivities;
    @JsonProperty("auditThresholdsData")
    private CMAuditThresholdsData auditThresholdsData;
    @JsonProperty("serverAdvancedProperties")
    private CMServerAdvancedProperties serverAdvancedProperties;

    public long getServerId() {
        return serverId;
    }

    public void setServerId(long serverId) {
        this.serverId = serverId;
    }

    public CMAuditInstanceGeneralProperties getGeneralProperties() {
        return generalProperties;
    }

    public void setGeneralProperties(CMAuditInstanceGeneralProperties generalProperties) {
        this.generalProperties = generalProperties;
    }

    public CMAuditedActivities getAuditedActivities() {
        return auditedActivities;
    }

    public void setAuditedActivities(CMAuditedActivities auditedActivities) {
        this.auditedActivities = auditedActivities;
    }

    public CMInstanceUsersAndRoles getPrivilegedRolesAndUsers() {
        return privilegedRolesAndUsers;
    }

    public void setPrivilegedRolesAndUsers(CMInstanceUsersAndRoles privilegedRolesAndUsers) {
        this.privilegedRolesAndUsers = privilegedRolesAndUsers;
    }

    public CMAuditedPrivilegedUserActivities getAuditedPrivilegedUserActivities() {
        return auditedPrivilegedUserActivities;
    }

    public void setAuditedPrivilegedUserActivities(CMAuditedPrivilegedUserActivities auditedPrivilegedUserActivities) {
        this.auditedPrivilegedUserActivities = auditedPrivilegedUserActivities;
    }

    public CMAuditThresholdsData getAuditThresholdsData() {
        return auditThresholdsData;
    }

    public void setAuditThresholdsData(CMAuditThresholdsData auditThresholdsData) {
        this.auditThresholdsData = auditThresholdsData;
    }

    public CMServerAdvancedProperties getServerAdvancedProperties() {
        return serverAdvancedProperties;
    }

    public void setServerAdvancedProperties(CMServerAdvancedProperties serverAdvancedProperties) {
        this.serverAdvancedProperties = serverAdvancedProperties;
    }
}
