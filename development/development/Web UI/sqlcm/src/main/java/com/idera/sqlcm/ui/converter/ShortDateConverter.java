package com.idera.sqlcm.ui.converter;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

import java.util.Date;

@SuppressWarnings("rawtypes")
public class ShortDateConverter implements Converter {
    /**
     * Convert Date to String (MM/dd/yyyy).
     * @param val date to be converted
     * @param comp associated component
     * @param ctx bind context for associate Binding and extra parameter (e.g. format)
     * @return the converted String
     */
	@Override
    public Object coerceToUi(Object val, Component comp, BindContext ctx) {
		if(val == null){
			return ELFunctions.getLabel(SQLCMI18NStrings.NONE);
		}
		return Utils.getFomatedShortDate((Date) val);
    }
     
	/**
     * Convert String to Date.
     * @param val date in string form
     * @param comp associated component
     * @param ctx bind context for associate Binding and extra parameter (e.g. format)
     * @return the converted Date
     */
	@Override
    public Object coerceToBean(Object val, Component comp, BindContext ctx) {
		if(val == null){
			return ELFunctions.getLabel(SQLCMI18NStrings.NONE);
		}
        return Utils.parseDate((String) val);
    }
}