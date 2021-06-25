package com.idera.sqlcm.ui.converter;

import com.idera.sqlcm.entities.Instance;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class PrimaryInstanceNameConverter implements Converter {
    @Override public Object coerceToUi(Object o, Component component, BindContext bindContext) {
        if( o == null || !(o instanceof Instance) ) return "";
        Instance instance = (Instance)(o);
        if (instance.isPrimary()) {
            return instance.getInstanceName() + ELFunctions.getLabel(SQLCMI18NStrings.PRIMARY_BRACKETS);
        } else {
            return instance.getInstanceName();
        }
    }

    @Override public Object coerceToBean(Object o, Component component, BindContext bindContext) {
        return null;
    }
}
