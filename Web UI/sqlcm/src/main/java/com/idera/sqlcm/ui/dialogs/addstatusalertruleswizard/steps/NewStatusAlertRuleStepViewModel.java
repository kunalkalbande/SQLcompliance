package com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.RulesCoreConstants;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.SelectAlertActions;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps.StatusAlertActionStepViewModel;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.NewStatusAlertRulesData;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Textbox;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class NewStatusAlertRuleStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addstatusalertruleswizard/steps/new-status-alert-rules-step.zul";
    protected AlertsRuleSource alertsRuleSource;
    private ListModelList<Category> intervalListModelList;
    protected Category category;
    NewStatusAlertRulesData newStatusRules;
    private Category currentInterval = Category.TRACE_DIR_FULL_AGENT;
    String regulationGuidelinesDesc= ELFunctions.getLabel(SQLCMI18NStrings.TRACE_DIR_FULL_AGENT_DESC);
    String specifyValueLabel = ELFunctions.getLabel(SQLCMI18NStrings.MAX_PERCENTAGE_TARCE_USED);
    
    @Wire
    Spinner spStatusAlertRules;
    
    @Wire
	private Textbox eventAlertName;
    
	private String eventAlertDescription;
    private String _defMatchString = "50";
    private int _defAlertLevel = 2;
    private int _defFieldId = 1;
    private int _ruleType = 2;
    String _targetInstance= "<ALL>";
    RulesCoreConstants ruleCoreConstants = new RulesCoreConstants();
    private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCondition> conditionEvents;
	
	@Wire
	private Combobox cmAlertLevel;
	
	@Wire
	private Combobox cmStatusRuleType;
	
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
    	TRACE_DIR_FULL_AGENT(1, ELFunctions.getLabel(SQLCMI18NStrings.TRACE_DIR_FULL_AGENT),1),//TODO AS ask .NET team id
    	TRACE_DIR_FULL_COLLECT(2, ELFunctions.getLabel(SQLCMI18NStrings.TRACE_DIR_FULL_COLLECT),2),
    	NO_HEARTBEAT(3, ELFunctions.getLabel(SQLCMI18NStrings.NO_HEARTBEAT),3),
    	REPOSITORY_TOO_BIG(4, ELFunctions.getLabel(SQLCMI18NStrings.REPOSITORY_TOO_BIG),4),
    	SQL_SERVER_DOWN(5, ELFunctions.getLabel(SQLCMI18NStrings.SQL_SERVER_DOWN),5);

        private String label;
        private int id;
        private int fieldId;
        private Category(int id, String label,int fieldId) {
            this.label = label;
            this.id = id;
            this.fieldId = fieldId;
        }

        public int getFieldId() {
			return fieldId;
		}

		public String getLabel() {
            return label;
        }

        public String getName() {
            return this.name();
        }

        public int getId() {
            return id;
        }
    }
    
    private void initIntervalList() {
        intervalListModelList = new ListModelList<>();
        intervalListModelList.add(Category.TRACE_DIR_FULL_AGENT);
        intervalListModelList.add(Category.TRACE_DIR_FULL_COLLECT);
        intervalListModelList.add(Category.NO_HEARTBEAT);
        intervalListModelList.add(Category.REPOSITORY_TOO_BIG);
        intervalListModelList.add(Category.SQL_SERVER_DOWN);
        intervalListModelList.setSelection(Arrays.asList(currentInterval));
    }
        
    public NewStatusAlertRuleStepViewModel() {
        super();
        newStatusRules = new NewStatusAlertRulesData();
        setRegulationGuidelinesDesc(regulationGuidelinesDesc);
        newStatusRules.setMatchString(_defMatchString);
        newStatusRules.setAlertLevel(_defAlertLevel);
        newStatusRules.setFieldId(_defFieldId);
        newStatusRules.setAlertType(_ruleType);
        newStatusRules.setTargetInstances(_targetInstance);
    }
    
    @Override
	public void onCancel(AddStatusAlertRulesSaveEntity wizardSaveEntity) {
    	if(Sessions.getCurrent().getAttribute("QueryType")!=null)
    	{
    		Sessions.getCurrent().removeAttribute("QueryType");
    	}
    	 String uri = "instancesAlertsRule";
         uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
         Executions.sendRedirect(uri);
	}

    @Override
    public void doOnShow(AddStatusAlertRulesSaveEntity wizardSaveEntity) {
    	 if(Sessions.getCurrent().getAttribute("QueryType")!=null){
     		conditionEvents = (List<CMAlertRulesCondition>) Sessions.getCurrent().getAttribute("conditionEvents");
     		alertRules =(List<CMAlertRules>) Sessions.getCurrent().getAttribute("alertRules");
    		if(conditionEvents!=null && alertRules!=null);
    		{
    			initializer(alertRules);
    			BindUtils.postNotifyChange(null, null, NewStatusAlertRuleStepViewModel.this, "*");
    		}
    	 }
    }

    public void initializer(List<CMAlertRules> alertRules){
    	
    	eventAlertName.setValue(alertRules.get(0).getNames());
    	
    	setEventAlertDescription(alertRules.get(0).getDescription());
    	
    	for (Map.Entry<String, String> entry : ruleCoreConstants.RulesLevel()
				.entrySet()) {
			if (entry.getKey().equals(Integer.toString(alertRules.get(0).getAlertLevel()))) {
				setSpecifyValueLabel(entry.getValue());
			}
			
    	 }
    	for (Map.Entry<String, String> entry : ruleCoreConstants.StatusAlertConditionMap()
				.entrySet()) {
			if (entry.getKey().equals(Long.toString(conditionEvents.get(0).getFieldId()))) {
				
				Category[] category = Category.values();
				
				spStatusAlertRules.setVisible(false);
				
				selectRuleTypeDescription(category[Integer.parseInt(entry.getKey())-1].name(),spStatusAlertRules);
				
				cmAlertLevel.setSelectedIndex(alertRules.get(0).getAlertLevel()-1);
				
				newStatusRules.setAlertLevel(alertRules.get(0).getAlertLevel());
				
				cmStatusRuleType.setSelectedIndex((int) (long)(conditionEvents.get(0).getFieldId())-1);
			
				if(spStatusAlertRules.isVisible()){
				set_defMatchString(conditionEvents.get(0).getMatchString());
				
				}
			}
    	 }
    	
      }
    
    @Override
    public boolean isFirst() {
        return true;
    }

    @Override
    public String getNextStepZul() {
        return StatusAlertActionStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.SQL_STATUS_ALERT_TYPE_TIPS);
    }

    @Command("selectEventSource")
    public void selectEventSource(@BindingParam("id") String id) {
        alertsRuleSource = AlertsRuleSource.valueOf(id);
        newStatusRules.setAlertLevel(alertsRuleSource.id);
        newStatusRules.setAlertType(2);
    }
    
    @Command("selectAddEventFilter")
    public void selectAddEventFilter(@BindingParam("radioGroup") Radiogroup radioGroup) throws RestException {
    	getNextButton().setDisabled(false);
    	initIntervalList();
    	Set<Category> selectedIntervals = intervalListModelList.getSelection(); // must contain only 1 item because single selection mode.
        if (selectedIntervals != null && !selectedIntervals.isEmpty()) {
            for (Category i : selectedIntervals) {
                currentInterval = i;
                break;
            }
        }
    }
    
    @Command("selectRuleTypeDescription")
	public void selectRuleTypeDescription(@BindingParam("id") String id , @BindingParam("spinner") Spinner spStatusAlertRules) {
		regulationGuidelinesDesc = ""; 
		category = Category.valueOf(id);
		newStatusRules.setFieldId(category.fieldId);
		specifyValueLabel = "";
		switch(category.id)
       {
		case 1:
			String _defTraceDir= "50";
			regulationGuidelinesDesc = ELFunctions.getLabel(SQLCMI18NStrings.TRACE_DIR_FULL_AGENT_DESC);
			specifyValueLabel = ELFunctions.getLabel(SQLCMI18NStrings.MAX_PERCENTAGE_TARCE_USED);
			spStatusAlertRules.setVisible(true);
			spStatusAlertRules.setText(_defTraceDir);
			set_defMatchString(_defTraceDir);
			break;
		case 2:
			String _defTracefile = "10";
			regulationGuidelinesDesc = ELFunctions.getLabel(SQLCMI18NStrings.TRACE_DIR_FULL_COLLECT_DESC);
			specifyValueLabel = ELFunctions.getLabel(SQLCMI18NStrings.TARCE_DIRECTORY_SIZE);
			spStatusAlertRules.setVisible(true);
			spStatusAlertRules.setText(_defTracefile);
			set_defMatchString(_defTracefile);
			break;
		case 3:
			String _defHeartBeat = "0";
			regulationGuidelinesDesc = ELFunctions.getLabel(SQLCMI18NStrings.NO_HEARTBEAT_DESC);
			spStatusAlertRules.setVisible(false);
			set_defMatchString(_defHeartBeat);
			break;
		case 4:
			String _defDatabaseSize= "20";
			regulationGuidelinesDesc = ELFunctions.getLabel(SQLCMI18NStrings.REPOSITORY_TOO_BIG_DESC);
			specifyValueLabel = ELFunctions.getLabel(SQLCMI18NStrings.DATABASE_SIZE);
			spStatusAlertRules.setVisible(true);
			spStatusAlertRules.setText(_defDatabaseSize);
			set_defMatchString(_defDatabaseSize);
			break;
		case 5:
			String _defSQLServerDown = "0";
			regulationGuidelinesDesc = ELFunctions.getLabel(SQLCMI18NStrings.SQL_SERVER_DOWN_DESC);
			spStatusAlertRules.setVisible(false);
			set_defMatchString(_defSQLServerDown);
		    break;	
		}
		
		BindUtils.postNotifyChange(null, null,NewStatusAlertRuleStepViewModel.this, "regulationGuidelinesDesc");
		BindUtils.postNotifyChange(null, null,NewStatusAlertRuleStepViewModel.this, "specifyValueLabel");
		
	}
    public String get_defMatchString() {
		return _defMatchString;
	}

	public void set_defMatchString(String _defMatchString) {
		this._defMatchString = _defMatchString;
	}    
    
	public String getEventAlertDescription() {
		return eventAlertDescription;
	}

	public void setEventAlertDescription(String eventAlertDescription) {
		this.eventAlertDescription = eventAlertDescription;
	}
    
	public Spinner getSpStatusAlertRules() {
		return spStatusAlertRules;
	}

	public void setSpStatusAlertRules(Spinner spStatusAlertRules) {
		this.spStatusAlertRules = spStatusAlertRules;
	}

	public String getRegulationGuidelinesDesc() {
		return regulationGuidelinesDesc;
	}
	
	public String getSpecifyValueLabel() {
		return specifyValueLabel;
	}

	public void setSpecifyValueLabel(String specifyValueLabel) {
		this.specifyValueLabel = specifyValueLabel;
	}

	public void setRegulationGuidelinesDesc(String regulationGuidelinesDesc) {
		this.regulationGuidelinesDesc = regulationGuidelinesDesc;
	}
    
    @Override
    public void onBeforeNext(AddStatusAlertRulesSaveEntity wizardSaveEntity) {
    	newStatusRules.setName(eventAlertName.getValue());
    	newStatusRules.setDescription(this.eventAlertDescription);
    	newStatusRules.setMatchString(_defMatchString);
        wizardSaveEntity.setNewStatusAlertRulesData(newStatusRules);
    }
    
    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
    }
    
    /*SCM-498*/
    @Command("resetValue")
    public void resetValue()
    {
    	if(spStatusAlertRules.getValue()==null)
        {
        	Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
        			spStatusAlertRules, "end_center", 3000);
        	_defMatchString="1";
            
        }
    	if (spStatusAlertRules.getValue()<=0)
    	{
    		_defMatchString="1";
    		
    		 Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
    				 spStatusAlertRules, "end_center", 3000);
    	}
    	
    	if (spStatusAlertRules.getValue()>100)
    	{
    		_defMatchString="100";
    		Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
   				 spStatusAlertRules, "end_center", 3000);
    	}
    	BindUtils.postNotifyChange(null, null, NewStatusAlertRuleStepViewModel.this, "*");
    }
    
   
    
}