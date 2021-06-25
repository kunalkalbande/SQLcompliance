package com.idera.sqlcm.ui.dialogs.eventFilterWizard.steps;

import java.util.Map;

import mazz.i18n.annotation.I18NMessage;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Session;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.facade.EventFiltersFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.EventFilterSaveEntity;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.NewEventFilterRules;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.RegulationSettings;
import com.idera.sqlcm.ui.eventFilters.EventFilterCondition;
import com.idera.sqlcm.ui.eventFilters.EventFiltersGridViewModel;
import com.idera.sqlcm.ui.eventFilters.SpecifyAppNameViewModel.App;
import com.idera.sqlcm.ui.eventFilters.SpecifyDatabaseViewModel.Data;
import com.idera.sqlcm.ui.eventFilters.SpecifyHostNameViewModel.Host;
import com.idera.sqlcm.ui.eventFilters.SpecifyLoginViewModel.Login;
import com.idera.sqlcm.ui.eventFilters.SpecifySessionLoginViewModel.SessionLogin;
import com.idera.sqlcm.ui.eventFilters.SpecifyObjectsViewModel.Objects;

//AddAlertRulesSaveEntity

public class SummaryStepViewModel extends AddWizardStepBase {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/eventFilterWizard/steps/summary-step.zul";
	String FinalQuery = "";
	InsertQueryData insertQueryData;
	private String auditLevelName;
	private String serverName;
	private ListModelList<CMDatabase> selectedDatabaseList;
	private String eventFilterRuleDescription="";

	public NewEventFilterRules getSavedFilterInfo;

	public RegulationSettings getRegulationSettings;
	EventFilterCondition eventCondition = new EventFilterCondition();
	EventFiltersGridViewModel eventFiltersGridViewModel;
	public String sqlServerValue;
	public String sqlServerName;
	public String filterText;
	public String sqlObject;
	public String sqlObjectName;
	public String sqlDatabaseValue;
	public String applicationName;
	public String applicationNameValue;
	public String loginNam;
	public String hostName;
	public String hostNameValue;
	public String sessionName;
	public String sessionNameValue;
	public String privileged;
	public int logType;
	public boolean invalidFilterCondition;
	public boolean invalidFilter;
	boolean enabled;
	
	public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
	}

	public String getPrivileged() {
		return privileged;
	}

	public void setPrivileged(String privileged) {
		this.privileged = privileged;
	}
	public String privilegedValue;
	public String getPrivilegedValue() {
		return privilegedValue;
	}

	public void setPrivilegedValue(String privilegedValue) {
		this.privilegedValue = privilegedValue;
	}

	public String getSessionNameValue() {
		return sessionNameValue;
	}

	public void setSessionNameValue(String sessionNameValue) {
		this.sessionNameValue = sessionNameValue;
	}

	public String getSessionName() {
		return sessionName;
	}

	public void setSessionName(String sessionName) {
		this.sessionName = sessionName;
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

	public String getLoginNam() {
		return loginNam;
	}

	public void setLoginNam(String loginNam) {
		this.loginNam = loginNam;
	}
	public String loginNameValue;
	public String getLoginNameValue() {
		return loginNameValue;
	}

	public void setLoginNameValue(String loginNameValue) {
		this.loginNameValue = loginNameValue;
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

	public String getSqlDatabaseValue() {
		return sqlDatabaseValue;
	}

	public void setSqlDatabaseValue(String sqlDatabaseValue) {
		this.sqlDatabaseValue = sqlDatabaseValue;
	}
	public String sqlDatabaseName;
	public String getSqlDatabaseName() {
		return sqlDatabaseName;
	}

	public void setSqlDatabaseName(String sqlDatabaseName) {
		this.sqlDatabaseName = sqlDatabaseName;
	}

	public String getSqlObjectName() {
		return sqlObjectName;
	}

	public void setSqlObjectName(String sqlObjectName) {
		this.sqlObjectName = sqlObjectName;
	}

	public String getSqlObject() {
		return sqlObject;
	}

	public void setSqlObject(String sqlObject) {
		this.sqlObject = sqlObject;
	}

	public String getFilterText() {
		return filterText;
	}

	public void setFilterText(String filterText) {
		this.filterText = filterText;
	}
	public String generateFilter;
	public String getGenerateFilter() {
		return generateFilter;
	}

	public void setGenerateFilter(String generateFilter) {
		this.generateFilter = generateFilter;
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
	@Wire
	Label sqlObjectIdUnspecified,sqlObjectIdSpecified,sqlObjectIdAll;
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
	Label hostNameIdUnspecified,hostNameIdSpecified,hostNameIdAll;
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
	Label privilegedIdAll ,privilegedIdSpecified;
	public Label getPrivilegedIdSpecified() {
		return privilegedIdSpecified;
	}

	public void setPrivilegedIdSpecified(Label privilegedIdSpecified) {
		this.privilegedIdSpecified = privilegedIdSpecified;
	}

	public Label getPrivilegedIdAll() {
		return privilegedIdAll;
	}

	public void setPrivilegedIdAll(Label privilegedIdAll) {
		this.privilegedIdAll = privilegedIdAll;
	}

	@Wire
	Label applicationNameIdUnspecified,applicationNameIdSpecified,applicationNameIdAll;
	
	@Wire
	Label filterType, filterRemText;
	
	public Label getApplicationNameIdAll() {
		return applicationNameIdAll;
	}

	public void setApplicationNameIdAll(Label applicationNameIdAll) {
		this.applicationNameIdAll = applicationNameIdAll;
	}

	public Label getApplicationNameIdSpecified() {
		return applicationNameIdSpecified;
	}

	public void setApplicationNameIdSpecified(Label applicationNameIdSpecified) {
		this.applicationNameIdSpecified = applicationNameIdSpecified;
	}

	public Label getApplicationNameIdUnspecified() {
		return applicationNameIdUnspecified;
	}

	public void setApplicationNameIdUnspecified(Label applicationNameIdUnspecified) {
		this.applicationNameIdUnspecified = applicationNameIdUnspecified;
	}
	@Wire
	Label sqlDatabaseIdUnspecified,sqlDatabaseIdSpecified,sqlDatabaseIdAll;
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

public Label getSqlDatabaseIdUnspecified() {
		return sqlDatabaseIdUnspecified;
	}

	public void setSqlDatabaseIdUnspecified(Label sqlDatabaseIdUnspecified) {
		this.sqlDatabaseIdUnspecified = sqlDatabaseIdUnspecified;
	}
	@Wire 
	Label sessionNameIdUnspecified,sessionNameIdSpecified,sessionNameIdAll;
	public Label getSessionNameIdAll() {
		return sessionNameIdAll;
	}

	public void setSessionNameIdAll(Label sessionNameIdAll) {
		this.sessionNameIdAll = sessionNameIdAll;
	}

	public Label getSessionNameIdSpecified() {
		return sessionNameIdSpecified;
	}

	public void setSessionNameIdSpecified(Label sessionNameIdSpecified) {
		this.sessionNameIdSpecified = sessionNameIdSpecified;
	}

	public Label getSessionNameIdUnspecified() {
		return sessionNameIdUnspecified;
	}

	public void setSessionNameIdUnspecified(Label sessionNameIdUnspecified) {
		this.sessionNameIdUnspecified = sessionNameIdUnspecified;
	}
	@Wire
	Label loginIdSpecified,loginIdUnspecified,loginIdAll;
public Label getLoginIdAll() {
		return loginIdAll;
	}

	public void setLoginIdAll(Label loginIdAll) {
		this.loginIdAll = loginIdAll;
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
Label sqlServerIdUnspecified,sqlServerIdSpecified,sqlServerIdAll;
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
	@Wire
	Checkbox chkEnabled;

	String margin = "\t";

	@I18NMessage("")
	String FILTER_DESCRIPTION = "SQLCM.Labels.filter.description";

	public SummaryStepViewModel() {
		super();
	}

	public String getEventFilterRuleDescription() {
		return eventFilterRuleDescription;
	}

	public void setEventFilterRuleDescription(String eventFilterRuleDescription) {
		this.eventFilterRuleDescription = eventFilterRuleDescription;
	}
	

	@Override
	public void onCancel(EventFilterSaveEntity wizardSaveEntity) {
		if(Sessions.getCurrent().getAttribute("QueryType")!=null)
		{
			Sessions.getCurrent().removeAttribute("QueryType");
		}
		
		String uri = "eventFiltersView";
		uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
		Executions.sendRedirect(uri);
	}
	
	public void doOnShow(EventFilterSaveEntity wizardSaveEntity) {
		getSavedFilterInfo = wizardSaveEntity.getNewEventAlertRules();
		getRegulationSettings = wizardSaveEntity.getRegulationSettings();		
		String description = "Filter ";
		setEnabled(true);
		String EventType;
		EventType = getSavedFilterInfo.getEventFilter();		
		if (EventType != null) 
		{
			
			filterText =EventType;
			description += EventType;
			if(!(EventType.equals("All Events"))){
				description += " events";
				if(getSavedFilterInfo.getEventFilterCategoryFiledId()==1){
					filterType.setValue("Filter all ");
				}
				else{
					filterType.setValue("Filter the ");
				}
				filterRemText.setValue(" events");
			}
			else{
				filterType.setValue("Filter ");
			}
			
		}
		else{
			filterType.setValue("Filter ");
			filterText ="All Events";
		}

		String specifiedSQLServer = " specified SQL Server";
		boolean sqlServerCheck = getRegulationSettings.getSQLServer();
		Map<String, Object> TargetServer = getRegulationSettings.getTargetInstances();
		if(sqlServerCheck == true){
    		
    		if(TargetServer != null && TargetServer.size()>0)
    		{
    			String tInstances = TargetServer.keySet().toString();
    			description +="\r\n";
    			description += margin + "on SQL Server ";
    			description +=tInstances.substring(1,(tInstances.length()-1));
    			sqlServerName=" "+tInstances.substring(1,(tInstances.length()-1));
    			sqlServerValue=" "+"on SQL Server";
    			sqlServerIdUnspecified.setVisible(false);
    			sqlServerIdSpecified.setVisible(true);
    			sqlServerIdAll.setVisible(true);
    			sqlServerIdAll.setStyle("color:blue");
    		}
    		
    		else{

    			description += "\r\n";
    			description += margin + "on specified SQL Servers  ";
    			sqlServerValue= "on";
    			sqlServerName = " specified SQL Servers";
    			sqlServerIdUnspecified.setVisible(false);
    			sqlServerIdSpecified.setVisible(true);
    			sqlServerIdAll.setVisible(true);
    			sqlServerIdAll.setStyle("color:red");
    		}
    		
    		
    	}
    	else{

			description += "\r\n";
			description += margin + "on any SQL Server ";
			sqlServerValue=" "+"on any SQL Server";
			sqlServerIdUnspecified.setVisible(false);
			sqlServerIdSpecified.setVisible(true);
			sqlServerIdAll.setVisible(false);		
    	
    	}		
		boolean specifiedDbCheck = getRegulationSettings.getDatabaseName();
		ListModelList<Data> specifiedDatabase = (ListModelList<Data>)getRegulationSettings.getDbNameList();
		if (specifiedDbCheck == true) 
		{
			sqlDatabaseName=null;
			description += "\r\n";
			description += margin + "and ";
			sqlDatabaseValue=" "+"and ";
			int count =(specifiedDatabase!=null)?specifiedDatabase.getSize():0;
			switch (count) {
			case 0:
				description += "specified databases   ";
				//sqlDatabaseValue+=" "+"specified databases";
				sqlDatabaseName = "specified databases";
				sqlDatabaseIdAll.setStyle("color:red");
				break;
			case 1:
				description += "database ";
				sqlDatabaseValue+="database";
				break;
			default:
				description += "databases ";
				sqlDatabaseValue+="databases";
			}
			if(sqlDatabaseName==null){
				sqlDatabaseName="";
				sqlDatabaseIdAll.setStyle("color:blue");
			}
			for (int j = 0; j < count; j++) {
				sqlDatabaseName+=" "+ specifiedDatabase.get(j).getDataBaseName();
				description += specifiedDatabase.get(j).getDataBaseName();
				if (j < count-1) {
					description += ",";
					sqlDatabaseName+=" ,";
				}
			}
			if(sqlDatabaseName==null)
			{
				sqlDatabaseIdUnspecified.setVisible(true);
				sqlDatabaseIdSpecified.setVisible(false);
				sqlDatabaseIdAll.setVisible(false);
				
			}else
			{
				sqlDatabaseIdUnspecified.setVisible(false);
				sqlDatabaseIdSpecified.setVisible(true);
				sqlDatabaseIdAll.setVisible(true);
			}
			
		}else
		{
			sqlDatabaseIdUnspecified.setVisible(false);
			sqlDatabaseIdSpecified.setVisible(false);
			sqlDatabaseIdAll.setVisible(false);
			
		}

		boolean specifiedObCheck = getRegulationSettings.getObjectName();
		ListModelList<Objects> specifiedObject = getRegulationSettings.getObjectNameList();
		if (specifiedObCheck == true) {
			description += "\r\n";
			description += margin + "and ";
			sqlObject=" "+"and";
			sqlObjectName=null;
			int count = (specifiedObject!=null)?specifiedObject.getSize():0;
			switch (count) {
			case 0:
				description += "specified objects   ";
				//sqlObject+=" "+"specified objects";
				sqlObjectName=" "+"specified objects";
				sqlObjectIdAll.setStyle("color:red");
				break;
			case 1:
				description += "Object";
				sqlObject+=" "+"Object";
				break;
			default:
				description += "Objects ";
				sqlObject+=" "+"Objects";
			}
			if(sqlObjectName==null){
				sqlObjectName="";
				sqlObjectIdAll.setStyle("color:blue");
			}
			for (int j = 0; j < count; j++) {
				sqlObjectName+=" "+specifiedObject.get(j).getObjectName();
				description += specifiedObject.get(j).getObjectName();
				if (j < specifiedObject.getSize() - 1) {
					description += ",";
                  sqlObjectName+=",";
				}
			}
			if(sqlObjectName==null)
			{
				sqlObjectIdUnspecified.setVisible(true);
				sqlObjectIdSpecified.setVisible(false);
				sqlObjectIdAll.setVisible(false);
			}else
			{
				sqlObjectIdUnspecified.setVisible(false);
				sqlObjectIdSpecified.setVisible(true);
				sqlObjectIdAll.setVisible(true);
			}
			
			
		}else
		{
			sqlObjectIdUnspecified.setVisible(false);
			sqlObjectIdSpecified.setVisible(false);
			sqlObjectIdAll.setVisible(false);
			
		}
		

		// boolean isprivilegedCheck =	
		// getRegulationSettings.getIsPrivilegedCheck();
		boolean isprivileged = getRegulationSettings.getIsPrivilegedCheck();
		if (isprivileged == true) 
		{privileged=" "+"where Privileged User is ";
			description += "\r\n";
			description += margin + "where Privileged User is ";
			privilegedValue=" "+isprivileged;
			description += isprivileged;
			privilegedIdSpecified.setVisible(true);
			    privilegedIdAll.setVisible(true);
		}
		else
		{
			privilegedIdSpecified.setVisible(false);
		    privilegedIdAll.setVisible(false);
		}
		boolean specifiedAppCheck = getRegulationSettings.getApplicationName();
		ListModelList<App> specifiedApplication = getRegulationSettings.getAppNameList();
		if (specifiedAppCheck == true) {
			description += "\r\n";
			description += margin + "and Application Name like ";
			applicationName=" "+"and Application Name like";
			int count = (specifiedApplication!=null)?specifiedApplication.getSize():0;
			if (count == 0) {
				description += "specified words   ";
				applicationNameValue=" specified words";
				applicationNameIdAll.setStyle("color:red");
			} else {
				applicationNameValue="";
				applicationNameIdAll.setStyle("color:blue");
				for (int j = 0; j < count; j++) {
					applicationNameValue+=" '"+specifiedApplication.get(j).getAppName()+"'";
					description += specifiedApplication.get(j).getAppName();
					if (j < specifiedApplication.getSize() - 1) {
						description += ",";
                     applicationNameValue+=" or";
					}
				}
			}
		if(applicationNameValue==null)
		{
			applicationNameIdSpecified.setVisible(false);			
			applicationNameIdUnspecified.setVisible(true);	
		    applicationNameIdAll.setVisible(false);
		
		}else
		{
			applicationNameIdSpecified.setVisible(true);			
			applicationNameIdUnspecified.setVisible(false);	
		    applicationNameIdAll.setVisible(true);
		
		}
		}
		else
		{
			applicationNameIdSpecified.setVisible(false);			
			applicationNameIdUnspecified.setVisible(false);	
		    applicationNameIdAll.setVisible(false);
			}

		boolean specifiedLoginCheck = getRegulationSettings.getLoginName();
		ListModelList<Login> specifiedLogin = getRegulationSettings.getLoginNameList();
		if (specifiedLoginCheck == true) 
		{
			loginNam=" "+"and Login Name like";
			description += "\r\n";
			description += margin + "and Login Name like ";
			int count = (specifiedLogin!=null)?specifiedLogin.getSize():0;
			if (count == 0) {
				description += "specified words   ";
				loginNameValue=" specified words";
				loginIdAll.setStyle("color:red");
			} else {
				loginNameValue="";
				loginIdAll.setStyle("color:blue");
				for (int j = 0; j < count; j++) {
					description += specifiedLogin.get(j).getLoginName();
					loginNameValue+=" '"+specifiedLogin.get(j).getLoginName()+"'";
					if (j < specifiedLogin.getSize() - 1) 
					{
						description += ",";
						loginNameValue+=" or";
					}
				}
			}
			
			if(loginNameValue==null)
			{
				loginIdSpecified.setVisible(false);
				loginIdUnspecified.setVisible(true);
				loginIdAll.setVisible(false);	
			}else
			{
				loginIdSpecified.setVisible(true);
				loginIdUnspecified.setVisible(false);
				loginIdAll.setVisible(true);
			}
			
		}
		else
		{
			loginIdSpecified.setVisible(false);
			loginIdUnspecified.setVisible(false);
			loginIdAll.setVisible(false);
		}

		boolean specifiedSessionLoginCheck = getRegulationSettings.getSessionLoginName();
		ListModelList<SessionLogin> specifiedSessionLogin = getRegulationSettings.getSessionLoginNameList();
		if (specifiedSessionLoginCheck == true) 
		{
			sessionName=" "+ "and Session Login Name like";
			description += "\r\n";
			description += margin + "and Session Login Name like ";
			int count = (specifiedSessionLogin!=null)?specifiedSessionLogin.getSize():0;
			if (count == 0) 
			{  sessionNameValue=" specified words";
				description += "specified words   ";
				sessionNameIdAll.setStyle("color:red");
			} else 
			{
				sessionNameIdAll.setStyle("color:blue");
				sessionNameValue=" ";
				for (int j = 0; j < specifiedSessionLogin.getSize(); j++) 
				{sessionNameValue+="'"+specifiedSessionLogin.get(j).getSessionLoginName()+"'";
					description += specifiedSessionLogin.get(j).getSessionLoginName();
					if (j < specifiedSessionLogin.getSize() - 1) 
					{
						description += ",";
						sessionNameValue+=" "+"or";
					}
				}
			}
			if(sessionNameValue==null)
			{
				sessionNameIdUnspecified.setVisible(true);
				sessionNameIdSpecified.setVisible(false);
				sessionNameIdAll.setVisible(false);
			}else
			{
				sessionNameIdUnspecified.setVisible(false);
				sessionNameIdSpecified.setVisible(true);
				sessionNameIdAll.setVisible(true);
			}
		}else
		{
			sessionNameIdUnspecified.setVisible(false);
			sessionNameIdSpecified.setVisible(false);
			sessionNameIdAll.setVisible(false);
		}

		boolean specifiedHostCheck = getRegulationSettings.getHostName();
		ListModelList<Host> specifiedHost = getRegulationSettings.getHostNameList();
		if (specifiedHostCheck == true) 
		{ hostName=" "+"and Host Name like";
			description += "\r\n";
			description += margin + "and Host Name like ";
			int count = (specifiedHost!=null)?specifiedHost.getSize():0;
			if (count == 0) 
			{
				description += "specified words   ";
				hostNameValue=" specified words";
				hostNameIdAll.setStyle("color:red");
			} 
			else 
			{
				hostNameIdAll.setStyle("color:blue");
				hostNameValue="";
				for (int j = 0; j < count; j++) 
				{
					description += specifiedHost.get(j).getHostName();
					hostNameValue+=" '"+specifiedHost.get(j).getHostName()+"'";
					if (j < specifiedHost.getSize() - 1) 
					{
						description += ",";
						hostNameValue+=" or";
					}
				}
			}
			if(hostNameValue==null)
			{
				hostNameIdUnspecified.setVisible(true);
				hostNameIdSpecified.setVisible(false);
				hostNameIdAll.setVisible(false);
			}else
			{
				hostNameIdUnspecified.setVisible(false);
				hostNameIdSpecified.setVisible(true);
				hostNameIdAll.setVisible(true);	
			}
		}
		else
		{
			hostNameIdUnspecified.setVisible(false);
			hostNameIdSpecified.setVisible(false);
			hostNameIdAll.setVisible(false);
			
		}
		Sessions.getCurrent().removeAttribute("isThisEnabled");
		eventFilterRuleDescription = description;
		setEventFilterRuleDescription(eventFilterRuleDescription);
		
		if(Sessions.getCurrent().getAttribute("isEnabled")!=null){
			setEnabled((boolean)Sessions.getCurrent().getAttribute("isEnabled"));
			Sessions.getCurrent().removeAttribute("isEnabled");
		}
		BindUtils.postNotifyChange(null, null, SummaryStepViewModel.this,"*");
		BindUtils.postNotifyChange(null, null, SummaryStepViewModel.this,"eventFilterRuleDescription");
	}

	public void InsertEventFilterData(String Query) {
		insertQueryData = new InsertQueryData();
		insertQueryData.setDataQuery(Query);
		try {
			EventFiltersFacade.insertEventFilterData(insertQueryData);
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
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
				.getLabel(SQLCMI18NStrings.SQL_SERVER_FINAL_FILTER_TIPS);
	}

	@Override
	public void onFinish(EventFilterSaveEntity wizardSaveEntity) {
		// getParentWizardViewModel().getWizardSaveEntity();
		invalidFilter = false;
		invalidFilterCondition = true;
		getSavedFilterInfo = wizardSaveEntity.getNewEventAlertRules();
		getRegulationSettings = wizardSaveEntity.getRegulationSettings();

		String Query = "";
		String strQType = (String) Sessions.getCurrent().getAttribute(
				"QueryType");
		if (strQType != null) {

			if(Sessions.getCurrent().getAttribute("isThisEnabled")!=null){
				eventCondition.set_enabled((boolean)Sessions.getCurrent().getAttribute("isThisEnabled"));
				Sessions.getCurrent().removeAttribute("isThisEnabled");
			}
			if (strQType.equals("Update")) {
				Query = UpdateData();
				logType = 61;
			} else {
				Query = InsertDataFromExisting();
				logType = 59;
			}
		} else {
			Query = InsertData();
			logType = 59;
		}
		
		String ruleName = getSavedFilterInfo.getName();
		String desc = (getSavedFilterInfo.getDescription() != null) ? getSavedFilterInfo
				.getDescription() : "";
		String logEntry = "";
		eventFilterRuleDescription = eventFilterRuleDescription.replaceAll("\t",
				"");
		eventFilterRuleDescription = "Name:  " + ruleName + "\r\nDescription:  "
				+ desc + "\r\n\r\nFilter:  " + eventFilterRuleDescription;
		logEntry = "INSERT INTO {0} (eventTime, logType, logUser, logSqlServer, logInfo) "
				+ "VALUES (GETUTCDATE(),"
				+ logType
				+ ",{1},'','"
				+ eventFilterRuleDescription + "');";
		Query += logEntry;
		
		
		Sessions.getCurrent().removeAttribute("ObjectMatchString");
		Sessions.getCurrent().removeAttribute("DbMatchString");
		Sessions.getCurrent().removeAttribute("AppMatchString");
		Sessions.getCurrent().removeAttribute("LoginMatchString");
		Sessions.getCurrent().removeAttribute("HostMatchString");
		Sessions.getCurrent().removeAttribute("sessionLoginMatchString");
		Sessions.getCurrent().removeAttribute("PrivilegedMatchString");
		Sessions.getCurrent().removeAttribute("SQL Server");
		Sessions.getCurrent().removeAttribute("Name");
		Sessions.getCurrent().removeAttribute("Description");
		Sessions.getCurrent().removeAttribute("TargetInst");
		Sessions.getCurrent().removeAttribute("EventConditionId");
		Sessions.getCurrent().removeAttribute("QueryType");
		Sessions.getCurrent().removeAttribute("eventTypeMatchString");	
		Sessions.getCurrent().removeAttribute("eventMatchString");
		Sessions.getCurrent().removeAttribute("eventType");
		Sessions.getCurrent().removeAttribute("isEnabled");

		InsertEventFilterData(Query);

		if(invalidFilterCondition){
			WebUtil.showWarningBoxWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.INVALID_EVENT_FILTER_CONDITION), 
					ELFunctions.getLabel(SQLCMI18NStrings.INVALID_FILTER_TITLE));
		}
		else if(invalidFilter){
			WebUtil.showWarningBoxWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.INVALID_EVENT_FILTER), 
					ELFunctions.getLabel(SQLCMI18NStrings.INVALID_FILTER_TITLE));
		}
		BindUtils.postNotifyChange(null, null, this, "entitiesModel");
		String uri = "eventFiltersView";
		uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
		Executions.sendRedirect(uri);
	}

	public String InsertDataFromExisting() {
		Map<String, String> EventConditionId = (Map<String, String>) Sessions
				.getCurrent().getAttribute("EventConditionId");

		String strEventData = (String) EventConditionId.get("0");
		String[] strArrayValues;
		strArrayValues = strEventData.split(",");

		int filterId = Integer.parseInt(strEventData);

		int enabled = 0;
		if (isEnabled()) {
			enabled = 1;
		}

		//String targetInstances = "WIN-DN3LAAJAHLT";
		String targetInstances = "<ALL>";
    	if(getRegulationSettings.getTargetInstances()!=null && getRegulationSettings.getTargetInstances().size()>0)
    	{
    		targetInstances = getRegulationSettings.getTargetInstances().keySet().toString();
    		targetInstances = targetInstances.substring(1,(targetInstances.length()-1)).replaceAll(", ", ";");
    	}

    	if(getRegulationSettings.getSQLServer()){
			if(targetInstances.equalsIgnoreCase("<ALL>")){
				targetInstances = "";
				invalidFilter = true;
				invalidFilterCondition = false;
			}
		}
    	
    	String FilterName = "New Event Filter";
    	if(getSavedFilterInfo.getName()!=null){
    		FilterName = getSavedFilterInfo.getName();
    	}  
    	
    	String description = "";
    	if(getSavedFilterInfo.getDescription()!=null){
    		description = getSavedFilterInfo.getDescription();
    	} 
    	    	
		String FinalQuery = "INSERT INTO SQLcompliance..EventFilters (name,description,eventType,targetInstances,enabled) VALUES('"
				+ FilterName
				+ "', '"
				+ description
				+ "', "
				+ 1
				+ ", '"
				+ targetInstances
				+ "',"
				+ enabled
				+ "); DECLARE @IDENTITY INT SELECT @IDENTITY = MAX(filterId) FROM SQLcompliance..EventFilters;";

		// Loop through it
		String objMatchString =(getRegulationSettings.getObjectMatchString() 
				== null)?"":getRegulationSettings.getObjectMatchString();
		if (getRegulationSettings.getObjectName()) {
			if(objMatchString.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 6;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ objMatchString  + "');";
		}

		String dbMatchString = (getRegulationSettings.getDbMatchString() 
				== null)?"":getRegulationSettings.getDbMatchString();
		if (getRegulationSettings.getDatabaseName()){
			if(dbMatchString.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 5;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ dbMatchString + "');";
		}

		String appMatchString = (getRegulationSettings.getAppMatchString() 
				== null)?"":getRegulationSettings.getAppMatchString();
		if (getRegulationSettings.getApplicationName()) {
			if(appMatchString.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 2;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ appMatchString + "');";
		}

		String logMatchString = (getRegulationSettings.getLoginMatchString() == 
				null)?"":getRegulationSettings.getLoginMatchString();
		if (getRegulationSettings.getLoginName()) {
			if(logMatchString.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 3;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ logMatchString + "');";
		}

		String hostMatchString = (getRegulationSettings.getHostMatchString() 
				== null)?"":getRegulationSettings.getHostMatchString();
		if (getRegulationSettings.getHostName()) {
			if(hostMatchString.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 10;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ hostMatchString + "');";
		}

		String sessionLoginMatch = (getRegulationSettings.getSessionLoginMatchString() 
				== null)?"":getRegulationSettings.getSessionLoginMatchString();
		if (getRegulationSettings.getSessionLoginName()) {
			if(sessionLoginMatch.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 11;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ getRegulationSettings.getSessionLoginMatchString()
					+ "');";
		}

		if (getRegulationSettings.getSessionPrivilegedMatchString() != null) {
			int fieldId = 7;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ getRegulationSettings.getSessionPrivilegedMatchString()
					+ "');";
		}
		
		if (getSavedFilterInfo.getEventFilterCategorMatchString() != null) {
			int fieldId = getSavedFilterInfo.getEventFilterCategoryFiledId();
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ getSavedFilterInfo.getEventFilterCategorMatchString()
					+ "');";
		}

		

		return FinalQuery;
	}

	public String InsertData() {
		int enabled = 0;
		if (isEnabled()) {
			enabled = 1;
		}
		String targetInstances = "<ALL>";
    	if(getRegulationSettings.getTargetInstances()!=null && getRegulationSettings.getTargetInstances().size()>0)
    	{
    		targetInstances = getRegulationSettings.getTargetInstances().keySet().toString();
    		targetInstances = targetInstances.substring(1,(targetInstances.length()-1)).replaceAll(", ", ";");
    	}
		if(getRegulationSettings.getSQLServer()){
			if(targetInstances.equalsIgnoreCase("<ALL>")){
				targetInstances = "";
				invalidFilter = true;
			}
		}
		String FilterName = "New Event Filter";
    	if(getSavedFilterInfo.getName()!=null){
    		FilterName = getSavedFilterInfo.getName();
    	}  
    	
    	String description = "";
    	if(getSavedFilterInfo.getDescription()!=null){
    		description = getSavedFilterInfo.getDescription();
    	}

		String FinalQuery = "INSERT INTO SQLcompliance..EventFilters (name,description,eventType,targetInstances,enabled) VALUES('"
				+ FilterName
				+ "', '"
				+ description
				+ "', "
				+ 1
				+ ", '"
				+ targetInstances
				+ "',"
				+ enabled
				+ "); DECLARE @IDENTITY INT SELECT @IDENTITY = MAX(filterId) FROM SQLcompliance..EventFilters;";

		// Loop through it
		String objMatchString =(getRegulationSettings.getObjectMatchString() 
				== null)?"":getRegulationSettings.getObjectMatchString();
		if (getRegulationSettings.getObjectName()) {
			if(objMatchString.equals("")){
				invalidFilter = true;
			}
			invalidFilterCondition = false;
			int fieldId = 6;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ objMatchString + "');";
		}

		String dbMatchString = (getRegulationSettings.getDbMatchString() 
				== null)?"":getRegulationSettings.getDbMatchString();
		if (getRegulationSettings.getDatabaseName()){
			if(dbMatchString.equals("")){
				invalidFilter = true;
			}
			invalidFilterCondition = false;
			int fieldId = 5;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ dbMatchString + "');";
		}

		String appMatchString = (getRegulationSettings.getAppMatchString() 
				== null)?"":getRegulationSettings.getAppMatchString();
		if (getRegulationSettings.getApplicationName()) {
			if(appMatchString.equals("")){
				invalidFilter = true;
			}
			invalidFilterCondition = false;
			int fieldId = 2;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ appMatchString + "');";
		}

		String logMatchString = (getRegulationSettings.getLoginMatchString() == 
				null)?"":getRegulationSettings.getLoginMatchString();
		if (getRegulationSettings.getLoginName()) {
			if(logMatchString.equals("")){
				invalidFilter = true;
			}
			invalidFilterCondition = false;			
			int fieldId = 3;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ logMatchString + "');";
		}

		String hostMatchString = (getRegulationSettings.getHostMatchString() 
				== null)?"":getRegulationSettings.getHostMatchString();
		if (getRegulationSettings.getHostName()) {
			if(hostMatchString.equals("")){
				invalidFilter = true;
			}
			invalidFilterCondition = false;
			int fieldId = 10;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ getRegulationSettings.getHostMatchString() + "');";
		}

		String sessionLoginMatch = (getRegulationSettings.getSessionLoginMatchString() 
				== null)?"":getRegulationSettings.getSessionLoginMatchString();
		if (getRegulationSettings.getSessionLoginName()) {
			if(sessionLoginMatch.equals("")){
				invalidFilter = true;
			}
			invalidFilterCondition = false;
			int fieldId = 11;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ getRegulationSettings.getSessionLoginMatchString()
					+ "');";
		}

		if (getRegulationSettings.getSessionPrivilegedMatchString() != null) {
			int fieldId = 7;
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ getRegulationSettings.getSessionPrivilegedMatchString()
					+ "');";
		}
		
		if (getSavedFilterInfo.getEventFilterCategorMatchString() != null) {
			invalidFilterCondition = false;
			int fieldId = getSavedFilterInfo.getEventFilterCategoryFiledId();
			FinalQuery += "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES (@IDENTITY, "
					+ fieldId
					+ ", '"
					+ getSavedFilterInfo.getEventFilterCategorMatchString()
					+ "');";
		}

		return FinalQuery;
	}

	public String UpdateData() {
		Map<String, String> EventConditionId = (Map<String, String>) Sessions
				.getCurrent().getAttribute("EventConditionId");

		String strEventData = (String) EventConditionId.get("0");
		String[] strArrayValues;
		strArrayValues = strEventData.split(",");

		int filterId = Integer.parseInt(strEventData);
		int enabled = 0;
		if (isEnabled()) {
			enabled = 1;
		}
		String targetInstances = "<ALL>";
    	if(getRegulationSettings.getTargetInstances()!=null && getRegulationSettings.getTargetInstances().size()>0)
    	{
    		targetInstances = getRegulationSettings.getTargetInstances().keySet().toString();
    		targetInstances = targetInstances.substring(1,(targetInstances.length()-1)).replaceAll(", ", ";");
    	}
    	if(getRegulationSettings.getSQLServer()){
    		if(targetInstances.equalsIgnoreCase("<ALL>")){
    			targetInstances = "";
    			invalidFilter = true;
    		}
    	}
		String FinalQuery = "UPDATE SQLcompliance..EventFilters SET name='"
				+ getSavedFilterInfo.getName() + "', description='"
				+ getSavedFilterInfo.getDescription() + "', eventType=" + 1
				+ ", targetInstances='" + targetInstances + "', enabled="
				+ enabled + "  WHERE filterId=" + filterId + "; ";

		// Loop through it
		String MatchString = "";
		String objMatchString =(getRegulationSettings.getObjectMatchString() 
				== null)?"":getRegulationSettings.getObjectMatchString();
		if (getRegulationSettings.getObjectName()) {
			int fieldId = 6;
			if(objMatchString.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	       			 +" BEGIN "
	       			 +"UPDATE SQLcompliance..EventFilterConditions SET matchString='"
					 + objMatchString
					 + "' WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+""
	          	     +" END "
	          	     + " ELSE "
	          	     +" BEGIN "
	          	     + "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES ("+filterId+", "
	          	     + fieldId
	          	     + ", '"
	          	     + objMatchString + "')"
	        	     +" END;";
		}
		else{
			int fieldId = 6;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	      			 +" BEGIN "
	      			 +"DELETE FROM SQLcompliance..EventFilterConditions "
	         	     + " where fieldId = "
	      			 +fieldId+" AND filterId = "
	         	     +filterId+" END;";
		}

		String dbMatchString = (getRegulationSettings.getDbMatchString() 
				== null)?"":getRegulationSettings.getDbMatchString();
		if (getRegulationSettings.getDatabaseName()){
			if(dbMatchString.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 5;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	       			 +" BEGIN "
	       			 +"UPDATE SQLcompliance..EventFilterConditions SET matchString='"
					 + dbMatchString
					 + "' WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+""
	          	     +" END "
	          	     + " ELSE "
	          	     +" BEGIN "
	          	     + "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES ("+filterId+", "
	          	     + fieldId
	          	     + ", '"
	          	     + dbMatchString + "')"
	        	     +" END;";
		}
		else{
			int fieldId = 5;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	      			 +" BEGIN "
	      			 +"DELETE FROM SQLcompliance..EventFilterConditions "
	         	     + " where fieldId = "
	      			 +fieldId+" AND filterId = "
	         	     +filterId+" END;";
		}

		String appMatchString = (getRegulationSettings.getAppMatchString() 
				== null)?"":getRegulationSettings.getAppMatchString();
		if (getRegulationSettings.getApplicationName()) {
			if(appMatchString.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 2;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	       			 +" BEGIN "
	       			 +"UPDATE SQLcompliance..EventFilterConditions SET matchString='"
					 + appMatchString
					 + "' WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+""
	          	     +" END "
	          	     + " ELSE "
	          	     +" BEGIN "
	          	     + "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES ("+filterId+", "
	          	     + fieldId
	          	     + ", '"
	          	     + appMatchString + "')"
	        	     +" END;";
		}
		else{
			int fieldId = 2;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	      			 +" BEGIN "
	      			 +"DELETE FROM SQLcompliance..EventFilterConditions "
	         	     + " where fieldId = "
	      			 +fieldId+" AND filterId = "
	         	     +filterId+" END;";
		}

		String logMatchString = (getRegulationSettings.getLoginMatchString() == 
				null)?"":getRegulationSettings.getLoginMatchString();
		if (getRegulationSettings.getLoginName()) {
			if(logMatchString.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 3;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	       			 +" BEGIN "
	       			 +"UPDATE SQLcompliance..EventFilterConditions SET matchString='"
					 + logMatchString
					 + "' WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+""
	          	     +" END "
	          	     + " ELSE "
	          	     +" BEGIN "
	          	     + "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES ("+filterId+", "
	          	     + fieldId
	          	     + ", '"
	          	     + logMatchString + "')"
	        	     +" END;";
		}
		else{
			int fieldId = 3;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	      			 +" BEGIN "
	      			 +"DELETE FROM SQLcompliance..EventFilterConditions "
	         	     + " where fieldId = "
	      			 +fieldId+" AND filterId = "
	         	     +filterId+" END;";
		}
		
		String hostMatchString = (getRegulationSettings.getHostMatchString() 
				== null)?"":getRegulationSettings.getHostMatchString();
		if (getRegulationSettings.getHostName()) {
			if(hostMatchString.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 10;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	       			 +" BEGIN "
	       			 +"UPDATE SQLcompliance..EventFilterConditions SET matchString='"
					 + hostMatchString
					 + "' WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+""
	          	     +" END "
	          	     + " ELSE "
	          	     +" BEGIN "
	          	     + "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES ("+filterId+", "
	          	     + fieldId
	          	     + ", '"
	          	     + getRegulationSettings.getHostMatchString() + "')"
	        	     +" END;";
		}
		else{
			int fieldId = 10;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	      			 +" BEGIN "
	      			 +"DELETE FROM SQLcompliance..EventFilterConditions "
	         	     + " where fieldId = "
	      			 +fieldId+" AND filterId = "
	         	     +filterId+" END;";
		}

		String sessionLoginMatch = (getRegulationSettings.getSessionLoginMatchString() 
				== null)?"":getRegulationSettings.getSessionLoginMatchString();
		if (getRegulationSettings.getSessionLoginName()) {
			if(sessionLoginMatch.equals("")){
				invalidFilter = true;
			}
				invalidFilterCondition = false;
			int fieldId = 11;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	       			 +" BEGIN "
	       			 +"UPDATE SQLcompliance..EventFilterConditions SET matchString='"
					 + sessionLoginMatch
					 + "' WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+""
	          	     +" END "
	          	     + " ELSE "
	          	     +" BEGIN "
	          	     + "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES ("+filterId+", "
	          	     + fieldId
	          	     + ", '"
	          	     + sessionLoginMatch + "')"
	        	     +" END;";
		}
		else{
			int fieldId = 11;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	      			 +" BEGIN "
	      			 +"DELETE FROM SQLcompliance..EventFilterConditions "
	         	     + " where fieldId = "
	      			 +fieldId+" AND filterId = "
	         	     +filterId+" END;";
		}

		if (getRegulationSettings.getSessionPrivilegedMatchString() != null) {
			int fieldId = 7;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	       			 +" BEGIN "
	       			 +"UPDATE SQLcompliance..EventFilterConditions SET matchString='"
					 + getRegulationSettings.getSessionPrivilegedMatchString()
					 + "' WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+""
	          	     +" END "
	          	     + " ELSE "
	          	     +" BEGIN "
	          	     + "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES ("+filterId+", "
	          	     + fieldId
	          	     + ", '"
	          	     + getRegulationSettings.getSessionPrivilegedMatchString() + "')"
	        	     +" END;";
		}
		else{
			int fieldId = 7;
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = "
	      			 +fieldId+" AND filterId = "
	       			 +filterId+")"
	      			 +" BEGIN "
	      			 +"DELETE FROM SQLcompliance..EventFilterConditions "
	         	     + " where fieldId = "
	      			 +fieldId+" AND filterId = "
	         	     +filterId+" END;";
		}
		
		if (getSavedFilterInfo.getEventFilterCategorMatchString() != null) {
			invalidFilterCondition = false;
			int fieldId = getSavedFilterInfo.getEventFilterCategoryFiledId();			
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE fieldId = 1 OR fieldId = 0"
	      			 +" AND filterId = "
	       			 +filterId+")"
	       			 +" BEGIN "
	       			 +"UPDATE SQLcompliance..EventFilterConditions SET matchString='"
					 + getSavedFilterInfo.getEventFilterCategorMatchString()+ "'"
					 +" ,fieldId=" +fieldId
					 + " WHERE (fieldId = 0 OR fieldId =1) AND filterId = "
	       			 +filterId+""
	          	     +" END "
	          	     + " ELSE "
	          	     +" BEGIN "
	          	     + "INSERT INTO SQLcompliance..EventFilterConditions (filterId, fieldId, matchString) VALUES ("+filterId+", "
	          	     + fieldId
	          	     + ", '"
	          	     + getSavedFilterInfo.getEventFilterCategorMatchString() + "')"
	        	     +" END;";
		}
		else{				
			FinalQuery += "IF EXISTS (SELECT * FROM SQLcompliance..EventFilterConditions WHERE (fieldId = "
		      			 +"1 or fieldId = 0) AND filterId = "
		       			 +filterId+")"
		      			 +" BEGIN "
		      			 +"DELETE FROM SQLcompliance..EventFilterConditions "
		         	     + " WHERE (fieldId = "
		      			 + "1 or fieldId = 0) AND filterId = "
		         	     +filterId+" END;";
			
		}
		return FinalQuery;
	}
	
	@Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
    }
}