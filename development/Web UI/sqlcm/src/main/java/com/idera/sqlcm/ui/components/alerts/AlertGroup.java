package com.idera.sqlcm.ui.components.alerts;

import com.google.common.base.Objects;
import com.idera.sqlcm.entities.CMAlert;

public class AlertGroup {

    private AlertType alertType;
    private AlertLevel alertLevel;
    private long alertsCount;

    public AlertType getAlertType() {
        return alertType;
    }

    public AlertLevel getAlertLevel() {
        return alertLevel;
    }

    public long getAlertsCount() {
        return alertsCount;
    }

    public AlertGroup(CMAlert alert) {
        this(alert.getAlertType(), alert.getAlertLevel(), 0);
    }

    public AlertGroup(int alertTypeId, int alertLevelId) {
        this.alertType = AlertType.getById(alertTypeId);
        this.alertLevel = AlertLevel.getById(alertLevelId);
    }

    public AlertGroup(int alertTypeId, int alertLevelId, long alertsCount) {
        this.alertType = AlertType.getById(alertTypeId);
        this.alertLevel = AlertLevel.getById(alertLevelId);
        this.alertsCount = alertsCount;
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(alertType, alertLevel);
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        AlertGroup that = (AlertGroup) o;
        return Objects.equal(this.alertType, that.alertType) && Objects.equal(this.alertLevel, that.alertLevel);
    }

    @Override
    public String toString() {
        return Objects.toStringHelper(this).add("alertType", alertType).add("alertLevel", alertLevel).toString();
    }
}