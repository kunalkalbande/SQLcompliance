package com.idera.sqlcm.facade;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMAlertsSummary;
import com.idera.sqlcm.entities.CMEnvironment;
import com.idera.sqlcm.entities.CMEnvironmentDetails;
import com.idera.sqlcm.rest.SQLCMRestClient;

public class CMEnvironmentFacade {

	public static CMEnvironment getCMEnvironment() throws RestException {
		return SQLCMRestClient.getInstance().getCMEnvironment();
	}

	public static CMEnvironmentDetails getCMEnvironmentDetails() throws RestException {
		return SQLCMRestClient.getInstance().getCMEnvironmentDetails();
	}

	public static CMAlertsSummary getEnvironmentAlerts(int days) throws RestException {
		return SQLCMRestClient.getInstance().getEnvironmentAlerts(days);
	}
}
