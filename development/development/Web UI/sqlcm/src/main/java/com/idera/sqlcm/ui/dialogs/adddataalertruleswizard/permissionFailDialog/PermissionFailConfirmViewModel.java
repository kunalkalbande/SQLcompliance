package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.permissionFailDialog;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMPermission;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.ExecutionArgParam;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class PermissionFailConfirmViewModel {

    private static final String ZUL_URL = "~./sqlcm/dialogs/adddataalertruleswizard/permission-fail-confirm-dialog.zul";

    public static final String INSTANCE_ARG               = "instance";
    public static final String FAILED_PERMISSION_LIST_ARG = "failedPermissionList";
    public static final String LISTENER_ARG               = "dialogListener";

    public static class CMPermissionDecorator implements IFailedPermissionItemDecorator {

        private static final String NAME_STYLE = " color: #000000; font-weight: bold; ";
        private static final String RESOLUTION_STEPS_STYLE = " color: #000000; ";

        private CMPermission cmPermission;

        public CMPermissionDecorator(CMPermission cmPermission) {
            this.cmPermission = cmPermission;
        }

        public String getName() {
            return (cmPermission != null) ? cmPermission.getName() : null;
        }

        public String getDesc() {
            return (cmPermission != null) ? cmPermission.getResolutionSteps() : null;
        }

        public String getNameStyle() {
            return NAME_STYLE;
        }

        public String getDescStyle() {
            return RESOLUTION_STEPS_STYLE;
        }

    }

    public static class FirstRow implements IFailedPermissionItemDecorator {

        private static final String NAME_STYLE = " color: #FF0000;  font-weight: bold; ";

        private String instanceName;

        public FirstRow(String instanceName) {
            this.instanceName = instanceName;
        }

        public String getName() {
            return ELFunctions.getLabelWithParams(SQLCMI18NStrings.PERMISSIONS_CHECK_FOR_INSTANCE, instanceName);
        }

        public String getDesc() {
            return "";
        }

        public String getNameStyle() {
            return NAME_STYLE;
        }

        public String getDescStyle() {
            return "";
        }

    }

    @Wire
    private Window permissionFailConfirmWindow;

    private List<IFailedPermissionItemDecorator> failedPermissionList;
    private int failedPermissionsCount = -1;
    private PermissionFailConfirmDialogListener listener;

    public static interface PermissionFailConfirmDialogListener {
        void onIgnore();
        void onReCheck();
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
                             @ExecutionArgParam(INSTANCE_ARG) CMInstance instance,
                             @ExecutionArgParam(FAILED_PERMISSION_LIST_ARG) List<CMPermission> failedList,
                             @ExecutionArgParam(LISTENER_ARG) PermissionFailConfirmDialogListener listener) {
        Selectors.wireComponents(view, this, false);

        if (instance == null) {
            throw new RuntimeException(INSTANCE_ARG + " must not be null! ");
        }

        if (failedList == null) {
            throw new RuntimeException(FAILED_PERMISSION_LIST_ARG + " must not be null! ");
        }

        this.listener = listener;

        this.failedPermissionList = new ArrayList<>(failedList.size() + 1);
        this.failedPermissionList.add(new FirstRow(instance.getInstanceName()));
        this.failedPermissionList.addAll(decorateFailedPermissionList(failedList));
        this.failedPermissionsCount = failedList.size();
    }

    private List<IFailedPermissionItemDecorator> decorateFailedPermissionList(List<CMPermission> failedPermissionList) {
        List<IFailedPermissionItemDecorator> decorateItems = new ArrayList<>(failedPermissionList.size());
        for (CMPermission p : failedPermissionList) {
            decorateItems.add(new CMPermissionDecorator(p));
        }
        return decorateItems;
    }

    @Command("ignoreCommand")
    public void ignoreCommand() {
        permissionFailConfirmWindow.detach();
        if (listener != null) {
            listener.onIgnore();
        }
    }

    @Command("stayCommand")
    public void stayCommand() {
        permissionFailConfirmWindow.detach();
        if (listener != null) {
            listener.onReCheck();
        }
    }

    public List<IFailedPermissionItemDecorator> getFailedPermissionList() {
        return failedPermissionList;
    }

    public String getReqPermission() {
        return ELFunctions.getLabelWithParams(SQLCMI18NStrings.PERMISSION_FAIL_DIALOG_REQ_PERMISSION, failedPermissionsCount);
    }

    public String getContinueOrStay() {
        return ELFunctions.getLabel(SQLCMI18NStrings.PERMISSION_FAIL_DIALOG_CAN_STILL_CONTINUE_OR_STAY);
    }

    public static void show(CMInstance instance, List<CMPermission> failedPermissionList,
                            PermissionFailConfirmDialogListener listener) {
        Map args = new HashMap();
        args.put(INSTANCE_ARG, instance);
        args.put(FAILED_PERMISSION_LIST_ARG, failedPermissionList);
        args.put(LISTENER_ARG, listener);
        Window window = (Window) Executions.createComponents(ZUL_URL, null, args);
        window.doHighlighted();
    }
}
