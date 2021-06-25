package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.DefaultTreeModel;
import org.zkoss.zul.DefaultTreeNode;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.TreeNode;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.database.properties.CMSensitiveColumnTableData;
import com.idera.sqlcm.entities.database.properties.CMStringCMTableEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.dialogs.userTables.UserTablesForSensitiveColumnsViewModel;

public class SensitiveColumnsStepViewModel extends AddWizardStepBase implements
	UserTablesForSensitiveColumnsViewModel.DialogListener {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/sensitive-columns-step.zul";

    private ListModelList<CMTable> tableListModel;

    @Wire
    private Button configureSCButton;

    private DefaultTreeModel treeModel;

    boolean BeforeAfterCheck = false;

    private SensitiveColumnsTablesListener sensitiveColumnsTablesListener = new SensitiveColumnsTablesListener();

    public SensitiveColumnsStepViewModel() {
	super();
    }

    @Override
    public String getNextStepZul() {
	if (BeforeAfterCheck)
	    return RegulationGuidelineBeforeAfterStepViewModel.ZUL_PATH;
	else
	    return PermissionsCheckStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
	return true;
    }

    @Override
    public String getTips() {
	return ELFunctions
		.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_SENSITIVE_COLUMNS_TIPS);
    }

    @Override
    public String getHelpUrl() {
	return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Sensitive+Column+window";
    }

    public DefaultTreeModel getTreeModel() {
	return treeModel;
    }

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

    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
	BeforeAfterCheck = wizardEntity.getServerConfigEntity()
		.getAuditedActivities().isAuditBeforeAfter();
	TreeNode instanceNode = new DefaultTreeNode(new RootNodeData(
		getInstance()), addDatabaseNodes(wizardEntity
		.getServerConfigEntity().getDatabaseList()));
	treeModel = new DefaultTreeModel(new DefaultTreeNode(null,
		Arrays.asList(instanceNode)));
	treeModel.setOpenObjects(Arrays.asList(instanceNode));
	configureSCButton.setDisabled(true);
	BindUtils.postNotifyChange(null, null, this, "treeModel");
    }

    private List<? extends TreeNode<CMDatabase>> addDatabaseNodes(
	    List<CMDatabase> databaseList) {
	List<DefaultTreeNode<CMDatabase>> nodes = new ArrayList<>(
		databaseList.size());
	for (CMDatabase cmDatabase : databaseList) {
	    DefaultTreeNode<CMDatabase> node = new DefaultTreeNode<>(cmDatabase);
	    nodes.add(node);
	}
	return nodes;
    }

    @Command("onConfigureBtnClick")
    public void onAddBtnClick() {

	TreeNode<CMDatabase> dbNode = (TreeNode<CMDatabase>) Utils
		.getSingleSelectedItem(treeModel);
	CMDatabase db = dbNode.getData();
	try {
	    CMInstance cmInstance = getInstance();
	    SensitiveColumnsConfigureViewModel.showDialog(cmInstance, null, db,
		    sensitiveColumnsTablesListener);
	} catch (Exception e) {
	    // TODO Auto-generated catch block
	    e.printStackTrace();
	}
    }

    @Override
    public void onCloseUserTablesForSensitiveColumnsDialog(CMInstance instance,
	    CMDatabase database, List<CMTable> selectedTables) {
	updateTableListComponentsState();
    }

    @Command("onTreeItemClick")
    public void onTreeItemClick() {
	updateTableListComponentsState();
    }

    @Command("onListItemClick")
    public void onListItemClick() {
	updateTableListComponentsState();
    }

    private void updateTableListComponentsState() {
	TreeNode node = (TreeNode) Utils.getSingleSelectedItem(treeModel);
	Object data = node.getData();
	if (data != null && data instanceof CMDatabase) {
	    CMDatabase db = (CMDatabase) data;
	    if (tableListModel != null) {
		tableListModel.setMultiple(true);
	    }
	    configureSCButton.setDisabled(false);
	} else {
	    tableListModel = null;
	    configureSCButton.setDisabled(true);
	}
	BindUtils.postNotifyChange(null, null, this, "tableListModel");
    }

    private class SensitiveColumnsTablesListener implements
	    SensitiveColumnsConfigureViewModel.DialogListener {
	@Override
	public void onCloseSensitiveColumnsConfigureDialog(
		ListModelList<CMStringCMTableEntity> sensitiveColumnsTablesListModelList) {
	    tableListModel = new ListModelList<>();

	    TreeNode node = (TreeNode) Utils.getSingleSelectedItem(treeModel);
	    Object data = node.getData();
	    if (data != null && data instanceof CMDatabase) {
		CMDatabase db = (CMDatabase) data;
		CMSensitiveColumnTableData tableData = new CMSensitiveColumnTableData();
		tableData
			.setSensitiveTableColumnDictionary(sensitiveColumnsTablesListModelList);
		db.setSensitiveColumnTableData(tableData);
	    }
	}
    }

}
