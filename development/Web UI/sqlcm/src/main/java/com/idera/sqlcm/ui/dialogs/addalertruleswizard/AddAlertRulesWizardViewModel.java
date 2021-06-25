package com.idera.sqlcm.ui.dialogs.addalertruleswizard;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.steps.*;
import com.idera.sqlcm.wizard.AbstractAlertWizardViewModel;
import com.idera.sqlcm.wizard.WizardAlertStepManager;

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

public class AddAlertRulesWizardViewModel extends
		AbstractAlertWizardViewModel<AddAlertRulesSaveEntity> {

	public static final String ZUL_URL = "~./sqlcm/alertRulesWizard/wizard-dialog.zul";

	private static final String INSTANCE_ARG = "instance_arg";

	private CMInstance instance;
	
   @Wire
	Window wizardAlertWindow;
	
	@Wire 
	Label newAlertRuleWizard;
	
	public CMInstance getInstance() {
		return instance;
	}

	@Override
	protected AddAlertRulesSaveEntity createSaveEntity() {
		return new AddAlertRulesSaveEntity();
	}

	@Init(superclass = true)
	public void init(@ContextParam(ContextType.VIEW) Component view) {
	}
	 @AfterCompose(superclass = true)
		public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
			Selectors.wireComponents(view, this, false);
			if(Sessions.getCurrent().getAttribute("QueryType")!=null
					&& !Sessions.getCurrent().getAttribute("QueryType").equals("FromExisting")){
				// titleLabel.setValue(ELFunctions.getLabel(SQLCMI18NStrings.EDIT_EVENT_ALERT_RULES));
				wizardAlertWindow.setTitle(ELFunctions.getLabel(SQLCMI18NStrings.EDIT_EVENT_ALERT_RULES));
			}
			else
			{
				wizardAlertWindow.setTitle(ELFunctions.getLabel(SQLCMI18NStrings.NEW_EVENT_ALERT_RULES));
			}
			
			}
		
	@Override
	public void registerSteps(final WizardAlertStepManager stepManager) {
		stepManager.registerStep(SelectEventFilterStepViewModel.ZUL_PATH)
				.registerStep(AlertActionStepViewModel.ZUL_PATH)
				.registerStep(NewAlertRulesStepViewModel.ZUL_PATH)
				.registerStep(SummaryStepViewModel.ZUL_PATH);
	}

	@Override
	protected void doSave(AddAlertRulesSaveEntity wizardSaveModel) {
		try {
			DatabasesFacade.addAlertRules(wizardSaveModel);
		} catch (RestException e) {
			WebUtil.showErrorBox(e, SQLCMI18NStrings.ERROR_SAVE_DATABASE);
		}
	}

	 @Command
	    public void closeDialog(@BindingParam("comp") Window x) {
	        x.detach();
	    }
	/**
	 * This method removes all data from save entity that not needed for current
	 * audit collection level. Entity can contain redundant data in case user
	 * changed data in more than one wizard flow.
	 */


	
	public static void showAddAlertRulesWizard(WizardListener listener) {
		Map args = new HashMap();
		args.put(LISTENER_ARG, listener);
		Window window = (Window) Executions.createComponents(
				AddAlertRulesWizardViewModel.ZUL_URL, null, args);
		window.doHighlighted();
	}
}
