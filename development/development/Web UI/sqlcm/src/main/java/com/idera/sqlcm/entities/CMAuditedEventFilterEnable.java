package com.idera.sqlcm.entities;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditedEventFilterEnable {

	@JsonProperty("isEnable")
	boolean isEnable;

	@JsonProperty("eventId")
	long eventId;

	public long getEventId() {
		return eventId;
	}

	public void setEventId(long eventId) {
		this.eventId = eventId;
	}

	public boolean isEnable() {
		return isEnable;
	}

	public void setEnable(boolean isEnable) {
		this.isEnable = isEnable;
	}

	@Override
	public String toString() {
		return "CMAuditEventFilterEnableRequest{" +
				"eventId=" + eventId +
				", isEnable=" + isEnable +
				'}';
   }
}
