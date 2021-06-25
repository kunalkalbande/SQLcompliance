package com.idera;

import com.idera.i18n.I18NStrings;
import com.idera.server.StartUpWebServer;
import com.idera.server.web.WebConstants;
import org.apache.catalina.*;
import org.apache.catalina.connector.Connector;
import org.apache.catalina.core.StandardContext;
import org.apache.catalina.core.StandardHost;
import org.apache.catalina.deploy.SecurityCollection;
import org.apache.catalina.deploy.SecurityConstraint;
import org.apache.catalina.loader.WebappLoader;
import org.apache.catalina.startup.Embedded;

import java.io.File;
import java.io.IOException;
import java.util.List;

public class TomcatWrapper implements Wrapper {

    // Tomcat's SSL cipher list is *inclusive*
    private static final String ciphers = "SSL_RSA_WITH_RC4_128_MD5," + "SSL_RSA_WITH_RC4_128_SHA," + "TLS_RSA_WITH_AES_128_CBC_SHA," + "TLS_DHE_RSA_WITH_AES_128_CBC_SHA,"
            + "TLS_DHE_DSS_WITH_AES_128_CBC_SHA," + "SSL_RSA_WITH_3DES_EDE_CBC_SHA," + "SSL_DHE_RSA_WITH_3DES_EDE_CBC_SHA," + "SSL_DHE_DSS_WITH_3DES_EDE_CBC_SHA";

    private static final String NAME = "Tomcat Wrapper";

    private final Boolean started = false;

    private static TomcatWrapper instance = null;

    private Embedded uiContainer;
    ;

    private static final String WEB_APP_DIR = "webapps";

    private static final String TEMP_DIR = WEB_APP_DIR + System.getProperty("file.separator") + "web-temp";
    private static final Integer MAX_IDLE_TIME = 30000;

    private static final String UI_ENGINE_NAME = "UI WebApp Engine";
    private static final String UI_WEBAPP_NAME = "zk-web.war";

    private static final String SSL_REDIRECT_CONSTRAINT_NAME = "globalSSLRedirect";

    protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(TomcatWrapper.class);
    protected static final org.apache.log4j.Logger debug = org.apache.log4j.Logger.getLogger(TomcatWrapper.class);

    protected String appBase = null;
    protected String uiWebAppName = UI_WEBAPP_NAME;
    protected WebServerProperties webServerProperties;
    protected Boolean isLicensed = false;

    protected StandardContext uiContext = null;
    protected StandardHost uiHost = null;
    protected Engine uiEngine = null;
    protected Connector uiHTTPConnector = null;
    protected Connector uiSSLConnector = null;

    protected StandardContext comContext = null;
    protected StandardHost comHost = null;
    protected Engine comEngine = null;
    protected Connector comSSLConnector = null;

    protected Boolean uiContainerRunning = false;

    public static final WebappLoader loader = GlobalConstants.loader;

    private static final class LifecycleEventConnector extends Connector {

        public LifecycleEventConnector() throws Exception {
            super();
        }

        @Override
        public void initialize() throws LifecycleException {
            this.lifecycle.fireLifecycleEvent(INIT_EVENT, null);
            super.initialize();
        }
    }

    private static final LifecycleListener SHUTDOWN_STARTUP_WEB_SERVER_LISTENER = new LifecycleListener() {

        @Override
        public void lifecycleEvent(LifecycleEvent event) {
            if (Lifecycle.INIT_EVENT.equals(event.getType())) {
                try {
                    StartUpWebServer.getInstance().shutdown();
                } catch (IOException e) {
                    logger.error("Unable to stop Start up web server");
                }
            }
        }
    };

    private TomcatWrapper() {
    }

    public static TomcatWrapper getInstance() {
        if (instance == null) {
            instance = new TomcatWrapper();
        }
        return instance;
    }

    @Override
    public String getName() {
        return NAME;
    }

    @Override
    public Boolean isStarted() {
        return this.started;
    }

    public Boolean getIsLicensed() {
        return this.isLicensed;
    }

    public void setIsLicensed(Boolean isLicensed) {
        this.isLicensed = isLicensed;
    }

    @Override
    public void shutdown() throws ShutdownException {
        debug.debug("TomcatWrapper: shutdown called");
        if (this.uiContainer != null) {
            try {
                this.uiContainerRunning = false;
                debug.debug("TomcatWrapper: shutdown: Terminating UI container");
                this.uiContainer.stop();
                logger.info("Container info: " + this.uiContainer.getInfo());
                this.uiContainer.destroy();
                logger.info("Container info after destroy: " + this.uiContainer.getInfo());
                this.uiContainer = null;
            } catch (LifecycleException e) {
                logger.error(e, I18NStrings.WEB_SERVER_TERMINATED_ABNORMALLY);
            }
        }
    }

    public void restart(List<String> dependencies) throws Exception {
        logger.info("Restart in Tomcat Wapper.");
        this.uiContainer.removeContext(this.uiContext);
        this.uiContainer.removeHost(this.uiHost);
        this.uiContainer.removeEngine(this.uiEngine);
        this.uiHTTPConnector.stop();
        this.uiSSLConnector.stop();
        this.uiHTTPConnector.destroy();
        this.uiSSLConnector.destroy();
        this.uiHTTPConnector.removeLifecycleListener(SHUTDOWN_STARTUP_WEB_SERVER_LISTENER);
        this.uiSSLConnector.removeLifecycleListener(SHUTDOWN_STARTUP_WEB_SERVER_LISTENER);
        this.uiContainer.removeConnector(this.uiHTTPConnector);
        this.uiContainer.removeConnector(this.uiSSLConnector);
        this.uiContext = null;
        this.uiHost = null;
        this.uiEngine = null;
        this.uiSSLConnector = null;
        this.uiHTTPConnector = null;
        // Shutdown the main tomcat instance.
        logger.info("Initiating tomcat shutdown");
        this.shutdown();
        // Start the HTTP server to server the progress page.
        this.startHTTPServerForProgressPage();
        // Start the main tomcat instance.
        // Shutting down the HTTP temp server is tied to an init life cycle of a
        // tomcat instance.
        this.start(dependencies);
    }

    /*
     * starts the HTTP server to serve the static startup progress page.
     */
    private void startHTTPServerForProgressPage() {
        try {
            if (this.webServerProperties.getStartupWebServerEnabled() == false) {
                return;
            }
            logger.info("Starting startup HTTP web server");
            StartUpWebServer.getInstance().startup();
        } catch (Throwable ex) {
            logger.warn(ex, I18NStrings.FAILED_TO_START_WEB_SERVER_TO_DISPLAY_LOADING_SCREEN);
        }
    }

    public void setUIWebAppName(String aWarFileName) {
        this.uiWebAppName = aWarFileName;
    }

    public void setWebServerProperties(WebServerProperties aWebServerProperties) {
        this.webServerProperties = aWebServerProperties;
    }

    public void reconfigWebServerProperties(WebServerProperties webServerProperties) throws Exception {
        this.setWebServerProperties(webServerProperties);
        if (this.uiContainerRunning) {
            this.configureUIWebApp(System.getProperty("web.home", "."), null);
        }
    }

    protected void removeTempDirs() throws ShutdownException {
        String webAppsPath = System.getProperty("web.home", ".") + System.getProperty("file.separator") + TEMP_DIR;
        File webApps = new File(webAppsPath);
        if (!webApps.exists() || !webApps.isDirectory()) {
            throw new ShutdownException(I18NStrings.COULD_NOT_IDENTIFY_WEBAPPS_DIRECTORY);
        }
        for (String child : webApps.list()) {
            File dir = new File(webApps, child);
            // Only remove directories, otherwise we nuke the WAR file
            if (dir.isDirectory()) {
                deleteDir(dir);
            }
        }
    }

    private static void deleteDir(File dir) throws ShutdownException {
        if (dir.exists()) {
            if (dir.isDirectory()) {
                for (String child : dir.list()) {
                    deleteDir(new File(dir, child));
                }
            }
            // The directory is now empty so delete it
            if (!dir.delete()) {
                throw new ShutdownException(I18NStrings.COULD_NOT_REMOVE_TEMP_DIRECTORY, dir.toString());
            }
        }
    }

    @Override
    public void start(List<String> dependencies) throws StartupException {
        try {
            String webHome = System.getProperty("web.home", ".");
            debug.debug("TomcatWrapper: start() called with web.home = " + webHome);
            this.uiContainer = new Embedded();
            this.uiContainer.setCatalinaHome(webHome);
            this.uiContainer.setCatalinaBase(webHome);
            this.configureUIWebApp(webHome, dependencies);
            logger.info("TomcatWrapper: start(): WebApps configured");
            this.uiContainer.setAwait(true);
            logger.info("TomcatWrapper: start: Starting UI container");
            this.uiContainer.start();
            this.uiContainerRunning = true;
            logger.info("TomcatWrapper: start: UI container running");
        } catch (Exception ex) {
            throw new StartupException(ex, I18NStrings.COULD_NOT_START_WEB_SERVER);
        }
    }

    protected void configureUIWebApp(String webHome, List<String> dependencies) throws Exception {
        for (String dependency : dependencies) {
            String path = new File(dependency).toURI().toURL().toString();
            logger.info("WebAppLoader : Loading " + path);
            loader.addRepository(path);
        }
        if (this.uiContext == null) {
            /* uiWebAppName = "zk-web"; */
            this.uiContext = (StandardContext) this.uiContainer.createContext("", webHome + "/" + WEB_APP_DIR + "/" + this.uiWebAppName);
            this.uiContext.setLoader(loader);
            this.uiContext.setSessionCookieName(WebConstants.CWF_SESSION_ID_COOKIE_NAME);
            this.uiContext.setReloadable(true);
            this.uiContext.setWorkDir(TEMP_DIR + "/ui");
        }
        if (this.webServerProperties.getSslRedirect()) {
            boolean constraintExists = false;
            for (SecurityConstraint constraint : this.uiContext.findConstraints()) {
                if (constraint.getDisplayName().equals(SSL_REDIRECT_CONSTRAINT_NAME)) {
                    constraintExists = true;
                    break;
                }
            }
            if (!constraintExists) {
                SecurityConstraint constraint = new SecurityConstraint();
                constraint.setDisplayName(SSL_REDIRECT_CONSTRAINT_NAME);
                constraint.setAuthConstraint(false);
                SecurityCollection collection = new SecurityCollection();
                collection.addPattern("/*");
                constraint.setUserConstraint("CONFIDENTIAL");
                constraint.addCollection(collection);
                this.uiContext.addConstraint(constraint);
            }
        } else {
            for (SecurityConstraint constraint : this.uiContext.findConstraints()) {
                if (constraint.getDisplayName().equals(SSL_REDIRECT_CONSTRAINT_NAME)) {
                    this.uiContext.removeConstraint(constraint);
                }
            }
        }
        if (this.uiHost == null) {
            this.uiHost = (StandardHost) this.uiContainer.createHost("localhost", new File(webHome).getCanonicalPath());
            this.uiHost.addChild(this.uiContext);
            this.uiHost.setWorkDir(TEMP_DIR + "/ui");
            this.uiHost.setUnpackWARs(ServerConstants.WEB_HOT_DEPLOY_ENABLED);
            this.uiHost.setAutoDeploy(ServerConstants.WEB_HOT_DEPLOY_ENABLED);
        }
        if (this.uiEngine == null) {
            this.uiEngine = this.uiContainer.createEngine();
            this.uiEngine.setName(UI_ENGINE_NAME);
            this.uiEngine.addChild(this.uiHost);
            this.uiEngine.setDefaultHost(this.uiHost.getName());
            this.uiContainer.addEngine(this.uiEngine);
        }
        boolean httpConnectorModified = false, sslConnectorModified = false;
        if (this.uiHTTPConnector != null) {
            int currentPort = this.uiHTTPConnector.getPort();
            int currentMaxThreads = (Integer) this.uiHTTPConnector.getAttribute("maxThreads");
            int newPort = this.webServerProperties.getHttpPort();
            int newMaxThreads = this.webServerProperties.getHttpMaxConnections();
            Object rdPortObj = this.uiHTTPConnector.getAttribute("redirectPort");
            boolean redirectPortChanged = false;
            if (rdPortObj == null && this.webServerProperties.getSslRedirect()) {
                redirectPortChanged = true;
            } else if (rdPortObj != null && !this.webServerProperties.getSslRedirect()) {
                redirectPortChanged = true;
            } else if (rdPortObj != null && this.webServerProperties.getSslRedirect()) {
                Integer rdPort = Integer.parseInt(rdPortObj.toString());
                if (!rdPort.equals(this.webServerProperties.getSslPort())) {
                    redirectPortChanged = true;
                }
            }
            boolean httpEnabled = this.webServerProperties.getHttpEnabled();
            boolean portChanged = currentPort != newPort;
            boolean maxThreadsChanged = currentMaxThreads != newMaxThreads;
            if (!httpEnabled || portChanged || maxThreadsChanged || redirectPortChanged) {
                this.uiContainer.removeConnector(this.uiHTTPConnector);
                this.uiHTTPConnector.pause();
                this.uiHTTPConnector.destroy();
                this.uiHTTPConnector = null;
            }
            if (httpEnabled && (portChanged || maxThreadsChanged || redirectPortChanged)) {
                this.uiHTTPConnector = this
                        .createHTTPConnector(newPort, this.webServerProperties.getSslRedirect() ? this.webServerProperties.getSslPort() : -1,
                                newMaxThreads);
                this.uiHTTPConnector.addLifecycleListener(SHUTDOWN_STARTUP_WEB_SERVER_LISTENER);
                this.uiContainer.addConnector(this.uiHTTPConnector);
                httpConnectorModified = true;
            }
        } else if (this.webServerProperties.isHttpEnabled()) {
            this.uiHTTPConnector = this.createHTTPConnector(this.webServerProperties.getHttpPort(),
                    this.webServerProperties.getSslRedirect() ? this.webServerProperties.getSslPort() : -1,
                    this.webServerProperties.getHttpMaxConnections());
            this.uiHTTPConnector.addLifecycleListener(SHUTDOWN_STARTUP_WEB_SERVER_LISTENER);
            this.uiContainer.addConnector(this.uiHTTPConnector);
            httpConnectorModified = true;
        }
        if (this.uiSSLConnector != null) {
            int currentPort = this.uiSSLConnector.getPort();
            int currentMaxThreads = (Integer) this.uiSSLConnector.getAttribute("maxThreads");
            int newPort = this.webServerProperties.getSslPort();
            int newMaxThreads = this.webServerProperties.getSslMaxConnections();
            String newKeystorePath = this.webServerProperties.getSslKeystorePath();
            String oldKeystorePath = this.uiSSLConnector.getAttribute("keystoreFile").toString();
            boolean sslEnabled = this.webServerProperties.getSslEnabled();
            boolean portChanged = currentPort != newPort;
            boolean maxThreadsChanged = currentMaxThreads != newMaxThreads;
            boolean keystoreChanged = !newKeystorePath.equals(oldKeystorePath);
            if (!sslEnabled || portChanged || maxThreadsChanged || keystoreChanged) {
                this.uiContainer.removeConnector(this.uiSSLConnector);
                this.uiSSLConnector.pause();
                this.uiSSLConnector.destroy();
                this.uiSSLConnector = null;
            }
            if (sslEnabled && (portChanged || maxThreadsChanged || keystoreChanged)) {
                this.uiSSLConnector = this.createSSLConnector(newPort, newMaxThreads, this.webServerProperties.getSslKeystorePath());
                this.uiSSLConnector.addLifecycleListener(SHUTDOWN_STARTUP_WEB_SERVER_LISTENER);
                this.uiContainer.addConnector(this.uiSSLConnector);
                sslConnectorModified = true;
            }
        } else if (this.webServerProperties.isSslEnabled()) {
            this.uiSSLConnector = this.createSSLConnector(this.webServerProperties.getSslPort(), this.webServerProperties.getSslMaxConnections(),
                    this.webServerProperties.getSslKeystorePath());
            this.uiSSLConnector.addLifecycleListener(SHUTDOWN_STARTUP_WEB_SERVER_LISTENER);
            this.uiContainer.addConnector(this.uiSSLConnector);
            sslConnectorModified = true;
        }
        if (this.uiContainerRunning) {
            if (this.uiHTTPConnector != null && httpConnectorModified) {
                this.uiHTTPConnector.start();
            }
            if (this.uiSSLConnector != null && sslConnectorModified) {
                this.uiSSLConnector.start();
            }
        }
    }

    protected Connector createHTTPConnector(int port, int sslRedirectPort, int maxThreads) throws Exception {
        Connector connector = new LifecycleEventConnector();
        org.apache.tomcat.util.IntrospectionUtils.setProperty(connector, "port", "" + port);
        connector.setAttribute("maxThreads", maxThreads);
        connector.setAttribute("keepAliveTimeout", MAX_IDLE_TIME);
        connector.setAttribute("connectionTimeout", MAX_IDLE_TIME);
        if (sslRedirectPort > 0) {
            org.apache.tomcat.util.IntrospectionUtils.setProperty(connector, "redirectPort", "" + sslRedirectPort);
        }
        return connector;
    }

    protected Connector createSSLConnector(int port, int maxThreads, String keyStorePath) throws Exception {
        Connector connector = new LifecycleEventConnector();
        connector.setScheme("https");
        connector.setSecure(true);
        connector.setProperty("SSLEnabled", "true");
        org.apache.tomcat.util.IntrospectionUtils.setProperty(connector, "port", "" + port);
        connector.setAttribute("maxThreads", maxThreads);
        connector.setAttribute("keepAliveTimeout", MAX_IDLE_TIME);
        connector.setAttribute("connectionTimeout", MAX_IDLE_TIME);
        connector.setAttribute("scheme", "https");
        // connector.setAttribute("sslProtocol", "SSLv3");
        connector.setAttribute("secure", true);
        connector.setAttribute("ciphers", ciphers);
        connector.setAttribute("keystoreFile", keyStorePath);
        connector.setAttribute("keystorePass", "password");
        connector.setAttribute("truststoreFile", keyStorePath);
        connector.setAttribute("truststorePass", "password");
        return connector;
    }
}
