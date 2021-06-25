package com.idera.sqlcm.ui.databases;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import org.jfree.chart.JFreeChart;
import org.jfree.chart.axis.DateAxis;
import org.jfree.chart.axis.DateTickUnit;
import org.jfree.chart.axis.DateTickUnitType;
import org.jfree.chart.axis.NumberAxis;
import org.jfree.chart.plot.XYPlot;
import org.jfree.chart.renderer.xy.XYLineAndShapeRenderer;
import org.jfree.data.RangeType;
import org.zkoss.zkex.zul.impl.JFreeChartEngine;
import org.zkoss.zul.Chart;

import java.awt.*;
import java.text.SimpleDateFormat;

public class LineChartEngine extends JFreeChartEngine {

    private static final Color LINE_COLOR = new Color(0x2165C6);

    private static final int LINE_WIDTH = 2;

    private static final DateTickUnit ONE_DAY_TICK =
        new DateTickUnit(DateTickUnitType.HOUR, 6,
            new SimpleDateFormat(ELFunctions.getLabel(SQLCMI18NStrings.ACTIVITY_CHART_ONE_DAY_TICK_DATE_FORMAT)));

    private static final DateTickUnit SEVEN_DAYS_TICK =
        new DateTickUnit(DateTickUnitType.HOUR, 42,
            new SimpleDateFormat(ELFunctions.getLabel(SQLCMI18NStrings.ACTIVITY_CHART_SEVEN_DAYS_TICK_DATE_FORMAT)));

    private static final DateTickUnit THIRTY_DAYS_TICK =
        new DateTickUnit(DateTickUnitType.DAY, 5,
            new SimpleDateFormat(ELFunctions.getLabel(SQLCMI18NStrings.ACTIVITY_CHART_THIRTY_DAYS_TICK_DATE_FORMAT)));

    private Interval interval;

    public Interval getInterval() {
        return interval;
    }

    public void setInterval(Interval interval) {
        this.interval = interval;
    }

    @Override
    public boolean prepareJFreeChart(JFreeChart jfchart, Chart chart) {
        XYPlot xyPlot = (XYPlot) jfchart.getPlot();
        XYLineAndShapeRenderer renderer = (XYLineAndShapeRenderer) xyPlot.getRenderer();
        renderer.setBaseShapesFilled(true);
        renderer.setSeriesStroke(0, new BasicStroke(LINE_WIDTH));
        renderer.setSeriesShapesVisible(0, true);
        renderer.setSeriesPaint(0, LINE_COLOR);
        xyPlot.setDomainGridlinesVisible(true);
        xyPlot.setRangeGridlinesVisible(true);
        DateAxis dateAxis = (DateAxis) xyPlot.getDomainAxis();

        switch (interval) {
            case ONE_DAY:
                dateAxis.setTickUnit(ONE_DAY_TICK);
                break;
            case SEVEN_DAY:
                dateAxis.setTickUnit(SEVEN_DAYS_TICK);
                break;
            case THIRTY_DAY:
                dateAxis.setTickUnit(THIRTY_DAYS_TICK);
                break;
            default:
                throw new RuntimeException(" Invalid interval value " + interval);
        }
        dateAxis.setAutoTickUnitSelection(false);

        NumberAxis valueAxis = new NumberAxis();
        valueAxis.setAutoRange(true);
        valueAxis.setStandardTickUnits(NumberAxis.createIntegerTickUnits());
        valueAxis.setRangeType(RangeType.POSITIVE);
        valueAxis.setAutoRangeMinimumSize(1);
        xyPlot.setRangeAxis(valueAxis);

        return false;
    }
}
