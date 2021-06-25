package com.idera.sqlcm.ui.databaseEvents.filters;

import com.idera.sqlcm.ui.components.filter.model.FilterChild;
import com.idera.sqlcm.ui.instances.filters.InstancesOptionFilterValues;

public class EventFilterChild extends FilterChild {

    private EventsOptionFilterValues filter;

    public EventFilterChild(EventsOptionFilterValues filter) {
        this.filter = filter;
        this.id = filter.getId();
        this.intValue = filter.getIntValue();
        this.value = filter.getValue();
        this.label = filter.getLabel();
    }
}
