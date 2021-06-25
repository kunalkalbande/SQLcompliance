package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.server.web.ELFunctions;

public enum State {
    DISABLED(0, ELFunctions.getLabel("Labels.sql-cm.state-disabled")),
    ENABLED(1, ELFunctions.getLabel("Labels.sql-cm.state-enabled")),
    YES_Email(1, ELFunctions.getLabel("Labels.SQLCM.Labels.yes")),
    NO_Email(0, ELFunctions.getLabel("Labels.SQLCM.Labels.no")),
    YES_EventLog(1, ELFunctions.getLabel("Labels.SQLCM.Labels.yes")),
    NO_EventLog(0, ELFunctions.getLabel("Labels.SQLCM.Labels.no")),
    YES_SNMP(1, ELFunctions.getLabel("Labels.SQLCM.Labels.yes")),
    NO_SNMP(0, ELFunctions.getLabel("Labels.SQLCM.Labels.no")),
    DISABLED_2(2, ELFunctions.getLabel("Labels.sql-cm.state-disabled"));

    private int index;
    private String label;

    State(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static State getByIndex(int index) {
        return State.values()[index];
    }
}
