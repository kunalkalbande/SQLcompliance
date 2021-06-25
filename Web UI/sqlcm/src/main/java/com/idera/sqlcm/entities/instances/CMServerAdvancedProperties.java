package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMServerAdvancedProperties {
    @JsonProperty("defaultDatabasePermissions")
    private int defaultDatabasePermissions;
    @JsonProperty("sqlStatementLimit")
    private int sqlStatementLimit;

    public int getDefaultDatabasePermissions() {
        return defaultDatabasePermissions;
    }

    public void setDefaultDatabasePermissions(int defaultDatabasePermissions) {
        this.defaultDatabasePermissions = defaultDatabasePermissions;
    }

    public int getSqlStatementLimit() {
        return sqlStatementLimit;
    }

    public void setSqlStatementLimit(int sqlStatementLimit) {
        this.sqlStatementLimit = sqlStatementLimit;
    }
}
