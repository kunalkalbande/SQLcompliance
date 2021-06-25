package com.idera.sqlcm.ui.dialogs;

import java.io.*;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import javax.activation.MimetypesFileTypeMap;
import javax.servlet.http.HttpSession;
import javax.xml.ws.Action;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.ForwardEvent;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Label;
import org.zkoss.zul.Filedownload;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Listcell;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Timer;
import org.zkoss.zul.Window;

import com.google.common.eventbus.AllowConcurrentEvents;
import com.idera.sqlcm.entities.CMActivityLogs;
import com.idera.sqlcm.entities.CMColumnDetails;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.CMTableDetails;
import com.idera.sqlcm.entities.ProfilerObject;
import com.idera.common.Utility;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.auditReports.ColumnSearchReport;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addserverwizard.ServerWizardViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.AddWizardStepBase;
import com.idera.sqlcm.ui.instancedetails.InstanceEventsViewModel;
import com.idera.sqlcm.ui.instancedetails.InstanceOverview;
import com.idera.sqlcm.wizard.AbstractAlertWizardViewModel;
import com.idera.sqlcm.wizard.WizardAlertStepManager;

public class ColumnSearchViewModel{
public static final String ZUL_URL = "~./sqlcm/instancedetails/column_search.zul";

	private String help;

	public List<CMDatabase> serverList;
	public List tableList;
	public ListModelList columns=null;
	public static CMInstance instanceDetail;
	public List<CMTableDetails> tableDetails= new ArrayList<CMTableDetails>();
	public List<CMTableDetails> allTableDetails;
	public List<CMColumnDetails> allColumnDetails;
	public List<CMColumnDetails> ColumnDetails= new ArrayList<CMColumnDetails>();
	public List<String> databaseList;
	public List server;
	public String instanceName;
	public String dbName;
	public String tableName;
	public long databaseId;
	public long instanceId;
	public String profileName;
	public List defaultDb=new ArrayList();
	public static String activeProfile;
	public List<ProfilerObject> profileList;
	public List<String> uniqueCategoryList;
	private List<CMColumnDetails> allColDetails = new ArrayList<CMColumnDetails>();
	
	@Wire
	private Timer timer;

	@Wire
    private Combobox tableNameList;
	
	@Wire
	private Listbox columnList;
	
	@Wire
	private Label activeSearchProfile;
	
	@Wire
	private Button performSearch;
	
	@Wire
	private Button exportReport;
	
	
	public List<String> getUniqueCategoryList() {
		return uniqueCategoryList;
	}


	public void setUniqueCategoryList(List<String> uniqueCategoryList) {
		this.uniqueCategoryList = uniqueCategoryList;
	}

	
	
	
	public List<CMColumnDetails> getAllColDetails() {
		return allColDetails;
	}


	public void setAllColDetails(List<CMColumnDetails> allColDetails) {
		this.allColDetails = allColDetails;
	}


	public String getProfileName() {
		return profileName;
	}


	public void setProfileName(String profileName) {
		this.profileName = profileName;
	}
	
	public List<CMTableDetails> getAllTableDetails() {
		return allTableDetails;
	}
	public void setAllTableDetails(List<CMTableDetails> allTableDetails) {
		this.allTableDetails = allTableDetails;
	}
	
	public List<String> getDatabaseList() {
		return databaseList;
	}


	public void setDatabaseList(List<String> databaseList) {
		this.databaseList = databaseList;
	}
	
	public static String getActiveProfile() {
		return activeProfile;
	}
		
	public static void setActiveProfile(String activeProfile) {
		ColumnSearchViewModel.activeProfile = activeProfile;
	}


	public List<CMTableDetails> getTableDetails() {
		return tableDetails;
	}


	public void setTableDetails(List<CMTableDetails> tableDetails) {
		this.tableDetails = tableDetails;
	}

	public List<ProfilerObject> getProfileList() {
		return profileList;
	}

	public void setProfileList(List<ProfilerObject> profileList) {
		this.profileList = profileList;
	}
	
	public long getInstanceId() {
		return instanceId;
	}


	public void setInstanceId(long instanceId) {
		this.instanceId = instanceId;
	}


	public long getDatabaseId() {
		return databaseId;
	}


	public void setDatabaseId(long databaseId) {
		this.databaseId = databaseId;
	}


	public String getInstanceName() {
		return instanceName;
	}


	public String getDbName() {
		return dbName;
	}


	public void setDbName(String dbName) {
		this.dbName = dbName;
	}


	public String getTableName() {
		return tableName;
	}


	public void setTableName(String tableName) {
		this.tableName = tableName;
	}


	public void setInstanceName(String instanceName) {
		this.instanceName = instanceName;
	}


	public static CMInstance getInstanceDetail() {
		return instanceDetail;
	}


	public static void setInstanceDetail(CMInstance instanceDetail) {
		ColumnSearchViewModel.instanceDetail = instanceDetail;
	}


	public List<CMColumnDetails> getAllColumnDetails() {
		return allColumnDetails;
	}


	public void setAllColumnDetails(List<CMColumnDetails> allColumnDetails) {
		this.allColumnDetails = allColumnDetails;
	}


	public List<CMColumnDetails> getColumnDetails() {
		return ColumnDetails;
	}


	public void setColumnDetails(List<CMColumnDetails> columnDetails) {
		ColumnDetails = columnDetails;
	}


	public String getHelp() {
		return help;
	}

		
	 public List<CMDatabase> getServerList() {
		return serverList;
	}

	public void setServerList(List<CMDatabase> serverList) {
		this.serverList = serverList;
	}


	public List getTableList() {
		return tableList;
	}


	public void setTableList(List tableList) {
		this.tableList = tableList;
	}


	public ListModelList getColumns() {
		return columns;
	}

	public void setColumns(ListModelList columns) {
		this.columns = columns;
	}

	public static void showColumnSearch(CMInstance instance) {
		    setInstanceDetail(instance);
	        Window window = (Window) Executions.createComponents(ZUL_URL, null, null);
	        window.doHighlighted();
	        HttpSession session = (HttpSession)(Executions.getCurrent()).getDesktop().getSession().getNativeSession();
			session.setAttribute("instanceDetail",instance);
	       
	    }
	
	public static void showColumnSearchAfterConfigSearch(String activeProfile, CMInstance instance) {
		if(null != activeProfile){
			setActiveProfile(activeProfile);
			setInstanceDetail(instanceDetail);
		}
		else
			setActiveProfile("");
    }
	 
	  @Command
	  public void closeDialog(@BindingParam("comp") Window x) {
	      x.detach();
	      InstanceEventsViewModel obj= new InstanceEventsViewModel();
	      obj.refreshData();
	  }
	  
	  @AfterCompose
	    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
	        Selectors.wireComponents(view, this, false);
	        help  = "http://wiki.idera.com/x/eAC5Ag";
	        getProfile();
	        serverList();
	        getProfileData();
	        
      if ((getActiveProfile()!=null) && !(getActiveProfile().equals("")) && !(getActiveProfile().equals("None Selected"))){
    	  activeSearchProfile.setValue(getActiveProfile());	   
    	  activeSearchProfile.setTooltiptext(getActiveProfile());
    	  performSearch.setDisabled(false);
	        }
      else
      {
    	  performSearch.setDisabled(true);
    	  activeSearchProfile.setValue("None Selected");	     
      }
      }
	    
	  
	  @Command("refreshEvents")
	  public void refresh()
	  {
		  try{
			  	HttpSession session = (HttpSession)(Executions.getCurrent()).getDesktop().getSession().getNativeSession();
				CMInstance instanceDel=(CMInstance)session.getAttribute("instanceDetail");
				setInstanceDetail(instanceDel);
			  	String isUpdated=DatabasesFacade.getIsUpdated();
			  	if(isUpdated.equals("1"))
			  	{
			  		if(!tableDetails.isEmpty())
				  	{
					  tableDetails.clear();				 
				  	}
				  	if(!ColumnDetails.isEmpty())
				  	{
				  		ColumnDetails.clear();
				  	}
				  	getProfile();
			        serverList();
			        getProfileData();
			        activeSearchProfile.setValue(getActiveProfile());
			        activeSearchProfile.setTooltiptext(getActiveProfile());
			        timer.stop();
			        DatabasesFacade.updateIsUpdated("0");
			        timer.stop();
			        if ((getActiveProfile()==null) ||(getActiveProfile()=="") || (getActiveProfile()=="None Selected") ){
			        	
			        	performSearch.setDisabled(true);
			        
			        }
			        else {
			        	performSearch.setDisabled(false);
			        }
			        
			        if ((getActiveProfile()!=null) && !(getActiveProfile().equals("")) && !(getActiveProfile().equals("None Selected"))){
			      	  activeSearchProfile.setValue(getActiveProfile());	 
			      	  activeSearchProfile.setTooltiptext(getActiveProfile());
			  	        
			  	        }
			        else {
			      	  performSearch.setDisabled(true);
			      	  activeSearchProfile.setValue("None Selected");	     
			        }
			        
					BindUtils.postNotifyChange(null, null, ColumnSearchViewModel.this, "*");
			  	}
			  	
		  }
		  catch(Exception e)
		  {
			e.getStackTrace();  
		  }
	  }
	  
	  public List<CMDatabase> serverList()
	  {		  
	    try{	    	
	    	List<CMDatabase>  list;
	    	List<CMTableDetails> tableDetail;
	    	List<CMColumnDetails> columnDetail;
	    	List<String> databaseNameList = new ArrayList<String>();
	    	List<CMDatabase> actualList= new ArrayList<CMDatabase>();
	    	CMInstance instance=getInstanceDetail();
	    	setInstanceId(instance.getId());
			        defaultDb.add("master");
			        defaultDb.add("model");
			        defaultDb.add("tempdb");
			        defaultDb.add("msdb");
			        defaultDb.add("mssqlsystemresource");
			        
	    	     String name= instance.getInstanceName();
	    	     
	    	     setInstanceName(name);
		        list= DatabasesFacade.getAllDatabaseList(instance.getId());
		        
		        Iterator<CMDatabase> itr= list.iterator();
		        int i=0;
		        while(itr.hasNext())
		        {
		        	CMDatabase obj= itr.next();
		        	if(i==0){
				        CMDatabase selectDb = new CMDatabase(); 
				        selectDb.setDatabaseId(0);
				        selectDb.setName("Select a database (blank for all)");
				        selectDb.setServerId(instance.getId());
				        actualList.add(selectDb);
				        databaseNameList.add(selectDb.getName());
				        i++;
		        	}
		        	if(defaultDb.contains(obj.getName()))
		        	{
		        		list.remove(obj.getName());
		        		
		        	}
		        	else
		        	{
		        		actualList.add(obj);
		        		databaseNameList.add(obj.getName());
		        	}
		        	
		        }
		        setServerList(actualList);
		        setDatabaseList(databaseNameList);
		        tableDetail=DatabasesFacade.getTableDetailSummaryForAll(databaseList, instanceId, activeProfile);  
		        setAllTableDetails(tableDetail);
		        columnDetail=DatabasesFacade.getColumnDetailSummaryForAll(databaseList, instanceId, activeProfile);
		        setAllColumnDetails(columnDetail);
		  	}
	  	catch(Exception e)
	  	{
	  		e.getStackTrace();
	  	}
		  return serverList;
		  
	  }
	 
	  @SuppressWarnings("rawtypes")
	public void getProfileData()
	  {   


		try
		{
			List<ProfilerObject> profileData= DatabasesFacade.getProfileDetails();
			setProfileList(profileData);
			
			List<String> categoryList=new ArrayList();
		      Iterator<ProfilerObject> itr = profileData.iterator();
		      while(itr.hasNext()){
		       ProfilerObject po = itr.next();
		       if(!categoryList.contains(po.getProfileName())){
		          categoryList.add(po.getProfileName());
		       }
		      }
		      setUniqueCategoryList(categoryList);
		      if(categoryList.isEmpty())
		      {
		    	  performSearch.setDisabled(true);
		      }
		      else
		      {
		    	  performSearch.setDisabled(false);
		      }
		      BindUtils.postNotifyChange(null, null, ColumnSearchViewModel.this, "*");
			
		}
		catch(Exception e)
		{
			
		}
	  }
  
  
	  @Command("selectedRow")
	  public void columnDetails(@BindingParam("id") List<Component> object)
	  {
		  String dbName="";
		  String tblName="";
		  Iterator<CMColumnDetails> itr;
		  if(null!=object)
		  {
			  List<Component> selectedItem=columnList.getSelectedItem().getChildren();
			  for(Component currentComp : selectedItem)
			  {
				  Listcell lc = (Listcell) currentComp;
			        if(lc.getColumnIndex() == 0){
			        	dbName = lc.getLabel();
			            System.out.println("name = " + dbName);
			        }
	
			        if(lc.getColumnIndex()==1){
			        	tblName = lc.getLabel();
			            System.out.println("role = " + tblName);
			        }
	
			  }
			  itr=allColumnDetails.iterator();
			  List<CMColumnDetails> temp=new ArrayList<CMColumnDetails>();;
			  while(itr.hasNext())
			  {
				  
				  CMColumnDetails colDetails=itr.next();
				  if(colDetails.getDatabaseName().equals(dbName) && colDetails.getTableName().equals(tblName))
				  {
					colDetails.setSize(getSizeOfDataType(colDetails.getDataType())); 
					
					temp.add(colDetails);  
				  }
			  }
			  setColumnDetails(temp);
			  BindUtils.postNotifyChange(null, null, ColumnSearchViewModel.this, "columnDetails");
		  }
	  }

	  //Start - SCM-216: Table Listing for current instance
	  @Command("selectDb")
	  @NotifyChange("tableList")
		public List<CMTable> addDatabaseClick(@BindingParam("id") String dbName) {
			System.out.println("id is as" + dbName);
			try {
				List<CMTable> tables;

				setDbName(dbName);
				setTableName(null);
				if(!tableDetails.isEmpty())
				{
					tableDetails.clear();
				}
				if(!ColumnDetails.isEmpty())
				{
					ColumnDetails.clear();
				}
				List tableList = new ArrayList();
				
				tables = DatabasesFacade.getTableList(instanceId, dbName, "");
				
				Iterator itr= tables.iterator();
				tableList.add("Select a table (blank for all)");
				  while(itr.hasNext())
				  {
					  CMTable details=(CMTable)itr.next();
					 
					  tableList.add(details.getFullTableName());
					 
				  }
				setTableList(tableList);
			} catch (Exception e) {
				e.getStackTrace();
			}
			 BindUtils.postNotifyChange(null, null, ColumnSearchViewModel.this, "*");
		 return tableList;
		}
	  
	  @Command("refreshTableNameList")
		public void refreshTableNameList() {
		  tableNameList.setText("Select a table (blank for all)");
		}
	  
	  @Command("selectedTable")
	  public void addTableClick(@BindingParam("id") String tableName){
		  setTableName(tableName);
		  if(!tableDetails.isEmpty())
			{
				tableDetails.clear();
			}
		  if(!ColumnDetails.isEmpty())
			{
				ColumnDetails.clear();
			}
		  BindUtils.postNotifyChange(null, null, ColumnSearchViewModel.this, "*");
		  System.out.println("Table name:" + tableName);
	  }
	  
	  
	  @Command("selectedProfile")
	  public void addProfileClick(@BindingParam("id") String profileName){
		  setProfileName(profileName);
		  BindUtils.postNotifyChange(null, null, ColumnSearchViewModel.this, "*");
		  System.out.println("Table name:" + profileName);
	  }
	  
	  //End - SCM-216: Table Listing for current instance
	  
	  @Command
	    public void showConfigeSearch()
	    {
		  try{
			  	timer.start();
			  	DatabasesFacade.updateIsUpdated("0");
		    	ConfigureSearchViewModel.showConfigeSearch(instanceDetail);
		  }
		  catch(Exception e)
		  {
			  e.getStackTrace();
		  }
	    }
	  
	  @Command("performSearch")
	  public void performSearch()
	  {
		  try{
			  
			 /* if(dbName!=null)
			  {*/
				  List<CMTableDetails> tableDetail = new ArrayList<CMTableDetails>();
				  List<CMColumnDetails> coldetail= new ArrayList<CMColumnDetails>();
				  Iterator<CMTableDetails> itr= allTableDetails.iterator();
				 
				  while(itr.hasNext() )
				  {
					  CMTableDetails details=itr.next();
					  Iterator<CMColumnDetails> itr1=allColumnDetails.iterator();
					  if(dbName==null || dbName.equals("Select a database (blank for all)")  )
					  {
						  tableDetail.add(details);
						  
					  }
					  else if(details.getDatabaseName().equals(dbName) && details.getSchemaTableName().equals(tableName))
					  {
						  tableDetail.add(details);
						 
					  }
					  else if(details.getDatabaseName().equals(dbName) && null==tableName)
					  {
						  tableDetail.add(details);					  
					  }
					  
					  while(itr1.hasNext())
					  {
						  
						  CMColumnDetails colDetails=itr1.next();
						  if(dbName==null || dbName.equals("Select a database (blank for all)") )
						  {
							  //tableDetail.add(details);
							  if(colDetails.getDatabaseName().equals(details.getDatabaseName()) && details.getSchemaTableName().equals(colDetails.getTableName()))
							  {
								  colDetails.setSize(getSizeOfDataType(colDetails.getDataType())); 
								  coldetail.add(colDetails);
							  }
						  }
						  else if(details.getDatabaseName().equals(dbName) && details.getSchemaTableName().equals(tableName))
						  {
							  //tableDetail.add(details);
							  if(colDetails.getDatabaseName().equals(dbName) && colDetails.getTableName().equals(tableName))
							  {	  colDetails.setSize(getSizeOfDataType(colDetails.getDataType())); 
								  coldetail.add(colDetails);  
							  }
						  }
						  else if(details.getDatabaseName().equals(dbName) && (null==tableName||"Select a table (blank for all)".equals(tableName)))
						  {
							  if(colDetails.getDatabaseName().equals(dbName) && details.getSchemaTableName().equals(colDetails.getTableName()))
							  {
								  colDetails.setSize(getSizeOfDataType(colDetails.getDataType())); 
								  coldetail.add(colDetails);  
							  }
						  }
					  }
					  
				  }
	
				  setTableDetails(tableDetail);
				  setAllColDetails(coldetail);
				 				  
				  if(tableDetail.size() <= 0)
				  {
					  WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_GET_COLUMN_SEARCH_DATA);
				  }
		  }
		  catch(Exception e)
		  {
			  e.getStackTrace();
		  }
		  BindUtils.postNotifyChange(null, null, ColumnSearchViewModel.this, "*");
	  }
	  
	 public void getProfile() {
		  try{
		    	String activeProfile = "";
		    	activeProfile= DatabasesFacade.getActiveProfile();
			    setActiveProfile(activeProfile);
			    if(getActiveProfile().equals("") || null==getActiveProfile())
			    {
			    	performSearch.setDisabled(true);
			    }
			    else
			    {
			    	performSearch.setDisabled(false);	
			    }
			    
			    
			    if ("".equals(getActiveProfile())) {
					activeSearchProfile.setValue("None Selected");
					
				} else {
					setActiveProfile(getActiveProfile());
					activeSearchProfile.setValue(getActiveProfile());
					activeSearchProfile.setTooltiptext(getActiveProfile());
					
				}
			 }
		  	catch(Exception e)
		  	{
		  		e.getStackTrace();
		  	}
		  BindUtils.postNotifyChange(null, null, ColumnSearchViewModel.this, "*");
	  }

	  @Command("createCsvFile")	  
	  public void createCsvFile()
	  {
		ColumnSearchReport.writeCsvFile(tableDetails, allColDetails, instanceName);
	  }
	  	  
	  	  
	  public int getSizeOfDataType(String type)
	  {
		  int size=0;
		  if(type.equals("nchar") || type.equals("nvarchar")||type.equals("varchar"))
		  {
			  size=8000;
		  }
		  if(type.equals("sysname"))
		  {
			  size=256;
		  }
		  if(type.equals("datetime")||type.equals("timestamp")||type.equals("bigint")||type.equals("float")||type.equals("money"))
		  {
			  size=8;
		  }
		  if(type.equals("int") || type.equals("real") || type.equals("smallmoney"))
		  {
			  size=4;
		  }
		  
		  if(type.equals("bit") || type.equals("tinyint"))
		  {
			  size=1;
		  }
		  
		  if(type.equals("smallint"))
		  {
			  size=2;
		  }
		  
		  if(type.equals("ntext") || type.equals("image") || type.equals("uniqueidentifier")||type.equals("text"))
		  {
			  size=16;
		  }
		  if(type.equals("decimal"))
		  {
			  size=17;
		  }		
		  if(type.equals("varbinary(50)") || type.equals("varchar(50)"))
		  {
			  size=50;
		  }
		  if(type.equals("char(10)"))
		  {
			  size=10;
		  }
		  if(type.equals("date")|| type.equals("datetimeoffset(7)"))
		  {
			  size=3;
		  }
		  if(type.startsWith("decimal("))
		  {
			 size=10; 
		  }
		  if(type.equals("geography") || type.equals("geometry") || type.equals("xml"))
		  {
			  size=-1;
		  }
		  if(type.equals("hierarchyid"))
		  {
			  size=892;
		  }
		  if(type.equals("sql_variant"))
		  {
			 size=8016; 
		  }
		  return size;
	  }
	  
}
