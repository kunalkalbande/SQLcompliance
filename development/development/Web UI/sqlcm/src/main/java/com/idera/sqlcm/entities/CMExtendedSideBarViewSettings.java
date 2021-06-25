package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMExtendedSideBarViewSettings extends CMSideBarViewSettings {

    Long selectedTableBAD;

    Long selectedColumnBAD;

    String selectedArchivedDatabase;

    public Long getSelectedTableBAD() {
        return selectedTableBAD;
    }

    public void setSelectedTableBAD(Long selectedTableBAD) {
        this.selectedTableBAD = selectedTableBAD;
    }

    public Long getSelectedColumnBAD() {
        return selectedColumnBAD;
    }

    public void setSelectedColumnBAD(Long selectedColumnBAD) {
        this.selectedColumnBAD = selectedColumnBAD;
    }

    public String getSelectedArchivedDatabase() {
        return selectedArchivedDatabase;
    }

    public void setSelectedArchivedDatabase(String selectedArchivedDatabase) {
        this.selectedArchivedDatabase = selectedArchivedDatabase;
    }

    @Override
    public String toString() {
        return "CMExtendedSideBarViewSettings{" +
            "selectedTableBAD=" + selectedTableBAD +
            ", selectedColumnBAD=" + selectedColumnBAD +
            ", selectedArchivedDatabase='" + selectedArchivedDatabase + '\'' +
            '}';
    }
}
