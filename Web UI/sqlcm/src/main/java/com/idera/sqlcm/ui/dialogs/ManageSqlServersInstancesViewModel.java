package com.idera.sqlcm.ui.dialogs;

import com.idera.common.rest.RestException;
import com.idera.server.web.ELFunctions;
import com.idera.server.web.component.zul.grid.ExtListheader;
import com.idera.sqlcm.entities.CMEntity;
import com.idera.sqlcm.entities.CMManagedInstance;
import com.idera.sqlcm.entities.CMManagedInstances;
import com.idera.sqlcm.entities.CMPageSortRequest;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.preferences.CommonGridPreferencesBean;
import com.idera.sqlcm.ui.preferences.PreferencesUtil;
import com.idera.sqlcm.utils.SQLCMConstants;

import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.NotifyChange;
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
import org.zkoss.zul.A;
import org.zkoss.zul.Image;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.Listhead;
import org.zkoss.zul.Menupopup;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Spinner;
import org.zkoss.zul.Window;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

public class ManageSqlServersInstancesViewModel {
    public static final String ZUL_URL = "~./sqlcm/dialogs/manageSQLServerInstances.zul";
    private static final String DEFAULT_SORT_COLUMN = "instance";
    public static final String UPDATE_MANAGE_INSTANCE_LIST_EVENT = "update-cm-manage-instance-list-event";

    @Wire
    Listbox instancesListBox;
    @Wire
    Spinner listBoxRowsBox;
    @Wire
    Paging listBoxPageId;
    @Wire
    A editProperties;
    @Wire
    A editCredentials;
    @Wire
    protected Listhead entitiesListBoxHead;

    private long recordCount;

    private int activePage = SQLCMConstants.DEFAULT_PAGE;

    private Integer pageSize = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

    private int sortDirection = SQLCMConstants.SORT_ASCENDING;

    private String sortColumn = DEFAULT_SORT_COLUMN;
    
    private long fileSize;
    
    private String help;
    private ListModelList<CMManagedInstance> instancesModelList;
    private ListModelList<CMEntity> selectedEntities = new ListModelList<>();
    private Set<CMEntity> pickedInstanceSet = new HashSet<CMEntity>();
    private int prevPageSize;
    private String preferencesSessionVariableName = CommonGridPreferencesBean.SESSION_VARIABLE_NAME;
    private int rowsCount = SQLCMConstants.DEFAULT_ROW_GRID_COUNT;

    public static void showManageSqlServersInstancesDialog() {
        Window window = (Window) Executions.createComponents(ZUL_URL, null, null);
        window.doHighlighted();
    }

    public String getHelp() {
        return help;
    }

    public ListModelList<CMManagedInstance> getInstancesModelList() {
        return instancesModelList;
    }

    public long getFileSize() {
		return fileSize;
	}

	public void setFileSize(long fileSize) {
		this.fileSize = fileSize;
	}

	public ListModelList<CMEntity> getSelectedEntities() {
        return selectedEntities;
    }

    public void setSelectedEntities(ListModelList<CMEntity> selectedEntities) {
        this.selectedEntities = selectedEntities;
    }

    public Integer getPageSize() {
        return pageSize;
    }

    public void setPageSize(Integer pageSize) {
        this.pageSize = pageSize;
    }

    public void setActivePage(int selectedPage) {
        activePage = selectedPage + 1;
        initialiseInstancesModelList();
    }

    public long getTotalSize() {
        return recordCount;
    }

    public int getActivePage() {
        return SQLCMConstants.DEFAULT_PAGE;
    }

    @AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
        Selectors.wireComponents(view, this, false);
        help = "http://wiki.idera.com/x/fwC5Ag ";

        initialiseInstancesModelList();
        instancesListBox.setNonselectableTags("<div><tr><td><a><img>");
        subscribeForSortEvent();
        subscribeToUpdateEvent();        
    }

    @Command
    public void doCheck(@BindingParam("checked") boolean isPicked, @BindingParam("picked") CMManagedInstance item) {
        if (isPicked) {
            pickedInstanceSet.add(item);
            if (pickedInstanceSet.size() > 1) {
                updatePopupForBulkOps();
            }
        } else {
            pickedInstanceSet.remove(item);
            removeBulkOps(item);
        }
        checkAvailableActions();
    }

    private void checkAvailableActions() {
        if (pickedInstanceSet.size() > 1) {
            editCredentials.setDisabled(false);
            editProperties.setDisabled(true);
        } else if (pickedInstanceSet.size() == 1) {
            editCredentials.setDisabled(true);
            editProperties.setDisabled(false);
            removeBulkOps(pickedInstanceSet.iterator().next());
        } else {
            editCredentials.setDisabled(true);
            editProperties.setDisabled(true);
        }
    }

    @Command
    @NotifyChange("pickedInstanceSet")
    public void doCheckAll() {
        if (!selectedEntities.isEmpty()) {
            pickedInstanceSet.clear();
            pickedInstanceSet.addAll(instancesModelList.getSelection());
            if (instancesModelList.getSelection().size() > 1) {
                updatePopupForBulkOps();
            }
            editCredentials.setDisabled(false);
        } else {
            for (CMEntity item : pickedInstanceSet) {
                removeBulkOps(item);
            }
            editCredentials.setDisabled(true);
        }
    }

    @Command
    public void editProperties(@BindingParam("instanceId") Long instanceId) {
        if (instanceId == null) {
            InstanceDetailsPropertiesViewModel.showInstanceDetailsPropertiesDialog(pickedInstanceSet.iterator().next().getId());
        } else {
            InstanceDetailsPropertiesViewModel.showInstanceDetailsPropertiesDialog(instanceId);
        }
    }

    @Command
    public void editCredentials(@BindingParam("instanceId") Long instanceId) {
        List<Long> instanceIdList = new ArrayList<>();
        if (instanceId == null) {
            for (CMManagedInstance instance : instancesModelList.getSelection()) {
                instanceIdList.add(instance.getId());
            }
        } else {
            instanceIdList.add(instanceId);
        }
        EditCredentialViewModel.showEditCredentialDialog(instanceIdList);
    }

    @Command("setGridRowsCount")
    public void setGridRowsCount() {
        try {
            int tmpPageSize = listBoxRowsBox.getValue();
            if (tmpPageSize > 100) {
                Clients.showNotification(ELFunctions.getLabel(SQLCMI18NStrings.PAGE_SIZE_ERROR), "warning",
                        listBoxRowsBox, "end_center", 3000);
                tmpPageSize = 100;
                listBoxRowsBox.setValue(tmpPageSize);
            }
            if (tmpPageSize != pageSize) {
                activePage = SQLCMConstants.DEFAULT_PAGE;
                BindUtils.postNotifyChange(null, null, this, "activePage");
            }
            listBoxPageId.setPageSize(tmpPageSize);
            prevPageSize = tmpPageSize;
            pageSize = tmpPageSize;
            int value=0;
            if(instancesModelList !=null && !instancesModelList.isEmpty())
            {
            	
            	
            		value=instancesModelList.size();
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
        initialiseInstancesModelList();
    }

    @Command
    public void closeDialog(@BindingParam("comp") Window x) {
        x.detach();
    }

    private void initialiseInstancesModelList() {
        CMPageSortRequest request = new CMPageSortRequest();
        request.setPage(activePage);
        request.setPageSize(pageSize);
        request.setSortDirection(sortDirection);
        request.setSortColumn(sortColumn);
        instancesModelList = new ListModelList<>();
        try {
            CMManagedInstances managedInstances = InstancesFacade.getManagedInstances(request);
            instancesModelList.clear();
            instancesModelList.addAll(managedInstances.getManagedInstances());
            instancesModelList.setMultiple(true);
            recordCount = managedInstances.getTotalCount();
            BindUtils.postNotifyChange(null, null, this, "instancesModelList");
            setFileSize(recordCount);
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_LOAD_INSTANCE_LIST);
        }
    }

    private void updatePopupForBulkOps() {
        StringBuilder parameters = new StringBuilder();
        for (CMEntity item : pickedInstanceSet) {
            parameters.append(item.getId()).append(',');
        }
        parameters.setLength(parameters.length() - 1);
        for (CMEntity item : pickedInstanceSet) {
            A actionLink = (A) instancesListBox.getFellow("makeActionLink" + item.getId());
            setMenuPopupForBulkOps(actionLink, parameters.toString());
            Image actionLinkImage = (Image) actionLink.getFirstChild();
            actionLinkImage.setSrc(ELFunctions.getImageURLWithoutSize(SQLCMConstants.ACTION_ICON_BULK));
        }
    }

    private void setMenuPopupForBulkOps(A actionLink, final String parameters) {
        actionLink.setPopup((Menupopup) actionLink.getFellow("bulkActionsMenuPopup"));
       /* Component removeInstancesMenuItem = actionLink.getFellow("removeInstancesMenuItem");
        WebUtil.removeAllOnClickEventListeners(removeInstancesMenuItem, "onClick");
        removeInstancesMenuItem.addEventListener("onClick", new EventListener<Event>() {
            @Override
            public void onEvent(Event event) throws Exception {
                remove(parameters);
            }
        });*/
    }

    private void removeBulkOps(CMEntity item) {
        A actionLink = (A) instancesListBox.getFellow("makeActionLink" + item.getId());
        actionLink.setPopup((Menupopup) actionLink.getFellow("actionsMenuPopup" + item.getId()));
        Image actionLinkImage = (Image) actionLink.getFirstChild();
        actionLinkImage.setSrc(ELFunctions.getImageURLWithoutSize(SQLCMConstants.ACTION_ICON_SINGLE));
    }

    protected void subscribeForSortEvent() {
        Collection<Component> heads = entitiesListBoxHead.getChildren();
        for (Component headerComponent : heads) {
            headerComponent.addEventListener(Events.ON_SORT, new EventListener<SortEvent>() {
                public void onEvent(SortEvent event) throws Exception {
                    String sortParam = ((ExtListheader) event.getTarget()).getValue();
                    if (sortParam == null || sortParam.trim().isEmpty()) {
                        throw new RuntimeException(" Invalid column value that is used as sort parameter! ");
                    }
                    sortColumn = sortParam;
                    if (event.isAscending()) {
                        sortDirection = SQLCMConstants.SORT_ASCENDING;
                    } else {
                        sortDirection = SQLCMConstants.SORT_DESCENDING;
                    }
                    initialiseInstancesModelList();
                }
            });
        }
    }

    protected void subscribeToUpdateEvent() {
        EventQueue<Event> eq = EventQueues.lookup(UPDATE_MANAGE_INSTANCE_LIST_EVENT, EventQueues.APPLICATION, true);
        eq.subscribe(new EventListener<Event>() {
            public void onEvent(Event event) throws Exception {
                refreshInstanceList();
            }
        });
    }

    private void refreshInstanceList() {
        instancesModelList.clear();
        selectedEntities.clear();
        pickedInstanceSet.clear();
        initialiseInstancesModelList();
        checkAvailableActions();
    }
}
