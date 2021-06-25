package com.idera.sqlcm.ui.dialogs.addserverwizard;

import java.util.HashMap;
import java.util.Map;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.ExecutionArgParam;
import org.zkoss.bind.annotation.Init;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.AddServerInvisibleStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.AgentDeploymentStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.AgentServiceAccountStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.AgentTraceDirectoryStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.AlwaysOnAvailabilityGroupStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.AuditCollectionLevelStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.DatabaseAuditSettingsStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.DatabaseLoadErrorStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.DefaultPermissionsStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.DmlAndSelectAuditFiltersStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.ExistingAuditDataStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.LicenseLimitReachedStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.PermissionsCheckStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.PrivilegedUserAuditedActivityStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.PrivilegedUsersStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.RegulationGuidelineBeforeAfterStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.RegulationGuidelinesApplyStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.RegulationGuidelinesAuditedActivitiesStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.RegulationGuidelinesStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.RegulationServerAuditSettingsStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.SelectDatabasesStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.SensitiveColumnsStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.ServerAuditSettingsStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.SpecifySqlServerStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.SqlServerClusterStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.SummaryStepViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.TrustedUsersStepViewModel;
import com.idera.sqlcm.wizard.AbstractWizardViewModel;
import com.idera.sqlcm.wizard.WizardStepManager;

@AfterCompose(superclass = true)
public class ServerWizardViewModel extends
	AbstractWizardViewModel<AddServerWizardEntity> {

    private static final String ZUL_URL = "~./sqlcm/dialogs/addserverwizard/server-wizard-dialog.zul";

    private static final String INSTANCE_ARG = "instance_arg";
    private static final String WIZARD_MODE_ARG = "mode_arg";

    private CMInstance instance;
    private WizardMode wizardMode;

    public CMInstance getInstance() {
        return instance;
    }

    public void setInstance(CMInstance instance) {
        this.instance = instance;
    }

    public WizardMode getWizardMode() {
        return wizardMode;
    }

    @Override
    public String getTitle() {
	if (WizardMode.ADD_INSTANCE.equals(wizardMode)) {
	    return ELFunctions
		    .getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_TITLE);
	} else if (WizardMode.ADD_DATABASE_ONLY.equals(wizardMode)) {
	    return ELFunctions
		    .getLabel(SQLCMI18NStrings.ADD_SERVER_DATABASES_WIZARD_AUDIT_TITLE);
	} else {
	    return "?Title?";
	}
    }

    @Override
    protected AddServerWizardEntity createSaveEntity() {
        AddServerWizardEntity addServerWizardEntity = new AddServerWizardEntity();
        addServerWizardEntity.setServerConfigEntity(new ServerConfigEntity());
        return addServerWizardEntity;
    }

    @Init(superclass=true)
    public void init(@ExecutionArgParam(INSTANCE_ARG) CMInstance instance,
                     @ExecutionArgParam(WIZARD_MODE_ARG) WizardMode wizardMode) {
        if (wizardMode == null) {
            throw new RuntimeException(WIZARD_MODE_ARG + " is null! ");
        }
        this.wizardMode = wizardMode;

        if (WizardMode.ADD_DATABASE_ONLY.equals(wizardMode) && instance == null) {
            throw new RuntimeException(INSTANCE_ARG + " is null! ");
        }
        this.instance = instance;
    }

    @Override
    public void registerSteps(final WizardStepManager stepManager) {
        stepManager
                .registerStep(AuditCollectionLevelStepViewModel.ZUL_PATH)
                .registerStep(DatabaseAuditSettingsStepViewModel.ZUL_PATH)
                .registerStep(ServerAuditSettingsStepViewModel.ZUL_PATH)
                .registerStep(SpecifySqlServerStepViewModel.ZUL_PATH)
                .registerStep(SqlServerClusterStepViewModel.ZUL_PATH)
                .registerStep(AgentDeploymentStepViewModel.ZUL_PATH)
                .registerStep(AddServerInvisibleStepViewModel.ZUL_PATH)
                .registerStep(AgentTraceDirectoryStepViewModel.ZUL_PATH)
                .registerStep(DatabaseLoadErrorStepViewModel.ZUL_PATH)
                .registerStep(ExistingAuditDataStepViewModel.ZUL_PATH)
                .registerStep(DefaultPermissionsStepViewModel.ZUL_PATH)
                .registerStep(AgentServiceAccountStepViewModel.ZUL_PATH)
                .registerStep(DmlAndSelectAuditFiltersStepViewModel.ZUL_PATH)
                .registerStep(PermissionsCheckStepViewModel.ZUL_PATH)
                .registerStep(PrivilegedUsersStepViewModel.ZUL_PATH)
                .registerStep(PrivilegedUserAuditedActivityStepViewModel.ZUL_PATH)
                .registerStep(RegulationGuidelinesApplyStepViewModel.ZUL_PATH)
                .registerStep(RegulationGuidelinesStepViewModel.ZUL_PATH)
                .registerStep(RegulationServerAuditSettingsStepViewModel.ZUL_PATH)
                .registerStep(SelectDatabasesStepViewModel.ZUL_PATH)
                .registerStep(RegulationGuidelinesAuditedActivitiesStepViewModel.ZUL_PATH)
                .registerStep(SensitiveColumnsStepViewModel.ZUL_PATH)
                .registerStep(RegulationGuidelineBeforeAfterStepViewModel.ZUL_PATH)
                .registerStep(SummaryStepViewModel.ZUL_PATH)
                .registerStep(LicenseLimitReachedStepViewModel.ZUL_PATH)
                .registerStep(AlwaysOnAvailabilityGroupStepViewModel.ZUL_PATH)
                .registerStep(TrustedUsersStepViewModel.ZUL_PATH);
    }

    @Override
    protected void doSave(AddServerWizardEntity wizardEntity) {
        saveServerConfig(wizardEntity);
    }

    private void saveServerConfig(AddServerWizardEntity wizardEntity) {
        ServerConfigEntity serverConfigEntity = wizardEntity.getServerConfigEntity();
        cleanupModelForAuditCollectionLevel(serverConfigEntity);

        try {
            InstancesFacade.addInstanceConfig(serverConfigEntity);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERROR_SAVE_INSTANCE_CONFIG);
        }
    }

    private void cleanupTableListForDatabases(ServerConfigEntity wizardSaveModel) {
        if (wizardSaveModel == null || wizardSaveModel.getDatabaseList() == null) {
            return;
        }
        for (CMDatabase db : wizardSaveModel.getDatabaseList()) {
            db.setSensitiveColumnTableData(null);
        }
    }

    /**
     * This method removes all data from save entity that not needed for current audit collection level.
     * Entity can contain redundant data in case user changed data in more than one wizard flow.
     */
    private void cleanupModelForAuditCollectionLevel(ServerConfigEntity serverConfigEntity) {
        AuditCollectionLevelStepViewModel.AuditLevel auditCollectionLevel
                = AuditCollectionLevelStepViewModel.AuditLevel.getById(serverConfigEntity.getCollectionLevel());
        switch (auditCollectionLevel) {
            case DEFAULT:
                serverConfigEntity.setTrustedRolesAndUsers(null);
                serverConfigEntity.setPrivilegedRolesAndUsers(null);
                serverConfigEntity.setRegulationSettings(null);
                serverConfigEntity.setAvailabilityGroupList(null);
                serverConfigEntity.setUserAuditedActivities(null);
                serverConfigEntity.setAuditedActivities(null);
                serverConfigEntity.setDmlSelectFilters(null);
                serverConfigEntity.setUpdateServerSettings(false);
                serverConfigEntity.setServerSettingsToBeUpdated(null);
                cleanupTableListForDatabases(serverConfigEntity);
                break;
            case CUSTOM:
                serverConfigEntity.setRegulationSettings(null);
                cleanupTableListForDatabases(serverConfigEntity);
                break;
            case REGULATION: 
            	if(isAddDatabasesOnlyMode()){
            		serverConfigEntity.setAuditedServerActivities(null);
            	}
            	else
            		serverConfigEntity.setServerType(true);
            	
            	if(serverConfigEntity.getAuditedActivities() != null && 
            			!(serverConfigEntity.getAuditedActivities().isCustomEnabled())){
            		serverConfigEntity.setAuditedServerActivities(null);
            		serverConfigEntity.setAuditedActivities(null);                    		
            	}
                serverConfigEntity.setTrustedRolesAndUsers(null);
                serverConfigEntity.setDmlSelectFilters(null);
                serverConfigEntity.setUpdateServerSettings(false);
                serverConfigEntity.setServerSettingsToBeUpdated(null);
                break;
            default:
                throw new RuntimeException(" Not able to cleanup model for " + auditCollectionLevel);
        }
    }

    public boolean isAddDatabasesOnlyMode() {
        return WizardMode.ADD_DATABASE_ONLY.equals(wizardMode);
    }

    public boolean isAddInstanceMode() {
        return WizardMode.ADD_INSTANCE.equals(wizardMode);
    }

    public static void showAddDatabasesOnlyWizard(CMInstance instance,
	    WizardListener listener) {
	Map args = new HashMap();
	args.put(INSTANCE_ARG, instance);
	args.put(WIZARD_MODE_ARG, WizardMode.ADD_DATABASE_ONLY);
	args.put(LISTENER_ARG, listener);
	showWizard(ZUL_URL, args);
    }

    public static void showAddInstanceWizard(WizardListener listener) {
        Map args = new HashMap();
        args.put(WIZARD_MODE_ARG, WizardMode.ADD_INSTANCE);
        args.put(LISTENER_ARG, listener);
        showWizard(ZUL_URL, args);
    }

}
