package com.idera.sqlcm.ui.administration;

import com.idera.sqlcm.ui.dialogs.ImportSqlServersViewModel;
import com.idera.sqlcm.ui.dialogs.ManageConfigRefreshViewModel;
import com.idera.sqlcm.ui.dialogs.ManageLicenseViewModel;
import com.idera.sqlcm.ui.dialogs.ManageSqlServersInstancesViewModel;
import com.idera.sqlcm.ui.dialogs.ManageUsersViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.ServerWizardViewModel;
import com.idera.sqlcm.wizard.AbstractWizardViewModel;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.select.Selectors;

public class AdministrationViewModel  implements AbstractWizardViewModel.WizardListener {

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
    }

    @Command
    public void showManageUsersDialog() {
        ManageUsersViewModel.showManageUsersDialog();
    }

    @Command
    public void showManageLicenseDialog() {
        ManageLicenseViewModel.showManageLicenseDialog();
    }

    @Command
    public void showImportSQLServersDialog() {
        ImportSqlServersViewModel.showImportSQLServersDialog();
    }

    @Command
    public void showManageSQLServerInstancesDialog() {
        ManageSqlServersInstancesViewModel.showManageSqlServersInstancesDialog();
    }

    @Command
    public void openAddInstanceDialog() {
        ServerWizardViewModel.showAddInstanceWizard(this);
    }

    @Override
    public void onCancel() {
        // do nothing
    }

    @Override
    public void onFinish() {
        // do nothing
    }
    
    @Command
    public void showManageConfigRefresh()
    {
    	ManageConfigRefreshViewModel.showManageConfigRefresh();
    }
}
