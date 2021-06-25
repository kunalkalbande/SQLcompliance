package com.idera.sqlcm.ui.dialogs.adddatabasewizard.permissionFailDialog;

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
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class PermissionFailConfirmViewModel {

    private static final String ZUL_URL = "~./sqlcm/dialogs/adddatabasewizard/permission-fail-confirm-dialog.zul";

    public static final String INSTANCE_ARG               = "instance";
    public static final String FAILED_PERMISSION_LIST_ARG = "failedPermissionList";
    public static final String INSTANCE_ARG_LIST               = "allInstances";
    public static final String ALL_FAILED_PERMISSION_LIST_ARG = "allFailedPermissionList";
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
                             @ExecutionArgParam(INSTANCE_ARG_LIST) List<CMInstance> instanceList,
                             @ExecutionArgParam(ALL_FAILED_PERMISSION_LIST_ARG) List<List<CMPermission>> allPermissionList,
                             @ExecutionArgParam(LISTENER_ARG) PermissionFailConfirmDialogListener listener) {
        Selectors.wireComponents(view, this, false);

        if (instance == null && instanceList == null) {
            throw new RuntimeException(INSTANCE_ARG + " must not be null! ");
        }

        if (failedList == null && allPermissionList == null) {
            throw new RuntimeException(FAILED_PERMISSION_LIST_ARG + " must not be null! ");
        }

        this.listener = listener;

        //failedList.size() + 1
        this.failedPermissionList = new ArrayList<>();
        if(instance != null && failedList != null)
        {
        	this.failedPermissionList.add(new FirstRow(instance.getInstanceName()));
        	this.failedPermissionList.addAll(decorateFailedPermissionList(failedList));
        	this.failedPermissionsCount = failedList.size();
        }
        else
        { 
        	int i= 0;
        	int failedPermissionCountList = 0;
        	for(CMInstance cmInstance :instanceList){
				this.failedPermissionList.add(new FirstRow(cmInstance.getInstanceName()));
				this.failedPermissionList.addAll(decorateFailedPermissionList(allPermissionList.get(i)));
				failedPermissionCountList = failedPermissionCountList + allPermissionList.get(i).size();
				i++;
        	}
        	this.failedPermissionsCount = failedPermissionCountList;
        }
        //this.failedPermissionsCount = failedList.size();
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
    	if(Sessions.getCurrent().getAttribute("showResolutionStep")!=null)
		{
			String Status = (String)Sessions.getCurrent().getAttribute("showResolutionStep");
			if(Status.equals("True"))
			{
        Map args = new HashMap();
        args.put(INSTANCE_ARG, instance);
        args.put(FAILED_PERMISSION_LIST_ARG, failedPermissionList);
        args.put(LISTENER_ARG, listener);
        Window window = (Window) Executions.createComponents(ZUL_URL, null, args);
        window.doHighlighted();
    }
}
    }
    
    public static void showList(List<CMInstance> instanceList,List<List<CMPermission>> allPermissionList, PermissionFailConfirmDialogListener listener) {
		if (Sessions.getCurrent().getAttribute("showResolutionStep") != null) {
			String Status = (String) Sessions.getCurrent().getAttribute(
					"showResolutionStep");
			if (Status.equals("True")) {
				Map args = new HashMap();
				args.put(INSTANCE_ARG_LIST, instanceList);
				args.put(ALL_FAILED_PERMISSION_LIST_ARG, allPermissionList);
				args.put(LISTENER_ARG, listener);
				Window window = (Window) Executions.createComponents(ZUL_URL,
						null, args);
				window.doHighlighted();
			}
		}
	}
}
