package com.idera.sqlcm.facade;
import java.util.Map;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMAlertRulesEnableRequest;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.snmpTrap.UpdateSNMPConfiguration;
import com.idera.sqlcm.ui.dialogs.UpdateSNMPThresholdConfiguration;
public class SNMPConfigFacade extends CommonFacade{

	public static void getUpdateSNMPConfiguration(UpdateSNMPConfiguration updateSNMPConfiguration) throws RestException {
		try {
			SQLCMRestClient.getInstance().getUpdateSNMPConfiguration(updateSNMPConfiguration);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	public static void getUpdateSNMPThresholdConfiguration(UpdateSNMPThresholdConfiguration updateSNMPThresholdConfiguration) throws RestException {
		try {
			SQLCMRestClient.getInstance().getUpdateSNMPThresholdConfiguration(updateSNMPThresholdConfiguration);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	
	public static void deleteThresholdConfiguration(String instanceName) throws RestException {
		try {
			SQLCMRestClient.getInstance().deleteThresholdConfiguration(instanceName);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}
	
	public static SNMPConfigData updateSnmpConfigData() throws RestException {
		try{
			return SQLCMRestClient.getInstance().updateSnmpConfigData();
		}
		catch(Exception e){
			throw new RestException(e);
		}
	}
	
	public boolean checkSnmpAddress(SNMPConfigData snmpConfigData) throws RestException {
		try{
			return SQLCMRestClient.getInstance().checkSnmpAddress(snmpConfigData);
		}
		catch(Exception e){
			throw new RestException(e);
		}
	}
	
}
