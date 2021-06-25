package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.addserverwizard.AddInstanceResult;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.utils.CmCsvReader;
import org.apache.log4j.Logger;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.util.media.Media;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.UploadEvent;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;

import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Arrays;
import java.util.List;

public class ImportSqlServersViewModel {
    public static final String ZUL_URL = "~./sqlcm/dialogs/importSqlServers.zul";
    private static final Logger logger = Logger.getLogger(ImportSqlServersViewModel.class);

    private String help;
    private boolean showListBox;

    private ListModelList<String> importInstanceListModel = new ListModelList<>();

    public static void showImportSQLServersDialog() {
        Window window = (Window) Executions.createComponents(ZUL_URL, null, null);
        window.doHighlighted();
    }

    public String getHelp() {
        return help;
    }

    public ListModelList<String> getImportInstanceListModel() {
        return importInstanceListModel;
    }

    public boolean isDisabledImport() {
        return getImportInstanceListModel().getSelection().isEmpty();
    }

    public boolean isShowListBox() {
        return showListBox;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        help = "http://wiki.idera.com/x/JAQsAw";
        importInstanceListModel.setMultiple(true);
        showListBox = false;
    }

    @Command
    public void importInstances(@BindingParam("comp") Window x) {
        try {
            List<AddInstanceResult> responses = InstancesFacade.importInstances(importInstanceListModel.getSelection());
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_IMPORT_INSTANCES);
        }
        x.detach();
    }

    @Command
    @NotifyChange({"importInstanceGrid", "importInstanceListModel", "disabledImport", "showListBox"})
    public void uploadFile(@ContextParam(ContextType.TRIGGER_EVENT) UploadEvent event){
        processUploadEvent(event.getMedia());
    }

    private void processUploadEvent(Media media){
        if (media == null || !media.getName().endsWith(".csv")){
            WebUtil.showErrorBox(new Exception(ELFunctions.getLabel(I18NStrings.NOT_CSV_FILE)), I18NStrings.INVALID_FILE_FORMAT);
            return;
        }
        importInstanceListModel.clear();

        CmCsvReader reader =  new CmCsvReader(media.isBinary() ? new InputStreamReader(media.getStreamData()) : media.getReaderData());
        try{
            try {
                List<String[]> fileContent = reader.readAll();
                if (fileContent == null){
                    WebUtil.showErrorBox(I18NStrings.ERROR, I18NStrings.FAILED_TO_READ_FILE);
                    return;
                }
                for (String[] tmp: fileContent){
                    for (String singleContent: Arrays.asList(tmp)){
                        if (singleContent.trim().isEmpty() || importInstanceListModel.contains(singleContent)) continue;
                        importInstanceListModel.add(singleContent);
                    }
                }
                importInstanceListModel.setSelection(importInstanceListModel);
            } catch (IOException e) {
                WebUtil.showErrorBox(e, I18NStrings.FAILED_TO_READ_FILE);
            }
        }
        finally{
            try {
                reader.close();
            } catch (IOException e) {
                // user cannot do anything here if we failed to close the file.
                logger.error(I18NStrings.FAILED_TO_CLOSE_FILE, e);
            }
        }
        showListBox = true;
    }

    @Command
    @NotifyChange("disabledImport")
    public void checkImportButtonState() {}

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }
}
