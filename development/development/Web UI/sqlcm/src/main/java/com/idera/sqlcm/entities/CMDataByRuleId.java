package com.idera.sqlcm.entities;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMDataByRuleId {

	@JsonProperty("ruleId")
	long ruleId;
	
	public long getFilterId() {
		return ruleId;
	}

	public void setFilterId(long ruleId) {
		this.ruleId = ruleId;
	}
	
	@Override
	public String toString() {
		return "CMDataByRuleId{" +
				"ruleId=" + ruleId +
				'}';
   }
}
