package com.idera.sqlcm.ui.dialogs.converters;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import java.util.List;

public class ListEmptyBooleanConverter implements TypeConverter {
    
	public Object coerceToBean(Object arg0, Component arg1) {
        // TODO Auto-generated method stub
        return null;
    }

    public Object coerceToUi(Object value, Component comp) {
        if( value == null || !(value instanceof List) || ((List<Object>)value) == null || ((List<Object>)value).isEmpty()){
        	return true;
        }
        return false;
    }

}
