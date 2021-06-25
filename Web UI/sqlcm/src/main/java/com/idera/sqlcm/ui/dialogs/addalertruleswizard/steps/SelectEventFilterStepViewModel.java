package com.idera.sqlcm.ui.dialogs.addalertruleswizard.steps;

import java.util.Collection;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.ListIterator;
import java.util.Map;
import java.util.Set;

import com.idera.ccl.IderaDropdownList;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.CategoryData;
import com.idera.sqlcm.entities.CategoryRequest;
import com.idera.sqlcm.entities.CategoryResponse;
import com.idera.sqlcm.entities.RegulationType;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.KeyValueParser;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.RegulationSettings;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.EventType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyAppNameViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyAppNameViewModel.App;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyHostNameViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyHostNameViewModel.Host;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyLoginViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyObjectsViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyObjectsViewModel.Objects;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyDatabaseViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifySQLServerViewModel; 
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyDatabaseViewModel.Data;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyLoginViewModel.Login;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyPrivilegedUserViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyPrivilegedUserViewModel.PrivilegedUserName;


import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.A;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Window;


public class SelectEventFilterStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addalertruleswizard/steps/select-event-filter-level-step.zul";

    @Wire
    private Radiogroup rgFlow;

    private String regulationGuidelinesDesc;
    
    @Wire
    private A specifySQL;
    
    @Wire
    private A specifyDatabase;
    
    @Wire
    private A specifiedObjects;
    
    @Wire
    private A specifiedWords_db;
    
    @Wire
    private A specifiedWords;
    
    @Wire
    private A specifiedWordsName;
    
    @Wire
    private A selectEventType;
    
    @Wire
    private A excludeCertainEventType;
    
    @Wire
    private A specifiedPrivilegedUser;
    
    
	@Wire
    private A rowCounts;
    
   
	@Wire
    private Listbox SelectEventTypeMain;
    
    @Wire
    private Listbox SelectEventTypeSub;
    
    @Wire
	private Window specifyDatabaseWindow; 
    
    List<CategoryData> entitiesList;
    
    public List<CategoryData> getEntitiesList() {
		return entitiesList;
	}

	public void setEntitiesList(List<CategoryData> entitiesList) {
		this.entitiesList = entitiesList;
	}

	
	
	public static boolean sqlServerChecked=false;
	public static final String SQL_Server = "SQL Server";
    public static final String Database_Name = "Database Name";
    public static final String Object_Name = "Object Name";
    public static final String Host_Name = "Host Name";
    public static final String Application_Name = "Application Name";
    public static final String Login_Name = "Login Name";
    public static final String Access_Check_Passed = "Access Check Passed";
    public static final String Is_Privileged_User = "Is Privileged User"; 
    public static final String Exclude_certain_Event_Type = "Exclude Certain Event Type";
    public static boolean isPrivilegedUserNameCheckboxChecked = false;
    private ListModelList<RegulationType> regulationTypeListModelList;
    protected IsPrivilegedUser isPrivilegedUser;
    protected AccessUserCheck accessUserCheck;
    private Map<String,Long> checkedAlertRulesTypes = new HashMap<String,Long>();
    RegulationSettings rs = new RegulationSettings();
    private String isPrivilegedUserCheck;
    EventCondition eventCondition = new EventCondition();
    EventField eventtype = new EventField();
    private List<CMAlertRules> alertRules;
   	private List<CMAlertRulesCondition> conditionEvents;
   	int[] certainId = new int[1];
   	KeyValueParser keyValueParser;
   	String _targetInstance = "<All>";
   	SpecifyDatabaseViewModel specifyDatabaseVM;
   	@Wire 
   	private IderaDropdownList privilegedUser;
   	
   	@Wire
   	private Checkbox chkServer;
   	
   	@Wire
   	private Checkbox chkDatabase;
   	
   	@Wire
   	private Checkbox chkHost;
   	
   	@Wire
   	private Checkbox chkLogin;
   	
   	@Wire
   	private Checkbox chkApplication;
   	
   	@Wire
   	private Checkbox chkObject;
   	
   	@Wire
   	private Checkbox chkPrivilegedUser;
   	
   	@Wire 
   	private Checkbox chkAccessCheck;
   	
   	@Wire 
   	private IderaDropdownList accessCheck;
   	
   	@Wire
   	private Checkbox chkCertainEventType;
   	
   	@Wire
   	private Checkbox chkPrivilegedUserName;
   	
   	@Wire
   	private Checkbox chkRowCounts;
   	
   	public static enum IsPrivilegedUser
    {
        TRUE2(ELFunctions.getLabel(SQLCMI18NStrings.TRUE), 1),
        FALSE2(ELFunctions.getLabel(SQLCMI18NStrings.FALSE), 2);
     
        private String label;

        private int id;

        IsPrivilegedUser(String label, int id) {
            this.label = label;
            this.id = id;
       }

        public String getLabel() {
            return label;
        }
        public int getId() {
            return id;
        }
    }
  
    @Command("checkPrivilegedUser")
    public void checkPrivilegedUser(@BindingParam("id") String id) {
    	isPrivilegedUser = IsPrivilegedUser.valueOf(id);
        boolean bvar = Boolean.parseBoolean(isPrivilegedUser.label);
    	rs.setIsPrivilegedUser(bvar);
    	if(Sessions.getCurrent().getAttribute("PrivilegedMatchString")!=null){
    		if(bvar==true)
    			Sessions.getCurrent().setAttribute("PrivilegedMatchString","value(4)true");    		
    		else
    			Sessions.getCurrent().setAttribute("PrivilegedMatchString","value(5)false");
    	}
    }
    
    @Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}
    
    public static enum AccessUserCheck
    {
        TRUE(ELFunctions.getLabel(SQLCMI18NStrings.TRUE), 1),
        FALSE(ELFunctions.getLabel(SQLCMI18NStrings.FALSE), 2);
     
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
    	rs.setAccessCheckPassed(bvar);
    	if(Sessions.getCurrent().getAttribute("Access Check Passed")!=null){
    		if(bvar==true)
    			Sessions.getCurrent().setAttribute("Access Check Passed","value(4)true");    		
    		else
    			Sessions.getCurrent().setAttribute("Access Check Passed","value(5)false");
    	}
  }

    @Override
    protected void doOnShow(AddAlertRulesSaveEntity wizardSaveEntity){
    	String id="";
 		if(Sessions.getCurrent().getAttribute("eventTypeId")!=null)
				id=(String)Sessions.getCurrent().getAttribute("eventTypeId");
 		if(id.equals("101"))
 		{
 			chkCertainEventType.setChecked(false);
 			chkCertainEventType.setDisabled(true); 
 			rs.setExcludeCertainEventType(false);
 			excludeCertainEventType.setDisabled(true);
 		}
 		else{
 			chkCertainEventType.setDisabled(false);
 			excludeCertainEventType.setDisabled(true);
 			if(chkCertainEventType.isChecked()){
 				excludeCertainEventType.setDisabled(false); 				
 			}
 			else{
 				rs.setExcludeCertainEventType(false);
 			}	
 		
	 		if(Sessions.getCurrent().getAttribute("Type")!=null){
	 			chkCertainEventType.setChecked(true);
	    		excludeCertainEventType.setDisabled(false);
	    		long index = 9;
	    		rs.setExcludeCertainEventType(true);
	    		rs.setExcludeCertainMatchString((String)Sessions.getCurrent().getAttribute("Type"));
	    		rs.setExcludeCertainFieldId(0);
	    	}
 		}
    	if(Sessions.getCurrent().getAttribute("QueryType")!=null)
    	{
			conditionEvents = (List<CMAlertRulesCondition>) Sessions.getCurrent().getAttribute("conditionEvents");
			alertRules =(List<CMAlertRules>) Sessions.getCurrent().getAttribute("alertRules");     		
     		if(conditionEvents!=null && alertRules!=null);
     		{
     			initializer(alertRules);
     			BindUtils.postNotifyChange(null, null, SelectEventFilterStepViewModel.this, "*");
     		}
     	}
        getNextButton().setDisabled(false);
    }

    public void initializer(List<CMAlertRules> alertRules){
    	try {
    		
    		if(Sessions.getCurrent().getAttribute("QueryType")!= null && (!Sessions.getCurrent().getAttribute("QueryType").toString().isEmpty())){
    		GetData();
    		}
    	} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
      }
    
    @Override
    public String getNextStepZul() {
        return AlertActionStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.SQL_SERVER_OBJECT_TYPE_TIPS);
    }
    
    
    private A getRowCounts() {
		return rowCounts;
	}

	private void setRowCounts(A id) {
		this.rowCounts = id;
	}

    public A getSpecifiedPrivilegedUser() {
		return specifiedPrivilegedUser;
	}

	public void setSpecifiedPrivilegedUser(A id) {
		this.specifiedPrivilegedUser = id;
	} 
	
    private A getSpecifySQL() {
        return specifySQL;
    }
    
    private void setSpecifySQL(A id) {
        this.specifySQL=id;
    }
    
    private A getSpecifyDatabase() {
        return specifyDatabase;
    }
    
    private void setSpecifyDatabase(A id) {
        this.specifyDatabase=id;
    }
    
    private A getSpecifiedObjects() {
        return specifiedObjects;
    }
    
    private void setSpecifiedObjects(A id) {
        this.specifiedObjects=id;
    }
    
    private A getSpecifiedWords_db() {
        return specifiedWords_db;
    }
    
    private void setSpecifiedWords_db(A id) {
        this.specifiedWords_db=id;
    }
    
    private A getSpecifiedWords() {
        return specifiedWords;
    }
    
    private void setSpecifiedWords(A id) {
        this.specifiedWords=id;
    }
    
    private A getSpecifiedWordsName() {
        return specifiedWordsName;
    }
    
    private void setSpecifiedWordsName(A id) {
        this.specifiedWordsName=id;
    }
        
    private A getSelectEventType() {
        return selectEventType;
    }
    
    private void setSelectEventType(A id) {
        this.selectEventType=id;
    }
    
    private IderaDropdownList getAccessCheck() {
        return accessCheck;
    }
    
    private void setAccessCheck(IderaDropdownList id) {
        this.accessCheck=id;
    }
    
    private IderaDropdownList getPrivilegedUser() {
        return privilegedUser;
    }
    
    private void setPrivilegedUser(IderaDropdownList id) {
        this.privilegedUser=id;
    }
    
    @Command("afterRenderGrid")
    public void afterRenderGrid() {
        if (rgFlow.getItemCount() > 0) {
            rgFlow.setSelectedIndex(0);
        }
    }

@Command("eventAlertRules")
    public void eventAlertRules(@BindingParam("id") long id) {
        SpecifySQLServerViewModel.showSpecifySQLServersDialog(id);
    }

@Command("selectEventSource")
public void selectEventSource(@BindingParam("id") String id) throws RestException {
	AlertRulesFacade alertRulesFacade = new AlertRulesFacade();
	CategoryResponse categoryResponse = new CategoryResponse();
	CategoryRequest categoryRequest = new CategoryRequest();
	categoryRequest.setCategory(id);
	categoryResponse = alertRulesFacade.getCategoryInfo(categoryRequest);
	entitiesList = categoryResponse.getCategoryTable();
	setEntitiesList(entitiesList);
	BindUtils.postNotifyChange(null, null, SelectEventFilterStepViewModel.this, "*");
}

@Command("selectCategorySource")
public void selectCategorySource(@BindingParam("id") String id) throws RestException {
 certainId[0] = Integer.parseInt(id);
 Set lstbox = SelectEventTypeMain.getSelectedItems();
}



@Command("onCheckCertainEvent")
public void onCheckCertainEvent(@BindingParam("target") Checkbox target,@BindingParam("index") long index,@BindingParam("excludeCertainEventType") A excludeCertainEventType) {
	if (target.isChecked()) {
		String chkName = target.getName();
		checkedAlertRulesTypes.put(chkName, index);		
		excludeCertainEventType.setDisabled(false);
		Sessions.getCurrent().removeAttribute("Type");
		Sessions.getCurrent().removeAttribute("ExecludeCertainEventIds");
	}
	else 
	{
		checkedAlertRulesTypes.remove(target.getName());
		excludeCertainEventType.setDisabled(true);
		Sessions.getCurrent().removeAttribute("Type");
		Sessions.getCurrent().removeAttribute("ExecludeCertainEventIds");
	}
}
    @Command("onCheck")
    public void onCheck(@BindingParam("target") Checkbox target, @BindingParam("index") long index, @BindingParam("lstBox") IderaDropdownList lstBox,@BindingParam("lbl") A lbl) {
    	if (target.isChecked()) {
    		String chkName = target.getName();
    		checkedAlertRulesTypes.put(chkName, index);
    		if(lstBox!=null)
    		{
    			switch(lstBox.getId()) {
    			case "accessCheck" :
    				setAccessCheck(lstBox);
    				lstBox.setDisabled(false);
    				break; 
    			case "privilegedUser" :
    				setPrivilegedUser(lstBox);
    				lstBox.setDisabled(false);
    				break;       
    			}
    		}
    		else if( lbl!=null)
    		{
    			if(chkServer.isChecked())
    		{
    				sqlServerChecked=true;
    		}
    		else
    		{
    			sqlServerChecked=false;
    		}
    			
    			
    			
    		
    			switch(lbl.getId()) {
    			case "specifySQL" :
    				setSpecifySQL(lbl);
    				lbl.setDisabled(false);
    				Sessions.getCurrent().removeAttribute("SQL Server");
    	        	Sessions.getCurrent().removeAttribute("Instances");
    				break; 
    			case "specifyDatabase" :
    				setSpecifyDatabase(lbl);
    				lbl.setDisabled(false);
    				Sessions.getCurrent().removeAttribute("dbMatchString");
    				break;

    			case "specifiedObjects" :
    				setSpecifiedObjects(lbl);
    				Sessions.getCurrent().removeAttribute("objectMatchString");
    				lbl.setDisabled(false);
    				break;
    			case "specifiedWords_db" :
    				setSpecifiedWords_db(lbl);
    				Sessions.getCurrent().removeAttribute("hostMatchString");
    				lbl.setDisabled(false);
    				break; 
    			case "specifiedWords" :
    				setSpecifiedWords(lbl);
    				Sessions.getCurrent().removeAttribute("appMatchString");
    				lbl.setDisabled(false);
    				break;

    			case "specifiedWordsName" :
    				setSpecifiedWordsName(lbl);
    				Sessions.getCurrent().removeAttribute("loginMatchString");
    				lbl.setDisabled(false);
    				break;

    			case "selectEventType" :
    				setSelectEventType(lbl);
    				lbl.setDisabled(false);
    				break;	
    			case "specifiedPrivilegedUser" :
    				setSpecifiedPrivilegedUser(lbl);
    				Sessions.getCurrent().removeAttribute("privilegedUserNameMatch");
    				lbl.setDisabled(false);
    				break;
    			case "rowCounts" :
    				setRowCounts(lbl);
    				Sessions.getCurrent().removeAttribute("rowCountDetails");
    				lbl.setDisabled(false);
    				break;	
    				
    			}
    		}
    	} else {
    		if(lstBox!=null)
    		{
    			lstBox.setDisabled(true);
    			switch(lstBox.getId()) {
    			case "accessCheck" :
    				setAccessCheck(null);
    				break; 
    			case "privilegedUser" :
    				setPrivilegedUser(null);
    				break;       
    			}
    		}
    		else if( lbl!=null)
    		{
    			lbl.setDisabled(true);
    		}
    		checkedAlertRulesTypes.remove(target.getName());
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
    	
    	if(!chkServer.isChecked()){     
    		rs.setTargetInstances(null);
        	rs.setSqlServer(false);
        	Sessions.getCurrent().removeAttribute("SQL Server");
        	Sessions.getCurrent().removeAttribute("Instances");
        }
    	
    	if(!chkCertainEventType.isChecked()){
    		rs.setExcludeCertainMatchString(null);
    		Sessions.getCurrent().removeAttribute("Type");
    	}
		
    	if(!chkAccessCheck.isChecked()){
    		rs.setAccessChkMatchString(null);
    		rs.setAccessCheckPassedChk(false);
    		Sessions.getCurrent().removeAttribute("Access Check Passed");    		
		}
    	
    	if(!chkPrivilegedUser.isChecked()){
    		rs.setPrivilegedUserMatchString(null);
    		rs.setPrivilegedCheck(false);
    		Sessions.getCurrent().removeAttribute("PrivilegedMatchString");
    	}
    	
    	if(chkServer.isChecked()){
        	rs.setSqlServer(true);
    		Map<String, Object> Instances = new HashMap<String, Object>();
    		if(Sessions.getCurrent().getAttribute("Instances")!=null){
            	Instances = (Map<String, Object>) Sessions.getCurrent().getAttribute("Instances");
            }
    		else if(Sessions.getCurrent().getAttribute("SQL Server")!=null 
    				&& !Sessions.getCurrent().getAttribute("SQL Server").toString().isEmpty()){
    			Instances.put((String)Sessions.getCurrent().getAttribute("SQL Server"), Sessions.getCurrent().getAttribute("SQL Server"));
    		}
    		if (Instances!= null && (!Instances.isEmpty()))
            {
            	rs.setTargetInstances(Instances);
            }
    	}
        if(!chkDatabase.isChecked()){
			rs.setDbMatchString(null); 
			rs.setDatabaseName(false);
			rs.setDbNameList(null);
			Sessions.getCurrent().removeAttribute("dbMatchString");
		}
        else
		{
        	rs.setDatabaseName(true);
			String dbMatchString = "";
			rs.setDbFieldId(5);
			dbMatchString = (String) Sessions.getCurrent().getAttribute(
					"dbMatchString");
			if (dbMatchString != null && !dbMatchString.isEmpty()
					&& !dbMatchString.equals("")) {				
        			rs.setDbMatchString(dbMatchString);  		
        			
        		ListModelList<String> DBDataVal = GetExtractedDataOptions(dbMatchString);	
				SpecifyDatabaseViewModel DBViewModel = new SpecifyDatabaseViewModel();
				
				if (DBDataVal != null && (!DBDataVal.isEmpty())) {
					int length=DBDataVal.getSize();
					ListModelList<Data> dataList = new ListModelList<>();
					for(int i = 0; i < length; i++)
					{						
						dataList.add(DBViewModel.new Data(DBDataVal.get(i)));
					}
					rs.setDbNameList(dataList);
				}
			}
			
		}

        
        if(!chkObject.isChecked()){
			rs.setObjectMatchString(null);
			rs.setObjectName(false);
			rs.setObjectNameList(null);
			Sessions.getCurrent().removeAttribute("objectMatchString");
		}
		else
		{			 
			rs.setObjectName(true);
			rs.setObjectFieldId(6);
			String objectMatchString = "";
			objectMatchString = (String) Sessions.getCurrent().getAttribute("objectMatchString");
			if (objectMatchString != null && !objectMatchString.isEmpty()
					&& !objectMatchString.equals("")) {
        			rs.setObjectMatchString(objectMatchString); 
				
        		ListModelList<String> ObjectDataVal = GetExtractedDataOptions(objectMatchString);	
				SpecifyObjectsViewModel ObjectViewModel = new SpecifyObjectsViewModel();	
				
				if (ObjectDataVal != null && (!ObjectDataVal.isEmpty())) {
					int length=ObjectDataVal.getSize();
					ListModelList<Objects> dataList = new ListModelList<>();
					for(int i = 0; i < length; i++)
					{						
						dataList.add(ObjectViewModel.new Objects(ObjectDataVal.get(i)));
					}
					rs.setObjectNameList(dataList);
				}
			}
			
		}
        if(!chkHost.isChecked()){
			rs.setHostMatchString(null);
			rs.setHostName(false);
			rs.setHostNameList(null);
			Sessions.getCurrent().removeAttribute("hostMatchString");
		} 
		else
		{		 
			rs.setHostName(true);
			rs.setHostFieldId(10);
			String hostMatchString = "";
			hostMatchString = (String) Sessions.getCurrent().getAttribute(
					"hostMatchString");
			if (hostMatchString != null && !hostMatchString.isEmpty()
					&& !hostMatchString.equals("")) {				
        			rs.setHostMatchString(hostMatchString); 
				rs.setHostName(true);
				
				ListModelList<String> HostDataVal = GetExtractedDataOptions(hostMatchString);	
				SpecifyHostNameViewModel hostViewModel = new SpecifyHostNameViewModel();	
				
				if (HostDataVal != null && (!HostDataVal.isEmpty())) {
					int length=HostDataVal.getSize();
					ListModelList<Host> dataList = new ListModelList<>();
					for(int i = 0; i < length; i++)
					{						
						dataList.add(hostViewModel.new Host(HostDataVal.get(i)));
					}
					rs.setHostNameList(dataList);
				}
			}
			
		}
        
        if(!chkApplication.isChecked()){
			rs.setAppMatchString(null);
			rs.setApplicationName(false);
			rs.setAppNameList(null);
			Sessions.getCurrent().removeAttribute("appMatchString");
		}
		else
		{
			 
			rs.setApplicationName(true);
			rs.setAppFieldId(2);
			String appMatchString = "";
			appMatchString = (String) Sessions.getCurrent().getAttribute("appMatchString");
			if (appMatchString != null && !appMatchString.isEmpty()
					&& !appMatchString.equals("")) {
        			rs.setAppMatchString(appMatchString); 
				
        		ListModelList<String> AppDataVal = GetExtractedDataOptions(appMatchString);	
				SpecifyAppNameViewModel appViewModel = new SpecifyAppNameViewModel();	
				
				if (AppDataVal != null && (!AppDataVal.isEmpty())) {
					ListModelList<App> dataList = new ListModelList<>();
					int length=AppDataVal.getSize();
					for(int i = 0; i < length; i++)
					{						
						dataList.add(appViewModel.new App(AppDataVal.get(i)));
					}
					rs.setAppNameList(dataList);
				}
			}
			
		}
        
        if(!chkLogin.isChecked()){
			rs.setLoginMatchString(null);
			rs.setLoginName(false);
			rs.setLoginNameList(null);
			Sessions.getCurrent().removeAttribute("loginMatchString");
		}
		else
		{    		
			rs.setLoginName(true);
			rs.setLoginFieldId(3);
			String loginMatchString = "";
			loginMatchString = (String) Sessions.getCurrent().getAttribute("loginMatchString");
			if (loginMatchString != null && !loginMatchString.isEmpty()
					&& !loginMatchString.equals("")) {
				rs.setLoginMatchString(loginMatchString);
				
				ListModelList<String> LoginDataVal = GetExtractedDataOptions(loginMatchString);	
				SpecifyLoginViewModel loginViewModel = new SpecifyLoginViewModel();	
				
				if (LoginDataVal != null && (!LoginDataVal.isEmpty())) {					
					ListModelList<Login> dataList = new ListModelList<>();
					int length=LoginDataVal.getSize();
					for(int i = 0; i < length; i++)
					{						
						dataList.add(loginViewModel.new Login(LoginDataVal.get(i)));
					}
					rs.setLoginNameList(dataList);
				}
			}
			
		}
        
        if(!chkPrivilegedUserName.isChecked())
        {
        	rs.setPrivilegedUserName(false);
        	rs.setPrivilegedUserNameMatchString(null);
        	rs.setPrivilegedUserNameList(null);
        	Sessions.getCurrent().removeAttribute("privilegedUserNameMatch");
        }
        else
        {
	    isPrivilegedUserNameCheckboxChecked = true;
	    rs.setPrivilegedUserName(true);
	    rs.setPrivilegedFieldId(13);
	    String privilegedUserNameMatchString = "";
	    privilegedUserNameMatchString = (String) Sessions.getCurrent()
		    .getAttribute("privilegedUserNameMatch");
	    if (privilegedUserNameMatchString != null
		    && !privilegedUserNameMatchString.isEmpty()
		    && !privilegedUserNameMatchString.equals("")) {
		rs.setPrivilegedUserNameMatchString(privilegedUserNameMatchString);
		ListModelList<String> PrivilegedUserNameVal = GetExtractedDataOptions(privilegedUserNameMatchString);
		SpecifyPrivilegedUserViewModel privilegedViewModel = new SpecifyPrivilegedUserViewModel();
		ListModelList<PrivilegedUserName> dataList = new ListModelList<>();
		if (PrivilegedUserNameVal != null
			&& (!PrivilegedUserNameVal.isEmpty())) {

		    int length = PrivilegedUserNameVal.getSize();
		    for (int i = 0; i < length; i++) {
			dataList.add(privilegedViewModel.new PrivilegedUserName(
				PrivilegedUserNameVal.get(i)));
		    }
		}

		rs.setPrivilegedUserNameList(dataList);
	    }
	}

        if(!chkRowCounts.isChecked())
        {
        	rs.setRowCountMatchString(null);
        	rs.setRowCountWithTimeInterval(false);
        	Sessions.getCurrent().removeAttribute("rowCountDetails");
        }
        else
        {
        	rs.setRowCountWithTimeInterval(true);
        	String rowcountMatchString="";
        	rowcountMatchString =(String)Sessions.getCurrent().getAttribute("rowCountDetails");
        	rs.setRowCountMatchString(rowcountMatchString);
        	rs.setRowCountFieldId(14);        	
        }
        
        if(chkCertainEventType.isChecked()){
        	eventtype.setDataFormat(MatchType.Integer);
    		eventtype.set_type(EventType.SqlServer);
    	    eventCondition.set_boolValue(false);
    	    eventCondition.set_nulls(false);
    		eventCondition.set_inclusive(true);
    		eventCondition.set_targetInts(certainId);
    		rs.setExcludeCertainEventType(true);
			rs.setExcludeCertainFieldId(0);
    		try {    			
    			String matchString =  "";    			
    			if(Sessions.getCurrent().getAttribute("Type")!=null
    					&& !Sessions.getCurrent().getAttribute("Type").toString().isEmpty() ){
    				matchString=(String)Sessions.getCurrent().getAttribute("Type");
    				if(!matchString.equalsIgnoreCase("include(1)0value(0)")){
	    					rs.setExcludeCertainMatchString(matchString);
	    				String id=(String)Sessions.getCurrent().getAttribute("eventTypeId");	
	    				if(id==null){
	    					id="SECURITY";
	    				}
	    				String allEvents="";
	    				AlertRulesFacade alertRulesFacade = new AlertRulesFacade();
	         			CategoryResponse categoryResponse = new CategoryResponse();
	         			CategoryRequest categoryRequest = new CategoryRequest();
	         			categoryRequest.setCategory(id);
	    				categoryResponse = alertRulesFacade.getCategoryInfo(categoryRequest);
	    				List<CategoryData> entitiesListEvents = categoryResponse.getCategoryTable();
						int i=0;
	    				while(matchString.lastIndexOf(")")!=matchString.length()-1){
	    					int indexStart=matchString.lastIndexOf(")")+1;
	    					int indexEnd=(matchString.indexOf(",")==-1)?matchString.length():matchString.indexOf(",");
	    					int eventTypeId=Integer.parseInt(matchString.substring(indexStart,indexEnd));
	    					String firstHalf=matchString.substring(0,indexStart);
	    					String secondHalf=(matchString.indexOf(",")==-1)?"":matchString.substring(indexEnd+1);
	    					matchString=firstHalf+secondHalf;
	    					for(CategoryData cd:entitiesListEvents){
	    						if(cd.getEvtypeid()==eventTypeId){
	    							if(i==0)
	    								allEvents +=cd.getName();
	    							else
	    								allEvents +=","+cd.getName();
	    							i++;
	    						}
	    					}
	    					
	    					if(allEvents.length()>110){
	    						allEvents=allEvents.substring(0,110)+"...";
	    						break;
	    					}
	    				}
	    				Sessions.getCurrent().setAttribute("ExcludeCirtainEventString", allEvents);
	    			}
    				else
    					rs.setExcludeCertainMatchString(null);
    			}
				else
					rs.setExcludeCertainMatchString(null);
    		} catch (Exception e) {
    			// TODO Auto-generated catch block
    			e.printStackTrace();
    		}
        }
        
        else{
        	rs.setExcludeCertainEventType(false);
        	rs.setExcludeCertainMatchString(null);
        }
        
        
        if (checkedAlertRulesTypes.containsKey(Access_Check_Passed) || chkAccessCheck.isChecked()) {
        	if(accessCheck.getSelectedIndex()==1)
				rs.setAccessCheckPassed(false);
        	else
        		rs.setAccessCheckPassed(true);
            rs.setAccessCheckPassedChk(true);            
    		eventtype.setDataFormat(MatchType.Bool);
    		eventtype.set_type(EventType.SqlServer);
    		eventCondition.set_boolValue(rs.getAccessCheckPassed());
    		try {
				String accessChkMatchString =  eventCondition.UpdateMatchString(eventtype,eventCondition);
				rs.setAccessChkMatchString(accessChkMatchString);  					
				rs.setAccessChkFieldId(4);
			} catch (Exception e) {
			// TODO Auto-generated catch block
				e.printStackTrace();
			}
        }

        if (checkedAlertRulesTypes.containsKey(Is_Privileged_User) || chkPrivilegedUser.isChecked()) {
        	if(privilegedUser.getSelectedIndex()==1)
    			rs.setIsPrivilegedUser(false);
        	else
        		rs.setIsPrivilegedUser(true);
        	rs.setIsPrivilegedCheck(true);
        	eventtype.setDataFormat(MatchType.Bool);
    		eventtype.set_type(EventType.SqlServer);
    		eventCondition.set_boolValue(rs.getIsPrivilegedUser());
    		try {
				String privilegedUserMatchString =  eventCondition.UpdateMatchString(eventtype,eventCondition);	
				rs.setPrivilegedUserMatchString(privilegedUserMatchString); 
				rs.setPrivilegedUserFieldId(7);
			} catch (Exception e) {
			// TODO Auto-generated catch block
				e.printStackTrace();
			}
        }
        
        wizardSaveEntity.setRegulationSettings(rs);
    }

    public void GetData() {   	
    	if(Sessions.getCurrent().getAttribute("Access Check Passed")!=null){    		
    		String strMatchString = (String) Sessions.getCurrent().getAttribute("Access Check Passed");

    		Map<String, String> EventNodeDataValue = new HashMap<String, String>();
    		try {
    			EventNodeDataValue = keyValueParser.ParseString(strMatchString);
    		} catch (Exception e) {
    			// TODO Auto-generated catch block
    			e.printStackTrace();
    		}
    		for (Map.Entry<String, String> entry : EventNodeDataValue
    				.entrySet()) {
    			
    			String chkName = chkAccessCheck.getName();
        		long index = 8;
        		checkedAlertRulesTypes.put(chkName,index);

    			if (entry.getKey().equals("value")) {
    				chkAccessCheck.setChecked(true);
    				if(entry.getValue().equals("true")||entry.getValue().equals("True")){
    					accessCheck.setDisabled(false);
    					rs.setAccessCheckPassed(false);
    					accessCheck.setSelectedIndex(0);    					
    				}
    				else{
    					accessCheck.setDisabled(false);
    					rs.setAccessCheckPassed(true);
    					accessCheck.setSelectedIndex(1);    					
    				}
    			}
    		}
    	}
    	
    	if(Sessions.getCurrent().getAttribute("PrivilegedMatchString")!=null){
    		String strMatchString = (String) Sessions.getCurrent().getAttribute("PrivilegedMatchString");

    		Map<String, String> EventNodeDataValue = new HashMap<String, String>();
    		try {
    			EventNodeDataValue = keyValueParser.ParseString(strMatchString);
    		} catch (Exception e) {
    			// TODO Auto-generated catch block
    			e.printStackTrace();
    		}
    		for (Map.Entry<String, String> entry : EventNodeDataValue
    				.entrySet()) {

    			String chkName = chkPrivilegedUser.getName();
        		long index = 7;
        		checkedAlertRulesTypes.put(chkName,index);
        		
    			if (entry.getKey().equals("value")) {
    				chkPrivilegedUser.setChecked(true);
    				if(entry.getValue().equals("true")||entry.getValue().equals("True")){
    					rs.setIsPrivilegedUser(false);
    					privilegedUser.setSelectedIndex(0);
    					privilegedUser.setDisabled(false);
    				}
    				else{
    					rs.setIsPrivilegedUser(true);
    					privilegedUser.setSelectedIndex(1);
    					privilegedUser.setDisabled(false);
    				}
    			}
    		}
    	}
    	if(Sessions.getCurrent().getAttribute("SQL Server")!=null){
    		String strInstances = (String) Sessions.getCurrent().getAttribute("SQL Server");
    		if(!strInstances.equals("<ALL>")){
    		chkServer.setChecked(true);
    		specifySQL.setDisabled(false);    		
    	 }
    		//need to work on it;
       	}
    	
    	
    	if(Sessions.getCurrent().getAttribute("dbMatchString")!=null){
    		chkDatabase.setChecked(true);
    		specifyDatabase.setDisabled(false);
    		String chkName = chkDatabase.getName();
    		long index = 1;
    		checkedAlertRulesTypes.put(chkName,index);
            rs.setDbMatchString((String)Sessions.getCurrent().getAttribute("dbMatchString"));
    		
    	}
    	
    	if(Sessions.getCurrent().getAttribute("objectMatchString")!=null){
    		chkObject.setChecked(true);
    		specifiedObjects.setDisabled(false);
    		String chkName = chkObject.getName();
    		long index = 2;
    		checkedAlertRulesTypes.put(chkName,index);
    		rs.setObjectMatchString((String)Sessions.getCurrent().getAttribute("objectMatchString"));
    	}
    	
    	if(Sessions.getCurrent().getAttribute("hostMatchString")!=null){
    		chkHost.setChecked(true);
    		specifiedWords_db.setDisabled(false);
    		String chkName = chkHost.getName();
    		long index = 3;
    		checkedAlertRulesTypes.put(chkName,index);
    		rs.setHostMatchString((String)Sessions.getCurrent().getAttribute("hostMatchString"));
    	}
    	
    	if(Sessions.getCurrent().getAttribute("appMatchString")!=null){
    		chkApplication.setChecked(true);
    		specifiedWords.setDisabled(false);
    		String chkName = chkApplication.getName();
    		long index = 4;
    		checkedAlertRulesTypes.put(chkName,index);
    		rs.setAppMatchString((String)Sessions.getCurrent().getAttribute("appMatchString"));
    	}
    	
    	if(Sessions.getCurrent().getAttribute("loginMatchString")!=null){
    		chkLogin.setChecked(true);
    		specifiedWordsName.setDisabled(false);
    		String chkName = chkLogin.getName();
    		long index = 5;
    		checkedAlertRulesTypes.put(chkName,index);
    		rs.setLoginMatchString((String)Sessions.getCurrent().getAttribute("loginMatchString"));
    	}
    	
    	if(Sessions.getCurrent().getAttribute("Type")!=null){
    		long index = 9;
    		checkedAlertRulesTypes.put(Exclude_certain_Event_Type, index);
    		chkCertainEventType.setChecked(true);
    		excludeCertainEventType.setDisabled(false);
    		rs.setExcludeCertainEventType(true);
    		rs.setExcludeCertainMatchString((String)Sessions.getCurrent().getAttribute("Type"));
    		rs.setExcludeCertainFieldId(0);
    	}
    	if(Sessions.getCurrent().getAttribute("privilegedUserNameMatch")!=null)
    	{
    		long index=10;
    		chkPrivilegedUserName.setChecked(true);
    		specifiedPrivilegedUser.setDisabled(false);
    		String chkPrivName= chkPrivilegedUserName.getName();
    		checkedAlertRulesTypes.put(chkPrivName, index);
    		rs.setPrivilegedUserNameMatchString((String)Sessions.getCurrent().getAttribute("privilegedUserNameMatch"));
    	}
    		
    	if(Sessions.getCurrent().getAttribute("rowCountDetails")!=null)
    	{
    		long index=11;
    		chkRowCounts.setChecked(true);
    		rowCounts.setDisabled(false);
    		String chkRowCountsName=chkRowCounts.getName();
    		checkedAlertRulesTypes.put(chkRowCountsName, index);
    		rs.setRowCountMatchString((String)Sessions.getCurrent().getAttribute("rowCountDetails"));
    	}
    	
    }
    
    @Override
    public void onFinish(AddAlertRulesSaveEntity wizardSaveEntity){
    	onBeforeNext(wizardSaveEntity);
    	SummaryStepViewModel summaryStepViewModel=new SummaryStepViewModel();
    	summaryStepViewModel.onFinish(wizardSaveEntity);
    }
    
    private ListModelList<String> GetExtractedDataOptions(String MatchString)
    {
    	Map<String, String> EventNodeDataValue = new HashMap<String, String>();
    	String strValues = "";
    	String active = MatchString;
    	int index = MatchString.indexOf("(");

    	try {
    		while (index != -1) {
    			String sKey, sValue;
    			String length;

    			sKey = active.substring(0, index);
    			active = active.substring(index + 1);
    			index = active.indexOf(")");
    			length = active.substring(0, index);
    			active = active.substring(index + 1);
    			sValue = (active.subSequence(0, Integer.parseInt(length))
    					.toString());
    			active = (active.substring(Integer.parseInt(length))
    					.toString());
    			EventNodeDataValue.put(sKey, sValue);
    			index = active.indexOf("(");
    		}
    	} catch (Exception e) {
    		try {
    			throw new Exception("Improperly formed KeyValue string.", e);
    		} catch (Exception e1) {
    			// TODO Auto-generated catch block
    			e1.printStackTrace();
    		}
    	}

    	int iCountArray = 0;

    	for (Map.Entry<String, String> entry : EventNodeDataValue.entrySet()) 
    	{
    		if (entry.getKey().equals("count")) {
    			iCountArray = Integer.parseInt(entry.getValue());
    		}	
    	}
    	
    	ListModelList<String> list = new ListModelList<>();		
    	for (int k = 0; k < iCountArray; k++) {			
    		strValues += (String) EventNodeDataValue.get("" + k) + "^";	
    		list.add((String) EventNodeDataValue.get("" + k));
    	}
    	return list;
    }
    
    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
    }
} 