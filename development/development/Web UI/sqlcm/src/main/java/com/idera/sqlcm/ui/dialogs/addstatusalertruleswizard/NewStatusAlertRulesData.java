package com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard;
import com.fasterxml.jackson.annotation.JsonProperty;

public class NewStatusAlertRulesData {
        
	    @JsonProperty("name")
	    private String name;
	    
	    @JsonProperty("alertLevel")
	    private int alertLevel;
	    
	    @JsonProperty("description")
	    private String description;
	    
	    @JsonProperty("fieldId")
	    private int fieldId;
	    
	    @JsonProperty("matchString")
	    private String matchString;
	    
	    @JsonProperty("alertType")
	    private int alertType;
	    
	    @JsonProperty("targetInstances")
	    private String targetInstances;

		
	    public int getAlertType() {
			return alertType;
		}

		public void setAlertType(int alertType) {
			this.alertType = alertType;
		}

		public String getTargetInstances() {
			return targetInstances;
		}

		public void setTargetInstances(String targetInstances) {
			this.targetInstances = targetInstances;
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

		public int getFieldId() {
			return fieldId;
		}

		public void setFieldId(int fieldId) {
			this.fieldId = fieldId;
		}

		public String getMatchString() {
			return matchString;
		}

		public void setMatchString(String matchString) {
			this.matchString = matchString;
		}    
}    