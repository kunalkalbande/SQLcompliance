package com.idera;

public class WebServerProperties {

    protected boolean httpEnabled;
    protected Integer httpPort;
    protected Integer httpMaxConnections;
    protected Integer startupWebServerWaitTime;

    protected boolean sslEnabled;
    protected boolean startupWebServerEnabled;
    protected boolean sslRedirect;
    protected Integer sslPort;
    protected Integer monitorPort;
    /**
     *
     */
    protected Integer sslMaxConnections;
    protected String sslKeystorePath;
    protected boolean mockCoreServices;
    protected String productClassPath;

    public String getProductClassPath() {
        return this.productClassPath;
    }

    public Integer getMonitorPort() {
        return this.monitorPort;
    }

    public void setMonitorPort(Integer port) {
        this.monitorPort = port;
    }

    public void setProductClassPath(String productClassPath) {
        this.productClassPath = productClassPath;
    }

    public boolean isMockCoreServices() {
        return this.mockCoreServices;
    }

    public void setMockCoreServices(boolean mockCoreServices) {
        this.mockCoreServices = mockCoreServices;
    }

    public WebServerProperties() {
    }

    public boolean isHttpEnabled() {
        return this.httpEnabled;
    }

    public boolean getHttpEnabled() {
        return this.isHttpEnabled();
    }

    public void setHttpEnabled(boolean httpEnabled) {
        this.httpEnabled = httpEnabled;
    }

    public Integer getHttpPort() {
        return this.httpPort;
    }

    public void setHttpPort(Integer httpPort) {
        this.httpPort = httpPort;
    }

    public Integer getHttpMaxConnections() {
        return this.httpMaxConnections;
    }

    public void setHttpMaxConnections(Integer httpMaxConnections) {
        this.httpMaxConnections = httpMaxConnections;
    }

    public boolean isSslEnabled() {
        return this.sslEnabled;
    }

    public boolean getSslEnabled() {
        return this.isSslEnabled();
    }

    public void setSslEnabled(boolean sslEnabled) {
        this.sslEnabled = sslEnabled;
    }

    public boolean getSslRedirect() {
        return this.sslRedirect;
    }

    public void setSslRedirect(boolean sslRedirect) {
        this.sslRedirect = sslRedirect;
    }

    public Integer getSslPort() {
        return this.sslPort;
    }

    public void setSslPort(Integer sslPort) {
        this.sslPort = sslPort;
    }

    public Integer getSslMaxConnections() {
        return this.sslMaxConnections;
    }

    public void setSslMaxConnections(Integer sslMaxConnections) {
        this.sslMaxConnections = sslMaxConnections;
    }

    public String getSslKeystorePath() {
        return this.sslKeystorePath;
    }

    public void setSslKeystorePath(String sslKeystorePath) {
        this.sslKeystorePath = sslKeystorePath;
    }

    public boolean getStartupWebServerEnabled() {
        return this.startupWebServerEnabled;
    }

    public void setStartupWebServerEnabled(boolean startupWebServerEnabled) {
        this.startupWebServerEnabled = startupWebServerEnabled;
    }

    public Integer getStartupWebServerWaitTime() {
        return this.startupWebServerWaitTime;
    }

    public void setStartupWebServerWaitTime(Integer startupWebServerWaitTime) {
        this.startupWebServerWaitTime = startupWebServerWaitTime;
    }
}
