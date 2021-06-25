package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditedPrivilegedUserActivities {
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
    private boolean isAgentVersionSupported;
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
    
    // SQLCm 5.4_4.1.1_Extended Events Start
    @JsonProperty("auditUserExtendedEvents")
    private boolean auditUserExtendedEvents;
    

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
        return isAgentVersionSupported;
    }

    public void setAgentVersionSupported(boolean isAgentVersionSupported) {
        this.isAgentVersionSupported = isAgentVersionSupported;
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
    
    // SQLCm 5.4_4.1.1_Extended Events Start
    public boolean isAuditUserExtendedEvents() {
		return auditUserExtendedEvents;
	}

	public void setAuditUserExtendedEvents(boolean auditUserExtendedEvents) {
		this.auditUserExtendedEvents = auditUserExtendedEvents;
	}
    // SQLCm 5.4_4.1.1_Extended Events End

    public boolean isAuditUserCaptureDDL() {
        return auditUserCaptureDDL;
    }

    public void setAuditUserCaptureDDL(boolean auditUserCaptureDDL) {
        this.auditUserCaptureDDL = auditUserCaptureDDL;
    }
}
