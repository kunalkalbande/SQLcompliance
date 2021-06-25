package com.idera.sqlcm.entities;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CategoryResponse {
	@JsonProperty("categoryTable")
    private List<CategoryData> categoryTable;

	public List<CategoryData> getCategoryTable() {
		return categoryTable;
	}

	public void setCategoryTable(List<CategoryData> categoryTable) {
		this.categoryTable = categoryTable;
	}
	
	public CategoryResponse()
    {}

	
	 @Override
	    public String toString() {
	        return "CategoryResponse{" +
	            "categoryTable=" + categoryTable +
	            '}';
	    }
}
