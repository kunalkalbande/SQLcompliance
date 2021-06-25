/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.ui.importAuditSetting;

import com.fasterxml.jackson.annotation.JsonProperty;

public class DatabaseDetails {
	
	@JsonProperty("dbId")
	int dbId;
	
	@JsonProperty("srvId")
	int srvId;
	
	@JsonProperty("dbName")
	String dbName;

	public int getDbId() {
		return dbId;
	}

	public void setDbId(int dbId) {
		this.dbId = dbId;
	}

	public int getSrvId() {
		return srvId;
	}

	public void setSrvId(int srvId) {
		this.srvId = srvId;
	}

	public String getDbName() {
		return dbName;
	}

	public void setDbName(String dbName) {
		this.dbName = dbName;
	}
}

/***End SQLCm 5.4***/
