package com.idera.sqlcm.ui.instances;

import com.idera.common.rest.RestException;
import com.idera.cwf.ui.dialogs.CustomMessageBox;
import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.common.grid.CommonGridViewModel;
import com.idera.sqlcm.common.grid.CommonGridViewReport;
import com.idera.sqlcm.entities.*;
import com.idera.sqlcm.entities.instances.CMRemoveInstanceResponse;
import com.idera.sqlcm.enumerations.UpgradeAgentType;
import com.idera.sqlcm.facade.InstancesFacade;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.rest.SQLCMRestClient;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.server.web.session.SessionUtil;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.dialogs.AgentPropertiesViewModel;
import com.idera.sqlcm.ui.dialogs.EnterAgentCredentialViewModel;
import com.idera.sqlcm.ui.dialogs.ImportSqlServersViewModel;
import com.idera.sqlcm.ui.dialogs.InstanceDetailsPropertiesViewModel;
import com.idera.sqlcm.ui.dialogs.InstancePropertiesViewModel;
import com.idera.sqlcm.ui.dialogs.addserverwizard.ServerWizardViewModel;
import com.idera.sqlcm.ui.importAuditSetting.AddImportAuditWizardViewModel;
import com.idera.sqlcm.ui.instances.filters.InstanceFilter;
import com.idera.sqlcm.ui.instances.filters.InstanceFilterChild;
import com.idera.sqlcm.ui.instances.filters.InstancesFilters;
import com.idera.sqlcm.ui.instances.filters.InstancesOptionFilterValues;
import com.idera.sqlcm.ui.permissionsCheck.PermissionCheckViewModel;
import com.idera.sqlcm.utils.SQLCMConstants;
import com.idera.sqlcm.wizard.AbstractWizardViewModel;
import com.idera.sqlcm.wizard.ImportAbstractWizardViewModel.WizardListener;

import net.sf.jasperreports.engine.JRException;

import org.apache.commons.io.IOUtils;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Comboitem;
import org.zkoss.zul.Filedownload;
import org.zkoss.zul.Image;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Menupopup;
import org.zkoss.zul.Messagebox;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerException;



@Init(superclass = true)
@AfterCompose(superclass = true)
public class InstancesGridViewModel extends CommonGridViewModel implements AbstractWizardViewModel.WizardListener,
        InstancePropertiesViewModel.DialogListener, WizardListener {
	    @Command
	    public void editProperties(@BindingParam("checked") boolean isPicked, @BindingParam("picked") Instance item) {
	        InstancePropertiesViewModel.showInstancePropertiesDialog(item.getId());
	    }

	
	
    public final static String INSTANCES_SESSION_VARIABLE_NAME = "SQLcmInstancesSessionDataBean";

    private InstanceIconURLConverter iconURLConverter;
    private Set<CMEntity> pickedInstanceSet = new HashSet<CMEntity>();
    private ListModelList<CMEntity> selectedEntities;
    private String currentInstanceName;

    public ListModelList<CMEntity> getSelectedEntities() {
        return selectedEntities;
    }

    public void setSelectedEntities(ListModelList<CMEntity> selectedEntities) {
        this.selectedEntities = selectedEntities;
    }

   
    
    public InstancesGridViewModel() {
        entityFacade = new InstancesFacade();
        exportBeanClass = Instance.class;
        preferencesSessionVariableName = INSTANCES_SESSION_VARIABLE_NAME;
        iconURLConverter = new InstanceIconURLConverter();
    }

    public InstanceIconURLConverter getIconURLConverter() {
        return iconURLConverter;
    }

    public void setIconURLConverter(InstanceIconURLConverter iconURLConverter) {
        this.iconURLConverter = iconURLConverter;
    }

    protected ListModelList<Filter> getFiltersDefinition() {
        ListModelList<Filter> filtersDefinition = new ListModelList<>();

        Filter filter;

        filter = new InstanceFilter(InstancesFilters.STATUS);
        filter.addFilterChild(new InstanceFilterChild(InstancesOptionFilterValues.STATUS_OK));
        filter.addFilterChild(new InstanceFilterChild(InstancesOptionFilterValues.STATUS_ERROR));
        filtersDefinition.add(filter);

        filter = new InstanceFilter(InstancesFilters.INSTANCE_NAME);
        filtersDefinition.add(filter);

        filter = new InstanceFilter(InstancesFilters.STATUS_TEXT);
        filtersDefinition.add(filter);

        filter = new InstanceFilter(InstancesFilters.NUMBER_OF_AUDITED_DB);
        filtersDefinition.add(filter);

        filter = new InstanceFilter(InstancesFilters.SQL_SERVER_VERSION_EDITION);
        filtersDefinition.add(filter);

        filter = new InstanceFilter(InstancesFilters.AUDIT_STATUS);
        filter.addFilterChild(new InstanceFilterChild(InstancesOptionFilterValues.AUDIT_STATUS_ENABLED));
        filter.addFilterChild(new InstanceFilterChild(InstancesOptionFilterValues.AUDIT_STATUS_DISABLED));
        filtersDefinition.add(filter);

        filter = new InstanceFilter(InstancesFilters.LAST_AGENT_CONTACT);
        filtersDefinition.add(filter);

        return filtersDefinition;
    }

    protected CommonGridViewReport makeCommonGridViewReport() {
        InstancesGridViewReport instancesGridViewReport = new InstancesGridViewReport("Instances", "", "Instances Summary");
        instancesGridViewReport.setDataMapForListInstance(entitiesModel);
        return instancesGridViewReport;
    }

    @Command
    public void remove(@BindingParam("instance") Instance instance) {
        if (WebUtil.showConfirmationBoxWithIcon(SQLCMI18NStrings.MESSAGE_REMOVE_SQL_SERVER, SQLCMI18NStrings.TITLE_REMOVE_SQL_SERVER, "~./sqlcm/images/high-16x16.png", true)) {
            CustomMessageBox.UserResponse userResponse = WebUtil.showConfirmationBoxWithIconAndCancel(
                    Arrays.asList(SQLCMI18NStrings.MESSAGE_KEEP_SQL_SERVER_AUDIT_DATA_1, SQLCMI18NStrings.MESSAGE_KEEP_SQL_SERVER_AUDIT_DATA_2),
                    SQLCMI18NStrings.TITLE_KEEP_SQL_SERVER_AUDIT_DATA, "~./sqlcm/images/high-16x16.png");
            if (!userResponse.equals(CustomMessageBox.UserResponse.CANCEL)) {
                boolean deleteEventsDatabase = !userResponse.equals(CustomMessageBox.UserResponse.YES);

                try {
                    CMRemoveInstanceResponse result = InstancesFacade.removeAuditServers(Arrays.asList(instance.getId()), deleteEventsDatabase).get(0);

                    if (!result.isWasRemoved() && !result.getErrorMessage().isEmpty()) {
                        WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.CAN_NOT_REMOVE_AUDIT_SERVER, result.getErrorMessage());
                    }

                    if (!result.isWasAgentDeactivated()) {
                        WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_CAN_NOT_REMOVE_AGENT, instance.getInstanceName());
                    }

                    if (result.isWasRemoved()) {
                        WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_SERVER_REMOVED);
                        refreshEntitiesList();
                    }
                } catch (RestException e) {
                    WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_REMOVE_AUDIT_SERVER);
                }
            }
        }
    }

    @Command("removeSelected")
    public void removeSelected() {
        List<Long> parameters = new ArrayList<>();
        if(pickedInstanceSet.size()>0){
	        for (CMEntity item : pickedInstanceSet) {
	            parameters.add(item.getId());
	        }
	
	        if (WebUtil.showConfirmationBoxWithIcon(SQLCMI18NStrings.MESSAGE_REMOVE_SQL_SERVER, SQLCMI18NStrings.TITLE_REMOVE_SQL_SERVER, "~./sqlcm/images/high-16x16.png", true)) {
	            CustomMessageBox.UserResponse userResponse = WebUtil.showConfirmationBoxWithIconAndCancel(
	                    Arrays.asList(SQLCMI18NStrings.MESSAGE_KEEP_SQL_SERVER_AUDIT_DATA_1, SQLCMI18NStrings.MESSAGE_KEEP_SQL_SERVER_AUDIT_DATA_2),
	                    SQLCMI18NStrings.TITLE_KEEP_SQL_SERVER_AUDIT_DATA, "~./sqlcm/images/high-16x16.png");
	            if (!userResponse.equals(CustomMessageBox.UserResponse.CANCEL)) {
	                boolean deleteEventsDatabase = !userResponse.equals(CustomMessageBox.UserResponse.YES);
	
	                try {
	                    List<CMRemoveInstanceResponse> result = InstancesFacade.removeAuditServers(parameters, deleteEventsDatabase);
	
	                    List<String> removedInstanceNames = new ArrayList<>();
	
	                    for (CMRemoveInstanceResponse response: result) {
	                        if (response.isWasRemoved()) {
	                            removedInstanceNames.add(getInstanceById(response.getServerId()).getInstanceName());
	                        }
	                    }
	
	                    if (!removedInstanceNames.isEmpty()) {
	                        WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_REMOVE_INSTANCES_RESULT, com.idera.sqlcm.server.web.ELFunctions.listToString(removedInstanceNames));
	                        refreshEntitiesList();
	                        pickedInstanceSet.clear();
	                        selectedEntities.clear();
	                    } else {
	                        WebUtil.showErrorBox(new RestException(), SQLCMI18NStrings.CAN_NOT_REMOVE_AUDIT_SERVER);
	                    }
	                } catch (RestException e) {
	                    WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_REMOVE_AUDIT_SERVER);
	                }
	            }
	        }
        }
        
        else{
        	WebUtil.showInfoBoxWithCustomMessage("Please select an instance.");
        }
    }
    
    @Command
    @NotifyChange("pickedInstanceSet")
    public void doCheck(@BindingParam("checked") boolean isPicked, @BindingParam("picked") Instance item) {
        if (isPicked) {
            pickedInstanceSet.add(item);
            if (pickedInstanceSet.size() > 25) {
                updatePopupForBulkOps();
            }
        } else {
            pickedInstanceSet.remove(item);
            removeBulkOps(item);
        }
        
    }

    @Command
    @NotifyChange("pickedInstanceSet")
    public void doCheckAll() {
        if (!selectedEntities.isEmpty()) {

            pickedInstanceSet.clear();
            pickedInstanceSet.addAll(entitiesModel.getSelection());
            if (entitiesModel.getSelection().size() > 25) {
                updatePopupForBulkOps();
            }
        } else {
            pickedInstanceSet.clear();

            for (CMEntity item : pickedInstanceSet) {
                removeBulkOps(item);
            }
        }
    }

    public void removeBulkOps(CMEntity item) {
        A actionLink = (A) entitiesListBox.getFellow("makeActionLink" + item.getId());
        actionLink.setPopup((Menupopup) actionLink.getFellow("actionsMenuPopup" + item.getId()));
        Image actionLinkImage = (Image) actionLink.getFirstChild();
        actionLinkImage.setSrc(ELFunctions.getImageURLWithoutSize(SQLCMConstants.ACTION_ICON_SINGLE));
    }

    public void updatePopupForBulkOps() {
        StringBuilder parameters = new StringBuilder();
        for (CMEntity item : pickedInstanceSet) {
            parameters.append(item.getId()).append(',');
        }
        parameters.setLength(parameters.length() - 1);
        for (CMEntity item : pickedInstanceSet) {
            A actionLink = (A) entitiesListBox.getFellow("makeActionLink" + item.getId());
            setMenuPopupForBulkOps(actionLink);
            Image actionLinkImage = (Image) actionLink.getFirstChild();
            actionLinkImage.setSrc(ELFunctions.getImageURLWithoutSize(SQLCMConstants.ACTION_ICON_BULK));
        }
    }

    public void setMenuPopupForBulkOps(A actionLink) {
        actionLink.setPopup((Menupopup) actionLink.getFellow("bulkActionsMenuPopup"));
        Component removeInstancesMenuItem = actionLink.getFellow("removeInstancesMenuItem");
        WebUtil.removeAllOnClickEventListeners(removeInstancesMenuItem, "onClick");
        removeInstancesMenuItem.addEventListener("onClick", new EventListener<Event>() {
            @Override
            public void onEvent(Event event) throws Exception {
                removeSelected();
            }
        });
    }

    @Command
    public void enableAuditing(@BindingParam("instanceId") long instanceId,
                               @BindingParam("enable") boolean enable) {
        CMServerAuditingRequest cmServerAuditingRequest = new CMServerAuditingRequest();
        cmServerAuditingRequest.setServerIdList(new ArrayList<Long>(Arrays.<Long>asList(instanceId)));
        cmServerAuditingRequest.setEnable(enable);
        try {
            InstancesFacade.changeAuditingForServers(cmServerAuditingRequest);
            refreshEntitiesList();
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_DISABLE_ENABLE_AUDITING);
        }
    }

    @Command
    public void updateAuditSettings(@BindingParam("instanceId") Long serverId) {
        try {
            String newStatus = InstancesFacade.updateAuditConfigurationForServer(serverId);
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

    @Command
    public void checkAgentStatus(@BindingParam("instanceName") String instanceName) {
        try {
            CMCheckAgentStatusResult cmCheckAgentStatusResult = InstancesFacade.checkAgentStatus(instanceName);
            if (cmCheckAgentStatusResult.isActive()) {
                WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_CHECK_AGENT_STATUS_SUCCESS,
                        cmCheckAgentStatusResult.getAgentServer());
            } else {
                WebUtil.showInfoBox(SQLCMI18NStrings.MESSAGE_CHECK_AGENT_STATUS_FAILURE,
                        cmCheckAgentStatusResult.getAgentServer());
            }
        } catch (RestException e) {
            WebUtil.showErrorBox(e, SQLCMI18NStrings.FAILED_TO_CHECK_AGENT_STATUS);
        }
    }

    @Command("refreshData")
    public void refreshData() {
        Executions.getCurrent().sendRedirect("");
    }

    @Command
    public void openAddInstanceDialog() {
        ServerWizardViewModel.showAddInstanceWizard(this);
    }

    @Override
    public void onCancel() {
        // do nothing
    }

    @Override
    public void onFinish() {
    	BindUtils.postNotifyChange(null, null, this, "*");
    }


    @Command
    public void showInstancePropertiesDialog() {
    	if(pickedInstanceSet.size()==1){
    		Instance instance=(Instance)pickedInstanceSet.iterator().next();
    		InstancePropertiesViewModel.showInstancePropertiesDialog(instance.getId(), this);
    	}
    	else if(pickedInstanceSet.size()> 1){
    		WebUtil.showInfoBoxWithCustomMessage("Please select only one instance.");
    	}
    	else
    	{
    		WebUtil.showInfoBoxWithCustomMessage("Please select an instance.");
    	}
    }
    
    @Command
    public void showInstancePropertiesDialogGear(@BindingParam("instanceId") Long instanceId) {
        InstancePropertiesViewModel.showInstancePropertiesDialog(instanceId, this);
    }
  
    @Override
    public void onOk() { // callback method for instances dialog
        refreshEntitiesList();
    }

    @Command("openInstance")
    public void openInstance(@BindingParam("id") int id) {
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("instanceView/" + id));
    }

    @Command
    public void showAgentPropertiesDialog(@BindingParam("instanceId") Long instanceId) {
        AgentPropertiesViewModel.showAgentPropertiesDialog(instanceId);
    }

    protected Map<String, Boolean> collectColumnsVisibilityMap() {
        return null;
    }

    protected void retrieveColumnsVisibility(CMSideBarViewSettings alertsSettings){
    }

    private Instance getInstanceById(long id) {
        for (CMEntity entity: entitiesModel) {
            if (entity.getId() == id){
                return (Instance)entity;
            }
        }
        return null;
    }
       
	// SCM-387
	@Command
	public void ExportServerAuditSettings(@BindingParam("instanceName") String currentInstanceName) {
		InstancesFacade instanceFacade = new InstancesFacade();
		try {
			String exportedPath = instanceFacade.ExportServerAuditSettings(currentInstanceName);
			if (exportedPath.equalsIgnoreCase("failed")) {
				WebUtil.showInfoBoxWithCustomMessage("Export failed.");
			} else {

				WebUtil.showInfoBoxWithCustomMessage("File Exported Successfully to " + "'" + exportedPath + "'.");
			}
		} catch (Exception e) {
			WebUtil.showInfoBoxWithCustomMessage("Export failed.");
			e.printStackTrace();
		}
	}
   	
	@Command("export")
	public void export(@BindingParam("combo") Combobox combo,
			@BindingParam("selectedId") String selectedId) throws JRException,
			IOException, TransformerException, ParserConfigurationException {
		if (selectedId.equalsIgnoreCase("createPDFMenuItem")) {
			exportToPdf();
		}
		if (selectedId.equalsIgnoreCase("createXLSMenuItem")) {
			exportToExcel();
		}
		if (selectedId.equalsIgnoreCase("createXMLMenuItem")) {
			exportToXml();
		}
		combo.setValue("type");

	}
	
    @Command    
    public void showPermissionsCheckDialog(@BindingParam("instanceId") Long instanceId) {
        PermissionCheckViewModel.showPermissionsCheckDialog(instanceId);    
    }
    
    @Command("importAuditFile")
    public void importFile()
    {
    	AddImportAuditWizardViewModel.showWizard(this);
    }    
}
