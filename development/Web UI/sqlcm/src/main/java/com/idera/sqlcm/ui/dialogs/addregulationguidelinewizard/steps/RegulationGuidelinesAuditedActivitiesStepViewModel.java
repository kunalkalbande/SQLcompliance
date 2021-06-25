package com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps;

import java.util.Arrays;
import java.util.Collections;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.database.properties.CMSensitiveColumnTableData;
import com.idera.sqlcm.entities.database.properties.CMStringCMTableEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;

public class RegulationGuidelinesAuditedActivitiesStepViewModel extends RegulationGuidelineAddWizardStepBase {

public static final String ZUL_PATH = "~./sqlcm/dialogs/regulationguidelinewizard/steps/regulation-guidelines-audited-activities-step.zul";

private CMAuditedActivities auditedActivities = new CMAuditedActivities();
private boolean filterEventsAccessChecked = true;
private ListModelList<AccessCheckOption> accessCheckOptionListModelList;

public CMAuditedActivities getAuditedActivities() {
	if(auditedActivities != null)
		return auditedActivities;
	else
		return auditedActivities = new CMAuditedActivities();
}

public void setAuditedActivities(CMAuditedActivities auditedActivities) {
	this.auditedActivities = auditedActivities;
}
	private CMAuditedActivities checkWizardPath;
	public CMAuditedActivities getCheckWizardPath() {
	  return checkWizardPath;
	}
	public void setCheckWizardPath(CMAuditedActivities checkWizardPath) {
		this.checkWizardPath = checkWizardPath;
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
    	String nextZulPath;
    	checkWizardPath = getParentWizardViewModel().getWizardEntity().getAuditedActivities();
    	if(checkWizardPath.isAuditPrivilegedUsers()){
    		nextZulPath =  RegulationGuidelinePrivilegedUsersStepViewModel.ZUL_PATH;
        }
    	else if(checkWizardPath.isAuditSensitiveColumns()){
    		nextZulPath = RegulationGuidelineSensitiveColumnsStepViewModel.ZUL_PATH;	
    	}
    	else if(checkWizardPath.isAuditBeforeAfter()){
    		nextZulPath = RegulationGuidelineBeforeAfterStepViewModel.ZUL_PATH;
    	}
    	else
    		nextZulPath =  RegulationGuidelinePermissionsCheckStepViewModel.ZUL_PATH;
    	return nextZulPath;
    }
    
    @Override
    protected void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) { 
    	auditedActivities = wizardSaveEntity.getAuditedActivities();
    	accessCheckOptionListModelList = new ListModelList<>();
        accessCheckOptionListModelList.addAll(Arrays.asList(AccessCheckOption.PASSED_ONLY, AccessCheckOption.FAILED_ONLY));
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
    	checkWizardPath = getParentWizardViewModel().getWizardEntity().getAuditedActivities();
        if(!checkWizardPath.isAuditDDL() && !checkWizardPath.isAuditDML() &&
        		!checkWizardPath.isAuditSecurity() && !checkWizardPath.isAuditAdmin()
        		&& !checkWizardPath.isAuditSELECT()){
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
			getParentWizardViewModel().getWizardEntity().setPrivilegedRolesAndUsers(null);
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
		ListModelList<CMDatabase> cmDatabaselist = (ListModelList<CMDatabase>) getParentWizardViewModel().getWizardEntity().getDatabaseList();
		if(cmDatabaselist != null ){
			for(CMDatabase cmDatabase : cmDatabaselist){
		cmDatabase.setBeforeAfterTableData(Collections
			.<CMTable> emptyList());
			}
	    getParentWizardViewModel().getWizardEntity().setDatabaseList(
		    cmDatabaselist);
		}	
	}
	
	public void removeSensitiveColumnData(){
		ListModelList<CMDatabase> cmDatabaselist = (ListModelList<CMDatabase>) getParentWizardViewModel().getWizardEntity().getDatabaseList();
		if(cmDatabaselist != null ){
			for(CMDatabase cmDatabase : cmDatabaselist){
		CMSensitiveColumnTableData tableData = new CMSensitiveColumnTableData();
		tableData.setSensitiveTableColumnDictionary(Collections
			.<CMStringCMTableEntity> emptyList());
		cmDatabase.setSensitiveColumnTableData(tableData);
			}
			getParentWizardViewModel().getWizardEntity().setDatabaseList(cmDatabaselist);
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
	public void onBeforeNext(AddDatabasesSaveEntity wizardSaveEntity) { 		
		if (filterEventsAccessChecked) {
            auditedActivities.setAuditAccessCheck(Utils.getSingleSelectedItem(accessCheckOptionListModelList).getId());
        } else {
            auditedActivities.setAuditAccessCheck(AccessCheckOption.DISABLED.getId());
        }
		wizardSaveEntity.setAuditedActivities(auditedActivities);
	}
	
	@Override
	public boolean onBeforeCancel(AddDatabasesSaveEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public String getHelpUrl() {
		// TODO Auto-generated method stub
		return null;
	}
}
