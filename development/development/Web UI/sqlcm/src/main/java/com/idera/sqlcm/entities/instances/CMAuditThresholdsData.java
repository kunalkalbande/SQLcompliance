package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

public class CMAuditThresholdsData {
    @JsonProperty("thresholdList")
    private List<CMThreshold> thresholdList;

    public List<CMThreshold> getThresholdList() {
        return thresholdList;
    }

    public void setThresholdList(List<CMThreshold> thresholdList) {
        this.thresholdList = thresholdList;
    }
}
