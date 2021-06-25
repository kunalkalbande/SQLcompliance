package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.commons.lang.StringUtils;
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
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Vlayout;
import org.zkoss.zul.Window;

import com.google.common.base.Function;
import com.google.common.base.Strings;
import com.google.common.collect.Collections2;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMAuditedDatabase;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.database.properties.CMDatabaseProperties;
import com.idera.sqlcm.entities.database.properties.CMStringCMTableEntity;
import com.idera.sqlcm.enumerations.AuditUserTables;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.userColumns.UserColumnsViewModel;
import com.idera.sqlcm.ui.dialogs.userTables.UserTablesForSensitiveColumnsViewModel;
import com.idera.sqlcm.utils.SQLCMConstants;

public class SensitiveColumnsConfigureViewModel {

	public static final String ZUL_URL = "~./sqlcm/dialogs/addserverwizard/steps/sensitive-columns-configure-dialog.zul";

	public static final String INSTANCE_ARG = "instance_arg";

	public static final String DATABASE_ARG = "database_arg";

	public static final String CM_DATABASE_ARG = "cm_database_arg";

	public static final String DATABASE_ID = "database-id";

	public static final String LISTENER_ARG = "listener_arg";

	public static final CMDatabase dbDatabaseAdded = new CMDatabase();

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
			return new CMStringCMTableEntity(cmTable.getFullTableName(),
					cmTable);
		}
	};

	private String help = "http://wiki.idera.com/display/SQLCM/Reduce+audit+data+to+optimize+performance";

	private String optimizePerformanceLink;

	private CMInstance instance;

	private CMAuditedDatabase database;

	private CMDatabaseProperties databaseProperties;

	private String selectionType = "Individual";

	private ListModelList<CMTable> auditedUserTablesListModelList = new ListModelList<>();

	private CMDatabase cmDatabase;

	// Sensitive Columns
	@Wire("#include_sensitiveColumnsTab #sensitiveColumnErrorLabel")
	protected Label sensitiveColumnErrorLabel;

	@Wire("#include_sensitiveColumnsTab #sensitiveColumnMainContent")
	protected Vlayout sensitiveColumnMainContent;

	private ListModelList<CMStringCMTableEntity> sensitiveColumnsTablesListModelList = new ListModelList<>();
	private ListModel<CMStringCMTableEntity> sensitiveColumnsDatasetTablesListModelList = new ListModelList<>();
	private ListModelList<CMStringCMTableEntity> sensitiveColumnsIndividualTablesListModelList = new ListModelList<>();
	private List<CMTable> sensitiveColumnsTablesList = new ArrayList<>();
	private List<CMTable> sensitiveColumnsDatasetTablesList = new ArrayList<>();

	private static final Function<CMStringCMTableEntity, CMTable> TO_CMTABLE_TRANSFORMATION_FOR_SC = new Function<CMStringCMTableEntity, CMTable>() {
		@Override
		public CMTable apply(CMStringCMTableEntity cmStringCMTableEntity) {
			return cmStringCMTableEntity.getDatasetTableList().get(0);
		}
	};

	private SensitiveColumnsAddTablesListener sensitiveColumnsAddTablesListener = new SensitiveColumnsAddTablesListener();

	private SensitiveColumnsEditColumnsListener sensitiveColumnsEditColumnsListener = new SensitiveColumnsEditColumnsListener();

	@Wire("#removeSCTableButton")
	Button removeSCTableButton;

	@Wire("#editSCTableButton")
	Button editSCTableButton;

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

	public ListModelList<CMStringCMTableEntity> getSensitiveColumnsTablesListModelList() {
		return sensitiveColumnsTablesListModelList;
	}

	private CMDatabaseProperties getDatabaseProperties(Long databaseId) {
		CMDatabaseProperties databaseProperties = null;
		try {
			databaseProperties = DatabasesFacade
					.getDatabaseProperties(databaseId);
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_DATABASE_PROPERTIES);
		}
		return databaseProperties;
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
			@ExecutionArgParam(LISTENER_ARG) DialogListener listener) {
		Selectors.wireComponents(view, this, false);
		Selectors.wireEventListeners(view, this);
		optimizePerformanceLink = SQLCMConstants.SERVER_OPTIMIZE_AUDIT_PERFORMANCE;
		if (Executions.getCurrent().getArg().containsKey(INSTANCE_ARG)) {
			HashMap<String, Object> args = (HashMap<String, Object>) Executions
					.getCurrent().getArg();
			if (args.containsKey(DATABASE_ID)) {
				Long databaseId = (Long) args.get(DATABASE_ID);
				databaseProperties = getDatabaseProperties(databaseId);
				database = (CMAuditedDatabase) args.get(DATABASE_ARG);
			}
			instance = (CMInstance) args.get(INSTANCE_ARG);
			cmDatabase = (CMDatabase) args.get(CM_DATABASE_ARG);
			this.listener = listener;
		} else {
			WebUtil.showErrorBoxWithCustomMessage(ELFunctions
					.getMessage(SQLCMI18NStrings.DATABASE_PARAMETER_NOT_PASSED));
			return;
		}
		sensitiveColumnsTablesListModelList.clear();
		if (cmDatabase.getSensitiveColumnTableData() != null
				&& cmDatabase.getSensitiveColumnTableData()
						.getSensitiveTableColumnDictionary() != null
				&& cmDatabase.getSensitiveColumnTableData()
						.getSensitiveTableColumnDictionary().size() > 0) {
			sensitiveColumnsTablesListModelList.addAll(cmDatabase
					.getSensitiveColumnTableData()
					.getSensitiveTableColumnDictionary());

		}
	}

	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@Command
	public void updateAuditDatabaseProperties(@BindingParam("comp") Window x) {
		try {
			if (databaseProperties.getDmlSelectFilters().getAuditUserTables() == AuditUserTables.FOLLOWING
					.getId()
					&& (auditedUserTablesListModelList == null || auditedUserTablesListModelList
							.isEmpty())) {
				WebUtil.showErrorBox(new RestException(),
						SQLCMI18NStrings.ERROR_ONE_TABLE_MUST_BE_SELECTED);
			} else if (!databaseProperties.getDmlSelectFilters()
					.isAuditDmlAll()
					&& databaseProperties.getDmlSelectFilters()
							.getAuditUserTables() == AuditUserTables.NONE
							.getId()
					&& !databaseProperties.getDmlSelectFilters()
							.isAuditDmlOther()
					&& !databaseProperties.getDmlSelectFilters()
							.isAuditStoredProcedures()
					&& !databaseProperties.getDmlSelectFilters()
							.isAuditSystemTables()) {
				WebUtil.showErrorBox(
						new RestException(),
						SQLCMI18NStrings.ERROR_ONE_TYPE_OF_OBJECT_MUST_BE_AUDITED);
			} else {
				databaseProperties.getSensitiveColumnTableData()
						.setSensitiveTableColumnDictionary(
								sensitiveColumnsTablesListModelList);
				String response = DatabasesFacade
						.updateDatabaseProperties(databaseProperties);
				if (!Strings.isNullOrEmpty(response)) {
					WebUtil.showErrorBoxWithCustomMessage(
							response,
							SQLCMI18NStrings.FAILED_TO_UPDATE_DATABASE_PROPERTIES);
				}
				x.detach();
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_UPDATE_DATABASE_PROPERTIES);
		}
	}

	@Command
	public void addSensitiveColumnsTable(
			@BindingParam("selection") String selection) {
		CMDatabase db = new CMDatabase();
		db = cmDatabase;
		sensitiveColumnsTablesList.clear();
		sensitiveColumnsIndividualTablesListModelList.clear();

		for (CMStringCMTableEntity cmStringCMTableEntity : sensitiveColumnsTablesListModelList) {
			if (cmStringCMTableEntity.getType().equals("Individual")) {
				sensitiveColumnsIndividualTablesListModelList
						.add(cmStringCMTableEntity);
			}
		}
		sensitiveColumnsTablesList.addAll(Collections2.transform(
				sensitiveColumnsIndividualTablesListModelList,
				TO_CMTABLE_TRANSFORMATION_FOR_SC));
		UserTablesForSensitiveColumnsViewModel
				.showWithoutTips(
						instance,
						db,
						sensitiveColumnsTablesList,
						sensitiveColumnsAddTablesListener,
						ELFunctions
								.getLabel(SQLCMI18NStrings.USER_TABLES_SENSITIVE_COLUMNS_DIALOG_TITLE));
		selectionType = selection;
	}

	@Command
	public void addDatasetSensitiveColumnsTable(
			@BindingParam("selection") String selection) {
		CMDatabase db = new CMDatabase();
		db = cmDatabase;
		sensitiveColumnsDatasetTablesList.clear();
		sensitiveColumnsTablesList
				.addAll(Collections2.transform(
						sensitiveColumnsTablesListModelList,
						TO_CMTABLE_TRANSFORMATION));
		UserTablesForSensitiveColumnsViewModel
				.showWithoutTips(
						instance,
						db,
						sensitiveColumnsDatasetTablesList,
						sensitiveColumnsAddTablesListener,
						ELFunctions
								.getLabel(SQLCMI18NStrings.USER_TABLES_SENSITIVE_COLUMNS_DIALOG_TITLE));
		selectionType = selection;
	}

	@Command
	@NotifyChange("sensitiveColumnsTablesListModelList")
	public void removeSensitiveColumnsTable() {
		sensitiveColumnsTablesListModelList
				.removeAll(sensitiveColumnsTablesListModelList.getSelection());
		if (sensitiveColumnsTablesListModelList == null
				|| sensitiveColumnsTablesListModelList.getSelection().isEmpty()) {
			removeSCTableButton.setDisabled(true);
			editSCTableButton.setDisabled(true);
		}
	}

	@Command
	@NotifyChange("sensitiveColumnsTablesListModelList")
	public void editSensitiveColumnsTable() {
		if (sensitiveColumnsTablesListModelList.getSelection().iterator()
				.hasNext()) {
			CMStringCMTableEntity entityToEdit = sensitiveColumnsTablesListModelList
					.getSelection().iterator().next();
			List<CMTable> tables = entityToEdit.getDatasetTableList();
			try {
				List<String> columns = new ArrayList();
				String entityName = "";
				for (CMTable table : tables) {
					if (!table.isSelectedColumns()
							|| table.getColumnList() == null
							|| table.getColumnList().isEmpty()) {
						columns.addAll(DatabasesFacade
								.getNewDatabaseColumnList(
										table.getFullTableName(),
					instance.getName(),
										cmDatabase.getName()));
						entityName += table.getFullTableName();
						if (!entityToEdit.getType().equals("Individual")) {
							for (int i = 0; i < columns.size(); i++) {
								columns.set(i, table.getFullTableName() + "."
										+ columns.get(i));
							}
						}
					} else {
						columns.addAll(table.getColumnList());
						entityName += table.getFullTableName() + ",";
					}
				}
				if (entityName.endsWith(",")) {
					entityName = entityName.substring(0,
							entityName.length() - 1);
				}
				sensitiveColumnsEditColumnsListener.instanceName = instance
			.getName();
				sensitiveColumnsEditColumnsListener.databaseName = cmDatabase
						.getName();
				if (entityToEdit.getType().equals("Individual")) {
					if (tables.get(0).isSelectedColumns()) {
						tables.get(0).setColumnList(columns);
					}
					UserColumnsViewModel.showForSensitiveColumns(0L,
							tables.get(0), columns,
							sensitiveColumnsEditColumnsListener, true,
			    instance.getName(), cmDatabase.getName());
				} else {
					CMTable tempTableForEdit = new CMTable();
					tempTableForEdit.setcolumnId(tables.get(0).getcolumnId());
					tempTableForEdit.setDatabaseId(tables.get(0)
							.getDatabaseId());
					tempTableForEdit.setColumnList(columns);
					tempTableForEdit.setSelectedColumns(true);
					tempTableForEdit.setFullTableName(entityName);
					tempTableForEdit.setType(entityToEdit.getType());
					UserColumnsViewModel.showForSensitiveColumns(0L,
							tempTableForEdit, columns,
							sensitiveColumnsEditColumnsListener, true,
			    instance.getName(), cmDatabase.getName());
				}
			} catch (RestException e) {
				WebUtil.showErrorBox(e,
						SQLCMI18NStrings.FAILED_TO_LOAD_COLUMN_LIST);
			}
		}
	}

	@Command
	@NotifyChange({ "removeSCTableButton", "editSCTableButton" })
	public void enableRemoveEditSCTables() {
		removeSCTableButton.setDisabled(false);
		editSCTableButton.setDisabled(false);
	}

	private class SensitiveColumnsAddTablesListener implements
			UserTablesForSensitiveColumnsViewModel.DialogListener {

		@Override
		public void onCloseUserTablesForSensitiveColumnsDialog(
				CMInstance instance, CMDatabase database,
				List<CMTable> selectedTables) {
			List<String> columns = new ArrayList<>();
			List<String> columnsAll = new ArrayList<>();
			if (selectionType.equals("Dataset")
					&& (selectedTables == null || selectedTables.size() == 0))
				return;
			for (CMTable cm : selectedTables) {
				cm.setType(selectionType);
			}

			if (selectionType.equals("Dataset")) {
				CMStringCMTableEntity cmStringCMTableEntity = new CMStringCMTableEntity();

				// Fetch dataset ID
				long datasetID = 1;
				for (CMStringCMTableEntity currentEntity : sensitiveColumnsTablesListModelList) {
					if (currentEntity.getDatasetTableList().get(0)
							.getcolumnId() >= datasetID) {
						datasetID = currentEntity.getDatasetTableList().get(0)
								.getcolumnId() + 1;
					}
				}

				try {
					List<String> tableNames = new ArrayList<>();
					for (CMTable table : selectedTables) {
						columns = DatabasesFacade.getNewDatabaseColumnList(
				table.getFullTableName(), instance.getName(),
				database.getName());
						for (int i = 0; i < columns.size(); i++) {
							columns.set(i, table.getFullTableName() + "."
									+ columns.get(i));
						}
						table.setColumnList(columns);
						table.setSelectedColumns(true);
						table.setcolumnId(datasetID);
						columnsAll.addAll(columns);
						tableNames.add(table.getFullTableName());
					}

					cmStringCMTableEntity.setDatasetTableList(selectedTables);

					cmStringCMTableEntity.setKey(StringUtils.join(tableNames,
							","));

					cmStringCMTableEntity.setType("Dataset");

					sensitiveColumnsTablesListModelList
							.add(cmStringCMTableEntity);
				} catch (RestException e) {
					WebUtil.showErrorBox(e,
							SQLCMI18NStrings.FAILED_TO_LOAD_COLUMN_LIST);
				}
			} else {
				List<CMStringCMTableEntity> tempList = new ArrayList();
				for (CMStringCMTableEntity entity : sensitiveColumnsTablesListModelList) {
					if (!entity.getType().equals("Individual")) {
						tempList.add(entity);
					}
				}
				sensitiveColumnsTablesListModelList.clear();
				sensitiveColumnsTablesListModelList.addAll(tempList);
				for (CMTable table : selectedTables) {
					CMStringCMTableEntity cmStringCMTableEntity = new CMStringCMTableEntity();
					List<CMTable> listCMTable = new ArrayList<CMTable>();
					listCMTable.add(table);
					cmStringCMTableEntity.setKey(table.getFullTableName());
					cmStringCMTableEntity.setDatasetTableList(listCMTable);
					cmStringCMTableEntity.setType("Individual");
					sensitiveColumnsTablesListModelList
							.add(cmStringCMTableEntity);
				}
			}

			BindUtils.postNotifyChange(null, null,
					SensitiveColumnsConfigureViewModel.this,
					"sensitiveColumnsTablesListModelList");
		}
	}

	private class SensitiveColumnsEditColumnsListener implements
			UserColumnsViewModel.DialogListener {
		String instanceName;
		String databaseName;

		@Override
		public void onCloseUserColumnsDialog(Long databaseId, CMTable table) {
			for (CMStringCMTableEntity cmStringCMTableEntity : sensitiveColumnsTablesListModelList) {
				if (cmStringCMTableEntity.getKey().equals(
						table.getFullTableName())
						&& cmStringCMTableEntity.getDatasetTableList().get(0)
								.getcolumnId() == table.getcolumnId()) {
					for (CMTable tableToUpdate : cmStringCMTableEntity
							.getDatasetTableList()) {
						try {
							if (!table.isSelectedColumns()) {
								tableToUpdate.setColumnList(new ArrayList());
							} else if (table.getColumnList() != null
									|| !table.getColumnList().isEmpty()) {
								List<String> tableColumns = DatabasesFacade
										.getNewDatabaseColumnList(tableToUpdate
												.getFullTableName(),
												instanceName, databaseName);
								if (tableToUpdate.getType().equals("Dataset")) {
									for (int i = 0; i < tableColumns.size(); i++) {
										tableColumns.set(
												i,
												tableToUpdate
														.getFullTableName()
														+ "."
														+ tableColumns.get(i));
									}
								}
								List<String> matchedColumns = new ArrayList();
								for (String columnName : tableColumns) {
									if (table.getColumnList().contains(
											columnName)) {
										matchedColumns.add(columnName);
									}
								}
								tableToUpdate.setColumnList(matchedColumns);
							}
						} catch (RestException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
					}
					BindUtils.postNotifyChange(null, null,
							SensitiveColumnsConfigureViewModel.this,
							"sensitiveColumnsTablesListModelList");
					return;
				}
			}
		}
	}

	public static void showDialog(CMInstance instance,
			CMAuditedDatabase database, CMDatabase db, DialogListener listener) {
		Map<String, Object> args = new HashMap<>();
		if (instance != null)
			args.put(INSTANCE_ARG, instance);
		if (database != null) {
			args.put(DATABASE_ARG, database);
			args.put(DATABASE_ID, database.getId());
		}
		args.put(CM_DATABASE_ARG, db);
		args.put(SensitiveColumnsConfigureViewModel.LISTENER_ARG, listener);
		Window window = (Window) Executions.createComponents(
				SensitiveColumnsConfigureViewModel.ZUL_URL, null, args);
		window.doHighlighted();
	}

	public interface DialogListener {
		void onCloseSensitiveColumnsConfigureDialog(
				ListModelList<CMStringCMTableEntity> sensitiveColumnsTablesListModelList);
	}

	@Command
	public void okCommand(@BindingParam("comp") Window x) {
		if (listener != null) {
			listener.onCloseSensitiveColumnsConfigureDialog(sensitiveColumnsTablesListModelList);
		}
		x.detach();
	}
}