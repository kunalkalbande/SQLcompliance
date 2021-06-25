package com.idera.sqlcm.entities;

public class CMPageSortRequest {
    private long page;
    private long pageSize;
    private String sortColumn;
    private int sortDirection;

    public long getPage() {
        return page;
    }

    public void setPage(long page) {
        this.page = page;
    }

    public long getPageSize() {
        return pageSize;
    }

    public void setPageSize(long pageSize) {
        this.pageSize = pageSize;
    }

    public String getSortColumn() {
        return sortColumn;
    }

    public void setSortColumn(String sortColumn) {
        this.sortColumn = sortColumn;
    }

    public int getSortDirection() {
        return sortDirection;
    }

    public void setSortDirection(int sortDirection) {
        this.sortDirection = sortDirection;
    }
}
