package com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMPermission;
import com.idera.sqlcm.entities.CMPermissionInfo;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.instances.CMInstanceProperties;
import com.idera.sqlcm.enumerations.DefaultDBPermission;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.converter.PermissionStatusToCssStyleConverter;
import com.idera.sqlcm.ui.converter.PermissionStatusToImagePathConverter;
import com.idera.sqlcm.ui.converter.PermissionStatusToLabelConverter;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.CMThresholdAdapter;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.KeyValueParser;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.EventType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps.DataAlertActionStepViewModel;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.permissionFailDialog.PermissionFailConfirmViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyHostNameViewModel.Category;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyHostNameViewModel.Host;


import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyMailAddressesViewModel.Login;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

import javax.mail.internet.InternetAddress;

public class SpecifyMailAddressesViewModel implements AddPrivilegedUsersViewModel.DialogListener {
	private String help;
	EventCondition eventCondition = new EventCondition();
    EventField eventtype = new EventField();
	public String[]  targetString = {};
	public String getHelp() {
		return this.help;
	}
    
	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@Override
	public void onOk(long instanceId,List<CMInstancePermissionBase> selectedPermissionList) {
		
	}

	@Override
	public void onCancel(long instanceId) {
		
		
	}

	
	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) throws Exception { 
		help = "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
		if(Sessions.getCurrent().getAttribute("addressList")!=null){
			String addressList[] = (String[])Sessions.getCurrent().getAttribute("addressList");
			for(int i=0;i<addressList.length;i++){
				Login login = new Login(addressList[i]);
    			dataList.add(login);
			}
			BindUtils.postNotifyChange(null, null, SpecifyMailAddressesViewModel.this, "*");
		}
        else if(Sessions.getCurrent().getAttribute("QueryType")!=null){         	
        	if(Sessions.getCurrent().getAttribute("mailAddress")!=null){
        		List<String> items= (List<String>)Sessions.getCurrent().getAttribute("mailAddress");
        		for (String temp : items) {
        			Login login = new Login(temp);
        			dataList.add(login);
        		}       
           	}
        	BindUtils.postNotifyChange(null, null, SpecifyMailAddressesViewModel.this, "*");
        }
    }
    
    
	
	
	
	public class PermissionFailConfirmDialogListenerImpl implements
			PermissionFailConfirmViewModel.PermissionFailConfirmDialogListener {
		@Override
		public void onIgnore() {
			/* getNextButton().setDisabled(false); */
		}

		@Override
		public void onReCheck() {
			// do nothing
		}
	}

 //4.1.1.5
	public static enum Category {
		LISTED(1, ELFunctions.getLabel(SQLCMI18NStrings.LISTED)),//TODO AS ask .NET team id
		EXCEPT_LISTED(2, ELFunctions.getLabel(SQLCMI18NStrings.EXCEPT_LISTED));
    	private String label;
        private int index;

        private Category(int index, String label) {
            this.label = label;
            this.index = index;

        }

        public String getLabel() {
            return label;
        }

        public String getName() {
            return this.name();
        }

        public int getIndex() {
            return index;
        }
    }
	  private Category currentInterval = Category.LISTED;
	  private ListModelList<Category> intervalListModelList;
	  private void initIntervalList(int selectedIndex) {
	        intervalListModelList = new ListModelList<>();
	        intervalListModelList.add(Category.LISTED);
	        intervalListModelList.add(Category.EXCEPT_LISTED);
	        currentInterval = intervalListModelList.get(selectedIndex);
	        intervalListModelList.setSelection(Arrays.asList(currentInterval));
	    }
	
	@Command("selectAddEventFilter")
    public void selectAddEventFilter(@BindingParam("radioGroup") Radiogroup radioGroup) throws RestException {
    	int iSelected = radioGroup.getSelectedIndex();
    	initIntervalList(iSelected);
    	Set<Category> selectedIntervals = intervalListModelList.getSelection(); // must contain only 1 item because single selection mode.
        if (selectedIntervals != null && !selectedIntervals.isEmpty()) {
            for (Category i : selectedIntervals) {
                currentInterval = i;
                Sessions.getCurrent().setAttribute("specifyLoginRadio",currentInterval.label);
                break;
            }
        }
    }
	
	
	
	@Command("submitChoice")
	public void submitChoice(@BindingParam("comp") Window x) throws Exception{
		if(Sessions.getCurrent().getAttribute("specifyAlertMessage")!=null)
		{
		eventCondition = (EventCondition) Sessions.getCurrent().getAttribute("specifyAlertMessage");
		}
		else
		{
			initData();
		}
		
		if(dataList!=null && (!dataList.isEmpty()))
		{
			eventCondition.set_targetStrings(GetTargetString());
		}
		Sessions.getCurrent().setAttribute("specifyAlertMessage",eventCondition);
		Sessions.getCurrent().setAttribute("addressList",GetTargetString());		
		x.detach();
	}
	
	public void initData(){
		eventCondition = new EventCondition();
		String alertMessageTitle = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_STATUS_ALERT_RULES_TITLE);
		String alertMessageBody =  ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_STATUS_ALERT_RULES_MSG);
		eventCondition.set_messageTitle(alertMessageTitle);
		eventCondition.set_messageBody(alertMessageBody);	
	}
    
	public String[] GetTargetString()
	{   
		targetString = new String[dataList.getSize()];
		for (int j = 0; j < dataList.getSize(); j++) {
	    targetString[j] = dataList.get(j).getLoginName().toString();
	    }
		return targetString;
     }
	
	private String loginNameMatch;
 
    public String getLoginNameMatch() {
        return loginNameMatch;
    }
 
    public void setLoginNameMatch(String loginNameMatch) {
        this.loginNameMatch = loginNameMatch;
    }
	
private ListModelList<Login> dataList = new ListModelList<>();
	
    @Command
    @NotifyChange("dataList")
    public void addItem(@BindingParam("objectNameMatch") Textbox objectNameMatch) {
    	String loginNameMatch = this.loginNameMatch;
    	try{
    		if(isValidEmailAddress(loginNameMatch)){
		    	if(loginNameMatch!=null && (!loginNameMatch.isEmpty())){
					boolean chkPass = true;
					for (int j = 0; j < dataList.getSize(); j++) {
						if (dataList.get(j).getLoginName().toString().equals(loginNameMatch)){
							WebUtil.showInfoBoxWithCustomMessage("The list already contains " + loginNameMatch);
							chkPass = false;
							break;
						}	
					}
					if (chkPass){
					Login login = new Login(loginNameMatch);
					dataList.add(login);
					objectNameMatch.setValue("");
					}
		    	}
    		}
    		else{
    			WebUtil.showInfoBoxWithCustomMessage("Invalid email address format.");
    		}
		}
    	catch(Exception e){
    		WebUtil.showInfoBoxWithCustomMessage("Invalid email address format.");
    	}
    }
    
    public boolean isValidEmailAddress(String email) {
        String ePattern = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$";
        java.util.regex.Pattern p = java.util.regex.Pattern.compile(ePattern);
        java.util.regex.Matcher m = p.matcher(email);
        return m.matches();
    }

    public ListModelList<Login> getDataList() {
    	return dataList;
    }
    public void setDataList(ListModelList<Login> dataList) {
    	this.dataList = dataList;
    }
    
    public class Login {
    	String loginName;

    	public String getLoginName() {
    		return loginName;
    	}
    	public void setLoginName(String loginName) {
    		this.loginName = loginName;
    	}
    	public Login(String loginName) {
    		super();
    		this.loginName = loginName;
    	}
    }	

    @Command("onItemClick")
    public void onItemClick() {
    	enableRemoveButtonIfSelected();
    }
    
    private void enableRemoveButtonIfSelected() {
    	Set selectedItems = dataList.getSelection();
    	if (selectedItems != null && selectedItems.size() > 0) {
    		// removeBtn.setDisabled(false);
    	} else {
    		//removeBtn.setDisabled(true);
    	}
    }

    @Command("onRemoveBtnClick")
    public void onRemoveBtnClick() {
        Utils.removeAllSelectedItems(dataList);
        enableRemoveButtonIfSelected();
        BindUtils.postNotifyChange(null, null, this, "dataList");
    }   
}
