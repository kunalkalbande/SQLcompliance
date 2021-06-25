package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

public class CMApplyReindexForArchiveRequest {

    @JsonProperty("archive")
    private String archive;

    @JsonProperty("indexStartTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date indexStartTime;

    @JsonProperty("indexDurationHours")
    private int indexDurationHours;

    @JsonProperty("indexDurationMinutes")
    private int indexDurationMinutes;

    public CMApplyReindexForArchiveRequest() {
    }

    public String getArchive() {
        return archive;
    }

    public void setArchive(String archive) {
        this.archive = archive;
    }

    public Date getIndexStartTime() {
        return indexStartTime;
    }

    public void setIndexStartTime(Date indexStartTime) {
        this.indexStartTime = indexStartTime;
    }

    public int getIndexDurationHours() {
        return indexDurationHours;
    }

    public void setIndexDurationHours(int indexDurationHours) {
        this.indexDurationHours = indexDurationHours;
    }

    public int getIndexDurationMinutes() {
        return indexDurationMinutes;
    }

    public void setIndexDurationMinutes(int indexDurationMinutes) {
        this.indexDurationMinutes = indexDurationMinutes;
    }

    @Override
    public String toString() {
        return "CMApplyReindexForArchiveRequest{" +
            "archive='" + archive + '\'' +
            ", indexStartTime=" + indexStartTime +
            ", indexDurationHours=" + indexDurationHours +
            ", indexDurationMinutes=" + indexDurationMinutes +
            '}';
    }
}
