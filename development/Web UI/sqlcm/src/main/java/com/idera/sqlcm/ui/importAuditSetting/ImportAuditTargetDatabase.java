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

public class ImportAuditTargetDatabase extends AddWizardStepBase {
	
	public static final String ZUL_PATH = "~./sqlcm/ImportAuditSetting/target-databases.zul";
	
	@Wire
	Listbox targetDB;
	
	@Wire
	Button nextButton;
	
	List<TargetDatabaseDetails> tagetDBDetails = new ArrayList<TargetDatabaseDetails>();
	
	ListModelList<TargetDatabaseDetails> finalTargetDBDetails = new ListModelList<TargetDatabaseDetails>();
	
	public ListModelList<TargetDatabaseDetails> getFinalTargetDBDetails() {
		return finalTargetDBDetails;
	}

	public void setFinalTargetDBDetails(
			ListModelList<TargetDatabaseDetails> finalTargetDBDetails) {
		this.finalTargetDBDetails = finalTargetDBDetails;
	}
	List<ServerDetails> srvDetails = new ArrayList<ServerDetails>();
	List<DatabaseDetails> dbDetails = new ArrayList<DatabaseDetails>();
	
	@Override
    protected void doOnShow(AddWizardImportEntity wizardSaveEntity) {
		nextButton.setDisabled(true);
		tagetDBDetails.clear();
		srvDetails = wizardSaveEntity.getServerDetails();
		dbDetails = wizardSaveEntity.getDatabaseDetails();
		List<DatabaseDetails> dDet = new ArrayList<DatabaseDetails>(wizardSaveEntity.getUserdbSelection());
		DatabaseDetails dName = new DatabaseDetails();
		for(int k = 0 ; k< dDet.size() ; k++)
		{
			if(dDet.get(k)!=null)
				dName = dDet.get(k);
		}
		
		for(int j =0; j< srvDetails.size() ; j++)
		{
			for (int i = 0 ; i< dbDetails.size() ; i++)
			{
				if(srvDetails.get(j).getServerId()==dbDetails.get(i).getSrvId())
				{
					for (ServerDetails srvDet: wizardSaveEntity.getUserServerSelection()){
						if(srvDet.getServerName().equals(srvDetails.get(j).getServerName())){
							TargetDatabaseDetails targetDBDet = new TargetDatabaseDetails();
							targetDBDet.serverName = srvDetails.get(j).getServerName();
							targetDBDet.srvId = srvDetails.get(j).getServerId();
							targetDBDet.dbName = dbDetails.get(i).getDbName();
							targetDBDet.dbId = dbDetails.get(i).getDbId();
							if(wizardSaveEntity.getUsermatchdbNameSelection()==true && !dName.dbName.equals(targetDBDet.dbName))
								continue;
							tagetDBDetails.add(targetDBDet);
							break;
						}
					}
				}
			}
		}
		setFinalTargetDBDetails(new ListModelList<>(tagetDBDetails));
		finalTargetDBDetails.setMultiple(true);
		if(Sessions.getCurrent().getAttribute("targetDbList") != null){
			Set<TargetDatabaseDetails> dbList = (Set<TargetDatabaseDetails>)Sessions.getCurrent().getAttribute("targetDbList");
			ListModelList<TargetDatabaseDetails> targetDBDetails = new ListModelList<TargetDatabaseDetails>();
			for(TargetDatabaseDetails targetDatabaseDetails:dbList){
				for(TargetDatabaseDetails databaseDetails : finalTargetDBDetails){
					if(databaseDetails.dbName.equals(targetDatabaseDetails.dbName)
							&& databaseDetails.serverName.equals(targetDatabaseDetails.serverName))
						targetDBDetails.add(databaseDetails);
				}
			}
			if(targetDBDetails.size()>0){
				finalTargetDBDetails.setSelection(targetDBDetails);
			}
		}		
		targetDB.setModel(finalTargetDBDetails);
		if(finalTargetDBDetails.getSelection().size()>0){
			Sessions.getCurrent().setAttribute("targetDbList",finalTargetDBDetails.getSelection());
			nextButton.setDisabled(false);
		}
		else{
			Sessions.getCurrent().removeAttribute("targetDbList");
		}
	}
	
	@Command("ClearAll")
	public void clearAll()
	{
		Sessions.getCurrent().removeAttribute("targetDbList");
		finalTargetDBDetails.clearSelection();
		nextButton.setDisabled(true);
	}
	
	@Command("SelectAll")
	public void selectAll()
	{ 
		Sessions.getCurrent().setAttribute("targetDbList",finalTargetDBDetails.getSelection());
		finalTargetDBDetails.setSelection(finalTargetDBDetails);
		if(finalTargetDBDetails.size()>0)
			nextButton.setDisabled(false);
	}
	
	@Command("listItemSelect")
	public void listItemSelect()
	{
		Sessions.getCurrent().setAttribute("targetDbList",finalTargetDBDetails.getSelection());
		nextButton.setDisabled(false);
	}
	
	@Override
    public String getNextStepZul() {
        return ImportAuditSummary.ZUL_PATH;
    }
	@Override
	public boolean isValid() {
		// TODO Auto-generated method stub
		return false;
	}
	@Override
	public void onBeforePrev(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		
	}
	@Override
	public void onBeforeNext(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		wizardSaveEntity.setUserdbServerComboSelection(finalTargetDBDetails.getSelection());
	}
}

/***End SQLCm 5.4***/
