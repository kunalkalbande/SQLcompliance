package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMManagedInstance extends CMEntity {
    private String instance;
    private CMManageInstanceCredentials credentials;

    private String comments;
    private String location;
    private String owner;
    private CMDataCollectionSettings dataCollectionSettings;

    public String getInstance() {
        return instance;
    }

    public void setInstance(String instance) {
        this.instance = instance;
    }

    public CMManageInstanceCredentials getCredentials() {
        return credentials;
    }

    public void setCredentials(CMManageInstanceCredentials credentials) {
        this.credentials = credentials;
    }

    public String getComments() {
        return comments;
    }

    public void setComments(String comments) {
        this.comments = comments;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    public String getOwner() {
        return owner;
    }

    public void setOwner(String owner) {
        this.owner = owner;
    }

    public CMDataCollectionSettings getDataCollectionSettings() {
        return dataCollectionSettings;
    }

    public void setDataCollectionSettings(CMDataCollectionSettings dataCollectionSettings) {
        this.dataCollectionSettings = dataCollectionSettings;
    }
}
