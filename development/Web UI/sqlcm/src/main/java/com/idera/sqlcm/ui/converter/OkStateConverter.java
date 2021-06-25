package com.idera.sqlcm.ui.converter;

import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class OkStateConverter implements Converter {
    @Override
    public Object coerceToUi(Object o, Component component, BindContext bindContext) {
        if( o == null || !(o instanceof Boolean) ) return "";
        Boolean isEnabled = (Boolean)(o);
        return isEnabled ? ELFunctions.getLabel(SQLCMI18NStrings.OK) : "";
    }

    @Override
    public Object coerceToBean(Object o, Component component, BindContext bindContext) {
        return null;
    }
}
