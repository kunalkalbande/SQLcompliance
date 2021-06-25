package com.idera.sqlcm.ui.components.filter.elements;

import com.idera.sqlcm.ui.components.VisualFilter;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.CheckEvent;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Div;

public class FilterCheckBox extends Div implements FilterElement {

    public static final String ON_CHECK = "onCheckFilter";
    private static final long serialVersionUID = 1L;

    @Wire
    private Checkbox filterCheckbox;

    private boolean checked;
    private String value;
    private String parentFilterId;
    private VisualFilter visualFilter;

    public FilterCheckBox() {
        Executions.createComponents("~./sqlcm/components/filter/filter-checkbox.zul", this, null);
        Selectors.wireComponents(this, this, false);
        Selectors.wireEventListeners(this, this);
    }

    public boolean isChecked() {
        return checked;
    }

    public void setChecked(boolean checked) {
        this.checked = checked;
        filterCheckbox.setChecked(checked);
    }

    public String getValue() {
        return filterCheckbox.getValue();
    }

    public void setValue(String value) {
        this.value = value;
        this.filterCheckbox.setValue(value);
        this.filterCheckbox.setLabel(value);
    }

    public String getParentFilterId() {
        return parentFilterId;
    }

    public void setParentFilterId(String parentFilterId) {
        this.parentFilterId = parentFilterId;
    }

    @Override
    public void removeFilter() {
        setChecked(false);
        Events.postEvent(this, new CheckBoxFilterEditEvent(ON_CHECK, false));
    }

    @Override
    public void cleanFilter() {
        setChecked(false);
    }

    @Listen("onCheck = #filterCheckbox")
    public void checkboxChecked(CheckEvent event) {
        setChecked(event.isChecked());
        Events.postEvent(this, new CheckBoxFilterEditEvent(ON_CHECK, isChecked()));
    }


    public class CheckBoxFilterEditEvent extends Event {
        private static final long serialVersionUID = 1L;

        public CheckBoxFilterEditEvent(String eventName, boolean isChecked) {
            super(eventName, FilterCheckBox.this);
            setChecked(isChecked);
        }

        public String getFilterId() {
            return getParentFilterId();
        }

        public String getThisId() {
            return getId();
        }

        public boolean isFilterChecked() {
            return isChecked();
        }

        public void clearSetValue() {
            cleanFilter();
        }
    }

}
