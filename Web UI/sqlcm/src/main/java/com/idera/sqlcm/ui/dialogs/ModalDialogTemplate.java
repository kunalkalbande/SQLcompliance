package com.idera.sqlcm.ui.dialogs;

import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Window;

public class ModalDialogTemplate extends SelectorComposer<Window> {

    static final long serialVersionUID = 1L;
    private static final String DIALOG_HELP_REFERENCE = "/some_url";
    @Wire
    protected A closeLink;
    @Wire
    protected A dialogHelpLink;

    public void doAfterCompose(Window component) throws Exception {
        super.doAfterCompose(component);
        if (dialogHelpLink != null) {
            dialogHelpLink.setHref(DIALOG_HELP_REFERENCE);
            dialogHelpLink.setTarget("_blank");
        }
    }

    @Listen("onClick = a#closeLink")
    public void closeWindow() {
        getSelf().detach();
    }
}
