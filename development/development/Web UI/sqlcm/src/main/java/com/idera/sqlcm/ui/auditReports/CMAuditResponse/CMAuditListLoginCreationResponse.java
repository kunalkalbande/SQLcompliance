package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditListLoginCreationResponse  {
	
	@JsonProperty("AuditLoginCreation")
    private List<CMAuditLoginCreationResponse> LoginCreationResponse;    	

	public List<CMAuditLoginCreationResponse> getLoginCreationResponse() {
		return LoginCreationResponse;
	}

	public void setLoginCreationResponse(List<CMAuditLoginCreationResponse> loginCreationResponse) {
		LoginCreationResponse = loginCreationResponse;
	}

	public CMAuditListLoginCreationResponse()
    {}

	
    @Override
    public String toString() {
        return "CMLAuditListLoginCreationResponse{" +
            "LoginCreationResponse=" + LoginCreationResponse +
            '}';
    }
}
