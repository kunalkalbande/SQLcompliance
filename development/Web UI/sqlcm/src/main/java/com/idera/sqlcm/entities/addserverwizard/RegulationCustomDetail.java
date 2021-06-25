package com.idera.sqlcm.entities.addserverwizard;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;

public class RegulationCustomDetail {

	@JsonProperty("auditedServerActivities")
    private CMAuditedActivities auditedServerActivities;
	
	@JsonProperty("auditedDatabaseActivities")
    private CMAuditedActivities auditedDatabaseActivities;

	public CMAuditedActivities getAuditedServerActivities() {
		return auditedServerActivities;
	}

	public void setAuditedServerActivities(CMAuditedActivities auditedServerActivities) {
		this.auditedServerActivities = auditedServerActivities;
	}

	public CMAuditedActivities getAuditedDatabaseActivities() {
		return auditedDatabaseActivities;
	}

	public void setAuditedDatabaseActivities(CMAuditedActivities auditedDatabaseActivities) {
		this.auditedDatabaseActivities = auditedDatabaseActivities;
	}
}
