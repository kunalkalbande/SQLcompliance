package com.idera.sqlcm.ui.instancesAlertsRule;

import java.io.File;
import java.io.IOException;
import java.io.StringReader;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;

import org.w3c.dom.Document;

import net.sf.jasperreports.engine.JRException;

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
import org.zkoss.util.media.Media;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.UploadEvent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listheader;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Window;

import com.idera.common.rest.JSONHelper;
import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCategory;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.CMAlertRulesEnableRequest;
import com.idera.sqlcm.entities.CMAlertRulesExportResponse;
import com.idera.sqlcm.entities.CMDataByRuleId;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEventFilterListData;
import com.idera.sqlcm.entities.CMFilteredAlertRulesResponse;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.entities.CMViewSettings;
import com.idera.sqlcm.entities.ViewNameData;
import com.idera.sqlcm.entities.ViewNameResponse;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.facade.FilterFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.smtpConfiguration.SmtpConfiguration;
import com.idera.sqlcm.snmpTrap.SnmpTrapSender;
import com.idera.sqlcm.ui.components.FilterSelector;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.dialogs.EventPropertiesViewModel;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesWizardViewModel;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.RulesCoreConstants;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.steps.NewAlertRulesStepViewModel.AlertsRuleSource;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.steps.SummaryStepViewModel;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.AddDataAlertRulesWizardViewModel;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesWizardViewModel;
import com.idera.sqlcm.ui.instances.filters.InstanceFilterChild;
import com.idera.sqlcm.ui.instances.filters.InstancesOptionFilterValues;
import com.idera.sqlcm.ui.instancesAlertsRule.filters.AlertRuleFilter;
import com.idera.sqlcm.ui.instancesAlertsRule.filters.AlertRuleFilterChild;
import com.idera.sqlcm.ui.instancesAlertsRule.filters.AlertRuleTypeFilterChild;
import com.idera.sqlcm.ui.instancesAlertsRule.filters.AlertsRuleFilters;
import com.idera.sqlcm.ui.instancesAlertsRule.filters.AlertsRuleOptionValues;
import com.idera.sqlcm.ui.instancesAlertsRule.filters.AlertsRuleTypeOptionValues;
import com.idera.sqlcm.windowsLog.WindowsEventLogger;
import com.idera.sqlcm.wizard.AbstractAlertWizardViewModel;
import com.idera.sqlcm.wizard.AbstractStatusWizardViewModel;
import com.idera.sqlcm.wizard.AbstractDataAlertWizardViewModel;
import com.idera.sqlcm.snmpTrap.UpdateSNMPConfiguration;

public class InstancesAlertsRuleGridViewModel extends CommonGridViewModel
		implements AbstractAlertWizardViewModel.WizardListener,
		AbstractStatusWizardViewModel.WizardListener,
		AbstractDataAlertWizardViewModel.WizardListener {
	private static final String ZUL_URL = "~./sqlcm/prompt/DeletePrompt.zul";
	
	private static final String INSTANCES_ALERTS_RULES_SESSION_DATA_BEAN = "InstancesAlertsRuleSessionDataBean";
	private static final String CHANGE_VISIBILITY_EVENT = "changeColumnVisibility";
	protected AlertsSource alertsRuleSource;
	protected ListModelList alertsRuleColumnsListModelList;
	protected CMFilteredAlertRulesResponse cmFilteredAlertRulesResponse;
	private AlertsRuleLabelURLConverter labelURLConverter;
	private AlertsRuleTypeURLConverter typeURLConverter;
	private AlertsRuleMailMessageURLConverter messageURLConverter;
	private AlertsRuleLogMessageURLConverter logMessageURLConverter;
	private AlertsRulesnmpTrapMessageURLConverter snmpTrapURLConverter;
	private AlertsRuleEnableIconConverter enableIconURLConverter;
	private AlertsRuleSource ruleSource;
	private ListModelList<CMDatabase> databaseList;
	private CMDataByRuleId cmDataByRuleId;
	private CMAlertRulesExportResponse cmAlertRulesExportResponse;
	private CMInstance instance;
	private Set<CMEntity> pickedAlertRuleSet = new HashSet<CMEntity>();
	private int recordCount;
	int iCount = 0;
	int iConditionCount = 0;
	String TargetInstance = "";
	public Map<String, String> MainEventData = new TreeMap<String, String>();
	static CMAlertRulesEnableRequest cmAlertRulesEnableRequest;
	UpdateSNMPConfiguration updateSNMPConfiguration;
	WindowsEventLogger windowslog;
	SmtpConfiguration smtpConfiguration;
	SnmpTrapSender snmpTrapSender;
	SummaryStepViewModel summaryStepViewModel;
	private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCategory> alertCategory;
	private List<CMAlertRulesCondition> conditionEvents;
	@Wire
    Paging listBoxPageId;
    int PAGE_SIZE = 50;
    @Wire
    Intbox listBoxRowsBox;
    
	FilterFacade filterFacade = new FilterFacade();

	List<ViewNameData> nameList;

	private int refreshDuration; //SQLCM 5.4 SCM-9

	public int getRefreshDuration() { //SQLCM 5.4 SCM-9 Start
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}
 // SQLCM 5.4 SCM-9 END
 
	private ListModelList<CMEntity> selectedEntities =new ListModelList<CMEntity>();

	public ListModelList<CMEntity> getSelectedEntities() {
		return selectedEntities;
	}

	public void setSelectedEntities(ListModelList<CMEntity> selectedEntities) {
		this.selectedEntities = selectedEntities;
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
	java.util.Map<String, String> ExportList = new HashMap();
	public static enum AlertsSource {
		EVENT(ELFunctions.getLabel(SQLCMI18NStrings.EVENT), 1),
		DATA(ELFunctions.getLabel(SQLCMI18NStrings.DATA), 2),
		STATUS(ELFunctions.getLabel(SQLCMI18NStrings.STATUS), 3),
		IMPORT(ELFunctions.getLabel(SQLCMI18NStrings.IMPORT_BUTTON), 4),
		FROM_EXISTING(ELFunctions.getLabel(SQLCMI18NStrings.FROM_EXISTING), 5);

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

	public InstancesAlertsRuleGridViewModel() {
		entityFacade = new AlertRulesFacade();
		filterFacade = new FilterFacade();
		alertsRuleSource = AlertsSource.EVENT;
		exportBeanClass = InstancesAlertsRuleGridViewModel.class;
		preferencesSessionVariableName = INSTANCES_ALERTS_RULES_SESSION_DATA_BEAN;
		typeURLConverter = new AlertsRuleTypeURLConverter();
		labelURLConverter = new AlertsRuleLabelURLConverter();
		messageURLConverter = new AlertsRuleMailMessageURLConverter();
		logMessageURLConverter = new AlertsRuleLogMessageURLConverter();
		snmpTrapURLConverter = new AlertsRulesnmpTrapMessageURLConverter();
		enableIconURLConverter = new AlertsRuleEnableIconConverter();
		ViewNameResponse viewNameResponse = new ViewNameResponse();
		try {
			viewNameResponse = filterFacade.getViewName(preferencesSessionVariableName);
			nameList = viewNameResponse.getViewNameTable();
			//Start 5.3.1 Null Pointer Exception on clicking Load View
			for(int i=0; i<nameList.size(); i++){
				if(nameList.get(i).getViewName().contains("_")){
					String viewNameSubString = nameList.get(i).getViewName().substring(
							0,nameList.get(i).getViewName().lastIndexOf("_"));
					nameList.get(i).setViewName(viewNameSubString);
				}
			}
			//End 5.3.1 Null Pointer Exception on clicking Load View
			setNameList(nameList);
			BindUtils.postNotifyChange(null, null, InstancesAlertsRuleGridViewModel.this, "*");
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		try{
		super.afterCompose(view);
		//SQLCM 4.5 SCM -9 Start
		String refreshDuration= RefreshDurationFacade.getRefreshDuration();
		int refDuration=Integer.parseInt(refreshDuration);
		refDuration=refDuration*1000;
		setRefreshDuration(refDuration); //SQLCM 4.5 SCM -9
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
		alertsRuleColumnsListModelList = getGridPreferencesInSession()
				.getAlertRulesColumnsListModelList();
		if (alertsRuleColumnsListModelList == null) {
			alertsRuleColumnsListModelList = new ListModelList<AlertsRuleColumns>();
			alertsRuleColumnsListModelList.add(AlertsRuleColumns.RULE);
			alertsRuleColumnsListModelList.add(AlertsRuleColumns.RULE_TYPE);
			alertsRuleColumnsListModelList.add(AlertsRuleColumns.SERVER);
			alertsRuleColumnsListModelList.add(AlertsRuleColumns.LEVEL);
			alertsRuleColumnsListModelList.add(AlertsRuleColumns.E_MAIL);
			alertsRuleColumnsListModelList.add(AlertsRuleColumns.EVENT_LOG);
			alertsRuleColumnsListModelList.add(AlertsRuleColumns.SNMP_TRAP);
			getGridPreferencesInSession().setAlertRulesColumnsListModelList(
					alertsRuleColumnsListModelList);
		}
		applyColumnsVisibility();
		BindUtils.postNotifyChange(null, null, this,
				"alertsRuleColumnsListModelList");
	}
		@Override
	 	@Command("exportToXml")
	    public void exportToXml(){
			if(pickedAlertRuleSet.size()!=0){
				String ruleIdString="";
				if(pickedAlertRuleSet.size()!=entitiesModel.size()){
					int i=0;
					for(CMEntity cmEntity:pickedAlertRuleSet){
						CMAlertRules cmAlertRules= (CMAlertRules)cmEntity;
						if(i==0){
							ruleIdString = " AND (ruleId = "+cmAlertRules.getRuleId();
						}
						else{
							ruleIdString +=" OR ruleId = "+cmAlertRules.getRuleId();
						}
						i++;
					}
					ruleIdString +=")"; 
				}
				try{
					AlertRulesFacade alertRulesFacade=new AlertRulesFacade();
					String exportedPath=alertRulesFacade.getAlertRulesExportStatus(ruleIdString);
			    	 if(!exportedPath.equalsIgnoreCase("failed")){
			    		 WebUtil.showInfoBoxWithCustomMessage("File Exported Successfully to "+"'" +exportedPath+"'.");
		    	 }
		    	 else {
		    		 WebUtil.showInfoBoxWithCustomMessage("Export failed.");
		    	 }
				}
				catch(RestException e){
				}
			}
			else{
				WebUtil.showInfoBoxWithCustomMessage("Please select Alert Rules.");
			}
	    }
	
	 	@Command
	    public void doCheck(@BindingParam("checked") boolean isPicked, @BindingParam("picked") CMEntity item) {
	        if (isPicked) {
	            pickedAlertRuleSet.add(item);
	        } else {
	        	pickedAlertRuleSet.remove(item);
	        }
	        
	    }

	    @Command
	    public void doCheckAll() {
	        if (!selectedEntities.isEmpty()) {
	        	pickedAlertRuleSet.clear();
	            pickedAlertRuleSet.addAll(entitiesModel.getSelection());
	        } else {
	        	pickedAlertRuleSet.clear();
	        }
	    }

	@Override
	@Command("refreshEvents")
	public void refreshEntitiesList() {
		Map<String, Object> customFilterRequest = getCustomFilterRequest(filterRequest);
		try {
			cmFilteredAlertRulesResponse = ((AlertRulesFacade) entityFacade).getFilteredAlertRules(customFilterRequest);
			entitiesList = (List) cmFilteredAlertRulesResponse.getAlertRules();
			setEventFilterList(cmFilteredAlertRulesResponse.getEventType());
			getInstancesAlertRulesSummary();
			int totalRecords = cmFilteredAlertRulesResponse.getRecordCount();
			if (totalRecords < 0) {
				totalRecords = 0;
			}
			recordCount = totalRecords;
		} catch (RestException ex) {
			WebUtil.showErrorBox(ex,
					SQLCMI18NStrings.FAILED_TO_LOAD_FILTERED_ALERTS);
		}
		super.verifyEntitiesList();
		BindUtils.postNotifyChange(null, null, this, "instancesAlertsSummary");
		BindUtils.postNotifyChange(null, null, this,"instancesAlertRulesSummary");
		BindUtils.postNotifyChange(null, null, this, "totalSize");
		BindUtils.postNotifyChange(null, null, this, "entitiesModel");
	}

	@Command("selectEventSource")
	public void selectEventSource(@BindingParam("id") String id) {
		alertsRuleSource = AlertsSource.valueOf(id);
		refreshEntitiesList();
	}
	
	@Command("openInstance")
    public void openInstance(@BindingParam("instanceId") long id) {
		if(id>0)
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instanceView/" + id));
    }
	
	public int getTotalSize() {
        return recordCount;
    }
	
	protected void subscribeToVisibilityChangeEvent() {
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,EventQueues.SESSION, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getData().equals(preferencesSessionVariableName)) {
					applyColumnsVisibility();
					BindUtils.postNotifyChange(null, null,InstancesAlertsRuleGridViewModel.this,"alertsRuleColumnsListModelList");
				}
			}
		});
	}
	 

	public CMFilteredAlertRulesResponse getCmFilteredAlertsResponse() {
		return cmFilteredAlertRulesResponse;
	}

	public String getInstancesAlertRulesSummary() {
		if (this.cmFilteredAlertRulesResponse != null) {
			return ELFunctions.getLabelWithParams(
					SQLCMI18NStrings.INSTANCES_ALERTS_RULES_SUMMARY,
					cmFilteredAlertRulesResponse.getTotalEventAlertRules(),
					cmFilteredAlertRulesResponse.getTotalDataAlertRules(),
					cmFilteredAlertRulesResponse.getTotalStatusAlertRules());
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

	@Command("exportAlertEvents")
	public void exportEvents(@BindingParam("ruleId") long ruleId)
			throws Exception {
		String ruleIdString = "AND ruleId = " + ruleId;
		try{
			AlertRulesFacade alertRulesFacade=new AlertRulesFacade();
			String exportedPath=alertRulesFacade.getAlertRulesExportStatus(ruleIdString);
	    	 if(!exportedPath.equalsIgnoreCase("failed")){
	    		 WebUtil.showInfoBoxWithCustomMessage("File Exported Successfully to "+"'" +exportedPath+"'.");
    	 }
    	 else {
    		 WebUtil.showInfoBoxWithCustomMessage("Export failed.");
    	 }
		}
		catch(RestException e){
			WebUtil.showInfoBoxWithCustomMessage("Export failed.");
		}
	}
	
	@Command("changeOpenCloseState")
	public void changeOpenCloseState(
			@ContextParam(ContextType.TRIGGER_EVENT) Event event) {
		Groupbox filterGroup = (Groupbox) event.getTarget();
		filterGroup.getCaption().setSclass(
				"open-" + (filterGroup.isOpen() ? "true" : "false"));
	}

	@Command("showEventPropertiesDialog")
	public void showEventProperties(@BindingParam("rowIndex") int rowIndex) {
		EventPropertiesViewModel.showEventPropertiesWindow(rowIndex,
				entitiesModel, 1);
	}

	@Command("saveViewState")
	public void saveViewState() {
		CMViewSettings settings = new CMViewSettings();
		String ViewName = (String) Sessions.getCurrent().getAttribute("ViewName");
		Sessions.getCurrent().setAttribute("View", "instancesAlertsRule");
        settings.setViewId(preferencesSessionVariableName);
        settings.setViewName(ViewName);
		CMSideBarViewSettings alertsSettings = new CMSideBarViewSettings();
		alertsSettings.setFilterData(getGridPreferencesInSession().getFilters());
		Map<String, Boolean> columns = new LinkedHashMap<String, Boolean>();
		for (AlertsRuleColumns alertsColumn : AlertsRuleColumns.values()) {
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
		    		CommonGridViewModel.ZUL_URL_saveViewName, null,null);
		window.doHighlighted();
	}
	        
    @Command("selectViewName")
	public void selectViewName(@BindingParam("id") String id){
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
			for (AlertsRuleColumns alertsColumn : AlertsRuleColumns.values()) {
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
		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instancesAlertsRule"));
	}

	@Command("changeColumnVisibility")
	public void changeColumnVisibility(
			@BindingParam("checked") boolean isPicked,
			@BindingParam("columnId") String columnId) {
		AlertsRuleColumns.findColumnById(columnId).setVisible(isPicked);
		getGridPreferencesInSession().setAlertRulesColumnsListModelList(
				alertsRuleColumnsListModelList);
		EventQueue<Event> eq = EventQueues.lookup(CHANGE_VISIBILITY_EVENT,
				EventQueues.SESSION, false);
		if (eq != null) {
			eq.publish(new Event(CHANGE_VISIBILITY_EVENT, null,
					preferencesSessionVariableName));
		}
	}

	@Command
	public void enableAlertRules(@BindingParam("ruleId") long ruleId,
			@BindingParam("enable") boolean enable) {

		cmAlertRulesEnableRequest = new CMAlertRulesEnableRequest();
		cmAlertRulesEnableRequest.setRuleId(ruleId);
		cmAlertRulesEnableRequest.setEnable(enable);
		try {
			AlertRulesFacade.changeAlertRulesStatus(cmAlertRulesEnableRequest);
			refreshEntitiesList();
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
		}
	}
	    
	  @Command("deleteAlertRules")
			public void deleteRules(@BindingParam("ruleId") final long ruleId) throws Exception {
		  		Map args = new HashMap();
		  		if(Sessions.getCurrent().getAttribute("PopUpConnfirm")==null) 
		  		{
		  			Sessions.getCurrent().setAttribute("ruleIdForDelete", "alertRules|"+ruleId);
		  			Window window = (Window) Executions.createComponents(InstancesAlertsRuleGridViewModel.ZUL_URL, null, args);
		  			window.doHighlighted();
		  		}
		  		else
		  		{
		  			deleteAlertRules(ruleId);
		  			refreshEntitiesList();
		  		}
	  		}

	 public void deleteAlertRules(long ruleId)
	 {             
	     cmAlertRulesEnableRequest = new CMAlertRulesEnableRequest();
	     cmAlertRulesEnableRequest.setRuleId(ruleId);
	      try {
	       	   AlertRulesFacade.deleteAlertRules(cmAlertRulesEnableRequest);
	         } catch (RestException exception) {
	           WebUtil.showErrorBox(exception,
	             SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
	         }
	}
	 
	private void applyColumnsVisibility() {
		for (AlertsRuleColumns alertsColumn : AlertsRuleColumns.values()) {
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
		customFilterRequest.put("AlertRuleType", alertsRuleSource.getId());
		return customFilterRequest;
	}

	public AlertsRuleLabelURLConverter getLabelURLConverter() {
		return labelURLConverter;
	}

	public AlertsRuleTypeURLConverter getTypeURLConverter() {
		return typeURLConverter;
	}

	public AlertsRuleMailMessageURLConverter getMessageURLConverter() {
		return messageURLConverter;
	}

	public AlertsRuleLogMessageURLConverter getLogMessageURLConverter() {
		return logMessageURLConverter;
	}

	public AlertsRulesnmpTrapMessageURLConverter getSnmpTrapURLConverter() {
		return snmpTrapURLConverter;
	}

	public AlertsRuleEnableIconConverter getEnableIconURLConverter() {
		return enableIconURLConverter;
	}

	protected ListModelList<Filter> getFiltersDefinition() {
		ListModelList<Filter> filtersDefinition = new ListModelList<>();

		Filter filter;

		filter = new AlertRuleFilter(AlertsRuleFilters.RULE);
		filtersDefinition.add(filter);

		filter = new AlertRuleFilter(AlertsRuleFilters.RULE_TYPE);
		filter.addFilterChild(new AlertRuleTypeFilterChild(AlertsRuleTypeOptionValues.EVENT));
		filter.addFilterChild(new AlertRuleTypeFilterChild(AlertsRuleTypeOptionValues.DATA));
		filter.addFilterChild(new AlertRuleTypeFilterChild(AlertsRuleTypeOptionValues.STATUS));
		filtersDefinition.add(filter);

		filter = new AlertRuleFilter(AlertsRuleFilters.SERVER);
		filtersDefinition.add(filter);

		filter = new AlertRuleFilter(AlertsRuleFilters.LEVEL);
		filter.addFilterChild(new AlertRuleFilterChild(AlertsRuleOptionValues.SEVERE_STATUS));
		filter.addFilterChild(new AlertRuleFilterChild(AlertsRuleOptionValues.HIGH_STATUS));
		filter.addFilterChild(new AlertRuleFilterChild(AlertsRuleOptionValues.MEDIUM_STATUS));
		filter.addFilterChild(new AlertRuleFilterChild(AlertsRuleOptionValues.LOW_STATUS));
		filtersDefinition.add(filter);		
		filter = new AlertRuleFilter(AlertsRuleFilters.E_MAIL);
		filter.addFilterChild(new InstanceFilterChild(InstancesOptionFilterValues.EMAIL_YES));
		filter.addFilterChild(new InstanceFilterChild(InstancesOptionFilterValues.EMAIL_NO));
		filtersDefinition.add(filter);


		filter = new AlertRuleFilter(AlertsRuleFilters.EVENT_LOG);
		filter.addFilterChild(new InstanceFilterChild(InstancesOptionFilterValues.EVENT_LOG_YES));
		filter.addFilterChild(new InstanceFilterChild(InstancesOptionFilterValues.EVENT_LOG_NO));
		filtersDefinition.add(filter);

		filter = new AlertRuleFilter(AlertsRuleFilters.SNMP_TRAP);
		filter.addFilterChild(new InstanceFilterChild(InstancesOptionFilterValues.SNMP_TRAP_YES));
		filter.addFilterChild(new InstanceFilterChild(InstancesOptionFilterValues.SNMP_TRAP_NO));
		filtersDefinition.add(filter);

		return filtersDefinition;
	}

	public ListModelList<?> getAlertsRuleColumnsListModelList() {
		return alertsRuleColumnsListModelList;
	}

	protected InstancesAlertsRuleGridViewReport makeCommonGridViewReport() {
		InstancesAlertsRuleGridViewReport instancesEventsGridViewReport = new InstancesAlertsRuleGridViewReport(
				"Instances Alert Rules", "", "AlertRules");
		if (pickedAlertRuleSet.size() > 0) {
			entitiesModelReport = new ListModelList<CMEntity>();
			int entitiesModelSize=entitiesModel.size();
			for (CMEntity cmEntity : pickedAlertRuleSet) {
				CMAlertRules cmAlertRules = (CMAlertRules) cmEntity;
				for (int i = 0; i < entitiesModelSize; i++) {
					CMAlertRules cmAlertRulesReport = (CMAlertRules) entitiesModel.get(i);
					if (cmAlertRules.getRuleId() == cmAlertRulesReport.getRuleId()) {
						entitiesModelReport.add(entitiesModel.get(i)); 
					}
				}

			}
			instancesEventsGridViewReport.setDataMapForListInstance(entitiesModelReport);
			return instancesEventsGridViewReport;
		}
		else{
			WebUtil.showInfoBoxWithCustomMessage("Please select alert rules.");
		}
		return null;
	}

	@Command("instanceAlertsView")
	public void openInstance(@BindingParam("id") int id) {
		if (id == 1) {
			Sessions.getCurrent().setAttribute("alert_type","instancesAlerts");
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("instancesAlerts"));
		} else {
			refreshEntitiesList();
		}
	}

	@Command("addAlertRules")
	public void addAlertRules(@BindingParam("id") String id) throws Exception {
		if (id.equals("EVENT")) {
			AddAlertRulesWizardViewModel.showAddAlertRulesWizard(this);
		}

		if (id.equals("STATUS")) {
			AddStatusAlertRulesWizardViewModel.showAddStatusAlertRulesWizard(this);
		}
		if (id.equals("DATA")) {
			AddDataAlertRulesWizardViewModel.showAddDataAlertRulesWizard(this);
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


	@Command("editAlertRules")
	public void editAlertRules(@BindingParam("ruleId") long ruleId)
			throws Exception {
		Map<String, Object> customFilterRequest = getCustomFilterRequest(filterRequest);
		RulesCoreConstants rulesCoreConstants = new RulesCoreConstants();
		cmDataByRuleId = new CMDataByRuleId();
		cmDataByRuleId.setFilterId(ruleId);
		try {
			cmAlertRulesExportResponse = ((AlertRulesFacade) entityFacade)
					.getAlertRulesExportResponse(cmDataByRuleId);
			conditionEvents = (List) cmAlertRulesExportResponse.getAlertRulesCondition();

			alertRules = (List) cmAlertRulesExportResponse.getAlertRules();
			
			alertCategory = (List) cmAlertRulesExportResponse.getAlertRulesCategory();
		} catch (RestException ex) {
			WebUtil.showErrorBox(ex,
					SQLCMI18NStrings.FAILED_TO_LOAD_FILTERED_ALERTS);
		}		
		String RuleTypeId =Integer.toString(alertRules.get(0).getAlertType());
		for (Map.Entry<String, String> entry : rulesCoreConstants.RulesType()
				.entrySet()) {
 
			if (entry.getKey().equals(RuleTypeId)) {
				RuleTypeId = entry.getValue();
			}
		}
		Sessions.getCurrent().setAttribute("QueryType", "Update");		 
		Sessions.getCurrent().setAttribute("QueryTypeForColumn","Update");
		Sessions.getCurrent().setAttribute("conditionEvents",conditionEvents);
		Sessions.getCurrent().setAttribute("alertRules",alertRules);
		Sessions.getCurrent().setAttribute("alertCategory", alertCategory);
		addAlertRules(RuleTypeId);
	}
	
	
	@Command("fromExistingAlertRules")
	public void fromExistingAlertRules(@BindingParam("ruleId") long ruleId)
			throws Exception {
		Map<String, Object> customFilterRequest = getCustomFilterRequest(filterRequest);
		RulesCoreConstants rulesCoreConstants = new RulesCoreConstants();
		cmDataByRuleId = new CMDataByRuleId();
		cmDataByRuleId.setFilterId(ruleId);
		try {
			cmAlertRulesExportResponse = ((AlertRulesFacade) entityFacade)
					.getAlertRulesExportResponse(cmDataByRuleId);
			conditionEvents = (List) cmAlertRulesExportResponse.getAlertRulesCondition();

			alertRules = (List) cmAlertRulesExportResponse.getAlertRules();
		} catch (RestException ex) {
			WebUtil.showErrorBox(ex,
					SQLCMI18NStrings.FAILED_TO_LOAD_FILTERED_ALERTS);
		}		
		String RuleTypeId =Integer.toString(alertRules.get(0).getAlertType());
		for (Map.Entry<String, String> entry : rulesCoreConstants.RulesType()
				.entrySet()) {

			if (entry.getKey().equals(RuleTypeId)) {
				RuleTypeId = entry.getValue();
			}
		}
		Sessions.getCurrent().setAttribute("QueryType", "FromExisting");
		Sessions.getCurrent().setAttribute("QueryTypeForColumn","FromExisting");
		Sessions.getCurrent().setAttribute("conditionEvents",conditionEvents);
		Sessions.getCurrent().setAttribute("alertRules",alertRules);
		addAlertRules(RuleTypeId);
	}
	
		@Command("importFile")
	public void importFile() throws JRException, IOException,
			TransformerException, ParserConfigurationException {
		for (Map.Entry<String, String> EventDataValues : MainEventData
				.entrySet()) {
			System.out.println("----" + EventDataValues.getKey() + "::"
					+ EventDataValues.getValue());
		}
	}
	
	@Command("uploadFile")
	public void uploadFile(@ContextParam(ContextType.TRIGGER_EVENT) UploadEvent event) {				
		String strRet = "0";
		Media media=event.getMedia();		
		strRet = ReadXMLContent(media);
		
		
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
					new Exception(ELFunctions.getLabel(SQLCMI18NStrings.INVALID_FILE_FORMAT_ALERT_RULES_IMPORT)),
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
				File xmlFile= new File(media.getName());
				String xmlPath=media.getStringData();
				AlertRulesFacade alertRulesFacade=new AlertRulesFacade();
				String statusMessage=alertRulesFacade.importAlertRules(xmlPath);
				if(!statusMessage.equalsIgnoreCase("failed")){
					refreshEntitiesList();
					WebUtil.showInfoBoxWithCustomMessage("Successfully Imported.");
				   setGridRowsCount();
				
				}
				else{
					WebUtil.showInfoBoxWithCustomMessage("Import failed.");
				}			
			}
			catch(RestException e){WebUtil.showInfoBoxWithCustomMessage("Import failed.");}
		}
	}

	public String ReadXMLContent(Media media) {
		if (media == null || !media.getName().endsWith(".xml")) {
			return "1";			
		}
		try {
			String FilEName = media.getName();
			String file = media.getStringData();

			DocumentBuilder dBuilder = DocumentBuilderFactory.newInstance()
					.newDocumentBuilder();

			InputSource is = new InputSource(new StringReader(file));
			Document doc = dBuilder.parse(is);
			
			if(doc.getDocumentElement().getNodeName().equals("AlertRuleTemplate"))
			{
				System.out.println("Root element :"
						+ doc.getDocumentElement().getNodeName());
	
				if (doc.hasChildNodes()) {
	
					printNote(doc.getChildNodes());
					
				}
				return "3";
			}
			else
			{
				return "2";
			}

		} 
		catch (Exception e) {
			System.out.println(e.getMessage());
			return "4";
		}

	}

	private void printNote(NodeList nodeList) 
	{
		for (int count = 0; count < nodeList.getLength(); count++) 
		{
		  Node tempNode = nodeList.item(count);

			if (tempNode.getNodeType() == Node.ELEMENT_NODE) 
			{
				/*System.out.println("\nNode Name =" + tempNode.getNodeName()	+ " [OPEN]");
				System.out.println("All Node Value =" + tempNode.getTextContent());*/
		
				if (tempNode.getNodeName().equals("Condition"))
					iConditionCount++;
		
				if (iConditionCount == 0) 
				{
					MainEventData.put(tempNode.getNodeName(),tempNode.getTextContent());
				}
				else
				{
					if(tempNode.getNodeName() == "MessageData")
					{
						MainEventData.put(tempNode.getNodeName(), tempNode.getTextContent());
					}
					else
					{
						MainEventData.put(tempNode.getNodeName() + "_" + iConditionCount, tempNode.getTextContent());
					}
				}
					
				if (tempNode.getNodeName().equals("TargetInstance"))
				{
					TargetInstance += tempNode.getTextContent() + "," ;
				}
		
				iCount++;
		
				if (tempNode.hasAttributes()) 
				{
					NamedNodeMap nodeMap = tempNode.getAttributes();
		
					for (int i = 0; i < nodeMap.getLength(); i++) 
					{
		
						Node node = nodeMap.item(i);
						System.out.println("attr name : " + node.getNodeName());
						System.out.println("attr value : " + node.getNodeValue());
						if (tempNode.getNodeName().equals("Condition"))
						{
							if (iConditionCount > 0)
							{
								MainEventData.put(tempNode.getNodeName() + "_" + iConditionCount + "_" + node.getNodeName(),node.getNodeValue());
							}
						}
						else
						{
							MainEventData.put(tempNode.getNodeName() + "_" + node.getNodeName(), node.getNodeValue());
						}
						iCount++;
					}
				}
		
				if (tempNode.hasChildNodes()) 
				{
					printNote(tempNode.getChildNodes());
				}
				
				/*System.out.println("Node Name =" + tempNode.getNodeName() + " [CLOSE]");*/
			}
		}
	}
	
	@Command("snmpTrapFire")
	public void snmpTrapFire() {

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
