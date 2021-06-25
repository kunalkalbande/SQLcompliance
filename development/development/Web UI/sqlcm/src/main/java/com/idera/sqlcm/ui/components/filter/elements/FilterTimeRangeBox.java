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
import org.zkoss.zul.*;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

public class FilterTimeRangeBox extends Div implements FilterElement {

    public static final String ON_TIME_RANGE_CHANGE = "onTimeRangeFilterChanged";
    public static final String ON_TIME_RANGE_REMOVE = "onTimeRangeFilterRemoved";
    private static final long serialVersionUID = 1L;

    @Wire
    private Label filterTimeFrom;
    @Wire
    private Timebox filterTimeValueFrom;
    @Wire
    private Label filterTimeTill;
    @Wire
    private Timebox filterTimeValueTill;

    private String placeholder;
    private String parentFilterId;
    private Date _lastValueFrom = new Date();
    private Date _lastValueTill = new Date();
    private String value;
    private VisualFilter visualFilter;

    public FilterTimeRangeBox() {
        Executions.createComponents("~./sqlcm/components/filter/filter-timerangebox.zul", this, null);
        Selectors.wireComponents(this, this, false);
        Selectors.wireEventListeners(this, this);
        filterTimeFrom.setValue(ELFunctions.getLabel(SQLCMI18NStrings.FILTER_FROM));
        filterTimeTill.setValue(ELFunctions.getLabel(SQLCMI18NStrings.FILTER_TO));
        filterTimeValueFrom.setTimeZone(TimeZone.getDefault());
        filterTimeValueTill.setTimeZone(TimeZone.getDefault());
    }

    public String getPlaceholder() {
        return placeholder;
    }

    public void setPlaceholder(String placeholder) {
        this.placeholder = placeholder;
        filterTimeValueFrom.setPlaceholder(placeholder);
        filterTimeValueTill.setPlaceholder(placeholder);
    }

    public Date getValueFrom() {
        return filterTimeValueFrom.getValue();
    }

    public void setValueFrom(Date value) {
        _lastValueFrom = value;
        this.filterTimeValueFrom.setValue(value);
    }

    public Date getValueTill() {
        return filterTimeValueTill.getValue();
    }

    public void setValueTill(Date value) {
        _lastValueTill = value;
        this.filterTimeValueTill.setValue(value);
    }

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        SimpleDateFormat dateFormat = new SimpleDateFormat(SQLCMConstants.FILTER_DATE_TIME_FORMAT);
        String[] values =  value.split(SQLCMConstants.FILTER_RANGE_VALUES_SEP_REGEXP);
        if (values.length == 2) {
            try {
                SimpleDateFormat transformTimeFormat = new SimpleDateFormat(SQLCMConstants.FILTER_DATE_FORMAT);
                setValueFrom(dateFormat.parse(transformTimeFormat.format(new Date()) + " " + values[0]));
                setValueTill(dateFormat.parse(transformTimeFormat.format(new Date()) + " " + values[1]));
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

    @Listen("onChange = #filterTimeValueFrom; onOK = #filterTimeValueFrom;")
    public void dateFromChanged(Event event) {
        if (_lastValueFrom == null || !_lastValueFrom.equals(getValueFrom())) {
            if (getValueFrom() == null) {
                setValueFrom(getValueTill());
            }
            if (getValueTill() == null || getValueFrom().getTime() > getValueTill().getTime()) {
                SimpleDateFormat transformTimeFormat = new SimpleDateFormat(SQLCMConstants.FILTER_DATE_FORMAT);
                setValueTill(new Date(getValueFrom().getTime()));
            }
            _lastValueFrom = getValueFrom();
            _lastValueTill = getValueTill();
            Events.postEvent(this, new TimeRangeFilterChangeEvent(ON_TIME_RANGE_CHANGE));
        }
    }

    @Listen("onChange = #filterTimeValueTill; onOK = #filterTimeValueTill")
    public void timeTillChanged(Event event) {
        if (_lastValueTill == null || !_lastValueTill.equals(getValueTill())) {
            if (getValueTill() == null) {
                setValueTill(getValueFrom());
            }
            if (getValueFrom() == null || getValueFrom().getTime() > getValueTill().getTime()) {
                setValueFrom(new Date(getValueTill().getTime()));
            }
            _lastValueFrom = getValueFrom();
            _lastValueTill = getValueTill();
            Events.postEvent(this, new TimeRangeFilterChangeEvent(ON_TIME_RANGE_CHANGE));
        }
    }

    @Override
    public void removeFilter() {
        Events.postEvent(this.getParent(), new TimeRangeFilterChangeEvent(ON_TIME_RANGE_REMOVE));
    }

    @Override
    public void cleanFilter() {
        setValueFrom(null);
        setValueTill(null);
        filterTimeValueFrom.setText("");
        filterTimeValueTill.setText("");
    }

    public class TimeRangeFilterChangeEvent extends Event {
        private static final long serialVersionUID = 1L;

        public TimeRangeFilterChangeEvent(String eventName) {
            super(eventName, FilterTimeRangeBox.this);
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
