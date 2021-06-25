package com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps;

import java.util.List;

import org.zkoss.zk.ui.Sessions;
import org.zkoss.zul.ListModelList;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.facade.CMTreeFacade;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;

public class RegulationGuidelinesApplyStepViewModel extends RegulationGuidelineAddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/regulationguidelinewizard/steps/regulation-guidelines-apply-step.zul";

    
    public RegulationGuidelinesApplyStepViewModel() {
        super();
    }

    @Override
    protected void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) {
        getNextButton().setDisabled(false);
        long dbId = (Long)Sessions.getCurrent().getAttribute("dbId");
        if(wizardSaveEntity.getDatabaseList() == null){
    	List<CMDatabase> listtreeModel = null;
    	try {
			listtreeModel = new ListModelList<CMDatabase>(CMTreeFacade.getTreeNodes(getInstance().getId()));
			for (CMDatabase cmDatabase : new ListModelList<CMDatabase>(
	    		    listtreeModel)) {
	    		if (cmDatabase.getId() != dbId) {
	    		    listtreeModel.remove(cmDatabase);
	    		}
    	    }
			wizardSaveEntity.setDatabaseList(listtreeModel);
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} 
        }       
    }
    
    @Override
    public String getNextStepZul() {
        return RegulationGuidelinesStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isFirst() {
        return true;
    }

    @Override
    public boolean isValid() {
        return true;
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
