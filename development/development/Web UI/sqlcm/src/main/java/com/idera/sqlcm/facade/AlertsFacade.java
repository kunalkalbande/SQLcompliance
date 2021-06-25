package com.idera.sqlcm.facade;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMFilteredAlertsResponse;
import com.idera.sqlcm.rest.SQLCMRestClient;

import java.util.Map;

public class AlertsFacade extends CommonFacade {

	public static CMFilteredAlertsResponse getFilteredAlerts(Map<String, Object> filterRequest) throws RestException {
		CMFilteredAlertsResponse resultData = new CMFilteredAlertsResponse();
		try {
			resultData =  SQLCMRestClient.getInstance().getFilteredAlerts(filterRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}
}
