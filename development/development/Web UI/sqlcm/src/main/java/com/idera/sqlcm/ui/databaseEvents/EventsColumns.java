package com.idera.sqlcm.ui.databaseEvents;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum EventsColumns {

    ACCESS_CHECK("accessCheckColumn", SQLCMI18NStrings.ACCESS_CHECK, true, false, true),
    AFTER_VALUE("afterValueColumn", SQLCMI18NStrings.AFTER_VALUE, true, false, true),
    APPLICATION("applicationColumn", SQLCMI18NStrings.APPLICATION, true, false, true),
    AUDITED_UPDATES("auditedUpdatesColumn", SQLCMI18NStrings.AUDITED_UPDATES, true, false, true),
    BEFORE_VALUE("beforeValueColumn", SQLCMI18NStrings.BEFORE_VALUE, true, false, true),
    CATEGORY("categoryColumn", SQLCMI18NStrings.CATEGORY, true, true, true),
    COLUMN("columnColumn", SQLCMI18NStrings.COLUMN, true, false, true),
    COLUMNS_UPDATED("columnsUpdatedColumn", SQLCMI18NStrings.COLUMNS_UPDATED, true, false, true),
    DATABASE("databaseColumn", SQLCMI18NStrings.DATABASE, true, true, true),
    DATABASE_USER("databaseUserColumn", SQLCMI18NStrings.DATABASE_USER, true, false, true),
    DATE("dateColumn", SQLCMI18NStrings.DATE, true, true, true),
    DETAILS("detailsColumn", SQLCMI18NStrings.DETAILS, true, true, true),
    EVENT("eventColumn", SQLCMI18NStrings.EVENT, true, true, true),
    HOST("hostColumn", SQLCMI18NStrings.HOST, true, false, true),
    ICON("iconColumn", SQLCMI18NStrings.ICON, true, true, true),
    LOGIN("loginColumn", SQLCMI18NStrings.LOGIN, true, true, true),
    OBJECT("objectColumn", SQLCMI18NStrings.OBJECT, true, false, true),
    OWNER("ownerColumn", SQLCMI18NStrings.OWNER, true, false, true),
    PRIMARY_KEY("primaryKeyColumn", SQLCMI18NStrings.PRIMARY_KEY, true, false, true),
    PRIVILEGED_USER("privilegedUserColumn", SQLCMI18NStrings.PRIVILEGED_USER, true, false, true),
    ROLE("roleColumn", SQLCMI18NStrings.ROLE, true, false, true),
    SCHEMA("schemaColumn", SQLCMI18NStrings.SCHEMA, true, false, true),
    SERVER("serverColumn", SQLCMI18NStrings.SERVER, true, false, true),
    SESSION_LOGIN("sessionLoginColumn", SQLCMI18NStrings.SESSION_LOGIN, true, false, true),
    SPID("spidColumn", SQLCMI18NStrings.SPID, true, false, true),
    TABLE("tableColumn", SQLCMI18NStrings.TABLE, true, false, true),
    TARGET_LOGIN("targetLoginColumn", SQLCMI18NStrings.TARGET_LOGIN, true, false, true),
    TARGET_OBJECT("targetObjectColumn", SQLCMI18NStrings.TARGET_OBJECT, true, true, true),
    TARGET_USER("targetUserColumn", SQLCMI18NStrings.TARGET_USER, true, false, true),
    TIME("timeColumn", SQLCMI18NStrings.TIME, true, true, true),
    OPTIONS("optionsColumn", SQLCMI18NStrings.EVENTS_OPTIONS_COLUMN, false, true, false);

    private final String columnId;

    private String labelKey;

    private boolean sortable;

    private boolean visible;

    private boolean allowedToDisable;

    private static String DATABASE_EVENTS_SESSION_DATA_BEAN = "DatabaseEventsGridViewModel";

    private EventsColumns(String columnId, String labelKey, boolean sortable, boolean visible, boolean allowedToDisable) {
        this.columnId = columnId;
        this.labelKey = labelKey;
        this.sortable = sortable;
        this.visible = visible;
        this.allowedToDisable = allowedToDisable;
    }

    private static Map<String, EventsColumns> lookup = new HashMap<String, EventsColumns>();

    static {
        for (EventsColumns alertsColumns : EventsColumns.values()) {
            lookup.put(alertsColumns.columnId, alertsColumns);
        }
    }

    public boolean isVisible() {
        return visible;
    }

    public void setVisible(boolean visible) {
        this.visible = visible;
    }

    public static EventsColumns findColumnById(String columnId) {
        return lookup.get(columnId);
    }

    public static String getSource() {
        return DATABASE_EVENTS_SESSION_DATA_BEAN;
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

    public boolean isAllowedToDisable() {
        return allowedToDisable;
    }

    @Override
    public String toString() {
        return columnId;
    }

}
