package com.idera.sqlcm.ui.components.alerts;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Objects;
import com.idera.sqlcm.entities.CMAlert;

public class AlertGroupDTO {

    @JsonProperty("alertType")
    private int alertType;

    @JsonProperty("alertLevel")
    private int alertLevel;

    @JsonProperty("alertsCount")
    private long alertsCount;

    public int getAlertType() {
        return alertType;
    }

    public void setAlertType(int alertType) {
        this.alertType = alertType;
    }

    public int getAlertLevel() {
        return alertLevel;
    }

    public void setAlertLevel(int alertLevel) {
        this.alertLevel = alertLevel;
    }

    public long getAlertsCount() {
        return alertsCount;
    }

    public void setAlertsCount(long alertsCount) {
        this.alertsCount = alertsCount;
    }


}