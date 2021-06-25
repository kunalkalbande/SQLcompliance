package com.idera.sqlcm.ui.dialogs.eventFilterWizard;

import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.steps.*;
import com.idera.sqlcm.wizard.AbstractFilterWizardViewModel;
import com.idera.sqlcm.wizard.WizardFilterStepManager;
import com.idera.sqlcm.wizard.WizardStepManager;

import org.zkoss.bind.annotation.AfterCompose;
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
public class EventFilterWizardViewModel extends
		AbstractFilterWizardViewModel<EventFilterSaveEntity> {

	public static final String ZUL_URL = "~./sqlcm/wizard/EventFilterWizard-dialog.zul";

	private static final String INSTANCE_ARG = "instance_arg";

	private CMInstance instance;
	
	/*@Wire
	Label newEventFilter;*/

	@Wire
	Window wizardFilterWindow;

	
	public CMInstance getInstance() {
		return instance;
	}

	@Override
	protected EventFilterSaveEntity createSaveEntity() {
		return new EventFilterSaveEntity();
	}

	@Init(superclass = true)
	public void init(@ExecutionArgParam(INSTANCE_ARG) CMInstance instance) {
		/*if (instance == null) {
			throw new RuntimeException(INSTANCE_ARG + " is null! ");
		}*/
		this.instance = instance;
	}
	
	 @AfterCompose(superclass = true)
		public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {		
		 Selectors.wireComponents(view, this, false);
			if(Sessions.getCurrent().getAttribute("Name")!=null
					&& !Sessions.getCurrent().getAttribute("QueryType").equals("FromExisting")){
				 //newEventFilter.setValue(ELFunctions.getLabel(SQLCMI18NStrings.EDIT_FILTER));
				 wizardFilterWindow.setTitle(ELFunctions.getLabel(SQLCMI18NStrings.EDIT_FILTER));
			 } //eventFilters eventFiltersRules
		}

	@Override
	public void registerSteps(final WizardFilterStepManager stepManager) {
		stepManager
				.registerStep(NewEventFilterStepViewModel.ZUL_PATH)				
				.registerStep(SelectNewEventStepViewModel.ZUL_PATH)
				.registerStep(SummaryStepViewModel.ZUL_PATH);
	}

	@Override
	protected void doSave(EventFilterSaveEntity wizardSaveModel) {
		cleanupModelForAuditCollectionLevel(wizardSaveModel);
	}

	private void cleanupTableListForDatabases(
			EventFilterSaveEntity wizardSaveModel) {
		for (CMDatabase db : wizardSaveModel.getDatabaseList()) {
			db.setSensitiveColumnTableData(null);
		}
	}

	/**
	 * This method removes all data from save entity that not needed for current
	 * audit collection level. Entity can contain redundant data in case user
	 * changed data in more than one wizard flow.
	 */
	private void cleanupModelForAuditCollectionLevel(
			EventFilterSaveEntity wizardSaveModel) {

	}

	public static void showEventFilterWizard(CMInstance instance,
			WizardListener listener) {
		Map args = new HashMap();
		args.put(INSTANCE_ARG, instance);
		args.put(LISTENER_ARG, listener);
		Window window = (Window) Executions.createComponents(EventFilterWizardViewModel.ZUL_URL, null, args);
		window.doHighlighted();
	}
}
