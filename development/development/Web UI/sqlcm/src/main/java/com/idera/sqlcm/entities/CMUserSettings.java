package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMUserSettings {
    @JsonProperty("dashboardUserId")
    private long dashboardUserId;
    @JsonProperty("account")
    private String account;
    @JsonProperty("email")
    private String email;
    @JsonProperty("sessionTimout")
    private Long sessionTimeout;
    @JsonProperty("subscribed")
    private boolean subscribed;

    public long getDashboardUserId() {
        return dashboardUserId;
    }

    public void setDashboardUserId(long dashboardUserId) {
        this.dashboardUserId = dashboardUserId;
    }

    public String getAccount() {
        return account;
    }

    public void setAccount(String account) {
        this.account = account;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public Long getSessionTimeout() {
        return sessionTimeout;
    }

    public void setSessionTimeout(Long sessionTimeout) {
        this.sessionTimeout = sessionTimeout;
    }

    public boolean isSubscribed() {
        return subscribed;
    }

    public void setSubscribed(boolean subscribed) {
        this.subscribed = subscribed;
    }
}
