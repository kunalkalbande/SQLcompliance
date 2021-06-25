package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.idera.sqlcm.ui.components.filter.FilterData;

import java.util.Map;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMSideBarViewSettings extends CMEntity {

    FilterData filterData;

    Map<String, Boolean> columnVisibility;

    public FilterData getFilterData() {
        return filterData;
    }

    public void setFilterData(FilterData filterData) {
        this.filterData = filterData;
    }

    public Map<String, Boolean> getColumnVisibility() {
        return columnVisibility;
    }

    public void setColumnVisibility(Map<String, Boolean> columnVisibility) {
        this.columnVisibility = columnVisibility;
    }

    @Override
    public String toString() {
        return "CMViewEntitiesSettings{" +
            "filterData=" + filterData +
            ", columnVisibility=" + columnVisibility +
            '}';
    }
}
