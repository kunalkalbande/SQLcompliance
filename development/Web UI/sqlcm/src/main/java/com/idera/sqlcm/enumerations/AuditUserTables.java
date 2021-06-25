package com.idera.sqlcm.enumerations;

public enum AuditUserTables {
    NONE(0, 2),
    ALL(1, 0),
    FOLLOWING(2, 1);

    private int id;
    private int idUI;

    AuditUserTables(int id, int idUI) {
        this.id = id;
        this.idUI = idUI;
    }

    public int getId() {
        return id;
    }

    public int getIdUI() {
        return idUI;
    }

    public static AuditUserTables getById(int id) {
        for (AuditUserTables value : AuditUserTables.values()) {
            if (value.getId() == id)
                return value;
        }
        return NONE;
    }

    public static AuditUserTables getByIdUI(int idUI) {
        for (AuditUserTables value : AuditUserTables.values()) {
            if (value.getIdUI() == idUI)
                return value;
        }
        return NONE;
    }
}
