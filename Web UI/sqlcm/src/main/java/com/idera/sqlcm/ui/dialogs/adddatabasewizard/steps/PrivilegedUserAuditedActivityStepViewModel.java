package com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.ListModelList;

import java.util.Arrays;

public class PrivilegedUserAuditedActivityStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/adddatabasewizard/steps/privileged-user-audited-activity-step.zul";

    public enum AuditActivity {
        ALL(true, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_AUDIT_ALL)),
        SELECTED(false, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_AUDIT_SELECTED));

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
        PASSED(0, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_PASSED)),
        FAILED(2, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_FAILED)),
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

    public CMAuditedActivities getAuditedActivities() {
        return auditedActivities;
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
        return SensitiveColumnsStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_PRIVILEGED_USERS_AUDITED_ACTIVITY_TIPS);
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
        BindUtils.postNotifyChange(null, null, this, "accessCheckDisabled");
        BindUtils.postNotifyChange(null, null, this, "auditSelectedActivitiesDisabled");
        BindUtils.postNotifyChange(null, null, this, "captureSqlDisabled");
        BindUtils.postNotifyChange(null, null, this, "captureTransactionDisabled");
    }

    @Command("onCheckedDbModification")
    public void onCheckedDbModification() {
        BindUtils.postNotifyChange(null, null, this, "captureSqlDisabled");
        BindUtils.postNotifyChange(null, null, this, "captureTransactionDisabled");
    }

    @Command("onCheckedDbSelects")
    public void onCheckedDbSelects() {
        BindUtils.postNotifyChange(null, null, this, "captureSqlDisabled");
    }

    @Command("onCheckedFilterEventsAccessCheck")
         public void onCheckedFilterEventsAccessCheck() {
        BindUtils.postNotifyChange(null, null, this, "accessCheckDisabled");
    }

    @Command("onAuditCaptureSQLCheck")
    public void onAuditCaptureSQLCheck(@BindingParam("checked") boolean checked) {
        if (checked) {
            WebUtil.showWarningBox(SQLCMI18NStrings.ADD_DATABASE_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE);
        }
    }

    public boolean isCaptureSqlDisabled() {
        return !(auditedActivities.isAuditDML() || auditedActivities.isAuditSELECT()) || isAuditSelectedActivitiesDisabled();
    }

    public boolean isCaptureTransactionDisabled() {
        return !auditedActivities.isAuditDML() || isAuditSelectedActivitiesDisabled();
    }

    public boolean isFilterEventsAccessChecked() {
        return filterEventsAccessChecked;
    }

    public void setFilterEventsAccessChecked(boolean filterEventsAccessChecked) {
        this.filterEventsAccessChecked = filterEventsAccessChecked;
    }

    @Override
    public void onBeforeNext(AddDatabasesSaveEntity wizardSaveEntity) {
        if (filterEventsAccessChecked) {
            auditedActivities.setAuditAccessCheck(Utils.getSingleSelectedItem(filterAccessCheckListModelList).getId());
        } else {
            auditedActivities.setAuditAccessCheck(FilterAccessCheck.DISABLED.getId());
        }

        auditedActivities.setAuditAllUserActivities(isAuditSelectedActivitiesDisabled());

        wizardSaveEntity.setUserAuditedActivities(auditedActivities);
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
