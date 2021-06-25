package com.idera.sqlcm.ui.dialogs.eventFilterWizard.steps;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.EventType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.CMEventFilterRules;
import com.idera.sqlcm.entities.CMExportEvent;
import com.idera.sqlcm.entities.CMExportEventConditionData;
import com.idera.sqlcm.entities.CMExportEventType;
import com.idera.sqlcm.entities.CategoryData;
import com.idera.sqlcm.entities.CategoryRequest;
import com.idera.sqlcm.entities.CategoryResponse;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.NewEventFilterRules;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.EventFilterSaveEntity;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.RegulationSettings;

import java.util.List;

import org.apache.tools.ant.taskdefs.condition.IsSet;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Textbox;

import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.Set;

public class SelectNewEventStepViewModel extends AddWizardStepBase {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/eventFilterWizard/steps/select-new-event-step.zul";
	private ListModelList<CMEventFilterRules> eventFilterRulesList;

	protected AccessUserCheck accessUserCheck;
	EventCondition eventCondition = new EventCondition();
    EventField eventtype = new EventField();
	protected EventAlertLevel eventAlertLevel;
	protected EventTypeFilter eventTypeFilter;
	protected EventFilterCategory eventFilterCategory;
	protected EventFilterType eventFilterType;
	private ListModelList<FilterEventsCategory> intervalListModelList;
	private String eventFilterName = "New Event Filter";
	private String eventFilterDescription;
	int[] certainId = new int[1];
	int eventTypeIndex = 0;
	private Map<String,Long> checkedEventTypes = new HashMap<String,Long>();
	
	@Wire
	private Radiogroup rgAlertRules;
	
	@Wire
    private Combobox SelectEventTypeMain;
	
	List<CMExportEventType> eventType;
    
    @Wire
    private Combobox SelectEventTypeSub;
    

    List<CategoryData> entitiesList;
    
    public List<CategoryData> getEntitiesList() {
		return entitiesList;
	}

	public void setEntitiesList(List<CategoryData> entitiesList) {
		this.entitiesList = entitiesList;
	}

	@Wire
	Combobox SelectCategory;
	Listbox SelectEventType1;
	Listbox SelectEventType2;
	private Map<String, Long> checkedCategoryTypes = new HashMap<String, Long>();

	/*
	 * public String getFilterDescription() { return FilterDescription; }
	 * 
	 * public void setFilterDescription(String filterDescription) {
	 * FilterDescription = filterDescription; }
	 */

	@Wire("Listbox")
	Listbox cm_LstBox;

	public String getEventFilterName() {
		return eventFilterName;
	}

	public void setEventFilterName(String eventFilterName) {
		this.eventFilterName = eventFilterName;
	}

	public String getEventFilterDescription() {
		return eventFilterDescription;
	}

	public void setEventFilterDescription(String eventFilterDescription) {
		this.eventFilterDescription = eventFilterDescription;
	}

	protected FilterEventsCategory filterEventsCategory;
	CMEventFilterRules cmEventFilterRule = new CMEventFilterRules();
	private FilterEventsCategory currentInterval = FilterEventsCategory.ALL;
	NewEventFilterRules newEventRules;

	public static enum EventTypeFilter {
		EVENTTYPE_ADMIN(ELFunctions.getLabel(SQLCMI18NStrings.ADMIN), 1), EVENTTYPE_DDL(
				ELFunctions.getLabel(SQLCMI18NStrings.DDL), 2), EVENTTYPE_DML(
				ELFunctions.getLabel(SQLCMI18NStrings.DML), 3), EVENTTYPE_LOGIN(
				ELFunctions.getLabel(SQLCMI18NStrings.LOGIN), 4), EVENTTYPE_SECURITY(
				ELFunctions.getLabel(SQLCMI18NStrings.SECURITY), 5), EVENTTYPE_SELECT(
				ELFunctions.getLabel(SQLCMI18NStrings.SELECT), 6), EVENTTYPE_USER_DEFINED(
				ELFunctions.getLabel(SQLCMI18NStrings.USER_DEFINED), 7);

		private String label;

		private int id;

		EventTypeFilter(String label, int id) {
			this.label = label;
			this.id = id;
		}

		public int getId() {
			return id;
		}
	}

	@Wire("Textbox")
	Textbox FilterName;

	@Wire
	private Textbox mc_eventFilterDescription;

	/* Textbox FilterDescription; */

	public static enum EventFilterCategory {
		CAT_ADMIN(ELFunctions.getLabel(SQLCMI18NStrings.ADMIN), 1), CAT_DDL(
				ELFunctions.getLabel(SQLCMI18NStrings.DDL), 2), CAT_DML(
				ELFunctions.getLabel(SQLCMI18NStrings.DML), 3), CAT_LOGIN(
				ELFunctions.getLabel(SQLCMI18NStrings.LOGIN), 4), CAT_SECURITY(
				ELFunctions.getLabel(SQLCMI18NStrings.SECURITY), 5), CAT_SELECT(
				ELFunctions.getLabel(SQLCMI18NStrings.SELECT), 6), CAT_USER_DEFINED(
				ELFunctions.getLabel(SQLCMI18NStrings.USER_DEFINED), 7);

		private String label;
		private int id;

		EventFilterCategory(String label, int id) {
			this.label = label;
			this.id = id;
		}

		public int getId() {
			return id;
		}
	}

	public static enum EventAlertLevel {
		LOW(ELFunctions.getLabel(SQLCMI18NStrings.LOW_ALERT_RULES), 1), MEDIUM(
				ELFunctions.getLabel(SQLCMI18NStrings.MEDIUM_ALERT_RULES), 2), HIGH(
				ELFunctions.getLabel(SQLCMI18NStrings.HIGH_ALERT_RULES), 3), SEVERE(
				ELFunctions.getLabel(SQLCMI18NStrings.SEVERE_ALERT_RULES), 4);

		private String label;

		private int id;

		EventAlertLevel(String label, int id) {
			this.label = label;
			this.id = id;
		}

		public int getId() {
			return id;
		}
	}

	public static enum EventFilterType {
		EVENT_ADMIN(ELFunctions.getLabel(SQLCMI18NStrings.ADMIN), 1), EVENT_DDL(
				ELFunctions.getLabel(SQLCMI18NStrings.DDL), 2), EVENT_DML(
				ELFunctions.getLabel(SQLCMI18NStrings.DML), 3), EVENT_LOGIN(
				ELFunctions.getLabel(SQLCMI18NStrings.LOGIN), 4), EVENT_SECURITY(
				ELFunctions.getLabel(SQLCMI18NStrings.SECURITY), 5), EVENT_SELECT(
				ELFunctions.getLabel(SQLCMI18NStrings.SELECT), 6), EVENT_USER_DEFINED(
				ELFunctions.getLabel(SQLCMI18NStrings.USER_DEFINED), 7);

		private String label;
		private int id;

		EventFilterType(String label, int id) {
			this.label = label;
			this.id = id;
		}

		public int getId() {
			return id;
		}
	}

	public static enum FilterEventsCategory {
		ALL(1, ELFunctions.getLabel(SQLCMI18NStrings.ALL_EVENTS)), CATEGORY(2,
				ELFunctions.getLabel(SQLCMI18NStrings.CATEGORY)), EVENT_TYPE(
				3,
				ELFunctions
						.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TYPE));

		private String label;
		private int index;

		private FilterEventsCategory(int index, String label) {
			this.label = label;
			this.index = index;

		}

		public String getLabel() {
			return label;
		}

		public String getName() {
			return this.name();
		}

		public int getIndex() {
			return index;
		}
	}

	private void initIntervalList(int selectedItem) {
		intervalListModelList = new ListModelList<>();
		intervalListModelList.add(FilterEventsCategory.ALL);
		intervalListModelList.add(FilterEventsCategory.CATEGORY);
		intervalListModelList.add(FilterEventsCategory.EVENT_TYPE);
		currentInterval = intervalListModelList.get(selectedItem);
		intervalListModelList.setSelection(Arrays.asList(currentInterval));
	}

	public SelectNewEventStepViewModel() {
		super();
		newEventRules = new NewEventFilterRules();
	}

	@Override
	public boolean isFirst() {
		return true;
	}

	@Override
	public String getNextStepZul() {
		return NewEventFilterStepViewModel.ZUL_PATH;
	}

	@Override
	public boolean isValid() {
		return true;
	}

	@Override
	public String getTips() {
		return ELFunctions
				.getLabel(SQLCMI18NStrings.SQL_SERVER_EVENT_TYPE_FILTER_TIPS);
	}

	@Command("selectCategorySource")
	public void selectCategorySource(@BindingParam("id") String id, @BindingParam("label") String label) throws RestException {
	 certainId[0] = Integer.parseInt(id);
	 newEventRules.setEventFilter(label);
	 //Set lstbox = SelectEventTypeMain.getSelectedItems();
	}
	
	@Command("selectAlertLevel")
	public void selectAlertLevel(@BindingParam("id") String id) {
		eventAlertLevel = EventAlertLevel.valueOf(id);
		newEventRules.setEventFilterLevel(eventAlertLevel.id);
	}

	@Command("selectEventCategory")
	public void selectedEventCategory(@BindingParam("id") String id) throws RestException {
		eventTypeIndex = 0;
		selectEventCategory(id);
	}
	public void selectEventCategory(String id) throws RestException {
		AlertRulesFacade alertRulesFacade = new AlertRulesFacade();
		CategoryResponse categoryResponse = new CategoryResponse();
		CategoryRequest categoryRequest = new CategoryRequest();
		categoryRequest.setCategory(id);
		categoryResponse = alertRulesFacade.getCategoryInfo(categoryRequest);
		entitiesList = categoryResponse.getCategoryTable();
		setEntitiesList(entitiesList);
		SelectEventTypeMain.setValue(entitiesList.get(eventTypeIndex).getName());
		newEventRules.setEventFilter(entitiesList.get(eventTypeIndex).getName());
		certainId[0] = entitiesList.get(eventTypeIndex).getEvtypeid();
		BindUtils.postNotifyChange(null, null, SelectNewEventStepViewModel.this, "*");
	}
	
	@Command("selectEventCategoryType")
	public void selectEventCategoryType(@BindingParam("id") String id, @BindingParam("label") String label){
		certainId[0] = Integer.parseInt(id.substring(4,id.length()));
		newEventRules.setEventFilter(label);
	}

	@Command("selectEventSource")
	public void selectEventSource(@BindingParam("id") String id) {
		eventTypeFilter = EventTypeFilter.valueOf(id);
		newEventRules.setEventFilterEventType(eventTypeFilter.id);
	}
	
	@Override
	public void onCancel(EventFilterSaveEntity wizardSaveEntity) {
		if(Sessions.getCurrent().getAttribute("QueryType")!=null)
		{
			Sessions.getCurrent().removeAttribute("QueryType");
		}
		
		String uri = "eventFiltersView";
		uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
		Executions.sendRedirect(uri);
	}
	
	@Command("onCheckCertainEvent")
	public void onCheckCertainEvent(@BindingParam("target") Checkbox target,@BindingParam("index") long index, 
			@BindingParam("param1") Combobox param1,@BindingParam("lstBoxSub") Combobox lstBoxSub,@BindingParam("lstBoxMain") Combobox lstBoxMain) throws Exception {
		if (target.isChecked()) {
			eventTypeIndex = 0;
			String chkName = target.getName();
			if (index == 0) {
				param1.setDisabled(true);
				lstBoxSub.setDisabled(true);
				lstBoxMain.setDisabled(true);
			}
			else if (index == 1) {
				param1.setDisabled(false);
				param1.setSelectedIndex(2);
				newEventRules.setEventFilter(param1.getValue());
				certainId[0] = 4;
				lstBoxSub.setDisabled(true);
				lstBoxMain.setDisabled(true);
			}
			else if(index ==2){	
				param1.setDisabled(true);
				lstBoxSub.setDisabled(false);
				lstBoxMain.setDisabled(false);
				selectEventCategory("DDL");
			}
		}
	}

	@Command("selectEventType")
	public void selectEventType(@BindingParam("id") String id) {
		eventFilterType = EventFilterType.valueOf(id);
		newEventRules.setEventFilterType(eventFilterType.id);
	}

	@Command("selectAddEventFilter")
	public void selectAddEventFilter(
			@BindingParam("radioGroup") Radiogroup radioGroup)
			throws RestException {
		getNextButton().setDisabled(false);
		int iSelected = radioGroup.getSelectedIndex();
		initIntervalList(iSelected);
		Set<FilterEventsCategory> selectedIntervals = intervalListModelList
				.getSelection();
		if(iSelected==0){
			String _strDefaultLevel = FilterEventsCategory.ALL.label;
			newEventRules.setEventFilter(_strDefaultLevel);
		}		
	}

	@Override
	public void onBeforeNext(EventFilterSaveEntity wizardSaveEntity) {
		// wizardSaveEntity.setNewEventAlertRules(newEventRules);
		newEventRules.setName(this.eventFilterName);
		newEventRules.setDescription(this.eventFilterDescription);
		if(rgAlertRules.getSelectedIndex()==2)
		{
        	eventtype.setDataFormat(MatchType.Integer);
    		eventtype.set_type(EventType.SqlServer);
    	    eventCondition.set_boolValue(false);
    	    eventCondition.set_nulls(false);
    		eventCondition.set_inclusive(true);
    		eventCondition.set_targetInts(certainId);
    		try {
    			String matchString =  eventCondition.UpdateMatchString(eventtype,eventCondition);
    			newEventRules.setEventFilterCategorMatchString(matchString);
    			newEventRules.setEventFilterCategoryFiledId(0);
    		} catch (Exception e) {
    			// TODO Auto-generated catch block
    			e.printStackTrace();
    		}
		}
		
		else if(rgAlertRules.getSelectedIndex()==1)
		{
        	eventtype.setDataFormat(MatchType.Integer);
    		eventtype.set_type(EventType.SqlServer);
    	    eventCondition.set_boolValue(false);
    	    eventCondition.set_nulls(false);
    		eventCondition.set_inclusive(true);
    		eventCondition.set_targetInts(certainId);
    		try {
    			String matchString =  eventCondition.UpdateMatchString(eventtype,eventCondition);
    			newEventRules.setEventFilterCategorMatchString(matchString);
    			newEventRules.setEventFilterCategoryFiledId(1);
    		} catch (Exception e) {
    			// TODO Auto-generated catch block
    			e.printStackTrace();
    		}
		}
		
		else{
			newEventRules.setEventFilterCategorMatchString(null);
		}
		
		wizardSaveEntity.setNewEventAlertRules(newEventRules);
		if(Sessions.getCurrent().getAttribute("QueryType")!=null){
			if(Sessions.getCurrent().getAttribute("isEnabled")!=null)
				Sessions.getCurrent().setAttribute("isThisEnabled",(boolean)Sessions.getCurrent().getAttribute("isEnabled"));
			RegulationSettings rs=new RegulationSettings();			
				if(Sessions.getCurrent().getAttribute("SQL Server")!=null){
					Map<String, Object> Instances = new HashMap<String, Object>();					
					String serverString=(String)Sessions.getCurrent().getAttribute("SQL Server");
					if(!serverString.equalsIgnoreCase("<ALL>")){
						rs.setSQLServer(true);
					}
					Instances.put(serverString, serverString);
					if (Instances != null && (!Instances.isEmpty())) {
						rs.setTargetInstances(Instances);
					}
				}       	
 			   if(Sessions.getCurrent().getAttribute("DbMatchString")!=null){    
 				    rs.setDatabaseName(true);
 		            rs.setDbMatchString((String)Sessions.getCurrent().getAttribute("DbMatchString"));    		
 		      	}
 			   if(Sessions.getCurrent().getAttribute("ObjectMatchString")!=null){
 				   rs.setObjectName(true);
 		    		rs.setObjectMatchString((String)Sessions.getCurrent().getAttribute("ObjectMatchString"));
 				}
 			  if(Sessions.getCurrent().getAttribute("HostMatchString")!=null){
 				  rs.setHostName(true);
 		    		rs.setHostMatchString((String)Sessions.getCurrent().getAttribute("HostMatchString"));
 		    	}
 			  if(Sessions.getCurrent().getAttribute("AppMatchString")!=null){
 				  rs.setApplicationName(true);
 	    		rs.setAppMatchString((String)Sessions.getCurrent().getAttribute("AppMatchString"));
 			  }
 			  if(Sessions.getCurrent().getAttribute("LoginMatchString")!=null){
 				rs.setLoginName(true);
 	    		rs.setLoginMatchString((String)Sessions.getCurrent().getAttribute("LoginMatchString"));
 			  }
 			  if(Sessions.getCurrent().getAttribute("sessionLoginMatchString")!=null){
 				  rs.setSessionLoginName(true);
 				 rs.setSessionLoginMatchString((String)Sessions.getCurrent().getAttribute("sessionLoginMatchString"));
 			  } 			   
 			  if(Sessions.getCurrent().getAttribute("PrivilegedMatchString")!=null){
	        	rs.setSessionPrivilegedMatchString((String) Sessions.getCurrent().getAttribute("PrivilegedMatchString"));	        	
 			   }        		
        	wizardSaveEntity.setRegulationSettings(rs);  
        }
	}
	
	@Override
    public void onFinish(EventFilterSaveEntity wizardSaveEntity){
    	onBeforeNext(wizardSaveEntity);
    	SummaryStepViewModel summaryStepViewModel=new SummaryStepViewModel();
    	summaryStepViewModel.onFinish(wizardSaveEntity);
    }
	

	public static enum AccessUserCheck {
		TRUE(ELFunctions.getLabel(SQLCMI18NStrings.TRUE), 1), FALSE(ELFunctions
				.getLabel(SQLCMI18NStrings.FALSE), 2);

		private String label;

		private int id;

		AccessUserCheck(String label, int id) {
			this.label = label;
			this.id = id;
		}

		public int getId() {
			return id;
		}
	}

	@Command("checkAccessUser")
	public void checkAccessUser(@BindingParam("id") String id) {
		accessUserCheck = AccessUserCheck.valueOf(id);
		boolean bvar = Boolean.parseBoolean(accessUserCheck.label);
		// rs.setAccessCheckPassed(bvar);
	}

	@Override
	public void doOnShow(EventFilterSaveEntity wizardSaveEntity) {
		String strName = "";
		String strDescription = "";

		strName = (String) Sessions.getCurrent().getAttribute("Name");
		if (strName != null) {
			strName = (String) Sessions.getCurrent().getAttribute("Name");
			strDescription = (String) Sessions.getCurrent().getAttribute(
					"Description");
			
			setEventFilterName(strName);
			setEventFilterDescription(strDescription);

			newEventRules.setDescription(strDescription);
			newEventRules.setName(strName);

			mc_eventFilterDescription.setValue(strDescription);
			
			eventType=(List<CMExportEventType>)Sessions.getCurrent().getAttribute("eventType");	
			if(Sessions.getCurrent().getAttribute("eventMatchString")!=null){
				rgAlertRules.setSelectedIndex(1);
				SelectCategory.setDisabled(false);
				String eventMatchString= (String) Sessions.getCurrent().getAttribute("eventMatchString");
				int eventId=Integer.parseInt(eventMatchString.substring(eventMatchString.lastIndexOf(")")+1,eventMatchString.length()));
				certainId[0]=eventId;
				
				switch (eventId) {
				case 1:
					SelectCategory.setSelectedIndex(3);
					break;
				case 2:
					SelectCategory.setSelectedIndex(1);
					break;
				case 3:
					SelectCategory.setSelectedIndex(4);
					break;
				case 4:
					SelectCategory.setSelectedIndex(2);
					break;
				case 5:
					SelectCategory.setSelectedIndex(5);
					break;
				case 6:
					SelectCategory.setSelectedIndex(0);
					break;
				case 9:
					SelectCategory.setSelectedIndex(6);
				default:
					break;
				}
			}
			else if(Sessions.getCurrent().getAttribute("eventTypeMatchString")!=null){
				try
				{
					selectEventCategory(eventType.get(0).getCategory());					
				}
				catch(RestException r){}
				
				String eventMatchString= (String) Sessions.getCurrent().getAttribute("eventTypeMatchString");
				int eventId=Integer.parseInt(eventMatchString.substring(eventMatchString.lastIndexOf(")")+1,eventMatchString.length()));
				
				rgAlertRules.setSelectedIndex(2);
				SelectEventTypeMain.setDisabled(false);
				SelectEventTypeSub.setDisabled(false);
				switch (eventType.get(0).getCategory().toUpperCase()) {
				case "ADMIN":
					SelectEventTypeSub.setSelectedIndex(0);
		    		break;
		    	case "DDL":
		    		SelectEventTypeSub.setSelectedIndex(1);
		    		break;
		    	case "DML":
		    		SelectEventTypeSub.setSelectedIndex(2);
		    		break;	
		    	case "LOGIN":
		    		SelectEventTypeSub.setSelectedIndex(3);
		    		break;
		    	case "SECURITY":
		    		SelectEventTypeSub.setSelectedIndex(4);
		    		break;
		    	case "SELECT":
		    		SelectEventTypeSub.setSelectedIndex(5);
		    		break;
		    	default:
		    		SelectEventTypeSub.setSelectedIndex(6);
		    		break;
				}
				
				certainId[0]=eventId;
		    	//eventCategory1=101;
		    	int temp=0;
		    	for (int i = 0; i < entitiesList.size(); i++) {
					int value = entitiesList.get(i).getEvtypeid();
					if (value == eventId) {
						temp=i;
						break;						
					}
				}
		    	eventTypeIndex = temp;
		    	try
				{
					selectEventCategory(eventType.get(0).getCategory());					
				}
				catch(RestException r){}
			}
			
		} else {
			int _defaultLevel;
			String _strDefaultLevel;

			_defaultLevel = wizardSaveEntity.getEventFilterCategory();

			_defaultLevel = EventAlertLevel.MEDIUM.id;
			newEventRules.setEventFilterLevel(_defaultLevel);

			_defaultLevel = EventTypeFilter.EVENTTYPE_DDL.id;
			newEventRules.setEventFilterEventType(_defaultLevel);

			_defaultLevel = EventFilterType.EVENT_DDL.id;
			newEventRules.setEventFilterType(_defaultLevel);

			_strDefaultLevel = FilterEventsCategory.ALL.label;
			newEventRules.setEventFilter(_strDefaultLevel);

			strDescription = newEventRules.getDescription();
			strName = newEventRules.getName();
		}
	}
	
	@Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
    }
}