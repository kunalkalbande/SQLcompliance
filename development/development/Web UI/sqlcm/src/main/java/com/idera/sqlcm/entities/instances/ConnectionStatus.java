package com.idera.sqlcm.entities.instances;

import com.idera.common.Status;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

public enum ConnectionStatus implements Status {
    OK(0, SQLCMI18NStrings.OK, "instance-up", 2),
    ERROR(1, SQLCMI18NStrings.ERROR, "instance-error", 1),
    WARNING(2, SQLCMI18NStrings.WARNING, "instance-unknown", 3),
    REFRESHING(3, SQLCMI18NStrings.REFRESHING, "agent-update", 0);

    private int code;
    private String state;
    private String iconURL;
    private Integer sortValue;

    ConnectionStatus(int code, String state, String iconURL, Integer sortValue) {
        this.code = code;
        this.state = state;
        this.iconURL = iconURL;
        this.sortValue = sortValue;
    }

    public static ConnectionStatus getConnectionStatus(boolean isOk) {
        if (isOk) {
            return ConnectionStatus.OK;
        }
        return ConnectionStatus.ERROR;
    }

    public static ConnectionStatus getConnectionStatus(int online, boolean needUpdate, boolean differentManagmentService, int refreshStatus) {
        if (refreshStatus == 1) return ConnectionStatus.REFRESHING;
        if (needUpdate || differentManagmentService) return ConnectionStatus.WARNING;
        if (online == 1) return ConnectionStatus.OK;
        return ConnectionStatus.ERROR;
    }

    public int getCode() {
        return code;
    }

    public void setCode(int code) {
        this.code = code;
    }

    public String getState() {
        return state;
    }

    public void setState(String state) {
        this.state = state;
    }

    public String getI18Nkey() {
        return state;
    }

    public String getIconURL() {
        return iconURL;
    }

    public void setIconURL(String iconURL) {
        this.iconURL = iconURL;
    }

    public Integer getSortValue() {
        return sortValue;
    }

    public void setSortValue(Integer sortValue) {
        this.sortValue = sortValue;
    }
}

