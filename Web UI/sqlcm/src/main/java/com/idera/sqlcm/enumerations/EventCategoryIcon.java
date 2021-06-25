package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum EventCategoryIcon {
    LOGIN_EVENT(1, "event-login-16x16"),
    DDL_EVENT(2, "event-ddl-16x16"),
    SECURITY_EVENT(3, "event-security-16x16"),
    DML_EVENT(4, "event-dml-16x16"),
    SELECT_EVENT(5, "event-select-16x16"),
    ADMIN_EVENT(6, "event-admin-16x16"),
    USER_DEFINED_EVENT(9, "event-user-defined-16x16"),
    DEFAULT_EVENT(0, "event-default-16x16");

    private long id;

    private String iconURL;

    EventCategoryIcon(int id, String iconURL) {
        this.id = id;
        this.iconURL = iconURL;
    }

    public long getId() {
        return id;
    }

    public String getIconURL() {
        return iconURL;
    }

    public static EventCategoryIcon getByIndex(long index) {
        for (EventCategoryIcon value : EventCategoryIcon.values()) {
            if (value.getId() == index) {
                return value;
            }
        }
        return DEFAULT_EVENT;
    }
}
