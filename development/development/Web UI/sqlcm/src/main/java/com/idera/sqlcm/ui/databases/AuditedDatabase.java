package com.idera.sqlcm.ui.databases;

import com.idera.common.Utility;
import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.server.web.component.zul.grid.ExtListheader;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMAuditedDatabase;
import com.idera.sqlcm.entities.CMBeforeAfterDataEventsResponse;
import com.idera.sqlcm.entities.CMDatabaseAuditingRequest;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.StatisticData;
import com.idera.sqlcm.entities.StatisticDataResponse;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.EventsFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.server.web.component.SQLCMLineChart;
import com.idera.sqlcm.ui.converter.ListEmptyBooleanConverter;
import com.idera.sqlcm.ui.databases.DatabaseOverview;
import com.idera.sqlcm.ui.databases.LineChartEngine;
import com.idera.sqlcm.ui.databases.PieChartEngine;
import com.idera.sqlcm.ui.dialogs.EventPropertiesViewModel;
import com.idera.sqlcm.ui.dialogs.databaseProperties.DatabasePropertiesViewModel;
import com.idera.sqlcm.ui.importAuditSetting.AddImportAuditWizardViewModel;
import com.idera.sqlcm.ui.preferences.CommonGridPreferencesBean;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.utils.SQLCMConstants;
import com.idera.sqlcm.wizard.ImportAbstractWizardViewModel.WizardListener;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.event.SortEvent;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.*;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;

public class AuditedDatabase implements WizardListener {

	protected CommonFacade entityFacade;

	static final String DATABASE_VIEW_CHANGE_INTERVAL_EVENT = "databaseViewChangeIntervalEvent";

	protected ListModelList<CMEntity> entitiesModel;

	protected Map<String, Object> emptyFilter = new TreeMap<>();

	protected String preferencesSessionVariableName = "AuditedDatabaseDataBean";

	protected int rowsCount = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

	private static final Logger logger = Logger
			.getLogger(AuditedDatabase.class);

	Long instanceId;

	Long databaseId;

	int fileSize;

	public int getFileSize() {
		return fileSize;
	}

	public void setFileSize(int fileSize) {
		this.fileSize = fileSize;
	}

	public int refreshDuration;

	public int getRefreshDuration() {
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}

	private int prevPageSize;

	@Wire
	protected Listbox entitiesListBox;

	@Wire
	Paging listBoxPageId;

	@Wire
	Spinner listBoxRowsBox;

	private static final String DEFAULT_CHART_WIDTH = "300px";

	private Interval currentInterval = Interval.ONE_DAY;

	private StatisticData statics;

	private List<StatisticDataResponse> statisticDataResponse;

	private Converter listEmptyBooleanConverter = new ListEmptyBooleanConverter();

	private String errorMsg;

	protected CMAuditedDatabase database;

	protected CMInstance instance;

	@Wire
	private SQLCMLineChart alertsActivity;

	@Wire
	private Component contentLayout;

	@Wire
	private Component chartContainer;

	@Wire
	private Chart activityChart;

	@Wire
	private Chart pieChart;

	@Wire
	private Component instEventsView;

	@Wire
	protected Listhead entitiesListBoxHead;

	private int recordCount;

	private int activePage = SQLCMConstants.DEFAULT_PAGE;

	private Integer pageSize = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

	private int sortDirection = SQLCMConstants.SORT_ASCENDING;

	private String sortColumn = SQLCMConstants.DEFAULT_EVENTS_SORT_COLUMN;

	private ChartModel activityChartModel;

	private ChartModel activityPieChartModel;

	private ListModelList<Interval> intervalListModelList;

	public Converter getListEmptyBooleanConverter() {
		return listEmptyBooleanConverter;
	}

	public ChartModel getActivityChartModel() {
		return activityChartModel;
	}

	public ChartModel getActivityPieChartModel() {
		return activityPieChartModel;
	}

	public enum Category {
		ADMIN(8, ELFunctions.getLabel(SQLCMI18NStrings.ADMIN)), DDL(9,
				ELFunctions.getLabel(SQLCMI18NStrings.DDL)), DML(11,
				ELFunctions.getLabel(SQLCMI18NStrings.DML)), SECURITY(10,
				ELFunctions.getLabel(SQLCMI18NStrings.SECURITY)), SELECT(15,
				ELFunctions.getLabel(SQLCMI18NStrings.SELECT));

		private String label;

		private int index;

		private Category(int index, String label) {
			this.label = label;
			this.index = index;

		}

		public String getLabel() {
			return label;
		}

		public String getName() {
			return this.name();
		}

		public int getIndex() {
			return index;
		}
	}

	public AuditedDatabase() {
	}

	private void loadDatabaseEvents() {
		CMBeforeAfterDataEventsResponse response = null;
		entitiesModel = new ListModelList<>();
		try {
			response = EventsFacade.getEventsByIntervalForDatabase(instanceId,
					databaseId, currentInterval, activePage, pageSize,
					sortDirection, sortColumn);
			int totalRecords = response.getRecordCount();
			if (totalRecords < 0) {
				totalRecords = 0;
			}
			recordCount = totalRecords;
			entitiesModel = new ListModelList<>(
					(List<CMEntity>) response.getEvents());
			if(entitiesModel !=null && !entitiesModel.isEmpty())
            {
				setFileSize(recordCount);        	
            }
            else
            {
            	setFileSize(0);
            }
			BindUtils.postNotifyChange(null, null, this, "entitiesModel");
			BindUtils.postNotifyChange(null, null, this, "totalSize");
		} catch (RestException e) {
			WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_EVENT_LIST);
		}
		BindUtils.postNotifyChange(null, null, AuditedDatabase.this, "*");
	}

	@Init
	public void init() {

		entityFacade = new EventsFacade();
		instanceId = Utils.parseInstanceIdArg();
		databaseId = Utils.parseDatabaseIdArg();
		Sessions.getCurrent().setAttribute("dbId", databaseId);
		try {
			instance = InstancesFacade.getInstanceDetails(instanceId);
			database = DatabasesFacade.getDatabaseDetails(
					instanceId.toString(), databaseId.toString());
			loadDatabaseEvents();
		} catch (RestException e) {
			WebUtil.showErrorBox(e, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
		}
		errorMsg = null;
		lineChartEngine = new LineChartEngine();
		pieChartEngine = new PieChartEngine();
		try {
			initIntervalList();
			loadStatics(currentInterval);
		} catch (NumberFormatException e) {
			errorMsg = ELFunctions
					.getLabel(SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
			logger.info(errorMsg);
		}
	}

	public ListModelList<CMEntity> getEntitiesModel() {
		return entitiesModel;
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		try {
			String refreshDuration = RefreshDurationFacade.getRefreshDuration();
			int refDuration = Integer.parseInt(refreshDuration);
			refDuration = refDuration * 1000;
			setRefreshDuration(refDuration);
			setFileSize(recordCount);
			entitiesListBox.setNonselectableTags("<div><tr><td><a><img>");
			subscribeForSortEvent();
			initChart();
			loadPieChart();
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	private void loadPieChart() {
		logger.info("Chart rendering started");
		pieChart.invalidate();
		pieChart.setModel(statics.getPieModel());
	}

	private void initIntervalList() {
		intervalListModelList = new ListModelList<>();
		intervalListModelList.add(Interval.ONE_DAY);
		intervalListModelList.add(Interval.SEVEN_DAY);
		intervalListModelList.add(Interval.THIRTY_DAY);
		intervalListModelList.setSelection(Arrays.asList(currentInterval));
	}

	public ListModelList<Interval> getIntervalListModelList() {
		return intervalListModelList;
	}

	private void loadStatics(Interval interval) {
		try {
			statics = new StatisticData();
			statisticDataResponse = DatabasesFacade.getRecentDatabaseActivity(
					instanceId, interval, databaseId);
			List<StatisticData.Statistic> statist = new ArrayList<>();
			for (Category cat : Category.values()) {
				for (StatisticDataResponse stat : statisticDataResponse) {
					if (stat.getKey() == cat.getIndex()) {
						for (StatisticData.Statistic st : stat.getValue()) {
							st.setCategoryName(cat.getLabel());
							statist.add(st);
						}
					}
				}
			}
			statics.setStatics(statist);
			activityChartModel = statics.getDataXYModel();
			activityPieChartModel = statics.getPieModel();
			lineChartEngine.setInterval(interval);
		} catch (RestException e) {
			WebUtil.showErrorBox(e, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
		}
	}

	private void setEmptyChart() {
		alertsActivity.setTitle("");
		alertsActivity.setErrorMessage(Utility
				.getMessage(SQLCMI18NStrings.NO_DATA_AVAILABLE));
	}

	public int getTotalSize() {
		return recordCount;
	}

	public Integer getPageSize() {
		return pageSize;
	}

	public void setPageSize(Integer pageSize) {
		this.pageSize = pageSize;
	}

	public void setActivePage(int selectedPage) {
		activePage = selectedPage + 1;
		loadDatabaseEvents();
	}

	public int getActivePage() {
		return SQLCMConstants.DEFAULT_PAGE;
	}

	protected void subscribeForSortEvent() {
		Collection<Component> heads = entitiesListBoxHead.getChildren();
		for (Component headerComponent : heads) {
			headerComponent.addEventListener(Events.ON_SORT,
					new EventListener<SortEvent>() {
						public void onEvent(SortEvent event) throws Exception {
							String sortParam = ((ExtListheader) event
									.getTarget()).getValue();
							if (sortParam == null || sortParam.trim().isEmpty()) {
								throw new RuntimeException(
										" Invalid column value that is used as sort parameter! ");
							}
							sortColumn = sortParam;
							if (event.isAscending()) {
								sortDirection = SQLCMConstants.SORT_ASCENDING;
							} else {
								sortDirection = SQLCMConstants.SORT_DESCENDING;
							}
							loadDatabaseEvents();
						}
					});
		}
	}

	@Command("setGridRowsCount")
	public void setGridRowsCount() {
		try {
			int tmpPageSize = listBoxRowsBox.getValue();
			if (tmpPageSize > 100) {
				Clients.showNotification(
						ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR),
						"warning", listBoxRowsBox, "end_center", 3000);
				tmpPageSize = 100;
				listBoxRowsBox.setValue(tmpPageSize);
			}
			if (tmpPageSize != pageSize) {
				activePage = SQLCMConstants.DEFAULT_PAGE;
			}
			listBoxPageId.setPageSize(tmpPageSize);
			prevPageSize = tmpPageSize;
			pageSize = tmpPageSize;
			if (entitiesModel != null && !entitiesModel.isEmpty()) {
				setFileSize(recordCount);
			} else {
				setFileSize(0);
			}

		} catch (WrongValueException exp) {
			listBoxPageId.setPageSize(prevPageSize);
		}
		PreferencesUtil.getInstance().setGridPagingPreferencesInSession(
				preferencesSessionVariableName, listBoxPageId.getPageSize());
		loadDatabaseEvents();
		BindUtils.postNotifyChange(null, null, AuditedDatabase.this, "*");
	}

	@NotifyChange({ "activityChartModel", "activityPieChartModel",
			"entitiesModel" })
	@Command("refreshEventList")
	public void refreshEventList() throws RestException {
		loadStatics(currentInterval);
		loadDatabaseEvents();
		loadPieChart();
		generateRefreshEvent();
	}

	@NotifyChange({ "activityChartModel", "activityPieChartModel",
			"entitiesModel" })
	@Command("selectIntervalDays")
	public void selectIntervalDays(
			@BindingParam("radioGroup") Radiogroup radioGroup)
			throws RestException {
		currentInterval = Utils.getSingleSelectedItem(intervalListModelList);
		loadStatics(currentInterval);
		loadDatabaseEvents();
		loadPieChart();
		generateRefreshEvent();
	}

	boolean threeD = false;

	public boolean isThreeD() {
		return threeD;
	}

	LineChartEngine lineChartEngine;

	PieChartEngine pieChartEngine;

	public LineChartEngine getLineChartEngine() {
		return lineChartEngine;
	}

	public PieChartEngine getPieChartEngine() {
		return pieChartEngine;
	}

	public CMAuditedDatabase getDatabase() {
		return database;
	}

	public CMInstance getInstance() {
		return instance;
	}

	public void initChart() {
		/**
		 * Code below used to receive chart container width & generate
		 * JFreeChart component. This hack is used because JFreeChart widget
		 * does not support width in percentage.
		 */
		chartContainer.addEventListener("onCreateChartContainer",
				new EventListener() {
					@Override
					public void onEvent(Event event) throws Exception {
						Object eventData = event.getData();
						String chartWidth;
						if (eventData instanceof Number) {
							int chartWidthValue = ((Number) eventData)
									.intValue();
							if (chartWidthValue > 0) {
								chartWidth = chartWidthValue + "px";
							} else {
								chartWidth = DEFAULT_CHART_WIDTH;
								logger.error(" Invalid chart width value number -> "
										+ eventData);
							}
						} else {
							chartWidth = DEFAULT_CHART_WIDTH;
							logger.error(" Invalid chart width value -> "
									+ eventData);
						}
						activityChart.setWidth(chartWidth);
						activityChartModel = statics.getDataXYModel();
						BindUtils.postNotifyChange(null, null,
								AuditedDatabase.this, "activityChartModel");
					}
				});
	}

	protected CommonGridPreferencesBean getGridPreferencesInSession() {
		return PreferencesUtil.getInstance().getGridPreferencesInSession(
				preferencesSessionVariableName);
	}

	@Command("showEventPropertiesDialog")
	public void showEventProperties(@BindingParam("rowIndex") int rowIndex) {
		EventPropertiesViewModel.showEventPropertiesWindow(rowIndex,
				entitiesModel, instanceId);
	}

	@Command("changeAuditing")
	public void changeAuditing() {
		CMDatabaseAuditingRequest cmDatabaseAuditingRequest = new CMDatabaseAuditingRequest();
		cmDatabaseAuditingRequest.setDatabaseIdList(new ArrayList<Long>(Arrays
				.<Long> asList(database.getId())));
		cmDatabaseAuditingRequest.setEnable(!database.isEnabled());
		try {
			DatabasesFacade
					.changeAuditingForDatabase(cmDatabaseAuditingRequest);
			database.setIsEnabled(!database.isEnabled());
			BindUtils.postNotifyChange(null, null, this, "database");
			EventQueue<Event> eq = EventQueues.lookup(
					DatabaseOverview.CHANGE_AUDITING_STATE,
					EventQueues.SESSION, false);
			if (eq != null) {
				eq.publish(new Event("onClick", null, database.isEnabled()));
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
		}
	}

	@Command("removeDatabase")
	public void removeDatabase() {
		if (WebUtil.showConfirmationBoxWithIcon(
				SQLCMI18NStrings.DATABASE_REMOVE_MESSAGE,
				SQLCMI18NStrings.DATABASE_REMOVE_TITLE,
				"~./sqlcm/images/high-16x16.png", true, (Object) null)) {
			try {
				Map databaseMap = new HashMap<>();
				databaseMap.put("databaseId", database.getId());
				DatabasesFacade.removeDatabase(databaseMap);
				Executions.sendRedirect(WebUtil
						.buildPathRelativeToCurrentProduct("index"));
			} catch (RestException ex) {
				WebUtil.showErrorBox(ex,
						SQLCMI18NStrings.DATABASE_FAILED_DELETE);
			}
		}
	}

	@Command("editProperties")
	public void editProperties() {
		Map<String, Object> args = new HashMap<>();
		args.put(DatabasePropertiesViewModel.INSTANCE_ARG, instance);
		args.put(DatabasePropertiesViewModel.DATABASE_ARG, database);
		args.put(DatabasePropertiesViewModel.DATABASE_ID, database.getId());
		Window window = (Window) Executions.createComponents(
				DatabasePropertiesViewModel.ZUL_URL, null, args);
		window.doHighlighted();
	}

	@Command("openDatabaseView")
	public void openDatabaseView() {
		Executions.sendRedirect(WebUtil
				.buildPathRelativeToCurrentProduct("databaseEventsView/"
						+ instanceId + "/" + databaseId));
	}

	@Command("openInstance")
	public void openInstance(@BindingParam("instanceId") long id) {
		if (id > 0)
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("instanceView/" + id));
	}

	@Command("openDatabase")
	public void openDatabase() {
		Executions.getCurrent().sendRedirect("");
	}

	private void generateRefreshEvent() {
		EventQueue<Event> eq = EventQueues
				.lookup(DATABASE_VIEW_CHANGE_INTERVAL_EVENT,
						EventQueues.SESSION, false);
		if (eq != null) {
			eq.publish(new Event(DATABASE_VIEW_CHANGE_INTERVAL_EVENT, null,
					currentInterval.getDays()));
		}
	}

	@Command("choosedetailView")
	public void choosedetailView(@BindingParam("id") String Id,
			@BindingParam("databaseAction") Combobox databaseAction)
			throws IOException, TransformerException,
			ParserConfigurationException, Exception {

		databaseAction.setValue("Database Activities");
		if (Id.equals("editProperties")) {
			editProperties();
		}
		if (Id.equals("changeAuditing")) {
			changeAuditing();
		}
		if (Id.equals("removeDatabase")) {
			removeDatabase();
		}
		if (Id.equals("importAuditSetting")) {
			AddImportAuditWizardViewModel.showWizard(this);
		}

		if (Id.equals("exportAuditSetting")) {
			ExportDatabaseAuditSettings((int) (long) databaseId);
		}

	}

	public void ExportDatabaseAuditSettings(int database) {
		DatabasesFacade databaseFacade = new DatabasesFacade();
		String exportedPath = databaseFacade
				.ExportDatabaseAuditSettings(database);
		if (!exportedPath.equalsIgnoreCase("failed")) {
			WebUtil.showInfoBoxWithCustomMessage("File Exported Successfully to "
					+ "'" + exportedPath + "'.");
		} else {
			WebUtil.showInfoBoxWithCustomMessage("Export failed.");
		}
	}

	@Override
	public void onCancel() {
		// TODO Auto-generated method stub

	}

	@Override
	public void onFinish() {
		// TODO Auto-generated method stub

	}

}
