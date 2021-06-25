package com.idera.sqlcm.ui.activityLogsView.filters;

import com.idera.sqlcm.ui.components.filter.model.Filter;

public class ActivityLogsViewFilter extends Filter {

	private ActivityLogsViewFilters filter;

	public ActivityLogsViewFilter(ActivityLogsViewFilters filter) {
		this.filter = filter;
        this.filterId = filter.getFilterId();
        this.filterName = filter.getFilterName();
        this.filterType = filter.getFilterType();
        this.columnId = filter.getColumnId().toString();
	}
}
