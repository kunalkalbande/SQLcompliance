package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMEventFilterListData {
	
	@JsonProperty("eventType")
    private String eventType;
   
	 public String getEventType() {
		return eventType;
	}

	public void setEventType(String eventType) {
		this.eventType = eventType;
	}
	
	@Override
	    public String toString() {
	        return "CMEventFilterListData{" +
	        		"eventType=" + eventType +
	            '}';
	    }
}
