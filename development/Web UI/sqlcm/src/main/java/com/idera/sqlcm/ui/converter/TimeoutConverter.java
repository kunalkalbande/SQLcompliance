package com.idera.sqlcm.ui.converter;

import com.idera.sqlcm.server.web.WebConstants;
import org.apache.commons.lang.time.DurationFormatUtils;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

//use in manageUsers.zul
public class TimeoutConverter implements Converter {

    @Override
    public Object coerceToUi(Object o, Component component, BindContext bindContext) {
        if (o == null || !(o instanceof Long) || ((Long) o).intValue() == 0) {
            return "";
        }
        Long timeout = (Long) (o);

        return DurationFormatUtils.formatDuration(timeout, WebConstants.DURATION_FORMAT);
    }

    @Override
    public Object coerceToBean(Object o, Component component, BindContext bindContext) {
        return null;
    }
}
