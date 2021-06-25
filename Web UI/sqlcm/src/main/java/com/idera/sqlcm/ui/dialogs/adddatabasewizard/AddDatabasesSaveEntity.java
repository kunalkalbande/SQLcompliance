package com.idera.sqlcm.ui.dialogs.adddatabasewizard;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMDmlSelectFilters;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.database.properties.CMSensitiveColumnTableData;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.wizard.IWizardEntity;

import java.util.List;

public class AddDatabasesSaveEntity extends CMEntity implements IWizardEntity {

    @JsonProperty("databaseList")
    private List<CMDatabase> databaseList;

    @JsonProperty("collectionLevel")
    private int collectionLevel;

    @JsonProperty("trustedRolesAndUsers")
    private Object trustedRolesAndUsers;

    @JsonProperty("privilegedRolesAndUsers")
    private Object privilegedRolesAndUsers;

    @JsonProperty("regulationSettings")
    private RegulationSettings regulationSettings;

    @JsonProperty("availabilityGroupList")
    private Object availabilityGroupList;

    @JsonProperty("auditedActivities")
    private CMAuditedActivities auditedActivities;

    @JsonProperty("userAuditedActivities")
    private CMAuditedActivities userAuditedActivities;

    @JsonProperty("dmlSelectFilters")
    private CMDmlSelectFilters dmlSelectFilters;

    @JsonProperty("auditExceptions")
    private Boolean auditExceptions = false;

    @JsonProperty("sensitiveColumnTableDictionary")
    private CMSensitiveColumnTableData sensitiveColumnTableDictionary;

   

    public List<CMDatabase> getDatabaseList() {
        return databaseList;
    }

    public void setDatabaseList(List<CMDatabase> databaseList) {
        this.databaseList = databaseList;
    }

    public int getCollectionLevel() {
        return collectionLevel;
    }

    public void setCollectionLevel(int collectionLevel) {
        this.collectionLevel = collectionLevel;
    }

    public Object getTrustedRolesAndUsers() {
        return trustedRolesAndUsers;
    }

    public void setTrustedRolesAndUsers(Object trustedRolesAndUsers) {
        this.trustedRolesAndUsers = trustedRolesAndUsers;
    }

    public Object getPrivilegedRolesAndUsers() {
        return privilegedRolesAndUsers;
    }

    public void setPrivilegedRolesAndUsers(Object privilegedRolesAndUsers) {
        this.privilegedRolesAndUsers = privilegedRolesAndUsers;
    }

    public RegulationSettings getRegulationSettings() {
        return regulationSettings;
    }

    public void setRegulationSettings(RegulationSettings regulationSettings) {
        this.regulationSettings = regulationSettings;
    }

    public Object getAvailabilityGroupList() {
        return availabilityGroupList;
    }

    public void setAvailabilityGroupList(Object availabilityGroupList) {
        this.availabilityGroupList = availabilityGroupList;
    }

    public CMAuditedActivities getAuditedActivities() {
        return auditedActivities;
    }

    public void setAuditedActivities(CMAuditedActivities auditedActivities) {
        this.auditedActivities = auditedActivities;
    }

    public CMAuditedActivities getUserAuditedActivities() {
        return userAuditedActivities;
    }

    public void setUserAuditedActivities(
	    CMAuditedActivities userAuditedActivities) {
	this.userAuditedActivities = userAuditedActivities;
    }

    public CMDmlSelectFilters getDmlSelectFilters() {
        return dmlSelectFilters;
    }

    public void setDmlSelectFilters(CMDmlSelectFilters dmlSelectFilters) {
        this.dmlSelectFilters = dmlSelectFilters;
    }

    public Boolean getAuditExceptions() {
        return auditExceptions;
    }

    public void setAuditExceptions(Boolean auditExceptions) {
        this.auditExceptions = auditExceptions;
    }
}
