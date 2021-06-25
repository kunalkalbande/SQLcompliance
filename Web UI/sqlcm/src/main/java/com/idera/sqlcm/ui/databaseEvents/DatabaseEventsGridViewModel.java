package com.idera.sqlcm.ui.databaseEvents;

import com.idera.common.rest.JSONHelper;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.common.grid.EventsGridViewModel;
import com.idera.sqlcm.entities.CMAuditedDatabase;
import com.idera.sqlcm.entities.CMDatabaseAuditingRequest;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.entities.CMViewSettings;
import com.idera.sqlcm.entities.ViewNameData;
import com.idera.sqlcm.entities.ViewNameResponse;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.EventsFacade;
import com.idera.sqlcm.facade.FilterFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.FilterSelector;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.components.filter.model.FilterType;
import com.idera.sqlcm.ui.databaseEvents.filters.EventFilter;
import com.idera.sqlcm.ui.databaseEvents.filters.EventFilterChild;
import com.idera.sqlcm.ui.databaseEvents.filters.EventsFilters;
import com.idera.sqlcm.ui.databaseEvents.filters.EventsOptionFilterValues;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;
import java.io.IOException;
import java.util.*;

public class DatabaseEventsGridViewModel extends EventsGridViewModel {

    private static final Logger logger = Logger.getLogger(DatabaseEventsGridViewModel.class);

    private static final String DATABASE_EVENTS_SESSION_DATA_BEAN = "DatabaseEventsSessionDataBean";

    protected CMAuditedDatabase database;
    public int refreshDuration;
    List<ViewNameData> nameList;
    
    private int prevPageSize;
    
    public int getRefreshDuration() {
		return refreshDuration;
	}

    public List<ViewNameData> getNameList() {
		return nameList;
	}
	public void setNameList(List<ViewNameData> nameList) {
		this.nameList = nameList;
	}
    
	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}
	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        super.afterCompose(view);
        try{
			String refreshDuration= RefreshDurationFacade.getRefreshDuration();
			int refDuration=Integer.parseInt(refreshDuration);
			refDuration=refDuration*1000;
			setRefreshDuration(refDuration);
        }
        catch(Exception e)
        {
        	e.getStackTrace();
        }
    }

    public DatabaseEventsGridViewModel() {
    }

    
    @Init
    public void init() {
        entityFacade = new EventsFacade();
        eventsSource = EventsSource.AUDIT_EVENTS;
        exportBeanClass = DatabaseEventsGridViewModel.class;
        preferencesSessionVariableName = DATABASE_EVENTS_SESSION_DATA_BEAN;
        try {
            Long instanceId = Utils.parseInstanceIdArg();
            Long databaseId = Utils.parseDatabaseIdArg();
            instance = InstancesFacade.getInstanceDetails(instanceId);
            database = DatabasesFacade.getDatabaseDetails(instanceId.toString(), databaseId.toString());
        } catch (RestException ex) {
            WebUtil.showErrorBox(ex, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
        }
        ViewNameResponse viewNameResponse = new ViewNameResponse();
		try {
			viewNameResponse = FilterFacade.getViewName(preferencesSessionVariableName);
			nameList = viewNameResponse.getViewNameTable();
			for(int i=0; i<nameList.size(); i++){
				if(nameList.get(i).getViewName().contains("_")){
					String viewNameSubString = nameList.get(i).getViewName().substring(
							0,nameList.get(i).getViewName().lastIndexOf("_"));
					nameList.get(i).setViewName(viewNameSubString);
				}
			}
			setNameList(nameList);
			BindUtils.postNotifyChange(null, null, DatabaseEventsGridViewModel.this, "*");
		} catch (RestException e) {
			e.printStackTrace();
		}
    }

    protected void initColumnList() {
        eventsColumnsListModelList = getGridPreferencesInSession().getColumnsListModelList();
        if (eventsColumnsListModelList == null) {
            eventsColumnsListModelList = new ListModelList<EventsColumns>();
            for (EventsColumns eventColumn : EventsColumns.values()) {
                if (eventColumn.isAllowedToDisable()) {
                    eventsColumnsListModelList.add(eventColumn);
                }
            }
        }
        applyColumnsVisibility();
        BindUtils.postNotifyChange(null, null, this, "eventsColumnsListModelList");
    }

    public CMAuditedDatabase getDatabase() {
        return database;
    }

    protected void subscribeToVisibilityChangeEvent() {
        EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT, EventQueues.SESSION, true);
        eq.subscribe(new EventListener<Event>() {
            public void onEvent(Event event) throws Exception {
                if (event.getData().equals(preferencesSessionVariableName)) {
                    applyColumnsVisibility();
                    BindUtils.postNotifyChange(null, null, DatabaseEventsGridViewModel.this, "eventsColumnsListModelList");
                }
            }
        });
    }

    @Command("changeColumnVisibility")
    public void changeColumnVisibility(@BindingParam("checked") boolean isPicked, @BindingParam("columnId") String columnId) {
        EventsColumns.findColumnById(columnId).setVisible(isPicked);
        getGridPreferencesInSession().setColumnsListModelList(eventsColumnsListModelList);
        EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT, EventQueues.SESSION, false);
        if (eq != null) {
            eq.publish(new Event(CHANGE_VISIBILITY_EVENT, null, preferencesSessionVariableName));
        }
    }

    protected void applyColumnsVisibility() {
        for (EventsColumns eventsColumn : EventsColumns.values()) {
            applyColumnVisibility(eventsColumn.isVisible(), eventsColumn.getColumnId());
        }
    }

    protected ListModelList<Filter> getFiltersDefinition() {
        ListModelList<Filter> filtersDefinition = new ListModelList<>();

        Filter filter;

        for (EventsFilters eventsFilter : EventsFilters.values()) {
            filter = new EventFilter(eventsFilter);
            if (!eventsFilter.getFilterType().equals(FilterType.OPTIONS)) {
                filtersDefinition.add(filter);
            } else if (filter.getFilterId() == EventsFilters.ACCESS_CHECK.getFilterId()) {
                filter.addFilterChild(new EventFilterChild(EventsOptionFilterValues.ACCESS_CHECK_PASSED));
                filter.addFilterChild(new EventFilterChild(EventsOptionFilterValues.ACCESS_CHECK_FAILED));
                filtersDefinition.add(filter);
            } else if (filter.getFilterId() == EventsFilters.PRIVILEGED_USER.getFilterId()) {
                filter.addFilterChild(new EventFilterChild(EventsOptionFilterValues.PRIVILEGED_USER_YES));
                filter.addFilterChild(new EventFilterChild(EventsOptionFilterValues.PRIVILEGED_USER_NO));
                filtersDefinition.add(filter);
            }
        }

        return filtersDefinition;
    }

    public ListModelList<?> getEventsColumnsListModelList() {
        return eventsColumnsListModelList;
    }

    protected CommonGridViewReport makeCommonGridViewReport() {
        DatabaseEventsGridViewReport databaseEventsGridViewReport = new DatabaseEventsGridViewReport("Database Events", "", "Events");
        databaseEventsGridViewReport.setDataMapForListInstance(entitiesModel);
        return databaseEventsGridViewReport;
    }

    protected Map<String, Object> getCustomFilterRequest(Map<String, Object> filterRequest) {
        Map<String, Object> customFilterRequest = super.getCustomFilterRequest(filterRequest);
        customFilterRequest.put("DatabaseId", database.getId());
        return customFilterRequest;
    }

    @Command("changeAuditing")
    public void changeAuditing() {
        CMDatabaseAuditingRequest cmDatabaseAuditingRequest = new CMDatabaseAuditingRequest();
        cmDatabaseAuditingRequest.setDatabaseIdList(new ArrayList<Long>(Arrays.<Long>asList(database.getId())));
        cmDatabaseAuditingRequest.setEnable(!database.isEnabled());
        try {
            DatabasesFacade.changeAuditingForDatabase(cmDatabaseAuditingRequest);
            database.setIsEnabled(!database.isEnabled());
            BindUtils.postNotifyChange(null, null, this, "database");
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
        }
    }

    @Command("removeDatabase")
    public void removeDatabase() {
        if (WebUtil.showConfirmationBoxWithIcon(SQLCMI18NStrings.DATABASE_REMOVE_MESSAGE, SQLCMI18NStrings.DATABASE_REMOVE_TITLE,
        		"~./sqlcm/images/high-16x16.png", true, (Object) null)) {
            try {
                Map databaseMap = new HashMap<>();
                databaseMap.put("databaseId", database.getId());
                DatabasesFacade.removeDatabase(databaseMap);
                Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("index"));
            } catch (RestException ex) {
                WebUtil.showErrorBox(ex, SQLCMI18NStrings.DATABASE_FAILED_DELETE);
            }
        }
    }

    protected Map<String, Boolean> collectColumnsVisibilityMap() {
        Map<String, Boolean> columns = new LinkedHashMap<String, Boolean>();
        for (EventsColumns eventsColumns : EventsColumns.values()) {
            columns.put(eventsColumns.getColumnId(), eventsColumns.isVisible());
        }
        return columns;
    }

    protected void retrieveColumnsVisibility(CMSideBarViewSettings alertsSettings) {
        Map<String, Boolean> columns = alertsSettings.getColumnVisibility();
        for (EventsColumns eventsColumns : EventsColumns.values()) {
            eventsColumns.setVisible(columns.get(eventsColumns.getColumnId()).booleanValue());
        }
    }
    
    @Command("saveViewState")
	public void saveViewState() {
    	CMViewSettings settings = new CMViewSettings();
        String ViewName = (String) Sessions.getCurrent().getAttribute("ViewName");
        Sessions.getCurrent().setAttribute("View", "databaseEventsView/" + instance.getId()  + "/" + database.getId());
        settings.setViewId(preferencesSessionVariableName);
        settings.setViewName(ViewName);
        try {
            sideBarSettings = getSideBarViewSettings();
            sideBarSettings.setFilterData(getGridPreferencesInSession().getFilters());
            sideBarSettings.setColumnVisibility(collectColumnsVisibilityMap());
            settings.setFilter(JSONHelper.serializeToJson(sideBarSettings));
            Sessions.getCurrent().setAttribute("settings",settings);
        } catch (IOException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_PROCESS_VIEW_STATE_FOR_SAVING);
        } 
    Window window = (Window) Executions.createComponents(
    		CommonGridViewModel.ZUL_URL_saveViewName, null,
			null);
	window.doHighlighted();
    }
    
    @Command("loadViewState")
    public void loadViewState() {
        try {
        	String viewName =  (String) Sessions.getCurrent().getAttribute("ViewName");
            cmViewSettings = FilterFacade.getViewSettings(viewName);
            if (cmViewSettings.getFilter() == null) {
                WebUtil.showInfoBox(SQLCMI18NStrings.YOU_DO_NOT_HAVE_A_FAVORITE_FILTER_SET);
                return;
            }
            parseSideBarViewSettings();
            retrieveColumnsVisibility(sideBarSettings);

            EventQueue<Event> eq = EventQueues.lookup(CHANGE_COLUMN_VISIBILITY_EVENT, EventQueues.SESSION, false);
            if (eq != null) {
                eq.publish(new Event(CHANGE_COLUMN_VISIBILITY_EVENT, null, preferencesSessionVariableName));
            }

            filterData = sideBarSettings.getFilterData();
            filtersModel = getFiltersDefinition();
            if (filterData != null) {
                filterData.setSource(preferencesSessionVariableName);
                getGridPreferencesInSession().setFilters(filterData);
                eq = EventQueues.lookup(FilterSelector.CHANGE_FILTER_EVENT, EventQueues.SESSION, false);
                if (eq != null) {
                    eq.publish(new Event(FilterSelector.CHANGE_FILTER_EVENT, null, filterData));
                }
            }
        } catch (IOException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_PROCESS_LOADED_VIEW_STATE);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_SAVED_VIEW_STATE);
        }
        Sessions.getCurrent().removeAttribute("ViewName");
      Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("databaseEventsView/" + instance.getId() + "/" + database.getId()));
    }
    
    @Command("selectViewName")
   	public void selectViewName(@BindingParam("id") String id){
   		String viewNameWithPrefrences=preferencesSessionVariableName.substring(0,
           		preferencesSessionVariableName.lastIndexOf("SessionDataBean"));
   		Sessions.getCurrent().setAttribute("ViewName", id +  "_" +viewNameWithPrefrences);
   }    
}
