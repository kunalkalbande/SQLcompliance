package com.idera.sqlcm.enumerations;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public enum AgentDeploymentType {
    AUTOMATIC(false, ELFunctions.getLabel(SQLCMI18NStrings.AGENT_DEPLOYMENT_AUTOMATIC)),
    MANUAL(true, ELFunctions.getLabel(SQLCMI18NStrings.AGENT_DEPLOYMENT_MANUAL));

    private boolean index;
    private String label;

    AgentDeploymentType(boolean index, String label) {
        this.index = index;
        this.label = label;
    }

    public boolean getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static AgentDeploymentType getByIndex(boolean index) {
        AgentDeploymentType result = null;
        AgentDeploymentType[] values = AgentDeploymentType.values();
        for (int i= 0; i < values.length; i++) {
            if (values[i].getIndex() == index) {
                result = values[i];
            }
        }
        return result;
    }
}
