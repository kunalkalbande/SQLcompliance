package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditListDMLRespose  {
	
	@JsonProperty("AuditDML")
    private List<CMAuditDMLResponse> DMLResponse;    	

	public List<CMAuditDMLResponse> getDMLResponse() {
		return DMLResponse;
	}

	public void setDMLResponse(List<CMAuditDMLResponse> dMLResponse) {
		DMLResponse = dMLResponse;
	}

	public CMAuditListDMLRespose()
    {}

	
    @Override
    public String toString() {
        return "CMAuditListDMLResponse{" +
            "DMLResponse=" + DMLResponse +
            '}';
    }
}
