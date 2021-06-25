package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMDataAlertColumnInfo {
	
	@JsonProperty("srvId")
    private int srvId;

	@JsonProperty("dbId")
    private int dbId;

    @JsonProperty("objectId")
    private int objectId;

    @JsonProperty("name")
    private String name;

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

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}
	

    @Override
    public String toString() {
        return "CMDataAlertColumnInfo{" +
            "srvId=" + srvId +
            ", dbId='" + dbId + '\'' +
            ", objectId=" + objectId +
            ", name=" + name +
            '}';
    }
   
}
