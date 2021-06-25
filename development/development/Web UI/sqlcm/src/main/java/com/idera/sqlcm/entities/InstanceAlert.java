package com.idera.sqlcm.entities;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class InstanceAlert extends CMEntity {

	private long instanceId;
	private CMStatus alertStatus;
	private RuleType alertType;
	private String instanceName;
	private String sourceRule;
	private Date time;

	public long getInstanceId() {
		return instanceId;
	}

	public void setInstanceId(long instanceId) {
		this.instanceId = instanceId;
	}

	public CMStatus getAlertStatus() {
		return alertStatus;
	}

	public void setAlertStatus(CMStatus alertStatus) {
		this.alertStatus = alertStatus;
	}

	public RuleType getAlertType() {
		return alertType;
	}

	public void setAlertType(RuleType alertType) {
		this.alertType = alertType;
	}

	public String getInstanceName() {
		return instanceName;
	}

	public void setInstanceName(String instanceName) {
		this.instanceName = instanceName;
	}

	public String getSourceRule() {
		return sourceRule;
	}

	public void setSourceRule(String sourceRule) {
		this.sourceRule = sourceRule;
	}

	public Date getTime() {
		return time;
	}

	public void setTime(Date time) {
		this.time = time;
	}

}
