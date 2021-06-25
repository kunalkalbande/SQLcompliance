/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.ui.importAuditSetting;

import com.fasterxml.jackson.annotation.JsonProperty;

public class ServerDetails {
	
	@JsonProperty("serverId")
	int serverId;
	
	@JsonProperty("serverName")
	String serverName;
	
	public int getServerId() {
		return serverId;
	}

	public void setServerId(int serverId) {
		this.serverId = serverId;
	}

	public String getServerName() {
		return serverName;
	}

	public void setServerName(String serverName) {
		this.serverName = serverName;
	}
}

/***End SQLCm 5.4***/