package com.idera.sqlcm.enumerations;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

public enum Category {
    OVERALL_ACTIVITY(21, ELFunctions.getLabel(SQLCMI18NStrings.OVERALL_ACTIVITY)),
    EVENT_ALERT(4, ELFunctions.getLabel(SQLCMI18NStrings.EVENT_ALERTS)),
    FAILED_LOGIN(6, ELFunctions.getLabel(SQLCMI18NStrings.FAILED_LOGIN)),
    SECURITY(10, ELFunctions.getLabel(SQLCMI18NStrings.SECURITY)),
    DDL(9, ELFunctions.getLabel(SQLCMI18NStrings.DDL)),
    PRIVILEGED_USER(5, ELFunctions.getLabel(SQLCMI18NStrings.PRIVILEGED_USER));

    private String label;
    private int index;

    private Category(int index, String label) {
        this.label = label;
        this.index = index;

    }

    public String getLabel() {
        return label;
    }

    public String getName() {
        return this.name();
    }

    public int getIndex() {
        return index;
    }
}
