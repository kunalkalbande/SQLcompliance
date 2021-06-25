package com.idera.sqlcm.entities.addserverwizard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class InstanceAvailableResult {

    @JsonProperty("isAvailable")
    private boolean isAvailable;

    @JsonProperty("errorMessage")
    private String errorMessage;

    public boolean isAvailable() {
        return isAvailable;
    }

    public void setAvailable(boolean isAvailable) {
        this.isAvailable = isAvailable;
    }

    public String getErrorMessage() {
        return errorMessage;
    }

    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage;
    }
}
