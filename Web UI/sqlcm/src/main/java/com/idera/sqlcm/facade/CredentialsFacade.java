package com.idera.sqlcm.facade;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.addserverwizard.AgentServiceAccount;
import com.idera.sqlcm.rest.SQLCMRestClient;

public class CredentialsFacade extends CommonFacade {

	public static boolean validateDomainCredentials(AgentServiceAccount agentServiceAccount) throws RestException {
		return SQLCMRestClient.getInstance().validateDomainCredentials(agentServiceAccount);
	}
}
