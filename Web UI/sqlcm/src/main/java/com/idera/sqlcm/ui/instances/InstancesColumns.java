package com.idera.sqlcm.ui.instances;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;

public enum InstancesColumns {
    CHECKED("checkedColumn", SQLCMI18NStrings.INSTANCES_CHECKED_COLUMN, false),
    STATUS("statusColumn", SQLCMI18NStrings.STATUS_COLUMN, true),
    INSTANCE_NAME("instanceNameColumn", SQLCMI18NStrings.INSTANCE_NAME_COLUMN, true),
    STATUS_TEXT("statusTextColumn", SQLCMI18NStrings.STATUS_TEXT_COLUMN, true),
    NUMBER_OF_AUDITED_DB("numOfAuditedDBColumn", SQLCMI18NStrings.NUMBER_OF_AUDITED_DB_COLUMN, true),
    SQL_SERVER_VERSION_EDITION("serverVersionEditionColumn", SQLCMI18NStrings.SQL_SERVER_VERSION_AND_EDITION_COLUMN, true),
    AUDIT_STATUS("auditStatusColumn", SQLCMI18NStrings.AUDIT_STATUS_COLUMN, true),
    LAST_AGENT_CONTACT("lastAgentContactColumn", SQLCMI18NStrings.LAST_AGENT_CONTACT_COLUMN, true),
    OPTIONS("optionsColumn", SQLCMI18NStrings.INSTANCES_OPTIONS_COLUMN, false);

    private final String columnId;
    private String labelKey;
    private boolean sortable;

    private InstancesColumns(String columnId, String labelKey, boolean sortable) {
        this.columnId = columnId;
        this.labelKey = labelKey;
        this.sortable = sortable;
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

    @Override
    public String toString() {
        return columnId;
    }

}
