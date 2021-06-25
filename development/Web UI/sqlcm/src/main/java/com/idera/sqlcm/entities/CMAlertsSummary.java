package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAlertsSummary {

    @JsonProperty("auditedInstances")
    CMAuditedAlerts auditedInstances;

    @JsonProperty("auditedDatabases")
    CMAuditedAlerts auditedDatabases;

    public CMAlertsSummary() {
    }

    public CMAuditedAlerts getAuditedInstances() {
        return auditedInstances;
    }

    public void setAuditedInstances(CMAuditedAlerts auditedInstances) {
        this.auditedInstances = auditedInstances;
    }

    public CMAuditedAlerts getAuditedDatabases() {
        return auditedDatabases;
    }

    public void setAuditedDatabases(CMAuditedAlerts auditedDatabases) {
        this.auditedDatabases = auditedDatabases;
    }

    @Override
    public String toString() {
        return "CMAlertsSummary{" +
            "auditedInstances=" + auditedInstances +
            ", auditedDatabases=" + auditedDatabases +
            '}';
    }
}
