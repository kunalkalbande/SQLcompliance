/***Start SQLCm 5.4***/
/*Requirement 4.1.4.1*/

package com.idera.sqlcm.wizard;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.*;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Button;
import org.zkoss.zul.Html;
import org.zkoss.zul.Panelchildren;
import org.zkoss.zul.Window;

import java.util.Map;

/**
 * Important: You must use following annotations inside heirs of this class:
 *
 * @param <WSE>
 */
public abstract class ImportWizardViewModel<WSE extends IWizardEntity> {

    private static final Logger logger = Logger.getLogger(AbstractWizardViewModel.class);

    private static String MSG_COMPONENT_NOT_EXISTS = " %s component with id '%s' does not exist ";

    protected static final String LISTENER_ARG = "listener_arg";

    public interface WizardListener {
       void onCancel();
       void onFinish();
    }

    private WizardListener listener;

	@Wire
	private Window wizardWindow;
	
	@Wire
	private Window wizardAlertWindow;
	
	@Wire
	private Window wizardDataWindow;
	
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

    private WSE wizardEntity;

    private WizardStepManagerImport stepManager;

    public abstract String getTitle();

    public String getTips() {
        return tips;
    }

    public String getHelpUrl() {
        return helpUrl;
    }

    public WSE getWizardEntity() {
        return wizardEntity;
    }
    
    protected WizardStepManagerImport createStepManager() {
        return new WizardStepManagerImport(this);
    }

    protected abstract WSE createSaveEntity();

    public final void resetSaveEntity() {
        createSaveEntityOrException();
    }

    public abstract void registerSteps(final WizardStepManagerImport stepManager);

    @Init
    public void init(@ContextParam(ContextType.VIEW) Component view, @ContextParam(ContextType.BIND_CONTEXT) BindContext bindComposer) {
        stepManager = createStepManager();
        createSaveEntityOrException();
        registerSteps(stepManager);
    }

    private void createSaveEntityOrException() {
        wizardEntity = createSaveEntity();
        if (wizardEntity == null) {
            throw new RuntimeException(" wizardEntity must not be null! ");
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
        IStepImport step = stepManager.getFirst();
        tips = step.getTips();
        helpUrl = step.getHelpUrl();
        step.onShow(getWizardEntity());
    }

    private void applyBeginButtonsState() {
        saveButton.setVisible(false);
        nextButton.setVisible(true);
        prevButton.setDisabled(true);
    }

    private void checkRequiredComponentsExistOrException() {
		if (wizardWindow == null && wizardAlertWindow==null
				&& wizardDataWindow ==null && wizardImportWindow == null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"Window", "wizardWindow"));
		}

        if (prevButton == null) {
            throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS, "Button", "prevButton"));
        }

        if (nextButton == null) {
            throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS, "Button", "nextButton"));
        }

        if (saveButton == null) {
            throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS, "Button", "saveButton"));
        }

        if (cancelButton == null) {
            throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS, "Button", "cancelButton"));
        }

        if (mainPanelChildren == null) {
            throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS, "Panelchildren", "mainPanelChildren"));
        }
    }

	public final void close() {
		if(wizardWindow != null){
		wizardWindow.detach();
		}
		else if(wizardAlertWindow!=null){
			wizardAlertWindow.detach();
			listener=null;
		}
        else if(wizardDataWindow!=null){
        	wizardDataWindow.detach();
        	listener=null;

		}
        else if(wizardImportWindow!=null){
        	wizardImportWindow.detach();
        	listener=null;

        }
	}

	@Command("cancelCommand")
	public void cancelButtonClick() {
        if (stepManager.getCurrentStep().onBeforeCancel(wizardEntity)) {
            return;
        }

        stepManager.getCurrentStep().onCancel(wizardEntity);
        close();

        if (listener != null) {
            listener.onCancel();
        }
    }

    public void goNext() {
        goNext(false);
    }

    public void goNext(boolean removeCurrentStepFromStack) {
        IStepImport currentStep = stepManager.getCurrentStep();
        if (currentStep.isValid()) {
            currentStep.onBeforeNext(wizardEntity);
            IStepImport step = stepManager.next(removeCurrentStepFromStack);
            updateTips(step);
            updateHelpUrl(step);
            updateButtonsVisibility(step);
            if (step != null) {
                step.onShow(wizardEntity);
            }
            BindUtils.postNotifyChange(null, null, this, "tips");
            BindUtils.postNotifyChange(null, null, this, "helpUrl");
            BindUtils.postNotifyChange(null, null, this, "currentStep");
        }
    }

    @Command("nextCommand")
    public void nextButtonClick() {
        goNext();
    }

    private void updateButtonsVisibility(IStepImport step) {
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
    @NotifyChange({"tips", "helpUrl", "currentStep"})
    public void prevButtonClick() {
        stepManager.getCurrentStep().onBeforePrev(wizardEntity);
        IStepImport step = stepManager.prev();
        updateTips(step);
        updateHelpUrl(step);
        updateButtonsVisibility(step);
        step.onShow(wizardEntity);
    }

    private void updateTips(IStepImport step) {
        tips = (step != null)? step.getTips() : "";
    }

    private void updateHelpUrl(IStepImport step) {
        helpUrl = (step != null)? step.getHelpUrl() : "";
    }

    @Command("saveCommand")
    public void saveButtonClick() {
        if (!stepManager.getCurrentStep().isValid()) {
            return;
        }
        Clients.showBusy(ELFunctions.getLabel(I18NStrings.SAVING));
        stepManager.getCurrentStep().onBeforeNext(wizardEntity);
        stepManager.getCurrentStep().onFinish(wizardEntity);
        doSave(wizardEntity);
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

    public IStepImport getCurrentStep() {
        return stepManager.getCurrentStep();
    }

    protected static void showWizard(String zulPath, Map<?, ?> args) {
        Window window = (Window) Executions.createComponents(zulPath, null, args);
        window.doHighlighted();
    }
}

/***End SQLCm 5.4***/
