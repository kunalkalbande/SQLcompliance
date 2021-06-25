package com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps.*;
import com.idera.sqlcm.wizard.AbstractStatusWizardViewModel;
import com.idera.sqlcm.wizard.WizardStatusStepManager;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
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
public class AddStatusAlertRulesWizardViewModel extends AbstractStatusWizardViewModel<AddStatusAlertRulesSaveEntity> {

    public static final String ZUL_URL = "~./sqlcm/statusAlertRulesWizard/wizard-dialog.zul";

    private static final String INSTANCE_ARG = "instance_arg";

    private CMInstance instance;

   /* @Wire
	private Label titleLabel;*/
    
    @Wire
    Window wizardStatusWindow;
    
    @Wire
    Label newAlertRuleWizard;
    
    public CMInstance getInstance() {
        return instance;
    }

    @Override
    protected AddStatusAlertRulesSaveEntity createSaveEntity() {
        return new AddStatusAlertRulesSaveEntity();
    }
  
    @Init(superclass=true)
    public void init(){
    	
    }

    
    @AfterCompose(superclass = true)
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		if(Sessions.getCurrent().getAttribute("QueryType")!=null
				&& !Sessions.getCurrent().getAttribute("QueryType").equals("FromExisting")){
			//titleLabel.setValue(ELFunctions.getLabel(SQLCMI18NStrings.EDIT_STATUS_ALERT_RULES));
			wizardStatusWindow.setTitle(ELFunctions.getLabel(SQLCMI18NStrings.EDIT_STATUS_ALERT_RULES));
		 }
		else
		{
			wizardStatusWindow.setTitle(ELFunctions.getLabel(SQLCMI18NStrings.NEW_STATUS_ALERT_RULES));
			//titleLabel.setValue(ELFunctions.getLabel(SQLCMI18NStrings.NEW_STATUS_ALERT_RULES));
		}
		//Sessions.getCurrent().removeAttribute("QueryType");
	}  
    
    @Override
    public void registerSteps(final WizardStatusStepManager stepManager) {
        stepManager
                .registerStep(StatusAlertActionStepViewModel.ZUL_PATH)
                .registerStep(NewStatusAlertRuleStepViewModel.ZUL_PATH)
                .registerStep(SummaryStepViewModel.ZUL_PATH);
    }

    @Override
    protected void doSave(AddStatusAlertRulesSaveEntity wizardSaveModel) {
        try {
            DatabasesFacade.addStatusAlertRules(wizardSaveModel);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERROR_SAVE_DATABASE);
        }
    }


    /**
     * This method removes all data from save entity that not needed for current audit collection level.
     * Entity can contain redundant data in case user changed data in more than one wizard flow.
     */

    public static void showAddStatusAlertRulesWizard(WizardListener listener) {
        Map args = new HashMap();
        args.put(LISTENER_ARG, listener);
        Window window = (Window) Executions.createComponents(AddStatusAlertRulesWizardViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }    
}
