package com.idera.sqlcm.ui.dialogs;

import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Window;

public class WelcomeDialogComposer extends SelectorComposer<Window> {

	private static final long serialVersionUID = 1810718150126742788L;

	public static final String ZUL_URL = "dialogs/welcome-dialog.zul";

	@Wire
	private Button closeButton;

	@Override
	public void doAfterCompose(Window comp) throws Exception {
		super.doAfterCompose(comp);

		closeButton.setFocus(true);
	}

	@Listen("onClick = #closeButton")
	public void closeEventHandler() {
		getSelf().detach();
	}
}
