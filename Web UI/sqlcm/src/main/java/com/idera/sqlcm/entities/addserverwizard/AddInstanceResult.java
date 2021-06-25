package com.idera.sqlcm.entities.addserverwizard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class AddInstanceResult {

    @JsonProperty("instance")
    private String instance;

    @JsonProperty("serverId")
    private long serverId;

    @JsonProperty("wasSuccessfullyAdded")
    private boolean wasSuccessfullyAdded;

    @JsonProperty("wasAgentDeployedAutomatically")
    private boolean wasAgentDeployedAutomatically;

    @JsonProperty("errorMessage")
    private String errorMessage;

    @JsonProperty("shouldIndexesToBeUpdated")
    private boolean shouldIndexesToBeUpdated;

    public String getInstance() {
        return instance;
    }

    public void setInstance(String instance) {
        this.instance = instance;
    }

    public long getServerId() {
        return serverId;
    }

    public void setServerId(long serverId) {
        this.serverId = serverId;
    }

    public boolean getWasSuccessfullyAdded() {
        return wasSuccessfullyAdded;
    }

    public void setWasSuccessfullyAdded(boolean wasSuccessfullyAdded) {
        this.wasSuccessfullyAdded = wasSuccessfullyAdded;
    }

    public String getErrorMessage() {
        return errorMessage;
    }

    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage;
    }

    public boolean isShouldIndexesToBeUpdated() {
        return shouldIndexesToBeUpdated;
    }

    public void setShouldIndexesToBeUpdated(boolean shouldIndexesToBeUpdated) {
        this.shouldIndexesToBeUpdated = shouldIndexesToBeUpdated;
    }

    public boolean getWasAgentDeployedAutomatically() {
        return wasAgentDeployedAutomatically;
    }

    public void setWasAgentDeployedAutomatically(boolean wasAgentDeployedAutomatically) {
        this.wasAgentDeployedAutomatically = wasAgentDeployedAutomatically;
    }
}
