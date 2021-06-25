package com.idera.sqlcm.ui.instancesAlertsRule;

import ar.com.fdvs.dj.domain.constants.HorizontalAlign;

import com.idera.server.resourse.GetResource;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.entities.CMAlert;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEventDetails;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.report.SQLCMBaseReport;
import com.idera.sqlcm.ui.instanceEvents.EventsColumns;
import com.idera.sqlcm.ui.instances.filters.InstancesOptionFilterValues;
import com.idera.sqlcm.ui.instancesAlerts.AlertsColumns;
import com.idera.sqlcm.ui.instancesAlerts.filters.AlertRuleTypeEnum;
import com.idera.sqlcm.ui.instancesAlerts.filters.AlertsOptionValues;

import org.zkoss.zul.ListModelList;

import java.io.InputStream;
import java.text.DateFormat;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

public class InstancesAlertsRuleGridViewReport extends CommonGridViewReport {
    protected List<LinkedHashMap<String, Object>> dataMap = new ArrayList<>();
    protected LinkedHashMap<String, ColumnProperties> columnPropertiesMap = new LinkedHashMap<>();
    private DateFormat dateFormat;
    public InstancesAlertsRuleGridViewReport(String reportTitle, String reportSubTitle, String reportName) {
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

        List<CMAlertRules> alertsModel = new ListModelList<>();
        for (CMEntity entity : modelList) {
            if (entity instanceof CMAlertRules) {
                alertsModel.add((CMAlertRules) entity);
            }
        }

        for (CMAlertRules alert :alertsModel) {
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

            if (AlertsRuleColumns.RULE.isVisible()){
            addValidValueToMap(alert.getNames(), AlertsRuleColumns.RULE.getColumnId(),
                               AlertsRuleColumns.RULE.getLabelKey(), tmpMap, addColumn);
            }
            if (AlertsRuleColumns.RULE_TYPE.isVisible()){
            addValidValueToMap(AlertRuleTypeEnum.findLabelByKey(alert.getAlertType()), AlertsRuleColumns.RULE_TYPE.getColumnId(),
                               AlertsRuleColumns.RULE_TYPE.getLabelKey(), tmpMap, addColumn);
            }
            if (AlertsRuleColumns.SERVER.isVisible()){
            addValidValueToMap(alert.getTargetInstances(), AlertsRuleColumns.SERVER.getColumnId(),
                               AlertsRuleColumns.SERVER.getLabelKey(), tmpMap, addColumn);
            }
            if (AlertsRuleColumns.LEVEL.isVisible()){
            addValidValueToMap(AlertsOptionValues.findLabelByKey(alert.getAlertLevel()), AlertsRuleColumns.LEVEL.getColumnId(),
                               AlertsRuleColumns.LEVEL.getLabelKey(), tmpMap, addColumn);
            }
            if (AlertsRuleColumns.E_MAIL.isVisible()){
            addValidValueToMap(alert.getEmailMessage()==1?"YES":"NO", AlertsRuleColumns.E_MAIL.getColumnId(),
                               AlertsRuleColumns.E_MAIL.getLabelKey(), tmpMap, addColumn);
            }
            if (AlertsRuleColumns.EVENT_LOG.isVisible()){
            addValidValueToMap(alert.getLogMessage()==1?"YES":"NO", AlertsRuleColumns.EVENT_LOG.getColumnId(),
                               AlertsRuleColumns.EVENT_LOG.getLabelKey(), tmpMap, addColumn);
            }
            if (AlertsRuleColumns.SNMP_TRAP.isVisible()){
            addValidValueToMap(alert.getSnmpTrap()==1?"YES":"NO", AlertsRuleColumns.SNMP_TRAP.getColumnId(),
                               AlertsRuleColumns.SNMP_TRAP.getLabelKey(), tmpMap, addColumn);
            }
            dataMap.add(tmpMap);
            addColumn = false;
        }
    }
}
