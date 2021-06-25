package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.server.web.ELFunctions;

public enum PermissionType {

    SERVER_ROLES(0, ELFunctions.getLabel("Labels.sql-cm.logins-roles-location.server-roles")),
    SERVER_LOGINS(1, ELFunctions.getLabel("Labels.sql-cm.logins-roles-location.server-logins"));

    private int index;
    private String label;

    PermissionType(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static PermissionType getByIndex(int index) {
        return PermissionType.values()[index];
    }
}
