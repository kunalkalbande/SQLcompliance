package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMAgentGeneralProperties {
    @JsonProperty("agentComputer")
    private String agentComputer;

    @JsonProperty("agentSettings")
    private CMAgentSettings agentSettings;

    @JsonProperty("auditSettings")
    private CMAuditSettings auditSettings;

    public String getAgentComputer() {
        return agentComputer;
    }

    public void setAgentComputer(String agentComputer) {
        this.agentComputer = agentComputer;
    }

    public CMAgentSettings getAgentSettings() {
        return agentSettings;
    }

    public void setAgentSettings(CMAgentSettings agentSettings) {
        this.agentSettings = agentSettings;
    }

    public CMAuditSettings getAuditSettings() {
        return auditSettings;
    }

    public void setAuditSettings(CMAuditSettings auditSettings) {
        this.auditSettings = auditSettings;
    }
}
