package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;


import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Textbox;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.addserverwizard.AddServerEntity;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.addserverwizard.AgentDeploymentProperties;
import com.idera.sqlcm.entities.addserverwizard.ServerRegisteredStatusInfo;
import com.idera.sqlcm.enumerations.ServerRegisteredStatus;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.SelectServerNameViewModel;

public class SpecifySqlServerStepViewModel extends AddWizardStepBase implements SelectServerNameViewModel.DialogListener {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/specify-sql-server-step.zul";

    @Wire
    private Textbox tbServerName;

    private String serverName = "";

    private String serverDesc = "";

    public String getServerName() {
        return serverName;
    }

    public void setServerName(String serverName) {
        this.serverName = serverName;
    }

    public String getServerDesc() {
        return serverDesc;
    }

    public void setServerDesc(String serverDesc) {
        this.serverDesc = serverDesc;
    }

    @Override
    public boolean isLast() {
        return false;
    }

    @Override
    public String getNextStepZul() {
        ServerRegisteredStatusInfo serverRegisteredStatusInfo = getParentWizardViewModel().getWizardEntity().getServerRegisteredStatusInfo();

        if (serverRegisteredStatusInfo == null) {
            throw new RuntimeException(" ServerRegisteredStatusInfo must not be null! ");
        }

        String nextZulPath;
        if (ServerRegisteredStatus.WAS_REGISTERED.getId() == serverRegisteredStatusInfo.getRegisteredStatus()) {
            nextZulPath = ExistingAuditDataStepViewModel.ZUL_PATH;
        } else {
            nextZulPath = SqlServerClusterStepViewModel.ZUL_PATH;
        }
        return nextZulPath;
    }

    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
        getNextButton().setDisabled(false); // next button always enable
        getPrevButton().setDisabled(true); // prev button always disabled
    }

    public ServerRegisteredStatusInfo loadServerStatusByName(String instanceName) {
        ServerRegisteredStatusInfo serverRegisteredStatusInfo = null;
        try {
            serverRegisteredStatusInfo = InstancesFacade.checkServerRegisteredStatusByName(instanceName);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_CREDENTIALS_ERROR_VALIDATE_CALL);
        }
        return serverRegisteredStatusInfo;
    }

    @Override
    public boolean isValid() {
        boolean isValid = true;
        if (validateServerName(serverName)) {
            ServerRegisteredStatusInfo serverRegisteredStatusInfo = loadServerStatusByName(serverName);
            if (serverRegisteredStatusInfo == null) {
                isValid = false;
            } else {
                getParentWizardViewModel().getWizardEntity().setServerRegisteredStatusInfo(serverRegisteredStatusInfo);
                ServerRegisteredStatus serverRegisteredStatus = ServerRegisteredStatus.getByIdOrException(serverRegisteredStatusInfo.getRegisteredStatus());
                if (ServerRegisteredStatus.IS_REGISTERED.equals(serverRegisteredStatus)) {
                    WebUtil.showErrorBox(new RuntimeException(), SQLCMI18NStrings.ADD_SERVER_WIZARD_SPECIFY_SERVER_ALREADY_REGISTED);
                    isValid = false;
                }
            }
        } else {
            isValid = false;
        }
        return isValid;
    }

    private boolean validateServerName(String serverName) {
        if (serverName != null && !serverName.trim().isEmpty()) {
            Clients.clearWrongValue(tbServerName);
            return true;
        }
        Clients.wrongValue(tbServerName, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_SPECIFY_SERVER_ENTER_SERVER_NAME));
        return false;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_SPECIFY_SERVER_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Add+Server+window";
    }

    @Command("onShowSelectServerListClick")
    public void onShowSelectServerListClick() {
        SelectServerNameViewModel.showDialog(this);
    }

    @Override
    public void onOk(String serverName) {
        this.serverName = serverName;
        BindUtils.postNotifyChange(null, null, this, "serverName");
    }

    @Override
    public void onCancel() {
        // do nothing
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        AddServerEntity addServerEntity = new AddServerEntity();
        addServerEntity.setInstance(serverName.trim());
        addServerEntity.setDescription(serverDesc.trim());
        addServerEntity.setAgentDeploymentProperties(new AgentDeploymentProperties());

        wizardEntity.setAddServerEntity(addServerEntity);
    }

}
