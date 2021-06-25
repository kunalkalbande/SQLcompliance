package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMEventFilters extends CMEntity{

    @JsonProperty("filterid")
    private long filterid;

    @JsonProperty("names")
    private String names;
    
    @JsonProperty("description")
    private String description;

    @JsonProperty("eventType")
    private long eventType;

    @JsonProperty("targetInstances")
    private String targetInstances;

    @JsonProperty("enabled")
    private Boolean enabled;
    
    @JsonProperty("instanceId")
    private long instanceId;
    
    @JsonProperty("validFilter")
    private int validFilter;
    

	public int getValidFilter() {
		return validFilter;
	}

	public void setValidFilter(int validFilter) {
		this.validFilter = validFilter;
	}

	public long getFilterid() {
		return filterid;
	}

	public void setFilterid(long filterid) {
		this.filterid = filterid;
	}

	public String getNames() {
		return names;
	}

	public void setNames(String names) {
		this.names = names;
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}

	public long getEventType() {
		return eventType;
	}

	public void setEventType(long eventType) {
		this.eventType = eventType;
	}

	public String getTargetInstances() {
		return targetInstances;
	}
	
	public void setTargetInstances(String targetInstances) {
		this.targetInstances = targetInstances;
	}

	public Boolean getEnabled() {
		return enabled;
	}

	public void setEnabled(Boolean enabled) {
		this.enabled = enabled;
	}
	

	public long getInstanceId() {
		return instanceId;
	}

	public void setInstanceId(long instanceId) {
		this.instanceId = instanceId;
	}



	public CMEventFilters() {
    }    

    @Override
    public String toString() {
        return "CMEntityEventFilters{" +
            "filterid=" + filterid +
            ", name='" + name + '\'' +
            ", description='" + description + '\'' +
            ", eventType=" + eventType +
            ", targetInstances='" + targetInstances + '\'' +
            ", enabled='" + enabled + '\'' +
            '}';
    }
}
