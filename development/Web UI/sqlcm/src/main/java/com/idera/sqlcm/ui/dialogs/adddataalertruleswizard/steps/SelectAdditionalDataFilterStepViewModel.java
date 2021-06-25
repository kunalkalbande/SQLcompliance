package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import com.idera.ccl.IderaDropdownList;
import com.idera.sqlcm.entities.CMAlertRules;
import com.idera.sqlcm.entities.CMAlertRulesCondition;
import com.idera.sqlcm.entities.CategoryData;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.RegulationSettings;
import com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.AddDataAlertRulesSaveEntity;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyAppNameViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyAppNameViewModel.App;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyLoginViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifySQLServerViewModel;
import com.idera.sqlcm.ui.instancesAlertsRule.SpecifyLoginViewModel.Login;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.A;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Window;

public class SelectAdditionalDataFilterStepViewModel extends AddWizardStepBase {

	public static final String ZUL_PATH = "~./sqlcm/dialogs/adddataalertruleswizard/steps/select-data-filter-additional-step.zul";
	RegulationSettings rs = new RegulationSettings();
	@Wire
	private Radiogroup rgFlow;

	@Wire
	private A specifiedWords;

	int fieldId;
	private List<CMAlertRulesCondition> conditionEvents;
	private List<CMAlertRules> alertRules;
	
	@Wire
	private A specifyRowCountThreshold;

	@Wire
	private A specifiedWordsName;

	List<CategoryData> entitiesList;

	public void setEntitiesList(List<CategoryData> entitiesList) {
		this.entitiesList = entitiesList;
	}

	private Map<String, Long> checkedAlertRulesTypes = new HashMap<String, Long>();
	
	@Wire
	private Checkbox chkLogin;

	@Wire
	private Checkbox chkApplication;

	@Wire
	private Checkbox rowCountWithTimeInterval;

	public Checkbox getRowCountWithTimeInterval() {
		return rowCountWithTimeInterval;
	}

	public void setRowCountWithTimeInterval(Checkbox rowCountWithTimeInterval) {
		this.rowCountWithTimeInterval = rowCountWithTimeInterval;
	}

	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@Override
	public String getNextStepZul() {
		return DataAlertActionStepViewModel.ZUL_PATH;
	}

	@Override
	public boolean isValid() {
		return true;
	}

	@Command("afterRenderGrid")
	public void afterRenderGrid() {
		if (rgFlow.getItemCount() > 0) {
			rgFlow.setSelectedIndex(0);
		}
	}

	@Command("eventAlertRules")
	public void eventAlertRules(@BindingParam("id") long id) {
		SpecifySQLServerViewModel.showSpecifySQLServersDialog(id);
	}

	@Command("onCheck")
	public void onCheck(@BindingParam("target") Checkbox target,
			@BindingParam("index") long index,
			@BindingParam("lstBox") IderaDropdownList lstBox,
			@BindingParam("lbl") A lbl) {
		if (target.isChecked()) {
			String chkName = target.getName();
			checkedAlertRulesTypes.put(chkName, index);
			lbl.setDisabled(false);

			if (chkName.equals("Application Name")) {

				specifiedWords.setDisabled(false);
				chkApplication.setChecked(true);
			}
			if (chkName.equals("Login Name")) {
				specifiedWordsName.setDisabled(false);
				chkLogin.setChecked(true);
			}
			if (chkName.equals("Row Count (with Time Interval)")) {
				specifyRowCountThreshold.setDisabled(false);
				rowCountWithTimeInterval.setChecked(true);
			}

		} else {
			String chkName = target.getName();
			checkedAlertRulesTypes.remove(target.getName());
			lbl.setDisabled(true);
			if (chkName.equals("Application Name")) {
				specifiedWords.setDisabled(true);
				Sessions.getCurrent().removeAttribute("appMatchString");
			}
			if (chkName.equals("Login Name")) {
				specifiedWordsName.setDisabled(true);
				Sessions.getCurrent().removeAttribute("loginMatchString");
			}
			if (chkName.equals("Row Count (with Time Interval)")) {
				specifyRowCountThreshold.setDisabled(true);
				Sessions.getCurrent().removeAttribute("rowCountDetails");
			}
		}
		BindUtils.postNotifyChange(null, null,
				SelectAdditionalDataFilterStepViewModel.this, "*");
	}

	@Override
	public void onBeforeNext(AddDataAlertRulesSaveEntity wizardSaveEntity) {
		if (chkApplication.isChecked()) {
			specifiedWords.setDisabled(false);
			rs.setApplicationName(true);
			rs.setAppFieldId(15);
			String appMatchString = "";
			appMatchString = (String) Sessions.getCurrent().getAttribute(
					"appMatchString");
			if (appMatchString != null && !appMatchString.isEmpty()
					&& !appMatchString.equals("")) {
				rs.setAppMatchString(appMatchString);

				ListModelList<String> AppDataVal = GetExtractedDataOptions(appMatchString);
				SpecifyAppNameViewModel appViewModel = new SpecifyAppNameViewModel();

				if (AppDataVal != null && (!AppDataVal.isEmpty())) {
					ListModelList<App> dataList = new ListModelList<>();
					int length = AppDataVal.getSize();
					for (int i = 0; i < length; i++) {
						dataList.add(appViewModel.new App(AppDataVal.get(i)));
					}
					rs.setAppNameList(dataList);
					wizardSaveEntity.getRegulationSettings().setAppNameList(
							dataList);
				} else {
					rs.setAppNameList(null);
					wizardSaveEntity.getRegulationSettings().setAppNameList(
							null);
				}

			} else {
				rs.setAppNameList(null);
				wizardSaveEntity.getRegulationSettings().setAppNameList(null);

			}
			wizardSaveEntity.getRegulationSettings().setAppMatchString(
					appMatchString);
			wizardSaveEntity.getRegulationSettings().setAppFieldId(15);
			wizardSaveEntity.getRegulationSettings().setApplicationName(true);
		}

	else {
	    wizardSaveEntity.getRegulationSettings().setApplicationName(false);
	    Sessions.getCurrent().removeAttribute("appMatchString");
	}
		if (chkLogin.isChecked()) {
			rs.setLoginName(true);
			rs.setLoginFieldId(16);
			String loginMatchString = "";

			loginMatchString = (String) Sessions.getCurrent().getAttribute(
					"loginMatchString");

			if (loginMatchString != null && !loginMatchString.isEmpty()
					&& !loginMatchString.equals("")) {
				rs.setLoginMatchString(loginMatchString);

				ListModelList<String> loginDataVal = GetExtractedDataOptions(loginMatchString);
				SpecifyLoginViewModel loginViewModel = new SpecifyLoginViewModel();

				if (loginDataVal != null && (!loginDataVal.isEmpty())) {
					ListModelList<Login> dataList = new ListModelList<>();
					int length = loginDataVal.getSize();
					for (int i = 0; i < length; i++) {
						dataList.add(loginViewModel.new Login(loginDataVal
								.get(i)));
					}
					rs.setLoginNameList(dataList);
					wizardSaveEntity.getRegulationSettings().setLoginNameList(
							dataList);
				} else {
					rs.setLoginNameList(null);
					wizardSaveEntity.getRegulationSettings().setLoginNameList(
							null);
				}
			} else {
				rs.setLoginNameList(null);
				wizardSaveEntity.getRegulationSettings().setLoginNameList(null);
			}

			wizardSaveEntity.getRegulationSettings().setLoginMatchString(
					loginMatchString);
			wizardSaveEntity.getRegulationSettings().setLoginFieldId(16);
			wizardSaveEntity.getRegulationSettings().setLoginName(true);
		} else {

			wizardSaveEntity.getRegulationSettings().setLoginName(false);
			Sessions.getCurrent().removeAttribute("loginMatchString");
		}

		if (rowCountWithTimeInterval.isChecked()) {
			String rowCountData = (String) Sessions.getCurrent().getAttribute(
					"rowCountDetails");
			wizardSaveEntity.getRegulationSettings().setRowCountMatchString(
					rowCountData);
			wizardSaveEntity.getRegulationSettings().setRowCountFieldId(14);
			wizardSaveEntity.getRegulationSettings()
					.setRowCountWithTimeInterval(true);
	}

	else {
	    wizardSaveEntity.getRegulationSettings()
		    .setRowCountWithTimeInterval(false);
	    Sessions.getCurrent().removeAttribute("rowCountDetails");
		}
	}

	@Override
	protected void doOnShow(AddDataAlertRulesSaveEntity wizardSaveEntity) {
		String matchString[];
		if (Sessions.getCurrent().getAttribute("QueryType") != null) {
			int charCount = 0;
			String loginName = "";
			String inclusive = "";
			long fieldId = 0;
			if (Sessions.getCurrent().getAttribute("isInitializedDataRule") == null) {
				conditionEvents = (List<CMAlertRulesCondition>) Sessions
						.getCurrent().getAttribute("conditionEvents");
				CMAlertRulesCondition cMAlertRulesConditionObj = new CMAlertRulesCondition();
				matchString = new String[conditionEvents.size()];
				for (int i = 0; i < conditionEvents.size(); i++) {
					matchString[i] = conditionEvents.get(i).getMatchString();
					fieldId = conditionEvents.get(i).getFieldId();

					if (fieldId == 16) {
						String loginNameWizard = matchString[i];
						if (loginNameWizard != null
								&& !loginNameWizard.isEmpty()) {
							charCount = Integer.parseInt(loginNameWizard
									.substring(
											loginNameWizard.indexOf("(") + 1,
											loginNameWizard.indexOf(")")));
							loginName = loginNameWizard.substring(
									loginNameWizard.indexOf(")") + 1,
									loginNameWizard.indexOf(")") + charCount
											+ 1);
							inclusive = loginNameWizard
									.substring(loginNameWizard
											.indexOf("include"));
							charCount = Integer.parseInt(inclusive.substring(
									inclusive.indexOf("(") + 1,
									inclusive.indexOf(")")));
							inclusive = inclusive.substring(
									inclusive.indexOf(")") + 1,
									inclusive.indexOf(")") + charCount + 1);
						}
						if (loginName != null) {
							Sessions.getCurrent().setAttribute(
									"loginMatchString", loginNameWizard);
						}
					}
					if (fieldId == 15) {
						String applicationNameWizard = matchString[i]
								.toString();
						if (applicationNameWizard != null) {
							Sessions.getCurrent().setAttribute(
									"appMatchString", applicationNameWizard);

						}
					}
					if (fieldId == 14) {
						String rowCountWizard = matchString[i].toString();
						if (rowCountWizard != null
								&& rowCountWizard.length() > 0) {
							String rowCountFinalVal = rowCountWizard
									.substring(rowCountWizard
											.indexOf("rowcount"));
							int rowCountCharCount = Integer
									.parseInt(rowCountFinalVal.substring(
											rowCountFinalVal.indexOf("(") + 1,
											rowCountFinalVal.indexOf(")")));
							String rowCount = (rowCountFinalVal.substring(
									rowCountFinalVal.indexOf(")") + 1,
									rowCountFinalVal.indexOf(")")
											+ rowCountCharCount + 1));
							if (rowCount != null) {

								Sessions.getCurrent().setAttribute(
										"rowCountDetails", rowCountWizard);
							}
						} else {
							rowCountWithTimeInterval.setChecked(true);
							specifyRowCountThreshold.setDisabled(false);
						}
					}

				}
				Sessions.getCurrent().setAttribute("isInitializedDataRule",
						true);
			}
			if (Sessions.getCurrent().getAttribute("rowCountDetails") != null) {
				rowCountWithTimeInterval.setChecked(true);
				specifyRowCountThreshold.setDisabled(false);

			}
			if (Sessions.getCurrent().getAttribute("loginMatchString") != null) {
				chkLogin.setChecked(true);
				specifiedWordsName.setDisabled(false);

			}
			if (Sessions.getCurrent().getAttribute("appMatchString") != null) {

				chkApplication.setChecked(true);
				specifiedWords.setDisabled(false);

			}
				BindUtils.postNotifyChange(null, null,
						SelectAdditionalDataFilterStepViewModel.this, "*");
		}
	}

	@Override
	public void onCancel(AddDataAlertRulesSaveEntity wizardSaveEntity) {
		if (Sessions.getCurrent().getAttribute("QueryType") != null) {
			Sessions.getCurrent().removeAttribute("QueryType");
			Sessions.getCurrent().removeAttribute("Category");
		}

		String uri = "instancesAlertsRule";
		uri = WebUtil.buildPathRelativeToCurrentProduct(uri);
		Executions.sendRedirect(uri);
	}

	private ListModelList<String> GetExtractedDataOptions(String matchString) {
		Map<String, String> EventNodeDataValue = new HashMap<String, String>();
		String active = matchString;
		int index = matchString.indexOf("(");

		try {
			while (index != -1) {
				String sKey, sValue;
				String length;

				sKey = active.substring(0, index);
				active = active.substring(index + 1);
				index = active.indexOf(")");
				length = active.substring(0, index);
				active = active.substring(index + 1);
				sValue = (active.subSequence(0, Integer.parseInt(length))
						.toString());
				active = (active.substring(Integer.parseInt(length)).toString());
				EventNodeDataValue.put(sKey, sValue);
				index = active.indexOf("(");
			}
		} catch (Exception e) {
			try {
				throw new Exception("Improperly formed KeyValue string.", e);
			} catch (Exception e1) {
				// TODO Auto-generated catch block
				e1.printStackTrace();
			}
		}

		int iCountArray = 0;

		for (Map.Entry<String, String> entry : EventNodeDataValue.entrySet()) {
			if (entry.getKey().equals("count")) {
				iCountArray = Integer.parseInt(entry.getValue());
			}
		}

		ListModelList<String> list = new ListModelList<>();
		for (int k = 0; k < iCountArray; k++) {
			list.add((String) EventNodeDataValue.get("" + k));
		}
		return list;
	} 

	@Override
	public String getHelpUrl() {
		return "http://wiki.idera.com/display/SQLCM/SQL+Compliance+Manager+Home";
	}
}
