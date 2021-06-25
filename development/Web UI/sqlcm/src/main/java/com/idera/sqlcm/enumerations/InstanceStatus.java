package com.idera.sqlcm.enumerations;

public enum InstanceStatus {
    OK(0),
    WARNING(1),
    ALERT(2),
    ARCHIVE(3),
    DISABLED(4),
    UNKNOWN(6);

    private long id;

    InstanceStatus(long id) {
        this.id = id;
    }

    public long getId() {
        return id;
    }

    public static InstanceStatus getById(long id) {
        for (InstanceStatus e : values()) {
            if (e.id == id) {
                return e;
            }
        }
        return UNKNOWN;
    }

}
