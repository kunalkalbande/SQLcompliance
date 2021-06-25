package com.idera.sqlcm.data.dialogs;

import com.fasterxml.jackson.annotation.JsonProperty;

public class DiscoveredInstance{

    @JsonProperty("UserName")
    private String userName;

    @JsonProperty("Password")
    private String password;

    @JsonProperty("SecurityModel")
    private int securityModel;

    @JsonProperty("WmiUser")
    private String wmiUser;

    @JsonProperty("WmiPassword")
    private String wmiPassword;

    @JsonProperty("ServerName")
    private String serverName;
    
    @JsonProperty("ServerId")
    private int serverId;
    
    @JsonProperty("InstanceName")
    private String instanceName;
    
    @JsonProperty("InstanceId")
    private int instanceId;
    
    public DiscoveredInstance() {
    }

    public DiscoveredInstance(String userName, String password, int securityModel, String wmiUser, String wmiPassword, String instanceName) {
        this.userName = userName;
        this.password = password;
        this.securityModel = securityModel;
        this.wmiUser = wmiUser;
        this.wmiPassword = wmiPassword;
        this.instanceName = instanceName;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public int getSecurityModel() {
        return securityModel;
    }

    public void setSecurityModel(int securityModel) {
        this.securityModel = securityModel;
    }

    public String getWmiUser() {
        return wmiUser;
    }

    public void setWmiUser(String wmiUser) {
        this.wmiUser = wmiUser;
    }

    public String getWmiPassword() {
        return wmiPassword;
    }

    public void setWmiPassword(String wmiPassword) {
        this.wmiPassword = wmiPassword;
    }

    public String getServerName() {
        return serverName;
    }

    public void setServerName(String serverName) {
        this.serverName = serverName;
    }

    public String getInstanceName() {
        return instanceName;
    }

    public void setInstanceName(String instanceName) {
        this.instanceName = instanceName;
    }

    public int getServerId() {
        return serverId;
    }

    public void setServerId(int serverId) {
        this.serverId = serverId;
    }

    public int getInstanceId() {
        return instanceId;
    }

    public void setInstanceId(int instanceId) {
        this.instanceId = instanceId;
    }
}
