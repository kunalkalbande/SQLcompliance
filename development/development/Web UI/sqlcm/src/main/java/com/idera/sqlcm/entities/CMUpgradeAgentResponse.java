package com.idera.sqlcm.entities;

public class CMUpgradeAgentResponse extends CMResponse {
    private String upgradeStatusMessage;

    public String getUpgradeStatusMessage() {
        return upgradeStatusMessage;
    }

    public void setUpgradeStatusMessage(String upgradeStatusMessage) {
        this.upgradeStatusMessage = upgradeStatusMessage;
    }
}
