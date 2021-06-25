package com.idera.sqlcm.ui.instancesAlertsRule;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

import org.zkoss.zul.Textbox;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Window;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.EventType;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField.MatchType;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.permissionFailDialog.PermissionFailConfirmViewModel;
import com.idera.sqlcm.ui.eventFilters.EventFilterCondition;


public class SpecifyPrivilegedUserViewModel implements
AddPrivilegedUsersViewModel.DialogListener {

	private String help;
	EventFilterCondition eventCondition = new EventFilterCondition();
	EventField eventtype = new EventField();
	public String[] targetString = {};
	@Wire
	Radiogroup rgAlertRules;
	
	@Wire
	Textbox objectNameMatchs;
	

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
				Sessions.getCurrent().setAttribute("specifyLoginRadio",
						currentInterval.label);
				break;
			}
		}
	}

	@Command("submitChoice")
	public void submitChoice(@BindingParam("comp") Window x) throws Exception {
		
		eventCondition.set_blanks(false);
		eventCondition.set_nulls(false);
		eventCondition.set_boolValue(true);
		eventtype.setDataFormat(MatchType.String);
		eventtype.set_type(EventType.SqlServer);
		eventCondition.set_inclusive(rgAlertRules.getSelectedIndex() == 0 ? true
				: false);
		if (dataLists != null && (!dataLists.isEmpty())) {
			eventCondition.set_targetStrings(GetTargetString());
		}
		String matchString = eventCondition.UpdateMatchString(eventtype, eventCondition);
		Sessions.getCurrent().setAttribute("specifyLoginList",dataLists);
		Sessions.getCurrent().setAttribute("privilegedUserNameMatch",matchString);
		x.detach();
	}

	public String[] GetTargetString() {
		targetString = new String[dataLists.getSize()];
		for (int j = 0; j < dataLists.getSize(); j++) {
			targetString[j] = dataLists.get(j).getPrivilegedUserName().toString();
		}
		return targetString;
	}

	private String privilegedUserNameMatch;

	public String getPrivilegedUserNameMatch() {
		return privilegedUserNameMatch;
	}

	public void setPrivilegedUserNameMatch(String privilegedUserNameMatch) {
		this.privilegedUserNameMatch = privilegedUserNameMatch;
	}

	private ListModelList<PrivilegedUserName> dataLists = new ListModelList<>();

	@Command
	@NotifyChange("dataList")
	public void addItem() {
		String privilegedUserNameMatch = this.privilegedUserNameMatch;
		if (privilegedUserNameMatch != null && (!privilegedUserNameMatch.isEmpty())) {
			boolean chkPass = true;
			for (int j = 0; j < dataLists.getSize(); j++) {
				if (dataLists.get(j).getPrivilegedUserName().toString()
						.equals(privilegedUserNameMatch)) {
					WebUtil.showInfoBoxWithCustomMessage("The list already contains "
							+ privilegedUserNameMatch);
					chkPass = false;
					break;
				}
			}
			if (chkPass) {
				PrivilegedUserName privilegedUser = new PrivilegedUserName(privilegedUserNameMatch);
				dataLists.add(privilegedUser);
				setPrivilegedUserNameMatch("");
				objectNameMatchs.setValue("");
			}
		}
	}
	

	public ListModelList<PrivilegedUserName> getDataLists() {
		return dataLists;
	}

	public void setDataLists(ListModelList<PrivilegedUserName> dataLists) {
		this.dataLists = dataLists;
	}

	public class PrivilegedUserName {
		String privilegedUserName;

		

		public String getPrivilegedUserName() {
			return privilegedUserName;
		}

		public void setPrivilegedUserName(String privilegedUserName) {
			this.privilegedUserName = privilegedUserName;
		}



		public PrivilegedUserName(String privilegedUserName) {
			super();
			this.privilegedUserName = privilegedUserName;
		}
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


	@Command("onRemoveBtnClick")
	public void onRemoveBtnClick() {
		Utils.removeAllSelectedItems(dataLists);
		BindUtils.postNotifyChange(null, null, this, "dataList");
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) throws RestException {
		Selectors.wireComponents(view, this, false);
		String strMatchString = (String) Sessions.getCurrent().getAttribute("privilegedUserNameMatch");
		if (strMatchString != null && !strMatchString.isEmpty()){
			GetData();
			BindUtils.postNotifyChange(null, null, this, "*");}
	}

	public void GetData() {
		String strMatchString = (String) Sessions.getCurrent().getAttribute("privilegedUserNameMatch");

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
			String privilegedUserMatch = (String) EventNodeDataValue.get("" + i);
			PrivilegedUserName previleged = new PrivilegedUserName(privilegedUserMatch);
			setPrivilegedUserNameMatch(privilegedUserMatch);
			addItem();
		}
	}
}
