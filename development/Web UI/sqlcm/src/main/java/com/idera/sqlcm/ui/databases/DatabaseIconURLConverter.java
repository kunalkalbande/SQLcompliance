package com.idera.sqlcm.ui.databases;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.CMAuditedDatabase;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.Instance;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

@SuppressWarnings("rawtypes")
public class DatabaseIconURLConverter implements Converter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1, BindContext arg2) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component arg1, BindContext arg2) {
		if( obj == null || !(obj instanceof CMDatabase) ) return "";

		CMDatabase cmAuditedDatabase = (CMDatabase) obj;

		if(cmAuditedDatabase.isEnabled()){
			return ELFunctions.getImageURL("check-green-circle", "small");
		}else{
			return ELFunctions.getImageURL("instance-error", "small");
		}
	}
}
