package com.idera.sqlcm.ui.dialogs.addalertruleswizard.steps;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCategory;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.CategoryData;
import com.idera.sqlcm.entities.CategoryRequest;
import com.idera.sqlcm.entities.CategoryResponse;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.KeyValueParser;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.NewEventAlertRules;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.EventType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.RegulationSettings;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.SelectAlertActions;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps.NewStatusAlertRuleStepViewModel;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps.NewStatusAlertRuleStepViewModel.Category;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.steps.SelectNewEventStepViewModel;

import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Label;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

import javax.mail.Session;

public class NewAlertRulesStepViewModel extends AddWizardStepBase {

	

	public static final String ZUL_PATH = "~./sqlcm/dialogs/addalertruleswizard/steps/new-event-alert-rules-step.zul";
	private ListModelList<CMAlertRules> alertRulesList;
	protected AlertsRuleSource alertsRuleSource;
	private ListModelList<Category> intervalListModelList;
	protected Category category;
	CMAlertRules cmAlertRule= new CMAlertRules();
	NewEventAlertRules newEventRules;
	private Category currentInterval = Category.SECURITY_CHANGES_ALERT_RULES;
	
	private String eventAlertDescription;
	private Label label;
	EventCondition eventCondition = new EventCondition();
	EventField eventtype = new EventField();
    public int[] _targetInts ={};
    int alertType = 1;
    private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCondition> conditionEvents;
	private List<CMAlertRulesCategory> categoryEvents;
	public KeyValueParser keyValueParser;
	public int eventTypeIndex = 0;
	public String comboToolTip="Create index";

	
	int[] certainId = new int[1];	

	@Wire
    private Listbox SelectEventTypeMain;
	
	@Wire
	private Textbox eventAlertName;
	
	@Wire
	private Combobox eventAlertRules;
	
	List<CategoryData> entitiesList;	

	RegulationSettings rs=new RegulationSettings();
    
    public List<CategoryData> getEntitiesList() {
		return entitiesList;
	}
    
    
    public String getComboToolTip() {
		return comboToolTip;
	}
    public void setComboToolTip(String comboToolTip) {
		this.comboToolTip = comboToolTip;
	}
    
	public void setEntitiesList(List<CategoryData> entitiesList) {
		this.entitiesList = entitiesList;
	}
	
	@Wire
	private Radiogroup rgAlertRules;
	
	@Wire
	private Combobox SelectEventSub;
	
	@Wire
	private Combobox SelectEventMain;
	
    public static enum AlertsRuleSource
    {
        LOW(ELFunctions.getLabel(SQLCMI18NStrings.LOW_ALERT_RULES), 1),
        MEDIUM(ELFunctions.getLabel(SQLCMI18NStrings.MEDIUM_ALERT_RULES), 2),
        HIGH(ELFunctions.getLabel(SQLCMI18NStrings.HIGH_ALERT_RULES), 3),
        SEVERE(ELFunctions.getLabel(SQLCMI18NStrings.SEVERE_ALERT_RULES), 4);
       
       
        private String label;

        private int id;

        AlertsRuleSource(String label, int id) {
            this.label = label;
            this.id = id;
       }

        public int getId() {
            return id;
        }
    }
    
    public static enum Category {
    	SECURITY_CHANGES_ALERT_RULES(3, ELFunctions.getLabel(SQLCMI18NStrings.SECURITY_CHANGES_ALERT_RULES)),//TODO AS ask .NET team id
    	ADMINISTRATIVE_ACTIVITY_ALERT_RULES(6, ELFunctions.getLabel(SQLCMI18NStrings.ADMINISTRATIVE_ACTIVITY_ALERT_RULES)),
    	LOGIN_ACTIVITY_ALERT_RULES(1, ELFunctions.getLabel(SQLCMI18NStrings.LOGIN_ACTIVITY_ALERT_RULES)),
    	SPECIFIC_EVENT_ALERT_RULES(101, ELFunctions.getLabel(SQLCMI18NStrings.SPECIFIC_EVENT_ALERT_RULES)),
    	DATA_DEFINITION_ALERT_RULES(2, ELFunctions.getLabel(SQLCMI18NStrings.DATA_DEFINITION_ALERT_RULES)),
    	DATA_MANIPULATION_ALERT_RULES(4, ELFunctions.getLabel(SQLCMI18NStrings.DATA_MANIPULATION_ALERT_RULES)),
    	USER_DEFINED_ACTIVITY_ALERT_RULES(9, ELFunctions.getLabel(SQLCMI18NStrings.USER_DEFINED_ACTIVITY_ALERT_RULES));

        private String label;
        private int index;

        private Category(int index, String label) {
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
    
    private void initIntervalList(int selectedIndex) {
        intervalListModelList.add(Category.SECURITY_CHANGES_ALERT_RULES);
        intervalListModelList.add(Category.ADMINISTRATIVE_ACTIVITY_ALERT_RULES);
        intervalListModelList.add(Category.LOGIN_ACTIVITY_ALERT_RULES);
        intervalListModelList.add(Category.SPECIFIC_EVENT_ALERT_RULES);
        intervalListModelList.add(Category.DATA_DEFINITION_ALERT_RULES);
        intervalListModelList.add(Category.DATA_MANIPULATION_ALERT_RULES);
        intervalListModelList.add(Category.USER_DEFINED_ACTIVITY_ALERT_RULES);
        currentInterval = intervalListModelList.get(selectedIndex);
        intervalListModelList.setSelection(Arrays.asList(currentInterval));
    }
        
    public NewAlertRulesStepViewModel() {
        super();
        newEventRules = new NewEventAlertRules();
        intervalListModelList = new ListModelList<>();
        newEventRules.setAlertType(alertType);
    }

    @Override
    public boolean isFirst() {
        return true;
    }

    @Override
    public String getNextStepZul() {
        return SelectEventFilterStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.SQL_SERVER_EVENT_TYPE_TIPS);
    }
    
    @Override
    public void doOnShow(AddAlertRulesSaveEntity wizardSaveEntity){
    	newEventRules.setFieldId(1);
    	 int _defaultLevel= alertsRuleSource.MEDIUM.id;
    	 if(newEventRules.getAlertLevel()==0)
    		 newEventRules.setAlertLevel(_defaultLevel);    	  
    	newEventRules.setEventFilter(currentInterval.label);    	
    	 if(Sessions.getCurrent().getAttribute("QueryType")!=null && 
    			 (newEventRules.getMatchString() == null || newEventRules.getMatchString().isEmpty())){
     		conditionEvents = (List<CMAlertRulesCondition>) Sessions.getCurrent().getAttribute("conditionEvents");
     		alertRules =(List<CMAlertRules>) Sessions.getCurrent().getAttribute("alertRules");
     		categoryEvents = (List<CMAlertRulesCategory>) Sessions.getCurrent().getAttribute("alertCategory");
     		if(conditionEvents!=null && alertRules!=null);
     		{
     			try {
     				if(Sessions.getCurrent().getAttribute("Category")!=null){
     				for (int i = 0; i < conditionEvents.size(); i++) {    		   
     				   long conditionId = conditionEvents.get(i).getConditionId();
     				   long fieldId = conditionEvents.get(i).getFieldId();
	     				   if(fieldId==1){     					   
	     					   conditionEvents.get(i).setMatchString((String)Sessions.getCurrent().getAttribute("Category"));
	     				   }	   
     				  }
     				}
     	    	
					initializer(alertRules,conditionEvents,categoryEvents);
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
     			BindUtils.postNotifyChange(null, null, NewAlertRulesStepViewModel.this, "*"); 
     		}
     	}
    	initIntervalList(rgAlertRules.getSelectedIndex());
     	Set<Category> selectedIntervals = intervalListModelList.getSelection(); // must contain only 1 item because single selection mode.
         if (selectedIntervals != null && !selectedIntervals.isEmpty()) {
             for (Category i : selectedIntervals) {
                 currentInterval = i;  
                 if(rgAlertRules.getSelectedIndex()!=3)
                	 newEventRules.setEventFilter(currentInterval.label);
                 break;
             }   
         }	
    }
    
    @Command("selectCategorySource")
	public void selectCategorySource(@BindingParam("id") String id,@BindingParam("name") String name) throws RestException {
    	if(id == null)
    		id = "101";
	 certainId[0] = Integer.parseInt(id);
	 newEventRules.setFieldId(0);
	 comboToolTip = name;
	 BindUtils.postNotifyChange(null, null,this, "comboToolTip");
	}    
    
    
    
    @Command("selectEventCategoryById")
    public void selectEventCategoryById(@BindingParam("id") String id) throws RestException {
    	eventTypeIndex = 0;
    	selectEventCategory(id);
    }
    
	public void selectEventCategory(String id) throws RestException {
		AlertRulesFacade alertRulesFacade = new AlertRulesFacade();
		CategoryResponse categoryResponse = new CategoryResponse();
		CategoryRequest categoryRequest = new CategoryRequest();
		String cat=id.substring(4, id.length());
		Sessions.getCurrent().setAttribute("eventCat",cat);
		categoryRequest.setCategory(cat.toLowerCase());
		categoryResponse = alertRulesFacade.getCategoryInfo(categoryRequest);
		entitiesList = categoryResponse.getCategoryTable();
		setEntitiesList(entitiesList);
		ListModelList<CategoryData> entitiesListForEventType = new ListModelList<CategoryData>(entitiesList);
    	SelectEventMain.setModel(entitiesListForEventType);
    	SelectEventMain.setValue(entitiesListForEventType.get(eventTypeIndex).getName());  	  
    	newEventRules.setEventFilter(entitiesListForEventType.get(eventTypeIndex).getName());
    	certainId[0]= entitiesListForEventType.get(eventTypeIndex).getEvtypeid();
    	newEventRules.setFieldId(0);
    	BindUtils.postNotifyChange(null, null, NewAlertRulesStepViewModel.this, "*");
	}
    
    public void initializer(List<CMAlertRules> alertRules,List<CMAlertRulesCondition> conditionEvents,  List<CMAlertRulesCategory> alertCategory) throws Exception{
    	
    	int eventCategory = 0;
    	String matchString = null;
    	String tempMatchString = null;
    	keyValueParser= new KeyValueParser();
    	eventAlertName.setValue(alertRules.get(0).getNames());
    	setEventAlertDescription(alertRules.get(0).getDescription());
    	eventAlertRules.setSelectedIndex(alertRules.get(0).getAlertLevel()-1);
    	newEventRules.setAlertLevel(alertRules.get(0).getAlertLevel());
    	rgAlertRules.setSelectedIndex(alertRules.get(0).getAlertType());
    	if(alertRules.get(0).getTargetInstances()!=null & alertRules.get(0).getTargetInstances()!= "<all>" ){
    		 Sessions.getCurrent().setAttribute("SQL Server",alertRules.get(0).getTargetInstances());  		   
    	   }
    	java.util.Map<String, String> EventFilterConditionId = new HashMap();
    	java.util.Map<String, String> ExportList = new HashMap();
		EventFilterConditionId.put("0", "" + alertRules.get(0).getRuleId());
		int tempFieldId=0;
    	for (int i = 0; i < conditionEvents.size(); i++) {    		   
			   long conditionId = conditionEvents.get(i).getConditionId();
			   long fieldId = conditionEvents.get(i).getFieldId();
			   if(fieldId==1 || fieldId == 0){	
				   if(conditionEvents.get(i).getMatchString().indexOf("include(1)1")!=-1){
					   if(tempMatchString == null || fieldId==1){
						   tempMatchString = conditionEvents.get(i).getMatchString();
					   		tempFieldId = (int)fieldId;
					   }
				   }
			   }
			   if(fieldId==0 && conditionEvents.get(i).getMatchString().indexOf("include(1)0")!=-1){
				   String temp = conditionEvents.get(i).getMatchString();
				   int index=temp.lastIndexOf(")")+1;
				   temp = temp.substring(index,temp.length());
				   Sessions.getCurrent().setAttribute("ExecludeCertainEventIds",temp);
			   }
			   matchString = conditionEvents.get(i).getMatchString();
			   String keyVal = "" + (i + 1);
			   ExportList.put(keyVal, conditionId + "," + fieldId + ","
			     + matchString);			   
			  }
    	
    	int tempEventCatagory = 0;
    	int eventPosition = tempMatchString.lastIndexOf(")");
    	eventCategory=Integer.parseInt(String.valueOf(tempMatchString.substring(eventPosition+1,tempMatchString.length())));
		newEventRules.setFieldId(tempFieldId);
    	if(tempFieldId!=0)
    		tempEventCatagory = eventCategory;
    	else
    		tempEventCatagory=101;
		    switch (tempEventCatagory) {
		    case 3:
		    	rgAlertRules.setSelectedIndex(0);
		    	Sessions.getCurrent().setAttribute("eventTypeId", "SECURITY");
		    	break;
		    case 6:
		    	rgAlertRules.setSelectedIndex(1);
		    	Sessions.getCurrent().setAttribute("eventTypeId", "ADMIN");
		    	break;
		    case 1:
		    	rgAlertRules.setSelectedIndex(2);
		    	Sessions.getCurrent().setAttribute("eventTypeId", "LOGIN");
		    	break;
		    case 2:
		    	rgAlertRules.setSelectedIndex(4);
		    	Sessions.getCurrent().setAttribute("eventTypeId", "DDL");
		    	break;
		    case 4:
		    	rgAlertRules.setSelectedIndex(5);
		    	Sessions.getCurrent().setAttribute("eventTypeId", "DML"); 
		    	break;
		    case 9:
		    	rgAlertRules.setSelectedIndex(6);
		    	Sessions.getCurrent().setAttribute("eventTypeId", "USER_DEFINED");
		    	break;
	
		    default:
		    	rgAlertRules.setSelectedIndex(3);
		    	SelectEventSub.setDisabled(false);
		    	SelectEventMain.setDisabled(false);
		    	String eventCat="";
		    	if(Sessions.getCurrent().getAttribute("eventCat")!=null){
		    		eventCat=(String)Sessions.getCurrent().getAttribute("eventCat");
		    	}
		    	else{
		    		eventCat= alertCategory.get(0).getCategory().toUpperCase();
		    	}
		    	selectEventCategory("CAT_"+eventCat);
		    	switch(eventCat){
		    	case "ADMIN":
		    		SelectEventSub.setSelectedIndex(0);
		    		break;
		    	case "DDL":
		    		SelectEventSub.setSelectedIndex(1);
		    		break;
		    	case "DML":
		    		SelectEventSub.setSelectedIndex(2);
		    		break;	
		    	case "LOGIN":
		    		SelectEventSub.setSelectedIndex(3);
		    		break;
		    	case "SECURITY":
		    		SelectEventSub.setSelectedIndex(4);
		    		break;
		    	case "SELECT":
		    		SelectEventSub.setSelectedIndex(5);
		    		break;
		    	case "USER DEFINED":
		    		SelectEventSub.setSelectedIndex(6);
		    		break;	
		    	} 
		    	
		    	Sessions.getCurrent().setAttribute("eventTypeId", "101");
		    	certainId[0]=eventCategory;
		    	int temp=0;
		    	for (int i = 0; i < entitiesList.size(); i++) {
					int value = entitiesList.get(i).getEvtypeid();
					if (value == eventCategory) {
						temp=i;
						break;
					}
				}
		    	eventTypeIndex = temp;
		    	selectEventCategory("CAT_"+eventCat);
		    }    	
		SetDataValues(ExportList, EventFilterConditionId);
      }
    
    public void SetDataValues(Map<String, String> ExportList, Map<String, String> EventFilterConditionId) {
  	  String strEventFieldId = "";
  	  String strEventDataVal = "";
  	  String strEventConditionId = "";
  	  int Counter = 1;
  	 
  	  java.util.Map<String, String> EventFilterConditionData = new HashMap();

  	  for (int p = 0; p < ExportList.size(); p++) {
  	   EventFilterConditionData.put("" + p, ExportList.get("" + (p + 1)));
  	  }
  	  

 	   if(alertRules.get(0).getTargetInstances()!=null){
 		 Sessions.getCurrent().setAttribute("SQL Server",alertRules.get(0).getTargetInstances());  		   
 	   } 

  	  for (Map.Entry<String, String> EventDataValues : EventFilterConditionData
  	    .entrySet()) {

			strEventDataVal = EventDataValues.getValue();
			String[] strArray;
			strArray = strEventDataVal.split(",",3);
			strEventDataVal = strArray[2];
			strEventFieldId = strArray[1];

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
  	    Sessions.getCurrent().setAttribute("hostMatchString",strEventDataVal);
  	    rs.setHostName(true);
  	   }
  	   if (strEventFieldId.equals("9")){
   	    Sessions.getCurrent().setAttribute("SQL Server",strEventDataVal);
   	    rs.setSqlServer(true);
   	    }
   	   if (strEventFieldId.equals("8")){
   	    Sessions.getCurrent().setAttribute("Object Type",strEventDataVal);
   	    rs.setObjectName(true);
   	    }
  	   if (strEventFieldId.equals("7")){
  	    Sessions.getCurrent().setAttribute("PrivilegedMatchString",strEventDataVal);
  	    }
  	   if (strEventFieldId.equals("6")){
  	    Sessions.getCurrent().setAttribute("objectMatchString",strEventDataVal);
  	    rs.setObjectName(true);
  	    }
  	   if (strEventFieldId.equals("5")){
  	    Sessions.getCurrent().setAttribute("dbMatchString",strEventDataVal);
  	    rs.setDatabaseName(true);
  	    }
  	   if (strEventFieldId.equals("4")){
   	    Sessions.getCurrent().setAttribute("Access Check Passed",strEventDataVal);   	    
   	    }
  	   if (strEventFieldId.equals("3")){
  	    Sessions.getCurrent().setAttribute("loginMatchString",strEventDataVal);
  	    rs.setLoginName(true);
  	    }
  	   if (strEventFieldId.equals("2")){
  	    Sessions.getCurrent().setAttribute("appMatchString",strEventDataVal);
  	    rs.setApplicationName(true);
  	    }
  	   if (strEventFieldId.equals("1")){
  		    Sessions.getCurrent().setAttribute("Category",strEventDataVal);
  		}
  	   if (strEventFieldId.equals("14")){
  		 	Sessions.getCurrent().setAttribute("rowCountDetails",strEventDataVal);
  	    }
	   if (strEventFieldId.equals("13")){
  		 	Sessions.getCurrent().setAttribute("privilegedUserNameMatch",strEventDataVal);
  	    }  	   
  	   if (strEventFieldId.equals("0")){
  		   if(strEventDataVal.indexOf("include(1)0")!=-1){
  		    Sessions.getCurrent().setAttribute("Type", strEventDataVal);
  		    rs.setExcludeCertainEventType(true); 
  		   }
  		   else
  			Sessions.getCurrent().setAttribute("Category",strEventDataVal);
  		}
  	  } 
  	  Sessions.getCurrent().setAttribute("EventConditionId", EventFilterConditionId);
  	 }    

    @Command("selectEventSource")
    public void selectEventSource(@BindingParam("id") String id) {
        alertsRuleSource = AlertsRuleSource.valueOf(id);
        newEventRules.setAlertLevel(alertsRuleSource.id);
    }
    
    public String getEventAlertDescription() {
        return eventAlertDescription;
    }
  
    public void setEventAlertDescription(String eventAlertDescription) {
        this.eventAlertDescription = eventAlertDescription;
    }
    
    
    @Command("selectAddEventFilter")
    public void selectAddEventFilter(@BindingParam("radioGroup") Radiogroup radioGroup) throws RestException {
    	getNextButton().setDisabled(false);
    	int iSelected = radioGroup.getSelectedIndex();
    	Sessions.getCurrent().removeAttribute("ExecludeCertainEventIds");
    	if(iSelected != 3)
    	{
    		SelectEventSub.setDisabled(true);
	    	SelectEventMain.setDisabled(true);
	    }

		newEventRules.setFieldId(1);
    	Sessions.getCurrent().removeAttribute("Type");
    	switch (iSelected) {
	    case 0:
	    	Sessions.getCurrent().setAttribute("eventTypeId", "SECURITY"); 
	    	break;
	    case 1:
	    	Sessions.getCurrent().setAttribute("eventTypeId", "ADMIN"); 
	    	break;
	    case 2:
	    	Sessions.getCurrent().setAttribute("eventTypeId", "LOGIN"); 
	    	break;
	    case 4:
	    	Sessions.getCurrent().setAttribute("eventTypeId", "DDL"); 
	    	break;
	    case 5:
	    	Sessions.getCurrent().setAttribute("eventTypeId", "DML"); 
	    	break;
	    case 6:
	    	Sessions.getCurrent().setAttribute("eventTypeId", "USER_DEFINED"); 
	    	break;
	    case 3:
	    	SelectEventSub.setDisabled(false);
	    	SelectEventSub.setValue("DDL");
			SelectEventMain.setDisabled(false);
			eventTypeIndex = 0;
			selectEventCategory("CAT_DDL");
	    	Sessions.getCurrent().setAttribute("eventTypeId", "101");
	    	break;
	 }   	    	
    	initIntervalList(iSelected);
    	Set<Category> selectedIntervals = intervalListModelList.getSelection(); // must contain only 1 item because single selection mode.
        if (selectedIntervals != null && !selectedIntervals.isEmpty()) {
            for (Category i : selectedIntervals) {
                currentInterval = i; 
                if(iSelected!=3)
                	newEventRules.setEventFilter(currentInterval.label);
                break;
            }   
        }
    	
    }
    
    public int[] GetTargetInstt()
	{ 
     _targetInts = new int[1];
	 _targetInts[0] = currentInterval.index;
	 if(_targetInts[0]==101)
	 {
		 return certainId;
	 }
	 return _targetInts;
     }
    
    
    public void GetMatchString(){
    	eventtype.setDataFormat(MatchType.Integer);
		eventtype.set_type(EventType.SqlServer);
	    eventCondition.set_boolValue(false);
	    eventCondition.set_nulls(false);
		eventCondition.set_inclusive(true);
		eventCondition.set_targetInts(GetTargetInstt());
		try {
			String matchString =  eventCondition.UpdateMatchString(eventtype,eventCondition);
			newEventRules.setMatchString(matchString);
			Sessions.getCurrent().setAttribute("Category",matchString);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    }
    
    public void eventAlertName(@BindingParam("val") String val) {
    	String eventAlertRuleName = val;
    	if(eventAlertRuleName!=null && eventAlertRuleName!=""){
    		CMAlertRules cmAlertRule= new CMAlertRules();
    		cmAlertRule.setName(eventAlertRuleName);
    	}
    }
    @Override
	public void onCancel(AddAlertRulesSaveEntity wizardSaveEntity) {
    	if(Sessions.getCurrent().getAttribute("QueryType")!=null)
    	{
    		Sessions.getCurrent().removeAttribute("QueryType");
    		Sessions.getCurrent().removeAttribute("Category");
    	}
    	
    	 String uri = "instancesAlertsRule";
         uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
         Executions.sendRedirect(uri);
	}
    
    @Override
    public void onBeforeNext(AddAlertRulesSaveEntity wizardSaveEntity) {
    	newEventRules.setName(eventAlertName.getValue());
    	newEventRules.setDescription(this.eventAlertDescription);
    	GetMatchString();
        wizardSaveEntity.setNewEventAlertRules(newEventRules); 
        if(Sessions.getCurrent().getAttribute("QueryType")!=null){
        	Map<String, Object> Instances = new HashMap<String, Object>();
        	if(alertRules.get(0).getTargetInstances()!=null){
        		String targetInstance = alertRules.get(0).getTargetInstances();
        		if(!targetInstance.equalsIgnoreCase("<ALL>")){
        			rs.setSqlServer(true);
        			rs.setSQLServer(true);
        		}
        		else{
        			rs.setSqlServer(false);
        			rs.setSQLServer(false);
        		}
        		Instances.put(targetInstance, targetInstance);
        		rs.setTargetInstances(Instances);
        	}
        	for (int i = 0; i < conditionEvents.size(); i++) {    		   
 			   //long conditionId = conditionEvents.get(i).getConditionId();
 			   long fieldId = conditionEvents.get(i).getFieldId();
 			   String matchString=conditionEvents.get(i).getMatchString();
 			   if(fieldId==5){
	        	rs.setDbFieldId(5);
 				rs.setDbMatchString(matchString);
 			   }
 			   else if(fieldId==6){
	        	rs.setObjectMatchString(matchString);
	        	rs.setObjectFieldId(6);
 			   }
 			   else if(fieldId==10){
	        	rs.setHostMatchString(matchString);
	        	rs.setHostFieldId(10);
 			   }
 			   else if(fieldId==2){
	        	rs.setAppMatchString(matchString);
	        	rs.setAppFieldId(2);
 			   }
 			   else if(fieldId==3){
	        	rs.setLoginMatchString(matchString);
	        	rs.setLoginFieldId(3);
 			   }
 			   else if(fieldId==0 && matchString.contains("include(1)0value")){
	        	rs.setExcludeCertainMatchString(matchString);
	        	rs.setExcludeCertainFieldId(0);
 			   }
 			   else if(fieldId==4){
	        	rs.setAccessChkMatchString(matchString);
	        	rs.setAccessChkFieldId(4);
 			   }
 			   else if(fieldId==7){
	        	rs.setPrivilegedUserMatchString(matchString);
	        	rs.setPrivilegedUserFieldId(7);
 			   }
			   else if(fieldId==14){
				   
						String rowCountWizard = matchString;
						if(!(rowCountWizard.isEmpty()))
						{
							String rowCountFinalVal = rowCountWizard
									.substring(rowCountWizard.indexOf("rowcount"));
							int rowCountCharCount = Integer.parseInt(rowCountFinalVal
									.substring(rowCountFinalVal.indexOf("(") + 1,
											rowCountFinalVal.indexOf(")")));
							String rowCount = (rowCountFinalVal.substring(
									rowCountFinalVal.indexOf(")") + 1,
									rowCountFinalVal.indexOf(")") + rowCountCharCount
											+ 1));
							if (rowCount != null) {
								
								Sessions.getCurrent().setAttribute("rowCountDetails",
										rowCountWizard);
							}
						}
				   
				   rs.setRowCountFieldId(14);
				   rs.setRowCountWithTimeInterval(true);
 				   rs.setRowCountMatchString(matchString);
 			    }
 			   else if(fieldId==13){
 				   rs.setPrivilegedFieldId(13); 				   
 				   rs.setPrivilegedUserNameMatchString(matchString); 
 			    } 			   
        	}
        	
        	wizardSaveEntity.setRegulationSettings(rs);        	
        	SelectAlertActions selectAlertActions=new SelectAlertActions();        	
        	selectAlertActions.setSnmpTrap(alertRules.get(0).getSnmpTrap()==1);
        	selectAlertActions.setLogMessage(alertRules.get(0).getLogMessage()==1);
        	selectAlertActions.setEmailMessage(alertRules.get(0).getEmailMessage()==1);
        	String messageSring = alertRules.get(0).getMessage();
        	selectAlertActions.setMessage(messageSring.replaceAll("recepients", "recipients"));
        	selectAlertActions.setSnmpServerAddress(alertRules.get(0).getSnmpServerAddress()); 
        	selectAlertActions.setAddress(alertRules.get(0).getSnmpServerAddress() );
        	selectAlertActions.setPort(alertRules.get(0).getSnmpPort());
        	selectAlertActions.setCommunity(alertRules.get(0).getSnmpCommunity());
        	wizardSaveEntity.setSelectAlertActions(selectAlertActions); 
        	Map<Long,Boolean> ruleIdAndEnable = new HashMap<>();
        	ruleIdAndEnable.put(alertRules.get(0).getRuleId(), alertRules.get(0).isEnabled());
        	Sessions.getCurrent().setAttribute("alertRuleId",ruleIdAndEnable);
        }
    }
    
    @Override
    public void onFinish(AddAlertRulesSaveEntity wizardSaveEntity){
    	onBeforeNext(wizardSaveEntity); 
    	SummaryStepViewModel summaryStepViewModel=new SummaryStepViewModel();
    	summaryStepViewModel.onFinish(wizardSaveEntity);
    }
    
    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
    }
}