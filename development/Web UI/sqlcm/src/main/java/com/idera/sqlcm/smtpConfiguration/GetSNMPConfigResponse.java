package com.idera.sqlcm.smtpConfiguration;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditApplicationResponse;

@JsonIgnoreProperties(ignoreUnknown = true)
public class GetSNMPConfigResponse {
	
	
		@JsonProperty("SNMPConfig")
	    private List<GetSNMPConfigResponseList> SNMPConfig;    
		
		
		public List<GetSNMPConfigResponseList> getGetSNMPConfigResponse() {
			return SNMPConfig;
		}


		public void setGetSNMPConfigResponse(
				List<GetSNMPConfigResponseList> getSNMPConfigResponse) {
			SNMPConfig = getSNMPConfigResponse;
		}


		public GetSNMPConfigResponse()       
	    {}

		
	    @Override
	    public String toString() {
	        return "GetSNMPConfigResponse{" +
	            "SNMPConfigResponse=" + SNMPConfig +
	            '}';
	    }
	}
