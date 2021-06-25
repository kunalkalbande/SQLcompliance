package com.idera.sqlcm.ui.eventFilters;

import com.idera.sqlcm.beEntities.AuditedInstanceBE;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.Instance;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.instances.CMInstanceProperties;
import com.idera.sqlcm.enumerations.DefaultDBPermission;
import com.idera.sqlcm.facade.EventFiltersFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.server.web.session.SessionUtil;
import com.idera.sqlcm.ui.components.filter.FilterData;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.CMThresholdAdapter;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.permissionFailDialog.PermissionFailConfirmViewModel;
import com.idera.sqlcm.ui.dialogs.eventFilterWizard.EventFilterSaveEntity;
import com.idera.sqlcm.utils.SQLCMConstants;

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
import org.zkoss.zk.ui.select.Selectors;
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
	public static final String ZUL_URL_specifySQLServers = "~./sqlcm/eventFiltersView/specifySQLServers.zul";
	public static final String ZUL_URL_specifyObjects = "~./sqlcm/eventFiltersView/specifyObjects.zul";
	public static final String ZUL_URL_specifyDatabase = "~./sqlcm/eventFiltersView/specifyDatabase.zul";
	public static final String ZUL_URL_specifyAppName = "~./sqlcm/eventFiltersView/specifyAppName.zul";
	public static final String ZUL_URL_specifyHostName = "~./sqlcm/eventFiltersView/specifyHostName.zul";
	public static final String ZUL_URL_specifyLoginName = "~./sqlcm/eventFiltersView/specifyLoginName.zul";
	public static final String ZUL_URL_specifySessionLogin = "~./sqlcm/eventFiltersView/specifySessionLogin.zul";
	public static final int INVALID_CHECKS_VALUE = -1;
	protected CommonFacade entityFacade;
	protected Map<String, Object> filterRequest = new TreeMap<>();
	public Map<String, Object> targetInstances = new HashMap<String, Object>();

	public static final String INSTANCE_ID = "instance-id";
	private CMInstance instance;
	private EventFilterSaveEntity eventFilterSaveEntity = new EventFilterSaveEntity();
	private EventFiltersFacade eventFiltersFacade = new EventFiltersFacade();


	// Audited Activities
	public SpecifySQLServerViewModel() {
		super();
		entityFacade = new InstancesFacade();
		refreshEntitiesList();
	}
	
	protected void refreshEntitiesList() {
		entitiesList = eventFiltersFacade.getAllEntitiesEventFilter(filterRequest);
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
			if(Sessions.getCurrent().getAttribute("SQL Server")!=null 
					&& Sessions.getCurrent().getAttribute("SQL Server").toString().isEmpty()){
				Sessions.getCurrent().removeAttribute("SQL Server");
			}
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
	private String optimizePerformanceLink;

	private CMInstanceProperties instanceProperties = new CMInstanceProperties();
	private ListModelList<DefaultDBPermission> dbPermissionListModelList;

	private ListModelList<CMInstancePermissionBase> privilegedUserListModelList = new ListModelList();

	private boolean accessCheckFilterEnable;

	private boolean filterEventsAccessChecked;

	private ListModelList<CMEntity> selectedEntities;
	protected List<AuditedInstanceBE> entitiesList;
	protected ListModelList<AuditedInstanceBE> entitiesModel;
	protected ListModelList<Filter> filtersModel;
	protected FilterData filterData;

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
					SpecifySQLServerViewModel.ZUL_URL_specifyAppName, null,
					args);
			window.doHighlighted();
		}
		
		if (instanceId == 5) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyLoginName, null,
					args);
			window.doHighlighted();
		}
		
		if (instanceId == 6) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifyHostName, null,
					args);
			window.doHighlighted();
		}
		
		if (instanceId == 7) {
			Window window = (Window) Executions.createComponents(
					SpecifySQLServerViewModel.ZUL_URL_specifySessionLogin, null,
					args);
			window.doHighlighted();
		}
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		help = "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
		Selectors.wireComponents(view, this, false);
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
		String instanceList = "";
		int temp = 1;
		while (itr.hasNext()) {
			iteratorID = (Integer) itr.next();
			try {
				instance = InstancesFacade.getInstanceDetails(iteratorID);
				eventFilterSaveEntity.setTargetInstances(instance.getInstance());
				if(temp>1)
					instanceList +=";";
				instanceList += instance.getInstance();
				targetInstances.put(instance.getInstance(), instance);
				temp++;
			} catch (RestException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		Sessions.getCurrent().setAttribute("Instances",targetInstances);
		Sessions.getCurrent().setAttribute("SQL Server", instanceList);
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
