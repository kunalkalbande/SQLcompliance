package com.idera.sqlcm.ui.components.filter.model;

public enum  FilterType {
    TEXT(FilterInputType.TEXT, FilterOperator.LIKE, "text-icon"),
    DIGIT(FilterInputType.DIGIT, FilterOperator.LIKE, "digit-icon"),
    DATE(FilterInputType.DATE, FilterOperator.EQUALS, "duration-icon"),
    OPTIONS(FilterInputType.OPTIONS, FilterOperator.LIKE, "option-icon"),
    INSTANCE(FilterInputType.TEXT, FilterOperator.LIKE, "instance-icon"),
    DATABASE(FilterInputType.TEXT, FilterOperator.LIKE, "database-icon"),
    VERSION(FilterInputType.OPTIONS, FilterOperator.LIKE, "digit-icon"),
    DIGIT_RANGE(FilterInputType.DIGIT_RANGE, FilterOperator.RANGE, "digit-icon"),
    DATE_RANGE(FilterInputType.DATE_RANGE, FilterOperator.RANGE, "duration-icon"),
    TIME_RANGE(FilterInputType.TIME_RANGE, FilterOperator.RANGE, "duration-icon"),
    DATE_RANGE_WITH_ERROR(FilterInputType.DATE_RANGE, FilterOperator.RANGE, "duration-with-error"),
    COMBO(FilterInputType.COMBO, FilterOperator.LIKE,"text-icon");

    private final FilterInputType inputType;
    private final FilterOperator operator;
    private final String iconUrl;

    private FilterType(final FilterInputType inputType, final FilterOperator operator, final String iconUrl) {
        this.inputType = inputType;
        this.operator = operator;
        this.iconUrl = iconUrl;
    }

    public FilterInputType getInputType() {
        return inputType;
    }

    public FilterOperator getOperator() {
        return operator;
    }

    public String getIconUrl() {
        return iconUrl;
    }

    @Override
    public String toString() {
        return inputType.toString();
    }
}
