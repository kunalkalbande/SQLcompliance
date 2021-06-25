package com.idera.sqlcm.ui.instancesAlertsRule;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.ui.instancesAlertsRule.filters.AlertsRuleMessageOptionValues;

import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

@SuppressWarnings("rawtypes")
public class AlertsRuleMailMessageURLConverter implements Converter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1, BindContext arg2) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component arg1, BindContext arg2) {
		if( obj == null || !(obj instanceof CMAlertRules) ) return "";

		CMAlertRules alert = (CMAlertRules) obj;
		
		return AlertsRuleMessageOptionValues.findLabelByKey(alert.getEmailMessage());
	}

}
