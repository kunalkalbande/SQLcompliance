package com.idera.sqlcm.ui.dialogs.adddataalertruleswizard.steps;

import com.idera.sqlcm.entities.CMInstancePermissionBase;
import com.idera.sqlcm.ui.dialogs.AddPrivilegedUsersViewModel;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventCondition;
import com.idera.sqlcm.ui.dialogs.addalertruleswizard.EventField;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Decimalbox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class SpecifyRowCountThresholdViewModel implements
		AddPrivilegedUsersViewModel.DialogListener {
	EventField eventtype = new EventField();
	public String[] targetString = {};
	EventCondition eventCondition = new EventCondition();
	String compOpr = ">";
	String rowCount;
	String timeInterval;

	public String getCompOpr() {
		return compOpr;
	}

	public void setCompOpr(String compOpr) {
		this.compOpr = compOpr;
	}

	public String getRowCount() {
		return rowCount;
	}

	public void setRowCount(String rowCount) {
		this.rowCount = rowCount;
	}

	public String getTimeInterval() {
		return timeInterval;
	}

	public void setTimeInterval(String timeInterval) {
		this.timeInterval = timeInterval;
	}
	@Command
	public void closeDialog(@BindingParam("comp") Window x) {
		x.detach();
	}

	@Override
	public void onOk(long instanceId,
			List<CMInstancePermissionBase> selectedPermissionList) {
	}

	@Override
	public void onCancel(long instanceId) {
		Sessions.getCurrent().removeAttribute("rowCountDetails");
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
		Selectors.wireComponents(view, this, false);
		if (Sessions.getCurrent().getAttribute("rowCountDetails") != null) {
			GetData();
			BindUtils.postNotifyChange(null, null, this, "*");
		}
	}

	public void GetData() {
		String strMatchString = (String) Sessions.getCurrent().getAttribute(
				"rowCountDetails");
		if (!strMatchString.isEmpty()) {
			Map<String, String> EventNodeDataValue = new HashMap<String, String>();
			String active = strMatchString;
			int index = strMatchString.indexOf("(");

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
					active = (active.substring(Integer.parseInt(length))
							.toString());
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
			if (active.length() > 0)
				try {
					throw new Exception("Improperly formed KeyValue string.");
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			
			if (EventNodeDataValue.containsKey("operator"))
				setCompOpr(EventNodeDataValue.get("operator").toString());
			if (EventNodeDataValue.containsKey("rowcount"))
				setRowCount(EventNodeDataValue.get("rowcount").toString());
			if (EventNodeDataValue.containsKey("timeframe"))
				setTimeInterval(EventNodeDataValue.get("timeframe").toString());
		}
	}

	@Command("submitChoice")
	public void submitChoice(@BindingParam("rowCountDropDown") Combobox cb,
			@BindingParam("rowCountTextBox") Textbox tb,
			@BindingParam("timeFrame") Decimalbox tf,
			@BindingParam("comp") Window x) throws Exception {
		compOpr = cb.getText();
		rowCount = tb.getText();
		timeInterval = tf.getText();
		String rowCountMatchString = "operator(" + compOpr.length() + ")"
				+ compOpr + "rowcount(" + rowCount.length() + ")" + rowCount
				+ "timeframe(" + timeInterval.length() + ")" + timeInterval;
		Sessions.getCurrent().setAttribute("rowCountDetails",
				rowCountMatchString);
		x.detach();
	}

}
