package com.idera.sqlcm.ui.components.alerts;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.report.IderaBaseReport;
import com.idera.sqlcm.entities.CMAlert;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

public class AlertsReport extends IderaBaseReport {

    protected List<LinkedHashMap<String, Object>> dataMap = new ArrayList<>();
    protected LinkedHashMap<String, ColumnProperties> columnPropertiesMap = new LinkedHashMap<>();

    public AlertsReport(String reportTitle, String reportSubTitle, String reportName) {
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
        return I18NStrings.NO_INSTANCES_FOUND;
    }

    public void setDataForSummary(List<AlertMetric> data) {
        boolean addColumn = true;
        Integer totalItems = 0;
        for (AlertMetric alertMetric : data) {
            totalItems += alertMetric.getAlerts().size();
            LinkedHashMap<String, Object> tmpMap = new LinkedHashMap<>();
            addValidValueToMap(alertMetric.getLevel(), "STATUS", I18NStrings.STATUS, tmpMap, addColumn, true);
            addValidValueToMap(alertMetric.getTitleMessage(), "DESCRIPTION", I18NStrings.DESCRIPTION, tmpMap, addColumn);
            dataMap.add(tmpMap);
            addColumn = false;
        }
        if (totalItems > 0)
            setReportSubTitle(reportSubTitle + "\\n" + getFormattedTotalMessage(totalItems));
    }

    public void setDataForDetailedReport(List<AlertMetric> data) {
        boolean addColumn = true;
        Integer totalItems = 0;
        for (AlertMetric alertMetric : data) {
            LinkedHashMap<String, Object> tmpMap = new LinkedHashMap<>();
            totalItems += alertMetric.getAlerts().size();
            addValidValueToMap(alertMetric.getLevel(), "LEVEL", I18NStrings.LEVEL, tmpMap, addColumn, true);
            addValidValueToMap(alertMetric.getTitleMessage(), "DESCRIPTION", I18NStrings.DESCRIPTION, tmpMap, addColumn);
            dataMap.add(tmpMap);
            addColumn = false;
            for (CMAlert alert : alertMetric.getAlerts()) {
                LinkedHashMap<String, Object> detailsMap = new LinkedHashMap<>();
                addValidValueToMap(alert.getInstanceName() + " - " + alert.getSourceRule() + " - " + alert.getEventTypeName()
                        , "DESCRIPTION", I18NStrings.DESCRIPTION, detailsMap, addColumn);
                dataMap.add(detailsMap);
            }
        }
        if (totalItems > 0)
            setReportSubTitle(reportSubTitle + "\\n" + getFormattedTotalMessage(totalItems));
    }

    protected String getFormattedTotalMessage(Integer totalCount) {
        return ELFunctions.getMessageWithParams(SQLCMI18NStrings.YOU_HAVE_X_ALERTS, totalCount);
    }
}
