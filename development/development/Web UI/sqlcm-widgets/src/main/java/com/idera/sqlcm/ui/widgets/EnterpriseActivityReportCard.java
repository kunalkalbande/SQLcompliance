package com.idera.sqlcm.ui.widgets;

import java.util.ArrayList;
import java.util.List;

import org.zkoss.bind.BindUtils;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Chart;
import org.zkoss.zul.ChartModel;
import org.zkoss.zul.ListModelList;



import org.zkoss.zul.Panel;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.common.dashboard.Composers.DashboardBaseWidgetComposer;
import com.idera.dashboard.ui.widget.DashboardWidget;
import com.idera.sqlcm.entities.StatisticData;
import com.idera.sqlcm.entities.StatisticDataResponse;
import com.idera.sqlcm.entities.StatisticData.Statistic;
import com.idera.sqlcm.enumerations.Category;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.server.web.component.SQLCMLineChart;
import com.idera.sqlcm.ui.charts.LineChartEngine;

public class EnterpriseActivityReportCard extends DashboardBaseWidgetComposer {
	
	@Wire
	Chart activityChart;
	@Wire
    private Component chartContainer;
	
	@Wire
	Panel panel;
	
	
	private static final long serialVersionUID = 1L;

	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory
			.getLogger(EnterpriseActivityReportCard.class);

	public static final String ZUL_URL = "widgets/instance-detail-widget.zul";
	
	public static final String AUDITED_INSTANCE = "auditedInstance";
	
	private static final String DEFAULT_CHART_WIDTH = "300px";
	
	private Category currentCategory = Category.EVENT_ALERT;

	private Interval currentInterval = Interval.THIRTY_DAY;

	protected AnnotateDataBinder binder;

	private StatisticData statics;
			 
	private ChartModel activityChartModel;
	
	@Wire
    private SQLCMLineChart alertsActivity;
	
	private ListModelList<Interval> intervalListModelList;

    private ListModelList<Category> categoryListModelList;
    
    public ListModelList<Interval> getIntervalListModelList() {
        return intervalListModelList;
    }
    
    public ListModelList<Category> getCategoryListModelList() {
        return categoryListModelList;
    }
   	public ChartModel getActivityChartModel() {
		return activityChartModel;
	}
  
	@Override
	public void doAfterCompose(DashboardWidget widget) throws Exception {
		super.doAfterCompose(widget);
		engine = new LineChartEngine();
	}

	@Override
	public TypeReference<List<StatisticDataResponse>> getModelType() {
		return new TypeReference<List<StatisticDataResponse>>() {
		};
	}
       
    boolean threeD = false;

    public boolean isThreeD() {
        return threeD;
    }

    LineChartEngine engine;

    public LineChartEngine getEngine() {
        return engine;
    }

    public void initChart() {
        //**
        // * Code below used to receive chart container width & generate JFreeChart component.
        // * This hack is used because JFreeChart widget does not support width in percentage.
         //*//
        chartContainer.addEventListener("onCreateChartContainer", new EventListener() {
            @Override
            public void onEvent(Event event) throws Exception {
                Object eventData = event.getData();
                String chartWidth;
                if (eventData instanceof Number) {
                    int chartWidthValue = ((Number)eventData).intValue();
                    if (chartWidthValue > 0) {
                        chartWidth = chartWidthValue + "px";
                    } else {
                        chartWidth = DEFAULT_CHART_WIDTH;
                        logger.error(" Invalid chart width value number -> " + eventData);
                    }
                } else {
                    chartWidth = DEFAULT_CHART_WIDTH;
                    logger.error(" Invalid chart width value -> " + eventData);
                }
                activityChart.setWidth(chartWidth);
                activityChartModel = statics.getDataXYModel();
                BindUtils.postNotifyChange(null, null, EnterpriseActivityReportCard.this, "activityChartModel");
            }
        });
    }

	@Override
	public void setWidgetData(Object obj) {
		if(obj != null){
			List<StatisticDataResponse> statisticDataResponse = (List<StatisticDataResponse>) obj;
			List<Statistic> statsData = new ArrayList<>();
			
			for(StatisticDataResponse stat: statisticDataResponse){
				for(Statistic st: stat.getValue()){
					if (st.getCategory() == Category.EVENT_ALERT.getIndex())
						st.setCategoryName(Category.EVENT_ALERT.getLabel());
					if (st.getCategory() == Category.DDL.getIndex())
						st.setCategoryName(Category.DDL.getLabel());
					if (st.getCategory() == Category.FAILED_LOGIN.getIndex())
						st.setCategoryName(Category.FAILED_LOGIN.getLabel());
					if (st.getCategory() == Category.PRIVILEGED_USER.getIndex())
						st.setCategoryName(Category.PRIVILEGED_USER.getLabel());
					if (st.getCategory() == Category.SECURITY.getIndex())
						st.setCategoryName(Category.SECURITY.getLabel());
					statsData.add(st);
				}
			}
				
            statics = new StatisticData();
            statics.setStatics(statsData);
			activityChartModel = statics.getDataXYModel();
			activityChart.setModel(activityChartModel);
			engine.setInterval(currentInterval);		
			initChart();
			BindUtils.postNotifyChange(null, null,
					EnterpriseActivityReportCard.this, "activityChartModel");
			
		}
		else
		{
			activityChart.setTitle("No Data Found");
		}
	}

	@Override
	public String getEventName() {
		config.setName("Enterprise Activity Report Card");
		
		return String.format("%d:%s", config.getId(),
				EnterpriseActivityReportCard.class.getName());
	}

	@Override
	public String getDataURI(){
		String url = String.format("%s%s", config.getProduct().getRestUrl(),
				config.getDataURI());
		if (!url.contains("?days"))
		{
			url = url + "?days=7&category=100";
		}
		return url;
	}
	
	@Listen("onMaximize = #panel")
	public void setWidth(){ 
	}
}
