/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.ui.importAuditSetting;

import java.util.ArrayList;
import java.util.List;
import java.util.Set;

import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;

public class ImportAuditTargetServer extends AddWizardStepBase {
	public static final String ZUL_PATH = "~./sqlcm/ImportAuditSetting/target-servers.zul";
	
	@Wire
	Listbox serverName;
	
	@Wire
	Button nextButton;
	
	boolean toShowImportAuditTargetDatabase;
	List<ServerDetails> srvDetails = new ArrayList<ServerDetails>();
	List<ServerDetails> srvName = new ArrayList<ServerDetails>();
	ListModelList<ServerDetails> serverNames = new ListModelList<ServerDetails>();
	
	public ListModelList<ServerDetails> getServerNames() {
		return serverNames;
	}

	public void setServerNames(ListModelList<ServerDetails> serverNames) {
		this.serverNames = serverNames;
	}

	@Override
    protected void doOnShow(AddWizardImportEntity wizardSaveEntity) {
		toShowImportAuditTargetDatabase = true;
		nextButton.setDisabled(true);
		srvName.clear();
		srvDetails = wizardSaveEntity.getServerDetails();
		for(int i = 0 ; i< srvDetails.size() ; i++)
		{
			ServerDetails serverDetails = new ServerDetails();
			serverDetails.serverName = srvDetails.get(i).getServerName();
			serverDetails.serverId = srvDetails.get(i).getServerId();
			srvName.add(serverDetails);
		}
		setServerNames(new ListModelList<>(srvName));
		serverNames.setMultiple(true);
		if(Sessions.getCurrent().getAttribute("serverNames") != null){
			Set<ServerDetails> serverList = (Set<ServerDetails>)Sessions.getCurrent().getAttribute("serverNames");
			ListModelList<ServerDetails> serverNameList = new ListModelList<ServerDetails>();
			for(ServerDetails targetServerDetails:serverList){
				for(ServerDetails serverDetails : serverNames){
					if(targetServerDetails.serverName.equals(serverDetails.serverName))
						serverNameList.add(serverDetails);
				}
			}
			if(serverNameList.size()>0){
				serverNames.setSelection(serverNameList);
			}
		}
		
		serverName.setModel(serverNames);
		if(serverNames.getSelection().size()>0){
			Sessions.getCurrent().setAttribute("serverNames",serverNames.getSelection());
			nextButton.setDisabled(false);
		}
		else{
			Sessions.getCurrent().removeAttribute("serverNames");
		}
		if(wizardSaveEntity.userCheckDatabase == false && wizardSaveEntity.usercheckDatabasePrivilage == false)
			toShowImportAuditTargetDatabase = false;
	}
	
	@Command("ClearAll")
	public void clearAll()
	{
		serverNames.clearSelection();
		nextButton.setDisabled(true);
		Sessions.getCurrent().removeAttribute("serverNames");
	}
	
	@Command("SelectAll")
	public void selectAll()
	{
		serverNames.setSelection(serverNames);
		nextButton.setDisabled(false);
		Sessions.getCurrent().setAttribute("serverNames",serverNames.getSelection());
	}
	
	@Command("listItemSelect")
	public void listItemSelect()
	{
		nextButton.setDisabled(false);
		Sessions.getCurrent().setAttribute("serverNames",serverNames.getSelection());
	}
	
	@Override
    public String getNextStepZul() {
		if(!toShowImportAuditTargetDatabase)
			return ImportAuditSummary.ZUL_PATH;
		else
			return ImportAuditTargetDatabase.ZUL_PATH;
    }
	@Override
	public boolean isValid() {
		// TODO Auto-generated method stub
		return false;
	}
	@Override
	public void onFinish(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		
	}
	@Override
	public void onCancel() {
		// TODO Auto-generated method stub
		
	}
	/*@Override
	public void onShow(){
		// TODO Auto-generated method stub
		
	}*/
	@Override
	public void onBeforePrev(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub

	}
	@Override
	public void onBeforeNext(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		wizardSaveEntity.setUserServerSelection(serverNames.getSelection());
	}
}

/***End SQLCm 5.4***/