package com.idera.sqlcm.utils;

public interface SQLCMConstants {
    int DEFAULT_ROW_GRID_COUNT_INSTANCE = 50;
    int DEFAULT_ROW_GRID_COUNT = 50;
    String FILTER_VALUES_SEP = ",";
    String FILTER_VALUES_SEP_REGEXP = "\\,";
    String FILTER_RANGE_VALUES_SEP = "-";
    String FILTER_RANGE_VALUES_SEP_REGEXP = "\\-";
    String FILTER_DATE_FORMAT = "MMM dd, yyyy";
    String FILTER_TIME_FORMAT = "hh mm ss a";
    String FILTER_DATE_TIME_FORMAT = "MMM dd, yyyy hh mm ss a";

    String GET_REQUEST_PARAM_SEPARATOR = "&";
    String GET_REQUEST_PARAM_VAL_PREF = "=";

    String DATE_FILTER_REQUEST_PREFIX = "/Date(";
    String DATE_FILTER_REQUEST_SUFIX = ")/";

    String TIME_FILTER_REQUEST_PREFIX = "/Date(";
    String TIME_FILTER_REQUEST_SUFIX = ")/";

    String ACTION_ICON_SINGLE = "gray-gear-16x16";
    String ACTION_ICON_BULK = "gray-gear-bulk-16x16";

    String BASE_IMAGE_URL = "~./sqlcm/images";
    String DEFAULT_IMAGE_EXT = ".png";

    String PROD_INSTANCE_SESSION_PARAM_NAME = "currentSqlCMProduct";
    String USER_STGS_SESSION_PARAM_NAME = "SqlCMUserSetting";

    String SERVER_OPTIMIZE_AUDIT_PERFORMANCE = "http://wiki.idera.com/display/SQLCM/Reduce+audit+data+to+optimize+performance";
    String BUY_LICENSE_LINK = "https://www.idera.com/buynow/onlinestore";

    int DEFAULT_TIMEOUT_IN_MINUTES = 30;
    int DEFAULT_TIMEOUT_IN_SECONDS = DEFAULT_TIMEOUT_IN_MINUTES * 60;
    long DEFAULT_TIMEOUT_IN_MILLIS = DEFAULT_TIMEOUT_IN_SECONDS * 1000;

    int SORT_ASCENDING = 0;
    int SORT_DESCENDING = 1;
    int DEFAULT_PAGE = 1;
    String DEFAULT_ALERTS_SORT_COLUMN = "alertTime";
    String DEFAULT_EVENTS_SORT_COLUMN = "Time";
}
