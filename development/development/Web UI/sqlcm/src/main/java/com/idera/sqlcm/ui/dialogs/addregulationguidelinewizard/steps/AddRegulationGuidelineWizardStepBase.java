package com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps;

import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.addRegulationGuidelineWizard.AddRegulationWizardViewModel;
import com.idera.sqlcm.wizard.StepBase;
import org.zkoss.zul.Button;

public abstract class AddRegulationGuidelineWizardStepBase extends StepBase<AddRegulationWizardViewModel, AddDatabasesSaveEntity> {

    private CMInstance instance;

    private Button nextButton;

    @Override
    public final void onAfterWire() {
        instance = getParentWizardViewModel().getInstance();
        nextButton = getParentWizardViewModel().getNextButton();
        onDoAfterWire();
    }

    @Override
    public final void onShow(AddDatabasesSaveEntity wizardSaveEntity) {
        //nextButton.setDisabled(true);
        doOnShow(wizardSaveEntity);
    }

    protected void onDoAfterWire() {
        // do nothing
    }

    protected void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) {

    }

    protected CMInstance getInstance() {
        return instance;
    }

    protected Button getNextButton() {
        return nextButton;
    }
}
