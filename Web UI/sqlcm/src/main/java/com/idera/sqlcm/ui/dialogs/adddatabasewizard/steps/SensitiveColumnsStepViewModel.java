package com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps;

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
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.dialogs.userTables.UserTablesForSensitiveColumnsViewModel;

public class SensitiveColumnsStepViewModel extends AddWizardStepBase implements
	UserTablesForSensitiveColumnsViewModel.DialogListener {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/adddatabasewizard/steps/sensitive-columns-step.zul";

    private ListModelList<CMTable> tableListModel;

    @Wire
    private Button addTablesButton;

    @Wire
    private Button removeTablesButton;

    private DefaultTreeModel treeModel;

    public SensitiveColumnsStepViewModel() {
        super();
    }

    @Override
    public String getNextStepZul() {
        return PermissionsCheckStepViewModel.ZUL_PATH;
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
	TreeNode instanceNode = new DefaultTreeNode(new RootNodeData(
		getInstance()),
		addDatabaseNodes(wizardSaveEntity.getDatabaseList()));
	treeModel = new DefaultTreeModel(new DefaultTreeNode(null,
		Arrays.asList(instanceNode)));
	treeModel.setOpenObjects(Arrays.asList(instanceNode));
	BindUtils.postNotifyChange(null, null, this, "treeModel");
	addTablesButton.setDisabled(true);
	addTablesButton.setDisabled(true);
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

   

    @Override
    public void onCloseUserTablesForSensitiveColumnsDialog(CMInstance instance,
	    CMDatabase database, List<CMTable> selectedTables) {

	updateTableListComponentsState();
    }

    @Command("onRemoveBtnClick")
    public void onRemoveBtnClick() {
        Utils.removeAllSelectedItems(tableListModel);
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
	    
	    addTablesButton.setDisabled(false);	    
	} else {
	    addTablesButton.setDisabled(true);	   
	}
	BindUtils.postNotifyChange(null, null, this, "tableListModel");
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
