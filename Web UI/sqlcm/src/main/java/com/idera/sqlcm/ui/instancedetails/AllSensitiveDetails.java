//Start SQLCm-5.4 
//Requirement - 4.1.3.1. 


package com.idera.sqlcm.ui.instancedetails;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.CMDataAlertColumnInfo;
import com.idera.sqlcm.entities.CMDataAlertDBInfo;
import com.idera.sqlcm.entities.CMDataAlertTableInfo;

public class AllSensitiveDetails {

	@JsonProperty("sensitiveTable")
    private List<TableDetails> sensitiveTable;

    @JsonProperty("sensitiveDatabase")
    private List<DatabaseDetails> sensitiveDatabase;
    
    @JsonProperty("sensitiveColumn")
    private List<ColumnDetails> sensitiveColumn;
        
    @JsonProperty("validFile")
    private boolean validFile;

    public boolean isValidFile() {
		return validFile;
	}

	public void setValidFile(boolean validFile) {
		this.validFile = validFile;
	}

	public List<TableDetails> getSensitiveTable() {
		return sensitiveTable;
	}

	public void setSensitiveTable(List<TableDetails> sensitiveTable) {
		this.sensitiveTable = sensitiveTable;
	}

	public List<DatabaseDetails> getSensitiveDatabase() {
		return sensitiveDatabase;
	}

	public void setSensitiveDatabase(List<DatabaseDetails> sensitiveDatabase) {
		this.sensitiveDatabase = sensitiveDatabase;
	}

	public List<ColumnDetails> getSensitiveColumn() {
		return sensitiveColumn;
	}

	public void setSensitiveColumn(List<ColumnDetails> sensitiveColumn) {
		this.sensitiveColumn = sensitiveColumn;
	}
}

//End - SQLCm-5.4
