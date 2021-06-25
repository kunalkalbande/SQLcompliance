package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.server.web.ELFunctions;

public enum TimeMeasurement {
    MINUTE(0, ELFunctions.getLabel("SQLCM.time-measurement.minute")),
    DAY(1, ELFunctions.getLabel("SQLCM.time-measurement.day"));

    private int index;
    private String label;

    TimeMeasurement(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static TimeMeasurement getByIndex(int index) {
        return TimeMeasurement.values()[index];
    }
}
