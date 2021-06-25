package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum ArchiveCheckStatus {
    NONE(-1, ""),
    COMPLETED(0, ELFunctions.getLabel(SQLCMI18NStrings.ARCHIVE_CHECK_STATUS_COMPLETED)),
    IN_PROGRESS(1, ELFunctions.getLabel(SQLCMI18NStrings.ARCHIVE_CHECK_STATUS_IN_PROGRESS)),
    FAILED_INTEGRITY(2, ELFunctions.getLabel(SQLCMI18NStrings.ARCHIVE_CHECK_STATUS_FAILED_INTEGRITY)),
    FAILED_WITH_ERRORS(3, ELFunctions.getLabel(SQLCMI18NStrings.ARCHIVE_CHECK_STATUS_FAILED_WITH_ERRORS)),
    INCOMPLETE(4, ELFunctions.getLabel(SQLCMI18NStrings.ARCHIVE_CHECK_STATUS_INCOMPLETE));

    private int index;
    private String label;

    ArchiveCheckStatus(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static ArchiveCheckStatus getByIndex(int index) {
        ArchiveCheckStatus result = null;
        for (int i= 0; i < ArchiveCheckStatus.values().length; i++) {
            if (IntegrityCheckStatus.values()[i].getIndex() == index) {
                result = ArchiveCheckStatus.values()[i];
            }
        }
        return result;
    }
}
