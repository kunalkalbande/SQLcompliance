package com.idera.sqlcm.entities;

import java.util.ArrayList;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMColumnDetails {

	@JsonProperty("DatabaseName")
    private String DatabaseName;
	
	
	@JsonProperty("TableName")
    private String TableName;

    @JsonProperty("FieldName")
    private String FieldName;

    @JsonProperty("DataType")
    private String DataType;

    @JsonProperty("MatchStr")
    private String MatchStr;
    
    private int size;


	public String getMatchStr() {
		return MatchStr;
	}

	public void setMatchStr(String matchStr) {
		MatchStr = matchStr;
	}

	public int getSize() {
		return size;
	}

	public void setSize(int size) {
		this.size = size;
	}

	public String getTableName() {
		return TableName;
	}

	public void setTableName(String tableName) {
		TableName = tableName;
	}

	public String getFieldName() {
		return FieldName;
	}

	public void setFieldName(String fieldName) {
		FieldName = fieldName;
	}

	public String getDataType() {
		return DataType;
	}

	public void setDataType(String dataType) {
		DataType = dataType;
	}
	
	public String getDatabaseName() {
		return DatabaseName;
	}

	public void setDatabaseName(String databaseName) {
		DatabaseName = databaseName;
	}

}
