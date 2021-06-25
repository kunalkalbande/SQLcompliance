package com.idera.sqlcm.ui.preferences;

import com.google.common.base.Objects;

public class AlertsPreferencesBean {

    public final static String SESSION_VARIABLE_NAME = "AlertsPreferencesBeanSQLCM";

    private boolean alertsHidden;
    private boolean alertsGrouped;
    private boolean alertsShowAll;

    public boolean isAlertsHidden() {
        return alertsHidden;
    }

    public void setAlertsHidden(boolean alertsHidden) {
        this.alertsHidden = alertsHidden;
    }

    public boolean isAlertsGrouped() {
        return alertsGrouped;
    }

    public void setAlertsGrouped(boolean alertsGrouped) {
        this.alertsGrouped = alertsGrouped;
    }

    public boolean isAlertsShowAll() {
        return alertsShowAll;
    }

    public void setAlertsShowAll(boolean alertsShowAll) {
        this.alertsShowAll = alertsShowAll;
    }

    @Override
    public String toString() {
        return Objects.toStringHelper(this)
                      .add("SESSION_VARIABLE_NAME", SESSION_VARIABLE_NAME)
                      .add("alertsHidden", alertsHidden)
                      .add("alertsGrouped", alertsGrouped)
                      .add("alertsShowAll", alertsShowAll)
                      .toString();
    }
}
