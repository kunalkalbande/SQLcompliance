package com.idera.sqlcm.ui.logsView;

import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMActivityLogs;
import com.idera.sqlcm.ui.activityLogsView.filters.ActivityLogsViewOptionValues;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

@SuppressWarnings("rawtypes")
public class ActivityLogsIconURLConverter implements Converter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1, BindContext arg2) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component arg1, BindContext arg2) {
		if( obj == null || !(obj instanceof CMActivityLogs) ) return "";

		CMActivityLogs alert = (CMActivityLogs) obj;
		
		if(alert.getEventType().contains("Error") || alert.getEventType().contains("error")){
			return ELFunctions.getImageURL(ActivityLogsViewOptionValues.findImageByKey(3), "small");
		}
		else if(alert.getEventType().contains("Resolution") || alert.getEventType().contains("resolution")){
			return ELFunctions.getImageURL(ActivityLogsViewOptionValues.findImageByKey(2), "small");
		}
		else{
			return ELFunctions.getImageURL(ActivityLogsViewOptionValues.findImageByKey(1), "small");
		}
	}
}
