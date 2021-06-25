package com.idera.sqlcm.entities;
import com.fasterxml.jackson.annotation.JsonProperty;

public class ViewNameData {
	@JsonProperty("viewName")
    private String viewName;

	public String getViewName() {
		return viewName;
	}
	
	public void setViewName(String viewName) {
		this.viewName = viewName;
	}


	@Override
    public String toString() {
        return "CategoryData{" +
            "viewName=" + viewName +
            '}';
    }
}
