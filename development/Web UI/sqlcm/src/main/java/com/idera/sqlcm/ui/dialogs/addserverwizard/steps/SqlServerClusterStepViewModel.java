package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;


import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;

public class SqlServerClusterStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/sql-server-cluster-step.zul";

    private boolean isServerHostedByCluster;

    public boolean isServerHostedByCluster() {
        return isServerHostedByCluster;
    }

    public void setServerHostedByCluster(boolean isServerHostedByCluster) {
        this.isServerHostedByCluster = isServerHostedByCluster;
    }

    @Override
    public String getNextStepZul() {
        return AgentDeploymentStepViewModel.ZUL_PATH;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_SERVER_CLUSTER_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+SQL+Server+Cluster+window";
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardEntity) {
        getParentWizardViewModel().getWizardEntity().getAddServerEntity()
                .setVirtualServer(isServerHostedByCluster());
    }
}
