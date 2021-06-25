package com.idera.sqlcm.ui.changeLogsView.filters;

import com.idera.sqlcm.ui.components.filter.model.Filter;

public class ChangeLogsViewFilter extends Filter {

	private ChangeLogsViewFilters filter;

	public ChangeLogsViewFilter(ChangeLogsViewFilters filter) {
		this.filter = filter;
        this.filterId = filter.getFilterId();
        this.filterName = filter.getFilterName();
        this.filterType = filter.getFilterType();
        this.columnId = filter.getColumnId().toString();
	}
}
