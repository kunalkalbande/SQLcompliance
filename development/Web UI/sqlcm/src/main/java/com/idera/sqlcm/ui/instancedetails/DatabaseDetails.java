//Start SQLCm-5.4 
//Requirement - 4.1.3.1. 

package com.idera.sqlcm.ui.instancedetails;

import org.zkoss.zul.ListModelList;

import com.fasterxml.jackson.annotation.JsonProperty;

public class DatabaseDetails {

	@JsonProperty("dbId")
    private int dbId;

	@JsonProperty("name")
    private String name;
	
	@JsonProperty("srvId")
    private long srvId;
	
	@JsonProperty("selected")
	private boolean selected;
	
	ListModelList<TableDetails> tableDetails = new ListModelList<TableDetails>();
	
	boolean expand = false;
		
	public boolean isExpand() {
		return expand;
	}

	public void setExpand(boolean expand) {
		this.expand = expand;
	}

	public ListModelList<TableDetails> getTableDetails() {
		return tableDetails;
	}

	public void setTableDetails(ListModelList<TableDetails> tableDetails) {
		this.tableDetails = tableDetails;
	}

	public boolean isSelected() {
		return selected;
	}

	public void setSelected(boolean selected) {
		this.selected = selected;
	}

	public long getSrvId() {
		return srvId;
	}

	public void setSrvId(long srvId) {
		this.srvId = srvId;
	}

	public int getDbId() {
	return dbId;
    }

    public void setDbId(int dbId) {
	this.dbId = dbId;
    }

    public String getName() {
	return name;
    }

    public void setName(String name) {
	this.name = name;
    }
}

//End SQLCm-5.4