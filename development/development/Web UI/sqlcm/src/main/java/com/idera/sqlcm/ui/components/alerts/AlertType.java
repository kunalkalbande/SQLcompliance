package com.idera.sqlcm.ui.components.alerts;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import org.apache.commons.lang.StringUtils;

import java.util.HashMap;

public enum AlertType {
    DATA(3, "data", 0, SQLCMI18NStrings.ALERTS_TYPE_DATA, SQLCMI18NStrings.ALERTS_TYPE_DATA_PLURAL),
    EVENT(1, "event", 1, SQLCMI18NStrings.ALERTS_TYPE_EVENT, SQLCMI18NStrings.ALERTS_TYPE_EVENT_PLURAL),
    STATUS(2, "status", 2, SQLCMI18NStrings.ALERTS_TYPE_STATUS, SQLCMI18NStrings.ALERTS_TYPE_STATUS_PLURAL);

    private static HashMap<Integer, AlertType> lookUpByIdMap = new HashMap<>();
    private static HashMap<String, AlertType> lookUpByNameMap = new HashMap<>();
    private static HashMap<Integer, AlertType> lookUpByLevel = new HashMap<>();

    static {
        for (AlertType metric : AlertType.values()) {
            lookUpByIdMap.put(metric.getId(), metric);
            lookUpByNameMap.put(StringUtils.lowerCase(metric.getValue()), metric);
            lookUpByLevel.put(metric.getOrder(), metric);
        }
    }

    private int id;
    private String value;
    private int order;
    private String labelKey;
    private String labelKeyPlural;

    AlertType(int id, String value, int level, String labelKey, String labelKeyPlural) {
        this.id = id;
        this.value = value;
        this.order = level;
        this.labelKey = labelKey;
        this.labelKeyPlural = labelKeyPlural;
    }

    public static AlertType getById(int id) {
        AlertType severity =  lookUpByIdMap.get(id);
        return (severity == null) ? AlertType.EVENT : severity;
    }

    public static AlertType getByValue(String value) {
        AlertType severity = lookUpByNameMap.get(StringUtils.lowerCase(value));
        return (severity == null) ? AlertType.EVENT : severity;
    }

    public static AlertType getByLevel(int id) {
        AlertType severity =  lookUpByLevel.get(id);
        return (severity == null) ? AlertType.EVENT : severity;
    }

    public String getValue() {
        return value;
    }

    public int getId() {
        return id;
    }

    public int getOrder() {
        return order;
    }

    public String getLabel() {
        return ELFunctions.getLabel(labelKey);
    }

    public String getLabelPlural() {
        return ELFunctions.getLabel(labelKeyPlural);
    }
}
