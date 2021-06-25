/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.ui.importAuditSetting;

import java.io.IOException;

import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.util.media.Media;
import org.zkoss.zk.ui.event.UploadEvent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Textbox;

import com.idera.sqlcm.facade.InstancesFacade;

public class NewImportStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/ImportAuditSetting/Import_audit_setting.zul";
    String xmlData;
    public AddWizardImportEntity wizardImportEntity; //testing
    
    @Wire
    Textbox filetoimport;
    
    @Override
    public boolean isFirst() {
        return true;
    }

    @Override
    public String getNextStepZul() {
        return ImportAuditSelectSetting.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    public NewImportStepViewModel() {
        super();
     }
	@Override
	public void onBeforePrev(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onBeforeNext(AddWizardImportEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		wizardSaveEntity.setDatabase(wizardImportEntity.getDatabase());
		wizardSaveEntity.setDatabasePrivUser(wizardImportEntity.getDatabasePrivUser());
		wizardSaveEntity.setDBDetails(wizardImportEntity.getDBDetails());
		wizardSaveEntity.setMatchDBNames(wizardImportEntity.getMatchDBNames());
		wizardSaveEntity.setPrivUserConfig(wizardImportEntity.getPrivUserConfig());
		wizardSaveEntity.setServerDetails(wizardImportEntity.getServerDetails());
		wizardSaveEntity.setDatabaseDetails(wizardImportEntity.getDatabaseDetails());
		wizardSaveEntity.setServerLevelConfig(wizardImportEntity.getServerLevelConfig());
		wizardSaveEntity.setXmlData(xmlData);
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
			if(!wizardImportEntity.validFile){
				com.idera.sqlcm.server.web.WebUtil.showInfoBoxWithCustomMessage("Unable to parse the selected file");
				}
		}
		else
		{
			com.idera.sqlcm.server.web.WebUtil.showInfoBoxWithCustomMessage("Unable to parse the selected file.");
		}
		
	}
}

/***End SQLCm 5.4***/