package com.idera.sqlcm.entities;
import com.fasterxml.jackson.annotation.JsonProperty;

public class CategoryData {
	@JsonProperty("name")
    private String name;
	
	@JsonProperty("evtypeid")
    private int evtypeid;
	
	private boolean checkBool;

	public boolean isCheckBool() {
		return checkBool;
	}

	public void setCheckBool(boolean checkBool) {
		this.checkBool = checkBool;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public int getEvtypeid() {
		return evtypeid;
	}

	public void setEvtypeid(int evtypeid) {
		this.evtypeid = evtypeid;
	}
	
	@Override
    public String toString() {
        return "CategoryData{" +
            "name=" + name +
             "evtypeid=" + evtypeid +
            '}';
    }
}
