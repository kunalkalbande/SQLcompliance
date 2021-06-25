package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditListRowCountResponse  {
	
	@JsonProperty("AuditRowCount")
    private List<CMAuditRowCountResponse> RowCountResponse;    	

	public List<CMAuditRowCountResponse> getRowCountResponse() {
		return RowCountResponse;
	}

	public void setRowCountResponse(List<CMAuditRowCountResponse> rowCountResponse) {
		rowCountResponse = rowCountResponse;
	}

	public CMAuditListRowCountResponse()
    {}

	
    @Override
    public String toString() {
        return "CMLAuditListRowCountResponse{" +
            "RowCountResponse=" + RowCountResponse +
            '}';
    }
}
