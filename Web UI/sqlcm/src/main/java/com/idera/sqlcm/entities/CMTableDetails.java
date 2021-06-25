package com.idera.sqlcm.entities;

import java.text.DecimalFormat;
import java.text.NumberFormat;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMTableDetails {
	@JsonProperty("DatabaseName")
    private String DatabaseName;

    @JsonProperty("SchemaTableName")
    private String SchemaTableName;

    @JsonProperty("Size")
    private double Size;

    @JsonProperty("SizeString")
    private String SizeString;

    @JsonProperty("RowCount")
    private int RowCount;

    @JsonProperty("ColumnsIdentified")
    private int ColumnsIdentified;

	public String getDatabaseName() {
		return DatabaseName;
	}

	public void setDatabaseName(String databaseName) {
		DatabaseName = databaseName;
	}

	public String getSchemaTableName() {
		return SchemaTableName;
	}

	public void setSchemaTableName(String schemaTableName) {
		SchemaTableName = schemaTableName;
	}

	public double getSize() {
		NumberFormat formatter = new DecimalFormat("#0.00");
		return Double.parseDouble(formatter.format(Size));
	}

	public void setSize(double size) {
		NumberFormat formatter = new DecimalFormat("#0.00");
		String decimalSize =formatter.format(size);
		Size=Double.parseDouble(decimalSize);
	}

	public String getSizeString() {
		return SizeString;
	}

	public void setSizeString(String sizeString) {
		SizeString = sizeString;
	}

	public int getRowCount() {
		return RowCount;
	}

	public void setRowCount(int rowCount) {
		RowCount = rowCount;
	}

	public int getColumnsIdentified() {
		return ColumnsIdentified;
	}

	public void setColumnsIdentified(int columnsIdentified) {
		ColumnsIdentified = columnsIdentified;
	}

}
