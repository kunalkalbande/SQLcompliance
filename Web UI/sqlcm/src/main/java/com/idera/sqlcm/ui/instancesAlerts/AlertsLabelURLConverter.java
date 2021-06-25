package com.idera.sqlcm.ui.instancesAlerts;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMAlert;
import com.idera.sqlcm.ui.instancesAlerts.filters.AlertsOptionValues;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

@SuppressWarnings("rawtypes")
public class AlertsLabelURLConverter implements Converter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1, BindContext arg2) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component arg1, BindContext arg2) {
		if( obj == null || !(obj instanceof CMAlert) ) return "";

		CMAlert alert = (CMAlert) obj;
		
		return AlertsOptionValues.findLabelByKey(alert.getAlertLevel());
	}

}
