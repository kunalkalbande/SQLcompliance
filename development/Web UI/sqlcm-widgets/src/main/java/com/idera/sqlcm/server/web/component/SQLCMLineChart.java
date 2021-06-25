package com.idera.sqlcm.server.web.component;

import org.apache.log4j.Logger;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebConstants;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqldm.d3zk.chart.LineChart;

public class SQLCMLineChart extends IderaSqlcmChart {

	private static final long serialVersionUID = 1L;
	private static final Logger log = Logger.getLogger(SQLCMLineChart.class);

	public SQLCMLineChart() {
		super("~./sqlcm/components/lineChart.zul");
	}

	@Override
	@SuppressWarnings("unchecked")
	public LineChart<Number> getChart() {
		return (LineChart<Number>) super.getChart();
	}

	@Override
	public void updateChart() {
		try {
			if (getModel() != null) {
				getChart().setModel(getModel());
				getChart().setDrawHorizontalGridLines(true);
				getChart().setDrawVerticalGridLines(true);
				getChart().setShowSeriesLabels(true);
				getChart().setTruncateSeriesLabels(false);
				getErrorLabel().setVisible(false);
				getChart().setVisible(true);

				//this.setXAxisScaleType("time");
				//getChart().setXAxisTickFormat("%I:%M %p");
				getChart().setShowSeriesLabels(true);
				getChart().setXAxisTickCount(WebConstants.CHART_X_AXIS_TICKS_COUNT);
				getChart().setXAxisLegendSpacing(new Integer(25));
			} else {
				showError(noDataMessage);
			}
		} catch (Exception x) {
			log.error(x.getMessage(), x);
			showError(ELFunctions.getMessage(I18NStrings.ERROR_OCCURRED_LOADING_CHART));
		}
	}
	
	public void setXAxisScaleType(String xAxisScaleType) {
		getChart().setXAxisScaleType(xAxisScaleType);
	}

	public void setXAxisScaleType(Interval interval) {
		switch(interval) {
		case SEVEN_DAY:
			getChart().setXAxisScaleType("time");
			getChart().setXAxisTickFormat("%m/%d/%y");
			break;
		case THIRTY_DAY:
			getChart().setXAxisScaleType("time");
			getChart().setXAxisTickFormat("%m/%d/%y");
			break;
		case ONE_DAY:
		case ALL:
		default:
			getChart().setXAxisScaleType("time");
			getChart().setXAxisTickFormat("%I:%M %p");
			break;
		}
	}

}
