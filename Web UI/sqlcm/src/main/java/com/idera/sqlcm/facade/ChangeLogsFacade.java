package com.idera.sqlcm.facade;

import java.util.Map;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMActivityLogs;
import com.idera.sqlcm.entities.CMChangeLogs;
import com.idera.sqlcm.entities.CMFilteredChangeLogsResponse;
import com.idera.sqlcm.rest.SQLCMRestClient;

public class ChangeLogsFacade extends CommonFacade {

	public CMFilteredChangeLogsResponse getFilteredChangeLogs(Map<String, Object> filterRequest) throws RestException {
		CMFilteredChangeLogsResponse resultData = new CMFilteredChangeLogsResponse();
		try {
			resultData =  new SQLCMRestClient().getFilteredChangeLogs(filterRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}
	
	public static CMChangeLogs getChangeProperties(String eventId) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getChangeProperties(eventId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
}
