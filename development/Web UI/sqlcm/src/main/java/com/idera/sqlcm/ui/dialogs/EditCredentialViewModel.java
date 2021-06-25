package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMManageInstanceCredentials;
import com.idera.sqlcm.enumerations.AccountType;
import com.idera.sqlcm.facade.InstancesFacade;
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
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class EditCredentialViewModel {
    public static final String ZUL_URL = "~./sqlcm/dialogs/editCredential.zul";
    private static final String INSTANCE_ID_LIST = "instance-id-list";

    private String help;
    private ListModelList<AccountType> accountTypeModelList;
    private boolean showLogin = false;
    private boolean showCredentialFields = true;
    private CMManageInstanceCredentials credentials;
    private List<Long> instanceIdList;

    public static void showEditCredentialDialog(List<Long> instanceIdList) {
        Map<String, Object> args = new HashMap<>();
        args.put(INSTANCE_ID_LIST, instanceIdList);
        Window window = (Window) Executions.createComponents(ZUL_URL, null, args);
        window.doHighlighted();
    }

    public String getHelp() {
        return help;
    }

    public ListModelList<AccountType> getAccountTypeModelList() {
        return accountTypeModelList;
    }

    public boolean isShowLogin() {
        return showLogin;
    }

    public boolean isShowCredentialFields() {
        return showCredentialFields;
    }

    public CMManageInstanceCredentials getCredentials() {
        return credentials;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        help = "http://wiki.idera.com/x/KQQsAw";

        HashMap<String, Object> args = (HashMap<String, Object>)Executions.getCurrent().getArg();
        instanceIdList = (List<Long>)args.get(INSTANCE_ID_LIST);

        initializeListboxes();
        showCredentialFields();
        credentials = new CMManageInstanceCredentials();
    }

    @Command
    @NotifyChange({"showLogin", "showCredentialFields"})
    public void showCredentialFields() {
        AccountType selectedAccountType = accountTypeModelList.getSelection().iterator().next();
        if (selectedAccountType.equals(AccountType.SERVER_LOGIN_ACCOUNT)) {
            showCredentialFields = true;
            showLogin = true;
        } else if (selectedAccountType.equals(AccountType.WINDOWS_USER_ACCOUNT)){
            showCredentialFields = true;
            showLogin = false;
        } else if (selectedAccountType.equals(AccountType.CM_ACCOUNT)) {
            showCredentialFields = false;
            showLogin = false;
        }
    }

    @Command
    public void testCredentials() {
        credentials.setAccountType(accountTypeModelList.getSelection().iterator().next().getIndex());
        TestCredentialsViewModel.showTestCredentialsDialog(credentials, instanceIdList);
    }

    @Command
    public void save(@BindingParam("comp") Window x) {
        try {
            credentials.setAccountType(accountTypeModelList.getSelection().iterator().next().getIndex());
            InstancesFacade.updateManagedInstancesCredentials(credentials, instanceIdList);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPDATE_INSTANCE);
        }
        EventQueue<Event> eq = EventQueues.lookup(ManageSqlServersInstancesViewModel.UPDATE_MANAGE_INSTANCE_LIST_EVENT, EventQueues.APPLICATION, false);
        if (eq != null) {
            eq.publish(new Event("onClick", null, null ));
        }
        x.detach();
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    private void initializeListboxes() {
        accountTypeModelList = new ListModelList<>();
        accountTypeModelList.addAll(Arrays.asList(AccountType.WINDOWS_USER_ACCOUNT, AccountType.CM_ACCOUNT, AccountType.SERVER_LOGIN_ACCOUNT));
        accountTypeModelList.setSelection(Arrays.asList(AccountType.CM_ACCOUNT));
    }

}
