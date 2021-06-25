package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

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
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.UploadEvent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Textbox;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.RegulationSettings;
import com.idera.sqlcm.entities.RegulationType;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.addserverwizard.RegulationCustomDetail;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.facade.RegulationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.importAuditSetting.AddWizardImportEntity;

public class RegulationGuidelinesStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/regulation-guidelines-step.zul";
    public static final String NEW_LINE = "\r\n";
    public static final String PCI_DSS_NAME = "Payment Card Data Security Standard (PCI DSS)";
    public static final String HIPAA_NAME = "Health Insurance Portability and Accountability Act (HIPAA)";
    public static final String DISA_NAME = "Defense Information Security Agency (DISA STIG)";
    public static final String NERC_NAME = "North American Electric Reliability Corporation (NERC)";
    public static final String CIS_NAME = "Center for Internet Security (CIS)";
    public static final String SOX_NAME = "Sarbanes-Oxley Act (SOX)";
    public static final String FERPA_NAME = "Family Educational Rights and Privacy Act (FERPA)";

    private ListModelList<RegulationType> regulationTypeListModelList;

    public static boolean isUploadInvalid = false;
    public static boolean isCustomCheck = false;

    private Map<String, RegulationType> selectedRegulationTypes;
    
    // Added for Custom Regulation Guideline Support  -- Start
    @Wire
    Textbox filetoimport;    
    String xmlData;
    
    public AddWizardImportEntity wizardImportEntity;
    private RegulationCustomDetail customAuditedActivities;
    private CMAuditedActivities auditedActivities = new CMAuditedActivities();
    private CMAuditedActivities auditedServerActivities = new CMAuditedActivities();
    private RegulationCustomDetail regulationCustomDetail;
    
    // Added for Custom Regulation Guideline Support  -- End    
    public RegulationGuidelinesStepViewModel() {
        super();
    }

    @Override
    public String getNextStepZul() {
    	if(getParentWizardViewModel().isAddDatabasesOnlyMode()){
    		return RegulationGuidelinesAuditedActivitiesStepViewModel.ZUL_PATH;
    	}
    	else
    		return RegulationServerAuditSettingsStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_REG_GUIDE_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Enforce+Regulation+Guidelines+window";
    }

    @Override
    protected void onDoAfterWire() {
        try {
            regulationTypeListModelList = new ListModelList<>(RegulationFacade.getRegulationTypeList());
            // Removed CIS for database level.
            if(getParentWizardViewModel().isAddDatabasesOnlyMode()){
            	regulationTypeListModelList.remove(4);
            }
            regulationTypeListModelList.sort(regulationTypeComparator, true);
            isUploadInvalid = false;
            isCustomCheck = false;
            selectedRegulationTypes = new HashMap<>(regulationTypeListModelList.size());
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
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
        getNextButton().setDisabled(selectedRegulationTypes.size() == 0 && !isCustomCheck);
        if(wizardEntity.getServerConfigEntity() != null && (wizardEntity).getServerConfigEntity().getAuditedActivities() !=null)
        {
        wizardEntity.getServerConfigEntity().getAuditedActivities().setCustomEnabled(false);
        }
        
    }

    public ListModelList<RegulationType> getRegulationTypeListModelList() {
        return regulationTypeListModelList;
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
        	wizardImportEntity.setServerAuditedActivities(null);
        	customAuditedActivities =  new RegulationCustomDetail();
        	customAuditedActivities.setAuditedDatabaseActivities(null);
        	customAuditedActivities.setAuditedServerActivities(null);
        	filetoimport.setText(ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_NEW_CUSTOM_TEMPLATE));
        	getNextButton().setDisabled(selectedRegulationTypes.size() == 0);
        }
    }

    @Command("onCheck")
    public void onCheck(@BindingParam("target") Checkbox target, @BindingParam("index") long index) {
        RegulationType regulationType = target.getValue();
        if (target.isChecked()) {
            selectedRegulationTypes.put(regulationType.getName(), regulationType);
            getNextButton().setDisabled(isUploadInvalid);
        } else {
            selectedRegulationTypes.remove(regulationType.getName());
            if (isUploadInvalid || (selectedRegulationTypes.size() == 0 && !isCustomCheck))
    	    {
    	    	getNextButton().setDisabled(true);
    	    }
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
				getNextButton().setDisabled(selectedRegulationTypes.size() == 0 || isUploadInvalid);
				WebUtil.showErrorBoxWithCustomMessage("The uploaded XML is not in the correct format. Please review the help files for more details.");
            }
			else if(getParentWizardViewModel().isAddDatabasesOnlyMode() && wizardImportEntity.getServerLevelConfig().equals("true"))
			{
				isUploadInvalid = true;
				getNextButton().setDisabled(selectedRegulationTypes.size() == 0 || isUploadInvalid);
				WebUtil.showErrorBoxWithCustomMessage("The selected file contains Server Audit Settings.");
			}
			else if(!getParentWizardViewModel().isAddDatabasesOnlyMode() && wizardImportEntity.getServerLevelConfig().equals("false"))
			{
				isUploadInvalid = true;
				getNextButton().setDisabled(selectedRegulationTypes.size() == 0 || isUploadInvalid);
				WebUtil.showErrorBoxWithCustomMessage("The selected file does not contain Server Audit Settings.");
			}
		}
		else
		{
			isUploadInvalid = true;
			getNextButton().setDisabled(selectedRegulationTypes.size() == 0 || isUploadInvalid);
			WebUtil.showErrorBoxWithCustomMessage("The uploaded XML is not in the correct format. Please review the help files for more details.");
		}		
		if(isUploadInvalid)
			filetoimport.setText(ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_REG_GUIDE_APPLY_NEW_CUSTOM_TEMPLATE));
    }
    
    private void injectRegulationSettings(AddServerWizardEntity wizardEntity) {

        final Map<String, RegulationType> selectedRegTypes = wizardEntity.getSelectedRegulationTypes();

        RegulationSettings rs = new RegulationSettings();

        if (selectedRegTypes.containsKey(PCI_DSS_NAME)) {
            rs.setPci(true);
        }
        if (selectedRegTypes.containsKey(HIPAA_NAME)) {
            rs.setHipaa(true);
        }
        if (selectedRegTypes.containsKey(DISA_NAME)) {
            rs.setDisa(true);
        }
        if (selectedRegTypes.containsKey(NERC_NAME)) {
            rs.setNerc(true);
        }
        if (selectedRegTypes.containsKey(CIS_NAME)) {
            rs.setCis(true);
        }
        if (selectedRegTypes.containsKey(SOX_NAME)) {
            rs.setSox(true);
        }
        if (selectedRegTypes.containsKey(FERPA_NAME)) {
            rs.setFerpa(true);
        }
        if(wizardImportEntity != null && wizardImportEntity.getAuditedActivities() != null)
        {
        	customAuditedActivities =  new RegulationCustomDetail();
        	customAuditedActivities.setAuditedDatabaseActivities(wizardImportEntity.getAuditedActivities());
        } 
        if(wizardImportEntity != null && wizardImportEntity.getServerAuditedActivities() != null)
        {
        	customAuditedActivities.setAuditedServerActivities(wizardImportEntity.getServerAuditedActivities());
        } 
    	try {  		  
    		regulationCustomDetail = InstancesFacade.getRegulationSettingsServer(rs);
    		auditedActivities = regulationCustomDetail.getAuditedDatabaseActivities();
    		auditedServerActivities = regulationCustomDetail.getAuditedServerActivities();
    		if (!(rs.getPci()
    				|| rs.getHipaa()
    				|| rs.isFerpa()
    				|| rs.isSox()
    				|| rs.isNerc()
    				|| rs.isDisa()) && customAuditedActivities != null && customAuditedActivities
    						.getAuditedDatabaseActivities() != null){
    			auditedActivities.setAuditDDL(false);
    			auditedActivities.setAuditSecurity(false);
    			auditedActivities.setAuditAdmin(false);
    		}
            if( customAuditedActivities != null && customAuditedActivities.getAuditedDatabaseActivities() != null){
            	DatabaseLevelAuditSettingsUnion(auditedActivities,customAuditedActivities.getAuditedDatabaseActivities());
            	}
            if( customAuditedActivities != null && customAuditedActivities.getAuditedServerActivities() != null){
            	ServerLevelAuditSettingsUnion(auditedServerActivities,customAuditedActivities.getAuditedServerActivities());
            	}
            wizardEntity.getServerConfigEntity().setAuditedActivities(auditedActivities);
            wizardEntity.getServerConfigEntity().setAuditedServerActivities(auditedServerActivities);
            Sessions.getCurrent().setAttribute("RegulationCustomDetail",regulationCustomDetail);
            wizardEntity.getServerConfigEntity().setRegulationSettings(rs);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERROR_SAVE_DATABASE);
        }        
    }
    
	public void DatabaseLevelAuditSettingsUnion(CMAuditedActivities auditedActivities,CMAuditedActivities customAuditedActivities){
		
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
	
	public void ServerLevelAuditSettingsUnion(CMAuditedActivities auditedActivities,CMAuditedActivities customAuditedActivities){
		
		auditedActivities.setAuditLogins(auditedActivities.isAuditLogins() || customAuditedActivities.isAuditLogins());
    	auditedActivities.setAuditFailedLogins(auditedActivities.isAuditFailedLogins() || customAuditedActivities.isAuditFailedLogins());
    	auditedActivities.setAuditSecurity(auditedActivities.isAuditSecurity() || customAuditedActivities.isAuditSecurity());
    	auditedActivities.setAuditSELECT(auditedActivities.isAuditSELECT() || customAuditedActivities.isAuditSELECT());
    	auditedActivities.setAuditAdmin(auditedActivities.isAuditAdmin() || customAuditedActivities.isAuditAdmin());
    	auditedActivities.setAuditDDL(auditedActivities.isAuditDDL() || customAuditedActivities.isAuditDDL());  
    	auditedActivities.setAuditDefinedEvents(auditedActivities.isAuditDefinedEvents() || customAuditedActivities.isAuditDefinedEvents());
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

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        wizardEntity.setSelectedRegulationTypes(selectedRegulationTypes); // used in summary step SummaryStepViewModel
        injectRegulationSettings(wizardEntity); // used to sent to server and in RegulationGuidelinesApplyStepViewModel
    }
}
