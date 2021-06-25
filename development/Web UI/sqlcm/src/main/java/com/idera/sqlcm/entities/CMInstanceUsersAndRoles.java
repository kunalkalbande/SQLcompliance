package com.idera.sqlcm.entities;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonProperty;

public class CMInstanceUsersAndRoles {

    @JsonProperty("userList")
    private List<CMInstanceUser> userList;

    @JsonProperty("roleList")
    private List<CMInstanceRole> roleList;


    public List<CMInstanceUser> getUserList() {
        return userList;
    }

    public void setUserList(List<CMInstanceUser> userList) {
        this.userList = userList;
    }

    public List<CMInstanceRole> getRoleList() {
        return roleList;
    }

    public void setRoleList(List<CMInstanceRole> roleList) {
        this.roleList = roleList;
    }

    @JsonIgnore
    public List<CMInstancePermissionBase> getUsersAndRoles() {
        int usersCount = (userList != null) ? userList.size() : 0;
        int rolesCount = (roleList != null) ? roleList.size() : 0;

        List<CMInstancePermissionBase> permissionList = new ArrayList<>(usersCount + rolesCount);

        if (userList != null) {
            permissionList.addAll(userList);
        }

        if (roleList != null) {
            permissionList.addAll(roleList);
        }

        return permissionList;
    }

    public static CMInstanceUsersAndRoles composeInstance(Collection<CMInstancePermissionBase> usersAndRolesList) {
        CMInstanceUsersAndRoles cmInstanceUsersAndRoles = new CMInstanceUsersAndRoles();

        if (usersAndRolesList == null) {
            return cmInstanceUsersAndRoles;
        }

        List<CMInstanceRole> roles = new ArrayList<>();
        List<CMInstanceUser> users = new ArrayList<>();
        for (CMInstancePermissionBase p : usersAndRolesList) {
            if (p instanceof CMInstanceRole) {
                roles.add((CMInstanceRole)p);
            } else if (p instanceof CMInstanceUser) {
                users.add((CMInstanceUser)p);
            } else {
                throw new RuntimeException(" Incompatible role object " + p.getClass().getName());
            }
        }
        cmInstanceUsersAndRoles.setRoleList(roles);
        cmInstanceUsersAndRoles.setUserList(users);

        return cmInstanceUsersAndRoles;
    }

}
