package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.server.web.ELFunctions;

public enum ServerStatus {
    OK(0, ELFunctions.getLabel("Labels.sql-cm.server-status-ok")),
    DISABLED(1, ELFunctions.getLabel("Labels.sql-cm.server-status-disabled")),
    DOWN(2, ELFunctions.getLabel("Labels.sql-cm.server-status-down")),
    ERROR(3, ELFunctions.getLabel("Labels.sql-cm.server-status-error")),
    SLOW(4, ELFunctions.getLabel("Labels.sql-cm.server-status-slow")),
    UP(5, ELFunctions.getLabel("Labels.sql-cm.server-status-up")),
    UNKNOWN(6, ELFunctions.getLabel("Labels.sql-cm.server-status-unknown"));

    private int index;
    private String label;

    ServerStatus(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static ServerStatus getByIndex(int index) {
        return ServerStatus.values()[index];
    }
}
