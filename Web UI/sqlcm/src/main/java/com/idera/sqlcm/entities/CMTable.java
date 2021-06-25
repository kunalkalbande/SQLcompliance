package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.google.common.base.Joiner;
import com.google.common.base.Strings;
import com.idera.sqlcm.enumerations.NumbersOfRows;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import org.apache.commons.collections.CollectionUtils;

import java.util.ArrayList;
import java.util.List;

public class CMTable {

    @JsonProperty("id")
    private long id;

    @JsonProperty("objectId")
    private long objectId;

    @JsonProperty("databaseId")
    private long databaseId;

    @JsonProperty("serverId")
    private long serverId;

    @JsonProperty("objectType")
    private long objectType;

    @JsonProperty("tableName")
    private String tableName;

    @JsonProperty("fullTableName")
    private String fullTableName;

    @JsonProperty("schemaName")
    private String schemaName;

    @JsonProperty("rowLimit")
    private long rowLimit;

    @JsonProperty("selectedColumns")
    private boolean selectedColumns;

    @JsonProperty("columnList")
    private List<String> columnList = new ArrayList<>();

    @JsonProperty("type")
    private String type;

    @JsonProperty("columnId")
    private long columnId;

    public long getId() {
        return id;
    }

    public void setId(long id) {
        this.id = id;
    }

    public long getObjectId() {
        return objectId;
    }

    public void setObjectId(long objectId) {
        this.objectId = objectId;
    }

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

    public long getObjectType() {
        return objectType;
    }

    public void setObjectType(long objectType) {
        this.objectType = objectType;
    }

    public String getTableName() {
        return tableName;
    }

    public void setTableName(String tableName) {
        this.tableName = tableName;
    }

    public String getFullTableName() {
	if (Strings.isNullOrEmpty(fullTableName))
	    return tableName;
	return fullTableName;
    }

    public void setFullTableName(String fullTableName) {
        this.fullTableName = fullTableName;
    }

    public String getSchemaName() {
        return schemaName;
    }

    public void setSchemaName(String schemaName) {
        this.schemaName = schemaName;
    }

    public long getRowLimit() {
        return rowLimit;
    }

    public void setRowLimit(long rowLimit) {
        this.rowLimit = rowLimit;
    }

    public String getRowLimitString() {
        return NumbersOfRows.getByValue(rowLimit).getLabel();
    }

    public boolean isSelectedColumns() {
        return selectedColumns;
    }

    public void setSelectedColumns(boolean selectedColumns) {
        this.selectedColumns = selectedColumns;
    }

    public List<String> getColumnList() {
        return columnList;
    }

    public String getColumnListString() {
	if (!selectedColumns || CollectionUtils.isEmpty(columnList))
	    return ELFunctions
		    .getLabel(SQLCMI18NStrings.DB_PROPS_DIALOG_ALL_COLUMNS);
	return Joiner.on(',').join(columnList).toString();
    }

    public void setColumnList(List<String> columnList) {
        this.columnList = columnList;
    }

    public String getType() {
	return type;
    }

    public void setType(String type) {
	this.type = type;
    }

    public long getcolumnId() {
	return columnId;
    }

    public void setcolumnId(long columnId) {
	this.columnId = columnId;
    }

    @Override
    public boolean equals(Object o) {
	if (this == o)
	    return true;
	if (o == null || getClass() != o.getClass())
	    return false;

        CMTable cmTable = (CMTable) o;

	return !(fullTableName != null ? !fullTableName
		.equals(cmTable.fullTableName) : cmTable.fullTableName != null);

    }

    @Override
    public int hashCode() {
        return fullTableName != null ? fullTableName.hashCode() : 0;
    }
}