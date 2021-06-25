package com.idera.sqlcm.facade;

import mazz.i18n.Logger;
import mazz.i18n.LoggerFactory;

import com.idera.common.rest.RestException;
import com.idera.i18n.I18NStrings;
import com.idera.sqlcm.entities.LicenseDetails;
import com.idera.sqlcm.entities.LicenseDetails.LicenseState;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.rest.SQLCMRestClient;

public class LicenseDetailsFacade {

	private static final Logger logger = LoggerFactory.getLogger(LicenseDetailsFacade.class);

	public static LicenseDetails getLicenseDetails() throws RestException {

		return SQLCMRestClient.getInstance().getLicenseDetails();
	}

	public static LicenseDetails getLicenseAt(int index) throws RestException {

		LicenseDetails licDetails = getLicenseDetails();
		if (licDetails != null && licDetails.getLicenses() != null) {
			return licDetails.getLicenses().get(index);
		}
		return null;
	}

	public static boolean isValidLicense() {
		try {
			LicenseDetails lic = getLicenseDetails();
			logger.info(I18NStrings.LICENSE_TRIAL_LICENSE_STATUS, lic);
			if (lic != null && lic.getState() == LicenseState.Valid) {
				return true;
			}
		} catch (Exception ex) {
			logger.error(SQLCMI18NStrings.ERR_GET_LICENSE_DETAILS, ex);
		}
		return false;
	}
}
