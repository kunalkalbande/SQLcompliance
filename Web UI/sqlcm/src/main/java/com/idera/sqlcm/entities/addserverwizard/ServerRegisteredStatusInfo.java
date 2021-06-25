package com.idera.sqlcm.entities.addserverwizard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class ServerRegisteredStatusInfo {

    @JsonProperty("registeredStatus")
    private long registeredStatus;

    @JsonProperty("eventDatabaseName")
    private String eventDatabaseName;

    public long getRegisteredStatus() {
        return registeredStatus;
    }

    public void setRegisteredStatus(long registeredStatus) {
        this.registeredStatus = registeredStatus;
    }

    public String getEventDatabaseName() {
        return eventDatabaseName;
    }

    public void setEventDatabaseName(String eventDatabaseName) {
        this.eventDatabaseName = eventDatabaseName;
    }
}
