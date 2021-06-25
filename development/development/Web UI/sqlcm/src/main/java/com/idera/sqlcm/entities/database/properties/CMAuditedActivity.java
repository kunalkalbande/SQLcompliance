package com.idera.sqlcm.entities.database.properties;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Objects;


@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditedActivity {

    @JsonProperty("auditDDL")
    private boolean auditDDL;

    @JsonProperty("auditSecurity")
    private boolean auditSecurity;

    @JsonProperty("auditAdmin")
    private boolean auditAdmin;

    @JsonProperty("auditDML")
    private boolean auditDML;

    @JsonProperty("auditSELECT")
    private boolean auditSELECT;

    @JsonProperty("auditCaptureSQL")
    private boolean auditCaptureSQL;

    @JsonProperty("auditCaptureTrans")
    private boolean auditCaptureTrans;

    @JsonProperty("allowCaptureSql")
    private boolean allowCaptureSql;

    @JsonProperty("isAgentVersionSupported")
    private boolean agentVersionSupported;

    @JsonProperty("auditAccessCheck")
    private int auditAccessCheck;

    @JsonProperty("auditAllUserActivities")
    private boolean auditAllUserActivities;

    @JsonProperty("auditLogins")
    private boolean auditLogins;

    @JsonProperty("auditFailedLogins")
    private boolean auditFailedLogins;

    @JsonProperty("auditDefinedEvents")
    private boolean auditDefinedEvents;

    @JsonProperty("auditUserCaptureDDL")
    private boolean auditUserCaptureDDL;

    public boolean isAuditDDL() {
        return auditDDL;
    }

    public void setAuditDDL(boolean auditDDL) {
        this.auditDDL = auditDDL;
    }

    public boolean isAuditSecurity() {
        return auditSecurity;
    }

    public void setAuditSecurity(boolean auditSecurity) {
        this.auditSecurity = auditSecurity;
    }

    public boolean isAuditAdmin() {
        return auditAdmin;
    }

    public void setAuditAdmin(boolean auditAdmin) {
        this.auditAdmin = auditAdmin;
    }

    public boolean isAuditDML() {
        return auditDML;
    }

    public void setAuditDML(boolean auditDML) {
        this.auditDML = auditDML;
    }

    public boolean isAuditSELECT() {
        return auditSELECT;
    }

    public void setAuditSELECT(boolean auditSELECT) {
        this.auditSELECT = auditSELECT;
    }

    public boolean isAuditCaptureSQL() {
        return auditCaptureSQL;
    }

    public void setAuditCaptureSQL(boolean auditCaptureSQL) {
        this.auditCaptureSQL = auditCaptureSQL;
    }

    public boolean isAuditCaptureTrans() {
        return auditCaptureTrans;
    }

    public void setAuditCaptureTrans(boolean auditCaptureTrans) {
        this.auditCaptureTrans = auditCaptureTrans;
    }

    public boolean isAllowCaptureSql() {
        return allowCaptureSql;
    }

    public void setAllowCaptureSql(boolean allowCaptureSql) {
        this.allowCaptureSql = allowCaptureSql;
    }

    public boolean isAgentVersionSupported() {
        return agentVersionSupported;
    }

    public void setAgentVersionSupported(boolean agentVersionSupported) {
        this.agentVersionSupported = agentVersionSupported;
    }

    public int getAuditAccessCheck() {
        return auditAccessCheck;
    }

    public void setAuditAccessCheck(int auditAccessCheck) {
        this.auditAccessCheck = auditAccessCheck;
    }

    public boolean isAuditAllUserActivities() {
        return auditAllUserActivities;
    }

    public void setAuditAllUserActivities(boolean auditAllUserActivities) {
        this.auditAllUserActivities = auditAllUserActivities;
    }

    public boolean isAuditLogins() {
        return auditLogins;
    }

    public void setAuditLogins(boolean auditLogins) {
        this.auditLogins = auditLogins;
    }

    public boolean isAuditFailedLogins() {
        return auditFailedLogins;
    }

    public void setAuditFailedLogins(boolean auditFailedLogins) {
        this.auditFailedLogins = auditFailedLogins;
    }

    public boolean isAuditDefinedEvents() {
        return auditDefinedEvents;
    }

    public void setAuditDefinedEvents(boolean auditDefinedEvents) {
        this.auditDefinedEvents = auditDefinedEvents;
    }

    public boolean isAuditUserCaptureDDL() {
        return auditUserCaptureDDL;
    }

    public void setAuditUserCaptureDDL(boolean auditUserCaptureDDL) {
        this.auditUserCaptureDDL = auditUserCaptureDDL;
    }

    @Override
    public String toString() {
        return Objects.toStringHelper(this)
                      .add("auditDDL", auditDDL)
                      .add("auditSecurity", auditSecurity)
                      .add("auditAdmin", auditAdmin)
                      .add("auditDML", auditDML)
                      .add("auditSELECT", auditSELECT)
                      .add("auditCaptureSQL", auditCaptureSQL)
                      .add("auditCaptureTrans", auditCaptureTrans)
                      .add("allowCaptureSql", allowCaptureSql)
                      .add("agentVersionSupported", agentVersionSupported)
                      .add("auditAccessCheck", auditAccessCheck)
                      .add("auditAllUserActivities", auditAllUserActivities)
                      .add("auditLogins", auditLogins)
                      .add("auditFailedLogins", auditFailedLogins)
                      .add("auditDefinedEvents", auditDefinedEvents)
                      .toString();
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        CMAuditedActivity that = (CMAuditedActivity) o;
        return Objects.equal(this.auditDDL, that.auditDDL) &&
                Objects.equal(this.auditSecurity, that.auditSecurity) &&
                Objects.equal(this.auditAdmin, that.auditAdmin) &&
                Objects.equal(this.auditDML, that.auditDML) &&
                Objects.equal(this.auditSELECT, that.auditSELECT) &&
                Objects.equal(this.auditCaptureSQL, that.auditCaptureSQL) &&
                Objects.equal(this.auditCaptureTrans, that.auditCaptureTrans) &&
                Objects.equal(this.allowCaptureSql, that.allowCaptureSql) &&
                Objects.equal(this.agentVersionSupported, that.agentVersionSupported) &&
                Objects.equal(this.auditAccessCheck, that.auditAccessCheck) &&
                Objects.equal(this.auditAllUserActivities, that.auditAllUserActivities) &&
                Objects.equal(this.auditLogins, that.auditLogins) &&
                Objects.equal(this.auditFailedLogins, that.auditFailedLogins) &&
                Objects.equal(this.auditDefinedEvents, that.auditDefinedEvents);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(auditDDL, auditSecurity, auditAdmin, auditDML, auditSELECT, auditCaptureSQL,
                auditCaptureTrans, allowCaptureSql, agentVersionSupported, auditAccessCheck, auditAllUserActivities,
                auditLogins, auditFailedLogins, auditDefinedEvents);
    }
}
