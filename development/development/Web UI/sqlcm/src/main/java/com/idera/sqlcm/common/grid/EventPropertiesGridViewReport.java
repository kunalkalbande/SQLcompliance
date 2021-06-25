package com.idera.sqlcm.common.grid;

import ar.com.fdvs.dj.domain.constants.HorizontalAlign;
import ar.com.fdvs.dj.domain.constants.VerticalAlign;
import com.idera.server.resourse.GetResource;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEventDetails;
import com.idera.sqlcm.entities.CMEventProperties;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.report.SQLCMBaseReport;
import com.idera.sqlcm.ui.databaseEvents.EventsColumns;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.ListModelMap;

import java.io.InputStream;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public class EventPropertiesGridViewReport extends SQLCMBaseReport {
    protected List<LinkedHashMap<String, Object>> dataMap = new ArrayList<>();
    protected LinkedHashMap<String, ColumnProperties> columnPropertiesMap = new LinkedHashMap<>();

    public EventPropertiesGridViewReport(String reportTitle, String reportSubTitle, String reportName) {
        super(reportTitle, reportSubTitle, reportName);
    }

    public EventPropertiesGridViewReport(String reportTitle) {
        super(reportTitle);
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

    public void setDataMapEventProperties(Map<String, Object> modelList) {
        boolean addColumn = true;
        for(Map.Entry<String, Object> entry : modelList.entrySet()) {
            LinkedHashMap<String, Object> tmpMap = new LinkedHashMap<>();
            InputStream tmpUrl = GetResource.getResource(imagesPath + imagesExt);
            if (tmpUrl != null) {
                tmpMap.put("stateImage", tmpUrl);
                if (addColumn) {
                    ColumnProperties col = new ColumnProperties("value");
                    col.setHorizontalAlign(HorizontalAlign.CENTER);
                    col.setFixedWidth(false);
                    getColumns().put("stateImage", col);
                }
            }
            String key = entry.getKey();
            Object value = entry.getValue();
            addValidValueToMap(key, "key", SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_PROPERTY, tmpMap, addColumn);
            addValidValueToMap(value, "value", SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_VALUE, tmpMap, addColumn);
        dataMap.add(tmpMap);
        addColumn = false;
    }
}

    public void setDataEventProperties(List<String> modelList) {
        boolean addColumn = true;
        for(String entry : modelList) {
            LinkedHashMap<String, Object> tmpMap = new LinkedHashMap<>();
            addValidValueToMap(entry, "key", "", tmpMap, addColumn);
            dataMap.add(tmpMap);
            addColumn = false;
        }
    }
}