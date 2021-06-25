package com.idera.sqlcm.ui.instancesAlerts.filters;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum AlertRuleTypeEnum {
    RULETYPE_EVENT("ruleTypeEventTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.EVENT), 1, ELFunctions.getLabel(SQLCMI18NStrings.EVENT), "event"),
    RULETYPE_DATA("ruleTypeDataTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.DATA), 2, ELFunctions.getLabel(SQLCMI18NStrings.DATA), "data"),
    RULETYPE_STATUS("ruleTypeStatusTextFilter", ELFunctions.getLabel(SQLCMI18NStrings.STATUS), 3, ELFunctions.getLabel(SQLCMI18NStrings.STATUS), "status");
    
    private String id;
    private String value;
    private int intValue;
    private String label;
    private String image;

    private static Map<Integer, AlertRuleTypeEnum> lookup = new HashMap<Integer, AlertRuleTypeEnum>();
    static {
        for (AlertRuleTypeEnum status : AlertRuleTypeEnum.values()) {
            lookup.put(status.intValue, status);
        }
    }

    AlertRuleTypeEnum(String id, String value, int intValue, String label, String imageURL) {
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