package com.idera.sqlcm.entities;

public class CMAuditedActivity{

    private String regulationGuidelines;

    private String enhancedDatabase;

    private String beforeAfter = "";

    private String sensitiveColumns = "";

    private String trustedUsers;

    private String eventFilters;

    public String getRegulationGuidelines() {
        return regulationGuidelines;
    }

    public void setRegulationGuidelines(String regulationGuidelines) {
        this.regulationGuidelines = regulationGuidelines;
    }

    public String getEnhancedDatabase() {
        return enhancedDatabase;
    }

    public void setEnhancedDatabase(String enhancedDatabase) {
        this.enhancedDatabase = enhancedDatabase;
    }

    public String getBeforeAfter() {
        return beforeAfter;
    }

    public void setBeforeAfter(String beforeAfter) {
        this.beforeAfter = beforeAfter;
    }

    public String getSensitiveColumns() {
        return sensitiveColumns;
    }

    public void setSensitiveColumns(String sensitiveColumns) {
        this.sensitiveColumns = sensitiveColumns;
    }

    public String getTrustedUsers() {
        return trustedUsers;
    }

    public void setTrustedUsers(String trustedUsers) {
        this.trustedUsers = trustedUsers;
    }

    public String getEventFilters() {
        return eventFilters;
    }

    public void setEventFilters(String eventFilters) {
        this.eventFilters = eventFilters;
    }

    @Override
    public String toString() {
        return "CMAuditedActivity{" +
            "regulationGuidelines='" + regulationGuidelines + '\'' +
            ", enhancedDatabase='" + enhancedDatabase + '\'' +
            ", beforeAfter='" + beforeAfter + '\'' +
            ", sensitiveColumns='" + sensitiveColumns + '\'' +
            ", trustedUsers='" + trustedUsers + '\'' +
            ", eventFilters='" + eventFilters + '\'' +
            '}';
    }
}
