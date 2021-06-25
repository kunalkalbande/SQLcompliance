package com.idera.sqlcm.ui.converter;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

public class CheckStringConverter implements TypeConverter {
    @Override
    public Object coerceToUi(Object value, Component component) {
        if (value == null || !(value instanceof String))	return "";

        String name = (String)value;
        if( name.isEmpty()){
            return ELFunctions.getLabel(I18NStrings.NOT_SPECIFIED);
        }

        return value;
    }

    @Override
    public Object coerceToBean(Object value, Component component) {
        return null;
    }
}
