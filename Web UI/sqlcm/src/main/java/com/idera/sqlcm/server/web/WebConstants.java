package com.idera.sqlcm.server.web;

public interface WebConstants extends com.idera.server.web.WebConstants {

	int DEFAULT_PAGE_SIZE = 5;
	int CHART_X_AXIS_TICKS_COUNT = 3;
	String DISPLAY_DATE_FORMAT = "MM/dd/yyyy hh:mm:ss a";
	Object HEADER_PANEL = null;
	String INSTANCE_ALERTS_REFRESH_EVENT_QUEUE = "instance-alerts-event-queue";
	String INSTANCE_ALERTS_REFRESH_EVENT = "instance-alert-refresh-event";
	String PRM_DATABASE_ID = "dbId";
	String PRM_INSTANCE_ID = "instId";
	String PRM_INSTANCE_NAME = "instanceName";
	String PRODUCT_HELP_LINK = "https://www.idera.com/productssolutions/sqlserver/sqlcompliancemanager";
	String PRODUCT_NAME = "sqlcm";
	int REFRESH_INTERVAL = 600000;
	String DATE_FORMAT="MMM dd yyyy hh:mm a";
	String SHORT_DATE_FORMAT="MM/dd/yyyy";
	String TIME_FORMAT="hh:mm:ss a";
	String SHORT_TIME_FORMAT="hh:mm a";
	String DURATION_FORMAT="HH:mm";

	String DASHBOARD_VIEW_NAME = "dashboard";
	String INSTANCE_DETAILS_VIEW_NAME = "instanceDetails";

}
