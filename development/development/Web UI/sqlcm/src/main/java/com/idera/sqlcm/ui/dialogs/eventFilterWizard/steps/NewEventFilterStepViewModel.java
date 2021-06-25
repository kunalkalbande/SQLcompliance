package com.idera.sqlcm.ui.dialogs.eventFilterWizard.steps;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.idera.sqlcm.entities.CMExportEvent;
import com.idera.sqlcm.entities.CMExportEventConditionData;
import com.idera.sqlcm.entities.RegulationType;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.KeyValueParser;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.EventFilterSaveEntity;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.RegulationSettings;
import com.idera.sqlcm.ui.eventFilters.SpecifyAppNameViewModel;
import com.idera.sqlcm.ui.eventFilters.SpecifyAppNameViewModel.App;
import com.idera.sqlcm.ui.eventFilters.SpecifyDatabaseViewModel;
import com.idera.sqlcm.ui.eventFilters.SpecifyDatabaseViewModel.Data;
import com.idera.sqlcm.ui.eventFilters.SpecifyHostNameViewModel;
import com.idera.sqlcm.ui.eventFilters.SpecifyHostNameViewModel.Host;
import com.idera.sqlcm.ui.eventFilters.SpecifyLoginViewModel;
import com.idera.sqlcm.ui.eventFilters.SpecifyLoginViewModel.Login;
import com.idera.sqlcm.ui.eventFilters.SpecifyObjectsViewModel;
import com.idera.sqlcm.ui.eventFilters.SpecifyObjectsViewModel.Objects;
import com.idera.sqlcm.ui.eventFilters.SpecifySQLServerViewModel;
import com.idera.sqlcm.ui.eventFilters.SpecifySessionLoginViewModel;
import com.idera.sqlcm.ui.eventFilters.SpecifySessionLoginViewModel.SessionLogin;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.A;

public class NewEventFilterStepViewModel extends AddWizardStepBase {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/eventFilterWizard/steps/new-event-filter-step.zul";
	RegulationSettings rs = new RegulationSettings();
	//protected PrivilegedUser privilegedUser;
	protected IsPrivilegedUser isPrivilegedUser;
	private List<CMExportEvent> events;
	private List<CMExportEventConditionData> conditionEvents;
	public KeyValueParser keyValueParser;
	
	@Wire
	Combobox privilegedUser;

	@Wire
	private Checkbox chkSQLServer;
	
	@Wire
	private Checkbox chkLoginName;
	
	@Wire
	private Checkbox chkHostName;
	
	@Wire
	private Checkbox chkObject;
	
	@Wire
	private Checkbox chkSessionLogin;
	
	@Wire 
	private Checkbox chkDatabase;
	
	@Wire 
	private Checkbox chkApplication;
	
	@Wire 
	private Checkbox chkPriviledgeUser;
	
	@Wire
	private Listbox accessCheck;
	
	@Wire
	private A specifySQL;
	
	@Wire
	private A specifyDatabase;
	
	@Wire
	private A specifyObject;
	
	@Wire
	private A specifyApplicationName;
	
	@Wire
	private A specifyHostName;
	
	@Wire
	private A specifyLoginName;
	
	@Wire
	private A specifySessionLogin;
	
	@Wire
	private Checkbox chkAccessCheck;


	private String regulationGuidelinesDesc;

	public static final String SQL_Server = "SQL Server";
	public static final String Database_Name = "Database Name";
	public static final String Object_Name = "Object Name";
	public static final String Host_Name = "Host Name";
	public static final String Application_Name = "Application Name";
	public static final String Login_Name = "Login Name";
	public static final String Session_Login_Name = "Session Login";

	public static final String Access_Check_Passed = "Access Check Passed";
	public static final String Is_Privileged_User = "Is Privileged User";

	public static final String Exclude_certain_Event_Type = "Exclude certain Event Type";
	private ListModelList<RegulationType> regulationTypeListModelList;
	private Map<String, Long> checkedAlertRulesTypes = new HashMap<String, Long>();

	private Combobox priviligedUser;

	@Wire
	Combobox privilegedUsers;

	public static enum PrivilegedUser {
		LABEL_TRUE(ELFunctions.getLabel(SQLCMI18NStrings.LABEL_TRUE), 1), LABEL_FALSE(
				ELFunctions.getLabel(SQLCMI18NStrings.LABEL_FALSE), 0);

		private String label;
		private int id;

		PrivilegedUser(String label, int id) {
			this.label = label;
			this.id = id;
		}

		public int getId() {
			return id;
		}
	}

	public static enum IsPrivilegedUser {
		True(ELFunctions.getLabel(SQLCMI18NStrings.TRUE), 1), False(ELFunctions
				.getLabel(SQLCMI18NStrings.FALSE), 2);

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

	@Wire
	private Radiogroup rgFlow;

	public Combobox getPriviligedUser() {
		return priviligedUser;
	}

	public void setPriviligedUser(Combobox priviligedUser) {
		this.priviligedUser = priviligedUser;
	}

	public A getSpecifySQL() {
		return specifySQL;
	}

	public void setSpecifySQL(A specifySQL) {
		this.specifySQL = specifySQL;
	}

	public A getSpecifyDatabase() {
		return specifyDatabase;
	}

	public void setSpecifyDatabase(A specifyDatabase) {
		this.specifyDatabase = specifyDatabase;
	}

	public A getSpecifyObject() {
		return specifyObject;
	}

	public void setSpecifyObject(A specifyObject) {
		this.specifyObject = specifyObject;
	}

	public A getSpecifyApplicationName() {
		return specifyApplicationName;
	}

	public void setSpecifyApplicationName(A specifyApplicationName) {
		this.specifyApplicationName = specifyApplicationName;
	}

	public A getSpecifyHostName() {
		return specifyHostName;
	}

	public void setSpecifyHostName(A specifyHostName) {
		this.specifyHostName = specifyHostName;
	}

	public A getSpecifyLoginName() {
		return specifyLoginName;
	}

	public void setSpecifyLoginName(A specifyLoginName) {
		this.specifyLoginName = specifyLoginName;
	}

	public A getSpecifySessionLogin() {
		return specifySessionLogin;
	}

	public void setSpecifySessionLogin(A specifySessionLogin) {
		this.specifySessionLogin = specifySessionLogin;
	}

	@Command("eventFilters")
	public void eventFilters(@BindingParam("id") long id) {
		SpecifySQLServerViewModel.showSpecifySQLServersDialog(id);
	}

	@Override
	public String getNextStepZul() {
		return SummaryStepViewModel.ZUL_PATH;
	}

	@Override
	public boolean isValid() {
		return true;
	}

	@Command("afterRenderGrid")
	public void afterRenderGrid() {
		// select first item of radio group
		if (rgFlow.getItemCount() > 0) {
			rgFlow.setSelectedIndex(0);
		}
	}

	@Command("onCheck")
	public void onCheck(@BindingParam("target") Checkbox target,
			@BindingParam("index") long index,
			@BindingParam("lstBox") Combobox lstBox, @BindingParam("lbl") A lbl) {
		if (target.isChecked()) {
			String chkName = target.getName();
			checkedAlertRulesTypes.put(chkName, index);
			if (lstBox != null) {
				switch (lstBox.getId()) {
				case "privilegedUsers":
					setPriviligedUser(lstBox);
					lstBox.setDisabled(false);
					break;
				}
			} else if (lbl != null) {
				switch (lbl.getId()) {
				case "specifySQL":
					setSpecifySQL(lbl);
					lbl.setDisabled(false);
					break;
				case "specifyDatabase":
					setSpecifyDatabase(lbl);
					lbl.setDisabled(false);
					break;

				case "specifyObject":
					setSpecifyObject(lbl);
					lbl.setDisabled(false);					
					break;
				case "specifyApplicationName":
					setSpecifyApplicationName(lbl);
					lbl.setDisabled(false);
					break;
				case "specifyHostName":
					setSpecifyHostName(lbl);
					lbl.setDisabled(false);
					break;

				case "specifyLoginName":
					setSpecifyLoginName(lbl);
					lbl.setDisabled(false);
					break;

				case "specifySessionLogin":
					setSpecifySessionLogin(lbl);
					lbl.setDisabled(false);
					break;
				}
			}
		} else {			
			
			String chkName = target.getName();
			checkedAlertRulesTypes.put(chkName, index);
			if (lstBox != null) {
				lstBox.setDisabled(true);				
			} else if (lbl != null) {
				lbl.setDisabled(true);
				switch (lbl.getId()) {
				case "specifySQL":
					Sessions.getCurrent().removeAttribute("TargetInst");
					Sessions.getCurrent().removeAttribute("SQL Server");					
					break;
				case "specifyDatabase":
					Sessions.getCurrent().removeAttribute("DbMatchString");
					Sessions.getCurrent().removeAttribute("specifyDataBaseMatchString");
					break;

				case "specifyObject":
					Sessions.getCurrent().removeAttribute("ObjectMatchString");
					Sessions.getCurrent().removeAttribute("specifyObjectMatchString");
					break;
				case "specifyApplicationName":
					Sessions.getCurrent().removeAttribute("AppMatchString");
					Sessions.getCurrent().removeAttribute("specifyAppMatchString");
					break;
				case "specifyHostName":
					Sessions.getCurrent().removeAttribute("HostMatchString");
					Sessions.getCurrent().removeAttribute("specifyHostMatchString");
					break;

				case "specifyLoginName":
					Sessions.getCurrent().removeAttribute("LoginMatchString");
					Sessions.getCurrent().removeAttribute("specifyLoginMatchString");
					break;

				case "specifySessionLogin":
					Sessions.getCurrent().removeAttribute("sessionLoginMatchString");
					Sessions.getCurrent().removeAttribute("specifySessionLoginMatchString");
					break;
				}
			}
			checkedAlertRulesTypes.remove(target.getName());
		}

		BindUtils.postNotifyChange(null, null,
				NewEventFilterStepViewModel.this, "regulationGuidelinesDesc");
	}

	@Override
	protected void doOnShow(EventFilterSaveEntity wizardSaveEntity) {
		getNextButton().setDisabled(false);
		String strMatchString = (String) Sessions.getCurrent().getAttribute(
				"PrivilegedMatchString");
		if (strMatchString != null) {
			strMatchString = strMatchString.substring((strMatchString.length()-4));
			if (strMatchString.equals("True")) {
				privilegedUsers.setSelectedIndex(0);
				rs.setIsPrivilegedCheck(true);
			} else {
				privilegedUsers.setSelectedIndex(1);
				rs.setIsPrivilegedCheck(false);
			}
		}
		
		if(Sessions.getCurrent().getAttribute("QueryType")!=null)
    	{
     		conditionEvents = (List<CMExportEventConditionData>) Sessions.getCurrent().getAttribute("conditionEvents");
     		events = (List<CMExportEvent>) Sessions.getCurrent().getAttribute("events");
     		if(conditionEvents!=null && events!=null);
     		{
     			initializer(events);
     			BindUtils.postNotifyChange(null, null, NewEventFilterStepViewModel.this, "*");
     		}
     	}
	}

	public void initializer(List<CMExportEvent> events){
		GetData();
	}
	
	public void GetData() { 
    	if(Sessions.getCurrent().getAttribute("PrivilegedMatchString")!=null){
    		String strMatchString = (String) Sessions.getCurrent().getAttribute("PrivilegedMatchString");
    		chkPriviledgeUser.setChecked(true);
    		privilegedUsers.setDisabled(false);
    		Map<String, String> EventNodeDataValue = new HashMap<String, String>();
    		try {
    			EventNodeDataValue = keyValueParser.ParseString(strMatchString);
    		} catch (Exception e) {
    			// TODO Auto-generated catch block
    			e.printStackTrace();
    		}
    	}
    	if(Sessions.getCurrent().getAttribute("SQL Server")!=null){
    		String strInstances = (String) Sessions.getCurrent().getAttribute("SQL Server");
    		if(!strInstances.equals("<ALL>")){
    		chkSQLServer.setChecked(true);
    		specifySQL.setDisabled(false);
    		}
    		//need to work on it;
       	}
    	
    	
    	if(Sessions.getCurrent().getAttribute("DbMatchString")!=null){
    		chkDatabase.setChecked(true);
    		specifyDatabase.setDisabled(false);
    		String chkName = chkDatabase.getName();
    		long index = 1;
    		checkedAlertRulesTypes.put(chkName,index);
            rs.setDbMatchString((String)Sessions.getCurrent().getAttribute("DbMatchString"));
    		
    	}
    	
    	if(Sessions.getCurrent().getAttribute("ObjectMatchString")!=null){
    		chkObject.setChecked(true);
    		specifyObject.setDisabled(false);
    		String chkName = chkObject.getName();
    		long index = 2;
    		checkedAlertRulesTypes.put(chkName,index);
    		rs.setObjectMatchString((String)Sessions.getCurrent().getAttribute("ObjectMatchString"));
    	}
    	
    	if(Sessions.getCurrent().getAttribute("HostMatchString")!=null){
    		chkHostName.setChecked(true);
    		specifyHostName.setDisabled(false);
    		String chkName = chkHostName.getName();
    		long index = 3;
    		checkedAlertRulesTypes.put(chkName,index);
    		rs.setHostMatchString((String)Sessions.getCurrent().getAttribute("HostMatchString"));
    	}
    	
    	if(Sessions.getCurrent().getAttribute("AppMatchString")!=null){
    		chkApplication.setChecked(true);
    		specifyApplicationName.setDisabled(false);
    		String chkName = chkApplication.getName();
    		long index = 4;
    		checkedAlertRulesTypes.put(chkName,index);
    		rs.setAppMatchString((String)Sessions.getCurrent().getAttribute("AppMatchString"));
    	}
    
    	if(Sessions.getCurrent().getAttribute("LoginMatchString")!=null){
    		chkLoginName.setChecked(true);
    		specifyLoginName.setDisabled(false);
    		String chkName = chkLoginName.getName();
    		long index = 5;
    		checkedAlertRulesTypes.put(chkName,index);
    		rs.setLoginMatchString((String)Sessions.getCurrent().getAttribute("LoginMatchString"));
    	}
    	
    	if(Sessions.getCurrent().getAttribute("sessionLoginMatchString")!=null){
    		chkSessionLogin.setChecked(true);
    		specifySessionLogin.setDisabled(false);
    		String chkName = chkSessionLogin.getName();
    		long index = 11;
    		checkedAlertRulesTypes.put(chkName,index);
    		rs.setAppMatchString((String)Sessions.getCurrent().getAttribute("sessionLoginMatchString"));
    	}
    	
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

	@Override
	public void onBeforeNext(EventFilterSaveEntity wizardSaveEntity) {
		if(chkSQLServer.isChecked()){
			rs.setSQLServer(true);
			rs.setTargetInstances(null);
			if(Sessions.getCurrent().getAttribute("SQL Server")!=null 
					&& !Sessions.getCurrent().getAttribute("SQL Server").toString().isEmpty()){
				Map<String, Object> Instances = new HashMap<String, Object>();
				String serverString=(String)Sessions.getCurrent().getAttribute("SQL Server");
				Instances.put(serverString, serverString);
				if (Instances != null && (!Instances.isEmpty())) {
					rs.setTargetInstances(Instances);
				}
			}
		}
		else{
			rs.setSQLServer(false);
			rs.setTargetInstances(null);
		}

		if (chkDatabase.isChecked()) {
			rs.setDatabaseName(true);	
			rs.setDbNameList(null);
			rs.setDbMatchString("");
			ListModelList<Data> dbNameList = new ListModelList<>();
			
			if(Sessions.getCurrent().getAttribute(
					"specifyDataBaseMatchString")!=null){
				String dbRadio;
				dbRadio = (String) Sessions.getCurrent().getAttribute(
						"specifyDataBaseRadio");
				dbNameList = (ListModelList<Data>) Sessions.getCurrent()
						.getAttribute("specifyDataBaseList");
				if (dbNameList != null && (!dbNameList.isEmpty())) {
					rs.setDbNameList(dbNameList);
					if (dbRadio != null && (!dbRadio.isEmpty())) {
						rs.setDbRadioSelected(dbRadio);
					}
				}
	
				String dbMatchString = "";
				dbMatchString = (String) Sessions.getCurrent().getAttribute(
						"specifyDataBaseMatchString");
				if (dbMatchString != null) {
					rs.setDbMatchString(dbMatchString);
				}
			}
			else if(Sessions.getCurrent().getAttribute("DbMatchString")!=null){
				String strMatchString = (String) Sessions.getCurrent().getAttribute("DbMatchString");
				dbNameList.clear();
				String active = strMatchString;
				int index = strMatchString.indexOf("(");

				try {
					SpecifyDatabaseViewModel dbNameTmp = new SpecifyDatabaseViewModel();
					while (index != -1) {
						String sValue,sKey;
						String length;
						active = active.substring(index + 1);
						sKey = active.substring(0, index);
						index = active.indexOf(")");
						length = active.substring(0, index);
						active = active.substring(index + 1);
						sValue = (active.subSequence(0, Integer.parseInt(length))
								.toString());
						active = (active.substring(Integer.parseInt(length)).toString());
						dbNameTmp.setEventDatabaseName(sValue);
						dbNameTmp.addItem();
						index = active.indexOf("(");
						if(active.indexOf("blanks")==0)
							index=-1;
					}
					rs.setDbNameList(dbNameTmp.getDataList());
				}
				catch (Exception e) {
					try {
						throw new Exception("Improperly formed KeyValue string.", e);
					} catch (Exception e1) {
						// TODO Auto-generated catch block
						e1.printStackTrace();
					}
				}
				rs.setDbMatchString(strMatchString);
			}
		}
		else
		{  
			rs.setDatabaseName(false);
			rs.setDbMatchString(null);
		}

		if (chkObject.isChecked()) {
			rs.setObjectName(true);
			rs.setObjectMatchString("");
			rs.setObjectNameList(null);
			if(Sessions.getCurrent().getAttribute(
					"specifyObjectMatchString")!=null){
				ListModelList<Objects> objectNameList = new ListModelList<>();				
				String objectRadio;
				objectRadio = (String) Sessions.getCurrent().getAttribute("specifyObjectRadio");
				objectNameList = (ListModelList<Objects>) Sessions.getCurrent().getAttribute("specifyObjectList");
				if (objectNameList != null && !objectNameList.isEmpty()) {
					rs.setObjectNameList(objectNameList);
					if ( objectRadio != null && !objectRadio.isEmpty()) {
						rs.setObjectRadioSelected(objectRadio);
					}
				}
	
				String objectMatchString = "";
				objectMatchString = (String) Sessions.getCurrent().getAttribute(
						"specifyObjectMatchString");
				if (objectMatchString != null) {
					rs.setObjectMatchString(objectMatchString);
				}
			}
			
			else if(Sessions.getCurrent().getAttribute("ObjectMatchString")!=null){
				String strMatchString = (String) Sessions.getCurrent().getAttribute("ObjectMatchString");
				String active = strMatchString;
				int index = strMatchString.indexOf("(");

				try {
					SpecifyObjectsViewModel objNameTmp = new SpecifyObjectsViewModel();
					while (index != -1) {
						String sValue,sKey;
						String length;
						active = active.substring(index + 1);
						sKey = active.substring(0, index);
						index = active.indexOf(")");
						length = active.substring(0, index);
						active = active.substring(index + 1);
						sValue = (active.subSequence(0, Integer.parseInt(length))
								.toString());
						active = (active.substring(Integer.parseInt(length)).toString());
						objNameTmp.setObjectNameMatch(sValue);
						objNameTmp.addItem();
						index = active.indexOf("(");
						if(active.indexOf("blanks")==0)
							index=-1;
					}
					rs.setObjectNameList(objNameTmp.getDataList());
				}
				catch (Exception e) {
					try {
						throw new Exception("Improperly formed KeyValue string.", e);
					} catch (Exception e1) {
						// TODO Auto-generated catch block
						e1.printStackTrace();
					}
				}
				rs.setDbMatchString(strMatchString);
			}

		}
		else
		{
			rs.setObjectName(false);
			rs.setObjectMatchString(null);
		}

		if (chkHostName.isChecked()) {
			rs.setHostName(true);
			rs.setHostMatchString("");
			rs.setHostNameList(null);
			if(Sessions.getCurrent().getAttribute("specifyHostMatchString")!=null){
				ListModelList<Host> hostNameList = new ListModelList<>();
				String hostRadio;
				hostRadio = (String) Sessions.getCurrent().getAttribute(
						"specifyHostRadio");
				hostNameList = (ListModelList<Host>) Sessions.getCurrent()
						.getAttribute("specifyHostList");
				if ((hostNameList != null) && (!hostNameList.isEmpty())) {
					rs.setHostNameList(hostNameList);
					if (hostRadio != null && (!hostRadio.isEmpty())) {
						rs.setHostRadioSelected(hostRadio);
					}
				}
	
				String hostMatchString = "";
				hostMatchString = (String) Sessions.getCurrent().getAttribute(
						"specifyHostMatchString");
				if (hostMatchString != null) {
					rs.setHostMatchString(hostMatchString);
				}
			}
			
			else if(Sessions.getCurrent().getAttribute("HostMatchString")!=null){
				String strMatchString = (String) Sessions.getCurrent().getAttribute("HostMatchString");
				String active = strMatchString;
				int index = strMatchString.indexOf("(");

				try {
					SpecifyHostNameViewModel dbNameTmp = new SpecifyHostNameViewModel();
					while (index != -1) {
						String sValue,sKey;
						String length;
						active = active.substring(index + 1);
						sKey = active.substring(0, index);
						index = active.indexOf(")");
						length = active.substring(0, index);
						active = active.substring(index + 1);
						sValue = (active.subSequence(0, Integer.parseInt(length))
								.toString());
						active = (active.substring(Integer.parseInt(length)).toString());
						dbNameTmp.setHostNameMatch(sValue);
						dbNameTmp.addItem();
						index = active.indexOf("(");
						if(active.indexOf("blanks")==0)
							index=-1;
					}
					rs.setHostNameList(dbNameTmp.getDataList());
				}
				catch (Exception e) {
					try {
						throw new Exception("Improperly formed KeyValue string.", e);
					} catch (Exception e1) {
						// TODO Auto-generated catch block
						e1.printStackTrace();
					}
				}
				rs.setDbMatchString(strMatchString);
			}
			
		}
		else
		{
			rs.setHostName(false);
			rs.setHostMatchString(null);
		}

		if (chkApplication.isChecked()) {
			rs.setApplicationName(true);
			rs.setAppMatchString("");
			rs.setAppNameList(null);
			if( Sessions.getCurrent().getAttribute(
					"specifyAppMatchString")!=null){
				ListModelList<App> appNameList = new ListModelList<>();
				String appRadio;
				appRadio = (String) Sessions.getCurrent().getAttribute(
						"specifyAppRadio");
				appNameList = (ListModelList<App>) Sessions.getCurrent()
						.getAttribute("specifyAppList");
				if (appNameList != null && (!appNameList.isEmpty())) {
					rs.setAppNameList(appNameList);
					if (appRadio != null && (!appRadio.isEmpty())) {
						rs.setAppRadioSelected(appRadio);
					}
				}
	
				String appMatchString = "";
				appMatchString = (String) Sessions.getCurrent().getAttribute(
						"specifyAppMatchString");
				if (appMatchString != null) {
					rs.setAppMatchString(appMatchString);
				}
			}
			
			else if(Sessions.getCurrent().getAttribute("AppMatchString")!=null){
				String strMatchString = (String) Sessions.getCurrent().getAttribute("AppMatchString");
				String active = strMatchString;
				int index = strMatchString.indexOf("(");

				try {
					SpecifyAppNameViewModel dbNameTmp = new SpecifyAppNameViewModel();
					while (index != -1) {
						String sValue,sKey;
						String length;
						active = active.substring(index + 1);
						sKey = active.substring(0, index);
						index = active.indexOf(")");
						length = active.substring(0, index);
						active = active.substring(index + 1);
						sValue = (active.subSequence(0, Integer.parseInt(length))
								.toString());
						active = (active.substring(Integer.parseInt(length)).toString());
						dbNameTmp.setAppNameMatch(sValue);
						dbNameTmp.addItem();
						index = active.indexOf("(");
						if(active.indexOf("blanks")==0)
							index=-1;
					}
					rs.setAppNameList(dbNameTmp.getDataList());
				}
				catch (Exception e) {
					try {
						throw new Exception("Improperly formed KeyValue string.", e);
					} catch (Exception e1) {
						// TODO Auto-generated catch block
						e1.printStackTrace();
					}
				}
				rs.setDbMatchString(strMatchString);
			}
			
		}
		else
		{
			rs.setApplicationName(false );
			rs.setAppMatchString(null);
		}

		if (chkLoginName.isChecked()) {
			rs.setLoginName(true);
			rs.setLoginMatchString("");
			rs.setLoginNameList(null);
			if(Sessions.getCurrent().getAttribute(
					"specifyLoginMatchString")!=null){
				ListModelList<Login> loginNameList = new ListModelList<>();
				String loginRadio;
				loginRadio = (String) Sessions.getCurrent().getAttribute(
						"specifyLoginRadio");
				loginNameList = (ListModelList<Login>) Sessions.getCurrent()
						.getAttribute("specifyLoginList");
				if (loginNameList != null && (!loginNameList.isEmpty())) {
					rs.setLoginNameList(loginNameList);
					if (loginRadio != null && (!loginRadio.isEmpty())) {
						rs.setLoginRadioSelected(loginRadio);
					}
				}
	
				String loginMatchString = "";
				loginMatchString = (String) Sessions.getCurrent().getAttribute(
						"specifyLoginMatchString");
				if (loginMatchString != null) {
					rs.setLoginMatchString(loginMatchString);
				}
			}
			else if(Sessions.getCurrent().getAttribute("LoginMatchString")!=null){
				String strMatchString = (String) Sessions.getCurrent().getAttribute("LoginMatchString");
				String active = strMatchString;
				int index = strMatchString.indexOf("(");

				try {
					SpecifyLoginViewModel dbNameTmp = new SpecifyLoginViewModel();
					while (index != -1) {
						String sValue,sKey;
						String length;
						active = active.substring(index + 1);
						sKey = active.substring(0, index);
						index = active.indexOf(")");
						length = active.substring(0, index);
						active = active.substring(index + 1);
						sValue = (active.subSequence(0, Integer.parseInt(length))
								.toString());
						active = (active.substring(Integer.parseInt(length)).toString());
						dbNameTmp.setLoginNameMatch(sValue);
						dbNameTmp.addItem();
						index = active.indexOf("(");
						if(active.indexOf("blanks")==0)
							index=-1;
					}
					rs.setLoginNameList(dbNameTmp.getDataList());
				}
				catch (Exception e) {
					try {
						throw new Exception("Improperly formed KeyValue string.", e);
					} catch (Exception e1) {
						// TODO Auto-generated catch block
						e1.printStackTrace();
					}
				}
				rs.setDbMatchString(strMatchString);
			}
			
		}
		else
		{
			rs.setLoginName(false);
			rs.setLoginMatchString(null);
		}

		if (chkSessionLogin.isChecked()) {
			rs.setSessionLoginName(true);
			rs.setSessionLoginMatchString("");
			rs.setSessionLoginNameList(null);
			if(Sessions.getCurrent()
					.getAttribute("specifySessionLoginMatchString")!=null){
				ListModelList<SessionLogin> sessionLoginNameList = new ListModelList<>();
				String sessionLoginRadio;
				sessionLoginRadio = (String) Sessions.getCurrent().getAttribute(
						"specifySessionLoginRadio");
				sessionLoginNameList = (ListModelList<SessionLogin>) Sessions
						.getCurrent().getAttribute("specifySessionLoginList");
				if (sessionLoginNameList != null
						&& (!sessionLoginNameList.isEmpty())) {
					rs.setSessionLoginNameList(sessionLoginNameList);
					if (sessionLoginRadio != null && (!sessionLoginRadio.isEmpty())) {
						rs.setSessionLoginRadioSelected(sessionLoginRadio);
					}
				}
				String sessionLoginMatchString = "";
				sessionLoginMatchString = (String) Sessions.getCurrent()
						.getAttribute("specifySessionLoginMatchString");
				if (sessionLoginMatchString != null) {
					rs.setSessionLoginMatchString(sessionLoginMatchString);
				}
			}
			
			else if(Sessions.getCurrent().getAttribute("sessionLoginMatchString")!=null){
				String strMatchString = (String) Sessions.getCurrent().getAttribute("sessionLoginMatchString");
				String active = strMatchString;
				int index = strMatchString.indexOf("(");

				try {
					SpecifySessionLoginViewModel dbNameTmp = new SpecifySessionLoginViewModel();
					while (index != -1) {
						String sValue,sKey;
						String length;
						active = active.substring(index + 1);
						sKey = active.substring(0, index);
						index = active.indexOf(")");
						length = active.substring(0, index);
						active = active.substring(index + 1);
						sValue = (active.subSequence(0, Integer.parseInt(length))
								.toString());
						active = (active.substring(Integer.parseInt(length)).toString());
						dbNameTmp.setSessionLoginNameMatch(sValue);
						dbNameTmp.addItem();
						index = active.indexOf("(");
						if(active.indexOf("blanks")==0)
							index=-1;
					}
					rs.setSessionLoginNameList(dbNameTmp.getDataList());
				}
				catch (Exception e) {
					try {
						throw new Exception("Improperly formed KeyValue string.", e);
					} catch (Exception e1) {
						// TODO Auto-generated catch block
						e1.printStackTrace();
					}
				}
				rs.setDbMatchString(strMatchString);
			}
			
		}
		else
		{
			rs.setSessionLoginName(false);
			rs.setSessionLoginMatchString(null);
		}
		
		if(chkPriviledgeUser.isChecked())
		{
			rs.setIsPrivilegedCheck(true);		
			
			if (privilegedUsers.getSelectedIndex()==0) {
				String matchString = "value(4)True";
				rs.setSessionPrivilegedMatchString(matchString);
			} else {
				String matchString = "value(5)False";
				rs.setSessionPrivilegedMatchString(matchString);
			}		
		}
		else{
			rs.setIsPrivilegedCheck(false);
			rs.setSessionPrivilegedMatchString(null);
		}

		if (checkedAlertRulesTypes.containsKey(Exclude_certain_Event_Type)) {
			rs.setExcludeCertainEventType(true);
		}
		wizardSaveEntity.setRegulationSettings(rs);
	}
	
	@Override
    public void onFinish(EventFilterSaveEntity wizardSaveEntity){
    	onBeforeNext(wizardSaveEntity);
    	SummaryStepViewModel summaryStepViewModel=new SummaryStepViewModel();
    	summaryStepViewModel.onFinish(wizardSaveEntity);
    }
	

	@Command("checkPrivilegedUser")
	public void checkPrivilegedUser(@BindingParam("id") String id) {
		isPrivilegedUser = IsPrivilegedUser.valueOf(id);
		boolean bvar = Boolean.parseBoolean(isPrivilegedUser.label);
		rs.setIsPrivilegedUser(bvar);

		if (bvar) {
			String matchString = "value(4)True";
			rs.setSessionPrivilegedMatchString(matchString);
		} else {
			String matchString = "value(5)False";
			rs.setSessionPrivilegedMatchString(matchString);
		}
	}

	@Override
	public String getTips() {
		return ELFunctions
				.getLabel(SQLCMI18NStrings.SQL_SERVER_ObJECT_TYPE_FILTER_TIPS);
	}
	
	
	private String GetExtractedDataOptions(String MatchString)
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
		}
		return strValues.substring(0, (strValues.length()-1));
	}

	@Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
    }
}