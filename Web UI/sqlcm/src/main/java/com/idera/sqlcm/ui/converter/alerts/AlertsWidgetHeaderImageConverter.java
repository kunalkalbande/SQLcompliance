package com.idera.sqlcm.ui.converter.alerts;

import com.idera.sqlcm.ui.components.alerts.AlertLevel;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class AlertsWidgetHeaderImageConverter implements Converter {

    public static final String SEVERE_IMAGE = "/images/healthcheck-heart-red.png";
    public static final String HIGH_IMAGE = "/images/healthcheck-heart-yellow.png";
    public static final String MEDIUM_IMAGE = "/images/healthcheck-heart-gray.png";
    public static final String LOW_IMAGE = "/images/healthcheck-heart-green.png";

    @Override
    public Object coerceToUi(Object obj, Component component, BindContext bindContext) {
        if (obj == null || !(obj instanceof AlertLevel)) return "";
        switch ((AlertLevel) obj) {
            case SEVERE:
                return SEVERE_IMAGE;
            case HIGH:
                return HIGH_IMAGE;
            case MEDIUM:
                return MEDIUM_IMAGE;
            case LOW:
                return LOW_IMAGE;
            default:
                return LOW_IMAGE;
        }
    }

    @Override
    public Object coerceToBean(Object obj, Component component, BindContext bindContext) {
        return null;
    }
}
