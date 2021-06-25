package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMArchive {

    @JsonProperty("instance")
    private String instance;

    @JsonProperty("databaseName")
    private String databaseName;

    public CMArchive() {
    }

    public CMArchive(String instance, String databaseName) {
        this.instance = instance;
        this.databaseName = databaseName;
    }

    public String getInstance() {
        return instance;
    }

    public void setInstance(String instance) {
        this.instance = instance;
    }

    public String getDatabaseName() {
        return databaseName;
    }

    public void setDatabaseName(String databaseName) {
        this.databaseName = databaseName;
    }

    @Override
    public String toString() {
        return "CMArchived{" +
            "instance='" + instance + '\'' +
            ", databaseName='" + databaseName + '\'' +
            '}';
    }
}
