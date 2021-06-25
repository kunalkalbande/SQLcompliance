package com.idera.sqlcm.ui.dashboard;

import com.google.common.base.Function;
import com.google.common.base.Strings;
import com.google.common.collect.Lists;
import com.google.common.collect.Ordering;
import com.google.common.primitives.Ints;
import com.idera.common.converter.NotEmptyCollectionBooleanConverter;
import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMAlert;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.server.web.WebConstants;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.alerts.*;
import com.idera.sqlcm.ui.converter.alerts.*;
import com.idera.sqlcm.ui.converter.basic.MoreThanOneItemInCollectionBooleanConverter;
import com.idera.sqlcm.ui.converter.basic.OneItemInCollectionBooleanConverter;
import com.idera.sqlcm.ui.converter.basic.StringEmptyBooleanConverter;
import com.idera.sqlcm.ui.converter.basic.StringNotEmptyBooleanConverter;
import com.idera.sqlcm.ui.dialogs.EventPropertiesViewModel;
import com.idera.sqlcm.ui.preferences.AlertsPreferencesBean;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.lang.builder.CompareToBuilder;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.*;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Grid;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Window;

import java.util.*;

public class AlertsListViewModel {

    private static final Comparator<AlertMetric> ALERT_METRICS_SEVERITY_COMPARATOR_OLD = new Comparator<AlertMetric>() {
        @Override
        public int compare(AlertMetric o1, AlertMetric o2) {
            if (o1 == null && o2 != null) {
                return 1;
            }
            if (o1 != null && o2 == null) {
                return -1;
            }
            if (o1 == null && o2 == null) {
                return 0;
            }
            return new CompareToBuilder()
                .append(o2.getLevel().getLevel(), o1.getLevel().getLevel())
                .append(o2.getAlerts().size(), o1.getAlerts().size()).toComparison();
        }
    };

    private static final Comparator<AlertMetric> ALERT_METRICS_SEVERITY_COMPARATOR = new Comparator<AlertMetric>() {
        @Override
        public int compare(AlertMetric o1, AlertMetric o2) {
            if (o1 == null && o2 != null) {
                return 1;
            }
            if (o1 != null && o2 == null) {
                return -1;
            }
            if (o1 == null && o2 == null) {
                return 0;
            }
            return new CompareToBuilder()
                .append(o2.getLevel().getLevel(), o1.getLevel().getLevel()).toComparison();
        }
    };

    private static final Function<CMAlert, AlertGroup> ALERT_GROUPING_FUNCTION = new Function<CMAlert, AlertGroup>() {
        @Override
        public AlertGroup apply(CMAlert alert) {
            return new AlertGroup(alert);
        }
    };

    private static Converters converters;
    private boolean alertsExist;
    private boolean alertsMoreThanLimit;
    private boolean alertsHidden;
    private boolean alertsShowAll;
    private String formattedTotalAlertsMessage;
    private int alertsCount;
    private AlertLevel highestLevel;
    private Integer initialAlertsCount = 5;
    private ListModelList<AlertMetric> allAlertMetrics = new ListModelList<>();
    private List<AlertMetric> alertMetrics = new ListModelList<>();
    private boolean instanceDetailsPage;
    private CMInstance instance;
    private Long instanceId;
    private Map<String, Object> alertsRequestParameter = new HashMap<>();

    @Init
    public void init(@ExecutionArgParam("viewName") String viewName) {
        if (Strings.isNullOrEmpty(viewName)) {
            instanceDetailsPage = false;
        } else {
            instanceDetailsPage = viewName.equalsIgnoreCase(WebConstants.INSTANCE_DETAILS_VIEW_NAME);
        }
        if (instanceDetailsPage) {
            try {
                long currentInstanceId = Utils.parseInstanceIdArg();
                List<CMInstance> instanceList = InstancesFacade.getInstanceList();
                for (CMInstance instance : instanceList) {
                    if (instance.getId() == currentInstanceId) {
                        this.instance = instance;
                    }
                }
            } catch (RestException e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_INSTANCE_LIST);
            }
        }
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        Selectors.wireEventListeners(view, this);
        if (instanceDetailsPage) {
            subscribeToChangeInstanceEvent();
        }
        loadAlerts();
        displayAlerts();
        wireFooters(view);
    }

    private void wireFooters(Component view) {
        Selectors.wireEventListeners(view, this);
    }

    private void loadAlerts() {
        try {
            allAlertMetrics.clear();
            instanceId = Long.MIN_VALUE;
            if (instanceDetailsPage) {
                if (instance == null) {
                    return;
                }
                instanceId = instance.getId();
            }
            alertsCount = 0;
            List<AlertGroupDTO> alertGroupDTOList = SQLCMRestClient.getInstance().getAlertsGroups(instanceId);
            for (AlertGroupDTO alertGroup : alertGroupDTOList) {
                alertsCount += alertGroup.getAlertsCount();
                List<CMAlert> alertsByGroup = SQLCMRestClient.getInstance().getAlerts(instanceId,
                    alertGroup.getAlertType(), alertGroup.getAlertLevel(), AlertPager.DEFAULT_PAGE_SIZE, AlertPager.DEFAULT_PAGE);
                if (!CollectionUtils.isEmpty(alertsByGroup)) {
                    allAlertMetrics.add(new AlertMetric(new AlertGroup(alertGroup.getAlertType(), alertGroup.getAlertLevel(),
                        alertGroup.getAlertsCount()), alertsByGroup, instanceId));
                }
            }
            Collections.sort(allAlertMetrics, ALERT_METRICS_SEVERITY_COMPARATOR_OLD);
        } catch (RestException e) {
            WebUtil.showErrorBox(e.getMsgKey());
        }
    }

    private List<AlertMetric> convertToAlertMetricListModelList(List<AlertGroupDTO> alertGroupDTOList) {
        if (alertGroupDTOList == null) {
            return null;
        }
        List<AlertMetric> alertMetricList = new ListModelList<>(alertGroupDTOList.size());
        for (AlertGroupDTO ag : alertGroupDTOList) {
            alertMetricList.add(new AlertMetric(new AlertGroup(ag.getAlertType(), ag.getAlertLevel(), ag.getAlertsCount())));
        }
        return alertMetricList;
    }

    private void displayAlerts() {
        alertMetrics.clear();
        alertMetrics.addAll(getAlertsPreferencesInSession().isAlertsShowAll() ? allAlertMetrics
            : allAlertMetrics.size() <= initialAlertsCount ? allAlertMetrics : allAlertMetrics.subList(0, initialAlertsCount));
        checkHighestLevel();
        BindUtils.postNotifyChange(null, null, this, "highestLevel");
        setAlertsExist(!alertMetrics.isEmpty());
        BindUtils.postNotifyChange(null, null, this, "alertsExist");
        setAlertsMoreThanLimit(alertMetrics.size() > initialAlertsCount);
        BindUtils.postNotifyChange(null, null, this, "alertsMoreThanLimit");
        setFormattedTotalAlertsMessage(alertsExist
            ? ELFunctions.getMessageWithParams(SQLCMI18NStrings.ALERTS_COUNT, alertsCount)
            : ELFunctions.getLabel(SQLCMI18NStrings.NO_ALERTS));
        BindUtils.postNotifyChange(null, null, this, "formattedTotalAlertsMessage");
        BindUtils.postNotifyChange(null, null, this, "alertMetrics");
        setAlertsPreferencesFromSession();
    }

    private AlertsPreferencesBean getAlertsPreferencesInSession() {
        return PreferencesUtil.getInstance().getAlertsPreferencesInSession(AlertsPreferencesBean.SESSION_VARIABLE_NAME);
    }

    private void setAlertsPreferencesFromSession() {
        setAlertsHidden(getAlertsPreferencesInSession().isAlertsHidden());
        BindUtils.postNotifyChange(null, null, this, "alertsHidden");
        setAlertsShowAll(getAlertsPreferencesInSession().isAlertsShowAll());
        BindUtils.postNotifyChange(null, null, this, "alertsShowAll");
    }

    private void checkHighestLevel() {
        Ordering<AlertMetric> o = new Ordering<AlertMetric>() {
            @Override
            public int compare(AlertMetric left, AlertMetric right) {
                return Ints.compare(left.getLevel().getLevel(), right.getLevel().getLevel());
            }
        };
        setHighestLevel(allAlertMetrics.isEmpty() ? AlertLevel.LOW : o.max(allAlertMetrics).getLevel());
    }

    private int calculateCountOfAlerts(List<AlertMetric> alertMetrics) {
        int sum = 0;
        if (!alertMetrics.isEmpty()) {
            for (AlertMetric alertMetric : alertMetrics) {
                sum += alertMetric.getAlerts().size();
            }
        }
        return sum;
    }

    protected void subscribeToChangeInstanceEvent() {
        EventQueue<Event> eq = EventQueues.lookup(com.idera.sqlcm.ui.instancedetails.InstanceEventsViewModel.CHANGE_INSTANCE_EVENT,
            EventQueues.SESSION, true);
        eq.subscribe(new org.zkoss.zk.ui.event.EventListener<Event>() {
            public void onEvent(Event event) throws Exception {
                Object instanceObj = event.getData();
                if (instanceObj instanceof CMInstance) {
                    instance = (CMInstance) instanceObj;
                    loadAlerts();
                    displayAlerts();
                }
            }
        });
    }

    public Converters getConverters() {
        if (converters == null) {
            converters = new Converters();
        }
        return converters;
    }

    public boolean isAlertsExist() {
        return alertsExist;
    }

    public void setAlertsExist(boolean alertsExist) {
        this.alertsExist = alertsExist;
    }

    public boolean isAlertsMoreThanLimit() {
        return alertsMoreThanLimit;
    }

    public void setAlertsMoreThanLimit(boolean alertsMoreThanLimit) {
        this.alertsMoreThanLimit = alertsMoreThanLimit;
    }

    public boolean isAlertsHidden() {
        return alertsHidden;
    }

    public void setAlertsHidden(boolean alertsHidden) {
        this.alertsHidden = alertsHidden;
    }

    public boolean isAlertsShowAll() {
        return alertsShowAll;
    }

    public void setAlertsShowAll(boolean alertsShowAll) {
        this.alertsShowAll = alertsShowAll;
    }

    public String getFormattedTotalAlertsMessage() {
        return formattedTotalAlertsMessage;
    }

    public void setFormattedTotalAlertsMessage(String formattedTotalAlertsMessage) {
        this.formattedTotalAlertsMessage = formattedTotalAlertsMessage;
    }

    public AlertLevel getHighestLevel() {
        return highestLevel;
    }

    public void setHighestLevel(AlertLevel highestLevel) {
        this.highestLevel = highestLevel;
    }

    public List<AlertMetric> getAlertMetrics() {
        return alertMetrics;
    }

    public void setAlertMetrics(List<AlertMetric> alertMetrics) {
        this.alertMetrics = alertMetrics;
    }

    @Listen("onClick = #showHideAllLink")
    public void showHideAll() {
        setAlertsHidden(!isAlertsHidden());
        PreferencesUtil.getInstance().setAlertsHiddenInSession(AlertsPreferencesBean.SESSION_VARIABLE_NAME, alertsHidden);
        BindUtils.postNotifyChange(null, null, this, "alertsHidden");
    }

    @Listen("onClick = #showAllLink")
    public void showAll() {
        setAlertsShowAll(!isAlertsShowAll());
        PreferencesUtil.getInstance().setAlertsShowAllInSession(AlertsPreferencesBean.SESSION_VARIABLE_NAME, alertsShowAll);
        displayAlerts();
        BindUtils.postNotifyChange(null, null, this, "alertsShowAll");
    }

    @Listen("onClick = #exportReportsLink")
    public void onClickExportReportsLink(Event evt) {
        if (WebUtil.getComponentById(Executions.getCurrent().getDesktop(), "exportRecommendationsDialog") != null) {
            return;
        }
        final Map<String, Object> valueMap = new HashMap<>();
        valueMap.put(AlertsExportDialog.ALERTS_LIST, alertMetrics);
        Window window = (Window) Executions.createComponents(AlertsExportDialog.ZUL_URL, null, valueMap);
        window.doHighlighted();
    }

    @Command
    @NotifyChange("alertMetrics")
    public void showHideAlertDetails(@BindingParam("alertMetric") AlertMetric alertMetric) {
        if (alertMetric == null) {
            return;
        }
        alertMetric.setDetailsVisible(!alertMetric.isDetailsVisible());
    }

    @NotifyChange("alertMetrics")
    @Command("dismissAlerts")
    public void dismissAlerts(@BindingParam("alertMetric") AlertMetric alertMetric) {
        if (!WebUtil.showMessageBoxWithUserPreference(SQLCMI18NStrings.DISMISS_ALERT_WARNING, SQLCMI18NStrings.DISMISS_ALERT)) {
            return;
        }
        try {
            Map<String, Object> alertsRequest = new HashMap<>();
            alertsRequest.put("instanceId", instanceId == Long.MIN_VALUE ? -1 : instanceId);
            alertsRequest.put("alertType", alertMetric.getAlertGroup().getAlertType().getId());
            alertsRequest.put("alertLevel", alertMetric.getAlertGroup().getAlertLevel().getId());
            SQLCMRestClient.getInstance().dismissAlertsByGroupAndLevel(alertsRequest);
            loadAlerts();
            displayAlerts();
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
        }
    }

    @NotifyChange("alertMetrics")
    @Command("dismissAlert")
    public void dismissAlert(@BindingParam("alert") CMAlert alert) {
        if (!WebUtil.showMessageBoxWithUserPreference(SQLCMI18NStrings.DISMISS_ALERT_WARNING, SQLCMI18NStrings.DISMISS_ALERT)) {
            return;
        }
        try {
            SQLCMRestClient.getInstance().dismissAlert(alert.getId());
            loadAlerts();
            displayAlerts();
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
        }
    }

    @Command("showProperties")
    public void showProperties(final @BindingParam("alerts") ListModelList<CMAlert> alerts,
                               final @BindingParam("instanceId") long instanceId,
                               final @BindingParam("rowIndex") int rowIndex) {
        if (alerts == null) {
            return;
        }
        EventPropertiesViewModel.showEventPropertiesWindow(rowIndex, alerts, instanceId);
    }

    public static class Converters {

        private StringEmptyBooleanConverter stringEmptyBooleanConverter = new StringEmptyBooleanConverter();

        private StringNotEmptyBooleanConverter stringNotEmptyBooleanConverter = new StringNotEmptyBooleanConverter();

        private NotEmptyCollectionBooleanConverter notEmptyCollectionBooleanConverter = new NotEmptyCollectionBooleanConverter();

        private MoreThanOneItemInCollectionBooleanConverter moreThanOneItemInCollectionBooleanConverter = new
            MoreThanOneItemInCollectionBooleanConverter();

        private OneItemInCollectionBooleanConverter oneItemInCollectionBooleanConverter = new OneItemInCollectionBooleanConverter();

        private AlertsWidgetShowHideLabelConverter alertsWidgetShowHideLabelConverter = new AlertsWidgetShowHideLabelConverter();

        private AlertsWidgetHeaderImageConverter alertsWidgetHeaderImageConverter = new AlertsWidgetHeaderImageConverter();

        private AlertsWidgetSeverityImageConverter alertsWidgetSeverityImageConverter = new AlertsWidgetSeverityImageConverter();

        private AlertsWidgetShowHideAllAlertsLabelConverter alertsWidgetShowHideAllAlertsLabelConverter = new
            AlertsWidgetShowHideAllAlertsLabelConverter();

        private AlertMetricShowHideDetailsLabelConverter alertMetricShowHideDetailsLabelConverter = new AlertMetricShowHideDetailsLabelConverter();

        public StringEmptyBooleanConverter getStringEmptyBooleanConverter() {
            return stringEmptyBooleanConverter;
        }

        public StringNotEmptyBooleanConverter getStringNotEmptyBooleanConverter() {
            return stringNotEmptyBooleanConverter;
        }

        public NotEmptyCollectionBooleanConverter getNotEmptyCollectionBooleanConverter() {
            return notEmptyCollectionBooleanConverter;
        }

        public MoreThanOneItemInCollectionBooleanConverter getMoreThanOneItemInCollectionBooleanConverter() {
            return moreThanOneItemInCollectionBooleanConverter;
        }

        public OneItemInCollectionBooleanConverter getOneItemInCollectionBooleanConverter() {
            return oneItemInCollectionBooleanConverter;
        }

        public AlertsWidgetShowHideLabelConverter getAlertsWidgetShowHideLabelConverter() {
            return alertsWidgetShowHideLabelConverter;
        }

        public AlertsWidgetHeaderImageConverter getAlertsWidgetHeaderImageConverter() {
            return alertsWidgetHeaderImageConverter;
        }

        public AlertsWidgetSeverityImageConverter getAlertsWidgetSeverityImageConverter() {
            return alertsWidgetSeverityImageConverter;
        }

        public AlertsWidgetShowHideAllAlertsLabelConverter getAlertsWidgetShowHideAllAlertsLabelConverter() {
            return alertsWidgetShowHideAllAlertsLabelConverter;
        }

        public AlertMetricShowHideDetailsLabelConverter getAlertMetricShowHideDetailsLabelConverter() {
            return alertMetricShowHideDetailsLabelConverter;
        }
    }
}
