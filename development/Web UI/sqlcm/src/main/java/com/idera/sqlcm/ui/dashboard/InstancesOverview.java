package com.idera.sqlcm.ui.dashboard;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.server.web.ELFunctions.IconSize;
import com.idera.sqlcm.entities.CMAlertsSummary;
import com.idera.sqlcm.entities.CMAuditedAlerts;
import com.idera.sqlcm.entities.CMEnvironmentDetails;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.facade.CMEnvironmentFacade;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.filter.FilterData;
import com.idera.sqlcm.ui.instances.InstancesColumns;
import com.idera.sqlcm.ui.instances.InstancesGridViewModel;
import com.idera.sqlcm.ui.instances.filters.InstancesOptionFilterValues;
import com.idera.sqlcm.ui.instancesAlerts.AlertsColumns;
import com.idera.sqlcm.ui.instancesAlerts.filters.AlertsOptionValues;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zul.ListModelList;

import java.util.HashMap;
import java.util.Map;

public class InstancesOverview {

    private static final Logger logger = Logger.getLogger(InstancesOverview.class);

    CMEnvironmentDetails cmEnvironmentDetails;

    CMAuditedAlerts cmAuditedInstances;

    CMAuditedAlerts cmAuditedDatabases;

    String systemStatusLabel;

    String systemStatusImage;

    private int days = Interval.SEVEN_DAY.getDays();

    private int refreshDuration;    

	public int getRefreshDuration() {
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
	this.refreshDuration = refreshDuration;
	}
    
    public enum SystemStatus {
        OK(0, ELFunctions.getLabel(SQLCMI18NStrings.SYSTEM_STATUS_OK), "ok"),
        WARNING(1, ELFunctions.getLabel(SQLCMI18NStrings.SYSTEM_STATUS_WARNING), "warning"),
        ERROR(2, ELFunctions.getLabel(SQLCMI18NStrings.SYSTEM_STATUS_ERROR), "critical"),
        NO_INSTANCES(3, ELFunctions.getLabel(SQLCMI18NStrings.SYSTEM_STATUS_NO_REGISTERED_INSTANCES), "warning");

        private int id;

        private String label;

        private String imageName;

		private static Map<Integer, SystemStatus> lookup = new HashMap<Integer, SystemStatus>();

        static {
            for (SystemStatus status : SystemStatus.values()) {
                lookup.put(status.id, status);
            }
        }

        SystemStatus(int id, String label, String imageName) {
            this.label = label;
            this.id = id;
            this.imageName = imageName;
        }

        public int getId() {
            return id;
        }

        public String getLabel() {
            return label;
        }

        public String getImageName() {
            return imageName;
        }

        public static String findLabelByKey(int statusId) {
            return lookup.get(statusId).getLabel();
        }

        public static String findImageByKey(int statusId) {
            return lookup.get(statusId).getImageName();
        }
    }

    @Init
    public void init() {
        try {
            subscribeToDashboardChangeInterval();
            loadAlertsSummary();
            loadEnvironmentDetails();
            String refreshDuration= RefreshDurationFacade.getRefreshDuration();
    		int refDuration=Integer.parseInt(refreshDuration);
    		refDuration=refDuration*1000;
    		setRefreshDuration(refDuration);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERR_GET_REST_EXCEPTION);
        }
    }

   

	protected void subscribeToDashboardChangeInterval() {
        EventQueue<Event> eq = EventQueues.lookup(DashboardViewModel.DASHBOARD_CHANGE_INTERVAL_EVENT, EventQueues.SESSION, true);
        eq.subscribe(new EventListener<Event>() {
            public void onEvent(Event event) throws Exception {
                days = (int) event.getData();
                loadAlertsSummary();
                loadEnvironmentDetails();
                BindUtils.postNotifyChange(null, null, InstancesOverview.this, "cmAuditedInstances");
                BindUtils.postNotifyChange(null, null, InstancesOverview.this, "systemStatusImage");
                BindUtils.postNotifyChange(null, null, InstancesOverview.this, "cmEnvironmentDetails");
                BindUtils.postNotifyChange(null, null, InstancesOverview.this, "systemStatusLabel");
            }
        });
    }

    private void loadAlertsSummary() {
        try {
            CMAlertsSummary cmAlertsSummary = CMEnvironmentFacade.getEnvironmentAlerts(days);
            cmAuditedInstances = cmAlertsSummary.getAuditedInstances();
            cmAuditedDatabases = cmAlertsSummary.getAuditedDatabases();
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ERR_LOADING_DASHBOARD_SIDE_BAR);
        }
    }

    private void loadEnvironmentDetails() throws RestException {
        String iconSize = IconSize.SMALL.getStringValue();
        cmEnvironmentDetails = CMEnvironmentFacade.getCMEnvironmentDetails();
        systemStatusLabel = SystemStatus.findLabelByKey(cmEnvironmentDetails.getEnvironmentHealth());
        String image = SystemStatus.findImageByKey(cmEnvironmentDetails.getEnvironmentHealth());
        systemStatusImage = com.idera.sqlcm.server.web.ELFunctions.getImageURL(image, iconSize);
    }

    public CMEnvironmentDetails getCmEnvironmentDetails() {
        return cmEnvironmentDetails;
    }

    public CMAuditedAlerts getCmAuditedInstances() {
        return cmAuditedInstances;
    }

    public CMAuditedAlerts getCmAuditedDatabases() {
        return cmAuditedDatabases;
    }

    public String getSystemStatusLabel() {
        return systemStatusLabel;
    }

    public String getSystemStatusImage() {
        return systemStatusImage;
    }

    public int getDays() {
        return days;
    }

    public void setDays(int days) {
        this.days = days;
    }

    @Command("refreshEvents")
    public void refreshEvent() {
            try {
            	subscribeToDashboardChangeInterval();
                loadAlertsSummary();
                loadEnvironmentDetails();
            } catch (RestException e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_AUDITED_DATABASE_LIST);
            }
        BindUtils.postNotifyChange(null, null, this, "*");
    }
    
    @Command("openAllRegisteredSQLServers")
    public void openAllRegisteredSQLServers() {
        FilterData filterData = new FilterData();
        filterData.put(InstancesColumns.AUDIT_STATUS.getColumnId(), null);
        PreferencesUtil.getInstance().setGridFilterPreferencesInSession(InstancesGridViewModel.INSTANCES_SESSION_VARIABLE_NAME, filterData);
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("auditedInstance"));
    }

    @Command("openAllAuditedSQLServers")
    public void openAllAuditedSQLServers() {
        FilterData filterData = new FilterData();
        filterData.put(InstancesColumns.AUDIT_STATUS.getColumnId(), String.valueOf(InstancesOptionFilterValues.AUDIT_STATUS_ENABLED.getIntValue()));
        PreferencesUtil.getInstance().setGridFilterPreferencesInSession(InstancesGridViewModel.INSTANCES_SESSION_VARIABLE_NAME, filterData);
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("auditedInstance"));
    }

    @Command("openAllAuditedDatabases")
    public void openAllAuditedDatabases() {
        FilterData filterData = new FilterData();
        filterData.put(InstancesColumns.AUDIT_STATUS.getColumnId(), String.valueOf(InstancesOptionFilterValues.AUDIT_STATUS_DISABLED.getIntValue()));
        PreferencesUtil.getInstance().setGridFilterPreferencesInSession(InstancesGridViewModel.INSTANCES_SESSION_VARIABLE_NAME, filterData);
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instancesAlerts"));
    }

    @Command("openSevereAlerts")
    public void openSevereAlerts() {
        performInstancesAlertsRedirect(String.valueOf(AlertsOptionValues.SEVERE_STATUS.getIntValue()));
    }

    @Command("openHighAlerts")
    public void openHighAlerts() {
        performInstancesAlertsRedirect(String.valueOf(AlertsOptionValues.HIGH_STATUS.getIntValue()));
    }

    @Command("openMediumAlerts")
    public void openMediumAlerts() {
        performInstancesAlertsRedirect(String.valueOf(AlertsOptionValues.MEDIUM_STATUS.getIntValue()));
    }

    @Command("openLowAlerts")
    public void openLowAlerts() {
        performInstancesAlertsRedirect(String.valueOf(AlertsOptionValues.LOW_STATUS.getIntValue()));
    }

    private void performInstancesAlertsRedirect(String alertValue) {
        FilterData filterData = new FilterData();
        filterData.put(AlertsColumns.LEVEL.getColumnId(), alertValue);
        PreferencesUtil.getInstance().setGridFilterPreferencesInSession("InstancesAlertsSessionDataBean", filterData);
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instancesAlerts"));

    }

}
