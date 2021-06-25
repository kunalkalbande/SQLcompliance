package com.idera.sqlcm.ui.basepage;

import com.idera.sqlcm.entities.CMUserSettings;
import com.idera.sqlcm.facade.UserFacade;
import com.idera.sqlcm.server.web.session.SessionUtil;
import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Page;
import org.zkoss.zk.ui.event.ClientInfoEvent;
import org.zkoss.zk.ui.metainfo.ComponentInfo;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zul.Window;

import com.idera.sqlcm.facade.LicenseDetailsFacade;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.LicenseExpired;

public class BasePageComposer extends SelectorComposer<Window> {

	private static final long serialVersionUID = 1L;
	public static final String ZUL_URL = "basepage/basepage.zul";
	protected static final Logger log = Logger.getLogger(BasePageComposer.class);

	@Override
	public ComponentInfo doBeforeCompose(Page page, Component parent,
			ComponentInfo compInfo) {
		// Check for valid license.
		if (!LicenseDetailsFacade.isValidLicense()) {
			String url = WebUtil.buildPathRelativeToCurrentProduct(
					LicenseExpired.ZUL_URL);
			Executions.getCurrent().sendRedirect(url);
		}
		try {
			CMUserSettings currentUser = UserFacade.getByDashboardUserId(SessionUtil.getCurrentUser().getId());
			if (currentUser != null) {
				int sessionTimeout = currentUser.getSessionTimeout().intValue() / 1000;
				SessionUtil.setupTimeout(Executions.getCurrent().getSession(), sessionTimeout);
			}
		} catch (Exception ex) {
			log.error("Error while registering widgets for IDERA Dashboard", ex);
			WebUtil.showErrorBox(ex.getCause(), ex.getLocalizedMessage());
		}
		return compInfo;
	};

	@Override
	public void doAfterCompose(Window component) throws Exception {

	}

}
