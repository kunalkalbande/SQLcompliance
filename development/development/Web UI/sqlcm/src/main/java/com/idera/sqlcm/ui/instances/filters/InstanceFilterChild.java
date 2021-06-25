package com.idera.sqlcm.ui.instances.filters;


import com.idera.sqlcm.ui.components.filter.model.FilterChild;

public class InstanceFilterChild extends FilterChild {

	private InstancesOptionFilterValues filter;
	
	public InstanceFilterChild(InstancesOptionFilterValues filter) {
		this.filter = filter;
        this.id = filter.getId();
        this.intValue = filter.getIntValue();
        this.value = filter.getValue();
        this.label = filter.getLabel();
	}
}
