package com.idera.sqlcm.ui.converter;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

import java.util.Date;

@SuppressWarnings("rawtypes")
public class DateToNeverConverter implements Converter {

	@Override
    public Object coerceToUi(Object val, Component comp, BindContext ctx) {
		if(val == null){
			return ELFunctions.getLabel(SQLCMI18NStrings.NEVER);
		}
		return val;
    }
	
	@Override
    public Object coerceToBean(Object val, Component comp, BindContext ctx) {
		if(val == null){
			return ELFunctions.getLabel(SQLCMI18NStrings.NEVER);
		}
        return val;
    }
}