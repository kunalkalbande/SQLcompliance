package com.idera.server.web.report;

import ar.com.fdvs.dj.core.DynamicJasperHelper;
import ar.com.fdvs.dj.core.layout.ClassicLayoutManager;
import ar.com.fdvs.dj.domain.AutoText;
import ar.com.fdvs.dj.domain.DynamicReport;
import ar.com.fdvs.dj.domain.ImageBanner;
import ar.com.fdvs.dj.domain.Style;
import ar.com.fdvs.dj.domain.builders.ColumnBuilder;
import ar.com.fdvs.dj.domain.builders.FastReportBuilder;
import ar.com.fdvs.dj.domain.constants.*;
import ar.com.fdvs.dj.domain.constants.Font;
import ar.com.fdvs.dj.domain.constants.Transparency;
import ar.com.fdvs.dj.domain.entities.columns.AbstractColumn;

import com.idera.i18n.I18NStrings;
import com.idera.server.resourse.GetResource;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import net.sf.jasperreports.engine.*;
import net.sf.jasperreports.engine.data.JRBeanCollectionDataSource;
import net.sf.jasperreports.engine.export.*;

import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.zkoss.util.media.AMedia;
import org.zkoss.zul.Filedownload;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import java.awt.*;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.*;
import java.util.List;
import java.util.Map.Entry;

public abstract class IderaBaseReport implements IderaReport {

    protected static final String ONLY_ALPHANUMERIC_REGEX = "[^a-zA-Z0-9]+";
    protected static final String IS_NUMBER_REGEX = "-?\\d+(\\.\\d+)?";
    protected String imagesPath;
    protected String imagesExt;
    protected String ideraLogoPath;
    protected int ideraLogoWidth;
    protected int ideraLogoHeight;
    protected String ideraProductLogoPath;
    protected int ideraProductLogoWidth;
    protected int ideraProductLogoHeight;
    protected String ideraProductName;
    protected String reportName;
    protected JasperPrint generatedPrint = null;
    protected String reportTitle;
    protected String reportSubTitle;
    protected Style headerStyle = new Style();
    protected FastReportBuilder reportTemplate = new FastReportBuilder();
    DateFormat dateFormat = new SimpleDateFormat("MM/dd/yyyy HH:mm");
    public IderaBaseReport(String reportName) {
        dateFormat.setTimeZone(Utils.getClientTimeZone());
        initProperties();
        this.reportName = reportName;
        reportTemplate.setUseFullPageWidth(true);
        reportTemplate.setLeftMargin(4).setTopMargin(0).setHeaderHeight(0);//setLeftMargin(0);//.setHeaderHeight(0);
    }

    public IderaBaseReport(String reportTitle, String reportSubTitle, String reportName) {
        dateFormat.setTimeZone(Utils.getClientTimeZone());
        initProperties();
        this.reportName = reportName;
        this.reportTitle = reportTitle;
        this.reportSubTitle = reportSubTitle;

        Style titleStyle = new Style();
        titleStyle.setBorderTop(new Border(Border.BORDER_WIDTH_NONE, Border.BORDER_STYLE_SOLID));
        titleStyle.setFont(Font.VERDANA_BIG_BOLD);
        titleStyle.setBackgroundColor(Color.DARK_GRAY);
        titleStyle.setTextColor(Color.WHITE);
        titleStyle.setTransparency(Transparency.OPAQUE);

        Style oddRowStyle = new Style();
        oddRowStyle.setBorder(Border.NO_BORDER());
        oddRowStyle.setBackgroundColor(Color.LIGHT_GRAY);
        oddRowStyle.setTransparency(Transparency.OPAQUE);

        reportTemplate.setTitleStyle(titleStyle).setTitle(reportTitle).setSubtitle(reportSubTitle)
                .setLeftMargin(10).setRightMargin(10).setTopMargin(20).setBottomMargin(20)
                .setTitleHeight(10).setPrintBackgroundOnOddRows(true).setOddRowBackgroundStyle(oddRowStyle);

        if (GetResource.getResourceURL(this.ideraLogoPath) != null) {
            reportTemplate.addImageBanner(GetResource.getResourceURL(this.ideraLogoPath).toString(), this.ideraLogoWidth, this.ideraLogoHeight,
                    ImageBanner.ALIGN_LEFT, ImageScaleMode.REAL_SIZE);
        }
        if (GetResource.getResourceURL(this.ideraProductLogoPath) != null) {
            reportTemplate.addImageBanner(GetResource.getResourceURL(this.ideraProductLogoPath).toString(), this.ideraProductLogoWidth,
                    this.ideraProductLogoHeight,
                    ImageBanner.ALIGN_RIGHT, ImageScaleMode.REAL_SIZE);
        }

        reportTemplate.setUseFullPageWidth(true);

        headerStyle.setFont(Font.VERDANA_MEDIUM);
        headerStyle.setBorderBottom(Border.PEN_2_POINT());
        headerStyle.setHorizontalAlign(HorizontalAlign.LEFT);
        headerStyle.setVerticalAlign(VerticalAlign.JUSTIFIED);
        headerStyle.setBackgroundColor(Color.WHITE);
        headerStyle.setTextColor(Color.BLACK);
        headerStyle.setTransparency(Transparency.OPAQUE);
        headerStyle.setPaddingTop(20);

        reportTemplate.addAutoText(dateFormat.format(new Date()), AutoText.POSITION_FOOTER, AutoText.ALIGMENT_RIGHT);
        reportTemplate.addAutoText(AutoText.AUTOTEXT_PAGE_X_SLASH_Y, AutoText.POSITION_FOOTER, AutoText.ALIGMENT_CENTER);
        reportTemplate.setAllowDetailSplit(false);
    }

    public static List<String> translateToExportableList(List<String> listOfFilters) {
        List<String> returnList = new ArrayList<String>();
        for (String tmpString : listOfFilters) {
            returnList.add(tmpString.isEmpty() ? ELFunctions.getLabel(I18NStrings.NOT_SPECIFIED)
                    : tmpString.replace("\\", "\\\\"));
        }
        return returnList;
    }

    protected void initProperties() {
        imagesPath = IderaBaseReportPreferences.IMAGES_PATH;
        imagesExt = IderaBaseReportPreferences.IMAGES_EXT;
        ideraLogoPath = IderaBaseReportPreferences.IDERA_LOGO_PATH;
        ideraLogoWidth = IderaBaseReportPreferences.IDERA_LOGO_WIDTH;
        ideraLogoHeight = IderaBaseReportPreferences.IDERA_LOGO_HEIGHT;
        ideraProductLogoPath = IderaBaseReportPreferences.IDERA_PRODUCT_LOGO_PATH;
        ideraProductLogoWidth = IderaBaseReportPreferences.IDERA_PRODUCT_LOGO_WIDTH;
        ideraProductLogoHeight = IderaBaseReportPreferences.IDERA_PRODUCT_LOGO_HEIGHT;
        ideraProductName = IderaBaseReportPreferences.IDERA_PRODUCT_NAME;
    }

    protected abstract List<LinkedHashMap<String, Object>> getData();

    protected abstract LinkedHashMap<String, ColumnProperties> getColumns();

    protected String getMessageWhenNoData() {
        return I18NStrings.NO_RESULTS_FOUND;
    }

    protected void setReportTitel(String title) {
        reportTemplate.setTitle(title);
    }

    protected void setReportSubTitle(String subTitle) {
        reportTemplate.setSubtitle(subTitle);
    }

    @Override
    public JasperPrint getJasperPrint(List<LinkedHashMap<String, Object>> data, boolean ignorePaging)
            throws JRException {

        if (generatedPrint != null)
            return generatedPrint;

        // if the list is empty and there is no data
        if (getColumns().isEmpty() && data.isEmpty()) {
            ColumnProperties tmpProperty = new ColumnProperties(getMessageWhenNoData());
            tmpProperty.setHorizontalAlign(HorizontalAlign.CENTER);
            getColumns().put("NO_RESULTS_FOUND", tmpProperty);
            LinkedHashMap<String, Object> tmpMap = new LinkedHashMap<String, Object>();
            tmpMap.put("NO_RESULTS_FOUND", "");
            data.add(tmpMap);
            headerStyle.setHorizontalAlign(HorizontalAlign.CENTER);
        }

        for (Entry<String, ColumnProperties> columnProperty : getColumns().entrySet()) {
            AbstractColumn columnSqlServer = null;
            Style columnStyle = new Style();
            columnStyle.setHorizontalAlign(columnProperty.getValue().getHorizontalAlign());
            columnStyle.setVerticalAlign(columnProperty.getValue().getVerticalAlign());
            columnStyle.setFont(Font.VERDANA_SMALL);
            columnStyle.setStreching(Stretching.RELATIVE_TO_TALLEST_OBJECT);
            columnStyle.setStretchWithOverflow(true);
            if (columnProperty.getKey().endsWith("Image")) {
                columnSqlServer = ColumnBuilder.getNew().setColumnProperty(columnProperty.getKey(), Object.class.getName())
                        .setColumnType(ColumnBuilder.COLUMN_TYPE_IMAGE).setImageScaleMode(ImageScaleMode.REAL_SIZE).setTitle(
                                columnProperty.getValue().getColumnValue().isEmpty() ? "" : ELFunctions
                                        .getLabel(columnProperty.getValue().getColumnValue())).setHeaderStyle(headerStyle).setStyle(columnStyle)
                        .build();
            } else
                columnSqlServer = ColumnBuilder.getNew().setColumnProperty(columnProperty.getKey(), Object.class.getName()).setTitle(
                        columnProperty.getValue().getColumnValue().isEmpty() ? "" : ELFunctions.getLabel(columnProperty.getValue().getColumnValue()))
                        .setHeaderStyle(headerStyle).setStyle(columnStyle).build();
            columnSqlServer.setFixedWidth(columnProperty.getValue().getFixedWidth());

            reportTemplate.addColumn(columnSqlServer);
        }



        DynamicReport dynamicReport = reportTemplate.build();
        JasperReport report = DynamicJasperHelper.generateJasperReport(dynamicReport, new ClassicLayoutManager(), new HashMap<String, Object>());



        JRDataSource dataSource = new JRBeanCollectionDataSource(data);

        HashMap<String, Object> optionsMap = new HashMap<String, Object>();
        if (ignorePaging) {
            optionsMap.put(JRParameter.IS_IGNORE_PAGINATION, Boolean.TRUE);
        }
        generatedPrint = JasperFillManager.fillReport(report, optionsMap, dataSource);

        return generatedPrint;
    }

    public void generatePDFReport() throws JRException, IOException {

        ByteArrayOutputStream output = new ByteArrayOutputStream();
        try {
            JasperPrint jasperPrint = getJasperPrint(getData(), false);
            JRPdfExporter exporter = new JRPdfExporter();
            exporter.setParameter(JRPdfExporterParameter.JASPER_PRINT, jasperPrint);
            exporter.setParameter(JRPdfExporterParameter.OUTPUT_STREAM, output);

            exporter.exportReport();
            final InputStream mediais = new ByteArrayInputStream(output.toByteArray());
            final AMedia amedia = new AMedia(reportName + ".pdf", "pdf", "application/pdf", mediais);
            Filedownload.save(amedia);
        } finally {
            output.close();
        }
    }

    public void generateXLSReport() throws JRException, IOException {

        ByteArrayOutputStream output = new ByteArrayOutputStream();
        try {
            JasperPrint jasperPrint = getJasperPrint(translateFromStringToNativeType(getData()), true);
            JRXlsExporter exporter = new JRXlsExporter();
            exporter.setParameter(JRXlsExporterParameter.JASPER_PRINT, jasperPrint);
            exporter.setParameter(JRXlsExporterParameter.OUTPUT_STREAM, output);
            exporter.setParameter(JRXlsExporterParameter.IS_ONE_PAGE_PER_SHEET, Boolean.FALSE);
            exporter.setParameter(JRXlsExporterParameter.IS_DETECT_CELL_TYPE, Boolean.TRUE);

            exporter.exportReport();
            final InputStream mediais = new ByteArrayInputStream(output.toByteArray());
            final AMedia amedia = new AMedia(reportName + ".xls", "xls", "text/xls", mediais);
            Filedownload.save(amedia);
        } finally {
            output.close();
        }
    }

    public void generateXMLReport() throws JRException, IOException, TransformerException,
            ParserConfigurationException {
        DocumentBuilderFactory docFactory = DocumentBuilderFactory.newInstance();
        DocumentBuilder docBuilder = docFactory.newDocumentBuilder();

        // root elements
        Document doc = docBuilder.newDocument();
        Element rootElement = doc.createElement("IDERA");
        doc.appendChild(rootElement);

        Element product = doc.createElement(ideraProductName);
        rootElement.appendChild(product);

        // set attribute to staff element
        Attr attr = doc.createAttribute("id");
        attr.setValue("1");
        product.setAttributeNode(attr);

        Element productData = doc.createElement("Heading");
        Element titlename = doc.createElement("Title");
        titlename.appendChild(doc.createTextNode(reportTitle));
        productData.appendChild(titlename);
        Element subTitlename = doc.createElement("SubTitle");
        subTitlename.appendChild(doc.createTextNode(reportSubTitle.replace("\\n", "")));
        productData.appendChild(subTitlename);
        product.appendChild(productData);

        for (Map<String, Object> tmp : getData()) {

            productData = doc.createElement("Data");
            for (Entry<String, Object> tmpMap : tmp.entrySet()) {
                String nodeName = tmpMap.getKey();
                if (nodeName.endsWith("Image"))
                    continue;

                String test = getColumns().get(tmpMap.getKey()) == null ? tmpMap.getKey()
                        : ELFunctions.getLabel( getColumns().get(tmpMap.getKey()).getColumnValue());
                test = test.replaceAll(ONLY_ALPHANUMERIC_REGEX, "");

                Element firstname = doc.createElement(test);
                firstname.appendChild(doc.createTextNode(tmpMap.getValue().toString()));
                productData.appendChild(firstname);
            }
            product.appendChild(productData);
        }

        // Add time stamp
        productData = doc.createElement("CreationTime");
        Element timeString = doc.createElement("TimeString");
        timeString.appendChild(doc.createTextNode(dateFormat.format(new Date())));
        productData.appendChild(timeString);
        product.appendChild(productData);

        TransformerFactory transformerFactory = TransformerFactory.newInstance();

        Transformer transformer = transformerFactory.newTransformer();
        transformer.setOutputProperty(OutputKeys.INDENT, "yes");
        ByteArrayOutputStream output = new ByteArrayOutputStream();
        StreamResult result = new StreamResult(output);
        transformer.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "2");
        try {
            DOMSource source = new DOMSource(doc);
            transformer.transform(source, result);

            final InputStream mediais = new ByteArrayInputStream(output.toByteArray());
            final AMedia amedia = new AMedia(reportName + ".xml", "xml", "text/xml", mediais);
            Filedownload.save(amedia);
        } finally {
            output.close();
        }
    }

    public void generateCSVReport() throws JRException, IOException {

        ByteArrayOutputStream output = new ByteArrayOutputStream();
        try {
            JasperPrint jasperPrint = getJasperPrint(translateFromStringToNativeType(getData()), true);
            JRCsvExporter exporter = new JRCsvExporter();
            exporter.setParameter(JRCsvExporterParameter.JASPER_PRINT, jasperPrint);
            exporter.setParameter(JRCsvExporterParameter.OUTPUT_STREAM, output);

            exporter.exportReport();

            final InputStream mediais = new ByteArrayInputStream(output.toByteArray());
            final AMedia amedia = new AMedia(reportName + ".csv", "csv", "text/csv", mediais);
            Filedownload.save(amedia);
        } finally {
            output.close();
        }
    }

    public void generateTXTReport() throws JRException, IOException {

        ByteArrayOutputStream output = new ByteArrayOutputStream();
        try {
            JasperPrint jasperPrint = getJasperPrint(getData(), false);
            JRTextExporter exporter = new JRTextExporter();
            exporter.setParameter(JRTextExporterParameter.JASPER_PRINT, jasperPrint);
            exporter.setParameter(JRTextExporterParameter.OUTPUT_STREAM, output);
            exporter.setParameter(JRTextExporterParameter.PAGE_HEIGHT, 56);
            exporter.setParameter(JRTextExporterParameter.PAGE_WIDTH, 250);
            exporter.exportReport();
            final InputStream mediais = new ByteArrayInputStream(output.toByteArray());
            final AMedia amedia = new AMedia(reportName + ".txt", "text", "text/plain", mediais);
            Filedownload.save(amedia);
        } finally {
            output.close();
        }
    }

    protected void addValidValueToMap(Object value, String propertyName, String columnName,
                                      Map<String, Object> tableDataMap, boolean addColumn,
                                      boolean fixedWidth) {
        addValidValueToMap(value, propertyName, columnName, tableDataMap, addColumn,
                HorizontalAlign.LEFT, VerticalAlign.MIDDLE, fixedWidth);
    }

    protected void addValidValueToMap(Object value, String propertyName, String columnName,
                                      Map<String, Object> tableDataMap, boolean addColumn) {
        addValidValueToMap(value, propertyName, columnName, tableDataMap, addColumn,
                HorizontalAlign.LEFT, VerticalAlign.MIDDLE, false);
    }

    protected void addValidValueToMap(Object value, String propertyName, String columnName,
                                      Map<String, Object> tableDataMap, boolean addColumn,
                                      HorizontalAlign halignment, VerticalAlign valignment,
                                      boolean fixedWidth) {
        if (value == null) {
            value = "";
        }
        tableDataMap.put(propertyName, (value instanceof String && ((String) value).isEmpty()) ? "" : value);

        if (addColumn) {
            ColumnProperties col = new ColumnProperties(columnName);
            col.setHorizontalAlign(halignment);
            col.setVerticalAlign(valignment);
            col.setFixedWidth(fixedWidth);
            getColumns().put(propertyName, col);
        }
    }

    // this is for translating all the strings to numbers so that they can be
    // formated correctly when publishing xls or csv
    protected List<LinkedHashMap<String, Object>> translateFromStringToNativeType(List<LinkedHashMap<String, Object>> dataList) {
        List<LinkedHashMap<String, Object>> returnList =
                new ArrayList<LinkedHashMap<String, Object>>();
        for (LinkedHashMap<String, Object> lnkMap : dataList) {
            LinkedHashMap<String, Object> tmpData = new LinkedHashMap<String, Object>();
            for (Entry<String, Object> dataField : lnkMap.entrySet()) {
                Object translatedData = dataField.getValue();
                if (dataField.getValue() != null && dataField.getValue() instanceof String) {
                    String tmpString = (String) dataField.getValue();
                    tmpString = tmpString.replace(",", "");
                    if (tmpString.matches(IS_NUMBER_REGEX)) {
                        try {
                            translatedData = Double.parseDouble(tmpString);
                        } catch (NumberFormatException ex) {
                            // don't do anything
                        }
                    } else {
                        translatedData = "\n" + tmpString + "\n";
                    }
                } else if (dataField.getValue() != null && dataField.getValue() instanceof Date) {
                    translatedData = dateFormat.format((Date) dataField.getValue());
                }
                tmpData.put(dataField.getKey(), translatedData);
            }
            returnList.add(tmpData);
        }
        return returnList;
    }

    public class ColumnProperties {
        private String columnValue = "";
        private HorizontalAlign horizontalAlign = HorizontalAlign.LEFT;
        private VerticalAlign verticalAlign = VerticalAlign.TOP;
        private boolean fixedWidth;

        public ColumnProperties(String columnValue) {
            this.columnValue = columnValue;
        }

        public String getColumnValue() {
            return columnValue;
        }

        public void setColumnValue(String columnValue) {
            this.columnValue = columnValue;
        }

        public HorizontalAlign getHorizontalAlign() {
            return horizontalAlign;
        }

        public void setHorizontalAlign(HorizontalAlign horizontalAlign) {
            this.horizontalAlign = horizontalAlign;
        }

        public VerticalAlign getVerticalAlign() {
            return verticalAlign;
        }

        public void setVerticalAlign(VerticalAlign verticalAlign) {
            this.verticalAlign = verticalAlign;
        }

        public boolean getFixedWidth() {
            return fixedWidth;
        }

        public void setFixedWidth(boolean fixedWidth) {
            this.fixedWidth = fixedWidth;
        }
  }
}

