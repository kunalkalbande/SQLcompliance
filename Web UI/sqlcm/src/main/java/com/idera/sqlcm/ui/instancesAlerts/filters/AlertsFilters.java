package com.idera.sqlcm.ui.instancesAlerts.filters;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.components.filter.model.FilterType;
import com.idera.sqlcm.ui.instancesAlerts.AlertsColumns;

public enum AlertsFilters {
    INSTANCE_NAME("InstanceName", FilterType.TEXT, SQLCMI18NStrings.INSTANCE_NAME, AlertsColumns.INSTANCE_NAME),
    DATE("Date", FilterType.DATE_RANGE, SQLCMI18NStrings.DATE, AlertsColumns.DATE),
    TIME("Time", FilterType.TIME_RANGE, SQLCMI18NStrings.TIME, AlertsColumns.TIME),
    LEVEL("Levels", FilterType.OPTIONS, SQLCMI18NStrings.LEVEL, AlertsColumns.LEVEL),
    SOURCE_RULE("SourceRule", FilterType.TEXT, SQLCMI18NStrings.SOURCE_RULE, AlertsColumns.SOURCE_RULE),
    EVENT("Event", FilterType.COMBO, SQLCMI18NStrings.EVENT, AlertsColumns.EVENT),
    DETAILS("Detail", FilterType.TEXT, SQLCMI18NStrings.DETAIL, AlertsColumns.DETAIL);

    private String filterId;
    private FilterType filterType;
    private String filterName;
    private AlertsColumns columnId;

    AlertsFilters(String filterId, FilterType filterType, String filterName, AlertsColumns columnId) {
        this.filterId = filterId;
        this.filterType = filterType;
        this.filterName = filterName;
        this.columnId = columnId;
    }

    public String getFilterId() {
        return filterId;
    }

    public FilterType getFilterType() {
        return filterType;
    }

    public String getFilterName() {
        return filterName;
    }

    public AlertsColumns getColumnId() {
        return columnId;
    }

}