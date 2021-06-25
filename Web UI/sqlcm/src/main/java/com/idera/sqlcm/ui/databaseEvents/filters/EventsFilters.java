package com.idera.sqlcm.ui.databaseEvents.filters;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.components.filter.model.FilterType;
import com.idera.sqlcm.ui.databaseEvents.EventsColumns;

public enum EventsFilters {

    ACCESS_CHECK("AccessCheck", FilterType.OPTIONS, SQLCMI18NStrings.ACCESS_CHECK, EventsColumns.ACCESS_CHECK),
    AFTER_VALUE("AfterValue", FilterType.TEXT, SQLCMI18NStrings.AFTER_VALUE, EventsColumns.AFTER_VALUE),
    APPLICATION("Application", FilterType.TEXT, SQLCMI18NStrings.APPLICATION, EventsColumns.APPLICATION),
    AUDITED_UPDATES("AuditedUpdates", FilterType.DIGIT_RANGE, SQLCMI18NStrings.AUDITED_UPDATES, EventsColumns.AUDITED_UPDATES),
    BEFORE_VALUE("BeforeValue", FilterType.TEXT, SQLCMI18NStrings.BEFORE_VALUE, EventsColumns.BEFORE_VALUE),
    CATEGORY("Category", FilterType.TEXT, SQLCMI18NStrings.CATEGORY, EventsColumns.CATEGORY),
    COLUMN("Column", FilterType.TEXT, SQLCMI18NStrings.COLUMN, EventsColumns.COLUMN),
    COLUMNS_UPDATED("ColumnsUpdated", FilterType.DIGIT_RANGE, SQLCMI18NStrings.COLUMNS_UPDATED, EventsColumns.COLUMNS_UPDATED),
    DATABASE("DatabaseName", FilterType.TEXT, SQLCMI18NStrings.DATABASE, EventsColumns.DATABASE),
    DATABASE_USER("DatabaseUser", FilterType.TEXT, SQLCMI18NStrings.DATABASE_USER, EventsColumns.DATABASE_USER),
    DATE("Date", FilterType.DATE_RANGE, SQLCMI18NStrings.DATE, EventsColumns.DATE),
    DETAILS("Details", FilterType.TEXT, SQLCMI18NStrings.DETAILS, EventsColumns.DETAILS),
    EVENT("EventType", FilterType.COMBO, SQLCMI18NStrings.EVENT, EventsColumns.EVENT),
    HOST("Host", FilterType.TEXT, SQLCMI18NStrings.HOST, EventsColumns.HOST),
    LOGIN("LoginName", FilterType.TEXT, SQLCMI18NStrings.LOGIN, EventsColumns.LOGIN),
    OBJECT("Object", FilterType.TEXT, SQLCMI18NStrings.OBJECT, EventsColumns.OBJECT),
    OWNER("Owner", FilterType.TEXT, SQLCMI18NStrings.OWNER, EventsColumns.OWNER),
    PRIMARY_KEY("PrimaryKey", FilterType.TEXT, SQLCMI18NStrings.PRIMARY_KEY, EventsColumns.PRIMARY_KEY),
    PRIVILEGED_USER("PrivilegedUser", FilterType.OPTIONS, SQLCMI18NStrings.PRIVILEGED_USER, EventsColumns.PRIVILEGED_USER),
    ROLE("Role", FilterType.TEXT, SQLCMI18NStrings.ROLE, EventsColumns.ROLE),
    SCHEMA("Schema", FilterType.TEXT, SQLCMI18NStrings.SCHEMA, EventsColumns.SCHEMA),
    SERVER("Server", FilterType.TEXT, SQLCMI18NStrings.SERVER, EventsColumns.SERVER),
    SESSION_LOGIN("SessionLogin", FilterType.TEXT, SQLCMI18NStrings.SESSION_LOGIN, EventsColumns.SESSION_LOGIN),
    SPID("Spid", FilterType.DIGIT_RANGE, SQLCMI18NStrings.SPID, EventsColumns.SPID),
    TABLE("Table", FilterType.TEXT, SQLCMI18NStrings.TABLE, EventsColumns.TABLE),
    TARGET_LOGIN("TargetLogin", FilterType.TEXT, SQLCMI18NStrings.TARGET_LOGIN, EventsColumns.TARGET_LOGIN),
    TARGET_OBJECT("TargetObject", FilterType.TEXT, SQLCMI18NStrings.TARGET_OBJECT, EventsColumns.TARGET_OBJECT),
    TARGET_USER("TargetUser", FilterType.TEXT, SQLCMI18NStrings.TARGET_USER, EventsColumns.TARGET_USER),
    TIME("Time", FilterType.TIME_RANGE, SQLCMI18NStrings.TIME, EventsColumns.TIME),;

    private String filterId;

    private FilterType filterType;

    private String filterName;

    private EventsColumns columnId;

    EventsFilters(String filterId, FilterType filterType, String filterName, EventsColumns columnId) {
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

    public EventsColumns getColumnId() {
        return columnId;
    }

}