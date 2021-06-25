package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.entities.CMManagedInstance;
import com.idera.sqlcm.entities.CMManagedInstanceDetails;
import com.idera.sqlcm.enumerations.AccountType;
import com.idera.sqlcm.enumerations.TimeMeasurement;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class InstanceDetailsPropertiesViewModel {
    public static final String ZUL_URL = "~./sqlcm/dialogs/instanceDetailsProperties.zul";
    private static final String INSTANCE_ID = "instanceId";

    @Wire
    Combobox ownersCombobox;

    @Wire
    Combobox locationsCombobox;

    @Wire
    Window editInstanceDetailsProperties;
  
    private String help;
    private CMManagedInstance managedInstance;
    private List<String> ownersList;
    private List<String> locationsList;
    private ListModelList<TimeMeasurement> timeMeasurementModelList;
    private ListModelList<AccountType> accountTypeModelList;
    private boolean showLogin = false;
    private boolean showCredentialFields = true;

    public static void showInstanceDetailsPropertiesDialog(long instanceId) {
        Map<String, Object> args = new HashMap<>();
        args.put(INSTANCE_ID, instanceId);
        Window window = (Window) Executions.createComponents(ZUL_URL, null, args);
        window.doHighlighted();
    }

    public String getHelp() {
        return help;
    }

    public List<String> getOwnersList() {
        return ownersList;
    }

    public List<String> getLocationsList() {
        return locationsList;
    }

    public CMManagedInstance getManagedInstance() {
        return managedInstance;
    }

    public ListModelList<TimeMeasurement> getTimeMeasurementModelList() {
        return timeMeasurementModelList;
    }

    public ListModelList<AccountType> getAccountTypeModelList() {
        return accountTypeModelList;
    }

    public boolean isShowLogin() {
        return showLogin;
    }

    public boolean isShowCredentialFields() {
        return showCredentialFields;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        help = "http://wiki.idera.com/x/fwC5Ag";

        initializeListboxes();
        HashMap<String, Object> args = (HashMap<String, Object>)Executions.getCurrent().getArg();
        long instanceId = (Long)args.get(INSTANCE_ID);
        try {
            CMManagedInstanceDetails instanceDetails = InstancesFacade.getManagedInstanceDetails(instanceId);
            managedInstance = instanceDetails.getManagedInstanceProperties();

            ownersList = instanceDetails.getOwners();
            ownersCombobox.setValue(managedInstance.getOwner());

            locationsList = new ListModelList<>(instanceDetails.getLocations());
            locationsCombobox.setValue(managedInstance.getLocation());

            accountTypeModelList.setSelection(Arrays.asList(AccountType.getByIndex(managedInstance.getCredentials().getAccountType())));
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_INSTANCE);
        }

        showCredentialFields();
    }

    @Command
    @NotifyChange({"showLogin", "showCredentialFields"})
    public void showCredentialFields() {
        AccountType selectedAccountType = accountTypeModelList.getSelection().iterator().next();
        if (selectedAccountType.equals(AccountType.SERVER_LOGIN_ACCOUNT)) {
            showCredentialFields = true;
            showLogin = true;            
            editInstanceDetailsProperties.setHeight("750px");
            
        } else if (selectedAccountType.equals(AccountType.WINDOWS_USER_ACCOUNT)){
            showCredentialFields = false;
            showLogin = false;
            editInstanceDetailsProperties.setHeight("700px");
        } else if (selectedAccountType.equals(AccountType.CM_ACCOUNT)) {
            showCredentialFields = false;
            showLogin = false;
            editInstanceDetailsProperties.setHeight("700px");
        }
    }

    
    public void SetCreddentia()
    {
    	 int currentAccountType=accountTypeModelList.getSelection().iterator().next().getIndex();
    	
    	 if (currentAccountType == AccountType.CM_ACCOUNT.getIndex()) {
             managedInstance.getCredentials().setAccount(null);
             managedInstance.getCredentials().setPassword(null);
         }
    	 if (currentAccountType == AccountType.WINDOWS_USER_ACCOUNT.getIndex()) {
             managedInstance.getCredentials().setAccount(null);
             managedInstance.getCredentials().setPassword(null);
         }
   
    }
    
    
    @Command
    public void save(@BindingParam("comp") Window x) {
        try {
            managedInstance.setLocation(locationsCombobox.getValue().trim());
            managedInstance.setOwner(ownersCombobox.getValue().trim());

            int currentAccountType = accountTypeModelList.getSelection().iterator().next().getIndex();
            if (currentAccountType == AccountType.CM_ACCOUNT.getIndex()) {
                managedInstance.getCredentials().setAccount(null);
                managedInstance.getCredentials().setPassword(null);
            }
            managedInstance.getCredentials().setAccountType(currentAccountType);
            InstancesFacade.updateManageInstance(managedInstance);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPDATE_INSTANCE);
        }
        EventQueue<Event> eq = EventQueues.lookup(ManageSqlServersInstancesViewModel.UPDATE_MANAGE_INSTANCE_LIST_EVENT, EventQueues.APPLICATION, false);
        if (eq != null) {
            eq.publish(new Event("onClick", null, null ));
        }
        x.detach();
    }

    @Command
    public void testCredentials() {
        managedInstance.getCredentials().setAccountType(accountTypeModelList.getSelection().iterator().next().getIndex());
        TestCredentialsViewModel.showTestCredentialsDialog(managedInstance.getCredentials(), Arrays.asList(managedInstance.getId()));
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    private void initializeListboxes() {
        timeMeasurementModelList = new ListModelList<>();
        timeMeasurementModelList.addAll(Arrays.asList(TimeMeasurement.MINUTE, TimeMeasurement.DAY));
        timeMeasurementModelList.setSelection(Arrays.asList(TimeMeasurement.MINUTE));

        accountTypeModelList = new ListModelList<>();
        accountTypeModelList.addAll(Arrays.asList(AccountType.WINDOWS_USER_ACCOUNT, AccountType.CM_ACCOUNT, AccountType.SERVER_LOGIN_ACCOUNT));
        accountTypeModelList.setSelection(Arrays.asList(AccountType.CM_ACCOUNT));
    }

}
