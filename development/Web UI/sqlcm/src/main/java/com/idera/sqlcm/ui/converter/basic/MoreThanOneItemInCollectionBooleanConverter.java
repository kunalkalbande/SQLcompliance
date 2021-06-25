package com.idera.sqlcm.ui.converter.basic;

import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

import java.util.Collection;

public class MoreThanOneItemInCollectionBooleanConverter implements Converter {

    @Override
    public Object coerceToUi(Object obj, Component component, BindContext bindContext) {
        return !(obj == null || !(obj instanceof Collection<?>)) && (((Collection<?>) obj).size() > 1);
    }

    @Override
    public Object coerceToBean(Object obj, Component component, BindContext bindContext) {
        return null;
    }
}
