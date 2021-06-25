package com.idera.sqlcm.ui.instancesAlertsRule.filters;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum AlertsRuleTypeOptionValues {
	EVENT("eventTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.EVENT), 1, ELFunctions.getLabel(SQLCMI18NStrings.EVENT), "event"),
	STATUS("statusTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.STATUS), 2, ELFunctions.getLabel(SQLCMI18NStrings.STATUS), "status"),
	DATA("dataTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.DATA), 3, ELFunctions.getLabel(SQLCMI18NStrings.DATA), "data");

    private String id;
    private String value;
    private int intValue;
    private String label;
    private String image;

    private static Map<Integer, AlertsRuleTypeOptionValues> lookup = new HashMap<Integer, AlertsRuleTypeOptionValues>();
    static {
        for (AlertsRuleTypeOptionValues status : AlertsRuleTypeOptionValues.values()) {
            lookup.put(status.intValue, status);
        }
    }

    AlertsRuleTypeOptionValues(String id, String value, int intValue, String label, String imageURL) {
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
