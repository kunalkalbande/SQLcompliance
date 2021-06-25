package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;


import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.facade.LicenseFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addserverwizard.WizardMode;
import com.idera.sqlcm.wizard.AbstractWizardViewModel;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.Events;

public class LicenseLimitReachedStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/license-limit-reached-step.zul";

    private boolean canAddOneMoreInstance;

    @Override
    public String getNextStepZul() {
        return SpecifySqlServerStepViewModel.ZUL_PATH;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_LICENSE_LIMIT_REACHED_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return null;
    }

    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
        try {
            canAddOneMoreInstance = LicenseFacade.canAddOneMoreInstance();
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ADD_SERVER_WIZARD_LICENSE_LIMIT_REACHED_CALL_ERROR);
        }

        if (canAddOneMoreInstance) {
            getParentWizardViewModel().goNext();
        } else {
            getNextButton().setVisible(false);
            AbstractWizardViewModel wizard = getParentWizardViewModel();
            wizard.getSaveButton().setVisible(true);
            wizard.getCancelButton().setDisabled(true);
            wizard.getSaveButton().addEventListener(Events.ON_CLICK, new EventListener<Event>() {
                @Override
                public void onEvent(Event event) throws Exception {
                    getParentWizardViewModel().close();
                }
            });
        }

    }

    @Override
    public boolean isValid() {
        /**
         * If false onBeforeNext and onFinish and onSave will not be called after click on finish button
         */
        return canAddOneMoreInstance;
    }

    @Override
    public boolean isFirst() {
        return WizardMode.ADD_INSTANCE.equals(getParentWizardViewModel().getWizardMode());
    }
}
