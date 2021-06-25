package com.idera.sqlcm.facade;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMBeforeAfterDataEventsResponse;
import com.idera.sqlcm.entities.CMEventDetails;
import com.idera.sqlcm.entities.CMEventProperties;

import java.util.List;
import java.util.Map;

import com.idera.sqlcm.enumerations.Category;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.rest.SQLCMRestClient;

public class EventsFacade extends CommonFacade {

	public static CMBeforeAfterDataEventsResponse getAuditEvents(Map<String, Object> filterRequest) throws RestException {
		CMBeforeAfterDataEventsResponse resultData = null;
		try {
			resultData = SQLCMRestClient.getInstance().getFullInstanceAuditEvents(filterRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}

	public static CMBeforeAfterDataEventsResponse getArchivedEvents(Map<String, Object> filterRequest) throws RestException {
		CMBeforeAfterDataEventsResponse resultData = null;
		try {
			resultData = SQLCMRestClient.getInstance().getArchivedEvents(filterRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}

	public static CMEventProperties getEventProperties(String instanceId, String eventId, String eventDatabase) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getEventProperties(instanceId, eventId, eventDatabase);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

    public static List<CMEventDetails> getInstanceAuditEvents(long instId,
                                                       Interval interval, Category category) throws RestException {
        try {
            CMBeforeAfterDataEventsResponse response = SQLCMRestClient.getInstance().getInstanceAuditEvents(instId, interval.getDays(), category.getIndex());
            return (List<CMEventDetails>) response.getEvents();
        } catch (Exception e) {
            throw new RestException(e);
        }
    }

	public static CMBeforeAfterDataEventsResponse getEventsByIntervalForDatabase(long instId, long databaseId,
																				 Interval interval, int activePage,
																				 Integer pageSize, int sortDirection,
																				 String sortColumn) throws RestException {
		CMBeforeAfterDataEventsResponse resultData = null;
		try {
			resultData = SQLCMRestClient.getInstance().getEventsByIntervalForDatabase(instId, databaseId, interval.getDays(), activePage, pageSize, sortDirection, sortColumn);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}

	public static CMBeforeAfterDataEventsResponse getEventsByIntervalForInstance(long instId, Interval interval, int activePage,
																				 Integer pageSize, int sortDirection,
																				 String sortColumn) throws RestException {
		CMBeforeAfterDataEventsResponse resultData = null;
		try {
			resultData = SQLCMRestClient.getInstance().getEventsByIntervalForInstance(instId, interval.getDays(), activePage, pageSize, sortDirection, sortColumn);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}

}
