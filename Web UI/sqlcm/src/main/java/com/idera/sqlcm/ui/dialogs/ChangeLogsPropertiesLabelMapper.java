package com.idera.sqlcm.ui.dialogs;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import java.util.LinkedHashMap;

public class ChangeLogsPropertiesLabelMapper {

    static ChangeLogsPropertiesLabelMapper _instance;

    private static LinkedHashMap<String, String> propertiesDetailsTabMap = null;

    public synchronized static ChangeLogsPropertiesLabelMapper getInstance() {
        if (_instance == null) {
            _instance = new ChangeLogsPropertiesLabelMapper();
        }
        return _instance;
    }

    public static LinkedHashMap<String, String> getPropertiesDetailsTabMap() {
        return propertiesDetailsTabMap;
    }

    public ChangeLogsPropertiesLabelMapper() {
        propertiesDetailsTabMap = new LinkedHashMap<>();
        propertiesDetailsTabMap.put("eventId",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_ID));
        propertiesDetailsTabMap.put("logType",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TYPE_ID));
        propertiesDetailsTabMap.put("logUser",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_APPLICATION));
        propertiesDetailsTabMap.put("logSqlServer",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_PROP_DETAILS));
        propertiesDetailsTabMap.put("eventTime",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_PROP_DETAILS));
        propertiesDetailsTabMap.put("logInfo",ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_PROP_DETAILS));
       }

    public static String getLabelByPropertyName(String propName){
        String label = propertiesDetailsTabMap.get(propName);
        return label == null ? propName : label;
    }

}
