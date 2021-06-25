package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

public class CMAuditSettings {
    @JsonProperty("lastAgentUpdateDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastAgentUpdateDateTime;

    @JsonProperty("auditSettingsUpdateEnabled")
    private boolean auditSettingsUpdateEnabled;

    @JsonProperty("agentAuditLevel")
    private int agentAuditLevel;

    @JsonProperty("currentAuditLevel")
    private int currentAuditLevel;

    public Date getLastAgentUpdateDateTime() {
        return lastAgentUpdateDateTime;
    }

    public void setLastAgentUpdateDateTime(Date lastAgentUpdateDateTime) {
        this.lastAgentUpdateDateTime = lastAgentUpdateDateTime;
    }

    public boolean isAuditSettingsUpdateEnabled() {
        return auditSettingsUpdateEnabled;
    }

    public void setAuditSettingsUpdateEnabled(boolean auditSettingsUpdateEnabled) {
        this.auditSettingsUpdateEnabled = auditSettingsUpdateEnabled;
    }

    public int getAgentAuditLevel() {
        return agentAuditLevel;
    }

    public void setAgentAuditLevel(int agentAuditLevel) {
        this.agentAuditLevel = agentAuditLevel;
    }

    public int getCurrentAuditLevel() {
        return currentAuditLevel;
    }

    public void setCurrentAuditLevel(int currentAuditLevel) {
        this.currentAuditLevel = currentAuditLevel;
    }
}
