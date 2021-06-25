package com.idera.sqlcm.ui.converter.alerts;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class AlertRefreshLabelConverter implements Converter {

    @Override
    public Object coerceToUi(Object obj, Component component, BindContext bindContext) {
        if (obj == null || !(obj instanceof Boolean)) return "";
        return ELFunctions.getLabel((Boolean) obj ? I18NStrings.REFRESHING : I18NStrings.REFRESH);
    }

    @Override
    public Object coerceToBean(Object obj, Component component, BindContext bindContext) {
        return null;
    }
}
