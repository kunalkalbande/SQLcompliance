package com.idera.sqlcm.ui.activityLogsView.filters;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.components.filter.model.FilterType;
import com.idera.sqlcm.ui.logsView.ActivityLogsColumns;

public enum ActivityLogsViewFilters {
	        INSTANCE_NAME("InstanceName", FilterType.TEXT,SQLCMI18NStrings.INSTANCE_NAME, com.idera.sqlcm.ui.logsView.ActivityLogsColumns.INSTANCE_NAME),
			DATE("Date",FilterType.DATE_RANGE,SQLCMI18NStrings.DATE,ActivityLogsColumns.DATE),
			TIME("Time",FilterType.TIME_RANGE,SQLCMI18NStrings.TIME, ActivityLogsColumns.TIME),
			EVENT("Event",FilterType.COMBO,SQLCMI18NStrings.EVENT, ActivityLogsColumns.EVENT),
			DETAILS("Detail",FilterType.TEXT, SQLCMI18NStrings.DETAILS, ActivityLogsColumns.DETAIL);

	private String filterId;
	private FilterType filterType;
	private String filterName;
	private ActivityLogsColumns columnId;

	ActivityLogsViewFilters(String filterId, FilterType filterType,
			String filterName, ActivityLogsColumns columnId) {
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

	public ActivityLogsColumns getColumnId() {
		return columnId;
	}

}