package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.cwf.ui.dialogs.CustomMessageBox;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.entities.Instance;
import com.idera.sqlcm.entities.instances.CMRemoveInstanceResponse;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import org.zkoss.zul.Messagebox;

import java.util.Arrays;

/**
 * Created by iroman on 7/15/2015.
 */
public class RemoveInstanceDialog {

    private RemoveInstanceDialog() {
    }

    public static boolean removeInstance(CMInstance instance) {
        boolean wasRemoved = false;
        if (WebUtil.showConfirmationBoxWithIcon(SQLCMI18NStrings.MESSAGE_REMOVE_SQL_SERVER, SQLCMI18NStrings.TITLE_REMOVE_SQL_SERVER, "~./sqlcm/images/high-16x16.png", true)) {
            CustomMessageBox.UserResponse userResponse = WebUtil.showConfirmationBoxWithIconAndCancel(
                    Arrays.asList(SQLCMI18NStrings.MESSAGE_KEEP_SQL_SERVER_AUDIT_DATA_1, SQLCMI18NStrings.MESSAGE_KEEP_SQL_SERVER_AUDIT_DATA_2),
                    SQLCMI18NStrings.TITLE_KEEP_SQL_SERVER_AUDIT_DATA,"~./sqlcm/images/high-16x16.png");
            if (!userResponse.equals(CustomMessageBox.UserResponse.CANCEL)) {
                boolean deleteEventsDatabase = !userResponse.equals(CustomMessageBox.UserResponse.YES);

                try {
                    CMRemoveInstanceResponse result = InstancesFacade.removeAuditServers(Arrays.asList((instance.getId())), deleteEventsDatabase).get(0);

                    if (!result.isWasRemoved() && !result.getErrorMessage().isEmpty()) {
                        WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.CAN_NOT_REMOVE_AUDIT_SERVER, result.getErrorMessage());
                    }

                    if (!result.isWasAgentDeactivated()) {
                        WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_CAN_NOT_REMOVE_AGENT, instance.getInstance());
                    }

                    if (result.isWasRemoved()) {
                        WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_SERVER_REMOVED);
                        wasRemoved = true;
                    }
                } catch (RestException e) {
                    WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_REMOVE_AUDIT_SERVER);
                }
            }
        }
        return wasRemoved;
    }
}
