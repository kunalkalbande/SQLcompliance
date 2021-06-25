package com.idera.sqlcm.common.grid;

import com.idera.common.rest.JSONHelper;
import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.entities.CMViewSettings;
import com.idera.sqlcm.facade.FilterFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.FilterSelector;
import com.idera.sqlcm.ui.components.VisualFilter;
import com.idera.sqlcm.ui.components.filter.FilterData;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.dialogs.ColumnSearchViewModel;
import com.idera.sqlcm.ui.preferences.CommonGridPreferencesBean;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.utils.SQLCMConstants;

import net.sf.jasperreports.engine.JRException;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
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
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.event.SortEvent;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Column;
import org.zkoss.zul.Div;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Window;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;

import java.io.IOException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Collection;
import java.util.Comparator;
import java.util.Date;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

public abstract class CommonGridViewModel {

    protected CommonFacade entityFacade;
    protected Class exportBeanClass;
    protected List<CMEntity> entitiesList;
    protected ListModelList<CMEntity> entitiesModel;
    protected ListModelList<CMEntity> entitiesModelReport;
    protected ListModelList<Filter> filtersModel;
    protected FilterData filterData;
    protected Map<String, Object> filterRequest = new TreeMap<>();
    protected String preferencesSessionVariableName = CommonGridPreferencesBean.SESSION_VARIABLE_NAME;
    protected int rowsCount = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;
    protected static final String CHANGE_COLUMN_VISIBILITY_EVENT = "changeColumnVisibility";
    protected CMSideBarViewSettings sideBarSettings;
    protected CMViewSettings cmViewSettings;
    public static final String ZUL_URL_saveViewName =  "~./sqlcm/dialogs/saveView.zul";

    public int refreshDuration; // SQLCM 5.4 SQLCM-2172 start

    public int getRefreshDuration() {
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}
    
    @Wire
    protected Listbox entitiesListBox;
    @Wire
    protected Paging listBoxPageId;
    
    @Wire
    protected Spinner listBoxRowsBox;
    
    @Wire
    Label filteredByLabel;
    @Wire
    Div currentlySelectedFiltersComponentDiv;
    @Wire
    protected Label errorLabel;
    @Wire
    FilterSelector filterSelector;    
    

    public int fileSize;
    
	public int getFileSize() {
		return fileSize;
	}

	public void setFileSize(int fileSize) {
		this.fileSize = fileSize;
	}

	private int prevPageSize;

    protected ListModelList<Filter> getFiltersDefinition() {
        return new ListModelList<>();
    }

    public ListModelList<CMEntity> getEntitiesModel() {
        return entitiesModel;
    }

    public ListModelList<Filter> getFiltersModel() {
        return filtersModel;
    }

    public List<CMEntity> getEntitiesList() {
        return entitiesList;
    }

    public void setEntitiesList(List<CMEntity> entitiesList) {
        this.entitiesList = entitiesList;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        try{ 
        entitiesListBox.setNonselectableTags("<div><tr><td><a><img>");
        initFiltersAndGrid();
        refreshEntitiesList();
        applyGridPreferences();

        subscribeToFilterChangeEvent();
        subscribeForSortEvent();
        initPagination();
               	
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
 
    public void afterComposeReport(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        initPagination();
    }

    @Command
    public void onCreateFilterGroupBox(@ContextParam(ContextType.TRIGGER_EVENT) Event event) {
        Groupbox filterGroup = (Groupbox) event.getTarget();
        filterGroup.getCaption().setSclass(" open-" + (filterGroup.isOpen() ? "true" : "false"));
    }

    protected CommonGridPreferencesBean getGridPreferencesInSession() {
        return PreferencesUtil.getInstance().getGridPreferencesInSession(preferencesSessionVariableName);
    }

    protected void initPagination() {
    	if(preferencesSessionVariableName!=null 
    			&& preferencesSessionVariableName.equals("SQLcmInstancesSessionDataBean")){
    		rowsCount = SQLCMConstants.DEFAULT_ROW_GRID_COUNT_INSTANCE;
    	}
    	CommonGridPreferencesBean gp = getGridPreferencesInSession();
        if (gp.getGridRowsCount() > 0) {
            rowsCount = gp.getGridRowsCount();
        }
        prevPageSize = rowsCount;
        listBoxRowsBox.setValue(rowsCount);   
        setGridRowsCount();
        entitiesListBox.setPaginal(listBoxPageId);
    }

    protected void subscribeForSortEvent() {
        Collection<Component> heads = entitiesListBox.getHeads();
        for (Component headerComponent : heads) {
            headerComponent.addEventListener(Events.ON_SORT, new EventListener<SortEvent>() {
                public void onEvent(SortEvent event) throws Exception {
                    PreferencesUtil.getInstance()
                            .setGridSortingPreferencesInSession(preferencesSessionVariableName, event.getTarget().getId(),
                                    event.isAscending());
                }
            });
        }
    }

    protected void applyGridPreferences() {
        CommonGridPreferencesBean gp = getGridPreferencesInSession();
        if (gp.getSortedColumnId() != null && gp.getSortedColumnId().length() > 0) {

            Collection<Component> heads = entitiesListBox.getHeads();
            for (Component headerComponent : heads) {
                if (headerComponent.getId().equals(gp.getSortedColumnId())) {
                    Comparator<CMEntity> columnComparator = (Comparator<CMEntity>) (gp.isAscendingSortDirection()
                            ? ((Column) headerComponent).getSortAscending() : ((Column) headerComponent).getSortDescending());
                    entitiesModel.sort(columnComparator, gp.isAscendingSortDirection());
                    ((Column) headerComponent).setSortDirection(gp.isAscendingSortDirection() ? "ascending" : "descending");
                    break;
                }
            }
        }
    }
    @Command("refreshEvents")
    public void refreshEntitiesList() {
        entitiesList = entityFacade.getAllEntities(filterRequest);
        verifyEntitiesList();
        int totalRecords = entitiesList.size();
        if (totalRecords < 0) {
            totalRecords = 0;
        }
        setFileSize(totalRecords);
        BindUtils.postNotifyChange(null, null, this, "entitiesModel");
        BindUtils.postNotifyChange(null, null, this, "fileSize");
    }

    protected void verifyEntitiesList() {
        if (entitiesList == null) {
            entitiesModel = new ListModelList<>();
            errorLabel.getParent().setVisible(true);
        } else {
            entitiesModel = new ListModelList<>(entitiesList);
            entitiesModel.setMultiple(true);
            errorLabel.getParent().setVisible(false);
        }
    }

    protected void initFiltersAndGrid() {
        filtersModel = getFiltersDefinition();
        filterData = getGridPreferencesInSession().getFilters();
        applyFilter();
        BindUtils.postNotifyChange(null, null, this, "filtersModel");
    }

    protected void subscribeToFilterChangeEvent() {
        EventQueue<Event> eq = EventQueues.lookup(FilterSelector.CHANGE_FILTER_EVENT, EventQueues.SESSION, true);
        eq.subscribe(new EventListener<Event>() {
            public void onEvent(Event event) throws Exception {
                if (((FilterData) event.getData()).getSource().equals(preferencesSessionVariableName)) {
                    filterData = (FilterData) event.getData();
                    applyFilter();
                }
            }
        });
    }

    protected void applyFilter() {
        if (filterData != null) PreferencesUtil.getInstance().setGridFilterPreferencesInSession(preferencesSessionVariableName, filterData);
        applyFilterData();
        refreshEntitiesList();
        filterSelector.setSource(preferencesSessionVariableName);
        filterSelector.setFilters(filtersModel);
        updateCurrentlyAppliedFilters();
    }

    protected void applyFilterData() {
        filterRequest.clear();
        if (filterData != null && filterData.size() > 0) {
            for (Filter filter : filtersModel) {
                if (filterData.get(filter.getColumnId()) != null) {
                    filter.setSetValue(filterData.get(filter.getColumnId()));
                    switch (filter.getFilterType().getInputType()) {
                        case OPTIONS:
                        case TEXT:
                            filterRequest.put(filter.getFilterId(), filterData.get(filter.getColumnId()));
                            break;
                        case COMBO:
                            filterRequest.put(filter.getFilterId(), filterData.get(filter.getColumnId()));
                            break;
                        case DIGIT_RANGE:
                            String[] digitValues = filterData.get(filter.getColumnId()).split(SQLCMConstants.FILTER_RANGE_VALUES_SEP_REGEXP);
                            if (digitValues.length == 2) {
                                filterRequest.put(filter.getFilterId() + "From", Integer.valueOf(digitValues[0]));
                                filterRequest.put(filter.getFilterId() + "To", Integer.valueOf(digitValues[1]));
                            }
                            break;
                        case DATE_RANGE:
                            String[] dateValues = filterData.get(filter.getColumnId()).split(SQLCMConstants
                                    .FILTER_RANGE_VALUES_SEP_REGEXP);
                            SimpleDateFormat dateFormat = new SimpleDateFormat(SQLCMConstants.FILTER_DATE_FORMAT);
                            if (dateValues.length == 2) {
                                try {
                                    filterRequest.put(filter.getFilterId() + "From", SQLCMConstants.DATE_FILTER_REQUEST_PREFIX +
                                            String.valueOf(dateFormat.parse(dateValues[0]).getTime())
                                            + SQLCMConstants.DATE_FILTER_REQUEST_SUFIX);
                                    filterRequest.put(filter.getFilterId() + "To", SQLCMConstants.DATE_FILTER_REQUEST_PREFIX +
                                            String.valueOf(dateFormat.parse(dateValues[1]).getTime() + 86399999)
                                            + SQLCMConstants.DATE_FILTER_REQUEST_SUFIX);
                                } catch (ParseException e) {
                                    e.printStackTrace();
                                }
                            }
                            break;
                        case TIME_RANGE:
                            String[] timeValues = filterData.get(filter.getColumnId()).split(SQLCMConstants
                                .FILTER_RANGE_VALUES_SEP_REGEXP);
                            SimpleDateFormat timeFormat = new SimpleDateFormat(SQLCMConstants.FILTER_DATE_TIME_FORMAT);
                            if (timeValues.length == 2) {
                                try {
                                    SimpleDateFormat transformTimeFormat = new SimpleDateFormat(SQLCMConstants.FILTER_DATE_FORMAT);
                                    filterRequest.put(filter.getFilterId() + "From", SQLCMConstants.TIME_FILTER_REQUEST_PREFIX +
                                        String.valueOf(timeFormat.parse(transformTimeFormat.format(new Date())+ " " + timeValues[0]).getTime())
                                        + SQLCMConstants.TIME_FILTER_REQUEST_SUFIX);
                                    filterRequest.put(filter.getFilterId() + "To", SQLCMConstants.TIME_FILTER_REQUEST_PREFIX +
                                        String.valueOf(timeFormat.parse(transformTimeFormat.format(new Date()) + " " + timeValues[1]).getTime() + 999)
                                        + SQLCMConstants.TIME_FILTER_REQUEST_SUFIX);
                                } catch (ParseException e) {
                                    e.printStackTrace();
                                }
                            }
                            break;
                    }
                }
            }
        }
        BindUtils.postNotifyChange(null, null, this, "filtersModel");
    }

    private void updateCurrentlyAppliedFilters() {
        currentlySelectedFiltersComponentDiv.getChildren().clear();
        for (Filter filter : filtersModel) {
            if (filter.getSetValue() != null && !filter.getSetValue().equals("")) {
                VisualFilter visualFilter;
                switch (filter.getFilterType().getInputType()) {
                    case TEXT:
                        for (String filterChildValue : filter.getTextValues()) {
                            visualFilter = new VisualFilter(filter, filterChildValue);
                            updateVisualFilter(visualFilter);
                        }
                        break;
                    case COMBO:
                        for (String filterChildValue : filter.getTextValues()) {
                            visualFilter = new VisualFilter(filter, filterChildValue);
                            updateVisualFilter(visualFilter);
                        }
                        break;
                    case OPTIONS:
                        for (String filterChildValue : filter.getTextValues()) {
                            visualFilter = new VisualFilter(filter, filter.getFilterChildForIntValue(filterChildValue).getValue());
                            updateVisualFilter(visualFilter);
                        }
                        break;
                    case DIGIT_RANGE:
                        visualFilter = new VisualFilter(filter, filter.getSetValue());
                        updateVisualFilter(visualFilter);
                        break;
                    case TIME_RANGE:
                    case DATE_RANGE:
                        visualFilter = new VisualFilter(filter, filter.getSetValue());
                        updateVisualFilter(visualFilter);
                        break;
                }
            }
        }
        filteredByLabel.setVisible(!currentlySelectedFiltersComponentDiv.getChildren().isEmpty());
    }

    private void updateVisualFilter(VisualFilter visualFilter) {
        visualFilter.setOnClickEventListener(getCurrentFilterOnClickEventListener());
        currentlySelectedFiltersComponentDiv.appendChild(visualFilter);
    }

    private EventListener<Event> getCurrentFilterOnClickEventListener() {
        return new EventListener<Event>() {
            @Override
            public void onEvent(Event event) throws Exception {
                VisualFilter visualFilter = VisualFilter.getVisualFilterFromEvent(event);
                if ((visualFilter != null) && (visualFilter.getFilter() != null) && (visualFilter.getFilterValue() != null)) {
                    Filter filter = visualFilter.getFilter();
                    String filterValue = visualFilter.getFilterValue();
                    filterSelector.removeSpecificFilter(filter, filterValue);
                    currentlySelectedFiltersComponentDiv.removeChild(event.getTarget());
                    filteredByLabel.setVisible(!currentlySelectedFiltersComponentDiv.getChildren().isEmpty());
                }
            }
        };
    }

    @Command("setGridRowsCount")
    public void setGridRowsCount() {
        try {
        	 int value=0;
        	if(entitiesList !=null && !entitiesList.isEmpty())
            {                       	
            		value=entitiesList.size();
            		setFileSize(value);           	
            }
            else if(entitiesModel!= null && !entitiesModel.isEmpty())
            {           	
            		value=entitiesModel.size();
            		setFileSize(value);
            }
            else
            {
            	setFileSize(0);	
            }
        	
            int pageSize = listBoxRowsBox.getValue();
            if (pageSize > 100) {
                Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
                        listBoxRowsBox, "end_center", 3000);
                pageSize = 100;
                listBoxRowsBox.setValue(pageSize);
            }
            listBoxPageId.setPageSize(pageSize);
            prevPageSize = pageSize;
           
            
			

        } catch (WrongValueException exp) {
            listBoxPageId.setPageSize(prevPageSize);
        }
        PreferencesUtil.getInstance().setGridPagingPreferencesInSession(preferencesSessionVariableName, listBoxPageId.getPageSize());
        BindUtils.postNotifyChange(null, null, CommonGridViewModel.this, "*");
        
    }

    @Command("exportToPdf")
    public void exportToPdf() throws JRException, IOException {
    if(makeCommonGridViewReport()!=null)
        makeCommonGridViewReport().generatePDFReport();
    }

    @Command("exportToExcel")
    public void exportToExcel() throws JRException, IOException {
	if(makeCommonGridViewReport()!=null)
        makeCommonGridViewReport().generateXLSReport();
    }

    @Command("exportToXml")
    public void exportToXml() throws JRException, IOException, TransformerException, ParserConfigurationException {
        makeCommonGridViewReport().generateXMLReport();
    }

    protected abstract CommonGridViewReport makeCommonGridViewReport();

    @Command("saveViewState")
    public void saveViewState() {
    	CMViewSettings settings = new CMViewSettings();
        String ViewName = (String) Sessions.getCurrent().getAttribute("ViewName");
		Sessions.getCurrent().setAttribute("View", "instancesAlerts");
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
    public void SaveViewName() {
        CMViewSettings settings = new CMViewSettings();
        String ViewName = (String) Sessions.getCurrent().getAttribute("ViewName");
        settings.setViewId(preferencesSessionVariableName);
        settings.setViewName(ViewName);
        try {
        	settings = (CMViewSettings) Sessions.getCurrent().getAttribute("settings");
        	//Start 5.3.1 Null Pointer Exception on clicking Load View
            String viewNameWithPrefrences=ViewName + "_" + settings.getViewId().substring(0,
            		settings.getViewId().lastIndexOf("SessionDataBean"));
            //End 5.3.1 Null Pointer Exception on clicking Load View
        	settings.setViewName(viewNameWithPrefrences);
            FilterFacade.saveViewSettings(settings);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_SAVE_VIEW_STATE);
        }
    }

    protected CMSideBarViewSettings getSideBarViewSettings() {
        return new CMSideBarViewSettings();
    }

    protected void parseSideBarViewSettings() throws IOException {
        sideBarSettings = JSONHelper.deserializeFromJson(cmViewSettings.getFilter(), new com.fasterxml.jackson.core.type.TypeReference<CMSideBarViewSettings>() {
        });
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
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instancesAlerts"));
    }

    protected abstract Map<String, Boolean> collectColumnsVisibilityMap();

    protected abstract void retrieveColumnsVisibility(CMSideBarViewSettings alertsSettings);
}
