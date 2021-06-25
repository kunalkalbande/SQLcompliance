package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CMEventDetails extends CMEvent {

    @JsonProperty("categoryId")
    private long categoryId;

    @JsonProperty("eventTypeId")
    private long eventTypeId;

    @JsonProperty("targetObject")
    private String targetObject;

    @JsonProperty("details")
    private String details;

    @JsonProperty("accessCheck")
    private int accessCheck;

    @JsonProperty("afterValue")
    private String afterValue;

    @JsonProperty("application")
    private String application;

    @JsonProperty("auditedUpdates")
    private int auditedUpdates;

    @JsonProperty("beforeValue")
    private String beforeValue;

    @JsonProperty("column")
    private String column;

    @JsonProperty("columnsUpdated")
    private int columnsUpdated;

    @JsonProperty("databaseUser")
    private String databaseUser;

    @JsonProperty("host")
    private String host;

    @JsonProperty("object")
    private String object;

    @JsonProperty("owner")
    private String owner;

    @JsonProperty("primaryKey")
    private String primaryKey;

    @JsonProperty("privilegedUser")
    private boolean privilegedUser;

    @JsonProperty("role")
    private String role;

    @JsonProperty("schema")
    private String schema;

    @JsonProperty("server")
    private String server;

    @JsonProperty("sessionLogin")
    private String sessionLogin;

    @JsonProperty("spid")
    private int spid;

    @JsonProperty("table")
    private String table;

    @JsonProperty("targetLogin")
    private String targetLogin;

    @JsonProperty("targetUser")
    private String targetUser;

    public CMEventDetails() {
    }

    public long getCategoryId() {
        return categoryId;
    }

    public void setCategoryId(long categoryId) {
        this.categoryId = categoryId;
    }

    public long getEventTypeId() {
        return eventTypeId;
    }

    public void setEventTypeId(long eventTypeId) {
        this.eventTypeId = eventTypeId;
    }

    public String getTargetObject() {
        return targetObject;
    }

    public void setTargetObject(String targetObject) {
        this.targetObject = targetObject;
    }

    public String getDetails() {
        return details;
    }

    public void setDetails(String details) {
        this.details = details;
    }

    public int getAccessCheck() {
        return accessCheck;
    }

    public void setAccessCheck(int accessCheck) {
        this.accessCheck = accessCheck;
    }

    public String getAfterValue() {
        return afterValue;
    }

    public void setAfterValue(String afterValue) {
        this.afterValue = afterValue;
    }

    public String getApplication() {
        return application;
    }

    public void setApplication(String application) {
        this.application = application;
    }

    public int getAuditedUpdates() {
        return auditedUpdates;
    }

    public void setAuditedUpdates(int auditedUpdates) {
        this.auditedUpdates = auditedUpdates;
    }

    public String getBeforeValue() {
        return beforeValue;
    }

    public void setBeforeValue(String beforeValue) {
        this.beforeValue = beforeValue;
    }

    public String getColumn() {
        return column;
    }

    public void setColumn(String column) {
        this.column = column;
    }

    public int getColumnsUpdated() {
        return columnsUpdated;
    }

    public void setColumnsUpdated(int columnsUpdated) {
        this.columnsUpdated = columnsUpdated;
    }

    public String getDatabaseUser() {
        return databaseUser;
    }

    public void setDatabaseUser(String databaseUser) {
        this.databaseUser = databaseUser;
    }

    public String getHost() {
        return host;
    }

    public void setHost(String host) {
        this.host = host;
    }

    public String getObject() {
        return object;
    }

    public void setObject(String object) {
        this.object = object;
    }

    public String getOwner() {
        return owner;
    }

    public void setOwner(String owner) {
        this.owner = owner;
    }

    public String getPrimaryKey() {
        return primaryKey;
    }

    public void setPrimaryKey(String primaryKey) {
        this.primaryKey = primaryKey;
    }

    public boolean getPrivilegedUser() {
        return privilegedUser;
    }

    public void setPrivilegedUser(boolean privilegedUser) {
        this.privilegedUser = privilegedUser;
    }

    public String getRole() {
        return role;
    }

    public void setRole(String role) {
        this.role = role;
    }

    public String getSchema() {
        return schema;
    }

    public void setSchema(String schema) {
        this.schema = schema;
    }

    public String getServer() {
        return server;
    }

    public void setServer(String server) {
        this.server = server;
    }

    public String getSessionLogin() {
        return sessionLogin;
    }

    public void setSessionLogin(String sessionLogin) {
        this.sessionLogin = sessionLogin;
    }

    public int getSpid() {
        return spid;
    }

    public void setSpid(int spid) {
        this.spid = spid;
    }

    public String getTable() {
        return table;
    }

    public void setTable(String table) {
        this.table = table;
    }

    public String getTargetLogin() {
        return targetLogin;
    }

    public void setTargetLogin(String targetLogin) {
        this.targetLogin = targetLogin;
    }

    public String getTargetUser() {
        return targetUser;
    }

    public void setTargetUser(String targetUser) {
        this.targetUser = targetUser;
    }

    public int getHashCode(){
        return hashCode();
    }
    @Override
    public String toString() {
        return "CMEventDetails{" +
            "categoryId=" + categoryId +
            ", eventTypeId=" + eventTypeId +
            ", targetObject='" + targetObject + '\'' +
            ", details='" + details + '\'' +
            ", accessCheck='" + accessCheck + '\'' +
            ", afterValue='" + afterValue + '\'' +
            ", application='" + application + '\'' +
            ", auditedUpdates='" + auditedUpdates + '\'' +
            ", beforeValue='" + beforeValue + '\'' +
            ", column='" + column + '\'' +
            ", columnsUpdated='" + columnsUpdated + '\'' +
            ", databaseUser='" + databaseUser + '\'' +
            ", host='" + host + '\'' +
            ", object='" + object + '\'' +
            ", owner='" + owner + '\'' +
            ", primaryKey='" + primaryKey + '\'' +
            ", privilegedUser='" + privilegedUser + '\'' +
            ", role='" + role + '\'' +
            ", server='" + server + '\'' +
            ", sessionLogin='" + sessionLogin + '\'' +
            ", spid='" + spid + '\'' +
            ", table='" + table + '\'' +
            ", targetLogin='" + targetLogin + '\'' +
            ", targetUser='" + targetUser + '\'' +
            '}';
    }
}
