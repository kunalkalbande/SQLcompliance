package com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;
import com.idera.sqlcm.entities.database.properties.CMAuditedActivity;
import com.idera.sqlcm.entities.database.properties.CMDatabaseProperties;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.converter.PermissionToImagePathConverter;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps.RegulationGuidelinesAuditedActivitiesStepViewModel.AccessCheckOption;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Set;

public class RegulationGuidelinePrivilegedUsersStepViewModel extends RegulationGuidelineAddWizardStepBase implements AddPrivilegedUsersViewModel.DialogListener {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/regulationguidelinewizard/steps/privileged-users-step.zul";

    @Wire
    private Button removeBtn;

    private Converter permissionToImagePathConverter = new PermissionToImagePathConverter();

    private ListModelList<CMInstancePermissionBase> permList = new ListModelList<>();
    private CMAuditedActivities auditedActivities;

    public RegulationGuidelinePrivilegedUsersStepViewModel() {
        super();
    }
    
    private CMAuditedActivities checkWizardPath;

    private CMDatabaseProperties databaseProperties;

    public CMDatabaseProperties getDatabaseProperties() {
	return databaseProperties;
    }

    public CMAuditedActivities getCheckWizardPath() {
		return checkWizardPath;
	}

	public void setCheckWizardPath(CMAuditedActivities checkWizardPath) {
		this.checkWizardPath = checkWizardPath;
    }

    private CMDatabaseProperties getDatabaseProperties(Long databaseId) {
	CMDatabaseProperties databaseProperties = null;
	try {
	    databaseProperties = DatabasesFacade
		    .getDatabaseProperties(databaseId);
	} catch (RestException e) {
	    WebUtil.showErrorBox(e,
		    SQLCMI18NStrings.FAILED_TO_LOAD_DATABASE_PROPERTIES);
	}
	return databaseProperties;
    }
    
    @Override
    protected void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) {
	long dbId = (Long) Sessions.getCurrent().getAttribute("dbId");

	databaseProperties = getDatabaseProperties(dbId);
	CMInstanceUsersAndRoles usersAndRoles = databaseProperties
		.getPrivilegedRolesAndUsers();
	List<CMInstancePermissionBase> selectedPermissionList = usersAndRoles
		.getUsersAndRoles();
	for (CMInstancePermissionBase p : selectedPermissionList) {
	    if (!permList.contains(p)) {
		permList.add(p);
	    }
	}
	CMAuditedActivity cmAud = databaseProperties
		.getAuditedPrivilegedUserActivities();
	CMAuditedActivities userAuditedActivities = new CMAuditedActivities();
	userAuditedActivities.setAuditLogins(cmAud.isAuditLogins());
	userAuditedActivities.setAuditFailedLogins(cmAud.isAuditFailedLogins());
	userAuditedActivities.setAuditSecurity(cmAud.isAuditSecurity());
	userAuditedActivities.setAuditAdmin(cmAud.isAuditAdmin());
	userAuditedActivities.setAuditDDL(cmAud.isAuditDDL());
	userAuditedActivities.setAuditDML(cmAud.isAuditDML());
	userAuditedActivities.setAuditSELECT(cmAud.isAuditSELECT());
	userAuditedActivities.setAuditDefinedEvents(cmAud
		.isAuditDefinedEvents());
	userAuditedActivities.setAuditCaptureSQL(cmAud.isAuditCaptureSQL());
	userAuditedActivities.setAuditCaptureTrans(cmAud.isAuditCaptureTrans());
	userAuditedActivities.setAuditAccessCheck(cmAud.getAuditAccessCheck());
	userAuditedActivities.setAuditUserCaptureDDL(cmAud.isAuditUserCaptureDDL());
	userAuditedActivities.setAuditAllUserActivities(cmAud
		.isAuditAllUserActivities());

	wizardSaveEntity.setUserAuditedActivities(userAuditedActivities);
	BindUtils.postNotifyChange(null, null, this, "permList");
    }
    @Override
    public String getNextStepZul() {
        String nextStepZul;
        if (permList.size() > 0) {
            nextStepZul = RegulationGuidelinePrivilegedUserAuditedActivityStepViewModel.ZUL_PATH;
        } else {
        	checkWizardPath =  getParentWizardViewModel().getWizardEntity().getAuditedActivities();
        	if(checkWizardPath.isAuditSensitiveColumns()){
            nextStepZul = RegulationGuidelineSensitiveColumnsStepViewModel.ZUL_PATH;
        	}
        	else if(checkWizardPath.isAuditBeforeAfter()){
        		nextStepZul = RegulationGuidelineBeforeAfterStepViewModel.ZUL_PATH;
        	}
        	else
        		nextStepZul = RegulationGuidelinePermissionsCheckStepViewModel.ZUL_PATH;
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
    	for (CMInstancePermissionBase p: selectedPermissionList) {
            if (!permList.contains(p)) {
                permList.add(p);
            }
    	}
        //permList.addAll(selectedPermissionList);
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
