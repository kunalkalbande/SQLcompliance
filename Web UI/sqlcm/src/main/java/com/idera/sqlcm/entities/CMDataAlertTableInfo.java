package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMDataAlertTableInfo {
	@JsonProperty("srvId")
    private int srvId;

	@JsonProperty("dbId")
    private int dbId;

    @JsonProperty("objectId")
    private int objectId;
    
    @JsonProperty("schemaName")
    private String schemaName;

	@JsonProperty("name")
    private String name;
    
    @JsonProperty("selectedColumn")
    private int selectedColumn;
    
    public int getSrvId() {
		return srvId;
	}

	public void setSrvId(int srvId) {
		this.srvId = srvId;
	}

	public int getDbId() {
		return dbId;
	}

	public void setDbId(int dbId) {
		this.dbId = dbId;
	}

	public int getObjectId() {
		return objectId;
	}

	public void setObjectId(int objectId) {
		this.objectId = objectId;
	}

	public int getSelectedColumn() {
		return selectedColumn;
	}

	public void setSelectedColumn(int selectedColumn) {
		this.selectedColumn = selectedColumn;
	}
	
	public String getSchemaName() {
		return schemaName;
	}

	public void setSchemaName(String schemaName) {
		this.schemaName = schemaName;
	}
	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}    

    @Override
    public String toString() {
        return "CMDataAlertTableInfo{" +
            "srvId=" + srvId +
            ", dbId='" + dbId + '\'' +
            ", objectId=" + objectId +
            ", schemaName=" + schemaName +
            ", name=" + name +
            ", selectedColumn=" + selectedColumn +
            '}';
    }
}
