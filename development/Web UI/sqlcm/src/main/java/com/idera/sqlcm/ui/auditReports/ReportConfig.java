package com.idera.sqlcm.ui.auditReports;
 
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
 



import com.idera.sqlcm.ui.dialogs.eventFilterWizard.NewEventFilterRules;

import net.sf.jasperreports.engine.JRDataSource;
import net.sf.jasperreports.engine.JRException;
 
public class ReportConfig {
 
    private String source = "~./sqlcm/auditReportView/jasperreport.jasper";
    private Map<String, Object> parameters;
    private JRDataSource dataSource;
    AuditReportGridViewReport auditReportGridViewReport;
    public ReportConfig() {
    	parameters = new HashMap<String, Object>();
        parameters.put("ReportTitle", "Address Report");
        parameters.put("DataFile", "CustomDataSource from java");
    }
 
    public String getSource() {
        return source;
    }
 
    public Map<String, Object> getParameters() {
        return parameters;
    }
 
    public JRDataSource getDataSource() {
        return dataSource;
    }
 
    public void setDataSource(JRDataSource reportDataSource) {
        this.dataSource = reportDataSource;
    }
}