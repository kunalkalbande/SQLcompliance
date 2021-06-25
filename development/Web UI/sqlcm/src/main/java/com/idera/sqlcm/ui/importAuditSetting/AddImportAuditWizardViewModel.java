/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.ui.importAuditSetting;

import com.idera.sqlcm.wizard.IWizardEntity;
import com.idera.sqlcm.wizard.ImportAbstractWizardViewModel;
import com.idera.sqlcm.wizard.WizardImportStepManager;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.Window;

	import java.util.HashMap;
import java.util.Map;

	//@AfterCompose(superclass = true)
	public class AddImportAuditWizardViewModel extends ImportAbstractWizardViewModel<AddWizardImportEntity>
	{
	 public static final String ZUL_URL = "~./sqlcm/ImportAuditSetting/import_audit_setting_first.zul";

    private static final String INSTANCE_ARG = "instance_arg";

    private AddWizardImportEntity instance;
    
    public AddWizardImportEntity getInstance() {
        return instance;
    }
    
    @Override
    protected AddWizardImportEntity createSaveEntity() {
        return new AddWizardImportEntity();
    }
    
    @Init(superclass=true)
    public void init(){
    	
    }
    
    @AfterCompose(superclass = true)
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
	}  
    
    @Override
    public void registerSteps(final WizardImportStepManager stepManager) {
        stepManager
                .registerStep(NewImportStepViewModel.ZUL_PATH)
                .registerStep(ImportAuditSelectSetting.ZUL_PATH)
                .registerStep(ImportAuditTargetServer.ZUL_PATH)
                .registerStep(ImportAuditTargetDatabase.ZUL_PATH)
                .registerStep(ImportAuditSummary.ZUL_PATH);
    }

  

    /**
     * This method removes all data from save entity that not needed for current audit collection level.
     * Entity can contain redundant data in case user changed data in more than one wizard flow.
     */

    public static void showWizard(WizardListener listener) {
        Map args = new HashMap();
        args.put(LISTENER_ARG, listener);
        Window window = (Window) Executions.createComponents(AddImportAuditWizardViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }

	@Override
	protected void doSave(IWizardEntity wizardSaveModel) {
		// TODO Auto-generated method stub
		
	}
}
	
/***End SQLCm 5.4***/