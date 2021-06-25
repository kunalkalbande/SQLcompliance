package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMBeforeAfterDataEntity {

    @JsonProperty("key")
    private long key;

    @JsonProperty("value")
    private String value;

    public CMBeforeAfterDataEntity() {
    }

    public CMBeforeAfterDataEntity(long key, String value) {
        this.key = key;
        this.value = value;
    }

    public long getKey() {
        return key;
    }

    public void setKey(long key) {
        this.key = key;
    }

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        this.value = value;
    }

    @Override
    public String toString() {
        return "CMBeforeAfterDataEntity{" +
            "key=" + key +
            ", value='" + value + '\'' +
            '}';
    }
}
