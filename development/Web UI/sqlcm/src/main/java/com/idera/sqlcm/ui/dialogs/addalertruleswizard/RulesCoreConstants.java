package com.idera.sqlcm.ui.dialogs.addalertruleswizard;
import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

public class RulesCoreConstants {
	
	Map<String,String> FilterConditions;
	Map<String, String> FilterName;
	Map<String, String> TepmlateData;
	Map<String, String> RulesLevel;
	Map<String, String> RulesType;
	Map<String, String> StutusAlertConditionMap;
	Map<String, String> DataAlertConditionMap;
	Map<String, String> EventLogEntryMap;
	public RulesCoreConstants(){
				
	}
	
	public Map<String, String> FilterConditionList(){
		FilterConditions = new HashMap<String, String>();
		FilterConditions.put("11", "Session Login");
		FilterConditions.put("10", "Hostname");
		FilterConditions.put("9", "SQL Server");
		FilterConditions.put("8", "Object Type");
		FilterConditions.put("7", "Privileged User");
		FilterConditions.put("6", "Object Name");
		FilterConditions.put("5", "Database");
		FilterConditions.put("4", "Access Check Passed");
		FilterConditions.put("3", "Login Name");
		FilterConditions.put("2", "Application name");
		FilterConditions.put("1", "Category");
		FilterConditions.put("0", "Type");
        return FilterConditions;
	}
	
	public Map<String, String> FilterNameList(){
		FilterName = new HashMap<String, String>();
		FilterName.put("Session Login", "sessionLoginName");
		FilterName.put("Hostname", "hostName");
		FilterName.put("SQL Server", "serverName");
		FilterName.put("Object Type", "objectType");
		FilterName.put("Privileged User", "privilegedUser");
		FilterName.put("Object Name", "objectName");
		FilterName.put("Database", "databaseName");
		FilterName.put("Access Check Passed", "success");
		FilterName.put("Login Name", "loginName");
		FilterName.put("Application name", "applicationName");
		FilterName.put("Category", "eventCategory");
		FilterName.put("Type", "eventType");
		return FilterName;
	}
	
	public Map<String, String> TemplateList(){
		TepmlateData = new HashMap<String, String>();
		TepmlateData.put("Alert Level","$AlertLevel$");
		TepmlateData.put("Alert Time","$AlertTime$");
		TepmlateData.put("Alert Type", "$AlertType$");
		TepmlateData.put("Application Name", "$ApplicationName$");
		TepmlateData.put("Database Name", "$DatabaseName$");
		TepmlateData.put("Event Id", "$EventId$");
		TepmlateData.put("Event Time", "$EventTime$");
		TepmlateData.put("Event Type", "$EventType$");
		TepmlateData.put("Host Name", "$HostName$");
		TepmlateData.put("Login", "$Login$");
		TepmlateData.put("Object Name", "$ObjectName$");
		TepmlateData.put("Privileged User",  "$PrivilegedUser$");
		TepmlateData.put("Server Name",  "$ServerName$");
		TepmlateData.put("SQL Text", "$SQLText$");
		TepmlateData.put("Success",  "$Success$");
		TepmlateData.put("Target Login", "$TargetLogin$");
		TepmlateData.put("Target Object", "$TargetObject$");
		TepmlateData.put("Target User", "$TargetUser$");
		TepmlateData.put("Actual Value", "$ActualValue$");
		TepmlateData.put("Alert Type Name", "$AlertTypeName$");
		TepmlateData.put("Computer Name", "$ComputerName$");
		TepmlateData.put("Threshold Value", "$ThresholdValue$");
		TepmlateData.put("Column Name", "$ColumnName$");
		TepmlateData.put("Table Name", "$TableName$");
		TepmlateData.put("After Data Value", "$AfterDataValue$");
		TepmlateData.put("Before Data Value", "$BeforeDataValue$");
		return TepmlateData;
	}
	
	public Map<String, String> RulesLevel(){
		RulesLevel = new HashMap<String, String>();
		RulesLevel.put("1", "Low");
		RulesLevel.put("2", "Medium");
		RulesLevel.put("3", "High");
		RulesLevel.put("4", "Severe");
        return RulesLevel;
	}
	

	public Map<String, String> RulesType(){
		RulesType = new HashMap<String, String>();
		RulesType.put("1", "EVENT");
		RulesType.put("2", "STATUS");
		RulesType.put("3", "DATA");
		return RulesType;
	}
	

	public Map<String, String> StatusAlertConditionMap(){
		StutusAlertConditionMap = new HashMap<String, String>();
		StutusAlertConditionMap.put("1", ELFunctions.getLabel(SQLCMI18NStrings.TRACE_DIR_FULL_AGENT));
		StutusAlertConditionMap.put("2", ELFunctions.getLabel(SQLCMI18NStrings.TRACE_DIR_FULL_COLLECT));
		StutusAlertConditionMap.put("3", ELFunctions.getLabel(SQLCMI18NStrings.NO_HEARTBEAT));
		StutusAlertConditionMap.put("4", ELFunctions.getLabel(SQLCMI18NStrings.REPOSITORY_TOO_BIG));
		StutusAlertConditionMap.put("5", ELFunctions.getLabel(SQLCMI18NStrings.SQL_SERVER_DOWN) );
		return StutusAlertConditionMap;
	}
	

	public Map<String, String> DataAlertConditionMap(){
		DataAlertConditionMap = new HashMap<String, String>();
		DataAlertConditionMap.put("1", ELFunctions.getLabel(SQLCMI18NStrings.SENSITIVE_COLUMN_ACCESSED));
		DataAlertConditionMap.put("2", ELFunctions.getLabel(SQLCMI18NStrings.NUMRIC_COLUMN_VALUE_CHANGED));
		DataAlertConditionMap.put("3", ELFunctions.getLabel(SQLCMI18NStrings.COLUMN_VALUE_CHANGED));
		return DataAlertConditionMap;
	}
	
	public Map<String, String> EventLogEntryMap(){
		EventLogEntryMap = new HashMap<String, String>();
		EventLogEntryMap.put("0", ELFunctions.getLabel(SQLCMI18NStrings.INFORMATION));
		EventLogEntryMap.put("1", ELFunctions.getLabel(SQLCMI18NStrings.ALERTS_SEVERITY_WARNING));
		EventLogEntryMap.put("2", ELFunctions.getLabel(SQLCMI18NStrings.ALERTS_SEVERITY_CRITICAL));
		return EventLogEntryMap;
	}
}
