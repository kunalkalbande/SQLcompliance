package com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;

import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.ListModelList;

import java.util.ArrayList;
import java.util.List;

public class SelectDatabasesStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/adddatabasewizard/steps/select-databases-step.zul";

    private ListModelList<CMDatabase> databaseList;

    public SelectDatabasesStepViewModel() {
        super();
    }

    public ListModelList<CMDatabase> getDatabaseList() {
        return databaseList;
    }

    @Override
    public boolean isFirst() {
        return true;
    }

    @Override
    public void onDoAfterWire() {
        long instanceId = getInstance().getId();
        try {
            List<CMDatabase> dbList = DatabasesFacade.getDatabaseList(instanceId);
            databaseList = new ListModelList<>(dbList);
            databaseList.setMultiple(true);
        } catch (RestException e) {
            getParentWizardViewModel().close();
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_DATABASE_LIST);
        }

        getNextButton().setDisabled(true);
    }

    @Override
    public String getNextStepZul() {
        return AuditCollectionLevelStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_SELECT_DATABASES_TIPS);
    }

    @Command("selectClick")
    public void selectClick() {
        disableNextButtonIfNoSelectedDatabases();
    }

    private void disableNextButtonIfNoSelectedDatabases() {
        boolean disableNextButton = true;
        if (databaseList.getSelection().size() > 0) {
            disableNextButton = false;
        }
        getNextButton().setDisabled(disableNextButton);
    }

    @Command("selectAllClick")
    public void selectAllClick() {
        databaseList.setSelection(databaseList);
        getNextButton().setDisabled(false);
    }

    @Command("unSelectAllClick")
    public void unSelectAllClick() {
        databaseList.clearSelection();
        getNextButton().setDisabled(true);
    }

    @Override
    protected void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) {
        if (databaseList.size() == 0) {
            getParentWizardViewModel().close();
            WebUtil.showInfoBox(SQLCMI18NStrings.ADD_DATABASE_WIZARD_ALL_ALREADY_AUDITED);
            return;
        }

        disableNextButtonIfNoSelectedDatabases();
    }

    @Override
    public void onBeforeNext(AddDatabasesSaveEntity wizardSaveEntity) {
        wizardSaveEntity.setDatabaseList(new ArrayList(databaseList.getSelection()));
    }

	@Override
	public String getHelpUrl() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public boolean onBeforeCancel(AddDatabasesSaveEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		return false;
	}
}