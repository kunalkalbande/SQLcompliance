/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.ui.importAuditSetting;

import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesSaveEntity;
import com.idera.sqlcm.wizard.ImportStepBase;

import org.zkoss.zul.Button;

public abstract class AddWizardStepBase extends ImportStepBase<AddImportAuditWizardViewModel, AddWizardImportEntity> {

	private AddWizardImportEntity instance;
    private Button nextButton;

    @Override
    public final void onAfterWire() {
    	instance = getParentWizardViewModel().getInstance();
        nextButton = getParentWizardViewModel().getNextButton();
        onDoAfterWire();
    }

    @Override
    public final void onShow(AddWizardImportEntity wizardSaveEntity) {
        //nextButton.setDisabled(true);
        doOnShow(wizardSaveEntity);
    }
    
    protected void doOnShow(AddWizardImportEntity wizardSaveEntity) {

    }
    
    protected void onDoAfterWire() {
        // do nothing
    }
    
    protected AddWizardImportEntity getInstance() {
        return instance;
    }
    
    protected Button getNextButton() {
        return nextButton;
    }

	public void onCancel(long instanceId) {
		// TODO Auto-generated method stub
		
	}
	
	public void onCancel(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		
	}
}

/***End SQLCm 5.4***/