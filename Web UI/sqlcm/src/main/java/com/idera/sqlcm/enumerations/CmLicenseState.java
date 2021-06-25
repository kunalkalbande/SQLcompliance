package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum CmLicenseState {
    VALID(0, ELFunctions.getLabel(SQLCMI18NStrings.LICENSE_STATE_OK)),
    INVALID_KEY(1, ELFunctions.getLabel(SQLCMI18NStrings.LICENSE_STATE_INVALID_KEY)),
    INVALID_PRODUCT_ID(2, ELFunctions.getLabel(SQLCMI18NStrings.LICENSE_STATE_INVALID_PRODUCT_ID)),
    INVALID_SCOPE(3, ELFunctions.getLabel(SQLCMI18NStrings.LICENSE_STATE_INVALID_SCOPE)),
    INVALID_EXPIRED(4, ELFunctions.getLabel(SQLCMI18NStrings.LICENSE_STATE_INVALID_EXPIRED)),
    INVALID_MIXED_TYPES(5, ELFunctions.getLabel(SQLCMI18NStrings.LICENSE_STATE_INVALID_MIXED_TYPES)),
    INVALID_DUPLICATE_LICENSE(6, ELFunctions.getLabel(SQLCMI18NStrings.LICENSE_STATE_INVALID_DUPLICATE_LICENSE)),
    INVALID_PRODUCT_VERSION(7, ELFunctions.getLabel(SQLCMI18NStrings.LICENSE_STATE_INVALID_PRODUCT_VERSION));

    private int index;
    private String label;

    CmLicenseState(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static CmLicenseState getByIndex(int index) {
        for(CmLicenseState e : values()) {
            if(e.index == index) {
                return e;
            }
        }
        return null;
    }
}
