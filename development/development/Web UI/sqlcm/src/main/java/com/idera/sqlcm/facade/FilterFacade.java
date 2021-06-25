package com.idera.sqlcm.facade;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.rest.JSONHelper;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMViewSettings;
import com.idera.sqlcm.entities.CategoryRequest;
import com.idera.sqlcm.entities.CategoryResponse;
import com.idera.sqlcm.entities.ViewNameResponse;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.ui.components.filter.model.Filter;

import java.io.IOException;
import java.util.List;

public class FilterFacade {
    public static String serializeFilterList(List<Filter> filters) throws IOException {
        return JSONHelper.serializeToJson(filters);
    }

    public static List<Filter> deserializeFilterList(String serializedFilterList) throws IOException {
        return JSONHelper.deserializeFromJson(serializedFilterList,
                new TypeReference<List<Filter>>() {
                });
    }

    public static void saveViewSettings(CMViewSettings settings) throws RestException {
        try {
            SQLCMRestClient.getInstance().saveViewSettings(settings);
        } catch (Exception e) {
            throw new RestException(e);
        }
    }

    public static CMViewSettings getViewSettings(String id) throws RestException {
        CMViewSettings resultData = new CMViewSettings();
        try {
            resultData =  SQLCMRestClient.getInstance().getViewSettings(id);
        } catch (Exception e) {
            throw new RestException(e);
        }
        return resultData;
    }
    
    public static ViewNameResponse getViewName(String ViewId) throws RestException {
    	ViewNameResponse resultData = new ViewNameResponse();
		try {
			resultData = new SQLCMRestClient().getViewName(ViewId);
		} catch (Exception e) {
			throw new RestException(e);
		}
		return resultData;
	}
    
    public static List<String> getAllAuditSettings() throws RestException{
		List<String> auditSettingsList = null;
		try {
			auditSettingsList = SQLCMRestClient.getInstance().GetAllAuditSettings();
		} catch (RestException e) {
			throw new RestException(e);
		}
		return auditSettingsList;
	}
    
}
