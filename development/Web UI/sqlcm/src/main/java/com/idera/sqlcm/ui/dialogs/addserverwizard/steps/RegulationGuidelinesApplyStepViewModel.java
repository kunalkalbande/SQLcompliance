package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;


public class RegulationGuidelinesApplyStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/regulation-guidelines-apply-step.zul";


    public RegulationGuidelinesApplyStepViewModel() {
        super();
    }

    @Override
    public String getNextStepZul() {
    	getNextButton().setDisabled(false);
        return RegulationGuidelinesStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override

    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Apply+Regulation+window";
    }







}
