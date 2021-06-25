package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;


import com.idera.common.rest.JSONHelper;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.addserverwizard.AddInstanceResult;
import com.idera.sqlcm.entities.addserverwizard.AddServerEntity;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.addserverwizard.InstanceAvailableResult;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;

import java.io.IOException;

public class AddServerInvisibleStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/add-server-invisible-step.zul";

    @Override
    public String getNextStepZul() {
        String nextStepZulPath = null;
        try {
            String instanceName = getParentWizardViewModel().getWizardEntity().getAddServerEntity().getInstance();
            InstanceAvailableResult instanceAvailableResult = InstancesFacade.isInstanceAvailable(instanceName);
            if (instanceAvailableResult.isAvailable()) {
                nextStepZulPath = SelectDatabasesStepViewModel.ZUL_PATH;
            } else {
                nextStepZulPath = DatabaseLoadErrorStepViewModel.ZUL_PATH;
            }
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERROR_CHECK_INSTANCE_AVAILABILITY);
        }
        return nextStepZulPath;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_LICENSE_LIMIT_REACHED_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return null;
    }

    private AddInstanceResult addInstance(AddServerEntity serverEntity) {
        AddInstanceResult result = null;

        try {
            result = InstancesFacade.addInstance(serverEntity);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERROR_SAVE_INSTANCE);
        }

        if (result == null) {
            throw new RuntimeException(" AddInstanceResult must not be null! ");
        }

        return result;
    }

    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
        AddInstanceResult addInstanceResult = addInstance(wizardEntity.getAddServerEntity());

        if (!addInstanceResult.getWasSuccessfullyAdded()) {
            WebUtil.showErrorBox(new RestException(), addInstanceResult.getErrorMessage());
        }

        int agentDeploymentStatusId = wizardEntity.getAddServerEntity().getAgentDeployStatus();

        if (AgentDeploymentStepViewModel.DeployAgentOption.DEPLOY_NOW.getId() == agentDeploymentStatusId &&
                !addInstanceResult.getWasAgentDeployedAutomatically()) {
            WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.ERROR_DEPLOY_AGENT);
        }

        wizardEntity.setAddInstanceResult(addInstanceResult);

        CMInstance instance = new CMInstance();
        instance.setId(addInstanceResult.getServerId());
        instance.setName(addInstanceResult.getInstance());
        setInstance(instance);

        getParentWizardViewModel().goNext();

    }

    @Override
    public boolean isLast() {
        return false;
    }
}
