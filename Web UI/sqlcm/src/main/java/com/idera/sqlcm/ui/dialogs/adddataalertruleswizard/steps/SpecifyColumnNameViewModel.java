package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMDataAlertColumnInfo;
import com.idera.sqlcm.entities.CMDataAlertDBInfo;
import com.idera.sqlcm.entities.CMDataAlertRuleInfoRequest;
import com.idera.sqlcm.entities.CMDataAlertRulesInfo;
import com.idera.sqlcm.entities.CMDataAlertTableInfo;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.DefaultTreeModel;
import org.zkoss.zul.DefaultTreeNode;
import org.zkoss.zul.Label;
import org.zkoss.zul.TreeNode;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;

public class SpecifyColumnNameViewModel implements
		AddPrivilegedUsersViewModel.DialogListener {
	private String help;
	private DefaultTreeModel treeModel;
	CMDataAlertRulesInfo cmDataAlertRulesInfo = new CMDataAlertRulesInfo();
	CMDataAlertTableInfo cmDataAlertTableInfo = new CMDataAlertTableInfo();
	CMDataAlertDBInfo cmDataAlertDBinfo = new CMDataAlertDBInfo();
	CMDataAlertColumnInfo cmDataAlertColumnInfo = new CMDataAlertColumnInfo();
	AlertRulesFacade alertRulesFacade;
	DefaultTreeNode<CMDataAlertDBInfo> node;
	CMInstance instance;

	HashSet<Integer> serverIds = new HashSet<Integer>();

	@Wire
	Label dataDesc;

	int instanceId;

	@Wire
	private Window specifyColumnName;

	@Wire
	private Button OKButton;

	String columnName;
	String tableName;
	String dbName;
	String serverName;

	public String getHelp() {
		return this.help;
	}

	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@Override
	public void onOk(long instanceId,
			List<CMInstancePermissionBase> selectedPermissionList) {

	}

	@Override
	public void onCancel(long instanceId) {
	}

	public static class RootNodeData extends CMEntity {
		RootNodeData(CMInstance instance) {
			if (instance != null) {
				this.id = instance.getId();
				this.name = instance.getInstanceName();
			}
		}
	}

	public static class RootNodeDatabase extends CMEntity {
		RootNodeDatabase(CMDataAlertDBInfo dbInstance) {
			if (dbInstance != null) {
				this.id = dbInstance.getDbId();
				this.name = dbInstance.getName();
			}
		}
	}

	public static class RootNodeColumn extends CMEntity {
		RootNodeColumn(CMDataAlertTableInfo tInstance) {
			if (tInstance != null) {
				this.id = tInstance.getDbId();
				this.name = tInstance.getName();
			}
		}
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view)
			throws Exception {
		Selectors.wireComponents(view, this, false);
		if (Sessions.getCurrent().getAttribute("RuleTypeAccess") != null) {
			String ruleTypeAccess = (String) Sessions.getCurrent()
					.getAttribute("RuleTypeAccess");
			if (ruleTypeAccess.equals("Sensitive column")) {
				dataDesc.setValue("Choose the Database for which you want to alert on sensitive column access.");
			} else {
				dataDesc.setValue("");
			}
		}
		int ConditionID = 1;
		if (Sessions.getCurrent().getAttribute("FieldId") != null) {
			ConditionID = (int) Sessions.getCurrent().getAttribute("FieldId");
			CMDataAlertRuleInfoRequest cmDataAlertRuleInfoRequest = new CMDataAlertRuleInfoRequest();
			cmDataAlertRuleInfoRequest.setSrvId(instanceId);
			cmDataAlertRuleInfoRequest.setConditionId(ConditionID);
			alertRulesFacade = new AlertRulesFacade();
			cmDataAlertRulesInfo = alertRulesFacade
					.getCMDataAlertRulesInfo(cmDataAlertRuleInfoRequest);
			if (cmDataAlertRulesInfo.getSensitiveDatabase().size() != 0) {
				for (int i = 0; i < cmDataAlertRulesInfo.getSensitiveDatabase()
						.size(); i++) {
							if (!serverIds.contains(cmDataAlertRulesInfo
									.getSensitiveDatabase().get(i).getSrvId())) {
								serverIds.add(cmDataAlertRulesInfo
										.getSensitiveDatabase().get(i)
										.getSrvId());
							}
						}

				GetData();
					}
			 
			else{
				if(ConditionID==1)
					dataDesc.setValue(ELFunctions.getLabel(SQLCMI18NStrings.MSG_DATA_ALERT_RULES_SENSITIVE_COLUMN));
				else
					dataDesc.setValue(ELFunctions.getLabel(SQLCMI18NStrings.MSG_DATA_ALERT_RULES_BEFORE_AFETR));			
			}
			BindUtils.postNotifyChange(null, null, this, "*");
		}
	}

	public DefaultTreeModel getTreeModel() {
		return treeModel;
	}

	protected void GetData() {

		instance = new CMInstance();
		List<TreeNode> instanceNode = new ArrayList<>();
		for (int serverId : serverIds) {

			try {
				instance = InstancesFacade.getInstanceDetails(serverId);
			} catch (RestException e) {
				e.printStackTrace();
			}

			instanceNode.add(new DefaultTreeNode(new RootNodeData(instance),
					addDatabaseNodes(
							cmDataAlertRulesInfo.getSensitiveDatabase(),
							instance)));
		}
		treeModel = new DefaultTreeModel(
				new DefaultTreeNode(null, instanceNode));
		treeModel.setOpenObjects(instanceNode);
		BindUtils.postNotifyChange(null, null, this, "treeModel");
	}

	private List<? extends TreeNode<CMDataAlertDBInfo>> addDatabaseNodes(
			List<CMDataAlertDBInfo> databaseList, CMInstance instance) {
		List<TreeNode<CMDataAlertDBInfo>> nodes = new ArrayList<>(
				databaseList.size());
		for (CMDataAlertDBInfo cmDataAlertDBinfo : databaseList) {
			if (cmDataAlertDBinfo.getSrvId() == instance.getId()) {
				TreeNode dbnode = new DefaultTreeNode(new RootNodeDatabase(
						cmDataAlertDBinfo), addTablesNodes(
						cmDataAlertRulesInfo.getSensitiveTable(),
						cmDataAlertDBinfo));
				nodes.add(dbnode);
			}
		}
		return nodes;
	}

	private List<? extends TreeNode<CMDataAlertTableInfo>> addTablesNodes(
			List<CMDataAlertTableInfo> tableList,
			CMDataAlertDBInfo cmDataAlertDBinfo) {
		List<TreeNode<CMDataAlertTableInfo>> node = new ArrayList<>(
				tableList.size());
		for (CMDataAlertTableInfo cmDataAlertTableInfo : tableList) {
			TreeNode<CMDataAlertTableInfo> tnode = new DefaultTreeNode(
					new RootNodeColumn(cmDataAlertTableInfo), addColumnNodes(
							cmDataAlertRulesInfo.getSensitiveColumn(),
							cmDataAlertTableInfo));
			if (cmDataAlertTableInfo.getDbId() == cmDataAlertDBinfo.getDbId()) {
				node.add(tnode);
			}
		}
		return node;
	}

	private List<? extends TreeNode<CMDataAlertColumnInfo>> addColumnNodes(
			List<CMDataAlertColumnInfo> columnList,
			CMDataAlertTableInfo cmDataAlertTableInfo) {
		HashSet<String> columnNodeName = new HashSet<String>();
		List<DefaultTreeNode<CMDataAlertColumnInfo>> colnode = new ArrayList<>(
				columnList.size());
		DefaultTreeNode<CMDataAlertColumnInfo> cnode = new DefaultTreeNode<>(
				cmDataAlertColumnInfo);
		
		for (CMDataAlertColumnInfo cmDataAlertColumnInfo : columnList) {
			if(cmDataAlertColumnInfo.getName().startsWith(cmDataAlertTableInfo.getName() + "."))
				cmDataAlertColumnInfo.setName(cmDataAlertColumnInfo.getName().replace(cmDataAlertTableInfo.getName() + ".", ""));
			if (cmDataAlertColumnInfo.getObjectId() == cmDataAlertTableInfo
					.getObjectId()
					&& cmDataAlertColumnInfo.getDbId() == cmDataAlertTableInfo
							.getDbId()) {
				DefaultTreeNode<CMDataAlertColumnInfo> tnode = new DefaultTreeNode(
						cmDataAlertColumnInfo);
				if (!columnNodeName.contains(cmDataAlertColumnInfo.getName())) {
					columnNodeName.add(cmDataAlertColumnInfo.getName());
					colnode.add(tnode);
				}
			}
		}

		if (colnode.size() == 0) {
			cmDataAlertColumnInfo.setName("All Columns");
			cmDataAlertColumnInfo.setDbId(cmDataAlertTableInfo.getDbId());
			cmDataAlertColumnInfo.setSrvId(cmDataAlertTableInfo.getSrvId());
			cnode.setData(cmDataAlertColumnInfo);
			colnode.add(cnode);
		}

		return colnode;
	}

	@Command("onTreeItemClick")
	public void onTreeItemClick() {
		updateTableListComponentsState();
	}

	private void updateTableListComponentsState() {
		Object data = null;
		Object pdata = null;
		Object pPdata = null;
		Object pPPdata = null;
		TreeNode node = (TreeNode) Utils.getSingleSelectedItem(treeModel);
		if (node != null) {
			data = node.getData();
		}
		if (data != null) {
			pdata = node.getParent().getData();
		}
		if (pdata != null) {
			pPdata = node.getParent().getParent().getData();
		}
		if (pPdata != null) {
			pPPdata = node.getParent().getParent().getParent().getData();
		}
		if (node.isLeaf()) {
			OKButton.setDisabled(false);
			if (data != null && data instanceof CMDataAlertColumnInfo) {
				CMDataAlertColumnInfo db = (CMDataAlertColumnInfo) data;
				columnName = db.getName();
			}

			if (pdata != null
					&& pdata instanceof SpecifyColumnNameViewModel.RootNodeColumn) {
				SpecifyColumnNameViewModel.RootNodeColumn tbl = (SpecifyColumnNameViewModel.RootNodeColumn) pdata;
				tableName = tbl.getName();
			}

			if (pPdata != null
					&& pPdata instanceof SpecifyColumnNameViewModel.RootNodeDatabase) {
				SpecifyColumnNameViewModel.RootNodeDatabase db = (SpecifyColumnNameViewModel.RootNodeDatabase) pPdata;
				dbName = db.getName();
			}

			if (pPPdata != null
					&& pPPdata instanceof SpecifyColumnNameViewModel.RootNodeData) {
				SpecifyColumnNameViewModel.RootNodeData svr = (SpecifyColumnNameViewModel.RootNodeData) pPPdata;
				serverName = svr.getName();
			}
		} else {
			OKButton.setDisabled(true);
		}
		BindUtils.postNotifyChange(null, null, this, "*");

	}

	@Command("submitChoice")
	public void submitChoice() {
		if (columnName != null) {
			if(columnName.equalsIgnoreCase("All Columns")){
				columnName = "<ALL>";
			}
			Sessions.getCurrent().setAttribute("columnName", columnName);
			Sessions.getCurrent().setAttribute("completeTableName", tableName); //SQLCM-4257
			Sessions.getCurrent().setAttribute("tableName", tableName.substring(tableName.indexOf('.') + 1));
			Sessions.getCurrent().setAttribute("dbName", dbName);
			Sessions.getCurrent().setAttribute("serverName", serverName);
		}
		specifyColumnName.detach();
	}
}
