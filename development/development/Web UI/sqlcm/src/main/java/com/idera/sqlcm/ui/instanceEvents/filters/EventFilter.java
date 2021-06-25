package com.idera.sqlcm.ui.instanceEvents.filters;

import com.idera.sqlcm.ui.components.filter.model.Filter;

public class EventFilter extends Filter {

	private EventsFilters filter;

	public EventFilter(EventsFilters filter) {
		this.filter = filter;
        this.filterId = filter.getFilterId();
        this.filterName = filter.getFilterName();
        this.filterType = filter.getFilterType();
        this.columnId = filter.getColumnId().toString();
	}
}
