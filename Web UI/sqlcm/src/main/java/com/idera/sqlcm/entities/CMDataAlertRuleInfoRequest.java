package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMDataAlertRuleInfoRequest {
	@JsonProperty("srvId")
    int srvId;
	
	@JsonProperty("conditionId")
    int conditionId;

	public int getConditionId() {
		return conditionId;
	}

	public void setConditionId(int conditionId) {
		this.conditionId = conditionId;
	}

	public int getSrvId() {
		return srvId;
	}

	public void setSrvId(int srvId) {
		this.srvId = srvId;
	}
	
	@Override
    public String toString() {
        return "CMDataAlertRuleInfoRequest{" +
            "srvId=" + srvId +
            "conditionId="+conditionId+
            '}';
	}
}
