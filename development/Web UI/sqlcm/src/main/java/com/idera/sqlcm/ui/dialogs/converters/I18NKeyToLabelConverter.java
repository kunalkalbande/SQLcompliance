package com.idera.sqlcm.ui.dialogs.converters;

import com.idera.server.web.ELFunctions;
import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

public class I18NKeyToLabelConverter implements TypeConverter {

    public Object coerceToBean(Object arg0, Component arg1) {
        // TODO Auto-generated method stub
        return null;
    }

    public Object coerceToUi(Object value, Component comp) {
        if( value == null || !(value instanceof String) ) return"";
        return ELFunctions.getLabel((String) value);
    }

}
