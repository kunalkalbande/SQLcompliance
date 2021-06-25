package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMUpdatedArchiveProperties extends CMArchivedDatabase {

    @JsonProperty("newDefaultAccess")
    private long newDefaultAccess;

    @JsonProperty("oldDefaultAccess")
    private long oldDefaultAccess;

    public long getNewDefaultAccess() {
        return newDefaultAccess;
    }

    public void setNewDefaultAccess(long newDefaultAccess) {
        this.newDefaultAccess = newDefaultAccess;
    }

    public long getOldDefaultAccess() {
        return oldDefaultAccess;
    }

    public void setOldDefaultAccess(long oldDefaultAccess) {
        this.oldDefaultAccess = oldDefaultAccess;
    }

    @Override
    public String toString() {
        return "CMUpdatedArchiveProperties{" +
            "newDefaultAccess=" + newDefaultAccess +
            ", oldDefaultAccess=" + oldDefaultAccess +
            '}';
    }
}