//Start SQLCm-5.4 
//Requirement - 4.1.3.1. 

package com.idera.sqlcm.ui.instancedetails;

import com.fasterxml.jackson.annotation.JsonProperty;

public class ColumnDetails {

	@JsonProperty("dbId")
    private int dbId;
	
	@JsonProperty("tblId")
    private int tblId;

    @JsonProperty("colId")
    private int colId;

    @JsonProperty("name")
    private String name;
    
    @JsonProperty("selected")
	private boolean selected;

	public boolean isSelected() {
		return selected;
	}

	public void setSelected(boolean selected) {
		this.selected = selected;
	}

	public int getTblId() {
		return tblId;
	}

	public void setTblId(int tblId) {
		this.tblId = tblId;
	}

	public int getColId() {
		return colId;
	}

	public void setColId(int colId) {
		this.colId = colId;
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
