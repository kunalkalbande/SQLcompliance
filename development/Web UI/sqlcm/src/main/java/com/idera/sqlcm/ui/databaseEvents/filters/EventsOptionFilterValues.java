package com.idera.sqlcm.ui.databaseEvents.filters;

import com.idera.sqlcm.enumerations.State;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum EventsOptionFilterValues {

    ACCESS_CHECK_FAILED("AccessCheckFailedFilter", "FAILED", 0, ELFunctions.getLabel(SQLCMI18NStrings.EVENT_ACCESS_CHECK_FAILED)),
    ACCESS_CHECK_PASSED("AccessCheckPassedFilter", "PASSED", 1, ELFunctions.getLabel(SQLCMI18NStrings.EVENT_ACCESS_CHECK_PASSED)),
    PRIVILEGED_USER_NO("PrivilegedUserNoFilter", "NO", 0, ELFunctions.getLabel(SQLCMI18NStrings.PRIVILEGED_USER_NO)),
    PRIVILEGED_USER_YES("PrivilegedUserYesFilter", "YES", 1, ELFunctions.getLabel(SQLCMI18NStrings.PRIVILEGED_USER_YES));

    private String id;

    private String value;

    private int intValue;

    private String label;

    EventsOptionFilterValues(String id, String value, int intValue, String label) {
        this.id = id;
        this.value = value;
        this.intValue = intValue;
        this.label = label;
    }

    public String getId() {
        return id;
    }

    public String getValue() {
        return value;
    }

    public int getIntValue() {
        return intValue;
    }

    public String getLabel() {
        return label;
    }
}
