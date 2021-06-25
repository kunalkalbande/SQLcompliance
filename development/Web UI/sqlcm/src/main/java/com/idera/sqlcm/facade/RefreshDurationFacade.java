package com.idera.sqlcm.facade;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMResponse;
import com.idera.sqlcm.entities.License;
import com.idera.sqlcm.rest.SQLCMRestClient;

public class RefreshDurationFacade {
	public static String setRefreshDuration(int refreshDuration) throws RestException {
        return SQLCMRestClient.getInstance().setRefreshDuration(refreshDuration);
    }

	public static String getRefreshDuration() throws RestException {
		return  SQLCMRestClient.getInstance().getRefreshDuration();
		
   }
}
