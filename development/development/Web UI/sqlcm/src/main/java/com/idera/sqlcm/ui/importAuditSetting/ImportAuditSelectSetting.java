/***Start SQLCm 5.4***/
/*Start - Requirement 4.1.4.1 - Import Audit Settings*/

package com.idera.sqlcm.ui.importAuditSetting;

import java.util.ArrayList;
import java.util.List;

import org.zkoss.bind.BindContext;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.ExecutionArgParam;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Div;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Radiogroup;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesSaveEntity;
import com.idera.sqlcm.ui.instancesAlertsRule.InstancesAlertsRuleGridViewModel;
import com.idera.sqlcm.wizard.IImportStep;
import com.idera.sqlcm.wizard.ImportAbstractWizardViewModel.WizardListener;
import com.lowagie.text.ListItem;
import com.lowagie.text.pdf.ArabicLigaturizer;


public class ImportAuditSelectSetting extends AddWizardStepBase{
	
	@Wire
	Checkbox serverLevelConfig;
	
	@Wire
	Checkbox privUserConfig;
	
	@Wire
	Checkbox database;
	
	@Wire
	Checkbox databasePrivUser;
	
	@Wire
	Checkbox matchDBNames;
	
	@Wire
	Listbox dbName;
	
	@Wire
	ListItem dbItems;
	
	@Wire
	Button nextButton;
	boolean usermatchdbNamesCheck=false;
	ListModelList<DatabaseDetails> dbNames = new ListModelList<DatabaseDetails>();
	List<DatabaseDetails> dbDtls = new ArrayList<DatabaseDetails>();
	
	public ListModelList<DatabaseDetails> getDbNames() {
		return dbNames;
	}

	public void setDbNames(ListModelList<DatabaseDetails> dbNames) {
		this.dbNames = dbNames;
	}
	public static final String ZUL_PATH = "~./sqlcm/ImportAuditSetting/import_audit_target_server.zul";
	
	@Override
    protected void doOnShow(AddWizardImportEntity wizardSaveEntity) {
		dbNames.clear();
		serverLevelConfig.setChecked(true);
		privUserConfig.setChecked(true);
		database.setChecked(true);
		databasePrivUser.setChecked(true);
		serverLevelConfig.setDisabled(false);
		privUserConfig.setDisabled(false);
		database.setDisabled(false);
		databasePrivUser.setDisabled(false);
		
		if(wizardSaveEntity.ServerLevelConfig.equals("false"))
		{
			serverLevelConfig.setDisabled(true);
			serverLevelConfig.setChecked(false);
		}
		
		if(wizardSaveEntity.privUserConfig.equals("false"))
		{
			privUserConfig.setDisabled(true);
			privUserConfig.setChecked(false);
		}
		
		if(wizardSaveEntity.Database.equals("false"))
		{
			database.setDisabled(true);
			database.setChecked(false);
		}
		if(wizardSaveEntity.DatabasePrivUser.equals("false"))
		{
			databasePrivUser.setDisabled(true);
			databasePrivUser.setChecked(false);
		}
		
		if(wizardSaveEntity.MatchDBNames.equals("false"))
		{
			matchDBNames.setDisabled(true);
		}
		if(wizardSaveEntity.dbDetails.size()!=0)
		{
			dbDtls = wizardSaveEntity.getDatabaseDetails();
			for(int i = 0 ; i< wizardSaveEntity.dbDetails.size() ; i++)
			{
				DatabaseDetails dbDet = new DatabaseDetails();
				//dbDet.dbName = dbDtls.get(i).getDbName();
				dbDet.dbName = wizardSaveEntity.dbDetails.get(i);
				for(int j = 0 ;j<dbDtls.size();j++)
				{
					if(dbDtls.get(j).getDbName().equals(dbDet.dbName)){
					//dbDet.dbId = dbDtls.get(j).getDbId();
					//dbDet.srvId = dbDtls.get(j).getSrvId();
					break;
					}
				}
				dbNames.add(dbDet);
			}
			dbName.setDisabled(false);
			dbName.setModel(dbNames);
			ListModelList<DatabaseDetails> firstSelection = new ListModelList<>();
			firstSelection.add(dbNames.get(0));
			dbNames.setSelection(firstSelection);
		}
		BindUtils.postNotifyChange(null, null, ImportAuditSelectSetting.this, "*");
	}
	
	@Command("serverLevelConfigcheck")
	public void serverLevelConfigcheck()
	{
		if(!serverLevelConfig.isChecked() && !privUserConfig.isChecked() && !database.isChecked() && !databasePrivUser.isChecked())
		{
			nextButton.setDisabled(true);
		}
		else if(serverLevelConfig.isChecked())
		{
			nextButton.setDisabled(false);
		}
	}
	
	@Command("privUserConfigcheck")
	public void privUserConfigcheck()
	{
		if(!serverLevelConfig.isChecked() && !privUserConfig.isChecked() && !database.isChecked() && !databasePrivUser.isChecked())
		{
			nextButton.setDisabled(true);
		}
		else if(privUserConfig.isChecked())
		{
			nextButton.setDisabled(false);
		}
	}
	
	@Command("databasecheck")
	public void databaseCheck()
	{
		if(!serverLevelConfig.isChecked() && !privUserConfig.isChecked() && !database.isChecked() && !databasePrivUser.isChecked())
		{
			nextButton.setDisabled(true);
		}
		if(database.isChecked())
		{
			nextButton.setDisabled(false);
			matchDBNames.setDisabled(false);
			dbName.setDisabled(false);
		}
		if(!database.isChecked() && !databasePrivUser.isChecked())
		{
			matchDBNames.setChecked(false);
			matchDBNames.setDisabled(true);
			dbName.setDisabled(true);
		}
		dbName.setModel(dbNames);
	}
	
	@Command("databasePrivUsercheck")
	public void databasePrivUsercheck()
	{
		if(!serverLevelConfig.isChecked() && !privUserConfig.isChecked() && !database.isChecked() && !databasePrivUser.isChecked())
		{
			nextButton.setDisabled(true);
		}
		if(databasePrivUser.isChecked())
		{
			nextButton.setDisabled(false);
			matchDBNames.setDisabled(false);
			dbName.setDisabled(false);
		}
		if(!database.isChecked() && !databasePrivUser.isChecked())
		{
			matchDBNames.setChecked(false);
			matchDBNames.setDisabled(true);
			dbName.setDisabled(true);
		}
		dbName.setModel(dbNames);
	}
	
	@Command("matchdbNamesCheck")
	public void matchdbNamesCheck()
	{
		if(matchDBNames.isChecked())
			usermatchdbNamesCheck=true;
		else
			usermatchdbNamesCheck=false;
	}
	
	@Override
    public String getNextStepZul() {
        return ImportAuditTargetServer.ZUL_PATH;
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
		wizardSaveEntity.setUsermatchdbNameSelection(usermatchdbNamesCheck);
		wizardSaveEntity.setUserdbSelection(dbNames.getSelection());
		
		if(serverLevelConfig.isChecked())
			wizardSaveEntity.setUserCheckServer(true);
		else
			wizardSaveEntity.setUserCheckServer(false);
		if(privUserConfig.isChecked())
			wizardSaveEntity.setUserCheckServerPrivilage(true);
		else
			wizardSaveEntity.setUserCheckServerPrivilage(false);
		if(database.isChecked())
			wizardSaveEntity.setUserCheckDatabase(true);
		else
		{
			wizardSaveEntity.setUserCheckDatabase(false);
		}
		if(databasePrivUser.isChecked())
			wizardSaveEntity.setUsercheckDatabasePrivilage(true);
		else
			wizardSaveEntity.setUsercheckDatabasePrivilage(false);
		if(!database.isChecked() && !databasePrivUser.isChecked())
			wizardSaveEntity.setUserdbSelection(null);
	}
}

/***End SQLCm 5.4***/