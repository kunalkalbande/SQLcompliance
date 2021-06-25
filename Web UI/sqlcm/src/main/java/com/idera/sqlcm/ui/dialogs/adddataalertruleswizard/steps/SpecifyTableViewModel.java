package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import com.idera.sqlcm.Utils;
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
import org.zkoss.zul.Label;
import org.zkoss.zul.DefaultTreeModel;
import org.zkoss.zul.DefaultTreeNode;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.TreeNode;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

public class SpecifyTableViewModel implements
		AddPrivilegedUsersViewModel.DialogListener {

	private DefaultTreeModel treeModel;
	CMDataAlertRulesInfo cmDataAlertRulesInfo = new CMDataAlertRulesInfo();
	CMDataAlertTableInfo cmDataAlertTableInfo = new CMDataAlertTableInfo();
	CMDataAlertDBInfo cmDataAlertDBinfo = new CMDataAlertDBInfo();
	AlertRulesFacade alertRulesFacade;

	@Wire
	Label dataDesc;
	
	DefaultTreeNode<CMDataAlertDBInfo> node;
	CMInstance instance;
	int instanceId;
	private String help;

	String tableName;
	String dbName;
	String serverName;

	@Wire
	private Window specifyTable;

	@Wire
	private Button OKButton;

	HashSet<Integer> serverIds = new HashSet<Integer>();

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

	// 4.1.1.5

	public static enum Category {
		LISTED(1, ELFunctions.getLabel(SQLCMI18NStrings.LISTED)), // TODO AS ask
																	// .NET team
																	// id
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

	private Category currentInterval = Category.LISTED;
	private ListModelList<Category> intervalListModelList;

	private void initIntervalList(int selectedIndex) {
		intervalListModelList = new ListModelList<>();
		intervalListModelList.add(Category.LISTED);
		intervalListModelList.add(Category.EXCEPT_LISTED);
		currentInterval = intervalListModelList.get(selectedIndex);
		intervalListModelList.setSelection(Arrays.asList(currentInterval));
	}

	@Command("selectAddEventFilter")
	public void selectAddEventFilter(
			@BindingParam("radioGroup") Radiogroup radioGroup)
			throws RestException {
		int iSelected = radioGroup.getSelectedIndex();
		initIntervalList(iSelected);
		Set<Category> selectedIntervals = intervalListModelList.getSelection(); // must
																				// contain
																				// only
																				// 1
																				// item
																				// because
																				// single
																				// selection
																				// mode.
		if (selectedIntervals != null && !selectedIntervals.isEmpty()) {
			for (Category i : selectedIntervals) {
				currentInterval = i;
				Sessions.getCurrent().setAttribute("specifyTableRadio",
						currentInterval.label);
				break;
			}
		}
	}

	// 4.1.1.5
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

	@Command("onTreeItemClick")
	public void onTreeItemClick() {
		updateTableListComponentsState();
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
				// TODO Auto-generated catch block
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
		List<DefaultTreeNode<CMDataAlertTableInfo>> node = new ArrayList<>(
				tableList.size());
		for (CMDataAlertTableInfo cmDataAlertTableInfo : tableList) {
			DefaultTreeNode<CMDataAlertTableInfo> tnode = new DefaultTreeNode<>(
					cmDataAlertTableInfo);
			if (cmDataAlertTableInfo.getDbId() == cmDataAlertDBinfo.getDbId()) {
				node.add(tnode);
			}
		}
		return node;
	}

	private void updateTableListComponentsState() {
		Object pData = null;
		Object pPData = null;
		TreeNode node = (TreeNode) Utils.getSingleSelectedItem(treeModel);
		Object data = node.getData();

		if (data != null) {
			pData = node.getParent().getData();
		}

		if (pData != null) {
			pPData = node.getParent().getParent().getData();
		}
		if (node.isLeaf()) {
			OKButton.setDisabled(false);
			if (data != null && data instanceof CMDataAlertTableInfo) {
				CMDataAlertTableInfo tbl = (CMDataAlertTableInfo) data;
				OKButton.setDisabled(false);
				tableName = tbl.getName();
			}

			if (pData != null
					&& pData instanceof SpecifyTableViewModel.RootNodeDatabase) {
				SpecifyTableViewModel.RootNodeDatabase db = (SpecifyTableViewModel.RootNodeDatabase) pData;
				dbName = db.getName();
			}

			if (pPData != null
					&& pPData instanceof SpecifyTableViewModel.RootNodeData) {
				SpecifyTableViewModel.RootNodeData svr = (SpecifyTableViewModel.RootNodeData) pPData;
				serverName = svr.getName();
			}
		} else {
			OKButton.setDisabled(true);
		}
		BindUtils.postNotifyChange(null, null, this, "*");
	}

	@Command("submitChoice")
	public void submitChoice() {
		if (tableName != null) {
			Sessions.getCurrent().setAttribute("completeTableName", tableName); //SQLCM-4257
			Sessions.getCurrent().setAttribute("tableName", tableName.substring(tableName.indexOf('.') + 1));
			Sessions.getCurrent().setAttribute("dbName", dbName);
			Sessions.getCurrent().setAttribute("serverName", serverName);
		}

		specifyTable.detach();
	}
}
