package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

public class CMAgentSettings {
    @JsonProperty("isDeployed")
    private boolean deployed;

    @JsonProperty("version")
    private String version;

    @JsonProperty("port")
    private int port;

    @JsonProperty("LastHeartbeatDateTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date lastHeartbeatDateTime;

    @JsonProperty("heartbeatInterval")
    private int heartbeatInterval;

    @JsonProperty("LoggingLevel")
    private int loggingLevel;

    public boolean isDeployed() {
        return deployed;
    }

    public void setDeployed(boolean deployed) {
        this.deployed = deployed;
    }

    public String getVersion() {
        return version;
    }

    public void setVersion(String version) {
        this.version = version;
    }

    public int getPort() {
        return port;
    }

    public void setPort(int port) {
        this.port = port;
    }

    public Date getLastHeartbeatDateTime() {
        return lastHeartbeatDateTime;
    }

    public void setLastHeartbeatDateTime(Date lastHeartbeatDateTime) {
        this.lastHeartbeatDateTime = lastHeartbeatDateTime;
    }

    public int getHeartbeatInterval() {
        return heartbeatInterval;
    }

    public void setHeartbeatInterval(int heartbeatInterval) {
        this.heartbeatInterval = heartbeatInterval;
    }

    public int getLoggingLevel() {
        return loggingLevel;
    }

    public void setLoggingLevel(int loggingLevel) {
        this.loggingLevel = loggingLevel;
    }
}
