package com.idera.sqlcm.facade;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.RegulationSection;
import com.idera.sqlcm.entities.RegulationType;
import com.idera.sqlcm.rest.SQLCMRestClient;

import java.util.List;

public class RegulationFacade {

	public static List<RegulationSection> getRegulationSectionDictionary() throws RestException {
		try {
			return SQLCMRestClient.getInstance().getRegulationSectionDictionary();
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

    public static List<RegulationType> getRegulationTypeList() throws RestException {
        try {
            return SQLCMRestClient.getInstance().getRegulationTypeList();
        } catch (Exception e) {
            throw new RestException(e);
        }
    }

    public static List<String> getAllRegulationGuidelines() throws RestException{
		List<String> regulationGuidelinesList = null;
		try {
			regulationGuidelinesList = SQLCMRestClient.getInstance().GetAllRegulationGuidelines();
		} catch (RestException e) {
			throw new RestException(e);
		}
		return regulationGuidelinesList;
	}
    
}
