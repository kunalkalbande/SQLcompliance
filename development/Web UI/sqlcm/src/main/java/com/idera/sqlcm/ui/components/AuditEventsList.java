package com.idera.sqlcm.ui.components;

import org.zkoss.zk.ui.HtmlMacroComponent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Listbox;
import org.zkoss.zul.ext.Paginal;

import java.util.ArrayList;
import java.util.List;

public class AuditEventsList extends HtmlMacroComponent {
    private static final long serialVersionUID = 1L;

    private static final String PAGING_MOLD = "paging";

    @Wire
    private Listbox auditEventsListbox;

    @Wire
    private Paginal paginal;

    private AnnotateDataBinder dataBinder;

    private List<InstanceListColumns> columns = new ArrayList<InstanceListColumns>();

    public enum InstanceListColumns {
        CATEGORY, EVENT, DATE, TIME, LOGIN, TARGET_OBJECT, DATABASE
    }

    public AuditEventsList() {
        setMacroURI("events/auditEventsList.zul");
        compose();
        // sets the HTML tags that do NOT trigger a selection of the row
        auditEventsListbox.setNonselectableTags("<div><tr><td><a><img>");
    }

    public List<InstanceListColumns> getColumns() {
        return columns;
    }

    public void setColumns(List<InstanceListColumns> columns) {

        if (columns == null) {
            this.columns = new ArrayList<InstanceListColumns>();
        } else {
            this.columns = columns;
        }
    }

    @Override
    public String getMold() {
        return auditEventsListbox.getMold();
    }

    @Override
    public void setMold(String mold) {
        auditEventsListbox.setMold(mold);

        updatePagingMoldBindings(PAGING_MOLD.equals(mold));

        loadComponent();
    }

    public void setAnnotateDataBinder(AnnotateDataBinder annotateDataBinder) {
        dataBinder = annotateDataBinder;

        updatePagingMoldBindings(PAGING_MOLD.equals(getMold()));
    }

    public AnnotateDataBinder getAnnotateDataBinder() {
        return dataBinder;
    }
    
    private void updatePagingMoldBindings(boolean pagingMold) {
        if (dataBinder != null) {
            dataBinder.bindBean("pagingMold", pagingMold);

            if (pagingMold) {
                auditEventsListbox.setPaginal(paginal);
            }
        }
    }

    private void loadComponent() {
        if (dataBinder != null) {
            dataBinder.loadComponent(auditEventsListbox);
        }
    }

    public Listbox getAuditEventsListbox(){
        return auditEventsListbox;
    }
}
