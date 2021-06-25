package com.idera.sqlcm.ui.dialogs.adddatabasewizard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class ServerSettingsData {
	
	@JsonProperty("serverId")
    private int serverId;

	public int getServerId() {
		return serverId;
	}

	public void setServerId(int serverId) {
		this.serverId = serverId;
	}
}
