package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMRemoveInstanceResponse {
    @JsonProperty("serverId")
    private long serverId;

    @JsonProperty("wasRemoved")
    private boolean wasRemoved;

    @JsonProperty("errorMessage")
    private String errorMessage;

    @JsonProperty("wasAgentDeactivated")
    private boolean wasAgentDeactivated;

    public long getServerId() {
        return serverId;
    }

    public void setServerId(long serverId) {
        this.serverId = serverId;
    }

    public boolean isWasRemoved() {
        return wasRemoved;
    }

    public void setWasRemoved(boolean wasRemoved) {
        this.wasRemoved = wasRemoved;
    }

    public String getErrorMessage() {
        return errorMessage;
    }

    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage;
    }

    public boolean isWasAgentDeactivated() {
        return wasAgentDeactivated;
    }

    public void setWasAgentDeactivated(boolean wasAgentDeactivated) {
        this.wasAgentDeactivated = wasAgentDeactivated;
    }
}
