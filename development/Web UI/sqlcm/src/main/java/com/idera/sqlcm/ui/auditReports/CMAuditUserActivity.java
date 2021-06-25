package com.idera.sqlcm.ui.auditReports;

import java.sql.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;


@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditUserActivity {

    @JsonProperty("instance")
    private String instance;

    @JsonProperty("database")
    private String database;

    @JsonProperty("loginName")
    private String loginName;

	@JsonProperty("category")
	private String category;

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

    @JsonProperty("user")
    private int user;
    
    @JsonProperty("sortColumn")
    private String sortColumn;
    
    @JsonProperty("rowCount")
    private int rowCount;    

	@Override
    public String toString() {
        return "UpdateSNMPConfiguration{" +
            "instance=" + instance +
            ",loginName=" + loginName+
            ", database=" + database +
            ", objectName=" + loginName +
            ", category=" + category +
            ", to=" + to +
            ", from=" + from +
            ", sql=" + sql +
            ", user=" + user +
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

	public String getLoginName() {
		return loginName;
	}

	public void setLoginName(String loginName) {
		this.loginName = loginName;
	}

	public String getCategory() {
		return category;
	}

	public void setCategory(String category) {
		
		
		this.category = category;
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

	public int getUser() {
		return user;
	}

	public void setUser(int user) {
		this.user = user;
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
	
public void setCategoryNames(String categoryValue)
{
	try{
if (categoryValue!=null)
{
	


	switch(categoryValue)
	{

		
		case "ALL":
			category="-1";	
			
		break;
		case "Admin":
			category="6";
			
	    case "Broker":
	    	category="8";
	    	
			break;
		case "DDL":
			category="2";
				
				break;
		case "DML":
			category="4";
				
				break;
		case "Integrity Check":
			category="0";
								
				break;
		case "Login":
			category="1";
					break;
		case "Security":
			category="3";	
			
					break;
		case "Select":
			category="5";		
			
					break;	
		case "Server":
			category="10";
						
						break;
		case "User Defined":
			category="9";

	}
}	
	
}catch(NullPointerException  e)
{
	
}
	}
}


