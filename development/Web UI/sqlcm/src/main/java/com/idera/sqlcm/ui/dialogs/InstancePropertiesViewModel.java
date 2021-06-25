package com.idera.sqlcm.ui.dialogs;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

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
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox; //SQLCm 5.4_4.1.1_Extended Events
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.ListModelSet;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Tabbox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import com.idera.ServerVersion;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMCheckAgentStatusResult;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMInstanceUsersAndRoles;
import com.idera.sqlcm.entities.instances.CMAgentProperties;
import com.idera.sqlcm.entities.instances.CMInstanceProperties;
import com.idera.sqlcm.enumerations.AccessCheckOption;
import com.idera.sqlcm.enumerations.AuditedActivity;
import com.idera.sqlcm.enumerations.DefaultDBPermission;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.server.web.session.SessionUtil;
import com.idera.sqlcm.utils.SQLCMConstants;

public class InstancePropertiesViewModel implements AddPrivilegedUsersViewModel.DialogListener {
    public static final String ZUL_URL = "~./sqlcm/dialogs/instanceProperties/instance-properties-dialog.zul";
    public static final String THRESHOLD_ZUL_URL = "~./sqlcm/dialogs/thresholdNotification/thresholdNotification.zul";
    public static final String INSTANCE_ID = "instance-id";
    public static final String DIALOG_LISTENER = "dialog-listener";
    public static final int DEFAULT_CHARACTERS_LIMIT = 512;
    public static final int MAX_REPORT_CHARACTERS_LIMIT = 32763;
    
    public static final int NO_LIMIT = -1;
    public static long instid;
    public Long instanceId;
    
    private boolean xeventDisabled = true;
    private boolean auditLogDisabled = true;
    private boolean traceChecked = true;
	public boolean isTraceChecked() {
		return traceChecked;
	}

	public void setTraceChecked(boolean traceChecked) {
		this.traceChecked = traceChecked;		
	}

	public boolean isTraceDisabled() {
		return traceDisabled;
	}

	public void setTraceDisabled(boolean traceDisabled) {
		this.traceDisabled = traceDisabled;
	}
	private boolean traceDisabled = true;
   
    public boolean isXeventDisabled() {
		return xeventDisabled;
	}

	public void setXeventDisabled(boolean xeventDisabled) {
		this.xeventDisabled = xeventDisabled;
	}

	public boolean isAuditLogDisabled() {
		return auditLogDisabled;
	}

	public void setAuditLogDisabled(boolean auditLogDisabled) {
		this.auditLogDisabled = auditLogDisabled;
	}

	public long getCurrentInstance() {
		return instid;
	}

    public interface DialogListener {
        void onOk();
    }

    private DialogListener dialogListener;

    //Audited Activities
    public enum AccessCheckFilter {
        PASSED_ONLY(0, ELFunctions.getLabel(SQLCMI18NStrings.INST_PROP_D_AUD_ACTIVITY_TAB_AUDIT_ACTIONS_PASS_ACCESS_CHECK)),
        FAILED_ONLY(2, ELFunctions.getLabel(SQLCMI18NStrings.INST_PROP_D_AUD_ACTIVITY_TAB_AUDIT_ACTIONS_FAILED_ACCESS_CHECK)),
        DISABLED(1, null);

        private String label;
        private int id;

        AccessCheckFilter(int id, String label) {
            this.id = id;
            this.label = label;
        }

        public String getLabel() {
            return label;
        }

        public int getId() {
            return id;
        }

        public static AccessCheckFilter getByIndex(int index) {
            AccessCheckFilter result = null;
            AccessCheckFilter[] values = AccessCheckFilter.values();
            for (int i= 0; i < values.length; i++) {
                if (values[i].getId() == index) {
                    result = values[i];
                }
            }
            return result;
        }
    }

    public enum Tab {
        GENERAL(0, "http://wiki.idera.com/display/SQLCM/Registered+SQL+Server+Properties+window+-+General+tab"),
        AUDITED_ACTIVITIES(1, "http://wiki.idera.com/display/SQLCM/Registered+SQL+Server+Properties+window+-+Audited+Activities+tab"),
        PRIVILEGED_USER_AUDITING(2, "http://wiki.idera.com/display/SQLCM/Registered+SQL+Server+Properties+window+-+Privileged+User+Auditing+tab"),
        AUDITING_THRESHOLDS(3, "http://wiki.idera.com/display/SQLCM/Registered+SQL+Server+Properties+window+-+Auditing+Thresholds+tab"),
        ADVANCED(4, "http://wiki.idera.com/display/SQLCM/Registered+SQL+Server+Properties+window+-+Advanced+tab");

        private String helpUrl;
        private int id;

        private Tab(int id, String helpUrl) {
            this.id = id;
            this.helpUrl = helpUrl;
        }

        public String getHelpUrl() {
            return helpUrl;
        }

        public int getId() {
            return id;
        }

        public static Tab getByIndex(int index) {
            Tab result = null;
            Tab[] values = Tab.values();
            for (Tab value : values) {
                if (value.getId() == index) {
                    result = value;
                }
            }
            return result;
        }
    }

    @Wire("#include_privilegedUserAuditingTab #removePrivilegedUserButton")
    Button removePrivilegedUserButton;

    @Wire("#include_generalTab #auditSettingsStatus")
    Textbox auditSettingsStatus;

    @Wire
    Tabbox tb;
    
    @Wire("#include_privilegedUserAuditingTab #extendedEventCheckbox")
    Checkbox extendedEventCheckbox;

    private String help;
    private String optimizePerformanceLink;

    private CMInstanceProperties instanceProperties;
    private ListModelList<DefaultDBPermission> dbPermissionListModelList;

    private ListModelSet<CMInstancePermissionBase> privilegedUserListModelSet = new ListModelSet<>();

    //AuditedActivities
    private ListModelList<AccessCheckFilter> accessCheckFilterListModelList;
    private boolean accessCheckFilterEnable;

    //privilegedUserAuditing
    private boolean filterEventsAccessChecked;
    private ListModelList<AccessCheckOption> accessCheckOptionListModelList;
    private ListModelList<AuditedActivity> auditedActivityListModelList;

    //Auditing thresholds
    private ListModelList<CMThresholdAdapter> thresholdsListModelList;

    //Advanced
    private int advancedCharactersLimit;
    private int maxReportCharactersLimit;
    private boolean enableCharacterLimitTextbox;

    public CMInstanceProperties getInstanceProperties() {
        return instanceProperties;
    }

    public void setInstanceProperties(CMInstanceProperties instanceProperties) {
        this.instanceProperties = instanceProperties;
    }

    public String getHelp() {
        return Tab.getByIndex(tb.getSelectedPanel().getIndex()).getHelpUrl();
    }

    public String getOptimizePerformanceLink() {
        return optimizePerformanceLink;
    }

    public ListModelList<DefaultDBPermission> getDbPermissionListModelList() {
        return dbPermissionListModelList;
    }

    public ListModelSet<CMInstancePermissionBase> getPrivilegedUserListModelSet() {
        return privilegedUserListModelSet;
    }

    public boolean isDisabledAuditedActivity() {
        return !SessionUtil.canAccess() || privilegedUserListModelSet.isEmpty();
    }

    public boolean isDisabledAuditSelectedActivity() {
        if (SessionUtil.canAccess()) {
            return (AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils.getSingleSelectedItem(auditedActivityListModelList))
                    || privilegedUserListModelSet.isEmpty());
        } else return true;
    }
    
    public boolean isDisablePrivilegedUserAccessCheck() {
        if (SessionUtil.canAccess()) {
            return !filterEventsAccessChecked
                    || privilegedUserListModelSet.isEmpty()
                    || AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils.getSingleSelectedItem(auditedActivityListModelList));
        } else return true;
    }

    public ListModelList<AccessCheckFilter> getAccessCheckFilterListModelList() {
        return accessCheckFilterListModelList;
    }

    public boolean isAccessCheckFilterEnable() {
        return accessCheckFilterEnable && SessionUtil.canAccess();
    }

    public void setAccessCheckFilterEnable(boolean accessCheckFilterEnable) {
        this.accessCheckFilterEnable = accessCheckFilterEnable;
    }

    public ListModelList<AccessCheckOption> getAccessCheckOptionListModelList() {
        return accessCheckOptionListModelList;
    }

    public void setAccessCheckOptionListModelList(ListModelList<AccessCheckOption> accessCheckOptionListModelList) {
        this.accessCheckOptionListModelList = accessCheckOptionListModelList;
    }

    public ListModelList<AuditedActivity> getAuditedActivityListModelList() {
        return auditedActivityListModelList;
    }

    public void setAuditedActivityListModelList(ListModelList<AuditedActivity> auditedActivityListModelList) {
        this.auditedActivityListModelList = auditedActivityListModelList;
    }

    public boolean isUserSQLCaptureEnabled() {
        if (!isDisabledAuditSelectedActivity()) {
            return !AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils.getSingleSelectedItem(auditedActivityListModelList))
                    && (instanceProperties.getAuditedPrivilegedUserActivities().isAuditDML()
                    || instanceProperties.getAuditedPrivilegedUserActivities().isAuditSELECT())
                    && instanceProperties.getAuditedPrivilegedUserActivities().isAllowCaptureSql();
                  
        } else {
            return false;
        }
    }
   

    public boolean isUserTransactionCaptureEnabled() {
        if (!isDisabledAuditSelectedActivity()) {
            return !AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils.getSingleSelectedItem(auditedActivityListModelList))
                    && instanceProperties.getAuditedPrivilegedUserActivities().isAuditDML()
                    && instanceProperties.getAuditedPrivilegedUserActivities().isAgentVersionSupported();
        } else {
            return false;
        }
    }

    public boolean isUserCaptureDDLEnabled() {
        if (!isDisabledAuditSelectedActivity()) {
            return instanceProperties.getAuditedPrivilegedUserActivities().isAuditDDL()
        		|| instanceProperties.getAuditedPrivilegedUserActivities().isAuditSecurity();
        } else {
            return false;
        }
    }

    public boolean isFilterEventsAccessChecked() {
        return filterEventsAccessChecked;
    }

    public void setFilterEventsAccessChecked(boolean filterEventsAccessChecked) {
        this.filterEventsAccessChecked = filterEventsAccessChecked;
    }

    public ListModelList<CMThresholdAdapter> getThresholdsListModelList() {
        return thresholdsListModelList;
    }

    public void setThresholdsListModelList(ListModelList<CMThresholdAdapter> thresholdsListModelList) {
        this.thresholdsListModelList = thresholdsListModelList;
    }

    public int getAdvancedCharactersLimit() {
        return advancedCharactersLimit;
    }

    public void setAdvancedCharactersLimit(int advancedCharactersLimit) {
        this.advancedCharactersLimit = advancedCharactersLimit;
    }
    
    public int getMaxReportCharactersLimit() {
        return maxReportCharactersLimit;
    }

    public void setMaxReportCharactersLimit(int maxReportCharactersLimit) {
        this.maxReportCharactersLimit = maxReportCharactersLimit;
    }

    public boolean isEnableCharacterLimitTextbox() {
        return enableCharacterLimitTextbox;
    }

    public void setEnableCharacterLimitTextbox(boolean enableCharacterLimitTextbox) {
        this.enableCharacterLimitTextbox = enableCharacterLimitTextbox;
    }

    public static void showInstancePropertiesDialog(Long instanceId) {
        showInstancePropertiesDialog(instanceId, null);
    }


    public static void showInstancePropertiesDialog(Long instanceId, DialogListener dialogListener) {
    	instid=instanceId;
    	if (instanceId == null) {
            throw new RuntimeException(" Instance Id must not be null! ");
        }
        Map<String, Object> args = new HashMap<>();
        args.put(INSTANCE_ID, instanceId);
        args.put(DIALOG_LISTENER, dialogListener);

        Window window = (Window) Executions.createComponents(InstancePropertiesViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        help = ServerVersion.SERVER_HELP_WEBSITE;
        help = Tab.GENERAL.getHelpUrl();
        optimizePerformanceLink = SQLCMConstants.SERVER_OPTIMIZE_AUDIT_PERFORMANCE;

        HashMap<String, Object> args = (HashMap<String, Object>)Executions.getCurrent().getArg();
        instanceId = (Long)args.get(INSTANCE_ID);
        dialogListener = (DialogListener) args.get(DIALOG_LISTENER);

        instanceProperties = initInstanceProperties(instanceId);

        //audited activities tab
        accessCheckFilterListModelList = initAccessCheckFilterListModelList();

        //privileged user auditing
        privilegedUserListModelSet.addAll(instanceProperties.getPrivilegedRolesAndUsers().getRoleList());
        privilegedUserListModelSet.addAll(instanceProperties.getPrivilegedRolesAndUsers().getUserList());
        accessCheckOptionListModelList = initAccessCheckOptionListModelList();
        auditedActivityListModelList = initAuditedActivityListModelList();

        //Auditing thresholds
        thresholdsListModelList = CMThresholdAdapter.wrapThresholdsData(instanceProperties.getAuditThresholdsData().getThresholdList());

        //Advanced
        dbPermissionListModelList = initAdvancedDefaultDbPermissionList();
        advancedCharactersLimit = initCharactersLimit();
        maxReportCharactersLimit = initReportCharactersLimit();
        if(instanceProperties.getAuditedPrivilegedUserActivities().isAuditCaptureSQL()){
        	instanceProperties.getAuditedPrivilegedUserActivities().setAuditUserExtendedEvents(false);
        }
        try{
	        String sqlServerVersion = instanceProperties.getGeneralProperties()
					.getInstanceVersion();
	        int instanceVersion = Integer.parseInt(sqlServerVersion.substring(0, 4));
			if (instanceVersion >= 2012) {
				CMAgentProperties agentProperties = InstancesFacade
						.getAgentProperties(instanceProperties
								.getServerId());
				String agentVersion = agentProperties
						.getGeneralProperties().getAgentSettings()
						.getVersion();
				float agentMVersion = Float.parseFloat(agentVersion
						.substring(0, 3));
				if(agentMVersion >= 5.4){
					xeventDisabled = false;
					if (instanceVersion >= 2017 && agentMVersion >= 5.5)
						auditLogDisabled = false;
				}
			}
        }
        catch(RestException ex){}

        traceDisabled = xeventDisabled && auditLogDisabled;
        traceChecked = !instanceProperties.getAuditedActivities().isAuditCaptureSQLXE()
        		&& !instanceProperties.getAuditedActivities().getAuditLogEnabled();
        BindUtils.postNotifyChange(null, null, this, "*");
    }

    @Command
    @NotifyChange("disablePrivilegedUserAccessCheck")
    public void checkPrivilegedUserAccessCheck() {

    }

    @Command
    @NotifyChange("enableCharacterLimitTextbox")
    public void enableCharacterLimitTextbox(@BindingParam("noLimit") boolean noLimit) {
        enableCharacterLimitTextbox = !noLimit;
    }

    @Command
    public void showAddPrivilegedUsers() {
        AddPrivilegedUsersViewModel.showDialog(instanceProperties.getServerId(), null, this);
    }

    @Command
    @NotifyChange({"privilegedUserListModelSet", "removePrivilegedUserButton", "disabledAuditedActivity", "disabledAuditSelectedActivity"})
    public void removeRL() {
        privilegedUserListModelSet.removeAll(privilegedUserListModelSet.getSelection());
        checkRemoveButtonState();
    }

    @Command
    @NotifyChange("removePrivilegedUserButton")
    public void enableRemoveButton() {
        removePrivilegedUserButton.setDisabled(false);
    }

    @Command
    @NotifyChange({"disabledAuditSelectedActivity", "userSQLCaptureEnabled", "userTransactionCaptureEnabled", "disablePrivilegedUserAccessCheck",
            "userCaptureDDLEnabled"})
    public void enableAuditProperties() {    }

    @Command
    @NotifyChange("#include_generalTab #auditSettingsStatus")
    public void updateAuditConfigurationForServer(@BindingParam("instanceId") Long serverId) {
        try {
            String newStatus = InstancesFacade.updateAuditConfigurationForServer(serverId);
            auditSettingsStatus.setValue(newStatus);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPDATE_AUDIT_CONFIGURATION_FOR_SERVER);
        }
    }

    @Command
    @NotifyChange({"userTransactionCaptureEnabled", "userSQLCaptureEnabled"})
    public void enableCaptureTransactionAdnSQL() {}

    @Command
    @NotifyChange("userCaptureDDLEnabled")
    public void enableUserCaptureDDL() {}

    @Command
    @NotifyChange("userSQLCaptureEnabled")
    public void enableCaptureSQL() {}
    
    @Command
    public void updateAuditServerProperties(@BindingParam("comp") Window x) {
        try {
        	
        	if(!enableCharacterLimitTextbox || advancedCharactersLimit > 32763){
        		WebUtil.showWarningBoxWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.INSTANCE_PROPERTIES_DIALOG_ADVANCED_TAB_REPORT_WARNING),
        				ELFunctions.getLabel(SQLCMI18NStrings.REGISTERED_SQL_SERVER_PROPERTIES_TITLE));
        	}        	
            instanceProperties.getAuditedActivities().setAuditAccessCheck(getAuditAccessCheckIndex());
            instanceProperties.setPrivilegedRolesAndUsers(CMInstanceUsersAndRoles.composeInstance(privilegedUserListModelSet));
            instanceProperties.getAuditedPrivilegedUserActivities().setAuditAllUserActivities(
                    Utils.getSingleSelectedItem(auditedActivityListModelList).isId());
            instanceProperties.getAuditedPrivilegedUserActivities().setAuditAccessCheck(getAuditAccessCheckIndexForPrivilegedUsers());
            instanceProperties.getAuditThresholdsData().setThresholdList(CMThresholdAdapter.unwrapThresholdAdapterList(thresholdsListModelList));
            instanceProperties.getServerAdvancedProperties().setDefaultDatabasePermissions(
                    Utils.getSingleSelectedItem(dbPermissionListModelList).getIndex());
            instanceProperties.getServerAdvancedProperties().setSqlStatementLimit(getStatementLimit());

            InstancesFacade.updateAuditServerProperties(instanceProperties);

            if (dialogListener != null) {
                dialogListener.onOk();
            }

            x.detach();
	   	    Executions.sendRedirect("");
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPDATE_AUDIT_SERVER_PROPERTIES);
        }
    }

    @Command
    @NotifyChange("help")
    public void getHelpLink() {}
    
    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    @Override
    public void onOk(long instanceId, List<CMInstancePermissionBase> selectedPermissionList) {
        privilegedUserListModelSet.addAll(selectedPermissionList);
        BindUtils.postNotifyChange(null, null, this, "privilegedUserListModelSet");
        BindUtils.postNotifyChange(null, null, this, "disabledAuditedActivity");
        BindUtils.postNotifyChange(null, null, this, "disabledAuditSelectedActivity");
        BindUtils.postNotifyChange(null, null, this, "userCaptureDDLEnabled");
    }

    @Override
    public void onCancel(long instanceId) {
    }

    private CMInstanceProperties initInstanceProperties(Long instanceId) {
        CMInstanceProperties instanceProperties = null;
        try {
            instanceProperties  = InstancesFacade.getInstanceProperties(instanceId);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_AUDIT_INSTANCE_PROPERTIES);
        }
        return instanceProperties;
    }

    private ListModelList<DefaultDBPermission> initAdvancedDefaultDbPermissionList() {
        ListModelList<DefaultDBPermission> dbPermissionListModelList = new ListModelList<>();
        dbPermissionListModelList.addAll(Arrays.asList(DefaultDBPermission.GRANT_READ_EVENTS_WITH_ADDS,
                DefaultDBPermission.GRANT_READ_EVENTS_ONLY, DefaultDBPermission.GRANT_DENY_READ));
        dbPermissionListModelList.setSelection(
                Arrays.asList(DefaultDBPermission.getByIndex(instanceProperties.getServerAdvancedProperties().getDefaultDatabasePermissions())));
        return dbPermissionListModelList;
    }

    private int initCharactersLimit() {
        if (instanceProperties.getServerAdvancedProperties().getSqlStatementLimit() == NO_LIMIT) {
            enableCharacterLimitTextbox = false;
            return DEFAULT_CHARACTERS_LIMIT;
        } else {
            enableCharacterLimitTextbox = true;
            return instanceProperties.getServerAdvancedProperties().getSqlStatementLimit();
        }
    }
    
    private int initReportCharactersLimit() {
        if (instanceProperties.getServerAdvancedProperties().getSqlStatementLimit() == NO_LIMIT) {
            enableCharacterLimitTextbox = false;
            return MAX_REPORT_CHARACTERS_LIMIT;
        } 
        else if (instanceProperties.getServerAdvancedProperties().getSqlStatementLimit() > 0 && instanceProperties.getServerAdvancedProperties().getSqlStatementLimit() < 32763)
        {
            enableCharacterLimitTextbox = true;
            return instanceProperties.getServerAdvancedProperties().getSqlStatementLimit();
        }
        else {
        	enableCharacterLimitTextbox = true;
        	return MAX_REPORT_CHARACTERS_LIMIT;
        }
    }
    
    private void checkRemoveButtonState() {
        if (privilegedUserListModelSet == null || privilegedUserListModelSet.getSelection().isEmpty()) {
            removePrivilegedUserButton.setDisabled(true);
        }
    }

    private ListModelList<AccessCheckFilter> initAccessCheckFilterListModelList() {
        ListModelList<AccessCheckFilter> accessCheckFilterListModelList = new ListModelList<>();
        accessCheckFilterListModelList.addAll(Arrays.asList(AccessCheckFilter.PASSED_ONLY, AccessCheckFilter.FAILED_ONLY));
        if (instanceProperties.getAuditedActivities().getAuditAccessCheck() == AccessCheckFilter.DISABLED.getId()) {
            accessCheckFilterEnable = false;
            accessCheckFilterListModelList.setSelection(Arrays.asList(AccessCheckFilter.PASSED_ONLY));
        } else {
            accessCheckFilterEnable = true;
            accessCheckFilterListModelList.setSelection(
                    Arrays.asList(AccessCheckFilter.getByIndex(instanceProperties.getAuditedActivities().getAuditAccessCheck())));
        }
        return accessCheckFilterListModelList;
    }

    private ListModelList<AccessCheckOption> initAccessCheckOptionListModelList() {
        ListModelList<AccessCheckOption> accessCheckOptionListModelList = new ListModelList<>();
        accessCheckOptionListModelList.addAll(Arrays.asList(AccessCheckOption.PASSED_ONLY, AccessCheckOption.FAILED_ONLY));
        if (instanceProperties.getAuditedPrivilegedUserActivities().getAuditAccessCheck() == AccessCheckOption.DISABLED.getId()) {
            filterEventsAccessChecked = false;
            accessCheckOptionListModelList.setSelection(Arrays.asList(AccessCheckOption.PASSED_ONLY));
        } else {
            filterEventsAccessChecked = true;
            accessCheckOptionListModelList.setSelection(
                Arrays.asList(AccessCheckOption.getByIndex(instanceProperties.getAuditedPrivilegedUserActivities().getAuditAccessCheck())));
        }
        return accessCheckOptionListModelList;
    }

    private ListModelList<AuditedActivity> initAuditedActivityListModelList() {
        ListModelList<AuditedActivity> auditedActivityListModelList = new ListModelList<>();
        auditedActivityListModelList.addAll(Arrays.asList(AuditedActivity.CHECK_ALL_ACTIVITIES, AuditedActivity.CHECK_SELECTED_ACTIVITIES));
        if (instanceProperties.getAuditedPrivilegedUserActivities().isAuditAllUserActivities()) {
            auditedActivityListModelList.setSelection(Arrays.asList(AuditedActivity.CHECK_ALL_ACTIVITIES));
        } else {
            auditedActivityListModelList.setSelection(Arrays.asList(AuditedActivity.CHECK_SELECTED_ACTIVITIES));
        }
        return auditedActivityListModelList;
    }

    private int getAuditAccessCheckIndex() {
        if (accessCheckFilterEnable) {
            return Utils.getSingleSelectedItem(accessCheckFilterListModelList).getId();
        } else {
            return AccessCheckFilter.DISABLED.getId();
        }
    }

    private int getAuditAccessCheckIndexForPrivilegedUsers() {
        if (filterEventsAccessChecked && !isDisabledAuditSelectedActivity()) {
            return Utils.getSingleSelectedItem(accessCheckOptionListModelList).getId();
        } else {
            return AccessCheckOption.DISABLED.getId();
        }
    }

    private int getStatementLimit() {
        if (enableCharacterLimitTextbox) {
            return advancedCharactersLimit;
        } else {
            return NO_LIMIT;
        }
    }

    @Command("onAuditCaptureSQLCheck")
    public void onAuditCaptureSQLCheck(@BindingParam("checked") boolean checked) {
        if (checked) {
            WebUtil.showWarningWithCustomMessage(ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_LOW_LEVEL_DETAILS_CAN_INCREASE));
        }
    }
    
    // SQLCm 5.4_4.1.1_Extended Events End
    @Command
    public void showThresholdNotificationDialog() {    
    	Map<String, Object> args = new HashMap<>();
        args.put(INSTANCE_ID, instid);
        args.put(DIALOG_LISTENER, null);
        Window window = (Window) Executions.createComponents(THRESHOLD_ZUL_URL, null, args);
        window.doHighlighted();
    
    }
    
    public long getCurrentInstanceValue(){
    	return getCurrentInstance();
    }
    
    private boolean checkAgentStatus(String serverName){
    	try {
            CMCheckAgentStatusResult cmCheckAgentStatusResult = InstancesFacade.checkAgentStatus(serverName);
            if (cmCheckAgentStatusResult.isActive()) {
                return true;
            } else {
                return false;
            }
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_CHECK_AGENT_STATUS);
            return false;
        }
    } 
	
	@Command("enableAuditOption")
	public void enableAuditOption(@BindingParam("target") Radiogroup target){
		if(target.getSelectedIndex() == 0){
			instanceProperties.getAuditedActivities().setAuditCaptureSQLXE(false);
			instanceProperties.getAuditedActivities().setAuditLogEnabled(false);
			return;
		}
		
		else if(target.getSelectedIndex() == 1){
			boolean isDllLoaded = InstancesFacade.isLinqDllLoaded();
			if (!isDllLoaded) {
				instanceProperties.getAuditedActivities()
						.setAuditCaptureSQLXE(false);
				WebUtil.showErrorWithCustomMessage(
						ELFunctions
								.getLabel(SQLCMI18NStrings.EXTENDED_EVENTS_LINQ_LOAD_MESSAGE),
						ELFunctions
								.getLabel(SQLCMI18NStrings.EXTENDED_EVENTS_LINQ_LOAD_MESSAGE_LINK));
				traceChecked = true;
				BindUtils.postNotifyChange(null, null, this, "traceChecked");
				return;
			}
		}

		if (!checkAgentStatus(instanceProperties.getGeneralProperties()
				.getInstance())) {
			if(target.getSelectedIndex() == 1){
				instanceProperties.getAuditedActivities().setAuditCaptureSQLXE(
						false);
				WebUtil.showErrorWithCustomMessage(ELFunctions
						.getLabel(SQLCMI18NStrings.EXTENDED_EVENTS_AGENT_ERROR_MESSAGE));
			}
			else{
			instanceProperties.getAuditedActivities().setAuditLogEnabled(
					false);
			WebUtil.showErrorWithCustomMessage(ELFunctions
					.getLabel(SQLCMI18NStrings.AUDIT_LOGS_AGENT_NOT_REACHABLE));
			}
			traceChecked = true;
			BindUtils.postNotifyChange(null, null, this, "traceChecked");
		}
	}
}

