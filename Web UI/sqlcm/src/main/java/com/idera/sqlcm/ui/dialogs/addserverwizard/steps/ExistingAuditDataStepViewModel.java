package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;


import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.addserverwizard.ServerRegisteredStatusInfo;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;
import org.zkoss.bind.BindUtils;
import org.zkoss.zul.ListModelList;

import java.util.Arrays;

public class ExistingAuditDataStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/existing-audit-data-step.zul";

    private String databaseName;

    private ListModelList<AuditDataAction> dataActionListModelList;

    public ListModelList<AuditDataAction> getDataActionListModelList() {
        return dataActionListModelList;
    }

    public enum AuditDataAction {

        KEEP_PREVIOUS(0,
                ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_EXISTING_AUDIT_DATA_KEEP_PREVIOUS)),
        DELETE_PREVIOUS(1,
                ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_EXISTING_AUDIT_DATA_DELETE_PREVIOUS));

        private int id;
        private String label;

        AuditDataAction(int id, String label) {
            this.id = id;
            this.label = label;
        }

        public int getId() {
            return id;
        }

        public String getLabel() {
            return label;
        }

        public String getName() {
            return this.name();
        }

        public static AuditDataAction getById(long id) {
            for(AuditDataAction e : values()) {
                if(e.id == id) {
                    return e;
                }
            }
            return null;
        }
    }

    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
        ServerRegisteredStatusInfo serverRegisteredStatusInfo = wizardEntity.getServerRegisteredStatusInfo();

        if (serverRegisteredStatusInfo == null) {
            throw new RuntimeException(" ServerRegisteredStatusInfo must not be null! ");
        }

        databaseName = serverRegisteredStatusInfo.getEventDatabaseName();
        BindUtils.postNotifyChange(null, null, this, "databaseName");
    }

    public ExistingAuditDataStepViewModel() {
        super();
        dataActionListModelList = new ListModelList();
        dataActionListModelList.add(AuditDataAction.KEEP_PREVIOUS);
        dataActionListModelList.add(AuditDataAction.DELETE_PREVIOUS);
        dataActionListModelList.setSelection(Arrays.asList(AuditDataAction.KEEP_PREVIOUS));
    }

    public String getDatabaseName() {
        return databaseName;
    }

    @Override
    public String getNextStepZul() {
        return SqlServerClusterStepViewModel.ZUL_PATH;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_EXISTING_AUDIT_DATA_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Existing+Audit+Data+window";
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        wizardEntity.getAddServerEntity()
                .setExistingAuditData(Utils.getSingleSelectedItem(dataActionListModelList).getId());
    }

}
