package com.idera.sqlcm.ui.instancesAlertsRule;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.HashMap;
import java.util.Map;

public enum AlertsRuleColumns {

	RULE("alertRuleColumn", SQLCMI18NStrings.RULE, true, true),
	RULE_TYPE("alertRuleTypeColumn", SQLCMI18NStrings.RULE_TYPE, true, true),
	SERVER("alertRuleServerColumn", SQLCMI18NStrings.SERVER, true, true),
    LEVEL("alertRuleLevelColumn", SQLCMI18NStrings.LEVEL, true, true),
    E_MAIL("alertRuleEmailColumn", SQLCMI18NStrings.ALERT_RULE_EMAIL, true, true),
    EVENT_LOG("alertRuleEventLogColumn", SQLCMI18NStrings.EVENT_LOG, true, true),
    SNMP_TRAP("alertRuleSNMPTrapColumn", SQLCMI18NStrings.SNMP_TRAP, true, true);

    private final String columnId;
    private String labelKey;
    private boolean sortable;
    private boolean visible;
    private static String INSTANCES_ALERTS_RULES_SESSION_DATA_BEAN = "InstancesAlertsRuleSessionDataBean";

    private static Map<String, AlertsRuleColumns> lookup = new HashMap<String, AlertsRuleColumns>();
    static {
        for (AlertsRuleColumns alertsColumns : AlertsRuleColumns.values()) {
            lookup.put(alertsColumns.columnId, alertsColumns);
        }
    }

    private AlertsRuleColumns(String columnId, String labelKey, boolean sortable, boolean visible) {
        this.columnId = columnId;
        this.labelKey = labelKey;
        this.sortable = sortable;
        this.visible = visible;
    }


    public String getColumnId() {
        return columnId;
    }

    public String getLabelKey() {
        return labelKey;
    }

    public boolean isSortable() {
        return sortable;
    }

    public String getLabel() {
        return ELFunctions.getLabel(getLabelKey());
    }

    public boolean isVisible() {
        return visible;
    }

    public void setVisible(boolean visible) {
        this.visible = visible;
    }

    public static AlertsRuleColumns findColumnById(String columnId) {
        return lookup.get(columnId);
    }

    public static String getSource(){
        return INSTANCES_ALERTS_RULES_SESSION_DATA_BEAN;
    }

    @Override
    public String toString() {
        return columnId;
    }

}
