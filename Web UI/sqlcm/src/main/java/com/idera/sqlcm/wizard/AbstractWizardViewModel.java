package com.idera.sqlcm.wizard;

import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.List;
import java.util.Map;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

import org.apache.commons.io.FileUtils;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.ExecutionArgParam;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Button;
import org.zkoss.zul.Filedownload;
import org.zkoss.zul.Panelchildren;
import org.zkoss.zul.Window;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;

/**
 * Important: You must use following annotations inside heirs of this class:
 *
 * @param <WSE>
 */
public abstract class AbstractWizardViewModel<WSE extends IWizardEntity> {

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
	private Window wizardStatusWindow;

   /* @Wire
    private Html tipsHtml;
*/
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

    private WizardStepManager stepManager;

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
    
    protected WizardStepManager createStepManager() {
        return new WizardStepManager(this);
    }

    protected abstract WSE createSaveEntity();

    public final void resetSaveEntity() {
        createSaveEntityOrException();
    }

    public abstract void registerSteps(final WizardStepManager stepManager);

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
        IStep step = stepManager.getFirst();
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
				&& wizardDataWindow ==null && wizardStatusWindow == null) {
			throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS,
					"Window", "wizardWindow"));
		}

        /*if (tipsHtml == null) {
            throw new RuntimeException(String.format(MSG_COMPONENT_NOT_EXISTS, "org.zkoss.zul.Html", "tipsHtml"));
        }*/

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
        else if(wizardStatusWindow!=null){
        	wizardStatusWindow.detach();
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
        IStep currentStep = stepManager.getCurrentStep();
        if (currentStep.isValid()) {
            currentStep.onBeforeNext(wizardEntity);
            IStep step = stepManager.next(removeCurrentStepFromStack);
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

    private void updateButtonsVisibility(IStep step) {
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
        IStep step = stepManager.prev();
        updateTips(step);
        updateHelpUrl(step);
        updateButtonsVisibility(step);
        step.onShow(wizardEntity);
    }

    private void updateTips(IStep step) {
        tips = (step != null)? step.getTips() : "";
    }

    private void updateHelpUrl(IStep step) {
        helpUrl = (step != null)? step.getHelpUrl() : "";
    }

    @Command("saveCommand")
    public void saveButtonClick() {
        if (!stepManager.getCurrentStep().isValid()) {
            return;
        }
        if ((wizardEntity instanceof  AddDatabasesSaveEntity) && ((AddDatabasesSaveEntity) wizardEntity).getAuditedActivities() != null
				&& ((AddDatabasesSaveEntity) wizardEntity).getAuditedActivities().isCustomEnabled()) {

			String saveDialogResponce = WebUtil.RegulationGuidlineSaveDialog();
			if (saveDialogResponce == null) {
				return;
			} else {
				Clients.showBusy(ELFunctions.getLabel(I18NStrings.SAVING));
				stepManager.getCurrentStep().onBeforeNext(wizardEntity);
				stepManager.getCurrentStep().onFinish(wizardEntity);
				doSave(wizardEntity);
				Clients.clearBusy();
				if (listener != null) {
					listener.onFinish();
				}
				if (!saveDialogResponce.equals("")) {
					downloadCustomRegulationFile((AddDatabasesSaveEntity)wizardEntity, saveDialogResponce);
				}
			}
		}
        else if ((wizardEntity instanceof  AddServerWizardEntity) && ((AddServerWizardEntity) wizardEntity).getServerConfigEntity().getAuditedActivities() != null
				&& ((AddServerWizardEntity) wizardEntity).getServerConfigEntity().getAuditedActivities().isCustomEnabled()) {

			String saveDialogResponce = WebUtil.RegulationGuidlineSaveDialog();
			if (saveDialogResponce == null) {
				return;
			} else {
				Clients.showBusy(ELFunctions.getLabel(I18NStrings.SAVING));
				stepManager.getCurrentStep().onBeforeNext(wizardEntity);
				stepManager.getCurrentStep().onFinish(wizardEntity);
				doSave(wizardEntity);
				Clients.clearBusy();
				if (listener != null) {
					listener.onFinish();
				}
				if (!saveDialogResponce.equals("")) {
					if(((AddServerWizardEntity) wizardEntity).getServerConfigEntity().isServerType()){
						downloadCustomAddServerRegulationFile((AddServerWizardEntity)wizardEntity, saveDialogResponce);
					}
					else{
						downloadCustomAddDBRegulationFile((AddServerWizardEntity)wizardEntity, saveDialogResponce);
					}
				}
			}
		}
		else{
			Clients.showBusy(ELFunctions.getLabel(I18NStrings.SAVING));
			stepManager.getCurrentStep().onBeforeNext(wizardEntity);
			stepManager.getCurrentStep().onFinish(wizardEntity);
			doSave(wizardEntity);
			Clients.clearBusy();
			if (listener != null) {
				listener.onFinish();
			}
		}
        close();
    }
    
    public void downloadCustomRegulationFile(AddDatabasesSaveEntity wizardSaveEntity,String fileName) {
    	List<CMDatabase> dblist =  wizardSaveEntity.getDatabaseList();
		DatabasesFacade df = new DatabasesFacade();
		List<String> fileLists = df.ExportDatabaseRegulationAuditSettings(dblist);
		if (fileLists != null && fileLists.size() > 0) {
			String zipPath = zipFiles(fileLists,fileName);
			fileDownload(zipPath);
			deleteFiles(fileLists);
		} else {
			WebUtil.showErrorWithCustomMessage(
					ELFunctions.getLabel(SQLCMI18NStrings.REGULATION_FILE_EXPORT_ERROR));
		}
	}
    
    public void downloadCustomAddServerRegulationFile(AddServerWizardEntity wizardSaveEntity,String fileName) {
    	String srvName = ((AddServerWizardEntity) wizardEntity).getAddInstanceResult().getInstance();
		InstancesFacade iFac = new InstancesFacade();
		List<String> fileLists = iFac.ExportServerRegulationAuditSettings(srvName);
		if (fileLists != null && fileLists.size() > 0) {
			String zipPath = zipFiles(fileLists,fileName);
			fileDownload(zipPath);
			deleteFiles(fileLists);
		} else {
			WebUtil.showErrorWithCustomMessage(
					ELFunctions.getLabel(SQLCMI18NStrings.REGULATION_FILE_EXPORT_ERROR));
		}
	}
    
	public void downloadCustomAddDBRegulationFile(AddServerWizardEntity wizardEntity,String fileName) {
			List<CMDatabase> dblist = ((AddServerWizardEntity) wizardEntity).getServerConfigEntity().getDatabaseList();
			DatabasesFacade df = new DatabasesFacade();
			List<String> fileLists = df.ExportDatabaseRegulationAuditSettings(dblist);
			if (fileLists != null && fileLists.size() > 0) {
				String zipPath = zipFiles(fileLists,fileName);
				fileDownload(zipPath);
				deleteFiles(fileLists);
			} else {
				WebUtil.showErrorWithCustomMessage(
						ELFunctions.getLabel(SQLCMI18NStrings.REGULATION_FILE_EXPORT_ERROR));
		}
	}
	
	private void deleteFiles(List<String> fileLists)
	{
		for(String file: fileLists)
		{
		try {
			Files.deleteIfExists(Paths.get(file));
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		}
	}
	
	public String zipFiles(List<String> files,String zipName){
        FileOutputStream fos = null;
        ZipOutputStream zipOut = null;
        FileInputStream fis = null;
        String zipFileName = "";
        try {
        	File zipParentName = new File(files.get(0));
        	String zipFileDirectory =zipParentName.getParent() + "\\ArchieveDirectory";
        	if(Files.isDirectory(Paths.get(zipFileDirectory))){
        		FileUtils.forceDelete(new File(zipFileDirectory));
        	}
        	Files.createDirectory(Paths.get(zipFileDirectory));
        	zipFileName =  zipFileDirectory + "\\" + zipName + ".zip";
            fos = new FileOutputStream(zipFileName);
            zipOut = new ZipOutputStream(new BufferedOutputStream(fos));
            for(String filePath : files){
                File input = new File(filePath);
                fis = new FileInputStream(input);
                ZipEntry ze = new ZipEntry(input.getName());
                zipOut.putNextEntry(ze);
                byte[] tmp = new byte[4*1024];
                int size = 0;
                while((size = fis.read(tmp)) != -1){
                    zipOut.write(tmp, 0, size);
                }
                zipOut.flush();
                fis.close();
            }
            zipOut.close();            
        } catch (FileNotFoundException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } catch (IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        } finally{
            try{
                if(fos != null) fos.close();
            } catch(Exception ex){
                 
            }
        }
        return zipFileName;
    }
	
	protected void fileDownload(String zipFile) {
		try {
			File file = new File(zipFile);
			Filedownload.save(file, "application/zip");
		} catch (Exception e) {
			// TODO: handle exception
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

    public IStep getCurrentStep() {
        return stepManager.getCurrentStep();
    }

    protected static void showWizard(String zulPath, Map<?, ?> args) {
        Window window = (Window) Executions.createComponents(zulPath, null, args);
        window.doHighlighted();
    }
}
