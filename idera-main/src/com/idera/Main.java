package com.idera;

import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.core.util.GlobalUtil;
import com.idera.core.util.TimeLogger;
import com.idera.cwf.model.Product;
import com.idera.i18n.I18NStrings;
import com.idera.i18n.I18NUtil;
import com.idera.i18n.LangProperties;
import com.idera.server.ServerException;
import com.idera.server.ServerFacade;
import com.idera.server.StartUpWebServer;
import com.sun.management.OperatingSystemMXBean;
import mazz.i18n.Logger;
import mazz.i18n.LoggerFactory;
import mazz.i18n.LoggerFactory.LoggerType;
import mazz.i18n.Msg;
import mazz.i18n.exception.LocalizedException;
import org.apache.log4j.ConsoleAppender;
import org.apache.log4j.PatternLayout;
import org.apache.log4j.PropertyConfigurator;
import org.apache.log4j.RollingFileAppender;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import util.ClassPathUtil;
import util.NonLocalizedByteSizeFormatter;

import java.io.*;
import java.lang.management.ManagementFactory;
import java.net.InetAddress;
import java.net.Socket;
import java.security.Security;
import java.util.*;

public class Main extends MonitorProtocol {

    public static final int DEFAULT_STOP_PORT = 9094;

    // uncomment this and other reference to embedded database[derby] to enable
    // it
    // private static final DerbyWrapper derbyWrapper =
    // DerbyWrapper.getInstance();
    private static final TomcatWrapper tomcatWrapper = TomcatWrapper.getInstance();

    private static final List<Wrapper> wrappers = new ArrayList<Wrapper>();

    protected static mazz.i18n.Logger logger;
    protected static org.apache.log4j.Logger debug;

    public static final String ADMIN_USERNAME_PROPERTY = "admin.user";
    public static final String ADMIN_PASSWORD_PROPERTY = "admin.pass";

    public static final String LOG_CONSOLE_APPENDER_BASE_NAME = "stdout";
    public static final String LOG_CONSOLE_APPENDER_NAME = "log4j.appender." + LOG_CONSOLE_APPENDER_BASE_NAME;
    public static final String LOG_CONSOLE_LAYOUT_PROPERTY = LOG_CONSOLE_APPENDER_NAME + ".layout";
    public static final String LOG_CONSOLE_PATTERN_PROPERTY = LOG_CONSOLE_LAYOUT_PROPERTY + ".ConversionPattern";
    public static final String LOG_CONSOLE_OUTPUT_PATTERN = "%d %-5p %x - %m%n";

    public static final String LOG_FILE_APPENDER_BASE_NAME = "LOGFILE";
    public static final String LOG_FILE_APPENDER_NAME = "log4j.appender." + LOG_FILE_APPENDER_BASE_NAME;
    public static final String LOG_FILE_NAME_PROPERTY = LOG_FILE_APPENDER_NAME + ".File";
    public static final String LOG_FILE_MAX_SIZE_PROPERTY = LOG_FILE_APPENDER_NAME + ".MaxFileSize";
    public static final String LOG_FILE_COUNT_PROPERTY = LOG_FILE_APPENDER_NAME + ".MaxBackupIndex";
    public static final String LOG_FILE_LAYOUT_PROPERTY = LOG_FILE_APPENDER_NAME + ".layout";
    public static final String LOG_FILE_PATTERN_PROPERTY = LOG_FILE_LAYOUT_PROPERTY + ".ConversionPattern";
    public static final String LOG_FILE_OUTPUT_PATTERN = "%d %-5p %x - %m%n";

    public static final String LOG_CONF_SIZE_PROPERTY = "log.max-size";
    public static final String LOG_CONF_COUNT_PROPERTY = "log.max-count";
    public static final String PLUGIN_PATH = "lib/";

    public static final String START_COMMAND = "start";
    public static final String STOP_COMMAND = "stop";
    public static final String ZK_WEB_WAR = "zk-web.war";

    public static final Integer WAIT_TIME_BETWEEN_RETRY = 5000;
    public static final Integer STOP_COMMAND_SOCKET_TIMEOUT = 1000;

    public static final Integer NUMBER_OF_RETRIES = 20;

    private static Main instance = null;

    @Override
    public void shutdown() {}

    public static boolean restart() {
        try {
            TimeLogger timeLogger = new TimeLogger("restart method in Main");
            timeLogger.enter();
            List<String> dependencies = getInstalledProducts();
            tomcatWrapper.restart(dependencies);
            timeLogger.exit();
            I18NUtil.invalidateCache();
            return true;
        } catch (IOException e) {
            logger.error("Unable to get the installed products from Core REST services");
        } catch (ShutdownException e) {
            logger.error("Unable to stop the running tomcat instance");
        } catch (StartupException e) {
            logger.error("Unable to restart the tomcat instance");
        } catch (Exception e) {
            logger.error("Unable to restart the tomcat instance");
        }
        return false;
    }

    public static void main(String[] args) throws Exception {
        // Initialize logging.
        try {
            // Force to always run in English
            Locale.setDefault(Locale.ENGLISH);
            // set configuration folder.
            boolean result = GlobalUtil.setConfigDir();
            // Initialize the log4j subsystem
            try {
                initLog4J();
            } catch (LocalizedException e) {
                throw new StartupException(e, I18NStrings.COULD_NOT_INITIALIZE_LOGGING);
            }
            if (debug.isDebugEnabled()) {
                debug.debug("set configuration Directory = " + result);
                debug.debug("configuration Directory = " + GlobalUtil.configDir);
            }
        } catch (Exception ex) {
            logger.error(ex, I18NStrings.ERROR_STARTING_UP_SERVER);
        }
        // Get input command.
        String cmd = START_COMMAND; // set default val to start command so it
        // runs in Eclipse
        if (args.length > 0) {
            cmd = args[0];
        }
        // Process based on the option.
        if (START_COMMAND.equalsIgnoreCase(cmd)) {
            logger.info(I18NStrings.WEB_SERVICE_STARTING);
            startWebService();
        } else if (STOP_COMMAND.equalsIgnoreCase(cmd)) {
            stopWebService();
        } else {
            logger.error(I18NStrings.WEB_SERVICE_UNKNOWN_COMMAND);
        }
    }

    public static void startWebService() {
        try {
            // Try to set the timezone for the server if it's configured
            try {
                LangProperties langProps = ServerFacade.getLangProperties();
                // Set the server's timezone if the user has defined it
                if (langProps.getDefaultTimezone() != null) {
                    TimeZone.setDefault(langProps.getDefaultTimezone());
                }
                if (langProps.getDefaultServerLocale() != null) {
                    StartUpWebServer.getInstance().setServerLocale(langProps.getDefaultServerLocale());
                }
            } catch (ServerException ex) {
                // If we can't read the lang.properties file, warn, but try to
                // start-up anyway
                logger.warn(ex, ex.getMsgKey(), (Object[]) ex.getVarArgs());
                logger.warn(I18NStrings.THE_SERVER_WILL_RUN_IN_THE_TIMEZONE_PROVIDED_BY_THE_OS);
            }
            try {
                StartUpWebServer.getInstance().startup();
            } catch (Throwable ex) {
                logger.warn(ex, I18NStrings.FAILED_TO_START_WEB_SERVER_TO_DISPLAY_LOADING_SCREEN);
            }
            // Perform configuration that needs to happen before the service
            // wrappers start
            debug.debug("Main: calling configure()");
            configure();
            List<String> dependencies = getInstalledProducts();
            // Log our system details (arch, OS, memory, etc...)
            debug.debug("Main: calling logSystemDetails()");
            logSystemDetails();
            // Start the service wrappers, they throw StartupException if they
            // fail
            for (Wrapper wrapper : wrappers) {
                logger.info(I18NStrings.WRAPPER_STARTING, wrapper.getName());
                wrapper.start(dependencies);
                logger.info(I18NStrings.WRAPPER_STARTED, wrapper.getName());
            }
            logger.info(I18NStrings.STARTED_CWF_SERVER, ServerVersion.SERVER_LONG_VERSION_STR);
            try {
                // Start the monitor that lets us receive shutdown signals
                WebServerProperties webProperties = ServerFacade.getWebServerProperties();
                logger.info(I18NStrings.STARTING_MONITOR_SERVICE, webProperties.getMonitorPort());
                int port = Integer.getInteger("buserver.stop.port", webProperties.getMonitorPort()).intValue();
                Monitor monitor = Monitor.monitor(instance, port);
                monitor.start();
                monitor.join();
                logger.info(I18NStrings.MONITOR_SERVICE_STARTED);
            } catch (InterruptedException ex) {
                logger.error(I18NStrings.MAIN_SERVER_INTERRUPTED, ex);
            } catch (Exception e) {
                logger.error(I18NStrings.MAIN_SERVER_INTERRUPTED, e);
                e.printStackTrace();
            }
        } catch (StartupException ex) {
            logger.error(ex, I18NStrings.ERROR_STARTING_UP_SERVER);
        } catch (Throwable t) {
            t.printStackTrace();
        } finally {
            logger.info(I18NStrings.SHUTTING_DOWN_SERVER);
            // Shutdown the wrappers in the reverse order of start-up
            Collections.reverse(wrappers);
            for (Wrapper wrapper : wrappers) {
                if (wrapper.isStarted()) {
                    try {
                        wrapper.shutdown();
                        logger.info(I18NStrings.WRAPPER_SHUT_DOWN, wrapper.getName());
                    } catch (ShutdownException ex) {
                        logger.error(ex, I18NStrings.ERROR_SHUTTING_DOWN_WRAPPER, wrapper.getName());
                    }
                }
            }
            System.exit(0);
        }
    }

    public static void stopWebService() throws Exception {
        WebServerProperties webProperties = ServerFacade.getWebServerProperties();
        logger.info(I18NStrings.WRAPPER_SHUT_DOWN, "Sending stop web application server request.");
        int port = Integer.getInteger("buserver.stop.port", webProperties.getMonitorPort()).intValue();
        Socket socket = null;
        try {
            socket = new Socket(InetAddress.getByName("127.0.0.1"), port);
            socket.setSoTimeout(STOP_COMMAND_SOCKET_TIMEOUT);
            DataOutputStream out = new DataOutputStream(socket.getOutputStream());
            out.write(Monitor.STOP_COMMAND.getBytes());
        } catch (Exception e) {
            logger.error(e, I18NStrings.ERROR_SEND_STOP_WEB_SERVER);
        } finally {
            if (socket != null) {
                try {
                    socket.close();
                } catch (Exception e) {
                    logger.error(e, I18NStrings.ERROR_CLOSE_STOP_WEB_SERVER_SOCKET);
                }
            }
        }
    }

    private static void addLanguageFiles() throws StartupException {
        try {
            File languageFilesDir = new File(ServerFacade.getLanguageFilesPath());
            if (languageFilesDir == null || !languageFilesDir.exists() || !languageFilesDir.isDirectory()) {
                logger.warn(I18NStrings.LANGUAGE_FILES_PATH_NOT_FOUND, languageFilesDir.getAbsolutePath());
                return;
            }
            ClassPathUtil.addFile(languageFilesDir);
        } catch (Exception e) {
            logger.error(e, I18NStrings.ERROR_LOADING_LANGUAGE_FILES);
            logger.warn(I18NStrings.ONLY_ENGLISH_WILL_BE_AVAILABLE);
        }
    }

    private static void configure() throws StartupException, IOException {
        // Add any language packs to the classpath
        addLanguageFiles();
        // Initialize the i18nlog default message bundle
        Msg.setBundleBaseNameDefault(GlobalConstants.DEFAULT_MESSAGE_BUNDLE_BASE_NAME);
        Logger.setDumpLogKeys(false);
        Logger.setDumpStackTraces(true);
        LoggerFactory.resetLoggerType(LoggerType.LOG4J);
        logger.info(I18NStrings.STARTING_CWF_SERVER, ServerVersion.SERVER_LONG_VERSION_STR);
        // Add the Bouncy Castle provider to the Security context
        Security.addProvider(new BouncyCastleProvider());
        // add all the database JAR files
        // addDatabaseJars();
        // make sure we are in headless mode for any graphics rendered
        System.setProperty("java.awt.headless", "true");
        WebServerProperties webServerProperties = null;
        try {
            webServerProperties = ServerFacade.getWebServerProperties();
        } catch (Exception e) {
            throw new StartupException(e, I18NStrings.UI_WEB_SERVER_PROPERTIES_NOT_FOUND);
        }
        String uiWebAppName = System.getProperty("ui.webapp", ZK_WEB_WAR);
        tomcatWrapper.setUIWebAppName(uiWebAppName);
        tomcatWrapper.setWebServerProperties(webServerProperties);
        // Add the wrappers in the order they need to start-up (shutdown happens
        // in reverse order)

        /*
         * add hibernate if you want to use embedded database the current
         * configured database is derby. wrappers.add(derbyWrapper);
         */
        wrappers.add(tomcatWrapper);
    }

    private static void cleanInstalledProducts() throws IOException {
        // Deletes all the Jar files from the products plugin directory.
        GlobalUtil.deleteFilesFromDirectory(PLUGIN_PATH);
    }

    private static List<String> getInstalledProducts() throws IOException {
        // Looks for all the products installed, copies their dependencies and
        // bring them into CLASSPATH.
        WebServerProperties webProperties = ServerFacade.getWebServerProperties();
        // TODO: System properties should not be used for module communication.
        List<String> dependencies = new ArrayList<String>();
        if (webProperties.getProductClassPath() != null) {
            dependencies.add(webProperties.getProductClassPath());
        } else {
            List<Product> installedProducts = getProducts();
            for (Product product : installedProducts) {
                String path = product.getPackageURI();
                logger.info(I18NStrings.ADDING_DEPENDENCY, path);
                dependencies.add(path);
            }
        }
        return dependencies;
    }

    private static List<Product> getProducts() {
        List<Product> products = null;
        int attempts = 1;
        while (products == null && attempts <= NUMBER_OF_RETRIES) {
            try {
                products = CoreRestClient.getInstance().getProducts();
                attempts++;
                break;
            } catch (RestException exception) {
                logger.info("Error while waiting on REST Service on " + attempts + " attempt");
                sleep();
            }
        }
        return products;
    }

    private static void sleep() {
        try {
            Thread.sleep(WAIT_TIME_BETWEEN_RETRY);
        } catch (InterruptedException e) {
            logger.warn(e, "Thread was interrupted.");
        }
    }

    private static void initLog4J() throws LocalizedException {
        String configurationDirectory = System.getProperty(GlobalConstants.CONFIG_DIR_PROPERTY);
        if (configurationDirectory == null) {
            configurationDirectory = GlobalConstants.DEFAULT_CONFIG_DIR;
        }
        // Initialize log4j from the properties file
        String log4jPropertiesFile = configurationDirectory + "/" + ServerConstants.LOG4J_CONFIG_FILE_NAME;
        Properties log4jProperties = new Properties();
        FileInputStream in = null;
        try {
            in = new FileInputStream(log4jPropertiesFile);
            log4jProperties.load(in);
        } catch (FileNotFoundException e) {
            throw new LocalizedException(e, I18NStrings.LOG4J_PROPERTIES_FILE_NOT_FOUND);
        } catch (IOException e) {
            throw new LocalizedException(e, I18NStrings.COULD_NOT_READ_LOG4J_PROPERTIES);
        } finally {
            if (in != null) {
                try {
                    in.close();
                } catch (IOException ex) {
                    logger.warn(I18NStrings.FAILED_TO_PROPERLY_CLOSE_PROPERTIES_FILE);
                }
            }
        }
        String rootLoggerConfig = "INFO";
        // Initialize the base properties for the stdout appender (if enabled)
        if (ServerConstants.ENABLE_CONSOLE_LOGGING == true) {
            log4jProperties.setProperty(LOG_CONSOLE_APPENDER_NAME, ConsoleAppender.class.getName());
            log4jProperties.setProperty(LOG_CONSOLE_LAYOUT_PROPERTY, PatternLayout.class.getName());
            log4jProperties.setProperty(LOG_CONSOLE_PATTERN_PROPERTY, LOG_CONSOLE_OUTPUT_PATTERN);
            rootLoggerConfig = rootLoggerConfig + "," + LOG_CONSOLE_APPENDER_BASE_NAME;
        }
        // Initialize the base properties for the LOGFILE appender
        log4jProperties.setProperty(LOG_FILE_APPENDER_NAME, RollingFileAppender.class.getName());
        log4jProperties.setProperty(LOG_FILE_LAYOUT_PROPERTY, PatternLayout.class.getName());
        log4jProperties.setProperty(LOG_FILE_PATTERN_PROPERTY, LOG_FILE_OUTPUT_PATTERN);
        rootLoggerConfig = rootLoggerConfig + "," + LOG_FILE_APPENDER_BASE_NAME;
        // Set Root Logger Property
        log4jProperties.setProperty("log4j.rootLogger", rootLoggerConfig);
        String logDirectory = System.getProperty(GlobalConstants.LOG_DIR_PROPERTY);
        if (logDirectory == null) {
            logDirectory = ServerConstants.DEFAULT_LOG_DIRECTORY;
        }
        File logDirectoryFile = new File(logDirectory);
        if (!logDirectoryFile.exists()) {
            new File(logDirectory).mkdirs();
//            throw new LocalizedException(I18NStrings.SPECIFIED_PATH_FOR_LOGS_DOES_NOT_EXIST);
        } else if (logDirectoryFile.exists() && !logDirectoryFile.isDirectory()) {
            throw new LocalizedException(I18NStrings.SPECIFIED_PATH_FOR_LOGS_IS_NOT_A_DIRECTORY);
        }
        String logFileName = logDirectory + "/" + ServerConstants.DEFAULT_LOG_FILE_NAME;
        log4jProperties.setProperty(LOG_FILE_NAME_PROPERTY, logFileName);
        String maxLogSize = System.getProperty(LOG_CONF_SIZE_PROPERTY);
        if (maxLogSize == null) {
            maxLogSize = ServerConstants.DEFAULT_LOG_FILE_MAX_SIZE;
        }
        log4jProperties.setProperty(LOG_FILE_MAX_SIZE_PROPERTY, maxLogSize);
        String maxLogCount = System.getProperty(LOG_CONF_COUNT_PROPERTY);
        if (maxLogCount == null) {
            maxLogCount = ServerConstants.DEFAULT_LOG_FILE_MAX_INDEX;
        }
        log4jProperties.setProperty(LOG_FILE_COUNT_PROPERTY, maxLogCount);
        PropertyConfigurator.configure(log4jProperties);
        logger = mazz.i18n.LoggerFactory.getLogger(Main.class);
        debug = org.apache.log4j.Logger.getLogger(Main.class);
    }

    private static void logSystemDetails() {
        logger.info(I18NStrings.OPERATING_SYSTEM, System.getProperty("os.name"));
        logger.info(I18NStrings.ARCHITECTURE, System.getProperty("os.arch"));
        logger.info(I18NStrings.OS_VERSION_VALUE, System.getProperty("os.version"));
        Runtime runtime = Runtime.getRuntime();
        logger.info(I18NStrings.PROCESSORS_DETECTED_VALUE, Integer.toString(runtime.availableProcessors()));
        logger.info(I18NStrings.MAX_CONFIGURED_HEAP_MEMORY_VALUE, NonLocalizedByteSizeFormatter.getFriendlyString(runtime.maxMemory()));
        OperatingSystemMXBean osMXBean = (OperatingSystemMXBean) ManagementFactory.getOperatingSystemMXBean();
        logger.info(I18NStrings.TOTAL_PHYSICAL_MEMORY_VALUE, NonLocalizedByteSizeFormatter.getFriendlyString(osMXBean.getTotalPhysicalMemorySize()));
        logger.info(I18NStrings.FREE_PHYSICAL_MEMORY_VALUE, NonLocalizedByteSizeFormatter.getFriendlyString(osMXBean.getFreePhysicalMemorySize()));
    }
}
