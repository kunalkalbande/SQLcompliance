package com.idera.sqlcm.ui.components.alerts;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import org.apache.commons.lang.StringUtils;

import java.util.HashMap;

public enum AlertLevel {
    SEVERE(4, "critical", "status-critical", 3, SQLCMI18NStrings.ALERTS_SEVERITY_CRITICAL),
    HIGH(3, "warning", "status-warning", 2, SQLCMI18NStrings.ALERTS_SEVERITY_WARNING),
    MEDIUM(2, "Medium", "status-informational", 1, SQLCMI18NStrings.ALERTS_SEVERITY_INFO),
    LOW(1, "ok", "status-ok", 0, SQLCMI18NStrings.ALERTS_SEVERITY_OK);

    private static HashMap<Integer, AlertLevel> lookUpByIdMap = new HashMap<>();
    private static HashMap<String, AlertLevel> lookUpByNameMap = new HashMap<>();
    private static HashMap<Integer, AlertLevel> lookUpByLevel = new HashMap<>();

    static {
        for (AlertLevel metric : AlertLevel.values()) {
            lookUpByIdMap.put(metric.getId(), metric);
            lookUpByNameMap.put(StringUtils.lowerCase(metric.getValue()), metric);
            lookUpByLevel.put(metric.getLevel(), metric);
        }
    }

    private int id;
    private String value;
    private String imageUrl;
    private int level;
    private String labelKey;

    AlertLevel(int id, String value, String imageUrl, int level, String labelKey) {
        this.id = id;
        this.value = value;
        this.imageUrl = imageUrl;
        this.level = level;
        this.labelKey = labelKey;
    }

    public static AlertLevel getById(int id) {
        AlertLevel severity = lookUpByIdMap.get(id);
        return (severity == null) ? AlertLevel.LOW : severity;
    }

    public static AlertLevel getByValue(String value) {
        AlertLevel severity = lookUpByNameMap.get(StringUtils.lowerCase(value));
        return (severity == null) ? AlertLevel.LOW : severity;
    }

    public static AlertLevel getByLevel(int id) {
        AlertLevel severity = lookUpByLevel.get(id);
        return (severity == null) ? AlertLevel.LOW : severity;
    }

    public String getValue() {
        return value;
    }

    public int getId() {
        return id;
    }

    public String getImageUrl() {
        return imageUrl;
    }

    public int getLevel() {
        return level;
    }

    public String getLabel() {
        return ELFunctions.getLabel(labelKey);
    }
}
