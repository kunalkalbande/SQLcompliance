package com.idera.sqlcm.ui.dialogs;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Iterator;
import java.util.List;

import javax.servlet.http.HttpSession;

import mazz.i18n.annotation.I18NMessage;

import org.apache.commons.lang.StringUtils;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zul.Label;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Div;
import org.zkoss.zul.Image;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Listitem;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;
import org.zkoss.zul.impl.LabelElement;

import com.idera.common.Utility;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.ProfilerObject;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.*;

public class ConfigureSearchViewModel {
	public static final String ZUL_URL = "~./sqlcm/instancedetails/ConfigureSearch.zul";
	public static final String HIGH_IMAGE = "~./sqlcm/images/status-high-48x48.png";
	public static final String OPEN_PROFILE = "open-profile";
	public static final String DELETE_PROFILE = "delete-profile";
	public static final String SAVE_NEW_PROFILE = "save-new-profile";
	@Wire
	private Button btnDiscard;
	@Wire
	private Button btnEditString;
	@Wire
	private Button btnNewString;
	@Wire
	private Button btnDeleteString;
	@Wire
	private Button btnClose;
	@Wire
	private Button btnDeleteProfile;
	@Wire
	private Button btnOpenProfile;
	@Wire
	private Button btnSaveNewProfile;
	@Wire
	private Button btnSaveProfile;
	@Wire
	private Button btnSaveUpdatedString;
	@Wire
	private Button btnSaveNewString;
	@Wire
	private Div newStringGrid;
	@Wire
	private Div profileListGrid;
	@Wire
	private Div editStringGrid;
	@Wire
	private Listbox profileDataListbox;
	@Wire
	private Listbox newStringListbox;
	@Wire
	private Listbox editItemListbox;
	@Wire
	private Listbox newDefinitionStringListboxError;
	@Wire
	private Listbox editDefinitionStringListboxError;
	@Wire
	private Textbox editSearchStringName;
	@Wire
	private Textbox editDefinition;
	@Wire
	private Label activeProfileSearch;
	@Wire
	private Textbox newSearchStringName;
	@Wire
	private Listitem editListItems;
	@Wire
	private Listitem profileListItems;
	@Wire
	private Combobox editCategory;
	@Wire
	private Combobox newCategory;
	@Wire
	private Textbox newDefinition;
	@Wire
	private Image newDefinitionStringError;
	@Wire
	private Image editDefinitionStringError;
	@Wire
	private A selecDeselectAllLink;

	public List<ProfilerObject> profileList;

	public String activeProfile;

	public static CMInstance instanceDetail;

	public List<ProfilerObject> activeProfileList;

	public List<ProfilerObject> intactProfileDetails;

	public List<ProfilerObject> singleProfileDetailsList;

	public List<String> uniqueCategoryList;

	public List<String> uniqueProfileList;

	public ProfilerObject selectedRowList;

	public List<ProfilerObject> getIntactProfileDetails() {
		return intactProfileDetails;
	}

	public void setIntactProfileDetails(
			List<ProfilerObject> intactProfileDetails) {
		this.intactProfileDetails = intactProfileDetails;
	}

	public List<ProfilerObject> getActiveProfileList() {
		return activeProfileList;
	}

	public void setActiveProfileList(List<ProfilerObject> activeProfileList) {
		this.activeProfileList = activeProfileList;
	}

	public String getActiveProfile() {
		return activeProfile;
	}

	public void setActiveProfile(String activeProfile) {
		this.activeProfile = activeProfile;
	}

	public List<ProfilerObject> getSingleProfileDetailsList() {
		return singleProfileDetailsList;
	}

	public void setSingleProfileDetailsList(
			List<ProfilerObject> singleProfileDetailsList) {
		this.singleProfileDetailsList = singleProfileDetailsList;
	}

	public List<String> getUniqueProfileList() {
		return uniqueProfileList;
	}

	public void setUniqueProfileList(List<String> uniqueProfileList) {
		this.uniqueProfileList = uniqueProfileList;
	}

	public ProfilerObject getSelectedRowList() {
		return selectedRowList;
	}

	public void setSelectedRowList(ProfilerObject selectedProfile) {
		this.selectedRowList = selectedProfile;
	}

	public List<String> getUniqueCategoryList() {
		return uniqueCategoryList;
	}

	public void setUniqueCategoryList(List<String> uniqueCategoryList) {
		this.uniqueCategoryList = uniqueCategoryList;
	}

	public List<ProfilerObject> getProfileList() {
		return profileList;
	}

	public void setProfileList(List<ProfilerObject> profileList) {
		this.profileList = profileList;
	}

	public static CMInstance getInstanceDetail() {
		return instanceDetail;
	}

	public static void setInstanceDetail(CMInstance instanceDetail) {
		ColumnSearchViewModel.instanceDetail = instanceDetail;
	}

	public static void showConfigeSearch(CMInstance instance) {
		ConfigureSearchViewModel.setInstanceDetail(instance);
		Window window = (Window) Executions.createComponents(ZUL_URL, null,
				null);
		window.doHighlighted();
	}

	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		getSelectedProfile();
		getProfileDetails();
	}

	public void getSelectedProfile() {
		try {
			String activeProfile = "";
			activeProfile = DatabasesFacade.getActiveProfile();
			if ("".equals(activeProfile)) {
				activeProfileSearch.setValue("None Selected");
				btnSaveNewProfile.setDisabled(true);
			} else {
				setActiveProfile(activeProfile);
				activeProfileSearch.setValue(activeProfile);
				btnSaveNewProfile.setDisabled(false);
				activeProfileSearch.setTooltiptext(activeProfile);
			}
		} catch (Exception e) {
			e.getStackTrace();
		}
		BindUtils.postNotifyChange(null, null, ConfigureSearchViewModel.this,
				"*");
	}

	public void getProfileDetails() {
		try {
			List<ProfilerObject> list;
			list = DatabasesFacade.getProfileDetails();
			setProfileList(list);
			List<String> categoryList = new ArrayList();
			List<String> profileList = new ArrayList();
			List<ProfilerObject> activeProfileList = new ArrayList();
			List<ProfilerObject> singleProfileDetailsList = new ArrayList();
			Iterator<ProfilerObject> itr = list.iterator();
			while (itr.hasNext()) {
				ProfilerObject po = itr.next();
				if (!categoryList.contains(po.getCategory())) {
					categoryList.add(po.getCategory());
				}
				if (!profileList.contains(po.getProfileName())) {
					profileList.add(po.getProfileName());
				}
				if (null != getActiveProfile()) {
					if (po.getProfileName()
							.equalsIgnoreCase(getActiveProfile())) {
						activeProfileList.add(po);
					}
				} else {
					if (!singleProfileDetailsList.contains(po))
						singleProfileDetailsList.add(po);
				}

			}
			if (null == getActiveProfile()) {
				setSingleProfileDetailsList(manipulateSingleProfileDetailsList(singleProfileDetailsList));
				btnSaveProfile.setDisabled(true);
			} else {
				setSingleProfileDetailsList(activeProfileList);
				setActiveProfileList(activeProfileList);
			}
			setUniqueCategoryList(categoryList);
			if (profileList.contains(null) || profileList.isEmpty()) {
				btnDeleteProfile.setDisabled(true);
				btnOpenProfile.setDisabled(true);
			} else {
				setUniqueProfileList(profileList);
				if (profileList.size() > 0 && activeProfileSearch.getValue().equals("None Selected"))
					btnOpenProfile.setDisabled(false);
				else if(profileList.size() > 1)
					btnOpenProfile.setDisabled(false);
				else
					btnOpenProfile.setDisabled(true);
				btnDeleteProfile.setDisabled(false);
			}
			btnDiscard.setDisabled(true);
			setIntactProfileDetails(getSingleProfileDetailsList());
			selecDeselectAllLink.setLabel(selecDeselectAllStatus());
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	public List<ProfilerObject> manipulateSingleProfileDetailsList(
			List<ProfilerObject> singleProfileDetailsList) {
		Iterator<ProfilerObject> profileDetailsIterator = singleProfileDetailsList
				.iterator();
		while (profileDetailsIterator.hasNext()) {
			ProfilerObject po = profileDetailsIterator.next();
			po.setIsStringChecked(false);
		}
		return singleProfileDetailsList;
	}

	@Command("newStringCommand")
	public void newString() {
		newCategory.setValue("");
		newSearchStringName.setValue("");
		newDefinition.setValue("");
		btnDiscard.setDisabled(false);
		btnNewString.setDisabled(true);
		btnEditString.setDisabled(true);
		btnDeleteString.setDisabled(true);
		btnSaveProfile.setDisabled(true);
		newStringGrid.setVisible(true);
		profileListGrid.setVisible(false);
		btnSaveUpdatedString.setVisible(false);
		btnSaveNewString.setVisible(true);
		btnSaveNewProfile.setDisabled(true);
		btnClose.setDisabled(true);
		btnDeleteProfile.setDisabled(true);
		btnOpenProfile.setDisabled(true);
		newDefinitionStringError.setVisible(false);
		newDefinitionStringListboxError.setVisible(false);
		selecDeselectAllLink.setDisabled(true);
		newDefinition.setStyle("align:left;border:1px solid #e6e6e6;text-transform:lowercase;");
	}

	@Command("discardCommand")
	public void discard() {
		btnDiscard.setDisabled(true);
		btnNewString.setDisabled(false);
		btnSaveUpdatedString.setDisabled(true);
		btnSaveUpdatedString.setVisible(true);
		btnClose.setDisabled(false);
		btnSaveNewString.setVisible(false);
		editStringGrid.setVisible(false);
		newStringGrid.setVisible(false);
		profileListGrid.setVisible(true);
		selecDeselectAllLink.setDisabled(false);
		if (profileDataListbox.getSelectedCount() > 0) {
			btnEditString.setDisabled(false);
			btnDeleteString.setDisabled(false);
		}
		if (null == getUniqueProfileList() || getUniqueProfileList().isEmpty()) {
			btnDeleteProfile.setDisabled(true);
			btnOpenProfile.setDisabled(true);
		} else {
			btnOpenProfile.setDisabled(false);
			btnDeleteProfile.setDisabled(false);
		}
		if (null != getActiveProfile() || "".equals(getActiveProfile())) {
			btnSaveNewProfile.setDisabled(false);
		} else {
			btnSaveNewProfile.setDisabled(true);
		}
		if (compareProfilerObjectList(getIntactProfileDetails(),
				currentProfileList()))
			btnSaveProfile.setDisabled(true);
		else
			btnSaveProfile.setDisabled(false);
	}

	@Command("selectedRow")
	public void selectedRow() {
		btnEditString.setDisabled(false);
		btnDeleteString.setDisabled(false);
		ProfilerObject selectedProfile = new ProfilerObject();
		int count = 0;
		List<Component> seletedListItems = profileDataListbox.getSelectedItem()
				.getChildren();
		Iterator<Component> itr = seletedListItems.iterator();
		while (count < 4) {
			if (count == 0)
				selectedProfile.setCategory(((LabelElement) itr.next())
						.getLabel());
			else if (count == 1)
				selectedProfile.setSearchStringName(((LabelElement) itr.next())
						.getLabel());
			else if (count == 2)
				selectedProfile.setDefinition(((LabelElement) itr.next())
						.getLabel());
			else if (count == 3) {
				Checkbox isCheck = (Checkbox) itr.next().getChildren().get(0);
				selectedProfile.setIsStringChecked(isCheck.isChecked());
			} else
				itr.next();
			count++;
		}
		setSelectedRowList(selectedProfile);
	}

	@Command("editStringCommand")
	public void editStringCommand() {
		editCategory.setValue(getSelectedRowList().getCategory());
		btnOpenProfile.setDisabled(true);
		editSearchStringName.setValue(getSelectedRowList()
				.getSearchStringName());
		editDefinition.setValue(getSelectedRowList().getDefinition());
		btnSaveNewProfile.setDisabled(true);
		btnClose.setDisabled(true);
		btnSaveProfile.setDisabled(true);
		btnOpenProfile.setDisabled(true);
		btnDeleteProfile.setDisabled(true);
		btnEditString.setDisabled(true);
		btnDeleteString.setDisabled(true);
		btnNewString.setDisabled(true);
		btnSaveUpdatedString.setDisabled(true);
		btnDiscard.setDisabled(false);
		editStringGrid.setVisible(true);
		profileListGrid.setVisible(false);
		btnSaveUpdatedString.setVisible(true);
		btnSaveNewString.setVisible(false);
		editDefinitionStringError.setVisible(false);
		editDefinitionStringListboxError.setVisible(false);
		newDefinition.setStyle("align:left;border:1px solid #e6e6e6;text-transform:lowercase;s");
		selecDeselectAllLink.setDisabled(true);
	}

	@Command("saveUpdatedStringCommand")
	public void saveUpdatedStringCommand() {
		try {
			ProfilerObject profileData = new ProfilerObject();
			List<Component> seletedListItems = editItemListbox.getChildren();
			profileData = getDataFromEditNewString(seletedListItems);
			if (containsEntry(profileData)) {
				editStringGrid.setVisible(false);
				profileListGrid.setVisible(true);
				WebUtil.showErrorBoxWithCustomMessage("Cannot save. Data already present.");
			} else {
				List<ProfilerObject> updateStrings = new ArrayList<>();
				updateStrings.add(getSelectedRowList());
				updateStrings.add(profileData);
				DatabasesFacade.updateString(updateStrings);
				getProfileDetails();
			}
			btnDiscard.setDisabled(true);
			btnClose.setDisabled(false);
			btnEditString.setDisabled(true);
			btnDeleteString.setDisabled(true);
			btnNewString.setDisabled(false);
			btnSaveUpdatedString.setDisabled(true);
			editStringGrid.setVisible(true);
			profileListGrid.setVisible(false);
			btnSaveUpdatedString.setVisible(true);
			btnSaveNewString.setVisible(false);
			editStringGrid.setVisible(false);
			profileListGrid.setVisible(true);
			BindUtils.postNotifyChange(null, null,
					ConfigureSearchViewModel.this, "*");
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	@Command("saveNewStringCommand")
	public void saveNewStringCommand() {
		try {
			ProfilerObject profileData = new ProfilerObject();
			List<Component> seletedListItems = newStringListbox.getChildren();
			profileData = getDataFromEditNewString(seletedListItems);
			if (containsEntry(profileData)) {
				newStringGrid.setVisible(false);
				profileListGrid.setVisible(true);
				WebUtil.showErrorBoxWithCustomMessage("Cannot save. Data already present.");
			} else {
				DatabasesFacade.insertNewString(profileData);
				getProfileDetails();
			}
			btnDiscard.setDisabled(true);
			btnSaveNewString.setVisible(false);
			btnSaveNewString.setDisabled(true);
			btnSaveUpdatedString.setVisible(true);
			btnSaveUpdatedString.setDisabled(true);
			btnNewString.setDisabled(false);
			btnClose.setDisabled(false);
			btnDeleteProfile.setDisabled(false);
			newStringGrid.setVisible(false);
			profileListGrid.setVisible(true);
			selecDeselectAllLink.setDisabled(false);
			BindUtils.postNotifyChange(null, null,
					ConfigureSearchViewModel.this, "*");
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	public boolean containsEntry(ProfilerObject newEntry) {
		boolean flag = false;
		if (null == getProfileList())
			return flag;
		Iterator<ProfilerObject> activeProfileListIterator = getIntactProfileDetails()
				.iterator();
		while (activeProfileListIterator.hasNext()) {
			ProfilerObject newProfile = new ProfilerObject();
			newProfile = activeProfileListIterator.next();
			if (newEntry.getCategory().equals(newProfile.getCategory())
					&& newEntry.getSearchStringName().equals(
							newProfile.getSearchStringName())
					&& newEntry.getDefinition().equals(
							newProfile.getDefinition())) {
				flag = true;
			}
		}
		return flag;
	}

	public void openProfileAfter(String profileName) {
		getProfileDetails();
		BindUtils.postNotifyChange(null, null, ConfigureSearchViewModel.this,
				"*");
	}

	public ProfilerObject getDataFromEditNewString(
			List<Component> seletedListItems) {
		int listboxHeader = 0;
		ProfilerObject profileData = new ProfilerObject();
		int count = 0;
		Iterator<Component> itr = seletedListItems.iterator();
		while (itr.hasNext()) {
			if (listboxHeader == 0) {
				itr.next();
				listboxHeader = 1;
			} else {
				List<Component> listItems = itr.next().getChildren();
				Iterator<Component> listItemsIterator = listItems.iterator();
				count = 0;
				while (count < 4) {
					if (count == 0)
						profileData.setCategory(((Combobox) listItemsIterator
								.next().getFirstChild()).getValue().toString()
								.trim());
					else if (count == 1)
						profileData
								.setSearchStringName(((Textbox) listItemsIterator
										.next().getFirstChild()).getValue()
										.toString().trim());
					else if (count == 2)
						profileData.setDefinition(((Textbox) listItemsIterator
								.next().getFirstChild()).getValue().toString()
								.toLowerCase().trim());
					count++;
				}
			}
		}
		return profileData;
	}

	@Command("editCategoryChange")
	public void editCategoryChange(@BindingParam("id") String text) {
		text = text.trim();
		if (text.equalsIgnoreCase(getSelectedRowList().getCategory())) {
			btnSaveUpdatedString.setDisabled(true);
		} else {
			btnSaveUpdatedString.setDisabled(false);
		}
	}

	@Command("editCategoryChanging")
	public void editCategoryChanging(@BindingParam("id") String text) {
		text = text.trim();
		if (text.equalsIgnoreCase(getSelectedRowList().getCategory())) {
			btnSaveUpdatedString.setDisabled(true);
		} else {
			btnSaveUpdatedString.setDisabled(false);
		}
		if (!isValidEntryEditString(text, 0))
			btnSaveUpdatedString.setDisabled(true);
	}

	@Command("editSearchStringNameChange")
	public void editSearchStringNameChange(@BindingParam("id") String text) {
		text = text.trim();
		if (text.equalsIgnoreCase(getSelectedRowList().getSearchStringName())) {
			btnSaveUpdatedString.setDisabled(true);
		} else {
			btnSaveUpdatedString.setDisabled(false);
		}
		if (!isValidEntryEditString(text, 1))
			btnSaveUpdatedString.setDisabled(true);
	}

	@Command("editDefinitionChange")
	public void editDefinitionChange(@BindingParam("id") String text) {
		text = text.trim();
		if (text.equalsIgnoreCase(getSelectedRowList().getDefinition())) {
			btnSaveUpdatedString.setDisabled(true);
		} else {
			if (validateDefinitionTextbox(text)) {
				btnSaveUpdatedString.setDisabled(false);
				editDefinitionStringError.setVisible(false);
				editDefinitionStringListboxError.setVisible(false);
				editDefinition
						.setStyle("align:left;border-style:inset;border:2px solid white;text-transform:lowercase;");
			} else {
				btnSaveUpdatedString.setDisabled(true);
				editDefinitionStringError.setVisible(true);
				editDefinitionStringListboxError.setVisible(true);
				editDefinition
						.setStyle("align:left;border-style:inset;border:2px solid red;text-transform:lowercase;");
			}
		}

		if (!isValidEntryEditString(text, 2))
			btnSaveUpdatedString.setDisabled(true);
	}

	public boolean isValidEntryNewString(String text, int index) {
		boolean flag = true;
		if (index == 0) {
			if ("".equals(newSearchStringName.getValue())
					|| "".equals(newDefinition.getValue()) || "".equals(text))
				flag = false;
		} else if (index == 1) {
			if ("".equals(newCategory.getValue())
					|| "".equals(newDefinition.getValue()) || "".equals(text))
				flag = false;
		} else if (index == 2) {
			if ("".equals(newCategory.getValue())
					|| "".equals(newSearchStringName.getValue())
					|| "".equals(text)) {
				newDefinitionStringError.setVisible(false);
				flag = false;
			}
		}
		if (newDefinitionStringListboxError.isVisible())
			flag = false;
		return flag;
	}

	public boolean isValidEntryEditString(String text, int index) {
		boolean flag = true;
		if (index == 0) {
			if ("".equals(editSearchStringName.getValue())
					|| "".equals(editDefinition.getValue()) || "".equals(text))
				flag = false;
		} else if (index == 1) {
			if ("".equals(editCategory.getValue())
					|| "".equals(editDefinition.getValue()) || "".equals(text))
				flag = false;
		} else if (index == 2) {
			if ("".equals(editCategory.getValue())
					|| "".equals(editSearchStringName.getValue())
					|| "".equals(text)) {
				editDefinitionStringError.setVisible(false);
				flag = false;
			}
		}
		if (editDefinitionStringListboxError.isVisible())
			flag = false;
		return flag;
	}

	@Command("editNewCategoryChange")
	public void editNewCategoryChange(@BindingParam("id") String text) {
		text = text.trim();
		if (!isValidEntryNewString(text, 0))
			btnSaveNewString.setDisabled(true);
		else {
			btnSaveNewString.setDisabled(false);
		}
	}

	@Command("editNewSearchStringNameChange")
	public void editNewSearchStringNameChange(@BindingParam("id") String text) {
		text = text.trim();
		if (!isValidEntryNewString(text, 1))
			btnSaveNewString.setDisabled(true);
		else {
			btnSaveNewString.setDisabled(false);
		}
	}

	@Command("editNewDefinitionChange")
	public void editNewDefinitionChange(@BindingParam("id") String text) {
		text = text.trim();
		
		if (validateDefinitionTextbox(text)) {
			btnSaveNewString.setDisabled(false);
			newDefinitionStringError.setVisible(false);
			newDefinitionStringListboxError.setVisible(false);
			newDefinition.setStyle("align:left;border:1px solid #e6e6e6;text-transform:lowercase;");
		} else {
			newDefinitionStringListboxError.setVisible(true);
			btnSaveNewString.setDisabled(true);
			newDefinitionStringError.setVisible(true);
			newDefinition
					.setStyle("align:left;border-style:inset;border:2px solid red;text-transform:lowercase;");
		}
		if (!isValidEntryNewString(text, 2))
			btnSaveNewString.setDisabled(true);
		else {
			btnSaveNewString.setDisabled(false);
		}
	}

	@I18NMessage("Select One...")
	String SELECT_ONE = "SQLCM.Labels.addNewAlertRule";

	@Command("deleteStringCommand")
	public void deleteStringCommand() {
		try {
			String references = "";
			ProfilerObject profilerObj =  getSelectedRowList();
			for(ProfilerObject profilerObject : profileList){
				if(!references.contains("'"+profilerObject.getProfileName()+"'") 
						&& profilerObject.getIsStringChecked() 
						&& !profilerObject.getProfileName().equals(activeProfile)
						&& profilerObject.getCategory().equals(profilerObj.getCategory())
						&& profilerObject.getSearchStringName().equals(profilerObj.getSearchStringName())){
					if(references.isEmpty())
						references += "'" + profilerObject.getProfileName() + "'";
					else
						references += "," + "'" +profilerObject.getProfileName()+ "'";					
				}
			}
			if (references.isEmpty()) {
				if (WebUtil.showConfirmationBoxWithIcon(
						ELFunctions.getLabel(SQLCMI18NStrings.COLUMN_SEARCH_PROFILE_DELETE_NO_PROFILE_CONFIRM)
							, ELFunctions.getLabel(SQLCMI18NStrings.WARNING),
							"/images/warning_white.svg", false, (Object) null)){
					DatabasesFacade.deleteStringSeleted(getSelectedRowList());
					btnDeleteString.setDisabled(true);
					btnEditString.setDisabled(true);
					getProfileDetails();
					performAfterDeleteOperation();
					btnSaveProfile.setDisabled(true);
					btnClose.setLabel("Close");
					BindUtils.postNotifyChange(null, null,
							ConfigureSearchViewModel.this, "*");
				}
			} else {
				if (WebUtil.showConfirmationBoxWithIcon(Utility.getMessage(
						SQLCMI18NStrings.COLUMN_SEARCH_STRING_DELETE,
						references), ELFunctions
						.getLabel(SQLCMI18NStrings.WARNING),
						"/images/warning_white.svg", false, (Object) null)) {
					DatabasesFacade.deleteStringSeleted(getSelectedRowList());
					btnDeleteString.setDisabled(true);
					btnEditString.setDisabled(true);
					getProfileDetails();
					performAfterDeleteOperation();
					btnSaveProfile.setDisabled(true);
					btnClose.setLabel("Close");
					BindUtils.postNotifyChange(null, null,
							ConfigureSearchViewModel.this, "*");
				}
			}
			btnClose.setDisabled(false);
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	public void performAfterDeleteOperation() {
		if (compareProfilerObjectList(getIntactProfileDetails(),
				currentProfileList())) {
			btnSaveNewProfile.setDisabled(true);
			profileDataListbox.setSelectedItem(null);
			;
			btnSaveProfile.setDisabled(true);
			btnClose.setLabel("Close");
		} else {
			if (null == getActiveProfile())
				btnSaveProfile.setDisabled(true);
			else
				btnSaveProfile.setDisabled(false);
			btnSaveNewProfile.setDisabled(true);
			btnClose.setLabel("Cancel");
		}
	}

	@Command("closeCancelCommand")
	public void closeCancelCommand(@BindingParam("comp") Window x) {
		if ("Cancel".equalsIgnoreCase(btnClose.getLabel())) {
			btnClose.setLabel("Close");
			btnSaveProfile.setDisabled(true);
			setSingleProfileDetailsList(getIntactProfileDetails());
			if (null == getActiveProfile())
				btnSaveNewProfile.setDisabled(true);
			else
				btnSaveNewProfile.setDisabled(false);
			BindUtils.postNotifyChange(null, null,
					ConfigureSearchViewModel.this, "*");
		} else if ("Close".equalsIgnoreCase(btnClose.getLabel())) {
			try{
			DatabasesFacade.updateIsUpdated("1");
			x.detach();
			ColumnSearchViewModel
					.showColumnSearchAfterConfigSearch(getActiveProfile(), getInstanceDetail());
			}
			catch(Exception e)
			{
				e.getStackTrace();
			}
		}
		
	//	WebUtil.showInfoBoxWithCustomMessage("correct");
		
	}

	@Command("deleteProfileCommand")
	public void deleteProfileCommand(@BindingParam("comp") Window x) {
		//ConfigureSearchChildViewModel configSearchChild = new ConfigureSearchChildViewModel();
		ConfigureSearchChildViewModel.showConfigureSearchChild(DELETE_PROFILE,
				getUniqueProfileList(), getActiveProfile(), null);
		getSelectedProfile();
		x.detach();
	}

	@Command("openProfileCommand")
	public void openProfileCommand(@BindingParam("comp") Window x) {
		//ConfigureSearchChildViewModel configSearchChild = new ConfigureSearchChildViewModel();
		ConfigureSearchChildViewModel.showConfigureSearchChild(OPEN_PROFILE,
				getUniqueProfileList(), getActiveProfile(), null);
		x.detach();
	}

	@Command("saveNewProfileCommand")
	public void saveNewProfileCommand(@BindingParam("comp") Window x) {
		//ConfigureSearchChildViewModel configSearchChild = new ConfigureSearchChildViewModel();
		ConfigureSearchChildViewModel.showConfigureSearchChild(SAVE_NEW_PROFILE, getUniqueProfileList(),
				null, currentProfileList());
		x.detach();
	}

	@Command("saveCurrentProfileCommand")
	public void saveCurrentProfileCommand(@BindingParam("comp") Window x) {
		try {
			if (WebUtil.showConfirmationBoxWithIcon(Utility.getMessage(
					SQLCMI18NStrings.COLUMN_SEARCH_SAVE_PROFILE,
					getActiveProfile()), ELFunctions
					.getLabel(SQLCMI18NStrings.WARNING),
					"/images/warning_white.svg", false, (Object) null)) {
				List<ProfilerObject> currentProfile = currentProfileList();
				Iterator<ProfilerObject> itr = currentProfile.iterator();
				while (itr.hasNext()) {
					itr.next().setProfileName(getActiveProfile());
				}
				DatabasesFacade.updateCurrentProfile(currentProfile);
				DatabasesFacade.updateIsUpdated("1");
				x.detach();
			}
		} catch (Exception e) {
			e.getStackTrace();
		}
	}

	@Command("checkUncheckString")
	public void checkUncheckString() {
		if (null == getActiveProfile()) {
			if (compareProfilerObjectList(getIntactProfileDetails(),
					currentProfileList())) {
				btnSaveNewProfile.setDisabled(true);
				btnClose.setLabel("Close");
			} else {
				btnSaveNewProfile.setDisabled(false);
				btnClose.setLabel("Cancel");
			}
		} else {
			if (compareProfilerObjectList(getActiveProfileList(),
					currentProfileList())) {
				btnSaveNewProfile.setDisabled(false);
				btnSaveProfile.setDisabled(true);
				btnClose.setLabel("Close");
			} else {
				if(!hasSelectionProfilerObjectList(currentProfileList())){
					btnSaveNewProfile.setDisabled(true);
					btnSaveProfile.setDisabled(true);
				}
				else{
					btnSaveNewProfile.setDisabled(false);
					btnSaveProfile.setDisabled(false);
				}				
				btnClose.setLabel("Cancel");
			}
		}
	}

	@Command("linkSelecDeselectAll")
	public void linkSelecDeselectAll() {
		List<ProfilerObject> currentList = currentProfileList();
		Iterator<ProfilerObject> itr = currentList.iterator();

		if ("Select All".equalsIgnoreCase(selecDeselectAllLink.getLabel())) {
			while (itr.hasNext()) {
				itr.next().setIsStringChecked(true);
			}
			setSingleProfileDetailsList(currentList);
			selecDeselectAllLink.setLabel("Deselect All");
			btnClose.setLabel("Cancel");
		} else if ("Deselect All".equalsIgnoreCase(selecDeselectAllLink
				.getLabel())) {
			while (itr.hasNext()) {
				itr.next().setIsStringChecked(false);
			}
			setSingleProfileDetailsList(currentList);
			selecDeselectAllLink.setLabel("Select All");
			btnClose.setLabel("Close");
		}
		if (compareProfilerObjectList(getSingleProfileDetailsList(),
				getIntactProfileDetails())) {
			btnClose.setLabel("Close");
			if (null == getActiveProfile()){
				btnSaveNewProfile.setDisabled(true);
			}
			else {
				btnSaveNewProfile.setDisabled(true);
				btnSaveProfile.setDisabled(true);
			}
		} else {			
			if (null == getActiveProfile()){
				if(!hasSelectionProfilerObjectList(currentList)){
					btnSaveNewProfile.setDisabled(true);
					btnSaveProfile.setDisabled(true);
				}
				else{
					btnSaveNewProfile.setDisabled(false);
				}
			}
			else{
				if(!hasSelectionProfilerObjectList(currentList)){
					btnSaveNewProfile.setDisabled(true);
					btnSaveProfile.setDisabled(true);
				}
				else{
					btnSaveNewProfile.setDisabled(false);
					btnSaveProfile.setDisabled(false);
				}
			}			
		}
		BindUtils.postNotifyChange(null, null, ConfigureSearchViewModel.this,"*");
	}

	public String selecDeselectAllStatus() {
		List<ProfilerObject> currentList = getIntactProfileDetails();
		Iterator<ProfilerObject> itr = currentList.iterator();
		List<String> checkUncheck = new ArrayList<String>();
		while (itr.hasNext()) {
			String isCheck = String.valueOf(itr.next().getIsStringChecked());
			if (!checkUncheck.contains(isCheck))
				checkUncheck.add(isCheck);
		}
		if (checkUncheck.size() > 1)
			return "Select All";
		else if ("false".equals(checkUncheck.get(0)))
			return "Select All";
		else
			return "Deselect All";
	}

	public List<ProfilerObject> currentProfileList() {
		int count = 0;
		int listboxHeader = 0;
		List<ProfilerObject> updatedProfileList = new ArrayList();
		List<Component> mainListbox = profileDataListbox.getChildren();
		Iterator<Component> mainListboxIterator = mainListbox.iterator();
		while (mainListboxIterator.hasNext()) {
			if (listboxHeader == 0) {
				mainListboxIterator.next();
				listboxHeader = 1;
			} else {
				List<Component> listItems = mainListboxIterator.next()
						.getChildren();
				Iterator<Component> listItemsIterator = listItems.iterator();
				count = 0;
				ProfilerObject profile = new ProfilerObject();
				profile.setProfileName(getActiveProfile());
				while (count < 4) {
					if (count == 0)
						profile.setCategory(((LabelElement) listItemsIterator
								.next()).getLabel());
					else if (count == 1)
						profile.setSearchStringName(((LabelElement) listItemsIterator
								.next()).getLabel());
					else if (count == 2)
						profile.setDefinition(((LabelElement) listItemsIterator
								.next()).getLabel());
					else if (count == 3) {
						Checkbox isCheck = (Checkbox) listItemsIterator.next()
								.getChildren().get(0);
						profile.setIsStringChecked(isCheck.isChecked());
					} else
						listItemsIterator.next();
					count++;
				}
				updatedProfileList.add(profile);
			}
		}
		return updatedProfileList;
	}

	public boolean compareProfilerObjectList(
			List<ProfilerObject> sourceProfilerObjectList,
			List<ProfilerObject> destinationProfilerObjectList) {
		Iterator<ProfilerObject> activeProfileListIterator = sourceProfilerObjectList
				.iterator();
		Iterator<ProfilerObject> updatedProfileListIterator = destinationProfilerObjectList
				.iterator();
		boolean isEqual = true;
		while (activeProfileListIterator.hasNext()) {
			ProfilerObject activeProfileObj = activeProfileListIterator.next();
			ProfilerObject updatedProfileObj = updatedProfileListIterator
					.next();
			if (!activeProfileObj.equals(updatedProfileObj)) {
				isEqual = false;
			}
		}
		return isEqual;
	}
	
	// Start Changes doen to make button disabled on the basis of selection of check box
	
	public boolean hasSelectionProfilerObjectList(
			List<ProfilerObject> destinationProfilerObjectList) {
		Iterator<ProfilerObject> updatedProfileListIterator = destinationProfilerObjectList
				.iterator();
		boolean hasSelection = false;
		while (updatedProfileListIterator.hasNext()) {
			ProfilerObject updatedProfileObj = updatedProfileListIterator.next();
			if (updatedProfileObj.getIsStringChecked()) {
				hasSelection = true;
			}
		}
		return hasSelection;
	}
	
	// Ends Changes doen to make button disabled on the basis of selection of check box

	public boolean validateDefinitionTextbox(String text) {
		boolean flag = true;
		if ("".equalsIgnoreCase(text) || (text.trim().length() == 0)) {
			flag = false;
		} else {
			if (text.contains(",")) {
				if (!(text.startsWith(",") || text.endsWith(","))) {
					List<String> definitionList = Arrays
							.asList(text.split(","));
					Iterator<String> itr = definitionList.iterator();
					while (itr.hasNext()) {
						String definition = itr.next();
						if (definition.length() > 1) {
							if ((definition.startsWith("%") || definition
									.endsWith("%"))
									&& (!(definition.substring(1,
											definition.length() - 1))
											.contains("%"))) {
							} else {
								flag = false;
							}
						} else
							flag = false;
					}
				} else {
					flag = false;
				}
			} else {
				if (text.length() > 1) {
					if ((text.startsWith("%") || text.endsWith("%"))
							&& (!(text.substring(1, text.length() - 1))
									.contains("%"))) {
					} else {
						flag = false;
					}
				} else
					flag = false;
			}
		}
		return flag;
	}
}
