package com.idera.sqlcm.ui.eventFilters;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.StringReader;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import net.sf.jasperreports.engine.JRException;

import org.apache.log4j.Logger;
import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.util.media.AMedia;
import org.zkoss.util.media.Media;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.UploadEvent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.A;
import org.zkoss.zul.Button;
import org.zkoss.zul.Filedownload;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.Image;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listheader;
import org.zkoss.zul.Menupopup;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Window;

import com.idera.common.rest.JSONHelper;
import com.idera.common.rest.RestException;
import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAuditEventFilterExportResponse;
import com.idera.sqlcm.entities.CMAuditedEventFilterEnable;
import com.idera.sqlcm.entities.CMDataByFilterId;
import com.idera.sqlcm.entities.CMDeleteEntity;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEventFilters;
import com.idera.sqlcm.entities.CMExportEvent;
import com.idera.sqlcm.entities.CMExportEventConditionData;
import com.idera.sqlcm.entities.CMExportEventType;
import com.idera.sqlcm.entities.CMFilterEventFiltersResponse;
import com.idera.sqlcm.entities.CMFilteredEventFiltersResponse;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.entities.CMViewSettings;
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.entities.Instance;
import com.idera.sqlcm.entities.SelectedEntry;
import com.idera.sqlcm.entities.ViewNameData;
import com.idera.sqlcm.entities.ViewNameResponse;
import com.idera.sqlcm.facade.EventFiltersFacade;
import com.idera.sqlcm.facade.FilterFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.FilterSelector;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.dialogs.EditFilterViewModel;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.CMEventFilterRules;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.EventFilterWizardViewModel;
import com.idera.sqlcm.ui.eventFilters.filters.EventFiltersViewFilter;
import com.idera.sqlcm.ui.eventFilters.filters.EventFiltersViewFilters;
import com.idera.sqlcm.ui.instances.filters.InstanceFilterChild;
import com.idera.sqlcm.ui.instances.filters.InstancesOptionFilterValues;
import com.idera.sqlcm.ui.logsView.ActivityLogsGridViewModel;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.utils.CmCsvReader;
import com.idera.sqlcm.utils.SQLCMConstants;
import com.idera.sqlcm.wizard.AbstractFilterWizardViewModel.WizardListener;

public class EventFiltersGridViewModel extends CommonGridViewModel implements
		WizardListener {

	private static final String ZUL_URL = "~./sqlcm/prompt/DeletePrompt.zul";
	private static final Logger logger = Logger.getLogger(EventFiltersGridViewModel.class);
	private static final String INSTANCES_ALERTS_SESSION_DATA_BEAN = "EventFiltersSessionDataBean";
	private static final String CHANGE_VISIBILITY_EVENT = "changeColumnVisibility";
	protected EventFiltersSource eventFiltersSource;
	protected ListModelList EventFiltersColumnsListModelList;
	protected CMFilterEventFiltersResponse cmFilterEventFiltersResponse;

	private EventFiltersIconURLConverter iconURLConverter;
	private EventFiltersLabelURLConverter statusURLConverter;

	private ListModelList<CMEntity> selectedEntities;
	private Set<CMEntity> pickedInstanceSet = new HashSet<CMEntity>();
	CMAuditedEventFilterEnable cmAuditedEventFilterEnable;
	CMAuditEventFilterExportResponse cmAuditEventFilterExportResponse;
	protected CMFilteredEventFiltersResponse cmFilteredEventFiltersResponse;
	private List<CMExportEvent> events;
	private List<CMExportEventType> eventTypeCategory;
	private List<CMExportEventConditionData> conditionEvents;
	CMDeleteEntity cmDeleteEntity;
	CMDataByFilterId cmDataByFilterId;
	InsertQueryData insertQueryData;
	private CMInstance instance;
	public int refreshDuration;
	
	public int getRefreshDuration() {
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}
	
	@Wire
    Paging listBoxPageId;
    
    int PAGE_SIZE = 50;
    
	FilterFacade filterFacade = new FilterFacade();

	List<ViewNameData> nameList;

	public List<ViewNameData> getNameList() {
		return nameList;
	}

	public void setNameList(List<ViewNameData> nameList) {
		this.nameList = nameList;
	}

    @Wire
    Spinner listBoxRowsBox;
    
    private int prevPageSize;
		
	private ListModelList<SelectedEntry> importInstanceListModel = new ListModelList<>();

	int iCount = 0;
	int iConditionCount = 0;
	String TargetInstance = "";
	
	public Map<String, String> MainEventData = new TreeMap<String, String>();

	public ListModelList<SelectedEntry> getImportInstanceListModel() {
		return importInstanceListModel;
	}

	@Wire
	Grid importInstanceGrid;

	@Wire
	Button importButton;

	public static enum EventFiltersSource {

		NEW(ELFunctions.getLabel(SQLCMI18NStrings.NEW), 1), IMPORT(ELFunctions
				.getLabel(SQLCMI18NStrings.IMPORT), 2), FROM_EXISTING(
				ELFunctions.getLabel(SQLCMI18NStrings.FROM_EXISTING), 3), SELECT_FILTER(
				ELFunctions.getLabel(SQLCMI18NStrings.ADD_NEW_FILTER), 4);

		private String label;

		private int id;

		EventFiltersSource(String label, int id) {
			this.label = label;
			this.id = id;
		}

		public int getId() {
			return id;
		}
	}

	public EventFiltersGridViewModel() {
		entityFacade = new EventFiltersFacade();
		filterFacade = new FilterFacade();
		eventFiltersSource = EventFiltersSource.NEW;
		exportBeanClass = EventFiltersGridViewModel.class;
		preferencesSessionVariableName = "EventFiltersSessionDataBean";
		iconURLConverter = new EventFiltersIconURLConverter();
		statusURLConverter = new EventFiltersLabelURLConverter();
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
			BindUtils.postNotifyChange(null, null, EventFiltersGridViewModel.this, "*");
		} catch (RestException e) {
			// TODO Auto-generated catch block
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
        BindUtils.postNotifyChange(null, null, EventFiltersGridViewModel.this, "*");
    }
	
	
	protected void initColumnList() {
		EventFiltersColumnsListModelList = getGridPreferencesInSession()
				.getEventFiltersColumnsListModelList();
		if (EventFiltersColumnsListModelList == null) {
			EventFiltersColumnsListModelList = new ListModelList<EventFiltersColumns>();

			EventFiltersColumnsListModelList.add(EventFiltersColumns.FILTER);
			EventFiltersColumnsListModelList.add(EventFiltersColumns.INSTANCE);
			EventFiltersColumnsListModelList.add(EventFiltersColumns.STATUS);
			EventFiltersColumnsListModelList
					.add(EventFiltersColumns.DESCRIPTION);
			getGridPreferencesInSession().setEventFiltersColumnsListModelList(
					EventFiltersColumnsListModelList);
		}
		applyColumnsVisibility();
		BindUtils.postNotifyChange(null, null, this,
				"EventFiltersColumnsListModelList");
	}

	public ListModelList<CMEntity> getSelectedEntities() {
		return selectedEntities;
	}

	public void setSelectedEntities(ListModelList<CMEntity> selectedEntities) {
		this.selectedEntities = selectedEntities;
	}

	public EventFiltersLabelURLConverter getStatusURLConverter() {
		return statusURLConverter;
	}

	@Override
	@Command("refreshEvents")
	public void refreshEntitiesList() {
		Map<String, Object> customFilterRequest = getCustomFilterRequest(filterRequest);
		try {
			cmFilterEventFiltersResponse = ((EventFiltersFacade) entityFacade)
					.getFilterEventFilters(customFilterRequest);
			entitiesList = (List) cmFilterEventFiltersResponse.getEvents();
			int totalRecords = entitiesList.size();
            if (totalRecords < 0) {
                totalRecords = 0;
            }
            setFileSize(totalRecords);
		} catch (RestException ex) {
			WebUtil.showErrorBox(ex,
					SQLCMI18NStrings.FAILED_TO_LOAD_AUDIT_EVENT_FILTERS);
		}
		super.verifyEntitiesList();
		BindUtils.postNotifyChange(null, null, this, "entitiesModel");
		BindUtils.postNotifyChange(null, null, EventFiltersGridViewModel.this, "*");
	}

	@Command("selectViewName")
	public void selectViewName(@BindingParam("id") String id) throws RestException {
		String viewNameWithPrefrences=preferencesSessionVariableName.substring(0,
        		preferencesSessionVariableName.lastIndexOf("SessionDataBean"));
		Sessions.getCurrent().setAttribute("ViewName", id +  "_" +viewNameWithPrefrences);
	}
	
	@Command("selectEventSource")
	public void selectEventSource(@BindingParam("id") String id) {
		eventFiltersSource = EventFiltersSource.valueOf(id);
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
							EventFiltersGridViewModel.this,
							"alertsColumnsListModelList");
				}
			}
		});
	}

	public CMFilteredEventFiltersResponse getCmFilteredAlertsResponse() {
		return cmFilteredEventFiltersResponse;
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

	@Command("showEditFilterDialog")
	public void showEditFilter(@BindingParam("rowIndex") int rowIndex) {
		EditFilterViewModel.showEditFilterWindow(rowIndex, entitiesModel, 1);
	}

	@Command("saveViewState")
	public void saveViewState() {
		CMViewSettings settings = new CMViewSettings();
		String ViewName = (String) Sessions.getCurrent().getAttribute("ViewName");
		Sessions.getCurrent().setAttribute("View","eventFiltersView");
        settings.setViewId(preferencesSessionVariableName);
        settings.setViewName(ViewName);
		CMSideBarViewSettings alertsSettings = new CMSideBarViewSettings();
		alertsSettings.setFilterData(getGridPreferencesInSession().getFilters());
		Map<String, Boolean> columns = new LinkedHashMap<String, Boolean>();
		for (EventFiltersColumns alertsColumn : EventFiltersColumns.values()) {
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
	
	@Command("loadViewState")
	public void loadViewState() {
		try {
			 String viewName = (String) Sessions.getCurrent().getAttribute("ViewName");
			CMViewSettings cmViewSettings = filterFacade
					.getViewSettings(viewName);
			if (cmViewSettings.getFilter() == null) {
				WebUtil.showInfoBox(SQLCMI18NStrings.FAILED_TO_COLLECT_VIEW_STATE);
				return;
			}
			CMSideBarViewSettings alertsSettings = JSONHelper.deserializeFromJson(
							cmViewSettings.getFilter(),
							new com.fasterxml.jackson.core.type.TypeReference<CMSideBarViewSettings>() {
							});

			Map<String, Boolean> columns = alertsSettings.getColumnVisibility();
			for (EventFiltersColumns alertsColumn : EventFiltersColumns
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
		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("eventFiltersView"));	
	}

	@Command("changeColumnVisibility")
	public void changeColumnVisibility(
			@BindingParam("checked") boolean isPicked,
			@BindingParam("columnId") String columnId) {
		EventFiltersColumns.findColumnById(columnId).setVisible(isPicked);
		getGridPreferencesInSession().setColumnsListModelList(
				EventFiltersColumnsListModelList);
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,
				EventQueues.SESSION, false);
		if (eq != null) {
			eq.publish(new Event(CHANGE_VISIBILITY_EVENT, null,
					preferencesSessionVariableName));
		}
	}

	private void applyColumnsVisibility() {
		for (EventFiltersColumns alertsColumn : EventFiltersColumns.values()) {
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
		customFilterRequest.put("AlertType", eventFiltersSource.getId());
		return customFilterRequest;
	}

	public EventFiltersIconURLConverter getIconURLConverter() {
		return iconURLConverter;
	}

	@Command
    public void doCheck(@BindingParam("checked") boolean isPicked, @BindingParam("picked") CMEntity item) {
        if (isPicked) {
        	pickedInstanceSet.add(item);
        } else {
        	pickedInstanceSet.remove(item);
        }
    }

    @Command
    public void doCheckAll() {
        if (!selectedEntities.isEmpty()) {
        	pickedInstanceSet.clear();
        	pickedInstanceSet.addAll(entitiesModel.getSelection());
        } else {
        	pickedInstanceSet.clear();
        }
    }
	public void removeBulkOps(CMEntity item) {
		A actionLink = (A) entitiesListBox.getFellow("makeActionLink"
				+ item.getId());
		actionLink.setPopup((Menupopup) actionLink.getFellow("actionsMenuPopup"
				+ item.getId()));
		Image actionLinkImage = (Image) actionLink.getFirstChild();
		actionLinkImage.setSrc(ELFunctions
				.getImageURLWithoutSize(SQLCMConstants.ACTION_ICON_SINGLE));
	}

	public void updatePopupForBulkOps() {
		StringBuilder parameters = new StringBuilder();
		for (CMEntity item : pickedInstanceSet) {
			parameters.append(item.getId()).append(',');
		}
		parameters.setLength(parameters.length() - 1);
		for (CMEntity item : pickedInstanceSet) {
			A actionLink = (A) entitiesListBox.getFellow("makeActionLink"
					+ item.getId());
			setMenuPopupForBulkOps(actionLink, parameters.toString());
			Image actionLinkImage = (Image) actionLink.getFirstChild();
			actionLinkImage.setSrc(ELFunctions
					.getImageURLWithoutSize(SQLCMConstants.ACTION_ICON_BULK));
		}
	}

	public void setMenuPopupForBulkOps(A actionLink, final String parameters) {
		actionLink.setPopup((Menupopup) actionLink
				.getFellow("bulkActionsMenuPopup"));
		Component removeInstancesMenuItem = actionLink
				.getFellow("removeInstancesMenuItem");
		WebUtil.removeAllOnClickEventListeners(removeInstancesMenuItem,
				"onClick");
		removeInstancesMenuItem.addEventListener("onClick",
				new EventListener<Event>() {
					@Override
					public void onEvent(Event event) throws Exception {
						/* remove(parameters); */
					}
				});
	}

	protected ListModelList<Filter> getFiltersDefinition() {
		ListModelList<Filter> filtersDefinition = new ListModelList<>();

		Filter filter;
		filter = new EventFiltersViewFilter(EventFiltersViewFilters.FILTER);
		filtersDefinition.add(filter);

		filter = new EventFiltersViewFilter(EventFiltersViewFilters.STATUS);
		filter.addFilterChild(new InstanceFilterChild(
				InstancesOptionFilterValues.AUDIT_STATUS_ENABLED));
		filter.addFilterChild(new InstanceFilterChild(
				InstancesOptionFilterValues.AUDIT_STATUS_DISABLED));
		filtersDefinition.add(filter);

		filter = new EventFiltersViewFilter(EventFiltersViewFilters.INSTANCE);
		filtersDefinition.add(filter);

		filter = new EventFiltersViewFilter(EventFiltersViewFilters.DESCRIPTION);
		filtersDefinition.add(filter);
		
		return filtersDefinition;
	}

	public ListModelList<?> getEventFiltersColumnsListModelList() {
		return EventFiltersColumnsListModelList;
	}

	protected EventFiltersGridViewReport makeCommonGridViewReport() {
		EventFiltersGridViewReport instancesEventsGridViewReport = new EventFiltersGridViewReport(
				"Event Filters", "", "EventFilters");
		if (pickedInstanceSet.size() > 0) {
			entitiesModelReport = new ListModelList<CMEntity>();
			for (CMEntity cmEntity : pickedInstanceSet) {
				CMEventFilters cmEventFilters = (CMEventFilters) cmEntity;
				for (int i = 0; i < entitiesModel.size(); i++) {
					CMEventFilters cmEventFiltersReport = (CMEventFilters) entitiesModel.get(i);
					if (cmEventFilters.getFilterid() == cmEventFiltersReport.getFilterid()) {
						entitiesModelReport.add(entitiesModel.get(i)); 
					}
				}
			}
			instancesEventsGridViewReport.setDataMapForListInstance(entitiesModelReport);
			return instancesEventsGridViewReport;
		}
		else{
			WebUtil.showInfoBoxWithCustomMessage("Please select Event Filters.");
		}
		return null;
	}

	@Command("eventFilters")
	public void openInstance(@BindingParam("id") int id) {
		if (id == 1) {
			refreshEntitiesList();
		} else {
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("eventFiltersRules"));
		}
	}
	
	@Command("openInstance")
    public void openInstance(@BindingParam("instanceId") long id) {
		if(id>0)
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instanceView/" + id));
    }
	
	@Command("fromExisting")
	public void fromExisting(@BindingParam("filterid") long filterId) throws JRException, IOException,
			TransformerException, ParserConfigurationException {
		Sessions.getCurrent().setAttribute("QueryType", "FromExisting");
		getDataFromDB(filterId);
	}

	@Command("propertiesEvents")
	public void propertiesEvents(@BindingParam("filterid") long filterId)
			throws Exception {
		Sessions.getCurrent().setAttribute("QueryType", "Update");
		getDataFromDB(filterId);
	}
	
	public void getDataFromDB(long filterId)
	{
		Map<String, Object> customFilterRequest = getCustomFilterRequest(filterRequest);
		cmDataByFilterId = new CMDataByFilterId();
		cmDataByFilterId.setFilterId(filterId);
		try {
			cmAuditEventFilterExportResponse = ((EventFiltersFacade) entityFacade).getAuditEventFilterExportResponse(cmDataByFilterId);
			conditionEvents = (List) cmAuditEventFilterExportResponse.getConditionEvents();

			events = (List) cmAuditEventFilterExportResponse.getEvents();
			eventTypeCategory=(List) cmAuditEventFilterExportResponse.getEventType();
		} catch (RestException ex) {
			WebUtil.showErrorBox(ex,
					SQLCMI18NStrings.FAILED_TO_LOAD_FILTERED_ALERTS);
		}

		java.util.Map<String, String> ExportList = new HashMap();

		filterId = events.get(0).getFilterId();
		String name = events.get(0).getName();
		String description = events.get(0).getDescription();
		long eventType = events.get(0).getEventType();
		String targetInstances = events.get(0).getTargetInstance();
		boolean enabled = events.get(0).isEnabled();
		
		java.util.Map<String, String> EventFilterConditionId = new HashMap();
		EventFilterConditionId.put("0", "" + filterId);
				
		Sessions.getCurrent().setAttribute("Name", name);
		Sessions.getCurrent().setAttribute("Description", description);
		Sessions.getCurrent().setAttribute("eventType", eventTypeCategory);
		Sessions.getCurrent().setAttribute("isEnabled", events.get(0).isEnabled());

		String[] arrTargetInstances = targetInstances.split(",");

		Map<String, Object> TargetInst = new HashMap<String, Object>();
		for (int i = 0; i < arrTargetInstances.length; i++) {
			TargetInst.put(("'" + i + "'"), arrTargetInstances[i]);
		}

		Sessions.getCurrent().setAttribute("TargetInst", TargetInst);

		int iEnabled = 0;
		if (enabled)
			iEnabled = 1;

		String KeyData = name + "," + description + "," + eventType + ","
				+ targetInstances + "," + iEnabled;
		ExportList.put("0", KeyData);

		for (int i = 0; i < conditionEvents.size(); i++) {
			long conditionId = conditionEvents.get(i).getConditionId();
			long fieldId = conditionEvents.get(i).getFieldId();
			String matchString = conditionEvents.get(i).getMatchString();
			String keyVal = "" + (i + 1);
			ExportList.put(keyVal, conditionId + "," + fieldId + ","
					+ matchString);
		}

		SetDataValues(ExportList, EventFilterConditionId);
	}

	public void SetDataValues(Map<String, String> ExportList, Map<String, String> EventFilterConditionId) {
		
		String strEventFieldId = "";
		String strEventDataVal = "";
		String strEventConditionId = "";
		int Counter = 1;
	
		java.util.Map<String, String> EventFilterConditionData = new HashMap();

		for (int p = 0; p < ExportList.size() - 1; p++) {
			EventFilterConditionData.put("" + p, ExportList.get("" + (p + 1)));
		}


 	   if(events.get(0).getTargetInstance()!=null){
 		 Sessions.getCurrent().setAttribute("SQL Server",events.get(0).getTargetInstance());  		   
 	   } 
	 	   
		for (Map.Entry<String, String> EventDataValues : EventFilterConditionData
				.entrySet()) {

			strEventDataVal = EventDataValues.getValue();
			String[] strArray;
			strArray = strEventDataVal.split(",");
			strEventFieldId = strArray[1];
			strEventDataVal = "";
			if(strArray.length>=3)
				strEventDataVal = strArray[2];

			EventFilterConditionId.put("" + Counter, strEventFieldId + "~" + strArray[0]);
			Counter++;
			if (strEventDataVal == null)
				try {
					throw new Exception("KeyValue string should not be null.");
				} catch (Exception e2) {
					// TODO Auto-generated catch block
					e2.printStackTrace();
				}

			if (strEventFieldId.equals("11")){
				Sessions.getCurrent().setAttribute("sessionLoginMatchString",strEventDataVal);
			}
			if (strEventFieldId.equals("10")){
				Sessions.getCurrent().setAttribute("HostMatchString",strEventDataVal);
			}
			if (strEventFieldId.equals("9")){
		   	    Sessions.getCurrent().setAttribute("SQL Server",strEventDataVal);
		    }
			if (strEventFieldId.equals("7")){
				Sessions.getCurrent().setAttribute("PrivilegedMatchString",strEventDataVal);
			}
			if (strEventFieldId.equals("6")){
				Sessions.getCurrent().setAttribute("ObjectMatchString",strEventDataVal);
			}
			if (strEventFieldId.equals("5")){
				Sessions.getCurrent().setAttribute("DbMatchString",strEventDataVal);
			}
			if (strEventFieldId.equals("3")){
				Sessions.getCurrent().setAttribute("LoginMatchString",strEventDataVal);
			}
			if (strEventFieldId.equals("2")){
				Sessions.getCurrent().setAttribute("AppMatchString",strEventDataVal);
			}
			if (strEventFieldId.equals("0")){
				Sessions.getCurrent().setAttribute("eventTypeMatchString",strEventDataVal);
			}
			if (strEventFieldId.equals("1")){
				Sessions.getCurrent().setAttribute("eventMatchString",strEventDataVal);
			}
		}
		
		Sessions.getCurrent().setAttribute("EventConditionId", EventFilterConditionId);
		
		addNewEventAlert();
	}

	@Command("deleteEvents")
	public void deleteFilters(@BindingParam("filterid") final long filterid) throws Exception {
  		Map args = new HashMap();
  		if(Sessions.getCurrent().getAttribute("PopUpConnfirm")==null) 
  		{
  			Sessions.getCurrent().setAttribute("ruleIdForDelete", "eventFilters|"+filterid);
  			Window window = (Window) Executions.createComponents(EventFiltersGridViewModel.ZUL_URL, null, args);
  			window.doHighlighted();
  		}
  		else
  		{
  			inactivateUser(filterid);
  			refreshEntitiesList();
  		}
		}
	
	public void inactivateUser(@BindingParam("filterid") final long filterid) {
		
					cmDeleteEntity = new CMDeleteEntity();
					cmDeleteEntity.setEventId(filterid);
					try {
						EventFiltersFacade.deleteEventFilter(cmDeleteEntity);
						//refreshEntitiesList();
					} catch (RestException exception) {
						WebUtil.showErrorBox(
								exception,
								SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
					}
				}

	@Command("importFile")
	public void importFile() throws JRException, IOException,
			TransformerException, ParserConfigurationException {
		int iEnable = 1;
		if (MainEventData.get("Enabled").equals("true")) {
			iEnable = 1;
		}
	}

	@Command
	@NotifyChange({ "importInstanceGrid", "importInstanceListModel", "importButton" })
	public void uploadFile(@ContextParam(ContextType.TRIGGER_EVENT) UploadEvent event) {
		String strRet = "0";
		strRet = ReadXMLContent(event.getMedia());
		
		if(strRet.equals("1"))
		{
			WebUtil.showErrorBox(
					new Exception(ELFunctions
							.getLabel(SQLCMI18NStrings.NOT_XML_FILE)),
					SQLCMI18NStrings.INVALID_FILE_FORMAT);
		}
		if(strRet.equals("2"))
		{
			WebUtil.showErrorBox(
					new Exception(ELFunctions.getLabel(SQLCMI18NStrings.INVALID_FILE_FORMAT_EVENT_FILTER_IMPORT)),
					SQLCMI18NStrings.ERROR);
		}
		if(strRet.equals("4"))
		{
			WebUtil.showErrorBox(
				new Exception("Please select a valid xml file."),
				SQLCMI18NStrings.ERROR);
		}
		if(strRet.equals("3"))
		{ 
			try{
				String xmlContent = event.getMedia().getStringData();
				EventFiltersFacade eventFiltersFacade=new EventFiltersFacade();
				String statusMessage=eventFiltersFacade.importAuditEventFilters(xmlContent);
				if(!statusMessage.equalsIgnoreCase("failed")){
					refreshEntitiesList();
					WebUtil.showInfoBoxWithCustomMessage("Successfully Imported.");
				}
			else{
				WebUtil.showInfoBoxWithCustomMessage("Import failed.");
			}			
		}
		catch(RestException e){WebUtil.showInfoBoxWithCustomMessage("Import failed.");}
			refreshEntitiesList();
		}
	}

	public void InsertEventFilterData(String Query) {
		insertQueryData = new InsertQueryData();
		insertQueryData.setDataQuery(Query);
		try {
			EventFiltersFacade.insertEventFilterData(insertQueryData);
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
		}
	}

	private void processUploadEvent(Media media) {
		if (media == null || !media.getName().endsWith(".xml")) {
			WebUtil.showErrorBox(
					new Exception(ELFunctions
							.getLabel(SQLCMI18NStrings.NOT_XML_FILE)),
					SQLCMI18NStrings.INVALID_FILE_FORMAT);
			return;
		}
		importInstanceListModel.clear();

		CmCsvReader reader = new CmCsvReader(
				media.isBinary() ? new InputStreamReader(media.getStreamData())
						: media.getReaderData());
		try {
			try {
				List<String[]> fileContent = reader.readAll();
				if (fileContent == null) {
					WebUtil.showErrorBox(I18NStrings.ERROR,
							I18NStrings.FAILED_TO_READ_FILE);
					return;
				}
				for (String[] tmp : fileContent) {
					for (String singleContent : Arrays.asList(tmp)) {
						SelectedEntry tmpEntry = new SelectedEntry(
								singleContent, true, "");
						if (singleContent.trim().isEmpty()
								|| importInstanceListModel.contains(tmpEntry))
							continue;

						importInstanceListModel.add(new SelectedEntry(singleContent, true, ""));
					}
				}
			} catch (IOException e) {
				WebUtil.showErrorBox(e, I18NStrings.FAILED_TO_READ_FILE);
			}
		} finally {
			try {
				reader.close();
			} catch (IOException e) {
				// user cannot do anything here if we failed to close the file.
				logger.error(I18NStrings.FAILED_TO_CLOSE_FILE, e);
			}
		}
		importInstanceGrid.setVisible(true);

		if (!importInstanceListModel.isEmpty()) {
			importButton.setDisabled(false);
		}
	}

	public void onCancel() {
		// do nothing
	}

	public void onFinish() {
		loadDatabases();
		BindUtils.postNotifyChange(null, null, this, "databaseList");
	}

	@Command("newEventAlert")
	public void addNewEventAlert() {
		try {
			instance = InstancesFacade.getInstanceDetails(1);
		} catch (RestException e) {
			e.printStackTrace();
		}
		EventFilterWizardViewModel.showEventFilterWizard(instance, this);
	}

	private void loadDatabases() {
	}

	public String ReadXMLContent(Media media) {
		if (media == null || !media.getName().endsWith(".xml")) {			
			return "1";
		}
		try {
			String FilEName = media.getName();
			String file = media.getStringData();

			DocumentBuilder dBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
			InputSource is = new InputSource(new StringReader(file));
			Document doc = dBuilder.parse(is);
			if(doc.getDocumentElement().getNodeName().equals("FilterTemplate"))
			{
				if (doc.hasChildNodes()) {	
					printNote(doc.getChildNodes());	
				}
				return "3";
			}
			else
			{
				return "2";
			}

		} catch (Exception e) {
			System.out.println(e.getMessage());
			return "4";
		}
	}

	private void printNote(NodeList nodeList) {
		
		for (int count = 0; count < nodeList.getLength(); count++) {

			Node tempNode = nodeList.item(count);

			// make sure it's element node.
			if (tempNode.getNodeType() == Node.ELEMENT_NODE) {

				if (tempNode.getNodeName().equals("Condition"))
					iConditionCount++;

				if (iConditionCount == 0) {
					MainEventData.put(tempNode.getNodeName(),
							tempNode.getTextContent());
				} else {
					MainEventData.put(tempNode.getNodeName() + "_"
							+ iConditionCount, tempNode.getTextContent());
				}
				
				if (tempNode.getNodeName().equals("TargetInstance"))
				{
					TargetInstance += tempNode.getTextContent() + "," ;
				}

				iCount++;

				if (tempNode.hasAttributes()) {

					// get attributes names and values
					NamedNodeMap nodeMap = tempNode.getAttributes();

					for (int i = 0; i < nodeMap.getLength(); i++) {

						Node node = nodeMap.item(i);
						//System.out.println("attr name : " + node.getNodeName());
						//System.out.println("attr value : " + node.getNodeValue());
						if (iConditionCount > 0)
							MainEventData.put(
									tempNode.getNodeName() + "_"
											+ iConditionCount + "_"
											+ node.getNodeName(),
									node.getNodeValue());
						else
							MainEventData.put(tempNode.getNodeName() + "_"
									+ node.getNodeName(), node.getNodeValue());

						iCount++;
					}

				}

				if (tempNode.hasChildNodes()) {
					// loop again if has child nodes
					printNote(tempNode.getChildNodes());

				}
				//System.out.println("Node Name =" + tempNode.getNodeName() + " [CLOSE]");
			}
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

	@Command
	public void enableAlertRules(@BindingParam("filterid") long filterid,
			@BindingParam("enable") boolean enable) {
		cmAuditedEventFilterEnable = new CMAuditedEventFilterEnable();
		cmAuditedEventFilterEnable.setEventId(filterid);
		cmAuditedEventFilterEnable.setEnable(enable);
		try {
			EventFiltersFacade
					.changeEventFilterStatus(cmAuditedEventFilterEnable);
			refreshEntitiesList();
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
		}
	}
	
	@Command 
	public void exportEvents(@BindingParam("filterid") String filterid){
		String ruleIdString = " WHERE filterId = " +filterid;
		try{
			EventFiltersFacade  eventFilterFacade = new EventFiltersFacade();
			String exportedPath = eventFilterFacade.getEventFilterExportStatus(ruleIdString);
	    	 if(!exportedPath.equalsIgnoreCase("failed")){
	    	 WebUtil.showInfoBoxWithCustomMessage("Exported Successfully to "+"'" +exportedPath+"'.");
    	 }
    	 else {
    		 WebUtil.showInfoBoxWithCustomMessage("Export failed.");
    	  }
		}
		catch(RestException e){
			WebUtil.showInfoBoxWithCustomMessage("Export failed.");
		}
	}

    @Override
 	@Command("exportToXml")
    public void exportToXml(){
		if(pickedInstanceSet.size()!=0){
			String ruleIdString="";
			if(pickedInstanceSet.size()!= entitiesModel.size()){
				int i=0;
				for(CMEntity cmEntity:pickedInstanceSet){
					CMEventFilters cmEventFilter= (CMEventFilters)cmEntity;
					if(i==0){
						ruleIdString = " WHERE filterId = " + cmEventFilter.getFilterid();
					}
					else{
						ruleIdString +=" OR filterId = " + cmEventFilter.getFilterid();
					}
					i++;
				}
			}
			try{
				EventFiltersFacade  eventFilterFacade = new EventFiltersFacade();
				String exportedPath = eventFilterFacade.getEventFilterExportStatus(ruleIdString);
		    	 if(!exportedPath.equalsIgnoreCase("failed")){
		    	 WebUtil.showInfoBoxWithCustomMessage("Exported Successfully to "+"'" +exportedPath+"'.");
	    	 }
	    	 else {
	    		 WebUtil.showInfoBoxWithCustomMessage("Export failed.");
	    	  }
			}
			catch(RestException e){
				WebUtil.showInfoBoxWithCustomMessage("Export failed.");
			}
		}
		else{
			WebUtil.showInfoBoxWithCustomMessage("Please select Event Filters.");
		}
    }
}
