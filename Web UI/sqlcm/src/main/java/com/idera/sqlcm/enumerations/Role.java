package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.server.web.ELFunctions;

public enum Role {
    ADMINISTRATOR(3, ELFunctions.getLabel("Labels.sql-cm.role-administrator")),
    READ_ONLY(5, ELFunctions.getLabel("Labels.sql-cm.role-read-only")),
    USER(4, ELFunctions.getLabel("Labels.sql-cm.role-user"));

    private int index;
    private String label;

    Role(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static Role getByIndex(int index) {
        for(Role e : values()) {
            if(e.index == index) {
                return e;
            }
        }
        return null;
    }
}
