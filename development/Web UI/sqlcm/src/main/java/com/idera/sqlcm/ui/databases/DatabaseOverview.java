package com.idera.sqlcm.ui.databases;

import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.ListModelList;

import com.idera.common.rest.RestException;
import com.idera.sqlcm.Utils;
import com.idera.sqlcm.entities.CMAuditedActivity;
import com.idera.sqlcm.entities.CMDatabase;
import com.idera.sqlcm.entities.CMDatabaseAuditedActivity;
import com.idera.sqlcm.entities.CMInstance;
import com.idera.sqlcm.facade.DatabasesFacade;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.addRegulationGuidelineWizard.AddRegulationWizardViewModel;
import com.idera.sqlcm.wizard.AbstractWizardViewModel.WizardListener;

public class DatabaseOverview implements WizardListener {

    private static final Logger logger = Logger.getLogger(DatabaseOverview.class);

    public static final String CHANGE_AUDITING_STATE = "privilegedUsers";

    private CMInstance instance;

    Long instanceId;

    Long databaseId;
    private int refreshDuration; //SQLCM 5.4 SCM-9 Start
    
   	

    ListModelList<CMDatabase> databasesModel;

    List<CMDatabase> cmAuditedDatabases;

    DatabaseIconURLConverter iconURLConverter;

    CMDatabaseAuditedActivity cmDatabaseAuditedActivity;

    CMAuditedActivity cmAuditedActivity;

    public int getRefreshDuration() {
   		return refreshDuration;
   	}

   	public void setRefreshDuration(int refreshDuration) {
   		this.refreshDuration = refreshDuration;
   	}
    
    @Init
    public void init() {
    	try{
        instanceId = Utils.parseInstanceIdArg();
        databaseId = Utils.parseDatabaseIdArg();
        loadDatabasesModel();
        iconURLConverter = new DatabaseIconURLConverter();
        loadDatabaseAuditedActivity();
        initAuditedActivity();
        String refreshDuration= RefreshDurationFacade.getRefreshDuration();
		int refDuration=Integer.parseInt(refreshDuration);
		refDuration=refDuration*1000;
		setRefreshDuration(refDuration);
    	}
    	catch(Exception e)
    	{
    		e.printStackTrace();
    	}
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        subscribeToEvents();
    }

    public ListModelList<CMDatabase> getDatabasesModel() {
        return databasesModel;
    }

    public DatabaseIconURLConverter getIconURLConverter() {
        return iconURLConverter;
    }

    public CMAuditedActivity getCmAuditedActivity() {
        return cmAuditedActivity;
    }

    public Long getDatabaseId() {
        return databaseId;
    }

    @Command("refreshEvents")
    public void refreshEvent() {
        try {
            	loadDatabasesModel();
                iconURLConverter = new DatabaseIconURLConverter();
                loadDatabaseAuditedActivity();
                initAuditedActivity();
            } catch (Exception e) {
                WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_AUDITED_DATABASE_LIST);
            }
        BindUtils.postNotifyChange(null, null, this, "*");
    }
    
    private void initAuditedActivity() {
        cmAuditedActivity = new CMAuditedActivity();
        cmAuditedActivity.setRegulationGuidelines(cmDatabaseAuditedActivity.getRegulationGuidelinesString());
        cmAuditedActivity.setEnhancedDatabase(cmDatabaseAuditedActivity.getDatabaseAuditedActivitiesString());
        cmAuditedActivity.setBeforeAfter(cmDatabaseAuditedActivity.getBeforeAfterTables());
        cmAuditedActivity.setSensitiveColumns(cmDatabaseAuditedActivity.getSensitiveColumnsTablesString());
        cmAuditedActivity.setTrustedUsers(cmDatabaseAuditedActivity.getTrustedUsersString());
        cmAuditedActivity.setEventFilters(cmDatabaseAuditedActivity.getEventFiltersString());

    }

    protected void subscribeToEvents() {
        EventQueue<Event> eq = EventQueues.lookup(CHANGE_AUDITING_STATE, EventQueues.SESSION, true);
        eq.subscribe(new EventListener<Event>() {
            public void onEvent(Event event) throws RestException {
                refreshSideBar();
            }
        });

        eq = EventQueues.lookup(AuditedDatabase.DATABASE_VIEW_CHANGE_INTERVAL_EVENT, EventQueues.SESSION, true);
        eq.subscribe(new EventListener<Event>() {
            public void onEvent(Event event) throws RestException {
                refreshSideBar();
            }
        });
    }

    private void refreshSideBar() {
        loadDatabasesModel();
        loadDatabaseAuditedActivity();
        initAuditedActivity();
        BindUtils.postNotifyChange(null, null, DatabaseOverview.this, "databasesModel");
        BindUtils.postNotifyChange(null, null, DatabaseOverview.this, "environmentGrid");
        BindUtils.postNotifyChange(null, null, DatabaseOverview.this, "cmAuditedActivity");
    }

    private void loadDatabaseAuditedActivity() {
        try {
            cmDatabaseAuditedActivity = DatabasesFacade.getAuditedActivityForDatabase(instanceId, databaseId);
        } catch (RestException e) {
            cmDatabaseAuditedActivity = new CMDatabaseAuditedActivity();
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_DATABASE_AUDITED_ACTIVITY);
        }
    }

    private void loadDatabasesModel() {
        try {
            cmAuditedDatabases = DatabasesFacade.getAuditedDatabasesForInstance(instanceId);
            databasesModel = new ListModelList<>(cmAuditedDatabases);
        } catch (RestException e) {
            databasesModel = new ListModelList<>();
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_AUDITED_DATABASES);
        }
    }

    @Command("openEnhancedDatabaseView")
    public void openEnhancedDatabaseView(@BindingParam("id") String id) {
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("databaseView/" + instanceId + "/" + id));
    }
    
    @Command("addApplyRegulationClick")
    public void addApplyRegulationClick() {
		try {
			instance=InstancesFacade.getInstanceDetails(instanceId);
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
        AddRegulationWizardViewModel.showAddDatabaseWizard(instance, this);
        refreshEvent();
    }

	@Override
	public void onCancel() {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onFinish() {
		// TODO Auto-generated method stub
		
	}
}
