package com.idera.sqlcm.ui.dashboard;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.beEntities.AuditedInstanceBE;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.ui.dialogs.welcome.WelcomeComposer;
import com.idera.sqlcm.ui.preferences.CommonGridPreferencesBean;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.Window;

import java.util.HashMap;
import java.util.List;

public class DashboardViewModel {

    static final String DASHBOARD_CHANGE_INTERVAL_EVENT = "dashboardChangeIntervalEvent";

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        List<AuditedInstanceBE> auditedInstanceBEList = null;
        try {
            auditedInstanceBEList = SQLCMRestClient.getInstance().getAllAuditedInstances(new HashMap<String, Object>());
        } catch (RestException e) {
            e.printStackTrace();
        }
        if (auditedInstanceBEList == null || auditedInstanceBEList.isEmpty()) {
            Window window = (Window) Executions.createComponents(WelcomeComposer.ZUL_URL, null, null);
            window.doHighlighted();
        }
    }
}
