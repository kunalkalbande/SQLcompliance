package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMArchivedDatabase extends CMArchive {
    
    @JsonProperty("displayName")
    private String displayName;

    @JsonProperty("description")
    private String description;

    public CMArchivedDatabase() {
    }

    public String getDisplayName() {
        return displayName;
    }

    public void setDisplayName(String displayName) {
        this.displayName = displayName;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    @Override
    public String toString() {
        return "CMArchivedDatabase{" +
            "displayName='" + displayName + '\'' +
            ", description='" + description + '\'' +
            '}';
    }
}
