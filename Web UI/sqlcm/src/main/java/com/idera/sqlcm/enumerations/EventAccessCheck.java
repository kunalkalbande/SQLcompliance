package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum EventAccessCheck {
    FAILED(0, ELFunctions.getLabel(SQLCMI18NStrings.EVENT_ACCESS_CHECK_FAILED)),
    PASSED(1, ELFunctions.getLabel(SQLCMI18NStrings.EVENT_ACCESS_CHECK_PASSED));

    private String label;

    private int id;

    private EventAccessCheck(int id, String label) {
        this.id = id;
        this.label = label;
    }

    public String getLabel() {
        return label;
    }

    public int getId() {
        return id;
    }

    public static EventAccessCheck getByIndex(int id) {
        for (EventAccessCheck e : values()) {
            if (e.id == id) {
                return e;
            }
        }
        return null;
    }
}
