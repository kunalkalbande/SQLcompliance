package com.idera.sqlcm.ui.changeLogsView.filters;


import com.idera.sqlcm.ui.components.filter.model.FilterChild;

public class ChangeLogsViewFilterChild extends FilterChild {

	private ChangeLogsViewOptionValues filter;
	
	public ChangeLogsViewFilterChild(ChangeLogsViewOptionValues filter) {
		this.filter = filter;
        this.id = filter.getId();
        this.intValue = filter.getIntValue();
        this.value = filter.getValue();
        this.label = filter.getLabel();
	}
}
