package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum IntegrityCheckStatus {
    NONE(-1, ""),
    PASSED(0, ELFunctions.getLabel(SQLCMI18NStrings.INTEGRITY_CHECK_STATUS_PASSED)),
    IN_PROGRESS(1, ELFunctions.getLabel(SQLCMI18NStrings.INTEGRITY_CHECK_STATUS_IN_PROGRESS)),
    FAILED(2, ELFunctions.getLabel(SQLCMI18NStrings.INTEGRITY_CHECK_STATUS_FAILED)),
    FAILED_REPAIRED(3, ELFunctions.getLabel(SQLCMI18NStrings.INTEGRITY_CHECK_STATUS_FAILED_REPAIRED)),
    INCOMPLETE(4, ELFunctions.getLabel(SQLCMI18NStrings.INTEGRITY_CHECK_STATUS_INCOMPLETE));

    private int index;
    private String label;

    IntegrityCheckStatus(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static IntegrityCheckStatus getByIndex(int index) {
        IntegrityCheckStatus result = null;
        for (int i= 0; i < IntegrityCheckStatus.values().length; i++) {
            if (IntegrityCheckStatus.values()[i].getIndex() == index) {
                result = IntegrityCheckStatus.values()[i];
            }
        }
        return result;
    }
}
