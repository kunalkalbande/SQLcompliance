/*package com.idera.sqlcm.ui.components.filter.elements;

public class FilterComboBox {

}
*/

package com.idera.sqlcm.ui.components.filter.elements;

import com.idera.sqlcm.server.web.ELFunctions;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Image;
import org.zkoss.zul.Textbox;

public class FilterComboBox extends Div implements FilterElement {

    public static final String ON_DEFAULT_TXT_CHANGE = "onNewTextFilterAdded";
    public static final String ON_EXISTING_TXT_CHANGE = "onExistingTextFilterChange";
    public static final String ON_EXISTING_REMOVE = "onTextFilterRemoved";
    private static final long serialVersionUID = 1L;
    public static int INDEX = 0;

    @Wire
    private Textbox filterTextValue;
    @Wire
    private Image filterTextImg;

    private boolean isDefault;
    private String placeholder;
    private String parentFilterId;
    private String previousValue = "";
    private String newValue = "";

    public FilterComboBox() {
        Executions.createComponents("~./sqlcm/components/filter/filter-combobox.zul", this, null);
        Selectors.wireComponents(this, this, false);
        Selectors.wireEventListeners(this, this);
    }

    public String getPlaceholder() {
        return placeholder;
    }

    public void setPlaceholder(String placeholder) {
        this.placeholder = placeholder;
        filterTextValue.setPlaceholder(placeholder);
    }

    public String getOldValue() {
        return previousValue;
    }

    public String getValue() {
        return filterTextValue.getValue().trim();
    }

    public void setValue(String value) {
        value = value.trim();
        previousValue = newValue;
        newValue = value;
        this.filterTextValue.setValue(value);
    }

    public boolean getIsDefault() {
        return isDefault;
    }

    public void setIsDefault(boolean isDefault) {
        this.isDefault = isDefault;
        if (isDefault) {
            filterTextImg.setSrc(ELFunctions.getImageURL("ok", "small"));
            filterTextValue.setValue("");
        } else {
            filterTextImg.setSrc(ELFunctions.getImageURL("cancel", "small"));
        }
    }

    public String getParentFilterId() {
        return parentFilterId;
    }

    public void setParentFilterId(String parentFilterId) {
        this.parentFilterId = parentFilterId;
        this.setId(parentFilterId + "_" + INDEX++);
    }

    @Listen("onChange = #filterTextValue; onOK = #filterTextValue")
    public void textChanged(Event event) {
        ((Textbox)event.getTarget()).setValue(getValue());
        if (!newValue.equals(getValue())) {
            if (isDefault) {
                Events.postEvent(this.getParent(), new TextFilterEditEvent(ON_DEFAULT_TXT_CHANGE));
            } else {
                Events.postEvent(this.getParent(), new TextFilterEditEvent(ON_EXISTING_TXT_CHANGE));
            }
            previousValue = newValue;
            newValue = getValue();
        }
    }

    @Override
    @Listen("onClick = #filterTextImg")
    public void removeFilter() {
        if (!isDefault) {
            Events.postEvent(this.getParent(), new TextFilterEditEvent(ON_EXISTING_REMOVE));
        }
    }

    @Override
    public void cleanFilter() {
        setValue("");
    }

    public void removeItself() {
        this.getParent().removeChild(this);
    }

    public class TextFilterEditEvent extends Event {
        private static final long serialVersionUID = 1L;

        public TextFilterEditEvent(String eventName) {
            super(eventName, FilterComboBox.this);
        }

        public String getFilterId() {
            return getParentFilterId();
        }

        public String getThisId() {
            return getId();
        }

        public String getPreviousValue() {
            return getOldValue();
        }

        public String getEnteredValue() {
            return getValue();
        }

        public void removeFilterTextBox() {
            removeItself();
        }

        public void clearSetValue() {
            cleanFilter();
        }
    }

}
