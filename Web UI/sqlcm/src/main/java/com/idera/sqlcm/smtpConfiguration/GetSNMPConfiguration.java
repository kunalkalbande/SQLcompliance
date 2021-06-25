package com.idera.sqlcm.smtpConfiguration;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class GetSNMPConfiguration {

	@JsonProperty("instanceName")
    String instanceName;
    
    public String getInstanceName() {
		return instanceName;
	}

	public void setInstanceName(String instanceName) {
		this.instanceName = instanceName;
	}

	@Override
    public String toString() {
        return "GetSNMPConfiguration{" +
            "instanceName=" + instanceName +
            '}';
    }
}