package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditedAlerts {

    @JsonProperty("total")
    private int total;

    @JsonProperty("severe")
    private int severe;

    @JsonProperty("high")
    private int high;

    @JsonProperty("medium")
    private int medium;

    @JsonProperty("low")
    private int low;

    public CMAuditedAlerts() {
    }

    public int getTotal() {
        return total;
    }

    public void setTotal(int total) {
        this.total = total;
    }

    public int getSevere() {
        return severe;
    }

    public void setSevere(int severe) {
        this.severe = severe;
    }

    public int getHigh() {
        return high;
    }

    public void setHigh(int high) {
        this.high = high;
    }

    public int getMedium() {
        return medium;
    }

    public void setMedium(int medium) {
        this.medium = medium;
    }

    public int getLow() {
        return low;
    }

    public void setLow(int low) {
        this.low = low;
    }

    @Override
    public String toString() {
        return "CMAuditedAlerts{" +
            "total=" + total +
            ", severe=" + severe +
            ", high=" + high +
            ", medium=" + medium +
            ", low=" + low +
            '}';
    }
}
