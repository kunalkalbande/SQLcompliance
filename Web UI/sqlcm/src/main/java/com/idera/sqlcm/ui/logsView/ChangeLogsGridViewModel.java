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
import com.idera.sqlcm.entities.CMEventFilterListData;
import com.idera.sqlcm.entities.CMFilteredChangeLogsResponse;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.entities.CMViewSettings;
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.entities.ViewNameData;
import com.idera.sqlcm.entities.ViewNameResponse;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.facade.ChangeLogsFacade;
import com.idera.sqlcm.facade.FilterFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.FilterSelector;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.dialogs.ChangeLogsPropertiesViewModel;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.ui.changeLogsView.filters.ChangeLogsViewFilter;
import com.idera.sqlcm.ui.changeLogsView.filters.ChangeLogsViewFilters;

public class ChangeLogsGridViewModel extends CommonGridViewModel {

	private static final String ZUL_URL = "~./sqlcm/prompt/DeletePrompt.zul";
	private static final Logger logger = Logger.getLogger(ChangeLogsGridViewModel.class);
	
	@Wire
    protected Listbox entitiesListBox;
    @Wire
    Paging listBoxPageId;
    
    int PAGE_SIZE = 50;
    
    @Wire
    Spinner listBoxRowsBox;
    
    private int prevPageSize;
    
    private int refreshDuration;
    
	public int getRefreshDuration() {
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}
	
	private static final String INSTANCES_ALERTS_SESSION_DATA_BEAN = "ChangeLogsSessionDataBean";
	
	private static final String CHANGE_VISIBILITY_EVENT = "changeColumnVisibility";
	
	protected AlertsSource alertsSource;
	
	protected ListModelList changeLogsColumnsListModelList;
	
	protected CMFilteredChangeLogsResponse cmFilteredChangeLogsResponse;
	
	private ChangeLogsIconURLConverter iconURLConverter;
	
	private ChangeLogsLabelURLConverter labelURLConverter;
	
	private List<CMEventFilterListData> eventFilterList;

	public List<CMEventFilterListData> getEventFilterList() {
		return eventFilterList;
	}

	public void setEventFilterList(List<CMEventFilterListData> eventFilterList) {
		this.eventFilterList = eventFilterList;
	}
	
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

		private String label;

		private int id;

		AlertsSource(String label, int id) {
			this.label = label;
			this.id = id;
		}

		public int getId() {
			return id;
		}
	}

	public ChangeLogsGridViewModel() {
		entityFacade = new ChangeLogsFacade();
		filterFacade = new FilterFacade();
		alertsSource = AlertsSource.EVENT_ALERTS;
		exportBeanClass = ChangeLogsGridViewModel.class;
		preferencesSessionVariableName = "ChangeLogsSessionDataBean";
		iconURLConverter = new ChangeLogsIconURLConverter();
		labelURLConverter = new ChangeLogsLabelURLConverter();
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
			BindUtils.postNotifyChange(null, null, ChangeLogsGridViewModel.this, "*");
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
            	value=entitiesList.size();
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
        BindUtils.postNotifyChange(null, null, ChangeLogsGridViewModel.this, "*");
    }
	
	protected void initColumnList() {
		changeLogsColumnsListModelList = getGridPreferencesInSession()
				.getChangeLogsColumnsListModelList();
		if (changeLogsColumnsListModelList == null) {
			changeLogsColumnsListModelList = new ListModelList<ChangeLogsColumns>();
			changeLogsColumnsListModelList.add(ChangeLogsColumns.DATE);
			changeLogsColumnsListModelList.add(ChangeLogsColumns.TIME);
			changeLogsColumnsListModelList
					.add(ChangeLogsColumns.LOG_SQL_SERVER);
			changeLogsColumnsListModelList.add(ChangeLogsColumns.LOG_TYPE);
			changeLogsColumnsListModelList.add(ChangeLogsColumns.LOG_USER);
			changeLogsColumnsListModelList.add(ChangeLogsColumns.LOG_INFO);

			getGridPreferencesInSession().setChangeLogsColumnsListModelList(
					changeLogsColumnsListModelList);
		}
		applyColumnsVisibility();
		BindUtils.postNotifyChange(null, null, this,
				"changeLogsColumnsListModelList");
	}

	@Override
	@Command("refreshEvents")
	public void refreshEntitiesList() {
		Map<String, Object> customFilterRequest = getCustomFilterRequest(filterRequest);
		try {
			cmFilteredChangeLogsResponse = ((ChangeLogsFacade) entityFacade)
					.getFilteredChangeLogs(customFilterRequest);
			entitiesList = (List) cmFilteredChangeLogsResponse.getAlerts();
			setEventFilterList(cmFilteredChangeLogsResponse.getEventType());
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
		BindUtils.postNotifyChange(null, null, ChangeLogsGridViewModel.this, "*");
	}

	@Command("deleteLogs")
	public void deleteChangeLogs(@BindingParam("eventId") final int eventId) throws Exception {
  		Map args = new HashMap();
  		if(Sessions.getCurrent().getAttribute("PopUpConnfirm")==null) 
  		{
  			Sessions.getCurrent().setAttribute("ruleIdForDelete", "changeLogs|"+eventId);
  			Window window = (Window) Executions.createComponents(ChangeLogsGridViewModel.ZUL_URL, null, args);
  			window.doHighlighted();
  		}
  		else
  		{
  			deleteLogs(eventId);
  			refreshEntitiesList();
  		}
	}
	
	public void deleteLogs(@BindingParam("eventId") int eventId){
		String DeleteChangeLogs = "";
		DeleteChangeLogs = "DELETE FROM [SQLcompliance].[dbo].[ChangeLog] WHERE [logId] = "+ eventId +";";
		
		 DeleteChangeLogs(DeleteChangeLogs);
	   }

	    public void DeleteChangeLogs(String DeleteChangeLogs){
	    	insertQueryData = new InsertQueryData();
	    	insertQueryData.setDataQuery(DeleteChangeLogs);
	    	try {
	    		AlertRulesFacade.insertStatusAlertRules(insertQueryData);
	    	} catch (RestException e) {
	    		WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
	    	}
	    }
	
	@Command("selectEventSource")
	public void selectEventSource(@BindingParam("id") String id) {
		alertsSource = AlertsSource.valueOf(id);
		refreshEntitiesList();
	}

	protected void subscribeToVisibilityChangeEvent() {
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,
				EventQueues.SESSION, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getData().equals(preferencesSessionVariableName)) {
					applyColumnsVisibility();
					BindUtils.postNotifyChange(null, null,
							ChangeLogsGridViewModel.this,
							"alertsColumnsListModelList");
				}
			}
		});
	}

	public CMFilteredChangeLogsResponse getCmFilteredAlertsResponse() {
		return cmFilteredChangeLogsResponse;
	}

	public String getInstancesAlertsSummary() {
		if (this.cmFilteredChangeLogsResponse != null) {
			return ELFunctions.getLabelWithParams(
					SQLCMI18NStrings.INSTANCES_ALERTS_SUMMARY,
					cmFilteredChangeLogsResponse.getTotalSevereAlerts(),
					cmFilteredChangeLogsResponse.getTotalHighAlerts(),
					cmFilteredChangeLogsResponse.getTotalMediumAlerts(),
					cmFilteredChangeLogsResponse.getTotalLowAlerts());
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
		ChangeLogsPropertiesViewModel.showEventPropertiesWindow(rowIndex,
				entitiesModel, 1);
	}

	@Command("saveViewState")
	public void saveViewState() {
		CMViewSettings settings = new CMViewSettings();
		String ViewName = (String) Sessions.getCurrent().getAttribute("ViewName");
		Sessions.getCurrent().setAttribute("View", "changeLogsView");
        settings.setViewId(preferencesSessionVariableName);
        settings.setViewName(ViewName);
		CMSideBarViewSettings alertsSettings = new CMSideBarViewSettings();
		alertsSettings
				.setFilterData(getGridPreferencesInSession().getFilters());
		Map<String, Boolean> columns = new LinkedHashMap<String, Boolean>();
		for (ChangeLogsColumns alertsColumn : ChangeLogsColumns.values()) {
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
			for (ChangeLogsColumns alertsColumn : ChangeLogsColumns.values()) {
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
		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("changeLogsView"));
	}

	@Command("changeColumnVisibility")
	public void changeColumnVisibility(
			@BindingParam("checked") boolean isPicked,
			@BindingParam("columnId") String columnId) {
		ChangeLogsColumns.findColumnById(columnId).setVisible(isPicked);
		getGridPreferencesInSession().setColumnsListModelList(
				changeLogsColumnsListModelList);
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,
				EventQueues.SESSION, false);
		if (eq != null) {
			eq.publish(new Event(CHANGE_VISIBILITY_EVENT, null,
					preferencesSessionVariableName));
		}
	}

	private void applyColumnsVisibility() {
		for (ChangeLogsColumns alertsColumn : ChangeLogsColumns.values()) {
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
		customFilterRequest.put("AlertType", alertsSource.getId());
		return customFilterRequest;
	}

	public ChangeLogsIconURLConverter getIconURLConverter() {
		return iconURLConverter;
	}

	public ChangeLogsLabelURLConverter getLabelURLConverter() {
		return labelURLConverter;
	}

	protected ListModelList<Filter> getFiltersDefinition() {
		ListModelList<Filter> filtersDefinition = new ListModelList<>();

		Filter filter;

		filter = new ChangeLogsViewFilter(ChangeLogsViewFilters.DATE);
		filtersDefinition.add(filter);

		filter = new ChangeLogsViewFilter(ChangeLogsViewFilters.TIME);
		filtersDefinition.add(filter);

		filter = new ChangeLogsViewFilter(ChangeLogsViewFilters.INSTANCE_NAME);
		filtersDefinition.add(filter);

		filter = new ChangeLogsViewFilter(ChangeLogsViewFilters.EVENT);
		filtersDefinition.add(filter);

		filter = new ChangeLogsViewFilter(ChangeLogsViewFilters.USER);
		filtersDefinition.add(filter);

		filter = new ChangeLogsViewFilter(ChangeLogsViewFilters.DESCRIPTION);
		filtersDefinition.add(filter);

		return filtersDefinition;
	}

	public ListModelList<?> getChangeLogsColumnsListModelList() {
		return changeLogsColumnsListModelList;
	}

	protected ChangeLogsGridViewReport makeCommonGridViewReport() {
		ChangeLogsGridViewReport instancesEventsGridViewReport = new ChangeLogsGridViewReport(
				"Change Logs", "", "Change_Logs");
		instancesEventsGridViewReport.setDataMapForListInstance(entitiesModel);
		return instancesEventsGridViewReport;
	}

	@Command("logsView")
	public void openInstance(@BindingParam("id") int id) {
		if (id == 1) {
			Sessions.getCurrent().setAttribute("log_type","activityLogsView");
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("activityLogsView"));
		} else {
			refreshEntitiesList();
		}
	}
	
	@Command("openInstance")
    public void openInstance(@BindingParam("instanceId") long id) {
		if(id>0)
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instanceView/" + id));
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
