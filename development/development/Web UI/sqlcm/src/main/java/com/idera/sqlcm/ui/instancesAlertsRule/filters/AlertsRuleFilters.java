package com.idera.sqlcm.ui.instancesAlertsRule.filters;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.components.filter.model.FilterType;
import com.idera.sqlcm.ui.instancesAlertsRule.AlertsRuleColumns;

public enum AlertsRuleFilters {
	RULE("Rule", FilterType.COMBO, SQLCMI18NStrings.RULE, AlertsRuleColumns.RULE),
	RULE_TYPE("RuleType", FilterType.OPTIONS, SQLCMI18NStrings.RULE_TYPE, AlertsRuleColumns.RULE_TYPE),
	SERVER("Server", FilterType.TEXT, SQLCMI18NStrings.SERVER, AlertsRuleColumns.SERVER),
	LEVEL("Levels", FilterType.OPTIONS, SQLCMI18NStrings.LEVEL, AlertsRuleColumns.LEVEL),
    E_MAIL("Email", FilterType.OPTIONS, SQLCMI18NStrings.ALERT_RULE_EMAIL, AlertsRuleColumns.E_MAIL),
    EVENT_LOG("EventLog", FilterType.OPTIONS, SQLCMI18NStrings.EVENT_LOG, AlertsRuleColumns.EVENT_LOG),
    SNMP_TRAP("SnmpTrap", FilterType.OPTIONS, SQLCMI18NStrings.SNMP_TRAP, AlertsRuleColumns.SNMP_TRAP);
	
    private String filterId;
    private FilterType filterType;
    private String filterName;
    private AlertsRuleColumns columnId;

    AlertsRuleFilters(String filterId, FilterType filterType, String filterName, AlertsRuleColumns columnId) {
        this.filterId = filterId;
        this.filterType = filterType;
        this.filterName = filterName;
        this.columnId = columnId;
    }

    public String getFilterId() {
        return filterId;
    }

    public FilterType getFilterType() {
        return filterType;
    }

    public String getFilterName() {
        return filterName;
    }

    public AlertsRuleColumns getColumnId() {
        return columnId;
    }

}