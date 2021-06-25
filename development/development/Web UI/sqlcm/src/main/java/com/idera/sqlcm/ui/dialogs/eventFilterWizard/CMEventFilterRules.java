package com.idera.sqlcm.ui.dialogs.eventFilterWizard;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMEventFilterRules {

	@JsonProperty("ruleId")
	private Long ruleId;

	@JsonProperty("name")
	private String name;

	@JsonProperty("description")
	private String description;

	@JsonProperty("eventFilterLevel")
	private String eventFilterLevel;

	@JsonProperty("eventFilterType")
	private Boolean eventFilterType;

	@JsonProperty("targetInstances")
	private Boolean targetInstances;

	@JsonProperty("enabled")
	private Boolean enabled;

	@JsonProperty("message")
	private Boolean message;

	@JsonProperty("logMessage")
	private Boolean logMessage;

	@JsonProperty("emailMessage")
	private Boolean emailMessage;

	@JsonProperty("snmpTrap")
	private Boolean snmpTrap;

	@JsonProperty("snmpServerAddress")
	private Boolean snmpServerAddress;

	@JsonProperty("snmpPort")
	private Boolean snmpPort;

	@JsonProperty("snmpCommunity")
	private Boolean snmpCommunity;

	public CMEventFilterRules() {
	}

	public Long getId() {
		return ruleId;
	}

	public void setId(Long id) {
		this.ruleId = id;
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

	public String getFilterLevel() {
		return eventFilterLevel;
	}

	public void setFilterLevel(String eventFilterLevel) {
		this.eventFilterLevel = eventFilterLevel;
	}

	public Boolean getEventFilterType() {
		return eventFilterType;
	}

	public void setEventFilterType(Boolean eventFilterType) {
		this.eventFilterType = eventFilterType;
	}

	public Boolean getTargetInstances() {
		return targetInstances;
	}

	public void setTargetInstances(Boolean targetInstances) {
		this.targetInstances = targetInstances;
	}

	public Boolean getEnabled() {
		return enabled;
	}

	public void setEnabled(Boolean enabled) {
		this.enabled = enabled;
	}

	public Boolean getMessage() {
		return message;
	}

	public void setMessage(Boolean message) {
		this.message = message;
	}

	public Boolean getLogMessage() {
		return logMessage;
	}

	public void setLogMessage(Boolean logMessage) {
		this.logMessage = logMessage;
	}

	public Boolean getEmailMessage() {
		return emailMessage;
	}

	public void setEmailMessage(Boolean emailMessage) {
		this.emailMessage = emailMessage;
	}

	public Boolean getSnmpTrap() {
		return snmpTrap;
	}

	public void setSnmpTrap(Boolean snmpTrap) {
		this.snmpTrap = snmpTrap;
	}

	public Boolean getSnmpServerAddress() {
		return snmpServerAddress;
	}

	public void setSnmpServerAddress(Boolean snmpServerAddress) {
		this.snmpServerAddress = snmpServerAddress;
	}

	public Boolean getSnmpPort() {
		return snmpPort;
	}

	public void setSnmpPort(Boolean snmpPort) {
		this.snmpPort = snmpPort;
	}

	public Boolean getSnmpCommunity() {
		return snmpCommunity;
	}

	public void setSnmpCommunity(Boolean snmpCommunity) {
		this.snmpCommunity = snmpCommunity;
	}

	@Override
	public String toString() {
		return "CMDatabaseAuditedActivity{" + "id=" + ruleId + ", name='"
				+ name + '\'' + ", description=" + description
				+ ", filterLevel=" + eventFilterLevel + ", filterType="
				+ eventFilterType + ", targetInstances=" + targetInstances
				+ ", enabled=" + enabled + ", message=" + message
				+ ", logMessage=" + logMessage + ", emailMessage="
				+ emailMessage + ", emailMessage=" + emailMessage
				+ ", snmpTrap=" + snmpTrap + ", snmpServerAddress="
				+ snmpServerAddress + ", snmpPort=" + snmpPort
				+ ", snmpCommunity=" + snmpCommunity + '\'' + '}';
	}
}
