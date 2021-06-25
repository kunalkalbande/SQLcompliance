package com.idera.sqlcm.ui.components.alerts;

import com.google.common.collect.Ordering;
import com.google.common.primitives.Ints;
import com.idera.sqlcm.entities.CMAlert;

import java.util.List;

public class AlertMetric {

    private static Ordering<CMAlert> o = new Ordering<CMAlert>() {
        @Override
        public int compare(CMAlert left, CMAlert right) {
            return Ints.compare(AlertLevel.getById(left.getAlertLevel()).getLevel(), AlertLevel.getById(right.getAlertLevel()).getLevel());
        }
    };

    private AlertLevel level;
    private AlertGroup alertGroup;
    private List<CMAlert> alerts;
    private boolean detailsVisible = false;
    private boolean refreshing = false;
    private String titleMessage;
    private AlertPager alertPager;
    private long instanceId;

    public AlertMetric(AlertGroup alertGroup) {
        this(alertGroup, null, Long.MIN_VALUE);
    }

    public AlertMetric(AlertGroup alertGroup, List<CMAlert> alerts, long instanceId) {
        this.alertGroup = alertGroup;
        this.alerts = alerts;
        this.instanceId = instanceId;
        this.setLevel(AlertLevel.getById(o.max(alerts).getAlertLevel()));
        this.titleMessage = alertGroup.getAlertsCount() + " " +
            (alertGroup.getAlertsCount() == 1 ? alertGroup.getAlertType().getLabel() : alertGroup.getAlertType().getLabelPlural());
        this.alertPager = new AlertPager(alertGroup, instanceId, alerts);
    }

    public AlertLevel getLevel() {
        return (level == null) ? AlertLevel.LOW : level;
    }

    public void setLevel(AlertLevel level) {
        this.level = level;
    }

    public AlertGroup getAlertGroup() {
        return alertGroup;
    }

    public List<CMAlert> getAlerts() {
        return alerts;
    }

    public boolean isRefreshing() {
        return refreshing;
    }

    public void setRefreshing(boolean refreshing) {
        this.refreshing = refreshing;
    }

    public boolean isDetailsVisible() {
        return detailsVisible;
    }

    public void setDetailsVisible(boolean detailsVisible) {
        this.detailsVisible = detailsVisible;
    }

    public String getTitleMessage() {
        return titleMessage;
    }

    public AlertPager getAlertPager() {
        return alertPager;
    }

}
