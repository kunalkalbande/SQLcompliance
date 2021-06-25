package com.idera.sqlcm.ui.eventFilters;

import com.idera.sqlcm.Utils;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMPermission;
import com.idera.sqlcm.entities.CMPermissionInfo;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.instances.CMInstanceProperties;
import com.idera.sqlcm.enumerations.DefaultDBPermission;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.converter.PermissionStatusToCssStyleConverter;
import com.idera.sqlcm.ui.converter.PermissionStatusToImagePathConverter;
import com.idera.sqlcm.ui.converter.PermissionStatusToLabelConverter;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.CMThresholdAdapter;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.AddAlertRulesSaveEntity;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.EventType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.permissionFailDialog.PermissionFailConfirmViewModel;
import com.idera.sqlcm.ui.eventFilters.SpecifyHostNameViewModel.Host;
import com.idera.sqlcm.ui.eventFilters.SpecifySessionLoginViewModel.Category;
import com.idera.sqlcm.ui.eventFilters.SpecifySessionLoginViewModel.SessionLogin;

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
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

public class SpecifySessionLoginViewModel implements
		AddPrivilegedUsersViewModel.DialogListener {
	private String help;
	EventFilterCondition eventCondition = new EventFilterCondition();
	EventField eventtype = new EventField();
	public String[] targetString = {};


	@Wire
	Radiogroup rgAlertRules;
	
	@Wire 
	Textbox objectNameMatch;
	
	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) throws RestException {
		help = "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
		Selectors.wireComponents(view, this, false);
		String strMatchString = (String) Sessions.getCurrent().getAttribute("sessionLoginMatchString");
		if (strMatchString != null && !strMatchString.isEmpty()){
			GetData();
		BindUtils.postNotifyChange(null, null, this, "*");}
	}
	
	
	
	public String getHelp() {
		return this.help;
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

	// 4.1.1.5
	public static enum Category {
		LISTED(1, ELFunctions.getLabel(SQLCMI18NStrings.LISTED)), // TODO AS ask
																	// .NET team
																	// id
		EXCEPT_LISTED(2, ELFunctions.getLabel(SQLCMI18NStrings.EXCEPT_LISTED));
		private String label;
		private int index;

		private Category(int index, String label) {
			this.label = label;
			this.index = index;

		}

		public String getLabel() {
			return label;
		}

		public String getName() {
			return this.name();
		}

		public int getIndex() {
			return index;
		}
	}

	private Category currentInterval = Category.LISTED;
	private ListModelList<Category> intervalListModelList;

	private void initIntervalList(int selectedIndex) {
		intervalListModelList = new ListModelList<>();
		intervalListModelList.add(Category.LISTED);
		intervalListModelList.add(Category.EXCEPT_LISTED);
		currentInterval = intervalListModelList.get(selectedIndex);
		intervalListModelList.setSelection(Arrays.asList(currentInterval));
	}

	@Command("selectAddEventFilter")
	public void selectAddEventFilter(
			@BindingParam("radioGroup") Radiogroup radioGroup)
			throws RestException {
		int iSelected = radioGroup.getSelectedIndex();
		initIntervalList(iSelected);
		Set<Category> selectedIntervals = intervalListModelList.getSelection();
		if (selectedIntervals != null && !selectedIntervals.isEmpty()) {
			for (Category i : selectedIntervals) {
				currentInterval = i;
				Sessions.getCurrent().setAttribute("specifySessionLoginRadio",
						currentInterval.label);
				break;
			}
		}
	}

	@Command("submitChoice")
	public void submitChoice(@BindingParam("comp") Window x) throws Exception {		

		if(currentInterval.getName().equals("LISTED"))
			eventCondition.set_inclusive(true);
		else
			eventCondition.set_inclusive(false);
		
		Sessions.getCurrent().setAttribute("specifySessionLoginRadio",currentInterval.label);
		
		
		eventCondition.set_blanks(false);
		eventCondition.set_nulls(false);
		eventCondition.set_boolValue(false);
		
		eventtype.setDataFormat(MatchType.String);
		eventtype.set_type(EventType.SqlServer);
		
		if (dataList != null && (!dataList.isEmpty())) {
			eventCondition.set_targetStrings(GetTargetString());
		}
		
		String matchString = eventCondition.UpdateMatchString(eventtype,
				eventCondition);
		Sessions.getCurrent().setAttribute("specifySessionLoginList", dataList);
		Sessions.getCurrent().setAttribute("specifySessionLoginMatchString", matchString);
		Sessions.getCurrent().setAttribute("sessionLoginMatchString",matchString);
		x.detach();
	}

	public String[] GetTargetString() {
		targetString = new String[dataList.getSize()];
		for (int j = 0; j < dataList.getSize(); j++) {
			targetString[j] = dataList.get(j).getSessionLoginName().toString();
		}
		return targetString;
	}

	private String sessionLoginNameMatch;

	public String getSessionLoginNameMatch() {
		return sessionLoginNameMatch;
	}

	public void setSessionLoginNameMatch(String sessionLoginNameMatch) {
		this.sessionLoginNameMatch = sessionLoginNameMatch;
	}

	private ListModelList<SessionLogin> dataList = new ListModelList<>();

	@Command
	@NotifyChange("dataList")
	public void addItem() {
		String sessionLoginNameMatch = this.sessionLoginNameMatch;
		if (sessionLoginNameMatch != null && (!sessionLoginNameMatch.isEmpty())) {
			boolean chkPass = true;
			for (int j = 0; j < dataList.getSize(); j++) {
				if (dataList.get(j).getSessionLoginName().toString()
						.equals(sessionLoginNameMatch)) {
					WebUtil.showInfoBoxWithCustomMessage("The list already contains "
							+ sessionLoginNameMatch);
					chkPass = false;
					break;
				}
			}
			if (chkPass) {
				SessionLogin sessionLogin = new SessionLogin(sessionLoginNameMatch);
				dataList.add(sessionLogin);				
				setSessionLoginNameMatch("");
				objectNameMatch.setValue("");
			}
		}
	}
	

	public ListModelList<SessionLogin> getDataList() {
		return dataList;
	}

	public void setDataList(ListModelList<SessionLogin> dataList) {
		this.dataList = dataList;
	}

	public class SessionLogin {
		String sessionLoginName;

		public String getSessionLoginName() {
			return sessionLoginName;
		}

		public void setSessionLoginName(String sessionLoginName) {
			this.sessionLoginName = sessionLoginName;
		}

		public SessionLogin(String sessionLoginName) {
			super();
			this.sessionLoginName = sessionLoginName;
		}
	}

	@Command("onItemClick")
	public void onItemClick() {
		enableRemoveButtonIfSelected();
	}

	private void enableRemoveButtonIfSelected() {
		Set selectedItems = dataList.getSelection();
		if (selectedItems != null && selectedItems.size() > 0) {
			// removeBtn.setDisabled(false);
		} else {
			// removeBtn.setDisabled(true);
		}
	}

	@Command("onRemoveBtnClick")
	public void onRemoveBtnClick() {
		Utils.removeAllSelectedItems(dataList);
		enableRemoveButtonIfSelected();
		BindUtils.postNotifyChange(null, null, this, "dataList");
	}
	
	public void GetData() {
		String strMatchString = (String) Sessions.getCurrent().getAttribute("sessionLoginMatchString");

		Map<String, String> EventNodeDataValue = new HashMap<String, String>();
		String active = strMatchString;
		int index = strMatchString.indexOf("(");

		try {
			while (index != -1) {
				String sKey, sValue;
				String length;

				sKey = active.substring(0, index);
				active = active.substring(index + 1);
				index = active.indexOf(")");
				length = active.substring(0, index);
				active = active.substring(index + 1);
				sValue = (active.subSequence(0, Integer.parseInt(length))
						.toString());
				active = (active.substring(Integer.parseInt(length)).toString());
				EventNodeDataValue.put(sKey, sValue);
				index = active.indexOf("(");
			}
		} catch (Exception e) {
			try {
				throw new Exception("Improperly formed KeyValue string.", e);
			} catch (Exception e1) {
				// TODO Auto-generated catch block
				e1.printStackTrace();
			}
		}
		if (active.length() > 0)
			try {
				throw new Exception("Improperly formed KeyValue string.");
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		
		String strBlanks = (String) EventNodeDataValue.get("blanks");
		if(strBlanks.equals("0"))
		{
			eventCondition.set_blanks(false);
		}
		else
		{
			eventCondition.set_blanks(true);
		}
		
		String strNulls = (String) EventNodeDataValue.get("nulls");
		if(strNulls.equals("0"))
		{
			eventCondition.set_nulls(false);
		}
		else
		{
			eventCondition.set_nulls(true);
		}
		
		String strInclusive = (String) EventNodeDataValue.get("include");
		if(strInclusive.equals("0"))
		{
			eventCondition.set_inclusive(false);
			rgAlertRules.setSelectedIndex(1);
		}
		else
		{
			eventCondition.set_inclusive(true);
		}
		
		String strCount = (String) EventNodeDataValue.get("count");
		for(int i = 0; i<Integer.parseInt(strCount); i++)
		{
			String sessionLoginNameMatch = (String) EventNodeDataValue.get("" + i);
			SessionLogin sessionLogin = new SessionLogin(sessionLoginNameMatch);
			setSessionLoginNameMatch(sessionLoginNameMatch);
			addItem();
		}
	}
}
