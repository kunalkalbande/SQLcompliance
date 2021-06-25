package com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps;

import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;

public class RegulationGuidelinesApplyStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/adddatabasewizard/steps/regulation-guidelines-apply-step.zul";

    public RegulationGuidelinesApplyStepViewModel() {
        super();
    }

    @Override
    public String getNextStepZul() {
        return PrivilegedUsersStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override




	public String getHelpUrl() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public boolean onBeforeCancel(AddDatabasesSaveEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		return false;
	}

}
