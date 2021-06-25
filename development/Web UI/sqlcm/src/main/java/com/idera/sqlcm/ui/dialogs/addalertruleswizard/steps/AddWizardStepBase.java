package com.idera.sqlcm.ui.dialogs.addalertruleswizard.steps;

import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesWizardViewModel;
import com.idera.sqlcm.wizard.AlertStepBase;
import org.zkoss.zul.Button;
import org.zkoss.zul.Window;

public abstract class AddWizardStepBase extends AlertStepBase<AddAlertRulesWizardViewModel, AddAlertRulesSaveEntity> {

    private CMInstance instance;

    private Button nextButton;
    private Button cancelButton;

    @Override
    public final void onAfterWire() {
        instance = getParentWizardViewModel().getInstance();
        nextButton = getParentWizardViewModel().getNextButton();
        cancelButton = getParentWizardViewModel().getCancelButton();
        onDoAfterWire();
    }

    public void doAfterCompose(Window comp) throws Exception {
    }

    public Button getCancelButton() {
		return cancelButton;
	}

	@Override
    public final void onShow(AddAlertRulesSaveEntity wizardSaveEntity) {
        doOnShow(wizardSaveEntity);
    }

    protected void onDoAfterWire() {
        // do nothing
    }

    protected void doOnShow(AddAlertRulesSaveEntity wizardSaveEntity) {

    }

    protected CMInstance getInstance() {
        return instance;
    }

    protected Button getNextButton() {
        return nextButton;
    }
}
