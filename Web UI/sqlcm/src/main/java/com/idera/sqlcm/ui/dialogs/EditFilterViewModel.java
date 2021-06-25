package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.common.grid.EventPropertiesGridViewReport;
import com.idera.sqlcm.entities.CMAlert;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMEvent;
import com.idera.sqlcm.entities.CMEventProperties;
import com.idera.sqlcm.facade.EventsFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import net.sf.jasperreports.engine.JRException;
import org.apache.commons.beanutils.PropertyUtils;
import org.apache.log4j.Logger;
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
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;
import org.zkoss.zul.Button;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;
import java.io.IOException;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

public class EditFilterViewModel {

    public static final String ZUL_URL = "~./sqlcm/dialogs/eventProperties/event-properties-dialog.zul";

    private static final Logger logger = Logger.getLogger(EditFilterViewModel.class);

    private CMEventProperties eventProperties;

    private Map<String, Object> eventPropertiesMap;

    private ListModelList<CMEvent> entitiesModel;

    Integer rowIndex;

    String instanceId;

    @Wire
    protected Button upButton;

    @Wire
    protected Button downButton;

    @Wire("#eventDetailsTab")
    Component detailsTab;

    @Wire("#eventGenetalTab")
    Component generalTab;


    public CMEventProperties getEventProperties() {
        return eventProperties;
    }

    public void setEventProperties(CMEventProperties eventProperties) {
        this.eventProperties = eventProperties;
    }

    public Map<String, Object> getEventPropertiesMap() {
        return eventPropertiesMap;
    }
/*
    public List<EventsPropertiesEnum> getEventsPropertiesEnum() {
        return Arrays.asList(EventsPropertiesEnum.values());
    }*/

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
        rowIndex = (int) args.get("rowIndex");
        entitiesModel = (ListModelList) args.get("entitiesModel");
        CMEntity entity = entitiesModel.getElementAt(rowIndex);
        if(entity instanceof CMEvent){
            instanceId =  args.get("instanceId").toString();
            loadEventProperty(instanceId, String.valueOf(entity.getId()));
        }else{
            instanceId = String.valueOf(((CMAlert)entity).getInstanceId());
            loadEventProperty(instanceId, String.valueOf(((CMAlert)entity).getAlertEventId()));
        }


    }

    @NotifyChange("eventPropertiesMap")
    private void loadEventProperty(String instanceId, String eventId) {
        try {
            eventProperties = EventsFacade.getEventProperties(instanceId, eventId,"");
        } catch (RestException ex) {
            WebUtil.showErrorBox(ex, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
        }
        eventPropertiesMap = new LinkedHashMap<String, Object>();
        HashMap<String, String> properties = EventPropertiesLabelMapper.getInstance().getPropertiesDetailsTabMap();
        for (Map.Entry<String, String> entry : properties.entrySet()) {
            try {
                String key = entry.getValue();
                Object value = PropertyUtils.getProperty(eventProperties, entry.getKey());
                eventPropertiesMap.put(key, value);
            } catch (Exception ex) {
                WebUtil.showErrorBox(ex, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
            }
        }
        updateButtons();
        detailsTab.invalidate();
        generalTab.invalidate();
    }

    @Command("loadNext")
    public void loadNext() {
        if (entitiesModel.size() - 1 > rowIndex) {
            rowIndex++;
            CMEntity entity = entitiesModel.getElementAt(rowIndex);
            if(entitiesModel.getElementAt(rowIndex) instanceof CMEvent){
                loadEventProperty(instanceId, String.valueOf(entity.getId()));
            }else{
                loadEventProperty(instanceId, String.valueOf(((CMAlert)entity).getAlertEventId()));
            }
        }
    }

    @Command("loadPrevious")
    public void loadPrevious() {
        if (rowIndex > 0) {
            rowIndex--;
            CMEntity entity = entitiesModel.getElementAt(rowIndex);
            if(entitiesModel.getElementAt(rowIndex) instanceof CMEvent){
                loadEventProperty(instanceId, String.valueOf(entity.getId()));
            }else{
                loadEventProperty(instanceId, String.valueOf(((CMAlert)entity).getAlertEventId()));
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

    private EventPropertiesGridViewReport makeEventPropertiesViewReport(){
        EventPropertiesGridViewReport viewReport= new EventPropertiesGridViewReport("Event Properties", "", "Event" + eventProperties.getEventId());
        viewReport.setDataMapEventProperties(initReportData());
        return viewReport;
    }

    private List<String> initShortReportData(){
        List<String> reportData = new LinkedList<String>();
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TIME) +  ": " + eventProperties.getStartTime());

        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_CATEGORY) +  ": " + eventProperties.getCategory());
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TYPE) +  ": " + eventProperties.getName());
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_APPLICATION) +  ": " + eventProperties.getApplicationName());
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_LOGIN) +  ": " + eventProperties.getLoginName());
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_DATABASE) +  ": " + eventProperties.getDatabaseName());
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_TARGET) +  ": " + eventProperties.getTargetObject());
        reportData.add(" ");
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_DETAILS) +  ": " + eventProperties.getDetails());
        for (Map.Entry<String, Object> entry : eventPropertiesMap.entrySet()) {
            reportData.add(entry.getKey() +  ": " + String.valueOf(entry.getValue()));
        }
        reportData.add(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_SQL_STATEMENT) +  ": " + eventProperties.getSqlStatement());
        return reportData;
    }

    private LinkedHashMap<String, Object> initReportData(){
        List<String> rep = new LinkedList<String>();
        LinkedHashMap<String, Object> reportData = new LinkedHashMap<>();
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TIME), eventProperties.getStartTime());
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_CATEGORY), eventProperties.getCategory());
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_TYPE), eventProperties.getName());
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_APPLICATION), eventProperties.getApplicationName());
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_LOGIN), eventProperties.getLoginName());
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_DATABASE), eventProperties.getDatabaseName());
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_TARGET), eventProperties.getTargetObject());
        reportData.put("", "");
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_EVENT_DETAILS), eventProperties.getDetails());
        reportData.putAll(eventPropertiesMap);
        reportData.put(ELFunctions.getLabel(SQLCMI18NStrings.EVENT_PROPERTIES_DIALOG_SQL_STATEMENT), eventProperties.getSqlStatement());
        return  reportData;
    }

    private void updateButtons() {
        upButton.setDisabled(rowIndex == 0 ?  true : false);
        downButton.setDisabled((entitiesModel.size()-1) == rowIndex ?  true : false);
    }

    public static void showEditFilterWindow(int rowIndex, ListModelList<? extends CMEntity> listModelList, long instanceId) {
        Map args = new HashMap();
        args.put("rowIndex", rowIndex);
        args.put("entitiesModel", listModelList);
        args.put("instanceId", instanceId);
        Window window = (Window) Executions.createComponents(EditFilterViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }
}

















