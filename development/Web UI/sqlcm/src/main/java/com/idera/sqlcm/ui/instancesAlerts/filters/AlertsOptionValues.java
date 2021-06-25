package com.idera.sqlcm.ui.instancesAlerts.filters;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum AlertsOptionValues {
    SEVERE_STATUS("severeStatusTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.SEVERE), 4, ELFunctions.getLabel(SQLCMI18NStrings.SEVERE), "severe"),
    HIGH_STATUS("highStatusTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.HIGH), 3, ELFunctions.getLabel(SQLCMI18NStrings.HIGH), "high"),
    MEDIUM_STATUS("mediumStatusIconFilter", ELFunctions.getLabel(SQLCMI18NStrings.MEDIUM), 2, ELFunctions.getLabel(SQLCMI18NStrings.MEDIUM), "medium"),
    LOW_STATUS("lowStatusIconFilter", ELFunctions.getLabel(SQLCMI18NStrings.LOW), 1, ELFunctions.getLabel(SQLCMI18NStrings.LOW), "low");

    private String id;
    private String value;
    private int intValue;
    private String label;
    private String image;

    private static Map<Integer, AlertsOptionValues> lookup = new HashMap<Integer, AlertsOptionValues>();
    static {
        for (AlertsOptionValues status : AlertsOptionValues.values()) {
            lookup.put(status.intValue, status);
        }
    }

    AlertsOptionValues(String id, String value, int intValue, String label, String imageURL) {
        this.id = id;
        this.value = value;
        this.intValue = intValue;
        this.label = label;
        this.image = imageURL;
    }

    public String getId() {
        return id;
    }

    public String getValue() {
        return value;
    }

    public int getIntValue() {
        return intValue;
    }

    public String getLabel() {
        return label;
    }

    public String getImage() {
        return image;
    }

    public static String findLabelByKey(int statusId) {
        return lookup.get(statusId).getLabel();
    }

    public static String findImageByKey(int statusId) {
        return lookup.get(statusId).getImage();
    }
}
