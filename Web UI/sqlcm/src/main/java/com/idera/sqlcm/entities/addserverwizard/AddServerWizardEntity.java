package com.idera.sqlcm.entities.addserverwizard;

import com.idera.sqlcm.entities.RegulationType;
import com.idera.sqlcm.ui.dialogs.addserverwizard.ServerWizardViewModel;
import com.idera.sqlcm.wizard.IWizardEntity;

import java.util.Map;

public class AddServerWizardEntity implements IWizardEntity {

    private ServerConfigEntity serverConfigEntity;
    private AddServerEntity addServerEntity;

    private boolean isAuditDatabasesFlag;
    private AddInstanceResult addInstanceResult;

    private ServerRegisteredStatusInfo serverRegisteredStatusInfo;

    public ServerRegisteredStatusInfo getServerRegisteredStatusInfo() {
        return serverRegisteredStatusInfo;
    }

    public void setServerRegisteredStatusInfo(ServerRegisteredStatusInfo serverRegisteredStatusInfo) {
        this.serverRegisteredStatusInfo = serverRegisteredStatusInfo;
    }

    /**
     * Just for wizard internal usage. This property is used inside
     * {@link ServerWizardViewModel#doSave(AddServerWizardEntity)} method to create
     * {@link ServerConfigEntity#regulationSettings} property
     * as AddDatabases REST API method required
     */
    private Map<String, RegulationType> selectedRegulationTypes;

    public ServerConfigEntity getServerConfigEntity() {
        return serverConfigEntity;
    }

    public void setServerConfigEntity(ServerConfigEntity serverConfigEntity) {
        this.serverConfigEntity = serverConfigEntity;
    }

    public AddServerEntity getAddServerEntity() {
        return addServerEntity;
    }

    public void setAddServerEntity(AddServerEntity addServerEntity) {
        this.addServerEntity = addServerEntity;
    }

    public Map<String, RegulationType> getSelectedRegulationTypes() {
        return selectedRegulationTypes;
    }

    public void setSelectedRegulationTypes(Map<String, RegulationType> selectedRegulationTypes) {
        this.selectedRegulationTypes = selectedRegulationTypes;
    }

    public boolean isAuditDatabasesChecked() {
        return isAuditDatabasesFlag;
    }

    public void setIsAuditDatabasesFlag(boolean isAuditDatabasesFlag) {
        this.isAuditDatabasesFlag = isAuditDatabasesFlag;
    }

    public AddInstanceResult getAddInstanceResult() {
        return addInstanceResult;
    }

    public void setAddInstanceResult(AddInstanceResult addInstanceResult) {
        this.addInstanceResult = addInstanceResult;
    }
}
