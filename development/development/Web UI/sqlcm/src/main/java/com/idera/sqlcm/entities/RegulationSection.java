package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.ArrayList;
import java.util.List;

public class RegulationSection {

    private String name;

    private List<RegulationSectionValue> regulationValueList = new ArrayList<RegulationSectionValue>();

    @JsonProperty("Key")
    public String getName() {
        return name;
    }

    @JsonProperty("Key")
    public void setName(String name) {
        this.name = name;
    }

    @JsonProperty("Value")
    public List<RegulationSectionValue> getValueList() {
        return regulationValueList;
    }

    @JsonProperty("Value")
    public void setValueList(List<RegulationSectionValue> Value) {
        this.regulationValueList = Value;
    }

}