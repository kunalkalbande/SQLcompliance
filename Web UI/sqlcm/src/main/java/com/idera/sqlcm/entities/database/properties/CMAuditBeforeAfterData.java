package com.idera.sqlcm.entities.database.properties;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Objects;

import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAuditBeforeAfterData {

    @JsonProperty("isAvailable")
    private boolean available;

    @JsonProperty("columnsSupported")
    private boolean columnsSupported;

    @JsonProperty("statusMessaage")
    private String statusMessage;

    @JsonProperty("missingTableStatusMessage")
    private String missingTableStatusMessage;

    @JsonProperty("clrStatus")
    private CMCLRStatus clrStatus;

    @JsonProperty("beforeAfterTableColumnDictionary")
    private List<CMStringCMTableEntity> beforeAfterTableColumnDictionary;

    public boolean isAvailable() {
        return available;
    }

    public void setAvailable(boolean available) {
        this.available = available;
    }

    public boolean isColumnsSupported() {
        return columnsSupported;
    }

    public void setColumnsSupported(boolean columnsSupported) {
        this.columnsSupported = columnsSupported;
    }

    public String getStatusMessage() {
        return statusMessage;
    }

    public void setStatusMessage(String statusMessage) {
        this.statusMessage = statusMessage;
    }

    public String getMissingTableStatusMessage() {
        return missingTableStatusMessage;
    }

    public void setMissingTableStatusMessage(String missingTableStatusMessage) {
        this.missingTableStatusMessage = missingTableStatusMessage;
    }

    public CMCLRStatus getClrStatus() {
        return clrStatus;
    }

    public void setClrStatus(CMCLRStatus clrStatus) {
        this.clrStatus = clrStatus;
    }

    public List<CMStringCMTableEntity> getBeforeAfterTableColumnDictionary() {
        return beforeAfterTableColumnDictionary;
    }

    public void setBeforeAfterTableColumnDictionary(List<CMStringCMTableEntity> beforeAfterTableColumnDictionary) {
        this.beforeAfterTableColumnDictionary = beforeAfterTableColumnDictionary;
    }

    @Override
    public String toString() {
        return Objects.toStringHelper(this)
                      .add("available", available)
                      .add("columnsSupported", columnsSupported)
                      .add("statusMessage", statusMessage)
                      .add("missingTableStatusMessage", missingTableStatusMessage)
                      .add("clrStatus", clrStatus)
                      .add("beforeAfterTableColumnDictionary", beforeAfterTableColumnDictionary)
                      .toString();
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        CMAuditBeforeAfterData that = (CMAuditBeforeAfterData) o;
        return Objects.equal(this.available, that.available) &&
                Objects.equal(this.columnsSupported, that.columnsSupported) &&
                Objects.equal(this.statusMessage, that.statusMessage) &&
                Objects.equal(this.missingTableStatusMessage, that.missingTableStatusMessage) &&
                Objects.equal(this.clrStatus, that.clrStatus) &&
                Objects.equal(this.beforeAfterTableColumnDictionary, that.beforeAfterTableColumnDictionary);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(available, columnsSupported, statusMessage, missingTableStatusMessage, clrStatus, beforeAfterTableColumnDictionary);
    }
}
