package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import java.util.Arrays;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.ListModelList;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;

public class PrivilegedUserAuditedActivityStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/privileged-user-audited-activity-step.zul";

    public enum AuditActivity {
        ALL(true, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_AUDIT_ALL)),
        SELECTED(false, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_AUDIT_SELECTED));

        private String label;
        private boolean value;

        private AuditActivity(boolean value, String label) {
            this.value = value;
            this.label = label;
        }

        public String getLabel() {
            return label;
        }

        public String getName() {
            return this.name();
        }

        public boolean getValue() {
            return value;
        }
    }

    private boolean filterEventsAccessChecked = true;

    public enum FilterAccessCheck {
        PASSED(0, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_PASSED)),
        FAILED(2, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_FAILED)),
        DISABLED(1, null);

        private String label;
        private int id;

        private FilterAccessCheck(int id, String label) {
            this.id = id;
            this.label = label;
        }

        public String getLabel() {
            return label;
        }

        public String getName() {
            return this.name();
        }

        public int getId() {
            return id;
        }
    }

    private CMAuditedActivities auditedActivities;

    private ListModelList<AuditActivity> auditActivityListModelList;

    private ListModelList<FilterAccessCheck> filterAccessCheckListModelList;
    private boolean isValidSetting;

    public CMAuditedActivities getAuditedActivities() {
        return auditedActivities;
    }

    private CMAuditedActivities checkWizardPath;
    
    public CMAuditedActivities getCheckWizardPath() {
		return checkWizardPath;
	}

	public void setCheckWizardPath(CMAuditedActivities checkWizardPath) {
		this.checkWizardPath = checkWizardPath;
	}

	public void setAuditedActivities(CMAuditedActivities auditedActivities) {
        this.auditedActivities = auditedActivities;
    }

    @Override
    public void onDoAfterWire() {
        auditedActivities = new CMAuditedActivities();
        auditedActivities.setAuditLogins(true);
        auditedActivities.setAuditFailedLogins(true);
        auditedActivities.setAuditSecurity(true);
        auditedActivities.setAuditAdmin(true);
        auditedActivities.setAuditDDL(true);

        auditActivityListModelList = new ListModelList<>();
        auditActivityListModelList.addAll(Arrays.asList(AuditActivity.values()));
        auditActivityListModelList.setSelection(Arrays.asList(AuditActivity.SELECTED));

        filterAccessCheckListModelList = new ListModelList<>();
        filterAccessCheckListModelList.addAll(Arrays.asList(FilterAccessCheck.PASSED, FilterAccessCheck.FAILED));
        filterAccessCheckListModelList.setSelection(Arrays.asList(FilterAccessCheck.PASSED));
    }

    public PrivilegedUserAuditedActivityStepViewModel() {
        super();
    }

    public ListModelList<AuditActivity> getAuditActivityListModelList() {
        return auditActivityListModelList;
    }

    public ListModelList<FilterAccessCheck> getFilterAccessCheckListModelList() {
        return filterAccessCheckListModelList;
    }

    @Override
    public String getNextStepZul() {
    	if(!isValidSetting){
    		return ZUL_PATH;
    	}
        String nextStepZul;
        if (getParentWizardViewModel().isAddDatabasesOnlyMode()) {
            nextStepZul = getNextStepZulForAddDatabasesOnlyMode();
        } else {
            nextStepZul = getNextStepZulForAddInstanceMode();
        }
        return nextStepZul;
    }

    private boolean isCustomAuditLevel() {
        int currentLevelId = getParentWizardViewModel().getWizardEntity().getServerConfigEntity().getCollectionLevel();
        return AuditCollectionLevelStepViewModel.AuditLevel.CUSTOM.getId() == currentLevelId;
    }

    private boolean isRegulationAuditLevel() {
        int currentLevelId = getParentWizardViewModel().getWizardEntity().getServerConfigEntity().getCollectionLevel();
        return AuditCollectionLevelStepViewModel.AuditLevel.REGULATION.getId() == currentLevelId;
    }

    private String getNextStepZulForAddDatabasesOnlyMode() {
        String nextStepZul;
        checkWizardPath =  getParentWizardViewModel().getWizardEntity().getServerConfigEntity().getAuditedActivities();
        if (isCustomAuditLevel()) {
            nextStepZul = DatabaseAuditSettingsStepViewModel.ZUL_PATH;
        } else {
        	if(checkWizardPath.isAuditSensitiveColumns()){
        		nextStepZul = SensitiveColumnsStepViewModel.ZUL_PATH;
            }
        	else if(checkWizardPath.isAuditBeforeAfter()){
        		nextStepZul = RegulationGuidelineBeforeAfterStepViewModel.ZUL_PATH;
        	}
        	else
        		nextStepZul = PermissionsCheckStepViewModel.ZUL_PATH;
        }
        return nextStepZul;
    }

    private String getNextStepZulForAddInstanceMode() {
        String nextStepZul;
        if (isRegulationAuditLevel()) {
            nextStepZul = RegulationGuidelinesAuditedActivitiesStepViewModel.ZUL_PATH;
        } else if (!getParentWizardViewModel().getWizardEntity().isAuditDatabasesChecked()) {
            nextStepZul = DefaultPermissionsStepViewModel.ZUL_PATH;
        } else {
            nextStepZul = DatabaseAuditSettingsStepViewModel.ZUL_PATH;
        }
        return nextStepZul;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Privileged+Users+Audited+Activity+window";
    }

    public boolean isAuditSelectedActivitiesDisabled() {
        AuditActivity selected = Utils.getSingleSelectedItem(auditActivityListModelList);
        return selected == null || !AuditActivity.SELECTED.equals(selected);
    }

    public boolean isAccessCheckDisabled() {
        return !filterEventsAccessChecked || isAuditSelectedActivitiesDisabled();
    }

    @Command("onCheckedActivity")
    public void onCheckedActivity() {
        notifyAccessCheckDisabled();
        BindUtils.postNotifyChange(null, null, this, "auditSelectedActivitiesDisabled");
        notifyCaptureSqlDisabled();
        notifyCaptureTransactionDisabled();
        notifyCaptureDDLActivitiesDisabled();
    }

    private void notifyCaptureTransactionDisabled() {BindUtils.postNotifyChange(null, null, this, "captureTransactionDisabled");}

    private void notifyCaptureDDLActivitiesDisabled() {BindUtils.postNotifyChange(null, null, this, "captureDDLActivitiesDisabled");}

    @Command("onCheckedDdDefinition")
    public void onCheckedDdDefinition() {
        notifyCaptureDDLActivitiesDisabled();
    }

    @Command("onCheckedDbModification")
    public void onCheckedDbModification() {
        notifyCaptureSqlDisabled();
        notifyCaptureTransactionDisabled();
    }

    private void notifyCaptureSqlDisabled() {BindUtils.postNotifyChange(null, null, this, "captureSqlDisabled");}

    @Command("onCheckedDbSelects")
    public void onCheckedDbSelects() {
        notifyCaptureSqlDisabled();
    }

    @Command("onCheckedFilterEventsAccessCheck")
         public void onCheckedFilterEventsAccessCheck() {
        notifyAccessCheckDisabled();
    }

    private void notifyAccessCheckDisabled() {BindUtils.postNotifyChange(null, null, this, "accessCheckDisabled");}

    @Command("onAuditCaptureSQLCheck")
    public void onAuditCaptureSQLCheck(@BindingParam("checked") boolean checked) {
        if (checked) {
            //WebUtil.showWarningBox(SQLCMI18NStrings.ADD_SERVER_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE);
            WebUtil.showWarningWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE));
        }
    }

    public boolean isCaptureSqlDisabled() {
        return !(auditedActivities.isAuditDML() || auditedActivities.isAuditSELECT()) || isAuditSelectedActivitiesDisabled();
    }

    public boolean isCaptureTransactionDisabled() {
        return !auditedActivities.isAuditDML() || isAuditSelectedActivitiesDisabled();
    }

    public boolean isCaptureDDLActivitiesDisabled() {
        return !(auditedActivities.isAuditDDL() || auditedActivities.isAuditSecurity()) || isAuditSelectedActivitiesDisabled();
    }

    public boolean isFilterEventsAccessChecked() {
        return filterEventsAccessChecked;
    }

    public void setFilterEventsAccessChecked(boolean filterEventsAccessChecked) {
        this.filterEventsAccessChecked = filterEventsAccessChecked;
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
    	if(!(isAuditSelectedActivitiesDisabled() 
    			|| auditedActivities.isAuditAdmin()
    			|| auditedActivities.isAuditDDL()
    			|| auditedActivities.isAuditDefinedEvents()
    			|| auditedActivities.isAuditDML()
    			|| auditedActivities.isAuditFailedLogins()
    			|| auditedActivities.isAuditLogins()
    			|| auditedActivities.isAuditSecurity()
    			|| auditedActivities.isAuditSELECT())){
    		WebUtil.showErrorBoxWithCustomMessage("You must select at least one type of activity to be audited for privileged users.");
    		isValidSetting = false;
    	}
    	else {
    		isValidSetting = true;
    	}
        if (filterEventsAccessChecked) {
            auditedActivities.setAuditAccessCheck(Utils.getSingleSelectedItem(filterAccessCheckListModelList).getId());
        } else {
            auditedActivities.setAuditAccessCheck(FilterAccessCheck.DISABLED.getId());
        }

        auditedActivities.setAuditAllUserActivities(isAuditSelectedActivitiesDisabled());

        wizardEntity.getServerConfigEntity().setUserAuditedActivities(auditedActivities);
    }
}
