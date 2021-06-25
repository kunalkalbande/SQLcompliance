package com.idera.sqlcm.entities;

import java.util.List;

public class CMManagedInstances {
    private List<CMManagedInstance> managedInstances;
    private long totalCount;

    public List<CMManagedInstance> getManagedInstances() {
        return managedInstances;
    }

    public void setManagedInstances(List<CMManagedInstance> managedInstances) {
        this.managedInstances = managedInstances;
    }

    public long getTotalCount() {
        return totalCount;
    }

    public void setTotalCount(long totalCount) {
        this.totalCount = totalCount;
    }
}
