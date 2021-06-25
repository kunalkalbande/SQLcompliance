package com.idera.sqlcm.ui.instancedetails;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import com.idera.server.web.component.zul.grid.ExtListheader;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMBeforeAfterDataEventsResponse;
import com.idera.sqlcm.entities.CMEventDetails;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.facade.EventsFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.charts.LineChartEngine;
import com.idera.sqlcm.ui.converter.LinkHrefConverter;
import com.idera.sqlcm.ui.converter.ListEmptyBooleanConverter;
import com.idera.sqlcm.ui.dialogs.EventPropertiesViewModel;
import com.idera.sqlcm.ui.dialogs.InstancePropertiesViewModel;
import com.idera.sqlcm.ui.permissionsCheck.PermissionCheckViewModel;
import com.idera.sqlcm.ui.preferences.CommonGridPreferencesBean;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.utils.SQLCMConstants;
import com.idera.sqlcm.ui.importAuditSetting.*;
import com.idera.sqlcm.wizard.*;
import com.idera.sqlcm.wizard.AbstractStatusWizardViewModel.WizardListener;

import net.sf.jasperreports.engine.JRException;

import org.apache.commons.lang.StringUtils;
import org.apache.commons.lang.builder.CompareToBuilder;
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
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Window;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.StatisticData;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.enumerations.Category;

import org.zkoss.zul.*;

import java.io.IOException;
import java.util.*;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;

public class InstanceEventsViewModel implements
		ImportAbstractWizardViewModel.WizardListener, WizardListener {

	private static final String ZUL_URL = "~./sqlcm/instancedetails/instance_detail_sensitive_column_import.zul"; // Start
																													// SQLCm-5.4
	private static final String ZUL_URL_Audit = "~./sqlcm/ImportAuditSetting/import_audit_setting_first.zul";
	private static final long serialVersionUID = 9072157864081300383L;
	public static final String CHANGE_INSTANCE_EVENT = "changeInstanceEvent";
	private static final Logger logger = Logger
			.getLogger(InstanceEventsViewModel.class);
	private static final int EVENTS_GRID_PAGE_SIZE = 10;
	private static final String POPUP_MAX_HEIGHT = "270px";
	private static final String DEFAULT_CHART_WIDTH = "300px";

	protected String preferencesSessionVariableName = CommonGridPreferencesBean.SESSION_VARIABLE_NAME
			+ "_InstanceEvents";
	protected int rowsCount = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

	private int prevPageSize;

	private Category currentCategory = Category.OVERALL_ACTIVITY;
	private Interval currentInterval = Interval.ONE_DAY;
	private StatisticData statics;
	private long currentInstanceId;
	private String currentInstanceName;
	private Converter linkHrefConverter = new LinkHrefConverter();
	private Converter listEmptyBooleanConverter = new ListEmptyBooleanConverter();

	private ListModelList<CMInstance> instanceNavigationModel;
	private ListModelList<CMEventDetails> eventsListModel;

	private int fileSize;

	private String errorMsg;

	@Wire
	private Listbox auditEventListBox;

	@Wire
	private Paging listBoxPageId;

	@Wire
	private Spinner listBoxRowsBox;

	@Wire
	private Popup instanceNavigationPopup;

	@Wire
	private Label errorTitleLabel;

	@Wire
	private Label errorDescriptionLabel;

	@Wire
	private Component chartContainer;

	@Wire
	private Chart activityChart;

	@Wire
	protected Listhead entitiesListBoxHead;

	@Wire
	private Include overview;

	protected CMInstance currentInstance = null;

	private int recordCount;

	public int refreshDuration; // SQLCM 5.4 SQLCM-2172

	private int activePage = SQLCMConstants.DEFAULT_PAGE;

	private Integer pageSize = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

	private int sortDirection = SQLCMConstants.SORT_ASCENDING;

	private String sortColumn = SQLCMConstants.DEFAULT_EVENTS_SORT_COLUMN;

	private ChartModel activityChartModel;

	private ListModelList<Interval> intervalListModelList;

	private ListModelList<Category> categoryListModelList;

	public int getFileSize() {
		return fileSize;
	}

	public void setFileSize(int fileSize) {
		this.fileSize = fileSize;
	}

	public int getRefreshDuration() {// SQLCM 5.4 SQLCM-2172 satrt
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}

	// SQLCM 5.4 SQLCM-2172 End

	public ListModelList<CMInstance> getInstanceNavigationModel() {
		return instanceNavigationModel;
	}

	public ListModelList<CMEventDetails> getEventsListModel() {
		return eventsListModel;
	}

	public String getCurrentInstanceName() {
		return currentInstanceName;
	}

	public Converter getLinkHrefConverter() {
		return linkHrefConverter;
	}

	public Converter getListEmptyBooleanConverter() {
		return listEmptyBooleanConverter;
	}

	public boolean isValidInstanceId() {
		return errorMsg == null;
	}

	public ChartModel getActivityChartModel() {
		return activityChartModel;
	}

	protected CommonGridPreferencesBean getGridPreferencesInSession() {
		return PreferencesUtil.getInstance().getGridPreferencesInSession(
				preferencesSessionVariableName);
	}

	public static final Comparator<CMInstance> INSTANCE_NAME_COMPARATOR = new Comparator<CMInstance>() {
		public int compare(CMInstance o1, CMInstance o2) {
			return new CompareToBuilder().append(
					StringUtils.lowerCase(o1.getName()),
					StringUtils.lowerCase(o2.getName())).toComparison();
		}
	};

	private void loadInstancesList() throws RestException {
		try {
			List<CMInstance> instanceList = InstancesFacade.getInstanceList();
			currentInstance = findInstanceById(instanceList, currentInstanceId);
			if (currentInstance != null) {
				currentInstanceName = currentInstance.getInstanceName()
						.toUpperCase();
				publishChangeInstanceEvent(currentInstance);

				Collections.sort(instanceList, INSTANCE_NAME_COMPARATOR);
				instanceNavigationModel = new ListModelList<>(instanceList,
						false);
			} else {
				errorMsg = ELFunctions
						.getLabelWithParams(
								SQLCMI18NStrings.INSTANCE_DETAIL_MSG_INSTANCE_WITH_ID_NOT_FOUND,
								currentInstanceId);
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_INSTANCE_LIST);
		}
	}

	private void initInstancesView() {
		int maxInstanceNameLength = 0;

		if (instanceNavigationModel == null) {
			return;
		}

		for (CMInstance coreInstance : instanceNavigationModel) {
			if (coreInstance.getInstanceName().length() > maxInstanceNameLength) {
				maxInstanceNameLength = coreInstance.getInstanceName().length();
			}
		}

		instanceNavigationPopup.setWidth(Integer
				.toString(maxInstanceNameLength * 10 + 30) + "px");
		instanceNavigationPopup.setHeight(POPUP_MAX_HEIGHT);
	}

	private static CMInstance findInstanceById(List<CMInstance> instanceList,
			long instanceId) {
		for (CMInstance instance : instanceList) {
			if (instance.getId() == instanceId) {
				return instance;
			}
		}
		return null;
	}

	@Init
	public void init() throws RestException {
		errorMsg = null;
		engine = new LineChartEngine();
		try {
			currentInstanceId = Utils.parseInstanceIdArg();

			initIntervalList();
			initCategoryList();
			loadInstancesList();
			loadEventList();
			loadStatistic();

		} catch (NumberFormatException e) {
			errorMsg = ELFunctions
					.getLabel(SQLCMI18NStrings.INSTANCE_DETAIL_MSG_ERROR_PARSE_INSTANCE_ID);
			logger.info(errorMsg);
		}

	}

	private void loadEventList() {
		CMBeforeAfterDataEventsResponse response = null;
		eventsListModel = new ListModelList<>();
		try {
			if (!currentInstance.getStatusText().equalsIgnoreCase(
					"Archive server")) {
				response = EventsFacade.getEventsByIntervalForInstance(
						currentInstanceId, currentInterval, activePage,
						pageSize, sortDirection, sortColumn);
				int totalRecords = response.getRecordCount();
				if (totalRecords < 0) {
					totalRecords = 0;
				}
				recordCount = totalRecords;
				eventsListModel = new ListModelList<>(
						(List<CMEventDetails>) response.getEvents());
				if (eventsListModel != null && !eventsListModel.isEmpty()) {
					setFileSize(recordCount);
				} else {
					setFileSize(0);
				}
				BindUtils.postNotifyChange(null, null, this, "eventsListModel");
			} else {
				recordCount = 0;
				BindUtils.postNotifyChange(null, null, this, "eventsListModel");
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_EVENT_LIST);
		}
		BindUtils.postNotifyChange(null, null, InstanceEventsViewModel.this,
				"*");
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		try {
			// SQLCM 4.5 SCM -9 Start
			String refreshDuration = RefreshDurationFacade.getRefreshDuration();
			int refDuration = Integer.parseInt(refreshDuration);
			refDuration = refDuration * 1000;
			setRefreshDuration(refDuration); // SQLCM 4.5 SCM -9
			// setGridRowsCount();
			setFileSize(recordCount);

			if (isValidInstanceId()) {
				/*
				 * if (this.auditEventListBox != null) {
				 * this.auditEventListBox.setPageSize(EVENTS_GRID_PAGE_SIZE); }
				 */
				subscribeForSortEvent();
				initChart();
				initInstancesView();
			} else {
				errorTitleLabel.setValue(ELFunctions
						.getLabel(I18NStrings.ERROR));
				errorDescriptionLabel.setValue(errorMsg);
			}
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	private void initIntervalList() {
		intervalListModelList = new ListModelList<>();
		intervalListModelList.add(Interval.ONE_DAY);
		intervalListModelList.add(Interval.SEVEN_DAY);
		intervalListModelList.add(Interval.THIRTY_DAY);
		intervalListModelList.setSelection(Arrays.asList(currentInterval));
	}

	private void initCategoryList() {
		categoryListModelList = new ListModelList<>();
		categoryListModelList.addAll(Arrays.asList(Category.values()));
		categoryListModelList.setSelection(Arrays.asList(currentCategory));
	}

	public ListModelList<Interval> getIntervalListModelList() {
		return intervalListModelList;
	}

	public ListModelList<Category> getCategoryListModelList() {
		return categoryListModelList;
	}

	private void publishChangeInstanceEvent(CMInstance currentInstance) {
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_INSTANCE_EVENT,
				EventQueues.SESSION, true);
		if (eq != null) {
			eq.publish(new Event("onClick", null, currentInstance));
		}
	}

	private void loadStatistic() {
		try {
			if (!currentInstance.getStatusText().equalsIgnoreCase(
					"Archive server")) {
				statics = new StatisticData();
				statics.setStatics(InstancesFacade.getInstanceStatsData(
						currentInstanceId, currentInterval, currentCategory));
				activityChartModel = statics.getDataXYModel();
			} else {
				statics = new StatisticData();
				activityChartModel = statics.getDataXYModel();
			}

		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_STATISTIC_DATA);
		}

		engine.setInterval(currentInterval);
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
		loadEventList();
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
							loadEventList();
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
			if (eventsListModel != null && !eventsListModel.isEmpty()) {

				setFileSize(recordCount);
			} else {
				setFileSize(0);
			}

		} catch (WrongValueException exp) {
			listBoxPageId.setPageSize(prevPageSize);
		}
		PreferencesUtil.getInstance().setGridPagingPreferencesInSession(
				preferencesSessionVariableName, listBoxPageId.getPageSize());
		loadEventList();
		BindUtils.postNotifyChange(null, null, InstanceEventsViewModel.this,
				"*");
	}

	@Command("openDBView")
	public void openDatabaseViewPage(@BindingParam("databaseId") long databaseId) {
		Executions.sendRedirect(WebUtil
				.buildPathRelativeToCurrentProduct("databaseView/"
						+ currentInstanceId + "/" + databaseId));
	}

	@Command("openEditInstanceProperties")
	public void openEditInstanceProperties() {
		InstancePropertiesViewModel
				.showInstancePropertiesDialog(currentInstanceId);
	}

	@Command("openPermissionsCheck")
	public void openPermissionsCheck() {
		PermissionCheckViewModel.showPermissionsCheckDialog(currentInstanceId);
	}

	@Command("openEventProperties")
	public void openEventProperties(@BindingParam("rowIndex") int rowIndex) {
		EventPropertiesViewModel.showEventPropertiesWindow(rowIndex,
				eventsListModel, currentInstanceId);
	}

	@Command("refreshData")
	public void refreshData() {
		Executions.getCurrent().sendRedirect(""); // reload current page
	}

	@NotifyChange({ "eventsListModel", "activityChartModel" })
	@Command("refreshEventList")
	public void refreshEventList() throws RestException {
		loadEventList();
		loadStatistic();
	}

	@NotifyChange({ "eventsListModel", "activityChartModel" })
	@Command("selectIntervalDays")
	public void selectIntervalDays(
			@BindingParam("radioGroup") Radiogroup radioGroup)
			throws RestException {
		currentInterval = Utils.getSingleSelectedItem(intervalListModelList);
		loadEventList();
		loadStatistic();
		changeInterval();
	}

	@NotifyChange({ "eventsListModel", "activityChartModel" })
	@Command("selectCategory")
	public void selectCategory() throws RestException {
		currentCategory = Utils.getSingleSelectedItem(categoryListModelList);
		loadEventList();
		loadStatistic();
	}

	@Command("openAllInstanceEventsPage")
	public void openAllInstanceEventsPage() {
		Executions.sendRedirect(WebUtil
				.buildPathRelativeToCurrentProduct("instanceEventsView/"
						+ currentInstanceId));
	}

	boolean threeD = false;

	public boolean isThreeD() {
		return threeD;
	}

	LineChartEngine engine;

	public LineChartEngine getEngine() {
		return engine;
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
								InstanceEventsViewModel.this,
								"activityChartModel");
					}
				});
	}

	private void changeInterval() {
		InstanceOverview instanceOverview = ((InstanceOverview) overview
				.getChildren().get(0).getAttribute("instOverview"));
		instanceOverview.reloadInstance(currentInterval.getDays());
	}

	// START SQLCm-5.4

	@Command("choosedetailView")
	public void choosedetailView(
			@BindingParam("id") String Id,
			@BindingParam("instaceActionCombobox") Combobox instaceActionCombobox)
			throws JRException, IOException, TransformerException,
			ParserConfigurationException {
		instaceActionCombobox.setValue("Instance Activities");
		if (Id.equals("editInstanceLink")) {
			openEditInstanceProperties();
		}
		if (Id.equals("permissionsCheckLink")) {
			openPermissionsCheck();
		}
		if (Id.equals("importAuditSetting")) {
			AddImportAuditWizardViewModel.showWizard(this);
		}
		if (Id.equals("exportAuditSetting")) {
			ExportServerAuditSettings(currentInstanceName);
		}

		if (Id.equals("Import")) {
			Sessions.getCurrent().setAttribute("currentInstanceName",
					currentInstanceName);
			Window window = (Window) Executions.createComponents(
					InstanceEventsViewModel.ZUL_URL, null, null);
			window.doHighlighted();
		}
	}

	public void ExportServerAuditSettings(String currentInstanceName) {
		InstancesFacade instanceFacade = new InstancesFacade();
		String exportedPath = instanceFacade
				.ExportServerAuditSettings(currentInstanceName);
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
