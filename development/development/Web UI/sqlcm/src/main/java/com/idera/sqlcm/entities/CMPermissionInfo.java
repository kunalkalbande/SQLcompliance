package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

public class CMPermissionInfo extends CMEntity {

    @JsonProperty("totalChecks")
    private int totalChecks;

    @JsonProperty("passedChecks")
    private int passedChecks;

    @JsonProperty("failedChecks")
    private int failedChecks;

    @JsonProperty("permissionsCheckList")
    private List<CMPermission> permissionsCheckList;

    public int getTotalChecks() {
        return totalChecks;
    }

    public void setTotalChecks(int totalChecks) {
        this.totalChecks = totalChecks;
    }

    public int getPassedChecks() {
        return passedChecks;
    }

    public void setPassedChecks(int passedChecks) {
        this.passedChecks = passedChecks;
    }

    public int getFailedChecks() {
        return failedChecks;
    }

    public void setFailedChecks(int failedChecks) {
        this.failedChecks = failedChecks;
    }

    public List<CMPermission> getPermissionsCheckList() {
        return permissionsCheckList;
    }

    public void setPermissionsCheckList(List<CMPermission> permissionsCheckList) {
        this.permissionsCheckList = permissionsCheckList;
    }
}
