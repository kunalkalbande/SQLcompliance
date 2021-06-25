package com.idera.sqlcm.facade;
import java.util.Map;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMAlertRulesEnableRequest;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.smtpConfiguration.GetSNMPConfigResponse;
import com.idera.sqlcm.smtpConfiguration.GetSNMPConfiguration;
import com.idera.sqlcm.snmpTrap.UpdateSNMPConfiguration;
import com.idera.sqlcm.ui.dialogs.UpdateSNMPThresholdConfiguration;
public class GetSNMPConfigFacade extends CommonFacade{

	public static GetSNMPConfigResponse getSNMPConfiguration(GetSNMPConfiguration getSNMPConfiguration) throws RestException {
		try {
			return SQLCMRestClient.getInstance().getSNMPThresholdConfiguration(getSNMPConfiguration);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
}
