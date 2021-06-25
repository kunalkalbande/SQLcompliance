package com.idera.sqlcm.ui.auditReports;

import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMSideBarViewSettings;
import com.idera.sqlcm.facade.ReportsFacade;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditApplicationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditDMLResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditLoginCreationResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditLoginDeletionResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditObjectActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditPermissionDeniedActivityResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditRowCountResponse;
import com.idera.sqlcm.ui.auditReports.CMAuditResponse.CMAuditUserActivityResponse;
import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.common.grid.CommonGridViewReport;

import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Paging;

import java.util.List;
import java.util.Map;
import java.util.TreeMap;

@Init(superclass = true)
public class AuditReportMainGridViewModel extends CommonGridViewModel {

	@Wire
	Paging listBoxPageId;
	    
	@Wire
	Intbox listBoxRowsBox;
	
	int PAGE_SIZE=50;
	
	public CMAuditApplication cmAuditApplication;	
	CMAuditDML cmAuditDML;
	CMAuditLoginCreation cmAuditLoginCreation;
	CMAuditLoginDeletion cmAuditLoginDeletion;
	CMAuditObjectActivity cmAuditObjectActivity;
	CMAuditPermissionDenied cmAuditPermissionDenied;
	CMAuditUserActivity cmAuditUserActivity;
	protected Map<String, Object> filterRequest = new TreeMap<>();
	protected List<CMEntity> entitiesList;
	protected ListModelList<CMEntity> entitiesModel;
	protected ListModelList<CMEntity> entitiesModelSelect;
	protected ListModelList AuditReportColumnsList;
	protected CMAuditApplicationResponse cMAuditApplicationResponse;
	List<CMAuditApplicationResponse> conditionEvents;
	List<CMAuditDMLResponse> conditionEventsDML;
	List<CMAuditLoginCreationResponse> conditionEventsLoginCreation;
	List<CMAuditLoginDeletionResponse> conditionEventsLoginDeletion;
	List<CMAuditObjectActivityResponse> conditionEventsObjectActivity;
	List<CMAuditPermissionDeniedActivityResponse> conditionEventsPermissionDeniedActivity;
	List<CMAuditUserActivityResponse> conditionEventsUserActivity;
	List<CMAuditRowCountResponse> conditionEventsRowCount;
	public ReportsFacade reportsFacade;

	public AuditReportMainGridViewModel() throws Exception {
		//getEntity();
	}
	
	public ListModelList<?> getAuditReportColumnsList() {
		return AuditReportColumnsList;
	}

	@Override
	protected CommonGridViewReport makeCommonGridViewReport() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected Map<String, Boolean> collectColumnsVisibilityMap() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected void retrieveColumnsVisibility(
			CMSideBarViewSettings alertsSettings) {
		// TODO Auto-generated method stub
		
	}

	@Command("openInstance")
	public void openInstance(@BindingParam("id") String id) {
		switch (id) {
		case "APPLICATION":
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("auditReportView"));
			break;
		case "CONFIGURATION":
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("configurationReportView"));
			break;
		case "DML":
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("dmlReportView"));
			break;
		case "LOGIN_CREATION":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("loginCreationReportView"));
			break;
		case "LOGIN_DELETION":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("loginDeletionReportView"));
			break;
		case "OBJECT_ACTIVITY":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("objectActivityReportView"));
			break;
		case "PERMISSION":
			Executions.sendRedirect(WebUtil
					.buildPathRelativeToCurrentProduct("permissionReportView"));
			break;
		case "USER_ACTIVITY":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("userActivityReportView"));
			break;
		case "ROW_COUNT":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("rowCountReportView"));
			break;
		case "REGULATORY_COMPLIANCE":
			Executions
					.sendRedirect(WebUtil
							.buildPathRelativeToCurrentProduct("regulatoryComplianceReportView"));
		default:
			break;
		}
	}
}
