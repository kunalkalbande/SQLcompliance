package com.idera.sqlcm.ui.logsView;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum ActivityLogsColumns {

	DATE("dateColumn", SQLCMI18NStrings.DATE, true, true),
    TIME("timeColumn", SQLCMI18NStrings.TIME, true, true),
    INSTANCE_NAME("instanceNameColumn", SQLCMI18NStrings.INSTANCE_NAME, true, true),
    EVENT("eventColumn", SQLCMI18NStrings.EVENT, true, true),
    DETAIL("detailColumn", SQLCMI18NStrings.DETAIL, true, true);

    private final String columnId;
    private String labelKey;
    private boolean sortable;
    private boolean visible;
    private static String INSTANCES_ALERTS_SESSION_DATA_BEAN = "InstancesAlertsSessionDataBean";

    private static Map<String, ActivityLogsColumns> lookup = new HashMap<String, ActivityLogsColumns>();
    static {
        for (ActivityLogsColumns alertsColumns : ActivityLogsColumns.values()) {
            lookup.put(alertsColumns.columnId, alertsColumns);
        }
    }

    private ActivityLogsColumns(String columnId, String labelKey, boolean sortable, boolean visible) {
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

    public static ActivityLogsColumns findColumnById(String columnId) {
        return lookup.get(columnId);
    }

    public static String getSource(){
        return INSTANCES_ALERTS_SESSION_DATA_BEAN;
    }

    @Override
    public String toString() {
        return columnId;
    }

}
