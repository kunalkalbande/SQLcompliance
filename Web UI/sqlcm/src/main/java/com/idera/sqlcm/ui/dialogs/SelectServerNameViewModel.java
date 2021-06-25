package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import org.zkoss.bind.annotation.*;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;

import java.util.HashMap;
import java.util.Map;

public class SelectServerNameViewModel {

    private static final String ZUL_URL = "~./sqlcm/dialogs/select-server-name-dialog.zul";

    private static final String PRIVILEGED_USERS_WINDOW_HELP_URL = "http://wiki.idera.com/display/SQLCM/Select+SQL+Server+window/";

    private static final String DIALOG_LISTENER_ARG = "dialog_listener_arg";

    @Wire
    private Window window;

    @Wire
    private Button okButton;

    public interface DialogListener {
        void onOk(String instanceName);
        void onCancel();
    }

    private DialogListener listener;

    private ListModelList<String> instanceNameListModelList = new ListModelList<>();

    public ListModelList<String> getInstanceNameListModelList() {
        return instanceNameListModelList;
    }

    public String getHelp() {
        return PRIVILEGED_USERS_WINDOW_HELP_URL;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
                             @ExecutionArgParam(DIALOG_LISTENER_ARG) DialogListener dialogListener) {
        Selectors.wireComponents(view, this, false);
        listener = dialogListener;
        okButton.setDisabled(true);

        try {
            instanceNameListModelList.addAll(InstancesFacade.getAllNotRegisteredInstanceNameList());
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.SERVER_LIST_POPUP_ERROR_LOAD_SERVERS_NAMES);
        }

    }

    @Command("onOkBtnClick")
    public void onOkBtnClick() {
        if (listener != null) {
            listener.onOk(Utils.getSingleSelectedItem(instanceNameListModelList));
        }
        window.detach();
    }

    @Command("onCancelBtnClick")
    public void onCancelBtnClick() {
        if (listener != null) {
            listener.onCancel();
        }
        window.detach();
    }

    @Command("onSelectItem")
    public void onSelectItem() {
        String instanceName = Utils.getSingleSelectedItem(instanceNameListModelList);
        if (instanceName != null) {
            okButton.setDisabled(false);
        }
    }

    public static void showDialog(DialogListener listener) {
        Map<String, Object> args = new HashMap<>();
        args.put(DIALOG_LISTENER_ARG, listener);
        Window window = (Window) Executions.createComponents(SelectServerNameViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }

}
