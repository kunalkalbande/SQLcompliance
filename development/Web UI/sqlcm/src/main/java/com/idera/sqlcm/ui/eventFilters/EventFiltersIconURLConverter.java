package com.idera.sqlcm.ui.eventFilters;

import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMActivityLogs;
import com.idera.sqlcm.entities.CMEventFilters;
import com.idera.sqlcm.ui.eventFilters.filters.EventFiltersViewOptionValues;

import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

@SuppressWarnings("rawtypes")
public class EventFiltersIconURLConverter implements Converter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1, BindContext arg2) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component arg1, BindContext arg2) {
		if( obj == null || !(obj instanceof CMEventFilters) ) return "";

		CMEventFilters alert = (CMEventFilters) obj;
		
		return ELFunctions.getImageURL(EventFiltersViewOptionValues
				.findImageByKey(alert.getEnabled()?1:0), "small");
	}

}
