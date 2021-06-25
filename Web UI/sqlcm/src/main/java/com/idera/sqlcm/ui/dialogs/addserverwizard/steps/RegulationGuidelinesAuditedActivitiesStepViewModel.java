package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import java.util.Arrays;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;

import antlr.collections.List;

public class RegulationGuidelinesAuditedActivitiesStepViewModel extends AddWizardStepBase {

public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/regulation-guidelines-audited-activities-step.zul";

private CMAuditedActivities auditedActivities = new CMAuditedActivities();
private boolean filterEventsAccessChecked = true;
private ListModelList<AccessCheckOption> accessCheckOptionListModelList;

@Wire 
Checkbox dbPrivUserRegulation;

public CMAuditedActivities getAuditedActivities() {
	if(auditedActivities != null)
		return auditedActivities;
	else
		return auditedActivities = new CMAuditedActivities();
}

public void setAuditedActivities(CMAuditedActivities auditedActivities) {
	this.auditedActivities = auditedActivities;
}

    public RegulationGuidelinesAuditedActivitiesStepViewModel() {
        super();
    }
    
    public enum AccessCheckOption {
        PASSED_ONLY(0, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_ONLY_PASSED)),
        FAILED_ONLY(2, ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_AUDIT_SETTINGS_DATABASE_ONLY_FAILED)),
        DISABLED(1, null);

        private String label;
        private int id;

        private AccessCheckOption(int id, String label) {
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
    @Override
    public String getNextStepZul() {
    	String nextStepZul;
        if (getParentWizardViewModel().isAddDatabasesOnlyMode()) {        	
        		nextStepZul = getNextStepZulForDatabaseMode();
        	}
        else{
        	nextStepZul = getNextStepZulForInstanceMode();
        }
        return nextStepZul;
    }
    
    public String getNextStepZulForInstanceMode(){
    	String nextStepZul;
    	if(auditedActivities.isAuditSensitiveColumns()){
			nextStepZul = SensitiveColumnsStepViewModel.ZUL_PATH;	
	    	}
    	else if(auditedActivities.isAuditBeforeAfter()){
    		nextStepZul = RegulationGuidelineBeforeAfterStepViewModel.ZUL_PATH;
    	}
    	else
    		nextStepZul = PermissionsCheckStepViewModel.ZUL_PATH;
    	return nextStepZul;
    }
    
    public String getNextStepZulForDatabaseMode(){
    	String nextStepZul;
    	if(auditedActivities.isAuditPrivilegedUsers()){
			nextStepZul = PrivilegedUsersStepViewModel.ZUL_PATH;	
	    	}
    	else if(auditedActivities.isAuditSensitiveColumns()){
			nextStepZul = SensitiveColumnsStepViewModel.ZUL_PATH;	
	    	}
    	else if(auditedActivities.isAuditBeforeAfter()){
    		nextStepZul = RegulationGuidelineBeforeAfterStepViewModel.ZUL_PATH;
    	}
    	else
    		nextStepZul = PermissionsCheckStepViewModel.ZUL_PATH;
    	return nextStepZul;
    }
    
    @Override
    public void onDoAfterWire() {
        accessCheckOptionListModelList = new ListModelList<>();
        accessCheckOptionListModelList.addAll(Arrays.asList(AccessCheckOption.PASSED_ONLY, AccessCheckOption.FAILED_ONLY));
        accessCheckOptionListModelList.setSelection(Arrays.asList(AccessCheckOption.PASSED_ONLY));
    }
    
    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) { 
    	if(!getParentWizardViewModel().isAddDatabasesOnlyMode()){
    		dbPrivUserRegulation.setVisible(false);
    		auditedActivities.setAuditPrivilegedUsers(false);
    	}
    	auditedActivities = wizardEntity.getServerConfigEntity().getAuditedActivities(); 
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
    public boolean isValid() {
        if(!auditedActivities.isAuditDDL() && !auditedActivities.isAuditDML() &&
        		!auditedActivities.isAuditSecurity() && !auditedActivities.isAuditAdmin()
        		&& !auditedActivities.isAuditSELECT()){
        	WebUtil.showErrorBoxWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.ERROR_ONE_TYPE_OF_ACTIVITY_MUST_BE_AUDITED), SQLCMI18NStrings.FAILED_TO_UPDATE_DATABASE_PROPERTIES);
        	return false;
        }
        else if ((!auditedActivities.isAuditDML())
				&& auditedActivities.isAuditBeforeAfter()) {
        	WebUtil.showInfoBoxWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.INFORMATION_ON_DML_NOT_SELECTED_AND_BAD_SELECTED_IN_REGULATION));
			return false;
		}
        else{
        	return true;
        }
    }
	
	@Command("onCheck")
    public void onCheck(@BindingParam("target") Checkbox target) {
		if(!target.isChecked() && target.getName().equals("Privileged User")){
			getParentWizardViewModel().getWizardEntity().getServerConfigEntity().setPrivilegedRolesAndUsers(null);
		}
		else if(!target.isChecked() && target.getName().equals("Before-After Data")){
			removeBeforeAfterData();
		}
		else if(!target.isChecked() && target.getName().equals("Sensitive Column")){
			removeSensitiveColumnData();
		}
		auditedActivities.setCustomEnabled(true);
        BindUtils.postNotifyChange(null, null, this, "*");
	}
	
	public void removeBeforeAfterData(){
		ListModelList<CMDatabase> cmDatabaselist = new ListModelList<CMDatabase> (getParentWizardViewModel().getWizardEntity().getServerConfigEntity().getDatabaseList());
		if(cmDatabaselist != null ){
			for(CMDatabase cmDatabase : cmDatabaselist){
				cmDatabase.setBeforeAfterTableData(null);
			}
			getParentWizardViewModel().getWizardEntity().getServerConfigEntity().setDatabaseList(cmDatabaselist);
		}	
	}
	
	public void removeSensitiveColumnData(){
		ListModelList<CMDatabase> cmDatabaselist = new ListModelList<CMDatabase>(getParentWizardViewModel().getWizardEntity().getServerConfigEntity().getDatabaseList());
		if(cmDatabaselist != null ){
			for(CMDatabase cmDatabase : cmDatabaselist){
				cmDatabase.setSensitiveColumnTableData(null);
			}
			getParentWizardViewModel().getWizardEntity().getServerConfigEntity().setDatabaseList(cmDatabaselist);
		}	
	}

    @Command("onCheckedAudit")
    public void onCheckedAudit() {
        notifyCaptureSqlDisabled();
    }
	
	@Command("onCheckedDbDefinition")
    public void onCheckedDbDefinition() {
		auditedActivities.setCustomEnabled(true);
        BindUtils.postNotifyChange(null, null, this, "captureDDLActivitiesDisabled");
    }

    @Command("onCheckedDbModification")
    public void onCheckedDbModification() {
    	auditedActivities.setCustomEnabled(true);
        notifyCaptureSqlDisabled();
        BindUtils.postNotifyChange(null, null, this, "captureTransactionDisabled");
    }

    private void notifyCaptureSqlDisabled() { BindUtils.postNotifyChange(null, null, this, "captureSqlDisabled"); }

    @Command("onCheckedDbSelects")
    public void onCheckedDbSelects() {
    	auditedActivities.setCustomEnabled(true);
        notifyCaptureSqlDisabled();
    }
    
    @Command("onCheckedFilterEventsAccessCheck")
    public void onCheckedFilterEventsAccessCheck() {
    	auditedActivities.setCustomEnabled(true);
        BindUtils.postNotifyChange(null, null, this, "accessCheckDisabled");
    }

    @Command("onAuditCaptureSQLCheck")
    public void onAuditCaptureSQLCheck(@BindingParam("checked") boolean checked) {
        auditedActivities.setCustomEnabled(true);
        if (checked) {
            WebUtil.showWarningBox(SQLCMI18NStrings.ADD_DATABASE_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE);
        }
    }

    public boolean isCaptureSqlDisabled() {
        return !(auditedActivities.isAuditDML() || auditedActivities.isAuditSELECT());
    }

    public boolean isCaptureTransactionDisabled() {
        return !auditedActivities.isAuditDML();
    }
    
    public boolean isCaptureDDLActivitiesDisabled() {
        return !(auditedActivities.isAuditDDL() || auditedActivities.isAuditSecurity());
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
		wizardEntity.getServerConfigEntity().setAuditedActivities(auditedActivities);
	}
	
	@Override
	public boolean onBeforeCancel(AddServerWizardEntity wizardEntity) {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public String getHelpUrl() {
		// TODO Auto-generated method stub
		return null;
	}
}
