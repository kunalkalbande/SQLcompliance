package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAlertRulesEnableRequest {

    @JsonProperty("isEnable")
    boolean isEnable;
    
    @JsonProperty("ruleId")
    long ruleId;

	public boolean isEnable() {
		return isEnable;
	}

	public void setEnable(boolean isEnable) {
		this.isEnable = isEnable;
	}

	public long getRuleId() {
		return ruleId;
	}

	public void setRuleId(long ruleId) {
		this.ruleId = ruleId;
	}

    @Override
    public String toString() {
        return "CMAlertRulesEnableRequest{" +
            "ruleId=" + ruleId +
            ", isEnable=" + isEnable +
            '}';
    }
}
