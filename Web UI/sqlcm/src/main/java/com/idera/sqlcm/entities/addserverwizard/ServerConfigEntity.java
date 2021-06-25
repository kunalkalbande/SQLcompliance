package com.idera.sqlcm.entities.addserverwizard;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMDmlSelectFilters;
import com.idera.sqlcm.entities.RegulationSettings;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;

public class ServerConfigEntity {

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
    private boolean auditExceptions = false;

    @JsonProperty("sensitiveColumnTableDictionary")
    private Object sensitiveColumnTableDictionary;

    @JsonProperty("updateServerSettings")
    private boolean updateServerSettings;
    
    @JsonProperty("auditedServerActivities")
    private CMAuditedActivities auditedServerActivities;

	@JsonProperty("serverSettingsToBeUpdated")
    private ServerSettingsToBeUpdated serverSettingsToBeUpdated;
	
	@JsonProperty("isServerType")
    private boolean isServerType;

    public boolean isServerType() {
		return isServerType;
	}

	public void setServerType(boolean isServerType) {
		this.isServerType = isServerType;
	}

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

    public void setUserAuditedActivities(CMAuditedActivities userAuditedActivities) {
        this.userAuditedActivities = userAuditedActivities;
    }

    public CMDmlSelectFilters getDmlSelectFilters() {
        return dmlSelectFilters;
    }

    public void setDmlSelectFilters(CMDmlSelectFilters dmlSelectFilters) {
        this.dmlSelectFilters = dmlSelectFilters;
    }

    public boolean isAuditExceptions() {
        return auditExceptions;
    }

    public void setAuditExceptions(boolean auditExceptions) {
        this.auditExceptions = auditExceptions;
    }

    public Object getSensitiveColumnTableDictionary() {
        return sensitiveColumnTableDictionary;
    }

    public void setSensitiveColumnTableDictionary(Object sensitiveColumnTableDictionary) {
        this.sensitiveColumnTableDictionary = sensitiveColumnTableDictionary;
    }

    public boolean isUpdateServerSettings() {
        return updateServerSettings;
    }

    public void setUpdateServerSettings(boolean updateServerSettings) {
        this.updateServerSettings = updateServerSettings;
    }

    public ServerSettingsToBeUpdated getServerSettingsToBeUpdated() {
        return serverSettingsToBeUpdated;
    }

    public void setServerSettingsToBeUpdated(ServerSettingsToBeUpdated serverSettingsToBeUpdated) {
        this.serverSettingsToBeUpdated = serverSettingsToBeUpdated;
    }
    
    public CMAuditedActivities getAuditedServerActivities() {
		return auditedServerActivities;
	}

	public void setAuditedServerActivities(CMAuditedActivities auditedServerActivities) {
		this.auditedServerActivities = auditedServerActivities;
	}
}
