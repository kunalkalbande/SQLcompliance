package com.idera.sqlcm.entities.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

public class CMAgentProperties {
    @JsonProperty("serverId")
    private long serverId;

    @JsonProperty("generalProperties")
    private CMAgentGeneralProperties generalProperties;

    @JsonProperty("deployment")
    private CMAgentDeployment deployment;

    @JsonProperty("sqlServerList")
    private List<CMAgentSqlServer> sqlServerList;

    @JsonProperty("traceOptions")
    private CMAgentTraceOptions traceOptions;

    public long getServerId() {
        return serverId;
    }

    public void setServerId(long serverId) {
        this.serverId = serverId;
    }

    public CMAgentGeneralProperties getGeneralProperties() {
        return generalProperties;
    }

    public void setGeneralProperties(CMAgentGeneralProperties generalProperties) {
        this.generalProperties = generalProperties;
    }

    public CMAgentDeployment getDeployment() {
        return deployment;
    }

    public void setDeployment(CMAgentDeployment deployment) {
        this.deployment = deployment;
    }

    public List<CMAgentSqlServer> getSqlServerList() {
        return sqlServerList;
    }

    public void setSqlServerList(List<CMAgentSqlServer> sqlServerList) {
        this.sqlServerList = sqlServerList;
    }

    public CMAgentTraceOptions getTraceOptions() {
        return traceOptions;
    }

    public void setTraceOptions(CMAgentTraceOptions traceOptions) {
        this.traceOptions = traceOptions;
    }
}
