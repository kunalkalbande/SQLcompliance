package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditedActivities {
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

    @JsonProperty("auditUserCaptureDDL")
    private boolean auditUserCaptureDDL;


    @JsonProperty("auditDefinedEvents")
    private boolean auditDefinedEvents;
    
    /*SQLCm 5.4_4.1.1_Extended Events B*/   
    @JsonProperty("auditCaptureSQLXE")
    private boolean auditCaptureSQLXE;
    
    //5.5 Audit Logs and Custom Regulation Guidelines
    @JsonProperty("isAuditLogEnabled")
    private boolean auditLogEnabled;

    @JsonProperty("auditBeforeAfter")
    private boolean auditBeforeAfter;
    
    @JsonProperty("auditSensitiveColumns")
    private boolean auditSensitiveColumns;
    
    @JsonProperty("auditPrivilegedUsers")
    private boolean auditPrivilegedUsers;
    
    @JsonProperty("customEnabled")
    private boolean customEnabled;

	public boolean isAuditBeforeAfter() {
		return auditBeforeAfter;
	}
	public void setAuditBeforeAfter(boolean auditBeforeAfter) {
		this.auditBeforeAfter = auditBeforeAfter;
	}
	public boolean isAuditSensitiveColumns() {
		return auditSensitiveColumns;
	}
	public void setAuditSensitiveColumns(boolean auditSensitiveColumns) {
		this.auditSensitiveColumns = auditSensitiveColumns;
	}
	public boolean isAuditPrivilegedUsers() {
		return auditPrivilegedUsers;
	}
	public void setAuditPrivilegedUsers(boolean auditPrivilegedUsers) {
		this.auditPrivilegedUsers = auditPrivilegedUsers;
	}
	
	public boolean isCustomEnabled() {
		return customEnabled;
	}
	public void setCustomEnabled(boolean customEnabled) {
		this.customEnabled = customEnabled;
	}
	public boolean getAuditLogEnabled() {
		return auditLogEnabled;
	}

	public void setAuditLogEnabled(boolean auditLogEnabled) {
		this.auditLogEnabled = auditLogEnabled;
	}

	public boolean isAuditCaptureSQLXE() {
		return auditCaptureSQLXE;
	}

	public void setAuditCaptureSQLXE(boolean auditCaptureSQLXE) {
		this.auditCaptureSQLXE = auditCaptureSQLXE;
	}

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

    public boolean isAuditUserCaptureDDL() {
        return auditUserCaptureDDL;
    }

    public void setAuditUserCaptureDDL(boolean auditUserCaptureDDL) {
        this.auditUserCaptureDDL = auditUserCaptureDDL;
    }
}
