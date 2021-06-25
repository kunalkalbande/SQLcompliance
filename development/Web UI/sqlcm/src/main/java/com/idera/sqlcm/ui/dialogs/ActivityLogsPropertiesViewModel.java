package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.common.grid.EventPropertiesGridViewReport;
import com.idera.sqlcm.entities.CMActivityLogs;
import com.idera.sqlcm.entities.CMAlert;
import com.idera.sqlcm.entities.CMEPBeforeAfterValue;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEvent;
import com.idera.sqlcm.entities.CMEventProperties;
import com.idera.sqlcm.facade.ActivityLogsFacade;
import com.idera.sqlcm.facade.EventsFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;

import net.sf.jasperreports.engine.JRException;

import org.apache.commons.beanutils.PropertyUtils;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.Hlayout;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Tabbox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;

import java.io.IOException;
import java.text.Format;
import java.text.SimpleDateFormat;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

public class ActivityLogsPropertiesViewModel {

    public static final String ZUL_URL = "~./sqlcm/dialogs/activityLogsProperties/activity-logs-dialog.zul";

    private static final Logger logger = Logger.getLogger(ActivityLogsPropertiesViewModel.class);
    public static final String ROW_INDEX_ARG = "rowIndex";
    public static final String ENTITIES_MODEL_ARG = "entitiesModel";
    public static final String INSTANCE_ID_ARG = "instanceId";
    public static final String CMACTIVITY_LOGS = "activityLogs";
    private CMActivityLogs eventProperties;

    private Map<String, Object> eventPropertiesMap;

    private ListModelList<CMEvent> entitiesModel;

    Integer rowIndex;

    String instanceId;
    
    @Wire
    private Textbox txtEventTime;

    @Wire
    Hlayout errorLayout;

    @Wire
    Hlayout mainLayout;

    private boolean disabledDownButton;

    private boolean disabledUpButton;

    @Wire("#eventDetailsTab")
    Component detailsTab;

    @Wire("#eventGenetalTab")
    Component generalTab;

    @Wire
    Tabbox tb;

    public boolean isDisabledDownButton() {
        return disabledDownButton;
    }

    public boolean isDisabledUpButton() {
        return disabledUpButton;
    }

    public CMActivityLogs getEventProperties() {
        return eventProperties;
    }

    public void setEventProperties(CMActivityLogs eventProperties) {
        this.eventProperties = eventProperties;
    }

    public Map<String, Object> getEventPropertiesMap() {
        return eventPropertiesMap;
    }

    public void setEventPropertiesMap(Map<String, Object> eventPropertiesMap) {
        this.eventPropertiesMap = eventPropertiesMap;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        initEventProperties();
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    private void initEventProperties() {
    	 HashMap<String, Object> args = (HashMap<String, Object>) Executions.getCurrent().getArg();
         rowIndex = (int) args.get(ROW_INDEX_ARG);
         entitiesModel = (ListModelList) args.get(ENTITIES_MODEL_ARG);
         CMEntity entity = entitiesModel.getElementAt(rowIndex);
        if (entity instanceof CMEvent) {
        	instanceId = args.get(INSTANCE_ID_ARG).toString();
            loadEventProperty(instanceId, String.valueOf(entity.getId()), ((CMEvent) entity).getEventDatabase());
        } else {
            instanceId = String.valueOf(((CMActivityLogs) entity).getInstanceName());
            loadEventProperty(instanceId, String.valueOf(((CMActivityLogs) entity).getEventId()), null);
        }
    }

    @NotifyChange({"eventPropertiesMap", "disabledUpButton", "disabledDownButton"})
    private void loadEventProperty(String instanceId, String eventId, String eventDatabase) {
        try {
        	eventProperties = ActivityLogsFacade.getActivityProperties(eventId);
            if (eventProperties == null) {
                throw new RestException("");
            }
            eventPropertiesMap = new LinkedHashMap<String, Object>();
            HashMap<String, String> properties = ActivityLogsPropertiesLabelMapper.getInstance().getPropertiesDetailsTabMap();
            for (Map.Entry<String, String> entry : properties.entrySet()) {
                String key = entry.getValue();
                Object value = PropertyUtils.getProperty(eventProperties, entry.getKey());
                eventPropertiesMap.put(key, value);
            }
            updateButtons();
            BindUtils.postNotifyChange(null, null, this,"eventProperties");
        } catch (Exception ex) {
            mainLayout.setVisible(false);
            errorLayout.setVisible(true);
        }
    }

    @NotifyChange({"eventPropertiesMap", "disabledUpButton", "disabledDownButton"})
    @Command("loadNext")
    public void loadNext() {
        if (entitiesModel.size() - 1 > rowIndex) {
            rowIndex++;
            CMEntity entity = entitiesModel.getElementAt(rowIndex);
            if (entitiesModel.getElementAt(rowIndex) instanceof CMEvent) {
                loadEventProperty(instanceId, String.valueOf(entity.getId()), ((CMEvent) entity).getEventDatabase());
            } else {
                loadEventProperty(instanceId, String.valueOf(((CMActivityLogs) entity).getEventId()), null);
            }
        }
    }

    @NotifyChange({"eventPropertiesMap", "disabledUpButton", "disabledDownButton"})
    @Command("loadPrevious")
    public void loadPrevious() {
        if (rowIndex > 0) {
            rowIndex--;
            CMEntity entity = entitiesModel.getElementAt(rowIndex);
            if (entitiesModel.getElementAt(rowIndex) instanceof CMEvent) {
                loadEventProperty(instanceId, String.valueOf(entity.getId()), ((CMEvent) entity).getEventDatabase());
            } else {
                loadEventProperty(instanceId, String.valueOf(((CMActivityLogs) entity).getEventId()), null);
            }
        }
    }

    @Command("copyToPdf")
    public void copyToPdf() throws JRException, IOException {
        makeEventPropertiesViewReport().generatePDFReport();
    }

    @Command("copyToExcel")
    public void copyToExcel() throws JRException, IOException {
        makeEventPropertiesViewReport().generateXLSReport();
    }

    @Command("copyToXml")
    public void copyToXml() throws JRException, IOException, TransformerException, ParserConfigurationException {
        makeEventPropertiesViewReport().generateXMLReport();
    }

    @Command("copyToTxt")
    public void copyToTxt() throws IOException, JRException {
        EventPropertiesGridViewReport eventPropertiesViewReport = new EventPropertiesGridViewReport(String.valueOf(eventProperties.getEventId()));
        eventPropertiesViewReport.setDataEventProperties(initShortReportData());
        eventPropertiesViewReport.generateTXTReport();
    }

    private EventPropertiesGridViewReport makeEventPropertiesViewReport() {
        EventPropertiesGridViewReport viewReport = new EventPropertiesGridViewReport("Activity Logs Properties", "", "Event" + eventProperties.getEventId());
        viewReport.setDataMapEventProperties(initReportData());
        return viewReport;
    }

    private List<String> initShortReportData() {
        List<String> reportData = new LinkedList<String>();
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TIME) + ": " + eventProperties.getEventTime());
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TYPE) + ": " + eventProperties.getEventType());
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_TARGET) + ": " + eventProperties.getInstanceName());
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_DETAILS) + ": " + eventProperties.getDetail());
        return reportData;
    }

    private LinkedHashMap<String, Object> initReportData() {
        List<String> rep = new LinkedList<String>();
        LinkedHashMap<String, Object> reportData = new LinkedHashMap<>();
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TIME), eventProperties.getEventTime());
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TYPE), eventProperties.getEventType());
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_TARGET), eventProperties.getInstanceName());
        reportData.put("", "");
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_DETAILS), eventProperties.getDetail());
        reportData.putAll(eventPropertiesMap);
        return reportData;
    }

    private void updateButtons() {
        disabledUpButton = rowIndex == 0 ? true : false;
        disabledDownButton = (entitiesModel.size() - 1) == rowIndex ? true : false;
    }

    public static void showEventPropertiesWindow(int rowIndex, ListModelList<? extends CMEntity> listModelList, long instanceId) {
        Map args = new HashMap();
        args.put(ROW_INDEX_ARG, rowIndex);
        args.put(ENTITIES_MODEL_ARG, listModelList);
        args.put(INSTANCE_ID_ARG, instanceId);
        Window window = (Window) Executions.createComponents(ActivityLogsPropertiesViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }
}

















