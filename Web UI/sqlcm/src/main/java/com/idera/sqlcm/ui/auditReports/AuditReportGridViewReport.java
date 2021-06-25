package com.idera.sqlcm.ui.auditReports;

import ar.com.fdvs.dj.domain.constants.HorizontalAlign;
import com.idera.server.resourse.GetResource;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.entities.CMActivityLogs;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEventDetails;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.report.SQLCMBaseReport;
import com.idera.sqlcm.ui.instanceEvents.EventsColumns;
import com.idera.sqlcm.ui.activityLogsView.filters.ActivityLogsViewOptionValues;
import org.zkoss.zul.ListModelList;

import java.io.InputStream;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

public class AuditReportGridViewReport extends CommonGridViewReport {
    protected List<LinkedHashMap<String, Object>> dataMap = new ArrayList<>();
    protected LinkedHashMap<String, ColumnProperties> columnPropertiesMap = new LinkedHashMap<>();

    public AuditReportGridViewReport(String reportTitle, String reportSubTitle, String reportName) {
        super(reportTitle, reportSubTitle, reportName);
    }

    @Override
    protected List<LinkedHashMap<String, Object>> getData() {
        return dataMap;
    }

    @Override
    protected LinkedHashMap<String, ColumnProperties> getColumns() {
        return columnPropertiesMap;
    }

    @Override
    protected String getMessageWhenNoData() {
        return SQLCMI18NStrings.NONE;
    }

    public void setDataMapForListInstance(ListModelList<CMEntity> modelList) {
        boolean addColumn = true;

        List<CMActivityLogs> alertsModel = new ListModelList<>();
        for (CMEntity entity : modelList) {
            if (entity instanceof CMActivityLogs) {
                alertsModel.add((CMActivityLogs) entity);
            }
        }

        for (CMActivityLogs alert :alertsModel) {
            LinkedHashMap<String, Object> tmpMap = new LinkedHashMap<>();
            InputStream tmpUrl = GetResource.getResource(imagesPath + imagesExt);
            if (tmpUrl != null) {
                tmpMap.put("stateImage", tmpUrl);
                if (addColumn) {
                    ColumnProperties col = new ColumnProperties(SQLCMI18NStrings.ALERTS);
                    col.setHorizontalAlign(HorizontalAlign.CENTER);
                    col.setFixedWidth(true);
                    getColumns().put("stateImage", col);
                }
            }

            dataMap.add(tmpMap);
            addColumn = false;
        }
    }
}
