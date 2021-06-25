package com.idera.sqlcm.entities.addserverwizard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class AgentDeploymentProperties {

    @JsonProperty("isDeployed")
    private boolean deployed;

    @JsonProperty("isDeployedManually")
    private boolean deployedManually;

    @JsonProperty("agentServiceAccount")
    private AgentServiceAccount agentServiceAccount;

    @JsonProperty("agentTraceDirectory")
    private String agentTraceDirectory;

    public boolean isDeployed() {
        return deployed;
    }

    public void setDeployed(boolean deployed) {
        this.deployed = deployed;
    }

    public boolean isDeployedManually() {
        return deployedManually;
    }

    public void setDeployedManually(boolean deployedManually) {
        this.deployedManually = deployedManually;
    }

    public AgentServiceAccount getAgentServiceAccount() {
        return agentServiceAccount;
    }

    public void setAgentServiceAccount(AgentServiceAccount agentServiceAccount) {
        this.agentServiceAccount = agentServiceAccount;
    }

    public String getAgentTraceDirectory() {
        return agentTraceDirectory;
    }

    public void setAgentTraceDirectory(String agentTraceDirectory) {
        this.agentTraceDirectory = agentTraceDirectory;
    }
}
