package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMFilteredAlertsResponse {

    @JsonProperty("Alerts")
    List<CMAlert> alerts;

    @JsonProperty("TotalHighAlerts")
    int totalHighAlerts;

    @JsonProperty("TotalLowAlerts")
    int totalLowAlerts;

    @JsonProperty("TotalMediumAlerts")
    int totalMediumAlerts;

    @JsonProperty("TotalSevereAlerts")
    int totalSevereAlerts;

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
    public CMFilteredAlertsResponse() {
    }

    public List<CMAlert> getAlerts() {
        return alerts;
    }

    public void setAlerts(List<CMAlert> alerts) {
        this.alerts = alerts;
    }

    public int getTotalHighAlerts() {
        return totalHighAlerts;
    }

    public void setTotalHighAlerts(int totalHighAlerts) {
        this.totalHighAlerts = totalHighAlerts;
    }

    public int getTotalLowAlerts() {
        return totalLowAlerts;
    }

    public void setTotalLowAlerts(int totalLowAlerts) {
        this.totalLowAlerts = totalLowAlerts;
    }

    public int getTotalMediumAlerts() {
        return totalMediumAlerts;
    }

    public void setTotalMediumAlerts(int totalMediumAlerts) {
        this.totalMediumAlerts = totalMediumAlerts;
    }

    public int getTotalSevereAlerts() {
        return totalSevereAlerts;
    }

    public void setTotalSevereAlerts(int totalSevereAlerts) {
        this.totalSevereAlerts = totalSevereAlerts;
    }

    public int getRecordCount() {
        return recordCount;
    }

    public void setRecordCount(int recordCount) {
        this.recordCount = recordCount;
    }

    @Override
    public String toString() {
        return "CMFilteredAlertsResponce{" +
            "alerts=" + alerts +
            ", totalHighAlerts=" + totalHighAlerts +
            ", totalLowAlerts=" + totalLowAlerts +
            ", totalMediumAlerts=" + totalMediumAlerts +
            ", totalSevereAlerts=" + totalSevereAlerts +
            ", eventType=" + eventType +
            '}';
    }
}
