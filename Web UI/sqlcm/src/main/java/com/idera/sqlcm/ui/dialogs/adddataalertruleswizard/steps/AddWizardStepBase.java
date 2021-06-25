package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.AddDataAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.AddDataAlertRulesWizardViewModel;
import com.idera.sqlcm.wizard.DataStepBase;
import org.zkoss.zul.Button;

public abstract class AddWizardStepBase extends DataStepBase<AddDataAlertRulesWizardViewModel, AddDataAlertRulesSaveEntity> {

    private CMInstance instance;

    private Button nextButton;

    @Override
    public final void onAfterWire() {
        instance = getParentWizardViewModel().getInstance();
        nextButton = getParentWizardViewModel().getNextButton();
        onDoAfterWire();
    }

    @Override
    public final void onShow(AddDataAlertRulesSaveEntity wizardSaveEntity) {
        //nextButton.setDisabled(true);
        doOnShow(wizardSaveEntity);
    }

    protected void onDoAfterWire() {
        // do nothing
    }

    protected void doOnShow(AddDataAlertRulesSaveEntity wizardSaveEntity) {

    }

    protected CMInstance getInstance() {
        return instance;
    }

    protected Button getNextButton() {
        return nextButton;
    }
}
