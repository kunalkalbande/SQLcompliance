package com.idera.sqlcm.ui.logsView;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum ChangeLogsColumns {

	DATE("dateColumn", SQLCMI18NStrings.DATE, true, true),
    TIME("timeColumn", SQLCMI18NStrings.TIME, true, true),
    LOG_SQL_SERVER("logSqlServer", SQLCMI18NStrings.INSTANCE_NAME, true, true),
    LOG_TYPE("logType", SQLCMI18NStrings.EVENT, true, true),
	LOG_USER("logUser", SQLCMI18NStrings.USER, true, true),
    LOG_INFO("logInfo", SQLCMI18NStrings.DESCRIPTION, true, true);
	

    private final String columnId;
    private String labelKey;
    private boolean sortable;
    private boolean visible;
    private static String INSTANCES_ALERTS_SESSION_DATA_BEAN = "InstancesAlertsSessionDataBean";

    private static Map<String, ChangeLogsColumns> lookup = new HashMap<String, ChangeLogsColumns>();
    static {
        for (ChangeLogsColumns alertsColumns : ChangeLogsColumns.values()) {
            lookup.put(alertsColumns.columnId, alertsColumns);
        }
    }

    private ChangeLogsColumns(String columnId, String labelKey, boolean sortable, boolean visible) {
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

    public static ChangeLogsColumns findColumnById(String columnId) {
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
