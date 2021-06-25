package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.ExecutionArgParam;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.ListModelSet;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Vlayout;
import org.zkoss.zul.Window;

import com.google.common.base.Function;
import com.google.common.collect.Collections2;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.database.properties.CMCLRStatus;
import com.idera.sqlcm.entities.database.properties.CMDatabaseProperties;
import com.idera.sqlcm.entities.database.properties.CMStringCMTableEntity;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.userColumns.UserColumnsViewModel;
import com.idera.sqlcm.ui.dialogs.userTables.UserTablesForSensitiveColumnsViewModel;
import com.idera.sqlcm.utils.SQLCMConstants;

public class BeforeAfterConfigureViewModel {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/before-after-configure-dialog.zul";

	public static final String INSTANCE_ARG = "instance_arg";

	public static final String DATABASE_ARG = "database_arg";

	public static final String DATABASE_ID = "database-id";

	public static final String LISTENER_ARG = "listener_arg";
	
	public static final String BEFOREAFTER_ENABLE_ARG = "beforeafter_enble_arg";

	private DialogListener listener;

	public static final String TRUSTED_USERS_TELL_ME_MORE_URL = "http://wiki.idera.com/display/SQLCM/Audited+Database+Properties+window+-+Trusted+Users+tab";

	private static final Function<CMStringCMTableEntity, CMTable> TO_CMTABLE_TRANSFORMATION = new Function<CMStringCMTableEntity, CMTable>() {
		@Override
		public CMTable apply(CMStringCMTableEntity cmStringCMTableEntity) {
			return cmStringCMTableEntity.getValue();
		}
	};

	private static final Function<CMTable, CMStringCMTableEntity> FROM_CMTABLE_TRANSFORMATION = new Function<CMTable, CMStringCMTableEntity>() {
		@Override
		public CMStringCMTableEntity apply(CMTable cmTable) {
			return new CMStringCMTableEntity(cmTable.getFullTableName(), cmTable);
		}
	};

	private String help = "http://wiki.idera.com/display/SQLCM/Reduce+audit+data+to+optimize+performance";

	private String optimizePerformanceLink;

	private CMInstance instance;

	private CMDatabase database;

	private CMDatabaseProperties databaseProperties;
	
	private CMCLRStatus cmCLRStatus;
	
	private boolean dmlEnabled;

	public boolean isDmlEnabled() {
		return dmlEnabled;
	}

	public void setDmlEnabled(boolean dmlEnabled) {
		this.dmlEnabled = dmlEnabled;
	}

	public CMCLRStatus getCmCLRStatus() {
		return cmCLRStatus;
	}

	public void setCmCLRStatus(CMCLRStatus cmCLRStatus) {
		this.cmCLRStatus = cmCLRStatus;
	}

	private ListModelList<CMTable> auditedUserTablesListModelList = new ListModelList<>();

	// Before-After Data
	@Wire("#beforeAfterDataErrorLabel")
	protected Label beforeAfterDataErrorLabel;

	@Wire("#beforeAfterDataMainContent")
	protected Vlayout beforeAfterDataMainContent;

	private boolean disableBeforeAfterData;

	private ListModelList<CMStringCMTableEntity> beforeAfterDataTablesListModelList = new ListModelList<>();

	private List<CMTable> beforeAfterDataTablesList = new ArrayList<>();

	private BeforeAfterDataAddTablesListener beforeAfterDataAddTablesListener = new BeforeAfterDataAddTablesListener();

	private BeforeAfterDataEditColumnsListener beforeAfterDataEditColumnsListener = new BeforeAfterDataEditColumnsListener();

	// TrustedUsers
	private ListModelSet<CMInstancePermissionBase> trustedUserListModelSet = new ListModelSet<>();

	@Wire("#include_dmlSelectFiltersTab #removeUserTablesAuditingButton")
	Button removeUserTablesAuditingButton;

	@Wire("#include_dmlSelectFiltersTab #userTablesRG")
	Radiogroup userTablesRG;

	@Wire("#removeBADTableButton")
	Button removeBADTableButton;

	@Wire("#editBADTableButton")
	Button editBADTableButton;

	public String getHelp() {
		return this.help;
	}

	public String getOptimizePerformanceLink() {
		return optimizePerformanceLink;
	}

	public CMDatabaseProperties getDatabaseProperties() {
		return databaseProperties;
	}

	public ListModelList<CMTable> getAuditedUserTablesListModelList() {
		return auditedUserTablesListModelList;
	}

	public ListModelList<CMStringCMTableEntity> getBeforeAfterDataTablesListModelList() {
		return beforeAfterDataTablesListModelList;
	}

	public boolean isEnableBeforeAfterData() {
		return true;
	}

	private CMDatabaseProperties getDatabaseProperties(Long databaseId) {
		CMDatabaseProperties databaseProperties = null;
		try {
			databaseProperties = DatabasesFacade.getDatabaseProperties(databaseId);
		} catch (RestException e) {
			WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_DATABASE_PROPERTIES);
		}
		return databaseProperties;
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
			@ExecutionArgParam(LISTENER_ARG) DialogListener listener) {
		Selectors.wireComponents(view, this, false);
		Selectors.wireEventListeners(view, this);
		optimizePerformanceLink = SQLCMConstants.SERVER_OPTIMIZE_AUDIT_PERFORMANCE;
		if (Executions.getCurrent().getArg().containsKey(DATABASE_ID)) {
			HashMap<String, Object> args = (HashMap<String, Object>) Executions.getCurrent().getArg();
			args.get(DATABASE_ID);
			database = (CMDatabase) args.get(DATABASE_ARG);
			instance = (CMInstance) args.get(INSTANCE_ARG);
			dmlEnabled = (boolean) args.get(BEFOREAFTER_ENABLE_ARG);
			this.listener = listener;
		} else {
			WebUtil.showErrorBoxWithCustomMessage(
					ELFunctions.getMessage(SQLCMI18NStrings.DATABASE_PARAMETER_NOT_PASSED));
			return;
		}
		if(database.getBeforeAfterTableData() != null 
    			&& database.getBeforeAfterTableData().size() > 0){
    		List<CMStringCMTableEntity> cmStringCMTableEntities = new ArrayList<CMStringCMTableEntity>();
    		for(CMTable cmTable : database.getBeforeAfterTableData()){
    			CMStringCMTableEntity cmStringCMTableEntity = new CMStringCMTableEntity();
    			cmStringCMTableEntity.setValue(cmTable);
    			cmStringCMTableEntities.add(cmStringCMTableEntity);
    		}
    		beforeAfterDataTablesListModelList.addAll(cmStringCMTableEntities);
    	}
		try {
			cmCLRStatus = SQLCMRestClient.getInstance().getServerCLRStatus(
					instance.getId());
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_ENABLE_CLR_STATUS);
		}
		BindUtils.postNotifyChange(null, null, BeforeAfterConfigureViewModel.this,"cmCLRStatus");
		// before-after data tab
		disableBeforeAfterData = !dmlEnabled;
	}

	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@Command
	public void addBeforeAfterDataTable() {
		beforeAfterDataTablesList.clear();
		beforeAfterDataTablesList
				.addAll(Collections2.transform(beforeAfterDataTablesListModelList, TO_CMTABLE_TRANSFORMATION));
		List<CMTable> auditedUserTablesListFilter = null;
		UserTablesForSensitiveColumnsViewModel.showWithoutTips(instance, database, database.getBeforeAfterTableData(),
				auditedUserTablesListFilter, beforeAfterDataAddTablesListener,
				ELFunctions.getLabel(SQLCMI18NStrings.USER_TABLES_FOR_BEFORE_AFTER_DATA_DIALOG_TITLE));
	}

	@Command
	@NotifyChange("removeUserTablesAuditingButton")
	public void enableRemoveUserTablesAuditingButton() {
		removeUserTablesAuditingButton.setDisabled(false);
	}

	@Command
	@NotifyChange("auditedUserTablesListModelList")
	public void removeDMLSelectTable() {
		auditedUserTablesListModelList.removeAll(auditedUserTablesListModelList.getSelection());
		if (auditedUserTablesListModelList == null || auditedUserTablesListModelList.getSelection().isEmpty()) {
			removeUserTablesAuditingButton.setDisabled(true);
		}
	}

	@Command
	@NotifyChange("beforeAfterDataTablesListModelList")
	public void removeBeforeAfterDataTable() {
		beforeAfterDataTablesListModelList.removeAll(beforeAfterDataTablesListModelList.getSelection());
		if (beforeAfterDataTablesListModelList == null || beforeAfterDataTablesListModelList.getSelection().isEmpty()) {
			removeBADTableButton.setDisabled(true);
			editBADTableButton.setDisabled(true);
		}
	}

	@Command
	@NotifyChange("beforeAfterDataTablesListModelList")
	public void editBeforeAfterDataTable() {
		if (beforeAfterDataTablesListModelList.getSelection().iterator().hasNext()) {
			CMTable table = beforeAfterDataTablesListModelList.getSelection().iterator().next().getValue();
			try {
				List<String> columns;
				if (table.getColumnList() == null || table.getColumnList().isEmpty()) {
					columns = DatabasesFacade.getNewDatabaseColumnList(table.getFullTableName(),instance.getName(), database.getName());
				} else {
					columns = table.getColumnList();
				}
				UserColumnsViewModel.showForNewBeforeAfterData(database.getId(), table, columns,
						beforeAfterDataEditColumnsListener,instance.getName(),database.getName());
			} catch (RestException e) {
				WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_COLUMN_LIST);
			}
		}
	}

	@Command
	@NotifyChange({ "removeBADTableButton", "editBADTableButton" })
	public void enableRemoveEditBADTables() {
		removeBADTableButton.setDisabled(false);
		editBADTableButton.setDisabled(false);
	}

	@Command("onAuditCaptureSQLCheck")
	public void onAuditCaptureSQLCheck(@BindingParam("checked") boolean checked) {
		if (checked) {
			WebUtil.showWarningWithCustomMessage(
					ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE));
		}
	}

	public boolean isDisableBeforeAfterData() {
		return disableBeforeAfterData;
	}

	private class BeforeAfterDataAddTablesListener implements UserTablesForSensitiveColumnsViewModel.DialogListener {

		@Override
		public void onCloseUserTablesForSensitiveColumnsDialog(CMInstance instance, CMDatabase database,
				List<CMTable> selectedTables) {
			beforeAfterDataTablesListModelList.clear();
			beforeAfterDataTablesListModelList
					.addAll(Collections2.transform(selectedTables, FROM_CMTABLE_TRANSFORMATION));
			BindUtils.postNotifyChange(null, null, BeforeAfterConfigureViewModel.this,
					"beforeAfterDataTablesListModelList");
		}
	}

	private class BeforeAfterDataEditColumnsListener implements UserColumnsViewModel.DialogListener {

		@Override
		public void onCloseUserColumnsDialog(Long databaseId, CMTable table) {
			for (CMStringCMTableEntity cmStringCMTableEntity : beforeAfterDataTablesListModelList) {
				if (cmStringCMTableEntity.getValue().getFullTableName().equals(table.getFullTableName())) {
					cmStringCMTableEntity.getValue().setRowLimit(table.getRowLimit());
					cmStringCMTableEntity.getValue().setColumnList(table.getColumnList());
					BindUtils.postNotifyChange(null, null, BeforeAfterConfigureViewModel.this,
							"beforeAfterDataTablesListModelList");
					return;
				}
			}
		}
	}

	public static void showDialog(CMInstance instance, CMDatabase database, DialogListener listener,boolean dmlCheck) {
		Map<String, Object> args = new HashMap<>();
		args.put(BeforeAfterConfigureViewModel.INSTANCE_ARG, instance);
		args.put(BeforeAfterConfigureViewModel.DATABASE_ARG, database);
		args.put(BeforeAfterConfigureViewModel.DATABASE_ID, database.getId());
		args.put(BeforeAfterConfigureViewModel.LISTENER_ARG, listener);
		args.put(BeforeAfterConfigureViewModel.BEFOREAFTER_ENABLE_ARG, dmlCheck);
		Window window = (Window) Executions.createComponents(BeforeAfterConfigureViewModel.ZUL_PATH, null, args);
		window.doHighlighted();
	}

	public interface DialogListener {
		void onCloseBeforeAfterConfigureDialog(ListModelList<CMStringCMTableEntity> beforeAfterDataTablesListModelList);
	}

	@Command
	public void okCommand(@BindingParam("comp") Window x) {
		if (listener != null) {
			listener.onCloseBeforeAfterConfigureDialog(beforeAfterDataTablesListModelList);
		}
		x.detach();
	}
	
	@Command
    public void enableCLR() {
		cmCLRStatus.setEnable(true);
        try {
            cmCLRStatus = 
                SQLCMRestClient.getInstance().enableCLR(cmCLRStatus);
            BindUtils.postNotifyChange(null, null, this, "cmCLRStatus");
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_ENABLE_CLR_STATUS);
        }
    }
}
