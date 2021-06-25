package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMExportEvent {

    @JsonProperty("filterId")
    private long filterId;

    @JsonProperty("name")
    private String name;

    @JsonProperty("description")
    private String description;

    @JsonProperty("eventType")
    private int eventType;

    @JsonProperty("targetInstance")
    private String targetInstance;
    
	@JsonProperty("enabled")
    private boolean enabled;

    public long getFilterId() {
		return filterId;
	}

	public void setFilterId(long filterId) {
		this.filterId = filterId;
	}

	public String getName() {
		return name;
	}
	
	public void setName(String name) {
		this.name = name;
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}

	public int getEventType() {
		return eventType;
	}

	public void setEventType(int eventType) {
		this.eventType = eventType;
	}

	public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
	}
	
	public String getTargetInstance() {
		return targetInstance;
	}

	public void setTargetInstance(String targetInstance) {
		this.targetInstance = targetInstance;
	}

	public CMExportEvent() {
    }

    public int getHashCode(){
        return hashCode();
    }
    @Override
    public String toString() {
        return "CMExportEvent{" +
            "filterId=" + filterId +
            ", name='" + name + '\'' +
            ", description='" + description + '\'' +
            ", eventType=" + eventType +
            ", targetInstance='" + targetInstance + '\'' +
            ", enabled=" + enabled + 
            '}';
    }
}
