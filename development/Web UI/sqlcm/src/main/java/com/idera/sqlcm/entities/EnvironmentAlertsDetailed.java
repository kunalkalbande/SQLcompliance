package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class EnvironmentAlertsDetailed {

    private EnvironmentAlerts alertsStatus;

    private long auditedInstanceCount;

    private long auditedDatabaseCount;

    public EnvironmentAlerts getAlertsStatus() {
        return alertsStatus;
    }

    public void setAlertsStatus(EnvironmentAlerts alertsStatus) {
        this.alertsStatus = alertsStatus;
    }

    public long getAuditedInstanceCount() {
        return auditedInstanceCount;
    }

    public void setAuditedInstanceCount(long auditedInstanceCount) {
        this.auditedInstanceCount = auditedInstanceCount;
    }

    public long getAuditedDatabaseCount() {
        return auditedDatabaseCount;
    }

    public void setAuditedDatabaseCount(long auditedDatabaseCount) {
        this.auditedDatabaseCount = auditedDatabaseCount;
    }
}
