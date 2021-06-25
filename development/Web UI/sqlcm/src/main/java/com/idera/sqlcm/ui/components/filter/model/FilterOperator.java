package com.idera.sqlcm.ui.components.filter.model;

public enum FilterOperator {
    EQUALS(0),
    LIKE(1),
    RANGE(2);

    private int value;

    private FilterOperator(final int value) {
        this.value = value;
    }

    public int getValue() {
        return value;
    }

    public static FilterOperator fromValue(int filterOperatorId) {
        for (FilterOperator c : FilterOperator.values()) {
            if (c.getValue() == filterOperatorId) {
                return c;
            }
        }
        throw new IllegalArgumentException("Invalid filter operator: " + filterOperatorId);

    }
}
