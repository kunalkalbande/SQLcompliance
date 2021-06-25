package com.idera.sqlcm.ui.converter;

import com.idera.sqlcm.enumerations.CmLicenseState;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class CmLicenseStateConverter implements Converter {
    @Override
    public Object coerceToUi(Object o, Component component, BindContext bindContext) {
        if( o == null || !(o instanceof Integer) ) return "";
        Integer index = (Integer)(o);
        return CmLicenseState.getByIndex(index).getLabel();
    }

    @Override
    public Object coerceToBean(Object o, Component component, BindContext bindContext) {
        return null;
    }
}
