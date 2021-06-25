package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditListConfigurationResponse  {
	
	@JsonProperty("ConfigurationCheck")
    private List<CMAuditConfigurationResponse> AuditConfiguration;    
	
	public List<CMAuditConfigurationResponse> getAuditConfiguration() {
		return AuditConfiguration;
	}

	public void setAuditConfiguration(
			List<CMAuditConfigurationResponse> auditConfig) {
		AuditConfiguration = auditConfig;
	}

	public CMAuditListConfigurationResponse()
    {}

	
    @Override
    public String toString() {
        return "CMAuditListConfigurationResponse{" +
            "AuditConfiguration=" + AuditConfiguration +
            '}';
    }
}
