package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import java.util.List;
import java.util.Set;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.converter.PermissionToImagePathConverter;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;

public class PrivilegedUsersStepViewModel extends AddWizardStepBase implements AddPrivilegedUsersViewModel.DialogListener {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/privileged-users-step.zul";

    @Wire
    private Button removeBtn;
    
    private CMAuditedActivities checkWizardPath;

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
            if (getParentWizardViewModel().isAddDatabasesOnlyMode()) {
                nextStepZul = getNextStepZulForAddDatabasesOnlyMode();
            } else {
                nextStepZul = getNextStepZulForAddInstanceMode();
            }
        }
        return nextStepZul;
    }

    private boolean isCustomAuditLevel() {
        int currentLevelId = getParentWizardViewModel().getWizardEntity().getServerConfigEntity().getCollectionLevel();
        return AuditCollectionLevelStepViewModel.AuditLevel.CUSTOM.getId() == currentLevelId;
    }

    private boolean isRegulationAuditLevel() {
        int currentLevelId = getParentWizardViewModel().getWizardEntity().getServerConfigEntity().getCollectionLevel();
        return AuditCollectionLevelStepViewModel.AuditLevel.REGULATION.getId() == currentLevelId;
    }

    private String getNextStepZulForAddDatabasesOnlyMode() {
        String nextStepZul;
        if (isCustomAuditLevel()) {
            nextStepZul = DatabaseAuditSettingsStepViewModel.ZUL_PATH;
        } else {
        	if(checkWizardPath.isAuditSensitiveColumns()){
        		nextStepZul = SensitiveColumnsStepViewModel.ZUL_PATH;
            }
        	else if(checkWizardPath.isAuditBeforeAfter()){
        		nextStepZul = RegulationGuidelineBeforeAfterStepViewModel.ZUL_PATH;
        	}
        	else
        		nextStepZul = PermissionsCheckStepViewModel.ZUL_PATH;
        }
        return nextStepZul;
    }

    private String getNextStepZulForAddInstanceMode() {
        String nextStepZul;
        if (isRegulationAuditLevel()) {
            nextStepZul = RegulationGuidelinesAuditedActivitiesStepViewModel.ZUL_PATH;
        } else if (!getParentWizardViewModel().getWizardEntity().isAuditDatabasesChecked()) {
            nextStepZul = DefaultPermissionsStepViewModel.ZUL_PATH;
        } else {
            nextStepZul = DatabaseAuditSettingsStepViewModel.ZUL_PATH;
        }
        return nextStepZul;
    }
    
    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {     	
    	checkWizardPath = wizardEntity.getServerConfigEntity().getAuditedActivities(); 
    }
    
    @Override
    public boolean isLast() {
        return false;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_PRIVILEGED_USERS_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Privileged+Users+window";
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
        notifyPermList();
    }

    private void notifyPermList() {BindUtils.postNotifyChange(null, null, this, "permList");}

    @Override
    public boolean isValid() {
        return true;
    }


    public ListModelList<CMInstancePermissionBase> getPermList() {
        return permList;
    }

    @Override
    public void onOk(long instanceId, List<CMInstancePermissionBase> selectedPermissionList) {
        for (CMInstancePermissionBase p: selectedPermissionList) {
            if (!permList.contains(p)) {
                permList.add(p);
            }
        }
        notifyPermList();
    }

    @Override
    public void onCancel(long instanceId) {
        // do nothing
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        CMInstanceUsersAndRoles usersAndRoles = CMInstanceUsersAndRoles.composeInstance(permList);
        wizardEntity.getServerConfigEntity().setPrivilegedRolesAndUsers(usersAndRoles);
    }
}
