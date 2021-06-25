package com.idera.sqlcm.entities;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMFilterEventFiltersResponse {
    @JsonProperty("Events")
    List<CMEventFilters> events;

    public CMFilterEventFiltersResponse() {
    }

    public List<CMEventFilters> getEvents() {
        return events;
    }

    public void setEvents(List<CMEventFilters> events) {
        this.events = events;
    }

    @Override
    public String toString() {
        return "CMFilteredEventFiltersResponse{" +
            "events=" + events +
            '}';
    }
}
