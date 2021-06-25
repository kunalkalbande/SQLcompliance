package com.idera.sqlcm.enumerations;

public enum NumbersOfRows {
    ALL(-1, "All", 0),
    R1000(1000, "1000", 1),
    R100(100, "100", 2),
    R10(10, "10", 3),
    R1(1, "1", 4),
    R0(0, "0", 5);

    private long value;
    private String label;
    private int index;

    NumbersOfRows(long value, String label, int index) {
        this.value = value;
        this.label = label;
        this.index = index;
    }

    public long getValue() {
        return value;
    }

    public String getLabel() {
        return label;
    }

    public int getIndex() {
        return index;
    }

    public static NumbersOfRows getByIndex(int index) {
        for (NumbersOfRows numbersOfRows : NumbersOfRows.values()) {
            if (numbersOfRows.getIndex() == index) {
                return numbersOfRows;
            }
        }
        return NumbersOfRows.R10;
    }

    public static NumbersOfRows getByValue(long value) {
        for (NumbersOfRows numbersOfRows : NumbersOfRows.values()) {
            if (numbersOfRows.getValue() == value) {
                return numbersOfRows;
            }
        }
        return NumbersOfRows.R10;
    }
}
