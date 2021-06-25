package com.idera.sqlcm.ui.permissionsCheck;

import com.idera.ServerVersion;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMPermission;
import com.idera.sqlcm.entities.CMPermissionInfo;
import com.idera.sqlcm.entities.CategoryData;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.instances.CMInstanceProperties;
import com.idera.sqlcm.enumerations.DefaultDBPermission;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.server.web.session.SessionUtil;
import com.idera.sqlcm.ui.components.filter.FilterData;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.converter.PermissionStatusToCssStyleConverter;
import com.idera.sqlcm.ui.converter.PermissionStatusToImagePathConverter;
import com.idera.sqlcm.ui.converter.PermissionStatusToLabelConverter;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.CMThresholdAdapter;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.permissionFailDialog.PermissionFailConfirmViewModel;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps.PermissionsCheckStepViewModel;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps.SummaryStepViewModel;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps.PermissionsCheckStepViewModel.PermissionFailConfirmDialogListenerImpl;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps.PermissionsCheckStepViewModel.PermissionStatus;
import com.idera.sqlcm.utils.SQLCMConstants;
import com.idera.sqlcm.wizard.IWizardEntity;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Listitem;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.io.File;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.zkoss.zul.Button;

public class PermissionCheckViewModel implements
		AddPrivilegedUsersViewModel.DialogListener {
	private static final String CSS_STYLE_PASSED = "color: green;";
	private static final String CSS_STYLE_FAILED = "color: red;";

	// SQLCM Req4.1.1.2 - Enhanced Instances View - Start
	public static final String ZUL_URL_PerCheck = "~./sqlcm/dialogs/permissionsCheck/permissions-check-dialog.zul";
	public static final int INVALID_CHECKS_VALUE = -1;
	/*
	 * private CMPermissionInfo permissionInfo; private
	 * ListModelList<CMPermission> permissionList;
	 */
	protected CommonFacade entityFacade;
	protected Map<String, Object> filterRequest = new TreeMap<>();
	// SQLCM Req4.1.1.2 - Enhanced Instances View - End

	public static final String INSTANCE_ID = "instance-id";
	private CMInstance instance;

	Map<String, Integer> instanceListMap = new TreeMap<>();

	private List<CMInstance> instanceList = new ArrayList<>();

	private List<List<CMPermission>> allPermissionList = new ArrayList<>();

	private Button checkPermissionsButton;

	public Button getCheckPermissionsButton() {
		return checkPermissionsButton;
	}

	public void setCheckPermissionsButton(Button checkPermissionsButton) {
		this.checkPermissionsButton = checkPermissionsButton;
	}

	// Audited Activities
	// SQLCM Req4.1.1.2 - Enhanced Instances View - Start
	public PermissionCheckViewModel() {
		Sessions.getCurrent().setAttribute("showResolutionStep", "True");
		entityFacade = new InstancesFacade();
		refreshEntitiesList();
	}

	protected void refreshEntitiesList() {
		entitiesList = entityFacade.getAllEntities(filterRequest);
		verifyEntitiesList();
		BindUtils.postNotifyChange(null, null, this, "entitiesModel");
	}

	protected void verifyEntitiesList() {
		if (entitiesList == null) {
			entitiesModel = new ListModelList<>();
			// errorLabel.getParent().setVisible(true);
		} else {
			entitiesModel = new ListModelList<>(entitiesList);
			entitiesModel.setMultiple(true);
			// errorLabel.getParent().setVisible(false);
		}
	}

	// SQLCM Req4.1.1.2 - Enhanced Instances View - End

	public enum AccessCheckFilter {
		PASSED_ONLY(
				0,
				ELFunctions
						.getLabel(SQLCMI18NStrings.INST_PROP_D_AUD_ACTIVITY_TAB_AUDIT_ACTIONS_PASS_ACCESS_CHECK)), FAILED_ONLY(
				2,
				ELFunctions
						.getLabel(SQLCMI18NStrings.INST_PROP_D_AUD_ACTIVITY_TAB_AUDIT_ACTIONS_FAILED_ACCESS_CHECK)), DISABLED(
				1, null);

		private String label;
		private int id;

		private AccessCheckFilter(int id, String label) {
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
			return AccessCheckFilter.values()[index];
		}
	}

	// Privileged User Auditing
	public enum AccessCheckOption {
		PASSED_ONLY(
				0,
				ELFunctions
						.getLabel(SQLCMI18NStrings.INST_PROP_D_PRIVILEGED_USER_AUD_TAB_PASSED)), FAILED_ONLY(
				2,
				ELFunctions
						.getLabel(SQLCMI18NStrings.INST_PROP_D_PRIVILEGED_USER_AUD_TAB_FAILED)), DISABLED(
				1, null);

		private String label;
		private int id;

		private AccessCheckOption(int id, String label) {
			this.id = id;
			this.label = label;
		}

		public String getLabel() {
			return label;
		}

		public int getId() {
			return id;
		}

		public static AccessCheckOption getByIndex(int index) {
			return AccessCheckOption.values()[index];
		}
	}

	// Privileged User Auditing
	public enum AuditedActivity {
		CHECK_ALL_ACTIVITIES(
				0,
				ELFunctions
						.getLabel(SQLCMI18NStrings.INST_PROP_D_PRIVILEGED_USER_AUD_TAB_AUDIT_ALL_ACTIVITIES)), CHECK_SELECTED_ACTIVITIES(
				1,
				ELFunctions
						.getLabel(SQLCMI18NStrings.INST_PROP_D_PRIVILEGED_USER_AUD_TAB_AUDIT_SELECTED_ACTIVITIES));

		private String label;
		private int id;

		private AuditedActivity(int id, String label) {
			this.id = id;
			this.label = label;
		}

		public String getLabel() {
			return label;
		}

		public int getId() {
			return id;
		}

		public static AuditedActivity getByIndex(int index) {
			return AuditedActivity.values()[index];
		}
	}

	@Wire("#include_advancedTab #charactersLimit")
	Textbox charactersLimit;

	@Wire("#include_privilegedUserAuditingTab #removePrivilegedUserButton")
	Button removePrivilegedUserButton;

	@Wire("#include_generalTab #auditSettingsStatus")
	Textbox auditSettingsStatus;

	// SQLCM Req4.1.1.2 - Enhanced Instances View - Start
	@Wire
	protected Listbox entitiesListBox;
	// SQLCM Req4.1.1.2 - Enhanced Instances View - End

	private String help;
	private String optimizePerformanceLink;

	private CMInstanceProperties instanceProperties;
	private ListModelList<DefaultDBPermission> dbPermissionListModelList;

	private ListModelList<CMInstancePermissionBase> privilegedUserListModelList = new ListModelList();

	// AuditedActivities
	private ListModelList<AccessCheckFilter> accessCheckFilterListModelList;
	private boolean accessCheckFilterEnable;

	// privilegedUserAuditing
	private boolean filterEventsAccessChecked;
	private ListModelList<AccessCheckOption> accessCheckOptionListModelList;
	private ListModelList<AuditedActivity> auditedActivityListModelList;

	// SQLCM Req4.1.1.2 - Enhanced Instances View - Start
	private ListModelList<CMEntity> selectedEntities;
	protected List<CMEntity> entitiesList;
	protected ListModelList<CMEntity> entitiesModel;
	protected ListModelList<Filter> filtersModel;
	protected FilterData filterData;
	// SQLCM Req4.1.1.2 - Enhanced Instances View - Start

	// Auditing thresholds
	private ListModelList<CMThresholdAdapter> thresholdsListModelList;

	public CMInstanceProperties getInstanceProperties() {
		return instanceProperties;
	}

	public void setInstanceProperties(CMInstanceProperties instanceProperties) {
		this.instanceProperties = instanceProperties;
	}

	public String getHelp() {
		return this.help;
	}

	public String getOptimizePerformanceLink() {
		return optimizePerformanceLink;
	}

	public ListModelList<DefaultDBPermission> getDbPermissionListModelList() {
		return dbPermissionListModelList;
	}

	public ListModelList getPrivilegedUserListModelList() {
		return privilegedUserListModelList;
	}

	public boolean isDisabledAuditedActivity() {
		return !SessionUtil.canAccess()
				|| privilegedUserListModelList.isEmpty();
	}

	public boolean isDisabledAuditSelectedActivity() {
		return (AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils
				.getSingleSelectedItem(auditedActivityListModelList)) || privilegedUserListModelList
				.isEmpty());
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

	public ListModelList<AccessCheckOption> getAccessCheckOptionListModelList() {
		return accessCheckOptionListModelList;
	}

	public void setAccessCheckOptionListModelList(
			ListModelList<AccessCheckOption> accessCheckOptionListModelList) {
		this.accessCheckOptionListModelList = accessCheckOptionListModelList;
	}

	public ListModelList<AuditedActivity> getAuditedActivityListModelList() {
		return auditedActivityListModelList;
	}

	public void setAuditedActivityListModelList(
			ListModelList<AuditedActivity> auditedActivityListModelList) {
		this.auditedActivityListModelList = auditedActivityListModelList;
	}

	public boolean isUserTransactionCaptureEnabled() {
		return !AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils
				.getSingleSelectedItem(auditedActivityListModelList))
				&& instanceProperties.getAuditedPrivilegedUserActivities()
						.isAuditDML()
				&& instanceProperties.getAuditedPrivilegedUserActivities()
						.isAgentVersionSupported();
	}

	public boolean isUserSQLCaptureEnabled() {
		return !AuditedActivity.CHECK_ALL_ACTIVITIES.equals(Utils
				.getSingleSelectedItem(auditedActivityListModelList))
				&& (instanceProperties.getAuditedPrivilegedUserActivities()
						.isAuditDML() || instanceProperties
						.getAuditedPrivilegedUserActivities().isAuditSELECT())
				&& instanceProperties.getAuditedPrivilegedUserActivities()
						.isAllowCaptureSql();
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

	public void setThresholdsListModelList(
			ListModelList<CMThresholdAdapter> thresholdsListModelList) {
		this.thresholdsListModelList = thresholdsListModelList;
	}

	// SQLCM Req4.1.1.2 - Enhanced Instances View - Start
	public ListModelList<CMEntity> getSelectedEntities() {
		return selectedEntities;
	}

	public void setSelectedEntities(ListModelList<CMEntity> selectedEntities) {
		this.selectedEntities = selectedEntities;
	}

	public ListModelList<CMEntity> getEntitiesModel() {
		return entitiesModel;
	}

	protected ListModelList<Filter> getFiltersDefinition() {
		return new ListModelList<>();
	}

	public ListModelList<Filter> getFiltersModel() {
		return filtersModel;
	}

	public List<CMEntity> getEntitiesList() {
		return entitiesList;
	}

	public void setEntitiesList(List<CMEntity> entitiesList) {
		this.entitiesList = entitiesList;
	}

	// SQLCM Req4.1.1.2 - Enhanced Instances View - End

	/*
	 * public static void showInstancePropertiesDialog(Long instanceId) { if
	 * (instanceId == null) { throw new
	 * RuntimeException(" Instance Id must not be null! "); } Map<String,
	 * Object> args = new HashMap<>(); args.put(INSTANCE_ID, instanceId);
	 * 
	 * Window window = (Window)
	 * Executions.createComponents(PermissionCheckViewModel.ZUL_URL, null,
	 * args); window.doHighlighted(); }
	 */

	// SQLCM Req4.1.1.2 - Enhanced Instances View - Start
	public static void showPermissionsCheckDialog(Long instanceId) {
		if (instanceId == null) {
			throw new RuntimeException(" Instance Id must not be null! ");
		}
		Map<String, Object> args = new HashMap<>();
		args.put(INSTANCE_ID, instanceId);

		Window window = (Window) Executions.createComponents(
				PermissionCheckViewModel.ZUL_URL_PerCheck, null, args);
		window.doHighlighted();
	}

	// SQLCM Req4.1.1.2 - Enhanced Instances View - End

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		help = ServerVersion.SERVER_HELP_WEBSITE;
		optimizePerformanceLink = SQLCMConstants.SERVER_OPTIMIZE_AUDIT_PERFORMANCE;

		HashMap<String, Object> args = (HashMap<String, Object>) Executions
				.getCurrent().getArg();
		Long instanceId = (Long) args.get(INSTANCE_ID);

		instanceProperties = initInstanceProperties(instanceId);

		// audited activities tab
		accessCheckFilterListModelList = initAccessCheckFilterListModelList();

		// privileged user auditing
		privilegedUserListModelList.addAll(instanceProperties
				.getPrivilegedRolesAndUsers().getRoleList());
		privilegedUserListModelList.addAll(instanceProperties
				.getPrivilegedRolesAndUsers().getUserList());
		accessCheckOptionListModelList = initAccessCheckOptionListModelList();
		auditedActivityListModelList = initAuditedActivityListModelList();

		// Auditing thresholds
		thresholdsListModelList = CMThresholdAdapter
				.wrapThresholdsData(instanceProperties.getAuditThresholdsData()
						.getThresholdList());

		initAdvancedTabLists();
	}

	@Command
	@NotifyChange("charactersLimit")
	public void enableCharacterLimitTextbox() {
		charactersLimit.setReadonly(false);
	}

	@Command
	@NotifyChange("charactersLimit")
	public void disableCharacterLimitTextbox() {
		charactersLimit.setReadonly(true);
	}

	@Command
	public void showAddPrivilegedUsers() {
		AddPrivilegedUsersViewModel.showDialog(
				instanceProperties.getServerId(), null, this);
	}

	@Command
	@NotifyChange({ "privilegedUserListModelList",
			"removePrivilegedUserButton", "disabledAuditedActivity",
			"disabledAuditSelectedActivity" })
	public void removeRL() {
		Utils.removeAllSelectedItems(privilegedUserListModelList);
		checkRemoveButtonState();
	}

	@Command
	@NotifyChange("removePrivilegedUserButton")
	public void enableRemoveButton() {
		removePrivilegedUserButton.setDisabled(false);
	}

	@Command
	@NotifyChange({ "disabledAuditSelectedActivity", "userSQLCaptureEnabled",
			"userTransactionCaptureEnabled" })
	public void enableAuditProperties() {
	}

	@Command
	@NotifyChange("#include_generalTab #auditSettingsStatus")
	public void updateAuditConfigurationForServer(
			@BindingParam("instanceId") Long serverId) {
		try {
			long requestMap = serverId;

			String newStatus = InstancesFacade
					.updateAuditConfigurationForServer(requestMap);
			auditSettingsStatus.setValue(newStatus);
		} catch (RestException e) {
			WebUtil.showErrorBox(
					e,
					SQLCMI18NStrings.FAILED_TO_UPDATE_AUDIT_CONFIGURATION_FOR_SERVER);
		}
	}

	@Command
	@NotifyChange({ "userTransactionCaptureEnabled", "userSQLCaptureEnabled" })
	public void enableCaptureTransactionAdnSQL() {
	}

	@Command
	@NotifyChange("userSQLCaptureEnabled")
	public void enableCaptureSQL() {
	}

	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		Sessions.getCurrent().removeAttribute("showResolutionStep");
		x.detach();
	}

	@Override
	public void onOk(long instanceId,
			List<CMInstancePermissionBase> selectedPermissionList) {
		privilegedUserListModelList.addAll(selectedPermissionList);
		BindUtils.postNotifyChange(null, null, this,
				"privilegedUserListModelList");
		BindUtils.postNotifyChange(null, null, this, "disabledAuditedActivity");
		BindUtils.postNotifyChange(null, null, this,
				"disabledAuditSelectedActivity");
	}

	@Override
	public void onCancel(long instanceId) {
	}

	// SQLCM Req4.1.1.2 - Enhanced Instances View - Start
	public String getOperationInfo() {
		int totalChecks = 8;
		int passedChecks = 0;
		int failedChecks = 0;
		if (permissionInfo != null) {
			totalChecks = permissionInfo.getTotalChecks();
			passedChecks = permissionInfo.getPassedChecks();
			failedChecks = permissionInfo.getFailedChecks();
		}
		return ELFunctions
				.getLabelWithParams(
						SQLCMI18NStrings.ADD_DATABASE_WIZARD_PERMISSION_CHECK_OPERATION_INFO,
						totalChecks, passedChecks, failedChecks);
	}

	// SQLCM Req4.1.1.2 - Enhanced Instances View - End

	public String getServerStatusInfo() {
		int totalServers = 0;
		if (permissionInfo != null) {
			totalServers = ids.size();
			passedServers = totalServers - failedServers;
		}
		return ELFunctions.getLabelWithParams(
				SQLCMI18NStrings.PERMISSION_CHECK_SERVER_OPERATION_INFO,
				totalServers, passedServers, failedServers);
	}

	private CMInstanceProperties initInstanceProperties(Long instanceId) {
		CMInstanceProperties instanceProperties = null;
		try {
			instanceProperties = InstancesFacade
					.getInstanceProperties(instanceId);
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_AUDIT_INSTANCE_PROPERTIES);
		}
		return instanceProperties;
	}

	private void initAdvancedTabLists() {
		dbPermissionListModelList = new ListModelList<>();
		dbPermissionListModelList.addAll(Arrays.asList(DefaultDBPermission
				.values()));
		dbPermissionListModelList.setSelection(Arrays
				.asList(DefaultDBPermission.GRANT_READ_EVENTS_WITH_ADDS));
	}

	private void checkRemoveButtonState() {
		if (privilegedUserListModelList == null
				|| privilegedUserListModelList.getSelection().isEmpty()) {
			removePrivilegedUserButton.setDisabled(true);
		}
	}

	private ListModelList<AccessCheckFilter> initAccessCheckFilterListModelList() {
		ListModelList<AccessCheckFilter> accessCheckFilterListModelList = new ListModelList<>();
		accessCheckFilterListModelList.addAll(Arrays.asList(
				AccessCheckFilter.PASSED_ONLY, AccessCheckFilter.FAILED_ONLY));
		if (instanceProperties.getAuditedActivities().getAuditAccessCheck() == AccessCheckFilter.DISABLED
				.getId()) {
			accessCheckFilterEnable = false;
			accessCheckFilterListModelList.setSelection(Arrays
					.asList(AccessCheckFilter.PASSED_ONLY));
		} else {
			accessCheckFilterEnable = true;
			accessCheckFilterListModelList.setSelection(Arrays
					.asList(AccessCheckFilter.getByIndex(instanceProperties
							.getAuditedActivities().getAuditAccessCheck())));
		}
		return accessCheckFilterListModelList;
	}

	private ListModelList<AccessCheckOption> initAccessCheckOptionListModelList() {
		ListModelList<AccessCheckOption> accessCheckOptionListModelList = new ListModelList<>();
		accessCheckOptionListModelList.addAll(Arrays.asList(
				AccessCheckOption.PASSED_ONLY, AccessCheckOption.FAILED_ONLY));
		if (instanceProperties.getAuditedPrivilegedUserActivities()
				.getAuditAccessCheck() == AccessCheckOption.DISABLED.getId()) {
			filterEventsAccessChecked = false;
			accessCheckOptionListModelList.setSelection(Arrays
					.asList(AccessCheckOption.PASSED_ONLY));
		} else {
			filterEventsAccessChecked = true;
			accessCheckOptionListModelList.setSelection(Arrays
					.asList(AccessCheckOption.getByIndex(instanceProperties
							.getAuditedPrivilegedUserActivities()
							.getAuditAccessCheck())));
		}
		return accessCheckOptionListModelList;
	}

	private ListModelList<AuditedActivity> initAuditedActivityListModelList() {
		ListModelList<AuditedActivity> auditedActivityListModelList = new ListModelList<>();
		auditedActivityListModelList.addAll(Arrays.asList(
				AuditedActivity.CHECK_ALL_ACTIVITIES,
				AuditedActivity.CHECK_SELECTED_ACTIVITIES));
		if (instanceProperties.getAuditedPrivilegedUserActivities()
				.isAuditAllUserActivities()) {
			auditedActivityListModelList.setSelection(Arrays
					.asList(AuditedActivity.CHECK_ALL_ACTIVITIES));
		} else {
			auditedActivityListModelList.setSelection(Arrays
					.asList(AuditedActivity.CHECK_SELECTED_ACTIVITIES));
		}
		return auditedActivityListModelList;
	}

	protected CMInstance getInstance() {
		return instance;
	}

	@Command("loadPermissionsInfo")
	private void loadPermissionsInfo(Integer id) {
		try {
			permissionInfo = InstancesFacade.getPermissionInfo(id);
			permissionList = new ListModelList<>(
					permissionInfo.getPermissionsCheckList());
			if (permissionInfo.getFailedChecks() > 0) {
				List<CMPermission> failedPermissions = extractFailedPermissions(permissionList);
				instance = InstancesFacade.getInstanceDetails(id);
				PermissionFailConfirmViewModel.show(instance,
						failedPermissions,
						new PermissionFailConfirmDialogListenerImpl());
			}
			BindUtils.postNotifyChange(null, null,
					PermissionCheckViewModel.this, "permissionList");
			BindUtils.postNotifyChange(null, null,
					PermissionCheckViewModel.this, "operationInfo");
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_PERMISSIONS_INFO);
		}

	}

	// Multiple instances permission check
	private void loadPermissionsInfoInstance(Integer id, boolean isLast) {
		try {
			permissionInfo = InstancesFacade.getPermissionInfo(id);
			permissionList = new ListModelList<>(
					permissionInfo.getPermissionsCheckList());
			if (permissionInfo.getFailedChecks() > 0) {
				List<CMPermission> failedPermissions = extractFailedPermissions(permissionList);
				allPermissionList.add(failedPermissions);
				instance = InstancesFacade.getInstanceDetails(id);
				instanceList.add(instance);
				
			}
			if (isLast) {
				if (allPermissionList.size() > 0) {
				PermissionFailConfirmViewModel.showList(instanceList,
						allPermissionList,
						new PermissionFailConfirmDialogListenerImpl());
				}
				BindUtils.postNotifyChange(null, null,
						PermissionCheckViewModel.this, "permissionList");
				BindUtils.postNotifyChange(null, null,
						PermissionCheckViewModel.this, "operationInfo");
				BindUtils.postNotifyChange(null, null,
						PermissionCheckViewModel.this, "serverStatusInfo");
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_PERMISSIONS_INFO);
		}
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

	private List<CMPermission> extractFailedPermissions(
			ListModelList<CMPermission> permissionList) {
		List<CMPermission> failedPermissions = new ArrayList<>(
				permissionList.size());
		for (CMPermission p : permissionList) {
			if (p.getStatus() == PermissionStatus.FAILED.getId()) {
				failedPermissions.add(p);
			}
		}
		return failedPermissions;
	}

	@Command("openInstance")
	public void openInstance(@BindingParam("id") int id) {
		Executions.sendRedirect(WebUtil
				.buildPathRelativeToCurrentProduct("instanceView/" + id));
	}

	private Integer iteratorID;

	@Command("reCheckClick1")
	public void reCheckClick1(@BindingParam("id") int id) {
		ids.clear();
		for (Map.Entry<String, Integer> entry : instanceListMap.entrySet()) {
			Integer value = entry.getValue();
			ids.add(value);
		}

		int temp = 1;
		Iterator itr = ids.iterator();
		while (itr.hasNext()) {
			iteratorID = (Integer) itr.next();
			if (temp == ids.size()) {
				loadPermissionsInfoInstance(iteratorID, true);
				instanceList = null;
				instanceList = new ArrayList<>();
				failedServers = allPermissionList.size();
				allPermissionList = null;
				allPermissionList = new ArrayList<>();
			} else
				loadPermissionsInfoInstance(iteratorID, false);
			temp++;
		}
	}

	@Command("chkStatus")
	public void checkResolutionShow() {
		if (Sessions.getCurrent().getAttribute("showResolutionStep") != null) {
			String Status = (String) Sessions.getCurrent().getAttribute(
					"showResolutionStep");
			if (Status.equals("True")) {
				Sessions.getCurrent().setAttribute("showResolutionStep",
						"False");
			} else
				Sessions.getCurrent()
						.setAttribute("showResolutionStep", "True");
		}
	}

	ArrayList<Integer> ids = new ArrayList<Integer>();
	int passedServers = 0;
	int failedServers = 0;

	@Command("idGetter")
	public void idGetter(@BindingParam("id") int id,
			@BindingParam("btn") Button btn,
			@BindingParam("name") String instanceName) {
		if (instanceListMap.containsKey(instanceName)) {
			instanceListMap.remove(instanceName);
		} else {
			instanceListMap.put(instanceName, id);
			btn.setDisabled(false);
		}
		if (instanceListMap.isEmpty()) {
			btn.setDisabled(true);
		}
	}

	private Converter permissionStatusToImagePathConverter = new PermissionStatusToImagePathConverter();

	private Converter permissionStatusToLabelConverter = new PermissionStatusToLabelConverter();

	private Converter permissionStatusToCssStyleConverter = new PermissionStatusToCssStyleConverter();

	private CMPermissionInfo permissionInfo;

	private ListModelList<CMPermission> permissionList;

	public Converter getPermissionStatusToImagePathConverter() {
		return permissionStatusToImagePathConverter;
	}

	public Converter getPermissionStatusToLabelConverter() {
		return permissionStatusToLabelConverter;
	}

	public Converter getPermissionStatusToCssStyleConverter() {
		return permissionStatusToCssStyleConverter;
	}

	public ListModelList<CMPermission> getPermissionList() {
		return permissionList;
	}

	public enum PermissionStatus {
		PASSED(
				0,
				ELFunctions
						.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_PERMISSION_PASSED),
				CSS_STYLE_PASSED, com.idera.server.web.ELFunctions.getImageURL(
						"check-green-circle",
						ELFunctions.IconSize.SMALL.getStringValue())),

		FAILED(
				1,
				ELFunctions
						.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_PERMISSION_FAILED),
				CSS_STYLE_FAILED, com.idera.server.web.ELFunctions.getImageURL(
						"instance-error",
						ELFunctions.IconSize.SMALL.getStringValue())),

		UNKNOWN(
				-1,
				ELFunctions
						.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_PERMISSION_UNKNOWN),
				CSS_STYLE_FAILED, com.idera.server.web.ELFunctions.getImageURL(
						"instance-unknown",
						ELFunctions.IconSize.SMALL.getStringValue()));

		private int id;
		private String label;
		private String labelCss;
		private String imageUrl;

		PermissionStatus(int id, String label, String labelCss, String imageUrl) {
			this.id = id;
			this.label = label;
			this.labelCss = labelCss;
			this.imageUrl = imageUrl;
		}

		public int getId() {
			return id;
		}

		public String getLabel() {
			return label;
		}

		public String getLabelCss() {
			return labelCss;
		}

		public String getImageUrl() {
			return imageUrl;
		}

		public static PermissionStatus getById(long id) {
			for (PermissionStatus e : values()) {
				if (e.id == id) {
					return e;
				}
			}
			return UNKNOWN;
		}
	}

}
