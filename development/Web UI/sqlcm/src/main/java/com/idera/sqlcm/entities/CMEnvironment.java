package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMEnvironment {

	private int auditedDatabaseCount;
	private int auditedSqlServerCount;
	private long processedEventCount;
	private int registeredSqlServerCount;

	public int getAuditedDatabaseCount() {
		return auditedDatabaseCount;
	}

	public void setAuditedDatabaseCount(int auditedDatabaseCount) {
		this.auditedDatabaseCount = auditedDatabaseCount;
	}

	public int getAuditedSqlServerCount() {
		return auditedSqlServerCount;
	}

	public void setAuditedSqlServerCount(int auditedSqlServerCount) {
		this.auditedSqlServerCount = auditedSqlServerCount;
	}

	public long getProcessedEventCount() {
		return processedEventCount;
	}

	public void setProcessedEventCount(long processedEventCount) {
		this.processedEventCount = processedEventCount;
	}

	public int getRegisteredSqlServerCount() {
		return registeredSqlServerCount;
	}

	public void setRegisteredSqlServerCount(int registeredSqlServerCount) {
		this.registeredSqlServerCount = registeredSqlServerCount;
	}
}
