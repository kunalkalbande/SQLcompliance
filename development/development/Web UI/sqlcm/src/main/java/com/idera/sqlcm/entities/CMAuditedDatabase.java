package com.idera.sqlcm.entities;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditedDatabase extends CMEntity {

	private String instance;
	private List<String> auditedActivities;
	private List<String> regulationGuidelines;
	private int beforeAfterTableCount;
	private int sensitiveColumnsTableCount;
	private int trustedUserCount;
	private boolean isEnabled;

	public String getInstance() {
		return instance;
	}

	public void setInstance(String instance) {
		this.instance = instance;
	}

	public String getAuditedActivities() {
		String result = "";
		if (auditedActivities != null) {
			result = auditedActivities.toString();
		}
		return result;
	}

	public void setAuditedActivities(List<String> auditedActivities) {
		this.auditedActivities = auditedActivities;
	}

	public List<String> getRegulationGuidelines() {
		return regulationGuidelines;
	}

	public void setRegulationGuidelines(List<String> regulationGuidelines) {
		this.regulationGuidelines = regulationGuidelines;
	}

	public int getBeforeAfterTableCount() {
		return beforeAfterTableCount;
	}

	public void setBeforeAfterTableCount(int beforeAfterTableCount) {
		this.beforeAfterTableCount = beforeAfterTableCount;
	}

	public int getSensitiveColumnsTableCount() {
		return sensitiveColumnsTableCount;
	}

	public void setSensitiveColumnsTableCount(int sensitiveColumnsTableCount) {
		this.sensitiveColumnsTableCount = sensitiveColumnsTableCount;
	}

	public int getTrustedUserCount() {
		return trustedUserCount;
	}

	public void setTrustedUserCount(int trustedUserCount) {
		this.trustedUserCount = trustedUserCount;
	}

	public boolean isEnabled() {
		return isEnabled;
	}

	public void setIsEnabled(boolean isEnabled) {
		this.isEnabled = isEnabled;
	}
}
