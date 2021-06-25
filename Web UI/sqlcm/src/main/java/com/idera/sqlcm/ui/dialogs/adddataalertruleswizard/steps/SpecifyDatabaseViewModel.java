package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMDataAlertDBInfo;
import com.idera.sqlcm.entities.CMDataAlertRuleInfoRequest;
import com.idera.sqlcm.entities.CMDataAlertRulesInfo;
import com.idera.sqlcm.entities.CMDataAlertTableInfo;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.facade.AlertRulesFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.AddDataAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps.SpecifyTableViewModel.RootNodeDatabase;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.permissionFailDialog.PermissionFailConfirmViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.SensitiveColumnsStepViewModel.RootNodeData;

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
import org.zkoss.zul.Button;
import org.zkoss.zul.DefaultTreeModel;
import org.zkoss.zul.DefaultTreeNode;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Listitem;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.TreeNode;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

public class SpecifyDatabaseViewModel implements
		AddPrivilegedUsersViewModel.DialogListener {

	// Author:- Abhay :: New Data Alert Rules

	CMDataAlertRulesInfo cmDataAlertRulesInfo = new CMDataAlertRulesInfo();
	CMDataAlertTableInfo cmDataAlertTableInfo = new CMDataAlertTableInfo();
	CMDataAlertDBInfo cmDataAlertDBinfo = new CMDataAlertDBInfo();
	AlertRulesFacade alertRulesFacade;
	CMDataAlertRulesInfo cmDataAlertRulesInfo_temp = new CMDataAlertRulesInfo();

	@Wire
	private Button OKButton;

	@Wire
	Label dataDesc;

	@Wire
	private Window specifyDatabase;

	int instanceId;

	private String help;

	String dbName;

	String serverName;

	HashSet<Integer> serverIds = new HashSet<Integer>();

	@JsonProperty("databaseList")
	private List<CMDatabase> databaseList;

	@JsonProperty("instance")
	CMInstance instance;

	public CMInstance getInstance() {
		return instance;
	}

	public void setInstance(CMInstance instance) {
		this.instance = instance;
	}

	public List<CMDatabase> getDatabaseList() {
		return databaseList;
	}

	public void setDatabaseList(List<CMDatabase> databaseList) {
		this.databaseList = databaseList;
	}

	public String getHelp() {
		return this.help;
	}

	private DefaultTreeModel treeModel;
	private ListModelList<CMTable> tableListModel;

	public ListModelList<CMTable> getTableListModel() {
		return tableListModel;
	}

	public static class RootNodeData extends CMEntity {
		RootNodeData(CMInstance instance) {
			if (instance != null) {
				this.id = instance.getId();
				this.name = instance.getInstanceName();
			}
		}
	}

	public DefaultTreeModel getTreeModel() {
		return treeModel;
	}

	@Command("onTreeItemClick")
	public void onTreeItemClick() {
		updateTableListComponentsState();
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view)
			throws RestException {
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
			cmDataAlertRuleInfoRequest.setSrvId(0);
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
		List<DefaultTreeNode<CMDataAlertDBInfo>> nodes = new ArrayList<>(
				databaseList.size());
		for (CMDataAlertDBInfo cmDataAlertDBinfo : databaseList) {
			if (cmDataAlertDBinfo.getSrvId() == instance.getId()) {
				DefaultTreeNode dbnode = new DefaultTreeNode(
						new RootNodeDatabase(cmDataAlertDBinfo));
				nodes.add(dbnode);
			}
		}
		return nodes;
	}

	private void updateTableListComponentsState() {
		Object data = null;
		Object pData = null;
		TreeNode node = (TreeNode) Utils.getSingleSelectedItem(treeModel);
		if (node != null) {
			data = node.getData();
		}

		if (data != null) {
			pData = node.getParent().getData();
		}
		if (node.isLeaf()) {
			OKButton.setDisabled(false);
			if (data != null
					&& data instanceof SpecifyTableViewModel.RootNodeDatabase) {
				SpecifyTableViewModel.RootNodeDatabase db = (SpecifyTableViewModel.RootNodeDatabase) data;
				dbName = db.getName();
			}

			if (pData != null
					&& pData instanceof SpecifyDatabaseViewModel.RootNodeData) {
				SpecifyDatabaseViewModel.RootNodeData svr = (SpecifyDatabaseViewModel.RootNodeData) pData;
				serverName = svr.getName();
			}
		} else {
			OKButton.setDisabled(true);
		}
		BindUtils.postNotifyChange(null, null, this, "*");
	}

	public SpecifyDatabaseViewModel() {
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

	public class PermissionFailConfirmDialogListenerImpl implements
			PermissionFailConfirmViewModel.PermissionFailConfirmDialogListener {
		@Override
		public void onIgnore() {
			/* getNextButton().setDisabled(false); */
		}

		@Override
		public void onReCheck() {
			// do nothing
		}
	}

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
		Set<Category> selectedIntervals = intervalListModelList.getSelection();
		if (selectedIntervals != null && !selectedIntervals.isEmpty()) {
			for (Category i : selectedIntervals) {
				currentInterval = i;
				Sessions.getCurrent().setAttribute("specifyDataBaseRadio",
						currentInterval.label);
				break;
			}
		}
	}

	@Command("submitChoice")
	public void submitChoice() {
		if (dbName != null) {
			Sessions.getCurrent().setAttribute("dbName", dbName);
			Sessions.getCurrent().setAttribute("serverName", serverName);
		}

		specifyDatabase.detach();
	}

	private Listitem objectNameMatch;
	private Listbox listObjectMatch;

	public Listitem getObjectNameMatch() {
		return objectNameMatch;
	}

	public void setObjectNameMatch(Listitem objectNameMatch) {
		this.objectNameMatch = objectNameMatch;
	}

	public void setListObjectMatch(Listbox listObjectMatch) {
		this.listObjectMatch = listObjectMatch;
	}

	String eventDatabaseName;

	public Listbox getListObjectMatch() {
		return listObjectMatch;
	}

	public String getEventDatabaseName() {
		return eventDatabaseName;
	}

	public void setEventDatabaseName(String eventDatabaseName) {
		this.eventDatabaseName = eventDatabaseName;
	}

	private ListModelList<Data> dataList = new ListModelList<>();

	@Command
	@NotifyChange("dataList")
	public void addItem() {
		Data data = new Data(this.eventDatabaseName);
		dataList.add(data);
	}

	public ListModelList<Data> getDataList() {
		return dataList;
	}

	public void setDataList(ListModelList<Data> dataList) {
		this.dataList = dataList;
	}

	public class Data {
		String dataBaseName;

		public String getDataBaseName() {
			return dataBaseName;
		}

		public void setDataBaseName(String dataBaseName) {
			this.dataBaseName = dataBaseName;
		}

		public Data(String dataBaseName) {
			super();
			this.dataBaseName = dataBaseName;
		}
	}
}
