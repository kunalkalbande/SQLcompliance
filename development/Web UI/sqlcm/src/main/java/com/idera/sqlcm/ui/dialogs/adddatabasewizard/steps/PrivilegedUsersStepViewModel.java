package com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps;

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
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;

import java.util.List;
import java.util.Set;

public class PrivilegedUsersStepViewModel extends AddWizardStepBase implements AddPrivilegedUsersViewModel.DialogListener {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/adddatabasewizard/steps/privileged-users-step.zul";

    @Wire
    private Button removeBtn;

    private Converter permissionToImagePathConverter = new PermissionToImagePathConverter();

    private ListModelList<CMInstancePermissionBase> permList = new ListModelList<>();

    public PrivilegedUsersStepViewModel() {
        super();
    }

    @Override
    public String getNextStepZul() {
        String nextStepZul;
        if (permList.size() > 0) {
            nextStepZul = PrivilegedUserAuditedActivityStepViewModel.ZUL_PATH;
        } else {
            nextStepZul = SensitiveColumnsStepViewModel.ZUL_PATH;
        }
        return nextStepZul;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_PRIVILEGED_USERS_TIPS);
    }

    public Converter getPermissionToImagePathConverter() {
        return permissionToImagePathConverter;
    }

    @Override
    public void onDoAfterWire() {
        permList.setMultiple(true);
        enableRemoveButtonIfSelected();
    }

    @Command("onAddBtnClick")
    public void onAddBtnClick() {
        AddPrivilegedUsersViewModel.showDialog(getInstance().getId(), null, this);
    }

    @Command("onItemClick")
    public void onItemClick() {
        enableRemoveButtonIfSelected();
    }

    private void enableRemoveButtonIfSelected() {
        Set selectedItems = permList.getSelection();
        if (selectedItems != null && selectedItems.size() > 0) {
            removeBtn.setDisabled(false);
        } else {
            removeBtn.setDisabled(true);
        }
    }

    @Command("onRemoveBtnClick")
    public void onRemoveBtnClick() {
        Utils.removeAllSelectedItems(permList);
        enableRemoveButtonIfSelected();
        BindUtils.postNotifyChange(null, null, this, "permList");
    }

    @Override
    public boolean isValid() {
        return true;
    }


    public ListModelList<CMInstancePermissionBase> getPermList() {
        return permList;
    }

    @Override
    public void onOk(long instanceId, List<CMInstancePermissionBase> selectedPermissionList) {
        permList.addAll(selectedPermissionList);
        BindUtils.postNotifyChange(null, null, this, "permList");
    }

    @Override
    public void onCancel(long instanceId) {
        // do nothing
    }

    @Override
    public void onBeforeNext(AddDatabasesSaveEntity wizardSaveEntity) {
        CMInstanceUsersAndRoles usersAndRoles = CMInstanceUsersAndRoles.composeInstance(permList);
        wizardSaveEntity.setPrivilegedRolesAndUsers(usersAndRoles);
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
