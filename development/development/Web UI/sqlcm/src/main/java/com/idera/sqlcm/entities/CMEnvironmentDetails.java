package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMEnvironmentDetails {

	@JsonProperty("registeredSqlServerCount")
	private long registeredSqlServerCount;

	@JsonProperty("auditedSqlServerCount")
	private long auditedSqlServerCount;

	@JsonProperty("auditedDatabaseCount")
	private long auditedDatabaseCount;

	@JsonProperty("processedEventCount")
	private long processedEventCount;

	@JsonProperty("environmentHealth")
	private int environmentHealth;

	public CMEnvironmentDetails() {
	}

	public long getRegisteredSqlServerCount() {
		return registeredSqlServerCount;
	}

	public void setRegisteredSqlServerCount(long registeredSqlServerCount) {
		this.registeredSqlServerCount = registeredSqlServerCount;
	}

	public long getAuditedSqlServerCount() {
		return auditedSqlServerCount;
	}

	public void setAuditedSqlServerCount(long auditedSqlServerCount) {
		this.auditedSqlServerCount = auditedSqlServerCount;
	}

	public long getAuditedDatabaseCount() {
		return auditedDatabaseCount;
	}

	public void setAuditedDatabaseCount(long auditedDatabaseCount) {
		this.auditedDatabaseCount = auditedDatabaseCount;
	}

	public long getProcessedEventCount() {
		return processedEventCount;
	}

	public void setProcessedEventCount(long processedEventCount) {
		this.processedEventCount = processedEventCount;
	}

	public int getEnvironmentHealth() {
		return environmentHealth;
	}

	public void setEnvironmentHealth(int environmentHealth) {
		this.environmentHealth = environmentHealth;
	}

	@Override
	public String toString() {
		return "CMEnvironmentDetails{" +
			"registeredSqlServerCount=" + registeredSqlServerCount +
			", auditedSqlServerCount=" + auditedSqlServerCount +
			", auditedDatabaseCount=" + auditedDatabaseCount +
			", processedEventCount=" + processedEventCount +
			", environmentHealth=" + environmentHealth +
			'}';
	}
}
