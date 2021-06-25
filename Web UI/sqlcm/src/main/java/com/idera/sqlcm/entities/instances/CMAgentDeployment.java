package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMAgentDeployment {
    @JsonProperty("serviceAccount")
    private String serviceAccount;

    @JsonProperty("wasManuallyDeployed")
    private boolean wasManuallyDeployed;

    public String getServiceAccount() {
        return serviceAccount;
    }

    public void setServiceAccount(String serviceAccount) {
        this.serviceAccount = serviceAccount;
    }

    public boolean isWasManuallyDeployed() {
        return wasManuallyDeployed;
    }

    public void setWasManuallyDeployed(boolean wasManuallyDeployed) {
        this.wasManuallyDeployed = wasManuallyDeployed;
    }
}
