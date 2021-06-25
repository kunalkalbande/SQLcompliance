package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import java.util.List;
import java.util.Map;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;

import com.google.common.base.Joiner;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.AddDataAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.NewDataAlertRules;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.RegulationSettings;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.SelectDataAlertActions;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyAppNameViewModel.App;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyLoginViewModel.Login;

public class SummaryStepViewModel extends AddWizardStepBase {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/adddataalertruleswizard/steps/summary-step.zul";
	InsertQueryData insertQueryData;
	public NewDataAlertRules getSavedAlertRulesInfo;

	private String dataAlertRuleDescription;
	public String sqlTableValue;
	public int logType;
	boolean isValidRule = true;

	public String getSqlTableValue() {
		return sqlTableValue;
	}

	public void setSqlTableValue(String sqlTableValue) {
		this.sqlTableValue = sqlTableValue;
	}


	public String sqlTableName;
    
	public String getSqlTableName() {
		return sqlTableName;
	}

	public void setSqlTableName(String sqlTableName) {
		this.sqlTableName = sqlTableName;
	}


	public RegulationSettings getRegulationSettings;
    
    public SelectDataAlertActions getSelectAlertActions;
    private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCondition> conditionEvents;
	boolean enabled;
	public String sqlServerValue;
	public String sqlServerName;
	public String sqlDatabaseValue;
	public String sqlColumnnameValue;
	public String applicationNameValue;
	private String loginNameValue;

	public String getApplicationNameValue() {
		return applicationNameValue;
	}

	public void setApplicationNameValue(String applicationNameValue) {
		this.applicationNameValue = applicationNameValue;
	}
	public String getLoginNameValue() {
		return loginNameValue;
	}

	public void setLoginNameValue(String loginNameValue) {
		this.loginNameValue = loginNameValue;
	}

	public String rowCountWithTimeIntervalValue;

	public String getRowCountWithTimeIntervalValue() {
		return rowCountWithTimeIntervalValue;
	}

	public void setRowCountWithTimeIntervalValue(
			String rowCountWithTimeIntervalValue) {
		this.rowCountWithTimeIntervalValue = rowCountWithTimeIntervalValue;
	}
	public String getSqlDatabaseValue() {
		return sqlDatabaseValue;
	}

	public void setSqlDatabaseValue(String sqlDatabaseValue) {
		this.sqlDatabaseValue = sqlDatabaseValue;
	}

	public String alertMessage;
	public String alertMessageValue;

	public String getAlertMessageValue() {
		return alertMessageValue;
	}

	public void setAlertMessageValue(String alertMessageValue) {
		this.alertMessageValue = alertMessageValue;
	}

	public String getAlertMessage() {
		return alertMessage;
	}

	public void setAlertMessage(String alertMessage) {
		this.alertMessage = alertMessage;
	}

	public String sqlDatabaseName;

	public String getSqlDatabaseName() {
		return sqlDatabaseName;
	}

	public void setSqlDatabaseName(String sqlDatabaseName) {
		this.sqlDatabaseName = sqlDatabaseName;
	}

	public String getSqlServerName() {
		return sqlServerName;
	}

	public void setSqlServerName(String sqlServerName) {
		this.sqlServerName = sqlServerName;
	}

	public String getSqlServerValue() {
		return sqlServerValue;
	}

	public void setSqlServerValue(String sqlServerValue) {
		this.sqlServerValue = sqlServerValue;
	}

	public String sqlColumnName;

	public String getSqlColumnName() {
		return sqlColumnName;
	}

	public void setSqlColumnName(String sqlColumnName) {
		this.sqlColumnName = sqlColumnName;
	}

	public String sqlColumnValue;

	public String getSqlColumnValue() {
		return sqlColumnValue;
	}

	public void setSqlColumnValue(String sqlColumnValue) {
		this.sqlColumnValue = sqlColumnValue;
	}

	public String generateText;

	public String getGenerateText() {
		return generateText;
	}

	public void setGenerateText(String generateText) {
		this.generateText = generateText;
	}

public String windowEventValue;
public String getWindowEventValue() {
	return windowEventValue;
}

public void setWindowEventValue(String windowEventValue) {
	this.windowEventValue = windowEventValue;
}


public String windowEventName;
	public String getWindowEventName() {
	return windowEventName;
}

public void setWindowEventName(String windowEventName) {
	this.windowEventName = windowEventName;
}


	public String generateValue;
    public String getGenerateValue() {
		return generateValue;
	}

	public void setGenerateValue(String generateValue) {
		this.generateValue = generateValue;
	}


	public String generateAlert;
    public String getGenerateAlert() {
		return generateAlert;
	}

	public void setGenerateAlert(String generateAlert) {
		this.generateAlert = generateAlert;
	}


	public String ruleType;
   
		
    public String getRuleType() {
		return ruleType;
	}

	public void setRuleType(String ruleType) {
		this.ruleType = ruleType;
	}

	@Wire 
	Label sqlAlertIdUnspecified,sqlAlertIdSpecified,sqlAlertIdAll;
	@Wire
	Label snmpAddressText, snmpAddress, snmpPort, snmpPortText, snmpCommunity,
			snmpCommunityText;
	@Wire
	Label applicationNameIdSpecified;

	public Label getApplicationNameIdSpecified() {
		return applicationNameIdSpecified;
	}

	public void setApplicationNameIdSpecified(Label applicationNameIdSpecified) {
		this.applicationNameIdSpecified = applicationNameIdSpecified;
	}

	public Label getSqlAlertIdAll() {
		return sqlAlertIdAll;
	}

	public void setSqlAlertIdAll(Label sqlAlertIdAll) {
		this.sqlAlertIdAll = sqlAlertIdAll;
	}

	public Label getSqlAlertIdSpecified() {
		return sqlAlertIdSpecified;
	}

	public void setSqlAlertIdSpecified(Label sqlAlertIdSpecified) {
		this.sqlAlertIdSpecified = sqlAlertIdSpecified;
	}

	public Label getSqlAlertIdUnspecified() {
		return sqlAlertIdUnspecified;
	}

	public void setSqlAlertIdUnspecified(Label sqlAlertIdUnspecified) {
		this.sqlAlertIdUnspecified = sqlAlertIdUnspecified;
	}

	@Wire
	Label sqlWindowEventIdUnspecified, sqlWindowEventIdSpecified,
			sqlWindowEventIdAll;

	public Label getSqlWindowEventIdAll() {
	return sqlWindowEventIdAll;
}

public void setSqlWindowEventIdAll(Label sqlWindowEventIdAll) {
	this.sqlWindowEventIdAll = sqlWindowEventIdAll;
}

	public Label getSqlWindowEventIdSpecified() {
	return sqlWindowEventIdSpecified;
}

public void setSqlWindowEventIdSpecified(Label sqlWindowEventIdSpecified) {
	this.sqlWindowEventIdSpecified = sqlWindowEventIdSpecified;
}

	public Label getSqlWindowEventIdUnspecified() {
	return sqlWindowEventIdUnspecified;
}

public void setSqlWindowEventIdUnspecified(Label sqlWindowEventIdUnspecified) {
	this.sqlWindowEventIdUnspecified = sqlWindowEventIdUnspecified;
}


	@Wire 
Label sqlDatabaseIdSpecified,sqlDatabaseIdUnspecified,sqlDatabaseIdAll;
	public Label getSqlDatabaseIdAll() {
	return sqlDatabaseIdAll;
}

public void setSqlDatabaseIdAll(Label sqlDatabaseIdAll) {
	this.sqlDatabaseIdAll = sqlDatabaseIdAll;
}

	public Label getSqlDatabaseIdUnspecified() {
	return sqlDatabaseIdUnspecified;
}

public void setSqlDatabaseIdUnspecified(Label sqlDatabaseIdUnspecified) {
	this.sqlDatabaseIdUnspecified = sqlDatabaseIdUnspecified;
}

	public Label getSqlDatabaseIdSpecified() {
	return sqlDatabaseIdSpecified;
}

public void setSqlDatabaseIdSpecified(Label sqlDatabaseIdSpecified) {
	this.sqlDatabaseIdSpecified = sqlDatabaseIdSpecified;
}

@Wire
Label sqlTableIdUnspecified ,sqlTableIdSpecified,sqlTableIdAll;
	public Label getSqlTableIdAll() {
	return sqlTableIdAll;
}

public void setSqlTableIdAll(Label sqlTableIdAll) {
	this.sqlTableIdAll = sqlTableIdAll;
}

	public Label getSqlTableIdSpecified() {
	return sqlTableIdSpecified;
}

public void setSqlTableIdSpecified(Label sqlTableIdSpecified) {
	this.sqlTableIdSpecified = sqlTableIdSpecified;
}

	public Label getSqlTableIdUnspecified() {
	return sqlTableIdUnspecified;
}

public void setSqlTableIdUnspecified(Label sqlTableIdUnspecified) {
	this.sqlTableIdUnspecified = sqlTableIdUnspecified;
}

@Wire
Label sqlColumnIdSpecified,sqlColumnIdUnspecified,sqlColumnIdAll;
	public Label getSqlColumnIdAll() {
	return sqlColumnIdAll;
}

public void setSqlColumnIdAll(Label sqlColumnIdAll) {
	this.sqlColumnIdAll = sqlColumnIdAll;
}

	public Label getSqlColumnIdUnspecified() {
	return sqlColumnIdUnspecified;
}

public void setSqlColumnIdUnspecified(Label sqlColumnIdUnspecified) {
	this.sqlColumnIdUnspecified = sqlColumnIdUnspecified;
}

	public Label getSqlColumnIdSpecified() {
	return sqlColumnIdSpecified;
}

public void setSqlColumnIdSpecified(Label sqlColumnIdSpecified) {
	this.sqlColumnIdSpecified = sqlColumnIdSpecified;
}


	@Wire
	Checkbox enableRuleNow;
	@Wire
	Label sqlServerIdUnspecified, sqlServerIdSpecified, sqlServerIdAll,
			applicationNameMsgString, applicationNameIdAllMsgString,
			loginNameIdSpecified, loginNameIdAllMsgString,
			rowCountThresholdMsgString, rowCountThresholdIdAllMsgString,
			timeIntervalMsgString, timeIntervalIdAllMsgString,
			timeIntervalHoursMsg;

	public Label getTimeIntervalHoursMsg() {
		return timeIntervalHoursMsg;
	}

	public void setTimeIntervalHoursMsg(Label timeIntervalHoursMsg) {
		this.timeIntervalHoursMsg = timeIntervalHoursMsg;
	}

	public Label getTimeIntervalIdAllMsgString() {
		return timeIntervalIdAllMsgString;
	}

	public void setTimeIntervalIdAllMsgString(Label timeIntervalIdAllMsgString) {
		this.timeIntervalIdAllMsgString = timeIntervalIdAllMsgString;
	}

	public Label getTimeIntervalMsgString() {
		return timeIntervalMsgString;
	}

	public void setTimeIntervalMsgString(Label timeIntervalMsgString) {
		this.timeIntervalMsgString = timeIntervalMsgString;
	}

	public Label getRowCountThresholdIdAllMsgString() {
		return rowCountThresholdIdAllMsgString;
	}

	public void setRowCountThresholdIdAllMsgString(
			Label rowCountThresholdIdAllMsgString) {
		this.rowCountThresholdIdAllMsgString = rowCountThresholdIdAllMsgString;
	}

	public Label getRowCountThresholdMsgString() {
		return rowCountThresholdMsgString;
	}

	public void setRowCountThresholdMsgString(Label rowCountThresholdMsgString) {
		this.rowCountThresholdMsgString = rowCountThresholdMsgString;
	}

	public Label getLoginNameIdAllMsgString() {
		return loginNameIdAllMsgString;
	}

	public void setLoginNameIdAllMsgString(Label loginNameIdAllMsgString) {
		this.loginNameIdAllMsgString = loginNameIdAllMsgString;
	}

	public Label getloginNameIdSpecified() {
		return loginNameIdSpecified;
	}

	public void setLoginNameMsgString(Label loginNameIdSpecified) {
		this.loginNameIdSpecified = loginNameIdSpecified;
	}

	public Label getApplicationNameIdAllMsgString() {
		return applicationNameIdAllMsgString;
	}

	public void setApplicationNameIdAllMsgString(
			Label applicationNameIdAllMsgString) {
		this.applicationNameIdAllMsgString = applicationNameIdAllMsgString;
	}

	public Label getApplicationNameMsgString() {
		return applicationNameMsgString;
	}

	public void setApplicationNameMsgString(Label applicationNameMsgString) {
		this.applicationNameMsgString = applicationNameMsgString;
	}

	public Label getSqlServerIdAll() {
		return sqlServerIdAll;
	}

	public void setSqlServerIdAll(Label sqlServerIdAll) {
		this.sqlServerIdAll = sqlServerIdAll;
	}

	public Label getSqlServerIdSpecified() {
		return sqlServerIdSpecified;
	}

	public void setSqlServerIdSpecified(Label sqlServerIdSpecified) {
		this.sqlServerIdSpecified = sqlServerIdSpecified;
	}

	public Label getSqlServerIdUnspecified() {
		return sqlServerIdUnspecified;
	}

	public void setSqlServerIdUnspecified(Label sqlServerIdUnspecified) {
		this.sqlServerIdUnspecified = sqlServerIdUnspecified;
	}
	private String applicationName;
	private String loginName;

	public String getApplicationName() {
		return applicationName;
	}

	public void setApplicationName(String applicationName) {
		this.applicationName = applicationName;
	}
	public String getLoginName() {
		return loginName;
	}

	public void setLoginName(String loginName) {
		this.loginName = loginName;
	}
	String margin = "\t";
    
    Long ruleId;
    public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
	}
	
	/*@Command("onCheck")
	public void onCheck(@BindingParam("target") Checkbox target){
		String chkName = target.getLabel();
		if (target.isChecked()) {
			setEnabled(true);
		}
		else{
			setEnabled(false);
		}
	}*/
	
	public SummaryStepViewModel() {
        super();
    }
		
    public String getDataAlertRuleDescription() {
		return dataAlertRuleDescription;
	}

	public void setDataAlertRuleDescription(String dataAlertRuleDescription) {
		this.dataAlertRuleDescription = dataAlertRuleDescription;
	}

	@Override
	public void onCancel(AddDataAlertRulesSaveEntity wizardSaveEntity) {
    	if(Sessions.getCurrent().getAttribute("QueryType")!=null)
    	{
    		Sessions.getCurrent().removeAttribute("QueryType");
    		Sessions.getCurrent().removeAttribute("QueryTypeForColumn");
			Sessions.getCurrent().removeAttribute("columnName");
            Sessions.getCurrent().removeAttribute("tableName");
      		Sessions.getCurrent().removeAttribute("dbName");
      		Sessions.getCurrent().removeAttribute("serverName");
    	}
    	 String uri = "instancesAlertsRule";
         uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
         Executions.sendRedirect(uri);
	}
	
    @Override
    public void doOnShow(AddDataAlertRulesSaveEntity wizardSaveEntity) {  
    	getSavedAlertRulesInfo = wizardSaveEntity.getNewEventAlertRules();
    	getRegulationSettings = wizardSaveEntity.getRegulationSettings();
    	getSelectAlertActions = wizardSaveEntity.getSelectAlertActions();
    	setEnabled(true);
    	String description="Generate a ";
    	generateText="Generate a";
    	int AlertLevel = getSavedAlertRulesInfo.getAlertLevel();
    	if(AlertLevel > 0){
    		switch(AlertLevel){
    		case 1:
    			generateValue=" "+"Low";
    			description +="Low";
    			break;
    		case 2:
    			generateValue=" "+"Medium";
    			description +="Medium";
    			break;
    		case 3:
    			generateValue=" "+"High";
    			description +="High";
    			break;
    		case 4:
    			generateValue=" "+"Server";
    			description +="Severe";
    			break;	
    		}
    	}
    	
    	generateAlert=" "+"alert ";
    	description +=" alert ";
    	description +="\r\n";
    	int fieldId;	
    	fieldId = getSavedAlertRulesInfo.getFieldId();
    	if(fieldId == 1)
    	{
    		ruleType=" "+" When a Sensitive Column is accessed";
    		description += "When a Sensitive Column is accessed";
    	}
    	
    	else
    	{
    		ruleType=" "+"when the value of a column changes";
    		description += "when the value of a column changes ";
    	}
    	boolean sqlColumnCheck = getRegulationSettings.isColumnName();
    	String targetColumn = (String) Sessions.getCurrent().getAttribute("columnName");
    	if(sqlColumnCheck == true){
    		if(targetColumn != null && !targetColumn.isEmpty() && !targetColumn.equalsIgnoreCase("<ALL>"))
    		{
    			description += sqlColumnValue=" "+"on column";
    			sqlColumnIdSpecified.setValue(sqlColumnValue);
    			description += sqlColumnValue=" "+targetColumn;
    			sqlColumnIdAll.setValue(targetColumn);
    			sqlColumnIdSpecified.setVisible(true);
    			sqlColumnIdAll.setVisible(true);
    			sqlColumnIdAll.setStyle("color:blue");
    		}
    		else{
    			sqlColumnValue=" "+"on ";
    			sqlColumnIdSpecified.setValue(sqlColumnValue);
    			sqlColumnValue=" "+"specified column";
    			sqlColumnIdAll.setValue(sqlColumnValue);
    			sqlColumnIdSpecified.setVisible(true);
    			sqlColumnIdAll.setVisible(true);
    			sqlColumnIdAll.setStyle("color:red");
    		}
    	}

    	else{
			sqlColumnIdSpecified.setVisible(false);
			sqlColumnIdAll.setVisible(false);
    	}
    	 
    	
    	boolean sqltableCheck = getRegulationSettings.isTableName();
    	String targetTable;
    	//targetTable = (String) Sessions.getCurrent().getAttribute("tableName");
    	targetTable = (String) Sessions.getCurrent().getAttribute("completeTableName"); //SQLCM-4257
    	if(sqltableCheck == true){
    		
    		if(targetTable != null && !targetTable.isEmpty() && !targetTable.equalsIgnoreCase("<ALL>"))
    		{
    			if(sqlColumnCheck)
    				sqlTableValue=" "+"in table ";
    			else
    				sqlTableValue=" on any column "+"in table ";
    			sqlTableIdSpecified.setValue(sqlTableValue);
    			description += margin + "in table ";
    			description +=targetTable;
    			sqlTableName=" "+targetTable;
    			sqlTableIdAll.setValue(sqlTableName);
    			sqlTableIdSpecified.setVisible(true);
    			sqlTableIdAll.setVisible(true);
    			sqlTableIdAll.setStyle("color:blue");
    		}
    		else{
    			if(sqlColumnCheck)
    				sqlTableValue=" "+"in a";
    			else
    				sqlTableValue=" on any column "+"in ";
    			sqlTableValue=" "+"in a ";
    			sqlTableName="specified table";
    			sqlTableIdSpecified.setValue(sqlTableValue);
    			sqlTableIdAll.setValue(sqlTableName);
    			sqlTableIdSpecified.setVisible(true);
    			sqlTableIdAll.setVisible(true);
    			sqlTableIdAll.setStyle("color:red");
    		}
    	}else
    	{
    		sqlTableIdSpecified.setVisible(false);
    		sqlTableIdAll.setVisible(false);
    	}
    	

    	boolean sqlDBCheck = getRegulationSettings.isDatabaseName();
    	String targetDatabase = getRegulationSettings.getDbName();
    	if(sqlDBCheck == true){
    		getRegulationSettings.setSqlServer(true);
    		if(targetDatabase != null && !targetDatabase.isEmpty() && !targetDatabase.equalsIgnoreCase("<ALL>"))
    		{
    			if(sqlColumnCheck){
    				sqlDatabaseValue=" "+" in Database";
    			}
    			else{
    				sqlDatabaseValue=" in any table"+" in Database";
    			}
    			
    			description +="\r\n";
    			description += margin + "on Database ";
    			description +=targetDatabase;
    			sqlDatabaseName=" "+targetDatabase;
    			sqlDatabaseIdSpecified.setValue(sqlDatabaseValue);
    			sqlDatabaseIdAll.setValue(sqlDatabaseName);
        		sqlDatabaseIdSpecified.setVisible(true);
        		sqlDatabaseIdAll.setVisible(true);
        		sqlDatabaseIdAll.setStyle("color:blue");
        		
    		}
    		else{
    			description += "\r\n";
    			description += margin + "in a specified database  ";
    			if(sqlColumnCheck){
    				sqlDatabaseValue=" "+"in a";
    			}
    			else{
    				sqlDatabaseValue=" in any table"+" "+"in a";
    			}
    			sqlDatabaseValue=" "+"in a";
    			sqlDatabaseName=" specified database";
    			sqlDatabaseIdSpecified.setValue(sqlDatabaseValue);
    			sqlDatabaseIdAll.setValue(sqlDatabaseName);
        		sqlDatabaseIdSpecified.setVisible(true);
        		sqlDatabaseIdAll.setVisible(true);
        		sqlDatabaseIdAll.setStyle("color:red");
    		}
    	}
    	else
    	{
    		sqlDatabaseIdSpecified.setVisible(false);
    		sqlDatabaseIdAll.setVisible(false);
    	}
    	
    	boolean sqlServerCheck = getRegulationSettings.isSqlServer();
    	String targetInstances = getRegulationSettings.getInstances();
    	if(sqlServerCheck == true){
    		if(sqlDBCheck){
    			sqlServerValue= "in instance ";
    		}
    		else
    			sqlServerValue="in any database in instance ";
    			
    		if(targetInstances!=null && !targetInstances.isEmpty() && !targetInstances.equalsIgnoreCase("<ALL>")){
        		sqlServerIdSpecified.setValue(sqlServerValue);
        		sqlServerIdAll.setValue(targetInstances);
        		sqlServerIdAll.setStyle("color:blue");
    		}
    		else{
    			sqlServerIdSpecified.setValue(sqlServerValue+"a ");
        		sqlServerIdAll.setValue("specified instance");
        		sqlServerIdAll.setStyle("color:red");
    		}
    		sqlServerIdSpecified.setVisible(true);
    		sqlServerIdAll.setVisible(true);
    	}
    	else{
    		sqlServerValue=" "+"on any SQL Server";
			description += "\r\n";
			description += margin + "on any SQL Server ";
			sqlServerIdSpecified.setVisible(true);
			sqlServerIdAll.setVisible(false);
		}
		String specifiedEmail = null;
		boolean specifiedAppCheck = getRegulationSettings.isApplicationName();
		ListModelList<App> specifiedApplication = getRegulationSettings
				.getAppNameList();
		if (specifiedAppCheck == true) {
			applicationName = " " + "and Application Name like";
			description += "\r\n";
			description += margin + "and Application Name like ";
			int count = (specifiedApplication != null) ? specifiedApplication
					.getSize() : 0;
			if (count == 0) {
				applicationNameValue = " " + "specified words";
				description += "specified words   ";
				applicationNameIdAllMsgString.setStyle("color:red");
			} else {
				applicationNameIdAllMsgString.setStyle("color:blue");
				applicationNameValue = "";
				for (int j = 0; j < count; j++) {
					description += specifiedApplication.get(j).getAppName();
					applicationNameValue += " '"
							+ specifiedApplication.get(j).getAppName() + "'";
					if (j < count - 1) {
						applicationNameValue += " " + "or";
						description += ",";
					}
				}
			}

			applicationNameIdSpecified.setVisible(true);
			applicationNameIdAllMsgString.setVisible(true);
		} else {
			applicationNameIdSpecified.setVisible(false);
			applicationNameIdAllMsgString.setVisible(false);
		}

		boolean specifiedLoginCheck = getRegulationSettings.isLoginName();
		ListModelList<Login> specifiedLogin = getRegulationSettings
				.getLoginNameList();
		if (specifiedLoginCheck == true) {
			loginName = " " + "and Login Name like";
			loginNameValue = "";
			description += "\r\n";
			description += margin + "and Login Name like ";
			int count = (specifiedLogin != null) ? specifiedLogin.getSize() : 0;
			if (count == 0) {
				loginNameValue = " " + "specified words";
				description += "specified words   ";
				loginNameIdAllMsgString.setStyle("color:red");
			} else {
				loginNameIdAllMsgString.setStyle("color:blue");
				for (int j = 0; j < count; j++) {
					loginNameValue += " '"
							+ specifiedLogin.get(j).getLoginName() + "'";
					description += specifiedLogin.get(j).getLoginName();

					if (j < count - 1) {
						loginNameValue += " " + "or";
						description += "or";
					}
				}
			}

			loginNameIdSpecified.setVisible(true);
			loginNameIdAllMsgString.setVisible(true);
		} else {
			loginNameIdSpecified.setVisible(false);
			loginNameIdAllMsgString.setVisible(false);
		}

	boolean rowCount = getRegulationSettings.isRowCountWithTimeInterval();
	String rowCountWizard = (String) Sessions.getCurrent().getAttribute(
		"rowCountDetails");
	if (rowCount == true) {
	    if (rowCountWizard != null && !rowCountWizard.isEmpty()) {
		String rowCountFinalVal = rowCountWizard
			.substring(rowCountWizard.indexOf("rowcount"));
		int rowCountCharCount = Integer.parseInt(rowCountFinalVal
			.substring(rowCountFinalVal.indexOf("(") + 1,
				rowCountFinalVal.indexOf(")")));
		String rowCountVal = (rowCountFinalVal.substring(
			rowCountFinalVal.indexOf(")") + 1,
			rowCountFinalVal.indexOf(")") + rowCountCharCount + 1));

		int compOprCharCount = Integer.parseInt(rowCountWizard
			.substring(rowCountWizard.indexOf("(") + 1,
				rowCountFinalVal.indexOf(")")));
		String strCompOpr = (rowCountWizard.substring(
			rowCountWizard.indexOf(")") + 1,
			rowCountWizard.indexOf(")") + compOprCharCount + 1));

		String timeInterval = "";
		String timeFrameString = rowCountWizard
			.substring(rowCountWizard.indexOf("timeframe"));
		int timeFrameCharCount = Integer.parseInt(timeFrameString
			.substring(timeFrameString.indexOf("(") + 1,
				timeFrameString.indexOf(")")));
		timeInterval = (timeFrameString.substring(
			timeFrameString.indexOf(")") + 1,
			timeFrameString.indexOf(")") + timeFrameCharCount + 1));

		if (!rowCountVal.isEmpty()) {
		    rowCountThresholdMsgString.setVisible(true);
		    rowCountThresholdIdAllMsgString.setVisible(true);
		    String rowCountMsg = "when Row Count is" + " ";
		    rowCountThresholdMsgString.setValue(rowCountMsg);
		    rowCountThresholdIdAllMsgString.setValue(strCompOpr + ""
			    + rowCountVal + "\r\n");
		    rowCountThresholdIdAllMsgString.setStyle("color:blue");

		    if (!timeInterval.isEmpty()) {
			String timeIntervalMsg = "over the period of hours"
				+ " ";
			timeIntervalMsgString.setVisible(true);
			timeIntervalIdAllMsgString.setVisible(true);

			timeIntervalMsgString.setValue(timeIntervalMsg);
			timeIntervalIdAllMsgString.setValue(timeInterval);
			timeIntervalIdAllMsgString.setStyle("color:blue");
		    }
		}
		else {
		    String rowCountValue = "Specify Row Count Threshold";
		    String rowCountMsg = "when Row Count is" + " ";
		    rowCountThresholdMsgString.setVisible(true);
		    rowCountThresholdIdAllMsgString.setVisible(true);
		    rowCountThresholdMsgString.setValue(rowCountMsg);
		    rowCountThresholdIdAllMsgString.setValue(rowCountValue);
		    rowCountThresholdIdAllMsgString.setStyle("color:red");
		}
	    }

	    else {
		String rowCountValue = "Specify Row Count Threshold";
		String rowCountMsg = "when Row Count is" + " ";
		rowCountThresholdMsgString.setVisible(true);
		rowCountThresholdIdAllMsgString.setVisible(true);
		rowCountThresholdMsgString.setValue(rowCountMsg);
		rowCountThresholdIdAllMsgString.setValue(rowCountValue);
		rowCountThresholdIdAllMsgString.setStyle("color:red");
	    }
	}

	else {
	    rowCountThresholdMsgString.setVisible(false);
	    rowCountThresholdIdAllMsgString.setVisible(false);
	    timeIntervalMsgString.setVisible(false);
	    timeIntervalIdAllMsgString.setVisible(false);
	}

		if (getSelectAlertActions.isEmailNotification()) {
			if (Sessions.getCurrent().getAttribute("addressList") != null) {
				String[] mailAddresses = (String[]) Sessions.getCurrent()
						.getAttribute("addressList");
				specifiedEmail = Joiner.on(",").skipNulls().join(mailAddresses)
						.replace(",", ",");
			}

			if (specifiedEmail != null && (!specifiedEmail.isEmpty())) {
				description += "\n" + margin;
				description += "send an alert message to";
				description += "\n" + margin;
				description += specifiedEmail;
				alertMessage=" "+"send an alert message to";
				alertMessageValue=" "+specifiedEmail;
				sqlAlertIdSpecified.setValue(alertMessage);
				sqlAlertIdAll.setValue(alertMessageValue);
				sqlAlertIdAll.setStyle("color:blue");
				
			} else {
				description += "\n";
				description += "send an alert message to";
				alertMessage="send an alert message to";
				description += "\n" + margin;
				description += "specified addresses ";
				alertMessageValue="specified addresses";
				sqlAlertIdSpecified.setValue(alertMessage);
				sqlAlertIdAll.setValue(alertMessageValue);
				sqlAlertIdAll.setStyle("color:red");
			}
			sqlAlertIdSpecified.setVisible(true);
			sqlAlertIdAll.setVisible(true);
		}
    	
    	boolean specifiedlogEntry = getSelectAlertActions.isWindowEventLogEntry();
		if (specifiedlogEntry == true) {	
			windowEventValue=" "+"and Windows Event Log entry of type ";	
				description += "\n" + margin;
				description += "and Windows Event Log entry of type  ";			
				description +=   getSelectAlertActions.getEventLogEntry();
    	windowEventName=" "+ getSelectAlertActions.getEventLogEntry();	
    	sqlWindowEventIdSpecified.setValue(windowEventValue);
    	sqlWindowEventIdAll.setValue(windowEventName);
    	sqlWindowEventIdSpecified.setVisible(true);
    	sqlWindowEventIdAll.setVisible(true);
    	
    }
		else
		{
	    	sqlWindowEventIdSpecified.setVisible(false);
	    	sqlWindowEventIdAll.setVisible(false);
		}	
		
		boolean specifiedSnmpLog = getSelectAlertActions.isSnmpTrap();		
		if(specifiedSnmpLog){
			if(getSelectAlertActions.isEmailNotification() || specifiedlogEntry){
				snmpAddressText.setValue("SNMP trap address ");
			}
			else{
				snmpAddressText.setValue("Send an alert message to \r\n SNMP trap address ");
			}
			
			if(getSelectAlertActions.getAddress()!=null && !getSelectAlertActions.getAddress().isEmpty()){
				snmpAddress.setValue(getSelectAlertActions.getAddress());
				snmpAddress.setStyle("color: blue");
			}
			else{
				snmpAddress.setValue("specified address.");
				snmpAddress.setStyle("color: red");
			}
			snmpPortText.setValue("SNMP trap port ");
			snmpPort.setValue(""+getSelectAlertActions.getPort());
			snmpCommunityText.setValue("SNMP trap community ");
			if(getSelectAlertActions.getCommunity()!=null && 
					!getSelectAlertActions.getCommunity().isEmpty()){
				snmpCommunity.setValue(getSelectAlertActions.getCommunity());
				snmpCommunity.setStyle("color: blue");
			}
			else{
				snmpCommunity.setValue("specified community");
				snmpCommunity.setStyle("color: red");
			}
			snmpCommunityText.setVisible(true);
			snmpCommunity.setVisible(true);
			snmpPortText.setVisible(true);
			snmpPort.setVisible(true);
			snmpAddress.setVisible(true);
			snmpAddressText.setVisible(true);
		}
		else{
			snmpCommunityText.setVisible(false);
			snmpCommunity.setVisible(false);
			snmpPortText.setVisible(false);
			snmpPort.setVisible(false);
			snmpAddress.setVisible(false);
			snmpAddressText.setVisible(false);
		}
		
		
    	dataAlertRuleDescription = description;
    	setDataAlertRuleDescription(dataAlertRuleDescription);
    	if(Sessions.getCurrent().getAttribute("QueryType")!=null)
    	{
    		Sessions.getCurrent().removeAttribute("alertRuleId");
			if (Sessions.getCurrent().getAttribute("conditionEvents") != null) {
				conditionEvents = (List<CMAlertRulesCondition>) Sessions
						.getCurrent().getAttribute("conditionEvents");
				alertRules = (List<CMAlertRules>) Sessions.getCurrent()
						.getAttribute("alertRules");
				if (conditionEvents != null && alertRules != null)
				{
					initializer(alertRules);
				}
			}
    	}

    	BindUtils.postNotifyChange(null, null, SummaryStepViewModel.this, "*");
    	 BindUtils.postNotifyChange(null, null, SummaryStepViewModel.this, "dataAlertRuleDescription");
    }
    
    public void initializer(List<CMAlertRules> alertRules) {
		if (alertRules.get(0).getRuleId() != null) {
			if (alertRules.get(0).isEnabled()) {
    			setEnabled(true);
    			enableRuleNow.setChecked(true);
    		}
    		else{
    			setEnabled(false);
    			enableRuleNow.setChecked(false);
    		}
			ruleId = alertRules.get(0).getRuleId();
		}
	}


    @Override
    public String getNextStepZul() {
        return null;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.SQL_FINISH_DATA_ALERT_RULES_TIPS);
    }
    
    public String GenerateEventAlertRulesQuery(){
       	int logMessage = (getSelectAlertActions.isWindowEventLogEntry()?1:0);
    	int emailMessage = (getSelectAlertActions.isEmailNotification()?1:0);
    	int snmpTrap = (getSelectAlertActions.isSnmpTrap()?1:0);
    	int enable = (isEnabled()?1:0);
    	String Description = "";
		if(getSavedAlertRulesInfo.getDescription()!=null){
			Description = getSavedAlertRulesInfo.getDescription();
		}
    	String Community = "";
        if(getSelectAlertActions.getCommunity()!=null){
        	Community = getSelectAlertActions.getCommunity();
        }
        String Address = "";
        if(getSelectAlertActions.getAddress()!=null){
        	Address =  getSelectAlertActions.getAddress();
        }
         
        String targetInstance = "";
		if(getRegulationSettings.getInstances()!=null)
			targetInstance = getRegulationSettings.getInstances();
		if(targetInstance.equals("")){
			isValidRule = false;
		}
    	String FinalRulesQuery = "INSERT INTO [SQLcompliance]..[AlertRules] ("
    			+ "name,"
    			+ "description,"
    			+ "alertLevel,"
    			+ "alertType,"
    			+ "targetInstances,"
    			+ "enabled,"
    			+ "message,"
    			+ "logMessage,"
    			+ "emailMessage,"
    			+ "snmpTrap,"
    			+ "snmpServerAddress,"
    			+ "snmpPort,"
    			+ "snmpCommunity)"
    			+ " VALUES(" 
    			+ " '" + getSavedAlertRulesInfo.getName() + "',"
    			+ " '" + Description + "',"
    			+ " " + getSavedAlertRulesInfo.getAlertLevel() + ","
    			+ " " + getSavedAlertRulesInfo.getAlertType() +","
    			+ " '" + targetInstance  + "',"
    			+ " " + enable  + ","
    			+ " '" + getSelectAlertActions.getMessage()  + "',"
    			+ " " + logMessage +","
    			+ " " + emailMessage + ","
    			+ " " + snmpTrap + ","
    			+ "'" + Address + "',"
    			+ " " + getSelectAlertActions.getPort() +","
    			+ " '" + Community + "');"
    			+ " DECLARE @IDENTITY INT SELECT @IDENTITY = MAX(ruleId) FROM [SQLcompliance]..[AlertRules];";
    	
    	if (getRegulationSettings.getMatchString()!=null && (!getRegulationSettings.getMatchString().isEmpty())){
    		if(getRegulationSettings.getMatchString().indexOf("(0)")!= -1)
    			isValidRule = false;
        	 FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
            	     + getSavedAlertRulesInfo.getFieldId()
            	     + ", '"
            	     + getRegulationSettings.getMatchString()
            	     + "');";
        	}
    	
    	boolean specifiedLoginCheck = getRegulationSettings.isLoginName();
		String loginMatchString = "";
    	if(getRegulationSettings.getLoginMatchString() != null){
    		
			loginMatchString = getRegulationSettings.getLoginMatchString();
		}
    	if (specifiedLoginCheck) {
			if(loginMatchString.equals("") || loginMatchString.contains("blanks(1)0count(1)0")){
				isValidRule = false;
			}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getLoginFieldId()
					+ ", '"
					+ loginMatchString + "');";
		}
    	
    	boolean specifiedAppCheck = getRegulationSettings.isApplicationName();
		String appMatchString = "";
		if(getRegulationSettings.getAppMatchString() != null){
			appMatchString = getRegulationSettings.getAppMatchString();
		}
		if (specifiedAppCheck) {
			if(appMatchString.equals("") || appMatchString.contains("blanks(1)0count(1)0")){
				isValidRule = false;
			}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getAppFieldId()
					+ ", '"
					+ appMatchString + "');";
		}
	boolean specifiedRowCount = getRegulationSettings
		.isRowCountWithTimeInterval();
		String rowCountMatchString = "";
		if (getRegulationSettings.getRowCountMatchString() != null) {
			rowCountMatchString = getRegulationSettings
					.getRowCountMatchString();
		}	
		long fieldId = 0;
		if (Sessions.getCurrent().getAttribute("conditionEvents") != null) {
			conditionEvents = (List<CMAlertRulesCondition>) Sessions
					.getCurrent().getAttribute("conditionEvents");
			for (int i = 0; i < conditionEvents.size(); i++) 
			{
				fieldId = conditionEvents.get(i).getFieldId();
			}
		}
	if (specifiedRowCount) {
			if ((rowCountMatchString.contains("rowcount(0)")||rowCountMatchString.equals("")) && fieldId != 1 && fieldId != 2 && fieldId != 3) {
				isValidRule = false;
			}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getRowCountFieldId()
					+ ", '"
					+ rowCountMatchString + "');";
		}
		return FinalRulesQuery;
	}

	public String GenerateDataAlertRulesUpdateQuery() {
		int logMessage = (getSelectAlertActions.isWindowEventLogEntry() ? 1 : 0);
		int emailMessage = (getSelectAlertActions.isEmailNotification() ? 1 : 0);
		int snmpTrap = (getSelectAlertActions.isSnmpTrap() ? 1 : 0);
		int enable = (isEnabled() ? 1 : 0);
		String Description = "";
		if (getSavedAlertRulesInfo.getDescription() != null) {
			Description = getSavedAlertRulesInfo.getDescription();
		}
		String Address = "";
		if (getSelectAlertActions.getAddress() != null) {
			Address = getSelectAlertActions.getAddress();
		}
		String Community = "";
		if (getSelectAlertActions.getCommunity() != null) {
			Community = getSelectAlertActions.getCommunity();
		}
		String targetInstance = "";
		if (getRegulationSettings.getInstances() != null)
			targetInstance = getRegulationSettings.getInstances();
		if (targetInstance.equals("")) {
			isValidRule = false;
		}
		String FinalRulesQuery = "Update [SQLcompliance]..[AlertRules] Set name= '"
				+ getSavedAlertRulesInfo.getName()
				+ "',"
				+ "description = '"
				+ Description
				+ "',"
				+ "alertLevel = "
				+ getSavedAlertRulesInfo.getAlertLevel()
				+ ","
				+ "alertType = "
				+ getSavedAlertRulesInfo.getAlertType()
				+ ","
				+ "targetInstances = '"
				+ targetInstance
				+ "',"
				+ "enabled = "
				+ enable
				+ ","
				+ "message = '"
				+ getSelectAlertActions.getMessage()
				+ "',"
				+ "logMessage = "
				+ logMessage
				+ ","
				+ "emailMessage = "
				+ emailMessage
				+ ","
				+ "snmpTrap = "
				+ snmpTrap
				+ ","
				+ "snmpServerAddress = '"
				+ Address
				+ "',"
				+ "snmpPort = "
				+ getSelectAlertActions.getPort()
				+ ","
				+ "snmpCommunity = '"
				+ Community
				+ "' WHERE ruleId = "
				+ ruleId + ";";
		if(getRegulationSettings.getMatchString().indexOf("(0)")!= -1)
			isValidRule = false;
		FinalRulesQuery += "Update  [SQLcompliance].[dbo].[AlertRuleConditions] set fieldId = "
				+ getSavedAlertRulesInfo.getFieldId()
				+ ", matchString = '"
				+ getRegulationSettings.getMatchString()
				+ "' where ruleId = "
		+ ruleId
		+ " and fieldId = "
		+ getSavedAlertRulesInfo.getFieldId() + ";";
		
		String appMatchString = "";
		boolean specifiedAppCheck = getRegulationSettings.isApplicationName();
		if(getRegulationSettings.getAppMatchString() != null){
			appMatchString = getRegulationSettings.getAppMatchString();
		}
		if (specifiedAppCheck) {
			if(appMatchString.equals("") || appMatchString.contains("blanks(1)0count(1)0")){
				isValidRule = false;
			}
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ getRegulationSettings.getAppFieldId()
					+ " AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getRegulationSettings.getAppFieldId()
					+ ", matchString = '"
					+ appMatchString
					+ "'where ruleId = "
					+ ruleId
					+ " and fieldId = "
					+ getRegulationSettings.getAppFieldId()
					+ ""
					+ " END "
					+ " ELSE "
					+ " BEGIN "
					+ "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES ("
					+ ruleId
					+ ", "
					+ getRegulationSettings.getAppFieldId()
					+ ", '"
					+ appMatchString
					+ "')"
					+ " END;";
		}

		else {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ "15 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 15"
					+ " END;";
		}

		boolean specifiedLoginCheck = getRegulationSettings.isLoginName();
		String loginMatchString = "";
		if (getRegulationSettings.getLoginMatchString() != null) {
			loginMatchString = getRegulationSettings.getLoginMatchString();
		}
		if (specifiedLoginCheck) {
			if (loginMatchString.equals("") ||loginMatchString.contains("blanks(1)0count(1)0")) {
				isValidRule = false;
			}
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ getRegulationSettings.getLoginFieldId()
					+ " AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getRegulationSettings.getLoginFieldId()
					+ ", matchString = '"
					+ loginMatchString
					+ "'where ruleId = "
					+ ruleId
					+ " and fieldId = "
					+ getRegulationSettings.getLoginFieldId()
					+ ""
					+ " END "
					+ " ELSE "
					+ " BEGIN "
					+ "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES ("
					+ ruleId
					+ ", "
					+ getRegulationSettings.getLoginFieldId()
					+ ", '"
					+ loginMatchString
					+ "')" + " END;";
		}

		else {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ "16 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 16"
					+ " END;";
		}

	

		boolean specifiedRowCountCheck = getRegulationSettings
				.isRowCountWithTimeInterval();
		String rowCountMatchString = "";
		if (getRegulationSettings.getRowCountMatchString() != null) {
			rowCountMatchString = getRegulationSettings
					.getRowCountMatchString();
		}
		long fieldId = 0;
		if (Sessions.getCurrent().getAttribute("conditionEvents") != null) {
			conditionEvents = (List<CMAlertRulesCondition>) Sessions
					.getCurrent().getAttribute("conditionEvents");
			for (int i = 0; i < conditionEvents.size(); i++) 
			{
				fieldId = conditionEvents.get(i).getFieldId();
			}
		}
		if (specifiedRowCountCheck) {
			if ((rowCountMatchString.contains("rowcount(0)")||rowCountMatchString.equals(""))&& fieldId != 0 && fieldId != 2 && fieldId != 3) {
				isValidRule = false;
			}
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ getRegulationSettings.getRowCountFieldId()
					+ " AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getRegulationSettings.getRowCountFieldId()
					+ ", matchString = '"
		    + rowCountMatchString
					+ "'where ruleId = "
					+ ruleId
					+ " and fieldId = "
					+ getRegulationSettings.getRowCountFieldId()
					+ ""
					+ " END "
					+ " ELSE "
					+ " BEGIN "
					+ "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES ("
					+ ruleId
					+ ", "
					+ getRegulationSettings.getRowCountFieldId()
					+ ", '"
					+ rowCountMatchString
					+ "')" + " END;";
		}

		else {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ "14 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 14"
					+ " END;";
		}

		return FinalRulesQuery;
	}

    @Override
    public void onFinish(AddDataAlertRulesSaveEntity wizardSaveEntity) {
    	isValidRule = true;
    	String QueryBuilder = "";
    	if(getSelectAlertActions.getMessage().length()<=2500){
	        if(Sessions.getCurrent().getAttribute("QueryType")!=null && Sessions.getCurrent().getAttribute("QueryType").equals("Update")){
	        	if(Sessions.getCurrent().getAttribute("alertRuleId")!=null){
	        		getSavedAlertRulesInfo = wizardSaveEntity.getNewEventAlertRules();
	            	getRegulationSettings = wizardSaveEntity.getRegulationSettings();
	            	getSelectAlertActions = wizardSaveEntity.getSelectAlertActions();
	            	Map<Long,Boolean> ruleIdAndEnable=(Map<Long,Boolean>)Sessions.getCurrent().getAttribute("alertRuleId"); 
	        		ruleId=ruleIdAndEnable.entrySet().iterator().next().getKey();
	        		setEnabled(ruleIdAndEnable.get(ruleId));
	            	Sessions.getCurrent().removeAttribute("alertRuleId");
	        	}
	
	        	QueryBuilder =  GenerateDataAlertRulesUpdateQuery();
	        	logType = 55;
	    	}
	    	else
	    	{
	    		QueryBuilder = GenerateEventAlertRulesQuery();
	    		logType = 53;
	    	}
	        
	        String ruleName = getSavedAlertRulesInfo.getName();
			String desc = (getSavedAlertRulesInfo.getDescription() != null) ? getSavedAlertRulesInfo
					.getDescription() : "";
			String logEntry = "";
			dataAlertRuleDescription = dataAlertRuleDescription.replaceAll("\t",
					"");
			dataAlertRuleDescription = "Name:  " + ruleName + "\r\nDescription:  "
					+ desc + "\r\n\r\nRule:  " + dataAlertRuleDescription;
			logEntry = "INSERT INTO {0} (eventTime, logType, logUser, logSqlServer, logInfo) "
					+ "VALUES (GETUTCDATE(),"
					+ logType
					+ ",{1},'','"
					+ dataAlertRuleDescription + "');";
			QueryBuilder += logEntry;
	        InsertEventAlertRules(QueryBuilder);
	        if(!isValidRule){
	        	WebUtil.showWarningBoxWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.INVALID_ALERT_RULE),
	        			ELFunctions.getLabel(SQLCMI18NStrings.INVALID_RULE_TITLE));
	        }
	    }
        else{
			WebUtil.showWarningBoxWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.INVALID_DATA_ALERT_MESSAGE),
					"Error");
		}

		Sessions.getCurrent().removeAttribute("QueryType");
		Sessions.getCurrent().removeAttribute("columnName");
        Sessions.getCurrent().removeAttribute("tableName");
  		Sessions.getCurrent().removeAttribute("dbName");
  		Sessions.getCurrent().removeAttribute("serverName");
        Sessions.getCurrent().removeAttribute("QueryTypeForColumn");
        Sessions.getCurrent().removeAttribute("specifyAlertMessage");
        if(Sessions.getCurrent().getAttribute("isValidSnmpAddress")!=null){
			Sessions.getCurrent().removeAttribute("isValidSnmpAddress");
        }
        if(Sessions.getCurrent().getAttribute("set_messageTitle")!=null){
    		Sessions.getCurrent().removeAttribute("set_messageTitle");
    	}    	
    	
		if(Sessions.getCurrent().getAttribute("set_messageBody")!=null){
			Sessions.getCurrent().removeAttribute("set_messageBody");
		}
		if(Sessions.getCurrent().getAttribute("addressList")!=null){
			Sessions.getCurrent().removeAttribute("addressList");
		}
		
		if(Sessions.getCurrent().getAttribute("mailAddress")!=null){
			Sessions.getCurrent().removeAttribute("mailAddress");
		}
		if(Sessions.getCurrent().getAttribute("RuleTypeAccess")!=null){
			Sessions.getCurrent().removeAttribute("RuleTypeAccess");
		}
		
		
        String uri = "instancesAlertsRule";
        uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
        Executions.sendRedirect(uri);
    }
    public void InsertEventAlertRules(String insertdDataAlertRulesRequest){
    	insertQueryData = new InsertQueryData();
    	insertQueryData.setDataQuery(insertdDataAlertRulesRequest);
    	try {
    		AlertRulesFacade.insertStatusAlertRules(insertQueryData);
    	} catch (RestException e) {
    		WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
    	}
    }

	@Override
	public String getHelpUrl() {
		return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
	}
}