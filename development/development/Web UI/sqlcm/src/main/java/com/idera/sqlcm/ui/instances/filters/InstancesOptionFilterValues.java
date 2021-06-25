package com.idera.sqlcm.ui.instances.filters;

import com.idera.sqlcm.enumerations.State;

public enum InstancesOptionFilterValues {
    AUDIT_STATUS_ENABLED( "enabledAuditStatusFilter",  "ENABLED", State.ENABLED.getIndex(), "Enabled"),
    AUDIT_STATUS_DISABLED("disabledAuditStatusFilter", "DISABLED", State.DISABLED.getIndex(), "Disabled"),
   
    EVENT_LOG_YES("eventLogYES", "YES", State.YES_EventLog.getIndex(), "Yes"),
    EVENT_LOG_NO("eventLogNO", "NO", State.NO_EventLog.getIndex(), "No"),
    EMAIL_YES("emailYES", "YES", State.YES_Email.getIndex(), "Yes"),
    EMAIL_NO("emailNO", "NO", State.NO_Email.getIndex(), "No"),
    SNMP_TRAP_YES("snmpTrapYES", "YES", State.YES_SNMP.getIndex(), "Yes"),
    SNMP_TRAP_NO("snmpTrapNO", "NO", State.NO_SNMP.getIndex(), "No"),
    
    STATUS_TEXT_ONLINE(   "onlineStatusTextFilter",    "ONLINE",   3, "Online"),
    STATUS_TEXT_ERROR(    "errorStatusTextFilter",     "ERROR",    2, "Error"),
    STATUS_OK(            "okStatusIconFilter",        "OK",       1, "OK"),
    STATUS_ERROR(         "errorStatusIconFilter",     "ERROR",    0, "Error");

    private String id;
    private String value;
    private int intValue;
    private String label;

    InstancesOptionFilterValues(String id, String value, int intValue, String label) {
        this.id = id;
        this.value = value;
        this.intValue = intValue;
        this.label = label;
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
}
