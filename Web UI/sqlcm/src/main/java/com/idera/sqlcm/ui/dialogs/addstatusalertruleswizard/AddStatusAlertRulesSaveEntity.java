package com.idera.sqlcm.ui.dialogs.addstatusalertruleswizard;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMDmlSelectFilters;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.instances.CMAuditedActivities;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.SelectAlertActions;
import com.idera.sqlcm.wizard.IWizardEntity;

import java.util.List;

public class AddStatusAlertRulesSaveEntity extends CMEntity implements IWizardEntity {

    @JsonProperty("newStatusAlertRulesData")
    private NewStatusAlertRulesData newStatusAlertRulesData;
   
    @JsonProperty("selectAlertActions")
    private SelectAlertActions selectAlertActions;
   
    
	public SelectAlertActions getSelectAlertActions() {
		return selectAlertActions;
	}

	public void setSelectAlertActions(SelectAlertActions selectAlertActions) {
		this.selectAlertActions = selectAlertActions;
	}

	public NewStatusAlertRulesData getNewStatusAlertRulesData() {
		return newStatusAlertRulesData;
	}

	public void setNewStatusAlertRulesData(
			NewStatusAlertRulesData newStatusAlertRulesData) {
		this.newStatusAlertRulesData = newStatusAlertRulesData;
	}
}
