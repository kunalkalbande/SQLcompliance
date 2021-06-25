package com.idera.sqlcm.ui.components.alerts;

import com.google.common.base.Objects;
import com.idera.i18n.I18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.ModalDialogTemplate;
import net.sf.jasperreports.engine.JRException;
import org.zkoss.zk.ui.Execution;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zkplus.databind.BindingListModelList;
import org.zkoss.zul.Grid;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radio;
import org.zkoss.zul.Window;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class AlertsExportDialog extends ModalDialogTemplate {

    public static final String ALERTS_REPORT_FILE_NAME = "AlertsSummary";
    public static final String ZUL_URL = "~./sqlcm/components/alerts/alertsExportDialog.zul";
    public static final String ALERTS_LIST = "alertsList";
    protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(AlertsExportDialog.class);
    protected static final org.apache.log4j.Logger debug = org.apache.log4j.Logger.getLogger(AlertsExportDialog.class);
    private static final long serialVersionUID = 1L;
    public static List<AlertMetric> alertMetrics = new ArrayList<>();
    public ListModelList<SelectedEntry> alertsListModel = new BindingListModelList<>(new ArrayList<SelectedEntry>(), false);
    protected AnnotateDataBinder binder = null;
    protected AlertsReport report;
    protected String subTitleAddition = "";

    @Wire
    protected Radio exportSummaryRadio;
    @Wire
    protected Radio exportDetailedRadio;
    @Wire
    protected Radio exportSelectedRadio;
    @Wire
    protected Radio exportAsPDFRadio;
    @Wire
    protected Radio exportAsXLSRadio;
    @Wire
    protected Radio exportAsXMLRadio;
    @Wire
    protected Grid alertsGrid;

    @Override
    public void doAfterCompose(Window comp) throws Exception {
        super.doAfterCompose(comp);
        binder = new AnnotateDataBinder(comp);
        final Execution execution = Executions.getCurrent();
        if (execution.getArg().get(ALERTS_LIST) != null && (execution.getArg().get(ALERTS_LIST) instanceof List)) {
            alertMetrics = (List<AlertMetric>) execution.getArg().get(ALERTS_LIST);
        }
        binder = new AnnotateDataBinder(comp);
        for (AlertMetric alertMetric : alertMetrics) {
            alertsListModel.add(new SelectedEntry(alertMetric.getLevel().getLabel() + " " + alertMetric.getTitleMessage(), false, alertMetric));
        }
        binder.bindBean("showSelectionGrid", !exportSelectedRadio.isChecked());
        binder.bindBean("alertsListModel", alertsListModel);
        binder.loadAll();
    }

    @Listen("onClick = #closeButton")
    public void onClickCloseButton(Event evt) {
        getSelf().detach();
    }

    @Listen("onClick = #exportButton")
    public void onClickExportButton(Event evt) {
        if (exportSummaryRadio.isChecked()) {
            generateReportSummary();
        } else if (exportDetailedRadio.isChecked()) {
            generateDetailedReport();
        } else {
            generateSelectedReport();
        }
        getSelf().detach();
    }

    @Listen("onClick = #alertsRadioGroup")
    public void onClickRecommendationRadioGroup(Event evt) {
        binder.bindBean("showSelectionGrid", !exportSelectedRadio.isChecked());
        binder.loadAll();
    }

    protected void generateReportSummary() {
        report = new AlertsReport("Alerts", "Summary Report", ALERTS_REPORT_FILE_NAME);
        List<AlertMetric> actionItems = alertMetrics;
        report.setDataForSummary(actionItems);
        generateReports();
    }

    protected void generateDetailedReport() {
        report = new AlertsReport("Alerts", "Detailed Report", ALERTS_REPORT_FILE_NAME);
        List<AlertMetric> actionItems = alertMetrics;
        report.setDataForDetailedReport(actionItems);
        generateReports();
    }

    protected void generateSelectedReport() {
        List<AlertMetric> selectedList = new ArrayList<>();
        report = new AlertsReport("Alerts", "Selected Report", ALERTS_REPORT_FILE_NAME);
        for (SelectedEntry tmpEntry : alertsListModel) {
            if (tmpEntry.getIsSelected()) {
                selectedList.add((AlertMetric) tmpEntry.getData());
            }
        }
        if (selectedList.isEmpty()) {
            throw new WrongValueException(alertsGrid, ELFunctions.getMessage(I18NStrings.SELECT_CATEGORY_TO_EXPORT));
        }
        report.setDataForDetailedReport(selectedList);
        generateReports();
    }

    protected void generateReports() {
        try {
            if (exportAsPDFRadio.isChecked()) {
                report.generatePDFReport();
            } else if (exportAsXLSRadio.isChecked()) {
                report.generateXLSReport();
            } else if (exportAsXMLRadio.isChecked()) {
                report.generateXMLReport();
            }
        } catch (JRException | IOException | TransformerException | ParserConfigurationException ex) {
            logger.error(ex, I18NStrings.FAILED_TO_EXPORT_REPORT);
            WebUtil.showErrorBox(ex, I18NStrings.FAILED_TO_EXPORT_REPORT);
        }
    }

    public static class SelectedEntry {
        protected String name;
        protected Object data = null;
        protected boolean isSelected = false;

        public SelectedEntry() {
        }

        public SelectedEntry(String name, boolean isSelected) {
            this.name = name;
            this.isSelected = isSelected;
        }

        public SelectedEntry(String name, boolean isSelected, Object data) {
            this.name = name;
            this.isSelected = isSelected;
            this.data = data;
        }

        public String getName() {
            return name;
        }

        public void setName(String name) {
            this.name = name;
        }

        public boolean getIsSelected() {
            return isSelected;
        }

        public void setIsSelected(boolean isSelected) {
            this.isSelected = isSelected;
        }

        public Object getData() {
            return data;
        }

        public void setData(String data) {
            this.data = data;
        }

        @Override
        public int hashCode() {
            return Objects.hashCode(name, data, isSelected);
        }

        @Override
        public boolean equals(Object o) {
            if (this == o) return true;
            if (o == null || getClass() != o.getClass()) return false;
            SelectedEntry that = (SelectedEntry) o;
            return Objects.equal(this.name, that.name) &&
                    Objects.equal(this.data, that.data) &&
                    Objects.equal(this.isSelected, that.isSelected);
        }
    }
}
