package com.idera.sqlcm.ui.eventFilters;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum EventFiltersColumns {

	FILTER("filterColumn", SQLCMI18NStrings.FILTER, true, true),
    STATUS("statusColumn", SQLCMI18NStrings.STATUS, true, true),
    INSTANCE("instanceColumn", SQLCMI18NStrings.INSTANCE, true, true),
    DESCRIPTION("descriptionColumn", SQLCMI18NStrings.DESCRIPTION, true, true);

    private final String columnId;
    private String labelKey;
    private boolean sortable;
    private boolean visible;
    private static String AUDIT_EVENT_FILTERS_SESSION_DATA_BEAN = "AuditEventFiltersSessionDataBean";

    private static Map<String, EventFiltersColumns> lookup = new HashMap<String, EventFiltersColumns>();
    static {
        for (EventFiltersColumns alertsColumns : EventFiltersColumns.values()) {
            lookup.put(alertsColumns.columnId, alertsColumns);
        }
    }

    private EventFiltersColumns(String columnId, String labelKey, boolean sortable, boolean visible) {
        this.columnId = columnId;
        this.labelKey = labelKey;
        this.sortable = sortable;
        this.visible = visible;
    }


    public String getColumnId() {
        return columnId;
    }

    public String getLabelKey() {
        return labelKey;
    }

    public boolean isSortable() {
        return sortable;
    }

    public String getLabel() {
        return ELFunctions.getLabel(getLabelKey());
    }

    public boolean isVisible() {
        return visible;
    }

    public void setVisible(boolean visible) {
        this.visible = visible;
    }

    public static EventFiltersColumns findColumnById(String columnId) {
        return lookup.get(columnId);
    }

    public static String getSource(){
        return AUDIT_EVENT_FILTERS_SESSION_DATA_BEAN;
    }

    @Override
    public String toString() {
        return columnId;
    }

}
