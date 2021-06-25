package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAvailabilityInfo extends CMEntity {

    @JsonProperty("databaseName")
    private String databaseName;

    @JsonProperty("replicaServerName")
    private String replicaServer;

    public String getDatabaseName() {
        return databaseName;
    }

    public void setDatabaseName(String databaseName) {
        this.databaseName = databaseName;
    }

    public String getReplicaServer() {
        return replicaServer;
    }

    public void setReplicaServer(String replicaServer) {
        this.replicaServer = replicaServer;
    }
}
