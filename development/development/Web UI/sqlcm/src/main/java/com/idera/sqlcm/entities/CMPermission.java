package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMPermission extends CMEntity {

    @JsonProperty("status")
    private int status;

    @JsonProperty("resolutionSteps")
    private String resolutionSteps;

    public int getStatus() {
        return status;
    }

    public void setStatus(int status) {
        this.status = status;
    }

    public String getResolutionSteps() {
        return resolutionSteps;
    }

    public void setResolutionSteps(String resolutionSteps) {
        this.resolutionSteps = resolutionSteps;
    }
}
