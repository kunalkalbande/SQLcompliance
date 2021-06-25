package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMServerAuditingRequest {

    @JsonProperty("serverIdList")
    private List<Long> serverIdList;

    @JsonProperty("enable")
    private boolean enable;

    public CMServerAuditingRequest() {
    }

    public List<Long> getServerIdList() {
        return serverIdList;
    }

    public void setServerIdList(List<Long> serverIdList) {
        this.serverIdList = serverIdList;
    }

    public boolean isEnable() {
        return enable;
    }

    public void setEnable(boolean enable) {
        this.enable = enable;
    }

    @Override
    public String toString() {
        return "CMServerAuditingRequest{" +
            "serverIdList=" + serverIdList +
            ", enable=" + enable +
            '}';
    }
}
