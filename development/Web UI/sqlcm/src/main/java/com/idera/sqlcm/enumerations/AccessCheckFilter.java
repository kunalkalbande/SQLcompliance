package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum AccessCheckFilter {
    PASSED_ONLY(0, ELFunctions.getLabel(SQLCMI18NStrings.INST_PROP_D_AUD_ACTIVITY_TAB_AUDIT_ACTIONS_PASS_ACCESS_CHECK)),
    FAILED_ONLY(2, ELFunctions.getLabel(SQLCMI18NStrings.INST_PROP_D_AUD_ACTIVITY_TAB_AUDIT_ACTIONS_FAILED_ACCESS_CHECK)),
    DISABLED(1, null);

    private int id;
    private String label;

    AccessCheckFilter(int id, String label) {
        this.id = id;
        this.label = label;
    }

    public int getId() {
        return id;
    }

    public String getLabel() {
        return label;
    }

    public static AccessCheckFilter getByIndex(int index) {
        for (AccessCheckFilter value : AccessCheckFilter.values()) {
            if (value.getId() == index) {
                return value;
            }
        }
        return DISABLED;
    }
}
