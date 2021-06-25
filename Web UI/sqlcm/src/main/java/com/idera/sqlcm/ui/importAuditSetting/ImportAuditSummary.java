/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.ui.importAuditSetting;

import com.idera.sqlcm.facade.DatabasesFacade;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Radiogroup;

public class ImportAuditSummary extends AddWizardStepBase{
	public static final String ZUL_PATH = "~./sqlcm/ImportAuditSetting/database-summary.zul";
	
	@Wire
	Radiogroup currentAuditSettings;
	
	@Override
    protected void doOnShow(AddWizardImportEntity wizardSaveEntity) {
		
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
	public void onBeforePrev(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onBeforeNext(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		
	}
	
	@Override
	public void onFinish(AddWizardImportEntity wizardSaveEntity)
	{
		wizardSaveEntity.overwriteSelection = currentAuditSettings.getSelectedIndex()==1;
		DatabasesFacade databaseFacade = new DatabasesFacade();
		try {
			databaseFacade.importDatabaseAuditSetting(wizardSaveEntity);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
}

/***End SQLCm 5.4***/