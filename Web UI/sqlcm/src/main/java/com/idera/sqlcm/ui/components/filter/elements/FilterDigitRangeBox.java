package com.idera.sqlcm.ui.components.filter.elements;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.utils.SQLCMConstants;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.Label;
import org.zkoss.zul.Spinner;

public class FilterDigitRangeBox extends Div implements FilterElement {

    public static final int EMPTY_VALUE = -1;
    public static final String ON_DIGIT_RANGE_CHANGE = "onDigitRangeFilterChanged";
    public static final String ON_DIGIT_RANGE_REMOVE = "onDigitRangeFilterRemoved";
    private static final long serialVersionUID = 1L;

    @Wire
    private Label filterFrom;
    @Wire
    private Intbox filterValueFrom;
    @Wire
    private Label filterTill;
    @Wire
    private Intbox filterValueTill;

    private String parentFilterId;
    private int _lastValueFrom = EMPTY_VALUE;
    private int _lastValueTill = EMPTY_VALUE;
    private String value;

    public FilterDigitRangeBox() {
        Executions.createComponents("~./sqlcm/components/filter/filter-digitrangebox.zul", this, null);
        Selectors.wireComponents(this, this, false);
        Selectors.wireEventListeners(this, this);
        filterFrom.setValue(ELFunctions.getLabel(SQLCMI18NStrings.FILTER_FROM));
        filterTill.setValue(ELFunctions.getLabel(SQLCMI18NStrings.FILTER_TO));
    }

    public int getValueFrom() {
        return (filterValueFrom.getValue() == null) ? EMPTY_VALUE : filterValueFrom.getValue();
    }

    public void setValueFrom(int value) {
        _lastValueFrom = value;
        this.filterValueFrom.setValue(value);
    }

    public int getValueTill() {
        return (filterValueTill.getValue() == null) ? EMPTY_VALUE : filterValueTill.getValue();
    }

    public void setValueTill(int value) {
        _lastValueTill = value;
        this.filterValueTill.setValue(value);
    }

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        String[] values =  value.split(SQLCMConstants.FILTER_RANGE_VALUES_SEP_REGEXP);
        if (values.length == 2) {
            setValueFrom(Integer.parseInt(values[0]));
            setValueTill(Integer.parseInt(values[1]));
        }
        this.value = value;
    }

    public String getParentFilterId() {
        return parentFilterId;
    }

    public void setParentFilterId(String parentFilterId) {
        this.parentFilterId = parentFilterId;
    }

    @Listen("onChange = #filterValueFrom; onOk = #filterValueFrom;")
    public void digitFromChanged(Event event) {
        if (_lastValueFrom != getValueFrom()) {
            if (getValueFrom() > getValueTill()) {
                setValueTill(getValueFrom());
            }
            _lastValueFrom = getValueFrom();
            _lastValueTill = getValueTill();
            Events.postEvent(this, new DigitRangeFilterChangeEvent(ON_DIGIT_RANGE_CHANGE));
        }
    }

    @Listen("onChange = #filterValueTill; onOk = #filterValueTill")
    public void digitTillChanged(Event event) {
        if (_lastValueTill != getValueTill()) {
            if (getValueFrom() > getValueTill()) {
                setValueFrom(getValueTill());
            }
            _lastValueFrom = getValueFrom();
            _lastValueTill = getValueTill();
            Events.postEvent(this, new DigitRangeFilterChangeEvent(ON_DIGIT_RANGE_CHANGE));
        }
    }

    @Override
    public void removeFilter() {
        Events.postEvent(this.getParent(), new DigitRangeFilterChangeEvent(ON_DIGIT_RANGE_REMOVE));
    }

    @Override
    public void cleanFilter() {
        _lastValueFrom = EMPTY_VALUE;
        _lastValueTill = EMPTY_VALUE;
        filterValueFrom.setText("");
        filterValueTill.setText("");
    }

    public class DigitRangeFilterChangeEvent extends Event {
        private static final long serialVersionUID = 1L;

        public DigitRangeFilterChangeEvent(String eventName) {
            super(eventName, FilterDigitRangeBox.this);
        }

        public String getFilterId() {
            return getParentFilterId();
        }

        public int getEnteredFromValue() {
            return getValueFrom();
        }

        public int getEnteredTillValue() {
            return getValueTill();
        }

        public void clearSetValue() {
            cleanFilter();
        }

    }

}
