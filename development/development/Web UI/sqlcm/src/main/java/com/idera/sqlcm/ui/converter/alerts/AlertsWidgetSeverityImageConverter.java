package com.idera.sqlcm.ui.converter.alerts;

import com.idera.sqlcm.ui.components.alerts.AlertLevel;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class AlertsWidgetSeverityImageConverter implements Converter {

    public static final String SEVERE_IMAGE = "~./sqlcm/images/close 24x24.png";
    public static final String HIGH_IMAGE = "~./sqlcm/images/warining 24x24.svg.png";
    public static final String MEDIUM_IMAGE = "~./sqlcm/images/medium-24x24.png";
    public static final String LOW_IMAGE = "~./sqlcm/images/ok 24x24.png";

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
