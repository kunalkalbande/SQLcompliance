package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMDatabaseAuditedActivity extends CMEntity{

    @JsonProperty("instance")
    private String instance;

    @JsonProperty("isEnabled")
    private boolean isEnabled;

    @JsonProperty("regulationGuidelinesString")
    private String regulationGuidelinesString;

    @JsonProperty("databaseAuditedActivitiesString")
    private String databaseAuditedActivitiesString;

    @JsonProperty("beforeAfterTables")
    private String beforeAfterTables;

    @JsonProperty("sensitiveColumnsTablesString")
    private String sensitiveColumnsTablesString;

    @JsonProperty("trustedUsersString")
    private String trustedUsersString;

    @JsonProperty("eventFiltersString")
    private String eventFiltersString;

    public CMDatabaseAuditedActivity() {
    }

    public String getInstance() {
        return instance;
    }

    public void setInstance(String instance) {
        this.instance = instance;
    }

    public boolean isEnabled() {
        return isEnabled;
    }

    public void setIsEnabled(boolean isEnabled) {
        this.isEnabled = isEnabled;
    }

    public String getRegulationGuidelinesString() {
        return regulationGuidelinesString;
    }

    public void setRegulationGuidelinesString(String regulationGuidelinesString) {
        this.regulationGuidelinesString = regulationGuidelinesString;
    }

    public String getDatabaseAuditedActivitiesString() {
        return databaseAuditedActivitiesString;
    }

    public void setDatabaseAuditedActivitiesString(String databaseAuditedActivitiesString) {
        this.databaseAuditedActivitiesString = databaseAuditedActivitiesString;
    }

    public String getBeforeAfterTables() {
        return beforeAfterTables;
    }

    public void setBeforeAfterTables(String beforeAfterTables) {
        this.beforeAfterTables = beforeAfterTables;
    }

    public String getSensitiveColumnsTablesString() {
        return sensitiveColumnsTablesString;
    }

    public void setSensitiveColumnsTablesString(String sensitiveColumnsTablesString) {
        this.sensitiveColumnsTablesString = sensitiveColumnsTablesString;
    }

    public String getTrustedUsersString() {
        return trustedUsersString;
    }

    public void setTrustedUsersString(String trustedUsersString) {
        this.trustedUsersString = trustedUsersString;
    }

    public String getEventFiltersString() {
        return eventFiltersString;
    }

    public void setEventFiltersString(String eventFiltersString) {
        this.eventFiltersString = eventFiltersString;
    }
}
