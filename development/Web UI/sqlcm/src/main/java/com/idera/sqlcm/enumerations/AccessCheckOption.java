package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum AccessCheckOption {
    PASSED_ONLY(0, ELFunctions.getLabel(SQLCMI18NStrings.INST_PROP_D_PRIVILEGED_USER_AUD_TAB_PASSED)),
    FAILED_ONLY(2, ELFunctions.getLabel(SQLCMI18NStrings.INST_PROP_D_PRIVILEGED_USER_AUD_TAB_FAILED)),
    DISABLED(1, null);

    private String label;
    private int id;

    private AccessCheckOption(int id, String label) {
        this.id = id;
        this.label = label;
    }

    public String getLabel() {
        return label;
    }

    public int getId() {
        return id;
    }

    public static AccessCheckOption getByIndex(int id) {
        for(AccessCheckOption e : values()) {
            if(e.id == id) {
                return e;
            }
        }
        return null;
    }
}
