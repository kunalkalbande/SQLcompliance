package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMAgentTraceOptions {
    @JsonProperty("agentTraceDirectory")
    private String agentTraceDirectory;

    @JsonProperty("traceFileRolloverSize")
    private int traceFileRolloverSize;

    @JsonProperty("collectionInterval")
    private int collectionInterval;

    @JsonProperty("forceCollectionInterval")
    private int forceCollectionInterval;

    @JsonProperty("traceStartTimeoutEnabled")
    private boolean traceStartTimeoutEnabled;

    @JsonProperty("traceStartTimeout")
    private int traceStartTimeout;

    @JsonProperty("temperDetectionIntervalEnabled")
    private boolean temperDetectionIntervalEnabled;

    @JsonProperty("temperDetectionInterval")
    private int temperDetectionInterval;

    @JsonProperty("traceDirectorySizeLimit")
    private int traceDirectorySizeLimit;

    @JsonProperty("unattendedTimeLimit")
    private int unattendedTimeLimit;

    public String getAgentTraceDirectory() {
        return agentTraceDirectory;
    }

    public void setAgentTraceDirectory(String agentTraceDirectory) {
        this.agentTraceDirectory = agentTraceDirectory;
    }

    public int getTraceFileRolloverSize() {
        return traceFileRolloverSize;
    }

    public void setTraceFileRolloverSize(int traceFileRolloverSize) {
        this.traceFileRolloverSize = traceFileRolloverSize;
    }

    public int getCollectionInterval() {
        return collectionInterval;
    }

    public void setCollectionInterval(int collectionInterval) {
        this.collectionInterval = collectionInterval;
    }

    public int getForceCollectionInterval() {
        return forceCollectionInterval;
    }

    public void setForceCollectionInterval(int forceCollectionInterval) {
        this.forceCollectionInterval = forceCollectionInterval;
    }

    public boolean isTraceStartTimeoutEnabled() {
        return traceStartTimeoutEnabled;
    }

    public void setTraceStartTimeoutEnabled(boolean traceStartTimeoutEnabled) {
        this.traceStartTimeoutEnabled = traceStartTimeoutEnabled;
    }

    public int getTraceStartTimeout() {
        return traceStartTimeout;
    }

    public void setTraceStartTimeout(int traceStartTimeout) {
        this.traceStartTimeout = traceStartTimeout;
    }

    public boolean isTemperDetectionIntervalEnabled() {
        return temperDetectionIntervalEnabled;
    }

    public void setTemperDetectionIntervalEnabled(boolean temperDetectionIntervalEnabled) {
        this.temperDetectionIntervalEnabled = temperDetectionIntervalEnabled;
    }

    public int getTemperDetectionInterval() {
        return temperDetectionInterval;
    }

    public void setTemperDetectionInterval(int temperDetectionInterval) {
        this.temperDetectionInterval = temperDetectionInterval;
    }

    public int getTraceDirectorySizeLimit() {
        return traceDirectorySizeLimit;
    }

    public void setTraceDirectorySizeLimit(int traceDirectorySizeLimit) {
        this.traceDirectorySizeLimit = traceDirectorySizeLimit;
    }

    public int getUnattendedTimeLimit() {
        return unattendedTimeLimit;
    }

    public void setUnattendedTimeLimit(int unattendedTimeLimit) {
        this.unattendedTimeLimit = unattendedTimeLimit;
    }
}
