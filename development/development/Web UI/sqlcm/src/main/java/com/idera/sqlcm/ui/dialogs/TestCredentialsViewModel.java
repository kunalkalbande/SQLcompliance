package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMManageInstanceCredentials;
import com.idera.sqlcm.entities.CMValidateInstanceCredentialResult;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class TestCredentialsViewModel {
    public static final String ZUL_URL = "~./sqlcm/dialogs/testCredentials.zul";
    public static final String CREDENTIALS = "credentials";
    public static final String INSTANCE_ID_LIST = "instancesIdList";

    private ListModelList<CMValidateInstanceCredentialResult> credentialStatusModelList;
    private CMManageInstanceCredentials credentials;
    private List<Long> instanceIdList;

    public static void showTestCredentialsDialog(CMManageInstanceCredentials credentials, List<Long> instanceIdList) {
        Map<String, Object> args = new HashMap<>();
        args.put(CREDENTIALS, credentials);
        args.put(INSTANCE_ID_LIST, instanceIdList);
        Window window = (Window) Executions.createComponents(ZUL_URL, null, args);
        window.doHighlighted();
    }

    public ListModelList<CMValidateInstanceCredentialResult> getCredentialStatusModelList() {
        return credentialStatusModelList;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        HashMap<String, Object> args = (HashMap<String, Object>)Executions.getCurrent().getArg();
        credentials = (CMManageInstanceCredentials)args.get(CREDENTIALS);
        instanceIdList = (List<Long>)args.get(INSTANCE_ID_LIST);
        credentialStatusModelList = new ListModelList<>(validateCredentials());
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    private List<CMValidateInstanceCredentialResult> validateCredentials() {
        try {
            return InstancesFacade.validateInstanceCredentials(credentials, instanceIdList);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_VALIDATE_MANAGE_INSTANCE_CREDENTIALS);
        }
        return null;
    }
}
