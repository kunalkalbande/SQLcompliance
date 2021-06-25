package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.common.rest.UnauthorizedException;
//import com.idera.common.rest.UnauthorizedException;
import com.idera.core.user.User;
import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMUser;
import com.idera.sqlcm.entities.CMUserSettings;
import com.idera.sqlcm.facade.UserFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.server.web.session.SessionUtil;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
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
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class ManageUsersViewModel {
    public static final String ZUL_URL = "~./sqlcm/dialogs/manageUsers.zul";
    public static final String UPDATE_USER_LIST_EVENT = "update-user-list-event";

    private String help;
    private boolean enableMultiOperators;
    private boolean enableUniOperators;    

    private Integer pageSize = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

	public Integer getPageSize() {
		return pageSize;
	}

	public int getActivePage() {
		return activePage;
	}

	public void setActivePage(int activePage) {
		this.activePage = activePage;
	}

	public void setPageSize(Integer pageSize) {
		this.pageSize = pageSize;
	}
	private long fileSize;
	@Wire
    Spinner listBoxRowsBox;
	
	@Wire
    Paging listBoxPageId;
	
	int recordCount;
	
	private int prevPageSize;

    private int activePage = SQLCMConstants.DEFAULT_PAGE;
	  
    public long getFileSize() {
		return fileSize;
	}

	public void setFileSize(long fileSize) {
		this.fileSize = fileSize;
	}
	
	public long getTotalSize() {
        return recordCount;
    }
    private ListModelList<CMUser> userListModel;

    public String getHelp() {
        return help;
    }

    public boolean isEnableMultiOperators() {
        if (userListModel != null && getUserListModel().getSelection().size() > 0 && SessionUtil.canAccess()) {
            enableMultiOperators = true;
        } else {
            enableMultiOperators = false;
        }
        return enableMultiOperators;
    }

    public boolean isEnableUniOperators() {
        if (getUserListModel() != null && getUserListModel().getSelection().size() == 1 && SessionUtil.canAccess()) {
            enableUniOperators = true;
        } else {
            enableUniOperators = false;
        }
        return enableUniOperators;
    }

    public ListModelList<CMUser> getUserListModel() {
        return userListModel;
    }

    public static void showManageUsersDialog() {
        Window window = (Window) Executions.createComponents(ZUL_URL, null, null);
        window.doHighlighted();
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        help  = "http://wiki.idera.com/x/eAC5Ag";
        initUsersList();
        subscribeToEvent();
    }

    @Command
    @NotifyChange({"enableMultiOperators", "enableUniOperators"})
    public void enableButtons() {
    }

    @Command
    public void addUser() {
        Map<String, Object> args = new HashMap<>();
        args.put("enableAddUserForm", true);

        Window window = (Window) Executions.createComponents(AddEditUserViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }

    @Command
    public void editUser() {
        CMUser selectedUser = userListModel.getSelection().iterator().next();
        Map<String, Object> args = new HashMap<>();
        args.put(AddEditUserViewModel.ENABLE_ADD_USER_FORM, false);
        args.put(AddEditUserViewModel.USER_ID, (int)selectedUser.getId());

        Window window = (Window) Executions.createComponents(AddEditUserViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }

    @Command
    public void removeUser()  {
        boolean deleteCurrentUser = false;
        //boolean delete = WebUtil.showConfirmationBox(I18NStrings.USERS_DELETE_CONFIRMATION_MESSAGE, SQLCMI18NStrings.USERS_DELETE_CONFIRMATION_TITLE);
        boolean delete = WebUtil.showMessageBoxWithUserPreference(SQLCMI18NStrings.USERS_DELETE_CONFIRMATION_MESSAGE, SQLCMI18NStrings.USERS_DELETE_CONFIRMATION_TITLE);

        if (delete) {
            Set<CMUser> cmUserSetToDelete = userListModel.getSelection();
            if (cmUserSetToDelete.size() == userListModel.size()) {
                WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.ERROR_ALL_USERS_REMOVE);
            } else {
                User currentUser = SessionUtil.getCurrentUser();
                List<User> usersToDelete = new ArrayList<>();
                Set<Long> userIdsToDelete = new HashSet<>();

                for (CMUser cmUser: cmUserSetToDelete) {
                    if ((int)cmUser.getId() == currentUser.getId()) {
                        deleteCurrentUser = true;
                        break;
                    } else {
                        usersToDelete.add(CMUser.unwrapUser(cmUser));
                        userIdsToDelete.add(cmUser.getId());
                    }
                }

                if (!deleteCurrentUser) {
                    try {
                        try {
							com.idera.core.facade.UserFacade.deleteUsers(usersToDelete);
						} catch (UnauthorizedException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
                        UserFacade.deleteUserSettings(userIdsToDelete);
                        refreshUserList();
                    } catch (RestException e) {
                        WebUtil.showErrorBox(e, I18NStrings.ADMINISTRATION_FAILED_DELETE_USERS);
                    }
                } else {
                    WebUtil.showErrorBox(new RestException(), I18NStrings.ERROR_SELF_USER_REMOVE);
                }
            }
        }
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    private void initUsersList() {
        try {
            Map<Long, CMUserSettings> userSettingsMap = createUserSettingsMap(UserFacade.getAllUsersSettings());
            userListModel = new ListModelList<>();
            userListModel.addAll(CMUser.wrapUserList(userSettingsMap, UserFacade.getAllUsers()));
            recordCount = userListModel.size();
            userListModel.setMultiple(true);
        } catch (RestException e) {
            WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.FAILED_TO_LOAD_USERS);
        }
    }

    protected void subscribeToEvent() {
        EventQueue<Event> eq = EventQueues.lookup(UPDATE_USER_LIST_EVENT, EventQueues.APPLICATION, true);
        eq.subscribe(new EventListener<Event>() {
            public void onEvent(Event event) throws Exception {
                refreshUserList();
            }
        });
    }

    private void refreshUserList() {
        initUsersList();
        BindUtils.postNotifyChange(null, null, ManageUsersViewModel.this, "userListModel");
        BindUtils.postNotifyChange(null, null, ManageUsersViewModel.this, "enableUniOperators");
        BindUtils.postNotifyChange(null, null, ManageUsersViewModel.this, "enableMultiOperators");
    }

    private Map<Long, CMUserSettings> createUserSettingsMap(List<CMUserSettings> userSettingsList) {
        Map<Long, CMUserSettings> userSettingsMap = new HashMap<>();
        for (CMUserSettings userSettings: userSettingsList) {
            userSettingsMap.put(userSettings.getDashboardUserId(), userSettings);
        }
        return userSettingsMap;
    }
    
    @Command("setGridRowsCount")
    public void setGridRowsCount() {
        try {
            int tmpPageSize = listBoxRowsBox.getValue();
            if (tmpPageSize > 100) {
                Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
                        listBoxRowsBox, "end_center", 3000);
                tmpPageSize = 100;
                listBoxRowsBox.setValue(tmpPageSize);
            }
            if (tmpPageSize != pageSize) {
                activePage = SQLCMConstants.DEFAULT_PAGE;
                BindUtils.postNotifyChange(null, null, this, "activePage");
            }
            listBoxPageId.setPageSize(tmpPageSize);
            prevPageSize = tmpPageSize;
            pageSize = tmpPageSize;
            int value=0;
            if(userListModel !=null && !userListModel.isEmpty())
            {
            	
            	
            		value=userListModel.size();
            		setFileSize(value);            	
            }
            else
            {
            	setFileSize(0);  
            }
        } catch (WrongValueException exp) {
            listBoxPageId.setPageSize(prevPageSize);
        }
        BindUtils.postNotifyChange(null, null, this, "userListModel");
    }
    
}