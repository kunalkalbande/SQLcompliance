package com.idera.sqlcm.ui.auditReports;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;


@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditDML {

    @JsonProperty("instance")
    private String instance;

    @JsonProperty("database")
    private String database;

    @JsonProperty("loginName")
    private String loginName;

    @JsonProperty("objectName")
    private String objectName;
    
    @JsonProperty("schemaName")
    private String schemaName;

    @JsonProperty("to")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date to;
    
    @JsonProperty("from")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date from;

    @JsonProperty("sql")
    private int sql;

    @JsonProperty("user")
    private int user;

    @JsonProperty("key")
    private String key;
    
    @JsonProperty("sortColumn")
    private String sortColumn;
    
    @JsonProperty("rowCount")
    private int rowCount;
    
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

	public String getLoginName() {
		return loginName;
	}

	public void setLoginName(String loginName) {
		this.loginName = loginName;
	}

	public String getObjectName() {
		return objectName;
	}

	public void setObjectName(String objectName) {
		this.objectName = objectName;
	}

	public String getSchemaName() {
		return schemaName;
	}

	public void setSchemaName(String schemaName) {
		this.schemaName = schemaName;
	}

	public int getSql() {
		return sql;
	}

	public void setSql(int sql) {
		this.sql = sql;
	}

	public int getUser() {
		return user;
	}

	public void setUser(int user) {
		this.user = user;
	}

	public String getKey() {
		return key;
	}

	public void setKey(String key) {
		this.key = key;
	}

	public Date getTo() {
		return to;
	}

	public void setTo(Date to) {
		this.to = to;
	}

	public String getSortColumn() {
		return sortColumn;
	}

	public void setSortColumn(String sortColumn) {
		this.sortColumn = sortColumn;
	}

	public int getRowCount() {
		return rowCount;
	}

	public void setRowCount(int rowCount) {
		this.rowCount = rowCount;
	}

	public Date getFrom() {
		return from;
	}

	public void setFrom(Date from) {
		this.from = from;
	}

	@Override
    public String toString() {
        return "UpdateSNMPConfiguration{" +
            "instance=" + instance +
            ", database=" + database +
            ", loginName=" + loginName +
            ", objectName=" + objectName +
            ", schemaName=" + schemaName +
            ", to=" + to +
            ", from=" + from +
            ", sql=" + sql +
            ", user=" + user +
            ", key=" + key +
            '}';
    }
}
