package com.idera.sqlcm.ui.instancedetails;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.enumerations.InstanceStatus;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.converter.SimpleDateConverter;
import com.idera.sqlcm.ui.databases.DatabaseIconURLConverter;
import com.idera.sqlcm.ui.dialogs.ColumnSearchViewModel;
import com.idera.sqlcm.ui.dialogs.ManageConfigRefreshViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.ServerWizardViewModel;
import com.idera.sqlcm.wizard.AbstractWizardViewModel;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.Converter;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;

import com.idera.server.web.ELFunctions;
import com.idera.server.web.ELFunctions.IconSize;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.facade.InstancesFacade;

import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;

import java.util.List;

public class InstanceOverview implements AbstractWizardViewModel.WizardListener {
	private static final Logger logger = Logger.getLogger(InstanceOverview.class);
    private static final long INVALID_INSTANCE_ID = Long.MIN_VALUE;
    private static final String EMPTY_STRING = "";
    private int days = Interval.SEVEN_DAY.getDays();
    private CMInstance instance;
    private long instanceId = INVALID_INSTANCE_ID;
    private Converter dateConverter = new SimpleDateConverter(SQLCMI18NStrings.NEVER);
    private static final String ZUL_URL = "~./sqlcm/instancedetails/instance_detail_sensitive_column_import.zul"; 
    public String getStatusText() {
        String statusText = "";
        if (instance != null) {
            statusText = instance.getStatusText();
            if (InstanceStatus.OK.getId() == instance.getStatus() & (statusText == null || statusText.isEmpty())) {
                statusText = ELFunctions.getLabel(SQLCMI18NStrings.OK);
            }
        }

        return statusText;
    }

    public Converter getDateConverter() {
        return dateConverter;
    }

    private DatabaseIconURLConverter iconURLConverter = new DatabaseIconURLConverter();

    private ListModelList<CMDatabase> databaseList;

    private int refreshDuration; //SQLCM 5.4 SCM-9 Start
    
   	public int getRefreshDuration() {
   		return refreshDuration;
   	}

   	public void setRefreshDuration(int refreshDuration) {
   		this.refreshDuration = refreshDuration;
   	}
    
    
    public ListModelList<CMDatabase> getDatabaseList() {
        return databaseList;
    }

    public DatabaseIconURLConverter getIconURLConverter() {
        return iconURLConverter;
    }

    public boolean isVisible() {
        return instance != null;
    }

    @Init
	public void init() {
        try
        {
    	instanceId = Utils.parseInstanceIdArg();
        Sessions.getCurrent().setAttribute("currentInstanceId", instanceId);
        loadInstance();
        loadDatabases();
        String refreshDuration= RefreshDurationFacade.getRefreshDuration();
		int refDuration=Integer.parseInt(refreshDuration);
		refDuration=refDuration*1000;
		setRefreshDuration(refDuration);
        }
        catch(Exception e)
        {
        	e.getStackTrace();
        }
	}
    
   
    @Command("refreshEvents")
    public void refreshEvent() {
        if (instance != null) {
            try {
                databaseList = new ListModelList<>(DatabasesFacade.getAuditedDatabasesForInstance(instance.getId()));
                loadInstance();
                loadDatabases();
            } catch (RestException e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_AUDITED_DATABASE_LIST);
            }
            
        }
        BindUtils.postNotifyChange(null, null, this, "*");
    }
      
    
    
    private void loadDatabases() {
        if (instance != null) {
            try {
                databaseList = new ListModelList<>(DatabasesFacade.getAuditedDatabasesForInstance(instance.getId()));
            } catch (RestException e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_AUDITED_DATABASE_LIST);
            }
        }
    }

    public String getStatusImage() {

		String iconSize = IconSize.SMALL.getStringValue();
		String imgUrl = ELFunctions.getImageURL("instance-unknown", iconSize);
		if (instance != null) {
			switch (InstanceStatus.getById(instance.getDetailedServerStatus())) {
			case DISABLED:
            case WARNING:
				imgUrl = ELFunctions.getImageURL("warning", iconSize);
				break;
			case ALERT:
				imgUrl = ELFunctions.getImageURL("instance-error", iconSize);
				break;
           case OK:
				imgUrl = ELFunctions.getImageURL("instance-up", iconSize);
				break;
           case ARCHIVE:
              	imgUrl = ELFunctions.getImageURL("instance-up", iconSize);
				break;
			default:
				imgUrl = ELFunctions.getImageURL("instance-unknown", iconSize);
				break;
			}
		}
		return imgUrl;
	}

	public CMInstance getInstance() {
		return instance;
	}

	public void setInstance(CMInstance instance) {
		this.instance = instance;
	}

	public String getInstanceName() {
		if (this.instance != null) {
			return this.instance.getInstanceName();
		}
		return ELFunctions.getLabel(SQLCMI18NStrings.N_A);
	}

    public String getAuditedDatabaseValue() {
        if (this.instance != null) {
            String totalDatabaseCount = String.valueOf(this.instance.getTotalDatabaseCount());
            if (instance.getTotalDatabaseCount() == 0) {
                totalDatabaseCount = ELFunctions.getLabel(SQLCMI18NStrings.N_A);
            }
            return ELFunctions.getLabelWithParams(SQLCMI18NStrings.AUDITED_DATABASES_VALUE,
                    instance.getAuditedDatabaseCount(), totalDatabaseCount);
        }
        return ELFunctions.getLabel(SQLCMI18NStrings.N_A);
    }

    public String getAuditedDatabasesCountValue() {
        if (this.instance != null) {
            return ""+instance.getAuditedDatabaseCount();
        }
        return ELFunctions.getLabel(SQLCMI18NStrings.N_A);
    }

    private void loadInstance(){
        reloadInstance(days);
    }

    public void reloadInstance(int days){
        try{
            instance = InstancesFacade.getInstanceDetails(instanceId, days);
        } catch (NumberFormatException e) {
            logger.info(e.getMessage());
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_INSTANCE_DETAIL);
        }
        BindUtils.postNotifyChange(null, null, InstanceOverview.this, "statusText");
        BindUtils.postNotifyChange(null, null, InstanceOverview.this, "instance");
    }

    @Command("openDBView")
    public void openDatabaseViewPageClick(@BindingParam("databaseId") long databaseId) {
        Executions.sendRedirect(WebUtil
            .buildPathRelativeToCurrentProduct("databaseView/" + instanceId + "/" + databaseId));
    }

    @Command("addDatabase")
    public void addDatabaseClick(@BindingParam("id") String id, @BindingParam("target") Combobox target) {
    	target.setValue("Select");
    	if(id.equals("ADDDATABASE"))
    	{
            ServerWizardViewModel.showAddDatabasesOnlyWizard(instance, this);
    	}
    	if(id.equals("SEARCHCOL"))
    	{
    		ColumnSearchViewModel.showColumnSearch(instance);
    	}
    	if(id.equals("Import"))
	    {
	    	Sessions.getCurrent().setAttribute("currentInstanceName", getInstanceName());
	    	Window window = (Window) Executions.createComponents(
	    			InstanceOverview.ZUL_URL, null,
					null);
			window.doHighlighted();
		}
    		
    }

    @Command("statusTextClick")
    public void statusTextClick() {
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("auditedInstance"));
    }

    public String getAuditedPrivilegedUsersActivitiesLabel() {
        if (instance == null) {
            return EMPTY_STRING;
        }
        String label = ELFunctions.getLabel(SQLCMI18NStrings.PRIVILEGED_USER_TITLE);
        return String.format(label, instance.getPrivilegedUsersCount());
    }

    @Override
    public void onCancel() {
        // do nothing
    }

    @Override
    public void onFinish() {
        loadDatabases();
        BindUtils.postNotifyChange(null, null, this, "databaseList");
    }
}
