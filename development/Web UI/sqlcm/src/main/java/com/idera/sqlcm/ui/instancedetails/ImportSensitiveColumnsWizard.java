//SQLCm-5.4 
//Requirement - 4.1.3.1. 
package com.idera.sqlcm.ui.instancedetails;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

import org.apache.derby.tools.dblook;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.util.media.Media;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.UploadEvent;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.DefaultTreeModel;
import org.zkoss.zul.DefaultTreeNode;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Tree;
import org.zkoss.zul.TreeNode;
import org.zkoss.zul.Treeitem;
import org.zkoss.zul.Window;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps.NewDataAlertRulesStepViewModel;
import com.idera.sqlcm.ui.importAuditSetting.AddImportAuditWizardViewModel;
public class ImportSensitiveColumnsWizard extends InstanceEventsViewModel{
	
	 public Map<String, String> MainEventData = new TreeMap<String, String>();
	 public AllSensitiveDetails retVal;
	 public String currentInstanceName;
	 ColumnDetails columnDetails = new ColumnDetails();
	 ListModelList <DatabaseDetails>  databaseList;
	 List<TableDetails> tableList = new ArrayList<TableDetails>();
	 List<ColumnDetails> columnList = new ArrayList<ColumnDetails>();
	 List <DatabaseDetails> dbList = new ArrayList<DatabaseDetails>();

	public ListModelList<DatabaseDetails> getDatabaseList() {
		return databaseList;
	}

	public void setDatabaseList(ListModelList<DatabaseDetails> databaseList) {
		this.databaseList = databaseList;
	}
	

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view){
		Selectors.wireComponents(view, this, false);
	}

	@Command("uploadFile")
		public void uploadFile(@ContextParam(ContextType.TRIGGER_EVENT) UploadEvent event,@BindingParam ("saveButton") Button saveButton,@BindingParam ("importedFile") Textbox importedFile)throws IOException {		 
		 	
		 	currentInstanceName = Sessions.getCurrent().getAttribute("currentInstanceName").toString();
		try {
			Media media = event.getMedia();
			String fileName = media.getName();
			importedFile.setText(fileName);
			String extension = fileName.substring(
					fileName.lastIndexOf(".") + 1, fileName.length());
			if (extension.equalsIgnoreCase("csv")) {
				String csvFileData = media.getStringData();
				InstancesFacade instanceFacade = new InstancesFacade();
				retVal = instanceFacade
						.validateSensitiveColumn(currentInstanceName + "*"
								+ csvFileData);
				if (retVal.isValidFile()) {
					saveButton.setDisabled(false);
		        	getTreeData();
				}
			} else {
				com.idera.sqlcm.server.web.WebUtil
						.showInfoBoxWithCustomMessage("Only csv files are allowed to import");
			}
		}
			catch(Exception ex)
			{
				com.idera.sqlcm.server.web.WebUtil.showInfoBoxWithCustomMessage("Please select appropriate file");
			}
	 }
	 		
	 @Command("OkayButton")
	 public void OkayButton(@BindingParam("comp") Window x) 
	 {
		try
		{
		 long instanceId = (long)Sessions.getCurrent().getAttribute("currentInstanceId");
		 List<TreeNode> dbnode= new ArrayList<TreeNode>();
			for (DatabaseDetails dbDetails : databaseList) {
				if (dbDetails.isSelected()) {
					dbDetails.setSrvId(instanceId);
					for (TableDetails tableDetail : dbDetails.getTableDetails()) {
						if (tableDetail.isSelected()) {
							tableList.add(tableDetail);
							for (ColumnDetails columnDetail : tableDetail
									.getColumList()) {
								if (columnDetail.isSelected())
									columnList.add(columnDetail);
							}
						}
					}
					dbList.add(dbDetails);
				}
			}
		 
		 retVal.setSensitiveDatabase(dbList);	
		 retVal.setSensitiveTable(tableList);
		 retVal.setSensitiveColumn(columnList);
		 InstancesFacade instanceFacade = new InstancesFacade();
		 instanceFacade.saveSensitiveColumnData(retVal);
		} catch(RestException e)
		{
			com.idera.sqlcm.server.web.WebUtil.showInfoBoxWithCustomMessage("Failed");
		}
		 x.detach();
		 
	 }
	 
	 @Command("expandDbCheck")
	 public void expandDbCheck(@BindingParam ("DbData") DatabaseDetails DbData){
		 if(DbData.isExpand()){
			 DbData.setExpand(false);
		 }
		 else{
			 DbData.setExpand(true);
		 }
		 BindUtils.postNotifyChange(null, null, this, "databaseList");
	 }
	 
	 @Command("expandTableCheck")
	 public void expandTableCheck(@BindingParam ("tableId") TableDetails tableId){
		 if(tableId.isExpand()){
			 tableId.setExpand(false);
		 }
		 else{
			 tableId.setExpand(true);
		 }
		 BindUtils.postNotifyChange(null, null, this, "databaseList");
	 }
	 
	 public void getTreeData()
	 {
		 List dbCheck = new ArrayList();
		 databaseList=new ListModelList<DatabaseDetails>();
		 for(DatabaseDetails databaseDetails : retVal.getSensitiveDatabase()){
			if (!dbCheck.contains(databaseDetails.getDbId())) {
				for (TableDetails tableDetails : retVal.getSensitiveTable()) {
					if (databaseDetails.getDbId() == tableDetails.getDbId()) {
						databaseDetails.getTableDetails().add(tableDetails);
						for (ColumnDetails columnDetails : retVal
								.getSensitiveColumn()) {
							if (tableDetails.getTblId() == columnDetails
									.getTblId()
									&& !tableDetails.getColumList().contains(
											columnDetails))
								tableDetails.getColumList().add(columnDetails);
						}
					}
				}

				databaseList.add(databaseDetails);
				dbCheck.add(databaseDetails.getDbId());
			}
		 }
		 BindUtils.postNotifyChange(null, null, this, "databaseList");
	 }
	 
	 @Command("onDbCheck")
	 public void onDbCheck(@BindingParam ("DbData") DatabaseDetails DbData,@BindingParam ("target") Checkbox target)
	 {
		 if(target.isChecked()){
			 for(int i = 0;i<DbData.getTableDetails().size();i++){
				 DbData.getTableDetails().get(i).setSelected(true);
				 for(int j= 0;j<DbData.getTableDetails().get(i).getColumList().size();j++){
					 DbData.getTableDetails().get(i).getColumList().get(j).setSelected(true);
				 }
			 }
		 }
		 else{
			 for(int i = 0;i<DbData.getTableDetails().size();i++){
				 DbData.getTableDetails().get(i).setSelected(false);
				 for(int j= 0;j<DbData.getTableDetails().get(i).getColumList().size();j++){
					 DbData.getTableDetails().get(i).getColumList().get(j).setSelected(false);
				 }
			 }
		 }
		 BindUtils.postNotifyChange(null, null, this, "databaseList");
	 }
	 
	 @Command("onTableCheck")
	 public void onTableCheck(@BindingParam ("tableData") TableDetails tableData,@BindingParam ("target") Checkbox target)
	 {
		 if(target.isChecked()){
			 for(ColumnDetails columnDetails : tableData.getColumList()){
				 columnDetails.setSelected(true);
			 }
			 for(DatabaseDetails dbDetails: databaseList){
				 if(dbDetails.getDbId() == tableData.getDbId()){
					 dbDetails.setSelected(true);
					 break;
				 }
			 }
		 }
		 else {
			for (ColumnDetails columnDetails : tableData.getColumList()) {
				columnDetails.setSelected(false);
				int flag = 0;
				for (DatabaseDetails dbDetails : databaseList) {
					if (dbDetails.getDbId() == tableData.getDbId()) {
						for (TableDetails tableDetail : dbDetails
								.getTableDetails()) {
							if (tableDetail.isSelected()) {
								dbDetails.setSelected(true);
								flag = 1;
								break;
							}
							if(flag == 0)
								dbDetails.setSelected(false);
						}
						break;
					}
				}
			}
		}
		 BindUtils.postNotifyChange(null, null, this, "databaseList");
	 }
	 
	 @Command("onColumnCheck")
	 public void onColumnCheck(@BindingParam ("columnData") ColumnDetails columnData,@BindingParam ("target") Checkbox target)
	 {
		 if(target.isChecked()){
			 for (DatabaseDetails dbDetails : databaseList){
				 if(dbDetails.getDbId() == columnData.getDbId()){
					 for(TableDetails TableDetails: dbDetails.getTableDetails()){
						 if(TableDetails.getTblId() == columnData.getTblId()){
							 TableDetails.setSelected(true);
							 break;
						 }
					 }
					 dbDetails.setSelected(true);
					 break;
				 }
			 }
		 }
		 else{
			 int dbFlag = 0;
			 int tableFlag = 0;
			 for (DatabaseDetails dbDetails : databaseList){
				 if(dbDetails.getDbId() == columnData.getDbId()){
					 for(TableDetails tableDetails: dbDetails.getTableDetails()){
						 if(tableDetails.getTblId() == columnData.getTblId()){
							 tableDetails.setSelected(false);
							 for(ColumnDetails columnDetails : tableDetails.getColumList()){
								 if(columnDetails.isSelected()){
									 dbFlag = 1;
									 tableFlag = 1;
									 break;
								 }
							 }
							 if(tableFlag == 1)
								 tableDetails.setSelected(true);
						 }
						 if(tableDetails.isSelected())
							 dbFlag = 1;
					 }
					 if(dbFlag == 0)
						 dbDetails.setSelected(false);
					 else
						 dbDetails.setSelected(true);
					 break;
				 }
			 }
		 }
		 BindUtils.postNotifyChange(null, null, this, "databaseList");
	 }
	 
	 
	 @Command
		public void closeDialog(@BindingParam("comp") Window x) {
			x.detach();
		}
}

//End SQLCm-5.4




