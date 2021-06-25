package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.common.rest.UnauthorizedException;
import com.idera.core.user.CreateUser;
import com.idera.core.user.Permission;
import com.idera.core.user.User;
import com.idera.core.user.UserAlreadyExistException;
import com.idera.core.user.UserDoesNotExistException;
import com.idera.core.user.UserHasNoActiveDirectoryAccess;
import com.idera.core.user.UserPermission;
import com.idera.cwf.model.Product;
import com.idera.cwf.model.UpdateUser;
import com.idera.cwf.model.UpdateUserPermissions;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMUserSettings;
import com.idera.sqlcm.entities.CMUser;
import com.idera.sqlcm.enumerations.Role;
import com.idera.sqlcm.facade.UserFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.utils.SQLCMConstants;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Timebox;
import org.zkoss.zul.Window;

import java.sql.Time;
import java.util.Arrays;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class AddEditUserViewModel {
    public static final String ZUL_URL = "~./sqlcm/dialogs/addEditUser.zul";
    public static final String ENABLE_ADD_USER_FORM = "enableAddUserForm";
    public static final String USER_ID = "userId";

    @Wire
	private Label titleLabel;
    
    @Wire
    Textbox txtUserName;

    @Wire
    Textbox txtEmail;

    @Wire
    Checkbox isSubscribeAlerts;

    @Wire
    Checkbox enableSessionTimeout;

    @Wire
    Timebox timeOutBox;

    private static final String USER_NAME_PATTERN1 = "[*@:=/\\[?\\]\\|\\\"<>+;]";
    private static final String DOMAIN_NAME_PATTERN1 = "[\\\\,~:!@#$%\\^_'\\.\\(\\)\\{\\}_\\s/_|?]";
    private static final String EMAIL_PATTERN = "^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@"
            + "[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$";

    private String timeFormat = "HH:MM";
    private ListModelList<Role> roles;
    private String help;
    private Time sessionTimeOut;
    private CMUser user;
    private boolean enableAddUserForm;

    public String getHelp() {
        return help;
    }

    public ListModelList<Role> getRoles() {
        return roles;
    }

    public Time getSessionTimeOut() {
        if (sessionTimeOut == null) {
            return new Time(SQLCMConstants.DEFAULT_TIMEOUT_IN_MILLIS);
        }
        return sessionTimeOut;
    }

    public void setSessionTimeOut(Time sessionTimeOut) {
        this.sessionTimeOut = sessionTimeOut;
    }

    public String getTimeFormat() {
        return timeFormat;
    }

    public CMUser getUser() {
        return user;
    }

    public void setUser(CMUser user) {
        this.user = user;
    }

    public boolean isEnableAddUserForm() {
        return enableAddUserForm;
    }

    public void setEnableAddUserForm(boolean enableAddUserForm) {
        this.enableAddUserForm = enableAddUserForm;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        help = "http://wiki.idera.com/x/eAC5Ag";

        initDefaultInfo();

        HashMap<String, Object> args = (HashMap<String, Object>)Executions.getCurrent().getArg();
        setEnableAddUserForm((boolean) args.get(ENABLE_ADD_USER_FORM));
        if (!isEnableAddUserForm()) {
        	titleLabel.setValue("Edit User");
            int userId = (int)args.get(USER_ID);
            setUser(CMUser.wrapUser(getCmUserSettingsById(new Long(userId)), getCoreUser(userId)));
            roles.setSelection(Arrays.asList(user.getRole()));
            if ((user.getSessionTimeout() == null) || (user.getSessionTimeout() == 0)) {
                sessionTimeOut = new Time(SQLCMConstants.DEFAULT_TIMEOUT_IN_MILLIS);
                enableSessionTimeout.setChecked(false);
                timeOutBox.setDisabled(true);
            } else {
                sessionTimeOut = new Time(user.getSessionTimeout());
                enableSessionTimeout.setChecked(true);
                timeOutBox.setDisabled(false);
            }
        } else {
        	titleLabel.setValue("Add User");
            setUser(new CMUser());
        }
    }

    @Command
    public void enableTimebox() {
        if (!enableSessionTimeout.isChecked()) {
            sessionTimeOut = new Time(SQLCMConstants.DEFAULT_TIMEOUT_IN_MILLIS);
        }
        timeOutBox.setDisabled(!enableSessionTimeout.isChecked());
        BindUtils.postNotifyChange(null, null, this, "sessionTimeOut");
    }

    @Command
    public void saveData(@BindingParam("comp") Window x) {
        validateDialog();
        if (enableAddUserForm) {
            try {
                boolean userExist = false;
                List<UserPermission> users = CoreRestClient.getInstance().getUsers();
                int newUserId = 0;
                String existAccount;
                String enteredAccount = user.getName();
                for (UserPermission userPermission : users) {
                    existAccount = userPermission.getAccount().toLowerCase();
                    if (!userExist && existAccount.equals(enteredAccount)) {
                        userExist = true;
                        newUserId = userPermission.getUserID();
                        break;
                    }
                }

                if (!userExist) {
                    //create new User in IderaDashboard and get his ID
                    CreateUser newUser = new CreateUser();
                    newUser.setAccount(user.getName());
                    newUser.setEmail(user.getEmail());
                    try {
						newUserId = CoreRestClient.getInstance().createUser(newUser).getUserID();
					} catch (UserHasNoActiveDirectoryAccess e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
                }
                else 
                {
                	WebUtil.showWarningBox (SQLCMI18NStrings.DUPLICATE_USER);
                }

                //create permission for created User
                Product currentProduct = (Product) Executions.getCurrent().getDesktop().getAttribute("currentProduct");
                Permission permission = new Permission();
                permission.setUserid(newUserId);
                permission.setRoleid(roles.getSelection().iterator().next().getIndex());
                permission.setProductid(currentProduct.getProductId());
                try {
					CoreRestClient.getInstance().assignPermission(permission);
				} 
                catch (UnauthorizedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
                	
				}
                
                catch (RestException e) {
					// TODO Auto-generated catch block
					//e.printStackTrace();
                	//WebUtil.showWarningBox (SQLCMI18NStrings.DUPLICATE_USER);
                	e.printStackTrace();
				}
                CMUserSettings createUpdateUser = new CMUserSettings();
                createUpdateUser.setDashboardUserId(newUserId);
                createUpdateUser.setAccount(user.getName());
                createUpdateUser.setEmail(user.getEmail());
                if (enableSessionTimeout.isChecked()) {
                    createUpdateUser.setSessionTimeout(convertTimeoutToLongValue(getSessionTimeOut()));
                }
                createUpdateUser.setSubscribed(user.isSubscribedToCriticalAlerts());
                UserFacade.createUpdateUser(createUpdateUser);

            } catch (UserAlreadyExistException e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.DUPLICATE_USER);
            } catch (UserDoesNotExistException e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.NOT_A_VALID_DOMAIN_USER);
            } catch (RestException e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.USER_NOT_SAVED);
            }
        } else {
            Product currentProduct = (Product) Executions.getCurrent().getDesktop().getAttribute("currentProduct");
            UpdateUserPermissions newUserPermissions = new UpdateUserPermissions();
            newUserPermissions.setProductId(currentProduct.getProductId());
            newUserPermissions.setRoleId(roles.getSelection().iterator().next().getIndex());

            UpdateUser updateUser = new UpdateUser();
            updateUser.setAccount(user.getName());
            updateUser.setIsEnabled(true);
            updateUser.setPermissions(Arrays.asList(newUserPermissions));

            CMUserSettings createUpdateUser = new CMUserSettings();
            createUpdateUser.setDashboardUserId(user.getId());
            createUpdateUser.setAccount(user.getName());
            createUpdateUser.setEmail(user.getEmail());
            if (enableSessionTimeout.isChecked()) {
                createUpdateUser.setSessionTimeout(convertTimeoutToLongValue(getSessionTimeOut()));
            }
            createUpdateUser.setSubscribed(user.isSubscribedToCriticalAlerts());
            try {
                try {
					CoreRestClient.getInstance().updateUser((int) user.getId(), updateUser);
				} catch (UnauthorizedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
                UserFacade.createUpdateUser(createUpdateUser);
            } catch (RestException e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.USER_NOT_SAVED);
            } catch (UserDoesNotExistException e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.NOT_A_VALID_DOMAIN_USER);
            }
        }

        EventQueue<Event> eq = EventQueues.lookup(ManageUsersViewModel.UPDATE_USER_LIST_EVENT, EventQueues.APPLICATION, false);
        if (eq != null) {
            eq.publish(new Event("onClick", null, null ));
        }
        x.detach();
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    private void initDefaultInfo() {
        roles = new ListModelList<>();
        roles.addAll(Arrays.asList(Role.ADMINISTRATOR,  Role.USER, Role.READ_ONLY));
        roles.setSelection(Arrays.asList(Role.READ_ONLY));
    }

    private void validateDialog() {
        String userName = txtUserName.getValue();
        validateIsEmpty(userName, txtUserName);
        validateDomainUserName(userName, txtUserName);
        validateSubscribeProps();
        validateSessionTimeoutValue();
    }

    private static void validateIsEmpty(String value, Component component) {
        if (value == null || value.trim().isEmpty()) {
            throw new WrongValueException(
                    component,
                    ELFunctions.getMessage(SQLCMI18NStrings.USERNAME_CANNOT_BE_EMPTY));
        }
    }

    private static void validateDomainUserName(String value, Component component) {
        String[] domainAmdUserName = value.split("\\\\");

        if (value.indexOf("\\") == value.lastIndexOf("\\") && domainAmdUserName.length == 2) {
            Pattern pattern = Pattern.compile(DOMAIN_NAME_PATTERN1);
            Matcher matcher = pattern.matcher(domainAmdUserName[0]);
            boolean containsInvalidCharacter = false;
            if (matcher.find()) {
                containsInvalidCharacter = true;
            } else {
                pattern = Pattern.compile(USER_NAME_PATTERN1);
                matcher = pattern.matcher(domainAmdUserName[1]);
                if (matcher.find()) {
                    containsInvalidCharacter = true;
                }
            }

            if (containsInvalidCharacter) {
                throw new WrongValueException(
                        component,
                        ELFunctions.getMessage(SQLCMI18NStrings.USER_NAME_CONTAINS_NO_PERMIT_CHARACTERS));
            }
        } else {
            throw new WrongValueException(
                    component,
                    ELFunctions.getMessage(SQLCMI18NStrings.USER_NAME_BAD_FORMAT));
        }
    }

    private void validateSubscribeProps() {
        if (isSubscribeAlerts.isChecked()) {
            String email = txtEmail.getValue();
            if (email == null || email.trim().isEmpty()) {
                throw new WrongValueException(
                        txtEmail,
                        ELFunctions.getMessage(SQLCMI18NStrings.USER_EMAIL_IS_REQUIRED_TO_ALERTS));
            }
        }

        if (!txtEmail.getValue().trim().isEmpty()) {
            validateEmail(txtEmail.getValue(), txtEmail);
        }
    }

    private static void validateEmail(String value, Component component) {
        Pattern pattern = Pattern.compile(EMAIL_PATTERN);
        Matcher matcher = pattern.matcher(value);
        if (!matcher.matches()) {
            throw new WrongValueException(
                    component,
                    ELFunctions.getMessage(SQLCMI18NStrings.USER_EMAIL_BAD_FORMAT));
        }
    }

    private void validateSessionTimeoutValue() {
        if (enableSessionTimeout.isChecked()) {
            int timeoutInMinutes = new Date(getSessionTimeOut().getTime()).getMinutes() + new Date(getSessionTimeOut().getTime()).getHours() * 60;
            if (timeoutInMinutes < 1) {
                throw new WrongValueException(
                        timeOutBox,
                        ELFunctions.getLabelWithParams(SQLCMI18NStrings.SESSION_TIMEOUT_CANNOT_BE_LESS_THAN_DEFAULT,
                                1));
            }
        }
    }

    private User getCoreUser(int userId) {
        try {
            return CoreRestClient.getInstance().getUser(userId);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_CORE_USER);
        }
        return null;
    }

    private CMUserSettings getCmUserSettingsById(long dashboardUserId) {
        try {
            return UserFacade.getByDashboardUserId(dashboardUserId);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_CM_USER_SETTINGS);
        }
        return null;
    }

    private long convertTimeoutToLongValue(Time time){
        long hoursInMillis = time.getHours() * 60 * 60 * 1000;
        long minutesInMillis = time.getMinutes() * 60 * 1000;
        long timeout = hoursInMillis + minutesInMillis;
        return timeout == 0 ? SQLCMConstants.DEFAULT_TIMEOUT_IN_MILLIS : timeout;
    }
}
