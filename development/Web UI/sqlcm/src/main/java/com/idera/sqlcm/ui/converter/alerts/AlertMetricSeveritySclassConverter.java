package com.idera.sqlcm.ui.converter.alerts;

import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class AlertMetricSeveritySclassConverter implements Converter {

    public static final String CRITICAL_SCLASS = "action-item-severity-critical";
    public static final String WARNING_SCLASS = "action-item-severity-warning";
    public static final String INFORMATIONAL_SCLASS = "action-item-severity-informational";

    @Override
    public Object coerceToUi(Object obj, Component component, BindContext bindContext) {
        if (obj == null || !(obj instanceof Integer)) return "";
        switch ((Integer) obj) {
            case 1:
                return INFORMATIONAL_SCLASS;
            case 2:
                return WARNING_SCLASS;
            case 3:
                return CRITICAL_SCLASS;
            default:
                return "";
        }
    }

    @Override
    public Object coerceToBean(Object obj, Component component, BindContext bindContext) {
        return null;
    }
}
