package com.idera.sqlcm.ui.auditReports;

import java.sql.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;


@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditRowCount {

    @JsonProperty("instance")
    private String instance;

    @JsonProperty("database")
    private String database;

    @JsonProperty("objectName")
    private String objectName;

    @JsonProperty("columnName")
    private String columnName;

    @JsonProperty("privilegedUsers")
    private String privilegedUsers;

    @JsonProperty("rowCountThreshold")
    private int rowCountThreshold;

    @JsonProperty("loginName")
    private String loginName;

    @JsonProperty("to")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private java.util.Date to;

    @JsonProperty("from")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private java.util.Date from;

    @JsonProperty("sql")
    private int sql;
    
	@Override
    public String toString() {
        return "UpdateSNMPConfiguration{" +
            "instance=" + instance +
            ",loginName=" + loginName+
            ", database=" + database +
            ", objectName=" + objectName +
            ", columnName=" + columnName +
            ", to=" + to +
            ", from=" + from +
            ", sql=" + sql +
            ", privilegedUsers=" + privilegedUsers +
            ", rowCountThreshold=" + rowCountThreshold +
            '}';
    }

	public String getInstance() {
		return instance;
	}

	public void setInstance(String instance) {
		this.instance = instance;
	}

	public String getDatabase() {
		return database;
	}

	public void setDatabase(String database) {
		this.database = database;
	}

	public String getObjectName() {
		return objectName;
	}

	public void setObjectName(String objectName) {
		this.objectName = objectName;
	}

	public String getLoginName() {
		return loginName;
	}

	public void setLoginName(String loginName) {
		this.loginName = loginName;
	}

	public String getColumn(String columnName) {
		return columnName;
	}

	public void setColumn(String columnName) {
		this.columnName = columnName;
	}

	public String getPrivilegedUsers(String privilegedUsers) {
		return privilegedUsers;
	}

	public void setPrivilegedUsers(String privilegedUsers) {
		this.privilegedUsers = privilegedUsers;
	}

	public void setRowCountThreshold(int rowCountThreshold) {
		this.rowCountThreshold = rowCountThreshold;
	}

	public int getRowCountThreshold(int rowCountThreshold) {
		return rowCountThreshold;
	}

	public java.util.Date getTo() {
		return to;
	}

	public void setTo(java.util.Date to) {
		this.to = to;
	}

	public java.util.Date getFrom() {
		return from;
	}

	public void setFrom(java.util.Date from) {
		this.from = from;
	}

	public int getSql() {
		return sql;
	}

	public void setSql(int sql) {
		this.sql = sql;
	}
}


