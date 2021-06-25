package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.ArrayList;
import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAttachArchive extends CMEntity {

    @JsonProperty("serverId")
    private Integer serverId;

    @JsonProperty("isEnabled")
    private Boolean isEnabled;

    @JsonProperty("sensitiveColumnTableList")
    private List<SensitiveColumnTableList> sensitiveColumnTableList = new ArrayList<SensitiveColumnTableList>();

    public Integer getServerId() {
        return serverId;
    }

    public void setServerId(Integer serverId) {
        this.serverId = serverId;
    }

    public Boolean getIsEnabled() {
        return isEnabled;
    }

    public void setIsEnabled(Boolean isEnabled) {
        this.isEnabled = isEnabled;
    }

    public List<SensitiveColumnTableList> getSensitiveColumnTableList() {
        return sensitiveColumnTableList;
    }

    public void setSensitiveColumnTableList(List<SensitiveColumnTableList> sensitiveColumnTableList) {
        this.sensitiveColumnTableList = sensitiveColumnTableList;
    }

    @Override
    public String toString() {
        return "CMAttachArchive{" +
            "serverId=" + serverId +
            ", isEnabled=" + isEnabled +
            ", sensitiveColumnTableList=" + sensitiveColumnTableList +
            '}';
    }

    public class SensitiveColumnTableList {

        @JsonProperty("id")
        private Integer id;

        @JsonProperty("objectId")
        private Integer objectId;

        @JsonProperty("databaseId")
        private Integer databaseId;

        @JsonProperty("serverId")
        private Integer serverId;

        @JsonProperty("objectType")
        private Integer objectType;

        @JsonProperty("tableName")
        private String tableName;

        @JsonProperty("fullTableName")
        private String fullTableName;

        @JsonProperty("schemaName")
        private String schemaName;

        @JsonProperty("rowLimit")
        private Integer rowLimit;

        @JsonProperty("selectedColumns")
        private Boolean selectedColumns;

        @JsonProperty("columnList")
        private List<String> columnList = new ArrayList<String>();

        public Integer getId() {
            return id;
        }

        public void setId(Integer id) {
            this.id = id;
        }

        public Integer getObjectId() {
            return objectId;
        }

        public void setObjectId(Integer objectId) {
            this.objectId = objectId;
        }

        public Integer getDatabaseId() {
            return databaseId;
        }

        public void setDatabaseId(Integer databaseId) {
            this.databaseId = databaseId;
        }

        public Integer getServerId() {
            return serverId;
        }

        public void setServerId(Integer serverId) {
            this.serverId = serverId;
        }

        public Integer getObjectType() {
            return objectType;
        }

        public void setObjectType(Integer objectType) {
            this.objectType = objectType;
        }

        public String getTableName() {
            return tableName;
        }

        public void setTableName(String tableName) {
            this.tableName = tableName;
        }

        public String getFullTableName() {
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

        public Integer getRowLimit() {
            return rowLimit;
        }

        public void setRowLimit(Integer rowLimit) {
            this.rowLimit = rowLimit;
        }

        public Boolean getSelectedColumns() {
            return selectedColumns;
        }

        public void setSelectedColumns(Boolean selectedColumns) {
            this.selectedColumns = selectedColumns;
        }

        public List<String> getColumnList() {
            return columnList;
        }

        public void setColumnList(List<String> columnList) {
            this.columnList = columnList;
        }

        @Override
        public String toString() {
            return "SensitiveColumnTableList{" +
                "id=" + id +
                ", objectId=" + objectId +
                ", databaseId=" + databaseId +
                ", serverId=" + serverId +
                ", objectType=" + objectType +
                ", tableName='" + tableName + '\'' +
                ", fullTableName='" + fullTableName + '\'' +
                ", schemaName='" + schemaName + '\'' +
                ", rowLimit=" + rowLimit +
                ", selectedColumns=" + selectedColumns +
                ", columnList=" + columnList +
                '}';
        }
    }
}
