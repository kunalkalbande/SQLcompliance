package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMPermission;
import com.idera.sqlcm.entities.CMPermissionInfo;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.converter.PermissionStatusToCssStyleConverter;
import com.idera.sqlcm.ui.converter.PermissionStatusToImagePathConverter;
import com.idera.sqlcm.ui.converter.PermissionStatusToLabelConverter;
import com.idera.sqlcm.entities.addserverwizard.ServerConfigEntity;
import com.idera.sqlcm.ui.dialogs.addserverwizard.permissionFailDialog.PermissionFailConfirmViewModel;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.ListModelList;

import java.util.ArrayList;
import java.util.List;

public class PermissionsCheckStepViewModel extends AddWizardStepBase {

    private static final String CSS_STYLE_PASSED = "color: green;";
    private static final String CSS_STYLE_FAILED = "color: red;";

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/permissions-check-step.zul";
    public static final int INVALID_CHECKS_VALUE = -1;

    public PermissionsCheckStepViewModel() {
        super();
    }

    private Converter permissionStatusToImagePathConverter = new PermissionStatusToImagePathConverter();

    private Converter permissionStatusToLabelConverter = new PermissionStatusToLabelConverter();

    private Converter permissionStatusToCssStyleConverter = new PermissionStatusToCssStyleConverter();

    private CMPermissionInfo permissionInfo;

    private ListModelList<CMPermission> permissionList;

    public Converter getPermissionStatusToImagePathConverter() {
        return permissionStatusToImagePathConverter;
    }

    public Converter getPermissionStatusToLabelConverter() {
        return permissionStatusToLabelConverter;
    }

    public Converter getPermissionStatusToCssStyleConverter() {
        return permissionStatusToCssStyleConverter;
    }

    public ListModelList<CMPermission> getPermissionList() {
        return permissionList;
    }

    public enum PermissionStatus {
        PASSED(0,
               ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_PERMISSION_PASSED),
               CSS_STYLE_PASSED,
                com.idera.server.web.ELFunctions.getImageURL("check-green-circle", ELFunctions.IconSize.SMALL.getStringValue())),

        FAILED(1,
               ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_PERMISSION_FAILED),
               CSS_STYLE_FAILED,
                com.idera.server.web.ELFunctions.getImageURL("instance-error", ELFunctions.IconSize.SMALL.getStringValue())),

        UNKNOWN(-1,
                ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_PERMISSION_UNKNOWN),
                CSS_STYLE_FAILED,
                com.idera.server.web.ELFunctions.getImageURL("instance-unknown", ELFunctions.IconSize.SMALL.getStringValue()));

        private int id;
        private String label;
        private String labelCss;
        private String imageUrl;

        PermissionStatus(int id, String label, String labelCss, String imageUrl) {
            this.id = id;
            this.label = label;
            this.labelCss = labelCss;
            this.imageUrl = imageUrl;
        }

        public int getId() {
            return id;
        }

        public String getLabel() {
            return label;
        }

        public String getLabelCss() {
            return labelCss;
        }

        public String getImageUrl() {
            return imageUrl;
        }

        public static PermissionStatus getById(long id) {
            for(PermissionStatus e : values()) {
                if(e.id == id) {
                    return e;
                }
            }
            return UNKNOWN;
        }
    }

    @Override
    public void onDoAfterWire() {
    }

    @Override
    public void doOnShow(AddServerWizardEntity wizardEntity) {
        if (getInstance() == null) {
            throw new IllegalStateException(" Instance must not be null! ");
        }

        if (permissionInfo == null) {
            loadPermissionsInfo();
        }
    }

    private void loadPermissionsInfo() {
        try {
            permissionInfo = InstancesFacade.getPermissionInfo(getInstance().getId());
            permissionList = new ListModelList<>(permissionInfo.getPermissionsCheckList());
            if (permissionInfo.getFailedChecks() > 0) {
                List<CMPermission> failedPermissions = extractFailedPermissions(permissionList);
                PermissionFailConfirmViewModel.show(getInstance(), failedPermissions,
                        new PermissionFailConfirmDialogListenerImpl());
                //SCM-583 
                getNextButton().setDisabled(true);
                
            }
            else if (permissionInfo.getFailedChecks() <= 0)
            {
            	getNextButton().setDisabled(false);
            	
            }
            BindUtils.postNotifyChange(null, null, PermissionsCheckStepViewModel.this, "permissionList");
            BindUtils.postNotifyChange(null, null, PermissionsCheckStepViewModel.this, "operationInfo");
        } catch (RestException e) {
            getParentWizardViewModel().close();
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_PERMISSIONS_INFO);
        }
    }

    private List<CMPermission> extractFailedPermissions(ListModelList<CMPermission> permissionList) {
        List<CMPermission> failedPermissions = new ArrayList<>(permissionList.size());
        for (CMPermission p : permissionList) {
            if (p.getStatus() == PermissionStatus.FAILED.getId()) {
                failedPermissions.add(p);
            }
        }
        return failedPermissions;
    }

    @Override
    public String getNextStepZul() {
        return SummaryStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_PERMISSION_CHECK_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+Permissions+Check+window";
    }

    public String getOperationInfo() {
        int totalChecks  = INVALID_CHECKS_VALUE;
        int passedChecks = INVALID_CHECKS_VALUE;
        int failedChecks = INVALID_CHECKS_VALUE;
        if (permissionInfo != null) {
            totalChecks  = permissionInfo.getTotalChecks();
            passedChecks = permissionInfo.getPassedChecks();
            failedChecks = permissionInfo.getFailedChecks();
        }
        return ELFunctions.getLabelWithParams(
                SQLCMI18NStrings.ADD_SERVER_WIZARD_PERMISSION_CHECK_OPERATION_INFO, totalChecks, passedChecks, failedChecks);
    }

    public class PermissionFailConfirmDialogListenerImpl
            implements PermissionFailConfirmViewModel.PermissionFailConfirmDialogListener {

        @Override
        public void onIgnore() {
            getNextButton().setDisabled(false);
        }

        @Override
        public void onReCheck() {
            // do nothing
        }

    }

    @Command("reCheckClick")
    public void reCheckClick() {
        loadPermissionsInfo();
    }


}
