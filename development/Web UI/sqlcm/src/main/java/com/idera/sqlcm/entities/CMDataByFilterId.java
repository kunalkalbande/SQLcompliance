package com.idera.sqlcm.entities;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CMDataByFilterId {

	@JsonProperty("filterId")
	long filterId;
	
	public long getFilterId() {
		return filterId;
	}

	public void setFilterId(long filterId) {
		this.filterId = filterId;
	}
	
	@Override
	public String toString() {
		return "CMDataByFilterIdRequest{" +
				"filterId=" + filterId +
				'}';
   }
}
