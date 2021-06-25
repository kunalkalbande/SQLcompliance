package com.idera.sqlcm.ui.converter;

import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.server.web.WebUtil;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

public class LinkHrefConverter implements Converter {

    @Override
    public Object coerceToUi(Object obj, Component component, BindContext bindContext) {
        if (obj == null || !(obj instanceof CMInstance)) {
            return "";
        }

        CMInstance instance = (CMInstance) obj;

        return WebUtil.buildPathRelativeToCurrentProduct("instanceView?instId=" +instance.getId());
    }

    @Override
    public Object coerceToBean(Object obj, Component component, BindContext bindContext) {
        return null;
    }
}
