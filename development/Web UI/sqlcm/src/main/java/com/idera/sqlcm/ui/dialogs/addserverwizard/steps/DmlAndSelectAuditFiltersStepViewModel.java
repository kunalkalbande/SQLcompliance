package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMDmlSelectFilters;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.*;

import java.util.Arrays;

public class DmlAndSelectAuditFiltersStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/dml-and-select-audit-filters-step.zul";

    public enum AuditOption {
        AUDIT_ALL_OBJECTS(true, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_ALL_DB_OBJECTS)),
        AUDIT_FOLLOWING_DB_OBJECTS(false, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_FILTERS_AUDIT_FOLLOWING_DB_OBJECTS));

        private String label;
        private boolean value;

        private AuditOption(boolean value, String label) {
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

    public enum AuditUserTable {
        NONE(0),
        ALL(1);

        private int value;

        AuditUserTable(int value) {
            this.value = value;
        }

        public int getValue() {
            return value;
        }
    }

    @Wire
    private Checkbox cbAuditUserTables;

    private ListModelList<AuditOption> auditOptionList;
    private CMDmlSelectFilters dmlSelectFilters;

    public DmlAndSelectAuditFiltersStepViewModel() {
        super();
    }

    public ListModelList<AuditOption> getAuditOptionList() {
        return auditOptionList;
    }

    @Command("onAuditCheck")
    public void onAuditCheck() {
        BindUtils.postNotifyChange(null, null, this, "auditFollowingDbObjectsDisable");
    }

    public boolean isAuditFollowingDbObjectsDisable() {
        AuditOption auditOption = Utils.getSingleSelectedItem(auditOptionList);
        return AuditOption.AUDIT_ALL_OBJECTS.equals(auditOption);
    }

    @Override
    protected void onDoAfterWire() {
        auditOptionList = new ListModelList<>(AuditOption.values().length);
        auditOptionList.addAll(Arrays.asList(AuditOption.values()));
        auditOptionList.setSelection(Arrays.asList(AuditOption.AUDIT_FOLLOWING_DB_OBJECTS));

        dmlSelectFilters = new CMDmlSelectFilters();
        setAuditUserTableChecked();

    }

    private void setAuditUserTableChecked() {
        cbAuditUserTables.setChecked(true);
        dmlSelectFilters.setAuditUserTables(AuditUserTable.ALL.getValue());
    }

    @Override
    public String getNextStepZul() {
        return TrustedUsersStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_FILTERS_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+DML+and+SELECT+Audit+Filters+window";
    }

    public CMDmlSelectFilters getDmlSelectFilters() {
        return dmlSelectFilters;
    }

    @Command("onAuditUserTablesCheck")
    public void onAuditUserTablesCheck(@BindingParam("checked") boolean checked) {
        if (checked) {
            dmlSelectFilters.setAuditUserTables(AuditUserTable.ALL.getValue());
        } else {
            dmlSelectFilters.setAuditUserTables(AuditUserTable.NONE.getValue());
        }
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        dmlSelectFilters.setAuditDmlAll(Utils.getSingleSelectedItem(auditOptionList).getValue());
        wizardEntity.getServerConfigEntity().setDmlSelectFilters(dmlSelectFilters);
    }
}
