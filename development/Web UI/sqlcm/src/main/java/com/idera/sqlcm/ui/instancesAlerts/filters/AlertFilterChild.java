package com.idera.sqlcm.ui.instancesAlerts.filters;


import com.idera.sqlcm.ui.components.filter.model.FilterChild;

public class AlertFilterChild extends FilterChild {

	private AlertsOptionValues filter;
	
	public AlertFilterChild(AlertsOptionValues filter) {
		this.filter = filter;
        this.id = filter.getId();
        this.intValue = filter.getIntValue();
        this.value = filter.getValue();
        this.label = filter.getLabel();
	}
}
