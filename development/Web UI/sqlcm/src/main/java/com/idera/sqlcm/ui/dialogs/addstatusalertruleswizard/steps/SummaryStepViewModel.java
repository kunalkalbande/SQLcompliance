package com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps;

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

import com.google.common.base.Joiner;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.SelectAlertActions;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps.SummaryStepViewModel;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.NewStatusAlertRulesData;

public class SummaryStepViewModel extends AddWizardStepBase {

	NewStatusAlertRulesData newStatusRules;
	SelectAlertActions selectAlertActions;
	InsertQueryData insertQueryData;
	private String statusAlertRuleDescription;
	public static final String ZUL_PATH = "~./sqlcm/dialogs/addstatusalertruleswizard/steps/summary-step.zul";
	boolean enabled;
	private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCondition> conditionEvents;
	NewStatusAlertRulesData newStatusAlertRulesData;
	Long ruleId;
	String margin = "\t";
	String newLine = "\r\n";
	public String generateText;
	public String generatePercentage;
	public String generateAlertRemain;
	public String alertMessage;
	public String alertMessageValue;
	public String windowEventValue;
	public String windowEventName;
	public String SnmpValue;
	public String generateGb;
	public String snmpName;
	public int logType;
	@Wire
	Label snmpAddressText,snmpAddress, snmpPort,snmpPortText, snmpCommunity,snmpCommunityText;
	public String getSnmpName() {
		return snmpName;
	}

	public void setSnmpName(String snmpName) {
		this.snmpName = snmpName;
	}

	public String getGenerateGb() {
		return generateGb;
	}

	public void setGenerateGb(String generateGb) {
		this.generateGb = generateGb;
	}

	public String getSnmpValue() {
		return SnmpValue;
	}

	public void setSnmpValue(String snmpValue) {
		SnmpValue = snmpValue;
	}
	@Wire
	Label generateGbId;
public Label getGenerateGbId() {
		return generateGbId;
	}

	public void setGenerateGbId(Label generateGbId) {
		this.generateGbId = generateGbId;
	}
@Wire
Label sqlSnmpIdAll;
	public Label getSqlSnmpIdAll() {
	return sqlSnmpIdAll;
}
	@Wire
	Label sqlSnmpIdSpecified;

public Label getSqlSnmpIdSpecified() {
		return sqlSnmpIdSpecified;
	}

	public void setSqlSnmpIdSpecified(Label sqlSnmpIdSpecified) {
		this.sqlSnmpIdSpecified = sqlSnmpIdSpecified;
	}

public void setSqlSnmpIdAll(Label sqlSnmpIdAll) {
	this.sqlSnmpIdAll = sqlSnmpIdAll;
}
	@Wire 
	Label sqlWindowEventIdUnspecified,sqlWindowEventIdSpecified,sqlWindowEventIdAll;
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

	public String getWindowEventName() {
		return windowEventName;
	}

	public void setWindowEventName(String windowEventName) {
		this.windowEventName = windowEventName;
	}

	public String getWindowEventValue() {
		return windowEventValue;
	}

	public void setWindowEventValue(String windowEventValue) {
		this.windowEventValue = windowEventValue;
	}

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

	@Wire
	Label sqlAlertIdUnspecified,sqlAlertIdSpecified,sqlAlertIdAll;
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

	public String getGenerateAlertRemain() {
		return generateAlertRemain;
	}

	public void setGenerateAlertRemain(String generateAlertRemain) {
		this.generateAlertRemain = generateAlertRemain;
	}

	public String getGeneratePercentage() {
		return generatePercentage;
	}

	public void setGeneratePercentage(String generatePercentage) {
		this.generatePercentage = generatePercentage;
	}

	public String getGenerateText() {
		return generateText;
	}

	public void setGenerateText(String generateText) {
		this.generateText = generateText;
	}

	public String generateValue;
	public String getGenerateValue() {
		return generateValue;
	}

	public void setGenerateValue(String generateValue) {
		this.generateValue = generateValue;
	}

	public String generateAlert;
	
	public String generateAlertText;
	
	public String getGenerateAlertText() {
		return generateAlertText;
	}

	public void setGenerateAlertText(String generateAlertText) {
		this.generateAlertText = generateAlertText;
	}

	public String getGenerateAlertTextRem() {
		return generateAlertTextRem;
	}

	public void setGenerateAlertTextRem(String generateAlertTextRem) {
		this.generateAlertTextRem = generateAlertTextRem;
	}
	public String generateAlertTextRem;
	 public String getGenerateAlert() {
		return generateAlert;
	}

	public void setGenerateAlert(String generateAlert) {
		this.generateAlert = generateAlert;
	}

	@Wire
	 Checkbox enableRuleNow;

	public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
	}

	public SummaryStepViewModel() {
		super();		
	}

	public String getStatusAlertRuleDescription() {
		return statusAlertRuleDescription;
	}

	public void setStatusAlertRuleDescription(String statusAlertRuleDescription) {
		this.statusAlertRuleDescription = statusAlertRuleDescription;
	}

	/*@Command("onCheck")
	public void onCheck(@BindingParam("target") Checkbox target) {
		String chkName = target.getLabel();
		if (target.isChecked()) {
			setEnabled(true);
		} else {
			setEnabled(false);
		}
	}*/
	
	@Override
	public void onCancel(AddStatusAlertRulesSaveEntity wizardSaveEntity) {
		if (Sessions.getCurrent().getAttribute("QueryType") != null) {
			Sessions.getCurrent().removeAttribute("QueryType");
		}
		 String uri = "instancesAlertsRule";
	     uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
	     Executions.sendRedirect(uri);
	}

	@Override
	public void doOnShow(AddStatusAlertRulesSaveEntity wizardSaveEntity) {
		generateAlert = "";
		generateAlertTextRem = "";
		generateAlertText = "";
		generateAlertRemain = "";
		generatePercentage = "";
		newStatusAlertRulesData = wizardSaveEntity.getNewStatusAlertRulesData();
		selectAlertActions = wizardSaveEntity.getSelectAlertActions();
		int AlertLevel = newStatusAlertRulesData.getAlertLevel();
		setEnabled(true);
    	String description="Generate a ";
    	generateText="Generate a";
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
    			generateValue=" "+"Severe";
    			description +="Severe";
    			break;	
    		}
    	}
		generateAlertText=" "+"alert ";
    	description +=" alert ";   
    	
		description += newLine;
    	description +="when the ";
    	generateAlertText+=" "+" when the";
		newStatusRules = wizardSaveEntity.getNewStatusAlertRulesData();
		switch (newStatusRules.getFieldId()) {
		case 1:
			generateAlert=" "+"Agent trace directory status";
			description += "Agent trace directory status";
			description += newLine;
			description += "meets or exceeds "
					+ newStatusAlertRulesData.getMatchString()
					+ "% of the trace directory size limit";
			
			generateAlertTextRem =" "+"meets or exceeds";
			generatePercentage=" "+newStatusAlertRulesData.getMatchString()+"%";
			generateAlertRemain=" "+" of the trace directory size limit";
			generateGbId.setVisible(true);
			break;
		case 2:
			generateAlert=" "+"Collection Server trace directory status";
			description += "Collection Server trace directory status";
			description += newLine;
			description += " meets or exceeds the trace directory size limit of "
					+ newStatusAlertRulesData.getMatchString() + "GB";
			generateAlertTextRem+=" "+"meets or exceeds the trace directory size limit of "+" ";
			generatePercentage =" "+  newStatusAlertRulesData.getMatchString() + "GB";
			generateAlertRemain="";
			generateGbId.setVisible(true);
			break;
		case 3:
			generateAlert ="Collection Server status";
			generateAlertTextRem =" indicates no heartbeat has been received from the SQLcompliance Agent";
			description += "Collection Server status";
			description += newLine;
			description += "indicates no heartbeat has been received from the SQLcompliance Agent";
			generateGbId.setVisible(true);
			break;
		case 4:
			generateAlert ="event database status";
			description += "event database status";
			description += newLine;
			description += "meets or exceeds the event database size limit of "
					+ newStatusAlertRulesData.getMatchString() + "GB";
			generateAlertTextRem=" meets or exceeds the event database size limit of";
			generatePercentage=" "+newStatusAlertRulesData.getMatchString() + "GB";
			generateGbId.setVisible(true);
			break;
		case 5:
			generateAlert ="Agent status";
			generateAlertTextRem="indicates the SQLcompliance Agent cannot connect to a SQL Server";
			description += "Agent status";
			description += newLine;
			description += "indicates the SQLcompliance Agent cannot connect to a SQL Server";
			generateGbId.setVisible(false);
			break;
		}
		String specifiedEmail = null;
		if (selectAlertActions.isEmailMessage()) {
			if (Sessions.getCurrent().getAttribute("addressList") != null) {
				String[] mailAddresses = (String[]) Sessions.getCurrent()
						.getAttribute("addressList");
				specifiedEmail = Joiner.on(",").skipNulls().join(mailAddresses)
						.replace(",", ",");
			}
			if (specifiedEmail != null && (!specifiedEmail.isEmpty())) {
				alertMessage=newLine+margin; 
				description += newLine + margin;
				description += "send an alert message to";
				description += newLine + margin;
				alertMessage+="send an alert message to";
				alertMessageValue=" "+specifiedEmail;
				description += specifiedEmail;
				sqlAlertIdSpecified.setValue(alertMessage);
				sqlAlertIdAll.setValue(alertMessageValue);
				sqlAlertIdAll.setStyle("color:blue");
			} else {
				alertMessage=newLine;
				alertMessage+="send an alert message to";
				description += newLine;
				description += "send an alert message to";
				description += newLine + margin;
				alertMessageValue=newLine;
				alertMessageValue+="specified addresses";
				description += "specified addresses ";	
				sqlAlertIdSpecified.setValue(alertMessage);
				sqlAlertIdAll.setValue(alertMessageValue);
				sqlAlertIdAll.setStyle("color:red");
			}
			sqlAlertIdSpecified.setVisible(true);
			sqlAlertIdAll.setVisible(true);
		}
		else
		{
			sqlAlertIdSpecified.setVisible(false);
			sqlAlertIdAll.setVisible(false);
		
		}
		
		boolean specifiedlogEntry = selectAlertActions.isLogMessage();
		if (specifiedlogEntry == true) {
			boolean specifiedEmailCheck = selectAlertActions.isEmailMessage();
			if (specifiedEmailCheck == false) {				
				windowEventValue=newLine;
				windowEventValue+="Send an alert message to";
				description += newLine + margin;
				description += "Send an alert message to";
				description += newLine + margin;
				description += "Windows Event Log entry of type  ";
				windowEventValue+="Windows Event Log entry of type";				

			} else {
				windowEventValue=newLine;
				windowEventValue+="and Windows Event Log entry of type";
				description += newLine + margin;
				description += "and Windows Event Log entry of type  ";	
			}
			
			description += selectAlertActions.getEventLogEntry();
		windowEventName=newLine;
		windowEventName+=selectAlertActions.getEventLogEntry();
		sqlWindowEventIdSpecified.setValue(windowEventValue);
		sqlWindowEventIdAll.setValue(windowEventName);
		sqlWindowEventIdAll.setStyle("color:blue");
		sqlWindowEventIdSpecified.setVisible(true);
		sqlWindowEventIdAll.setVisible(true);
		}else
		{
			sqlWindowEventIdSpecified.setVisible(false);
			sqlWindowEventIdAll.setVisible(false);			
		}
		/*boolean specifiedSNMPTrap = selectAlertActions.isSnmpTrap();
		if (specifiedSNMPTrap == true) {
			description += newLine + margin;
			description += "specified network management console";
			SnmpValue=newLine;
			SnmpValue+="specified ";
			snmpName=" "+"network management console";
			sqlSnmpIdAll.setVisible(true);
			sqlSnmpIdSpecified.setVisible(true);
		}
		else
		{
			sqlSnmpIdAll.setVisible(false);	
			sqlSnmpIdSpecified.setVisible(false);
		}*/
		statusAlertRuleDescription = description;
		

		boolean specifiedSnmpLog = selectAlertActions.isSnmpTrap();		
		if(specifiedSnmpLog){
			if(selectAlertActions.isEmailMessage() || specifiedlogEntry){
				snmpAddressText.setValue("SNMP trap address ");
			}
			else{
				snmpAddressText.setValue("Send an alert message to \r\n SNMP trap address ");
			}
			
			if(selectAlertActions.getAddress()!=null && !selectAlertActions.getAddress().isEmpty()){
				snmpAddress.setValue(selectAlertActions.getAddress());
				snmpAddress.setStyle("color: blue");
			}
			else{
				snmpAddress.setValue("specified address.");
				snmpAddress.setStyle("color: red");
			}
			snmpPortText.setValue("SNMP trap port ");
			snmpPort.setValue(""+selectAlertActions.getPort());
			snmpCommunityText.setValue("SNMP trap community ");
			if(selectAlertActions.getCommunity()!=null && 
					!selectAlertActions.getCommunity().isEmpty()){
				snmpCommunity.setValue(selectAlertActions.getCommunity());
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


		BindUtils.postNotifyChange(null, null, SummaryStepViewModel.this,"*");
		BindUtils.postNotifyChange(null, null, SummaryStepViewModel.this,
				"statusAlertRuleDescription");
	}

	public void initializer(List<CMAlertRules> alertRules) {
		if (alertRules.get(0).getRuleId() != null) {
			if (alertRules.get(0).isEnabled()) {
    			setEnabled(true);
    		}
    		else{
    			setEnabled(false);
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
		return ELFunctions
				.getLabel(SQLCMI18NStrings.SQL_FINISH_STATUS_ALERT_RULES_TIPS);
	}

	public String GenerateStatusAlertRulesQuery() {
		int logMessage = (selectAlertActions.isLogMessage() ? 1 : 0);
		int emailMessage = (selectAlertActions.isEmailMessage() ? 1 : 0);
		int snmpTrap = (selectAlertActions.isSnmpTrap() ? 1 : 0);
		int enable = (isEnabled() ? 1 : 0);
		String Description ="";
		if (newStatusRules.getDescription() != null) {
			Description =newStatusRules.getDescription();
		}
		String Community = "";
		if (selectAlertActions.getCommunity() != null) {
			Community = selectAlertActions.getCommunity();
		}
		String Address = "";
		if (selectAlertActions.getAddress() != null) {
			Address = selectAlertActions.getAddress();
		}

		String FinalRulesQuery = "INSERT INTO [SQLcompliance]..[AlertRules] ("
				+ "name," + "description," + "alertLevel," + "alertType,"
				+ "targetInstances," + "enabled," + "message," + "logMessage,"
				+ "emailMessage," + "snmpTrap," + "snmpServerAddress,"
				+ "snmpPort," + "snmpCommunity)" + " VALUES(" + " '"
				+ newStatusRules.getName()
				+ "',"
				+ " '"
				+ Description
				+ "',"
				+ " "
				+ newStatusRules.getAlertLevel()
				+ ","
				+ " "
				+ newStatusRules.getAlertType()
				+ ","
				+ " '"
				+ newStatusRules.getTargetInstances()
				+ "',"
				+ " "
				+ enable
				+ ","
				+ " '"
				+ selectAlertActions.getMessage()
				+ "',"
				+ " "
				+ logMessage
				+ ","
				+ " "
				+ emailMessage
				+ ","
				+ " "
				+ snmpTrap
				+ ","
				+ " '"
				+ Address
				+ "',"
				+ " "
				+ selectAlertActions.getPort()
				+ ","
				+ " '"
				+ Community
				+ "');"
				+ " DECLARE @IDENTITY INT SELECT @IDENTITY = MAX(ruleId) FROM [SQLcompliance]..[AlertRules];";

		FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
				+ newStatusRules.getFieldId()
				+ ", '"
				+ newStatusRules.getMatchString() + "');";

		return FinalRulesQuery;
	}

	public String GenerateStatusAlertRulesUpdateQuery() {
		int logMessage = (selectAlertActions.isLogMessage() ? 1 : 0);
		int emailMessage = (selectAlertActions.isEmailMessage() ? 1 : 0);
		int snmpTrap = (selectAlertActions.isSnmpTrap() ? 1 : 0);
		int enable = (isEnabled() ? 1 : 0);
		String Description ="";
		if (newStatusRules.getDescription() != null) {
			Description =newStatusRules.getDescription();
		}
		String Community = "";
		if (selectAlertActions.getCommunity() != null) {
			Community = selectAlertActions.getCommunity();
		}
		String Address = "";
		if (selectAlertActions.getAddress() != null) {
			Address = selectAlertActions.getAddress();
		}
		String FinalRulesQuery = "Update [SQLcompliance]..[AlertRules] Set name= '"
				+ newStatusRules.getName()
				+ "',"
				+ "description = '"
				+ Description
				+ "',"
				+ "alertLevel = "
				+ newStatusRules.getAlertLevel()
				+ ","
				+ "alertType = "
				+ newStatusRules.getAlertType()
				+ ","
				+ "targetInstances = '"
				+ newStatusRules.getTargetInstances()
				+ "',"
				+ "enabled = "
				+ enable
				+ ","
				+ "message = '"
				+ selectAlertActions.getMessage()
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
				+ selectAlertActions.getPort()
				+ ","
				+ "snmpCommunity = '"
				+ Community
				+ "' WHERE ruleId = "
				+ ruleId + ";";

		FinalRulesQuery += "Update  [SQLcompliance].[dbo].[AlertRuleConditions] set fieldId = "
				+ newStatusRules.getFieldId()
				+ ", matchString = '"
				+ newStatusRules.getMatchString()
				+ "' WHERE ruleId = "
				+ ruleId + ";";
		return FinalRulesQuery;
	}

	@Override
	public void onFinish(AddStatusAlertRulesSaveEntity wizardSaveEntity) {
		newStatusRules = wizardSaveEntity.getNewStatusAlertRulesData();
		selectAlertActions = wizardSaveEntity.getSelectAlertActions();
		if(selectAlertActions.getMessage().length()<=2500){
			String QueryBuilder;
			if (Sessions.getCurrent().getAttribute("QueryType")!=null 
					&& Sessions.getCurrent().getAttribute("QueryType").equals("Update")) {
				if(Sessions.getCurrent().getAttribute("alertRuleId")!=null){
					newStatusAlertRulesData = wizardSaveEntity.getNewStatusAlertRulesData();
					selectAlertActions = wizardSaveEntity.getSelectAlertActions();
	            	Map<Long,Boolean> ruleIdAndEnable=(Map<Long,Boolean>)Sessions.getCurrent().getAttribute("alertRuleId"); 
	        		ruleId=ruleIdAndEnable.entrySet().iterator().next().getKey();
	        		setEnabled(ruleIdAndEnable.get(ruleId));
	            	Sessions.getCurrent().removeAttribute("alertRuleId");
	        	}
				QueryBuilder = GenerateStatusAlertRulesUpdateQuery();
				logType = 55;
			} else {
				QueryBuilder = GenerateStatusAlertRulesQuery();
				logType = 53;
			}
			
			String ruleName = newStatusAlertRulesData.getName();
			String desc = (newStatusAlertRulesData.getDescription() != null) ? newStatusAlertRulesData
					.getDescription() : "";
			String logEntry = "";
			statusAlertRuleDescription = statusAlertRuleDescription.replaceAll("\t","");
			statusAlertRuleDescription = "Name:  " + ruleName + "\r\nDescription:  "
					+ desc + "\r\n\r\nRule:  " + statusAlertRuleDescription;
			logEntry = "INSERT INTO {0} (eventTime, logType, logUser, logSqlServer, logInfo) "
					+ "VALUES (GETUTCDATE(),"
					+ logType
					+ ",{1},'','"
					+ statusAlertRuleDescription + "');";
			QueryBuilder += logEntry;
			InsertStatusAlertRules(QueryBuilder);
		}
		else{
			WebUtil.showWarningBoxWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.INVALID_STATUS_ALERT_MESSAGE),
					"Error");
		}

		
		
		Sessions.getCurrent().removeAttribute("QueryType");

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
		Sessions.getCurrent().removeAttribute("specifyAlertMessage");
		 String uri = "instancesAlertsRule";
	     uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
	     Executions.sendRedirect(uri);
	}

	@Override
	public void onCancel(long instanceId) {

	}

	public void InsertStatusAlertRules(String insertStatusAlertRulesRequest) {
		insertQueryData = new InsertQueryData();
		insertQueryData.setDataQuery(insertStatusAlertRulesRequest);
		try {
			AlertRulesFacade.insertStatusAlertRules(insertQueryData);
		} catch (RestException e) {
			String errorMessage = ELFunctions.getLabel(e.toString());
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
		}
	}
	
	 @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
    }
}