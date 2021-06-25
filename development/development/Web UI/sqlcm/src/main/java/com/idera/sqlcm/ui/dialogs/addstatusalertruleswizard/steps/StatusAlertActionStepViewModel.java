package com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps;

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
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.KeyValueParser;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.SelectAlertActions;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesSaveEntity;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class StatusAlertActionStepViewModel extends AddWizardStepBase {
    KeyValueParser keyValueParser;
    public static final String ZUL_PATH = "~./sqlcm/dialogs/addstatusalertruleswizard/steps/status-alert-action-step.zul";
    public static final String ZUL_URL_specifyAlertMessage = "~./sqlcm/dialogs/addstatusalertruleswizard/steps/specifyAlertMessage.zul";
	public static final String ZUL_URL_specifyMailAddresses = "~./sqlcm/dialogs/addstatusalertruleswizard/steps/specifyMailAddresses.zul";
    private Map<String,Long> checkedAlertActions = new HashMap<String,Long>();
    public static final String INSTANCE_ID = "instance-id";
    private CMAuditedActivities auditedActivities;
    private SelectAlertActions selectAlertActions;
    private EventCondition eventCondition;
    private EventField  eventtype;
    public static final String Email_Notification = "Email Notification";
    public static final String Windows_Event_Log_Entry = "Windows Event Log Entry";
    public static final String SNMP_TRAP = "SNMP Trap";
    private String alertMessageTitle = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_STATUS_ALERT_RULES_TITLE);
    private String alertMessageBody = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_STATUS_ALERT_RULES_MSG);
    protected EventLogEntry eventLogEntry;
    @Wire
    private A messageAlertAction;
    @Wire 
    private A specifiedAddresses;
    
	private String emailAddress;
	private String address;
	private int port;
	
	String strMessageBody;
    String strMessageTitle;
    String strSeverity;
    
    List<String>items;
	
	@Wire
	private String community;
	
	@Wire
	private Textbox txtEmailAddress;
	
	@Wire
	private Textbox txtaddress;
	
	@Wire
	private Textbox txtcommunity;
	
	@Wire
	private Checkbox chkLogEntry;
	
	@Wire 
	private Checkbox chkEmailNotification;
	
	@Wire 
	private IderaDropdownList eventLog;
	
	@Wire
	private Spinner spnport;
	private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCondition> conditionEvents;
	
	@Wire
	Checkbox chkSNMPTrap;
	
    public static enum EventLogEntry
    {
    	INFORMATION(ELFunctions.getLabel(SQLCMI18NStrings.INFORMATION), 1),
    	WARNING(ELFunctions.getLabel(SQLCMI18NStrings.WARNING), 2),
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
        auditedActivities = new CMAuditedActivities();
    }

    public StatusAlertActionStepViewModel() throws Exception {
        super();
        selectAlertActions = new SelectAlertActions();
        selectAlertActions.setLogMessage(false);
        selectAlertActions.setEmailMessage(false);
        selectAlertActions.setSnmpTrap(false);
    }
    
    @Override
	public void onCancel(AddStatusAlertRulesSaveEntity wizardSaveEntity) {
    	if(Sessions.getCurrent().getAttribute("QueryType")!=null)
    	{
    		Sessions.getCurrent().removeAttribute("QueryType");
    	}
    	 String uri = "instancesAlertsRule";
         uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
         Executions.sendRedirect(uri);
	}
    
    @Override
    protected void doOnShow(AddStatusAlertRulesSaveEntity wizardSaveEntity) {
    	String message = null;
		try {
			message = defaultMessageString();
		} catch (Exception e) {
			// TODO Auto-generated catch block
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
        if(Sessions.getCurrent().getAttribute("QueryType")!=null){ 
        conditionEvents = (List<CMAlertRulesCondition>) Sessions.getCurrent().getAttribute("conditionEvents");
        alertRules =(List<CMAlertRules>) Sessions.getCurrent().getAttribute("alertRules");
        	if(conditionEvents!=null && alertRules!=null);
        	{
        		initializer(alertRules);
        		BindUtils.postNotifyChange(null, null, StatusAlertActionStepViewModel.this, "*");
        	}
        }
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
		 		setAddress(alertRules.get(0).getSnmpServerAddress());
		 	}
		 	if(alertRules.get(0).getSnmpPort()>=0 && chkSNMPTrap.isChecked())
		 	{
		 		spnport.setValue(alertRules.get(0).getSnmpPort());
		 		
		 	}
		 	if(alertRules.get(0).getSnmpCommunity()!=null && chkSNMPTrap.isChecked())
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
		 				
		 				if(entry.getKey().equals("recipients")||entry.getKey().equals("recepients"))
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
				 if(items.size()>0)
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
    

	@Override
	public void onCancel(long instanceId) {
		Sessions.getCurrent().removeAttribute("conditionEvents");
		Sessions.getCurrent().removeAttribute("alertRules");
	}

    
    public String defaultMessageString() throws Exception{ 
    eventCondition = new EventCondition();
    eventCondition.set_messageTitle(alertMessageTitle);
	eventCondition.set_messageBody(alertMessageBody);
	String matchString =  eventCondition.UpdateMessageData(eventtype,eventCondition);
	
	return matchString;
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

        return ELFunctions.getLabel(SQLCMI18NStrings.SQL_STATUS_ALERT_ACTIONS_TIPS);
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
				//setMessageAlertAction(lbl);
				//lbl.setDisabled(false);
				lbl2.setDisabled(false);
				break;
			case "Windows Event Log Entry":
				//lbl.setDisabled(false);
				lstBox.setDisabled(false);
				eventLogEntry = EventLogEntry.valueOf(EventLogEntry.INFORMATION.toString());
				selectAlertActions.setEventLogEntry(eventLogEntry.label);
				break;
			case "SNMP Trap":
				//lbl.setDisabled(false);
				txt.setDisabled(false);
				spn.setDisabled(false);
				txt2.setDisabled(false);
				break;
			}
		}
		else {
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

		BindUtils.postNotifyChange(null, null, StatusAlertActionStepViewModel.this,
				"regulationGuidelinesDesc");
	}

	@Override
	public void onBeforeNext(AddStatusAlertRulesSaveEntity wizardSaveEntity) {
		Sessions.getCurrent().setAttribute("isValidSnmpAddress", true);
		if (checkedAlertActions.containsKey(Email_Notification) && chkEmailNotification.isChecked()) {
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
				alertRules.get(0).setSnmpServerAddress(txtaddress.getValue());
				alertRules.get(0).setSnmpTrap(1);
				alertRules.get(0).setSnmpCommunity(txtcommunity.getValue());
				alertRules.get(0).setEmailMessage(1);
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
				String strMessageData = eventCondition.UpdateMessageData(eventtype, eventCondition);
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
				alertRules.get(0).setLogMessage(1);
				alertRules.get(0).setMessage(mainMessage);
				
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
	public void onBeforePrev(AddStatusAlertRulesSaveEntity wizardSaveEntity){
		if(Sessions.getCurrent().getAttribute("isValidSnmpAddress")!=null){
			Sessions.getCurrent().setAttribute("isValidSnmpAddress", true);
		}
		if(chkSNMPTrap.isChecked()){
			if(alertRules!=null){
				alertRules.get(0).setSnmpServerAddress(txtaddress.getValue());
				alertRules.get(0).setSnmpCommunity(txtcommunity.getValue());
				alertRules.get(0).setSnmpPort(spnport.getValue());
				alertRules.get(0).setSnmpServerAddress(txtaddress.getValue());
				alertRules.get(0).setSnmpTrap(1);
				alertRules.get(0).setSnmpCommunity(txtcommunity.getValue());
			}
			Sessions.getCurrent().setAttribute("isValidSnmpAddress", true);
		}
		else{
			if(alertRules!=null)
				alertRules.get(0).setSnmpTrap(0);
		}
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

	public String getEmailAddress() {
		return emailAddress;
	}

	public void setEmailAddress(String emailAddress) {
		this.emailAddress = emailAddress;
	}
	
	public int getPort() {
		return port;
	}

	public void setPort(int port) {
		this.port = port;
	}
	
	public String getAddress() {
		return address;
	}
	
	public void setAddress(String address) {
		this.address = address;
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

	public Textbox getTxtEmailAddress() {
		return txtEmailAddress;
	}

	public void setTxtEmailAddress(Textbox txtEmailAddress) {
		this.txtEmailAddress = txtEmailAddress;
	}
	
	@Command("eventAlertRules")
    public void eventAlertRules(@BindingParam("id") long id) {
		StatusAlertActionStepViewModel.showSpecifySQLServersDialog(id);
    }
	
	public static void showSpecifySQLServersDialog(Long instanceId) {
		if (instanceId == null) {
			throw new RuntimeException(" Instance Id must not be null! ");
		}
		Map<String, Object> args = new HashMap<>();
		args.put(INSTANCE_ID, instanceId);
		
		if (instanceId == 7) {
			Window window = (Window) Executions.createComponents(
					StatusAlertActionStepViewModel.ZUL_URL_specifyAlertMessage, null,
					args);
			window.doHighlighted();
			}
		if (instanceId == 8) {
			Window window = (Window) Executions.createComponents(
					StatusAlertActionStepViewModel.ZUL_URL_specifyMailAddresses, null,
					args);
			window.doHighlighted();
			}
	}
	
	 @Override
	    public String getHelpUrl() {
	        return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
	    }
}
