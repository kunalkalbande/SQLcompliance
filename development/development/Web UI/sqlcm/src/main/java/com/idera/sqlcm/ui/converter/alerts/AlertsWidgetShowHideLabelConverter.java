package com.idera.sqlcm.ui.converter.alerts;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class AlertsWidgetShowHideLabelConverter implements Converter {

    @Override
    public Object coerceToUi(Object obj, Component component, BindContext bindContext) {
        if (obj == null || !(obj instanceof Boolean)) return "";
        return ELFunctions.getLabel(((Boolean) obj) ? SQLCMI18NStrings.SHOW_ALERTS : SQLCMI18NStrings.HIDE_ALERTS);
    }

    @Override
    public Object coerceToBean(Object obj, Component component, BindContext bindContext) {
        return null;
    }
}
