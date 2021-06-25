package com.idera.sqlcm.ui.instances.filters;

import com.idera.sqlcm.ui.components.filter.model.Filter;

public class InstanceFilter extends Filter {

	private InstancesFilters filter;
	
	public InstanceFilter(InstancesFilters filter) {
		this.filter = filter;
        this.filterId = filter.getFilterId();
        this.filterName = filter.getFilterName();
        this.filterType = filter.getFilterType();
        this.columnId = filter.getColumnId().toString();
	}
}
