package com.idera.sqlcm.ui.eventFilters.filters;


import com.idera.sqlcm.ui.components.filter.model.FilterChild;

public class EventFiltersViewFilterChild extends FilterChild {

	private EventFiltersViewOptionValues filter;
	
	public EventFiltersViewFilterChild(EventFiltersViewOptionValues filter) {
		this.filter = filter;
        this.intValue = filter.getIntValue();
        this.label = filter.getLabel();
	}
}
