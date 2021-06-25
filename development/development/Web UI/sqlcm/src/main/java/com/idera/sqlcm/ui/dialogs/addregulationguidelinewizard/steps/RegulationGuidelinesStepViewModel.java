package com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps;

import java.io.IOException;
import java.util.Comparator;
import java.util.HashMap;
import java.util.Map;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.util.media.Media;
import org.zkoss.zk.ui.event.UploadEvent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Textbox;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.RegulationType;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.facade.RegulationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.RegulationSettings;
import com.idera.sqlcm.ui.importAuditSetting.AddWizardImportEntity;

public class RegulationGuidelinesStepViewModel extends RegulationGuidelineAddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/regulationguidelinewizard/steps/regulation-guidelines-step.zul";
    public static final String NEW_LINE = "\r\n";

    public static final long PCI_DSS_TYPE = 1;
    public static final long HIPAA_TYPE = 2;
    public static final long DISA_TYPE = 3;
    public static final long NERC_TYPE = 4;
    public static final long CIS_TYPE = 5;
    public static final long SOX_TYPE = 6;
    public static final long FERPA_TYPE = 7;
    public static boolean isUploadInvalid = false;
    public static boolean isCustomCheck = false;
    
    @Wire
    Textbox filetoimport;    
    String xmlData;
    private CMAuditedActivities auditedActivities = new CMAuditedActivities();
    private CMAuditedActivities customAuditedActivities;    
    private ListModelList<RegulationType> regulationTypeListModelList;


    private Map<Long, Long> checkedTypes;
    public AddWizardImportEntity wizardImportEntity; 

    public RegulationGuidelinesStepViewModel() {
        super();
    }

    @Override
    public String getNextStepZul() {
        return RegulationGuidelinesAuditedActivitiesStepViewModel.ZUL_PATH;
    }
    

    @Override
    public boolean isValid() {
    	return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_REG_GUIDE_TIPS);
    }

    @Override
    protected void onDoAfterWire() {
        try {
            regulationTypeListModelList = new ListModelList<>(RegulationFacade.getRegulationTypeList());
            regulationTypeListModelList.remove(4); //Removed CIS for database level.
            regulationTypeListModelList.sort(regulationTypeComparator, true);
            checkedTypes = new HashMap<>(regulationTypeListModelList.size());
            isUploadInvalid = false;
            isCustomCheck = false;
            BindUtils.postNotifyChange(null, null, RegulationGuidelinesStepViewModel.this, "regulationTypeListModelList");
        } catch (RestException e) {
            getParentWizardViewModel().close();
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_REGULATION_TYPES);
        }
    }
   
    private static Comparator<RegulationType> regulationTypeComparator = new Comparator<RegulationType>() {
        @Override
        public int compare(RegulationType o1, RegulationType o2) {
            if (o1 != null && o2 != null) {
            	return Utils.compareStrings(
            			o1.getName(), o2.getName(), true);
            }
            return 0;
        }
    };

    @Override
    protected void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) {
        getNextButton().setDisabled(checkedTypes.size() == 0 && !isCustomCheck);
    }

    public ListModelList<RegulationType> getRegulationTypeListModelList() {
        return regulationTypeListModelList;
    }

    @Command("onCheck")
    public void onCheck(@BindingParam("target") Checkbox target, @BindingParam("index") long index) {
        long type = ((RegulationType)target.getValue()).getType();
        if (target.isChecked()) {
        	checkedTypes.put(type, type);
            getNextButton().setDisabled(isUploadInvalid);
        }
        else {
        	checkedTypes.remove(type);
        	if (isUploadInvalid || (checkedTypes.size() == 0 && !isCustomCheck)) {
        		getNextButton().setDisabled(true);
        	}
        }
    }
    
    @Command("onCheckCustom")
    public void onCheckCustom(@BindingParam("target") Checkbox target,@BindingParam("uploadButton") Button uploadButton,@BindingParam("filetoimport") Textbox filetoimport) {
        if (target.isChecked()) {
        	uploadButton.setDisabled(false);
        	filetoimport.setDisabled(false);
        	isUploadInvalid = true;
        	isCustomCheck = true;
            getNextButton().setDisabled(true);
        } else {
        	isCustomCheck = false;
            isUploadInvalid = false;
        	uploadButton.setDisabled(true);
        	filetoimport.setDisabled(true);
        	wizardImportEntity = new AddWizardImportEntity();
        	wizardImportEntity.setAuditedActivities(null);
        	customAuditedActivities = null;
        	filetoimport.setText(ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_NEW_CUSTOM_TEMPLATE));
        	getNextButton().setDisabled(checkedTypes.size() == 0);
        }
    }

	@Command("uploadFile")
	public void uploadFile(@ContextParam(ContextType.TRIGGER_EVENT) UploadEvent event, @BindingParam("nextButton") Button nextButton)throws IOException {
		Media media = event.getMedia();
		String fileName = media.getName();
		String extension = fileName.substring(fileName.lastIndexOf(".")+1 , fileName.length());
		filetoimport.setText(fileName);
		if(extension.equalsIgnoreCase("xml"))
		{
			xmlData = media.getStringData();			
			InstancesFacade instanceFacade = new InstancesFacade();
			wizardImportEntity = new AddWizardImportEntity();
			wizardImportEntity = instanceFacade.validateAuditSettings(xmlData);
			nextButton.setDisabled(false);
			isUploadInvalid = false;
			if(!wizardImportEntity.isValidFile()){
				isUploadInvalid = true;
				getNextButton().setDisabled(checkedTypes.size() == 0 || isUploadInvalid);
				WebUtil.showErrorBoxWithCustomMessage("The uploaded XML is not in the correct format. Please review the help files for more details.");
            }
			else if(wizardImportEntity.getServerLevelConfig().equals("true"))
			{
				isUploadInvalid = true;
				getNextButton().setDisabled(checkedTypes.size() == 0 || isUploadInvalid);
				WebUtil.showErrorBoxWithCustomMessage("The selected file contains Server Audit Settings.");
			}
		}
		else
		{
			isUploadInvalid = true;
			getNextButton().setDisabled(checkedTypes.size() == 0 || isUploadInvalid);
			WebUtil.showErrorBoxWithCustomMessage("The uploaded XML is not in the correct format. Please review the help files for more details.");
		}	
		if(isUploadInvalid)
			filetoimport.setText(ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_NEW_CUSTOM_TEMPLATE));
			
    }

    @Override
    public void onBeforeNext(AddDatabasesSaveEntity wizardSaveEntity) {
        RegulationSettings rs = new RegulationSettings();

        if (checkedTypes.containsKey(PCI_DSS_TYPE)) {
            rs.setPci(true);
        }

        if (checkedTypes.containsKey(HIPAA_TYPE)) {
            rs.setHipaa(true);
        }

        if (checkedTypes.containsKey(DISA_TYPE)) {
            rs.setDisa(true);
        }
        if (checkedTypes.containsKey(NERC_TYPE)) {
            rs.setNerc(true);
        }
        if (checkedTypes.containsKey(CIS_TYPE)) {
            rs.setCis(true);
        }
        if (checkedTypes.containsKey(SOX_TYPE)) {
            rs.setSox(true);
        }
        if (checkedTypes.containsKey(FERPA_TYPE)) {
            rs.setFerpa(true);
        }
        if(wizardImportEntity != null && wizardImportEntity.getAuditedActivities() != null)
        {
        	customAuditedActivities =  wizardImportEntity.getAuditedActivities();
        } 
        try {  		    		
            auditedActivities =  DatabasesFacade.getRegulationSettingsForDatabase(rs);
            if( customAuditedActivities != null){
            	auditedActivities.setAuditDDL(auditedActivities.isAuditDDL() || customAuditedActivities.isAuditDDL());
            	auditedActivities.setAuditDML(auditedActivities.isAuditDML() || customAuditedActivities.isAuditDML());
            	auditedActivities.setAuditSecurity(auditedActivities.isAuditSecurity() || customAuditedActivities.isAuditSecurity());
            	auditedActivities.setAuditSELECT(auditedActivities.isAuditSELECT() || customAuditedActivities.isAuditSELECT());
            	auditedActivities.setAuditAdmin(auditedActivities.isAuditAdmin() || customAuditedActivities.isAuditAdmin());
            	auditedActivities.setAuditCaptureSQL(auditedActivities.isAuditCaptureSQL() || customAuditedActivities.isAuditCaptureSQL());
            	auditedActivities.setAuditCaptureTrans(auditedActivities.isAuditCaptureTrans() || customAuditedActivities.isAuditCaptureTrans());
            	auditedActivities.setAuditUserCaptureDDL(auditedActivities.isAuditUserCaptureDDL() || customAuditedActivities.isAuditUserCaptureDDL());
            	auditedActivities.setAuditSensitiveColumns(auditedActivities.isAuditSensitiveColumns() || customAuditedActivities.isAuditSensitiveColumns());
            	auditedActivities.setAuditBeforeAfter(auditedActivities.isAuditBeforeAfter() || customAuditedActivities.isAuditBeforeAfter());
            	auditedActivities.setAuditPrivilegedUsers(auditedActivities.isAuditPrivilegedUsers() || customAuditedActivities.isAuditPrivilegedUsers());
            	if(auditedActivities.getAuditAccessCheck() == 1 || customAuditedActivities.getAuditAccessCheck() == 1)
            	{            		
            		auditedActivities.setAuditAccessCheck(1);
            	}
            	else if(auditedActivities.getAuditAccessCheck() == 0 || customAuditedActivities.getAuditAccessCheck() == 0){
            		auditedActivities.setAuditAccessCheck(0);
            	}
            	else{
            		auditedActivities.setAuditAccessCheck(2);
            	}
            	auditedActivities.setCustomEnabled(true);
            }
            wizardSaveEntity.setAuditedActivities(auditedActivities);
            wizardSaveEntity.setRegulationSettings(rs);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERROR_SAVE_DATABASE);
        }
    }

	@Override
	public String getHelpUrl() {
		return null;
	}

	@Override
	public boolean onBeforeCancel(AddDatabasesSaveEntity wizardSaveEntity) {
		return false;
	}


}
