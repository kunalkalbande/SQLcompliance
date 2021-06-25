package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAlertRules extends CMEntity{

	@JsonProperty("ruleId")
    private Long ruleId;

    @JsonProperty("names")
    private String names;
    
    @JsonProperty("description")
    private String description;

    @JsonProperty("targetInstances")
    private String targetInstances;

    @JsonProperty("alertLevel")
    private int alertLevel;
    
    @JsonProperty("instanceId")
    private long instanceId;
    
	@JsonProperty("alertType")
    private int alertType;

    @JsonProperty("message")
    private String message;
    
    @JsonProperty("logMessage")
    private int logMessage;

	@JsonProperty("emailMessage")
    private int emailMessage;

	@JsonProperty("snmpTrap")
    private int snmpTrap;
	
	@JsonProperty("enabled")
    private boolean enabled;
	
	@JsonProperty("snmpServerAddress")
    private String snmpServerAddress;
	  
	@JsonProperty("snmpPort")
    private int snmpPort;
	
	@JsonProperty("snmpCommunity")
    private String snmpCommunity;
	
	@JsonProperty("ruleValidation")
	private int ruleValidation;
	
	private String ruleType;
	
	private String levelUI;

	public String getLevelUI() {
		if(alertLevel == 1)return "Low";
		else if (alertLevel == 2) return "Medium";
		else if (alertLevel == 3)return "High";
		else return "Severe";
	}

	public void setLevelUI(String levelUI) {
		this.levelUI = levelUI;
	}

	public String getRuleType() {
		if(alertType == 1)return "Event";
		else if (alertType == 2) return "Status";
		else return "Data";
	}

	public void setRuleType(String ruleType) {
		this.ruleType = ruleType;
	}

	public int getRuleValidation() {
		return ruleValidation;
	}

	public void setRuleValidation(int ruleValidation) {
		this.ruleValidation = ruleValidation;
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}

	public String getMessage() {
		return message;
	}

	public void setMessage(String message) {
		this.message = message;
	}

	public String getSnmpServerAddress() {
		return snmpServerAddress;
	}

	public void setSnmpServerAddress(String snmpServerAddress) {
		this.snmpServerAddress = snmpServerAddress;
	}

	public int getSnmpPort() {
		return snmpPort;
	}

	public void setSnmpPort(int snmpPort) {
		this.snmpPort = snmpPort;
	}

	public String getSnmpCommunity() {
		return snmpCommunity;
	}

	public void setSnmpCommunity(String snmpCommunity) {
		this.snmpCommunity = snmpCommunity;
	}

	public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
	}

	public int getAlertLevel() {
		return alertLevel;
	}

    public void setAlertLevel(int alertLevel) {
		this.alertLevel = alertLevel;
	}

    public int getAlertType() {
		return alertType;
	}

	public void setAlertType(int alertType) {
		this.alertType = alertType;
	}

	public int getEmailMessage() {
		return emailMessage;
	}

	public void setEmailMessage(int emailMessage) {
		this.emailMessage = emailMessage;
	}


	public int getSnmpTrap() {
		return snmpTrap;
	}


	public void setSnmpTrap(int snmpTrap) {
		this.snmpTrap = snmpTrap;
	}

     public CMAlertRules() {
    }

    public Long getRuleId() {
		return ruleId;
	}

	public void setRuleId(Long ruleId) {
		this.ruleId = ruleId;
	}

	public String getNames() {
		return name;
	}

	public void setNames(String name) {
		this.name = name;
	}

	public String getTargetInstances() {
		return targetInstances;
	}

	public long getInstanceId() {
		return instanceId;
	}

	public void setInstanceId(long instanceId) {
		this.instanceId = instanceId;
	}

	public void setTargetInstances(String targetInstances) {
		this.targetInstances = targetInstances;
	}

    public int getLogMessage() {
		return logMessage;
	}

	public void setLogMessage(int logMessage) {
		this.logMessage = logMessage;
	}


	@Override
    public String toString() {
        return "CMAlertRules{" +
            "ruleId=" + ruleId +
            ", names='" + names + '\'' +
            ", targetInstances='" + targetInstances + '\'' +
            ", alertLevel=" + alertLevel +
            ", alertType=" + alertType +
            ", targetInstances=" + targetInstances +
            ", logMessage=" + logMessage +
            ", emailMessage=" + emailMessage +
            ", emailMessage=" + emailMessage +
            ", snmpTrap=" + snmpTrap +
            ", description='" + description + '\'' +
            ", message='" + message + '\'' +
            ", snmpServerAddress='" + snmpServerAddress + '\'' +
            ", snmpPort='" + snmpPort + 
            ", snmpCommunity='" + snmpCommunity + '\'' +
            ", enabled=" + enabled +
            ", ruleValidation=" + ruleValidation +
            '}';
    }
}
