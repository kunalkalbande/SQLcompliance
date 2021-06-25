package com.idera.sqlcm.entities;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.database.properties.CMSensitiveColumnTableData;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMDatabase extends CMEntity {
    
    @JsonProperty("databaseId")
    private long databaseId;

    @JsonProperty("serverId")
    private long serverId;

    @JsonProperty("isEnabled")
    private Boolean isEnabled;

    @JsonProperty("sensitiveTableColumnData")
    private CMSensitiveColumnTableData sensitiveColumnTableData;

    @JsonProperty("beforeAfterTableList")
    private List<CMTable> beforeAfterTableData;

	public long getDatabaseId() {
        return databaseId;
    }

    public void setDatabaseId(long databaseId) {
        this.databaseId = databaseId;
    }

    public long getServerId() {
        return serverId;
    }

    public void setServerId(long serverId) {
        this.serverId = serverId;
    }

    public boolean isEnabled() {
        return isEnabled;
    }

    public void setEnabled(Boolean isEnabled) {
        this.isEnabled = isEnabled;
    }

    public CMSensitiveColumnTableData getSensitiveColumnTableData() {
	return sensitiveColumnTableData;
    }

    public void setSensitiveColumnTableData(
	    CMSensitiveColumnTableData sensitiveColumnTableData) {
	this.sensitiveColumnTableData = sensitiveColumnTableData;
    }
    
    public List<CMTable> getBeforeAfterTableData() {
		return beforeAfterTableData;
	}

	public void setBeforeAfterTableData(List<CMTable> beforeAfterTableData) {
		this.beforeAfterTableData = beforeAfterTableData;
	}
}

