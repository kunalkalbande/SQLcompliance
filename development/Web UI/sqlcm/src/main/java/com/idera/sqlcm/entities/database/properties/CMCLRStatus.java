package com.idera.sqlcm.entities.database.properties;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Objects;


@JsonIgnoreProperties(ignoreUnknown = true)
public class CMCLRStatus {

    @JsonProperty("isConfigured")
    private boolean configured;

    @JsonProperty("isRunning")
    private boolean running;

    @JsonProperty("enable")
    private boolean enable;

    @JsonProperty("serverId")
    private Long serverId;

    @JsonProperty("statusMessage")
    private String statusMessage;

    public boolean isConfigured() {
        return configured;
    }

    public void setConfigured(boolean configured) {
        this.configured = configured;
    }

    public boolean isRunning() {
        return running;
    }

    public void setRunning(boolean running) {
        this.running = running;
    }

    public boolean isEnable() {
        return enable;
    }

    public void setEnable(boolean enable) {
        this.enable = enable;
    }

    public Long getServerId() {
        return serverId;
    }

    public void setServerId(Long serverId) {
        this.serverId = serverId;
    }

    public String getStatusMessage() {
        return statusMessage;
    }

    public void setStatusMessage(String statusMessage) {
        this.statusMessage = statusMessage;
    }

    @Override
    public String toString() {
        return Objects.toStringHelper(this)
                      .add("configured", configured)
                      .add("running", running)
                      .add("enable", enable)
                      .add("serverId", serverId)
                      .add("statusMessage", statusMessage)
                      .toString();
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        CMCLRStatus that = (CMCLRStatus) o;
        return Objects.equal(this.configured, that.configured) &&
                Objects.equal(this.running, that.running) &&
                Objects.equal(this.enable, that.enable) &&
                Objects.equal(this.serverId, that.serverId) &&
                Objects.equal(this.statusMessage, that.statusMessage);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(configured, running, enable, serverId, statusMessage);
    }
}
