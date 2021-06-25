//Start SQLCm-5.4 
//Requirement - 4.1.3.1. 

package com.idera.sqlcm.ui.instancedetails;

import org.zkoss.zul.ListModelList;

import com.fasterxml.jackson.annotation.JsonProperty;

public class TableDetails {

	@JsonProperty("dbId")
    private int dbId;

    @JsonProperty("tblId")
    private int tblId;

    @JsonProperty("name")
    private String name;
    
    @JsonProperty("selected")
	private boolean selected;

    ListModelList<ColumnDetails> columList = new ListModelList<ColumnDetails>();
    
    boolean expand = false;
	
	public boolean isExpand() {
		return expand;
	}

	public void setExpand(boolean expand) {
		this.expand = expand;
	}

	public ListModelList<ColumnDetails> getColumList() {
		return columList;
	}

	public void setColumList(ListModelList<ColumnDetails> columList) {
		this.columList = columList;
	}

	public boolean isSelected() {
		return selected;
	}

	public void setSelected(boolean selected) {
		this.selected = selected;
	}

	public int getDbId() {
		return dbId;
	}

	public void setDbId(int dbId) {
		this.dbId = dbId;
	}

	public int getTblId() {
		return tblId;
	}

	public void setTblId(int objectId) {
		this.tblId = objectId;
	}
	
	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}
}

//End SQLCm-5.4
