package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMArchiveProperties;
import com.idera.sqlcm.entities.CMUpdatedArchiveProperties;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Window;

import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;

public class ArchivePropertiesViewModel {

    public static final String ZUL_URL = "~./sqlcm/dialogs/archiveProperties/archive-properties-dialog.zul";

    private CMArchiveProperties archiveProperties;

    private String archiveName;

    private long oldDefaultAccess;

    private ListModelList<Permissions> permissionsListModelList;

    private DialogListener listener;

    @Wire("#archivePermissionsTab")
    Component archivePermissionsTab;

    @Wire("#archiveGeneralTab")
    Component generalTab;

    public interface DialogListener {
        void refreshDatabaseArchives();
    }

    public enum Permissions {
        GRANT_RIGHT_TO_READ(2, com.idera.sqlcm.server.web.ELFunctions.getLabel(SQLCMI18NStrings.ARCHIVE_PROPERTIES_DIALOG_GRANT_RIGHT_TO_READ)),
        GRANT_RIGHT_TO_READ_ONLY(1, com.idera.sqlcm.server.web.ELFunctions.getLabel(SQLCMI18NStrings.ARCHIVE_PROPERTIES_DIALOG_GRANT_RIGHT_TO_READ_ONLY)),
        DENY_READ_ACCESS(0, com.idera.sqlcm.server.web.ELFunctions.getLabel(SQLCMI18NStrings.ARCHIVE_PROPERTIES_DIALOG_DENY_READ_ACCESS));

        private String label;

        private long id;

        private Permissions(long id, String label) {
            this.id = id;
            this.label = label;
        }

        public String getLabel() {
            return label;
        }

        public long getId() {
            return id;
        }

        public static Permissions getByIndex(long index) {
            Permissions result = null;
            Permissions[] values = Permissions.values();
            for (int i = 0; i < values.length; i++) {
                if (values[i].getId() == index) {
                    result = values[i];
                }
            }
            return result;
        }
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        initArchiveProperties();
        initPermissionsList();
    }

    private void initPermissionsList() {
        permissionsListModelList = new ListModelList<>();
        permissionsListModelList.add(Permissions.GRANT_RIGHT_TO_READ);
        permissionsListModelList.add(Permissions.GRANT_RIGHT_TO_READ_ONLY);
        permissionsListModelList.add(Permissions.DENY_READ_ACCESS);
        permissionsListModelList.setSelection(Arrays.asList(Permissions.getByIndex(archiveProperties.getDefaultAccess())));
    }


    public ListModelList<Permissions> getPermissionsListModelList() {
        return permissionsListModelList;
    }

    public void setPermissionsListModelList(ListModelList<Permissions> permissionsListModelList) {
        this.permissionsListModelList = permissionsListModelList;
    }

    private void initArchiveProperties() {
        HashMap<String, Object> args = (HashMap<String, Object>) Executions.getCurrent().getArg();
        archiveName = (String) args.get("archiveName");
        listener = (DialogListener) args.get("listener");
        try {
            archiveProperties = DatabasesFacade.getArchiveProperties(archiveName);
            oldDefaultAccess = archiveProperties.getDefaultAccess();
        } catch (RestException e) {
            e.printStackTrace();
        }
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    @Command
    public void saveAndCloseDialog(@BindingParam("comp") Window x) {
        saveArchiveProperties();
        if (listener != null) {
            listener.refreshDatabaseArchives();
        }
        closeDialog(x);
    }

    private void saveArchiveProperties() {
        try {
            CMUpdatedArchiveProperties updateArchive = new CMUpdatedArchiveProperties();
            updateArchive.setDatabaseName(archiveProperties.getDatabaseName());
            updateArchive.setDescription(archiveProperties.getDescription());
            updateArchive.setDisplayName(archiveProperties.getDisplayName());
            updateArchive.setInstance(archiveProperties.getInstance());
            updateArchive.setNewDefaultAccess(archiveProperties.getDefaultAccess());
            updateArchive.setOldDefaultAccess(oldDefaultAccess);
            DatabasesFacade.updateArchiveProperties(updateArchive);
        } catch (RestException e) {
            e.printStackTrace();
        }
    }

    @Command("selectPermissions")
    public void selectPermissions(@BindingParam("radioGroup") Radiogroup radioGroup) throws RestException {
        archiveProperties.setDefaultAccess((Utils.getSingleSelectedItem(permissionsListModelList)).getId());
    }

    public CMArchiveProperties getArchiveProperties() {
        return archiveProperties;
    }

    public void setArchiveProperties(CMArchiveProperties archiveProperties) {
        this.archiveProperties = archiveProperties;
    }

    public static void showArchivePropertiesWindow(String archiveName, Object listener) {
        Map args = new HashMap();
        args.put("archiveName", archiveName);
        args.put("listener", listener);
        Window window = (Window) Executions.createComponents(ArchivePropertiesViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }
}

















