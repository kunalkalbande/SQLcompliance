package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class RegulationSectionValue {

    private String name;
    private String serverEvents;
    private String databaseEvents;

    @JsonProperty("name")
    public String getName() {
        return name;
    }

    @JsonProperty("name")
    public void setName(String name) {
        this.name = name;
    }

    @JsonProperty("serverEvents")
    public String getServerEvents() {
        return serverEvents;
    }

    @JsonProperty("serverEvents")
    public void setServerEvents(String serverEvents) {
        this.serverEvents = serverEvents;
    }

    @JsonProperty("databaseEvents")
    public String getDatabaseEvents() {
        return databaseEvents;
    }

    @JsonProperty("databaseEvents")
    public void setDatabaseEvents(String databaseEvents) {
        this.databaseEvents = databaseEvents;
    }

}


