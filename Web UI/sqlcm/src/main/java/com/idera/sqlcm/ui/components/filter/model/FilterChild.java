package com.idera.sqlcm.ui.components.filter.model;

public class FilterChild {
    protected String id;
    protected String value;
    protected int intValue;
    protected String label;
    protected boolean checked;

    public FilterChild() {

    }

    public FilterChild(String id, String value, int intValue, String label, boolean checked) {
        this.id = id;
        this.value = value;
        this.intValue = intValue;
        this.label = label;
        this.checked = checked;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        this.value = value;
    }

    public int getIntValue() {
        return intValue;
    }

    public void setIntValue(int intValue) {
        this.intValue = intValue;
    }

    public String getLabel() {
        return label;
    }

    public void setLabel(String label) {
        this.label = label;
    }

    public boolean isChecked() {
        return checked;
    }

    public void setChecked(boolean isChecked) {
        this.checked = isChecked;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        FilterChild that = (FilterChild) o;

        if (checked != that.checked) return false;
        if (intValue != that.intValue) return false;
        if (!id.equals(that.id)) return false;
        if (!label.equals(that.label)) return false;
        if (!value.equals(that.value)) return false;

        return true;
    }

    @Override
    public int hashCode() {
        int result = id.hashCode();
        result = 31 * result + value.hashCode();
        result = 31 * result + intValue;
        result = 31 * result + label.hashCode();
        result = 31 * result + (checked ? 1 : 0);
        return result;
    }

    @Override
    public String toString() {
        return "FilterChild{" +
                "id='" + id + '\'' +
                ", value='" + value + '\'' +
                ", intValue='" + intValue + '\'' +
                ", checked=" + checked +
                '}';
    }
}
