package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditListUserActivityResponse  {
	
	@JsonProperty("AuditUserActivity")
    private List<CMAuditUserActivityResponse> UserActivityResponse;    	

	public List<CMAuditUserActivityResponse> getUserActivityResponse() {
		return UserActivityResponse;
	}

	public void setUserActivityResponse(List<CMAuditUserActivityResponse> userActivityResponse) {
		UserActivityResponse = userActivityResponse;
	}

	public CMAuditListUserActivityResponse()
    {}

	
    @Override
    public String toString() {
        return "CMLAuditListUserActivityResponse{" +
            "UserActivityResponse=" + UserActivityResponse +
            '}';
    }
}
