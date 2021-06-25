package com.idera.sqlcm.ui.components.alerts;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMAlert;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.server.web.WebUtil;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.NotifyChange;

import java.util.List;

public class AlertPager {

    private static final String GRID = "grid";

    private static final String PAGE = "page";

    private static int instanceCount = 0;
    public static final int DEFAULT_PAGE = 1;
    public static final int DEFAULT_PAGE_SIZE = 10;
    private int pagerId;
    private long totalSize;
    private AlertGroup alertGroup;
    private long pageSize = DEFAULT_PAGE_SIZE;
    private long activePage = DEFAULT_PAGE;
    private List<CMAlert> alerts;
    long instanceId;
    private String gridId;
    private String pageId;

    public String getGridId() {
        return GRID + alertGroup.getAlertLevel() + alertGroup.getAlertType();
    }

    public String getPageId() {
        return PAGE + alertGroup.getAlertLevel() + alertGroup.getAlertType();
    }

    public AlertPager(long totalSize) {
        this.alertGroup = alertGroup;
        this.totalSize = totalSize;
        ++instanceCount;
        pagerId = instanceCount;
        initialiseModelList();
    }

    public AlertPager(AlertGroup alertGroup, long instanceId, List<CMAlert> alerts) {
        this.instanceId = instanceId;
        this.alertGroup = alertGroup;
        this.totalSize = alertGroup.getAlertsCount();
        this.alerts = alerts;
    }

    public int getPagerId() {
        return pagerId;
    }

    public long getPageSize() {
        return pageSize;
    }

    public void setPageSize(long pageSize) {
        this.pageSize = pageSize;
    }

    public long getTotalSize() {
        return totalSize;
    }

    public void setTotalSize(long totalSize) {
        this.totalSize = totalSize;
    }

    public long getActivePage() {
        return activePage;
    }

    public void setActivePage(long selectedPage) {
        this.activePage = selectedPage + 1;
        initialiseModelList();
    }

    public void initialiseModelList() {
        try {
            this.alerts = SQLCMRestClient.getInstance().getAlerts(
                instanceId,
                alertGroup.getAlertType().getId(),
                alertGroup.getAlertLevel().getId(),
                pageSize,
                activePage);
        } catch (RestException e) {
            WebUtil.showErrorBox(e.getMsgKey());
        }
        BindUtils.postNotifyChange(null, null, this, "alerts");
    }

    public void setAlerts(List<CMAlert> alerts) {
        this.alerts = alerts;
    }

    public List<CMAlert> getAlerts() {
        return alerts;
    }
}
