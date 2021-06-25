package com.idera.sqlcm.ui.instancesAlertsRule.filters;

import com.idera.sqlcm.ui.components.filter.model.FilterChild;

public class AlertRuleTypeFilterChild extends FilterChild{
	
private AlertsRuleTypeOptionValues filter;
	
	public AlertRuleTypeFilterChild(AlertsRuleTypeOptionValues filter) {
		this.filter = filter;
        this.id = filter.getId();
        this.intValue = filter.getIntValue();
        this.value = filter.getValue();
        this.label = filter.getLabel();
	}
}

