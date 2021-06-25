package com.idera.sqlcm.enumerations;

public enum UpgradeAgentType {
    MINOR_UPGRADE(0, "minorUpgrade"),
    MAJOR_UPGRADE(1, "majorUpgrade");

    private int index;
    private String label;

    UpgradeAgentType(int index, String label) {
        this.index = index;
        this.label = label;
    }

    public int getIndex() {
        return index;
    }

    public String getLabel() {
        return label;
    }

    public static UpgradeAgentType getByIndex(int index) {
        UpgradeAgentType result = null;
        UpgradeAgentType[] values = UpgradeAgentType.values();
        for (UpgradeAgentType value : values) {
            if (value.getIndex() == index) {
                result = value;
            }
        }
        return result;
    }
}
