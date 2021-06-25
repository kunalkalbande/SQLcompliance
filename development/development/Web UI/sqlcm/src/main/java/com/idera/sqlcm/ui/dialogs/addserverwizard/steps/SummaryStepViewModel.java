package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.RegulationType;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;
import com.idera.sqlcm.ui.dialogs.regulationDetails.RegulationDetailsViewModel;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.ListModelList;

import java.util.Map;

import static com.idera.sqlcm.ui.dialogs.addserverwizard.steps.AuditCollectionLevelStepViewModel.AuditLevel;

public class SummaryStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/summary-step.zul";

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
    public void doOnShow(AddServerWizardEntity wizardEntity) {
        ServerConfigEntity serverConfigEntity = wizardEntity.getServerConfigEntity();
        auditLevelName = AuditCollectionLevelStepViewModel.AuditLevel.getById(serverConfigEntity.getCollectionLevel()).getLabel();
        serverName = getInstance().getName();
        if (serverConfigEntity.getDatabaseList() != null) {
            selectedDatabaseList = new ListModelList<>(serverConfigEntity.getDatabaseList());
        }

        BindUtils.postNotifyChange(null, null, SummaryStepViewModel.this, "*");
    }

    private AuditLevel getCurrentRegulationLevel() {
        ServerConfigEntity saveEntity = getParentWizardViewModel().getWizardEntity().getServerConfigEntity();
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
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_SUMMARY_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Summary+window";
    }

    @Command("onOpenRegulationDetailClick")
    public void onOpenRegulationDetailClick() {
        Map<String, RegulationType> selectedRegulationTypes
                = getParentWizardViewModel().getWizardEntity().getSelectedRegulationTypes();
        RegulationDetailsViewModel.show(selectedRegulationTypes.keySet());
    }

}