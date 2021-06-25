package com.idera.sqlcm.ui.instancesAlertsRule;


import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;

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

public class SpecifyAlertMessageViewModel implements AddPrivilegedUsersViewModel.DialogListener {
	private String help;
	EventCondition eventCondition = new EventCondition();
	RulesCoreConstants rulesCoreConstants =new RulesCoreConstants();
    EventField eventtype = new EventField();
    RulesCoreConstants templateLists;
	public String[]  targetString = {};
	KeyValueParser keyValueParser;
	@Wire 
    private  Textbox objectNameMatch;
	
	@Wire
	private Textbox objectbodyMatch;
	
	String id = "objectbodyMatch";
	String alertMessageTitle;
    String alertMessageBody;
    java.util.Map<String, String> templateTagList = new HashMap<String, String>();

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
    	help = "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
        Selectors.wireComponents(view, this, false);
        initText();
        initTemplateList();
        if(Sessions.getCurrent().getAttribute("set_messageTitle")!=null && Sessions.getCurrent().getAttribute("set_messageBody")!=null)
		{
        objectNameMatch.setValue((String)Sessions.getCurrent().getAttribute("set_messageTitle"));
        objectbodyMatch.setValue((String)Sessions.getCurrent().getAttribute("set_messageBody"));
        }
        if(Sessions.getCurrent().getAttribute("QueryType")!=null){
        	List<CMAlertRulesCondition> conditionEvents = (List<CMAlertRulesCondition>) Sessions.getCurrent().getAttribute("conditionEvents");
            List<CMAlertRules>  alertRules =(List<CMAlertRules>) Sessions.getCurrent().getAttribute("alertRules");
             	if(conditionEvents!=null && alertRules!=null);
             	{
             		try{
             			initializer(alertRules);
             		}
             		catch(Exception e){}
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
    
    
     public String getAlertMessageTitle() {
		return alertMessageTitle;
	}

	public void setAlertMessageTitle(String alertMessageTitle) {
		this.alertMessageTitle = alertMessageTitle;
	}

	public String getAlertMessageBody() {
		return alertMessageBody;
	}

	public void setAlertMessageBody(String alertMessageBody) {
		this.alertMessageBody = alertMessageBody;
	}

	public SpecifyAlertMessageViewModel(){
     
     }
   
	public String getHelp() {
		return this.help;
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
	
	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@Override
	public void onOk(long instanceId,List<CMInstancePermissionBase> selectedPermissionList) {
		
	}

	@Override
	public void onCancel(long instanceId) {
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
	  private Template currentInterval;
	  private ListModelList<Category> intervalListModelList;
	  	
	 public void initText(){
		    alertMessageTitle = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_EVENT_ALERT_RULES_TITLE);
	        alertMessageBody = ELFunctions.getLabel(SQLCMI18NStrings.DEFAULT_EVENT_ALERT_RULES_MSG);
	        objectNameMatch.setValue(alertMessageTitle);
	        objectbodyMatch.setValue(alertMessageBody);
	   	 }
	 
	 public void initTemplateList(){
		 templateLists =new RulesCoreConstants();
		 templateTagList = templateLists.TemplateList();
		 for (Entry<String, String> entry : templateTagList.entrySet()) {
			 String keyValue=entry.getKey();
			 if(!keyValue.equalsIgnoreCase("Actual Value") 
					 && !keyValue.equalsIgnoreCase("Alert Type Name") 
					 && !keyValue.equalsIgnoreCase("Computer Name")
					 && !keyValue.equalsIgnoreCase("Threshold Value") 
					 && !keyValue.equalsIgnoreCase("Column Name") 
					 && !keyValue.equalsIgnoreCase("Table Name")){
				Template template = new Template(entry.getKey());
				templateList.add(template);
			 }		
		}
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
		if(Sessions.getCurrent().getAttribute("specifyAlertMessage")!=null)
		{
			eventCondition = (EventCondition) Sessions.getCurrent().getAttribute("specifyAlertMessage");
		}
		eventCondition.set_messageTitle(objectNameMatch.getValue());
		eventCondition.set_messageBody(objectbodyMatch.getValue());
		Sessions.getCurrent().setAttribute("set_messageTitle", objectNameMatch.getValue());
		Sessions.getCurrent().setAttribute("set_messageBody", objectbodyMatch.getValue());
		Sessions.getCurrent().setAttribute("specifyAlertMessage",eventCondition);
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
