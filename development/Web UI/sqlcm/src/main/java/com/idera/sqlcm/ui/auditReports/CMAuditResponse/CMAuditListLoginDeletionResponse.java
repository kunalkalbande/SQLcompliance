package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditListLoginDeletionResponse  {
	
	@JsonProperty("AuditLoginDeletion")
    private List<CMAuditLoginDeletionResponse> LoginDeletionResponse;    	

	public List<CMAuditLoginDeletionResponse> getLoginDeletionResponse() {
		return LoginDeletionResponse;
	}

	public void setLoginDeletionResponse(List<CMAuditLoginDeletionResponse> loginDeletionResponse) {
		LoginDeletionResponse = loginDeletionResponse;
	}

	public CMAuditListLoginDeletionResponse()
    {}

	
    @Override
    public String toString() {
        return "CMLAuditListLoginDeletionResponse{" +
            "LoginDeletionResponse=" + LoginDeletionResponse +
            '}';
    }
}
