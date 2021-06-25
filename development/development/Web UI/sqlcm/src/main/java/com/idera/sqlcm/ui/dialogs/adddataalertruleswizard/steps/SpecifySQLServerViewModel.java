package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import com.idera.ServerVersion;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMDataAlertDBInfo;
import com.idera.sqlcm.entities.CMDataAlertRuleInfoRequest;
import com.idera.sqlcm.entities.CMDataAlertRulesInfo;
import com.idera.sqlcm.entities.CMDataAlertTableInfo;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMPermission;
import com.idera.sqlcm.entities.CMPermissionInfo;
import com.idera.sqlcm.entities.Instance;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.instances.CMInstanceProperties;
import com.idera.sqlcm.enumerations.DefaultDBPermission;
import com.idera.sqlcm.facade.AlertRulesFacade;
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
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.permissionFailDialog.PermissionFailConfirmViewModel;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.NewEventAlertRules;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.steps.AddWizardStepBase;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyDatabaseViewModel.Category;
import com.idera.sqlcm.utils.SQLCMConstants;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.lang.Strings;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.TreeNode;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

public class SpecifySQLServerViewModel implements
		AddPrivilegedUsersViewModel.DialogListener {
	private static final String CSS_STYLE_PASSED = "color: green;";
	private static final String CSS_STYLE_FAILED = "color: red;";

	public static final String ZUL_URL_specifySQLServers = "~./sqlcm/dialogs/adddataalertruleswizard/specifySQLServers.zul";
	public static final String ZUL_URL_specifyTable = "~./sqlcm/dialogs/adddataalertruleswizard/specifyTable.zul";
	public static final String ZUL_URL_specifyDatabase = "~./sqlcm/dialogs/adddataalertruleswizard/specifyDatabase.zul";
	public static final String ZUL_URL_specifyColumnName = "~./sqlcm/dialogs/adddataalertruleswizard/specifyColumnName.zul";
	public static final String ZUL_URL_specifyAlertMessage = "~./sqlcm/dialogs/adddataalertruleswizard/steps/specifyAlertMessage.zul";
	public static final String ZUL_URL_specifyMailAddresses = "~./sqlcm/dialogs/adddataalertruleswizard/steps/specifyMailAddresses.zul";
	public static final int INVALID_CHECKS_VALUE = -1;
	protected CommonFacade entityFacade;
	protected Map<String, Object> filterRequest = new TreeMap<>();
	public Map<String, Object> targetInstances = new HashMap<String, Object>();

	public static final String INSTANCE_ID = "instance-id";
	private CMInstance instance;
	private AddAlertRulesSaveEntity addAlertRulesSaveEntity = new AddAlertRulesSaveEntity();
	private InstancesFacade instancesFacade = new InstancesFacade();
	AlertRulesFacade alertRulesFacade;
	CMDataAlertRulesInfo cmDataAlertRulesInfo;
	
	int ConditionID =1;

	@Wire
	Label dataDesc;

	@Wire
	private Button OKButton;

	@Wire
	private Window specifySQLServers;
	int instanceId;

	// Audited Activities
	public SpecifySQLServerViewModel() throws RestException {
		super();
		entityFacade = new InstancesFacade();
		refreshEntitiesList();
	}

	protected void refreshEntitiesList() throws RestException {
		entitiesList = entityFacade.getAllEntities(filterRequest);
		verifyEntitiesList();
		BindUtils.postNotifyChange(null, null, this, "entitiesModel");
	}

	protected void verifyEntitiesList() throws RestException {
		if (entitiesList == null) {
			entitiesModel = new ListModelList<>();

		} else {
			entitiesModel = new ListModelList<>();
			if (Sessions.getCurrent().getAttribute("FieldId") != null) {
				ConditionID = (int) Sessions.getCurrent().getAttribute(
						"FieldId");

				CMDataAlertRuleInfoRequest cmDataAlertRuleInfoRequest = new CMDataAlertRuleInfoRequest();
				cmDataAlertRuleInfoRequest.setSrvId(0);
				cmDataAlertRuleInfoRequest.setConditionId(ConditionID);
				alertRulesFacade = new AlertRulesFacade();
				cmDataAlertRulesInfo = alertRulesFacade
						.getCMDataAlertRulesInfo(cmDataAlertRuleInfoRequest);
				List<CMDataAlertDBInfo> sensitiveDatabaseList = new ArrayList<CMDataAlertDBInfo>();
				if (cmDataAlertRulesInfo.getSensitiveDatabase().size() != 0) {
					for (int i = 0; i < cmDataAlertRulesInfo
							.getSensitiveDatabase().size(); i++) {
						for (int j = 0; j < cmDataAlertRulesInfo
								.getSensitiveTable().size(); j++) {
							if (cmDataAlertRulesInfo.getSensitiveDatabase()
									.get(i).getDbId() == cmDataAlertRulesInfo
									.getSensitiveTable().get(j).getDbId()) {
								sensitiveDatabaseList.add(cmDataAlertRulesInfo
										.getSensitiveDatabase().get(i));
								break;
							}
						}

					}
					cmDataAlertRulesInfo
							.setSensitiveDatabase(sensitiveDatabaseList);
					if (sensitiveDatabaseList.size() == 0) {
						if(dataDesc !=null)
						{
							if(ConditionID==1)
								dataDesc.setValue(ELFunctions.getLabel(SQLCMI18NStrings.MSG_DATA_ALERT_RULES_SENSITIVE_COLUMN));
							else
								dataDesc.setValue(ELFunctions.getLabel(SQLCMI18NStrings.MSG_DATA_ALERT_RULES_BEFORE_AFETR));
						}
					} else {
						for (int i = 0; i < entitiesList.size(); i++) {
							for (int j = 0; j < sensitiveDatabaseList.size(); j++) {
								Instance instance = (Instance) entitiesList
										.get(i);
								if (instance.getId() == sensitiveDatabaseList
										.get(j).getSrvId()) {
									entitiesModel.add(entitiesList.get(i));
									break;
								}
							}
						}
					}
				}
			} 	
			// entitiesModel = new ListModelList<>(entitiesList);
			entitiesModel.setMultiple(true);
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
	private boolean accessCheckFilterEnable;

	// privilegedUserAuditing
	private boolean filterEventsAccessChecked;

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

	public boolean isAccessCheckFilterEnable() {
		return accessCheckFilterEnable;
	}

	public void setAccessCheckFilterEnable(boolean accessCheckFilterEnable) {
		this.accessCheckFilterEnable = accessCheckFilterEnable;
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

	public static void showSpecifySQLServersDialog(Long instanceId) {
		if (instanceId == null) {
			throw new RuntimeException(" Instance Id must not be null! ");
		}
		Map<String, Object> args = new HashMap<>();
		args.put(INSTANCE_ID, instanceId);

		if (instanceId == 1) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifySQLServers, null,
					args);
			window.doHighlighted();
		}

		if (instanceId == 2) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyDatabase, null,
					args);
			window.doHighlighted();
		}

		if (instanceId == 3) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyTable, null, args);
			window.doHighlighted();
		}
		if (instanceId == 4) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyColumnName, null,
					args);
			window.doHighlighted();

		}

		if (instanceId == 5) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyAlertMessage,
					null, args);
			window.doHighlighted();
		}

		if (instanceId == 6) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyMailAddresses,
					null, args);
			window.doHighlighted();
		}
	}

	// SQLCM Req4.1.1.2 - Enhanced Instances View - End

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		if(entitiesModel.size()==0){
			if(ConditionID==1)
				dataDesc.setValue(ELFunctions.getLabel(SQLCMI18NStrings.MSG_DATA_ALERT_RULES_SENSITIVE_COLUMN));
			else
				dataDesc.setValue(ELFunctions.getLabel(SQLCMI18NStrings.MSG_DATA_ALERT_RULES_BEFORE_AFETR));
		}
		else{
			if (Sessions.getCurrent().getAttribute("RuleTypeAccess") != null) {
				String ruleTypeAccess = (String) Sessions.getCurrent()
						.getAttribute("RuleTypeAccess");
				if (ruleTypeAccess.equals("Sensitive column")) {
					dataDesc.setValue("Choose the Database for which you want to alert on sensitive column access.");
				} else {
					dataDesc.setValue("");
				}
			}
		}
		help = ServerVersion.SERVER_HELP_WEBSITE;
		optimizePerformanceLink = SQLCMConstants.SERVER_OPTIMIZE_AUDIT_PERFORMANCE;

		HashMap<String, Object> args = (HashMap<String, Object>) Executions
				.getCurrent().getArg();
		Long instanceId = (Long) args.get(INSTANCE_ID);

		instanceProperties = initInstanceProperties(instanceId);
		privilegedUserListModelList.addAll(instanceProperties
				.getPrivilegedRolesAndUsers().getRoleList());
		privilegedUserListModelList.addAll(instanceProperties
				.getPrivilegedRolesAndUsers().getUserList());
		thresholdsListModelList = CMThresholdAdapter
				.wrapThresholdsData(instanceProperties.getAuditThresholdsData()
						.getThresholdList());

		initAdvancedTabLists();
	}

	@Command("onItemClick")
	public void onItemClick(@BindingParam("id") int id) {
		instanceId = id;
		OKButton.setDisabled(false);
	}

	@Command("submitChoice")
	public void submitChoice() {
		Sessions.getCurrent().setAttribute("instanceId", instanceId);
		CMInstanceProperties instanceProperties = null;
		try {
			if (instanceId != 0) {
				instanceProperties = InstancesFacade
						.getInstanceProperties(instanceId);
			}
		} catch (RestException e) {
			WebUtil.showErrorBox(e,
					SQLCMI18NStrings.FAILED_TO_LOAD_AUDIT_INSTANCE_PROPERTIES);
		}
		if (instanceProperties != null) {
			String instanceName = instanceProperties.getGeneralProperties()
					.getInstance();
			Sessions.getCurrent().setAttribute("serverName", instanceName);
		}
		specifySQLServers.detach();
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

	protected CMInstance getInstance() {
		return instance;
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
}
