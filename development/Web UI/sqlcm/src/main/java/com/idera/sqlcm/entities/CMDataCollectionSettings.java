package com.idera.sqlcm.entities;

public class CMDataCollectionSettings {
    private long collectionInterval;
    private long keepDataFor;

    public long getCollectionInterval() {
        return collectionInterval;
    }

    public void setCollectionInterval(long collectionInterval) {
        this.collectionInterval = collectionInterval;
    }

    public long getKeepDataFor() {
        return keepDataFor;
    }

    public void setKeepDataFor(long keepDataFor) {
        this.keepDataFor = keepDataFor;
    }
}
