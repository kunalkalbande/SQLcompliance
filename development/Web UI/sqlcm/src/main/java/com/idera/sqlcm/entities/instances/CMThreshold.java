package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMThreshold {
    @JsonProperty("serverId")
    private long serverId;
    @JsonProperty("statisticCategory")
    private int statisticCategory;
    @JsonProperty("warningThreshold")
    private long warningThreshold;
    @JsonProperty("criticalThreshold")
    private long criticalThreshold;
    @JsonProperty("period")
    private int period;
    @JsonProperty("enabled")
    private boolean enabled;

    public long getServerId() {
        return serverId;
    }

    public void setServerId(long serverId) {
        this.serverId = serverId;
    }

    public int getStatisticCategory() {
        return statisticCategory;
    }

    public void setStatisticCategory(int statisticCategory) {
        this.statisticCategory = statisticCategory;
    }

    public long getWarningThreshold() {
        return warningThreshold;
    }

    public void setWarningThreshold(long warningThreshold) {
        this.warningThreshold = warningThreshold;
    }

    public long getCriticalThreshold() {
        return criticalThreshold;
    }

    public void setCriticalThreshold(long criticalThreshold) {
        this.criticalThreshold = criticalThreshold;
    }

    public int getPeriod() {
        return period;
    }

    public void setPeriod(int period) {
        this.period = period;
    }

    public boolean isEnabled() {
        return enabled;
    }

    public void setEnabled(boolean enabled) {
        this.enabled = enabled;
    }
}
