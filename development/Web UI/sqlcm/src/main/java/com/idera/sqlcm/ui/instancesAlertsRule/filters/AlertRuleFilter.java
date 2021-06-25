package com.idera.sqlcm.ui.instancesAlertsRule.filters;

import com.idera.sqlcm.ui.components.filter.model.Filter;

public class AlertRuleFilter extends Filter {

	private AlertsRuleFilters filter;

	public AlertRuleFilter(AlertsRuleFilters filter) {
		this.filter = filter;
        this.filterId = filter.getFilterId();
        this.filterName = filter.getFilterName();
        this.filterType = filter.getFilterType();
        this.columnId = filter.getColumnId().toString();
	}
}
