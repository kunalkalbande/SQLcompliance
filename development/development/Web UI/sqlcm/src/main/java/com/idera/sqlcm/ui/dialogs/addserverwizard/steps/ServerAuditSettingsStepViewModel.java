package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.addserverwizard.ServerSettingsToBeUpdated;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.ListModelList;

import java.util.Arrays;

public class ServerAuditSettingsStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/server-audit-settings-step.zul";

    public enum AccessCheckOption {
        PASSED_ONLY(0, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_SETTINGS_DATABASE_ONLY_PASSED)),
        FAILED_ONLY(2, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_SETTINGS_DATABASE_ONLY_FAILED)),
        DISABLED(1, null);

        private String label;
        private int id;

        AccessCheckOption(int id, String label) {
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
        auditedActivities.setAuditFailedLogins(true);
        auditedActivities.setAuditSecurity(true);
        auditedActivities.setAuditDDL(true);
        accessCheckOptionListModelList = new ListModelList<>();
        accessCheckOptionListModelList.addAll(Arrays.asList(AccessCheckOption.PASSED_ONLY, AccessCheckOption.FAILED_ONLY));
        accessCheckOptionListModelList.setSelection(Arrays.asList(AccessCheckOption.PASSED_ONLY));
    }

    public ServerAuditSettingsStepViewModel() {
        super();
    }

    @Command("onCheckedFilterEventsAccessCheck")
    public void onCheckedFilterEventsAccessCheck() {
        BindUtils.postNotifyChange(null, null, this, "accessCheckDisabled");
    }

    @Override
    public String getNextStepZul() {
        String nextStepZulPath = PrivilegedUsersStepViewModel.ZUL_PATH;
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
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_SERVER_AUDIT_SETTINGS_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Server+Audit+Settings+window";
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
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        if (filterEventsAccessChecked) {
            auditedActivities.setAuditAccessCheck(Utils.getSingleSelectedItem(accessCheckOptionListModelList).getId());
        } else {
            auditedActivities.setAuditAccessCheck(AccessCheckOption.DISABLED.getId());
        }

        ServerSettingsToBeUpdated serverSettingsToBeUpdated = new ServerSettingsToBeUpdated();
        serverSettingsToBeUpdated.setServerId(getInstance().getId());
        serverSettingsToBeUpdated.setServerAuditedActivities(auditedActivities);

        wizardEntity.getServerConfigEntity().setUpdateServerSettings(true);
        wizardEntity.getServerConfigEntity().setServerSettingsToBeUpdated(serverSettingsToBeUpdated);
    }
}
