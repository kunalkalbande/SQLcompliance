package com.idera.sqlcm.ui.auditReports;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditRegulatoryCompliance {
	
	@JsonProperty("serverName")
    private String serverName;

    @JsonProperty("databaseName")
    private String databaseName;
    
    @JsonProperty("auditSettings")
    private int auditSettings;
    
    @JsonProperty("regulationGuidelines")
    private int regulationGuidelines;
    
    @JsonProperty("values")
    private int values;
    
    @Override
    public String toString() {
        return "UpdateSNMPConfiguration{" +
            "serverName=" + serverName +
            ",databaseName=" + databaseName+
            ",auditSettings=" + auditSettings +
            ",regulationGuidelines=" + regulationGuidelines +
            ",values=" + values +
            '}';
    }

	public String getServerName() {
		return serverName;
	}

	public void setServerName(String serverName) {
		this.serverName = serverName;
	}

	public String getDatabaseName() {
		return databaseName;
	}

	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}

	public int getAuditSettings() {
		return auditSettings;
	}

	public void setAuditSettings(int auditSettings) {
		this.auditSettings = auditSettings;
	}

	public int getRegulationGuidelines() {
		return regulationGuidelines;
	}

	public void setRegulationGuidelines(int regulationGuidelines) {
		this.regulationGuidelines = regulationGuidelines;
	}

	public int getValues() {
		return values;
	}

	public void setValues(int values) {
		this.values = values;
	}
	
}
