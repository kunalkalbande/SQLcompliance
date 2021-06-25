package com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps;

import com.google.common.base.Function;
import com.google.common.base.Strings;
import com.google.common.collect.Collections2;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.*;
import com.idera.sqlcm.entities.database.properties.CMDatabaseProperties;
import com.idera.sqlcm.entities.database.properties.CMStringCMTableEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.enumerations.AuditUserTables;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.userColumns.UserColumnsViewModel;
import com.idera.sqlcm.ui.dialogs.userTables.UserTablesForSensitiveColumnsViewModel;
import com.idera.sqlcm.utils.SQLCMConstants;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.*;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.*;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class BeforeAfterConfigureViewModel {

    public static final String ZUL_URL = "~./sqlcm/dialogs/regulationguidelinewizard/before-after-configure-dialog.zul";

    public static final String INSTANCE_ARG = "instance_arg";

    public static final String DATABASE_ARG = "database_arg";

    public static final String DATABASE_ID = "database-id";
    
    public static final String LISTENER_ARG = "listener_arg";
    
    public static final String DATABASE_CONFIG_ARG = "database_config_arg";
    
    public static final String DML_ENABLED = "dml_enabled";
    private DialogListener listener;
    
    public static final String TRUSTED_USERS_TELL_ME_MORE_URL = "http://wiki.idera.com/display/SQLCM/Audited+Database+Properties+window+-+Trusted+Users+tab";

    private static final Function<CMStringCMTableEntity, CMTable> TO_CMTABLE_TRANSFORMATION = new Function<CMStringCMTableEntity, CMTable>() {
        @Override
        public CMTable apply(CMStringCMTableEntity cmStringCMTableEntity) {
            return cmStringCMTableEntity.getValue();
        }
    };

    private static final Function<CMTable, CMStringCMTableEntity> FROM_CMTABLE_TRANSFORMATION = new Function<CMTable, CMStringCMTableEntity>() {
        @Override
        public CMStringCMTableEntity apply(CMTable cmTable) {
            return new CMStringCMTableEntity(cmTable.getFullTableName(), cmTable);
        }
    };

    private String help = "http://wiki.idera.com/display/SQLCM/Reduce+audit+data+to+optimize+performance";

    private String optimizePerformanceLink;

    private CMInstance instance;

    private CMAuditedDatabase database;

    private CMDatabaseProperties databaseProperties;
    
    private CMDatabase cmDatabase;

    private boolean dmlEnabled;

    private ListModelList<CMTable> auditedUserTablesListModelList = new ListModelList<>();

   //Before-After Data
    @Wire("#beforeAfterDataErrorLabel")
    protected Label beforeAfterDataErrorLabel;

    @Wire("#beforeAfterDataMainContent")
    protected Vlayout beforeAfterDataMainContent;

    private boolean disableBeforeAfterData;

    private ListModelList<CMStringCMTableEntity> beforeAfterDataTablesListModelList = new ListModelList<>();

    private List<CMTable> beforeAfterDataTablesList = new ArrayList<>();

    private BeforeAfterDataAddTablesListener beforeAfterDataAddTablesListener = new BeforeAfterDataAddTablesListener();

    private BeforeAfterDataEditColumnsListener beforeAfterDataEditColumnsListener = new BeforeAfterDataEditColumnsListener();
   
    @Wire("#include_dmlSelectFiltersTab #removeUserTablesAuditingButton")
    Button removeUserTablesAuditingButton;

    @Wire("#include_dmlSelectFiltersTab #userTablesRG")
    Radiogroup userTablesRG;

    @Wire("#removeBADTableButton")
    Button removeBADTableButton;

    @Wire("#editBADTableButton")
    Button editBADTableButton;

    public String getHelp() {
        return this.help;
    }

    public String getOptimizePerformanceLink() {
        return optimizePerformanceLink;
    }

    public CMDatabaseProperties getDatabaseProperties() {
        return databaseProperties;
    }

    public ListModelList<CMTable> getAuditedUserTablesListModelList() {
        return auditedUserTablesListModelList;
    }

    public ListModelList<CMStringCMTableEntity> getBeforeAfterDataTablesListModelList() {
        return beforeAfterDataTablesListModelList;
    }

    public boolean isEnableBeforeAfterData() {
        return databaseProperties.getDmlSelectFilters().isAuditDmlAll()
            || databaseProperties.getDmlSelectFilters().getAuditUserTables() != AuditUserTables.NONE.getId();
    }

    private CMDatabaseProperties getDatabaseProperties(Long databaseId) {
        CMDatabaseProperties databaseProperties = null;
        try {
            databaseProperties = DatabasesFacade.getDatabaseProperties(databaseId);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_DATABASE_PROPERTIES);
        }
        return databaseProperties;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view,
    		@ExecutionArgParam(LISTENER_ARG) DialogListener listener) {
        Selectors.wireComponents(view, this, false);
        Selectors.wireEventListeners(view, this);
        optimizePerformanceLink = SQLCMConstants.SERVER_OPTIMIZE_AUDIT_PERFORMANCE;
        if (Executions.getCurrent().getArg().containsKey(DATABASE_ID)) {
            HashMap<String, Object> args = (HashMap<String, Object>) Executions.getCurrent().getArg();
            Long databaseId = (Long) args.get(DATABASE_ID);
            databaseProperties = getDatabaseProperties(databaseId);
            database = (CMAuditedDatabase) args.get(DATABASE_ARG);
            instance = (CMInstance) args.get(INSTANCE_ARG);
            cmDatabase = (CMDatabase) args.get(DATABASE_CONFIG_ARG);
            dmlEnabled = (boolean) args.get(DML_ENABLED);
            this.listener = listener;
        } else {
            WebUtil.showErrorBoxWithCustomMessage(ELFunctions.getMessage(SQLCMI18NStrings.DATABASE_PARAMETER_NOT_PASSED));
            return;
        }
        //before-after data tab
        if (databaseProperties.getAuditBeforeAfterData().getStatusMessage() != null) {
            beforeAfterDataErrorLabel.getParent().setVisible(true);
            beforeAfterDataMainContent.setVisible(false);
        } else {
        	 	beforeAfterDataTablesListModelList.clear();
	        	if(cmDatabase.getBeforeAfterTableData() != null){
	        		List<CMStringCMTableEntity> cmStringCMTableEntities = new ArrayList<CMStringCMTableEntity>();
	        		for(CMTable cmTable : cmDatabase.getBeforeAfterTableData()){
	        			CMStringCMTableEntity cmStringCMTableEntity = new CMStringCMTableEntity();
	        			cmStringCMTableEntity.setValue(cmTable);
	        			cmStringCMTableEntities.add(cmStringCMTableEntity);
	        		}
	        		beforeAfterDataTablesListModelList.addAll(cmStringCMTableEntities);
	        	}
	        	else if (databaseProperties.getAuditBeforeAfterData().getBeforeAfterTableColumnDictionary() != null) {
	                beforeAfterDataTablesListModelList.addAll(databaseProperties.getAuditBeforeAfterData().getBeforeAfterTableColumnDictionary());
	            }       
            disableBeforeAfterData =!dmlEnabled;
        }       
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    @Command
    public void updateAuditDatabaseProperties(@BindingParam("comp") Window x) {
        try {
            if (databaseProperties.getDmlSelectFilters().getAuditUserTables() == AuditUserTables.FOLLOWING.getId() &&
                (auditedUserTablesListModelList == null || auditedUserTablesListModelList.isEmpty())) {
                WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.ERROR_ONE_TABLE_MUST_BE_SELECTED);
            } else if (!databaseProperties.getDmlSelectFilters().isAuditDmlAll() &&
                databaseProperties.getDmlSelectFilters().getAuditUserTables() == AuditUserTables.NONE.getId()
                && !databaseProperties.getDmlSelectFilters().isAuditDmlOther()
                && !databaseProperties.getDmlSelectFilters().isAuditStoredProcedures()
                && !databaseProperties.getDmlSelectFilters().isAuditSystemTables()) {
                WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.ERROR_ONE_TYPE_OF_OBJECT_MUST_BE_AUDITED);
            } else {
                databaseProperties.getAuditBeforeAfterData().setBeforeAfterTableColumnDictionary(beforeAfterDataTablesListModelList);
                String response = DatabasesFacade.updateDatabaseProperties(databaseProperties);
                if (!Strings.isNullOrEmpty(response)) {
                    WebUtil.showErrorBoxWithCustomMessage(response, SQLCMI18NStrings.FAILED_TO_UPDATE_DATABASE_PROPERTIES);
                }
                x.detach();
            }
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPDATE_DATABASE_PROPERTIES);
        }
    }

    

    @Command
    public void enableCLR() {
        databaseProperties.getAuditBeforeAfterData().getClrStatus().setEnable(true);
        try {
            databaseProperties.getAuditBeforeAfterData().setClrStatus(
                SQLCMRestClient.getInstance().enableCLR(databaseProperties.getAuditBeforeAfterData().getClrStatus()));
            BindUtils.postNotifyChange(null, null, this, "databaseProperties");
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_ENABLE_CLR_STATUS);
        }
    }

    @Command
    public void addBeforeAfterDataTable() {
        CMDatabase db = new CMDatabase();
        db.setName(database.getName());
        db.setDatabaseId(database.getId());
        beforeAfterDataTablesList.clear();
        beforeAfterDataTablesList.addAll(Collections2.transform(beforeAfterDataTablesListModelList, TO_CMTABLE_TRANSFORMATION));
        List<CMTable> auditedUserTablesListFilter = null;
        if (databaseProperties.getDmlSelectFilters().getAuditUserTables() == AuditUserTables.FOLLOWING.getId()) {
            auditedUserTablesListFilter = auditedUserTablesListModelList;
        }
        UserTablesForSensitiveColumnsViewModel.showWithoutTips(instance, db, beforeAfterDataTablesList, auditedUserTablesListFilter,
                beforeAfterDataAddTablesListener, ELFunctions.getLabel(SQLCMI18NStrings.USER_TABLES_FOR_BEFORE_AFTER_DATA_DIALOG_TITLE));
    }

    @Command
    @NotifyChange("removeUserTablesAuditingButton")
    public void enableRemoveUserTablesAuditingButton() {
        removeUserTablesAuditingButton.setDisabled(false);
    }

    @Command
    @NotifyChange("auditedUserTablesListModelList")
    public void removeDMLSelectTable() {
        auditedUserTablesListModelList.removeAll(auditedUserTablesListModelList.getSelection());
        if (auditedUserTablesListModelList == null || auditedUserTablesListModelList.getSelection().isEmpty()) {
            removeUserTablesAuditingButton.setDisabled(true);
        }
    }

    @Command
    @NotifyChange("beforeAfterDataTablesListModelList")
    public void removeBeforeAfterDataTable() {
        beforeAfterDataTablesListModelList.removeAll(beforeAfterDataTablesListModelList.getSelection());
        if (beforeAfterDataTablesListModelList == null || beforeAfterDataTablesListModelList.getSelection().isEmpty()) {
            removeBADTableButton.setDisabled(true);
            editBADTableButton.setDisabled(true);
        }
    }

    @Command
    @NotifyChange("beforeAfterDataTablesListModelList")
    public void editBeforeAfterDataTable() {
        if (beforeAfterDataTablesListModelList.getSelection().iterator().hasNext()) {
            CMTable table = beforeAfterDataTablesListModelList.getSelection().iterator().next().getValue();
            try {
                List<String> columns;
                if (table.getColumnList() == null || table.getColumnList().isEmpty()) {
                    columns = DatabasesFacade.getColumnList(databaseProperties.getDatabaseId(), table.getFullTableName());
                } else {
                    columns = table.getColumnList();
                }
                UserColumnsViewModel.showForBeforeAfterData(database.getId(), table, columns, beforeAfterDataEditColumnsListener);
            } catch (RestException e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_COLUMN_LIST);
            }
        }
    }
    
    @Command
    @NotifyChange({"removeBADTableButton", "editBADTableButton"})
    public void enableRemoveEditBADTables() {
        removeBADTableButton.setDisabled(false);
        editBADTableButton.setDisabled(false);
    }

    @Command("onAuditCaptureSQLCheck")
    public void onAuditCaptureSQLCheck(@BindingParam("checked") boolean checked) {
        if (checked) {
            //WebUtil.showWarningBox(SQLCMI18NStrings.ADD_SERVER_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE);
        	 WebUtil.showWarningWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE));
        }
    }
    
    public boolean isDisableBeforeAfterData() {
        return disableBeforeAfterData;
    }
    
    private class BeforeAfterDataAddTablesListener implements UserTablesForSensitiveColumnsViewModel.DialogListener {

        @Override
        public void onCloseUserTablesForSensitiveColumnsDialog(CMInstance instance, CMDatabase database, List<CMTable> selectedTables) {
            beforeAfterDataTablesListModelList.clear();
            beforeAfterDataTablesListModelList.addAll(Collections2.transform(selectedTables, FROM_CMTABLE_TRANSFORMATION));
            BindUtils.postNotifyChange(null, null, BeforeAfterConfigureViewModel.this, "beforeAfterDataTablesListModelList");
        }
    }

    private class BeforeAfterDataEditColumnsListener implements UserColumnsViewModel.DialogListener {

        @Override
        public void onCloseUserColumnsDialog(Long databaseId, CMTable table) {
            for (CMStringCMTableEntity cmStringCMTableEntity : beforeAfterDataTablesListModelList) {
                if (cmStringCMTableEntity.getValue().getFullTableName().equals(table.getFullTableName())) {
                    cmStringCMTableEntity.getValue().setRowLimit(table.getRowLimit());
                    cmStringCMTableEntity.getValue().setColumnList(table.getColumnList());
                    BindUtils.postNotifyChange(null, null, BeforeAfterConfigureViewModel.this, "beforeAfterDataTablesListModelList");
                    return;
                }
            }
        }
    }

    public static void showDialog(CMInstance instance,CMAuditedDatabase database,CMDatabase db,boolean dmlEnabled,DialogListener listener) {
        Map<String, Object> args = new HashMap<>();
		args.put(BeforeAfterConfigureViewModel.INSTANCE_ARG, instance);
		args.put(BeforeAfterConfigureViewModel.DATABASE_ARG, database);
		args.put(BeforeAfterConfigureViewModel.DATABASE_ID, database.getId());
		args.put(BeforeAfterConfigureViewModel.DATABASE_CONFIG_ARG, db);
		args.put(BeforeAfterConfigureViewModel.DML_ENABLED, dmlEnabled);
		args.put(BeforeAfterConfigureViewModel.LISTENER_ARG, listener);
		Window window = (Window) Executions.createComponents(
				BeforeAfterConfigureViewModel.ZUL_URL, null, args);
		window.doHighlighted();
    }
    
    public interface DialogListener {
        void onCloseBeforeAfterConfigureDialog(ListModelList<CMStringCMTableEntity> beforeAfterDataTablesListModelList);
    }
    
    @Command
    public void okCommand(@BindingParam("comp") Window x) {
            if (listener != null) {
                listener.onCloseBeforeAfterConfigureDialog(beforeAfterDataTablesListModelList);
            }
            x.detach();
    }
}
