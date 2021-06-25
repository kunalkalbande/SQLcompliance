package com.idera.sqlcm.facade;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMAlertRulesEnableRequest;
import com.idera.sqlcm.entities.CMAlertRulesExportResponse;
import com.idera.sqlcm.entities.CMAuditEventFilterExportResponse;
import com.idera.sqlcm.entities.CMDataAlertRuleInfoRequest;
import com.idera.sqlcm.entities.CMDataAlertRulesInfo;
import com.idera.sqlcm.entities.CMDataByFilterId;
import com.idera.sqlcm.entities.CMDataByRuleId;
import com.idera.sqlcm.entities.CMFilteredAlertRulesResponse;
import com.idera.sqlcm.entities.CMServerAuditingRequest;
import com.idera.sqlcm.entities.CategoryRequest;
import com.idera.sqlcm.entities.CategoryResponse;
import com.idera.sqlcm.entities.InsertQueryData;
import com.idera.sqlcm.rest.SQLCMRestClient;

import java.util.Map;

public class AlertRulesFacade extends CommonFacade {

	public static CMFilteredAlertRulesResponse getFilteredAlertRules(Map<String, Object> filterRequest) throws RestException {
		CMFilteredAlertRulesResponse resultData = new CMFilteredAlertRulesResponse();
		try {
			resultData =  SQLCMRestClient.getInstance().getAlertRulesFilters(filterRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}
	
	public static void changeAlertRulesStatus(CMAlertRulesEnableRequest cmRulesEnableRequest) throws RestException {
		try {
			SQLCMRestClient.getInstance().changeAlertRulesStatus(cmRulesEnableRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	
	public static void deleteAlertRules(CMAlertRulesEnableRequest cmRulesEnableRequest) throws RestException {
		try {
			SQLCMRestClient.getInstance().delteAlertRules(cmRulesEnableRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	
	public static void insertStatusAlertRules(InsertQueryData insertQueryData) throws RestException {
		try {
			SQLCMRestClient.getInstance().insertStatusAlertRules(insertQueryData);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public CMAlertRulesExportResponse getAlertRulesExportResponse(CMDataByRuleId cmDataByRuleId) throws RestException {
		CMAlertRulesExportResponse resultData = new CMAlertRulesExportResponse();
		try {
			resultData =  new SQLCMRestClient().exportAlertRules(cmDataByRuleId);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}
	
	public String getAlertRulesExportStatus(String ruleIdString) throws RestException {
		String resultData;
		try {
			resultData =  new SQLCMRestClient().exportAlertRulesById(ruleIdString);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}
	
	public String importAlertRules(String xmlPath) throws RestException {
		String resultData;
		try {
			resultData =  new SQLCMRestClient().importAlertRules(xmlPath);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}

	
	public static void exportAlertRules(CMDataByRuleId cmDataByRuleId)
			throws RestException {
		try {
			SQLCMRestClient.getInstance().exportAlertRules(cmDataByRuleId);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	
	public CMDataAlertRulesInfo getCMDataAlertRulesInfo(CMDataAlertRuleInfoRequest cmDataAlertRuleInfoRequest) throws RestException {
		CMDataAlertRulesInfo resultData = new CMDataAlertRulesInfo();
		try {
			resultData = new SQLCMRestClient().getCMDataAlertRulesInfo(cmDataAlertRuleInfoRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}
	
	public CategoryResponse getCategoryInfo(CategoryRequest categoryRequest) throws RestException {
		CategoryResponse resultData = new CategoryResponse();
		try {
			resultData = new SQLCMRestClient().getCategoryInfo(categoryRequest);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}

}
