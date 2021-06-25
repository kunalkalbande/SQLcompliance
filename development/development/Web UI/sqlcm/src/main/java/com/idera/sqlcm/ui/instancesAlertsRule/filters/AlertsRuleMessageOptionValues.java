package com.idera.sqlcm.ui.instancesAlertsRule.filters;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum AlertsRuleMessageOptionValues {
	YES("yesTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.YES), 1, ELFunctions.getLabel(SQLCMI18NStrings.YES), "alert-rules-enabled"),
	NO("noTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.NO), 0, ELFunctions.getLabel(SQLCMI18NStrings.NO), "alert-rules-disabled");

    private String id;
    private String value;
    private int intValue;
    private String label;
    private String image;

    private static Map<Integer, AlertsRuleMessageOptionValues> lookup = new HashMap<Integer, AlertsRuleMessageOptionValues>();
    static {
        for (AlertsRuleMessageOptionValues status : AlertsRuleMessageOptionValues.values()) {
            lookup.put(status.intValue, status);
        }
    }

    AlertsRuleMessageOptionValues(String id, String value, int intValue, String label, String imageURL) {
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

    public static String findLabelByKey(int i) {
        return lookup.get(i).getLabel();
    }

    public static String findImageByKey(int statusId) {
        return lookup.get(statusId).getImage();
    }
}
