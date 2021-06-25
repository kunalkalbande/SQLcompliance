package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditListObjectActivityResponse  {
	
	@JsonProperty("AuditObjectActivity")
    private List<CMAuditObjectActivityResponse> AuditObjectActivity;    	

	public List<CMAuditObjectActivityResponse> getObjectActivityResponse() {
		return AuditObjectActivity;
	}

	public void setObjectActivityResponse(List<CMAuditObjectActivityResponse> objectActivityResponse) {
		AuditObjectActivity = objectActivityResponse;
	}

	public CMAuditListObjectActivityResponse()
    {}

	
    @Override
    public String toString() {
        return "CMLAuditListObjectActivityResponse{" +
            "ObjectActivityResponse=" + AuditObjectActivity +
            '}';
    }
}
