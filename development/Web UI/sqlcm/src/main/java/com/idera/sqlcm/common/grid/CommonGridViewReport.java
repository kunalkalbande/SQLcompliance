package com.idera.sqlcm.common.grid;

import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.server.web.report.SQLCMBaseReport;
import org.zkoss.zul.ListModelList;

public abstract class CommonGridViewReport extends SQLCMBaseReport {

    public CommonGridViewReport(String reportTitle, String reportSubTitle, String reportName) {
        super(reportTitle, reportSubTitle, reportName);
    }

    public abstract void setDataMapForListInstance(ListModelList<CMEntity> modelList);

}
