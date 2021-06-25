package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addserverwizard.WizardMode;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zul.ListModelList;

import java.util.ArrayList;
import java.util.List;

import javax.mail.Session;

public class SelectDatabasesStepViewModel extends AddWizardStepBase {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/select-databases-step.zul";

	public static final String PROPERTY_NAME_DATABASE_LIST = "databaseList";

	private ListModelList<CMDatabase> databaseList;

	private boolean auditDatabasesCheckboxChecked = true;

	public boolean isDatabaseListEnabled() {
		return isAddDatabasesOnlyMode() || auditDatabasesCheckboxChecked;
	}

	public SelectDatabasesStepViewModel() {
		super();
	}

	public ListModelList<CMDatabase> getDatabaseList() {
		return databaseList;
	}

	public boolean isAuditDatabasesCheckboxChecked() {
		return auditDatabasesCheckboxChecked;
	}

	public void setAuditDatabasesCheckboxChecked(boolean auditDatabasesCheckboxChecked) {
		this.auditDatabasesCheckboxChecked = auditDatabasesCheckboxChecked;
	}

	public boolean isAuditDatabasesCheckboxVisible() {
		return !isAddDatabasesOnlyMode();
	}

	public boolean isAddDatabasesOnlyMode() {
		return getParentWizardViewModel().isAddDatabasesOnlyMode();
	}

	@Override
	public boolean isFirst() {
		return isAddDatabasesOnlyMode();
	}

	@Override
	public void onDoAfterWire() {
		if (isAddDatabasesOnlyMode()) {
			loadDatabaseList();
		}
	}

	private void loadDatabaseList() {

		if (databaseList != null && databaseList.size() > 0) {
			return; // already loaded
		}

		if (getInstance() == null) {
			throw new RuntimeException(" Could not load databases because instance is null! ");
		}

		long instanceId = getInstance().getId();
		try {
			List<CMDatabase> dbList = DatabasesFacade.getDatabaseList(instanceId);
			databaseList = new ListModelList<>(dbList);
			databaseList.setMultiple(true);
			BindUtils.postNotifyChange(null, null, this, "databaseList");
		} catch (RestException e) {
			getParentWizardViewModel().close();
			WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_DATABASE_LIST);
		}
	}

	private void reLoadDatabaseList() {
		databaseList.clear();
		loadDatabaseList();
	}

	@Override
	public String getNextStepZul() {
		return AlwaysOnAvailabilityGroupStepViewModel.ZUL_PATH;
	}

	@Override
	public String getTips() {
		return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_SELECT_DATABASES_TIPS);
	}

	@Override
	public String getHelpUrl() {
		return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Add+Databases+window";
	}

	@Command("selectClick")
	public void selectClick() {
		disableEnableNextButton();
	}

	private void disableEnableNextButton() {
		boolean disableNextButton = true;
		if ((isAuditDatabasesCheckboxVisible() && !auditDatabasesCheckboxChecked)
				|| databaseList.getSelection().size() > 0) {
			disableNextButton = false;
		}
		getNextButton().setDisabled(disableNextButton);
	}

	@Command("selectAllClick")
	@NotifyChange(PROPERTY_NAME_DATABASE_LIST)
	public void selectAllClick() {
		databaseList.setSelection(databaseList);
		getNextButton().setDisabled(false);
	}

	@Command("unSelectAllClick")
	@NotifyChange(PROPERTY_NAME_DATABASE_LIST)
	public void unSelectAllClick() {
		databaseList.clearSelection();
		getNextButton().setDisabled(true);
	}

	@Command("onAuditDatabasesCheck")
	public void onAuditDatabasesCheck() {
		disableEnableNextButton();
		BindUtils.postNotifyChange(null, null, this, "databaseListEnabled");
	}

	@Override
	protected void doOnShow(AddServerWizardEntity wizardEntity) {
		loadDatabaseList();
		WizardMode wizardMode = getParentWizardViewModel().getWizardMode();
		if (WizardMode.ADD_DATABASE_ONLY.equals(wizardMode) && databaseList != null && databaseList.size() == 0) {
			getParentWizardViewModel().close();
			WebUtil.showInfoBox(SQLCMI18NStrings.ADD_SERVER_WIZARD_ALL_ALREADY_AUDITED);
			return;
		}
		disableEnableNextButton();

		getParentWizardViewModel().getPrevButton().setDisabled(true);
	}

	@Override
	public void onBeforeNext(AddServerWizardEntity wizardEntity) {
		List<CMDatabase> dbList = new ArrayList();
		if (databaseList != null) {
			dbList.addAll(databaseList.getSelection());
		}
		wizardEntity.getServerConfigEntity().setDatabaseList(dbList);

		wizardEntity.setIsAuditDatabasesFlag(isDatabaseListEnabled());
	}

}