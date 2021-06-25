package com.idera.sqlcm.ui.prompt;


import java.util.Map;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Window;

import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.eventFilters.EventFiltersGridViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.*;
import com.idera.sqlcm.ui.logsView.ActivityLogsGridViewModel;
import com.idera.sqlcm.ui.logsView.ChangeLogsGridViewModel;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.wizard.AbstractAlertWizardViewModel;
import com.idera.sqlcm.wizard.AbstractDataAlertWizardViewModel;
import com.idera.sqlcm.wizard.AbstractStatusWizardViewModel;

public class PromptHandler extends CommonGridViewModel
implements AbstractAlertWizardViewModel.WizardListener,
AbstractStatusWizardViewModel.WizardListener,
AbstractDataAlertWizardViewModel.WizardListener {
	
	String deleteMessage;
	

   	public String getDeleteMessage() {
		return deleteMessage;
	}

	public void setDeleteMessage(String deleteMessage) {
		this.deleteMessage = deleteMessage;
	}

	@Command("closeDialog")
	public void closeDialog(@BindingParam("comp") Window x, @BindingParam("futureconfirm") Checkbox futureconfirm) {
		if(futureconfirm.isChecked()){
			Sessions.getCurrent().setAttribute("PopUpConnfirm","");
		}
		x.detach();
	}

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        String whatTODelete = (String)Sessions.getCurrent().getAttribute("ruleIdForDelete");
        int temp = whatTODelete.indexOf('|');
		whatTODelete = whatTODelete.substring(0, temp);
        if(whatTODelete.equalsIgnoreCase("alertRules"))
		{deleteMessage = ELFunctions.getLabel(SQLCMI18NStrings.CONFIRM_DELETE_RULE);}
        else if(whatTODelete.equalsIgnoreCase("eventFilters"))
		{deleteMessage = ELFunctions.getLabel(SQLCMI18NStrings.CONFIRM_DELETE_FILTER);}
    	else 
    		deleteMessage = ELFunctions.getLabel(SQLCMI18NStrings.CONFIRM_DELETE_LOG);
    }

		 
	@Command("delete")
	public void delete(@BindingParam("comp") Window x, @BindingParam("futureconfirm") Checkbox futureconfirm) throws Exception {
		String whatTODelete = (String)Sessions.getCurrent().getAttribute("ruleIdForDelete");
		int temp = whatTODelete.indexOf('|');
		String id = whatTODelete.substring(temp+1, whatTODelete.length());
		whatTODelete = whatTODelete.substring(0, temp);
		
		if(whatTODelete.equalsIgnoreCase("alertRules"))
		{
			InstancesAlertsRuleGridViewModel instancesAlertsRuleGridViewModel=new InstancesAlertsRuleGridViewModel();
			instancesAlertsRuleGridViewModel.deleteAlertRules(Long.parseLong((String)id));
		
			Sessions.getCurrent().removeAttribute("ruleIdForDelete");
			if(futureconfirm.isChecked()){
				Sessions.getCurrent().setAttribute("PopUpConnfirm","false");
			}
			x.detach();
			Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instancesAlertsRule"));
		}
		
		else if(whatTODelete.equalsIgnoreCase("eventFilters"))
		{
			EventFiltersGridViewModel eventFilterGridViewModel = new EventFiltersGridViewModel();
			eventFilterGridViewModel.inactivateUser(Long.parseLong((String)id));
			
			Sessions.getCurrent().removeAttribute("ruleIdForDelete");
			if(futureconfirm.isChecked()){
				Sessions.getCurrent().setAttribute("PopUpConnfirm","false");
			}
			x.detach();
			Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("eventFiltersView"));
		}
		
		else if(whatTODelete.equalsIgnoreCase("activityLogs"))
		{
			ActivityLogsGridViewModel activityLogsGridViewModel = new ActivityLogsGridViewModel();
			activityLogsGridViewModel.deleteLogs(Integer.parseInt((String)id));
			
			Sessions.getCurrent().removeAttribute("ruleIdForDelete");
			if(futureconfirm.isChecked()){
				Sessions.getCurrent().setAttribute("PopUpConnfirm","false");
			}
			x.detach();
			Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("activityLogsView"));
		}
		
		else if(whatTODelete.equalsIgnoreCase("changeLogs"))
		{
			ChangeLogsGridViewModel changeLogsGridViewModel = new ChangeLogsGridViewModel();
			changeLogsGridViewModel.deleteLogs(Integer.parseInt((String)id));
			
			Sessions.getCurrent().removeAttribute("ruleIdForDelete");
			if(futureconfirm.isChecked()){
				Sessions.getCurrent().setAttribute("PopUpConnfirm","false");
			}
			x.detach();
			Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("changeLogsView"));
		}	
	}

	@Override
	public void onCancel() {
		// TODO Auto-generated method stub             
		
	}

	@Override
	public void onFinish() {
		// TODO Auto-generated method stub
		
	}

	@Override
	protected CommonGridViewReport makeCommonGridViewReport() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected Map<String, Boolean> collectColumnsVisibilityMap() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected void retrieveColumnsVisibility(
			CMSideBarViewSettings alertsSettings) {
		// TODO Auto-generated method stub
		
	}	  
}
			