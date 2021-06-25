package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class NewDataAlertRules {

	    @JsonProperty("name")
	    private String name;
	    
	    @JsonProperty("alertLevel")
	    private int alertLevel;
	    
	    @JsonProperty("databaseName")
	    private boolean databaseName;
	    
	    @JsonProperty("description")
	    private String description;
	    
	    @JsonProperty("eventFilter")
	    private String eventFilter;
	    
	    @JsonProperty("matchString")
	    private String matchString;
	    
	    @JsonProperty("alertType")
	    private int alertType;
	    
	    @JsonProperty("fieldId")
	    private int fieldId;
	    
	    public String getMatchString() {
			return matchString;
		}

		public void setMatchString(String matchString) {
			this.matchString = matchString;
		}

		public int getFieldId() {
			return fieldId;
		}

		public void setFieldId(int fieldId) {
			this.fieldId = fieldId;
		}

		public int getAlertType() {
			return alertType;
		}

		public void setAlertType(int alertType) {
			this.alertType = alertType;
		}

		public String getName() {
	        return name;
	    }

	    public void setName(String name) {
	        this.name = name;
	    }
	    
	    public int getAlertLevel() {
	        return alertLevel;
	    }

	    public void setAlertLevel(int alertLevel) {
	        this.alertLevel = alertLevel;
	    }
	    
	    public String getDescription() {
	        return description;
	    }

	    public void setDescription(String description) {
	        this.description = description;
	    }
	    
	    public String getEventFilter() {
	        return eventFilter;
	    }

	    public void setEventFilter(String eventFilter) {
	        this.eventFilter = eventFilter;
	    }
}
