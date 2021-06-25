package com.idera.sqlcm.ui.preferences;


import com.idera.sqlcm.ui.components.filter.FilterData;
import org.zkoss.zul.ListModelList;

import java.util.Map;

public class CommonGridPreferencesBean {

    public final static String SESSION_VARIABLE_NAME = "CMCommonGridSessionDataBean";

    private FilterData filters;
    private boolean isAscendingSortDirection;
    private String sortedColumnId = "";
    private int gridRowsCount = 0;
    private ListModelList columnsListModelList;

    public FilterData getFilters() {
        return filters;
    }

    public void setFilters(FilterData filters) {
        this.filters = filters;
    }

    public boolean isAscendingSortDirection() {
        return isAscendingSortDirection;
    }

    public void setAscendingSortDirection(boolean isAscendingSortDirection) {
        this.isAscendingSortDirection = isAscendingSortDirection;
    }

    public String getSortedColumnId() {
        return sortedColumnId;
    }

    public void setSortedColumnId(String sortedColumnId) {
        this.sortedColumnId = sortedColumnId;
    }

    public int getGridRowsCount() {
        return gridRowsCount;
    }

    public void setGridRowsCount(int gridRowsCount) {
        this.gridRowsCount = gridRowsCount;
    }

    public ListModelList getColumnsListModelList() {
        return columnsListModelList;
    }

    public void setColumnsListModelList(ListModelList columnsListModelList) {
        this.columnsListModelList = columnsListModelList;
    }
    
    public ListModelList getAlertRulesColumnsListModelList() {
        return columnsListModelList;
    }

    public void setAlertRulesColumnsListModelList(ListModelList columnsListModelList) {
        this.columnsListModelList = columnsListModelList;
    }
    
    public ListModelList getActivityLogsColumnsListModelList() {
        return columnsListModelList;
    }
    
    /*SQLCM Req4.1.1.6 New Event Filters View - Start*/
    public ListModelList getEventFiltersColumnsListModelList() {
        return columnsListModelList;
    }
    /*SQLCM Req4.1.1.6 New Event Filters View - End*/
    
    public void setActivityLogsColumnsListModelList(ListModelList columnsListModelList) {
        this.columnsListModelList = columnsListModelList;
    }
    
    /*SQLCM Req4.1.1.6 New Event Filters View - Start*/
    public void setEventFiltersColumnsListModelList(ListModelList columnsListModelList) {
        this.columnsListModelList = columnsListModelList;
    }
    /*SQLCM Req4.1.1.6 New Event Filters View - End*/
    
    public ListModelList getChangeLogsColumnsListModelList() {
        return columnsListModelList;
    }
    
    public void setChangeLogsColumnsListModelList(ListModelList columnsListModelList) {
        this.columnsListModelList = columnsListModelList;
    }
    
}
