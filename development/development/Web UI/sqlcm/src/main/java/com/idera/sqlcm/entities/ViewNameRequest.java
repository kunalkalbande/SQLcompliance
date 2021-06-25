package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class ViewNameRequest {

	@JsonProperty("viewName")
    String viewName;

	public String getViewName() {
		return viewName;
	}

	public void setViewName(String viewName) {
		this.viewName = viewName;
	}
	
	@Override
    public String toString() {
        return "ViewNameRequest{" +
            "viewName=" + viewName +
            '}';
    }
}
