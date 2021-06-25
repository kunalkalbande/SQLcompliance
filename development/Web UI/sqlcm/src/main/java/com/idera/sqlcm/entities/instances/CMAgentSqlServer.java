package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMAgentSqlServer {
    @JsonProperty("instance")
    private String instance;

    @JsonProperty("description")
    private String description;

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
}
