package com.idera.sqlcm.ui.converter.basic;

import com.google.common.base.Strings;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class StringNotEmptyBooleanConverter implements Converter {

    @Override
    public Object coerceToUi(Object obj, Component component, BindContext bindContext) {
        return !(obj == null || !(obj instanceof String) || Strings.isNullOrEmpty((String) obj));
    }

    @Override
    public Object coerceToBean(Object obj, Component component, BindContext bindContext) {
        return null;
    }
}
