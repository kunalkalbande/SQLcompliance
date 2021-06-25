package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.beEntities.AuditedInstanceBE;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.RulesCoreConstants;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.AddDataAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.NewDataAlertRules;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.RegulationSettings;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.SelectDataAlertActions;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.AddStatusAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps.NewStatusAlertRuleStepViewModel;
import com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard.steps.NewStatusAlertRuleStepViewModel.Category;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Textbox;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

public class NewDataAlertRulesStepViewModel extends AddWizardStepBase {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/adddataalertruleswizard/steps/new-data-alert-rules-step.zul";
	private ListModelList<CMAlertRules> alertRulesList;
	protected AlertsRuleSource alertsRuleSource;
	private ListModelList<RuleType> intervalListModelList;
	private int _defFieldId = 1;
	protected RuleType ruleType;
	int alertType = 3;
	CMAlertRules cmAlertRule = new CMAlertRules();
	NewDataAlertRules newDataRules;
	private RuleType currentInterval = RuleType.SENSITIVE_COLUMN;
	String regulationGuidelinesDesc = ELFunctions.getLabel(SQLCMI18NStrings.SENSITIVE_COLUMN_ACCESSED_DESC);
	private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCondition> conditionEvents;
	 RulesCoreConstants ruleCoreConstants = new RulesCoreConstants();
	@Wire 
	private Textbox eventDataName;
	
	@Wire
	private Textbox eventDataDescription;
	
	@Wire 
	private Combobox dataAlertLevel;
	
	protected List<AuditedInstanceBE> entitiesList;
	
	private InstancesFacade instancesFacade = new InstancesFacade();
	
	protected Map<String, Object> filterRequest = new TreeMap<>();
	
	long dataAlertRuleFieldId;
	
	@Wire 
	private Combobox cmDataRuleType;
    private ListModelList<CMDatabase> databaseList;

	    @Override
	    public void onDoAfterWire() {
	    }
	   
	public static enum AlertsRuleSource {
		LOW(ELFunctions.getLabel(SQLCMI18NStrings.LOW_ALERT_RULES), 1), MEDIUM(
				ELFunctions.getLabel(SQLCMI18NStrings.MEDIUM_ALERT_RULES), 2), HIGH(
				ELFunctions.getLabel(SQLCMI18NStrings.HIGH_ALERT_RULES), 3), SEVERE(
				ELFunctions.getLabel(SQLCMI18NStrings.SEVERE_ALERT_RULES), 4);

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

	public static enum RuleType {
		SENSITIVE_COLUMN(0,ELFunctions.getLabel(SQLCMI18NStrings.SENSITIVE_COLUMN_ACCESSED), 1),
		NUMRIC_COLUMN_VALUE(1,ELFunctions.getLabel(SQLCMI18NStrings.NUMRIC_COLUMN_VALUE_CHANGED), 2),
		COLUMN_VALUE(2,ELFunctions.getLabel(SQLCMI18NStrings.COLUMN_VALUE_CHANGED), 3);

		private String label;

		private int id;
		private int index;

		RuleType(int index, String label, int id) {
			this.index = index;
			this.label = label;
			this.id = id;
		}
		public int getIndex() {
			return index;
		}
		
		public int getId() {
			return id;
		}
	}

	private void initIntervalList() {
		intervalListModelList = new ListModelList<>();
		intervalListModelList.add(RuleType.SENSITIVE_COLUMN);
		intervalListModelList.add(RuleType.COLUMN_VALUE);
		intervalListModelList.setSelection(Arrays.asList(currentInterval));
	}

	public NewDataAlertRulesStepViewModel() {
		super();
		newDataRules = new NewDataAlertRules();
		newDataRules.setAlertType(alertType);
		setRegulationGuidelinesDesc(regulationGuidelinesDesc);
	}

	@Override
	public boolean isFirst() {
		return true;
	}

	@Override
	public String getNextStepZul() {
		return SelectDataFilterStepViewModel.ZUL_PATH;
	}

	@Override
	public boolean isValid() {
		return true;
	}

	@Override
	public String getTips() {
		return ELFunctions.getLabel(SQLCMI18NStrings.SQL_DATA_ALERT_TYPE_TIPS);
	}

	@Command("selectEventSource")
	public void selectEventSource(@BindingParam("id") String id) {
		alertsRuleSource = AlertsRuleSource.valueOf(id);
		newDataRules.setAlertLevel(alertsRuleSource.id);
	}
	
	@Override
	public void onCancel(AddDataAlertRulesSaveEntity wizardSaveEntity) {
    	if(Sessions.getCurrent().getAttribute("QueryType")!=null)
    	{
    		Sessions.getCurrent().removeAttribute("QueryType");
    		Sessions.getCurrent().removeAttribute("QueryTypeForColumn");
    	}
    	 String uri = "instancesAlertsRule";
         uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
         Executions.sendRedirect(uri);
	}
	
	 @Override
	    public void doOnShow(AddDataAlertRulesSaveEntity wizardSaveEntity) {
	    	 if(Sessions.getCurrent().getAttribute("QueryType")!=null){
	     		conditionEvents = (List<CMAlertRulesCondition>) Sessions.getCurrent().getAttribute("conditionEvents");
	     		alertRules =(List<CMAlertRules>) Sessions.getCurrent().getAttribute("alertRules");
	     		dataAlertRuleFieldId=conditionEvents.get(0).getFieldId();
	    		if(conditionEvents!=null && alertRules!=null);
	    		{
	    			initializer(alertRules);
	    			BindUtils.postNotifyChange(null, null, NewDataAlertRulesStepViewModel.this, "*");
	    		}
	    	 }
	   }
	 
	 public void initializer(List<CMAlertRules> alertRules){
		   int id; 
		    eventDataName.setValue(alertRules.get(0).getNames());
		    eventDataDescription.setValue(alertRules.get(0).getDescription());
		    dataAlertLevel.setSelectedIndex(alertRules.get(0).getAlertLevel()-1);
	    	for (Map.Entry<String, String> entry : ruleCoreConstants.DataAlertConditionMap()
					.entrySet()) {
				if (entry.getKey().equals(Long.toString(conditionEvents.get(0).getFieldId()))) {
					RuleType[] ruleType1 = RuleType.values();
					String temp = ruleType1[Integer.parseInt(entry.getKey())-1].name();
					regulationGuidelinesDesc = "";
					ruleType = RuleType.valueOf(temp);
					if (ruleType.id == 1) {
						regulationGuidelinesDesc = ELFunctions.getLabel(SQLCMI18NStrings.SENSITIVE_COLUMN_ACCESSED_DESC);
						_defFieldId = 1;
					} else {
						regulationGuidelinesDesc = ELFunctions.getLabel(SQLCMI18NStrings.COLUMN_VALUE_CHANGED_DESC);
						_defFieldId = 3;
					}
					BindUtils.postNotifyChange(null, null,NewDataAlertRulesStepViewModel.this, "regulationGuidelinesDesc");
					if(conditionEvents.get(0).getFieldId()==1){
						id = 0;
					}
					else{
						id=1;
					}
					cmDataRuleType.setSelectedIndex(id);
				}
	    	 }
	    	Sessions.getCurrent().setAttribute("SQL server",alertRules.get(0).getTargetInstances());
	    	
	    	entitiesList = instancesFacade.getAllEntitiesInstances(filterRequest);
			if(Sessions.getCurrent().getAttribute("SQL server")!= null){
			String instanceName = (String)Sessions.getCurrent().getAttribute("SQL server");
			for (AuditedInstanceBE iterable : entitiesList) {
					if(instanceName.equals(iterable.getInstance().toString())){
						Sessions.getCurrent().setAttribute("instanceId",(int)iterable.getId());
				 }
			   }
			}
	    	
	      }
	@Command("selectRuleTypeDescription")
	public void selectRuleTypeDescription(@BindingParam("id") String id) {
		Sessions.getCurrent().removeAttribute("QueryTypeForColumn");
		regulationGuidelinesDesc = "";
		ruleType = RuleType.valueOf(id);
		if (ruleType.id == 1) {
			regulationGuidelinesDesc = ELFunctions.getLabel(SQLCMI18NStrings.SENSITIVE_COLUMN_ACCESSED_DESC);
			Sessions.getCurrent().setAttribute("RuleTypeAccess","Sensitive column");
			_defFieldId = 1;
		} else {
			regulationGuidelinesDesc = ELFunctions.getLabel(SQLCMI18NStrings.COLUMN_VALUE_CHANGED_DESC);
			Sessions.getCurrent().setAttribute("RuleTypeAccess","Column value");
			_defFieldId = 3;
		}
		
		if(Sessions.getCurrent().getAttribute("QueryType")!=null){
			conditionEvents.get(0).setFieldId(_defFieldId);
     		Sessions.getCurrent().setAttribute("conditionEvents",conditionEvents);
		}
		
		BindUtils.postNotifyChange(null, null,NewDataAlertRulesStepViewModel.this, "regulationGuidelinesDesc");
	}

	public String getRegulationGuidelinesDesc() {
		return regulationGuidelinesDesc;
	}

	public void setRegulationGuidelinesDesc(String regulationGuidelinesDesc) {
		this.regulationGuidelinesDesc = regulationGuidelinesDesc;
	}

	@Override
	public void onBeforeNext(AddDataAlertRulesSaveEntity wizardSaveEntity) {
		newDataRules.setName(eventDataName.getValue());
		newDataRules.setFieldId(_defFieldId);
		Sessions.getCurrent().setAttribute("FieldId", _defFieldId);
		newDataRules.setDescription(eventDataDescription.getValue());
		newDataRules.setAlertLevel(dataAlertLevel.getSelectedIndex()+1);
		wizardSaveEntity.setNewEventAlertRules(newDataRules);
		Sessions.getCurrent().setAttribute("selectedDataAlertType",cmDataRuleType.getSelectedIndex()+1);
	}

	@Override
	public String getHelpUrl() {
		return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
	}

}