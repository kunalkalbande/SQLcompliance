package com.idera.sqlcm.ui.auditReports;
import java.util.Arrays;
 
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zul.ListModelList;
 
public class JasperReportViewModel {
 
    /*ReportType reportType = null;*/
    private ReportConfig reportConfig = null;
    @Command("showReport")
    @NotifyChange("reportConfig")
    public void showReport() {
    	AuditReportGridViewReport auditReportGridViewReport=new AuditReportGridViewReport("Audited Application", "", "");
    }
}