package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import java.util.Arrays;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

public class RegulationServerAuditSettingsStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/regulation-server-audit-settings-step.zul";

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
    protected void doOnShow(AddServerWizardEntity wizardEntity) { 
    	auditedActivities = wizardEntity.getServerConfigEntity().getAuditedServerActivities();
    	if(auditedActivities.getAuditAccessCheck() == 0){
        	accessCheckOptionListModelList.setSelection(Arrays.asList(AccessCheckOption.PASSED_ONLY));
        }
        else if(auditedActivities.getAuditAccessCheck() == 2){
        	accessCheckOptionListModelList.setSelection(Arrays.asList(AccessCheckOption.FAILED_ONLY));
        }
        else
        	accessCheckOptionListModelList.setSelection(Arrays.asList(AccessCheckOption.DISABLED));
    	BindUtils.postNotifyChange(null, null, this, "*");
    }

    @Override
    public void onDoAfterWire() {
        auditedActivities = new CMAuditedActivities();        
        accessCheckOptionListModelList = new ListModelList<>();
        accessCheckOptionListModelList.addAll(Arrays.asList(AccessCheckOption.PASSED_ONLY, AccessCheckOption.FAILED_ONLY));
    }

    public RegulationServerAuditSettingsStepViewModel() {
        super();
    }

    @Command("onCheckedFilterEventsAccessCheck")
    public void onCheckedFilterEventsAccessCheck() {
    	auditedActivities.setCustomEnabled(true);
        BindUtils.postNotifyChange(null, null, this, "accessCheckDisabled");
    }

    @Command("onCheck")
    public void onCheck(@BindingParam("target") Checkbox target) {
    	if(!target.isChecked() && target.getName().equals("Privileged User")){
			getParentWizardViewModel().getWizardEntity().getServerConfigEntity().setPrivilegedRolesAndUsers(null);
		}
		auditedActivities.setCustomEnabled(true);
        BindUtils.postNotifyChange(null, null, this, "*");
	}
    
    @Override
    public String getNextStepZul() {
    	if(auditedActivities.isAuditPrivilegedUsers()){
    		return PrivilegedUsersStepViewModel.ZUL_PATH;
    	}
    	else{
    		return RegulationGuidelinesAuditedActivitiesStepViewModel.ZUL_PATH;
    	}        
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
        if(auditedActivities != null && auditedActivities.isCustomEnabled()){
        	wizardEntity.getServerConfigEntity().getAuditedActivities().setCustomEnabled(true);      
        }
        wizardEntity.getServerConfigEntity().setAuditedServerActivities(auditedActivities);;
    }
}
