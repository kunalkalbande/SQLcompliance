package com.idera.sqlcm.ui.auditReports;

import java.sql.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;


@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditLoginDeletion {

    @JsonProperty("instance")
    private String instance;

    @JsonProperty("login")
    private String login;

    @JsonProperty("from")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private java.util.Date from;

    @JsonProperty("to")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private java.util.Date to;
    
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

	public String getLogin() {
		return login;
	}

	public void setLogin(String login) {
		this.login = login;
	}

	public java.util.Date getFrom() {
		return from;
	}

	public void setFrom(java.util.Date from) {
		this.from = from;
	}

	public java.util.Date getTo() {
		return to;
	}

	public void setTo(java.util.Date to) {
		this.to = to;
	}

	public String getSortColumn() {
		return sortColumn;
	}

	public int getRowCount() {
		return rowCount;
	}

	public void setSortColumn(String sortColumn) {
		this.sortColumn = sortColumn;
	}

	public void setRowCount(int rowCount) {
		this.rowCount = rowCount;
	}

	@Override
    public String toString() {
        return "UpdateSNMPConfiguration{" +
            "instance=" + instance +
            ", login=" + login +
            ", from=" + from +
            ", to=" + to +
            '}';
    }
}
