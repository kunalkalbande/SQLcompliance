package com.idera.sqlcm.ui.components;

import com.idera.server.web.ELFunctions;
import com.idera.sqlcm.i18n.SQLCMI18NStrings;
import com.idera.sqlcm.server.web.WebUtil;
import com.idera.sqlcm.ui.components.filter.FilterData;
import com.idera.sqlcm.ui.components.filter.elements.*;
import com.idera.sqlcm.ui.components.filter.model.Filter;
import com.idera.sqlcm.ui.components.filter.model.FilterChild;
import com.idera.sqlcm.utils.SQLCMConstants;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.HtmlMacroComponent;
import org.zkoss.zk.ui.event.CheckEvent;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.event.MouseEvent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.ListModelList;

import java.text.SimpleDateFormat;
import java.util.Date;

public class FilterSelector extends HtmlMacroComponent {
    public static final String CHANGE_FILTER_EVENT = "changeFilter";
    public static final String REMOVE_FILTER_LNK_ID_PREFIX = "removeFilter_";
    private static final long serialVersionUID = 1L;
    private ListModelList<Filter> filters;

    @Wire
    private A clearWholeFilter;
    @Wire
    private A applyFilterAfterChanges;
    @Wire
    private Checkbox applyFilterOnChange;

    private boolean isFilterChanged;
    private boolean isAnyFilterSet;
    private boolean isApplyFilterOnChange = true;
    private String source;

    public FilterSelector() {
        setMacroURI("~./sqlcm/components/filter/filter-empty.zul");
        compose();
    }

    protected void compose() {
        super.compose();
        addEventListeners();
    }

    public void setSource(String source) {
        this.source = source;
    }

    public ListModelList<Filter> getFilters() {
        return filters;
    }

    public void setFilters(ListModelList<Filter> filters) {
        this.filters = filters;
        setMacroURI("~./sqlcm/components/filter/filter.zul");
        isAnyFilterSet();
        this.recreate();
    }

    private void addEventListeners() {
        if (applyFilterAfterChanges != null) {
            applyFilterOnChange.setChecked(isApplyFilterOnChange);
        }
        if (clearWholeFilter != null) {
            clearWholeFilter.setVisible(isAnyFilterSet);
        }
        if (filters != null && filters.size() > 0) {
            addEventListenerForStyleChanges();
            addEventListenersForFilterMainControls();
            addEventListenersForFilterRemoveLinks();
            addEventListenersForFilterChanges();
            
           
        }
    }

    private void addEventListenerForStyleChanges() {
        Filter filter;
        int size=filters.size();
        for (int i = 0; i < size; i++) {
            filter = filters.get(i);
            Groupbox group = (Groupbox) getFellow("group_" + filter.getFilterId());
            group.addEventListener(Events.ON_OPEN, new EventListener<Event>() {
                public void onEvent(Event event) throws Exception {
                    Groupbox filterGroup = (Groupbox) event.getTarget();
                    filterGroup.getCaption().setSclass(" open-" + (filterGroup.isOpen() ? "true" : "false"));
                }
            });
        }
    }

    private void addEventListenersForFilterMainControls() {
        applyFilterOnChange.addEventListener(Events.ON_CHECK, new EventListener<CheckEvent>() {
            public void onEvent(CheckEvent event) throws Exception {
                filterUpdateModeChangeHandler(event);
                isApplyFilterOnChange = event.isChecked();
            }
        });

        applyFilterAfterChanges.addEventListener(Events.ON_CLICK, new EventListener<MouseEvent>() {
            public void onEvent(MouseEvent event) throws Exception {
                publishChangeFilterEvent();
            }
        });

        clearWholeFilter.addEventListener(Events.ON_CLICK, new EventListener<MouseEvent>() {
            public void onEvent(MouseEvent event) throws Exception {
                clearAllFilters();
            }
        });
    }

    private void addEventListenersForFilterRemoveLinks() {
        for (Filter filter : filters) {
            A removeFilterCtrl = (A) getFellow(REMOVE_FILTER_LNK_ID_PREFIX + filter.getFilterId());
            removeFilterCtrl.setLabel(
                    ELFunctions.getLabelWithParams(SQLCMI18NStrings.FILTER_REMOVE, ELFunctions.getLabel(filter.getFilterName())));
            removeFilterCtrl.addEventListener(Events.ON_CLICK, new EventListener<MouseEvent>() {
                public void onEvent(MouseEvent event) throws Exception {
                    clearFilterAfterLnkClick(event);
                }
            });
        }
    }

    private void addEventListenersForFilterChanges() {
        for (final Filter filter : filters) {
            switch (filter.getFilterType().getInputType()) {
                case OPTIONS:
                    for (final FilterChild filterChild : filter.getFilterChildren()) {
                        FilterCheckBox checkbox = (FilterCheckBox) getFellow(filterChild.getId());
                        checkbox.addEventListener(FilterCheckBox.ON_CHECK, new EventListener<FilterCheckBox.CheckBoxFilterEditEvent>() {
                            public void onEvent(FilterCheckBox.CheckBoxFilterEditEvent event) throws Exception {
                                optionFilterChange(event.getFilterId(), event.getThisId(), event.isFilterChecked());
                            }
                        });
                    }
                    break;
                case TEXT:
                    Component txtContainer = getFellow(filter.getFilterId());
                    txtContainer.addEventListener(FilterTextBox.ON_EXISTING_REMOVE, new EventListener<FilterTextBox.TextFilterEditEvent>() {
                        public void onEvent(FilterTextBox.TextFilterEditEvent event) throws Exception {
                            textFilterChange(event.getFilterId(), event.getEnteredValue(), "");
                            event.removeFilterTextBox();
                        }
                    });
                    txtContainer.addEventListener(FilterTextBox.ON_EXISTING_TXT_CHANGE, new EventListener<FilterTextBox.TextFilterEditEvent>() {
                        public void onEvent(FilterTextBox.TextFilterEditEvent event) throws Exception {
                            if (checkExist(event)) {
                                textFilterChange(event.getFilterId(), event.getPreviousValue(), event.getPreviousValue());
                                return;
                            }
                            textFilterChange(event.getFilterId(), event.getPreviousValue(), event.getEnteredValue());
                        }
                    });
                    txtContainer.addEventListener(FilterTextBox.ON_DEFAULT_TXT_CHANGE, new EventListener<FilterTextBox.TextFilterEditEvent>() {
                        public void onEvent(FilterTextBox.TextFilterEditEvent event) throws Exception {
                            if (checkExist(event)) {
                                event.clearSetValue();
                                return;
                            }
                            FilterTextBox newFilterBox = new FilterTextBox();
                            newFilterBox.setParentFilterId(event.getFilterId());
                            newFilterBox.setValue(event.getEnteredValue());
                            newFilterBox.setIsDefault(false);
                            event.getTarget().getParent().insertBefore(newFilterBox, event.getTarget().getParent().getLastChild());
                            event.clearSetValue();
                            textFilterChange(event.getFilterId(), "", newFilterBox.getValue());
                        }
                    });
                    break;
                case COMBO:
                    Component comboContainer = getFellow(filter.getFilterId());
                    comboContainer.addEventListener(FilterTextBox.ON_EXISTING_REMOVE, new EventListener<FilterComboBox.TextFilterEditEvent>() {
                        public void onEvent(FilterComboBox.TextFilterEditEvent event) throws Exception {
                            textFilterChange(event.getFilterId(), event.getEnteredValue(), "");
                            event.removeFilterTextBox();
                        }
                    });
                    comboContainer.addEventListener(FilterComboBox.ON_EXISTING_TXT_CHANGE, new EventListener<FilterComboBox.TextFilterEditEvent>() {
                        public void onEvent(FilterComboBox.TextFilterEditEvent event) throws Exception {
                            if (checkExist(event)) {
                                textFilterChange(event.getFilterId(), event.getPreviousValue(), event.getPreviousValue());
                                return;
                            }
                            textFilterChange(event.getFilterId(), event.getPreviousValue(), event.getEnteredValue());
                        }
                    });
                    comboContainer.addEventListener(FilterComboBox.ON_DEFAULT_TXT_CHANGE, new EventListener<FilterComboBox.TextFilterEditEvent>() {
                        public void onEvent(FilterComboBox.TextFilterEditEvent event) throws Exception {
                            if (checkExist(event)) {
                                event.clearSetValue();
                                return;
                            }
                            FilterComboBox newFilterBox = new FilterComboBox();
                           newFilterBox.setParentFilterId(event.getFilterId());
                            newFilterBox.setValue(event.getEnteredValue());
                            textFilterChange(event.getFilterId(), "", newFilterBox.getValue());
                        }
                    });
                    break;
                case DIGIT_RANGE:
                    Component digitRangeContainer = getFellow(filter.getFilterId());
                    digitRangeContainer.addEventListener(FilterDigitRangeBox.ON_DIGIT_RANGE_CHANGE, new EventListener<FilterDigitRangeBox.DigitRangeFilterChangeEvent>() {
                        public void onEvent(FilterDigitRangeBox.DigitRangeFilterChangeEvent event) throws Exception {
                            digitRangeFilterChange(event.getFilterId(), event.getEnteredFromValue(), event.getEnteredTillValue());
                        }
                    });
                    digitRangeContainer.addEventListener(FilterDigitRangeBox.ON_DIGIT_RANGE_REMOVE, new EventListener<FilterDigitRangeBox.DigitRangeFilterChangeEvent>() {
                        public void onEvent(FilterDigitRangeBox.DigitRangeFilterChangeEvent event) throws Exception {
                            event.clearSetValue();
                            digitRangeFilterChange(event.getFilterId(), 0, 0);
                        }
                    });
                    break;
                case DATE_RANGE:
                    Component inputRangeContainer = getFellow(filter.getFilterId());
                    inputRangeContainer.addEventListener(FilterDateRangeBox.ON_DATE_RANGE_CHANGE, new EventListener<FilterDateRangeBox.DateRangeFilterChangeEvent>() {
                        public void onEvent(FilterDateRangeBox.DateRangeFilterChangeEvent event) throws Exception {
                            dateRangeFilterChange(event.getFilterId(), event.getEnteredFromValue(), event.getEnteredTillValue());
                        }
                    });
                    inputRangeContainer.addEventListener(FilterDateRangeBox.ON_DATE_RANGE_REMOVE, new EventListener<FilterDateRangeBox.DateRangeFilterChangeEvent>() {
                        public void onEvent(FilterDateRangeBox.DateRangeFilterChangeEvent event) throws Exception {
                            event.clearSetValue();
                            dateRangeFilterChange(event.getFilterId(), null, null);
                        }
                    });
                    break;
                case TIME_RANGE:
                    Component inputTimeRangeContainer = getFellow(filter.getFilterId());
                    inputTimeRangeContainer.addEventListener(FilterTimeRangeBox.ON_TIME_RANGE_CHANGE, new EventListener<FilterTimeRangeBox.TimeRangeFilterChangeEvent>() {
                        public void onEvent(FilterTimeRangeBox.TimeRangeFilterChangeEvent event) throws Exception {
                            timeRangeFilterChange(event.getFilterId(), event.getEnteredFromValue(), event.getEnteredTillValue());
                        }
                    });
                    inputTimeRangeContainer.addEventListener(FilterTimeRangeBox.ON_TIME_RANGE_REMOVE, new EventListener<FilterTimeRangeBox.TimeRangeFilterChangeEvent>() {
                        public void onEvent(FilterTimeRangeBox.TimeRangeFilterChangeEvent event) throws Exception {
                            event.clearSetValue();
                            timeRangeFilterChange(event.getFilterId(), null, null);
                        }
                    });
            }
        }
    }

    private boolean checkExist(FilterTextBox.TextFilterEditEvent event) {
        boolean exist = false;
        Filter filter = getFilterById(event.getFilterId());
        if (filter.getFilterChildForValue(event.getEnteredValue()) != null) {
            exist = true;
        }
        return exist;
    }

    private boolean checkExist(FilterComboBox.TextFilterEditEvent event) {
        boolean exist = false;
        Filter filter = getFilterById(event.getFilterId());
        if (filter.getFilterChildForValue(event.getEnteredValue()) != null) {
            exist = true;
        }
        return exist;
    }
    public void removeSpecificFilter(Filter filter, String filterValue) {
        String filterId = filter.getFilterId();
        switch (filter.getFilterType().getInputType()) {
            case TEXT:
                Component textFilterContainer = getFellow(filterId);
                try {
                    for (Component component : textFilterContainer.getChildren()) {
                        FilterTextBox filterBox = (FilterTextBox) component;
                        if (filterBox.getValue() == filterValue) {
                            textFilterContainer.removeChild(filterBox);
                        }
                    }
                } finally {
                    textFilterChange(filterId, filterValue, "");
                    break;
                }
            case COMBO:
                Component comboFilterContainer = getFellow(filterId);
                try {
                    for (Component component : comboFilterContainer.getChildren()) {
                    	FilterComboBox filterBox = (FilterComboBox) component;
                        if (filterBox.getValue() == filterValue) {
                        	comboFilterContainer.removeChild(filterBox);
                        }
                    }
                } finally {
                    textFilterChange(filterId, filterValue, "");
                    break;
                }
            case DIGIT_RANGE:
                FilterDigitRangeBox digitRangeBox = (FilterDigitRangeBox) getFellow(filterId);
                digitRangeBox.cleanFilter();
                digitRangeFilterChange(filterId, FilterDigitRangeBox.EMPTY_VALUE, FilterDigitRangeBox.EMPTY_VALUE);
                break;
            case DATE_RANGE:
                FilterDateRangeBox dateRangeBox = (FilterDateRangeBox) getFellow(filterId);
                dateRangeBox.cleanFilter();
                dateRangeFilterChange(filterId, null, null);
                break;
            case TIME_RANGE:
                FilterTimeRangeBox timeRangeBox = (FilterTimeRangeBox) getFellow(filterId);
                timeRangeBox.cleanFilter();
                timeRangeFilterChange(filterId, null, null);
                break;
            case OPTIONS:
                FilterChild filterChild = filter.getFilterChildForValue(filterValue);
                FilterCheckBox checkbox = (FilterCheckBox) getFellow(filterChild.getId());
                checkbox.removeFilter();
                optionFilterChange(filterId, filterChild.getId(), false);
                break;
        }
        publishChangeFilterEvent();
    }

    public void textFilterChange(String filterId, String oldValue, String newValue) {
        isFilterChanged = true;
        Filter filter = getFilterById(filterId);
        if (!oldValue.isEmpty()) {
            filter.removeFilterChild(filter.getFilterChildForValue(oldValue));
        }
        String filterValue = filter.getSetValue();
        if (!filterValue.isEmpty() && !newValue.isEmpty()) {
            filterValue = filterValue + SQLCMConstants.FILTER_VALUES_SEP;
        }
        filterValue = filterValue + newValue;

        filter.setSetValue(filterValue);
        updateFilterRemoveLnk(filter);
        finishUpFilterChangeProcess();
    }

    public void dateRangeFilterChange(String filterId, Date valueFrom, Date valueTill) {
        isFilterChanged = true;
        Filter filter = getFilterById(filterId);
        if (valueFrom == null && valueTill == null) {
            filter.setSetValue("");
        } else {
            SimpleDateFormat dateFormat = new SimpleDateFormat(SQLCMConstants.FILTER_DATE_FORMAT);
            filter.setSetValue(String.valueOf(dateFormat.format(valueFrom) + SQLCMConstants.FILTER_RANGE_VALUES_SEP
                    + dateFormat.format(valueTill)));
        }
        updateFilterRemoveLnk(filter);
        finishUpFilterChangeProcess();
    }

    public void timeRangeFilterChange(String filterId, Date valueFrom, Date valueTill) {
        isFilterChanged = true;
        Filter filter = getFilterById(filterId);
        if (valueFrom == null && valueTill == null) {
            filter.setSetValue("");
        } else {
            SimpleDateFormat dateFormat = new SimpleDateFormat(SQLCMConstants.FILTER_TIME_FORMAT);
            filter.setSetValue(String.valueOf(dateFormat.format(valueFrom) + SQLCMConstants.FILTER_RANGE_VALUES_SEP
                + dateFormat.format(valueTill)));
        }
        updateFilterRemoveLnk(filter);
        finishUpFilterChangeProcess();
    }

    public void digitRangeFilterChange(String filterId, int valueFrom, int valueTill) {
        isFilterChanged = true;
        Filter filter = getFilterById(filterId);
        // TODO IR FIXED IDSEC-623 'Number of audited DB' are not filtered by 0
//        if (valueFrom == 0 & valueTill == 0) {
//            filter.setSetValue("");
//        } else {
//            filter.setSetValue(String.valueOf(valueFrom + SQLCMConstants.FILTER_RANGE_VALUES_SEP + valueTill));
//        }
        filter.setSetValue(String.valueOf(valueFrom + SQLCMConstants.FILTER_RANGE_VALUES_SEP + valueTill));
        updateFilterRemoveLnk(filter);
        finishUpFilterChangeProcess();
    }


    public void optionFilterChange(String filterId, String filterChildId, boolean isChecked) {
        isFilterChanged = true;
        Filter filter = getFilterById(filterId);
        for (FilterChild filterChild : filter.getFilterChildren()) {
            if (filterChild.getId().equals(filterChildId)) {
                filterChild.setChecked(isChecked);
                FilterCheckBox filterBox = (FilterCheckBox) getFellow(filterChildId);
                if (isChecked) {
                    filterBox.setChecked(true);
                } else {
                    filterBox.setChecked(false);
                    filter.removeChildFromSetValue(filterChild);
                }
                updateFilterRemoveLnk(filter);
                break;
            }
        }
        finishUpFilterChangeProcess();
    }

    private void updateFilterRemoveLnk(Filter filter) {
        Component removeFilterCtrl = getFellow(REMOVE_FILTER_LNK_ID_PREFIX + filter.getFilterId());
        removeFilterCtrl.setVisible(filter.isValueSet());
        isAnyFilterSet = filter.isValueSet();
        isAnyFilterSet();
    }

    public void finishUpFilterChangeProcess() {
        if (isUpdateFilterImmediatelyMode()) {
            publishChangeFilterEvent();
        } else {
            applyFilterAfterChanges.getParent().setVisible(isFilterChanged);
            clearWholeFilter.setVisible(isAnyFilterSet);
        }
    }

    private void publishChangeFilterEvent() {
        EventQueue<Event> eq = EventQueues.lookup(CHANGE_FILTER_EVENT, EventQueues.SESSION, false);
        if (eq != null) {
            eq.publish(new Event("onClick", null, gatherFilterData()));
            isFilterChanged = false;
            applyFilterAfterChanges.getParent().setVisible(isFilterChanged);
            clearWholeFilter.setVisible(isAnyFilterSet);
        }
    }

    private FilterData gatherFilterData() {
        if (filters == null) return null;
        FilterData filterData = new FilterData();
        filterData.setSource(source);
        String filterValue;
        for (Filter filter : filters) {
            filterValue = filter.getSetValue();
            if (filterValue != null && filterValue.length() > 0) {
                filterData.put(filter.getColumnId(), filterValue);
            }
        }
        return filterData;
    }

    private void filterUpdateModeChangeHandler(CheckEvent event) {
        applyFilterOnChange.setChecked(event.isChecked());
        applyFilterAfterChanges.getParent().setVisible(!applyFilterOnChange.isChecked() && isFilterChanged);
        clearWholeFilter.setVisible(isAnyFilterSet);
        finishUpFilterChangeProcess();
    }

    private boolean isUpdateFilterImmediatelyMode() {
        return applyFilterOnChange.isChecked();
    }

    private void clearAllFilters() {
        for (Filter filter : filters) {
            clearSingleFilter(filter);
        }
        clearWholeFilter.setVisible(isAnyFilterSet);
        finishUpFilterChangeProcess();
        Executions.sendRedirect("");
    }

    public void clearFilterAfterLnkClick(MouseEvent event) {
        String clearFilterLnkId = event.getTarget().getId();
        String filterId = clearFilterLnkId.substring(REMOVE_FILTER_LNK_ID_PREFIX.length());

        for (Filter filter : filters) {
            if (filter.getFilterId().equals(filterId)) {
                clearSingleFilter(filter);
                isFilterChanged = true;
                finishUpFilterChangeProcess();
                break;
            }
        }
    }

    private void clearSingleFilter(Filter filter) {
        filter.setSetValue("");
        switch (filter.getFilterType().getInputType()) {
            case TEXT:
                Component textFilterContainer = getFellow(filter.getFilterId());
                try {
                    FilterTextBox filterBox = (FilterTextBox) textFilterContainer.getChildren().get(0);
                    while (!filterBox.getIsDefault()) {
                        textFilterContainer.removeChild(filterBox);
                        filterBox = (FilterTextBox) textFilterContainer.getChildren().get(0);
                    }
                    filterBox.setValue("");
                } finally {
                    break;
                }
            case COMBO:
                Component comboFilterContainer = getFellow(filter.getFilterId());
                try {
                    FilterComboBox filterBox = (FilterComboBox) comboFilterContainer.getChildren().get(0);
                    while (!filterBox.getIsDefault()) {
                    	comboFilterContainer.removeChild(filterBox);
                        filterBox = (FilterComboBox) comboFilterContainer.getChildren().get(0);
                    }
                    filterBox.setValue("");
                } finally {
                    break;
                }                
            case DIGIT_RANGE:
                FilterDigitRangeBox digitRangeBox = (FilterDigitRangeBox) getFellow(filter.getFilterId());
                digitRangeBox.cleanFilter();
                break;
            case DATE_RANGE:
                FilterDateRangeBox dateRangeBox = (FilterDateRangeBox) getFellow(filter.getFilterId());
                dateRangeBox.cleanFilter();
                break;
            case TIME_RANGE:
                FilterTimeRangeBox timeRangeBox = (FilterTimeRangeBox) getFellow(filter.getFilterId());
                timeRangeBox.cleanFilter();
                break;
            case OPTIONS:
                for (FilterChild filterChild : filter.getFilterChildren()) {
                    FilterCheckBox checkbox = (FilterCheckBox) getFellow(filterChild.getId());
                    checkbox.removeFilter();
                }
                break;
        }
        updateFilterRemoveLnk(filter);
    }

    private void isAnyFilterSet() {
        for (Filter filter : filters) {
            isAnyFilterSet = isAnyFilterSet || getFellow(REMOVE_FILTER_LNK_ID_PREFIX + filter.getFilterId()).isVisible();
        }
    }

    private Filter getFilterById(String filterId) {
        for (Filter filter : filters) {
            if (filter.getFilterId().equals(filterId)) {
                return filter;
            }
        }
        return null;
    }
}
