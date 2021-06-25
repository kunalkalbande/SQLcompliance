package com.idera.sqlcm.ui.auditReports;

import java.awt.print.PrinterJob;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Spinner;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.entities.Instance;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.FilterFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.facade.RegulationFacade;
import com.idera.sqlcm.facade.ReportsFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditApplicationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditConfigurationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditDMLResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditIderaDefaultValuesResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListApplicationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListConfigurationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListDMLRespose;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListLoginCreationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListLoginDeletionResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListObjectActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListPermissionDeniedActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListRegulatoryComplianceResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListRowCountResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListUserActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditLoginCreationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditLoginDeletionResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditObjectActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditPermissionDeniedActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditRegulatoryComplianceResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditRowCountResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditSettingResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditUserActivityResponse;
import com.idera.sqlcm.ui.auditReports.modelData.ConfigurationCheckReportModel;
import com.idera.sqlcm.ui.auditReports.modelData.ConfigurationServerCheckEvents;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;

import net.sf.jasperreports.engine.JRException;

@Init(superclass = true)
/*@AfterCompose(superclass = true)*/
public class AuditReportGridViewModel extends CommonGridViewModel {

	@Wire
	Paging listBoxPageId;
	    
	@Wire
	Spinner listBoxRowsBox;
	
	int PAGE_SIZE=50;
	
	private int prevPageSize;
	
	public CMAuditApplication cmAuditApplication;
	public CMAuditConfiguration cmAuditConfiguration;
	CMAuditDML cmAuditDML;
	CMAuditLoginCreation cmAuditLoginCreation;
	CMAuditLoginDeletion cmAuditLoginDeletion;
	CMAuditObjectActivity cmAuditObjectActivity;
	CMAuditPermissionDenied cmAuditPermissionDenied;
	CMAuditUserActivity cmAuditUserActivity;
	CMAuditRowCount cmAuditRowCount;
	private Boolean showSqlText;
	private int gridRowCount;
	private int showSqlTextInt;
	private int eventDatabase;
	private int eventDb;
	Logger logger = Logger.getLogger(SQLCMRestClient.class);
	public int getEventDb() {
		return eventDb;
	}

	public void setEventDb(int eventDb) {
		this.eventDb = eventDb;
	}

	@Wire
	Combobox txtdatabase;
	protected ListModelList<CMDatabase> databaseList;
	public Combobox getTxtdatabase() {
		return txtdatabase;
	}

	public void setTxtdatabase(Combobox txtdatabase) {
		this.txtdatabase = txtdatabase;
	}

	public ListModelList<CMDatabase> getDatabaseList() {
		return databaseList;
	}

	public void setDatabaseList(ListModelList<CMDatabase> databaseList) {
		this.databaseList = databaseList;
	}
	
	//Regulatory Compliance Check Report START
	@Wire
	Combobox txtserver;
	@Wire
	Combobox txtauditsetting;
	@Wire
	Combobox txtregulation;
	@Wire
	Combobox txtvalue;
	ListModelList<String> serversList, databasesList, auditSettingsList, regulationGuidelinesList, valuesList;
	ListModelList<String> configServersList, configDatabasesList;
	CMAuditRegulatoryCompliance cmAuditRegulatoryCompliance;
	int selectedServer, selectedDatabase, selectedAuditSetting, selectedRegulatoryGuideline, selectedValue;
	
	public void InitializeRCCComboboxLists() throws RestException {
		
		try {
			
			serversList = new ListModelList<String>();
			serversList.addAll(InstancesFacade.getAllAuditedInstances());
			
			databasesList = new ListModelList<String>();
			databasesList.addAll(DatabasesFacade.getDatabaseByServerName(URLEncoder.encode("<ALL>", "UTF-8")));
			
			auditSettingsList = new ListModelList<String>();
			auditSettingsList.addAll(FilterFacade.getAllAuditSettings());
			
			regulationGuidelinesList = new ListModelList<String>();
			regulationGuidelinesList.addAll(RegulationFacade.getAllRegulationGuidelines());
			
			valuesList = new ListModelList<String>();
			valuesList.add("Selected");
			valuesList.add("Varies");
			valuesList.add("Deselected");
			valuesList.add("N/A");
			
		}catch(Exception ex) {
			throw new RestException(ex);
		}
		
	}

	
		@Wire
		Combobox configSer;
		@Wire
		Combobox configDb;
		@Wire
		Combobox configAuditSettingsCombo;
		@Wire
		Combobox defaultStatusCombo;
		public Combobox getConfigSer() {
			return configSer;
		}

		public void setConfigSer(Combobox configSer) {
			this.configSer = configSer;
		}

		public Combobox getConfigDb() {
			return configDb;
		}

		public void setConfigDb(Combobox configDb) {
			this.configDb = configDb;
		}

		public Combobox getConfigAuditSettingsCombo() {
			return configAuditSettingsCombo;
		}

		public void setConfigAuditSettingsCombo(Combobox configAuditSettingsCombo) {
			this.configAuditSettingsCombo = configAuditSettingsCombo;
		}

		public Combobox getDefaultStatusCombo() {
			return defaultStatusCombo;
		}

		public void setDefaultStatusCombo(Combobox defaultStatusCombo) {
			this.defaultStatusCombo = defaultStatusCombo;
		}

		
	public int getSelectedServer() {
		return selectedServer;
	}

	public void setSelectedServer(int selectedServer) {
		this.selectedServer = selectedServer;
	}

	public int getSelectedDatabase() {
		return selectedDatabase;
	}

	public void setSelectedDatabase(int selectedDatabase) {
		this.selectedDatabase = selectedDatabase;
	}

	public int getSelectedAuditSetting() {
		return selectedAuditSetting;
	}

	public void setSelectedAuditSetting(int selectedAuditSetting) {
		this.selectedAuditSetting = selectedAuditSetting;
	}

	public int getSelectedRegulatoryGuideline() {
		return selectedRegulatoryGuideline;
	}

	public void setSelectedRegulatoryGuideline(int selectedRegulatoryGuideline) {
		this.selectedRegulatoryGuideline = selectedRegulatoryGuideline;
	}

	public int getSelectedValue() {
		return selectedValue;
	}

	public void setSelectedValue(int selectedValue) {
		this.selectedValue = selectedValue;
	}

	public Combobox getTxtserver() {
		return txtserver;
	}

	public void setTxtserver(Combobox txtserver) {
		this.txtserver = txtserver;
	}

	public Combobox getTxtauditsetting() {
		return txtauditsetting;
	}

	public void setTxtauditsetting(Combobox txtauditsetting) {
		this.txtauditsetting = txtauditsetting;
	}

	public Combobox getTxtregulation() {
		return txtregulation;
	}

	public void setTxtregulation(Combobox txtregulation) {
		this.txtregulation = txtregulation;
	}

	public Combobox getTxtvalue() {
		return txtvalue;
	}

	public void setTxtvalue(Combobox txtvalue) {
		this.txtvalue = txtvalue;
	}

	public List<String> getServersList() {
		return serversList;
	}

	public void setServersList(ListModelList<String> serversList) {
		this.serversList = serversList;
	}

	public List<String> getDatabasesList() {
		return databasesList;
	}

	public void setDatabasesList(ListModelList<String> databasesList) {
		this.databasesList = databasesList;
	}

	public List<String> getConfigServersList() {
		return configServersList;
	}

	public void setConfigServersList(ListModelList<String> serversList) {
		this.configServersList = serversList;
	}

	public List<String> getConfigDatabasesList() {
		return configDatabasesList;
	}

	public void setConfigDatabasesList(ListModelList<String> databasesList) {
		this.configDatabasesList = databasesList;
	}

	public List<String> getConfigAuditSettingsList() {
		return configAuditSettingsList;
	}

	public void setConfigAuditSettingsList(ListModelList<String> auditSettingsList) {
		this.configAuditSettingsList = auditSettingsList;
	}

	public List<String> getRegulationGuidelinesList() {
		return regulationGuidelinesList;
	}

	public void setRegulationGuidelinesList(ListModelList<String> regulationGuidelinesList) {
		this.regulationGuidelinesList = regulationGuidelinesList;
	}

	public List<String> getValuesList() {
		return valuesList;
	}

	public void setValuesList(ListModelList<String> valuesList) {
		this.valuesList = valuesList;
	}

	//Regulatory Compliance Check Report END

	/* start 5.3.1  
	 * Default value of * is missing for the Database and Application entry fields in Application History report.
	   Default value of * is missing for the Login entry field in LOGIN DELETION HISTORY REPORT.
	   Default value of * is missing for the Database and Login entry fields in USER ACTIVITY HISTORY REPORT.
	   Default value of * is missing for the Database and Target entry fields in OBJECT ACTIVITY REPORT.
	   Default value of * is missing for the Login entry field in LOGIN CREATION HISTORY REPORT.
	   Default value of * is missing for the Database and Login entry fields in  PERMISSION DENIED ACTIVITY REPORT.
	   Default values of * is missing for the Database, Login ,Primary Key and Target Object entry fields in BEFORE AND AFTER REPORT .
	   Default Date is missing in Application History report,LOGIN DELETION HISTORY REPORT,USER ACTIVITY HISTORY REPORT,OBJECT ACTIVITY REPORT,LOGIN CREATION HISTORY REPORT,PERMISSION DENIED ACTIVITY REPORT,BEFORE AND AFTER REPORT .
	*/
	private String login="*";
	private String target="*";
	private String schemaName;
	private String primaryKey="*";
	private String targetObject="*";
	private String category="ALL";
	private String database="*";
	private String column="*";
	private String application="*";
	private String privilegedUsers="";
	private int rowCountThreshold=0;
	private Boolean privilegedUser;
	private int privilegedUserInt;
	private Date toDate=toDateConverter();
	private Date fromDate=fromDateConverter();
	
	/**
	 * changes for Configuration check report ~ 5.6
	 */
	private int defaultStatus;
	private int auditSettings;
	private String selectedConfigDatabase;
	private ListModelList<String> configAuditSettingsList;
	String selectedConfigServer; //, selectedConfigDB="All";
	
	public String getSelectedConfigServer() {
		return selectedConfigServer;
	}

	public void setSelectedConfigServer(String selectedServer) {
		this.selectedConfigServer = selectedServer;
	}

/*	public String getSelectedConfigDB() {
		return selectedConfigDB;
	}

	public void setSelectedConfigDB(String selectedDatabase) {
		this.selectedConfigDB = selectedDatabase;
	}*/
	public int getDefaultStatus() {
		return defaultStatus;
	}

	public void setDefaultStatus(int defaultStatus) {
		this.defaultStatus = defaultStatus;
	}

	public int getAuditSettings() {
		return auditSettings;
	}

	public void setAuditSettings(int auditSettings) {
		this.auditSettings = auditSettings;
	}
	
	public List<String> getAuditSettingsList() {
		return auditSettingsList;
	}

	public void setAuditSettingsList(ListModelList<String> auditSettings) {
		this.auditSettingsList = auditSettings;
	}

	public void setConfigurationCheckReportsDefaults() throws RestException {
		configServersList = new ListModelList<>();
		configDatabasesList = new ListModelList<>();
		configServersList.addAll(serversList);
		configServersList.set(0, "ALL");
		configDatabasesList.addAll(databasesList);
		configDatabasesList.set(0, "ALL");
		configDatabasesList.remove("<Server Only>");
		configAuditSettingsList = new ListModelList<String>();
		ideraDefaultValues = reportsFacade.getIderaDefaultValues();
		// logger.info("AuditServer Defaults" + ideraDefaultValues.getServerDefaults().size());
		ideraDefaultDatabaseValues = reportsFacade.getIderaDatabaseDefaultValues();
		// logger.info("Audit DB Defaults" + ideraDefaultDatabaseValues.getServerDefaults().size());
		CMAuditSettingResponse obj = reportsFacade.getConfigAuditSettingsList();
		//logger.info("AUDIT REPORT" + obj.getAuditSettingsList());
		configAuditSettingsList.addAll(obj.getAuditSettingsList());
		
		if(configAuditSettingsList.indexOf("All") == 0) {
			configAuditSettingsList.set(0, "ALL");
		}
	}
	
	/**
	 * End of changes for Configuration check report ~ 5.6
	 */
	
	
	protected Map<String, Object> filterRequest = new TreeMap<>();
	protected List<CMEntity> entitiesList;
	protected ListModelList<CMEntity> entitiesModel;
	protected ListModelList<CMEntity> entitiesModelAbove2005;

	protected ListModelList<String> entitiesModelDatabases;
	public ListModelList<String> getEntitiesModelDatabases() {
		return entitiesModelDatabases;
	}

	public void setEntitiesModelDatabases(
			ListModelList<String> entitiesModelDatabases) {
		this.entitiesModelDatabases = entitiesModelDatabases;
	}

	protected ListModelList<CMEntity> entitiesModelSelect;
	protected ListModelList AuditReportColumnsList;
	protected CMAuditApplicationResponse cMAuditApplicationResponse;
	protected CMAuditConfigurationResponse cMAuditConfigurationResponse;
	List<CMAuditApplicationResponse> conditionEvents;
	//List<CMAuditConfigurationResponse>  conditionConfigurationEvents;
	List<CMAuditDMLResponse> conditionEventsDML;
	List<CMAuditLoginCreationResponse> conditionEventsLoginCreation;
	List<CMAuditLoginDeletionResponse> conditionEventsLoginDeletion;
	List<CMAuditObjectActivityResponse> conditionEventsObjectActivity;
	List<CMAuditPermissionDeniedActivityResponse> conditionEventsPermissionDeniedActivity;
	List<CMAuditUserActivityResponse> conditionEventsUserActivity;
	List<CMAuditRowCountResponse> conditionEventsRowCount;
	List<CMAuditRegulatoryComplianceResponse> conditionEventsRegulatoryCompliance;
	public ReportsFacade reportsFacade;
	public int refreshDuration; // SQLCM 5.4 SCM-9 start	
	
	private CMAuditIderaDefaultValuesResponse ideraDefaultValues;
	private CMAuditIderaDefaultValuesResponse ideraDefaultDatabaseValues;

	public int getRefreshDuration() {
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}
	// SQLCM 5.4 SCM-9 End
	
	/*Start 5.3.1
	Default Date is missing in Application History report,LOGIN DELETION HISTORY REPORT,USER ACTIVITY HISTORY REPORT,OBJECT ACTIVITY REPORT,LOGIN CREATION HISTORY REPORT,PERMISSION DENIED ACTIVITY REPORT,BEFORE AND AFTER REPORT .
	*/ 
	public Date toDateConverter()throws Exception
	{
		Calendar cal = Calendar.getInstance();
	    String dateString = new InstancesFacade().getLocalTime();
	   Date date=  Utils.parseDateOfUnknownFormat(dateString);
	  // DateFormat df = new SimpleDateFormat("yyyy-M-d"); 
	  // Date date =  df.parse(dateString);
	    cal.setTime(date);
	    cal.add(Calendar.HOUR_OF_DAY, 23);
	    cal.add(Calendar.MINUTE, 59);
	    cal.add(Calendar.SECOND, 59);
	    return cal.getTime();
	}
	public Date fromDateConverter()throws Exception
	{
		Calendar cal = Calendar.getInstance();
		String dateString = new InstancesFacade().getLocalTime();
		   Date date=  Utils.parseDateOfUnknownFormat(dateString);
	   // DateFormat df = new SimpleDateFormat("yyyy-M-d");
	    // DateFormat df = new SimpleDateFormat("M/d/yyyy");
	  //  Date date =  df.parse(dateString);
	    cal.setTime(date);
	    cal.add(Calendar.MONTH, -1);
	    cal.add(Calendar.DAY_OF_MONTH, -1);	    
	    return cal.getTime();
	}
	
	/*End 5.3.1
	Default Date is missing in Application History report,LOGIN DELETION HISTORY REPORT,USER ACTIVITY HISTORY REPORT,OBJECT ACTIVITY REPORT,LOGIN CREATION HISTORY REPORT,PERMISSION DENIED ACTIVITY REPORT,BEFORE AND AFTER REPORT .
	*/ 
	
	

	public AuditReportGridViewModel() throws Exception {
		
		InitializeRCCComboboxLists();
		setConfigurationCheckReportsDefaults();
		getEntity();
	}
	
	public ListModelList<?> getAuditReportColumnsList() {
		return AuditReportColumnsList;
	}
	
	private String instance;

	public String getInstance() {
		return instance;
	}

	public void setInstance(String instance) {
		this.instance = instance;
	}

	@Command("refreshData")
	public void refreshData() {
		Executions.getCurrent().sendRedirect("");
	}

	@Command
	public void print() {
		Clients.print();
	}

	@Command
	public void printPreview() {
		// new JRreportWindow(parent, true, repParams, repSrc, ds, "pdf");
	}

	@Command
	public void pageSetup() {
		PrinterJob.getPrinterJob();
	}

	@Command("exportToPdf")
	public void exportToPdf() throws JRException, IOException {
		makeCommonGridViewReport().generatePDFReport();
	}

	@Command("exportToExcel")
	public void exportToExcel() throws JRException, IOException {
		makeCommonGridViewReport().generateXLSReport();
	}

	@Command("exportToXml")
	public void exportToXml() throws JRException, IOException,
			TransformerException, ParserConfigurationException {
		makeCommonGridViewReport().generateXMLReport();
	}

	public void genrateReport() {
		makeCommonGridViewReport();
	}

	protected AuditReportGridViewReport makeCommonGridViewReport() {
		AuditReportGridViewReport auditReportGridViewReport = new AuditReportGridViewReport(
				"Application Activity", "", "");
		return auditReportGridViewReport;
	}

	@Command("openInstance")
	public void openInstance(@BindingParam("id") String id) {
		switch (id) {
		case "APPLICATION":
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("auditReportView"));
			break;
		case "CONFIGURATION":
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("configurationReportView"));
			break;
		case "DML":
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("dmlReportView"));
			break;
		case "LOGIN_CREATION":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("loginCreationReportView"));
			break;
		case "LOGIN_DELETION":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("loginDeletionReportView"));
			break;
		case "OBJECT_ACTIVITY":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("objectActivityReportView"));
			break;
		case "PERMISSION":
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("permissionReportView"));
			break;
		case "USER_ACTIVITY":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("userActivityReportView"));
			break;
		case "ROW_COUNT":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("rowCountReportView"));
			break;
		case "REGULATORY_COMPLIANCE":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("regulatoryComplianceReportView"));
			break;
		default:
			break;
		}
	}
	
	@Command("selectInstanceSource")
	public void selectInstanceSource() {
		Instance cmEntityObj = (Instance) entitiesModelAbove2005.get(eventDatabase);
		int auditedInstancesCount = entitiesModelAbove2005.getSize();
		Long instanceId = cmEntityObj.getId();
		entitiesModelDatabases = new ListModelList<>();
		entitiesModelDatabases.add("<ALL>");		
		HashSet<String> dbList = new HashSet<>();
		try {
			
			if (cmEntityObj.getInstanceName().equalsIgnoreCase("<ALL>"))
			{
				for (int i = 1; i < auditedInstancesCount ; i++)
				{
					dbList.addAll(DatabasesFacade.getEventsDatabasesForInstance(entitiesModelAbove2005.get(i).getId()));
				}
			}
			else 
			{
				dbList.addAll(DatabasesFacade.getEventsDatabasesForInstance(instanceId));
			}
			entitiesModelDatabases.addAll(dbList);
			
			databasesList = new ListModelList<String>();

			databasesList.addAll(DatabasesFacade.getDatabaseByServerName(URLEncoder.encode(serversList.get(selectedServer).toString(), "UTF-8")));
			configDatabasesList = new ListModelList<String>();
			if(selectedServer==0) {
				configDatabasesList.add("ALL");
				selectedDatabase=0;
			}
			else {
				configDatabasesList.addAll(databasesList);
				configDatabasesList.set(0, "ALL");
				configDatabasesList.remove("<Server Only>");
			}
			
		} catch (Exception e) {
			Logger logger = Logger.getLogger(SQLCMRestClient.class);
			logger.error(e.getMessage());
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		entitiesModelDatabases.setMultiple(true);
		databasesList.setMultiple(true);
		configDatabasesList.setMultiple(true);
		BindUtils.postNotifyChange(null, null, this, "entitiesModelDatabases");
		BindUtils.postNotifyChange(null, null, this, "databasesList");
		BindUtils.postNotifyChange(null, null, this, "configDatabasesList");
	}
	
	public ListModelList<CMEntity> getEntity() {
		entityFacade = new InstancesFacade();
		entitiesList = entityFacade.getAllEntities(filterRequest);
		if (entitiesList == null) {
			entitiesModel = new ListModelList<>();
			entitiesModelAbove2005 = new ListModelList<>();
		} else {
			entitiesModel = new ListModelList<>(entitiesList);
			entitiesModelAbove2005 = new ListModelList<>();
			entitiesModel.setMultiple(true);
			Instance allInstance = new Instance();
			allInstance.setInstanceName("<ALL>");
			entitiesModelAbove2005.add(allInstance);
			entitiesModelDatabases = new ListModelList<>();
			entitiesModelDatabases.add("<ALL>");
			for(CMEntity item : entitiesList) {
				if(item.getClass().equals(Instance.class)) {
					String sqlVersion = (((Instance)item).getSqlServerVersionEdition());
					if(sqlVersion != null && !sqlVersion.isEmpty() && !("Microsoft SQL Server 2000".equals(((Instance)item).getSqlServerVersionEdition())
							|| "Microsoft SQL Server 2005".equals(((Instance)item).getSqlServerVersionEdition()))) {
						entitiesModelAbove2005.add(item);
					}
				}
			}
			selectInstanceSource();
			entitiesModelAbove2005.setMultiple(true);
			entitiesModelDatabases.setMultiple(true);
		}
		BindUtils.postNotifyChange(null, null, this, "entitiesModel");
		return entitiesModel;
	}

	public ListModelList<CMEntity> getEntitiesModel() {
		return entitiesModel;
	}

	public void setEntitiesModel(ListModelList<CMEntity> entitiesModel) {
		this.entitiesModel = entitiesModel;
	}
	
	public ListModelList<CMEntity> getEntitiesModelAbove2005() {
		return entitiesModelAbove2005;
	}

	public void setEntitiesModelAbove2005(
			ListModelList<CMEntity> entitiesModelAbove2005) {
		this.entitiesModelAbove2005 = entitiesModelAbove2005;
	}


	
	public Boolean getShowSqlText() {
		if (showSqlTextInt == 0) {
			showSqlText = true;
		} else {
			showSqlText = false;
		}
		return showSqlText;
	}

	public void setShowSqlText(Boolean showSqlText) {
		this.showSqlText = showSqlText;
	}

	public int getShowSqlTextInt() {
		return showSqlTextInt;
	}

	public void setShowSqlTextInt(int showSqlTextInt) {
		this.showSqlTextInt = showSqlTextInt;
	}

	public ListModelList<CMEntity> getEntitiesModelSelect() {
		return entitiesModelSelect;
	}

	public void setEntitiesModelSelect(
			ListModelList<CMEntity> entitiesModelSelect) {
		this.entitiesModelSelect = entitiesModelSelect;
	}

	public int getEventDatabase() {
		return eventDatabase;
	}

	public void setEventDatabase(int eventDatabase) {
		this.eventDatabase = eventDatabase;
	}

	public String getCategory() {
		return category;
	}

	public void setCategory(String category) {
		this.category = category;
	}

	public String getDatabase() {
		return database;
	}

	public void setDatabase(String database) {
		this.database = database;
	}
	public String getSelectedConfigDatabase() {
		return selectedConfigDatabase;
	}

	public void setSelectedConfigDatabase(String database) {
		this.selectedConfigDatabase = database;
	}
	

	public String getApplication() {
		return application;
	}

	public void setApplication(String application) {
		this.application = application;
	}

	public Boolean getPrivilegedUser() {
		if (privilegedUserInt == 0) {
			privilegedUser = true;
		} else {
			privilegedUser = false;
		}
		return privilegedUser;
	}

	public void setPrivilegedUser(Boolean privilegedUser) {
		this.privilegedUser = privilegedUser;
	}

	public Date getToDate() {
		return toDate;
	}

	public void setToDate(Date toDate) {
		this.toDate = toDate;
	}

	public Date getFromDate() {
		return fromDate;
	}

	public void setFromDate(Date fromDate) {
		this.fromDate = fromDate;
	}

	public String getLogin() {
		return login;
	}

	public void setLogin(String login) {
		this.login = login;
	}

	public String getObject() {
		return target;
	}

	public String setObject(String target) {
		return this.target = target;
	}

	public String getColumn() {
		return column;
	}

	public String setColumn(String column) {
		return this.column = column;
	}

	public String getPrivilegedUsers() {
		return privilegedUsers;
	}

	public String setPrivilegedUsers(String privilegedUsers) {
		return this.privilegedUsers = privilegedUsers;
	}

	public int getRowCountThreshold() {
		return rowCountThreshold;
	}

	public int setRowCountThreshold(int rowCountThreshold) {
		return this.rowCountThreshold = rowCountThreshold;
	}

	public String getPrimaryKey() {
		return primaryKey;
	}

	public void setPrimaryKey(String primaryKey) {
		this.primaryKey = primaryKey;
	}

	public String getTargetObject() {
		return targetObject;
	}

	public void setTargetObject(String targetObject) {
		this.targetObject = targetObject;
	}

	public int getPrivilegedUserInt() {
		return privilegedUserInt;
	}

	public void setPrivilegedUserInt(int privilegedUserInt) {
		this.privilegedUserInt = privilegedUserInt;
	}

	String sortColumn = "date";
	int rowCount = 10000;

	private ListModelList<ConfigurationServerCheckEvents> conditionConfigurationEvents;
	@Command("auditApplication")
	public void auditApplication() throws ParseException {
		Instance cmEntityObj = (Instance) entitiesList.get(eventDatabase);
		String eventInstance = "SQLcompliance_"+cmEntityObj.getInstanceName();
		cmAuditApplication = new CMAuditApplication();
		cmAuditApplication.setApplication(application);
		cmAuditApplication.setCategory(category);
		cmAuditApplication.setCategoryNames(category);
		cmAuditApplication.setDatabase(database);
		cmAuditApplication.setFrom(fromDate);
		cmAuditApplication.setInstance(eventInstance);
		cmAuditApplication.setSQL(showSqlTextInt);
		cmAuditApplication.setTo(toDate);
		cmAuditApplication.setUser(privilegedUserInt);
		cmAuditApplication.setSortColumn(sortColumn);
		cmAuditApplication.setRowCount(rowCount);
		if(application==null||database==null||fromDate==null||toDate==null||category==null){
			WebUtil.showReportWarningBox("Please fill all the fields");	
		}
		else{
			try {
			CMAuditListApplicationResponse  cmAuditListApplicationResponse = new CMAuditListApplicationResponse();
			cmAuditListApplicationResponse = reportsFacade.getAuditApplicationReport(cmAuditApplication);
			conditionEvents = (List) cmAuditListApplicationResponse.getAuditApplication();
			int conditionEventsCount = conditionEvents.size();
			for(int i = 0; i<conditionEventsCount; i++) 
			{
				String appName = conditionEvents.get(i).getApplicationName();				
				String loginName = conditionEvents.get(i).getLoginName();
				String host = conditionEvents.get(i).getHostName();
				String dataBase = conditionEvents.get(i).getDatabaseName();
				String eventType = conditionEvents.get(i).getEventType();
				String targetObject = conditionEvents.get(i).getTargetObject();
				String detail = conditionEvents.get(i).getDetails();
				String startTime = conditionEvents.get(i).getStartTime();
				String sqltext = conditionEvents.get(i).getSqlText();
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_REPORT);
		}
			
		BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
		setGridRowsCount();
		}
	}

	@Command("auditConfiguration")
	public void auditConfiguration() throws ParseException, UnsupportedEncodingException {
		
		cmAuditConfiguration = new CMAuditConfiguration();
		selectedConfigDatabase = configDatabasesList.get(selectedDatabase);
		selectedConfigServer = configServersList.get(selectedServer);
		if(selectedConfigDatabase.equalsIgnoreCase("All")) { 
			selectedConfigDatabase="All";	
		}
		if(selectedConfigServer.equalsIgnoreCase("All")) {
			selectedConfigServer="All";	
		}
		
		cmAuditConfiguration.setDatabase(selectedConfigDatabase); 
		cmAuditConfiguration.setInstance(selectedConfigServer); 
		cmAuditConfiguration.setSetting(auditSettings);
		cmAuditConfiguration.setDefaultStatus(defaultStatus); 
		
		Logger logger = Logger.getLogger(AuditReportGridViewModel.class);
		logger.info("[Configuration report] server: " + selectedConfigServer + " database: " + selectedConfigDatabase + " auditSetting: " + auditSettings
				+ " defaultStatus: " + defaultStatus );
			try {
				CMAuditListConfigurationResponse cmAuditListConfigurationResponse = new CMAuditListConfigurationResponse();
				cmAuditListConfigurationResponse = reportsFacade.getAuditConfigurationReport(cmAuditConfiguration);
				// logger.info("(DEBUG)" + cmAuditListConfigurationResponse.getAuditConfiguration().);
				ConfigurationCheckReportModel configRep = new ConfigurationCheckReportModel();
				conditionConfigurationEvents = configRep.createReportModel(auditSettings, defaultStatus, cmAuditListConfigurationResponse, ideraDefaultValues, ideraDefaultDatabaseValues);
						
			} catch (RestException e) {
				WebUtil.showErrorBox(e,
						SQLCMI18NStrings.FAILED_TO_LOAD_REPORT);
			}

			BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
	
	}

	@Command("auditDML")
	public void auditDML() throws ParseException {
		Instance cmEntityObj = (Instance) entitiesList.get(eventDatabase);
		String eventInstance = "SQLcompliance_"+cmEntityObj.getInstanceName();
		if(targetObject!=null){		
		if(targetObject.contains(".")){
			String value[]=targetObject.split(".");
			schemaName=value[0];
			target=value[1];
		}		
		else{
			target=targetObject;
			schemaName="*";
		}
		}
		cmAuditDML = new CMAuditDML();
		cmAuditDML.setDatabase(database);
		cmAuditDML.setInstance(eventInstance);
		cmAuditDML.setLoginName(login);
		cmAuditDML.setObjectName(target);
		cmAuditDML.setSchemaName(schemaName);
		cmAuditDML.setTo(toDate);
		cmAuditDML.setFrom(fromDate);
		cmAuditDML.setKey(primaryKey);
		cmAuditDML.setSortColumn(sortColumn);
		cmAuditDML.setRowCount(rowCount);
		if(database==null||eventInstance==null||login==null||targetObject==null||target==null||schemaName==null||primaryKey==null||toDate==null||fromDate==null){
			WebUtil.showReportWarningBox("Please fill all the fields");
		}
		else{
		try {
			CMAuditListDMLRespose  cmAuditListDMLResponse = new CMAuditListDMLRespose();
			cmAuditListDMLResponse = reportsFacade.getAuditDMLActivityReport(cmAuditDML);
			conditionEventsDML = (List) cmAuditListDMLResponse.getDMLResponse();
			int conditionEventsCount = conditionEventsDML.size();
			for(int i = 0; i<conditionEventsCount; i++)
			{
			
				String eventType = conditionEventsDML.get(i).getEventType();				
				String startTime = conditionEventsDML.get(i).getStartTime();
				String loginName = conditionEventsDML.get(i).getLoginName();
				String dataBase = conditionEventsDML.get(i).getDatabaseName();
				String column = conditionEventsDML.get(i).getColumnName();
				String table = conditionEventsDML.get(i).getTable();
				String beforeValue = conditionEventsDML.get(i).getBeforeValue();
				String afterValue = conditionEventsDML.get(i).getAfterValue();
				String key = conditionEventsDML.get(i).getPrimaryKeys();
				Data data = new Data(eventType, startTime, loginName, dataBase, column, beforeValue, afterValue, key);
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_REPORT);
		}
		BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
		setGridRowsCount();
	}
	}

	@Command("auditLoginCreation")
	public void auditLoginCreation() throws ParseException {
		Instance cmEntityObj = (Instance) entitiesList.get(eventDatabase);
		String eventInstance = "SQLcompliance_"+cmEntityObj.getInstanceName();
		cmAuditLoginCreation = new CMAuditLoginCreation();
		cmAuditLoginCreation.setInstance(eventInstance);
		cmAuditLoginCreation.setLogin(login);
		cmAuditLoginCreation.setTo(toDate);
		cmAuditLoginCreation.setFrom(fromDate);
		cmAuditLoginCreation.setSortColumn(sortColumn);
		cmAuditLoginCreation.setRowCount(rowCount);
		if(eventInstance==null||login==null||toDate==null||fromDate==null){
			WebUtil.showReportWarningBox("Please fill all the fields");
		}
		else{
		try {
			CMAuditListLoginCreationResponse  cmAuditListApplicationResponse = new CMAuditListLoginCreationResponse();
			cmAuditListApplicationResponse = reportsFacade.getAuditLoginCreationReport(cmAuditLoginCreation);
			conditionEventsLoginCreation = (List) cmAuditListApplicationResponse.getLoginCreationResponse();
			int conditionEventsCount = conditionEventsLoginCreation.size();
			for(int i = 0; i<conditionEventsCount; i++)
			{
				
				String appName = conditionEventsLoginCreation.get(i).getApplicationName();				
				String loginName = conditionEventsLoginCreation.get(i).getLoginName();
				String host = conditionEventsLoginCreation.get(i).getHostName();
				String targetLoginName = conditionEventsLoginCreation.get(i).getTargetLoginName();
				String startTime = conditionEventsLoginCreation.get(i).getStartTime();
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_REPORT);
		}
		BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
		setGridRowsCount();
	}
	}

	@Command("auditLoginDeletion")
	public void auditLoginDeletion() throws ParseException {
		Instance cmEntityObj = (Instance) entitiesList.get(eventDatabase);
		String eventInstance = "SQLcompliance_"+cmEntityObj.getInstanceName();
		cmAuditLoginDeletion = new CMAuditLoginDeletion();
		cmAuditLoginDeletion.setInstance(eventInstance);
		cmAuditLoginDeletion.setLogin(login);
		cmAuditLoginDeletion.setTo(toDate);
		cmAuditLoginDeletion.setFrom(fromDate);
		cmAuditLoginDeletion.setSortColumn(sortColumn);
		cmAuditLoginDeletion.setRowCount(rowCount);
		if(eventInstance==null||login==null||toDate==null||fromDate==null){
			WebUtil.showReportWarningBox("Please fill all the fields");
		}		
		else{
		try {
			CMAuditListLoginDeletionResponse  cmAuditListLoginDeletionResponse = new CMAuditListLoginDeletionResponse();
			cmAuditListLoginDeletionResponse = reportsFacade.getAuditLoginDeletionReport(cmAuditLoginDeletion);
			conditionEventsLoginDeletion = (List) cmAuditListLoginDeletionResponse.getLoginDeletionResponse();
			int conditionEventsCount = conditionEventsLoginDeletion.size();
			for(int i = 0; i<conditionEventsCount; i++)
			{
				String appName = conditionEventsLoginDeletion.get(i).getApplicationName();				
				String loginName = conditionEventsLoginDeletion.get(i).getLoginName();
				String host = conditionEventsLoginDeletion.get(i).getHostName();
				String targetLoginName = conditionEventsLoginDeletion.get(i).getTargetLoginName();
				String startTime = conditionEventsLoginDeletion.get(i).getStartTime();
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_REPORT);
		}
		BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
		setGridRowsCount();
	}
	}

	@Command("auditObjectActivity")
	public void auditObjectActivity() throws ParseException {
		Instance cmEntityObj = (Instance) entitiesList.get(eventDatabase);
		String eventInstance = "SQLcompliance_"+cmEntityObj.getInstanceName();		
		cmAuditObjectActivity = new CMAuditObjectActivity();
		cmAuditObjectActivity.setDatabase(database);
		cmAuditObjectActivity.setLoginName(login);
		cmAuditObjectActivity.setInstance(eventInstance);
		cmAuditObjectActivity.setObjectName(targetObject);
		cmAuditObjectActivity.setCategory(category);
		cmAuditObjectActivity.setCategoryNames(category);
		cmAuditObjectActivity.setTo(toDate);
		cmAuditObjectActivity.setFrom(fromDate);
		cmAuditObjectActivity.setSql(showSqlTextInt);
		cmAuditObjectActivity.setUser(privilegedUserInt);
		cmAuditObjectActivity.setSortColumn(sortColumn);
		cmAuditObjectActivity.setRowCount(rowCount);
		if(eventInstance==null||targetObject==null||toDate==null||fromDate==null||login==null||category==null){
			WebUtil.showReportWarningBox("Please fill all the fields");
		}		
		else{
		try {
			CMAuditListObjectActivityResponse  cmAuditListObjectActivityResponse = new CMAuditListObjectActivityResponse();
			cmAuditListObjectActivityResponse = reportsFacade.getAuditObjectActivityReport(cmAuditObjectActivity);
			conditionEventsObjectActivity = (List) cmAuditListObjectActivityResponse.getObjectActivityResponse();
			int conditionEventsCount = conditionEventsObjectActivity.size();
			for(int i = 0; i<conditionEventsCount; i++)
			{				
				String appName = conditionEventsObjectActivity.get(i).getApplicationName();
				String loginName = conditionEventsObjectActivity.get(i).getLoginName();
				String host = conditionEventsObjectActivity.get(i).getHostName();
				String dataBase = conditionEventsObjectActivity.get(i).getDatabaseName();
				String eventType = conditionEventsObjectActivity.get(i).getEventType();
				String targetObject = conditionEventsObjectActivity.get(i).getTargetObject();
				String sqlText = conditionEventsObjectActivity.get(i).getSqlText();
				String startTime = conditionEventsObjectActivity.get(i).getStartTime();
				String detail = conditionEventsObjectActivity.get(i).getDetail();
				String sqltext = conditionEventsObjectActivity.get(i).getSqlText();
				Data data = new Data(appName, host, loginName, dataBase, eventType, targetObject, sqlText, startTime);
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_REPORT);
		}
		BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
		setGridRowsCount();
	}
	}

	@Command("auditPermissionDenied")
	public void auditPermissionDenied() throws ParseException {
		Instance cmEntityObj = (Instance) entitiesList.get(eventDatabase);
		String eventInstance = "SQLcompliance_"+cmEntityObj.getInstanceName();		
		cmAuditPermissionDenied = new CMAuditPermissionDenied();
		cmAuditPermissionDenied.setDatabase(database);
		cmAuditPermissionDenied.setInstance(eventInstance);
		cmAuditPermissionDenied.setLoginName(login);
		cmAuditPermissionDenied.setCategory(category);
		cmAuditPermissionDenied.setCategoryNames(category);
		cmAuditPermissionDenied.setTo(toDate);
		cmAuditPermissionDenied.setFrom(fromDate);
		cmAuditPermissionDenied.setSql(showSqlTextInt);
		cmAuditPermissionDenied.setUser(privilegedUserInt);
		cmAuditPermissionDenied.setSortColumn(sortColumn);
		cmAuditPermissionDenied.setRowCount(rowCount);
		if(eventInstance==null||login==null||toDate==null||fromDate==null||category==null|| category==null){
			WebUtil.showReportWarningBox("Please fill all the fields");
		}		
		else{
		try {
			CMAuditListPermissionDeniedActivityResponse  cmAuditListPermissionDeniedActivityResponse = new CMAuditListPermissionDeniedActivityResponse();
			cmAuditListPermissionDeniedActivityResponse = reportsFacade.getAuditPermissionDeniedReport(cmAuditPermissionDenied);
			conditionEventsPermissionDeniedActivity = (List) cmAuditListPermissionDeniedActivityResponse.getPermissionDeniedResponse();
			int conditionEventsCount = conditionEventsPermissionDeniedActivity.size();
			for(int i = 0; i<conditionEventsCount; i++)
			{
				
				String appName = conditionEventsPermissionDeniedActivity.get(i).getApplicationName();				
				String loginName = conditionEventsPermissionDeniedActivity.get(i).getLoginName();
				String host = conditionEventsPermissionDeniedActivity.get(i).getHostName();
				String dataBase = conditionEventsPermissionDeniedActivity.get(i).getDatabaseName();
				String eventType = conditionEventsPermissionDeniedActivity.get(i).getEventType();
				String targetObject = conditionEventsPermissionDeniedActivity.get(i).getTargetObject();
				String detail = conditionEventsPermissionDeniedActivity.get(i).getDetails();
				String startTime = conditionEventsPermissionDeniedActivity.get(i).getStartTime();
				String sqltext = conditionEventsPermissionDeniedActivity.get(i).getSqlText();
				Data data = new Data(appName, host, loginName, dataBase, eventType, targetObject, detail, startTime);
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_REPORT);
		}
		BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
		setGridRowsCount();
	}
	}

	@Command("auditUserActivity")
	public void auditUserActivity() throws ParseException {
		Instance cmEntityObj = (Instance) entitiesList.get(eventDatabase);
		String eventInstance = "SQLcompliance_"+cmEntityObj.getInstanceName();		
		cmAuditUserActivity = new CMAuditUserActivity();
		cmAuditUserActivity.setDatabase(database);
		cmAuditUserActivity.setInstance(eventInstance);
		cmAuditUserActivity.setLoginName(login);
		cmAuditUserActivity.setCategory(category);
		cmAuditUserActivity.setCategoryNames(category);
		cmAuditUserActivity.setTo(toDate);
		cmAuditUserActivity.setFrom(fromDate);
		cmAuditUserActivity.setSql(showSqlTextInt);
		cmAuditUserActivity.setUser(privilegedUserInt);
		cmAuditUserActivity.setSortColumn(sortColumn);
		cmAuditUserActivity.setRowCount(rowCount);
		if(eventInstance==null||login==null||toDate==null||fromDate==null||category==null){
			WebUtil.showReportWarningBox("Please fill all the fields");
		}
		else{
		try {
			CMAuditListUserActivityResponse  cmAuditListUserActivityResponse = new CMAuditListUserActivityResponse();
			cmAuditListUserActivityResponse = reportsFacade.getAuditUserActivityReport(cmAuditUserActivity);
			conditionEventsUserActivity = (List) cmAuditListUserActivityResponse.getUserActivityResponse();
			int conditionEventsCount = conditionEventsUserActivity.size();
			for(int i = 0; i<conditionEventsCount; i++)
			{
				
				String appName = conditionEventsUserActivity.get(i).getApplicationName();				
				String loginName = conditionEventsUserActivity.get(i).getLoginName();
				String host = conditionEventsUserActivity.get(i).getHostName();
				String dataBase = conditionEventsUserActivity.get(i).getDatabaseName();
				String eventType = conditionEventsUserActivity.get(i).getEventType();
				String targetObject = conditionEventsUserActivity.get(i).getTargetObject();
				String detail = conditionEventsUserActivity.get(i).getDetails();
				String startTime = conditionEventsUserActivity.get(i).getStartTime();
				String sqltext = conditionEventsUserActivity.get(i).getSqlText();
				Data data = new Data(appName, host, loginName, dataBase, eventType, targetObject, detail, startTime);
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_REPORT);
		}
		BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
		setGridRowsCount();
	}	
	}
	
	@Command("auditRowCount")
	public void auditRowCount() throws ParseException {
		Instance cmEntityObj = (Instance) entitiesModelAbove2005.get(eventDatabase);
		String eventInstance;
		if(("<ALL>").equalsIgnoreCase(cmEntityObj.getInstanceName())) {
			eventInstance = cmEntityObj.getInstanceName();
		} else {
			eventInstance = "SQLcompliance_"+ cmEntityObj.getInstanceName();
		}
		cmAuditRowCount = new CMAuditRowCount();
		if (entitiesModelDatabases.get(eventDb).equalsIgnoreCase("<All>"))
		{
			cmAuditRowCount.setDatabase("*");
		}
		else
		{
			cmAuditRowCount.setDatabase(entitiesModelDatabases.get(eventDb));
		}
		cmAuditRowCount.setObjectName(targetObject);
		cmAuditRowCount.setInstance(eventInstance);
		cmAuditRowCount.setLoginName(login);
		cmAuditRowCount.setColumn(column);
		cmAuditRowCount.setPrivilegedUsers(privilegedUsers);
		cmAuditRowCount.setTo(toDate);
		cmAuditRowCount.setFrom(fromDate);
		cmAuditRowCount.setRowCountThreshold(rowCountThreshold);
		cmAuditRowCount.setSql(showSqlTextInt);
		if(eventInstance==null||login==null||toDate==null||fromDate==null||privilegedUsers==null||column==null||target==null){
			WebUtil.showReportWarningBox("Please fill all the fields");
		}
		else{
		try {
			CMAuditListRowCountResponse  cmAuditListRowCountResponse = new CMAuditListRowCountResponse();
			cmAuditListRowCountResponse = reportsFacade.getAuditRowCountReport(cmAuditRowCount);
			conditionEventsRowCount = (List) cmAuditListRowCountResponse.getRowCountResponse();
			int conditionEventsCount = conditionEventsRowCount.size();
			for(int i = 0; i<conditionEventsCount; i++)
			{
				String serverName = conditionEventsRowCount.get(i).getServerName();	
				String appName = conditionEventsRowCount.get(i).getApplicationName();				
				String loginName = conditionEventsRowCount.get(i).getLoginName();
				String role = conditionEventsRowCount.get(i).getRoleName();
				String dataBase = conditionEventsRowCount.get(i).getDatabaseName();
				String eventType = conditionEventsRowCount.get(i).getEventType();
				String targetObject = conditionEventsRowCount.get(i).getTargetObject();
				String spid = conditionEventsRowCount.get(i).getSpid();
				String column = conditionEventsRowCount.get(i).getColumnName();
				String startTime = conditionEventsRowCount.get(i).getStartTime();
				String sqlText = conditionEventsRowCount.get(i).getSqlText();
				String rowCounts = conditionEventsRowCount.get(i).getRowCounts();
				Data data = new Data(appName, role, loginName, dataBase, eventType, targetObject, spid, startTime);
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_REPORT);
		}
		BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
		setGridRowsCount();
	}	
	}
	
	@Command("auditRegulatoryCompliance")
	public void auditRegulatoryCompliance() throws ParseException {
		
		cmAuditRegulatoryCompliance = new CMAuditRegulatoryCompliance();
		String server, database;
		int auditSetting, regulatoryguideline, value;
		server = serversList.get(selectedServer);
		database = databasesList.get(selectedDatabase);
		auditSetting = selectedAuditSetting;
		regulatoryguideline = selectedRegulatoryGuideline;
		value = selectedValue;
		Logger logger = Logger.getLogger(SQLCMRestClient.class);
		logger.info("server: " + server + " database: " + database + " auditSetting: " + auditSetting + " regulatoryGuideline: " + regulatoryguideline + " value: " + value);
		cmAuditRegulatoryCompliance.setServerName(server);
		cmAuditRegulatoryCompliance.setDatabaseName(database);
		cmAuditRegulatoryCompliance.setAuditSettings(auditSetting);
		cmAuditRegulatoryCompliance.setRegulationGuidelines(regulatoryguideline);
		cmAuditRegulatoryCompliance.setValues(value);
		
		try {
			CMAuditListRegulatoryComplianceResponse  cmAuditListRegulatoryComplianceResponse = new CMAuditListRegulatoryComplianceResponse();
			cmAuditListRegulatoryComplianceResponse = reportsFacade.getRegulatoryComplianceReport(cmAuditRegulatoryCompliance);
			conditionEventsRegulatoryCompliance = (List) cmAuditListRegulatoryComplianceResponse.getRegulatoryComplianceResponse();
		} catch (RestException e) {
			WebUtil.showErrorBox(e, 
					SQLCMI18NStrings.FAILED_TO_LOAD_REPORT);
		}
		BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
		setGridRowsCount();
	}
	
	public List<CMAuditApplicationResponse> getConditionEvents() {
		return conditionEvents;
	}
	public ListModelList<ConfigurationServerCheckEvents> getConditionConfigurationEvents() {
		return conditionConfigurationEvents;
	}
	
	public List<CMAuditDMLResponse> getConditionEventsDML() {
		return conditionEventsDML;
	}

	public List<CMAuditLoginCreationResponse> getConditionEventsLoginCreation() {
		return conditionEventsLoginCreation;
	}

	public List<CMAuditLoginDeletionResponse> getConditionEventsLoginDeletion() {
		return conditionEventsLoginDeletion;
	}

	public List<CMAuditObjectActivityResponse> getConditionEventsObjectActivity() {
		return conditionEventsObjectActivity;
	}

	public List<CMAuditPermissionDeniedActivityResponse> getConditionEventsPermissionDeniedActivity() {
		return conditionEventsPermissionDeniedActivity;
	}

	public List<CMAuditUserActivityResponse> getConditionEventsUserActivity() {
		return conditionEventsUserActivity;
	}

	public List<CMAuditRowCountResponse> getConditionEventsRowCount() {
		return conditionEventsRowCount;
	}
	
	public List<CMAuditRegulatoryComplianceResponse> getConditionEventsRegulatoryCompliance() {
		return conditionEventsRegulatoryCompliance;
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
		
	public class Data {
		String ApplicationName;
		String Host;
		String LoginName;
		String DataBase;
		String EventType;
		String TargetObject;
		String Detail;
		String StartTime;
		
		public String getApplicationName() {
			return ApplicationName;
		}

		public void setApplicationName(String applicationName) {
			ApplicationName = applicationName;
		}

		public String getLoginName() {
			return LoginName;
		}

		public void setLoginName(String loginName) {
			LoginName = loginName;
		}		
		
		public String getHost() {
			return Host;
		}

		public void setHost(String host) {
			Host = host;
		}

		public String getDataBase() {
			return DataBase;
		}

		public void setDataBase(String dataBase) {
			DataBase = dataBase;
		}

		public String getEventType() {
			return EventType;
		}

		public void setEventType(String eventType) {
			EventType = eventType;
		}

		public String getTargetObject() {
			return TargetObject;
		}

		public void setTargetObject(String targetObject) {
			TargetObject = targetObject;
		}

		public String getDetail() {
			return Detail;
		}

		public void setDetail(String detail) {
			Detail = detail;
		}
		
		public String getStartTime() {
			return StartTime;
		}

		public void setStartTime(String startTime) {
			StartTime = startTime;
		}

		public Data(String applicationName, String host, String loginName, String dataBase, String eventType, String targetObject, String detail, String startTime) {
			super();
			this.ApplicationName = applicationName;
			this.Host = host;
			this.LoginName = loginName;
			this.DataBase = dataBase;
			this.EventType = eventType;
			this.TargetObject = targetObject;
			this.Detail = detail;
			this.StartTime = startTime;
		}
	}
	
	@Command("setGridRowsCount")
    public void setGridRowsCount() {
        try {
            int pageSize = listBoxRowsBox.getValue();
            if (pageSize > 100) {
                Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
                        listBoxRowsBox, "end_center", 3000);
                pageSize = 100;
                listBoxRowsBox.setValue(pageSize);
            }
            else if (pageSize<=0) {
                Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
                        listBoxRowsBox, "end_center", 3000);
                pageSize = 1;
                listBoxRowsBox.setValue(pageSize);
            }
            listBoxPageId.setPageSize(pageSize);
            prevPageSize = pageSize;
            int value=0;
            if(conditionEvents !=null && !conditionEvents.isEmpty())
            {
            	
            	
            		value=conditionEvents.size();
            		setFileSize(value);
            	
            }
        	else if(conditionConfigurationEvents!=null)
			{

				value=conditionConfigurationEvents .size();
				setFileSize(value);


			}
            else if(conditionEventsDML!=null)
            {
            	
            		value=conditionEventsDML.size();
            		setFileSize(value);
            	
            	
            }
            else if(conditionEventsLoginCreation !=null)
            {
            	
            		value=conditionEventsLoginCreation.size();
            		setFileSize(value);
            	
            }
            else if(conditionEventsLoginDeletion!=null)
            {
            	value=conditionEventsLoginDeletion.size();
            		setFileSize(value);
            	
            }
            else if(conditionEventsObjectActivity!=null)
            {
            	
            		value=conditionEventsObjectActivity.size();
            		setFileSize(value);
            	
            }
            else if(conditionEventsPermissionDeniedActivity!=null)
            {
            	
            		value=conditionEventsPermissionDeniedActivity.size();
            		setFileSize(value);
            	
            }
            else if(conditionEventsUserActivity!=null)
            {
            	
            		value=conditionEventsUserActivity.size();
            		setFileSize(value);
            	
            }
            else if(conditionEventsRowCount!=null)
            {
            	
            		value=conditionEventsRowCount.size();
            		setFileSize(value);
            	
            }
            else if(conditionEventsRegulatoryCompliance!=null) 
            {
            	value = conditionEventsRegulatoryCompliance.size();
            	setFileSize(value);
            }
            else
            {
            	setFileSize(0);
            }

        } catch (WrongValueException exp) {
            listBoxPageId.setPageSize(prevPageSize);
        }
        PreferencesUtil.getInstance().setGridPagingPreferencesInSession(preferencesSessionVariableName, listBoxPageId.getPageSize());
        BindUtils.postNotifyChange(null, null, AuditReportGridViewModel.this, "*");
        
    }
	
	
	
	
	@AfterCompose
	public void afterComposeReport(@ContextParam(ContextType.VIEW) Component view) {
		super.afterComposeReport(view);	
		listBoxRowsBox.setValue(PAGE_SIZE);
		try{			//SQLCM 4.5 SCM -9 Start
		String refreshDuration= RefreshDurationFacade.getRefreshDuration();
		int refDuration=Integer.parseInt(refreshDuration);
		refDuration=refDuration*1000;
		setRefreshDuration(refDuration); //SQLCM 4.5 SCM -9
		  //SQLCM 4.5 SCM -9 end
		}
		catch(Exception e)
		{
			e.getStackTrace();
		}
		
	}
}
