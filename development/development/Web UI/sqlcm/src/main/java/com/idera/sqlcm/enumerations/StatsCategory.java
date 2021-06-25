package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum StatsCategory {
    ALERTS(                4, ELFunctions.getLabel(SQLCMI18NStrings.STATS_CATEGORY_ALERTS),                 0),
    PRIVILEGED_USER_EVENTS(5, ELFunctions.getLabel(SQLCMI18NStrings.STATS_CATEGORY_PRIVILEGED_USER_EVENTS), 4),
    FAILED_LOGIN(          6, ELFunctions.getLabel(SQLCMI18NStrings.STATS_CATEGORY_FAILED_LOGIN),           1),
    DDL(                   9, ELFunctions.getLabel(SQLCMI18NStrings.STATS_CATEGORY_DDL),                    3),
    SECURITY(              10, ELFunctions.getLabel(SQLCMI18NStrings.STATS_CATEGORY_SECURITY),              2),
    EVENTS_PROCESSED(      21, ELFunctions.getLabel(SQLCMI18NStrings.STATS_CATEGORY_EVENTS_PROCESSED),      5);

    private int index;
    private String label;
    private int orderNumber;

    StatsCategory(int index, String label, int orderNumber) {
        this.index = index;
        this.label = label;
        this.orderNumber = orderNumber;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public int getOrderNumber() {
        return orderNumber;
    }

    public static StatsCategory getByIndex(int index) {
        StatsCategory result = null;
        for (int i= 0; i < StatsCategory.values().length; i++) {
            if (StatsCategory.values()[i].getIndex() == index) {
                result = StatsCategory.values()[i];
            }
        }
        return result;
    }
}
