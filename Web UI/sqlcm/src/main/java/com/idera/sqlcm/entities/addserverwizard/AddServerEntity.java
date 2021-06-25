package com.idera.sqlcm.entities.addserverwizard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class AddServerEntity {

    @JsonProperty("instance")
    private String instance;

    @JsonProperty("description")
    private String description;

    @JsonProperty("isVirtualServer")
    private boolean isVirtualServer;

    @JsonProperty("existingAuditData")
    private int existingAuditData;

    @JsonProperty("agentDeployStatus")
    private int agentDeployStatus;

    @JsonProperty("agentDeploymentProperties")
    private AgentDeploymentProperties agentDeploymentProperties;

    public String getInstance() {
        return instance;
    }

    public void setInstance(String instance) {
        this.instance = instance;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public boolean isVirtualServer() {
        return isVirtualServer;
    }

    public void setVirtualServer(boolean isVirtualServer) {
        this.isVirtualServer = isVirtualServer;
    }

    public int getExistingAuditData() {
        return existingAuditData;
    }

    public void setExistingAuditData(int existingAuditData) {
        this.existingAuditData = existingAuditData;
    }

    public int getAgentDeployStatus() {
        return agentDeployStatus;
    }

    public void setAgentDeployStatus(int agentDeployStatus) {
        this.agentDeployStatus = agentDeployStatus;
    }

    public AgentDeploymentProperties getAgentDeploymentProperties() {
        return agentDeploymentProperties;
    }

    public void setAgentDeploymentProperties(AgentDeploymentProperties agentDeploymentProperties) {
        this.agentDeploymentProperties = agentDeploymentProperties;
    }
}
