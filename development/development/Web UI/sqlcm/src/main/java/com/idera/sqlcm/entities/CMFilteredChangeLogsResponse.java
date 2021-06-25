package com.idera.sqlcm.entities;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMFilteredChangeLogsResponse {
    @JsonProperty("Alerts")
    List<CMChangeLogs> alerts;

    @JsonProperty("TotalHighAlerts")
    int totalHighAlerts;

    @JsonProperty("TotalLowAlerts")
    int totalLowAlerts;

    @JsonProperty("TotalMediumAlerts")
    int totalMediumAlerts;

    @JsonProperty("TotalSevereAlerts")
    int totalSevereAlerts;

    @JsonProperty("EventType")
    private List<CMEventFilterListData> eventType;
    public List<CMEventFilterListData> getEventType() {
		return eventType;
	}
	public void setEventType(List<CMEventFilterListData> eventType) {
		this.eventType = eventType;
	}
    public CMFilteredChangeLogsResponse() {
    }

    public List<CMChangeLogs> getAlerts() {
        return alerts;
    }

    public void setAlerts(List<CMChangeLogs> alerts) {
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

    @Override
    public String toString() {
        return "CMFilteredActivityLogsResponce{" +
            "alerts=" + alerts +
            ", totalHighAlerts=" + totalHighAlerts +
            ", totalLowAlerts=" + totalLowAlerts +
            ", totalMediumAlerts=" + totalMediumAlerts +
            ", totalSevereAlerts=" + totalSevereAlerts +
            '}';
    }
}
