package com.idera.sqlcm.ui.preferences;

import com.idera.sqlcm.ui.components.filter.FilterData;
import org.zkoss.zk.ui.Executions;

public class PreferencesUtil {

    private static final PreferencesUtil preferencesUtil = new PreferencesUtil();

    private PreferencesUtil() {
    }

    public static PreferencesUtil getInstance() {
        return preferencesUtil;
    }

    public CommonGridPreferencesBean getGridPreferencesInSession(String sessionVariableName) {
        CommonGridPreferencesBean gp = (CommonGridPreferencesBean) Executions.getCurrent().getSession().getAttribute(sessionVariableName);
        if (gp == null) {
            gp = new CommonGridPreferencesBean();
        }
        return gp;
    }

    public void setGridFilterPreferencesInSession(String sessionVariableName, FilterData filterData) {
        CommonGridPreferencesBean gp = getGridPreferencesInSession(sessionVariableName);
        if (filterData != null) {
            gp.setFilters(filterData);
        }
        Executions.getCurrent().getSession().setAttribute(sessionVariableName, gp);
    }

    public void setGridSortingPreferencesInSession(String sessionVariableName, String sortedColumnId, boolean isAscendingSortDirection) {
        CommonGridPreferencesBean gp = getGridPreferencesInSession(sessionVariableName);
        if (sortedColumnId != null && sortedColumnId.length() > 0) {
            gp.setSortedColumnId(sortedColumnId);
            gp.setAscendingSortDirection(isAscendingSortDirection);
        }
        Executions.getCurrent().getSession().setAttribute(sessionVariableName, gp);
    }

    public void setGridPagingPreferencesInSession(String sessionVariableName, int gridRowsCount) {
        CommonGridPreferencesBean gp = getGridPreferencesInSession(sessionVariableName);
        if (gridRowsCount > 0) {
            gp.setGridRowsCount(gridRowsCount);
        }
        Executions.getCurrent().getSession().setAttribute(sessionVariableName, gp);
    }

    public AlertsPreferencesBean getAlertsPreferencesInSession(String sessionVariableName) {
        AlertsPreferencesBean gp = (AlertsPreferencesBean) Executions.getCurrent().getSession().getAttribute(sessionVariableName);
        if (gp == null) {
            gp = new AlertsPreferencesBean();
        }
        return gp;
    }

    public void setAlertsHiddenInSession(String sessionVariableName, boolean alertsHidden) {
        AlertsPreferencesBean gp = getAlertsPreferencesInSession(sessionVariableName);
        gp.setAlertsHidden(alertsHidden);
        Executions.getCurrent().getSession().setAttribute(sessionVariableName, gp);
    }

    public void setAlertsShowAllInSession(String sessionVariableName, boolean alertsShowAll) {
        AlertsPreferencesBean gp = getAlertsPreferencesInSession(sessionVariableName);
        gp.setAlertsShowAll(alertsShowAll);
        Executions.getCurrent().getSession().setAttribute(sessionVariableName, gp);
    }

}
