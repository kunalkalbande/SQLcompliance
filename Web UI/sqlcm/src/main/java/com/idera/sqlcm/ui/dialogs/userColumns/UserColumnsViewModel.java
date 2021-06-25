package com.idera.sqlcm.ui.dialogs.userColumns;

import java.util.Arrays;
import java.util.HashMap;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.ExecutionArgParam;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Window;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.enumerations.NumbersOfRows;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.preferences.CommonGridPreferencesBean;
import com.idera.sqlcm.utils.SQLCMConstants;

public class UserColumnsViewModel {

    public static final String ZUL_URL = "~./sqlcm/dialogs/userColumns/user-columns-dialog.zul";

    private static final Logger logger = Logger.getLogger(UserColumnsViewModel.class);

    public static final String DATABASE_ARG = "database_arg";
    public static final String TABLE_ARG = "table_arg";
    public static final String SELECTED_COLUMNS_ARG = "selected_columns_arg";
    public static final String LISTENER_ARG = "listener_arg";

    public static final String MSG_MUST_NOT_BE_NULL = " must not be null! ";

    public interface DialogListener {
        void onCloseUserColumnsDialog(Long databaseId, CMTable table);
    }

    protected String preferencesSessionVariableName = CommonGridPreferencesBean.SESSION_VARIABLE_NAME;
    protected int rowsCount = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

    private int prevPageSize;
    private DialogListener listener;

    @Wire
    private Window userColumnsWindow;
    @Wire
    private Label titleLabel;

    private boolean disableColumnSelect;
    private boolean showRowSelector;
    private ListModelList<String> candidateListModelList;
    private ListModelList<String> chooseListModelList;
    private ListModelList<NumbersOfRows> rowsNumberListModelList;
    private Long currentDatabaseId;
    private CMTable currentTable;
    private List<String> currentSelectedColumns;
    private List<String> tableColumns;
	private static boolean isRegulationCall = false;
	private static String mInstanceName;
	private static String mDatabaseName;
    private boolean dataset;

    public boolean isDataset() {
	return dataset;
    }

    public void setDataset(boolean dataset) {
	this.dataset = dataset;
    }

    public UserColumnsViewModel() {
        candidateListModelList = new ListModelList<>();
        chooseListModelList = new ListModelList<>();
        candidateListModelList.setMultiple(true);
        chooseListModelList.setMultiple(true);
        rowsNumberListModelList = new ListModelList<>();
        rowsNumberListModelList.addAll(Arrays.asList(NumbersOfRows.values()));
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
                             @ExecutionArgParam(DATABASE_ARG) Long databaseId,
                             @ExecutionArgParam(TABLE_ARG) CMTable table,
                             @ExecutionArgParam(SELECTED_COLUMNS_ARG) List<String> selectedColumns,
                             @ExecutionArgParam(LISTENER_ARG) DialogListener listener) {
        Selectors.wireComponents(view, this, false);
        if (databaseId == null) {
            throw new RuntimeException(DATABASE_ARG + MSG_MUST_NOT_BE_NULL);
        }
        currentDatabaseId = databaseId;
        if (table == null) {
            throw new RuntimeException(TABLE_ARG + MSG_MUST_NOT_BE_NULL);
        }

        currentTable = table;
        if (selectedColumns == null) {
            throw new RuntimeException(SELECTED_COLUMNS_ARG + MSG_MUST_NOT_BE_NULL);
        }
        currentSelectedColumns = selectedColumns;
        if (currentSelectedColumns != null) {
            chooseListModelList.clear();
            chooseListModelList.addAll(currentSelectedColumns);
        }
        this.listener = listener;
        disableColumnSelect = !currentTable.isSelectedColumns();
        BindUtils.postNotifyChange(null, null, this, "disableColumnSelect");
        rowsNumberListModelList.setSelection(Arrays.asList(NumbersOfRows.getByValue(currentTable.getRowLimit())));
        loadColumns(currentDatabaseId, currentTable);
    }

    private void loadColumns(Long databaseId, CMTable table) {
        try {
            candidateListModelList.clear();
			if (table.getType() != null
					&& !table.getType().equalsIgnoreCase("Individual")) {
	        	for(String tableName : table.getFullTableName().split(",")){
					if (isRegulationCall) {
						tableColumns = DatabasesFacade
								.getNewDatabaseColumnList(tableName,
										mInstanceName, mDatabaseName);
					} else {
						tableColumns = DatabasesFacade.getColumnList(
								databaseId, tableName);
					}
		            if(tableColumns == null || tableColumns.size() == 0)
		            	continue;
	            	for(int i=0;i <tableColumns.size();i++){
						tableColumns.set(i,
								tableName + "." + tableColumns.get(i));
	            	}
		            candidateListModelList.addAll(tableColumns);
	        	}
		dataset = true;
			} else {
				if (isRegulationCall) {
					tableColumns = DatabasesFacade.getNewDatabaseColumnList(
							table.getFullTableName(), mInstanceName,
							mDatabaseName);
				} else {
					tableColumns = DatabasesFacade.getColumnList(databaseId,
							table.getFullTableName());
        	}
	            candidateListModelList.addAll(tableColumns);
            }
            candidateListModelList.removeAll(chooseListModelList);
            BindUtils.postNotifyChange(null, null, this, "candidateListModelList");
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_COLUMN_LIST);
        }
    }

    @Command
    public void okCommand() {
	if (currentTable.isSelectedColumns() && chooseListModelList.isEmpty()) {
	    WebUtil.showErrorBox(SQLCMI18NStrings.USER_COLUMNS_DIALOG_SELECTED_COLUMNS_ERROR);
		} else if (currentTable.getType() != null
				&& currentTable.getType().equals("Dataset")) {
			int matchCount = 0;
			for (String tableName : currentTable.getFullTableName().split(",")) {
				for (String columnName : chooseListModelList) {
					if (columnName.contains(tableName + ".")) {
						matchCount++;
						break;
					}
				}
			}
			if (matchCount < currentTable.getFullTableName().split(",").length) {
				WebUtil.showErrorBox(SQLCMI18NStrings.USER_COLUMNS_DIALOG_SELECTED_COLUMNS_ERROR_DATASET);
			} else {
				userColumnsWindow.detach();
				if (!currentTable.isSelectedColumns()) {
					currentTable.setColumnList(tableColumns);
				} else {
					currentTable.setColumnList(chooseListModelList);
				}
				if (listener != null) {
					listener.onCloseUserColumnsDialog(currentDatabaseId,
							currentTable);
				}
			}
	} else {
	    userColumnsWindow.detach();
	    if (!currentTable.isSelectedColumns()) {
		currentTable.setColumnList(null);
	    } else {
		currentTable.setColumnList(chooseListModelList);
	    }
	    if (listener != null) {
		listener.onCloseUserColumnsDialog(currentDatabaseId,
			currentTable);
	    }
	}
    }

    @Command
    public void closeCommand() {
        userColumnsWindow.detach();
    }

    @Command("addCommand")
    public void addCommand() {
        Set<String> selection = new LinkedHashSet<>(candidateListModelList.getSelection());
        chooseListModelList.addAll(selection);
        candidateListModelList.removeAll(selection);
    }

    @Command("removeCommand")
    public void removeCommand() {
        Set<String> selection = new LinkedHashSet<>(chooseListModelList.getSelection());
        candidateListModelList.addAll(selection);
        chooseListModelList.removeAll(selection);
    }

    @Command
    public void onCheckAuditColumns(@BindingParam("rg") Radiogroup rg) {
        currentTable.setSelectedColumns(rg.getSelectedIndex() == 1);
        disableColumnSelect = !currentTable.isSelectedColumns();
        BindUtils.postNotifyChange(null, null, this, "disableColumnSelect");
    }

    @Command
    public void onSelectRowNumber(@BindingParam("cb") Combobox cb) {
        currentTable.setRowLimit(NumbersOfRows.getByIndex(cb.getSelectedIndex()).getValue());
    }

    public static void showForBeforeAfterData(Long databaseId, CMTable tableName, List<String> selectedColumns, DialogListener listener) {
        Map args = new HashMap();
        args.put(DATABASE_ARG, databaseId);
        args.put(TABLE_ARG, tableName);
        args.put(SELECTED_COLUMNS_ARG, selectedColumns);
        args.put(LISTENER_ARG, listener);
        Window window = (Window) Executions.createComponents(ZUL_URL, null, args);
        Component titleLabel = window.getFellow("titleLabel");
        if (titleLabel != null && titleLabel instanceof Label) {
            ((Label) titleLabel).setValue(ELFunctions.getLabel(SQLCMI18NStrings.USER_COLUMNS_DIALOG_BAD_TITLE));
        }
        Component rowSelectorVL = window.getFellow("rowSelectorVL");
        if (rowSelectorVL != null) {
            rowSelectorVL.setVisible(true);
        }
        window.doHighlighted();
    }
    
    public static void showForNewBeforeAfterData(Long databaseId, CMTable tableName, List<String> selectedColumns, DialogListener listener,String instanceName, String databaseName) {
        Map args = new HashMap();
        args.put(DATABASE_ARG, databaseId);
        args.put(TABLE_ARG, tableName);
        args.put(SELECTED_COLUMNS_ARG, selectedColumns);
        args.put(LISTENER_ARG, listener);
        isRegulationCall = true;
        mInstanceName = instanceName;
		mDatabaseName = databaseName;
        Window window = (Window) Executions.createComponents(ZUL_URL, null, args);
        Component titleLabel = window.getFellow("titleLabel");
        if (titleLabel != null && titleLabel instanceof Label) {
            ((Label) titleLabel).setValue(ELFunctions.getLabel(SQLCMI18NStrings.USER_COLUMNS_DIALOG_BAD_TITLE));
        }
        Component rowSelectorVL = window.getFellow("rowSelectorVL");
        if (rowSelectorVL != null) {
            rowSelectorVL.setVisible(true);
        }
        window.doHighlighted();
    }

    public static void showForSensitiveColumns(Long databaseId, CMTable entity,
			List<String> selectedColumns, DialogListener listener,
			boolean regulationCall, String instanceName, String databaseName) {
	Map args = new HashMap();
		if (regulationCall) {
			isRegulationCall = regulationCall;
			mInstanceName = instanceName;
			mDatabaseName = databaseName;
		}
	args.put(DATABASE_ARG, databaseId);
	args.put(TABLE_ARG, entity);
	args.put(SELECTED_COLUMNS_ARG, selectedColumns);
	args.put(LISTENER_ARG, listener);
	Window window = (Window) Executions.createComponents(ZUL_URL, null,
		args);
	Component titleLabel = window.getFellow("titleLabel");
	if (titleLabel != null && titleLabel instanceof Label) {
	    ((Label) titleLabel).setValue(ELFunctions
		    .getLabel(SQLCMI18NStrings.USER_COLUMNS_DIALOG_SC_TITLE));
	}
	Component rowSelectorVL = window.getFellow("rowSelectorVL");
	if (rowSelectorVL != null) {
	    rowSelectorVL.setVisible(false);
	}
	window.doHighlighted();
    }

    public ListModelList<String> getCandidateListModelList() {
        return candidateListModelList;
    }

    public ListModelList<String> getChooseListModelList() {
        return chooseListModelList;
    }

    public ListModelList<NumbersOfRows> getRowsNumberListModelList() {
        return rowsNumberListModelList;
    }

    public CMTable getCurrentTable() {
        return currentTable;
    }

    public boolean isDisableColumnSelect() {
        return disableColumnSelect;
    }

    public void setDisableColumnSelect(boolean disableColumnSelect) {
        this.disableColumnSelect = disableColumnSelect;
    }
}
