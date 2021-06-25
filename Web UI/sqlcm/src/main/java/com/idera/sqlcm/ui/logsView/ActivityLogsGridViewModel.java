package com.idera.sqlcm.ui.logsView;

import java.io.IOException;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

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
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Listheader;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Window;

import com.idera.common.rest.JSONHelper;
import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.entities.CMActivityLogs;
import com.idera.sqlcm.entities.CMEventFilterListData;
import com.idera.sqlcm.entities.CMFilteredActivityLogsResponse;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.entities.CMViewSettings;
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.entities.ViewNameData;
import com.idera.sqlcm.entities.ViewNameResponse;
import com.idera.sqlcm.facade.ActivityLogsFacade;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.facade.FilterFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.FilterSelector;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.dialogs.ActivityLogsPropertiesViewModel;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.ui.activityLogsView.filters.ActivityLogsViewFilter;
import com.idera.sqlcm.ui.activityLogsView.filters.ActivityLogsViewFilters;

public class ActivityLogsGridViewModel extends CommonGridViewModel {
	
	private static final String ZUL_URL = "~./sqlcm/prompt/DeletePrompt.zul";
	private static final Logger logger = Logger.getLogger(ActivityLogsGridViewModel.class);
	
	@Wire
    protected Listbox entitiesListBox;
    @Wire
    Paging listBoxPageId;
    
    @Wire
    Spinner listBoxRowsBox;
    
    private int prevPageSize;
    
    public int refreshDuration;
    
	 public int getRefreshDuration() {
		return refreshDuration;
	}
	
	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}
	
	private static final String CHANGE_VISIBILITY_EVENT = "changeColumnVisibility";
	
	protected AlertsSource alertsSource;
	
	protected ListModelList activityLogsColumnsListModelList;
	
	protected CMFilteredActivityLogsResponse cmFilteredActivityLogsResponse;
	
	private ActivityLogsIconURLConverter iconURLConverter;
	
	private ActivityLogsLabelURLConverter labelURLConverter;
	
	private List<CMEventFilterListData> eventFilterList;
	public List<CMEventFilterListData> getEventFilterList() {
		return eventFilterList;
	}
	public void setEventFilterList(List<CMEventFilterListData> eventFilterList) {
		this.eventFilterList = eventFilterList;
	}
	
	List<CMActivityLogs> cmActivityLogs;
	InsertQueryData insertQueryData;
	
	FilterFacade filterFacade = new FilterFacade();
	List<ViewNameData> nameList;
	public List<ViewNameData> getNameList() {
		return nameList;
	}
	public void setNameList(List<ViewNameData> nameList) {
		this.nameList = nameList;
	}
	public static enum AlertsSource {
		EVENT_ALERTS(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_ALERTS), 1), STATUS_ALERTS(
				ELFunctions.getLabel(SQLCMI18NStrings.STATUS_ALERTS), 2), DATA_ALERTS(
				ELFunctions.getLabel(SQLCMI18NStrings.DATA_ALERTS), 3);

		private int id;

		AlertsSource(String label, int id) {
			this.id = id;
		}

		public int getId() {
			return id;
		}
	}

	public ActivityLogsGridViewModel() {
		entityFacade = new ActivityLogsFacade();
		filterFacade = new FilterFacade();
		alertsSource = AlertsSource.EVENT_ALERTS;
		exportBeanClass = ActivityLogsGridViewModel.class;
		preferencesSessionVariableName = "ActivityLogsSessionDataBean";
		iconURLConverter = new ActivityLogsIconURLConverter();
		labelURLConverter = new ActivityLogsLabelURLConverter();
		ViewNameResponse viewNameResponse = new ViewNameResponse();
		try {
			viewNameResponse = filterFacade.getViewName(preferencesSessionVariableName);
			nameList = viewNameResponse.getViewNameTable();
			for(int i=0; i<nameList.size(); i++){
				if(nameList.get(i).getViewName().contains("_")){
					String viewNameSubString = nameList.get(i).getViewName().substring(
							0,nameList.get(i).getViewName().lastIndexOf("_"));
					nameList.get(i).setViewName(viewNameSubString);
				}
			}
			setNameList(nameList);
			BindUtils.postNotifyChange(null, null, ActivityLogsGridViewModel.this, "*");
		} catch (RestException e) {
			e.printStackTrace();
		}
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		try{
			super.afterCompose(view);
			setGridRowsCount();
			String refreshDuration= RefreshDurationFacade.getRefreshDuration();
			int refDuration=Integer.parseInt(refreshDuration);
			refDuration=refDuration*1000;
			setRefreshDuration(refDuration); 
			initColumnList();
			subscribeToVisibilityChangeEvent();
		}
		catch(Exception e)
		{
			e.getStackTrace();
		}
		
	}
	
	@Command("setGridRowsCount")
    public void setGridRowsCount() {
        try {
            int pageSize = listBoxRowsBox.getValue();
            if (pageSize > 100) {
                Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
                        listBoxRowsBox, "end_center", 3000);
                pageSize = 100;
                listBoxRowsBox.setValue(pageSize);
            }
            listBoxPageId.setPageSize(pageSize);
            prevPageSize = pageSize;
            int value=0;
            if(entitiesList !=null && !entitiesList.isEmpty())
            {
				value = entitiesList.size();
				setFileSize(value);
			}
            else
            {
            	setFileSize(0);
            }
            
        } catch (WrongValueException exp) {
            listBoxPageId.setPageSize(prevPageSize);
        }
        PreferencesUtil.getInstance().setGridPagingPreferencesInSession(preferencesSessionVariableName, listBoxPageId.getPageSize());
        BindUtils.postNotifyChange(null, null, ActivityLogsGridViewModel.this, "*");
    }
	
	
	protected void initColumnList() {
		activityLogsColumnsListModelList = getGridPreferencesInSession()
				.getActivityLogsColumnsListModelList();
		if (activityLogsColumnsListModelList == null) {
			activityLogsColumnsListModelList = new ListModelList<ActivityLogsColumns>();
			activityLogsColumnsListModelList.add(ActivityLogsColumns.DATE);
			activityLogsColumnsListModelList.add(ActivityLogsColumns.TIME);
			activityLogsColumnsListModelList.add(ActivityLogsColumns.INSTANCE_NAME);
			activityLogsColumnsListModelList.add(ActivityLogsColumns.EVENT);
			activityLogsColumnsListModelList.add(ActivityLogsColumns.DETAIL);
			getGridPreferencesInSession().setActivityLogsColumnsListModelList(
					activityLogsColumnsListModelList);
		}
		applyColumnsVisibility();
		BindUtils.postNotifyChange(null, null, this,
				"activityLogsColumnsListModelList");
	}

	@Command("openInstance")
    public void openInstance(@BindingParam("instanceId") long id) {
		if(id>0)
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instanceView/" + id));
    }
	
	@Override
	@Command("refreshEvents")
	public void refreshEntitiesList() {
		Map<String, Object> customFilterRequest = getCustomFilterRequest(filterRequest);
		try {
			cmFilteredActivityLogsResponse = ((ActivityLogsFacade) entityFacade)
					.getFilteredActivitylogs(customFilterRequest);
			entitiesList = (List) cmFilteredActivityLogsResponse.getAlerts();
			setEventFilterList(cmFilteredActivityLogsResponse.getEventType());
			int totalRecords = entitiesList.size();
            if (totalRecords < 0) {
                totalRecords = 0;
            }
            setFileSize(totalRecords);
		} catch (RestException ex) {
			WebUtil.showErrorBox(ex,
					SQLCMI18NStrings.FAILED_TO_LOAD_FILTERED_ALERTS);
		}
		super.verifyEntitiesList();
		BindUtils.postNotifyChange(null, null, this, "instancesAlertsSummary");
		BindUtils.postNotifyChange(null, null, this, "entitiesModel");
		BindUtils.postNotifyChange(null, null, ActivityLogsGridViewModel.this, "*");
	}

	@Command("deleteLogs")
	public void deleteActivityLogs(@BindingParam("eventId") final int eventId) throws Exception{
  		Map args = new HashMap();
  		if(Sessions.getCurrent().getAttribute("PopUpConnfirm")==null) 
  		{
  			Sessions.getCurrent().setAttribute("ruleIdForDelete", "activityLogs|"+eventId);
  			Window window = (Window) Executions.createComponents(ActivityLogsGridViewModel.ZUL_URL, null, args);
  			window.doHighlighted();
  		}
  		else
  		{
  			deleteLogs(eventId);
  			refreshEntitiesList();
  		}
	}
	public void deleteLogs(@BindingParam("eventId") int eventId){
		String DeleteActivityLogs = "";
		DeleteActivityLogs = "DELETE FROM [SQLcompliance].[dbo].[AgentEvents] WHERE [eventId] = "+ eventId +";";
		DeleteAgentEventLogs(DeleteActivityLogs);
	  }

	    public void DeleteAgentEventLogs(String DeleteActivityLogs){
	    	insertQueryData = new InsertQueryData();
	    	insertQueryData.setDataQuery(DeleteActivityLogs);
	    	try {
	    		AlertRulesFacade.insertStatusAlertRules(insertQueryData);
	    	} catch (RestException e) {
	    		WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
	    	}
	    }

	protected void subscribeToVisibilityChangeEvent() {
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,
				EventQueues.SESSION, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getData().equals(preferencesSessionVariableName)) {
					applyColumnsVisibility();
					BindUtils.postNotifyChange(null, null,
							ActivityLogsGridViewModel.this,
							"alertsColumnsListModelList");
				}
			}
		});
	}

	public CMFilteredActivityLogsResponse getCmFilteredAlertsResponse() {
		return cmFilteredActivityLogsResponse;
	}

	public String getInstancesAlertsSummary() {
		if (this.cmFilteredActivityLogsResponse != null) {
			return ELFunctions.getLabelWithParams(
					SQLCMI18NStrings.INSTANCES_ALERTS_SUMMARY,
					cmFilteredActivityLogsResponse.getTotalSevereAlerts(),
					cmFilteredActivityLogsResponse.getTotalHighAlerts(),
					cmFilteredActivityLogsResponse.getTotalMediumAlerts(),
					cmFilteredActivityLogsResponse.getTotalLowAlerts());
		}
		return ELFunctions.getLabel(SQLCMI18NStrings.N_A);
	}

	private Listheader getListheader(String listheaderId) {
		List<Component> listHeaders = entitiesListBox.getListhead()
				.getChildren();
		Listheader lh = null;
		for (Component listheader : listHeaders) {
			if (listheaderId.equals(listheader.getId())) {
				lh = (Listheader) listheader;
				break;
			}
		}
		return lh;
	}

	@Command("changeOpenCloseState")
	public void changeOpenCloseState(
			@ContextParam(ContextType.TRIGGER_EVENT) Event event) {
		Groupbox filterGroup = (Groupbox) event.getTarget();
		filterGroup.getCaption().setSclass(
				" open-" + (filterGroup.isOpen() ? "true" : "false"));
	}

	@Command("showEventPropertiesDialog")
	public void showEventProperties(@BindingParam("rowIndex") int rowIndex) {
		ActivityLogsPropertiesViewModel.showEventPropertiesWindow(rowIndex,
				entitiesModel, 1);
	  }

	@Command("saveViewState")
	public void saveViewState() {
		CMViewSettings settings = new CMViewSettings();
		String ViewName = (String) Sessions.getCurrent().getAttribute("ViewName");
		Sessions.getCurrent().setAttribute("View", "activityLogsView");
        settings.setViewId(preferencesSessionVariableName);
        settings.setViewName(ViewName);
		CMSideBarViewSettings alertsSettings = new CMSideBarViewSettings();
		alertsSettings
				.setFilterData(getGridPreferencesInSession().getFilters());
		Map<String, Boolean> columns = new LinkedHashMap<String, Boolean>();
		for (ActivityLogsColumns alertsColumn : ActivityLogsColumns.values()) {
			columns.put(alertsColumn.getColumnId(), alertsColumn.isVisible());
		}
		alertsSettings.setColumnVisibility(columns);
		try {
			settings.setFilter(JSONHelper.serializeToJson(alertsSettings));
			Sessions.getCurrent().setAttribute("settings",settings);
		} catch (IOException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_COLLECT_VIEW_STATE);
		} 
		 Window window = (Window) Executions.createComponents(
		    		CommonGridViewModel.ZUL_URL_saveViewName, null,
					null);
			window.doHighlighted();
	}
	
	@Command("selectViewName")
	public void selectViewName(@BindingParam("id") String id) throws RestException {
		String viewNameWithPrefrences=preferencesSessionVariableName.substring(0,
        		preferencesSessionVariableName.lastIndexOf("SessionDataBean"));
		Sessions.getCurrent().setAttribute("ViewName", id +  "_" +viewNameWithPrefrences);
	}

	@Command("loadViewState")
	public void loadViewState() {
		try {
			String viewName =  (String) Sessions.getCurrent().getAttribute("ViewName");
			CMViewSettings cmViewSettings = filterFacade.getViewSettings(viewName);
			if (cmViewSettings.getFilter() == null) {
				WebUtil.showInfoBox(SQLCMI18NStrings.FAILED_TO_COLLECT_VIEW_STATE);
				return;
			}
			CMSideBarViewSettings alertsSettings = JSONHelper
					.deserializeFromJson(
							cmViewSettings.getFilter(),
							new com.fasterxml.jackson.core.type.TypeReference<CMSideBarViewSettings>() {
							});

			Map<String, Boolean> columns = alertsSettings.getColumnVisibility();
			for (ActivityLogsColumns alertsColumn : ActivityLogsColumns
					.values()) {
				alertsColumn.setVisible(columns.get(alertsColumn.getColumnId())
						.booleanValue());
			}

			EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,
					EventQueues.SESSION, false);
			if (eq != null) {
				eq.publish(new Event(CHANGE_VISIBILITY_EVENT, null,
						preferencesSessionVariableName));
			}

			filterData = alertsSettings.getFilterData();
			if (filterData != null) {
				filterData.setSource(preferencesSessionVariableName);
				eq = EventQueues.lookup(FilterSelector.CHANGE_FILTER_EVENT,
						EventQueues.SESSION, false);
				if (eq != null) {
					eq.publish(new Event(FilterSelector.CHANGE_FILTER_EVENT,
							null, filterData));
				}
			}
		} catch (IOException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_RETRIEVE_SAVED_VIEW_STATE);
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_SAVED_VIEW_STATE);
		}
		Sessions.getCurrent().removeAttribute("ViewName");
		String uri = "activityLogsView";
        uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
        Executions.sendRedirect(uri);
	}

	@Command("changeColumnVisibility")
	public void changeColumnVisibility(
			@BindingParam("checked") boolean isPicked,
			@BindingParam("columnId") String columnId) {
		ActivityLogsColumns.findColumnById(columnId).setVisible(isPicked);
		getGridPreferencesInSession().setColumnsListModelList(
				activityLogsColumnsListModelList);
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,
				EventQueues.SESSION, false);
		if (eq != null) {
			eq.publish(new Event(CHANGE_VISIBILITY_EVENT, null,
					preferencesSessionVariableName));
		}
	}

	private void applyColumnsVisibility() {
		for (ActivityLogsColumns alertsColumn : ActivityLogsColumns.values()) {
			applyColumnVisibility(alertsColumn.isVisible(),
					alertsColumn.getColumnId());
		}
	}

	public void applyColumnVisibility(Boolean isPicked, String columnId) {
		getListheader(columnId).setVisible(isPicked);
	}

	protected Map<String, Object> getCustomFilterRequest(
			Map<String, Object> filterRequest) {
		Map<String, Object> customFilterRequest = new TreeMap<>();
		customFilterRequest.putAll(filterRequest);
		//customFilterRequest.put("AlertType", alertsSource.getId());
		return customFilterRequest;
	}

	public ActivityLogsIconURLConverter getIconURLConverter() {
		return iconURLConverter;
	}

	public ActivityLogsLabelURLConverter getLabelURLConverter() {
		return labelURLConverter;
	}

	protected ListModelList<Filter> getFiltersDefinition() {
		ListModelList<Filter> filtersDefinition = new ListModelList<>();

		Filter filter;
		
		filter = new ActivityLogsViewFilter(ActivityLogsViewFilters.DATE);
		filtersDefinition.add(filter);

		filter = new ActivityLogsViewFilter(ActivityLogsViewFilters.TIME);
		filtersDefinition.add(filter);

		filter = new ActivityLogsViewFilter(
				ActivityLogsViewFilters.INSTANCE_NAME);
		filtersDefinition.add(filter);

		filter = new ActivityLogsViewFilter(ActivityLogsViewFilters.EVENT);
		filtersDefinition.add(filter);

		filter = new ActivityLogsViewFilter(ActivityLogsViewFilters.DETAILS);
		filtersDefinition.add(filter);

		return filtersDefinition;
	}

	public ListModelList<?> getActivityLogsColumnsListModelList() {
		return activityLogsColumnsListModelList;
	}

	protected ActivityLogsGridViewReport makeCommonGridViewReport() {
		ActivityLogsGridViewReport instancesEventsGridViewReport = new ActivityLogsGridViewReport(
				"Activity Logs", "", "Activity_Log");
		instancesEventsGridViewReport.setDataMapForListInstance(entitiesModel);
		return instancesEventsGridViewReport;
	}

	@Command("logsView")
	public void openInstance(@BindingParam("id") int id) {
		if (id == 1) {
			refreshEntitiesList();
		} else {
			Sessions.getCurrent().setAttribute("log_type","changeLogsView");
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("changeLogsView"));
		}
	}

	@Override
	protected Map<String, Boolean> collectColumnsVisibilityMap() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected void retrieveColumnsVisibility(
			CMSideBarViewSettings alertsSettings) {
		// TODO Auto-generated method stub

	}

}
