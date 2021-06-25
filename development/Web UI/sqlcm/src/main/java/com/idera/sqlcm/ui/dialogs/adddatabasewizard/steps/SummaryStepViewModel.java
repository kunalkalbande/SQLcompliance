package com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps;

import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.dialogs.regulationDetails.RegulationDetailsViewModel;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.ListModelList;

import static com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps.AuditCollectionLevelStepViewModel.AuditLevel;

public class SummaryStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/adddatabasewizard/steps/summary-step.zul";

    private String auditLevelName;

    private String serverName;

    public SummaryStepViewModel() {
        super();
    }

    private ListModelList<CMDatabase> selectedDatabaseList;

    public ListModelList<CMDatabase> getSelectedDatabaseList() {
        return selectedDatabaseList;
    }

    public String getAuditLevelName() {
        return auditLevelName;
    }

    public String getServerName() {
        return serverName;
    }

    public boolean isRegGuideDetailLinkVisibility() {
        AuditLevel auditLevel = getCurrentRegulationLevel();
        return AuditLevel.REGULATION.equals(auditLevel);
    }

    @Override
    public void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) {
        AddDatabasesSaveEntity addDatabasesSaveEntity = getParentWizardViewModel().getWizardEntity();
        auditLevelName = AuditCollectionLevelStepViewModel.AuditLevel.getById(wizardSaveEntity.getCollectionLevel()).getLabel();
        serverName = getInstance().getInstanceName();
        selectedDatabaseList = new ListModelList<>(addDatabasesSaveEntity.getDatabaseList());

        BindUtils.postNotifyChange(null, null, SummaryStepViewModel.this, "*");
    }

    private AuditLevel getCurrentRegulationLevel() {
        AddDatabasesSaveEntity saveEntity = getParentWizardViewModel().getWizardEntity();
        return AuditLevel.getById(saveEntity.getCollectionLevel());
    }

    @Override
    public String getNextStepZul() {
        return null;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_SUMMARY_TIPS);
    }

    @Override
    public void onFinish(AddDatabasesSaveEntity wizardSaveEntity) {
        getParentWizardViewModel().getWizardEntity();
    }

    @Command("onOpenRegulationDetailClick")
    public void onAddBtnClick() {
        System.out.println(" @#  onOpenRegulationDetailClick ");
       // RegulationDetailsViewModel.showView();
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