package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMEPBeforeAfterValue {

    @JsonProperty("rowNumber")
    private long rowNumber;

    @JsonProperty("primaryKey")
    private String primaryKey;

    @JsonProperty("column")
    private String column;

    @JsonProperty("beforeValue")
    private String beforeValue;

    @JsonProperty("afterValue")
    private String afterValue;

    public CMEPBeforeAfterValue() {
    }

    public long getRowNumber() {
        return rowNumber;
    }

    public void setRowNumber(long rowNumber) {
        this.rowNumber = rowNumber;
    }

    public String getPrimaryKey() {
        return primaryKey;
    }

    public void setPrimaryKey(String primaryKey) {
        this.primaryKey = primaryKey;
    }

    public String getColumn() {
        return column;
    }

    public void setColumn(String column) {
        this.column = column;
    }

    public String getBeforeValue() {
        return beforeValue;
    }

    public void setBeforeValue(String beforeValue) {
        this.beforeValue = beforeValue;
    }

    public String getAfterValue() {
        return afterValue;
    }

    public void setAfterValue(String afterValue) {
        this.afterValue = afterValue;
    }

    @Override
    public String toString() {
        return "CMEPBeforeAfterValue{" +
            "rowNumber=" + rowNumber +
            ", primaryKey='" + primaryKey + '\'' +
            ", column='" + column + '\'' +
            ", beforeValue='" + beforeValue + '\'' +
            ", afterValue='" + afterValue + '\'' +
            '}';
    }
}
