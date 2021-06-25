package com.idera.sqlcm.wizard;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.*;
import org.zkoss.zhtml.A;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Button;
import org.zkoss.zul.Html;
import org.zkoss.zul.Panelchildren;
import org.zkoss.zul.Window;

public abstract class AbstractAlertWizardViewModel<WSE extends IWizardEntity> {

	private static final Logger logger = Logger
			.getLogger(AbstractAlertWizardViewModel.class);

	private static String MSG_COMPONENT_NOT_EXISTS = " %s component with id '%s' does not exist ";

	protected static final String LISTENER_ARG = "listener_arg";

	public static interface WizardListener {
		void onCancel();

	void onFinish();
	}

	private WizardListener listener;

	@Wire
	private Window wizardAlertWindow;
	
	/*@Wire
	private Html tipsHtml;
*/
	@Wire
	private A helpUrlHtml;
	
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
	private String helpUrl;
	private WSE wizardSaveEntity;

	private WizardAlertStepManager stepManager;

	public String getTips() {
		return tips;
	}

	public String getHelpUrl() {
		return helpUrl;
	}

	public WSE getWizardSaveEntity() {
		return wizardSaveEntity;
	}

	protected WizardAlertStepManager createStepManager() {
		return new WizardAlertStepManager(this);
	}

	protected abstract WSE createSaveEntity();

	public final void resetSaveEntity() {
		createSaveEntityOrException();
	}

	public abstract void registerSteps(final WizardAlertStepManager stepManager);

	@Init
	public void init(@ContextParam(ContextType.VIEW) Component view, @ContextParam(ContextType.BIND_CONTEXT) BindContext bindComposer) {
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
		IAlertStep step = stepManager.getFirst();
		tips = step.getTips();
		helpUrl = step.getHelpUrl();
		step.onShow(getWizardSaveEntity());
	}

	private void applyBeginButtonsState() {
		saveButton.setVisible(true);
		nextButton.setVisible(true);
		prevButton.setDisabled(true);
		if(Sessions.getCurrent().getAttribute("QueryType")!=null
				&& !Sessions.getCurrent().getAttribute("QueryType").equals("FromExisting")){
			saveButton.setDisabled(false);
		}
		else
			saveButton.setDisabled(true);
	}

	private void checkRequiredComponentsExistOrException() {
		if (wizardAlertWindow==null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"Window", "wizardAlertWindow"));
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
		if(wizardAlertWindow!=null){
			wizardAlertWindow.detach();
		}

		if (Sessions.getCurrent().getAttribute("QueryType") != null) {
			Sessions.getCurrent().removeAttribute("QueryType");
		}
		if (Sessions.getCurrent().getAttribute("dbMatchString") != null) {
			Sessions.getCurrent().removeAttribute("dbMatchString");
		}
		if (Sessions.getCurrent().getAttribute("appMatchString") != null) {
			Sessions.getCurrent().removeAttribute("appMatchString");
		}
		if (Sessions.getCurrent().getAttribute("hostMatchString") != null) {
			Sessions.getCurrent().removeAttribute("hostMatchString");
		}
		if (Sessions.getCurrent().getAttribute("loginMatchString") != null) {
			Sessions.getCurrent().removeAttribute("loginMatchString");
		}
		if (Sessions.getCurrent().getAttribute("objectMatchString") != null) {
			Sessions.getCurrent().removeAttribute("objectMatchString");
		}
		if (Sessions.getCurrent().getAttribute("Type") != null) {
			Sessions.getCurrent().removeAttribute("Type");
		}
		if(Sessions.getCurrent().getAttribute("Access Check Passed")!=null)
			Sessions.getCurrent().removeAttribute("Access Check Passed");
		if(Sessions.getCurrent().getAttribute("PrivilegedMatchString")!=null)
			Sessions.getCurrent().removeAttribute("PrivilegedMatchString");
		if(Sessions.getCurrent().getAttribute("fieldIdForSpecificEvent")!=null)
			Sessions.getCurrent().removeAttribute("fieldIdForSpecificEvent");
		if(Sessions.getCurrent().getAttribute("SQL Server")!=null)
			Sessions.getCurrent().removeAttribute("SQL Server");
		if(Sessions.getCurrent().getAttribute("ExecludeCertainEventIds")!=null)
			Sessions.getCurrent().removeAttribute("ExecludeCertainEventIds");
		Sessions.getCurrent().removeAttribute("eventCat");
		if (Sessions.getCurrent().getAttribute("set_messageTitle") != null) {
			Sessions.getCurrent().removeAttribute("set_messageTitle");
		}

		if (Sessions.getCurrent().getAttribute("set_messageBody") != null) {
			Sessions.getCurrent().removeAttribute("set_messageBody");
		}

		if (Sessions.getCurrent().getAttribute("addressList") != null) {
			Sessions.getCurrent().removeAttribute("addressList");
		}

		if (Sessions.getCurrent().getAttribute("mailAddress") != null) {
			Sessions.getCurrent().removeAttribute("mailAddress");
		}

		Sessions.getCurrent().removeAttribute("Category");
		
		Sessions.getCurrent().removeAttribute("specifyAlertMessage");
		
		if(Sessions.getCurrent().getAttribute("isValidSnmpAddress")!=null){
				Sessions.getCurrent().removeAttribute("isValidSnmpAddress");
		}
		if(Sessions.getCurrent().getAttribute("alertRuleId")!=null)
			Sessions.getCurrent().removeAttribute("alertRuleId");
		
		if(Sessions.getCurrent().getAttribute("privilegedUserNameMatch")!=null){
			Sessions.getCurrent().removeAttribute("privilegedUserNameMatch");
		}
		
		if(Sessions.getCurrent().getAttribute("rowCountDetails")!=null){
			Sessions.getCurrent().removeAttribute("rowCountDetails");
		}
	}

	@Command("cancelCommand")
	public void cancelButtonClick() {
		stepManager.getCurrentStep().onCancel(wizardSaveEntity);
		close();
    	if (listener != null) {
			listener.onCancel();
		}
	}
	
	@Command("nextCommand")
	@NotifyChange({ "tips", "currentStep" })
	public void nextButtonClick() {
		nextButton.setDisabled(true);
		if (stepManager.getCurrentStep().isValid()) {
			stepManager.getCurrentStep().onBeforeNext(wizardSaveEntity);
			if(Sessions.getCurrent().getAttribute("isValidSnmpAddress")==null 
					|| (boolean)Sessions.getCurrent().getAttribute("isValidSnmpAddress")){
			IAlertStep step = stepManager.next();
			updateTips(step);
			updateHelpUrl(step);
			updateButtonsVisibility(step);
			step.onShow(wizardSaveEntity);
			BindUtils.postNotifyChange(null, null, this, "helpUrl");
			}
			else{
				nextButton.setDisabled(false);
			}
		}
	}

	private void updateButtonsVisibility(IAlertStep step) {
		if (step == null) {
			logger.error(" Not able to update buttons visibility because step is null ");
			return;
		}
		if (step.isFirst()) {						
			prevButton.setDisabled(true);
		} else {
			prevButton.setDisabled(false);			
		}
		
		if(Sessions.getCurrent().getAttribute("QueryType")!=null
				&& !Sessions.getCurrent().getAttribute("QueryType").equals("FromExisting")){
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
		prevButton.setDisabled(true);
		stepManager.getCurrentStep().onBeforePrev(wizardSaveEntity);
		IAlertStep step = stepManager.prev();
		updateTips(step);
		updateHelpUrl(step);
		updateButtonsVisibility(step);
		step.onShow(wizardSaveEntity);
		BindUtils.postNotifyChange(null, null, this, "helpUrl");
	}

	private void updateTips(IAlertStep step) {
		tips = (step != null) ? step.getTips() : "";
	}

	private void updateHelpUrl(IAlertStep step) {
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

		if(Sessions.getCurrent().getAttribute("isValidSnmpAddress")==null){
			close();
		}
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

	public IAlertStep getCurrentStep() {
		return stepManager.getCurrentStep();
	}

}
