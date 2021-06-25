package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.RegulationType;
import com.idera.sqlcm.facade.CMTreeFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.KeyValueParser;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.RulesCoreConstants;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.EventType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.steps.SelectEventFilterStepViewModel;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.AddDataAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.RegulationSettings;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps.RegulationGuidelineSensitiveColumnsStepViewModel.RootNodeData;
import com.idera.sqlcm.ui.dialogs.userTables.UserTablesForSensitiveColumnsViewModel;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.A;
import org.zkoss.zul.DefaultTreeModel;
import org.zkoss.zul.DefaultTreeNode;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.TreeNode;


public class SelectDataFilterStepViewModel extends AddWizardStepBase implements UserTablesForSensitiveColumnsViewModel.DialogListener{

    public static final String ZUL_PATH = "~./sqlcm/dialogs/adddataalertruleswizard/steps/select-data-filter-step.zul";

    @Wire
    private Radiogroup rgFlow;

    String[] strTarget;
    RegulationSettings rs = new RegulationSettings();
    EventCondition eventCondition = new EventCondition();
    private String regulationGuidelinesDesc;
    public static final String SQL_Server = "SQL Server";
    public static final String Database_Name = "Database Name";
    public static final String Table_Name = "Table Name";
    public static final String Column_Name = "Column";
    EventField eventfield = new EventField();
    private ListModelList<RegulationType> regulationTypeListModelList;
    KeyValueParser parser = new KeyValueParser();
    RulesCoreConstants coreConstants = new RulesCoreConstants();
    private DefaultTreeModel treeModel;
    private List<CMDatabase> listTreeModel;
    
    @Wire
    private Checkbox chkSQLServer;
    
    @Wire
    private Checkbox chkDatabase;
    
    @Wire
    private Checkbox chkTable;
    
    @Wire
    private Checkbox chkColumn;
    
    int fieldId;
    
    @Wire
    private A specifySQL;
    
    @Wire
    private A specifyDatabase;
   
    @Wire
    private A specifyTable;
    
    @Wire
    private A specifyColumn;
    private List<CMAlertRules> alertRules;
   	private List<CMAlertRulesCondition> conditionEvents;
   
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

	public A getSpecifyTable() {
		return specifyTable;
	}

	public void setSpecifyTable(A specifyTable) {
		this.specifyTable = specifyTable;
	}

	public A getSpecifyColumn() {
		return specifyColumn;
	}

	public void setSpecifyColumn(A specifyColumn) {
		this.specifyColumn = specifyColumn;
	}

    private Map<String,Long> checkedAlertRulesTypes = new HashMap<String,Long>();
    
    @Wire
    private Button addTablesButton;
       
    @Override
    protected void doOnShow(AddDataAlertRulesSaveEntity wizardSaveEntity) {
    	rs.setInstances("<ALL>");
    	rs.setDbName("<ALL>");
		rs.setTableNameData("<ALL>");
		rs.setColumnNameData("<ALL>");
		fieldId = (int) Sessions.getCurrent().getAttribute("FieldId");
		rs.setRuleValue(5000);
		
		if(Sessions.getCurrent().getAttribute("QueryType")!=null)
    	{
     		conditionEvents = (List<CMAlertRulesCondition>) Sessions.getCurrent().getAttribute("conditionEvents");
     		if(conditionEvents!=null && alertRules!=null);
     		{
     			initializer(conditionEvents);
     			BindUtils.postNotifyChange(null, null, SelectDataFilterStepViewModel.this, "*");
     		}
     	}
    }
    

    public void initializer(List<CMAlertRulesCondition> conditionEvents){
    	try {
    		if(Sessions.getCurrent().getAttribute("QueryTypeForColumn")!= null && (!Sessions.getCurrent().getAttribute("QueryTypeForColumn").toString().isEmpty())){
    			
    			if(Sessions.getCurrent().getAttribute("SQL server")!=null){
            		String strInstances = (String) Sessions.getCurrent().getAttribute("SQL server");
            		Sessions.getCurrent().removeAttribute("SQL server");
            		if(!strInstances.equals("<ALL>")){
            		chkSQLServer.setChecked(true);
            		specifySQL.setDisabled(false);
            		Sessions.getCurrent().setAttribute("serverName", strInstances);
            		 checkedAlertRulesTypes.put("SQL Server", 0L);
            		}
        		}
    			
    			Map<String,String>  DataConditionMap =  new HashMap<String,String>();
    			DataConditionMap = coreConstants.DataAlertConditionMap();
    			DataConditionMap = parser.ParseString(conditionEvents.get(0).getMatchString());
    			
    			for (Map.Entry<String, String> entry : DataConditionMap
    					.entrySet())
    			{
    				if (entry.getKey().equals("0"))
    				{
    					rs.setDbName(entry.getValue());
    					if(!rs.getDbName().equals("<ALL>")){
	    					chkDatabase.setChecked(true);
	    					Sessions.getCurrent().setAttribute("dbName",entry.getValue());
	    					specifyDatabase.setDisabled(false);
    					}
    				}
    				
    				if (entry.getKey().equals("1"))
    				{
    					rs.setTableNameData(entry.getValue());
    					if(!rs.getTableNameData().equals("<ALL>")){
    					specifyTable.setDisabled(false);
    					Sessions.getCurrent().setAttribute("tableName",entry.getValue());
    					chkTable.setChecked(true);
    					}
    				}
    				
    				if (entry.getKey().equals("2"))
    				{
    					rs.setColumnNameData(entry.getValue());	
    					if(!rs.getColumnNameData().equals("<ALL>")){
    					chkColumn.setChecked(true);
    					specifyColumn.setDisabled(false);
    					Sessions.getCurrent().setAttribute("columnName",entry.getValue());
    					}
    				}
    				
    				if (entry.getKey().equals("3"))
    				{
    					fieldId = Integer.parseInt(entry.getValue());
    				}
    				
    				
    				if (entry.getKey().equals("4"))
    				{
    					rs.setRuleValue(Integer.parseInt(entry.getValue()));
    				}
    				
    			}
    		} 
    		else
    		{
    			Sessions.getCurrent().removeAttribute("columnName");
                Sessions.getCurrent().removeAttribute("tableName");
          		Sessions.getCurrent().removeAttribute("dbName");
          		Sessions.getCurrent().removeAttribute("serverName");
    		}
    		
    		
    	} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
      }
    
    
    
    private List<? extends TreeNode<CMDatabase>> addDatabaseNodes(List<CMDatabase> databaseList) {
        List<DefaultTreeNode<CMDatabase>> nodes = new ArrayList<>(databaseList.size());
        for (CMDatabase cmDatabase: databaseList) {
            DefaultTreeNode<CMDatabase> node = new DefaultTreeNode<>(cmDatabase);
            nodes.add(node);
        }
        return nodes;
    }

    @Override
    public String getNextStepZul() {
        return SelectAdditionalDataFilterStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.SQL_DATA_ALERT_TYPE2_TIPS);
    }

    @Command("eventDataAlertRules")
    public void eventAlertRules(@BindingParam("id") long id) {
        SpecifySQLServerViewModel.showSpecifySQLServersDialog(id);
        /*showSpecifySQLServersDialog(id);*/
    }
    
    @Command("afterRenderGrid")
    public void afterRenderGrid() {
        // select first item of radio group
        if (rgFlow.getItemCount() > 0) {
            rgFlow.setSelectedIndex(0);
        }
    }
    
    public static class RootNodeData extends CMEntity {
        RootNodeData(CMInstance instance) {
            if (instance != null) {
                this.id = instance.getId();
                this.name = instance.getInstanceName();
            }
        }
    }
   
    @Override
	public void onCancel(AddDataAlertRulesSaveEntity wizardSaveEntity) {
    	if(Sessions.getCurrent().getAttribute("QueryType")!=null)
    	{
    		Sessions.getCurrent().removeAttribute("QueryType");
    		Sessions.getCurrent().removeAttribute("QueryTypeForColumn");
			Sessions.getCurrent().removeAttribute("columnName");
            Sessions.getCurrent().removeAttribute("tableName");
      		Sessions.getCurrent().removeAttribute("dbName");
      		Sessions.getCurrent().removeAttribute("serverName");
    	}
    	 String uri = "instancesAlertsRule";
         uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
         Executions.sendRedirect(uri);
	}
    
    @Command("onCheck")
    public void onCheck(@BindingParam("target") Checkbox target, @BindingParam("index") long index,@BindingParam("lbl") A lbl) {
        if (target.isChecked()) {
        	String chkName = target.getName();
            checkedAlertRulesTypes.put(chkName, index);
            lbl.setDisabled(false);
            
            if(chkName.equals("SQL Server")){
            	Sessions.getCurrent().getAttribute("serverName");
            	Sessions.getCurrent().removeAttribute("dbName");
            	Sessions.getCurrent().removeAttribute("columnName");
                Sessions.getCurrent().removeAttribute("tableName");
            }
            
            if(chkName.equals("Database Name")){
            	specifySQL.setDisabled(false);
            	chkSQLServer.setChecked(true);
            	Sessions.getCurrent().removeAttribute("dbName");
            	Sessions.getCurrent().removeAttribute("columnName");
                Sessions.getCurrent().removeAttribute("tableName");
            }
            
            if(chkName.equals("Table Name")){
            	specifySQL.setDisabled(false);
            	specifyDatabase.setDisabled(false);
                chkSQLServer.setChecked(true);
            	chkDatabase.setChecked(true);
            	chkTable.setChecked(true);
            	Sessions.getCurrent().removeAttribute("columnName");
                Sessions.getCurrent().removeAttribute("tableName");
            }
            
            if(chkName.equals("Column Name")){
            	specifySQL.setDisabled(false);
            	specifyDatabase.setDisabled(false);
            	specifyTable.setDisabled(false);
            	chkSQLServer.setChecked(true);
            	chkDatabase.setChecked(true);
            	chkTable.setChecked(true);
            	chkColumn.setChecked(true);
            	Sessions.getCurrent().removeAttribute("columnName");
            }
        } else {
        	String chkName = target.getName();
        	checkedAlertRulesTypes.remove(target.getName());
        	lbl.setDisabled(true);
        	if(chkName.equals("Table Name")){
        		chkColumn.setChecked(false);
        		specifyColumn.setDisabled(true);
        		Sessions.getCurrent().removeAttribute("columnName");
                Sessions.getCurrent().removeAttribute("tableName");
        	}
        	
        	if(chkName.equals("Database Name")){
        		chkTable.setChecked(false);
        		specifyTable.setDisabled(true);
        		chkColumn.setChecked(false);
        		specifyColumn.setDisabled(true);
        		Sessions.getCurrent().removeAttribute("dbName");
            	Sessions.getCurrent().removeAttribute("columnName");
                Sessions.getCurrent().removeAttribute("tableName");
        	}
        	
        	if(chkName.equals("SQL Server")){
        		chkDatabase.setChecked(false);
        		specifyDatabase.setDisabled(true);
        		chkTable.setChecked(false);
        		specifyTable.setDisabled(true);
        		chkColumn.setChecked(false);
        		specifyColumn.setDisabled(true);
        		Sessions.getCurrent().getAttribute("serverName");
            	Sessions.getCurrent().removeAttribute("dbName");
            	Sessions.getCurrent().removeAttribute("columnName");
                Sessions.getCurrent().removeAttribute("tableName");
        	}
        	if(chkName.equals("Column Name")){
        		Sessions.getCurrent().removeAttribute("columnName");
        	}
        }

       BindUtils.postNotifyChange(null, null, SelectDataFilterStepViewModel.this, "regulationGuidelinesDesc");
    }
    
    @Override
    public void onBeforeNext(AddDataAlertRulesSaveEntity wizardSaveEntity) {
    	String dbName = "";
    	String instanceName = "";
    	String tableName = "";
    	String columnName = "";
        if (chkSQLServer.isChecked()) {
        	rs.setDatabaseName(true);
        	rs.setInstances(instanceName);
        	if(Sessions.getCurrent().getAttribute("serverName")!=null 
        			&& !Sessions.getCurrent().getAttribute("serverName").toString().isEmpty())
        	{
        		instanceName = (String) Sessions.getCurrent().getAttribute("serverName");
        	}
        		rs.setInstances(instanceName);
    			rs.setDatabaseName(true);
    			rs.setSqlServer(true);        	
        }

        else{
        	rs.setInstances("<ALL>");
        	rs.setColumnName(false);
			rs.setTableName(false);
			rs.setDatabaseName(false);
			rs.setSqlServer(false);
        }
        
        if (chkDatabase.isChecked()) {        	
        	rs.setDatabaseName(true);
        	rs.setDbName(dbName);
        	if(Sessions.getCurrent().getAttribute("dbName")!=null 
        			&& !Sessions.getCurrent().getAttribute("dbName").toString().isEmpty())
        	{
        		 dbName = (String) Sessions.getCurrent().getAttribute("dbName");
        		 instanceName = (String) Sessions.getCurrent().getAttribute("serverName");
        	}
        		 rs.setDbName(dbName);
        		 rs.setInstances(instanceName);
     			 rs.setDatabaseName(true);
     			 rs.setSqlServer(true);
        
        }
        else{
        	rs.setDbName("<ALL>");
			rs.setDatabaseName(false);
			rs.setColumnName(false);
			rs.setTableName(false);
        }

        if (chkTable.isChecked()) {
        	rs.setTableName(true);
        	rs.setTableNameData(tableName);
        	if(Sessions.getCurrent().getAttribute("tableName")!=null 
        			&& !Sessions.getCurrent().getAttribute("tableName").toString().isEmpty())
        	{
        		 tableName = (String) Sessions.getCurrent().getAttribute("tableName");
        		 dbName = (String) Sessions.getCurrent().getAttribute("dbName");
        		 instanceName = (String) Sessions.getCurrent().getAttribute("serverName");
        	}
        		 rs.setTableNameData(tableName);
        		 rs.setDbName(dbName);
        		 rs.setInstances(instanceName);
        		 rs.setTableName(true);
     			rs.setDatabaseName(true);
     			rs.setSqlServer(true);        		 
        	
        }
        else{
        	rs.setTableNameData("<ALL>");
   		 	rs.setTableName(false);
			rs.setColumnName(false);
        }

        if (chkColumn.isChecked()) {
        	rs.setColumnName(true);
        	rs.setColumnNameData(columnName);
        	if(Sessions.getCurrent().getAttribute("columnName")!=null 
        			&& !Sessions.getCurrent().getAttribute("columnName").toString().isEmpty())
        	{
        		if(Sessions.getCurrent().getAttribute("columnName")!=null)
        		{
        			columnName = (String) Sessions.getCurrent().getAttribute("columnName");
        			if(columnName.equalsIgnoreCase("<ALL>")){
        				chkColumn.setChecked(false);
            			rs.setColumnName(false);
        			}
        		}
        	 }
        		rs.setColumnNameData(columnName);        			
        			rs.setTableName(true);
        			rs.setDatabaseName(true);
        			rs.setSqlServer(true);       		       		
        }
        
        else{
        	rs.setColumnNameData("<ALL>");
			rs.setColumnName(false);
        }
        
        try {
        	String strMatch = "";
        	strTarget = new String[5];
        	strTarget[0] = rs.getDbName();
        	strTarget[1] = rs.getTableNameData();
        	strTarget[2] = rs.getColumnNameData();
        	strTarget[3] = Integer.toString(fieldId);
        	strTarget[4] = Integer.toString(rs.getRuleValue());
        	for(int i=0; i<strTarget.length;i++)
        	{
        		strMatch += "(" +strTarget[i].length()+ ")"  ;
        		strMatch += strTarget[i];
        	}
			rs.setMatchString(strMatch);
			
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
        wizardSaveEntity.setRegulationSettings(rs);
    }
    

	@Override
	public void onCloseUserTablesForSensitiveColumnsDialog(CMInstance instance,
			CMDatabase database, List<CMTable> selectedTables) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public String getHelpUrl() {
		return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
	}
}
