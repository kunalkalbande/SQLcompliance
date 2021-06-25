package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;
import org.zkoss.zul.ListModelList;

import java.util.Arrays;


public class DefaultPermissionsStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/default-permissions-step.zul";

    private ListModelList<DefaultDbPermission> permissionsListModelList;

    public enum DefaultDbPermission {

        READ_EVENTS_AND_SQL_STATEMENTS(2,
                ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_DEFAULT_PERMISSIONS_READ_EVENTS_AND_SQL_STATEMENTS)),
        READ_EVENTS_ONLY(1,
                ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_DEFAULT_PERMISSIONS_READ_EVENTS_ONLY)),
        DENY_READ_ACCESS(0,
                ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_DEFAULT_PERMISSIONS_DENY_READ_ACCESS));

        private int id;
        private String label;

        DefaultDbPermission(int id, String label) {
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

        public static DefaultDbPermission getById(long id) {
            for(DefaultDbPermission e : values()) {
                if(e.id == id) {
                    return e;
                }
            }
            return null;
        }
    }

    public DefaultPermissionsStepViewModel() {
        super();
        permissionsListModelList = new ListModelList();
        permissionsListModelList.addAll(Arrays.asList(DefaultDbPermission.values()));
        permissionsListModelList.setSelection(Arrays.asList(DefaultDbPermission.READ_EVENTS_AND_SQL_STATEMENTS));
    }

    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
        getNextButton().setDisabled(false);
    }

    public ListModelList<DefaultDbPermission> getPermissionsListModelList() {
        return permissionsListModelList;
    }

    @Override
    public String getNextStepZul() {
        return PermissionsCheckStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_DEFAULT_PERMISSIONS_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Default+Permissions+window";
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        DefaultDbPermission defaultDbPermission = Utils.getSingleSelectedItem(permissionsListModelList);
        wizardEntity.getServerConfigEntity().getServerSettingsToBeUpdated().setDatabasePermissions(defaultDbPermission.getId());
    }
}
