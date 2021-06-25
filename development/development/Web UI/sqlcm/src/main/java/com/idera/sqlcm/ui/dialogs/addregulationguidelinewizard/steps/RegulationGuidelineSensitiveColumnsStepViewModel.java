package com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps;

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

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMAuditedDatabase;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.database.properties.CMSensitiveColumnTableData;
import com.idera.sqlcm.entities.database.properties.CMStringCMTableEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;

public class RegulationGuidelineSensitiveColumnsStepViewModel extends
	RegulationGuidelineAddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/regulationguidelinewizard/steps/sensitive-columns-step.zul";

    private ListModelList<CMTable> tableListModel;

    @Wire
    private Button configureSCButton;

    @Wire
    private Button removeTablesButton;
    private DefaultTreeModel treeModel;
    private List<CMDatabase> listTreeModel = null;
    private ListModelList<CMStringCMTableEntity> sensitiveColumnsTablesListModelList = new ListModelList<>();
    private SensitiveColumnsTablesListener sensitiveColumnsTablesListener = new SensitiveColumnsTablesListener();
    private CMAuditedActivities checkWizardPath;
    public CMAuditedActivities getCheckWizardPath() {
		return checkWizardPath;
	}

	public void setCheckWizardPath(CMAuditedActivities checkWizardPath) {
		this.checkWizardPath = checkWizardPath;
	}
    public RegulationGuidelineSensitiveColumnsStepViewModel() {
	super();
    }

    @Override
    public String getNextStepZul() {
    String nextZulPath;	
    checkWizardPath = getParentWizardViewModel().getWizardEntity().getAuditedActivities();
	if (checkWizardPath.isAuditBeforeAfter()) {
		nextZulPath =  RegulationGuidelineBeforeAfterStepViewModel.ZUL_PATH;
	} else
		nextZulPath =  RegulationGuidelinePermissionsCheckStepViewModel.ZUL_PATH;
	return nextZulPath;
    }

    @Override
    public boolean isValid() {
	return true;
    }

    @Override
    public String getTips() {
	return ELFunctions
		.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_SENSITIVE_COLUMNS_TIPS);
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
    protected void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) {
    	TreeNode instanceNode = new DefaultTreeNode(new RootNodeData(getInstance()),
                addDatabaseNodes(wizardSaveEntity.getDatabaseList()));
        treeModel = new DefaultTreeModel(new DefaultTreeNode(null, Arrays.asList(instanceNode)));
        treeModel.setOpenObjects(Arrays.asList(instanceNode));
        BindUtils.postNotifyChange(null, null, this, "treeModel");
    	configureSCButton.setDisabled(true);
    	configureSCButton.setDisabled(true);
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

    @Command("onTreeItemClick")
    public void onTreeItemClick() {
	updateTableListComponentsState();
    }

    @Command("onListItemClick")
    public void onListItemClick() {
	updateTableListComponentsState();
    }

    @Command("onConfigureBtnClick")
    public void onAddBtnClick() {
	TreeNode<CMDatabase> dbNode = (TreeNode<CMDatabase>) Utils
		.getSingleSelectedItem(treeModel);
	CMDatabase db = dbNode.getData();
	try {
	    CMInstance instance = InstancesFacade
		    .getInstanceDetails(getInstance().getId());
	    CMAuditedDatabase database = DatabasesFacade.getDatabaseDetails(
		    Long.toString(instance.getId()), Long.toString(db.getId()));
	    SensitiveColumnsConfigureViewModel.showDialog(instance, database,
		    db, sensitiveColumnsTablesListener);
	} catch (RestException e) {
	    // TODO Auto-generated catch block
	    e.printStackTrace();
	}
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

	    TreeNode node = (TreeNode)Utils.getSingleSelectedItem(treeModel);
	    Object data = node.getData();
	    if (data != null && data instanceof CMDatabase) {
		CMDatabase db = (CMDatabase) data;
		CMSensitiveColumnTableData tableData = new CMSensitiveColumnTableData();
		tableData.setSensitiveTableColumnDictionary(sensitiveColumnsTablesListModelList);
		db.setSensitiveColumnTableData(tableData);
	    }
	  }
    }

    @Override
    public void onBeforeNext(AddDatabasesSaveEntity wizardSaveEntity) {
    }

    @Override
    public String getHelpUrl() {
	// TODO Auto-generated method stub
	return null;
    }

    @Override
    public boolean onBeforeCancel(AddDatabasesSaveEntity wizardSaveEntity) {
	// TODO Auto-generated method stub
	return false;
    }

}
