package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.common.grid.EventsGridViewModel;
import com.idera.sqlcm.entities.CMArchiveProperties;
import com.idera.sqlcm.entities.CMAttachArchive;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Window;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class AttachArchiveViewModel {

    public static final String ZUL_URL = "~./sqlcm/dialogs/attachArchive/attach-archive-dialog.zul";

    private List<CMAttachArchive> cmAttachArchivesList;

    private long instanceId;

    private ListModelList<CMAttachArchive> archivesListModelList;

    CMArchiveProperties archiveProperties;

    ArchivePropertiesViewModel.DialogListener listener;

    private Window window;

    private String helpURL = "http://wiki.idera.com/display/SQLCM/Attach+Archive+Database+window";

    @Wire("#showAllCheckBox")
    Checkbox showAll;

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        HashMap<String, Object> args = (HashMap<String, Object>) Executions.getCurrent().getArg();
        instanceId = (long) args.get("instanceId");
        listener = (ArchivePropertiesViewModel.DialogListener) args.get("listener");
        loadArchivesList();
    }

    @Command("reloadArchives")
    @NotifyChange("archivesListModelList")
    public void loadArchivesList() {
        archivesListModelList = new ListModelList<>();
        try {
            cmAttachArchivesList = DatabasesFacade.getDatabasesForArchiveAttachment(instanceId, showAll.isChecked());
            archivesListModelList.addAll(cmAttachArchivesList);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_DATABASES_FOR_ATTACH_ARCHIVE);
        }
    }

    public ListModelList<CMAttachArchive> getArchivesListModelList() {
        return archivesListModelList;
    }

    public void setArchivesListModelList(ListModelList<CMAttachArchive> archivesListModelList) {
        this.archivesListModelList = archivesListModelList;
    }

    @Command("closeDialog")
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    @Command("selectArchive")
    @NotifyChange("archiveProperties")
    public void loadArchivesProperties(@BindingParam("id") String archiveName) {
        archiveProperties = null;
        try {
            archiveProperties = DatabasesFacade.getArchiveProperties(archiveName);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_ARCHIVE_PROPERTIES);
        }
    }

    @Command("attachAction")
    public void attachAction(@BindingParam("comp") Window x) {
        window = x;
        if (archiveProperties != null && archiveProperties.isCompatibleSchema()) {
            attachArchive();
        } else if (archiveProperties != null && !archiveProperties.isCompatibleSchema()) {
            if (WebUtil.showConfirmationBox(SQLCMI18NStrings.ARCHIVE_PROPERTIES_DIALOG_UPGRADE_DATABASE_TO_EARLIER_MESSAGE, SQLCMI18NStrings.ARCHIVE_ATTACH_HEADER_MESSAGE,
                Messagebox.EXCLAMATION, true, (Object) null)) {
                upgradeEventDatabaseSchema();
                if (checkNeedIndexUpdatesForArchive()) {
                    if (isNeedOptimizeIndexes()) {
                        updateIndexesForEventDatabase();
                        attachArchive();
                    } else {
                        if (!isIndexStartTimeForArchiveDatabaseIsValid()) {
                            ApplyReindexForArchiveViewModel.showApplyReindexForArchiveWindow(archiveProperties.getDatabaseName(), (EventsGridViewModel) listener);
                            closeDialog(window);
                        }
                        attachArchive();
                    }
                } else {
                    attachArchive();
                }
            }
        }
    }

    private void attachArchive() {
        try {
            DatabasesFacade.attachArchive(archiveProperties.getDatabaseName());
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_ATTACH_ARCHIVE);
        }
        refreshListener();
        closeDialog(window);
    }

    private void upgradeEventDatabaseSchema() {
        try {
            DatabasesFacade.upgradeEventDatabaseSchema(archiveProperties.getDatabaseName());
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPGRADE_EVENT_DATABASE_SCHEMA);
            refreshListener();
            closeDialog(window);
        }
    }

    private boolean checkNeedIndexUpdatesForArchive() {
        try {
            return DatabasesFacade.checkNeedIndexUpdatesForArchive(archiveProperties.getDatabaseName());
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_CHECK_IF_NEED_INDEX_UPDATES);
            refreshListener();
            closeDialog(window);
        }
        return false;
    }

    private boolean isNeedOptimizeIndexes() {
        return WebUtil.showConfirmationBox(SQLCMI18NStrings.ARCHIVE_PROPERTIES_DIALOG_UPGRADE_DATABASE_TO_OLDER_MESSAGE, SQLCMI18NStrings.ARCHIVE_ATTACH_HEADER_MESSAGE,
            Messagebox.EXCLAMATION, true, (Object) null);
    }

    private void updateIndexesForEventDatabase() {
        try {
            DatabasesFacade.updateIndexesForEventDatabase(archiveProperties.getDatabaseName());
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPDATE_INDEXES_FOR_EVENT_DATABASE);
            refreshListener();
            closeDialog(window);
        }
    }

    private boolean isIndexStartTimeForArchiveDatabaseIsValid() {
        try {
            return DatabasesFacade.isIndexStartTimeForArchiveDatabaseIsValid();
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_CHECK_IS_VALID_START_TIME_FOR_ARCHIVE_DATABASE);
            refreshListener();
            closeDialog(window);
        }
        return false;
    }

    private void refreshListener() {
        if (listener != null) {
            listener.refreshDatabaseArchives();
        }
    }

 /*   @Command("reloadArchives")
    public void reloadArchives() {
        loadArchivesList();
    }*/

  /*  @Command("selectArchive")
    public void selectArchive(@BindingParam("id") String id) {
        loadArchivesProperties(id);
    }*/

    public CMArchiveProperties getArchiveProperties() {
        return archiveProperties;
    }

    public String getHelpURL() {
        return helpURL;
    }

    public static void showAttachArchiveWindow(long instanceId, EventsGridViewModel listener) {
        Map args = new HashMap();
        args.put("instanceId", instanceId);
        args.put("listener", listener);
        Window window = (Window) Executions.createComponents(AttachArchiveViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }
}

















