package com.idera.sqlcm.ui.instancesAlerts;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.server.web.component.zul.grid.ExtListheader;
import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.entities.CMEventFilterListData;
import com.idera.sqlcm.entities.CMFilteredAlertsResponse;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.entities.ViewNameData;
import com.idera.sqlcm.entities.ViewNameResponse;
import com.idera.sqlcm.facade.AlertsFacade;
import com.idera.sqlcm.facade.FilterFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.dialogs.EventPropertiesViewModel;
import com.idera.sqlcm.ui.instancesAlerts.filters.AlertFilter;
import com.idera.sqlcm.ui.instancesAlerts.filters.AlertFilterChild;
import com.idera.sqlcm.ui.instancesAlerts.filters.AlertsFilters;
import com.idera.sqlcm.ui.instancesAlerts.filters.AlertsOptionValues;
import com.idera.sqlcm.ui.preferences.CommonGridPreferencesBean;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.utils.SQLCMConstants;

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
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.event.SortEvent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listhead;
import org.zkoss.zul.Listheader;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Spinner;

import java.util.Collection;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

public class InstancesAlertsGridViewModel extends CommonGridViewModel {

    private static final String INSTANCES_ALERTS_SESSION_DATA_BEAN = "InstancesAlertsSessionDataBean";

    private static final String CHANGE_VISIBILITY_EVENT = "changeColumnVisibility";

    @Wire
    protected Listhead entitiesListBoxHead;

    @Wire
    private Spinner listBoxRowsBox;

    @Wire
    private Paging listBoxPageId;

    private int refreshDuration;   

	private int recordCount;

    private int prevPageSize;

    private Integer pageSize = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

    protected AlertsSource alertsSource;

    protected ListModelList alertsColumnsListModelList;

    protected CMFilteredAlertsResponse cmFilteredAlertsResponse;

    private AlertsIconURLConverter iconURLConverter;

    private AlertsLabelURLConverter labelURLConverter;

	int PAGE_SIZE = 50;
	
	FilterFacade filterFacade = new FilterFacade();

	List<ViewNameData> nameList;

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

	private List<CMEventFilterListData> eventFilterList;
	public List<CMEventFilterListData> getEventFilterList() {
		return eventFilterList;
	}
	public void setEventFilterList(List<CMEventFilterListData> eventFilterList) {
		this.eventFilterList = eventFilterList;
	}
	public static enum AlertsSource {
		ALL_ALERTS(ELFunctions.getLabel(SQLCMI18NStrings.ALL_ALERTS), 4), EVENT_ALERTS(
				ELFunctions.getLabel(SQLCMI18NStrings.EVENT_ALERTS), 1), STATUS_ALERTS(
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

	public InstancesAlertsGridViewModel() {
		entityFacade = new AlertsFacade();
		alertsSource = AlertsSource.ALL_ALERTS;
		exportBeanClass = InstancesAlertsGridViewModel.class;
		preferencesSessionVariableName = INSTANCES_ALERTS_SESSION_DATA_BEAN;
		iconURLConverter = new AlertsIconURLConverter();
		labelURLConverter = new AlertsLabelURLConverter();
		ViewNameResponse viewNameResponse = new ViewNameResponse();
		try {
			viewNameResponse = FilterFacade.getViewName(preferencesSessionVariableName);
			nameList = viewNameResponse.getViewNameTable();
			//Start AJ : Null Pointer Exception on clicking Load View
			for(int i=0; i<nameList.size(); i++){
				if(nameList.get(i).getViewName().contains("_")){
					String viewNameSubString = nameList.get(i).getViewName().substring(
							0,nameList.get(i).getViewName().lastIndexOf("_"));
					nameList.get(i).setViewName(viewNameSubString);
				}
			}
			//End AJ: Null Pointer Exception on clicking Load View
			setNameList(nameList);
			BindUtils.postNotifyChange(null, null, InstancesAlertsGridViewModel.this, "*");
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
    	try{
        super.afterCompose(view);
        listBoxRowsBox.setValue(PAGE_SIZE);
        String refreshDuration= RefreshDurationFacade.getRefreshDuration();
		int refDuration=Integer.parseInt(refreshDuration);
		refDuration=refDuration*1000;
		setRefreshDuration(refDuration);
		setGridRowsCount();
        initColumnList();
        subscribeToVisibilityChangeEvent();
    	}
        catch(Exception e)
        {
        	e.getStackTrace();
        }
    }

	protected void initColumnList() {
		alertsColumnsListModelList = getGridPreferencesInSession()
				.getColumnsListModelList();
		if (alertsColumnsListModelList == null) {
			alertsColumnsListModelList = new ListModelList<AlertsColumns>();
			alertsColumnsListModelList.add(AlertsColumns.INSTANCE_NAME);
			alertsColumnsListModelList.add(AlertsColumns.DATE);
			alertsColumnsListModelList.add(AlertsColumns.TIME);
			alertsColumnsListModelList.add(AlertsColumns.LEVEL);
			alertsColumnsListModelList.add(AlertsColumns.SOURCE_RULE);
			alertsColumnsListModelList.add(AlertsColumns.EVENT);
			alertsColumnsListModelList.add(AlertsColumns.DETAIL);
			getGridPreferencesInSession().setColumnsListModelList(
					alertsColumnsListModelList);
		}
		applyColumnsVisibility();
		BindUtils.postNotifyChange(null, null, this,
				"alertsColumnsListModelList");
	}

    @Override
    @Command("refreshEvents")
    public void refreshEntitiesList() {
        Map<String, Object> customFilterRequest = getCustomFilterRequest(filterRequest);
        try {
            cmFilteredAlertsResponse = AlertsFacade.getFilteredAlerts(customFilterRequest);
            entitiesList = (List) cmFilteredAlertsResponse.getAlerts();
            setEventFilterList(cmFilteredAlertsResponse.getEventType());
            int totalRecords = cmFilteredAlertsResponse.getRecordCount();
            if (totalRecords < 0) {
                totalRecords = 0;
            }
            recordCount = totalRecords;
        } catch (RestException ex) {
            WebUtil.showErrorBox(ex, SQLCMI18NStrings.FAILED_TO_LOAD_FILTERED_ALERTS);
        }
        super.verifyEntitiesList();
        BindUtils.postNotifyChange(null, null, this, "instancesAlertsSummary");
        BindUtils.postNotifyChange(null, null, this, "entitiesModel");
        BindUtils.postNotifyChange(null, null, this, "totalSize");
		BindUtils.postNotifyChange(null, null, InstancesAlertsGridViewModel.this, "*");
    }

    @Override
    protected void initPagination() {
    	CommonGridPreferencesBean gp = getGridPreferencesInSession();
        if (gp.getGridRowsCount() > 0) {
            rowsCount = gp.getGridRowsCount();
        }
        prevPageSize = rowsCount;
        listBoxRowsBox.setValue(rowsCount);
        setGridRowsCount();
        entitiesListBox.setPaginal(listBoxPageId);
    }

	@Command("selectEventSource")
	public void selectEventSource(@BindingParam("id") String id) {
		alertsSource = AlertsSource.valueOf(id);
		refreshEntitiesList();
	}

	@Command("selectViewName")
	public void selectViewName(@BindingParam("id") String id){
		String viewNameWithPrefrences=preferencesSessionVariableName.substring(0,
        		preferencesSessionVariableName.lastIndexOf("SessionDataBean"));
		Sessions.getCurrent().setAttribute("ViewName", id +  "_" +viewNameWithPrefrences);
	}
	
	protected void subscribeToVisibilityChangeEvent() {
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,
				EventQueues.SESSION, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getData().equals(preferencesSessionVariableName)) {
					applyColumnsVisibility();
					BindUtils.postNotifyChange(null, null,
							InstancesAlertsGridViewModel.this,
							"alertsColumnsListModelList");
				}
			}
		});
	}

    public CMFilteredAlertsResponse getCmFilteredAlertsResponse() {
        return cmFilteredAlertsResponse;
    }

    public String getInstancesAlertsSummary() {
        if (this.cmFilteredAlertsResponse != null) {
            return ELFunctions.getLabelWithParams(SQLCMI18NStrings.INSTANCES_ALERTS_SUMMARY,
                cmFilteredAlertsResponse.getTotalSevereAlerts(), cmFilteredAlertsResponse.getTotalHighAlerts(),
                cmFilteredAlertsResponse.getTotalMediumAlerts(), cmFilteredAlertsResponse.getTotalLowAlerts());
        }
        return ELFunctions.getLabel(SQLCMI18NStrings.N_A);
    }

    private Listheader getListheader(String listheaderId) {
        List<Component> listHeaders = entitiesListBox.getListhead().getChildren();
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
    public void changeOpenCloseState(@ContextParam(ContextType.TRIGGER_EVENT) Event event) {
        Groupbox filterGroup = (Groupbox) event.getTarget();
        filterGroup.getCaption().setSclass(" open-" + (filterGroup.isOpen() ? "true" : "false"));
    }

    @Command("showEventPropertiesDialog")
    public void showEventProperties(@BindingParam("rowIndex") int rowIndex) {
        EventPropertiesViewModel.showEventPropertiesWindow(rowIndex, entitiesModel, 1);
    }

    protected Map<String, Boolean> collectColumnsVisibilityMap() {
        Map<String, Boolean> columns = new LinkedHashMap<String, Boolean>();
        for (AlertsColumns alertsColumn : AlertsColumns.values()) {
            columns.put(alertsColumn.getColumnId(), alertsColumn.isVisible());
        }
        return columns;
    }

    protected void retrieveColumnsVisibility(CMSideBarViewSettings alertsSettings) {
        Map<String, Boolean> columns = alertsSettings.getColumnVisibility();
        for (AlertsColumns alertsColumn : AlertsColumns.values()) {
            alertsColumn.setVisible(columns.get(alertsColumn.getColumnId()).booleanValue());
        }
    }

    @Command("changeColumnVisibility")
    public void changeColumnVisibility(@BindingParam("checked") boolean isPicked, @BindingParam("columnId") String columnId) {
        AlertsColumns.findColumnById(columnId).setVisible(isPicked);
        getGridPreferencesInSession().setColumnsListModelList(alertsColumnsListModelList);
        EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT, EventQueues.SESSION, false);
        if (eq != null) {
            eq.publish(new Event(CHANGE_VISIBILITY_EVENT, null, preferencesSessionVariableName));
        }
    }

    private void applyColumnsVisibility() {
        for (AlertsColumns alertsColumn : AlertsColumns.values()) {
            applyColumnVisibility(alertsColumn.isVisible(), alertsColumn.getColumnId());
        }
    }

    public void applyColumnVisibility(Boolean isPicked, String columnId) {
        getListheader(columnId).setVisible(isPicked);
    }

    protected Map<String, Object> getCustomFilterRequest(Map<String, Object> filterRequest) {
        Map<String, Object> customFilterRequest = new TreeMap<>();
        customFilterRequest.putAll(filterRequest);
        customFilterRequest.put("AlertType", alertsSource.getId());
        return customFilterRequest;
    }

    public AlertsIconURLConverter getIconURLConverter() {
        return iconURLConverter;
    }

    public AlertsLabelURLConverter getLabelURLConverter() {
        return labelURLConverter;
    }

    protected ListModelList<Filter> getFiltersDefinition() {
        ListModelList<Filter> filtersDefinition = new ListModelList<>();

        Filter filter;

        filter = new AlertFilter(AlertsFilters.INSTANCE_NAME);
        filtersDefinition.add(filter);

        filter = new AlertFilter(AlertsFilters.DATE);
        filtersDefinition.add(filter);

        filter = new AlertFilter(AlertsFilters.TIME);
        filtersDefinition.add(filter);

        filter = new AlertFilter(AlertsFilters.LEVEL);
        filter.addFilterChild(new AlertFilterChild(AlertsOptionValues.SEVERE_STATUS));
        filter.addFilterChild(new AlertFilterChild(AlertsOptionValues.HIGH_STATUS));
        filter.addFilterChild(new AlertFilterChild(AlertsOptionValues.MEDIUM_STATUS));
        filter.addFilterChild(new AlertFilterChild(AlertsOptionValues.LOW_STATUS));
        filtersDefinition.add(filter);

        filter = new AlertFilter(AlertsFilters.SOURCE_RULE);
        filtersDefinition.add(filter);

        filter = new AlertFilter(AlertsFilters.EVENT);
        filtersDefinition.add(filter);

        filter = new AlertFilter(AlertsFilters.DETAILS);
        filtersDefinition.add(filter);

        return filtersDefinition;
    }

    @Override
    protected void applyFilter() {
        super.applyFilter();
    }

    public ListModelList<?> getAlertsColumnsListModelList() {
        return alertsColumnsListModelList;
    }

    protected InstancesAlertsGridViewReport makeCommonGridViewReport() {
        InstancesAlertsGridViewReport instancesEventsGridViewReport = new InstancesAlertsGridViewReport("Instances Alerts", "", "Alerts");
        instancesEventsGridViewReport.setDataMapForListInstance(entitiesModel);
        return instancesEventsGridViewReport;
    }

    @Command("openInstance")
    public void openInstance(@BindingParam("instanceId") long id) {
    	if(id>0)
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instanceView/" + id));
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
    
    @Override
    protected void subscribeForSortEvent() {
        Collection<Component> heads = entitiesListBox.getChildren();
        for (Component headerComponent : heads) {
            headerComponent.addEventListener(Events.ON_SORT, new EventListener<SortEvent>() {
                public void onEvent(SortEvent event) throws Exception {
                    String sortParam = ((ExtListheader) event.getTarget()).getValue();
                    if (sortParam == null || sortParam.trim().isEmpty()) {
                        throw new RuntimeException(" Invalid column value that is used as sort parameter! ");
                    }
                    if (event.isAscending()) {
                    } else {
                    }
                    refreshEntitiesList();
                }
            });
        }
    }

    @Command("setGridRowsCount")
    public void setGridRowsCount() {
        try {
            int tmpPageSize = listBoxRowsBox.getValue();
            if (tmpPageSize > 100) {
                Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
                    listBoxRowsBox, "end_center", 3000);
                tmpPageSize = 100;
                listBoxRowsBox.setValue(tmpPageSize);
            }
            listBoxPageId.setPageSize(tmpPageSize);
            prevPageSize = tmpPageSize;
            pageSize = tmpPageSize;
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
        refreshEntitiesList();
        BindUtils.postNotifyChange(null, null, InstancesAlertsGridViewModel.this, "*");
    }
    
    @Command("instanceAlertsView")
    public void openInstance(@BindingParam("id") int id) {
    	if(id==1){
    		refreshEntitiesList(); 	
        }
    	else
    	{
    		Sessions.getCurrent().setAttribute("alert_type","instancesAlertsRule");
    		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instancesAlertsRule")); 		
    	}
    } 
}
