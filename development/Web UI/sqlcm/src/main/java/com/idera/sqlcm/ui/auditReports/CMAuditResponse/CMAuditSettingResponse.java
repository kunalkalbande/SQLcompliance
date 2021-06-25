package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditSettingResponse  {
	 @JsonProperty("ConfigurationCheck")
	  List<String> auditSettingsList;

	public List<String> getAuditSettingsList() {
		return auditSettingsList;
	}
	
	public void setAuditSettingsList(List<String> list) {
		 auditSettingsList = list;
	}
   }
