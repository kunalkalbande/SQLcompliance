package com.idera.sqlcm.ui.converter;

import com.idera.sqlcm.ui.dialogs.addserverwizard.steps.PermissionsCheckStepViewModel;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

public class PermissionStatusToImagePathConverter implements Converter {

    @Override
    public Object coerceToUi(Object obj, Component component, BindContext bindContext) {
        if (obj == null || !(obj instanceof Integer)) {
            return "";
        }
        Integer status = (Integer)obj;
        return PermissionsCheckStepViewModel.PermissionStatus.getById(status).getImageUrl();
    }

    @Override
    public Object coerceToBean(Object obj, Component component, BindContext bindContext) {
        return null;
    }
}
