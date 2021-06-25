package com.idera.sqlcm.ui.dialogs;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Properties;

import javax.mail.Session;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;
import org.zkoss.bind.annotation.AfterCompose;
import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.facade.GetSNMPConfigFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.facade.SNMPConfigData;
import com.idera.sqlcm.facade.SNMPConfigFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.smtpConfiguration.EmailUtil;
import com.idera.sqlcm.smtpConfiguration.GetSNMPConfigResponse;
import com.idera.sqlcm.smtpConfiguration.GetSNMPConfigResponseList;
import com.idera.sqlcm.smtpConfiguration.GetSNMPConfiguration;
import com.idera.sqlcm.snmpTrap.UpdateSNMPConfiguration;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.KeyValueParser;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.SelectAlertActions;

public class ThresholdNotificationViewModel  {

	private boolean filterEventsAccessChecked = true;

	private Map<String, Long> checkedAlertActions = new HashMap<String, Long>();
	
	private String help;
	GetSNMPConfiguration getSNMPConfiguration;
	UpdateSNMPConfiguration updateSNMPConfiguration;
	UpdateSNMPThresholdConfiguration updateSNMPThresholdConfiguration;
	
	private CMAuditedActivities auditedActivities;
	private SelectAlertActions selectAlertActions;

	public static final String Email_Notification = "Email Notification";
	public static final String Windows_Event_Log_Entry = "Windows Event Log Entry";
	public static final String SNMP_TRAP = "SNMP Trap";
	public static final String ZUL_URL_specifyAlertMessage = "~./sqlcm/dialogs/thresholdNotification/specifyAlertMessage.zul";
	private String alertMessageTitle = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_THRESHOLD_ALERT_RULES_TITLE);
    private String alertMessageBody = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_THRESHOLD_ALERT_RULES_MSG);
	
    @Wire
	private A messageAlertAction;
	
	private String emailAddress="";
	private String address="";
	private int port;
	boolean sendMailPermission = false;
	boolean logsPermission= false;
	boolean snmpPermission= false;
	boolean emailCheck= false;
	boolean winLogCheck= false;
	boolean snmpCheck= false;
	private int severity = -1;
	boolean warningSeverityChecked = false;
	boolean criticalSeverityChecked = false;
	boolean emailTextbox = false;
	boolean snmpAddressTextbox = false;
	boolean snmpPortTextbox= false;
	boolean snmpCommunityTextbox = false;
	boolean emailChkDisabled  = true;
	boolean winLogChkDisabled = true;
	boolean snmpChkDisabled = true;
	boolean messageAlert = true;
	public boolean isMessageAlert() {
		return messageAlert;
	}

	public void setMessageAlert(boolean messageAlert) {
		this.messageAlert = messageAlert;
	}

	public boolean isEmailChkDisabled() {
		return emailChkDisabled;
	}

	public void setEmailChkDisabled(boolean emailChkDisabled) {
		this.emailChkDisabled = emailChkDisabled;
	}

	public boolean isWinLogChkDisabled() {
		return winLogChkDisabled;
	}

	public void setWinLogChkDisabled(boolean winLogChkDisabled) {
		this.winLogChkDisabled = winLogChkDisabled;
	}

	public boolean isSnmpChkDisabled() {
		return snmpChkDisabled;
	}

	public void setSnmpChkDisabled(boolean snmpChkDisabled) {
		this.snmpChkDisabled = snmpChkDisabled;
	}
	
	
	public boolean isSnmpAddressTextbox() {
		return snmpAddressTextbox;
	}

	public void setSnmpAddressTextbox(boolean snmpAddressTextbox) {
		this.snmpAddressTextbox = snmpAddressTextbox;
	}

	public boolean isSnmpPortTextbox() {
		return snmpPortTextbox;
	}

	public void setSnmpPortTextbox(boolean snmpPortTextbox) {
		this.snmpPortTextbox = snmpPortTextbox;
	}

	public boolean isSnmpCommunityTextbox() {
		return snmpCommunityTextbox;
	}

	public void setSnmpCommunityTextbox(boolean snmpCommunityTextbox) {
		this.snmpCommunityTextbox = snmpCommunityTextbox;
	}

	public boolean isEmailCheckbox() {
		return emailTextbox;
	}

	public void setEmailCheckbox(boolean emailTextbox) {
		this.emailTextbox = emailTextbox;
	}

	public boolean isCriticalSeverityChecked() {
		return criticalSeverityChecked;
	}

	public void setCriticalSeverityChecked(boolean criticalSeverityChecked) {
		this.criticalSeverityChecked = criticalSeverityChecked;
	}

	public boolean isWarningSeverityChecked() {
		return warningSeverityChecked;
	}

	public void setWarningSeverityChecked(boolean warningSeverityChecked) {
		this.warningSeverityChecked = warningSeverityChecked;
	}

	@Wire
	private Checkbox warningChk;
	
	@Wire
	private Checkbox criticalChk;
	
	@Wire
	private Checkbox emailChk;
	
	@Wire
	private Checkbox winLogChk;
	
	@Wire
	private String community;
		
	@Wire
	private Textbox txtaddress,textMailAddress;
	
	@Wire
	private Textbox txtcommunity;
	
	@Wire
	private Listbox eventLog;
	
	@Wire
	private Spinner spnport;

	@Wire
	private Checkbox snmpChk;
	
	KeyValueParser keyValueParser;
	protected EventLogEntry eventLogEntry;
	protected EventCondition eventCondition;
	protected EventField eventField;
	private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCondition> conditionEvents;
	
	/*public ThresholdNotificationViewModel() throws Exception{
		
	}*/

	public static enum EventLogEntry {
		INFORMATION(ELFunctions.getLabel(SQLCMI18NStrings.INFORMATION), 0), WARNING(
				ELFunctions.getLabel(SQLCMI18NStrings.WARNING), 1), ERROR(
				ELFunctions.getLabel(SQLCMI18NStrings.ERROR), 3);

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

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) throws Exception{
		Selectors.wireComponents(view, this, false);
		getUpdateSNMPConfiguration();
	}	
	
	
	 public String defaultMessageString() throws Exception{ 
		    eventCondition = new EventCondition();
		    eventCondition.set_messageTitle(alertMessageTitle);
			eventCondition.set_messageBody(alertMessageBody);
			String matchString =  eventCondition.UpdateMessageData(eventField,eventCondition);			
			return matchString;
     }
 	
	public boolean isValid() {
		return true;
	}

	private A getMessageAlertAction() {
		return messageAlertAction;
	}

	private void setMessageAlertAction(A id) {
		this.messageAlertAction = id;
	}
	
	@Command("isWarningThreshold")
	public void isWarningThreshold(@BindingParam("warningChk") Checkbox warningChk,
			@BindingParam("criticalChk") Checkbox criticalChk,
			@BindingParam("chkbox1") Checkbox chkbox1,
			@BindingParam("chkbox2") Checkbox chkbox2,
			@BindingParam("chkbox3")Checkbox chkbox3,
			@BindingParam("btn1") Button btn1,
			@BindingParam("btn2") Button btn2,
			@BindingParam("txt1") Textbox txt1,
			@BindingParam("txt2") Textbox txt2,
			@BindingParam("txt3") Textbox txt3,
			@BindingParam("spn") Spinner spn,		
			@BindingParam("message") A message
			){
			if (warningChk.isChecked() || criticalChk.isChecked()){
				chkbox1.setDisabled(false);
				chkbox2.setDisabled(false);
				chkbox3.setDisabled(false);
				btn1.setDisabled(false);
				btn2.setDisabled(false);
				if(chkbox1.isChecked()==true){
					txt1.setDisabled(false);
				}
				if (chkbox3.isChecked()==true ) {
					txt2.setDisabled(false);
					txt3.setDisabled(false);
					spn.setDisabled(false);
				} 
				if(chkbox1.isChecked()||chkbox2.isChecked()||chkbox3.isChecked()){
					message.setDisabled(false);
				}
			}
			else {
				chkbox1.setDisabled(true);
				chkbox2.setDisabled(true);
				chkbox3.setDisabled(true);
				btn1.setDisabled(false);
				btn2.setDisabled(false);		
				txt1.setDisabled(true);
				txt2.setDisabled(true);
				txt3.setDisabled(true);
				spn.setDisabled(true);
				message.setDisabled(true);
			}
		}
	
	@Command("onCheckMail")
	public void onCheckMail(){
		if(emailChk.isChecked()){
			textMailAddress.setDisabled(false);
		}
		else{
			textMailAddress.setDisabled(false);
		}			
		if(emailChk.isChecked()||winLogChk.isChecked()||snmpChk.isChecked()){
			messageAlertAction.setDisabled(false);
		}
		else {
			messageAlertAction.setDisabled(true);
		}
	}
	
	@Command("onCheckLog")
	public void onCheckLog(){
		if(emailChk.isChecked()||winLogChk.isChecked()||snmpChk.isChecked()){
			messageAlertAction.setDisabled(false);
		}
		else {
			messageAlertAction.setDisabled(true);
		}
	}
	
	@Command("onCheckSnmp")
	public void onCheckSnmp(){
		if(snmpChk.isChecked())
		{
			txtaddress.setDisabled(false);
			spnport.setDisabled(false);
			txtcommunity.setDisabled(false);
		}
		else{
			txtaddress.setDisabled(true);
			spnport.setDisabled(true);
			txtcommunity.setDisabled(true);
		}
		if(emailChk.isChecked()||winLogChk.isChecked()||snmpChk.isChecked()){
			messageAlertAction.setDisabled(false);
		}
		else {
			messageAlertAction.setDisabled(true);
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

	public boolean isEmailCheck() {
		return emailCheck;
	}

	public void setEmailCheck(boolean emailCheck) {
		this.emailCheck = emailCheck;
	}

	public boolean isWinLogCheck() {
		return winLogCheck;
	}

	public void setWinLogCheck(boolean winLogCheck) {
		this.winLogCheck = winLogCheck;
	}

	public boolean isSnmpCheck() {
		return snmpCheck;
	}

	public void setSnmpCheck(boolean snmpCheck) {
		this.snmpCheck = snmpCheck;
	}

	@Command
	public String getHelp() {
        return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
    }
	
	String snmpServerAddress;
	int snmpPort;
	String snmpCommunity;
	String msgData = "";
	
	public void UpdateSNMPThresholdConfiguration() throws Exception {
		InstancePropertiesViewModel instancePropertiesViewModel = new InstancePropertiesViewModel();
		long instanceid = instancePropertiesViewModel.getCurrentInstanceValue();
		String instanceName = InstancesFacade.getInstanceDetails(instanceid).getInstanceName();
		if(Sessions.getCurrent().getAttribute("set_messageTitle")!=null && 
		Sessions.getCurrent().getAttribute("set_messageBody")!=null){
			alertMessageBody = (String)Sessions.getCurrent().getAttribute("set_messageBody");
			alertMessageTitle =(String)Sessions.getCurrent().getAttribute("set_messageTitle");			
		}
		
		alertMessageTitle ="title("+alertMessageTitle.length()+")"+ alertMessageTitle;
		alertMessageBody = "body("+alertMessageBody.length()+")"+ alertMessageBody;
		msgData = alertMessageBody + alertMessageTitle;
		updateSNMPThresholdConfiguration = new UpdateSNMPThresholdConfiguration();
		updateSNMPThresholdConfiguration.setInstanceName(instanceName);
		updateSNMPThresholdConfiguration.setSenderEmail(getEmailAddress());
		updateSNMPThresholdConfiguration.setSendMailPermission(sendMailPermission);
		updateSNMPThresholdConfiguration.setSnmpPermission(snmpPermission);
		updateSNMPThresholdConfiguration.setLogsPermission(logsPermission);
		updateSNMPThresholdConfiguration.setSnmpServerAddress(address);
		updateSNMPThresholdConfiguration.setPort(port);
		updateSNMPThresholdConfiguration.setCommunity(community);
		updateSNMPThresholdConfiguration.setSeverity(severity);
		updateSNMPThresholdConfiguration.setSrvId(instanceid);
		updateSNMPThresholdConfiguration.setMessageData(msgData);
		try {
			SNMPConfigFacade
					.getUpdateSNMPThresholdConfiguration(updateSNMPThresholdConfiguration);
			//refreshEntitiesList();
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
		}
	}
	
	public void UpdateSNMPConfiguration() {
		updateSNMPConfiguration = new UpdateSNMPConfiguration();
		updateSNMPConfiguration.setCommunity(community);
		if(port>=49451)
		{
			port=49451;
		}
		updateSNMPConfiguration.setPort(port);
		updateSNMPConfiguration.setSnmpServerAddress(address);
		try {
			SNMPConfigFacade
					.getUpdateSNMPConfiguration(updateSNMPConfiguration);
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
		}
	}
	
		
	@Command("okCommand")
	public void okCommand(@BindingParam("comp") Window x,
			@BindingParam("chkbox") Checkbox chkbox,
			@BindingParam("chkbox2") Checkbox chkbox2,
			@BindingParam("chkbox3")Checkbox chkbox3,
			@BindingParam("chkbox4")Checkbox chkbox4,
			@BindingParam("chkbox5")Checkbox chkbox5
			) throws Exception{
		boolean isValid = true;
		InstancePropertiesViewModel instancePropertiesViewModel = new InstancePropertiesViewModel();
		long instanceid = instancePropertiesViewModel.getCurrentInstanceValue();
		String instanceName = InstancesFacade.getInstanceDetails(instanceid).getInstanceName();
		if(!chkbox4.isChecked() && !chkbox5.isChecked())
		{
			try {
				SNMPConfigFacade
						.deleteThresholdConfiguration(instanceName);
			} catch (RestException e) {
				WebUtil.showErrorBox(e,
						SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
			}
		}
		else{
			if (chkbox.isChecked()) {
				sendMailPermission = true;
				//SendSMTPMail();
			}
			else if(!chkbox.isChecked())
				emailAddress = "";
			
			if(chkbox2.isChecked()){
				logsPermission= true;
				
			}
			if(chkbox4.isChecked() && !chkbox5.isChecked()){
				severity=0;
			}
			if(chkbox5.isChecked() && !chkbox4.isChecked()){
				severity=1;
			}
			
			if(chkbox4.isChecked() && chkbox5.isChecked()){
				severity=2; 
			}
			
			if(chkbox3.isChecked()){
				snmpPermission= true;
				//UpdateSNMPConfiguration();	
			}
			if (!address.equals("")) {
				isValid = false;
				SNMPConfigFacade snmpConfigFacade = new SNMPConfigFacade();
				SNMPConfigData snmpConfigData = new SNMPConfigData();
				snmpConfigData.setSnmpAddress(address);
				snmpConfigData.setPort(port);
				snmpConfigData.setCommunity(community);
				try {
					isValid = snmpConfigFacade.checkSnmpAddress(snmpConfigData);
				} 
				catch (RestException e) {
				}
			}
			if(isValid == true){
				UpdateSNMPThresholdConfiguration();
			}
			else{
				WebUtil.showWarningBoxWithCustomMessage("Unable to reach address '"+address+"'.", "Invalid Data");
			}
		}
		if(isValid){
			closeDialog(x);
		}
	}
	
	@Command("cancelCommand")
	public void cancelButtonClick(@BindingParam("comp") Window x){
		closeDialog(x);
	}
	
	@Command("closeDialog")
	public void closeDialog(@BindingParam("comp") Window x){
		x.detach();
		Sessions.getCurrent().removeAttribute("set_messageTitle");
		Sessions.getCurrent().removeAttribute("set_messageBody");
	}
	 
	public void getUpdateSNMPConfiguration() throws RestException{
		address = "";			
		port = 162;	
		community = "public";
		Boolean logPermission = false;
		Boolean sendMailPermission = false;		
		Boolean snmpPermission = false;
		String messageData = "";
		String msgBody = "";
		String msgTitle = "";
		int severity = -1;
		InstancePropertiesViewModel instancePropertiesViewModel = new InstancePropertiesViewModel();
		long instanceid = instancePropertiesViewModel.getCurrentInstanceValue();		
		String instanceName = InstancesFacade.getInstanceDetails(instanceid).getInstanceName();
		GetSNMPConfiguration getSNMPConfiguration=new GetSNMPConfiguration();
		getSNMPConfiguration.setInstanceName(instanceName);
		GetSNMPConfigResponse getSNMPConfigResponse = new GetSNMPConfigResponse();
		getSNMPConfigResponse=GetSNMPConfigFacade.getSNMPConfiguration(getSNMPConfiguration);
		List<GetSNMPConfigResponseList> configResponse = getSNMPConfigResponse.getGetSNMPConfigResponse();
		int configSize = configResponse.size();
		if(configSize == 0){
			SNMPConfigFacade snmpConfigFacade = new SNMPConfigFacade();
			SNMPConfigData snmpConfigData = snmpConfigFacade.updateSnmpConfigData();
			if(snmpConfigData.getSnmpAddress()!=null)
				setAddress(snmpConfigData.getSnmpAddress());
			else
				setAddress("");
			setPort(snmpConfigData.getPort());
			if(snmpConfigData.getCommunity()!=null)
				setCommunity(snmpConfigData.getCommunity());
			else
				setCommunity("");
		}
		for (int i = 0; i < configSize; i++) {
		setEmailAddress(configResponse.get(i).getSenderEmail());
		setAddress(configResponse.get(i).getSnmpServerAddress());
		@SuppressWarnings("unused")
		int str=	configResponse.get(i).getSnmpPort();
		setPort(configResponse.get(i).getSnmpPort());
		setCommunity(configResponse.get(i).getSnmpCommunity());
		logPermission=configResponse.get(i).getLogsPermission();
		sendMailPermission=configResponse.get(i).getSendMailPermission();		
		snmpPermission=configResponse.get(i).getSnmpPermission();
		severity=configResponse.get(i).getSeverity();
		messageData = configResponse.get(i).getMessageData();
		int titleIndex = 0;
		int bodyIndex = 0;
			if(messageData != null){
				titleIndex = messageData.indexOf("title(");
				bodyIndex = messageData.indexOf("body(");
				if(bodyIndex != -1){
					msgBody = messageData.substring(messageData.indexOf(')')+1,titleIndex);
				}
				if(titleIndex!=-1){
					msgTitle = messageData.substring(messageData.lastIndexOf(')')+1,messageData.length());
				}
				Sessions.getCurrent().setAttribute("set_messageTitle", msgTitle);
				Sessions.getCurrent().setAttribute("set_messageBody",msgBody);
			}			
		}
		if(severity == 0){
			warningSeverityChecked = true;
			emailChkDisabled  = false;
			winLogChkDisabled = false;
			snmpChkDisabled = false;
		}
		else if(severity == 1) {
			criticalSeverityChecked = true;
			emailChkDisabled  = false;
			winLogChkDisabled = false;
			snmpChkDisabled = false;
		}
		else if(severity == 2) {
			warningSeverityChecked = true;
			criticalSeverityChecked = true;
			emailChkDisabled  = false;
			winLogChkDisabled = false;
			snmpChkDisabled = false;
		}
		else{
			emailChkDisabled  = true;
			winLogChkDisabled = true;
			snmpChkDisabled = true;
		}
		if(sendMailPermission == false)
		{
			emailTextbox = true;
		}
		
		if(snmpPermission == false)
		{
			snmpAddressTextbox=true;
			snmpPortTextbox=true;
			snmpCommunityTextbox=true;
		}
		setMessageAlert(true);
		if (logPermission==true) {
			setWinLogCheck(true);
			setMessageAlert(false);
		}
		if (sendMailPermission==true) {
			setEmailCheck(true);
			setMessageAlert(false);
		}
		if (snmpPermission==true) {
			setSnmpCheck(true);
			setMessageAlert(false);
		}
	}
	@Command("alertMessageLink")
	public void alertMessageLink(){
		Window window = (Window) Executions.createComponents(
				this.ZUL_URL_specifyAlertMessage, null,
				null);
		window.doHighlighted();
	}
}
 