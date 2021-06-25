package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.ArrayList;
import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMEventProperties extends CMEntity {

    @JsonProperty("eventId")
    private long eventId;

    @JsonProperty("eventType")
    private long eventType;

    @JsonProperty("eventCategory")
    private long eventCategory;

    @JsonProperty("targetObject")
    private String targetObject;

    @JsonProperty("details")
    private String details;

    @JsonProperty("hash")
    private String hash;

    @JsonProperty("eventClass")
    private long eventClass;

    @JsonProperty("eventSubclass")
    private long eventSubclass;

    @JsonProperty("startTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date startTime;

    @JsonProperty("spid")
    private long spid;

    @JsonProperty("applicationName")
    private String applicationName;

    @JsonProperty("hostName")
    private String hostName;

    @JsonProperty("serverName")
    private String serverName;

    @JsonProperty("loginName")
    private String loginName;

    @JsonProperty("success")
    private long success;

    private String accessCheck;

    @JsonProperty("databaseName")
    private String databaseName;

    @JsonProperty("databaseId")
    private long databaseId;

    @JsonProperty("databaseUserName")
    private String databaseUserName;

    @JsonProperty("objectType")
    private long objectType;

    @JsonProperty("objectName")
    private String objectName;

    @JsonProperty("objectId")
    private long objectId;

    @JsonProperty("permissions")
    private long permissions;

    @JsonProperty("columnPermissions")
    private long columnPermissions;

    @JsonProperty("targetLoginName")
    private String targetLoginName;

    @JsonProperty("targetUserName")
    private String targetUserName;

    @JsonProperty("roleName")
    private String roleName;

    @JsonProperty("ownerName")
    private String ownerName;

    @JsonProperty("alertLevel")
    private long alertLevel;

    @JsonProperty("checkSum")
    private String checkSum;

    @JsonProperty("privilegedUser")
    private boolean privilegedUser;

    @JsonProperty("category")
    private String category;

    @JsonProperty("fileName")
    private String fileName;

    @JsonProperty("linkedServerName")
    private String linkedServerName;

    @JsonProperty("parentName")
    private String parentName;

    @JsonProperty("isSystem")
    private boolean isSystem;

    @JsonProperty("sessionLoginName")
    private String sessionLoginName;

    @JsonProperty("providerName")
    private String providerName;

    @JsonProperty("appNameId")
    private String appNameId;

    @JsonProperty("hostId")
    private String hostId;

    @JsonProperty("loginId")
    private String loginId;
	
    // Added for row counts
	
    @JsonProperty("rowCounts")
    private Long rowCounts=null;


    @JsonProperty("endTime")
    @JsonDeserialize(using = DataContractDateDeserializer.class)
    @JsonSerialize(using = DataContractUtcDateSerializer.class)
    private Date endTime;

    @JsonProperty("startSequence")
    private long startSequence;

    @JsonProperty("endSequence")
    private long endSequence;

    @JsonProperty("sensitiveColumns")
    private long sensitiveColumns;

    @JsonProperty("sqlStatement")
    private String sqlStatement;

    @JsonProperty("columnsAffected")
    private String columnsAffected;

    @JsonProperty("rowsAffected")
    private long rowsAffected;

    @JsonProperty("sqlVersion")
    private long sqlVersion;

    @JsonProperty("sensitiveColumnList")
    private ArrayList<String> sensitiveColumnList;

    @JsonProperty("beforeAfterData")
    private CMEPBeforeAfterData beforeAfterData;

    public Long getRowCounts() {
		return rowCounts;
	}

	public void setRowCounts(Long rowCounts) {
		this.rowCounts = rowCounts;
	}
    
    public CMEventProperties() {
    }

    public long getEventId() {
        return eventId;
    }

    public void setEventId(int eventId) {
        this.eventId = eventId;
    }

    public long getEventType() {
        return eventType;
    }

    public void setEventType(long eventType) {
        this.eventType = eventType;
    }

    public long getEventCategory() {
        return eventCategory;
    }

    public void setEventCategory(long eventCategory) {
        this.eventCategory = eventCategory;
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

    public String getHash() {
        return hash;
    }

    public void setHash(String hash) {
        this.hash = hash;
    }

    public long getEventClass() {
        return eventClass;
    }

    public void setEventClass(long eventClass) {
        this.eventClass = eventClass;
    }

    public long getEventSubclass() {
        return eventSubclass;
    }

    public void setEventSubclass(long eventSubclass) {
        this.eventSubclass = eventSubclass;
    }

    public Date getStartTime() {
        return startTime;
    }

    public void setStartTime(Date startTime) {
        this.startTime = startTime;
    }

    public long getSpid() {
        return spid;
    }

    public void setSpid(long spid) {
        this.spid = spid;
    }

    public String getApplicationName() {
        return applicationName;
    }

    public void setApplicationName(String applicationName) {
        this.applicationName = applicationName;
    }

    public String getHostName() {
        return hostName;
    }

    public void setHostName(String hostName) {
        this.hostName = hostName;
    }

    public String getServerName() {
        return serverName;
    }

    public void setServerName(String serverName) {
        this.serverName = serverName;
    }

    public String getLoginName() {
        return loginName;
    }

    public void setLoginName(String loginName) {
        this.loginName = loginName;
    }

    public long getSuccess() {
        return success;
    }

    public void setSuccess(long success) {
        this.success = success;
        this.accessCheck = (success == 0)
            ? ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_ACCESS_CHECK_FAILED)
            : ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_ACCESS_CHECK_PASSED);
    }

    public String getAccessCheck() {
        return accessCheck;
    }

    public String getDatabaseName() {
        return databaseName;
    }

    public void setDatabaseName(String databaseName) {
        this.databaseName = databaseName;
    }

    public long getDatabaseId() {
        return databaseId;
    }

    public void setDatabaseId(long databaseId) {
        this.databaseId = databaseId;
    }

    public String getDatabaseUserName() {
        return databaseUserName;
    }

    public void setDatabaseUserName(String databaseUserName) {
        this.databaseUserName = databaseUserName;
    }

    public long getObjectType() {
        return objectType;
    }

    public void setObjectType(long objectType) {
        this.objectType = objectType;
    }

    public String getObjectName() {
        return objectName;
    }

    public void setObjectName(String objectName) {
        this.objectName = objectName;
    }

    public long getObjectId() {
        return objectId;
    }

    public void setObjectId(long objectId) {
        this.objectId = objectId;
    }

    public long getPermissions() {
        return permissions;
    }

    public void setPermissions(long permissions) {
        this.permissions = permissions;
    }

    public long getColumnPermissions() {
        return columnPermissions;
    }

    public void setColumnPermissions(long columnPermissions) {
        this.columnPermissions = columnPermissions;
    }

    public String getTargetLoginName() {
        return targetLoginName;
    }

    public void setTargetLoginName(String targetLoginName) {
        this.targetLoginName = targetLoginName;
    }

    public String getTargetUserName() {
        return targetUserName;
    }

    public void setTargetUserName(String targetUserName) {
        this.targetUserName = targetUserName;
    }

    public String getRoleName() {
        return roleName;
    }

    public void setRoleName(String roleName) {
        this.roleName = roleName;
    }

    public String getOwnerName() {
        return ownerName;
    }

    public void setOwnerName(String ownerName) {
        this.ownerName = ownerName;
    }

    public long getAlertLevel() {
        return alertLevel;
    }
    public void setAlertLevel(long alertLevel) {
        this.alertLevel = alertLevel;
    }

    public String getCheckSum() {
        return checkSum;
    }

    public void setCheckSum(String checkSum) {
        this.checkSum = checkSum;
    }

    public boolean getPrivilegedUser() {
        return privilegedUser;
    }

    public void setPrivilegedUser(boolean privilegedUser) {
        this.privilegedUser = privilegedUser;
    }

    public String getCategory() {
        return category;
    }

    public void setCategory(String category) {
        this.category = category;
    }

    public String getFileName() {
        return fileName;
    }

    public void setFileName(String fileName) {
        this.fileName = fileName;
    }

    public String getLinkedServerName() {
        return linkedServerName;
    }

    public void setLinkedServerName(String linkedServerName) {
        this.linkedServerName = linkedServerName;
    }

    public String getParentName() {
        return parentName;
    }

    public void setParentName(String parentName) {
        this.parentName = parentName;
    }

    public boolean getIsSystem() {
        return isSystem;
    }

    public void setIsSystem(boolean isSystem) {
        this.isSystem = isSystem;
    }

    public String getSessionLoginName() {
        return sessionLoginName;
    }

    public void setSessionLoginName(String sessionLoginName) {
        this.sessionLoginName = sessionLoginName;
    }

    public String getProviderName() {
        return providerName;
    }

    public void setProviderName(String providerName) {
        this.providerName = providerName;
    }

    public String getAppNameId() {
        return appNameId;
    }

    public void setAppNameId(String appNameId) {
        this.appNameId = appNameId;
    }

    public String getHostId() {
        return hostId;
    }

    public void setHostId(String hostId) {
        this.hostId = hostId;
    }

    public String getLoginId() {
        return loginId;
    }

    public void setLoginId(String loginId) {
        this.loginId = loginId;
    }

    public Date getEndTime() {
        return endTime;
    }

    public void setEndTime(Date endTime) {
        this.endTime = endTime;
    }

    public long getStartSequence() {
        return startSequence;
    }

    public void setStartSequence(long startSequence) {
        this.startSequence = startSequence;
    }

    public long getEndSequence() {
        return endSequence;
    }

    public void setEndSequence(long endSequence) {
        this.endSequence = endSequence;
    }

    public long getSensitiveColumns() {
        return sensitiveColumns;
    }

    public void setSensitiveColumns(long sensitiveColumns) {
        this.sensitiveColumns = sensitiveColumns;
    }

    public String getSqlStatement() {
        return sqlStatement;
    }

    public void setSqlStatement(String sqlStatement) {
        this.sqlStatement = sqlStatement;
    }

    public String getColumnsAffected() {
        return columnsAffected;
    }

    public void setColumnsAffected(String columnsAffected) {
        this.columnsAffected = columnsAffected;
    }

// Added for row counts

    public long getRowsAffected() {
        return rowsAffected;
    }

    public void setRowsAffected(long rowsAffected) {
        this.rowsAffected = rowsAffected;
    }

    public long getSqlVersion() {
        return sqlVersion;
    }

    public void setSqlVersion(long sqlVersion) {
        this.sqlVersion = sqlVersion;
    }

    public ArrayList<String> getSensitiveColumnList() {
        return sensitiveColumnList;
    }

    public void setSensitiveColumnList(ArrayList<String> sensitiveColumnList) {
        this.sensitiveColumnList = sensitiveColumnList;
    }

    public CMEPBeforeAfterData getBeforeAfterData() {
        return beforeAfterData;
    }

    public void setBeforeAfterData(CMEPBeforeAfterData beforeAfterData) {
        this.beforeAfterData = beforeAfterData;
    }

    @Override
    public String toString() {
        return "CMEventProperties{" +
            "eventId=" + id +
            ", eventType=" + eventType +
            ", eventCategory=" + eventCategory +
            ", targetObject='" + targetObject + '\'' +
            ", details='" + details + '\'' +
            ", hash='" + hash + '\'' +
            ", eventClass=" + eventClass +
            ", eventSubclass=" + eventSubclass +
            ", startTime=" + startTime +
            ", spid=" + spid +
            ", applicationName='" + applicationName + '\'' +
            ", hostName='" + hostName + '\'' +
            ", serverName='" + serverName + '\'' +
            ", loginName='" + loginName + '\'' +
            ", success=" + success +
            ", databaseName='" + databaseName + '\'' +
            ", databaseId=" + databaseId +
            ", databaseUserName='" + databaseUserName + '\'' +
            ", objectType=" + objectType +
            ", objectName='" + objectName + '\'' +
            ", objectId=" + objectId +
            ", permissions=" + permissions +
            ", columnPermissions=" + columnPermissions +
            ", targetLoginName='" + targetLoginName + '\'' +
            ", targetUserName='" + targetUserName + '\'' +
            ", roleName='" + roleName + '\'' +
            ", ownerName='" + ownerName + '\'' +
            ", alertLevel=" + alertLevel +
            ", checkSum='" + checkSum + '\'' +
            ", privilegedUser=" + privilegedUser +
            ", name='" + name + '\'' +
            ", category='" + category + '\'' +
            ", fileName='" + fileName + '\'' +
            ", linkedServerName='" + linkedServerName + '\'' +
            ", parentName='" + parentName + '\'' +
            ", isSystem=" + isSystem +
            ", sessionLoginName='" + sessionLoginName + '\'' +
            ", providerName='" + providerName + '\'' +
            ", appNameId='" + appNameId + '\'' +
            ", hostId='" + hostId + '\'' +
            ", loginId='" + loginId + '\'' +
            ", endTime=" + endTime +
            ", startSequence=" + startSequence +
            ", endSequence=" + endSequence +
            ", sensitiveColumns=" + sensitiveColumns +
            '}';
    }
}