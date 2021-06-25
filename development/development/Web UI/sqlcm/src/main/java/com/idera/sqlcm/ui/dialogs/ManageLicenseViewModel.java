package com.idera.sqlcm.ui.dialogs;

import java.io.File;
import java.io.IOException;
import java.net.URL;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMResponse;
import com.idera.sqlcm.entities.License;
import com.idera.sqlcm.facade.LicenseFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.utils.SQLCMConstants;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Window;

public class ManageLicenseViewModel {
    public static final String ZUL_URL = "~./sqlcm/dialogs/manageLicense.zul";

    private String help;
    private License license;
    private String buyLicenseLink;
    private String newLicenseString;

    public String getHelp() {
        return help;
    }

    public License getLicense() {
        return license;
    }

    public String getBuyLicenseLink() {
        return buyLicenseLink;
    }

    public void setNewLicenseString(String newLicenseString) {
        this.newLicenseString = newLicenseString;
    }

    public static void showManageLicenseDialog() {
        Window window = (Window) Executions.createComponents(ZUL_URL, null, null);
        window.doHighlighted();
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        help = "http://wiki.idera.com/x/LQQsAw";
        buyLicenseLink = SQLCMConstants.BUY_LICENSE_LINK;

        initLicense();
    }

    @Command
    public void applyNewLicense() {
        try {
            CMResponse result = LicenseFacade.addLicense(newLicenseString);

            if (result.isSuccess()) {
                WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_APPLY_LICENSE_SUCCESS);
            } else {
                WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.MESSAGE_FAILED_TO_APPLY_LICENSE_KEY,
                        newLicenseString, result.getErrorMessage());
            }
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_APPLY_NEW_CM_LICENSE);
        }
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }
    
    @Command("openLicenseManager")
	public void openLicenseManager() {
    	try {
			String workingDir = System.getProperty("user.dir");
			String DisableUAC = " runas /profile /user:\\administrator \"C:\\Windows\\System32\\cmd.exe /k %windir%\\System32\\reg.exe ADD HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System /v EnableLUA /t REG_DWORD /d 0 /f\"";
			String EnableUAC = "runas /noprofile /user:mymachine\\administrator \"C:\\Windows\\System32\\cmd.exe /k %windir%\\System32\\reg.exe ADD HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System /v EnableLUA /t REG_DWORD /d 1 /f\"";
			workingDir = "cmd /c \"C:\\Program Files\\Idera\\SQLcompliance\\License Manager Utility.exe\"";      
			//WebUtil.showInfoBoxWithCustomMessage(workingDir);
			Runtime.getRuntime().exec(DisableUAC);
			Runtime.getRuntime().exec(workingDir);
			Runtime.getRuntime().exec(DisableUAC); 
        } catch (IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }    	
    }   
    

    private void initLicense() {
        try {
            license = LicenseFacade.getLicense();
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_LICENSE);
        }
    }
}
