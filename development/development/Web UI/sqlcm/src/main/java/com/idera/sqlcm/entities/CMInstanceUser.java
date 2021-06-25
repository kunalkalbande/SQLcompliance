package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Objects;

public class CMInstanceUser extends CMInstancePermissionBase {

    @JsonProperty("sid")
    private String sid;

    public String getSid() {
        return sid;
    }

    public void setSid(String sid) {
        this.sid = sid;
    }

    @Override public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        if (!super.equals(o)) return false;
        CMInstanceUser that = (CMInstanceUser) o;
        return Objects.equal(sid, that.sid);
    }

    @Override public int hashCode() {
        return Objects.hashCode(super.hashCode(), sid);
    }
}
