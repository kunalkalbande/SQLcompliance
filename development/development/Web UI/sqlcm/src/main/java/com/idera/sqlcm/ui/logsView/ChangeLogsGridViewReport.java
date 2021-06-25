package com.idera.sqlcm.ui.logsView;

import ar.com.fdvs.dj.domain.constants.HorizontalAlign;

import com.idera.server.resourse.GetResource;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.entities.CMChangeLogs;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEventDetails;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebConstants;
import com.idera.sqlcm.server.web.report.SQLCMBaseReport;
import com.idera.sqlcm.ui.instanceEvents.EventsColumns;
import com.idera.sqlcm.ui.instancesAlerts.AlertsColumns;
import com.idera.sqlcm.ui.changeLogsView.filters.ChangeLogsViewOptionValues;

import org.zkoss.zul.ListModelList;

import java.io.InputStream;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

public class ChangeLogsGridViewReport extends CommonGridViewReport {
    protected List<LinkedHashMap<String, Object>> dataMap = new ArrayList<>();
    protected LinkedHashMap<String, ColumnProperties> columnPropertiesMap = new LinkedHashMap<>();
    private DateFormat dateFormat;
    
    public ChangeLogsGridViewReport(String reportTitle, String reportSubTitle, String reportName) {
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

        List<CMChangeLogs> alertsModel = new ListModelList<>();
        for (CMEntity entity : modelList) {
            if (entity instanceof CMChangeLogs) {
                alertsModel.add((CMChangeLogs) entity);
            }
        }

        for (CMChangeLogs alert :alertsModel) {
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
           
            if (ChangeLogsColumns.DATE.isVisible()){
            dateFormat = new SimpleDateFormat(WebConstants.SHORT_DATE_FORMAT);
            addValidValueToMap(dateFormat.format(alert.getEventTime()), ChangeLogsColumns.DATE.getColumnId(),
            		ChangeLogsColumns.DATE.getLabelKey(), tmpMap, addColumn);
            }
            if (ChangeLogsColumns.TIME.isVisible()){
            dateFormat = new SimpleDateFormat(WebConstants.TIME_FORMAT);
            addValidValueToMap(dateFormat.format(alert.getEventTime()), ChangeLogsColumns.TIME.getColumnId(),
            		ChangeLogsColumns.TIME.getLabelKey(), tmpMap, addColumn);
            }
            if (ChangeLogsColumns.LOG_SQL_SERVER.isVisible()){
            addValidValueToMap(alert.getLogSqlServer(), ChangeLogsColumns.LOG_SQL_SERVER.getColumnId(),
            		ChangeLogsColumns.LOG_SQL_SERVER.getLabelKey(), tmpMap, addColumn);
            }
            if (ChangeLogsColumns.LOG_TYPE.isVisible()){
            addValidValueToMap(alert.getLogType(), ChangeLogsColumns.LOG_TYPE.getColumnId(),
            		ChangeLogsColumns.LOG_TYPE.getLabelKey(), tmpMap, addColumn);
            }
            if (ChangeLogsColumns.LOG_USER.isVisible()){
            addValidValueToMap(alert.getLogUser(), ChangeLogsColumns.LOG_USER.getColumnId(),
            		ChangeLogsColumns.LOG_USER.getLabelKey(), tmpMap, addColumn);
            }
            if (ChangeLogsColumns.LOG_INFO.isVisible()){
            addValidValueToMap(alert.getLogInfo(), ChangeLogsColumns.LOG_INFO.getColumnId(),
            		ChangeLogsColumns.LOG_INFO.getLabelKey(), tmpMap, addColumn);
            }
            dataMap.add(tmpMap);
            addColumn = false;
        }
    }
}
