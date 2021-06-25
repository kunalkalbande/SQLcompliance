package com.idera.sqlcm.ui.eventFilters.filters;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.components.filter.model.FilterType;
import com.idera.sqlcm.ui.eventFilters.EventFiltersColumns;

public enum EventFiltersViewFilters {
    
    /*SQLCM Req4.1.1.6 New Event Filters View - Start*/
    FILTER("Filter", FilterType.TEXT, SQLCMI18NStrings.FILTER, EventFiltersColumns.FILTER),
    INSTANCE("Instance", FilterType.INSTANCE, SQLCMI18NStrings.INSTANCE, EventFiltersColumns.INSTANCE),
    STATUS("isEnabled", FilterType.OPTIONS, SQLCMI18NStrings.STATUS, EventFiltersColumns.STATUS),
    DESCRIPTION("Description", FilterType.TEXT, SQLCMI18NStrings.DESCRIPTION, EventFiltersColumns.DESCRIPTION);
    /*SQLCM Req4.1.1.6 New Event Filters View - END*/
    
    /*DETAILS("detail", FilterType.TEXT, SQLCMI18NStrings.DETAILS, EventFiltersColumns.DETAIL);*/
	
    private String filterId;
    private FilterType filterType;
    private String filterName;
    private EventFiltersColumns columnId;

    EventFiltersViewFilters(String filterId, FilterType filterType, String filterName, EventFiltersColumns columnId) {
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

    public EventFiltersColumns getColumnId() {
        return columnId;
    }

}