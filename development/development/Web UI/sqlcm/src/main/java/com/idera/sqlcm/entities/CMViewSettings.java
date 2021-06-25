package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMViewSettings extends CMEntity {

    @JsonProperty("timeout")
    private long timeout;

    @JsonProperty("filter")
    private String filter;

    @JsonProperty("viewId")
    private String viewId;

    @JsonProperty("userId")
    private String userId;
    
    @JsonProperty("viewName")
    private String viewName;

    public CMViewSettings() {
    }

    public long getTimeout() {
        return timeout;
    }

    public void setTimeout(long timeout) {
        this.timeout = timeout;
    }

    public String getFilter() {
        return filter;
    }

    public void setFilter(String filter) {
        this.filter = filter;
    }

    public String getViewId() {
        return viewId;
    }

    public void setViewId(String viewId) {
        this.viewId = viewId;
    }

    public String getUserId() {
        return userId;
    }

    public void setUserId(String userId) {
        this.userId = userId;
    }

    public String getViewName() {
		return viewName;
	}

	public void setViewName(String viewName) {
		this.viewName = viewName;
	}
    @Override
    public String toString() {
        return "CMViewSettings{" +
            "timeout=" + timeout +
            ", filter='" + filter + '\'' +
            ", viewId='" + viewId + '\'' +
            ", userId='" + userId + '\'' +
            ", viewName='" + viewName + '\'' +
            '}';
    }
}
