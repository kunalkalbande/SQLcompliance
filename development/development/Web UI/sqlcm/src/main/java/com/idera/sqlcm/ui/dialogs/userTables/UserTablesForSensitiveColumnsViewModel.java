package com.idera.sqlcm.ui.dialogs.userTables;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.ExecutionArgParam;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Vlayout;
import org.zkoss.zul.Window;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.preferences.CommonGridPreferencesBean;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.utils.SQLCMConstants;

public class UserTablesForSensitiveColumnsViewModel {

	public static final String ZUL_URL = "~./sqlcm/dialogs/userTables/user-tables-for-sensitive-columns-dialog.zul";

	private static final Logger logger = Logger.getLogger(UserTablesForSensitiveColumnsViewModel.class);

	public static final String INSTANCE_ARG = "instance_arg";
	public static final String DATABASE_ARG = "database_arg";
	public static final String SELECTED_TABLES_ARG = "selected_tables_arg";
	public static final String FILTERED_TABLES_ARG = "filtered_tables_arg";
	public static final String LISTENER_ARG = "listener_arg";

	public static final String MSG_MUST_NOT_BE_NULL = " must not be null! ";
	public static final String EMPTY_FILTER_TEXT = "";

	public interface DialogListener {
		void onCloseUserTablesForSensitiveColumnsDialog(CMInstance instance, CMDatabase database,
				List<CMTable> selectedTables);
	}

	protected String preferencesSessionVariableName = CommonGridPreferencesBean.SESSION_VARIABLE_NAME + "_UserTables";
	protected int rowsCount = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

	private int prevPageSize;
	private DialogListener listener;

	@Wire
	private Paging listBoxPageId;

	@Wire
	private Spinner listBoxRowsBox;

	@Wire
	private Listbox lbAvailableDatabases;

	@Wire
	private Window userTablesWindow;

	@Wire
	private Div tipsDiv;

	@Wire
	private Label titleLabel;

	private String filterText;

	private ListModelList<CMTable> candidateListModelList;

	private ListModelList<CMTable> chooseListModelList;

	private List<CMTable> filteredTables;

	private CMInstance currentInstance;

	private CMDatabase currentDatabase;

	static String titleText;
	static String lableText;

	public String getLableText() {
		return lableText;
	}

	public void setLableText(String lableText) {
		this.lableText = lableText;
	}

	public String getTitleText() {
		return titleText;
	}

	public void setTitleText(String titleText) {
		this.titleText = titleText;
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
			@ExecutionArgParam(INSTANCE_ARG) CMInstance instance, @ExecutionArgParam(DATABASE_ARG) CMDatabase database,
			@ExecutionArgParam(SELECTED_TABLES_ARG) List<CMTable> selectedTables,
			@ExecutionArgParam(FILTERED_TABLES_ARG) List<CMTable> filteredTables,
			@ExecutionArgParam(LISTENER_ARG) DialogListener listener) {
		Selectors.wireComponents(view, this, false);
		lbAvailableDatabases.setPaginal(listBoxPageId);
		candidateListModelList = new ListModelList<>();
		chooseListModelList = new ListModelList<>();
		candidateListModelList.setMultiple(true);
		chooseListModelList.setMultiple(true);
		listBoxPageId.setPageSize(10);

		if (instance == null) {
			throw new RuntimeException(INSTANCE_ARG + MSG_MUST_NOT_BE_NULL);
		}
		currentInstance = instance;

		if (database == null) {
			throw new RuntimeException(DATABASE_ARG + MSG_MUST_NOT_BE_NULL);
		}
		currentDatabase = database;

		if (selectedTables != null) {
			chooseListModelList.clear();
			chooseListModelList.addAll(selectedTables);
		}

		this.filteredTables = filteredTables;

		this.listener = listener;

		loadTables(instance, database, EMPTY_FILTER_TEXT);
	}

	public String getFilterText() {
		return filterText;
	}

	public void setFilterText(String filterText) {
		this.filterText = filterText;
	}

	private void loadTables(CMInstance instance, CMDatabase database, String filterText) {
		try {
			List<CMTable> dbTables = DatabasesFacade.getTableList(instance.getId(), database.getName(), filterText);
			candidateListModelList.clear();
			for (CMTable table : dbTables)
				if (!table.getFullTableName()
						.equalsIgnoreCase("SQLcompliance_Data_Change.SQLcompliance_Changed_Data_Table"))
				{
					table.setRowLimit(10);
					candidateListModelList.add(table);
				}
			ListModelList candidatesForRemove = new ListModelList();
			boolean isFilteredExists = filteredTables != null && filteredTables.size() > 0;
			for (CMTable candidate : candidateListModelList) {
				for (CMTable choose : chooseListModelList) {
					if (candidate.equals(choose)) {
						candidatesForRemove.add(candidate);
					}
				}
				if (isFilteredExists && !filteredTables.contains(candidate)
						&& !candidatesForRemove.contains(candidate)) {
					candidatesForRemove.add(candidate);
				}
			}
			candidateListModelList.removeAll(candidatesForRemove);
			BindUtils.postNotifyChange(null, null, this, "titleText");
			BindUtils.postNotifyChange(null, null, this, "lableText");
			BindUtils.postNotifyChange(null, null, this, "candidateListModelList");
		} catch (RestException e) {
			WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_DATABASE_LIST);
		}
	}

	@Command("setGridRowsCount")
	public void setGridRowsCount() {
		try {
			int pageSize = listBoxRowsBox.getValue();
			if (listBoxRowsBox.getValue() == null) {
				Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR_STATUS), "warning",
						listBoxRowsBox, "end_center", 3000);
				pageSize = 5;
				listBoxRowsBox.setValue(pageSize);
			}
			if (pageSize > 50) {
				Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR_STATUS), "warning",
						listBoxRowsBox, "end_center", 3000);
				pageSize = 50;
				listBoxRowsBox.setValue(pageSize);
			} else if (pageSize < 5) {
				Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR_STATUS), "warning",
						listBoxRowsBox, "end_center", 3000);
				pageSize = 5;
				listBoxRowsBox.setValue(pageSize);
			}

			listBoxPageId.setPageSize(pageSize);
			prevPageSize = pageSize;
		} catch (WrongValueException exp) {
			listBoxPageId.setPageSize(prevPageSize);
		}
		PreferencesUtil.getInstance().setGridPagingPreferencesInSession(preferencesSessionVariableName,
				listBoxPageId.getPageSize());
	}

	@Command
	public void okCommand() {
		userTablesWindow.detach();
		if (listener != null) {
			listener.onCloseUserTablesForSensitiveColumnsDialog(currentInstance, currentDatabase,
					new ArrayList<>(chooseListModelList));
		}
	}

	@Command
	public void closeCommand() {
		userTablesWindow.detach();
	}

	@Command("addCommand")
	public void addCommand() {
		Set<CMTable> selection = new LinkedHashSet<>(candidateListModelList.getSelection());
		chooseListModelList.addAll(selection);
		candidateListModelList.removeAll(selection);
	}

	@Command("removeCommand")
	public void removeCommand() {
		Set<CMTable> selection = new LinkedHashSet<>(chooseListModelList.getSelection());
		candidateListModelList.addAll(selection);
		chooseListModelList.removeAll(selection);
	}

	@Command("filterClick")
	public void filterClick() {
		loadTables(currentInstance, currentDatabase, filterText);
	}

	public static void show(CMInstance instance, CMDatabase database, List<CMTable> selectedTables,
			DialogListener listener) {
		show(instance, database, selectedTables, null, listener);
	}

	public static void show(CMInstance instance, CMDatabase database, List<CMTable> selectedTables,
			List<CMTable> filteredTables, DialogListener listener) {
		Map args = new HashMap();
		args.put(INSTANCE_ARG, instance);
		args.put(DATABASE_ARG, database);
		args.put(SELECTED_TABLES_ARG, selectedTables);
		args.put(FILTERED_TABLES_ARG, filteredTables);
		args.put(LISTENER_ARG, listener);
		Window window = (Window) Executions.createComponents(ZUL_URL, null, args);
		window.doHighlighted();
	}

	public static void showWithoutTips(CMInstance instance, CMDatabase database, List<CMTable> selectedTables,
			List<CMTable> filteredTables, DialogListener listener, String title) {
		Map args = new HashMap();
		args.put(INSTANCE_ARG, instance);
		args.put(DATABASE_ARG, database);
		args.put(SELECTED_TABLES_ARG, selectedTables);
		args.put(FILTERED_TABLES_ARG, filteredTables);
		args.put(LISTENER_ARG, listener);
		Window window = (Window) Executions.createComponents(ZUL_URL, null, args);
		window.setTitle(title);

		titleText = title;
		if (titleText.indexOf("Before-After") != -1)
			lableText = ELFunctions.getLabel(SQLCMI18NStrings.BEFORE_AFTER_LABLE);
		else if (titleText.indexOf("DML/Select") != -1)
			lableText = ELFunctions.getLabel(SQLCMI18NStrings.DML_OR_SELECT_LABLE);
		else
			lableText = ELFunctions.getLabel(SQLCMI18NStrings.SENSITIVE_LABLE);
		Component userTablesWindow = window.getFellow("userTablesWindow");
		if (userTablesWindow != null && userTablesWindow instanceof Window) {
			((Window) userTablesWindow).setWidth("660px");
		}
		Component mainVlayout = window.getFellow("mainVlayout");
		if (mainVlayout != null && mainVlayout instanceof Vlayout) {
			((Vlayout) mainVlayout).setWidth("660px");
		}
		window.doHighlighted();
	}

	public static void showWithoutTips(CMInstance instance, CMDatabase database, List<CMTable> selectedTables,
			DialogListener listener, String title) {
		showWithoutTips(instance, database, selectedTables, null, listener, title);
	}

	public ListModelList<CMTable> getCandidateListModelList() {
		return candidateListModelList;
	}

	public void setCandidateListModelList(ListModelList<CMTable> candidateListModelList) {
		this.candidateListModelList = candidateListModelList;
	}

	public ListModelList<CMTable> getChooseListModelList() {
		return chooseListModelList;
	}

	public void setChooseListModelList(ListModelList<CMTable> chooseListModelList) {
		this.chooseListModelList = chooseListModelList;
	}
}
