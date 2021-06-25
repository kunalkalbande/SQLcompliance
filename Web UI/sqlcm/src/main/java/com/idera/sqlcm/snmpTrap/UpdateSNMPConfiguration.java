package com.idera.sqlcm.snmpTrap;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class UpdateSNMPConfiguration {

    @JsonProperty("snmpServerAddress")
    String snmpServerAddress;

	@JsonProperty("port")
    int port;
    
    @JsonProperty("community")
    String community;
    
    
    public String getSnmpServerAddress() {
		return snmpServerAddress;
	}

	public void setSnmpServerAddress(String snmpServerAddress) {
		this.snmpServerAddress = snmpServerAddress;
	}


	public int getPort() {
		return port;
	}


	public void setPort(int port) {
		this.port = port;
	}


	public String getCommunity() {
		return community;
	}


	public void setCommunity(String community) {
		this.community = community;
	}

    @Override
    public String toString() {
        return "UpdateSNMPConfiguration{" +
            "snmpServerAddress=" + snmpServerAddress +
            ", port=" + port +
            ", community=" + community +
            '}';
    }
}