package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class License {

    @JsonProperty("scope")
    private String type;

    @JsonProperty("state")
    private int status;

    private String expirationDate;

    private int licensedServers;

    private int monitoredServers;

    private String key;

    @JsonProperty("repositoryServer")
    private String repository;

    private String licenseType;

    private String licensedServerCount;

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public int getStatus() {
        return status;
    }

    public void setStatus(int status) {
        this.status = status;
    }

    public String getExpirationDate() {
        return expirationDate;
    }

    public void setExpirationDate(String expirationDate) {
        this.expirationDate = expirationDate;
    }

    public int getLicensedServers() {
        return licensedServers;
    }

    public void setLicensedServers(int licensedServers) {
        this.licensedServers = licensedServers;
    }

    public int getMonitoredServers() {
        return monitoredServers;
    }

    public void setMonitoredServers(int monitoredServers) {
        this.monitoredServers = monitoredServers;
    }

    public String getKey() {
        return key;
    }

    public void setKey(String key) {
        this.key = key;
    }

    public String getRepository() {
        return repository;
    }

    public void setRepository(String repository) {
        this.repository = repository;
    }

    public String getLicenseType() {
        return licenseType;
    }

    public void setLicenseType(String licenseType) {
        this.licenseType = licenseType;
    }

    public String getLicensedServerCount() {
        return licensedServerCount;
    }

    public void setLicensedServerCount(String licensedServerCount) {
        this.licensedServerCount = licensedServerCount;
    }
}
