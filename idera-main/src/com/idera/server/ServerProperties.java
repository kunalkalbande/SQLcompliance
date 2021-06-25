package com.idera.server;

import java.io.*;
import java.util.Properties;

public class ServerProperties implements Serializable {
    protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(ServerProperties.class);
    protected static final org.apache.log4j.Logger debug = org.apache.log4j.Logger.getLogger(ServerProperties.class);

    static final long serialVersionUID = 1L;

    protected static final Integer DEFAULT_COM_PORT = 5443;
    protected static final String PAGE_AUTO_REFRESH = "page-auto-refresh";
    protected static final Integer DEFAULT_PAGE_AUTO_REFRESH = 3600;

    protected Integer pageAutoRefresh;

    public ServerProperties() {
        // default constructor
    }

    public ServerProperties(String propertiesFile) throws IOException {
        this(new File(propertiesFile));
    }

    public ServerProperties(File propertiesFile) throws IOException {
        this();
        this.loadFromFile(propertiesFile);
    }

    public void loadFromFile(String propertiesFile) throws IOException {
        this.loadFromFile(new File(propertiesFile));
    }

    public void loadFromFile(File propertiesFile) throws IOException {
        Properties props = new Properties();
        if (propertiesFile.exists()) {
            FileInputStream is = new FileInputStream(propertiesFile);
            try {
                props.load(is);
            } finally {
                is.close();
            }
        }
        this.pageAutoRefresh = Integer.parseInt(props.getProperty(PAGE_AUTO_REFRESH, DEFAULT_PAGE_AUTO_REFRESH.toString()));
    }

    public void saveToFile(String propertiesFile) throws IOException {
        this.saveToFile(new File(propertiesFile));
    }

    public void saveToFile(File propertiesFile) throws IOException {
        Properties props = new Properties();
        FileOutputStream os = new FileOutputStream(propertiesFile);
        try {
            props.store(os, "");
        } finally {
            os.close();
        }
        debug.debug("ServerProperties:saveToFile successful");
    }

    public Integer getPageAutoRefresh() {
        return this.pageAutoRefresh;
    }

    public void setPageAutoRefresh(Integer pageAutoRefresh) {
        this.pageAutoRefresh = pageAutoRefresh;
    }
}
