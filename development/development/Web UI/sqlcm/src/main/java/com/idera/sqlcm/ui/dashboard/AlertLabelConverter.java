package com.idera.sqlcm.ui.dashboard;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.Instance;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

@SuppressWarnings("rawtypes")
public class AlertLabelConverter implements Converter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1, BindContext arg2) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component arg1, BindContext arg2) {
		if( obj == null || !(obj instanceof Instance) ) return "";

		Instance instance = (Instance) obj;

		return doConvert(instance);
	}

	public static String doConvert(Instance instance) {
		String label = "";

		if (instance.getSevereAlerts() != 0){
			label += instance.getSevereAlerts() + " " + ELFunctions.getMessage(SQLCMI18NStrings.ALERT_CRITICAL);
		}
		if (instance.getHighAlerts() != 0){
			label += instance.getHighAlerts() + " " + ELFunctions.getMessage(SQLCMI18NStrings.ALERT_WARNING);
		}
		if (instance.getMediumAlert() != 0){
			label += instance.getMediumAlert() +  " " + ELFunctions.getMessage(SQLCMI18NStrings.ALERT_INFORMATION);
		}
		if (instance.getLowAlerts() != 0){
			label += instance.getLowAlerts() + " " + ELFunctions.getMessage(SQLCMI18NStrings.ALERT_OK);
		}
		if(label.equals("")){
			label = "0";
		}

		return label;
	}

}
