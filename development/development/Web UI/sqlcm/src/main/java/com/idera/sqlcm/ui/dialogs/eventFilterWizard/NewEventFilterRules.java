package com.idera.sqlcm.ui.dialogs.eventFilterWizard;
import com.fasterxml.jackson.annotation.JsonProperty;

public class NewEventFilterRules {
	 @JsonProperty("name")
     private String name;
     
     @JsonProperty("eventFilterLevel")
     private int eventFilterLevel;
     
     @JsonProperty("description")
     private String description;
     
     @JsonProperty("eventFilter")
     private String eventFilter;
          
     @JsonProperty("eventFilterCategoryFiledId")
     private int eventFilterCategoryFiledId;
     
     @JsonProperty("eventFilterCategorMatchString")
     private String eventFilterCategorMatchString;
     
     @JsonProperty("eventFilterEventType")
     private int eventFilterEventType;
     
     @JsonProperty("eventFilterType")
     private int eventFilterType;
          
     public String getName() {
         return name;
     }

     public void setName(String name) {
         this.name = name;
     }
     
     public String getEventFilterCategorMatchString() {
 		return eventFilterCategorMatchString;
 	}

 	public void setEventFilterCategorMatchString(String eventFilterCategorMatchString) {
 		this.eventFilterCategorMatchString = eventFilterCategorMatchString;
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
     
     public int getEventFilterCategoryFiledId() {
         return eventFilterCategoryFiledId;
     }

     public void setEventFilterCategoryFiledId(int eventFilterCategoryFiledId) {
         this.eventFilterCategoryFiledId = eventFilterCategoryFiledId;
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
         this.eventFilterType = eventFilterType;
     }
}
