package com.idera.sqlcm.ui.activityLogsView.filters;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum ActivityLogsViewOptionValues {
    LOGS_ERROR("logsErrorImg", ELFunctions.getLabel(SQLCMI18NStrings.SEVERE_STATUS), 3, ELFunctions.getLabel(SQLCMI18NStrings.SEVERE), "statusError"),
    LOGS_RESOLUTION("logsResolutionImg", ELFunctions.getLabel(SQLCMI18NStrings.HIGH_STATUS), 2, ELFunctions.getLabel(SQLCMI18NStrings.HIGH), "statusGood"),
    LOGS_INFORMATION("logsInformationImg", ELFunctions.getLabel(SQLCMI18NStrings.MEDIUM_STATUS), 1, ELFunctions.getLabel(SQLCMI18NStrings.MEDIUM), "information");

    private String id;
    private String value;
    private int intValue;
    private String label;
    private String image;

    private static Map<Integer, ActivityLogsViewOptionValues> lookup = new HashMap<Integer, ActivityLogsViewOptionValues>();
    static {
        for (ActivityLogsViewOptionValues status : ActivityLogsViewOptionValues.values()) {
            lookup.put(status.intValue, status);
        }
    }

    ActivityLogsViewOptionValues(String id, String value, int intValue, String label, String imageURL) {
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
