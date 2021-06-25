package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

/**
 * @author Amarendra
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class EnvironmentAlerts {

	public static class EVAlert {
		public long high;
		public long low;
		public long medium;
		public long severe;
		public long total;
	}

	private EVAlert auditedInstances;
	private EVAlert auditedDatabases;

	/**
	 * @return the auditedInstances
	 */
	public EVAlert getAuditedInstances() {
		return auditedInstances;
	}

	/**
	 * @param auditedInstances
	 *            the auditedInstances to set
	 */
	public void setAuditedInstances(EVAlert auditedInstances) {
		this.auditedInstances = auditedInstances;
	}

	/**
	 * @return the auditedDatabases
	 */
	public EVAlert getAuditedDatabases() {
		return auditedDatabases;
	}

	/**
	 * @param auditedDatabases
	 *            the auditedDatabases to set
	 */
	public void setAuditedDatabases(EVAlert auditedDatabases) {
		this.auditedDatabases = auditedDatabases;
	}

}
