package com.idera.sqlcm.ui.auditReports;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;


@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditConfiguration {

	@JsonProperty("instance")
	private String instance;

	@JsonProperty("database")
	private String database;

	@JsonProperty("default")
	private int defaultStatus;    

	@JsonProperty("setting")
	private int setting;    

	@Override
	public String toString() {
		return "UpdateSNMPConfiguration{" +
				"server=" + instance +
				", database=" + database +
				", defaultStatus=" + defaultStatus +
				", auditSettings=" + setting +
				'}';
	}

	public String getInstance() {
		return instance;
	}

	public void setInstance(String instance) {
		this.instance = instance;
	}
	public String getDatabase() {
		return database;
	}
	public void setDatabase(String database) {
		this.database = database;
	}

	public int getDefaultStatus() {
		return defaultStatus;
	}

	public void setDefaultStatus(int defaultStatus) {
		this.defaultStatus = defaultStatus;
	}

	public int getSetting() {
		return setting;
	}

	public void setSetting(int setting) {
		this.setting = setting;
	}

}
