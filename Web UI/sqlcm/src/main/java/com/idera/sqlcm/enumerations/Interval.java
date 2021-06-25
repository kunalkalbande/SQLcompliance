package com.idera.sqlcm.enumerations;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

public enum Interval {
    ALL(0, "All"),
    ONE_DAY(1, ELFunctions.getLabel(SQLCMI18NStrings.ONE_DAY)),
    SEVEN_DAY(7, ELFunctions.getLabel(SQLCMI18NStrings.SEVEN_DAYS)),
    THIRTY_DAY(30, ELFunctions.getLabel(SQLCMI18NStrings.THIRTY_DAY));

    private String label;
    private int days;

    private Interval(int days, String label) {
        this.label = label;
        this.days = days;

    }

    public String getLabel() {
        return label;
    }

    public String getName() {
        return this.name();
    }

    public int getDays() {
        return days;
    }
}
