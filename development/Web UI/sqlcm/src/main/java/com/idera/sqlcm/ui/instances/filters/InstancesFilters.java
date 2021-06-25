package com.idera.sqlcm.ui.instances.filters;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.components.filter.model.FilterType;
import com.idera.sqlcm.ui.instances.InstancesColumns;

public enum InstancesFilters {
    STATUS("status", FilterType.OPTIONS, SQLCMI18NStrings.STATUS_COLUMN, InstancesColumns.STATUS),
    INSTANCE_NAME("instance", FilterType.TEXT, SQLCMI18NStrings.INSTANCE_NAME_COLUMN, InstancesColumns.INSTANCE_NAME),
    STATUS_TEXT("statusText", FilterType.TEXT, SQLCMI18NStrings.STATUS_TEXT_COLUMN, InstancesColumns.STATUS_TEXT),
    NUMBER_OF_AUDITED_DB("auditedDbCount", FilterType.DIGIT_RANGE, SQLCMI18NStrings.NUMBER_OF_AUDITED_DB_COLUMN,
            InstancesColumns.NUMBER_OF_AUDITED_DB),
    SQL_SERVER_VERSION_EDITION("sqlVersion", FilterType.TEXT,
            SQLCMI18NStrings.SQL_SERVER_VERSION_COLUMN, InstancesColumns.SQL_SERVER_VERSION_EDITION),
    AUDIT_STATUS("isEnabled", FilterType.OPTIONS, SQLCMI18NStrings.AUDIT_STATUS_COLUMN, InstancesColumns.AUDIT_STATUS),
    LAST_AGENT_CONTACT("lastAgentContact", FilterType.DATE_RANGE, SQLCMI18NStrings.LAST_AGENT_CONTACT_COLUMN,
            InstancesColumns.LAST_AGENT_CONTACT);

    private String filterId;
    private FilterType filterType;
    private String filterName;
    private InstancesColumns columnId;

    InstancesFilters(String filterId, FilterType filterType, String filterName, InstancesColumns columnId) {
        this.filterId = filterId;
        this.filterType = filterType;
        this.filterName = filterName;
        this.columnId = columnId;
    }

    public String getFilterId() {
        return filterId;
    }

    public FilterType getFilterType() {
        return filterType;
    }

    public String getFilterName() {
        return filterName;
    }

    public InstancesColumns getColumnId() {
        return columnId;
    }

}