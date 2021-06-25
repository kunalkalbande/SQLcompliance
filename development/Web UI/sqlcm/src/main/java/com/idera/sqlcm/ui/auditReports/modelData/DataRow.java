package com.idera.sqlcm.ui.auditReports.modelData;

public class DataRow {
	String name;
	boolean isHeader=false;
	boolean checked;
	boolean isSame;
	boolean isGreyed =false ;
	boolean isVisible=true;
	boolean isCheckBox;
	
	void setData(boolean v, boolean flag){
		checked=v;
		isSame=flag;
		isGreyed =false;
		isVisible=true;
	}
	void setData(boolean v, boolean flag, boolean isgreyed){
		checked=v;
		isSame=flag;
		isGreyed =isgreyed;
		isVisible=true;
	}
	void setData(boolean v, boolean flag, boolean isgreyed, boolean isVisible ){
		checked=v;
		isSame=flag;
		isGreyed =isgreyed;
		this.isVisible=isVisible;
	}
	public boolean getChecked() {
		return checked;
	}
	public void setChecked(boolean isChecked) {
		this.checked = isChecked;
	}
	public boolean getIsSame() {
		return isSame;
	}
	public void setIsSame(boolean isSame) {
		this.isSame = isSame;
	}
	public boolean getIsGreyed() {
		return isGreyed;
	}
	public void setIsGreyed(boolean isGreyed) {
		this.isGreyed = isGreyed;
	}
	
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public boolean getIsHeader() {
		return isHeader;
	}
	public void setIsHeader(boolean isHeader) {
		this.isHeader = isHeader;
	}
	public boolean getIsVisible() {
		return isVisible;
	}
	public void setIsVisible(boolean isVisible) {
		this.isVisible = isVisible;
	}
	public boolean getIsCheckBox() {
		return isCheckBox;
	}
	public void setIsCheckBox(boolean isCheckBox) {
		this.isCheckBox = isCheckBox;
	}
	
}