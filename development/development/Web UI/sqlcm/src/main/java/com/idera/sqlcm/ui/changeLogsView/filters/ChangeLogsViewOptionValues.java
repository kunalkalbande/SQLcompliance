package com.idera.sqlcm.ui.changeLogsView.filters;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum ChangeLogsViewOptionValues {
    SEVERE_STATUS("severeStatusTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.SEVERE_STATUS), 4, ELFunctions.getLabel(SQLCMI18NStrings.SEVERE), "severe"),
    HIGH_STATUS("highStatusTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.HIGH_STATUS), 3, ELFunctions.getLabel(SQLCMI18NStrings.HIGH), "high"),
    MEDIUM_STATUS("mediumStatusIconFilter", ELFunctions.getLabel(SQLCMI18NStrings.MEDIUM_STATUS), 2, ELFunctions.getLabel(SQLCMI18NStrings.MEDIUM), "medium"),
    LOW_STATUS("lowStatusIconFilter", ELFunctions.getLabel(SQLCMI18NStrings.LOW_STATUS), 1, ELFunctions.getLabel(SQLCMI18NStrings.LOW), "low");

    private String id;
    private String value;
    private int intValue;
    private String label;
    private String image;

    private static Map<Integer, ChangeLogsViewOptionValues> lookup = new HashMap<Integer, ChangeLogsViewOptionValues>();
    static {
        for (ChangeLogsViewOptionValues status : ChangeLogsViewOptionValues.values()) {
            lookup.put(status.intValue, status);
        }
    }

    ChangeLogsViewOptionValues(String id, String value, int intValue, String label, String imageURL) {
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
