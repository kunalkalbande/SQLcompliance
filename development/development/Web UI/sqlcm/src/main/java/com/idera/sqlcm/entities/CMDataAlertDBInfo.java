package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMDataAlertDBInfo {
	   @JsonProperty("dbId")
	    private int dbId;

		@JsonProperty("name")
	    private String name;
		
		@JsonProperty("srvId")
	    private int srvId;
		
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

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}
	
	@Override
    public String toString() {
        return "CMDataAlertDBInfo{" +
            "srvId=" + srvId +
            ",dbId=" + dbId +
            ", name='" + name + '\'' +
            '}';
	}
}
