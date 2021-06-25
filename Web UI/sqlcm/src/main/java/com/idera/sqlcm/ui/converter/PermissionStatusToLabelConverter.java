package com.idera.sqlcm.ui.converter;

import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.PermissionsCheckStepViewModel;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class PermissionStatusToLabelConverter implements Converter {

    @Override
    public Object coerceToUi(Object obj, Component component, BindContext bindContext) {
        if (obj == null || !(obj instanceof Integer)) {
            return obj;
        }
        Integer status = (Integer)obj;
        return PermissionsCheckStepViewModel.PermissionStatus.getById(status).getLabel();
    }

    @Override
    public Object coerceToBean(Object obj, Component component, BindContext bindContext) {
        return null;
    }
}
