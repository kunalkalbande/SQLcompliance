package com.idera.sqlcm.facade.filter;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;

public enum FilterTypes {
    INSTANCES_NAME(SQLCMI18NStrings.INSTANCE_NAME_COLUMN, "instance-icon"),
    STATUS_TEXT(SQLCMI18NStrings.STATUS_TEXT_COLUMN, "text-icon"),
    NUMBER_OF_AUDITED_DB(SQLCMI18NStrings.NUMBER_OF_AUDITED_DB_COLUMN, "digit-icon"),
    SQL_SERVER_VERSION_EDITION(SQLCMI18NStrings.SQL_SERVER_VERSION_AND_EDITION_COLUMN, "version-icon"),
    AUDIT_STATUS(SQLCMI18NStrings.AUDIT_STATUS_COLUMN, "text-icon"),
    LAST_AGENT_CONTACT(SQLCMI18NStrings.LAST_AGENT_CONTACT_COLUMN , "text-icon"); //// What means icon?

    String i18nKey;

    private String iconUrl;

    FilterTypes(String i18nKey, String iconUrl) {
        this.i18nKey = i18nKey;
        this.iconUrl = iconUrl;
    }

    public String getI18nKey() {
        return i18nKey;
    }

    public String getIconUrl() {
        return iconUrl;
    }

    public static FilterTypes fromString(String value) {
        for (FilterTypes tmp : FilterTypes.values()) {
            if (tmp.toString().equals(value))
                return tmp;
        }
        return null;
    }
}
