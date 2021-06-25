package com.idera.sqlcm.ui.dialogs;
import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class SaveViewViewModel extends CommonGridViewModel implements AddPrivilegedUsersViewModel.DialogListener {
		
	public static final String ZUL_URL_specifySQLServers = "~./sqlcm/dialogs/saveView.zul";
	private String help;
	
	@Wire 
	private Button OKButton;
	
	@Wire
	private Textbox viewNameTxt;
	
	@Wire 
	private Window saveViewDialogue;
	
	public String getHelp() {
		return this.help;
	}
		
	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@Override
	public void onOk(long instanceId,
			List<CMInstancePermissionBase> selectedPermissionList) {
		
	}

	@Override
	public void onCancel(long instanceId) {
		
	}
			
	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
	    Selectors.wireComponents(view, this, false);
	}
      
     @Command("submitChoice")
     public void submitChoice()
     {
    	if(viewNameTxt.getValue().length()>0){
    	String ViewName = viewNameTxt.getValue();
    	Sessions.getCurrent().setAttribute("ViewName", ViewName);
    	SaveViewName();
    	saveViewDialogue.detach();
    	String uri = (String) Sessions.getCurrent().getAttribute("View");
        uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
        Executions.sendRedirect(uri);
    	}
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
