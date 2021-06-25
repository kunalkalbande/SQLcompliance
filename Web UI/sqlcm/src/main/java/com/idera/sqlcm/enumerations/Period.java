package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.server.web.ELFunctions;

public enum Period {
    PER_HOUR(4, ELFunctions.getLabel("Labels.sql-cm.period-per-hour")),
    PER_DAY(96, ELFunctions.getLabel("Labels.sql-cm.period-per-day"));

    private int index;
    private String label;

    Period(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static Period getByIndex(int index) {
        for (Period e : values()) {
            if (e.index == index) {
                return e;
            }
        }
        return null;
    }
}
