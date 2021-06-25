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

public class DatabaseAuditSettingsStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/adddatabasewizard/steps/database-audit-settings-step.zul";

    public enum AccessCheckOption {
        PASSED_ONLY(0, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_ONLY_PASSED)),
        FAILED_ONLY(2, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_ONLY_FAILED)),
        DISABLED(1, null);

        private String label;
        private int id;

        private AccessCheckOption(int id, String label) {
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

    private boolean filterEventsAccessChecked = true;

    private CMAuditedActivities auditedActivities;

    private ListModelList<AccessCheckOption> accessCheckOptionListModelList;

    public CMAuditedActivities getAuditedActivities() {
        return auditedActivities;
    }

    public void setAuditedActivities(CMAuditedActivities auditedActivities) {
        this.auditedActivities = auditedActivities;
    }

    @Override
    public void onDoAfterWire() {
        auditedActivities = new CMAuditedActivities();
        accessCheckOptionListModelList = new ListModelList<>();
        accessCheckOptionListModelList.addAll(Arrays.asList(AccessCheckOption.PASSED_ONLY, AccessCheckOption.FAILED_ONLY));
        accessCheckOptionListModelList.setSelection(Arrays.asList(AccessCheckOption.PASSED_ONLY));
    }

    public DatabaseAuditSettingsStepViewModel() {
        super();
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

    @Command("onCheckedAudit")
    public void onCheckedAudit() {
        BindUtils.postNotifyChange(null, null, this, "captureSqlDisabled");
    }

    @Override
    public String getNextStepZul() {
        String nextStepZulPath = TrustedUsersStepViewModel.ZUL_PATH;
        if (auditedActivities.isAuditDML() || auditedActivities.isAuditSELECT()) {
            nextStepZulPath = DmlAndSelectAuditFiltersStepViewModel.ZUL_PATH;
        }
        return nextStepZulPath;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {

        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_SETTINGS_TIPS);
    }

    public boolean isCaptureSqlDisabled() {
        return !(auditedActivities.isAuditDML() || auditedActivities.isAuditSELECT());
    }

    public boolean isCaptureTransactionDisabled() {
        return !auditedActivities.isAuditDML();
    }

    public boolean isFilterEventsAccessChecked() {
        return filterEventsAccessChecked;
    }

    public void setFilterEventsAccessChecked(boolean filterEventsAccessChecked) {
        this.filterEventsAccessChecked = filterEventsAccessChecked;
    }

    public ListModelList<AccessCheckOption> getAccessCheckOptionListModelList() {
        return accessCheckOptionListModelList;
    }

    public void setAccessCheckOptionListModelList(ListModelList<AccessCheckOption> accessCheckOptionListModelList) {
        this.accessCheckOptionListModelList = accessCheckOptionListModelList;
    }

    @Override
    public void onBeforeNext(AddDatabasesSaveEntity wizardSaveEntity) {
        if (filterEventsAccessChecked) {
            auditedActivities.setAuditAccessCheck(Utils.getSingleSelectedItem(accessCheckOptionListModelList).getId());
        } else {
            auditedActivities.setAuditAccessCheck(AccessCheckOption.DISABLED.getId());
        }

        wizardSaveEntity.setAuditedActivities(auditedActivities);
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
