package com.idera.sqlcm.entities.addserverwizard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class AgentServiceAccount {

    @JsonProperty("account")
    private String account;

    @JsonProperty("password")
    private String password;

    public AgentServiceAccount() {
    }

    public AgentServiceAccount(String account, String password) {
        this.account = account;
        this.password = password;
    }

    public String getAccount() {
        return account;
    }

    public void setAccount(String account) {
        this.account = account;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }
}
