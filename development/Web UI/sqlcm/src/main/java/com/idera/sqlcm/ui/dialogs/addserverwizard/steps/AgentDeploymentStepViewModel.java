package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;


import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.addserverwizard.AddServerEntity;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.addserverwizard.AgentDeploymentProperties;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;
import com.idera.sqlcm.server.web.WebUtil;
import org.zkoss.bind.BindUtils;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;

import java.util.Arrays;

public class AgentDeploymentStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/agent-deployment-step.zul";

    private boolean isAgentAlreadyDeployed = false;
    
    private Boolean labelVisible = false;
   
	@Override
    public String getNextStepZul() {
        DeployAgentOption deployAgentOption = Utils.getSingleSelectedItem(deployOptionListModelList);

        String nextStepZulPath;
        if (DeployAgentOption.DEPLOY_NOW.equals(deployAgentOption)) {
            nextStepZulPath = AgentServiceAccountStepViewModel.ZUL_PATH;
        } else {
            nextStepZulPath = AddServerInvisibleStepViewModel.ZUL_PATH;
        }
        return nextStepZulPath;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_TIPS);
    }

	@Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+SQLcompliance+Agent+Deployment+window";
    }

    public Boolean getLabelVisible() {
		return labelVisible;
	}

	public void setLabelVisible(Boolean labelVisible) {
		this.labelVisible = labelVisible;
	}

    private ListModelList<DeployAgentOption> deployOptionListModelList;

    public ListModelList<DeployAgentOption> getDeployOptionListModelList() {
        return deployOptionListModelList;
    }

    public void setDeployOptionListModelList(ListModelList<DeployAgentOption> deployOptionListModelList) {
        this.deployOptionListModelList = deployOptionListModelList;
    }

    public enum DeployAgentOption {

        DEPLOY_NOW(0, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_DEPLOY_NOW)),
        DEPLOY_LATER(1, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_DEPLOY_LATER)),
        DEPLOY_MANUAL(2, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_MANUALLY_DEPLOY)),
        DEPLOYED_ALREADY(3, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_ALREADY_DEPLOYED));

        private int id;
        private String label;
        private boolean isDisabled;

        DeployAgentOption(int id, String label) {
            this.id = id;
            this.label = label;
        }

        public int getId() {
            return id;
        }

        public String getLabel() {
            return label;
        }

        public String getName() {
            return this.name();
        }

        public boolean isDisabled() {
            return isDisabled;
        }

        public void setDisabled(boolean isDisabled) {
            this.isDisabled = isDisabled;
        }

        public static DeployAgentOption getById(long id) {
            for(DeployAgentOption e : values()) {
                if(e.id == id) {
                    return e;
                }
            }
            return null;
        }
    }

    public AgentDeploymentStepViewModel() {
        super();
        deployOptionListModelList = new ListModelList();
        deployOptionListModelList.addAll(Arrays.asList(DeployAgentOption.values()));

    }

    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
        try {
            String instanceName = wizardEntity.getAddServerEntity().getInstance();
            AgentDeploymentProperties adp = InstancesFacade.getAgentDeploymentPropertiesForInstance(instanceName);
            isAgentAlreadyDeployed = adp.isDeployed();
            labelVisible = isAgentAlreadyDeployed;
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_DEPLOYMENT_LOAD_AGENT_STATUS_ERROR);
        }

        configureRadioGroup(wizardEntity, isAgentAlreadyDeployed);
        BindUtils.postNotifyChange(null, null, this, "*");
    }

    private void configureRadioGroupForNonDeployedAgent(AddServerWizardEntity wizardEntity) {
        if (deployOptionListModelList.contains(DeployAgentOption.DEPLOYED_ALREADY)) {
            deployOptionListModelList.remove(DeployAgentOption.DEPLOYED_ALREADY);
        }
        DeployAgentOption.DEPLOY_MANUAL.setDisabled(false);
        boolean isServerHostedByCluster = wizardEntity.getAddServerEntity().isVirtualServer();
        if (isServerHostedByCluster) {
            DeployAgentOption.DEPLOY_NOW.setDisabled(true);
            DeployAgentOption.DEPLOY_LATER.setDisabled(true);
            deployOptionListModelList.setSelection(Arrays.asList(DeployAgentOption.DEPLOY_MANUAL));
        } else {
            DeployAgentOption.DEPLOY_NOW.setDisabled(false);
            DeployAgentOption.DEPLOY_LATER.setDisabled(false);
            deployOptionListModelList.setSelection(Arrays.asList(DeployAgentOption.DEPLOY_NOW));
        }
    }

    private void configureRadioGroupForAlreadyDeployedAgent() {
        DeployAgentOption.DEPLOY_NOW.setDisabled(true);
        DeployAgentOption.DEPLOY_LATER.setDisabled(true);
        DeployAgentOption.DEPLOY_MANUAL.setDisabled(true);
        DeployAgentOption.DEPLOYED_ALREADY.setDisabled(false);
        if (!deployOptionListModelList.contains(DeployAgentOption.DEPLOYED_ALREADY)) {
            deployOptionListModelList.add(DeployAgentOption.DEPLOYED_ALREADY);
        }
        deployOptionListModelList.setSelection(Arrays.asList(DeployAgentOption.DEPLOYED_ALREADY));
    }

    private void configureRadioGroup(AddServerWizardEntity wizardEntity, boolean isAgentAlreadyDeployed) {
        if (isAgentAlreadyDeployed) {
            configureRadioGroupForAlreadyDeployedAgent();
        } else {
            configureRadioGroupForNonDeployedAgent(wizardEntity);
        }
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        DeployAgentOption deployAgentOption = Utils.getSingleSelectedItem(deployOptionListModelList);
        getParentWizardViewModel().getWizardEntity().getAddServerEntity().setAgentDeployStatus(deployAgentOption.getId());
    }
}
