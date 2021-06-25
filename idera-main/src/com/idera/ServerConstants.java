package com.idera;

import java.util.regex.Pattern;

public interface ServerConstants {

    public static final String WEB_CONFIG_FILE_NAME = "web.properties";

    public static final String LOG4J_CONFIG_FILE_NAME = "log4j.properties";
    public static final String LANG_PROPERTIES_FILE_NAME = "lang.properties";
    public static final String SERVER_PROPERTIES_FILE_NAME = "server.properties";
    public static final String LANGUAGE_FILES_FOLDER_NAME = "language-files";

    public static final String DEFAULT_LOG_DIRECTORY = "log";
    public static final String DEFAULT_LOG_FILE_NAME = "server.log";
    public static final String DEFAULT_LOG_FILE_MAX_SIZE = "5MB";
    public static final String DEFAULT_LOG_FILE_MAX_INDEX = "50";

    // Hot deploy should be disabled unless you're working on the UI.
    public static final Boolean WEB_HOT_DEPLOY_ENABLED = false;

    public static final Boolean ENABLE_CONSOLE_LOGGING = true;
    public static final Boolean VELOCITY_AUTORELOAD_ENABLED = false;

    // See: http://www.regular-expressions.info/email.html
    public static final Pattern EMAIL_ADDRESS_PATTERN = Pattern.compile(
            "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$",
            Pattern.CASE_INSENSITIVE);

    // Tag name patterns.
    // leading/trailing spaces in a tag name are ignored.
    // tag name begins with [a-zA-Z], subsequent chars can be any combo of
    // a-zA-Z0-9-_':()#!.@\s (\s: embedded space)
    // tag name cannot exceed 20 chars
    public static final String SINGLE_TAG_PATTERN = "(\\s*[a-zA-Z0-9][a-zA-Z0-9-_':()#!.@\\s]{0,19}\\s*)";

    // multiple tag names can be specified, they are separated by ;, this
    // constraint is only valid for add
    public static final String MULTI_TAG_SEPERATOR = GlobalConstants.MULTI_TAG_SEPERATOR;
    public static final String MULTI_TAG_PATTERN = GlobalConstants.MULTI_TAG_PATTERN;
    public static final String BLANK_PATTERN = GlobalConstants.BLANK_PATTERN;

    // Used to set "Unlimited" counts for things like max # of agents, add-ons,
    // etc...
    public static final int UNLIMITED_COUNT_DESIGNATOR = GlobalConstants.UNLIMITED_COUNT_DESIGNATOR;
}
