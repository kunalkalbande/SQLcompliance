package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditListRegulatoryComplianceResponse {

	@JsonProperty("AuditRegulatoryCompliance")
    private List<CMAuditRegulatoryComplianceResponse> RegulatoryComplianceResponse;    	

	public List<CMAuditRegulatoryComplianceResponse> getRegulatoryComplianceResponse() {
		return RegulatoryComplianceResponse;
	}

	public void setRowCountResponse(List<CMAuditRegulatoryComplianceResponse> RegulatoryComplianceResponse) {
		RegulatoryComplianceResponse = this.RegulatoryComplianceResponse;
	}

	public CMAuditListRegulatoryComplianceResponse()
    {}

	
    @Override
    public String toString() {
        return "CMAuditListRegulatoryComplianceResponse{" +
            "RegulatoryComplianceResponse=" + RegulatoryComplianceResponse +
            '}';
    }
	
}
