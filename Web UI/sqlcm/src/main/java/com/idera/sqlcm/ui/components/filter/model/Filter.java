package com.idera.sqlcm.ui.components.filter.model;

import com.idera.sqlcm.utils.SQLCMConstants;
import org.zkoss.zul.ListModelList;

public class Filter {
    protected String filterId;
    protected FilterType filterType;
    protected String filterName;
    protected String columnId;
    private ListModelList<FilterChild> filterChildren;
    private String setValue = "";

    public Filter() {
        this.filterChildren = new ListModelList<>();
    }

    public Filter(String filterId, FilterType filterType, String filterName, String columnId) {
        this.filterId = filterId;
        this.filterType = filterType;
        this.filterName = filterName;
        this.columnId = columnId;
        this.filterChildren = new ListModelList<>();
    }

    public String getFilterId() {
        return filterId;
    }

    public String getFilterName() {
        return filterName;
    }

    public FilterType getFilterType() {
        return filterType;
    }

    public String getColumnId() {
        return columnId;
    }

    public ListModelList<FilterChild> getFilterChildren() {
        return filterChildren;
    }

    public void addFilterChild(FilterChild filterChild) {
        this.filterChildren.add(filterChild);
    }

    public void removeChildFromSetValue(FilterChild filterChild) {
        getSetValue().replace(filterChild + SQLCMConstants.FILTER_VALUES_SEP, "");
    }

    public void removeFilterChild(FilterChild filterChild) {
        if (filterChild != null && !filterType.getInputType().equals(FilterInputType.OPTIONS))
            this.filterChildren.remove(filterChild);
    }

    public String[] getTextValues() {
        return getSetValue().split(SQLCMConstants.FILTER_VALUES_SEP_REGEXP);
    }

    public String getSetValue() {
        StringBuilder optionsValue = new StringBuilder();
        switch (getFilterType().getInputType()) {
            case OPTIONS:
                for (FilterChild filterChild : filterChildren) {
                    if (filterChild.isChecked()) {
                        if (optionsValue.length() > 0) {
                            optionsValue.append(SQLCMConstants.FILTER_VALUES_SEP);
                        }
                        optionsValue.append(filterChild.getIntValue());
                    }
                }
                return optionsValue.toString();
            case TEXT:
                for (FilterChild filterChild : filterChildren) {
                    if (optionsValue.length() > 0) {
                        optionsValue.append(SQLCMConstants.FILTER_VALUES_SEP);
                    }
                    optionsValue.append(filterChild.getValue());
                }
                return optionsValue.toString();
            case COMBO:
                for (FilterChild filterChild : filterChildren) {
                    if (optionsValue.length() > 0) {
                        optionsValue.append(SQLCMConstants.FILTER_VALUES_SEP);
                    }
                    optionsValue.append(filterChild.getValue());
                }
                return optionsValue.toString();
            case TIME_RANGE:
            case DATE_RANGE:
                for (FilterChild filterChild : filterChildren) {
                    if (optionsValue.length() > 0) {
                        optionsValue.append(SQLCMConstants.FILTER_RANGE_VALUES_SEP);
                    }
                    optionsValue.append(filterChild.getValue());
                }
                return optionsValue.toString();
            case DIGIT_RANGE:
                for (FilterChild filterChild : filterChildren) {
                    if (optionsValue.length() > 0) {
                        optionsValue.append(SQLCMConstants.FILTER_RANGE_VALUES_SEP);
                    }
                    optionsValue.append(filterChild.getValue());
                }
                return optionsValue.toString();
            default:
                return setValue;
        }
    }

    public void setSetValue(String setValue) {
        switch (getFilterType().getInputType()) {
            case OPTIONS:
                if (setValue.length() > 0) {
                    for (String childVal : setValue.split(SQLCMConstants.FILTER_VALUES_SEP_REGEXP)) {
                        for (FilterChild filterChild : filterChildren) {
                            if (filterChild.getValue().equals(childVal)) {
                                filterChild.setChecked(true);
                            }
                            if (filterChild.getIntValue() == Integer.valueOf(childVal)) {
                                filterChild.setChecked(true);
                            }
                        }
                    }
                } else {
                    for (FilterChild filterChild : filterChildren) {
                        filterChild.setChecked(false);
                    }
                }
                break;
            case TEXT:
                this.filterChildren = new ListModelList<>();
                this.setValue = setValue;
                if (setValue.length() > 0) {
                    int i = 0;
                    for (String childVal : setValue.split(SQLCMConstants.FILTER_VALUES_SEP_REGEXP)) {
                        filterChildren.add(new FilterChild(getFilterId() + "_" + i++, childVal, -1, childVal, true));
                    }
                }
                break;
            case COMBO:
                this.filterChildren = new ListModelList<>();
                this.setValue = setValue;
                if (setValue.length() > 0) {
                    int i = 0;
                    for (String childVal : setValue.split(SQLCMConstants.FILTER_VALUES_SEP_REGEXP)) {
                        filterChildren.add(new FilterChild(getFilterId() + "_" + i++, childVal, -1, childVal, true));
                    }
                }
                break;
            case DIGIT_RANGE:
            case TIME_RANGE:
            case DATE_RANGE:
                this.filterChildren = new ListModelList<>();
                this.setValue = setValue;
                String[] values =  setValue.split(SQLCMConstants.FILTER_RANGE_VALUES_SEP_REGEXP);
                if (values.length == 2) {
                    String from = values[0];
                    String to = values[1];
                    filterChildren.add(new FilterChild(getFilterId() + "_From", from, 1, from, true));
                    filterChildren.add(new FilterChild(getFilterId() + "_To", to, 2, to, true));
                }
            default:
                this.setValue = setValue;
                break;
        }
    }

    public boolean isValueSet() {
        String currentValue = this.getSetValue();
        return (currentValue != null && currentValue.length() > 0);
    }

    public FilterChild getFilterChildForValue(String value) {
        for (FilterChild filterChild : filterChildren) {
            if (filterChild.getValue().equals(value))
                return filterChild;
        }
        return null;
    }

    public FilterChild getFilterChildForIntValue(String intValue) {
        for (FilterChild filterChild : filterChildren) {
            if (filterChild.getIntValue() == Integer.valueOf(intValue))
                return filterChild;
        }
        return null;
    }

    @Override
    public String toString() {
        return "Filter{" +
                "filterId='" + filterId + '\'' +
                ", filterType=" + filterType +
                ", columnId='" + columnId + '\'' +
                ", filterChildren=" + filterChildren +
                ", setValue='" + setValue + '\'' +
                '}';
    }
}
