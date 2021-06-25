package com.idera.sqlcm.entities.database.properties;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Objects;

import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMSensitiveColumnTableData {

    @JsonProperty("columnsSupported")
    private boolean columnsSupported;

    @JsonProperty("statusMessaage")
    private String statusMessage;

    @JsonProperty("missingTableStatusMessage")
    private String missingTableStatusMessage;

    @JsonProperty("sensitiveTableColumnDictionary")
    private List<CMStringCMTableEntity> sensitiveTableColumnDictionary;

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

    public List<CMStringCMTableEntity> getSensitiveTableColumnDictionary() {
        return sensitiveTableColumnDictionary;
    }

    public void setSensitiveTableColumnDictionary(List<CMStringCMTableEntity> sensitiveTableColumnDictionary) {
        this.sensitiveTableColumnDictionary = sensitiveTableColumnDictionary;
    }

    @Override
    public String toString() {
        return Objects.toStringHelper(this)
                      .add("columnsSupported", columnsSupported)
                      .add("statusMessage", statusMessage)
                      .add("missingTableStatusMessage", missingTableStatusMessage)
                      .add("sensitiveTableColumnDictionary", sensitiveTableColumnDictionary)
                      .toString();
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        CMSensitiveColumnTableData that = (CMSensitiveColumnTableData) o;
        return Objects.equal(this.columnsSupported, that.columnsSupported) &&
                Objects.equal(this.statusMessage, that.statusMessage) &&
                Objects.equal(this.missingTableStatusMessage, that.missingTableStatusMessage) &&
                Objects.equal(this.sensitiveTableColumnDictionary, that.sensitiveTableColumnDictionary);
    }

    @Override
    public int hashCode() {
        return Objects.hashCode(columnsSupported, statusMessage, missingTableStatusMessage, sensitiveTableColumnDictionary);
    }
}
