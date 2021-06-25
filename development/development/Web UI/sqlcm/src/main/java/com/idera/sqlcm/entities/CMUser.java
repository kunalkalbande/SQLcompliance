package com.idera.sqlcm.entities;

import com.idera.core.user.Permission;
import com.idera.core.user.User;
import com.idera.cwf.model.Product;
import com.idera.sqlcm.enumerations.Role;
import org.zkoss.zk.ui.Executions;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class CMUser extends CMEntity {
    private Role role;
    private String email;
    private Long sessionTimeout;
    private boolean subscribedToCriticalAlerts;

    public Role getRole() {
        return role;
    }

    public void setRole(Role role) {
        this.role = role;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public Long getSessionTimeout() {
        return sessionTimeout;
    }

    public void setSessionTimeout(Long sessionTimeout) {
        this.sessionTimeout = sessionTimeout;
    }

    public boolean isSubscribedToCriticalAlerts() {
        return subscribedToCriticalAlerts;
    }

    public void setSubscribedToCriticalAlerts(boolean subscribedToCriticalAlerts) {
        this.subscribedToCriticalAlerts = subscribedToCriticalAlerts;
    }

    public static CMUser wrapUser(CMUserSettings userSettings, User user) {
        if (!user.isEnabled()) {
            return null;
        }
        Permission permission = getUserPermissionForCurrentProduct(user);
        if (permission == null) {
            return null;
        }
        CMUser cmUser = new CMUser();
        cmUser.setId(user.getId());
        cmUser.setName(user.getAccount());
        cmUser.setRole(Role.getByIndex(permission.getRoleid()));
        if (userSettings != null) {
            cmUser.setEmail(userSettings.getEmail());
            cmUser.setSessionTimeout(userSettings.getSessionTimeout());
            cmUser.setSubscribedToCriticalAlerts(userSettings.isSubscribed());
        }
        return cmUser;
    }

    public static List<CMUser> wrapUserList(Map<Long, CMUserSettings> userSettingsMap, List<User> userList) {
        List<CMUser> cmUsers = new ArrayList<>();
        for (User user: userList) {
            CMUser cmUser = wrapUser(userSettingsMap.get(new Long(user.getId())), user);
            if (cmUser != null) {
                cmUsers.add(cmUser);
            }
        }
        return cmUsers;
    }

    private static Permission getUserPermissionForCurrentProduct(User user) {
        Product currentProduct = (Product) Executions.getCurrent().getDesktop().getAttribute("currentProduct");
        for (Permission permission: user.getPermissions()) {
            if (currentProduct.getProductId() == permission.getProductid()) {
                return permission;
            }
        }
        return null;
    }

    public static User unwrapUser(CMUser cmUser) {
        User user = new User();
        user.setId((int) cmUser.getId());
        user.setAccount(cmUser.getName());
        return user;
    }
}
