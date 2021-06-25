package com.idera.sqlcm.entities;
import com.fasterxml.jackson.annotation.JsonProperty;

public class CMAlertRulesCondition {
    @JsonProperty("conditionId")
    private long conditionId;

    @JsonProperty("fieldId")
    private long fieldId;

    @JsonProperty("matchString")
    private String matchString;
    
	public long getConditionId() {
		return conditionId;
	}

	public void setConditionId(long conditionId) {
		this.conditionId = conditionId;
	}

	public long getFieldId() {
		return fieldId;
	}

	public void setFieldId(long fieldId) {
		this.fieldId = fieldId;
	}

	public String getMatchString() {
		return matchString;
	}

	public void setMatchString(String matchString) {
		this.matchString = matchString;
	}

	public CMAlertRulesCondition() {
    }

   
    @Override
    public String toString() {
        return "CMAlertRulesCondition{" +
            "conditionId=" + conditionId +
            ", fieldId=" + fieldId +
            ", matchString='" + matchString + '\'' +
            '}';
    }
}
