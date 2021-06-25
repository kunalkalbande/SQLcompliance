package com.idera.sqlcm.facade;

import java.util.List;
import java.util.Map;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.beEntities.AuditedInstanceBE;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMAuditEventFilterExportResponse;
import com.idera.sqlcm.entities.CMAuditedEventFilterEnable;
import com.idera.sqlcm.entities.CMDataByFilterId;
import com.idera.sqlcm.entities.CMDeleteEntity;
import com.idera.sqlcm.entities.CMFilterEventFiltersResponse;
import com.idera.sqlcm.entities.CMFilteredEventFiltersResponse;
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.rest.SQLCMRestClient;

public class EventFiltersFacade extends CommonFacade {

	
	public List<AuditedInstanceBE> getAllEntitiesEventFilter(Map<String, Object> filterRequest) {
		List<AuditedInstanceBE> resultData = null;
		try {
			resultData = SQLCMRestClient.getInstance().getAllAuditedInstances(filterRequest);			
		} catch (RestException e) {
			e.printStackTrace();
		}
		return resultData;
	}
	
	public CMFilterEventFiltersResponse getFilterEventFilters(Map<String, Object> filterRequest) throws RestException {
		CMFilterEventFiltersResponse resultData = new CMFilterEventFiltersResponse();
		try {
			resultData =  new SQLCMRestClient().getFilterEventFilters(filterRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}	
	
	
	
	public CMAuditEventFilterExportResponse getAuditEventFilterExportResponse(CMDataByFilterId cmDataByFilterId) throws RestException {
		CMAuditEventFilterExportResponse resultData = new CMAuditEventFilterExportResponse();
		try {
			resultData =  new SQLCMRestClient().exportEventFilter(cmDataByFilterId);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}	


	public static void changeEventFilterStatus(
			CMAuditedEventFilterEnable cmAuditedEventFilterEnable)
			throws RestException {
		try {
			SQLCMRestClient.getInstance().changeEventFilterStatus(
					cmAuditedEventFilterEnable);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	
	public static void deleteEventFilter(
			CMDeleteEntity cmDeleteEntity)
			throws RestException {
		try {
			SQLCMRestClient.getInstance().deleteEventFilter(cmDeleteEntity);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	
	public static void exportEventFilter(
			CMDataByFilterId cmDataByFilterId)
			throws RestException {
		try {
			SQLCMRestClient.getInstance().exportEventFilter(cmDataByFilterId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	
	public static void insertEventFilterData(InsertQueryData insertQueryData) throws RestException {
		  try {
			   SQLCMRestClient.getInstance().InsertEventFilterData(insertQueryData);
			  } catch (Exception e) {
			   throw new RestException(e);
		}
	}
	
	public String getEventFilterExportStatus(String ruleIdString) throws RestException {
		String resultData;
		try {
			resultData =  new SQLCMRestClient().exportEventFiltrById(ruleIdString);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}
	
		public String importAuditEventFilters(String xmlContent) throws RestException {
		String resultData;
		try {
			resultData =  new SQLCMRestClient().importAuditEventFilters(xmlContent);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}
}
