package com.idera.sqlcm.ui;

import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Label;
import org.zkoss.zul.Window;

import com.idera.ServerVersion;
import com.idera.common.Utility;
import com.idera.sqlcm.entities.LicenseDetails;
import com.idera.sqlcm.facade.LicenseDetailsFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

public class LicenseExpired extends SelectorComposer<Window> {

	private static final long serialVersionUID = 1L;
	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory
			.getLogger(LicenseExpired.class);

	public static final String ZUL_URL = "licenseExpired";

	protected AnnotateDataBinder binder;

	@Wire
	private Label licenseType;
	@Wire
	private Label expirationDate;

	@Override
	public void doAfterCompose(Window window) throws Exception {
		super.doAfterCompose(window);

		this.binder = new AnnotateDataBinder(this.getSelf());
		this.binder.bindBean("vendorWebsiteString", ServerVersion.SERVER_VENDOR_WEBSITE);
		this.binder.loadAll();

		LicenseDetails license = LicenseDetailsFacade.getLicenseDetails();
		if (license != null) {
			if (license.getScope() != null) {
				licenseType.setValue(Utility.getMessage(
						SQLCMI18NStrings.LICENSE_TYPE, license.getScope()));
			}
			if (license.getCreatedTime() != null) {
				expirationDate.setValue(Utility.getMessage(
						SQLCMI18NStrings.LICENSE_EXPIRATION_DATE,
						license.getCreatedTime()));
			}
		}
	}

}
