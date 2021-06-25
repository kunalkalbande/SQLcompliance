package com.idera.sqlcm.common.grid;

import com.idera.sqlcm.entities.CMEventDetails;
import com.idera.sqlcm.enumerations.EventAccessCheck;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

@SuppressWarnings("rawtypes")
public class EventPrivilegedUserConverter implements Converter {

    @Override
    public Object coerceToBean(Object arg0, Component arg1, BindContext arg2) {
        return null;
    }

    @Override
    public Object coerceToUi(Object obj, Component arg1, BindContext arg2) {
        if (obj == null || !(obj instanceof CMEventDetails)) {
            return "";
        }
        CMEventDetails event = (CMEventDetails) obj;
        return event.getPrivilegedUser()
            ? ELFunctions.getLabel(SQLCMI18NStrings.PRIVILEGED_USER_YES)
            : ELFunctions.getLabel(SQLCMI18NStrings.PRIVILEGED_USER_NO);
    }
}
