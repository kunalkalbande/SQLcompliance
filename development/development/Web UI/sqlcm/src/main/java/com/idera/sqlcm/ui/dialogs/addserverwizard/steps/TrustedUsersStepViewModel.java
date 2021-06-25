package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.converter.PermissionToImagePathConverter;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;

import java.util.List;
import java.util.Set;

public class TrustedUsersStepViewModel extends AddWizardStepBase implements AddPrivilegedUsersViewModel.DialogListener {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/trusted-users-step.zul";

    @Wire
    private Button removeButton;

    private Converter permissionToImagePathConverter = new PermissionToImagePathConverter();

    private ListModelList<CMInstancePermissionBase> permissionList = new ListModelList<>();

    public TrustedUsersStepViewModel() {
        super();
    }

    public Converter getPermissionToImagePathConverter() {
        return permissionToImagePathConverter;
    }

    @Override
    public void onDoAfterWire() {
        permissionList.setMultiple(true);
        enableRemoveButtonIfSelected();
    }

    @Command("addClick")
    public void addClick() {
        AddPrivilegedUsersViewModel.showDialog(getInstance().getId(), null, this);
    }

    @Command("removeClick")
    public void removeClick() {
        Utils.removeAllSelectedItems(permissionList);
        enableRemoveButtonIfSelected();
        notifyPermissionList();
    }

    @Command("onItemClick")
    public void onItemClick() {
        enableRemoveButtonIfSelected();
    }

    private void enableRemoveButtonIfSelected() {
        Set selectedItems = permissionList.getSelection();
        if (selectedItems != null && selectedItems.size() > 0) {
            removeButton.setDisabled(false);
        } else {
            removeButton.setDisabled(true);
        }
    }

    @Override
    public String getNextStepZul() {
        if (getParentWizardViewModel().isAddDatabasesOnlyMode()) {
            return PermissionsCheckStepViewModel.ZUL_PATH;
        }
        return DefaultPermissionsStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_TRUSTED_USERS_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Trusted+Users+window";
    }

    public ListModelList<CMInstancePermissionBase> getPermissionList() {
        return permissionList;
    }

    @Override
    public void onOk(long instanceId, List<CMInstancePermissionBase> selectedPermissionList) {
        for (CMInstancePermissionBase p: selectedPermissionList) {
            if (!permissionList.contains(p)) {
                permissionList.add(p);
            }
        }
        notifyPermissionList();
    }

    private void notifyPermissionList() {BindUtils.postNotifyChange(null, null, this, "permissionList");}

    @Override
    public void onCancel(long instanceId) {
        // do nothing
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        CMInstanceUsersAndRoles usersAndRoles = CMInstanceUsersAndRoles.composeInstance(permissionList);
        wizardEntity.getServerConfigEntity().setTrustedRolesAndUsers(usersAndRoles);
    }
}
