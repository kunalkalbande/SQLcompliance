package com.idera.sqlcm.ui.databases;

import org.jfree.chart.JFreeChart;
import org.jfree.chart.labels.StandardPieSectionLabelGenerator;
import org.jfree.chart.labels.StandardPieToolTipGenerator;
import org.jfree.chart.plot.DefaultDrawingSupplier;
import org.jfree.chart.plot.PiePlot;
import org.zkoss.zkex.zul.impl.JFreeChartEngine;
import org.zkoss.zul.Chart;

import java.awt.*;

public class PieChartEngine extends JFreeChartEngine {

    @Override
    public boolean prepareJFreeChart(JFreeChart jfchart, Chart chart) {
        jfchart.setBackgroundPaint(Color.green);

        PiePlot piePlot = (PiePlot) jfchart.getPlot();
        piePlot.setLabelBackgroundPaint(new Color(0xFFFFFF));
        piePlot.setLegendLabelGenerator(new StandardPieSectionLabelGenerator());
        piePlot.setToolTipGenerator(new StandardPieToolTipGenerator());
        piePlot.setLabelGenerator(null);

        Paint[] colors = new Paint[]{new Color(0x2865c1), new Color(0xc73902), new Color(0x47b1c2), new Color(0x77c82e), new Color(0xecb347)};
        DefaultDrawingSupplier defaults = new DefaultDrawingSupplier();
        piePlot.setDrawingSupplier(new DefaultDrawingSupplier(colors, new Paint[]{defaults.getNextFillPaint()}, new Paint[]{defaults.getNextOutlinePaint()},
            new Stroke[]{defaults.getNextStroke()}, new Stroke[]{defaults.getNextOutlineStroke()}, new Shape[]{defaults.getNextShape()}));
        piePlot.setShadowPaint(null);
        piePlot.setNoDataMessage("No data available");
        return false;
    }
}
