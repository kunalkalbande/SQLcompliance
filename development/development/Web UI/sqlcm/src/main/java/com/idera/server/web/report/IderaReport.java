package com.idera.server.web.report;

import net.sf.jasperreports.engine.JRException;
import net.sf.jasperreports.engine.JasperPrint;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;
import java.io.IOException;
import java.util.LinkedHashMap;
import java.util.List;

public interface IderaReport {

    JasperPrint getJasperPrint(List<LinkedHashMap<String, Object>> data, boolean ignorePaging) throws JRException;

    void generatePDFReport() throws JRException, IOException;

    void generateXLSReport() throws JRException, IOException;

    void generateXMLReport() throws JRException, IOException, TransformerException, ParserConfigurationException;

    void generateCSVReport() throws JRException, IOException;

}
