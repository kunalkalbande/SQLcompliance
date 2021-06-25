package com.idera.sqlcm.ui.instancesAlerts.filters;

import com.idera.sqlcm.ui.components.filter.model.Filter;

public class AlertFilter extends Filter {

	private AlertsFilters filter;

	public AlertFilter(AlertsFilters filter) {
		this.filter = filter;
        this.filterId = filter.getFilterId();
        this.filterName = filter.getFilterName();
        this.filterType = filter.getFilterType();
        this.columnId = filter.getColumnId().toString();
	}
}
