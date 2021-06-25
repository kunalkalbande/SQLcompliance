package com.idera.sqlcm.ui.dialogs.addalertruleswizard.steps;

import com.idera.ccl.IderaDropdownList;
import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.facade.SNMPConfigData;
import com.idera.sqlcm.facade.SNMPConfigFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.KeyValueParser;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.SelectAlertActions;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifySQLServerViewModel;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Checkbox;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class AlertActionStepViewModel extends AddWizardStepBase {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/addalertruleswizard/steps/alert-action-step.zul";

	private boolean filterEventsAccessChecked = true;

	private Map<String, Long> checkedAlertActions = new HashMap<String, Long>();

	private CMAuditedActivities auditedActivities;
	private SelectAlertActions selectAlertActions;

	public static final String Email_Notification = "Email Notification";
	public static final String Windows_Event_Log_Entry = "Windows Event Log Entry";
	public static final String SNMP_TRAP = "SNMP Trap";
	private String alertMessageTitle = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_EVENT_ALERT_RULES_TITLE);
    private String alertMessageBody = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_EVENT_ALERT_RULES_MSG);
	String strMessageBody;
    String strMessageTitle;
    String strSeverity;
    
    List<String>items;
    
    
    @Wire
	private A messageAlertAction;
	
	@Wire
	private Checkbox chkEmailNotification;
	
	@Wire
	private Checkbox chkLogEntry;
	
	@Wire
	private String community;
	
	@Wire
	private A specifiedAddresses;
	
	@Wire
	private Textbox txtaddress;
	
	@Wire
	private Textbox txtcommunity;
	
	@Wire
	protected IderaDropdownList eventLog;
	
	@Wire
	private Spinner spnport;

	@Wire
	private Checkbox chkSNMPTrap;
	
	KeyValueParser keyValueParser;
	
	@Wire
	protected EventLogEntry eventLogEntry;
	@Wire
	protected EventCondition eventCondition;
	protected EventField eventField;
	private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCondition> conditionEvents;

	public static enum EventLogEntry {
		INFORMATION(ELFunctions.getLabel(SQLCMI18NStrings.INFORMATION), 0),
		WARNING(ELFunctions.getLabel(SQLCMI18NStrings.WARNING), 1),
		ERROR(ELFunctions.getLabel(SQLCMI18NStrings.ERROR), 3);

		private String label;

		private int id;

		EventLogEntry(String label, int id) {
			this.label = label;
			this.id = id;
		}

		public int getId() {
			return id;
		}
	}

	public CMAuditedActivities getAuditedActivities() {
		return auditedActivities;
	}

	public void setAuditedActivities(CMAuditedActivities auditedActivities) {
		this.auditedActivities = auditedActivities;
	}

	@Override
	public void onDoAfterWire() {
		
	}
	
	@Override
	protected void doOnShow(AddAlertRulesSaveEntity wizardSaveEntity) {
		
		String message = null;
		try {
			message = defaultMessageString();
		} catch (Exception e) {
			e.printStackTrace();
		}
        selectAlertActions.setMessage(message);
        if(Sessions.getCurrent().getAttribute("isValidSnmpAddress")==null){
	        try{
		        SNMPConfigFacade snmpConfigFacade = new SNMPConfigFacade();
				SNMPConfigData snmpConfigData = snmpConfigFacade.updateSnmpConfigData();
				txtaddress.setValue(snmpConfigData.getSnmpAddress());
				spnport.setValue(snmpConfigData.getPort());
				txtcommunity.setValue(snmpConfigData.getCommunity());
	        }
	        catch(RestException e){}
        }
        if( Sessions.getCurrent().getAttribute("QueryType")!=null)
        {
			conditionEvents = (List<CMAlertRulesCondition>) Sessions.getCurrent().getAttribute("conditionEvents");
			alertRules =(List<CMAlertRules>) Sessions.getCurrent().getAttribute("alertRules");
			if(conditionEvents!=null && alertRules!=null);
			{
				initializer(alertRules);
				BindUtils.postNotifyChange(null, null, AlertActionStepViewModel.this, "*");
			}
        }
	}
	
	 public String defaultMessageString() throws Exception{ 
		    eventCondition = new EventCondition();
		    eventCondition.set_messageTitle(alertMessageTitle);
			eventCondition.set_messageBody(alertMessageBody);
			String matchString =  eventCondition.UpdateMessageData(eventField,eventCondition);
			return matchString;
		    }


	 public void initializer(List<CMAlertRules> alertRules){	 
	 
	     if(alertRules.get(0).getSnmpTrap()==1){    
	    	 chkSNMPTrap.setChecked(true);
	    	 txtaddress.setDisabled(false);
	    	 spnport.setDisabled(false);
	    	 txtcommunity.setDisabled(false);
	    	 messageAlertAction.setDisabled(false);
		 }
	     
	     if(alertRules.get(0).getLogMessage()==1){    	
	    	 chkLogEntry.setChecked(true);
	    	 eventLog.setDisabled(false);
	    	 messageAlertAction.setDisabled(false);
	    }
	 	if(alertRules.get(0).getSnmpServerAddress()!=null && chkSNMPTrap.isChecked())
	 	{
	 		txtaddress.setValue(alertRules.get(0).getSnmpServerAddress());
	 	}
	 	if(alertRules.get(0).getSnmpPort()>=0 && chkSNMPTrap.isChecked())
	 	{
	 		spnport.setValue(alertRules.get(0).getSnmpPort());
	 		
	 	}
	 	if(alertRules.get(0).getSnmpCommunity()!=null  && chkSNMPTrap.isChecked())
	 	{
	 		txtcommunity.setValue(alertRules.get(0).getSnmpCommunity());
	 	}
	 	if(alertRules.get(0).getMessage()!=null){
	 		Map<String, String> messageData = new HashMap<String, String>();
	 		try {
	 			messageData = keyValueParser.ParseString(alertRules.get(0).getMessage());
	 			selectAlertActions.setEmailAddress(alertRules.get(0).getMessage());
	 			for (Map.Entry<String, String> entry : messageData
	 					.entrySet()) {
	
	 				if (entry.getKey().equals("body")) {
	 					strMessageBody = entry.getValue();
	 				}
	 				
	 				if (entry.getKey().equals("title")) {
	 					strMessageTitle = entry.getValue();
	 				}
	 				
	 				if(entry.getKey().equals("recipients") || entry.getKey().equals("recepients"))
	 				{
	 					items = Arrays.asList(entry.getValue().split("\\s*,\\s*"));					
	 		    		String chkName = chkEmailNotification.getName();
	 		    		long index = 0;
	 		    		checkedAlertActions.put(chkName,index);	 		    		
	 				} 				
	 				if(entry.getKey().equals("severity"))
	 				{
	 					strSeverity = entry.getValue();
	 					eventLog.setSelectedIndex(Integer.parseInt(strSeverity));
	 					if(strSeverity.equals("0"))
	 						selectAlertActions.setEventLogEntry("Information");
	 					else if(strSeverity.equals("1"))
	 						selectAlertActions.setEventLogEntry("Warning");
	 					else
	 						selectAlertActions.setEventLogEntry("Error");
	 				}
	 			}
	 			
	 			eventCondition.set_messageTitle(strMessageTitle);
	 			eventCondition.set_messageBody(strMessageBody); 			
	 			Sessions.getCurrent().setAttribute("specifyAlertMessage",eventCondition);
	 			
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}		
	 	  }
	 		if(alertRules.get(0).getEmailMessage()==1){
			 chkEmailNotification.setChecked(true);
			 messageAlertAction.setDisabled(false);
			 specifiedAddresses.setDisabled(false);
			 if(items !=null && items.size()>0)
				 Sessions.getCurrent().setAttribute("mailAddress", items); 
		 }
	 		if(Sessions.getCurrent().getAttribute("mailAddress")!=null){
					String mailString[] = new String[items.size()];
					for(int i=0;i<items.size();i++){
						mailString[i]=items.get(i);
					}
					eventCondition.set_targetStrings(mailString);
					Sessions.getCurrent().setAttribute("specifyAlertMessage",eventCondition);
					Sessions.getCurrent().setAttribute("addressList",mailString);
		}
   }
 
 
	public AlertActionStepViewModel() {
		super();
		selectAlertActions = new SelectAlertActions();
	}

	@Override
	public String getNextStepZul() {
		return SummaryStepViewModel.ZUL_PATH;
	}

	@Override
	public boolean isValid() {
		return true;
	}

	@Override
	public String getTips() {
		return ELFunctions.getLabel(SQLCMI18NStrings.SQL_ALERT_ACTIONS_TIPS);
	}	
	
	@Command("eventAlertRules")
    public void eventAlertRules(@BindingParam("id") long id) {
        SpecifySQLServerViewModel.showSpecifySQLServersDialog(id);
    }

	@Command("selectEventSource")
	public void selectEventSource(@BindingParam("id") String id) {
		eventLogEntry = EventLogEntry.valueOf(id);
		selectAlertActions.setEventLogEntry(eventLogEntry.label);
	}

	@Command("onCheck")
	public void onCheck(@BindingParam("target") Checkbox target,
			@BindingParam("index") long index,
			@BindingParam("lstBox") IderaDropdownList lstBox, 
			@BindingParam("lbl") A lbl,
			@BindingParam("lbl2") A lbl2,
			@BindingParam("txt") Textbox txt,
			@BindingParam("txt2") Textbox txt2, 
			@BindingParam("spn") Spinner spn) {
		String chkName = target.getName();
		if (target.isChecked()) {
			checkedAlertActions.put(chkName, index);
			switch (chkName) {
			case "Email Notification":
				lbl2.setDisabled(false);
				break;
			case "Windows Event Log Entry":			
				lstBox.setDisabled(false);
				eventLogEntry = EventLogEntry.valueOf(EventLogEntry.INFORMATION.toString());
				selectAlertActions.setEventLogEntry(eventLogEntry.label);
				break;
			case "SNMP Trap":			
				txt.setDisabled(false);
				spn.setDisabled(false);
				txt2.setDisabled(false);				
				break;
			}
		} else {
			checkedAlertActions.remove(target.getName());
			switch (chkName) {
			case "Email Notification":
				lbl2.setDisabled(true);
				break;
			case "Windows Event Log Entry":				
				lstBox.setDisabled(true);
				break;
			case "SNMP Trap":
				txt.setDisabled(true);
				spn.setDisabled(true);
				txt2.setDisabled(true);
				break;
			}
		}
		
		if(chkEmailNotification.isChecked() || chkLogEntry.isChecked() ||
				chkSNMPTrap.isChecked()){
			messageAlertAction.setDisabled(false);
		}
		else{
			messageAlertAction.setDisabled(true);
		}

		BindUtils.postNotifyChange(null, null, AlertActionStepViewModel.this,
				"regulationGuidelinesDesc");
	}
	
	
	 @Override
		public void onCancel(AddAlertRulesSaveEntity wizardSaveEntity) {
	    	if(Sessions.getCurrent().getAttribute("QueryType")!=null)
	    	{
	    		Sessions.getCurrent().removeAttribute("QueryType");
	    		Sessions.getCurrent().removeAttribute("Category");
	    	}
	    	
	    	 String uri = "instancesAlertsRule";
	         uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
	         Executions.sendRedirect(uri);
		}

	@Override
	public void onBeforeNext(AddAlertRulesSaveEntity wizardSaveEntity) {
		Sessions.getCurrent().setAttribute("isValidSnmpAddress", true);
		if (chkEmailNotification.isChecked()) {
				selectAlertActions.setEmailMessage(true);
				if(alertRules!=null)
					alertRules.get(0).setEmailMessage(1);
			}
		else{
			selectAlertActions.setEmailMessage(false);
			if(alertRules!=null)
				alertRules.get(0).setEmailMessage(0);
		}
		

		if(checkedAlertActions.containsKey(SNMP_TRAP) || chkSNMPTrap.isChecked()) {
			if(!txtaddress.getValue().equals("")){
				SNMPConfigFacade snmpConfigFacade = new SNMPConfigFacade();
				SNMPConfigData snmpConfigData = new SNMPConfigData();
				snmpConfigData.setSnmpAddress(txtaddress.getValue());
				snmpConfigData.setPort(spnport.getValue());
				snmpConfigData.setCommunity(txtcommunity.getValue());
				boolean isValid = false;
				try{
					isValid = snmpConfigFacade.checkSnmpAddress(snmpConfigData);
				}
				catch(RestException e){}
				Sessions.getCurrent().setAttribute("isValidSnmpAddress", isValid);
			}
			selectAlertActions.setSnmpTrap(true);
			selectAlertActions.setAddress(txtaddress.getValue());			
			selectAlertActions.setCommunity(txtcommunity.getValue());			
			selectAlertActions.setPort(spnport.getValue());
			if(alertRules!=null){
				alertRules.get(0).setSnmpServerAddress(txtaddress.getValue());
				alertRules.get(0).setSnmpCommunity(txtcommunity.getValue());
				alertRules.get(0).setSnmpPort(spnport.getValue());
				alertRules.get(0).setSnmpTrap(1);
			}
		}
		else
		{
			selectAlertActions.setSnmpTrap(false);
			selectAlertActions.setAddress(null);
			selectAlertActions.setCommunity(null);
			selectAlertActions.setPort(0);
			if(alertRules!=null)
				alertRules.get(0).setSnmpTrap(0);
		}
		
		if(chkEmailNotification.isChecked() || chkLogEntry.isChecked() ||
				chkSNMPTrap.isChecked()){
			if(Sessions.getCurrent().getAttribute("specifyAlertMessage")!=null )
				eventCondition = (EventCondition) Sessions.getCurrent().getAttribute("specifyAlertMessage"); 
			else{		        
				eventCondition.set_messageTitle(alertMessageTitle);
				eventCondition.set_messageBody(alertMessageBody);
			}
			try {
				String strMessageData = eventCondition.UpdateMessageData(eventField, eventCondition);
				selectAlertActions.setMessage(strMessageData);				
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		if (checkedAlertActions.containsKey(Windows_Event_Log_Entry) || chkLogEntry.isChecked()) {
			selectAlertActions.setLogMessage(true);
			String mainMessage=selectAlertActions.getMessage();
			int indexOfSeverity=mainMessage.indexOf("severity(1)");
			mainMessage=mainMessage.substring(0,indexOfSeverity+11)+ eventLog.getSelectedIndex() +mainMessage.substring(indexOfSeverity+12,mainMessage.length());
			selectAlertActions.setMessage(mainMessage);			
			if(alertRules!=null){
				alertRules.get(0).setMessage(mainMessage);
				alertRules.get(0).setLogMessage(1);
			}			
		}
		else
		{
			selectAlertActions.setLogMessage(false);
			if(alertRules!=null)
				alertRules.get(0).setLogMessage(0);
		}
		wizardSaveEntity.setSelectAlertActions(selectAlertActions);
	}
	
	@Override
	public void onBeforePrev(AddAlertRulesSaveEntity wizardSaveEntity){
		if(Sessions.getCurrent().getAttribute("isValidSnmpAddress")!=null){
			Sessions.getCurrent().setAttribute("isValidSnmpAddress", true);			
		}
		if(chkSNMPTrap.isChecked()){
			if(alertRules!=null){
				alertRules.get(0).setSnmpServerAddress(txtaddress.getValue());
				alertRules.get(0).setSnmpCommunity(txtcommunity.getValue());
				alertRules.get(0).setSnmpPort(spnport.getValue());
				alertRules.get(0).setSnmpTrap(1);
			}
			Sessions.getCurrent().setAttribute("isValidSnmpAddress", true);
		}
		else{
			if(alertRules!=null){
				alertRules.get(0).setSnmpTrap(0);
			}
		}
	}
	
	@Override
    public void onFinish(AddAlertRulesSaveEntity wizardSaveEntity){
    	onBeforeNext(wizardSaveEntity);
    	SummaryStepViewModel summaryStepViewModel=new SummaryStepViewModel();
    	summaryStepViewModel.onFinish(wizardSaveEntity);
    }

	public Textbox getTxtcommunity() {
		return txtcommunity;
	}

	public void setTxtcommunity(Textbox txtcommunity) {
		this.txtcommunity = txtcommunity;
	}

	public Textbox getTxtaddress() {
		return txtaddress;
	}

	public void setTxtaddress(Textbox txtaddress) {
		this.txtaddress = txtaddress;
	}
	
	public void setCommunity(String community) {
		this.community = community;
	}
	
	public String getCommunity() {
		return community;
	}
	public Spinner getTxtport() {
		return spnport;
	}

	public void setTxtport(Spinner spnport) {
		this.spnport = spnport;
	}

	public A getSpecifiedAddresses() {
		return specifiedAddresses;
	}

	public void setSpecifiedAddresses(A specifiedAddresses) {
		this.specifiedAddresses = specifiedAddresses;
	}

	@Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
    }
}
