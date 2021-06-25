package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.server.web.ELFunctions;

public enum DefaultDBPermission {
    GRANT_READ_EVENTS_WITH_ADDS(2, ELFunctions.getLabel("Labels.sql-cm.inst-prop-d-advanced-tab-grant-read-events-with-adds")),
    GRANT_READ_EVENTS_ONLY(1, ELFunctions.getLabel("Labels.sql-cm.inst-prop-d-advanced-tab-grant-read-events-only")),
    GRANT_DENY_READ(0, ELFunctions.getLabel("Labels.sql-cm.inst-prop-d-advanced-tab-grant-deny-read"));

    private int index;
    private String label;

    DefaultDBPermission(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static DefaultDBPermission getByIndex(int index) {
        DefaultDBPermission result = null;
        for (int i= 0; i < DefaultDBPermission.values().length; i++) {
            if (DefaultDBPermission.values()[i].getIndex() == index) {
                result = DefaultDBPermission.values()[i];
            }
        }
        return result;
    }
}
