package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMDatabaseAuditingRequest{

    @JsonProperty("databaseIdList")
    private List<Long> databaseIdList;

    @JsonProperty("enable")
    private boolean enable;

    public CMDatabaseAuditingRequest() {
    }

    public List<Long> getDatabaseIdList() {
        return databaseIdList;
    }

    public void setDatabaseIdList(List<Long> databaseIdList) {
        this.databaseIdList = databaseIdList;
    }

    public boolean isEnable() {
        return enable;
    }

    public void setEnable(boolean enable) {
        this.enable = enable;
    }

    @Override
    public String toString() {
        return "CMDatabaseAuditingRequest{" +
            "databaseIdList=" + databaseIdList +
            ", enable=" + enable +
            '}';
    }
}
