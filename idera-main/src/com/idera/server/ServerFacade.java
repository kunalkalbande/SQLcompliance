package com.idera.server;

import com.idera.GlobalConstants;
import com.idera.GlobalUtil;
import com.idera.ServerConstants;
import com.idera.WebServerProperties;
import com.idera.i18n.I18NStrings;
import com.idera.i18n.LangProperties;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;
import java.util.regex.Pattern;

public class ServerFacade {

    private static final String PRODUCT_DEV_CLASS_PATH = "productClassPath";
    protected static final String HTTP_ENABLED_PROPERTY = "http-enabled";
    protected static final String HTTP_STARTUP_WEBSERVER_ENABLED_PROPERTY = "startup-webserver-enabled";
    protected static final String HTTP_MAX_CONN_PROPERTY = "http-max-connections";
    protected static final String HTTP_PORT_PROPERTY = "http-port";
    protected static final String SSL_ENABLED_PROPERTY = "ssl-enabled";
    protected static final String SSL_REDIRECT_PROPERTY = "ssl-redirect";
    protected static final String SSL_PORT_PROPERTY = "ssl-port";
    protected static final String KEYSTORE_DIR_PROPERTY = "ssl-keystore";
    protected static final String SSL_MAX_CONN_PROPERTY = "ssl-max-connections";
    protected static final Integer DEFAULT_HTTP_PORT = 80;
    protected static final Integer DEFAULT_HTTPS_PORT = 443;
    protected static final Integer DEFAULT_MAX_CONNECTIONS = 100;
    protected static final String STARTUP_SERVER_WAIT_TIME_PROPERTY = "startup-webserver-wait-duration";
    protected static final Integer DEFAULT_STARTUP_SERVER_WAIT_DURATION = 5000;
    protected static final String DEFAULT_SSL_KEYSTORE = GlobalUtil.configDir + System.getProperty("file.separator") + "keystore";
    protected static final String DEFAULT_COM_SSL_KEYSTORE = GlobalUtil.configDir + System.getProperty("file.separator") + "comkeystore";
    protected static final String MONITOR_PORT = "monitor-port";
    // protected static final String DEFAULT_COM_SSL_KEYSTORE =
    // GlobalUtil.configDir + System.getProperty("file.separator") + "keystore";

    protected static final Integer DEFAULT_API_HTTP_PORT = 9080;
    protected static final Integer DEFAULT_API_SSL_PORT = 9443;
    protected static final Integer DEFAULT_COM_SSL_PORT = 5443;

    protected static final String SERVER_BUILD_DATE_FORMAT = "yyyy/MM/dd HH:mm:ss";

    protected static final Pattern RESOURCE_BUNDLE_LOCALE_PATTERN = Pattern.compile(GlobalConstants.DEFAULT_MESSAGE_BUNDLE + "_(.+)\\.properties");

    public static final String VALID_DATABASE_DRIVER_SUFFIX = ".jar";

    public static final String MOCK_CORE_SERVICES_PROPERTY = "mock-core-services";

    public static String getLangPropertiesFilePath() {
        String langPropertiesPath = GlobalUtil.configDir + System.getProperty("file.separator") + ServerConstants.LANG_PROPERTIES_FILE_NAME;
        return langPropertiesPath;
    }

    public static String getServerPropertiesFilePath() {
        String serverPropertiesPath = GlobalUtil.configDir + System.getProperty("file.separator") + ServerConstants.SERVER_PROPERTIES_FILE_NAME;
        return serverPropertiesPath;
    }

    public static String getWebConfigFilePath() {
        String webConfigPath = GlobalUtil.configDir + System.getProperty("file.separator") + ServerConstants.WEB_CONFIG_FILE_NAME;
        return webConfigPath;
    }

    public static LangProperties getLangProperties() throws ServerException {
        String propFilePath = getLangPropertiesFilePath();
        try {
            return new LangProperties(propFilePath);
        } catch (IOException ex) {
            throw new ServerException(ex, I18NStrings.COULD_NOT_LOAD_LANG_PROPERTIES_FILE);
        }
    }

    public static void setLangProperties(LangProperties langProperties) throws ServerException {
        try {
            langProperties.saveToFile(getLangPropertiesFilePath());
        } catch (IOException ex) {
            throw new ServerException(ex, I18NStrings.COULD_NOT_SAVE_SERVER_PROPERTIES_FILE);
        }
    }

    public static String getLanguageFilesPath() {
        return GlobalUtil.configDir + "/" + ServerConstants.LANGUAGE_FILES_FOLDER_NAME;
    }

    public static WebServerProperties getWebServerProperties() throws IOException {
        WebServerProperties properties = new WebServerProperties();
        String webConfigPath = getWebConfigFilePath();
        File file = new File(webConfigPath);
        Properties props = new Properties();
        if (file.exists()) {
            InputStream is = new FileInputStream(file);
            try {
                props.load(is);
            } finally {
                is.close();
            }
        }
        properties.setMonitorPort(Integer.parseInt(props.getProperty(MONITOR_PORT)));
        properties.setHttpEnabled(Boolean.parseBoolean(props.getProperty(HTTP_ENABLED_PROPERTY, "true")));
        properties.setStartupWebServerEnabled(Boolean.parseBoolean(props.getProperty(HTTP_STARTUP_WEBSERVER_ENABLED_PROPERTY, "true")));
        properties.setHttpPort(Integer.parseInt(props.getProperty(HTTP_PORT_PROPERTY, DEFAULT_HTTP_PORT.toString())));
        properties.setHttpMaxConnections(Integer.parseInt(props.getProperty(HTTP_MAX_CONN_PROPERTY, DEFAULT_MAX_CONNECTIONS.toString())));
        properties.setSslEnabled(Boolean.parseBoolean(props.getProperty(SSL_ENABLED_PROPERTY, "false")));
        properties.setSslRedirect(Boolean.parseBoolean(props.getProperty(SSL_REDIRECT_PROPERTY, "false")));
        properties.setProductClassPath(props.getProperty(PRODUCT_DEV_CLASS_PATH, null));
        File file1 = new File(props.getProperty(KEYSTORE_DIR_PROPERTY, DEFAULT_SSL_KEYSTORE));
        properties.setSslKeystorePath(file1.getCanonicalPath());
        properties.setStartupWebServerWaitTime(
                Integer.parseInt(props.getProperty(STARTUP_SERVER_WAIT_TIME_PROPERTY, DEFAULT_STARTUP_SERVER_WAIT_DURATION.toString())));
        properties.setMockCoreServices(Boolean.parseBoolean(props.getProperty(MOCK_CORE_SERVICES_PROPERTY, "true")));
        properties.setSslPort(Integer.parseInt(props.getProperty(SSL_PORT_PROPERTY, DEFAULT_HTTPS_PORT.toString())));
        properties.setSslMaxConnections(Integer.parseInt(props.getProperty(SSL_MAX_CONN_PROPERTY, DEFAULT_MAX_CONNECTIONS.toString())));
        return properties;
    }

    public static ServerProperties getServerProperties() throws IOException {
        return new ServerProperties(getServerPropertiesFilePath());
    }
}
