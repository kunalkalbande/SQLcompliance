package com.idera.sqlcm.entities;

import com.fasterxml.jackson.annotation.*;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@JsonIgnoreProperties(ignoreUnknown = true)
@JsonPropertyOrder({
	"key",
	"value"
})
public class StatisticDataResponse {

	@JsonProperty("key")
	private Integer key;
	@JsonProperty("value")
	private List<StatisticData.Statistic> value = new ArrayList<StatisticData.Statistic>();
	@JsonIgnore
	private Map<String, Object> additionalProperties = new HashMap<String, Object>();

	/**
	 *
	 * @return
	 * The key
	 */
	@JsonProperty("key")
	public Integer getKey() {
		return key;
	}

	/**
	 *
	 * @param key
	 * The key
	 */
	@JsonProperty("key")
	public void setKey(Integer key) {
		this.key = key;
	}

	/**
	 *
	 * @return
	 * The value
	 */
	@JsonProperty("value")
	public List<StatisticData.Statistic> getValue() {
		return value;
	}

	/**
	 *
	 * @param value
	 * The value
	 */
	@JsonProperty("value")
	public void setValue(List<StatisticData.Statistic> value) {
		this.value = value;
	}

	@JsonAnyGetter
	public Map<String, Object> getAdditionalProperties() {
		return this.additionalProperties;
	}

	@JsonAnySetter
	public void setAdditionalProperty(String name, Object value) {
		this.additionalProperties.put(name, value);
	}

}