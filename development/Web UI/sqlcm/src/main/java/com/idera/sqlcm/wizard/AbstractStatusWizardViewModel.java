package com.idera.sqlcm.wizard;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.*;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Button;
import org.zkoss.zul.Html;
import org.zkoss.zul.Panelchildren;
import org.zkoss.zul.Window;

public abstract class AbstractStatusWizardViewModel<WSE extends IWizardEntity> {

	private static final Logger logger = Logger
			.getLogger(AbstractStatusWizardViewModel.class);

	private static String MSG_COMPONENT_NOT_EXISTS = " %s component with id '%s' does not exist ";

	protected static final String LISTENER_ARG = "listener_arg";

	public static interface WizardListener {
		void onCancel();

		void onFinish();
	}

	private WizardListener listener;

	@Wire
	private Window wizardStatusWindow;
	
	/*@Wire
	private Html tipsHtml;*/

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

	private WizardStatusStepManager stepManager;

	public String getTips() {
		return tips;
	}

	public String getHelpUrl() {
		return helpUrl;
	}

	public WSE getWizardSaveEntity() {
		return wizardSaveEntity;
	}

	protected WizardStatusStepManager createStepManager() {
		return new WizardStatusStepManager(this);
	}

	protected abstract WSE createSaveEntity();

	public final void resetSaveEntity() {
		createSaveEntityOrException();
	}

	public abstract void registerSteps(final WizardStatusStepManager stepManager);

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
		IStatusStep step = stepManager.getFirst();
		tips = step.getTips();
		helpUrl = step.getHelpUrl();
		step.onShow(getWizardSaveEntity());
	}

	private void applyBeginButtonsState() {
		saveButton.setVisible(false);
		nextButton.setVisible(true);
		prevButton.setDisabled(true);
	}

	private void checkRequiredComponentsExistOrException() {
		if (wizardStatusWindow==null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"Window", "wizardStatusWindow"));
		}

		/*if (tipsHtml == null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"org.zkoss.zul.Html", "tipsHtml"));
		}*/

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
		if(wizardStatusWindow!=null){
			wizardStatusWindow.detach();
		}
	}

	@Command("cancelCommand")
	public void cancelButtonClick() {
		stepManager.getCurrentStep().onCancel(wizardSaveEntity);
		if(Sessions.getCurrent().getAttribute("set_messageTitle")!=null){
    		Sessions.getCurrent().removeAttribute("set_messageTitle");
    	}    	
    	
		if(Sessions.getCurrent().getAttribute("set_messageBody")!=null){
			Sessions.getCurrent().removeAttribute("set_messageBody");
		}
		if(Sessions.getCurrent().getAttribute("addressList")!=null){
			Sessions.getCurrent().removeAttribute("addressList");
		}
		
		if(Sessions.getCurrent().getAttribute("mailAddress")!=null){
			Sessions.getCurrent().removeAttribute("mailAddress");
		}
		Sessions.getCurrent().removeAttribute("specifyAlertMessage");
		Sessions.getCurrent().removeAttribute("isValidSnmpAddress");
		close();

		if (listener != null) {
			listener.onCancel();
		}
	}
	
	@Command("nextCommand")
	@NotifyChange({ "tips", "currentStep" })
	public void nextButtonClick() {
		if (stepManager.getCurrentStep().isValid()) {
			stepManager.getCurrentStep().onBeforeNext(wizardSaveEntity);
			if(Sessions.getCurrent().getAttribute("isValidSnmpAddress")==null 
					|| (boolean)Sessions.getCurrent().getAttribute("isValidSnmpAddress")){
				IStatusStep step = stepManager.next();
				updateTips(step);
				updateHelpUrl(step);
				updateButtonsVisibility(step);
				step.onShow(wizardSaveEntity);
				BindUtils.postNotifyChange(null, null, this, "helpUrl");
			}
		}
	}

	private void updateButtonsVisibility(IStatusStep step) {
		if (step == null) {
			logger.error(" Not able to update buttons visibility because step is null ");
			return;
		}

		if (step.isFirst()) {
			prevButton.setDisabled(true);
		} else {
			prevButton.setDisabled(false);
		}

		if (step.isLast()) {
			saveButton.setVisible(true);
			nextButton.setVisible(false);
		} else {
			saveButton.setVisible(false);
			nextButton.setVisible(true);
		}
	}

	@Command("prevCommand")
	@NotifyChange({ "tips", "currentStep" })
	public void prevButtonClick() {
		stepManager.getCurrentStep().onBeforePrev(wizardSaveEntity);
		IStatusStep step = stepManager.prev();
		updateTips(step);
		updateHelpUrl(step);
		updateButtonsVisibility(step);
		step.onShow(wizardSaveEntity);
		BindUtils.postNotifyChange(null, null, this, "helpUrl");
	}

	private void updateTips(IStatusStep step) {
		tips = (step != null) ? step.getTips() : "";
	}
	
	private void updateHelpUrl(IStatusStep step) {
        helpUrl = (step != null)? step.getHelpUrl() : "";
    }
	
	@Command("saveCommand")
	public void saveButtonClick() {
		Clients.showBusy(ELFunctions.getLabel(I18NStrings.SAVING));
		stepManager.getCurrentStep().onBeforeNext(wizardSaveEntity);
		stepManager.getCurrentStep().onFinish(wizardSaveEntity);
		doSave(wizardSaveEntity);
		Clients.clearBusy();

		if (listener != null) {
			listener.onFinish();
		}

		close();
	}

	protected abstract void doSave(WSE wizardSaveModel);

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

	public IStatusStep getCurrentStep() {
		return stepManager.getCurrentStep();
	}

}
