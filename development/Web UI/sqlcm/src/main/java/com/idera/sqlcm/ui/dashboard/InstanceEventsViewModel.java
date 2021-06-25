package com.idera.sqlcm.ui.dashboard;

import com.idera.common.Utility;
import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.StatisticData;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.server.web.component.SQLCMLineChart;
import com.idera.sqlcm.enumerations.Category;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.ui.charts.LineChartEngine;
import com.idera.sqlcm.ui.converter.ListEmptyBooleanConverter;
import com.idera.sqlcm.ui.databaseEvents.EventsColumns;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Chart;
import org.zkoss.zul.ChartModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Radiogroup;

import java.util.Arrays;
import java.util.List;
import java.util.Set;

public class InstanceEventsViewModel {

    private static final Logger logger = Logger.getLogger(InstanceEventsViewModel.class);

    private static final String DEFAULT_CHART_WIDTH = "300px";

    private Category currentCategory = Category.OVERALL_ACTIVITY;

    private Interval currentInterval = Interval.ONE_DAY;

    private StatisticData statics;

    private Converter listEmptyBooleanConverter = new ListEmptyBooleanConverter();

    private String errorMsg;

    @Wire
    private SQLCMLineChart alertsActivity;

    @Wire
    private Component contentLayout;

    @Wire
    private Component chartContainer;

    @Wire
    private Chart activityChart;

    @Wire
    private Component instEventsView;

    private ChartModel activityChartModel;

    private ListModelList<Interval> intervalListModelList;

    private ListModelList<Category> categoryListModelList;

    public Converter getListEmptyBooleanConverter() {
        return listEmptyBooleanConverter;
    }

    public ChartModel getActivityChartModel() {
        return activityChartModel;
    }

    @Init
    public void init() {
        errorMsg = null;
        engine = new LineChartEngine();
        initIntervalList();
        initCategoryList();
        loadStatics(currentInterval, currentCategory);
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        initChart();
    }

    private void initIntervalList() {
        intervalListModelList = new ListModelList<>();
        intervalListModelList.add(Interval.ONE_DAY);
        intervalListModelList.add(Interval.SEVEN_DAY);
        intervalListModelList.add(Interval.THIRTY_DAY);
        intervalListModelList.setSelection(Arrays.asList(currentInterval));
    }

    private void initCategoryList() {
        categoryListModelList = new ListModelList<>();
        categoryListModelList.addAll(Arrays.asList(currentCategory.values()));
        categoryListModelList.setSelection(Arrays.asList(currentCategory));
    }

    public ListModelList<Interval> getIntervalListModelList() {
        return intervalListModelList;
    }

    public ListModelList<Category> getCategoryListModelList() {
        return categoryListModelList;
    }

    private void loadStatics(Interval interval, Category category) {
        try {
            List<StatisticData.Statistic> stat = InstancesFacade.getEnvironmentStatsData(interval, category);
            for (StatisticData.Statistic st : stat) {
                st.setCategoryName(category.getLabel());
            }
            statics = new StatisticData();
            statics.setStatics(stat);
            activityChartModel = statics.getDataXYModel();
            engine.setInterval(interval);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
        }
    }

    private void setEmptyChart() {
        alertsActivity.setTitle("");
        alertsActivity.setErrorMessage(Utility.getMessage(SQLCMI18NStrings.NO_DATA_AVAILABLE));
    }

    @NotifyChange({"activityChartModel"})
    @Command("refreshEventList")
    public void refreshEventList() throws RestException {
        loadStatics(currentInterval, currentCategory);
    }

    @NotifyChange({"activityChartModel"})
    @Command("selectIntervalDays")
    public void selectIntervalDays(@BindingParam("radioGroup") Radiogroup radioGroup) throws RestException {
        Set<Interval> selectedIntervals = intervalListModelList.getSelection(); // must contain only 1 item because single selection mode.
        if (selectedIntervals != null && !selectedIntervals.isEmpty()) {
            for (Interval i : selectedIntervals) {
                currentInterval = i;
                break;
            }
        }
        loadStatics(currentInterval, currentCategory);
        changeInterval();
    }

    public void changeInterval() {
        EventQueue<Event> eq = EventQueues.lookup(DashboardViewModel.DASHBOARD_CHANGE_INTERVAL_EVENT, EventQueues.SESSION, false);
        if (eq != null) {
            eq.publish(new Event(DashboardViewModel.DASHBOARD_CHANGE_INTERVAL_EVENT, null, currentInterval.getDays()));
        }
    }

    @NotifyChange({"activityChartModel"})
    @Command("selectCategory")
    public void selectCategory() throws RestException {
        Set<Category> selectedCategories = categoryListModelList.getSelection(); // must contain only 1 item because single selection mode.
        if (selectedCategories != null && !selectedCategories.isEmpty()) {
            for (Category c : selectedCategories) {
                currentCategory = c;
                break;
            }
        }
        loadStatics(currentInterval, currentCategory);
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
        /**
         * Code below used to receive chart container width & generate JFreeChart component.
         * This hack is used because JFreeChart widget does not support width in percentage.
         */
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
                BindUtils.postNotifyChange(null, null, InstanceEventsViewModel.this, "activityChartModel");
            }
        });
    }
}
