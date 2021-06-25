package com.idera.sqlcm.server.web.report;

import com.idera.server.web.report.IderaBaseReport;


public abstract class SQLCMBaseReport extends IderaBaseReport {

    protected static final mazz.i18n.Logger LOG = mazz.i18n.LoggerFactory.getLogger(SQLCMBaseReport.class);
    protected static final org.apache.log4j.Logger debug = org.apache.log4j.Logger.getLogger(SQLCMBaseReport.class);

    public SQLCMBaseReport(String reportTitle, String reportSubTitle, String reportName) {
        super(reportTitle, reportSubTitle, reportName);
    }

    public SQLCMBaseReport(String reportName) {
        super(reportName);
    }

    @Override
    protected void initProperties() {
        imagesPath = SQLCMBaseReportPreferences.IMAGES_PATH;
        imagesExt = SQLCMBaseReportPreferences.IMAGES_EXT;
        ideraLogoPath = SQLCMBaseReportPreferences.IDERA_LOGO_PATH;
        ideraLogoWidth = SQLCMBaseReportPreferences.IDERA_LOGO_WIDTH;
        ideraLogoHeight = SQLCMBaseReportPreferences.IDERA_LOGO_HEIGHT;
        ideraProductLogoPath = SQLCMBaseReportPreferences.IDERA_PRODUCT_LOGO_PATH;
        ideraProductLogoWidth = SQLCMBaseReportPreferences.IDERA_PRODUCT_LOGO_WIDTH;
        ideraProductLogoHeight = SQLCMBaseReportPreferences.IDERA_PRODUCT_LOGO_HEIGHT;
        ideraProductName = SQLCMBaseReportPreferences.IDERA_PRODUCT_NAME;
    }

}
