package com.idera.sqlcm.common.grid;

import com.idera.common.rest.JSONHelper;
import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMArchive;
import com.idera.sqlcm.entities.CMArchivedDatabase;
import com.idera.sqlcm.entities.CMBeforeAfterDataEntity;
import com.idera.sqlcm.entities.CMBeforeAfterDataEventsResponse;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEventFilterListData;
import com.idera.sqlcm.entities.CMExtendedSideBarViewSettings;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.EventsFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.ArchivePropertiesViewModel;
import com.idera.sqlcm.ui.dialogs.AttachArchiveViewModel;
import com.idera.sqlcm.ui.dialogs.EventPropertiesViewModel;
import com.idera.sqlcm.ui.instanceEvents.EventsColumns;
import com.idera.sqlcm.ui.instanceEvents.InstanceEventsGridViewModel;
import com.idera.sqlcm.ui.preferences.CommonGridPreferencesBean;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.utils.SQLCMConstants;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;

import com.idera.ccl.*;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.event.SortEvent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Listhead;
import org.zkoss.zul.Listheader;
import org.zkoss.zul.Messagebox;

import java.io.IOException;
import java.util.Collection;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

abstract public class EventsGridViewModel extends CommonGridViewModel implements ArchivePropertiesViewModel.DialogListener {

    private static Logger logger;

    protected CMInstance instance;

    protected EventsSource eventsSource;

    protected ListModelList eventsColumnsListModelList;

    protected ListModelList beforeAfterDataTables = new ListModelList();

    private static final long ALL_ENTITIES_BAD = -2147483648;

    protected long selectedTablesBAD = ALL_ENTITIES_BAD;

    protected ListModelList beforeAfterDataColumns = new ListModelList();

    protected long selectedColumnsBAD = ALL_ENTITIES_BAD;

    protected ListModelList instanceArchivedDatabases = new ListModelList();

    protected String selectedArchivedDatabase;

    List<CMArchivedDatabase> cmArchivedDatabases;

    protected static final String CHANGE_VISIBILITY_EVENT = "changeColumnVisibility";

    @Wire
    Groupbox archiveGroupBox;

    @Wire
    protected Listhead entitiesListBoxHead;

    @Wire
    protected Listbox entitiesListBox;
    
    @Wire
    protected Listhead gridColumns;
    
    private int recordCount;

    private int prevPageSize;

    private int activePage = SQLCMConstants.DEFAULT_PAGE;

    private Integer pageSize = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

    private int sortDirection = SQLCMConstants.SORT_ASCENDING;

    private String sortColumn = SQLCMConstants.DEFAULT_EVENTS_SORT_COLUMN;
    private List<CMEventFilterListData> eventFilterList;
    public List<CMEventFilterListData> getEventFilterList() {
		return eventFilterList;
	}
	public void setEventFilterList(List<CMEventFilterListData> eventFilterList) {
		this.eventFilterList = eventFilterList;
	}

    public static enum EventsSource {
       AUDIT_EVENTS(ELFunctions.getLabel(SQLCMI18NStrings.AUDIT_EVENTS)),
        ARCHIVED_EVENTS(ELFunctions.getLabel(SQLCMI18NStrings.ARCHIVED_EVENTS));
    	
    	/* AUDIT_EVENTS(ELFunctions.getLabel("Audit Events")),
         ARCHIVED_EVENTS(ELFunctions.getLabel("Archived Events"));
*/
        private String label;

        EventsSource(String label) {
            this.label = label;
        }
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        initArchivedDatabases();
        if (entitiesListBox == null) {
            entitiesListBox = new Listbox();
        }
        super.afterCompose(view);
		initColumnList();
		subscribeToVisibilityChangeEvent();
    }

    private void initArchivedDatabases() {
        try {
            selectedArchivedDatabase = null;
            instanceArchivedDatabases = new ListModelList();
            cmArchivedDatabases = DatabasesFacade.getArchivesList(instance.getInstance());
            instanceArchivedDatabases.addAll(cmArchivedDatabases);
            if (!cmArchivedDatabases.isEmpty() && cmArchivedDatabases.get(0) != null && selectedArchivedDatabase == null) {
                selectedArchivedDatabase = cmArchivedDatabases.get(0).getDatabaseName();
            }
        } catch (RestException e) {
            e.printStackTrace();
        }
        BindUtils.postNotifyChange(null, null, this, "selectedArchivedDatabase");
        BindUtils.postNotifyChange(null, null, this, "instanceArchivedDatabases");
    }

    protected void initColumnList()
    {
    	eventsColumnsListModelList = getGridPreferencesInSession().getColumnsListModelList();
        if (eventsColumnsListModelList == null) {
            eventsColumnsListModelList = new ListModelList<EventsColumns>();
            for (EventsColumns eventColumn : EventsColumns.values()) {
                if (eventColumn.isAllowedToDisable()) {
                    eventsColumnsListModelList.add(eventColumn);
                }
            }
        }
    	
    }

    public abstract ListModelList<?> getEventsColumnsListModelList();

    protected void subscribeToVisibilityChangeEvent()
    {
    	 EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT, EventQueues.SESSION, true);
         eq.subscribe(new EventListener<Event>() {
             public void onEvent(Event event) throws Exception {
                 if (event.getData().equals(preferencesSessionVariableName)) {
                     applyColumnsVisibility();
                     BindUtils.postNotifyChange(null, null, EventsGridViewModel.this, "eventsColumnsListModelList");
                 }
             }
         });
    }
    protected void applyColumnsVisibility() {
        for (EventsColumns eventsColumn : EventsColumns.values()) {
            applyColumnVisibility(eventsColumn.isVisible(), eventsColumn.getColumnId());
        }
    }
 

    public CMInstance getInstance() {
        return instance;
    }

    public List<CMBeforeAfterDataEntity> getBeforeAfterDataTables() {
        return beforeAfterDataTables;
    }

    public List<CMBeforeAfterDataEntity> getBeforeAfterDataColumns() {
        return beforeAfterDataColumns;
    }

    public long getSelectedTablesBAD() {
        return selectedTablesBAD;
    }

    public long getSelectedColumnsBAD() {
        return selectedColumnsBAD;
    }

    public ListModelList getInstanceArchivedDatabases() {
        return instanceArchivedDatabases;
    }

    public String getSelectedArchivedDatabase() {
        return selectedArchivedDatabase;
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
        refreshEntitiesList();
    }

    public int getActivePage() {
        return SQLCMConstants.DEFAULT_PAGE;
    }

    /*@Override
    protected void subscribeForSortEvent() {
        Collection<Component> heads = grid.getChildren();
        for (Component headerComponent : heads) {
            headerComponent.addEventListener(Events.ON_SORT, new EventListener<SortEvent>() {
                public void onEvent(SortEvent event) throws Exception {
                    String sortParam = ((Listheader) event.getTarget()).getValue();
                    if (sortParam == null || sortParam.trim().isEmpty()) {
                        throw new RuntimeException(" Invalid column value that is used as sort parameter! ");
                    }
                    sortColumn = sortParam;
                    if (event.isAscending()) {
                        sortDirection = SQLCMConstants.SORT_ASCENDING;
                    } else {
                        sortDirection = SQLCMConstants.SORT_DESCENDING;
                    }
                    refreshEntitiesList();
                }
            });
        }
    }*/
    
    @Override
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


    /*@Command("setGridRowsCount")
    public void setGridRowsCount() {
        try {
            int tmpPageSize = listBoxRowsBox.getValue();
            if (tmpPageSize > 100) {
                Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
                    listBoxRowsBox, "end_center", 3000);
                tmpPageSize = 100;
                listBoxRowsBox.setValue(tmpPageSize);
            }
            if (tmpPageSize != pageSize) {
                activePage = SQLCMConstants.DEFAULT_PAGE;
            }
            listBoxPageId.setPageSize(tmpPageSize);
            prevPageSize = tmpPageSize;
            pageSize = tmpPageSize;          
            int value=0;
            if(entitiesModel !=null && !entitiesModel.isEmpty())
            {            	
            		value=entitiesModel.size();
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
        BindUtils.postNotifyChange(null, null, EventsGridViewModel.this, "*");
    }*/

    @Override
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

    @Override
    @Command("refreshEvent")
    public void refreshEntitiesList() {
        Map<String, Object> customFilterRequest = getCustomFilterRequest(filterRequest);
        CMBeforeAfterDataEventsResponse response = null;
        try {
        	if (instance.getStatusText().equalsIgnoreCase("Archive server")) {
				eventsSource = EventsSource.ARCHIVED_EVENTS;
			}
            switch (eventsSource) {
                case AUDIT_EVENTS:
                	customFilterRequest.put("pageSize", null);
                    response = EventsFacade.getAuditEvents(customFilterRequest);
                    setEventFilterList(response.getEventType());
                    break;
                case ARCHIVED_EVENTS:
                    response = EventsFacade.getArchivedEvents(customFilterRequest);
                    break;
            }
            
            entitiesList = (List<CMEntity>) response.getEvents();
            int totalRecords = response.getRecordCount();
            if (totalRecords < 0) {
                totalRecords = 0;
            }
            recordCount = totalRecords;
            setFileSize(recordCount);
            initBeforeAfterData(response);
        } catch (RestException ex) {
            WebUtil.showErrorBox(ex, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
        }
        super.verifyEntitiesList();
        BindUtils.postNotifyChange(null, null, this, "entitiesModel");
        BindUtils.postNotifyChange(null, null, this, "totalSize");
        BindUtils.postNotifyChange(null, null, this, "fileSize");
    }

    private void initBeforeAfterData(CMBeforeAfterDataEventsResponse response) {
        beforeAfterDataTables.clear();
        beforeAfterDataColumns.clear();
        if (!response.getTables().isEmpty()) {
            beforeAfterDataTables.addAll(response.getTables());
        }
        if (!response.getColumns().isEmpty()) {
            beforeAfterDataColumns.addAll(response.getColumns());
        }
    }

    @Command("selectEventSource")
    public void selectEventSource(@BindingParam("id") String id) {
    	id= id.replace(' ', '_');
    	id = id.toUpperCase();
        eventsSource = EventsSource.valueOf(id);
        if (eventsSource == EventsSource.AUDIT_EVENTS) {
            archiveGroupBox.setVisible(false);
        }
        else {
            archiveGroupBox.setVisible(true);
        }
        refreshEntitiesList();
        setGridRowsCount();
    }

    @Command("changeColumnVisibility")
    public void changeColumnVisibility(@BindingParam("checked") boolean isPicked, @BindingParam("columnId") String columnId) {
        getColumn(columnId).setVisible(isPicked);
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

    private Listheader getColumn(String gridColumnId) {
        List<Component> columns = gridColumns.getChildren();
        Listheader gc = null;
        for (Component gridColumn : columns) {
            if (gridColumnId.equals(gridColumn.getId())) {
                gc = (Listheader) gridColumn;
                break;
            }
        }
        return gc;
    }

    @Override
    protected abstract CommonGridViewReport makeCommonGridViewReport();

    @Command("changeOpenCloseState")
    public void changeOpenCloseState(@ContextParam(ContextType.TRIGGER_EVENT) Event event) {
        Groupbox filterGroup = (Groupbox) event.getTarget();
        filterGroup.getCaption().setSclass(" open-" + (filterGroup.isOpen() ? "true" : "false"));
    }

    @Command("showEventPropertiesDialog")
    public void showEventProperties(@BindingParam("rowIndex") int rowIndex) {
        EventPropertiesViewModel.showEventPropertiesWindow(rowIndex, entitiesModel, instance.getId());
    }

    @Command("selectedTablesBAD")
    public void setSelectedTablesBAD(@BindingParam("id") int id, @BindingParam("label") String label, @BindingParam("cb") Combobox cb   ) {
        selectedTablesBAD = id;
        refreshEntitiesList();
        if(label != null && cb != null){
        	cb.setValue(label);
        }
    }

    @Command("selectedColumnsBAD")
    public void setSelectedColumnsBAD(@BindingParam("id") int id , @BindingParam("label") String label, @BindingParam("cb") Combobox cb  ) {
        selectedColumnsBAD = id;
        refreshEntitiesList();
        if(label != null && cb != null){
        	cb.setValue(label);
        }
    }

    @Command("selectArchive")
    public void setSelectedArchive(@BindingParam("id") String id) {
        selectedArchivedDatabase = id;
        refreshEntitiesList();
    }

    @Override
    protected void applyFilter() {
        activePage = SQLCMConstants.DEFAULT_PAGE;
        super.applyFilter();
        listBoxPageId.setActivePage(activePage - 1);
    }

    protected Map<String, Object> getCustomFilterRequest(Map<String, Object> filterRequest) {
        Map<String, Object> customFilterRequest = new TreeMap<>();
        customFilterRequest.putAll(filterRequest);
        customFilterRequest.put("ServerId", instance.getId());
        customFilterRequest.put("TableId", (selectedTablesBAD == ALL_ENTITIES_BAD) | (selectedTablesBAD == 0L) ? null : selectedTablesBAD);
        customFilterRequest.put("ColumnId", (selectedColumnsBAD == ALL_ENTITIES_BAD) | (selectedColumnsBAD == 0L) ? null : selectedColumnsBAD);
        customFilterRequest.put("page", activePage);
        customFilterRequest.put("pageSize", pageSize);
        customFilterRequest.put("sortColumn", sortColumn);
        customFilterRequest.put("sortDirection", sortDirection);
        if (eventsSource == EventsSource.ARCHIVED_EVENTS) {
            customFilterRequest.put("Archive", selectedArchivedDatabase);
        }
        return customFilterRequest;
    }

    public void applyColumnVisibility(Boolean isPicked, String columnId) {
        if (getColumn(columnId) != null) {
            getColumn(columnId).setVisible(isPicked);
        }
    }

    @Override
    protected CMSideBarViewSettings getSideBarViewSettings() {
        CMExtendedSideBarViewSettings settings = new CMExtendedSideBarViewSettings();
        settings.setSelectedTableBAD(selectedTablesBAD);
        settings.setSelectedColumnBAD(selectedColumnsBAD);
        settings.setSelectedArchivedDatabase(selectedArchivedDatabase);
        return settings;
    }

    @Override
    protected void parseSideBarViewSettings() throws IOException {
        sideBarSettings = JSONHelper.deserializeFromJson(cmViewSettings.getFilter(), new com.fasterxml.jackson.core.type.TypeReference<CMExtendedSideBarViewSettings>() {
        });
        applyExtendedSettings();
    }

    private void applyExtendedSettings() {
        selectedTablesBAD = ALL_ENTITIES_BAD;
        for (Object beforeAfterDataEntity : beforeAfterDataTables) {
            if (((CMBeforeAfterDataEntity) beforeAfterDataEntity).getKey() == ((CMExtendedSideBarViewSettings) sideBarSettings).getSelectedTableBAD()) {
                selectedTablesBAD = ((CMBeforeAfterDataEntity) beforeAfterDataEntity).getKey();
                break;
            }
        }
        selectedColumnsBAD = ALL_ENTITIES_BAD;
        for (Object beforeAfterDataEntity : beforeAfterDataColumns) {
            if (((CMBeforeAfterDataEntity) beforeAfterDataEntity).getKey() == ((CMExtendedSideBarViewSettings) sideBarSettings).getSelectedColumnBAD()) {
                selectedColumnsBAD = ((CMBeforeAfterDataEntity) beforeAfterDataEntity).getKey();
                break;
            }
        }
        selectedArchivedDatabase = null;
        for (Object instanceArchivedDatabase : instanceArchivedDatabases) {
            if (((CMArchivedDatabase) instanceArchivedDatabase).getDatabaseName().equals(((CMExtendedSideBarViewSettings) sideBarSettings).getSelectedArchivedDatabase())) {
                selectedArchivedDatabase = ((CMExtendedSideBarViewSettings) sideBarSettings).getSelectedArchivedDatabase();
                break;
            }
        }
        if (!cmArchivedDatabases.isEmpty() && cmArchivedDatabases.get(0) != null && selectedArchivedDatabase == null) {
            selectedArchivedDatabase = cmArchivedDatabases.get(0).getDatabaseName();
        }
        BindUtils.postNotifyChange(null, null, this, "beforeAfterDataTables");
        BindUtils.postNotifyChange(null, null, this, "beforeAfterDataColumns");
        BindUtils.postNotifyChange(null, null, this, "instanceArchivedDatabases");
        if (sideBarSettings.getFilterData() == null) {
            refreshEntitiesList();
        }
    }

    @Command("openInstance")
    public void openInstance(@BindingParam("instanceId") long id) {
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instanceView/" + id));
    }

    @Command("openDatabase")
    public void openDatabase(@BindingParam("databaseId") long databaseId, @BindingParam("instanceId") long instanceId) {
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("databaseView/" + instanceId + "/" + databaseId));
    }

    @Command("detachArchive")
    public void detachArchive() {
        if (WebUtil.showConfirmationBoxWithIcon(SQLCMI18NStrings.DATABASE_ARCHIVE_REMOVE_MESSAGE, SQLCMI18NStrings.DATABASE_ARCHIVE_REMOVE_TITLE,
            "/images/warning_white.svg", true, (Object) null)) {
            String instance = null;
            for (CMArchivedDatabase archivedDatabase : cmArchivedDatabases) {
                if (archivedDatabase.getDatabaseName().equals(selectedArchivedDatabase)) {
                    instance = archivedDatabase.getInstance();
                }
            }
            try {
                DatabasesFacade.detachArchive(new CMArchive(instance, selectedArchivedDatabase));
            } catch (RestException e) {
                e.printStackTrace();
            }
            initArchivedDatabases();
            refreshEntitiesList();
        }
    }

    @Command("attachArchive")
    public void attachArchive() {
        AttachArchiveViewModel.showAttachArchiveWindow(instance.getId(), this);
    }

    @Command("archiveProperties")
    public void archiveProperties() {
        ArchivePropertiesViewModel.showArchivePropertiesWindow(selectedArchivedDatabase, this);
    }

    public void refreshDatabaseArchives() {
        initArchivedDatabases();
    }
}
