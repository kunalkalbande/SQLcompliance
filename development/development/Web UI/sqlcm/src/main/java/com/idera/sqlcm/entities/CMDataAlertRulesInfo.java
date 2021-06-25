package com.idera.sqlcm.entities;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMDataAlertRulesInfo {

	@JsonProperty("sensitiveTable")
    private List<CMDataAlertTableInfo> sensitiveTable;

    @JsonProperty("sensitiveDatabase")
    private List<CMDataAlertDBInfo> sensitiveDatabase;
    
    @JsonProperty("sensitiveColumn")
    private List<CMDataAlertColumnInfo> sensitiveColumn;
        

    public List<CMDataAlertTableInfo> getSensitiveTable() {
		return sensitiveTable;
	}

	public void setSensitiveTable(List<CMDataAlertTableInfo> sensitiveTable) {
		this.sensitiveTable = sensitiveTable;
	}

	public List<CMDataAlertDBInfo> getSensitiveDatabase() {
		return sensitiveDatabase;
	}

	public void setSensitiveDatabase(List<CMDataAlertDBInfo> sensitiveDatabase) {
		this.sensitiveDatabase = sensitiveDatabase;
	}

	public List<CMDataAlertColumnInfo> getSensitiveColumn() {
		return sensitiveColumn;
	}

	public void setSensitiveColumn(List<CMDataAlertColumnInfo> sensitiveColumn) {
		this.sensitiveColumn = sensitiveColumn;
	}

	@Override
    public String toString() {
        return "CMDataAlertTableInfo{" +
            "sensitiveTable=" + sensitiveTable +
            ", sensitiveDatabase='" + sensitiveDatabase + '\'' +
            ", sensitiveColumn=" + sensitiveColumn +
        '}';
    }
}
