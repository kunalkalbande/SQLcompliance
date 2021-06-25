package com.idera.sqlcm.ui.activityLogsView.filters;


import com.idera.sqlcm.ui.components.filter.model.FilterChild;

public class ActivityLogsViewFilterChild extends FilterChild {

	private ActivityLogsViewOptionValues filter;
	
	public ActivityLogsViewFilterChild(ActivityLogsViewOptionValues filter) {
		this.filter = filter;
        this.id = filter.getId();
        this.intValue = filter.getIntValue();
        this.value = filter.getValue();
        this.label = filter.getLabel();
	}
}
