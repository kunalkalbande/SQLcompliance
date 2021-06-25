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

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.database.properties.CMStringCMTableEntity;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

public class RegulationGuidelineBeforeAfterStepViewModel extends AddWizardStepBase{

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/before-after-step.zul";

    private ListModelList<CMTable> tableListModel;
    
    @Wire
    private Button configureButton;

    private DefaultTreeModel treeModel;
    private ListModelList<CMStringCMTableEntity> beforeAfterDataTablesListModelList = new ListModelList<>();

    private BeforeAfterConfigureListener beforeAfterConfigureListener = new BeforeAfterConfigureListener();
    
    public RegulationGuidelineBeforeAfterStepViewModel() {
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
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_SENSITIVE_COLUMNS_TIPS);
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
    	TreeNode instanceNode = new DefaultTreeNode(new RootNodeData(getInstance()),
                addDatabaseNodes(wizardEntity.getServerConfigEntity().getDatabaseList()));
        treeModel = new DefaultTreeModel(new DefaultTreeNode(null, Arrays.asList(instanceNode)));
        treeModel.setOpenObjects(Arrays.asList(instanceNode));
        BindUtils.postNotifyChange(null, null, this, "treeModel");
    	configureButton.setDisabled(true);
        configureButton.setDisabled(true);
    }

    private List<? extends TreeNode<CMDatabase>> addDatabaseNodes(List<CMDatabase> databaseList) {
        List<DefaultTreeNode<CMDatabase>> nodes = new ArrayList<>(databaseList.size());
        for (CMDatabase cmDatabase: databaseList) {
            DefaultTreeNode<CMDatabase> node = new DefaultTreeNode<>(cmDatabase);
            nodes.add(node);
        }
        return nodes;
    }

    @Command("onConfigureBtnClick")
    public void onAddBtnClick() throws RestException {
        TreeNode<CMDatabase> dbNode = (TreeNode<CMDatabase>) Utils.getSingleSelectedItem(treeModel);
        CMDatabase db = dbNode.getData();
        CMInstance instance = getInstance();
        boolean dmlCheck = getParentWizardViewModel().getWizardEntity().getServerConfigEntity().getAuditedActivities().isAuditDML();
		BeforeAfterConfigureViewModel.showDialog(instance,db,beforeAfterConfigureListener,dmlCheck);
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
            List<CMTable> tableList = db.getBeforeAfterTableData();
            if (tableList instanceof ListModelList) {
                tableListModel = (ListModelList)tableList;
            } else {
                if (tableList != null) {
                    tableListModel = new ListModelList<>(tableList);
                    db.setBeforeAfterTableData(tableListModel);
                } else {
                    tableListModel = null; // clean tables view because current database does not have tables
                }
            }
            if (tableListModel != null) {
                tableListModel.setMultiple(true);
            }
            configureButton.setDisabled(false);
        } else {
            tableListModel = null;
            configureButton.setDisabled(true);
        }
        BindUtils.postNotifyChange(null, null, this, "tableListModel");
    }   
    
    private class BeforeAfterConfigureListener implements BeforeAfterConfigureViewModel.DialogListener {
		@Override
		public void onCloseBeforeAfterConfigureDialog(
				ListModelList<CMStringCMTableEntity> beforeAfterDataTablesListModelList) {
			    TreeNode<CMDatabase> dbNode = (TreeNode<CMDatabase>) Utils.getSingleSelectedItem(treeModel);
		        CMDatabase db = dbNode.getData();
		        db.setBeforeAfterTableData(new ArrayList<CMTable>());
			    for(CMStringCMTableEntity cmStringCMTableEntity : beforeAfterDataTablesListModelList)
			    {
			    	db.getBeforeAfterTableData().add(cmStringCMTableEntity.getValue());
			    }			    
			 }
		}    
	@Override
	public String getHelpUrl() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public boolean onBeforeCancel(AddServerWizardEntity wizardEntity) {
		// TODO Auto-generated method stub
		
		return false;
	}
}
