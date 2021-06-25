package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMExportEventConditionData {

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

	public CMExportEventConditionData() {
    }

   
    @Override
    public String toString() {
        return "CMExportEventConditionData{" +
            "conditionId=" + conditionId +
            ", fieldId=" + fieldId +
            ", matchString='" + matchString + '\'' +
            '}';
    }
}
