package com.idera.sqlcm.ui.instanceEvents.filters;

import com.idera.sqlcm.ui.components.filter.model.FilterChild;

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
