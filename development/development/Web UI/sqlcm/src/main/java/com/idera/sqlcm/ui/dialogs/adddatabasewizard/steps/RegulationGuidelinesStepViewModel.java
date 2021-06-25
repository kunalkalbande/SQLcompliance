package com.idera.sqlcm.ui.dialogs.adddatabasewizard.steps;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.entities.RegulationType;
import com.idera.sqlcm.facade.RegulationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.RegulationSettings;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;

import java.util.HashMap;
import java.util.Map;

public class RegulationGuidelinesStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/adddatabasewizard/steps/regulation-guidelines-step.zul";
    public static final String NEW_LINE = "\r\n";

    public static final long PCI_DSS_TYPE = 1;
    public static final long HIPAA_TYPE = 2;

    private ListModelList<RegulationType> regulationTypeListModelList;

    private String regulationGuidelinesDesc;

    private Map<Long, Long> checkedTypes;

    public RegulationGuidelinesStepViewModel() {
        super();
    }

    @Override
    public String getNextStepZul() {
        return RegulationGuidelinesApplyStepViewModel.ZUL_PATH;
    }

    @Override
    public boolean isValid() {
        return true;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_REG_GUIDE_TIPS);
    }

    @Override
    protected void onDoAfterWire() {
        try {
            regulationTypeListModelList = new ListModelList<>(RegulationFacade.getRegulationTypeList());
            checkedTypes = new HashMap<>(regulationTypeListModelList.size());
            BindUtils.postNotifyChange(null, null, RegulationGuidelinesStepViewModel.this, "regulationTypeListModelList");
        } catch (RestException e) {
            getParentWizardViewModel().close();
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_REGULATION_TYPES);
        }
    }

    @Override
    protected void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) {
        getNextButton().setDisabled(checkedTypes.size() == 0);
    }

    public ListModelList<RegulationType> getRegulationTypeListModelList() {
        return regulationTypeListModelList;
    }

    @Command("onCheck")
    public void onCheck(@BindingParam("target") Checkbox target, @BindingParam("index") long index) {
        regulationGuidelinesDesc = "";

        long type = ((RegulationType)target.getValue()).getType();
        if (target.isChecked()) {
            checkedTypes.put(type, type);
        } else {
            checkedTypes.remove(type);
        }

        // disable next button if at list one guideline not selected
        getNextButton().setDisabled(checkedTypes.size() == 0);

        regulationGuidelinesDesc = composeDescription(regulationTypeListModelList, checkedTypes);
        BindUtils.postNotifyChange(null, null, RegulationGuidelinesStepViewModel.this, "regulationGuidelinesDesc");
    }

    private String composeDescription(ListModelList<RegulationType> regulationTypeList, Map<Long, Long> checkedTypes) {
        String regGuidelinesDesc = "";
        for (RegulationType rt : regulationTypeList) {
            if (checkedTypes.containsKey(rt.getType())) {
                regGuidelinesDesc += rt.getDescription() + NEW_LINE + NEW_LINE;
            }
        }
        return regGuidelinesDesc;
    }

    public String getRegulationGuidelinesDesc() {
        return regulationGuidelinesDesc;
    }

    @Override
    public void onBeforeNext(AddDatabasesSaveEntity wizardSaveEntity) {
        RegulationSettings rs = new RegulationSettings();

        if (checkedTypes.containsKey(PCI_DSS_TYPE)) {
            rs.setPci(true);
        }

        if (checkedTypes.containsKey(HIPAA_TYPE)) {
            rs.setHipaa(true);
        }

        wizardSaveEntity.setRegulationSettings(rs);
    }

	@Override
	public String getHelpUrl() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public boolean onBeforeCancel(AddDatabasesSaveEntity wizardSaveEntity) {
		// TODO Auto-generated method stub
		return false;
	}


}
