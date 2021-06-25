package com.idera.sqlcm.ui.dialogs;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.LinkedHashMap;

public class EventPropertiesLabelMapper {

    public static final String START_TIME_PROPERTY = "startTime";
    static EventPropertiesLabelMapper _instance;

    private static LinkedHashMap<String, String> propertiesDetailsTabMap = null;

    public synchronized static EventPropertiesLabelMapper getInstance() {
        if (_instance == null) {
            _instance = new EventPropertiesLabelMapper();
        }
        return _instance;
    }

    public static LinkedHashMap<String, String> getPropertiesDetailsTabMap() {
        return propertiesDetailsTabMap;
    }

    public EventPropertiesLabelMapper() {
        propertiesDetailsTabMap = new LinkedHashMap<>();
        propertiesDetailsTabMap.put("eventId",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_ID));
        propertiesDetailsTabMap.put(START_TIME_PROPERTY,ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TIME));
        propertiesDetailsTabMap.put("eventType",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TYPE_ID));
        propertiesDetailsTabMap.put("name",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TYPE));
        propertiesDetailsTabMap.put("category",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_CATEGORY));
        propertiesDetailsTabMap.put("eventCategory",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_CATEGORY_ID));
        propertiesDetailsTabMap.put("applicationName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_APPLICATION_NAME));
        propertiesDetailsTabMap.put("targetObject",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_TARGET_OBJECT));
        propertiesDetailsTabMap.put("details",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_PROP_DETAILS));
        propertiesDetailsTabMap.put("eventClass",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_CLASS));
        propertiesDetailsTabMap.put("eventSubclass",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_SUBCLASS));
        propertiesDetailsTabMap.put("spid",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_SPID));
        propertiesDetailsTabMap.put("hostName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_HOST_NAME));
        propertiesDetailsTabMap.put("loginName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_LOGIN_NAME));
        propertiesDetailsTabMap.put("databaseName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_DATABASE_NAME));
        propertiesDetailsTabMap.put("databaseId",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_DATABASE_ID));
        propertiesDetailsTabMap.put("databaseUserName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_DATABASE_USER_NAME));
        propertiesDetailsTabMap.put("objectType",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_OBJECT_TYPE));
        propertiesDetailsTabMap.put("objectName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_OBJECT_NAME));
        propertiesDetailsTabMap.put("objectId",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_OBJECT_ID));
        propertiesDetailsTabMap.put("permissions",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_PERMISSIONS));
        propertiesDetailsTabMap.put("privilegedUser",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_PRIVILEGED_USER_EVENT));
        propertiesDetailsTabMap.put("columnPermissions",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_COLUMN_PERMISSIONS));
        propertiesDetailsTabMap.put("targetLoginName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_TARGET_LOGIN_NAME));
        propertiesDetailsTabMap.put("targetUserName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_TARGET_USER_NAME));
        propertiesDetailsTabMap.put("serverName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_SERVER_NAME));
        propertiesDetailsTabMap.put("roleName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_ROLE_NAME));
        propertiesDetailsTabMap.put("ownerName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_OWNER_NAME));
        propertiesDetailsTabMap.put("checkSum",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_CHECKSUM));
        propertiesDetailsTabMap.put("hash",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_HASH));
        propertiesDetailsTabMap.put("fileName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_FILE_NAME));
        propertiesDetailsTabMap.put("linkedServerName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_LINKED_SERVER_NAME));
        propertiesDetailsTabMap.put("parentName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_PARENT_NAME));
        propertiesDetailsTabMap.put("isSystem",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_SYSTEM_EVENT));
        propertiesDetailsTabMap.put("sessionLoginName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_SESSION_LOGIN_NAME));
        propertiesDetailsTabMap.put("providerName",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_PROVIDER_NAME));
        propertiesDetailsTabMap.put("accessCheck",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_ACCESS_CHECK));
    }

    public static String getLabelByPropertyName(String propName){
        String label = propertiesDetailsTabMap.get(propName);
        return label == null ? propName : label;
    }

}
