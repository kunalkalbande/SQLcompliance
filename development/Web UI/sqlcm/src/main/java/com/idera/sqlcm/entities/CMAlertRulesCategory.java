package com.idera.sqlcm.entities;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMAlertRulesCategory {
    @JsonProperty("name")
    private String name;

    @JsonProperty("category")
    private String category;
    

	public String getName() {
		return name;
	}


	public void setName(String name) {
		this.name = name;
	}


	public String getCategory() {
		return category;
	}


	public void setCategory(String category) {
		this.category = category;
	}


	public CMAlertRulesCategory() {
    }
	
    @Override
    public String toString() {
        return "CMAlertRulesCondition{" +
            ", name=" + name +
            ", category='" + category + '\'' +
            '}';
    }
}
