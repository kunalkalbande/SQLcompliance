package com.idera.sqlcm.ui.dialogs.eventFilterWizard.steps;

import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.EventFilterSaveEntity;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.EventFilterWizardViewModel;
import com.idera.sqlcm.wizard.FilterStepBase;
import com.idera.sqlcm.wizard.StepBase;

import org.zkoss.zul.Button;

public abstract class AddWizardStepBase extends FilterStepBase<EventFilterWizardViewModel, EventFilterSaveEntity> {

    private CMInstance instance;

    private Button nextButton;

    @Override
    public final void onAfterWire() {
        instance = getParentWizardViewModel().getInstance();
        nextButton = getParentWizardViewModel().getNextButton();
        onDoAfterWire();
    }

    @Override
    public final void onShow(EventFilterSaveEntity wizardSaveEntity) {
        //nextButton.setDisabled(true);
        doOnShow(wizardSaveEntity);
    }

    protected void onDoAfterWire() {
        // do nothing
    }

    protected void doOnShow(EventFilterSaveEntity wizardSaveEntity) {

    }

    protected CMInstance getInstance() {
        return instance;
    }

    protected Button getNextButton() {
        return nextButton;
    }
}
