package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMFilteredAlertRulesResponse {

    @JsonProperty("AlertRules")
    List<CMAlertRules> alertRules;

    @JsonProperty("TotalEventAlertRules")
    int totalEventAlertRules;

    @JsonProperty("TotalDataAlertRules")
    int totalDataAlertRules;

    @JsonProperty("TotalStatusAlertRules")
    int totalStatusAlertRules;

    @JsonProperty("recordCount")
    int recordCount;
    
    @JsonProperty("EventType")
    private List<CMEventFilterListData> eventType;
    public List<CMEventFilterListData> getEventType() {
		return eventType;
	}
	public void setEventType(List<CMEventFilterListData> eventType) {
		this.eventType = eventType;
	}
    
    public int getTotalEventAlertRules() {
		return totalEventAlertRules;
	}

	public void setTotalEventAlertRules(int totalEventAlertRules) {
		this.totalEventAlertRules = totalEventAlertRules;
	}

	public int getTotalDataAlertRules() {
		return totalDataAlertRules;
	}

	public void setTotalDataAlertRules(int totalDataAlertRules) {
		this.totalDataAlertRules = totalDataAlertRules;
	}

	public int getTotalStatusAlertRules() {
		return totalStatusAlertRules;
	}

	public void setTotalStatusAlertRules(int totalStatusAlertRules) {
		this.totalStatusAlertRules = totalStatusAlertRules;
	}

	public int getRecordCount() {
		return recordCount;
	}

	public void setRecordCount(int recordCount) {
		this.recordCount = recordCount;
	}

    public CMFilteredAlertRulesResponse() {
    }

    public List<CMAlertRules> getAlertRules() {
        return alertRules;
    }

    public void setAlertRules(List<CMAlertRules> alertRules) {
        this.alertRules = alertRules;
    }

    @Override
    public String toString() {
        return "CMFilteredAlertsResponce{" +
            "alertRules=" + alertRules +
            ", totalEventAlertRules=" + totalEventAlertRules +
            ", totalDataAlertRules=" + totalDataAlertRules +
            ", totalStatusAlertRules=" + totalStatusAlertRules +            
	        ", eventType=" + eventType +
            '}';
    }
}
