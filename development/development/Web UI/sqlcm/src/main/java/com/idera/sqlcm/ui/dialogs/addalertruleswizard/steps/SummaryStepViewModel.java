package com.idera.sqlcm.ui.dialogs.addalertruleswizard.steps;

import java.util.List;
import java.util.Map;

import mazz.i18n.annotation.I18NMessage;

import org.zkoss.bind.BindUtils;
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
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.facade.SNMPConfigData;
import com.idera.sqlcm.facade.SNMPConfigFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.NewEventAlertRules;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.RegulationSettings;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.SelectAlertActions;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyAppNameViewModel.App;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyDatabaseViewModel.Data;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyHostNameViewModel.Host;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyLoginViewModel.Login;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyObjectsViewModel.Objects;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyPrivilegedUserViewModel.PrivilegedUserName;

public class SummaryStepViewModel extends AddWizardStepBase {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/addalertruleswizard/steps/summary-step.zul";

	private String eventAlertRuleDescription="";

	public NewEventAlertRules getSavedAlertRulesInfo;

	public RegulationSettings getRegulationSettings;
	
	boolean isValid = true;

	int logType;

	@Wire
	Label loginIdSpecified, loginIdUnspecified, loginIdAll, selectedEvent, rowCountThresholdMsgString, rowCountThresholdIdAllMsgString,
	timeIntervalMsgString, timeIntervalIdAllMsgString,
	timeIntervalHoursMsg;;
	
	@Wire
	Label privilegedUserIdSpecified, privilegedUserIdUnspecified;
	
	@Wire
	Label snmpAddressText,snmpAddress, snmpPort,snmpPortText, snmpCommunity,snmpCommunityText;
	
	@Wire
	Label generateIdSpecified, generateAlertIdSpecified, applicationNameIdAll;
	@Wire
	Label generateIdAll, generateBoldIdSpecified, excludeEvents,
			excludeEventsData;

	public Label getGenerateBoldIdSpecified() {
		return generateBoldIdSpecified;
	}

	public void setGenerateBoldIdSpecified(Label generateBoldIdSpecified) {
		this.generateBoldIdSpecified = generateBoldIdSpecified;
	}

	public Label getGenerateIdAll() {
		return generateIdAll;
	}

	public void setGenerateIdAll(Label generateIdAll) {
		this.generateIdAll = generateIdAll;
	}

	public Label getGenerateAlertIdSpecified() {
		return generateAlertIdSpecified;
	}

	public void setGenerateAlertIdSpecified(Label generateAlertIdSpecified) {
		this.generateAlertIdSpecified = generateAlertIdSpecified;
	}

	public Label getGenerateIdSpecified() {
		return generateIdSpecified;
	}

	public void setGenerateIdSpecified(Label generateIdSpecified) {
		this.generateIdSpecified = generateIdSpecified;
	}

	public Label getLoginIdUnspecified() {
		return loginIdUnspecified;
	}

	public void setLoginIdUnspecified(Label loginIdUnspecified) {
		this.loginIdUnspecified = loginIdUnspecified;
	}

	public Label getLoginIdSpecified() {
		return loginIdSpecified;
	}

	public void setLoginIdSpecified(Label loginIdSpecified) {
		this.loginIdSpecified = loginIdSpecified;
	}

	@Wire
	Label sqlServerIdSpecified, sqlServerIdAll;
	@Wire
	Label sqlServerIdUnspecified;

	@Wire
	Label alertMessageValueLabel;

	public Label getSqlServerIdUnspecified() {
		return sqlServerIdUnspecified;
	}

	public void setSqlServerIdUnspecified(Label sqlServerIdUnspecified) {
		this.sqlServerIdUnspecified = sqlServerIdUnspecified;
	}

	public Label getSqlServerIdSpecified() {
		return sqlServerIdSpecified;
	}

	public void setSqlServerIdSpecified(Label sqlServerIdSpecified) {
		this.sqlServerIdSpecified = sqlServerIdSpecified;
	}

	public Label getSqlServerIdAll() {
		return sqlServerIdAll;
	}

	public void setSqlServerIdAll(Label sqlServerIdAll) {
		this.sqlServerIdAll = sqlServerIdAll;
	}

	@Wire
	Label sqlDatabaseIdSpecified, sqlDatabaseIdunspecified, sqlDatabaseIdAll;

	public Label getSqlDatabaseIdAll() {
		return sqlDatabaseIdAll;
	}

	public void setSqlDatabaseIdAll(Label sqlDatabaseIdAll) {
		this.sqlDatabaseIdAll = sqlDatabaseIdAll;
	}

	public Label getSqlDatabaseIdSpecified() {
		return sqlDatabaseIdSpecified;
	}

	public void setSqlDatabaseIdSpecified(Label sqlDatabaseIdSpecified) {
		this.sqlDatabaseIdSpecified = sqlDatabaseIdSpecified;
	}

	@Wire
	Label sqlObjectIdUnspecified, sqlObjectIdSpecified, sqlObjectIdAll;

	public Label getSqlObjectIdAll() {
		return sqlObjectIdAll;
	}

	public void setSqlObjectIdAll(Label sqlObjectIdAll) {
		this.sqlObjectIdAll = sqlObjectIdAll;
	}

	public Label getSqlObjectIdSpecified() {
		return sqlObjectIdSpecified;
	}

	public void setSqlObjectIdSpecified(Label sqlObjectIdSpecified) {
		this.sqlObjectIdSpecified = sqlObjectIdSpecified;
	}

	public Label getSqlObjectIdUnspecified() {
		return sqlObjectIdUnspecified;
	}

	public void setSqlObjectIdUnspecified(Label sqlObjectIdUnspecified) {
		this.sqlObjectIdUnspecified = sqlObjectIdUnspecified;
	}

	@Wire
	Label applicationNameIdUnspecified;

	public Label getApplicationNameIdUnspecified() {
		return applicationNameIdUnspecified;
	}

	public void setApplicationNameIdUnspecified(
			Label applicationNameIdUnspecified) {
		this.applicationNameIdUnspecified = applicationNameIdUnspecified;
	}

	@Wire
	Label hostNameIdUnspecified, hostNameIdSpecified, hostNameIdAll;

	public Label getHostNameIdAll() {
		return hostNameIdAll;
	}

	public void setHostNameIdAll(Label hostNameIdAll) {
		this.hostNameIdAll = hostNameIdAll;
	}

	public Label getHostNameIdSpecified() {
		return hostNameIdSpecified;
	}

	public void setHostNameIdSpecified(Label hostNameIdSpecified) {
		this.hostNameIdSpecified = hostNameIdSpecified;
	}

	public Label getHostNameIdUnspecified() {
		return hostNameIdUnspecified;
	}

	public void setHostNameIdUnspecified(Label hostNameIdUnspecified) {
		this.hostNameIdUnspecified = hostNameIdUnspecified;
	}

	@Wire
	Label applicationNameIdSpecified;

	public Label getApplicationNameIdSpecified() {
		return applicationNameIdSpecified;
	}

	public void setApplicationNameIdSpecified(Label applicationNameIdSpecified) {
		this.applicationNameIdSpecified = applicationNameIdSpecified;
	}

	@Wire
	Label privilegedIdUnspecified, privilegedIdSpecified;

	public Label getPrivilegedIdUnspecified() {
		return privilegedIdUnspecified;
	}

	public void setPrivilegedIdUnspecified(Label privilegedIdUnspecified) {
		this.privilegedIdUnspecified = privilegedIdUnspecified;
	}

	@Wire
	Label accessCheckedIdUnspecified, accessCheckedIdSpecified;

	public Label getAccessCheckedIdSpecified() {
		return accessCheckedIdSpecified;
	}

	public void setAccessCheckedIdSpecified(Label accessCheckedIdSpecified) {
		this.accessCheckedIdSpecified = accessCheckedIdSpecified;
	}

	public Label getAccessCheckedIdUnspecified() {
		return accessCheckedIdUnspecified;
	}

	public void setAccessCheckedIdUnspecified(Label accessCheckedIdUnspecified) {
		this.accessCheckedIdUnspecified = accessCheckedIdUnspecified;
	}

	@Wire
	Label alertMessageIdSpecified, alertMessageUnspecified;

	public Label getAlertMessageUnspecified() {
		return alertMessageUnspecified;
	}

	public void setAlertMessageUnspecified(Label alertMessageUnspecified) {
		this.alertMessageUnspecified = alertMessageUnspecified;
	}

	public Label getAlertMessageIdSpecified() {
		return alertMessageIdSpecified;
	}

	public void setAlertMessageIdSpecified(Label alertMessageIdSpecified) {
		this.alertMessageIdSpecified = alertMessageIdSpecified;
	}

	@Wire
	Label WindowAlertMessageIdSpecified, WindowAlertMessageIdUnspecified,
			WindowAlertMessageIdAll;

	public Label getWindowAlertMessageIdAll() {
		return WindowAlertMessageIdAll;
	}

	public void setWindowAlertMessageIdAll(Label windowAlertMessageIdAll) {
		WindowAlertMessageIdAll = windowAlertMessageIdAll;
	}

	public Label getWindowAlertMessageIdUnspecified() {
		return WindowAlertMessageIdUnspecified;
	}

	public void setWindowAlertMessageIdUnspecified(
			Label windowAlertMessageIdUnspecified) {
		WindowAlertMessageIdUnspecified = windowAlertMessageIdUnspecified;
	}

	public Label getWindowAlertMessageIdSpecified() {
		return WindowAlertMessageIdSpecified;
	}

	public void setWindowAlertMessageIdSpecified(
			Label windowAlertMessageIdSpecified) {
		WindowAlertMessageIdSpecified = windowAlertMessageIdSpecified;
	}

	public SelectAlertActions getSelectAlertActions;
	private String generateValue;
	private String generateText;
	private String generateAlert;
	private String securityChanges;
	private String sqlServerNames;
	private String privilegedValue;
	private String windowAlertMessage;
		
	
	public String rowCountWithTimeIntervalValue;

	public String getRowCountWithTimeIntervalValue() {
		return rowCountWithTimeIntervalValue;
	}
	
	
	public String getWindowAlertMessage() {
		return windowAlertMessage;
	}

	public void setWindowAlertMessage(String windowAlertMessage) {
		this.windowAlertMessage = windowAlertMessage;
	}

	private String windowAlertMessageValue;

	public String getWindowAlertMessageValue() {
		return windowAlertMessageValue;
	}

	public void setWindowAlertMessageValue(String windowAlertMessageValue) {
		this.windowAlertMessageValue = windowAlertMessageValue;
	}

	public String getPrivilegedValue() {
		return privilegedValue;
	}

	public void setPrivilegedValue(String privilegedValue) {
		this.privilegedValue = privilegedValue;
	}

	public String getSqlServerNames() {
		return sqlServerNames;
	}

	public void setSqlServerNames(String sqlServerNames) {
		this.sqlServerNames = sqlServerNames;
	}

	private String sqlServerValue;
	private String sqlDatabaseValue;
	private String sqlObject;
	private String sqlObjectName;
	private String alertMessage;

	public String getAlertMessage() {
		return alertMessage;
	}

	public void setAlertMessage(String alertMessage) {
		this.alertMessage = alertMessage;
	}

	public String getSqlObjectName() {
		return sqlObjectName;
	}

	public void setSqlObjectName(String sqlObjectName) {
		this.sqlObjectName = sqlObjectName;
	}

	private String sqlDatabaseName;
	private String applicationName;
	private String applicationNameValue;
	private String loginName;
	private String loginNameValue;
	private String hostName;
	private String hostNameValue;
	private String privileged;
	private String accessCheckValue;
	private String accessChecked;
	private String alertMessageValue;
	private String privilegedUser;
	private String privilegedUserNameValue;

	public String getPrivilegedUserNameValue() {
		return privilegedUserNameValue;
	}

	public void setPrivilegedUserNameValue(String privilegedUserNameValue) {
		this.privilegedUserNameValue = privilegedUserNameValue;
	}

	public String getPrivilegedUser() {
		return privilegedUser;
	}

	public void setPrivilegedUser(String privilegedUser) {
		this.privilegedUser = privilegedUser;
	}

	public String getAlertMessageValue() {
		return alertMessageValue;
	}

	public void setAlertMessageValue(String alertMessageValue) {
		this.alertMessageValue = alertMessageValue;
	}

	public String getAccessChecked() {
		return accessChecked;
	}

	public void setAccessChecked(String accessChecked) {
		this.accessChecked = accessChecked;
	}

	public String getAccessCheckValue() {
		return accessCheckValue;
	}

	public void setAccessCheckValue(String accessCheckValue) {
		this.accessCheckValue = accessCheckValue;
	}

	public String getPrivileged() {
		return privileged;
	}

	public void setPrivileged(String privileged) {
		this.privileged = privileged;
	}

	public String getHostNameValue() {
		return hostNameValue;
	}

	public void setHostNameValue(String hostNameValue) {
		this.hostNameValue = hostNameValue;
	}

	public String getHostName() {
		return hostName;
	}

	public void setHostName(String hostName) {
		this.hostName = hostName;
	}

	public String getLoginNameValue() {
		return loginNameValue;
	}

	public void setLoginNameValue(String loginNameValue) {
		this.loginNameValue = loginNameValue;
	}

	public String getLoginName() {
		return loginName;
	}

	public void setLoginName(String loginName) {
		this.loginName = loginName;
	}

	public String getApplicationNameValue() {
		return applicationNameValue;
	}

	public void setApplicationNameValue(String applicationNameValue) {
		this.applicationNameValue = applicationNameValue;
	}

	public String getApplicationName() {
		return applicationName;
	}

	public void setApplicationName(String applicationName) {
		this.applicationName = applicationName;
	}

	public String getSqlDatabaseName() {
		return sqlDatabaseName;
	}

	public void setSqlDatabaseName(String sqlDatabaseName) {
		this.sqlDatabaseName = sqlDatabaseName;
	}

	public String getSqlObject() {
		return sqlObject;
	}

	public void setSqlObject(String sqlObject) {
		this.sqlObject = sqlObject;
	}

	public String getSqlDatabaseValue() {
		return sqlDatabaseValue;
	}

	public void setSqlDatabaseValue(String sqlDatabaseValue) {
		this.sqlDatabaseValue = sqlDatabaseValue;
	}

	public String getSqlServerValue() {
		return sqlServerValue;
	}

	public void setSqlServerValue(String sqlServerValue) {
		this.sqlServerValue = sqlServerValue;
	}

	public String getSecurityChanges() {
		return securityChanges;
	}

	public void setSecurityChanges(String securityChanges) {
		this.securityChanges = securityChanges;
	}

	public String getGenerateAlert() {
		return generateAlert;
	}

	public void setGenerateAlert(String generateAlert) {
		this.generateAlert = generateAlert;
	}

	public String getGenerateText() {
		return generateText;
	}

	public void setGenerateText(String generateText) {
		this.generateText = generateText;
	}

	public String getGenerateValue() {
		return generateValue;
	}

	public void setGenerateValue(String generateValue) {
		this.generateValue = generateValue;
	}

	@Wire
	Checkbox enableRuleNow;

	String targetInstance;
	boolean enabled;
	public RegulationSettings rs;
	InsertQueryData insertQueryData;

	String margin = "\t";
	String newLine = "\r\n";

	@I18NMessage("")
	String ALERT_RULE_DESCRIPTION = "SQLCM.Labels.alertsRule.description";

	private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCondition> conditionEvents;
	Long ruleId;

	public SummaryStepViewModel() {
		super();
	}

	public String getEventAlertRuleDescription() {
		return eventAlertRuleDescription;
	}

	public void setEventAlertRuleDescription(String eventAlertRuleDescription) {
		this.eventAlertRuleDescription = eventAlertRuleDescription;
	}

	public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
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
	public void onCancel(AddAlertRulesSaveEntity wizardSaveEntity) {
		if (Sessions.getCurrent().getAttribute("QueryType") != null) {
			Sessions.getCurrent().removeAttribute("QueryType");
			Sessions.getCurrent().removeAttribute("Category");
		}

		String uri = "instancesAlertsRule";
		uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
		Executions.sendRedirect(uri);
	}

	@Override
	public void doOnShow(AddAlertRulesSaveEntity wizardSaveEntity) {
		getSavedAlertRulesInfo = wizardSaveEntity.getNewEventAlertRules();
		getRegulationSettings = wizardSaveEntity.getRegulationSettings();
		getSelectAlertActions = wizardSaveEntity.getSelectAlertActions();
		setEnabled(true);
		String description = "Generate a ";
		generateText = "Generate a";
		int AlertLevel = getSavedAlertRulesInfo.getAlertLevel();
		if (AlertLevel > 0) {
			switch (AlertLevel) {
			case 1:
				generateValue = " " + "Low";
				description += "Low";
				break;
			case 2:
				generateValue = " " + "Medium";

				description += "Medium";
				break;
			case 3:
				generateValue = " " + "High";
				description += "High";
				break;
			case 4:
				generateValue = " " + "Servere";
				description += "Severe";
				break;
			}
		}

		description += " alert ";
		generateAlert = "alert for";

		String EventType;
		EventType = getSavedAlertRulesInfo.getEventFilter();
		if (EventType != null) {
			selectedEvent.setValue("");
			securityChanges =  " " + EventType + " ";
			if(EventType.indexOf("Events")<0){
				selectedEvent.setValue("events ");
			}
			setSecurityChanges(securityChanges);

			description += "\r\n";
			description += margin + "for ";
			description += EventType;
			description += " events";
		}

		String specifiedSQLServer = "specified SQL Server";
		boolean sqlServerCheck = getRegulationSettings.getSQLServer();
		Map<String, Object> targetInstances = getRegulationSettings
				.getTargetInstances();

		if (sqlServerCheck == true) {
			sqlServerValue = "on";
			if (targetInstances != null && targetInstances.size() > 0) {
				String tInstances = targetInstances.keySet().toString();
				sqlServerValue += " SQL Server ";
				sqlServerNames = tInstances.substring(1,
						(tInstances.length() - 1));
				description += "\r\n";
				description += margin + " on SQL Server " + " ";

				description += tInstances.substring(1,
						(tInstances.length() - 1));
				sqlServerIdAll.setStyle("color:blue");
				// sqlServerIdUnspecified.setVisible(true);
				sqlServerIdSpecified.setVisible(true);
				sqlServerIdAll.setVisible(true);

			}

			else {
				sqlServerIdAll.setStyle("color:red");
				sqlServerNames = " specified SQL Servers";
				description += "\r\n";
				description += margin + "on specified SQL Servers  ";
				// sqlServerIdUnspecified.setVisible(true);
				sqlServerIdSpecified.setVisible(true);
				sqlServerIdAll.setVisible(true);
			}
		} else {
			sqlServerValue = " " + "on any SQL Server";
			description += "\r\n";
			description += margin + "on any SQL Server ";
			sqlServerIdSpecified.setStyle("color:black");
			sqlServerIdSpecified.setVisible(true);
			sqlServerIdAll.setVisible(false);
		}

		boolean specifiedDbCheck = getRegulationSettings.getDatabaseName();
		ListModelList<Data> specifiedDatabase = getRegulationSettings
				.getDbNameList();

		if (specifiedDbCheck == true) {
			sqlDatabaseValue = "and";
			description += "\r\n";
			description += margin + "and ";
			sqlDatabaseName = null;
			int count = (specifiedDatabase != null) ? specifiedDatabase
					.getSize() : 0;
			switch (count) {
			case 0:
				sqlDatabaseName = " " + "specified databases";
				description += "specified databases   ";
				sqlDatabaseIdAll.setStyle("color:red");
				break;
			case 1:
				sqlDatabaseValue += " " + "database";
				description += "database ";
				break;
			default:
				sqlDatabaseValue += " " + "databases";
				description += "databases ";
			}
			if (sqlDatabaseName == null) {
				sqlDatabaseName = "";
				sqlDatabaseIdAll.setStyle("color:blue");
			}
			for (int j = 0; j < count; j++) {
				sqlDatabaseName += " "
						+ specifiedDatabase.get(j).getDataBaseName();
				description += specifiedDatabase.get(j).getDataBaseName();
				if (j < specifiedDatabase.getSize() - 1) {
					sqlDatabaseName += " " + ",";
					description += ",";
				}
			}
			sqlDatabaseIdSpecified.setVisible(true);
			sqlDatabaseIdAll.setVisible(true);
		} else {
			sqlDatabaseIdSpecified.setVisible(false);
			sqlDatabaseIdAll.setVisible(false);
		}

		boolean specifiedObCheck = getRegulationSettings.getObjectName();
		ListModelList<Objects> specifiedObject = getRegulationSettings
				.getObjectNameList();
		if (specifiedObCheck == true) {
			sqlObjectName = null;
			sqlObject = " " + "and";
			description += "\r\n";
			description += margin + "and ";
			int count = (specifiedObject != null) ? specifiedObject.getSize()
					: 0;
			switch (count) {
			case 0:
				sqlObjectName = " " + "specified objects";
				description += "specified objects   ";
				sqlObjectIdAll.setStyle("color:red");
				break;
			case 1:
				sqlObject += " " + "object";
				description += "object";

				break;
			default:
				sqlObject += " " + "objects";
				description += "objects ";
			}
			if (sqlObjectName == null) {
				sqlObjectName = "";
				sqlObjectIdAll.setStyle("color:blue");
			}
			for (int j = 0; j < count; j++) {
				sqlObjectName += " " + specifiedObject.get(j).getObjectName();
				description += specifiedObject.get(j).getObjectName();
				if (j < count - 1) {
					sqlObjectName += " " + ",";
					description += ",";
				}
			}
			sqlObjectIdSpecified.setVisible(true);
			sqlObjectIdAll.setVisible(true);
		} else {
			sqlObjectIdSpecified.setVisible(false);
			sqlObjectIdAll.setVisible(false);
		}

		boolean isprivilegedCheck = getRegulationSettings
				.getIsPrivilegedCheck();
		boolean isprivileged = getRegulationSettings.getIsPrivilegedUser();
		if (isprivilegedCheck == true) {
			privileged = " " + "where Privileged User is";
			description += "\r\n";
			description += margin + "where Privileged User is ";
			description += isprivileged;
			privilegedValue = " " + isprivileged;
			privilegedIdUnspecified.setVisible(true);
			privilegedIdUnspecified.setStyle("color:blue");
			privilegedIdSpecified.setVisible(true);
		} else {
			privilegedIdUnspecified.setVisible(false);
			privilegedIdSpecified.setVisible(false);
		}

		boolean accessCheckPassed = getRegulationSettings
				.getAccessCheckPassedChk();
		boolean accessCheck = getRegulationSettings.getAccessCheckPassed();
		if (accessCheckPassed == true) {
			accessChecked = " " + "and Access Check Passed is";
			description += "\r\n";
			description += margin + "and Access Check Passed is ";
			description += accessCheck;
			accessCheckValue = " " + accessCheck;
			accessCheckedIdUnspecified.setVisible(true);
			accessCheckedIdUnspecified.setStyle("color:blue");
			accessCheckedIdSpecified.setVisible(true);
		} else {
			accessCheckedIdUnspecified.setVisible(false);
			accessCheckedIdSpecified.setVisible(false);
		}

		boolean specifiedAppCheck = getRegulationSettings.getApplicationName();
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
				applicationNameIdAll.setStyle("color:red");
			} else {
				applicationNameIdAll.setStyle("color:blue");
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
			applicationNameIdAll.setVisible(true);
		} else {
			applicationNameIdSpecified.setVisible(false);
			applicationNameIdAll.setVisible(false);
		}

		boolean specifiedLoginCheck = getRegulationSettings.getLoginName();
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
				loginIdAll.setStyle("color:red");
			} else {
				loginIdAll.setStyle("color:blue");
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

			loginIdSpecified.setVisible(true);
			loginIdAll.setVisible(true);
		} else {
			loginIdSpecified.setVisible(false);
			loginIdAll.setVisible(false);
		}

	boolean specifiedPrivUserCheck = getRegulationSettings
		.getPrivilegedUserName();
	String inExLog = getRegulationSettings
		.getPrivilegedUserNameMatchString();
	if (specifiedPrivUserCheck == true) {
	    getRegulationSettings.setPrivilegedUserName(true);
		if (inExLog != null && !inExLog.isEmpty()) {
			String excludeLog = inExLog.substring(inExLog.indexOf("include"));
			int charCountLog = Integer.parseInt((excludeLog.substring(
					excludeLog.indexOf("(") + 1, excludeLog.indexOf(")"))));
			int inOrExLog = Integer.parseInt(excludeLog.substring(
					excludeLog.indexOf(")") + 1, excludeLog.indexOf(")")
							+ charCountLog + 1));
		
		ListModelList<PrivilegedUserName> specifiedPrivilegedUser = getRegulationSettings.getPrivilegedUserNameList();
		if (specifiedPrivUserCheck == true) {
			String privilegedMsg = "";
			if (inOrExLog == 0) {
				privilegedUser = "and Privileged User Name not like" + " ";
			} else if (inOrExLog == 1) {
				privilegedUser = "and Privileged User Name like" + " ";
			}
			privilegedUserNameValue = "";
			description += "\r\n";
			description += margin + "and Privileged User Name like ";
		    int count = (specifiedPrivilegedUser != null) ? specifiedPrivilegedUser
			    .getSize() : 0;

		    if (count > 0) {

		    privilegedUserIdUnspecified.setStyle("color:blue");
		    for (int j = 0; j < count; j++) {
			privilegedUserNameValue += " '"
				+ specifiedPrivilegedUser.get(j)
					.getPrivilegedUserName() + "'";
			description += specifiedPrivilegedUser.get(j)
				.getPrivilegedUserName();

			if (j < count - 1) {
			    privilegedUserNameValue += " " + "or";
			    description += "or";
			}
		    }

		    privilegedUserIdSpecified.setVisible(true);
		    privilegedUserIdUnspecified.setVisible(true);
		    privilegedUserIdSpecified.setValue(privilegedMsg);
		    privilegedUserIdUnspecified
			    .setValue(privilegedUserNameValue);
		    } else {

			privilegedUserNameValue = "";
			privilegedUser = "and Privileged User Name like" + " ";
			privilegedUserNameValue = " " + "specified words";
			privilegedUserIdUnspecified.setStyle("color:red");
			privilegedUserIdSpecified.setVisible(true);
			privilegedUserIdUnspecified.setVisible(true);
			privilegedUserIdSpecified.setValue(privilegedUser);
			privilegedUserIdUnspecified
				.setValue(privilegedUserNameValue);
		    }
		}
	    }

	    else {

		privilegedUserNameValue = "";
		privilegedUser = "and Privileged User Name like" + " ";
		privilegedUserNameValue = " " + "specified words";
		privilegedUserIdUnspecified.setStyle("color:red");
		privilegedUserIdSpecified.setVisible(true);
		privilegedUserIdUnspecified.setVisible(true);
		privilegedUserIdSpecified.setValue(privilegedUser);
		privilegedUserIdUnspecified.setValue(privilegedUserNameValue);
	    }

	}

	else {
	    privilegedUserIdSpecified.setVisible(false);
	    privilegedUserIdUnspecified.setVisible(false);
	}

	boolean rowCount = getRegulationSettings.isRowCountWithTimeInterval();
	String rowCountString= (String) Sessions.getCurrent().getAttribute("rowCountDetails");
	if (rowCount == true) {
	    if (rowCountString != null && !rowCountString.isEmpty()) {
		String rowCountWizard = rowCountString;

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
		    String rowCountMsg = "when row count" + " ";
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
		else
			{
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
		boolean specifiedHostCheck = getRegulationSettings.getHostName();
		ListModelList<Host> specifiedHost = getRegulationSettings
				.getHostNameList();
		if (specifiedHostCheck == true) {
			hostNameValue = "";
			hostName = " " + " and Host Name like";
			description += "\r\n";
			description += margin + "and Host Name like ";
			int count = (specifiedHost != null) ? specifiedHost.getSize() : 0;
			if (count == 0) {
				hostNameValue = " " + "specified words";
				description += "specified words   ";
				hostNameIdAll.setStyle("color:red");
			} else {
				hostNameIdAll.setStyle("color:blue");
				for (int j = 0; j < specifiedHost.getSize(); j++) {
					description += specifiedHost.get(j).getHostName();
					hostNameValue += " " + specifiedHost.get(j).getHostName();
					if (j < count - 1) {
						hostNameValue += " " + ",";
						description += "or";
					}
				}
			}
			hostNameIdSpecified.setVisible(true);
			hostNameIdAll.setVisible(true);
		} else {
			hostNameIdSpecified.setVisible(false);
			hostNameIdAll.setVisible(false);
		}

		boolean specifiedExcludeEvent = getRegulationSettings
				.getExcludeCertainEventType();
		if (specifiedExcludeEvent) {
			excludeEvents.setVisible(true);
			excludeEventsData.setVisible(true);
			excludeEvents.setValue("and event type not ");
			description += "\r\n" + "and event type not ";
			excludeEventsData.setValue("");
			String excludeCertainMatchString = getRegulationSettings.getExcludeCertainMatchString();
			if (excludeCertainMatchString != null
					&& !excludeCertainMatchString.isEmpty()) {
				String specifiedExcludeEventData = (String) Sessions
						.getCurrent().getAttribute("ExcludeCirtainEventString");
				description += specifiedExcludeEventData;
				excludeEventsData.setValue(specifiedExcludeEventData);
				excludeEventsData.setStyle("color:blue");
			} else {
				description += "select event types";
				excludeEventsData.setValue("select event types");
				excludeEventsData.setStyle("color:red");
			}
		} else {
			excludeEvents.setVisible(false);
			excludeEventsData.setVisible(false);
		}
		String specifiedEmail = null;

		if (getSelectAlertActions.isEmailMessage()) {
			if (Sessions.getCurrent().getAttribute("addressList") != null) {
				String[] mailAddresses = (String[]) Sessions.getCurrent()
						.getAttribute("addressList");
				specifiedEmail = Joiner.on(",").skipNulls().join(mailAddresses)
						.replace(",", ",");
			}

			if (specifiedEmail != null && (!specifiedEmail.isEmpty())) {
				description += newLine + margin;
				description += "send an alert message to";
				alertMessageIdSpecified.setValue("send an alert message to ");
				description += newLine + margin;
				description += specifiedEmail;
				alertMessageValueLabel.setValue(specifiedEmail);
				alertMessageValueLabel.setStyle("color:blue");
			} else {
				description += newLine;
				description += "send an alert message to";
				alertMessageIdSpecified.setValue("send an alert message to ");
				description += newLine + margin;
				description += "specified addresses ";
				alertMessageValueLabel.setValue("specified addresses");
				alertMessageValueLabel.setStyle("color:red");
			}
			alertMessageValueLabel.setVisible(true);
			alertMessageIdSpecified.setVisible(true);
		} else {
			alertMessageValueLabel.setVisible(false);
			alertMessageIdSpecified.setVisible(false);
		}

		boolean specifiedlogEntry = getSelectAlertActions.isLogMessage();
		if (specifiedlogEntry == true) {
			boolean specifiedEmailCheck = getSelectAlertActions
					.isEmailMessage();

			if (specifiedEmailCheck == false) {
				description += newLine + margin;
				description += "Send an alert message to";
				String strTemp = "Send an alert message to";
				description += newLine + margin;
				description += "Windows Event Log entry of type  ";
				strTemp += newLine + margin
						+ "Windows Event Log entry of type  ";
				WindowAlertMessageIdSpecified.setValue(strTemp);
			} else {
				description += newLine + margin;
				description += "and Windows Event Log entry of type  ";
				WindowAlertMessageIdSpecified
						.setValue("and Windows Event Log entry of type  ");
			}
			description += getSelectAlertActions.getEventLogEntry();
			WindowAlertMessageIdAll.setValue(getSelectAlertActions
					.getEventLogEntry());
			WindowAlertMessageIdAll.setVisible(true);
			WindowAlertMessageIdSpecified.setVisible(true);
		} else {
			WindowAlertMessageIdAll.setVisible(false);
			WindowAlertMessageIdSpecified.setVisible(false);
		}
		eventAlertRuleDescription = description;
		
		boolean specifiedSnmpLog = getSelectAlertActions.isSnmpTrap();		
		if(specifiedSnmpLog){
			if(getSelectAlertActions.isEmailMessage() || specifiedlogEntry){
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
		
		
		
		

		setEventAlertRuleDescription(eventAlertRuleDescription);
		if (Sessions.getCurrent().getAttribute("QueryType") != null) {
			conditionEvents = (List<CMAlertRulesCondition>) Sessions
					.getCurrent().getAttribute("conditionEvents");
			alertRules = (List<CMAlertRules>) Sessions.getCurrent()
					.getAttribute("alertRules");
			if (conditionEvents != null && alertRules != null)
			{
				initializer(alertRules);
				BindUtils.postNotifyChange(null, null,
						SummaryStepViewModel.this, "*");
			}
		}

		BindUtils.postNotifyChange(null, null, SummaryStepViewModel.this, "*");
		BindUtils.postNotifyChange(null, null, SummaryStepViewModel.this,
				"eventAlertRuleDescription");
	}

	public void initializer(List<CMAlertRules> alertRules) {
		if (alertRules.get(0).getRuleId() != null) {
			if (alertRules.get(0).isEnabled()) {
				setEnabled(true);
			} else {
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
				.getLabel(SQLCMI18NStrings.SQL_FINISH_EVENT_ALERT_RULES_TIPS);
	}

	public String GenerateEventAlertRulesQuery() {
		int logMessage = (getSelectAlertActions.isLogMessage() ? 1 : 0);
		int emailMessage = (getSelectAlertActions.isEmailMessage() ? 1 : 0);
		int snmpTrap = (getSelectAlertActions.isSnmpTrap() ? 1 : 0);
		int enable = (isEnabled() ? 1 : 0);
		String instances = "<ALL>";
		if (getRegulationSettings.getTargetInstances() != null
				&& getRegulationSettings.getTargetInstances().size() > 0) {
			instances = getRegulationSettings.getTargetInstances().keySet()
					.toString();
			instances = instances.substring(1, (instances.length() - 1))
					.replaceAll(", ", ";");
		}
		String description = "";
		if (getSavedAlertRulesInfo.getDescription() != null) {
			description = getSavedAlertRulesInfo.getDescription();
		}
		String Community = "";
		if (getSelectAlertActions.getCommunity() != null) {
			Community = getSelectAlertActions.getCommunity();
		}
		String Address = "";
		if (getSelectAlertActions.getAddress() != null) {
			Address = getSelectAlertActions.getAddress();
		}

		boolean sqlServerCheck = getRegulationSettings.getSQLServer();
		if(sqlServerCheck){
			if(instances.equals("<ALL>")){
			instances="";
			isValid = false;
			}
		}
		String FinalRulesQuery = "INSERT INTO [SQLcompliance]..[AlertRules] ("
				+ "name," + "description," + "alertLevel," + "alertType,"
				+ "targetInstances," + "enabled," + "message," + "logMessage,"
				+ "emailMessage," + "snmpTrap," + "snmpServerAddress,"
				+ "snmpPort," + "snmpCommunity)" + " VALUES(" + " '"
				+ getSavedAlertRulesInfo.getName()
				+ "',"
				+ " '"
				+ description
				+ "',"
				+ " "
				+ getSavedAlertRulesInfo.getAlertLevel()
				+ ","
				+ " "
				+ getSavedAlertRulesInfo.getAlertType()
				+ ","
				+ " '"
				+ instances
				+ "',"
				+ " "
				+ enable
				+ ","
				+ " '"
				+ getSelectAlertActions.getMessage()
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
				+ "'"
				+ Address
				+ "',"
				+ " "
				+ getSelectAlertActions.getPort()
				+ ","
				+ " '"
				+ Community
				+ "');"
				+ " DECLARE @IDENTITY INT SELECT @IDENTITY = MAX(ruleId) FROM [SQLcompliance]..[AlertRules];";

		if (getSavedAlertRulesInfo.getMatchString() != null
				&& (!getSavedAlertRulesInfo.getMatchString().isEmpty())) {
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getSavedAlertRulesInfo.getFieldId()
					+ ", '"
					+ getSavedAlertRulesInfo.getMatchString() + "');";
		}

		boolean specifiedDbCheck = getRegulationSettings.getDatabaseName();
		String dbMatchString="";
		if(getRegulationSettings.getDbMatchString() != null){
			dbMatchString=getRegulationSettings.getDbMatchString();
		}
		if (specifiedDbCheck) {
			if(dbMatchString.equals("")){
				isValid = false;
			}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getDbFieldId()
					+ ", '"
					+ dbMatchString + "');";
		}

		boolean specifiedObCheck = getRegulationSettings.getObjectName();
		String objectMatchString = "";
		if(getRegulationSettings.getObjectMatchString() != null){
			objectMatchString = getRegulationSettings.getObjectMatchString();
		}
		if (specifiedObCheck) {
			if(objectMatchString.equals("")){
				isValid = false;
			}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getObjectFieldId()
					+ ", '"
					+ objectMatchString + "');";
		}

		boolean specifiedHostCheck = getRegulationSettings.getHostName();
		String hostMatchString = "";
		if(getRegulationSettings.getHostMatchString() != null){
			hostMatchString = getRegulationSettings.getHostMatchString();
		}		
		if (specifiedHostCheck) {		
			if(hostMatchString.equals("")){
				isValid = false;
			}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getHostFieldId()
					+ ", '"
					+ hostMatchString + "');";
		}
		

		boolean specifiedAppCheck = getRegulationSettings.getApplicationName();
		String appMatchString = "";
		if(getRegulationSettings.getAppMatchString() != null){
			appMatchString = getRegulationSettings.getAppMatchString();
		}
		if (specifiedAppCheck) {
			if(appMatchString.equals("")){
				isValid = false;
			}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getAppFieldId()
					+ ", '"
					+ appMatchString + "');";
		}

		boolean specifiedLoginCheck = getRegulationSettings.getLoginName();
		String loginMatchString = "";
		if(getRegulationSettings.getLoginMatchString() != null){
			loginMatchString = getRegulationSettings.getLoginMatchString();
		}
		if (specifiedLoginCheck) {
			if(loginMatchString.equals("")){
				isValid = false;
			}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getLoginFieldId()
					+ ", '"
					+ loginMatchString + "');";
		}

		if (getRegulationSettings.getAccessChkMatchString() != null
				&& (!getRegulationSettings.getAccessChkMatchString().isEmpty())) {
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getAccessChkFieldId()
					+ ", '"
					+ getRegulationSettings.getAccessChkMatchString() + "');";
		}

		if (getRegulationSettings.getPrivilegedUserMatchString() != null
				&& (!getRegulationSettings.getPrivilegedUserMatchString()
						.isEmpty())) {
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getPrivilegedUserFieldId()
					+ ", '"
					+ getRegulationSettings.getPrivilegedUserMatchString()
					+ "');";
		}
		boolean specifiedExcludeEvent = getRegulationSettings.getExcludeCertainEventType();
		String excludedEventsMatchString = "include(1)0value(0)";
		if(getRegulationSettings.getExcludeCertainMatchString() != null
				&& (!getRegulationSettings.getExcludeCertainMatchString()
						.isEmpty())){
			excludedEventsMatchString = getRegulationSettings.getExcludeCertainMatchString();
		}
		if (specifiedExcludeEvent) {
			if(excludedEventsMatchString.equals("include(1)0value(0)")){
				isValid = false;
			}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getExcludeCertainFieldId()
					+ ", '"
					+ excludedEventsMatchString
					+ "');";
		}	
		

		boolean specifiedPrivilegedUserName = getRegulationSettings.getPrivilegedUserName();
		String privilegedUserNameMatchString = "";
		if(getRegulationSettings.getPrivilegedUserNameMatchString() != null){
			privilegedUserNameMatchString = getRegulationSettings.getPrivilegedUserNameMatchString();
		}
		if(specifiedPrivilegedUserName)
		{
			if(privilegedUserNameMatchString.contains("blanks(1)0count(1)0") || privilegedUserNameMatchString.isEmpty()){
				isValid = false;
			}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getPrivilegedFieldId()
					+ ", '"
					+ privilegedUserNameMatchString + "');";
		}
		
		boolean specifiedRowCount = getRegulationSettings
				.isRowCountWithTimeInterval();
				String rowCountMatchString = "";		
		if(getRegulationSettings.getRowCountMatchString() != null){
			rowCountMatchString = getRegulationSettings.getRowCountMatchString();
		}
		if (specifiedRowCount) {
			if(rowCountMatchString.equals("") || rowCountMatchString.contains("rowcount(0)")){
					isValid = false;
				}
			FinalRulesQuery += "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES (@IDENTITY, "
					+ getRegulationSettings.getRowCountFieldId()
					+ ", '"
					+ rowCountMatchString + "');";
		}		
		return FinalRulesQuery;
	}

	public String GenerateEventAlertRulesUpdateQuery() {
		int logMessage = (getSelectAlertActions.isLogMessage() ? 1 : 0);
		int emailMessage = (getSelectAlertActions.isEmailMessage() ? 1 : 0);
		int snmpTrap = (getSelectAlertActions.isSnmpTrap() ? 1 : 0);
		int enable = (isEnabled() ? 1 : 0);
		String description = "";
		if (getSavedAlertRulesInfo.getDescription() != null) {
			description = getSavedAlertRulesInfo.getDescription();
		}
		String Community = "";
		if (getSelectAlertActions.getCommunity() != null) {
			Community = getSelectAlertActions.getCommunity();
		}
		String Address = "";
		if (getSelectAlertActions.getAddress() != null) {
			Address = getSelectAlertActions.getAddress();
		}
		if (getRegulationSettings.getTargetInstances() != null
				&& (!getRegulationSettings.getTargetInstances().isEmpty())) {
			targetInstance = getRegulationSettings.getTargetInstances()
					.keySet().toString();
			targetInstance = targetInstance.substring(1,
					(targetInstance.length() - 1)).replaceAll(", ", ";");
		} else {
			targetInstance = "<ALL>";
		}
		boolean sqlServerCheck = getRegulationSettings.getSQLServer();
		if(sqlServerCheck){
			if(targetInstance.equals("<ALL>")){
				targetInstance="";
				isValid = false;
			}
		}
		String FinalRulesQuery = "Update [SQLcompliance]..[AlertRules] Set name= '"
				+ getSavedAlertRulesInfo.getName()
				+ "',"
				+ "description = '"
				+ description
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
				+ Community + "' WHERE ruleId = " + ruleId + ";";

		
		if (getSavedAlertRulesInfo.getMatchString() != null
				&& (!getSavedAlertRulesInfo.getMatchString().isEmpty())) {
			FinalRulesQuery += "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getSavedAlertRulesInfo.getFieldId()
					+ ", matchString = '"
					+ getSavedAlertRulesInfo.getMatchString()
					+ "' where ruleId = "
					+ ruleId
					+ " and (fieldId = 0 OR fieldId = 1) and"
					+ " matchString like 'include(1)1%'" + ";";
		}

		boolean specifiedDbCheck = getRegulationSettings.getDatabaseName();
		String dbMatchString="";
		if(getRegulationSettings.getDbMatchString() != null){
			dbMatchString=getRegulationSettings.getDbMatchString();
		}
		
		if (specifiedDbCheck) {
			if(dbMatchString.equals("")){
				isValid = false;
			}
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ getRegulationSettings.getDbFieldId()
					+ " AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getRegulationSettings.getDbFieldId()
					+ ", matchString = '"
					+ dbMatchString
					+ "'where ruleId = "
					+ ruleId
					+ " and fieldId = "
					+ getRegulationSettings.getDbFieldId()
					+ ""
					+ " END "
					+ " ELSE "
					+ " BEGIN "
					+ "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES ("
					+ ruleId
					+ ", "
					+ getRegulationSettings.getDbFieldId()
					+ ", '"
					+ dbMatchString
					+ "')"
					+ " END;";
		} else {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ "5 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions]"
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 5"
					+ " END;";
		}

		boolean specifiedObCheck = getRegulationSettings.getObjectName();
		String objectMatchString = "";
		if(getRegulationSettings.getObjectMatchString() != null){
			objectMatchString = getRegulationSettings.getObjectMatchString();
		}
		if (specifiedObCheck) {
			if(objectMatchString.equals("")){
				isValid = false;
			}
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ getRegulationSettings.getObjectFieldId()
					+ " AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getRegulationSettings.getObjectFieldId()
					+ ", matchString = '"
					+ objectMatchString
					+ "'where ruleId = "
					+ ruleId
					+ " and fieldId = "
					+ getRegulationSettings.getObjectFieldId()
					+ ""
					+ " END "
					+ " ELSE "
					+ " BEGIN "
					+ "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES ("
					+ ruleId
					+ ", "
					+ getRegulationSettings.getObjectFieldId()
					+ ", '"
					+ objectMatchString
					+ "')" + " END;";

		} else {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ "6 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 6"
					+ " END;";
		}
		
		boolean specifiedHostCheck = getRegulationSettings.getHostName();
		String hostMatchString = "";
		if(getRegulationSettings.getHostMatchString() != null){
			hostMatchString = getRegulationSettings.getHostMatchString();
		}
		if (specifiedHostCheck) {
			if(hostMatchString.equals("")){
				isValid = false;
			}
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ getRegulationSettings.getHostFieldId()
					+ " AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getRegulationSettings.getHostFieldId()
					+ ", matchString = '"
					+ hostMatchString
					+ "'where ruleId = "
					+ ruleId
					+ " and fieldId = "
					+ getRegulationSettings.getHostFieldId()
					+ ""
					+ " END "
					+ " ELSE "
					+ " BEGIN "
					+ "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES ("
					+ ruleId
					+ ", "
					+ getRegulationSettings.getHostFieldId()
					+ ", '"
					+ hostMatchString
					+ "')"
					+ " END;";
		} else {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ "10 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 10"
					+ " END;";
		}
		

		boolean specifiedAppCheck = getRegulationSettings.getApplicationName();
		String appMatchString = "";
		if(getRegulationSettings.getAppMatchString() != null){
			appMatchString = getRegulationSettings.getAppMatchString();
		}
		if (specifiedAppCheck) {
			if(appMatchString.equals("")){
				isValid = false;
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
					+ "2 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 2"
					+ " END;";
		}
		
		boolean specifiedLoginCheck = getRegulationSettings.getLoginName();
		String loginMatchString = "";
		if(getRegulationSettings.getLoginMatchString() != null){
			loginMatchString = getRegulationSettings.getLoginMatchString();
		}
		if (specifiedLoginCheck) {
			if(loginMatchString.equals("")){
				isValid = false;
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
					+ "3 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 3"
					+ " END;";
		}

		if (getRegulationSettings.getAccessChkMatchString() != null
				&& (!getRegulationSettings.getAccessChkMatchString().isEmpty())) {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ getRegulationSettings.getAccessChkFieldId()
					+ " AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getRegulationSettings.getAccessChkFieldId()
					+ ", matchString = '"
					+ getRegulationSettings.getAccessChkMatchString()
					+ "'where ruleId = "
					+ ruleId
					+ " and fieldId = "
					+ getRegulationSettings.getAccessChkFieldId()
					+ ""
					+ " END "
					+ " ELSE "
					+ " BEGIN "
					+ "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES ("
					+ ruleId
					+ ", "
					+ getRegulationSettings.getAccessChkFieldId()
					+ ", '"
					+ getRegulationSettings.getAccessChkMatchString()
					+ "')"
					+ " END;";
		}

		else {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ "4 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 4"
					+ " END;";
		}

		if (getRegulationSettings.getPrivilegedUserMatchString() != null
				&& (!getRegulationSettings.getPrivilegedUserMatchString()
						.isEmpty())) {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ getRegulationSettings.getPrivilegedUserFieldId()
					+ " AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getRegulationSettings.getPrivilegedUserFieldId()
					+ ", matchString = '"
					+ getRegulationSettings.getPrivilegedUserMatchString()
					+ "'where ruleId = "
					+ ruleId
					+ " and fieldId = "
					+ getRegulationSettings.getPrivilegedUserFieldId()
					+ ""
					+ " END "
					+ " ELSE "
					+ " BEGIN "
					+ "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES ("
					+ ruleId
					+ ", "
					+ getRegulationSettings.getPrivilegedUserFieldId()
					+ ", '"
					+ getRegulationSettings.getPrivilegedUserMatchString()
					+ "')" + " END;";
		}

		else {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ "7 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 7"
					+ " END;";
		}

		boolean specifiedExcludeEvent = getRegulationSettings.getExcludeCertainEventType();
		String excludedEventsMatchString = "include(1)0value(0)";
		if(getRegulationSettings.getExcludeCertainMatchString() != null
				&& (!getRegulationSettings.getExcludeCertainMatchString()
						.isEmpty())){
			excludedEventsMatchString = getRegulationSettings.getExcludeCertainMatchString();
		}
		if (specifiedExcludeEvent) {
			if(excludedEventsMatchString.equals("include(1)0value(0)")){
				isValid = false;
			}
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ getRegulationSettings.getExcludeCertainFieldId()
					+ " AND ruleId = "
					+ ruleId
					+ " and matchString like 'include(1)0%')"
					+ " BEGIN "
					+ "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getRegulationSettings.getExcludeCertainFieldId()
					+ ", matchString = '"
					+ excludedEventsMatchString
					+ "'where ruleId = "
					+ ruleId
					+ " and fieldId = "
					+ getRegulationSettings.getExcludeCertainFieldId()
					+ " and matchString like 'include(1)0%'"
					+ " END "
					+ " ELSE "
					+ " BEGIN "
					+ "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES ("
					+ ruleId
					+ ", "
					+ getRegulationSettings.getExcludeCertainFieldId()
					+ ", '"
					+ excludedEventsMatchString
					+ "')" + " END;";
		}

		else {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ "0 AND ruleId = "
					+ ruleId
					+ " and matchString like 'include(1)0%')"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 0 and matchString like 'include(1)0%'"
					+ " END;";
		}

		boolean specifiedRowCountCheck = getRegulationSettings.isRowCountWithTimeInterval();
		String rowCountMatchString = "";
		if (getRegulationSettings.getRowCountMatchString() != null) {
			rowCountMatchString = getRegulationSettings.getRowCountMatchString();
		}
		if (specifiedRowCountCheck) {
			if (rowCountMatchString.equals("") || rowCountMatchString.contains("rowcount(0)")) {
				isValid = false;
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

		
		String privilegedUserName = "";
		if(getRegulationSettings.getPrivilegedUserNameMatchString() != null)
			privilegedUserName = getRegulationSettings.getPrivilegedUserNameMatchString();
		boolean specifyPrivilegedUserName=getRegulationSettings.getPrivilegedUserName();
		if (specifyPrivilegedUserName) {
			if(privilegedUserName.contains("blanks(1)0count(1)0") 
					|| privilegedUserName.isEmpty()){
				isValid = false;
			}	
		
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ getRegulationSettings.getPrivilegedFieldId()
					+ " AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "UPDATE [SQLcompliance].[dbo].[AlertRuleConditions] SET fieldId = "
					+ getRegulationSettings.getPrivilegedFieldId()
					+ ", matchString = '"
					+ privilegedUserName
					+ "'where ruleId = "
					+ ruleId
					+ " and fieldId = "
					+ getRegulationSettings.getPrivilegedFieldId()
					+ ""
					+ " END "
					+ " ELSE "
					+ " BEGIN "
					+ "INSERT INTO  [SQLcompliance].[dbo].[AlertRuleConditions] (ruleId, fieldId, matchString) VALUES ("
					+ ruleId
					+ ", "
					+ getRegulationSettings.getPrivilegedFieldId()
					+ ", '"
					+ privilegedUserName
					+ "')" + " END;";
		}

		else {
			FinalRulesQuery += "IF EXISTS (SELECT * FROM [SQLcompliance].[dbo].[AlertRuleConditions] WHERE fieldId = "
					+ "13 AND ruleId = "
					+ ruleId
					+ ")"
					+ " BEGIN "
					+ "DELETE FROM [SQLcompliance].[dbo].[AlertRuleConditions] "
					+ " where ruleId = "
					+ ruleId
					+ " and fieldId = 13"
					+ " END;";
			}

		return FinalRulesQuery;
	}

	@Override
	public void onFinish(AddAlertRulesSaveEntity wizardSaveEntity) {
		isValid = true;
		String QueryBuilder = "";
		boolean isValidAdreess = false;
		if(wizardSaveEntity.getSelectAlertActions().isSnmpTrap()){
			if(wizardSaveEntity.getSelectAlertActions().getAddress()!= null 
					&& !wizardSaveEntity.getSelectAlertActions().getAddress().equals("")){
				SNMPConfigFacade snmpConfigFacade = new SNMPConfigFacade();
				SNMPConfigData snmpConfigData = new SNMPConfigData();
				snmpConfigData.setSnmpAddress(wizardSaveEntity.getSelectAlertActions().getAddress());
				snmpConfigData.setPort(wizardSaveEntity.getSelectAlertActions().getPort());
				snmpConfigData.setCommunity(wizardSaveEntity.getSelectAlertActions().getCommunity());
				isValidAdreess = false;
				try{
					isValidAdreess = snmpConfigFacade.checkSnmpAddress(snmpConfigData);
				}
				catch(RestException e){}
			}else{
				isValidAdreess = true;
			}
		}
		else{
			isValidAdreess = true;
		}
		if(isValidAdreess){
			if(getSelectAlertActions == null || getSelectAlertActions.getMessage().length()<=2500){
				if (Sessions.getCurrent().getAttribute("QueryType") != null
						&& Sessions.getCurrent().getAttribute("QueryType")
								.equals("Update")) {
		
					if (Sessions.getCurrent().getAttribute("alertRuleId") != null) {
						getSavedAlertRulesInfo = wizardSaveEntity
								.getNewEventAlertRules();
						getRegulationSettings = wizardSaveEntity
								.getRegulationSettings();
						getSelectAlertActions = wizardSaveEntity
								.getSelectAlertActions();
						// targetInstance =
						// getRegulationSettings.getTargetInstances().entrySet().iterator().next().getKey();
						Map<Long, Boolean> ruleIdAndEnable = (Map<Long, Boolean>) Sessions
								.getCurrent().getAttribute("alertRuleId");
						ruleId = ruleIdAndEnable.entrySet().iterator().next().getKey();
						setEnabled(ruleIdAndEnable.get(ruleId));
					}
					QueryBuilder = GenerateEventAlertRulesUpdateQuery();
					logType = 55;
				} else {
					QueryBuilder = GenerateEventAlertRulesQuery();
					logType = 53;
				}
				if(Sessions.getCurrent().getAttribute("alertRuleId")!=null)
					Sessions.getCurrent().removeAttribute("alertRuleId");
				String ruleName = getSavedAlertRulesInfo.getName();
				String desc = (getSavedAlertRulesInfo.getDescription() != null) ? getSavedAlertRulesInfo
						.getDescription() : "";
				String logEntry = "";
				eventAlertRuleDescription = eventAlertRuleDescription.replaceAll("\t",
						"");
				eventAlertRuleDescription = "Name:  " + ruleName + "\r\nDescription:  "
						+ desc + "\r\n\r\nRule:  " + eventAlertRuleDescription;
				logEntry = "INSERT INTO {0} (eventTime, logType, logUser, logSqlServer, logInfo) "
						+ "VALUES (GETUTCDATE(),"
						+ logType
						+ ",{1},'','"
						+ eventAlertRuleDescription + "');";
				QueryBuilder += logEntry;
				
					InsertEventAlertRules(QueryBuilder);
				if(!isValid){
					WebUtil.showWarningBoxWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.INVALID_ALERT_RULE),
							ELFunctions.getLabel(SQLCMI18NStrings.INVALID_RULE_TITLE));
				}
			}
			else{
				WebUtil.showWarningBoxWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.INVALID_EVENT_ALERT_MESSAGE),
						"Error");
			}
			
			if(Sessions.getCurrent().removeAttribute("isValidSnmpAddress")!=null){
				Sessions.getCurrent().removeAttribute("isValidSnmpAddress");
			}
				String uri = "instancesAlertsRule";
				uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
				Executions.sendRedirect(uri);
		}
		else{
			Sessions.getCurrent().setAttribute("isValidSnmpAddress",true);
		}
	}

	public void InsertEventAlertRules(String insertStatusAlertRulesRequest) {
		insertQueryData = new InsertQueryData();
		insertQueryData.setDataQuery(insertStatusAlertRulesRequest);
		try {
			AlertRulesFacade.insertStatusAlertRules(insertQueryData);
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
		}
	}

	@Override
	public String getHelpUrl() {
		return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
	}
}
