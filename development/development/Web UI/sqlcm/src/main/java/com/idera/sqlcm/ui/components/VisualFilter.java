package com.idera.sqlcm.ui.components;

import com.idera.i18n.I18NStrings;
import com.idera.sqlcm.server.web.ELFunctions;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import org.zkoss.zk.ui.HtmlMacroComponent;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Image;
import org.zkoss.zul.Label;

public class VisualFilter extends HtmlMacroComponent {
    private static final String FILTER_ATTRIBUTE_KEY = "visual_filter_attribute";
    private static final String FILTER_VALUE_ATTRIBUTE_KEY = "visual_filter_value_attribute";
    private static final long serialVersionUID = 1L;

    @Wire
    private Div filterDiv;
    @Wire
    private Image filterTypeImage;
    @Wire
    private Label filterValueLabel;
    @Wire
    private Image filterRemoveImage;
    private Filter filter;
    private String filterValue;

    public VisualFilter(Filter filter, String filterValue) {
        setMacroURI("~./sqlcm/components/filter/visualFilter.zul");
        compose();
        this.filter = filter;
        this.filterValue = filterValue;
        updateFilter();
    }

    public static VisualFilter getVisualFilterFromEvent(Event event) {
        VisualFilter visualFilter = null;
        if ((event != null) && (event.getTarget() != null)) {
            visualFilter = (VisualFilter) event.getTarget().getParent().getParent().getParent();
            Object filter = visualFilter.getAttribute(FILTER_ATTRIBUTE_KEY);
            Object filterValue = visualFilter.getAttribute(FILTER_VALUE_ATTRIBUTE_KEY);
            if ((filter != null) && (filterValue != null) && (filter instanceof Filter) && (filterValue instanceof String)) {
                visualFilter.setFilter((Filter) filter);
                visualFilter.setFilterValue((String) filterValue);
            }
        }
        return visualFilter;
    }

    public Filter getFilter() {
        return filter;
    }

    public void setFilter(Filter filter) {
        this.filter = filter;
    }

    public String getFilterValue() {
        return filterValue;
    }

    public void setFilterValue(String filterValue) {
        this.filterValue = filterValue;
        filterValueLabel.setValue(filterValue);
        filterValueLabel.setTooltip(filterValue);
    }

    public void setOnClickEventListener(EventListener<Event> eventListener) {

        this.filterRemoveImage.addEventListener("onClick", eventListener);

        filterRemoveImage.setSclass(eventListener != null ? "hand-on-mouseover " + filterRemoveImage.getSclass() : "");

    }

    private void updateFilter() {
        if (filterValue.length() == 0) {
            filterValue = ELFunctions.getLabel(I18NStrings.NOT_SPECIFIED);
        }
        filterValueLabel.setValue(filterValue);
        filterValueLabel.setTooltip(filterValue);
        filterTypeImage.setSrc(ELFunctions.getImageURL(filter.getFilterType().getIconUrl(), "small"));
        filterTypeImage.setTooltip(ELFunctions.getLabel(filter.getFilterName()));
        filterTypeImage.setPopup(filter.getColumnId());


        this.setAttribute(FILTER_ATTRIBUTE_KEY, filter);
        this.setAttribute(FILTER_VALUE_ATTRIBUTE_KEY, filterValue);
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        if (!super.equals(o)) return false;

        VisualFilter that = (VisualFilter) o;

        if (filter != null ? !filter.equals(that.filter) : that.filter != null) return false;
        if (filterValue != null ? !filterValue.equals(that.filterValue) : that.filterValue != null) return false;

        return true;
    }

    @Override
    public int hashCode() {
        int result = filter != null ? filter.hashCode() : 0;
        result = 37 * result + (filterValue != null ? filterValue.hashCode() : 0);
        return result;
    }

    @Override
    public String toString() {
        return "VisualFilter{" +
                ", filterValue='" + filterValue + '\'' +
                ", filter=" + filter +
                '}';
    }
}
