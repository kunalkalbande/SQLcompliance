package com.idera.sqlcm.entities.database.properties;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Objects;
import com.idera.sqlcm.entities.CMTable;

import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMDMLSelectFilter {

    @JsonProperty("auditDmlAll")
    private boolean auditDmlAll;

    @JsonProperty("auditSystemTables")
    private boolean auditSystemTables;

    @JsonProperty("auditStoredProcedures")
    private boolean auditStoredProcedures;

    @JsonProperty("auditDmlOther")
    private boolean auditDmlOther;

    @JsonProperty("auditUserTables")
    private int auditUserTables;

    @JsonProperty("userTableList")
    private List<CMTable> userTableList;

    public boolean isAuditDmlAll() {
        return auditDmlAll;
    }

    public void setAuditDmlAll(boolean auditDmlAll) {
        this.auditDmlAll = auditDmlAll;
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

    public int getAuditUserTables() {
        return auditUserTables;
    }

    public void setAuditUserTables(int auditUserTables) {
        this.auditUserTables = auditUserTables;
    }

    public List<CMTable> getUserTableList() {
        return userTableList;
    }

    public void setUserTableList(List<CMTable> userTableList) {
        this.userTableList = userTableList;
    }

    @Override
    public String toString() {
        return Objects.toStringHelper(this)
                      .add("auditDmlAll", auditDmlAll)
                      .add("auditSystemTables", auditSystemTables)
                      .add("auditStoredProcedures", auditStoredProcedures)
                      .add("auditDmlOther", auditDmlOther)
                      .add("auditUserTables", auditUserTables)
                      .add("userTableList", userTableList)
                      .toString();
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        CMDMLSelectFilter that = (CMDMLSelectFilter) o;
        return Objects.equal(this.auditDmlAll, that.auditDmlAll) &&
                Objects.equal(this.auditSystemTables, that.auditSystemTables) &&
                Objects.equal(this.auditStoredProcedures, that.auditStoredProcedures) &&
                Objects.equal(this.auditDmlOther, that.auditDmlOther) &&
                Objects.equal(this.auditUserTables, that.auditUserTables) &&
                Objects.equal(this.userTableList, that.userTableList);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(auditDmlAll, auditSystemTables, auditStoredProcedures, auditDmlOther, auditUserTables, userTableList);
    }
}
