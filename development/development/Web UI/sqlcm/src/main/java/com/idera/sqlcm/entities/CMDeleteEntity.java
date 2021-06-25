package com.idera.sqlcm.entities;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMDeleteEntity {

	@JsonProperty("eventId")
	long eventId;

	public long getEventId() {
		return eventId;
	}

	public void setEventId(long eventId) {
		this.eventId = eventId;
	}

	

	@Override
	public String toString() {
		return "CMDeleteEntityRequest{" +
				"eventId=" + eventId +
				'}';
   }
}
