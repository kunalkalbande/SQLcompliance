package com.idera.sqlcm.entities;


import com.fasterxml.jackson.annotation.JsonProperty;

public class CMDmlSelectFilters {

    @JsonProperty("auditDmlAll")
    private boolean auditDmlAll;

    @JsonProperty("auditUserTables")
    private int auditUserTables;

    @JsonProperty("userTableList")
    private Object userTableList;

    @JsonProperty("auditSystemTables")
    private boolean auditSystemTables;

    @JsonProperty("auditStoredProcedures")
    private boolean auditStoredProcedures;

    @JsonProperty("auditDmlOther")
    private boolean auditDmlOther;

    public boolean isAuditDmlAll() {
        return auditDmlAll;
    }

    public void setAuditDmlAll(boolean auditDmlAll) {
        this.auditDmlAll = auditDmlAll;
    }

    public int getAuditUserTables() {
        return auditUserTables;
    }

    public void setAuditUserTables(int auditUserTables) {
        this.auditUserTables = auditUserTables;
    }

    public Object getUserTableList() {
        return userTableList;
    }

    public void setUserTableList(Object userTableList) {
        this.userTableList = userTableList;
    }

    public boolean isAuditSystemTables() {
        return auditSystemTables;
    }

    public void setAuditSystemTables(boolean auditSystemTables) {
        this.auditSystemTables = auditSystemTables;
    }

    public boolean isAuditStoredProcedures() {
        return auditStoredProcedures;
    }

    public void setAuditStoredProcedures(boolean auditStoredProcedures) {
        this.auditStoredProcedures = auditStoredProcedures;
    }

    public boolean isAuditDmlOther() {
        return auditDmlOther;
    }

    public void setAuditDmlOther(boolean auditDmlOther) {
        this.auditDmlOther = auditDmlOther;
    }
}
