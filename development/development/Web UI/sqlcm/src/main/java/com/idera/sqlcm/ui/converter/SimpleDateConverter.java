package com.idera.sqlcm.ui.converter;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

import java.util.Date;


@SuppressWarnings("rawtypes")
public class SimpleDateConverter implements Converter {

    private String nullDateLabelName = SQLCMI18NStrings.NONE;

    public SimpleDateConverter() {
    }

    public SimpleDateConverter(String nullDateLabelName) {
        if (nullDateLabelName != null) {
            this.nullDateLabelName = nullDateLabelName;
        }
    }

	@Override
    public Object coerceToUi(Object val, Component comp, BindContext ctx) {
		if(val == null){
			return ELFunctions.getLabel(nullDateLabelName);
		}
		return Utils.getFormatedDate((Date) val);
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
			return ELFunctions.getLabel(nullDateLabelName);
		}
        return Utils.parseDate((String) val);
    }
}