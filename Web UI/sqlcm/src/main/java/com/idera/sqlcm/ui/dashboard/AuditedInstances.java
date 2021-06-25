package com.idera.sqlcm.ui.dashboard;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.common.grid.CommonFacade;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMServerAuditingRequest;
import com.idera.sqlcm.entities.CMUpgradeAgentResponse;
import com.idera.sqlcm.enumerations.Interval;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.facade.RefreshDurationFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.dialogs.EnterAgentCredentialViewModel;
import com.idera.sqlcm.ui.instances.InstanceIconURLConverter;
import com.idera.sqlcm.ui.preferences.CommonGridPreferencesBean;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.utils.SQLCMConstants;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.event.SortEvent;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Spinner;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.Map;
import java.util.TreeMap;

public class AuditedInstances {

    protected CommonFacade entityFacade;

    protected ListModelList<CMEntity> entitiesModel;

    protected Map<String, Object> emptyFilter = new TreeMap<>();

    protected String preferencesSessionVariableName = "AuditedInstancesDataBean";

    protected int rowsCount = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

    private int prevPageSize;

    private InstanceIconURLConverter iconURLConverter;

    private AlertLabelConverter alertLabelConverter;

    private int days = Interval.SEVEN_DAY.getDays();
    
    public int refreshDuration; // SQLCM 5.4 SQLCM-2172 start

    public int getRefreshDuration() {
		return refreshDuration;
	}

	public void setRefreshDuration(int refreshDuration) {
		this.refreshDuration = refreshDuration;
	}
    

    @Wire
    protected Listbox entitiesListBox;

    @Wire
    Paging listBoxPageId;

    @Wire
    Spinner listBoxRowsBox;

    int fileSize;
    
    
    public int getFileSize() {
		return fileSize;
	}

	public void setFileSize(int fileSize) {
		this.fileSize = fileSize;
	}

	public AuditedInstances() {
        entityFacade = new InstancesFacade();
        iconURLConverter = new InstanceIconURLConverter();
        alertLabelConverter = new AlertLabelConverter();
        refreshEntitiesList();
        subscribeToDashboardChangeInterval();
    }

    protected void refreshEntitiesList() {
        entitiesModel = new ListModelList<>(entityFacade.getAllEntities(emptyFilter));
        BindUtils.postNotifyChange(null, null, this, "entitiesModel");
    }

    @Command("refreshEvents")
    public void refreshEntitiesLists() {
        entitiesModel = new ListModelList<>(entityFacade.getAllEntities(emptyFilter));
        BindUtils.postNotifyChange(null, null, this, "*");
    }
    
    public ListModelList<CMEntity> getEntitiesModel() {
        return entitiesModel;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
     try{
		Selectors.wireComponents(view, this, false);
    	initPagination();
    	entitiesListBox.setNonselectableTags("<div><tr><td><a><img>");
    	subscribeForSortEvent();  
                	
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

    protected CommonGridPreferencesBean getGridPreferencesInSession() {
        return PreferencesUtil.getInstance().getGridPreferencesInSession(preferencesSessionVariableName);
    }

    protected void initPagination() {
        CommonGridPreferencesBean gp = getGridPreferencesInSession();
        if (gp.getGridRowsCount() > 0) {
            rowsCount = gp.getGridRowsCount();
        }
        prevPageSize = rowsCount;
        listBoxRowsBox.setValue(rowsCount);
        setGridRowsCount();
        entitiesListBox.setPaginal(listBoxPageId);
    }

    protected void subscribeForSortEvent() {
        Collection<Component> heads = entitiesListBox.getHeads();
        for (Component headerComponent : heads) {
            headerComponent.addEventListener(Events.ON_SORT, new EventListener<SortEvent>() {
                public void onEvent(SortEvent event) throws Exception {
                    PreferencesUtil.getInstance()
                        .setGridSortingPreferencesInSession(preferencesSessionVariableName, event.getTarget().getId(),
                            event.isAscending());
                }
            });
        }
    }

    protected void subscribeToDashboardChangeInterval() {
        EventQueue<Event> eq = EventQueues.lookup(DashboardViewModel.DASHBOARD_CHANGE_INTERVAL_EVENT, EventQueues.SESSION, true);
        eq.subscribe(new EventListener<Event>() {
            public void onEvent(Event event) throws Exception {
                days = (int) event.getData();
                refreshEntitiesList();
            }
        });
    }


    @Command("setGridRowsCount")
    public void setGridRowsCount() {
        try {
            int pageSize = listBoxRowsBox.getValue();
            if (pageSize > 100) {
                Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
                    listBoxRowsBox, "end_center", 3000);
                pageSize = 100;
                listBoxRowsBox.setValue(pageSize);
            }
            listBoxPageId.setPageSize(pageSize);
            prevPageSize = pageSize;
            int value=0;
            if(entitiesModel !=null && !entitiesModel.isEmpty())
            {
            	
            	
            		value=entitiesModel.size();
            		setFileSize(value);
            	
            }
            else
            {
            	setFileSize(0);
            }
            
        } catch (WrongValueException exp) {
            listBoxPageId.setPageSize(prevPageSize);
        }
        PreferencesUtil.getInstance().setGridPagingPreferencesInSession(preferencesSessionVariableName, listBoxPageId.getPageSize());
        BindUtils.postNotifyChange(null, null, AuditedInstances.this, "*");
    }

    public InstanceIconURLConverter getIconURLConverter() {
        return iconURLConverter;
    }

    public void setIconURLConverter(InstanceIconURLConverter iconURLConverter) {
        this.iconURLConverter = iconURLConverter;
    }

    public AlertLabelConverter getAlertLabelConverter() {
        return alertLabelConverter;
    }

    public void setAlertLabelConverter(AlertLabelConverter alertLabelConverter) {
        this.alertLabelConverter = alertLabelConverter;
    }

    @Command("openInstance")
    public void openInstance(@BindingParam("id") int id) {
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instanceView/" + id));
        generateRefreshEvent();
    }

    @Command
    public void enableAuditing(@BindingParam("instanceId") long instanceId,
                               @BindingParam("enable") boolean enable) {
        CMServerAuditingRequest cmServerAuditingRequest = new CMServerAuditingRequest();
        cmServerAuditingRequest.setServerIdList(new ArrayList<Long>(Arrays.<Long>asList(instanceId)));
        cmServerAuditingRequest.setEnable(enable);
        try {
            InstancesFacade.changeAuditingForServers(cmServerAuditingRequest);
            generateRefreshEvent();
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
        }
    }

    @Command
    public void updateAuditSettings(@BindingParam("instanceId") Long serverId) {
        try {
            String newStatus = InstancesFacade.updateAuditConfigurationForServer(serverId);
            generateRefreshEvent();
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPDATE_AUDIT_CONFIGURATION_FOR_SERVER);
        }
    }

    private void doUpdateAgent(final long instanceId, String login, String pass) {
        CMUpgradeAgentResponse response;
        try {
            response = InstancesFacade.upgradeAgent(instanceId, login, pass);
            if (response.isSuccess()) {
                WebUtil.showInfoBoxWithCustomMessage(response.getUpgradeStatusMessage());
            } else {
                if (response.getErrorMessage() != null) {
                    WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.MESSAGE_CAN_NOT_UPGRADE_AGENT, response.getErrorMessage());
                }
            }

        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_UPGRADE_AGENT);
        }
    }

    @Command
    public void upgradeAgent(@BindingParam("instanceId") final long instanceId) {


        EnterAgentCredentialViewModel.Listener agentCredentialDialog = new EnterAgentCredentialViewModel.Listener() {

            @Override
            public void onOk(String login, String pass) {
                doUpdateAgent(instanceId, login, pass);
            }

            @Override
            public void onCancel() {

            }
        };

        EnterAgentCredentialViewModel.show(agentCredentialDialog);

    }

    private void generateRefreshEvent(){
        EventQueue<Event> eq = EventQueues.lookup(DashboardViewModel.DASHBOARD_CHANGE_INTERVAL_EVENT, EventQueues.SESSION, false);
        if (eq != null) {
            eq.publish(new Event(DashboardViewModel.DASHBOARD_CHANGE_INTERVAL_EVENT, null, days));
        }
    }
}
