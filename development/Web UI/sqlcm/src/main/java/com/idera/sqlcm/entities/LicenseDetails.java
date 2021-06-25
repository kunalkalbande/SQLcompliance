package com.idera.sqlcm.entities;

import java.util.Date;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class LicenseDetails extends CMEntity {

	public static enum LicenseState {
		Valid,
        InvalidKey,
        InvalidProductID,
        InvalidScope,
        InvalidExpired,
        InvalidMixedTypes,
        InvalidDuplicateLicense,
        InvalidProductVersion;
	};

	private LicenseState state;
	private String key;
	private String createdBy;
	private Date createdTime;
	private String scope;

	private List<LicenseDetails> licenses;

	public List<LicenseDetails> getLicenses() {
		return licenses;
	}

	public void setLicenses(List<LicenseDetails> licenses) {
		this.licenses = licenses;
	}

	public LicenseState getState() {
		return state;
	}

	public void setState(LicenseState state) {
		this.state = state;
	}

	public String getKey() {
		return key;
	}

	public void setKey(String key) {
		this.key = key;
	}

	public String getCreatedBy() {
		return createdBy;
	}

	public void setCreatedBy(String createdBy) {
		this.createdBy = createdBy;
	}

	public Date getCreatedTime() {
		return createdTime;
	}

	public void setCreatedTime(Date createdTime) {
		this.createdTime = createdTime;
	}

	public String getScope() {
		return scope;
	}

	public void setScope(String scope) {
		this.scope = scope;
	}

	@Override
	public String toString() {
		StringBuilder builder = new StringBuilder();
		builder.append("LicenseDetails [state=");
		builder.append(state);
		builder.append(", key=");
		builder.append(key);
		builder.append(", createdBy=");
		builder.append(createdBy);
		builder.append(", createdTime=");
		builder.append(createdTime);
		builder.append(", scope=");
		builder.append(scope);
		builder.append(", licenses=[");
		builder.append(licenses);
		builder.append("]]");
		return builder.toString();
	}

}
