package com.idera.sqlcm.ui.eventFilters.filters;

import com.idera.sqlcm.ui.components.filter.model.Filter;

public class EventFiltersViewFilter extends Filter {

	private EventFiltersViewFilters filter;

	public EventFiltersViewFilter(EventFiltersViewFilters filter) {
		this.filter = filter;
        this.filterId = filter.getFilterId();
        this.filterName = filter.getFilterName();
        this.filterType = filter.getFilterType();
        this.columnId = filter.getColumnId().toString();
	}
}
