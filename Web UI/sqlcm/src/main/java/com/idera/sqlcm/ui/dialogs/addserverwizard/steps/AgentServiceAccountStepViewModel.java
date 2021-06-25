package com.idera.sqlcm.ui.dialogs.addserverwizard.steps;


import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMDomainCredentials;
import com.idera.sqlcm.entities.addserverwizard.AddServerWizardEntity;
import com.idera.sqlcm.entities.addserverwizard.AgentServiceAccount;
import com.idera.sqlcm.facade.CredentialsFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.server.web.WebUtil;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Textbox;

import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.regex.PatternSyntaxException;

public class AgentServiceAccountStepViewModel extends AddWizardStepBase {

    public static final String ZUL_PATH = "~./sqlcm/dialogs/addserverwizard/steps/agent-service-account-step.zul";

    public static final String DOMAIN_ACCOUNT_REGEXP_PATTERN = "^[^\\\\]{1,}\\\\[^\\\\]{1,}$"; // Valid examples: a\\b, domain\\user

    @Wire
    private Textbox tbLogin;

    @Wire
    private Textbox tbPass;

    @Wire
    private Textbox tbConfirmPass;

    private String login;

    private String pass = "";

    private String confirmPass = "";

    public String getLogin() {
        return login;
    }

    public void setLogin(String login) {
        this.login = login;
    }

    public String getPass() {
        return pass;
    }

    public void setPass(String pass) {
        this.pass = pass;
    }

    public String getConfirmPass() {
        return confirmPass;
    }

    public void setConfirmPass(String confirmPass) {
        this.confirmPass = confirmPass;
    }

    @Override
    public String getNextStepZul() {
        return AgentTraceDirectoryStepViewModel.ZUL_PATH;
    }

    @Override
    public String getTips() {
        return ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_TIPS);
    }

    @Override
    public String getHelpUrl() {
        return "http://wiki.idera.com/display/SQLCM/Configuration+wizard+-+SQLcompliance+Agent+Service+Account+window";
    }

    private boolean isLoginMatchPattern(String login) {
        boolean match = false;
        try {
            Pattern regex = Pattern.compile(DOMAIN_ACCOUNT_REGEXP_PATTERN);
            Matcher regexMatcher = regex.matcher(login);
            match = regexMatcher.matches();
        } catch (PatternSyntaxException ex) {
            throw new RuntimeException(" Syntax error in the regular expression for validate login ");
        }
        return match;
    }

    public boolean validateLogin() {
        if (login != null && !login.trim().isEmpty() && isLoginMatchPattern(login)) {
            Clients.clearWrongValue(tbLogin);
            return true;
        }
        Clients.wrongValue(tbLogin, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_ENTER_LOGIN_ACCOUNT));
        return false;
    }

    public boolean validatePass() {
        if (pass.equals(confirmPass)) {
            Clients.clearWrongValue(tbPass);
            return true;
        }
        Clients.wrongValue(tbConfirmPass, ELFunctions.getLabel(SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_PASSWORD_DONT_MATCH));
        return false;
    }

    public boolean validateDomainCredentialsOnServer() {
        boolean isValidCredentials = false;

        try {
            isValidCredentials = CredentialsFacade.validateDomainCredentials(new AgentServiceAccount(login, pass));
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_CREDENTIALS_ERROR_VALIDATE_CALL);
        }

        if (!isValidCredentials) {
            WebUtil.showErrorBox(new RuntimeException(), SQLCMI18NStrings.ADD_SERVER_WIZARD_AGENT_SERVICE_ACCOUNT_CREDENTIALS_COULD_NOT_BE_VERIFIED);
        }

        return isValidCredentials;
    }

    @Override
    public boolean isValid() {
        return (validateLogin() & validatePass()) && validateDomainCredentialsOnServer();
    }

    @Override
    public void onBeforeNext(AddServerWizardEntity wizardSaveEntity) {
        wizardSaveEntity.getAddServerEntity().getAgentDeploymentProperties().setAgentServiceAccount(new AgentServiceAccount(login, pass));
    }
}
