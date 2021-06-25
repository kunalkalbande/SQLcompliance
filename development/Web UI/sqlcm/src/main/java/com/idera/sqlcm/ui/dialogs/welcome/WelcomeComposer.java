package com.idera.sqlcm.ui.dialogs.welcome;

import com.idera.i18n.I18NStrings;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.Abstract3StepWizardComposer;
import com.idera.sqlcm.ui.dialogs.ManageUsersViewModel;

import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Window;

public class WelcomeComposer extends Abstract3StepWizardComposer {

    public static final String ZUL_URL = "~./sqlcm/dialogs/welcome/welcome.zul";

    @Override
    public void doAfterCompose(Window comp) throws Exception {
        super.doAfterCompose(comp);
        this.step1Tips.setVisible(false);
        this.step2Tips.setVisible(false);
        this.step3Tips.setVisible(false);
        this.activate(new StepComponents(this.step1Tips, this.step1Content, this.step1Item));
    }

    @Listen("onClick = button#closeButton")
    public void closeWindow() {
        if (WebUtil.showConfirmationBoxWithIcon(I18NStrings.ARE_YOU_SURE_YOU_WANT_TO_CLOSE_THE_WIZARD, SQLCMI18NStrings.QUESTION, 
                "/images/confirm_question.svg", true, (Object) null)) {
            getSelf().detach();
        }
    }

    @Override
    public void validate() {

    }

    @Override
    @Listen("onDoSave = window")
    public void onDoSave(Event event) {
        //Clients.clearBusy();
        //validate();
        getSelf().detach();
    }

    @Override
    protected boolean showWarningOnClose() {
        return true;
    }

    @Override
    protected void updateSummaryValues() {

    }

    @Override
    public void validatesTEP1() {
    }

    @Override
    public void validatesTEP2() {
    }

//    @Command
    @Listen("onClick = #manageUsersLink")
    public void openManageUsers() {
        ManageUsersViewModel.showManageUsersDialog();
    }

}
