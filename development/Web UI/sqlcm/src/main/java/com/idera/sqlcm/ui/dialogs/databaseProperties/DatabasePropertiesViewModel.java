package com.idera.sqlcm.ui.dialogs.databaseProperties;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

import org.apache.commons.lang.StringUtils;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.ListModelSet;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Vlayout;
import org.zkoss.zul.Window;

import com.google.common.base.Function;
import com.google.common.base.Strings;
import com.google.common.collect.Collections2;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMAuditedDatabase;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;
import com.idera.sqlcm.entities.CMTable;
import com.idera.sqlcm.entities.database.properties.CMAuditedActivity;
import com.idera.sqlcm.entities.database.properties.CMDatabaseProperties;
import com.idera.sqlcm.entities.database.properties.CMStringCMTableEntity;
import com.idera.sqlcm.enumerations.AccessCheckFilter;
import com.idera.sqlcm.enumerations.AccessCheckOption;
import com.idera.sqlcm.enumerations.AuditUserTables;
import com.idera.sqlcm.enumerations.AuditedActivity;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.server.web.session.SessionUtil;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel.DialogListener;
import com.idera.sqlcm.ui.dialogs.userColumns.UserColumnsViewModel;
import com.idera.sqlcm.ui.dialogs.userTables.UserTablesForSensitiveColumnsViewModel;
import com.idera.sqlcm.utils.SQLCMConstants;

public class DatabasePropertiesViewModel {

    public static final String ZUL_URL = "~./sqlcm/dialogs/databaseProperties/database-properties-dialog.zul";

    public static final String INSTANCE_ARG = "instance_arg";

    public static final String DATABASE_ARG = "database_arg";

    public static final String DATABASE_ID = "database-id";

    public static final String TRUSTED_USERS_TELL_ME_MORE_URL = "http://wiki.idera.com/display/SQLCM/Audited+Database+Properties+window+-+Trusted+Users+tab";

    private static final Function<CMStringCMTableEntity, CMTable> TO_CMTABLE_TRANSFORMATION = new Function<CMStringCMTableEntity, CMTable>() {
        @Override
        public CMTable apply(CMStringCMTableEntity cmStringCMTableEntity) {
            return cmStringCMTableEntity.getValue();
        }
    };

    private static final Function<CMStringCMTableEntity, CMTable> TO_CMTABLE_TRANSFORMATION_FOR_SC = new Function<CMStringCMTableEntity, CMTable>() {
	@Override
	public CMTable apply(CMStringCMTableEntity cmStringCMTableEntity) {
	    return cmStringCMTableEntity.getDatasetTableList().get(0);
	}
    };

    private static final Function<CMTable, CMStringCMTableEntity> FROM_CMTABLE_TRANSFORMATION = new Function<CMTable, CMStringCMTableEntity>() {
	@Override
	public CMStringCMTableEntity apply(CMTable cmTable) {
	    return new CMStringCMTableEntity(cmTable.getFullTableName(),
		    cmTable);
	}
    };

    private static final Function<CMTable, CMStringCMTableEntity> FROM_CMTABLE_TRANSFORMATION_FOR_SC = new Function<CMTable, CMStringCMTableEntity>() {
	@Override
	public CMStringCMTableEntity apply(CMTable cmTable) {
	    CMStringCMTableEntity entity = new CMStringCMTableEntity(
		    cmTable.getFullTableName(), null);
	    List<CMTable> listTable = new ArrayList();
	    listTable.add(cmTable);
	    entity.setDatasetTableList(listTable);
	    return entity;
	}
    };

    private String help = "http://wiki.idera.com/display/SQLCM/Reduce+audit+data+to+optimize+performance";

    private String optimizePerformanceLink;

    private CMInstance instance;

    private CMAuditedDatabase database;

    private CMDatabaseProperties databaseProperties;

    private String selectionType = "Individual";

    // AuditedActivities
    private ListModelList<AccessCheckFilter> accessCheckFilterListModelList;

    private boolean accessCheckFilterEnable;

    private boolean enableCaptureSQL;

    private boolean enableCaptureTransaction;

    //DML/Select Filters
    private boolean disableDMLSelect;

    private boolean disableDMLSelectAuditAll;

    private boolean disableDMLSelectAuditFollowing;

    private ListModelList<CMTable> auditedUserTablesListModelList = new ListModelList<>();

    private DLMSelectAddTablesListener dlmSelectAddTablesListener = new DLMSelectAddTablesListener();

    //Before-After Data
    @Wire("#include_beforeAfterDataTab #beforeAfterDataErrorLabel")
    protected Label beforeAfterDataErrorLabel;

    @Wire("#include_beforeAfterDataTab #beforeAfterDataMainContent")
    protected Vlayout beforeAfterDataMainContent;

    private boolean disableBeforeAfterData;

    private ListModelList<CMStringCMTableEntity> beforeAfterDataTablesListModelList = new ListModelList<>();

    private List<CMTable> beforeAfterDataTablesList = new ArrayList<>();

    private BeforeAfterDataAddTablesListener beforeAfterDataAddTablesListener = new BeforeAfterDataAddTablesListener();

    private BeforeAfterDataEditColumnsListener beforeAfterDataEditColumnsListener = new BeforeAfterDataEditColumnsListener();

    //Sensitive Columns
    @Wire("#include_sensitiveColumnsTab #sensitiveColumnErrorLabel")
    protected Label sensitiveColumnErrorLabel;

    @Wire("#include_sensitiveColumnsTab #sensitiveColumnMainContent")
    protected Vlayout sensitiveColumnMainContent;

    private ListModelList<CMStringCMTableEntity> sensitiveColumnsTablesListModelList = new ListModelList<>();

    private ListModelList<CMStringCMTableEntity> sensitiveColumnsDatasetTablesListModelList = new ListModelList<>();

    private ListModelList<CMStringCMTableEntity> sensitiveColumnsIndividualTablesListModelList = new ListModelList<>();

    private List<CMTable> sensitiveColumnsTablesList = new ArrayList<>();

    private List<CMTable> sensitiveColumnsDatasetTablesList = new ArrayList<>();

    private SensitiveColumnsAddTablesListener sensitiveColumnsAddTablesListener = new SensitiveColumnsAddTablesListener();

    private SensitiveColumnsEditColumnsListener sensitiveColumnsEditColumnsListener = new SensitiveColumnsEditColumnsListener();

    private boolean enableUserTables;

    //TrustedUsers
    private ListModelSet<CMInstancePermissionBase> trustedUserListModelSet = new ListModelSet<>();

    private DialogListener trustedUserDialogListener = new DialogListener() {
        @Override
        public void onOk(long instanceId, List<CMInstancePermissionBase> selectedPermissionList) {
            trustedUserListModelSet.addAll(selectedPermissionList);
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "trustedUserListModelSet");
        }

        @Override
        public void onCancel(long instanceId) {
            // do nothing
        }
    };

    //PrivilegedUserAuditing
    private ListModelSet<CMInstancePermissionBase> privilegedUserListModelSet = new ListModelSet<>();

    private ListModelList<AccessCheckOption> accessCheckOptionListModelList;

    private ListModelList<AuditedActivity> auditedActivityListModelList;

    private boolean filterEventsAccessChecked;

    private DialogListener privilegedUserDialogListener = new DialogListener() {
        @Override
        public void onOk(long instanceId, List<CMInstancePermissionBase> selectedPermissionList) {
            privilegedUserListModelSet.addAll(selectedPermissionList);
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "privilegedUserListModelSet");
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "disabledAuditedActivity");
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "disabledAuditSelectedActivity");
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "userCaptureDDLEnabled");
        }

        @Override
        public void onCancel(long instanceId) {
            // do nothing
        }
    };

    @Wire("#include_dmlSelectFiltersTab #removeUserTablesAuditingButton")
    Button removeUserTablesAuditingButton;

    @Wire("#include_dmlSelectFiltersTab #userTablesRG")
    Radiogroup userTablesRG;

    @Wire("#include_beforeAfterDataTab #removeBADTableButton")
    Button removeBADTableButton;

    @Wire("#include_beforeAfterDataTab #editBADTableButton")
    Button editBADTableButton;

    @Wire("#include_sensitiveColumnsTab #removeSCTableButton")
    Button removeSCTableButton;

    @Wire("#include_sensitiveColumnsTab #editSCTableButton")
    Button editSCTableButton;

    @Wire("#include_privilegedUserAuditingTab #removePrivilegedUserButton")
    Button removePrivilegedUserButton;

    @Wire("#include_trustedUsersTab #removeTrustedUserButton")
    Button removeTrustedUserButton;

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

    public ListModelList<CMStringCMTableEntity> getSensitiveColumnsTablesListModelList() {
        return sensitiveColumnsTablesListModelList;
    }

    public ListModelSet<CMInstancePermissionBase> getTrustedUserListModelSet() {
        return trustedUserListModelSet;
    }

    public String getTrustedUsersTellMeMoreUrl() {
        return TRUSTED_USERS_TELL_ME_MORE_URL;
    }

    public ListModelSet<CMInstancePermissionBase> getPrivilegedUserListModelSet() {
        return privilegedUserListModelSet;
    }

    public ListModelList<AccessCheckOption> getAccessCheckOptionListModelList() {
        return accessCheckOptionListModelList;
    }

    public ListModelList<AuditedActivity> getAuditedActivityListModelList() {
        return auditedActivityListModelList;
    }

    public boolean isFilterEventsAccessChecked() {
        return filterEventsAccessChecked;
    }

    public void setFilterEventsAccessChecked(boolean filterEventsAccessChecked) {
        this.filterEventsAccessChecked = filterEventsAccessChecked;
    }

    public boolean isDisabledAuditSelectedActivity() {
        return (AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils.getSingleSelectedItem(auditedActivityListModelList))
            || privilegedUserListModelSet.isEmpty());
    }

    public boolean isUserSQLCaptureEnabled() {
        if (!isDisabledAuditSelectedActivity()) {
            return !AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils.getSingleSelectedItem(auditedActivityListModelList))
                && (databaseProperties.getAuditedPrivilegedUserActivities().isAuditDML()
                || databaseProperties.getAuditedPrivilegedUserActivities().isAuditSELECT())
                && databaseProperties.getAuditedPrivilegedUserActivities().isAllowCaptureSql();
        } else {
            return false;
        }
    }

    public boolean isUserTransactionCaptureEnabled() {
        if (!isDisabledAuditSelectedActivity()) {
            return !AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils.getSingleSelectedItem(auditedActivityListModelList))
                && databaseProperties.getAuditedPrivilegedUserActivities().isAuditDML()
                && databaseProperties.getAuditedPrivilegedUserActivities().isAgentVersionSupported();
        } else {
            return false;
        }
    }

    public boolean isUserCaptureDDLEnabled() {
        if (!isDisabledAuditSelectedActivity()) {
            return databaseProperties.getAuditedPrivilegedUserActivities().isAuditDDL()
        		|| databaseProperties.getAuditedPrivilegedUserActivities().isAuditSecurity();
        } else {
            return false;
        }
    }

    public boolean isAuditedActivitiesCaptureDDLEnabled() {
    	return databaseProperties.getAuditedActivities().isAuditDDL()
    		|| databaseProperties.getAuditedActivities().isAuditSecurity();
    }

    public boolean isDisabledAuditedActivity() {
        return !SessionUtil.canAccess() || privilegedUserListModelSet.isEmpty();
    }

    public boolean isDisablePrivilegedUserAccessCheck() {
        return !filterEventsAccessChecked
            || privilegedUserListModelSet.isEmpty()
            || AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils.getSingleSelectedItem(auditedActivityListModelList));
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

    private int getAuditAccessCheckIndexForPrivilegedUsers() {
        if (filterEventsAccessChecked && !isDisabledAuditSelectedActivity()) {
            return Utils.getSingleSelectedItem(accessCheckOptionListModelList).getId();
        } else {
            return AccessCheckOption.DISABLED.getId();
        }
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        Selectors.wireEventListeners(view, this);
        optimizePerformanceLink = SQLCMConstants.SERVER_OPTIMIZE_AUDIT_PERFORMANCE;
        if (Executions.getCurrent().getArg().containsKey(DATABASE_ID)) {
            HashMap<String, Object> args = (HashMap<String, Object>) Executions.getCurrent().getArg();
            Long databaseId = (Long) args.get(DATABASE_ID);
            databaseProperties = getDatabaseProperties(databaseId);
            database = (CMAuditedDatabase) args.get(DATABASE_ARG);
            instance = (CMInstance) args.get(INSTANCE_ARG);
        } else {
            WebUtil.showErrorBoxWithCustomMessage(ELFunctions.getMessage(SQLCMI18NStrings.DATABASE_PARAMETER_NOT_PASSED));
            return;
        }

        //audited activities tab
        accessCheckFilterListModelList = initAccessCheckFilterListModelList();
        onCheckAuditActivities();

        //dml/select filters tab
        disableDMLSelectAuditAll = disableDMLSelect || databaseProperties.getDmlSelectFilters().isAuditDmlAll();
        userTablesRG.setSelectedIndex(AuditUserTables.getById(databaseProperties.getDmlSelectFilters().getAuditUserTables()).getIdUI());
        disableDMLSelectAuditFollowing = disableDMLSelectAuditAll || (databaseProperties.getDmlSelectFilters().getAuditUserTables() != 2);
        if (databaseProperties.getDmlSelectFilters().getUserTableList() != null) {
            auditedUserTablesListModelList.addAll(databaseProperties.getDmlSelectFilters().getUserTableList());
        }

        //before-after data tab
        if (databaseProperties.getAuditBeforeAfterData().getStatusMessage() != null) {
            beforeAfterDataErrorLabel.getParent().setVisible(true);
            beforeAfterDataMainContent.setVisible(false);
        } else {
            beforeAfterDataTablesListModelList.clear();
            if (databaseProperties.getAuditBeforeAfterData().getBeforeAfterTableColumnDictionary() != null) {
                beforeAfterDataTablesListModelList.addAll(databaseProperties.getAuditBeforeAfterData().getBeforeAfterTableColumnDictionary());
            }
            disableBeforeAfterData = !databaseProperties.getAuditedActivities().isAuditDML();
        }

        //sensitive columns tab
        if (databaseProperties.getSensitiveColumnTableData().getStatusMessage() != null) {
            sensitiveColumnErrorLabel.getParent().setVisible(true);
            sensitiveColumnMainContent.setVisible(false);
        } else {
            sensitiveColumnsTablesListModelList.clear();
            if (databaseProperties.getSensitiveColumnTableData().getSensitiveTableColumnDictionary() != null) {
                sensitiveColumnsTablesListModelList.addAll(databaseProperties.getSensitiveColumnTableData().getSensitiveTableColumnDictionary());
            }
        }

        //trusted users
        if (databaseProperties.getTrustedRolesAndUsers() != null) {
            if (databaseProperties.getTrustedRolesAndUsers().getUserList() != null) {
                trustedUserListModelSet.addAll(databaseProperties.getTrustedRolesAndUsers().getUserList());
            }
            if (databaseProperties.getTrustedRolesAndUsers().getRoleList() != null) {
                trustedUserListModelSet.addAll(databaseProperties.getTrustedRolesAndUsers().getRoleList());
            }
        }

        //privileged user auditing
        if (databaseProperties.getPrivilegedRolesAndUsers() != null) {
            if (databaseProperties.getPrivilegedRolesAndUsers().getUserList() != null) {
                privilegedUserListModelSet.addAll(databaseProperties.getPrivilegedRolesAndUsers().getUserList());
            }
            if (databaseProperties.getPrivilegedRolesAndUsers().getRoleList() != null) {
                privilegedUserListModelSet.addAll(databaseProperties.getPrivilegedRolesAndUsers().getRoleList());
            }
        }
        accessCheckOptionListModelList = initAccessCheckOptionListModelList();
        auditedActivityListModelList = initAuditedActivityListModelList();
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
                databaseProperties.getAuditedActivities().setAuditAccessCheck(getAuditAccessCheckIndex());
                databaseProperties.getDmlSelectFilters().setUserTableList(auditedUserTablesListModelList);
                databaseProperties.getSensitiveColumnTableData().setSensitiveTableColumnDictionary(sensitiveColumnsTablesListModelList);
                databaseProperties.getAuditBeforeAfterData().setBeforeAfterTableColumnDictionary(beforeAfterDataTablesListModelList);
                databaseProperties.setTrustedRolesAndUsers(CMInstanceUsersAndRoles.composeInstance(trustedUserListModelSet));
                databaseProperties.setPrivilegedRolesAndUsers(CMInstanceUsersAndRoles.composeInstance(privilegedUserListModelSet));
                databaseProperties.getAuditedPrivilegedUserActivities().setAuditAllUserActivities(
                    Utils.getSingleSelectedItem(auditedActivityListModelList).isId());
                databaseProperties.getAuditedPrivilegedUserActivities().setAuditAccessCheck(getAuditAccessCheckIndexForPrivilegedUsers());
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

    @Listen("onCheck = #auditDMLCheckBox, #auditSELECTCheckBox")
    public void onCheckAuditActivities() {
        CMAuditedActivity auditedActivity = databaseProperties.getAuditedActivities();
        enableCaptureSQL = (auditedActivity.isAuditDML() || auditedActivity.isAuditSELECT()) && auditedActivity.isAllowCaptureSql();
        if (!enableCaptureSQL) {
            auditedActivity.setAuditCaptureSQL(false);
        }
        enableCaptureTransaction = auditedActivity.isAuditDML() && auditedActivity.isAgentVersionSupported();
        if (!enableCaptureSQL) {
            auditedActivity.setAuditCaptureTrans(false);
        }
        disableDMLSelect = !(enableCaptureSQL || enableCaptureTransaction);
        disableBeforeAfterData = !auditedActivity.isAuditDML();
        BindUtils.postNotifyChange(null, null, this, "enableCaptureSQL");
        BindUtils.postNotifyChange(null, null, this, "enableCaptureTransaction");
        BindUtils.postNotifyChange(null, null, this, "databaseProperties");
        BindUtils.postNotifyChange(null, null, this, "disableDMLSelect");
        BindUtils.postNotifyChange(null, null, this, "disableBeforeAfterData");
    }

    @Command
    public void onCheckAuditDBObjects(@BindingParam("rg") Radiogroup rg) {
        databaseProperties.getDmlSelectFilters().setAuditDmlAll(rg.getSelectedIndex() == 0);
        reRenderDMLSelectTab();
        disableDMLSelect = !(enableCaptureSQL || enableCaptureTransaction);
        disableDMLSelectAuditAll = disableDMLSelect || databaseProperties.getDmlSelectFilters().isAuditDmlAll();
        if (disableDMLSelectAuditAll) {
            removeUserTablesAuditingButton.setDisabled(true);
        }
        BindUtils.postNotifyChange(null, null, this, "disableDMLSelect");
        BindUtils.postNotifyChange(null, null, this, "disableDMLSelectAuditAll");
        BindUtils.postNotifyChange(null, null, this, "enableBeforeAfterData");
    }

    @Command
    public void onCheckUserTables(@BindingParam("rg") Radiogroup rg) {
        databaseProperties.getDmlSelectFilters().setAuditUserTables(AuditUserTables.getByIdUI(rg.getSelectedIndex()).getId());
        reRenderDMLSelectTab();
        disableDMLSelect = !(enableCaptureSQL || enableCaptureTransaction);
        disableDMLSelectAuditAll = disableDMLSelect || databaseProperties.getDmlSelectFilters().isAuditDmlAll();
        if (disableDMLSelectAuditAll) {
            removeUserTablesAuditingButton.setDisabled(true);
        }
        BindUtils.postNotifyChange(null, null, this, "disableDMLSelect");
        BindUtils.postNotifyChange(null, null, this, "disableDMLSelectAuditAll");
        BindUtils.postNotifyChange(null, null, this, "enableBeforeAfterData");
    }

    private void reRenderDMLSelectTab() {
        disableDMLSelect = !(enableCaptureSQL || enableCaptureTransaction);
        disableDMLSelectAuditAll = disableDMLSelect || databaseProperties.getDmlSelectFilters().isAuditDmlAll();
        disableDMLSelectAuditFollowing = disableDMLSelectAuditAll || (databaseProperties.getDmlSelectFilters().getAuditUserTables() != 2);
        if (disableDMLSelectAuditAll) {
            removeUserTablesAuditingButton.setDisabled(true);
        }
        BindUtils.postNotifyChange(null, null, this, "disableDMLSelect");
        BindUtils.postNotifyChange(null, null, this, "disableDMLSelectAuditAll");
        BindUtils.postNotifyChange(null, null, this, "disableDMLSelectAuditFollowing");
    }

    @Command
    public void addDMLSelectTable() {
        CMDatabase db = new CMDatabase();
        db.setName(database.getName());
        db.setDatabaseId(database.getId());
        UserTablesForSensitiveColumnsViewModel.showWithoutTips(instance, db, auditedUserTablesListModelList,
                dlmSelectAddTablesListener, ELFunctions.getLabel(SQLCMI18NStrings.USER_TABLES_FOR_DML_SELECT_DIALOG_TITLE));
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
    public void addSensitiveColumnsTable(
	    @BindingParam("selection") String selection) {
	CMDatabase db = new CMDatabase();
	db.setName(database.getName());
	db.setDatabaseId(database.getId());
	sensitiveColumnsTablesList.clear();
	sensitiveColumnsIndividualTablesListModelList.clear();

	for (CMStringCMTableEntity cmStringCMTableEntity : sensitiveColumnsTablesListModelList) {
	    if (cmStringCMTableEntity.getType().equals("Individual")) {
		sensitiveColumnsIndividualTablesListModelList
			.add(cmStringCMTableEntity);
	    }
	}
	sensitiveColumnsTablesList.addAll(Collections2.transform(
		sensitiveColumnsIndividualTablesListModelList,
		TO_CMTABLE_TRANSFORMATION_FOR_SC));
	UserTablesForSensitiveColumnsViewModel
		.showWithoutTips(
			instance,
			db,
			sensitiveColumnsTablesList,
			sensitiveColumnsAddTablesListener,
			ELFunctions
				.getLabel(SQLCMI18NStrings.USER_TABLES_SENSITIVE_COLUMNS_DIALOG_TITLE));
	selectionType = selection;
    }

    @Command
    public void addDatasetSensitiveColumnsTable(
	    @BindingParam("selection") String selection) {
	CMDatabase db = new CMDatabase();
	db.setName(database.getName());
	db.setDatabaseId(database.getId());
	sensitiveColumnsDatasetTablesList.clear();
	sensitiveColumnsTablesList
		.addAll(Collections2.transform(
			sensitiveColumnsTablesListModelList,
			TO_CMTABLE_TRANSFORMATION));
	UserTablesForSensitiveColumnsViewModel
		.showWithoutTips(
			instance,
			db,
			sensitiveColumnsDatasetTablesList,
			sensitiveColumnsAddTablesListener,
			ELFunctions
				.getLabel(SQLCMI18NStrings.USER_TABLES_SENSITIVE_COLUMNS_DIALOG_TITLE));
	selectionType = selection;
    }

    @Command
    @NotifyChange("removeUserTablesAuditingButton")
    public void enableRemoveUserTablesAuditingButton() {
        removeUserTablesAuditingButton.setDisabled(false);
    }

    @Command
    @NotifyChange({"trustedUserListModelSet", "removeTrustedUserButton"})
    public void removeTrustedRL() {
        trustedUserListModelSet.removeAll(trustedUserListModelSet.getSelection());
        checkRemoveTrustedUserButtonState();
    }

    @Command
    @NotifyChange({"privilegedUserListModelSet", "removePrivilegedUserButton", "disabledAuditedActivity", "disabledAuditSelectedActivity"})
    public void removeRL() {
        privilegedUserListModelSet.removeAll(privilegedUserListModelSet.getSelection());
        checkRemoveButtonState();
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
            removeSCTableButton.setDisabled(true);
            editSCTableButton.setDisabled(true);
        }
    }

    @Command
    @NotifyChange("sensitiveColumnsTablesListModelList")
    public void removeSensitiveColumnsTable() {
		sensitiveColumnsTablesListModelList
				.removeAll(sensitiveColumnsTablesListModelList.getSelection());
		if (sensitiveColumnsTablesListModelList == null
				|| sensitiveColumnsTablesListModelList.getSelection().isEmpty()) {
			removeSCTableButton.setDisabled(true);
			editSCTableButton.setDisabled(true);
		}
    }

    @Command
    @NotifyChange("sensitiveColumnsTablesListModelList")
    public void editSensitiveColumnsTable() {
	if (sensitiveColumnsTablesListModelList.getSelection().iterator()
		.hasNext()) {
	    CMStringCMTableEntity entityToEdit = sensitiveColumnsTablesListModelList
		    .getSelection().iterator().next();
	    List<CMTable> tables = entityToEdit.getDatasetTableList();
	    try {
		List<String> columns = new ArrayList();
		String entityName = "";
		for (CMTable table : tables) {
		    if (!table.isSelectedColumns()
			    || table.getColumnList() == null
			    || table.getColumnList().isEmpty()) {
			columns.addAll(DatabasesFacade.getColumnList(
				databaseProperties.getDatabaseId(),
				table.getFullTableName()));
			entityName += table.getFullTableName();
		    } else {
			columns.addAll(table.getColumnList());
			entityName += table.getFullTableName() + ",";
		    }
		}
		if (entityName.endsWith(",")) {
		    entityName = entityName.substring(0,
			    entityName.length() - 1);
		}
		if (entityToEdit.getType().equals("Individual")) {
		    if (tables.get(0).isSelectedColumns()) {
			tables.get(0).setColumnList(columns);
		    }
		    UserColumnsViewModel.showForSensitiveColumns(
			    database.getId(), tables.get(0), columns,
				sensitiveColumnsEditColumnsListener, false, "", "");
		} else {
		    CMTable tempTableForEdit = new CMTable();
		    tempTableForEdit.setcolumnId(tables.get(0).getcolumnId());
		    tempTableForEdit.setDatabaseId(tables.get(0)
			    .getDatabaseId());
		    tempTableForEdit.setColumnList(columns);
		    tempTableForEdit.setSelectedColumns(true);
		    tempTableForEdit.setFullTableName(entityName);
		    tempTableForEdit.setType(entityToEdit.getType());
		    UserColumnsViewModel.showForSensitiveColumns(
			    database.getId(), tempTableForEdit, columns,
				sensitiveColumnsEditColumnsListener, false, "", "");
		}
	    } catch (RestException e) {
		WebUtil.showErrorBox(e,
			SQLCMI18NStrings.FAILED_TO_LOAD_COLUMN_LIST);
	    }
	    // removeSCTableButton.setDisabled(true);
	}
	checkRemoveButtonState();
    }

    @Command
    public void showAddPrivilegedUsers() {
        AddPrivilegedUsersViewModel.showDialog(instance.getId(), null, privilegedUserDialogListener);
    }

    @Command
    public void showAddTrustedUsers() {
        AddPrivilegedUsersViewModel.showDialog(instance.getId(), null, trustedUserDialogListener);
    }

    @Command
    @NotifyChange({"disabledAuditSelectedActivity", "userSQLCaptureEnabled", "userTransactionCaptureEnabled", "disablePrivilegedUserAccessCheck",
        "userCaptureDDLEnabled"})
    public void enableAuditProperties() {
    }

    @Command
    @NotifyChange("userCaptureDDLEnabled")
    public void enableUserCaptureDDL() {
    }

    @Command
    @NotifyChange("auditedActivitiesCaptureDDLEnabled")
    public void enableAuditedActivitiesCaptureDDL() {
    }

    @Command
    @NotifyChange("disablePrivilegedUserAccessCheck")
    public void checkPrivilegedUserAccessCheck() {
    }

    @Command
    @NotifyChange({"userTransactionCaptureEnabled", "userSQLCaptureEnabled"})
    public void enableCaptureTransactionAdnSQL() {
    }

    @Command
    @NotifyChange("userSQLCaptureEnabled")
    public void enableCaptureSQL() {
    }

    @Command
    @NotifyChange("removePrivilegedUserButton")
    public void enableRemoveButton() {
        removePrivilegedUserButton.setDisabled(false);
    }

    @Command
    @NotifyChange("removeTrustedUserButton")
    public void enableRemoveTrustedUserButton() {
        removeTrustedUserButton.setDisabled(false);
    }

    @Command
    @NotifyChange({"removeBADTableButton", "editBADTableButton"})
    public void enableRemoveEditBADTables() {
        removeBADTableButton.setDisabled(false);
        editBADTableButton.setDisabled(false);
    }

    @Command
    @NotifyChange({"removeSCTableButton", "editSCTableButton"})
    public void enableRemoveEditSCTables() {
        removeSCTableButton.setDisabled(false);
        editSCTableButton.setDisabled(false);
    }

    @Command("onAuditCaptureSQLCheck")
    public void onAuditCaptureSQLCheck(@BindingParam("checked") boolean checked) {
        if (checked) {
            //WebUtil.showWarningBox(SQLCMI18NStrings.ADD_SERVER_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE);
        	 WebUtil.showWarningWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE));
        }
    }

    public ListModelList<AccessCheckFilter> getAccessCheckFilterListModelList() {
        return accessCheckFilterListModelList;
    }

    public boolean isAccessCheckFilterEnable() {
        return accessCheckFilterEnable;
    }

    public void setAccessCheckFilterEnable(boolean accessCheckFilterEnable) {
        this.accessCheckFilterEnable = accessCheckFilterEnable;
    }

    public boolean isEnableCaptureSQL() {
        return enableCaptureSQL;
    }

    public boolean isEnableCaptureTransaction() {
        return enableCaptureTransaction;
    }

    public boolean isDisableDMLSelect() {
        return disableDMLSelect;
    }

    public boolean isDisableDMLSelectAuditAll() {
        return disableDMLSelectAuditAll;
    }

    public boolean isDisableDMLSelectAuditFollowing() {
        return disableDMLSelectAuditFollowing;
    }

    public boolean isDisableBeforeAfterData() {
        return disableBeforeAfterData;
    }

    private ListModelList<AccessCheckFilter> initAccessCheckFilterListModelList() {
        ListModelList<AccessCheckFilter> accessCheckFilterListModelList = new ListModelList<>();
        accessCheckFilterListModelList.addAll(Arrays.asList(AccessCheckFilter.PASSED_ONLY, AccessCheckFilter.FAILED_ONLY));
        if (databaseProperties.getAuditedActivities().getAuditAccessCheck() == AccessCheckFilter.DISABLED.getId()) {
            accessCheckFilterEnable = false;
            accessCheckFilterListModelList.setSelection(Arrays.asList(AccessCheckFilter.PASSED_ONLY));
        } else {
            accessCheckFilterEnable = true;
            accessCheckFilterListModelList
                .setSelection(Arrays.asList(AccessCheckFilter.getByIndex(databaseProperties.getAuditedActivities().getAuditAccessCheck())));
        }
        return accessCheckFilterListModelList;
    }

    private int getAuditAccessCheckIndex() {
        if (accessCheckFilterEnable) {
            return Utils.getSingleSelectedItem(accessCheckFilterListModelList).getId();
        } else {
            return AccessCheckFilter.DISABLED.getId();
        }
    }

    private void checkRemoveButtonState() {
        if (privilegedUserListModelSet == null || privilegedUserListModelSet.getSelection().isEmpty()) {
            removePrivilegedUserButton.setDisabled(true);
        }
    }

    private void checkRemoveTrustedUserButtonState() {
        if (trustedUserListModelSet == null || trustedUserListModelSet.getSelection().isEmpty()) {
            removeTrustedUserButton.setDisabled(true);
        }
    }

    private ListModelList<AuditedActivity> initAuditedActivityListModelList() {
        ListModelList<AuditedActivity> auditedActivityListModelList = new ListModelList<>();
        auditedActivityListModelList.addAll(Arrays.asList(AuditedActivity.CHECK_ALL_ACTIVITIES, AuditedActivity.CHECK_SELECTED_ACTIVITIES));
        if (databaseProperties.getAuditedPrivilegedUserActivities().isAuditAllUserActivities()) {
            auditedActivityListModelList.setSelection(Arrays.asList(AuditedActivity.CHECK_ALL_ACTIVITIES));
        } else {
            auditedActivityListModelList.setSelection(Arrays.asList(AuditedActivity.CHECK_SELECTED_ACTIVITIES));
        }
        return auditedActivityListModelList;
    }

    private ListModelList<AccessCheckOption> initAccessCheckOptionListModelList() {
        ListModelList<AccessCheckOption> accessCheckOptionListModelList = new ListModelList<>();
        accessCheckOptionListModelList.addAll(Arrays.asList(AccessCheckOption.PASSED_ONLY, AccessCheckOption.FAILED_ONLY));
        if (databaseProperties.getAuditedPrivilegedUserActivities().getAuditAccessCheck() == AccessCheckOption.DISABLED.getId()) {
            filterEventsAccessChecked = false;
            accessCheckOptionListModelList.setSelection(Arrays.asList(AccessCheckOption.PASSED_ONLY));
        } else {
            filterEventsAccessChecked = true;
            accessCheckOptionListModelList.setSelection(
                Arrays.asList(AccessCheckOption.getByIndex(databaseProperties.getAuditedPrivilegedUserActivities().getAuditAccessCheck())));
        }
        return accessCheckOptionListModelList;
    }

    private class DLMSelectAddTablesListener implements UserTablesForSensitiveColumnsViewModel.DialogListener {

        @Override
        public void onCloseUserTablesForSensitiveColumnsDialog(CMInstance instance, CMDatabase database, List<CMTable> selectedTables) {
            auditedUserTablesListModelList.clear();
            auditedUserTablesListModelList.addAll(selectedTables);
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "auditedUserTablesListModelList");
        }
    }

    private class BeforeAfterDataAddTablesListener implements UserTablesForSensitiveColumnsViewModel.DialogListener {

        @Override
        public void onCloseUserTablesForSensitiveColumnsDialog(CMInstance instance, CMDatabase database, List<CMTable> selectedTables) {
            beforeAfterDataTablesListModelList.clear();
            beforeAfterDataTablesListModelList.addAll(Collections2.transform(selectedTables, FROM_CMTABLE_TRANSFORMATION));
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "beforeAfterDataTablesListModelList");
        }
    }

    private class BeforeAfterDataEditColumnsListener implements UserColumnsViewModel.DialogListener {

        @Override
        public void onCloseUserColumnsDialog(Long databaseId, CMTable table) {
            for (CMStringCMTableEntity cmStringCMTableEntity : beforeAfterDataTablesListModelList) {
                if (cmStringCMTableEntity.getValue().getFullTableName().equals(table.getFullTableName())) {
                    cmStringCMTableEntity.getValue().setRowLimit(table.getRowLimit());
                    cmStringCMTableEntity.getValue().setColumnList(table.getColumnList());
                    BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "beforeAfterDataTablesListModelList");
                    return;
                }
            }
        }
    }

    private class SensitiveColumnsAddTablesListener implements
	    UserTablesForSensitiveColumnsViewModel.DialogListener {

	@Override
	public void onCloseUserTablesForSensitiveColumnsDialog(
		CMInstance instance, CMDatabase database,
		List<CMTable> selectedTables) {
	    List<String> columns = new ArrayList<>();
	    List<String> columnsAll = new ArrayList<>();
	    
	    if (selectionType.equals("Dataset")
				&& (selectedTables == null || selectedTables.size() == 0))
			return;
	    for (CMTable cm : selectedTables) {
		cm.setType(selectionType);
	    }

	    if (selectionType.equals("Dataset")) {
		CMStringCMTableEntity cmStringCMTableEntity = new CMStringCMTableEntity();

		// Fetch dataset ID
		long datasetID = 1;
		for (CMStringCMTableEntity currentEntity : sensitiveColumnsTablesListModelList) {
		    if (currentEntity.getDatasetTableList().get(0)
			    .getcolumnId() >= datasetID) {
			datasetID = currentEntity.getDatasetTableList().get(0)
				.getcolumnId() + 1;
		    }
		}

		try {
		    List<String> tableNames = new ArrayList<>();
		    for (CMTable table : selectedTables) {
			columns = DatabasesFacade
				.getColumnList(table.getDatabaseId(),
					table.getFullTableName());
			for(int i=0;i<columns.size();i++){
				columns.set(i,table.getFullTableName()+"."+columns.get(i));
			}
			table.setColumnList(columns);
			table.setSelectedColumns(true);
			table.setcolumnId(datasetID);
			columnsAll.addAll(columns);
			tableNames.add(table.getFullTableName());
		    }

		    cmStringCMTableEntity.setDatasetTableList(selectedTables);

		    cmStringCMTableEntity.setKey(StringUtils.join(tableNames,","));

		    cmStringCMTableEntity.setType("Dataset");

		    sensitiveColumnsTablesListModelList
			    .add(cmStringCMTableEntity);
		} catch (RestException e) {
		    WebUtil.showErrorBox(e,
			    SQLCMI18NStrings.FAILED_TO_LOAD_COLUMN_LIST);
		}
	    } else {
		List<CMStringCMTableEntity> tempList = new ArrayList();
		for (CMStringCMTableEntity entity : sensitiveColumnsTablesListModelList) {
		    if (!entity.getType().equals("Individual")) {
			tempList.add(entity);
		    }
		}
		sensitiveColumnsTablesListModelList.clear();
		sensitiveColumnsTablesListModelList.addAll(tempList);
		for (CMTable table : selectedTables) {
		    CMStringCMTableEntity cmStringCMTableEntity = new CMStringCMTableEntity();
		    List<CMTable> listCMTable = new ArrayList<CMTable>();
		    listCMTable.add(table);
		    cmStringCMTableEntity.setKey(table.getFullTableName());
		    cmStringCMTableEntity.setDatasetTableList(listCMTable);
		    cmStringCMTableEntity.setType("Individual");
		    sensitiveColumnsTablesListModelList
			    .add(cmStringCMTableEntity);
		}
	    }

	    BindUtils.postNotifyChange(null, null,
		    DatabasePropertiesViewModel.this,
		    "sensitiveColumnsTablesListModelList");
	}
    }

    private class SensitiveColumnsEditColumnsListener implements
	    UserColumnsViewModel.DialogListener {

	@Override
	public void onCloseUserColumnsDialog(Long databaseId, CMTable table) {
	    for (CMStringCMTableEntity cmStringCMTableEntity : sensitiveColumnsTablesListModelList) {
		if (cmStringCMTableEntity.getKey().equals(
			table.getFullTableName())
			&& cmStringCMTableEntity.getDatasetTableList().get(0)
				.getcolumnId() == table.getcolumnId()) {
		    for (CMTable tableToUpdate : cmStringCMTableEntity
			    .getDatasetTableList()) {
			try {
			    if (!table.isSelectedColumns()) {
				tableToUpdate.setColumnList(new ArrayList());
			    } else if (table.getColumnList() != null
				    || !table.getColumnList().isEmpty()) {
				List<String> matchedColumns = new ArrayList();
				for (String columnName : table.getColumnList()) {
					if (tableToUpdate.getType().equals(
						"Dataset")) {
							if (columnName.contains(tableToUpdate
								.getFullTableName() + ".")) {
									matchedColumns.add(columnName);
				    		}
					} else {
							matchedColumns.add(columnName);
				}
				}
				tableToUpdate.setColumnList(matchedColumns);
			    }
			} catch (Exception e) {
			    // TODO Auto-generated catch block
			    e.printStackTrace();
			}
		    }
		    BindUtils.postNotifyChange(null, null,
			    DatabasePropertiesViewModel.this,
			    "sensitiveColumnsTablesListModelList");
		    return;
		}
	    }
	}
    }

    private class TrustedUserDialogListener implements AddPrivilegedUsersViewModel.DialogListener {

        @Override
        public void onOk(long instanceId, List<CMInstancePermissionBase> selectedPermissionList) {
            trustedUserListModelSet.addAll(selectedPermissionList);
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "trustedUserListModelSet");
        }

        @Override
        public void onCancel(long instanceId) {
            // do nothing
        }
    }

    private class PrivilegedUserDialogListener implements AddPrivilegedUsersViewModel.DialogListener {

        @Override
        public void onOk(long instanceId, List<CMInstancePermissionBase> selectedPermissionList) {
            privilegedUserListModelSet.addAll(selectedPermissionList);
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "privilegedUserListModelSet");
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "disabledAuditedActivity");
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "disabledAuditSelectedActivity");
            BindUtils.postNotifyChange(null, null, DatabasePropertiesViewModel.this, "userCaptureDDLEnabled");
        }

        @Override
        public void onCancel(long instanceId) {
            // do nothing
        }
    }

}
