package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.KeyValueParser;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.RulesCoreConstants;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps.NewDataAlertRulesStepViewModel.RuleType;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;

public class SpecifyAlertMessageViewModel implements AddPrivilegedUsersViewModel.DialogListener {
	private String help;
	EventCondition eventCondition = new EventCondition();
    EventField eventtype = new EventField();
    RulesCoreConstants rulesCoreConstants = new RulesCoreConstants();
    RulesCoreConstants templateLists;
	public String[]  targetString = {};
    private List<CMAlertRules> alertRules;
	private List<CMAlertRulesCondition> conditionEvents;
	Template currentInterval;
	KeyValueParser keyValueParser;
	String alertMessageTitle;
    String alertMessageBody;
    String id = "objectbodyMatch";
	@Wire 
    private  Textbox objectNameMatch;
	
	@Wire
	private Textbox objectbodyMatch;
	
    java.util.Map<String, String> templateTagList = new HashMap<String, String>();

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) throws Exception {
    	help = "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
        Selectors.wireComponents(view, this, false);
        initText();
        initTemplateList();
        if(Sessions.getCurrent().getAttribute("set_messageTitle")!=null && Sessions.getCurrent().getAttribute("set_messageBody")!=null)
		{
        objectNameMatch.setValue((String)Sessions.getCurrent().getAttribute("set_messageTitle"));
        objectbodyMatch.setValue((String)Sessions.getCurrent().getAttribute("set_messageBody"));
        BindUtils.postNotifyChange(null, null, SpecifyAlertMessageViewModel.this, "*");
        }
        if(Sessions.getCurrent().getAttribute("QueryType")!=null){
        conditionEvents = (List<CMAlertRulesCondition>) Sessions.getCurrent().getAttribute("conditionEvents");
        alertRules =(List<CMAlertRules>) Sessions.getCurrent().getAttribute("alertRules");
         	if(conditionEvents!=null && alertRules!=null);
         	{
         		initializer(alertRules);
         		BindUtils.postNotifyChange(null, null, SpecifyAlertMessageViewModel.this, "*");
         	}
        }	
    }
    
    public void initializer(List<CMAlertRules> alertRules) throws Exception{
    	if(alertRules.get(0).getMessage()!=null)
    	{
    		Map<String, String> parsedMessageData = new HashMap<String, String>();
    		keyValueParser = new KeyValueParser();
    		String messageData = alertRules.get(0).getMessage();
    		parsedMessageData =  keyValueParser.ParseString(messageData);
    		for (Map.Entry<String, String> entry : parsedMessageData
    				.entrySet()) {
    			if (entry.getKey().equals("title")) {
    				objectNameMatch.setValue(entry.getValue());
    			}
    			if (entry.getKey().equals("body")) {
    				objectbodyMatch.setValue(entry.getValue());
    			}
        	 }
    	 }
      }

	public SpecifyAlertMessageViewModel(){
     
     }
   
	public String getHelp() {
		return this.help;
	}
    
	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@Override
	public void onOk(long instanceId,List<CMInstancePermissionBase> selectedPermissionList) {
		
	}

	@Override
	public void onCancel(long instanceId) {
		Sessions.getCurrent().removeAttribute("conditionEvents");
	    Sessions.getCurrent().removeAttribute("alertRules");
	}

	public static enum Category {
		LISTED(1, ELFunctions.getLabel(SQLCMI18NStrings.LISTED)),//TODO AS ask .NET team id
		EXCEPT_LISTED(2, ELFunctions.getLabel(SQLCMI18NStrings.EXCEPT_LISTED));
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
	
	 public void initText(){
		    alertMessageTitle = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_DATA_ALERT_RULES_TITLE);
	        alertMessageBody = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_DATA_ALERT_RULES_MSG);
	        objectNameMatch.setValue(alertMessageTitle);
	        objectbodyMatch.setValue(alertMessageBody);
	 }
	 
	 public void initTemplateList(){
		 templateLists =new RulesCoreConstants();
		 templateTagList = templateLists.TemplateList();
		 //for (Entry<String, String> entry : templateTagList.entrySet()) {
		 int indexForDataAlertRule = 1;
		 if(Sessions.getCurrent().getAttribute("selectedDataAlertType") != null)
		 {
			 indexForDataAlertRule = (int)Sessions.getCurrent().getAttribute("selectedDataAlertType");
		 }
		 		Template template;
		 		if(indexForDataAlertRule != 1){
		            template = new Template("After Data Value");
					templateList.add(template);
	 			}
		 		template = new Template("Alert Level");
				templateList.add(template);
				template = new Template("Alert Time");
				templateList.add(template);
				template = new Template("Alert Type");
				templateList.add(template);
				if(indexForDataAlertRule != 1){
					template = new Template("Before Data Value");
					templateList.add(template);
				}
				template = new Template("Column Name");
				templateList.add(template);
				template = new Template("Database Name");
				templateList.add(template);
				template = new Template("Event Id");
				templateList.add(template);
				template = new Template("Event Time");
				templateList.add(template);
				template = new Template("Event Type");
				templateList.add(template);
				template = new Template("Host Name");
				templateList.add(template);
				template = new Template("Login");
				templateList.add(template);
				template = new Template("Server Name");
				templateList.add(template);
				template = new Template("Table Name");
				templateList.add(template);
		 //}
		 BindUtils.postNotifyChange(null, null, this, "templateList");
	 }
	  
	public java.util.Map<String, String> getTemplateTagList() {
		return templateTagList;
	}

	public void setTemplateTagList(java.util.Map<String, String> templateTagList) {
		this.templateTagList = templateTagList;
	}	
	
	@Command("submitChoice")
	public void submitChoice(@BindingParam("comp") Window x) throws Exception{
		eventCondition.set_messageTitle(objectNameMatch.getValue());
		eventCondition.set_messageBody(objectbodyMatch.getValue());
		String matchString =  eventCondition.UpdateMessageData(eventtype,eventCondition);
		Sessions.getCurrent().setAttribute("specifyAlertMessage",eventCondition);
		Sessions.getCurrent().setAttribute("set_messageTitle", objectNameMatch.getValue());
		Sessions.getCurrent().setAttribute("set_messageBody", objectbodyMatch.getValue());
		x.detach();
	}
    
	public String[] GetTargetString()
	{   
		targetString = new String[templateList.getSize()];
		for (int j = 0; j < templateList.getSize(); j++) {
	    targetString[j] = templateList.get(j).getTemplateName().toString();
	    }
		return targetString;
     }

    private ListModelList<Template> templateList = new ListModelList<>();
    
    public class Template {
    	String templateName;

    	public String getTemplateName() {
    		return templateName;
    	}
    	public void setTemplateName(String templateName) {
    		this.templateName = templateName;
    	}
    	public Template(String templateName) {
    		super();
    		this.templateName = templateName;
    	}
    }
	@Command("getLastFocuesdAttr")
	public void getLastFocuesdAttr(@BindingParam("id") Textbox lastFocused){
	   id = lastFocused.getId();
	}
	
	 @Command("onItemClick")
	 public void onItemClick() {
	        enableRemoveButtonIfSelected();
	 }

	private void enableRemoveButtonIfSelected() 
	{
	    Set<Template> selectedItems = templateList.getSelection();
	    if (selectedItems != null && !selectedItems.isEmpty()) {
			  for (Template i : selectedItems) {
				  currentInterval = i;
				  String strAlertMessage = currentInterval.templateName;
				  java.util.Map<String, String> tempMap = rulesCoreConstants.TemplateList();
				  for (Entry<String, String> entry : tempMap.entrySet()) {

						if (entry.getKey().equals(strAlertMessage)) {
							if(id.equals("objectNameMatch"))
							  {
								objectNameMatch.setValue(objectNameMatch.getValue() + entry.getValue());
							  }
							else
							{
								objectbodyMatch.setValue(objectbodyMatch.getValue() + entry.getValue());
							}
						}
				  }	
				  break;
			  }
		  }
    }
	
    String eventAddress;
	
	@Command
	@NotifyChange("templateList")
	public void addItem()
	{
		String eventDatabaseName = this.eventAddress;
		if(eventDatabaseName!=null && (!eventDatabaseName.isEmpty())){
			boolean chkPass = true;
			for (int j = 0; j < templateList.getSize(); j++) {
				if (templateList.get(j).getTemplateName().toString().equals(eventDatabaseName)){
					WebUtil.showInfoBoxWithCustomMessage("The list already contains " + eventDatabaseName);
					chkPass = false;
					break;
				}	
			}
			if (chkPass){
			Template data = new Template(eventDatabaseName);
			templateList.add(data);
			}
		}
	}
	

    @Command("onRemoveBtnClick")
    public void onRemoveBtnClick() {
        Utils.removeAllSelectedItems(templateList);
        enableRemoveButtonIfSelected();
        BindUtils.postNotifyChange(null, null, this, "templateList");
    }

	public ListModelList<Template> getTemplateList() {
		return templateList;
	}

	public void setTemplateList(ListModelList<Template> templateList) {
		this.templateList = templateList;
	}   
}
