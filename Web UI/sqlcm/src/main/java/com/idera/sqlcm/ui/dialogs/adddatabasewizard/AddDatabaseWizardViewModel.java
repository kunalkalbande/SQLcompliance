package com.idera.sqlcm.ui.dialogs.adddatabasewizard;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.database.properties.CMSensitiveColumnTableData;
import com.idera.sqlcm.entities.database.properties.CMStringCMTableEntity;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps.*;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifySQLServerViewModel;
import com.idera.sqlcm.wizard.AbstractWizardViewModel;
import com.idera.sqlcm.wizard.WizardStepManager;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ExecutionArgParam;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zul.Window;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

@AfterCompose(superclass = true)
public class AddDatabaseWizardViewModel extends AbstractWizardViewModel<AddDatabasesSaveEntity> {

    public static final String ZUL_URL = "~./sqlcm/wizard/wizard-dialog.zul";

    private static final String INSTANCE_ARG = "instance_arg";

    private CMInstance instance;

    public CMInstance getInstance() {
        return instance;
    }

    @Override
    protected AddDatabasesSaveEntity createSaveEntity() {
        return new AddDatabasesSaveEntity();
    }

    @Init(superclass=true)
    public void init(@ExecutionArgParam(INSTANCE_ARG) CMInstance instance) {
        if (instance == null) {
            throw new RuntimeException(INSTANCE_ARG + " is null! ");
        }
        this.instance = instance;
    }

    @Override
    public void registerSteps(final WizardStepManager stepManager) {
        stepManager
                .registerStep(AuditCollectionLevelStepViewModel.ZUL_PATH)
                .registerStep(DatabaseAuditSettingsStepViewModel.ZUL_PATH)
                .registerStep(DmlAndSelectAuditFiltersStepViewModel.ZUL_PATH)
                .registerStep(PermissionsCheckStepViewModel.ZUL_PATH)
                .registerStep(PrivilegedUsersStepViewModel.ZUL_PATH)
                .registerStep(PrivilegedUserAuditedActivityStepViewModel.ZUL_PATH)
                .registerStep(RegulationGuidelinesApplyStepViewModel.ZUL_PATH)
                .registerStep(RegulationGuidelinesStepViewModel.ZUL_PATH)
                .registerStep(SelectDatabasesStepViewModel.ZUL_PATH)
                .registerStep(SensitiveColumnsStepViewModel.ZUL_PATH)
                .registerStep(SummaryStepViewModel.ZUL_PATH)
                .registerStep(TrustedUsersStepViewModel.ZUL_PATH);
    }

    @Override
    protected void doSave(AddDatabasesSaveEntity wizardSaveModel) {
        cleanupModelForAuditCollectionLevel(wizardSaveModel);
        try {
            DatabasesFacade.addDatabases(wizardSaveModel);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERROR_SAVE_DATABASE);
        }
    }

    private void cleanupTableListForDatabases(AddDatabasesSaveEntity wizardSaveModel) {
        for (CMDatabase db : wizardSaveModel.getDatabaseList()) {
            db.setSensitiveColumnTableData(null);
        }
    }

    /**
     * This method removes all data from save entity that not needed for current audit collection level.
     * Entity can contain redundant data in case user changed data in more than one wizard flow.
     */
    private void cleanupModelForAuditCollectionLevel(AddDatabasesSaveEntity wizardSaveModel) {
        AuditCollectionLevelStepViewModel.AuditLevel auditCollectionLevel
                = AuditCollectionLevelStepViewModel.AuditLevel.getById(wizardSaveModel.getCollectionLevel());
        switch (auditCollectionLevel) {
            case DEFAULT:
                wizardSaveModel.setTrustedRolesAndUsers(null);
                wizardSaveModel.setPrivilegedRolesAndUsers(null);
                wizardSaveModel.setRegulationSettings(null);
                wizardSaveModel.setAvailabilityGroupList(null);
                wizardSaveModel.setUserAuditedActivities(null);
                wizardSaveModel.setAuditedActivities(null);
                wizardSaveModel.setDmlSelectFilters(null);
                cleanupTableListForDatabases(wizardSaveModel);
                break;
            case CUSTOM:
                wizardSaveModel.setRegulationSettings(null);
                wizardSaveModel.setPrivilegedRolesAndUsers(null);
                cleanupTableListForDatabases(wizardSaveModel);
                break;
            case REGULATION:
                wizardSaveModel.setAuditedActivities(null);
                wizardSaveModel.setTrustedRolesAndUsers(null);
                wizardSaveModel.setDmlSelectFilters(null);
                break;
            default:
                throw new RuntimeException(" Not able to cleanup model for " + auditCollectionLevel);
        }

    }

    public static void showAddDatabaseWizard(WizardListener listener) {
        Map args = new HashMap();
       // args.put(INSTANCE_ARG, instance);
        args.put(LISTENER_ARG, listener);
        Window window = (Window) Executions.createComponents(AddDatabaseWizardViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }

	@Override
	public String getTitle() {
		// TODO Auto-generated method stub
		return null;
	}
}
