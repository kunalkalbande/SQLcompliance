package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum AuditedActivity {
    CHECK_ALL_ACTIVITIES(     true,  ELFunctions.getLabel(SQLCMI18NStrings.INST_PROP_D_PRIVILEGED_USER_AUD_TAB_AUDIT_ALL_ACTIVITIES)),
    CHECK_SELECTED_ACTIVITIES(false, ELFunctions.getLabel(SQLCMI18NStrings.INST_PROP_D_PRIVILEGED_USER_AUD_TAB_AUDIT_SELECTED_ACTIVITIES));

    private String label;
    private boolean id;

    private AuditedActivity(boolean id, String label) {
        this.id = id;
        this.label = label;
    }

    public String getLabel() {
        return label;
    }

    public boolean isId() {
        return id;
    }

    public static AuditedActivity getByIndex(boolean id) {
        for(AuditedActivity e : values()) {
            if(e.id == id) {
                return e;
            }
        }
        return null;
    }
}

