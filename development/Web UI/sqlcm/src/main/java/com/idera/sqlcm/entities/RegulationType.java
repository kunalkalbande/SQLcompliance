package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class RegulationType {

    @JsonProperty("type")
    private long type;

    @JsonProperty("name")
    private String name;

    @JsonProperty("description")
    private String description;

    public long getType() {
        return type;
    }

    public void setType(long type) {
        this.type = type;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }
}