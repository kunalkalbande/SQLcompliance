package com.idera.sqlcm.facade;

import java.util.Map;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMActivityLogs;
import com.idera.sqlcm.entities.CMEventProperties;
import com.idera.sqlcm.entities.CMFilteredActivityLogsResponse;
import com.idera.sqlcm.rest.SQLCMRestClient;

public class ActivityLogsFacade extends CommonFacade {

	public CMFilteredActivityLogsResponse getFilteredActivitylogs(Map<String, Object> filterRequest) throws RestException {
		CMFilteredActivityLogsResponse resultData = new CMFilteredActivityLogsResponse();
		try {
			resultData =  new SQLCMRestClient().getFilteredActivityLogs(filterRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}
	
	public static CMActivityLogs getActivityProperties(String eventId) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getActivityProperties(eventId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
}
