package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.ArrayList;

public class CMEPBeforeAfterData {

    @JsonProperty("beforeAfterValueList")
    private ArrayList<CMEPBeforeAfterValue> beforeAfterValueList;

    @JsonProperty("columnsAffectedStatusMessage")
    private String columnsAffectedStatusMessage;

    @JsonProperty("isAvailable")
    private boolean available;

    @JsonProperty("rowsAffectedStatusMessage")
    private String rowsAffectedStatusMessage;

    @JsonProperty("statusMessage")
    private String statusMessage;

    public CMEPBeforeAfterData() {
    }

    public ArrayList<CMEPBeforeAfterValue> getBeforeAfterValueList() {
        return beforeAfterValueList;
    }

    public void setBeforeAfterValueList(ArrayList<CMEPBeforeAfterValue> beforeAfterValueList) {
        this.beforeAfterValueList = beforeAfterValueList;
    }

    public String getColumnsAffectedStatusMessage() {
        return columnsAffectedStatusMessage;
    }

    public void setColumnsAffectedStatusMessage(String columnsAffectedStatusMessage) {
        this.columnsAffectedStatusMessage = columnsAffectedStatusMessage;
    }

    public boolean isAvailable() {
        return available;
    }

    public void setAvailable(boolean available) {
        this.available = available;
    }

    public String getRowsAffectedStatusMessage() {
        return rowsAffectedStatusMessage;
    }

    public void setRowsAffectedStatusMessage(String rowsAffectedStatusMessage) {
        this.rowsAffectedStatusMessage = rowsAffectedStatusMessage;
    }

    public String getStatusMessage() {
        return statusMessage;
    }

    public void setStatusMessage(String statusMessage) {
        this.statusMessage = statusMessage;
    }

    @Override
    public String toString() {
        return "CMEPBeforeAfterData{" +
            "beforeAfterValueList=" + beforeAfterValueList +
            ", columnsAffectedStatusMessage='" + columnsAffectedStatusMessage + '\'' +
            ", available=" + available +
            ", rowsAffectedStatusMessage='" + rowsAffectedStatusMessage + '\'' +
            ", statusMessage='" + statusMessage + '\'' +
            '}';
    }
}
