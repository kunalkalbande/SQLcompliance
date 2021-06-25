package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMAvailabilityInfo;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.addserverwizard.WizardMode;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.zul.ListModelList;

import java.util.List;

public class AlwaysOnAvailabilityGroupStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/always-on-availability-group-step.zul";

    private static final Logger logger = Logger.getLogger(SelectDatabasesStepViewModel.class);

    private ListModelList<CMAvailabilityInfo> availabilityList;

    public AlwaysOnAvailabilityGroupStepViewModel() {
        super();
    }

    public ListModelList<CMAvailabilityInfo> getAvailabilityList() {
        return availabilityList;
    }

    private void loadList() {
        try {
            List<CMDatabase> databaseList = getParentWizardViewModel().getWizardEntity().getServerConfigEntity().getDatabaseList();

            if (databaseList == null || databaseList.size() == 0) {
                logger.info(" database list is null or empty ");
                return;
            }

            List<CMAvailabilityInfo> list = DatabasesFacade.getAvailabilityInfoList(databaseList);
            availabilityList = new ListModelList<>(list);
            BindUtils.postNotifyChange(null, null, this, "availabilityList");
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_AVAILABILITY_INFO_LIST);
        }
    }

    @Override
    public String getNextStepZul() {
        return AuditCollectionLevelStepViewModel.ZUL_PATH;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_ALWAYS_ON_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "";
    }

    @Override
    protected void doOnShow(AddServerWizardEntity wizardEntity) {
        loadList();
        if (availabilityList == null || availabilityList.size() == 0) {
            getParentWizardViewModel().goNext(true); // go next and remove from stack this step because we don't need forward user to this step
            // when user clicked prev button on next step.
        }
    }

}