package com.idera.sqlcm.facade;

import com.fasterxml.jackson.annotation.JsonProperty;

public class SNMPConfigData {
	
	 @JsonProperty("snmpAddress")
	 String snmpAddress;	 
	 
	@JsonProperty("port")
	 int port;
	 
	 @JsonProperty("community")
	 String community;
	
	public String getSnmpAddress() {
		return snmpAddress;
	}

	public void setSnmpAddress(String snmpAddress) {
		this.snmpAddress = snmpAddress;
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
        return "SNMPConfigData{" +
            "snmpAddress=" + snmpAddress +
            ", port=" + port +
            ", community=" + community +
            '}';
    }
}
