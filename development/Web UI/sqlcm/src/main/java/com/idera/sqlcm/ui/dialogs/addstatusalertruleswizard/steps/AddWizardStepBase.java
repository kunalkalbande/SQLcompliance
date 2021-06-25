package com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps;

import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesWizardViewModel;
import com.idera.sqlcm.wizard.StatusStepBase;

import org.zkoss.zk.ui.Component;
import org.zkoss.zul.Button;

public abstract class AddWizardStepBase extends StatusStepBase<AddStatusAlertRulesWizardViewModel, AddStatusAlertRulesSaveEntity> {

    private CMInstance instance;

    private Button nextButton;

    @Override
    public final void onAfterWire() {
        instance = getParentWizardViewModel().getInstance();
        nextButton = getParentWizardViewModel().getNextButton();
        onDoAfterWire();
    }

    @Override
    public final void onShow(AddStatusAlertRulesSaveEntity wizardSaveEntity) {
        //nextButton.setDisabled(true);
        doOnShow(wizardSaveEntity);
    }

    protected void onDoAfterWire() {
        // do nothing
    }

    protected void doOnShow(AddStatusAlertRulesSaveEntity wizardSaveEntity) {

    }

    protected CMInstance getInstance() {
        return instance;
    }

    protected Button getNextButton() {
        return nextButton;
    }

	public void onCancel(long instanceId) {
		// TODO Auto-generated method stub
		
	}

	public void onCancel(AddAlertRulesSaveEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		
	}
}
