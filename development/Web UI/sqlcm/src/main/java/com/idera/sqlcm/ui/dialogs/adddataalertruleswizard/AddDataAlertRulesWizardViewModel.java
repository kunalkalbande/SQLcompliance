package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps.*;
import com.idera.sqlcm.wizard.AbstractDataAlertWizardViewModel;
import com.idera.sqlcm.wizard.AbstractWizardViewModel;
import com.idera.sqlcm.wizard.WizardDataStepManager;
import com.idera.sqlcm.wizard.WizardStepManager;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.ExecutionArgParam;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Label;
import org.zkoss.zul.Window;

import java.util.HashMap;
import java.util.Map;

//@AfterCompose(superclass = true)
public class AddDataAlertRulesWizardViewModel extends AbstractDataAlertWizardViewModel<AddDataAlertRulesSaveEntity> {

    public static final String ZUL_URL = "~./sqlcm/dataAlertRulesWizard/wizard-dialog.zul";

    private static final String INSTANCE_ARG = "instance_arg";

    private CMInstance instance;

    public CMInstance getInstance() {
        return instance;
    }
  
   /* @Wire
	private Label titleLabel;
*/
    @Wire
    Window wizardDataWindow;
    
    @Wire 
	Label newAlertRuleWizard;
    
    @Override
    protected AddDataAlertRulesSaveEntity createSaveEntity() {
        return new AddDataAlertRulesSaveEntity();
    }
    
    @Init(superclass=true)
    public void init(){
    	
    }
    
    @AfterCompose(superclass = true)
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		if(Sessions.getCurrent().getAttribute("QueryType")!=null 
				&& !Sessions.getCurrent().getAttribute("QueryType").equals("FromExisting")){
			//titleLabel.setValue(ELFunctions.getLabel(SQLCMI18NStrings.EDIT_DATA_ALERT_RULES));
			wizardDataWindow.setTitle(ELFunctions.getLabel(SQLCMI18NStrings.EDIT_DATA_ALERT_RULES));
		 }
		else
		{
			wizardDataWindow.setTitle(ELFunctions.getLabel(SQLCMI18NStrings.NEW_DATA_ALERT_RULES));
			//titleLabel.setValue(ELFunctions.getLabel(SQLCMI18NStrings.NEW_DATA_ALERT_RULES));
		}
		
	}
    
    
    @Override
    public void registerSteps(final WizardDataStepManager stepManager) {
        stepManager
                .registerStep(SelectDataFilterStepViewModel.ZUL_PATH)
                .registerStep(DataAlertActionStepViewModel.ZUL_PATH)
                .registerStep(NewDataAlertRulesStepViewModel.ZUL_PATH)
                .registerStep(SummaryStepViewModel.ZUL_PATH)
                .registerStep(SelectAdditionalDataFilterStepViewModel.ZUL_PATH);
    }

    @Override
    protected void doSave(AddDataAlertRulesSaveEntity wizardSaveModel) {
        /*cleanupModelForAuditCollectionLevel(wizardSaveModel);*/
        try {
            DatabasesFacade.addDataAlertRules(wizardSaveModel);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERROR_SAVE_DATABASE);
        }
    }


    /**
     * This method removes all data from save entity that not needed for current audit collection level.
     * Entity can contain redundant data in case user changed data in more than one wizard flow.
     */

    public static void showAddDataAlertRulesWizard(WizardListener listener) {
        Map args = new HashMap();
        args.put(LISTENER_ARG, listener);
        Window window = (Window) Executions.createComponents(AddDataAlertRulesWizardViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }    
    
    @Command("eventDataAlertRules")
    public void eventDataAlertRules(@BindingParam("id") long id) {
        SpecifySQLServerViewModel.showSpecifySQLServersDialog(id);
    }
}
