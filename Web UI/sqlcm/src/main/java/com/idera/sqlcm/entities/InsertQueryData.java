package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class InsertQueryData {

    @JsonProperty("dataQuery")
    String dataQuery;
    
    public String getDataQuery() {
		return dataQuery;
	}

	public void setDataQuery(String dataQuery) {
		this.dataQuery = dataQuery;
	}

	@Override
    public String toString() {
        return "InsertQueryData{" +
            "dataQuery=" + dataQuery +
     '}';
    }
}
  
    