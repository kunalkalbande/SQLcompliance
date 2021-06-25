package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

public class CMBeforeAfterDataEventsResponse {

    @JsonProperty("Events")
    private List<CMEventDetails> events;

    @JsonProperty("Columns")
    private List<CMBeforeAfterDataEntity> columns;

    @JsonProperty("Tables")
    private List<CMBeforeAfterDataEntity> tables;

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

    public List<?> getEvents() {
        return events;
    }

    public void setEvents(List<CMEventDetails> events) {
        this.events = events;
    }

    public List<CMBeforeAfterDataEntity> getColumns() {
        return columns;
    }

    public void setColumns(List<CMBeforeAfterDataEntity> columns) {
        this.columns = columns;
    }

    public List<CMBeforeAfterDataEntity> getTables() {
        return tables;
    }

    public void setTables(List<CMBeforeAfterDataEntity> tables) {
        this.tables = tables;
    }

    public int getRecordCount() {
        return recordCount;
    }

    public void setRecordCount(int recordCount) {
        this.recordCount = recordCount;
    }

    @Override
    public String toString() {
        return "CMBeforeAfterDataEventsResponse{" +
            "events=" + events +
            ", columns=" + columns +
            ", tables=" + tables +
            ", eventType=" + eventType +
            '}';
    }
}
