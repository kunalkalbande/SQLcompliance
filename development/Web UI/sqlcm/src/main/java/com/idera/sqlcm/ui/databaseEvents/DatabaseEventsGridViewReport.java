package com.idera.sqlcm.ui.databaseEvents;

import ar.com.fdvs.dj.domain.constants.HorizontalAlign;

import com.idera.server.resourse.GetResource;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.common.grid.EventAccessCheckConverter;
import com.idera.sqlcm.common.grid.EventPrivilegedUserConverter;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEventDetails;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebConstants;
import com.idera.sqlcm.ui.instanceEvents.EventsColumns;

import org.zkoss.zul.ListModelList;

import java.io.InputStream;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

public class DatabaseEventsGridViewReport extends CommonGridViewReport {
    protected List<LinkedHashMap<String, Object>> dataMap = new ArrayList<>();
    protected LinkedHashMap<String, ColumnProperties> columnPropertiesMap = new LinkedHashMap<>();
    private DateFormat dateFormat;
    private EventAccessCheckConverter accessCheckConverter;
    private EventPrivilegedUserConverter privilegedUserConverter;

    public DatabaseEventsGridViewReport(String reportTitle, String reportSubTitle, String reportName) {
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

    protected EventAccessCheckConverter getAccessCheckConverter() {
        if (accessCheckConverter == null) {
            accessCheckConverter = new EventAccessCheckConverter();
        }
        return accessCheckConverter;
    }

    protected EventPrivilegedUserConverter getPrivilegedUserConverter() {
        if (privilegedUserConverter == null) {
            privilegedUserConverter = new EventPrivilegedUserConverter();
        }
        return privilegedUserConverter;
    }

    public void setDataMapForListInstance(ListModelList<CMEntity> modelList) {
        boolean addColumn = true;

        List<CMEventDetails> eventsModel = new ListModelList<>();
        for (CMEntity entity : modelList) {
            if (entity instanceof CMEventDetails) {
                eventsModel.add((CMEventDetails) entity);
            }
        }

        for (CMEventDetails event : eventsModel) {

            LinkedHashMap<String, Object> tmpMap = new LinkedHashMap<>();

            InputStream tmpUrl = GetResource.getResource(imagesPath + imagesExt);
            if (tmpUrl != null) {
                tmpMap.put("stateImage", tmpUrl);
                if (addColumn) {
                    ColumnProperties col = new ColumnProperties(SQLCMI18NStrings.DATABASE);
                    col.setHorizontalAlign(HorizontalAlign.CENTER);
                    col.setFixedWidth(true);
                    getColumns().put("stateImage", col);
                }
            }

            if (EventsColumns.CATEGORY.isVisible()) {
                addValidValueToMap(event.getCategory(), EventsColumns.CATEGORY.getColumnId(),
                    EventsColumns.CATEGORY.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.EVENT.isVisible()) {
                addValidValueToMap(event.getEvent(), EventsColumns.EVENT.getColumnId(),
                    EventsColumns.EVENT.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.DATE.isVisible()) {
                dateFormat = new SimpleDateFormat(WebConstants.SHORT_DATE_FORMAT);
                addValidValueToMap(dateFormat.format(event.getTime()), EventsColumns.DATE.getColumnId(),
                    EventsColumns.DATE.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.TIME.isVisible()) {
                dateFormat = new SimpleDateFormat(WebConstants.TIME_FORMAT);
                addValidValueToMap(dateFormat.format(event.getTime()), EventsColumns.TIME.getColumnId(),
                    EventsColumns.TIME.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.LOGIN.isVisible()) {
                addValidValueToMap(event.getLogin(), EventsColumns.LOGIN.getColumnId(),
                    EventsColumns.LOGIN.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.DATABASE.isVisible()) {
                addValidValueToMap(event.getDatabase(), EventsColumns.DATABASE.getColumnId(),
                    EventsColumns.DATABASE.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.TARGET_OBJECT.isVisible()) {
                addValidValueToMap(event.getTargetObject(), EventsColumns.TARGET_OBJECT.getColumnId(),
                    EventsColumns.TARGET_OBJECT.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.DETAILS.isVisible()) {
                addValidValueToMap(event.getDetails(), EventsColumns.DETAILS.getColumnId(),
                    EventsColumns.DETAILS.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.SPID.isVisible()) {
                addValidValueToMap(event.getSpid(), EventsColumns.SPID.getColumnId(),
                    EventsColumns.SPID.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.APPLICATION.isVisible()) {
                addValidValueToMap(event.getApplication(), EventsColumns.APPLICATION.getColumnId(),
                    EventsColumns.APPLICATION.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.HOST.isVisible()) {
                addValidValueToMap(event.getHost(), EventsColumns.HOST.getColumnId(),
                    EventsColumns.HOST.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.SERVER.isVisible()) {
                addValidValueToMap(event.getServer(), EventsColumns.SERVER.getColumnId(),
                    EventsColumns.SERVER.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.ACCESS_CHECK.isVisible()) {
                addValidValueToMap(getAccessCheckConverter().coerceToUi(event, null, null),
                    EventsColumns.ACCESS_CHECK.getColumnId(), EventsColumns.ACCESS_CHECK.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.DATABASE_USER.isVisible()) {
                addValidValueToMap(event.getDatabaseUser(), EventsColumns.DATABASE_USER.getColumnId(),
                    EventsColumns.DATABASE_USER.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.OBJECT.isVisible()) {
                addValidValueToMap(event.getObject(), EventsColumns.OBJECT.getColumnId(),
                    EventsColumns.OBJECT.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.TARGET_LOGIN.isVisible()) {
                addValidValueToMap(event.getTargetLogin(), EventsColumns.TARGET_LOGIN.getColumnId(),
                    EventsColumns.TARGET_LOGIN.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.TARGET_USER.isVisible()) {
                addValidValueToMap(event.getTargetUser(), EventsColumns.TARGET_USER.getColumnId(),
                    EventsColumns.TARGET_USER.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.ROLE.isVisible()) {
                addValidValueToMap(event.getRole(), EventsColumns.ROLE.getColumnId(),
                    EventsColumns.ROLE.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.OWNER.isVisible()) {
                addValidValueToMap(event.getOwner().toLowerCase(), EventsColumns.OWNER.getColumnId().toLowerCase(),
                    EventsColumns.OWNER.getLabelKey().toLowerCase(), tmpMap, addColumn);
            }

            if (EventsColumns.PRIVILEGED_USER.isVisible()) {
                addValidValueToMap(getPrivilegedUserConverter().coerceToUi(event, null, null),
                    EventsColumns.PRIVILEGED_USER.getColumnId(), EventsColumns.PRIVILEGED_USER.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.SESSION_LOGIN.isVisible()) {
                addValidValueToMap(event.getSessionLogin(), EventsColumns.SESSION_LOGIN.getColumnId(),
                    EventsColumns.SESSION_LOGIN.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.AUDITED_UPDATES.isVisible()) {
                addValidValueToMap(event.getAuditedUpdates(), EventsColumns.AUDITED_UPDATES.getColumnId(),
                    EventsColumns.AUDITED_UPDATES.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.PRIMARY_KEY.isVisible()) {
                addValidValueToMap(event.getPrimaryKey(), EventsColumns.PRIMARY_KEY.getColumnId(),
                    EventsColumns.PRIMARY_KEY.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.TABLE.isVisible()) {
                addValidValueToMap(event.getTable(), EventsColumns.TABLE.getColumnId(),
                    EventsColumns.TABLE.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.COLUMN.isVisible()) {
                addValidValueToMap(event.getColumn(), EventsColumns.COLUMN.getColumnId(),
                    EventsColumns.COLUMN.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.BEFORE_VALUE.isVisible()) {
                addValidValueToMap(event.getBeforeValue(), EventsColumns.BEFORE_VALUE.getColumnId(),
                    EventsColumns.BEFORE_VALUE.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.AFTER_VALUE.isVisible()) {
                addValidValueToMap(event.getAfterValue(), EventsColumns.AFTER_VALUE.getColumnId(),
                    EventsColumns.AFTER_VALUE.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.SCHEMA.isVisible()) {
                addValidValueToMap(event.getSchema(), EventsColumns.SCHEMA.getColumnId(),
                    EventsColumns.SCHEMA.getLabelKey(), tmpMap, addColumn);
            }

            if (EventsColumns.COLUMNS_UPDATED.isVisible()) {
                addValidValueToMap(event.getColumnsUpdated(), EventsColumns.COLUMNS_UPDATED.getColumnId(),
                    EventsColumns.COLUMNS_UPDATED.getLabelKey(), tmpMap, addColumn);
            }

            dataMap.add(tmpMap);
            addColumn = false;
        }
    }
}
