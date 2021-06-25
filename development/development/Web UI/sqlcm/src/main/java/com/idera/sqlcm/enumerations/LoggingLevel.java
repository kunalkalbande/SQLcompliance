package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum LoggingLevel {
    SILENT(0, ELFunctions.getLabel(SQLCMI18NStrings.LOGGING_LEVEL_SILENT)),
    NORMAL(1, ELFunctions.getLabel(SQLCMI18NStrings.LOGGING_LEVEL_NORMAL)),
    VERBOSE(2, ELFunctions.getLabel(SQLCMI18NStrings.LOGGING_LEVEL_VERBOSE)),
    DEBUG(3, ELFunctions.getLabel(SQLCMI18NStrings.LOGGING_LEVEL_DEBUG));

    private int index;
    private String label;

    LoggingLevel(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static LoggingLevel getByIndex(int index) {
        LoggingLevel result = null;
        for (int i= 0; i < LoggingLevel.values().length; i++) {
            if (LoggingLevel.values()[i].getIndex() == index) {
                result = LoggingLevel.values()[i];
            }
        }
        return result;
    }
}
