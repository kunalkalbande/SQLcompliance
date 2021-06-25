package com.idera.sqlcm.facade;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMResponse;
import com.idera.sqlcm.entities.License;
import com.idera.sqlcm.rest.SQLCMRestClient;

import java.util.Date;

public class LicenseFacade {
	
    public static License getLicense() throws RestException {
        return SQLCMRestClient.getInstance().getLicense();
    }

    public static CMResponse addLicense(String licenseString) throws RestException {
        return SQLCMRestClient.getInstance().addLicense(licenseString);
    }

    public static boolean canAddOneMoreInstance() throws RestException {
        return SQLCMRestClient.getInstance().canAddOneMoreInstance();
    }

}
