package com.idera.sqlcm.entities;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMAlertRulesExportResponse {

	    @JsonProperty("AlertRules")
	    private List<CMAlertRules> alertRules;

	    @JsonProperty("AlertRulesCondition")
	    private List<CMAlertRulesCondition> alertRulesCondition;
	    
	    @JsonProperty("AlertRulesCategory")
	    private List<CMAlertRulesCategory> alertRulesCategory;
	    
	    public List<CMAlertRules> getAlertRules() {
			return alertRules;
		}

	    public void setAlertRules(List<CMAlertRules> alertRules) {
			this.alertRules = alertRules;
		}

		public List<CMAlertRulesCondition> getAlertRulesCondition() {
			return alertRulesCondition;
		}
		
		public void setAlertRulesCondition(
				List<CMAlertRulesCondition> alertRulesCondition) {
			this.alertRulesCondition = alertRulesCondition;
		}
		
		public List<CMAlertRulesCategory> getAlertRulesCategory() {
			return alertRulesCategory;
		}

		public void setAlertRulesCategory(List<CMAlertRulesCategory> alertRulesCategory) {
			this.alertRulesCategory = alertRulesCategory;
		}

		public CMAlertRulesExportResponse()
	    {}

		
	    @Override
	    public String toString() {
	        return "CMAlertRulesExportResponce{" +
	            "alertRules=" + alertRules +
	            ", alertRulesCondition=" + alertRulesCondition +	            
	            ", alertRulesCategory=" + alertRulesCategory +
	            '}';
	    }
}
