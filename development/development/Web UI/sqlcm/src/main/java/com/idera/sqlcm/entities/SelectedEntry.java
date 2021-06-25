package com.idera.sqlcm.entities;

public class SelectedEntry {
    private String name;
    private Object data = null;
    private boolean isSelected = false;

    public SelectedEntry(){    }

    public SelectedEntry(String name, boolean isSelected){
        this.name = name;
        this.isSelected = isSelected;
    }

    public SelectedEntry(String name, boolean isSelected, Object data){
        this.name = name;
        this.isSelected = isSelected;
        this.data = data;
    }

    public String getName(){
        return name;
    }

    public void setName(String name){
        this.name = name;
    }

    public boolean getIsSelected(){
        return isSelected;
    }

    public void setIsSelected (boolean isSelected){
        this.isSelected = isSelected;
    }

    public Object getData(){
        return data;
    }

    public void setData(String data){
        this.data = data;
    }
}
