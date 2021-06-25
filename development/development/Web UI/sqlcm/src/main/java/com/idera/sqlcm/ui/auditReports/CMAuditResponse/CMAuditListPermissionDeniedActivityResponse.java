package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditListPermissionDeniedActivityResponse  {
	
	@JsonProperty("AuditPermissionDeniedActivity")
    private List<CMAuditPermissionDeniedActivityResponse> PermissionDeniedResponse;    	

	public List<CMAuditPermissionDeniedActivityResponse> getPermissionDeniedResponse() {
		return PermissionDeniedResponse;
	}

	public void setPermissionDeniedResponse(List<CMAuditPermissionDeniedActivityResponse> permissionDeniedResponse) {
		PermissionDeniedResponse = permissionDeniedResponse;
	}

	public CMAuditListPermissionDeniedActivityResponse()
    {}

	
    @Override
    public String toString() {
        return "CMLAuditListPermissionDeniedResponse{" +
            "PermissionDeniedResponse=" + PermissionDeniedResponse +
            '}';
    }
}
