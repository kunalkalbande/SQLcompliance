package com.idera.sqlcm.entities;


import com.fasterxml.jackson.annotation.JsonProperty;

public class CMDomainCredentials {
    @JsonProperty("account")
    private String account;

    @JsonProperty("password")
    private String password;

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
