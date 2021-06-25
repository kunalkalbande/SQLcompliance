package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import org.zkoss.bind.BindUtils;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;

import java.util.Arrays;


public class AuditCollectionLevelStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/audit-collection-level-step.zul";

    private ListModelList<AuditLevel> auditCollectionLevelListModelList = new ListModelList();

    public enum AuditLevel {

        DEFAULT(0, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_LABEL),
                ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_DEFAULT_DESC),
                PermissionsCheckStepViewModel.ZUL_PATH),
        CUSTOM(1, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_CUSTOM_LABEL),
               ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_CUSTOM_DESC),
                PrivilegedUsersStepViewModel.ZUL_PATH),
        REGULATION(2, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_REGULATION_LABEL),
                   ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_COLLECTION_RADIO_REGULATION_DESC),
                   RegulationGuidelinesApplyStepViewModel.ZUL_PATH);

        private int id;
        private String label;
        private String description;
        private String nextStepZul;

        private boolean isVisible = true;

        AuditLevel(int id, String label, String desc, String nextStepZul) {
            this.id = id;
            this.label = label;
            this.description = desc;
            this.nextStepZul = nextStepZul;
        }

        public int getId() {
            return id;
        }

        public String getLabel() {
            return label;
        }

        public String getLabelAndDescription() {
            return label + description;
        }

        public String getNextStepZul() {
            return nextStepZul;
        }

        public String getName() {
            return this.name();
        }

        public boolean isVisible() {
            return isVisible;
        }

        public void setVisible(boolean isVisible) {
            this.isVisible = isVisible;
        }

        public static AuditLevel getById(long id) {
            for(AuditLevel e : values()) {
                if(e.id == id) {
                    return e;
                }
            }
            return null;
        }
    }

    public AuditCollectionLevelStepViewModel() {
        super();
        auditCollectionLevelListModelList.addAll(Arrays.asList(AuditLevel.values()));
        auditCollectionLevelListModelList.setSelection(Arrays.asList(AuditLevel.DEFAULT));
    }

    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
        if (wizardEntity.isAuditDatabasesChecked()) {
            AuditLevel.REGULATION.setVisible(true);
        } else {
            AuditLevel.REGULATION.setVisible(false);
        }
        BindUtils.postNotifyChange(null, null, this, "auditCollectionLevelListModelList");
        getNextButton().setDisabled(false);
    }

    public ListModelList<AuditLevel> getAuditCollectionLevelListModelList() {
        return auditCollectionLevelListModelList;
    }

    @Override
    public String getNextStepZul() {
        String nextStepZul;
        AuditLevel radioValue = getSelectedAuditLevel();
        if (getParentWizardViewModel().isAddInstanceMode() && AuditLevel.CUSTOM.equals(radioValue)) {
            nextStepZul = ServerAuditSettingsStepViewModel.ZUL_PATH;
        } else {
            nextStepZul = radioValue.getNextStepZul();
        }
        return nextStepZul;
    }

    private AuditLevel getSelectedAuditLevel() {
        AuditLevel radioValue = Utils.getSingleSelectedItem(auditCollectionLevelListModelList);
        if (radioValue == null) {
            throw new RuntimeException(" Not selected next step ");
        }
        return radioValue;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AUDIT_COLLECTION_LEVEL_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Audit+Collection+Level+window";
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        wizardEntity.getServerConfigEntity().setCollectionLevel(getSelectedAuditLevel().getId());
    }
}
