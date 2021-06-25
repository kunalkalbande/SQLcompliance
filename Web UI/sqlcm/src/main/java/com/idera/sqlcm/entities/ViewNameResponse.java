package com.idera.sqlcm.entities;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;

public class ViewNameResponse {
	@JsonProperty("viewNameTable")
    private List<ViewNameData> viewNameTable;

	public List<ViewNameData> getViewNameTable() {
		return viewNameTable;
	}

	public void setViewNameTable(List<ViewNameData> viewNameTable) {
		this.viewNameTable = viewNameTable;
	}
	
	public ViewNameResponse()
    {}

	 @Override
	    public String toString() {
	        return "ViewNameResponse{" +
	            "viewNameTable=" + viewNameTable +
	            '}';
	  }
}
