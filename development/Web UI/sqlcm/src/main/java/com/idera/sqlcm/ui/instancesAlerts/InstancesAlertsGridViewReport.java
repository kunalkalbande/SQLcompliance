package com.idera.sqlcm.ui.instancesAlerts;

import ar.com.fdvs.dj.domain.constants.HorizontalAlign;
import com.idera.server.resourse.GetResource;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.entities.CMAlert;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEventDetails;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebConstants;
import com.idera.sqlcm.server.web.report.SQLCMBaseReport;
import com.idera.sqlcm.ui.instanceEvents.EventsColumns;
import com.idera.sqlcm.ui.instancesAlerts.filters.AlertsOptionValues;
import org.zkoss.zul.ListModelList;

import java.io.InputStream;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

public class InstancesAlertsGridViewReport extends CommonGridViewReport {
    protected List<LinkedHashMap<String, Object>> dataMap = new ArrayList<>();
    protected LinkedHashMap<String, ColumnProperties> columnPropertiesMap = new LinkedHashMap<>();
    private DateFormat dateFormat;

    public InstancesAlertsGridViewReport(String reportTitle, String reportSubTitle, String reportName) {
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

        List<CMAlert> alertsModel = new ListModelList<>();
        for (CMEntity entity : modelList) {
            if (entity instanceof CMAlert) {
                alertsModel.add((CMAlert) entity);
            }
        }

        for (CMAlert alert : alertsModel) {
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

            if (AlertsColumns.INSTANCE_NAME.isVisible()) {
                addValidValueToMap(alert.getInstanceName(), AlertsColumns.INSTANCE_NAME.getColumnId(),
                    AlertsColumns.INSTANCE_NAME.getLabelKey(), tmpMap, addColumn);
            }

            if (AlertsColumns.DATE.isVisible()) {
                dateFormat = new SimpleDateFormat(WebConstants.SHORT_DATE_FORMAT);
                addValidValueToMap(dateFormat.format(alert.getTime()), AlertsColumns.DATE.getColumnId(),
                    AlertsColumns.DATE.getLabelKey(), tmpMap, addColumn);
            }

            if (AlertsColumns.TIME.isVisible()) {
                dateFormat = new SimpleDateFormat(WebConstants.TIME_FORMAT);
                addValidValueToMap(dateFormat.format(alert.getTime()), AlertsColumns.TIME.getColumnId(),
                    AlertsColumns.TIME.getLabelKey(), tmpMap, addColumn);
            }

            if (AlertsColumns.LEVEL.isVisible()) {
                addValidValueToMap(AlertsOptionValues.findLabelByKey(alert.getAlertLevel()), AlertsColumns.LEVEL.getColumnId(),
                    AlertsColumns.LEVEL.getLabelKey(), tmpMap, addColumn);
            }

            if (AlertsColumns.SOURCE_RULE.isVisible()) {
                addValidValueToMap(alert.getSourceRule(), AlertsColumns.SOURCE_RULE.getColumnId(),
                    AlertsColumns.SOURCE_RULE.getLabelKey(), tmpMap, addColumn);
            }

            if (AlertsColumns.EVENT.isVisible()) {
                addValidValueToMap(alert.getEventTypeName(), AlertsColumns.EVENT.getColumnId(),
                    AlertsColumns.EVENT.getLabelKey(), tmpMap, addColumn);
            }

            if (AlertsColumns.DETAIL.isVisible()) {
                addValidValueToMap(alert.getDetail(), AlertsColumns.DETAIL.getColumnId(),
                    AlertsColumns.DETAIL.getLabelKey(), tmpMap, addColumn);
            }

            dataMap.add(tmpMap);
            addColumn = false;
        }
    }
}
