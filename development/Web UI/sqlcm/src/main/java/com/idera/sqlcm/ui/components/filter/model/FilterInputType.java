package com.idera.sqlcm.ui.components.filter.model;

public enum FilterInputType {
    DIGIT(0),
    DIGIT_RANGE(0),
    COMBO(1),
    TEXT(1),
    OPTIONS(2),
    DATE(3),
    DATE_RANGE(3),
    TIME_RANGE(4);
    //DIGIT_RANGE(4),
    //DATE_RANGE(5);

    private int value;

    private FilterInputType(final int value) {
        this.value = value;
    }

    public static FilterInputType fromValue(int filterOperatorId) {
        for (FilterInputType c : FilterInputType.values()) {
            if (c.getValue() == filterOperatorId) {
                return c;
            }
        }
        throw new IllegalArgumentException("Invalid filter input type: " + filterOperatorId);
    }

    public int getValue() {
        return value;
    }
}
