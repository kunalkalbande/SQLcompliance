package com.idera.sqlcm.facade;

import org.apache.log4j.Logger;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.ui.auditReports.CMAuditApplication;
import com.idera.sqlcm.ui.auditReports.CMAuditConfiguration;
import com.idera.sqlcm.ui.auditReports.CMAuditDML;
import com.idera.sqlcm.ui.auditReports.CMAuditLoginCreation;
import com.idera.sqlcm.ui.auditReports.CMAuditLoginDeletion;
import com.idera.sqlcm.ui.auditReports.CMAuditObjectActivity;
import com.idera.sqlcm.ui.auditReports.CMAuditPermissionDenied;
import com.idera.sqlcm.ui.auditReports.CMAuditRegulatoryCompliance;
import com.idera.sqlcm.ui.auditReports.CMAuditRowCount;
import com.idera.sqlcm.ui.auditReports.CMAuditUserActivity;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditIderaDefaultValuesResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListApplicationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListConfigurationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListDMLRespose;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListLoginCreationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListLoginDeletionResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListObjectActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListPermissionDeniedActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListRegulatoryComplianceResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListRowCountResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditListUserActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditSettingResponse;

public class ReportsFacade {

	Logger logger = Logger.getLogger(ReportsFacade.class);	
	public static CMAuditListApplicationResponse getAuditApplicationReport(CMAuditApplication cmAuditApplication)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditReport(cmAuditApplication);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAuditListConfigurationResponse getAuditConfigurationReport(CMAuditConfiguration cmAuditConfiguration)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditConfigurationReport(cmAuditConfiguration);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAuditIderaDefaultValuesResponse getIderaDefaultValues()
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getIderaDefaultValues();
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAuditIderaDefaultValuesResponse getIderaDatabaseDefaultValues()
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getIderaDatabaseDefaultValues();
		} catch (Exception e) {
			throw new RestException(e);
		}
	}


	public static CMAuditListDMLRespose getAuditDMLActivityReport(CMAuditDML cmAuditDML)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditDMLActivityReport(cmAuditDML);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAuditListLoginCreationResponse getAuditLoginCreationReport(CMAuditLoginCreation cmAuditLoginCreation)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditLoginCreationReport(cmAuditLoginCreation);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAuditListLoginDeletionResponse getAuditLoginDeletionReport(CMAuditLoginDeletion cmAuditLoginDeletion)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditLoginDeletionReport(cmAuditLoginDeletion);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAuditListPermissionDeniedActivityResponse getAuditPermissionDeniedReport(CMAuditPermissionDenied cmAuditPermissionDenied)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditPermissionDeniedReport(cmAuditPermissionDenied);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAuditListObjectActivityResponse getAuditObjectActivityReport(CMAuditObjectActivity cmAuditObjectActivity)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditObjectActivityReport(cmAuditObjectActivity);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAuditListUserActivityResponse getAuditUserActivityReport(CMAuditUserActivity cmAuditUserActivity)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditUserActivityReport(cmAuditUserActivity);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAuditListRowCountResponse getAuditRowCountReport(CMAuditRowCount cmRowCountActivity)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditRowCountReport(cmRowCountActivity);
		} catch (Exception e) {
			throw new RestException(e);
		}
	}

	public static CMAuditListRegulatoryComplianceResponse getRegulatoryComplianceReport(CMAuditRegulatoryCompliance cmAuditRegulatoryCompliance)
			throws RestException {
		try {
			return SQLCMRestClient.getInstance().getAuditRegulatoryComplianceReport(cmAuditRegulatoryCompliance);
		} catch(Exception e) {
			throw new RestException(e);
		}
	}


	public static CMAuditSettingResponse getConfigAuditSettingsList() throws RestException {
		try {
			// logger.info("Calling configAuditsettings");
			return SQLCMRestClient.getInstance().getConfigAuditSettingsList();
		} catch (Exception e) {
			// logger.info("Calling configAuditsettings : exception occured");
			throw new RestException(e);
		}
	}

}
