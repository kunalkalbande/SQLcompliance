package com.idera.sqlcm.ui.converter;

import java.util.List;

import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

public class ListEmptyBooleanConverter implements Converter {

    @Override
    public Object coerceToUi(Object value, Component component, BindContext bindContext) {
        if( value == null || !(value instanceof List) || ((List<Object>)value) == null || ((List<Object>)value).isEmpty()) return true;

        return false;
    }

    @Override
    public Object coerceToBean(Object o, Component component, BindContext bindContext) {
        return null;
    }
}
