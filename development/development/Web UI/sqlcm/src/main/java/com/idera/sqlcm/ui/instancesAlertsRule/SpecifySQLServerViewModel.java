package com.idera.sqlcm.ui.instancesAlertsRule;

import com.idera.sqlcm.beEntities.AuditedInstanceBE;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.instances.CMInstanceProperties;
import com.idera.sqlcm.enumerations.DefaultDBPermission;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.server.web.session.SessionUtil;
import com.idera.sqlcm.ui.components.filter.FilterData;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.CMThresholdAdapter;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.permissionFailDialog.PermissionFailConfirmViewModel;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

public class SpecifySQLServerViewModel implements
		AddPrivilegedUsersViewModel.DialogListener {
	public static final String ZUL_URL_specifySQLServers = "~./sqlcm/instanceAlertsRule/specifySQLServers.zul";
	public static final String ZUL_URL_specifyObjects = "~./sqlcm/instanceAlertsRule/specifyObjects.zul";
	public static final String ZUL_URL_specifyDatabase = "~./sqlcm/instanceAlertsRule/specifyDatabase.zul";
	public static final String ZUL_URL_specifyAppName = "~./sqlcm/instanceAlertsRule/specifyAppName.zul";
	public static final String ZUL_URL_specifyHostName = "~./sqlcm/instanceAlertsRule/specifyHostName.zul";
	public static final String ZUL_URL_specifyLoginName = "~./sqlcm/instanceAlertsRule/specifyLoginName.zul";
	public static final String ZUL_URL_specifyAlertMessage = "~./sqlcm/instanceAlertsRule/specifyAlertMessage.zul";
	public static final String ZUL_URL_specifyMailAddresses = "~./sqlcm/instanceAlertsRule/specifyMailAddresses.zul";
	public static final String ZUL_URL_excludeCertainEventType =  "~./sqlcm/instanceAlertsRule/excludeCertainEventType.zul";
	public static final String ZUL_URL_specifyPrivilegedUsers = "~./sqlcm/instanceAlertsRule/specifyPrivilegedUsers.zul";
	public static final String ZUL_URL_specifyRowCountThreshold = "~./sqlcm/instanceAlertsRule/specifyRowCountThreshold.zul";
	
	public static final int INVALID_CHECKS_VALUE = -1;
	protected CommonFacade entityFacade;
	protected Map<String, Object> filterRequest = new TreeMap<>();
	public Map<String, Object> targetInstances = new HashMap<String, Object>();

	public static final String INSTANCE_ID = "instance-id";
	private CMInstance instance;
	private AddAlertRulesSaveEntity addAlertRulesSaveEntity = new AddAlertRulesSaveEntity();
	private InstancesFacade instancesFacade = new InstancesFacade();

	// Audited Activities
	public SpecifySQLServerViewModel() {
		super();
		entityFacade = new InstancesFacade();
		refreshEntitiesList();
	}

	protected void refreshEntitiesList() {
		entitiesList = instancesFacade.getAllEntitiesInstances(filterRequest);
		if(Sessions.getCurrent().getAttribute("SQL Server")!= null && 
				!Sessions.getCurrent().getAttribute("SQL Server").toString().isEmpty()){
		String instanceName = (String)Sessions.getCurrent().getAttribute("SQL Server");
		String[] strArray;
		strArray = instanceName.split(";");
		for (AuditedInstanceBE iterable : entitiesList) {
			for(int i = 0; i < strArray.length; i++){
				if(strArray[i].equals(iterable.getInstance().toString())){
					iterable.setCheckBool(true);					
	 				ids.add((int)iterable.getId());
				}
			 }
		   }
		}
		verifyEntitiesList();
		BindUtils.postNotifyChange(null, null, this, "entitiesModel");
	}

	protected void verifyEntitiesList() {
		if (entitiesList == null) {
			entitiesModel = new ListModelList<>();

		} else {
			entitiesModel = new ListModelList<>(entitiesList);
			entitiesModel.setMultiple(true);
		}
	}

	@Wire("#include_advancedTab #charactersLimit")
	Textbox charactersLimit;

	@Wire("#include_privilegedUserAuditingTab #removePrivilegedUserButton")
	Button removePrivilegedUserButton;

	@Wire("#include_generalTab #auditSettingsStatus")
	Textbox auditSettingsStatus;

	@Wire
	protected Listbox entitiesListBox;

	private String help;
	private CMInstanceProperties instanceProperties;
	private ListModelList<DefaultDBPermission> dbPermissionListModelList;

	private ListModelList<CMInstancePermissionBase> privilegedUserListModelList = new ListModelList<CMInstancePermissionBase>();

	private boolean accessCheckFilterEnable;

	private boolean filterEventsAccessChecked;
	private ListModelList<CMEntity> selectedEntities;
	protected List<AuditedInstanceBE> entitiesList;
	protected ListModelList<AuditedInstanceBE> entitiesModel;
	protected ListModelList<Filter> filtersModel;
	protected FilterData filterData;	

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

	public ListModelList<AuditedInstanceBE> getEntitiesModel() {
		return entitiesModel;
	}

	protected ListModelList<Filter> getFiltersDefinition() {
		return new ListModelList<>();
	}

	public ListModelList<Filter> getFiltersModel() {
		return filtersModel;
	}

	public List<AuditedInstanceBE> getEntitiesList() {
		return entitiesList;
	}

	public void setEntitiesList(List<AuditedInstanceBE> entitiesList) {
		this.entitiesList = entitiesList;
	}

	// SQLCM Req4.1.1.2 - Enhanced Instances View - Start
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
					SpecifySQLServerViewModel.ZUL_URL_specifyObjects, null,
					args);
			window.doHighlighted();
		}

		if (instanceId == 4) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyHostName, null,
					args);
			window.doHighlighted();
		}

		if (instanceId == 5) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyAppName, null,
					args);
			window.doHighlighted();
		}

		if (instanceId == 6) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyLoginName, null,
					args);
			window.doHighlighted();
			}
		if (instanceId == 7) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyAlertMessage, null,
					args);
			window.doHighlighted();
			}
		if (instanceId == 8) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyMailAddresses, null,
					args);
			window.doHighlighted();
			}
		if (instanceId == 9) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_excludeCertainEventType, null,
					args);
			window.doHighlighted();
			}
		if (instanceId == 10) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyPrivilegedUsers, null,
					args);
			window.doHighlighted();
			}
		if (instanceId == 11) { 
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyRowCountThreshold, null,
					args);
			window.doHighlighted();
			}
	}

	// SQLCM Req4.1.1.2 - Enhanced Instances View - End

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		help = "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
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

	private Integer iteratorID;

	@Command("submitChoice")
	public void submitChoice(@BindingParam("id") int id,@BindingParam("comp") Window x) {
		Iterator itr = ids.iterator();
		String targetInstancesString="";
		int temp=1;
		while (itr.hasNext()) {
			iteratorID = (Integer) itr.next();
			try {
				instance = InstancesFacade.getInstanceDetails(iteratorID);
				addAlertRulesSaveEntity.setTargetInstances(instance.getInstance());
				targetInstances.put(instance.getInstance(), instance);
				if(temp>1)
					targetInstancesString += ";";
				targetInstancesString += instance.getInstance();
				temp++;
			} catch (RestException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		Sessions.getCurrent().setAttribute("Instances",targetInstances);
		Sessions.getCurrent().setAttribute("SQL Server",targetInstancesString);
		x.detach();
	} 
	
	ArrayList<Integer> ids = new ArrayList<Integer>();

	@Command("oncheck")
	public void oncheck(@BindingParam("id") int id) {
		if (ids.contains(id)) {
			Iterator<Integer> itr = ids.iterator(); // remove all even numbers
			while (itr.hasNext()) {
				Integer number = itr.next();
				if (number == id) {
					itr.remove();
				}
			}
		} else {
			ids.add(id);
		}
	}
	
	
}
