package com.idera.sqlcm.ui.components.filter.elements;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.ui.components.VisualFilter;
import com.idera.sqlcm.utils.SQLCMConstants;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Datebox;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

public class FilterDateRangeBox extends Div implements FilterElement {

    public static final String ON_DATE_RANGE_CHANGE = "onDateRangeFilterChanged";
    public static final String ON_DATE_RANGE_REMOVE = "onDateRangeFilterRemoved";
    private static final long serialVersionUID = 1L;

    @Wire
    private Label filterDateFrom;
    @Wire
    private Datebox filterDateValueFrom;
    @Wire
    private Label filterDateTill;
    @Wire
    private Datebox filterDateValueTill;

    private String placeholder;
    private String parentFilterId;
    private Date _lastValueFrom = new Date();
    private Date _lastValueTill = new Date();
    private String value;
    private VisualFilter visualFilter;

    public FilterDateRangeBox() {
        Executions.createComponents("~./sqlcm/components/filter/filter-daterangebox.zul", this, null);
        Selectors.wireComponents(this, this, false);
        Selectors.wireEventListeners(this, this);
        filterDateFrom.setValue(ELFunctions.getLabel(SQLCMI18NStrings.FILTER_FROM));
        filterDateTill.setValue(ELFunctions.getLabel(SQLCMI18NStrings.FILTER_TO));
        filterDateValueFrom.setTimeZone(TimeZone.getDefault());
        filterDateValueTill.setTimeZone(TimeZone.getDefault());
    }

    public String getPlaceholder() {
        return placeholder;
    }

    public void setPlaceholder(String placeholder) {
        this.placeholder = placeholder;
        filterDateValueFrom.setPlaceholder(placeholder);
        filterDateValueTill.setPlaceholder(placeholder);
    }

    public Date getValueFrom() {
        return filterDateValueFrom.getValue();
    }

    public void setValueFrom(Date value) {
        _lastValueFrom = value;
        this.filterDateValueFrom.setValue(value);
    }

    public Date getValueTill() {
        return filterDateValueTill.getValue();
    }

    public void setValueTill(Date value) {
        _lastValueTill = value;
        this.filterDateValueTill.setValue(value);
    }

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        SimpleDateFormat dateFormat = new SimpleDateFormat(SQLCMConstants.FILTER_DATE_FORMAT);
        String[] values =  value.split(SQLCMConstants.FILTER_RANGE_VALUES_SEP_REGEXP);
        if (values.length == 2) {
            try {
                setValueFrom(dateFormat.parse(values[0]));
                setValueTill(dateFormat.parse(values[1]));
            } catch (ParseException e) {
                e.printStackTrace();
                return;
            }
        }
        this.value = value;
    }

    public String getParentFilterId() {
        return parentFilterId;
    }

    public void setParentFilterId(String parentFilterId) {
        this.parentFilterId = parentFilterId;
    }

    @Listen("onChange = #filterDateValueFrom; onOK = #filterDateValueFrom;")
    public void dateFromChanged(Event event) {
        if (_lastValueFrom == null || !_lastValueFrom.equals(getValueFrom())) {
            if (getValueFrom() == null) {
                setValueFrom(getValueTill());
            }
            if (getValueTill() == null || getValueFrom().getTime() > getValueTill().getTime()) {
                setValueTill(getValueFrom());
            }
            _lastValueFrom = getValueFrom();
            _lastValueTill = getValueTill();
            Events.postEvent(this, new DateRangeFilterChangeEvent(ON_DATE_RANGE_CHANGE));
        }
    }

    @Listen("onChange = #filterDateValueTill; onOK = #filterDateValueTill")
    public void dateTillChanged(Event event) {
        if (_lastValueTill == null || !_lastValueTill.equals(getValueTill())) {
            if (getValueTill() == null) {
                setValueTill(getValueFrom());
            }
            if (getValueFrom() == null || getValueFrom().getTime() > getValueTill().getTime()) {
                setValueFrom(getValueTill());
            }
            _lastValueFrom = getValueFrom();
            _lastValueTill = getValueTill();
            Events.postEvent(this, new DateRangeFilterChangeEvent(ON_DATE_RANGE_CHANGE));
        }
    }

    @Override
    public void removeFilter() {
        Events.postEvent(this.getParent(), new DateRangeFilterChangeEvent(ON_DATE_RANGE_REMOVE));
    }

    @Override
    public void cleanFilter() {
        setValueFrom(null);
        setValueTill(null);
        filterDateValueFrom.setText("");
        filterDateValueTill.setText("");
    }

    public class DateRangeFilterChangeEvent extends Event {
        private static final long serialVersionUID = 1L;

        public DateRangeFilterChangeEvent(String eventName) {
            super(eventName, FilterDateRangeBox.this);
        }

        public String getFilterId() {
            return getParentFilterId();
        }

        public Date getEnteredFromValue() {
            return getValueFrom();
        }

        public Date getEnteredTillValue() {
            return getValueTill();
        }

        public void clearSetValue() {
            cleanFilter();
        }

    }

}
