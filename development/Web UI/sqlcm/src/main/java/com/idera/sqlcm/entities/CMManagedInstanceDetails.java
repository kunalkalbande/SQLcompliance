package com.idera.sqlcm.entities;

import java.util.List;

public class CMManagedInstanceDetails {
    private CMManagedInstance managedInstanceProperties;
    private List<String> owners;
    private List<String> locations;

    public CMManagedInstance getManagedInstanceProperties() {
        return managedInstanceProperties;
    }

    public void setManagedInstanceProperties(CMManagedInstance managedInstanceProperties) {
        this.managedInstanceProperties = managedInstanceProperties;
    }

    public List<String> getOwners() {
        return owners;
    }

    public void setOwners(List<String> owners) {
        this.owners = owners;
    }

    public List<String> getLocations() {
        return locations;
    }

    public void setLocations(List<String> locations) {
        this.locations = locations;
    }
}
