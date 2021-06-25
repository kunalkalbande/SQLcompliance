package com.idera.sqlcm.ui.dialogs.addregulationguidelinewizard.steps;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zul.ListModelList;

import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.AddDatabasesSaveEntity;
import com.idera.sqlcm.ui.dialogs.adddatabasewizard.RegulationSettings;
import com.idera.sqlcm.ui.dialogs.regulationDetails.RegulationDetailsViewModel;

public class RegulationGuidelineSummaryStepViewModel extends RegulationGuidelineAddWizardStepBase {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/regulationguidelinewizard/steps/summary-step.zul";

	private RegulationSettings regulationSettings;

	private String regulationLevel;
	
	private String regulationLevelForDetail;

	private String serverName;

	public RegulationGuidelineSummaryStepViewModel() {
		super();
	}

	private ListModelList<CMDatabase> selectedDatabaseList;
	private List<CMDatabase> currentDatabase;

	public List<CMDatabase> getCurrentDatabase() {
		return currentDatabase;
	}

	public ListModelList<CMDatabase> getSelectedDatabaseList() {
		return selectedDatabaseList;
	}

	public String getAuditLevelName() {
		return regulationLevel;
	}

	public String getServerName() {
		return serverName;
	}

	public boolean isRegGuideDetailLinkVisibility() {
		return true;
	}

	@Override
	public void doOnShow(AddDatabasesSaveEntity wizardSaveEntity) {
		getParentWizardViewModel().getWizardEntity();
		regulationSettings = wizardSaveEntity.getRegulationSettings();
		StringBuilder sb = new StringBuilder();
		if (regulationSettings != null) {
			if (regulationSettings.isHipaa()) {
				sb.append("HIPAA").append(",");
			}
			if (regulationSettings.isPci() == true) {
				sb.append("PCI DSS").append(",");
			}
			if (regulationSettings.isDisa() == true) {
				sb.append("DISA STIG").append(",");
			}
			if (regulationSettings.isNerc() == true) {
				sb.append("NERC").append(",");
			}
			if (regulationSettings.isCis() == true) {
				sb.append("CIS").append(",");
			}
			if (regulationSettings.isSox() == true) {
				sb.append("SOX").append(",");
			}
			if (regulationSettings.isFerpa() == true) {
				sb.append("FERPA").append(",");
			}
			if (sb.length() > 0) {
				regulationLevel = sb.deleteCharAt(sb.length() - 1).toString();
				regulationLevelForDetail = regulationLevel;
			}
			else
			{
				regulationLevelForDetail = "None";
			}
			if (wizardSaveEntity != null && wizardSaveEntity.getAuditedActivities().isCustomEnabled()) {
				regulationLevel = "Custom";
			}
		}
		serverName = getInstance().getInstanceName();
		wizardSaveEntity.setCollectionLevel(2);
		selectedDatabaseList = (ListModelList<CMDatabase>) wizardSaveEntity.getDatabaseList();
		BindUtils.postNotifyChange(null, null, RegulationGuidelineSummaryStepViewModel.this, "*");
	}

	@Override
	public String getNextStepZul() {
		return null;
	}

	@Override
	public boolean isValid() {
		return true;
	}

	@Override
	public String getTips() {
		return ELFunctions.getLabel(SQLCMI18NStrings.ADD_DATABASE_WIZARD_SUMMARY_TIPS);
	}

	@Override
	public void onFinish(AddDatabasesSaveEntity wizardSaveEntity) {   
	}

	@Command("onOpenRegulationDetailClick")
	public void onOpenRegulationDetailClick() {
		new AddDatabasesSaveEntity();
		Map<String, String> selectedRegulationTypes = new HashMap<String, String>();
		String regulationLevels[] = regulationLevelForDetail.split(",");
		if (regulationLevels != null) {
			for (int i = 0; i < regulationLevels.length; i++) {
				selectedRegulationTypes.put(regulationLevels[i], "");
			}
		}
		RegulationDetailsViewModel.show(selectedRegulationTypes.keySet());
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