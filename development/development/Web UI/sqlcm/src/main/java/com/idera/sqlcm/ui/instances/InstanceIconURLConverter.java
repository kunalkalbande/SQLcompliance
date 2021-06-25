package com.idera.sqlcm.ui.instances;

import com.idera.sqlcm.entities.Instance;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

import com.idera.server.web.ELFunctions;

@SuppressWarnings("rawtypes")
public class InstanceIconURLConverter implements Converter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1, BindContext arg2) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component arg1, BindContext arg2) {
		if( obj == null || !(obj instanceof Instance) ) return "";

		Instance instance = (Instance) obj;
		
		return ELFunctions.getImageURL(instance.getStatus().getIconURL(), "small");
	}

}
