package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

public class CMAuditEventFilterExportResponse {

    @JsonProperty("Events")
    private List<CMExportEvent> events;

    @JsonProperty("ConditionEvents")
    private List<CMExportEventConditionData> conditionEvents;
    
    @JsonProperty("EventType")
    private List<CMExportEventType> eventType;
    
    public List<CMExportEventType> getEventType() {
		return eventType;
	}

	public void setEventType(List<CMExportEventType> eventType) {
		this.eventType = eventType;
	}

	public CMAuditEventFilterExportResponse()
    {}

	public List<CMExportEvent> getEvents() {
		return events;
	}

	public void setEvents(List<CMExportEvent> events) {
		this.events = events;
	}

	public List<CMExportEventConditionData> getConditionEvents() {
		return conditionEvents;
	}

	public void setConditionEvents(List<CMExportEventConditionData> conditionEvents) {
		this.conditionEvents = conditionEvents;
	}

	
    @Override
    public String toString() {
        return "CMAuditEventFilterExportResponse{" +
            "events=" + events +
            ", conditionEvents=" + conditionEvents +
            ", eventType=" + eventType +
            '}';
    }



	
}
