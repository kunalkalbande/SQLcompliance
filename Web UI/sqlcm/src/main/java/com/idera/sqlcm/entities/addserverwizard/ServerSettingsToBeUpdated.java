package com.idera.sqlcm.entities.addserverwizard;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;


public class ServerSettingsToBeUpdated {

    @JsonProperty("serverId")
    private long serverId;

    @JsonProperty("serverAuditedActivities")
    private CMAuditedActivities serverAuditedActivities;

    @JsonProperty("databasePermissions")
    private int databasePermissions;

    public long getServerId() {
        return serverId;
    }

    public void setServerId(long serverId) {
        this.serverId = serverId;
    }

    public CMAuditedActivities getServerAuditedActivities() {
        return serverAuditedActivities;
    }

    public void setServerAuditedActivities(CMAuditedActivities serverAuditedActivities) {
        this.serverAuditedActivities = serverAuditedActivities;
    }

    public int getDatabasePermissions() {
        return databasePermissions;
    }

    public void setDatabasePermissions(int databasePermissions) {
        this.databasePermissions = databasePermissions;
    }
}
