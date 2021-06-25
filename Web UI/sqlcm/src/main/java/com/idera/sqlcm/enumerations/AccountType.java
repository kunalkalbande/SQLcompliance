package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum AccountType {
    WINDOWS_USER_ACCOUNT(2, ELFunctions.getLabel(SQLCMI18NStrings.ACCOUNT_TYPE_WINDOWS_USER_ACCOUNT)),
    SERVER_LOGIN_ACCOUNT(1, ELFunctions.getLabel(SQLCMI18NStrings.ACCOUNT_TYPE_SERVER_LOGIN_ACCOUNT)),
    CM_ACCOUNT(2, ELFunctions.getLabel(SQLCMI18NStrings.ACCOUNT_TYPE_CM_ACCOUNT));

    private int index;
    private String label;

    AccountType(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static AccountType getByIndex(int index) {
        return AccountType.values()[index];
    }
}
