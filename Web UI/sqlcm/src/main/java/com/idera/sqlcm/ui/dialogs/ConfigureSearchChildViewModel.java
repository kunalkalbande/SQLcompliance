package com.idera.sqlcm.ui.dialogs;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import javax.servlet.http.HttpSession;

import org.apache.commons.lang.StringUtils;
import org.springframework.web.util.WebUtils;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Label;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import bsh.StringUtil;

import com.idera.common.Utility;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.ProfilerObject;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;

public class ConfigureSearchChildViewModel {
	public static final String ZUL_URL = "~./sqlcm/instancedetails/configure-search-child.zul";
	public static final String OPEN_PROFILE_CHILD = "open-profile";
	public static final String DELETE_PROFILE_CHILD = "delete-profile";
	public static final String SAVE_NEW_PROFILE_CHILD = "save-new-profile";

	@Wire
	private Label titleLabel;
	@Wire
	private Listbox listboxOpenDeleteProfile;
	@Wire
	private Grid listboxSaveNewProfile;
	@Wire
	private Button btnDeleteProfile;
	@Wire
	private Button btnOpenProfile;
	@Wire
	private Button btnOpenDeleteCancel;
	@Wire
	private Button btnRestoreDefault;
	@Wire
	private Button btnSaveNewProfileCancel;
	@Wire
	private Button btnSaveNewProfiles;
	@Wire
	private Textbox txtNewProfileName;
	@Wire
	private Combobox comboboxOpenDeleteProfile;

	public static CMInstance instanceDetail;
	
	public static List<String> profileList;

	public static String activeProfile;

	public static String pageStatus;

	public static List<ProfilerObject> newProfileList;

	public List<String> uniqueProfileList;

	public static List<ProfilerObject> getNewProfileList() {
		return newProfileList;
	}

	public static CMInstance getInstanceDetail() {
		return instanceDetail;
	}

	public static void setInstanceDetail(CMInstance instanceDetail) {
		ConfigureSearchChildViewModel.instanceDetail = instanceDetail;
	}

	public static void setNewProfileList(List<ProfilerObject> newProfileList) {
		ConfigureSearchChildViewModel.newProfileList = newProfileList;
	}

	public static String getActiveProfile() {
		return activeProfile;
	}

	public static void setActiveProfile(String activeProfile) {
		ConfigureSearchChildViewModel.activeProfile = activeProfile;
	}

	public List<String> getUniqueProfileList() {
		return uniqueProfileList;
	}

	public void setUniqueProfileList(List<String> uniqueProfileList) {
		this.uniqueProfileList = uniqueProfileList;
	}

	public static List<String> getProfileList() {
		return profileList;
	}

	public static void setProfileList(List<String> profileList) {
		ConfigureSearchChildViewModel.profileList = profileList;
	}

	public static String getPageStatus() {
		return pageStatus;
	}

	public static void setPageStatus(String pageStatus) {
		ConfigureSearchChildViewModel.pageStatus = pageStatus;
	}

	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
		ConfigureSearchViewModel.showConfigeSearch(instanceDetail);
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		if (null != getProfileList() || !getProfileList().isEmpty())
			setUniqueProfileList(getProfileList());
		if (null != getActiveProfile() || !"".equals(getActiveProfile())) {
			comboboxOpenDeleteProfile.setValue(getActiveProfile());
			btnDeleteProfile.setDisabled(false);
			btnOpenProfile.setDisabled(false);
		} else {
			btnDeleteProfile.setDisabled(true);
			btnOpenProfile.setDisabled(true);
		}
		if (DELETE_PROFILE_CHILD.equals(getPageStatus()))
			showDeleteProfile();
		else if (OPEN_PROFILE_CHILD.equals(getPageStatus()))
			showOpenProfile();
		else if (SAVE_NEW_PROFILE_CHILD.equals(getPageStatus()))
			showNewProfile();

	}

	public static void showConfigureSearchChild(String pageStatus,
			List<String> uniqueProfileList, String activeProfileName,
			List<ProfilerObject> newProfile) {
		setPageStatus(pageStatus);
		if (null != uniqueProfileList) {
			setProfileList(uniqueProfileList);
		} else {
			List<String> falseString = new ArrayList<String>();
			falseString.add("");
			setProfileList(falseString);
		}
		if (null != activeProfileName)
			setActiveProfile(activeProfileName);
		else
			setActiveProfile("");
		if (null != newProfile) {
			setNewProfileList(newProfile);
		}
		Window window = (Window) Executions.createComponents(ZUL_URL, null,
				null);
		window.doHighlighted();
	}

	public void showOpenProfile() {
		titleLabel.setValue("Open Search Profile");
		listboxOpenDeleteProfile.setVisible(true);
		listboxSaveNewProfile.setVisible(false);
		btnDeleteProfile.setVisible(false);
		btnRestoreDefault.setVisible(false);
		btnOpenProfile.setVisible(true);
	}

	public void showDeleteProfile() {
		titleLabel.setValue("Delete Search Profile");
		listboxOpenDeleteProfile.setVisible(true);
		listboxSaveNewProfile.setVisible(false);
		btnDeleteProfile.setVisible(true);
		btnRestoreDefault.setVisible(true);
		btnOpenProfile.setVisible(false);
	}

	public void showNewProfile() {
		titleLabel.setValue("Save New Search Profile");
		listboxOpenDeleteProfile.setVisible(false);
		listboxSaveNewProfile.setVisible(true);
	}

	@Command("editNewProfileName")
	public void editNewProfileName(@BindingParam("id") String text) {
		String profileName = text.trim();
		if ( !"".equals(profileName)) {			
			btnSaveNewProfiles.setDisabled(false);
		} else {
			btnSaveNewProfiles.setDisabled(true);
		}
	}

	@Command("editProfileListCombobox")
	public void editProfileListCombobox(@BindingParam("id") String text) {
		if (containsCaseInsensitive(text, getProfileList())) {
			text = getProfileNameMatches(text, getProfileList());
			comboboxOpenDeleteProfile.setValue(text);
			comboboxOpenDeleteProfile.setText(text);
			btnOpenProfile.setDisabled(false);
			btnDeleteProfile.setDisabled(false);
		} else {
			btnOpenProfile.setDisabled(true);
			btnDeleteProfile.setDisabled(true);
		}
	}

	@Command("saveNewProfileCommand")
	public void saveNewProfileCommand(@BindingParam("comp") Window x) {
		try {
			String profileName = txtNewProfileName.getValue().toUpperCase().trim();
			List<String> uniqueProfileList=getUniqueProfileList();
			if(!uniqueProfileList.contains(profileName))
			{			
			
				List<ProfilerObject> newProfile = getNewProfileList();
				Iterator<ProfilerObject> itr = newProfile.iterator();
				while (itr.hasNext()) {
					itr.next().setProfileName(profileName);
				}
				DatabasesFacade.createProfile(newProfile);
			}
			else
			{
				WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_PROFILE_NAME);
			}
		} catch (Exception e) {
			e.getStackTrace();
		}
		x.detach();
		ConfigureSearchViewModel.showConfigeSearch(instanceDetail);
	}

	@Command("openProfileCommand")
	public void openProfileCommand(@BindingParam("comp") Window x) {
		try {
			x.detach();
			DatabasesFacade.activateProfile(comboboxOpenDeleteProfile
					.getValue());
			ConfigureSearchViewModel.setInstanceDetail(instanceDetail);
			ConfigureSearchViewModel.showConfigeSearch(instanceDetail);
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	@Command("deleteProfileCommand")
	public void deleteProfileCommand(@BindingParam("comp") Window x) {
		try {
			x.detach();
			String profile = comboboxOpenDeleteProfile.getValue();
			if (WebUtil.showConfirmationBoxWithIcon(Utility.getMessage(
					SQLCMI18NStrings.COLUMN_SEARCH_PROFILE_DELETE_CONFIRM,
					profile), ELFunctions
					.getLabel(SQLCMI18NStrings.WARNING),
					"/images/warning_white.svg", false, (Object) null)) {
				DatabasesFacade.deleteProfile(comboboxOpenDeleteProfile
						.getValue());
				ConfigureSearchViewModel.showConfigeSearch(instanceDetail);
			}
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	@Command("restoreDefaultCommand")
	public void restoreDefaultCommand(@BindingParam("comp") Window x) {
		try {
			if (WebUtil.showConfirmationBoxWithIcon(Utility.getMessage(
					SQLCMI18NStrings.COLUMN_SEARCH_RESET_PROFILE,
					SQLCMI18NStrings.COLUMN_SEARCH_TITLE), ELFunctions
					.getLabel(SQLCMI18NStrings.WARNING),
					"/images/warning_white.svg", false, (Object) null)) {
				DatabasesFacade.resetData();
				ConfigureSearchViewModel.showConfigeSearch(instanceDetail);
			}
			x.detach();
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	public boolean containsCaseInsensitive(String editedText,
			List<String> profileList) {
		for (String profileName : profileList) {
			if (profileName.equalsIgnoreCase(editedText)) {
				return true;
			}
		}
		return false;
	}

	public String getProfileNameMatches(String editedText,
			List<String> profileList) {
		for (String profileName : profileList) {
			if (profileName.equalsIgnoreCase(editedText)) {
				return profileName;
			}
		}
		return "";
	}

	@Command("closeCommand")
	public void closeCommand(@BindingParam("comp") Window x) {
		x.detach();
		ConfigureSearchViewModel.showConfigeSearch(instanceDetail);
	}
}
