package com.idera.sqlcm.entities.addserverwizard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class ServerRegisteredStatusInfoRequestArg {

    @JsonProperty("instance")
    private String instance;

    public ServerRegisteredStatusInfoRequestArg(String instance) {
        this.instance = instance;
    }

    public String getInstance() {
        return instance;
    }

    public void setInstance(String instance) {
        this.instance = instance;
    }
}
