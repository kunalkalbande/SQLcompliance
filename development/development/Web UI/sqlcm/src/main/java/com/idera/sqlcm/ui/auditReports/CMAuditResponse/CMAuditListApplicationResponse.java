package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditListApplicationResponse  {
	
	@JsonProperty("AuditApplication")
    private List<CMAuditApplicationResponse> AuditApplication;    
	
	public List<CMAuditApplicationResponse> getAuditApplication() {
		return AuditApplication;
	}

	public void setAuditApplication(
			List<CMAuditApplicationResponse> auditApplication) {
		AuditApplication = auditApplication;
	}

	public CMAuditListApplicationResponse()
    {}

	
    @Override
    public String toString() {
        return "CMAuditListApplicationResponse{" +
            "AuditApplication=" + AuditApplication +
            '}';
    }
}
