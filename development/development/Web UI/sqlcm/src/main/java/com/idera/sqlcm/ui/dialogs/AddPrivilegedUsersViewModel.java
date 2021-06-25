package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMInstanceUser;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;
import com.idera.sqlcm.enumerations.PermissionType;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.converter.PermissionToImagePathConverter;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.*;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;

import java.util.*;

public class AddPrivilegedUsersViewModel {

    private static final String ZUL_URL = "~./sqlcm/dialogs/instanceProperties/add-privileged-users-dialog.zul";

    private static final String PRIVILEGED_USERS_WINDOW_HELP_URL = "http://wiki.idera.com/display/SQLCM/Add+Privileged+Users+window";

    private static final String INSTANCE_ID_ARG          = "instance_id_arg";
    private static final String DIALOG_LISTENER_ARG      = "dialog_listener_arg";
    private static final String SELECTED_PERMISSIONS_ARG = "selected_permissions_arg";

    @Wire
    private Window addPrivilegedUsers;

    @Wire
    private Button addBtn;

    @Wire
    private Button removeBtn;

    private Converter permissionToImagePathConverter = new PermissionToImagePathConverter();

    private long currentInstanceId = Long.MIN_VALUE;

    private CMInstanceUsersAndRoles availableUsersAndRoles;

    private ListModelList<PermissionType> permissionTypeListModelList;

    private ListModelList<? extends CMInstancePermissionBase> availableUserListModel;

    private ListModelList<? extends CMInstancePermissionBase> availableRoleListModel;

    private ListModelList<? extends CMInstancePermissionBase> availableUsersAndRolesListModel;

    private ListModelList<CMInstancePermissionBase> selectedUsersAndRolesListModel;

    public Converter getPermissionToImagePathConverter() {
        return permissionToImagePathConverter;
    }

    public ListModelList<? extends CMInstancePermissionBase> getAvailableUsersAndRolesListModel() {
        return availableUsersAndRolesListModel;
    }

    public ListModelList<? extends CMInstancePermissionBase> getSelectedUsersAndRolesListModel() {
        return selectedUsersAndRolesListModel;
    }

    public ListModelList<PermissionType> getPermissionTypeListModelList() {
        return permissionTypeListModelList;
    }

    public interface DialogListener {
        void onOk(long instanceId, List<CMInstancePermissionBase> selectedPermissionList);
        void onCancel(long instanceId);
    }

    private DialogListener listener;

    public String getHelp() {
        return PRIVILEGED_USERS_WINDOW_HELP_URL;
    }

    private ListModelList<PermissionType> initPermissionTypeModel() {
        ListModelList<PermissionType> permissionTypeList = new ListModelList<>();
        permissionTypeList.addAll(Arrays.asList(PermissionType.SERVER_LOGINS, PermissionType.SERVER_ROLES));
        permissionTypeList.setSelection(Arrays.asList(PermissionType.SERVER_ROLES));
        return permissionTypeList;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
                             @ExecutionArgParam(INSTANCE_ID_ARG) long instanceId,
                             @ExecutionArgParam(SELECTED_PERMISSIONS_ARG) List<CMInstanceUser> selectedPermissionList,
                             @ExecutionArgParam(DIALOG_LISTENER_ARG) DialogListener dialogListener) {
        Selectors.wireComponents(view, this, false);

        currentInstanceId = instanceId;
        listener          = dialogListener;

        selectedUsersAndRolesListModel = new ListModelList<>();
        selectedUsersAndRolesListModel.setMultiple(true);
        if (selectedPermissionList != null) {
            selectedUsersAndRolesListModel.addAll(selectedPermissionList);
        }

        try {
            availableUsersAndRoles = InstancesFacade.getInstanceUsersAndRoles(instanceId);
            availableRoleListModel = Utils.createZkModelListFromList(availableUsersAndRoles.getRoleList());
            availableUserListModel = Utils.createZkModelListFromList(availableUsersAndRoles.getUserList());

            availableRoleListModel.setMultiple(true);
            availableUserListModel.setMultiple(true);

            permissionTypeListModelList = initPermissionTypeModel();

            PermissionType selectedPermissionType = Utils.getSingleSelectedItem(permissionTypeListModelList);

            switchAvailableListByPermissionType(selectedPermissionType);

            enableAddButtonIfSelected();
            enableRemoveButtonIfSelected();
        } catch (RestException e) {
            view.detach();
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_INSTANCE_USERS_AND_ROLES);
        }
    }

    private void switchAvailableListByPermissionType(PermissionType selectedLoginsRolesLocation) {

        if (selectedLoginsRolesLocation == null) {
            throw new IllegalArgumentException(" Permission argument must not be null! ");
        }

        switch (selectedLoginsRolesLocation) {
            case SERVER_LOGINS:
                availableUsersAndRolesListModel = availableUserListModel;
                break;
            case SERVER_ROLES:
                availableUsersAndRolesListModel = availableRoleListModel;
                break;
            default:
                throw new RuntimeException(" Invalid Permission type! ");
        }
    }

    @Command("onSelectPermissionType")
    @NotifyChange("availableUsersAndRolesListModel")
    public void onSelectPermissionType(@BindingParam("selectedItemValue") PermissionType selectedItemValue) {
        switchAvailableListByPermissionType(selectedItemValue);
    }

    @Command("onAddBtnClick")
    @NotifyChange("selectedUsersAndRolesListModel")
    public void onAddBtnClick() {
        Set<? extends CMInstancePermissionBase> selected = availableUsersAndRolesListModel.getSelection();
        for (CMInstancePermissionBase p : selected) {
            if (!selectedUsersAndRolesListModel.contains(p)) {
                selectedUsersAndRolesListModel.add(p);
            }
        }

    }

    @Command("onRemoveBtnClick")
    @NotifyChange("selectedUsersAndRolesListModel")
    public void onRemoveBtnClick() {
        Utils.removeAllSelectedItems(selectedUsersAndRolesListModel);
        enableRemoveButtonIfSelected();
    }

    @Command("onBtnOkClick")
    public void onBtnOkClick() {
        if (listener != null) {
            listener.onOk(currentInstanceId, selectedUsersAndRolesListModel);
        }
        addPrivilegedUsers.detach();
    }

    private void enableAddButtonIfSelected() {
        Set selectedItems = availableUsersAndRolesListModel.getSelection();
        if (selectedItems != null && selectedItems.size() > 0) {
            addBtn.setDisabled(false);
        } else {
            addBtn.setDisabled(true);
        }
    }

    private void enableRemoveButtonIfSelected() {
        Set selectedItems = selectedUsersAndRolesListModel.getSelection();
        if (selectedItems != null && selectedItems.size() > 0) {
            removeBtn.setDisabled(false);
        } else {
            removeBtn.setDisabled(true);
        }
    }

    @Command("onAvailableListItemClick")
    public void onAvailableListItemClick() {
        enableAddButtonIfSelected();
    }

    @Command("onSelectedListItemClick")
    public void onSelectedListItemClick() {
        enableRemoveButtonIfSelected();
    }

    @Command("onBtnCancelClick")
    public void onBtnCancelClick() {
        if (listener != null) {
            listener.onCancel(currentInstanceId);
        }
        addPrivilegedUsers.detach();
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    public static void showDialog(long instanceId, List<CMInstancePermissionBase> selectedPermissionList, DialogListener listener) {
        Map<String, Object> args = new HashMap<>();
        args.put(INSTANCE_ID_ARG, instanceId);
        args.put(SELECTED_PERMISSIONS_ARG, selectedPermissionList);
        args.put(DIALOG_LISTENER_ARG, listener);

        Window window = (Window) Executions.createComponents(AddPrivilegedUsersViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }

}
