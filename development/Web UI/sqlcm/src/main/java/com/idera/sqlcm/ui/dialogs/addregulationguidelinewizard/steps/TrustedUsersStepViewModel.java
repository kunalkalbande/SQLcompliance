package com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.converter.PermissionToImagePathConverter;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;

import java.util.List;
import java.util.Set;

public class TrustedUsersStepViewModel extends RegulationGuidelineAddWizardStepBase implements AddPrivilegedUsersViewModel.DialogListener {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/regulationguidelinewizard/steps/trusted-users-step.zul";

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
        BindUtils.postNotifyChange(null, null, this, "permissionList");
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
        return RegulationGuidelinePermissionsCheckStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_TRUSTED_USERS_TIPS);
    }

    public ListModelList<CMInstancePermissionBase> getPermissionList() {
        return permissionList;
    }

    @Override
    public void onOk(long instanceId, List<CMInstancePermissionBase> selectedPermissionList) {
        permissionList.addAll(selectedPermissionList);
        BindUtils.postNotifyChange(null, null, this, "permissionList");
    }

    @Override
    public void onCancel(long instanceId) {
        // do nothing
    }

    @Override
    public void onBeforeNext(AddDatabasesSaveEntity wizardSaveEntity) {
        CMInstanceUsersAndRoles usersAndRoles = CMInstanceUsersAndRoles.composeInstance(permissionList);
        wizardSaveEntity.setTrustedRolesAndUsers(usersAndRoles);
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
