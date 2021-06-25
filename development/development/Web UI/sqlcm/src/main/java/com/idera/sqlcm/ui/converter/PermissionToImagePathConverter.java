package com.idera.sqlcm.ui.converter;

import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.entities.CMInstanceRole;
import com.idera.sqlcm.entities.CMInstanceUser;
import com.idera.sqlcm.server.web.ELFunctions;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class PermissionToImagePathConverter implements Converter {

    private static final String ROLE_ICON = "key-icon";
    private static final String LOGIN_ICON = "UserDefinedPermission16x16";
    private static final String EMPTY_STRING = "";

    private static final Logger logger = Logger.getLogger(PermissionToImagePathConverter.class);

    @Override
    public Object coerceToUi(Object obj, Component component, BindContext bindContext) {
        if (obj == null || !(obj instanceof CMInstancePermissionBase)) {
            return EMPTY_STRING;
        }
        CMInstancePermissionBase permission = (CMInstancePermissionBase)obj;

        String imageUrl;
        if (permission instanceof CMInstanceRole) {
            imageUrl = ELFunctions.getImageURLWithoutSize(ROLE_ICON);
        } else if (permission instanceof CMInstanceUser) {
            imageUrl = ELFunctions.getImageURLWithoutSize(LOGIN_ICON);
        } else {
            logger.error(" Unable to get image for uknown type " + permission);
            return EMPTY_STRING;
        }
        return imageUrl;
    }

    @Override
    public Object coerceToBean(Object obj, Component component, BindContext bindContext) {
        return null;
    }
}
