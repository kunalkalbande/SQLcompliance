package com.idera.sqlcm.facade;

import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.core.user.User;
import com.idera.sqlcm.entities.CMUserSettings;
import com.idera.sqlcm.rest.SQLCMRestClient;

import java.util.List;
import java.util.Set;

public class UserFacade {
    public static List<User> getAllUsers() throws RestException {
        return CoreRestClient.getInstance().getUsersWithPermission();
    }

    public static CMUserSettings getByDashboardUserId(long userId) throws RestException {
        return SQLCMRestClient.getInstance().getCMUserSettingsByID(userId);
    }

    public static void createUpdateUser(CMUserSettings user) throws RestException {
        SQLCMRestClient.getInstance().createUpdateUser(user);
    }

    public static List<CMUserSettings> getAllUsersSettings() throws RestException {
        return SQLCMRestClient.getInstance().getAllCMUserSettings();
    }

    public static void deleteUserSettings(Set<Long> userSettingsIds) throws RestException {
        SQLCMRestClient.getInstance().deleteCMUserSettingsByIDs(userSettingsIds);
    }
}
