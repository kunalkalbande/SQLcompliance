package com.idera.sqlcm.ui.instanceEvents;

import com.idera.common.rest.JSONHelper;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.common.grid.EventsGridViewModel;
import com.idera.sqlcm.entities.CMServerAuditingRequest;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.entities.CMViewSettings;
import com.idera.sqlcm.entities.ViewNameData;
import com.idera.sqlcm.entities.ViewNameResponse;
import com.idera.sqlcm.facade.EventsFacade;
import com.idera.sqlcm.facade.FilterFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.FilterSelector;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.components.filter.model.FilterType;
import com.idera.sqlcm.ui.instanceEvents.filters.EventFilter;
import com.idera.sqlcm.ui.instanceEvents.filters.EventFilterChild;
import com.idera.sqlcm.ui.instanceEvents.filters.EventsFilters;
import com.idera.sqlcm.ui.instanceEvents.filters.EventsOptionFilterValues;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public class InstanceEventsGridViewModel extends EventsGridViewModel {

	private static final Logger logger = Logger
			.getLogger(InstanceEventsGridViewModel.class);

	private static final String INSTANCE_EVENTS_SESSION_DATA_BEAN = "InstanceEventsSessionDataBean";

	List<ViewNameData> nameList;
	public int refreshDuration; // SQLCM-5.4 SCM-9 start	

	public boolean disabledButton;
	
	public boolean isDisabledButton() {
		return disabledButton;
	}

	String eventType;

	public String getEventType() {
		return eventType;
	}

	public void setEventType(String eventType) {
		this.eventType = eventType;
	}

	public int getRefreshDuration() {
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}

	public List<ViewNameData> getNameList() {
		return nameList;
	}

	public void setNameList(List<ViewNameData> nameList) {
		this.nameList = nameList;
	}

	private ListModelList<String> eventOptions;

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		try {
			super.afterCompose(view);
			eventOptions = new ListModelList<>();
			if (!instance.getStatusText().equalsIgnoreCase("Archive server")) {
				eventOptions.add("Audit Events");
				eventType = "Audit Events";
			} else {
				selectEventSource("Archived Events");
				eventType = "Archived Events";
				disabledButton = true;
			}
			eventOptions.add("Archived Events");

			BindUtils.postNotifyChange(null, null,
					InstanceEventsGridViewModel.this, "*");

			// SQLCM 4.5 SCM -9 Start
			String refreshDuration = RefreshDurationFacade.getRefreshDuration();
			int refDuration = Integer.parseInt(refreshDuration);
			refDuration = refDuration * 1000;
			setRefreshDuration(refDuration);
			// SQLCM 4.5 SCM -9
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	public InstanceEventsGridViewModel() {
		entityFacade = new EventsFacade();
		eventsSource = EventsSource.AUDIT_EVENTS;
		exportBeanClass = InstanceEventsGridViewModel.class;
		preferencesSessionVariableName = INSTANCE_EVENTS_SESSION_DATA_BEAN;
		ViewNameResponse viewNameResponse = new ViewNameResponse();
		try {
			viewNameResponse = FilterFacade
					.getViewName(preferencesSessionVariableName);
			nameList = viewNameResponse.getViewNameTable();
			// Start 5.3.1 Null Pointer Exception on clicking Load View
			for (int i = 0; i < nameList.size(); i++) {
				if (nameList.get(i).getViewName().contains("_")) {
					String viewNameSubString = nameList
							.get(i)
							.getViewName()
							.substring(
									0,
									nameList.get(i).getViewName()
											.lastIndexOf("_"));
					nameList.get(i).setViewName(viewNameSubString);
				}
			}
			// End 5.3.1 Null Pointer Exception on clicking Load View
			setNameList(nameList);
			BindUtils.postNotifyChange(null, null,
					InstanceEventsGridViewModel.this, "*");
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		try {
			Long instanceId = Utils.parseInstanceIdArg();
			instance = InstancesFacade.getInstanceDetails(instanceId);
		} catch (RestException ex) {
			WebUtil.showErrorBox(ex, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
		}
	}

	protected void initColumnList() {
		eventsColumnsListModelList = getGridPreferencesInSession()
				.getColumnsListModelList();
		if (eventsColumnsListModelList == null) {
			eventsColumnsListModelList = new ListModelList<EventsColumns>();
			for (EventsColumns eventColumn : EventsColumns.values()) {
				if (eventColumn.isAllowedToDisable()) {
					eventsColumnsListModelList.add(eventColumn);
				}
			}
		}
		applyColumnsVisibility();
		BindUtils.postNotifyChange(null, null, this,
				"eventsColumnsListModelList");
	}

	protected void subscribeToVisibilityChangeEvent() {
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,
				EventQueues.SESSION, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getData().equals(preferencesSessionVariableName)) {
					applyColumnsVisibility();
					BindUtils.postNotifyChange(null, null,
							InstanceEventsGridViewModel.this,
							"eventsColumnsListModelList");
				}
			}
		});
	}

	@Command("changeColumnVisibility")
	public void changeColumnVisibility(
			@BindingParam("checked") boolean isPicked,
			@BindingParam("columnId") String columnId) {
		EventsColumns.findColumnById(columnId).setVisible(isPicked);
		getGridPreferencesInSession().setColumnsListModelList(
				eventsColumnsListModelList);
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,
				EventQueues.SESSION, false);
		if (eq != null) {
			eq.publish(new Event(CHANGE_VISIBILITY_EVENT, null,
					preferencesSessionVariableName));
		}
	}

	protected void applyColumnsVisibility() {
		for (EventsColumns eventsColumn : EventsColumns.values()) {
			applyColumnVisibility(eventsColumn.isVisible(),
					eventsColumn.getColumnId());
		}
	}

	protected ListModelList<Filter> getFiltersDefinition() {
		ListModelList<Filter> filtersDefinition = new ListModelList<>();

		Filter filter;

		for (EventsFilters eventsFilter : EventsFilters.values()) {
			filter = new EventFilter(eventsFilter);
			if (!eventsFilter.getFilterType().equals(FilterType.OPTIONS)) {
				filtersDefinition.add(filter);
			} else if (filter.getFilterId() == EventsFilters.ACCESS_CHECK
					.getFilterId()) {
				filter.addFilterChild(new EventFilterChild(
						EventsOptionFilterValues.ACCESS_CHECK_PASSED));
				filter.addFilterChild(new EventFilterChild(
						EventsOptionFilterValues.ACCESS_CHECK_FAILED));
				filtersDefinition.add(filter);
			} else if (filter.getFilterId() == EventsFilters.PRIVILEGED_USER
					.getFilterId()) {
				filter.addFilterChild(new EventFilterChild(
						EventsOptionFilterValues.PRIVILEGED_USER_YES));
				filter.addFilterChild(new EventFilterChild(
						EventsOptionFilterValues.PRIVILEGED_USER_NO));
				filtersDefinition.add(filter);
			}
		}

		return filtersDefinition;
	}

	public ListModelList<?> getEventsColumnsListModelList() {
		return eventsColumnsListModelList;
	}

	protected CommonGridViewReport makeCommonGridViewReport() {
		InstanceEventsGridViewReport instanceEventsGridViewReport = new InstanceEventsGridViewReport(
				"Instance Events", "", "Events");
		instanceEventsGridViewReport.setDataMapForListInstance(entitiesModel);
		return instanceEventsGridViewReport;
	}

	protected Map<String, Object> getCustomFilterRequest(
			Map<String, Object> filterRequest) {
		return super.getCustomFilterRequest(filterRequest);
	}

	@Command("changeAuditing")
	public void changeAuditing() {
		CMServerAuditingRequest cmServerAuditingRequest = new CMServerAuditingRequest();
		cmServerAuditingRequest.setServerIdList(new ArrayList<Long>(Arrays
				.<Long> asList(instance.getId())));
		cmServerAuditingRequest.setEnable(!instance.isEnabled());
		try {
			InstancesFacade.changeAuditingForServers(cmServerAuditingRequest);
			instance.setEnabled(!instance.isEnabled());
			BindUtils.postNotifyChange(null, null, this, "instance");
		} catch (RestException e) {
			WebUtil.showErrorBox(e, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
		}
	}

	protected Map<String, Boolean> collectColumnsVisibilityMap() {
		Map<String, Boolean> columns = new LinkedHashMap<String, Boolean>();
		for (EventsColumns eventsColumns : EventsColumns.values()) {
			columns.put(eventsColumns.getColumnId(), eventsColumns.isVisible());
		}
		return columns;
	}

	protected void retrieveColumnsVisibility(
			CMSideBarViewSettings alertsSettings) {
		Map<String, Boolean> columns = alertsSettings.getColumnVisibility();
		for (EventsColumns eventsColumns : EventsColumns.values()) {
			eventsColumns.setVisible(columns.get(eventsColumns.getColumnId())
					.booleanValue());
		}
	}

	@Command("saveViewState")
	public void saveViewState() {
		CMViewSettings settings = new CMViewSettings();
		String ViewName = (String) Sessions.getCurrent().getAttribute(
				"ViewName");
		Sessions.getCurrent().setAttribute("View",
				"instanceEventsView/" + instance.getId());
		settings.setViewId(preferencesSessionVariableName);
		settings.setViewName(ViewName);
		try {
			sideBarSettings = getSideBarViewSettings();
			sideBarSettings.setFilterData(getGridPreferencesInSession()
					.getFilters());
			sideBarSettings.setColumnVisibility(collectColumnsVisibilityMap());
			settings.setFilter(JSONHelper.serializeToJson(sideBarSettings));
			Sessions.getCurrent().setAttribute("settings", settings);
		} catch (IOException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_PROCESS_VIEW_STATE_FOR_SAVING);
		}
		Window window = (Window) Executions.createComponents(
				CommonGridViewModel.ZUL_URL_saveViewName, null, null);
		window.doHighlighted();
	}

	@Command("selectViewName")
	public void selectViewName(@BindingParam("id") String id) {
		// Start 5.3.1 Null Pointer Exception on clicking Load View
		String viewNameWithPrefrences = preferencesSessionVariableName
				.substring(0, preferencesSessionVariableName
						.lastIndexOf("SessionDataBean"));
		Sessions.getCurrent().setAttribute("ViewName",
				id + "_" + viewNameWithPrefrences);
		// End 5.3.1 Null Pointer Exception on clicking Load View
	}

	public void SaveViewName() {
		CMViewSettings settings = new CMViewSettings();
		String ViewName = (String) Sessions.getCurrent().getAttribute(
				"ViewName");
		settings.setViewId(preferencesSessionVariableName);
		settings.setViewName(ViewName);
		try {
			settings = (CMViewSettings) Sessions.getCurrent().getAttribute(
					"settings");
			// Start 5.3.1 Null Pointer Exception on clicking Load View
			String viewNameWithPrefrences = ViewName
					+ "_"
					+ settings.getViewId()
							.substring(
									0,
									settings.getViewId().lastIndexOf(
											"SessionDataBean"));
			// End 5.3.1 Null Pointer Exception on clicking Load View
			settings.setViewName(viewNameWithPrefrences);
			FilterFacade.saveViewSettings(settings);
		} catch (RestException e) {
			WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_SAVE_VIEW_STATE);
		}
	}

	@Command("loadViewState")
	public void loadViewState() {
		try {
			String viewName = (String) Sessions.getCurrent().getAttribute(
					"ViewName");
			cmViewSettings = FilterFacade.getViewSettings(viewName);
			if (cmViewSettings.getFilter() == null) {
				WebUtil.showInfoBox(SQLCMI18NStrings.YOU_DO_NOT_HAVE_A_FAVORITE_FILTER_SET);
				return;
			}
			parseSideBarViewSettings();
			retrieveColumnsVisibility(sideBarSettings);

			EventQueue<Event> eq = EventQueues.lookup(
					CHANGE_COLUMN_VISIBILITY_EVENT, EventQueues.SESSION, false);
			if (eq != null) {
				eq.publish(new Event(CHANGE_COLUMN_VISIBILITY_EVENT, null,
						preferencesSessionVariableName));
			}

			filterData = sideBarSettings.getFilterData();
			filtersModel = getFiltersDefinition();
			if (filterData != null) {
				filterData.setSource(preferencesSessionVariableName);
				getGridPreferencesInSession().setFilters(filterData);
				eq = EventQueues.lookup(FilterSelector.CHANGE_FILTER_EVENT,
						EventQueues.SESSION, false);
				if (eq != null) {
					eq.publish(new Event(FilterSelector.CHANGE_FILTER_EVENT,
							null, filterData));
				}
			}
		} catch (IOException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_PROCESS_LOADED_VIEW_STATE);
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_SAVED_VIEW_STATE);
		}
		Sessions.getCurrent().removeAttribute("ViewName");
		Executions.sendRedirect(WebUtil
				.buildPathRelativeToCurrentProduct("instanceEventsView/"
						+ instance.getId()));
	}

	public ListModelList<String> getEventOptions() {
		return eventOptions;
	}

	public void setEventOptions(ListModelList<String> eventOptions) {
		this.eventOptions = eventOptions;
	}

}
