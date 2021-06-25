package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import com.idera.cwf.ui.dialogs.CustomMessageBox;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.addserverwizard.AddInstanceResult;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.RemoveInstanceDialog;
import com.idera.sqlcm.ui.dialogs.addserverwizard.ServerWizardViewModel;
import com.idera.sqlcm.wizard.AbstractStep;
import org.zkoss.zul.Button;

public abstract class AddWizardStepBase extends AbstractStep<ServerWizardViewModel, AddServerWizardEntity> {

    private Button nextButton;
    private Button prevButton;

    @Override
    public final void onAfterWire() {
        nextButton = getParentWizardViewModel().getNextButton();
        prevButton = getParentWizardViewModel().getPrevButton();
        onDoAfterWire();
    }

    @Override
    public final void onShow(AddServerWizardEntity wizardEntity) {
        //nextButton.setDisabled(true);
        doOnShow(wizardEntity);
    }

    protected void onDoAfterWire() {
        // do nothing
    }

    @Override
    public boolean isValid() {
        return true;
    }

    protected void doOnShow(AddServerWizardEntity wizardEntity) {

    }

    protected CMInstance getInstance() {
        return getParentWizardViewModel().getInstance();
    }

    protected void setInstance(CMInstance instance) {
        getParentWizardViewModel().setInstance(instance);
    }

    protected Button getNextButton() {
        return nextButton;
    }

    protected Button getPrevButton() {
        return prevButton;
    }

    @Override
    public boolean onBeforeCancel(AddServerWizardEntity wizardSaveEntity) {
        AddInstanceResult addInstanceResult = wizardSaveEntity.getAddInstanceResult();
        if (addInstanceResult == null) {
            return false;
        }
        CustomMessageBox.UserResponse userResponse = WebUtil.showConfirmationBoxWithCancel(SQLCMI18NStrings.MESSAGE_DO_YOU_WISH_DELETE_SERVER,
                SQLCMI18NStrings.TITLE_SQL_SERVER_IS_ADDED);
        if (addInstanceResult.getWasSuccessfullyAdded() && userResponse == CustomMessageBox.UserResponse.YES) {
            RemoveInstanceDialog.removeInstance(getInstance());
            return false;
        } else if (addInstanceResult.getWasSuccessfullyAdded() && userResponse == CustomMessageBox.UserResponse.NO) {
            return false;
        } else {
            return true;
        }
    }

    @Override
    public final void onCancel(AddServerWizardEntity wizardSaveEntity) {

    }
}
