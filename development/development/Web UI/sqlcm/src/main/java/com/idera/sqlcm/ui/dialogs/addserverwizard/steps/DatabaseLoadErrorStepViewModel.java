package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;


import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import org.zkoss.bind.BindUtils;

public class DatabaseLoadErrorStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/database-load-error-step.zul";

    @Override
    public String getNextStepZul() {
        return AuditCollectionLevelStepViewModel.ZUL_PATH;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_DATABASE_LOAD_ERROR_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return null;
    }

    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
        getPrevButton().setDisabled(true);
    }

}
