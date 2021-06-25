package com.idera.sqlcm.ui.dialogs.addalertruleswizard;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMDmlSelectFilters;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.wizard.IWizardEntity;

import java.util.List;
import java.util.Map;

public class AddAlertRulesSaveEntity extends CMEntity implements IWizardEntity {

	@JsonProperty("newEventAlertRules")
	private NewEventAlertRules newEventAlertRules;

	@JsonProperty("collectionLevel")
	private int collectionLevel;

	@JsonProperty("targetInstances")
	private String targetInstances;

	@JsonProperty("matchString")
	Map<String, String> matchString;

	@JsonProperty("trustedRolesAndUsers")
	private Object trustedRolesAndUsers;

	@JsonProperty("privilegedRolesAndUsers")
	private Object privilegedRolesAndUsers;

	@JsonProperty("regulationSettings")
	private RegulationSettings regulationSettings;

	@JsonProperty("availabilityGroupList")
	private Object availabilityGroupList;

	@JsonProperty("selectAlertActions")
	private SelectAlertActions selectAlertActions;

	@JsonProperty("userAuditedActivities")
	private CMAuditedActivities userAuditedActivities;

	@JsonProperty("dmlSelectFilters")
	private CMDmlSelectFilters dmlSelectFilters;

	@JsonProperty("auditExceptions")
	private Boolean auditExceptions = false;

	@JsonProperty("sensitiveColumnTableDictionary")
	private Object sensitiveColumnTableDictionary;

	public NewEventAlertRules getNewEventAlertRules() {
		return newEventAlertRules;
	}

	public void setNewEventAlertRules(NewEventAlertRules newEventAlertRules) {
		this.newEventAlertRules = newEventAlertRules;
	}

	public int getCollectionLevel() {
		return collectionLevel;
	}

	public void setCollectionLevel(int collectionLevel) {
		this.collectionLevel = collectionLevel;
	}

	public String getTargetInstances() {
		return targetInstances;
	}

	public void setTargetInstances(String targetInstances) {
		this.targetInstances = targetInstances;
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

	public SelectAlertActions getSelectAlertActions() {
		return selectAlertActions;
	}

	public void setSelectAlertActions(SelectAlertActions selectAlertActions) {
		this.selectAlertActions = selectAlertActions;
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

	public Object getSensitiveColumnTableDictionary() {
		return sensitiveColumnTableDictionary;
	}

	public Map<String, String> getMatchString() {
		return matchString;
	}

	public void setMatchString(Map<String, String> matchString) {
		this.matchString = matchString;
	}

	public void setSensitiveColumnTableDictionary(
			Object sensitiveColumnTableDictionary) {
		this.sensitiveColumnTableDictionary = sensitiveColumnTableDictionary;
	}
}
