package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.instances.CMAgentProperties;
import com.idera.sqlcm.enumerations.AgentDeploymentType;
import com.idera.sqlcm.enumerations.LoggingLevel;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Tabbox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;

public class AgentPropertiesViewModel {
    public static final String ZUL_URL = "~./sqlcm/dialogs/agentProperties/agentPropertiesDialog.zul";
    public static final String INSTANCE_ID = "instance-id";
    public static final int NO_LIMIT = -1;

    public enum Tab {
        GENERAL(0, "http://wiki.idera.com/display/SQLCM/SQLcompliance+Agent+Properties+window+-+General+tab"),
        DEPLOYMENT(1, "http://wiki.idera.com/display/SQLCM/SQLcompliance+Agent+Properties+window+-+Deployment+tab"),
        SQL_SERVERS(2, "http://wiki.idera.com/display/SQLCM/SQLcompliance+Agent+Properties+window+-+SQL+Servers+tab"),
        TRACE_OPTIONS(3, "http://wiki.idera.com/display/SQLCM/SQLcompliance+Agent+Properties+window+-+Trace+Options+tab");

        private String helpUrl;
        private int id;

        private Tab(int id, String helpUrl) {
            this.id = id;
            this.helpUrl = helpUrl;
        }

        public String getHelpUrl() {
            return helpUrl;
        }

        public int getId() {
            return id;
        }

        public static Tab getByIndex(int index) {
            Tab result = null;
            Tab[] values = Tab.values();
            for (Tab value : values) {
                if (value.getId() == index) {
                    result = value;
                }
            }
            return result;
        }
    }

    @Wire("#include_generalTab #auditSettingsStatus")
    Textbox auditSettingsStatus;

    @Wire
    Tabbox tb;
    
    private String help;
    private CMAgentProperties agentProperties;
    private ListModelList<LoggingLevel> loggingLevelListModelList;
    private ListModelList<AgentDeploymentType> agentDeploymentTypeListModelList;

    private int traceLimit;
    private boolean enableTraceLimit;

    private int unattendedTimeLimit;
    private boolean enableUnattendedTimeLimit;

    public String getHelp() {
        return Tab.getByIndex(tb.getSelectedPanel().getIndex()).getHelpUrl();
    }

    public CMAgentProperties getAgentProperties() {
        return agentProperties;
    }

    public void setAgentProperties(CMAgentProperties agentProperties) {
        this.agentProperties = agentProperties;
    }

    public ListModelList<LoggingLevel> getLoggingLevelListModelList() {
        return loggingLevelListModelList;
    }

    public void setLoggingLevelListModelList(ListModelList<LoggingLevel> loggingLevelListModelList) {
        this.loggingLevelListModelList = loggingLevelListModelList;
    }

    public ListModelList<AgentDeploymentType> getAgentDeploymentTypeListModelList() {
        return agentDeploymentTypeListModelList;
    }

    public boolean isDisableAgentDeployment() {
        return !agentProperties.getDeployment().getServiceAccount().isEmpty();
    }

    public int getTraceLimit() {
        return traceLimit;
    }

    public void setTraceLimit(int traceLimit) {
        this.traceLimit = traceLimit;
    }

    public boolean isEnableTraceLimit() {
        return enableTraceLimit;
    }

    public void setEnableTraceLimit(boolean enableTraceLimit) {
        this.enableTraceLimit = enableTraceLimit;
    }

    public int getUnattendedTimeLimit() {
        return unattendedTimeLimit;
    }

    public void setUnattendedTimeLimit(int unattendedTimeLimit) {
        this.unattendedTimeLimit = unattendedTimeLimit;
    }

    public boolean isEnableUnattendedTimeLimit() {
        return enableUnattendedTimeLimit;
    }

    public void setEnableUnattendedTimeLimit(boolean enableUnattendedTimeLimit) {
        this.enableUnattendedTimeLimit = enableUnattendedTimeLimit;
    }

    public static void showAgentPropertiesDialog(Long instanceId) {
        if (instanceId == null) {
            throw new RuntimeException(" Instance Id must not be null! ");
        }
        Map<String, Object> args = new HashMap<>();
        args.put(INSTANCE_ID, instanceId);

        Window window = (Window) Executions.createComponents(AgentPropertiesViewModel.ZUL_URL, null, args);
        window.doHighlighted();
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        help = Tab.GENERAL.getHelpUrl();

        HashMap<String, Object> args = (HashMap<String, Object>)Executions.getCurrent().getArg();
        Long instanceId = (Long)args.get(INSTANCE_ID);
        agentProperties = initAgentProperties(instanceId);

        loggingLevelListModelList = initLoggingLevelModelList();
        agentDeploymentTypeListModelList = initAgentDeploymentTypeList();
        traceLimit = initTraceLimit();
        unattendedTimeLimit = initUnattendedTimeLimit();
    }

    @Command
    @NotifyChange("#auditSettingsStatus")
    public void updateAuditConfigurationForServer(@BindingParam("instanceId") Long serverId) {
        try {
            String newStatus = InstancesFacade.updateAuditConfigurationForServer(serverId);
            auditSettingsStatus.setValue(newStatus);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPDATE_AUDIT_CONFIGURATION_FOR_SERVER);
        }
    }

    @Command
    @NotifyChange("enableTraceLimit")
    public void enableTraceLimit(@BindingParam("noLimit") boolean noLimit) {
        enableTraceLimit = !noLimit;
    }

    @Command
    @NotifyChange("enableUnattendedTimeLimit")
    public void enableUnattendedTimeLimit(@BindingParam("noLimit") boolean noLimit) {
        enableUnattendedTimeLimit = !noLimit;
    }

    @Command
    public void updateAgentProperties(@BindingParam("comp") Window x) {
        try {
            agentProperties.getGeneralProperties().getAgentSettings().setLoggingLevel(
                    Utils.getSingleSelectedItem(loggingLevelListModelList).getIndex());
            agentProperties.getTraceOptions().setTraceDirectorySizeLimit(getTraceLimitForSaving());
            agentProperties.getTraceOptions().setUnattendedTimeLimit(getUnattendedTimeLimitForSaving());

            InstancesFacade.updateAgentProperties(agentProperties);
            x.detach();
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPDATE_AGENT_PROPERTIES);
        }
    }

    @Command
    @NotifyChange("help")
    public void getHelpLink() {}

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    private ListModelList<LoggingLevel> initLoggingLevelModelList() {
        ListModelList<LoggingLevel> loggingLevelListModelList = new ListModelList<>();
        loggingLevelListModelList.addAll(Arrays.asList(LoggingLevel.values()));
        loggingLevelListModelList.setSelection(Arrays.asList(
                LoggingLevel.getByIndex(agentProperties.getGeneralProperties().getAgentSettings().getLoggingLevel())));
        return loggingLevelListModelList;
    }

    private ListModelList<AgentDeploymentType> initAgentDeploymentTypeList() {
        ListModelList<AgentDeploymentType> agentDeploymentTypeListModelList = new ListModelList<>();
        agentDeploymentTypeListModelList.addAll(Arrays.asList(AgentDeploymentType.values()));
        agentDeploymentTypeListModelList.setSelection(Arrays.asList(
                AgentDeploymentType.getByIndex(agentProperties.getDeployment().isWasManuallyDeployed())));
        return agentDeploymentTypeListModelList;
    }

    private CMAgentProperties initAgentProperties(Long instanceId) {
        CMAgentProperties agentProperties = null;
        try {
            agentProperties = InstancesFacade.getAgentProperties(instanceId);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_AGENT_PROPERTIES);
        }
        return agentProperties;
    }

    private int initTraceLimit() {
        if (agentProperties.getTraceOptions().getTraceDirectorySizeLimit() == NO_LIMIT) {
            enableTraceLimit = false;
            return 0;
        } else {
            enableTraceLimit = true;
            return agentProperties.getTraceOptions().getTraceDirectorySizeLimit();
        }
    }

    private int initUnattendedTimeLimit() {
        if (agentProperties.getTraceOptions().getUnattendedTimeLimit() == NO_LIMIT) {
            enableUnattendedTimeLimit = false;
            return 0;
        } else {
            enableUnattendedTimeLimit = true;
            return agentProperties.getTraceOptions().getUnattendedTimeLimit();
        }
    }

    private int getTraceLimitForSaving() {
        if (enableTraceLimit) {
            return traceLimit;
        } else {
            return NO_LIMIT;
        }
    }

    private int getUnattendedTimeLimitForSaving() {
        if (enableUnattendedTimeLimit) {
            return unattendedTimeLimit;
        } else {
            return NO_LIMIT;
        }
    }
}
