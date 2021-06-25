/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.wizard;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.annotation.*;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Button;
import org.zkoss.zul.Panelchildren;
import org.zkoss.zul.Window;

public abstract class ImportAbstractWizardViewModel<WSE extends IWizardEntity> {

	private static final Logger logger = Logger
			.getLogger(ImportAbstractWizardViewModel.class);

	private static String MSG_COMPONENT_NOT_EXISTS = " %s component with id '%s' does not exist ";

	protected static final String LISTENER_ARG = "listener_arg";

	public static interface WizardListener {
		void onCancel();
		void onFinish();
	}

	private WizardListener listener;

	@Wire
	private Window wizardImportWindow;

	@Wire
	protected Panelchildren mainPanelChildren;

	@Wire
	protected Button cancelButton;

	@Wire
	protected Button prevButton;

	@Wire
	protected Button nextButton;

	@Wire
	protected Button saveButton;

	private String tips;
	private String helpUrl = "";
	
	private WSE wizardSaveEntity;

	private WizardImportStepManager stepManager;
	
	protected abstract WSE createSaveEntity();
	
	public String getTips() {
		return tips;
	}

	public String getHelpUrl() {
		return helpUrl;
	}

	public WSE getWizardSaveEntity() {
		return wizardSaveEntity;
	}

	protected WizardImportStepManager createStepManager() {
		return new WizardImportStepManager(this);
	}

	public abstract void registerSteps(final WizardImportStepManager stepManager);

	@Init
	public void init(@ContextParam(ContextType.VIEW) Component view,
			@ContextParam(ContextType.BIND_CONTEXT) BindContext bindComposer) {
		stepManager = createStepManager();
		createSaveEntityOrException();
		registerSteps(stepManager);
	}
	
	private void createSaveEntityOrException() {
		wizardSaveEntity = createSaveEntity();
		if (wizardSaveEntity == null) {
			throw new RuntimeException(" wizardSaveEntity must not be null! ");
		}
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
			@ContextParam(ContextType.BIND_CONTEXT) BindContext bindContext,
			@ExecutionArgParam(LISTENER_ARG) WizardListener listener) {
		Selectors.wireComponents(view, this, false);
		this.listener = listener;
		checkRequiredComponentsExistOrException();
		applyBeginButtonsState();
		stepManager.initSteps(mainPanelChildren);
		IImportStep step = stepManager.getFirst();
		tips = step.getTips();
		step.onShow(getWizardSaveEntity());
	}

	private void applyBeginButtonsState() {
		saveButton.setVisible(true);
		nextButton.setVisible(true);
		prevButton.setDisabled(true);
	}

	
	private void checkRequiredComponentsExistOrException() {
		if (wizardImportWindow==null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"Window", "wizardImportWindow"));
		}
		
		if (prevButton == null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"Button", "prevButton"));
		}

		if (nextButton == null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"Button", "nextButton"));
		}

		if (saveButton == null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"Button", "saveButton"));
		}

		if (cancelButton == null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"Button", "cancelButton"));
		}

		if (mainPanelChildren == null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"Panelchildren", "mainPanelChildren"));
		}
	}

	public final void close() {
		if(wizardImportWindow!=null){
			wizardImportWindow.detach();
		}
		Sessions.getCurrent().removeAttribute("targetDbList");
		Sessions.getCurrent().removeAttribute("serverNames");
	}

	@Command("cancelCommand")
	public void cancelButtonClick() {
		stepManager.getCurrentStep().onCancel();
		close();
		if (listener != null) {
			listener.onCancel();
		}
	}
	
	@Command("nextCommand")
	public void nextButtonClick() {
		if (true) {
			stepManager.getCurrentStep().onBeforeNext(wizardSaveEntity);
			IImportStep step = stepManager.next();
			updateTips(step);
			updateButtonsVisibility(step);
			step.onShow(wizardSaveEntity);
		}
	}

	private void updateButtonsVisibility(IImportStep step) {
		if (step == null) {
			logger.error(" Not able to update buttons visibility because step is null ");
			return;
		}
		if (step.isFirst()) {						
			prevButton.setDisabled(true);
		} else {
			prevButton.setDisabled(false);			
		}
		
		if(Sessions.getCurrent().getAttribute("QueryType")!=null){
			saveButton.setDisabled(false);
		}
		else
			saveButton.setDisabled(true);

		if (step.isLast()) {
			saveButton.setDisabled(false);
			nextButton.setDisabled(true);
		} else {			
			nextButton.setDisabled(false);
		}
	}

	@Command("prevCommand")
	@NotifyChange({ "tips", "currentStep" })
	public void prevButtonClick() {
		stepManager.getCurrentStep().onBeforePrev();
		IImportStep step = stepManager.prev();
		updateButtonsVisibility(step);
		step.onShow(wizardSaveEntity);
	}

	private void updateTips(IImportStep step) {
		tips = (step != null) ? step.getTips() : "";
	}
	
	@Command("saveCommand")
	public void saveButtonClick() {
		Clients.showBusy(ELFunctions.getLabel(I18NStrings.SAVING));
		stepManager.getCurrentStep().onBeforeNext(wizardSaveEntity);
		stepManager.getCurrentStep().onFinish(wizardSaveEntity);
		Clients.clearBusy();

		if (listener != null) {
			listener.onFinish();
		}

		close();
	}
	

	public Button getCancelButton() {
		return cancelButton;
	}

	public Button getPrevButton() {
		return prevButton;
	}

	public Button getNextButton() {
		return nextButton;
	}

	public Button getSaveButton() {
		return saveButton;
	}

	public IImportStep getCurrentStep() {
		return stepManager.getCurrentStep();
	}

	protected void doSave(IWizardEntity wizardSaveModel) {
		// TODO Auto-generated method stub
	}
}

/***End SQLCm 5.4***/
