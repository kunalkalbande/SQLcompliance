package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMExportEventType {

	@JsonProperty
	private String name;
	
	@JsonProperty
	private String category;

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getCategory() {
		return category;
	}

	public void setCategory(String category) {
		this.category = category;
	}
	
	public CMExportEventType(){		
	}
	
	 @Override
	    public String toString() {
	        return "CMExportEventType{" +
	            "name=" + name +
	            ", category=" + category +'\'' +
	            '}';
	    }
	
}
