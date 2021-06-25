package com.idera.sqlcm.server.web.session;

import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Session;
import org.zkoss.zk.ui.Sessions;

import com.idera.core.user.Permission;
import com.idera.core.user.User;
import com.idera.cwf.model.Product;
import com.idera.sqlcm.utils.CmXmlConfigReader;
import com.idera.sqlcm.utils.SQLCMConstants;

public class SessionUtil extends com.idera.server.web.session.SessionUtil {

	public static void setSessionVariable(String key, Object value) {
		Session session = Sessions.getCurrent();
		if (session != null) {
			session.setAttribute(key, value);
		}
	}

	public static Object getSessionVariable(String key) {
		Session session = Sessions.getCurrent();
		Object value = null;
		if (session != null) {
			value = session.getAttribute(key);
		}
		return value;
	}

	public static void setupTimeout(Session session, int timeout) {
		if (timeout == -2 || timeout == 0) {
			session.setMaxInactiveInterval(SQLCMConstants.DEFAULT_TIMEOUT_IN_SECONDS);
		} else if (timeout == -1) {
			session.setMaxInactiveInterval(-1);
		} else {
			session.setMaxInactiveInterval(timeout);
		}
	}

	public static boolean isAdmin() {
		User user = getCurrentUser();
		for (Permission permission : user.getPermissions()) {
			if (permission.getRole().equalsIgnoreCase("DashboardAdministrator") || permission.getRole().equalsIgnoreCase("ProductAdministrator")) {
				return true;
			}
		}
		return false;
	}

	public static boolean canAccess() {
		User user = getCurrentUser();
		if (user.getPermissions().isEmpty()) return false;
		for (Permission permission : user.getPermissions()) {
			if (permission.getRole().equalsIgnoreCase("DashboardGuest") || permission.getRole().equalsIgnoreCase("ProductGuest")) {
				return false;
			}
		}
		return true;
	}

	public static boolean canNotAccess() {
		return !canAccess();
	}

	public static boolean hasPageAccess() {
		User user = getCurrentUser();
		Product product = (Product) Executions.getCurrent().getDesktop().getAttribute("currentProduct");
		String productName = product.getName();
		String instanceName = product.getInstanceName();
		if (user.isCoreAdmin() || user.isCoreGuest() || user.isProductAdmin(productName, instanceName)
				|| user.isProductGuest(productName, instanceName) || user.isProductUser(productName, instanceName)) {
			return true;
		}
		return false;
	}

	public static boolean getConfigValue(String page, String element) {
		return CmXmlConfigReader.getBooleanValue(page, element);
	}
	public static boolean getConfigValueForAdmin(String page, String element) {
		return (CmXmlConfigReader.getBooleanValue(page, element) && isAdmin());
	}
	public static boolean getConfigValueForAccessPermittedUsers(String page, String element) {
		return (CmXmlConfigReader.getBooleanValue(page, element) && canAccess());
	}
}
