package com.idera.sqlcm.ui.dialogs.eventFilterWizard;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMDmlSelectFilters;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.wizard.IWizardEntity;

import java.util.List;

public class EventFilterSaveEntity extends CMEntity implements
		IWizardEntity {
	
	@JsonProperty("name")
    private String name;
    
    @JsonProperty("eventFilterLevel")
    private int eventFilterLevel;
    
    @JsonProperty("description")
    private String description;
    
    @JsonProperty("eventFilter")
    private String eventFilter;
    
    @JsonProperty("eventFilterCategory")
    private int eventFilterCategory;
    
    @JsonProperty("eventFilterEventType")
    private int eventFilterEventType;
    
    @JsonProperty("eventFilterType")
    private int eventFilterType;
    

	@JsonProperty("newEventAlertRules")	
	private NewEventFilterRules newEventAlertRules;
	
	//-----------------------------------------------------------------------------------------------------------
	@JsonProperty("targetInstances")
    private String targetInstances;
	
	@JsonProperty("regulationSettings")
	private RegulationSettings regulationSettings;
    //-----------------------------------------------------------------------------------------------------------
    public String getTargetInstances() {
        return targetInstances;
    }

    public void setTargetInstances(String targetInstances) {
        this.targetInstances = targetInstances;
    }
    
    @JsonProperty("databaseList")
	private List<CMDatabase> databaseList;	

    //-----------------------------------------------------------------------------------------------------------
	public NewEventFilterRules getNewEventAlertRules() {
		return newEventAlertRules;
	}

	public void setNewEventAlertRules(NewEventFilterRules newEventAlertRules) {
		this.newEventAlertRules = newEventAlertRules;
	}
	
    public int getEventFilterLevel() {
        return eventFilterLevel;
    }

    public void setEventFilterLevel(int eventFilterLevel) {
        this.eventFilterLevel = eventFilterLevel;
    }
    
    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }
    
    public String getEventFilter() {
        return eventFilter;
    }

    public void setEventFilter(String eventFilter) {
        this.eventFilter = eventFilter;
    }
    
    public int getEventFilterCategory() {
        return eventFilterCategory;
    }

    public void setEventFilterCategory(int eventFilterCategory) {
        this.eventFilterCategory = eventFilterCategory;
    }
    
    public int getEventFilterEventType() {
        return eventFilterEventType;
    }

    public void setEventFilterEventType(int eventFilterEventType) {
        this.eventFilterEventType = eventFilterEventType;
    }
    
    public int getEventFilterType() {
        return eventFilterType;
    }

    public void setEventFilterType(int eventFilterType) {
        this.eventFilterType = eventFilterEventType;
    }
    
    
    
    public RegulationSettings getRegulationSettings() {
		return regulationSettings;
	}

	public void setRegulationSettings(RegulationSettings regulationSettings) {
		this.regulationSettings = regulationSettings;
	}

    public List<CMDatabase> getDatabaseList() {
		return databaseList;
	}
    
    public void setDatabaseList(List<CMDatabase> databaseList) {
		this.databaseList = databaseList;
	}
}
