package com.idera.sqlcm.ui.instances;

import ar.com.fdvs.dj.domain.constants.HorizontalAlign;
import com.idera.i18n.I18NStrings;
import com.idera.server.resourse.GetResource;
import com.idera.server.web.report.IderaBaseReport;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.Instance;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import org.zkoss.zul.ListModelList;

import java.io.InputStream;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

public class InstancesGridViewReport extends CommonGridViewReport {
    protected List<LinkedHashMap<String, Object>> dataMap = new ArrayList<>();
    protected LinkedHashMap<String, IderaBaseReport.ColumnProperties> columnPropertiesMap = new LinkedHashMap<>();

    public InstancesGridViewReport(String reportTitle, String reportSubTitle, String reportName) {
        super(reportTitle, reportSubTitle, reportName);
    }

    @Override
    protected List<LinkedHashMap<String, Object>> getData() {
        return dataMap;
    }

    @Override
    protected LinkedHashMap<String, IderaBaseReport.ColumnProperties> getColumns() {
        return columnPropertiesMap;
    }

    @Override
    protected String getMessageWhenNoData() {
        return I18NStrings.NO_INSTANCES_FOUND;
    }

    public void setDataMapForListInstance(ListModelList<CMEntity> modelList) {
        boolean addColumn = true;

        List<Instance> instancesModel = new ListModelList<>();
        for (CMEntity entity : modelList) {
            if (entity instanceof Instance) {
                instancesModel.add((Instance) entity);
            }
        }

        for (Instance instance : instancesModel) {

            LinkedHashMap<String, Object> tmpMap = new LinkedHashMap<>();

            InputStream tmpUrl = GetResource.getResource(imagesPath + instance.getStatus().getIconURL() + imagesExt);
            if (tmpUrl != null) {
                tmpMap.put("stateImage", tmpUrl);
                if (addColumn) {
                    ColumnProperties col = new ColumnProperties(SQLCMI18NStrings.STATUS);
                    col.setHorizontalAlign(HorizontalAlign.CENTER);
                    col.setFixedWidth(true);
                    getColumns().put("stateImage", col);
                }
            }
            addValidValueToMap(instance.getInstanceName(), InstancesColumns.INSTANCE_NAME.getColumnId(),
                               InstancesColumns.INSTANCE_NAME.getLabelKey(), tmpMap, addColumn);
            addValidValueToMap(instance.getStatusText(), InstancesColumns.STATUS_TEXT.getColumnId(),
                               InstancesColumns.STATUS_TEXT.getLabelKey(), tmpMap, addColumn);
            addValidValueToMap(instance.getNumberOfAuditedDatabases(), InstancesColumns.NUMBER_OF_AUDITED_DB.getColumnId(),
                               InstancesColumns.NUMBER_OF_AUDITED_DB.getLabelKey(), tmpMap, addColumn);
            addValidValueToMap(instance.getSqlServerVersionEdition(), InstancesColumns.SQL_SERVER_VERSION_EDITION.getColumnId(),
                               InstancesColumns.SQL_SERVER_VERSION_EDITION.getLabelKey(), tmpMap, addColumn);
            addValidValueToMap(instance.getAuditStatus(), InstancesColumns.AUDIT_STATUS.getColumnId(),
                               InstancesColumns.AUDIT_STATUS.getLabelKey(), tmpMap, addColumn);
            addValidValueToMap(instance.getLastAgentContact(), InstancesColumns.LAST_AGENT_CONTACT.getColumnId(),
                               InstancesColumns.LAST_AGENT_CONTACT.getLabelKey(), tmpMap, addColumn);

            dataMap.add(tmpMap);
            addColumn = false;
        }

    }

}
