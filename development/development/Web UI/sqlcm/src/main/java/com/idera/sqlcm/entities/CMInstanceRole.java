package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Objects;

public class CMInstanceRole extends CMInstancePermissionBase {

    @JsonProperty("id")
    private long id;

    @JsonProperty("fullName")
    private String fullName;

    public long getId() {
        return id;
    }

    public void setId(long id) {
        this.id = id;
    }

    public String getFullName() {
        return fullName;
    }

    public String getDisplayName() {
        return getFullName();
    }

    public void setFullName(String fullName) {
        this.fullName = fullName;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        if (!super.equals(o)) return false;
        CMInstanceRole that = (CMInstanceRole) o;
        return Objects.equal(id, that.id) &&
                Objects.equal(fullName, that.fullName);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(super.hashCode(), id, fullName);
    }
}