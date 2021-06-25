package com.idera.sqlcm.ui.auditReports.modelData;

import com.fasterxml.jackson.annotation.JsonProperty;

public class DefaultKeyValuePairs {
	@JsonProperty("key")
	String key;

	@JsonProperty("value")
	String value;

	public String getKey() {
		return key;
	}

	public void setKey(String key) {
		this.key = key;
	}

	public String getValue() {
		return value;
	}

	public void setValue(String value) {
		this.value = value;
	}
}
