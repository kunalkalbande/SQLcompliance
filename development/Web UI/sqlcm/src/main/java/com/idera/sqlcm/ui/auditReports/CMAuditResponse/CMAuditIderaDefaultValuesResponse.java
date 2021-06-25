package com.idera.sqlcm.ui.auditReports.CMAuditResponse;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.ui.auditReports.modelData.DefaultKeyValuePairs;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditIderaDefaultValuesResponse  {
	 @JsonProperty("defaultProperties")
	  List<DefaultKeyValuePairs> serverDefaults;

	public List<DefaultKeyValuePairs> getServerDefaults() {
		return serverDefaults;
	}

	public void setServerDefaults(List<DefaultKeyValuePairs> serverDefaults) {
		this.serverDefaults = serverDefaults;
	}
	
	public Map<String, String> getDefaultMap(){
		Map<String, String> map = new HashMap<String,String>();
		for (DefaultKeyValuePairs defaultKeyValuePairs : serverDefaults) {
			map.put(defaultKeyValuePairs.getKey(), (defaultKeyValuePairs.getValue()));
		}
		return map;
	
	}
   }/*
public class DefaultKeyValuePairs{
	 @JsonProperty("key")
	String key;
	 
	 @JsonProperty("value")
	String value;
}*/
/*abstract class ServerDefaults{
	 @JsonProperty("Logins")
	    int Logins;

	 @JsonProperty("FailedLogins")
	    int FailedLogins;

	public int getLogins() {
		return Logins;
	}

	public void setLogins(int logins) {
		Logins = logins;
	}

	public int getFailedLogins() {
		return FailedLogins;
	}

	public void setFailedLogins(int failedLogins) {
		FailedLogins = failedLogins;
	}
}

abstract class DatabaseDefaults{
	 @JsonProperty("DatabaseDefination")
	    int DatabaseDefination;

	 @JsonProperty("SecurityChanges")
	    int SecurityChanges;

	public int getDatabaseDefination() {
		return DatabaseDefination;
	}

	public void setDatabaseDefination(int databaseDefination) {
		DatabaseDefination = databaseDefination;
	}

	public int getSecurityChanges() {
		return SecurityChanges;
	}

	public void setSecurityChanges(int securityChanges) {
		SecurityChanges = securityChanges;
	}
}*/