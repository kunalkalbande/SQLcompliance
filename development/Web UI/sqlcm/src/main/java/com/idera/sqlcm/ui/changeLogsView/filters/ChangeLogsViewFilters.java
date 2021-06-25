package com.idera.sqlcm.ui.changeLogsView.filters;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.components.filter.model.FilterType;
import com.idera.sqlcm.ui.logsView.ChangeLogsColumns;

public enum ChangeLogsViewFilters {
	
    DATE("Date", FilterType.DATE_RANGE, SQLCMI18NStrings.DATE, ChangeLogsColumns.DATE),
    TIME("Time", FilterType.TIME_RANGE, SQLCMI18NStrings.TIME, ChangeLogsColumns.TIME),
    INSTANCE_NAME("InstanceName", FilterType.TEXT, SQLCMI18NStrings.INSTANCE_NAME, ChangeLogsColumns.LOG_SQL_SERVER),
    EVENT("Event", FilterType.COMBO, SQLCMI18NStrings.EVENT, ChangeLogsColumns.LOG_TYPE),
    USER("User", FilterType.TEXT, SQLCMI18NStrings.USER, ChangeLogsColumns.LOG_USER),
    DESCRIPTION("Detail", FilterType.TEXT, SQLCMI18NStrings.DESCRIPTION, ChangeLogsColumns.LOG_INFO);
    

    private String filterId;
    private FilterType filterType;
    private String filterName;
    private ChangeLogsColumns columnId;

    ChangeLogsViewFilters(String filterId, FilterType filterType, String filterName, ChangeLogsColumns columnId) {
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

    public ChangeLogsColumns getColumnId() {
        return columnId;
    }

}