package com.idera.sqlcm.entities;


import com.fasterxml.jackson.annotation.JsonProperty;

public class CMCheckAgentStatusResult {

    @JsonProperty("isActive")
    private boolean isActive;

    @JsonProperty("agentServer")
    private String agentServer;

    public boolean isActive() {
        return isActive;
    }

    public void setActive(boolean isActive) {
        this.isActive = isActive;
    }

    public String getAgentServer() {
        return agentServer;
    }

    public void setAgentServer(String agentServer) {
        this.agentServer = agentServer;
    }
}
